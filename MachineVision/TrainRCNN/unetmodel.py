# -*- coding: utf-8 -*-
"""
Created on Tue Jan  3 14:37:38 2023

@author: Dana
"""

from tensorflow import keras
from tensorflow.keras import layers


class UNETModel:
    def __init__(self, input_shape, output_shape):
        """

        :param input_shape: shape of input images, default is 28 * 28 *1
        :param output_shape: number of classes. for mnist_ds, default is 10
        
        """
        self.input_shape = input_shape
        self.output_shape = output_shape
        self.kernel_size = 3
    
    def Conv2D_block(self, input_tensor, n_filters, batchnorm=True):
        "Function to add 2 convolutional layers"
        ## Convolution layer - 1
        x = layers.Conv2D(filters=n_filters, kernel_size=(self.kernel_size, self.kernel_size),\
                          kernel_initializer='he_normal', padding='same')(input_tensor)
        ##batchnormalization
        if batchnorm:
            x = layers.BatchNormalization()(x)
        ## Activation
        x = layers.Activation('relu')(x)
        
        ## Convolution layer - 2
        x = layers.Conv2D(filters=n_filters, kernel_size=(self.kernel_size, self.kernel_size), \
                         kernel_initializer='he_normal', padding='same')(x)
        ##batchnormalization
        if batchnorm:
            x = layers.BatchNormalization()(x)
        ## Activation
        x = layers.Activation('relu')(x)
        
        return x
    
    def unetfromscratch(self, n_filters=16, dropout=0.1, batchnorm=True):
        
        initializer = keras.initializers.VarianceScaling(scale=0.7, mode='fan_in', distribution='truncated_normal', seed=150)
        
        input          = layers.Input(shape=(self.input_shape[0], self.input_shape[1], self.input_shape[2]), name = 'input')
        
        ##Encoder Path \\ Contractor Path
        c1 = self.Conv2D_block(input, n_filters, batchnorm=batchnorm)
        p1 = layers.MaxPooling2D((2,2))(c1)
        p1 = layers.Dropout(dropout)(p1)
        
        c2 = self.Conv2D_block(p1, n_filters*2, batchnorm=batchnorm)
        p2 = layers.MaxPooling2D((2,2))(c2)
        p2 = layers.Dropout(dropout)(p2)
        
        c3 = self.Conv2D_block(p2, n_filters*4, batchnorm=batchnorm)
        p3 = layers.MaxPooling2D((2,2))(c3)
        p3 = layers.Dropout(dropout)(p3)
        
        c4 = self.Conv2D_block(p3, n_filters*8, batchnorm=batchnorm)
        p4 = layers.MaxPooling2D((2,2))(c3)
        p4 = layers.Dropout(dropout)(p4)
        
        c5 = self.Conv2D_block(p4, n_filters*16, batchnorm=batchnorm)
        
        ## Decoder Path \\ Expansion Path
        u6 = layers.Conv2DTranspose(n_filters*8, (self.kernel_size, self.kernel_size), strides=(1,1), padding='same')(c5)
        u6 = layers.concatenate([u6, c4])
        u6 = layers.Dropout(dropout)(u6)
        c6 = self.Conv2D_block(u6, n_filters*8, batchnorm=batchnorm)
        
        u7 = layers.Conv2DTranspose(n_filters*4, (self.kernel_size, self.kernel_size), strides=(2,2), padding='same')(c6)
        u7 = layers.concatenate([u7, c3])
        u7 = layers.Dropout(dropout)(u7)
        c7 = self.Conv2D_block(u7, n_filters*4, batchnorm=batchnorm)
        
        u8 = layers.Conv2DTranspose(n_filters*2, (self.kernel_size, self.kernel_size), strides=(2,2), padding='same')(c7)
        u8 = layers.concatenate([u8, c2])
        u8 = layers.Dropout(dropout)(u8)
        c8 = self.Conv2D_block(u8, n_filters*2, batchnorm=batchnorm)
        
        u9 = layers.Conv2DTranspose(n_filters, (self.kernel_size, self.kernel_size), strides=(2,2), padding='same')(c8)
        u9 = layers.concatenate([u9, c1])
        u9 = layers.Dropout(dropout)(u9)
        c9 = self.Conv2D_block(u9, n_filters, batchnorm=batchnorm)
        
        ## final 1*1 Convolution layer
        output = layers.Conv2D(1, (1,1), activation='relu')(c9)
        model = keras.models.Model(inputs=[input], outputs=[output])
        
        return model