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

            result.Save("D:\\Desktop\\IMG\\TEMP\\" + Type + "match.tif");
            sourceImage.Save("D:\\Desktop\\IMG\\TEMP\\" + Type + "sourceImage.tif");
            return true;
        }

        public bool foundCircle(Mat img)
        {
            //CircleF[] circles = CvInvoke.HoughCircles(img, Emgu.CV.CvEnum.HoughType.Gradient, 1, 5, )
            return true;
        }

        public void RLSA(Mat tmpImg)
        {
            int hor_thres = 22;
            int zero_count = 0;
            int one_flag = 0;
            for (int i = 0; i < tmpImg.Rows; i++)
            {
                for (int j = 0; j < tmpImg.Cols; j++)
                {
                    if (tmpImg.GetData(new int[] { i, j })[0] == 255)//at<uchar>(i, j) == 255)
                    {
                        if (one_flag == 255)
                        {
                            if (zero_count <= hor_thres)
                            {
                                //tmpImg.SetTo();
                                //tmpImg(cv::Range(i, i + 1), cv::Range(j - zero_count, j)).setTo(cv::Scalar::all(255));
                                tmpImg = new Mat();
                                //CvInvoke.cvRange(tmpImg, i, i + 1);
                                //.ToImage<Gray, byte>();
                            }
                            else
                            {
                                one_flag = 0;
                            }
                            zero_count = 0;
                        }
                        one_flag = 255;
                    }
                    else
                    {
                        if (one_flag == 255)
                        {
                            zero_count = zero_count + 1;
                        }
                    }
                }
            }
        }
    }
}
