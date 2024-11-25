using Microsoft.Extensions.Logging;
using static ForestNETLib.Log.LevelExtensions;

namespace ForestNETLib.Log
{
    /// <summary>
    /// Class to create logging factory for Microsoft.Extensions.Logging without having an external file, but loading from config file is supported, too.
    /// Supports log to file and log to console.
    /// </summary>
    public class LogConfig : IDisposable
    {

        /* Fields */

        private bool b_logCompleteSqlQuery;

        /* Properties */

        public ILoggerFactory? LoggerFactory { get; private set; }
        public int SourceMaxWidthConsole { get; set; }
        public int SourceMaxWidthFile { get; set; }
        public ColorConsoleLoggerConfiguration ColorConsoleLoggerConfiguration { get; private set; }
        public FileLoggerConfiguration FileLoggerConfiguration { get; private set; }
        public bool UseConsoleLogging { get; set; }
        public bool UseFileLogging { get; set; }
        public Level MinimumLevel { get; set; }
        /// <summary>
        /// Add level filters for console logging, bool key stands for [true - internal; false - normal] logging mode
        /// </summary>
        public System.Collections.Generic.Dictionary<bool, Level> ConsoleLoggingFilters { get; private set; }
        /// <summary>
        /// Add level filters for file logging, bool key stands for [true - internal; false - normal] logging mode
        /// </summary>
        public System.Collections.Generic.Dictionary<bool, Level> FileLoggingFilters { get; private set; }
        /// <summary>
        /// true - log exceptions on normal and internal logger, false - log exceptions only on internal logger
        /// </summary>
        public bool LogExceptionsBoth { get; set; }
        public bool LogCompleteSqlQuery
        {
            get
            {
                return b_logCompleteSqlQuery;
            }
            set
            {
                if (value)
                {
                    ForestNETLib.Core.Global.LogWarning("ForestNETLib.SQL.Base log query flag has been set true");
                    ForestNETLib.Core.Global.LogWarning("if log access is not secured, using this is a security vulnerability!");
                }

                this.b_logCompleteSqlQuery = value;
            }
        }

        /* Methods */

        /// <summary>
        /// Log config constructor, setting standard settings for logging
        /// </summary>
        public LogConfig()
        {
            this.SourceMaxWidthConsole = 50;
            this.SourceMaxWidthFile = 50;
            this.ColorConsoleLoggerConfiguration = new ColorConsoleLoggerConfiguration();
            this.FileLoggerConfiguration = new FileLoggerConfiguration();
            this.ColorConsoleLoggerConfiguration.SourceMaxWidth = this.SourceMaxWidthConsole;
            this.FileLoggerConfiguration.SourceMaxWidth = this.SourceMaxWidthFile;
            this.FileLoggerConfiguration.AddConfigurationElement(
                new FileLoggerConfigurationElement
                {
                    Category = "Log;InternalLog",
                    FilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + ForestNETLib.IO.File.DIR ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'"),
                    FileName = "%dt_log.%n1.txt",
                    FileCount = 10,
                    FileLimitInBytes = 1000000
                }
            );
            this.UseConsoleLogging = true;
            this.UseFileLogging = false;
            this.MinimumLevel = Level.ALMOST_ALL;
            this.ConsoleLoggingFilters = [];
            this.FileLoggingFilters = [];
            this.LogExceptionsBoth = false;
            this.LogCompleteSqlQuery = false;
        }

        /// <summary>
        /// Disposing log config instance
        /// </summary>
        public void Dispose()
        {
            /* dispose current logger factory and current logging */
            this.LoggerFactory?.Dispose();
            
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Reset logging factory
        /// </summary>
        public void ResetLog()
        {
            /* dispose current logger factory and current logging */
            this.Dispose();

            /* update source max. width in both configuration instances */
            this.ColorConsoleLoggerConfiguration.SourceMaxWidth = this.SourceMaxWidthConsole;
            this.FileLoggerConfiguration.SourceMaxWidth = this.SourceMaxWidthFile;

            /* create new logging factory */
            this.LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
            {
                /* remove all providers */
                builder.ClearProviders();
                /* set minimum level */
                builder.SetMinimumLevel(LevelToNETLogLevel(this.MinimumLevel));

                /* set filter levels for console */
                if ((this.UseConsoleLogging) && (this.ConsoleLoggingFilters.Count > 0))
                {
                    foreach (System.Collections.Generic.KeyValuePair<bool, Level> o_consoleLoggingFilter in this.ConsoleLoggingFilters)
                    {
                        builder.AddFilter<ColorConsoleLoggerProvider>(o_consoleLoggingFilter.Key ? "InternalLog" : "Log", LevelToNETLogLevel(o_consoleLoggingFilter.Value));
                    }
                }

                /* set filter levels for file */
                if ((this.UseFileLogging) && (this.FileLoggingFilters.Count > 0))
                {
                    foreach (System.Collections.Generic.KeyValuePair<bool, Level> o_fileLoggingFilter in this.FileLoggingFilters)
                    {
                        builder.AddFilter<FileLoggerProvider>(o_fileLoggingFilter.Key ? "InternalLog" : "Log", LevelToNETLogLevel(o_fileLoggingFilter.Value));
                    }
                }
            });

            /* add console provider if we want to use it */
            if (this.UseConsoleLogging)
            {
                this.LoggerFactory.AddColorConsole(this.ColorConsoleLoggerConfiguration);
            }

            /* add file provider if we want to use it */
            if (this.UseFileLogging)
            {
                this.LoggerFactory.AddFile(this.FileLoggerConfiguration);
            }
        }

        /// <summary>
        /// Reset logging factory to standard values
        /// </summary>
        public void ResetLogToStandard()
        {
            this.SourceMaxWidthConsole = 50;
            this.SourceMaxWidthFile = 50;
            this.ColorConsoleLoggerConfiguration = new ColorConsoleLoggerConfiguration();
            this.FileLoggerConfiguration = new FileLoggerConfiguration();
            this.ColorConsoleLoggerConfiguration.SourceMaxWidth = this.SourceMaxWidthConsole;
            this.FileLoggerConfiguration.SourceMaxWidth = this.SourceMaxWidthFile;
            this.FileLoggerConfiguration.AddConfigurationElement(
                new FileLoggerConfigurationElement
                {
                    Category = "Log;InternalLog",
                    FilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + ForestNETLib.IO.File.DIR ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'"),
                    FileName = "%dt_log.%n1.txt",
                    FileCount = 10,
                    FileLimitInBytes = 1000000
                }
            );
            this.UseConsoleLogging = true;
            this.UseFileLogging = false;
            this.MinimumLevel = Level.ALMOST_ALL;
            this.ConsoleLoggingFilters = [];
            this.FileLoggingFilters = [];
            this.LogExceptionsBoth = false;
            this.LogCompleteSqlQuery = false;

            /* reset logging factory */
            this.ResetLog();
        }

        /// <summary>
        /// Set logging settings from a config file and reset logging factory
        /// </summary>
        /// <param name="p_s_configFile">path to configuration file for logging settings</param>
        /// <exception cref="ArgumentException">config file does not exist</exception>
        public void ResetLogFromFile(string p_s_configFile)
        {
            /* check if config file really exists */
            if (!ForestNETLib.IO.File.Exists(p_s_configFile))
            {
                throw new ArgumentException("Config file '" + p_s_configFile + "' does not exist.");
            }
            
            /* open config file and read all lines */
            ForestNETLib.IO.File o_file = new(p_s_configFile, false);

            /* set logging settings from file lines */
            this.ResetLogFromLines(o_file.FileContentAsList ?? throw new ArgumentException("File '" + p_s_configFile + "' has no content to load log configuration"));
        }

        /// <summary>
        /// Set logging settings from a list of string lines and reset logging factory
        /// </summary>
        /// <param name="p_a_configLines">list of string lines for logging settings</param>
        public void ResetLogFromLines(System.Collections.Generic.List<string> p_a_configLines)
        {
            /* flags to clear lists if we have new configuration lines in file for it */
            bool b_cleanedConsoleLoggingFilters = false;
            bool b_cleanedFileLoggingFilters = false;
            bool b_cleanedLevelConsoleColorMap = false;
            bool b_cleanedFileLoggerConfigurationElements = false;

            /* process each config file line */
            foreach (string s_foo in p_a_configLines)
            {
                /* trim line */
                string s_line = s_foo.Trim();

                /* check if we have an '=' character within line, otherwise we skip this line */
                if (!s_line.Contains('='))
                {
                    continue;
                }

                /* check if line starts with '#' character or with '//', then we will skip this line */
                if ((s_line.StartsWith("#")) || (s_line.StartsWith("//")))
                {
                    continue;
                }

                /* split config line with '=' character */
                string[] a_lineParts = s_line.Split("=");

                /* if we have not two line parts after splitting with '=' character, we will skip this line */
                if (a_lineParts.Length != 2)
                {
                    continue;
                }

                /* get key and value of line and trim both parts */
                string s_key = a_lineParts[0].Trim();
                string s_value = a_lineParts[1].Trim();

                /* only read lines with expected keys: */

                if (s_key.ToLower().Equals("UseConsoleLogging".ToLower()))
                {
                    /* value must be true/false */
                    if (ForestNETLib.Core.Helper.IsBoolean(s_value))
                    {
                        this.UseConsoleLogging = Boolean.Parse(s_value);
                    }
                }
                else if (s_key.ToLower().Equals("UseFileLogging".ToLower()))
                {
                    /* value must be true/false */
                    if (ForestNETLib.Core.Helper.IsBoolean(s_value))
                    {
                        this.UseFileLogging = Boolean.Parse(s_value);
                    }
                }
                else if (s_key.ToLower().Equals("LogExceptionsBoth".ToLower()))
                {
                    /* value must be true/false */
                    if (ForestNETLib.Core.Helper.IsBoolean(s_value))
                    {
                        this.LogExceptionsBoth = Boolean.Parse(s_value);
                    }
                }
                else if (s_key.ToLower().Equals("SourceMaxWidthConsole".ToLower()))
                {
                    /* source max. width for console must be an integer */
                    if (ForestNETLib.Core.Helper.IsInteger(s_value))
                    {
                        this.SourceMaxWidthConsole = int.Parse(s_value);
                    }
                }
                else if (s_key.ToLower().Equals("SourceMaxWidthFile".ToLower()))
                {
                    /* source max. width for file must be an integer */
                    if (ForestNETLib.Core.Helper.IsInteger(s_value))
                    {
                        this.SourceMaxWidthFile = int.Parse(s_value);
                    }
                }
                else if (s_key.ToLower().Equals("MinimumLevel".ToLower()))
                {
                    this.MinimumLevel = StringToLevel(s_value);
                }
                else if (s_key.ToLower().Equals("LogControl".ToLower()))
                {
                    /* multiple level values for log control must be divided by '+' character */
                    if (s_value.Contains('+'))
                    {
                        byte by_foo = 0x00;

                        /* iterate each level value, splitted by '+' character */
                        foreach (string s_logControlLevel in s_value.Split("+"))
                        {
                            by_foo += (byte)StringToLevel(s_logControlLevel.Trim());
                        }

                        ForestNETLib.Core.Global.Instance.LogControl = by_foo;
                    }
                    else
                    {
                        ForestNETLib.Core.Global.Instance.LogControl = (byte)StringToLevel(s_value);
                    }
                }
                else if (s_key.ToLower().Equals("InternalLogControl".ToLower()))
                {
                    /* multiple level values for internal log control must be divided by '+' character */
                    if (s_value.Contains('+'))
                    {
                        byte by_foo = 0x00;

                        /* iterate each level value, splitted by '+' character */
                        foreach (string s_internalLogControlLevel in s_value.Split("+"))
                        {
                            by_foo += (byte)StringToLevel(s_internalLogControlLevel.Trim());
                        }

                        ForestNETLib.Core.Global.Instance.InternalLogControl = by_foo;
                    }
                    else
                    {
                        ForestNETLib.Core.Global.Instance.InternalLogControl = (byte)StringToLevel(s_value);
                    }
                }
                else if (s_key.ToLower().Equals("ConsoleLoggingFilter".ToLower()))
                {
                    /* config line value must contain a ';' character */
                    if (s_value.Contains(';'))
                    {
                        /* split line for ConsoleLoggingFilter by ';' character */
                        string[] a_valueParts = s_value.Split(";");

                        /* we must have two value parts */
                        if (a_valueParts.Length == 2)
                        {
                            /* read logging category and filter values */
                            string s_loggingCategory = a_valueParts[0].Trim();
                            string s_loggingFilter = a_valueParts[1].Trim();

                            /* logging category must be true/false */
                            if (ForestNETLib.Core.Helper.IsBoolean(s_loggingCategory))
                            {
                                /* check if console logging filters already has been cleared */
                                if (!b_cleanedConsoleLoggingFilters)
                                {
                                    /* clear dictionary */
                                    this.ConsoleLoggingFilters.Clear();
                                    /* set flag */
                                    b_cleanedConsoleLoggingFilters = true;
                                }

                                /* add logging category and level as console logging filter */
                                this.ConsoleLoggingFilters.Add(Boolean.Parse(s_loggingCategory), StringToLevel(s_loggingFilter));
                            }
                        }
                    }
                }
                else if (s_key.ToLower().Equals("FileLoggingFilter".ToLower()))
                {
                    /* config line value must contain a ';' character */
                    if (s_value.Contains(';'))
                    {
                        /* split line for FileLoggingFilter by ';' character */
                        string[] a_valueParts = s_value.Split(";");

                        /* we must have two value parts */
                        if (a_valueParts.Length == 2)
                        {
                            /* read logging category and filter values */
                            string s_loggingCategory = a_valueParts[0].Trim();
                            string s_loggingFilter = a_valueParts[1].Trim();

                            /* logging category must be true/false */
                            if (ForestNETLib.Core.Helper.IsBoolean(s_loggingCategory))
                            {
                                /* check if file logging filters already has been cleared */
                                if (!b_cleanedFileLoggingFilters)
                                {
                                    /* clear dictionary */
                                    this.FileLoggingFilters.Clear();
                                    /* set flag */
                                    b_cleanedFileLoggingFilters = true;
                                }

                                /* add logging category and level as file logging filter */
                                this.FileLoggingFilters.Add(Boolean.Parse(s_loggingCategory), StringToLevel(s_loggingFilter));
                            }
                        }
                    }
                }
                else if (s_key.ToLower().Equals("LogLevelToColorMap".ToLower()))
                {
                    /* config line value must contain a ';' character */
                    if (s_value.Contains(';'))
                    {
                        /* split line for LogLevelToColorMap by ';' character */
                        string[] a_valueParts = s_value.Split(";");

                        /* we must have two value parts */
                        if (a_valueParts.Length == 2)
                        {
                            /* read NET log level and console color values */
                            Microsoft.Extensions.Logging.LogLevel e_logLevel = StringToNETLogLevel(a_valueParts[0].Trim());
                            ConsoleColor e_consoleColor = ForestNETLib.Log.ColorConsoleLoggerConfiguration.StringToConsoleColor(a_valueParts[1].Trim());

                            /* check if log level color map already has been cleared */
                            if (!b_cleanedLevelConsoleColorMap)
                            {
                                /* clear dictionary */
                                this.ColorConsoleLoggerConfiguration.LogLevelToColorMap.Clear();
                                /* set flag */
                                b_cleanedLevelConsoleColorMap = true;
                            }

                            /* add NET log level and console color as map entry */
                            this.ColorConsoleLoggerConfiguration.LogLevelToColorMap.Add(e_logLevel, e_consoleColor);
                        }
                    }
                }
                else if (s_key.ToLower().Equals("FileLoggerConfigurationElement".ToLower()))
                {
                    /* config line value must contain a ';' character */
                    if (s_value.Contains(';'))
                    {
                        /* split line for FileLoggerConfigurationElement by ';' character */
                        string[] a_valueParts = s_value.Split(";");

                        /* value parts must be 3 or 5 */
                        if ((a_valueParts.Length != 3) && (a_valueParts.Length != 5))
                        {
                            continue;
                        }

                        /* read category from value */
                        string s_category = a_valueParts[0].Trim();

                        if (s_category.Contains(',')) /* category has multiple values */
                        {
                            /* split category values */
                            string[] a_categories = s_category.Split(",");

                            /* only accept 2 category values */
                            if (a_categories.Length == 2)
                            {
                                /* both values must be 'Log' or 'InternalLog' */
                                if (
                                    ((a_categories[0].ToLower().Equals("Log".ToLower())) && (a_categories[1].ToLower().Equals("InternalLog".ToLower())))
                                    ||
                                    ((a_categories[0].ToLower().Equals("InternalLog".ToLower())) && (a_categories[1].ToLower().Equals("Log".ToLower())))
                                    )
                                {
                                    s_category = "Log;InternalLog";
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else /* just one category value */
                        {
                            /* category value must be 'Log' or 'InternalLog' */
                            if ((!s_category.ToLower().Equals("Log".ToLower())) && (!s_category.ToLower().Equals("InternalLog".ToLower())))
                            {
                                continue;
                            }
                        }

                        /* read file path from value */
                        string s_filePath = a_valueParts[1].ToLower().Trim();

                        /* check if file path starts with '%localstate' */
                        if (s_filePath.StartsWith("%localstate"))
                        {
                            /* replace '%localstate' with local folder path, where assembly is located */
                            s_filePath = (System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + ForestNETLib.IO.File.DIR ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'"))
                                + s_filePath.Substring(11);
                        }

                        /* check if file path starts with '%tempstate' */
                        if (s_filePath.StartsWith("%tempstate"))
                        {
                            /* replace '%tempstate' with local app data temporary folder path */
                            s_filePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + ForestNETLib.IO.File.DIR + "temp" + ForestNETLib.IO.File.DIR
                                + s_filePath.Substring(10);
                        }
                        
                        /* add directory separator if it is not at the end of file path */
                        if (!s_filePath.EndsWith(ForestNETLib.IO.File.DIR))
                        {
                            s_filePath += ForestNETLib.IO.File.DIR;
                        }

                        /* check if file path really exists */
                        if (!ForestNETLib.IO.File.FolderExists(s_filePath))
                        {
                            continue;
                        }
                        
                        /* read file name from value */
                        string s_fileName = a_valueParts[2].Trim();

                        /* standard values for file count and file limit in bytes */
                        int i_fileCount = 1;
                        int i_fileLimitInBytes = 1000000;

                        /* if we have more than 3 values, we expect 5 values */
                        if (a_valueParts.Length == 5)
                        {
                            /* read file count from value */
                            string s_fileCount = a_valueParts[3].Trim();

                            /* file count must be an integer */
                            if (ForestNETLib.Core.Helper.IsInteger(s_fileCount))
                            {
                                i_fileCount = int.Parse(s_fileCount);
                            }

                            /* file count must be between 1..10000 */
                            if ((i_fileCount < 1) || (i_fileCount > 10000))
                            {
                                continue;
                            }

                            /* read file limit in bytes from value */
                            string s_fileLimitInBytes = a_valueParts[4].Trim();

                            /* file limit in bytes must be an integer */
                            if (ForestNETLib.Core.Helper.IsInteger(s_fileLimitInBytes))
                            {
                                i_fileLimitInBytes = int.Parse(s_fileLimitInBytes);
                            }

                            /* file limit in bytes must be between 1000..1000000000 */
                            if ((i_fileLimitInBytes < 1000) || (i_fileLimitInBytes > 1000000000))
                            {
                                continue;
                            }
                        }

                        /* check if file logger configuration elements list already has been cleared */
                        if (!b_cleanedFileLoggerConfigurationElements)
                        {
                            /* clear file logger configuration elements list */
                            this.FileLoggerConfiguration.ClearAllConfigurationElements();
                            /* set flag */
                            b_cleanedFileLoggerConfigurationElements = true;
                        }

                        /* add file logger configuration element */
                        this.FileLoggerConfiguration.AddConfigurationElement(
                            new FileLoggerConfigurationElement
                            {
                                Category = s_category,
                                FilePath = s_filePath,
                                FileName = s_fileName,
                                FileCount = i_fileCount,
                                FileLimitInBytes = i_fileLimitInBytes
                            }
                        );
                    }
                }
            }

            /* initialize logging */
            this.ResetLog();
        }

        /// <summary>
        /// Creates a logger instance of logger factory of this current log config class
        /// </summary>
        /// <param name="p_s_categoryName">category name parameter</param>
        /// <returns>ILogger instance</returns>
        /// <exception cref="ArgumentNullException">category name parameter is null or empty</exception>
        /// <exception cref="NullReferenceException">logger factory is not initialized</exception>
        public ILogger CreateLoggerInstance(string p_s_categoryName)
        {
            if (ForestNETLib.Core.Helper.IsStringEmpty(p_s_categoryName))
            {
                throw new ArgumentNullException(nameof(p_s_categoryName), "Category name parameter is null or empty");
            }

            if (this.LoggerFactory == null)
            {
                throw new NullReferenceException("Logger factory is not initialized");
            }

            return this.LoggerFactory.CreateLogger(p_s_categoryName);
        }
    }
}
