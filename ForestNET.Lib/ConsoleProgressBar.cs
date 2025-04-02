namespace ForestNET.Lib
{
    /// <summary>
    /// Creates a progress bar which can be used in consoles.
    /// Possibility of giving a starting text, done text and marquee text while progress is processing.
    /// </summary>
    public class ConsoleProgressBar
    {

        /* Fields */

        private readonly long l_animationInterval;
        private readonly int i_blockLength;
        private readonly int i_marqueeLength;
        private readonly int i_marqueeInterval;
        private System.Timers.Timer? o_timer;
        private ProgressBarTimerHandler? o_progressBarTimerHandler;

        /* Properties */

        /// <summary>
        /// replace start text of console progress bar task while running
        /// </summary>
        public string? StartText
        {
            set
            {
                if (this.o_progressBarTimerHandler != null)
                {
                    this.o_progressBarTimerHandler.StartText = value;
                }
            }
        }

        /// <summary>
        /// replace done text of console progress bar task while running
        /// </summary>
        public string? DoneText
        {
            set
            {
                if (this.o_progressBarTimerHandler != null)
                {
                    this.o_progressBarTimerHandler.DoneText = value;
                }
            }
        }

        /// <summary>
        /// replace marquee text of console progress bar task while running
        /// </summary>
        public string? MarqueeText
        {
            set
            {
                if (this.o_progressBarTimerHandler != null)
                {
                    this.o_progressBarTimerHandler.MarqueeText = value;
                }
            }
        }

        /// <summary>
        /// overwrite console progress bar progress by reporting a new value
        /// </summary>
        public double Report
        {
            set
            {
                if (this.o_progressBarTimerHandler != null)
                {
                    this.o_progressBarTimerHandler.Progress = Math.Max(0, Math.Min(1, value));
                    if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("Reported progress: '" + Math.Max(0, Math.Min(1, value)) + "'");
                }
            }
        }

        /* Methods */

        /// <summary>
        /// Create console progress bar instance.
        /// animation interval: 125 - block length: 32 - marquee length: 32 - marquee interval: 4
        /// </summary>
        public ConsoleProgressBar() : this(32)
        {

        }

        /// <summary>
        /// Create console progress bar instance.
        /// animation interval: 125 - marquee interval: 4
        /// </summary>
        /// <param name="p_i_blockLength">length of the console progress bar animation and length of marquee text</param>
        public ConsoleProgressBar(int p_i_blockLength) : this(125, p_i_blockLength, p_i_blockLength)
        {

        }

        /// <summary>
        /// Create console progress bar instance.
        /// marquee interval: 4
        /// </summary>
        /// <param name="p_l_animationInterval">time interval in milliseconds for animation task execution</param>
        /// <param name="p_i_blockLength">length of the console progress bar animation and length of marquee text</param>
        public ConsoleProgressBar(long p_l_animationInterval, int p_i_blockLength) : this(p_l_animationInterval, p_i_blockLength, p_i_blockLength)
        {

        }

        /// <summary>
        /// Create console progress bar instance.
        /// marquee interval: 4
        /// </summary>
        /// <param name="p_l_animationInterval">time interval in milliseconds for animation task execution</param>
        /// <param name="p_i_blockLength">length of the console progress bar animation</param>
        /// <param name="p_i_marqueeLength">length of marquee text</param>
        /// <param name="p_i_marqueeInterval">sub time interval of animation interval for marquee movement speed, e.g. 4 means every 4*animationInterval</param>
        public ConsoleProgressBar(long p_l_animationInterval, int p_i_blockLength, int p_i_marqueeLength) : this(p_l_animationInterval, p_i_blockLength, p_i_marqueeLength, 4)
        {

        }

        /// <summary>
        /// Create console progress bar instance.
        /// animation interval: 125 - block length: 32
        /// </summary>
        /// <param name="p_i_marqueeLength">length of marquee text</param>
        /// <param name="p_i_marqueeInterval">sub time interval of animation interval for marquee movement speed, e.g. 4 means every 4*animationInterval</param>
        public ConsoleProgressBar(int p_i_marqueeLength, int p_i_marqueeInterval) : this(125, 32, p_i_marqueeLength, p_i_marqueeInterval)
        {

        }

        /// <summary>
        /// Create console progress bar instance.
        /// </summary>
        /// <param name="p_l_animationInterval">time interval in milliseconds for animation task execution</param>
        /// <param name="p_i_blockLength">length of the console progress bar animation</param>
        /// <param name="p_i_marqueeLength">length of marquee text</param>
        /// <param name="p_i_marqueeInterval">sub time interval of animation interval for marquee movement speed, e.g. 4 means every 4*animationInterval</param>
        public ConsoleProgressBar(long p_l_animationInterval, int p_i_blockLength, int p_i_marqueeLength, int p_i_marqueeInterval)
        {
            this.l_animationInterval = p_l_animationInterval;
            this.i_blockLength = p_i_blockLength;
            this.i_marqueeLength = p_i_marqueeLength;
            this.i_marqueeInterval = p_i_marqueeInterval;

            ForestNET.Lib.Global.ILogConfig("animationInterval: '" + this.l_animationInterval + "'");
            ForestNET.Lib.Global.ILogConfig("blockLength: '" + this.i_blockLength + "'");
            ForestNET.Lib.Global.ILogConfig("marqueeLength: '" + this.i_marqueeLength + "'");
            ForestNET.Lib.Global.ILogConfig("marqueeInterval: '" + this.i_marqueeInterval + "'");
        }

        /// <summary>
        /// Initialize and start rendering progress bar in console, can be reused after closing instance
        /// </summary>
        /// <exception cref="InvalidOperationException">progress bar is already initialised</exception>
        public void Init()
        {
            this.Init(null, null, null);
        }

        /// <summary>
        /// Initialize and start rendering progress bar in console, can be reused after closing instance
        /// </summary>
        /// <param name="p_s_startText">text shown if progress bar is starting</param>
        /// <param name="p_s_doneText">text shown if progress bar is done</param>
        /// <exception cref="InvalidOperationException">progress bar is already initialised</exception>
        public void Init(string? p_s_startText, string? p_s_doneText)
        {
            this.Init(p_s_startText, p_s_doneText, null);
        }

        /// <summary>
        /// Initialize and start rendering progress bar in console, can be reused after closing instance
        /// </summary>
        /// <param name="p_s_startText">text shown if progress bar is starting</param>
        /// <param name="p_s_doneText">text shown if progress bar is done</param>
        /// <param name="p_s_marqueeText">text within marquee area</param>
        /// <exception cref="InvalidOperationException">progress bar is already initialised</exception>
        public void Init(string? p_s_startText, string? p_s_doneText, string? p_s_marqueeText)
        {
            if ((this.o_progressBarTimerHandler != null) && (this.o_progressBarTimerHandler.Started))
            {
                throw new InvalidOperationException("ProgressBar is already initialised.");
            }

            /* create new instance of private sub class */
            this.o_progressBarTimerHandler = new ProgressBarTimerHandler(this.i_blockLength, this.i_marqueeLength, this.i_marqueeInterval)
            {
                StartText = p_s_startText,
                DoneText = p_s_doneText,
                MarqueeText = p_s_marqueeText
            };

            ForestNET.Lib.Global.ILogConfig("startText: '" + p_s_startText + "'");
            ForestNET.Lib.Global.ILogConfig("doneText: '" + p_s_doneText + "'");
            ForestNET.Lib.Global.ILogConfig("marqueeText: '" + p_s_marqueeText + "'");

            /* create timer instance, starting timer with task and animation interval */
            this.o_timer = new System.Timers.Timer();
            this.o_timer.Elapsed += async (p_o_sender, p_o_elapsedEventArgs) => await this.o_progressBarTimerHandler.TimerRun();
            this.o_timer.Interval = this.l_animationInterval;
            this.o_timer.AutoReset = true;
            this.o_timer.Enabled = true;

            /* initialize progress bar timer handler flags */
            this.o_progressBarTimerHandler.Stop = false;
            this.o_progressBarTimerHandler.Started = true;

            ForestNET.Lib.Global.ILogFinest("ConsoleProgressBar timer started");
            ForestNET.Lib.Global.ILogFinest("ConsoleProgressBar initialised");
        }

        /// <summary>
        /// Closed console progress bar, but can be reused by executing Init again
        /// </summary>
        public void Close()
        {
            /* stop task and truncate shown text */
            this.o_progressBarTimerHandler?.Close();

            /* purge timer instance */
            if (this.o_timer != null)
            {
                this.o_timer.Stop();
                this.o_timer.Dispose();
                ForestNET.Lib.Global.ILogFinest("ConsoleProgressBar timer stopped");
            }

            ForestNET.Lib.Global.ILogFinest("ConsoleProgressBar closed");
        }

        /* Internal Classes */

        private class ProgressBarTimerHandler : Runnable
        {

            /* Constants */

            private const int i_lockTimeout = 30000;

            /* Fields */

            private readonly int i_blockLength = 32;
            private readonly int i_marqueeLength = 32;
            private double d_progress = 0;

            private readonly System.Threading.ReaderWriterLock o_lock = new();

            private readonly string s_animationCharacters = "|/-\\";
            private int i_animationIndex = 0;
            private int i_marqueeIndex = -1;
            private bool b_scrollForward = true;
            private int i_scrollCounter = 0;
            private readonly int i_marqueeInterval = 4;

            private string? s_startText = "";
            private string? s_doneText = "";
            private string? s_marqueeText = "";
            private string s_text = "";

            /* Properties */

            /// <summary>
            /// thread safe property to read/write progress value
            /// </summary>
            public double Progress
            {
                get
                {
                    double d_foo;
                    this.o_lock.AcquireReaderLock(i_lockTimeout);
                    d_foo = this.d_progress;
                    this.o_lock.ReleaseReaderLock();
                    return d_foo;
                }
                set
                {
                    this.o_lock.AcquireWriterLock(i_lockTimeout);
                    this.d_progress = value;
                    this.o_lock.ReleaseWriterLock();
                }
            }

            /// <summary>
            /// thread safe property to set start text value
            /// </summary>
            public string? StartText
            {
                set
                {
                    if (!ForestNET.Lib.Helper.IsStringEmpty(value))
                    {
                        this.o_lock.AcquireWriterLock(i_lockTimeout);
                        this.s_startText = value;
                        this.o_lock.ReleaseWriterLock();
                    }
                }
            }

            /// <summary>
            /// thread safe property to set done text value
            /// </summary>
            public string? DoneText
            {
                set
                {
                    if (!ForestNET.Lib.Helper.IsStringEmpty(value))
                    {
                        this.o_lock.AcquireWriterLock(i_lockTimeout);
                        this.s_doneText = value;
                        this.o_lock.ReleaseWriterLock();
                    }
                }
            }

            /// <summary>
            /// thread safe property to set marquee text value
            /// </summary>
            public string? MarqueeText
            {
                set
                {
                    if (!ForestNET.Lib.Helper.IsStringEmpty(value))
                    {
                        this.o_lock.AcquireWriterLock(i_lockTimeout);
                        this.s_marqueeText = value;
                        this.o_lock.ReleaseWriterLock();
                    }
                }
            }

            /* Methods */

            /// <summary>
            /// Create progress bar timer task instance
            /// </summary>
            /// <param name="p_i_blockLength">length of the console progress bar animation</param>
            /// <param name="p_i_marqueeLength">length of marquee text</param>
            /// <param name="p_i_marqueeInterval">sub time interval of animation interval for marquee movement speed, e.g. 4 means every 4*animationInterval</param>
            public ProgressBarTimerHandler(int p_i_blockLength, int p_i_marqueeLength, int p_i_marqueeInterval)
            {
                this.i_blockLength = p_i_blockLength;
                this.i_marqueeLength = p_i_marqueeLength;
                this.i_marqueeInterval = p_i_marqueeInterval;
            }

            /// <summary>
            /// Method for .NET timer ElapsedEventHandler async task
            /// </summary>
            public System.Threading.Tasks.Task TimerRun()
            {
                /* executing run method of progress bar timer handler as task */
                return System.Threading.Tasks.Task.Run(() => Run());
            }

            /// <summary>
            /// Progress bar timer task run method which will be executed each interval
            /// </summary>
            override public void Run()
            {
                /* timer task is already disposed */
                if (this.Stop) return;

                string s_foo = "";

                try
                {
                    if (!ForestNET.Lib.Helper.IsStringEmpty(this.s_startText))
                    {
                        s_foo = this.s_startText + " ";
                    }

                    /* implement marquee text if it has a value */
                    if ((this.s_marqueeText != null) && (!ForestNET.Lib.Helper.IsStringEmpty(this.s_marqueeText)))
                    {
                        if (this.s_marqueeText.Length == this.i_marqueeLength)
                        {
                            /* marquee text length and marquee block length miraculously are the same */
                            s_foo += "<" + this.s_marqueeText + "> ";
                        }
                        else
                        {
                            /* implement marquee interval with modulo calculation and an endless counter, or index < 0 */
                            if ((this.i_scrollCounter % this.i_marqueeInterval == 0) || (this.i_marqueeIndex < 0))
                            {
                                if (this.b_scrollForward)
                                {
                                    /* increment index if we are scrolling forward */
                                    this.i_marqueeIndex++;
                                }
                                else
                                {
                                    /* decrement index if we are not scrolling forward */
                                    this.i_marqueeIndex--;
                                }
                            }

                            /* calculate start index */
                            int i_start = this.i_marqueeIndex % this.s_marqueeText.Length;
                            /* calculate end index */
                            int i_end = this.i_marqueeIndex + this.i_marqueeLength;

                            if (this.i_marqueeIndex == 0)
                            {
                                /* scrolling forward if we start or returned to index 0 */
                                this.b_scrollForward = true;
                            }
                            else if (i_end >= this.s_marqueeText.Length)
                            {
                                /* scrolling backward if end index greater equal complete marquee text length */
                                this.b_scrollForward = false;
                            }
                            else
                            {
                                /* keep scroll state unchanged */
                            }

                            /* handle state if marquee text length is lower than specified marquee length for progress bar */
                            if (this.s_marqueeText.Length < this.i_marqueeLength)
                            {
                                /* overwrite index values, so the whole marquee text will be shown */
                                i_start = 0;
                                i_end = this.s_marqueeText.Length;
                            }

                            /* render marquee substring */
                            s_foo += "<" + this.s_marqueeText.Substring(i_start, i_end - i_start) + "> ";

                            /* increment endless counter */
                            this.i_scrollCounter++;
                        }
                    }

                    /* calculate how many blocks we can render for progress */
                    int i_handlerBlockCount = (int)(this.Progress * i_blockLength);

                    /* render the actual progress bar in console, '#' sign for completed progress, '-' sign for uncompleted progress */
                    s_foo += "[" + ProgressBarTimerHandler.CreateCharacterChain('#', i_handlerBlockCount) + ProgressBarTimerHandler.CreateCharacterChain('-', i_blockLength - i_handlerBlockCount) + "]";
                    /* render percentage value of progress bar in console + animation character */
                    s_foo += " " + String.Format("{0:0.00}", (this.Progress * 100.0d)) + "% " + s_animationCharacters[i_animationIndex++ % s_animationCharacters.Length];
                }
                catch (Exception o_exc)
                {
                    s_foo += o_exc.Message;
                }

                try
                {
                    /* update rendering of progress bar */
                    this.Update(s_foo);
                }
                catch (Exception o_exc)
                {
                    /* log exception of rendering progress bar */
                    ForestNET.Lib.Global.LogException("Exception in rendering console progress bar with Update(String): ", o_exc);
                }
            }

            /// <summary>
            /// Method for rendering progress bar
            /// </summary>
            /// <param name="p_s_text">complete console progress bar text, with animation, marquee, percentage and text</param>
            private void Update(string p_s_text)
            {
                /* calculate prefix length, where no characters must be changed */
                int i_prefixLength = 0;

                while (
                        (i_prefixLength < Math.Min(this.s_text.Length, p_s_text.Length)) /* prefix length is lower than minimum of local text field or text parameter value */
                         && (p_s_text[i_prefixLength] == this.s_text[i_prefixLength]) /* local text field and text parameter value have the same character on the same position */
                )
                {
                    i_prefixLength++;
                }

                /* backspace to the first differing character */
                System.Text.StringBuilder o_stringBuilder = new();
                o_stringBuilder.Append(ProgressBarTimerHandler.CreateCharacterChain('\b', this.s_text.Length - i_prefixLength));

                /* add new suffix */
                o_stringBuilder.Append(p_s_text.AsSpan(i_prefixLength));

                /* get amount of overlapping characters */
                int i_overlapCount = this.s_text.Length - p_s_text.Length;

                /* if new text is shorter, delete overlapping characters */
                if (i_overlapCount > 0)
                {
                    o_stringBuilder.Append(ProgressBarTimerHandler.CreateCharacterChain(' ', i_overlapCount));
                    o_stringBuilder.Append(ProgressBarTimerHandler.CreateCharacterChain('\b', i_overlapCount));
                }

                /* render progress bar */
                System.Console.Write(o_stringBuilder);

                /* store new text in local field */
                this.s_text = p_s_text;
            }

            /// <summary>
            /// Closing progress bar timer task and render done text value
            /// </summary>
            public void Close()
            {
                /* stop flag, to stop run method from executing */
                this.Stop = true;
                /* tells if run method is not started anymore */
                this.Started = false;
                /* delete console progress bar text */
                /* so you can enter any status text after progress has been finished or canceled */
                this.Update("");

                /* render done text value */
                System.Console.Write(this.s_doneText + ForestNET.Lib.IO.File.NEWLINE);
            }

            /// <summary>
            /// Creates a string chain of one character only
            /// </summary>
            /// <param name="p_c_char">character used for string chain</param>
            /// <param name="p_i_amount">length of string chain</param>
            /// <returns>String</returns>
            private static string CreateCharacterChain(char p_c_char, int p_i_amount)
            {
                System.Text.StringBuilder o_stringBuilder = new();

                for (int i = 0; i < p_i_amount; i++)
                {
                    o_stringBuilder.Append(p_c_char);
                }

                return o_stringBuilder.ToString();
            }
        }
    }
}
