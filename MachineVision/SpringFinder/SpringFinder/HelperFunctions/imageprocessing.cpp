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
namespace fs =std::filesystem;
using namespace cv;

ImageProcessing::ImageProcessing(){}
ImageProcessing::~ImageProcessing(){}

std::vector<std::string> GetImages(std::string folder);
std::vector<std::string> ParseString(std::string s, std::string delimiter);
bool compareNat(const std::string& a, const std::string& b);

int ImageProcessing::ProcessImage()
{
    char path[22] = "TestImages/Processed/";
    std::vector<std::string> images = GetImages(path);
    //Sort files
    std::sort(images.begin(), images.end(), compareNat);
    std::copy(images.begin(), images.end(),std::ostream_iterator<std::string>(std::cout, "\n"));
    for (int ifile = 0; ifile < images.size(); ifile++)
    {
        string filename = images[ifile];
        string image_name = ParseString(ParseString(filename, "/")[2], ".")[0];
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

        //dnn::Net net = dnn::readNetFromONNX("./models/Trucks/pix2pix_1epoch.onnx");

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
        Mat templ = imread("TestImages/SingleSpring.bmp", IMREAD_GRAYSCALE);

        int newHeight = 256;
        int newWidth = 1024;

        //resize(im, im, Size(newWidth, newHeight)); // resize image
        Size templ_sz = templ.size();
        resize(templ, templ, Size(templ_sz.width * 2, templ_sz.height * 1.8)); //resize template

        //Mat imfiltered = imgseg.FrequencySegmentation(im, rects);     // Segment image using frequency based methods
        //Mat imsegmented = imgseg.Predict(imfiltered, rects, cv::Size(newWidth, newHeight), label);
        //imgseg.SelectiveSearch(im, rects);                              // Segment image using Selective Search method
        //imgseg.Watershed(im, rects, newWidth, newHeight);             // Segment image using watershed method
        //imgseg.WatershedwoCond(imsegmented, rects, newWidth, newHeight);             // Segment image using watershed method
        //imgseg.TextureSegmentation(im, rects);                        // Segment image using texture based methods
        imgseg.TemplateSegmentation(im, templ, image_name);              // Segment image based on template image
    }
    //vector<vector<int>> bboxnum(rects.size(), vector<int>(label.size(), 0));

    //std::cout << "Total Number of Region Proposals: " << rects.size() << std::endl;

    //bool doneflag = false;

    //if (!doneflag)
    //{
    //    Mat imOut = im.clone();                                                 // create a copy of original image

    //    modelscore.ModelScore(rects, conf, bboxloc, mLoc, net, imOut, newHeight, newWidth, maxLoc);

    //    sort.SortColumns(maxVal, conf, glob_maxValue, glob_maxLoc, label, bboxnum);

    //    double img_width = (double)newWidth * 5;
    //    double img_height = (double)newHeight * 5;

    //    resize(img_orig, img_orig, Size(img_width, img_height));

    //    double bboxscalefactor_width = ((double)img_orig.size().width / (double)newWidth);
    //    double bboxscalefactor_height = ((double)img_orig.size().height / (double)newHeight);

    //    // Plot non-maximal suppression bounding boxes
    //    nonmax.NMS(label, rects, bboxloc, bboxlabel_afteriou, bboxloc_afteriou, bboxnum);
    //    plot.NonMaximalFeatures(bboxlabel_afteriou, bboxloc_afteriou, bboxscalefactor_width, bboxscalefactor_height, img_orig);

    //    // Plot highest probability bounding boxes
    //    plot.HighestProbabilityFeatures(label, bboxloc, bboxnum, bboxscalefactor_width, bboxscalefactor_height, img_orig);

    //    // Plot Mean probability bounding boxes
    //    plot.MeanFeatures(conf, label, bboxloc, bboxnum, bboxscalefactor_width, bboxscalefactor_height, img_orig);

    //    // Plot Median probability bounding boxes
    //    plot.MedianFeatures(conf, label, bboxloc, bboxnum, bboxscalefactor_width, bboxscalefactor_height, img_orig);

    //    // Plot Mode probability bounding boxes
    //    plot.ModeFeatures(conf, label, bboxloc, bboxnum, bboxscalefactor_width, bboxscalefactor_height, img_orig);

    //    // Write Probabilities to text file:
    //    ofstream out("ConfidenceProbabilities.txt");

    //    // Print ranked probabilities to text file
    //    write.ConfidenceProbabilities(conf, label);

    //    // record key press
    //    int l = waitKey();
    //    if (l == 99)
    //    {
    //    }
    //    else
    //    {
    //    }
    //    doneflag = true;
    //}
    return 0;
}


std::vector<std::string> GetImages(std::string image_dir)
{
    std::vector<std::string> images;

    std::string path(image_dir);
    std::string ext(".bmp");

    int i = 0;
    for (auto& p : fs::recursive_directory_iterator(path))
    {
        if ((p.path().extension() == ext) && ((p.path().string()).find("Error")) == string::npos)
        {
            images.push_back(p.path().string());
        }
    }

    return images;
}

std::vector<std::string> ParseString(std::string s, std::string delimiter)
{
    size_t start;
    size_t end = 0;
    std::vector<std::string> out;
    while ((start = s.find_first_not_of(delimiter, end)) != std::string::npos)
    {
        end = s.find(delimiter, start);
        out.push_back(s.substr(start, end - start));
    }
    return out;
};

bool compareNat(const std::string& a, const std::string& b)
{
    if (a.empty())
        return true;
    if (b.empty())
        return false;
    if (std::isdigit(a[0]) && !std::isdigit(b[0]))
        return true;
    if (!std::isdigit(a[0]) && std::isdigit(b[0]))
        return false;
    if (!std::isdigit(a[0]) && !std::isdigit(b[0]))
    {
        if (std::toupper(a[0]) == std::toupper(b[0]))
            return compareNat(a.substr(1), b.substr(1));
        return (std::toupper(a[0]) < std::toupper(b[0]));
    }

    // Both strings begin with digit --> parse both numbers
    std::istringstream issa(a);
    std::istringstream issb(b);
    int ia, ib;
    issa >> ia;
    issb >> ib;
    if (ia != ib)
        return ia < ib;

    // Numbers are the same --> remove numbers and recurse
    std::string anew, bnew;
    std::getline(issa, anew);
    std::getline(issb, bnew);
    return (compareNat(anew, bnew));
}

