#pragma once

class Regression
{
	public:
		void SinusoidalRegression(std::vector<std::string*> data, double& phase, double& mag, double& shift)
		{
			double M_PI = 3.14;
			int numOfData = sizeof(&data);
			double* datax = new double[numOfData];
			double* datay = new double[numOfData];
			double* dataz = new double[numOfData];

			double a(0);
			double b(0);
			double c(0);

			double XiXi(0);
			double XiYi(0);
			double XiZi(0);
			double YiZi(0);
			double YiYi(0);
			double Xi(0);
			double Yi(0);
			double Zi(0);

			for (int i = 0; i < numOfData; i++)
			{
				datax[i] = cos((2 * M_PI) * double(i % numOfData) / double(numOfData));
				datay[i] = sin((2 * M_PI) * double(i % numOfData) / double(numOfData));
				dataz[i] = std::stof(*data[i]);
			}

			// ---------------- Compute sinusoid of best fit in the form y=a+c*sin(x+phase) as derived at http://mariotapilouw.blogspot.com/2012/03/sine-fitting.html --------------//
			LeastSquaresFit(datax, datay, dataz, XiXi, XiYi, XiZi, YiZi, YiYi, Xi, Yi, Zi);
			Calc_MatrixInverse(mag, a, b, c, XiXi, XiYi, XiZi, Xi, YiYi, YiZi, Yi, Zi, dataz);
			Calc_Phase(phase, a, b);
			shift = a;
		}

		void LeastSquaresFit(double*& datax, double*& datay, double*& dataz, double& XiXi, double& XiYi, double& XiZi, double& YiZi, double& YiYi, double& Xi, double& Yi, double& Zi)
		{
			for (int i = 0; i < sizeof(dataz); i++)
			{
				XiYi += datax[i] * datay[i];
				XiZi += datax[i] * dataz[i];
				YiZi += datay[i] * dataz[i];
				XiXi += datax[i] * datax[i];
				YiYi += datax[i] * datay[i];
				Xi += datax[i];
				Yi += datay[i];
				Zi += dataz[i];
			}
		}

		void Calc_MatrixInverse(double& mag, double& a, double& b, double& c, double& XiXi, double& XiYi, double& XiZi, double& Xi, double& YiYi, double& YiZi, double& Yi, double& Zi, double*& dataz)
		{
			double p[3] = { 0 }; // product of fitting equation 
			double A[3][3];
			double B[3][3];
			double C[3][3];
			double X[3][3];
			int i;
			int j;
			double x = 0;
			double n = 0; //n is the determinant of A

			A[0][0] = XiXi;
			A[0][1] = XiYi;
			A[0][2] = Xi;
			A[1][0] = XiYi;
			A[1][1] = YiYi;
			A[1][2] = Yi;
			A[2][0] = Xi;
			A[2][1] = Yi;
			A[2][2] = sizeof(dataz);

			for (i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					B[i][j] = 0;
					C[i][j] = 0;
				}
			}

			for (i = 0, j = 0; j < 3; j++)
			{
				if (j == 2)
				{
					n += A[i][j] * A[i + 1][0] * A[i + 2][1];
				}
				else if (j == 1)
				{
					n += A[i][j] * A[i + 1][j + 1] * A[i + 2][0];
				}
				else
				{
					n += A[i][j] * A[i + 1][j + 1] * A[i + 2][j + 2];
				}
			}

			for (i = 2, j = 0; j < 3; j++)
			{
				if (j == 2)
				{
					n -= A[i][j] * A[i - 1][0] * A[i - 2][1];
				}
				else if (j == 1)
				{
					n -= A[i][j] * A[i - 1][j + 1] * A[i - 2][0];
				}
				else
				{
					n -= A[i][j] * A[i - 1][j + 1] * A[i - 2][j + 2];
				}
			}

			// Check determinant n of matrix A
			if (n)
			{
				x = 1.0 / n;

				for (i = 0; i < 3; i++)
				{
					for (j = 0; j < 3; j++)
					{
						B[i][j] = A[j][i];
					}
				}

				C[0][0] = (B[1][1] * B[2][2]) - (B[2][1] * B[1][2]);
				C[0][1] = ((B[1][0] * B[2][2]) - (B[2][0] * B[1][2])) * (-1);
				C[0][2] = (B[1][0] * B[2][1]) - (B[2][0] * B[1][1]);

				C[1][0] = ((B[0][1] * B[2][2]) - (B[2][1] * B[0][2])) * (-1);
				C[1][1] = (B[0][0] * B[2][2]) - (B[2][0] * B[0][2]);
				C[1][2] = ((B[0][0] * B[2][1]) - (B[2][0] * B[0][1])) * (-1);

				C[2][0] = (B[0][1] * B[1][2]) - (B[1][1] * B[0][2]);
				C[2][1] = ((B[0][0] * B[1][2]) - (B[1][0] * B[0][2])) * (-1);
				C[2][2] = (B[0][0] * B[1][1]) - (B[1][0] * B[0][1]);

				for (i = 0; i < 3; i++)
				{
					for (j = 0; j < 3; j++)
					{
						X[i][j] = C[i][j] * x;
					}
				}

				p[0] = XiZi;
				p[1] = YiZi;
				p[2] = Zi;

				a = X[0][0] * p[0] + X[0][1] * p[1] + X[0][2] * p[2];
				b = X[1][0] * p[0] + X[1][1] * p[1] + X[1][2] * p[2];
				c = X[2][0] * p[0] + X[2][1] * p[1] + X[2][2] * p[2];
			}
			else  // determinant=0
			{
				a = 1;
				b = 1;
				c = 0;
			}

			//mag = sqrt(a * a + b * b);
			mag = sqrt(c * c);
		}

		void Calc_Phase(double& phase, double& a, double& b)
		{
			double phi;
			double M_PI = 3.14;

			if ((b == 0) && (a >= 0))
			{
				phase = M_PI / 2;
			}
			else if ((b == 0) && (a >= 0))
			{
				phase = 3 * M_PI / 2;
			}
			else if ((a >= 0) && (b > 0))
			{
				phi = atan(fabs(a / b));
				phase = phi;
			}
			else if ((a >= 0) && (b < 0))
			{
				phi = atan(fabs(a / b));
				phase = M_PI - phi;
			}
			else if ((a < 0) && (b > 0))
			{
				phi = atan(fabs(a / b));
				phase = 2 * M_PI - phi;
			}
			else if ((a < 0) && (b < 0))
			{
				phi = atan(fabs(a / b));
				phase = phi + M_PI;
			}
		}
};