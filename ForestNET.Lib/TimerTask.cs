namespace ForestNET.Lib
{
    /// <summary>
    /// Abstract timer task class creating an instance which holds the developer specified run method, interval and start time.
    /// </summary>
    public abstract class TimerTask
    {

        /* Fields */

        private readonly System.Collections.Generic.List<System.DayOfWeek> a_excludeWeekdays;

        /* Properties */

        public ForestNET.Lib.DateInterval Interval { get; private set; }
        public System.TimeSpan StartTime { get; private set; }
        public bool Repeat { get; set; }
        public bool ShowException { get; set; }
        protected bool Stop { get; set; }

        /* Methods */

        /// <summary>
        /// Create a timer task instance with interval value
        /// </summary>
        /// <param name="p_o_interval">interval when at the end the timer task will be always executed and interval will start new</param>
        public TimerTask(ForestNET.Lib.DateInterval p_o_interval) : this(p_o_interval, System.TimeSpan.Zero)
        {

        }

        /// <summary>
        /// Create a timer task instance with interval value and a start time
        /// </summary>
        /// <param name="p_o_interval">interval when at the end the timer task will be always executed and interval will start new</param>
        /// <param name="p_o_startTime">start time when the timer task will execute for the first time</param>
        public TimerTask(ForestNET.Lib.DateInterval p_o_interval, System.TimeSpan p_o_startTime)
        {
            this.a_excludeWeekdays = [];
            this.Interval = p_o_interval;
            this.StartTime = p_o_startTime;
            this.Repeat = true;
            this.ShowException = false;
            this.Stop = false;

            ForestNET.Lib.Global.ILogConfig("Created TimerTask with interval '" + p_o_interval + "' and start time '" + p_o_startTime + "'");
        }

        /// <summary>
        /// Delete all exclude weekdays settings
        /// </summary>
        public void ClearExcludeWeekdays()
        {
            this.a_excludeWeekdays.Clear();
            ForestNET.Lib.Global.ILogConfig("Exclude weekday settings cleared");
        }

        /// <summary>
        /// Define weekdays where timer task will not be executed, e.g. Saturday and Sunday<br />
        /// 										1 - System.DayOfWeek.Monday<br />
        /// 										2 - System.DayOfWeek.Tuesday<br />
        /// 										3 - System.DayOfWeek.Wednesday<br />
        /// 										4 - System.DayOfWeek.Thursday<br />
        /// 										5 - System.DayOfWeek.Friday<br />
        /// 										6 - System.DayOfWeek.Saturday<br />
        /// 										0 - System.DayOfWeek.Sunday<br />
        /// </summary>
        /// <param name="p_e_dayOfWeek">weekday number 1..7</param>
        public void ExcludeWeekday(System.DayOfWeek p_e_dayOfWeek)
        {
            if (!this.a_excludeWeekdays.Contains(p_e_dayOfWeek))
            {
                this.a_excludeWeekdays.Add(p_e_dayOfWeek);
                ForestNET.Lib.Global.ILogConfig("Exclude weekday '" + p_e_dayOfWeek + "'");
            }
        }

        /// <summary>
        ///  new instance of timer task must define a method runTimerTask()
        /// </summary>
        abstract public void RunTimerTask();

        /// <summary>
        /// timer task main run method which will always be executed at the end of an interval
        /// </summary>
        public System.Threading.Tasks.Task Run()
        {
            /* if our RunTimerTask descided to set the stop flag, we will just return null task */
            if (this.Stop)
            {
                return System.Threading.Tasks.Task.FromResult<object?>(null);
            }

            /* get current weekday */
            System.DayOfWeek o_dayOfWeek = System.DateTime.Now.DayOfWeek;

            /* only execute runTimerTask if current weekday is not excluded */
            if (!this.a_excludeWeekdays.Contains(o_dayOfWeek))
            {
                try
                {
                    /* execute runTimerTask */
                    return System.Threading.Tasks.Task.Run(() => this.RunTimerTask());
                }
                catch (Exception o_exc)
                {
                    /* show exception stack trace if flag is set */
                    if (this.ShowException)
                    {
                        ForestNET.Lib.Global.LogException(o_exc);
                    }
                }
            }
            else
            {
                ForestNET.Lib.Global.ILogConfig("Weekday '" + o_dayOfWeek + "' is excluded");
            }

            return System.Threading.Tasks.Task.FromResult<object?>(null);
        }
    }
}
