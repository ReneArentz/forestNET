namespace ForestNETLib.Core
{
    /// <summary>
    /// Global singleton class to store central values and objects, global log methods using Microsoft.Extensions.Logging.
    /// </summary>
    public sealed class Global
    {

        /* Fields */

        private static readonly Lazy<Global> o_instance = new(() => new Global());
        
        /* Properties */

        /// <summary>
        /// property to access singleton instance
        /// </summary>
        /// <returns>Global singleton</returns>
        public static Global Instance
        {
            get
            {
                return o_instance.Value;
            }
        }
        public System.Security.Cryptography.RandomNumberGenerator RandomNumberGenerator { get; private set; }
        
        /* Methods */

        /// <summary>
        /// private constructor of singleton class, can set standard values for objects or settings
        /// </summary>
        private Global()
        {
            this.RandomNumberGenerator = System.Security.Cryptography.RandomNumberGenerator.Create();
        }

        /// <summary>
        /// Disposing global instance and releasing all resources
        /// </summary>
        public void Dispose()
        {
            /* dispose random number generator */
            this.RandomNumberGenerator.Dispose();
        }
    }
}
