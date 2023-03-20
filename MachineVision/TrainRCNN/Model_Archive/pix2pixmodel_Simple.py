# -*- coding: utf-8 -*-
"""
Created on Tue Apr 19 08:34:25 2022

@author: Dana
"""
import tensorflow as tf
import numpy as np
import torch

# from tensorflow import keras
from keras import layers

import keras


class Pix2PixModel:
    def __init__(self, input_shape, output_shape):
        """

        :param input_shape: shape of input images, default is 28 * 28 *1
        :param output_shape: number of classes. for mnist_ds, default is 10

        """
        self.input_shape = input_shape
        self.output_shape = output_shape
    
def pix2pixfromscratch(input_shape, output_shape):
    
    input = layers.Input(shape=(input_shape[0], input_shape[1], input_shape[2]))
    
    # layers.Layers(dtype=tf.uint8)
    
    # initializer = keras.initializers.VarianceScaling(scale=0.7, mode='fan_in', distribution='truncated_normal', seed=150)
    # initializer = keras.initializers.GlorotNormal(seed=None)
    # initializer = keras.initializers.Constant(value=2)
    
    # Block 1
    b1_cnv2d_1 = layers.Conv2D(filters=16, kernel_size=(3, 3), strides=(2, 2), padding='valid',
                    use_bias=True, name='b1_cnv2d_1',activation='relu', kernel_initializer='normal')(input)
    
    output = layers.Dense(output_shape, name='model_output', activation='sigmoid',
                   kernel_initializer='he_uniform')(b1_cnv2d_1)
    
    model = keras.models.Model(input, output)
    
    return model