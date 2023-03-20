#pragma once

#include <opencv2/core.hpp>
#include <opencv2/dnn/dnn.hpp>

using namespace std;
using namespace cv;

class ImageSegmentation
{
	public:
		ImageSegmentation();   //Constructor
		~ImageSegmentation();  //Destructor

		void SelectiveSearch(Mat& im, vector<Rect>& rects);

		cv::Mat Predict(Mat im, vector<Rect>& rects, cv::Size IMAGE_SIZE, vector<string> label);

		void Watershed(Mat& im, vector<Rect>& rects, int width, int height);

		void WatershedwoCond(Mat& im, vector<Rect>& rects, int width, int height);

		void TextureSegmentation(Mat& im, vector<Rect>& rects);

		Mat FrequencySegmentation(Mat& im, vector<Rect>& rects);

		void TemplateSegmentation(Mat& im, Mat& templ, string image_name);

	private:

};
