using NeatNetwork;
using NeatNetwork.NetworkFiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Functionality.ImageProcessing;

namespace NetworkTrainer
{
    public class Program
    {
        public static int NetworkOutputSquareSideResolution = 50;

        public static void Main(string[] args)
        {
            while (true)
            {
                if (MessageBox.Show("Do you wish to train a network???") == DialogResult.Yes)
                {
                    if (MessageBox.Show($"The current network output square side resolution is {NetworkOutputSquareSideResolution}, do you wish to change it???", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Console.WriteLine("Select a resolution for the network output square side, valid inputs are numbers in this format: 20");
                        NetworkOutputSquareSideResolution = GetInputInt();
                    }

                    int imageResolution = NetworkOutputSquareSideResolution * NetworkOutputSquareSideResolution;
                    int resolutionDataSize = imageResolution * 3;

                    int[] autoEncoderShape = new int[] { resolutionDataSize, 500, 150, 27, 150, 500, resolutionDataSize };

                    int[] generativeShape = new int[] { resolutionDataSize, 500, 150, 100, 50, 50, 250, 300, 500, resolutionDataSize };

                    int[] discriminativeShape = new int[] { resolutionDataSize, 500, 100, 20, 2, 1 };
                }

                RunNetworkExecutionInterface();
            }
        }

        private static NN TrainAutoEncoderOnImages(List<string> paths, int[] autoEncoderShape, double learningRate, bool showResultMessageBox)
        {
            var watch = Stopwatch.StartNew();

            NN output = new NN(autoEncoderShape, NeatNetwork.Libraries.Activation.ActivationFunctions.Sigmoid);

            List<double[]> imagesData = new List<double[]>();
            foreach (var imagePath in paths)
            {
                Bitmap original = new Bitmap(imagePath);
                Bitmap reduced = new Bitmap(original, new Size(NetworkOutputSquareSideResolution, NetworkOutputSquareSideResolution));

                imagesData.AddRange(GetImageVariations(reduced));

                original.Dispose();
                reduced.Dispose();
            }

            var testCost = output.SupervisedTrain(imagesData, imagesData, NeatNetwork.Libraries.Cost.CostFunctions.SquaredMean, learningRate, 0.05, 3, false);
            watch.Stop();

            if (showResultMessageBox)
                MessageBox.Show($"Training of a new autoencoder with {paths.Count} images and {imagesData.Count} images including modificated images in {watch.Elapsed.TotalMinutes} minutes with a test cost of {testCost}",
                    "Traning info", MessageBoxButtons.OK);

            return output;
        }

        private static int GetInputInt()
        {
            while (true)
            {
                try
                {
                    int output = Convert.ToInt32(Console.ReadLine());
                    return output;
                }
                catch (Exception)
                {
                    Console.WriteLine($"Didn't match format, please just enter an integer number.");
                }
            }
        }

        private static void RunNetworkExecutionInterface()
        {
            AbstractVideoGenerator.Program.Main();
        }
    }
}
