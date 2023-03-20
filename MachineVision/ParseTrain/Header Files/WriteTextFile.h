#pragma once
#include <fstream>
#include <vector>

using namespace std;

class WriteTextFile
{
public:
	void ConfidenceProbabilities(std::vector<std::vector<double>> &conf, std::vector<std::string> &label)
	{
        int nrow, ncol;
        nrow, ncol = conf.size();

        // Write Probabilities to text file:
        std::ofstream out("ConfidenceProbabilities.txt");

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
	}
};

