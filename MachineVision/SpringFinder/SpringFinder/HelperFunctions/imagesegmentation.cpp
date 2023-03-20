#include <iostream>

#include "imagesegmentation.h"
#include "sort.h"
#include "pooling.h"
#include "texturehelperfunctions.h"
#include "frequencyfiltering.h"
#include "writetextfile.h"

#include <opencv2/opencv.hpp>
#include <opencv2/core.hpp>
#include <opencv2/imgproc/segmentation.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/imgproc/imgproc.hpp>

#include <opencv2/ximgproc/segmentation.hpp>

#include <opencv2/dnn/dnn.hpp>

#include <opencv2/highgui.hpp>
#include <opencv2/core/utility.hpp>
#include <opencv2/imgcodecs.hpp>

using namespace std;
using namespace cv;
using namespace cv::ximgproc::segmentation;

ImageSegmentation::ImageSegmentation() {}
ImageSegmentation::~ImageSegmentation() {}

void ImageSegmentation::SelectiveSearch(Mat& im, vector<Rect>& rects)
{
    Ptr<SelectiveSearchSegmentation> ss = createSelectiveSearchSegmentation(); // create Selective Search Segmentation Object using default parameters
    ss->setBaseImage(im);                                                      // set input image on which we will run segmentation
    ss->switchToSelectiveSearchFast();                                      // Switch to fast but low recall Selective Search method
    ss->process(rects);                                                        // run selective search segmentation on input image
}
cv::Mat ImageSegmentation::Predict(Mat im, vector<Rect>& rects, cv::Size IMAGE_SIZE, vector<string> label)
{
    TextureHelperFunctions texture;
    //GaussianBlur(im, im, Size(3, 3), 1);
    dnn::Net net = dnn::readNetFromONNX("./models/Trucks/pix2pix_5epoch.onnx");

    //cv::Mat blob;
    //blob.convertTo(blob, CV_32F, 1.0 , -0.5);

    cv::Mat blob = dnn::blobFromImage(im, 255);

    //blob = blob.reshape(1, { 1, IMAGE_SIZE.height,  IMAGE_SIZE.width, 1});

    net.setInput(blob);
    //std::vector<cv::String> output_layers = net.getUnconnectedOutLayersNames();
    //net.setPreferableBackend(cv::dnn::DNN_BACKEND_OPENCV);
    //net.setPreferableTarget(cv::dnn::DNN_TARGET_CPU);
    cv::Mat output = net.forward();    // run selective search segmentation on input image

    cv::Mat newoutput = output.reshape(1, { ((int)label.size())*IMAGE_SIZE.height, IMAGE_SIZE.width });

    //texture.displayImage("Predictions", newoutput);

    cv::Mat segmentation(IMAGE_SIZE.height, IMAGE_SIZE.width, CV_32F);

    vector<float> classpred(label.size());
    
    for (int i = 0; i < IMAGE_SIZE.height; i++)
    {
        for (int j = 0; j < IMAGE_SIZE.width; j++)
        {
           /* for (int iclass = 0; iclass < label.size(); iclass++)
            {
                if (newoutput.at<float>(iclass * (IMAGE_SIZE.height), j) > 0.0)
                {
                    std::cout << to_string(newoutput.at<float>(iclass * (IMAGE_SIZE.height), j));
                }

                classpred[iclass] = newoutput.at<float>(iclass*(IMAGE_SIZE.height), j);
            }*/
  
            segmentation.at<float>(i, j) = newoutput.at<float>(i,j);
        }
    }

   
    //texture.displayImage("Segmentation", segmentation);
    normalize(segmentation, segmentation, 0, 255, NORM_MINMAX);
    //texture.saveImage("cGANSemanticSegmentation", segmentation);

    return segmentation;
};

void ImageSegmentation::Watershed(Mat& im, vector<Rect>& rects, int width, int height)
{
    TextureHelperFunctions texture;

    Mat graymat;
    Mat opening;
    Mat sure_bg(height , width, CV_32F);
    Mat sure_fg(height, width, CV_32F);
    Mat dist_transform;
    Mat unknown(height, width, CV_32F);
    Mat mask;
    float kerneldata[9] = { 1, 1, 1, 1, 1, 1, 1, 1, 1 };
    Mat kernelmatrix(3, 3, CV_32F, kerneldata);
    vector<int> poolxy = { 8,9 };

    double minVal;
    double maxVal = 0.0;
    Point minLoc;
    Point maxLoc;

    graymat = im;
    //cvtColor(im, graymat, cv::COLOR_BGR2GRAY);
    //cvtColor(im, graymat, cv::COLOR_BGR2HSV);

    GaussianBlur(graymat, graymat, Size(3, 3), 10);
    //blur(graymat, graymat, Size(9, 9));
    //medianBlur(graymat, graymat, 5);
    //filter2D(graymat, graymat, -1, 0.25*kernelmatrix);

    Pooling pool;
    Mat graymatout;
    int channels = 1;
    //pool.MaxPooling2D(graymat, graymat, poolxy, channels);
    //pool.AvePooling2D(graymat, graymat, poolxy, channels);
    //pool.MinPooling2D(graymat, graymat, poolxy, channels);

    texture.displayImage("Output1", graymat);

    threshold(graymat, graymat, 0, 1,THRESH_OTSU);
    //threshold(graymat, graymat, 127, 180, THRESH_BINARY);

    vector<int> kernel(3, 1);

    morphologyEx(graymat, opening, MORPH_OPEN, (int)1);
    int erosion_type = 0;
    int erosion_size = 0;
    Mat element = getStructuringElement(erosion_type,
        Size(2 * erosion_size + 1, 2 * erosion_size + 1),
        Point(erosion_size, erosion_size));

    erode(graymat, sure_bg, element);
    dilate(graymat, sure_bg, kernelmatrix);
    distanceTransform(opening, dist_transform, cv::DIST_L2, (int)5);
    minMaxLoc(dist_transform, &minVal, &maxVal, &minLoc, &maxLoc);
    threshold(dist_transform, sure_fg, (double)0.1 * (maxVal), 255, 0);
    threshold(dist_transform, sure_bg, (double)0.1 * (maxVal), 255, 0);
    for (int i = 0; i < sure_bg.rows-10; i++)
    {
        for (int j = 0; j < sure_bg.cols-10; j++)
        {
            unknown.at<float>(i, j) = sure_bg.at<float>(i, j) - sure_fg.at<float>(i, j);
        }
    }

    Mat markers(unknown.size(), CV_32S);

    sure_fg.convertTo(sure_fg, CV_8UC1);

    connectedComponents(sure_fg, markers);

    markers.setTo(Scalar(0, 0, 0), unknown == 0);

    cvtColor(im, im, COLOR_GRAY2RGB);
    watershed(im, markers);
    im.setTo(Scalar(0, 0, 255), markers == -1);

    Sort sort;

    markers.convertTo(markers, CV_32F);
    vector<float> unique_markers;
    vector<cv::Rect> watershedbb;
    Mat im_out;
    vector<std::vector<cv::Point>> contours;
    vector<Vec4i> hierarchy;
    Mat im_gray;

    threshold(im, im_out, 0, 250, THRESH_BINARY_INV);
    cvtColor(im_out, im_gray, COLOR_BGR2GRAY);
    
    texture.displayImage("Output2", im_gray);
    
    findContours(im_gray, contours, hierarchy, RETR_TREE, CHAIN_APPROX_NONE);

    for (size_t idx = 0; idx < contours.size(); idx++)
    {
        drawContours(im, contours, idx, Scalar(0, 255, 0));
    }

    texture.displayImage("Output", im);

    vector<vector<Point> > contours_poly(contours.size());
    vector<Rect> boundRect(contours.size());
    vector<Point2f>center(contours.size());
    vector<float>radius(contours.size());
    RNG rng(12345);

    for (size_t i = 0; i < contours.size(); i++)
    {
        approxPolyDP(Mat(contours[i]), contours_poly[i], 3, true);
        boundRect[i] = boundingRect(Mat(contours_poly[i]));
        minEnclosingCircle(contours_poly[i], center[i], radius[i]);
    }

    rects = boundRect;

    int k = 0;
    while (k < rects.size())
    {
        rectangle(im, rects[k], Scalar(0, 255, 0));
        k++;
    }

    texture.displayImage("Output", im);
    
    imwrite("WatershedSegmentation.jpg", im);
}

void ImageSegmentation::WatershedwoCond(Mat& im, vector<Rect>& rects, int width, int height)
{
    TextureHelperFunctions texture;

    Mat graymat;
    Mat opening;
    Mat sure_bg(height, width, CV_32F);
    Mat sure_fg(height, width, CV_32F);
    Mat dist_transform;
    Mat unknown(height, width, CV_32F);
    Mat mask;
    float kerneldata[9] = { 1, 1, 1, 1, 1, 1, 1, 1, 1 };
    Mat kernelmatrix(3, 3, CV_32F, kerneldata);
    vector<int> poolxy = { 8,9 };

    double minVal;
    double maxVal = 0.0;
    Point minLoc;
    Point maxLoc;

    graymat = im;
    //cvtColor(im, graymat, cv::COLOR_BGR2GRAY);
    //cvtColor(im, graymat, cv::COLOR_BGR2HSV);

    //GaussianBlur(graymat, graymat, Size(3, 3), 10);
    //blur(graymat, graymat, Size(9, 9));
    //medianBlur(graymat, graymat, 5);
    //filter2D(graymat, graymat, -1, 0.25*kernelmatrix);

    Pooling pool;
    Mat graymatout;
    int channels = 1;
    //pool.MaxPooling2D(graymat, graymat, poolxy, channels);
    //pool.AvePooling2D(graymat, graymat, poolxy, channels);
    //pool.MinPooling2D(graymat, graymat, poolxy, channels);

    texture.displayImage("Output1", graymat);

    //threshold(graymat, graymat, 0, 1, THRESH_OTSU);
    //threshold(graymat, graymat, 127, 180, THRESH_BINARY);

    vector<int> kernel(3, 1);

   //morphologyEx(graymat, opening, MORPH_OPEN, (int)1);
    int erosion_type = 0;
    int erosion_size = 0;
    Mat element = getStructuringElement(erosion_type,
        Size(2 * erosion_size + 1, 2 * erosion_size + 1),
        Point(erosion_size, erosion_size));
    
    erode(graymat, graymat, element);
    //dilate(graymat, graymat, kernelmatrix);
    //distanceTransform(opening, dist_transform, cv::DIST_L2, (int)5);
    //minMaxLoc(dist_transform, &minVal, &maxVal, &minLoc, &maxLoc);
    //threshold(dist_transform, sure_fg, (double)0.1 * (maxVal), 255, 0);
    //threshold(dist_transform, sure_bg, (double)0.1 * (maxVal), 255, 0);
    //for (int i = 0; i < sure_bg.rows - 10; i++)
    //{
    //    for (int j = 0; j < sure_bg.cols - 10; j++)
    //    {
    //        unknown.at<float>(i, j) = sure_bg.at<float>(i, j) - sure_fg.at<float>(i, j);
    //    }
    //}

    Mat markers(unknown.size(), CV_32S);

    im.convertTo(im, CV_8UC1);

    connectedComponents(im, markers);

    markers.setTo(Scalar(0, 0, 0), unknown == 1);

    cvtColor(im, im, COLOR_GRAY2RGB);
    watershed(im, markers);
    im.setTo(Scalar(255, 0, 0), markers == -1);

    Sort sort;

    markers.convertTo(markers, CV_32F);
    vector<float> unique_markers;
    vector<cv::Rect> watershedbb;
    Mat im_out;
    vector<std::vector<cv::Point>> contours;
    vector<Vec4i> hierarchy;
    Mat im_gray;

    threshold(im, im_out, 0, 100, THRESH_BINARY);
    cvtColor(im_out, im_gray, COLOR_BGR2GRAY);

    texture.displayImage("Output2", im_gray);

    findContours(im_gray, contours, hierarchy, RETR_TREE, CHAIN_APPROX_NONE);

    for (size_t idx = 0; idx < contours.size(); idx++)
    {
        drawContours(im, contours, idx, Scalar(0, 255, 0));
    }

    texture.displayImage("Output", im);

    texture.saveImage("Measured", im);

    vector<vector<Point> > contours_poly(contours.size());
    vector<Rect> boundRect(contours.size());
    vector<Point2f>center(contours.size());
    vector<float>radius(contours.size());
    RNG rng(12345);

    for (size_t i = 0; i < contours.size(); i++)
    {
        approxPolyDP(Mat(contours[i]), contours_poly[i], 3, true);
        boundRect[i] = boundingRect(Mat(contours_poly[i]));
        minEnclosingCircle(contours_poly[i], center[i], radius[i]);
    }

    rects = boundRect;

    int k = 0;
    while (k < rects.size())
    {
        rectangle(im, rects[k], Scalar(0, 255, 0));
        k++;
    }

    texture.displayImage("BBoxOutput", im);

    imwrite("WatershedSegmentation.jpg", im);
}

void ImageSegmentation::TextureSegmentation(Mat& im, vector<Rect>& rects)
{
    Mat input = im;

    TextureHelperFunctions texture;
    float const TEXTURE_EDGE_THRESHOLD = 130;

    texture.displayImage("Image", input);

    //Padd image
    Mat paddedImage;
    texture.symmetricPadding(input, paddedImage);
    texture.displayImage("Padded image", paddedImage);

    //Calculate entropy matrix
    Mat entropyMat = Mat::zeros(input.rows, input.cols, CV_32FC1);
    texture.entropyFilt(paddedImage, entropyMat);

    //Rescale entropy matrix
    normalize(entropyMat, entropyMat, 1, 0, NORM_MINMAX, -1, Mat());
    texture.displayImage("Normalized entropy", entropyMat);
    texture.saveImage("Normalized entropy", entropyMat);

    //Convert to [0, 255]]
    Mat convertedEntropyMat;
    texture.conversion(entropyMat, convertedEntropyMat);

    //Threshold textures
    threshold(convertedEntropyMat, convertedEntropyMat, TEXTURE_EDGE_THRESHOLD,100/*1*/, THRESH_BINARY);
    texture.displayImage("Threshold Normalized entropy", convertedEntropyMat);
    texture.saveImage("Threshold Normalized entropy", convertedEntropyMat);

    //Filter areas
    Mat filteredEntropyMat = Mat::zeros(input.rows, input.cols, CV_8UC1);
    texture.bwareaopen(convertedEntropyMat, filteredEntropyMat);
    texture.displayImage("Filtered entropy", filteredEntropyMat);
    texture.saveImage("Filtered entropy", filteredEntropyMat);

    //Smooth edges and close open holes
    //texture.imclose(filteredEntropyMat);
    //texture.displayImage("Closed image", filteredEntropyMat);
    //texture.saveImage("Closed image", filteredEntropyMat);

    //Fill holes
    //texture.imfill(filteredEntropyMat);
    //texture.displayImage("Filled image", filteredEntropyMat);
    //texture.saveImage("Filled image", filteredEntropyMat);

    //Mask image
    Mat output;
    /*texture.maskImage(input, filteredEntropyMat, output);
    texture.displayImage("Output", output);*/
}

Mat ImageSegmentation::FrequencySegmentation(Mat& im, vector<Rect>& rects)
{
    Mat input = im;

    TextureHelperFunctions texture;
    FrequencyFiltering ff;

    //texture.displayImage("Image", input);

    Mat padded;                            //expand input image to optimal size
    int m = getOptimalDFTSize(input.rows);
    int n = getOptimalDFTSize(input.cols); // on the border add zero values
    copyMakeBorder(input, padded, 0, m - input.rows, 0, n - input.cols, BORDER_CONSTANT, Scalar::all(0));
    Mat planes[] = { Mat_<float>(padded), Mat::zeros(padded.size(), CV_32F) };
    Mat complexI;
    merge(planes, 2, complexI);         // Add to the expanded another plane with zeros
    dft(complexI, complexI);            // this way the result may fit in the source matrix
    // compute the magnitude and switch to logarithmic scale
    // => log(1 + sqrt(Re(DFT(I))^2 + Im(DFT(I))^2))
    split(complexI, planes);                   // planes[0] = Re(DFT(I), planes[1] = Im(DFT(I))
    magnitude(planes[0], planes[1], planes[0]);// planes[0] = magnitude
    Mat magI = planes[0];
    magI += Scalar::all(1);                    // switch to logarithmic scale
    log(magI, magI);
    // crop the spectrum, if it has an odd number of rows or columns
    magI = magI(Rect(0, 0, magI.cols & -2, magI.rows & -2));
    // rearrange the quadrants of Fourier image  so that the origin is at the image center
    int cx = magI.cols / 2;
    int cy = magI.rows / 2;
    Mat q0(magI, Rect(0, 0, cx, cy));   // Top-Left - Create a ROI per quadrant
    Mat q1(magI, Rect(cx, 0, cx, cy));  // Top-Right
    Mat q2(magI, Rect(0, cy, cx, cy));  // Bottom-Left
    Mat q3(magI, Rect(cx, cy, cx, cy)); // Bottom-Right
    Mat tmp;                           // swap quadrants (Top-Left with Bottom-Right)
    q0.copyTo(tmp);
    q3.copyTo(q0);
    tmp.copyTo(q3);
    q1.copyTo(tmp);                    // swap quadrant (Top-Right with Bottom-Left)
    q2.copyTo(q1);
    tmp.copyTo(q2);

    // construct H
    Mat H;
    H = ff.construct_H(magI, "Gaussian", 40);

    // filtering
    Mat complexIH;
    ff.filtering(complexI, complexIH, H);

    // IDFT
    Mat imgOut;
    dft(complexIH, imgOut, DFT_INVERSE | DFT_REAL_OUTPUT);

    normalize(magI, magI, 0, 1, NORM_MINMAX); // Transform the matrix with float values into a
    normalize(imgOut, imgOut, 0, 1, NORM_MINMAX); // Transform the matrix with float values into a
    // viewable image form (float between values 0 and 1).
    //imshow("Input Image", input);    // Show the result
    //imshow("spectrum magnitude", magI);
    //imshow("filtered image", imgOut);
    //waitKey();

    return imgOut;
}

void ImageSegmentation::TemplateSegmentation(Mat& img, Mat& templ, string image_name)
{
    Mat img_display;
    Mat result;
    Mat mask;
    bool use_mask = false;
    cv::TemplateMatchModes match_method = cv::TemplateMatchModes::TM_SQDIFF;

    img.copyTo(img_display);
    int result_cols = img.cols - templ.cols + 1;
    int result_rows = img.rows - templ.rows + 1;
    result.create(result_rows, result_cols, CV_32FC1);
    bool method_accepts_mask = (TM_SQDIFF == match_method || match_method == TM_CCORR_NORMED);
    if (use_mask && method_accepts_mask)
    {
        matchTemplate(img, templ, result, match_method, mask);
    }
    else
    {
        matchTemplate(img, templ, result, match_method);
    }
    normalize(result, result, 0, 1, NORM_MINMAX, -1, Mat());
    double minVal; double maxVal; Point minLoc; Point maxLoc;
    Point matchLoc;
    minMaxLoc(result, &minVal, &maxVal, &minLoc, &maxLoc, Mat());
    if (match_method == TM_SQDIFF || match_method == TM_SQDIFF_NORMED)
    {
        matchLoc = minLoc;
    }
    else
    {
        matchLoc = maxLoc;
    }
    rectangle(img_display, matchLoc, Point(matchLoc.x + templ.cols, matchLoc.y + templ.rows), Scalar(255, 255, 255), 4, 8, 0);
    rectangle(result, matchLoc, Point(matchLoc.x + templ.cols, matchLoc.y + templ.rows), Scalar::all(0), 2, 8, 0);
    
    TextureHelperFunctions texture;
    texture.saveImage((image_name+"_TemplateMatching"), img_display);
    WriteTextFile wrtxtf;
    Size bbox_sz = Size(matchLoc.x + templ.cols, matchLoc.y + templ.rows);
    wrtxtf.BoundingBoxDim(bbox_sz.width, bbox_sz.height);

    //imshow("ResultWindow", result);
    //imshow("ImageWindow", img_display);
    return;
}