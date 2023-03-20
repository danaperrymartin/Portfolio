using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace FileArchiverUI
{
    public class Archiver
    {
        //---------------------------------------------------------------------------------------------------------------------------------------------
        //  PRIVATE
        //---------------------------------------------------------------------------------------------------------------------------------------------
        private List<string> trn_files;
        //---------------------------------------------------------------------------------------------------------------------------------------------
        //  PUBLIC
        //---------------------------------------------------------------------------------------------------------------------------------------------
        public delegate void ArchiverProgressBarUpdateEventHandler(int pmax);
        public event ArchiverProgressBarUpdateEventHandler ArchiverProgressBarEvent;

        public delegate void ArchiverFileNameUpdateEventHandler(string filename);
        public event ArchiverFileNameUpdateEventHandler ArchiverFileNameEvent;

        public delegate void ArchiverStatusStripUpdateEventHandler(int status, string isite);
        public event ArchiverStatusStripUpdateEventHandler ArchiverStatusStripEvent;

        public string delay { set; get; }
        public string srcdrive { set; get; }
        public string destdrive { set; get; }

        private void RaiseArchiverProgressBarEvent(int pmax)
        {
            if(ArchiverProgressBarEvent != null)
            {
                ArchiverProgressBarEvent(pmax);
            }
        }

        private void RaiseArchiverFileNameEvent(string filename)
        {
            if (ArchiverFileNameEvent != null)
            {
                ArchiverFileNameEvent(filename);
            }
        }

        private void RaiseArchiverStatusStripEvent(int status, string isite)
        {
            if (ArchiverFileNameEvent != null)
            {
                ArchiverStatusStripEvent(status, isite);
            }
        }

        public void StartProcess(string start_date, string end_date)
        {
            trn_files = new List<string>();
            GetTrnFiles(start_date, end_date);
        }

        private void GetTrnFiles(string start_date, string end_date)
        {
            
            //string site;
            //newdir = newdrive;
            //string month_string;
            //string day_string;
            //string delimiter = "/";
            //string sep = "\\";

            string[] startdate_parsed = start_date.Split('/');
            string[] enddate_parsed = end_date.Split('/');
            string drive = srcdrive;
           
            System.IO.DirectoryInfo siteinfo = new System.IO.DirectoryInfo(srcdrive);
            string[] sitedirs = Directory.GetDirectories(siteinfo.ToString());
            if (sitedirs.Length > 0)
            {
                foreach (string isite in sitedirs)
                {
                    RaiseArchiverStatusStripEvent(0, isite);
                    var directory = new DirectoryInfo(isite);
                    //DateTime from_date = DateTime.Now.AddYears(-10);
                    //DateTime to_date = DateTime.Now.AddYears(-2);

                    DateTime from_date = Convert.ToDateTime(start_date);
                    DateTime to_date = Convert.ToDateTime(end_date).AddHours(24);
                    var files_newmeth = directory.GetFiles("*", SearchOption.AllDirectories)
                                        .Where(file => file.CreationTime >= from_date && file.CreationTime <= to_date);

                    foreach (FileInfo ifile in files_newmeth)
                    {
                        trn_files.Add(ifile.FullName.ToString());
                    }
                }
            }
            else if (sitedirs.Length==0)
            {
                RaiseArchiverStatusStripEvent(0, siteinfo.ToString());
                var directory = new DirectoryInfo(siteinfo.ToString());
                
                DateTime from_date = Convert.ToDateTime(start_date);
                DateTime to_date = Convert.ToDateTime(end_date).AddHours(24);
                var files_newmeth = directory.GetFiles("*", SearchOption.AllDirectories)
                                    .Where(file => file.CreationTime >= from_date && file.CreationTime <= to_date);

                foreach (FileInfo ifile in files_newmeth)
                {
                    trn_files.Add(ifile.FullName.ToString());
                }
            }
            CopyTrnFiles();
        }

        private void CopyTrnFiles()
        {
            RaiseArchiverProgressBarEvent(trn_files.Count);
            foreach (string ifile in trn_files)
            {
                RaiseArchiverStatusStripEvent(1, ifile.Split('\\')[1]);
                int milliseconds = Convert.ToInt32(delay) * 1000;
                Thread.Sleep(milliseconds);
                RaiseArchiverFileNameEvent(ifile);
                FileInfo currentfile = new FileInfo(ifile);
                FileInfo afile = new FileInfo(ifile.Replace(srcdrive.Split('\\')[0], destdrive));
                if (afile.Directory.Exists)
                {
                    //currentfile.CopyTo(afile.ToString(), true);
                    currentfile.MoveTo(afile.ToString());
                }
                else
                {
                    afile.Directory.Create();
                    //currentfile.CopyTo(afile.ToString(), true);
                    currentfile.MoveTo(afile.ToString());
                }
                RaiseArchiverProgressBarEvent(trn_files.Count);
            }
            RaiseArchiverProgressBarEvent(trn_files.Count);
        }
    }
}
