using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Configuration;
using System.Drawing.Imaging;
using Numerisation_GIST;
using Emgu.CV.Structure;
using Emgu.CV;

namespace Numerisation_GIST
{
    public class Program
    {
        public static String cheminModele { get; private set; }
        public static String cheminImage { get; private set; }
        public static String cheminTemp { get; private set; }
        public static Size tailleImg { get; private set; }
        public static int scanDPI { get; private set; }
        //Classe contenant les méthodes lié à tesseract
        public static TesseractTraitement console { get; private set; }
        //Classe contenant les méthodes lié au traitement d'image (rognage, binarisation,...)
        public readonly static ImageTraitement imgTraitement = new ImageTraitement();
        public readonly static ImageTemplate imgTemplate = new ImageTemplate();
        public readonly static Numerisation numerisation;// = new Numerisation();
        //Liste les images contenu dans le dossier cheminImage
        private static List<Image<Gray, byte>> lesImages;
        //Liste les pattern image (zone, mots à cherchés, numéro de page,...)
        private static List<PatternPage> lesImagesZone;

        static String verifChemin(string chemin)
        {
            if(!chemin[chemin.Length - 1].Equals("\\"))
            {
                chemin = chemin + "\\";
            }
            return chemin;
        }

        //Charge certaines variable externalisé dans un fichier de config (App.config)
        static private void initVariable()
        {
            try
            {
                scanDPI = int.Parse(ConfigurationManager.AppSettings["scanDPI"]);
                cheminModele = verifChemin(ConfigurationManager.AppSettings["cheminModele"]);
                cheminImage = verifChemin(ConfigurationManager.AppSettings["cheminImage"]);
                cheminTemp = verifChemin(ConfigurationManager.AppSettings["cheminTemp"]);
                tailleImg = new Size(int.Parse(ConfigurationManager.AppSettings["tailleImg.w"]), int.Parse(ConfigurationManager.AppSettings["tailleImg.h"]));
                String tessdata = verifChemin(ConfigurationManager.AppSettings["tessdata"]);
                console = new TesseractTraitement(tessdata);
            }
            catch (System.FormatException e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Erreur lors du chargement de la configuration : scanDPI et tailleImg doivent être des nombres");
                Console.WriteLine("L'application va s'arrêter");
                Console.ReadKey();
                System.Environment.Exit(1);
            }
            catch (Exception e)
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
            PatternPage p1 = new PatternPage(1, cheminModele + "1.tif");
            //Création de zone de recherche, le rectangle est une zone à rogner sur l'image, 
            //le tableau de mot est une liste de mot que l'ont peut trouver (dépend de si la numérisation n'a pas de décalage avec le modèle)
            p1.lesZones.Add(new ZoneVerif(new Rectangle(34, 772, 812, 442), new string[] {"biotechnologie", "biochimie"}));
            p1.lesZones.Add(new ZoneVerif(new Rectangle(76, 1522, 655, 299), new string[] {"informatique", "décision", "réhabilitation"}));
            Console.WriteLine(p1.ToString());

            PatternPage p2 = new PatternPage(2, cheminModele + "2.tif");
            p2.lesZones.Add(new ZoneVerif(new Rectangle(0, 1090, 604, 366), new string[] { "situation actuelle"}));
            p2.lesZones.Add(new ZoneVerif(new Rectangle(90, 1750, 840, 410), new string[] { "Enseignement secondaire", "Type de bac"}));
            Console.WriteLine(p2.ToString());

            PatternPage p3 = new PatternPage(3, cheminModele + "3.tif");
            p3.lesZones.Add(new ZoneVerif(new Rectangle(50, 1476, 1126, 402), new string[] {"évaluation de français", "Si oui, lequel", "référence pour les langues", "autres connaissances linguistiques" }));
            p3.lesZones.Add(new ZoneVerif(new Rectangle(0, 1826, 570, 270), new string[] {"anglais", "niveau obtenu", "autre langue"}));
            Console.WriteLine(p3.ToString());

            PatternPage p4 = new PatternPage(4, cheminModele + "4.tif");
            p4.lesZones.Add(new ZoneVerif(new Rectangle(0, 801, 1254, 549), new string[] { "en tant que", "Dernière entreprise ou organisme" }));
            p4.lesZones.Add(new ZoneVerif(new Rectangle(0, 2184, 1329, 600), new string[] { "un stage", "celui-ci s'est il", "contrat de travail" }));
            Console.WriteLine(p4.ToString());

            PatternPage p5 = new PatternPage(5, cheminModele + "5.tif");
            p5.lesZones.Add(new ZoneVerif(new Rectangle(0, 0, 1164, 531), new string[] { "Pour les formations en apprentissage", "Avez vous trouvé une entreprise"}));
            p5.lesZones.Add(new ZoneVerif(new Rectangle(0, 1638, 1212, 630), new string[] { "Certifie sur l'honneur", "Certifie sur ihonneur", "exactitude des renseignements" }));
            Console.WriteLine(p5.ToString());

            PatternPage p6 = new PatternPage(6, cheminModele + "6.tif");
            p6.lesZones.Add(new ZoneVerif(new Rectangle(642, 3, 1134, 441), new string[] { "Informations sur l’apprentissage", "Informations sur l'apprentissage", "Informations sur iapprentissage" }));
            p6.lesZones.Add(new ZoneVerif(new Rectangle(0, 1038, 996, 633), new string[] { "Processus de candidature", "La durée légale du travail", "Le recrutement en licence pro" }));
            Console.WriteLine(p6.ToString());

            PatternPage p7 = new PatternPage(7, cheminModele + "7.tif");
            p7.lesZones.Add(new ZoneVerif(new Rectangle(16, 1390, 632, 306), new string[] {"classement", "discipline"}));
            p7.lesZones.Add(new ZoneVerif(new Rectangle(0, 1956, 576, 298), new string[] {"signature", "fait le", "important : cette fiche"}));
            Console.WriteLine(p7.ToString());

            PatternPage p8 = new PatternPage(8, cheminModele + "8.tif");
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
        static private bool initImage()
        {
            Console.WriteLine("Chargement des images\n");
            lesImages = new List<Image<Gray, byte>>();

            string[] tif = Directory.GetFiles(cheminImage, "*.tif");

            //Vérification du nombre d'image comparé au nombre de pattern
            if (tif.Length < lesImagesZone.Count)
            {
                Console.WriteLine("Le nombre d'image scanné est inférieur au nombre d'image modèle :");
                Console.WriteLine("Image scanné : " + lesImages.Count);
                Console.WriteLine("Image modèle : " + lesImagesZone.Count);
                return false;
            }

            foreach (string fichierImg in tif)
            {
                Console.WriteLine("Chargement de " + fichierImg);
                Image<Gray, byte> img = new Image<Gray, byte>(fichierImg);
                //Vérification de l'image 
                if (!verifImg(fichierImg, img))
                    return false;
            }
            return true;
        }

        //Vérifie que les images chargé sont conformes (redimentionne les images trop grande)
        static bool verifImg(string path, Image<Gray, byte> img)
        {
            //Récupération PPP (Image<Gray, byte> semble passer l'image en 96DPI qq soit sa taille original donc on recharge via un objet C#)
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
                    img = imgTraitement.redimImage(img, tailleImg.Width, tailleImg.Height);
                }
                else
                {
                    Console.WriteLine("Une des images est trop petite (mauvaise qualité) :");
                    Console.WriteLine("Image scanné : " + img.Width + "x" + img.Height);
                    Console.WriteLine("Image modèle : " + tailleImg.Width + "x" + tailleImg.Height);
                    return false;
                }
            }
            lesImages.Add(img);
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

        //Supression des fichiers dans le dossier cheminImage et scan
        static void Scan()
        {
            Console.WriteLine("Début de scan");
            //Suppression des fichiers dans le dossier image
            foreach (String pathF in Directory.GetFiles(cheminImage))
            {
                File.Delete(pathF);
            }

            //Si null, il n'y a pas de scanner disponible
            if(numerisation.deviceID == null)
            {
                Console.WriteLine("\nApplication terminée, impossible d'effectuer un scan, appuyez sur une touche pour fermer");
                Console.ReadKey();
                System.Environment.Exit(1);
            }

            List<Image> lesImagesS = numerisation.Scan();
            int i = 1;

            if(lesImagesS == null)
            {
                return;
            }

            foreach (Image imgS in lesImagesS)
            {
                imgS.Save(cheminImage + i + ".tif");
                i++;
            }
            Console.WriteLine("Fin de scan\n");
        }

        static void Main(string[] args)
        {
            Console.WriteLine("======================================================================================");
            Console.WriteLine("=================================== Initialisation ===================================");
            Console.WriteLine("======================================================================================");
            initVariable();
            //Scan a commenter si pas de scanner
            //Scan();
            creerDossierTemp();
            initImageZone();
            if (!initImage())
            {
                Console.WriteLine("L'application va s'arrêter");
                Console.ReadKey();
                System.Environment.Exit(1);
            }
            
            //Dictionnaire contenant pour chaque pattern, la page numérisé
            Dictionary<PatternPage, Image<Gray, byte>> lesCorrespondances = new Dictionary<PatternPage, Image<Gray, byte>>();

            //Clone des pattern
            List<PatternPage> lesImagesZoneTemp = new List<PatternPage>(lesImagesZone);

            Console.WriteLine("======================================================================================");
            Console.WriteLine("===================================== Recherche ======================================");
            Console.WriteLine("======================================================================================");
            try {
                foreach (Image<Gray, byte> img in lesImages)
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
                
                foreach (PatternPage pattern in lesCorrespondances.Keys)
                {
                    Console.WriteLine("Page Pattern " + pattern.numero + " = Image " + (lesImages.IndexOf(lesCorrespondances[pattern]) + 1));
                }
                Console.WriteLine("\nApplication terminée, appuyez sur une touche pour fermer");
                Console.ReadKey();
            }catch(Exception e)
            {
                Console.WriteLine("\nApplication terminée avec erreur, appuyez sur une touche pour fermer");
                Console.WriteLine(e);
                Console.ReadKey();
            }
            suppressionDossierTemp();
        }
    }
}
