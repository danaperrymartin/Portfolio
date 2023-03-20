using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Drawing.Imaging;
using System.Threading;
using System.Diagnostics;
using static System.Net.WebRequestMethods;
using iText.Kernel.Colors;
using iText.IO.Image;
using System.IO.MemoryMappedFiles;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;
using System.Windows;
using System.Runtime.InteropServices;

namespace PlayTrainVideo
{
    public partial class Form1 : Form
    {
        //---------------------------------------------------------------------------------------------------------------------------------------------
        //  PUBLIC
        //---------------------------------------------------------------------------------------------------------------------------------------------
        public ColorPalette GrayScalePalette { set; get; }

        public delegate void UpdateImageDlgt();
        public UpdateImageDlgt m_UpdateImageDlgt;

        Bitmap bmp;
        Bitmap bmp_t;

        //*********************************************************************************************************************************************
        //
        //  PRIVATE
        //
        //*********************************************************************************************************************************************
        private List<byte[]> m_LineListOriginal;
        private List<byte[]> m_LineList2;
        private string FileName { set; get; }
        private DateTime CameraStartTime { set; get; }
        private int BmpHeight { set; get; }

        private static Form1 _instance;
        LoadFile lf;
        private long offset;
        private int numberoflines;

        private int img_height;
        private int img_width;
        //*********************************************************************************************************************************************
        //
        //  CONSTRUCTORS/DESTRUCTORS/CLEANUP
        //
        //*********************************************************************************************************************************************
        public Form1()
        {
            InitializeComponent();

            m_UpdateImageDlgt = new UpdateImageDlgt(UpdateImage);

            //var renderThread = new Thread(new ThreadStart(RenderForever));
            //renderThread.Start();

            bmp   = new Bitmap(pictureBox1.Width, pictureBox1.Height, PixelFormat.Format8bppIndexed);
            bmp_t = new Bitmap(pictureBox1.Width, pictureBox1.Height, PixelFormat.Format8bppIndexed);
            
            GrayScalePalette = bmp.Palette;

            //------------------------------------------------------
            //  Create grayscale bitmap palette
            //------------------------------------------------------
            for (int i = 0; i < 256; i++)
            {
                GrayScalePalette.Entries[i] = System.Drawing.Color.FromArgb(i, i, i);
            }

            m_LineListOriginal = new List<byte[]>();
            m_LineList2 = new List<byte[]>();

            lf = new LoadFile();
            _instance = this;
            offset = 0;
            numberoflines = 2*1290;
            img_height = pictureBox1.Height;
            img_width = pictureBox1.Width;
        }

        public static Form1 _GetInstance()
        {
            if (_instance != null) return _instance;
            return _instance = new Form1();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
        
        private void M_animageImage_UpdateImageDlgt()
        {
            BeginInvoke(m_UpdateImageDlgt);
        }
        private void btn_LoadBinFile(object sender, EventArgs e)
        {
            LoadBinFile(sender, e);
        }
        private void btn_DisplayFrame(object sender, EventArgs e)
        {
            DisplayFrame(offset);
        }
        private void btn_PreviousFrame(object sender, EventArgs e)
        {
            offset = offset - numberoflines;
            DisplayFrame(offset);
        }
        private void btn_NextFrame_Click(object sender, EventArgs e)
        {
            offset = offset + numberoflines;
            DisplayFrame(offset);
        }
        private void LoadBinFile(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.DefaultExt = ".bin";
            dlg.Filter = "Binary|*.bin";
            dlg.FilterIndex = 0;
            dlg.Title = "Load Binary Image File";
            dlg.Multiselect = false;

            string szFileName = string.Empty;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                szFileName = dlg.FileName;
                this.FileName = szFileName;

                string[] saVals = szFileName.Split(new char[] { ' ', '_', '.', '\\' }, StringSplitOptions.RemoveEmptyEntries);

                int iPos = 0;
                int iYearPos = 0;
                int iYear = 0;

                for (int i = 0; i < saVals.Length; i++)
                {
                    if (saVals[i].Length == 4 &&
                        int.TryParse(saVals[i], out iYear) == true)
                    {
                        iYearPos = i;
                    }
                    if (saVals[i] == "=")
                    {
                        iPos = i + 1;
                        break;
                    }
                }
                try
                {
                    iYear = int.Parse(saVals[iYearPos]);
                    int iMonth = int.Parse(saVals[iYearPos + 1]);
                    int iDay = int.Parse(saVals[iYearPos + 2]);
                    int iHour = int.Parse(saVals[iPos]);
                    int iMinute = int.Parse(saVals[iPos + 1]);
                    int iSecond = int.Parse(saVals[iPos + 2]);
                    int iMillisecond = int.Parse(saVals[iPos + 3]);

                    CameraStartTime = new DateTime(iYear, iMonth, iDay, iHour, iMinute, iSecond, iMillisecond);
                }
                catch
                {
                }
                BmpHeight = 2048;
                var thread = new Thread(() => lf.Process(FileName, BmpHeight));
                thread.Start();
            }
        }

        private void UpdateImage()
        {
            pictureBox1.Image.Dispose();
        }

        void DisplayFrame(long offset)
        {
            lf = lf._GetInstance();
            int linescan_resolution = 2048;
            
            MemoryStream ms = new MemoryStream();

            using (var stream = lf.mmf.CreateViewStream((long)(offset * (long)linescan_resolution), (long)(numberoflines * linescan_resolution * sizeof(int))))
            {
                BinaryReader binReader = new BinaryReader(stream);
                byte[] pData = binReader.ReadBytes((int)numberoflines * linescan_resolution * sizeof(int));
                ms.Write(pData, 0, pData.Length);

                Bitmap bmp = new Bitmap(linescan_resolution, numberoflines, PixelFormat.Format24bppRgb);
                
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,bmp.PixelFormat);
                IntPtr ptr = bmpData.Scan0;
                int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
                byte[] grayscaleValues = new byte[bytes];

                // Copy the RGB values into the array.
                System.Runtime.InteropServices.Marshal.Copy(ptr, grayscaleValues, 0, bytes);

                for (int counter = 0; counter < grayscaleValues.Length; counter++)
                    grayscaleValues[counter] = pData[counter];

                //for (int counter = linescan_resolution; counter > 0; counter -= 1)
                //grayscaleValues[counter] = pData[counter];

                System.Runtime.InteropServices.Marshal.Copy(grayscaleValues, 0, ptr, bytes);

                // Unlock the bits.
                bmp.UnlockBits(bmpData);

                // Draw the modified image.
                //e.Graphics.DrawImage(bmp_t, 0, 150);

                //int i = 0;
                //int i_t = numberoflines * linescan_resolution;

                ////for (int y = 0; y < numberoflines; y++)
                //Parallel.For(0, numberoflines, y =>
                //{
                //    //for (int x = 0; x < linescan_resolution; x++)
                //    Parallel.For(0, linescan_resolution, x =>
                //    {
                //        //ColorPalette pixel_tmp;
                //        System.Drawing.Color pixel_tmp;
                //        pixel_tmp = GrayScalePalette.Entries[pData[i_t]];
                //        bmp_t.SetPixel(Math.Abs(y - numberoflines + 1), Math.Abs(x - linescan_resolution + 1), pixel_tmp);
                //        //bmp.SetPixel(x, y, GrayScalePalette.Entries[pData[i]]);
                //        //i++;
                //        i_t--;
                //    });
                //});

                //using (bmp)
                //{
                //    bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                //}

                int sf = Int32.Parse(textBox1.Text);
                //pictureBox1.Width = img_width*sf;
                //pictureBox1.Height = img_height*sf;
                pictureBox1.Image = resizeImage(bmp, img_width*sf, img_height*sf);
                pictureBox1.Update();
                //bmp.Save("Test.jpg", ImageFormat.Jpeg);
                //bmp_t.Save("Test_t.jpg", ImageFormat.Jpeg);
            }
        }

        Image resizeImage(Image imgPhoto, double Width, double Height)
        {
            if (checkBox1.Checked)
            {
                imgPhoto.RotateFlip(RotateFlipType.Rotate90FlipX);
            }
            else
            {
                imgPhoto.RotateFlip(RotateFlipType.Rotate90FlipNone);
            }
            
            int sourceWidth  = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height/3;  // Divide by 3 becuase grayscale image but underlying data structure is RGB
            int sourceX      = 0;
            int sourceY      = 0;
            int destX        = 0;
            int destY        = 0;

            float nPercent  = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((Width -
                              (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((Height -
                              (sourceHeight * nPercent)) / 2)*0;
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(Convert.ToInt32(destWidth), Convert.ToInt32(destHeight),
                              PixelFormat.Format24bppRgb);

            //bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
            //                 imgPhoto.VerticalResolution);
            //bmPhoto.SetResolution(destWidth, destHeight);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
           // grPhoto.Clear(System.Drawing.Color.Red);
            grPhoto.InterpolationMode =
                    InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);
            grPhoto.Dispose();
            return bmPhoto;
        }

        [DllImport("ImageProcessingLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int subtract(int a, int b);
        [DllImport("ImageProcessingLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ProcessImageEntry(byte[] input_image, long datalen);
        private void btn_ProcessFrame_Click(object sender, EventArgs e)
        {
            //int x = Convert.ToInt32("5");
            //int y = Convert.ToInt32("10");
            //int z = subtract(x, y);
            Bitmap bmp = (Bitmap)pictureBox1.Image;
            using (MemoryStream sourceImageStream = new MemoryStream())
            {
                pictureBox1.Image.Save(sourceImageStream, System.Drawing.Imaging.ImageFormat.Png);
                byte[] sourceImageData = sourceImageStream.ToArray();
                ProcessImageEntry(sourceImageData, sourceImageStream.Length);
            }

            
            //var thread = new Thread(() => ProcessImage());
            //thread.Start();
        }
    }
}