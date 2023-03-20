// External Classes
#include "Sort.h"
#include "OnnxModelScorer.h"
#include "NonMaximalSuppression.h"
#include "PlotFeatures.h"
#include "WriteTextFile.h"
#include "ImageSegmentation.h"

// VS Libraries
#include <iostream>
#include <list>

class ImageProcessing
{
    public:
        int ProcessImage()
        {
            char filename[36] = "TestImages/FarSideOutput.bmp";
            char searchtype[2] = "f";
            bool highest_prob = 1;
            double minVal;
            double maxVal = 0.0;
            Point minLoc;
            Point maxLoc;
            std::vector<cv::Rect*> bboxloc_afteriou;
            std::vector<string> bboxlabel_afteriou;
            std::vector<vector<double>> conf;
            std::vector<Rect*> bboxloc;
            std::vector<Rect> rects;
            vector<cv::Point> mLoc;
            Mat result;
            std::vector<double*> glob_maxValue{ new double(0), new double(0), new double(0) };
            std::vector<int*> glob_maxLoc{ new int(0), new int(0), new int(0) };

            //cv::dnn::Net net = cv::dnn::readNetFromONNX("models/Trucks/resnet_v0.onnx");
            //std::vector<std::string> label{ "Spring", "Truck" };
            //std::vector<std::string> label{ "Background", "Truck" };

            // Initialize external class objects
            Sort sort;
            OnnxModelScorer modelscore;
            NonMaximalSuppression nonmax;
            PlotFeatures plot;
            WriteTextFile write;
            ImageSegmentation imgseg;

            // speed-up using multithreads
            setUseOptimized(true);
            setNumThreads(8);

            // read image
            Mat im = imread(filename);
            Mat img_orig = im.clone();

            int newHeight = 100;
            int newWidth = 204902 / 100;
            resize(im, im, Size(newWidth, newHeight)); // resize image

            //imgseg.SelectiveSearch(im, rects);         // Segment image using Selective Search method
            imgseg.Watershed(im, rects, newHeight, newWidth);               // Segment image using watershed method
            //imgseg.TextureSegmentation(im, rects);     // Segment image using texture based methods
            
            imgseg.KMeansClustering(im, rects);          // Segment image using texture based methods
            resize(im, im, Size(newWidth, newHeight));       // resize image
            imgseg.Watershed(im, rects, newHeight, newWidth);

            cv::dnn::Net net = cv::dnn::readNetFromONNX("models/Trucks/resnet_v0.onnx");
            std::vector<std::string> label{ "Background", "Truck" };
            ProcessSelectiveSearch(rects, label, im, conf, bboxloc, mLoc, net, newHeight, newWidth, maxVal, glob_maxValue, glob_maxLoc, img_orig, maxLoc, bboxloc_afteriou, bboxlabel_afteriou);

            return 0;
        }
        
        void ParseTrainImage(unsigned char* buffer, std::vector<double_t>& coupler_info, std::vector<double_t>& wheel_info, double train_start_time, int64 FileSize, std::vector<string> trn_car, std::filesystem::path savedirectory = "", std::string camerside = "")
        {
            sort(coupler_info.begin(), coupler_info.end());
            sort(wheel_info.begin(), wheel_info.end());
            std::vector<int> wheel_idx;

            double_t sample_rate= 20000;
            
            for (int iwheel = 0; iwheel <trn_car.size(); iwheel++)
            {
                double_t no_of_secs = wheel_info[iwheel] - train_start_time;
                if (no_of_secs > 0)
                {
                    wheel_idx.push_back((int)(no_of_secs*sample_rate));
                }
            }

            /*std::vector<int> match_index_2;
            match_index_2.reserve(wheel_info.size());
            for (auto it1 = wheel_info.begin(), it2 = train_time_vector.begin();
                it1 != wheel_info.end() && it2 != train_time_vector.end();
                ++it2) 
            {
                while (it1 != wheel_info.end() && *it1 < *it2)
                    ++it1;
                    if (it1 != wheel_info.end() && *it1 == *it2) 
                    {
                        match_index_2.push_back(it2 - train_time_vector.begin());
                    }
            }*/

            Reshape rsp;
            DisplayImage dspimg;
            int img_width = 2048;  // Average spacing between two rising edges = 1690 pixels
            int img_height = 1690;

            int numimages = wheel_idx.size();
            for (int iwheel = 0; iwheel < numimages; iwheel++)
            {
                std::vector<vector<double>> mat_reshaped(img_width, vector<double>(img_height, 0));
                cv::Mat img_mat(mat_reshaped.size(), mat_reshaped.at(0).size(), CV_64FC1);

                rsp.ReshapeMatrix(numimages, buffer, img_width, img_height, mat_reshaped, img_mat, wheel_idx[iwheel], true, FileSize, trn_car[iwheel], savedirectory, camerside);
            }
            
            bool doneflag = true;
        }

        void ProcessSelectiveSearch(std::vector<Rect> rects, std::vector<std::string> label, Mat im, std::vector<vector<double>> conf, std::vector<Rect*> bboxloc, vector<cv::Point> mLoc, cv::dnn::Net net, int newHeight, int newWidth, double maxVal, std::vector<double*> glob_maxValue, std::vector<int*> glob_maxLoc, Mat img_orig, Point maxLoc, std::vector<cv::Rect*> bboxloc_afteriou, std::vector<string> bboxlabel_afteriou)
        {
            Sort sort;
            OnnxModelScorer modelscore;
            NonMaximalSuppression nonmax;
            WriteTextFile write;
            PlotFeatures plot;

            std::vector<vector<int>> bboxnum(rects.size(), vector<int>(label.size(), 0));

            std::cout << "Total Number of Region Proposals: " << rects.size() << std::endl;

            bool doneflag = false;

            if (!doneflag)
            {
                Mat imOut = im.clone();                                                 // create a copy of original image

                modelscore.ModelScore(rects, conf, bboxloc, mLoc, net, imOut, newHeight, newWidth, maxLoc);

                sort.SortColumns(maxVal, conf, glob_maxValue, glob_maxLoc, label, bboxnum);

                double img_width = (double)newWidth * 5;
                double img_height = (double)newHeight * 5;

                cv::resize(img_orig, img_orig, Size(img_width, img_height));

                double bboxscalefactor_width = ((double)img_orig.size().width / (double)newWidth);
                double bboxscalefactor_height = ((double)img_orig.size().height / (double)newHeight);

                /*if (scalefactor_width < 1 || scalefactor_height < 1)
                {
                    if (scalefactor_width < 1)
                    {
                        scalefactor_width = 1;
                    }
                    if (scalefactor_height < 1)
                    {
                        scalefactor_height = 1;
                    }
                }*/

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
        }
};