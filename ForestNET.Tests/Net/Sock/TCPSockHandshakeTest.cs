namespace ForestNET.Tests.Net.Sock
{
    public class TCPSockHandshakeTest
    {
        [Test]
        public void TestTCPSockHandshake()
        {
            try
            {
                TestConfig.InitiateTestLogging();

                for (int i = 0; i < 10; i++)
                {
                    TestTCPSockHandshakeTest().Wait();

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

        private static async Task TestTCPSockHandshakeTest()
        {
            try
            {
                string s_host = "127.0.0.1";
                int i_port = 8080;

                ForestNET.Lib.Net.Sock.Task.Recv.HandshakeReceive o_serverTask = new(ForestNET.Lib.Net.Sock.Type.TCP_SERVER);
                ForestNET.Lib.Net.Sock.Task.Send.HandshakeSend o_clientTask = new(ForestNET.Lib.Net.Sock.Type.TCP_CLIENT);

                /* SERVER */

                ForestNET.Lib.Net.Sock.Recv.ReceiveTCP o_socketReceive = new(
                    ForestNET.Lib.Net.Sock.Recv.ReceiveType.SOCKET,      /* socket type */
                    s_host,                                             /* receiving address */
                    i_port,                                             /* receiving port */
                    o_serverTask,                                       /* server task */
                    10000,                                              /* timeout milliseconds */
                    -1,                                                 /* max. number of executions */
                    1500,                                               /* receive buffer size */
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
                    1500,                                                       /* buffer size */
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

                await System.Threading.Tasks.Task.Delay(100);

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
