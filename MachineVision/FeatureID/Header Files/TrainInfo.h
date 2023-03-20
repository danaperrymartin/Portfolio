#include <string>
#include <vector>
#include <filesystem>

#include <chrono>
#include <iostream>
#include <ctime>

#include <time.h>
#include <stdio.h>
#include <string>
#include <filesystem>
#include <chrono>

namespace fs = std::filesystem;

using namespace std;
using namespace std::filesystem;
using namespace std::chrono;
using namespace std::chrono_literals;


class TrainInfo
{
	public:
        pair<list<std::string>, list<std::wstring>> GetWheelFiles(std::string directory)
        {
            cout << "Finding .whl files..." << std::endl;
            std::string path(directory);
            std::string ext(".txt");
            list<std::string> whl_files;
            list<std::wstring> whl_files_timestamp;
            for (auto& p : fs::recursive_directory_iterator(path))
            {
                if (p.path().extension() == ext)
                    //std::cout << p.path().filename() << '\n';
                    whl_files.push_back(std::wstring_convert<std::codecvt_utf8<wchar_t>>().to_bytes(p.path().filename()));
                    //whl_files_timestamp.push_back((std::filesystem::last_write_time(p.path())));
            }
            return { whl_files, whl_files_timestamp };
        }

        std::vector<std::string> GetMatchingBinFiles(std::string whl_file)
        {
            cout << ("Determining corresponding .bin files for..."+whl_file) << std::endl;
            std::string delimiter = "\\";
            std::vector<std::string> parsed_string = ParseString(whl_file, delimiter);
            std::string delimiter2 = "_";
            std::vector<std::string> parsed_string2 = ParseString(parsed_string[2], delimiter2);
            std::string delimiter3 = " ";
            std::vector<std::string> parsed_string3 = ParseString(parsed_string2[3], delimiter3);
            
            std::vector<std::string> bin_files;

            bin_files.push_back((parsed_string2[0] + delimiter2 + parsed_string2[1] + delimiter2 + parsed_string2[2] + delimiter2 + parsed_string3[0] + delimiter2 + parsed_string3[1] + delimiter2 + parsed_string2[4] + delimiter2 + parsed_string2[5] + delimiter2 + parsed_string2[6] + delimiter2 + (std::string)"FarSide_2048x3072.bin"));
            bin_files.push_back((parsed_string2[0] + delimiter2 + parsed_string2[1] + delimiter2 + parsed_string2[2] + delimiter2 + parsed_string3[0] + delimiter2 + parsed_string3[1] + delimiter2 + parsed_string2[4] + delimiter2 + parsed_string2[5] + delimiter2 + parsed_string2[6] + delimiter2 + (std::string)"NearSide_2048x3072.bin"));

            return bin_files;
        }

        std::string GettrnFile(std::string whl_file_path,std::string whl_file)
        {
            cout << "Finding corresponding .trn file..." << std::endl;
            std::string delimiter = "_";
            std::vector<std::string> parsed_string = ParseString(whl_file, delimiter);
            std::string drive = "L:";
            std::string sep = "\\";
            std::string trn_directory = drive + sep + parsed_string[0] + sep + parsed_string[1] + sep + parsed_string[2] + sep + parsed_string[3];
            
            std::string path(trn_directory);
            std::string ext(".trn");
            std::filesystem::path trn_file;
            std::vector<pair<std::filesystem::_File_time_clock::duration, int>> trn_writetime_delta;
            std::vector<std::filesystem::path> trn_files;
            std::filesystem::_File_time_clock::duration delta_time;
            std::vector<double> dt;

            std::string whl_file_abs = (whl_file_path + whl_file);
            int i = 0;
            for (auto& p : fs::recursive_directory_iterator(path))
            {
                if (p.path().extension() == ext)
                {
                    std::string delimiter = "_";
                    std::vector<std::string> parsed_string = ParseString(std::wstring_convert<std::codecvt_utf8<wchar_t>>().to_bytes(p.path().filename()), delimiter);
                    std::string delimiter2 = " ";
                    std::vector<std::string> parsed_string2 = ParseString(parsed_string[4], delimiter2);
                    std::vector<std::string> parsed_string3 = ParseString(parsed_string[6], delimiter2);

                    std::string joiner = "/";
                    std::string joiner2 = " ";
                    std::string joiner3 = ":";
                    std::string joiner4 = ".";

                    std::string trn_starttime = (parsed_string[2] + joiner + parsed_string[3] + joiner + parsed_string2[0] + joiner2 + parsed_string2[1] + joiner3 + parsed_string[5] + joiner3 + parsed_string3[0]);
                    std::tm tmb{};
                    std::istringstream(trn_starttime) >> std::get_time(&tmb, "%Y/%m/%d %H:%m%S");
                    auto t1 = std::mktime(&tmb);
                    using namespace std::chrono;
                                       
                    //std::cout << std::format("File write time is {}\n", (std::filesystem::last_write_time(p.path()) - std::filesystem::last_write_time(whl_file_abs)));
                    
                    auto ftime = std::filesystem::last_write_time(p.path());
                    delta_time = (std::filesystem::last_write_time(p.path()) - std::filesystem::last_write_time(whl_file_abs));
                    trn_writetime_delta.push_back(make_pair(abs(delta_time), i));
                    trn_files.push_back(p.path());

                    i++;
                }
            }
            sort(trn_writetime_delta.begin(), trn_writetime_delta.end());
            trn_file = trn_files[trn_writetime_delta[0].second];
            return trn_file.string();
        }

        pair<vector<double_t>, vector<double_t>>  ParseWheelDetect(std::string filename)
		{
            std::string s = filename;
            
            std::ifstream t(filename);
            std::stringstream buffer;
            buffer << t.rdbuf();
            
            std::string delimiter = "\n";
            std::vector<std::string> parsed_string = ParseString(buffer.str(), delimiter);
            
            //std::vector<std::string> parsed_string2;
            std::string delimiter2 = "\t";
            std::vector<double_t> wheel_info;
            std::vector<double_t> coupler_info;

            for (int iline = 1; iline < parsed_string.size(); iline++)
            {
                std::vector<std::string> tmp = ParseString(parsed_string[iline], delimiter2);
                
                std::tm dt_coupler;
                std::tm dt_wheel;
                std::istringstream ss_coupler(tmp[1]);
                std::istringstream ss_wheel(tmp[2]);

                ss_coupler >> std::get_time(&dt_coupler, "%m/%d/%Y %H:%M:%S.%06d");
                ss_wheel >> std::get_time(&dt_wheel, "%m/%d/%Y %H:%M:%S.%06d");

               /* if (ss_coupler.fail() | ss_wheel.fail()) 
                {
                    throw std::runtime_error{ "failed to parse time string" };
                }*/
                
                if (ss_coupler.str().find_first_of('-', 0) != std::string::npos)
                {
                    /*coupler_info.push_back('0');*/
                }
                else
                {
                    std::vector<std::string> parsed_string1 = ParseString(tmp[1], "/");
                    std::vector<std::string> parsed_string2 = ParseString(parsed_string1.back(), " ");
                    std::vector<std::string> parsed_string3 = ParseString(parsed_string2.back(), ":");
                    std::vector<std::string> parsed_string4 = ParseString(parsed_string3.back(), ".");

                    std::string::size_type sz;     // alias of size_t
                    double_t coupler_time = ((std::stod(parsed_string2[0], &sz) + (double_t)2000) * 31540000000) + (std::stod(parsed_string1[0], &sz) * 2628000000) + (std::stod(parsed_string1[1], &sz) * 86400) + (std::stod(parsed_string3[0], &sz) * 3600) + (std::stod(parsed_string3[1], &sz) * 60) + std::stod(parsed_string4[0], &sz) + std::stod(parsed_string4[1], &sz) / ((double)1000);

                    std::time_t couplertime_stamp = _mktime64(&dt_coupler);
                    //coupler_info.push_back(static_cast<std::double_t>(couplertime_stamp));
                    coupler_info.push_back(coupler_time);
                }
                
                if (ss_wheel.str().find_first_of('-', 0) != std::string::npos)
                {
                   /* wheel_info.push_back('0');*/
                }
                else
                {
                    std::vector<std::string> parsed_string1 = ParseString(tmp[2], "/");
                    std::vector<std::string> parsed_string2 = ParseString(parsed_string1.back(), " ");
                    std::vector<std::string> parsed_string3 = ParseString(parsed_string2.back(), ":");
                    std::vector<std::string> parsed_string4 = ParseString(parsed_string3.back(), ".");

                    std::string::size_type sz;     // alias of size_t
                    double_t wheel_time = ((std::stod(parsed_string2[0], &sz) + (double_t)2000)*31540000000) + (std::stod(parsed_string1[0], &sz)*2628000000) + (std::stod(parsed_string1[1], &sz)* 86400)+ (std::stod(parsed_string3[0], &sz)* 3600)+ (std::stod(parsed_string3[1], &sz)*60) + std::stod(parsed_string4[0], &sz) + std::stod(parsed_string4[1], &sz) / ((double)1000);

                    std::time_t wheeltime_stamp = _mktime64(&dt_wheel);
                    //wheel_info.push_back(static_cast<std::double_t>(wheeltime_stamp));
                    wheel_info.push_back(wheel_time);
                }
            }
            return { coupler_info, wheel_info };
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

        struct Car_Info
        {
            std::string Car_End;
            std::string Car_Owner;
            std::string Car_Number;
            std::string Car_Type;
            std::string Car_Axles;
        };

        struct Crib_Info
        {
            std::string Track_Type;
            int Crib_Number;
            std::string Crib_Location;
            std::string Crib_Type;

            std::vector<std::string> WheelForce_PeakTime;
            std::vector<std::string> WheelForce_Vert;
            std::vector<std::string> WheelForce_Lat;
            std::vector<std::string> WheelForce_Static;
        };

        std::vector<TrainInfo::Car_Info>  Parsetrn_AEI(const char* filename)
        {
            std::vector<TrainInfo::Car_Info> Cars;

            std::string s = filename;

            std::ifstream t(filename);
            std::stringstream buffer;
            buffer << t.rdbuf();

            std::string delimiter = "\n";
            std::vector<std::string> parsed_string = ParseString(buffer.str(), delimiter);

            std::string delimiter2 = "\t";
            std::vector<double_t> wheel_info;
            std::vector<double_t> coupler_info;
            std::string AEI_line = "[Tags Read]";
            for (int iline = 1; iline < parsed_string.size(); iline++)
            {
                //iline = 15044;
                if (parsed_string[iline].compare(AEI_line)==0)
                {
                    iline = iline + 3;
                    TrainInfo::Car_Info current_train;
                    std::string AEI_endline = ("Number of Axles in Train");
                    while(parsed_string[iline].find(AEI_endline) != 0)
                    {
                        std::vector<std::string> tmp = ParseString(parsed_string[iline], delimiter2);

                        current_train.Car_End = tmp[1];
                        current_train.Car_Owner = tmp[2];
                        current_train.Car_Number = tmp[3];
                        current_train.Car_Type = tmp[4];
                        current_train.Car_Axles = tmp[5];

                        Cars.push_back(current_train);
                        
                        iline++;
                    }
                    break;
                }
            }

            return Cars;
        }

        std::vector<TrainInfo::Crib_Info> Parsetrn_StrainGuage(const char* filename)
        {
            std::vector<TrainInfo::Crib_Info> Car_Forces;

            std::string s = filename;

            std::ifstream t(filename);
            std::stringstream buffer;
            buffer << t.rdbuf();

            std::string delimiter = "\n";
            std::vector<std::string> parsed_string = ParseString(buffer.str(), delimiter);

            std::string delimiter2 = "\t";
            std::vector<double_t> wheel_info;
            std::vector<double_t> coupler_info;
            std::string PeakData_line = "[Peak Data]";
            int iline = 0;
            const char* PeakData_endline = ("[Tags Read]");
            while (iline < parsed_string.size())
            {
                if (parsed_string[iline].compare(PeakData_line) == 0)
                {
                    TrainInfo::Crib_Info current_train;

                    std::string delimiter = " ";
                    std::vector<std::string> tmp_string = ParseString(parsed_string[iline+1], delimiter);
                    current_train.Crib_Location = tmp_string[6];
                    current_train.Crib_Number = stoi(tmp_string[5]);
                    current_train.Track_Type = tmp_string[4];
                    current_train.Crib_Type = tmp_string[7];
                    //Car_Forces.push_back(current_train);
                    iline = iline + 4;

                    const char* Crib_endline = "Peak Axle Count: ";
                    int icrib = 0;
                    while (parsed_string[iline].compare(PeakData_endline) != 0)
                    {
                        std::string string_tmp = parsed_string[iline].substr(0, 17);
                        if (strcmp(string_tmp.c_str(), Crib_endline) == 0)
                        {
                            Car_Forces.push_back(current_train);
                            current_train = TrainInfo::Crib_Info();
                            std::string string_tmp = parsed_string[iline + 1].substr(0, 7);
                            if (strcmp(string_tmp.c_str(), "Board: ") == 0)
                            {
                                std::string delimiter = " ";
                                std::vector<std::string> tmp_string = ParseString(parsed_string[iline + 1], delimiter);
                                current_train.Crib_Location = tmp_string[6];
                                current_train.Crib_Number = stoi(tmp_string[5]);
                                current_train.Track_Type = tmp_string[4];
                                current_train.Crib_Type = tmp_string[7];

                                iline = iline + 4;
                            }
                            else
                            {
                                break;
                            }
                        }

                        std::vector<std::string> tmp = ParseString(parsed_string[iline], delimiter2);

                        current_train.WheelForce_PeakTime.push_back(tmp[1]);
                        current_train.WheelForce_Vert.push_back(tmp[2]);
                        current_train.WheelForce_Lat.push_back(tmp[3]);
                        current_train.WheelForce_Static.push_back(tmp[5]);

                        iline++;
                    }
                }
                iline++;
            }
            return Car_Forces;
        }
};