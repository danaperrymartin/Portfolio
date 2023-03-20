# -*- coding: utf-8 -*-
"""
Created on Tue Jul 19 14:12:57 2022

@author: Dana
"""
import os
import math
import numpy as np
import matplotlib.pyplot as plt

def ParseSingleDatFile(root, subdirs, filename):
    currentfile = open(root + '/' + filename,'r+')
    tmpstr = root.split('\\')
    
    with currentfile as activefile:
        CarTimeSeries = activefile.readlines()
        CarTimeSeries = [x.strip() for x in CarTimeSeries]
        
    return CarTimeSeries

def ParseDatFiles(dir1):
    print('In')
    CarTimeSeries_list = []
    CarNames_list = []
    exclude = ['Archive']
    for root, subdirs, files in os.walk(dir1):
        if not any(([string in root for string in exclude])):
            if any('.dat' in ifile for ifile in files):
                for ifile in files:
                    result = ParseSingleDatFile(root, subdirs, ifile)
                    CarTimeSeries_list.append(result)
                    CarNames_list.append(ifile[:-4])
            
    return CarTimeSeries_list, CarNames_list

def PlotRaw_SubPlot(CarTimeSeries_list, CarNames_list, gdir):
    left, width = .25, .5
    bottom, height = .25, .5
    right = left + width
    top = bottom + height
    
    fig,axs = plt.subplots(int(math.ceil((len(CarTimeSeries_list)/10))),int(math.ceil(10)),sharex=True, sharey=True)
    fig.text(0.5, 0.0, 'Time (s)', ha='center')
    fig.text(0.04, 0.5, 'Force (kips)', va='center', rotation='vertical')
    
    icol = 0
    irow = 0
    icount = 0
    print('Plotting raw time series...Subplot')
    while icount<len(CarTimeSeries_list):
        
        axs[irow,icol].set_yticks(np.linspace(np.min(np.array(CarTimeSeries_list[icount]).astype(np.double)), np.max(np.array(CarTimeSeries_list[icount]).astype(np.double)),3))
        axs[irow,icol].plot(np.array(CarTimeSeries_list[icount]).astype(np.double), color='black',linewidth=0.1)
        
        fig.text(0.5, 0.0, 'Time (s)', ha='center')
        fig.text(0.04, 0.5, 'Max Lateral Force (kips)', va='center', rotation='vertical')
        
        axs[irow,icol].text(0.5*(left+right), 0.20*(bottom+top),(CarNames_list[icount]) ,
        horizontalalignment='center',
        verticalalignment='center',
        fontsize=2, color='black',
        rotation=0,
        transform=axs[irow,icol].transAxes)
        
        icount = icount+1
        icol = icol+1
        if icol>(int(math.ceil((10)))-1):
            irow=irow+1
            icol = 0
            
    plt.savefig(gdir+'HuntingTimeSeries.png', dpi=2500,bbox_inches = "tight")
    plt.clf()
    
def PlotRaw_IndPlot(CarTimeSeries_list, CarNames_list, gdir):
    left, width = .25, .5
    bottom, height = .25, .5
    right = left + width
    top = bottom + height
    
    icol = 0
    irow = 0
    icount = 0
    print('Plotting raw time series...individual plots')
    while icount<len(CarTimeSeries_list):
        
        plt.xlabel('Sample (-)')
        plt.ylabel('Max Lateral Load (kips)')
        
        plt.plot(np.array(CarTimeSeries_list[icount]).astype(np.double), color='black',linewidth=0.3)
        
        plt.yticks(np.linspace(np.min(np.array(CarTimeSeries_list[icount]).astype(np.double)), np.max(np.array(CarTimeSeries_list[icount]).astype(np.double)),3))
        
        plt.grid(True)
        
        plt.savefig(gdir+CarNames_list[icount]+'.png', dpi=1000,bbox_inches = "tight")
        plt.clf()
        
        icount = icount+1
        icol = icol+1
        if icol>(int(math.ceil((10)))-1):
            irow=irow+1
            icol = 0
            
       

if __name__ == '__main__': 
    
    GlobalDirectory = 'C:\\CodeProjects\\MachineVision\\FeatureID\\HuntingData\\Gage_2022_07_08 15_54_22_774\\'
    
    CarTimeSeries_list, CarNames_list = ParseDatFiles(GlobalDirectory)
    PlotRaw_SubPlot(CarTimeSeries_list, CarNames_list, GlobalDirectory)
    PlotRaw_IndPlot(CarTimeSeries_list, CarNames_list, GlobalDirectory)
    