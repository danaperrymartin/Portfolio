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
    freq = []
    mag = []
    
    with currentfile as activefile:
        WheelFrequencyInfo = activefile.readlines()
        WheelFrequencyInfo = [(x.strip()).split() for x in WheelFrequencyInfo]
        for icrib in range(1,len(WheelFrequencyInfo)):
            freq.append(float(WheelFrequencyInfo[icrib][0]))
            mag.append(float(WheelFrequencyInfo[icrib][1]))
        
    return [freq, mag]

def ParseDatFiles(dir1):
    print('In')
    CarFrequency_Nearlist = []
    CarMagnitude_Nearlist = []
    CarNames_Nearlist = []
    
    CarFrequency_Farlist = []
    CarMagnitude_Farlist = []
    CarNames_Farlist = []
    
    exclude = ['Archive', '.png']
    for root, subdirs, files in os.walk(dir1):
        if not any(([string in root for string in exclude])):
            if any('.dat' in ifile for ifile in files):
                for ifile in files:
                    if not('.png' in ifile):
                        [freq, mag] = ParseSingleDatFile(root, subdirs, ifile)
                        if('_Near' in ifile):
                            CarFrequency_Nearlist.append(freq)
                            CarMagnitude_Nearlist.append(mag)
                            CarNames_Nearlist.append(ifile[:-4])
                        elif('_Far' in ifile):
                            CarFrequency_Farlist.append(freq)
                            CarMagnitude_Farlist.append(mag)
                            CarNames_Farlist.append(ifile[:-4])
            
    return CarFrequency_Nearlist, CarMagnitude_Nearlist, CarNames_Nearlist, CarFrequency_Farlist, CarMagnitude_Farlist, CarNames_Farlist

def Plot_PSD(CarFrequency_Nearlist, CarMagnitude_Nearlist, CarNames_Nearlist, CarFrequency_Farlist, CarMagnitude_Farlist, CarNames_Farlist, gdir, trackside):
    left, width = .25, .5
    bottom, height = .25, .5
    right = left + width
    top = bottom + height
    
    fig,axs = plt.subplots(int(math.ceil((len(CarFrequency_Nearlist)/10))),int(math.ceil(10)),sharex=True, sharey=True)
    
    fig.text(0.5, 0.0, 'Freq (Hz)', ha='center')
    fig.text(0.04, 0.5, 'Power (db)', va='center', rotation='vertical')
    
    icol = 0
    irow = 0
    icount = 0
    print('Plotting raw time series...Subplot')
    while icount<len(CarFrequency_Nearlist):
        if(np.max(np.array(CarFrequency_Nearlist[icount]))<20):
            axs[irow,icol].set_xticks(np.linspace(0, 5 ,5))
            axs[irow,icol].set_yticks(np.linspace(np.min(np.array(CarMagnitude_Nearlist[icount]).astype(np.double)), np.max(np.array(CarMagnitude_Nearlist[icount]).astype(np.double)),3))
            axs[irow,icol].scatter(np.array(CarFrequency_Nearlist[icount]).astype(np.double),np.array(CarMagnitude_Nearlist[icount]).astype(np.double), color='black',linewidth=0.1)
            
            axs[irow,icol].text(0.5*(left+right), 0.2*(bottom+top),(CarNames_Nearlist[icount]) ,
            horizontalalignment='center',
            verticalalignment='center',
            fontsize=0.5, color='black',
            rotation=0,
            transform=axs[irow,icol].transAxes)
            
            icount = icount+1
            icol = icol+1
            if icol>(int(math.ceil((10)))-1):
                irow=irow+1
                icol = 0
        else:
            icount=icount+1
    plt.savefig(gdir+'\\PSDInfo_'+trackside+'.png', dpi=2500,bbox_inches = "tight")
    plt.clf()

if __name__ == '__main__': 
    
    GlobalDirectory = 'C:\\CodeProjects\\HuntingID\\HuntingData\\ExperimentalDFFT'
    subdirectories = os.listdir(GlobalDirectory)
    
    itrain=0
    
    while itrain<len(subdirectories):
        CarFrequency_Nearlist, CarMagnitude_Nearlist, CarNames_Nearlist, CarFrequency_Farlist, CarMagnitude_Farlist, CarNames_Farlist=ParseDatFiles((GlobalDirectory+"\\"+subdirectories[itrain]))
        Plot_PSD(CarFrequency_Nearlist, CarMagnitude_Nearlist, CarNames_Nearlist, CarFrequency_Farlist, CarMagnitude_Farlist, CarNames_Farlist, (GlobalDirectory+"\\"+subdirectories[itrain]), "Near")
        itrain = itrain+1