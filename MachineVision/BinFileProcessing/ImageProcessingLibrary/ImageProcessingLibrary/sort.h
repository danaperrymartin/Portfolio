#pragma once
#include "pch.h"
#include <opencv2\core.hpp>

using namespace std;
using namespace cv;

class Sort
{
	public:
		Sort();
		~Sort();

		void SortColumns(double& maxVal, vector<vector<double>>& conf, vector<double*> glob_maxValue, vector<int*>& glob_maxLoc, vector<string>& label, vector<vector<int>>& bboxnum);
		
		void unique(const Mat& input, vector<float>& out, vector<Rect>& watershedbb, bool sort);

	private:
};