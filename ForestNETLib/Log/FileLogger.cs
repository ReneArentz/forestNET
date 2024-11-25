using Microsoft.Extensions.Logging;

namespace ForestNETLib.Log
{
    /// <summary>
    /// File holder class holding all necessary information for one log file; file configuration element, it's log message queue, source max. width and thread control
    /// </summary>
    public class FileHolder : IDisposable
    {

        /* Fields */

        private readonly System.Collections.Concurrent.ConcurrentQueue<string> o_queue;

        /* Properties */

        public System.Collections.Concurrent.ConcurrentQueue<string> Queue { get { return o_queue; } }
        public FileLoggerConfigurationElement? FileConfigurationElement { get; set; }
        public int SourceMaxWidth { get; set; }
        public System.Threading.Thread? Thread { get; set; }
        public bool StopThread { get; set; }
        public bool ThreadClosed { get; set; }

        /* Methods */

        /// <summary>
        /// Constructor of file holder
        /// </summary>
        public FileHolder()
        {
            this.o_queue = new System.Collections.Concurrent.ConcurrentQueue<string>();
        }

        /// <summary>
        /// Disposing file holder and stopping it's thread in that way
        /// </summary>
        public void Dispose()
        {
            /* set flag to stop thread */
            this.StopThread = true;

            int j = 0;

            /* wait for thread closed flag */
            while (!this.ThreadClosed)
            {
                /* wait 25 milliseconds */
                System.Threading.Thread.Sleep(25);

                j++;

                /* try for amount of time parameter (ms) to wait for closing thread */
                if (j > (5000 / 25))
                {
                    Console.WriteLine("EXCEPTION: file logging thread has not been closed within 5 seconds. Thread will be aborted.");

                    /* time has elapsed */
                    break;
                }
            }

            /* if thread is not closed, we must abort it */
            if (!this.ThreadClosed)
            {
                /* interrupt thread */
                this.Thread?.Interrupt();
            }
            
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Thread method for writing log messages to file
        /// </summary>
        public void WriteLogWithThread()
        {
            /* thread execution */
            while (true)
            {
                if (this.o_queue.TryDequeue(out string? s_log)) /* try to dequeue log message */
                {
                    if (!ForestNETLib.Core.Helper.IsStringEmpty(s_log))
                    {
                        /* log message parameter must start with an integer */
                        if (ForestNETLib.Core.Helper.IsInteger(s_log.Substring(0, 1)))
                        {
                            /* cast integer from log message to log level */
                            LogLevel o_logLevel = (LogLevel)int.Parse(s_log.Substring(0, 1));
                            /* handle rest as log message */
                            string s_message = s_log.Substring(1);
                            /* line which we want to write to log file */
                            string s_line = "";

                            /* add datetime stamp to line */
                            s_line += $"[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture) + "] ";
                            /* add log level to line */
                            s_line += $"[{o_logLevel,-11}] ";

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

                                        if (s_sourcePath.Length > this.SourceMaxWidth) /* check if source path exceeds max. width */
                                        {
                                            /* cut source path and show only tail of it */
                                            s_sourcePath = s_sourcePath.Substring(s_sourcePath.Length - this.SourceMaxWidth);
                                        }
                                        else /* source path if within max. width */
                                        {
                                            /* fill up rest of max. width characters with white spaces */
                                            for (int i = s_sourcePath.Length; i < this.SourceMaxWidth; i++)
                                            {
                                                s_sourcePath += " ";
                                            }
                                        }

                                        /* add source path to line */
                                        s_line += "[" + s_sourcePath + "] ";
                                    }
                                    else
                                    {
                                        string s_sourcePath = "";

                                        /* write empty source path */
                                        for (int i = s_sourcePath.Length; i < this.SourceMaxWidth; i++)
                                        {
                                            s_sourcePath += " ";
                                        }

                                        /* add source path to line */
                                        s_line += "[" + s_sourcePath + "] ";
                                    }

                                    /* add message to line, removing any newline characters */
                                    s_line += a_messageParts[1].Replace(Environment.NewLine, " ");
                                    /* add exception to line, removing any newline characters */
                                    s_line += a_messageParts[2].Replace(Environment.NewLine, " ");
                                }
                                else /* just a usual log message */
                                {
                                    /* add log message to line, removing any newline characters */
                                    s_line += s_message.Replace(Environment.NewLine, " ");
                                }
                            }

                            /* add newline characters */
                            s_line += Environment.NewLine;

                            /* determine complete file path, with settings of file configuration element and parsing method for file name */
                            string s_filePath = this.FileConfigurationElement?.FilePath + 
                                ((!this.FileConfigurationElement?.FilePath.EndsWith(ForestNETLib.IO.File.DIR.ToString())) ?? false ? ForestNETLib.IO.File.DIR.ToString() : "") + 
                                this.FileConfigurationElement?.ParseFileName(s_line.Length);
                            
                            /* add line to log file */
                            System.IO.File.AppendAllText(s_filePath, s_line);
                        }
                    }
                }
                else /* wait for 25 milliseconds */
                {
                    System.Threading.Thread.Sleep(25);
                }

                /* check if stop thread flag is set and we have no more log message to write */
                /* if it needs more than 5000 ms to write remaining log messages, this thread will be aborted by FileHolder.Dispose method itself */
                if ((this.StopThread) && (this.o_queue.IsEmpty))
                {
                    /* break thread while loop execution */
                    break;
                }
            }

            /* set thread closed flag */
            this.ThreadClosed = true;
        }
    }

    /// <summary>
    /// Class for file logger configuration element. Settings for file path, file name, file limit in bytes and file count.
    /// </summary>
    public class FileLoggerConfigurationElement
    {
        /* Fields */

        private int i_fileLimit;
        private int i_fileCount;

        /* Properties */

        public string Category { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public int FileLimitInBytes
        {
            get
            {
                return this.i_fileLimit;
            }
            set
            {
                if (value < 1000)
                {
                    throw new ArgumentException("FileLimitInBytes must be at least greater equal 1000 Bytes (1 KB).");
                }

                if (value > 1000000000)
                {
                    throw new ArgumentException("FileLimitInBytes must be lower than 1_000_000_000 Bytes (1 GB).");
                }

                this.i_fileLimit = value;
            }
        }
        public int FileCount
        {
            get
            {
                return this.i_fileCount;
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("FileCount must be at least '1'.");
                }

                if (value > 10000)
                {
                    throw new ArgumentException("FileCount must be lower than '10_000'.");
                }

                this.i_fileCount = value;
            }
        }

        /* Methods */

        /// <summary>
        /// Constructor of file logger configuration element
        /// </summary>
        public FileLoggerConfigurationElement()
        {
            this.Category = "";
            this.FilePath = "";
            this.FileName = "";

            /* set standard file limit in bytes -> 1 MB */
            this.FileLimitInBytes = 1000000;

            /* set standard file count */
            this.FileCount = 1;
        }

        /// <summary>
        /// Replace all occurrences of '%dt' within file name parameter with current datestamp (yyyy-MM-dd)<br />
        /// Replace all occurrences of '%hh' within file name parameter with current hour (HH)<br />
        /// Replace all occurrences of '%mm' within file name parameter with current minute (mm)<br />
        /// Replace all occurrences of '%ss' within file name parameter with current second (ss)<br />
        /// Replace placeholder '%n[0-4]' for file count<br />
        /// </summary>
        /// <returns>new file name value</returns>
        public string ParseFileName(int p_i_lineLength)
        {
            /* replace all occurrences of date time placeholders within file name */
            string s_foo = this.FileName
                .Replace("%dt", DateTime.Now.ToString("yyyy-MM-dd"))
                .Replace("%hh", DateTime.Now.ToString("HH"))
                .Replace("%mm", DateTime.Now.ToString("mm"))
                .Replace("%ss", DateTime.Now.ToString("ss"));

            /* handle complex file name if we have multiple file count > 1 */
            if (this.FileCount > 1)
            {
                /* get position of file count placeholder */
                int i_placeHolderPosition = s_foo.IndexOf("%n");
                /* get number of leading zeros of file count placeholder */
                int i_leadingZeros = int.Parse(s_foo.Substring(i_placeHolderPosition + 2, 1));
                /* create a filter value for listing all files we want to handle */
                string s_filter = s_foo.Substring(0, i_placeHolderPosition);

                /* get to the path where we want to handle our potential log file(s), order by last modified time, name and filtered by name */
                string s_filePath = this.FilePath + ((!this.FilePath.EndsWith(ForestNETLib.IO.File.DIR.ToString())) ? ForestNETLib.IO.File.DIR.ToString() : "");
                System.Collections.Generic.List<ForestNETLib.IO.ListingElement> a_potentialFiles = ForestNETLib.IO.File.ListDirectory(s_filePath)
                    .OrderByDescending(o_listingElement => o_listingElement.LastModifiedTime)
                    .ThenByDescending(o_listingElement => o_listingElement.Name)
                    .Where(o_listingElement => !o_listingElement.IsDirectory && (o_listingElement.Name?.StartsWith(s_filter) ?? false))
                    .ToList();
                
                if (a_potentialFiles.Count < 1) /* we found no file */
                {
                    /* prepare file path and name with file count '1' */
                    s_foo = s_foo.Substring(0, i_placeHolderPosition) + 1.ToString().PadLeft(i_leadingZeros + 1, '0') + s_foo.Substring(i_placeHolderPosition + 3);
                }
                else
                {
                    if (a_potentialFiles[0].Name != null)
                    {
                        string s_potentialFile = a_potentialFiles[0].Name ?? "Name property is null";

                        /* check if found file exceed our file limit from this configuration element */
                        if ((ForestNETLib.IO.File.FileLength(s_filePath + s_potentialFile) + p_i_lineLength) > this.FileLimitInBytes)
                        {
                            /* retrieve file part which is variable after position of file count placeholder */
                            string s_fileVariablePart = s_potentialFile.Substring(i_placeHolderPosition);
                            int i;

                            /* get position where file count part ends */
                            for (i = 0; i < s_fileVariablePart.Length; i++)
                            {
                                if (!char.IsDigit(s_fileVariablePart[i])) break;
                            }

                            /* retrieve file count value */
                            int i_fileNameNumber = int.Parse(s_fileVariablePart.Substring(0, i));
                            /* increase file count value */
                            i_fileNameNumber++;

                            /* if we exceed file count max. value, we start again with '1' */
                            if ((i_fileNameNumber > this.FileCount) || (a_potentialFiles.Count > this.FileCount))
                            {
                                i_fileNameNumber = 1;
                            }

                            /* prepare file path and name with increased file count */
                            s_foo = s_potentialFile.Substring(0, i_placeHolderPosition) + i_fileNameNumber.ToString().PadLeft(i_leadingZeros + 1, '0') + s_potentialFile.Substring(i_placeHolderPosition + i);

                            /* find files with leading zeros but same file name number, in case this setting has been changed over time */
                            for (int j = 0; j < 5; j++)
                            {
                                /* generate potential existing file path and name with our current increased file count value with all possible leading zeros */
                                string s_foo2 = s_potentialFile.Substring(0, i_placeHolderPosition) + i_fileNameNumber.ToString().PadLeft(j + 1, '0') + s_potentialFile.Substring(i_placeHolderPosition + i);

                                /* check if this potential file exists */
                                if (ForestNETLib.IO.File.Exists(s_filePath + s_foo2))
                                {
                                    /* delete it, so we can create a new one */
                                    ForestNETLib.IO.File.DeleteFile(s_filePath + s_foo2);
                                }
                            }
                        }
                        else
                        {
                            /* use found file */
                            s_foo = s_potentialFile ?? s_foo;
                        }
                    }
                }
            }

            return s_foo;
        }
    }

    /// <summary>
    /// Configuration class for file logger. Settings for map of categories and file paths and source path max. width are available.
    /// </summary>
    public class FileLoggerConfiguration
    {

        /* Fields */

        private readonly System.Collections.Generic.List<FileLoggerConfigurationElement> a_elements;

        /* Properties */

        public int EventId { get; set; }
        public FileLoggerConfigurationElement[] ConfigurationElements { get { return [.. this.a_elements]; } }
        public int SourceMaxWidth { get; set; }

        /* Methods */

        /// <summary>
        /// Constructor of file logger configuration
        /// </summary>
        public FileLoggerConfiguration()
        {
            this.a_elements = [];

            /* set standard source path max. width */
            this.SourceMaxWidth = 50;
        }

        /// <summary>
        /// Adding file configuration element to list, if category is not already in use
        /// </summary>
        /// <param name="p_o_element">file configuration element which will be added to list</param>
        /// <exception cref="ArgumentException">category is already used within FileLoggerConfiguration</exception>
        public void AddConfigurationElement(FileLoggerConfigurationElement p_o_element)
        {
            /* iterate all file configuration elements */
            foreach (FileLoggerConfigurationElement o_element in this.a_elements)
            {
                /* check if category is already in use */
                if (o_element.Category.Equals(p_o_element.Category))
                {
                    throw new ArgumentException("Category '" + p_o_element.Category + "' is already used within FileLoggerConfiguration.");
                }
            }

            /* add element to list */
            this.a_elements.Add(p_o_element);
        }

        /// <summary>
        /// Delete a file configuration element from list by category name
        /// </summary>
        /// <param name="p_s_cateogry">category name which is in use within FileLoggerConfiguration</param>
        /// <exception cref="ArgumentException">Element not found by category name</exception>
        public void DeleteConfigurationElementByCategory(string p_s_cateogry)
        {
            bool b_found = false;
            int i = 0;

            /* iterate all file configuration elements */
            foreach (FileLoggerConfigurationElement o_element in this.a_elements)
            {
                /* check if category matches parameter */
                if (o_element.Category.Equals(p_s_cateogry))
                {
                    b_found = true;
                    /* remove element from list */
                    this.a_elements.RemoveAt(i);
                }

                i++;
            }

            /* we have not found an element with category name */
            if (!b_found)
            {
                throw new ArgumentException("Category '" + p_s_cateogry + "' not found within FileLoggerConfiguration.");
            }
        }

        /// <summary>
        /// Deletes all configuration elements in current list
        /// </summary>
        public void ClearAllConfigurationElements()
        {
            this.a_elements.Clear();
        }
    }

    /// <summary>
    /// File logger class.
    /// </summary>
    public class FileLogger : ILogger, IDisposable
    {

        /* Fields */

        private readonly string s_name;
        private readonly FileHolder o_fileHolder;
        private readonly FileLoggerConfiguration o_config;

        /* Properties */

        public string Name
        {
            get { return this.s_name; }
        }

        /* Methods */

        /// <summary>
        /// File logger constructor
        /// </summary>
        /// <param name="p_s_name">logger name for using filter</param>
        /// <param name="p_o_fileHolder">file holder instance</param>
        /// <param name="p_o_config">file configuration instance</param>
        public FileLogger(string p_s_name, FileHolder p_o_fileHolder, FileLoggerConfiguration p_o_config)
        {
            /* set parameters to class variables */
            this.s_name = p_s_name;
            this.o_fileHolder = p_o_fileHolder;
            this.o_config = p_o_config;
        }

        /// <summary>
        /// Disposing file logger
        /// </summary>
        public void Dispose()
        {
            /* anything to close here */
            
            GC.SuppressFinalize(this);
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
            return true;
        }

        /// <summary>
        /// Writes a log entry to file
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
            string s_message = p_o_state.ToString() ?? "";

            /* if event id is zero or matches configured with parameter event id */
            if (this.o_config.EventId == 0 || this.o_config.EventId == p_o_eventId.Id)
            {
                /* cast log level to number */
                int i_logLevel = (int)p_o_logLevel;

                /* enqueue log level number and message */
                this.o_fileHolder.Queue.Enqueue(i_logLevel + s_message);
            }
        }
    }

    /// <summary>
    /// File logger provider, holding all loggers generated by logger factory.
    /// </summary>
    [ProviderAlias("File")]
    public class FileLoggerProvider : ILoggerProvider
    {

        /* Fields */

        private readonly System.Collections.Generic.Dictionary<string, FileHolder> a_fileHolders;
        private readonly System.Collections.Concurrent.ConcurrentDictionary<string, FileLogger> a_loggers;
        private readonly FileLoggerConfiguration o_config;

        /* Properties */

        /* Methods */

        /// <summary>
        /// File logger provider constructor
        /// </summary>
        /// <param name="p_o_config">file configuration instance, used when creating new file logger(s)</param>
        public FileLoggerProvider(FileLoggerConfiguration p_o_config)
        {
            if (p_o_config.ConfigurationElements.Length < 1)
            {
                throw new ArgumentException("File configuration instance has no elements for categories or file paths, it is empty.");
            }

            /* iterate each entry in list of file logger configuration elements */
            foreach (FileLoggerConfigurationElement o_element in p_o_config.ConfigurationElements)
            {
                /* check if we have a just one placeholder '%n[0-4]' for file count within file name */
                if ((o_element.FileCount > 1) && ((!ForestNETLib.Core.Helper.MatchesRegex(o_element.FileName, "(.*)(%n[0-4]{1})(.*)")) || (ForestNETLib.Core.Helper.CountSubStrings(o_element.FileName, "%n[0-4]{1}") != 1)))
                {
                    throw new ArgumentException("File configuration instance has a file count greater than 1, but no placeholder(only one allowed) '%n[0-4]' within file name setting.");
                }

                /* if we must handle multiple log files */
                if (o_element.FileCount > 1)
                {
                    /* get leading zeroes out of file count placeholder */
                    int i_leadingZeros = int.Parse(o_element.FileName.Substring(o_element.FileName.IndexOf("%n") + 2, 1));
                    int i_amountOfDigits;

                    /* determine our amount of digits for leading zeroes based on our file count setting */
                    if (o_element.FileCount < 10)
                    {
                        i_amountOfDigits = 1;
                    }
                    else if (o_element.FileCount < 100)
                    {
                        i_amountOfDigits = 2;
                    }
                    else if (o_element.FileCount < 1000)
                    {
                        i_amountOfDigits = 3;
                    }
                    else if (o_element.FileCount < 10000)
                    {
                        i_amountOfDigits = 4;
                    }
                    else
                    {
                        i_amountOfDigits = 5;
                    }

                    i_amountOfDigits--;

                    /* if our leading zeroes value within file name is lower than the digits we need for leading zeroes based on our file count setting, we throw an exception */
                    /* e.g. we have a file count of '< 1000' which means '1..999', we need at least '2' leading zeroes */
                    if (i_leadingZeros < i_amountOfDigits)
                    {
                        throw new ArgumentException("Cannot use file count of '" + o_element.FileCount + "' with leading zeros '" + i_leadingZeros + "' for file count within file name.");
                    }
                }
            }

            this.a_fileHolders = [];
            this.a_loggers = new System.Collections.Concurrent.ConcurrentDictionary<string, FileLogger>(StringComparer.OrdinalIgnoreCase);
            this.o_config = p_o_config;
        }

        /// <summary>
        /// Creating new file logger
        /// </summary>
        /// <param name="p_s_categoryName">logger name</param>
        /// <returns>object based on ILogger interface</returns>
        public ILogger CreateLogger(string p_s_categoryName)
        {
            /* get file holder instance by category name */
            if (this.a_fileHolders.TryGetValue(p_s_categoryName, out FileHolder? o_fileHolder))
            {
                /* check file holder instance */
                if (o_fileHolder == null)
                {
                    throw new NullReferenceException("File holder is null");
                }

                /* create new file logger instance with file holder and configuration instance */
                return a_loggers.GetOrAdd(p_s_categoryName, s_name => new FileLogger(s_name, o_fileHolder, this.o_config));
            }

            /* there is no file holder instance, so we must prepare a new one or find an existing one if multiple categories using just one single file path */
            bool b_foundAnotherFileHolder = false;

            /* need to determine file configuration element for file holder based on list of configuration elements of file logger configuration */
            FileLoggerConfigurationElement? o_fileConfigurationElement = null;

            /* iterate each entry in list of file logger configuration elements */
            foreach (FileLoggerConfigurationElement o_element in this.o_config.ConfigurationElements)
            {
                if (o_element.Category.Contains(';')) /* multiple category value recognized */
                {
                    /* split category value into multiple category names */
                    string[] a_categories = o_element.Category.Split(";");
                    bool b_found = false;

                    /* iterate each category name */
                    foreach (string s_category in a_categories)
                    {
                        /* if we find with another category a file holder, we must use this one, because our configuration wants it so */
                        if ((this.a_fileHolders.TryGetValue(s_category, out FileHolder? o_valueFileholder)) && (!s_category.Equals(p_s_categoryName)))
                        {
                            /* get existing file holder instance from other category name */
                            o_fileHolder = o_valueFileholder;
                            /* set other file holder flag */
                            b_foundAnotherFileHolder = true;
                            /* set found flag */
                            b_found = true;

                            break;
                        }

                        /* check if we found our category */
                        if (s_category.Equals(p_s_categoryName))
                        {
                            /* set found flag */
                            b_found = true;
                            /* category found, we can break our loop here */
                            break;
                        }
                    }

                    /* if found flag is set */
                    if (b_found)
                    {
                        /* set file configuration element from list of file logger configuration */
                        o_fileConfigurationElement = o_element;
                        /* file configuration element set, we can break our loop here */
                        break;
                    }
                }
                else /* single category value recognized */
                {
                    /* check if we found our category */
                    if (o_element.Category.Equals(p_s_categoryName))
                    {
                        /* set file configuration element from list of file logger configuration */
                        o_fileConfigurationElement = o_element;
                        /* file configuration element set, we can break our loop here */
                        break;
                    }
                }
            }

            /* create new file holder instance if we have not find another file holder instance by category name */
            if (!b_foundAnotherFileHolder)
            {
                /* create new file holder instance and set found file configuration element and source max. width from configuration instance */
                o_fileHolder = new FileHolder
                {
                    FileConfigurationElement = o_fileConfigurationElement ?? throw new ArgumentException("Could not find a file path for category '" + p_s_categoryName + "' within file logger configuration."),
                    SourceMaxWidth = this.o_config.SourceMaxWidth
                };

                /* start thread within file holder which is writing log messages to file which arrives to file holder queue */
                o_fileHolder.Thread = new(o_fileHolder.WriteLogWithThread);
                o_fileHolder.Thread.Start();
            }

            /* check file holder instance */
            if (o_fileHolder == null)
            {
                throw new NullReferenceException("File holder is null");
            }

            /* add file holder instance with our category name to dictionary */
            this.a_fileHolders.Add(p_s_categoryName, o_fileHolder);

            /* create new file logger instance with file holder and configuration instance */
            return a_loggers.GetOrAdd(p_s_categoryName, s_name => new FileLogger(s_name, o_fileHolder, this.o_config));
        }

        /// <summary>
        /// Disposing file logger provider will dispose all logger as well
        /// </summary>
        public void Dispose()
        {
            /* iterate all file holders and dispose them, before clearing the dictionary */
            foreach (var o_fileHolder in a_fileHolders)
            {
                o_fileHolder.Value.Dispose();
            }

            a_fileHolders.Clear();

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
