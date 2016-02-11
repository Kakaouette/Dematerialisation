using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;

namespace Numerisation_GIST
{
    class ImageTemplate
    {
        //Implémentaition de RLSA (j'ai rien compris mais ça marche) retourne le document structuré
        public Mat RLSA(Mat tmpImg)
        {
            int hor_thres = 25;
            int zero_count = 0;
            int one_flag = 0;
            Image<Gray, byte> img = tmpImg.ToImage<Gray, byte>();
            Byte[,,] data = img.Data;

            //Console.WriteLine("Data size=" + tmpImg.Data);
            for (int i = 0; i < tmpImg.Rows; i++)
            {
                for (int j = 0; j < tmpImg.Cols; j++)
                {
                    if (data[i, j, 0] == 0)
                    {
                        if (one_flag == 255)
                        {
                            if (zero_count <= hor_thres)
                            {

                                for(int j2 = j - zero_count; j2 <= j; j2++)
                                {
                                    if (i >= 0 && j2 >= 0 )
                                    {
                                        //Console.WriteLine("Data primary acces=[" + i + ", " + j2 + ", " + 0 + "]");
                                        data[i, j2, 0] = 0;
                                    }
                                }
                            }
                            else
                            {
                                one_flag = 0;
                            }
                            zero_count = 0;
                        }
                        one_flag = 255;
                    }
                    else
                    {
                        if (one_flag == 255)
                        {
                            zero_count = zero_count + 1;
                        }
                    }
                }
            }
            img.Data = data;
            return new Mat(img.Mat, img.ROI);
        }
    }
}
