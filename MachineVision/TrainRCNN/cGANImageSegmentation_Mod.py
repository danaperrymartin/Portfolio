# -*- coding: utf-8 -*-
"""
Created on Wed Mar 23 12:34:12 2022

@author: Dana
"""

import tensorflow as tf
import matplotlib.pyplot as plt
import numpy as np
import os
import cv2
# import sklearn
import onnx
import onnxmltools
# import freighttraindataset
import pix2pixmodel
import unetmodel

import keras
import keras.backend as K

from tensorflow.python.keras.models import load_model
from tensorflow.python.ops import math_ops
from tensorflow.keras.losses import binary_crossentropy
from PIL import Image as im
from fastrcnn.MyLabelBinarizer import MyLabelBinarizer
from sklearn.preprocessing import MultiLabelBinarizer
# from tensorflow_examples.models.pix2pix import pix2pix
from IPython.display import clear_output
from keras.callbacks import ModelCheckpoint, EarlyStopping

directory_image = "./train_images/MaskData/images/"
directory_maskdefinitions = "./train_images/MaskData/imagemasks/"
directory_labeldefinitions = "./train_images/MaskData/imagelabels/"

file_name = []
label = []
segmentation_mask = []
species = []
image = []

dest_size = (256,512,1)

for e,i in enumerate(os.listdir(directory_maskdefinitions)):
    if(e<4):
        print(i)
        file_name.append(directory_image+i.split("_")[1][4:])
        # image.append(tf.image.convert_image_dtype(tf.image.resize(cv2.imread(directory_image+i.split("_")[1][4:],cv2.IMREAD_GRAYSCALE),(dest_size[0],dest_size[1])),dtype=tf.float32))
        # label.append(tf.image.convert_image_dtype(tf.image.resize(cv2.imread(directory_maskdefinitions+i,cv2.IMREAD_GRAYSCALE),(dest_size[0],dest_size[1])),dtype=tf.float32))
        
        image.append(tf.image.convert_image_dtype(np.expand_dims(cv2.resize(cv2.imread(directory_image+i.split("_")[1][4:],cv2.IMREAD_GRAYSCALE)/255,(dest_size[1],dest_size[0])),axis=2),dtype=tf.float32))
        label.append(tf.image.convert_image_dtype(np.expand_dims(cv2.resize(cv2.imread(directory_labeldefinitions+i.split("_")[0]+"_Label"+i.split("_")[1][4:],cv2.IMREAD_GRAYSCALE),(dest_size[1],dest_size[0])),axis=2),dtype=tf.float32))
        
        segmentation_mask.append(tf.image.convert_image_dtype(np.expand_dims(cv2.resize(cv2.imread(directory_maskdefinitions+i, cv2.IMREAD_GRAYSCALE)/255,(dest_size[1],dest_size[0])),axis=2),dtype=tf.float32)+label[e])
        species.append(tf.image.convert_image_dtype(np.expand_dims(cv2.resize(cv2.imread(directory_maskdefinitions+i)*0,(dest_size[1],dest_size[0])),axis=2),dtype=tf.float32))

file_name = [file_name[i] for i in range(len(file_name))]
image = np.array([image[i] for i in range(len(file_name))])

label = np.array([label[i] for i in range(len(file_name))])
label_onehot = tf.one_hot(label, depth=4) #(np.arange(label.max()) == label[...,None]-1).astype(int)

# label_binarizer = sklearn.preprocessing.LabelBinarizer()
# label_binarizer.fit(range(int(np.max(label)+1)))
# label = label_binarizer.transform(label[0])

segmentation_mask = np.array([segmentation_mask[i] for i in range(len(file_name))])

dataset = {'train': {'file_name':file_name,'image':image, 'label': label, 'segmentation_mask':segmentation_mask},
            'test': {'file_name':file_name,'image':image, 'label': label, 'segmentation_mask':segmentation_mask}}

# dataset = {'train':tfds.features.FeaturesDict({
#     'file_name': tf.string,
#     'image': tf.uint8,
#     'label': tf.int64,
#     'segmentation_mask':tf.uint8,
#     'species': tf.int64}),
#     'test':tfds.features.FeaturesDict({'file_name': tf.string,
#         'image': tf.uint8,
#         'label': tf.int64,
#         'segmentation_mask':tf.uint8,
#         'species': tf.int64})}
# info = []
# info.features =tfds.features.FeaturesDict({
#     'file_name': tfds.features.Text(),
#     'image': tfds.features.Image(shape=(None, None, 3), dtype=tf.uint8),
#     'label': tfds.features.ClassLabel(num_classes=2,names=['Truck','Spring']),
#     'segmentation_mask': tfds.features.Image(shape=(None, None, 1), dtype=tf.uint8),
#     'species': tfds.features.ClassLabel(num_classes=1,names=['FreightCar'])})

# _, info = tfds.load('oxford_iiit_pet:3.*.*', with_info=True)
# dataset, info = tfds.load('freighttraindataset:1.*.*', with_info = True)

class Augment(tf.keras.layers.Layer):
  def __init__(self, seed=42):
    super().__init__()
    # both use the same seed, so they'll make the same random changes.
    self.augment_inputs = tf.keras.layers.RandomFlip(mode="horizontal", seed=seed)
    self.augment_labels = tf.keras.layers.RandomFlip(mode="horizontal", seed=seed)

  def call(self, inputs, labels):
    inputs = self.augment_inputs(inputs)
    labels = self.augment_labels(labels)
    return inputs, labels

def normalize(input_image, input_mask):
  input_image = tf.cast(input_image, tf.float32)
  # input_mask -= 1
  return input_image, input_mask

def load_image(datapoint):
  input_image = tf.image.resize(datapoint['image'], dest_size)
  input_mask = tf.image.resize(datapoint['segmentation_mask'], dest_size)

  input_image, input_mask = normalize(input_image, input_mask)

  return input_image, input_mask

def display(display_list):
  plt.figure(figsize=(15, 15))
  title = ['Input Image', 'True Mask', 'Predicted Mask']
  for i in range(len(display_list)):
    plt.subplot(1, len(display_list), i+1)
    plt.title(title[i])
    plt.imshow(tf.keras.utils.array_to_img(display_list[i]))
    # plt.imshow(im.fromarray(display_list[i]))
    plt.axis('off')
  plt.show()

TRAIN_LENGTH =10#len(dataset['train']['image'])
BATCH_SIZE = 1
BUFFER_SIZE = 1
STEPS_PER_EPOCH = TRAIN_LENGTH // BATCH_SIZE

train_images = tf.data.Dataset.from_tensor_slices((dataset['train']['image'],dataset['train']['segmentation_mask']))
test_images = tf.data.Dataset.from_tensor_slices((dataset['test']['image'],dataset['test']['segmentation_mask']))

train_batches = (
    train_images
    .cache()
    .shuffle(BUFFER_SIZE)
    .batch(BATCH_SIZE)
    .repeat()
    .prefetch(buffer_size=tf.data.AUTOTUNE))

test_batches = test_images.batch(BATCH_SIZE)

for images, masks in train_batches.take(1):
    # try:
    sample_image, sample_mask = images[0], masks[0]
    # display([sample_image, sample_mask])
    # except: 
    #     sample_image, sample_mask = np.squeeze(images[0],axis=0), np.squeeze(masks[0],axis=0)
    #     display([sample_image, sample_mask])
    
OUTPUT_CLASSES = 2
   
p2p = pix2pixmodel.Pix2PixModel((dest_size[0],dest_size[1],dest_size[2]), OUTPUT_CLASSES)
model = p2p.pix2pixfromscratch()

# unet = unetmodel.UNETModel((dest_size[0],dest_size[1],dest_size[2]), OUTPUT_CLASSES)
# model = unet.unetfromscratch()

def custom_loss_function(y_true, y_pred):
    print("Computing loss...")
    print(y_true.shape)
    # calculating squared difference between target and predicted values 
    # loss = -1*(K.sum(tf.multiply(y_true,K.log(y_pred)),axis=2))  # (batch_size, 2)
    loss = tf.reduce_mean(K.square(y_pred*255-y_true*255))
    # multiplying the values with weights along batch dimension
    # summing both loss values along batch dimension 
    # loss = K.sum(loss, axis=-1)   # (batch_size,)
    return loss

class CategoricalTruePositives(keras.metrics.Metric):
    def __init__(self, name="categorical_true_positives", **kwargs):
        super(CategoricalTruePositives, self).__init__(name=name, **kwargs)
        self.true_positives = self.add_weight(name="ctp", initializer="zeros")

    def update_state(self, y_true, y_pred, sample_weight=None):
        y_pred = tf.reshape(tf.argmax(y_pred, axis=1), shape=(-1, 1))
        values = tf.cast(y_true, "int32") == tf.cast(y_pred, "int32")
        values = tf.cast(values, "float32")
        if sample_weight is not None:
            sample_weight = tf.cast(sample_weight, "float32")
            values = tf.multiply(values, sample_weight)
        self.true_positives.assign_add(tf.reduce_sum(values))

    def result(self):
        return self.true_positives

    def reset_state(self):
        # The state of the metric will be reset at the start of each epoch.
        self.true_positives.assign(0.0)

class CustomMSE(keras.losses.Loss):
    def __init__(self, regularization_factor=0, name="custom_mse"):
        super().__init__(name=name)
        self.regularization_factor = regularization_factor

    def call(self, y_true, y_pred):
        print(y_true.shape)
        print(y_pred.shape)
        mse = tf.square(y_true - y_pred)
        # mse = tf.math.reduce_mean(tf.square(y_true - y_pred))
        # reg = tf.math.reduce_mean(tf.square(0.5 - y_pred))
        # mse = tf.square(y_true - y_pred)
        # reg = tf.square(0.5 - y_pred)
        return mse
        # return mse + reg * self.regularization_factor

# model.load_weights("model.h5")

model.compile(optimizer=tf.optimizers.Adam(),
              loss= CustomMSE(),
              metrics=[tf.keras.metrics.CategoricalAccuracy(name="categorical_accuracy", dtype=tf.float32)])

# onnx_model = onnxmltools.convert_keras(model) 
# onnx.save_model(onnx_model, ("model.onnx"))
# tf.keras.utils.plot_model(model, show_shapes=True)

def create_mask(pred_mask):
  pred_mask = tf.argmax(pred_mask, axis=-1)
  pred_mask = pred_mask[..., tf.newaxis]
  return pred_mask[0]

def show_predictions(train_batches=None, num=np.random.randint(0,100)):
  if train_batches:
    print(num)
    for image, mask in train_batches.take(num):
      pred_mask = model.predict(image)
      
      display([image[0], mask[0], create_mask(pred_mask)])
  else:
     print(num)
     display([sample_image, sample_mask,create_mask(model.predict(sample_image[tf.newaxis, ...]))])

show_predictions()

class DisplayCallback(tf.keras.callbacks.Callback):
  def on_epoch_end(self, epoch, logs=None):
    clear_output(wait=True)
    show_predictions()
    print ('\nSample Prediction after epoch {}\n'.format(epoch+1))
    
EPOCHS = 4
VAL_SUBSPLITS = 2
VALIDATION_STEPS = len(dataset['test']['image'])

checkpoint = keras.callbacks.ModelCheckpoint(("model.h5"), monitor='categorical_accuracy', verbose=0, save_best_only=True, save_weights_only=True, mode='auto', save_freq='epoch')
early = EarlyStopping(monitor='categorical_accuracy', min_delta=0.000001, patience=100, verbose=1,restore_best_weights=True, mode='auto')

model_history = model.fit(train_batches.take(100), epochs=EPOCHS,
                          steps_per_epoch=STEPS_PER_EPOCH,
                          validation_steps=VALIDATION_STEPS,
                          validation_data=test_batches,
                          callbacks=[DisplayCallback(),checkpoint,early])

## Save Model Graph and weights
from tensorflow.python.tools import freeze_graph
from tensorflow.python.framework.convert_to_constants import convert_variables_to_constants_v2
# Convert Keras model to ConcreteFunction
full_model = tf.function(lambda x: model(x))
full_model = full_model.get_concrete_function(
    tf.TensorSpec(model.inputs[0].shape, tf.uint8, name="input"))
# Get frozen ConcreteFunction
frozen_func = convert_variables_to_constants_v2(full_model)
frozen_func.graph.as_graph_def()
layers = [op.name for op in frozen_func.graph.get_operations()]

# Save frozen graph from frozen ConcreteFunction to hard drive
# tf.io.write_graph(graph_or_graph_def=frozen_func.graph,
#                   logdir="./",
#                   name="pix2pixmodel.pb",
#                   as_text=False)
# tf.io.write_graph(graph_or_graph_def=frozen_func.graph,
#                   logdir="./",
#                   name="pix2pixmodel.pbtxt",
#                   as_text=True)

# 
# model.save("pix2pixmodel.pb")
# onnx_model = onnxmltools.convert_keras(model) 
# onnx.save_model(onnx_model, ("model.onnx"))

# tf.io.write_graph(sess.graph.as_graph_def(), './', 'pix2pixmodel.pbtxt', as_text=True)

import onnx
import tf2onnx
spec = (tf.TensorSpec((None, dest_size[0], dest_size[1], dest_size[2]), tf.float32, name="input"),)
onnx_model,_ =  tf2onnx.convert.from_keras(model, input_signature=spec, output_path="pix2pix.onnx")
out_filename = "pix2pix.onnx"
onnx.save_model(onnx_model, out_filename)

loss = model_history.history['loss']
val_loss = model_history.history['val_loss']

plt.figure()
plt.plot(model_history.epoch, loss, 'r', label='Training loss')
plt.plot(model_history.epoch, val_loss, 'bo', label='Validation loss')
plt.title('Training and Validation Loss')
plt.xlabel('Epoch')
plt.ylabel('Loss Value')
plt.ylim([0, 1])
plt.legend()
plt.show()

show_predictions(test_batches, 5)

from tensorflow.keras.models import load_model
# model =tf.keras.models.load_model("model.onnx")
# model = load_model("model.h5", compile=False)
# model = cv2.dnn.readNetFromONNX('model.onnx')
# model.compile(optimizer='adam',
#               loss=CustomMSE(),
#               metrics=['mean_squared_error', 'accuracy'])

testimg = np.expand_dims(np.expand_dims(cv2.resize(cv2.imread('./test_images/ChooChoo/Truck37.jpg',cv2.IMREAD_GRAYSCALE),(dest_size[1],dest_size[0])),axis=2),axis=0)
cv2.imwrite("TestImg.jpg",cv2.resize(cv2.imread('./test_images/ChooChoo/Truck37.jpg',cv2.IMREAD_GRAYSCALE),(dest_size[1],dest_size[0])))
out=model.predict(testimg)

net = cv2.dnn.readNetFromONNX("pix2pix.onnx")
image = cv2.imread("TestImg.jpg", cv2.IMREAD_GRAYSCALE)
blob = cv2.dnn.blobFromImage(image, 1)

net.setInput(blob)   
detections = net.forward() # cv2.dnn_Net.forward() function

from PIL import Image as im
import matplotlib.pyplot as plt

tmp = (np.array(detections)).argmax(axis=0)
for iclass in range(tmp.shape[2]):
    fig = plt.figure()
    image = plt.imshow(tmp[:,:,iclass])
    plt.colorbar(image)
    im.fromarray((np.array(tmp[:,:,iclass])).astype(np.uint8),'L').save('imgfromarray'+str(iclass)+'.jpg')
