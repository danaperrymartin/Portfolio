// HuntingID.cpp : This file contains the 'main' function. Program execution begins and ends there.
//
#include "HeaderFiles/TrainInfo.h"
#include "HeaderFiles/Interpolation.h"
#include "HeaderFiles/FastFourierTransform.h"
#include "HeaderFiles/Regression.h"

#include <chrono>
#include <thread>

const int upsamp = 100;
size_t cribs = 15;
std::vector<std::filesystem::path> trn_filenames;

void processTrain(std::string trainfile);
std::vector<std::filesystem::path> GettrnFiles(std::string site, std::string start_date, std::string end_date);

int main()
{
    //----------------Parse trn information----------------------------------------------
    std::string site = "Gage";
    std::string start_date = "10/22/2022";
    std::string end_date = "10/22/2022";
    
    trn_filenames = GettrnFiles(site, start_date, end_date);
    
    int ifile = 0;
    while (ifile < trn_filenames.size())
    {
        processTrain(trn_filenames[ifile].string());
        ifile++;
    }

    return 1;
};

void processTrain(std::string trainfile)
{
    TrainInfo trn;
    
    trn.cribs = cribs;
    std::cout << trainfile << std::endl;

    // Parse AEI information
    trn.Parsetrn_AEI(trainfile.c_str());
    if (trn.Cars.size() == 1)
    {
        return;
        std::cout << "No AEI information..." << std::endl;
        trn.Cars.clear();
    }

    /*if ((strcmp((Cars[0].Car_Owner).c_str(), "AMTK") == 0) || (strcmp((Cars[0].Car_Owner).c_str(), "IDTX") == 0))
    {
        ifile++;
        continue;
    }*/

    /////*------------Parse trn file---------------------*////
    trn.Parsetrn_StrainGuage_m1(trainfile.c_str());
    // ----------- Parsing Method for sites with isolated sattellites ------------ //
    //std::vector<TrainInfo::Crib_Info> Trn_Forces = trn.Parsetrn_StrainGuage_m2((trn_filenames[ifile].string()).c_str());

    /////*------------Reshape Data---------------------*////
    trn.Reshape4Car(trainfile.c_str());

    trn.Reshape4Wheel();

    std::vector<std::string> name = trn.ParseString((trainfile).substr(0, 56), "\\");
    std::vector<std::string> subname = trn.ParseString(name[6], "_");
    std::filesystem::path directory_name = ("C:\\CodeProjects\\HuntingID\\HuntingData\\ExperimentalDFFT\\" + (name[6] + "\\"));
    std::string name_nospace = directory_name.string();
    name_nospace.erase(std::remove_if(name_nospace.begin(), name_nospace.end(), ::isspace), name_nospace.end());
    directory_name = name_nospace;

    double* _huntingmetric = new double[trn.Cars.size()]{ 0.0 };

    int icar = 0;
    for (int iwheel = 0; iwheel < trn.Wheel_Forces_Near.size(); iwheel++)
    {
        InterpolationHandle interp(cribs, upsamp);  // Re-initialize interpolation class for each wheel at size of valid cribs
        FastFourierTransformHandle fft(cribs * upsamp);

        cribs = trn.Wheel_Forces_Near[iwheel].size();
        
        //-------------Sinusoidal Regression -------------------//
        //float phase(0);
        //float mag(0);
        //float shift(0);
        //reg.SinusoidalRegression(dataz, phase, mag, shift);
        //-------------Compute Discrete Fast Fourier Transform -------------------//
        if (trn.Wheel_Forces_Near[iwheel].size() > (int)(trn.Wheel_Forces_Near[iwheel].size() * 0.70))
        {
            //cout << to_string(iwheel);
            interp->MKLInterpolation(trn.Wheel_Forces_Near_Array[iwheel].WheelForce_PeakTime, trn.Wheel_Forces_Near_Array[iwheel].WheelForce_NormLat, directory_name, trn.Trn_Whl[iwheel]);
            std::vector<std::complex<float>> fft_out = std::vector<std::complex<float>>(cribs * upsamp);
            fft->ComputeFastFourierTransform(interp->r, directory_name, trn.Trn_Whl[iwheel]);
            // Sum up magnitudes associated with frequencies between 2Hz and 4 Hz for a car
            if (iwheel > 0)
            {
                if (trn.ParseString(trn.Trn_Whl[iwheel], "_")[0].compare(trn.ParseString(trn.Trn_Whl[iwheel - 1], "_")[0]) == 0)
                {
                    _huntingmetric[icar] += fft->huntingmetric;
                }
                else
                {
                    _huntingmetric[icar] = _huntingmetric[icar] / trn.Cars[icar].Car_Axles;
                    icar++;
                }
            }
        }
        
        //directory_name = ("C:\\CodeProjects\\HuntingID\\HuntingData\\" + (subname[1] + name[1] + name[2] + name[3] + name[4]) + "_" + "\\");
        //if (!std::filesystem::exists(directory_name))
        //{
        //   std::filesystem::create_directory(directory_name);
        //}
    }

    ofstream huntingfile((directory_name.string() + name[6]+".hnt"));
    for (int count = 0; count < trn.Cars.size(); count++)
    {
        if (count == 0)
        {
            huntingfile << ((string)"Car" + "\t" + (string)"HuntingMetric") << "\n";
        }
        huntingfile << ((string)(trn.Trn_Car[count]) + "\t" + to_string(_huntingmetric[count])) << "\n";
    }
    huntingfile.close();

    delete[] _huntingmetric;
    name.clear();
    subname.clear();
    directory_name.clear();
};

std::vector<std::filesystem::path> GettrnFiles(std::string site, std::string start_date, std::string end_date)
{
    TrainInfo trn;
    std::string month_string = { "" };
    std::string day_string = { "" };
    std::vector<std::filesystem::path> trn_files;

    cout << "Finding .trn files..." << std::endl;
    std::string delimiter = "/";
    std::vector<std::string> startdate_parsed = trn.ParseString(start_date, delimiter);
    std::string drive = "L:";
    std::string sep = "\\";

    std::string delimiter2 = "/";
    std::vector<std::string> enddate_parsed = trn.ParseString(end_date, delimiter);
    std::string sep2 = "\\";

    int iyear = stoi(startdate_parsed[2]);
    int iday = stoi(startdate_parsed[1]);
    int imonth = stoi(startdate_parsed[0]);
    int daymax = 30;
    int endyear = std::stoi(enddate_parsed[2]);
    int endmonth = stoi(enddate_parsed[0]);

    while (iyear <= endyear)
    {
        while (imonth <= endmonth)
        {
            if (imonth % 2 != 0 && imonth / 2 != 1)
            {
                daymax = 30;
            }
            else if (imonth % 2 != 0 && imonth / 2 == 1)
            {
                daymax = 28;
            }
            else
            {
                daymax = 31;
            }

            while (iday <= stoi(enddate_parsed[1]))
            {
                size_t sz_month = to_string(imonth).size();
                size_t sz_day = to_string(iday).size();

                if (sz_month == 1)
                {
                    month_string = "0" + to_string(imonth);
                }
                else
                {
                    month_string = to_string(imonth);
                }
                if (sz_day == 1)
                {
                    day_string = "0" + to_string(iday);
                }
                else
                {
                    day_string = to_string(iday);
                }

                std::string trn_directory = drive + sep + site + sep + to_string(iyear) + sep + month_string + sep + day_string;

                std::string path(trn_directory);
                std::string ext(".trn");

                int i = 0;
                for (auto& p : fs::recursive_directory_iterator(path))
                {
                    if ((p.path().extension() == ext) && ((p.path().string()).find("Error")) == string::npos)
                    {
                        delimiter = "_";
                        std::vector<std::string> parsed_string = trn.ParseString(std::wstring_convert<std::codecvt_utf8<wchar_t>>().to_bytes(p.path().filename()), delimiter);
                        delimiter2 = " ";
                        std::vector<std::string> parsed_string2 = trn.ParseString(parsed_string[4], delimiter2);
                        std::vector<std::string> parsed_string3 = trn.ParseString(parsed_string[6], delimiter2);

                        std::string joiner = "/";
                        std::string joiner2 = " ";
                        std::string joiner3 = ":";
                        std::string joiner4 = ".";

                        std::string trn_starttime = (parsed_string[2] + joiner + parsed_string[3] + joiner + parsed_string2[0] + joiner2 + parsed_string2[1] + joiner3 + parsed_string[5] + joiner3 + parsed_string3[0]);
                        std::tm tmb{};
                        std::istringstream(trn_starttime) >> std::get_time(&tmb, "%Y/%m/%d %H:%m%S");
                        auto t1 = std::mktime(&tmb);
                        using namespace std::chrono;

                        trn_files.push_back(p.path());
                    }
                }
                if (iday > daymax)
                {
                    iday = 1;
                }
                else
                {
                    iday++;
                }
            } // end iday
            imonth++;
        } // end imonth
        iyear++;
    } // end iyear

    return trn_files;
};