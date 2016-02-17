using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;

namespace Numerisation_GIST
{
    //Représente un pattern de page avec son numéro de page et une liste de zone
    //Contient toutes les méthodes permettant de vérifié qu'une image (Objet Mat) appartient au pattern
    class PatternPage
    {
        public readonly int numero;
        public List<ZoneVerif> lesZones {get; private set; }
        public Image<Gray, byte> image { get; private set; }

        public PatternPage(int numero, List<ZoneVerif> lesZones, string file)
        {
            this.numero = numero;
            this.lesZones = lesZones;
            this.image = new Image<Gray, byte>(file);
        }

        public PatternPage(int numero, string file)
        {
            this.numero = numero;
            this.lesZones = new List<ZoneVerif>();
            this.image = new Image<Gray, byte>(file);
        }

        //Vérifie qu'un des mot est présent dans la chaîne, si oui renvoi true, sinon false
        bool isPresent(String chaine, String[] mots)
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
        public bool isPage(Image<Gray, byte> img)
        {
            //Passage en binaire
            Image<Gray, byte> imgBin = Program.imgTraitement.convertBinOtsu(img);
            int i = 1;
            foreach (ZoneVerif z in this.lesZones)
            {
                //découpe & sauvegarde
                Image<Gray, byte> imgR = Program.imgTraitement.rognerImage(imgBin, z.zone);
                //tesseract
                String texteTesseract = Program.console.tesseractAnalyse(imgR);
                //Si le texte reconnu par tesseract ne correspond pas à celui de la zone, return false
                if (!isPresent(texteTesseract, z.motsCherche))
                {
                    Console.WriteLine("\tPattern " + numero + " - Pas de correspondance avec la zone " + i);
                    return false;
                }
                i++;
                imgR.Dispose();
            }
            //Si tous les tests de zone ont renvoyé vrai on renvoi true
            imgBin.Dispose();
            return true;
        }

        override public string ToString()
        {
            String s = "PatternPage{numero=" + numero + ", zone=[";
            foreach (ZoneVerif z in lesZones)
                s += "{" + z + "}";
            s = s.Substring(0, s.Length-2);
            return s + "]}";
        }
    }
}
