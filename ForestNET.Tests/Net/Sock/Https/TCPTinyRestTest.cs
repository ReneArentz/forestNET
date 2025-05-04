namespace ForestNET.Tests.Net.Sock.Https
{
    public class TCPTinyRestTest
    {
        [Test]
        public void TestTCPTinyRest()
        {
            try
            {
                TestConfig.InitiateTestLogging();

                string s_currentDirectory = (System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'")) + ForestNET.Lib.IO.File.DIR;
                string s_resourcesDirectory = s_currentDirectory + "Resources" + ForestNET.Lib.IO.File.DIR;
                string s_certificatesDirectory = s_resourcesDirectory + "com" + ForestNET.Lib.IO.File.DIR;
                string s_rootDirectory = s_resourcesDirectory + "restserver" + ForestNET.Lib.IO.File.DIR;
                string s_sessionDirectory = s_resourcesDirectory + "restsessions" + ForestNET.Lib.IO.File.DIR;

                string s_host = "127.0.0.1";
                string s_hostPart = s_host.Substring(0, s_host.LastIndexOf("."));
                int i_port = 443;

                foreach (ForestNET.Lib.IO.ListingElement o_file in ForestNET.Lib.IO.File.ListDirectory(s_sessionDirectory))
                {
                    if ((o_file.FullName != null) && (!o_file.FullName.EndsWith("dummy.txt")))
                    {
                        ForestNET.Lib.IO.File.DeleteFile(o_file.FullName);
                    }
                }

                /* SERVER */

                ForestNET.Lib.Net.Https.Config o_serverConfig = new("https://" + s_host, ForestNET.Lib.Net.Https.Mode.REST, ForestNET.Lib.Net.Sock.Recv.ReceiveType.SERVER)
                {
                    AllowSourceList = [s_hostPart + ".1/24"],
                    Host = s_host,
                    Port = i_port,
                    RootDirectory = s_rootDirectory,
                    SessionDirectory = s_sessionDirectory,
                    SessionMaxAge = new ForestNET.Lib.DateInterval("PT30M"),
                    SessionRefresh = true,
                    ForestREST = new TestREST(),

                    PrintExceptionStracktrace = false
                };

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

                ForestNET.Lib.Net.Https.Config o_clientConfig = new("https://" + s_host, ForestNET.Lib.Net.Https.Mode.REST, ForestNET.Lib.Net.Sock.Recv.ReceiveType.SOCKET);
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
                    null)
                {
                    RemoteCertificateName = "ForestNET Server PW"
                };

                o_clientConfig.SendingSocketInstance = o_socketSend;

                /* START SERVER + use CLIENT */

                System.Threading.Tasks.Task o_taskServer = System.Threading.Tasks.Task.Run(async () =>
                {
                    await o_socketReceive.Run();
                });

                Thread.Sleep(100);

                string s_destination = "https://" + s_host;

                /* ------------------------------------------ */
                /* Testing GET #1                             */
                /* ------------------------------------------ */

                o_clientTask.SetRequest(s_destination + "/persons", ForestNET.Lib.Net.Request.RequestType.GET);
                o_clientTask.ExecuteRequest().Wait();

                string s_expectedReturnValues = "200 - OK";

                Assert.That(
                    s_expectedReturnValues, Is.EqualTo(o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage),
                    "received return values do not match '200 - OK', but are '" + o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage + "'"
                );

                string s_hash = ForestNET.Lib.Helper.HashByteArray("SHA-256", System.Text.Encoding.UTF8.GetBytes(o_clientTask.Response ?? "")) ?? "";
                string s_expectedHash = "852396605C97809EAA877969A45C160A76701BA84096B463E00220CDF108CCC5";

                Assert.That(
                    s_hash, Is.EqualTo(s_expectedHash),
                    "received REST response #1 does not match expected hash value '" + s_expectedHash + "', but it is '" + s_hash + "'"
                );

                /* ------------------------------------------ */
                /* Testing GET #2                             */
                /* ------------------------------------------ */

                o_clientTask.SetRequest(s_destination + "/messages", ForestNET.Lib.Net.Request.RequestType.GET);
                o_clientTask.ContentType = ForestNET.Lib.Net.Request.PostType.HTML;
                o_clientTask.AddRequestParameter("To[gt]", 2);
                o_clientTask.AddRequestParameter("To[lt]", 4);
                o_clientTask.ExecuteRequest().Wait();

                s_expectedReturnValues = "200 - OK";

                Assert.That(
                    s_expectedReturnValues, Is.EqualTo(o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage),
                    "received return values do not match '200 - OK', but are '" + o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage + "'"
                );

                s_hash = ForestNET.Lib.Helper.HashByteArray("SHA-256", System.Text.Encoding.UTF8.GetBytes(o_clientTask.Response ?? "")) ?? "";
                s_expectedHash = "A5F3E58824C0906E8B73A0ACE7EFDBD851EC444A1814F810B89DA5E9B4FE8FA1";

                Assert.That(
                    s_hash, Is.EqualTo(s_expectedHash),
                    "received REST response #2 does not match expected hash value '" + s_expectedHash + "', but it is '" + s_hash + "'"
                );

                /* ------------------------------------------ */
                /* Testing POST #1                            */
                /* ------------------------------------------ */

                o_clientTask.SetRequest(s_destination + "/persons", ForestNET.Lib.Net.Request.RequestType.POST);
                o_clientTask.ContentType = ForestNET.Lib.Net.Request.PostType.HTML;
                o_clientTask.AddRequestParameter("PIN", 621897);
                o_clientTask.AddRequestParameter("Name", "Max Mustermann");
                o_clientTask.AddRequestParameter("Age", 35);
                o_clientTask.AddRequestParameter("City", "Essen");
                o_clientTask.AddRequestParameter("Country", "DE");
                o_clientTask.ExecuteRequest().Wait();

                s_expectedReturnValues = "200 - OK";

                Assert.That(
                    s_expectedReturnValues, Is.EqualTo(o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage),
                    "received return values do not match '200 - OK', but are '" + o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage + "'"
                );

                s_hash = ForestNET.Lib.Helper.HashByteArray("SHA-256", System.Text.Encoding.UTF8.GetBytes(o_clientTask.Response ?? "")) ?? "";
                s_expectedHash = "B5B545137C25FC8BAA0C1802848DCC338025BA942E8C9BD3582F63BC62937CAC";

                Assert.That(
                    s_hash, Is.EqualTo(s_expectedHash),
                    "received REST response #3 does not match expected hash value '" + s_expectedHash + "', but it is '" + s_hash + "'"
                );

                /* ------------------------------------------ */
                /* Testing GET #3                             */
                /* ------------------------------------------ */

                o_clientTask.SetRequest(s_destination + "/persons/5", ForestNET.Lib.Net.Request.RequestType.GET);
                o_clientTask.ExecuteRequest().Wait();

                s_expectedReturnValues = "200 - OK";

                Assert.That(
                    s_expectedReturnValues, Is.EqualTo(o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage),
                    "received return values do not match '200 - OK', but are '" + o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage + "'"
                );

                s_hash = ForestNET.Lib.Helper.HashByteArray("SHA-256", System.Text.Encoding.UTF8.GetBytes(o_clientTask.Response ?? "")) ?? "";
                s_expectedHash = "2C03EAFDDAC3A50252019FE2C272836D72810C5FFF60F4B67463B2404FF97673";

                Assert.That(
                    s_hash, Is.EqualTo(s_expectedHash),
                    "received REST response #4 does not match expected hash value '" + s_expectedHash + "', but it is '" + s_hash + "'"
                );

                /* ------------------------------------------ */
                /* Testing POST #2                            */
                /* ------------------------------------------ */

                o_clientTask.SetRequest(s_destination + "/persons/5/messages", ForestNET.Lib.Net.Request.RequestType.POST);
                o_clientTask.ContentType = ForestNET.Lib.Net.Request.PostType.HTML;
                o_clientTask.AddRequestParameter("To", "Jennifer Garcia");
                o_clientTask.AddRequestParameter("Subject", "Hey");
                o_clientTask.AddRequestParameter("Message", "How are you doing?");
                o_clientTask.ExecuteRequest().Wait();

                s_expectedReturnValues = "200 - OK";

                Assert.That(
                    s_expectedReturnValues, Is.EqualTo(o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage),
                    "received return values do not match '200 - OK', but are '" + o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage + "'"
                );

                s_hash = ForestNET.Lib.Helper.HashByteArray("SHA-256", System.Text.Encoding.UTF8.GetBytes(o_clientTask.Response ?? "")) ?? "";
                s_expectedHash = "DD2AC51B19874FF6343FA15E08379DB04057D9C3B30E02E622FFFAAEABE6597E";

                Assert.That(
                    s_hash, Is.EqualTo(s_expectedHash),
                    "received REST response #5 does not match expected hash value '" + s_expectedHash + "', but it is '" + s_hash + "'"
                );

                /* ------------------------------------------ */
                /* Testing GET #4                             */
                /* ------------------------------------------ */

                o_clientTask.SetRequest(s_destination + "/persons/3/messages", ForestNET.Lib.Net.Request.RequestType.GET);
                o_clientTask.ExecuteRequest().Wait();

                s_expectedReturnValues = "200 - OK";

                Assert.That(
                    s_expectedReturnValues, Is.EqualTo(o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage),
                    "received return values do not match '200 - OK', but are '" + o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage + "'"
                );

                s_hash = ForestNET.Lib.Helper.HashByteArray("SHA-256", System.Text.Encoding.UTF8.GetBytes(o_clientTask.Response ?? "")) ?? "";
                s_expectedHash = "6E2257CB54D74E701ED2D4954585B74ADD927115A82525AE44F727685671C3B3";

                Assert.That(
                    s_hash, Is.EqualTo(s_expectedHash),
                    "received REST response #6 does not match expected hash value '" + s_expectedHash + "', but it is '" + s_hash + "'"
                );

                /* ------------------------------------------ */
                /* Testing PUT #1                             */
                /* ------------------------------------------ */

                o_clientTask.SetRequest(s_destination + "/persons/1/messages/2", ForestNET.Lib.Net.Request.RequestType.PUT);
                o_clientTask.ContentType = ForestNET.Lib.Net.Request.PostType.HTML;
                o_clientTask.AddRequestParameter("Subject", "Subject changed");
                o_clientTask.ExecuteRequest().Wait();

                s_expectedReturnValues = "200 - OK";

                Assert.That(
                    s_expectedReturnValues, Is.EqualTo(o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage),
                    "received return values do not match '200 - OK', but are '" + o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage + "'"
                );

                s_hash = ForestNET.Lib.Helper.HashByteArray("SHA-256", System.Text.Encoding.UTF8.GetBytes(o_clientTask.Response ?? "")) ?? "";
                s_expectedHash = "7BDF9ACF042781E6CE795AA741472834D7466159A5316087AF9A9477BB1F2EA2";

                Assert.That(
                    s_hash, Is.EqualTo(s_expectedHash),
                    "received REST response #7 does not match expected hash value '" + s_expectedHash + "', but it is '" + s_hash + "'"
                );

                /* ------------------------------------------ */
                /* Testing GET #5                             */
                /* ------------------------------------------ */

                o_clientTask.SetRequest(s_destination + "/messages", ForestNET.Lib.Net.Request.RequestType.GET);
                o_clientTask.ExecuteRequest().Wait();

                s_expectedReturnValues = "200 - OK";

                Assert.That(
                    s_expectedReturnValues, Is.EqualTo(o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage),
                    "received return values do not match '200 - OK', but are '" + o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage + "'"
                );

                s_hash = ForestNET.Lib.Helper.HashByteArray("SHA-256", System.Text.Encoding.UTF8.GetBytes(o_clientTask.Response ?? "")) ?? "";
                s_expectedHash = "4F3F56053CC7F74119F9DA016D75D5D564DA11FD1E3BC5B546FFBC8CB503370C";

                Assert.That(
                    s_hash, Is.EqualTo(s_expectedHash),
                    "received REST response #8 does not match expected hash value '" + s_expectedHash + "', but it is '" + s_hash + "'"
                );

                /* ------------------------------------------ */
                /* Testing PUT #2                             */
                /* ------------------------------------------ */

                o_clientTask.SetRequest(s_destination + "/persons/2", ForestNET.Lib.Net.Request.RequestType.PUT);
                o_clientTask.ContentType = ForestNET.Lib.Net.Request.PostType.HTML;
                o_clientTask.AddRequestParameter("Name", "Elisabeth Johnson");
                o_clientTask.AddRequestParameter("Age", 59);
                o_clientTask.ExecuteRequest().Wait();

                s_expectedReturnValues = "200 - OK";

                Assert.That(
                    s_expectedReturnValues, Is.EqualTo(o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage),
                    "received return values do not match '200 - OK', but are '" + o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage + "'"
                );

                s_hash = ForestNET.Lib.Helper.HashByteArray("SHA-256", System.Text.Encoding.UTF8.GetBytes(o_clientTask.Response ?? "")) ?? "";
                s_expectedHash = "8A6E186070AAD202B200D5555CCB8459AD6B6F83BC52CA8E668AF2363B7A9459";

                Assert.That(
                    s_hash, Is.EqualTo(s_expectedHash),
                    "received REST response #9 does not match expected hash value '" + s_expectedHash + "', but it is '" + s_hash + "'"
                );

                /* ------------------------------------------ */
                /* Testing GET #6                             */
                /* ------------------------------------------ */

                o_clientTask.SetRequest(s_destination + "/persons", ForestNET.Lib.Net.Request.RequestType.GET);
                o_clientTask.ExecuteRequest().Wait();

                s_expectedReturnValues = "200 - OK";

                Assert.That(
                    s_expectedReturnValues, Is.EqualTo(o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage),
                    "received return values do not match '200 - OK', but are '" + o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage + "'"
                );

                s_hash = ForestNET.Lib.Helper.HashByteArray("SHA-256", System.Text.Encoding.UTF8.GetBytes(o_clientTask.Response ?? "")) ?? "";
                s_expectedHash = "267297F2F82B724B6599DD492D6FB6C0303918328F033B45B010B6EA88D40F6D";

                Assert.That(
                    s_hash, Is.EqualTo(s_expectedHash),
                    "received REST response #10 does not match expected hash value '" + s_expectedHash + "', but it is '" + s_hash + "'"
                );

                /* ------------------------------------------ */
                /* Testing GET #7                             */
                /* ------------------------------------------ */

                o_clientTask.SetRequest(s_destination + "/messages", ForestNET.Lib.Net.Request.RequestType.GET);
                o_clientTask.ExecuteRequest().Wait();

                s_expectedReturnValues = "200 - OK";

                Assert.That(
                    s_expectedReturnValues, Is.EqualTo(o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage),
                    "received return values do not match '200 - OK', but are '" + o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage + "'"
                );

                s_hash = ForestNET.Lib.Helper.HashByteArray("SHA-256", System.Text.Encoding.UTF8.GetBytes(o_clientTask.Response ?? "")) ?? "";
                s_expectedHash = "FD3249A0CC4DACB32BED491B5673CF823520BE7B1E007727C888B62FC1D3D571";

                Assert.That(
                    s_hash, Is.EqualTo(s_expectedHash),
                    "received REST response #11 does not match expected hash value '" + s_expectedHash + "', but it is '" + s_hash + "'"
                );

                /* ------------------------------------------ */
                /* Testing DELETE #1                          */
                /* ------------------------------------------ */

                o_clientTask.SetRequest(s_destination + "/persons/4/messages/1", ForestNET.Lib.Net.Request.RequestType.DELETE);
                o_clientTask.ExecuteRequest().Wait();

                s_expectedReturnValues = "200 - OK";

                Assert.That(
                    s_expectedReturnValues, Is.EqualTo(o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage),
                    "received return values do not match '200 - OK', but are '" + o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage + "'"
                );

                s_hash = ForestNET.Lib.Helper.HashByteArray("SHA-256", System.Text.Encoding.UTF8.GetBytes(o_clientTask.Response ?? "")) ?? "";
                s_expectedHash = "D36C1A10F77B0D650C97A3A234152647DF3A33123B91A6E3025EB01357A0B0C0";

                Assert.That(
                    s_hash, Is.EqualTo(s_expectedHash),
                    "received REST response #12 does not match expected hash value '" + s_expectedHash + "', but it is '" + s_hash + "'"
                );

                /* ------------------------------------------ */
                /* Testing GET #8                             */
                /* ------------------------------------------ */

                o_clientTask.SetRequest(s_destination + "/persons/4/messages", ForestNET.Lib.Net.Request.RequestType.GET);
                o_clientTask.ExecuteRequest().Wait();

                s_expectedReturnValues = "200 - OK";

                Assert.That(
                    s_expectedReturnValues, Is.EqualTo(o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage),
                    "received return values do not match '200 - OK', but are '" + o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage + "'"
                );

                s_hash = ForestNET.Lib.Helper.HashByteArray("SHA-256", System.Text.Encoding.UTF8.GetBytes(o_clientTask.Response ?? "")) ?? "";
                s_expectedHash = "0F12CE171A0BF9242F20013D6906E1C343315CBE77A5FA2EA0E9C38896D4809C";

                Assert.That(
                    s_hash, Is.EqualTo(s_expectedHash),
                    "received REST response #13 does not match expected hash value '" + s_expectedHash + "', but it is '" + s_hash + "'"
                );

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
