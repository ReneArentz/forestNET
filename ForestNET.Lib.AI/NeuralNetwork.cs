namespace ForestNET.Lib.AI
{
    /// <summary>
    /// Class which represents a neural network with input, output and hidden neurons.
    /// structure should be seen as layers for a feed forward ann.
    /// </summary>
    public class NeuralNetwork
    {

        /* Delegates */

        /// <summary>
        /// delegate definition which can be instanced outside of NeuralNetwork class to post progress anywhere when neural network accuracy will be calculated
        /// </summary>
        public delegate void PostProgress(long p_l_dataSet, long p_l_amountDataSet, double p_d_incorrectResults);

        /* Fields */

        private readonly List<int> a_layers;
        private readonly List<List<double>> a_neurons;
        private readonly List<List<List<double>>> a_weights;

        private string s_delimiter = string.Empty;

        private double d_trainingSetQuota;
        private double d_generalizationSetQuota;
        private TrainingDataSet? o_trainingDataSet;

        private PostProgress? del_postProgress;

        /* Properties */

        public List<int> Layers
        {
            get
            {
                return this.a_layers;
            }
        }

        public List<List<double>> Neurons
        {
            get
            {
                return this.a_neurons;
            }
        }

        public List<List<List<double>>> Weights
        {
            get
            {
                return this.a_weights;
            }
        }

        public string Delimiter
        {
            get
            {
                return this.s_delimiter;
            }
            set
            {
                if (value.Length != 1)
                {
                    throw new ArgumentException("Delimiter must have length(1) - ['" + value + "' -> length(" + value.Length + ")]");
                }

                this.s_delimiter = value;
            }
        }

        public bool UseRectifierActivationFunction
        {
            get; set;
        } = false; /* default: using sigmoid function */

        public TrainingDataSet? TrainingDataSet
        {
            get
            {
                return this.o_trainingDataSet;
            }
            set
            {
                this.o_trainingDataSet = value;
            }
        }

        public PostProgress? Delegate
        {
            set
            {
                this.del_postProgress = value;
            }
        }

        /* Methods */

        /// <summary>
        /// Constructor of neural network class, creating an ann with amount of input, hidden and output neurons within several layers.
        /// linebreak will be read from system, delimiter will be ';'.
        /// </summary>
        /// <param name="p_a_layers">list of integers, each value tells us the amount of neurons in a layer - first layer is the input layer and last layer is the output layer</param>
        /// <exception cref="ArgumentException">parameter of constructor have an illegal argument</exception>
        public NeuralNetwork(List<int> p_a_layers) :
            this(p_a_layers, ";")
        {

        }

        /// <summary>
        /// Constructor of neural network class, creating an ann with amount of input, hidden and output neurons within several layers.
        /// linebreak will be read from system, delimiter will be ';'.
        /// 											should not be [0-9] 'e' ',' '.' '+' or '-'
        /// </summary>
        /// <param name="p_a_layers">list of integers, each value tells us the amount of neurons in a layer - first layer is the input layer and last layer is the output layer</param>
        /// <param name="p_s_delimiter">delimiter which will be used to separate neurons value from each other if we save to a file</param>
        /// <exception cref="ArgumentException">parameter of constructor have an illegal argument, or value in hidden layer list is not greater equal 1</exception>
        public NeuralNetwork(List<int> p_a_layers, string p_s_delimiter)
        {
            /* check for minimum value for amount of hidden neurons */
            if (p_a_layers.Count < 3)
            {
                throw new ArgumentException("Parameter for 'layers' must have at least '3' as size, but size is '" + p_a_layers.Count + "'");
            }

            /* check delimiter parameter */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_delimiter))
            {
                throw new ArgumentException("Parameter for 'delimiter' is null or empty");
            }

            this.a_layers = p_a_layers;

            /* set delimiter */
            this.Delimiter = p_s_delimiter;

            /* default to training set properties, must be set later */
            this.d_trainingSetQuota = 0.0d;
            this.d_generalizationSetQuota = 0.0d;
            this.o_trainingDataSet = null;

            /* initialize neuron array */
            this.a_neurons = new(this.a_layers.Count);

            /* initialize neuron arrays for hidden layers */
            foreach (int i_foo in this.a_layers)
            {
                if (i_foo <= 0)
                {
                    throw new ArgumentException("Value of a 'layer' must be at least greater equal '1', but is '" + i_foo + "'");
                }

                this.a_neurons.Add(new(i_foo + 1));
            }

            for (int i = 0; i < this.a_layers.Count; i++)
            {
                /* set layer neuron array to zero values */
                for (int j = 0; j < this.a_layers[i]; j++)
                {
                    this.a_neurons[i].Add(0.0);
                }

                /* add bias neuron in layer array */
                this.a_neurons[i].Add(-1.0);
            }

            /* initialize weight array 1st dimension */
            this.a_weights = new(this.a_layers.Count - 1);

            /* we iterate each layer to get to know how many weights we must store and how many weights explicitly are between them */
            for (int i = 0; i < this.a_layers.Count - 1; i++)
            { /* size - 1, cause there are just size - 1 weight layers between all layers */
                /* initialize weight array 2nd dimension */
                this.a_weights.Add(new(this.a_layers[i] + 1));

                /* set neuron(i) between neuron(i+1) weights array to zero values */
                for (int j = 0; j <= this.a_layers[i]; j++)
                {
                    /* initialize weight array 3rd dimension */
                    this.a_weights[i].Add(new(this.a_layers[i + 1]));

                    /* add 3rd dimension weights as zero values */
                    for (int k = 0; k < this.a_layers[i + 1]; k++)
                    {
                        this.a_weights[i][j].Add(0.0d);
                    }
                }
            }

            /* initialize weights with random values */
            this.InitializeWeights();
        }

        /// <summary>
        /// This method will initialize all weights with random values
        /// </summary>
        public void InitializeWeights()
        {
            /* initialize layer weights array to random values - iterate each hidden layer, but not the last */
            for (int i = 0; i < this.a_layers.Count - 1; i++)
            { /* size - 1, cause there are just size - 1 weight layers between all layers */
                /* set range for generating random values with 1/sqrt(x), high value x means small range cause we have enough to randomize */
                double d_range = 1 / Math.Sqrt(Convert.ToDouble(this.a_layers[i]));

                /* set neuron(i) between neuron(i+1) weights array to random values */
                for (int j = 0; j <= this.a_layers[i]; j++)
                {
                    /* set weights with random values - ( random(0..99) + 1 / 100 * 2 * range ) - range */
                    for (int k = 0; k < this.a_layers[i + 1]; k++)
                    {
                        this.a_weights[i][j][k] = ((ForestNET.Lib.Helper.RandomDoubleRange(0.0d, 99.0d) + 1.0d) / 100.0d * 2.0d * d_range) - d_range;
                    }
                }
            }
        }

        /// <summary>
        /// Method to set training set and generalization set quotas, both together must no exceed 1.0
        /// </summary>
        /// <param name="p_d_trainingSetQuota">how much percent of training data will be used for training</param>
        /// <param name="p_d_generalizationSetQuota">how much percent of training data will be used for generalization</param>
        /// <exception cref="ArgumentException">sum of training set quota and generalization set quota is lower equal 0.0 or greater than 1.0</exception>
        public void SetTrainingQuotas(double p_d_trainingSetQuota, double p_d_generalizationSetQuota)
        {
            double d_sum = p_d_trainingSetQuota + p_d_generalizationSetQuota;

            if (d_sum <= 0.0d)
            {
                throw new ArgumentException("Sum of training set quota and generalization set quota must be greater than '0.0', but it is '" + d_sum + "'");
            }

            if (d_sum > 1.0d)
            {
                throw new ArgumentException("Sum of training set quota and generalization set quota must be lower equal '1.0', but it is '" + d_sum + "'");
            }

            this.d_trainingSetQuota = p_d_trainingSetQuota;
            this.d_generalizationSetQuota = p_d_generalizationSetQuota;
        }

        /// <summary>
        /// Activation function for neurons of neural network.
        /// sigmoid function as default, or rectifier function via class property.
        /// </summary>
        /// <param name="p_d_value">value which will be checked if neuron will fire or not</param>
        /// <returns>activation value for neuron</returns>
        private double ActivationFunction(double p_d_value)
        {
            if (this.UseRectifierActivationFunction)
            {
                /* rectifier function */
                return Math.Max(0.0d, p_d_value);
            }
            else
            {
                /* sigmoid function */
                return (1.0d / (1.0d + Math.Exp(-1.0d * p_d_value)));
            }
        }

        /// <summary>
        /// This method tells us if a output value of our neural network has reached it's desired value with the correct accuracy
        /// </summary>
        /// <param name="p_d_value">output value we want to check</param>
        /// <param name="p_d_desiredValue">our ideal desired value</param>
        /// <param name="p_d_desiredAccuracy">our desired accuracy</param>
        /// <returns>bool					true - output value has reached desired value, false - output value not near desired value</returns>
        public static bool ClampOutput(double p_d_value, double p_d_desiredValue, double p_d_desiredAccuracy)
        {
            /* create difference between output value and desired value */
            double d_diff = p_d_desiredValue - p_d_value;

            /* correct negative values */
            if (d_diff < 0.0d)
            {
                d_diff *= -1.0d;
            }

            /* calculate desired deviation */
            double d_desiredDeviation = p_d_desiredValue - (p_d_desiredValue * (p_d_desiredAccuracy / 100.0d));

            if (p_d_desiredValue == 0.0d)
            {
                /* calculate desired deviation towards zero */
                d_desiredDeviation = (1.0d - (p_d_desiredAccuracy / 100.0d));
            }

            /* if difference is lower equal desired deviation */
            if (d_diff <= d_desiredDeviation)
            {
                return true; /* matches expectation */
            }
            else
            {
                return false; /* not matches expectation */
            }
        }

        /// <summary>
        /// send a pattern of input neuron values through the neural network
        /// </summary>
        /// <param name="p_a_pattern">input pattern values</param>
        private void FeedForward(List<double> p_a_pattern)
        {
            /* check input amount with amount of first layer neurons, -1 because of bias neuron */
            if (p_a_pattern.Count != this.a_neurons[0].Count - 1)
            {
                throw new ArgumentException("Illegal amount of input neurons '" + p_a_pattern.Count + "'. Neural network's first layer has '" + (this.a_neurons[0].Count - 1) + "' neurons.");
            }

            /* insert input pattern to the network */
            for (int i = 0; i < this.a_layers[0]; i++)
            {
                this.a_neurons[0][i] = p_a_pattern[i];
            }

            /* calculating the values between layers, including bias neuron - iterate each hidden layer */
            for (int i = 1; i < this.a_layers.Count; i++)
            {
                /* get layer(i + 1) between layer(i) neurons */
                for (int j = 0; j < this.a_layers[i]; j++)
                {
                    /* delete old value, by setting to zero */
                    this.a_neurons[i][j] = 0.0d;

                    /* calculate weights sum */
                    for (int k = 0; k <= this.a_layers[i - 1]; k++)
                    {
                        /* neuron = neuron + ( previous_neruon * weight_between_both ) */
                        this.a_neurons[i][j] = this.a_neurons[i][j] + (this.a_neurons[i - 1][k] * this.a_weights[i - 1][k][j]);
                    }

                    /* determine results by the activation function */
                    this.a_neurons[i][j] = this.ActivationFunction(this.a_neurons[i][j]);
                }
            }
        }

        /// <summary>
        /// Method to determine the current neural network accuracy with an amount of input data and expected result data sets
        /// </summary>
        /// <param name="p_a_dataSet">amount of data entries with input patterns and expected output patterns</param>
        /// <param name="p_d_desiredAccuracy">desired accuracy we want to claim with our data entries parameter</param>
        /// <returns>return the network accuracy in percentage</returns>
        public double GetNetworkAccuracy(List<DataEntry> p_a_dataSet, double p_d_desiredAccuracy)
        {
            double d_incorrectResults = 0;

            /* iterate each data entry */
            for (int i = 0; i < p_a_dataSet.Count; i++)
            {
                /* send data entry pattern through the neural network */
                this.FeedForward(p_a_dataSet[i].Pattern);

                /* compare neural network output with expected output from data entry */
                for (int j = 0; j < this.a_layers[this.a_layers.Count - 1]; j++)
                {
                    /* pattern is wrong if calculated output differs from known output */
                    if (!NeuralNetwork.ClampOutput(this.a_neurons[this.a_layers.Count - 1][j], p_a_dataSet[i].Target[j], p_d_desiredAccuracy))
                    {
                        /* comparison failed, so we increase incorrect result counter and break the for loop */
                        d_incorrectResults++;
                        break;
                    }
                }

                /* post progress of calculating neural network accuracy */
                this.del_postProgress?.Invoke(i + 1, p_a_dataSet.Count, d_incorrectResults);
            }

            /* return the network accuracy in percentage with our incorrect result counter and amount of our data entries */
            return 100.0d - (d_incorrectResults / Convert.ToDouble(p_a_dataSet.Count) * 100.0d);
        }

        /// <summary>
        /// send a pattern of input neuron values through the neural network
        /// </summary>
        /// <param name="p_a_pattern">input pattern values</param>
        /// <returns>all output neuron values of this neural network</returns>
        public List<double> FeedForwardPattern(List<double> p_a_pattern)
        {
            /* send pattern through neural network */
            this.FeedForward(p_a_pattern);

            /* create list of output neurons as our result value */
            List<double> a_results = new(this.a_layers[this.a_layers.Count - 1]);

            /* create a copy of neural network output neurons after the network processed the input pattern */
            for (int i = 0; i < this.a_layers[this.a_layers.Count - 1]; i++)
            {
                a_results.Add(this.a_neurons[this.a_layers.Count - 1][i]);
            }

            /* return list of output neurons */
            return a_results;
        }

        /// <summary>
        /// Save all weights values of neural network to a file, separated by delimiter property
        /// </summary>
        /// <param name="p_s_filename">filePath + file which will contain all weight values</param>
        /// <exception cref="System.IO.IOException">could not save weight values to file</exception>
        /// <exception cref="System.IO..FileNotFoundException">invalid filePath or fileName</exception>
        public void SaveWeights(string p_s_filename)
        {
            /* open (new) file */
            ForestNET.Lib.IO.File o_file = new(p_s_filename, !ForestNET.Lib.IO.File.Exists(p_s_filename));

            /* temp variable for list of weights */
            List<string> a_weights = [];

            /* get all weights between layers, iterate each layer */
            for (int i = 0; i < this.a_layers.Count - 1; i++)
            { /* size - 1, cause there are just size - 1 weight layers between all layers */
                for (int j = 0; j <= this.a_layers[i]; j++)
                {
                    for (int k = 0; k < this.a_layers[i + 1]; k++)
                    {
                        a_weights.Add(this.a_weights[i][j][k].ToString(System.Globalization.CultureInfo.InvariantCulture));
                    }
                }
            }

            /* save weights data into file, separated by delimiter property */
            o_file.ReplaceContent(ForestNET.Lib.Helper.JoinList(a_weights, this.s_delimiter[0]));
        }

        /// <summary>
        /// Load weights for neural network from a file
        /// </summary>
        /// <param name="p_s_filename">filePath + file which contains all weight values</param>
        /// <exception cref="System.IO.IOException">could not load weight values from file</exception>
        /// <exception cref="InvalidOperationException">amount of stored weights is not correct</exception>
        public void LoadWeights(string p_s_filename)
        {
            /* check if file exists, if not just create a new one with current initialized random weights */
            if (!ForestNET.Lib.IO.File.Exists(p_s_filename))
            {
                this.SaveWeights(p_s_filename);
                ForestNET.Lib.Global.LogWarning("File[" + p_s_filename + "] does not exist; created file with initialized random weights");
            }

            /* open file */
            ForestNET.Lib.IO.File o_file = new(p_s_filename, false);

            /* temp list of weights */
            List<double> a_weights = [];

            /* iterate each file line */
            for (int i_line = 1; i_line <= o_file.FileLines; i_line++)
            {
                /* separate stored weight values with delimiter property */
                string[] a_lineValues = o_file.ReadLine(i_line).Split(this.s_delimiter);

                /* store each weight value in temp list */
                for (int i = 0; i < a_lineValues.Length; i++)
                {
                    a_weights.Add(Convert.ToDouble(a_lineValues[i], System.Globalization.CultureInfo.InvariantCulture));
                }
            }

            /* counting all weights */
            int i_expectedAmount = 0;

            /* iterate each layer for counting all expected weights */
            for (int i = 0; i < this.a_layers.Count - 1; i++)
            {
                i_expectedAmount += (this.a_layers[i] + 1) * this.a_layers[i + 1];
            }

            /* check amount of read weights from file with settings from current neural network */
            if (a_weights.Count != i_expectedAmount)
            {
                throw new InvalidOperationException("Invalid amount of weights[" + a_weights.Count + "]. Expected amount of weights = [" + i_expectedAmount + "]");
            }

            int l = 0;

            /* load weights from file to layers, iterate each layer */
            for (int i = 0; i < this.a_layers.Count - 1; i++)
            { /* size - 1, cause there are just size - 1 weight layers between all layers */
                for (int j = 0; j <= this.a_layers[i]; j++)
                {
                    for (int k = 0; k < this.a_layers[i + 1]; k++)
                    {
                        this.a_weights[i][j][k] = a_weights[l++];
                    }
                }
            }
        }

        /// <summary>
        /// Loading training data entries from a file, using quotas to split entries as training, generalization and validation entries.
        /// sum of both quotas must be lower equal 1.0.
        /// if the sum of both is lower than 1.0, rest will be used for validation.
        /// </summary>
        /// <param name="p_s_filename">filePath + file which contains all training data set entries</param>
        /// <exception cref="ArgumentException">wrong quota values</exception>
        /// <exception cref="System.IO..FileNotFoundException">invalid filePath or fileName</exception>
        /// <exception cref="System.IO.IOException">file could not be read</exception>
        public void LoadTrainingDataSetFromFile(string p_s_filename)
        {
            this.LoadTrainingDataSetFromFile(p_s_filename, -1.0d, -1.0d);
        }

        /// <summary>
        /// Loading training data entries from a file, using quotas to split entries as training, generalization and validation entries.
        /// sum of both quotas must be lower equal 1.0.
        /// if the sum of both is lower than 1.0, rest will be used for validation.
        /// </summary>
        /// <param name="p_s_filename">filePath + file which contains all training data set entries</param>
        /// <param name="p_d_trainingSetQuota">percentage as double how many entries will be used for training</param>
        /// <param name="p_d_generalizationSetQuota">percentage as double how many entries will be used for generalization</param>
        /// <exception cref="ArgumentException">wrong quota values or wrong values from file for a data entry</exception>
        /// <exception cref="System.IO.FileNotFoundException">invalid filePath or fileName</exception>
        /// <exception cref="System.IO.IOException">file could not be read</exception>
        public void LoadTrainingDataSetFromFile(string p_s_filename, double p_d_trainingSetQuota, double p_d_generalizationSetQuota)
        {
            List<DataEntry> a_data = [];
            this.o_trainingDataSet = new();

            /* check if file exists */
            if (!ForestNET.Lib.IO.File.Exists(p_s_filename))
            {
                throw new System.IO.FileNotFoundException("File[" + p_s_filename + "] does not exist");
            }

            /* use quota parameters if there are not -1.0 */
            if ((p_d_trainingSetQuota != -1.0d) && (p_d_generalizationSetQuota != -1.0d))
            {
                this.SetTrainingQuotas(p_d_trainingSetQuota, p_d_generalizationSetQuota);
            }

            /* open file */
            ForestNET.Lib.IO.File o_file = new(p_s_filename, false);

            /* iterate each file line */
            for (int i_line = 1; i_line <= o_file.FileLines; i_line++)
            {
                /* separate stored training data set values with delimiter property */
                string[] a_lineValues = o_file.ReadLine(i_line).Split(this.s_delimiter);

                /* add file line to array, if amount of elements in line is correct */
                if (a_lineValues.Length == (this.a_layers[0] + this.a_layers[this.a_layers.Count - 1]))
                {
                    DataEntry o_dataEntry = new();

                    for (int i = 0; i < (this.a_layers[0] + this.a_layers[this.a_layers.Count - 1]); i++)
                    {
                        if (i < this.a_layers[0])
                        {
                            o_dataEntry.Pattern.Add(Convert.ToDouble(a_lineValues[i], System.Globalization.CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            o_dataEntry.Target.Add(Convert.ToDouble(a_lineValues[i], System.Globalization.CultureInfo.InvariantCulture));
                        }
                    }

                    a_data.Add(o_dataEntry);
                }
                else
                {
                    throw new ArgumentException("Training set line has '" + a_lineValues.Length + "' values which is not '" + (this.a_layers[0] + this.a_layers[this.a_layers[this.a_layers.Count - 1]]) + "', because our neural network has '" + this.a_layers[0] + "' input values and '" + this.a_layers[this.a_layers[this.a_layers.Count - 1]] + "' output values");
                }
            }

            /* shuffle data entries, so training will be more effective */
            ForestNET.Lib.Helper.ShuffleList(a_data);

            /* calculate our training, generalization and validation entry size */
            int i_trainingDataEndIndex = (int)Math.Round(this.d_trainingSetQuota * Convert.ToDouble(a_data.Count));
            int i_generalizationSize = (int)Math.Round(this.d_generalizationSetQuota * Convert.ToDouble(a_data.Count));

            /* load data entries to training set */
            for (int i = 0; i < i_trainingDataEndIndex; i++)
            {
                this.o_trainingDataSet.TrainingSet.Add(a_data[i]);
            }

            /* load data entries to generalization set */
            for (int i = i_trainingDataEndIndex; i < i_trainingDataEndIndex + i_generalizationSize; i++)
            {
                this.o_trainingDataSet.GeneralizationSet.Add(a_data[i]);
            }

            /* load data entries to validation set */
            for (int i = i_trainingDataEndIndex + i_generalizationSize; i < a_data.Count; i++)
            {
                this.o_trainingDataSet.ValidationSet.Add(a_data[i]);
            }
        }
    }
}