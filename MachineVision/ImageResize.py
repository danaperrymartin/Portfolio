# -*- coding: utf-8 -*-
"""
Created on Tue Oct 12 10:45:00 2021

@author: Dana
"""

import os
import cv2  

# Function to rename multiple files
def main():
  
    direct = "C:/KaggleComps/tensorflow-great-barrier-reef/train_images/COT/"
    direct_dest = "C:/KaggleComps/tensorflow-great-barrier-reef/train_images/COT/"
    counter = 0
    
    for count, filename in enumerate(os.listdir(direct)):
        dst = str("%02d" % counter) + ".jpg"
        src =direct + filename
        dst =direct_dest+ dst
          
        # rename() function will
        # rename all the files
        img = cv2.imread(src, cv2.IMREAD_UNCHANGED)
        resized = cv2.resize(img, (128,128), interpolation = cv2.INTER_AREA)
        cv2.imwrite(src,resized)
        # os.rename(src, dst)
        # txtfilename = dst.split(".jpg")
        # f = open(txtfilename[0]+'.csv', 'w+')
        counter = counter+1
  
# Driver Code
if __name__ == '__main__':
      
    # Calling main() function
    main()