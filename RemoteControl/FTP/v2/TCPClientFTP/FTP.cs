using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPClientFTP
{
    class FTP
    {
        //---------------------------------------------------------------------------------------------------------------------------------------------
        //  CONSTANTS
        //---------------------------------------------------------------------------------------------------------------------------------------------

        //---------------------------------------------------------------------------------------------------------------------------------------------
        //  PUBLIC
        //---------------------------------------------------------------------------------------------------------------------------------------------
        public delegate void TCPClientMessageEventHandler(string szMsg);
        public event TCPClientMessageEventHandler TCPClientMessageEvent;

        public DateTime GetLastLastHB
        {
            set { tcp_client.dtLastHB = value; }
            get { return tcp_client.dtLastHB; }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------
        //  PRIVATE
        //---------------------------------------------------------------------------------------------------------------------------------------------
        private TCPClient tcp_client;
        private frmMain frmmain;
        
        //*********************************************************************************************************************************************
        //
        //  CONSTRUCTORS/DESTRUCTORS/CLEANUP
        //
        //*********************************************************************************************************************************************
        public FTP()
        {
            //--------------------------------------------------------------
            //  Init member variables
            //--------------------------------------------------------------
            //ProgramLog.Init();
            tcp_client = new TCPClient();
            //frmmain = new frmMain();

            tcp_client.dtLastHB = DateTime.Now;
        }

        public void wait4handshake(TcpClient client, string message)
        {
            tcp_client = new TCPClient();
            NetworkStream stream = client.GetStream();
            RaiseTCPMessageEvent(message);

            const int bytesize = 1024;
            byte[] buffer = new byte[bytesize];
            int bytesread = 0;

            while (client.Available > 0 || bytesread == 0)
            {
                bytesread = client.GetStream().Read(buffer, 0, bytesize); //reads stream and places into buffer.  Returns number of bytes read.
            }

            var data = ASCIIEncoding.ASCII.GetString(buffer);

            if(data.Contains("hb|"))
            {
                wait4handshake(client, message);
            }
            tcp_client.dtLastHB = DateTime.Now;

            RaiseTCPMessageEvent(data);

            //return true;
        }

        public void sendhandshake(TcpClient client, string handshake)
        {
            NetworkStream stream = client.GetStream();
            byte[] message = Encoding.ASCII.GetBytes(handshake);
            stream.Write(message, 0, message.Length);
        }

        public string getCommand(TcpClient client)
        {
            //tcp_client = new TCPClient();
            RaiseTCPMessageEvent("Server: Getting Command...");
            const int bytesize = 1024;
            byte[] buffer = new byte[bytesize];
            string data = null;
            int bytesread = 0;

            while (client.Available > 0 || bytesread == 0)
            {
                bytesread = client.GetStream().Read(buffer, 0, bytesize); //reads stream and places into buffer.  Returns number of bytes read.

                RaiseTCPMessageEvent("Server: Command Received was -- " + data);
            }

            tcp_client.dtLastHB = DateTime.Now;

            data = ASCIIEncoding.ASCII.GetString(buffer);
            return data;
        }

        public void sendCommand(NetworkStream stream, String command)
        {
            byte[] message = Encoding.ASCII.GetBytes(command);
            stream.Write(message, 0, message.Length);
            RaiseTCPMessageEvent("Client: Sent command - " + command);
        }

        public string getFileName(TcpClient client)
        {
            //tcp_client = new TCPClient();
            RaiseTCPMessageEvent("Server: Getting file name...");
            const int bytesize = 1024;
            byte[] buffer = new byte[bytesize];
            int bytesread = 0;
            var data = "";
            while (client.Available > 0 || bytesread == 0)
            {
                bytesread = client.GetStream().Read(buffer, 0, bytesize); //reads stream and places into buffer.  Returns number of bytes read. 
            }

            tcp_client.dtLastHB = DateTime.Now;

            data = ASCIIEncoding.ASCII.GetString(buffer);
            RaiseTCPMessageEvent("Server: Filename received is -- " + data);

            return data;
        }

        public void sendfilename(TcpClient client, string filename)
        {
            NetworkStream stream = client.GetStream();
            byte[] message = Encoding.ASCII.GetBytes(filename);
            stream.Write(message, 0, message.Length);
        }

        public void sendfilesize(TcpClient client, string filesize)
        {
            NetworkStream stream = client.GetStream();
            byte[] existingfilesize = Encoding.ASCII.GetBytes(filesize);
            stream.Write(existingfilesize, 0, existingfilesize.Length);
        }

        public float wait4filesize(TcpClient client, string message)
        {
            //tcp_client = new TCPClient();
            NetworkStream stream = client.GetStream();
            RaiseTCPMessageEvent(message);
            const int bytesize = 1024;
            byte[] buffer = new byte[bytesize];
            int bytesread = 0;

            while (client.Available > 0 || bytesread == 0)
            {
                bytesread = client.GetStream().Read(buffer, 0, bytesize); //reads stream and places into buffer.  Returns number of bytes read.
            }

            tcp_client.dtLastHB = DateTime.Now;

            var filesize = ASCIIEncoding.ASCII.GetString(buffer);

            return float.Parse(filesize);
        }

        public float getpayloadbytes(TcpClient client, string message)
        {
            //tcp_client = new TCPClient();
            NetworkStream stream = client.GetStream();
            RaiseTCPMessageEvent(message);
            const int bytesize = 1024;
            byte[] buffer = new byte[bytesize];
            int bytesread = 0;

            while (client.Available > 0 || bytesread == 0)
            {
                bytesread = client.GetStream().Read(buffer, 0, bytesize); //reads stream and places into buffer.  Returns number of bytes read.
            }

            tcp_client.dtLastHB = DateTime.Now;

            float payload_size = BitConverter.ToInt64(buffer, 0);
            
            return payload_size;
        }

        public Tuple<byte[], bool> getData(TcpClient client)
        {
            //tcp_client = new TCPClient();
            NetworkStream stream = client.GetStream();

            bool lastpacket = false;
            string packet_percent = getpacketpercent(client);
            sendhandshake(client, "Server: Recieved packet percent - " + packet_percent.TrimEnd('\0'));
            if (packet_percent.Contains("100"))
            {
                lastpacket = true;
            }
            else
            {
                lastpacket = false;
            }

            RaiseTCPMessageEvent("Server: Getting stream length");
            byte[] streamlength = new byte[1024];
            int bytesread = stream.Read(streamlength, 0, 1024);

            int bytesSent = 0;
            int bytesToRead = BitConverter.ToInt32(streamlength, 0);
            RaiseTCPMessageEvent("Server: Stream length..." + bytesToRead.ToString());
            sendhandshake(client, "Server: Recieved streamsize");

            byte[] bytes = new byte[bytesToRead];
            int bufferSize = 65536 / 2;

            int percent_completed = 0;
            while (bytesToRead > 0)
            {
                int curDataSize = Math.Min(bufferSize, bytesToRead);
                if (client.Available < curDataSize)
                {
                    curDataSize = client.Available;//this save me  
                }
                stream.Read(bytes, bytesSent, curDataSize);
                bytesSent += curDataSize;
                bytesToRead -= curDataSize;
                if ((((int)(((double)bytesSent / (double)(bytesToRead + bytesSent)) * 100)) - percent_completed) > 0)
                {
                    //frmmain.m_SetProgressBarDlgt(percent_completed);
                }
                percent_completed = (int)(((double)bytesSent / (double)(bytesToRead + bytesSent)) * 100);

                tcp_client.dtLastHB = DateTime.Now;
            }
            return Tuple.Create(bytes, lastpacket);
        }

        public void sendData(byte[] data, TcpClient client, long packet_percent)
        {
            NetworkStream stream = client.GetStream();
            sendpacketpercent(stream, packet_percent.ToString());
            wait4handshake(client, "Client: Waiting for verification server recieved packet_percent");
            int bufferSize = Math.Min(65536, data.Length);
            
            byte[] dataLength = BitConverter.GetBytes(data.Length);
            stream.Write(dataLength, 0, dataLength.Length); //Serves as sending a handshake
            wait4handshake(client, "Client: Waiting for verification server recieved streamsize");

            int bytesSent = 0;
            int bytesLeft = data.Length;
            while (bytesLeft > 0 && client.Connected)
            {
                int curDataSize = Math.Min(bufferSize, bytesLeft);
                stream.Write(data, bytesSent, curDataSize);
                bytesSent += curDataSize;
                bytesLeft -= curDataSize;
            }
            GetLastLastHB = DateTime.Now;
            bool success = true;
            stream.Flush();
        }

        private string getpacketpercent(TcpClient client)
        {
            //tcp_client = new TCPClient();
            RaiseTCPMessageEvent("Server: Getting packet percent...");
            const int bytesize = 1024;
            byte[] buffer = new byte[bytesize];
            string bytesread = client.GetStream().Read(buffer, 0, bytesize).ToString(); //reads stream and places into buffer.  Returns number of bytes read.
            var data = ASCIIEncoding.ASCII.GetString(buffer);

            tcp_client.dtLastHB = DateTime.Now;

            RaiseTCPMessageEvent("Server: Packet percent complete is -- " + data + "%");

            return data;
        }

        private void sendpacketpercent(NetworkStream stream, String packet_percent)
        {
            byte[] message = Encoding.ASCII.GetBytes(packet_percent);
            stream.Write(message, 0, message.Length);
        }

        public void SendHeartBeat(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] baData = System.Text.Encoding.ASCII.GetBytes("hb|");
            stream.Write(baData, 0, 3);
            tcp_client.dtLastHB = DateTime.Now;
        }

        public void GetHeartBeat(TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            const int bytesize = 1024;
            byte[] buffer = new byte[bytesize];
            int bytesread = 0;

            while (client.Available > 0 || bytesread == 0)
            {
                bytesread = client.GetStream().Read(buffer, 0, bytesize); //reads stream and places into buffer.  Returns number of bytes read.
            }

            var data = ASCIIEncoding.ASCII.GetString(buffer);

            if (data.Contains("hb|"))
            {
                tcp_client.dtLastHB = DateTime.Now;
            }
            else
            {
                RaiseTCPMessageEvent("Missed heart beat...");
            }
            //return true;
        }

        public void DirSearch(string sDir, List<string> files)
        {
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    foreach (string f in Directory.GetFiles(d))
                    {
                        files.Add(f);
                    }
                    DirSearch(d, files);
                }
            }
            catch (System.Exception excpt)
            {
               ProgramLog.Log(excpt.Message.ToString());
            }
        }

        public void DeleteEmptyDir(string sDir)
        {
            foreach (string d in Directory.GetDirectories(sDir))
            {
                if (Directory.GetFiles(d).Length == 0)
                {
                    Directory.Delete(d, true);
                }
                //DeleteEmptyDir(d);
            }
        }

        private void RaiseTCPMessageEvent(string szMsg)
        {
            if (TCPClientMessageEvent != null)
            {
                TCPClientMessageEvent(szMsg);
            }
        }

        
    }
}
