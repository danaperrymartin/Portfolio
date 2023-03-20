# -*- coding: utf-8 -*-
"""
Created on Tue May 10 14:32:23 2022

@author: Dana
"""

import numpy as np
import tensorflow as tf
import time
import tensorflow.keras.layers as KE
import tensorflow.keras.layers as KL

from tensorflow.keras.models import load_model
from tensorflow.python import keras
from tensorflow.python.keras import backend as K

def log2_graph(x):
    """Implementation of Log2. TF doesn't have a native implementation."""
    return tf.math.log(x) / tf.math.log(2.0)

def parse_image_meta_graph(meta):
    """Parses a tensor that contains image attributes to its components.
    See compose_image_meta() for more details.

    meta: [batch, meta length] where meta length depends on NUM_CLASSES

    Returns a dict of the parsed tensors.
    """
    image_id = meta[:, 0]
    original_image_shape = meta[:, 1:2]
    image_shape = meta[:, 3:5]
    window = meta[:, 5:9]  # (y1, x1, y2, x2) window of image in in pixels
    scale = meta[:, 9]
    active_class_ids = meta[:, 10:]
    return {
        "image_id": image_id,
        "original_image_shape": original_image_shape,
        "image_shape": image_shape,
        "window": window,
        "scale": scale,
        "active_class_ids": active_class_ids,
    }

class PyramidROIAlign(KE.Layer):
    """Implements ROI Pooling on multiple levels of the feature pyramid.

    Params:
    - pool_shape: [pool_height, pool_width] of the output pooled regions. Usually [7, 7]

    Inputs:
    - boxes: [batch, num_boxes, (y1, x1, y2, x2)] in normalized
             coordinates. Possibly padded with zeros if not enough
             boxes to fill the array.
    - image_meta: [batch, (meta data)] Image details. See compose_image_meta()
    - feature_maps: List of feature maps from different levels of the pyramid.
                    Each is [batch, height, width, channels]

    Output:
    Pooled regions in the shape: [batch, num_boxes, pool_height, pool_width, channels].
    The width and height are those specific in the pool_shape in the layer
    constructor.
    """

    def __init__(self, pool_shape, **kwargs):
        super(PyramidROIAlign, self).__init__(**kwargs)
        self.pool_shape = tuple(pool_shape)

    def get_config(self):
        config = super(PyramidROIAlign, self).get_config()
        config['pool_shape'] = self.pool_shape
        return config

    def call(self, inputs):
        # Crop boxes [batch, num_boxes, (y1, x1, y2, x2)] in normalized coords
        boxes = inputs[0][2]

        # Image meta
        # Holds details about the image. See compose_image_meta()
        image_meta = np.array(inputs[1])

        # Feature Maps. List of feature maps from different level of the
        # feature pyramid. Each is [batch, height, width, channels]
        feature_maps = inputs[2:]

        # Assign each ROI to a level in the pyramid based on the ROI area.
        y1, x1, y2, x2 = tf.split(boxes, 4, axis=1)
        h = y2 - y1
        w = x2 - x1
        # Use shape of first image. Images in a batch must have the same size.
        image_shape = parse_image_meta_graph(np.array(image_meta))['image_shape']
        # Equation 1 in the Feature Pyramid Networks paper. Account for
        # the fact that our coordinates are normalized here.
        # e.g. a 224x224 ROI (in pixels) maps to P4
        image_area = tf.cast(image_shape[0] * image_shape[1], tf.float32)
        roi_level = log2_graph(tf.sqrt(tf.cast(h, tf.float32) * tf.cast(w,tf.float32)) / (224.0 / tf.sqrt(image_area)))
        roi_level = tf.minimum(5, tf.maximum(
            2, 4 + tf.cast(tf.round(roi_level), tf.int32)))
        # roi_level = tf.squeeze(roi_level, 2)

        # Loop through levels and apply ROI pooling to each. P2 to P5.
        pooled = []
        box_to_level = []
        for i, level in enumerate(range(2, 6)):
            ix = tf.compat.v1.where(tf.equal(roi_level, level))
            if(len(ix)>0):
                level_boxes = tf.gather(boxes, ix)
    
                # Box indices for crop_and_resize.
                box_indices = tf.cast(ix[:, 0], tf.int32)
    
                # Keep track of which box is mapped to which level
                box_to_level.append(ix)
    
                # Stop gradient propogation to ROI proposals
                level_boxes = tf.stop_gradient(level_boxes)
                box_indices = tf.stop_gradient(box_indices)
    
                # Crop and Resize
                # From Mask R-CNN paper: "We sample four regular locations, so
                # that we can evaluate either max or average pooling. In fact,
                # interpolating only a single value at each bin center (without
                # pooling) is nearly as effective."
                #
                # Here we use the simplified approach of a single value per bin,
                # which is how it's done in tf.crop_and_resize()
                # Result: [batch * num_boxes, pool_height, pool_width, channels]
                pooled.append(tf.image.crop_and_resize(
                    np.expand_dims(np.expand_dims(np.array(feature_maps[i])[:,:,0],0),-1), tf.reshape(np.array(level_boxes)[:,0,:][0],(1,4)), tf.reshape(np.array(box_indices)[0],(1)), 
                    self.pool_shape,method="bilinear"))

        # Pack pooled features into one tensor
        pooled = tf.concat(pooled, axis=0)

        # Pack box_to_level mapping into one array and add another
        # column representing the order of pooled boxes
        box_to_level = tf.concat(box_to_level, axis=0)
        box_range = tf.expand_dims(tf.range(tf.shape(input=box_to_level)[0]), 1)
        box_to_level = tf.concat([tf.cast(box_to_level, tf.int32), box_range],axis=1)

        # Rearrange pooled features to match the order of the original boxes
        # Sort box_to_level by batch then box index
        # TF doesn't have a way to sort by two columns, so merge them and sort.
        sorting_tensor = box_to_level[:, 0] * 100000 + box_to_level[:, 1]
        ix = tf.nn.top_k(sorting_tensor, k=tf.shape(
            input=box_to_level)[0]).indices[::-1]
        ix = tf.gather(box_to_level[:, 2], ix)
        pooled = tf.gather(pooled, ix)

        # Re-add the batch dimension
        shape = tf.concat([tf.shape(input=boxes)[:2], tf.shape(input=pooled)[1:]], axis=0)
        # pooled = tf.reshape(pooled, shape)
        pooled = np.expand_dims(pooled,axis=-1)
        return pooled

class BatchNorm(KL.BatchNormalization):
    """Extends the Keras BatchNormalization class to allow a central place
    to make changes if needed.

    Batch normalization has a negative effect on training if batches are small
    so this layer is often frozen (via setting in Config class) and functions
    as linear layer.
    """
    def call(self, inputs, training=None):
        """
        Note about training values:
            None: Train BN layers. This is the normal mode
            False: Freeze BN layers. Good when batch size is small
            True: (don't use). Set layer in training mode even when making inferences
        """
        return super(self.__class__, self).call(inputs, training=training)

class ObjectLocalizer ( object ) :

	def fpn_classifier_graph(rois, feature_maps, image_meta,pool_size, num_classes, train_bn=True,fc_layers_size=1024):
		"""Builds the computation graph of the feature pyramid network classifier
		and regressor heads.
		
		rois: [batch, num_rois, (y1, x1, y2, x2)] Proposal boxes in normalized
		      coordinates.
		feature_maps: List of feature maps from different layers of the pyramid,
		              [P2, P3, P4, P5]. Each has a different resolution.
		image_meta: [batch, (meta data)] Image details. See compose_image_meta()
		pool_size: The width of the square feature map generated from ROI Pooling.
		num_classes: number of classes, which determines the depth of the results
		train_bn: Boolean. Train or freeze Batch Norm layers
		fc_layers_size: Size of the 2 FC layers
		
		Returns:
		    logits: [batch, num_rois, NUM_CLASSES] classifier logits (before softmax)
		    probs: [batch, num_rois, NUM_CLASSES] classifier probabilities
		    bbox_deltas: [batch, num_rois, NUM_CLASSES, (dy, dx, log(dh), log(dw))] Deltas to apply to
		                 proposal boxes
		"""
		# ROI Pooling
		# Shape: [batch, num_rois, POOL_SIZE, POOL_SIZE, channels]
		x = PyramidROIAlign([pool_size, pool_size],
		                    name="roi_align_classifier")([rois, image_meta] + feature_maps)
		# Two 1024 FC layers (implemented with Conv2D for consistency)
		x = KL.TimeDistributed(KL.Conv2D(fc_layers_size, (pool_size, pool_size), padding="valid"),
		                       name="mrcnn_class_conv1")(x)
		x = KL.TimeDistributed(BatchNorm(), name='mrcnn_class_bn1')(x, training=train_bn)
		x = KL.Activation('relu')(x)
		x = KL.TimeDistributed(KL.Conv2D(fc_layers_size, (1, 1)),
		                       name="mrcnn_class_conv2")(x)
		x = KL.TimeDistributed(BatchNorm(), name='mrcnn_class_bn2')(x, training=train_bn)
		x = KL.Activation('relu')(x)
		
		shared = KL.Lambda(lambda x: K.squeeze(K.squeeze(x, 3), 2),
		                   name="pool_squeeze")(x)
		
		# Classifier head
		mrcnn_class_logits = KL.TimeDistributed(KL.Dense(num_classes),
		                                        name='mrcnn_class_logits')(shared)
		mrcnn_probs = KL.TimeDistributed(KL.Activation("softmax"),
		                                 name="mrcnn_class")(mrcnn_class_logits)
		
		# BBox head
		# [batch, num_rois, NUM_CLASSES * (dy, dx, log(dh), log(dw))]
		x = KL.TimeDistributed(KL.Dense(num_classes * 4, activation='linear'),
		                       name='mrcnn_bbox_fc')(shared)
		# Reshape to [batch, num_rois, NUM_CLASSES, (dy, dx, log(dh), log(dw))]
		s = K.int_shape(x)
		if s[1] is None:
		    mrcnn_bbox = KL.Reshape((-1, num_classes, 4), name="mrcnn_bbox")(x)
		else:
		    mrcnn_bbox = KL.Reshape((s[1], num_classes, 4), name="mrcnn_bbox")(x)
		
		return mrcnn_class_logits, mrcnn_probs, mrcnn_bbox


# 	def fit(X, Y, hyperparameters):
# 		initial_time = time.time()
        
# 		model = load_model("model.h5", compile=False)
# 		model.compile(optimizer='adam',
# 						loss='mse',
# 						metrics=['mean_squared_error', 'accuracy'])
# 		model.fit(X, Y,
# 						batch_size=hyperparameters['batch_size'],
# 						epochs=hyperparameters['epochs'],
# 						callbacks=hyperparameters['callbacks'],
# 						validation_data=hyperparameters['val_data']
# 						)
# 		final_time = time.time()
# 		eta = (final_time - initial_time)
# 		time_unit = 'seconds'
# 		if eta >= 60:
# 			eta = eta / 60
# 			time_unit = 'minutes'
# 		print('Elapsed time acquired for {} epoch(s) -> {} {}'.format(hyperparameters['epochs'], eta, time_unit))

	def evaluate(self, test_X, test_Y):
		return self.__model.evaluate(test_X, test_Y)

	def predict(self, X):
		predictions = self.__model.predict(X)
		return predictions

	def save_model(self, file_path):
		self.__model.save(file_path)

	def load_model(self, file_path):
		self.__model = keras.models.load_model(file_path)

	def load_model_weights(self , file_path ) :
		self.__model.load_weights( file_path )
        


