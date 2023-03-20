#pragma once
#include <opencv2/ximgproc/segmentation.hpp>
#include <opencv2/highgui.hpp>
#include <opencv2/core.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/core/core.hpp>
#include <opencv2/highgui/highgui.hpp>
#include <opencv2/dnn.hpp>

using namespace cv;
using namespace cv::ximgproc::segmentation;
class OnnxModelScorer
{
public:
    void ModelScore(std::vector<Rect> &rects, std::vector<std::vector<double>>& conf, std::vector<Rect*>& bboxloc, std::vector<cv::Point> mLoc, cv::dnn::Net& net, Mat &imOut, int &newHeight, int &newWidth, Point& maxLoc)
    {
        for (int i = 0; i < rects.size(); i++)
        {
            cv::Mat blob = imOut(rects[i]);
            cv::resize(blob, blob, Size(newHeight, newWidth));
            blob.convertTo(blob, CV_32F, 1.0 / 255, -0.5);
            
            blob = blob.reshape(1, { 1, newHeight, newWidth, 3 });

            net.setInput(blob);
            Mat result = net.forward();

            conf.push_back(result);
            bboxloc.push_back(&rects[i]);

            int iter = reinterpret_cast<int>(&maxLoc);
            mLoc.push_back(maxLoc);

        }
    }
};

