using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCV
{
    //Classe listant une série de méthode permettant la modification d'image (Classe Mat)
    public class ImageTraitement
    {
        //Retourne une image convertit en binaire via la méthode Otsu
        public Mat convertBinOtsu(Mat Img_Org_Gray)
        {
            //Init image de destination
            Mat Img_Dest_Gray = new Mat();
            //calcul de Otsu
            CvInvoke.Threshold(Img_Org_Gray, Img_Dest_Gray, 0, 255, Emgu.CV.CvEnum.ThresholdType.Otsu | Emgu.CV.CvEnum.ThresholdType.Binary);
            //retourne l'image en binaire
            return Img_Dest_Gray;
        }

        //Retourne une nouvelle image rognée
        public Mat rognerImage(Mat img, int x, int y, int longueur, int largeur)
        {
            return new Mat(img, new System.Drawing.Rectangle(x, y, longueur, largeur));
        }

        //Retourne une nouvelle image rognée
        public Mat rognerImage(Mat img, System.Drawing.Rectangle rect)
        {
            return new Mat(img, rect);
        }

        //Redimentionne
        public Mat redimImage(Mat img, int longueur, int largeur)
        {
            return new  Mat(img.ToImage<Gray, Byte>().Resize(longueur, largeur, Emgu.CV.CvEnum.Inter.Linear, false).Mat, new Rectangle(0, 0, longueur, largeur));
        }
    }
}
