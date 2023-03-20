#pragma once
#include <map>
#include <string>

#include <opencv2/core.hpp>

using namespace cv;
using namespace std;

class TextureHelperFunctions
{
	private:
		static unsigned int const AREA_THRESHOLD = 2000; //originally 2000
		
		

	public:
		//TextureHelperFunctions();
		//~TextureHelperFunctions();

		static void displayImage(string const& windowName, Mat& image, bool withExit = false);

		static void saveImage(string const& windowName, Mat image);

		template<typename T>
		static void getIteratorAtPoint(Mat const& image, Point const& p, MatIterator_<T>& it);

		template<typename T>
		static MatConstIterator_<T> getIteratorAtPoint(Mat const& image, Point const& p);

		template<typename T>
		static MatIterator_<T> getIteratorAtPoint(Mat& image, Point const& p);

		static void removeColumn(Mat const& image, Mat& histogram, Point const& p, map<int, int>& m);

		static void addColumn(Mat const& image, Mat& histogram, Point const& p, map<int, int>& m);

		static void removeRow(Mat const& image, Mat& histogram, Point const& p, map<int, int>& m);

		static void addRow(Mat const& image, Mat& histogram, Point const& p, map<int, int>& m);

		static double calculateEntropy(Mat const& histogram);

		static double calculateEntropyAndUpdateHistogram(map<int, int>& m, double previousPixelEntropyValue, Mat& histogram);

		static void calculateInitialHistogram(Mat const& image, Mat& histogram);

		static void entropyFilt(Mat const& image, Mat& entropy);

		static void calculateSymetricPoint(Point const& inPoint, Point& outPoint, Size const& psize);

		static void symmetricPadding(Mat& image, Mat& paddedImage);

		static void bwareaopen(Mat const& input, Mat& output);

		static void imclose(Mat& image);

		static void imfill(Mat& image);

		static void maskImage(Mat const& image, Mat const& mask, Mat& output);

		static void conversion(Mat const& input, Mat& output);

	
};