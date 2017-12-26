using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSLibrary.Drawing
{
    /// <summary>
    /// 画像のテンプレートマッチングを行う
    /// </summary>
    public class ImageMatcher
    {
        public bool ImageMatch(Bitmap screen, Bitmap img, ref Point pos)
        {
            bool result = false;
        
            int[,] screenGray = imgToGray(screen);
            int[,] imgGray = imgToGray(img);
        
            int i = 0;
            int j = 0;
            //比較ループ - Y
            for (i = 0; i < screenGray.GetLength(0); i++)
            {
                //比較ループ - X
                for (j = 0; j < screenGray.GetLength(1); j++)
                {
                    if (screenGray[i, j] == imgGray[0,0])
                    {
                        if (RapidMatch(screenGray, imgGray, i, j))
                        {
                            //マッチ
                            result = true;
                            pos.X = j;
                            pos.Y = i;
                            goto Match_End;
                        }
                    }
                }
            }
        
            Match_End:
        
            return result;
        }

        private bool RapidMatch(int[,] screenGray, int[,] imgGray, int i, int j)
        {
            for (int y = 0; y < imgGray.GetLength(0); y++)
            {
                for (int x = 0; x < imgGray.GetLength(1); x++)
                {
                    if (imgGray[y, x] != screenGray[y + i, x + j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public int[,] imgToGray(Bitmap img)
        {
            int width = img.Width;
            int height = img.Height;
            int[,] gray_img = new int[height, width];
        
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = img.GetPixel(x, y);
                    gray_img[y, x] = pixel.ToArgb();
                }
            }
        
            return gray_img;
        }
    }
}
