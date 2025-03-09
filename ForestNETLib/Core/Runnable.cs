namespace ForestNETLib.Core
{
    /// <summary>
    /// All necessary tools to create a runnable thread method with any class and using it's individual properties
    /// </summary> 
    public abstract class Runnable
    {
        /// <summary>
        /// started flag which can be used to stop thread run-method by breaking (endless) loop. Initialized with false.
        /// </summary>
        public bool Started { get; set; } = false;
        /// <summary>
        /// stop flag which can be used to stop thread run-method by breaking (endless) loop. Initialized with false.
        /// </summary>
        public bool Stop { get; set; } = false;
        /// <summary>
        /// the object's run method to be called separately as a thread
        /// </summary>
        public abstract void Run();
    }
}
