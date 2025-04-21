using ForestNET.Lib.AI;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Sandbox.Tests.AI
{
    public class AITest
    {
        public static void TestAIMenu(string p_s_currentDirectory)
        {
            string s_currentDirectory = p_s_currentDirectory;
            string s_resourcesAIDirectory = s_currentDirectory + "resources" + ForestNET.Lib.IO.File.DIR + "ai" + ForestNET.Lib.IO.File.DIR;
            string s_resourcesMNISTDirectory = s_currentDirectory + "resources" + ForestNET.Lib.IO.File.DIR + "mnist" + ForestNET.Lib.IO.File.DIR;
            double d_trainingSetQuota = 0.9; /* 90% training data */
            double d_generalizationSetQuota = 0.1; /* 10% generalization data */
            bool b_useRectifier = false;
            bool b_useBatch = false;
            int i_input;

            do
            {
                Console.WriteLine("++++++++++++++++++++++++++++++++");
                Console.WriteLine("+ test AI                      +");
                Console.WriteLine("++++++++++++++++++++++++++++++++");

                Console.WriteLine("");

                Console.WriteLine("[1] create training set from MNIST data (10k records)");
                Console.WriteLine("[2] create training set from MNIST data (60k records)");
                Console.WriteLine("[3] train neural network with 10k MNIST records and 3 layers(784, 320, 10)");
                Console.WriteLine("[4] train neural network with 60k MNIST records and 3 layers(784, 320, 10)");
                Console.WriteLine("[5] get neural network accuracy with 10k MNIST records");
                Console.WriteLine("[6] get neural network accuracy with 60k MNIST records");
                Console.WriteLine("[7] test neural network with random record out of 10k MNIST records");
                Console.WriteLine("[8] test neural network with own test image(test.png - 128x128 px - 8bit depth)");
                Console.WriteLine("[9] create MNIST sample output (1000 images)");
                Console.WriteLine("[0] quit");

                Console.WriteLine("");

                i_input = ForestNET.Lib.Console.ConsoleInputInteger("Enter menu number[1-9;0]: ", "Invalid input.", "Please enter a value.") ?? 0;

                Console.WriteLine("");

                if (i_input == 1)
                {
                    AITest.CreateTrainingSetFromMNISTData(s_resourcesMNISTDirectory + "t10k-images-idx3-ubyte.gz", s_resourcesMNISTDirectory + "t10k-labels-idx1-ubyte.gz", s_resourcesAIDirectory + "trainingDataSet_10k.txt", -1);
                }
                else if (i_input == 2)
                {
                    AITest.CreateTrainingSetFromMNISTData(s_resourcesMNISTDirectory + "train-images-idx3-ubyte.gz", s_resourcesMNISTDirectory + "train-labels-idx1-ubyte.gz", s_resourcesAIDirectory + "trainingDataSet_60k.txt", -1);
                }
                else if (i_input == 3)
                {
                    AITest.TrainAI([784, 320, 10], b_useRectifier, b_useBatch, s_resourcesAIDirectory + "neuralNetwork.txt", s_resourcesAIDirectory + "trainingDataSet_10k.txt", d_trainingSetQuota, d_generalizationSetQuota);
                }
                else if (i_input == 4)
                {
                    AITest.TrainAI([784, 320, 10], b_useRectifier, b_useBatch, s_resourcesAIDirectory + "neuralNetwork.txt", s_resourcesAIDirectory + "trainingDataSet_60k.txt", d_trainingSetQuota, d_generalizationSetQuota);
                }
                else if (i_input == 5)
                {
                    Console.WriteLine("");

                    double d_input = ForestNET.Lib.Console.ConsoleInputDouble("Enter target accuracy(e.g. 80.0): ", "Invalid input.", "Please enter a value.") ?? 0.0d;

                    Console.WriteLine("");

                    AITest.GetAIAccuracy([784, 320, 10], b_useRectifier, s_resourcesAIDirectory + "neuralNetwork.txt", s_resourcesAIDirectory + "trainingDataSet_10k.txt", d_trainingSetQuota, d_generalizationSetQuota, d_input);
                }
                else if (i_input == 6)
                {
                    Console.WriteLine("");

                    double d_input = ForestNET.Lib.Console.ConsoleInputDouble("Enter target accuracy(e.g. 80.0): ", "Invalid input.", "Please enter a value.") ?? 0.0d;

                    Console.WriteLine("");

                    AITest.GetAIAccuracy([784, 320, 10], b_useRectifier, s_resourcesAIDirectory + "neuralNetwork.txt", s_resourcesAIDirectory + "trainingDataSet_60k.txt", d_trainingSetQuota, d_generalizationSetQuota, d_input);
                }
                else if (i_input == 7)
                {
                    AITest.TestAIRandom(s_resourcesMNISTDirectory + "t10k-images-idx3-ubyte.gz", s_resourcesMNISTDirectory + "t10k-labels-idx1-ubyte.gz", 10000, [784, 320, 10], s_resourcesAIDirectory + "neuralNetwork.txt");
                }
                else if (i_input == 8)
                {
                    AITest.TestAI(s_currentDirectory + ForestNET.Lib.IO.File.DIR + "test.png", 128, 128, 28, [784, 320, 10], s_resourcesAIDirectory + "neuralNetwork.txt");
                }
                else if (i_input == 9)
                {
                    AITest.CreateMNISTOutput(s_resourcesMNISTDirectory + "train-images-idx3-ubyte.gz", s_resourcesMNISTDirectory + "train-labels-idx1-ubyte.gz", s_currentDirectory + ForestNET.Lib.IO.File.DIR + "samplesMNIST" + ForestNET.Lib.IO.File.DIR);
                }

                if ((i_input >= 1) && (i_input <= 9))
                {
                    Console.WriteLine("");

                    _ = ForestNET.Lib.Console.ConsoleInputString("Press any key to continue . . . ", true);

                    Console.WriteLine("");
                }

                Console.WriteLine("");

            } while (i_input != 0);
        }

        public static void CreateMNISTOutput(string p_s_filePathImage, string p_s_filePathLabel, string p_s_testDirectory)
        {
            if (ForestNET.Lib.IO.File.FolderExists(p_s_testDirectory))
            {
                ForestNET.Lib.IO.File.DeleteDirectory(p_s_testDirectory);
            }

            ForestNET.Lib.IO.File.CreateDirectory(p_s_testDirectory);

            MNISTHandling o_mnistHandling = new(p_s_filePathImage, p_s_filePathLabel);

            int i = 0;

            while (o_mnistHandling.HasNext)
            {
                if (i % 1000 == 0)
                {
                    Console.WriteLine(i);
                }

                o_mnistHandling.SelectNext();

                if (i < 1000)
                {
                    byte by_label = o_mnistHandling.CurrentLabelValue;
                    byte[] a_data = o_mnistHandling.CurrentImageData;

                    if (i < 50)
                    {
                        Console.WriteLine(i + " -> " + by_label);
                    }

                    if (i < 10)
                    {
                        byte[] a_binary = o_mnistHandling.GetDataAsBinaryArray(a_data);
                        int i_index = 0;

                        for (int y = 0; y < o_mnistHandling.ImageHeight; y++)
                        {
                            for (int x = 0; x < o_mnistHandling.ImageWidth; x++, i_index++)
                            {
                                if (a_binary[i_index] == 0)
                                {
                                    Console.Write("  ");
                                }
                                else
                                {
                                    Console.Write("8 ");
                                }
                            }

                            Console.Write(ForestNET.Lib.IO.File.NEWLINE);
                        }
                    }

                    using (Image<SixLabors.ImageSharp.PixelFormats.L8> o_image = new(o_mnistHandling.ImageWidth, o_mnistHandling.ImageHeight))
                    {
                        int i_index = 0;

                        for (int y = 0; y < o_mnistHandling.ImageHeight; y++)
                        {
                            for (int x = 0; x < o_mnistHandling.ImageWidth; x++, i_index++)
                            {
                                byte data = a_data[i_index];
                                int i_gray = 255 - (((int)data) & 0xFF);
                                int i_rgb = i_gray | (i_gray << 8) | (i_gray << 16);

                                o_image[x, y] = Color.FromRgb((byte)i_rgb, (byte)i_rgb, (byte)i_rgb).ToPixel<SixLabors.ImageSharp.PixelFormats.L8>();
                            }
                        }

                        o_image.SaveAsPng(p_s_testDirectory + "img" + by_label + "." + i + ".png");
                    }
                }

                i++;
            }

            Console.WriteLine(i);
        }

        public static byte[] ImageToBinaryPixelArray(string p_s_filePath, int p_i_height, int p_i_width, int p_i_destinationDimension)
        {
            if (!ForestNET.Lib.IO.File.Exists(p_s_filePath))
            {
                throw new System.IO.IOException("file '" + p_s_filePath + "' does not exists");
            }

            sbyte[] a_pixels = [];
            int i_pixelIndex = 1;

            using (Image<SixLabors.ImageSharp.PixelFormats.L8> o_image = Image.Load<SixLabors.ImageSharp.PixelFormats.L8>(p_s_filePath))
            {
                /* I dont know why, but we must flip and rotate the image */
                o_image.Mutate(x => x.RotateFlip(RotateMode.Rotate90, FlipMode.Horizontal));

                a_pixels = new sbyte[o_image.Width * o_image.Height];

                for (int x = 0; x < o_image.Width; x++)
                {
                    for (int y = 0; y < o_image.Height; y++)
                    {
                        SixLabors.ImageSharp.PixelFormats.Rgba32 o_pixel = new();
                        o_image[x, y].ToRgba32(ref o_pixel);
                        /* r, g and b value are equal in 8bit grayscale */
                        a_pixels[i_pixelIndex - 1] = (sbyte)o_pixel.R;

                        i_pixelIndex++;
                    }
                }
            }

            if (p_i_width != p_i_destinationDimension)
            {
                a_pixels = SquareScaling(a_pixels, p_i_width, p_i_height, p_i_destinationDimension);
            }

            byte[] a_return = new byte[p_i_destinationDimension * p_i_destinationDimension];

            int k = 0;

            for (int i = 0; i < p_i_destinationDimension; i++)
            {
                for (int j = 0; j < p_i_destinationDimension; j++)
                {

                    if (a_pixels[k++] <= 0)
                    {
                        a_return[(k - 1)] = 0;
                    }
                    else
                    {
                        a_return[(k - 1)] = 1;
                    }
                }
            }

            return a_return;
        }

        public static void PrintBinaryPixelArray(byte[] p_a_pixels, int p_i_height, int p_i_width)
        {
            int k = 0;

            for (int i = 0; i < p_i_width; i++)
            {
                Console.Write("- ");
            }

            Console.Write(ForestNET.Lib.IO.File.NEWLINE);

            for (int i = 0; i < p_i_height; i++)
            {
                for (int j = 0; j < p_i_width; j++)
                {
                    if (p_a_pixels[k++] == 0)
                    {
                        Console.Write(" ");
                    }
                    else
                    {
                        Console.Write("8 ");
                    }
                }

                Console.Write(ForestNET.Lib.IO.File.NEWLINE);
            }

            for (int i = 0; i < p_i_width; i++)
            {
                Console.Write("- ");
            }

            Console.Write(ForestNET.Lib.IO.File.NEWLINE);
        }

        public static sbyte[] SquareScaling(sbyte[] p_a_sourceImagePixelByteArray, int p_i_sourceWidth, int p_i_sourceHeight, int p_i_destinationDimension)
        {
            sbyte[] a_return = new sbyte[(p_i_destinationDimension * p_i_destinationDimension) + 1];

            double xFactor = (double)p_i_sourceWidth / (double)p_i_destinationDimension;
            double yFactor = (double)p_i_sourceHeight / (double)p_i_destinationDimension;

            double fraction_x, fraction_y, minus1_x, minus1_y;
            int top_x, top_y, bottom_x, bottom_y;

            sbyte g11, g12, g21, g22;
            sbyte g1, g2;
            sbyte gray;

            for (int x = 0; x < p_i_destinationDimension; ++x)
            {
                for (int y = 0; y < p_i_destinationDimension; ++y)
                {
                    bottom_x = (int)Math.Floor(x * xFactor);
                    bottom_y = (int)Math.Floor(y * yFactor);

                    top_x = bottom_x + 1;

                    if (top_x >= p_i_sourceWidth)
                        top_x = bottom_x;

                    top_y = bottom_y + 1;

                    if (top_y >= p_i_sourceHeight)
                        top_y = bottom_y;

                    /* determine proportions */
                    fraction_x = x * xFactor - bottom_x;
                    fraction_y = y * yFactor - bottom_y;
                    minus1_x = 1.0 - fraction_x;
                    minus1_y = 1.0 - fraction_y;

                    /* extract pixel */
                    g11 = p_a_sourceImagePixelByteArray[bottom_x + 1 + (bottom_y * p_i_sourceWidth)];
                    g12 = p_a_sourceImagePixelByteArray[top_x + 1 + (bottom_y * p_i_sourceWidth)];
                    g21 = p_a_sourceImagePixelByteArray[bottom_x + 1 + (top_y * p_i_sourceWidth)];

                    /* check for out of bound */
                    if (top_x + 1 + (top_y * p_i_sourceWidth) < (p_i_sourceHeight * p_i_sourceWidth))
                    {
                        g22 = p_a_sourceImagePixelByteArray[top_x + 1 + (top_y * p_i_sourceWidth)];
                    }
                    else
                    {
                        g22 = p_a_sourceImagePixelByteArray[(p_i_sourceHeight * p_i_sourceWidth) - 1];
                    }

                    /* mix pixel */
                    g1 = (sbyte)(minus1_x * g11 + fraction_x * g12);
                    g2 = (sbyte)(minus1_x * g21 + fraction_x * g22);

                    gray = (sbyte)(minus1_y * (double)(g1) + fraction_y * (double)(g2));

                    /* set pixel */
                    a_return[x + 1 + (y * p_i_destinationDimension)] = gray;
                }
            }

            /* upper left corner pixel scaling error */
            a_return[0] = -1;

            return a_return;
        }

        public static void CreateTrainingSetFromMNISTData(string p_s_filePathImage, string p_s_filePathLabel, string p_s_filePathTrainingSet, int p_i_maxAmount)
        {
            if (!ForestNET.Lib.IO.File.Exists(p_s_filePathImage))
            {
                throw new Exception("MNIST image file[" + p_s_filePathImage + "] does not exists");
            }

            if (!ForestNET.Lib.IO.File.Exists(p_s_filePathLabel))
            {
                throw new Exception("MNIST label file[" + p_s_filePathLabel + "] does not exists");
            }

            ForestNET.Lib.IO.File o_fileTrainingSet = new(p_s_filePathTrainingSet, (!ForestNET.Lib.IO.File.Exists(p_s_filePathTrainingSet)));
            o_fileTrainingSet.TruncateContent();

            MNISTHandling o_mnistHandling = new(p_s_filePathImage, p_s_filePathLabel);

            ForestNET.Lib.ConsoleProgressBar o_consoleProgressBar = new();
            o_consoleProgressBar.Init("Creating training data set file . . .", "Training data set file created.");

            int i = 0;

            while (o_mnistHandling.HasNext)
            {
                if (i % 100 == 0)
                {
                    o_consoleProgressBar.Report = (double)i / o_mnistHandling.Amount;
                }

                /* read data from MNIST */
                o_mnistHandling.SelectNext();

                if ((p_i_maxAmount >= 0) && (i >= p_i_maxAmount))
                {
                    break;
                }

                byte by_label = o_mnistHandling.CurrentLabelValue;
                byte[] a_data = o_mnistHandling.CurrentImageData;

                System.Text.StringBuilder o_stringBuilder = new();

                byte[] a_binary = o_mnistHandling.GetDataAsBinaryArray(a_data);
                int i_index = 0;

                for (int y = 0; y < o_mnistHandling.ImageHeight; y++)
                {
                    for (int x = 0; x < o_mnistHandling.ImageWidth; x++, i_index++)
                    {
                        if (a_binary[i_index] == 0)
                        {
                            o_stringBuilder.Append("0.0;");
                        }
                        else
                        {
                            o_stringBuilder.Append("1.0;");
                        }
                    }
                }

                string s_pattern = o_stringBuilder.ToString();

                if (ForestNET.Lib.Helper.IsStringEmpty(s_pattern))
                {
                    throw new Exception("Could not convert #" + i + " label[" + by_label + "] to training data set");
                }

                string? s_target = by_label switch
                {
                    0 => "1.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0",
                    1 => "0.0;1.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0",
                    2 => "0.0;0.0;1.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0",
                    3 => "0.0;0.0;0.0;1.0;0.0;0.0;0.0;0.0;0.0;0.0",
                    4 => "0.0;0.0;0.0;0.0;1.0;0.0;0.0;0.0;0.0;0.0",
                    5 => "0.0;0.0;0.0;0.0;0.0;1.0;0.0;0.0;0.0;0.0",
                    6 => "0.0;0.0;0.0;0.0;0.0;0.0;1.0;0.0;0.0;0.0",
                    7 => "0.0;0.0;0.0;0.0;0.0;0.0;0.0;1.0;0.0;0.0",
                    8 => "0.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;1.0;0.0",
                    9 => "0.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;1.0",
                    _ => null,
                } ?? throw new Exception("Invalid label[" + by_label + "] found");

                o_fileTrainingSet.AppendLine(s_pattern + s_target);

                i++;
            }

            o_consoleProgressBar.Close();
        }

        private static List<List<double>> GetAILinearTrainingSettings()
        {
            List<List<double>> a_simpleAITrainingSettings = [];

            int i = 0;

            a_simpleAITrainingSettings.Add([]);
            a_simpleAITrainingSettings[i].Add(0.999d); // learning rate
            a_simpleAITrainingSettings[i].Add(0.001d); // momentum
            a_simpleAITrainingSettings[i].Add(25.0d); // desired accuracy
            a_simpleAITrainingSettings[i++].Add(100.0d); // max epochs

            a_simpleAITrainingSettings.Add([]);
            a_simpleAITrainingSettings[i].Add(0.9d); // learning rate
            a_simpleAITrainingSettings[i].Add(0.1d); // momentum
            a_simpleAITrainingSettings[i].Add(50.0d); // desired accuracy
            a_simpleAITrainingSettings[i++].Add(100.0d); // max epochs

            a_simpleAITrainingSettings.Add([]);
            a_simpleAITrainingSettings[i].Add(0.8d); // learning rate
            a_simpleAITrainingSettings[i].Add(0.2d); // momentum
            a_simpleAITrainingSettings[i].Add(55.0d); // desired accuracy
            a_simpleAITrainingSettings[i++].Add(100.0d); // max epochs

            a_simpleAITrainingSettings.Add([]);
            a_simpleAITrainingSettings[i].Add(0.7d); // learning rate
            a_simpleAITrainingSettings[i].Add(0.3d); // momentum
            a_simpleAITrainingSettings[i].Add(60.0d); // desired accuracy
            a_simpleAITrainingSettings[i++].Add(100.0d); // max epochs

            a_simpleAITrainingSettings.Add([]);
            a_simpleAITrainingSettings[i].Add(0.6d); // learning rate
            a_simpleAITrainingSettings[i].Add(0.4d); // momentum
            a_simpleAITrainingSettings[i].Add(65.0d); // desired accuracy
            a_simpleAITrainingSettings[i++].Add(100.0d); // max epochs

            a_simpleAITrainingSettings.Add([]);
            a_simpleAITrainingSettings[i].Add(0.5d); // learning rate
            a_simpleAITrainingSettings[i].Add(0.5d); // momentum
            a_simpleAITrainingSettings[i].Add(70.0d); // desired accuracy
            a_simpleAITrainingSettings[i++].Add(100.0d); // max epochs

            a_simpleAITrainingSettings.Add([]);
            a_simpleAITrainingSettings[i].Add(0.4d); // learning rate
            a_simpleAITrainingSettings[i].Add(0.6d); // momentum
            a_simpleAITrainingSettings[i].Add(75.0d); // desired accuracy
            a_simpleAITrainingSettings[i++].Add(100.0d); // max epochs

            a_simpleAITrainingSettings.Add([]);
            a_simpleAITrainingSettings[i].Add(0.3d); // learning rate
            a_simpleAITrainingSettings[i].Add(0.7d); // momentum
            a_simpleAITrainingSettings[i].Add(80.0d); // desired accuracy
            a_simpleAITrainingSettings[i++].Add(100.0d); // max epochs

            a_simpleAITrainingSettings.Add([]);
            a_simpleAITrainingSettings[i].Add(0.2d); // learning rate
            a_simpleAITrainingSettings[i].Add(0.8d); // momentum
            a_simpleAITrainingSettings[i].Add(85.0d); // desired accuracy
            a_simpleAITrainingSettings[i++].Add(100.0d); // max epochs

            a_simpleAITrainingSettings.Add([]);
            a_simpleAITrainingSettings[i].Add(0.1d); // learning rate
            a_simpleAITrainingSettings[i].Add(0.9d); // momentum
            a_simpleAITrainingSettings[i].Add(90.0d); // desired accuracy
            a_simpleAITrainingSettings[i++].Add(100.0d); // max epochs

            a_simpleAITrainingSettings.Add([]);
            a_simpleAITrainingSettings[i].Add(0.05d); // learning rate
            a_simpleAITrainingSettings[i].Add(0.95d); // momentum
            a_simpleAITrainingSettings[i].Add(95.0d); // desired accuracy
            a_simpleAITrainingSettings[i++].Add(100.0d); // max epochs

            a_simpleAITrainingSettings.Add([]);
            a_simpleAITrainingSettings[i].Add(0.025d); // learning rate
            a_simpleAITrainingSettings[i].Add(0.97d); // momentum
            a_simpleAITrainingSettings[i].Add(97.0d); // desired accuracy
            a_simpleAITrainingSettings[i++].Add(100.0d); // max epochs

            a_simpleAITrainingSettings.Add([]);
            a_simpleAITrainingSettings[i].Add(0.0125d); // learning rate
            a_simpleAITrainingSettings[i].Add(0.98d); // momentum
            a_simpleAITrainingSettings[i].Add(98.0d); // desired accuracy
            a_simpleAITrainingSettings[i++].Add(100.0d); // max epochs

            a_simpleAITrainingSettings.Add([]);
            a_simpleAITrainingSettings[i].Add(0.00625d); // learning rate
            a_simpleAITrainingSettings[i].Add(0.99d); // momentum
            a_simpleAITrainingSettings[i].Add(99.0d); // desired accuracy
            a_simpleAITrainingSettings[i++].Add(100.0d); // max epochs

            return a_simpleAITrainingSettings;
        }

        public static void TrainAI(List<int> p_a_layers, bool p_b_useRectifier, bool p_b_useBatch, string p_s_neuralNetworkFile, string p_s_trainigDataSetFile, double p_d_trainingSetQuota, double p_d_generalizationSetQuota)
        {
            List<List<double>> a_trainingSettings = GetAILinearTrainingSettings();

            NeuralNetwork? o_neuralNetwork = null;
            int i_pointer = 0;
            int i_detectEndlessLoop = 0;
            int i_restarts = 0;
            bool b_once = false;

            ForestNET.Lib.ConsoleProgressBar o_consoleProgressBar = new();

            NeuralNetworkTrainer.PostProgress del_postProgress = (long p_l_epoch, long p_l_maxEpochs, double p_d_desiredAccuracy, double p_d_trainingSetAccuracy, double p_d_generalizationSetAccuracy, double p_d_validationSetAccuracy) =>
            {
                o_consoleProgressBar.Report = (double)p_l_epoch / p_l_maxEpochs;
                o_consoleProgressBar.MarqueeText = "reached tra. " + string.Format("{0:0.00}", p_d_trainingSetAccuracy) + "%|gen. " + string.Format("{0:0.00}", p_d_generalizationSetAccuracy) + "%";
            };

            do
            {
                Console.WriteLine(DateTime.Now);

                if (i_restarts > 5)
                {
                    throw new Exception("AI training failed");
                }

                if (i_detectEndlessLoop > 50)
                {
                    i_detectEndlessLoop = 0;
                    i_pointer = 0;
                    b_once = false;
                    i_restarts++;
                }

                if (!b_once)
                {
                    o_neuralNetwork = new(p_a_layers)
                    {
                        UseRectifierActivationFunction = p_b_useRectifier
                    };
                    o_neuralNetwork.LoadTrainingDataSetFromFile(p_s_trainigDataSetFile, p_d_trainingSetQuota, p_d_generalizationSetQuota);

                    if (ForestNET.Lib.IO.File.Exists(p_s_neuralNetworkFile))
                    {
                        o_neuralNetwork.LoadWeights(p_s_neuralNetworkFile);

                        do
                        {
                            double d_trainingAcc = o_neuralNetwork.GetNetworkAccuracy((o_neuralNetwork.TrainingDataSet ?? throw new NullReferenceException("training data set is null")).TrainingSet, a_trainingSettings[i_pointer][2]);

                            if (d_trainingAcc >= 97.0d)
                            {
                                i_pointer = a_trainingSettings.Count - 1;
                                break;
                            }
                            else if (d_trainingAcc >= a_trainingSettings[i_pointer][2])
                            {
                                Console.WriteLine("neural network with '" + string.Format("{0:0.00}", d_trainingAcc) + "%' accuracy is greater equal des. accuracy '" + string.Format("{0:0.00}", a_trainingSettings[i_pointer][2]) + "%' of training settings");
                                Console.WriteLine("update training settings");
                                Console.WriteLine();
                                i_pointer++;
                            }
                            else
                            {
                                break;
                            }
                        } while (true);
                    }

                    b_once = true;
                }

                NeuralNetworkTrainer o_neuralNetworkTrainer = new(o_neuralNetwork ?? throw new NullReferenceException("neural network is null"))
                {
                    LearningRate = a_trainingSettings[i_pointer][0],
                    Momentum = a_trainingSettings[i_pointer][1],
                    UseBatch = p_b_useBatch,
                    MaxEpochs = Convert.ToInt64(a_trainingSettings[i_pointer][3]),
                    DesiredAccuracy = a_trainingSettings[i_pointer][2],
                    Delegate = del_postProgress
                };

                o_consoleProgressBar.Init(
                    "Train neural network with des. accuracy of '" + string.Format("{0:0.00}", a_trainingSettings[i_pointer][2]) + "%'" + " . . .",
                    "Training finished after max. " + Convert.ToInt64(a_trainingSettings[i_pointer][3]) + " epochs. Target des. accuracy was '" + string.Format("{0:0.00}", a_trainingSettings[i_pointer][2]) + "%'",
                    "reached tra. " + string.Format("{0:0.00}", 0.0d) + "%|gen. " + string.Format("{0:0.00}", 0.0d) + "%"
                );
                o_neuralNetworkTrainer.TrainNetwork();
                o_consoleProgressBar.Close();

                if ((o_neuralNetworkTrainer.TrainingSetAccuracy >= a_trainingSettings[i_pointer][2]) || (o_neuralNetworkTrainer.GeneralizationSetAccuracy >= a_trainingSettings[i_pointer][2]))
                {
                    /* saving neural network to a file */
                    o_neuralNetwork.SaveWeights(p_s_neuralNetworkFile);

                    i_pointer++;
                    Console.WriteLine("update training settings");
                }
                else
                {
                    Console.WriteLine("reuse current training settings");
                }

                Console.WriteLine("");

                i_detectEndlessLoop++;
            } while (i_pointer <= (a_trainingSettings.Count - 1));
        }

        public static void GetAIAccuracy(List<int> p_a_layers, bool p_b_useRectifier, string p_s_neuralNetworkFile, string p_s_trainigDataSetFile, double p_d_trainingSetQuota, double p_d_generalizationSetQuota, double p_d_desiredAccuracy)
        {
            NeuralNetwork o_neuralNetwork = new(p_a_layers)
            {
                UseRectifierActivationFunction = p_b_useRectifier
            };

            o_neuralNetwork.LoadTrainingDataSetFromFile(p_s_trainigDataSetFile, p_d_trainingSetQuota, p_d_generalizationSetQuota);
            o_neuralNetwork.LoadWeights(p_s_neuralNetworkFile);

            ForestNET.Lib.ConsoleProgressBar o_consoleProgressBar = new();

            NeuralNetwork.PostProgress del_postProgress = (long p_l_dataSet, long p_l_amountDataSet, double p_d_incorrectResults) =>
            {
                o_consoleProgressBar.Report = (double)p_l_dataSet / p_l_amountDataSet;
                o_consoleProgressBar.MarqueeText = "calculated accuracy: '" + string.Format("{0:0.00}", 100.0d - (p_d_incorrectResults / Convert.ToDouble(p_l_amountDataSet) * 100.0d)) + "%'";
            };

            o_neuralNetwork.Delegate = del_postProgress;

            Console.WriteLine(DateTime.Now);

            o_consoleProgressBar.Init(
                "Calculate neural network accuracy with des. accuracy of '" + string.Format("{0:0.00}", p_d_desiredAccuracy) + "%'" + " . . .",
                "Neural network accuracy calculated.",
                "calculated accuracy: '100%'"
            );

            double d_trainingAcc = o_neuralNetwork.GetNetworkAccuracy((o_neuralNetwork.TrainingDataSet ?? throw new NullReferenceException("training data set is null")).TrainingSet, p_d_desiredAccuracy);

            o_consoleProgressBar.Close();

            Console.WriteLine("neural network has '" + string.Format("{0:0.00}", d_trainingAcc) + "%' accuracy with des. accuracy of '" + string.Format("{0:0.00}", p_d_desiredAccuracy) + "%'");

            Console.WriteLine(DateTime.Now);
        }

        public static void TestAI(string p_s_filePath, int p_i_height, int p_i_width, int p_i_destinationDimension, List<int> p_a_layers, string p_s_neuralNetworkFile)
        {
            if (!ForestNET.Lib.IO.File.Exists(p_s_filePath))
            {
                throw new Exception("File[" + p_s_filePath + "] does not exists");
            }

            byte[] a_pixels = [];

            /* convert image file to 28 square pixel dimension array as input */
            try
            {
                a_pixels = AITest.ImageToBinaryPixelArray(p_s_filePath, p_i_height, p_i_width, p_i_destinationDimension);
            }
            catch (Exception o_exc)
            {
                Console.WriteLine();
                Console.WriteLine("Could not convert image to destination dimension '" + p_i_destinationDimension + "'; " + o_exc.Message);
                Console.WriteLine("Please check if the picture has 8bit depth.");
                Console.WriteLine();

                return;
            }

            AITest.PrintBinaryPixelArray(a_pixels, p_i_destinationDimension, p_i_destinationDimension);

            List<double> a_input = [];

            int k = 0;

            for (int i = 0; i < p_i_destinationDimension; i++)
            {
                for (int j = 0; j < p_i_destinationDimension; j++)
                {
                    if (a_pixels[k++] == 0)
                    {
                        a_input.Add(0.0d);
                    }
                    else
                    {
                        a_input.Add(1.0d);
                    }
                }
            }

            if (!ForestNET.Lib.IO.File.Exists(p_s_neuralNetworkFile))
            {
                throw new Exception("Neural network file[" + p_s_neuralNetworkFile + "] does not exists");
            }

            NeuralNetwork o_neuralNetwork = new(p_a_layers);
            o_neuralNetwork.LoadWeights(p_s_neuralNetworkFile);

            List<double> a_output = o_neuralNetwork.FeedForwardPattern(a_input);
            List<string> a_keys = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9"];
            Dictionary<string, double> a_result = [];

            /* transfer result */
            for (int i = 0; i < a_output.Count; i++)
            {
                a_result.Add(a_keys[i], a_output[i] * 100.0d);
            }

            /* print  best 5 result */
            int i_result = 0;

            /* sort result */
            foreach (KeyValuePair<string, double> o_result in a_result.OrderByDescending(key => key.Value))
            {
                Console.WriteLine(o_result.Key + ": " + string.Format("{0:0.0000}", o_result.Value) + " %");

                i_result++;

                if (i_result >= 5)
                {
                    break;
                }
            }
        }

        public static void TestAIRandom(string p_s_filePathImage, string p_s_filePathLabel, int p_i_maxAmount, List<int> p_a_layers, string p_s_neuralNetworkFile)
        {
            if (!ForestNET.Lib.IO.File.Exists(p_s_filePathImage))
            {
                throw new Exception("MNIST image file[" + p_s_filePathImage + "] does not exists");
            }

            if (!ForestNET.Lib.IO.File.Exists(p_s_filePathLabel))
            {
                throw new Exception("MNIST label file[" + p_s_filePathLabel + "] does not exists");
            }

            if (p_i_maxAmount < 1)
            {
                throw new Exception("Invalid max. amount parameter '" + p_i_maxAmount + "', must be at least '1'");
            }

            MNISTHandling o_mnistHandling = new(p_s_filePathImage, p_s_filePathLabel);
            int i_random = ForestNET.Lib.Helper.RandomIntegerRange(1, p_i_maxAmount);
            int i_cnt = 0;

            while (o_mnistHandling.HasNext)
            {
                /* read data from MNIST */
                o_mnistHandling.SelectNext();

                if ((i_cnt++ + 1) == i_random)
                {
                    break;
                }
            }

            byte by_label = o_mnistHandling.CurrentLabelValue;
            byte[] a_pixels = o_mnistHandling.GetDataAsBinaryArray(o_mnistHandling.CurrentImageData);


            AITest.PrintBinaryPixelArray(a_pixels, o_mnistHandling.ImageHeight, o_mnistHandling.ImageWidth);

            Console.WriteLine();
            Console.WriteLine("#" + (i_cnt + 1) + " of MNIST database is a '" + by_label + "'");
            Console.WriteLine();

            List<double> a_input = [];

            int k = 0;

            for (int i = 0; i < o_mnistHandling.ImageHeight; i++)
            {
                for (int j = 0; j < o_mnistHandling.ImageWidth; j++)
                {
                    if (a_pixels[k++] == 0)
                    {
                        a_input.Add(0.0d);
                    }
                    else
                    {
                        a_input.Add(1.0d);
                    }
                }
            }

            if (!ForestNET.Lib.IO.File.Exists(p_s_neuralNetworkFile))
            {
                throw new Exception("Neural network file[" + p_s_neuralNetworkFile + "] does not exists");
            }

            NeuralNetwork o_neuralNetwork = new(p_a_layers);
            o_neuralNetwork.LoadWeights(p_s_neuralNetworkFile);

            List<double> a_output = o_neuralNetwork.FeedForwardPattern(a_input);
            List<string> a_keys = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9"];
            Dictionary<string, double> a_result = [];

            /* transfer result */
            for (int i = 0; i < a_output.Count; i++)
            {
                a_result.Add(a_keys[i], a_output[i] * 100.0d);
            }

            /* print best 5 result */
            int i_result = 0;

            /* sort result */
            foreach (KeyValuePair<string, double> o_result in a_result.OrderByDescending(key => key.Value))
            {
                Console.WriteLine(o_result.Key + ": " + string.Format("{0:0.0000}", o_result.Value) + " %");

                i_result++;

                if (i_result >= 5)
                {
                    break;
                }
            }
        }
    }
}
