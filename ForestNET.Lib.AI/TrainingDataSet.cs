namespace ForestNET.Lib.AI
{
    /// <summary>
    /// Class for storing a complete training data set with training, generalization and validation proportion.
    /// </summary>
    public class TrainingDataSet
    {

        /* Fields */

        /* Properties */

        public List<DataEntry> TrainingSet { get; private set; }
        public List<DataEntry> GeneralizationSet { get; private set; }
        public List<DataEntry> ValidationSet { get; private set; }

        /* Methods */

        public TrainingDataSet()
        {
            this.TrainingSet = [];
            this.GeneralizationSet = [];
            this.ValidationSet = [];
        }
    }
}
