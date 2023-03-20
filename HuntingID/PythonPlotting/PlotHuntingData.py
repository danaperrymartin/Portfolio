# -*- coding: utf-8 -*-
"""
Created on Tue Jul 19 14:12:57 2022

@author: Dana
"""
import os
import math
import itertools
import numpy as np
import matplotlib.pyplot as plt

def ParseSingleDatFile(root, subdirs, filename):
    currentfile = open(root + '/' + filename,'r+')
    tmpstr = root.split('\\')
    
    with currentfile as activefile:
        CarTimeSeries = activefile.readlines()
        CarTimeSeries = [(x.strip()).split() for x in CarTimeSeries]
        
    return CarTimeSeries

def ParseDatFiles(dir1):
    print('In')
    CarLatForceTimeSeries_Nearlist = []
    CarStaticForceTimeSeries_Nearlist = []
    CarNames_Nearlist = []
    
    CarLatForceTimeSeries_Farlist = []
    CarStaticForceTimeSeries_Farlist = []
    CarTimeSeries_Farlist = []
    CarNames_Farlist = []
    
    exclude = ['Archive', '.png']
    for root, subdirs, files in os.walk(dir1):
        if not any(([string in root for string in exclude])):
            if any('.dat' in ifile for ifile in files):
                for ifile in files:
                    if not('.png' in ifile):
                        result = ParseSingleDatFile(root, subdirs, ifile)
                        if('_Near' in ifile):
                            CarLatForceTimeSeries_Nearlist.append(np.array(result)[:,0])
                            CarStaticForceTimeSeries_Nearlist.append(np.array(result)[:,1])
                            CarNames_Nearlist.append(ifile[:-4])
                        elif('_Far' in ifile):
                            CarLatForceTimeSeries_Farlist.append(np.array(result)[:,0])
                            CarStaticForceTimeSeries_Farlist.append(np.array(result)[:,1])
                            CarNames_Farlist.append(ifile[:-4])
            
    return CarLatForceTimeSeries_Nearlist, CarStaticForceTimeSeries_Nearlist,CarLatForceTimeSeries_Farlist, CarStaticForceTimeSeries_Farlist, CarNames_Nearlist, CarTimeSeries_Farlist, CarNames_Farlist

def PlotRaw_SubPlot(CarTimeSeries_list, CarNames_list, gdir, trackside):
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
        
        axs[irow,icol].text(0.5*(left+right), 0.2*(bottom+top),(CarNames_list[icount]) ,
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
            
    plt.savefig(gdir+'\\HuntingTimeSeries_'+trackside+'.png', dpi=2500,bbox_inches = "tight")
    plt.clf()
    
def Plot_OverLay(CarWeight_list,CarLatForceTimeSeries_Nearlist, CarStaticForceTimeSeries_Nearlist, CarNames_Nearlist, CarLatForceTimeSeries_Farlist, CarStaticForceTimeSeries_Farlist, CarNames_Farlist, gdir, trackside):
    if(any(np.array(CarWeight_list)<1000000000000)):
        left, width = .25, .5
        bottom, height = 0.25, 0.5
        right = left + width
        top = bottom + height
        
        fig,axs = plt.subplots(int(math.ceil((len(CarLatForceTimeSeries_Nearlist)/10))),int(math.ceil(10)),sharex=True, sharey=True)
        fig.text(0.5, 0.0, 'Time (s)', ha='center', fontsize=8)
        fig.text(0.04, 0.5, 'Force (kips)', va='center', rotation='vertical', fontsize=8)
        # fig.legend(axs,labels=["NearLatForce", "FarLatForce", "NormalizedNearStatic", "NormalizedFarStatic"], loc="upper right")
        
        # handles, labels = axs.get_legend_handles_labels()
        # axs.legend(handles, ["NearLatForce", "FarLatForce", "NormalizedNearStatic", "NormalizedFarStatic"])
        # axs.tick_params(axis='y', labelsize= 2)
        
        icol = 0
        irow = 0
        icount = 0
        print('Plotting raw time series...SubplotNearFar'+gdir)
        
        while icount<len(CarLatForceTimeSeries_Nearlist):
            if(CarWeight_list[irow+icol]<20):
                axs[irow,icol].set_yticks(np.linspace(np.min(np.array(CarLatForceTimeSeries_Nearlist[icount]).astype(np.double)), np.max(np.array(CarLatForceTimeSeries_Nearlist[icount]).astype(np.double)),3))
                axs[irow,icol].plot(np.array(CarLatForceTimeSeries_Nearlist[icount]).astype(np.double), color='red',linewidth=0.1)
                axs[irow,icol].plot(np.array(CarLatForceTimeSeries_Farlist[icount]).astype(np.double), color='blue',linewidth=0.1)
                
                # axs[irow,icol].set_yticks(np.linspace(0.0, 1.0,3))
                # axs[irow,icol].plot((np.array(CarStaticForceTimeSeries_Nearlist[icount]).astype(np.double))/np.max(np.array(list(itertools.chain.from_iterable(CarStaticForceTimeSeries_Nearlist))).astype(np.double)), color='red',linestyle ='dashed' ,linewidth=0.1)
                # axs[irow,icol].plot((np.array(CarStaticForceTimeSeries_Farlist[icount]).astype(np.double))/np.max(np.array(list(itertools.chain.from_iterable(CarStaticForceTimeSeries_Farlist))).astype(np.double)), color='blue',linestyle ='dashed', linewidth=0.1)
                
                # axs[irow,icol].plot((np.array(CarStaticForceTimeSeries_Nearlist[icount-1]).astype(np.double))/np.max((np.array(CarStaticForceTimeSeries_Nearlist[icount]).astype(np.double))), color='red',linestyle ='dashed' ,linewidth=0.1)
                # axs[irow,icol].plot((np.array(CarStaticForceTimeSeries_Farlist[icount-1]).astype(np.double))/np.max((np.array(CarStaticForceTimeSeries_Farlist[icount]).astype(np.double))), color='blue',linestyle ='dashed', linewidth=0.1)
                
                axs[irow,icol].grid()
                axs[irow,icol].tick_params(axis='y', labelsize= 2)
                
                axs[irow,icol].text(0.5*(left+right), 0.5*(bottom+top),(CarNames_Nearlist[icount]) ,
                horizontalalignment='center',
                verticalalignment='center',
                fontsize=0.5, color='black',
                rotation=0,
                transform=axs[irow,icol].transAxes)
            # if(icount==0):
                # axs[irow,icol].legend(labels=["NearLatForce", "FarLatForce", "NormalizedNearStatic", "NormalizedFarStatic"], loc="best", fontsize=4)
            axs[irow,icol].grid()
            axs[irow,icol].tick_params(axis='y', labelsize= 2)    
            icount = icount+1
            icol = icol+1
            if icol>(int(math.ceil((10)))-1):
                irow=irow+1
                icol = 0
        # axs[irow,icol].tick_params(axis='y', labelsize= 2)        
    plt.savefig(gdir+'\\HuntingTimeSeries_NearFarOverlayed.png', dpi=2500,bbox_inches = "tight")
    plt.clf()
    
def CarWeight(CarLatForceTimeSeries_Nearlist, CarStaticForceTimeSeries_Nearlist, CarNames_Nearlist, CarLatForceTimeSeries_Farlist, CarStaticForceTimeSeries_Farlist, CarNames_Farlist, gdir, trackside):
    
    CarWeight = []
    
    icol = 0
    irow = 0
    icount = 0
    print('Building car weight list')
    ind = np.arange(len(CarLatForceTimeSeries_Nearlist))
    while icount<len(CarLatForceTimeSeries_Nearlist):
        
        CarWeight.append(np.max((np.array(CarStaticForceTimeSeries_Nearlist[icount]).astype(np.double))))
        
        icount = icount+1
        icol = icol+1
        if icol>(int(math.ceil((10)))-1):
            irow=irow+1
            icol = 0
    # plt.bar(ind,CarWeight) 
    # plt.ylabel("Static Weight (kips)")
    # plt.xlabel("Car Position (-)")
    # plt.savefig(gdir+'\\CarWeight.png', dpi=2500,bbox_inches = "tight")
    # plt.clf()
    return CarWeight
    
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
        
        plt.savefig(gdir+'\\'+CarNames_list[icount]+'.png', dpi=1000,bbox_inches = "tight")
        plt.clf()
        
        icount = icount+1
        icol = icol+1
        if icol>(int(math.ceil((10)))-1):
            irow=irow+1
            icol = 0

if __name__ == '__main__': 
    
    GlobalDirectory = 'C:\\CodeProjects\\HuntingID\\HuntingData'
    subdirectories = os.listdir(GlobalDirectory)
    
    itrain=0
    
    while itrain<len(subdirectories):
        # try:
        CarLatForceTimeSeries_Nearlist, CarStaticForceTimeSeries_Nearlist,CarLatForceTimeSeries_Farlist, CarStaticForceTimeSeries_Farlist, CarNames_Nearlist, CarTimeSeries_Farlist, CarNames_Farlist = ParseDatFiles((GlobalDirectory+"\\"+subdirectories[itrain]))
        
        # PlotRaw_SubPlot(CarLatForceTimeSeries_Nearlist, CarNames_Nearlist, (GlobalDirectory+"\\"+subdirectories[itrain]), "Near")
        # PlotRaw_SubPlot(CarLatForceTimeSeries_Farlist, CarNames_Farlist,(GlobalDirectory+"\\"+subdirectories[itrain]), "Far")
        CarWeight_list = CarWeight(CarLatForceTimeSeries_Nearlist, CarStaticForceTimeSeries_Nearlist, CarNames_Nearlist, CarLatForceTimeSeries_Farlist, CarStaticForceTimeSeries_Farlist, CarNames_Farlist,(GlobalDirectory+"\\"+subdirectories[itrain]), "Far")
        
        Plot_OverLay(CarWeight_list,CarLatForceTimeSeries_Nearlist, CarStaticForceTimeSeries_Nearlist, CarNames_Nearlist, CarLatForceTimeSeries_Farlist, CarStaticForceTimeSeries_Farlist, CarNames_Farlist,(GlobalDirectory+"\\"+subdirectories[itrain]), "Far")
        # PlotRaw_IndPlot(CarTimeSeries_Nearlist+CarTimeSeries_Farlist, CarNames_Nearlist+CarNames_Farlist, (GlobalDirectory+"\\"+subdirectories[itrain]))
        
        CarWeight_list = []
        itrain=itrain+1
        # except:
        #     print('Issue with '+ subdirectories[itrain])
        #     itrain=itrain+1
        #     continue
