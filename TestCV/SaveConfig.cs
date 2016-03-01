using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numerisation_GIST
{
    public class SaveConfig
    {
        public String tessdata;
        public String cheminModele;
        public String cheminImage;
        public String cheminTemp;
        public int tailleImgW = 1991;
        public int tailleImgH = 2818;
        public int numerisationDPI;

        public SaveConfig() { }

        public SaveConfig(String tess, String modele, String image, String temp, int width, int height, int dpi)
        {
            tessdata = tess;
            cheminModele = modele;
            cheminImage = image;
            cheminTemp = temp;
            tailleImgH = height;
            tailleImgW = width;
            numerisationDPI = dpi;
        }
    }
}
