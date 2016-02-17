using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numerisation_GIST
{
    //définition de zone où se trouve plusieurs chaîne de caractère
    //La zone est un rectangle défini en pixel, le tableau contient plusieurs 
    //chaîne de caractère qui devrait se trouver dans cette zone (en théorie, cela dépend du décalage par rapport au modèle lors de la numérisation)
    public class ZoneTexte
    {
        public Rectangle zone { get; set; }
        public String[] mots { get; set; }

        public ZoneTexte()
        {
            zone = new Rectangle();
            mots = null;
        }

        public ZoneTexte(Rectangle zone, String[] motsCherche)
        {
            this.zone = zone;
            this.mots = motsCherche;
        }

        override public string ToString()
        {
            string s = "ZoneTextuel{zone=" + this.zone + ", mots=[";
            foreach(String m in this.mots)
            {
                s += m + ", ";
            }
            s = s.Substring(0, s.Length - 2);
            return s + "]}";
        }
    }
}
