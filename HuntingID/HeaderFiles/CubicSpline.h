#pragma once

#include"TriDiagonalMatrix.h"

class CubicSpline
{
private:

	int GetNextXIndex(float x)
	{
		if (x < xOrig[_lastIndex])
		{
			//throw new ArgumentException("The X values to evaluate must be sorted.");
		}

		while ((_lastIndex < originalarraysize - 2) && (x > xOrig[_lastIndex + 1]))
		{
			_lastIndex++;
		}

		return _lastIndex;
	}

	float EvalSpline(float x, int j, bool debug = false)
	{
		dx = xOrig[j] - xOrig[j + 1];
		t = (x - xOrig[j]) / dx;
		y_tmp = (1 - t) * yOrig[j] + t * yOrig[j + 1] + t * (1 - t) * (a_splinecoeff[j] * (1 - t) + b_splinecoeff[j] * t); // equation 9
		//if (debug) Console.WriteLine("xs = {0}, j = {1}, t = {2}", x, j, t);
		return y_tmp;
	}

public:
	const int newsize;
	int originalarraysize;
	int _lastIndex;
	int* upsampledarraysize;
	float* a_splinecoeff;
	float* b_splinecoeff;
	float* r;
	float* k;

	float* xOrig;
	float* yOrig;
	float* x_upsampled;
	float* y;
	float* yout;
	// a and b are each spline's coefficients

	float dx;
	float t;
	float y_tmp;

	int n;
	int j;
	float dx1, dx2, dy1, dy2;

	TriDiagonalMatrixHandle* tdm;

	CubicSpline(const int ns, const int os) : newsize(ns), originalarraysize(os)
	{
		a_splinecoeff = new float[newsize] { 0 };
		b_splinecoeff = new float[newsize] { 0 };

		_lastIndex = int(0);
		upsampledarraysize = new int[newsize](0);

		r = new float[originalarraysize](0);
		k = new float[originalarraysize](0);
		xOrig = new float[originalarraysize](0);
		yOrig = new float[originalarraysize](0);

		x_upsampled = new float[newsize] { 0 };
		y = new float[newsize] { 0 };
		yout = new float[newsize] { 0 };

		dx = 0;
		t = 0;
		y_tmp = 0;

		n = 0;
		j = 0;
		dx1 = 0;
		dx2 = 0;
		dy1 = 0;
		dy2=0;
	}

	~CubicSpline()
	{
		delete& newsize;
		delete& originalarraysize;
		delete& _lastIndex;
		free(upsampledarraysize);
		free(a_splinecoeff);
		free(b_splinecoeff);
		free(r);
		free(k);

		free(xOrig);
		free(yOrig);
		free(x_upsampled);
		free(y);
		free(yout);

		delete& dx;
		delete& t;
		delete& y_tmp;
		delete& n;
		delete& j;
		delete& dx1, dx2, dy1, dy2;

		free(tdm);
	}

	void setInterpolationarray(float* value)
	{
		memcpy(x_upsampled, value, *upsampledarraysize * sizeof(float));
	}
	// Getter
	float* getInterpolationarray()
	{
		return x_upsampled;
	}

	float* Eval(float* x, bool debug = false)
	{
		//CheckAlreadyFitted();

		n = *upsampledarraysize;

		_lastIndex = 0; // Reset simultaneous traversal in case there are multiple calls

		for (int i = 0; i < n; i++)
		{
			// Find which spline can be used to compute this x (by simultaneous traverse)
			j = GetNextXIndex(x[i]);

			// Evaluate using j'th spline
			y[i] = EvalSpline(x[i], j, debug);
		}
		return y;
	}

	CubicSpline(float* x, float* y, float startSlope = NAN, float endSlope = NAN, bool debug = false) :newsize()
	{
		Fit(x, y, startSlope, endSlope, debug);
	}

	float* FitAndEval(float* x, float* y, float* xs, int size_orig, int size_upsamp, float startSlope = NAN, float endSlope = NAN, bool debug = false)
	{
		Fit(x, y, size_orig, startSlope, endSlope, debug);
		return Eval(xs, debug);
	}

	void Fit(float* x, float* y, int size_orig, float startSlope = NAN, float endSlope = NAN, bool debug = false)
	{
		/*if (Single.IsInfinity(startSlope) || Single.IsInfinity(endSlope))
		{
			throw new Exception("startSlope and endSlope cannot be infinity.");
		}*/

		// Save x and y for eval
		xOrig = x;
		yOrig = y;

		n = originalarraysize;
		
		TriDiagonalMatrixHandle tdm(n);
		
		// First row is different (equation 16 from the article)
		if (isnan(startSlope))
		{
			dx1 = x[1] - x[0];
			tdm->C[0] = 1.0f / dx1;
			tdm->B[0] = 2.0f * tdm->C[0];
			r[0] = 3 * (y[1] - y[0]) / (dx1 * dx1);
		}
		else
		{
			tdm->B[0] = 1;
			r[0] = startSlope;
		}

		// Body rows (equation 15 from the article)
		for (int i = 1; i < n - 1; i++)
		{
			dx1 = x[i] - x[i - 1];
			dx2 = x[i + 1] - x[i];

			tdm->A[i] = 1.0f / dx1;
			tdm->C[i] = 1.0f / dx2;
			tdm->B[i] = 2.0f * (tdm->A[i] + tdm->C[i]);

			dy1 = y[i] - y[i - 1];
			dy2 = y[i + 1] - y[i];
			r[i] = 3 * (dy1 / (dx1 * dx1) + dy2 / (dx2 * dx2));
		}

		// Last row also different (equation 17 from the article)
		if (isnan(endSlope))
		{
			dx1 = x[n - 1] - x[n - 2];
			dy1 = y[n - 1] - y[n - 2];
			tdm->A[n - 1] = 1.0f / dx1;
			tdm->B[n - 1] = 2.0f * tdm->A[n - 1];
			r[n - 1] = 3 * (dy1 / (dx1 * dx1));
		}
		else
		{
			tdm->B[n - 1] = 1;
			r[n - 1] = endSlope;
		}

		//if (debug) Console.WriteLine("Tri-diagonal matrix:\n{0}", m.ToDisplayString(":0.0000", "  "));
		//if (debug) Console.WriteLine("r: {0}", ArrayUtil.ToString<float>(r));

		// k is the solution to the matrix
		k = tdm->Solve(r, originalarraysize);
		//if (debug) Console.WriteLine("k = {0}", ArrayUtil.ToString<float>(k));

		for (int i = 1; i < n; i++)
		{
			dx1 = x[i] - x[i - 1];
			dy1 = y[i] - y[i - 1];
			a_splinecoeff[i - 1] = k[i - 1] * dx1 - dy1; // equation 10 from the article
			b_splinecoeff[i - 1] = -k[i] * dx1 + dy1; // equation 11 from the article
		}

		//if (debug) Console.WriteLine("a: {0}", ArrayUtil.ToString<float>(a));
		//if (debug) Console.WriteLine("b: {0}", ArrayUtil.ToString<float>(b));
	};

	float* Compute(float* x, float* y, float* xs, int size_orig, int size_upsamp, float startSlope = NAN, float endSlope = NAN, bool debug = false)
	{
		yout = FitAndEval(x, y, xs, size_orig, size_upsamp, startSlope, endSlope, debug);
		return yout;
	}
};

class CubicSplineHandle
{
	private:
		CubicSpline* imp;

	public:

		CubicSplineHandle(const int ns, const int os) : imp(new CubicSpline(ns, os)) {  }

		~CubicSplineHandle()
		{
			free(imp);
		}

		CubicSpline* operator->() { return imp; }
};
