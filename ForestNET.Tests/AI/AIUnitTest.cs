using ForestNET.Lib.AI;

namespace ForestNET.Tests.AI
{
    public class AIUnitTest
    {
        [Test]
        public void TestAI()
        {
            try
            {
                string s_currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'");
                string s_resourcesAIDirectory = Environment.CurrentDirectory + ForestNET.Lib.IO.File.DIR + "Resources" + ForestNET.Lib.IO.File.DIR + "ai" + ForestNET.Lib.IO.File.DIR;

                TrainingDataSet o_trainingDataset;
                double d_trainingSetQuota = 1.0; /* always 100% because with these simple examples we have very few training data */
                double d_generalizationSetQuota = 0.0; /* 0% because training quota is 100% */
                bool b_useRectifier = false;
                bool b_useBatch = false;

                /* AND with 3 inputs and 1 output, 4 hidden */
                o_trainingDataset = new();
                o_trainingDataset.TrainingSet.Add(new DataEntry([0.0d, 0.0d, 0.0d], [0.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([0.0d, 0.0d, 1.0d], [0.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([0.0d, 1.0d, 0.0d], [0.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([0.0d, 1.0d, 1.0d], [0.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([1.0d, 0.0d, 0.0d], [0.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([1.0d, 0.0d, 1.0d], [0.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([1.0d, 1.0d, 0.0d], [0.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([1.0d, 1.0d, 1.0d], [1.000d]));

                TestAILogicGate([3, 4, 1], b_useRectifier, b_useBatch, o_trainingDataset, d_trainingSetQuota, d_generalizationSetQuota, 2);

                /* NAND with 3 inputs and 1 output, 4 hidden */
                o_trainingDataset = new();
                o_trainingDataset.TrainingSet.Add(new DataEntry([0.0d, 0.0d, 0.0d], [1.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([0.0d, 0.0d, 1.0d], [1.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([0.0d, 1.0d, 0.0d], [1.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([0.0d, 1.0d, 1.0d], [1.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([1.0d, 0.0d, 0.0d], [1.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([1.0d, 0.0d, 1.0d], [1.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([1.0d, 1.0d, 0.0d], [1.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([1.0d, 1.0d, 1.0d], [0.000d]));

                TestAILogicGate([3, 4, 1], b_useRectifier, b_useBatch, o_trainingDataset, d_trainingSetQuota, d_generalizationSetQuota, 2);

                /* OR with 3 inputs and 1 output, 4 hidden */
                o_trainingDataset = new();
                o_trainingDataset.TrainingSet.Add(new DataEntry([0.0d, 0.0d, 0.0d], [0.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([0.0d, 0.0d, 1.0d], [1.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([0.0d, 1.0d, 0.0d], [1.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([0.0d, 1.0d, 1.0d], [1.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([1.0d, 0.0d, 0.0d], [1.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([1.0d, 0.0d, 1.0d], [1.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([1.0d, 1.0d, 0.0d], [1.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([1.0d, 1.0d, 1.0d], [1.000d]));

                TestAILogicGate([3, 4, 1], b_useRectifier, b_useBatch, o_trainingDataset, d_trainingSetQuota, d_generalizationSetQuota, 2);

                /* NOR with 3 inputs and 1 output, 4 hidden */
                o_trainingDataset = new();
                o_trainingDataset.TrainingSet.Add(new DataEntry([0.0d, 0.0d, 0.0d], [1.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([0.0d, 0.0d, 1.0d], [0.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([0.0d, 1.0d, 0.0d], [0.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([0.0d, 1.0d, 1.0d], [0.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([1.0d, 0.0d, 0.0d], [0.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([1.0d, 0.0d, 1.0d], [0.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([1.0d, 1.0d, 0.0d], [0.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([1.0d, 1.0d, 1.0d], [0.000d]));

                TestAILogicGate([3, 4, 1], b_useRectifier, b_useBatch, o_trainingDataset, d_trainingSetQuota, d_generalizationSetQuota, 2);

                /* XOR with 3 inputs and 1 output, 4 hidden */
                o_trainingDataset = new();
                o_trainingDataset.TrainingSet.Add(new DataEntry([0.0d, 0.0d, 0.0d], [0.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([0.0d, 0.0d, 1.0d], [1.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([0.0d, 1.0d, 0.0d], [1.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([0.0d, 1.0d, 1.0d], [0.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([1.0d, 0.0d, 0.0d], [1.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([1.0d, 0.0d, 1.0d], [0.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([1.0d, 1.0d, 0.0d], [0.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([1.0d, 1.0d, 1.0d], [1.000d]));

                TestAILogicGate([3, 4, 1], b_useRectifier, b_useBatch, o_trainingDataset, d_trainingSetQuota, d_generalizationSetQuota, 2);

                /* XNOR with 3 inputs and 1 output, 4 hidden */
                o_trainingDataset = new();
                o_trainingDataset.TrainingSet.Add(new DataEntry([0.0d, 0.0d, 0.0d], [1.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([0.0d, 0.0d, 1.0d], [0.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([0.0d, 1.0d, 0.0d], [0.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([0.0d, 1.0d, 1.0d], [0.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([1.0d, 0.0d, 0.0d], [0.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([1.0d, 0.0d, 1.0d], [0.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([1.0d, 1.0d, 0.0d], [0.000d]));
                o_trainingDataset.TrainingSet.Add(new DataEntry([1.0d, 1.0d, 1.0d], [1.000d]));

                TestAILogicGate([3, 4, 1], b_useRectifier, b_useBatch, o_trainingDataset, d_trainingSetQuota, d_generalizationSetQuota, 2);

                /* XOR with training data set from file, 3 inputs and 1 output, 4 hidden */
                string s_trainingSetFile = s_resourcesAIDirectory + "xor.txt";

                TestAIWithTrainingDataInFile([3, 4, 1], b_useRectifier, b_useBatch, s_trainingSetFile, d_trainingSetQuota, d_generalizationSetQuota, 2);

                /* test ridiculous ANN with training data set from file */
                string s_neuralNetworkFile = s_resourcesAIDirectory + "RidiculousAI.txt";
                s_trainingSetFile = s_resourcesAIDirectory + "RidiculousTrainingSet.txt";

                TestAIWithTrainingDataInFile([3, 32, 16, 8, 3], b_useRectifier, b_useBatch, s_trainingSetFile, d_trainingSetQuota, d_generalizationSetQuota, 3);

                /* test ridiculous ANN without training, loading directly from file */
                TestAIFromFile([3, 32, 16, 8, 3], b_useRectifier, b_useBatch, s_neuralNetworkFile, s_trainingSetFile, 3);
            }
            catch (Exception o_exc)
            {
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
        }

        private static List<List<double>> GetAILinearTrainingSettings()
        {
            List<List<double>> a_simpleAITrainingSettings = [];

            int i = 0;

            a_simpleAITrainingSettings.Add([]);
            a_simpleAITrainingSettings[i].Add(0.999d); // learning rate
            a_simpleAITrainingSettings[i].Add(0.001d); // momentum
            a_simpleAITrainingSettings[i].Add(25.0d); // desired accuracy
            a_simpleAITrainingSettings[i++].Add(40000.0d); // max epochs

            a_simpleAITrainingSettings.Add([]);
            a_simpleAITrainingSettings[i].Add(0.9d); // learning rate
            a_simpleAITrainingSettings[i].Add(0.1d); // momentum
            a_simpleAITrainingSettings[i].Add(50.0d); // desired accuracy
            a_simpleAITrainingSettings[i++].Add(40000.0d); // max epochs

            a_simpleAITrainingSettings.Add([]);
            a_simpleAITrainingSettings[i].Add(0.8d); // learning rate
            a_simpleAITrainingSettings[i].Add(0.2d); // momentum
            a_simpleAITrainingSettings[i].Add(55.0d); // desired accuracy
            a_simpleAITrainingSettings[i++].Add(40000.0d); // max epochs

            a_simpleAITrainingSettings.Add([]);
            a_simpleAITrainingSettings[i].Add(0.7d); // learning rate
            a_simpleAITrainingSettings[i].Add(0.3d); // momentum
            a_simpleAITrainingSettings[i].Add(60.0d); // desired accuracy
            a_simpleAITrainingSettings[i++].Add(40000.0d); // max epochs

            a_simpleAITrainingSettings.Add([]);
            a_simpleAITrainingSettings[i].Add(0.6d); // learning rate
            a_simpleAITrainingSettings[i].Add(0.4d); // momentum
            a_simpleAITrainingSettings[i].Add(65.0d); // desired accuracy
            a_simpleAITrainingSettings[i++].Add(40000.0d); // max epochs

            a_simpleAITrainingSettings.Add([]);
            a_simpleAITrainingSettings[i].Add(0.5d); // learning rate
            a_simpleAITrainingSettings[i].Add(0.5d); // momentum
            a_simpleAITrainingSettings[i].Add(70.0d); // desired accuracy
            a_simpleAITrainingSettings[i++].Add(40000.0d); // max epochs

            a_simpleAITrainingSettings.Add([]);
            a_simpleAITrainingSettings[i].Add(0.4d); // learning rate
            a_simpleAITrainingSettings[i].Add(0.6d); // momentum
            a_simpleAITrainingSettings[i].Add(75.0d); // desired accuracy
            a_simpleAITrainingSettings[i++].Add(40000.0d); // max epochs

            a_simpleAITrainingSettings.Add([]);
            a_simpleAITrainingSettings[i].Add(0.3d); // learning rate
            a_simpleAITrainingSettings[i].Add(0.7d); // momentum
            a_simpleAITrainingSettings[i].Add(80.0d); // desired accuracy
            a_simpleAITrainingSettings[i++].Add(40000.0d); // max epochs

            a_simpleAITrainingSettings.Add([]);
            a_simpleAITrainingSettings[i].Add(0.2d); // learning rate
            a_simpleAITrainingSettings[i].Add(0.8d); // momentum
            a_simpleAITrainingSettings[i].Add(85.0d); // desired accuracy
            a_simpleAITrainingSettings[i++].Add(40000.0d); // max epochs

            a_simpleAITrainingSettings.Add([]);
            a_simpleAITrainingSettings[i].Add(0.1d); // learning rate
            a_simpleAITrainingSettings[i].Add(0.9d); // momentum
            a_simpleAITrainingSettings[i].Add(90.0d); // desired accuracy
            a_simpleAITrainingSettings[i++].Add(40000.0d); // max epochs

            a_simpleAITrainingSettings.Add([]);
            a_simpleAITrainingSettings[i].Add(0.05d); // learning rate
            a_simpleAITrainingSettings[i].Add(0.95d); // momentum
            a_simpleAITrainingSettings[i].Add(95.0d); // desired accuracy
            a_simpleAITrainingSettings[i++].Add(40000.0d); // max epochs

            a_simpleAITrainingSettings.Add([]);
            a_simpleAITrainingSettings[i].Add(0.025d); // learning rate
            a_simpleAITrainingSettings[i].Add(0.97d); // momentum
            a_simpleAITrainingSettings[i].Add(97.0d); // desired accuracy
            a_simpleAITrainingSettings[i++].Add(40000.0d); // max epochs

            a_simpleAITrainingSettings.Add([]);
            a_simpleAITrainingSettings[i].Add(0.0125d); // learning rate
            a_simpleAITrainingSettings[i].Add(0.98d); // momentum
            a_simpleAITrainingSettings[i].Add(99.0d); // desired accuracy
            a_simpleAITrainingSettings[i++].Add(40000.0d); // max epochs

            return a_simpleAITrainingSettings;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private static void TestAILogicGate(List<int> p_a_layers, bool p_b_useRectifier, bool p_b_useBatch, TrainingDataSet p_o_trainigDataSet, double p_d_trainingSetQuota, double p_d_generalizationSetQuota, int p_i_resultPrecision)
        {
            List<List<double>> a_trainingSettings = GetAILinearTrainingSettings();

            NeuralNetwork? o_neuralNetwork = null;
            int i_pointer = 0;
            int i_detectEndlessLoop = 0;
            int i_restarts = 0;
            bool b_once = false;

            do
            {
                //Console.WriteLine(i_pointer);

                if (i_restarts > 5)
                {
                    Assert.Fail("AI training failed");
                    break;
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
                        TrainingDataSet = p_o_trainigDataSet,
                        UseRectifierActivationFunction = p_b_useRectifier
                    };

                    o_neuralNetwork.SetTrainingQuotas(p_d_trainingSetQuota, p_d_generalizationSetQuota);

                    b_once = true;
                }

                NeuralNetworkTrainer o_neuralNetworkTrainer = new(o_neuralNetwork ?? throw new NullReferenceException("neural network is null"))
                {
                    LearningRate = a_trainingSettings[i_pointer][0],
                    Momentum = a_trainingSettings[i_pointer][1],
                    UseBatch = p_b_useBatch,
                    MaxEpochs = Convert.ToInt64(a_trainingSettings[i_pointer][3]),
                    DesiredAccuracy = a_trainingSettings[i_pointer][2]
                };

                o_neuralNetworkTrainer.TrainNetwork();

                if ((o_neuralNetworkTrainer.TrainingSetAccuracy >= a_trainingSettings[i_pointer][2]) || (o_neuralNetworkTrainer.GeneralizationSetAccuracy >= a_trainingSettings[i_pointer][2]))
                {
                    i_pointer++;
                }

                i_detectEndlessLoop++;
            } while (i_pointer <= (a_trainingSettings.Count - 1));

            foreach (DataEntry o_dataEntry in p_o_trainigDataSet.TrainingSet)
            {
                List<double> a_output = (o_neuralNetwork ?? throw new NullReferenceException("neural network is null")).FeedForwardPattern(o_dataEntry.Pattern);

                for (int i = 0; i < a_output.Count; i++)
                {
                    //Console.WriteLine( i + ": " + a_output[i] + " - " + o_dataEntry.Target[i] + " | " + Math.Round(a_output[i], p_i_resultPrecision, MidpointRounding.AwayFromZero) + " - " + Math.Round(o_dataEntry.Target[i], p_i_resultPrecision, MidpointRounding.AwayFromZero) );
                    Assert.That(NeuralNetwork.ClampOutput(a_output[i], o_dataEntry.Target[i], 99.0d), Is.True, "output[" + a_output[i] + "] does not match expected output[" + o_dataEntry.Target[i] + "]");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private static void TestAIWithTrainingDataInFile(List<int> p_a_layers, bool p_b_useRectifier, bool p_b_useBatch, string p_s_trainigDataSetFile, double p_d_trainingSetQuota, double p_d_generalizationSetQuota, int p_i_resultPrecision)
        {
            List<List<double>> a_trainingSettings = GetAILinearTrainingSettings();

            NeuralNetwork? o_neuralNetwork = null;
            int i_pointer = 0;
            int i_detectEndlessLoop = 0;
            int i_restarts = 0;
            bool b_once = false;

            do
            {
                if (i_restarts > 0)
                {
                    Assert.Fail("AI training failed");
                    break;
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

                    b_once = true;
                }

                NeuralNetworkTrainer o_neuralNetworkTrainer = new(o_neuralNetwork ?? throw new NullReferenceException("neural network is null"))
                {
                    LearningRate = a_trainingSettings[i_pointer][0],
                    Momentum = a_trainingSettings[i_pointer][1],
                    UseBatch = p_b_useBatch,
                    MaxEpochs = Convert.ToInt64(a_trainingSettings[i_pointer][3]),
                    DesiredAccuracy = a_trainingSettings[i_pointer][2]
                };

                o_neuralNetworkTrainer.TrainNetwork();

                if ((o_neuralNetworkTrainer.TrainingSetAccuracy >= a_trainingSettings[i_pointer][2]) || (o_neuralNetworkTrainer.GeneralizationSetAccuracy >= a_trainingSettings[i_pointer][2]))
                {
                    i_pointer++;
                }

                i_detectEndlessLoop++;
            } while (i_pointer <= (a_trainingSettings.Count - 1));

            /* comment in for saving neural network to a file */
            //o_neuralNetwork.SaveWeights(
            //	(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'")) +
            //	ForestNET.Lib.IO.File.DIR +
            //	"ai.txt"
            //);

            foreach (DataEntry o_dataEntry in ((o_neuralNetwork ?? throw new NullReferenceException("neural network is null")).TrainingDataSet ?? throw new NullReferenceException("Training data set of neural network is null")).TrainingSet)
            {
                List<double> a_output = o_neuralNetwork.FeedForwardPattern(o_dataEntry.Pattern);

                for (int i = 0; i < a_output.Count; i++)
                {
                    //Console.WriteLine(i + ": " + a_output[i] + " - " + o_dataEntry.Target[i] + " | " + Math.Round(a_output[i], p_i_resultPrecision, MidpointRounding.AwayFromZero) + " - " + Math.Round(o_dataEntry.Target[i], p_i_resultPrecision, MidpointRounding.AwayFromZero));
                    Assert.That(NeuralNetwork.ClampOutput(a_output[i], o_dataEntry.Target[i], 99.0d), Is.True, "output[" + a_output[i] + "] does not match expected output[" + o_dataEntry.Target[i] + "]");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private static void TestAIFromFile(List<int> p_a_layers, bool p_b_useRectifier, bool p_b_useBatch, string p_s_neuralNetworkFile, string p_s_trainigDataSetFile, int p_i_resultPrecision)
        {
            NeuralNetwork o_neuralNetwork = new(p_a_layers);
            o_neuralNetwork.LoadWeights(p_s_neuralNetworkFile);
            o_neuralNetwork.LoadTrainingDataSetFromFile(p_s_trainigDataSetFile, 1.0, 0.0);
            o_neuralNetwork.UseRectifierActivationFunction = p_b_useRectifier;

            foreach (DataEntry o_dataEntry in (o_neuralNetwork.TrainingDataSet ?? throw new NullReferenceException("Training data set of neural network is null")).TrainingSet)
            {
                List<double> a_output = o_neuralNetwork.FeedForwardPattern(o_dataEntry.Pattern);

                for (int i = 0; i < a_output.Count; i++)
                {
                    //Console.WriteLine(i + ": " + a_output[i] + " - " + o_dataEntry.Target[i] + " | " + Math.Round(a_output[i], p_i_resultPrecision, MidpointRounding.AwayFromZero) + " - " + Math.Round(o_dataEntry.Target[i], p_i_resultPrecision, MidpointRounding.AwayFromZero));
                    Assert.That(NeuralNetwork.ClampOutput(a_output[i], o_dataEntry.Target[i], 99.0d), Is.True, "output[" + a_output[i] + "] does not match expected output[" + o_dataEntry.Target[i] + "]");
                }
            }
        }
    }
}