using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Text.RegularExpressions;
using System.Reflection;
using IronPython.Hosting;
using Newtonsoft.Json;
using ScottPlot;

namespace Historical_Data
{
    public partial class Form1
    {
        //---------------------------------------------------------------------------------------------------------------------------------------------
        //	CONSTANTS
        //---------------------------------------------------------------------------------------------------------------------------------------------

        //---------------------------------------------------------------------------------------------------------------------------------------------
        //	PUBLIC
        //---------------------------------------------------------------------------------------------------------------------------------------------

        //---------------------------------------------------------------------------------------------------------------------------------------------
        //	PRIVATE
        //---------------------------------------------------------------------------------------------------------------------------------------------

        //*********************************************************************************************************************************************
        //
        //	CONSTRUCTORS/DESTRUCTORS/CLEANUP
        //
        //*********************************************************************************************************************************************

        //*********************************************************************************************************************************************
        //
        //	PUBLIC
        //
        //*********************************************************************************************************************************************

        private void Create_bnsPlot()
        {
            //string python = @"C:\\Users\\Dana\\anaconda3\\python.exe";
            string x = (string)listBox1.SelectedItem;
            string y = (string)listBox2.SelectedItem;
            double[] Time = new double[bnsDataStructureList.Count];
            double[] LForce = new double[bnsDataStructureList.Count];

            double[] x_data = new double[bnsDataStructureList.Count];
            double[] y_data = new double[bnsDataStructureList.Count];

            bnsDataStructure bds = new bnsDataStructure();
            for (int k = 0; k < bnsDataStructureList.Count; k++)
            {
                switch (listBox1.SelectedIndex) // switch for x axis data
                {
                    case 0: // Time field
                        x_data[k] = Convert.ToDouble(bnsDataStructureList[k].Time.ToOADate());
                        if (k == bnsDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: true);
                            formsPlot1.plt.XLabel("Date");
                        }
                        break;
                    case 1: // Trax field
                        x_data[k] = Convert.ToDouble(bnsDataStructureList[k].Trax);
                        if (k == bnsDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Trax (-)");
                        }
                        break;
                    case 2: // Crax field
                        x_data[k] = Convert.ToDouble(bnsDataStructureList[k].Crax);
                        if (k == bnsDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Crax (-)");
                        }
                        break;
                    case 3: // VLoad field
                        x_data[k] = Convert.ToDouble(bnsDataStructureList[k].VLoad);
                        if (k == bnsDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Vertical Load (kips)");
                        }
                        break;
                    case 4: //LForce field
                        x_data[k] = Convert.ToDouble(bnsDataStructureList[k].LForce);
                        if (k == bnsDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Lateral Load (kips)");
                        }
                        break;
                    case 5: // TSLV field
                        x_data[k] = Convert.ToDouble(bnsDataStructureList[k].TSLV);
                        if (k == bnsDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("TSLV (-)");
                        }
                        break;
                    case 6: // SWLV field
                        x_data[k] = Convert.ToDouble(bnsDataStructureList[k].SWLV);
                        if (k == bnsDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("SWLV (-)");
                        }
                        break;
                    case 7: // ASLV field
                        x_data[k] = Convert.ToDouble(bnsDataStructureList[k].ASLV);
                        if (k == bnsDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("ASLV (-)");
                        }
                        break;
                    case 8: // AOA field
                        x_data[k] = Convert.ToDouble(bnsDataStructureList[k].AOA);
                        if (k == bnsDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("AOA (-)");
                        }
                        break;
                }

                switch (listBox2.SelectedIndex) // switch for y axis data
                {
                    case 0: // Time field
                        y_data[k] = bnsDataStructureList[k].Time.ToOADate();
                        if (k == bnsDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: true);
                            formsPlot1.plt.YLabel("Date (-)");
                        }
                        break;
                    case 1: // Trax field
                        y_data[k] = Convert.ToDouble(bnsDataStructureList[k].Trax);
                        if (k == bnsDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Trax (-)");
                        }
                        break;
                    case 2: // Crax field
                        y_data[k] = Convert.ToDouble(bnsDataStructureList[k].Crax);
                        if (k == bnsDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Crax (-)");
                        }
                        break;
                    case 3: // VLoad field
                        y_data[k] = Convert.ToDouble(bnsDataStructureList[k].VLoad);
                        if (k == bnsDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Vertical Load (kips)");
                        }
                        break;
                    case 4: //LForce field
                        y_data[k] = Convert.ToDouble(bnsDataStructureList[k].LForce);
                        if (k == bnsDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Lateral Load (kips)");
                        }
                        break;
                    case 5: // TSLV field
                        y_data[k] = Convert.ToDouble(bnsDataStructureList[k].TSLV);
                        if (k == bnsDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("TSLV (-)");
                        }
                        break;
                    case 6: // SWLV field
                        y_data[k] = Convert.ToDouble(bnsDataStructureList[k].SWLV);
                        if (k == bnsDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("SWLV (-)");
                        }
                        break;
                    case 7: // ASLV field
                        y_data[k] = Convert.ToDouble(bnsDataStructureList[k].ASLV);
                        if (k == bnsDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("ASLV (-)");
                        }
                        break;
                    case 8: // AOA field
                        y_data[k] = Convert.ToDouble(bnsDataStructureList[k].AOA);
                        if (k == bnsDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("AOA (-)");
                        }
                        break;
                }

                label3.Text = bnsDataStructureList[k].Time.ToString();
                //BeginInvoke(m_barDelegate2, bnsDataStructureList.Count());
                label3.Update();
            }
            formsPlot1.plt.Clear();
            if (checkBox3.Checked)
            {
                formsPlot1.plt.PlotScatter(x_data, y_data, lineWidth: 0);
            }
            if (checkBox4.Checked)
            {
                var hist = new ScottPlot.Statistics.Histogram(x_data, min: x_data.Min(), max: x_data.Max());
                double barWidth = hist.binSize * 1.2; // slightly over-side to reduce anti-alias rendering artifacts
                formsPlot1.plt.PlotBar(hist.bins, hist.countsFrac, barWidth: barWidth, outlineWidth: 0);
                formsPlot1.plt.PlotScatter(hist.bins, hist.countsFracCurve, markerSize: 0, lineWidth: 2, color: Color.Black);
                formsPlot1.plt.YLabel("Frequency (fraction)");
                formsPlot1.plt.Axis(null, null, 0, null);
                formsPlot1.plt.Grid(lineStyle: LineStyle.Dot);
            }
            formsPlot1.Render();
        }

        private void Create_bnwPlot()
        {
            string x = (string)listBox1.SelectedItem;
            string y = (string)listBox2.SelectedItem;
            double[] Time = new double[bnwDataStructureList.Count];
            double[] LForce = new double[bnwDataStructureList.Count];

            double[] x_data = new double[bnwDataStructureList.Count];
            double[] y_data = new double[bnwDataStructureList.Count];

            bnsDataStructure bds = new bnsDataStructure();
            for (int k = 0; k < bnwDataStructureList.Count; k++)
            {
                switch (listBox1.SelectedIndex) // switch for x axis data
                {
                    case 0: // Time field
                        x_data[k] = Convert.ToDouble(bnwDataStructureList[k].Time.ToOADate());
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: true);
                            formsPlot1.plt.XLabel("Date");
                        }
                        break;
                    case 1: // Track Number
                        x_data[k] = Convert.ToDouble(bnwDataStructureList[k].TrackNumber);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Track Number (-)");
                        }
                        break;
                    case 2: // Train Speed
                        x_data[k] = Convert.ToDouble(bnwDataStructureList[k].TrainSpeed);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Train Speed (mph)");
                        }
                        break;
                    case 3: // Locos
                        x_data[k] = Convert.ToDouble(bnwDataStructureList[k].Locos);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("# Locos (-)");
                        }
                        break;
                    case 4: // SlaveLocos
                        x_data[k] = Convert.ToDouble(bnwDataStructureList[k].SlaveLocos);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("# SlaveLocos (-)");
                        }
                        break;
                    case 5: // Cars
                        x_data[k] = Convert.ToDouble(bnwDataStructureList[k].Cars);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("# Cars (-)");
                        }
                        break;
                    case 6: // LocoAxles
                        x_data[k] = Convert.ToDouble(bnwDataStructureList[k].LocoAxles);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("# Loco Axles (-)");
                        }
                        break;
                    case 7: // SlaveAxles
                        x_data[k] = Convert.ToDouble(bnwDataStructureList[k].SlaveAxles);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("# SlaveLoco Axles (-)");
                        }
                        break;
                    case 8: // CarAxles
                        x_data[k] = Convert.ToDouble(bnwDataStructureList[k].CarAxles);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("# Car Axles (-)");
                        }
                        break;
                    case 9: // LocoTon
                        x_data[k] = Convert.ToDouble(bnwDataStructureList[k].LocoTon);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Loco Weight (tons)");
                        }
                        break;
                    case 10: // CarTon
                        x_data[k] = Convert.ToDouble(bnwDataStructureList[k].CarTon);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Car Weight (tons)");
                        }
                        break;
                    case 11: // ExtTemp
                        x_data[k] = Convert.ToDouble(bnwDataStructureList[k].ExtTemp);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Externa Temp (deg F)");
                        }
                        break;
                    case 12: // IntTemp
                        x_data[k] = Convert.ToDouble(bnwDataStructureList[k].IntTemp);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Internal Temp (deg F)");
                        }
                        break;
                    case 13: // Relative Humidity
                        x_data[k] = Convert.ToDouble(bnwDataStructureList[k].RelHum);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Relative Humidity (%)");
                        }
                        break;
                    case 14: // Wind Speed
                        x_data[k] = Convert.ToDouble(bnwDataStructureList[k].WindSpd);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Wind Speed (mph)");
                        }
                        break;
                    case 15: // Wind Direction
                        x_data[k] = Convert.ToDouble(bnwDataStructureList[k].WindDir);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Wind Direction (deg)");
                        }
                        break;
                    case 16: // Vehicle Number
                        x_data[k] = Convert.ToDouble(bnwDataStructureList[k].VehicleNumber);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Vehicle Number (-)");
                        }
                        break;
                    case 17: // Car Weight
                        x_data[k] = Convert.ToDouble(bnwDataStructureList[k].CarWeight);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Car Weight (ton)");
                        }
                        break;
                    case 18: // Car Confidence
                        x_data[k] = Convert.ToDouble(bnwDataStructureList[k].CarConfidence);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Car Confidence (-)");
                        }
                        break;
                    case 19: // Car Truck Count
                        x_data[k] = Convert.ToDouble(bnwDataStructureList[k].CarTruckCount);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Car Truck Count (-)");
                        }
                        break;
                    case 20: // Car Axle Count
                        x_data[k] = Convert.ToDouble(bnwDataStructureList[k].CarAxleCount);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Car Axle Count (-)");
                        }
                        break;
                    case 21: // Car Order Number
                        x_data[k] = Convert.ToDouble(bnwDataStructureList[k].CarOrderNum);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Car Order Number (-)");
                        }
                        break;
                    case 22: // Car Speed
                        x_data[k] = Convert.ToDouble(bnwDataStructureList[k].CarSpeed);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Car Speed (mph)");
                        }
                        break;
                    case 23: // Truck Weight
                        x_data[k] = Convert.ToDouble(bnwDataStructureList[k].TruckWeight);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Truck Weight (ton)");
                        }
                        break;
                    case 24: // Truck Confidence
                        x_data[k] = Convert.ToDouble(bnwDataStructureList[k].TruckConfidence);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Truck Confidence (-)");
                        }
                        break;
                    case 25: // Wheel Space
                        x_data[k] = Convert.ToDouble(bnwDataStructureList[k].Wheelspace);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Wheel Space (in)");
                        }
                        break;
                    case 26: // Hunting Index
                        x_data[k] = Convert.ToDouble(bnwDataStructureList[k].HuntingIndex);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Hunting Index (-)");
                        }
                        break;
                    case 27: // Vehicle Axle Number
                        x_data[k] = Convert.ToDouble(bnwDataStructureList[k].VehicleAxleNum);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Vehicle Axle # (-)");
                        }
                        break;
                    case 28: // Average Verticle Force
                        x_data[k] = Convert.ToDouble(bnwDataStructureList[k].AvgVertF);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Average Vertical Force (kips)");
                        }
                        break;
                    case 29: // Maximum Vertical Force
                        x_data[k] = Convert.ToDouble(bnwDataStructureList[k].MaxVertF);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Maximum Vertical Force (kips)");
                        }
                        break;
                    case 30: // Average Lateral Force
                        x_data[k] = Convert.ToDouble(bnwDataStructureList[k].AvgLatF);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Average Lateral Force (kips)");
                        }
                        break;
                    case 31: // Maximum Lateral Force
                        x_data[k] = Convert.ToDouble(bnwDataStructureList[k].MaxLatF);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Maximum Lateral Force (kips)");
                        }
                        break;
                }

                switch (listBox2.SelectedIndex) // switch for y axis data
                {
                    case 0: // Time field
                        y_data[k] = Convert.ToDouble(bnwDataStructureList[k].Time.ToOADate());
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: true);
                            formsPlot1.plt.YLabel("Date");
                        }
                        break;
                    case 1: // Track Number
                        y_data[k] = Convert.ToDouble(bnwDataStructureList[k].TrackNumber);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Track Number (-)");
                        }
                        break;
                    case 2: // Train Speed
                        y_data[k] = Convert.ToDouble(bnwDataStructureList[k].TrainSpeed);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Train Speed (mph)");
                        }
                        break;
                    case 3: // Locos
                        y_data[k] = Convert.ToDouble(bnwDataStructureList[k].Locos);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("# Locos (-)");
                        }
                        break;
                    case 4: // SlaveLocos
                        y_data[k] = Convert.ToDouble(bnwDataStructureList[k].SlaveLocos);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("# SlaveLocos (-)");
                        }
                        break;
                    case 5: // Cars
                        y_data[k] = Convert.ToDouble(bnwDataStructureList[k].Cars);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("# Cars (-)");
                        }
                        break;
                    case 6: // LocoAxles
                        y_data[k] = Convert.ToDouble(bnwDataStructureList[k].LocoAxles);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("# Loco Axles (-)");
                        }
                        break;
                    case 7: // SlaveAxles
                        y_data[k] = Convert.ToDouble(bnwDataStructureList[k].SlaveAxles);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("# SlaveLoco Axles (-)");
                        }
                        break;
                    case 8: // CarAxles
                        y_data[k] = Convert.ToDouble(bnwDataStructureList[k].CarAxles);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("# Car Axles (-)");
                        }
                        break;
                    case 9: // LocoTon
                        y_data[k] = Convert.ToDouble(bnwDataStructureList[k].LocoTon);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Loco Weight (tons)");
                        }
                        break;
                    case 10: // CarTon
                        y_data[k] = Convert.ToDouble(bnwDataStructureList[k].CarTon);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Car Weight (tons)");
                        }
                        break;
                    case 11: // ExtTemp
                        y_data[k] = Convert.ToDouble(bnwDataStructureList[k].ExtTemp);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Externa Temp (deg F)");
                        }
                        break;
                    case 12: // IntTemp
                        y_data[k] = Convert.ToDouble(bnwDataStructureList[k].IntTemp);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Internal Temp (deg F)");
                        }
                        break;
                    case 13: // Relative Humidity
                        y_data[k] = Convert.ToDouble(bnwDataStructureList[k].RelHum);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Relative Humidity (%)");
                        }
                        break;
                    case 14: // Wind Speed
                        y_data[k] = Convert.ToDouble(bnwDataStructureList[k].WindSpd);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Wind Speed (mph)");
                        }
                        break;
                    case 15: // Wind Direction
                        y_data[k] = Convert.ToDouble(bnwDataStructureList[k].WindDir);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Wind Direction (deg)");
                        }
                        break;
                    case 16: // Vehicle Number
                        y_data[k] = Convert.ToDouble(bnwDataStructureList[k].VehicleNumber);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Vehicle Number (-)");
                        }
                        break;
                    case 17: // Car Weight
                        y_data[k] = Convert.ToDouble(bnwDataStructureList[k].CarWeight);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Car Weight (ton)");
                        }
                        break;
                    case 18: // Car Confidence
                        y_data[k] = Convert.ToDouble(bnwDataStructureList[k].CarConfidence);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Car Confidence (-)");
                        }
                        break;
                    case 19: // Car Truck Count
                        y_data[k] = Convert.ToDouble(bnwDataStructureList[k].CarTruckCount);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Car Truck Count (-)");
                        }
                        break;
                    case 20: // Car Axle Count
                        y_data[k] = Convert.ToDouble(bnwDataStructureList[k].CarAxleCount);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Car Axle Count (-)");
                        }
                        break;
                    case 21: // Car Order Number
                        y_data[k] = Convert.ToDouble(bnwDataStructureList[k].CarOrderNum);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Car Order Number (-)");
                        }
                        break;
                    case 22: // Car Speed
                        y_data[k] = Convert.ToDouble(bnwDataStructureList[k].CarSpeed);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Car Speed (mph)");
                        }
                        break;
                    case 23: // Truck Weight
                        y_data[k] = Convert.ToDouble(bnwDataStructureList[k].TruckWeight);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Truck Weight (ton)");
                        }
                        break;
                    case 24: // Truck Confidence
                        y_data[k] = Convert.ToDouble(bnwDataStructureList[k].TruckConfidence);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Truck Confidence (-)");
                        }
                        break;
                    case 25: // Wheel Space
                        y_data[k] = Convert.ToDouble(bnwDataStructureList[k].Wheelspace);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Wheel Space (in)");
                        }
                        break;
                    case 26: // Hunting Index
                        y_data[k] = Convert.ToDouble(bnwDataStructureList[k].HuntingIndex);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Hunting Index (-)");
                        }
                        break;
                    case 27: // Vehicle Axle Number
                        y_data[k] = Convert.ToDouble(bnwDataStructureList[k].VehicleAxleNum);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Vehicle Axle # (-)");
                        }
                        break;
                    case 28: // Average Verticle Force
                        y_data[k] = Convert.ToDouble(bnwDataStructureList[k].AvgVertF);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Average Vertical Force (kips)");
                        }
                        break;
                    case 29: // Maximum Vertical Force
                        y_data[k] = Convert.ToDouble(bnwDataStructureList[k].MaxVertF);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Maximum Vertical Force (kips)");
                        }
                        break;
                    case 30: // Average Lateral Force
                        y_data[k] = Convert.ToDouble(bnwDataStructureList[k].AvgLatF);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Average Lateral Force (kips)");
                        }
                        break;
                    case 31: // Maximum Lateral Force
                        y_data[k] = Convert.ToDouble(bnwDataStructureList[k].MaxLatF);
                        if (k == bnwDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Maximum Lateral Force (kips)");
                        }
                        break;
                }

                label3.Text = bnwDataStructureList[k].Time.ToString();
                label3.Update();
            }
            formsPlot1.plt.Clear();
            if (checkBox3.Checked)
            {
                formsPlot1.plt.PlotScatter(x_data, y_data, lineWidth: 0);
            }
            if (checkBox4.Checked)
            {
                var hist = new ScottPlot.Statistics.Histogram(x_data, min: x_data.Min(), max: x_data.Max());
                double barWidth = hist.binSize * 1.2; // slightly over-side to reduce anti-alias rendering artifacts
                formsPlot1.plt.PlotBar(hist.bins, hist.countsFrac, barWidth: barWidth, outlineWidth: 0);
                formsPlot1.plt.PlotScatter(hist.bins, hist.countsFracCurve, markerSize: 0, lineWidth: 2, color: Color.Black);
                formsPlot1.plt.YLabel("Frequency (fraction)");
                formsPlot1.plt.Axis(null, null, 0, null);
                formsPlot1.plt.Grid(lineStyle: LineStyle.Dot);
            }
            formsPlot1.Render();
        }

        private void Create_trnPlot()
        {
            //string python = @"C:\\Users\\Dana\\anaconda3\\python.exe";
            string x = (string)listBox1.SelectedItem;
            string y = (string)listBox2.SelectedItem;

            double[] x_data = new double[trnDataStructureList.Count];
            double[] y_data = new double[trnDataStructureList.Count];

            //trnDataStructure trn = new trnDataStructure();
            for (int k = 0; k < trnDataStructureList.Count; k++)
            {
                switch (listBox1.SelectedIndex) // switch for x axis data
                {
                    case 0: // Time field
                        x_data[k] = Convert.ToDouble(trnDataStructureList[k].PeakTime.ToOADate());
                        if (trnDataStructureList[k].PeakTime.Year < 2000)
                        {
                            x_data[k] = Double.NaN;
                        }
                        if (k == trnDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: true);
                            formsPlot1.plt.XLabel("Date");
                        }
                        break;
                    case 1: // Peak Number
                        x_data[k] = Convert.ToDouble(trnDataStructureList[k].PeakNumber);
                        if (k == trnDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Peak Number (-)");
                        }
                        break;
                    case 2: // Maximum Vertical Force
                        x_data[k] = Convert.ToDouble(trnDataStructureList[k].MaxVertF);
                        if (k == trnDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Max Vert Force (kips)");
                        }
                        break;
                    case 3: // Maximum Lateral Force
                        x_data[k] = Convert.ToDouble(trnDataStructureList[k].MaxLatF);
                        if (k == trnDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Max Lat Force (kips)");
                        }
                        break;
                    case 4: // Angle of Attack Time
                        x_data[k] = Convert.ToDouble(trnDataStructureList[k].AOAtime);
                        if (k == trnDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("AOA Time (-)");
                        }
                        break;
                    case 5: // LPVert
                        x_data[k] = Convert.ToDouble(trnDataStructureList[k].LPVert);
                        if (k == trnDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("LPVert (-)");
                        }
                        break;
                    case 6: // Track Type
                        try
                        {
                            if (trnDataStructureList[k].TrackType != null)
                            {
                                if (trnDataStructureList[k].TrackType.Contains("Tangent"))
                                {
                                    x_data[k] = 1;
                                }
                                if ((trnDataStructureList[k].TrackType.Contains("EastCurve")))
                                {
                                    x_data[k] = 2;
                                }
                                if (trnDataStructureList[k].TrackType.Contains("WestCurve"))
                                {
                                    x_data[k] = 3;
                                }
                            }
                            else { x_data[k] = Double.NaN; }
                        }
                        catch { x_data[k] = 0; }
                        if (k == trnDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Track Type (1=Tangent, 2=EastCurve, 3=WestCurve)");
                        }
                        break;
                    case 7: // Crib Number
                        x_data[k] = Convert.ToDouble(trnDataStructureList[k].CribNumber);
                        if (k == trnDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Crib Number (-)");
                        }
                        break;
                    case 8: // Rail Type
                        if (trnDataStructureList[k].RailType != null)
                        {
                            if (trnDataStructureList[k].RailType.Contains("Near"))
                            {
                                x_data[k] = 1;
                            }
                            if (trnDataStructureList[k].RailType.Contains("Far"))
                            {
                                x_data[k] = 2;
                            }
                        }
                        if (k == trnDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Rail Type (1=Near, 2=Far)");
                        }
                        break;
                    case 9: // Orientation
                        if (trnDataStructureList[k].Orientation != null)
                        {
                            if (trnDataStructureList[k].Orientation.Contains("Vertical"))
                            {
                                x_data[k] = 1;
                            }
                            if (trnDataStructureList[k].Orientation.Contains("Lateral"))
                            {
                                x_data[k] = 2;
                            }
                        }
                        else { x_data[k] = Double.NaN; }
                        if (k == trnDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Orientation (1=Vertical, 2=Lateral)");
                        }
                        break;
                    case 10: // AxleCount
                        x_data[k] = Convert.ToDouble(trnDataStructureList[k].AxleCount);
                        if (k == trnDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Total Axle Count (-)");
                        }
                        break;
                    case 11: // Position
                        x_data[k] = Convert.ToDouble(trnDataStructureList[k].Position);
                        if (k == trnDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Car Position (-)");
                        }
                        break;
                    case 12: // Car End
                        if (trnDataStructureList[k].CarEnd != null)
                        {
                            if (trnDataStructureList[k].CarEnd.Contains("A"))
                            {
                                x_data[k] = 1;
                            }
                            if (trnDataStructureList[k].CarEnd.Contains("B"))
                            {
                                x_data[k] = 2;
                            }
                        }
                        else { x_data[k] = Double.NaN; }
                        if (k == trnDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Car End (1=A, 2=B)");
                        }
                        break;
                    case 13: // Total Axles
                        x_data[k] = Convert.ToDouble(trnDataStructureList[k].TotalTrainAxle);
                        if (k == trnDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeX: false);
                            formsPlot1.plt.XLabel("Total Train Axles (-)");
                        }
                        break;
                }

                switch (listBox2.SelectedIndex) // switch for y axis data
                {
                    case 0: // Time field
                        y_data[k] = Convert.ToDouble(trnDataStructureList[k].PeakTime.ToOADate());
                        if (trnDataStructureList[k].PeakTime.Year < 2000)
                        {
                            y_data[k] = Double.NaN;
                        }
                        if (k == trnDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: true);
                            formsPlot1.plt.YLabel("Date");
                        }
                        break;
                    case 1: // Peak Number
                        y_data[k] = Convert.ToDouble(trnDataStructureList[k].PeakNumber);
                        if (k == trnDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Peak Number (-)");
                        }
                        break;
                    case 2: // Maximum Vertical Force
                        y_data[k] = Convert.ToDouble(trnDataStructureList[k].MaxVertF);
                        if (trnDataStructureList[k].MaxVertF == 0)
                        {
                            y_data[k] = Double.NaN;
                        }
                        if (k == trnDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Max Vert Force (kips)");
                        }
                        break;
                    case 3: // Maximum Lateral Force
                        y_data[k] = Convert.ToDouble(trnDataStructureList[k].MaxLatF);
                        if (k == trnDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Max Lat Force (kips)");
                        }
                        break;
                    case 4: // Angle of Attack Time
                        y_data[k] = Convert.ToDouble(trnDataStructureList[k].AOAtime);
                        if (k == trnDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("AOA Time (-)");
                        }
                        break;
                    case 5: // LPVert
                        y_data[k] = Convert.ToDouble(trnDataStructureList[k].LPVert);
                        if (k == trnDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("LPVert (-)");
                        }
                        break;
                    case 6: // Track Type
                        try
                        {
                            if (trnDataStructureList[k].TrackType != null)
                            {
                                if (trnDataStructureList[k].TrackType.Contains("Tangent"))
                                {
                                    y_data[k] = 1;
                                }
                                if (trnDataStructureList[k].TrackType.Contains("EastCurve") || trnDataStructureList[k].TrackType.Contains("WestCurve"))
                                {
                                    y_data[k] = 2;
                                }
                            }
                            else { y_data[k] = Double.NaN; }
                        }
                        catch { y_data[k] = 0; }
                        if (k == trnDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Track Type (1=Tangent, 2=Curve)");
                        }
                        break;
                    case 7: // Crib Number
                        y_data[k] = Convert.ToDouble(trnDataStructureList[k].CribNumber);
                        if (k == trnDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Crib Number (-)");
                        }
                        break;
                    case 8: // Rail Type
                        if (trnDataStructureList[k].RailType != null)
                        {
                            if (trnDataStructureList[k].RailType.Contains("Near"))
                            {
                                y_data[k] = 1;
                            }
                            if (trnDataStructureList[k].RailType.Contains("Far"))
                            {
                                y_data[k] = 2;
                            }
                        }
                        if (k == trnDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Rail Type (1=Near, 2=Far)");
                        }
                        break;
                    case 9: // Orientation
                        if (trnDataStructureList[k].Orientation != null)
                        {
                            if (trnDataStructureList[k].Orientation.Contains("Vertical"))
                            {
                                y_data[k] = 1;
                            }
                            if (!trnDataStructureList[k].TrackType.Contains("Lateral"))
                            {
                                y_data[k] = 2;
                            }
                        }
                        if (k == trnDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Orientation (1=Vertical, 2=Lateral)");
                        }
                        break;
                    case 10: // AxleCount
                        y_data[k] = Convert.ToDouble(trnDataStructureList[k].AxleCount);
                        if (k == trnDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Total Axle Count (-)");
                        }
                        break;
                    case 11: // Position
                        y_data[k] = Convert.ToDouble(trnDataStructureList[k].Position);
                        if (k == trnDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Car Position (-)");
                        }
                        break;
                    case 12: // Car End
                        if (trnDataStructureList[k].CarEnd != null)
                        {
                            if (trnDataStructureList[k].CarEnd.Contains("A"))
                            {
                                y_data[k] = 1;
                            }
                            if (trnDataStructureList[k].CarEnd.Contains("B"))
                            {
                                y_data[k] = 2;
                            }
                        }
                        else { y_data[k] = Double.NaN; }
                        if (k == trnDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Car End (1=A, 2=B)");
                        }
                        break;
                    case 13: // Total Axles
                        y_data[k] = Convert.ToDouble(trnDataStructureList[k].TotalTrainAxle);
                        if (k == trnDataStructureList.Count - 1)
                        {
                            formsPlot1.plt.Ticks(dateTimeY: false);
                            formsPlot1.plt.YLabel("Total Train Axles (-)");
                        }
                        break;
                }

                label3.Text = trnDataStructureList[k].PeakTime.ToString();
                label3.Update();
            }
            formsPlot1.plt.Clear();
            if (checkBox3.Checked)
            {
                formsPlot1.plt.PlotScatter(x_data, y_data, lineWidth: 0);
            }
            if (checkBox4.Checked)
            {
                var hist = new ScottPlot.Statistics.Histogram(x_data, min: x_data.Min(), max: x_data.Max());
                double barWidth = hist.binSize * 1.2; // slightly over-side to reduce anti-alias rendering artifacts
                formsPlot1.plt.PlotBar(hist.bins, hist.countsFrac, barWidth: barWidth, outlineWidth: 0);
                formsPlot1.plt.PlotScatter(hist.bins, hist.countsFracCurve, markerSize: 0, lineWidth: 2, color: Color.Black);
                formsPlot1.plt.YLabel("Frequency (fraction)");
                formsPlot1.plt.Axis(null, null, 0, null);
                formsPlot1.plt.Grid(lineStyle: LineStyle.Dot);
            }
            formsPlot1.Render();
        }
    }
}
