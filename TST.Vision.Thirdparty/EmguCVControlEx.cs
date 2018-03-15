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
        List<Mat> m_cvObjectList = new List<Mat>();
        List<Point> curvePoints = new List<Point>();
        List<cvShowString> m_showstingList = new List<cvShowString>();

        Graphics graphics = null;
        float startX = 0;
        float startY = 0;

        Boolean startDraw = false;
        private ENUM_EmguCVControlEx_Mode m_CurrentMode = ENUM_EmguCVControlEx_Mode.None;

        private const double ZoomScale = 0.1;
        private double m_CurrentZoomRate = 1.0;

        int _centerPointY = 0;
        int _centerPointX = 0;
        Point _mouseClientPoint = new Point();

        Rectangle ImagePart = new Rectangle(0, 0, 0, 0);
        ImageBox cvImgBox = new ImageBox();

        public EmguCVControlEx(int w, int h)
        {
            InitializeComponent(w, h);
            SetWokingMode(ENUM_EmguCVControlEx_Mode.None);
            graphics = this.CreateGraphics();

            _centerPointX = this.Width / 2;
            _centerPointY = this.Height / 2;
            this.Controls.Add(cvImgBox);
        }

        #region Draw coordinate feature
        private void DrawCoordinate(int startX, int startY, int width, int height)
        {
            Console.WriteLine(startX);
            Console.WriteLine(startY);

            Point xAxisStart = new Point(0, startY);
            Point xAxisEnd = new Point(width, startY);

            Point yAxisStart = new Point(startX, 0);
            Point yAxisEnd = new Point(startX, height);

            Pen myPen = new Pen(Color.Red, 3);

            graphics.DrawLine(myPen, xAxisStart, xAxisEnd);
            graphics.DrawLine(myPen, yAxisStart, yAxisEnd);
        }
        #endregion

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
            //DrawCoordinate(this.ImagePart.X, this.ImagePart.Y, this.ImagePart.Width, this.ImagePart.Height);
            DrawCoordinate(_centerPointX, _centerPointY, this.Width, this.Height);
            //Reset current rate to 1
            m_CurrentZoomRate = 1.0;
        }
        #endregion

        public void ShowMessage(int x, int y, string message, System.Drawing.Color color)
        {
            Point position = new Point(x, y);
            Font font = new Font("黑体", 16, GraphicsUnit.Pixel);
            SolidBrush fontLine = new SolidBrush(color);
            graphics.DrawString(message, font, fontLine, position);
        }

        #region Zoom mode operation
        private void EmguCV_Zoom_MouseDown(object sender, MouseEventArgs e)
        {
            _mouseClientPoint = this.PointToClient(Control.MousePosition);

            switch (e.Button)
            {
                case MouseButtons.Left:

                    m_CurrentZoomRate += ZoomScale;
                    Console.WriteLine("Left");
                    Console.WriteLine(_mouseClientPoint);
                    ZoomImage(_mouseClientPoint.X, _mouseClientPoint.Y, true);
                    break;
                case MouseButtons.Right:
                    Console.WriteLine("Right");
                    if (m_CurrentMode > 0)
                    {
                        m_CurrentZoomRate -= ZoomScale;
                    }
                    else
                    {
                        m_CurrentZoomRate = 0.1;
                    }
                    Console.WriteLine(_mouseClientPoint);
                    ZoomImage(_mouseClientPoint.X, _mouseClientPoint.Y, false);
                    break;
                default:
                    break;
            }
        }

        private void ZoomImage(double x, double y, bool bZoomIn)
        {
            int xAdd = 0;
            int yAdd = 0;

            if (bZoomIn)
            {
                //Handle the small image
                if (this.ImagePart.Width < this.Width && this.ImagePart.Height < this.Height)
                {
                    this.ImagePart.Width = (int)(this.ImagePart.Width * m_CurrentZoomRate);
                    this.ImagePart.Height = (int)(this.ImagePart.Height * m_CurrentZoomRate);
                }
                else
                {
                    this.ImagePart.Width = (int)(this.ImagePart.Width * m_CurrentZoomRate);
                    this.ImagePart.Height = (int)(this.ImagePart.Height * m_CurrentZoomRate);
                    xAdd = (int)(x * (m_CurrentZoomRate - 1.0));
                    yAdd = (int)(y * (m_CurrentZoomRate - 1.0));
                    this.ImagePart.X = this.ImagePart.X - xAdd;
                    this.ImagePart.Y = this.ImagePart.Y - yAdd;
                }
            }
            else
            {
                if (m_CurrentZoomRate < 1.0)
                {
                    this.ImagePart.Width = (int)(this.ImagePart.Width / m_CurrentZoomRate);
                    this.ImagePart.Height = (int)(this.ImagePart.Height / m_CurrentZoomRate);
                    xAdd = (int)(x * (1.0 - 1.0 / m_CurrentZoomRate));
                    yAdd = (int)(y * (1.0 - 1.0 / m_CurrentZoomRate));
                    this.ImagePart.X = this.ImagePart.X + xAdd;
                    this.ImagePart.Y = this.ImagePart.Y + yAdd;
                }
                this.ImagePart.Width = (int)(this.ImagePart.Width / m_CurrentZoomRate);
                this.ImagePart.Height = (int)(this.ImagePart.Height / m_CurrentZoomRate);
                xAdd = (int)(x * (1.0 - 1.0 / m_CurrentZoomRate));
                yAdd = (int)(y * (1.0 - 1.0 / m_CurrentZoomRate));
                this.ImagePart.X = this.ImagePart.X + xAdd;
                this.ImagePart.Y = this.ImagePart.Y + yAdd;
            }
            Console.WriteLine("m_CurrentZoomRate" + m_CurrentZoomRate);
            Console.WriteLine("Width" + this.ImagePart.Width);
            Console.WriteLine("Height" + this.ImagePart.Height);
            graphics.DrawImage(this.image.ToBitmap(), this.ImagePart.X, this.ImagePart.Y, this.ImagePart.Width, this.ImagePart.Height);
        }
        #endregion

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
                    //bool bZoomIn = (MouseButtons.Left == e.Button);
                    //ZoomImage(e.X, e.Y, bZoomIn);
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
            //SetDisPlayMode();
            SetWokingMode(ENUM_EmguCVControlEx_Mode.RectangleROI);
            return new Rectangle(0, 0, 1, 1);
        }

        public object DrawRotateRectangleROI()
        {
            return null;
        }

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
        #endregion

        #region Set working mode operation
        public void SetWokingMode(ENUM_EmguCVControlEx_Mode changeMode)
        {
            this.MouseUp -= this.EmguCV_MouseUp;
            this.MouseMove -= this.EmguCV_MouseMove;
            this.MouseDown -= this.EmguCV_MouseDown;
            this.MouseDown -= this.EmguCV_Zoom_MouseDown;

            m_CurrentMode = changeMode;
            switch (changeMode)
            {
                case ENUM_EmguCVControlEx_Mode.None:
                    break;
                case ENUM_EmguCVControlEx_Mode.Display:
                    break;
                case ENUM_EmguCVControlEx_Mode.ImageZoom:
                    this.MouseDown += this.EmguCV_Zoom_MouseDown;
                    break;
                case ENUM_EmguCVControlEx_Mode.ImageMove:
                    break;
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
