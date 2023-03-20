using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TCPServerFTP
{
    public class TCPServer
    {
        //---------------------------------------------------------------------------------------------------------------------------------------------
        //  CONSTANTS
        //---------------------------------------------------------------------------------------------------------------------------------------------
        private const int THREAD_TIMEOUT = 3000;    //in ms
        private const int MAX_MESSAGE_SIZE = 512;    //in bytes

        //---------------------------------------------------------------------------------------------------------------------------------------------
        //  PUBLIC
        //---------------------------------------------------------------------------------------------------------------------------------------------
        public delegate void ConnectionEventHandler(ConnectionStatus status);
        public event ConnectionEventHandler ConnectionEvent;

        public delegate void TCPClientMessageEventHandler(string szMsg);
        public event TCPClientMessageEventHandler TCPClientMessageEvent;

        public delegate void TCPClientFileNameEventHandler(string filename);
        public event TCPClientFileNameEventHandler TCPClientFileNameEvent;

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
        private Thread m_ServerThread;
        private ManualResetEvent m_mreConnected;
        private bool m_bCheckThreadStarted;
        public FTP ftp { set; get; }
        //private frmMain frmmain;

        //*********************************************************************************************************************************************
        //
        //  CONSTRUCTORS/DESTRUCTORS/CLEANUP
        //
        //*********************************************************************************************************************************************
        public TCPServer()
        {
            //--------------------------------------------------------------
            //  Init member variables
            //--------------------------------------------------------------
            //ProgramLog.Init();
            m_bExit = false;

            Client = null;

            m_mreConnected = new ManualResetEvent(false);

            SendQueue = new Queue<string>();
            ReceiveQueue = new Queue<string>();

            m_bCheckThreadStarted = false;

            SendQueue = new Queue<string>();
            ReceiveQueue = new Queue<string>();

        }

        public void Exit()
        {
            m_bExit = true;
            m_mreConnected.Set();

            //--------------------------------------------------------------
            //  Clean up thread
            //--------------------------------------------------------------
            if (m_ServerThread != null &&
                m_ServerThread.Join(THREAD_TIMEOUT) == false)
            {
                m_ServerThread.Abort();
            }

            m_ServerThread = null;

            if (Client != null)
            {
                Client.Close();
                Client.Client.Dispose();
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

        public int Port { set; get; }

        public void StartServer(object obj)
        {

            //--------------------------------------------------------------
            //  Clean up thread
            //--------------------------------------------------------------
            if (m_ServerThread != null &&
                m_ServerThread.Join(THREAD_TIMEOUT) == false)
            {
                m_ServerThread.Abort();
            }

            m_ServerThread = null;

            if (Client != null)
            {
                Client.Close();
                Client.Client.Dispose();
            }

            m_mreConnected = new ManualResetEvent(false);

            //--------------------------------------------------------------
            //  Start a listen thread on the specified address/port
            //--------------------------------------------------------------
            m_ServerThread = new Thread(new ParameterizedThreadStart(ServerThread));
            m_ServerThread.Name = "ServerThread";
            m_ServerThread.Priority = ThreadPriority.Normal;
            m_ServerThread.Start(null);

            RaiseTCPMessageEvent("Waiting for connection");

            if (m_bCheckThreadStarted == false)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(CheckReceivedMessages));
                m_bCheckThreadStarted = true;
            }
        }

        public void SendMessage(string szMsg)
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

        private void GetFiles()
        {
            //ftp = new FTP();
            NetworkStream stream = null;
            byte[] baData = new byte[MAX_MESSAGE_SIZE];
            int iLen = 0;
            string szFullMessage = string.Empty;
            string szData = string.Empty;
            TimeSpan ts = TimeSpan.Zero;

            string szIPAddress = IPAddress;

            int iPos = szIPAddress.IndexOf(':');

            if (iPos != -1)
            {
                string szPort = szIPAddress.Substring(iPos + 1);
                Port = int.Parse(szPort);
                szIPAddress = szIPAddress.Substring(0, iPos);
            }

            IPAddress listenAddress = System.Net.IPAddress.Parse(szIPAddress);

            TcpListener listener = new TcpListener(listenAddress, Port);

            listener.Start();

            listener.BeginAcceptTcpClient(new AsyncCallback(ConnectCallback), listener);

            m_mreConnected.WaitOne();

            //--------------------------------------------------------------
            //  Client has connected at this point
            //--------------------------------------------------------------
            RaiseTCPMessageEvent("Connected");

            if (Client != null && Client.Connected == true)
            {
                stream = Client.GetStream();
            }

            while (m_bExit == false)
            {
                if (stream.DataAvailable == true && m_bExit == false)
                {
                    //try
                    //{
                        ftp.wait4handshake(Client, "Server: Waiting for client to open file");
                        RaiseTCPMessageEvent("Server: Still Connected..." + " \n");
                        ftp.sendhandshake(Client, "Server: Ready for command");
                        string command = ftp.getCommand(Client);
                        ftp.sendhandshake(Client, "Server: Recieved command");

                        if (command.Contains("TransferFile"))
                        {
                            RaiseTCPMessageEvent("Server: ReceivingFile..." + "\n");

                            string path = ftp.getFileName(Client);

                            path = (path.ToString());

                            string[] parsed_path = path.Split('\\');
                            string filename = parsed_path[parsed_path.Length - 1];
                            int indexToRemove = Array.FindIndex(parsed_path, row => row == filename);
                            parsed_path = parsed_path.Where((source, index) => index != indexToRemove && index > 4).ToArray();
                            string filedirectory = "C:\\DatTransImage\\" + String.Join("\\", parsed_path);
                            if (!Directory.Exists(filedirectory))
                            {
                                Directory.CreateDirectory(filedirectory);
                            }
                            ftp.sendhandshake(Client, "Server: received filename - " + path);
                            RaiseTCPFileNameEvent(filename);
                            FileStream file = new FileStream((filedirectory + "\\" + filename).TrimEnd('\0'), FileMode.OpenOrCreate, FileAccess.ReadWrite);
                            
                            if (file.Length > 0)
                            {
                                file.Seek(file.Length, SeekOrigin.Begin);
                            }

                            ftp.wait4handshake(Client, "Server: Ready for filesize");
                            ftp.sendfilesize(Client, (file.Length).ToString());
                            
                            RaiseTCPMessageEvent("Server: Trying to get data stream");
                            
                            float payload_size = ftp.getpayloadbytes(Client, "Server: Waiting for payload bytes");
                            
                            ftp.sendhandshake(Client, "Server: received payload size - " + payload_size.ToString());

                            float totalbytesreceived = (float)file.Length;
                            bool lastpacket = false;

                            RaiseTCPMessageEvent("Server: Writing to file..." + filename.TrimEnd('\0'));

                            while (totalbytesreceived < payload_size)
                            {
                                var output = ftp.getData(Client);

                                MemoryStream ms = new MemoryStream(output.Item1);
                                ms.WriteTo(file);
                                ms.Flush();

                                lastpacket = output.Item2;

                                totalbytesreceived = totalbytesreceived + (output.Item1).Length;

                                output = new Tuple<byte[], bool>(null, false);
                                ftp.sendhandshake(Client, "Server: Received packet");
                                ftp.GetLastLastHB = DateTime.Now;
                            }

                            //Thread.Sleep(3000);
                            ftp.wait4handshake(Client, "Server: Received datastream");

                            //RaiseTCPMessageEvent("Server: Total bytes received..." + ((double)totalbytesreceived).ToString());

                            //RaiseTCPMessageEvent("Server: Trying to create file..." + filename.TrimEnd('\0'));

                            //RaiseTCPMessageEvent("Server: Writing to file..." + filename.TrimEnd('\0'));

                            //RaiseTCPMessageEvent("Server: Wrote to file..." + filename.TrimEnd('\0'));

                            file.Flush();
                            file.Close();

                            ftp.sendhandshake(Client, "Server: wrote filestream");
                            ftp.GetLastLastHB = DateTime.Now;
                        }
                    //}
                    //catch (Exception exc)
                    //{
                    //    ProgramLog.Log(exc.ToString());
                    //    Console.WriteLine("Server: Connection Lost..." + " \n");
                    //    //Client.Dispose();
                    //    //Client.Close();
                    //}
                }
                //else 
                //{
                //--------------------------------------------------------------
                //  Make sure we are receiving the heartbeat
                //--------------------------------------------------------------
                ts = DateTime.Now - ftp.GetLastLastHB;

                if (ts.TotalSeconds >= 60.0)
                {
                    //--------------------------------------------------------------
                    //  Get heartbeat message
                    //--------------------------------------------------------------
                    try
                    {
                        ftp.SendHeartBeat(Client);
                    }
                    catch (Exception ex)
                    {
                        RaiseConnectionEvent(ConnectionStatus.Lost);
                        RaiseTCPMessageEvent("Lost Connection");
                        //RaiseTCPMessageEvent(ex.ToString());
                        ProgramLog.Log(ex.ToString());
                            
                        //Start new thread
                        ThreadPool.QueueUserWorkItem(new WaitCallback(StartServer));
                            
                        //Exit this thread
                        return;

                    }
                }
                //}
                //--------------------------------------------------------------
                //  Give other threads a chance to run
                //--------------------------------------------------------------
                Thread.Sleep(100);
            }
        }

        private void ServerThread(object obj)
        {
            //GetMessage();
            GetFiles();
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;
            Client = listener.EndAcceptTcpClient(ar);
            listener.Stop();
            listener = null;
            Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            Client.NoDelay = true;
            LingerOption linger = new LingerOption(true, 0);
            Client.LingerState = linger;

            RaiseConnectionEvent(ConnectionStatus.Connected);
            m_mreConnected.Set();
        }

        private void CheckReceivedMessages(object obj)
        {
            while (m_bExit == false)
            {
                if (ReceiveQueue.Count > 0)
                {
                    RaiseMessageReceivedEvent(ReceiveQueue.Dequeue());
                }
                Thread.Sleep(1);
            }
        }

        private void RaiseConnectionEvent(ConnectionStatus status)
        {
            if (ConnectionEvent != null)
            {
                ConnectionEvent(status);
            }
        }
        private void RaiseTCPMessageEvent(string szMsg)
        {
            if (TCPClientMessageEvent != null)
            {
                TCPClientMessageEvent(szMsg);
            }
        }

        private void RaiseTCPFileNameEvent(string filename)
        {
            if (TCPClientFileNameEvent != null)
            {
                TCPClientFileNameEvent(filename);
            }
        }

        private void RaiseMessageReceivedEvent(string szMsg)
        {
            if (HandleMessageReceivedEvent != null)
            {
                HandleMessageReceivedEvent(szMsg);
            }
        }
    }
}
