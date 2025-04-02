namespace ForestNET.Tests.Log
{
    public class LogConfigUnitTest
    {
        [Test]
        public void TestLogConfig()
        {
            try
            {
                string s_currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'");
                string s_testDirectory = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "testLogConfig" + ForestNET.Lib.IO.File.DIR;
                string s_tempDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + ForestNET.Lib.IO.File.DIR + "temp";
                string s_testTempDirectory = s_tempDirectory + ForestNET.Lib.IO.File.DIR + "testLogConfig" + ForestNET.Lib.IO.File.DIR;

                if (ForestNET.Lib.IO.File.FolderExists(s_testDirectory))
                {
                    ForestNET.Lib.IO.File.DeleteDirectory(s_testDirectory);
                }

                if (ForestNET.Lib.IO.File.FolderExists(s_testTempDirectory))
                {
                    ForestNET.Lib.IO.File.DeleteDirectory(s_testTempDirectory);
                }

                ForestNET.Lib.IO.File.CreateDirectory(s_testDirectory);
                Assert.That(
                    ForestNET.Lib.IO.File.FolderExists(s_testDirectory),
                    Is.True,
                    "directory[" + s_testDirectory + "] does not exist"
                );

                ForestNET.Lib.IO.File.CreateDirectory(s_testTempDirectory);
                Assert.That(
                    ForestNET.Lib.IO.File.FolderExists(s_testTempDirectory),
                    Is.True,
                    "directory[" + s_testTempDirectory + "] does not exist"
                );

                /* ##################################################################################################### */

                if (ForestNET.Lib.IO.File.Exists(s_testDirectory + "testLogConfigAll.log.01"))
                {
                    ForestNET.Lib.IO.File.DeleteFile(s_testDirectory + "testLogConfigAll.log.01");
                }

                if (ForestNET.Lib.IO.File.Exists(s_testDirectory + "testLogConfig.log.01"))
                {
                    ForestNET.Lib.IO.File.DeleteFile(s_testDirectory + "testLogConfig.log.01");
                }

                if (ForestNET.Lib.IO.File.Exists(s_testDirectory + "testLogConfigInternal.log.01"))
                {
                    ForestNET.Lib.IO.File.DeleteFile(s_testDirectory + "testLogConfigInternal.log.01");
                }

                string s_otherLogFile = s_testTempDirectory + "testLogConfigOtherImplementation.log";

                if (ForestNET.Lib.IO.File.Exists(s_otherLogFile))
                {
                    ForestNET.Lib.IO.File.DeleteFile(s_otherLogFile);
                }

                /* ##################################################################################################### */

                System.Collections.Generic.List<string> a_configLines =
                [
                    "MinimumLevel = FINEST",
                    "UseConsole = false",
                    "ConsoleLoggingFilter = false; FINEST",
                    "ConsoleLoggingFilter = true; FINEST",
                    "FileLoggingFilter = false; FINEST",
                    "FileLoggingFilter = true; FINEST",
                    "UseFileLogging = true",
                    "FileLoggerConfigurationElement = Log,InternalLog; " + s_testDirectory + ";testLogConfigAll.log.%n1; 25; 1000000"
                ];

                ForestNET.Lib.Global.Instance.ResetLogFromLines(a_configLines);

                ForestNET.Lib.Global.Instance.LogControl = (byte)ForestNET.Lib.Log.Level.ALL; /* all levels */
                ForestNET.Lib.Global.Instance.InternalLogControl = ForestNET.Lib.Global.Instance.LogControl;

                ForestNET.Lib.Global.LogSevere("logSevere");
                ForestNET.Lib.Global.LogWarning("logWarning");
                ForestNET.Lib.Global.Log("log");
                ForestNET.Lib.Global.LogConfig("logConfig");
                ForestNET.Lib.Global.LogFine("logFine");
                ForestNET.Lib.Global.LogFiner("logFiner");
                ForestNET.Lib.Global.LogFinest("logFinest");

                ForestNET.Lib.Global.ILogSevere("ilogSevere");
                ForestNET.Lib.Global.ILogWarning("ilogWarning");
                ForestNET.Lib.Global.ILog("ilog");
                ForestNET.Lib.Global.ILogConfig("ilogConfig");
                ForestNET.Lib.Global.ILogFine("ilogFine");
                ForestNET.Lib.Global.ILogFiner("ilogFiner");
                ForestNET.Lib.Global.ILogFinest("ilogFinest");

                ForestNET.Lib.Global.Instance.ResetLog();

                ForestNET.Lib.IO.File o_file = new(s_testDirectory + "testLogConfigAll.log.01", false);
                System.Collections.Generic.List<string> a_lines = o_file.FileContentAsList ?? throw new Exception("config 'testLogConfigAll.log.01' file has no content");

                for (int i = 0; i < a_lines.Count; i++)
                {
                    if (i == 0)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("logSevere"),
                            "#1 line != 'logSevere'"
                        );
                    }
                    else if (i == 1)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("logWarning"),
                            "#2 line != 'logWarning'"
                        );
                    }
                    else if (i == 2)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("log"),
                            "#3 line != 'log'"
                        );
                    }
                    else if (i == 3)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("logConfig"),
                            "#4 line != 'logConfig'"
                        );
                    }
                    else if (i == 4)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("logFine"),
                            "#5 line != 'logFine'"
                        );
                    }
                    else if (i == 5)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("logFiner"),
                            "#6 line != 'logFiner'"
                        );
                    }
                    else if (i == 6)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("logFinest"),
                            "#7 line != 'logFinest'"
                        );
                    }
                    else if (i == 7)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("ilogSevere"),
                            "#8 line != 'ilogSevere'"
                        );
                    }
                    else if (i == 8)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("ilogWarning"),
                            "#9 line != 'ilogWarning'"
                        );
                    }
                    else if (i == 9)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("ilog"),
                            "#10 line != 'ilog'"
                        );
                    }
                    else if (i == 10)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("ilogConfig"),
                            "#11 line != 'ilogConfig'"
                        );
                    }
                    else if (i == 11)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("ilogFine"),
                            "#12 line != 'ilogFine'"
                        );
                    }
                    else if (i == 12)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("ilogFiner"),
                            "#13 line != 'ilogFiner'"
                        );
                    }
                    else if (i == 13)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("ilogFinest"),
                            "#14 line != 'ilogFinest'"
                        );
                    }
                }

                ForestNET.Lib.IO.File.DeleteFile(s_testDirectory + "testLogConfigAll.log.01");

                /* ##################################################################################################### */

                a_configLines =
                [
                    "MinimumLevel = FINEST",
                    "UseConsole = false",
                    "ConsoleLoggingFilter = false; FINEST",
                    "ConsoleLoggingFilter = true; FINEST",
                    "FileLoggingFilter = false; FINEST",
                    "FileLoggingFilter = true; FINEST",
                    "UseFileLogging = true",
                    "FileLoggerConfigurationElement = Log; " + s_testDirectory + ";testLogConfig.log.%n1; 25; 1000000",
                    "FileLoggerConfigurationElement = InternalLog; " + s_testDirectory + ";testLogConfigInternal.log.%n1; 25; 1000000"
                ];

#pragma warning disable IDE0039 // Use local function
                ForestNET.Lib.Global.OtherLogImplementation del_otherLogImplementation =
                    (bool p_b_internalLog, byte p_by_logLevel, string p_s_callerFilePath, string p_s_callerMemberName, int p_i_callerLineNumber, string p_s_logMessage) =>
                    {
                        try
                        {
                            if (p_by_logLevel > 0x00)
                            {
                                ForestNET.Lib.IO.File o_otherLogFile = new(s_otherLogFile, !ForestNET.Lib.IO.File.Exists(s_otherLogFile));
                                o_otherLogFile.AppendLine("[" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture) + "]" + " " + ((p_b_internalLog) ? "[Internal]" : "[Normal]") + " " + "[" + ForestNET.Lib.Log.LevelExtensions.LevelToString((ForestNET.Lib.Log.Level)p_by_logLevel) + "]" + " " + "[" + p_s_callerFilePath.Substring(p_s_callerFilePath.LastIndexOf(ForestNET.Lib.IO.File.DIR) + 1) + "]" + " " + "[" + p_s_callerMemberName + ":" + p_i_callerLineNumber + "]" + "   " + p_s_logMessage);
                            }
                        }
                        catch (Exception o_exc)
                        {
                            Assert.Fail(o_exc.ToString());
                        }
                    }
                ;
#pragma warning restore IDE0039 // Use local function

                ForestNET.Lib.Global.Instance.DelegateLogImplementation = del_otherLogImplementation;
                ForestNET.Lib.Global.Instance.ResetLogFromLines(a_configLines);

                ForestNET.Lib.Global.Instance.LogControl = (byte)ForestNET.Lib.Log.Level.ALL; /* all levels */
                ForestNET.Lib.Global.Instance.InternalLogControl = (byte)ForestNET.Lib.Log.Level.SEVERE + (byte)ForestNET.Lib.Log.Level.INFO + (byte)ForestNET.Lib.Log.Level.FINE + (byte)ForestNET.Lib.Log.Level.FINER; /* only SEVERE, INFO, FINE, FINER */

                ForestNET.Lib.Global.LogSevere("logSevere");
                ForestNET.Lib.Global.ILogSevere("ilogSevere");
                ForestNET.Lib.Global.LogWarning("logWarning");
                ForestNET.Lib.Global.ILogWarning("ilogWarning");
                ForestNET.Lib.Global.Log("log");
                ForestNET.Lib.Global.ILog("ilog");
                ForestNET.Lib.Global.LogConfig("logConfig");
                ForestNET.Lib.Global.ILogConfig("ilogConfig");
                ForestNET.Lib.Global.LogFine("logFine");
                ForestNET.Lib.Global.ILogFine("ilogFine");
                ForestNET.Lib.Global.LogFiner("logFiner");
                ForestNET.Lib.Global.ILogFiner("ilogFiner");
                ForestNET.Lib.Global.LogFinest("logFinest");
                ForestNET.Lib.Global.ILogFinest("ilogFinest");

                ForestNET.Lib.Global.Instance.ResetLog();

                o_file = new ForestNET.Lib.IO.File(s_testDirectory + "testLogConfig.log.01", false);
                a_lines = o_file.FileContentAsList ?? throw new Exception("config 'testLogConfig.log.01' file has no content");

                for (int i = 0; i < a_lines.Count; i++)
                {
                    if (i == 0)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("logSevere"),
                            "#1 line != 'logSevere'"
                        );
                    }
                    else if (i == 1)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("logWarning"),
                            "#2 line != 'logWarning'"
                        );
                    }
                    else if (i == 2)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("log"),
                            "#3 line != 'log'"
                        );
                    }
                    else if (i == 3)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("logConfig"),
                            "#4 line != 'logConfig'"
                        );
                    }
                    else if (i == 4)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("logFine"),
                            "#5 line != 'logFine'"
                        );
                    }
                    else if (i == 5)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("logFiner"),
                            "#6 line != 'logFiner'"
                        );
                    }
                    else if (i == 6)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("logFinest"),
                            "#7 line != 'logFinest'"
                        );
                    }
                }

                ForestNET.Lib.IO.File.DeleteFile(s_testDirectory + "testLogConfig.log.01");

                o_file = new ForestNET.Lib.IO.File(s_testDirectory + "testLogConfigInternal.log.01", false);
                a_lines = o_file.FileContentAsList ?? throw new Exception("config 'testLogConfigInternal.log.01' file has no content");

                for (int i = 0; i < a_lines.Count; i++)
                {
                    if (i == 0)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("ilogSevere"),
                            "#1 line != 'ilogSevere'"
                        );
                    }
                    else if (i == 1)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("ilog"),
                            "#2 line != 'ilog'"
                        );
                    }
                    else if (i == 2)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("ilogFine"),
                            "#3 line != 'ilogFine'"
                        );
                    }
                    else if (i == 3)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("ilogFiner"),
                            "#4 line != 'ilogFiner'"
                        );
                    }
                }

                ForestNET.Lib.IO.File.DeleteFile(s_testDirectory + "testLogConfigInternal.log.01");

                o_file = new ForestNET.Lib.IO.File(s_testTempDirectory + "testLogConfigOtherImplementation.log", false);
                a_lines = o_file.FileContentAsList ?? throw new Exception("config 'testLogConfigOtherImplementation.log' file has no content");

                for (int i = 0; i < a_lines.Count; i++)
                {
                    if (i == 0)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("logSevere"),
                            "#1 line != 'logSevere'"
                        );
                    }
                    else if (i == 1)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("ilogSevere"),
                            "#2 line != 'ilogSevere'"
                        );
                    }
                    else if (i == 2)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("logWarning"),
                            "#3 line != 'logWarning'"
                        );
                    }
                    else if (i == 3)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("log"),
                            "#4 line != 'log'"
                        );
                    }
                    else if (i == 4)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("ilog"),
                            "#5 line != 'ilog'"
                        );
                    }
                    else if (i == 5)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("logConfig"),
                            "#6 line != 'logConfig'"
                        );
                    }
                    else if (i == 6)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("logFine"),
                            "#7 line != 'logFine'"
                        );
                    }
                    else if (i == 7)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("ilogFine"),
                            "#7 line != 'ilogFine'"
                        );
                    }
                    else if (i == 8)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("logFiner"),
                            "#7 line != 'logFiner'"
                        );
                    }
                    else if (i == 9)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("ilogFiner"),
                            "#7 line != 'ilogFiner'"
                        );
                    }
                    else if (i == 10)
                    {
                        Assert.That(
                            a_lines[i], Does.Contain("logFinest"),
                            "#7 line != 'logFinest'"
                        );
                    }
                }

                ForestNET.Lib.IO.File.DeleteFile(s_testTempDirectory + "testLogConfigOtherImplementation.log");

                /* ##################################################################################################### */

                ForestNET.Lib.IO.File.DeleteDirectory(s_testDirectory);
                Assert.That(
                    ForestNET.Lib.IO.File.FolderExists(s_testDirectory),
                    Is.False,
                    "directory[" + s_testDirectory + "] does exist"
                );

                ForestNET.Lib.IO.File.DeleteDirectory(s_testTempDirectory);
                Assert.That(
                    ForestNET.Lib.IO.File.FolderExists(s_testTempDirectory),
                    Is.False,
                    "directory[" + s_testTempDirectory + "] does exist"
                );

                ForestNET.Lib.Global.Instance.ResetLogToStandard();
            }
            catch (Exception o_exc)
            {
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
        }
    }
}
