using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;

namespace TST.Vision.Thirdparty
{
    public class CvTypeConvert
    {
        public static object GetImag(string filename)
        {
            Mat image = CvInvoke.Imread(filename, Emgu.CV.CvEnum.ImreadModes.AnyColor);
            return image;
        }
    }
}

