# -*- coding: utf-8 -*-
"""
Created on Thu Apr 14 09:20:00 2022

@author: Dana
"""

import os
import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
import re
from PIL import Image as im
import cv2

directory_image = "./data/images/"
directory_maskdefinitions = "./data/class masks/"

images = []
classname = []
mask_num = []
imageshapes = []

def split(word):
    return [float(char) for char in word]

for e,i in enumerate(os.listdir(directory_maskdefinitions)):
    file = open(directory_maskdefinitions+i,mode = 'r')
    contents = file.read().splitlines()
    file.close()
    
    images.append(i.split("_")[0]+".jpg")
    imageshapes.append((len(contents),len(contents[0]),1))
    classname.append((i.split("_")[1]).split(".csv")[0])
    del contents
    # mask_num.append(i.split("_")[2].split(".")[0])

datapoints = {'image':images,'imageshapes':imageshapes, 'class': classname}
df_datapoints = pd.DataFrame(datapoints)

my_dpi = 96
for e,i in enumerate(df_datapoints['image'].unique()):
    for f,j in enumerate(df_datapoints['imageshapes'].unique()):
        for g,k in enumerate(df_datapoints['class'].unique()):
            file = open(directory_maskdefinitions+i.split('.jpg')[0]+'_'+k+'.csv',mode = 'r')
            contents = file.read().splitlines()
            file.close()
            img_file = cv2.resize(cv2.imread(directory_image+i,cv2.IMREAD_GRAYSCALE),(len(contents[0]),len(contents)))
            
            mask = np.empty((len(contents),len(contents[0])), dtype=np.uint8)
            for irow in range(len(contents)):
                # mask.append(split(contents[irow]))
                mask[irow,:]=split(contents[irow])
            
            masklabel = (g+1)*mask
            label = im.fromarray((np.array(masklabel)).astype(np.uint8),'L').save(k.split('.csv')[0]+'_Label'+i.split('.jpg')[0]+'.jpg')
            
            mask = np.multiply(masklabel,img_file)
            # fig = plt.figure(figsize = (j[0]/my_dpi,j[1]/my_dpi),dpi=my_dpi)
            # color_map=plt.cm.get_cmap()
            image = im.fromarray((np.array(mask)).astype(np.uint8),'L').save(k.split('.csv')[0]+'_Mask'+i.split('.jpg')[0]+'.jpg')
            # ax = plt.imshow(im.fromarray((mask*255).astype(np.uint8),'L'),interpolation='nearest')
            # ax = plt.axis('off')
            # plt.savefig(k.split('.csv')[0]+'_Mask'+i.split('.jpg')[0]+'.jpg', bbox_inches='tight', pad_inches=0, dpi=my_dpi)
            # plt.close("all")
            
            # image = im.fromarray(np.asarray(mask))
            # image = image.convert('RGB')
            # image = im.frombytes('RGB',(j[1],j[0]), fig.canvas.tostring_rgb(),decoder_name='ascii')
            # image.save(str(g)+'Test.bmp')

# image = cv2.imread('Truck_MaskImage381.jpg',cv2.IMREAD_COLOR)        




