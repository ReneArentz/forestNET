namespace ForestNET.Tests.Core
{
    public class TimerUnitTest
    {
        [Test]
        public void TestTimer()
        {
            try
            {
                string s_currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'");
                string s_testDirectory = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "testTimer" + ForestNET.Lib.IO.File.DIR;

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

                string s_file = s_testDirectory + "timer.log";
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

                System.TimeSpan o_startTime = System.DateTime.Now.AddSeconds(10).TimeOfDay;
                string s_dateInterval = "PT5S";

                ForestNET.Lib.Timer o_timer;
                TestTimerTask o_task = new(new(s_dateInterval), o_startTime, s_file);
                o_task.ExcludeWeekday(System.DayOfWeek.Saturday);
                o_task.ExcludeWeekday(System.DayOfWeek.Sunday);
                o_timer = new(o_task);

                o_timer.StartTimer();

                System.Threading.Thread.Sleep(31000);

                o_timer.StopTimer();

                o_file = new(s_file);

                System.DayOfWeek e_dayOfWeek = System.DateTime.Now.DayOfWeek;

                if ((e_dayOfWeek == System.DayOfWeek.Saturday) || (e_dayOfWeek == System.DayOfWeek.Sunday))
                {
                    Assert.That(
                        o_file.FileLines, Is.EqualTo(0),
                        "file lines != 0"
                    );
                }
                else
                {
                    Assert.That(
                        o_file.FileLines, Is.EqualTo(4),
                        "file lines != 4"
                    );

                    Assert.That(
                        o_file.ReadLine(1), Does.EndWith("Counter: 1"),
                        "line #1 does not end with 'Counter: 1'"
                    );
                    Assert.That(
                        o_file.ReadLine(2), Does.EndWith("Counter: 2"),
                        "line #1 does not end with 'Counter: 2'"
                    );
                    Assert.That(
                        o_file.ReadLine(3), Does.EndWith("Counter: 3"),
                        "line #1 does not end with 'Counter: 3'"
                    );
                    Assert.That(
                        o_file.ReadLine(4), Does.EndWith("Counter: 4"),
                        "line #1 does not end with 'Counter: 4'"
                    );
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

        internal class TestTimerTask : ForestNET.Lib.TimerTask
        {
            private int i_cnt = 1;
            private readonly string s_file;

            public TestTimerTask(ForestNET.Lib.DateInterval p_o_interval, System.TimeSpan p_o_startTime, string p_s_file) : base(p_o_interval, p_o_startTime)
            {
                this.s_file = p_s_file;
            }

            override public void RunTimerTask()
            {
                ForestNET.Lib.IO.File o_file = new(s_file);
                o_file.AppendLine(System.DateTime.Now + "\t" + "Work in progress..." + "\t" + "Counter: " + this.i_cnt++);
            }
        }
    }
}
