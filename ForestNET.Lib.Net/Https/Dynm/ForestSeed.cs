namespace ForestNET.Lib.Net.Https.Dynm
{
    /// <summary>
    /// Abstract class for ForestSeed implementation. Makes it possible to prepare content and access other resource, like DB, and set temporary values in Seed list for rendering process.
    /// </summary>
    public abstract class ForestSeed
    {

        /* Fields */

        private string? s_linebreak;

        /* Properties */

        protected ForestNET.Lib.Net.Https.Seed? Seed { get; private set; }
        /// <summary>
		/// set used line break within html or htm files which using dynamic content
		/// </summary>
        /// <exception cref="ArgumentNullException">line break parameter is null or empty</exception>
        public string? LineBreak
        {
            get
            {
                return this.s_linebreak;
            }
            protected set
            {
                if ((value == null) || (value.Length < 1))
                {
                    throw new ArgumentNullException(nameof(value), "Line break parameter is null or empty");
                }

                this.s_linebreak = value;
            }
        }

        /* Methods */

        /// <summary>
        /// The core dynamic method which takes a seed instance as parameter, so anything within preparing content has access to all request/response/other resources
        /// </summary>
        /// <param name="p_o_value">seed instance object</param>
        /// <exception cref="Exception">any exception which occurred while fetching content</exception>
        public void FetchContent(ForestNET.Lib.Net.Https.Seed p_o_value)
        {
            this.Seed = p_o_value;
            this.LineBreak = ForestNET.Lib.IO.File.NEWLINE;
            this.PrepareContent();
        }

        /// <summary>
        /// Method which must be created and will be called during fetching content
        /// </summary>
        /// <exception cref="Exception">any exception which occurred while preparing content</exception>
        abstract public void PrepareContent();
    }
}
