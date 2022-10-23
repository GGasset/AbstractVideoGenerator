using NeatNetwork;
using NeatNetwork.NetworkFiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Functionality.ImageProcessing;
using static Functionality.PathGetter;

namespace NetworkTrainer
{
    public class Program
    {
        public static int NetworkOutputSquareSideResolution = 50;

        [STAThread]
        public static void Main(string[] args)
        {
            while (true)
            {
                if (MessageBox.Show("Do you wish to train a network???", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (MessageBox.Show($"The current network output square side resolution is {NetworkOutputSquareSideResolution}, do you want to keep it???", "", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        Console.WriteLine("Select a resolution for the network output square side, valid inputs are numbers in this format: 20");
                        NetworkOutputSquareSideResolution = GetInputInt();
                        Console.WriteLine("\n");
                    }

                    int imageResolution = NetworkOutputSquareSideResolution * NetworkOutputSquareSideResolution;
                    int resolutionDataSize = imageResolution * 3;

                    int[] autoEncoderShape = new int[] { resolutionDataSize, 500, 150, 27, 150, 500, resolutionDataSize };

                    int[] generativeShape = new int[] { resolutionDataSize, 500, 150, 100, 50, 50, 250, 300, 500, resolutionDataSize };

                    int[] discriminativeShape = new int[] { resolutionDataSize, 500, 100, 20, 2, 1 };

                    int[] acceptedOptions = new int[] { 1, 2 };
                    int inputedOption = -1;
                    bool successfullySelectedOption = true;
                    do
                    {
                        try
                        {
                            Console.WriteLine("What type of network do you wish to train??\n\t1 - autoencoder\n\t2 - Gan");
                            inputedOption = Convert.ToInt32(Console.ReadLine());
                            successfullySelectedOption = acceptedOptions.Contains(inputedOption);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Please input an integer number");
                            successfullySelectedOption = false;
                        }
                    } while (!successfullySelectedOption);

                    Console.WriteLine("Enter learning rate value. The format must be one of these: 0.5 - .5 - 1");
                    double learningRate = GetInputDouble();

                    List<string> paths = new List<string>();
                    do
                    {
                        List<string> currentPaths = GetImagePathsFromFolder();
                        if (currentPaths != null)
                        {
                            paths.AddRange(currentPaths);
                            Console.WriteLine(GetFolderPathFromFilePath(currentPaths[0]));
                        }
                    } while (MessageBox.Show("Do you wish to add one more folder for training", "", MessageBoxButtons.YesNo) == DialogResult.Yes);

                    NN autoencoder = null;
                    switch (inputedOption)
                    {
                        case 1:
                            autoencoder = TrainAutoEncoderOnImages(paths, autoEncoderShape, learningRate, true);
                            break;
                    }

                    SaveFileDialog saveFileDialog = new SaveFileDialog()
                    {
                        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                        AddExtension = true,
                        Filter = "Text files (*.txt)|*.txt",
                        Title = "Select name and where you wish to save your networks",
                    };

                    while (saveFileDialog.ShowDialog() != DialogResult.OK);

                    string path = saveFileDialog.FileName;

                    string fileText = string.Empty;

                    List<Task<string>> networkToStringTasks = new List<Task<string>>();
                    switch (inputedOption)
                    {
                        case 1:
                            fileText += "autoencoder";
                            networkToStringTasks.Add(Task.Run(() => autoencoder.ToString()));
                            break;
                    }
                    fileText += "\nJGG\n";

                    foreach (var toStringTask in networkToStringTasks)
                    {
                        toStringTask.Wait();
                        fileText += toStringTask.Result;
                    }

                    File.WriteAllText(path, fileText);
                }

                RunNetworkExecutionInterface();
            }
        }

        private static NN TrainAutoEncoderOnImages(List<string> paths, int[] autoEncoderShape, double learningRate, bool showResultMessageBox)
        {
            var watch = Stopwatch.StartNew();

            Console.WriteLine("Creating network...");
            NN output = new NN(autoEncoderShape, NeatNetwork.Libraries.Activation.ActivationFunctions.Sigmoid);
            Console.WriteLine("Network created!");

            Console.WriteLine("Making modifications of the image and parsing all images...");
            List<double[]> imagesData = new List<double[]>();
            int i = 1;
            Console.WriteLine($"0/{paths.Count}");
            foreach (var imagePath in paths)
            {
                Bitmap original = new Bitmap(imagePath);
                Bitmap reduced = new Bitmap(original, new Size(NetworkOutputSquareSideResolution, NetworkOutputSquareSideResolution));

                imagesData.AddRange(GetImageVariations(reduced));

                original.Dispose();
                reduced.Dispose();
                if (i % 50 == 0)
                    Console.WriteLine($"{i}/{paths.Count}");
                i++;
            }
            Console.WriteLine("Finished making modifications of the image and parsing all images!");

            Console.WriteLine("Training network...");
            var testCost = output.SupervisedTrain(imagesData, imagesData, NeatNetwork.Libraries.Cost.CostFunctions.SquaredMean, learningRate, 0.05, 10, false);
            watch.Stop();

            if (showResultMessageBox)
                MessageBox.Show($"Training of a new autoencoder with {paths.Count} images and {imagesData.Count} images including modificated images in {watch.Elapsed.TotalMinutes} minutes with a test cost of {testCost}",
                    "Traning info", MessageBoxButtons.OK);

            return output;
        }

        private static string GetFolderPathFromFilePath(string folderPath)
        {
            return Path.GetDirectoryName(folderPath);
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

        private static double GetInputDouble()
        {
            while (true)
            {
                try
                {
                    string numberStr = Console.ReadLine();
                    if (numberStr.StartsWith("."))
                        numberStr = "0" + numberStr;
                    if (numberStr.Split('.').Length > 2)
                        throw new Exception();
                    double output = Convert.ToDouble(numberStr);
                    return output;
                }
                catch (Exception)
                {
                    Console.WriteLine("Didn't match specified format, please enter a decimal number in this format: 0.5 - .5 - 1");
                }
            }
        }

        private static void RunNetworkExecutionInterface()
        {
            AbstractVideoGenerator.Program.Main();
        }
    }
}
