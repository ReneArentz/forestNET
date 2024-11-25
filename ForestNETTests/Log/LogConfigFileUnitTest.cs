namespace ForestNETTests.Log
{
    public class LogConfigFileUnitTest
    {
        [Test]
        public void TestLogConfigFile()
        {
            try
            {
                string s_resourcesDirectory = Environment.CurrentDirectory + ForestNETLib.IO.File.DIR + "Resources" + ForestNETLib.IO.File.DIR;
                string s_logConfigFile = s_resourcesDirectory + "log" + ForestNETLib.IO.File.DIR + "LogConfigFileUnitTest.txt";

                string s_currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'");
                string s_testDirectory = s_currentDirectory + ForestNETLib.IO.File.DIR + "testLogConfigFile" + ForestNETLib.IO.File.DIR;
                string s_tempDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + ForestNETLib.IO.File.DIR + "temp";
                string s_testTempDirectory = s_tempDirectory + ForestNETLib.IO.File.DIR + "testLogConfigFile" + ForestNETLib.IO.File.DIR;

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

                if (ForestNETLib.IO.File.Exists(s_testDirectory + "testLogConfigFile.log"))
                {
                    ForestNETLib.IO.File.DeleteFile(s_testDirectory + "testLogConfigFile.log");
                }

                if (ForestNETLib.IO.File.Exists(s_testTempDirectory + "testLogConfigInternalFile.log"))
                {
                    ForestNETLib.IO.File.DeleteFile(s_testTempDirectory + "testLogConfigInternalFile.log");
                }

                /* ##################################################################################################### */

                Assert.That(ForestNETLib.IO.File.Exists(s_logConfigFile), Is.True, "configuration file does not exist at '" + s_logConfigFile + "'");

                ForestNETLib.Core.Global.Instance.ResetLogFromFile(s_logConfigFile);

                ForestNETLib.Core.Global.Instance.LogControl = 0x7F; /* all levels */
                ForestNETLib.Core.Global.Instance.InternalLogControl = 0x0D; /* only SEVERE, INFO, CONFIG */

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

                ForestNETLib.IO.File o_file = new(s_testDirectory + "testLogConfigFile.log", false);
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

                ForestNETLib.IO.File.DeleteFile(s_testDirectory + "testLogConfigFile.log");

                o_file = new ForestNETLib.IO.File(s_testTempDirectory + "testLogConfigInternalFile.log", false);
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

                ForestNETLib.IO.File.DeleteFile(s_testTempDirectory + "testLogConfigInternalFile.log");

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
