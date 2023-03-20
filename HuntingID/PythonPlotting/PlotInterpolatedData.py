# -*- coding: utf-8 -*-
"""
Created on Tue Oct 11 13:05:03 2022

@author: Dana
"""

import os
import numpy as np
import matplotlib.pyplot as plt
import math

def ParseSingleDatFile(root, subdirs, filename):
    currentfile = open(root + '/' + filename,'r+')
    tmpstr = root.split('\\')
    time = []
    value = []
    InterpData = []
    
    with currentfile as activefile:
        InterpData = activefile.readlines()
        InterpData = [(x.strip()).split() for x in InterpData]
        for i in range(1,len(InterpData)):
            time.append(float(InterpData[i][0]))
            value.append(float(InterpData[i][1]))
        
    return [time, value]

def ParseDatFiles(dir1):
    print('In')
    OriginalData = []
    MKLData = []
    WikiData = []
    exclude = ['Archive', '.png']
    for root, subdirs, files in os.walk(dir1):
        if not any(([string in root for string in exclude])):
            if any('.interp' in ifile for ifile in files):
                for ifile in files:
                    if ('.interp' in ifile):
                        [time, value] = ParseSingleDatFile(root, subdirs, ifile)
                        if('MKL' in ifile):
                            MKLData.append([time,value])
                        elif('CustomClass' in ifile):
                            WikiData.append([time,value])
                        elif('OriginalValue' in ifile):
                            OriginalData.append([time,value])
            
    return OriginalData, MKLData, WikiData

def Plot_TimeSeries(OriginalData, MKLData, WikiData, gdir):
    left, width = .25, .5
    bottom, height = .25, .5
    right = left + width
    top = bottom + height
    
    fig,axs = plt.subplots(1,1,sharex=True, sharey=True)
    
    fig.text(0.5, 0.0, 'Time (s)', ha='center')
    fig.text(0.00, 0.5, 'Value ($F_{lat}$/$F_{stat}$)', va='center', rotation='vertical')
    
    print('Plotting interpolated data')
    axs.scatter(np.array(MKLData[0][0]).astype(np.double),np.array(MKLData[0][1]).astype(np.double), color='orange',linewidth=0.1)
    axs.scatter(np.array(OriginalData[0][0]).astype(np.double),np.array(OriginalData[0][1]).astype(np.double), color='black',s=200,marker='X',linewidth=0.1)
    # axs.scatter(np.array(WikiData[0][0]).astype(np.double),np.array(WikiData[0][1]).astype(np.double), color='red',linewidth=0.1)
    axs.title.set_text('Gage')
    plt.show()
    
    # plt.savefig(gdir+'\\InterpolationComp_'+'.png', dpi=2500,bbox_inches = "tight")
    # plt.clf()
    
if __name__ == '__main__': 
    
    GlobalDirectory = 'C:\\CodeProjects\\HuntingID\\HuntingData\\ExperimentalDFFT'
    
    OriginalData, MKLData, WikiData = ParseDatFiles((GlobalDirectory))
    Plot_TimeSeries(OriginalData, MKLData, WikiData, (GlobalDirectory))