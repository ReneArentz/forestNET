using Microsoft.Extensions.Logging;

namespace ForestNETLib.Log
{
    /// <summary>
    /// Logger extension methods, like specially adapted log functions and add logger methods for logger factory.
    /// </summary>
    public static class LoggerExtensions
    {

        /* Fields */

        /* Properties */

        /* Methods */

        /// <summary>
        /// Method to add color console provider to logger factory instance
        /// </summary>
        /// <param name="p_o_factory">logger factory instance</param>
        /// <param name="p_o_config">color console configuration instance, used when creating new color console logger(s)</param>
        /// <returns>object of type ILoggerFactory</returns>
        public static ILoggerFactory AddColorConsole(this ILoggerFactory p_o_factory, ColorConsoleLoggerConfiguration p_o_config)
        {
            p_o_factory.AddProvider(new ColorConsoleLoggerProvider(p_o_config));
            return p_o_factory;
        }

        /// <summary>
        /// Method to add file provider to logger factory instance
        /// </summary>
        /// <param name="p_o_factory">logger factory instance</param>
        /// <param name="p_o_config">file configuration instance, used when creating new file logger(s)</param>
        /// <returns>object of type ILoggerFactory</returns>
        public static ILoggerFactory AddFile(this ILoggerFactory p_o_factory, FileLoggerConfiguration p_o_config)
        {
            p_o_factory.AddProvider(new FileLoggerProvider(p_o_config));
            return p_o_factory;
        }

        /// <summary>
        /// Log critical message without using exception object parameter
        /// </summary>
        /// <param name="p_o_logger">logger instance</param>
        /// <param name="p_s_message">log message</param>
        /// <param name="p_s_callerFilePath">log source path or filled by CallerFilePath</param>
        /// <param name="p_s_callerMemberName">log member name or filled by CallerMemberName</param>
        /// <param name="p_i_callerLineNumber">log line number or filled by CallerLineNumber</param>
        public static void LogCritical(this ILogger p_o_logger, string p_s_message, [System.Runtime.CompilerServices.CallerFilePath] string p_s_callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string p_s_callerMemberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int p_i_callerLineNumber = 0)
        {
            LogCritical(p_o_logger, null, p_s_message, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber);
        }

        /// <summary>
        /// Log critical message
        /// </summary>
		/// <param name="p_o_logger">logger instance</param>
		/// <param name="p_o_exception">log exception</param>
		/// <param name="p_s_message">log message</param>
		/// <param name="p_s_callerFilePath">log source path or filled by CallerFilePath</param>
		/// <param name="p_s_callerMemberName">log member name or filled by CallerMemberName</param>
		/// <param name="p_i_callerLineNumber">log line number or filled by CallerLineNumber</param>
		public static void LogCritical(this ILogger p_o_logger, Exception? p_o_exception, string p_s_message = "", [System.Runtime.CompilerServices.CallerFilePath] string p_s_callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string p_s_callerMemberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int p_i_callerLineNumber = 0)
        {
            string s_exceptionMessage = "";

            /* check if exception parameter is not null */
            if (p_o_exception != null)
            {
                /* set exception message variable */
                s_exceptionMessage = p_o_exception.Message;
            }

            /* if exception parameter is not null, but message parameter is null or empty */
            if ((p_o_exception != null) && (ForestNETLib.Core.Helper.IsStringEmpty(p_s_message)))
            {
                /* use exception parameter as message parameter */
                s_exceptionMessage = "";
                p_s_message = p_o_exception.Message;
            }

            /* log message with own format, prefix, delimiter and filepath, member name and line number */
            p_o_logger.LogCritical("fNET§§§{message}§§§{exception}§§§{callerFilePath}§§§{callerMemberName}§§§{callerLineNumber}", p_s_message, s_exceptionMessage, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber);
        }

        /// <summary>
        /// Log error message without using exception object parameter
        /// </summary>
        /// <param name="p_o_logger">logger instance</param>
        /// <param name="p_s_message">log message</param>
        /// <param name="p_s_callerFilePath">log source path or filled by CallerFilePath</param>
        /// <param name="p_s_callerMemberName">log member name or filled by CallerMemberName</param>
        /// <param name="p_i_callerLineNumber">log line number or filled by CallerLineNumber</param>
        public static void LogError(this ILogger p_o_logger, string p_s_message, [System.Runtime.CompilerServices.CallerFilePath] string p_s_callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string p_s_callerMemberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int p_i_callerLineNumber = 0)
        {
            LogError(p_o_logger, null, p_s_message, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber);
        }

        /// <summary>
        /// Log error message
        /// </summary>
        /// <param name="p_o_logger">logger instance</param>
        /// <param name="p_o_exception">log exception</param>
        /// <param name="p_s_message">log message</param>
        /// <param name="p_s_callerFilePath">log source path or filled by CallerFilePath</param>
        /// <param name="p_s_callerMemberName">log member name or filled by CallerMemberName</param>
        /// <param name="p_i_callerLineNumber">log line number or filled by CallerLineNumber</param>
        public static void LogError(this ILogger p_o_logger, Exception? p_o_exception, string p_s_message = "", [System.Runtime.CompilerServices.CallerFilePath] string p_s_callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string p_s_callerMemberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int p_i_callerLineNumber = 0)
        {
            string s_exceptionMessage = "";

            /* check if exception parameter is not null */
            if (p_o_exception != null)
            {
                /* set exception message variable */
                s_exceptionMessage = p_o_exception.Message;
            }

            /* if exception parameter is not null, but message parameter is null or empty */
            if ((p_o_exception != null) && (ForestNETLib.Core.Helper.IsStringEmpty(p_s_message)))
            {
                /* use exception parameter as message parameter */
                s_exceptionMessage = "";
                p_s_message = p_o_exception.Message;
            }

            /* log message with own format, prefix, delimiter and filepath, member name and line number */
            p_o_logger.LogError("fNET§§§{message}§§§{exception}§§§{callerFilePath}§§§{callerMemberName}§§§{callerLineNumber}", p_s_message, s_exceptionMessage, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber);
        }

        /// <summary>
        /// Log warning message without using exception object parameter
        /// </summary>
        /// <param name="p_o_logger">logger instance</param>
        /// <param name="p_s_message">log message</param>
        /// <param name="p_s_callerFilePath">log source path or filled by CallerFilePath</param>
        /// <param name="p_s_callerMemberName">log member name or filled by CallerMemberName</param>
        /// <param name="p_i_callerLineNumber">log line number or filled by CallerLineNumber</param>
        public static void LogWarning(this ILogger p_o_logger, string p_s_message, [System.Runtime.CompilerServices.CallerFilePath] string p_s_callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string p_s_callerMemberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int p_i_callerLineNumber = 0)
        {
            LogWarning(p_o_logger, null, p_s_message, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber);
        }

        /// <summary>
        /// Log warning message
        /// </summary>
        /// <param name="p_o_logger">logger instance</param>
        /// <param name="p_o_exception">log exception</param>
        /// <param name="p_s_message">log message</param>
        /// <param name="p_s_callerFilePath">log source path or filled by CallerFilePath</param>
        /// <param name="p_s_callerMemberName">log member name or filled by CallerMemberName</param>
        /// <param name="p_i_callerLineNumber">log line number or filled by CallerLineNumber</param>
        public static void LogWarning(this ILogger p_o_logger, Exception? p_o_exception, string p_s_message = "", [System.Runtime.CompilerServices.CallerFilePath] string p_s_callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string p_s_callerMemberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int p_i_callerLineNumber = 0)
        {
            string s_exceptionMessage = "";

            /* check if exception parameter is not null */
            if (p_o_exception != null)
            {
                /* set exception message variable */
                s_exceptionMessage = p_o_exception.Message;
            }

            /* if exception parameter is not null, but message parameter is null or empty */
            if ((p_o_exception != null) && (ForestNETLib.Core.Helper.IsStringEmpty(p_s_message)))
            {
                /* use exception parameter as message parameter */
                s_exceptionMessage = "";
                p_s_message = p_o_exception.Message;
            }

            /* log message with own format, prefix, delimiter and filepath, member name and line number */
            p_o_logger.LogWarning("fNET§§§{message}§§§{exception}§§§{callerFilePath}§§§{callerMemberName}§§§{callerLineNumber}", p_s_message, s_exceptionMessage, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber);
        }

        /// <summary>
        /// Log information message without using exception object parameter
        /// </summary>
        /// <param name="p_o_logger">logger instance</param>
        /// <param name="p_s_message">log message</param>
        /// <param name="p_s_callerFilePath">log source path or filled by CallerFilePath</param>
        /// <param name="p_s_callerMemberName">log member name or filled by CallerMemberName</param>
        /// <param name="p_i_callerLineNumber">log line number or filled by CallerLineNumber</param>
        public static void LogInformation(this ILogger p_o_logger, string p_s_message, [System.Runtime.CompilerServices.CallerFilePath] string p_s_callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string p_s_callerMemberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int p_i_callerLineNumber = 0)
        {
            LogInformation(p_o_logger, null, p_s_message, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber);
        }

        /// <summary>
        /// Log information message
        /// </summary>
        /// <param name="p_o_logger">logger instance</param>
        /// <param name="p_o_exception">log exception</param>
        /// <param name="p_s_message">log message</param>
        /// <param name="p_s_callerFilePath">log source path or filled by CallerFilePath</param>
        /// <param name="p_s_callerMemberName">log member name or filled by CallerMemberName</param>
        /// <param name="p_i_callerLineNumber">log line number or filled by CallerLineNumber</param>
        public static void LogInformation(this ILogger p_o_logger, Exception? p_o_exception, string p_s_message = "", [System.Runtime.CompilerServices.CallerFilePath] string p_s_callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string p_s_callerMemberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int p_i_callerLineNumber = 0)
        {
            string s_exceptionMessage = "";

            /* check if exception parameter is not null */
            if (p_o_exception != null)
            {
                /* set exception message variable */
                s_exceptionMessage = p_o_exception.Message;
            }

            /* if exception parameter is not null, but message parameter is null or empty */
            if ((p_o_exception != null) && (ForestNETLib.Core.Helper.IsStringEmpty(p_s_message)))
            {
                /* use exception parameter as message parameter */
                s_exceptionMessage = "";
                p_s_message = p_o_exception.Message;
            }

            /* log message with own format, prefix, delimiter and filepath, member name and line number */
            p_o_logger.LogInformation("fNET§§§{message}§§§{exception}§§§{callerFilePath}§§§{callerMemberName}§§§{callerLineNumber}", p_s_message, s_exceptionMessage, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber);
        }

        /// <summary>
        /// Log debug message without using exception object parameter
        /// </summary>
        /// <param name="p_o_logger">logger instance</param>
        /// <param name="p_s_message">log message</param>
        /// <param name="p_s_callerFilePath">log source path or filled by CallerFilePath</param>
        /// <param name="p_s_callerMemberName">log member name or filled by CallerMemberName</param>
        /// <param name="p_i_callerLineNumber">log line number or filled by CallerLineNumber</param>
        public static void LogDebug(this ILogger p_o_logger, string p_s_message, [System.Runtime.CompilerServices.CallerFilePath] string p_s_callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string p_s_callerMemberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int p_i_callerLineNumber = 0)
        {
            LogDebug(p_o_logger, null, p_s_message, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber);
        }

        /// <summary>
        /// Log debug message
        /// </summary>
        /// <param name="p_o_logger">logger instance</param>
        /// <param name="p_o_exception">log exception</param>
        /// <param name="p_s_message">log message</param>
        /// <param name="p_s_callerFilePath">log source path or filled by CallerFilePath</param>
        /// <param name="p_s_callerMemberName">log member name or filled by CallerMemberName</param>
        /// <param name="p_i_callerLineNumber">log line number or filled by CallerLineNumber</param>
        public static void LogDebug(this ILogger p_o_logger, Exception? p_o_exception, string p_s_message = "", [System.Runtime.CompilerServices.CallerFilePath] string p_s_callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string p_s_callerMemberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int p_i_callerLineNumber = 0)
        {
            string s_exceptionMessage = "";

            /* check if exception parameter is not null */
            if (p_o_exception != null)
            {
                /* set exception message variable */
                s_exceptionMessage = p_o_exception.Message;
            }

            /* if exception parameter is not null, but message parameter is null or empty */
            if ((p_o_exception != null) && (ForestNETLib.Core.Helper.IsStringEmpty(p_s_message)))
            {
                /* use exception parameter as message parameter */
                s_exceptionMessage = "";
                p_s_message = p_o_exception.Message;
            }

            /* log message with own format, prefix, delimiter and filepath, member name and line number */
            p_o_logger.LogDebug("fNET§§§{message}§§§{exception}§§§{callerFilePath}§§§{callerMemberName}§§§{callerLineNumber}", p_s_message, s_exceptionMessage, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber);
        }

        /// <summary>
        /// Log trace message without using exception object parameter
        /// </summary>
        /// <param name="p_o_logger">logger instance</param>
        /// <param name="p_s_message">log message</param>
        /// <param name="p_s_callerFilePath">log source path or filled by CallerFilePath</param>
        /// <param name="p_s_callerMemberName">log member name or filled by CallerMemberName</param>
        /// <param name="p_i_callerLineNumber">log line number or filled by CallerLineNumber</param>
        public static void LogTrace(this ILogger p_o_logger, string p_s_message, [System.Runtime.CompilerServices.CallerFilePath] string p_s_callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string p_s_callerMemberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int p_i_callerLineNumber = 0)
        {
            LogTrace(p_o_logger, null, p_s_message, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber);
        }

        /// <summary>
        /// Log trace message
        /// </summary>
        /// <param name="p_o_logger">logger instance</param>
        /// <param name="p_o_exception">log exception</param>
        /// <param name="p_s_message">log message</param>
        /// <param name="p_s_callerFilePath">log source path or filled by CallerFilePath</param>
        /// <param name="p_s_callerMemberName">log member name or filled by CallerMemberName</param>
        /// <param name="p_i_callerLineNumber">log line number or filled by CallerLineNumber</param>
        public static void LogTrace(this ILogger p_o_logger, Exception? p_o_exception, string p_s_message = "", [System.Runtime.CompilerServices.CallerFilePath] string p_s_callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string p_s_callerMemberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int p_i_callerLineNumber = 0)
        {
            string s_exceptionMessage = "";

            /* check if exception parameter is not null */
            if (p_o_exception != null)
            {
                /* set exception message variable */
                s_exceptionMessage = p_o_exception.Message;
            }

            /* if exception parameter is not null, but message parameter is null or empty */
            if ((p_o_exception != null) && (ForestNETLib.Core.Helper.IsStringEmpty(p_s_message)))
            {
                /* use exception parameter as message parameter */
                s_exceptionMessage = "";
                p_s_message = p_o_exception.Message;
            }

            /* log message with own format, prefix, delimiter and filepath, member name and line number */
            p_o_logger.LogTrace("fNET§§§{message}§§§{exception}§§§{callerFilePath}§§§{callerMemberName}§§§{callerLineNumber}", p_s_message, s_exceptionMessage, p_s_callerFilePath, p_s_callerMemberName, p_i_callerLineNumber);
        }
    }
}
