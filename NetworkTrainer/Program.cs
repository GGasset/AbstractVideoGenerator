using NeatNetwork;
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
        private static int NetworkOutputSquareSideResolution = 50;

        public static void Main(string[] args)
        {
            while (true)
            {
                if (MessageBox.Show("Do you wish to train a network???") == DialogResult.Yes)
                {

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


        private static void RunNetworkExecutionInterface()
        {
            AbstractVideoGenerator.Program.Main();
        }
    }
}
