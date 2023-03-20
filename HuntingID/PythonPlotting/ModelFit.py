# -*- coding: utf-8 -*-
"""
Created on Thu Jul 28 14:15:38 2022

@author: Dana
"""

import os
import math
import numpy as np
import matplotlib.pyplot as plt

from scipy import optimize
from IPython.display import display, Math

def ParseSingleDatFile(root, subdirs, filename):
    currentfile = open(root + '/' + filename,'r+')
    tmpstr = root.split('\\')
    
    with currentfile as activefile:
        CarTimeSeries = activefile.readlines()
        CarTimeSeries = [x.strip() for x in CarTimeSeries]
        
    return CarTimeSeries

def ParseDatFiles(dir1):
    print('In')
    CarTimeSeries_Nearlist = []
    CarNames_Nearlist = []
    CarTimeSeries_Farlist = []
    CarNames_Farlist = []
    
    exclude = ['Archive']
    for root, subdirs, files in os.walk(dir1):
        if not any(([string in root for string in exclude])):
            if any('.dat' in ifile for ifile in files):
                for ifile in files:
                    result = ParseSingleDatFile(root, subdirs, ifile)
                    if('_Near' in ifile):
                        CarTimeSeries_Nearlist.append(result)
                        CarNames_Nearlist.append(ifile[:-4])
                    elif('_Far' in ifile):
                        CarTimeSeries_Farlist.append(result)
                        CarNames_Farlist.append(ifile[:-4])
            
    return CarTimeSeries_Nearlist, CarNames_Nearlist, CarTimeSeries_Farlist, CarNames_Farlist

def test_func(x, dist, amp, omega, phi):
    return dist + amp * np.cos(omega * x + phi)

def FitSinusoidModel(CarTimeSeries_list, CarNames_list, gdir, trackside):
    left, width = .25, .5
    bottom, height = .25, .5
    right = left + width
    top = bottom + height
    
    fig,axs = plt.subplots(int(2),int(2),sharex=False, sharey=False)
    fig.text(0.5, 0.0, 'Sample (-)', ha='center')
    fig.text(0.04, 0.5, 'Parameter (-)', va='center', rotation='vertical')
    
    icar=0
    while (icar<len(CarTimeSeries_list)):
        try:
            # params, params_covariance = optimize.curve_fit(test_func, np.linspace(0, len(CarTimeSeries_list[icar]), num=len(CarTimeSeries_list[icar])), CarTimeSeries_list[icar], p0=[1, 1, 2, 1])
            params, params_covariance = optimize.curve_fit(test_func, np.linspace(0, len(CarTimeSeries_list[icar]), num=len(CarTimeSeries_list[icar])), CarTimeSeries_list[icar], maxfev = 100000)
        except:
            icar = icar+1
            continue
        
        # axs[irow,icol].set_yticks(np.linspace(np.min(np.array(CarTimeSeries_list[icount]).astype(np.double)), np.max(np.array(CarTimeSeries_list[icount]).astype(np.double)),3))
        param_list = ['MeanOffsetfromZero','amplitude', 'omega', 'phase']
        
        icol = 0
        irow = 0
        iparam = 0
        while (iparam<len(params)):
            
            axs[irow,icol].scatter(icar, np.double(params[iparam]), color='black',linewidth=0.1)
            
            axs[irow,icol].text(0.5*(left+right), 0.80*(bottom+top),(param_list[iparam]) ,
            horizontalalignment='center',
            verticalalignment='center',
            fontsize=8, color='red',
            rotation=0,
            transform=axs[irow,icol].transAxes)
            
            iparam= iparam+1
            icol  = icol+1
            
            if icol>(1):
                irow = irow+1
                icol = 0
            
        icar = icar+1
    plt.savefig(gdir+'CarModelParameterComparison'+trackside+'.png', dpi=2500,bbox_inches = "tight")
    plt.close()
    plt.clf()
        
if __name__ == '__main__': 
    
    GlobalDirectory = 'C:\\CodeProjects\\HuntingID\\HuntingData'
    subdirectories = os.listdir(GlobalDirectory)
    
    itrain=0
    while itrain<len(subdirectories):
        
        CarTimeSeries_Nearlist, CarNames_Nearlist, CarTimeSeries_Farlist, CarNames_Farlist = ParseDatFiles((GlobalDirectory+"\\"+subdirectories[itrain]))
        
        FitSinusoidModel(CarTimeSeries_Nearlist, CarNames_Nearlist, (GlobalDirectory+"\\"+subdirectories[itrain]), "Near")
        FitSinusoidModel(CarTimeSeries_Farlist, CarNames_Farlist, (GlobalDirectory+"\\"+subdirectories[itrain]), "Far")
        itrain=itrain+1
    
    