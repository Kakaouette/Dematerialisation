using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Configuration;
using Emgu.CV.Structure;
using Emgu.CV;
using System.Windows.Forms;

namespace Numerisation_GIST
{
    public class Program
    {
        public static PageModele p;
        public static PageModele p2;
        public static String cheminModele { get; private set; }
        public static String cheminImage { get; private set; }
        public static String cheminTmp { get; private set; }
        public static Size tailleImg { get; private set; }
        public static int numerisationDPI { get; private set; }
        public static int tailleCheckBox { get; private set; }
        //Classe contenant les méthodes lié à tesseract
        public static TesseractTraitement tesseract { get; private set; }
        //Classe contenant les méthodes lié au traitement d'image (rognage, binarisation,...)
        public readonly static ImageModification imageModification = new ImageModification();
        public readonly static MatModification matModification = new MatModification();
        public static Numerisation numerisation;
        //Liste les images contenu dans le dossier cheminImage
        private static List<Image<Gray, byte>> lesImagesNum;
        //Liste les pattern image (zone, mots à cherchés, numéro de page,...)
        private static Master M1;
        private static Master M2;
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
            Console.WriteLine("-------------------------------- Variables App.config --------------------------------\n");
            try
            {
                numerisationDPI = int.Parse(ConfigurationManager.AppSettings["numerisationDPI"]);
                tailleCheckBox = int.Parse(ConfigurationManager.AppSettings["tailleCheckBox"]);
                cheminModele = verifChemin(ConfigurationManager.AppSettings["cheminModele"]);
                cheminImage = verifChemin(ConfigurationManager.AppSettings["cheminImage"]);
                cheminTmp = verifChemin(ConfigurationManager.AppSettings["cheminTemp"]);
                tailleImg = new Size(int.Parse(ConfigurationManager.AppSettings["tailleImg.w"]), int.Parse(ConfigurationManager.AppSettings["tailleImg.h"]));
                String tessdata = verifChemin(ConfigurationManager.AppSettings["tessdata"]);
                tesseract = new TesseractTraitement(tessdata);
                //a commenté si pas de scanner
                numerisation = new NumerisationTwain();

                Console.WriteLine("numerisationDPI\t="+ "\t" + numerisationDPI);
                Console.WriteLine("cheminModele\t=" + "\t" + cheminModele);
                Console.WriteLine("cheminImage\t=" + "\t" + cheminImage);
                Console.WriteLine("cheminTemp\t=" + "\t" + cheminTmp);
                Console.WriteLine("tailleImg\t=" + "\t" + tailleImg);
                Console.WriteLine("tessdata\t=" + "\t" + tessdata);
                Console.WriteLine();

                if(numerisationDPI < 240)
                {
                    Console.WriteLine("numerisationDPI doit être égale ou supérieur à 240");
                    throw new Exception();
                }
            }
            catch (System.FormatException e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Erreur lors du chargement de la configuration : scanDPI et tailleImg doivent être des nombres");
                throw new Exception();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Erreur lors du chargement de la configuration, vérifiez le fichier de configuration.");
                throw new Exception();
            }
            
        }

        //Si le dossier Temp existe on le supprime, puis on le créer
        static private void creerDossierTmp()
        {
            if (Directory.Exists(cheminTmp))
            {
                foreach (String pathF in Directory.GetFiles(cheminTmp))
                {
                    File.Delete(pathF);
                }
            }
            else {
                Directory.CreateDirectory(cheminTmp);
            }
        }

        //init des image servant de modèle avec zone et mot à chercher (récupération d'un fichier en JSON)
        static private Master initImageModele(String master)
        {
            Master m = null;
            Console.WriteLine(master);
            try {
                m = JsonSerialization.ReadFromJsonFile<Master>(cheminModele + "config-Modèle-" + master + ".json");
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Erreur le fichier " + cheminModele + "config-Modèle-M1.json" + " n'existe pas !");
                throw new Exception();
            }
            catch (Newtonsoft.Json.JsonSerializationException e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Erreur lors de la lecture du fichier JSON");
                throw new Exception();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Erreur lors de la lecture du fichier JSON");
                throw new Exception();
            }
            
            foreach(PageModele page in m.lesPagesModeles)
            {
                page.chargerImage();
                Console.WriteLine(JsonSerialization.toJSON<PageModele>(page));
                Console.WriteLine();
            }
            
            if (master.Equals("M2"))
            {/*
                p = m.lesPagesModeles[0];
                p.marqueur = new Rectangle(222, 916, 65, 65);
                p.casesACocher = new List<CaseACocher>();
                p.casesACocher.Add(new CaseACocher(new Point(222, 916), "test"));
                p.casesACocher.Add(new CaseACocher(new Point(222, 992), "test"));
                p.casesACocher.Add(new CaseACocher(new Point(222, 1145), "test"));
                p.casesACocher.Add(new CaseACocher(new Point(222, 1278), "test"));

                p.casesACocher.Add(new CaseACocher(new Point(222, 1506), "test"));
                p.casesACocher.Add(new CaseACocher(new Point(222, 1649), "test"));
                p.casesACocher.Add(new CaseACocher(new Point(222, 1792), "test"));
                p.casesACocher.Add(new CaseACocher(new Point(222, 1931), "test"));
                p.casesACocher.Add(new CaseACocher(new Point(222, 2008), "test"));

                p.casesACocher.Add(new CaseACocher(new Point(128, 2080), "test"));
                p.casesACocher.Add(new CaseACocher(new Point(128, 2155), "test"));
                p.casesACocher.Add(new CaseACocher(new Point(128, 2230), "test"));
                p.casesACocher.Add(new CaseACocher(new Point(128, 2306), "test"));
                */
                /* PageModele p2 = m.lesPagesModeles[1];
                 p2.marqueur = new Rectangle(135, 1835, 1845, 1905);
                 PageModele p3 = m.lesPagesModeles[2];
                 p3.marqueur = new Rectangle(135, 1355, 1845, 1425);
                 p3.zoneInfos = new List<ZoneInfo>();
                 p3.zoneInfos.Add(new ZoneInfo(100, 384, 1785, 917, "Tableau : Enseignement supérieur"));
                 p3.rubriques = new List<ZoneInfo>();
                 p3.rubriques.Add(new ZoneInfo(86, 67, 1819, 1246, "Cursus"));
                 p3.rubriques.Add(new ZoneInfo(73, 1312, 1860, 1000, "Compétences linguistiques"));

                 PageModele p4 = m.lesPagesModeles[3];
                 p4.marqueur = new Rectangle(135, 115, 1845, 190);
                 p4.zoneInfos = new List<ZoneInfo>();
                 p4.zoneInfos.Add(new ZoneInfo(118, 274, 1763, 503, "Projet professionel"));
                 p4.rubriques = new List<ZoneInfo>();
                 p4.rubriques.Add(new ZoneInfo(104, 197, 1789, 2528, "Projet professionel et expériences"));

                 PageModele p5 = m.lesPagesModeles[4];
                 //A modifier ! on trouve pas ce qu'il faut comme marqueur
                 p5.marqueur = new Rectangle(130, 115, 335, 335);
                 p5.zoneInfos = new List<ZoneInfo>();
                 p5.zoneInfos.Add(new ZoneInfo(118, 702, 1759, 2003, "Exposé des motivations personnelles et du projet d'études"));
                 p5.rubriques = new List<ZoneInfo>();
                 p5.rubriques.Add(new ZoneInfo(114, 83, 1767, 2798, "Projet professionel et expériences"));

                 PageModele p6 = m.lesPagesModeles[5];
                 p6.marqueur = new Rectangle(325, 1510, 1685, 1700);

                 PageModele p7 = m.lesPagesModeles[6];
                 p7.marqueur = new Rectangle(135, 115, 1845, 330);
                 p7.zoneInfos = new List<ZoneInfo>();
                 p7.zoneInfos.Add(new ZoneInfo(312, 350, 1737, 83, "Mention"));
                 p7.zoneInfos.Add(new ZoneInfo(326, 428, 1533, 79, "Spécialité"));
                 p7.zoneInfos.Add(new ZoneInfo(314, 506, 1541, 79, "Parcours"));
                 p7.zoneInfos.Add(new ZoneInfo(128, 694, 1709, 93, "Nom et Prénom de l'étudiant"));
                 p7.zoneInfos.Add(new ZoneInfo(126, 872, 1725, 99, "Établissement fréquenté"));
                 p7.rubriques = new List<ZoneInfo>();
                 p7.rubriques.Add(new ZoneInfo(104, 351, 1789, 1967, "Fiche confidentielle d'appréciation"));

                 PageModele p8 = m.lesPagesModeles[7];
                 p8.marqueur = new Rectangle(135, 1620, 1845, 1695);*/

                JsonSerialization.WriteToJsonFile<Master>(cheminModele + "config-Modèle-" + master + ".json", m, false);
                
            }
            
            Console.WriteLine('\n');
            return m;
        }

        //récupération des image en tif dans le dossier spécifié
        static private bool initImageScan()
        {
            Console.WriteLine("---------------------------------- Images numérisées ---------------------------------\n");
            lesImagesNum = new List<Image<Gray, byte>>();

            string[] tif = Directory.GetFiles(cheminImage, "*.tif");

            foreach (string fichierImg in tif)
            {
                Console.WriteLine(fichierImg);
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

        static void suppressionDossierTmp()
        {
            foreach(String pathF in Directory.GetFiles(cheminTmp))
            {
                File.Delete(pathF);
            }
            Directory.Delete(cheminTmp);
        }

        //Supression des fichiers dans le dossier cheminImage et scan
        static void EffectuerUneNumerisation()
        {
            Console.WriteLine("------------------------------------ Numérisation ------------------------------------\n");
            //Suppression des fichiers dans le dossier image
            foreach (String fichier in Directory.GetFiles(cheminImage))
            {
                File.Delete(fichier);
            }
            
            //Si null, il n'y a pas de scanner disponible
            if(!numerisation.ready)
            {
                Console.WriteLine("Impossible d'effectuer un scan car il n'y a aucun scanner qui peut effectuer une numérisation");
                throw new Exception();
            }

            bool continueNum = true;
            List<Image> lesImagesNumeriser = new List<Image>();
            while (continueNum)
            {
                lesImagesNumeriser.AddRange(numerisation.Scan());
                bool readKey = false;

                while (!readKey)
                {
                    Console.WriteLine("Continuer la numérisation ? (Répondez 'o' ou 'n')");
                    Char rep = Console.ReadKey().KeyChar;

                    if (rep.Equals('o'))
                    {
                        Console.WriteLine("Début d'une nouvelle numérisation");
                        readKey = true;
                    }
                    else if (rep.Equals('n'))
                    {
                        Console.WriteLine("Fin de la numérisation");
                        continueNum = false;
                        readKey = true;
                    }
                    Console.WriteLine();
                }
            }
            int i = 1;

            if(lesImagesNumeriser == null)
            {
                return;
            }

            foreach (Image img in lesImagesNumeriser)
            {
                Console.WriteLine("Enregistrement de " + i + " dans " + cheminImage + i + ".tif");
                img.Save(cheminTmp + i + ".tif");
                i++;
            }
        }

        [STAThread]
        static void Main(string[] args)
        {
            Application.Run(new MainWindow());
            /*
            try
            {

                Console.WriteLine("======================================================================================");
                Console.WriteLine("=================================== Initialisation ===================================");
                Console.WriteLine("======================================================================================");
                initVariable();
                //a commenter si pas de scanner
                //EffectuerUneNumerisation();
                creerDossierTmp();
                //Chargement des images modèle du master 1 et 2
                Console.WriteLine("---------------------------- Modèles dans le fichier JSON ----------------------------\n");
                M1 = initImageModele("M1");
                M2 = initImageModele("M2");
                
                if (!initImageScan())
                {
                    Console.WriteLine("L'application va s'arrêter");
                    throw new Exception();
                }

                Marqueurs marq = new Marqueurs();
                M2.lesPagesModeles[0].image.Save("..\\..\\Include\\IMG\\huehue.tif");
                //string s = "Parcours de test Parcours qui fait des Parcours";
                
                //List<CaseACocher> lesCases = marq.patternFinding(lesImagesNum[7], M2.lesPagesModeles[0]);
                //List<CaseACocher> lesCases = marq.patternFinding(p.image, M2.lesPagesModeles[0]);
                //matModification.CaseCoche(p, lesCases, lesImagesNum[7]);


                //Dictionnaire contenant pour chaque pattern, la page numérisé
                Dictionary <PageModele, Image<Gray, byte>> lesCorrespondances = new Dictionary<PageModele, Image<Gray, byte>>();

                //Clone des pattern
                List<Image<Gray, Byte>> lesImagesNumTmp = new List<Image<Gray, Byte>>(lesImagesNum);
                Console.WriteLine();
                Console.WriteLine("======================================================================================");
                Console.WriteLine("===================================== Recherche ======================================");
                Console.WriteLine("======================================================================================");
                Console.WriteLine("----------------------------------- Année de Master ----------------------------------");
                if (M1.estMaster(lesImagesNum))
                {
                    Console.WriteLine("Les images scanné sont correspondent au Master 1");
                    lesPagesModeles = M1.lesPagesModeles;
                }else if (M2.estMaster(lesImagesNum))
                {
                    Console.WriteLine("Les images scanné sont correspondent au Master 2");
                    lesPagesModeles = M2.lesPagesModeles;
                }
                else
                {
                    bool readKey = false;

                    while (!readKey)
                    {
                        Console.WriteLine("Le master n'a pas pu être déterminé ! Est-ce le master 1 ou 2 ? (répondez '1' ou '2')");
                        Char rep = Console.ReadKey().KeyChar;

                        if (rep.Equals('1'))
                        {
                            Console.WriteLine("Les images scanné sont correspondent au Master 1");
                            lesPagesModeles = M1.lesPagesModeles;
                            readKey = true;
                        }
                        else if (rep.Equals('2'))
                        {
                            Console.WriteLine("Les images scanné sont correspondent au Master 2");
                            lesPagesModeles = M2.lesPagesModeles;
                            readKey = true;
                        }
                        Console.WriteLine();
                    }
                 }

                //Vérification du nombre d'image comparé au nombre de modèle

                if (lesImagesNum.Count < lesPagesModeles.Count)
                {
                    Console.WriteLine("Le nombre d'image scanné est inférieur au nombre d'image modèle :");
                    Console.WriteLine("Image scanné : " + lesImagesNum.Count);
                    Console.WriteLine("Image modèle : " + lesPagesModeles.Count);
                    throw new ArgumentException();
                }

                foreach (PageModele modele in lesPagesModeles)
                {
                    Console.WriteLine("-------------------------------------- Modèle " + modele.numero + " --------------------------------------");
                    foreach (Image<Gray, Byte> img in lesImagesNumTmp)
                    {
                        Console.Write("Image " + (lesImagesNum.IndexOf(img)+1) + "\t");
                        //Si l'image correspond au pattern, suppression du pattern dans la liste temp
                        if (modele.estPage(img))
                        {
                            lesCorrespondances.Add(modele, img);
                            //Commenté pour voir si le pattern ne correspond pas a plusieurs image
                            lesImagesNumTmp.Remove(img);
                            Console.WriteLine("\tCorrespond");
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
                    Console.WriteLine("Modèle " + modele.numero + "\t=\tImage " + (lesImagesNum.IndexOf(lesCorrespondances[modele]) + 1));
                }

                if(lesCorrespondances.Keys.Count != lesPagesModeles.Count)
                {
                    String s = "Les modeles ";
                    foreach(PageModele modele in lesPagesModeles)
                    {
                        if (!lesCorrespondances.ContainsKey(modele))
                        {
                            s += modele.numero + ", ";
                        }
                    }
                    s = s.Substring(0, s.Length - 2);
                    Console.WriteLine(s + " n'ont pas de correspondance avec les image numérisées");
                }

                Console.WriteLine("\nApplication terminée, appuyez sur une touche pour fermer");
                Console.ReadKey();
            }
            catch(Exception e)
            {
                Console.WriteLine("\nApplication terminée avec erreur, appuyez sur une touche pour fermer");
                Console.WriteLine(e);
                Console.ReadKey();
            }
            //suppressionDossierTmp();
        */
        }
    }
}
