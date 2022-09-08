using System;
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

namespace AbstractVideoGenerator
{
    public partial class MainForm : Form
    {
        static string[] supportedExtensions = new string[] { "JPG", "JPEG", "PNG" };

        public MainForm()
        {
            InitializeComponent();
        }

        private void trainFromImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
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

        public double[] GetGaussianNoise(double mean, double standarDeviation, int arrayLength)
        {
            double[] output = new double[arrayLength];
            Normal normalDistribution = new Normal(mean, standarDeviation);
            for (int i = 0; i < arrayLength; i++)
                output[i] = normalDistribution.Sample();
            return output;
        }
    }
}
