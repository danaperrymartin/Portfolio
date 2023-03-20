#include <opencv2/opencv.hpp>
#include <opencv2/core.hpp>
#include <opencv2/ximgproc/segmentation.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/imgproc/imgproc.hpp>
#include <opencv2/highgui.hpp>
#include <opencv2/core/utility.hpp>
#include <opencv2/imgcodecs.hpp>

using namespace cv;


int slider_max, slider, displayHeight, num_imgs;
int displayWidth = 1900;
Mat src1;  // original big image
Mat dst;
cv::Rect roi;

class DisplayImage
{
public:
    int ShowImage(cv::Mat im, int num_imgs)
    {
        src1 = im;
        slider_max = src1.cols - displayWidth;
        slider = 0;
        displayHeight = src1.rows/ (num_imgs);

        namedWindow("Result", WINDOW_AUTOSIZE); // Create Window
        char TrackbarName[50];
        sprintf_s(TrackbarName, "Pixel Pos");
        createTrackbar(TrackbarName, "Result", &slider, slider_max, on_trackbar);
        on_trackbar(slider, 0);
        waitKey(0);

        return 0;

        /*cv::imshow("Output", im);
        int l = cv::waitKey();
        if (l == 99)
        {
        }
        else
        {
            cv::destroyAllWindows();
        }*/
    };

    static void on_trackbar(int, void*)
    {
        roi = cv::Rect(slider, 0, displayWidth, displayHeight);  //Update ROI for display
        dst = src1(roi);

        imshow("Result", dst);
    };

};

