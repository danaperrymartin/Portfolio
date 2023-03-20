# -*- coding: utf-8 -*-
"""
Created on Fri Jul  8 10:26:08 2022

@author: Dana
"""

import os
import numpy as np
import pandas as pd
import shutil

from PIL import Image
from datetime import datetime

class TrainVisionCarWheelInfo(object):
    def __init__(self, StartTime=0, Time=[], DateTime=[], Wheel=[], Coupler=[], Site = None):
        
        self.StartTime = 0
        self.Time = []
        self.DateTime = []
        self.Wheel =[]
        self.Coupler = []
        self.Site = None

def ParseSingleFile(root, subdirs, filename, Train):
    currentfile = open(root + '/' + filename,'r+')
    tmpstr = filename.split('_')
    Train.StartTime = datetime.strptime(tmpstr[3].split(' ')[0]+'.'+tmpstr[2]+'.'+tmpstr[1]+' '+tmpstr[3].split(' ')[1]+':'+tmpstr[4]+':'+tmpstr[4]+','+tmpstr[5],
                           '%d.%m.%Y %H:%M:%S,%f')
    with currentfile as activefile:
        content = activefile.readlines()
        content = [x.strip() for x in content]
        iline=0
        line = content[iline]
        tmp_values = line.split('	')
        tmp_values = [x for x in tmp_values if len(x)>1]
        for iline in range(1,len(content)-1):
            tmp_values = content[iline].split('	')
            tmp_values = [x for x in tmp_values if len(x)>=1]
            Train.Coupler.append(tmp_values[1])
            Train.Wheel.append(tmp_values[2])
            
    return Train

def ParseWheelDetectFiles(dir1):
    print('In')
    CarWheel_DataList = []
    exclude = ['Archive']
    for root, subdirs, files in os.walk(dir1):
        if not any(([string in root for string in exclude])):
            if any(['.txt' in ifile for ifile in files]):
                for ifile in files:
                    if ('.txt' in ifile):
                        Train = TrainVisionCarWheelInfo()
                        CarWheel_DataList=ParseSingleFile(root, subdirs, ifile, Train)
            
    return Train


def ReadBinFile(maindir, filename):
    
    maxjpegsize = 2078
    
    w = 65000
    with open((maindir+filename), mode='rb') as file:
        filelength = int(file.seek(0,2))
        file.close()
    h = maxjpegsize
    with open((maindir+filename), mode='rb') as file: # b is important -> binary
        d = np.fromfile(file,dtype=np.uint8,count=w*h).reshape(w,h)    
        # iline=0
        # while (iline<filelength):
        #     d = np.fromfile(file,dtype=np.uint8,count=w*h).reshape(w,h)
        #     iline+=1
    
    PILimage = Image.fromarray(d)
    # PILimage.rotate(90, expand = 1)
    PILimage.save('result.jpg')
    
    # for image in range(0, int(int(file.seek(0,2)/(2048))/maxjpegsize)):
    #     # h = int(file.seek(0,2)/3072)
    #     PILimage = Image.fromarray(d)
    #     # PILimage.rotate(90, expand = 1)
    #     PILimage.save('result'+image+'.jpg')

    
if __name__ == '__main__': 
    
    GlobalDirectory = 'C:\\Dat4Transfer\\'
    CarWheel_DataList = ParseWheelDetectFiles(GlobalDirectory)
    filename = 'Gage_2022_07_02_10_22_56_FarSide_2048x3072.bin'
    RawImageData = ReadBinFile(GlobalDirectory, filename)