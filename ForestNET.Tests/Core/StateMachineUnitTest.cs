namespace ForestNET.Tests.Core
{
    public class StateMachineUnitTest
    {
        [Test]
        public void TestStateMachine()
        {
            try
            {
                string s_currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'");
                string s_testDirectory = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "testStateMachine" + ForestNET.Lib.IO.File.DIR;

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

                string s_file = s_testDirectory + "StateMachine.log";
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

                List<string> a_states = ["BEGINSTATE", "STATE1", "STATE2", "STATE3", "FINALSTATE"];
                List<string> a_returnCodes = ["TRANSFER", "START", "IDLE"];
                ForestNET.Lib.StateMachine o_stateMachine = new(a_states, a_returnCodes);

                o_stateMachine.AddTransition("BEGINSTATE", "START", "STATE1");
                o_stateMachine.AddTransition("BEGINSTATE", "IDLE", "BEGINSTATE");
                o_stateMachine.AddTransition("STATE1", "TRANSFER", "STATE2");
                o_stateMachine.AddTransition("STATE2", "TRANSFER", "STATE3");
                o_stateMachine.AddTransition("STATE3", "TRANSFER", "FINALSTATE");
                o_stateMachine.AddTransition("FINALSTATE", ForestNET.Lib.StateMachine.EXIT, ForestNET.Lib.StateMachine.EXIT);

                /* define state machine methods */
                o_stateMachine.AddStateMethod(new ForestNET.Lib.StateMachine.StateMethodContainer(
                    "BEGINSTATE",
                    p_a_genericList =>
                    {
                        p_a_genericList[1] = ((int)p_a_genericList[1]) + 1;

                        int i_random = ForestNET.Lib.Helper.RandomIntegerRange(1, 100);

                        if ((i_random >= 26) && (i_random <= 50))
                        {
                            ((ForestNET.Lib.IO.File)p_a_genericList[0]).AppendLine("BEGINSTATE");
                            return "START";
                        }
                        else
                        {
                            ((ForestNET.Lib.IO.File)p_a_genericList[0]).AppendLine("BEGINSTATE - IDLE");
                            p_a_genericList[2] = ((int)p_a_genericList[2]) + 1;
                            return "IDLE";
                        }
                    }
                ));

                o_stateMachine.AddStateMethod(new ForestNET.Lib.StateMachine.StateMethodContainer(
                    "STATE1",
                    p_a_genericList =>
                    {
                        ((ForestNET.Lib.IO.File)p_a_genericList[0]).AppendLine("STATE1");
                        p_a_genericList[1] = ((int)p_a_genericList[1]) + 1;
                        return "TRANSFER";
                    }
                ));

                o_stateMachine.AddStateMethod(new ForestNET.Lib.StateMachine.StateMethodContainer(
                    "STATE2",
                    p_a_genericList =>
                    {
                        ((ForestNET.Lib.IO.File)p_a_genericList[0]).AppendLine("STATE2");
                        p_a_genericList[1] = ((int)p_a_genericList[1]) + 1;
                        return "TRANSFER";
                    }
                ));

                o_stateMachine.AddStateMethod(new ForestNET.Lib.StateMachine.StateMethodContainer(
                    "STATE3",
                    p_a_genericList =>
                    {
                        ((ForestNET.Lib.IO.File)p_a_genericList[0]).AppendLine("STATE3");
                        p_a_genericList[1] = ((int)p_a_genericList[1]) + 1;
                        return "TRANSFER";
                    }
                ));

                o_stateMachine.AddStateMethod(new ForestNET.Lib.StateMachine.StateMethodContainer(
                    "FINALSTATE",
                    p_a_genericList =>
                    {
                        ((ForestNET.Lib.IO.File)p_a_genericList[0]).AppendLine("FINALSTATE");
                        p_a_genericList[1] = ((int)p_a_genericList[1]) + 1;
                        return ForestNET.Lib.StateMachine.EXIT;
                    }
                ));

                /* execute state machine until EXIT state */
                string s_returnCode = ForestNET.Lib.StateMachine.EXIT;
                string s_currentState = "BEGINSTATE";
                List<object> a_genericList =
                [
                    o_file,
                    0,
                    0
                ];

                do
                {
                    s_returnCode = o_stateMachine.ExecuteStateMethod(s_currentState, a_genericList);
                    s_currentState = o_stateMachine.LookupTransitions(s_currentState, s_returnCode);
                } while (!s_returnCode.Equals(ForestNET.Lib.StateMachine.EXIT));

                int i_amountLines = (int)a_genericList[1];
                int i_amountIDLE = (int)a_genericList[2];
                ForestNET.Lib.IO.File o_checkFile = new(s_file);

                Assert.That(
                    o_checkFile.FileLines, Is.EqualTo(i_amountLines),
                    "file lines(" + o_checkFile.FileLines + ") != " + i_amountLines
                );

                int i_checkAmountIDLE = 0;

                for (int i = 1; i <= o_checkFile.FileLines; i++)
                {
                    string s_line = o_checkFile.ReadLine(i);

                    if (s_line.EndsWith("IDLE"))
                    {
                        i_checkAmountIDLE++;
                    }
                }

                Assert.That(
                    i_checkAmountIDLE, Is.EqualTo(i_amountIDLE),
                    "i_checkAmountIDLE(" + i_checkAmountIDLE + ") != " + i_amountIDLE
                );

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

        [Test]
        public void TestStateMachineWithTimer()
        {
            try
            {
                string s_currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'");
                string s_testDirectory = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "testStateMachineWithTimer" + ForestNET.Lib.IO.File.DIR;

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

                string s_file = s_testDirectory + "StateMachineWithTimer.log";
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

                StateMachineTimerTask o_task = new(new("PT1S"), s_file);
                ForestNET.Lib.Timer o_timer = new(o_task);

                o_timer.StartTimer();

                for (int i = 0; i <= 20; i++)
                {
                    System.Threading.Thread.Sleep(1000);
                }

                o_timer.StopTimer();

                List<object> a_genericList = o_task.GenericList ?? throw new Exception("No return list from timer task");
                int i_amountLines = (int)a_genericList[1];
                int i_amountCIRCLE_END = (int)a_genericList[2];
                ForestNET.Lib.IO.File o_checkFile = new(s_file);

                Assert.That(
                    o_checkFile.FileLines, Is.EqualTo(i_amountLines),
                    "file lines(" + o_checkFile.FileLines + ") != " + i_amountLines
                );

                int i_checkAmountCIRCLE_END = 0;

                for (int i = 1; i <= o_checkFile.FileLines; i++)
                {
                    string s_line = o_checkFile.ReadLine(i);

                    if (s_line.EndsWith("CIRCLE_END"))
                    {
                        i_checkAmountCIRCLE_END++;
                    }
                }

                Assert.That(
                    i_checkAmountCIRCLE_END, Is.EqualTo(i_amountCIRCLE_END),
                    "i_checkAmountCIRCLE_END(" + i_checkAmountCIRCLE_END + ") != " + i_amountCIRCLE_END
                );

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

        private class StateMachineTimerTask : ForestNET.Lib.TimerTask
        {

            /* Fields */

            private ForestNET.Lib.StateMachine? o_stateMachine;
            private string? s_currentState;
            private string? s_returnCode;
            private List<object>? a_genericList;

            /* Properties */

            public List<object>? GenericList
            {
                get
                {
                    if ((this.a_genericList != null) && (this.a_genericList.Count == 3))
                    {
                        return this.a_genericList;
                    }
                    else
                    {
                        return null;
                    }
                }
                private set
                {
                    this.a_genericList = value;
                }
            }

            /* Methods */

            public StateMachineTimerTask(ForestNET.Lib.DateInterval p_o_interval, string p_s_logFile) : base(p_o_interval)
            {
                this.BuildStateMachine(p_s_logFile);
            }

            public StateMachineTimerTask(ForestNET.Lib.DateInterval p_o_interval, System.TimeSpan p_o_startTime, string p_s_logFile) : base(p_o_interval, p_o_startTime)
            {
                this.BuildStateMachine(p_s_logFile);
            }

            private void BuildStateMachine(string p_s_logFile)
            {
                try
                {
                    ForestNET.Lib.IO.File o_file = new(p_s_logFile);

                    List<string> a_states = ["RED", "RED_YELLOW", "GREEN", "YELLOW"];
                    List<string> a_returnCodes = ["CIRCLE_END", "CAR_CONTACT"];
                    this.o_stateMachine = new(a_states, a_returnCodes);

                    this.o_stateMachine.AddTransition("RED", "CAR_CONTACT", "RED_YELLOW");
                    this.o_stateMachine.AddTransition("RED", "CIRCLE_END", "RED");
                    this.o_stateMachine.AddTransition("RED", ForestNET.Lib.StateMachine.EXIT, ForestNET.Lib.StateMachine.EXIT);
                    this.o_stateMachine.AddTransition("RED_YELLOW", "CIRCLE_END", "GREEN");
                    this.o_stateMachine.AddTransition("GREEN", "CIRCLE_END", "YELLOW");
                    this.o_stateMachine.AddTransition("YELLOW", "CIRCLE_END", "RED");

                    /* define state machine methods */
                    this.o_stateMachine.AddStateMethod(new ForestNET.Lib.StateMachine.StateMethodContainer(
                        "RED",
                        p_a_genericList =>
                        {
                            p_a_genericList[1] = ((int)p_a_genericList[1]) + 1;

                            int i_random = ForestNET.Lib.Helper.RandomIntegerRange(1, 100);

                            if ((i_random >= 1) && (i_random <= 40))
                            {
                                ((ForestNET.Lib.IO.File)p_a_genericList[0]).AppendLine("RED - CIRCLE_END");
                                p_a_genericList[2] = ((int)p_a_genericList[2]) + 1;
                                return "CIRCLE_END";
                            }
                            else if ((i_random >= 41) && (i_random <= 75))
                            {
                                ((ForestNET.Lib.IO.File)p_a_genericList[0]).AppendLine("RED - CAR_CONTACT");
                                return "CAR_CONTACT";
                            }
                            else
                            {
                                ((ForestNET.Lib.IO.File)p_a_genericList[0]).AppendLine("RED - EXIT");
                                return "EXIT";
                            }
                        }
                    ));

                    this.o_stateMachine.AddStateMethod(new ForestNET.Lib.StateMachine.StateMethodContainer(
                        "RED_YELLOW",
                        p_a_genericList =>
                        {
                            ((ForestNET.Lib.IO.File)p_a_genericList[0]).AppendLine("RED_YELLOW");
                            p_a_genericList[1] = ((int)p_a_genericList[1]) + 1;
                            return "CIRCLE_END";
                        }
                    ));

                    this.o_stateMachine.AddStateMethod(new ForestNET.Lib.StateMachine.StateMethodContainer(
                        "GREEN",
                        p_a_genericList =>
                        {
                            ((ForestNET.Lib.IO.File)p_a_genericList[0]).AppendLine("GREEN");
                            p_a_genericList[1] = ((int)p_a_genericList[1]) + 1;
                            return "CIRCLE_END";
                        }
                    ));

                    this.o_stateMachine.AddStateMethod(new ForestNET.Lib.StateMachine.StateMethodContainer(
                        "YELLOW",
                        p_a_genericList =>
                        {
                            ((ForestNET.Lib.IO.File)p_a_genericList[0]).AppendLine("YELLOW");
                            p_a_genericList[1] = ((int)p_a_genericList[1]) + 1;
                            return "CIRCLE_END";
                        }
                    ));

                    /* define init values */
                    this.s_returnCode = ForestNET.Lib.StateMachine.EXIT;
                    this.s_currentState = "RED";
                    this.a_genericList =
                    [
                        o_file,
                        0,
                        0
                    ];
                }
                catch (Exception o_exc)
                {
                    Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
                }
            }

            override public void RunTimerTask()
            {
                this.s_returnCode = this.o_stateMachine?.ExecuteStateMethod(this.s_currentState, this.a_genericList);

                if ((this.s_returnCode == null) || (this.s_returnCode.Equals(ForestNET.Lib.StateMachine.EXIT)))
                {
                    this.Stop = true;
                }

                this.s_currentState = this.o_stateMachine?.LookupTransitions(this.s_currentState, this.s_returnCode);
            }
        }
    }
}
