#pragma once
#include <opencv2/core.hpp>

using namespace std;
using namespace cv;
#pragma once

class Pooling
{
	public:
		Pooling();
		~Pooling();

		void MaxPooling2D(Mat img, Mat& imgout, vector<int>& poolxy, int& channels);

		void AvePooling2D(Mat img, Mat& imgout, vector<int>& poolxy, int& channels);

		void MinPooling2D(Mat img, Mat& imgout, vector<int>& poolxy, int& channels);

	private:

};