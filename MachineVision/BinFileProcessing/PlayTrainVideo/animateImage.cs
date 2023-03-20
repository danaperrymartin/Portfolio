using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlayTrainVideo
{
    public class animateImage : Form1
    {
        //Create a Bitmpap Object.
        //Bitmap animatedImage = new Bitmap("SampleAnimation.gif");
        bool currentlyAnimating = false;

        public delegate void UpdateImageEventHandler(Image image);
        public event UpdateImageEventHandler UpdateImageEvent;

        //This method begins the animation.
        public static void AnimateImage(Bitmap bmp)
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

        //private void OnFrameChanged(object o, EventArgs e)
        //{
        //    //Force a call to the Paint event handler.
        //    this.Invalidate();
        //}

        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    //Begin the animation.
        //    //AnimateImage();

        //    //Get the next frame ready for rendering.
        //    ImageAnimator.UpdateFrames();

        //    //Draw the next frame in the animation.
        //    e.Graphics.DrawImage(this.image, new Point(0, 0));
        //}

        private void UpdateImage(Bitmap bmp)
        {
            //if (UpdateImageEvent != null)
            //{
                RaiseUpdateImageEvent(bmp);
            //}
        }

        private void RaiseUpdateImageEvent(Bitmap bmp)
        {
            if (UpdateImageEvent != null)
            {
                UpdateImageEvent(bmp);
            }
        }

        //public static void Main()
        //{
        //    Application.Run(new animateImage());
        //}
    }
}
