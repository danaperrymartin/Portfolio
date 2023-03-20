#pragma once
#include "Sort.h"
#include "Pooling.h"
#include "TextureHelperFunctions.h"

#include <opencv2/opencv.hpp>
#include <opencv2/core.hpp>
#include <opencv2/ximgproc/segmentation.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/imgproc/imgproc.hpp>
#include <opencv2/highgui.hpp>
#include <opencv2/core/utility.hpp>
#include <opencv2/imgcodecs.hpp>

#include <iostream>

using namespace std;
using namespace cv;
using namespace cv::ximgproc::segmentation;

class ImageSegmentation
{
public:
    void SelectiveSearch(Mat &im, std::vector<Rect>& rects)
    {
        Ptr<SelectiveSearchSegmentation> ss = createSelectiveSearchSegmentation(); // create Selective Search Segmentation Object using default parameters
        ss->setBaseImage(im);                                                      // set input image on which we will run segmentation
        ss->switchToSelectiveSearchFast();                                         // Switch to fast but low recall Selective Search method
        ss->process(rects);                                                        // run selective search segmentation on input image
    }
    void Watershed(Mat &im, std::vector<Rect>& rects, int newHeight, int newWidth)
    {
        Mat graymat;
        Mat opening;
        Mat sure_bg(newHeight, newWidth, CV_32F);
        Mat sure_fg(newHeight, newWidth, CV_32F);
        Mat dist_transform;
        Mat unknown(newHeight, newWidth, CV_32F);
        Mat mask;
        float kerneldata[9] = { 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        Mat kernelmatrix(3, 3, CV_32F, kerneldata);
        std::vector<int> poolxy = {8,9};
        
        double minVal;
        double maxVal = 0.0;
        Point minLoc;
        Point maxLoc;

        cvtColor(im, graymat, cv::COLOR_BGR2GRAY);
        //cvtColor(im, graymat, cv::COLOR_BGR2HSV);

        //GaussianBlur(graymat, graymat, Size(3,3),100);
        //blur(graymat, graymat, Size(9, 9));
        //medianBlur(graymat, graymat, 5);
        //filter2D(graymat, graymat, -1, 0.25*kernelmatrix);
        
        Pooling pool;
        Mat graymatout;
        int channels = 1;
        //pool.MaxPooling2D(graymat, graymat, poolxy, channels);
        //pool.AvePooling2D(graymat, graymat, poolxy, channels);
        //pool.MinPooling2D(graymat, graymat, poolxy, channels);

        cv::imshow("Output1", graymat);
        int x = waitKey();
        if (x == 99)
        {
        }
        else
        {
            cv::destroyAllWindows();
        }

        //threshold(graymat, graymat, 0, 255, THRESH_BINARY_INV + THRESH_OTSU);
        threshold(graymat, graymat, 127, 255, THRESH_BINARY);
        
        std::vector<int> kernel(3, 1);
        
        morphologyEx(graymat, opening, MORPH_OPEN,(int)1);
        dilate(graymat, sure_bg, kernelmatrix);
        distanceTransform(opening, dist_transform, cv::DIST_L2, (int)5);
        minMaxLoc(dist_transform, &minVal, &maxVal, &minLoc, &maxLoc);
        threshold(dist_transform, sure_fg, (double)0.09*(maxVal), 255, 0);
        for (int i = 0; i < sure_bg.rows; i++)
        {
            for (int j = 0; j < sure_bg.cols; j++)
            {
                unknown.at<float>(i,j) = sure_bg.at<float>(i,j) - sure_fg.at<float>(i,j);
            }
        }

        Mat markers(unknown.size(), CV_32S);
        
        sure_fg.convertTo(sure_fg, CV_8UC1);

        connectedComponents(sure_fg, markers);
        
        markers.setTo(Scalar(0,0,0), unknown == 0);
        
        watershed(im,markers);
        im.setTo(Scalar(0, 0, 255), markers == -1);
        
        Sort sort;

        markers.convertTo(markers, CV_32F);
        std::vector<float> unique_markers;
        std::vector<cv::Rect> watershedbb;
        Mat im_out;
        std::vector<std::vector<cv::Point>> contours;
        vector<Vec4i> hierarchy;
        Mat im_gray;
        
        threshold(im, im_out, 0, 250, THRESH_BINARY_INV);
        cvtColor(im_out, im_gray, COLOR_BGR2GRAY);
        cv::imshow("Output2", im_gray);
        int v = waitKey();
        if (v == 99)
        {
        }
        else
        {
            cv::destroyAllWindows();
        }

        findContours(im_gray, contours, hierarchy, RETR_TREE, CHAIN_APPROX_NONE);
        
        for (size_t idx = 0; idx < contours.size(); idx++) 
        {
            drawContours(im, contours, idx, Scalar(0,255,0));
        }

        cv::imshow("Output", im);
        int r = waitKey();
        if (r == 99)
        {
        }
        else
        {
            cv::destroyAllWindows();
        }

        std::vector<vector<Point> > contours_poly(contours.size());
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

        cv::imshow("Output", im);
        int u = waitKey();
        if (u == 99)
        {
        }
        else
        {
            cv::destroyAllWindows();
        }
        cv::imwrite("WatershedSegmentation.jpg", im);
    }
    void TextureSegmentation(Mat& im, std::vector<Rect>& rects)
    {
        Mat input = im;
        
        TextureHelperFunctions texture;
        float const TEXTURE_EDGE_THRESHOLD = 204;//0.8;

        /*texture.displayImage("Image", input);*/

        //Padd image
        Mat paddedImage;
        texture.symmetricPadding(input, paddedImage);
        /*texture.displayImage("Padded image", paddedImage);*/

        //Calculate entropy matrix
        Mat entropyMat = Mat::zeros(input.rows, input.cols, CV_32FC1);
        texture.entropyFilt(paddedImage, entropyMat);

        //Rescale entropy matrix
        normalize(entropyMat, entropyMat, 0, 1, NORM_MINMAX, -1, Mat());
        /*texture.displayImage("Normalized entropy", entropyMat);
        texture.saveImage("Normalized entropy", entropyMat);*/
        
        //Convert to [0, 255]]
        Mat convertedEntropyMat;
        texture.convertion(entropyMat, convertedEntropyMat);

        //Threshold textures
        threshold(convertedEntropyMat, convertedEntropyMat, TEXTURE_EDGE_THRESHOLD, 255/*1*/, THRESH_BINARY);
        /*texture.displayImage("Threshold Normalized entropy", convertedEntropyMat);
        texture.saveImage("Threshold Normalized entropy", convertedEntropyMat);*/

        //Filter areas
        Mat filteredEntropyMat = Mat::zeros(input.rows, input.cols, CV_8UC1);
        texture.bwareaopen(convertedEntropyMat, filteredEntropyMat);
        /*texture.displayImage("Filtered entropy", filteredEntropyMat);
        texture.saveImage("Filtered entropy", filteredEntropyMat);*/

        writebitmap("FilteredEntropy.bmp", (uchar*)filteredEntropyMat.data, im.size[0], im.size[1]);

        //Smooth edges and close open holes
        texture.imclose(filteredEntropyMat);
        /*texture.displayImage("Closed image", filteredEntropyMat);
        texture.saveImage("Closed image", filteredEntropyMat);*/
        
        //Fill holes
        texture.imfill(filteredEntropyMat);
        /*texture.displayImage("Filled image", filteredEntropyMat);
        texture.saveImage("Filled image", filteredEntropyMat);*/

        //Mask image
        Mat output;
        /*texture.maskImage(input, filteredEntropyMat, output);
        texture.displayImage("Output", output);*/
    }

    cv::Mat KMeansClustering(Mat& im, std::vector<Rect>& rects)
    {
        const unsigned int singleLineSize = im.rows * im.cols;
        const unsigned int K = 5;
        Mat data = im.reshape(1, singleLineSize);
        data.convertTo(data, CV_32F);
        std::vector<int> labels;
        cv::Mat1f colors;
        int MAX_ITERATIONS = 100;
        cv::kmeans(data, K, labels, cv::TermCriteria(cv::TermCriteria::EPS + cv::TermCriteria::COUNT, 10, 1.), MAX_ITERATIONS, cv::KMEANS_PP_CENTERS, colors);
        Mat outputImage = data.reshape(1, im.rows);
        outputImage.convertTo(outputImage, CV_32F);
        //cv::imshow("KMeansClustering", outputImage);
        
        return outputImage;
    }

    struct BmpHeader {
        char bitmapSignatureBytes[2] = { 'B', 'M' };
        uint32_t sizeOfBitmapFile = 54 + 786432;
        uint32_t reservedBytes = 0;
        uint32_t pixelDataOffset = 54;
    } bmpHeader;

    struct BmpInfoHeader {
        uint32_t sizeOfThisHeader = 40;
        int32_t width = 2048 / 3; // in pixels
        int32_t height = 2049024; // in pixels
        uint16_t numberOfColorPlanes = 1; // must be 1
        uint16_t colorDepth = 24;
        uint32_t compressionMethod = 0;
        uint32_t rawBitmapDataSize = 0; // generally ignored
        int32_t horizontalResolution = 3780 / 3; // in pixel per meter
        int32_t verticalResolution = 3780; // in pixel per meter
        uint32_t colorTableEntries = 0;
        uint32_t importantColors = 0;
    } bmpInfoHeader;

    struct Pixel_RGB {
        uint8_t blue = 255;
        uint8_t green = 255;
        uint8_t red = 0;
    } pixel_rgb;

    struct Pixel_Gray {
        uint8_t gray = 255;
    } pixel_gray;

    void writebitmap(std::string image_name, unsigned char* buffer, int datachunk, int img_height)
    {
        std::ofstream fout(image_name, ios::binary);
        fout.write((char*)&bmpHeader, 14);
        fout.write((char*)&bmpInfoHeader, 40);

        // writing pixel data
        size_t numberOfPixels = bmpInfoHeader.width * bmpInfoHeader.height;
        unsigned int kk = 0;

        for (int j = 0; j < img_height; j++)
        {
            fout.write((char*)&buffer[kk], 2048);
            kk = kk + 2048;
        }

        fout.close();
    }
};