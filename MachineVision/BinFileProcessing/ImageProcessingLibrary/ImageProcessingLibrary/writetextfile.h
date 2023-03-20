#pragma once

#include <vector>
#include <string>

using namespace std;

class WriteTextFile
{
	public:
		WriteTextFile();
		~WriteTextFile();

		void ConfidenceProbabilities(vector<vector<double>>& conf, vector<string>& label);

	private:

};