using System;

namespace Historical_Data
{
	public class trnDataStructure
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

		///Default constructor
		public trnDataStructure()
		{
            //--------------------------------------------------------------
            //	Init member variables
            //--------------------------------------------------------------
            PeakNumber = 0;
            PeakTime = DateTime.Now;
            MaxVertF = 0;
            MaxLatF = 0;
            AOAtime = 0;
            LPVert = 0;
            TrackType = null;
            CribNumber = 0;
            RailType = null;
            Orientation = null;
            AxleCount = 0;
            Position = 0;
            CarEnd = null;
            Owner = null;
            CarNumber = 0;
            CarType = 0;
            CarAxle = 0;
            CarTime = DateTime.Now;
            TimeSinceTag = 0;
            ReadPort = 0;
            TotalTrainAxle = 0;
        }

		//*********************************************************************************************************************************************
		//
		//	PUBLIC
		//
		//*********************************************************************************************************************************************
		
		public int PeakNumber { set; get; }
		public DateTime PeakTime { set; get; }
		public float MaxVertF { set; get; }
		public float MaxLatF { set; get; }
		public float AOAtime { set; get; }
		public float LPVert { set; get; }
		public string TrackType { set; get; }
		public int CribNumber { set; get; }
		public string RailType { set; get; }
		public string Orientation{set; get;}
		public int AxleCount { set; get; }
		public int Position { set; get; }
		public string CarEnd { set; get; }
		public string Owner { set; get; }
		public int CarNumber { set; get; }
		public int CarType { set; get; }
		public int CarAxle { set; get; }
		public DateTime CarTime { set; get; }
		public float TimeSinceTag { set; get; }
		public int ReadPort { set; get; }
		public int TotalTrainAxle { set; get; }
		//*********************************************************************************************************************************************
		//
		//	PRIVATE
		//
		//*********************************************************************************************************************************************
	}
}
