#include "DisplayImage.h"
#include "Reshape.h"
#include "BinFile.h"

// VS Libraries
#include <iostream>

using namespace cv;

class LoadImage
{
    public:
        void LoadBinImage(std::string filename, unsigned char* &buffer, FILE* &pFile, int64 &lSize)
        {
            Mat binimg;

            BinFile binfile;

            //load bin file
            binfile.LoadBinFile(filename.c_str(), buffer, pFile, lSize);
            //unsigned char* buffer_local = ret.first;
            //int64 FileSize = ret.second;

            //return {buffer_local, FileSize};
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