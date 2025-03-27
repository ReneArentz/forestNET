namespace ForestNET.Lib.Log
{
    public static class LevelExtensions
    {
        /// <summary>
        /// Converts a string log level parameter to a matching enum level
        /// </summary>
        /// <param name="p_s_level">string log level parameter</param>
        /// <returns>matching enum level, if no match succeeds it will return Level.OFF</returns>
        /// <exception cref="ArgumentNullException">parameter is not set</exception>
        public static Level StringToLevel(string p_s_level)
        {
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_level))
            {
                throw new ArgumentNullException(nameof(p_s_level), "Level parameter is null");
            }

            if (p_s_level.ToUpper().Equals("OFF"))
            {
                return Level.OFF;
            }
            else if (p_s_level.ToUpper().Equals("SEVERE"))
            {
                return Level.SEVERE;
            }
            else if (p_s_level.ToUpper().Equals("WARNING"))
            {
                return Level.WARNING;
            }
            else if (p_s_level.ToUpper().Equals("INFO"))
            {
                return Level.INFO;
            }
            else if (p_s_level.ToUpper().Equals("CONFIG"))
            {
                return Level.CONFIG;
            }
            else if (p_s_level.ToUpper().Equals("FINE"))
            {
                return Level.FINE;
            }
            else if (p_s_level.ToUpper().Equals("FINER"))
            {
                return Level.FINER;
            }
            else if (p_s_level.ToUpper().Equals("FINEST"))
            {
                return Level.FINEST;
            }
            else if (p_s_level.ToUpper().Equals("MASS"))
            {
                return Level.MASS;
            }
            else if (p_s_level.ToUpper().Equals("ALMOST_ALL"))
            {
                return Level.ALMOST_ALL;
            }
            else if (p_s_level.ToUpper().Equals("ALL"))
            {
                return Level.ALL;
            }
            else
            {
                return Level.OFF;
            }
        }

        /// <summary>
        /// Converts a string parameter to a matching microsoft logging level enum
        /// </summary>
        /// <param name="p_s_level">string parameter</param>
        /// <returns>matching microsoft logging level enum, if no match succeeds it will return Microsoft.Extensions.Logging.LogLevel.None</returns>
        /// <exception cref="ArgumentNullException">parameter is not set</exception>
        public static Microsoft.Extensions.Logging.LogLevel StringToNETLogLevel(string p_s_level)
        {
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_level))
            {
                throw new ArgumentNullException(nameof(p_s_level), "Microsoft logging level parameter is null");
            }

            if (p_s_level.ToUpper().Equals("CRITICAL"))
            {
                return Microsoft.Extensions.Logging.LogLevel.Critical;
            }
            else if (p_s_level.ToUpper().Equals("ERROR"))
            {
                return Microsoft.Extensions.Logging.LogLevel.Error;
            }
            else if (p_s_level.ToUpper().Equals("WARNING"))
            {
                return Microsoft.Extensions.Logging.LogLevel.Warning;
            }
            else if (p_s_level.ToUpper().Equals("INFO"))
            {
                return Microsoft.Extensions.Logging.LogLevel.Information;
            }
            else if (p_s_level.ToUpper().Equals("INFORMATION"))
            {
                return Microsoft.Extensions.Logging.LogLevel.Information;
            }
            else if (p_s_level.ToUpper().Equals("DEBUG"))
            {
                return Microsoft.Extensions.Logging.LogLevel.Debug;
            }
            else if (p_s_level.ToUpper().Equals("TRACE"))
            {
                return Microsoft.Extensions.Logging.LogLevel.Trace;
            }
            else if (p_s_level.ToUpper().Equals("NONE"))
            {
                return Microsoft.Extensions.Logging.LogLevel.None;
            }
            else
            {
                return Microsoft.Extensions.Logging.LogLevel.None;
            }
        }

        /// <summary>
        /// Converts a enum log level parameter to a matching logging level as string
        /// </summary>
        /// <param name="p_e_level">enum log level parameter</param>
        /// <returns>matching logging level as string</returns>
        /// <exception cref="ArgumentException">parameter is not set correctly</exception>
        public static string LevelToString(Level p_e_level)
        {
            if (p_e_level == Level.OFF)
            {
                return "OFF";
            }
            else if (p_e_level == Level.SEVERE)
            {
                return "SEVERE";
            }
            else if (p_e_level == Level.WARNING)
            {
                return "WARNING";
            }
            else if (p_e_level == Level.INFO)
            {
                return "INFO";
            }
            else if (p_e_level == Level.CONFIG)
            {
                return "CONFIG";
            }
            else if (p_e_level == Level.FINE)
            {
                return "FINE";
            }
            else if (p_e_level == Level.FINER)
            {
                return "FINER";
            }
            else if (p_e_level == Level.FINEST)
            {
                return "FINEST";
            }
            else if (p_e_level == Level.MASS)
            {
                return "MASS";
            }
            else if (p_e_level == Level.ALMOST_ALL)
            {
                return "ALMOST_ALL";
            }
            else if (p_e_level == Level.ALL)
            {
                return "ALL";
            }
            else
            {
                throw new ArgumentException("Level parameter is illegal");
            }
        }

        /// <summary>
        /// Converts a microsoft logging level to a matching enum logging level of ForestNETLib
        /// </summary>
        /// <param name="p_o_logLevel">microsoft logging level</param>
        /// <returns>enum logging level of ForestNETLib</returns>
        /// <exception cref="ArgumentException">microsoft logging level parameter is illegal</exception>
        public static Level NETLogLevelToLevel(Microsoft.Extensions.Logging.LogLevel p_o_logLevel)
        {
            if (p_o_logLevel == Microsoft.Extensions.Logging.LogLevel.None)
            {
                return Level.OFF;
            }
            else if (p_o_logLevel == Microsoft.Extensions.Logging.LogLevel.Critical)
            {
                return Level.SEVERE;
            }
            else if (p_o_logLevel == Microsoft.Extensions.Logging.LogLevel.Error)
            {
                return Level.SEVERE;
            }
            else if (p_o_logLevel == Microsoft.Extensions.Logging.LogLevel.Warning)
            {
                return Level.WARNING;
            }
            else if (p_o_logLevel == Microsoft.Extensions.Logging.LogLevel.Information)
            {
                return Level.INFO;
            }
            else if (p_o_logLevel == Microsoft.Extensions.Logging.LogLevel.Debug)
            {
                return Level.FINE;
            }
            else if (p_o_logLevel == Microsoft.Extensions.Logging.LogLevel.Trace)
            {
                return Level.FINER;
            }
            else
            {
                throw new ArgumentException("Microsoft logging level parameter is illegal");
            }
        }

        /// <summary>
        /// Converts a enum logging level of ForestNETLib to a matching microsoft logging level
        /// </summary>
        /// <param name="p_o_logLevel">enum logging level of ForestNETLib</param>
        /// <returns>microsoft logging level</returns>
        /// <exception cref="ArgumentException">enum logging level parameter of ForestNETLib is illegal</exception>
        public static Microsoft.Extensions.Logging.LogLevel LevelToNETLogLevel(Level p_e_level)
        {
            if (p_e_level == Level.OFF)
            {
                return Microsoft.Extensions.Logging.LogLevel.None;
            }
            else if (p_e_level == Level.SEVERE)
            {
                return Microsoft.Extensions.Logging.LogLevel.Error;
            }
            else if (p_e_level == Level.WARNING)
            {
                return Microsoft.Extensions.Logging.LogLevel.Warning;
            }
            else if (p_e_level == Level.INFO)
            {
                return Microsoft.Extensions.Logging.LogLevel.Information;
            }
            else if (p_e_level == Level.CONFIG)
            {
                return Microsoft.Extensions.Logging.LogLevel.Debug;
            }
            else if (p_e_level == Level.FINE)
            {
                return Microsoft.Extensions.Logging.LogLevel.Debug;
            }
            else if (p_e_level == Level.FINER)
            {
                return Microsoft.Extensions.Logging.LogLevel.Trace;
            }
            else if (p_e_level == Level.FINEST)
            {
                return Microsoft.Extensions.Logging.LogLevel.Trace;
            }
            else if (p_e_level == Level.MASS)
            {
                return Microsoft.Extensions.Logging.LogLevel.Trace;
            }
            else if (p_e_level == Level.ALMOST_ALL)
            {
                return Microsoft.Extensions.Logging.LogLevel.Trace;
            }
            else if (p_e_level == Level.ALL)
            {
                return Microsoft.Extensions.Logging.LogLevel.Trace;
            }
            else
            {
                throw new ArgumentException("enum logging level parameter is illegal");
            }
        }
    }
}
