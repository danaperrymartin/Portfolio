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

        private void GetAllFiles()
        {
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
                        AllFilesList.Add(iday);
                        icountday++;
                    }
                }
            }
        }
    }
}
