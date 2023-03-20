using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace TCPClientFTP
{
    public partial class frmMain : Form
    {
        //---------------------------------------------------------------------------------------------------------------------------------------------
        //  CONSTANTS
        //---------------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------------
        //  PUBLIC
        //---------------------------------------------------------------------------------------------------------------------------------------------

        //---------------------------------------------------------------------------------------------------------------------------------------------
        //  PRIVATE
        //---------------------------------------------------------------------------------------------------------------------------------------------

        private delegate void UpdateConnectionStatusDlgt(TCPClient.ConnectionStatus status);
        private UpdateConnectionStatusDlgt m_UpdateConnectionStatusDlgt;

        private delegate void AddMsgDlgt(string szMsg);
        private AddMsgDlgt m_AddMsgDlgt;

        private TCPClient m_TCPClient;

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
            //---------------------------------------------------------------
            m_AddMsgDlgt = new AddMsgDlgt(AddMsg);
            m_UpdateConnectionStatusDlgt = new UpdateConnectionStatusDlgt(UpdateConnectionStatus);

            m_TCPClient = new TCPClient();
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

        private void UpdateConnectionStatus(TCPClient.ConnectionStatus status)
        {
            if (InvokeRequired == true)
            {
                BeginInvoke(m_UpdateConnectionStatusDlgt, status);
            }
            else
            {

                switch (status)
                {
                    case TCPClient.ConnectionStatus.Unknown:
                        pbxStatus.Image = imageListLED.Images[0];   //gray
                        break;

                    case TCPClient.ConnectionStatus.Connected:
                        pbxStatus.Image = imageListLED.Images[1];   //green
                        break;

                    case TCPClient.ConnectionStatus.Retrying:
                        pbxStatus.Image = imageListLED.Images[2];   //Yellow
                        break;

                    case TCPClient.ConnectionStatus.Lost:
                        pbxStatus.Image = imageListLED.Images[3];   //red
                        break;

                    default:
                        pbxStatus.Image = imageListLED.Images[3];   //red
                        break;
                }
            }
        }

        private void btnSendMsg_Click(object sender, EventArgs e)
        {
            m_TCPClient.SendMessage("Client - " + DateTime.Now.ToString());
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            m_TCPClient.IPAddress = ucIPAddress1.IPAddress;
            m_TCPClient.TCPClientMessageEvent += M_TCPClient_TCPClientMessageEvent;
            m_TCPClient.ConnectionEvent += M_TCPClient_ConnectionEvent;
            m_TCPClient.HandleMessageReceivedEvent += M_TCPClient_HandleMessageReceivedEvent;
            m_TCPClient.StartClient(null);
        }

        private void M_TCPClient_TCPClientMessageEvent(string szMsg)
        {
            AddMsg(szMsg);
        }

        private void M_TCPClient_ConnectionEvent(TCPClient.ConnectionStatus status)
        {
            UpdateConnectionStatus(status);
        }

        private void M_TCPClient_HandleMessageReceivedEvent(string szMsg)
        {
            AddMsg(szMsg);
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

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_TCPClient.Exit();
        }
    }
}
