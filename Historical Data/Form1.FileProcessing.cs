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

        private void GetFiles()
        {
            GetAllFiles();
            bnsDataStructure bds = new bnsDataStructure();
            bnwDataStructure bnw = new bnwDataStructure();
            DateTime StartDate = dateTimePicker1.Value;
            DateTime EndDate = dateTimePicker2.Value;
            string[] GlobalDirectory = Directory.GetDirectories(SearchResult);
            bool EOY = false;
            bool EOM = false;
            int icountyear = 0;
            foreach (string iyear in GlobalDirectory.SkipWhile((iyear, i) => String.Compare(iyear, (SearchResult + "\\" + StartDate.Year.ToString()), false) != 0).TakeWhile((iyear, j) => String.Compare(iyear, (SearchResult + "\\" + EndDate.Year.ToString()), true) != 1))
            {
                int icountmonth = 0;
                IEnumerable<string> MonthFolders = Directory.GetDirectories(iyear);
                if (icountyear == 0)
                {
                    IEnumerable<string> newMonthFolders = MonthFolders.SkipWhile((imonth, i) => String.Compare(imonth, (iyear + "\\" + StartDate.Month.ToString("00")), false) != 0);
                    MonthFolders = newMonthFolders;
                }
                if (String.Equals(iyear, (SearchResult + "\\" + EndDate.Year.ToString())))
                {
                    EOY = true;
                }
                if (EOY)
                {
                    IEnumerable<string> newMonthFolders = MonthFolders.TakeWhile((imonth, i) => String.Compare(imonth, (iyear + "\\" + EndDate.Month.ToString("00")), true) != 1);
                    MonthFolders = newMonthFolders;
                }
                icountyear++;
                foreach (string imonth in MonthFolders)
                {
                    int icountday = 0;
                    IEnumerable<string> DayFolders = Directory.GetDirectories(imonth);
                    if (icountmonth == 0 && icountyear == 1)
                    {
                        IEnumerable<string> newDayFolders = DayFolders.SkipWhile((iday, i) => String.Compare(iday, (imonth + "\\" + StartDate.Day.ToString("00")), false) != 0).TakeWhile((iday, i) => String.Compare(iday, (imonth + "\\" + EndDate.Day.ToString("00")), true) != 1);
                        DayFolders = newDayFolders;
                    }

                    string[] monthlim = imonth.Split('\\');
                    if (Convert.ToInt32(monthlim[monthlim.Length - 1]) == EndDate.Month && EOY)
                    {
                        EOM = true;
                    }
                    if (EOM)
                    {
                        IEnumerable<string> newDayFolders = DayFolders.TakeWhile((iday, i) => String.Compare(iday, (imonth + "\\" + EndDate.Day.ToString("00")), true) != 1);
                        DayFolders = newDayFolders;
                    }

                    icountmonth++;
                    foreach (string iday in DayFolders)
                    {
                        try
                        {
                            if (checkBox1.Checked)
                            {
                                string[] SummaryFiles = Directory.GetFiles((iday + "\\Summary\\"), "*.bns");
                                if (SummaryFiles != null)
                                {
                                    Parse_bnsFiles(SummaryFiles);
                                }
                            }
                            if (checkBox2.Checked)
                            {
                                string[] SummaryFiles = Directory.GetFiles((iday + "\\Summary\\"), "*.bnw");
                                if (SummaryFiles != null)
                                {
                                    Parse_bnwFiles(SummaryFiles);
                                }
                            }
                            if (checkBox5.Checked)
                            {
                                string[] SummaryFiles = Directory.GetFiles((iday + "\\Data\\"), "*.trn");
                                if (SummaryFiles != null)
                                {
                                    Parse_trnFiles(SummaryFiles);
                                }
                            }
                        }
                        catch { }
                        BeginInvoke(m_barDelegate, AllFilesList.Count());
                        icountday++;
                    }
                }
            }
            bloaded = true;
            BeginInvoke(m_barDelegate, AllFilesList.Count());
        }

        //-------------------------------------------------------
        // --------------- Code for bns Files-----------------//
        private void Parse_bnsFiles(string[] SummaryFiles)
        {
            bnsDataStructure bds = new bnsDataStructure();

            foreach (string ifile in SummaryFiles)
            {
                bool breakflag = false;
                AllLines.Clear();
                FileInfo fi = new FileInfo(ifile);
                bnsDataStructureList.Add(bds);
                using (StreamReader sr = File.OpenText(ifile))
                {
                    while (!sr.EndOfStream)
                    {
                        AllLines.Add(sr.ReadLine());
                    }
                }
                int x = 0;
                while (!breakflag & x < AllLines.Count)
                {
                    if (AllLines[x].Contains("Crib"))
                    {
                        try
                        {
                            Process_bnsLines(AllLines, fi.LastWriteTimeUtc);
                            breakflag = true;
                        }
                        catch
                        {
                            breakflag = true;
                        }
                    }
                    else
                    {
                        x++;
                    }
                }
            }
        }

        private void Process_bnsLines(List<string> allLines, DateTime fd)
        {
            bnsDataStructure bds = new bnsDataStructure();
            int ii = 0;
            while (ii < allLines.Count)
            {
                if (allLines[ii].Contains("Crib"))
                {
                    string[] tmp = allLines[ii].Split(' ');
                    bds.CribNumber = Convert.ToInt32(tmp[1]);
                    bds.CribDirection = tmp[2];
                    ii++;
                }
                if (allLines[ii].Contains("TIME"))
                {
                    ii++;
                    while (!allLines[ii].Contains("Crib") && !allLines[ii].Equals(""))
                    {
                        string[] datatmp = allLines[ii].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        bds.Time = fd.AddSeconds(Convert.ToDouble(datatmp[0]));
                        bds.Validity = Convert.ToChar(datatmp[1]);
                        bds.Trax = Convert.ToInt32(datatmp[2]);
                        bds.Crax = Convert.ToInt32(datatmp[3]);
                        bds.Axle = (datatmp[4] + datatmp[5]);
                        bds.Truck = Convert.ToChar(datatmp[6]);
                        bds.VLoad = Convert.ToDouble(datatmp[7]);
                        bds.LForce = Convert.ToDouble(datatmp[8]);
                        bds.TSLV = Convert.ToDouble(datatmp[9]);
                        bds.SWLV = Convert.ToDouble(datatmp[10]);
                        bds.ASLV = Convert.ToDouble(datatmp[11]);
                        bds.AOA = Convert.ToDouble(datatmp[12]);
                        bnsDataStructureList.Add(bds);
                        ii++;
                    }
                }
                else { ii++; }
            }
        }
        //-------------------------------------------------------
        // --------------- Code for bnw Files-----------------//

        private void Parse_bnwFiles(string[] SummaryFiles)
        {
            bnwDataStructure bws = new bnwDataStructure();

            foreach (string ifile in SummaryFiles)
            {
                bool breakflag = false;
                AllLines.Clear();
                FileInfo fi = new FileInfo(ifile);
                bnwDataStructureList.Add(bws);
                using (StreamReader sr = File.OpenText(ifile))
                {
                    while (!sr.EndOfStream)
                    {
                        AllLines.Add(sr.ReadLine());
                    }
                }
                int x = 0;
                while (!breakflag & x < AllLines.Count)
                {
                    if (AllLines[x].Contains("TI") || AllLines[x].Contains("VI") || AllLines[x].Contains("TM") || AllLines[x].Contains("WM"))
                    {
                        //try
                        //{
                        Process_bnwLines(AllLines, fi.LastWriteTimeUtc);
                        breakflag = true;
                        //}
                        //catch
                        //{
                        //    breakflag = true;
                        //}
                    }
                    else
                    {
                        x++;
                    }
                }
            }
        }

        private void Process_bnwLines(List<string> allLines, DateTime fd)
        {
            bnwDataStructure bnw = new bnwDataStructure();
            int ii = 0;
            while (ii < allLines.Count)
            {
                if (allLines[ii].Contains("TI,"))
                {
                    string[] tmp = allLines[ii].Split(',');
                    bnw.Time = Convert.ToDateTime(tmp[1]);
                    bnw.Site = tmp[2];
                    bnw.TrackNumber = Convert.ToInt32(tmp[3]);
                    bnw.TrainDirection = Convert.ToChar(tmp[4]);
                    bnw.TrainSpeed = (float)Convert.ToDouble(tmp[5]);
                    bnw.Locos = Convert.ToInt32(tmp[6]);
                    bnw.SlaveLocos = Convert.ToInt32(tmp[7]);
                    bnw.Cars = Convert.ToInt32(tmp[8]);
                    bnw.LocoAxles = Convert.ToInt32(tmp[9]);
                    bnw.SlaveAxles = Convert.ToInt32(tmp[10]);
                    bnw.CarAxles = Convert.ToInt32(tmp[11]);
                    bnw.LocoTon = Convert.ToInt32(tmp[12]);
                    bnw.CarTon = Convert.ToInt32(tmp[13]);
                    bnw.ExtTemp = (float)Convert.ToDouble(tmp[14]);
                    bnw.IntTemp = (float)Convert.ToDouble(tmp[15]);
                    bnw.RelHum = (float)Convert.ToDouble(tmp[16]);
                    bnw.WindSpd = (float)Convert.ToDouble(tmp[17]);
                    bnw.WindDir = Convert.ToInt32(tmp[18]);
                    bnw.Validity = tmp[19];
                    bnw.TrainType = tmp[20];
                    bnwDataStructureList.Add(bnw);
                    ii++;
                }
                if (allLines[ii].Contains("VI,"))
                {
                    string[] datatmp = allLines[ii].Split(new char[] { ',' });
                    bnw.VehicleInitial = datatmp[1];
                    bnw.VehicleNumber = Convert.ToInt32(datatmp[2]);
                    bnw.CarOrientation = datatmp[3];
                    bnw.CarWeight = (float)Convert.ToDouble(datatmp[4]);
                    bnw.CarConfidence = Convert.ToInt32(datatmp[5]);
                    bnw.VehicleType = datatmp[6];
                    bnw.CarStatus = datatmp[7];
                    bnw.CarTruckCount = Convert.ToInt32(datatmp[8]);
                    bnw.CarAxleCount = Convert.ToInt32(datatmp[9]);
                    bnw.CarOrderNum = Convert.ToInt32(datatmp[10]);
                    bnw.CheckStatus = datatmp[11];
                    bnw.CarSpeed = (float)Convert.ToDouble(datatmp[12]);
                    bnwDataStructureList.Add(bnw);
                    ii++;
                }
                if (allLines[ii].Contains("TM,"))
                {
                    string[] datatmp = allLines[ii].Split(new char[] { ',' });
                    bnw.TruckNumber = datatmp[1];
                    bnw.TruckWeight = (float)Convert.ToDouble(datatmp[2]);
                    bnw.TruckConfidence = (float)Convert.ToDouble(datatmp[3]);
                    bnw.Wheelspace = Convert.ToInt32(datatmp[4]);
                    bnw.HuntingIndex = (float)Convert.ToDouble(datatmp[5]);
                    bnwDataStructureList.Add(bnw);
                    ii++;
                }
                if (allLines[ii].Contains("WM,"))
                {
                    string[] datatmp = allLines[ii].Split(new char[] { ',' });
                    bnw.VehicleSide = datatmp[1];
                    bnw.VehicleAxleNum = Convert.ToInt32(datatmp[2]);
                    bnw.AvgVertF = (float)Convert.ToDouble(datatmp[3]);
                    bnw.MaxVertF = (float)Convert.ToDouble(datatmp[4]);
                    bnw.AvgLatF = (float)Convert.ToDouble(datatmp[5]);
                    bnw.MaxLatF = (float)Convert.ToDouble(datatmp[6]);
                    bnwDataStructureList.Add(bnw);
                    ii++;
                }
                else { ii++; }
            }
        }

        //-------------------------------------------------------
        // --------------- Code for trn Files-----------------//
        private void Parse_trnFiles(string[] SummaryFiles)
        {
            int filecounter = 0;
            foreach (string ifile in SummaryFiles)
            {
                filecounter++;
                bool breakflag = false;
                AllLines.Clear();
                FileInfo fi = new FileInfo(ifile);
                using (StreamReader sr = File.OpenText(ifile))
                {
                    while (!sr.EndOfStream)
                    {
                        AllLines.Add(sr.ReadLine());
                    }
                }
                int x = 0;
                while (!breakflag & x < AllLines.Count)
                {
                    if (AllLines[x].Contains("Board:"))
                    {
                        try
                        {
                            Process_trnLines(AllLines, fi.LastWriteTimeUtc, filecounter);
                            breakflag = true;
                        }
                        catch
                        {
                            breakflag = true;
                        }
                    }
                    else
                    {
                        x++;
                    }
                }
            }
        }
        private void Process_trnLines(List<string> allLines, DateTime fd, int filecounter)
        {
            int ii = 0;
            while (ii < allLines.Count)
            {
                trnDataStructure trn = new trnDataStructure();
                if (allLines[ii].Contains("Board:"))
                {
                    string[] datatmp = allLines[ii].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    trn.TrackType = datatmp[4];
                    trn.CribNumber = Convert.ToInt32(datatmp[5]);
                    trn.RailType = datatmp[6];
                    trn.Orientation = datatmp[7];
                    ii++;
                    ii++;
                }
                if (allLines[ii].Contains("-------------------------------------------------------------------------------------------"))
                {
                    ii++;
                    while (!allLines[ii].Contains("Peak Axle Count") && !allLines[ii].Equals(""))
                    {
                        string[] datatmp = allLines[ii].Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        trn.PeakNumber = Convert.ToInt32(datatmp[0]);
                        trn.PeakTime = Convert.ToDateTime(datatmp[1].Replace("_","/"));
                        trn.MaxVertF = (float)Convert.ToDouble(datatmp[2]);
                        trn.MaxLatF = (float)Convert.ToDouble(datatmp[3]);
                        trn.AOAtime = (float)Convert.ToDouble(datatmp[4]);
                        trn.LPVert = (float)Convert.ToDouble(datatmp[5]);
                        trnDataStructureList.Add(trn);
                        ii++;
                    }
                    if (allLines[ii].Contains("Peak Axle Count"))
                    {
                        string[] datatmp = allLines[ii].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        trn.AxleCount = Convert.ToInt32(datatmp[3]);
                        trnDataStructureList.Add(trn);
                        ii++;  
                    }
                }
                //if (allLines[ii].Contains("[Tags Read]"))
                //{
                //    ii++;
                //    ii++;
                //    ii++;
                //    ii++;
                //    while (!allLines[ii].Equals("[Channel Calibrations]"))
                //    {
                //        trnDataStructure Trn = new trnDataStructure();
                //        string [] Datatmp = allLines[ii].Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                //        string[] datatmpentry = Datatmp[0].Split(new char[] { ')' }, StringSplitOptions.RemoveEmptyEntries);
                //        Trn.Position = Convert.ToInt32(datatmpentry[0]);
                //        Trn.CarEnd = (Datatmp[1]);
                //        Trn.Owner = (Datatmp[2]);
                //        Trn.CarNumber = Convert.ToInt32(Datatmp[3]);
                //        Trn.CarType = Convert.ToInt32(Datatmp[4]);
                //        Trn.CarAxle = Convert.ToInt32(Datatmp[5]);
                //        Trn.CarTime = Convert.ToDateTime(Datatmp[6]);
                //        Trn.TimeSinceTag = (float)Convert.ToDouble(Datatmp[7]);
                //        Trn.ReadPort = Convert.ToInt32(Datatmp[8].Replace(" ",""));
                //        trnDataStructureList.Add(Trn);
                //        ii++;
                //        if (!allLines[ii].Equals(""))
                //        {
                //            ii++;
                //            string[] datatmp = allLines[ii].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                //            Trn.TotalTrainAxle = Convert.ToInt32(datatmp[5]);
                //            trnDataStructureList.Add(Trn);
                //        }
                //    }
                //}
                if (allLines[ii].Contains(""))
                { ii++; }
            }
        }
    }
}
