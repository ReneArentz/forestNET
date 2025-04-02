namespace ForestNET.Tests.IO
{
    public class FileSystemWatcherUnitTest
    {
        [Test]
        public void TestFileSystemWatcher()
        {
            try
            {
                TestConfig.InitiateTestLogging();

                string s_currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'");
                string s_testDirectory = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "testFileSystemWatcher" + ForestNET.Lib.IO.File.DIR;
                
                if (ForestNET.Lib.IO.File.FolderExists(s_testDirectory))
                {
                    ForestNET.Lib.IO.File.DeleteDirectory(s_testDirectory);
                }

                ForestNET.Lib.IO.File.CreateDirectory(s_testDirectory);
                Assert.That(
                    ForestNET.Lib.IO.File.FolderExists(s_testDirectory),
                    Is.True,
                    "directory[" + s_testDirectory + "] does not exist"
                );

                string s_file = s_testDirectory + "fileSystemWatcher.log";
                ForestNET.Lib.IO.File o_file = new(s_file, true);
                Assert.That(
                    ForestNET.Lib.IO.File.Exists(s_file),
                    Is.True,
                    "file[" + s_file + "] does not exist"
                );
                Assert.That(
                    o_file.FileLines, Is.EqualTo(0),
                    "file lines != 0"
                );

                string s_watchDirectory = s_testDirectory + ForestNET.Lib.IO.File.DIR + "foo" + ForestNET.Lib.IO.File.DIR;

                if (ForestNET.Lib.IO.File.FolderExists(s_watchDirectory))
                {
                    ForestNET.Lib.IO.File.DeleteDirectory(s_watchDirectory);
                }

                ForestNET.Lib.IO.File.CreateDirectory(s_watchDirectory);
                Assert.That(
                    ForestNET.Lib.IO.File.FolderExists(s_watchDirectory),
                    Is.True,
                    "directory[" + s_watchDirectory + "] does not exist"
                );

                /**/

                System.TimeSpan o_startTime = System.DateTime.Now.AddSeconds(2).TimeOfDay;
                ForestNET.Lib.DateInterval o_dateInterval = new("PT1S");
                bool b_recursive = true;

                string s_fileExtensionFilter = "*.txt|*_log.xml";

                ForestNET.Lib.IO.FileSystemWatcher o_fileSystemWatcher = new FileSystemWatcherTest(s_watchDirectory, o_dateInterval, o_startTime, s_file);

                o_fileSystemWatcher.ExcludeWeekday(System.DayOfWeek.Monday);
                o_fileSystemWatcher.ExcludeWeekday(System.DayOfWeek.Sunday);
                o_fileSystemWatcher.Create = true;
                o_fileSystemWatcher.Change = true;
                o_fileSystemWatcher.Delete = true;
                o_fileSystemWatcher.Access = false;
                o_fileSystemWatcher.Recursive = b_recursive;
                o_fileSystemWatcher.FileExtensionFilter = s_fileExtensionFilter;

                o_fileSystemWatcher.StartWatcher();

                System.Threading.Thread.Sleep(3100);

                string s_fileFoo = s_watchDirectory + "firstFile.txt";
                ForestNET.Lib.IO.File o_fileFoo = new(s_fileFoo, true);

                System.Threading.Thread.Sleep(1100); o_fileFoo.AppendLine("one");
                System.Threading.Thread.Sleep(1100); o_fileFoo.AppendLine("two");
                System.Threading.Thread.Sleep(1100); o_fileFoo.AppendLine("three");
                System.Threading.Thread.Sleep(1100); ForestNET.Lib.IO.File.DeleteFile(s_fileFoo);

                s_fileFoo = s_watchDirectory + "ignore.xml";
                o_fileFoo = new ForestNET.Lib.IO.File(s_fileFoo, true);

                System.Threading.Thread.Sleep(1100); o_fileFoo.AppendLine("one");
                System.Threading.Thread.Sleep(1100); o_fileFoo.AppendLine("two");
                System.Threading.Thread.Sleep(1100); o_fileFoo.AppendLine("three");
                System.Threading.Thread.Sleep(1100); ForestNET.Lib.IO.File.DeleteFile(s_fileFoo);

                s_fileFoo = s_watchDirectory + "not_ignore_log.xml";
                o_fileFoo = new ForestNET.Lib.IO.File(s_fileFoo, true);

                System.Threading.Thread.Sleep(1100); o_fileFoo.AppendLine("one");
                System.Threading.Thread.Sleep(1100); o_fileFoo.AppendLine("two");
                System.Threading.Thread.Sleep(1100); o_fileFoo.AppendLine("three");
                System.Threading.Thread.Sleep(1100); ForestNET.Lib.IO.File.DeleteFile(s_fileFoo);

                string s_watchSubDirectory = s_watchDirectory + ForestNET.Lib.IO.File.DIR + "sub" + ForestNET.Lib.IO.File.DIR;
                ForestNET.Lib.IO.File.CreateDirectory(s_watchSubDirectory);
                Assert.That(
                    ForestNET.Lib.IO.File.FolderExists(s_watchSubDirectory),
                    Is.True,
                    "directory[" + s_watchSubDirectory + "] does not exist"
                );

                s_fileFoo = s_watchSubDirectory + "firstFile.txt";
                o_fileFoo = new ForestNET.Lib.IO.File(s_fileFoo, true);

                System.Threading.Thread.Sleep(1100); o_fileFoo.AppendLine("one");
                System.Threading.Thread.Sleep(1100); o_fileFoo.AppendLine("two");
                System.Threading.Thread.Sleep(1100); o_fileFoo.AppendLine("three");
                System.Threading.Thread.Sleep(1100); ForestNET.Lib.IO.File.DeleteFile(s_fileFoo);

                s_fileFoo = s_watchSubDirectory + "ignore.xml";
                o_fileFoo = new ForestNET.Lib.IO.File(s_fileFoo, true);

                System.Threading.Thread.Sleep(1100); o_fileFoo.AppendLine("one");
                System.Threading.Thread.Sleep(1100); o_fileFoo.AppendLine("two");
                System.Threading.Thread.Sleep(1100); o_fileFoo.AppendLine("three");
                System.Threading.Thread.Sleep(1100); ForestNET.Lib.IO.File.DeleteFile(s_fileFoo);

                s_fileFoo = s_watchSubDirectory + "not_ignore_log.xml";
                o_fileFoo = new ForestNET.Lib.IO.File(s_fileFoo, true);

                System.Threading.Thread.Sleep(1100); o_fileFoo.AppendLine("one");
                System.Threading.Thread.Sleep(1100); o_fileFoo.AppendLine("two");
                System.Threading.Thread.Sleep(1100); o_fileFoo.AppendLine("three");
                System.Threading.Thread.Sleep(1100); ForestNET.Lib.IO.File.DeleteFile(s_fileFoo);
                System.Threading.Thread.Sleep(1100);

                o_fileSystemWatcher.StopWatcher();

                /**/

                o_file = new(s_file);

                System.DayOfWeek o_dayOfWeek = System.DateTime.Now.DayOfWeek;

                if ((o_dayOfWeek == System.DayOfWeek.Monday) || (o_dayOfWeek == System.DayOfWeek.Sunday))
                {
                    Assert.That(
                        o_file.FileLines, Is.EqualTo(0),
                        "file lines != 0"
                    );
                }
                else
                {
                    Assert.That(
                        o_file.FileLines, Is.EqualTo(20),
                        "file lines != 20"
                    );

                    for (int i = 0; i < o_file.FileLines; i++)
                    {
                        Assert.That(
                            o_file.ReadLine(i + 1), Does.EndWith(" - " + (i + 1)),
                            "line #" + (i + 1) + " does not end with '- " + (i + 1) + "'"
                        );
                    }
                }

                ForestNET.Lib.IO.File.DeleteDirectory(s_testDirectory);
                Assert.That(
                    ForestNET.Lib.IO.File.FolderExists(s_testDirectory),
                    Is.False,
                    "directory[" + s_testDirectory + "] does exist"
                );
            }
            catch (Exception o_exc)
            {
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
        }

        internal class FileSystemWatcherTest : ForestNET.Lib.IO.FileSystemWatcher
        {
            private int i_cnt = 1;
            private readonly string s_logFile;

            public FileSystemWatcherTest(string p_s_directory, ForestNET.Lib.DateInterval p_o_interval, System.TimeSpan p_o_startTime, string p_s_logFile) : base(p_s_directory, p_o_interval, p_o_startTime)
            {
                this.s_logFile = p_s_logFile;
            }

            override public void CreateEvent(ForestNET.Lib.IO.ListingElement p_o_listingElement)
            {
                try
                {
                    ForestNET.Lib.IO.File o_file = new(this.s_logFile);
                    o_file.AppendLine(System.DateTime.Now + "\t" + "File created: " + p_o_listingElement.Name + " - " + i_cnt++);
                }
                catch (Exception o_exc)
                {
                    Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
                }
            }

            override public void ChangeEvent(ForestNET.Lib.IO.ListingElement p_o_listingElement)
            {
                try
                {
                    ForestNET.Lib.IO.File o_file = new(this.s_logFile);
                    o_file.AppendLine(System.DateTime.Now + "\t" + "File changed: " + p_o_listingElement.Name + " - at " + p_o_listingElement.LastModifiedTime + " - " + i_cnt++);
                }
                catch (Exception o_exc)
                {
                    Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
                }
            }

            override public void DeleteEvent(ForestNET.Lib.IO.ListingElement p_o_listingElement)
            {
                try
                {
                    ForestNET.Lib.IO.File o_file = new(this.s_logFile);
                    o_file.AppendLine(System.DateTime.Now + "\t" + "File deleted: " + p_o_listingElement.FullName + " - " + i_cnt++);
                }
                catch (Exception o_exc)
                {
                    Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
                }
            }

            override public void AccessEvent(ForestNET.Lib.IO.ListingElement p_o_listingElement)
            {
                try
                {
                    ForestNET.Lib.IO.File o_file = new(this.s_logFile);
                    o_file.AppendLine(System.DateTime.Now + "\t" + "File accessed: " + p_o_listingElement.Name + " - " + i_cnt++);
                }
                catch (Exception o_exc)
                {
                    Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
                }
            }
        }
    }
}
