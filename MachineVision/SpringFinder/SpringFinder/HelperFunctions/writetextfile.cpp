#include <fstream>
#include <vector>

#include <opencv2/core.hpp>

#include "writetextfile.h"

using namespace std;
using namespace cv;

WriteTextFile::WriteTextFile() {}
WriteTextFile::~WriteTextFile() {}

void WriteTextFile::ConfidenceProbabilities(vector<vector<double>>& conf, vector<string>& label)
{
    int nrow, ncol;
    nrow, ncol = conf.size();

    // Write Probabilities to text file:
    ofstream out("ConfidenceProbabilities.txt");

    // Print ranked probabilities to text file
    for (int i = 0; i < conf.size(); i++)
    {
        if (i == 0)
        {
            for (int j = 0; j < label.size(); j++)
            {
                out << label[j];
                out << "\t";
            }
            out << "\n";
        }
        for (int j = 0; j < label.size(); j++)
        {
            out << conf[i][j];
            out << "\t";
        }
        out << "\n";
    }
    out.close();
};

void WriteTextFile::BoundingBoxDim(int bbox_width, int bbox_height)
{

    // Write Probabilities to text file:
    ofstream out("BoundingBoxDim.txt", std::ios_base::app);
    out << to_string(bbox_width) + "\t" + to_string(bbox_height) <<std::endl;
    out.close();
};