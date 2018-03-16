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

        List<Rectangle> m_cvObjectList = new List<Rectangle>();
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

        private bool m_bCanMove = false;

        public EmguCVControlEx(int w, int h)
        {
            InitializeComponent(w, h);
            SetWokingMode(ENUM_EmguCVControlEx_Mode.None);
            graphics = this.CreateGraphics();
        }
        #region Display the picture operation
        public void DislpayObj(object obj)
        {
            if (obj == null)
                return;
            if (obj is Mat)
            {
                m_cvObjectList.Clear();
                m_showstingList.Clear();
                m_cvImage = obj as Mat;
                ResetDisplayImag(m_cvImage);
            }
            else
            {
                Rectangle cvObj = (Rectangle)obj;
                m_cvObjectList.Add(cvObj);
                DisPlayObject(cvObj);
            }
        }

        private void DisPlayObject(Rectangle obj)
        {
            if (obj != null)
            {
                CvInvoke.cvSetImageROI(m_cvImage, obj);
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
                m_CurrentZoomRate = 1.0 + ZoomScale;
                double ratioX = (x - this.ImagePart.X) * ZoomScale;
                double ratioY = (y - this.ImagePart.Y) * ZoomScale;
                CurrentRectangle.X = (int)(this.ImagePart.X - ratioX);
                CurrentRectangle.Y = (int)(this.ImagePart.Y - ratioY);
                CurrentRectangle.Width = (int)(this.ImagePart.Width * m_CurrentZoomRate);
                CurrentRectangle.Height = (int)(this.ImagePart.Height * m_CurrentZoomRate);
            }
            else
            {
                m_CurrentZoomRate = m_CurrentZoomRate + ZoomScale;
                double ratioX = this.image.Width / m_CurrentZoomRate / CurrentRectangle.Width;
                double ratioY = this.image.Height / m_CurrentZoomRate / CurrentRectangle.Height;
                CurrentRectangle.X = (int)(x - (x - CurrentRectangle.X) * ratioX);
                CurrentRectangle.Y = (int)(y - (y - CurrentRectangle.Y) * ratioY);
                CurrentRectangle.Width = (int)(this.image.Width / m_CurrentZoomRate);
                CurrentRectangle.Height = (int)(this.image.Height / m_CurrentZoomRate);
            }

            ImagePart = CurrentRectangle;

            BufferedGraphicsContext currentContext = BufferedGraphicsManager.Current;
            BufferedGraphics myBuffer = currentContext.Allocate(graphics, new Rectangle(0, 0, this.Width, this.Height));
            Graphics g = myBuffer.Graphics;
            g.Clear(Color.Black);
            g.DrawImage(bitmap, ImagePart.X, ImagePart.Y, ImagePart.Width, ImagePart.Height);
            myBuffer.Render(graphics);
            myBuffer.Dispose();
            g.Dispose();
        }

        #region Move picture operation
        private void EmguCV_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (this.m_cvImage == null)
                return;
            this.startX = e.X;
            this.startY = e.Y;

            switch (m_CurrentMode)
            {
                case ENUM_EmguCVControlEx_Mode.RectangleROI:
                    this.startDraw = true;
                    break;
                case ENUM_EmguCVControlEx_Mode.IrregularROI:
                    this.startDraw = true;
                    Point p = new Point(e.X, e.Y);
                    this.curvePoints.Add(p);
                    break;
                case ENUM_EmguCVControlEx_Mode.ImageZoom:
                    bool bZoomIn = (MouseButtons.Left == e.Button);
                    ZoomImage(e.X, e.Y, bZoomIn);
                    break;
                case ENUM_EmguCVControlEx_Mode.ImageMove:
                    m_bCanMove = (MouseButtons.Left == e.Button);
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
                case ENUM_EmguCVControlEx_Mode.ImageMove:
                    if (m_bCanMove)
                    {
                        MoveImage(e.X - startX, e.Y - startY);
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
                case ENUM_EmguCVControlEx_Mode.RectangleROI:
                    int w = (int)(e.X - this.startX);
                    int h = (int)(e.Y - this.startY);
                    this.graphics.DrawRectangle(new Pen(new SolidBrush(Color.Red)), this.startX, this.startY, w, h);
                    this.startDraw = false;
                    this.DislpayObj(new Rectangle((int)startX, (int)startY, w, h));
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
                case ENUM_EmguCVControlEx_Mode.ImageMove:
                    if (m_bCanMove)
                    {
                        ImagePart.X += (int)(e.X - startX);
                        ImagePart.Y += (int)(e.Y - startY);
                        m_bCanMove = false;
                    }
                    break;
                default:
                    break;
            }
        }

        private void MoveImage(double xDelta, double yDelta)
        {
            BufferedGraphicsContext currentContext = BufferedGraphicsManager.Current;
            BufferedGraphics myBuffer = currentContext.Allocate(graphics, new Rectangle(0, 0, this.Width, this.Height));
            Graphics g = myBuffer.Graphics;
            g.Clear(Color.Black);
            g.DrawImage(bitmap, (float)(ImagePart.X + xDelta), (float)(ImagePart.Y + yDelta), ImagePart.Width, ImagePart.Height);
            myBuffer.Render(graphics);
            myBuffer.Dispose();
            g.Dispose();
        }
        #endregion

        #region Draw ROI operation
        public object DrawCircleROI()
        {
            return null;
        }

        public object DrawRectangleROI()
        {
            if (m_cvImage == null)
            {
                return null;
            }
            SetWokingMode(ENUM_EmguCVControlEx_Mode.RectangleROI);
            return m_cvImage;
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
                case ENUM_EmguCVControlEx_Mode.ImageZoom:
                    this.MouseDown += this.EmguCV_MouseDown;
                    break;
                case ENUM_EmguCVControlEx_Mode.ImageMove:
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
