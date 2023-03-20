# -*- coding: utf-8 -*-
"""
Created on Tue Nov 15 08:50:20 2022

@author: Dana
"""

import numpy as np
import sqlalchemy
from sqlalchemy.engine import URL
from sqlalchemy import create_engine
from sqlalchemy import text

import pandas as pd
import matplotlib.pyplot as plt

from wordcloud import WordCloud
from wordcloud import ImageColorGenerator
from wordcloud import STOPWORDS

def create_server_connection():
    connection = None
    try:
        server = 'SERVER'
        database = 'train_db'
        username = 'prt'
        password = 'smrmprt'
        
        connection_string = 'DRIVER={ODBC Driver 18 for SQL Server};SERVER='+server+';DATABASE='+database+';ENCRYPT=no;UID='+username+';PWD='+ password
        connection_url = URL.create("mssql+pyodbc", query={"odbc_connect": connection_string})
        
        engine = create_engine(connection_url)
        print("MySQL Database connection successful")
    except sqlalchemy.error as err:
        print(f"Error: '{err}'")

    return engine

def read_query(connection, query):
    result = None
    try:
        # data = pd.read_sql(query,connection)  # without parameters [non-prepared statement]
        connection = connection.connect()
        data = pd.read_sql(text(query), con=connection)
        connection.close()
        return data
    except sqlalchemy.exc.DatabaseError as err:
        print(f"Error: '{err}'")

if __name__ == '__main__':
    
    #--- Create SQL connection
    connection = create_server_connection()
    
    q_whl = """
    SELECT *
    FROM wheel where TrainIndex>693663 AND HuntingIndex>0
    """
    
    q_car = """
    SELECT *
    FROM car where DateTime>'2022-11-05' AND HNT>0
    """
    
    #--- Create dataframes from SQL queries
    data_whl = read_query(connection, q_whl)
    data_car = read_query(connection, q_car)
    
    #---Close SQL connection
    connection.dispose()
    
    #--- Look at hunting index values for wheels
    # hist_whl = data_whl[data_whl['HuntingIndex']>0].hist(column='HuntingIndex', bins=100)
    hist_car = data_car[data_car['HNT']>0].hist(column='HNT', bins=100)
    #---- Determine Outliers and associated Cars -----------
    box_car  = data_car[data_car['HNT']>0].boxplot(column='HNT')
    Q1 = data_car['HNT'].quantile(0.25)
    Q3 = data_car['HNT'].quantile(0.75)
    IQR = Q3-Q1
    lower_lim = Q1-1.5*IQR
    upper_lim = Q3+1.5*IQR
    outliers_15_low = (data_car['HNT']<lower_lim)
    outliers_15_up = (data_car['HNT']>upper_lim)
    df_outliers = data_car[outliers_15_up]
    df_outliers.to_csv('HuntingOutliers.csv')
    
    #--- Visualize owners of cars with most hunting
    stopwords = set(STOPWORDS)
    wordcloud = WordCloud(stopwords=stopwords, background_color="white").generate(' '.join(data_car['Owner']))
    plt.figure( figsize=(15,10))
    plt.imshow(wordcloud, interpolation='bilinear')
    plt.axis("off")
    plt.show()
    
    wordcloud2 = WordCloud(stopwords=stopwords, background_color="white").generate(' '.join(data_car['CarType']))
    plt.figure( figsize=(15,10))
    plt.imshow(wordcloud2, interpolation='bilinear')
    plt.axis("off")
    plt.show()
    
    #---- Fitler tanker cars out
    df_tankercar = data_car[(data_car['Owner']=='TILX') | (data_car['Owner']=='GATX')]
    
    #--- Filter BNSF cars 
    df_bnsfcar = data_car[(data_car['Owner']=='BNSF') | (data_car['Owner']=='BN')]
    
    #--- Filter IRSX cars
    df_irsxcar = data_car[(data_car['Owner']=='IRSX')]
    
    #--- Look at hunting metrics for tanker cars (TILX and GATX)
    hist_tankercar = df_tankercar.hist(column='HNT', bins=100)
    
    #--- Compute percentage of tanker cars relative to all cars
    percenttanker = (1-(np.abs(len(data_car)-len(df_tankercar))/len(data_car)))*100
    
    #--- Compare percentage of tanker cars with hunting indexes >0 vs. non tanker cars >0
    percenttanker_HNT = (1-(np.abs((data_car['HNT'].sum()-df_tankercar['HNT'].sum()))/data_car['HNT'].sum()))*100
    
    #--- Compute percentage of bnsf cars relative to all cars
    percentbnsf = (1-(np.abs(len(data_car)-len(df_bnsfcar))/len(data_car)))*100
    
    #--- Compare percentage of tanker cars with hunting indexes >0 vs. all cars
    percentbnsf_HNT = (1-(np.abs((data_car['HNT'].sum()-df_bnsfcar['HNT'].sum()))/data_car['HNT'].sum()))*100
    
    #--- Compute percentage of irsx cars relative to all cars
    percentirsx = (1-(np.abs(len(data_car)-len(df_irsxcar))/len(data_car)))*100
    
    #--- Compare percentage of irsx cars with hunting indexes >0 vs. all cars >0
    percentirsx_HNT = (1-(np.abs((data_car['HNT'].sum()-df_irsxcar['HNT'].sum()))/data_car['HNT'].sum()))*100
    
    #--- Find car owner with most hunting
    df_car_hnt = pd.crosstab(pd.cut(data_car['HNT'], bins=len(data_car['Owner'].unique())), data_car['Owner'])
    
    df_hunter = pd.DataFrame({'HNT':(df_car_hnt.sum(axis=0))}).sort_values(by=['HNT'], ascending=False)
    
    #----- Compare correlation among hunting index and other indices
    data_car.plot.scatter(x='Ratio', y='HNT')
    data_car.plot.scatter(x='ASLV', y='HNT')
    data_car.plot.scatter(x='SWLV', y='HNT')
    data_car.plot.scatter(x='TSLV', y='HNT')
    data_car.plot.scatter(x='ELW', y='HNT')
    data_car.plot.scatter(x='F2A', y='HNT')
    data_car.plot.scatter(x='FCI', y='HNT')
    data_car.plot.scatter(x='GSI', y='HNT')
    data_car.plot.scatter(x='S2S', y='HNT')
    data_car.plot.scatter(x='SBI', y='HNT')
    data_car.plot.scatter(x='TWI', y='HNT')
    data_car.plot.scatter(x='VTI', y='HNT')
    data_car.plot.scatter(x='WLI', y='HNT')
    data_car.plot.scatter(x='AvgDynamicRatio', y='HNT')
    
    data_whl.plot.scatter(x='CarAxleNum', y='HuntingIndex')
    
    #---- See if any site shows excess hunting
    data_whl.plot.scatter(x='SiteIndex', y='HuntingIndex')
    
    
    