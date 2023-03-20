# -*- coding: utf-8 -*-
"""
Created on Sun Aug  8 12:38:05 2021

@author: Dana
"""
from setuptools import setup
from Cython.Build import cythonize 
import py2exe

setup(install_requires=[
        'time',
        'os',
        'winshel',
        'numpy',
        'multiprocessing',
        'itertools',
        'functools',
        'operator',
        'pandas',
        'shutil',
        'math',
        're',
        'datetime',
        'smtplib',
        'ssl',
        'scipy',
        'email',
        'pyunpack',
        'python-dev-tools',
        'Cython',
        'easyprocess',
        'patool',
        'py7zr'
    ]
)



#from setuptools import setup 
#from Cython.Build import cythonize 

#setup( 
#    ext_modules = cythonize("TPDSiteHealthAnalyzer_UserMain.py") 
#)