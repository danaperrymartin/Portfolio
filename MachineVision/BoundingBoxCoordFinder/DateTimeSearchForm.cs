using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace BoundingBoxCoordFinder
{
    public partial class DateTimeSearchForm : Form
    {

        //---------------------------------------------------------------------------------------------------------------------------------------------
        //  PRIVATE
        //---------------------------------------------------------------------------------------------------------------------------------------------
        private int m_iConnectionCount;
        private int TotalTrains;

        public DateTimeSearchForm(List<TrainImageDB> ImageList)
        {
            InitializeComponent();
            m_iConnectionCount = 0;
            TotalTrains = 0;
            ImageList = null;
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

        public List<CarImageDB> CarList { set; get; }

        private void btn_search_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = ConnectionImageDB();
            cmd.CommandType = CommandType.StoredProcedure;

            if (rdiobtn_CarSearch.Checked)
            {
                List<CarQuery> CarQueryList = new List<CarQuery>();
                string szLine = string.Empty;
                string[] saLine = null;

                if (textBox1.Text != string.Empty)
                {
                    //--------------------------------------------------------------
                    //  Get lines to start with
                    //--------------------------------------------------------------
                    string[] saVals = textBox1.Text.Split('\n');

                    //--------------------------------------------------------------
                    //  Go through lines and break out individual cars
                    //--------------------------------------------------------------
                    for (int i = 0; i < saVals.Length; i++)
                    {
                        szLine = saVals[i].Replace("\r", "");

                        //--------------------------------------------------------------
                        //  Handle case where only 1 car is listed
                        //--------------------------------------------------------------
                        if (szLine.Contains(",") == false)
                        {
                            if (szLine != string.Empty)
                            {
                                saLine = szLine.Split(' ');
                                CarQuery cq = new CarQuery();

                                cq.Owner = saLine[0];
                                cq.CarNumber = int.Parse(saLine[1]);
                                CarQueryList.Add(cq);
                            }
                        }
                        else
                        {
                            //--------------------------------------------------------------
                            //  Multiple cars, same owner
                            //--------------------------------------------------------------
                            saLine = szLine.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                            string szOwner = saLine[0];

                            for (int j = 1; j < saLine.Length; j++)
                            {
                                CarQuery cq = new CarQuery();

                                cq.Owner = szOwner;
                                cq.CarNumber = int.Parse(saLine[j].Trim());
                                CarQueryList.Add(cq);

                            }
                        }
                    }
                    
                    cmd.CommandText = "CarQueryList";
                    CarList = CreateCarList(CarQueryList);
                    List<CarImageDB> ImageList = CarList;
                }
            }
            else if (rdiobtn_TrainSearch.Checked)
            {
                List<Car> carList = new List<Car>();
                string FirstCar = string.Empty;

                //--------------------------------------------------------------
                //  Different searches based on fields filled out
                //--------------------------------------------------------------

                if (textBox1.Text != string.Empty)
                {
                    //--------------------------------------------------------------
                    //  Create a car query based on input
                    //--------------------------------------------------------------
                    string[] saLine = textBox1.Text.Split(' ');

                    CarQuery cq = new CarQuery();

                    if (saLine.Length == 2)
                    {
                        cq.Owner = saLine[0];
                        cq.CarNumber = int.Parse(saLine[1]);

                        FirstCar = cq.Owner + cq.CarNumber.ToString();
                    }
                    else
                    {
                        FirstCar = textBox1.Text;
                    }
                }

                string SearchSiteName = "Armorel";
                string TrackNum = "Track 1";
                string Direction = "North";

                 CreateTrainList();
            }
            
            this.Close();
        }

        private List<CarImageDB> CreateCarList(List<CarQuery> carQueryList)
        {
            List<CarImageDB> carList = new List<CarImageDB>();
            DateTime dtStart = DateTime.Now;
            DateTime dtStop = DateTime.Now;
            Database DB = new Database();

            dtStart = (DateTime)dateTimePicker1.Value;
            dtStop  = (DateTime)dateTimePicker2.Value;

            //--------------------------------------------------------------
            //  Handle special case of same day
            //--------------------------------------------------------------
            if (dtStart == dtStop)
            {
                dtStop = dtStop.AddDays(1);
            }

            if (DB != null)
            {
                if (carQueryList.Count > 0)
                {
                    //--------------------------------------------------------------
                    //  Go through each query and create a car
                    //--------------------------------------------------------------
                    for (int i = 0; i < carQueryList.Count; i++)
                    {
                        CarQuery cq = carQueryList[i];

                        DB.GetCarImageDB(cq, dtStart, dtStop, carList, "Near Truck Side");
                    }
                }
                else
                {
                    DB.GetCarsByDateImageDB(dtStart, dtStop, carList, "Near Truck Side");
                }
            }

            return carList;
        }

        private List<TrainImageDB> TrainList { set; get; }
        List<CarQuery> CarQueryList { set; get; }
        private List<TrainImageDB> CreateTrainList()
        {
            TrainList = new List<TrainImageDB>();
            DateTime dtStart = DateTime.Now;
            DateTime dtStop = DateTime.Now;

            //--------------------------------------------------------------
            //  Get the query list from the session
            //--------------------------------------------------------------
            Database DB = new Database();

            if (DB != null)
            {
                dtStart = (DateTime)dateTimePicker1.Value;
                dtStop = (DateTime)dateTimePicker2.Value;

                //--------------------------------------------------------------
                //  Handle special case of same day
                //--------------------------------------------------------------
                if (dtStart == dtStop)
                {
                    dtStop = dtStop.AddDays(1);
                }

                string szSiteName = (string)"Armorel";
                int iSiteIndex = DB.GetSiteIndex(szSiteName);

                string szTrackNum = (string)"TrackNum";
                int iTrackNum = 0;

                if (szTrackNum != string.Empty)
                {
                    iTrackNum = int.Parse(szTrackNum);
                }

                string szDir = (string)"Direction";

                if (szDir == string.Empty)
                {
                    szDir = "z";
                }

                int iPage = (int)0;

                int iStart = iPage * 1;

                string szFirstCar = (string)"FirstCar";

                if (szFirstCar == string.Empty)
                {
                    TotalTrains = DB.GetTrainsOnePageArmorel("z", dtStart, dtStop, iSiteIndex, iTrackNum, szDir, iStart, 1, TrainList);
                }
            }
            return TrainList;
        }
    }
}
