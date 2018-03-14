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
using Emgu.CV.UI;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;

namespace TST.Vision.Thirdparty
{
    public enum ENUM_EmguCVControlEx_Mode
    {
        None,
        Display,
        ImageMove,
        ImageZoom,
        IrregularROI,
        RectangleROI
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
        Bitmap bitmap = null;

        List<Mat> m_cvObjectList = new List<Mat>();
        List<cvShowString> m_showstingList = new List<cvShowString>();

        Graphics graphics = null;
        float startX = 0;
        float startY = 0;
        List<Point> curvePoints = new List<Point>();
        Boolean startDraw = false;
        private ENUM_EmguCVControlEx_Mode m_CurrentMode = ENUM_EmguCVControlEx_Mode.None;

        private const double ZoomScale = 0.1;
        private double m_CurrentZoomRate = 0;
        Rectangle ImagePart = new Rectangle(0, 0, 0, 0);
        ImageBox cvImgBox = new ImageBox();

        public EmguCVControlEx(int w, int h)
        {
            InitializeComponent(w, h);
            SetWokingMode(ENUM_EmguCVControlEx_Mode.None);
            graphics = this.CreateGraphics();
            this.Controls.Add(cvImgBox);
        }
        #region Display the picture operation
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
            this.bitmap = img.ToBitmap();
            Rectangle disRect = new Rectangle();

            if (img.Size.Width < this.Width && img.Size.Height < this.Height)
            {
                // If img small then display screen, not scale just display in center
                disRect.Width = img.Size.Width;
                disRect.Height = img.Size.Height;
                disRect.X += (int)((this.Width - disRect.Width) / 2);
                disRect.Y += (int)((this.Height - disRect.Height) / 2);
                m_CurrentZoomRate = 1.0;
            }
            else
            {
                double ZoomW = (this.Width * 1.0) / img.Size.Width;
                double ZoomH = (this.Height * 1.0) / img.Size.Height;
                double m_ZoomRate = Math.Min(ZoomW, ZoomH);
                disRect.Width = (int)Math.Round(m_ZoomRate * img.Size.Width);
                disRect.Height = (int)Math.Round(m_ZoomRate * img.Size.Height);
                if (disRect.Width < this.Width)
                    disRect.X += (int)((this.Width - disRect.Width) / 2);
                if (disRect.Height < this.Height)
                    disRect.Y += (int)((this.Height - disRect.Height) / 2);
            }

            double TempZoomW = m_cvImage.Width / (this.Width * 1.0);
            double TempZoomH = m_cvImage.Height / (this.Height * 1.0);
            m_CurrentZoomRate = Math.Max(TempZoomW, TempZoomH);
            m_CurrentZoomRate = Math.Max(m_CurrentZoomRate, 1.0);//显示象素小于窗体宽度时有异常
            this.ImagePart = disRect;

            graphics.Clear(Color.Black);
            graphics.DrawImage(this.bitmap, disRect.X, disRect.Y, disRect.Width, disRect.Height);
        }
        #endregion

        public void ShowMessage(int x, int y, string message, System.Drawing.Color color)
        {
            Point position = new Point(x, y);
            Font font = new Font("黑体", 16, GraphicsUnit.Pixel);
            SolidBrush fontLine = new SolidBrush(color);
            graphics.DrawString(message, font, fontLine, position);
        }

        private void ZoomImage(double x, double y, bool bZoomIn)
        {
            Rectangle CurrentRectangle = ImagePart;
            double TempZoomRate = m_CurrentZoomRate;
            if (bZoomIn)
            {
                TempZoomRate = m_CurrentZoomRate - ZoomScale;
                if (TempZoomRate < 1)
                {
                    m_CurrentZoomRate = 1;
                }
                else
                {
                    m_CurrentZoomRate = TempZoomRate;
                }

                //double offsetX = this.image.Width / m_CurrentZoomRate - CurrentRectangle.Width;
                //double offsetY = this.image.Height / m_CurrentZoomRate - CurrentRectangle.Height;
                //CurrentRectangle.X -= (int)(((x - CurrentRectangle.X) / CurrentRectangle.Width) * offsetX);
                //CurrentRectangle.Y -= (int)(((y - CurrentRectangle.Y) / CurrentRectangle.Height) * offsetY);
                double ratioX = this.image.Width / m_CurrentZoomRate / CurrentRectangle.Width;
                double ratioY = this.image.Height / m_CurrentZoomRate / CurrentRectangle.Height;
                CurrentRectangle.X = (int)(x - (x - CurrentRectangle.X) * ratioX);
                CurrentRectangle.Y = (int)(y - (y - CurrentRectangle.Y) * ratioY);
            }
            else
            {
                m_CurrentZoomRate = m_CurrentZoomRate + ZoomScale;
                //double offsetX = -(this.image.Width / m_CurrentZoomRate - CurrentRectangle.Width);
                //double offsetY = -(this.image.Height / m_CurrentZoomRate - CurrentRectangle.Height);
                //CurrentRectangle.X += (int)(((x - CurrentRectangle.X) / CurrentRectangle.Width) * offsetX);
                //CurrentRectangle.Y += (int)(((y - CurrentRectangle.Y) / CurrentRectangle.Height) * offsetY);
                double ratioX = this.image.Width / m_CurrentZoomRate / CurrentRectangle.Width;
                double ratioY = this.image.Height / m_CurrentZoomRate / CurrentRectangle.Height;
                CurrentRectangle.X = (int)(x - (x - CurrentRectangle.X) * ratioX);
                CurrentRectangle.Y = (int)(y - (y - CurrentRectangle.Y) * ratioY);
            }
            CurrentRectangle.Width = (int)(this.image.Width / m_CurrentZoomRate);
            CurrentRectangle.Height = (int)(this.image.Height / m_CurrentZoomRate);
            ImagePart = CurrentRectangle;
            graphics.Clear(Color.Black);
            graphics.DrawImage(this.bitmap, this.ImagePart.X, this.ImagePart.Y,
                this.ImagePart.Width, this.ImagePart.Height);
        }

        #region Move picture operation
        private void EmguCV_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Console.WriteLine("EmguCV_MouseDown");
            if (this.m_cvImage == null)
                return;
            this.startX = 0;
            this.startY = 0;

            switch (m_CurrentMode)
            {
                case ENUM_EmguCVControlEx_Mode.RectangleROI:
                    this.startX = e.X;
                    this.startY = e.Y;
                    this.startDraw = true;
                    break;
                case ENUM_EmguCVControlEx_Mode.IrregularROI:
                    this.startX = e.X;
                    this.startY = e.Y;
                    this.startDraw = true;
                    Point p = new Point(e.X, e.Y);
                    this.curvePoints.Add(p);
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
            Console.WriteLine("EmguCV_MouseMove");
            switch (m_CurrentMode)
            {
                case ENUM_EmguCVControlEx_Mode.IrregularROI:
                    if (this.startDraw)
                    {
                        Point p = new Point(e.X, e.Y);
                        this.curvePoints.Add(p);
                    }
                    break;
                default:
                    break;
            }
        }

        private void EmguCV_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Console.WriteLine("EmguCV_MouseUp");
            if (this.m_cvImage == null)
                return;

            switch (m_CurrentMode)
            {
                case ENUM_EmguCVControlEx_Mode.RectangleROI:
                    int w = (int)(e.X - this.startX);
                    int h = (int)(e.Y - this.startY);
                    this.graphics.DrawRectangle(new Pen(new SolidBrush(Color.Red)), this.startX, this.startY, w, h);
                    this.startDraw = false;
                    break;
                case ENUM_EmguCVControlEx_Mode.IrregularROI:
                    this.startDraw = false;
                    Point p = new Point(e.X, e.Y);
                    this.curvePoints.Add(p);
                    Pen redPen = new Pen(Color.Red, 1);
                    graphics.DrawClosedCurve(redPen, curvePoints.ToArray());

                    Mat roi = new Mat(m_cvImage.Size, DepthType.Cv8U, 3);
                    Mat dst = new Mat();
                    VectorOfVectorOfPoint contour = new VectorOfVectorOfPoint();
                    VectorOfPoint pts = new VectorOfPoint();
                    pts.Push(curvePoints.ToArray());
                    contour.Push(pts);
                    CvInvoke.DrawContours(roi, contour, 0, new MCvScalar(255, 255, 255), -1);
                    m_cvImage.CopyTo(dst, roi);
                    graphics.DrawImage(dst.ToImage<Bgr, Byte>().ToBitmap(), 0, 0, dst.Width, dst.Height);
                    break;
                default:
                    break;
            }
        }

        private void MoveImage(double xDelta, double yDelta)
        {
        }
        #endregion

        #region Draw ROI operation
        public object DrawCircleROI()
        {
            return null;
        }

        public object DrawRectangleROI()
        {
            Console.WriteLine("DrawRectangleROI");
            if (m_cvImage == null)
            {
                return null;
            }
            SetWokingMode(ENUM_EmguCVControlEx_Mode.RectangleROI);
            return new Rectangle(0, 0, 1, 1);
        }

        public object DrawRotateRectangleROI()
        {
            return null;
        }
        #endregion

        public void DrawIrregularROI()
        {
            Console.WriteLine("DrawIrregularROI");
            if (m_cvImage == null)
            {
                return;
            }
            SetWokingMode(ENUM_EmguCVControlEx_Mode.IrregularROI);
        }

        public void DrawContour()
        {
            Console.WriteLine("DrawContour");
            if (m_cvImage == null)
            {
                return;
            }
            SetWokingMode(ENUM_EmguCVControlEx_Mode.None);
            Image<Bgr, Byte> gray = new Image<Bgr, byte>(m_cvImage.Width, m_cvImage.Height);
            CvInvoke.CvtColor(m_cvImage, gray, ColorConversion.Bgr2Gray);
            CvInvoke.Threshold(gray, gray, 177, 255, 0);
            VectorOfVectorOfPoint vvp = new VectorOfVectorOfPoint();
            Mat hierarchy = new Mat();
            CvInvoke.FindContours(gray, vvp, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxNone);
            Image<Bgr, Byte> disp = new Image<Bgr, byte>(m_cvImage.Width, m_cvImage.Height);
            CvInvoke.DrawContours(disp, vvp, -1, new MCvScalar(255, 255, 255), 1);
            graphics.DrawImage(disp.ToBitmap(), ImagePart.X, ImagePart.Y, ImagePart.Width, ImagePart.Height);
        }

        #region Set working mode operation
        public void SetWokingMode(ENUM_EmguCVControlEx_Mode changeMode)
        {
            //Minimum	0	The ImageBox is only used for displaying image. No right-click menu nor Pan/Zoom available
            //RightClickMenu	1	Enable the right click menu
            //PanAndZoom	2	Enable Pan and Zoom
            //Everything	3	Support for the right click menu, Pan and Zoom

            this.cvImgBox.HorizontalScrollBar.Enabled = false;
            this.cvImgBox.VerticalScrollBar.Enabled = false;
            this.cvImgBox.FunctionalMode = ImageBox.FunctionalModeOption.Minimum;
            this.MouseDown -= this.EmguCV_MouseDown;
            this.MouseMove -= this.EmguCV_MouseMove;
            this.MouseUp -= this.EmguCV_MouseUp;
            m_CurrentMode = changeMode;
            switch (changeMode)
            {
                case ENUM_EmguCVControlEx_Mode.None:
                    break;
                case ENUM_EmguCVControlEx_Mode.Display:
                    break;
                //case ENUM_EmguCVControlEx_Mode.ImageZoom:
                //    this.cvImgBox.FunctionalMode = ImageBox.FunctionalModeOption.PanAndZoom;
                //    break;
                case ENUM_EmguCVControlEx_Mode.ImageMove:
                    this.cvImgBox.HorizontalScrollBar.Enabled = true;
                    this.cvImgBox.VerticalScrollBar.Enabled = true;
                    this.cvImgBox.FunctionalMode = ImageBox.FunctionalModeOption.Minimum;
                    break;
                case ENUM_EmguCVControlEx_Mode.ImageZoom:
                case ENUM_EmguCVControlEx_Mode.IrregularROI:
                case ENUM_EmguCVControlEx_Mode.RectangleROI:
                    this.MouseDown += this.EmguCV_MouseDown;
                    this.MouseMove += this.EmguCV_MouseMove;
                    this.MouseUp += this.EmguCV_MouseUp;
                    break;
                default:
                    break;
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
        #endregion
    }
}


