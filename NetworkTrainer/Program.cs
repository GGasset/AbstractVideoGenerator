using NeatNetwork;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Functionality.ImageProcessing;
using static Functionality.PathGetter;
using static Functionality.NetworkHolder;
using static AbstractVideoGenerator.Program;

namespace NetworkTrainer
{
    public class Program
    {
        public static int NetworkOutputSquareSideResolution = 24;

        [STAThread]
        public static void Main(string[] args)
        {
            PrepareApp();

            if (args.Length == 0)
            {
                if (MessageBox.Show("Do you wish to train a network???", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    NetworkBootCamp(false);
                }
            }
            else if (args[0] == "Train")
            {
                NetworkBootCamp(true);
            }

            while (true)
            {
                var dialogResult = MessageBox.Show("Do you wish to load a network?", "", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    dialogResult = MessageBox.Show("Do you wish to train the network?", "", MessageBoxButtons.YesNo);
                    LoadNN();
                    if (dialogResult == DialogResult.Yes)
                    {
                        Main(new string[] { "Train" });
                        return;
                    }

                    RunNetworkExecutionUI();
                }
                else
                {
                    dialogResult = MessageBox.Show("Do you wish to train a network?", "", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        Main(new string[0]);
                        return;
                    }
                }
            }

        }

        private static void NetworkBootCamp(bool trainExistingNetwork)
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
                    Console.WriteLine("Please input an accepted integer number");
                    successfullySelectedOption = false;
                }
            } while (!successfullySelectedOption);

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

            switch (inputedOption)
            {
                case 1:
                    TrainAutoEncoderOnImages(paths, autoEncoderShape, learningRate);
                    break;
                case 2:
                    TrainGanOnImages(paths, generativeShape, discriminativeShape, learningRate);
                    break;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                AddExtension = true,
                Filter = "Text files (*.txt)|*.txt",
                Title = "Select name and where you wish to save your networks",
            };

            while (saveFileDialog.ShowDialog() != DialogResult.OK) ;

            string path = saveFileDialog.FileName;

            string fileText = string.Empty;

            List<Task<string>> networkToStringTasks = new List<Task<string>>();
            switch (inputedOption)
            {
                case 0:
                    fileText += "Gan";
                    networkToStringTasks.Add(Task.Run(() => discriminative.ToString()));
                    networkToStringTasks.Add(Task.Run(() => generative.ToString()));
                    break;
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


            RunNetworkExecutionUI();
        }

        private static void TrainGanOnImages(List<string> paths, int[] generativeShape, int[] discriminativeShape, double learningRate, ReinforcementLearningNN reinforcementGenerative = null)
        {
            Console.WriteLine("How many training iterations (Epochs) you wish to make until training termination?");
            int epochs = GetInputInt();

            Console.WriteLine("On how many images does it needs to be trained at each iteration (epoch)?");
            int imagesPerEpoch = GetInputInt();

            if (reinforcementGenerative == null)
            {
                Console.WriteLine("Creating Generative network...");
                reinforcementGenerative = new ReinforcementLearningNN(new NN(generativeShape, NeatNetwork.Libraries.Activation.ActivationFunctions.Sigmoid), learningRate);
                Console.WriteLine("Creating Discriminative network...");
                discriminative = new NN(discriminativeShape, NeatNetwork.Libraries.Activation.ActivationFunctions.Sigmoid);
                Console.WriteLine("Networks created!");
            }

            List<double[]> images = ExpandImages(paths);

            Console.WriteLine("Generating labels...");
            List<double[]> discriminativeRealY = new List<double[]>();
            for (int i = 0; i < images.Count; i++)
                discriminativeRealY.Add(new double[] { 1 });

            List<double[]> discriminativeFakeY = new List<double[]>();
            for (int i = 0; i < imagesPerEpoch; i++)
                discriminativeFakeY.Add(new double[] { 0 });

            Console.WriteLine("Labels generated.");

            Random r = new Random(DateTime.Now.Millisecond);
            var watch = new Stopwatch();
            int totalSeconds = 0;
            int batchLength = 6;
            for (int i = 0; i < epochs; i++)
            {
                watch.Restart();
                watch.Start();
                Console.WriteLine("Training Discriminative on Real data... " + i);
                for (int j = 0; j < (int)Math.Ceiling((double)imagesPerEpoch / batchLength); j++)
                {
                    discriminative.SupervisedLearningBatch(images, discriminativeRealY, batchLength, NeatNetwork.Libraries.Cost.CostFunctions.SquaredMean, learningRate);
                    Console.WriteLine($"{j * batchLength}/{imagesPerEpoch}");
                }

                Console.WriteLine("Creating Gaussian noise images... " + i);
                List<double[]> gaussianNoiseImages = GenerateGaussianNoiseImages(0.5, .15, imagesPerEpoch, discriminative.Shape[0]);

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
                var timeWhenFinished = DateTime.Now.AddSeconds((totalSeconds / i) * (epochs - i));
                Console.WriteLine($"Epoch {i} finished! Training will be completed by {Enum.GetName(typeof(DayOfWeek), timeWhenFinished.DayOfWeek)}, {timeWhenFinished.Hour}:{timeWhenFinished.Minute}h");
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

        private static List<double[]> ExpandImages(List<string> imagePaths)
        {
            Console.WriteLine("Expanding and parsing image data...");
            List<double[]> imagesData = new List<double[]>();
            Console.WriteLine($"0/{imagePaths.Count}");
            int i = 1;
            foreach (var imagePath in imagePaths)
            {
                Bitmap original = new Bitmap(imagePath);
                Bitmap reduced = new Bitmap(original, new Size(NetworkOutputSquareSideResolution, NetworkOutputSquareSideResolution));

                imagesData.AddRange(GetImageVariations(reduced));

                original.Dispose();
                reduced.Dispose();
                if (i % 50 == 0)
                    Console.WriteLine($"{i}/{imagePaths.Count}");
                i++;
            }
            Console.WriteLine("Finished expanding and parsing image data!");
            return imagesData;
        }

        private static void LoadNN()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select a .txt file generated by this app to load your NNs",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "Text files (*.txt)|*.txt",
                Multiselect = false,
            };

            while (openFileDialog.ShowDialog() != DialogResult.OK) ;

            var filePath = openFileDialog.FileName;

            string text = string.Empty;
            using (StreamReader reader = new StreamReader(filePath))
            {
                text = reader.ReadToEnd();
            }

            string[] headerContent = text.Split(new string[] { "\nJGG\n" }, StringSplitOptions.None);
            string header = headerContent[0];
            string content = headerContent[1];

            string[] networkStrs = content.Split(new string[] { "\n====\n" }, StringSplitOptions.None);
            List<Task<NN>> NNTasks = new List<Task<NN>>();

            switch (header)
            {
                case "autoencoder":
                    NNTasks.Add(Task.Run(() => new NN(networkStrs[0])));
                    break;
                case "Gan":
                    NNTasks.Add(Task.Run(() => new NN(networkStrs[0])));
                    NNTasks.Add(Task.Run(() => new NN(networkStrs[1])));
                    break;
                default:
                    var dialogResult = MessageBox.Show("This file wasn't generated by this app and thus is incompatible. Consider restarting the app and selecting a valid Neural Network or training one.", "Error", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.No)
                    {
                        Console.WriteLine("Sorry, it is the way it is.");
                    }
                    return;
            }

            foreach (var networkTask in NNTasks)
            {
                networkTask.Wait();
            }

            if (header == "autoencoder")
            {
                autoencoder = NNTasks[0].Result;
            }
            else
            {
                discriminative = NNTasks[0].Result;
                generative = NNTasks[1].Result;
            }
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