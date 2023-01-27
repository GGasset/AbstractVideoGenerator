using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;

namespace Functionality
{
    public static class ImageProcessing
    {
        public static List<double[]> GenerateGaussianNoiseImages(double mean, double std, int imageCount, int imageResolution)
        {
            List<double[]> images = new List<double[]>();
            Task<double[]>[] tasks = new Task<double[]>[imageCount];
            for (int i = 0; i < imageCount; i++)
            {
                tasks[i] = Task.Run(() => GetGaussianNoise(mean, std, imageResolution));
            }

            for (int i = 0; i < imageCount; i++)
            {
                tasks[i].Wait();
                images.Add(tasks[i].Result);
            }
            return images;
        }

        public static double[] ApplyGaussianNoiseToImageData(double[] original, double mean, double std)
        {
            double[] output = GetGaussianNoise(mean, std, original.Length);
            for (int i = 0; i < original.Length; i++)
                output[i] = Math.Max(0, Math.Min(1, output[i] + original[i]));

            return output;
        }

        public static double[] GetGaussianNoise(double mean, double std, int arrayLength)
        {
            double[] output = new double[arrayLength];
            Normal normalDistribution = new Normal(mean, std);
            for (int i = 0; i < arrayLength; i++)
                output[i] = normalDistribution.Sample();
            return output;
        }

        public static double[] BitmapToDoubleArray(Bitmap bitmap, bool isGreyScale = false, double dataDividend = 255)
        {
            int imageSize = bitmap.Height * bitmap.Width;
            int bitmapData = imageSize * 3;

            double[] output = new double[isGreyScale? imageSize : bitmapData];
            int i = 0;
            if (!isGreyScale)
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
            else
                for (int x = 0; x < bitmap.Width; x++)
                {
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        Color cPixel = bitmap.GetPixel(x, y);

                        int color = cPixel.R + cPixel.G + cPixel.B;
                        color /= 3;

                        output[i] = color / dataDividend;
                    }
                }

            return output;
        }

        public static Bitmap DoubleArrayToBitmap(double[] imageData, int bitmapWidth, int bitmapHeight, bool isGreyScale = false, double dataMultiplier = 255)
        {
            int counter = 0;
            Bitmap output = new Bitmap(bitmapWidth, bitmapHeight);
            if (!isGreyScale)
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
            else
                for (int x = 0; x < bitmapWidth; x++)
                {
                    for (int y = 0; y < bitmapHeight; y++)
                    {
                        byte greyscale = Convert.ToByte(Math.Max(0, Math.Min(1, imageData[counter])));
                        Color color = Color.FromArgb(greyscale, greyscale, greyscale);
                        output.SetPixel(x, y, color);

                        counter++;
                    }
                }
            return output;
        }

        public static List<double[]> GetImageVariations(Bitmap original, bool isGreyScale = false)
        {
            List<double[]> imagesData = new List<double[]>();
            imagesData.Add(BitmapToDoubleArray(original));
            Bitmap variation = new Bitmap(original);
            variation.RotateFlip(RotateFlipType.Rotate180FlipXY);
            imagesData.Add(BitmapToDoubleArray(variation, isGreyScale));
            variation.Dispose();

            variation = new Bitmap(original);
            variation.RotateFlip(RotateFlipType.Rotate90FlipNone);
            imagesData.Add(BitmapToDoubleArray(variation, isGreyScale));
            variation.Dispose();
            variation = new Bitmap(original);

            variation.RotateFlip(RotateFlipType.Rotate270FlipXY);
            imagesData.Add(BitmapToDoubleArray(variation, isGreyScale));
            variation.Dispose();
            variation = new Bitmap(original);

            variation.RotateFlip(RotateFlipType.Rotate180FlipNone);
            imagesData.Add(BitmapToDoubleArray(variation, isGreyScale));
            variation.Dispose();
            variation = new Bitmap(original);

            variation.RotateFlip(RotateFlipType.RotateNoneFlipXY);
            imagesData.Add(BitmapToDoubleArray(variation, isGreyScale));
            variation.Dispose();
            variation = new Bitmap(original);

            variation.RotateFlip(RotateFlipType.Rotate270FlipNone);
            imagesData.Add(BitmapToDoubleArray(variation, isGreyScale));
            variation.Dispose();
            variation = new Bitmap(original);

            variation.RotateFlip(RotateFlipType.Rotate90FlipXY);
            imagesData.Add(BitmapToDoubleArray(variation, isGreyScale));
            variation.Dispose();
            variation = new Bitmap(original);

            variation.RotateFlip(RotateFlipType.RotateNoneFlipX);
            imagesData.Add(BitmapToDoubleArray(variation, isGreyScale));
            variation.Dispose();
            variation = new Bitmap(original);

            variation.RotateFlip(RotateFlipType.Rotate180FlipY);
            imagesData.Add(BitmapToDoubleArray(variation, isGreyScale));
            variation.Dispose();
            variation = new Bitmap(original);

            variation.RotateFlip(RotateFlipType.Rotate90FlipX);
            imagesData.Add(BitmapToDoubleArray(variation, isGreyScale));
            variation.Dispose();
            variation = new Bitmap(original);

            variation.RotateFlip(RotateFlipType.Rotate270FlipY);
            imagesData.Add(BitmapToDoubleArray(variation, isGreyScale));
            variation.Dispose();
            variation = new Bitmap(original);

            variation.RotateFlip(RotateFlipType.Rotate180FlipX);
            imagesData.Add(BitmapToDoubleArray(variation, isGreyScale));
            variation.Dispose();
            variation = new Bitmap(original);

            variation.RotateFlip(RotateFlipType.RotateNoneFlipY);
            imagesData.Add(BitmapToDoubleArray(variation, isGreyScale));
            variation.Dispose();
            variation = new Bitmap(original);

            variation.RotateFlip(RotateFlipType.Rotate270FlipX);
            imagesData.Add(BitmapToDoubleArray(variation, isGreyScale));
            variation.Dispose();
            variation = new Bitmap(original);

            variation.RotateFlip(RotateFlipType.Rotate90FlipY);
            imagesData.Add(BitmapToDoubleArray(variation, isGreyScale));
            variation.Dispose();

            return imagesData;
        }

    }
}
