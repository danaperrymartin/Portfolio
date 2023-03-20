#pragma once
#include <opencv2/core.hpp>

using namespace cv;

class FrequencyFiltering
{
	public:
		void fftshift(const Mat& input_img, Mat& output_img);
		void calculateDFT(Mat& scr, Mat& dst);
		Mat construct_H(Mat& scr, String type, float D0);
		void filtering(Mat& scr, Mat& dst, Mat& H);

	private:
};
