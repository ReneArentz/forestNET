namespace ForestNETTests.Log
{
    public class LogConfigUnitTest
    {
        [Test]
        public void TestLogConfig()
        {
            try
            {
                string s_currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'");
                string s_testDirectory = s_currentDirectory + ForestNETLib.IO.File.DIR + "testLogConfig" + ForestNETLib.IO.File.DIR;
                string s_tempDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + ForestNETLib.IO.File.DIR + "temp";
                string s_testTempDirectory = s_tempDirectory + ForestNETLib.IO.File.DIR + "testLogConfig" + ForestNETLib.IO.File.DIR;

                if (ForestNETLib.IO.File.FolderExists(s_testDirectory))
                {
                    ForestNETLib.IO.File.DeleteDirectory(s_testDirectory);
                }

                if (ForestNETLib.IO.File.FolderExists(s_testTempDirectory))
                {
                    ForestNETLib.IO.File.DeleteDirectory(s_testTempDirectory);
                }

                ForestNETLib.IO.File.CreateDirectory(s_testDirectory);
                Assert.That(
                    ForestNETLib.IO.File.FolderExists(s_testDirectory),
                    Is.True,
                    "directory[" + s_testDirectory + "] does not exist"
                );

                ForestNETLib.IO.File.CreateDirectory(s_testTempDirectory);
                Assert.That(
                    ForestNETLib.IO.File.FolderExists(s_testTempDirectory),
                    Is.True,
                    "directory[" + s_testTempDirectory + "] does not exist"
                );

                /* ##################################################################################################### */

                if (ForestNETLib.IO.File.Exists(s_testDirectory + "testLogConfigAll.log.01"))
                {
                    ForestNETLib.IO.File.DeleteFile(s_testDirectory + "testLogConfigAll.log.01");
                }

                if (ForestNETLib.IO.File.Exists(s_testDirectory + "testLogConfig.log.01"))
                {
                    ForestNETLib.IO.File.DeleteFile(s_testDirectory + "testLogConfig.log.01");
                }

                if (ForestNETLib.IO.File.Exists(s_testDirectory + "testLogConfigInternal.log.01"))
                {
                    ForestNETLib.IO.File.DeleteFile(s_testDirectory + "testLogConfigInternal.log.01");
                }

                string s_otherLogFile = s_testTempDirectory + "testLogConfigOtherImplementation.log";

                if (ForestNETLib.IO.File.Exists(s_otherLogFile))
                {
                    ForestNETLib.IO.File.DeleteFile(s_otherLogFile);
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

                ForestNETLib.Core.Global.Instance.ResetLogFromLines(a_configLines);

                ForestNETLib.Core.Global.Instance.LogControl = (byte)ForestNETLib.Log.Level.ALL; /* all levels */
                ForestNETLib.Core.Global.Instance.InternalLogControl = ForestNETLib.Core.Global.Instance.LogControl;

                ForestNETLib.Core.Global.LogSevere("logSevere");
                ForestNETLib.Core.Global.LogWarning("logWarning");
                ForestNETLib.Core.Global.Log("log");
                ForestNETLib.Core.Global.LogConfig("logConfig");
                ForestNETLib.Core.Global.LogFine("logFine");
                ForestNETLib.Core.Global.LogFiner("logFiner");
                ForestNETLib.Core.Global.LogFinest("logFinest");

                ForestNETLib.Core.Global.ILogSevere("ilogSevere");
                ForestNETLib.Core.Global.ILogWarning("ilogWarning");
                ForestNETLib.Core.Global.ILog("ilog");
                ForestNETLib.Core.Global.ILogConfig("ilogConfig");
                ForestNETLib.Core.Global.ILogFine("ilogFine");
                ForestNETLib.Core.Global.ILogFiner("ilogFiner");
                ForestNETLib.Core.Global.ILogFinest("ilogFinest");

                ForestNETLib.Core.Global.Instance.ResetLog();

                ForestNETLib.IO.File o_file = new(s_testDirectory + "testLogConfigAll.log.01", false);
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

                ForestNETLib.IO.File.DeleteFile(s_testDirectory + "testLogConfigAll.log.01");

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
                ForestNETLib.Core.Global.OtherLogImplementation del_otherLogImplementation =
                    (bool p_b_internalLog, byte p_by_logLevel, string p_s_callerFilePath, string p_s_callerMemberName, int p_i_callerLineNumber, string p_s_logMessage) =>
                    {
                        try
                        {
                            if (p_by_logLevel > 0x00)
                            {
                                ForestNETLib.IO.File o_otherLogFile = new(s_otherLogFile, !ForestNETLib.IO.File.Exists(s_otherLogFile));
                                o_otherLogFile.AppendLine("[" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture) + "]" + " " + ((p_b_internalLog) ? "[Internal]" : "[Normal]") + " " + "[" + ForestNETLib.Log.LevelExtensions.LevelToString((ForestNETLib.Log.Level)p_by_logLevel) + "]" + " " + "[" + p_s_callerFilePath.Substring(p_s_callerFilePath.LastIndexOf(ForestNETLib.IO.File.DIR) + 1) + "]" + " " + "[" + p_s_callerMemberName + ":" + p_i_callerLineNumber + "]" + "   " + p_s_logMessage);
                            }
                        }
                        catch (Exception o_exc)
                        {
                            Assert.Fail(o_exc.ToString());
                        }
                    }
                ;
#pragma warning restore IDE0039 // Use local function

                ForestNETLib.Core.Global.Instance.DelegateLogImplementation = del_otherLogImplementation;
                ForestNETLib.Core.Global.Instance.ResetLogFromLines(a_configLines);

                ForestNETLib.Core.Global.Instance.LogControl = (byte)ForestNETLib.Log.Level.ALL; /* all levels */
                ForestNETLib.Core.Global.Instance.InternalLogControl = (byte)ForestNETLib.Log.Level.SEVERE + (byte)ForestNETLib.Log.Level.INFO + (byte)ForestNETLib.Log.Level.FINE + (byte)ForestNETLib.Log.Level.FINER; /* only SEVERE, INFO, FINE, FINER */

                ForestNETLib.Core.Global.LogSevere("logSevere");
                ForestNETLib.Core.Global.ILogSevere("ilogSevere");
                ForestNETLib.Core.Global.LogWarning("logWarning");
                ForestNETLib.Core.Global.ILogWarning("ilogWarning");
                ForestNETLib.Core.Global.Log("log");
                ForestNETLib.Core.Global.ILog("ilog");
                ForestNETLib.Core.Global.LogConfig("logConfig");
                ForestNETLib.Core.Global.ILogConfig("ilogConfig");
                ForestNETLib.Core.Global.LogFine("logFine");
                ForestNETLib.Core.Global.ILogFine("ilogFine");
                ForestNETLib.Core.Global.LogFiner("logFiner");
                ForestNETLib.Core.Global.ILogFiner("ilogFiner");
                ForestNETLib.Core.Global.LogFinest("logFinest");
                ForestNETLib.Core.Global.ILogFinest("ilogFinest");

                ForestNETLib.Core.Global.Instance.ResetLog();

                o_file = new ForestNETLib.IO.File(s_testDirectory + "testLogConfig.log.01", false);
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

                ForestNETLib.IO.File.DeleteFile(s_testDirectory + "testLogConfig.log.01");

                o_file = new ForestNETLib.IO.File(s_testDirectory + "testLogConfigInternal.log.01", false);
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

                ForestNETLib.IO.File.DeleteFile(s_testDirectory + "testLogConfigInternal.log.01");

                o_file = new ForestNETLib.IO.File(s_testTempDirectory + "testLogConfigOtherImplementation.log", false);
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

                ForestNETLib.IO.File.DeleteFile(s_testTempDirectory + "testLogConfigOtherImplementation.log");

                /* ##################################################################################################### */

                ForestNETLib.IO.File.DeleteDirectory(s_testDirectory);
                Assert.That(
                    ForestNETLib.IO.File.FolderExists(s_testDirectory),
                    Is.False,
                    "directory[" + s_testDirectory + "] does exist"
                );

                ForestNETLib.IO.File.DeleteDirectory(s_testTempDirectory);
                Assert.That(
                    ForestNETLib.IO.File.FolderExists(s_testTempDirectory),
                    Is.False,
                    "directory[" + s_testTempDirectory + "] does exist"
                );

                ForestNETLib.Core.Global.Instance.ResetLogToStandard();
            }
            catch (Exception o_exc)
            {
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
        }
    }
}
