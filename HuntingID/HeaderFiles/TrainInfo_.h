#include <filesystem>
#include <fstream>
#include <codecvt>
#include <iostream>

namespace fs = std::filesystem;

using namespace std;
using namespace std::filesystem;

class TrainInfo
{
    private:
        

	public:
        struct Car_Info
        {
            std::string Car_End;
            std::string Car_Owner;
            std::string Car_Number;
            std::string Car_Type;
            double Car_Axles;
        };

        struct Crib_Info
        {
            bool isnotmode;
            std::string Track_Type;
            int Crib_Number;
            std::string Crib_Location;
            std::string Crib_Type;

            std::vector<double> WheelForce_PeakTime;
            std::vector<double> WheelForce_Vert;
            std::vector<double> WheelForce_Lat;
            std::vector<double> WheelForce_Static;
        };

        struct Crib_InfoII
        {
            bool* isnotmode;
            std::string* Track_Type;
            int* Crib_Number;
            std::string* Crib_Location;
            std::string* Crib_Type;

            double* WheelForce_PeakTime[16];
            double* WheelForce_Vert[16];
            double* WheelForce_Lat[16];
            double* WheelForce_Static[16];
        };

        std::vector<std::string> Trn_Car;
        std::vector<std::string> Trn_Whl;
        std::vector<TrainInfo::Crib_Info> Trn_Forces;
        std::vector<TrainInfo::Car_Info>Cars;
        std::vector<TrainInfo::Crib_InfoII>Whl_Forces;

        std::vector<std::vector<TrainInfo::Crib_Info>> Wheel_Forces_Near;
        std::vector<std::vector<TrainInfo::Crib_Info>> Wheel_Forces_Far;
        std::vector<TrainInfo::Crib_Info> currenttrain_list;

        TrainInfo* trn;

        TrainInfo()
        {
            /*std::vector<TrainInfo::Crib_Info> Trn_Forces;
            std::vector<TrainInfo::Car_Info>Cars;
            std::vector<std::string> Trn_Car;
            std::vector<std::vector<TrainInfo::Crib_Info>> Wheel_Forces_Near;
            std::vector<std::vector<TrainInfo::Crib_Info>> Wheel_Forces_Far;
            std::vector<std::string> Trn_Whl;*/
        };

        ~TrainInfo()
        {
        };

        void SetClassInstance(TrainInfo* cin)
        {
            trn = cin;
        }

        TrainInfo* GetClassInstance()
        {
            return trn;
        }

        std::vector<std::filesystem::path> GettrnFiles(std::string site, std::string start_date, std::string end_date)
        {
            std::string month_string = { "" };
            std::string day_string = { "" };
            std::vector<std::filesystem::path> trn_files;

            cout << "Finding .trn files..." << std::endl;
            std::string delimiter = "/";
            std::vector<std::string> startdate_parsed = ParseString(start_date, delimiter);
            std::string drive = "L:";
            std::string sep = "\\";

            std::string delimiter2 = "/";
            std::vector<std::string> enddate_parsed = ParseString(end_date, delimiter);
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
                    if (imonth % 2 != 0 && imonth/2!=1)
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
                        int sz_month = to_string(imonth).size();
                        int sz_day   = to_string(iday).size();
                        
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
                                std::vector<std::string> parsed_string = ParseString(std::wstring_convert<std::codecvt_utf8<wchar_t>>().to_bytes(p.path().filename()), delimiter);
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
        }

        void Parsetrn_AEI(const char* filename)
        {
            std::size_t found = 0;
            std::string AEI_endline = ("Number of Axles in Train");
            std::vector<std::string> tmp = { "" };

            std::string s = filename;

            std::ifstream t(filename);
            std::stringstream buffer;
            buffer << t.rdbuf();

            std::string delimiter = "\n";
            std::vector<std::string> parsed_string = ParseString(buffer.str(), delimiter);

            std::string delimiter2 = "\t";
            std::string AEI_line = "[Tags Read]";
            for (int iline = 1; iline < parsed_string.size(); iline++)
            {
                found = parsed_string[iline].find(AEI_line);
                if (found != std::string::npos)
                {
                    iline = iterateline(parsed_string, iline, "---");
                    iline = iline + 1;
                    TrainInfo::Car_Info current_train;

                    while(parsed_string[iline].find(AEI_endline) != 0 && parsed_string[iline].find("[Channel Calibrations")!=0)
                    {
                        tmp = ParseString(parsed_string[iline], delimiter2);
                        if (tmp.size() != 1)
                        {
                            current_train.Car_End = tmp[1];
                            current_train.Car_Owner = tmp[2];
                            current_train.Car_Number = tmp[3];
                            current_train.Car_Type = tmp[4];
                            current_train.Car_Axles = stod(tmp[5]);

                            Cars.push_back(current_train);
                            tmp.clear();
                        }
                        
                        iline++;
                    }
                    if (parsed_string[iline].find("[Channel Calibrations") == 0)
                    {
                        current_train.Car_End = "IRSX";
                        current_train.Car_Owner = "NA";
                        current_train.Car_Number = "NA";
                        current_train.Car_Type = "NA";
                        current_train.Car_Axles = 0.0;

                        Cars.push_back(current_train);
                        tmp.clear();
                    }
                    break;
                }
            }

            //return Cars;
        }

        void Parsetrn_StrainGuage_m1(const char* filename)
        {
            std::size_t found = 0;
            std::vector<std::string> tmp_string = { "" };
            std::string string_tmp = {""};
            const char* Crib_endline = "Peak Axle Count: ";
            int icrib = 0;

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
                found = parsed_string[iline].find(PeakData_line);
                
                if(found != std::string::npos)
                {
                    TrainInfo::Crib_Info current_train;
                    
                    std::string delimiter = " ";
                    tmp_string = ParseString(parsed_string[iline+1], delimiter);
                    current_train.Crib_Location = tmp_string[6];
                    current_train.Crib_Number = stoi(tmp_string[5]);
                    current_train.Track_Type = tmp_string[4];
                    current_train.Crib_Type = tmp_string[7];

                    iline = iterateline(parsed_string, iline, "---");
                    iline = iline + 1;

                    icrib = 0;
                    while (parsed_string[iline].compare(PeakData_endline) != 0)
                    {
                        //cout << "iline" + to_string(iline);
                        //cout << parsed_string[iline];
                        if (strcmp((parsed_string[iline].substr(0, 17)).c_str(), Crib_endline) == 0)
                        {
                            Trn_Forces.push_back(current_train);
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

                                iline = iterateline(parsed_string, iline, "---");
                                iline = iline + 1;
                            }
                            else
                            {
                                break;
                            }
                        }

                        std::vector<std::string> tmp = ParseString(parsed_string[iline], delimiter2);
                        if (strcmp((parsed_string[iline].substr(0, 17)).c_str(), Crib_endline) != 0)
                        {
                            if (tmp.size() > 1)
                            {
                                current_train.WheelForce_PeakTime.push_back(convert2datetime(tmp[1]));
                                current_train.WheelForce_Vert.push_back(stod(tmp[2]));
                                current_train.WheelForce_Lat.push_back(stod(tmp[3]));
                                current_train.WheelForce_Static.push_back(stod(tmp[5]));
                            }
                            else
                            {
                                current_train.WheelForce_PeakTime.push_back(0.0);
                                current_train.WheelForce_Vert.push_back(0.0);
                                current_train.WheelForce_Lat.push_back(0.0);
                                current_train.WheelForce_Static.push_back(0.0);
                            }

                            iline++;
                        }
                    }
                }
                iline++;
            }
            //return Car_Forces;
        }

        std::vector<TrainInfo::Crib_Info> Parsetrn_StrainGuage_m2(const char* filename)
        {
            std::vector<TrainInfo::Crib_Info> Car_Forces;
            std::string delimiter = "\n";
            std::string string_tmp = "";
            std::vector<std::string> tmp_string = { "" };
            std::vector<std::string> tmp = { "" };
            std::string delimiter2 = "\t";
            std::vector<double_t> wheel_info;
            std::vector<double_t> coupler_info;
            std::string PeakData_line = "[Peak Data]";

            std::string s = filename;

            std::ifstream t(filename);
            std::stringstream buffer;
            buffer << t.rdbuf();

            std::vector<std::string> parsed_string = ParseString(buffer.str(), delimiter);
          
            int iline = 0;
            const char* PeakData_endline = ("[Tags Read]");
            while (iline < parsed_string.size())
            {
                std::size_t found = parsed_string[iline].find(PeakData_line);
                if (found != std::string::npos)
                {
                    TrainInfo::Crib_Info current_train;
                    iline = iline + 1;
                    
                    tmp_string = ParseString(parsed_string[iline + 1], delimiter);
                    current_train.Crib_Location = tmp_string[6];
                    current_train.Crib_Number = stoi(tmp_string[5]);
                    current_train.Track_Type = tmp_string[4];
                    current_train.Crib_Type = tmp_string[7];
                    iline = iline + 4;

                    const char* Crib_endline = "Peak Axle Count: ";
                    int icrib = 0;
                    while (parsed_string[iline].compare(PeakData_endline) != 0)
                    {
                        string_tmp = parsed_string[iline].substr(0, 17);
                        if (strcmp(string_tmp.c_str(), Crib_endline) == 0)
                        {
                            Car_Forces.push_back(current_train);
                            current_train = TrainInfo::Crib_Info();
                            string_tmp = parsed_string[iline + 1].substr(0, 7);
                            if (strcmp(string_tmp.c_str(), "Board: ") == 0)
                            {
                                tmp_string = ParseString(parsed_string[iline + 1], delimiter);
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

                        tmp = ParseString(parsed_string[iline], delimiter2);

                        if (tmp.size() > 1)
                        {
                            current_train.WheelForce_PeakTime.push_back(convert2datetime(tmp[1]));
                            current_train.WheelForce_Vert.push_back(stod(tmp[2]));
                            current_train.WheelForce_Lat.push_back(stod(tmp[3]));
                            current_train.WheelForce_Static.push_back(stod(tmp[5]));
                        }
                        else
                        {
                            current_train.WheelForce_PeakTime.push_back(0.0);
                            current_train.WheelForce_Vert.push_back(0.0);
                            current_train.WheelForce_Lat.push_back(0.0);
                            current_train.WheelForce_Static.push_back(0.0);
                        }

                        iline++;
                    }
                }
                iline++;
            }
            return Car_Forces;
        }

        void Reshape4Car(std::string ifile)
        {
            //std::vector<std::string> Trn_Car;
            LengthNotEqual();
            int iaxle_total = 0;
            std::vector<std::string> name = ParseString(ifile.substr(0, 56), "\\");
            std::vector<std::string> subname = ParseString(name[6], "_");
            std::vector<std::string> crib_side = { "Near", "Far" };

            std::string name_nospace = (name[6]);
            name_nospace.erase(std::remove_if(name_nospace.begin(), name_nospace.end(), ::isspace), name_nospace.end());

            std::string directory_name = ("C:\\CodeProjects\\HuntingID\\HuntingData\\" + name_nospace + "\\");
            //if (!std::filesystem::exists(directory_name))
            //{
            std::filesystem::create_directory(directory_name);
            //}

            for (int icar = 0; icar < Cars.size(); icar++)
            {
                ofstream outfile_near;
                outfile_near.open(directory_name + Cars[icar].Car_Owner + Cars[icar].Car_Number + "_" + to_string((int)Cars[icar].Car_Axles) + "_Near" + ".hdat");

                ofstream outfile_far;
                outfile_far.open(directory_name + Cars[icar].Car_Owner + Cars[icar].Car_Number + "_" + to_string((int)Cars[icar].Car_Axles) + "_Far" + ".hdat");

                for (int iaxle = 0; iaxle < int(Cars[icar].Car_Axles); iaxle++)
                {
                    for (int icrib = 0; icrib < Trn_Forces.size(); icrib++)
                    {
                        if (iaxle_total < Trn_Forces[icrib].WheelForce_Lat.size())
                        {
                            if (Trn_Forces[icrib].Crib_Location.compare("Near") == 0)
                            {
                                outfile_near << (to_string(Trn_Forces[icrib].WheelForce_Lat[iaxle_total]) + "\t" + to_string(Trn_Forces[icrib].WheelForce_Static[iaxle_total]) + "\n");
                            }
                            else if (Trn_Forces[icrib].Crib_Location.compare("Far") == 0)
                            {
                                outfile_far << (to_string(Trn_Forces[icrib].WheelForce_Lat[iaxle_total]) + "\t" + to_string(Trn_Forces[icrib].WheelForce_Static[iaxle_total]) + "\n");
                            }
                        }
                    }
                    //Trn_Car.push_back(Cars[icar].Car_Owner + Cars[icar].Car_Number + "_" + std::to_string(iaxle));
                    iaxle_total++;
                }
                Trn_Car.push_back(Cars[icar].Car_Owner + Cars[icar].Car_Number);
                outfile_near.close();
                outfile_far.close();
            }
            //return trn_car;
        };

        void Reshape4Wheel()
        {
            for (int icar = 0; icar < Cars.size(); icar++)
            {
                for (int iaxle = 0; iaxle < (int)(Cars[icar].Car_Axles); iaxle++)
                {
                    Wheel_Forces_Near.push_back(currenttrain_list);
                    Wheel_Forces_Far.push_back(currenttrain_list);
                }
            };

            int iaxle_total = 0;
            for (int icar = 0; icar < Cars.size(); icar++)
            {
                for (int iaxle = 0; iaxle < (int)(Cars[icar].Car_Axles); iaxle++)
                {
                    for (int icrib = 0; icrib < Trn_Forces.size(); icrib++)
                    {
                        if (iaxle_total < Trn_Forces[icrib].WheelForce_PeakTime.size())
                        {
                            TrainInfo::Crib_Info current_train;
                            current_train.Crib_Location = Trn_Forces[icrib].Crib_Location;
                            current_train.Crib_Number = Trn_Forces[icrib].Crib_Number;
                            current_train.Track_Type = Trn_Forces[icrib].Track_Type;
                            current_train.Crib_Type = Trn_Forces[icrib].Crib_Type;

                            current_train.WheelForce_PeakTime.push_back(Trn_Forces[icrib].WheelForce_PeakTime[iaxle_total]);
                            current_train.WheelForce_Lat.push_back(Trn_Forces[icrib].WheelForce_Lat[iaxle_total]);
                            current_train.WheelForce_Vert.push_back(Trn_Forces[icrib].WheelForce_Vert[iaxle_total]);
                            current_train.WheelForce_Static.push_back(Trn_Forces[icrib].WheelForce_Static[iaxle_total]);

                            if (Trn_Forces[icrib].Crib_Location.compare("Near") == 0)
                            {
                                Wheel_Forces_Near[iaxle_total].push_back(current_train);
                                //Wheel_Forces_Near.WheelForce_Lat[icrib] = Trn_Forces[icrib].WheelForce_Lat[iaxle_total];
                            }
                            else if (Trn_Forces[icrib].Crib_Location.compare("Far") == 0)
                            {
                                Wheel_Forces_Far[iaxle_total].push_back(current_train);
                                //Wheel_Forces_Far.WheelForce_Lat[iaxle] = &Trn_Forces[icrib].WheelForce_Lat[iaxle_total];
                            }
                        }
                    }
                    //Whl_Forces.push_back(*Wheel_Forces_Near);
                    Trn_Whl.push_back(Cars[icar].Car_Owner + Cars[icar].Car_Number + "_" + std::to_string(iaxle));
                    iaxle_total++;
                }
            }
            //return trn_wheel;
        };

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

        void LengthNotEqual()
        {
            std::vector<int> vector_length;
            
            for (int icrib = 0; icrib < Trn_Forces.size(); icrib++)
            {
                vector_length.push_back(Trn_Forces[icrib].WheelForce_Lat.size());
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

            for (int icrib = 0; icrib < Trn_Forces.size(); icrib++)
            {
                if (Trn_Forces[icrib].WheelForce_Lat.size() == vector_length[mode])
                {
                    Trn_Forces[icrib].isnotmode = false;
                    //isnotmode.push_back(false);
                }
                else
                {
                    Trn_Forces[icrib].isnotmode = true;
                    //isnotmode.push_back(true);
                }
            }
            //return isnotmode;
        }

        int iterateline(std::vector<std::string> stringvect, int linecurrent, std::string texttofind)
        {

            while ((stringvect[linecurrent].substr(0, 3)).compare(texttofind) != 0)
            {
                linecurrent = linecurrent + 1;
            }

            return linecurrent;
        }

        double convert2datetime(std::string string_time)
        {

            std::vector<std::string> parsed_time1 = ParseString(string_time, "_");
            
            std::vector<std::string> parsed_time2 = ParseString(parsed_time1[2], " ");
            std::vector<std::string> parsed_time3 = ParseString(parsed_time2[1], ":");
            double secondsinyear = 31540000;
            double secondsinmonth= 2628000;
            double secondsinday = 864500;
            double secondsinhour = 3600;
            double secondsinminute = 60;

            double secondssinceepoch = stod(parsed_time1[0])* secondsinyear + stod(parsed_time1[1])*secondsinmonth + stod(parsed_time2[0])*secondsinday + stod(parsed_time3[0])*secondsinhour + stod(parsed_time3[1])*secondsinminute + stod(parsed_time3[2]);
            //std::string trn_starttime = string_time;
            //std::tm tmb{};
            //std::istringstream(string_time) >> std::get_time(&tmb, "%Y_%m_%d %H:%M:%S.%06d");
            //auto t1 = std::mktime(&tmb);*/

            return secondssinceepoch;
        }

};

class TrainInfoHandle
{
private:
    TrainInfo* imp;

public:

    TrainInfoHandle() : imp(new TrainInfo()) {  }

    ~TrainInfoHandle()
    {
        free(imp);
    }

    TrainInfo* operator->() { return imp; }
};