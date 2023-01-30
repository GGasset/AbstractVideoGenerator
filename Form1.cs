using NeatNetwork;
using NeatNetwork.NetworkFiles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Functionality.ImageProcessing;
using static Functionality.PathGetter;
using static Functionality.NetworkHolder;

namespace AbstractVideoGenerator
{
    public partial class MainForm : Form
    {
        private Timer autoencoderVideoTimer;
        private double[] compressedVideoImage;

        #region Form things

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadedNetworklabel.Text += Enum.GetName(typeof(LoadedNetworkType), LoadedNetworklabel);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Display.Image.Dispose();
        }

        #region Auto encoder execution

        private void ShowAutoencoderImageBttn_Click(object sender, EventArgs e)
        {
            autoencoderVideoTimer?.Stop();

            CheckIfNNIsReady(autoencoder);

            Bitmap originalImage;
            string imagePath = GetImagePath();
            if (imagePath == null)
                return;

            originalImage = new Bitmap(imagePath);
            int networkOutputSquareSideSize = GetOutputSquareSideSize();

            Bitmap reducedImage = new Bitmap(originalImage, new Size(networkOutputSquareSideSize, networkOutputSquareSideSize));

            double[] X = BitmapToDoubleArray(reducedImage);
            double[] reconstructedImage = autoencoder.Execute(X);

            Bitmap reconstructedBitmap = DoubleArrayToBitmap(reconstructedImage, networkOutputSquareSideSize, networkOutputSquareSideSize);
            Bitmap augmentedBitmap = new Bitmap(reconstructedBitmap, Display.Size);
            Display.Image = augmentedBitmap;

            originalImage.Dispose();
            reducedImage.Dispose();
            reconstructedBitmap.Dispose();
        }

        private void AutoencoderVideoSelectedImageBttn_Click(object sender, EventArgs e)
        {
            CheckIfNNIsReady(autoencoder);

            autoencoderVideoTimer = new Timer()
            {
                Interval = 33
            };

            string imagePath = GetImagePath();
            if (imagePath == null)
                return;

            int networkOutputSquareSideSize = GetOutputSquareSideSize();

            Bitmap bmp = new Bitmap(imagePath);
            Bitmap downscaledBmp = new Bitmap(bmp, networkOutputSquareSideSize, networkOutputSquareSideSize);
            Display.Image = new Bitmap(bmp, Display.Size);

            double[] X = BitmapToDoubleArray(downscaledBmp);
            compressedVideoImage = autoencoder.ExecuteUpToLayer(X, GetAutoencoderMostCompressedLayer());

            bmp.Dispose();
            downscaledBmp.Dispose();

            autoencoderVideoTimer.Tick += ShowAlteredImage;
            autoencoderVideoTimer.Start();
        }

        private void ShowAlteredImage(object sender, EventArgs e)
        {
            var nOutput = autoencoder.ExecuteFromLayer(GetAutoencoderMostCompressedLayer(), compressedVideoImage);
            int networkOutputSquareSideSize = GetOutputSquareSideSize();
            Bitmap outputNetworkImage = DoubleArrayToBitmap(nOutput, networkOutputSquareSideSize, networkOutputSquareSideSize);
            Bitmap upscaledBmp = new Bitmap(outputNetworkImage, Display.Size);
            Display.Image.Dispose();
            Display.Image = upscaledBmp;

            outputNetworkImage.Dispose();

            Random r = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < compressedVideoImage.Length; i++)
            {
                double variation = (r.NextDouble() - .5) / 5;
                compressedVideoImage[i] += variation;
            }
        }

        #endregion Auto encoder execution

        #region Gan execution

        private void ShowGanImageBttn_Click(object sender, EventArgs e)
        {
            CheckIfNNIsReady(generative);

            Display.Image.Dispose();

            string imagePath = GetImagePath();

            Bitmap original = new Bitmap(imagePath);
            Bitmap resized = new Bitmap(original, new Size(GetOutputSquareSideSize(), GetOutputSquareSideSize()));

            Display.Image = DoubleArrayToBitmap(generative.Execute(BitmapToDoubleArray(resized)), resized.Width, resized.Height);

            original.Dispose();
            resized.Dispose();
        }

        #endregion Gan execution

        #region Stable diffusion

        private void StableDiffusionImage_Click(object sender, EventArgs e)
        {
            CheckIfNNIsReady(reverseDiffusor);

            double[] output = GetGaussianNoise(.5, .15, discriminative.Shape[0]);
            for (int i = 1; i < reverseDiffusorDiffusions; i++)
            {
                output = reverseDiffusor.Execute(output);
            }

            Display.Image.Dispose();
            var squareSideSize = GetOutputSquareSideSize();
            var outputBmp = DoubleArrayToBitmap(output, squareSideSize, squareSideSize);
            var resized = new Bitmap(outputBmp, Display.Size);
            outputBmp.Dispose();
            Display.Image = resized;
        }

        #endregion Stable diffusion

        #endregion Form things

        #region functionality

        private void CheckIfNNIsReady(NN n)
        {
            if (n == null)
            {
                var dialogResult = MessageBox.Show("Please, first load or train the network you are trying to use.\nDo you wish to do it now?", "", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes) Close();
            }
        }

        public static string FolderToName(string folderPath)
        {
            if (folderPath.EndsWith(@"\"))
                folderPath = folderPath.Remove(folderPath.LastIndexOf(@"\"));

            folderPath = folderPath.Remove(0, folderPath.LastIndexOf(@"\") + 1);
            return folderPath;
        }

        private int GetOutputSquareSideSize()
        {
            int output;
            switch (loadedNetwork)
            {
                case LoadedNetworkType.autoencoder:
                    output = autoencoder.Shape[0];
                    break;
                case LoadedNetworkType.Gans:
                    output = discriminative.Shape[0];
                    break;
                case LoadedNetworkType.ReverseDiffusor:
                    output = reverseDiffusor.Shape[0];
                    break;
                default:
                    throw new NotImplementedException();
            }
            output /= 3;
            output = Convert.ToInt32(Math.Sqrt(output));
            return output;
        }

        private int GetAutoencoderMostCompressedLayer()
        {
            int autoencoderCompressedLayer = -1;
            int[] autoencoderShape = autoencoder.Shape;
            int minLayerLength = int.MaxValue;
            for (int i = 1; i < autoencoder.LayerCount; i++)
                if (autoencoderShape[i] < minLayerLength)
                    autoencoderCompressedLayer = i - 1;
            return autoencoderCompressedLayer;
        }

        #endregion functionality

    }
}