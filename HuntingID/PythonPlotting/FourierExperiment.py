# -*- coding: utf-8 -*-
"""
Created on Mon Nov  7 08:31:27 2022

@author: Dana
"""

import numpy as np
import matplotlib.pyplot as plt

def CreateTimeSeries(frequency, amplitude, time):
    timeseries=[]
    hz2radps = 2*3.14
    
    timeseries = np.sin(frequency*(2*3.14)*time)
    
    plt.subplot(411)
    plt.title('Individual Sinusoids')
    plt.plot(time, timeseries)
    plt.grid()
    plt.ylabel('Magnitude (-)')
    
    return timeseries



if __name__ == '__main__': 
    
    frequency = [1,2,3,4,5]
    amplitude = [1,1,1,1,1]
    timevect = np.linspace(0, 100, 100)
    
    timeseries = []
    
    fig,axs = plt.subplots(figsize =(20, 15))
    for i in range(0, len(frequency)):
        timeseries.append(CreateTimeSeries(frequency[i], 1, timevect))
        
        plt.subplot(412)
        plt.psd(timeseries[i], 1024, 1/0.01)
        
        ft = np.fft.fft(timeseries[i])
        freq = np.fft.fftfreq(100)
        huntingindex = 0
        for j in range(0, len(freq)):
            if((ft[j].imag>2) & (ft[j].imag<4)):
                mag = np.sqrt(ft[i].real**2+ft[i].imag**2)
                huntingindex = 20*np.log(mag)+huntingindex
        plt.subplot(413)
        plt.scatter(frequency[i], huntingindex, linewidth=10)
        plt.ylabel('Hunting Index', size=12)
        plt.grid()
        
        plt.subplot(414)
        plt.plot(freq, np.sqrt(ft.real**2 + ft.imag**2))
        plt.ylabel('FFT Magnitude', size=12)
        plt.grid()
    plt.subplots_adjust(top=0.9,
                    wspace=0.4,
                    hspace=0.1)
    plt.legend(labels=['1Hz','2Hz', '3Hz', '4Hz', '5Hz'], loc="right", fontsize=12)     
    plt.show()
     
    for i in range(0, len(frequency)):
        if(i==0):
            superpos_timeseries = timeseries[i]
        if(i>0):
            superpos_timeseries = np.add(superpos_timeseries, timeseries[i])
    
    fig,axs = plt.subplots(figsize =(20, 15), )
    plt.subplot(311)
    plt.title('Super Postition of Sinusoids')
    plt.plot(superpos_timeseries)
    plt.ylabel('Magnitude (-)', size=16)
    plt.grid()
    plt.subplot(312)
    plt.psd(superpos_timeseries, 2048, 1/0.01)  
    plt.xlim((0, 20))
    plt.subplot(313)
    superpos_ft = np.fft.fft(superpos_timeseries)
    superpos_freq = np.fft.fftfreq(100)
    plt.plot(freq, np.sqrt(superpos_ft.real**2 + superpos_ft.imag**2))
    plt.grid()
    plt.ylabel('FFT Magnitude', size=16)
    
    plt.show()
        