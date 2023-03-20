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
    public partial class Form1 : Form
    {
        //-------------------------------------------------------------------------------------------------
        //  PRIVATE
        //-------------------------------------------------------------------------------------------------
        private string SearchResult = null;
        List<string> AllLines;
        List<string> AllFilesList;
        List<bnsDataStructure> bnsDataStructureList;
        List<bnwDataStructure> bnwDataStructureList;
        List<trnDataStructure> trnDataStructureList;

        public delegate void BarDelegate(int lng);
        public delegate void BarDelegate2(int lng);
        public BarDelegate m_barDelegate;
        //public BarDelegate2 m_barDelegate2;
        bool bloaded = false;
        private CheckBox lastChecked;
        private CheckBox lastChecked2;
        //*********************************************************************************************************************************************
        //
        //	CONSTRUCTORS/DESTRUCTORS/CLEANUP
        //
        //*********************************************************************************************************************************************
        public Form1()
        {
            InitializeComponent();
            AllLines = new List<string>();
            bnsDataStructureList = new List<bnsDataStructure>();
            bnwDataStructureList = new List<bnwDataStructure>();
            trnDataStructureList = new List<trnDataStructure>();
            AllFilesList = new List<string>();
            m_barDelegate = new BarDelegate(UpdateBar);
            //m_barDelegate2 = new BarDelegate2(UpdateBar2);
        }
        //*************************************************************************************************
        //
        //  PUBLIC MEMBERS
        //
        //*************************************************************************************************

        //*************************************************************************************************
        //
        //  PRIVATE MEMBERS
        //
        //*************************************************************************************************

        private void btn_Browse_Click(object sender, EventArgs e)
        {
            //DialogResult result = folderBrowserDialog1.ShowDialog(); // Shows the dialog box
            CommonOpenFileDialog dlg = new CommonOpenFileDialog();
            dlg.IsFolderPicker = true;
            
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok) // Test result.
            {
                textBox1.Text = dlg.FileName;
                SearchResult = dlg.FileName;
            }
        }

        private void btn_Run_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(ProcessThread));
        }

        //Run the Processing and GUI on different threads
        private void ProcessThread(object obj)
        {
            if ((bnsDataStructureList.Count == 0 && checkBox1.Checked) || (bnwDataStructureList.Count == 0 && checkBox2.Checked) || (trnDataStructureList.Count == 0 && checkBox5.Checked))
            {
                GetFiles();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (bloaded)
            {
                if (checkBox1.Checked && (checkBox3.Checked || checkBox4.Checked))
                {
                    if (bnsDataStructureList.Count != 0)
                    {
                        Create_bnsPlot();
                    }
                }
                if (checkBox2.Checked && (checkBox3.Checked || checkBox4.Checked))
                {
                    if (bnwDataStructureList.Count != 0)
                    {
                        Create_bnwPlot();
                    }
                }
                if (checkBox5.Checked && (checkBox3.Checked || checkBox4.Checked))
                {
                    if (trnDataStructureList.Count != 0)
                    {
                        Create_trnPlot();
                    }
                }
            }
        }
        private void listBox2_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (bloaded)
            {
                if (checkBox1.Checked && (checkBox3.Checked || checkBox4.Checked))
                {
                    if (bnsDataStructureList.Count != 0)
                    {
                        Create_bnsPlot();
                    }
                }
                if (checkBox2.Checked && (checkBox3.Checked || checkBox4.Checked))
                {
                    if (bnwDataStructureList.Count != 0)
                    {
                        Create_bnwPlot();
                    }
                }
                if (checkBox5.Checked && (checkBox3.Checked || checkBox4.Checked))
                {
                    if (trnDataStructureList.Count != 0)
                    {
                        Create_trnPlot();
                    }
                }
            }
        }
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            bloaded = false;
            bnsDataStructureList = new List<bnsDataStructure>();
            bnwDataStructureList = new List<bnwDataStructure>();
            trnDataStructureList = new List<trnDataStructure>();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            bloaded = false;
            bnsDataStructureList = new List<bnsDataStructure>();
            bnwDataStructureList = new List<bnwDataStructure>();
            trnDataStructureList = new List<trnDataStructure>();
        }
    }
}
