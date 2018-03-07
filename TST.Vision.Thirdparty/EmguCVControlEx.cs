using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

namespace TST.Vision.Thirdparty
{
    public enum ENUM_EmguCVControlEx_Mode
    {
        Display,
        ImageMove,
        ImageZoom
    }

    struct cvShowString
    {
        public readonly string message;
        public readonly int leftcornerX;
        public readonly int leftcornerY;
        public readonly string color;
        public cvShowString(int x, int y, string message, string color)
        {
            this.message = message;
            this.leftcornerX = x;
            this.leftcornerY = y;
            this.color = color;
        }
    }

    public partial class EmguCVControlEx : UserControl, TST.Base.DisplayWindowControl
    {
        Mat m_cvImage = null;
        List<Mat> m_cvObjectList = new List<Mat>();
        List<cvShowString> m_showstingList = new List<cvShowString>();
        PictureBox picBox = new PictureBox();
        Graphics graphics = null;
        float startX = 0;
        float startY = 0;
        //private double m_CurrentZoomRate = 0;
        public EmguCVControlEx()
        {
            InitializeComponent();
            SetWokingMode(ENUM_EmguCVControlEx_Mode.Display);
            ///
            /// PictureBox
            ///
            this.picBox.BackColor = System.Drawing.Color.Black;
            this.picBox.Location = new System.Drawing.Point(41, 65);
            this.picBox.Name = "cvPictureBox";
            this.picBox.Size = new System.Drawing.Size(437, 477);
            this.picBox.TabIndex = 0;
            this.picBox.Location = this.Location;
            this.Controls.Add(this.picBox);

            //graphics = this.CreateGraphics();
            graphics = this.picBox.CreateGraphics();
        }

        public void DislpayObj(object obj)
        {
            if (obj == null)
                return;
            if (obj.GetType().Equals(typeof(Mat)))
            {
                m_cvObjectList.Clear();
                m_showstingList.Clear();
                m_cvImage = obj as Mat;
                ResetDisplayImag(m_cvImage);
            }
            else
            {
                Mat cvObj = obj as Mat;
                m_cvObjectList.Add(cvObj);
                DisPlayObject(cvObj);
            }
        }

        private void DisPlayObject(object obj)
        {
            if (obj != null && obj is Mat)
            {
            }
        }

        private void ResetDisplayImag(Mat TempCvImage)
        {
            if (TempCvImage == null)
            {
                return;
            }
            Image<Bgr, Byte> img = TempCvImage.ToImage<Bgr, Byte>();
            /*
            double TempZoomW = (this.Width * 1.0) / img.Size.Width;
            double TempZoomH = (this.Height * 1.0) / img.Size.Height;
            m_CurrentZoomRate = Math.Min(TempZoomH, TempZoomW);
            m_CurrentZoomRate = Math.Min(m_CurrentZoomRate, 1.0);
            int displayWith = (int)Math.Round(m_CurrentZoomRate * img.Size.Width);
            int displayHeight = (int)Math.Round(m_CurrentZoomRate * img.Size.Height);
            picBox.Image = img.Resize(displayWith, displayHeight, Inter.Nearest).ToBitmap();
             * */
            picBox.SizeMode = PictureBoxSizeMode.Zoom;
            picBox.Image = img.ToBitmap();
        }

        public void ShowMessage(int x, int y, string message, System.Drawing.Color color)
        {
        }

        private void ZoomImage(double x, double y, bool bZoomIn)
        {
        }

        private void MoveImage(double xDelta, double yDelta)
        {
        }

        public object DrawCircleROI()
        {
            return null;
        }

        private void ROIMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Console.WriteLine("ROIMouseDown X = " + e.X + "Y = " + e.Y);
            this.startX = e.X;
            this.startY = e.Y;
        }

        private void ROIMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Console.WriteLine("ROIMouseMove X = " + e.X + "Y = " + e.Y);
        }

        private void ROIMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Console.WriteLine("ROIMouseUp X = " + e.X + "Y = " + e.Y);
            Point mouseDownLocation = new Point(e.X, e.Y);
            float w = e.X - this.startX;
            float h = e.Y - this.startY;
            this.graphics.DrawRectangle(new Pen(new SolidBrush(Color.Red)), this.startX, this.startY, w, h);
            this.startX = 0;
            this.startY = 0;
        }

        public object DrawRectangleROI()
        {
            Console.WriteLine("DrawRectangleROI");
            if (m_cvImage == null)
            {
                return null;
            }
            this.picBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ROIMouseDown);
            this.picBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ROIMouseMove);
            this.picBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ROIMouseUp);
            SetWokingMode(ENUM_EmguCVControlEx_Mode.Display);
            return new Rectangle(0, 0, 1, 1);
        }

        public object DrawRotateRectangleROI()
        {
            return null;
        }

        public void SetWokingMode(ENUM_EmguCVControlEx_Mode changeMode)
        {
        }

        public void SetDisPlayMode()
        {
            SetWokingMode(ENUM_EmguCVControlEx_Mode.Display);
        }

        public void SetZoomMode()
        {
            SetWokingMode(ENUM_EmguCVControlEx_Mode.ImageZoom);
        }

        public void SetMoveMode()
        {
            SetWokingMode(ENUM_EmguCVControlEx_Mode.ImageMove);
        }
    }
}
