using Emgu.CV;
using Emgu.CV.Structure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numerisation_GIST
{
    class Master
    {
        [JsonRequired]
        public ZoneTexte laZoneTextuelle;

        [JsonRequired]
        public List<PageModele> lesPagesModeles;

        public Master()
        {}

        public bool estMaster(List<Image<Gray, Byte>> lesImagesNum)
        {
            int i = 1;
            foreach (Image<Gray, Byte> img in lesImagesNum)
            {
                //binaire
                Image<Gray, byte> imgBin = Program.imageModification.convertionBinaire(img);
                //découpe
                Image<Gray, byte> imgR = Program.imageModification.rogner(imgBin, laZoneTextuelle.zone);
                //tesseract
                String texte = Program.tesseract.tesseractAnalyse(imgR);

                img.Save(Program.cheminTmp + i + ".tif");
                imgR.Save(Program.cheminTmp + i + "Crop.tif");
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
