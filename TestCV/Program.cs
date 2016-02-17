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
        public static int numerisationDPI { get; private set; }
        //Classe contenant les méthodes lié à tesseract
        public static TesseractTraitement console { get; private set; }
        //Classe contenant les méthodes lié au traitement d'image (rognage, binarisation,...)
        public readonly static ImageModification imageModification = new ImageModification();
        public readonly static MatModification matModification = new MatModification();
        public readonly static Numerisation numerisation;// = new Numerisation();
        //Liste les images contenu dans le dossier cheminImage
        private static List<Image<Gray, byte>> lesImagesNum;
        //Liste les pattern image (zone, mots à cherchés, numéro de page,...)
        private static List<PageModele> lesPagesModeles;

        static String verifChemin(string chemin)
        {
            if (!chemin[chemin.Length - 1].Equals('\\'))
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
                numerisationDPI = int.Parse(ConfigurationManager.AppSettings["scanDPI"]);
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
                foreach (String pathF in Directory.GetFiles(cheminTemp))
                {
                    File.Delete(pathF);
                }
            }
            else {
                Directory.CreateDirectory(cheminTemp);
            }
        }

        //init des image servant de modèle avec zone et mot à chercher (récupération d'un fichier en JSON)
        static private void initImageModele()
        {
            Console.WriteLine("Initialisation des patterns\n");

            try {
                lesPagesModeles = JsonSerialization.ReadFromJsonFile<List<PageModele>>(cheminModele + "config-Modèle.json");
            }catch(Exception e)
            {
                Console.WriteLine("Erreur lors de la lecture du fichier JSON, le fichier existe-il ? L'application va s'arrêter");
                System.Environment.Exit(1);
            }
            
            foreach(PageModele page in lesPagesModeles)
            {
                page.chargerImage();
                Console.WriteLine(page);
            }

            Console.WriteLine('\n');
        }

        //récupération des image en tif dans le dossier spécifié
        static private bool initImageScan()
        {
            Console.WriteLine("Chargement des images\n");
            lesImagesNum = new List<Image<Gray, byte>>();

            string[] tif = Directory.GetFiles(cheminImage, "*.tif");

            //Vérification du nombre d'image comparé au nombre de pattern
            if (tif.Length < lesPagesModeles.Count)
            {
                Console.WriteLine("Le nombre d'image scanné est inférieur au nombre d'image modèle :");
                Console.WriteLine("Image scanné : " + lesImagesNum.Count);
                Console.WriteLine("Image modèle : " + lesPagesModeles.Count);
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
        static bool verifImg(string chemin, Image<Gray, byte> img)
        {
            //Récupération PPP (Image<Gray, byte> semble passer l'image en 96DPI qq soit sa taille original donc on recharge via un objet C#)
            Bitmap btp = new Bitmap(chemin);
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
                    img = imageModification.redimensionner(img, tailleImg.Width, tailleImg.Height);
                }
                else
                {
                    Console.WriteLine("Une des images est trop petite (mauvaise qualité) :");
                    Console.WriteLine("Image scanné : " + img.Width + "x" + img.Height);
                    Console.WriteLine("Image modèle : " + tailleImg.Width + "x" + tailleImg.Height);
                    return false;
                }
            }
            lesImagesNum.Add(img);
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
        static void EffectuerUneNumerisation()
        {
            Console.WriteLine("Début de la numérisation");
            //Suppression des fichiers dans le dossier image
            foreach (String fichier in Directory.GetFiles(cheminImage))
            {
                File.Delete(fichier);
            }

            //Si null, il n'y a pas de scanner disponible
            if(numerisation.deviceID == null)
            {
                Console.WriteLine("\nApplication terminée, impossible d'effectuer un scan, appuyez sur une touche pour fermer");
                Console.ReadKey();
                System.Environment.Exit(1);
            }

            List<Image> lesImagesNumeriser = numerisation.Scan();
            int i = 1;

            if(lesImagesNumeriser == null)
            {
                return;
            }

            foreach (Image img in lesImagesNumeriser)
            {
                img.Save(cheminImage + i + ".tif");
                i++;
            }
            Console.WriteLine("Fin de numérisation\n");
        }

        static void Main(string[] args)
        {
            Console.WriteLine("======================================================================================");
            Console.WriteLine("=================================== Initialisation ===================================");
            Console.WriteLine("======================================================================================");
            initVariable();
            //Scan a commenter si pas de scanner
            //EffectuerUneNumerisation();
            creerDossierTemp();
            initImageModele();
            if (!initImageScan())
            {
                Console.WriteLine("L'application va s'arrêter");
                Console.ReadKey();
                System.Environment.Exit(1);
            }

            //Dictionnaire contenant pour chaque pattern, la page numérisé
            Dictionary <PageModele, Image<Gray, byte>> lesCorrespondances = new Dictionary<PageModele, Image<Gray, byte>>();

            //Clone des pattern
            List<PageModele> lesImagesZoneTemp = new List<PageModele>(lesPagesModeles);

            Console.WriteLine("======================================================================================");
            Console.WriteLine("===================================== Recherche ======================================");
            Console.WriteLine("======================================================================================");
            try {
                foreach (Image<Gray, byte> img in lesImagesNum)
                {
                    Console.WriteLine("Image " + (lesImagesNum.IndexOf(img) + 1));
                    foreach (PageModele modele in lesImagesZoneTemp)
                    {
                        //Si l'image correspond au pattern, suppression du pattern dans la liste temp
                        if (modele.estPage(img))
                        {
                            lesCorrespondances.Add(modele, img);
                            //Commenté pour voir si le pattern ne correspond pas a plusieurs image
                            lesImagesZoneTemp.Remove(modele);
                            Console.WriteLine("\tPage Pattern " + modele.numero + " = Image " + (lesImagesNum.IndexOf(lesCorrespondances[modele]) + 1));
                            break;
                        }
                    }
                    Console.WriteLine();
                }

                Console.WriteLine("======================================================================================");
                Console.WriteLine("====================================== Résultat ======================================");
                Console.WriteLine("======================================================================================");
                
                foreach (PageModele modele in lesCorrespondances.Keys)
                {
                    Console.WriteLine("Page Modele " + modele.numero + " = Image " + (lesImagesNum.IndexOf(lesCorrespondances[modele]) + 1));
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
