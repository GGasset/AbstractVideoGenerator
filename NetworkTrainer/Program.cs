using NeatNetwork;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static AbstractVideoGenerator.Program;
using static Functionality.ImageProcessing;
using static Functionality.NetworkHolder;
using static Functionality.PathGetter;

namespace NetworkTrainer
{
    public class Program
    {
        public static int NetworkOutputSquareSideResolution = 24;

        [STAThread]
        public static void Main(string[] args)
        {
            PrepareUI();

            if (args?.Length == 0 || args == null)
            {
                args = null;
                if (MessageBox.Show("Do you wish to train a network???", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    NetworkBootCamp(args);
                }
            }
            else
            {
                switch (args[0])
                {
                    case "Train existing":
                        NetworkBootCamp(args);
                        break;
                    case "Accepted":
                        NetworkBootCamp(args);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            Update();
        }

        private static void Update()
        {
            while (true)
            {
                var dialogResult = MessageBox.Show("Do you wish to load a network?", "", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    dialogResult = MessageBox.Show("Do you wish to train the network?", "", MessageBoxButtons.YesNo);
                    LoadNN();
                    if (dialogResult == DialogResult.Yes)
                    {
                        Main(new string[] { "Train existing" });
                        return;
                    }

                    RunNetworkExecutionUI();
                }
                else
                {
                    dialogResult = MessageBox.Show("Do you wish to train a network?", "", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        Main(new string[] { "Accepted" });
                        return;
                    }
                }
            }
        }

        private static void NetworkBootCamp(string[] args)
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

            int GanInputLength = 25;
            int[] generativeShape = new int[] { GanInputLength, 50, 75, 100, 150, 200, 250, 500, 1000, resolutionDataSize };

            int[] discriminativeShape = new int[] { resolutionDataSize, 500, 100, 20, 2, 1 };

            int[] reverseDiffusorShape = new int[] { resolutionDataSize, 150, 200, 300, 400, resolutionDataSize };

            int selectedInputOption = -1;
            bool successfullySelectedOption;
            if (args?[0] != "Train existing")
            {
                string[] names = Enum.GetNames(typeof(NetworkType));
                int[] values = (int[])Enum.GetValues(typeof(NetworkType));
                do
                {
                    try
                    {
                        Console.WriteLine("What type of network do you wish to train??");
                        for (int i = 0; i < names.Length; i++)
                        {
                            Console.WriteLine($"\t{values[i]} - {names[i]}");
                        }
                        selectedInputOption = Convert.ToInt32(Console.ReadLine());
                        successfullySelectedOption = values.Contains(selectedInputOption);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Please input an accepted integer number");
                        successfullySelectedOption = false;
                    }
                } while (!successfullySelectedOption);
                loadedNetwork = (NetworkType)selectedInputOption;
            }

            Console.WriteLine("Enter learning rate value. The format must be one of these: 0,5 - ,5 - 1");
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

            switch (loadedNetwork)
            {
                case NetworkType.autoencoder:
                    TrainAutoEncoderOnImages(paths, autoEncoderShape, learningRate);
                    break;

                case NetworkType.Gans:
                    TrainGanOnImages(paths, generativeShape, discriminativeShape, learningRate);
                    break;

                case NetworkType.ReverseDiffusor:
                    TrainReverseDiffusorOnImages(paths, reverseDiffusorShape, learningRate);
                    break;

                default:
                    throw new NotImplementedException();
            }

            SaveNN();

            RunNetworkExecutionUI();
        }

        private static void TrainReverseDiffusorOnImages(List<string> paths, int[] reverseDiffusorShape, double learningRate)
        {
            Console.WriteLine("How many diffusion per image?");
            int diffusedImagesPerImage = GetInputInt();
            reverseDiffusorDiffusions = diffusedImagesPerImage;

            Console.WriteLine("On how many images does it need to be trained per epoch? (Recommended a low number like 30)");
            int totalImagesPerEpoch = GetInputInt();

            Console.WriteLine("How many epochs? (Recommended a high number like 3000)");
            int epochs = GetInputInt();

            Console.WriteLine("Creating network...");
            reverseDiffusor = new NN(reverseDiffusorShape, NeatNetwork.Libraries.Activation.ActivationFunctions.Sigmoid);

            Console.WriteLine("Expanding images...");
            List<double[]> images = ExpandImages(paths);

            var watch = new Stopwatch();
            Random r = new Random(DateTime.Now.Millisecond);

            int totalSeconds = 0;
            for (int i = 0; i < epochs; i++)
            {
                watch.Restart();
                watch.Start();
                Console.WriteLine("Doing diffusion on images.. " + i);
                // List of images containing diffused images array of arrays
                List<double[][]> trainingImages = new List<double[][]>();
                for (int j = 0; j < totalImagesPerEpoch; j++)
                {
                    trainingImages.Add(new double[diffusedImagesPerImage][]);

                    // Original image
                    trainingImages[j][0] = images[r.Next(images.Count)];
                    for (int k = 1; k < diffusedImagesPerImage; k++)
                    {
                        //Add diffused image
                        trainingImages[j][k] = ApplyGaussianNoiseToImageData(trainingImages[j][k - 1], 0, .15);
                    }

                    if (j % 10 == 0)
                        Console.WriteLine($"{j * diffusedImagesPerImage}/{totalImagesPerEpoch * diffusedImagesPerImage}");
                }

                Console.WriteLine("Parsing data... " + i);
                List<double[]> X = new List<double[]>();
                List<double[]> Y = new List<double[]>();
                for (int j = 0; j < totalImagesPerEpoch; j++)
                {
                    for (int k = diffusedImagesPerImage - 1; k >= 1; k--)
                    {
                        X.Add(trainingImages[j][k]);
                        Y.Add(trainingImages[j][k - 1]);
                    }
                    if (j % 10 == 0)
                    {
                        Console.WriteLine($"{j}/{totalImagesPerEpoch}");
                    }
                }
                trainingImages.Clear();

                Console.WriteLine("Training network... " + i);
                reverseDiffusor.SupervisedTrain(X, Y, NeatNetwork.Libraries.Cost.CostFunctions.SquaredMean, learningRate, 0, 6, false);

                watch.Stop();
                totalSeconds += watch.Elapsed.Seconds;
                int secondsToAdd = totalSeconds / (i + 1) * (epochs - i);
                DateTime finishingTime = DateTime.Now.AddSeconds(secondsToAdd);
                Console.WriteLine($"Finished epoch {i}! Training will be finished by {Enum.GetName(typeof(DayOfWeek), finishingTime.DayOfWeek)}, {finishingTime.Day} at {finishingTime.Hour}:{finishingTime.Minute}H");
            }
        }

        private static void TrainGanOnImages(List<string> paths, int[] generativeShape, int[] discriminativeShape, double learningRate)
        {
            Console.WriteLine("How many training iterations (Epochs) you wish to make until training termination?");
            int epochs = GetInputInt();

            Console.WriteLine("On how many images does it needs to be trained at each iteration (epoch)?");
            int imagesPerEpoch = GetInputInt();

            ReinforcementLearningNN reinforcementGenerative;
            if (generative == null)
            {
                Console.WriteLine("Creating Generative network...");
                reinforcementGenerative = new ReinforcementLearningNN(new NN(generativeShape, NeatNetwork.Libraries.Activation.ActivationFunctions.Sigmoid), learningRate);
                Console.WriteLine("Creating Discriminative network...");
                discriminative = new NN(discriminativeShape, NeatNetwork.Libraries.Activation.ActivationFunctions.Sigmoid);
                Console.WriteLine("Networks created!");
            }
            else
            {
                reinforcementGenerative = new ReinforcementLearningNN(generative, learningRate);
            }

            List<double[]> images = ExpandImages(paths);

            Console.WriteLine("Generating labels...");
            List<double[]> discriminativeRealY = new List<double[]>();
            for (int i = 0; i < images.Count; i++)
                discriminativeRealY.Add(new double[] { 1 });

            List<double[]> discriminativeFakeY = new List<double[]>();
            for (int i = 0; i < imagesPerEpoch; i++)
                discriminativeFakeY.Add(new double[] { 0 });

            Random r = new Random(DateTime.Now.Millisecond);
            var watch = new Stopwatch();
            int totalSeconds = 0;
            int batchLength = 6;
            for (int i = 0; i < epochs; i++)
            {
                watch.Restart();
                watch.Start();
                Console.WriteLine("Training Discriminative on real data... " + i);
                for (int j = 0; j < (int)Math.Ceiling((double)imagesPerEpoch / batchLength); j++)
                {
                    discriminative.SupervisedLearningBatch(images, discriminativeRealY, batchLength, NeatNetwork.Libraries.Cost.CostFunctions.SquaredMean, learningRate);
                    Console.WriteLine($"{j * batchLength}/{imagesPerEpoch}");
                }

                Console.WriteLine("Creating Gaussian noise images... " + i);
                List<double[]> gaussianNoiseImages = GenerateGaussianNoiseImages(0.5, .15, imagesPerEpoch, reinforcementGenerative.n.Shape[0]);

                Console.WriteLine("Generative Generating images and Discriminative still discriminating... " + i);
                List<double[]> generatedImages = new List<double[]>();

                for (int j = 0; j < imagesPerEpoch; j++)
                {
                    double[] generatedImage;
                    generatedImages.Add(generatedImage = reinforcementGenerative.Execute(gaussianNoiseImages[j]));
                    double discriminativeActivation = discriminative.Execute(generatedImage)[0];
                    reinforcementGenerative.SetLastReward(GetReward(discriminativeActivation));

                    if (j % 5 == 0)
                    {
                        Console.WriteLine($"{j}/{imagesPerEpoch}");
                    }
                    double GetReward(double discriminativeOutput)
                    {
                        return discriminativeOutput * 100;
                    }
                }
                Console.WriteLine("Generative upgrading itself to another level.. " + i);
                reinforcementGenerative.TerminateAgent();

                Console.WriteLine("Training Discriminative on Fake data... " + i);
                discriminative.SupervisedTrain(generatedImages, discriminativeFakeY, NeatNetwork.Libraries.Cost.CostFunctions.SquaredMean, learningRate, 0, batchLength, false);

                watch.Stop();
                totalSeconds += watch.Elapsed.Seconds;
                var finishingTime = DateTime.Now.AddSeconds(totalSeconds / (i + 1) * (epochs - i));
                Console.WriteLine($"Epoch {i} finished! Training will be completed by {Enum.GetName(typeof(DayOfWeek), finishingTime.DayOfWeek)}, {finishingTime.Day} at {finishingTime.Hour}:{finishingTime.Minute}H");
            }

            generative = reinforcementGenerative.n;
        }

        private static void TrainAutoEncoderOnImages(List<string> paths, int[] autoEncoderShape, double learningRate)
        {
            Console.WriteLine("Enter maximum test cost for termination. Value must be in this format: 0,15 - ,15 - 0");
            double maximumTestCost = GetInputDouble();
            Console.WriteLine();
            Console.Write("Max epochs: ");
            int maxEpochs = GetInputInt();
            Console.WriteLine();

            var watch = Stopwatch.StartNew();

            if (autoencoder == null)
            {
                Console.WriteLine("Creating network...");
                autoencoder = new NN(autoEncoderShape, NeatNetwork.Libraries.Activation.ActivationFunctions.Sigmoid);
                Console.WriteLine("Network created!");
            }

            List<double[]> imagesData = ExpandImages(paths);

            Console.WriteLine("Training network...");

            double testCost = 10E30;
            int epochCounter = 0;
            while (maximumTestCost < testCost && epochCounter < maxEpochs)
            {
                testCost = autoencoder.SupervisedTrain(imagesData, imagesData, NeatNetwork.Libraries.Cost.CostFunctions.SquaredMean, learningRate, 0.05, 10, true);
                epochCounter++;
                Console.WriteLine($"Finished iteration {epochCounter} with a test cost of {testCost}");
            }
            watch.Stop();

            MessageBox.Show($"Training of a new autoencoder with {paths.Count} images and {imagesData.Count} images including modificated images in {watch.Elapsed.TotalMinutes} minutes with a test cost of {testCost} after {epochCounter} iterations",
                "Traning info", MessageBoxButtons.OK);
        }

        private static List<double[]> ExpandImages(List<string> imagePaths, bool isGreyScale = false)
        {
            Console.WriteLine("Expanding and parsing image data...");
            List<double[]> imagesData = new List<double[]>();
            Console.WriteLine($"0/{imagePaths.Count}");
            int i = 1;
            foreach (var imagePath in imagePaths)
            {
                Bitmap original = new Bitmap(imagePath);
                Bitmap reduced = new Bitmap(original, new Size(NetworkOutputSquareSideResolution, NetworkOutputSquareSideResolution));

                imagesData.AddRange(GetImageVariations(reduced, isGreyScale));

                original.Dispose();
                reduced.Dispose();
                if (i % 50 == 0)
                    Console.WriteLine($"{i}/{imagePaths.Count}");
                i++;
            }
            Console.WriteLine("Finished expanding and parsing image data!");
            return imagesData;
        }

        private static void SaveNN()
        {
            List<Task<string>> networkToStringTasks = new List<Task<string>>();
            switch (loadedNetwork)
            {
                case NetworkType.autoencoder:
                    Task<string> autoencoderTask = Task.Run(() => autoencoder.ToString());

                    string autoencoderPath = GetSavePath("Autoencoder");

                    autoencoderTask.Wait();

                    File.WriteAllText(autoencoderPath, autoencoderTask.Result);
                    break;

                case NetworkType.Gans:
                    Task<string> discriminativeTask = Task.Run(() => discriminative.ToString());
                    Task<string> generativeTask = Task.Run(() => generative.ToString());

                    string generativePath = GetSavePath("Generative");
                    string discriminativePath = GetSavePath("Discriminative");

                    discriminativeTask.Wait();
                    generativeTask.Wait();

                    File.WriteAllText(generativePath, discriminativeTask.Result);
                    File.WriteAllText(discriminativePath, discriminativeTask.Result);
                    break;

                case NetworkType.ReverseDiffusor:
                    Task<string> reverseDiffusorTask = Task.Run(() => reverseDiffusor.ToString());

                    string reverseDiffusorPath = GetSavePath("Stable diffusion");

                    reverseDiffusorTask.Wait();

                    File.WriteAllText(reverseDiffusorPath, reverseDiffusorTask.Result);
                    break;

                default:
                    throw new NotImplementedException();
            }

        }

        private static void LoadNN()
        {
            autoencoder = null;
            generative = null;
            discriminative = null;
            reverseDiffusor = null;
            loadedNetwork = AskForNetworkType();

            string[] networksNames;
            switch (loadedNetwork)
            {
                case NetworkType.autoencoder:
                    networksNames = new string[] { "Autoencoder" };
                    break;
                case NetworkType.Gans:
                    networksNames = new string[] {"Generative", "Discriminative"};
                    break;
                case NetworkType.ReverseDiffusor:
                    networksNames = new string[] {"Stable diffusion"};
                    break;
                case NetworkType.DiffusionSubtractor:
                    networksNames = new string[] {"Stable diffusion noise prediction"};
                    break;
                default:
                    throw new NotImplementedException();
            }

            string[] networksPaths = new string[networksNames.Length];
            StreamReader[] readers = new StreamReader[networksNames.Length];
            for (int i = 0; i < networksNames.Length; i++)
            {
                networksPaths[i] = GetSavePath(networksNames[i], action: "load");
                readers[i] = new StreamReader(networksPaths[i]);
            }

            switch (loadedNetwork)
            {
                case NetworkType.autoencoder:
                    autoencoder = new NN(readers[0]);
                    break;
                case NetworkType.Gans:
                    generative = new NN(readers[0]);
                    discriminative = new NN(readers[1]);
                    break;
                case NetworkType.ReverseDiffusor:
                    reverseDiffusor = new NN(readers[0]);
                    break;
                case NetworkType.DiffusionSubtractor:
                    diffusionSubtractor = new NN(readers[0]);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private static string GetSavePath(string networkName, string action = "save")
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                AddExtension = true,
                Filter = "Text files (*.txt)|*.txt",
                Title = $"Select name and where you wish to {action} the {networkName} network",
            };

            while (saveFileDialog.ShowDialog() != DialogResult.OK) ;

            string path = saveFileDialog.FileName;

            return path;
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
                    numberStr = numberStr.Replace('.', ',');
                    if (numberStr.StartsWith(","))
                        numberStr = "0" + numberStr;
                    if (numberStr.Split(',').Length > 2)
                        throw new Exception();
                    double output = Convert.ToDouble(numberStr);
                    return output;
                }
                catch (Exception)
                {
                    Console.WriteLine("Didn't match specified format, please enter a decimal number in this format: 0,5 - ,5 - 1");
                }
            }
        }

        private static void RunNetworkExecutionUI()
        {
            AbstractVideoGenerator.Program.Main();
        }
    }
}