namespace ForestNET.Tests.Net.Sock
{
    public class UDPSocketTest
    {
        [Test]
        public void TestUDPSock()
        {
            try
            {
                TestConfig.InitiateTestLogging();

                for (int i = 0; i < 10; i++)
                {
                    TestUDPSockTest(1).Wait();

                    try
                    {
                        /* 10 milliseconds to close connection */
                        Thread.Sleep(10);
                    }
                    catch (Exception e)
                    {
                        ForestNET.Lib.Global.LogException(e);
                    }
                }

                for (int i = 0; i < 10; i++)
                {
                    TestUDPSockTest(2).Wait();

                    try
                    {
                        /* 10 milliseconds to close connection */
                        Thread.Sleep(10);
                    }
                    catch (Exception e)
                    {
                        ForestNET.Lib.Global.LogException(e);
                    }
                }
            }
            catch (Exception o_exc)
            {
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
        }

        private static async Task TestUDPSockTest(int p_i_taskNumber)
        {
            try
            {
                string s_host = "127.0.0.1";
                int i_port = 8080;

                UDPTest o_serverTask = new(ForestNET.Lib.Net.Sock.Type.UDP_SERVER)
                {
                    TaskNumber = p_i_taskNumber
                };
                UDPTest o_clientTask = new(ForestNET.Lib.Net.Sock.Type.UDP_CLIENT)
                {
                    TaskNumber = p_i_taskNumber
                };

                /* SERVER */

                ForestNET.Lib.Net.Sock.Recv.ReceiveUDP o_socketReceive = new(
                    ForestNET.Lib.Net.Sock.Recv.ReceiveType.SOCKET,      /* socket type */
                    s_host,                                             /* receiving address */
                    i_port,                                             /* receiving port */
                    o_serverTask,                                       /* server task */
                    10000,                                              /* timeout milliseconds */
                    -1,                                                 /* max. number of executions */
                    1500                                                /* receive buffer size */
                );

                /* CLIENT */

                ForestNET.Lib.Net.Sock.Send.SendUDP o_socketSend = new(
                    s_host,                                                     /* destination address */
                    i_port,                                                     /* destination port */
                    o_clientTask,                                               /* client task */
                    25,                                                         /* interval for waiting for other communication side */
                    5,                                                          /* max. number of executions */
                    1500,                                                       /* buffer size */
                    System.Net.Dns.GetHostName(),                               /* sender address */
                    0                                                           /* sender port */
                )
                {
                    /* need to set timeout for connect async to udp target */
                    Timeout = 10000
                };

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

                await System.Threading.Tasks.Task.Delay(200);

                o_socketSend.StopSocket();
                o_socketReceive.StopSocket();

                await System.Threading.Tasks.Task.Delay(10);
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.LogException(o_exc);
                throw;
            }
        }
    }
}
