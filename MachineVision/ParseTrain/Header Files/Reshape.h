#pragma once
#include <vector>
#include <string>
#include <vector>
#include <opencv2/core.hpp>

#include <iostream>

using namespace cv;
using namespace std;

class Reshape
{
	public:
		auto  ReshapeMatrix(int numimages, unsigned char* buffer, int destwidth, int destheight, std::vector<vector<double>> mat_reshaped, cv::Mat& img_mat, int64 start_idx, bool save_img, int64 lSize, std::string car="", std::filesystem::path img_savedirectory="", std::string camerside="")
		{
			cout << "Reshaping image matrix..." << std::endl;
			unsigned int k = std::min(start_idx*2048, lSize-start_idx*2048);
			for (int j = 0; j < destheight; j++)
			{
				for (int i = 0; i < destwidth; i++)
				{
					if (k < lSize)
					{
						img_mat.at<double>(i, j) = (double)buffer[k] / 255;
						k++;
					}
				}
			}

			if (save_img)
			{
				cout << "Saveing" + car << std::endl;
				Mat resized_down;
				int down_width = 1920/2;
				int down_height = 1080/2;
				resize(img_mat, resized_down, Size(down_width, down_height), INTER_LINEAR);
				vector<int> compression_params;
				compression_params.push_back(IMWRITE_JPEG_QUALITY);
				compression_params.push_back(95);
				
				bool result = imwrite((img_savedirectory.string() +"\\"+camerside+"_" + car + ".jpg"), resized_down * 255, compression_params);
			}
			//}
		}
};