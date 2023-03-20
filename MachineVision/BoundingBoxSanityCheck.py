#!/usr/bin/env python
# coding: utf-8

# In[3]:

import h5py
import os,cv2,keras
import pandas as pd
import matplotlib.pyplot as plt
import numpy as np
import time
import os
# os.environ["CUDA_VISIBLE_DEVICES"] = "-1"
import tensorflow as tf
from sklearn import preprocessing
from sklearn.preprocessing import MultiLabelBinarizer
from sklearn.preprocessing import LabelEncoder
from sklearn.preprocessing import OneHotEncoder

from keras.models import Sequential
from keras.layers import Conv2D, MaxPooling2D,Dense, Activation, Flatten
from tensorflow.keras.optimizers import SGD
from tensorflow.keras import initializers
from tensorflow.compat.v1 import ConfigProto
from tensorflow.compat.v1 import InteractiveSession


path = "./TrainRCNN/train_images/ChooChoo_Truck/"
annot = "./TrainRCNN/train_images/ChooChoo_Truck/Truck_Annotations/"

for e,i in enumerate(os.listdir(annot)):
    if e < 9:
        filename = i.split(".")[0]+".jpg"
        print(filename)
        img = cv2.imread(os.path.join(path,filename))
        df = pd.read_csv(os.path.join(annot,i))
        plt.imshow(img)
        for row in df.iterrows():
            x1 = int(row[1][0].split(" ")[0])
            y1 = int(row[1][0].split(" ")[1])
            x2 = int(row[1][0].split(" ")[2])
            y2 = int(row[1][0].split(" ")[3])
            cv2.rectangle(img,(x1,y1),(x2,y2),(255,0,0), 5)
        # plt.figure()
        # plt.imshow(img)
        cv2.imwrite('./SanityCheckTestImage_'+str(e)+'.jpg',img)