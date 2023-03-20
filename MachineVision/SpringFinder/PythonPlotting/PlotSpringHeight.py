# -*- coding: utf-8 -*-
"""
Created on Mon Feb 27 09:03:45 2023

@author: Dana
"""

import math
import numpy as np
import matplotlib.pyplot as plt

springheight = []
springwidth = []

def ParseDataFile(file):
    currentfile = open(file,'r+')
    
    with currentfile as activefile:
        spring_data = activefile.readlines()
        spring_data = [(x.strip()).split() for x in spring_data]
        for i in range(1,len(spring_data)):
            springwidth.append(spring_data[i][0])
            springheight.append(float(spring_data[i][1]))
        
    return [springwidth, springheight]
    
def PlotData(spring_width, spring_height):
    left, width = .25, .5
    bottom, height = .25, .5
    right = left + width
    top = bottom + height
    
    fig,axs = plt.subplots(int(2),int(1), figsize=(15, 15), sharex=False, sharey=False)
    
    icol = 0
    irow = 0
    icount = 0
    print('Plotting raw time series...Subplot')
        
    axs[0].plot(np.array(spring_height).astype(np.double), color='black',linewidth=1)
    axs[0].set(xlabel='Truck', ylabel='Spring height (pixels)')
    axs[0].grid()
    axs[1].hist(spring_height, bins=100)
    axs[1].set(xlabel='Spring height (binned)', ylabel='# of Occurrences')
    axs[1].grid()
    
    plt.show()        
    # plt.savefig(gdir+'\\HuntingTimeSeries_'+trackside+'.png', dpi=2500,bbox_inches = "tight")
    # plt.clf()
    
if __name__ == '__main__': 
    
    GlobalDirectory = 'C:\\CodeProjects\\MachineVision\\SpringFinder\\SpringFinder\\BoundingBoxDim.txt'
    
    springwidth, springheight = ParseDataFile((GlobalDirectory))
    PlotData(springwidth, springheight)
    
    
    