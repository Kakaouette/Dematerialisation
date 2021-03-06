﻿using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numerisation_GIST
{
    public class Master
    {
        public ZoneTexte laZoneTextuelle;
        public List<PageModele> lesPagesModeles;

        public Master()
        {}

        public bool estMaster(List<Image<Gray, Byte>> lesImagesNum)
        {
            int i = 1;
            foreach (Image<Gray, Byte> img in lesImagesNum)
            {
                //binaire
                Image<Gray, byte> imgBin = MainWindow.imageModification.convertionBinaire(img);
                //découpe
                Image<Gray, byte> imgR = MainWindow.imageModification.rogner(imgBin, laZoneTextuelle.zone);
                //tesseract
                String texte = MainWindow.tesseract.tesseractAnalyse(imgR);

                img.Save(MainWindow.cheminTmp + i + ".tif");
                imgR.Save(MainWindow.cheminTmp + i + "Crop.tif");
                i++;
                foreach(String cmp in laZoneTextuelle.mots)
                {
                    if (texte.ToLower().Contains(cmp.ToLower()))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
