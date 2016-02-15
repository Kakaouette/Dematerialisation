using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;

namespace Numerisation_GIST
{
    public class ImageTemplate
    {
        private Image<Gray, byte> RLSAH(Image<Gray, byte> img)
        {
            int hor_thres = 22;
            int zero_count = 0;
            int one_flag = 0;
            Byte[,,] data = img.Data;

            for (int i = 0; i < img.Mat.Rows; i++)
            {
                for (int j = 0; j < img.Mat.Cols; j++)
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
            return new Image<Gray, byte>(data);
        }

        private Image<Gray, byte> RLSAV(Image<Gray, byte> img)
        {
            int hor_thres = 5;
            int zero_count = 0;
            int one_flag = 0;
            Byte[,,] data = img.Data;

            for (int i = 0; i < img.Mat.Cols; i++)
            {
                for (int j = 0; j < img.Mat.Rows; j++)
                {
                    if (data[j, i, 0] == 0)
                    {
                        if (one_flag == 255)
                        {
                            if (zero_count <= hor_thres)
                            {
                                for (int j2 = j - zero_count; j2 <= j; j2++)
                                {
                                    if (i >= 0 && j2 >= 0)
                                    {
                                        data[j2, i, 0] = 0;
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
            return new Image<Gray, byte>(data);
        }

        //Implémentaition de RLSA (j'ai rien compris mais ça marche) retourne le document structuré
        public Image<Gray, byte> RLSA(Image<Gray, byte> tmpImg)
        {
            Image<Gray, byte> calcul = tmpImg.Clone();
            Image<Gray, byte> vertical = RLSAV(calcul);
            Image<Gray, byte> horizontal = RLSAH(calcul);
            Image<Gray, byte> structure = tmpImg.Clone();

            CvInvoke.BitwiseAnd(vertical, horizontal, structure);
            return structure;
        }
    }
}
