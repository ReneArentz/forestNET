namespace ForestNET.Tests.Net.Sock.Https
{
    public class TCPTinyHttpsTest
    {
        [Test]
        public void TestTCPTinyHttps()
        {
            try
            {
                TestConfig.InitiateTestLogging();

                TestTCPTinyHttpsRequests();
                TestTCPTinyHttpsSession();
            }
            catch (Exception o_exc)
            {
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
        }

        private static void TestTCPTinyHttpsRequests()
        {
            try
            {
                string s_currentDirectory = (System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'")) + ForestNET.Lib.IO.File.DIR;
                string s_testDirectory = s_currentDirectory + "testTCPTinyHttps" + ForestNET.Lib.IO.File.DIR;
                string s_resourcesDirectory = s_currentDirectory + "Resources" + ForestNET.Lib.IO.File.DIR;
                string s_certificatesDirectory = s_resourcesDirectory + "com" + ForestNET.Lib.IO.File.DIR;
                string s_rootDirectory = s_resourcesDirectory + "httpsserver" + ForestNET.Lib.IO.File.DIR;
                string s_sessionDirectory = s_resourcesDirectory + "httpssessions" + ForestNET.Lib.IO.File.DIR;

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

                ForestNET.Lib.Net.Https.Config o_serverConfig = new("https://" + s_host, ForestNET.Lib.Net.Https.Mode.DYNAMIC, ForestNET.Lib.Net.Sock.Recv.ReceiveType.SERVER)
                {
                    AllowSourceList = [s_hostPart + ".1/24"],
                    Host = s_host,
                    Port = i_port,
                    RootDirectory = s_rootDirectory,
                    SessionDirectory = s_sessionDirectory,
                    SessionMaxAge = new ForestNET.Lib.DateInterval("PT30M"),
                    SessionRefresh = true,
                    ForestSeed = new TestSeed(),

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

                ForestNET.Lib.Net.Https.Config o_clientConfig = new("https://" + s_host, ForestNET.Lib.Net.Https.Mode.NORMAL, ForestNET.Lib.Net.Sock.Recv.ReceiveType.SOCKET);
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
                /* Testing GET                                */
                /* ------------------------------------------ */

                o_clientTask.SetRequest(s_destination + "/", ForestNET.Lib.Net.Request.RequestType.GET);
                o_clientTask.ExecuteRequest().Wait();

                string s_expectedReturnValues = "200 - OK";

                Assert.That(
                    s_expectedReturnValues, Is.EqualTo(o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage),
                    "received return values do not match '200 - OK', but are '" + o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage + "'"
                );

                string s_hash = ForestNET.Lib.Helper.HashByteArray("SHA-256", System.Text.Encoding.UTF8.GetBytes(o_clientTask.Response ?? "")) ?? "";
                string s_expectedHash = "1E8C23F854BFAF0C3EE24342BB1C96037BDDB10EC6DFE30E3C085DE458AE84ED";

                Assert.That(
                    s_hash, Is.EqualTo(s_expectedHash),
                    "received dynamic static response does not match expected hash value '" + s_expectedHash + "', but it is '" + s_hash + "'"
                );

                /* ------------------------------------------ */
                /* Testing POST with form data                */
                /* ------------------------------------------ */

                o_clientTask.SetRequest(s_destination + "/reflectPOST.html", ForestNET.Lib.Net.Request.RequestType.POST);
                o_clientTask.ContentType = ForestNET.Lib.Net.Request.PostType.HTML;
                o_clientTask.AddRequestParameter("fname", "John");
                o_clientTask.AddRequestParameter("lname", "Doe");
                o_clientTask.AddRequestParameter("trip-start", "2022-04-08");
                o_clientTask.AddRequestParameter("scales", "on");
                o_clientTask.AddRequestParameter("bsubmit", "Send");

                o_clientTask.ExecuteRequest().Wait();

                s_expectedReturnValues = "200 - OK";

                Assert.That(
                    s_expectedReturnValues, Is.EqualTo(o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage),
                    "received return values do not match '200 - OK', but are '" + o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage + "'"
                );

                s_hash = ForestNET.Lib.Helper.HashByteArray("SHA-256", System.Text.Encoding.UTF8.GetBytes(o_clientTask.Response ?? "")) ?? "";
                s_expectedHash = "26914E936A016292B8800192F176A968B2A6F44B67C573A6C41039EA2A658107";

                Assert.That(
                    s_hash, Is.EqualTo(s_expectedHash),
                    "received dynamic static POST response does not match expected hash value '" + s_expectedHash + "', but it is '" + s_hash + "'"
                );

                /* ------------------------------------------ */
                /* Testing POST with form and file data       */
                /* ------------------------------------------ */

                o_clientTask.SetRequest(s_destination + "/reflectPOSTandFILES.html", ForestNET.Lib.Net.Request.RequestType.POST);
                o_clientTask.ContentType = ForestNET.Lib.Net.Request.PostType.HTMLATTACHMENTS;
                o_clientTask.AddRequestParameter("fname", "John");
                o_clientTask.AddRequestParameter("lname", "Doe");
                o_clientTask.AddRequestParameter("trip-start", "2022-04-08");
                o_clientTask.AddRequestParameter("scales", "on");
                o_clientTask.AddRequestParameter("bsubmit", "Send");

                string s_filepath = s_resourcesDirectory + "httprequest" + ForestNET.Lib.IO.File.DIR + "products.json";
                o_clientTask.AddAttachement("products_json_file", s_filepath);

                o_clientTask.ExecuteRequest().Wait();

                s_expectedReturnValues = "200 - OK";

                Assert.That(
                    s_expectedReturnValues, Is.EqualTo(o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage),
                    "received return values do not match '200 - OK', but are '" + o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage + "'"
                );

                s_hash = ForestNET.Lib.Helper.HashByteArray("SHA-256", System.Text.Encoding.UTF8.GetBytes(o_clientTask.Response ?? "")) ?? "";
                s_expectedHash = "84C2CDA17D7A42A0519B898CA87F8E09CD3B935AD85A957F4030F8AC3BE8512D";

                Assert.That(
                    s_hash, Is.EqualTo(s_expectedHash),
                    "received dynamic static POST and FILES response does not match expected hash value '" + s_expectedHash + "', but it is '" + s_hash + "'"
                );

                /* ------------------------------------------ */
                /* Testing DOWNLOAD                           */
                /* ------------------------------------------ */

                o_clientTask.SetRequest(s_destination + "/products.json", ForestNET.Lib.Net.Request.RequestType.DOWNLOAD);
                o_clientTask.DownloadFilename = s_testDirectory + "products.json";
                o_clientTask.ExecuteRequest().Wait();

                Assert.That(
                    ((o_clientTask.ReturnCode == 200) && (o_clientTask.ReturnMessage != null) && (o_clientTask.ReturnMessage.StartsWith("File[/products.json] downloaded to"))),
                    "return code[" + o_clientTask.ReturnCode + "] is not 200 or file could not be downloaded"
                );

                Assert.That(ForestNET.Lib.IO.File.Exists(s_testDirectory + "products.json"), "file[" + s_testDirectory + "products.json" + "] does not exist");
                Assert.That(ForestNET.Lib.IO.File.FileLength(s_testDirectory + "products.json"), Is.EqualTo(12929), "file length of downloaded file != 12929, file length = " + ForestNET.Lib.IO.File.FileLength(s_testDirectory + "products.json"));

                s_hash = ForestNET.Lib.IO.File.HashFile(o_clientTask.DownloadFilename, "SHA-256") ?? "";
                s_expectedHash = "916F2AEA973FCC33D008654B3A5EA583B95BFFC906E5B000EED5110AD0524F94";

                Assert.That(
                    s_hash, Is.EqualTo(s_expectedHash),
                    "received download file[" + s_testDirectory + "products.json" + "] does not match expected hash value '" + s_expectedHash + "', but it is '" + s_hash + "'"
                );

                /* ------------------------------------------ */
                /* Stop SERVER and Close CLIENT               */
                /* ------------------------------------------ */

                o_socketReceive?.StopSocket();
                o_socketSend?.StopSocket();

                Thread.Sleep(2500);

                ForestNET.Lib.IO.File.DeleteDirectory(s_testDirectory);
                Assert.That(
                    ForestNET.Lib.IO.File.FolderExists(s_testDirectory),
                    Is.False,
                    "directory[" + s_testDirectory + "] does exist"
                );
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.LogException(o_exc);
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
        }

        private static void TestTCPTinyHttpsSession()
        {
            _ = TestTCPTinyHttpsSessionRequests(true, true, null);
            string? s_cookie = TestTCPTinyHttpsSessionRequests(true, false, null);
            _ = TestTCPTinyHttpsSessionRequests(false, false, s_cookie);
        }

        private static string? TestTCPTinyHttpsSessionRequests(bool p_b_useCookiesFromPreviousRequest, bool p_b_sessionRefresh, string? p_s_cookie)
        {
            try
            {
                string s_currentDirectory = (System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'")) + ForestNET.Lib.IO.File.DIR;
                string s_resourcesDirectory = s_currentDirectory + "Resources" + ForestNET.Lib.IO.File.DIR;
                string s_certificatesDirectory = s_resourcesDirectory + "com" + ForestNET.Lib.IO.File.DIR;
                string s_rootDirectory = s_resourcesDirectory + "httpsserver" + ForestNET.Lib.IO.File.DIR;
                string s_sessionDirectory = s_resourcesDirectory + "httpssessions" + ForestNET.Lib.IO.File.DIR;

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

                ForestNET.Lib.Net.Https.Config o_serverConfig = new("https://" + s_host, ForestNET.Lib.Net.Https.Mode.DYNAMIC, ForestNET.Lib.Net.Sock.Recv.ReceiveType.SERVER)
                {
                    AllowSourceList = [s_hostPart + ".1/24"],
                    Host = s_host,
                    Port = i_port,
                    RootDirectory = s_rootDirectory,
                    SessionDirectory = s_sessionDirectory,
                    SessionMaxAge = new ForestNET.Lib.DateInterval("PT30M"),
                    SessionRefresh = p_b_sessionRefresh,
                    ForestSeed = new SessionSeed(),

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

                ForestNET.Lib.Net.Https.Config o_clientConfig = new("https://" + s_host, ForestNET.Lib.Net.Https.Mode.NORMAL, ForestNET.Lib.Net.Sock.Recv.ReceiveType.SOCKET);
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
                o_clientConfig.ClientUseCookiesFromPreviousRequest = p_b_useCookiesFromPreviousRequest;

                /* START SERVER + use CLIENT */

                System.Threading.Tasks.Task o_taskServer = System.Threading.Tasks.Task.Run(async () =>
                {
                    await o_socketReceive.Run();
                });

                Thread.Sleep(100);

                string s_destination = "https://" + s_host;

                /* ------------------------------------------ */
                /* Testing SESSION #1                         */
                /* ------------------------------------------ */

                o_clientTask.SetRequest(s_destination + "/reflectSESSION.html", ForestNET.Lib.Net.Request.RequestType.GET);

                if ((o_clientTask.Seed != null) && (p_s_cookie != null) && (!ForestNET.Lib.Helper.IsStringEmpty(p_s_cookie)))
                {
                    o_clientTask.Seed.RequestHeader.Cookie = p_s_cookie;
                }

                o_clientTask.ExecuteRequest().Wait();

                string s_expectedReturnValues = "200 - OK";

                Assert.That(
                    s_expectedReturnValues, Is.EqualTo(o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage),
                    "received return values do not match '200 - OK', but are '" + o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage + "'"
                );

                string s_hash = ForestNET.Lib.Helper.HashByteArray("SHA-256", System.Text.Encoding.UTF8.GetBytes(o_clientTask.Response ?? "")) ?? "";
                string s_expectedHash = "EA3C3FAB7626CD0D7448838DFA434EB0067042B0A9DB5ABE930A83B396D48797";

                if ((!p_b_useCookiesFromPreviousRequest) && (!p_b_sessionRefresh))
                {
                    s_expectedHash = "EA3C3FAB7626CD0D7448838DFA434EB0067042B0A9DB5ABE930A83B396D48797";
                }

                Assert.That(
                    s_hash, Is.EqualTo(s_expectedHash),
                    "received dynamic static response #1 does not match expected hash value '" + s_expectedHash + "', but it is '" + s_hash + "'"
                );

                if ((!p_b_useCookiesFromPreviousRequest) && (!p_b_sessionRefresh))
                {
                    s_expectedHash = "EA3C3FAB7626CD0D7448838DFA434EB0067042B0A9DB5ABE930A83B396D48797";
                }

                /* ------------------------------------------ */
                /* Testing SESSION #2                         */
                /* ------------------------------------------ */

                o_clientTask.SetRequest(s_destination + "/reflectSESSION.html", ForestNET.Lib.Net.Request.RequestType.GET);

                o_clientTask.ExecuteRequest().Wait();

                s_expectedReturnValues = "200 - OK";

                Assert.That(
                    s_expectedReturnValues, Is.EqualTo(o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage),
                    "received return values do not match '200 - OK', but are '" + o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage + "'"
                );

                s_hash = ForestNET.Lib.Helper.HashByteArray("SHA-256", System.Text.Encoding.UTF8.GetBytes(o_clientTask.Response ?? "")) ?? "";

                if (p_b_useCookiesFromPreviousRequest) s_expectedHash = "8AAAE71E7CF6B6EBE98E1F2745EC841394D83043FB2203F5C6D2BDE08F036AC9";

                Assert.That(
                    s_hash, Is.EqualTo(s_expectedHash),
                    "received dynamic static response #2 does not match expected hash value '" + s_expectedHash + "', but it is '" + s_hash + "'"
                );

                /* ------------------------------------------ */
                /* Testing SESSION #3                         */
                /* ------------------------------------------ */

                o_clientTask.SetRequest(s_destination + "/reflectSESSION.html", ForestNET.Lib.Net.Request.RequestType.GET);

                o_clientTask.ExecuteRequest().Wait();

                s_expectedReturnValues = "200 - OK";

                Assert.That(
                    s_expectedReturnValues, Is.EqualTo(o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage),
                    "received return values do not match '200 - OK', but are '" + o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage + "'"
                );

                s_hash = ForestNET.Lib.Helper.HashByteArray("SHA-256", System.Text.Encoding.UTF8.GetBytes(o_clientTask.Response ?? "")) ?? "";

                if (p_b_useCookiesFromPreviousRequest) s_expectedHash = "B9353DC9ED4880CF2AB6D1A4BCB1BB84BBD22A0C95C28CB1AB3B0070CA669D47";

                Assert.That(
                    s_hash, Is.EqualTo(s_expectedHash),
                    "received dynamic static response #3 does not match expected hash value '" + s_expectedHash + "', but it is '" + s_hash + "'"
                );

                /* ------------------------------------------ */
                /* Testing SESSION #4                         */
                /* ------------------------------------------ */

                o_clientTask.SetRequest(s_destination + "/reflectSESSION.html", ForestNET.Lib.Net.Request.RequestType.GET);

                o_clientTask.ExecuteRequest().Wait();

                s_expectedReturnValues = "200 - OK";

                Assert.That(
                    s_expectedReturnValues, Is.EqualTo(o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage),
                    "received return values do not match '200 - OK', but are '" + o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage + "'"
                );

                s_hash = ForestNET.Lib.Helper.HashByteArray("SHA-256", System.Text.Encoding.UTF8.GetBytes(o_clientTask.Response ?? "")) ?? "";

                if (p_b_useCookiesFromPreviousRequest) s_expectedHash = "DA79A4DFA84B52ED011FE5AD1A3DFF90DE6B4677B34F7516CF987E4EF06B6334";

                Assert.That(
                    s_hash, Is.EqualTo(s_expectedHash),
                    "received dynamic static response #4 does not match expected hash value '" + s_expectedHash + "', but it is '" + s_hash + "'"
                );

                /* ------------------------------------------ */
                /* Testing SESSION #5                         */
                /* ------------------------------------------ */

                o_clientTask.SetRequest(s_destination + "/reflectSESSION.html", ForestNET.Lib.Net.Request.RequestType.GET);

                o_clientTask.ExecuteRequest().Wait();

                s_expectedReturnValues = "200 - OK";

                Assert.That(
                    s_expectedReturnValues, Is.EqualTo(o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage),
                    "received return values do not match '200 - OK', but are '" + o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage + "'"
                );

                s_hash = ForestNET.Lib.Helper.HashByteArray("SHA-256", System.Text.Encoding.UTF8.GetBytes(o_clientTask.Response ?? "")) ?? "";

                if (p_b_useCookiesFromPreviousRequest) s_expectedHash = "17777FE9D81333244954CDEFB22D8A797A3C2FAE6C9045B99FC3DFBC65C22B77";

                Assert.That(
                    s_hash, Is.EqualTo(s_expectedHash),
                    "received dynamic static response #5 does not match expected hash value '" + s_expectedHash + "', but it is '" + s_hash + "'"
                );

                /* ------------------------------------------ */
                /* Stop SERVER and Close CLIENT               */
                /* ------------------------------------------ */

                o_socketReceive?.StopSocket();
                o_socketSend?.StopSocket();

                Thread.Sleep(2500);

                return o_clientTask.Seed?.ResponseHeader.Cookie?.ClientCookieToString();
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.LogException(o_exc);
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }

            return null;
        }
    }
}
