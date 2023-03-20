#include "pch.h"

#include <string>
#include <fstream>
#include <iostream>
#include <filesystem>

#include <opencv2/core.hpp>
#include <opencv2/dnn/dnn.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/imgcodecs.hpp>
#include <opencv2/highgui.hpp>

#include "imageprocessing.h"
#include "sort.h"
#include "onnxmodelscorer.h"
#include "nonmaximalsuppression.h"
#include "plotfeatures.h"
#include "writetextfile.h"
#include "imagesegmentation.h"

using namespace std;
using namespace cv;

ImageProcessing::ImageProcessing(){}
ImageProcessing::~ImageProcessing(){}

int ImageProcessing::ProcessImage()
{
    char filename[36] = "TestImages/Truck2.bmp";
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

    dnn::Net net = dnn::readNetFromONNX("./models/Trucks/pix2pix.onnx");

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
    Mat im = imread(filename, IMREAD_GRAYSCALE);
    //Mat im = imread(filename);
    Mat img_orig = im.clone();

    //read template
    Mat templ = imread("TestImages/Spring.bmp", IMREAD_GRAYSCALE);

    int newHeight = 256;
    int newWidth = 1024;

    //resize(im, im, Size(newWidth, newHeight)); // resize image
    //resize(templ, templ, Size(newWidth/2, newHeight/2)); //resize template

    //Mat imfiltered = imgseg.FrequencySegmentation(im, rects);     // Segment image using frequency based methods
    //Mat imsegmented = imgseg.Predict(imfiltered, rects, cv::Size(newWidth, newHeight), label);
    //imgseg.SelectiveSearch(im, rects);                              // Segment image using Selective Search method
    //imgseg.Watershed(im, rects, newWidth, newHeight);             // Segment image using watershed method
    //imgseg.WatershedwoCond(imsegmented, rects, newWidth, newHeight);             // Segment image using watershed method
    //imgseg.TextureSegmentation(im, rects);                        // Segment image using texture based methods
    imgseg.TemplateSegmentation(im, templ);              // Segment image based on template image

    vector<vector<int>> bboxnum(rects.size(), vector<int>(label.size(), 0));

    std::cout << "Total Number of Region Proposals: " << rects.size() << std::endl;

    bool doneflag = false;

    if (!doneflag)
    {
        Mat imOut = im.clone();                                                 // create a copy of original image

        modelscore.ModelScore(rects, conf, bboxloc, mLoc, net, imOut, newHeight, newWidth, maxLoc);

        sort.SortColumns(maxVal, conf, glob_maxValue, glob_maxLoc, label, bboxnum);

        double img_width = (double)newWidth * 5;
        double img_height = (double)newHeight * 5;

        resize(img_orig, img_orig, Size(img_width, img_height));

        double bboxscalefactor_width = ((double)img_orig.size().width / (double)newWidth);
        double bboxscalefactor_height = ((double)img_orig.size().height / (double)newHeight);

        // Plot non-maximal suppression bounding boxes
        nonmax.NMS(label, rects, bboxloc, bboxlabel_afteriou, bboxloc_afteriou, bboxnum);
        plot.NonMaximalFeatures(bboxlabel_afteriou, bboxloc_afteriou, bboxscalefactor_width, bboxscalefactor_height, img_orig);

        // Plot highest probability bounding boxes
        plot.HighestProbabilityFeatures(label, bboxloc, bboxnum, bboxscalefactor_width, bboxscalefactor_height, img_orig);

        // Plot Mean probability bounding boxes
        plot.MeanFeatures(conf, label, bboxloc, bboxnum, bboxscalefactor_width, bboxscalefactor_height, img_orig);

        // Plot Median probability bounding boxes
        plot.MedianFeatures(conf, label, bboxloc, bboxnum, bboxscalefactor_width, bboxscalefactor_height, img_orig);

        // Plot Mode probability bounding boxes
        plot.ModeFeatures(conf, label, bboxloc, bboxnum, bboxscalefactor_width, bboxscalefactor_height, img_orig);

        // Write Probabilities to text file:
        ofstream out("ConfidenceProbabilities.txt");

        // Print ranked probabilities to text file
        write.ConfidenceProbabilities(conf, label);

        // record key press
        int l = waitKey();
        if (l == 99)
        {
        }
        else
        {
        }
        doneflag = true;
    }
    return 0;
}

