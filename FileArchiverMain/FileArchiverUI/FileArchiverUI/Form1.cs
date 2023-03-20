using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Threading;
using System.Windows.Interop;

namespace FileArchiverUI
{
    public partial class Form1 : Form
    {
        //-------------------------------------------------------------------------------------------------
        //  PRIVATE
        //-------------------------------------------------------------------------------------------------
        private string newdir;
        public BarDelegate m_barDelegate;
        public FileLabelDelegate m_filelabelDelegate;
        public StatusStripDelegate m_statusstripDelegate;

        private Archiver m_Archiver;

        private Thread m_ArchiverThread;

        //-------------------------------------------------------------------------------------------------
        //  PUBLIC
        //-------------------------------------------------------------------------------------------------
        public delegate void BarDelegate(int lng);
        public delegate void FileLabelDelegate(string filename);
        public delegate void StatusStripDelegate(int status, string isite);
        //*********************************************************************************************************************************************
        //
        //	CONSTRUCTORS/DESTRUCTORS/CLEANUP
        //
        //*********************************************************************************************************************************************
        public Form1()
        {
            InitializeComponent();
            m_barDelegate = new BarDelegate(UpdateBar);
            m_filelabelDelegate = new FileLabelDelegate(UpdateFileLabel);
            m_statusstripDelegate = new StatusStripDelegate(UpdateStatusStrip);
            StartPosition = FormStartPosition.CenterScreen;

            m_Archiver = new Archiver();

        }
        
        private void frmMain_Load(object sender, EventArgs e)
        {
            dateTimePicker_start.Format = DateTimePickerFormat.Custom;
            dateTimePicker_start.CustomFormat = "MM/dd/yyyy";
            dateTimePicker_end.Format = DateTimePickerFormat.Custom;
            dateTimePicker_end.CustomFormat = "MM/dd/yyyy";

            m_Archiver.ArchiverProgressBarEvent += M_FileArchiver_FileArchiverProgressBarEvent;
            m_Archiver.ArchiverFileNameEvent += M_FileArchiver_FileArchiverFileNameEvent;
            m_Archiver.ArchiverStatusStripEvent += M_FileArchiver_FileArchiverStatusStripEvent;
        }

        [DllImport(@"lib\FileArchiverLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GettrnFiles(string site, string start_date, string end_date, string sourcedrive);

        [DllImport(@"lib\FileArchiverLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CopytrnFiles2Archive(string newdrive);
        private void btn_run_Click(object sender, EventArgs e)
        {
            
            try
            {
                //toolStripStatusLabel1.Text = "";
                //statusStrip1.BackColor = Color.Green;
                //statusStrip1.Update();
                string startdate = dateTimePicker_start.Text;// txtbox_startdate.Text;
                string enddate = dateTimePicker_end.Text;// txtbox_enddate.Text;
                m_Archiver.srcdrive = txtbox_srcdrive.Text;
                //string sourcedrive = txtbox_srcdrive.Text;
                string destdrive = txtbox_destdrive.Text;
                m_Archiver.destdrive = txtbox_destdrive.Text;
                //GettrnFiles(site, startdate, enddate, sourcedrive);
                //CopytrnFiles2Archive(destdrive);
                //trn_files = new List<string>();

                m_ArchiverThread = new Thread(() => m_Archiver.StartProcess(startdate, enddate));
                m_ArchiverThread.Start();
                //GetTrnFiles(startdate, enddate, sourcedrive, destdrive);
                //CopyTrnFiles();
            }
            catch(Exception ex)
            {
                toolStripStatusLabel1.Text = "Error";
                statusStrip1.BackColor = Color.Red;
                statusStrip1.Update();
            }
        }

        

        private void UpdateBar(int lng)
        {
            if (InvokeRequired == true)
            {
                BeginInvoke(m_barDelegate, lng);
            }
            else
            {
                pBar1.Maximum = lng;
                if (pBar1.Value == pBar1.Maximum)
                {
                    lbl_currentfile.Text = "Finished...";
                    pBar1.Value = 0;
                    lbl_progress.Text = "";
                    pBar1.Update();
                    statusStrip1.Visible = false;
                    statusStrip1.Update();
                }
                else if (pBar1.Value != pBar1.Maximum && !(lbl_currentfile.Text).Contains("Finished..."))
                {
                    lbl_progress.Text = ((int)(((double)pBar1.Value / (double)lng) * 100)).ToString() + "%";
                    lbl_progress.Update();
                    pBar1.Value++;
                    pBar1.Update();
                }
            }
            
        }

        private void UpdateFileLabel(string currentfilename)
        {
            if (InvokeRequired == true)
            {
                BeginInvoke(m_filelabelDelegate, currentfilename);
            }
            else
            {
                lbl_currentfile.Text = currentfilename;
                lbl_currentfile.Update();
            }
            
        }

        private void UpdateStatusStrip(int status, string isite)
        {
            if (InvokeRequired == true)
            {
                BeginInvoke(m_statusstripDelegate, status, isite);
            }
            else
            {
                for (int i = 0; i < statusStrip1.Items.Count; i++)
                {
                    if (i == status)
                    {
                        statusStrip1.Items[i].Visible = true;
                        statusStrip1.Items[i].Enabled = true;
                    }
                    else if(i!=status)
                    {
                        statusStrip1.Items[i].Visible = false;
                        statusStrip1.Items[i].Enabled = false;
                    }
                }
                statusStrip1.Items[status].Text = (statusStrip1.Items[status].Text).Split('.')[0];
                statusStrip1.Text = statusStrip1.Items[status].Text + "..." + isite;
                statusStrip1.BackColor = statusStrip1.Items[status].BackColor;
                statusStrip1.Update();
            }
        }

        private void btn_browsesrcdrive_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dlg = new CommonOpenFileDialog();
            dlg.IsFolderPicker = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok) // Test result.
            {
                txtbox_srcdrive.Text = dlg.FileName;
            }
        }

        private void btn_browsedestdrive_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dlg = new CommonOpenFileDialog();
            dlg.IsFolderPicker = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok) // Test result.
            {
                txtbox_destdrive.Text = dlg.FileName;
            }
        }

        //-----  Add Event Handlers for GUI updating ----//
        private void M_FileArchiver_FileArchiverProgressBarEvent(int pmax)
        {
            UpdateBar(pmax);
        }

        private void M_FileArchiver_FileArchiverFileNameEvent(string filename)
        {
            UpdateFileLabel(filename);
        }

        private void M_FileArchiver_FileArchiverStatusStripEvent(int status, string isite)
        {
            UpdateStatusStrip(status, isite);
        }
    }
}
