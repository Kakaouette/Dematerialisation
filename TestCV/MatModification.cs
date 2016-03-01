using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;

namespace Numerisation_GIST
{
    public class MatModification
    {
        //Classe contenant les méthodes lié au traitement d'image (rognage, binarisation,...)
        public readonly static ImageModification imageModification = new ImageModification();

        public Image<Gray, byte> RLSAH(Image<Gray, byte> img)
        {
            int hor_thres = 22;
            int zero_count = 0;
            int one_flag = 0;
            Byte[,,] data = img.Data;

            for (int i = 0; i < img.Mat.Rows; i++)
            {
                for (int j = 0; j < img.Mat.Cols; j++)
                {
                    if (data[i, j, 0] == 0)
                    {
                        if (one_flag == 255)
                        {
                            if (zero_count <= hor_thres)
                            {
                                for (int j2 = j - zero_count; j2 <= j; j2++)
                                {
                                    if (i >= 0 && j2 >= 0)
                                    {
                                        data[i, j2, 0] = 0;
                                    }
                                }
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
            return new Image<Gray, byte>(data);
        }

        public Image<Gray, byte> RLSAV(Image<Gray, byte> img)
        {
            int hor_thres = 5;
            int zero_count = 0;
            int one_flag = 0;
            Byte[,,] data = img.Data;

            for (int i = 0; i < img.Mat.Cols; i++)
            {
                for (int j = 0; j < img.Mat.Rows; j++)
                {
                    if (data[j, i, 0] == 0)
                    {
                        if (one_flag == 255)
                        {
                            if (zero_count <= hor_thres)
                            {
                                for (int j2 = j - zero_count; j2 <= j; j2++)
                                {
                                    if (i >= 0 && j2 >= 0)
                                    {
                                        data[j2, i, 0] = 0;
                                    }
                                }
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
            return new Image<Gray, byte>(data);
        }

        //Implémentation de RLSA (j'ai rien compris mais ça marche) retourne le document structuré
        public Image<Gray, byte> RLSA(Image<Gray, byte> tmpImg)
        {
            Image<Gray, byte> calcul = tmpImg.Clone();
            Image<Gray, byte> vertical = RLSAV(calcul);
            Image<Gray, byte> horizontal = RLSAH(calcul);
            Image<Gray, byte> structure = tmpImg.Clone();

            CvInvoke.BitwiseAnd(vertical, horizontal, structure);
            return structure;
        }

        //Calcule le ratio de pixels noir entre une image modèle et une image scanné
        public double RatioPixelsNoir(Image<Gray, byte> image, Image<Gray, byte> imageModele)
        {
            Image<Gray, byte> imageBinaire = new ImageModification().convertionBinaire(image);
            Image<Gray, byte> imageModeleBinaire = new ImageModification().convertionBinaire(imageModele);

            double blackPixelImage = (imageBinaire.Rows * imageBinaire.Cols) - imageBinaire.CountNonzero()[0];
            double blackPixelImageModele = (imageModeleBinaire.Rows * imageModeleBinaire.Cols) - imageModeleBinaire.CountNonzero()[0];
            double resultat = blackPixelImageModele / blackPixelImage;
            Console.WriteLine("L'image modèle contient " + resultat * 100 + "% du total de pixels noirs de l'image scanné");

            return resultat;
        }

        //Indique si les caches sont cochées ou non à partir de 2 listes de points données en paramètres. La 1ère liste permet de connaitre les coordonnées des cases pour cette image (l'image modèle), idem pour la 2ème liste mais avec la 2ème image.
        public String[,] CaseCoche(List<Image<Gray, Byte>> lesCasesModele, List<Image<Gray, Byte>> lesCases)
        {
            int taille = lesCasesModele.Count;
            String[,] resultat = new String[taille, 2];
            //CaseACocher[] tableauModele = new CaseACocher[taille];
            //CaseACocher[] tableauNum = new CaseACocher[taille];
            //p.casesACocher.CopyTo(tableauModele);
            //liste.CopyTo(tableauNum);

            //Image<Gray, byte> imageModele;
            //Image<Gray, byte> image;


            for (int i = 0; i < taille - 1; i++)
            {
                //imageModele = imageModification.rogner(p.image, tableauModele[i].coord.X + p.marqueur.X, tableauModele[i].coord.Y + p.marqueur.Y, 50, 50);
                //imageModele.Save("..\\..\\Include\\IMG\\TestTrans\\testRognMod" + i + ".tif");
                //image = imageModification.rogner(img, tableauNum[i].coord.X, tableauNum[i].coord.Y, 50, 50);
                //image.Save("..\\..\\Include\\IMG\\TestTrans\\testRogn" + i + ".tif");

                double ratio = RatioPixelsNoir(lesCases[i], lesCasesModele[i]);
                resultat[i, 0] = "case " + i;
                if (ratio < double.Parse(ConfigurationManager.AppSettings["ratioCaseACocher"]))
                {
                    //La case est cochée
                    resultat[i, 1] = "Cochée";
                    Console.WriteLine(resultat[i, 0] + " coché");
                }
                else
                {
                    //La case n'est pas cochée
                    resultat[i, 1] = "Non cochée";
                    Console.WriteLine(resultat[i, 0] + " non coché");
                }
            }

            return resultat;
        }
    }
}
