using System;
using Emgu.CV;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Configuration;
using System.Drawing.Imaging;
using Numerisation_GIST;
using Emgu.CV.Structure;

namespace TestCV
{
    public class Program
    {
        public static String cheminImage;
        public static String cheminTemp;
        private static Size tailleImg;
        //Classe contenant les méthodes lié à tesseract
        public static TesseractTraitement console { get; private set; }
        //Classe contenant les méthodes lié au traitement d'image (rognage, binarisation,...)
        public readonly static ImageTraitement imgTraitement = new ImageTraitement();
        //Liste les images contenu dans le dossier cheminImage
        private static List<Mat> lesImages;
        //Liste les pattern image (zone, mots à cherchés, numéro de page,...)
        private static List<PatternPage> lesImagesZone;

        //Charge certaines variable externalisé dans un fichier de config (App.config)
        static private void initVariable()
        {
            try
            {
                cheminImage = ConfigurationManager.AppSettings["cheminImage"];
                cheminTemp = ConfigurationManager.AppSettings["cheminTemp"];
                tailleImg = new Size(int.Parse(ConfigurationManager.AppSettings["tailleImg.w"]), int.Parse(ConfigurationManager.AppSettings["tailleImg.h"]));
                String tessdata = ConfigurationManager.AppSettings["tessdata"];
                console = new TesseractTraitement(tessdata);
            }catch(Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Erreur lors du chargement de la configuration, vérifiez le fichier de configuration."); 
                Console.WriteLine("L'application va s'arrêter");
                Console.ReadKey();
                System.Environment.Exit(1);
            }
            
        }

        //Si le dossier Temp existe on le supprime, puis on le créer
        static private void creerDossierTemp()
        {
            if (Directory.Exists(cheminTemp))
            {
                suppressionDossierTemp();
            }
            Directory.CreateDirectory(cheminTemp);
        }

        //init des pattern image avec zone et mot à chercher
        static private void initImageZone()
        {
            Console.WriteLine("Initialisation des patterns\n");
            lesImagesZone = new List<PatternPage>();
            //Création de page
            PatternPage p1 = new PatternPage(1);
            //Création de zone de recherche, le rectangle est une zone à rogner sur l'image, 
            //le tableau de mot est une liste de mot que l'ont peut trouver (dépend de si la numérisation n'a pas de décalage avec le modèle)
            p1.lesZones.Add(new ZoneVerif(new Rectangle(34, 772, 812, 442), new string[] {"biotechnologie", "biochimie"}));
            p1.lesZones.Add(new ZoneVerif(new Rectangle(76, 1522, 655, 299), new string[] {"informatique", "décision", "réhabilitation"}));
            Console.WriteLine(p1.ToString());

            PatternPage p2 = new PatternPage(2);
            p2.lesZones.Add(new ZoneVerif(new Rectangle(0, 1090, 604, 366), new string[] { "situation actuelle"}));
            p2.lesZones.Add(new ZoneVerif(new Rectangle(90, 1750, 840, 410), new string[] { "Enseignement secondaire", "Type de bac"}));
            Console.WriteLine(p2.ToString());

            PatternPage p3 = new PatternPage(3);
            p3.lesZones.Add(new ZoneVerif(new Rectangle(50, 1476, 1126, 402), new string[] {"évaluation de français", "Si oui, lequel", "référence pour les langues", "autres connaissances linguistiques" }));
            p3.lesZones.Add(new ZoneVerif(new Rectangle(0, 1826, 570, 270), new string[] {"anglais", "niveau obtenu", "autre langue"}));
            Console.WriteLine(p3.ToString());

            PatternPage p4 = new PatternPage(4);
            p4.lesZones.Add(new ZoneVerif(new Rectangle(0, 801, 1254, 549), new string[] { "en tant que", "Dernière entreprise ou organisme" }));
            p4.lesZones.Add(new ZoneVerif(new Rectangle(0, 2184, 1329, 600), new string[] { "un stage", "celui-ci s'est il", "contrat de travail" }));
            Console.WriteLine(p4.ToString());

            PatternPage p5 = new PatternPage(5);
            p5.lesZones.Add(new ZoneVerif(new Rectangle(0, 0, 1164, 531), new string[] { "Pour les formations en apprentissage", "Avez vous trouvé une entreprise"}));
            p5.lesZones.Add(new ZoneVerif(new Rectangle(0, 1638, 1212, 630), new string[] { "Certifie sur l'honneur", "Certifie sur ihonneur", "exactitude des renseignements" }));
            Console.WriteLine(p5.ToString());

            PatternPage p6 = new PatternPage(6);
            p6.lesZones.Add(new ZoneVerif(new Rectangle(642, 3, 1134, 441), new string[] { "Informations sur l’apprentissage", "Informations sur l'apprentissage", "Informations sur iapprentissage" }));
            p6.lesZones.Add(new ZoneVerif(new Rectangle(0, 1038, 996, 633), new string[] { "Processus de candidature", "La durée légale du travail", "Le recrutement en licence pro" }));
            Console.WriteLine(p6.ToString());

            PatternPage p7 = new PatternPage(7);
            p7.lesZones.Add(new ZoneVerif(new Rectangle(16, 1390, 632, 306), new string[] {"classement", "discipline"}));
            p7.lesZones.Add(new ZoneVerif(new Rectangle(0, 1956, 576, 298), new string[] {"signature", "fait le", "important : cette fiche"}));
            Console.WriteLine(p7.ToString());

            PatternPage p8 = new PatternPage(8);
            p8.lesZones.Add(new ZoneVerif(new Rectangle(0, 0, 1035, 552), new string[] { "Pièces à joindre au dossier", "Pour les documents rédigées", "Photocopie du ou des diplômes"}));
            p8.lesZones.Add(new ZoneVerif(new Rectangle(441, 1014, 1110, 597), new string[] { "université de la rochelle", "avenue michel crépeau"}));
            Console.WriteLine(p8.ToString());

            lesImagesZone.Add(p1);
            lesImagesZone.Add(p2);
            lesImagesZone.Add(p3);
            lesImagesZone.Add(p4);
            lesImagesZone.Add(p5);
            lesImagesZone.Add(p6);
            lesImagesZone.Add(p7);
            lesImagesZone.Add(p8);
            Console.WriteLine('\n');
        }

        //récupération des image en tif dans le dossier spécifié
        static private bool initMat()
        {
            Console.WriteLine("Chargement des images\n");
            lesImages = new List<Mat>();

            string[] tif = Directory.GetFiles(cheminImage, "*.tif");

            //Vérification du nombre d'image comparé au nombre de pattern
            if (tif.Length != lesImagesZone.Count)
            {
                Console.WriteLine("Le nombre d'image scanné est inférieur au nombre d'image modèle :");
                Console.WriteLine("Image scanné : " + lesImages.Count);
                Console.WriteLine("Image modèle : " + lesImagesZone.Count);
                return false;
            }

            foreach (string fichierImg in tif)
            {
                Console.WriteLine("Chargement de " + fichierImg);
                Mat img = CvInvoke.Imread(fichierImg, Emgu.CV.CvEnum.LoadImageType.Grayscale);
                lesImages.Add(img);
                //Vérification de l'image 
                if (!verifImg(fichierImg, img))
                    return false;
            }
            return true;
        }

        //Vérifie que les images chargé sont conformes (redimentionne les images trop grande)
        static bool verifImg(string path, Mat img)
        {
            int i = 1;

            //Récupération DPI
            Bitmap btp = new Bitmap(path);
            if(btp.HorizontalResolution < 240 || btp.VerticalResolution < 240)
            {
                Console.WriteLine("Le PPP d'une image est inférieur au minimum spécifié (240) :");
                Console.WriteLine("Image scanné : " + btp.HorizontalResolution + "x" + btp.VerticalResolution);
                return false;
            }
            //Libération du bitmap
            btp.Dispose();
            
            //Vérification de la taille en pixel
            if (!img.Size.Equals(tailleImg))
            {
                if((img.Size.Height > (tailleImg.Height - 25)) && (img.Size.Width > (tailleImg.Width - 25)))
                {
                    Mat imgR = imgTraitement.redimImage(img, tailleImg.Width, tailleImg.Height);
                    int index = lesImages.IndexOf(img);
                    lesImages.RemoveAt(index);
                    lesImages.Insert(index, imgR);
                }
                else
                {
                    Console.WriteLine("Une des images est trop petite (mauvaise qualité) :");
                    Console.WriteLine("Image scanné : " + img.Width + "x" + img.Height);
                    Console.WriteLine("Image modèle : " + tailleImg.Width + "x" + tailleImg.Height);
                    return false;
                }
            }
            return true;
        }

        static void suppressionDossierTemp()
        {
            foreach(String pathF in Directory.GetFiles(cheminTemp))
            {
                File.Delete(pathF);
            }
            Directory.Delete(cheminTemp);
        }

        static void Main(string[] args)
        {/*
            //Test pour retrouver la case coché

            initVariable();
            creerDossierTemp();
            Mat imgt = CvInvoke.Imread("..\\..\\Include\\IMG\\Template\\test.tif", Emgu.CV.CvEnum.LoadImageType.Grayscale);

            imgt = imgTraitement.convertBinOtsu(imgt);
            try {
                ImageTemplate template = new ImageTemplate();
                template.RLSA(imgt);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine("fin");
            Console.ReadKey();
            System.Environment.Exit(1);
            */

            Console.WriteLine("======================================================================================");
            Console.WriteLine("=================================== Initialisation ===================================");
            Console.WriteLine("======================================================================================");
            initVariable();
            //creerDossierTemp();
            initImageZone();
            if (!initMat())
            {
                Console.WriteLine("L'application va s'arrêter");
                Console.ReadKey();
                System.Environment.Exit(1);
            }

            //Dictionnaire contenant pour chaque pattern, la page numérisé
            Dictionary<PatternPage, Mat> lesCorrespondances = new Dictionary<PatternPage, Mat>();

            //Clone des pattern
            List<PatternPage> lesImagesZoneTemp = new List<PatternPage>(lesImagesZone);

            Console.WriteLine("======================================================================================");
            Console.WriteLine("===================================== Recherche ======================================");
            Console.WriteLine("======================================================================================");
            try {
                foreach (Mat img in lesImages)
                {
                    Console.WriteLine("Image " + (lesImages.IndexOf(img) + 1));
                    foreach (PatternPage pattern in lesImagesZoneTemp)
                    {
                        //Si l'image correspond au pattern, suppression du pattern dans la liste temp
                        if (pattern.isPage(img))
                        {
                            lesCorrespondances.Add(pattern, img);
                            //Commenté pour voir si le pattern ne correspond pas a plusieurs image
                            lesImagesZoneTemp.Remove(pattern);
                            Console.WriteLine("\tPage Pattern " + pattern.numero + " = Image " + (lesImages.IndexOf(lesCorrespondances[pattern]) + 1));
                            break;
                        }
                    }
                    Console.WriteLine();
                }

                Console.WriteLine("======================================================================================");
                Console.WriteLine("====================================== Résultat ======================================");
                Console.WriteLine("======================================================================================");
                ImageTemplate template = new ImageTemplate();
                
                foreach (PatternPage pattern in lesCorrespondances.Keys)
                {
                    Console.WriteLine("Page Pattern " + pattern.numero + " = Image " + (lesImages.IndexOf(lesCorrespondances[pattern]) + 1));
                    Mat structImg = template.RLSA(imgTraitement.convertBinOtsu(lesCorrespondances[pattern]));
                    structImg.Save(cheminTemp + "Struct-" + pattern.numero + ".tif");
                }
                Console.WriteLine("\nApplication terminée, appuyer sur une touche pour fermer");
                Console.ReadKey();
            }catch(Exception e)
            {
                Console.WriteLine("\nApplication terminée avec erreur, appuyer sur une touche pour fermer");
                Console.WriteLine(e);
                Console.ReadKey();
            }
            //suppressionDossierTemp();
        }
    }
}
