using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerisation_GIST;
using Emgu.CV.Structure;
using Emgu.CV;
using System.Drawing;
using Emgu.CV.UI;
using Emgu.CV.CvEnum;

namespace Numerisation_GIST
{
    class TestDetectionBlanc
    {
        public static Image<Gray, byte> imgTemplate { get; private set; }
        public static Image<Gray, byte> img { get; private set; }
        public readonly static MatModification matModification = new MatModification();

        static void Main(string[] args)
        {
            Console.WriteLine("test1");
            imgTemplate = new Image<Gray, byte>("..\\..\\Include\\IMG\\testDetectionBlanc\\1MZoneT2.tif");
            Console.WriteLine("test2");
            img = new Image<Gray, byte>("..\\..\\Include\\IMG\\testDetectionBlanc\\1ZoneT2.tif");
            Console.WriteLine("test3");
            /*HistogramViewer.Show(imgTemplate);
            HistogramViewer.Show(img);*/
            imgTemplate = new ImageModification().convertionBinaire(imgTemplate);
            img = new ImageModification().convertionBinaire(img);
            // Create and initialize histogram
            DenseHistogram hist1 = new DenseHistogram(256, new RangeF(0.0f, 255.0f));
            // Histogram Computing
            hist1.Calculate<Byte>(new Image<Gray, byte>[] { img }, true, null);
            //hist1.
            // Create and initialize histogram
            DenseHistogram hist2 = new DenseHistogram(256, new RangeF(0.0f, 255.0f));
            // Histogram Computing
            hist2.Calculate<Byte>(new Image<Gray, byte>[] { imgTemplate }, true, null);


            Double result = Emgu.CV.CvInvoke.CompareHist(hist1, hist2, HistogramCompMethod.Correl);
            Console.WriteLine("Correlation : "+result);
            result = Emgu.CV.CvInvoke.CompareHist(hist1, hist2, HistogramCompMethod.Chisqr);
            Console.WriteLine("Chi-Square  : " + result);
            result = Emgu.CV.CvInvoke.CompareHist(hist1, hist2, HistogramCompMethod.Intersect);
            Console.WriteLine("Intersection : " + result);
            result = Emgu.CV.CvInvoke.CompareHist(hist1, hist2, HistogramCompMethod.Bhattacharyya);
            Console.WriteLine("Bhattacharyya distance  : " + result);
            result = Emgu.CV.CvInvoke.CompareHist(hist1, hist2, HistogramCompMethod.Hellinger);
            Console.WriteLine("Synonym for Bhattacharyya  : " + result);
            result = Emgu.CV.CvInvoke.CompareHist(hist1, hist2, HistogramCompMethod.ChisqrAlt);
            Console.WriteLine("Alternative Chi-Square  : " + result);

            Console.WriteLine(img.Rows + " " + img.Cols);
            double blackPixelImg = (img.Rows * img.Cols) - img.CountNonzero()[0];
            double blackPixelImgTemplate = (imgTemplate.Rows * imgTemplate.Cols) - imgTemplate.CountNonzero()[0];
            double resultat = blackPixelImgTemplate / blackPixelImg;
            Console.WriteLine(resultat);

            Console.ReadKey();
        }
    }
}
