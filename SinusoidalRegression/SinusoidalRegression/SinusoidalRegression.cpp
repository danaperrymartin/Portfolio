#include <iostream>
#include <cmath>
#include <algorithm>
#include <numeric>
#include <vector>
#include <string>

#include <fstream>
#include <sstream>

using namespace std;

// Function to calculate the predicted value using the model
double predict(double x, double a, double b, double c, double d) 
{
    return a * sin(b * x + c) + d;
}

// Function to calculate the sum of squares error
double calcError(vector<pair<double, double>> data, double a, double b, double c, double d) 
{
    double error = 0.0;
    for (auto [x, y] : data) {
        error += pow(y - predict(x, a, b, c, d), 2);
    }
    return error;
}

vector<string> split(const std::string& s, char delim, std::vector<std::string> row_values)
{
    std::stringstream ss;
    ss.str(s);
    std::string item;
    while (getline(ss, item, delim)) 
    {
        row_values.push_back(item);
    }

    return row_values;
}


int main() 
{
    vector<pair<double, double>> data;// = { {1, 2}, {2, 3}, {3, 2}, {4, 1}, {5, 0} };

    fstream rawdatafile;
    rawdatafile.open("RawData.dat", ios::in); //open a file to perform read operation using file object
    if (rawdatafile.is_open()) 
    { //checking whether the file is open
        string tp;
        int icount = 0;
        while (getline(rawdatafile, tp)) 
        { //read data from file object and put it into string.
            cout << tp << "\n"; //print the data of the string
            if (icount > 0)
            {
                vector<string> row_values;
                row_values = split(tp, '\t', row_values);
                data.push_back(make_pair(stod(row_values[0]), stod(row_values[1])));
            }
            icount++;
        }
        rawdatafile.close(); //close the file object.
    }

    double a = 0.5, b = 1, c = 0.0, d = 0.0;
    double error = calcError(data, a, b, c, d);

    double learning_rate = 0.01;
    int max_iterations = 100000;
    for (int i = 0; i < max_iterations; i++) {
        //calculate gradient for each parameter
        double grad_a = 0.0, grad_b = 0.0, grad_c = 0.0, grad_d = 0.0;
        for (auto [x, y] : data) {
            grad_a += -2 * (y - predict(x, a, b, c, d)) * sin(b * x + c);
            grad_b += -2 * (y - predict(x, a, b, c, d)) * a * x * cos(b * x + c);
            grad_c += -2 * (y - predict(x, a, b, c, d)) * a * cos(b * x + c);
            grad_d += -2 * (y - predict(x, a, b, c, d));
        }
        //update parameter with gradient
        a -= learning_rate * grad_a;
        b -= learning_rate * grad_b;
        c -= learning_rate * grad_c;
        d -= learning_rate * grad_d;

        double new_error = calcError(data, a, b, c, d);
        // check if error is decreasing
        if (abs(error - new_error) < 1e-6) {
            break;
        }
        error = new_error;
    }
    cout << "a: " << a << endl;
    cout << "b: " << b << endl;
    cout << "c: " << c << endl;
    cout << "d: " << d << endl;

    ofstream datafile("DataFile.dat");
    for (int idat = 0; idat < data.size(); idat++)
    {
        if (idat == 0)
        {
            datafile << ((string)"x" + "\t" + (string)"y") << "\n";
        }
        datafile << to_string(data[idat].first) + "\t" + to_string(data[idat].second) << "\n";
    }
    datafile.close();

    ofstream regressionfile("RegressionValue.reg");
    for (int iparam = 0; iparam < 1; iparam++)
    {
        if(iparam == 0)
        {
            regressionfile << ("f(x) = a* sin(b * x + c) + d") << "\n";
            regressionfile << ((string)"a" + "\t" + (string)"b" + "\t" + (string)"c" + "\t" + (string)"d") << "\n";
        }
        regressionfile << to_string(a) + "\t" + to_string(b) + "\t" + to_string(c) + "\t" + to_string(d) << "\n";
    }
    regressionfile.close();

    return 0;
}