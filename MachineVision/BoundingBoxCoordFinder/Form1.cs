using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;


namespace BoundingBoxCoordFinder
{
    public partial class Form1 : Form
    {
        protected bool validData;
        protected bool m_bMouseDown;
        protected Rectangle m_rect;
        protected Point m_point;
        protected List<PointF> PointList = new List<PointF>();
        public static string fname;
        public double img_factor;
        private int bBox_num;
        private int mask_num;
        private int class_num;
        public string classname;
        private string[,] text;

        string path;
        public Image image;
        public Image resizedimg;
        protected Thread getImageThread;

        double increment = 50; // Percentage.
        double factor = 1.0;
        int numscalings = 1;
        double totalscaling_width = 1.0;
        double totalscaling_height = 1.0;
        private int icount;
        public float vert_pixelperinch = 0002F;
        public float horz_pixelperinch = 0002F;
        int idisp = 0;
        private int btn_DisplayMaskPrevious_Counter = 0;
        private int btn_DisplayMaskNext_Counter = 0;
        public string[] imgfiles = null;

        public Image ActiveImage { get; private set; }

        public Form1()
        {
            InitializeComponent();
            m_rect = Rectangle.Empty;
            m_bMouseDown = false;
            fname = null;
            bBox_num = 0;
            mask_num = 0;
            class_num = 2;
            classname = null;
            text = null;
            Size originalsize = new Size((int)0,(int) 0);
            double factor = 1.0;
            txtBoxHorizontalDist.Visible = false;
            txtBoxVerticalDist.Visible = false;
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            string filename;
            validData = GetFilename(out filename, e);

            Image myImg = Image.FromFile(filename);
            int getWidth = myImg.Width;
            int getHeight = myImg.Height;

            ActiveImage = Image.FromFile(filename);
            pictureBox1.Width = myImg.Width;
            pictureBox1.Height = myImg.Height;

            fname = Path.GetFileNameWithoutExtension(filename);
            //StreamWriter s = new StreamWriter("C:/CodeProjects/MachineVision/BoundingBoxCoordFinder/" + fname+ "_" + txtboxClassName.Text + ".csv");
            //s.WriteLine("");
            //s.Close();
            if (validData)
            {
                path = filename;
                getImageThread = new Thread(new ThreadStart(LoadImage));
                getImageThread.Start();
                e.Effect = DragDropEffects.Copy;
            }
            else
                e.Effect = DragDropEffects.None;
        }

        private bool GetFilename(out string filename, DragEventArgs e)
        {
            bool ret = false;
            filename = String.Empty;
            if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
                Array data = ((IDataObject)e.Data).GetData("FileDrop") as Array;
                if (data != null)
                {
                    if ((data.Length == 1) && (data.GetValue(0) is String))
                    {
                        filename = ((string[])data)[0];
                        string ext = Path.GetExtension(filename).ToLower();
                        if ((ext == ".jpg") || (ext == ".png") || (ext == ".bmp"))
                        {
                            ret = true;
                        }
                    }
                }
            }
            return ret;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (validData)
            {
                while (getImageThread.IsAlive)
                {
                    Application.DoEvents();
                    Thread.Sleep(0);
                }
                pictureBox1.Image = image;
            }
        }

        protected void LoadImage()
        {
            if (image != null)
            {
                image.Dispose();
                image = null;
            }
            image = new Bitmap(path);
        }

        protected void LoadImageandDisplay()
        {
            if (image != null)
            {
                image.Dispose();
                image = null;
            }

            image = new Bitmap(path);
            int getWidth = image.Width;
            int getHeight = image.Height;

            //pictureBox1.Width = getWidth;
            //pictureBox1.Height = getHeight;

            ActiveImage = Image.FromFile(path);
            pictureBox1.Image = image;
            
            //while (getImageThread.IsAlive)
            //{
            //    Application.DoEvents();
            //    Thread.Sleep(0);
            //}
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if(txtboxClassName.Text!=classname)
            {
                class_num++;
            }
            if (!radioButton3.Checked && radioButton1.Checked)
            {
                txtBoxHorizontalDist.Visible = false;
                txtBoxVerticalDist.Visible = false;
                checkBox1.Enabled = true;
                checkBox2.Enabled = true;
                if (checkBox1.Checked)
                {
                    m_bMouseDown = true;
                    m_rect.X = e.X;
                    m_rect.Y = e.Y;

                    string text = (((int)((double)e.X / totalscaling_width)).ToString() + " " + ((int)((double)e.Y / totalscaling_height))).ToString();

                    StreamWriter s = new StreamWriter("C:/CodeProjects/MachineVision/BoundingBoxCoordFinder/" + fname + "_" + txtboxClassName.Text + ".csv", append: true);
                    s.Write(text);
                    s.Close();
                }
                else if (checkBox2.Checked)
                {
                    m_bMouseDown = true;
                    m_point.X = e.X;
                    m_point.Y = e.Y;
                    PointList.Add(m_point);
                }
            }
            else if (!radioButton3.Checked && radioButton2.Checked)
            {
                txtBoxHorizontalDist.Visible = false;
                txtBoxVerticalDist.Visible = false;
                checkBox1.Enabled = true;
                checkBox2.Enabled = true;
                if (checkBox1.Checked)
                {
                    m_bMouseDown = true;
                    m_rect.X = e.X;
                    m_rect.Y = e.Y;

                    //string text = (((int)((double)e.X / totalscaling_width)).ToString() + " " + ((int)((double)e.Y / totalscaling_height))).ToString();

                    //StreamWriter s = new StreamWriter("C:/CodeProjects/MachineVision/BoundingBoxCoordFinder/" + fname + "_" + txtboxClassName.Text + ".csv", append: true);
                    //s.Write(text);
                    //s.Close();
                }
                else if (checkBox2.Checked)
                {
                    m_bMouseDown = true;
                    m_point.X = e.X;
                    m_point.Y = e.Y;
                    PointList.Add(m_point);
                }
            }
            //else
            //{
            //    txtBoxHorizontalDist.Visible = true;
            //    txtBoxVerticalDist.Visible = true;
            //    checkBox1.Checked = true;
            //    checkBox1.Enabled = false;
            //    checkBox2.Checked = false;
            //    checkBox2.Enabled = false;
            //    m_bMouseDown = true;
            //    m_rect.X = e.X;
            //    m_rect.Y = e.Y;

            //    string text = (((int)((double)e.X / totalscaling_width)).ToString() + " " + ((int)((double)e.Y / totalscaling_height))).ToString();

            //    StreamWriter s = new StreamWriter("C:/CodeProjects/MachineVision/BoundingBoxCoordFinder/CalibrationFactors" + fname + "_" + txtboxClassName.Text + ".csv", append: true);
            //    s.Write(text);
            //    s.Close();
            //}
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            classname = txtboxClassName.Text;
            if (radioButton1.Checked)
            {
                if (checkBox1.Checked)
                {
                    if (m_bMouseDown == true)
                    {
                        bBox_num++;
                        m_rect.Width = Math.Abs(m_rect.X - e.X);
                        m_rect.Height = Math.Abs(m_rect.Y - e.Y);
                        //if (img_factor > 0)
                        //{
                        string text = " " + ((int)((double)e.X / totalscaling_width)).ToString() + " " + ((int)((double)e.Y / totalscaling_height)).ToString();
                        //string text = " " + (2 * (int)((double)e.X / factor)).ToString() + " " + (2 * (int)((double)e.Y / factor)).ToString();

                        StreamWriter s = new StreamWriter("C:/CodeProjects/MachineVision/BoundingBoxCoordFinder/" + fname + "_" + txtboxClassName.Text + ".csv", append: true);
                        s.WriteLine(text);
                        s.Flush();
                        s.Close();
                        m_bMouseDown = false;

                        string[] arrLine = File.ReadAllLines("C:/CodeProjects/MachineVision/BoundingBoxCoordFinder/" + fname + "_" + txtboxClassName.Text + ".csv");
                        arrLine[0] = bBox_num.ToString();
                        File.WriteAllLines("C:/CodeProjects/MachineVision/BoundingBoxCoordFinder/" + fname + "_" + txtboxClassName.Text + ".csv", arrLine);
                    }
                }
                else if (checkBox2.Checked)
                {
                    if (m_bMouseDown == true)
                    {
                        bBox_num++;
                        m_point.X = e.X;
                        m_point.Y = e.Y;
                        PointList.Add(m_point);
                        List<double> dblPointList_x = new List<double>();
                        List<double> dblPointList_y = new List<double>();

                        StreamWriter s = new StreamWriter("C:/CodeProjects/MachineVision/BoundingBoxCoordFinder/" + fname + "_" + txtboxClassName.Text + ".csv", append: true);
                        s.WriteLine("\n");
                        for (int i = 0; i < PointList.Count; i++)
                        {
                            string text = " " + "(" + (Convert.ToDouble(PointList[i].X) / totalscaling_width).ToString() + "," + (Convert.ToDouble(PointList[i].Y) / totalscaling_height).ToString() + ")    ";
                            s.WriteLine(text);
                        }

                        s.Flush();
                        s.Close();
                        m_bMouseDown = false;
                        string[] arrLine = File.ReadAllLines("C:/CodeProjects/MachineVision/BoundingBoxCoordFinder/" + fname + "_" + txtboxClassName.Text + ".csv");
                        arrLine[0] = bBox_num.ToString();
                        File.WriteAllLines("C:/CodeProjects/MachineVision/BoundingBoxCoordFinder/" + fname + "_" + txtboxClassName.Text + ".csv", arrLine);
                    }
                }
            }
            else if (radioButton2.Checked)
            {
                if (checkBox1.Checked)
                {
                    PointList.Clear();
                    m_rect.Width = Math.Abs(m_rect.X - e.X);
                    m_rect.Height = Math.Abs(m_rect.Y - e.Y);
                    m_point.X = m_rect.X;
                    m_point.Y = m_rect.Y;
                    PointList.Add(m_point);

                    m_point.X = m_rect.X + m_rect.Width;
                    m_point.Y = m_rect.Y + m_rect.Height;
                    PointList.Add(m_point);
                    //for (int iwidth = 1; iwidth < m_rect.Width; iwidth++)
                    //{
                    //    for (int iheight = 1; iheight < m_rect.Height; iheight++)
                    //    {
                    //        m_point.X = e.X+iwidth;
                    //        m_point.Y = e.Y+iheight;
                    //        PointList.Add(m_point);
                    //    }
                    //}
                }
                if (m_bMouseDown == true)
                {
                    mask_num++;
                    m_point.X = e.X;
                    m_point.Y = e.Y;
                    PointList.Add(m_point);
                    m_bMouseDown = false;
                    if (File.Exists("C:/CodeProjects/MachineVision/BoundingBoxCoordFinder/" + fname + "_" + txtboxClassName.Text + ".csv"))
                    {
                        string text_tmp = File.ReadAllText("C:/CodeProjects/MachineVision/BoundingBoxCoordFinder/" + fname + "_" + txtboxClassName.Text + ".csv");
                        int i = 0, j = 0;
                        string[,] currentmask = new string[pictureBox1.Height, pictureBox1.Width];

                        foreach (var row in text_tmp.Split('\n'))
                        {
                            j = 0;
                            foreach (var col in row.Trim())
                            {
                                text[i, j] = row[j].ToString();
                                j++;
                            }
                            i++;
                        }
                    }
                    else
                    {
                        text = new string[pictureBox1.Height, pictureBox1.Width];
                    }
                    int[] intPointList_x = new int[PointList.Count];
                    int[] intPointList_y = new int[PointList.Count];

                    //if (mask_num == 1)
                    //{
                    //    s.WriteLine(mask_num.ToString() + "\n");
                    //}

                    icount = 0;
                    while (icount < PointList.Count)
                    {
                        intPointList_x[icount] = Convert.ToInt32(PointList[icount].X);
                        intPointList_y[icount] = Convert.ToInt32(PointList[icount].Y);
                        icount++;
                    }

                    StreamWriter s = new StreamWriter("C:/CodeProjects/MachineVision/BoundingBoxCoordFinder/" + fname + "_" + txtboxClassName.Text + ".csv", append: false);

                    for (int i = 0; i < pictureBox1.Height; i++)
                    {
                        if (i != 0)
                        {
                            s.Write("\n");
                        }
                        for (int j = 0; j < pictureBox1.Width; j++)
                        {
                            if (checkBox1.Checked)
                            {
                                if (((intPointList_y.Min() < i) && (i < intPointList_y.Max())) && ((intPointList_x.Min() < j) && (j < intPointList_x.Max())))
                                {
                                    text[i, j] = "1";// class_num.ToString();
                                    s.Write(text[i, j]);
                                }
                                else if (text[i,j]!="0" && text[i, j]!= null)
                                {
                                    text[i, j] = text[i,j];
                                    s.Write(text[i, j]);
                                }
                                else
                                {
                                    text[i, j] = "0";
                                    s.Write(text[i, j]);
                                }
                            }
                            else if (checkBox2.Checked)
                            {
                                if (intPointList_x.Any(x => x == i) &&  intPointList_y.Any(y => y == j))
                                {
                                    text[i, j] = "1"; class_num.ToString();
                                    s.Write(text[i, j]);
                                }
                                else
                                {
                                    text[i, j] = "0";
                                    s.Write(text[i, j]);
                                }
                            }
                        }
                    }

                    //for (int i = 0; i < pictureBox1.Height; i++)
                    //{
                    //    s.Write(text);
                    //}

                    //s.WriteLine("\n---------------------EOF-------------------------------");
                    s.Flush();
                    s.Close();
                    m_bMouseDown = false;
                    //string[] arrLine = File.ReadAllLines("C:/CodeProjects/MachineVision/BoundingBoxCoordFinder/" + fname + "_" + txtboxClassName.Text + "_" + mask_num.ToString() + ".csv");
                    //arrLine[0] = mask_num.ToString();
                    //File.WriteAllLines("C:/CodeProjects/MachineVision/BoundingBoxCoordFinder/" + fname + "_" + txtboxClassName.Text + "_" + mask_num.ToString() + ".csv", arrLine);
                }
            }
            else if (radioButton3.Checked && radioButton3.Text != "Measure")
            {
                txtBoxHorizontalDist.Enabled = true;
                txtBoxVerticalDist.Enabled = true;
                m_bMouseDown = false;
                m_rect.Width = Math.Abs(m_rect.X - e.X);
                m_rect.Height = Math.Abs(m_rect.Y - e.Y);

                vert_pixelperinch = (float)(Convert.ToDouble(m_rect.Height) / Convert.ToDouble(txtBoxVerticalDist.Text));// / ((float)totalscaling_height);
                horz_pixelperinch = (float)(Convert.ToDouble(m_rect.Width) / Convert.ToDouble(txtBoxHorizontalDist.Text));// / ((float)totalscaling_width);

                txtBoxVerticalDist.Text = vert_pixelperinch.ToString() + " vert pxl/in";
                txtBoxHorizontalDist.Text = horz_pixelperinch.ToString() + " horz pxl/in";
                txtBoxHorizontalDist.Enabled = false;
                txtBoxVerticalDist.Enabled = false;

                radioButton3.Text = "Measure";
                radioButton3.Enabled = false;
            }
            else if (radioButton3.Checked && radioButton3.Text == "Measure")
            {
                m_bMouseDown = false;
                m_rect.Width = Math.Abs(m_rect.X - e.X);
                m_rect.Height = Math.Abs(m_rect.Y - e.Y);

                txtBoxVerticalDist.Text = ((float)m_rect.Height/vert_pixelperinch).ToString() + " inches tall";
                txtBoxHorizontalDist.Text = ((float)m_rect.Width/horz_pixelperinch).ToString() + " inches long";
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (checkBox1.Checked)
            {
                if (m_bMouseDown == true)
                {
                    m_rect.Width = Math.Abs(e.X - m_rect.X);
                    m_rect.Height = Math.Abs(e.Y - m_rect.Y);
                    pictureBox1.Invalidate(true);
                }
            }
            else if (checkBox2.Checked)
            {
                if (m_bMouseDown == true)
                {
                    m_point.X = e.X;
                    m_point.Y = e.Y;
                    PointList.Add(m_point);
                    pictureBox1.Invalidate(true);
                }
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Pen p_green = new Pen(Color.Green, 1.0f);
            Graphics G = e.Graphics;
            if (checkBox1.Checked)
            {
                if (m_rect != Rectangle.Empty)
                {
                    G.DrawRectangle(p_green, m_rect);
                }
            }
            if(checkBox2.Checked)
            {
                if (PointList != null && PointList.Count!=0)
                {
                    G.DrawPolygon(p_green, PointList.ToArray());
                }
            }
        }

        //private double pictureBox1_MouseHover(object sender, MouseEventArgs e)
        //{
        //    double factor = 1.0;
        //    factor -= (increment / 100.0);
        //    pictureBox1.Image = resizeImage(pictureBox1.Image, pictureBox1.Image.Width * factor, pictureBox1.Image.Height * factor);

        //    pictureBox1.Focus();
        //    img_factor = factor;
        //    return img_factor;
        //}

         Image resizeImage(Image imgPhoto, double Width, double Height)
         {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
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
                              (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(Convert.ToInt32(Width), Convert.ToInt32(Height),
                              PixelFormat.Format24bppRgb);

            //bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
            //                 imgPhoto.VerticalResolution);
            bmPhoto.SetResolution(pictureBox1.Width, pictureBox1.Height);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.Red);
            grPhoto.InterpolationMode =
                    InterpolationMode.HighQualityBicubic;
            
            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);
            grPhoto.Dispose();
            pictureBox1.Size = new Size(destWidth, destHeight);
            return bmPhoto;
         }
         
        Image DisplayMask(Image imgPhoto, string[,] mask)
        {
            int sourceWidth = pictureBox1.Width;
            int sourceHeight = pictureBox1.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 1;
            //float nPercentW = 0;
            //float nPercentH = 0;

            //nPercentW = ((float)Width / (float)sourceWidth);
            //nPercentH = ((float)Height / (float)sourceHeight);
            //if (nPercentH < nPercentW)
            //{
            //    nPercent = nPercentH;
            //    destX = System.Convert.ToInt16((Width -
            //                  (sourceWidth * nPercent)) / 2);
            //}
            //else
            //{
            //    nPercent = nPercentW;
            //    destY = System.Convert.ToInt16((Height -
            //                  (sourceHeight * nPercent)) / 2);
            //}

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(Convert.ToInt32(pictureBox1.Width), Convert.ToInt32(pictureBox1.Height),
                              PixelFormat.Format24bppRgb);
            bmPhoto = (Bitmap)imgPhoto;
            bmPhoto.SetResolution(pictureBox1.Width, pictureBox1.Height);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            //grPhoto.Clear(Color.Red);
            grPhoto.InterpolationMode =
                    InterpolationMode.HighQualityBicubic;

            for (int ix=0; ix < bmPhoto.Width; ix++)
            {
                for (int iy = 0; iy < bmPhoto.Height; iy++)
                {
                    if (mask[iy,ix]=="0")
                    {
                        bmPhoto.SetPixel(ix,iy, Color.Black);
                    }
                    else if (mask[iy,ix] != "0")
                    {
                        bmPhoto.SetPixel(ix,iy, bmPhoto.GetPixel(ix,iy));
                    }
                }
            }

            grPhoto.DrawImage(bmPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);
            grPhoto.Dispose();
            pictureBox1.Size = new Size(destWidth, destHeight);
            return bmPhoto;
        }

        private void ClearBtn_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
            pictureBox1.Width = default;
            pictureBox1.Height = default;
            pictureBox1.Update();
            image.Dispose();
            ActiveImage.Dispose();
            bBox_num  = 0;
            mask_num  = 0;
            class_num = 2;
            txtboxClassName.Text = "Enter Class Name";
            factor = 1.0;
            m_rect = Rectangle.Empty;
            PointList = new List<PointF>();
            m_point = Point.Empty;
            m_bMouseDown = false;
            fname = null;
            numscalings = 1;
            radioButton3.Text = "Calibrate";
            radioButton3.Checked = false;
            radioButton3.Enabled = true;
            txtBoxHorizontalDist.Text = "Horizontal Distance (inches)";
            txtBoxVerticalDist.Text = "Vertical Distance (inches)";
        }

        void button2_Click(object sender, EventArgs e)
        {
            var frm = new Form();
            //pictureBox1.Focus();
            factor = increment / 100.0;
            double img_factor = factor;
            double new_width = pictureBox1.Width * img_factor;
            double new_height = pictureBox1.Height * img_factor;
            resizedimg = resizeImage(pictureBox1.Image, pictureBox1.Image.Width * img_factor, pictureBox1.Image.Height * img_factor);
            pictureBox1.Image = resizedimg;
            numscalings++;
            totalscaling_width = (double)pictureBox1.Size.Width / (double)image.Width;
            totalscaling_height = (double)pictureBox1.Size.Height / (double)image.Height;
            //pictureBox1.Focus();
        }

        private void btn3_Click(object sender, EventArgs e)
        {
            var frm = new Form();
            //pictureBox1.Focus();
            string currentmask_tmp = File.ReadAllText("C:/CodeProjects/MachineVision/BoundingBoxCoordFinder/" + fname + "_" + txtboxClassName.Text + "_" + mask_num.ToString() + ".csv");
            int i = 0, j=0;
            string[,] currentmask = new string[pictureBox1.Height,pictureBox1.Width];
            
            foreach (var row in currentmask_tmp.Split('\n'))
            {
                j = 0;
                foreach (var col in row.Trim())
                {
                    currentmask[i, j] = row[j].ToString();
                    j++;
                }
                i++;
            }
            pictureBox1.Image = DisplayMask(pictureBox1.Image, currentmask);
            //pictureBox1.Focus();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            txtBoxHorizontalDist.Visible = true;
            txtBoxVerticalDist.Visible = true;
            checkBox1.Checked = true;
            checkBox1.Enabled = false;
            checkBox2.Checked = false;
            checkBox2.Enabled = false;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            txtBoxHorizontalDist.Visible = false;
            txtBoxVerticalDist.Visible = false;
            checkBox1.Checked = false;
            checkBox1.Enabled = true;
            checkBox2.Checked = false;
            checkBox2.Enabled = true;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            txtBoxHorizontalDist.Visible = false;
            txtBoxVerticalDist.Visible = false;
            checkBox1.Checked = false;
            checkBox1.Enabled = true;
            checkBox2.Checked = false;
            checkBox2.Enabled = true;
        }

        private void btn_LoadImgfromDB_Click(object sender, EventArgs e)
        {
            List<TrainImageDB> ImageList = new List<TrainImageDB>();
            var formPopup = new DateTimeSearchForm(ImageList);
            formPopup.ShowDialog(this); // if you need non-modal window
            //formPopup.Activate();

            //getImageThread = new Thread(new ThreadStart(LoadImage));
            //getImageThread = new Thread(new ThreadStart(formPopup.Show));
            //getImageThread.Start();

            imgfiles = Directory.GetFiles("..\\..\\images\\");

            //for (int idisp=0; idisp < imgfiles.Length; idisp++)
            //{
                string filename = imgfiles[idisp];
                
                //Image myImg = Image.FromFile(filename);
                //int getWidth = myImg.Width;
                //int getHeight = myImg.Height;

                //ActiveImage = Image.FromFile(filename);
                //pictureBox1.Width = myImg.Width;
                //pictureBox1.Height = myImg.Height;

                //fname = Path.GetFileNameWithoutExtension(filename);
                //StreamWriter s = new StreamWriter("C:/CodeProjects/MachineVision/BoundingBoxCoordFinder/" + fname + "_" + txtboxClassName.Text + ".csv");
                //s.WriteLine("");
                //s.Close();

                path = filename;
                //getImageThread = new Thread(new ThreadStart(LoadImageandDisplay));
                //getImageThread.Start();
                this.LoadImageandDisplay();
            //}
        }

        private void btn_next_Click(object sender, EventArgs e)
        {
            this.button1.Click += new System.EventHandler(this.ClearBtn_Click);
            idisp++;
            string filename = imgfiles[idisp];
            path = filename;
            this.LoadImageandDisplay();
        }

        private void txtboxClassName_TextChanged(object sender, EventArgs e)
        {
            mask_num = 0;
        }

        private void btn_DisplayMaskNext_Click(object sender, EventArgs e)
        {
            btn_DisplayMaskNext_Counter++;
            pictureBox1.Image = resizeImage(image, pictureBox1.Width, pictureBox1.Height);
            int nextmask = (mask_num - btn_DisplayMaskPrevious_Counter) + btn_DisplayMaskNext_Counter;
            btn_DisplayMaskPrevious_Counter = 0;
            if (nextmask <= mask_num)
            {
                var frm = new Form();
                //pictureBox1.Focus();
                string currentmask_tmp = File.ReadAllText("C:/CodeProjects/MachineVision/BoundingBoxCoordFinder/" + fname + "_" + txtboxClassName.Text + "_" + "mask" + nextmask.ToString() + ".csv");
                int i = 0, j = 0;
                string[,] currentmask = new string[resizedimg.Height, resizedimg.Width];
                foreach (var row in currentmask_tmp.Split('\n'))
                {
                    j = 0;
                    foreach (var col in row.Trim())
                    {
                        currentmask[i, j] = row[j].ToString();
                        j++;
                    }
                    i++;
                }

                pictureBox1.Image = DisplayMask(pictureBox1.Image, currentmask);
            }
            else
            {
                pictureBox1.Image = resizeImage(image, pictureBox1.Width, pictureBox1.Height);
                string currentmask_tmp = File.ReadAllText("C:/CodeProjects/MachineVision/BoundingBoxCoordFinder/" + fname + "_" + txtboxClassName.Text + "_" + mask_num.ToString() + ".csv");
                int i = 0, j = 0;
                string[,] currentmask = new string[image.Height, image.Width];
                foreach (var row in currentmask_tmp.Split('\n'))
                {
                    j = 0;
                    foreach (var col in row.Trim())
                    {
                        currentmask[i, j] = row[j].ToString();
                        j++;
                    }
                    i++;
                }
                pictureBox1.Image = DisplayMask(resizedimg, currentmask);
            }
        }

        private void btn_DisplayMaskPrevious_Click(object sender, EventArgs e)
        {
            btn_DisplayMaskNext_Counter = 0;
            btn_DisplayMaskPrevious_Counter++;
            pictureBox1.Image = resizeImage(image, pictureBox1.Width, pictureBox1.Height);

            int previousmask = mask_num - btn_DisplayMaskPrevious_Counter;

            if (previousmask >= 1)
            {
                //pictureBox1.Focus();
                string currentmask_tmp = File.ReadAllText("C:/CodeProjects/MachineVision/BoundingBoxCoordFinder/" + fname + "_" + txtboxClassName.Text + "_" + "mask" + previousmask.ToString() + ".csv");
                int i = 0, j = 0;
                string[,] currentmask = new string[resizedimg.Height, resizedimg.Width];
                foreach (var row in currentmask_tmp.Split('\n'))
                {
                    j = 0;
                    foreach (var col in row.Trim())
                    {
                        currentmask[i, j] = row[j].ToString();
                        j++;
                    }
                    i++;
                }
                pictureBox1.Image = DisplayMask(pictureBox1.Image, currentmask);
            }
        }

        private void btn_ScaleMaskForOriginalImageSize_Click(object sender, EventArgs e)
        {
            int currentmasknum = mask_num + (btn_DisplayMaskNext_Counter - btn_DisplayMaskPrevious_Counter);

            string currentmask_tmp = File.ReadAllText("C:/CodeProjects/MachineVision/BoundingBoxCoordFinder/" + fname + "_" + txtboxClassName.Text + ".csv");
            int i = 0, j = 0;
            string[,] currentmask = new string[resizedimg.Height, resizedimg.Width];
            int[,] nearestneighbor = new int[image.Height, image.Width];

            foreach (var row in currentmask_tmp.Split('\n'))
            {
                j = 0;
                foreach (var col in row.Trim())
                {
                    currentmask[i, j] = row[j].ToString();
                    j++;
                }
                i++;
            }

            StreamWriter s = new StreamWriter("C:/CodeProjects/MachineVision/BoundingBoxCoordFinder/" + fname + "_" + txtboxClassName.Text + "_4Orig.csv", append: false);
            for (int irow=0; irow<image.Height; irow++)
            {
                if (irow!=0)
                {
                    s.Write("\n");
                }
                for (int jcol = 0; jcol < image.Width; jcol++)
                {
                    nearestneighbor[irow, jcol] = Convert.ToInt32(currentmask[(int)Math.Min(currentmask.GetLength(0)-1,Math.Round(irow * totalscaling_height)), (int)Math.Min(currentmask.GetLength(1)-1,Math.Round(jcol * totalscaling_width))]);
                    s.Write(nearestneighbor[irow, jcol]);
                }
            }
            s.Flush();
            s.Close();
        }
    }
}
