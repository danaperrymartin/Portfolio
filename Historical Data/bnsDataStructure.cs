﻿using System;

namespace Historical_Data
{
	public class bnsDataStructure
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
		public bnsDataStructure()
        {
			//--------------------------------------------------------------
			//	Init member variables
			//--------------------------------------------------------------
			CribNumber = 0;
			CribDirection = null;
			Time     = DateTime.Now;
			Validity = ' ';
			Trax = ' ';
			Crax = ' ';
			Axle = null;
			Truck = ' ';
			VLoad = 0;
			LForce = 0;
			TSLV = 0;
			SWLV = 0;
			ASLV = 0;
			AOA = 0;
		}

		//*********************************************************************************************************************************************
		//
		//	PUBLIC
		//
		//*********************************************************************************************************************************************

		
		public int CribNumber { set; get; }
		public string CribDirection { set; get; }
		public DateTime Time { set; get; }
		public char Validity { set; get; }
		public int Trax { set; get; }
		public int Crax { set; get; }
		public string Axle { set; get; }
		public char Truck { set; get; }
		public double VLoad { set; get; }
		public double LForce { set; get; }
		public double TSLV { set; get; }
		public double SWLV { set; get; }
		public double ASLV { set; get; }
		public double AOA { set; get; }



		//*********************************************************************************************************************************************
		//
		//	PRIVATE
		//
		//*********************************************************************************************************************************************
	}
}
