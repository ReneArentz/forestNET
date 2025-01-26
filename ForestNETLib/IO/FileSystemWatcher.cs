namespace ForestNETLib.IO
{
    /// <summary>
    /// Abstract FileSystemWatcher class to get events about file create, change, delete and access events of a directory.
    /// optional to get events recursive for all sub directories and optional file extension filter.
    /// Inherit from this class and define the event methods to tell your program what to do if an event is fired.
    /// </summary>
    public abstract class FileSystemWatcher : ForestNETLib.Core.TimerTask
    {

        /* Fields */

        private readonly string s_directory;
        private System.Collections.Generic.List<ListingElement> a_files;
        private ForestNETLib.Core.Timer? o_timerObject;
        private bool b_recursive;
        private bool b_create;
        private bool b_change;
        private bool b_delete;
        private bool b_access;
        private string s_fileExtensionFilter;

        /* Properties */

        public bool Recursive
        {
            get
            {
                return this.b_recursive;
            }
            set
            {
                this.b_recursive = value;
                ForestNETLib.Core.Global.ILogConfig("updated recursive to '" + value + "'");
            }
        }

        public bool Create
        {
            get
            {
                return this.b_create;
            }
            set
            {
                this.b_create = value;
                ForestNETLib.Core.Global.ILogConfig("updated create to '" + value + "'");
            }
        }

        public bool Change
        {
            get
            {
                return this.b_change;
            }
            set
            {
                this.b_change = value;
                ForestNETLib.Core.Global.ILogConfig("updated change to '" + value + "'");
            }
        }

        public bool Delete
        {
            get
            {
                return this.b_delete;
            }
            set
            {
                this.b_delete = value;
                ForestNETLib.Core.Global.ILogConfig("updated delete to '" + value + "'");
            }
        }

        public bool Access
        {
            get
            {
                return this.b_access;
            }
            set
            {
                this.b_access = value;
                ForestNETLib.Core.Global.ILogConfig("updated access to '" + value + "'");
            }
        }

        public string FileExtensionFilter
        {
            get
            {
                return this.s_fileExtensionFilter;
            }
            set
            {
                this.s_fileExtensionFilter = value;
                ForestNETLib.Core.Global.ILogConfig("updated file extension filter to '" + value + "'");
            }
        }

        /* Methods */

        /// <summary>
        /// Create a file system watcher instance with interval value
        /// </summary>
        /// <param name="p_s_directory">full path to directory which will be watched</param>
        /// <param name="p_o_interval">interval when at the end the timer task will be always executed and interval will start new</param>
        /// <exception cref="ArgumentException">directory parameter is not a valid path</exception>
        public FileSystemWatcher(string p_s_directory, ForestNETLib.Core.DateInterval p_o_interval) : this(p_s_directory, p_o_interval, System.TimeSpan.Zero)
        {

        }

        /// <summary>
        /// Create a file system watcher instance with interval value and a start time
        /// </summary>
        /// <param name="p_s_directory">full path to directory which will be watched</param>
        /// <param name="p_o_interval">interval when at the end the timer task will be always executed and interval will start new</param>
        /// <param name="p_o_startTime">start time when the timer task will execute for the first time</param>
        /// <exception cref="ArgumentException">directory parameter is not a valid path</exception>
        public FileSystemWatcher(string p_s_directory, ForestNETLib.Core.DateInterval p_o_interval, System.TimeSpan p_o_startTime) : base(p_o_interval, p_o_startTime)
        {
            /* check if directory parameter is a real directory */
            if (!File.IsDirectory(p_s_directory))
            {
                throw new ArgumentException("File path[" + p_s_directory + "] is not a valid directory");
            }

            /* init class fields */
            this.s_directory = p_s_directory;
            this.a_files = [];
            this.o_timerObject = null;
            this.b_recursive = false;
            this.b_create = false;
            this.b_change = false;
            this.b_delete = false;
            this.b_access = false;
            this.s_fileExtensionFilter = "";
        }

        /// <summary>
        /// get dynamic list of file extension filter elements
        /// </summary>
        /// <returns>string list</returns>
        private System.Collections.Generic.List<string> GetFileExtensionFilter()
        {
            /* list of file extension or other filter restrictions we want to use */
            System.Collections.Generic.List<string> a_filter = [];

            /* check if file extension filter is not empty */
            if (!ForestNETLib.Core.Helper.IsStringEmpty(this.s_fileExtensionFilter))
            {
                /* if value contains delimiter, we need to split up the file extension filter */
                if (this.s_fileExtensionFilter.Contains('|'))
                {
                    foreach (string s_fileExtensionFilter in this.s_fileExtensionFilter.Split("|"))
                    {
                        /* add file extension filter to list */
                        a_filter.Add(s_fileExtensionFilter);
                    }
                }
                else
                {
                    /* add file extension filter to list */
                    a_filter.Add(s_fileExtensionFilter);
                }
            }

            /* return list of filter restrictions */
            return a_filter;
        }

        /// <summary>
        /// Start file system watcher with timer object as basis.
        /// multiple starts are not possible.
        /// </summary>
        /// <exception cref="System.IO.IOException">directory could not be listed or issue with reading file/directory attributes</exception>
        /// <exception cref="ArgumentException">path does not exist</exception>
        public void StartWatcher()
        {
            /* only start if timer object is not set, preventing double start */
            if (this.o_timerObject == null)
            {
                /* list of file extension or other filter restrictions we want to use */
                System.Collections.Generic.List<string> a_filter = this.GetFileExtensionFilter();

                /* iterate all files in observing directory */
                foreach (ListingElement o_listingElement in File.ListDirectory(this.s_directory, this.b_recursive))
                {
                    /* if we have any filter values */
                    if ((a_filter.Count > 0) && (o_listingElement.Name != null))
                    {
                        /* if one filter value allows all files with any extension, we do not need any restriction */
                        if (!a_filter.Contains("*.*"))
                        {
                            bool b_match = false;

                            /* iterate each filter restriction */
                            foreach (string s_filter in a_filter)
                            {
                                ForestNETLib.Core.Global.ILog(s_filter + " - " + o_listingElement.Name);
                                /* if filter starts with wildcard, we are controlling just the end of file names */
                                if (s_filter.StartsWith("*"))
                                {
                                    /* remove wildcard */
                                    string s_filter2 = s_filter.Substring(1);

                                    /* check if filename ends with filter restriction */
                                    if (o_listingElement.Name.EndsWith(s_filter2))
                                    {
                                        b_match = true;
                                    }
                                }
                                else
                                { /* otherwise file name must match completely */
                                    if (o_listingElement.Name.Equals(s_filter))
                                    {
                                        b_match = true;
                                    }
                                }
                            }

                            /* if we have no match with our restriction, we can skip this file */
                            if (!b_match)
                            {
                                continue;
                            }
                        }
                    }

                    /* add file to file list */
                    this.a_files.Add(o_listingElement);
                    ForestNETLib.Core.Global.ILogFinest("Added file on start of file system watcher to have it's initial state [" + o_listingElement.FullName + "]");
                }

                /* create timer object and start timer */
                this.o_timerObject = new ForestNETLib.Core.Timer(this);
                this.Stop = false;
                this.o_timerObject.StartTimer();
                ForestNETLib.Core.Global.ILogFiner("File system watcher has been started");
            }
            else
            {
                ForestNETLib.Core.Global.ILog("File system watcher already has been started - no action has been done");
            }
        }

        /// <summary>
        /// Stops file system watcher and sets object null, so it can be start again
        /// multiple stops are not possible
        /// </summary>
        public void StopWatcher()
        {
            /* only stop if timer object is set, preventing double stop */
            if (this.o_timerObject != null)
            {
                this.Stop = true;
                this.o_timerObject.StopTimer();
                this.o_timerObject = null;
                ForestNETLib.Core.Global.ILogFiner("File system watcher has been stopped");
            }
            else
            {
                ForestNETLib.Core.Global.ILog("File system watcher already has been stopped or never started - no action has been done");
            }
        }

        /// <summary>
        /// File System Watcher run method which will recognize and fire events
        /// </summary>
        /// <exception cref="System.IO.IOException">directory could not be listed or issue with reading file/directory attributes</exception>
        /// <exception cref="ArgumentException">path does not exist</exception>
        override public void RunTimerTask()
        {
            /* temp list for scanning file of newest state */
            System.Collections.Generic.List<ListingElement> a_currentFiles = [];

            /* list of file extension or other filter values we want to use */
            System.Collections.Generic.List<string> a_filter = this.GetFileExtensionFilter();

            /* iterate all files in observing directory */
            foreach (ListingElement o_listingElement in File.ListDirectory(this.s_directory, this.b_recursive))
            {
                /* if we have any filter values */
                if ((a_filter.Count > 0) && (o_listingElement.Name != null))
                {
                    /* if one filter value allows all files with any extension, we do not need any restriction */
                    if (!a_filter.Contains("*.*"))
                    {
                        bool b_match = false;

                        /* iterate each filter restriction */
                        foreach (string s_filter in a_filter)
                        {
                            /* if filter starts with wildcard, we are controlling just the end of file names */
                            if (s_filter.StartsWith("*"))
                            {
                                /* remove wildcard */
                                string s_filter2 = s_filter.Substring(1);

                                /* check if filename ends with filter restriction */
                                if (o_listingElement.Name.EndsWith(s_filter2))
                                {
                                    b_match = true;
                                }
                            }
                            else
                            { /* otherwise file name must match completely */
                                if (o_listingElement.Name.Equals(s_filter))
                                {
                                    b_match = true;
                                }
                            }
                        }

                        /* if we have no match with our restriction, we can skip this file */
                        if (!b_match)
                        {
                            continue;
                        }
                    }
                }

                /* add file to temporary file list */
                a_currentFiles.Add(o_listingElement);
            }

            /* iterate all files from last timer iteration, to find changed and deleted files */
            foreach (ListingElement o_listingElement in this.a_files)
            {
                /* skip elements which are directories */
                if (o_listingElement.IsDirectory)
                {
                    continue;
                }

                /* help variables for file state comparison */
                ListingElement? o_compareListingElement = null;
                bool b_found = false;

                /* iterate all files of newest state */
                foreach (ListingElement o_searchListingElement in a_currentFiles)
                {
                    /* if full name matches, file still exists */
                    if (o_listingElement.FullName == o_searchListingElement.FullName)
                    {
                        o_compareListingElement = o_searchListingElement;
                        b_found = true;
                        break;
                    }
                }

                if (b_found)
                { /* file still exists */
                    /* check if we want to handle change events */
                    if (this.b_change)
                    {
                        if (o_compareListingElement?.LastModifiedTime?.CompareTo(o_listingElement.LastModifiedTime) > 0)
                        {
                            /* fire change event */
                            ForestNETLib.Core.Global.ILogFinest("fire change event '" + o_compareListingElement.FullName + "'");
                            this.ChangeEvent(o_compareListingElement);
                        }
                    }

                    /* check if we want to handle access events */
                    if (this.b_access)
                    {
                        if (o_compareListingElement?.LastAccessTime?.CompareTo(o_listingElement.LastAccessTime) > 0)
                        {
                            /* fire access event */
                            ForestNETLib.Core.Global.ILogFinest("fire access event '" + o_compareListingElement.FullName + "'");
                            this.AccessEvent(o_compareListingElement);
                        }
                    }
                }
                else
                { /* file was deleted */
                    /* check if we want to handle delete events */
                    if (this.b_delete)
                    {
                        /* fire delete event */
                        ForestNETLib.Core.Global.ILogFinest("fire delete event '" + o_listingElement.FullName + "'");
                        this.DeleteEvent(o_listingElement);
                    }
                }
            }

            /* iterate all files of newest state, to find new files */
            foreach (ListingElement o_listingElement in a_currentFiles)
            {
                /* skip elements which are directories */
                if (o_listingElement.IsDirectory)
                {
                    continue;
                }

                /* help variable for file state comparison */
                bool b_found = false;

                /* iterate all files from last timer iteration */
                foreach (ListingElement o_searchListingElement in this.a_files)
                {
                    /* if full name matches, file exists before */
                    if (o_listingElement.FullName == o_searchListingElement.FullName)
                    {
                        b_found = true;
                        break;
                    }
                }

                /* check if file not exists before and if we want to handle create events */
                if ((!b_found) && (this.b_create))
                {
                    /* fire create event */
                    ForestNETLib.Core.Global.ILogFinest("fire create event '" + o_listingElement.FullName + "'");
                    this.CreateEvent(o_listingElement);
                }
            }

            /* update file list with temporary file list for next timer tick */
            this.a_files = a_currentFiles;
        }

        /// <summary>
        /// What should happen if a new file has been created
        /// </summary>
        /// <param name="p_o_listingElement">file element of type ListingElement</param>
        abstract public void CreateEvent(ListingElement p_o_listingElement);

        /// <summary>
        /// What should happen if a file has been changed
        /// </summary>
        /// <param name="p_o_listingElement">file element of type ListingElement</param>
        abstract public void ChangeEvent(ListingElement p_o_listingElement);

        /// <summary>
        /// What should happen if a file has been deleted
        /// </summary>
        /// <param name="p_o_listingElement">file element of type ListingElement</param>
        abstract public void DeleteEvent(ListingElement p_o_listingElement);

        /// <summary>
        /// What should happen if a file has been accessed
        /// ### NOT WORKING ON EVERY OS, OS SETTINGS CAN PREVENT THIS EVENT ###
        /// </summary>
        /// <param name="p_o_listingElement">file element of type ListingElement</param>
        abstract public void AccessEvent(ListingElement p_o_listingElement);
    }
}