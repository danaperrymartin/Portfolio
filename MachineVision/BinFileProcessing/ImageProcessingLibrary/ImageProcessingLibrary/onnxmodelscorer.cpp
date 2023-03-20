#include "pch.h"

#include "onnxmodelscorer.h"

using namespace std;
using namespace cv;

OnnxModelScorer::OnnxModelScorer(){}
OnnxModelScorer::~OnnxModelScorer() {}

void OnnxModelScorer::ModelScore(vector<Rect>& rects, vector<vector<double>>& conf, vector<Rect*>& bboxloc, vector<Point> mLoc, dnn::Net& net, Mat& imOut, int& newHeight, int& newWidth, Point& maxLoc)
{
    for (int i = 0; i < rects.size(); i++)
    {
        Mat blob = imOut(rects[i]);
        resize(blob, blob, Size(newHeight, newWidth));
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