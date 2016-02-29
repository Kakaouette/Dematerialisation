using Emgu.CV;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numerisation_GIST
{
    //Classe listant une série de méthode intéragissant avec la console windows (notamment Tesseract)
    public class TesseractTraitement
    {
        private Tesseract tesseract;

        public TesseractTraitement(String tessdata)
        {
            tesseract = new Tesseract(tessdata, "fra", OcrEngineMode.TesseractCubeCombined);
        }

        public String tesseractAnalyse(Image<Gray, byte> img)
        {
            tesseract.Recognize(img);
            return tesseract.GetText();
        }

        public List<Rectangle> selectRectGauche(Image<Gray, Byte> imgcoucou, String mot, int taille, int offset)
        {
            List<Rectangle> lesRect = new List<Rectangle>();
            tesseract.Recognize(imgcoucou);
            Emgu.CV.OCR.Tesseract.Character[] coucou = tesseract.GetCharacters();
            String stringcoucou = "";
            foreach (Emgu.CV.OCR.Tesseract.Character c in coucou)
            {
                stringcoucou += c.Text;
            }
            stringcoucou = stringcoucou.ToLower();
            mot = mot.ToLower();
            int depassement = 0;

            while (stringcoucou.ToString().IndexOf(mot) != -1)
            {
                int s = stringcoucou.ToString().IndexOf(mot);
                stringcoucou = stringcoucou.Substring(s + 1);
                Rectangle r = new Rectangle(0, coucou[depassement + s].Region.Y + offset, coucou[depassement + s].Region.X, taille);
                lesRect.Add(r);
                depassement = depassement + s + 1;
            }
            return lesRect;
        }

        public List<Rectangle> selectRectAll(Image<Gray, Byte> imgcoucou, String mot, int taille, int offset)
        {
            List<Rectangle> lesRect = new List<Rectangle>();
            tesseract.Recognize(imgcoucou);
            Emgu.CV.OCR.Tesseract.Character[] coucou = tesseract.GetCharacters();
            String stringcoucou = "";
            foreach (Emgu.CV.OCR.Tesseract.Character c in coucou)
            {
                stringcoucou += c.Text;
            }
            stringcoucou = stringcoucou.ToLower();
            mot = mot.ToLower();
            int depassement = 0;

            while (stringcoucou.ToString().IndexOf(mot) != -1)
            {
                int s = stringcoucou.ToString().IndexOf(mot);
                stringcoucou = stringcoucou.Substring(s + 1);
                Rectangle r = new Rectangle(0, coucou[depassement + s].Region.Y + offset, imgcoucou.Width, taille);
                lesRect.Add(r);
                depassement = depassement + s + 1;
            }
            return lesRect;
        }
    }
}
