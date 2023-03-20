#pragma once

#include <opencv2/core.hpp>

using namespace std;
using namespace cv;

class PlotFeatures
{
	public:
		PlotFeatures();
		~PlotFeatures();

		void NonMaximalFeatures(vector<string>& bboxlabel_afteriou, vector<Rect*>& bboxloc_afteriou, double& scalefactor_width, double& scalefactor_height, Mat& im);

		void HighestProbabilityFeatures(vector<string>& label, vector<Rect*>& bboxloc, vector<vector<int>>& bboxnum, double& scalefactor_width, double& scalefactor_height, Mat& im);

		void MeanFeatures(vector<vector<double>>& conf, vector<string>& label, vector<Rect*>& bboxloc, vector<vector<int>>& bboxnum, double& scalefactor_width, double& scalefactor_height, Mat& im);

		void MedianFeatures(vector<vector<double>>& conf, vector<string>& label, vector<Rect*>& bboxloc, vector<vector<int>>& bboxnum, double& scalefactor_width, double& scalefactor_height, Mat& im);

		void ModeFeatures(vector<vector<double>>& conf, vector<string>& label, vector<Rect*>& bboxloc, vector<vector<int>>& bboxnum, double& scalefactor_width, double& scalefactor_height, Mat& im);

	private:

};