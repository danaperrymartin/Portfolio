#pragma once

#include <vector>
#include <string>

#include <opencv2/core.hpp>

using namespace std;

class WriteTextFile
{
	public:
		WriteTextFile();
		~WriteTextFile();

		void ConfidenceProbabilities(vector<vector<double>>& conf, vector<string>& label);

		void BoundingBoxDim(int bbox_width, int bbox_length);

	private:

};