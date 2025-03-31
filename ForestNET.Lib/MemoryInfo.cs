using ForestNET.Lib.Log;

namespace ForestNET.Lib
{
    /// <summary>
    /// MemoryInfo class to log memory info of current programm.
    /// </summary>
    public class MemoryInfo : Runnable
    {

        /* Fields */

        private readonly Microsoft.Extensions.Logging.ILogger o_logger;
        private readonly int i_intervalMilliseconds;
        private readonly Level e_level;
        private readonly bool b_memoryChanged;
        private long l_prevUsed;
        private long l_maxUsed;

        /* Properties */

        /* Methods */

        /// <summary>
        /// Creates a runnable thread instance with Run-method to log memory info of current process
        /// </summary>
        /// <param name="p_o_logger">logger object where the memory information will be logged</param>
        /// <param name="p_i_intervalMilliseconds">interval how often the memory information should be added to log</param>
        /// <param name="p_e_level">on which log level the memory information will be added to log</param>
        /// <exception cref="ArgumentException">will tell invalid value for interval milliseconds</exception>
        public MemoryInfo(Microsoft.Extensions.Logging.ILogger p_o_logger, int p_i_intervalMilliseconds, Level p_e_level) : this(p_o_logger, p_i_intervalMilliseconds, p_e_level, false)
        {

        }

        /// <summary>
        /// Creates a runnable thread instance with Run-method to log memory info of current process
        /// </summary>
        /// <param name="p_o_logger">logger object where the memory information will be logged</param>
        /// <param name="p_i_intervalMilliseconds">interval how often the memory information should be added to log</param>
        /// <param name="p_e_level">on which log level the memory information will be added to log</param>
        /// <param name="p_b_memoryChanged">only add memory information to log if used memory has changed between current and previous iteration</param>
        /// <exception cref="ArgumentException">will tell invalid value for interval milliseconds</exception>
        public MemoryInfo(Microsoft.Extensions.Logging.ILogger p_o_logger, int p_i_intervalMilliseconds, Level p_e_level, bool p_b_memoryChanged)
        {
            /* check parameter value */
            if (p_i_intervalMilliseconds < 1)
            {
                throw new ArgumentException("Interval must be at least '1' millisecond, but was set to '" + p_i_intervalMilliseconds + "' millisecond(s)");
            }

            /* assume parameters to values and set defaults */
            this.o_logger = p_o_logger;
            this.i_intervalMilliseconds = p_i_intervalMilliseconds;
            this.b_memoryChanged = p_b_memoryChanged;
            this.e_level = p_e_level;
            this.l_prevUsed = 0;
            this.l_maxUsed = 0;
        }

        /// <summary>
        /// thread run-method
        /// </summary>
        override public void Run()
        {
            try
            {
                /* endless loop until stop-flag is set */
                while (!this.Stop)
                {
                    /* sleep for defined interval in milliseconds */
                    System.Threading.Thread.Sleep(this.i_intervalMilliseconds);

                    /* get memory info from process diagnostic info instance */
                    long l_used = System.Diagnostics.Process.GetCurrentProcess().PrivateMemorySize64;

                    /* update used memory info if we reached a new peak */
                    if (l_used > this.l_maxUsed)
                    {
                        this.l_maxUsed = l_used;
                    }

                    /* if used memory info have not changed, we will jump to start of endless loop */
                    if (l_used == this.l_prevUsed)
                    {
                        if (this.b_memoryChanged)
                        {
                            continue;
                        }
                    }

                    /* prepare log message */
                    string s_logMessage = "Memory-Info - Used: " + Helper.FormatBytes(l_used) + ", dUsed: " + Helper.FormatBytes(l_used - this.l_prevUsed);

                    /* add memory information to log */
                    switch (LevelExtensions.LevelToNETLogLevel(this.e_level))
                    {
                        case Microsoft.Extensions.Logging.LogLevel.Critical:
                            this.o_logger.LogCritical(s_logMessage);
                            break;
                        case Microsoft.Extensions.Logging.LogLevel.Error:
                            this.o_logger.LogError(s_logMessage);
                            break;
                        case Microsoft.Extensions.Logging.LogLevel.Warning:
                            this.o_logger.LogWarning(s_logMessage);
                            break;
                        case Microsoft.Extensions.Logging.LogLevel.Information:
                            this.o_logger.LogInformation(s_logMessage);
                            break;
                        case Microsoft.Extensions.Logging.LogLevel.Debug:
                            this.o_logger.LogDebug(s_logMessage);
                            break;
                        case Microsoft.Extensions.Logging.LogLevel.Trace:
                            this.o_logger.LogTrace(s_logMessage);
                            break;
                        default:
                            this.o_logger.LogInformation(s_logMessage);
                            break;
                    }

                    /* update previous used memory info */
                    this.l_prevUsed = l_used;
                }
            }
            catch (Exception o_exc)
            {
                /* log exception */
                Global.LogException(o_exc);
            }
            finally
            {
                /* prepare log message */
                string s_logMessage = "Memory-Info - max. Used: " + Helper.FormatBytes(this.l_maxUsed);

                /* add memory information to log */
                switch (LevelExtensions.LevelToNETLogLevel(this.e_level))
                {
                    case Microsoft.Extensions.Logging.LogLevel.Critical:
                        this.o_logger.LogCritical(s_logMessage);
                        break;
                    case Microsoft.Extensions.Logging.LogLevel.Error:
                        this.o_logger.LogError(s_logMessage);
                        break;
                    case Microsoft.Extensions.Logging.LogLevel.Warning:
                        this.o_logger.LogWarning(s_logMessage);
                        break;
                    case Microsoft.Extensions.Logging.LogLevel.Information:
                        this.o_logger.LogInformation(s_logMessage);
                        break;
                    case Microsoft.Extensions.Logging.LogLevel.Debug:
                        this.o_logger.LogDebug(s_logMessage);
                        break;
                    case Microsoft.Extensions.Logging.LogLevel.Trace:
                        this.o_logger.LogTrace(s_logMessage);
                        break;
                    default:
                        this.o_logger.LogInformation(s_logMessage);
                        break;
                }
            }
        }
    }
}
