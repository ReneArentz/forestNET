namespace ForestNET.Lib.AI
{
    /// <summary>
    /// Trainer class to train a neural network with backpropagation algorithm.
    /// </summary>
    public class NeuralNetworkTrainer
    {

        /* Delegates */

        /// <summary>
        /// delegate definition which can be instanced outside of NeuralNetworkTrainer class to post progress anywhere when a neural network will be trained
        /// </summary>
        public delegate void PostProgress(long p_l_epoch, long p_l_maxEpochs, double p_d_desiredAccuracy, double p_d_trainingSetAccuracy, double p_d_generalizationSetAccuracy, double p_d_validationSetAccuracy);

        /* Fields */

        private readonly NeuralNetwork o_neuralNetwork;

        private double d_learningRate;
        private double d_momentum;
        private long l_maxEpochs;
        private double d_desiredAccuracy;

        private readonly List<List<List<double>>> a_delta;
        private readonly List<List<double>> a_errorGradients;

        private double d_trainingSetAccuracy;
        private double d_generalizationSetAccuracy;
        private double d_validationSetAccuracy;

        private bool b_useBatch;

        private PostProgress? del_postProgress;

        /* Properties */

        public double TrainingSetAccuracy
        {
            get
            {
                return this.d_trainingSetAccuracy;
            }
        }

        public double GeneralizationSetAccuracy
        {
            get
            {
                return this.d_generalizationSetAccuracy;
            }
        }

        public double ValidationSetAccuracy
        {
            get
            {
                return this.d_validationSetAccuracy;
            }
        }

        public double DesiredAccuracy
        {
            get
            {
                return this.d_desiredAccuracy;
            }
            set
            {
                if (value <= 0.0d)
                {
                    throw new ArgumentException("Desired accuracy parameter must be greater than 0.0, but it is '" + value + "'");
                }

                if (value > 100.0d)
                {
                    throw new ArgumentException("Desired accuracy parameter must be lower than 100.0, but it is '" + value + "'");
                }

                this.d_desiredAccuracy = value;
            }
        }

        public long MaxEpochs
        {
            get
            {
                return this.l_maxEpochs;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Max epochs parameter must be greater than 0, but it is '" + value + "'");
                }

                this.l_maxEpochs = value;
            }
        }

        public double LearningRate
        {
            get { return this.d_learningRate; }
            set
            {
                if (value <= 0.0d)
                {
                    throw new ArgumentException("Learning rate parameter must be greater than 0.0, but it is '" + value + "'");
                }

                if (value > 1.0d)
                {
                    throw new ArgumentException("Learning rate parameter must be lower than 1.0, but it is '" + value + "'");
                }

                this.d_learningRate = value;
            }
        }

        public double Momentum
        {
            get { return this.d_momentum; }
            set
            {
                if (value <= 0.0d)
                {
                    throw new ArgumentException("Momentum parameter must be greater than 0.0, but it is '" + value + "'");
                }

                if (value > 1.0d)
                {
                    throw new ArgumentException("Momentum parameter must be lower than 1.0, but it is '" + value + "'");
                }

                this.d_momentum = value;
            }
        }

        public bool UseBatch
        {
            get
            {
                return this.b_useBatch;
            }
            set
            {
                this.b_useBatch = value;
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
        /// Constructor of neural network trainer class - default values: learning rate (0.001), momentum (0.9), max epochs (1500), desired accuracy (90), useBatch (false)
        /// </summary>
        /// <param name="p_o_neuralNetwork">neural network which will be trained with this trainer class</param>
        /// <exception cref="ArgumentException">invalid neural network parameter</exception>
        public NeuralNetworkTrainer(NeuralNetwork p_o_neuralNetwork) :
            this(p_o_neuralNetwork, null)
        {

        }

        /// <summary>
        /// Constructor of neural network trainer class - default values: learning rate (0.001), momentum (0.9), max epochs (1500), desired accuracy (90), useBatch (false)
        /// </summary>
        /// <param name="p_o_neuralNetwork">neural network which will be trained with this trainer class</param>
        /// <param name="p_del_postProgress">delegate to post progress of neural network training after each epoch</param>
        /// <exception cref="ArgumentException">invalid neural network parameter</exception>
        public NeuralNetworkTrainer(NeuralNetwork p_o_neuralNetwork, PostProgress? p_del_postProgress)
        {
            /* check for a valid neural network parameter */
            if (p_o_neuralNetwork == null)
            {
                throw new ArgumentException("Parameter neural network is null");
            }
            else
            {
                this.o_neuralNetwork = p_o_neuralNetwork;
            }

            /* use delegate parameter if not null */
            if (p_del_postProgress != null)
            {
                this.del_postProgress = p_del_postProgress;
            }

            /* set default values */
            this.d_learningRate = 0.001d;
            this.d_momentum = 0.9d;

            this.l_maxEpochs = 1500L;

            this.d_desiredAccuracy = 90.0d;

            this.d_trainingSetAccuracy = 0.0d;
            this.d_generalizationSetAccuracy = 0.0d;
            this.d_validationSetAccuracy = 0.0d;

            this.b_useBatch = false;

            /* initialize delta array 1st dimension  */
            this.a_delta = new(this.o_neuralNetwork.Layers.Count - 1);

            /* we iterate each layer to get to know how many deltas we must store and how many deltas explicitly are between them */
            for (int i = 0; i < this.o_neuralNetwork.Layers.Count - 1; i++)
            { /* size - 1, cause there are just size - 1 delta layers between all layers */
                /* initialize delta array 2nd dimension */
                this.a_delta.Add(new(this.o_neuralNetwork.Layers[i] + 1));

                /* for each neuron in layer i */
                for (int j = 0; j <= this.o_neuralNetwork.Layers[i]; j++)
                {
                    /* initialize delta array 3rd dimension */
                    this.a_delta[i].Add(new(this.o_neuralNetwork.Layers[i + 1]));

                    /* add 3rd dimension as zero values, for each neuron in layer i + 1 */
                    for (int k = 0; k < this.o_neuralNetwork.Layers[i + 1]; k++)
                    {
                        this.a_delta[i][j].Add(0.0d);
                    }
                }
            }

            /* initialize error gradient 1st dimension */
            this.a_errorGradients = new(this.o_neuralNetwork.Layers.Count - 1);

            /* we iterate each layer to get to know how many error gradients we must store, but not the first layer */
            for (int i = 0; i < this.o_neuralNetwork.Layers.Count - 1; i++)
            { /* size - 1, cause there are just size - 1 error gradient layers between all layers */
                /* initialize delta array 2nd dimension */
                this.a_errorGradients.Add(new(this.o_neuralNetwork.Layers[i + 1] + 1));

                /* set error gradients array as zero values */
                for (int j = 0; j <= this.o_neuralNetwork.Layers[i + 1]; j++)
                {
                    this.a_errorGradients[i].Add(0.0d);
                }
            }
        }

        /// <summary>
        /// Method to start training of the neural network
        /// </summary>
        public void TrainNetwork()
        {
            /* set training epoch value to zero */
            long l_epoch = 0L;

            /* train the network using the training dataset and the generalization dataset for testing */
            while ((this.d_trainingSetAccuracy < this.d_desiredAccuracy || ((this.d_generalizationSetAccuracy < this.d_desiredAccuracy) && (this.d_trainingSetAccuracy != 100.0d))) && (l_epoch < this.l_maxEpochs))
            {
                /* train neural network with training set */
                this.ExecuteTrainingEpoch((this.o_neuralNetwork.TrainingDataSet ?? throw new NullReferenceException("Training data set of neural network is null")).TrainingSet);

                /* get generalization accuracy of current neural network by generalization set and desired accuracy */
                this.d_generalizationSetAccuracy = this.o_neuralNetwork.GetNetworkAccuracy(this.o_neuralNetwork.TrainingDataSet.GeneralizationSet, this.d_desiredAccuracy);

                /* increase training epoch value after trained one epoch */
                l_epoch++;

                /* post progress of current training */
                this.del_postProgress?.Invoke(l_epoch, this.l_maxEpochs, this.d_desiredAccuracy, this.d_trainingSetAccuracy, this.d_generalizationSetAccuracy, this.d_validationSetAccuracy);
            }

            /* get validation accuracy of current neural network by validation set and desired accuracy */
            this.d_validationSetAccuracy = this.o_neuralNetwork.GetNetworkAccuracy((this.o_neuralNetwork.TrainingDataSet ?? throw new NullReferenceException("Training data set of neural network is null")).ValidationSet, this.d_desiredAccuracy);
        }

        /// <summary>
        /// method which executed one training epoch with training set
        /// </summary>
        /// <param name="a_trainingSet">training set with input patterns and expected output patterns</param>
        private void ExecuteTrainingEpoch(List<DataEntry> a_trainingSet)
        {
            /* variable for the number of false patterns */
            double d_incorrectPatterns = 0;

            /* iterate each training data entry */
            for (int i = 0; i < (int)a_trainingSet.Count; i++)
            {
                /* process input pattern through neural network */
                _ = this.o_neuralNetwork.FeedForwardPattern(a_trainingSet[i].Pattern);

                /* use backpropagate algorithm with expected output pattern */
                this.Backpropagate(a_trainingSet[i].Target);

                /* compare neural network output with expected output from training data entry */
                for (int j = 0; j < this.o_neuralNetwork.Layers[this.o_neuralNetwork.Layers.Count - 1]; j++)
                {
                    /* pattern is wrong if calculated output differs from known output */
                    if (!NeuralNetwork.ClampOutput(this.o_neuralNetwork.Neurons[this.o_neuralNetwork.Layers.Count - 1][j], a_trainingSet[i].Target[j], this.d_desiredAccuracy))
                    {
                        /* comparison failed, so we increase incorrect result counter and break the for loop */
                        d_incorrectPatterns++;
                        break;
                    }
                }

            }

            /* useBatch = true, update weights after training epoch finished with all training data entries */
            if (this.b_useBatch)
            {
                this.UpdateWeights();
            }

            /* calculate training set accuracy with current incorrect result counter */
            this.d_trainingSetAccuracy = 100.0d - (d_incorrectPatterns / Convert.ToDouble(a_trainingSet.Count) * 100.0d);
        }

        /// <summary>
        /// backpropagation algorithm method
        /// </summary>
        /// <param name="a_desiredOutputs">desired output pattern from training data entry</param>
        private void Backpropagate(List<double> a_desiredOutputs)
        {
            int i_lastLayerIndex = this.o_neuralNetwork.Layers.Count - 1;
            int i_secondToLastLayerIndex = this.o_neuralNetwork.Layers.Count - 2;

            /* modify the deltas between second to last layer and last layer */
            for (int i = 0; i < this.o_neuralNetwork.Layers[i_lastLayerIndex]; i++)
            {
                /* get error gradients for each output neuron */
                double d_outputErrorGradient = this.o_neuralNetwork.Neurons[i_lastLayerIndex][i] * (1.0d - this.o_neuralNetwork.Neurons[i_lastLayerIndex][i]) * (a_desiredOutputs[i] - this.o_neuralNetwork.Neurons[i_lastLayerIndex][i]);
                /* set output error gradient */
                this.a_errorGradients[i_secondToLastLayerIndex][i] = d_outputErrorGradient;

                /* for all neurons in the second to last layer and the bias neuron */
                for (int j = 0; j <= this.o_neuralNetwork.Layers[i_secondToLastLayerIndex]; j++)
                {
                    /* calculation of weight changes, for batch learning (with momentum) and normal (stochastic) learning */
                    if (!this.b_useBatch)
                    {
                        this.a_delta[i_secondToLastLayerIndex][j][i] = this.d_learningRate * this.o_neuralNetwork.Neurons[i_secondToLastLayerIndex][j] * this.a_errorGradients[i_secondToLastLayerIndex][i] + this.d_momentum * this.a_delta[i_secondToLastLayerIndex][j][i];
                    }
                    else
                    {
                        this.a_delta[i_secondToLastLayerIndex][j][i] = this.d_learningRate * this.o_neuralNetwork.Neurons[i_secondToLastLayerIndex][j] * this.a_errorGradients[i_secondToLastLayerIndex][i] + this.a_delta[i_secondToLastLayerIndex][j][i];
                    }
                }
            }

            /* iterate each layer from second to last to first one */
            for (int i = i_secondToLastLayerIndex; i > 0; i--)
            {
                /* modify the deltas between layer i and layer i-1 */
                for (int j = 0; j < this.o_neuralNetwork.Layers[i]; j++)
                {
                    /* the sum of the weights between both layers multiplied by the error gradient */
                    double d_sum = 0;

                    /* sum it up for each neuron */
                    for (int k = 0; k < this.o_neuralNetwork.Layers[i + 1]; k++)
                    {
                        d_sum += this.o_neuralNetwork.Weights[i][j][k] * this.a_errorGradients[i][k];
                    }

                    /* set error gradients for the neuron */
                    this.a_errorGradients[i - 1][j] = this.o_neuralNetwork.Neurons[i][j] * (1.0d - this.o_neuralNetwork.Neurons[i][j]) * d_sum;

                    /* for all neurons in layer i - 1 and the bias neuron */
                    for (int k = 0; k <= this.o_neuralNetwork.Layers[i - 1]; k++)
                    {
                        /* calculation of weight changes, for batch learning (with momentum) and normal (stochastic) learning */
                        if (!this.b_useBatch)
                        {
                            this.a_delta[i - 1][k][j] = this.d_learningRate * this.o_neuralNetwork.Neurons[i - 1][k] * this.a_errorGradients[i - 1][j] + this.d_momentum * this.a_delta[i - 1][k][j];
                        }
                        else
                        {
                            this.a_delta[i - 1][k][j] = this.d_learningRate * this.o_neuralNetwork.Neurons[i - 1][k] * this.a_errorGradients[i - 1][j] + this.a_delta[i - 1][k][j];
                        }
                    }
                }
            }

            /* useBatch = false, update weights after each training data entry */
            if (!b_useBatch)
            {
                this.UpdateWeights();
            }
        }

        /// <summary>
        /// method which updates all weights within the backpropagate algorithm
        /// can be used after each training epoch or each training data entry
        /// </summary>
        private void UpdateWeights()
        {
            /* iterate each layer */
            for (int i = 0; i < this.o_neuralNetwork.Layers.Count - 1; i++)
            { /* size - 1, cause there are just size - 1 delta layers between all layers */
                /* update weights from layer i to layer i+1 */
                for (int j = 0; j <= this.o_neuralNetwork.Layers[i]; j++)
                {
                    for (int k = 0; k < this.o_neuralNetwork.Layers[i + 1]; k++)
                    {
                        /* update weights, add up delta values */
                        this.o_neuralNetwork.Weights[i][j][k] = this.o_neuralNetwork.Weights[i][j][k] + this.a_delta[i][j][k];

                        /* if batch learning, then delta value is zero otherwise previous delta is needed for momentum */
                        if (this.b_useBatch)
                        {
                            this.a_delta[i][j][k] = 0.0d;
                        }
                    }
                }
            }
        }
    }
}