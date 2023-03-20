# -*- coding: utf-8 -*-
"""
Created on Tue Oct 12 10:45:00 2021

@author: Dana
"""

import os
  
# Function to rename multiple files
def main():
  
    direct = "TrainRCNN/train_images/ChooChoo_Truck/Truck_Annotations/"
    direct_dest = "TrainRCNN/train_images/ChooChoo_Truck/Truck_Annotations/"
    counter = 0
    
    for count, filename in enumerate(os.listdir(direct)):
        dst = str("Truck"+"%02d" % counter) + ".csv"
        src =direct + filename
        dst =direct_dest+ dst
          
        # rename() function will
        # rename all the files
        os.rename(src, dst)
        # txtfilename = dst.split(".jpg")
        # f = open(txtfilename[0]+'.csv', 'w+')
        counter = counter+1
  
# Driver Code
if __name__ == '__main__':
      
    # Calling main() function
    main()