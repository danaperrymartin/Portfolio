#include "pch.h"

#include "nonmaximalsuppression.h"

NonMaximalSuppression::NonMaximalSuppression(){}

NonMaximalSuppression::~NonMaximalSuppression() {}

void NonMaximalSuppression::NMS(std::vector<std::string>& label, std::vector<cv::Rect>& rects, std::vector<cv::Rect*>& bboxloc, std::vector<std::string>& bboxlabel_afteriou, std::vector<cv::Rect*>& bboxloc_afteriou, std::vector<std::vector<int>>& bboxnum)
{
    cv::Rect bbox_tmpa;
    cv::Rect bbox_tmpb;
    std::vector<int> ignore_k;
    std::vector<int> ignore_j;

    // Non maximum suppression
    for (int j = 0; j < label.size(); j++)
    {
        int k = 1;
        while (k < rects.size())
        {
            bbox_tmpa = *bboxloc[bboxnum[k - 1][j]];

            int b = 0;
            while (b < rects.size())
            {
                bbox_tmpb = *bboxloc[bboxnum[b][j]];
                double INTERSECTION = bbox_tmpa.area() - bbox_tmpb.area();
                double UNION = (bbox_tmpa.area() + bbox_tmpb.area());

                double iou = abs(INTERSECTION) / (UNION);
                /*f (bbox_tmpa.area() == bbox_tmpb.area())
                {*/
                if ((b == 0) || (iou > 0.8) && !std::count(ignore_k.begin(), ignore_k.end(), b) && !std::count(ignore_j.begin(), ignore_j.end(), j))
                {
                    ignore_j.push_back(j);
                    ignore_k.push_back(b);
                    bboxloc_afteriou.push_back(bboxloc[bboxnum[b][j]]);
                    bboxlabel_afteriou.push_back(label[j]);
                }
                //}
                b++;
            }
            k++;
        }
    }
}