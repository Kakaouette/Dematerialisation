using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;

namespace Numerisation_GIST
{
    class ImageTemplate
    {
        public bool isInTemplate(Mat sourceImage, Mat templateImage, Emgu.CV.CvEnum.TemplateMatchingType Type)
        {
            Image<Gray, float> result = sourceImage.ToImage<Gray, float>().MatchTemplate(templateImage.ToImage<Gray, float>(), Type);
            double[] min, max;
            Point[] point1, point2;
            result.MinMax(out min, out max, out point1, out point2);
            Point point3 = new Point();
            point3.X = point1[0].X + templateImage.Width;
            point3.Y = point1[0].Y + templateImage.Height;
            MCvScalar scalar = new MCvScalar(1);

            Console.WriteLine("1="+point1[0]);
            Console.WriteLine("2=" + point2[0]);
            Console.WriteLine("3=" + point3);

            CvInvoke.Rectangle(sourceImage, new Rectangle(point1[0].X, point1[0].Y, (point3.X - point1[0].X), (point3.Y - point1[0].Y)), scalar, 5, Emgu.CV.CvEnum.LineType.EightConnected, 0);
            //CvInvoke.Rectangle(sourceImage.Ptr, point1[0], point3, scalar, 5, Emgu.CV.CvEnum.LINE_TYPE.EIGHT_CONNECTED, 0);

            //this.pictureBox1.Image = sourceImage.ToBitmap();
            result.Save("D:\\Desktop\\IMG\\TEMP\\" + Type + "match.tif");
            sourceImage.Save("D:\\Desktop\\IMG\\TEMP\\" + Type + "sourceImage.tif");
            return true;
        }
    }
}
