using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCV
{
    //définition de zone où se trouve plusieurs chaîne de caractère
    //La zone est un rectangle défini en pixel, le tableau contient plusieurs 
    //chaîne de caractère qui devrait se trouver dans cette zone (en théorie, cela dépend du décalage par rapport au modèle lors de la numérisation)
    public class ZoneVerif
    {
        public readonly Rectangle zone;
        public readonly String[] motsCherche;

        public ZoneVerif(Rectangle zone, String[] motsCherche)
        {
            this.zone = zone;
            this.motsCherche = motsCherche;
        }

        override public string ToString()
        {
            string s = "ZoneVerif{Rectangle=" + this.zone + ", mots=[";
            foreach(String m in this.motsCherche)
            {
                s += m + ", ";
            }
            s = s.Substring(0, s.Length - 2);
            return s + "]}";
        }
    }

    public class ZoneVerifCollection : Collection<ZoneVerif>
    {
        
    }
}
