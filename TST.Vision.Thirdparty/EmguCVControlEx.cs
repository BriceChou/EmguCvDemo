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
        None,
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
        Image<Bgr, Byte> image = null;
        List<Mat> m_cvObjectList = new List<Mat>();
        List<cvShowString> m_showstingList = new List<cvShowString>();
        Graphics graphics = null;
        float startX = 0;
        float startY = 0;
        private ENUM_EmguCVControlEx_Mode m_CurrentMode = ENUM_EmguCVControlEx_Mode.None;
        private const double ZoomScale = 0.1;
        private double m_CurrentZoomRate = 0;
        Rectangle ImagePart = new Rectangle(0, 0, 0, 0);
        public EmguCVControlEx(int w, int h)
        {
            InitializeComponent(w, h);
            SetWokingMode(ENUM_EmguCVControlEx_Mode.None);
            graphics = this.CreateGraphics();
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
            this.image = img;
            this.ImagePart.Width = 0;
            this.ImagePart.Height = 0;
            this.ImagePart.X = 0;
            this.ImagePart.Y = 0;
            if (img.Size.Width < this.Width && img.Size.Height < this.Height)
            {
                // If img small then display screen, not scale just display in center
                this.ImagePart.Width = img.Size.Width;
                this.ImagePart.Height = img.Size.Height;
                this.ImagePart.X += (int)((this.Width - this.ImagePart.Width) / 2);
                this.ImagePart.Y += (int)((this.Height - this.ImagePart.Height) / 2);
                m_CurrentZoomRate = 1.0;
            }
            else
            {
                double TempZoomW = (this.Width * 1.0) / img.Size.Width;
                double TempZoomH = (this.Height * 1.0) / img.Size.Height;
                m_CurrentZoomRate = Math.Min(TempZoomH, TempZoomW);
                this.ImagePart.Width = (int)Math.Round(m_CurrentZoomRate * img.Size.Width);
                this.ImagePart.Height = (int)Math.Round(m_CurrentZoomRate * img.Size.Height);
                if (this.ImagePart.Width < this.Width)
                    this.ImagePart.X += (int)((this.Width - this.ImagePart.Width) / 2);
                if (this.ImagePart.Height < this.Height)
                    this.ImagePart.Y += (int)((this.Height - this.ImagePart.Height) / 2);
            }

            graphics.DrawImage(img.ToBitmap(), this.ImagePart.X, this.ImagePart.Y, this.ImagePart.Width, this.ImagePart.Height);
        }

        public void ShowMessage(int x, int y, string message, System.Drawing.Color color)
        {
        }

        private void ZoomImage(double x, double y, bool bZoomIn)
        {
            Rectangle CurrentRectangle = ImagePart;
            m_CurrentZoomRate = 1 / m_CurrentZoomRate;
            double TempZoomScale = ZoomScale, TempZoomRate = m_CurrentZoomRate;
            double lefterCornerXoffset = (x - CurrentRectangle.X) / (CurrentRectangle.Width * 1.0) * (this.Width * 1.0) * TempZoomScale;
            double lefterCornerYoffset = (y - CurrentRectangle.Y) / (CurrentRectangle.Height * 1.0) * (this.Height * 1.0) * TempZoomScale;
            if (bZoomIn)
            {
                // Zoom in
                TempZoomRate = m_CurrentZoomRate - ZoomScale;
                if (TempZoomRate < 1)
                {
                    TempZoomScale = m_CurrentZoomRate - 1;
                    m_CurrentZoomRate = 1;
                    lefterCornerXoffset = (x - CurrentRectangle.X) / (CurrentRectangle.Width * 1.0) * (this.Width * 1.0) * TempZoomScale;
                    lefterCornerYoffset = (y - CurrentRectangle.Y) / (CurrentRectangle.Height * 1.0) * (this.Height * 1.0) * TempZoomScale;
                }
                else
                {
                    m_CurrentZoomRate = TempZoomRate;
                }

                CurrentRectangle.X += (int)Math.Round(lefterCornerXoffset);
                CurrentRectangle.Y += (int)Math.Round(lefterCornerYoffset);
            }
            else
            {
                // Zoom out
                m_CurrentZoomRate = m_CurrentZoomRate + ZoomScale;
                CurrentRectangle.X -= (int)Math.Round(lefterCornerXoffset);
                CurrentRectangle.Y -= (int)Math.Round(lefterCornerYoffset);

                /*
                m_CurrentZoomRate -= ZoomScale;
                this.ImagePart.Width = (int)(image.Size.Width * m_CurrentZoomRate);
                this.ImagePart.Height = (int)(image.Size.Height * m_CurrentZoomRate);
                this.ImagePart.X += (int)(x * ZoomScale / TempZoomRate);
                this.ImagePart.Y += (int)(y * ZoomScale / TempZoomRate);
                 * */
            }
            CurrentRectangle.Width = (int)Math.Round(this.Width * m_CurrentZoomRate);
            CurrentRectangle.Height = (int)Math.Round(this.Height * m_CurrentZoomRate);
            ImagePart = CurrentRectangle;
            graphics.Clear(Color.Black);
            graphics.DrawImage(image.ToBitmap(), this.ImagePart.X, this.ImagePart.Y, this.ImagePart.Width, this.ImagePart.Height);
        }

        private void MoveImage(double xDelta, double yDelta)
        {
        }

        public object DrawCircleROI()
        {
            return null;
        }

        private void EmguCV_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (this.m_cvImage == null)
                return;
            
            switch (m_CurrentMode)
            {
                case ENUM_EmguCVControlEx_Mode.Display:
                    this.startX = e.X;
                    this.startY = e.Y;
                    break;
                case ENUM_EmguCVControlEx_Mode.ImageZoom:
                    bool bZoomIn = (MouseButtons.Left == e.Button);
                    ZoomImage(e.X, e.Y, bZoomIn);
                    break;
                case ENUM_EmguCVControlEx_Mode.ImageMove:
                    //m_bCanMove = (MouseButtons.Left == e.Button);
                    //m_StartPoint = new PointF((float)e.X, (float)e.Y);
                    break;
                default:
                    break;
            }
        }

        private void EmguCV_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
        }

        private void EmguCV_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (this.m_cvImage == null)
                return;

            switch (m_CurrentMode)
            {
                case ENUM_EmguCVControlEx_Mode.Display:
                    float w = e.X - this.startX;
                    float h = e.Y - this.startY;
                    this.graphics.DrawRectangle(new Pen(new SolidBrush(Color.Red)), this.startX, this.startY, w, h);
                    this.startX = 0;
                    this.startY = 0;
                    break;
                default:
                    break;
            }
        }

        public object DrawRectangleROI()
        {
            Console.WriteLine("DrawRectangleROI");
            if (m_cvImage == null)
            {
                return null;
            }
            SetDisPlayMode();
            return new Rectangle(0, 0, 1, 1);
        }

        public object DrawRotateRectangleROI()
        {
            return null;
        }

        public void SetWokingMode(ENUM_EmguCVControlEx_Mode changeMode)
        {
            m_CurrentMode = changeMode;
            if (m_CurrentMode == ENUM_EmguCVControlEx_Mode.None)
            {
                this.MouseDown -= this.EmguCV_MouseDown;
                this.MouseMove -= this.EmguCV_MouseMove;
                this.MouseUp -= this.EmguCV_MouseUp;
            }
            else
            {
                this.MouseDown += this.EmguCV_MouseDown;
                this.MouseMove += this.EmguCV_MouseMove;
                this.MouseUp += this.EmguCV_MouseUp;
            }
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
