using ForestNET.Lib.SQL;

namespace Sandbox.Tests.Net.Https
{
    public class HttpsTest
    {
        public static void TestTinyHttpsMenu(string p_s_currentDirectory)
        {
            int i_input;

            HttpsTest.HttpsConfig o_httpsConfig = HttpsTest.ReadHttpsConfig(p_s_currentDirectory);

            do
            {
                Console.WriteLine("++++++++++++++++++++++++++++++++");
                Console.WriteLine("+ test Tiny Https              +");
                Console.WriteLine("++++++++++++++++++++++++++++++++");

                Console.WriteLine("");

                Console.WriteLine("[1] run tiny https server");
                Console.WriteLine("[2] run tiny https server with sql pool in background");
                Console.WriteLine("[3] run tiny https client");
                Console.WriteLine("[4] test tiny soap server + client");
                Console.WriteLine("[5] test soap number conversion client");
                Console.WriteLine("[6] test tiny rest server + client");
                Console.WriteLine("[0] quit");

                Console.WriteLine("");

                i_input = ForestNET.Lib.Console.ConsoleInputInteger("Enter menu number[1-5;0]: ", "Invalid input.", "Please enter a value[1-11;0].") ?? 0;

                Console.WriteLine("");

                if (i_input == 1)
                {
                    HttpsTest.RunTinyHttpsServer(o_httpsConfig);
                }
                else if (i_input == 2)
                {
                    HttpsTest.RunTinyHttpsServerSqlPool(o_httpsConfig);
                }
                else if (i_input == 3)
                {
                    HttpsTest.RunTinyHttpsClient(o_httpsConfig);
                }
                else if (i_input == 4)
                {
                    HttpsTest.RunTinySoapServerAndClient(o_httpsConfig);
                }
                else if (i_input == 5)
                {
                    HttpsTest.RunTinySoapClientNumberConversion(o_httpsConfig);
                }
                else if (i_input == 6)
                {
                    HttpsTest.RunTinyRestServerAndClient(o_httpsConfig);
                }

                Console.WriteLine("");

            } while (i_input != 0);
        }

        internal class HttpsConfig
        {
            public string serverIp = string.Empty;
            public int serverPort = 0;
            public string clientIp = string.Empty;
            public int clientPort = 0;

            public string sqlPoolHost = string.Empty;
            public int sqlPoolPort = 0;
            public string sqlPoolDatasource = string.Empty;
            public string sqlPoolUser = string.Empty;
            public string sqlPoolPassword = string.Empty;

            public string currentDirectory = string.Empty;
        }

        private static HttpsConfig ReadHttpsConfig(string p_s_currentDirectory)
        {
            string s_resourcesHttpsDirectory = p_s_currentDirectory + "Resources" + ForestNET.Lib.IO.File.DIR + "https" + ForestNET.Lib.IO.File.DIR;
            string s_httpsConfigFile = "httpsConfig.txt";

            if (!ForestNET.Lib.IO.File.Exists(s_resourcesHttpsDirectory + s_httpsConfigFile))
            {
                throw new Exception("file[" + s_resourcesHttpsDirectory + s_httpsConfigFile + "] does not exists");
            }

            ForestNET.Lib.IO.File o_httpsConfigFile = new(s_resourcesHttpsDirectory + s_httpsConfigFile, false);

            if (o_httpsConfigFile.FileLines != 9)
            {
                throw new Exception("invalid config file[" + s_resourcesHttpsDirectory + s_httpsConfigFile + "]; must have '9 lines', but has '" + o_httpsConfigFile.FileLines + " lines'");
            }

            HttpsTest.HttpsConfig o_httpsConfig = new()
            {
                currentDirectory = p_s_currentDirectory
            };

            for (int i = 1; i <= o_httpsConfigFile.FileLines; i++)
            {
                string s_line = o_httpsConfigFile.ReadLine(i);

                if (i == 1)
                {
                    if (!s_line.StartsWith("serverIp"))
                    {
                        throw new Exception("Line #" + i + " does not start with 'serverIp'");
                    }

                    string[] a_split = s_line.Split("=");

                    if (a_split.Length != 2)
                    {
                        throw new Exception("Invalid key value pair for 'serverIp': '" + s_line + "'");
                    }

                    if (ForestNET.Lib.Helper.IsStringEmpty(a_split[1].Trim()))
                    {
                        throw new Exception("Invalid empty value, for 'serverIp'");
                    }

                    o_httpsConfig.serverIp = a_split[1].Trim();
                }
                else if (i == 2)
                {
                    if (!s_line.StartsWith("serverPort"))
                    {
                        throw new Exception("Line #" + i + " does not start with 'serverPort'");
                    }

                    string[] a_split = s_line.Split("=");

                    if (a_split.Length != 2)
                    {
                        throw new Exception("Invalid key value pair for 'serverPort': '" + s_line + "'");
                    }

                    if (!ForestNET.Lib.Helper.IsInteger(a_split[1].Trim()))
                    {
                        throw new Exception("Invalid value for 'serverPort': '" + a_split[1].Trim() + "' is not an integer");
                    }

                    o_httpsConfig.serverPort = int.Parse(a_split[1].Trim());
                }
                else if (i == 3)
                {
                    if (!s_line.StartsWith("clientIp"))
                    {
                        throw new Exception("Line #" + i + " does not start with 'clientIp'");
                    }

                    string[] a_split = s_line.Split("=");

                    if (a_split.Length != 2)
                    {
                        throw new Exception("Invalid key value pair for 'clientIp': '" + s_line + "'");
                    }

                    if (ForestNET.Lib.Helper.IsStringEmpty(a_split[1].Trim()))
                    {
                        throw new Exception("Invalid empty value, for 'clientIp'");
                    }

                    o_httpsConfig.clientIp = a_split[1].Trim();
                }
                else if (i == 4)
                {
                    if (!s_line.StartsWith("clientPort"))
                    {
                        throw new Exception("Line #" + i + " does not start with 'clientPort'");
                    }

                    string[] a_split = s_line.Split("=");

                    if (a_split.Length != 2)
                    {
                        throw new Exception("Invalid key value pair for 'clientPort': '" + s_line + "'");
                    }

                    if (!ForestNET.Lib.Helper.IsInteger(a_split[1].Trim()))
                    {
                        throw new Exception("Invalid value for 'clientPort': '" + a_split[1].Trim() + "' is not an integer");
                    }

                    o_httpsConfig.clientPort = int.Parse(a_split[1].Trim());
                }
                else if (i == 5)
                {
                    if (!s_line.StartsWith("sqlPoolHost"))
                    {
                        throw new Exception("Line #" + i + " does not start with 'sqlPoolHost'");
                    }

                    string[] a_split = s_line.Split("=");

                    if (a_split.Length != 2)
                    {
                        throw new Exception("Invalid key value pair for 'sqlPoolHost': '" + s_line + "'");
                    }

                    if (ForestNET.Lib.Helper.IsStringEmpty(a_split[1].Trim()))
                    {
                        throw new Exception("Invalid empty value, for 'sqlPoolHost'");
                    }

                    o_httpsConfig.sqlPoolHost = a_split[1].Trim();
                }
                else if (i == 6)
                {
                    if (!s_line.StartsWith("sqlPoolPort"))
                    {
                        throw new Exception("Line #" + i + " does not start with 'sqlPoolPort'");
                    }

                    string[] a_split = s_line.Split("=");

                    if (a_split.Length != 2)
                    {
                        throw new Exception("Invalid key value pair for 'sqlPoolPort': '" + s_line + "'");
                    }

                    if (!ForestNET.Lib.Helper.IsInteger(a_split[1].Trim()))
                    {
                        throw new Exception("Invalid value for 'sqlPoolPort': '" + a_split[1].Trim() + "' is not an integer");
                    }

                    o_httpsConfig.sqlPoolPort = int.Parse(a_split[1].Trim());
                }
                else if (i == 7)
                {
                    if (!s_line.StartsWith("sqlPoolDatasource"))
                    {
                        throw new Exception("Line #" + i + " does not start with 'sqlPoolDatasource'");
                    }

                    string[] a_split = s_line.Split("=");

                    if (a_split.Length != 2)
                    {
                        throw new Exception("Invalid key value pair for 'sqlPoolDatasource': '" + s_line + "'");
                    }

                    if (ForestNET.Lib.Helper.IsStringEmpty(a_split[1].Trim()))
                    {
                        throw new Exception("Invalid empty value, for 'sqlPoolDatasource'");
                    }

                    o_httpsConfig.sqlPoolDatasource = a_split[1].Trim();
                }
                else if (i == 8)
                {
                    if (!s_line.StartsWith("sqlPoolUser"))
                    {
                        throw new Exception("Line #" + i + " does not start with 'sqlPoolUser'");
                    }

                    string[] a_split = s_line.Split("=");

                    if (a_split.Length != 2)
                    {
                        throw new Exception("Invalid key value pair for 'sqlPoolUser': '" + s_line + "'");
                    }

                    if (ForestNET.Lib.Helper.IsStringEmpty(a_split[1].Trim()))
                    {
                        throw new Exception("Invalid empty value, for 'sqlPoolUser'");
                    }

                    o_httpsConfig.sqlPoolUser = a_split[1].Trim();
                }
                else if (i == 9)
                {
                    if (!s_line.StartsWith("sqlPoolPassword"))
                    {
                        throw new Exception("Line #" + i + " does not start with 'sqlPoolPassword'");
                    }

                    string[] a_split = s_line.Split("=");

                    if (a_split.Length != 2)
                    {
                        throw new Exception("Invalid key value pair for 'sqlPoolPassword': '" + s_line + "'");
                    }

                    if (ForestNET.Lib.Helper.IsStringEmpty(a_split[1].Trim()))
                    {
                        throw new Exception("Invalid empty value, for 'sqlPoolPassword'");
                    }

                    o_httpsConfig.sqlPoolPassword = a_split[1].Trim();
                }
            }

            Console.WriteLine("++++++++++++++++++++++++++++++++");
            Console.WriteLine("+ HTTPS config read            +");
            Console.WriteLine("++++++++++++++++++++++++++++++++");

            Console.WriteLine("");

            Console.WriteLine("server ip" + "\t\t" + o_httpsConfig.serverIp);
            Console.WriteLine("server port" + "\t\t" + o_httpsConfig.serverPort);
            Console.WriteLine("client ip" + "\t\t" + o_httpsConfig.clientIp);
            Console.WriteLine("client port" + "\t\t" + o_httpsConfig.clientPort);

            Console.WriteLine("");

            Console.WriteLine("sql pool host" + "\t\t" + o_httpsConfig.sqlPoolHost);
            Console.WriteLine("sql pool port" + "\t\t" + o_httpsConfig.sqlPoolPort);
            Console.WriteLine("sql pool datasource" + "\t" + o_httpsConfig.sqlPoolDatasource);
            Console.WriteLine("sql pool user" + "\t\t" + o_httpsConfig.sqlPoolUser);
            Console.WriteLine("sql pool password" + "\t" + o_httpsConfig.sqlPoolPassword);

            Console.WriteLine("");

            return o_httpsConfig;
        }

        private static void RunTinyHttpsServer(HttpsConfig p_o_httpsConfig)
        {
            string s_resourcesDirectory = p_o_httpsConfig.currentDirectory + "Resources" + ForestNET.Lib.IO.File.DIR;
            string s_certificatesDirectory = s_resourcesDirectory + "com" + ForestNET.Lib.IO.File.DIR;
            string s_rootDirectory = s_resourcesDirectory + "httpsserver" + ForestNET.Lib.IO.File.DIR;
            string s_sessionDirectory = s_resourcesDirectory + "httpssessions" + ForestNET.Lib.IO.File.DIR;

            string s_host = p_o_httpsConfig.serverIp;
            string s_hostPart = s_host.Substring(0, s_host.LastIndexOf("."));
            int i_port = p_o_httpsConfig.serverPort;

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
                AllowSourceList = [s_hostPart + ".1/24", "172.0.0.0/8"],
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
                ForestNET.Lib.Net.Sock.Recv.ReceiveType.SERVER,                                                  /* socket type */
                s_host,                                                                                         /* receiving address */
                i_port,                                                                                         /* receiving port */
                o_serverTask,                                                                                   /* server task */
                30000,                                                                                          /* timeout milliseconds */
                -1,                                                                                             /* max. number of executions */
                8192,																							/* receive buffer size */
                s_certificatesDirectory + "server" + ForestNET.Lib.IO.File.DIR + "cert-server-with-pw.pfx",		/* server certificate file */
                "123456"																						/* password for server certificate */
            );

            /* START SERVER */

            System.Threading.Tasks.Task o_taskServer = System.Threading.Tasks.Task.Run(async () =>
            {
                await o_socketReceive.Run();
            });

            Console.WriteLine("Server started with '" + s_host + ":" + i_port + "'");
            Console.WriteLine("You can access the site with a browser. Keep in mind it will be an unknown certificate.");
            Console.WriteLine("Try 'https://" + s_host + ":" + i_port + "' or 'https://" + s_host + ":" + i_port + "/complete_page'");

            ForestNET.Lib.Console.ConsoleInputString("Please enter any key to stop tiny https server . . . ", true);

            /* ------------------------------------------ */
            /* Stop SERVER                                */
            /* ------------------------------------------ */

            o_socketReceive?.StopSocket();
        }

        private static void RunTinyHttpsServerSqlPool(HttpsConfig p_o_httpsConfig)
        {
            Console.WriteLine("Be sure that the sql connection parameters in the config file are correct for MariaDB: '" + p_o_httpsConfig.sqlPoolHost + ":" + p_o_httpsConfig.sqlPoolPort + "'");

            string s_resourcesDirectory = p_o_httpsConfig.currentDirectory + "Resources" + ForestNET.Lib.IO.File.DIR;
            string s_certificatesDirectory = s_resourcesDirectory + "com" + ForestNET.Lib.IO.File.DIR;
            string s_rootDirectory = s_resourcesDirectory + "httpsserversql" + ForestNET.Lib.IO.File.DIR;
            string s_sessionDirectory = s_resourcesDirectory + "httpssessions" + ForestNET.Lib.IO.File.DIR;

            string s_host = p_o_httpsConfig.serverIp;
            string s_hostPart = s_host.Substring(0, s_host.LastIndexOf("."));
            int i_port = p_o_httpsConfig.serverPort;

            foreach (ForestNET.Lib.IO.ListingElement o_file in ForestNET.Lib.IO.File.ListDirectory(s_sessionDirectory))
            {
                if ((o_file.FullName != null) && (!o_file.FullName.EndsWith("dummy.txt")))
                {
                    ForestNET.Lib.IO.File.DeleteFile(o_file.FullName);
                }
            }

            /* START - check MARIADB */

            ForestNET.Lib.Global o_glob = ForestNET.Lib.Global.Instance;
            o_glob.BaseGateway = ForestNET.Lib.SQL.BaseGateway.MARIADB;
            o_glob.Base = new ForestNET.Lib.SQL.MariaDB.BaseMariaDB(p_o_httpsConfig.sqlPoolHost + ":" + p_o_httpsConfig.sqlPoolPort, p_o_httpsConfig.sqlPoolDatasource, p_o_httpsConfig.sqlPoolUser, p_o_httpsConfig.sqlPoolPassword);

            PersonRecord o_record = new();
            List<PersonRecord>? a_records = null;

            try
            {
                a_records = o_record.GetRecords();
            }
            catch (Exception)
            {
                Console.WriteLine("Could not query 'sys_forestnet_person'; creating table . . .");

                /* create table */
                Query<Create> o_queryCreate = new(o_glob.BaseGateway, SqlType.CREATE, "sys_forestnet_person");
                List<Dictionary<string, string>> a_columnsDefinition = [];

                Dictionary<string, string> o_properties = new()
                {
                    { "name", "Id" },
                    { "columnType", "integer [int]" },
                    { "constraints", "NOT NULL;PRIMARY KEY;AUTO_INCREMENT" }
                };
                a_columnsDefinition.Add(o_properties);

                o_properties = new()
                {
                    { "name", "PersonalIdentificationNumber" },
                    { "columnType", "integer [int]" },
                    { "constraints", "NOT NULL;UNIQUE" }
                };
                a_columnsDefinition.Add(o_properties);

                o_properties = new()
                {
                    { "name", "Name" },
                    { "columnType", "text [255]" },
                    { "constraints", "NOT NULL" }
                };
                a_columnsDefinition.Add(o_properties);

                o_properties = new()
                {
                    { "name", "Age" },
                    { "columnType", "integer [int]" },
                    { "constraints", "NOT NULL" }
                };
                a_columnsDefinition.Add(o_properties);

                o_properties = new()
                {
                    { "name", "City" },
                    { "columnType", "text [255]" },
                    { "constraints", "NOT NULL" }
                };
                a_columnsDefinition.Add(o_properties);

                o_properties = new()
                {
                    { "name", "Country" },
                    { "columnType", "text [36]" },
                    { "constraints", "NOT NULL" }
                };
                a_columnsDefinition.Add(o_properties);

                foreach (Dictionary<string, string> o_columnDefinition in a_columnsDefinition)
                {
                    ColumnStructure o_column = new(o_queryCreate)
                    {
                        Name = o_columnDefinition["name"],
                        AlterOperation = "ADD"
                    };
                    o_column.ColumnTypeAllocation(o_columnDefinition["columnType"]);

                    if (o_columnDefinition.TryGetValue("constraints", out string? s_foo))
                    {
                        string[] a_constraints = s_foo.Split(";");

                        for (int i = 0; i < a_constraints.Length; i++)
                        {
                            o_column.AddConstraint(o_queryCreate.ConstraintTypeAllocation(a_constraints[i]));

                            if ((a_constraints[i].CompareTo("DEFAULT") == 0) && (o_columnDefinition.TryGetValue("constraintDefaultValue", out string? s_baz)))
                            {
                                o_column.ConstraintDefaultValue = (Object)s_baz;
                            }
                        }
                    }

                    o_queryCreate.GetQuery<Create>()?.Columns.Add(o_column);
                }

                List<Dictionary<string, Object?>> a_result = o_glob.Base?.FetchQuery(o_queryCreate) ?? [];

                /* check table has been created */

                if (a_result.Count != 1)
                    throw new Exception("Result row amount of create query is not '1', it is '" + a_result.Count + "'");

                Console.WriteLine("Table 'sys_forestnet_person' created.");
            }

            if ((a_records == null) || (a_records.Count != 4))
            {
                o_record.TruncateTable();

                Console.WriteLine("Truncated table 'sys_forestnet_person'.");

                o_record = new PersonRecord
                {
                    ColumnPersonalIdentificationNumber = 643532,
                    ColumnName = "John Smith",
                    ColumnAge = 32,
                    ColumnCity = "New York",
                    ColumnCountry = "US"
                };
                o_record.InsertRecord();

                o_record = new PersonRecord
                {
                    ColumnPersonalIdentificationNumber = 284255,
                    ColumnName = "Elizabeth Miller",
                    ColumnAge = 21,
                    ColumnCity = "Hamburg",
                    ColumnCountry = "DE"
                };
                o_record.InsertRecord();

                o_record = new PersonRecord
                {
                    ColumnPersonalIdentificationNumber = 116974,
                    ColumnName = "Jennifer Garcia",
                    ColumnAge = 48,
                    ColumnCity = "London",
                    ColumnCountry = "UK"
                };
                o_record.InsertRecord();

                o_record = new PersonRecord
                {
                    ColumnPersonalIdentificationNumber = 295556,
                    ColumnName = "Jakub Kowalski",
                    ColumnAge = 39,
                    ColumnCity = "Warsaw",
                    ColumnCountry = "PL"
                };
                o_record.InsertRecord();

                Console.WriteLine("Inserted 4 standard rows into table 'sys_forestnet_person'.");
            }

            o_glob.Base?.CloseConnection();

            /* END - check MARIADB */

            /* SERVER */

            ForestNET.Lib.Net.SQL.Pool.HttpsConfig o_serverConfig = new("https://" + s_host, ForestNET.Lib.Net.Https.Mode.DYNAMIC, ForestNET.Lib.Net.Sock.Recv.ReceiveType.SERVER)
            {
                AllowSourceList = [s_hostPart + ".1/24", "172.0.0.0/8"],
                Host = s_host,
                Port = i_port,
                RootDirectory = s_rootDirectory,
                SessionDirectory = s_sessionDirectory,
                SessionMaxAge = new ForestNET.Lib.DateInterval("PT30M"),
                SessionRefresh = true,
                ForestSeed = new TestSeedSqlPool(),

                PrintExceptionStracktrace = false,

                BasePool = new ForestNET.Lib.SQL.Pool.BasePool(
                    3,
                    new ForestNET.Lib.DateInterval("PT10S"),
                    1000,
                    o_glob.BaseGateway,
                    p_o_httpsConfig.sqlPoolHost + ":" + p_o_httpsConfig.sqlPoolPort,
                    p_o_httpsConfig.sqlPoolDatasource,
                    p_o_httpsConfig.sqlPoolUser,
                    p_o_httpsConfig.sqlPoolPassword
                )
            };

            ForestNET.Lib.Net.Sock.Task.Recv.Https.TinyHttpsServer o_serverTask = new(o_serverConfig);
            ForestNET.Lib.Net.Sock.Recv.ReceiveTCP o_socketReceive = new(
                ForestNET.Lib.Net.Sock.Recv.ReceiveType.SERVER,                                                  /* socket type */
                s_host,                                                                                         /* receiving address */
                i_port,                                                                                         /* receiving port */
                o_serverTask,                                                                                   /* server task */
                30000,                                                                                          /* timeout milliseconds */
                -1,                                                                                             /* max. number of executions */
                8192,																							/* receive buffer size */
                s_certificatesDirectory + "server" + ForestNET.Lib.IO.File.DIR + "cert-server-with-pw.pfx",		/* server certificate file */
                "123456"																						/* password for server certificate */
            );

            /* START SERVER + BASE POOL */

            System.Threading.Tasks.Task o_taskServer = System.Threading.Tasks.Task.Run(async () =>
            {
                await o_socketReceive.Run();
            });
            o_serverConfig.BasePool.Start();

            Console.WriteLine("Server started with '" + s_host + ":" + i_port + "' and background base pool MARIADB");
            Console.WriteLine("You can access the site with a browser. Keep in mind it will be an unknown certificate.");
            Console.WriteLine("Try 'https://" + s_host + ":" + i_port + "'");

            ForestNET.Lib.Console.ConsoleInputString("Please enter any key to stop tiny https server . . . ", true);

            /* ------------------------------------------ */
            /* Stop SERVER + BASE POOL                    */
            /* ------------------------------------------ */

            o_socketReceive?.StopSocket();
            o_serverConfig.BasePool?.Stop();
        }

        private static void RunTinyHttpsClient(HttpsConfig p_o_httpsConfig)
        {
            string s_host = p_o_httpsConfig.clientIp;
            int i_port = p_o_httpsConfig.clientPort;

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
                null
            )
            {
                //RemoteCertificateName = "ANY_CERTIFICATE_ACCEPT"
                RemoteCertificateName = "INSERT_HOST_FOR_TLS"
            };

            o_clientConfig.SendingSocketInstance = o_socketSend;

            string s_url = "https://www.forestany.net";
            ForestNET.Lib.Net.Request.RequestType e_requestTypeMethod = ForestNET.Lib.Net.Request.RequestType.GET;
            ForestNET.Lib.Net.Request.PostType? e_postType = null;
            Dictionary<string, Object> m_requestParamters = [];
            Dictionary<string, string> m_attachments = [];

            int i_input;
            string? s_proxyHost = null;
            int i_proxyPort = 0;
            string? s_proxyUser = null;
            string? s_proxyPassword = null;

            do
            {
                Console.WriteLine("Testing Tiny Https Client");

                Console.WriteLine("");

                Console.WriteLine("URL: " + s_url);
                Console.WriteLine("Request type method: " + e_requestTypeMethod);
                Console.WriteLine("Post content type: " + e_postType);
                Console.WriteLine("Request parmaters:");

                if (m_requestParamters.Count > 0)
                {
                    foreach (KeyValuePair<string, Object> o_pair in m_requestParamters)
                    {
                        Console.WriteLine("\t" + o_pair.Key + " = " + o_pair.Value);
                    }
                }

                Console.WriteLine("Request attachment:");

                if (m_attachments.Count > 0)
                {
                    foreach (KeyValuePair<string, string> o_pair in m_attachments)
                    {
                        Console.WriteLine("\t" + o_pair.Key + " = " + o_pair.Value);
                    }
                }

                Console.WriteLine("Proxy: " + s_proxyHost + ":" + i_proxyPort);
                Console.WriteLine("Proxy Auth: " + s_proxyUser + ":******");

                Console.WriteLine("");

                Console.WriteLine("[1] set url");
                Console.WriteLine("[2] set request type method");
                Console.WriteLine("[3] set post content type");
                Console.WriteLine("[4] add request parameters");
                Console.WriteLine("[5] add attachment");
                Console.WriteLine("[6] add proxy");
                Console.WriteLine("[7] add proxy authentication");
                Console.WriteLine("[8] execute request");
                Console.WriteLine("[0] quit");

                Console.WriteLine("");

                i_input = ForestNET.Lib.Console.ConsoleInputInteger("Enter menu number[1-8;0]: ", "Invalid input.", "Please enter a value.") ?? 0;

                Console.WriteLine("");

                if (i_input == 1)
                {
                    s_url = ForestNET.Lib.Console.ConsoleInputString("URL: ", false, "Please enter a value.") ?? string.Empty;
                }
                else if (i_input == 2)
                {
                    bool b_valid = false;

                    do
                    {
                        string s_foo = ForestNET.Lib.Console.ConsoleInputString("Request type method: ", false, "Please enter a value.") ?? string.Empty;

                        switch (s_foo)
                        {
                            case "GET":
                                e_requestTypeMethod = ForestNET.Lib.Net.Request.RequestType.GET;
                                b_valid = true;
                                break;
                            case "POST":
                                e_requestTypeMethod = ForestNET.Lib.Net.Request.RequestType.POST;
                                b_valid = true;
                                break;
                            case "PUT":
                                e_requestTypeMethod = ForestNET.Lib.Net.Request.RequestType.PUT;
                                b_valid = true;
                                break;
                            case "DELETE":
                                e_requestTypeMethod = ForestNET.Lib.Net.Request.RequestType.DELETE;
                                b_valid = true;
                                break;
                            default:
                                Console.WriteLine("Please enter a valid request type method. [GET|POST|PUT|DELETE]");
                                break;
                        }
                    } while (!b_valid);
                }
                else if (i_input == 3)
                {
                    bool b_valid = false;

                    do
                    {
                        string s_foo = ForestNET.Lib.Console.ConsoleInputString("Post content type: ", false, "Please enter a value.") ?? string.Empty;

                        switch (s_foo)
                        {
                            case "HTML":
                                e_postType = ForestNET.Lib.Net.Request.PostType.HTML;
                                b_valid = true;
                                break;
                            case "HTMLATTACHMENTS":
                                e_postType = ForestNET.Lib.Net.Request.PostType.HTMLATTACHMENTS;
                                b_valid = true;
                                break;
                            default:
                                Console.WriteLine("Please enter a valid post content type. [HTML|HTMLATTACHMENTS]");
                                break;
                        }
                    } while (!b_valid);
                }
                else if (i_input == 4)
                {
                    string s_foo = ForestNET.Lib.Console.ConsoleInputString("Request parameters(e.g. key1=value1&key2=value2): ", false, "Please enter a value.") ?? string.Empty;

                    if (s_foo.Contains('&'))
                    {
                        string[] a_keyValuePairs = s_foo.Split("&");

                        foreach (string s_foo2 in a_keyValuePairs)
                        {
                            if (s_foo2.Contains('='))
                            {
                                string[] a_keyValuePair = s_foo2.Split("=");

                                m_requestParamters.Add(a_keyValuePair[0], a_keyValuePair[1]);
                            }
                        }
                    }
                    else
                    {
                        if (s_foo.Contains('='))
                        {
                            string[] a_keyValuePair = s_foo.Split("=");

                            m_requestParamters.Add(a_keyValuePair[0], a_keyValuePair[1]);
                        }
                    }
                }
                else if (i_input == 5)
                {
                    string s_filename = ForestNET.Lib.Console.ConsoleInputString("Attachment filename: ", false, "Please enter a value.") ?? string.Empty;
                    string s_filepath = ForestNET.Lib.Console.ConsoleInputString("Attachment local path to file: ", false, "Please enter a value.") ?? string.Empty;

                    m_attachments.Add(s_filename, s_filepath);
                }
                else if (i_input == 6)
                {
                    s_proxyHost = ForestNET.Lib.Console.ConsoleInputString("Proxy Address: ", true, "Please enter a value.") ?? null;
                    i_proxyPort = ForestNET.Lib.Console.ConsoleInputInteger("Proxy Port: ", "Please enter a value.") ?? 0;
                }
                else if (i_input == 7)
                {
                    s_proxyUser = ForestNET.Lib.Console.ConsoleInputString("Proxy User: ", true, "Please enter a value.") ?? null;
                    s_proxyPassword = ForestNET.Lib.Console.ConsoleInputPassword("Proxy Password: ", "Please enter a value.", '*') ?? null;
                }
                else if (i_input == 8)
                {
                    o_clientTask.SetRequest(s_url, e_requestTypeMethod);

                    if (e_postType != null)
                    {
                        o_clientTask.ContentType = e_postType;
                    }

                    if (m_requestParamters.Count > 0)
                    {
                        foreach (KeyValuePair<string, Object> o_pair in m_requestParamters)
                        {
                            o_clientTask.AddRequestParameter(o_pair.Key, o_pair.Value);
                        }
                    }

                    if (m_attachments.Count > 0)
                    {
                        foreach (KeyValuePair<string, string> o_pair in m_attachments)
                        {
                            o_clientTask.AddAttachement(o_pair.Key, o_pair.Value);
                        }
                    }

                    o_clientTask.ProxyAddress = s_proxyHost;
                    o_clientTask.ProxyPort = i_proxyPort;
                    o_clientTask.ProxyAuthenticationUser = s_proxyUser;
                    o_clientTask.ProxyAuthenticationPassword = s_proxyPassword;

                    o_clientTask.ExecuteRequest().Wait();

                    Console.WriteLine(o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage);
                    Console.WriteLine(o_clientTask.Response);

                    e_requestTypeMethod = ForestNET.Lib.Net.Request.RequestType.GET;
                    e_postType = null;
                    m_requestParamters = [];
                    m_attachments = [];
                    o_clientTask.SetRequest(s_url, e_requestTypeMethod);
                }

                Console.WriteLine("");

            } while (i_input != 0);

            /* ------------------------------------------ */
            /* Stop CLIENT                                */
            /* ------------------------------------------ */

            o_socketSend?.StopSocket();
        }

        private static void RunTinySoapServerAndClient(HttpsConfig p_o_httpsConfig)
        {
            string s_resourcesDirectory = p_o_httpsConfig.currentDirectory + "Resources" + ForestNET.Lib.IO.File.DIR;
            string s_certificatesDirectory = s_resourcesDirectory + "com" + ForestNET.Lib.IO.File.DIR;
            string s_rootDirectory = s_resourcesDirectory + "soapserver" + ForestNET.Lib.IO.File.DIR;

            string s_host = p_o_httpsConfig.serverIp;
            string s_hostPart = s_host.Substring(0, s_host.LastIndexOf("."));
            int i_port = p_o_httpsConfig.serverPort;

            string s_clientHost = p_o_httpsConfig.clientIp;
            int i_clientPort = p_o_httpsConfig.clientPort;

            /* SERVER */

            ForestNET.Lib.Net.Https.Config o_serverConfig = new("https://" + s_host, ForestNET.Lib.Net.Https.Mode.SOAP, ForestNET.Lib.Net.Sock.Recv.ReceiveType.SERVER)
            {
                AllowSourceList = [s_hostPart + ".1/24", "172.0.0.0/8"],
                Host = s_host,
                Port = i_port,
                RootDirectory = s_rootDirectory,
                NotUsingCookies = true,

                PrintExceptionStracktrace = true
            };

            ForestNET.Lib.Net.Https.SOAP.WSDL o_serverWsdl = new(s_rootDirectory + "calculator" + ForestNET.Lib.IO.File.DIR + "calculator.wsdl");
            o_serverWsdl.AddSOAPOperation("Add", CalculatorImpl.Add);
            o_serverWsdl.AddSOAPOperation("Subtract", CalculatorImpl.Subtract);
            o_serverWsdl.AddSOAPOperation("Multiply", CalculatorImpl.Multiply);
            o_serverWsdl.AddSOAPOperation("Divide", CalculatorImpl.Divide);

            o_serverConfig.WSDL = o_serverWsdl;

            ForestNET.Lib.Net.Sock.Task.Recv.Https.TinyHttpsServer o_serverTask = new(o_serverConfig);
            ForestNET.Lib.Net.Sock.Recv.ReceiveTCP o_socketReceive = new(
                ForestNET.Lib.Net.Sock.Recv.ReceiveType.SERVER,                                                  /* socket type */
                s_host,                                                                                         /* receiving address */
                i_port,                                                                                         /* receiving port */
                o_serverTask,                                                                                   /* server task */
                30000,                                                                                          /* timeout milliseconds */
                -1,                                                                                             /* max. number of executions */
                8192,																							/* receive buffer size */
                s_certificatesDirectory + "server" + ForestNET.Lib.IO.File.DIR + "cert-server-with-pw.pfx",		/* server certificate file */
                "123456"																						/* password for server certificate */
            );

            /* CLIENT */

            ForestNET.Lib.Net.Https.Config o_clientConfig = new("https://" + s_clientHost, ForestNET.Lib.Net.Https.Mode.SOAP, ForestNET.Lib.Net.Sock.Recv.ReceiveType.SOCKET);
            ForestNET.Lib.Net.Sock.Task.Send.Https.TinyHttpsClient o_clientTask = new(o_clientConfig);

            /* it is wrong to check reachability in this test, because server side is not online when creating socket instance for sending */
            bool b_checkReachability = false;

            ForestNET.Lib.Net.Sock.Send.SendTCP o_socketSend = new(
                s_clientHost,                   /* destination address */
                i_clientPort,                   /* destination port */
                o_clientTask,                   /* client task */
                30000,                          /* timeout milliseconds */
                b_checkReachability,            /* check if destination is reachable */
                1,                              /* max. number of executions */
                25,                             /* interval for waiting for other communication side */
                1500,                           /* buffer size */
                System.Net.Dns.GetHostName(),	/* local address */
                0,								/* local port */
                null                            /* client certificate allow list */
            )
            {
                RemoteCertificateName = "ForestNET Server PW"
            };

            o_clientConfig.SendingSocketInstance = o_socketSend;

            ForestNET.Lib.Net.Https.SOAP.WSDL o_clientWsdl = new(s_rootDirectory + "calculator" + ForestNET.Lib.IO.File.DIR + "calculator.wsdl");
            o_clientConfig.WSDL = o_clientWsdl;

            /* START SERVER + CLIENT */

            System.Threading.Tasks.Task o_taskServer = System.Threading.Tasks.Task.Run(async () =>
            {
                await o_socketReceive.Run();
            });

            Thread.Sleep(100);

            Console.WriteLine("SOAP Server started with '" + s_host + ":" + i_port + "' | You may must change the IP address in wsdl file under 'soap:address location'");
            Console.WriteLine("You can call the SOAP operations with the following menu...");

            int i_input;

            do
            {
                Console.WriteLine("Testing Tiny SOAP Server + Client");

                Console.WriteLine("");

                Console.WriteLine("[1] test SOAP Add method");
                Console.WriteLine("[2] test SOAP Subtract method");
                Console.WriteLine("[3] test SOAP Multiply method");
                Console.WriteLine("[4] test SOAP Divide method");
                Console.WriteLine("[0] quit");

                Console.WriteLine("");

                i_input = ForestNET.Lib.Console.ConsoleInputInteger("Enter menu number[1-4;0]: ", "Invalid input.", "Please enter a value.") ?? 0;

                Console.WriteLine("");

                if (i_input == 1)
                {
                    Calculator.Add o_add = new()
                    {
                        param1 = ForestNET.Lib.Console.ConsoleInputDouble("Value #1: ", "Entered value is not a double value.", "Please enter a value.") ?? 0.0d,
                        param2 = ForestNET.Lib.Console.ConsoleInputDouble("Value #2: ", "Entered value is not a double value.", "Please enter a value.") ?? 0.0d
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

                        if (o_addResult != null)
                        {
                            Console.WriteLine(o_add.param1 + " + " + o_add.param2 + " = " + o_addResult.result);
                        }
                        else
                        {
                            Console.WriteLine("SOAP response object is null");
                        }
                    }
                }
                else if (i_input == 2)
                {
                    Calculator.Subtract o_subtract = new()
                    {
                        param1 = ForestNET.Lib.Console.ConsoleInputDouble("Value #1: ", "Entered value is not a double value.", "Please enter a value.") ?? 0.0d,
                        param2 = ForestNET.Lib.Console.ConsoleInputDouble("Value #2: ", "Entered value is not a double value.", "Please enter a value.") ?? 0.0d
                    };

                    o_clientTask.SetSOAPRequest(o_clientWsdl.ServiceInstance.ServicePorts[0].AddressLocation, o_subtract);
                    o_clientTask.ExecuteRequest().Wait();

                    if (o_clientTask.SOAPFault != null)
                    {
                        throw new Exception("SOAP fault occured: " + o_clientTask.SOAPFault);
                    }
                    else
                    {
                        Calculator.SubtractResult o_subtractResult = (Calculator.SubtractResult)(o_clientTask.SOAPResponse ?? throw new NullReferenceException("SOAP Response is null"));

                        if (o_subtractResult != null)
                        {
                            Console.WriteLine(o_subtract.param1 + " - " + o_subtract.param2 + " = " + o_subtractResult.result);
                        }
                        else
                        {
                            Console.WriteLine("SOAP response object is null");
                        }
                    }
                }
                else if (i_input == 3)
                {
                    Calculator.Multiply o_multiply = new()
                    {
                        param1 = ForestNET.Lib.Console.ConsoleInputDouble("Value #1: ", "Entered value is not a double value.", "Please enter a value.") ?? 0.0d,
                        param2 = ForestNET.Lib.Console.ConsoleInputDouble("Value #2: ", "Entered value is not a double value.", "Please enter a value.") ?? 0.0d
                    };

                    o_clientTask.SetSOAPRequest(o_clientWsdl.ServiceInstance.ServicePorts[0].AddressLocation, o_multiply);
                    o_clientTask.ExecuteRequest().Wait();

                    if (o_clientTask.SOAPFault != null)
                    {
                        throw new Exception("SOAP fault occured: " + o_clientTask.SOAPFault);
                    }
                    else
                    {
                        Calculator.MultiplyResult o_multiplyResult = (Calculator.MultiplyResult)(o_clientTask.SOAPResponse ?? throw new NullReferenceException("SOAP Response is null"));

                        if (o_multiplyResult != null)
                        {
                            Console.WriteLine(o_multiply.param1 + " * " + o_multiply.param2 + " = " + o_multiplyResult.result);
                        }
                        else
                        {
                            Console.WriteLine("SOAP response object is null");
                        }
                    }
                }
                else if (i_input == 4)
                {
                    Calculator.Divide o_divide = new()
                    {
                        param1 = ForestNET.Lib.Console.ConsoleInputDouble("Value #1: ", "Entered value is not a double value.", "Please enter a value.") ?? 0.0d,
                        param2 = ForestNET.Lib.Console.ConsoleInputDouble("Value #2: ", "Entered value is not a double value.", "Please enter a value.") ?? 0.0d
                    };

                    o_clientTask.SetSOAPRequest(o_clientWsdl.ServiceInstance.ServicePorts[0].AddressLocation, o_divide);
                    o_clientTask.ExecuteRequest().Wait();

                    if (o_clientTask.SOAPFault != null)
                    {
                        throw new Exception("SOAP fault occured: " + o_clientTask.SOAPFault);
                    }
                    else
                    {
                        Calculator.DivideResult o_divideResult = (Calculator.DivideResult)(o_clientTask.SOAPResponse ?? throw new NullReferenceException("SOAP Response is null"));

                        if (o_divideResult != null)
                        {
                            Console.WriteLine(o_divide.param1 + " / " + o_divide.param2 + " = " + o_divideResult.result);
                        }
                        else
                        {
                            Console.WriteLine("SOAP response object is null");
                        }
                    }
                }

                Console.WriteLine("");

            } while (i_input != 0);

            /* ------------------------------------------ */
            /* Stop SERVER and Close CLIENT               */
            /* ------------------------------------------ */

            o_socketReceive?.StopSocket();
            o_socketSend?.StopSocket();

            Console.WriteLine("SOAP Server stopped");

            Thread.Sleep(2500);
        }

        private static void RunTinySoapClientNumberConversion(HttpsConfig p_o_httpsConfig)
        {
            string s_resourcesDirectory = p_o_httpsConfig.currentDirectory + "Resources" + ForestNET.Lib.IO.File.DIR;
            string s_rootDirectory = s_resourcesDirectory + "soapclient" + ForestNET.Lib.IO.File.DIR;

            string s_host = p_o_httpsConfig.clientIp;
            int i_port = p_o_httpsConfig.clientPort;

            /* CLIENT */

            ForestNET.Lib.Net.Https.Config o_clientConfig = new("https://" + s_host, ForestNET.Lib.Net.Https.Mode.SOAP, ForestNET.Lib.Net.Sock.Recv.ReceiveType.SOCKET);
            ForestNET.Lib.Net.Sock.Task.Send.Https.TinyHttpsClient o_clientTask = new(o_clientConfig);

            /* it is wrong to check reachability in this test, because server side is not online when creating socket instance for sending */
            bool b_checkReachability = false;

            ForestNET.Lib.Net.Sock.Send.SendTCP o_socketSend = new(
                s_host,                         /* destination address */
                i_port,							/* destination port */
                o_clientTask,                   /* client task */
                30000,                          /* timeout milliseconds */
                b_checkReachability,            /* check if destination is reachable */
                1,                              /* max. number of executions */
                25,                             /* interval for waiting for other communication side */
                8192,                           /* buffer size */
                System.Net.Dns.GetHostName(),	/* local address */
                0,								/* local port */
                null                            /* client certificate allow list */
            )
            {
                //RemoteCertificateName = "ANY_CERTIFICATE_ACCEPT"
                RemoteCertificateName = "INSERT_HOST_FOR_TLS"
            };

            o_clientConfig.SendingSocketInstance = o_socketSend;

            ForestNET.Lib.Net.Https.SOAP.WSDL o_clientWsdl = new(s_rootDirectory + "numberconversion.wsdl");
            o_clientConfig.WSDL = o_clientWsdl;

            int i_input;

            do
            {
                Console.WriteLine("Testing Tiny SOAP Client with external Number-Conversion service");

                Console.WriteLine("");

                Console.WriteLine("[1] test SOAP service 'Number to words'");
                Console.WriteLine("[2] test SOAP service 'Number to dollars'");
                Console.WriteLine("[0] quit");

                Console.WriteLine("");

                i_input = ForestNET.Lib.Console.ConsoleInputInteger("Enter menu number[1-2;0]: ", "Invalid input.", "Please enter a value.") ?? 0;

                Console.WriteLine("");

                if (i_input == 1)
                {
                    NumberConv.NumberToWords o_foo = new()
                    {
                        ubiNum = ForestNET.Lib.Console.ConsoleInputLong("Number to words value: ", "Entered value is not a long value.", "Please enter a value.") ?? 0L
                    };

                    o_clientTask.SetSOAPRequest(o_clientWsdl.ServiceInstance.ServicePorts[0].AddressLocation, o_foo);
                    o_clientTask.ExecuteRequest().Wait();

                    NumberConv.NumberToWordsResponse o_response = (NumberConv.NumberToWordsResponse)(o_clientTask.SOAPResponse ?? throw new NullReferenceException("SOAP Response is null"));
                    Console.WriteLine(o_foo.ubiNum + " = " + o_response.NumberToWordsResult);
                }
                else if (i_input == 2)
                {
                    NumberConv.NumberToDollars o_foo = new()
                    {
                        dNum = Convert.ToDecimal(ForestNET.Lib.Console.ConsoleInputDouble("Number to dollars value: ", "Entered value is not a double value.", "Please enter a value.") ?? 0.0d)
                    };

                    o_clientTask.SetSOAPRequest(o_clientWsdl.ServiceInstance.ServicePorts[0].AddressLocation, o_foo);
                    o_clientTask.ExecuteRequest().Wait();

                    NumberConv.NumberToDollarsResponse o_response = (NumberConv.NumberToDollarsResponse)(o_clientTask.SOAPResponse ?? throw new NullReferenceException("SOAP Response is null"));
                    Console.WriteLine(o_foo.dNum + " = " + o_response.NumberToDollarsResult);
                }

                Console.WriteLine("");

            } while (i_input != 0);

            /* ------------------------------------------ */
            /* Close CLIENT                               */
            /* ------------------------------------------ */

            o_socketSend?.StopSocket();

            Console.WriteLine("SOAP Client stopped");

            Thread.Sleep(2500);
        }

        private static void RunTinyRestServerAndClient(HttpsConfig p_o_httpsConfig)
        {
            string s_resourcesDirectory = p_o_httpsConfig.currentDirectory + "Resources" + ForestNET.Lib.IO.File.DIR;
            string s_certificatesDirectory = s_resourcesDirectory + "com" + ForestNET.Lib.IO.File.DIR;
            string s_rootDirectory = s_resourcesDirectory + "restserver" + ForestNET.Lib.IO.File.DIR;
            string s_sessionDirectory = s_resourcesDirectory + "restsessions" + ForestNET.Lib.IO.File.DIR;

            string s_host = p_o_httpsConfig.serverIp;
            string s_hostPart = s_host.Substring(0, s_host.LastIndexOf("."));
            int i_port = p_o_httpsConfig.serverPort;

            string s_clientHost = p_o_httpsConfig.clientIp;
            int i_clientPort = p_o_httpsConfig.clientPort;

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
                AllowSourceList = [s_hostPart + ".1/24", "172.0.0.0/8"],
                Host = s_host,
                Port = i_port,
                RootDirectory = s_rootDirectory,
                SessionDirectory = s_sessionDirectory,
                SessionMaxAge = new ForestNET.Lib.DateInterval("PT30M"),
                SessionRefresh = true,
                ForestREST = new TestREST(),

                PrintExceptionStracktrace = true
            };

            ForestNET.Lib.Net.Sock.Task.Recv.Https.TinyHttpsServer o_serverTask = new(o_serverConfig);
            ForestNET.Lib.Net.Sock.Recv.ReceiveTCP o_socketReceive = new(
                ForestNET.Lib.Net.Sock.Recv.ReceiveType.SERVER,                                                  /* socket type */
                s_host,                                                                                         /* receiving address */
                i_port,                                                                                         /* receiving port */
                o_serverTask,                                                                                   /* server task */
                30000,                                                                                          /* timeout milliseconds */
                -1,                                                                                             /* max. number of executions */
                8192,																							/* receive buffer size */
                s_certificatesDirectory + "server" + ForestNET.Lib.IO.File.DIR + "cert-server-with-pw.pfx",		/* server certificate file */
                "123456"																						/* password for server certificate */
            );

            /* CLIENT */

            ForestNET.Lib.Net.Https.Config o_clientConfig = new("https://" + s_clientHost, ForestNET.Lib.Net.Https.Mode.REST, ForestNET.Lib.Net.Sock.Recv.ReceiveType.SOCKET);
            ForestNET.Lib.Net.Sock.Task.Send.Https.TinyHttpsClient o_clientTask = new(o_clientConfig);

            /* it is wrong to check reachability in this junit test, because server side is not online when creating socket instance for sending */
            bool b_checkReachability = false;

            ForestNET.Lib.Net.Sock.Send.SendTCP o_socketSend = new(
                s_clientHost,                   /* destination address */
                i_clientPort,                   /* destination port */
                o_clientTask,                   /* client task */
                30000,                          /* timeout milliseconds */
                b_checkReachability,            /* check if destination is reachable */
                1,                              /* max. number of executions */
                25,                             /* interval for waiting for other communication side */
                1500,                           /* buffer size */
                System.Net.Dns.GetHostName(),	/* local address */
                0,								/* local port */
                null                            /* client certificate allow list */
            )
            {
                RemoteCertificateName = "ForestNET Server PW"
            };

            o_clientConfig.SendingSocketInstance = o_socketSend;
            o_clientConfig.ClientUseCookiesFromPreviousRequest = true;

            /* START SERVER + CLIENT */

            System.Threading.Tasks.Task o_taskServer = System.Threading.Tasks.Task.Run(async () =>
            {
                await o_socketReceive.Run();
            });

            Thread.Sleep(100);

            Console.WriteLine("REST Server started with '" + s_host + ":" + i_port + "'");
            Console.WriteLine("You can call the REST methods with the following menu...");

            string s_url = "https://" + s_host + ":" + i_port;
            string s_requestPath = "/persons";
            ForestNET.Lib.Net.Request.RequestType e_requestTypeMethod = ForestNET.Lib.Net.Request.RequestType.GET;
            Dictionary<string, Object> m_requestParamters = [];

            int i_input;

            do
            {
                Console.WriteLine("Testing Tiny Rest Server + Client");

                Console.WriteLine("");

                Console.WriteLine("Request path: " + s_requestPath);
                Console.WriteLine("Request type method: " + e_requestTypeMethod);
                Console.WriteLine("Request parmaters:");

                if (m_requestParamters.Count > 0)
                {
                    foreach (KeyValuePair<string, Object> o_pair in m_requestParamters)
                    {
                        Console.WriteLine("\t" + o_pair.Key + " = " + o_pair.Value);
                    }
                }

                Console.WriteLine("");

                Console.WriteLine("[1] change request path e.g. /persons/4/messages");
                Console.WriteLine("[2] set request type method");
                Console.WriteLine("[3] add request parameters");
                Console.WriteLine("[4] execute request");
                Console.WriteLine("[0] quit");

                Console.WriteLine("");

                i_input = ForestNET.Lib.Console.ConsoleInputInteger("Enter menu number[1-4;0]: ", "Invalid input.", "Please enter a value.") ?? 0;

                Console.WriteLine("");

                if (i_input == 1)
                {
                    s_requestPath = ForestNET.Lib.Console.ConsoleInputString("Request path(e.g. '/persons/2'): ", false, "Please enter a value.") ?? string.Empty;
                }
                else if (i_input == 2)
                {
                    bool b_valid = false;

                    do
                    {
                        string s_foo = ForestNET.Lib.Console.ConsoleInputString("Request type method: ", false, "Please enter a value.") ?? string.Empty;

                        switch (s_foo)
                        {
                            case "GET":
                                e_requestTypeMethod = ForestNET.Lib.Net.Request.RequestType.GET;
                                b_valid = true;
                                break;
                            case "POST":
                                e_requestTypeMethod = ForestNET.Lib.Net.Request.RequestType.POST;
                                b_valid = true;
                                break;
                            case "PUT":
                                e_requestTypeMethod = ForestNET.Lib.Net.Request.RequestType.PUT;
                                b_valid = true;
                                break;
                            case "DELETE":
                                e_requestTypeMethod = ForestNET.Lib.Net.Request.RequestType.DELETE;
                                b_valid = true;
                                break;
                            default:
                                Console.WriteLine("Please enter a valid request type method. [GET|POST|PUT|DELETE]");
                                break;
                        }
                    } while (!b_valid);
                }
                else if (i_input == 3)
                {
                    Console.WriteLine("Request parameters string example #1: \tPIN=621897&Name=Max Mustermann&Age=35&City=Bochum&Country=DE");
                    Console.WriteLine("Request parameters string example #2: \tTo=Jennifer Garcia&Subject=Hey&Message=How are you doing?");
                    string s_foo = ForestNET.Lib.Console.ConsoleInputString("Request parameters(e.g. key1=value1&key2=value2): ", false, "Please enter a value.") ?? string.Empty;

                    if (s_foo.Contains('&'))
                    {
                        string[] a_keyValuePairs = s_foo.Split("&");

                        foreach (string s_foo2 in a_keyValuePairs)
                        {
                            if (s_foo2.Contains('='))
                            {
                                string[] a_keyValuePair = s_foo2.Split("=");

                                m_requestParamters[a_keyValuePair[0]] = a_keyValuePair[1];
                            }
                        }
                    }
                    else
                    {
                        if (s_foo.Contains('='))
                        {
                            string[] a_keyValuePair = s_foo.Split("=");

                            m_requestParamters[a_keyValuePair[0]] = a_keyValuePair[1];
                        }
                    }
                }
                else if (i_input == 4)
                {
                    o_clientTask.SetRequest(s_url + s_requestPath, e_requestTypeMethod);

                    if ((e_requestTypeMethod != ForestNET.Lib.Net.Request.RequestType.GET) && (m_requestParamters.Count > 0))
                    {
                        o_clientTask.ContentType = ForestNET.Lib.Net.Request.PostType.HTML;
                    }

                    if (m_requestParamters.Count > 0)
                    {
                        foreach (KeyValuePair<string, Object> o_pair in m_requestParamters)
                        {
                            o_clientTask.AddRequestParameter(o_pair.Key, o_pair.Value);
                        }
                    }

                    o_clientTask.ExecuteRequest().Wait();

                    Console.WriteLine(o_clientTask.ReturnCode + " - " + o_clientTask.ReturnMessage);
                    Console.WriteLine(o_clientTask.Response);

                    e_requestTypeMethod = ForestNET.Lib.Net.Request.RequestType.GET;
                    m_requestParamters = [];
                    o_clientTask.SetRequest(s_url, e_requestTypeMethod);
                }

                Console.WriteLine("");

            } while (i_input != 0);

            /* ------------------------------------------ */
            /* Stop SERVER + Close CLIENT                 */
            /* ------------------------------------------ */

            o_socketReceive?.StopSocket();
            o_socketSend?.StopSocket();

            Console.WriteLine("REST Server stopped");

            Thread.Sleep(2500);
        }
    }
}
