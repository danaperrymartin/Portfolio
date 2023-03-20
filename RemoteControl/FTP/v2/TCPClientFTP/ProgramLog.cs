using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TCPClientFTP
{
	public class ProgramLog
	{
		//-------------------------------------------------------------------------------------------------
		//	CONSTANTS
		//-------------------------------------------------------------------------------------------------
		private const string LOG_NAME = "ProgramLog.txt";
        private const int MEGABYTE = 1048576;

        private const long MAX_LOG_FILE_SIZE_IN_BYTES = 2 * MEGABYTE;

		//-------------------------------------------------------------------------------------------------
		//	PUBLIC
		//-------------------------------------------------------------------------------------------------

		//-------------------------------------------------------------------------------------------------
		//	PRIVATE
		//-------------------------------------------------------------------------------------------------

		//--------------------------------------------------------------------------------
		//  Special static variable to allow a class static function call to log data
		//  without having to instantiate multiple instances of the class
		//--------------------------------------------------------------------------------
		private static StreamWriter m_sw = null;

		private static object m_lockObj;

		//*************************************************************************************************
		//
		//	CONSTRUCTORS/DESTRUCTORS
		//
		//*************************************************************************************************

		//*************************************************************************************************
		//
		//	PUBLIC MEMBERS
		//
		//*************************************************************************************************

		/// <summary>
		/// Initializes/Opens the exception log
		/// </summary>
		/// <param name="szPath">Location to place log</param>
		/// <returns>True if opened successfully</returns>
		public static bool Init( string szPath )
		{
			bool bSuccess = true;
			string szFull = szPath + @"\" + LOG_NAME;

			m_lockObj = new object();

			try
			{
                //------------------------------------------------------
                //  Handle case where log is full
                //------------------------------------------------------
                CreateOldLogIfNeeded( szFull );

				//------------------------------------------------------
				//	Create or append the exception log
				//------------------------------------------------------
				m_sw = new StreamWriter( szFull, true, Encoding.ASCII );

				//------------------------------------------------------
				//	Add a Timestamp
				//------------------------------------------------------
				m_sw.WriteLine();
				m_sw.WriteLine( "---------------------------------------------------------" );
				m_sw.WriteLine( DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() );
				m_sw.WriteLine( "---------------------------------------------------------" );
				m_sw.WriteLine();
				m_sw.Flush();
                m_sw.Close();
			}
			catch
			{
				m_sw = null;
				bSuccess = false;
			}

			return bSuccess;
		}

        /// <summary>
        /// 
        /// Uses the executable path as the program log path
        /// </summary>
        /// <returns> true if file created</returns>
        public static bool Init()
        {
            string szAppPath = string.Empty;
            int iPos;

            szAppPath = Application.ExecutablePath;
            iPos = szAppPath.LastIndexOf('\\');
            szAppPath = szAppPath.Substring( 0, iPos );

            return Init(szAppPath);
        }
		/// <summary>
		/// Adds log message to exception log file
		/// </summary>
		/// <param name="szMsg">Message to write to disk</param>
		public static void Log( string szMsg )
		{
			if( m_sw != null )
			{
				lock( m_lockObj )
                {
                    string szAppPath = string.Empty;
                    int iPos;

                    szAppPath = Application.ExecutablePath;
                    iPos = szAppPath.LastIndexOf('\\');
                    szAppPath = szAppPath.Substring(0, iPos);
                    szAppPath += @"\" + LOG_NAME;
                    m_sw = new StreamWriter(szAppPath, true, Encoding.ASCII);

					DateTime dt = DateTime.Now;
					string szDT = dt.Hour.ToString( "D2" ) + ":" + dt.Minute.ToString( "D2" ) + ":" + dt.Second.ToString( "D2" ) + "." + dt.Millisecond.ToString( "D3" );
					m_sw.WriteLine( szDT + " " + szMsg );
					m_sw.Flush();
                    m_sw.Close();
				}
			}
		}

		//*************************************************************************************************
		//
		//	PRIVATE MEMBERS
		//
		//*************************************************************************************************

        private static void CreateOldLogIfNeeded( string szPath )
        {
            string szNewPath = szPath.Replace( ".txt", ".old");

            if (File.Exists(szPath) == true)
            {
                try
                {
                    FileInfo fi = new FileInfo(szPath);

                    if (fi.Length > MAX_LOG_FILE_SIZE_IN_BYTES)
                    {
                        //------------------------------------------------------
                        //  Delete old file
                        //------------------------------------------------------
                        File.Delete(szNewPath);

                        //------------------------------------------------------
                        //  Rename file to old
                        //------------------------------------------------------
                        File.Move(szPath, szNewPath);
                    }
                }
                catch
                {
                    //------------------------------------------------------
                    //  Ignore error
                    //------------------------------------------------------
                }
            }
        }
	}
}
