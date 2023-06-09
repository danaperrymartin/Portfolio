# -*- coding: utf-8 -*-
"""
Created on Mon Apr 18 10:36:29 2022

@author: Dana
"""

# coding=utf-8
# Copyright 2022 The TensorFlow Datasets Authors.
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

"""Freight train dataset."""

import os

import tensorflow as tf

import tensorflow_datasets.public_api as tfds

_DESCRIPTION = """\
Freight train dataset
"""

_CITATION = """\
@InProceedings{martin,
  author       = "",
  title        = "",
  booktitle    = "",
  year         = "",
}
"""

_BASE_URL = "http://www.robots.ox.ac.uk/~vgg/data/pets/data"

_LABEL_CLASSES = [
    "Truck", "Spring"
]
_SPECIES_CLASSES = ["FreightCar"]


class FreightTrain(tfds.core.GeneratorBasedBuilder):
  """Freight train dataset"""

  VERSION = tfds.core.Version("3.2.0")

  def _info(self):
    return tfds.core.DatasetInfo(
        builder=self,
        description=_DESCRIPTION,
        features=tfds.features.FeaturesDict({
            "image":
                tfds.features.Image(),
            "label":
                tfds.features.ClassLabel(names=_LABEL_CLASSES),
            "species":
                tfds.features.ClassLabel(names=_SPECIES_CLASSES),
            "file_name":
                tfds.features.Text(),
            "segmentation_mask":
                tfds.features.Image(shape=(None, None, 1), use_colormap=True)
        }),
        supervised_keys=("image", "label"),
        homepage="www.prorailtech.com/",
        citation=_CITATION,
    )

  def _split_generators(self, dl_manager):
    """Returns splits."""
    # Download images and annotations that come in separate archives.
    # Note, that the extension of archives is .tar.gz even though the actual
    # archives format is uncompressed tar.
    # dl_paths = dl_manager.download_and_extract({
    #     "images": _BASE_URL + "/images.tar.gz",
    #     "annotations": _BASE_URL + "/annotations.tar.gz",
    # })
    dl_paths = {
        "images": '.train_images/MaskData/',
        "annotations": '.train_images/MaskData/',
    }

    images_path_dir = os.path.join(dl_paths["images"], "images")
    annotations_path_dir = os.path.join(dl_paths["annotations"], "imagemasks")

    # Setup train and test splits
    train_split = tfds.core.SplitGenerator(
        name="train",
        gen_kwargs={
            "images_dir_path":
                images_path_dir,
            "annotations_dir_path":
                annotations_path_dir,
            "images_list_file":
                os.path.join(annotations_path_dir, "trainval.txt"),
        },
    )
    test_split = tfds.core.SplitGenerator(
        name="test",
        gen_kwargs={
            "images_dir_path": images_path_dir,
            "annotations_dir_path": annotations_path_dir,
            "images_list_file": os.path.join(annotations_path_dir, "test.txt")
        },
    )

    return [train_split, test_split]

  def _generate_examples(self, images_dir_path, annotations_dir_path,
                         images_list_file):
    with tf.io.gfile.GFile(images_list_file, "r") as images_list:
      for line in images_list:
        image_name, label, species, _ = line.strip().split(" ")

        trimaps_dir_path = os.path.join(annotations_dir_path, "trimaps")

        trimap_name = image_name + ".png"
        image_name += ".jpg"
        label = int(label) - 1
        species = int(species) - 1

        record = {
            "image": os.path.join(images_dir_path, image_name),
            "label": int(label),
            "species": species,
            "file_name": image_name,
            "segmentation_mask": os.path.join(trimaps_dir_path, trimap_name)
        }
        yield image_name, record