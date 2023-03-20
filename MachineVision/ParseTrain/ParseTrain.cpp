// ParseTrain.cpp : This file contains the 'main' function. Program execution begins and ends there.
//
#include <iostream>
#include <vector>
#include <algorithm>
#include <filesystem>

#include <opencv2/opencv.hpp>
#include <opencv2/core.hpp>
#include <opencv2/ximgproc/segmentation.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/imgproc/imgproc.hpp>
#include <opencv2/highgui.hpp>
#include <opencv2/core/utility.hpp>
#include <opencv2/imgcodecs.hpp>

#include "Header Files/LoadImage.h"
#include "Header Files/ImageProcessing.h"
#include "Header Files/Bitmap.h"

using namespace cv;

int main()
{
	//Load a binary image file
	LoadImage ldimg;

	const std::string filename = "C:\\Dat4TransferReceive\\Gage_2022_07_08_12_47_54_847_FarSide_2048x3072.bin";
	// allocate memory to contain the whole file:
	int64 lSize;
	FILE* pFile;
	pFile = fopen(filename.c_str(), "rb");
	if (pFile == NULL) { fputs("File error", stderr); exit(1); }

	_fseeki64(pFile, 0, SEEK_END);
	lSize = _ftelli64(pFile);
	rewind(pFile);
	unsigned char* buffer;
	buffer = (unsigned char*)malloc(lSize);
	memset(buffer, 0, lSize);
	if (buffer == NULL) { fputs("Memory error", stderr); exit(2); }
	
	ldimg.LoadBinImage(filename, buffer, pFile, lSize);
	//unsigned char* buffer = ret.first;
	//long long int FileSize = lSize;

	long long int img_width  = 2048; 
	long long int num_images = 24;
	long long int img_height =(lSize / (long long int)2048) / num_images;
	long long int start_idx = 0;

	BMPInfoHeader info_header;
	info_header.width = img_width;
	info_header.height = img_height;
	BMPFileHeader file_header;
	file_header.file_size = 54  + img_height*img_width;
	
	//BMP bmp("TestImages\\HexBolt.bmp");

	for (int i = 0; i < num_images; i++)
	{
		std::string image_name = "image" + to_string(i) + ".bmp";
		
		if(((lSize - start_idx) / img_width) < img_height)
			img_height = (lSize - start_idx) / img_width;
		
		info_header.height = img_height;
		BMP bmp(img_width, img_height);
		bmp.fill_region((long long int)0, (long long int)0, img_width, img_height, start_idx, buffer, 255);
		bmp.write(image_name.c_str());
		bmp.data.clear();
		
		/*std::string image_name = "image"+to_string(i)+".bmp";
		writebitmap(buffer, image_name, start_idx,  datachunk, img_height);*/
		start_idx = start_idx + img_width*img_height;
	}
	free(buffer);
	// --------Process buffer into cars-------
	ImageProcessing imgprcs;
	//int status = imgprcs.ProcessImage();
};