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
        Hashtable networks = new Hashtable();

        int[] autoEncoderShape,
            generativeShape,
            discriminatoryShape;

        NeuronHolder.NeuronTypes[] autoEncoderLayers,
            generativeLayers,
            discriminatoryLayers;

        #region Form things

        public MainForm()
        {
            InitializeComponent();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            int displaySize = Display.Size.Width * Display.Size.Height;
            int displayInfoSize = displaySize * 3;

            autoEncoderShape = new int[] { displayInfoSize, 1000000, 1000, 10, 1000, 1000000, displayInfoSize };

            autoEncoderLayers = new NeuronHolder.NeuronTypes[autoEncoderShape.Length - 1];
            for (int i = 0; i < autoEncoderLayers.Length; i++)
                autoEncoderLayers[i] = NeuronHolder.NeuronTypes.LSTM;


            generativeShape = new int[] { displayInfoSize, 1000000, 500000, 20000, 10000000, displayInfoSize };

            autoEncoderLayers = new NeuronHolder.NeuronTypes[generativeShape.Length - 1];
            for (int i = 0; i < generativeLayers.Length; i++)
                generativeLayers[i] = NeuronHolder.NeuronTypes.LSTM;


            discriminatoryShape = new int[] { displayInfoSize, 1000000, 50000, 500, 20, 2, 1 };

            discriminatoryLayers = new NeuronHolder.NeuronTypes[autoEncoderShape.Length - 1];
            for (int i = 0; i < discriminatoryLayers.Length; i++)
                discriminatoryLayers[i] = NeuronHolder.NeuronTypes.LSTM;
        }

        private void trainFromImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            networks.Clear();

            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
            {
                Description = "You must select a folder with folders that contains images"
            };

            folderBrowserDialog.ShowDialog();

            List<string> directories = new List<string>(Directory.GetDirectories(folderBrowserDialog.SelectedPath));

            List<string[]> imagesPaths = new List<string[]>();
            foreach (var imageDirectory in directories)
            {
                imagesPaths.Add(FilterFiles(Directory.GetFiles(imageDirectory)));
            }

            List<int> emptyFoldersIndexes = new List<int>();
            for (int i = 0; i < directories.Count; i++)
                if (imagesPaths[i].Length == 0)
                    emptyFoldersIndexes.Add(i);

            for (int i = emptyFoldersIndexes.Count - 1; i >= 0; i--)
            {
                directories.RemoveAt(emptyFoldersIndexes[i]);
                imagesPaths.RemoveAt(emptyFoldersIndexes[i]);
            }

            List<string> directoryNames = new List<string>();
            foreach (var directoryPath in directories)
            {
                directoryNames.Add(FolderToName(directoryPath));
            }
            comboBox.Items.Clear();
            comboBox.Items.AddRange(directoryNames.ToArray());

            Random r = new Random();
            for (int i = 0; i < imagesPaths.Count; i++)
            {
                StatusLabel.Text = $"Training networks for {directories[i]} folder";

                networks.Add($"G{directoryNames[i]}", new RNN(generativeShape, generativeLayers, NeatNetwork.Libraries.Activation.ActivationFunctions.Sigmoid));
                networks.Add($"D{directoryNames[i]}", new RNN(discriminatoryShape, discriminatoryLayers, NeatNetwork.Libraries.Activation.ActivationFunctions.Sigmoid));
                networks.Add($"A{directoryNames[i]}", new RNN(autoEncoderShape, autoEncoderLayers, NeatNetwork.Libraries.Activation.ActivationFunctions.Sigmoid));


            }
        }

        #endregion

        #region functionality

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

        #endregion

        #region network things

        public double[] BitmapToDoubleArray(Bitmap bitmap)
        {

        }

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
