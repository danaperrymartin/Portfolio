using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace TCPServerFTP
{
    public partial class frmMain : Form
    {
        //---------------------------------------------------------------------------------------------------------------------------------------------
        //  CONSTANTS
        //---------------------------------------------------------------------------------------------------------------------------------------------

        //---------------------------------------------------------------------------------------------------------------------------------------------
        //  PUBLIC
        //---------------------------------------------------------------------------------------------------------------------------------------------
        public delegate void SetProgressBarDlgt(int iNum);
        public SetProgressBarDlgt m_SetProgressBarDlgt;

        public delegate void SetLabelName1Dlgt(string label);
        public SetLabelName1Dlgt m_SetLabelName1Dlgt;
        //---------------------------------------------------------------------------------------------------------------------------------------------
        //  PRIVATE
        //---------------------------------------------------------------------------------------------------------------------------------------------
        private delegate void AddMsgDlgt(string szMsg);
        private AddMsgDlgt m_AddMsgDlgt;
        private TCPServer m_TCPServer;
        private FTP m_ftp;

        private enum ConnectionStatus
        {
            Unknown,
            Connected,
            Retrying,
            Lost
        }

        private delegate void UpdateConnectionStatusDlgt(TCPServer.ConnectionStatus status);
        private UpdateConnectionStatusDlgt m_UpdateConnectionStatusDlgt;

        private delegate void HandleNewMessagesDlgt();
        private HandleNewMessagesDlgt m_HandleNewMessagesDlgt;

        //*********************************************************************************************************************************************
        //
        //  CONSTRUCTORS/DESTRUCTORS/CLEANUP
        //
        //*********************************************************************************************************************************************

        public frmMain()
        {
            InitializeComponent();

            //--------------------------------------------------------------
            //  Init member variables
            //--------------------------------------------------------------
            m_UpdateConnectionStatusDlgt = new UpdateConnectionStatusDlgt(UpdateConnectionStatus);

            m_AddMsgDlgt = new AddMsgDlgt(AddMsg);
            m_TCPServer = new TCPServer();
            //m_LogErrorDlgt = new LogErrorDlgt(LogError);

            m_ftp = new FTP();
            m_TCPServer.ftp = m_ftp;
            m_ftp.tcp_server = m_TCPServer;

            m_SetProgressBarDlgt = new SetProgressBarDlgt(SetProgressBar);
            m_SetLabelName1Dlgt = new SetLabelName1Dlgt(SetLabelName);
        }

        //*********************************************************************************************************************************************
        //
        //  PUBLIC
        //
        //*********************************************************************************************************************************************

        //*********************************************************************************************************************************************
        //
        //  PRIVATE
        //
        //*********************************************************************************************************************************************

        private void frmMain_Load(object sender, EventArgs e)
        {
            m_TCPServer.IPAddress = ucIPAddress1.IPAddress;
            m_TCPServer.TCPClientMessageEvent += M_TCPServer_TCPClientMessageEvent;
            m_TCPServer.ConnectionEvent += M_TCPServer_ConnectionEvent;
            m_TCPServer.HandleMessageReceivedEvent += M_TCPServer_HandleMessageReceivedEvent;
            
            m_ftp.FTPClientProgressBarEvent += M_TCPServer_FTPClientProgressBarEvent;
            m_TCPServer.TCPClientFileNameEvent += M_TCPServer_FTPClientFileNameEvent;
            m_TCPServer.StartServer(null);

            //AddSitesToPanel(m_listSites);
        }

        private void M_TCPServer_HandleMessageReceivedEvent(string szMsg)
        {
            AddMsg(szMsg);
        }

        private void M_TCPServer_ConnectionEvent(TCPServer.ConnectionStatus status)
        {
            UpdateConnectionStatus(status);
        }

        private void M_TCPServer_TCPClientMessageEvent(string szMsg)
        {
            AddMsg(szMsg);
        }

        private void M_TCPServer_FTPClientProgressBarEvent(int progress)
        {
            SetProgressBar(progress);
        }

        private void M_TCPServer_FTPClientFileNameEvent(string filename)
        {
            SetLabelName(filename);
        }

        private void UpdateConnectionStatus( TCPServer.ConnectionStatus status)
        {
            if( InvokeRequired == true)
            {
                BeginInvoke(m_UpdateConnectionStatusDlgt, status);
            }
            else
            {
                switch( status)
                {
                    case TCPServer.ConnectionStatus.Unknown:
                        pbxStatus.Image = imageListLED.Images[0];   //gray
                        break;

                    case TCPServer.ConnectionStatus.Connected:
                        pbxStatus.Image = imageListLED.Images[1];   //green
                        break;

                    case TCPServer.ConnectionStatus.Retrying:
                        pbxStatus.Image = imageListLED.Images[2];   //Yellow
                        break;

                    case TCPServer.ConnectionStatus.Lost:
                        pbxStatus.Image = imageListLED.Images[3];   //red
                        break;

                    default:
                        pbxStatus.Image = imageListLED.Images[3];   //red
                        break;
                }
            }
        }

        private void AddMsg(string szMsg)
        {
            if (InvokeRequired == true)
            {
                BeginInvoke(m_AddMsgDlgt, szMsg);
            }
            else
            {
                string szTime = DateTime.Now.ToString() + " ";

                lbMsg.Items.Add(szTime + szMsg);
            }
        }

        private void btnSendMsg_Click(object sender, EventArgs e)
        {
            m_TCPServer.SendMessage("Server - " + DateTime.Now.ToString());
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_TCPServer.Exit();
        }

        private void SetProgressBar(int iNum)
        {
            if (InvokeRequired == true)
            {
                BeginInvoke(m_SetProgressBarDlgt, iNum);
            }
            else
            {
                progressBar1.Minimum = 0;
                progressBar1.Maximum = 100;
                progressBar1.Value = iNum;
                progressBar1.Step = iNum;
            }
        }

        private void SetLabelName(string labelName)
        {
            if (InvokeRequired == true)
            {
                BeginInvoke(m_SetLabelName1Dlgt, labelName);
            }
            else
            {
                lbl_FileTransferName.Text = labelName;
            }
        }

        AddSitesToPanel(List<Site>)
        {

        }
    }
}
