namespace ForestNET.Lib.Log
{
    /// <summary>
    /// Defines general logging levels.
    /// </summary>
    public enum Level
    {
        /// <summary>
        /// Use all log levels.
        /// </summary>
        ALL = 0xFF,

        /// <summary>
        /// Use all log levels between SEVERE .. FINEST.
        /// </summary>
        ALMOST_ALL = 0x7F,

        /// <summary>
        /// Logs messages which are used to debug information of the application in it's most detail level. For example each byte of a network message.
        /// </summary>
        MASS = 0x80,

        /// <summary>
        /// Logs messages which are used to debug information of the application. Debug level 3.
        /// </summary>
        FINEST = 0x40,

        /// <summary>
        /// Logs messages which are used to debug information of the application. Debug level 2.
        /// </summary>
        FINER = 0x20,

        /// <summary>
        /// Logs messages which are used to debug information of the application. Debug level 1.
        /// </summary>
        FINE = 0x10,

        /// <summary>
        /// Logs messages which are used to check configuration settings and instantiation information.
        /// </summary>
        CONFIG = 0x08,

        /// <summary>
        /// Log message for general information of the application.
        /// </summary>
        INFO = 0x04,

        /// <summary>
        /// Log messages for abnormal or unexpected event within the application.
        /// </summary>
        WARNING = 0x02,

        /// <summary>
        /// Log messages for critical states or system crash.
        /// </summary>
        SEVERE = 0x01,

        /// <summary>
        /// Not using log messages at all. Not writing any messages.
        /// </summary>
        OFF = 0x00
    }
}
