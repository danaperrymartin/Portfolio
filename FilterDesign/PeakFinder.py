# -*- coding: utf-8 -*-
"""
Created on Thu May 26 11:41:55 2022

@author: Dana
"""
import datetime
import numpy as np
import subprocess
import pandas as pd
import os

from uc_Channel import Peak
from subprocess import PIPE


def FindPeaks_ExternalApp(InputData, iSampleRate, iDIV, MAXSLOPE,fPPV, WINDOW_WIDTH ):
    
    ListPeaks = []
    
    ForceFile = ".\\PeakFinder\\bin\\x64\\Release\\net5.0\\ForceData.txt"
    TrainScanPeakFile = ".\\PeakFinder\\bin\\x64\\Release\\net5.0\\TrainScanPeaks.txt"
    df = pd.DataFrame(InputData)
    df.to_csv(ForceFile, sep=',',index=False)
    try:
        # InputData_str = [str(i) for i in InputData if i]
        # InputData_str=','.join(InputData_str)
        proc = subprocess.Popen([".\\PeakFinder\\bin\\x64\\Release\\net5.0\\PeakFinder.exe", str(iSampleRate), str(iDIV), str(MAXSLOPE), str(fPPV), str(WINDOW_WIDTH)],stdin=PIPE,stdout=PIPE)
        out = proc.communicate(timeout=60)
        PeakIdxs=(out[0].decode('utf-8')).split('\r\n')
        
        # PeakIdxs = subprocess.run([".\\PeakFinder\\bin\\x64\\Release\\net5.0\\PeakFinder.exe", str(iSampleRate)],capture_output=True)
        # PeakIdxs=(PeakIdxs.stdout.decode('utf-8')).split('\r\n')
        
        # peakfile = open(TrainScanPeakFile,'r')
        # with peakfile as activefile:
        #     content = activefile.readlines()
        
        ListPeaks = [int(i) for i in PeakIdxs if i]
        
        os.remove(ForceFile)
        # os.remove(TrainScanPeakFile)
    except:
        print('Error in peak counting...')
        return ListPeaks
    return ListPeaks

def FindPeaks(data, data_filtered, peak_thresh, lowpass_peak_thresh, SampleRate):
    ListPeaks = []
    DataList = []
    listLowPass = []
    fDiv = 24576/SampleRate
    iLowPassWindow = 100/fDiv
    # listLowPass = movingaverage(data)
    FindNextPeakLowFilter(data, data_filtered, SampleRate, peak_thresh, lowpass_peak_thresh)
    
    return ListPeaks

def FindNextPeakLowFilter(data,listLowPassData, SampleRate, peak_thresh,lowpass_peak_thresh):
    i=0
    j=0
    iLeft=0
    iRight=0
    iPBW=SampleRate
    iDIV=10
    WINDOW_WIDTH = iPBW/iDIV
    MAX_SLOPE = 35
    MAX_SLOPE= MAX_SLOPE-5
    # fPPV=54
    fSlope=0
    fRadToDeg = 180/3.14
    # listPeaks = []
    DataList = data
    dtStartTime = datetime.MINYEAR
    
    bFound=False
    
    while(i<len(listLowPassData)):
        print(i)
        if ((iLeft == 0) & (listLowPassData[i] > peak_thresh)):
            iLeft = i
        elif ((iLeft != 0) & (listLowPassData[i] < peak_thresh)):
            iRight = i
            bFound = True
            continue
        i+=1
        
    if ((iRight > 0) & (bFound == True)):
        iPBW = iRight - iLeft
        j = iLeft
        WINDOW_WIDTH = iPBW / iDIV
    
        bFound = False
    
        # \\------------------------------------------------------
        # \\  Expand left
        # \\------------------------------------------------------
        while ((bFound == False) & (j >= 0) & ((j- WINDOW_WIDTH) >= 0)):
            fSlope = (listLowPassData[j] - listLowPassData[j - WINDOW_WIDTH]) / WINDOW_WIDTH
    
            # \\------------------------------------------------------
            # \\  Convert to degrees
            # \\------------------------------------------------------
            fSlope = np.arctan(fSlope) * fRadToDeg
            if (fSlope < MAX_SLOPE):
                iLeft = j
                bFound = True
                continue;
            j-=1
    
        bFound = False
        j = iRight
    
        # \\------------------------------------------------------
        # \\  Expand right
        # \\------------------------------------------------------
        while ((bFound == False) & (j + WINDOW_WIDTH < len(listLowPassData))):
            fSlope = (listLowPassData[j] - listLowPassData[j + WINDOW_WIDTH]) / WINDOW_WIDTH
    
            # \\------------------------------------------------------
            # \\  Convert to degrees
            # \\------------------------------------------------------
            fSlope = np.arctan(fSlope) * fRadToDeg
            if (fSlope < MAX_SLOPE):
                iRight = j
                bFound = True
                continue
            j+=1
            
        iPBW = iRight - iLeft
    
        # \\------------------------------------------------------
        # \\  Add peak to list
        # \\------------------------------------------------------
        AddPeak(iLeft, iRight, DataList, listLowPassData, dtStartTime);
    
        # \\------------------------------------------------------
        # \\  Continue on
        # \\------------------------------------------------------
        WINDOW_WIDTH = iPBW / iDIV
    
        i = iRight + 1
        iLeft = 0
        iRight = 0
    
        # \\------------------------------------------------------
        # \\  Compute min peak width
        # \\------------------------------------------------------
        iMinPBW = (int)(iPBW * 0.9)
        iMaxPBW = (int)(iPBW * 3.0)
    
        while (i < len(listLowPassData) - WINDOW_WIDTH - 1):
            fSlope = (listLowPassData[i + WINDOW_WIDTH] - listLowPassData[i]) / WINDOW_WIDTH
    
            # \\------------------------------------------------------
            # \\  Convert to degrees
            # \\------------------------------------------------------
            fSlope = np.arctan(fSlope) * fRadToDeg
    
            # \\--------------------------------------------------------------
            # \\  Run algoritm based on sample rate
            # \\
            # \\  NOTE: This should theoretically be ok for all speeds, but 
            # \\  I made a distinction until I can do additional testing.
            # \\--------------------------------------------------------------
            if (SampleRate < 5000):
                if ((iLeft == 0) & (fSlope > MAX_SLOPE & listLowPassData[i] > lowpass_peak_thresh)):
                    iLeft = i
                    iRight = 0
                elif ((iLeft != 0) & ((fSlope < MAX_SLOPE & fSlope > -MAX_SLOPE))):
                    iRight = i
                    iPBW = iRight - iLeft
    
                    # \\------------------------------------------------------
                    # \\  Now see if this is a peak
                    # \\------------------------------------------------------
                    if ((iPBW > iMinPBW) & (iPBW < iMaxPBW)):
                        AddPeak(iLeft, iRight, DataList, listLowPassData, dtStartTime)
                        
                        # \\------------------------------------------------------
                        # \\  Compute min peak width
                        # \\------------------------------------------------------
                        iMinPBW = (int)(iPBW * 0.9)
                        iMaxPBW = (int)(iPBW * 3.0)
    
                        iLeft = 0
                        iRight = 0
                        WINDOW_WIDTH = iPBW / iDIV
    
            else:
                if ((iLeft == 0) & (fSlope > MAX_SLOPE)):
                    iLeft = i
                    iRight = 0
                elif ((iLeft != 0) & ((fSlope < 0 & fSlope > -MAX_SLOPE))):
                    iRight = i
                    iPBW = iRight - iLeft
    
                    # \\------------------------------------------------------
                    # \\  Now see if this is a peak
                    # \\------------------------------------------------------
                    if ((iPBW > iMinPBW) & (iPBW < iMaxPBW)):
                        AddPeak(iLeft, iRight, DataList, listLowPassData, dtStartTime)
    
                        # \\------------------------------------------------------
                        # \\  Compute min peak width
                        # \\------------------------------------------------------
                        iMinPBW = (int)(iPBW * 0.9)
                        iMaxPBW = (int)(iPBW * 3.0)
    
                        iLeft = 0
                        iRight = 0
                        WINDOW_WIDTH = iPBW / iDIV
            i+=1
            
def AddPeak(iLeft, iRight,  DataList, listLowPassData, dtStartTime, SampleRate):
    ListPeaks = []
    i = 0
    p = Peak();
    fMax = -99999999
    fMin = 999999999
    iMaxPos = 0
    iMinPos = 0

    # \\--------------------------------------------------------------
    # \\  Range check params
    # \\--------------------------------------------------------------
    iLeft = np.maximum(iLeft, 0)
    iRight = np.minimum(iRight, DataList.Count - 1)

    # \\------------------------------------------------------
    # \\  Fill in range
    # \\------------------------------------------------------
    p.set_StartPosition(iLeft)
    p.set_EndPosition(iRight)

    # \\------------------------------------------------------
    # \\  Find max dynamic value
    # \\------------------------------------------------------
    for i in range(iLeft,iRight):
        if ((DataList[i]) > fMax):
            fMax = DataList[i]
            iMaxPos = i

    p.set_Position(iMaxPos)

    fMax = -999999
    iMaxPos = 0

    # \\------------------------------------------------------
    # \\  Find max low pass value
    # \\------------------------------------------------------
    for i in range(iLeft,iRight):
        if (listLowPassData[i] > fMax):
            fMax = listLowPassData[i]
            iMaxPos = i

    p.set_LowPassPosition(iMaxPos)

    # \\------------------------------------------------------
    # \\  Now find base - left
    # \\------------------------------------------------------
    for i in range(p.get_Position(), np.maximum((p.get_Position - p.get_PeakWidth() * 2), 0),-1):
        if (listLowPassData[i] < fMin):
            fMin = listLowPassData[i]
            iMinPos = i
    p.set_BaseLeft(fMin)

    fMin = 9999999
    iMinPos = 0

    # \\------------------------------------------------------
    # \\  Now find base - right
    # \\------------------------------------------------------
    for i in range(p.get_Position(), (p.get_Position() + p.get_PeakWidth() * 2),1):
        if ( (i < listLowPassData.Count) & (listLowPassData[i] < fMin)):
            fMin = listLowPassData[i]
            iMinPos = i

    p.set_BaseRight(fMin)

    # \\------------------------------------------------------
    # \\  Update Kip values
    # \\------------------------------------------------------
    if ((p.get_Position() >= 0) & (p.get_Position() < len(DataList))):
        p.set_Kip(DataList[p.get_Position()] - p.get_BaseKip())

        p.set_LowPassKip(listLowPassData[p.get_LowPassPosition()] - p.get_BaseKip())

        dNumSamplesInMS = SampleRate / 1000.0
        iMS = (int)(p.get_Position() / dNumSamplesInMS)

        p.set_PeakTime(dtStartTime + datetime.timedelta(0, 0, 0, 0, iMS))

        # \\------------------------------------------------------
        # \\  Add peak to list
        # \\------------------------------------------------------
        ListPeaks.append(p)
