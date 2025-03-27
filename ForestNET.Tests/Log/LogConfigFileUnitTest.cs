namespace ForestNET.Tests.Log
{
    public class LogConfigFileUnitTest
    {
        [Test]
        public void TestLogConfigFile()
        {
            try
            {
                string s_resourcesDirectory = Environment.CurrentDirectory + ForestNET.Lib.IO.File.DIR + "Resources" + ForestNET.Lib.IO.File.DIR;
                string s_logConfigFile = s_resourcesDirectory + "log" + ForestNET.Lib.IO.File.DIR + "LogConfigFileUnitTest.txt";

                string s_currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'");
                string s_testDirectory = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "testLogConfigFile" + ForestNET.Lib.IO.File.DIR;
                string s_tempDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + ForestNET.Lib.IO.File.DIR + "temp";
                string s_testTempDirectory = s_tempDirectory + ForestNET.Lib.IO.File.DIR + "testLogConfigFile" + ForestNET.Lib.IO.File.DIR;

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

                if (ForestNET.Lib.IO.File.Exists(s_testDirectory + "testLogConfigFile.log"))
                {
                    ForestNET.Lib.IO.File.DeleteFile(s_testDirectory + "testLogConfigFile.log");
                }

                if (ForestNET.Lib.IO.File.Exists(s_testTempDirectory + "testLogConfigInternalFile.log"))
                {
                    ForestNET.Lib.IO.File.DeleteFile(s_testTempDirectory + "testLogConfigInternalFile.log");
                }

                /* ##################################################################################################### */

                Assert.That(ForestNET.Lib.IO.File.Exists(s_logConfigFile), Is.True, "configuration file does not exist at '" + s_logConfigFile + "'");

                ForestNET.Lib.Global.Instance.ResetLogFromFile(s_logConfigFile);

                ForestNET.Lib.Global.Instance.LogControl = 0x7F; /* all levels */
                ForestNET.Lib.Global.Instance.InternalLogControl = 0x0D; /* only SEVERE, INFO, CONFIG */

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

                ForestNET.Lib.IO.File o_file = new(s_testDirectory + "testLogConfigFile.log", false);
                System.Collections.Generic.List<string> a_lines = o_file.FileContentAsList ?? throw new Exception("config 'testLogConfigFile.log' file has no content");

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
                }

                ForestNET.Lib.IO.File.DeleteFile(s_testDirectory + "testLogConfigFile.log");

                o_file = new ForestNET.Lib.IO.File(s_testTempDirectory + "testLogConfigInternalFile.log", false);
                a_lines = o_file.FileContentAsList ?? throw new Exception("config 'testLogConfigInternalFile.log' file has no content");

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
                            a_lines[i], Does.Contain("ilogConfig"),
                            "#3 line != 'ilogConfig'"
                        );
                    }
                }

                ForestNET.Lib.IO.File.DeleteFile(s_testTempDirectory + "testLogConfigInternalFile.log");

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
