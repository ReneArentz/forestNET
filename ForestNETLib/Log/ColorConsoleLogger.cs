using Microsoft.Extensions.Logging;

namespace ForestNETLib.Log
{
    /// <summary>
    /// Configuration class for color console logger. Settings for console colors for log levels and source path max. width are available.
    /// </summary>
    public class ColorConsoleLoggerConfiguration
    {

        /* Fields */

        /* Properties */

        public int EventId { get; set; }
        public System.Collections.Generic.Dictionary<LogLevel, ConsoleColor> LogLevelToColorMap { get; set; }
        public int SourceMaxWidth { get; set; }

        /* Methods */

        /// <summary>
        /// Constructor of color console logger configuration
        /// </summary>
        public ColorConsoleLoggerConfiguration()
        {
            /* set standard console colors for log levels */
            this.LogLevelToColorMap = new System.Collections.Generic.Dictionary<LogLevel, ConsoleColor>()
            {
                { LogLevel.Critical, ConsoleColor.DarkRed },
                { LogLevel.Error, ConsoleColor.Red },
                { LogLevel.Warning, ConsoleColor.Yellow },
                { LogLevel.Information, ConsoleColor.Gray },
                { LogLevel.Debug, ConsoleColor.Magenta },
                { LogLevel.Trace, ConsoleColor.Blue }
            };

            /* set standard source path max. width */
            this.SourceMaxWidth = 50;
        }

        /// <summary>
        /// Converts a string parameter to a matching console color
        /// </summary>
        /// <param name="p_s_consoleColor">string log level parameter</param>
        /// <returns>matching console color, if no match succeeds it will return ConsoleColor.Gray</returns>
        /// <exception cref="ArgumentNullException">parameter is not set</exception>
        public static ConsoleColor StringToConsoleColor(string p_s_consoleColor)
        {
            if (ForestNETLib.Core.Helper.IsStringEmpty(p_s_consoleColor))
            {
                throw new ArgumentNullException(nameof(p_s_consoleColor), "Console color parameter is null");
            }

            if (p_s_consoleColor.ToUpper().Equals("Black".ToUpper()))
            {
                return ConsoleColor.Black;
            }
            else if (p_s_consoleColor.ToUpper().Equals("DarkBlue".ToUpper()))
            {
                return ConsoleColor.DarkBlue;
            }
            else if (p_s_consoleColor.ToUpper().Equals("DarkGreen".ToUpper()))
            {
                return ConsoleColor.DarkGreen;
            }
            else if (p_s_consoleColor.ToUpper().Equals("DarkCyan".ToUpper()))
            {
                return ConsoleColor.DarkCyan;
            }
            else if (p_s_consoleColor.ToUpper().Equals("DarkRed".ToUpper()))
            {
                return ConsoleColor.DarkRed;
            }
            else if (p_s_consoleColor.ToUpper().Equals("DarkMagenta".ToUpper()))
            {
                return ConsoleColor.DarkMagenta;
            }
            else if (p_s_consoleColor.ToUpper().Equals("DarkYellow".ToUpper()))
            {
                return ConsoleColor.DarkYellow;
            }
            else if (p_s_consoleColor.ToUpper().Equals("Gray".ToUpper()))
            {
                return ConsoleColor.Gray;
            }
            else if (p_s_consoleColor.ToUpper().Equals("DarkGray".ToUpper()))
            {
                return ConsoleColor.DarkGray;
            }
            else if (p_s_consoleColor.ToUpper().Equals("Blue".ToUpper()))
            {
                return ConsoleColor.Blue;
            }
            else if (p_s_consoleColor.ToUpper().Equals("Green".ToUpper()))
            {
                return ConsoleColor.Green;
            }
            else if (p_s_consoleColor.ToUpper().Equals("Cyan".ToUpper()))
            {
                return ConsoleColor.Cyan;
            }
            else if (p_s_consoleColor.ToUpper().Equals("Red".ToUpper()))
            {
                return ConsoleColor.Red;
            }
            else if (p_s_consoleColor.ToUpper().Equals("Magenta".ToUpper()))
            {
                return ConsoleColor.Magenta;
            }
            else if (p_s_consoleColor.ToUpper().Equals("Yellow".ToUpper()))
            {
                return ConsoleColor.Yellow;
            }
            else if (p_s_consoleColor.ToUpper().Equals("White".ToUpper()))
            {
                return ConsoleColor.White;
            }
            else
            {
                return ConsoleColor.Gray;
            }
        }
    }

    /// <summary>
    /// Color console logger class.
    /// </summary>
    public class ColorConsoleLogger : ILogger, IDisposable
    {

        /* Fields */

        private readonly string s_name;
        private readonly ColorConsoleLoggerConfiguration o_config;

        /* Properties */

        public string Name
        {
            get { return this.s_name; }
        }

        /* Methods */

        /// <summary>
        /// Color console logger constructor
        /// </summary>
        /// <param name="p_s_name">logger name for using filter</param>
        /// <param name="p_o_config">color console configuration instance</param>
        public ColorConsoleLogger(string p_s_name, ColorConsoleLoggerConfiguration p_o_config)
        {
            /* set parameters to class variables */
            this.s_name = p_s_name;
            this.o_config = p_o_config;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        /// <summary>
        /// Checks if the given <paramref name="p_o_logLevel"/> is enabled.
        /// </summary>
        /// <param name="p_o_logLevel">Level to be checked.</param>
        /// <returns><c>true</c> if enabled.</returns>
        public bool IsEnabled(LogLevel p_o_logLevel)
        {
            return this.o_config.LogLevelToColorMap.ContainsKey(p_o_logLevel);
        }

        /// <summary>
        /// Disposing color console logger and stopping it's thread in that way
        /// </summary>
        public void Dispose()
        {
            /* anything to close here */

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Writes a log entry to color console
        /// </summary>
        /// <param name="p_o_logLevel">Entry will be written on this level.</param>
        /// <param name="p_o_eventId">Id of the event.</param>
        /// <param name="p_o_state">The entry to be written. Can be also an object.</param>
        /// <param name="p_o_exception">The exception related to this entry.</param>
        /// <param name="p_o_formatter">(NOT USED) Function to create a <see cref="string"/> message of the <paramref name="p_o_state"/> and <paramref name="p_o_exception"/>.</param>
        /// <typeparam name="TState">The type of the object to be written.</typeparam>
        public void Log<TState>(LogLevel p_o_logLevel, EventId p_o_eventId, TState p_o_state, Exception? p_o_exception, Func<TState, Exception, string> p_o_formatter)
        {
            /* check if log level is enabled */
            if (!IsEnabled(p_o_logLevel))
            {
                return;
            }

            /* check if state object is set */
            if (p_o_state == null)
            {
                return;
            }

            /* extract message from state parameter */
            string s_foo = p_o_state.ToString() ?? "";

            /* if event id is zero or matches configured with parameter event id */
            if (this.o_config.EventId == 0 || this.o_config.EventId == p_o_eventId.Id)
            {
                /* cast log level to number */
                int i_logLevel = (int)p_o_logLevel;

                /* write log level number and message to console */
                string s_log = i_logLevel + s_foo;

                /* log message parameter must start with an integer */
                if (ForestNETLib.Core.Helper.IsInteger(s_log.Substring(0, 1)))
                {
                    /* cast integer from log message to log level */
                    LogLevel o_logLevel = (LogLevel)int.Parse(s_log.Substring(0, 1));
                    /* handle rest as log message */
                    string s_message = s_log.Substring(1);

                    /* store console original color */
                    ConsoleColor e_originalColor = Console.ForegroundColor;

                    /* use configured console color based on log level map */
                    Console.ForegroundColor = this.o_config.LogLevelToColorMap[o_logLevel];
                    /* add datetime stamp to console */
                    Console.Write($"[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture) + "] ");
                    /* add log level to console */
                    Console.Write($"[{o_logLevel,-11}] ");

                    /* log message must not be empty */
                    if (!ForestNETLib.Core.Helper.IsStringEmpty(s_message))
                    {
                        /* log messag must start with 'fNET§§§' */
                        if (s_message.StartsWith("fNET§§§"))
                        {
                            /* split log message by '§§§' */
                            string[] a_messageParts = s_message.Split("§§§");

                            /* if log source path is not empty */
                            if (!ForestNETLib.Core.Helper.IsStringEmpty(a_messageParts[3]))
                            {
                                string s_sourcePath = "";

                                if (a_messageParts[3].Contains('\\')) /* if we have a windows path */
                                {
                                    /* store last part of source path */
                                    s_sourcePath = a_messageParts[3].Substring(a_messageParts[3].LastIndexOf("\\") + 1);
                                }
                                else if (a_messageParts[3].Contains('/')) /* if we have a unix path */
                                {
                                    /* store last part of source path */
                                    s_sourcePath = a_messageParts[3].Substring(a_messageParts[3].LastIndexOf("/") + 1);
                                }

                                /* if log member name is not empty */
                                if (!ForestNETLib.Core.Helper.IsStringEmpty(a_messageParts[4]))
                                {
                                    /* store member name of source path */
                                    s_sourcePath += " -> " + a_messageParts[4];

                                    /* if line number is not empty, an integer and not zero */
                                    if ((!ForestNETLib.Core.Helper.IsStringEmpty(a_messageParts[5])) && (ForestNETLib.Core.Helper.IsInteger(a_messageParts[5])) && (!a_messageParts[5].Equals("0")))
                                    {
                                        /* store line number of source path */
                                        s_sourcePath += ":" + a_messageParts[5];
                                    }
                                }

                                if (s_sourcePath.Length > this.o_config.SourceMaxWidth) /* check if source path exceeds max. width */
                                {
                                    /* cut source path and show only tail of it */
                                    s_sourcePath = s_sourcePath.Substring(s_sourcePath.Length - this.o_config.SourceMaxWidth);
                                }
                                else /* source path if within max. width */
                                {
                                    /* fill up rest of max. width characters with white spaces */
                                    for (int i = s_sourcePath.Length; i < this.o_config.SourceMaxWidth; i++)
                                    {
                                        s_sourcePath += " ";
                                    }
                                }

                                /* add source path to console */
                                Console.Write("[" + s_sourcePath + "] ");
                            }
                            else
                            {
                                string s_sourcePath = "";

                                /* write empty source path */
                                for (int i = s_sourcePath.Length; i < this.o_config.SourceMaxWidth; i++)
                                {
                                    s_sourcePath += " ";
                                }

                                /* add source path to console */
                                Console.Write("[" + s_sourcePath + "] ");
                            }

                            /* add message to console, removing any newline characters */
                            Console.Write(a_messageParts[1].Replace(Environment.NewLine, " "));
                            /* add exception to console, removing any newline characters */
                            Console.Write(a_messageParts[2].Replace(Environment.NewLine, " "));
                        }
                        else /* just a usual log message */
                        {
                            /* add log message to console, removing any newline characters */
                            Console.Write(s_message.Replace(Environment.NewLine, " "));
                        }
                    }

                    /* add newline characters */
                    Console.Write(Environment.NewLine);

                    /* restore console original color */
                    Console.ForegroundColor = e_originalColor;
                }
            }
        }
    }

    /// <summary>
    /// Color console logger provider, holding all loggers generated by logger factory.
    /// </summary>
    [ProviderAlias("ColorConsole")]
    public class ColorConsoleLoggerProvider : ILoggerProvider
    {

        /* Fields */

        private readonly ColorConsoleLoggerConfiguration o_config;
        private readonly System.Collections.Concurrent.ConcurrentDictionary<string, ColorConsoleLogger> a_loggers = new(StringComparer.OrdinalIgnoreCase);

        /* Properties */

        /* Methods */

        /// <summary>
        /// Color console logger provider constructor
		/// </summary>
        /// <param name="p_o_config">color console configuration instance, used when creating new color console logger(s)</param>
        public ColorConsoleLoggerProvider(ColorConsoleLoggerConfiguration p_o_config)
        {
            o_config = p_o_config;
        }

        /// <summary>
        /// Creating new color console logger
        /// </summary>
        /// <param name="p_s_categoryName">logger name</param>
        /// <returns>object based on ILogger interface</returns>
        public ILogger CreateLogger(string p_s_categoryName)
        {
            return a_loggers.GetOrAdd(p_s_categoryName, s_name => new ColorConsoleLogger(s_name, this.o_config));
        }

        /// <summary>
        /// Disposing color console logger provider will dispose all logger as well
        /// </summary>
        public void Dispose()
        {
            /* iterate all loggers and dispose them, before clearing the dictionary */
            foreach (var o_logger in a_loggers)
            {
                o_logger.Value.Dispose();
            }

            a_loggers.Clear();

            GC.SuppressFinalize(this);
        }
    }
}
