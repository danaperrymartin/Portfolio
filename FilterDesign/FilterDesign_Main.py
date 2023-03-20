# -*- coding: utf-8 -*-
"""
Created on Thu Jul 29 09:28:11 2021

@author: Dana Martin
"""
# TPD Raw Data Processor 

##Standard Functions

## Custom Functions
# import pickle
# import time
import os
# import scipy
import numpy as np
# import multiprocessing as mp
import itertools
# import functools
# import operator
import pandas as pd
import shutil
import math
# import datetime
import smtplib
import py7zr
import matplotlib.pyplot as plt
# import stat
import datetime
import subprocess

# from multiprocessing import Process
# from multiprocessing import Pool
from scipy.signal import butter, lfilter
from operator import attrgetter
from pandas import DataFrame
from scipy import signal
from scipy.signal import find_peaks
from scipy import stats
from matplotlib import cm
# from email.mime.text import MIMEText
from email.mime.multipart import MIMEMultipart
# from email.mime.base import MIMEBase 
# from email import encoders
from scipy.stats import norm
from email.mime.application import MIMEApplication
from uc_Channel import Peak
from PeakFinder import FindPeaks, FindPeaks_ExternalApp

GlobalDirectory = 'C:\\TrainScan3\\Raw\\'
# GlobalDirectory = 'C:\\Raw Data\\TPD_Test\\'
Site = 'Gage'
# SampleRate = 5120
# SampleRate = 24576
SampleRate = 15000

left, width = .25, .5
bottom, height = .25, .5
right = left + width
top = bottom + height

class TPDCribInfo(object):
    def __init__(self, Site=None, ForceTimeSeries=[], Time=[], DateTime=[], Crib=None, Gain=None, Track=None):
        self.Site = Site
        self.ForceTimeSeries = []
        self.Time = []
        self.DateTime = []
        self.Crib = Crib
        self.Gain = Gain
        self.Track = Track
        
    def Voltage2Force(self, TimeSeries, Gain):
        TimeSeries = np.multiply(TimeSeries,Gain)
        return TimeSeries

def LowPass(x, n):
    sampling_freq = SampleRate
    cutoff_freq = 100
    [b, a] = signal.butter(n, cutoff_freq,btype='lowpass', fs=sampling_freq)
    xf = signal.filtfilt(b,a,x) #Filter the signal
    return(xf)

# def HighPass(x):
#     sampling_freq = SampleRate
#     cutoff_freq = 250
#     [b, a] = signal.butter(3, cutoff_freq,btype='highpass', fs=sampling_freq)
#     xf = signal.filtfilt(b,a,x) #Filter the signal
#     return(xf)

# def highpassfilter2(x):
#     fp = 250 # % pass band frequency
#     df = 500  # % transition width
#     fs = SampleRate # % sampling frequency
    
#     dfn = df/fs
#     fc = fp+df/2
#     fcn = fc/fs
    
#     N = 3.3/dfn
#     n = (N-1)/2
#     w = 0
#     for i in range(-n,n):
#         w[i+n+1]= 0.54+0.46*(math.cos(2*3.14*i/N))
    
#     hd = 0
    
#     for i in range(-n,n):
#         if (i==0):
#             hd[i+n+1]= 2*fcn
#         else:
#             hd[i+n+1] = 2*fcn*(math.sin(i*2*3.14*fcn)/(2*3.14*fcn*i))
    
#     lp = np.multiply(hd,w)
#     # g1 = fft(lp,fs)
#     # WIN = fft(w,fs)
#     # WIN2 = 20*log10(abs(WIN/max(WIN)))
#     # g2 = 20*log10(abs(g1/max(g1)))
    
#     return w
    
def highpassfilter(x):
    FREQUENCY_HIGH_PASS = 250
    i = 0
    dt = 1.0 / SampleRate  # div by sample rate
    dRC = 1.0 / (FREQUENCY_HIGH_PASS * 2.0 * 3.14)
    dAlpha = dRC / (dRC + dt)
    usNewVal = 0
    listHighPass = []
    
    # //------------------------------------------------------
    # //  Only create filtered data list if raw data exists
    # //------------------------------------------------------
    if (len(x) > 0):
        listHighPass.append(x[0])
        d1 = 0
        d2 = 0
        h1 = 0
        for i in range(1,len(x)):
            h1 = listHighPass[i - 1]
            d1 = x[i]
            d2 = x[i - 1]
            usNewVal = (dAlpha * (h1 + d1 - d2))
    
            listHighPass.append(usNewVal)
    
    return listHighPass

def movingaverage(x):
    sampling_freq = SampleRate
    iLowPassWindow = (int)(100/(24576/sampling_freq))
    
    k=0
    icount=0
    fSum=0
    fAvg=0
    xf=[]
    listvals=[]
    for i in range(iLowPassWindow):
        listvals.append(0)
        icount=icount+1
    
    for j in range(iLowPassWindow):
        listvals.append(x[j])
        fSum=fSum+x[j]
        icount=icount+1
        
    while(k<(len(x)-iLowPassWindow)):
        fAvg=fSum/icount
        xf.append(fAvg)
        fSum=fSum-listvals[0]
        listvals.pop(0)
        fSum = fSum+x[k]
        listvals.append(x[k])
        k=k+1
        
    for i in range(iLowPassWindow):
        xf.append(0)
        
    xf = np.array(xf)
    return(xf)

def bandpassfilter(x, n):
    sampling_freq = SampleRate
    
    cutoff_freq_1 = 60
    passband_freq = [cutoff_freq_1, 2* cutoff_freq_1]
    # [b, a] = signal.butter(n, passband_freq ,btype='bandpass',analog=False,fs=sampling_freq)
    # xf = signal.filtfilt(b,a,x) #Filter the signal
    sos = signal.cheby1(n, 5,passband_freq,btype='bandstop',analog=False,output='sos',fs=sampling_freq)
    xf = signal.sosfiltfilt(sos,x) #Filter the signal
    
    # cutoff_freq_2 = 1400
    # passband_freq2 = [0.85 * cutoff_freq_2, 1.1* cutoff_freq_2]
    # # [b, a] = signal.butter(n, normalized_cutoff_freq2,btype='bandpass',analog=False,fs=sampling_freq)
    # # xf = signal.filtfilt(b,a,x) #Filter the signal
    # sos = signal.cheby1(n, 5,passband_freq2,btype='bandpass',analog=False,output='sos',fs=sampling_freq)
    # xf = signal.sosfiltfilt(sos,x) #Filter the signal
    
    # cutoff_freq_3 = 2250
    # passband_freq3 = [0.85 * cutoff_freq_3 / sampling_freq, 1.1* cutoff_freq_3 / sampling_freq]
    # # [b, a] = signal.butter(n, normalized_cutoff_freq3,btype='bandpass',analog=False,fs=sampling_freq)
    # # xf = signal.filtfilt(b,a,x) #Filter the signal
    # sos = signal.cheby1(n, 5, passband_freq3,btype='bandpass',analog=False,output='sos',fs=sampling_freq)
    # xf = signal.sosfiltfilt(sos,x) #Filter the signal
    
    return(xf)

def ProcessDuplicates(TPD_DataList,isite):
    Unique_Sites, indices = np.unique(list(map(attrgetter('Site'),TPD_DataList)), return_index=True)
    tmp = [item for item in TPD_DataList if item.Site == Unique_Sites[isite]]
    Unique_Cribs, idx, count = np.unique(list(map(attrgetter('Crib'),tmp)), return_index=True, return_counts=True)
    
    Duplicate_CribNames = Unique_Cribs[count>1]
    # Duplicate_idx = idx[count>1]
    
    for idup in Duplicate_CribNames:
       id_dup = [i for i, s in enumerate(list(map(attrgetter('Crib'),tmp))) if idup in s]
       icnt = 1
       while icnt<len(id_dup):
           tmp[id_dup[0]].ForceTimeSeries = np.append(tmp[id_dup[0]].ForceTimeSeries,tmp[id_dup[icnt]].ForceTimeSeries)
           # np.append(tmp[id_dup[0]].Time,tmp[id_dup[icnt]].Time)
           tmp[id_dup[0]].Time.extend(list(np.linspace(tmp[id_dup[0]].Time[-1],tmp[id_dup[0]].Time[-1]+tmp[id_dup[icnt]].Time[-1],num=len(tmp[id_dup[icnt]].Time))))
           tmp.remove(tmp[id_dup[icnt]])
           icnt = icnt+1
    return tmp
    
def SeparateOrientations(TPD_DataList):
    tmp = [[] for _ in range(2)]
    for x in TPD_DataList:
        if 'Vertical' in x.Crib:
            print('Found vertical??? ' + x.Crib)
            tmp[0].append(x)
        else:
           print('Found lateral??? ' + x.Crib)
           tmp[1].append(x)
    return tmp

def SendEmail(CatastrophicFailures,Site,filenames):
    # port = 25  # For SSL
    # smtp_server = "192.168.0.2"
    # sender_email = "SiteMonitor@prorailtech.com"
    # receiver_email = "danam@prorailtech.com"
    # password = "Prt12345"
    
    # message = """\
    # Subject: Hi there
    
    # This message is sent from Python."""
    
    # context = ssl.create_default_context()
    # with smtplib.SMTP(smtp_server, port) as server:
    #     server.login(sender_email, password)
    #     server.sendmail(sender_email, receiver_email, message)
        
    port = 25  # For SSL
    # smtp_server = "96.69.156.94" #Global
    smtp_server = "192.168.0.2" #Local
    adminaddress = "SiteMonitor@prorailtech.com"
    password = "Prt12345"
    fromaddr = "danam@prorailtech.com"
    recipients = ['danam@prorailtech.com']#,'michaelr@prorailtech.com','brianb@prorailtech.com','stevem@prorailtech.com']
    
    for irecipient in recipients:
        # instance of MIMEMultipart 
        msg = MIMEMultipart() 
          
        # storing the senders email address   
        msg['From'] = fromaddr 
          
        # storing the receivers email address  
        msg['To'] = irecipient
          
        # storing the subject  
        msg['Subject'] = ('SensorAI - '+Site+' Catastrophic Sensor Failure')
          
        # string to store the body of the mail 
        # body = DataFrame(CatastrophicFailures,columns=['Catastrophically Failing Sensors']).to_html()
        
        for f in filenames:
            #file_path = os.path.join(filename, filename)
            attachment = MIMEApplication(open(f, "rb").read())
            attachment.add_header('Content-Disposition','attachment', filename=f)
            msg.attach(attachment)
            
            # attach the body with the msg instance 
            #msg.attach(MIMEText(body, 'html'))
            # instance of MIMEBase and named as p 
            #p = MIMEBase('application', 'octet-stream') 
              
            # To change the payload into encoded form 
            # p.set_payload((attachment).read()) 
              
            # encode into base64 
            # encoders.encode_base64(p) 
              
            # open the file to be sent  
            # filename = "FailureHistoryTestFile.txt"
            # try:
            #attachment = open(filename, "r").read() 
            # p.add_header('Content-Disposition', "attachment; filename= FailureHistory.sens") 
              
            # except:
            #     attachment = open(filename, "rb").read()
            #     p.add_header('Content-Disposition', "attachment; filename= TPDTimeSeries.png") 
                
            #msg.attach(p)  
            
            # attach the instance 'p' to instance 'msg' 
            # msg.attach(p) 
            
        # creates SMTP session 
        s = smtplib.SMTP(smtp_server, port) 
          
        # start TLS for security 
        # s.starttls() 
          
        # Authentication 
        s.login(adminaddress, password) 
          
        # Converts the Multipart msg into a string 
        text = msg.as_string() 
          
        # sending the mail 
        s.sendmail(fromaddr, irecipient, text) 
          
        # terminating the session 
        s.quit() 
            
def ID_Failures(TPD_DataList):
    CatastrophicFailures = []
    Unique_Sites, indices = np.unique(list(map(attrgetter('Site'),TPD_DataList)), return_index=True)
    NumCribs = [[] for _ in range(len(Unique_Sites))]
    for isite in range(len(Unique_Sites)):
        tmp = ProcessDuplicates(TPD_DataList,isite)
        NumCribs[isite] = len(tmp)
        Data = tmp
        row_lengths = []
        for irow in list(map(attrgetter('ForceTimeSeries'),Data)):
            row_lengths.append(len(irow))
            
        max_length = max(row_lengths)
        ForceTimeSeries = np.zeros((max_length,len(Data)))
        ForcePSD_f = []
        ForcePSD_Pxx = []
        Force_PeakCount = []
        df_PeakCount = []
        icol=0
        fs = Data[1].Time[1]
        for irow in range(len(Data)):
            fs = 1/Data[irow].Time[1]
            ForceTimeSeries[0:len(Data[irow].ForceTimeSeries),icol] = Data[irow].ForceTimeSeries
            tmp_f,tmp_Pxx = signal.welch(Data[irow].ForceTimeSeries,fs,nperseg=100, scaling='spectrum')
            ForcePSD_f.append(tmp_f)
            ForcePSD_Pxx.append(tmp_Pxx)
            icol=icol+1
        df_PSD   = DataFrame(np.array(ForcePSD_Pxx).T,index=np.array(ForcePSD_f[0]),columns=list(map(attrgetter('Crib'),Data)))
         
        tmp_sep = SeparateOrientations(tmp)
        icntr = 0
        for iorient in tmp_sep:
           if tmp_sep[icntr]: 
                if icntr==0:
                    CribOrientation = 'Vertical'
                else:
                    CribOrientation = 'Lateral'
                    
                NumCribs[isite] = len(iorient)
                Data = iorient
                row_lengths = []
                for irow in list(map(attrgetter('ForceTimeSeries'),Data)):
                    row_lengths.append(len(irow))
                    
                max_length = max(row_lengths)
                ForceTimeSeries = np.zeros((max_length,len(Data)))
                ForcePSD_f = []
                ForcePSD_Pxx = []
                
                icol=0
                fs = Data[0].Time[1]
                for irow in range(len(Data)):
                    fs = 1/Data[irow].Time[1]
                    ForceTimeSeries[0:len(Data[irow].ForceTimeSeries),icol] = Data[irow].ForceTimeSeries
                    tmp_f,tmp_Pxx = signal.welch(Data[irow].ForceTimeSeries,fs,nperseg=100, scaling='spectrum')
                    if icntr==0:
                        # peaks, _ = find_peaks(bandpassfilter(Data[irow].ForceTimeSeries,2), height=6, distance=170)
                        peaks = FindPeaks_ExternalApp(Data[irow].ForceTimeSeries, SampleRate)
                        Force_PeakCount.append(len(peaks))
                    ForcePSD_f.append(tmp_f)
                    ForcePSD_Pxx.append(tmp_Pxx)
                    icol=icol+1
                df_PSD   = DataFrame(np.array(ForcePSD_Pxx).T,index=np.array(ForcePSD_f[0]),columns=list(map(attrgetter('Crib'),Data)))
                      
                if icntr==0:
                    df_VertPSDMedian = df_PSD.median(axis=1)
                    df_PeakCount = DataFrame([Force_PeakCount],columns=list(map(attrgetter('Crib'),Data)))
                    #====Identify Catstrophic Failures using Peak Count metric========
                    CatastrophicFailures.append(df_PeakCount[((0.9*int(stats.mode(Force_PeakCount)[0])>df_PeakCount)| (df_PeakCount>1.1*int(stats.mode(Force_PeakCount)[0])))].columns)
            
                    # df_VertPSDMedian.plot(color="black",label=('Median_'+CribOrientation))
                    df_ErrorPSD = df_PSD.subtract(df_VertPSDMedian,axis=0)
                else:
                    df_LatPSDMedian = df_PSD.median(axis=1)
                    # df_LatPSDMedian.plot(color="black",label=('Median_'+CribOrientation))
                    df_ErrorPSD = df_PSD.subtract(df_LatPSDMedian,axis=0)
                
                fig = df_PSD.plot(logx=False, logy=True, cmap = cm.get_cmap('tab20c') )
                plt.legend(bbox_to_anchor=(1.0, 1.0))
                # ax.set_ylim(0,1)
                # ax.set_xlim(0,500)
                fig.set_xlabel("frequency (Hz)")
                fig.set_ylabel("PSD [V**2/Hz]")
                plt.savefig(('PSD_'+Unique_Sites[isite]+CribOrientation+'.png'), dpi=2500, bbox_inches = "tight")
                
                fig2 = df_ErrorPSD.plot(loglog=True, cmap = cm.get_cmap('tab20c'),title=(CribOrientation+' Error Plot'))
                plt.legend(bbox_to_anchor=(1.0, 1.0))
                fig2.set_xlabel("frequency (Hz)")
                fig2.set_ylabel("Residual from Median PSD [V**2/Hz]")
                plt.savefig(('PSDError_'+Unique_Sites[isite]+CribOrientation+'.png'), dpi=2500, bbox_inches = "tight")
                #====Identify Catstrophic Failures using PSD metric========
                CatastrophicFailures.append(df_ErrorPSD.columns[df_ErrorPSD.iloc[1,:]>2])
                icntr = icntr+1
        CatastrophicFailures = list(itertools.chain(*CatastrophicFailures)) 
        if CatastrophicFailures:
            failurehistory_new = DataFrame(columns = ['Crib','CumulativeFailures'])
            filenames = [(os.getcwd()+"\\FailureHistoryFile.txt"),(os.getcwd()+"\\"+Site+"TPDTimeSeries.png")]
            if os.path.isfile(filenames[0]):
                f = open(filenames[0], "r+")
            else:
                f = open(filenames[0],"w+")
            failurehistory=f.readlines()[1:]
            f.close()
            tmp_failhist = [i.split('\t') for i in failurehistory]
            tmp_failhist = DataFrame([j[1:] for j in tmp_failhist],columns=['Crib','CumulativeFailures'])
            icnt=0
            for ifail in CatastrophicFailures:
                tmp_idx = tmp_failhist['Crib'].str.contains(ifail)
                idx = tmp_idx[tmp_idx].index
                totalfailures = 1
                for index in idx:
                    totalfailures = totalfailures+int(tmp_failhist['CumulativeFailures'][index])
                    tmp_failhist = tmp_failhist.drop([index],axis=0)
                failurehistory_new.loc[icnt]=([ifail,totalfailures])
                icnt = icnt+1
            if not tmp_failhist.empty:
                tmp_failhist['CumulativeFailures']=tmp_failhist['CumulativeFailures'].astype(int)
                failurehistory_new=failurehistory_new.append(tmp_failhist)
            f = open(filenames[0], "w")
            failurehistory_new.to_csv(filenames[0],sep='\t')
            f.close()        
            SendEmail(CatastrophicFailures,Unique_Sites[isite],filenames)

def TunePeakCounter(TPD_DataList):
    from sklearn import linear_model
    import statsmodels.api as sm
    import statistics
    import random
    
    Unique_Sites, indices = np.unique(list(map(attrgetter('Site'),TPD_DataList)), return_index=True)
    NumCribs = [[] for _ in range(len(Unique_Sites))]
    for isite in range(len(Unique_Sites)):
        tmp = ProcessDuplicates(TPD_DataList,isite)
        NumCribs[isite] = len(tmp)
        icol = 0
        irow = 0
        icount = 0
        print('Tuning Analysis...'+Unique_Sites[isite])
        
        True_Peaks = 420
        
        iDIV_List=[]
        iDIV = 100
        iDIV_Range = range(50,200,10)
        
        MAXSLOPE_List = []
        MAXSLOPE = 65
        MAXSLOPE_Range = range(35,105,10)
        
        fPPV_List = []
        fPPV = 54
        fPPV_Range = range(34,104,10)
        
        WINDOWWIDTH_List = []
        WINDOW_WIDTH = 50
        WINDOW_WIDTH_Range = range(30,100,10)
        
        grad_iDIV = 0
        grad_MAXSLOPE = 0
        grad_fPPV = 0
        grad_WINDOW_WIDTH = 0
            
        Parameter_Combo_List = []
        stdlpfpeaks_list = []
        # lpfpeaks_list = []
        critical_length = 0
        eta_lr = 10
        
        fig,axs = plt.subplots(1,1,sharex=True, sharey=True)
        fig.text(0.5, 0.0, 'Iteration', ha='center')
        fig.text(0.04, 0.5, 'Std Peak Count', va='center', rotation='vertical')
        fig.suptitle('Peak Counter Tuning')
        inotwork = 0
        iwork = 0
        min_delta = 10
        delta=100
        delta_list=[]
        delta_noise = 1
        while ((critical_length<100) & (delta>min_delta)):
                    
            icount=0
            
            if ((critical_length<10) | (inotwork>2)):
                grad_descent = False
                
                iDIV = int(random.sample(iDIV_Range,1)[0])
                MAXSLOPE = int(random.sample(MAXSLOPE_Range,1)[0])
                fPPV = int(random.sample(fPPV_Range,1)[0])
                WINDOW_WIDTH = int(random.sample(WINDOW_WIDTH_Range,1)[0])
                
                Parameter_Combo = [iDIV,MAXSLOPE,fPPV,WINDOW_WIDTH]
                
                while (Parameter_Combo in Parameter_Combo_List):
                    iDIV = int(random.sample(iDIV_Range,1)[0])
                    MAXSLOPE = int(random.sample(MAXSLOPE_Range,1)[0])
                    fPPV = int(random.sample(fPPV_Range,1)[0])
                    WINDOW_WIDTH = int(random.sample(WINDOW_WIDTH_Range,1)[0])
                    Parameter_Combo = [iDIV,MAXSLOPE,fPPV,WINDOW_WIDTH]
                
                Parameter_Combo_List.append(Parameter_Combo)
                
            else:
                grad_descent = True
                idx = delta_list.index(min(delta_list))
                Parameter_Combo = Parameter_Combo_List[idx]
                
                ## Computing gradient and choosing update values
                grad_iDIV = delta/((iDIV_List[idx]-iDIV_List[idx-1])+delta_noise)
                grad_MAXSLOPE = delta/((MAXSLOPE_List[idx]-MAXSLOPE_List[idx-1])+delta_noise)
                grad_fPPV = delta/((fPPV_List[idx]-fPPV_List[idx-1])+delta_noise)
                grad_WINDOW_WIDTH = delta/((WINDOWWIDTH_List[idx]-WINDOWWIDTH_List[idx-1])+delta_noise)
                                
                iDIV =  int(iDIV_List[idx] - eta_lr*grad_iDIV)
                MAXSLOPE = int(MAXSLOPE_List[idx] - eta_lr*grad_MAXSLOPE)
                fPPV = int(fPPV_List[idx] - eta_lr*grad_fPPV)
                WINDOW_WIDTH = int(WINDOWWIDTH_List[idx] - eta_lr*grad_WINDOW_WIDTH)
                
                Parameter_Combo = [iDIV,MAXSLOPE,fPPV,WINDOW_WIDTH]
                Parameter_Combo_List.append(Parameter_Combo)
            lpfpeaks_list = []    
            while icount<NumCribs[isite]:
                
                lpfpeaks = FindPeaks_ExternalApp(tmp[icount].ForceTimeSeries, SampleRate, int(iDIV), int(MAXSLOPE),int(fPPV), int(WINDOW_WIDTH))
                
                if (len(lpfpeaks)<=1):
                    inotwork+=1
                    iwork=0
                    print('Parameters did not work')
                    badparameterset = True
                    break
                else:
                    inotwork=0
                    iwork+=1
                    lpfpeaks_list.append(len(lpfpeaks))
                    badparameterset=False
                    # np.append(np.array(lpfpeaks), lpfpeaks, axis=1)
                    
                icount = icount+1
                icol = icol+1
                if icol>(int(math.ceil((math.sqrt(NumCribs[isite]))))-1):
                    irow=irow+1
                    icol = 0
                                     
            if not (badparameterset):
                stdlpfpeaks_list.append(statistics.pstdev(lpfpeaks_list))
                delta = np.median(np.add(abs(np.subtract(True_Peaks,lpfpeaks_list)),abs((stdlpfpeaks_list[critical_length]-stdlpfpeaks_list[critical_length-1]))))
                delta_list.append(delta)
                    
                axs.plot(critical_length,delta_list[len(delta_list)-1], 'k-', linewidth=0.5)
                axs.grid(True)
                
                iDIV_List.append(iDIV)
                MAXSLOPE_List.append(MAXSLOPE)      
                fPPV_List.append(fPPV)
                WINDOWWIDTH_List.append(WINDOW_WIDTH)
            try:
                critical_length = (np.array(stdlpfpeaks_list)[stdlpfpeaks_list!=float('NaN')]).size   
                print(critical_length)
            except:
                continue
    
    PeakCountperCrib = {'Cost':delta_list,
                        'stdPeaks': stdlpfpeaks_list,
                        'iDIV': iDIV_List,
                        'fPPV': fPPV_List,
                        'WINDOW_WIDTH': WINDOWWIDTH_List,
                        'MAXSLOPE': MAXSLOPE_List}
    
    df = pd.DataFrame(PeakCountperCrib, columns=['Cost','stdPeaks', 'iDIV', 'fPPV', 'WINDOW_WIDTH', 'MAXSLOPE'])
    df = df.dropna()
    
    X = df[['iDIV','fPPV','WINDOW_WIDTH']] # here we have 2 variables for multiple regression. If you just want to use one variable for simple linear regression, then use X = df['Interest_Rate'] for example.Alternatively, you may add additional variables within the brackets
    Y = df['Cost']
    
    X = sm.add_constant(X)
    model = sm.OLS(Y, X).fit()
    print_model = model.summary()
    print(print_model)
    
    df.reset_index().plot(x='index', y='Cost', kind='scatter')
    
    plt.show()
    plt.savefig(Unique_Sites[isite]+'TuningConvergence.png', dpi=2500,bbox_inches = "tight")
    plt.clf()
    
    return model, df
    
def TestPeakCounter(TPD_DataList):
    from sklearn import linear_model
    import statsmodels.api as sm
    import statistics
    import random
    
    Unique_Sites, indices = np.unique(list(map(attrgetter('Site'),TPD_DataList)), return_index=True)
    NumCribs = [[] for _ in range(len(Unique_Sites))]
    for isite in range(len(Unique_Sites)):
        tmp = ProcessDuplicates(TPD_DataList,isite)
        NumCribs[isite] = len(tmp)
        icol = 0
        irow = 0
        icount = 0
        print('Testing Peak Counter...'+Unique_Sites[isite])
        
        iDIV = 10#50#170#60#50#60
        fPPV = 54 #94#84#84#94
        WINDOW_WIDTH = SampleRate/50/iDIV #50#30#90#30#80
        MAXSLOPE = 35
        
        fig,axs = plt.subplots(int(math.ceil((math.sqrt(NumCribs[isite])))),int(math.ceil(math.sqrt(NumCribs[isite]))),sharex=True, sharey=True)
        fig.text(0.5, 0.0, 'Iteration', ha='center')
        fig.text(0.04, 0.5, 'Std Peak Count', va='center', rotation='vertical')
        fig.suptitle('Peak Counter Tuning')
        
        lpfpeaks_list = []
        
        icount=0
         
        while icount<NumCribs[isite]:
            if('Vertical' in (tmp[icount].Crib)):
                print('Counting peaks for '+ tmp[icount].Crib)
                lpfpeaks = FindPeaks_ExternalApp(tmp[icount].ForceTimeSeries, SampleRate, int(iDIV), int(MAXSLOPE),int(fPPV), int(WINDOW_WIDTH))
                    
                axs[irow,icol].plot(tmp[icount].Time,tmp[icount].ForceTimeSeries, 'k-', np.asarray(tmp[icount].Time)[lpfpeaks],tmp[icount].ForceTimeSeries[lpfpeaks],'r+', linewidth=0.1)
                axs[irow,icol].grid(True)
                
                axs[irow,icol].text(0.5*(left+right), 0.80*(bottom+top),(str(tmp[icount].Crib)+'_'+('lpf'+str(len(lpfpeaks))+' Peaks Counted')) ,
                horizontalalignment='center',
                verticalalignment='center',
                fontsize=2, color='black',
                rotation=0,
                transform=axs[irow,icol].transAxes)
            icount = icount+1
            icol = icol+1
            if icol>(int(math.ceil((math.sqrt(NumCribs[isite]))))-1):
                irow=irow+1
                icol = 0
            
    plt.savefig(Unique_Sites[isite]+'TunedPeakCounter.png', dpi=2500,bbox_inches = "tight")
    plt.clf()
    
def PlotRaw(TPD_DataList):
    Unique_Sites, indices = np.unique(list(map(attrgetter('Site'),TPD_DataList)), return_index=True)
    NumCribs = [[] for _ in range(len(Unique_Sites))]
    for isite in range(len(Unique_Sites)):
        tmp = ProcessDuplicates(TPD_DataList,isite)
        NumCribs[isite] = len(tmp)
        fig,axs = plt.subplots(int(math.ceil((math.sqrt(NumCribs[isite])))),int(math.ceil(math.sqrt(NumCribs[isite]))),sharex=True, sharey=True)
        fig.text(0.5, 0.0, 'Time (s)', ha='center')
        fig.text(0.04, 0.5, 'Force (kips)', va='center', rotation='vertical')
        fig.suptitle(Unique_Sites[isite])
        icol = 0
        irow = 0
        icount = 0
        print('Plotting raw time series for...'+Unique_Sites[isite])
        while icount<NumCribs[isite]:
            
            # bpfpeaks, _ = find_peaks(bandpassfilter(tmp[icount].ForceTimeSeries,3), height=2)
            # lpfpeaks, _ = find_peaks(movingaverage(tmp[icount].ForceTimeSeries),height=5)
            # peak_thresh =  max(tmp[icount].ForceTimeSeries)*0.8
            # lowpass_peak_thresh = 0
            # bpfpeaks = FindPeaks(tmp[icount].ForceTimeSeries,bandpassfilter(tmp[icount].ForceTimeSeries,3),peak_thresh,lowpass_peak_thresh)
            # bpfpeaks = 0
            # now = datetime.datetime.now()
            # dt_string = now.strftime("%d/%m/%Y %H:%M:%S")
            # ForceTimeSeries_string = [str(x) for x in tmp[icount].ForceTimeSeries]
            # Peaks= (TrainScan3_Peaks.stdout).split('\r\n')
            # peak_thresh = 15 
            # lowpass_peak_thresh = 2
            # lpfpeaks = FindPeaks(tmp[icount].ForceTimeSeries,movingaverage(tmp[icount].ForceTimeSeries),peak_thresh,lowpass_peak_thresh)
            iDIV = 50
            MAXSLOPE = 35
            fPPV = 54
            WINDOW_WIDTH = (SampleRate/50)/iDIV
            
            lpfpeaks = FindPeaks_ExternalApp(tmp[icount].ForceTimeSeries, SampleRate, int(iDIV), int(MAXSLOPE),int(fPPV), int(WINDOW_WIDTH))
            hpfpeaks = FindPeaks_ExternalApp(highpassfilter(tmp[icount].ForceTimeSeries), SampleRate, int(iDIV), int(MAXSLOPE),int(fPPV), int(WINDOW_WIDTH))
            
            # axs[irow,icol].plot(tmp[icount].Time,tmp[icount].ForceTimeSeries, 'b-',lpfpeaks,tmp[icount].ForceTimeSeries[lpfpeaks],'bs',hpfpeaks,tmp[icount].ForceTimeSeries[hpfpeaks],'g^')
            axs[irow,icol].plot(tmp[icount].Time,tmp[icount].ForceTimeSeries, 'k-', np.asarray(tmp[icount].Time)[lpfpeaks],tmp[icount].ForceTimeSeries[lpfpeaks],'r+', linewidth=0.1)
            axs[irow,icol].grid(True)
            
            # axs[irow,icol].plot(lpfpeaks,tmp[icount].ForceTimeSeries[lpfpeaks],2, color='blue', marker='o',fillstyle='full', linewidth=0.1)
            # axs[irow,icol].plot(hpfpeaks,tmp[icount].ForceTimeSeries[hpfpeaks],2, color='red', marker='X',fillstyle='full', linewidth=0.1)
            
            axs[irow,icol].text(0.5*(left+right), 0.80*(bottom+top),(str(tmp[icount].Crib)+'_'+('hpf'+str(len(hpfpeaks))+'_'+'lpf'+str(len(lpfpeaks))+' Peaks Counted')) ,
            horizontalalignment='center',
            verticalalignment='center',
            fontsize=2, color='black',
            rotation=0,
            transform=axs[irow,icol].transAxes)
            
            icount = icount+1
            icol = icol+1
            if icol>(int(math.ceil((math.sqrt(NumCribs[isite]))))-1):
                irow=irow+1
                icol = 0
                
        plt.savefig(Unique_Sites[isite]+'RawTimeSeries.png', dpi=2500,bbox_inches = "tight")
        plt.clf()
        
def PlotSlopeHistogram(TPD_DataList):
    Unique_Sites, indices = np.unique(list(map(attrgetter('Site'),TPD_DataList)), return_index=True)
    NumCribs = [[] for _ in range(len(Unique_Sites))]
    for isite in range(len(Unique_Sites)):
        tmp = ProcessDuplicates(TPD_DataList,isite)
        NumCribs[isite] = len(tmp)
        
        icol = 0
        irow = 0
        icount = 0
        print('Plotting histogram analysis series for...'+Unique_Sites[isite])
        while icount<NumCribs[isite]:
            
            fig,axs = plt.subplots(1,1,sharex=True, sharey=True)
            fig.text(0.5, 0.0, 'Slope (deg)', ha='center')
            fig.text(0.04, 0.5, '# Occurrences', va='center', rotation='vertical')
            fig.suptitle(Unique_Sites[isite])
            
            listlowpassdata = highpassfilter(movingaverage(tmp[icount].ForceTimeSeries))
            WINDOW_WIDTH = int((SampleRate/50)/10)
            fSlope=[]
            j=WINDOW_WIDTH
            while((j>=0) & ((j- WINDOW_WIDTH) >= 0) & ((j+WINDOW_WIDTH)<len(listlowpassdata))):
                fSlope.append(math.atan(np.multiply(listlowpassdata[j],54)-np.multiply(listlowpassdata[j-WINDOW_WIDTH],54)/WINDOW_WIDTH)*(180 / 3.14))
                j+= WINDOW_WIDTH
            
            # occurrences,bin_edges,_= plt.hist(fSlope,bins=10)
            fSlope = np.array(fSlope)
            fSlope = fSlope[ (abs(fSlope) >= 20)]
            fSlope = list(fSlope)
            
            plt.hist(fSlope,bins=20,edgecolor = "black")
            
            mu, std = norm.fit(fSlope)
            # xmin, xmax = plt.xlim()
            x = np.linspace(-40, 40, 100)
            p = norm.pdf(x, mu, std)
            # axs.plot(x, norm.pdf(x, mu, std), linewidth=2)
            title = "Fit results: mu = %.2f,  std = %.2f" % (mu, std)
            plt.title(title)
            
            # axs.hist(fSlope,bins=10, color='black')
            axs.grid(True)
            
            axs.text(0.5*(left+right), 0.80*(bottom+top),(str(tmp[icount].Crib)),
            horizontalalignment='center',
            verticalalignment='center',
            fontsize=2, color='yellow',
            rotation=0,
            transform=axs.transAxes)
            
            plt.savefig((Unique_Sites[isite]+'HistogramSlope_'+tmp[icount].Crib.replace(" ","")+'.png'), dpi=2500,bbox_inches = "tight")
            plt.close()
            plt.clf()
            
            icount = icount+1
            icol = icol+1
            if icol>(int(math.ceil((math.sqrt(NumCribs[isite]))))-1):
                irow=irow+1
                icol = 0
                
def Collect_Data(Truck):
    print('Collecting data for'+' '+Truck.Site+' '+Truck.Crib)
    Truck.ForceTimeSeries = Truck.Voltage2Force(np.array(Truck.ForceTimeSeries,dtype=np.float32), Truck.Gain)
    return Truck
    
def ParseSingleFile(root, subdirs, filename, Truck, site):
    # ForceTimeSeries = []
    # Time = []
    currentfile = open(root + '/' + filename,'r+')
    tmpstr = root.split('\\')
    Truck.Site = site
    tmpstr = tmpstr[-1].split('_')
    Truck.Track = tmpstr[-1]
    with currentfile as activefile:
        content = activefile.readlines()
        content = [x.strip() for x in content]
        iline=0
        line = content[iline]
        tmp_values = line.split('=')
        Truck.Gain = float(tmp_values[1].strip())        
        Truck.Crib = str(tmpstr[-1]+' '+ filename.replace('.txt',''))
        iline = 2
        tmp_values = content[iline].split('=')
        numbersamples = int(tmp_values[1])
        iline = iline+1
        tmp_values = content[iline].split('=')
        samplerate = int(tmp_values[1])
        Truck.Time = [0+ 1/samplerate*interval for interval in range(numbersamples) ]
        Truck.DateTime = pd.to_datetime(Truck.Time)
        iline = 4
        while iline < len(content):
            line = content[iline]
            Truck.ForceTimeSeries.append(line)
            iline=iline+1
        currentfile.close()
    return Truck

def ParseFiles(dir1,site):
    print('In')
    TPD_DataList = []
    exclude = ['Archive','072821']
    for root, subdirs, files in os.walk(dir1):
        if not any(([string in root for string in exclude])):
            if any('.txt' in ifile for ifile in files):
                # pool = mp.Pool(mp.cpu_count())
                # Truck = TPDCribInfo()
                # results = [pool.apply_async(ParseSingleFile,args=(root, subdirs, ifile, Truck),callback=Collect_Data).get() for ifile in files]
                # TPD_DataList.append(results)
                # print("Submitted tasks to pool")
                # pool.close()
                # pool.join()
                for ifile in files:
                    if not ('.7z' in ifile):
                        Truck = TPDCribInfo()
                        result = ParseSingleFile(root, subdirs, ifile, Truck,site)
                        result = Collect_Data(result)
                        TPD_DataList.append(result)
    # TPD_DataList = list(itertools.chain(*TPD_DataList))   
    # ListFile = open("TPD_DataList.pkl","wb")
    # pickle.dump(TPD_DataList, ListFile)
    # ListFile.close()
    
    for item in os.listdir(dir1):
        try:
            shutil.rmtree(dir1+item) 
        except:
            os.remove(dir1+item)
        # os.chmod( dir1+item, stat.S_IWRITE )
        # os.unlink( dir1+item )
        # os.remove(dir1+item)
            
    return TPD_DataList
    
if __name__ == '__main__': 
    
    # while True:
    #     try:
    #         print('trying...')
    if os.listdir(GlobalDirectory)[0]:
        if ('.7z' in os.listdir(GlobalDirectory)[0] and '.tmp' not in os.listdir(GlobalDirectory)[0]):
            archive = py7zr.SevenZipFile(GlobalDirectory+os.listdir(GlobalDirectory)[0],mode='r')
            archive.extractall(path=GlobalDirectory)
            archive.close()
            print('Running...')
            TPD_DataList = ParseFiles(GlobalDirectory,Site)
            
            # PlotSlopeHistogram(TPD_DataList)
            
            # PlotRaw(TPD_DataList)
            # model, df = TunePeakCounter(TPD_DataList)
            # minvalue = df[ df['Cost']==df['Cost'].min()]
            TestPeakCounter(TPD_DataList)
            # ID_Failures(TPD_DataList)
            del TPD_DataList
            os.system("del /s /q %systemdrive%\$Recycle.bin")
        # except:
        #       print('waiting...')
        #       continue