using ForestNET.Lib.Log;

namespace ForestNET.Lib
{
    /// <summary>
    /// Global singleton class to store central values and objects, global log methods using Microsoft.Extensions.Logging.
    /// </summary>
    public sealed class Global
    {

        /* Delegates */

        /// <summary>
        /// interface which can be implemented to add or use other log functionality
        /// </summary>
        public delegate void OtherLogImplementation(bool p_b_internalLog, byte p_by_logLevel, string p_s_callerFilePath, string p_s_callerMemberName, int p_i_callerLineNumber, string p_s_logMessage);

        /* Fields */

        private static readonly Lazy<Global> o_instance = new(() => new Global());
        private readonly LogConfig o_logConfig;

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
        public byte InternalLogControl { get; set; }
        public byte LogControl { get; set; }
        public Microsoft.Extensions.Logging.ILogger? LOG { get; private set; }
        public Microsoft.Extensions.Logging.ILogger? ILOG { get; private set; }
        public System.Security.Cryptography.RandomNumberGenerator RandomNumberGenerator { get; private set; }
        public OtherLogImplementation? DelegateLogImplementation { private get; set; }

        /* Methods */

        /// <summary>
        /// private constructor of singleton class, can set standard values for objects or settings
        /// </summary>
        private Global()
        {
            this.o_logConfig = new();

            this.InternalLogControl = (byte)Level.OFF;
            this.LogControl = (byte)Level.SEVERE + (byte)Level.WARNING + (byte)Level.INFO;
            this.RandomNumberGenerator = System.Security.Cryptography.RandomNumberGenerator.Create();
            this.DelegateLogImplementation = null;

            /* initialize logging for first time */
            this.ResetLog();
        }

        /// <summary>
        /// Disposing global instance and releasing all resources
        /// </summary>
        public void Dispose()
        {
            /* dispose current logger config and current logging */
            this.o_logConfig?.Dispose();
            /* dispose random number generator */
            this.RandomNumberGenerator.Dispose();
        }

        /// <summary>
        /// Reset logging factory, clearing delegate log implementation as well
        /// </summary>
        public void ResetLog()
        {
            /* clear delegate log implementation */
            this.DelegateLogImplementation = null;

            /* reset logging factory */
            this.o_logConfig.ResetLog();

            /* create normal and internal logger instances */
            this.LOG = this.o_logConfig.CreateLoggerInstance("Log");
            this.ILOG = this.o_logConfig.CreateLoggerInstance("InternalLog");
        }

        /// <summary>
        /// Reset logging factory to standard values, clearing delegate log implementation as well and reset log controls to standard values
        /// </summary>
        public void ResetLogToStandard()
        {
            /* set log controls to standard values */
            this.InternalLogControl = (byte)Level.OFF;
            this.LogControl = (byte)Level.SEVERE + (byte)Level.WARNING + (byte)Level.INFO;

            /* clear delegate log implementation */
            this.DelegateLogImplementation = null;

            /* reset logging factory to standard values */
            this.o_logConfig.ResetLogToStandard();

            /* create normal and internal logger instances */
            this.LOG = this.o_logConfig.CreateLoggerInstance("Log");
            this.ILOG = this.o_logConfig.CreateLoggerInstance("InternalLog");
        }

        /// <summary>
        /// Set logging settings from a config file and reset logging factory
        /// </summary>
        /// <param name="p_s_configFile">path to configuration file for logging settings</param>
        /// <exception cref="ArgumentException">config file does not exist</exception>
        public void ResetLogFromFile(string p_s_configFile)
        {
            /* set logging factory from config file */
            this.o_logConfig.ResetLogFromFile(p_s_configFile);

            /* create normal and internal logger instances */
            this.LOG = this.o_logConfig.CreateLoggerInstance("Log");
            this.ILOG = this.o_logConfig.CreateLoggerInstance("InternalLog");
        }

        /// <summary>
        /// Set logging settings from a list of string lines and reset logging factory
        /// </summary>
        /// <param name="p_a_configLines">list of string lines for logging settings</param>
        public void ResetLogFromLines(System.Collections.Generic.List<string> p_a_configLines)
        {
            /* set logging factory from config file */
            this.o_logConfig.ResetLogFromLines(p_a_configLines);

            /* create normal and internal logger instances */
            this.LOG = this.o_logConfig.CreateLoggerInstance("Log");
            this.ILOG = this.o_logConfig.CreateLoggerInstance("InternalLog");
        }


        /// <summary>
        /// Check if a level for logging is active
        /// </summary>
        /// <param name="p_by_byteLevel">byte value level [Global.SEVERE, Global.WARNING, Global.INFO, Global.CONFIG, Global.FINE, Global.FINER, Global.FINEST, Global.MASS]</param>
        /// <returns>true - log level is active, false - log level is not active</returns>
        public static bool IsLevel(byte p_by_byteLevel)
        {
            return ((Instance.LogControl & p_by_byteLevel) == p_by_byteLevel);
        }

        /// <summary>
        /// Check if a level for internal logging is active
        /// </summary>
        /// <param name="p_by_byteLevel">byte value level [Global.SEVERE, Global.WARNING, Global.INFO, Global.CONFIG, Global.FINE, Global.FINER, Global.FINEST, Global.MASS]</param>
        /// <returns>true - internal log level is active, false - internal log level is not active</returns>
        public static bool IsILevel(byte p_by_byteLevel)
        {
            return ((Instance.InternalLogControl & p_by_byteLevel) == p_by_byteLevel);
        }


        /// <summary>
        /// Create log message with SEVERE log level, if log level is included in logControl field
        /// </summary>
        /// <param name="p_s_log">log message which will be logged</param>
        /// <param name="p_s_callerFilePath">log source path or filled by CallerFilePath</param>
        /// <param name="p_s_callerMemberName">log member name or filled by CallerMemberName</param>
        /// <param name="p_i_callerLineNumber">log line number or filled by CallerLineNumber</param>
        public static void LogSevere(string p_s_log, [System.Runtime.CompilerServices.CallerFilePath] string p_s_callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string p_s_callerMemberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int p_i_callerLineNumber = 0)
        {
            if ((Instance.LogControl & (byte)Level.SEVERE) == (byte)Level.SEVERE)
            {
                Instance.LOG?.LogError(p_s_log, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber);

                /* call other log implementation */
                Instance.DelegateLogImplementation?.Invoke(false, (byte)Level.SEVERE, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber, p_s_log);
            }
        }

        /// <summary>
        /// Create log message with WARNING log level, if log level is included in logControl field
        /// </summary>
        /// <param name="p_s_log">log message which will be logged</param>
        /// <param name="p_s_callerFilePath">log source path or filled by CallerFilePath</param>
        /// <param name="p_s_callerMemberName">log member name or filled by CallerMemberName</param>
        /// <param name="p_i_callerLineNumber">log line number or filled by CallerLineNumber</param>
        public static void LogWarning(string p_s_log, [System.Runtime.CompilerServices.CallerFilePath] string p_s_callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string p_s_callerMemberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int p_i_callerLineNumber = 0)
        {
            if ((Instance.LogControl & (byte)Level.WARNING) == (byte)Level.WARNING)
            {
                Instance.LOG?.LogWarning(p_s_log, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber);

                /* call other log implementation */
                Instance.DelegateLogImplementation?.Invoke(false, (byte)Level.WARNING, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber, p_s_log);
            }
        }

        /// <summary>
        /// Create log message with INFO log level, if log level is included in logControl field
        /// </summary>
        /// <param name="p_s_log">log message which will be logged</param>
        /// <param name="p_s_callerFilePath">log source path or filled by CallerFilePath</param>
        /// <param name="p_s_callerMemberName">log member name or filled by CallerMemberName</param>
        /// <param name="p_i_callerLineNumber">log line number or filled by CallerLineNumber</param>
        public static void Log(string p_s_log, [System.Runtime.CompilerServices.CallerFilePath] string p_s_callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string p_s_callerMemberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int p_i_callerLineNumber = 0)
        {
            if ((Instance.LogControl & (byte)Level.INFO) == (byte)Level.INFO)
            {
                Instance.LOG?.LogInformation(p_s_log, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber);

                /* call other log implementation */
                Instance.DelegateLogImplementation?.Invoke(false, (byte)Level.INFO, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber, p_s_log);
            }
        }

        /// <summary>
        /// Create log message with CONFIG log level, if log level is included in logControl field
        /// </summary>
        /// <param name="p_s_log">log message which will be logged</param>
        /// <param name="p_s_callerFilePath">log source path or filled by CallerFilePath</param>
        /// <param name="p_s_callerMemberName">log member name or filled by CallerMemberName</param>
        /// <param name="p_i_callerLineNumber">log line number or filled by CallerLineNumber</param>
        public static void LogConfig(string p_s_log, [System.Runtime.CompilerServices.CallerFilePath] string p_s_callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string p_s_callerMemberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int p_i_callerLineNumber = 0)
        {
            if ((Instance.LogControl & (byte)Level.CONFIG) == (byte)Level.CONFIG)
            {
                Instance.LOG?.LogDebug(p_s_log, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber);

                /* call other log implementation */
                Instance.DelegateLogImplementation?.Invoke(false, (byte)Level.CONFIG, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber, p_s_log);
            }
        }

        /// <summary>
        /// Create log message with FINE log level, if log level is included in logControl field
        /// </summary>
        /// <param name="p_s_log">log message which will be logged</param>
        /// <param name="p_s_callerFilePath">log source path or filled by CallerFilePath</param>
        /// <param name="p_s_callerMemberName">log member name or filled by CallerMemberName</param>
        /// <param name="p_i_callerLineNumber">log line number or filled by CallerLineNumber</param>
        public static void LogFine(string p_s_log, [System.Runtime.CompilerServices.CallerFilePath] string p_s_callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string p_s_callerMemberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int p_i_callerLineNumber = 0)
        {
            if ((Instance.LogControl & (byte)Level.FINE) == (byte)Level.FINE)
            {
                Instance.LOG?.LogDebug(p_s_log, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber);

                /* call other log implementation */
                Instance.DelegateLogImplementation?.Invoke(false, (byte)Level.FINE, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber, p_s_log);
            }
        }

        /// <summary>
        /// Create log message with FINER log level, if log level is included in logControl field
        /// </summary>
        /// <param name="p_s_log">log message which will be logged</param>
        /// <param name="p_s_callerFilePath">log source path or filled by CallerFilePath</param>
        /// <param name="p_s_callerMemberName">log member name or filled by CallerMemberName</param>
        /// <param name="p_i_callerLineNumber">log line number or filled by CallerLineNumber</param>
        public static void LogFiner(string p_s_log, [System.Runtime.CompilerServices.CallerFilePath] string p_s_callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string p_s_callerMemberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int p_i_callerLineNumber = 0)
        {
            if ((Instance.LogControl & (byte)Level.FINER) == (byte)Level.FINER)
            {
                Instance.LOG?.LogTrace(p_s_log, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber);

                /* call other log implementation */
                Instance.DelegateLogImplementation?.Invoke(false, (byte)Level.FINER, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber, p_s_log);
            }
        }

        /// <summary>
        /// Create log message with FINEST log level, if log level is included in logControl field
        /// </summary>
        /// <param name="p_s_log">log message which will be logged</param>
        public static void LogFinest(string p_s_log, [System.Runtime.CompilerServices.CallerFilePath] string p_s_callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string p_s_callerMemberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int p_i_callerLineNumber = 0)
        {
            if ((Instance.LogControl & (byte)Level.FINEST) == (byte)Level.FINEST)
            {
                Instance.LOG?.LogTrace(p_s_log, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber);

                /* call other log implementation */
                Instance.DelegateLogImplementation?.Invoke(false, (byte)Level.FINEST, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber, p_s_log);
            }
        }

        /// <summary>
        /// Create log message with MASS log level, if log level is included in logControl field
        /// </summary>
        /// <param name="p_s_log">log message which will be logged</param>
        /// <param name="p_s_callerFilePath">log source path or filled by CallerFilePath</param>
        /// <param name="p_s_callerMemberName">log member name or filled by CallerMemberName</param>
        /// <param name="p_i_callerLineNumber">log line number or filled by CallerLineNumber</param>
        public static void LogMass(string p_s_log, [System.Runtime.CompilerServices.CallerFilePath] string p_s_callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string p_s_callerMemberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int p_i_callerLineNumber = 0)
        {
            if ((Instance.LogControl & (byte)Level.MASS) == (byte)Level.MASS)
            {
                Instance.LOG?.LogTrace(p_s_log, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber);

                /* call other log implementation */
                Instance.DelegateLogImplementation?.Invoke(false, (byte)Level.MASS, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber, p_s_log);
            }
        }


        /// <summary>
        /// method to log an exception with microsoft.extension.logging tools
        /// </summary>
        /// <param name="p_o_exc">exception which will be add to the log as severe</param>
        /// <param name="p_s_callerFilePath">log source path or filled by CallerFilePath</param>
        /// <param name="p_s_callerMemberName">log member name or filled by CallerMemberName</param>
        /// <param name="p_i_callerLineNumber">log line number or filled by CallerLineNumber</param>
        public static void LogException(Exception p_o_exc, [System.Runtime.CompilerServices.CallerFilePath] string p_s_callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string p_s_callerMemberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int p_i_callerLineNumber = 0)
        {
            Global.LogException(null, p_o_exc, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber);
        }

        /// <summary>
        /// method to log an exception with microsoft.extension.logging tools
        /// </summary>
        /// <param name="p_s_caption">caption before exception message</param>
        /// <param name="p_o_exc">exception which will be add to the log as severe</param>
        /// <param name="p_s_callerFilePath">log source path or filled by CallerFilePath</param>
        /// <param name="p_s_callerMemberName">log member name or filled by CallerMemberName</param>
        /// <param name="p_i_callerLineNumber">log line number or filled by CallerLineNumber</param>
        public static void LogException(string? p_s_caption, Exception p_o_exc, [System.Runtime.CompilerServices.CallerFilePath] string p_s_callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string p_s_callerMemberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int p_i_callerLineNumber = 0)
        {
            /* set caption to emtpy string in case it is null */
            if (p_s_caption == null)
            {
                p_s_caption = "";
            }

            if ((Instance.o_logConfig.LogExceptionsBoth) && ((Instance.LogControl & (byte)Level.SEVERE) == (byte)Level.SEVERE))
            {
                Instance.LOG?.LogCritical(p_s_caption + p_o_exc.Message, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber);
            }

            if ((Instance.InternalLogControl & (byte)Level.SEVERE) == (byte)Level.SEVERE)
            {
                Instance.ILOG?.LogCritical(p_s_caption + p_o_exc.Message, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber);
            }

            /* call other log implementation */
            Instance.DelegateLogImplementation?.Invoke(false, (byte)Level.SEVERE, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber, p_s_caption + p_o_exc.Message);

            Exception? o_exception = p_o_exc;
            string s_ident = "";

            /* add stack trace elements to log with indent */
            do
            {
                /* check stack trace of exception */
                if ((!ForestNET.Lib.Helper.IsStringEmpty(o_exception.StackTrace)) && (o_exception.StackTrace != null))
                {
                    System.Collections.Generic.List<string> a_lines = [];
                    string[]? a_foo = null;

                    /* split stack trace lines by new line characters */
                    if (o_exception.StackTrace.Contains("\r\n"))
                    {
                        a_foo = o_exception.StackTrace.Split("\r\n");
                    }
                    else if (o_exception.StackTrace.Contains('\n'))
                    {
                        a_foo = o_exception.StackTrace.Split("\n");
                    }
                    else if (o_exception.StackTrace.Contains('\r'))
                    {
                        a_foo = o_exception.StackTrace.Split("\r");
                    }

                    if (a_foo != null)
                    {
                        /* add stack trace lines to list */
                        foreach (string s_foo in a_foo)
                        {
                            a_lines.Add(s_foo);
                        }
                    }
                    else
                    {
                        /* add stack trace line to list */
                        a_lines.Add(o_exception.StackTrace);
                    }

                    /* reverse order of stored stack lines */
                    a_lines.Reverse();

                    /* add stack trace lines to normal and internal log */
                    foreach (string s_foo in a_lines)
                    {
                        if ((Instance.o_logConfig.LogExceptionsBoth) && ((Instance.LogControl & (byte)Level.SEVERE) == (byte)Level.SEVERE))
                        {
                            Instance.LOG?.LogCritical(s_ident + s_foo, "", "", 0);
                        }

                        if ((Instance.InternalLogControl & (byte)Level.SEVERE) == (byte)Level.SEVERE)
                        {
                            Instance.ILOG?.LogCritical(s_ident + s_foo, "", "", 0);
                        }

                        /* call other log implementation */
                        Instance.DelegateLogImplementation?.Invoke(false, (byte)Level.SEVERE, "", "", 0, s_ident + s_foo);
                    }
                }

                /* go into inner exception */
                o_exception = o_exception.InnerException;

                /* increase ident for next inner exception stack trace */
                s_ident += " ";
            } while (o_exception != null); /* stop if we have no more inner exception */
        }


        /// <summary>
        /// Create internal log message with SEVERE log level, if internal log level is included in logControl field
        /// </summary>
        /// <param name="p_s_log">log message which will be logged</param>
        /// <param name="p_s_callerFilePath">log source path or filled by CallerFilePath</param>
        /// <param name="p_s_callerMemberName">log member name or filled by CallerMemberName</param>
        /// <param name="p_i_callerLineNumber">log line number or filled by CallerLineNumber</param>
        public static void ILogSevere(string p_s_log, [System.Runtime.CompilerServices.CallerFilePath] string p_s_callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string p_s_callerMemberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int p_i_callerLineNumber = 0)
        {
            if ((Instance.InternalLogControl & (byte)Level.SEVERE) == (byte)Level.SEVERE)
            {
                Instance.ILOG?.LogError(p_s_log, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber);

                /* call other log implementation */
                Instance.DelegateLogImplementation?.Invoke(true, (byte)Level.SEVERE, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber, p_s_log);
            }
        }

        /// <summary>
        /// Create internal log message with WARNING log level, if internal log level is included in logControl field
        /// </summary>
        /// <param name="p_s_log">log message which will be logged</param>
        /// <param name="p_s_callerFilePath">log source path or filled by CallerFilePath</param>
        /// <param name="p_s_callerMemberName">log member name or filled by CallerMemberName</param>
        /// <param name="p_i_callerLineNumber">log line number or filled by CallerLineNumber</param>
        public static void ILogWarning(string p_s_log, [System.Runtime.CompilerServices.CallerFilePath] string p_s_callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string p_s_callerMemberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int p_i_callerLineNumber = 0)
        {
            if ((Instance.InternalLogControl & (byte)Level.WARNING) == (byte)Level.WARNING)
            {
                Instance.ILOG?.LogWarning(p_s_log, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber);

                /* call other log implementation */
                Instance.DelegateLogImplementation?.Invoke(true, (byte)Level.WARNING, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber, p_s_log);
            }
        }

        /// <summary>
        /// Create internal log message with INFO log level, if internal log level is included in logControl field
        /// </summary>
        /// <param name="p_s_log">log message which will be logged</param>
        /// <param name="p_s_callerFilePath">log source path or filled by CallerFilePath</param>
        /// <param name="p_s_callerMemberName">log member name or filled by CallerMemberName</param>
        /// <param name="p_i_callerLineNumber">log line number or filled by CallerLineNumber</param>
        public static void ILog(string p_s_log, [System.Runtime.CompilerServices.CallerFilePath] string p_s_callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string p_s_callerMemberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int p_i_callerLineNumber = 0)
        {
            if ((Instance.InternalLogControl & (byte)Level.INFO) == (byte)Level.INFO)
            {
                Instance.ILOG?.LogInformation(p_s_log, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber);

                /* call other log implementation */
                Instance.DelegateLogImplementation?.Invoke(true, (byte)Level.INFO, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber, p_s_log);
            }
        }

        /// <summary>
        /// Create internal log message with INFO log level, if internal log level is included in logControl field, limit each log line to a constant amount of characters
        /// </summary>
        /// <param name="p_s_log">log message which will be logged</param>
        /// <param name="p_i_limit">constant limit amount of characters</param>
        /// <param name="p_s_callerFilePath">log source path or filled by CallerFilePath</param>
        /// <param name="p_s_callerMemberName">log member name or filled by CallerMemberName</param>
        /// <param name="p_i_callerLineNumber">log line number or filled by CallerLineNumber</param>
        public static void ILogLarge(string p_s_log, int p_i_limit, [System.Runtime.CompilerServices.CallerFilePath] string p_s_callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string p_s_callerMemberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int p_i_callerLineNumber = 0)
        {
            if ((p_i_limit > 0) && (p_s_log.Length > p_i_limit))
            {
                if ((Instance.InternalLogControl & (byte)Level.INFO) == (byte)Level.INFO)
                {
                    Instance.ILOG?.LogInformation(p_s_log.Substring(0, p_i_limit), p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber);

                    /* call other log implementation */
                    Instance.DelegateLogImplementation?.Invoke(true, (byte)Level.INFO, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber, p_s_log.Substring(0, p_i_limit));

                    /* call recursive to log the complete log message */
                    ILogLarge(p_s_log.Substring(p_i_limit), p_i_limit);
                }
            }
            else
            {
                if ((Instance.InternalLogControl & (byte)Level.INFO) == (byte)Level.INFO)
                {
                    Instance.ILOG?.LogInformation(p_s_log, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber);

                    /* call other log implementation */
                    Instance.DelegateLogImplementation?.Invoke(true, (byte)Level.INFO, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber, p_s_log);
                }
            }
        }

        /// <summary>
        /// Create internal log message with CONFIG log level, if internal log level is included in logControl field
        /// </summary>
        /// <param name="p_s_log">log message which will be logged</param>
        /// <param name="p_s_callerFilePath">log source path or filled by CallerFilePath</param>
        /// <param name="p_s_callerMemberName">log member name or filled by CallerMemberName</param>
        /// <param name="p_i_callerLineNumber">log line number or filled by CallerLineNumber</param>
        public static void ILogConfig(string p_s_log, [System.Runtime.CompilerServices.CallerFilePath] string p_s_callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string p_s_callerMemberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int p_i_callerLineNumber = 0)
        {
            if ((Instance.InternalLogControl & (byte)Level.CONFIG) == (byte)Level.CONFIG)
            {
                Instance.ILOG?.LogDebug(p_s_log, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber);

                /* call other log implementation */
                Instance.DelegateLogImplementation?.Invoke(true, (byte)Level.CONFIG, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber, p_s_log);
            }
        }

        /// <summary>
        /// Create internal log message with FINE log level, if internal log level is included in logControl field
        /// </summary>
        /// <param name="p_s_log">log message which will be logged</param>
        /// <param name="p_s_callerFilePath">log source path or filled by CallerFilePath</param>
        /// <param name="p_s_callerMemberName">log member name or filled by CallerMemberName</param>
        /// <param name="p_i_callerLineNumber">log line number or filled by CallerLineNumber</param>
        public static void ILogFine(string p_s_log, [System.Runtime.CompilerServices.CallerFilePath] string p_s_callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string p_s_callerMemberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int p_i_callerLineNumber = 0)
        {
            if ((Instance.InternalLogControl & (byte)Level.FINE) == (byte)Level.FINE)
            {
                Instance.ILOG?.LogDebug(p_s_log, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber);

                /* call other log implementation */
                Instance.DelegateLogImplementation?.Invoke(true, (byte)Level.FINE, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber, p_s_log);
            }
        }

        /// <summary>
        /// Create internal log message with FINER log level, if internal log level is included in logControl field
        /// </summary>
        /// <param name="p_s_log">log message which will be logged</param>
        /// <param name="p_s_callerFilePath">log source path or filled by CallerFilePath</param>
        /// <param name="p_s_callerMemberName">log member name or filled by CallerMemberName</param>
        /// <param name="p_i_callerLineNumber">log line number or filled by CallerLineNumber</param>
        public static void ILogFiner(string p_s_log, [System.Runtime.CompilerServices.CallerFilePath] string p_s_callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string p_s_callerMemberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int p_i_callerLineNumber = 0)
        {
            if ((Instance.InternalLogControl & (byte)Level.FINER) == (byte)Level.FINER)
            {
                Instance.ILOG?.LogTrace(p_s_log, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber);

                /* call other log implementation */
                Instance.DelegateLogImplementation?.Invoke(true, (byte)Level.FINER, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber, p_s_log);
            }
        }

        /// <summary>
        /// Create internal log message with FINEST log level, if internal log level is included in logControl field
        /// </summary>
        /// <param name="p_s_log">log message which will be logged</param>
        /// <param name="p_s_callerFilePath">log source path or filled by CallerFilePath</param>
        /// <param name="p_s_callerMemberName">log member name or filled by CallerMemberName</param>
        /// <param name="p_i_callerLineNumber">log line number or filled by CallerLineNumber</param>
        public static void ILogFinest(string p_s_log, [System.Runtime.CompilerServices.CallerFilePath] string p_s_callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string p_s_callerMemberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int p_i_callerLineNumber = 0)
        {
            if ((Instance.InternalLogControl & (byte)Level.FINEST) == (byte)Level.FINEST)
            {
                Instance.ILOG?.LogTrace(p_s_log, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber);

                /* call other log implementation */
                Instance.DelegateLogImplementation?.Invoke(true, (byte)Level.FINEST, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber, p_s_log);
            }
        }

        /// <summary>
        /// Create internal log message with MASS log level, if internal log level is included in logControl field
        /// </summary>
        /// <param name="p_s_log">log message which will be logged</param>
        /// <param name="p_s_callerFilePath">log source path or filled by CallerFilePath</param>
        /// <param name="p_s_callerMemberName">log member name or filled by CallerMemberName</param>
        /// <param name="p_i_callerLineNumber">log line number or filled by CallerLineNumber</param>
        public static void ILogMass(string p_s_log, [System.Runtime.CompilerServices.CallerFilePath] string p_s_callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string p_s_callerMemberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int p_i_callerLineNumber = 0)
        {
            if ((Instance.InternalLogControl & (byte)Level.MASS) == (byte)Level.MASS)
            {
                Instance.ILOG?.LogTrace(p_s_log, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber);

                /* call other log implementation */
                Instance.DelegateLogImplementation?.Invoke(true, (byte)Level.MASS, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber, p_s_log);
            }
        }
    }
}
