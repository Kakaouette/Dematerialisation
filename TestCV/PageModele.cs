using Emgu.CV;
using Emgu.CV.Structure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Numerisation_GIST
{
    //Représente un pattern de page avec son numéro de page et une liste de zone
    //Contient toutes les méthodes permettant de vérifié qu'une image (Objet Mat) appartient au pattern
    public class PageModele
    {
        [JsonRequired]
        public int numero { get; set; }

        [JsonRequired]
        public List<ZoneTexte> lesZonesTextuelle { get; set; }

        [JsonRequired]
        public string cheminImage { get; set; }

        [JsonIgnore]
        public Image<Gray, byte> image { get; private set; }

        public PageModele()
        {
            this.numero = 0;
            this.lesZonesTextuelle = null;
            this.image = null;
        }

        public PageModele(int numero, List<ZoneTexte> lesZones, string file)
        {
            this.numero = numero;
            this.lesZonesTextuelle = lesZones;
            this.image = new Image<Gray, byte>(file);
            this.cheminImage = file;
        }

        public PageModele(int numero, string file)
        {
            this.numero = numero;
            this.lesZonesTextuelle = new List<ZoneTexte>();
            this.image = new Image<Gray, byte>(file);
            this.cheminImage = file;
        }

        //Vérifie qu'un des mot est présent dans la chaîne, si oui renvoi true, sinon false
        bool textePresent(String chaine, String[] mots)
        {
            //Clonage dans une variable de la chaine en minuscule
            string chaineL = ((String)chaine.Clone()).ToLower();

            foreach (string mot in mots)
            {
                if (chaineL.Contains(mot.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }

        //vérifie que la page en paramètre appartient au pattern (true si oui)
        public bool estPage(Image<Gray, byte> img)
        {
            //Passage en binaire
            Image<Gray, byte> imgBin = Program.imageModification.convertionBinaire(img);
            int i = 1;
            foreach (ZoneTexte z in this.lesZonesTextuelle)
            {
                //découpe
                Image<Gray, byte> imgR = Program.imageModification.rogner(imgBin, z.zone);
                //tesseract
                String texteTesseract = Program.tesseract.tesseractAnalyse(imgR);
                //Si le texte reconnu par tesseract ne correspond pas à celui de la zone, return false
                if (!textePresent(texteTesseract, z.mots))
                {
                    Console.WriteLine("\tPas de correspondance avec la zone " + i);
                    return false;
                }
                i++;
                imgR.Dispose();
            }
            //Si tous les tests de zone ont renvoyé vrai on renvoi true
            imgBin.Dispose();
            return true;
        }

        public void chargerImage()
        {
            this.image = new Image<Gray, Byte>(this.cheminImage);
        }

        override public string ToString()
        {
            String s = "PageModele{numero=" + numero + ", chemin=" + cheminImage + ", lesZonesTextuelle=[";
            foreach (ZoneTexte z in lesZonesTextuelle)
                s += "{" + z + "}";
            s = s.Substring(0, s.Length-2);
            return s + "]}";
        }
    }
}
