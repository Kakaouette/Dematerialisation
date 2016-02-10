using Emgu.CV;
using System;
using System.Collections.Generic;

namespace TestCV
{
    //Représente un pattern de page avec son numéro de page et une liste de zone
    //Contient toutes les méthodes permettant de vérifié qu'une image (Objet Mat) appartient au pattern
    class PatternPage
    {
        public readonly int numero;
        public List<ZoneVerif> lesZones {get; private set; }

        public PatternPage(int numero, List<ZoneVerif> lesZones)
        {
            this.numero = numero;
            this.lesZones = lesZones;
        }

        public PatternPage(int numero)
        {
            this.numero = numero;
            this.lesZones = new List<ZoneVerif>();
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
        public bool isPage(Mat img)
        {
            //Passage en binaire
            Mat imgBin = Program.imgTraitement.convertBinOtsu(img);
            int i = 1;
            foreach (ZoneVerif z in this.lesZones)
            {
                //découpe & sauvegarde
                Mat imgR = Program.imgTraitement.rognerImage(imgBin, z.zone);
                imgR.Save(Program.cheminTemp + this.numero + "-" + i + "-crop-page.tif");
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
