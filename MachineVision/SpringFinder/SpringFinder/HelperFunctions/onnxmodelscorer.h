#pragma once
#include <opencv2/ximgproc/segmentation.hpp>
#include <opencv2/highgui.hpp>
#include <opencv2/core.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/core/core.hpp>
#include <opencv2/highgui/highgui.hpp>
#include <opencv2/dnn.hpp>

using namespace cv;
//using namespace cv::imgproc::segmentation;

class OnnxModelScorer
{
	public:
		OnnxModelScorer();
		~OnnxModelScorer();

		void ModelScore(std::vector<Rect>& rects, std::vector<std::vector<double>>& conf, std::vector<Rect*>& bboxloc, std::vector<Point> mLoc, dnn::Net& net, Mat& imOut, int& newHeight, int& newWidth, Point& maxLoc);

	private:

};