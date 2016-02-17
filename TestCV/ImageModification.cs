using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numerisation_GIST
{
    //Classe listant une série de méthode permettant la modification d'image (Classe Mat)
    public class ImageModification
    {
        //Retourne une image convertit en binaire via la méthode Otsu
        public Image<Gray, byte> convertionBinaire(Image<Gray, byte> img)
        {
            Image<Gray, byte> src = img.Clone();
            //Init image de destination
            Image<Gray, byte> dest = new Image<Gray, byte>(img.Size);
            //calcul de Otsu
            CvInvoke.Threshold(src, dest, 0, 255, Emgu.CV.CvEnum.ThresholdType.Otsu | Emgu.CV.CvEnum.ThresholdType.Binary);
            //retourne l'image en binaire
            return dest;
        }

        //Retourne une nouvelle image rognée
        public Image<Gray, byte> rogner(Image<Gray, byte> img, int x, int y, int longueur, int largeur)
        {
            return new Mat(img.Mat, new System.Drawing.Rectangle(x, y, longueur, largeur)).ToImage<Gray, byte>();
        }

        //Retourne une nouvelle image rognée
        public Image<Gray, byte> rogner(Image<Gray, byte> img, System.Drawing.Rectangle rect)
        {
            return new Mat(img.Mat, rect).ToImage<Gray, byte>();
        }

        //Redimensionne
        public Image<Gray, byte> redimensionner(Image<Gray, byte> img, int longueur, int largeur)
        {
            Image<Gray, byte> imgR = img.Clone();
            return imgR.Resize(longueur, largeur, Emgu.CV.CvEnum.Inter.Linear, false);
        }
    }
}
