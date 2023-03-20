#include "pch.h"

#include "pooling.h"

#include <opencv2/core.hpp>

using namespace std;
using namespace cv;

Pooling::Pooling() {}
Pooling::~Pooling() {}

void Pooling::MaxPooling2D(Mat img, Mat& imgout, vector<int>& poolxy, int& channels)
{
    imgout = img.clone();
    for (int y = 0; y < img.rows; ++y)
    {
        for (int x = 0; x < img.cols; ++x)
        {
            for (int i = 0; i < poolxy[0]; ++i)
            {
                for (int j = 0; j < poolxy[1]; ++j)
                {
                    for (int c = 0; c < channels; ++c)
                    {
                        float value = img.at<float>(y + i, x + j);
                        imgout.at<float>(y, x) = max(img.at<float>(y, x), value);
                    }
                }
            }
        }
    }
}

void Pooling::AvePooling2D(cv::Mat img, cv::Mat& imgout, std::vector<int>& poolxy, int& channels)
{
    imgout = img.clone();
    for (int y = 0; y < img.rows; ++y)
    {
        for (int x = 0; x < img.cols; ++x)
        {
            for (int i = 0; i < poolxy[0]; ++i)
            {
                for (int j = 0; j < poolxy[1]; ++j)
                {
                    for (int c = 0; c < channels; ++c)
                    {
                        float value = img.at<float>(y + i, x + j);
                        imgout.at<float>(y, x) = (img.at<float>(y, x) + value) / poolxy[0];
                    }
                }
            }
        }
    }
}

void Pooling::MinPooling2D(cv::Mat img, cv::Mat& imgout, std::vector<int>& poolxy, int& channels)
{
    imgout = img.clone();
    for (int y = 0; y < img.rows; ++y)
    {
        for (int x = 0; x < img.cols; ++x)
        {
            for (int i = 0; i < poolxy[0]; ++i)
            {
                for (int j = 0; j < poolxy[1]; ++j)
                {
                    for (int c = 0; c < channels; ++c)
                    {
                        float value = img.at<float>(y + i, x + j);
                        imgout.at<float>(y, x) = min(img.at<float>(y, x), value);
                    }
                }
            }
        }
    }
}