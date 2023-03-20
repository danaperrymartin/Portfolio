#pragma once
#include <string>
#include <sstream> 

using namespace std;

class TriDiagonalMatrix
{
	private:

	public:
		float* A;
		float* B;
		float* C;
		float* cPrime;
		float* dPrime;
		float* x;

		int di;
		int n;
		int dlength;

		TriDiagonalMatrix(int number) : n(number)
		{
			A = new float[n] {0};
			B = new float[n] {0};
			C = new float[n] {0};
			cPrime = new float[n](0);
			dPrime = new float[n](0);
			x = new float[n](0);

			di = 0;
			n = 0;
			dlength = 0;
		}

		~TriDiagonalMatrix()
		{
			free(A);
			free(B);
			free(C);
			free(cPrime);
			free(dPrime);
			free(x);

			delete& di;
			delete& n;
			delete& dlength;
		}

		// Getter
		const int N(int arraylength) 
		{
			return arraylength;
		}

		//Getter
		float getthis_function(int row, int col)
		{
			di = row - col;

			if (di == 0)
			{
				return B[row];
			}
			else if (di == -1)
			{
				//Debug.Assert(row < N - 1);
				return C[row];
			}
			else if (di == 1)
			{
				//Debug.Assert(row > 0);
				return A[row];
			}
			else return 0;
		}

		//Setter
		void setthis_function(int value, int row, int col)
		{
			di = row - col;

			if (di == 0)
			{
				B[row] = value;
			}
			else if (di == -1)
			{
				//Debug.Assert(row < N - 1);
				C[row] = value;
			}
			else if (di == 1)
			{
				//Debug.Assert(row > 0);
				A[row] = value;
			}
			else
			{
				//throw new ArgumentException("Only the main, super, and sub diagonals can be set.");
			}
		}

		float* Solve(float* d, int arraysize)
		{
			n = N(arraysize);
		    dlength = arraysize;
			if (dlength != n)
			{
				throw std::invalid_argument("The input d is not the same size as this matrix.");
			}

			// cPrime
			cPrime[0] = C[0] / B[0];

			for (int i = 1; i < n; i++)
			{
				cPrime[i] = C[i] / (B[i] - cPrime[i - 1] * A[i]);
			}

			// dPrime
			dPrime[0] = d[0] / B[0];

			for (int i = 1; i < n; i++)
			{
				dPrime[i] = (d[i] - dPrime[i - 1] * A[i]) / (B[i] - cPrime[i - 1] * A[i]);
			}

			// Back substitution
			x[n - 1] = dPrime[n - 1];

			for (int i = n - 2; i >= 0; i--)
			{
				x[i] = dPrime[i] - cPrime[i] * x[i + 1];
			}
			return x;
		};
};

class TriDiagonalMatrixHandle
{
	private:
		TriDiagonalMatrix* imp;

	public:

		TriDiagonalMatrixHandle(int number) : imp(new TriDiagonalMatrix(number)) {  }

		~TriDiagonalMatrixHandle()
		{
			free(imp);
		}

		TriDiagonalMatrix* operator->() { return imp; }
};
