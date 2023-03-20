# -*- coding: utf-8 -*-
"""
Created on Thu Jul 29 11:33:28 2021

@author: Dana
"""

import numpy as np
import datetime

class Peak(object):
    def __init__(self):
        self.m_iStartPosition = 0
        self.m_iEndPosition = 0
        self.m_iPosition = 0
        self.m_fKip = 0
        self.m_iSignalLevelCount = 0
        self.m_fLowPassKip = 0
        self.m_iLowPassPosition = 0
        self.m_fStaticKip = 0
        self.m_iStaticStart = 0
        self.m_iStaticEnd = 0
        self.m_fBaseLeft = 0
        self.m_fBaseRight = 0
        self.m_dtPeakTime = datetime.MINYEAR
    
    # setter method
    def set_StartPosition(self, x):
        self._m_iStartPosition = x
    # getter method
    def get_StartPosition(self):
        return self._m_iStartPosition
    
    # setter method
    def set_EndPosition(self, x):
        self._m_iEndPosition = x
    # getter method
    def get_EndPosition(self):
        return self._m_iEndPosition
    
    def set_StaticStart(self, x):
        self._m_iStaticStart = x
    def get_StaticStart(self):
        return self._m_iStaticStart
    
    def set_StaticEnd(self, x):
        self._m_iStaticEnd = x
    def get_StaticEnd(self):
        return self._m_iStaticEnd
    
    def set_Position(self, x):
        self._m_iPosition = x
    def get_Position(self):
        return self._m_iPosition
    
    def set_Kip(self, x):
        self._m_fKip = x
    def get_Kip(self):
        return self._m_fKip   
    
    def get_BaseKip(self):
        tmp = (self.m_fBaseLeft + self.m_fBaseRight) / 2
        return tmp
    
    def get_BaseLeft(self):
        return self._m_fBaseLeft
    def set_BaseLeft(self):
        return self._m_fBaseLeft
    
    def get_BaseRight(self):
        return self._m_fBaseRight
    def set_BaseRight(self):
        return self._m_fBaseRight
    
    def get_LowPassKip(self):
        return self._m_fLowPassKip
    def set_LowPassKip(self):
        return self._m_fLowPassKip
    
    def get_LowPassPosition(self):
        return self._m_iLowPassPosition
    def set_LowPassPosition(self):
        return self._m_iLowPassPosition
    
    def get_SignalLevelCount(self):
        return self._m_iSignalLevelCount
    def set_SignalLevelCount(self):
        return self._m_iSignalLevelCount
    
    def get_PeakWidth(self):
        tmp = self.m_iEndPosition-self.m_iStartPosition
        return tmp
    
    def get_StaticKip(self):
        return self._m_fStaticKip
    def set_StaticKip(self):
        return self._m_fStaticKip
    
    def get_PeakTime(self):
        return self._m_dtPeakTime
    def set_PeakTime(self):
        return self._m_dtPeakTime