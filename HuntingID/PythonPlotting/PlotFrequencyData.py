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
    mag = []
    freq = []
    
    with currentfile as activefile:
        FreqData = activefile.readlines()
        FreqData = [(x.strip()).split() for x in FreqData]
        for i in range(1,len(FreqData)):
            mag.append(float(FreqData[i][0]))
            freq.append(float(FreqData[i][1]))
        
    return [mag, freq]

def ParseDatFiles(dir1):
    print('In')
    humpfreqData = []
    huntfreqData = []
    exclude = ['Archive', '.png']
    for root, subdirs, files in os.walk(dir1):
        if not any(([string in root for string in exclude])):
            if any('.freq' in ifile for ifile in files):
                for ifile in files:
                    if ('.freq' in ifile):
                        [mag, freq] = ParseSingleDatFile(root, subdirs, ifile)
                        if('humpfreq' in ifile):
                            humpfreqData.append( [mag, freq])
                        elif('huntfreq' in ifile):
                            huntfreqData.append( [mag, freq])
            
    return humpfreqData, huntfreqData

def Plot_Frequency(humpfreqData, huntfreqData, gdir):
    left, width = .25, .5
    bottom, height = .25, .5
    right = left + width
    top = bottom + height
    
    fig,axs = plt.subplots(1,1,sharex=True, sharey=True)
    
    fig.text(0.5, 0.0, 'Frequency (Hz)', ha='center')
    fig.text(0.00, 0.5, 'Magnitude (-)', va='center', rotation='vertical')
    
    print('Plotting interpolated data')
    # axs.scatter(np.array(humpfreqData[0][0]).astype(np.double),np.array(humpfreqData[0][1]).astype(np.double), color='blue',linewidth=0.1)
    axs.scatter(np.array(huntfreqData[0][0]).astype(np.double),np.array(huntfreqData[0][1]).astype(np.double), color='black',linewidth=0.1)
    axs.legend(labels=["hunt freq"], loc="best", fontsize=12)
    axs.grid()
    axs.title.set_text("Gage Hunting Frequency vs. Magnitude")
    
    plt.show()
    
    # plt.savefig(gdir+'\\InterpolationComp_'+'.png', dpi=2500,bbox_inches = "tight")
    # plt.clf()
    
if __name__ == '__main__': 
    
    GlobalDirectory = 'C:\\CodeProjects\\HuntingID\\HuntingData\\ExperimentalDFFT'
    
    humpfreqData, huntfreqData = ParseDatFiles((GlobalDirectory))
    Plot_Frequency(humpfreqData, huntfreqData, (GlobalDirectory))