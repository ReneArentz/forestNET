namespace ForestNETLib.Core
{
    /// <summary>
    /// Timer class with timer task object to execute repeating code within a time interval.
    /// </summary>
    public class Timer
    {

        /* Fields */

        private System.Timers.Timer? o_timer;
        private readonly TimerTask o_timerTask;
        private System.Threading.CancellationTokenSource? o_token;

        /* Properties */

        /* Methods */

        /// <summary>
        /// Create a timer instance, only with a timer task object
        /// </summary>
        /// <param name="p_o_timerTask">timer task which holds the real content which will be executed after the end of an interval</param>
        public Timer(TimerTask p_o_timerTask)
        {
            this.o_timerTask = p_o_timerTask;
        }

        /// <summary>
        /// Start timer instance with timer task object and defined interval and optional start time value
        /// </summary>
        public async void StartTimer()
        {
            if (this.o_timerTask.StartTime != System.TimeSpan.Zero)
            {
                /* get current time */
                System.TimeSpan o_localTime = System.DateTime.Now.TimeOfDay;

                ForestNETLib.Core.Global.ILogConfig("time now: '" + o_localTime + "'");
                ForestNETLib.Core.Global.ILogConfig("start delay calculation with start time: '" + this.o_timerTask.StartTime + "'");

                /* start delay calculation, so we subtract current time from set start time */
                int i_startDelay = Convert.ToInt32(this.o_timerTask.StartTime.TotalMilliseconds - o_localTime.TotalMilliseconds);
                ForestNETLib.Core.Global.ILogConfig("calculated start delay: '" + i_startDelay + " ms'");

                /* if calculated start delay is negative, it will be set for next day */
                if (i_startDelay < 0)
                {
                    i_startDelay += 24 * 60 * 60 * 1_000;
                    ForestNETLib.Core.Global.ILogConfig("start delay is negative, it will be set for next day: '" + System.DateTime.Now.AddSeconds(i_startDelay / 1_000) + "'");
                }

                /* prepare cancellation token */
                this.o_token = new System.Threading.CancellationTokenSource();
                this.o_token.Token.ThrowIfCancellationRequested();

                try
                {
                    /* wait start delay */
                    await System.Threading.Tasks.Task.Delay(i_startDelay, this.o_token.Token);
                }
                catch (Exception) /* catch excption if task delay is cancelled by token */
                {
                    /* nothing to do */
                }
                finally
                {
                    /* release token */
                    this.o_token.Dispose();
                    this.o_token = null;
                }
            }

            /* create new timer object */
            this.o_timer = new System.Timers.Timer();

            /* start timer with timer task object, optional start delay and interval duration */
            this.o_timer.Elapsed += async (p_o_sender, p_o_elapsedEventArgs) => await this.o_timerTask.Run();
            this.o_timer.Interval = this.o_timerTask.Interval.ToDuration();
            this.o_timer.AutoReset = this.o_timerTask.Repeat;
            this.o_timer.Start();
            ForestNETLib.Core.Global.ILogFinest("timer started");
        }

        /// <summary>
        /// Stops timer instance
        /// </summary>
        public void StopTimer()
        {
            /* cancel delay if active */
            this.o_token?.Cancel();

            /* stop timer instance */
            if (this.o_timer != null)
            {
                this.o_timer.Stop();
                this.o_timer.Dispose();
                ForestNETLib.Core.Global.ILogFinest("timer stopped");
            }
        }
    }
}
