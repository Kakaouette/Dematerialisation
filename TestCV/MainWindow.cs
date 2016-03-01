using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Numerisation_GIST
{

    public partial class MainWindow : Form
    {
        public static String cheminModele { get; private set; }
        public static String cheminImage { get; private set; }
        public static String cheminTmp { get; private set; }
        public static Size tailleImg { get; private set; }
        public static int numerisationDPI { get; private set; }
        //Classe contenant les méthodes lié à tesseract
        public static TesseractTraitement tesseract { get; private set; }
        //Liste les pattern image (zone, mots à cherchés, numéro de page,...)
        private static Master M1;
        private static Master M2;

        //Liste les images contenu dans le dossier cheminImage
        private static List<Image<Gray, byte>> lesImagesNum;

        //Classe contenant les méthodes lié au traitement d'image (rognage, binarisation,...)
        public readonly static ImageModification imageModification = new ImageModification();

        private static List<PageModele> lesPagesModeles;

        private static bool init = false;


        //Dictionnaire contenant pour chaque pattern, la page numérisé
        Dictionary<PageModele, Image<Gray, byte>> lesCorrespondances;

        public MainWindow()
        {
            InitializeComponent();

            in_DPI.SelectedItem = "300";

            if (File.Exists("config.xml"))
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(SaveConfig));
                Stream stream = new FileStream("config.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
                SaveConfig config = ((SaveConfig)serializer.Deserialize(stream));
                in_cheminTess.Text = config.tessdata;
                in_cheminModeles.Text = config.cheminModele;
                in_cheminImages.Text = config.cheminImage;
                in_cheminTemp.Text = config.cheminTemp;
                in_tailleImgX.Value = config.tailleImgW;
                in_tailleImgY.Value = config.tailleImgH;
                in_DPI.SelectedItem = config.numerisationDPI;
                stream.Close();
            }
        }


        #region Update toolstrip
        public delegate void updateToolStripDelegate(string category, string info);
        
        public void updateToolStrip(string category, string info)
        {
            toolStrip_Category.Text = category;
            toolStrip_Info.Text = info;
        }

        private void updateInfoTraitement(object sender, String a, String b)
        {
            this.Invoke(new updateToolStripDelegate(this.updateToolStrip), new object[] {a, b});
        }
        #endregion

        #region Update buttons
        public delegate void updateButtonsDelegate(Boolean valide);

        public void updateButtons(Boolean valide)
        {
            if (valide)
            {
                btn_execute.Enabled = true;
            }
            if (!valide)
            {
                btn_execute.Enabled = false;
            }
            foreach (Button btn in this.groupBox1.Controls.OfType<Button>()) { 
                if (valide)
                {
                    btn.Enabled = true;
                }
                if (!valide)
                {
                    btn.Enabled = false;
                }
            }
        }

        private void updateButtonsTraitement(object sender, Boolean valide)
        {
            this.Invoke(new updateButtonsDelegate(this.updateButtons), new object[] { valide });
        }
        #endregion

        #region Update buttons
        public delegate void updateResultatDelegate(String result);

        public void updateResultat(String result)
        {
            txb_resultat.Text = result;
        }

        private void updateResultatTraitement(object sender, String result)
        {
            this.Invoke(new updateResultatDelegate(this.updateResultat), new object[] { result });
        }
        #endregion
        
        #region Update buttons
        public delegate void updateTreeDelegate(String treeString);

        public void updateTree(String treeString)
        {
            //
            // This is the first node in the view.
            //
            TreeNode treeNode = new TreeNode("Windows");
            tree_result.Nodes.Add(treeNode);
            //
            // Another node following the first node.
            //
            treeNode = new TreeNode("Linux");
            tree_result.Nodes.Add(treeNode);
            //
            // Create two child nodes and put them in an array.
            // ... Add the third node, and specify these as its children.
            //
            TreeNode node2 = new TreeNode("C#");
            TreeNode node3 = new TreeNode("VB.NET");
            TreeNode[] array = new TreeNode[] { node2, node3 };
            //
            // Final node.
            //
            treeNode = new TreeNode("Dot Net Perls", array);
            tree_result.Nodes.Add(treeNode);

            // tNode = treeView1.Nodes.Add("Websites") ;
            // treeView1.Nodes[0].Nodes.Add("Net-informations.com");
        }

        private void updateTreeDelegateTraitement(object sender, String treeString)
        {
            this.Invoke(new updateTreeDelegate(this.updateTree), new object[] { treeString });
        }
        #endregion

        private void btn_execute_Click(object sender, EventArgs e)
        {
            dpi = Int32.Parse((String)in_DPI.SelectedItem);
            Thread executeThread = new Thread(threadExecute);                       // Create thread to receive client messages
            executeThread.Start();
        }

        private void btn_initialiser_Click(object sender, EventArgs e)
        {
            dpi = Int32.Parse((String)in_DPI.SelectedItem);
            Thread initThread = new Thread(threadInit);                       // Create thread to receive client messages
            initThread.Start();
        }

        private void threadInit()
        {
            try
            {
                updateButtonsTraitement(new object(), false);

                updateInfoTraitement(this, "Initialisation : ", "");

                btn_initValider_Click(new object(), new EventArgs());

                //a commenter si pas de scanner
                //!!! EffectuerUneNumerisation();

                // Chargement des images modèles du master 1 et 2            
                M1 = initImageModele("M1");
                M2 = initImageModele("M2");

                initImageScan();

                updateInfoTraitement(this, "Initialisation terminée - ", "Succès !");
                init = true;
            }
            catch (Exception ex)
            {
                updateInfoTraitement(this, "Initialisation terminée - ", "Echec...");
                updateResultatTraitement(this, ex.ToString());
            }
            finally
            {
                updateButtonsTraitement(new object(), true);
            }
        }

        int dpi = 300;

        private void threadExecute()
        {
            try
            {
                if (!init)
                {
                    threadInit();
                }

                updateButtonsTraitement(new object(), false);

                btn_initValider_Click(new object(), new EventArgs());

                updateInfoTraitement(new object(), "Recherche - Année de Master : ", "");
                
                //Dictionnaire contenant pour chaque pattern, la page numérisé
                lesCorrespondances = new Dictionary<PageModele, Image<Gray, byte>>();

                //Clone des pattern
                List<Image<Gray, Byte>> lesImagesNumTmp = new List<Image<Gray, Byte>>(lesImagesNum);

                updateInfoTraitement(this, "Recherche - Année de Master : ", "Test...");
                if (M1.estMaster(lesImagesNum))
                {
                    updateInfoTraitement(this, "Recherche - Année de Master : ", "Les images scannées correspondent au Master 1");
                    lesPagesModeles = M1.lesPagesModeles;
                }
                else if (M2.estMaster(lesImagesNum))
                {
                    updateInfoTraitement(this, "Recherche - Année de Master : ", "Les images scannées correspondent au Master 2");
                    lesPagesModeles = M2.lesPagesModeles; 
                }
                else
                {
                    CustomMessageBox cmb = new CustomMessageBox("Aidez-moi !",
                        "Le master n'a pas pu être déterminé ! Est-ce le master 1 ou 2 ?",
                        "Master 1", "Master 2");
                    DialogResult dr = cmb.ShowDialog();

                    if (dr.Equals(DialogResult.Yes))
                    {
                        updateInfoTraitement(this, "Recherche - Année de Master : ", "Les images scannées correspondent au Master 1");
                        lesPagesModeles = M1.lesPagesModeles;
                    }
                    else if(dr.Equals(DialogResult.No))
                    {
                        updateInfoTraitement(this, "Recherche - Année de Master : ", "Les images scannées correspondent au Master 2");
                        lesPagesModeles = M2.lesPagesModeles;
                    }
                }

                updateInfoTraitement(this, "Recherche - Vérification modèles : ", "");
                // Vérification du nombre d'images comparé au nombre de modèles
                if (lesImagesNum.Count < lesPagesModeles.Count)
                {
                    updateInfoTraitement(this, "", 
                        "Le nombre d'images scannées est inférieur au nombre d'images modèles : "
                        + "Scans : " + lesImagesNum.Count + " - "
                        + "Modèles : " + lesPagesModeles.Count);
                    updateButtonsTraitement(new object(), true);
                    return;
                }

                foreach (PageModele modele in lesPagesModeles)
                {
                    foreach (Image<Gray, Byte> img in lesImagesNumTmp)
                    {
                        updateInfoTraitement(this, "Recherche - Vérification modèles : ", 
                            "Modèle " + modele.numero + " - Test sur image " + (lesImagesNum.IndexOf(img) + 1));
                        // Si l'image correspond au pattern, suppression du pattern dans la liste temp
                        if (modele.estPage(img))
                        {
                            lesCorrespondances.Add(modele, img);
                            // Commenté pour voir si le pattern ne correspond pas a plusieurs image
                            lesImagesNumTmp.Remove(img);
                            updateInfoTraitement(this, "Recherche - Vérification modèles : ", 
                                "Modèle " + modele.numero + " - Test sur image " + (lesImagesNum.IndexOf(img) + 1) + " [Correspondance]");
                            break;
                        }
                    }
                }


                updateInfoTraitement(this, "Résultats ...", "");

                String resultat = "";
                foreach (PageModele modele in lesCorrespondances.Keys)
                {
                    resultat += "Modèle " + modele.numero + "\t=\tImage " + (lesImagesNum.IndexOf(lesCorrespondances[modele]) + 1) + Environment.NewLine;
                }

                if (lesCorrespondances.Keys.Count != lesPagesModeles.Count)
                {
                    String str = "Les modeles ";
                    foreach (PageModele modele in lesPagesModeles)
                    {
                        if (!lesCorrespondances.ContainsKey(modele))
                        {
                            str += modele.numero + ", ";
                        }
                    }
                    str = str.Substring(0, str.Length - 2);
                    resultat = str + " n'ont pas de correspondance avec les images numérisées\n";
                }

                suppressionDossierTmp();
                updateResultatTraitement(this, resultat);
                updateInfoTraitement(this, "Traitement terminé.", "");
            }
            catch (Exception ex)
            {
                updateInfoTraitement(this, "Echec... ", "Une erreur est survenue");
                updateResultatTraitement(this, ex.ToString());
            }
            finally
            {
                updateButtonsTraitement(new object(), true);
            }
        }

        static void suppressionDossierTmp()
        {
            foreach (String pathF in Directory.GetFiles(cheminTmp))
            {
                File.Delete(pathF);
            }
            Directory.Delete(cheminTmp);
        }

        /// <summary>
        /// Vérifie les informations d'initialisation et informe des erreurs.
        /// </summary>
        private void btn_initValider_Click(object sender, EventArgs e)
        {    
            updateInfoTraitement(this, "Initialisation : ", "Vérification dossiers...");
            
            lbl_initError.Text = "";

            if (!Directory.Exists(in_cheminModeles.Text))
            {
                lbl_initError.Text += "Le dossier de modeles n'existe pas. ";
            }
            if (!Directory.Exists(in_cheminImages.Text))
            {
                lbl_initError.Text += "Le dossier d'images n'existe pas. ";
            } 
            if (Directory.Exists(in_cheminTemp.Text))
            {
                foreach (String pathF in Directory.GetFiles(in_cheminTemp.Text))
                {
                    File.Delete(pathF);
                }
            }
            else
            {
                Directory.CreateDirectory(in_cheminTemp.Text);
            }
            if (!Directory.Exists(in_cheminTess.Text))
            {
                lbl_initError.Text += "Le dossier de tesseract n'existe pas. ";
            }

            if (lbl_initError.Text == "")
            {
                try
                {
                    updateInfoTraitement(this, "Initialisation : ", "Initialisation valeurs ...");
                    numerisationDPI = dpi;
                    cheminModele = in_cheminModeles.Text;
                    cheminImage = in_cheminImages.Text;
                    cheminTmp = in_cheminTemp.Text;
                    tailleImg = new Size((int)in_tailleImgX.Value, (int)in_tailleImgY.Value);
                    updateInfoTraitement(this, "Initialisation : ", "Initialisation Tesseract ...");
                    String tessdata = in_cheminTess.Text;
                    tesseract = new TesseractTraitement(tessdata);

                    updateInfoTraitement(this, "Initialisation : ", "Configuration validée.");
                }
                catch (Exception ex)
                {
                    throw new Exception("Erreur lors du chargement de la configuration, vérifiez le fichier de configuration.", ex);
                }
            }
            else if (!((Button)sender).Name.Equals("btn_initValider"))
            {
                throw new Exception("Erreur lors du chargement de la configuration, vérifiez le fichier de configuration.");
            }
        }

        /// <summary>
        /// Initialisation des images servant de modèle avec zone et mot à chercher.
        /// (Récupération d'un fichier en JSON)
        /// </summary>
        /// <param name="master"></param>
        /// <returns></returns>
        private Master initImageModele(String master)
        {
            Master m = null;

            updateInfoTraitement(this, "Initialisation - Image Modele \"" + master + "\" : ", "Lecture JSON ...");
            try
            {
                m = JsonSerialization.ReadFromJsonFile<Master>(cheminModele + "config-Modèle-" + master + ".json");
            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException)
                {
                    throw new FileNotFoundException("Erreur le fichier " + cheminModele + "config-Modèle-M1.json" + " n'existe pas !", ex);
                }
                else if (ex is Newtonsoft.Json.JsonSerializationException)
                {
                    throw new Newtonsoft.Json.JsonSerializationException("Erreur lors de la lecture du fichier JSON", ex);
                }
                else
                {
                    throw new Exception("Erreur lors de la lecture du fichier JSON", ex);
                }
            }

            foreach (PageModele page in m.lesPagesModeles)
            {
                page.chargerImage();
                updateInfoTraitement(this, "Initialisation - Image Modele \"" + master + "\" : ", JsonSerialization.toJSON<PageModele>(page) + " ...");
            }

            if (master.Equals("M1"))
            {
                PageModele p = m.lesPagesModeles[0];
                p.zoneInfos = new List<ZoneInfo>();
                p.zoneInfos.Add(new ZoneInfo("Nom et prénom", "Prénom", 60, 0));
                p.zoneInfos.Add(new ZoneInfo("Numéro INE", "baccalau", 60, -45));
                p.zoneInfos.Add(new ZoneInfo("2eme Voeu : Mention", "Indiquer", 60, 75));
                p.zoneInfos.Add(new ZoneInfo("2eme Voeu : Parcours", "Indiquer", 60, 145));

                p.casesACocher = new List<ZoneInfo>();
                p.casesACocher.Add(new ZoneInfo("Génie biotechnologie", "Parcours G", 60, 0));
                p.casesACocher.Add(new ZoneInfo("Biochimie", "Parcours B", 60, 0));
                p.casesACocher.Add(new ZoneInfo("Ingénierie & co", "Parcours I", 60, 0));

                p.rubriques = new List<ZoneInfo>();
                p.rubriques.Add(new ZoneInfo("Informations étudiant", "Prénom", 220, 0));
                p.rubriques.Add(new ZoneInfo("1er Voeu", "Cocher", 1470, 0));
                p.rubriques.Add(new ZoneInfo("2eme Voeu", "Indiquer", 225, 0));

                PageModele p2 = m.lesPagesModeles[1];
                p2.zoneInfos = new List<ZoneInfo>();
                p2.zoneInfos.Add(new ZoneInfo("Nom", "inutile", 130, 0));
                p2.zoneInfos.Add(new ZoneInfo("Prénom", "Prénom", 60, 0));
                p2.zoneInfos.Add(new ZoneInfo("Numéro INE", "Marital", 60, 70));
                p2.zoneInfos.Add(new ZoneInfo("Nationalité", "Nationalité", 60, 0));
                p2.zoneInfos.Add(new ZoneInfo("Date et lieu de naissance", "naissance", 60, 0));
                p2.zoneInfos.Add(new ZoneInfo("Pays", "pays", 60, 0));
                p2.zoneInfos.Add(new ZoneInfo("Adresse", "Adresse", 140, 0));
                p2.zoneInfos.Add(new ZoneInfo("Code Postal et Ville", "postal", 60, 0));
                p2.zoneInfos.Add(new ZoneInfo("Pays", "postal", 60, 70));
                p2.zoneInfos.Add(new ZoneInfo("Email", "mail", 60, 0));

                p2.casesACocher = new List<ZoneInfo>();

                p2.rubriques = new List<ZoneInfo>();
                p2.rubriques.Add(new ZoneInfo("Informations générales", "inutile", 1700, -110));
                p2.rubriques.Add(new ZoneInfo("Cursus", "enseignement", 800, -160));

                PageModele p3 = m.lesPagesModeles[2];
                p3.zoneInfos = new List<ZoneInfo>();

                p3.casesACocher = new List<ZoneInfo>();
            
                p3.rubriques = new List<ZoneInfo>();
                p3.rubriques.Add(new ZoneInfo("Enseignement supérieur", "officiel", 1200, -110));
                p3.rubriques.Add(new ZoneInfo("Compétences linguistiques", "cadre", 950, -110));

                PageModele p4 = m.lesPagesModeles[3];
                p4.zoneInfos = new List<ZoneInfo>();
                p4.zoneInfos.Add(new ZoneInfo("Projet professionnel", "fessionnel", 560, 0));

                p4.casesACocher = new List<ZoneInfo>();

                p4.rubriques = new List<ZoneInfo>();
                p4.rubriques.Add(new ZoneInfo("Projet professionnel", "fessionnel", 560, 0));
                p4.rubriques.Add(new ZoneInfo("Emplois / Stages", "Dernière", 1950, -90));

                PageModele p5 = m.lesPagesModeles[4];
                p5.zoneInfos = new List<ZoneInfo>();
                p5.zoneInfos.Add(new ZoneInfo("Exposé des motivations", "motivation", 1350, 0));
                p5.zoneInfos.Add(new ZoneInfo("Soussigné", "soussigné", 60, 0));
                p5.zoneInfos.Add(new ZoneInfo("Fait à...", "Fait", 60, 0));
                p5.zoneInfos.Add(new ZoneInfo("Signature", "Signature", 550, 0));

                p5.casesACocher = new List<ZoneInfo>();

                p5.rubriques = new List<ZoneInfo>();
                p5.rubriques.Add(new ZoneInfo("Entreprise apprentissage", "apprentissage", 460, 0));
                p5.rubriques.Add(new ZoneInfo("Motivations", "motivation", 2200, 0));

                PageModele p6 = m.lesPagesModeles[5];
                p6.zoneInfos = new List<ZoneInfo>();
                p6.casesACocher = new List<ZoneInfo>();
                p6.rubriques = new List<ZoneInfo>();

                PageModele p7 = m.lesPagesModeles[6];
                p7.zoneInfos = new List<ZoneInfo>();
                p7.zoneInfos.Add(new ZoneInfo("Projet professionnel", "fessionnel", 560, 0));

                p7.casesACocher = new List<ZoneInfo>();

                p7.rubriques = new List<ZoneInfo>();
                p7.rubriques.Add(new ZoneInfo("Projet professionnel", "fessionnel", 560, 0));
                p7.rubriques.Add(new ZoneInfo("Emplois / Stages", "Dernière", 1950, -90));

                PageModele p8 = m.lesPagesModeles[7];
                p8.zoneInfos = new List<ZoneInfo>();
                p8.casesACocher = new List<ZoneInfo>();
                p8.rubriques = new List<ZoneInfo>();
                p8.rubriques.Add(new ZoneInfo("Rappel infos étudiant", "Mention", 620, 0));
                p8.rubriques.Add(new ZoneInfo("Personne recommandant", "recommandant", 270, 0));
                p8.rubriques.Add(new ZoneInfo("Appréciation", "DISCIPLINE", 600, 0));
                p8.rubriques.Add(new ZoneInfo("Signature", "Fait le", 200, 0));

                JsonSerialization.WriteToJsonFile<Master>(cheminModele + "config-Modèle-" + master + ".json", m, false);
                
            }

            return m;
        }

        /// <summary>
        /// Récupération des image en tif dans le dossier spécifié
        /// </summary>
        private void initImageScan()
        {
            updateInfoTraitement(this, "Initialisation - Récupération des images : ", "");
            lesImagesNum = new List<Image<Gray, byte>>();

            string[] tif = Directory.GetFiles(cheminImage, "*.tif");

            int noTif = 0;

            foreach (string fichierImg in tif)
            {
                updateInfoTraitement(this, "Initialisation - Récupération des images : ", "[" + noTif + "/" + tif.Length + "] - " 
                    + fichierImg + " ...");
                Image<Gray, byte> img = new Image<Gray, byte>(fichierImg);
                //Vérification de l'image 
                try
                {
                    verifImg(fichierImg, img);
                }
                catch (Exception e)
                {
                    throw e;
                }
                noTif++;
            }
        }

        /// <summary>
        /// Vérifie que les images chargé sont conformes :
        /// Redimensionne les images trop grandes.
        /// </summary>
        /// <param name="chemin"></param>
        /// <param name="img"></param>
        /// <returns></returns>
        static private void verifImg(string chemin, Image<Gray, byte> img)
        {
            //Récupération PPP (Image<Gray, byte> semble passer l'image en 96DPI qq soit sa taille original donc on recharge via un objet C#)
            Bitmap btp = new Bitmap(chemin);
            if (btp.HorizontalResolution < 240 || btp.VerticalResolution < 240)
            {
                throw new Exception("Le PPP d'une image est inférieur à 240 : " 
                    + "Scan " + btp.HorizontalResolution + "x" + btp.VerticalResolution);
            }

            //Libération du bitmap
            btp.Dispose();

            //Vérification de la taille en pixel
            if (!img.Size.Equals(tailleImg))
            {
                if ((img.Size.Height > (tailleImg.Height - 25)) && (img.Size.Width > (tailleImg.Width - 25)))
                {
                    img = imageModification.redimensionner(img, tailleImg.Width, tailleImg.Height);
                }
                else
                {
                    throw new Exception("Une des images est trop petite (mauvaise qualité) : " 
                        + "Scan " + img.Width + "x" + img.Height + " - "
                        + "Modèle " + tailleImg.Width + "x" + tailleImg.Height);
                }
            }

            lesImagesNum.Add(img);
        }

        private void btn_sauvegarder_Click(object sender, EventArgs e)
        {
            updateInfoTraitement(this, "", "Enregistrement...");
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(SaveConfig));
            Stream stream = new FileStream("config.xml", FileMode.Create, FileAccess.Write, FileShare.None);
            serializer.Serialize(stream, 
                new SaveConfig(in_cheminTess.Text, in_cheminModeles.Text, in_cheminImages.Text, in_cheminTemp.Text, 
                (int)in_tailleImgX.Value, (int)in_tailleImgY.Value, Int32.Parse((String)in_DPI.SelectedItem)));
            stream.Close();
            updateInfoTraitement(this, "", "Enregistré avec succès");
        }

        private void in_numeriser_CheckedChanged(object sender, EventArgs e)
        {
            if(in_numeriser.CheckState == CheckState.Checked)
            {
                in_numeriserOption.Enabled = true;
            }
            else if (in_numeriser.CheckState == CheckState.Unchecked)
            {
                in_numeriserOption.Enabled = false;
            }
        }

        private void btn_chemins_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult dr;
            switch (((Button)sender).Name)
            {
                case "btn_cheminImages":
                    fbd.SelectedPath = in_cheminImages.Text;
                    dr = fbd.ShowDialog();
                    if(dr == DialogResult.OK)
                        in_cheminImages.Text = fbd.SelectedPath;
                    break;
                case "btn_cheminModeles":
                    fbd.SelectedPath = in_cheminModeles.Text;
                    dr = fbd.ShowDialog();
                    if (dr == DialogResult.OK)
                        in_cheminModeles.Text = fbd.SelectedPath;
                    break;
                case "btn_cheminTemp":
                    fbd.SelectedPath = in_cheminTemp.Text;
                    dr = fbd.ShowDialog();
                    if (dr == DialogResult.OK)
                        in_cheminTemp.Text = fbd.SelectedPath;
                    break;
                case "btn_cheminTess":
                    fbd.SelectedPath = in_cheminTess.Text;
                    dr = fbd.ShowDialog();
                    if (dr == DialogResult.OK)
                        in_cheminTess.Text = fbd.SelectedPath;
                    break;
                default:
                    break;
            }
        }
    }
}
