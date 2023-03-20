using Elasticsearch.Net;
using Nest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO.MemoryMappedFiles;

namespace PlayTrainVideo
{
    public class LoadFile
    {
        //*********************************************************************************************************************************************
        //  PUBLIC
        //*********************************************************************************************************************************************
        public static Bitmap bmp;
        public static Bitmap bmp_Last;
        public MemoryMappedFile mmf;
        //*********************************************************************************************************************************************
        //
        //  PRIVATE
        //
        //*********************************************************************************************************************************************
        private List<byte[]> m_LineListOriginal;
        private List<byte[]> m_LineList2;
        private Form1 frm1;
        private LoadFile _instance;

        //*********************************************************************************************************************************************
        //
        //  CONSTRUCTORS/DESTRUCTORS/CLEANUP
        //
        //*********************************************************************************************************************************************
        public LoadFile()
        {
            m_LineListOriginal = new List<byte[]>();
            m_LineList2 = new List<byte[]>();
            frm1 = null;
            _instance = this;
        }

        ~LoadFile(){}

        public LoadFile _GetInstance()
        {
            if (_instance != null) return _instance;
            return _instance = new LoadFile();
        }

        public void Process(string FileName, int BmpHeight)
        {
            loadfile(FileName);
            //BeginInvoke(m_UpdateHScrollDlgt);
        }

        public void loadfile_Archive(string szFileName, int iLineSize)
        {
            //animateImage aI = new animateImage();
            long lCount = 0;
            m_LineListOriginal.Clear();

            if (m_LineList2 != null)
            {
                m_LineList2.Clear();
            }

            int iPos = szFileName.LastIndexOf('\\');
            string szTemp = szFileName.Substring(iPos + 1);

            iPos = szTemp.IndexOf("StartTime");

            if (iPos != -1)
            {
                szTemp = szTemp.Substring(0, iPos);
            }

            BinaryReader br = new BinaryReader(File.Open(szFileName, FileMode.Open));

            int iNumSteps = (int)(br.BaseStream.Length / iLineSize);

            int iOffset = 0;

            while (lCount < br.BaseStream.Length)
            {
                byte[] baLine = new byte[iLineSize];

                for (int i = 0; i < iLineSize; i++)
                {
                    baLine[i] = br.ReadByte();
                    lCount++;
                }

                bmp = GetBitmap(iOffset, baLine.Length, true, false);
                //aI.AnimateImage(bmp);
                iOffset = 0;

                m_LineListOriginal.Add(baLine);
            }

            br.Close();

            //Image image = GetBitmap(0, m_LineListOriginal.Count, true, false);
            //BeginInvoke(m_UpdateImageDlgt, image);
        }

        public int loadfile(string szFileName)
        {
            
            //int resolution1 = 2048;
            //int resolution2 = 20000;
            //BinaryReader binReader = new BinaryReader(File.Open(szFileName, FileMode.Open));
            //long arraylength = binReader.BaseStream.Length;
            //buffer = new char[0];
            //binReader.Read(buffer, 0, (int)binReader.BaseStream.Length);
            //if (buffer.Length != binReader.BaseStream.Length / resolution1)
            //{
            //    throw new Exception("Reading Error");
            //}

            mmf = MemoryMappedFile.CreateFromFile(@szFileName, FileMode.Open, "ImgA");

            return 1;
        }

        public Bitmap GetBitmap(int iOffset, int iWidth, bool bProcessed, bool bFlip)
        {
            frm1 = Form1._GetInstance();
            //------------------------------------------------------
            //  Create grayscale bitmap palette
            //------------------------------------------------------
            for (int i = 0; i < 256; i++)
            {
                frm1.GrayScalePalette.Entries[i] = Color.FromArgb(i, i, i);
            }

            List<Byte[]> LineList = null;

            //if (bProcessed == false)
            //{
            LineList = m_LineListOriginal;
            //}
            //else
            //{
            //    LineList = m_LineList2;
            //}

            if (LineList.Count >= iWidth)
            {
                bmp = new Bitmap(iWidth, LineList[0].Length, PixelFormat.Format8bppIndexed);
                bmp.Palette = frm1.GrayScalePalette;
                Rectangle rect = new Rectangle(0, 0, iWidth, LineList[0].Length);

                BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
                List<int> DetectList = new List<int>();

                if (iOffset >= 0 &&
                    iOffset + iWidth < LineList.Count)
                {
                    unsafe
                    {
                        //--------------------------------------------------------------
                        //  Go through the bitmap top to bottom, across lines
                        //--------------------------------------------------------------
                        for (int i = 0; i < LineList[0].Length; i++)
                        {
                            byte* bypDst = (byte*)(bmpData.Scan0 + i * bmpData.Stride);

                            if (bFlip == false)
                            {
                                for (int j = iOffset; j < iOffset + iWidth; j++)
                                {
                                    *bypDst = LineList[j][i];
                                    bypDst++;
                                }
                            }
                            else
                            {
                                for (int j = iOffset + iWidth - 1; j >= iOffset; j--)
                                {
                                    *bypDst = LineList[j][i];
                                    bypDst++;
                                }
                            }
                        }
                    }
                }
                bmp.UnlockBits(bmpData);
            }
            return bmp;
        }

        private static void AnimateImage(Bitmap bmp)
        {
            double maxFPS = 100;
            double minFramePeriodMsec = 1000.0 / maxFPS;
            Stopwatch stopwatch = Stopwatch.StartNew();

            Bitmap bmp_last = bmp;
            //if (!currentlyAnimating)
            //{
            //    Begin the animation only once.
            //    ImageAnimator.Animate(image, new EventHandler(this.OnFrameChanged));
            //    ProgressUpdate(this, new animateImage.OnFrameChanged);
            //    currentlyAnimating = true;
            //}

            lock (bmp_Last)
            {
                bmp_Last.Dispose();
                bmp_Last = (Bitmap)bmp.Clone();
                //UpdateImage(bmp_Last);
            }
            // FPS limiter
            double msToWait = minFramePeriodMsec - stopwatch.ElapsedMilliseconds;
            if (msToWait > 0)
                Thread.Sleep((int)msToWait);
            stopwatch.Restart();
        }
    }
}
