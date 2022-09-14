using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MathNet.Numerics.Distributions;
using NeatNetwork;
using NeatNetwork.NetworkFiles;

namespace AbstractVideoGenerator
{
    public partial class MainForm : Form
    {
        static string[] supportedExtensions = new string[] { "JPG", "JPEG", "PNG" };

        int networkSideSize;
        int networkResolution;
        int networkResolitionDataSize;

        int[] autoEncoderShape,
            generativeShape,
            discriminatoryShape;

        NeuronHolder.NeuronTypes[] autoEncoderLayers,
            generativeLayers,
            discriminatoryLayers;

        RNN generative;
        NN autoEncoder, discriminative;

        List<string[]> imagePaths;
        List<string> folderNames;
        List<string> shuffledImages;

        #region Form things

        public MainForm()
        {
            InitializeComponent();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            networkSideSize = 20;
            int resolution = networkResolution = networkSideSize * networkSideSize;
            int resolutionDataSize = networkResolitionDataSize = resolution * 3;

            autoEncoderShape = new int[] { resolutionDataSize, 500, 150, 75, 150, 500, resolutionDataSize };

            /*autoEncoderLayers = new NeuronHolder.NeuronTypes[autoEncoderShape.Length - 1];
            for (int x = 0; x < autoEncoderLayers.Length; x++)
                autoEncoderLayers[x] = NeuronHolder.NeuronTypes.Neuron;*/


            generativeShape = new int[] { resolutionDataSize, 500, 150, 100, 50, 50, 250, 300, 500, resolutionDataSize };

            generativeLayers = new NeuronHolder.NeuronTypes[generativeShape.Length - 1];
            for (int i = 0; i < generativeLayers.Length; i++)
                generativeLayers[i] = NeuronHolder.NeuronTypes.LSTM;


            discriminatoryShape = new int[] { resolutionDataSize, 500, 100, 20, 2, 1 };

            /*discriminatoryLayers = new NeuronHolder.NeuronTypes[autoEncoderShape.Length - 1];
            for (int x = 0; x < discriminatoryLayers.Length; x++)
                discriminatoryLayers[x] = NeuronHolder.NeuronTypes.LSTM;*/
        }

        private void SelectDataSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        #region Auto encoder

        #region Execution

        private void ShowAutoencoderImageBttn_Click(object sender, EventArgs e)
        {
            if (shuffledImages == null)
            {
                MessageBox.Show("First set image paths");
                return;
            }

            if (autoEncoder == null)
            {
                MessageBox.Show("First initialize autoencoder network");
                return;
            }

            Bitmap image = new Bitmap(shuffledImages[new Random(DateTime.Now.Millisecond + rI++).Next(shuffledImages.Count)]);
            image = new Bitmap(image, new Size(networkSideSize, networkSideSize));

            double[] X = BitmapToDoubleArray(image);
            double[] reconstructedImage = autoEncoder.Execute(X);

            Bitmap reconstructedBitmap = DoubleArrayToBitmap(reconstructedImage, networkSideSize, networkSideSize);
            Bitmap augmentedBitmap = new Bitmap(reconstructedBitmap, Display.Size);
            Display.Image = augmentedBitmap;
        }

        #endregion

        #region Training

        private void TrainAutoencoder1NForAllFoldersBttn_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #endregion

        #endregion

        #region functionality

        public void GetImagePathsFromFolderContainingImageFolders(bool multipleNNs)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
            {
                Description = "You must select a folder with folders that contains images"
            };

            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
                return;


            List<string> directories = new List<string>(Directory.GetDirectories(folderBrowserDialog.SelectedPath));

            List<string[]> imagesPaths = new List<string[]>();
            List<string> unhirearchicalImagePaths = new List<string>();

            foreach (var imageDirectory in directories)
            {
                string[] currentPaths;
                imagesPaths.Add(currentPaths = FilterFiles(Directory.GetFiles(imageDirectory)));
                unhirearchicalImagePaths.AddRange(currentPaths);
            }

            shuffledImages = ShufflePaths(unhirearchicalImagePaths);

            List<int> emptyFoldersIndexes = new List<int>();
            for (int i = 0; i < directories.Count; i++)
                if (imagesPaths[i].Length == 0)
                    emptyFoldersIndexes.Add(i);

            for (int i = emptyFoldersIndexes.Count - 1; i >= 0; i--)
            {
                directories.RemoveAt(emptyFoldersIndexes[i]);
                imagesPaths.RemoveAt(emptyFoldersIndexes[i]);
            }

            folderNames = new List<string>();
            foreach (var directoryPath in directories)
            {
                string folderName = FolderToName(directoryPath);
                folderNames.Add(folderName);
            }
            comboBox.Items.Clear();
            if (multipleNNs)
            {
                comboBox.Items.AddRange(folderNames.ToArray());
                comboBox.Items.Add("");
            }
        }

        public string[] FilterFiles(string[] filePaths)
        {
            var output = new List<string>();
            foreach (var filePath in filePaths)
            {
                bool containsSupportedExtension = false;
                foreach (var supportedExtension in supportedExtensions)
                    containsSupportedExtension = filePath.ToLowerInvariant().Contains(supportedExtension.ToLowerInvariant()) || containsSupportedExtension;

                if (containsSupportedExtension)
                    output.Add(filePath);
            }
            return output.ToArray();
        }

        public static string FolderToName(string folderPath)
        {
            if (folderPath.EndsWith(@"\"))
                folderPath = folderPath.Remove(folderPath.LastIndexOf(@"\"));

            folderPath = folderPath.Remove(0, folderPath.LastIndexOf(@"\") + 1);
            return folderPath;
        }

        static int rI = 0;
        public static List<string> ShufflePaths(List<string> paths)
        {
            //Create a copy
            List<string> input = paths.ToList();
            Random r = new Random(DateTime.Now.Millisecond + rI++);

            List<string> output = new List<string>();
            int pathsCount = paths.Count;
            for (int i = 0; i < pathsCount; i++)
            {
                int selectedI = r.Next(input.Count);

                output.Add(input[selectedI]);
                input.RemoveAt(selectedI);
            }
            return output;
        }

        #endregion

        #region network things

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

                    R = Convert.ToByte(imageData[counter] * dataMultiplier);
                    counter++;
                    G = Convert.ToByte(imageData[counter] * dataMultiplier);
                    counter++;
                    B = Convert.ToByte(imageData[counter] * dataMultiplier);
                    counter++;

                    Color color = Color.FromArgb(255, R, G, B);

                    output.SetPixel(x, y, color);
                }
            }
            return output;
        }

        /*public List<Bitmap> GetBitmapVariations(Bitmap originalBitmap)
        {
            //originalBitmap.Clone();
        }*/

        public double[] GetGaussianNoise(double mean, double standarDeviation, int arrayLength)
        {
            double[] output = new double[arrayLength];
            Normal normalDistribution = new Normal(mean, standarDeviation);
            for (int i = 0; i < arrayLength; i++)
                output[i] = normalDistribution.Sample();
            return output;
        }

        #endregion
    }
}
