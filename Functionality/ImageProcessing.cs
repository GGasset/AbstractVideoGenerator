using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionality
{
    public static class ImageProcessing
    {
        public static double[] BitmapToDoubleArray(Bitmap bitmap, double dataDividend = 255)
        {
            int imageSize = bitmap.Height * bitmap.Width;
            int bitmapData = imageSize * 3;

            double[] output = new double[bitmapData];
            int i = 0;
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color cPixel = bitmap.GetPixel(x, y);

                    output[i] = cPixel.R / dataDividend;
                    i++;
                    output[i] = cPixel.G / dataDividend;
                    i++;
                    output[i] = cPixel.B / dataDividend;
                    i++;
                }
            }
            return output;
        }

        public static Bitmap DoubleArrayToBitmap(double[] imageData, int bitmapWidth, int bitmapHeight, double dataMultiplier = 255)
        {
            int counter = 0;
            Bitmap output = new Bitmap(bitmapWidth, bitmapHeight);
            for (int x = 0; x < bitmapWidth; x++)
            {
                for (int y = 0; y < bitmapHeight; y++)
                {
                    byte R, G, B;

                    R = Convert.ToByte(Math.Min(Math.Max(imageData[counter], 0), 1) * dataMultiplier);
                    counter++;
                    G = Convert.ToByte(Math.Min(Math.Max(imageData[counter], 0), 1) * dataMultiplier);
                    counter++;
                    B = Convert.ToByte(Math.Min(Math.Max(imageData[counter], 0), 1) * dataMultiplier);
                    counter++;

                    Color color = Color.FromArgb(255, R, G, B);

                    output.SetPixel(x, y, color);
                }
            }
            return output;
        }

        public static List<double[]> GetImageVariations(Bitmap original)
        {
            List<double[]> imagesData = new List<double[]>();
            imagesData.Add(BitmapToDoubleArray(original));
            Bitmap variation = new Bitmap(original);
            variation.RotateFlip(RotateFlipType.Rotate180FlipXY);
            imagesData.Add(BitmapToDoubleArray(variation));
            variation.Dispose();

            variation = new Bitmap(original);
            variation.RotateFlip(RotateFlipType.Rotate90FlipNone);
            imagesData.Add(BitmapToDoubleArray(variation));
            variation.Dispose();
            variation = new Bitmap(original);

            variation.RotateFlip(RotateFlipType.Rotate270FlipXY);
            imagesData.Add(BitmapToDoubleArray(variation));
            variation.Dispose();
            variation = new Bitmap(original);

            variation.RotateFlip(RotateFlipType.Rotate180FlipNone);
            imagesData.Add(BitmapToDoubleArray(variation));
            variation.Dispose();
            variation = new Bitmap(original);

            variation.RotateFlip(RotateFlipType.RotateNoneFlipXY);
            imagesData.Add(BitmapToDoubleArray(variation));
            variation.Dispose();
            variation = new Bitmap(original);

            variation.RotateFlip(RotateFlipType.Rotate270FlipNone);
            imagesData.Add(BitmapToDoubleArray(variation));
            variation.Dispose();
            variation = new Bitmap(original);

            variation.RotateFlip(RotateFlipType.Rotate90FlipXY);
            imagesData.Add(BitmapToDoubleArray(variation));
            variation.Dispose();
            variation = new Bitmap(original);

            variation.RotateFlip(RotateFlipType.RotateNoneFlipX);
            imagesData.Add(BitmapToDoubleArray(variation));
            variation.Dispose();
            variation = new Bitmap(original);

            variation.RotateFlip(RotateFlipType.Rotate180FlipY);
            imagesData.Add(BitmapToDoubleArray(variation));
            variation.Dispose();
            variation = new Bitmap(original);

            variation.RotateFlip(RotateFlipType.Rotate90FlipX);
            imagesData.Add(BitmapToDoubleArray(variation));
            variation.Dispose();
            variation = new Bitmap(original);

            variation.RotateFlip(RotateFlipType.Rotate270FlipY);
            imagesData.Add(BitmapToDoubleArray(variation));
            variation.Dispose();
            variation = new Bitmap(original);

            variation.RotateFlip(RotateFlipType.Rotate180FlipX);
            imagesData.Add(BitmapToDoubleArray(variation));
            variation.Dispose();
            variation = new Bitmap(original);

            variation.RotateFlip(RotateFlipType.RotateNoneFlipY);
            imagesData.Add(BitmapToDoubleArray(variation));
            variation.Dispose();
            variation = new Bitmap(original);

            variation.RotateFlip(RotateFlipType.Rotate270FlipX);
            imagesData.Add(BitmapToDoubleArray(variation));
            variation.Dispose();
            variation = new Bitmap(original);

            variation.RotateFlip(RotateFlipType.Rotate90FlipY);
            imagesData.Add(BitmapToDoubleArray(variation));
            variation.Dispose();

            return imagesData;
        }
    }
}
