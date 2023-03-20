// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"

#include <stdio.h>
#include <codecvt>
#include <vector>
#include <filesystem>

using namespace std;

std::vector<std::filesystem::path> trn_files;
int parsedstringlength = 0;

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

extern "C"
{
	__declspec(dllexport) int add(int a, int b)
	{
		return a + b;
	}
	__declspec(dllexport) int subtract(int a, int b)
	{
		return a - b;
	}

    __declspec(dllexport) void GettrnFiles(const char* site, const char* start_date, const char* end_date, const char* sourcedrive)
    {
        std::string month_string = { "" };
        std::string day_string = { "" };
        //std::vector<std::filesystem::path> trn_files;

        //cout << "Finding .trn files..." << std::endl;
        std::string delimiter = "/";
        std::vector<std::string> startdate_parsed = ParseString(start_date, delimiter);
        std::string drive = sourcedrive;
        std::string sep = "\\";

        std::string delimiter2 = "/";
        std::vector<std::string> enddate_parsed = ParseString(end_date, delimiter);
        std::string sep2 = "\\";

        double iyear = stod(startdate_parsed[2]);
        double iday = stod(startdate_parsed[1]);
        double imonth = stod(startdate_parsed[0]);
        double daymax = 30;
        double endyear = stod(enddate_parsed[2]);
        double endmonth = stod(enddate_parsed[0]);

        while (iyear <= endyear)
        {
            while ((imonth <= endmonth) || (iyear < endyear))
            {
                double logic_a = remainder(imonth, 2.0);
                if ((logic_a == 0.0) && (imonth / 2 != 1.0))
                {
                    daymax = 30;
                }
                else if (logic_a == 0.0 && imonth / 2 == 1.0)
                {
                    daymax = 28;
                }
                else
                {
                    daymax = 31;
                }

                while (((int)iday <= std::stoi(enddate_parsed[1])) || ((imonth <= endmonth) && (iyear <= endyear)))
                {
                    size_t sz_month = to_string((int)imonth).size();
                    size_t sz_day = to_string((int)iday).size();

                    if (sz_month == 1)
                    {
                        month_string = "0" + to_string((int)imonth);
                    }
                    else
                    {
                        month_string = to_string((int)imonth);
                    }
                    if (sz_day == 1)
                    {
                        day_string = "0" + to_string((int)iday);
                    }
                    else
                    {
                        day_string = to_string((int)iday);
                    }

                    std::string trn_directory = drive + sep + site + sep + to_string((int)iyear) + sep + month_string + sep + day_string;

                    std::string path(trn_directory);
                    std::string ext(".trn");

                    int i = 0;
                    std::filesystem::file_status s = std::filesystem::file_status{};
                    if (std::filesystem::status_known(s) ? std::filesystem::exists(path) : std::filesystem::exists(path))
                    {
                        for (const std::filesystem::directory_entry& dir_entry : std::filesystem::recursive_directory_iterator(path))
                        {
                            if (!dir_entry.is_directory())
                            {
                                /*delimiter = "_";
                                std::vector<std::string> parsed_string = ParseString(std::wstring_convert<std::codecvt_utf8<wchar_t>>().to_bytes(dir_entry.path().filename()), delimiter);
                                delimiter2 = " ";
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
                                using namespace std::chrono;*/

                                trn_files.push_back(dir_entry.path());
                            }
                        }
                    }
                    if ((int)iday >= (int)daymax)
                    {
                        break;
                    }
                    else
                    {
                        iday++;
                    }
                } // end iday
                iday = 1;
                imonth++;
                if (imonth > 12)
                {
                    break;
                }
            } // end imonth
            imonth = 1;
            iyear++;
        } // end iyear
    };

    __declspec(dllexport) void CopytrnFiles2Archive(const char* newdrive)
    {
        int ifile = 0;
        while (ifile < trn_files.size())
        {
            std::string delimiter = "\\";
            std::vector<std::string> parsed_string = ParseString(std::wstring_convert<std::codecvt_utf8<wchar_t>>().to_bytes(trn_files[ifile]), delimiter);
            parsedstringlength = static_cast<int>(parsed_string.size());

            std::filesystem::path new_pathwfile = newdrive;
            int i = 1;
            while (i < parsedstringlength)
            {
                new_pathwfile += delimiter + parsed_string[i];// +delimiter + parsed_string[2] + delimiter + parsed_string[3] + delimiter + parsed_string[4] + delimiter + parsed_string[5] + delimiter + parsed_string[6] + delimiter + parsed_string[7]);
                i++;
            }

            std::filesystem::path new_path = newdrive;
            i = 1;
            while (i < (parsedstringlength-1))
            {
                new_path += delimiter + parsed_string[i];// (newdrive + delimiter + parsed_string[1] + delimiter + parsed_string[2] + delimiter + parsed_string[3] + delimiter + parsed_string[4] + delimiter + parsed_string[5]);
                i++;
            }
            
            
            if (!std::filesystem::exists(new_path))
            {
                std::filesystem::create_directories(new_path);
            }

            if (!std::filesystem::exists(new_pathwfile))
            {
                std::filesystem::copy(trn_files[ifile], new_pathwfile);
            }
            ifile++;
        }
    };

    __declspec(dllexport) void RemoveFilesFromStorage(std::vector<std::filesystem::path> trn_filenames, std::string newdrive)
    {
        int ifile = 0;
        while (ifile < trn_filenames.size())
        {
            std::string delimiter = "\\";
            std::vector<std::string> parsed_string = ParseString(std::wstring_convert<std::codecvt_utf8<wchar_t>>().to_bytes(trn_filenames[ifile]), delimiter);


            std::filesystem::path new_pathwfile = (newdrive + delimiter + parsed_string[1] + delimiter + parsed_string[2] + delimiter + parsed_string[3] + delimiter + parsed_string[4] + delimiter + parsed_string[5] + delimiter + parsed_string[6]);

            if (std::filesystem::exists(new_pathwfile) && (std::filesystem::file_size(new_pathwfile) == std::filesystem::file_size(trn_filenames[ifile])))
            {
                std::filesystem::remove(trn_filenames[ifile]);
            }
            ifile++;
        }
    };
}
