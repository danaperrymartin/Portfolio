using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Drawing;

namespace GetTrainImages
{
    class Program
    {
        //---------------------------------------------------------------------------------------------------------------------------------------------
        //  PRIVATE
        //---------------------------------------------------------------------------------------------------------------------------------------------
        private int m_iConnectionCount;

        public static void Main()
        {
            TrainImageDB t = new TrainImageDB();
            var prgm = new Program();
            //Console.WriteLine("Get Train Image (1) or Car Image (2):");

            //int CARorTRAIN = Convert.ToInt32(Console.ReadLine());
            //if (CARorTRAIN == 1)
            //{
            //    Console.WriteLine("Enter Train Index:");
            //    int iTrainIndex = Convert.ToInt32(Console.ReadLine());
            //    byte[] image = prgm.GetTrainImageDB(iTrainIndex,TrainImageDB t);
            //    File.WriteAllBytes("Image.jpg", image);
            //}
            //if (CARorTRAIN == 2)
            //{
            bool again = true;
            while(again)
            {
                Console.WriteLine("Enter Car Index:");
                int iCarIndex = Convert.ToInt32(Console.ReadLine());
                byte[] image = prgm.GetImageDBBigImage(iCarIndex);
                File.WriteAllBytes("Image" + iCarIndex.ToString() + ".jpg", image);
            }
                //}
            }

            public byte[] GetImageDBBigImage(int iCarIndex)
        {
            byte[] baImage = null;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = ConnectionImageDB();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Get_BigImageByCarIndex";

            SqlParameter param = new SqlParameter("iCarIndex", SqlDbType.Int);
            param.Value = iCarIndex;
            cmd.Parameters.Add(param);

            try
            {
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read() == true)
                {
                    baImage = (byte[])reader["FullImage"];
                }
            }
            catch (Exception ex)
            {
                int q = 9;
            }

            cmd.Connection.Close();
            m_iConnectionCount--;

            return baImage;
        }

        public void GetTrainImageDB(int iTrainIndex, ref TrainImageDB t)
        {
            //--------------------------------------------------------------
            //  Look up train based on train index
            //--------------------------------------------------------------
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = ConnectionImageDB();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Get_CarByIndex";

            SqlParameter param = new SqlParameter("iCarIndex", SqlDbType.Int);
            param.Value = iTrainIndex;
            cmd.Parameters.Add(param);

            try
            {
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read() == true)
                {
                    t.Index = (int)reader["Index"];
                    t.SiteIndex = (int)reader["SiteIndex"];
                    t.Datetime = (DateTime)reader["DateTime"];
                    t.TrackNum = (byte)reader["TrackNum"];
                    t.Direction = ((string)reader["Direction"])[0];
                    t.NumCars = (short)reader["CarCount"];
                    t.LeadLoco = (string)reader["FirstCar"];
                }
            }
            catch (Exception ex)
            {
                int q = 9;
            }

            cmd.Connection.Close();
            m_iConnectionCount--;
        }

        private SqlConnection ConnectionImageDB()
        {
            SqlConnection newConnection = null;

            newConnection = new SqlConnection("Data Source = Server; Initial Catalog = images; User ID = prt; Password = smrmprt;connection timeout=10");

            try
            {
                newConnection.Open();
                m_iConnectionCount++;
            }
            catch (Exception ex)
            {
                newConnection = null;
            }

            return newConnection;
        }

        public class TrainImageDB
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

            //*********************************************************************************************************************************************
            //
            //  CONSTRUCTORS/DESTRUCTORS/CLEANUP
            //
            //*********************************************************************************************************************************************
            /// <summary>
            /// Default constructor
            /// </summary>
            public TrainImageDB()
            {
                //--------------------------------------------------------------
                //  Init member variables
                //--------------------------------------------------------------
                Index = -1;
                SiteIndex = -1;
                Datetime = DateTime.Now;
                TrackNum = 0;
                Direction = ' ';
                NumCars = 0;
                LeadLoco = string.Empty;
            }
            //*********************************************************************************************************************************************
            //
            //  PUBLIC
            //
            //*********************************************************************************************************************************************
            public int Index { set; get; }
            public int SiteIndex { set; get; }
            public DateTime Datetime { set; get; }
            public int TrackNum { set; get; }
            public char Direction { set; get; }
            public int NumCars { set; get; }
            public string LeadLoco { set; get; }


            public string DateTimeFormatted
            {
                get
                {
                    string szTime = Datetime.Year.ToString() + "/" + Datetime.Month.ToString("D2") + "/" + Datetime.Day.ToString("D2") + " " + Datetime.Hour.ToString("D2") + ":" + Datetime.Minute.ToString("D2") + ":" + Datetime.Second.ToString("D2");

                    return szTime;
                }
            }
            //*********************************************************************************************************************************************
            //
            //  PRIVATE
            //
            //*********************************************************************************************************************************************
        }
    }
}
