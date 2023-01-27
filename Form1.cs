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
            if (autoencoder != null)
            {
                LoadedNetworksLabel.Text += "autoencoder";
            }
            if (generative != null)
            {
                LoadedNetworksLabel.Text += ", Gans";
            }
            if (reverseDiffusor != null)
            {
                LoadedNetworksLabel.Text += ", Stable diffusion";
            }

            string text = LoadedNetworksLabel.Text;
            string[] introductionBody = text.Split(':');
            introductionBody[0] += ":";

            string bodyIntroduction = " , ";
            if (introductionBody[1].StartsWith(bodyIntroduction))
                introductionBody[1].Remove(0, bodyIntroduction.Length);

            LoadedNetworksLabel.Text = introductionBody[0] + introductionBody[1];
        }


        #region Auto encoder execution

        private void ShowAutoencoderImageBttn_Click(object sender, EventArgs e)
        {
            autoencoderVideoTimer?.Stop();

            if (autoencoder == null)
            {
                MessageBox.Show("First initialize autoencoder network");
                return;
            }

            Bitmap originalImage;
            string imagePath = GetImagePath();
            if (imagePath == null)
                return;

            originalImage = new Bitmap(imagePath);
            int networkOutputSquareSideSize = GetAutoencoderOutputSquareSideSize();

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
            if (autoencoder == null)
            {
                MessageBox.Show("First you need to train or load an autoencoder network", "ERROR");
                return;
            }

            autoencoderVideoTimer = new Timer()
            {
                Interval = 33
            };

            string imagePath = GetImagePath();
            if (imagePath == null)
                return;

            int networkOutputSquareSideSize = GetAutoencoderOutputSquareSideSize();

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
            int networkOutputSquareSideSize = GetAutoencoderOutputSquareSideSize();
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

        #region GanExecution

        private void ShowGanImageBttn_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #endregion Form things

        #region functionality

        public static string FolderToName(string folderPath)
        {
            if (folderPath.EndsWith(@"\"))
                folderPath = folderPath.Remove(folderPath.LastIndexOf(@"\"));

            folderPath = folderPath.Remove(0, folderPath.LastIndexOf(@"\") + 1);
            return folderPath;
        }

        private int GetAutoencoderOutputSquareSideSize()
        {
            int output = autoencoder.Shape[autoencoder.Shape.Length - 1];
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