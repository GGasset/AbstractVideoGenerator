using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MathNet.Numerics.Distributions;
using NeatNetwork;
using NeatNetwork.NetworkFiles;
using static Functionality.ImageProcessing;
using static Functionality.PathGetter;

namespace AbstractVideoGenerator
{
    public partial class MainForm : Form
    {

        public static int networkSideSize = 60;
        int networkResolution;
        int networkResolitionDataSize;

        int[] autoEncoderShape,
            generativeShape,
            discriminatoryShape;
        int autoencoderCompressedLayer;

        NeuronHolder.NeuronTypes[] autoEncoderLayers,
            generativeLayers,
            discriminatoryLayers;

        NN autoEncoder, discriminative, generative;

        List<string[]> imagePaths;
        List<string> folderNames;
        List<string> shuffledImages;

        Timer autoencoderVideoTimer;
        double[] compressedVideoImage;

        #region Form things

        public MainForm()
        {
            InitializeComponent();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            int resolution = networkResolution = networkSideSize * networkSideSize;
            int resolutionDataSize = networkResolitionDataSize = resolution * 3;

            autoEncoderShape = new int[] { resolutionDataSize, 500, 150, 27, 150, 500, resolutionDataSize };

            autoencoderCompressedLayer = -1;
            int minLayerLength = int.MaxValue;
            for (int i = 1; i < autoEncoderShape.Length; i++)
            {
                if (autoEncoderShape[i] < minLayerLength)
                    autoencoderCompressedLayer = i - 1;
            }
            
            /*autoEncoderLayers = new NeuronHolder.NeuronTypes[autoEncoderShape.Length - 1];
            for (int x = 0; x < autoEncoderLayers.Length; x++)
                autoEncoderLayers[x] = NeuronHolder.NeuronTypes.Neuron;*/


            generativeShape = new int[] { resolutionDataSize, 500, 150, 100, 50, 50, 250, 300, 500, resolutionDataSize };

            generativeLayers = new NeuronHolder.NeuronTypes[generativeShape.Length - 1];
            /*for (int i = 0; i < generativeLayers.Length; i++)
                generativeLayers[i] = NeuronHolder.NeuronTypes.LSTM;*/


            discriminatoryShape = new int[] { resolutionDataSize, 500, 100, 20, 2, 1 };

            /*discriminatoryLayers = new NeuronHolder.NeuronTypes[autoEncoderShape.Length - 1];
            for (int x = 0; x < discriminatoryLayers.Length; x++)
                discriminatoryLayers[x] = NeuronHolder.NeuronTypes.LSTM;*/
        }

        #region Save Load (IO)

        private void saveToFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (autoEncoder == null && discriminative == null)
            {
                MessageBox.Show("Nothing to save (First train or load desired network/s)", "ERROR", MessageBoxButtons.OK);
                return;
            }

            if (MessageBox.Show("Do you wish to save your NN/s?", "", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                AddExtension = true,
                Filter = "Text files (*.txt)|*.txt",
                Title = "Select name and where you wish to save your networks",
            };

            while (saveFileDialog.ShowDialog() != DialogResult.OK);

            var path = saveFileDialog.FileName;
            string str = string.Empty;

            List<Task<string>> strTasks = new List<Task<string>>();
            if (autoEncoder != null && discriminative != null)
            {
                str += "autoencoder Gan";
                strTasks.Add(Task.Run(() => autoEncoder.ToString()));
                strTasks.Add(Task.Run(() => discriminative.ToString()));
                strTasks.Add(Task.Run(() => generative.ToString()));
            }
            else if (autoEncoder != null)
            {
                str += "autoencoder";
                strTasks.Add(Task.Run(() => autoEncoder.ToString()));
            }
            else
            {
                str += "Gan";
                strTasks.Add(Task.Run(() => discriminative.ToString()));
                strTasks.Add(Task.Run(() => generative.ToString()));
            }

            str += "\nJGG\n";

            bool isFinished = false;
            while (!isFinished)
            {
                System.Threading.Thread.Sleep(200);
                isFinished = true;
                foreach (var task in strTasks)
                {
                    isFinished = isFinished && task.IsCompleted;
                }
            }

            foreach (var strTask in strTasks)
            {
                str += strTask.Result;
                str += "\n====\n";
            }
            str = str.Remove(str.LastIndexOf("\n====\n"));

            File.WriteAllText(path, str);
        }


        private void loadFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you wish to load NN/s?", "", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select a .txt file generated by this app to load your NNs",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "Text files (*.txt)|*.txt",
                Multiselect = false,
            };

            while (openFileDialog.ShowDialog() != DialogResult.OK);

            var filePath = openFileDialog.FileName;
            var text = File.ReadAllText(filePath);
            string[] headerContent = text.Split(new string[] { "\nJGG\n" }, StringSplitOptions.None);
            string header = headerContent[0];
            string content = headerContent[1];

            string[] networkStrs = content.Split(new string[] { "\n====\n" }, StringSplitOptions.None);
            List<Task<NN>> NNTasks = new List<Task<NN>>();

            if (header == "autoencoder Gan")
            {
                NNTasks.Add(Task.Run(() => new NN(networkStrs[0])));
                NNTasks.Add(Task.Run(() => new NN(networkStrs[1])));
                NNTasks.Add(Task.Run(() => new NN(networkStrs[2])));
            }
            else if (header == "autoencoder")
            {
                NNTasks.Add(Task.Run(() => new NN(networkStrs[0])));
            }
            else if (header == "Gan")
            {
                NNTasks.Add(Task.Run(() => new NN(networkStrs[0])));
                NNTasks.Add(Task.Run(() => new NN(networkStrs[1])));
            }
            else
            {
                MessageBox.Show("This file wasn't generated by this app and thus is incompatible.", "Error", MessageBoxButtons.OK);
                return;
            }

            bool isCompleted = false;
            while (!isCompleted)
            {
                System.Threading.Thread.Sleep(100);
                isCompleted = true;
                foreach (var networkTask in NNTasks)
                {
                    isCompleted = isCompleted && networkTask.IsCompleted;
                }
            }

            if (header == "autoencoder Gan")
            {
                autoEncoder = NNTasks[0].Result;
                discriminative = NNTasks[1].Result;
                generative = NNTasks[2].Result;
            }
            else if (header == "autoencoder")
            {
                autoEncoder = NNTasks[0].Result;
            }
            else
            {
                discriminative = NNTasks[0].Result;
                generative = NNTasks[1].Result;
            }
        }

        #endregion Save Load (IO)

        #region Auto encoder

        #region Execution

        private void ShowAutoencoderImageBttn_Click(object sender, EventArgs e)
        {
            if (autoencoderVideoTimer != null)
                autoencoderVideoTimer.Stop();

            if (autoEncoder == null)
            {
                MessageBox.Show("First initialize autoencoder network");
                return;
            }

            Bitmap originalImage;
            if (shuffledImages == null)
            {
                string imagePath = GetImagePath();
                if (imagePath == null)
                    return;

                originalImage = new Bitmap(imagePath);
            }
            else
            {
                originalImage = new Bitmap(shuffledImages[new Random(DateTime.Now.Millisecond + rI++).Next(shuffledImages.Count)]);
            }

            Bitmap reducedImage = new Bitmap(originalImage, new Size(networkSideSize, networkSideSize));

            double[] X = BitmapToDoubleArray(reducedImage);
            double[] reconstructedImage = autoEncoder.Execute(X);

            Bitmap reconstructedBitmap = DoubleArrayToBitmap(reconstructedImage, networkSideSize, networkSideSize);
            Bitmap augmentedBitmap = new Bitmap(reconstructedBitmap, Display.Size);
            Display.Image = augmentedBitmap;

            originalImage.Dispose();
            reducedImage.Dispose();
            reconstructedBitmap.Dispose();
        }

        private void AutoencoderVideoSelectedImageBttn_Click(object sender, EventArgs e)
        {
            if (autoEncoder == null)
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

            Bitmap bmp = new Bitmap(imagePath);
            Bitmap downscaledBmp = new Bitmap(bmp, networkSideSize, networkSideSize);
            Display.Image = new Bitmap(bmp, Display.Size);
            
            double[] X = BitmapToDoubleArray(downscaledBmp);
            compressedVideoImage = autoEncoder.ExecuteUpToLayer(X, autoencoderCompressedLayer);

            bmp.Dispose();
            downscaledBmp.Dispose();

            autoencoderVideoTimer.Tick += ShowAlteredImage;
            autoencoderVideoTimer.Start();
        }

        private void ShowAlteredImage(object sender, EventArgs e)
        {
            var nOutput = autoEncoder.ExecuteFromLayer(autoencoderCompressedLayer, compressedVideoImage);
            Bitmap outputNetworkImage = DoubleArrayToBitmap(nOutput, networkSideSize, networkSideSize);
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

        #endregion Execution

        #region Training


        /// <returns>Includes original image data</returns>

        #endregion Training

        #endregion Auto encoder

        #endregion Form things

        #region functionality

        public static string FolderToName(string folderPath)
        {
            if (folderPath.EndsWith(@"\"))
                folderPath = folderPath.Remove(folderPath.LastIndexOf(@"\"));

            folderPath = folderPath.Remove(0, folderPath.LastIndexOf(@"\") + 1);
            return folderPath;
        }


        #endregion

        #region network things

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
