# -*- coding: utf-8 -*-
# Dana Martin
# 01/03/2022
# Feature detection tool
import os
import sys
import xml.etree
import mrcnn.utils
import mrcnn.config
import mrcnn.model
import pandas as pd

from numpy import zeros, asarray

def clear_all():
    """Clears all the variables from the workspace of the spyder application."""
    gl = globals().copy()
    for var in gl:
        #if var[0] == '_': continue
        #if 'func' in str(globals()[var]): continue
        #if 'module' in str(globals()[var]): continue
        del globals()[var]
    
class TruckDataset(mrcnn.utils.Dataset):
    
    def load_dataset(self, dataset_dir, is_train=True):
    	self.add_class("dataset", 1, "Truck")
    
    	images_dir = dataset_dir + '/'
    	annotations_dir = dataset_dir + '/Truck_Annotations/'
    
    	for filename in os.listdir(images_dir):
    		if (".jpg" in filename):
    			image_id = filename[:-4]
    			image_id = image_id[-2:]
    			if is_train and int(image_id) >= 33:
    				continue
    
    			if not is_train and int(image_id) < 33 and int(image_id)>35:
    				continue
    
    			img_path = images_dir + filename
    			ann_path = annotations_dir +'Truck' +image_id + '.csv'
    
    			self.add_image('dataset', image_id=image_id, path=img_path, annotation=ann_path)
    
    def extract_boxes(self, filename):
    	df = pd.read_csv(filename)
    	boxes = list()
    	for row in df.iterrows():
    		x1 = int(row[1][0].split(" ")[0])
    		y1 = int(row[1][0].split(" ")[1])
    		x2 = int(row[1][0].split(" ")[2])
    		y2 = int(row[1][0].split(" ")[3])
    		coords = [x1, y1, x2, y2]
    		boxes.append(coords)
    
    	width = int(abs(x1-x2))
    	height = int(abs(y1-y2))
    	return boxes, width, height
    
    def load_mask(self, image_id):
    	info = self.image_info[image_id]
    	path = info['annotation']
    	boxes, w, h = self.extract_boxes(path)
    	masks = zeros([h, w, len(boxes)], dtype='uint8')
    
    	class_ids = list()
    	for i in range(len(boxes)):
    		box = boxes[i]
    		row_s, row_e = box[1], box[3]
    		col_s, col_e = box[0], box[2]
    		masks[row_s:row_e, col_s:col_e, i] = 1
    		class_ids.append(self.class_names.index('Truck'))
    	return masks, asarray(class_ids, dtype='int32')

class TruckConfig(mrcnn.config.Config):
    NAME = "Truck_cfg"
        
    BACKBONE = "resnet101"
        
    VALIDATION_STEPS = 2
    GPU_COUNT = 1
    IMAGES_PER_GPU = 1
        
    NUM_CLASSES = 2
    
    STEPS_PER_EPOCH = 131
    IMAGE_RESIZE_MODE = "crop"
    IMAGE_MIN_DIM = 64
    IMAGE_MAX_DIM = 512
    
train_set = TruckDataset()
train_set.load_dataset(dataset_dir='./train_images/ChooChoo_Truck', is_train=True)
train_set.prepare()
    
valid_dataset = TruckDataset()
valid_dataset.load_dataset(dataset_dir='./train_images/ChooChoo_Truck', is_train=False)
valid_dataset.prepare()

model = mrcnn.model.MaskRCNN(mode='training', 
                                model_dir='./models', 
                                config=TruckConfig())

# model.load_weights(filepath='models/Truck_mask_rcnn.h5', 
#                     by_name=True, 
#                     exclude=["mrcnn_class_logits", "mrcnn_bbox_fc",  "mrcnn_bbox", "mrcnn_mask"])
    
model.train(train_dataset=train_set, 
            val_dataset=valid_dataset, 
            learning_rate=TruckConfig().LEARNING_RATE, 
            epochs=10, 
            layers='heads')
    
model_path = 'models/Spring_mask_rcnn.h5'
model.keras_model.save_weights(model_path)

import cv2
import mrcnn.visualize

model = mrcnn.model.MaskRCNN(mode="inference", 
                              model_dir="./models/",
                              config=TruckConfig())
    
model.keras_model.summary()
    
model.load_weights(filepath="models/Truck_mask_rcnn.h5", 
                    by_name=True)

# image = cv2.imread("./test_images/video_0/55.jpg")
image = cv2.imread("./test_images/ChooChoo/TestImg1.jpg")
image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
    
r = model.detect(images=[image], 
                    verbose=0)
    
CLASS_NAMES = ['BG', 'Truck']
    
r = r[0]
    
mrcnn.visualize.display_instances(image=image, 
                                    boxes=r['rois'], 
                                    masks=r['masks'], 
                                    class_ids=r['class_ids'], 
                                    class_names=CLASS_NAMES, 
                                    scores=r['scores'])
    
clear_all()
#from numba import cuda 
#cuda.select_device(0)
#cuda.close()

