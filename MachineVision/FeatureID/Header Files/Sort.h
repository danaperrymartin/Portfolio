#pragma once
#include <vector>
#include <string>
#include <algorithm>
#include <vector>
#include <opencv2/core.hpp>

#include <iostream>

using namespace cv;

class Sort
{
public:
    auto  SortColumns(double& maxVal, std::vector<std::vector<double>>& conf, std::vector<double*> glob_maxValue, std::vector<int*>& glob_maxLoc, std::vector<std::string> &label, std::vector<std::vector<int>>& bboxnum)
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
        return conf, bboxnum;
	}
    void unique(const cv::Mat &input, std::vector<float>& out, std::vector<cv::Rect>& watershedbb, bool sort)
    {
        std::vector<int> ignore_i;
        std::vector<int> ignore_j;
        for (int j = 0; j < input.cols; j++)
        {
            for (int i = 0; i < input.rows; i++)
            {
                if (input.at<float>(i, j)!=-1 && input.at<float>(i, j) != 1 && input.at<float>(i, j)!= input.at<float>(i+1, j+1) && std::count(out.begin(), out.end(), input.at<float>(i, j))==0)
                {
                    std::cout << "Unique??: " << input.at<float>(i, j) << std::endl;
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
};

