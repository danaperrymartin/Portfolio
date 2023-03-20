# -*- coding: utf-8 -*-
"""
Created on Tue Oct 11 13:05:03 2022

@author: Dana
"""

import os
import numpy as np
import matplotlib.pyplot as plt
import math

car = []
huntingmetric = []

def ParseSingleDatFile(root, subdirs, filename):
    currentfile = open(root + '/' + filename,'r+')
    tmpstr = root.split('\\')
   
    
    with currentfile as activefile:
        HuntData = activefile.readlines()
        HuntData = [(x.strip()).split() for x in HuntData]
        for i in range(1,len(HuntData)):
            car.append(HuntData[i][0])
            huntingmetric.append(float(HuntData[i][1]))
        
    # return [car, huntingmetric]

def ParseDatFiles(dir1):
    print('In')
    # huntData = []
    exclude = ['Archive', '.png']
    for root, subdirs, files in os.walk(dir1):
        if not any(([string in root for string in exclude])):
            if any('.hnt' in ifile for ifile in files):
                for ifile in files:
                    if ('.hnt' in ifile):
                        ParseSingleDatFile(root, subdirs, ifile)                        
                        # huntData.append( [car, hmetric])
            
    # return huntData

def Plot_Frequency(gdir):
    
    huntData =[car, huntingmetric]
    
    left, width = .25, .5
    bottom, height = .25, .5
    right = left + width
    top = bottom + height
    
    fig,axs = plt.subplots(figsize =(20, 15))
    
    fig.text(0.5, 0.0, 'Sum of Magnitude between 2 and 4 Hz', ha='center')
    fig.text(0.00, 0.5, 'Car Name', va='center', rotation='vertical')
    
    print('Plotting huntingmetric data')
    axs.barh(huntData[0][0],huntData[0][1])
    #axs.legend(labels=["hunt freq"], loc="best", fontsize=12)
    #axs.grid()
    #axs.title.set_text("Gage Hunting Metric")
    
    
    # Remove axes splines
    for s in ['top', 'bottom', 'left', 'right']:
        axs.spines[s].set_visible(False)
     
    # Remove x, y Ticks
    axs.xaxis.set_ticks_position('none')
    axs.yaxis.set_ticks_position('none')
     
    # Add padding between axes and labels
    axs.xaxis.set_tick_params(pad = 5)
    axs.yaxis.set_tick_params(pad = 10)
     
    # Add x, y gridlines
    axs.grid(b = True, color ='grey',
            linestyle ='-.', linewidth = 0.5,
            alpha = 0.2)
     
    # Show top values
    axs.invert_yaxis()
     
    # Add annotation to bars
    for i in axs.patches:
        plt.text(i.get_width()+0.2, i.get_y()+0.5,
                 str(round((i.get_width()), 2)),
                 fontsize = 10, fontweight ='bold',
                 color ='grey')
     
    # Add Plot Title
    axs.set_title('Hunting Metric per Car',
                 loc ='center', )
     
    # Add Text watermark
    fig.text(0.9, 0.15, 'Marteen', fontsize = 12,
             color ='grey', ha ='right', va ='bottom',
             alpha = 0.7)
    
    
    plt.show()
    
    # plt.savefig(gdir+'\\InterpolationComp_'+'.png', dpi=2500,bbox_inches = "tight")
    # plt.clf()
    
if __name__ == '__main__': 
    
    GlobalDirectory = 'C:\\CodeProjects\\HuntingID\\HuntingData\\ExperimentalDFFT'
    
    ParseDatFiles((GlobalDirectory))
    Plot_Frequency(GlobalDirectory)