namespace ForestNET.Tests.Net.Sock
{
    public class TCPSockBigFileTest
    {
        [Test]
        public void TestTCPSockBigFile()
        {
            try
            {
                TestConfig.InitiateTestLogging();

                string s_currentDirectory = (System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'")) + ForestNET.Lib.IO.File.DIR;
                string s_testDirectory = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "testTCPSockBigFileTest" + ForestNET.Lib.IO.File.DIR;

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

                string s_sourceFile = s_testDirectory + "sourceFile.txt";
                string s_destinationFile = s_testDirectory + "destinationFile.txt";
                _ = new ForestNET.Lib.IO.File(s_sourceFile, true);

                ForestNET.Lib.IO.File.ReplaceFileContent(s_sourceFile, ForestNET.Lib.IO.File.GenerateRandomFileContent_100MB(), System.Text.Encoding.ASCII);
                Assert.That(ForestNET.Lib.IO.File.FileLength(s_sourceFile), Is.EqualTo(104857600), "file length != 104857600");

                /* ------------------------------------------------- */

                string s_host = "127.0.0.1";
                int i_port = 8080;

                TCPTest o_serverTask = new(ForestNET.Lib.Net.Sock.Type.TCP_SERVER)
                {
                    TaskNumber = 3,
                    BigFilePath = s_destinationFile
                };
                TCPTest o_clientTask = new(ForestNET.Lib.Net.Sock.Type.TCP_CLIENT)
                {
                    TaskNumber = 3,
                    BigFilePath = s_sourceFile
                };

                /* SERVER */

                ForestNET.Lib.Net.Sock.Recv.ReceiveTCP o_socketReceive = new(
                    ForestNET.Lib.Net.Sock.Recv.ReceiveType.SOCKET,      /* socket type */
                    s_host,                                             /* receiving address */
                    i_port,                                             /* receiving port */
                    o_serverTask,                                       /* server task */
                    10000,                                              /* timeout milliseconds */
                    1,                                                 /* max. number of executions */
                    8192,                                               /* receive buffer size */
                    null                                                /* ssl context */
                );

                /* CLIENT */

                /* it is wrong to check reachability in this unit test, because server side is not online when creating socket instance for sending */
                bool b_checkReachability = false;

                ForestNET.Lib.Net.Sock.Send.SendTCP o_socketSend = new(
                    s_host,                                                     /* destination address */
                    i_port,                                                     /* destination port */
                    o_clientTask,                                               /* client task */
                    10000,                                                      /* timeout milliseconds */
                    b_checkReachability,                                        /* check if destination is reachable */
                    1,                                                          /* max. number of executions */
                    25,                                                         /* interval for waiting for other communication side */
                    8192,                                                       /* buffer size */
                    System.Net.Dns.GetHostName(),                               /* sender address */
                    0,                                                          /* sender port */
                    null                                                        /* ssl context */
                );

                /* START SERVER + CLIENT */

                System.Threading.Tasks.Task o_taskServer = System.Threading.Tasks.Task.Run(async () =>
                {
                    await o_socketReceive.Run();
                });

                System.Threading.Tasks.Task o_taskClient = System.Threading.Tasks.Task.Run(async () =>
                {
                    await System.Threading.Tasks.Task.Delay(10);
                    await o_socketSend.Run();
                });

                System.Threading.Tasks.Task.Delay(3500).Wait();

                o_socketSend.StopSocket();
                o_socketReceive.StopSocket();

                System.Threading.Tasks.Task.Delay(10).Wait();

                /* ------------------------------------------------- */

                string? s_hashSource = ForestNET.Lib.IO.File.HashFile(s_sourceFile, "SHA-256");
                string? s_hashDestination = ForestNET.Lib.IO.File.HashFile(s_destinationFile, "SHA-256");

                Assert.That(s_hashSource, Is.EqualTo(s_hashDestination), "hash value of source and destination file are not matching: '" + s_hashSource + "' != '" + s_hashDestination + "'");

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
    }
}
