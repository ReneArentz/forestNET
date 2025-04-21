namespace ForestNET.Lib.AI
{
    /// <summary>
    /// Class for storing training data entries with input patterns and expected target output patterns.
    /// </summary>
    public class DataEntry
    {

        /* Fields */

        /* Properties */

        public List<double> Pattern { get; set; }
        public List<double> Target { get; set; }

        /* Methods */

        public DataEntry()
        {
            this.Pattern = [];
            this.Target = [];
        }

        public DataEntry(List<double> p_a_pattern, List<double> p_a_target)
        {
            this.Pattern = p_a_pattern;
            this.Target = p_a_target;
        }
    }
}