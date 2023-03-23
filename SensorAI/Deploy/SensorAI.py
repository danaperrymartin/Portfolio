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

# from multiprocessing import Process
# from multiprocessing import Pool
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
from email.mime.application import MIMEApplication

# GlobalDirectory = 'C:\\TrainScan3\\Raw\\'
GlobalDirectory = 'C:\\TrainScan3\\Raw\\'
Site = 'Thackerville'
SampleRate = 15360

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
    normalized_cutoff_freq = 2 * cutoff_freq
    [b, a] = signal.butter(n, cutoff_freq,btype='lowpass', fs=sampling_freq)
    xf = signal.filtfilt(b,a,x) #Filter the signal
    return(xf)

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
    
    cutoff_freq_1 = 750
    passband_freq = [0.85 *cutoff_freq_1, 1.2* cutoff_freq_1]
    # [b, a] = signal.butter(n, normalized_cutoff_freq,btype='bandpass',analog=False,fs=sampling_freq)
    # xf = signal.filtfilt(b,a,x) #Filter the signal
    sos = signal.cheby1(n, 5,passband_freq,btype='bandpass',analog=False,output='sos',fs=sampling_freq)
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
    # port =   # For SSL
    # smtp_server = ""
    # sender_email = ""
    # receiver_email = "danam@prorailtech.com"
    # password = ""
    
    # message = """\
    # Subject: Hi there
    
    # This message is sent from Python."""
    
    # context = ssl.create_default_context()
    # with smtplib.SMTP(smtp_server, port) as server:
    #     server.login(sender_email, password)
    #     server.sendmail(sender_email, receiver_email, message)
        
    port =   # For SSL
    smtp_server = "" #Global
    # smtp_server = "" #Local
    adminaddress = "SiteMonitor@prorailtech.com"
    password = ""
    fromaddr = ""
    recipients = ['']#,'michaelr@prorailtech.com','brianb@prorailtech.com','stevem@prorailtech.com']
    
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
            if iorient:
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
                        peaks, _ = find_peaks(movingaverage(Data[irow].ForceTimeSeries), height=6, distance=170)
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
                
                fig = df_PSD.plot(loglog=True, cmap = cm.get_cmap('tab20c') )
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
            axs[irow,icol].plot(tmp[icount].Time,movingaverage(tmp[icount].ForceTimeSeries), color='black',linewidth=0.1)
            axs[irow,icol].grid(True)
            peaks, _ = find_peaks(movingaverage(tmp[icount].ForceTimeSeries), height=6, distance=170)
            
            axs[irow,icol].text(0.5*(left+right), 0.80*(bottom+top),(str(tmp[icount].Crib)+'_'+str(len(peaks))+'Peaks Counted') ,
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
                
        plt.savefig(Unique_Sites[isite]+'TPDTimeSeries.png', dpi=2500,bbox_inches = "tight")
        plt.clf()

def Collect_Data(Truck):
    print('Collecting data for'+' '+Truck.Site+' '+Truck.Crib)
    Truck.ForceTimeSeries = Truck.Voltage2Force(np.array(Truck.ForceTimeSeries,dtype=np.float32), Truck.Gain)
    return Truck

def ParseSingleFile(root, subdirs, filename, Truck, site):
    # ForceTimeSeries = []
    # Time = []
    currentfile = open(root + '\\' + filename,'r+')
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
    
    while True:
        try:
            print('trying...')
            if os.listdir(GlobalDirectory)[0]:
                if ('.7z' in os.listdir(GlobalDirectory)[0] and '.tmp' not in os.listdir(GlobalDirectory)[0]):
                    archive = py7zr.SevenZipFile(GlobalDirectory+os.listdir(GlobalDirectory)[0],mode='r')
                    archive.extractall(path=GlobalDirectory)
                    archive.close()
                    print('Running...')
                    TPD_DataList = ParseFiles(GlobalDirectory,Site)
                    PlotRaw(TPD_DataList)
                    ID_Failures(TPD_DataList)
                    del TPD_DataList
                    os.system("del /s /q %systemdrive%\$Recycle.bin")
        except:
              print('waiting...')
              continue