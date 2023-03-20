#include "pch.h"

#include <iostream>
#include <vector>

#include <opencv2/core.hpp>

#include "sort.h"

using namespace std;
using namespace cv;

Sort::Sort() {}
Sort::~Sort() {}

void Sort::SortColumns(double& maxVal, vector<vector<double>>& conf, vector<double*> glob_maxValue, vector<int*>& glob_maxLoc, vector<string>& label, vector<vector<int>>& bboxnum)
{
    for (int i = 1; i < conf.size(); i++)
    {
        for (int j = 0; j < label.size(); j++)
        {
            bboxnum[i][j] = i;
            maxVal = conf[i][j];
            if (i > 0)
            {
                if (maxVal > conf[i - 1][j])
                {
                    glob_maxValue[j] = &conf[i][j];
                    *glob_maxLoc[j] = bboxnum[i][j];

                }
                int k = 1;
                while (k < conf.size())
                {
                    if (conf[k][j] > conf[k - 1][j])
                    {
                        double tmp1 = conf[k - 1][j];
                        int tmp2 = bboxnum[k - 1][j];

                        conf[k - 1][j] = conf[k][j];
                        conf[k][j] = tmp1;
                        bboxnum[k - 1][j] = bboxnum[k][j];
                        bboxnum[k][j] = tmp2;
                        k = 1;
                    }
                    else
                    {
                        k++;
                    }
                }
            }
        }
    }
    //return conf, bboxnum;
}

void Sort::unique(const Mat& input, vector<float>& out, vector<Rect>& watershedbb, bool sort)
{
    vector<int> ignore_i;
    vector<int> ignore_j;
    for (int j = 0; j < input.cols; j++)
    {
        for (int i = 0; i < input.rows; i++)
        {
            if (input.at<float>(i, j) != -1 && input.at<float>(i, j) != 1 && input.at<float>(i, j) != input.at<float>(i + 1, j + 1) && count(out.begin(), out.end(), input.at<float>(i, j)) == 0)
            {
                cout << "Unique??: " << input.at<float>(i, j) << std::endl;
                out.push_back(input.at<float>(i, j));
                ignore_i.push_back(i);
                ignore_j.push_back(j);
                watershedbb.push_back(cv::Rect(i, j, 10, 20));
            }
        }
    }
    if (sort)
    {
        std::sort(out.begin(), out.end());
    }
}