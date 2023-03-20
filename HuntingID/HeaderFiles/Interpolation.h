#pragma once
#include <stdlib.h>     /* malloc, calloc, realloc, free */
#include <string>
#include <filesystem>
#include <fstream>

#include "mkl.h"      // Interpolation

#define SPLINE_ORDER DF_PP_CUBIC       /* A cubic spline to construct */
//#define UPSAMPLEFACTOR 100

class Interpolation
{
    private:
        const int valid_cribs;
        const int UPSAMPLEFACTOR;
        MKL_INT NX;
        int NSITE;
        float* datax;
        float* datay;
        
    public:
        float* r;
        float* x_interp;
        float* out;
        float* scoeff;
        float* ic;

        /*Interpolation() : valid_cribs(), UPSAMPLEFACTOR()
        {
        }*/

        Interpolation(const int num_cribs, const int upsamp) : valid_cribs(num_cribs), UPSAMPLEFACTOR(upsamp)
        {
            datax = new float[NX] {0.0};
            datay = new float[NX] {0.0};
            NX = valid_cribs;                          /* Size of partition, number of breakpoints */
            NSITE = valid_cribs * UPSAMPLEFACTOR;      /* Number of interpolation sites */
            r = new float[NSITE] {0.0};
            x_interp = new float[NSITE] {0.0};
            out = new float[NSITE] {0.0};
            scoeff = new float[(NX - 1) * SPLINE_ORDER];
            ic = NULL;
        }

        ~Interpolation()
        {
            delete[] datax;
            delete[] datay;
            delete[] r;
            delete[] x_interp;
            delete[] out;
            delete[] scoeff;
            delete[] ic;

           /* r=NULL;
            datax=NULL;
            datay=NULL;
            x_interp=NULL;
            out=NULL;
            scoeff=NULL;
            delete[] ic;*/
            //mkl_free_buffers();
        }

        void ClearInterpolation()
        {
            /*free(r);
            free(datax);
            free(datay);
            free(x_interp);
            free(out);
            free(scoeff);
            free(ic);*/

            r = NULL;
            datax = NULL;
            datay = NULL;
            x_interp = NULL;
            out = NULL;
            scoeff = NULL;
            ic = NULL;
            //mkl_free_buffers();
        }

        void freeMKL()
        {
            mkl_free_buffers();
        }

        void MKLInterpolation(double* datain_x, double* datain_y, std::filesystem::path &save_directory, std::string &trn_car)
        {
            datax = new float[NX] {0.0};
            datay = new float[NX] {0.0};

            for (int i = 0; i < NX; i++)
            {
                datax[i] = (float)(datain_x[i]-datain_x[0]);
                datay[i] = (float)datain_y[i];
            }
            
            x_interp = new float[NSITE] {0};
            out = new float[NSITE] {0.0};
            std::vector<pair<float, float>> vect(NSITE);
            
            /*--Upsample the data--*/
            x_interp = upsample(datax[0], datax[NX-1]);

            MKL_LONG status = 0;
            
            MKL_INT xhint = DF_NON_UNIFORM_PARTITION;   /* The partition is non-uniform. */;
            MKL_INT ny = 1;                             /*Linear function*/
            MKL_INT yhint = DF_NO_HINT;                 /* No additional information about the function is provided. */
            DFTaskPtr task = NULL;
            
            /* Create a Data Fitting task */
            status = dfsNewTask1D(&task, NX, datax, xhint, ny, datay, yhint);
            
            const MKL_INT s_order = SPLINE_ORDER; //DF_PP_LINEAR;          /* Spline is of the fourth order (cubic spline). */
            const MKL_INT s_type = DF_PP_BESSEL; //(use with cubic)      //DF_PP_DEFAULT (use w/ quad)     /* Spline is of the Bessel cubic type. */
            const float* bc = NULL;                                        /* No boundary conditions */
            const MKL_INT bc_type =  DF_BC_NOT_A_KNOT;
            MKL_INT ic_type = DF_NO_IC;
            ic = NULL;                                             /* Define internal conditions for cubic spline construction (none in this example) */
            scoeff = new float[(NX - 1) * SPLINE_ORDER];           /* Array of spline coefficients */
            MKL_INT scoeffhint = DF_NO_HINT;                              /* No additional information about the spline. */
            
            /* Set spline parameters in the Data Fitting task */
            status = dfsEditPPSpline1D(task, s_order, s_type, bc_type, bc, ic_type, ic, scoeff, scoeffhint);

            /* Construct a cubic Bessel spline: Pi(x) = c1,i + c2,i(x - xi) + c3,i(x - xi)^2 + c4,i(x - xi)^3; the library packs spline
            coefficients to scoeff: scoeff[4*i+0] = c1,i, scoef[4*i+1] = c2,i, scoeff[4*i+2] = c3,i, scoef[4*i+1] = c4,i */
            status = dfsConstruct1D(task, DF_PP_SPLINE, DF_METHOD_STD);

            MKL_INT sitehint = DF_NON_UNIFORM_PARTITION;     /* Partition of sites is non-uniform */
            MKL_INT dorder[SPLINE_ORDER]{ 0 };             /* Array of size ndorder that defines the order of the derivatives to be computed at the interpolation sites. If all the elements in dorder are zero, the library computes the spline values only. If you do not need interpolation computations, set ndorder to zero and pass a NULL pointer to dorder. */
            MKL_INT ndorder = SPLINE_ORDER;                  /* Cubic interpolation */
            const float* datahint = DF_NO_APRIORI_INFO;      /* No additional information about breakpoints or sites is provided. */
            MKL_INT rhint = DF_MATRIX_STORAGE_ROWS;          /* The library packs interpolation results in row-major format. */
            MKL_INT* cell = NULL;                            /* Cell indices are not required. */

            /* Compute the spline values at the points site(i), i=0,..., nsite-1 and place the results to array r */
            status = dfsInterpolate1D(task, DF_INTERP, DF_METHOD_PP, NSITE, x_interp, sitehint, ndorder, dorder, datahint, r, rhint, cell);
            status = dfDeleteTask(&task);

            for (int i = 0; i < NSITE; i++)
            {
                vect[i] = make_pair(x_interp[i], r[i]);
            }
            
            if (!std::filesystem::exists(save_directory))
            {
                std::filesystem::create_directory(save_directory);
            }

            ofstream mklfile(save_directory.string() + trn_car + "MKLInterpolatedValues.interp");
            for (int count = 0; count < NSITE; count++)
            {
                if (count == 0)
                {
                    mklfile << ((string)"Interpolation Time (s)" + "\t" + (string)"Interpolated Value (kips)") << "\n";
                }
                mklfile << to_string(x_interp[count]) + "\t" + to_string(r[count]) << "\n";
            }
            mklfile.close();

            ofstream truthfile(save_directory.string() + trn_car + "OriginalValues.interp");
            for (int icrib = 0; icrib < NX; icrib++)
            {
                if (icrib == 0)
                {
                    truthfile << ((string)"Time (s)" + "\t" + (string)"Measured Value (kips)") << "\n";
                }
                truthfile << (to_string(datain_x[icrib]- datain_x[0]) + "\t" + to_string(datain_y[icrib])) << "\n";
            }
            truthfile.close();
        };

        float* upsample(float start_in, float end_in)
        {
            float* interp = new float[NSITE] {0.0};
            float start = static_cast<float>(start_in);
            float end = static_cast<float>(end_in);
            float num = static_cast<float>(NSITE);

            float delta = (end - start) / (num - 1);
            for (int i = 0; i < NSITE; ++i)
            {
                interp[i] = (start + delta * i);
            }
            return interp;
        }     
};

class InterpolationHandle
{
    private:
        

    public:
        Interpolation* ih;
        InterpolationHandle(const int vc, const int upsamp) : ih(new Interpolation(vc, upsamp)) {  }

        void terminateInterpolationHandle()
        {
            ih = NULL;
        }

        ~InterpolationHandle()
        {
            delete ih;
            //free(ih);
        }

        Interpolation* operator->() { return ih; }
};