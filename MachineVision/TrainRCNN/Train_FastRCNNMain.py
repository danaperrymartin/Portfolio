# -*- coding: utf-8 -*-
# Dana Martin
# 10/11/2021
# Feature detection tool
import multiprocessing
import cv2,os,selective_search
import numpy as np
import tensorflow as tf
import torch

from fastrcnn.network_model import NetworkModel
from fastrcnn.ImageManipulation import ImageManipulation

def nms(boxes, overlapThresh, conf, confthresh):
	# if there are no boxes, return an empty list
	if len(boxes) == 0:
		return []
	# if the bounding boxes integers, convert them to floats --
	# this is important since we'll be doing a bunch of divisions
	if boxes.dtype.kind == "i":
		boxes = boxes.astype("float")
	# initialize the list of picked indexes	
	pick = []
	# grab the coordinates of the bounding boxes
	x1 = boxes[:,0]
	y1 = boxes[:,1]
	x2 = x1+boxes[:,2]
	y2 = y1+boxes[:,3]
	# compute the area of the bounding boxes and sort the bounding
	# boxes by the bottom-right y-coordinate of the bounding box
	areas = (abs(x2 - x1)) * (abs(y2 - y1))
	idxs = np.argsort(conf, axis=0, kind='quicksort')
	# keep looping while some indexes still remain in the indexes
	# list
	keep = []
	col = 1
	while len(idxs) > 0:
		idx = idxs[-1]
		keep.append(boxes[idx])
		idxs=idxs[:-1]
		# select coordinates of BBoxes according to
		# the indices in order
		print(torch.tensor(idxs).squeeze())
		xx1 = torch.index_select(torch.tensor(x1),dim = 0, index = torch.tensor(idxs[:,col]))
		xx2 = torch.index_select(torch.tensor(x2),dim = 0, index = torch.tensor(idxs[:,col]))
		yy1 = torch.index_select(torch.tensor(y1),dim = 0, index = torch.tensor(idxs[:,col]))
		yy2 = torch.index_select(torch.tensor(y2),dim = 0, index = torch.tensor(idxs[:,col]))        
        
		# Step 2
		# find the coordinates of the intersection boxes
		xx1 = torch.max(xx1, torch.tensor(x1[idx,col]))
		yy1 = torch.max(yy1, torch.tensor(y1[idx,col]))
		xx2 = torch.min(xx2, torch.tensor(x2[idx,col]))
		yy2 = torch.min(yy2, torch.tensor(y2[idx,col]))
		# find height and width of the intersection boxes
		w = xx2 - xx1
		h = yy2 - yy1
		# take max with 0.0 to avoid negative w and h
		# due to non-overlapping boxes
		w = torch.clamp(w, min=0.0)
		h = torch.clamp(h, min=0.0)
        
		# find the intersection area
		inter = w*h
		# find the areas of BBoxes according the indices in order
		rem_areas = torch.index_select(areas, dim = 0, index = idxs)
		# find the union of every prediction T in P
		# with the prediction S
		# Note that areas[idx] represents area of S
		union = (rem_areas - inter) + areas[idx]
		# find the IoU of every prediction in P with S
		IoU = inter / union
		# keep the boxes with IoU less than thresh_iou
		mask = IoU < confthresh
		idxs = idxs[mask]        
        
	return keep

if __name__ == '__main__':
    
    multiprocessing.freeze_support()
    
    method =1
    channels=3
    scalefactor = 0.1
    width = 64 #int(2048*scalefactor)
    height = 64#int(24522*scalefactor)
    input_shape=[width, height, channels]
    output_shape = 2
    
    batch_size = 1
    
    cv2.setUseOptimized(True);
    ss = cv2.ximgproc.segmentation.createSelectiveSearchSegmentation()
    
    imgpath = "train_images/ChooChoo_Truck/"
    annot = (imgpath+"Truck_Annotations/")
    numimage2load = 1#87
    
    ## Create instance and initialize data set creation
    im = ImageManipulation( annot, imgpath, numimage2load, ss, input_shape, output_shape, batch_size)
    
    if (method ==1):
        train_ds, val_ds = im.getdata_m0()
    if (method==2):
        train_ds, val_ds = im.getdata_m1()
    if (method==3):
        train_ds, val_ds = im.getdata_m2()
    
    ## Add augmented data dataset
    train_ds, val_ds = im.getaugmenteddata(train_ds, val_ds)
    
    ## Define model architecture
    # model = net_model.vggmodel()
    # model = net_model.seq_v0()
    # model = net_model.res_net_v0()
    # model = net_model.res_net_v1()
    # model = net_model.res_net_v2()
    modelstring = "seq_v0"
    
    # Create instance and initialize NetworkModel()
    net_model = NetworkModel(input_shape, output_shape, modelstring)
    
    ## Now we start the training of the model using fit()
    # net_model.train(train_ds, val_ds, modelstring)
    
    testimagepath = "test_images/ChooChoo/"
    testimage = "TestImg1.jpg"
    
    [timeelapsed, conf_shared, bbox_shared] = net_model.test(ss, testimage, testimagepath)
    
    conf = [[conf_shared[c].x, conf_shared[c].y] for c in range(len(conf_shared))]
    bbox = [[bbox_shared[c].w, bbox_shared[c].x, bbox_shared[c].y, bbox_shared[c].z] for c in range(len(bbox_shared))]
    
    acceptable_accuracy = 1
    bboxespicked = nms(np.array(bbox), 0.6, np.array(conf),acceptable_accuracy)
    
    img = cv2.imread(os.path.join(testimagepath,testimage))
    for i in range(len(conf)):
        x,y,w,h = bbox[i]
        if conf[0][1] >= acceptable_accuracy:
            cv2.rectangle(img, (int(x), int(y)), (int(x+w), int(y+h)), (0, 255, 0), cv2.LINE_AA)
    cv2.imwrite(("Picture.jpg"), img)
    