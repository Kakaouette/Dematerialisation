using System;

namespace Numerisation_GIST
{
    //définition de zone où se trouve plusieurs chaîne de caractère
    //La zone est un rectangle défini en pixel, le tableau contient plusieurs 
    //chaîne de caractère qui devrait se trouver dans cette zone (en théorie, cela dépend du décalage par rapport au modèle lors de la numérisation)
    public class ZoneInfo
    {
        public String nom;
        public String motcle;
        public int hauteur;
        public int offset;

        public ZoneInfo()
        {
        }

        public ZoneInfo(String nom, String motcle, int hauteur, int offset)
        {
            this.nom = nom;
            this.motcle = motcle;
            this.hauteur = hauteur;
            this.offset = offset;
        }

        override public string ToString()
        {
            return "ZoneInfo{nom=" + this.nom +
                ", motcle=" + this.motcle +
                ", hauteur=" + this.hauteur +
                ", offset=" + this.offset + "}";
        }
    }
}

