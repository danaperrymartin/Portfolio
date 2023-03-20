# -*- coding: utf-8 -*-
"""
Created on Tue Apr 19 08:34:25 2022

@author: Dana
"""
import tensorflow as tf
import numpy as np
import torch

from tensorflow import keras
from tensorflow.keras import layers


class Pix2PixModel:
    def __init__(self, input_shape, output_shape):
        """

        :param input_shape: shape of input images, default is 28 * 28 *1
        :param output_shape: number of classes. for mnist_ds, default is 10

        """
        self.input_shape = input_shape
        self.output_shape = output_shape
    
    def pix2pixfromscratch(self):
        
        initializer = keras.initializers.VarianceScaling(scale=0.7, mode='fan_in', distribution='truncated_normal', seed=150)
        # initializer = keras.initializers.GlorotNormal(seed=None)
        # initializer = keras.initializers.Constant(value=2)
        
        # Block 1
        input          = layers.Input(shape=(self.input_shape[0], self.input_shape[1], self.input_shape[2]), name = 'input')
        
        # b1_conv2dTrans_1 = layers.Conv2DTranspose(filters=64, kernel_size=(3,3), strides=(1,1), padding='same',
        #                   use_bias=True, dilation_rate=(1,1), name='b1_conv2dTrans_1', kernel_initializer=initializer)(input)
        
        b1_conv2d_1    = layers.Conv2D(filters=32, kernel_size=(3, 3), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=1, groups=1, name='b1_cnv2d_1', kernel_initializer=initializer)(input)
        
        # b1_clip_1      = layers.Lambda(lambda x: tf.keras.backend.clip(x, min_value=0, max_value=6),name='b1_clip_1')(b1_conv2d_1)
       
        b1_conv2d_2    = layers.Conv2D(filters=32, kernel_size=(3, 3), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=1, groups=1, name='b1_cnv2d_2', kernel_initializer=initializer)(b1_conv2d_1)
        
        b1_clip_2      = layers.Lambda(lambda x: tf.keras.backend.clip(x, min_value=0, max_value=6),name='b1_clip_2')(b1_conv2d_2)
        
        b1_conv2d_3    = layers.Conv2D(filters=16, kernel_size=(1, 1), strides=(1, 1), padding='same',
                         use_bias=False, dilation_rate=1, groups=1, name='b1_cnv2d_3', kernel_initializer=initializer)(b1_clip_2)
        
        b1_conv2d_4    = layers.Conv2D(filters=1, kernel_size=(1, 1), strides=(1, 1), padding='same',
                         use_bias=False, dilation_rate=1, groups=1, name='b1_cnv2d_4', kernel_initializer=initializer)(b1_conv2d_3)
        
        b1_norm_1      = layers.BatchNormalization(epsilon=1e-3, momentum=0.999, center=True, scale=True, name='b1_norm_1')(b1_conv2d_4)
        
        b1_clip_3      = layers.Lambda(lambda x: tf.keras.backend.clip(x, min_value=0, max_value=6), name='b1_clip_3')(b1_norm_1)
        
        # b1_upsample_1 = layers.UpSampling2D(size=(2, 1), data_format=None, interpolation="nearest")(b1_clip_3)
        
        # Block 2
        b2_pad_1    = layers.ZeroPadding2D(padding=(0, 0), name='b2_pad_1', data_format=None)(b1_clip_3)
        
        b2_conv2d_1 = layers.Conv2D(filters=96, kernel_size=(3, 3), strides=(2, 2), padding='same',
                         use_bias=False, dilation_rate=(1,1), groups=1, name='b2_cnv2d_1', kernel_initializer=initializer)(b2_pad_1)
        
        b2_clip_1   = layers.Lambda(lambda x: tf.keras.backend.clip(x, min_value=0, max_value=6),name='b2_clip_1')(b2_conv2d_1)
        
        b2_conv2d_2 = layers.Conv2D(filters=24, kernel_size=(1, 1), strides=(1, 1), padding='same',
                         use_bias=False, dilation_rate=(1,1), groups=1, name='b2_cnv2d_2', kernel_initializer=initializer)(b2_clip_1)
        # Block 3
        b3_conv2d_1 = layers.Conv2D(filters=1, kernel_size=(1, 1), strides=(1, 1), padding='same',
                         use_bias=False, dilation_rate=1, groups=1, name='b3_cnv2d_1', kernel_initializer=initializer)(b2_conv2d_2)
        
        b3_norm_1   = layers.BatchNormalization(epsilon=1e-3, momentum=0.999, center=True, scale=True, name='b3_norm_1')(b3_conv2d_1)
        
        b3_clip_1   = layers.Lambda(lambda x: tf.keras.backend.clip(x, min_value=0, max_value=6),name='b3_clip_1')(b3_norm_1)
        
        b3_conv2d_2 = layers.Conv2D(filters=144, kernel_size=(3,3), strides=(1, 1), padding='same',
                         use_bias=False, dilation_rate=1, groups=1, name='b3_cnv2d_2', kernel_initializer=initializer)(b3_clip_1)
        
        b3_clip_2   = layers.Lambda(lambda x: tf.keras.backend.clip(x, min_value=0, max_value=6),name='b3_clip_2')(b3_conv2d_2)
        
        b3_conv2d_3 = layers.Conv2D(filters=24, kernel_size=(1,1), strides=(1, 1), padding='same',
                         use_bias=False, dilation_rate=1, groups=1, name='b3_cnv2d_3', kernel_initializer=initializer)(b3_clip_2)
        
        # Add blocks 2 & 3
        b23_add   =  layers.add([b2_conv2d_2, b3_conv2d_3])
        b23_conv2d_1 = layers.Conv2D(filters=144, kernel_size=(1,1), strides=(1, 1), padding='same',
                         use_bias=False, dilation_rate=1, groups=1, name='b23_conv2d_1', kernel_initializer=initializer)(b23_add)
        b23_clip_1   = layers.Lambda(lambda x: tf.keras.backend.clip(x, min_value=0, max_value=6),name='b23_clip_1')(b23_conv2d_1)
        
        # Block 4
        b4_pad_1    = layers.ZeroPadding2D(padding=(0, 0), name='b4_pad_1', data_format=None)(b23_clip_1)
        
        b4_conv2d_1 = layers.Conv2D(filters=144, kernel_size=(3,3), strides=(2, 2), padding='same',
                         use_bias=False, dilation_rate=(1,1), groups=1, name='b4_conv2d_1', kernel_initializer=initializer)(b4_pad_1)
        
        b4_clip_1   = layers.Lambda(lambda x: tf.keras.backend.clip(x, min_value=0, max_value=6),name='b4_clip_1')(b4_conv2d_1)
        
        b4_conv2d_2 = layers.Conv2D(filters=32, kernel_size=(1,1), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=(1,1), groups=1, name='b4_conv2d_2', kernel_initializer=initializer)(b4_clip_1)
        # Block 5
        b5_conv2d_1 = layers.Conv2D(filters=1, kernel_size=(1,1), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=(1,1), groups=1, name='b5_conv2d_1', kernel_initializer=initializer)(b4_conv2d_2)
    
        b5_norm_1   = layers.BatchNormalization(epsilon=1e-3, momentum=0.999, center=True, scale=True, name='b5_norm_1')(b5_conv2d_1)
        
        b5_clip_1   = layers.Lambda(lambda x: tf.keras.backend.clip(x, min_value=0, max_value=6),name='b5_clip_1')(b5_norm_1)
        
        b5_conv2d_2 = layers.Conv2D(filters=192, kernel_size=(3,3), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=(1,1), groups=1, name='b5_conv2d_2', kernel_initializer=initializer)(b5_clip_1)
        
        b5_clip_2   = layers.Lambda(lambda x: tf.keras.backend.clip(x, min_value=0, max_value=6),name='b5_clip_2')(b5_conv2d_2)
        
        b5_conv2d_3 = layers.Conv2D(filters=32, kernel_size=(1,1), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=(1,1), groups=1, name='b5_conv2d_3', kernel_initializer=initializer)(b5_clip_2)
        
        # Add blocks 4 & 5
        b45_add   =  layers.add([b4_conv2d_2, b5_conv2d_3])
        
        # Block 6
        b6_conv2d_1 = layers.Conv2D(filters=192, kernel_size=(1,1), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=(1,1), groups=1, name='b6_conv2d_1', kernel_initializer=initializer)(b45_add)
        
        b6_clip_1   = layers.Lambda(lambda x: tf.keras.backend.clip(x, min_value=0, max_value=6),name='b6_clip_1')(b6_conv2d_1)
        
        b6_conv2d_2 = layers.Conv2D(filters=192, kernel_size=(3,3), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=(1,1), groups=1, name='b6_conv2d_2', kernel_initializer=initializer)(b6_clip_1)
        
        b6_clip_2   = layers.Lambda(lambda x: tf.keras.backend.clip(x, min_value=0, max_value=6),name='b6_clip_2')(b6_conv2d_2)
        
        b6_conv2d_3 = layers.Conv2D(filters=32, kernel_size=(1,1), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=(1,1), groups=1, name='b6_conv2d_3', kernel_initializer=initializer)(b6_clip_2)
        # Add blocks 5 & 6
        b56_add      =  layers.add([b45_add, b6_conv2d_3])
        b56_conv2d_1 = layers.Conv2D(filters=192, kernel_size=(1,1), strides=(1, 1), padding='same',
                         use_bias=False, dilation_rate=1, groups=1, name='b56_conv2d_1', kernel_initializer=initializer)(b56_add)
        b56_clip_1   = layers.Lambda(lambda x: tf.keras.backend.clip(x, min_value=0, max_value=6),name='b56_clip_1')(b56_conv2d_1)
        
        # Block 7
        b7_pad_1    = layers.ZeroPadding2D(padding=(0, 0), name='b7_pad_1', data_format=None)(b56_clip_1)
        
        b7_conv2d_1 = layers.Conv2D(filters=192, kernel_size=(3,3), strides=(2,2), padding='same',
                         use_bias=False, dilation_rate=1, groups=1, name='b7_conv2d_1', kernel_initializer=initializer)(b7_pad_1)
        
        b7_clip_1   = layers.Lambda(lambda x: tf.keras.backend.clip(x, min_value=0, max_value=6),name='b7_clip_1')(b7_conv2d_1)
        
        b7_conv2d_2 = layers.Conv2D(filters=64, kernel_size=(1,1), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=1, groups=1, name='b7_conv2d_2', kernel_initializer=initializer)(b7_clip_1)
        
        # Block 8
        b8_conv2d_1 = layers.Conv2D(filters=1, kernel_size=(1,1), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=1, groups=1, name='b8_conv2d_1', kernel_initializer=initializer)(b7_conv2d_2)
        
        b8_norm_1   = layers.BatchNormalization(epsilon=1e-3, momentum=0.999, center=True, scale=True, name='b8_norm_1')(b8_conv2d_1)
        
        b8_clip_1   = layers.Lambda(lambda x: tf.keras.backend.clip(x, min_value=0, max_value=6),name='b8_clip_1')(b8_norm_1)
        
        b8_conv2d_2 = layers.Conv2D(filters=384, kernel_size=(3,3), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=1, groups=1, name='b8_conv2d_2', kernel_initializer=initializer)(b8_clip_1)
        
        b8_clip_2   = layers.Lambda(lambda x: tf.keras.backend.clip(x, min_value=0, max_value=6),name='b8_clip_2')(b8_conv2d_2)
        
        b8_conv2d_3 = layers.Conv2D(filters=64, kernel_size=(1,1), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=1, groups=1, name='b8_conv2d_3', kernel_initializer=initializer)(b8_clip_2)
        
        # Add blocks 7 & 8
        b78_add      =  layers.add([b7_conv2d_2, b8_conv2d_3])
        
        # Block 9
        b9_conv2d_1 = layers.Conv2D(filters=384, kernel_size=(1,1), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=1, groups=1, name='b9_conv2d_1', kernel_initializer=initializer)(b78_add)
        
        b9_clip_1   = layers.Lambda(lambda x: tf.keras.backend.clip(x, min_value=0, max_value=6),name='b9_clip_1')(b9_conv2d_1)
        
        b9_conv2d_2 = layers.Conv2D(filters=384, kernel_size=(3,3), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=1, groups=1, name='b9_conv2d_2', kernel_initializer=initializer)(b9_clip_1)
        
        b9_clip_2   = layers.Lambda(lambda x: tf.keras.backend.clip(x, min_value=0, max_value=6),name='b9_clip_2')(b9_conv2d_2)
        
        b9_conv2d_3 = layers.Conv2D(filters=64, kernel_size=(1,1), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=1, groups=1, name='b9_conv2d_3', kernel_initializer=initializer)(b9_clip_2)
        
        # Add blocks 8 & 9
        b89_add      =  layers.add([b78_add, b9_conv2d_3])
        
        # Block 10
        b10_conv2d_1 = layers.Conv2D(filters=384, kernel_size=(1,1), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=1, groups=1, name='b10_conv2d_1', kernel_initializer=initializer)(b89_add)
        
        b10_clip_1   = layers.Lambda(lambda x: tf.keras.backend.clip(x, min_value=0, max_value=6),name='b10_clip_1')(b10_conv2d_1)
        
        b10_conv2d_2 = layers.Conv2D(filters=384, kernel_size=(3,3), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=1, groups=1, name='b10_conv2d_2', kernel_initializer=initializer)(b10_clip_1)
        
        b10_clip_2   = layers.Lambda(lambda x: tf.keras.backend.clip(x, min_value=0, max_value=6),name='b10_clip_2')(b10_conv2d_2)
        
        b10_conv2d_3 = layers.Conv2D(filters=64, kernel_size=(1,1), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=1, groups=1, name='b10_conv2d_3', kernel_initializer=initializer)(b10_clip_2)
        
        # Add blocks 9 & 10
        b910_add      =  layers.add([b89_add, b10_conv2d_3])
        
        # Block 11
        b11_conv2d_1 = layers.Conv2D(filters=384, kernel_size=(1,1), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=1, groups=1, name='b11_conv2d_1', kernel_initializer=initializer)(b910_add)
        
        b11_clip_1   = layers.Lambda(lambda x: tf.keras.backend.clip(x, min_value=0, max_value=6),name='b11_clip_1')(b11_conv2d_1)
        
        b11_conv2d_2 = layers.Conv2D(filters=384, kernel_size=(3,3), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=1, groups=1, name='b11_conv2d_2', kernel_initializer=initializer)(b11_clip_1)
        
        b11_clip_2   = layers.Lambda(lambda x: tf.keras.backend.clip(x, min_value=0, max_value=6),name='b11_clip_2')(b11_conv2d_2)
        
        b11_conv2d_3 = layers.Conv2D(filters=96, kernel_size=(1,1), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=(1,1), groups=1, name='b11_conv2d_3', kernel_initializer=initializer)(b11_clip_2)
        
        # Block 12
        b12_conv2d_1 = layers.Conv2D(filters=1, kernel_size=(1,1), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=(1,1), groups=1, name='b12_conv2d_1', kernel_initializer=initializer)(b11_conv2d_3)
        
        b12_norm_1   = layers.BatchNormalization(epsilon=1e-3, momentum=0.999, center=True, scale=True, name='b12_norm_1')(b12_conv2d_1)
        
        b12_clip_1   = layers.Lambda(lambda x: tf.keras.backend.clip(x, min_value=0, max_value=6),name='b12_clip_1')(b12_norm_1)
        
        b12_conv2d_2 = layers.Conv2D(filters=576, kernel_size=(3,3), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=(1,1), groups=1, name='b12_conv2d_2', kernel_initializer=initializer)(b12_clip_1)
        
        b12_clip_2   = layers.Lambda(lambda x: tf.keras.backend.clip(x, min_value=0, max_value=6),name='b12_clip_2')(b12_conv2d_2)
        
        b12_conv2d_3 = layers.Conv2D(filters=96, kernel_size=(1,1), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=(1,1), groups=1, name='b12_conv2d_3', kernel_initializer=initializer)(b12_clip_2)
        
        # Add blocks 11 & 12
        b1112_add      =  layers.add([b11_conv2d_3, b12_conv2d_3])
        
        # Block 13
        b13_conv2d_1 = layers.Conv2D(filters=576, kernel_size=(1,1), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=(1,1), groups=1, name='b13_conv2d_1', kernel_initializer=initializer)(b1112_add)
        
        b13_clip_1   = layers.Lambda(lambda x: tf.keras.backend.clip(x, min_value=0, max_value=6),name='b13_clip_1')(b13_conv2d_1)
        
        b13_conv2d_2 = layers.Conv2D(filters=576, kernel_size=(3,3), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=(1,1), groups=1, name='b13_conv2d_2', kernel_initializer=initializer)(b13_clip_1)
        
        b13_clip_2   = layers.Lambda(lambda x: tf.keras.backend.clip(x, min_value=0, max_value=6),name='b13_clip_2')(b13_conv2d_2)
        
        b13_conv2d_3 = layers.Conv2D(filters=96, kernel_size=(1,1), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=(1,1), groups=1, name='b13_conv2d_3', kernel_initializer=initializer)(b13_clip_2)
        
        # Add blocks 12 & 13
        b1213_add      =  layers.add([b1112_add, b13_conv2d_3])
        
        b1213_conv2d_1 = layers.Conv2D(filters=1, kernel_size=(1,1), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=(1,1), groups=1, name='b1213_conv2d_1', kernel_initializer=initializer)(b1213_add)
        
        b1213_clip_1   = layers.Lambda(lambda x: tf.keras.backend.clip(x, min_value=0, max_value=6),name='b1213_clip_1')(b1213_conv2d_1)
        
        # Block 14
        b14_pad_1    = layers.ZeroPadding2D(padding=(0, 0), name='b14_pad_1', data_format=None)(b1213_clip_1)
        
        b14_conv2d_1 = layers.Conv2D(filters=576, kernel_size=(3,3), strides=(2,2), padding='same',
                         use_bias=False, dilation_rate=(1,1), groups=1, name='b14_conv2d_1', kernel_initializer=initializer)(b14_pad_1)
        
        b14_clip_1   = layers.Lambda(lambda x: tf.keras.backend.clip(x, min_value=0, max_value=6),name='b14_clip_1')(b14_conv2d_1)
        
        b14_conv2d_2 = layers.Conv2D(filters=160, kernel_size=(1,1), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=(1,1), groups=1, name='b14_conv2d_2', kernel_initializer=initializer)(b14_clip_1)
        
        # Block 15
        b15_conv2d_1 = layers.Conv2D(filters=1, kernel_size=(1,1), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=(1,1), groups=1, name='b15_conv2d_1', kernel_initializer=initializer)(b14_conv2d_2)
        
        b15_norm_1   = layers.BatchNormalization(epsilon=1e-3, momentum=0.999, center=True, scale=True, name='b15_norm_1')(b15_conv2d_1)
        
        b15_clip_1   = layers.Lambda(lambda x: tf.keras.backend.clip(x, min_value=0, max_value=6),name='b15_clip_1')(b15_norm_1)
        
        b15_conv2d_2 = layers.Conv2D(filters=96, kernel_size=(3,3), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=(1,1), groups=1, name='b15_conv2d_2', kernel_initializer=initializer)(b15_clip_1)
        
        b15_clip_2   = layers.Lambda(lambda x: tf.keras.backend.clip(x, min_value=0, max_value=6),name='b15_clip_2')(b15_conv2d_2)
        
        b15_conv2d_3 = layers.Conv2D(filters=160, kernel_size=(1,1), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=(1,1), groups=1, name='b15_conv2d_3', kernel_initializer=initializer)(b15_clip_2)
        
        # Add blocks 14 & 15
        b1415_add      =  layers.add([b14_conv2d_2, b15_conv2d_3])
        
        # Block 16
        b16_conv2d_1 = layers.Conv2D(filters=96, kernel_size=(1,1), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=(1,1), groups=1, name='b16_conv2d_1', kernel_initializer=initializer)(b1415_add)
        
        b16_clip_1   = layers.Lambda(lambda x: tf.keras.backend.clip(x, min_value=0, max_value=6),name='b16_clip_1')(b16_conv2d_1)
        
        b16_conv2d_2 = layers.Conv2D(filters=96, kernel_size=(3,3), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=(1,1), groups=1, name='b16_conv2d_2', kernel_initializer=initializer)(b16_clip_1)
        
        b16_clip_2   = layers.Lambda(lambda x: tf.keras.backend.clip(x, min_value=0, max_value=6),name='b16_clip_2')(b16_conv2d_2)
        
        b16_conv2d_3 = layers.Conv2D(filters=160, kernel_size=(1,1), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=(1,1), groups=1, name='b16_conv2d_3', kernel_initializer=initializer)(b16_clip_2)
        
        # Add blocks 15 & 16
        b1516_add      =  layers.add([b1415_add,  b16_conv2d_3])
        
        # Block 17
        b17_conv2d_1 = layers.Conv2D(filters=96, kernel_size=(1,1), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=(1,1), groups=1, name='b17_conv2d_1', kernel_initializer=initializer)(b1516_add)
        
        b17_clip_1   = layers.Lambda(lambda x: tf.keras.backend.clip(x, min_value=0, max_value=6),name='b17_clip_1')(b17_conv2d_1)
        
        b17_conv2d_2 = layers.Conv2D(filters=96, kernel_size=(3,3), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=(1,1), groups=1, name='b17_conv2d_2', kernel_initializer=initializer)(b17_clip_1)
        
        b17_clip_2   = layers.Lambda(lambda x: tf.keras.backend.clip(x, min_value=0, max_value=6),name='b17_clip_2')(b17_conv2d_2)
        
        b17_conv2d_3 = layers.Conv2D(filters=1, kernel_size=(1,1), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=(1,1), groups=1, name='b17_conv2d_3', kernel_initializer=initializer)(b17_clip_2)
        
        b17_conv2dTrans_1 = layers.Conv2DTranspose(filters=1, kernel_size=(3,3), strides=(2,2), padding='same',
                         use_bias=False, dilation_rate=(1,1), name='b17_conv2dTrans_1', kernel_initializer=initializer)(b17_conv2d_3)
        
        b17_norm_1   = layers.BatchNormalization(epsilon=1e-3, momentum=0.999, center=True, scale=True, name='b17_norm_1')(b17_conv2dTrans_1)
        
        b17_relu_1 = layers.ReLU(name='b17_relu_1')(b17_norm_1)
        
        # Block 18
        b18_concat_1 = layers.Concatenate(axis=0)([b1213_clip_1, b17_relu_1])
        
        b18_conv2dTrans_1 = layers.Conv2DTranspose(filters=1, kernel_size=(1,1), strides=(8,8), padding='same',
                         use_bias=False, dilation_rate=(1,1), name='b18_conv2dTrans_1', kernel_initializer=initializer)(b18_concat_1)
        
        b18_norm_1   = layers.BatchNormalization(epsilon=1e-3, momentum=0.999, center=True, scale=True, name='b18_norm_1')(b18_conv2dTrans_1)
        
        b18_relu_1 = layers.ReLU(name='b18_relu_1')(b18_norm_1)
        
        # Block 19
        # b19_upsample_1 = layers.UpSampling2D(size=(4, 4), data_format=None, interpolation="nearest")(b18_relu_1)
        
        b19_concat_1 = layers.Concatenate(axis=0)([b1_clip_3, b18_relu_1])
        
        b19_conv2dTrans_1 = layers.Conv2DTranspose(filters=1, kernel_size=(3,3), strides=(1,1), padding='same',
                         use_bias=False, dilation_rate=(1,1), name='b19_conv2dTrans_1', kernel_initializer=initializer)(b19_concat_1)
        
        # b19_upsample_1 = layers.UpSampling2D(size=(2,1), data_format=None, interpolation="nearest")(b19_conv2dTrans_1)
        
        # b19_transpose_1 = layers.Lambda(lambda x: tf.reshape(x,[1, 128, 128,1]), name='b19_transpose_1')(b19_conv2dTrans_1)
        
        # b19_permute_1 = layers.Permute((2,1), input_shape=(128,128))
        
        b19_pooling_1 = layers.MaxPooling2D(pool_size=(2, 2), strides=None, padding="valid", data_format='channels_last', name='b19_maxpooling_1')(b19_conv2dTrans_1)
        
        b19_upsample_1 = layers.UpSampling2D(size=(2,2), data_format=None, interpolation="nearest")(b19_pooling_1)
        
        b19_concat_1 = layers.Concatenate(axis=0)([b19_conv2dTrans_1, b19_upsample_1])
                
        output = layers.Dense(self.output_shape, name='model_output', activation='relu')(b19_concat_1)
        
        model = keras.models.Model(input, output)
        
        
        return model