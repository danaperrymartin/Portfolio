#pragma once
#include <fstream>
#include <complex>
#include <vector>
#include <filesystem>
#include <math.h> 

//#include "Regression.h"

class FastFourierTransform
{
    private:
        const int NSITE;

    public:
        std::string save_directory;
        double* frequency;
        double* magnitude;
        double huntingmetric;

        /*FastFourierTransform():NSITE()
        {};*/

        FastFourierTransform(const int nsite):NSITE(nsite)
        {
            frequency = new double[NSITE]{0.0};
            magnitude = new double[NSITE] {0.0};
            huntingmetric = 0;

            //NX = valid_cribs;                     /* Size of partition, number of breakpoints */
            //NSITE = valid_cribs * UPSAMPLEFACTOR;                /* Number of interpolation sites */
            //memorysize = NSITE * sizeof(float);
            ////cribs = valid_cribs;
            //x = new float[valid_cribs] {0};
            ////float x[cribs]{ 0 };
            ////float y[cribs]{ 0 };
            //y = new float[valid_cribs] {0};
            ////float xs[NSITE]{ 0 };
            //xs = new float[NSITE]{0};
            ////float ys[NSITE]{ 0 };
            //ys = new float[NSITE]{0};

            //r = new float[NSITE]{ y[0] };
            //scoeff = new float[(NX - 1) * SPLINE_ORDER]{0};   /* Array of spline coefficients */
            //nx = valid_cribs;

            //
            //carwhl    = "";
            //vect      = std::vector<pair<float, float>>(valid_cribs);
            //x_complex = std::vector<std::complex<float>>(NX);
            //y_complex = std::vector<std::complex<float>>(NSITE);
            ////freq_mag  = std::vector<float>(NSITE);
            //freq_pwr = std::vector<float>(NSITE);

            //status =  0 ;
            //xhint = DF_NON_UNIFORM_PARTITION;           /* The partition is non-uniform. */
            //ny = 1;                                     /* linear function. */
            //yhint = DF_NO_HINT;                         /* No additional information about the function is provided. */
            //s_order = SPLINE_ORDER; //DF_PP_LINEAR;     /* Spline is of the fourth order (cubic spline). */
            //s_type = DF_PP_BESSEL; // DF_PP_BESSEL;     /* Spline is of the Bessel cubic type. */
            //ic_type = DF_NO_IC;
            //ic = NULL;                                  /* Define internal conditions for cubic spline construction (none in this example) */

            //bc_type = DF_BC_NOT_A_KNOT; //DF_NO_BC; // DF_BC_NOT_A_KNOT;
            //bc = NULL; /* No boundary conditions */
            //scoeffhint = DF_NO_HINT; /* No additional information about the spline. */
            //nsite = NSITE;
            //sitehint = DF_NON_UNIFORM_PARTITION;     /* Partition of sites is non-uniform */
            //ndorder = SPLINE_ORDER-1;                             /* Cubic interpolation */
            //datahint = DF_NO_APRIORI_INFO;      /* No additional information about breakpoints or sites is provided. */
            //rhint = DF_MATRIX_STORAGE_ROWS;          /* The library packs interpolation results in row-major format. */
            //cell = NULL;                            /* Cell indices are not required. */
            //my_desc1_handle = NULL;
            //my_desc2_handle = NULL;

            //start = static_cast<float>(0);
            //end = static_cast<float>(0);
            //num = static_cast<int>(NSITE);
            //delta = 0;
        }

        ~FastFourierTransform()
        {
            //delete &save_directory;
            delete[] frequency;
            delete[] magnitude;
            //delete huntingmetric;

            //mkl_free_buffers();
        }

        void ClearFastFourierTransform()
        {
            /*free(frequency);
            free(magnitude);
            huntingmetric = 0;*/

            frequency = NULL;
            magnitude = NULL;
            huntingmetric = 0;
        }

        void ComputeFastFourierTransform(float* &data_in, std::filesystem::path directory_name, std::string& trn_car)
        {
            std::vector<std::string> tmp = {""};
            /*std::vector<pair<float, float>> vect(NX);
            float* x = new float[NX] {0};
            float* y = new float[NX] {0};
            float* xs = new float[NSITE] {0};
            float* ys = new float[NSITE] {0};*/
            std::vector<std::complex<float>> vectcomplex = std::vector<std::complex<float>>(NSITE);
            //std::vector<std::complex<float>> y_complex = std::vector<std::complex<float>>(NSITE);
            //carwhl = trn_whl;
            //fft = getClassInstance();
            //std::vector<std::complex<float>> x_complex(cribs);
            //double y_double[cribs]{ 0 };
            //std::vector<std::complex<float>> y_complex(cribs);
            save_directory = directory_name.string();

            MKL_LONG status;
            DFTI_DESCRIPTOR_HANDLE my_desc1_handle = NULL;
            DFTI_DESCRIPTOR_HANDLE my_desc2_handle = NULL;

            /*for (int i =0; i < NX; i++)
            {
                tmp = ParseString(dateTime[i], ":");
                vect[i] = make_pair(std::stof(tmp[2]), std::stof(data_in[i]));
            }*/

            // sort vectors
            //std::sort(vect.begin(), vect.end());

            // populate vectors with sorted values
            /*for (int i = 0; i < NX; i++)
            {
                x[i]         = vect[i].first;
                y[i]         = vect[i].second;
            }*/

            // Upsample original time vector for interpolation sites
            //xs = upsample(vect[0].first, vect[NX - 1].first);
            //memcpy(xs, upsample(vect[0].first, vect[data_in.size() - 1].first), memorysize);
            
            // fit cubic spline to data using Intel's MKL
            //Interpolation(x, y, xs, trn_car);

            // fit cubic spline to data using custom class from Wikipedia article https://en.wikipedia.org/wiki/Spline_interpolation
            //CubicSplineHandle cs(NSITE, NX);
            //memcpy(ys, cs->Compute(x, y, xs, NX, NSITE, NAN, NAN, false), NSITE*sizeof(float));
            // 
            //ofstream customclassfile(save_directory.string() + "\\CustomClassInterpolatedValues.interp");
            //for (int count = 0; count < NSITE; count++)
            //{
                //if (count == 0)
                //{
                    //customclassfile << ((string)"Interpolation Time (s)" + "\t" + (string)"Interpolated Value (kips)") << "\n";
                //}
                //customclassfile << to_string(xs[count]) + "\t" + to_string(ys[count]) << "\n";
            //}
            //customclassfile.close();

            int iorig = 0;
            //Populate vectors for DFFT
            for (int i = 0; i < NSITE; i++)
            {
                vectcomplex[i] = std::polar(data_in[i], (float)0);
            }

            // Compute Discrete Fourier Transform using Intel MKL FFT implementation for lateral (humping frequency)
            //status = DftiCreateDescriptor(&my_desc1_handle, DFTI_SINGLE, DFTI_COMPLEX, 1, NX);
            //status = DftiSetValue(my_desc1_handle, DFTI_PLACEMENT, DFTI_NOT_INPLACE); //Out of place FFT
            //status = DftiCommitDescriptor(my_desc1_handle);
            //status = DftiComputeForward(my_desc1_handle, x_complex.data());
            //status = DftiFreeDescriptor(&my_desc1_handle);
            
            // Compute Discrete Fourier Transform using Intel MKL FFT implementation for lateral (hunint frequency)
            status = DftiCreateDescriptor(&my_desc2_handle, DFTI_SINGLE, DFTI_COMPLEX, 1, NSITE);
            //status = DftiSetValue(my_desc1_handle, DFTI_PLACEMENT, DFTI_NOT_INPLACE); //Out of place FFT
            status = DftiCommitDescriptor(my_desc2_handle);
            status = DftiComputeForward(my_desc2_handle, vectcomplex.data());
            status = DftiFreeDescriptor(&my_desc2_handle);

            //ComputeFrequency(x, NX, trn_car, "humpfreq");
            //ComputeFrequencyandMagnitude(x_complex, trn_car, "humpfreq");

            //ComputeFrequency(xs, NSITE, trn_car, "huntfreq");
            ComputeFrequencyandMagnitude(vectcomplex, trn_car, "huntfreq");

           /* ofstream fftParamsOutfile_near(directory_name.string()+"\\DFFTParams_" + trn_car + "_Near.fftdat");
            for (int iprint = 0; iprint < NSITE; iprint++)
            {
                if (iprint == 0)
                {
                    fftParamsOutfile_near << ("Real\tImaginary\n");
                }
                fftParamsOutfile_near << (to_string(y_complex[iprint].real()) + "\t" + to_string(y_complex[iprint].imag()) + "\n");
            }
            fftParamsOutfile_near.close();*/
            
            //return vectcomplex;
        };

        void ComputeFrequencyandMagnitude(std::vector<std::complex<float>> fftcoef, std::string trn_car, std::string type)
        {
            std::vector<pair<float,float>> freq = std::vector< pair<float, float>>(fftcoef.size());
            /*------------------Compute frequency magnitude from DFFT coefficients---------------------*/
            for (size_t ifreq = fftcoef.size()/2; ifreq < fftcoef.size(); ifreq++)
            {
                freq[ifreq] = make_pair(sqrt(fftcoef[ifreq].imag()* fftcoef[ifreq].imag() + fftcoef[ifreq].real()* fftcoef[ifreq].real()), fftcoef[ifreq].imag());
            } 

            /*---------------sort the vector---------------------------*/
            sort(freq.begin(), freq.end());

            ofstream frequencyfile(save_directory + "\\"+ trn_car +type+"_FrequencyInfo.freq");
            int ihunt = 0;
            for (int count = 0; count < fftcoef.size(); count++)
            {
                if (count == 0)
                {
                    frequencyfile << ((string)"Frequency Magnitude" +"\t" +(string)"Frequency (Hz)") << "\n";
                }
                frequencyfile << (to_string(freq[count].first)+"\t"+ to_string(freq[count].second)) << "\n";
                if ((2 < freq[count].second) && (freq[count].second < 4))
                {
                    frequency[ihunt] = freq[count].second;
                    magnitude[ihunt] = freq[count].first;
                    huntingmetric += magnitude[ihunt];
                    ihunt++;
                }
            }
            frequencyfile.close();
            freq.clear();
        }

        //void ComputePowerSpectrum(std::vector<std::complex<float>> fftcoef)
        //{
        //    std::vector<float> freq_pwr = std::vector<float>(NSITE);
        //    /*------------------Compute frequency magnitude from DFFT coefficients---------------------*/
        //    for (int ifreq = 0; ifreq < NSITE; ifreq++)
        //    {
        //        freq_pwr[ifreq] = pow(fftcoef[ifreq].imag(), 2) + pow(fftcoef[ifreq].real(), 2);
        //    }

        //    /*---------------sort the vector---------------------------*/
        //    sort(freq_pwr.begin(), freq_pwr.end());
        //}

        std::vector<std::string> ParseString(std::string s, std::string delimiter)
        {
            size_t start;
            size_t end = 0;
            std::vector<std::string> out;
            while ((start = s.find_first_not_of(delimiter, end)) != std::string::npos)
            {
                end = s.find(delimiter, start);
                out.push_back(s.substr(start, end - start));
            }
            return out;
        };
};

class FastFourierTransformHandle
{
    private:
        

    public:
        FastFourierTransform* fft_h;

        FastFourierTransformHandle(const int NSITE) : fft_h(new FastFourierTransform(NSITE)) {  }

        void terminateFastFourierTransformHandle()
        {
            fft_h = NULL;
        }

        ~FastFourierTransformHandle()
        {
            delete fft_h;
            //free(fft_h);
        }

        FastFourierTransform* operator->() { return fft_h; }
};