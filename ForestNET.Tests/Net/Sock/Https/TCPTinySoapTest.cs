namespace ForestNET.Tests.Net.Sock.Https
{
    public class TCPTinySoapTest
    {
        [Test]
        public void TestTCPTinySoap()
        {
            try
            {
                TestConfig.InitiateTestLogging();

                string s_currentDirectory = (System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'")) + ForestNET.Lib.IO.File.DIR;
                string s_resourcesDirectory = s_currentDirectory + "Resources" + ForestNET.Lib.IO.File.DIR;
                string s_certificatesDirectory = s_resourcesDirectory + "com" + ForestNET.Lib.IO.File.DIR;
                string s_rootDirectory = s_resourcesDirectory + "soapserver" + ForestNET.Lib.IO.File.DIR;

                string s_host = "127.0.0.1";
                string s_hostPart = s_host.Substring(0, s_host.LastIndexOf("."));
                int i_port = 443;

                /* SERVER */

                ForestNET.Lib.Net.Https.Config o_serverConfig = new("https://" + s_host, ForestNET.Lib.Net.Https.Mode.SOAP, ForestNET.Lib.Net.Sock.Recv.ReceiveType.SERVER)
                {
                    AllowSourceList = [s_hostPart + ".1/24"],
                    Host = s_host,
                    Port = i_port,
                    RootDirectory = s_rootDirectory,
                    NotUsingCookies = true,

                    PrintExceptionStracktrace = false
                };

                ForestNET.Lib.Net.Https.SOAP.WSDL o_serverWsdl = new(s_rootDirectory + "calculator" + ForestNET.Lib.IO.File.DIR + "calculator.wsdl");
                o_serverWsdl.AddSOAPOperation("Add", CalculatorImpl.Add);
                o_serverWsdl.AddSOAPOperation("Subtract", CalculatorImpl.Subtract);
                o_serverWsdl.AddSOAPOperation("Multiply", CalculatorImpl.Multiply);
                o_serverWsdl.AddSOAPOperation("Divide", CalculatorImpl.Divide);

                o_serverConfig.WSDL = o_serverWsdl;

                ForestNET.Lib.Net.Sock.Task.Recv.Https.TinyHttpsServer o_serverTask = new(o_serverConfig);
                ForestNET.Lib.Net.Sock.Recv.ReceiveTCP o_socketReceive = new(
                    ForestNET.Lib.Net.Sock.Recv.ReceiveType.SERVER,
                    s_host,
                    i_port,
                    o_serverTask,
                    30000,
                    -1,
                    1500,
                    s_certificatesDirectory + "server" + ForestNET.Lib.IO.File.DIR + "cert-server-with-pw.pfx",
                    "123456"
                );

                /* CLIENT */

                ForestNET.Lib.Net.Https.Config o_clientConfig = new("https://" + s_host, ForestNET.Lib.Net.Https.Mode.SOAP, ForestNET.Lib.Net.Sock.Recv.ReceiveType.SOCKET);
                ForestNET.Lib.Net.Sock.Task.Send.Https.TinyHttpsClient o_clientTask = new(o_clientConfig);

                /* it is wrong to check reachability in this junit test, because server side is not online when creating socket instance for sending */
                bool b_checkReachability = false;

                ForestNET.Lib.Net.Sock.Send.SendTCP o_socketSend = new(
                    s_host,
                    i_port,
                    o_clientTask,
                    30000,
                    b_checkReachability,
                    1,
                    25,
                    1500,
                    System.Net.Dns.GetHostName(),
                    0,
                    null
                )
                {
                    RemoteCertificateName = "ForestNET Server PW"
                };

                o_clientConfig.SendingSocketInstance = o_socketSend;

                ForestNET.Lib.Net.Https.SOAP.WSDL o_clientWsdl = new(s_rootDirectory + "calculator" + ForestNET.Lib.IO.File.DIR + "calculator.wsdl");
                o_clientConfig.WSDL = o_clientWsdl;

                /* START SERVER + use CLIENT */

                System.Threading.Tasks.Task o_taskServer = System.Threading.Tasks.Task.Run(async () =>
                {
                    await o_socketReceive.Run();
                });

                Thread.Sleep(100);

                /* ------------------------------------------ */
                /* Testing SOAP Add                           */
                /* ------------------------------------------ */

                Calculator o_calculator = new();

                Calculator.Add o_add = new()
                {
                    param1 = 10.25,
                    param2 = 5.85
                };

                o_clientTask.SetSOAPRequest(o_clientWsdl.ServiceInstance.ServicePorts[0].AddressLocation, o_add);
                o_clientTask.ExecuteRequest().Wait();

                if (o_clientTask.SOAPFault != null)
                {
                    throw new Exception("SOAP fault occured: " + o_clientTask.SOAPFault);
                }
                else
                {
                    Calculator.AddResult o_addResult = (Calculator.AddResult)(o_clientTask.SOAPResponse ?? throw new NullReferenceException("SOAP Response is null"));
                    double d_result = o_add.param1 + o_add.param2;

                    Assert.That(d_result, Is.EqualTo(o_addResult.result), "result is not '" + d_result + "', but '" + o_addResult.result + "'");
                }

                /* ------------------------------------------ */
                /* Testing SOAP Subtract                      */
                /* ------------------------------------------ */

                o_calculator = new();

                Calculator.Subtract o_sub = new()
                {
                    param1 = 10.25,
                    param2 = 5.85
                };

                o_clientTask.SetSOAPRequest(o_clientWsdl.ServiceInstance.ServicePorts[0].AddressLocation, o_sub);
                o_clientTask.ExecuteRequest().Wait();

                if (o_clientTask.SOAPFault != null)
                {
                    throw new Exception("SOAP fault occured: " + o_clientTask.SOAPFault);
                }
                else
                {
                    Calculator.SubtractResult o_subResult = (Calculator.SubtractResult)(o_clientTask.SOAPResponse ?? throw new NullReferenceException("SOAP Response is null"));
                    double d_result = o_add.param1 - o_add.param2;

                    Assert.That(d_result, Is.EqualTo(o_subResult.result), "result is not '" + d_result + "', but '" + o_subResult.result + "'");
                }

                /* ------------------------------------------ */
                /* Testing SOAP Multiply                      */
                /* ------------------------------------------ */

                o_calculator = new();

                Calculator.Multiply o_mul = new()
                {
                    param1 = 10.25,
                    param2 = 5.85
                };

                o_clientTask.SetSOAPRequest(o_clientWsdl.ServiceInstance.ServicePorts[0].AddressLocation, o_mul);
                o_clientTask.ExecuteRequest().Wait();

                if (o_clientTask.SOAPFault != null)
                {
                    throw new Exception("SOAP fault occured: " + o_clientTask.SOAPFault);
                }
                else
                {
                    Calculator.MultiplyResult o_mulResult = (Calculator.MultiplyResult)(o_clientTask.SOAPResponse ?? throw new NullReferenceException("SOAP Response is null"));
                    double d_result = o_add.param1 * o_add.param2;

                    Assert.That(d_result, Is.EqualTo(o_mulResult.result), "result is not '" + d_result + "', but '" + o_mulResult.result + "'");
                }

                /* ------------------------------------------ */
                /* Testing SOAP Divide                        */
                /* ------------------------------------------ */

                o_calculator = new();

                Calculator.Divide o_div = new()
                {
                    param1 = 10.25,
                    param2 = 5.85
                };

                o_clientTask.SetSOAPRequest(o_clientWsdl.ServiceInstance.ServicePorts[0].AddressLocation, o_div);
                o_clientTask.ExecuteRequest().Wait();

                if (o_clientTask.SOAPFault != null)
                {
                    throw new Exception("SOAP fault occured: " + o_clientTask.SOAPFault);
                }
                else
                {
                    Calculator.DivideResult o_divResult = (Calculator.DivideResult)(o_clientTask.SOAPResponse ?? throw new NullReferenceException("SOAP Response is null"));
                    double d_result = o_add.param1 / o_add.param2;

                    /* double is not that precise with transposing xml and back, so we need to round */
                    Assert.That(Math.Round(d_result, 13), Is.EqualTo(Math.Round(o_divResult.result, 13)), "result is not '" + Math.Round(d_result, 13) + "', but '" + Math.Round(o_divResult.result, 13) + "'");
                }

                /* ------------------------------------------ */
                /* Stop SERVER and Close CLIENT               */
                /* ------------------------------------------ */

                o_socketReceive?.StopSocket();
                o_socketSend?.StopSocket();

                Thread.Sleep(2500);
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.LogException(o_exc);
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
        }
    }
}
