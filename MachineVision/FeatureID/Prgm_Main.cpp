#include <list>
#include <string>
#include <codecvt>
#include <locale>
#include <filesystem>

#include "Header Files\LoadImage.h"
#include "Header Files\TrainInfo.h"
#include "Header Files\ImageProcessing.h"

// VS Libraries
//#include <iostream>


// VS Libraries
//#include <iostream>

//using namespace cv;

static void help() {
    std::cout << std::endl <<
        "Usage:" << std::endl <<
        "./ssearch input_image (f|q)" << std::endl <<
        "f=fast, q=quality" << std::endl <<
        "Use l to display less rects, m to display more rects, q to quit" << std::endl;
}

std::vector<bool> LengthNotEqual(std::vector<TrainInfo::Crib_Info> Trn_Info) 
{
    std::vector<int> vector_length;
    for (int icrib = 0; icrib < Trn_Info.size(); icrib++)
    {
        vector_length.push_back(Trn_Info[icrib].WheelForce_Lat.size());
    }

    sort(vector_length.begin(), vector_length.end());

    int tempCount = 1, mode = 1, lastNumber = vector_length[0];
    for (int index = 1; index < vector_length.size(); ++index)
    {
        if (lastNumber == vector_length[index])
            ++tempCount;
        else
        {
            lastNumber = vector_length[index];
            if (tempCount > mode)
                mode = tempCount;
            tempCount = 0;
        }
    }

    std::vector<bool> isnotmode;
    for (int icrib = 0; icrib < Trn_Info.size(); icrib++)
    {
        if (Trn_Info[icrib].WheelForce_Lat.size() == vector_length[mode])
        {
            isnotmode.push_back(false);
        }
        else
        {
            isnotmode.push_back(true);
        }
    }
    return isnotmode;
}


int main()
{
    // Parse wheel detect file
    TrainInfo trn;
    //const char* whl_directory = "E:\\Gage Vision Data\\Gage_2022_07_12 19_58_27_063_WheelDetect.txt";
    std::string whl_directory = "E:\\Gage Vision Data\\";
    auto out = trn.GetWheelFiles(whl_directory);
    auto whl_files = out.first;
    auto whl_files_wpath = out.second;
    std::for_each(whl_files.begin(), whl_files.end(), [&](const std::string& ifile)
    {
            auto ret2 = trn.ParseWheelDetect(whl_directory + ifile);
            std::vector<double_t> wheel_info;
            std::vector<double_t> coupler_info;
            coupler_info = ret2.first;
            wheel_info = ret2.second;

            //Load a binary image file
            LoadImage ldimg;
            vector<std::string> bin_files = trn.GetMatchingBinFiles(whl_directory + ifile);
            std::string savedirectory = ("C:\\CodeProjects\\MachineVision\\FeatureID\\ProcessedImages\\" + ifile.substr(0, 28));
            for (int ibinfile = 0; ibinfile < bin_files.size(); ibinfile++)
            {
                const std::string filename = (std::string)whl_directory + bin_files[ibinfile];
                std::string camerside = bin_files[ibinfile].substr(29, 7);

                auto ret = ldimg.LoadBinImage(filename);
                unsigned char* buffer = ret.first;
                int64 FileSize = ret.second;
                double train_start_time = ldimg.CreateTrainTimeVector(bin_files[ibinfile].c_str(), FileSize);

                //----------------Parse trn information----------------------------------------------
                std::string trn_filename = trn.GettrnFile(whl_directory, bin_files[ibinfile]);
                std::vector<TrainInfo::Car_Info>Cars = trn.Parsetrn_AEI(trn_filename.c_str());
                std::vector<TrainInfo::Crib_Info> Trn_Forces = trn.Parsetrn_StrainGuage(trn_filename.c_str());

                std::vector<bool> notequal = LengthNotEqual(Trn_Forces);

                std::vector<string> trn_car;

                int iaxle_total = 0;
                std::filesystem::path directory_name = ("C:\\CodeProjects\\MachineVision\\FeatureID\\HuntingData\\" + ifile.substr(0, 28) + "\\");
                std::filesystem::create_directory(directory_name);

                std::filesystem::path img_savedir = ("C:\\CodeProjects\\MachineVision\\FeatureID\\ProcessedImages\\" + ifile.substr(0, 28) + "\\");
                std::filesystem::create_directory(img_savedir);

                for (int icar = 0; icar < (int)Cars.size(); icar++)
                {
                    ofstream outfile;
                    outfile.open(directory_name.string() + Cars[icar].Car_Owner + Cars[icar].Car_Number + "_" + Cars[icar].Car_Axles + ".dat");

                    for (int iaxle = 0; iaxle < std::stoi(Cars[icar].Car_Axles); iaxle++)
                    {
                        for (int icrib = 0; icrib < Trn_Forces.size(); icrib++)
                        {
                            if (!notequal[icrib])
                            {
                                outfile << (Trn_Forces[icrib].WheelForce_Lat[iaxle_total] + "\n");
                            }
                        }
                        trn_car.push_back(Cars[icar].Car_Owner + Cars[icar].Car_Number + "_" + std::to_string(iaxle));
                        iaxle_total++;
                    }
                    outfile.close();
                }

                //---------------------------Call Python to generate time series plots--------
                //system(const char* "C:\Users\Dana\anaconda3\envs\SpyderPython\python.exe HuntingData//PlotHuntingData.py");

                //----------------------------Process Image---------------------------
                ImageProcessing imgprcs;
                // --------Parse .bin file into jpegs -------
                imgprcs.ParseTrainImage(buffer, coupler_info, wheel_info, train_start_time, FileSize, trn_car, img_savedir, camerside);
                free(buffer);
                // --------Process an Image-------
                int status = imgprcs.ProcessImage();
            }
    });
}