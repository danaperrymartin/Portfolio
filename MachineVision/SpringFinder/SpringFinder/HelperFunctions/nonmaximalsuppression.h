#pragma once

#include <vector>
#include <string>

#include <opencv2/core.hpp>

class NonMaximalSuppression
{
	public:
		NonMaximalSuppression();   //Constructor
		~NonMaximalSuppression();  //Destructor

		void NMS(std::vector<std::string>& label, std::vector<cv::Rect>& rects, std::vector<cv::Rect*>& bboxloc, std::vector<std::string>& bboxlabel_afteriou, std::vector<cv::Rect*>& bboxloc_afteriou, std::vector<std::vector<int>>& bboxnum);
	private:

};