#include <opencv2\opencv.hpp>
#include <iostream>
#include <fstream>
#include <iterator>
#include <vector>
#include <cstdio>
#include <bitset>

#include <ctime>
#include <iomanip>
#include <sstream>
#include <iostream>
#include <sstream>
#include <ctime>

using namespace cv;
using namespace std;

typedef bitset<8> BYTE;

class BinFile
{
    public:
        void LoadBinFile(const char *rawbinfile, unsigned char* &buffer, FILE* &pFile, int64 &lSize)
        {
            std::string s = rawbinfile;
            std::string delimiter = "_";
            
            std::vector<std::string> parsed_string = ParseString(s, delimiter);
            std::vector<std::string> resolution1 = ParseString(parsed_string.back(),"x");
            std::vector<std::string> resolution2 = ParseString(resolution1.back(), ".");

            std::vector<std::string> resolution = { resolution1[0], resolution2[0] };
 
            size_t result;

            // copy the file into the buffer:
            result = fread(buffer, std::stoi(resolution[0]), lSize / std::stoi(resolution[0]), pFile);
            if (result != lSize / std::stoi(resolution[0])) { fputs("Reading error", stderr); exit(3); }

            /* the whole file is now loaded in the memory buffer. */
            // terminate
            fclose(pFile);
            //free(buffer);

            //return { buffer, lSize };
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

