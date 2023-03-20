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
import scipy.fftpack

def ParseSingleDatFile(root, subdirs, filename):
    currentfile = open(root + '/' + filename,'r+')
    tmpstr = root.split('\\')
    x=[]
    y=[]
    
    with currentfile as activefile:
        CarTimeSeries = activefile.readlines()
        CarTimeSeries = [(x.strip()).split() for x in CarTimeSeries]
        for i in range(1,len(CarTimeSeries)):
            x.append(float(CarTimeSeries[i][0]))
            y.append(float(CarTimeSeries[i][1]))
        
    return x,y

def ParseSingleSinFile(root, subdirs, filename):
    currentfile = open(root + '/' + filename,'r+')
    tmpstr = root.split('\\')
    a = int()
    b = int()
    c = int()
    d = int()
    
    with currentfile as activefile:
        sinparams = activefile.readlines()
        sinparams = [(x.strip()).split() for x in sinparams]
        a = (float(sinparams[2][0]))
        b = (float(sinparams[2][1]))
        c = (float(sinparams[2][2]))
        d = (float(sinparams[2][3]))
        
    return a,b,c,d

def ParseFiles(dir1):
    print('In')
    data_x = []
    data_y = []
    sinparam = []
    
    exclude = ['Archive', '.png']
    for root, subdirs, files in os.walk(dir1):
        if not any(([string in root for string in exclude])):
            for ifile in files:
                if('.dat' in ifile):
                    data_x,data_y = ParseSingleDatFile(root, subdirs, ifile)
                elif('.reg' in ifile):
                    a,b,c,d = ParseSingleSinFile(root, subdirs, ifile)
                        
            
    return data_x, data_y, a, b, c, d

def GenerateSin(a, b, c, d, data_x):
    # x = np.linspace(0, data_x[len(data_x)-1], 10)
    x = np.linspace(0, 0.5, 100)
    Hz2radpers = 6.28
    y = a*np.sin(b*(Hz2radpers)*x+c)+d
    
    return x,y

def PlotRaw_SubPlot(data_x, data_y, a, b, c, d, gdir, trackside):
    left, width = .25, .5
    bottom, height = .25, .5
    right = left + width
    top = bottom + height
    
    fig,axs = plt.subplots(int(3),int(math.ceil(1)), figsize=(20,15), sharex=False, sharey=False)
    # fig.text(0.5, 0.0, 'Time (s)', ha='center')
    # fig.text(0.0, 0.5, 'Force (kips)', va='center', rotation='vertical')
    
    icol = 0
    irow = 0
    icount = 0
    print('Plotting raw time series...Subplot')
    
    # axs.set_yticks(np.linspace(np.min(np.array(data_y[icount]).astype(np.double)), np.max(np.array(data_x[icount]).astype(np.double)),3))
    # axs[0].set_title('Raw Data')
    axs[0].scatter(np.array(data_x).astype(np.double),np.array(data_y).astype(np.double), color='black',linewidth=0.5)
    axs[0].set_xlabel('Time (s)')
    axs[0].set_ylabel('Lateral/Vertical_{static} (-)')
    axs[0].grid()
    axs[0].legend({'Raw Data'}, loc='upper right')
    x_sin, y_sin = GenerateSin(a,b,c,d, data_x)
    # axs[1].set_title('Sinusoidal Regression')
    axs[1].grid()
    axs[1].plot(x_sin, y_sin)
    axs[1].set_xlabel('Time (s)')
    axs[1].set_ylabel('Lateral/Vertical_{static} (-)')
    axs[1].legend({'Sinusoidal Regression'}, loc='upper right')
    
    # Number of samplepoints
    N = len(data_y)
    # sample spacing
    T = 1.0 / 10.0
    yf = scipy.fftpack.fft(data_y-np.mean(data_y))
    xf = np.linspace(0.0, 1.0/(2.0*T), N//2)
    
    axs[2].set_title('Raw Data FFT')
    axs[2].plot(xf, 2.0/N * np.abs(yf[:N//2]))
    axs[2].set_xlabel('Frequency (Hz)')
    axs[2].set_ylabel('')
    axs[2].grid()
    axs[2].legend({'Raw Data FFT'}, loc='upper right')
    
    # fig.text(0.5, 0.0, 'Time (s)', ha='center')
    # fig.text(0.04, 0.5, 'Max Lateral Force (kips)', va='center', rotation='vertical')
    
    # axs[irow,icol].text(0.5*(left+right), 0.2*(bottom+top),(CarNames_list[icount]) ,
    # horizontalalignment='center',
    # verticalalignment='center',
    # fontsize=0.5, color='black',
    # rotation=0,
    # transform=axs[irow,icol].transAxes)
    
    icount = icount+1
    icol = icol+1
    if icol>(int(math.ceil((10)))-1):
        irow=irow+1
        icol = 0
            
    plt.savefig(gdir+'\\HuntingTimeSeries_'+trackside+'.png', dpi=1500,bbox_inches = "tight")
    plt.clf()
    
    
if __name__ == '__main__': 
    
    GlobalDirectory = 'C:\\CodeProjects\\SinusoidalRegression\\SinusoidalRegression'
    subdirectories = os.listdir(GlobalDirectory)

    itrain = 0
    
    data_x, data_y, a, b, c, d = ParseFiles((GlobalDirectory+"\\"+subdirectories[itrain]))
    
    PlotRaw_SubPlot(data_x, data_y, a, b, c, d, (GlobalDirectory+"\\"+subdirectories[itrain]), "Near")
    
