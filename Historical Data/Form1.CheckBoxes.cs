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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox activeCheckBox = sender as CheckBox;
            if (activeCheckBox != lastChecked && lastChecked != null) lastChecked.Checked = false;
            lastChecked = activeCheckBox.Checked ? activeCheckBox : null;

            listBox1.Items.Clear();
            listBox2.Items.Clear();
            string[] ItemsForPlot = { "Time", "Trax", "Crax", "VLoad", "LForce", "TSLV", "SWLV", "ASLV", "AOA" };
            foreach (string ii in ItemsForPlot)
            {
                listBox1.Items.Add(ii);
                listBox2.Items.Add(ii);
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox activeCheckBox = sender as CheckBox;
            if (activeCheckBox != lastChecked && lastChecked != null) lastChecked.Checked = false;
            lastChecked = activeCheckBox.Checked ? activeCheckBox : null;

            listBox1.Items.Clear();
            listBox2.Items.Clear();
            string[] ItemsForPlot = { "Time", "TrackNumber", "TrainSpeed", "Locos", "SlaveLocos", "Cars", "LocoAxles", "SlaveAxles", "CarAxles", "LocoTon", "CarTon", "ExtTemp", "IntTemp", "RelHum", "WindSpd", "WindDir", "VehicleNumber", "CarWeight", "CarConfidence", "CarTruckCount", "CarAxleCount", "CarOrderNum", "CarSpeed", "TruckWeight", "TruckConfidence", "Wheelspace", "HuntingIndex", "VehicleAxleNum", "AvgVertF", "MaxVertF", "AvgLatF", "MaxLatF" };
            foreach (string ii in ItemsForPlot)
            {
                listBox1.Items.Add(ii);
                listBox2.Items.Add(ii);
            }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox activeCheckBox = sender as CheckBox;
            if (activeCheckBox != lastChecked && lastChecked != null) lastChecked.Checked = false;
            lastChecked = activeCheckBox.Checked ? activeCheckBox : null;

            listBox1.Items.Clear();
            listBox2.Items.Clear();
            string[] ItemsForPlot = { "Time", "PeakNumber", "MaxVertForce", "MaxLatForce", "AOATime","LPVert","TrackType","CribNumber","RailType","Orientation","AxleCount","Position","CarEnd","TotalAxles"};
            foreach (string ii in ItemsForPlot)
            {
                listBox1.Items.Add(ii);
                listBox2.Items.Add(ii);
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox activeCheckBox2 = sender as CheckBox;
            if (activeCheckBox2 != lastChecked2 && lastChecked2 != null) lastChecked2.Checked = false;
            lastChecked2 = activeCheckBox2.Checked ? activeCheckBox2 : null;
            formsPlot1.plt.Clear();
            if (checkBox1.Checked)
            {
                if (bnsDataStructureList.Count != 0)
                {
                    Create_bnsPlot();
                }
            }
            if (checkBox2.Checked)
            {
                if (bnwDataStructureList.Count != 0)
                {
                    Create_bnwPlot();
                }
            }
            if (checkBox5.Checked)
            {
                if (trnDataStructureList.Count != 0)
                {
                    Create_trnPlot();
                }
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox activeCheckBox2 = sender as CheckBox;
            if (activeCheckBox2 != lastChecked2 && lastChecked2 != null) lastChecked2.Checked = false;
            lastChecked2 = activeCheckBox2.Checked ? activeCheckBox2 : null;
            formsPlot1.plt.Clear();
            if (checkBox1.Checked)
            {
                if (bnsDataStructureList.Count != 0)
                {
                    Create_bnsPlot();
                }
            }
            if (checkBox2.Checked)
            {
                if (bnwDataStructureList.Count != 0)
                {
                    Create_bnwPlot();
                }
            }
            if (checkBox5.Checked)
            {
                if (trnDataStructureList.Count != 0)
                {
                    Create_trnPlot();
                }
            }
        }
    }
}
