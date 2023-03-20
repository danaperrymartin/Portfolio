#include "plotfeatures.h"
#include "nonmaximalsuppression.h"

#include <opencv2/core.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/highgui.hpp>

using namespace cv;
using namespace std;

PlotFeatures::PlotFeatures() {}
PlotFeatures::~PlotFeatures() {}

void PlotFeatures::NonMaximalFeatures(vector<string>& bboxlabel_afteriou, vector<Rect*>& bboxloc_afteriou, double& scalefactor_width, double& scalefactor_height, Mat& im)
{
    Mat img_orig = im.clone();
    Rect bboxloc_tmp;

    int k = 0;
    while (k < bboxloc_afteriou.size())
    {
        bboxloc_tmp = *bboxloc_afteriou[k];
        bboxloc_tmp.x = (int)((double)bboxloc_tmp.x * scalefactor_width);
        bboxloc_tmp.y = (int)((double)bboxloc_tmp.y * scalefactor_height);

        bboxloc_tmp.width = bboxloc_tmp.width * scalefactor_width;
        bboxloc_tmp.height = bboxloc_tmp.height * scalefactor_height;
        rectangle(img_orig, bboxloc_tmp, Scalar(0, 255, 0));
        putText(img_orig, //target image
            bboxlabel_afteriou[k], //text
            Point(bboxloc_tmp.x, bboxloc_tmp.y + 10), //top-left position
            FONT_HERSHEY_COMPLEX_SMALL,
            0.5,
            CV_RGB(255, 255, 255), //font color
            2);
        k++;
    }

    namedWindow("Output", 0);
    imshow("Output", img_orig);
    int l = waitKey();
    if (l == 99)
    {
    }
    else
    {
        destroyAllWindows();
    }
    imwrite("FeatureID_NMS.jpg", img_orig);
}

void PlotFeatures::HighestProbabilityFeatures(vector<string>& label, vector<Rect*>& bboxloc, vector<vector<int>>& bboxnum, double& scalefactor_width, double& scalefactor_height, Mat& im)
{
    Mat img_orig = im.clone();
    Rect bboxloc_tmp;

    for (int j = 0; j < label.size(); j++)
    {
        int k = 0;
        while (k < 5)
        {
            bboxloc_tmp = *bboxloc[bboxnum[k][j]];
            bboxloc_tmp.x = (int)((double)bboxloc_tmp.x * scalefactor_width);
            bboxloc_tmp.y = (int)((double)bboxloc_tmp.y * scalefactor_height);

            bboxloc_tmp.width = bboxloc_tmp.width * scalefactor_width;
            bboxloc_tmp.height = bboxloc_tmp.height * scalefactor_height;
            rectangle(img_orig, bboxloc_tmp, Scalar(0, 255, 0));
            putText(img_orig, //target image
                label[j], //text
                Point(bboxloc_tmp.x, bboxloc_tmp.y + (10)), //top-left position
                FONT_HERSHEY_COMPLEX_SMALL,
                0.25,
                CV_RGB(255, 255, 255), //font color
                2);
            k++;
        }
    }

    imshow("Output", img_orig);
    int l = waitKey();
    if (l == 99)
    {
    }
    else
    {
        destroyAllWindows();
    }
    imwrite("FeatureID_MaxProb.jpg", img_orig);
}

void PlotFeatures::MeanFeatures(vector<vector<double>>& conf, vector<string>& label, vector<Rect*>& bboxloc, vector<vector<int>>& bboxnum, double& scalefactor_width, double& scalefactor_height, Mat& im)
{
    Mat img_orig = im.clone();
    // Compute mean probability across labels
    for (int j = 0; j < label.size(); j++)
    {
        double total_prob = 0.0;
        double mean_prob = 0.0;
        int mean_loc = 0;

        int k = 0;
        while (k < conf.size())
        {
            total_prob = total_prob + conf[k][j];
            k++;
        }

        mean_prob = total_prob / conf.size();

        double thresh = 0.5; // max threshold of the matching value for COT
        //double thresh = 0.000001; // max threshold of the matching value for Car
        vector<int> found_loc;

        for (int i = 0; i < conf.size(); i++)
        {
            if (abs(conf[i][j] - mean_prob) < thresh)
            {
                found_loc.push_back(i);
            }
        }

        vector<Rect> rects(found_loc.size());
        k = 0;
        while (k < found_loc.size())
        {
            rects[k] = (*bboxloc[bboxnum[found_loc[k]][j]]);
            k++;
        }

        NonMaximalSuppression nonmax;
        vector<Rect*> bboxloc_afteriou;
        vector<string> bboxlabel_afteriou;
        nonmax.NMS(label, rects, bboxloc, bboxlabel_afteriou, bboxloc_afteriou, bboxnum);

        Rect bboxloc_tmp;
        k = 0;
        //while (kk < found_loc.size())
        while (k < bboxloc_afteriou.size())
        {
            bboxloc_tmp = *bboxloc_afteriou[k];
            //bboxloc_tmp = *bboxloc[bboxnum[found_loc[k]][j]];
            bboxloc_tmp.x = (int)((double)bboxloc_tmp.x * scalefactor_width);
            bboxloc_tmp.y = (int)((double)bboxloc_tmp.y * scalefactor_height);

            bboxloc_tmp.width = bboxloc_tmp.width * scalefactor_width;
            bboxloc_tmp.height = bboxloc_tmp.height * scalefactor_height;
            rectangle(img_orig, bboxloc_tmp, Scalar(0, 255, 0));
            putText(img_orig, //target image
                label[j], //text
                Point(bboxloc_tmp.x, bboxloc_tmp.y + (10)), //top-left position
                FONT_HERSHEY_COMPLEX_SMALL,
                0.25,
                CV_RGB(255, 255, 255), //font color
                2);
            k++;
        }
    }
    imshow("Output", img_orig);
    int l = waitKey();
    if (l == 99)
    {
    }
    else
    {
        destroyAllWindows();
    }
    imwrite("FeatureID_MeanProb.jpg", img_orig);
}

void PlotFeatures::MedianFeatures(vector<vector<double>>& conf, vector<string>& label, vector<Rect*>& bboxloc, vector<vector<int>>& bboxnum, double& scalefactor_width, double& scalefactor_height, Mat& im)
{
    Mat img_orig = im.clone();
    // Compute mean probability across labels
    for (int j = 0; j < label.size(); j++)
    {
        double median_prob = 0.0;

        median_prob = conf[conf.size() / 2][j];


        double thresh = 0.02; // max threshold of the matching value for COT
        //double thresh = 0.000001; // max threshold of the matching value for Car
        vector<int> found_loc;

        for (int i = 0; i < conf.size(); i++)
        {
            if (abs(conf[i][j] - median_prob) < thresh)
            {
                found_loc.push_back(i);
            }
        }

        Rect bboxloc_tmp;
        int kk = 0;
        while (kk < found_loc.size())
        {
            bboxloc_tmp = *bboxloc[bboxnum[found_loc[kk]][j]];
            bboxloc_tmp.x = (int)((double)bboxloc_tmp.x * scalefactor_width);
            bboxloc_tmp.y = (int)((double)bboxloc_tmp.y * scalefactor_height);

            bboxloc_tmp.width = bboxloc_tmp.width * scalefactor_width;
            bboxloc_tmp.height = bboxloc_tmp.height * scalefactor_height;
            rectangle(img_orig, bboxloc_tmp, Scalar(0, 255, 0));
            putText(img_orig, //target image
                label[j], //text
                Point(bboxloc_tmp.x, bboxloc_tmp.y + (10)), //top-left position
                FONT_HERSHEY_COMPLEX_SMALL,
                0.25,
                CV_RGB(255, 255, 255), //font color
                2);
            kk++;
        }
    }
    imshow("Output", img_orig);
    int l = waitKey();
    if (l == 99)
    {
    }
    else
    {
        destroyAllWindows();
    }
    imwrite("FeatureID_MedianProb.jpg", img_orig);
}

void PlotFeatures::ModeFeatures(vector<vector<double>>& conf, vector<string>& label, vector<Rect*>& bboxloc, vector<vector<int>>& bboxnum, double& scalefactor_width, double& scalefactor_height, Mat& im)
{
    Mat img_orig = im.clone();
    // Compute mean probability across labels
    for (int j = 0; j < label.size(); j++)
    {
        double mode_prob = 0.0;

        double number = conf[0][j];
        double max = conf[0][j];
        double min = conf[0][j];

        for (int i = 1, countMode = 1, count = 1; i < conf.size(); ++i)
        {
            if (conf[i][j] == number)
                ++countMode;
            if (countMode > count) {
                count = countMode;
                mode_prob = number;
            }
            if (conf[i][j] > max)
                max = conf[i][j];
            if (conf[i][j] < min)
                min = conf[i][j];
            number = conf[i][j];
        }

        double thresh = 0.02; // max threshold of the matching value for COT
        //double thresh = 0.00000001; // max threshold of the matching value for Car
        vector<int> found_loc;

        for (int i = 0; i < conf.size(); i++)
        {
            if (abs(conf[i][j] - mode_prob) < thresh)
            {
                found_loc.push_back(i);
            }
        }

        Rect bboxloc_tmp;
        int kk = 0;
        while (kk < found_loc.size())
        {
            bboxloc_tmp = *bboxloc[bboxnum[found_loc[kk]][j]];
            bboxloc_tmp.x = (int)((double)bboxloc_tmp.x * scalefactor_width);
            bboxloc_tmp.y = (int)((double)bboxloc_tmp.y * scalefactor_height);

            bboxloc_tmp.width = bboxloc_tmp.width * scalefactor_width;
            bboxloc_tmp.height = bboxloc_tmp.height * scalefactor_height;
            rectangle(img_orig, bboxloc_tmp, Scalar(0, 255, 0));
            putText(img_orig, //target image
                label[j], //text
                Point(bboxloc_tmp.x, bboxloc_tmp.y + (10)), //top-left position
                FONT_HERSHEY_COMPLEX_SMALL,
                0.25,
                CV_RGB(255, 255, 255), //font color
                2);
            kk++;
        }
    }
    imshow("Output", img_orig);
    int l = waitKey();
    if (l == 99)
    {
    }
    else
    {
        destroyAllWindows();
    }
    imwrite("FeatureID_ModeProb.jpg", img_orig);
}