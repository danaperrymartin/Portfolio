#include "pch.h"

#include <fstream>
#include <vector>

#include "writetextfile.h"

using namespace std;

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