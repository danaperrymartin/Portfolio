// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"

#include "imageprocessing.h"
#include "sort.h"
#include "onnxmodelscorer.h"
#include "nonmaximalsuppression.h"
#include "plotfeatures.h"
#include "writetextfile.h"
#include "imagesegmentation.h"
#include "texturehelperfunctions.h"

#include <opencv2\core.hpp>
#include <opencv2\dnn\dnn.hpp>
#include <opencv2\imgcodecs.hpp>

using namespace cv;
using namespace std;

extern "C"
{
    __declspec(dllexport) int add(int a, int b)
    {
        return a + b;
    }
    __declspec(dllexport) int subtract(int a, int b)
    {
        return a - b;
    }
    __declspec(dllexport) void ProcessImageEntry(unsigned char* data, long dataLen)
    {
        vector<unsigned char> inputImageBytes(data, data + dataLen);
        Mat img = imdecode(inputImageBytes, IMREAD_COLOR);
        cv::cvtColor(img, img, COLOR_RGB2GRAY);  // Convert to grayscale

        TextureHelperFunctions texture;
        texture.displayImage("InputImage", img);

        //char filename[99] = "C:/CodeProjects/MachineVision/BinFileProcessing/PlayTrainVideo/bin/x64/Debug/TestImages/Truck2.bmp";
        char searchtype[2] = "f";
        bool highest_prob = 1;
        double minVal;
        double maxVal = 0.0;
        Point minLoc;
        Point maxLoc;
        vector<Rect*> bboxloc_afteriou;
        vector<string> bboxlabel_afteriou;
        vector<vector<double>> conf;
        vector<Rect*> bboxloc;
        vector<Rect> rects;
        vector<Point> mLoc;
        Mat result;
        vector<double*> glob_maxValue{ new double(0), new double(0), new double(0) };
        vector<int*> glob_maxLoc{ new int(0), new int(0), new int(0) };

        //dnn::Net net = dnn::readNetFromONNX("models/Trucks/pix2pix.onnx");

        //dnn::Net net = dnn::readNetFromTensorflow("./models/Trucks/pix2pixmodel.pb");
        vector<string> label{ "Background", "Truck" };

        // Initialize external class objects
        Sort sort;
        OnnxModelScorer modelscore;
        NonMaximalSuppression nonmax;
        PlotFeatures plot;
        WriteTextFile write;
        ImageSegmentation imgseg;
        
        // speed-up using multithreads
        setUseOptimized(true);
        setNumThreads(1);

        // read image
        //Mat img = imread(filename, IMREAD_GRAYSCALE);
        //Mat img;
        
        //read template
        Mat templ = imread("C:/CodeProjects/MachineVision/SpringFinder/SpringFinder/TestImages/Spring3.bmp", IMREAD_GRAYSCALE);

        int newHeight = 256;
        int newWidth = 1024;

        resize(img, img, Size(newWidth, newHeight)); // resize image
        cv::Size sz = img.size();
        resize(templ, templ, Size(sz.height, sz.height)); //resize template
        texture.displayImage("template", templ);

        Mat imfiltered = imgseg.FrequencySegmentation(img, rects);     // Segment image using frequency based methods
        Mat imsegmented = imgseg.Predict(imfiltered, rects, cv::Size(newWidth, newHeight), label);
        //imgseg.SelectiveSearch(im, rects);                              // Segment image using Selective Search method
        //imgseg.Watershed(im, rects, newWidth, newHeight);             // Segment image using watershed method
        //imgseg.WatershedwoCond(imsegmented, rects, newWidth, newHeight);             // Segment image using watershed method
        //imgseg.TextureSegmentation(im, rects);                        // Segment image using texture based methods
        imgseg.TemplateSegmentation(img, templ);              // Segment image based on template image
    }
};

