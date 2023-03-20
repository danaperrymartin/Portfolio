using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace TCPClientFTP
{
    class TCPClient
    {
        //---------------------------------------------------------------------------------------------------------------------------------------------
        //  CONSTANTS
        //---------------------------------------------------------------------------------------------------------------------------------------------
        private const int THREAD_TIMEOUT = 3000;    //in ms
        private const int MAX_MESSAGE_SIZE = 512;    //in bytes
        private const string IMAGE_FILE_PATH = @"C:\\TrainView\\Images";

        //---------------------------------------------------------------------------------------------------------------------------------------------
        //  PUBLIC
        //---------------------------------------------------------------------------------------------------------------------------------------------
        public delegate void ConnectionEventHandler(ConnectionStatus status);
        public event ConnectionEventHandler ConnectionEvent;

        public delegate void TCPClientMessageEventHandler(string szMsg);
        public event TCPClientMessageEventHandler TCPClientMessageEvent;

        public delegate void HandleMessageReceivedEventHandler(string szMsg);
        public event HandleMessageReceivedEventHandler HandleMessageReceivedEvent;

        public enum ConnectionStatus                                                                                                                                                              
        {
            Unknown,
            Connected,
            Retrying,
            Lost
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------
        //  PRIVATE
        //---------------------------------------------------------------------------------------------------------------------------------------------
        bool m_bExit;
        bool m_bDone;
        private Thread m_ClientThread;
        private ManualResetEvent m_mreConnected;
        private FTP ftp;

        private bool m_bCheckThreadStarted;
        private bool m_bTransferring;

        //*********************************************************************************************************************************************
        //
        //  CONSTRUCTORS/DESTRUCTORS/CLEANUP
        //
        //*********************************************************************************************************************************************

        /// <summary>
        /// Default constructor
        /// </summary>
        public TCPClient()
        {
            //--------------------------------------------------------------
            //  Init member variables
            //--------------------------------------------------------------
            m_bExit = false;
            m_bDone = false;
            m_bTransferring = false;
            Client = null;

            m_mreConnected = new ManualResetEvent(false);

            SendQueue = new Queue<string>();
            ReceiveQueue = new Queue<string>();

            m_bCheckThreadStarted = false;
        }

        /// <summary>
        /// Cleans up for exit
        /// </summary>
        public void Exit()
        {
            m_bExit = true;
            m_mreConnected.Set();

            //--------------------------------------------------------------
            //  Clean up thread
            //--------------------------------------------------------------
            if (m_ClientThread != null &&
                m_ClientThread.Join(THREAD_TIMEOUT) == false)
            {
                m_ClientThread.Abort();
            }

            m_ClientThread = null;

            if (Client != null)
            {
                Client.Close();
                Client.Dispose();
            }

            Client = null;

        }

        //*********************************************************************************************************************************************
        //
        //  PUBLIC
        //
        //*********************************************************************************************************************************************

        public string IPAddress { set; get; }

        public DateTime dtLastHB { set; get; }

        public void StartClient( object obj)
        {
            //--------------------------------------------------------------
            //  Clean up thread
            //--------------------------------------------------------------
            if (m_ClientThread != null &&
                m_ClientThread.Join(THREAD_TIMEOUT) == false)
            {
                m_ClientThread.Abort();
            }

            m_ClientThread = null;

            if (Client != null)
            {
                Client.Close();
                Client.Dispose();
            }

            m_mreConnected = new ManualResetEvent(false);

            //--------------------------------------------------------------
            //  Start a connection thread on the specified address/port
            //--------------------------------------------------------------
            m_ClientThread = new Thread(new ParameterizedThreadStart(ClientThread));
            m_ClientThread.Name = "ClientThread";
            m_ClientThread.Priority = ThreadPriority.Normal;
            m_ClientThread.Start(null);

            if (m_bCheckThreadStarted == false)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(CheckReceivedMessages));
                m_bCheckThreadStarted = true;
            }
        }

        public void SendMessage( string szMsg)
        {
            SendQueue.Enqueue(szMsg);
        }

        //*********************************************************************************************************************************************
        //
        //  PRIVATE
        //
        //*********************************************************************************************************************************************
        private Queue<string> SendQueue { set; get; }

        private Queue<string> ReceiveQueue { set; get; }

        private TcpClient Client { set; get; }

        private int Port { set; get; }

        private void SendMessage()
        {
            byte[] baData = new byte[MAX_MESSAGE_SIZE];
            int iLen = 0;
            string szFullMessage = string.Empty;
            string szData = string.Empty;
            NetworkStream stream = null;
            DateTime dtLastHB = DateTime.Now;
            TimeSpan ts = TimeSpan.Zero;

            string szIPAddress = IPAddress;

            int iPos = szIPAddress.IndexOf(':');

            if (iPos != -1)
            {
                string szPort = szIPAddress.Substring(iPos + 1);
                Port = int.Parse(szPort);
                szIPAddress = szIPAddress.Substring(0, iPos);
            }

            Client = new TcpClient(AddressFamily.InterNetwork);

            Client.BeginConnect(szIPAddress, Port, new AsyncCallback(NetworkConnected), null);
            RaiseTCPMessageEvent("Trying to connect " + szIPAddress + " " + Port.ToString());

            m_mreConnected.WaitOne();

            //--------------------------------------------------------------
            //  Client has connected at this point
            //--------------------------------------------------------------
            if (Client != null && Client.Connected == true)
            {
                stream = Client.GetStream();
            }

            while (m_bExit == false)
            {
                //--------------------------------------------------------------
                //  See if messages available
                //--------------------------------------------------------------
                if (stream.DataAvailable == true && m_bExit == false)
                {
                    baData = new byte[MAX_MESSAGE_SIZE];

                    //--------------------------------------------------------------
                    //  Get next message received
                    //--------------------------------------------------------------
                    iLen = stream.Read(baData, 0, MAX_MESSAGE_SIZE);

                    //--------------------------------------------------------------
                    //  We might have more than one message or a partial message
                    //--------------------------------------------------------------
                    szData = Encoding.ASCII.GetString(baData, 0, iLen);

                    for (int i = 0; i < szData.Length; i++)
                    {
                        if (szData[i] != '|')
                        {
                            szFullMessage += szData[i];
                        }
                        else
                        {
                            //--------------------------------------------------------------
                            //  Handle the heartbeat message
                            //--------------------------------------------------------------
                            if (szFullMessage != "hb")
                            {
                                //--------------------------------------------------------------
                                //  Message completed add to queue
                                //--------------------------------------------------------------
                                ReceiveQueue.Enqueue(szFullMessage);
                                szFullMessage = string.Empty;
                            }
                            else
                            {
                                szFullMessage = string.Empty;
                                dtLastHB = DateTime.Now;
                            }
                        }
                    }
                }

                //--------------------------------------------------------------
                //  Make sure we are receiving the heartbeat
                //--------------------------------------------------------------
                ts = DateTime.Now - ftp.GetLastLastHB;

                if (ts.TotalSeconds >= 1.0)
                {
                    RaiseConnectionEvent(ConnectionStatus.Lost);
                    RaiseTCPMessageEvent("Lost Connection");

                    //Start new thread
                    ThreadPool.QueueUserWorkItem(new WaitCallback(StartClient));

                    //Exit this thread
                    return;

                }
                //--------------------------------------------------------------
                //  Send heartbeat message
                //--------------------------------------------------------------
                baData = System.Text.Encoding.ASCII.GetBytes("hb|");

                try
                {
                    stream.Write(baData, 0, 3);
                }
                catch (Exception ex)
                {
                    RaiseConnectionEvent(ConnectionStatus.Lost);
                    RaiseTCPMessageEvent("Lost Connection");

                    //Start new thread
                    ThreadPool.QueueUserWorkItem(new WaitCallback(StartClient));

                    //Exit this thread
                    return;
                }

                //--------------------------------------------------------------
                //  Give other threads a chance to run
                //--------------------------------------------------------------
                Thread.Sleep(100);
            }
        }

        private void SendFiles()
        {
            ftp = new FTP();
            byte[] baData = new byte[MAX_MESSAGE_SIZE];
            int iLen = 0;
            string szFullMessage = string.Empty;
            string szData = string.Empty;
            NetworkStream stream = null;
            DateTime dtLastHB = DateTime.Now;
            TimeSpan ts = TimeSpan.Zero;

            string szIPAddress = IPAddress;

            int iPos = szIPAddress.IndexOf(':');

            if (iPos != -1)
            {
                string szPort = szIPAddress.Substring(iPos + 1);
                Port = int.Parse(szPort);
                szIPAddress = szIPAddress.Substring(0, iPos);
            }

            Client = new TcpClient(AddressFamily.InterNetwork);

            Client.BeginConnect(szIPAddress, Port, new AsyncCallback(NetworkConnected), null);
            RaiseTCPMessageEvent("Trying to connect " + szIPAddress + " " + Port.ToString());

            m_mreConnected.WaitOne();

            //--------------------------------------------------------------
            //  Client has connected at this point
            //--------------------------------------------------------------
            if (Client != null && Client.Connected == true)
            {
                stream = Client.GetStream();
            }

            while (m_bExit == false)
            {
                //--------------------------------------------------------------
                //  See if ther are any jpg files in TrainView//Image directory
                //--------------------------------------------------------------
                List<string> saFiles = new List<string>();
                ftp.DirSearch(IMAGE_FILE_PATH, saFiles);
                RaiseTCPMessageEvent("Number of files in directory--" + (saFiles.Count).ToString());

                //--------------------------------------------------------------
                //  See if messages available
                //--------------------------------------------------------------
                if (m_bExit == false && saFiles.Count>0)
                {
                    try
                    {
                        //--------------------------------------------------------------
                        //  Only start transfer if connected
                        //--------------------------------------------------------------
                        if (Client.Connected)
                        {
                            RaiseTCPMessageEvent("Starting file transfer...");
                            m_bTransferring = true;

                            ////--------------------------------------------------------------
                            ////  Send over the file transfer command
                            ////--------------------------------------------------------------
                            RaiseTCPMessageEvent("Client: Beginning file transfer");

                            for (int ifile = saFiles.Count - 1; ifile >= 0; ifile--)
                            {
                                RaiseTCPMessageEvent("Client: Beginning transfer of file " + saFiles[ifile]);
                                string currentfile = saFiles[ifile];

                                FileStream file = new FileStream(currentfile, FileMode.Open, FileAccess.Read);
                                ftp.sendhandshake(Client, "Client: opened file");
                                ftp.wait4handshake(Client, "Client: waiting for server ready for command");
                                String command = "TransferFile";

                                ftp.sendCommand(stream, command);
                                ftp.wait4handshake(Client, "Client: Waiting4Handshake...TransferFileCommand");

                                ftp.sendfilename(Client, saFiles[ifile]);
                                ftp.wait4handshake(Client, "Client: Waiting for server...filename");

                                ftp.sendhandshake(Client, "Client: Sending file size.");
                                float total_bytes_sent = ftp.wait4filesize(Client, "Client: Waiting for server...get existing file size");

                                float packet_size = Math.Min(file.Length, 65536);  // packet size to transfer (1e9 bytes in 1GB)
                                float datapackets = (int)Math.Ceiling((double)(file.Length / packet_size));  //Number of datapackets for full file transfer given defined packet size

                                byte[] stream_length = BitConverter.GetBytes(file.Length);
                                RaiseTCPMessageEvent("Client: File size -- " + file.Length.ToString());

                                stream.Write(stream_length, 0, stream_length.Length); //Serves as sending a handshake
                                ftp.wait4handshake(Client, "Client: Waiting for server...total payload length");

                                while (total_bytes_sent < file.Length && Client.Connected)
                                {
                                    packet_size = Math.Min(packet_size, (file.Length - total_bytes_sent));
                                    byte[] bytes = new byte[(long)packet_size];

                                    if (total_bytes_sent > 0)
                                    {
                                        file.Seek((long)total_bytes_sent, SeekOrigin.Begin);
                                    }
                                    file.Read(bytes, 0, (int)bytes.Length);

                                    ftp.sendData(bytes, Client, (int)((total_bytes_sent / (float)file.Length) * 100));

                                    file.Flush();
                                    datapackets--;
                                    total_bytes_sent = packet_size + total_bytes_sent;

                                    ftp.wait4handshake(Client, "Client: Waiting4Handshake...packet transfer");
                                }
                                ftp.GetLastLastHB = DateTime.Now;
                                ftp.sendhandshake(Client, "Client: Waiting4Handshake...DataTransfer");

                                file.Close();
                                File.Delete(currentfile);

                                ftp.wait4handshake(Client, "Client: Server wrote to filestream?");
                            }
                        }

                        DirectoryInfo id = new DirectoryInfo("C:\\TrainView\\Images\\");
                        FileInfo[] fii = id.GetFiles("*");

                        for (int i = 0; i < fii.Length; i++)
                        {
                            File.Delete(fii[i].FullName);
                        }

                        ftp.DeleteEmptyDir("C:\\TrainView\\Images\\");

                        m_bDone = true;
                        //RaiseTCPMessageEvent("Client: Exiting file transfer thread normally");
                        //Thread.Sleep(1000);
                    }  
                    catch (Exception ex)
                    {
                        ProgramLog.Log(ex.ToString());
                        //Exit this thread
                        return;
                    }
                }
                //--------------------------------------------------------------
                //  Make sure we are receiving the heartbeat
                //--------------------------------------------------------------
                ts = DateTime.Now - ftp.GetLastLastHB;

                if (ts.TotalSeconds >= 60.0)
                {
                    //--------------------------------------------------------------
                    //  Send heartbeat message
                    //--------------------------------------------------------------
                    try
                    {
                        ftp.GetHeartBeat(Client);
                    }
                    catch (Exception ex)
                    {
                        RaiseConnectionEvent(ConnectionStatus.Lost);
                        RaiseTCPMessageEvent("Lost Connection");

                        //Start new thread
                        ThreadPool.QueueUserWorkItem(new WaitCallback(StartClient));

                        //Exit this thread
                        return;
                    }
                }
                //--------------------------------------------------------------
                //  Give other threads a chance to run
                //--------------------------------------------------------------
                Thread.Sleep(100);
            }
        }

        private void ClientThread(object obj)
        {
            //SendMessage();
            SendFiles();
        }

        private void NetworkConnected(System.IAsyncResult ar)
        {
            bool bConnected = true;

            try
            {
                if (Client != null && Client.Client != null)
                {
                    Client.EndConnect(ar);
                }
            }
            catch (Exception ex)
            {
                RaiseConnectionEvent(ConnectionStatus.Lost);
                bConnected = false;
                RaiseTCPMessageEvent(ex.Message);

                Thread.Sleep(1000);
                //--------------------------------------------------------------
                //  Try again
                //--------------------------------------------------------------
                string szIPAddress = IPAddress;

                int iPos = szIPAddress.IndexOf(':');

                string szPort = szIPAddress.Substring(iPos + 1);
                Port = int.Parse(szPort);

                szIPAddress = szIPAddress.Substring(0, iPos);

                Client.BeginConnect(szIPAddress, Port, new AsyncCallback(NetworkConnected), null);
                RaiseTCPMessageEvent("Trying to connect");

            }

            if (ar.IsCompleted == true && bConnected == true)
            {
                m_mreConnected.Set();
                RaiseConnectionEvent(ConnectionStatus.Connected);
                RaiseTCPMessageEvent("connected");
            }
        }

        private void CheckReceivedMessages(object obj)
        {
            while(m_bExit == false)
            {
                if( ReceiveQueue.Count > 0)
                {
                    RaiseMessageReceivedEvent(ReceiveQueue.Dequeue());
                }
                Thread.Sleep(1);
            }
        }

        private void RaiseConnectionEvent( ConnectionStatus status)
        {
            if( ConnectionEvent != null)
            {
                ConnectionEvent(status);
            }
        }

        private void RaiseTCPMessageEvent( string szMsg)
        {
            if( TCPClientMessageEvent != null )
            {
                TCPClientMessageEvent(szMsg);
            }
        }

        private void RaiseMessageReceivedEvent( string szMsg)
        {
            if(HandleMessageReceivedEvent != null)
            {
                HandleMessageReceivedEvent(szMsg);
            }
        }
    }
}
