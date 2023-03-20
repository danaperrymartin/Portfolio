#include "DisplayImage.h"
#include "Reshape.h"
#include "BinFile.h"

#include <opencv2/core.hpp>

// VS Libraries
#include <iostream>

using namespace cv;

class LoadImage
{
    public:
        pair<unsigned char*, int64> LoadBinImage(std::string filename)
        {
            Mat binimg;

            BinFile binfile;

            //load bin file
            int num_imgs = 65535/2048;
            const int bufferwidth = 2048 * num_imgs;
            const int bufferheight = 3072*10;

            auto ret = binfile.LoadBinFile(filename.c_str());
            unsigned char* buffer_local = ret.first;
            int64 FileSize = ret.second;
            
            DisplayImage dspimg;
            Reshape rsp;

            /*img = cv::imdecode((uint8_t) *buffer, -1);*/

            std::vector<vector<double>> mat_reshaped(bufferwidth, vector<double>(bufferheight, 0));
            cv::Mat img_mat(mat_reshaped.size(), mat_reshaped.at(0).size(), CV_64FC1);

            int numimages = 1;
            //endoffile= FileSize / 2048 - bufferwidth;
            rsp.ReshapeMatrix(numimages, (unsigned char*)buffer_local, bufferwidth, bufferheight, mat_reshaped, img_mat, 0, false, FileSize);

            // Create a Size(1, nSize) Mat object of 8-bit, single-byte elements
            //Mat rawData(1, sizeof(buffer), CV_8UC1, (void*)buffer);
            //img = cv::imdecode((uint8_t)*buffer, -1);

            int down_width = 1920;
            int down_height = 4120;
            Mat resized_down;
            cv::resize(img_mat, resized_down, Size(down_width, down_height), cv::INTER_LINEAR);

            dspimg.ShowImage(resized_down, num_imgs);

           /* memcpy(buffer, buffer_local, sizeof(buffer_local));
            free(buffer_local);*/
            
            return {buffer_local, FileSize};
        }

        double CreateTrainTimeVector(const char* rawbinfile, int64 lSize)
        {
            auto ret = GetStartTime(rawbinfile);
            time_t starttime = ret.first;
            double_t starttime_dbl = ret.second;

            //float delta_time = 6.66666666667 * pow(10, -5); // 1 / 15000;
            //std::vector<float> timevector(lSize);
            //std::generate(timevector.begin(), timevector.end(), [n = 0, &delta_time, &starttime]() mutable { return ((n++ * delta_time) + starttime); });

            return starttime_dbl;
        }

        pair<time_t, double> GetStartTime(const char* rawbinfile)
        {
            std::string s = rawbinfile;
            std::string delimiter = "_";

            std::vector<std::string> parsed_string = ParseString(s, delimiter);

            std::tm dt;
            std::string joiner="/";
            std::string joiner2 = " ";
            std::string joiner3 = ":";
            std::string joiner4 = ".";
            
            std::string starttime = parsed_string[1] + joiner + parsed_string[2] + joiner + parsed_string[3] + joiner2 + parsed_string[4] + joiner3 + parsed_string[5] + joiner3 + parsed_string[6] + joiner4 + parsed_string[7];
            double starttime_db = (std::stod(parsed_string[1]) * (double)31540000000) + (std::stod(parsed_string[2]) * (double)2628000000) + (std::stod(parsed_string[3])* (double)86400)+ (std::stod(parsed_string[4])* (double)3600)+ (std::stod(parsed_string[5])*(double)60) + std::stod(parsed_string[6])+ std::stod(parsed_string[7]) / ((double)1000);

            std::istringstream ss(starttime);
            ss >> std::get_time(&dt, "%Y/%m/%d %H:%M:%S");
            if (ss.fail()) {
                throw std::runtime_error{ "failed to parse time string" };
            }
            std::time_t time_stamp = mktime(&dt);

            return { time_stamp, starttime_db };
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
};