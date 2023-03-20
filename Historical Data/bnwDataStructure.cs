using System;

namespace Historical_Data
{
	public class bnwDataStructure
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
		public bnwDataStructure()
        {
			//--------------------------------------------------------------
			//	Init member variables
			//--------------------------------------------------------------
			Time = DateTime.Now;
			Site = null;
			TrackNumber = 0;
			TrainDirection =' ';
			TrainSpeed = 0;
			Locos = 0;
			SlaveLocos = 0;
			Cars = 0;
			LocoAxles = 0;
			SlaveAxles = 0;
			CarAxles = 0;
			LocoTon = 0;
			CarTon = 0;
			ExtTemp = 0;
			IntTemp = 0;
			RelHum = 0;
			WindSpd = 0;
			WindDir = 0;
			Validity = null;
			TrainType = null;
			VehicleInitial = null;
			VehicleNumber = 0;
			CarOrientation = null;
			CarWeight = 0;
			CarConfidence = 0;
			VehicleType = null;
			CarStatus = null;
			CarTruckCount = 0;
			CarAxleCount = 0;
			CarOrderNum = 0;
			CheckStatus = null;
			CarSpeed = 0;
			TruckNumber = null;
		    TruckWeight = 0;
			TruckConfidence = 0;
			Wheelspace = 0;
			HuntingIndex = 0;
			VehicleSide = null;
			VehicleAxleNum = 0;
			AvgVertF = 0;
			MaxVertF = 0;
			AvgLatF = 0;
			MaxLatF = 0;
	}

		//*********************************************************************************************************************************************
		//
		//	PUBLIC
		//
		//*********************************************************************************************************************************************

		public DateTime Time { set; get; }
		public string Site { set; get; }
		public int TrackNumber { set; get; }
		public char TrainDirection { set; get; }
		public float TrainSpeed { set; get; }
		public int Locos { set; get; }
		public int SlaveLocos { set; get; }
		public int Cars { set; get; }
		public int LocoAxles { set; get; }
		public int SlaveAxles { set; get; }
		public int CarAxles { set; get; }
		public int LocoTon { set; get; }
		public int CarTon { set; get; }
		public float ExtTemp { set; get; }
		public float IntTemp { set; get; }
		public float RelHum { set; get; }
		public float WindSpd { set; get; }
		public int WindDir { set; get; }
		public string Validity { set; get; }
		public string TrainType { set; get; }
		public string VehicleInitial { set; get; }
		public int VehicleNumber { set; get; }
		public string CarOrientation { set; get; }
		public float CarWeight { set; get; }
        public float CarConfidence { set; get; }
		public string VehicleType { set; get; }
		public string CarStatus { set; get; }
		public int CarTruckCount { set; get; }
		public int CarAxleCount { set; get; }
		public int CarOrderNum { set; get; }
		public string CheckStatus { set; get; }
		public float CarSpeed { set; get; }
		public string TruckNumber { set; get; }
		public float TruckWeight { set; get; }
		public float TruckConfidence { set; get; }
		public int Wheelspace { set; get; }
		public float HuntingIndex { set; get; }
		public string VehicleSide { set; get; }
		public int VehicleAxleNum { set; get; }
		public float AvgVertF { set; get; }
		public float MaxVertF { set; get; }
		public float AvgLatF { set; get; }
		public float MaxLatF { set; get; }
		//*********************************************************************************************************************************************
		//
		//	PRIVATE
		//
		//*********************************************************************************************************************************************
	}
}
