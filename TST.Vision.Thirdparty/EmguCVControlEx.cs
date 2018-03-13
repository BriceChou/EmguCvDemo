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
        IrregularROI
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

            this.cvImgBox.Image = img;
            this.cvImgBox.Location = new Point(this.ImagePart.X, this.ImagePart.Y);
            this.cvImgBox.Size = new Size(this.ImagePart.Width, this.ImagePart.Height);
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

        }

        #region Move picture operation
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
            SetDisPlayMode();
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
            Console.WriteLine("DrawContour vvp.length = " + vvp.Size);
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
            switch (changeMode)
            {
                case ENUM_EmguCVControlEx_Mode.None:
                    break;
                case ENUM_EmguCVControlEx_Mode.Display:
                    break;
                case ENUM_EmguCVControlEx_Mode.ImageZoom:
                    this.cvImgBox.FunctionalMode = ImageBox.FunctionalModeOption.PanAndZoom;
                    break;
                case ENUM_EmguCVControlEx_Mode.ImageMove:
                    this.cvImgBox.HorizontalScrollBar.Enabled = true;
                    this.cvImgBox.VerticalScrollBar.Enabled = true;
                    this.cvImgBox.FunctionalMode = ImageBox.FunctionalModeOption.Minimum;
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


