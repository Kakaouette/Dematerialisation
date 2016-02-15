using Emgu.CV;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCV
{
    //Classe listant une série de méthode intéragissant avec la console windows (notamment Tesseract)
    public class TesseractTraitement
    {
        Tesseract tesseract;

        public TesseractTraitement(String tessdata)
        {
            tesseract = new Tesseract(tessdata, "fra", OcrEngineMode.TesseractCubeCombined);
        }

        public String tesseractAnalyse(Image<Gray, byte> img)
        {
            tesseract.Recognize(img);
            return tesseract.GetText();
        }
    }
}
