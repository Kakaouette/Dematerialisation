using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numerisation_GIST
{
    class Marqueurs
    {
        public static Image<Gray, Byte> img1 { get; private set; }
        public static Image<Gray, Byte> img2 { get; private set; }
        public static MatModification mat { get; private set; }
        public static List<Point> caseACocher;

       /* static void Main(string[] agrs)
        {
            //Chargement images et RLSA
            img1 = new Image<Gray, Byte>("..\\..\\Include\\IMG\\Modèle\\M2\\1.tif");
            //img2 = new Image<Gray, Byte>("..\\..\\Include\\IMG\\TestTrans\\2.tif");
            img2 = new Image<Gray, Byte>("..\\..\\Include\\IMG\\400\\Numérisation Niveau de gris\\Dossier Couleur\\1.tif");
            img2 = new ImageModification().redimensionner(img2, 1991, 2818);

            caseACocher = new List<Point>();
            caseACocher.Add(new Point(100,825));
            caseACocher.Add(new Point(100, 900));
            caseACocher.Add(new Point(100, 1045));
            caseACocher.Add(new Point(100, 1170));
            caseACocher.Add(new Point(100, 1385));
            caseACocher.Add(new Point(100, 1525));
            caseACocher.Add(new Point(100, 1660));
            caseACocher.Add(new Point(100, 1795));
            caseACocher.Add(new Point(100, 1865));
            caseACocher.Add(new Point(10, 1935));
            caseACocher.Add(new Point(10, 2005));
            caseACocher.Add(new Point(10, 2075));
            caseACocher.Add(new Point(10, 2145));

            Image<Gray, Byte> img1Temp, img2Temp, img3Temp, img4Temp, img5Temp, img6Temp, img7Temp, img8Temp, img9Temp;
            img1Temp = img1;
            img1Temp.Save("..\\..\\Include\\IMG\\TestTrans\\img1.tif");
            img2Temp = new ImageModification().convertionBinaire(img2);
            img2Temp.Save("..\\..\\Include\\IMG\\TestTrans\\img2.tif");


            //Il faut prendre marqueur sur l'image 1 et le retrouver sur l'image 2 puis faire translation
            Image<Gray, Byte> patron = img1Temp.GetSubRect(new Rectangle(130, 115, 335, 335));
            patron = new ImageModification().convertionBinaire(patron);
            patron.Save("..\\..\\Include\\IMG\\TestTrans\\patron.tif");
            patternFinding(img2Temp,patron);
            Console.WriteLine("FINI LOL");
            Console.ReadKey();

        }*/

        public List<Point> patternFinding(Image<Gray, Byte> num, PageModele modele)
        {
            List<Point> lesPoints = new List<Point>();
            Image<Gray, Byte> pattern = new ImageModification().rogner(modele.image, modele.marqueur);
            pattern = new ImageModification().convertionBinaire(pattern);
            //pattern.Save(Program.cheminTmp + "test.tif");
            Image<Gray, Byte> imgToShow = num.Copy();

            using (Image<Gray, float> result = num.MatchTemplate(pattern, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed))
            {
                double[] minValues, maxValues;
                
                Point[] minLocations, maxLocations;
                result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);

                if (maxValues[0] > 0.6)
                {
                    Rectangle match = new Rectangle(maxLocations[0],new Size(310,310));
                    imgToShow.Draw(match, new Gray(), 3);
                    foreach (CaseACocher p in modele.casesACocher)
                    {
                        lesPoints.Add(new Point(maxLocations[0].X + p.coord.X, maxLocations[0].Y + p.coord.Y));
                        match = new Rectangle(new Point(maxLocations[0].X + p.coord.X, maxLocations[0].Y + p.coord.Y), new Size(Program.tailleCheckBox, Program.tailleCheckBox));
                        imgToShow.Draw(match, new Gray(), 3);
                    }
                    
                }

            }
            imgToShow.Save("..\\..\\Include\\IMG\\TestTrans\\testTrouve.tif");
            return lesPoints;
        }
    }

        
}
