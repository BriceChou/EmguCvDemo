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
        public static object GetImag(string file)
        {
            Mat image = CvInvoke.Imread(file);
            return image;
        }
    }
}
