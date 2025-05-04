namespace Sandbox.Tests.Net
{
    public class NetChatLobbyTest
    {
        private static NetChatLobbyTest.NetChatLobbyConfig? o_netConfig = null;

        private static Task? o_taskLobby = null;
        private static Task? o_taskChatReceive = null;

        private static CancellationTokenSource? o_cancellationTokenSourceLobby = new();
        private static CancellationTokenSource? o_cancellationTokenSourceChatReceive = new();

        private static ForestNET.Lib.Net.Sock.Com.Communication? o_communicationLobby = null;
        private static ForestNET.Lib.Net.Sock.Com.Communication? o_communicationChat = null;

        private static readonly Dictionary<DateTime, string> m_clientLobbyEntries = [];
        private static DateTime? o_lastPing = null;
        private static bool b_connected = false;
        private static readonly ForestNET.Lib.Net.Msg.MessageBox o_messageBox = new(1, 1500);
        private static readonly Dictionary<DateTime, Tuple<string, string>> m_chatHistory = [];

        public static void TestNetChatLobbyMenu(string p_s_currentDirectory)
        {
            o_netConfig = NetChatLobbyTest.ReadNetConfig(p_s_currentDirectory);

            o_netConfig.hostIp = string.Empty;
            o_netConfig.chatUser = ForestNET.Lib.Console.ConsoleInputString("Own name: ", false) ?? throw new NullReferenceException("Own name is null");
            o_netConfig.isServer = ForestNET.Lib.Console.ConsoleInputBoolean("Host chat room[true|false]: ") ?? false;

            if (o_netConfig.isServer)
            {
                o_netConfig.chatRoom = ForestNET.Lib.Console.ConsoleInputString("Room name: ", false) ?? throw new NullReferenceException("Room name is null");
            }

            Console.WriteLine("");

            Console.WriteLine("++++++++++++++++++++++++++++++++");
            Console.WriteLine(" net Chat Lobby " + ((o_netConfig.isServer) ? "Server" : "Client"));
            Console.WriteLine("++++++++++++++++++++++++++++++++");

            if (o_netConfig.isServer)
            {
                DoServer(o_netConfig);
            }
            else
            {
                DoClient(o_netConfig);
            }
        }

        private static void DoServer(NetChatLobbyConfig p_o_config)
        {
            List<string> a_states = ["LOBBY", "CHAT", "SHUTDOWN"];
            List<string> a_returnCodes = ["START_CHAT", "CLOSE_CHAT", "CHAT_IDLE", "CLOSE_SERVER", "IDLE", "SHUTDOWN"];
            ForestNET.Lib.StateMachine o_stateMachine = new(a_states, a_returnCodes);

            o_stateMachine.AddTransition("LOBBY", "IDLE", "LOBBY");
            o_stateMachine.AddTransition("LOBBY", "START_CHAT", "CHAT");
            o_stateMachine.AddTransition("CHAT", "CLOSE_CHAT", "LOBBY");
            o_stateMachine.AddTransition("CHAT", "CHAT_IDLE", "CHAT");
            o_stateMachine.AddTransition("LOBBY", "CLOSE_SERVER", "SHUTDOWN");
            o_stateMachine.AddTransition("SHUTDOWN", ForestNET.Lib.StateMachine.EXIT, ForestNET.Lib.StateMachine.EXIT);

            /* define state machine methods */
            o_stateMachine.AddStateMethod(new ForestNET.Lib.StateMachine.StateMethodContainer(
                "LOBBY",
                p_a_genericList =>
                {
                    if (o_communicationLobby == null)
                    {
                        NetLobby(p_o_config);
                    }

                    if (o_communicationChat == null)
                    {
                        NetChat(p_o_config);
                    }

                    if (b_connected)
                    {
                        m_chatHistory.Clear();
                        ClearMessageBox();
                        return "START_CHAT";
                    }

                    char c_foo = ForestNET.Lib.Console.ConsoleInputCharacter("stop server by enter[y|n]: ") ?? 'n';

                    if (c_foo == 'y')
                    {
                        return "CLOSE_SERVER";
                    }
                    else
                    {
                        return "IDLE";
                    }
                }
            ));

            o_stateMachine.AddStateMethod(new ForestNET.Lib.StateMachine.StateMethodContainer(
                "CHAT",
                p_a_genericList =>
                {
                    Console.Clear();

                    if ((!b_connected) || ((o_lastPing != null) && (DateTime.Now.Subtract(o_lastPing ?? DateTime.Now).Seconds > (5 * 60))))
                    {
                        Console.WriteLine("Other side closed chat or last ping over 5 minutes ago.");
                        Console.WriteLine("Leaving chat ...");
                        b_connected = false;
                        m_chatHistory.Clear();
                        ClearMessageBox();
                        return "CLOSE_CHAT";
                    }

                    RenderChat(p_o_config);

                    string s_commandChat = ForestNET.Lib.Console.ConsoleInputString("Message|R for refresh|EXIT for closing chat: ") ?? "EXIT";

                    if (s_commandChat.Equals("EXIT"))
                    {
                        b_connected = false;
                        m_chatHistory.Clear();
                        o_messageBox.EnqueueObject(ForestNET.Lib.Helper.ToISO8601UTC(DateTime.Now) + "|" + p_o_config.chatUser + "|%EXIT%");
                        Thread.Sleep(1500);

                        if (o_communicationChat != null)
                        {
                            try
                            {
                                o_communicationChat.Stop();
                                Thread.Sleep(2500);
                                o_communicationChat = null;
                            }
                            catch (Exception)
                            {

                            }
                            finally
                            {
                                o_communicationChat = null;
                                b_connected = false;
                            }
                        }

                        return "CLOSE_CHAT";
                    }

                    if (!s_commandChat.Equals("R"))
                    {
                        m_chatHistory[DateTime.Now] = new Tuple<string, string>(p_o_config.chatUser, s_commandChat);
                        o_messageBox.EnqueueObject(ForestNET.Lib.Helper.ToISO8601UTC(DateTime.Now) + "|" + p_o_config.chatUser + "|" + s_commandChat);
                    }

                    return "CHAT_IDLE";
                }
            ));

            o_stateMachine.AddStateMethod(new ForestNET.Lib.StateMachine.StateMethodContainer(
                "SHUTDOWN",
                p_a_genericList =>
                {
                    m_chatHistory.Clear();
                    ClearMessageBox();
                    b_connected = false;
                    o_lastPing = null;

                    if (o_taskLobby != null)
                    {
                        o_cancellationTokenSourceLobby?.Cancel();
                        Thread.Sleep(500);
                        o_cancellationTokenSourceLobby = new();
                        o_taskLobby = null;
                    }

                    if (o_communicationLobby != null)
                    {
                        try
                        {
                            o_communicationLobby.Stop();
                            Thread.Sleep(2500);
                        }
                        catch (Exception)
                        {

                        }
                        finally
                        {
                            o_communicationLobby = null;
                        }
                    }

                    if (o_taskChatReceive != null)
                    {
                        o_cancellationTokenSourceChatReceive?.Cancel();
                        Thread.Sleep(500);
                        o_cancellationTokenSourceChatReceive = new();
                        o_taskChatReceive = null;
                    }

                    if (o_communicationChat != null)
                    {
                        try
                        {
                            o_communicationChat.Stop();
                            Thread.Sleep(2500);
                            o_communicationChat = null;
                        }
                        catch (Exception)
                        {

                        }
                        finally
                        {
                            o_communicationChat = null;
                        }
                    }

                    return ForestNET.Lib.StateMachine.EXIT;
                }
            ));

            /* execute state machine until EXIT state */
            string s_returnCode = ForestNET.Lib.StateMachine.EXIT;
            string s_currentState = "LOBBY";
            List<object> a_genericList = [];

            do
            {
                s_returnCode = o_stateMachine.ExecuteStateMethod(s_currentState, a_genericList);
                s_currentState = o_stateMachine.LookupTransitions(s_currentState, s_returnCode);
            } while (!s_returnCode.Equals(ForestNET.Lib.StateMachine.EXIT));
        }

        private static void DoClient(NetChatLobbyConfig p_o_config)
        {
            List<string> a_states = ["LOBBY", "CHAT", "SHUTDOWN"];
            List<string> a_returnCodes = ["START_CHAT", "CLOSE_CHAT", "CHAT_IDLE", "CLOSE_CLIENT", "IDLE", "SHUTDOWN"];
            ForestNET.Lib.StateMachine o_stateMachine = new(a_states, a_returnCodes);

            o_stateMachine.AddTransition("LOBBY", "IDLE", "LOBBY");
            o_stateMachine.AddTransition("LOBBY", "START_CHAT", "CHAT");
            o_stateMachine.AddTransition("CHAT", "CLOSE_CHAT", "LOBBY");
            o_stateMachine.AddTransition("CHAT", "CHAT_IDLE", "CHAT");
            o_stateMachine.AddTransition("LOBBY", "CLOSE_CLIENT", "SHUTDOWN");
            o_stateMachine.AddTransition("SHUTDOWN", ForestNET.Lib.StateMachine.EXIT, ForestNET.Lib.StateMachine.EXIT);

            /* define state machine methods */
            o_stateMachine.AddStateMethod(new ForestNET.Lib.StateMachine.StateMethodContainer(
                "LOBBY",
                p_a_genericList =>
                {
                    Console.Clear();

                    if (o_taskChatReceive != null)
                    {
                        o_cancellationTokenSourceChatReceive?.Cancel();
                        Thread.Sleep(500);
                        o_cancellationTokenSourceChatReceive = new();
                        o_taskChatReceive = null;
                    }

                    if (o_communicationChat != null)
                    {
                        try
                        {
                            o_communicationChat.Stop();
                            Thread.Sleep(2500);
                            o_communicationChat = null;
                        }
                        catch (Exception)
                        {

                        }

                        finally
                        {
                            o_communicationChat = null;
                        }
                    }

                    o_lastPing = null;

                    if (o_communicationLobby == null)
                    {
                        NetLobby(p_o_config);
                    }

                    if (m_clientLobbyEntries.Count > 0)
                    {
                        int i = 1;

                        foreach (KeyValuePair<DateTime, string> o_entry in m_clientLobbyEntries)
                        {
                            Console.WriteLine("#" + i++ + " - " + o_entry.Value);
                        }
                    }

                    char c_option = ForestNET.Lib.Console.ConsoleInputCharacter("Choose chat room with '1, 2, 3, ...' or press 'R' for refresh or '0' for stop client: ") ?? '0';

                    if (c_option == '0')
                    {
                        return "CLOSE_CLIENT";
                    }
                    else if (c_option == 'R')
                    {
                        return "IDLE";
                    }
                    else
                    {
                        int i_foo = -1;

                        try
                        {
                            i_foo = int.Parse("" + c_option);
                        }
                        catch (Exception)
                        {
                            return "IDLE";
                        }

                        if ((i_foo >= 0) && (i_foo <= m_clientLobbyEntries.Count))
                        {
                            if (m_clientLobbyEntries.Count > 0)
                            {
                                int i = 1;
                                foreach (KeyValuePair<DateTime, string> o_entry in m_clientLobbyEntries)
                                {
                                    if (i == i_foo)
                                    {
                                        p_o_config.hostIp = (o_entry.Value.Split('|')[1]).Split(':')[0];
                                        break;
                                    }
                                }
                            }

                            ClearMessageBox();
                            return "START_CHAT";
                        }
                        else
                        {
                            return "IDLE";
                        }
                    }
                }
            ));

            o_stateMachine.AddStateMethod(new ForestNET.Lib.StateMachine.StateMethodContainer(
                "CHAT",
                p_a_genericList =>
                {
                    Console.Clear();

                    if (o_taskLobby != null)
                    {
                        o_cancellationTokenSourceLobby?.Cancel();
                        Thread.Sleep(500);
                        o_cancellationTokenSourceLobby = new();
                        o_taskLobby = null;
                    }

                    if (o_communicationLobby != null)
                    {
                        try
                        {
                            o_communicationLobby.Stop();
                            Thread.Sleep(2500);
                        }
                        catch (Exception)
                        {

                        }

                        finally
                        {
                            o_communicationLobby = null;
                        }
                    }

                    if (o_communicationChat == null)
                    {
                        NetChat(p_o_config);
                        b_connected = true;
                    }

                    if ((!b_connected) || ((o_lastPing != null) && (DateTime.Now.Subtract(o_lastPing ?? DateTime.Now).Seconds > (5 * 60))))
                    {
                        Console.WriteLine("Other side closed chat or last ping over 5 minutes ago.");
                        Console.WriteLine("Leaving chat ...");
                        b_connected = false;
                        m_chatHistory.Clear();
                        ClearMessageBox();
                        return "CLOSE_CHAT";
                    }

                    RenderChat(p_o_config);

                    string s_commandChat = ForestNET.Lib.Console.ConsoleInputString("Message|R for refresh|EXIT for closing chat: ") ?? "EXIT";

                    if (s_commandChat.Equals("EXIT"))
                    {
                        b_connected = false;
                        m_chatHistory.Clear();
                        o_messageBox.EnqueueObject(ForestNET.Lib.Helper.ToISO8601UTC(DateTime.Now) + "|" + p_o_config.chatUser + "|%EXIT%");
                        Thread.Sleep(1000);
                        return "CLOSE_CHAT";
                    }

                    if (!s_commandChat.Equals("R"))
                    {
                        m_chatHistory[DateTime.Now] = new Tuple<String, String>(p_o_config.chatUser, s_commandChat);
                        o_messageBox.EnqueueObject(ForestNET.Lib.Helper.ToISO8601UTC(DateTime.Now) + "|" + p_o_config.chatUser + "|" + s_commandChat);
                    }

                    return "CHAT_IDLE";
                }
            ));

            o_stateMachine.AddStateMethod(new ForestNET.Lib.StateMachine.StateMethodContainer(
                "SHUTDOWN",
                p_a_genericList =>
                {
                    m_chatHistory.Clear();
                    ClearMessageBox();
                    b_connected = false;
                    o_lastPing = null;

                    if (o_taskLobby != null)
                    {
                        o_cancellationTokenSourceLobby?.Cancel();
                        Thread.Sleep(500);
                        o_cancellationTokenSourceLobby = new();
                        o_taskLobby = null;
                    }

                    if (o_communicationLobby != null)
                    {
                        try
                        {
                            o_communicationLobby.Stop();
                            Thread.Sleep(2500);
                            o_communicationLobby = null;
                        }
                        catch (Exception)
                        {

                        }

                        finally
                        {
                            o_communicationLobby = null;
                        }
                    }

                    if (o_taskChatReceive != null)
                    {
                        o_cancellationTokenSourceChatReceive?.Cancel();
                        Thread.Sleep(500);
                        o_cancellationTokenSourceChatReceive = new();
                        o_taskChatReceive = null;
                    }

                    if (o_communicationChat != null)
                    {
                        try
                        {
                            o_communicationChat.Stop();
                            Thread.Sleep(2500);
                            o_communicationChat = null;
                        }
                        catch (Exception)
                        {

                        }
                        finally
                        {
                            o_communicationChat = null;
                        }
                    }

                    return ForestNET.Lib.StateMachine.EXIT;
                }
            ));

            /* execute state machine until EXIT state */
            string s_returnCode = ForestNET.Lib.StateMachine.EXIT;
            string s_currentState = "LOBBY";
            List<object> a_genericList = [];

            do
            {
                s_returnCode = o_stateMachine.ExecuteStateMethod(s_currentState, a_genericList);
                s_currentState = o_stateMachine.LookupTransitions(s_currentState, s_returnCode);
            } while (!s_returnCode.Equals(ForestNET.Lib.StateMachine.EXIT));
        }

        private class NetChatLobbyConfig
        {
            public string currentDirectory = string.Empty;
            public string chatUser = string.Empty;
            public string chatRoom = string.Empty;
            public string hostIp = string.Empty;
            public bool isServer;
            public string localIp = string.Empty;
            public int serverPort;
            public int udpMulticastTTL;
            public string udpMulticastIp = string.Empty;
            public int udpMulticastPort;
        }

        private static NetChatLobbyConfig ReadNetConfig(string p_s_currentDirectory)
        {
            string s_currentDirectory = p_s_currentDirectory;
            string s_resourcesNetDirectory = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "Resources" + ForestNET.Lib.IO.File.DIR + "net" + ForestNET.Lib.IO.File.DIR;
            string s_netConfigFile = "netChatLobbyConfig.txt";

            if (!ForestNET.Lib.IO.File.Exists(s_resourcesNetDirectory + s_netConfigFile))
            {
                throw new Exception("file[" + s_resourcesNetDirectory + s_netConfigFile + "] does not exists");
            }

            ForestNET.Lib.IO.File o_netConfigFile = new(s_resourcesNetDirectory + s_netConfigFile, false);

            if (o_netConfigFile.FileLines < 5)
            {
                throw new Exception("invalid config file[" + s_resourcesNetDirectory + s_netConfigFile + "]; must have at least '5 lines', but has '" + o_netConfigFile.FileLines + " lines'");
            }

            NetChatLobbyTest.NetChatLobbyConfig o_netConfig = new()
            {
                currentDirectory = p_s_currentDirectory,
                udpMulticastTTL = 1
            };

            for (int i = 1; i <= o_netConfigFile.FileLines; i++)
            {
                string s_line = o_netConfigFile.ReadLine(i);

                if (i == 1)
                {
                    if (!s_line.StartsWith("localIp"))
                    {
                        throw new Exception("Line #" + i + " does not start with 'localIp'");
                    }

                    string[] a_split = s_line.Split("=");

                    if (a_split.Length != 2)
                    {
                        throw new Exception("Invalid key value pair for 'localIp': '" + s_line + "'");
                    }

                    if (ForestNET.Lib.Helper.IsStringEmpty(a_split[1].Trim()))
                    {
                        throw new Exception("Invalid empty value, for 'localIp'");
                    }

                    o_netConfig.localIp = a_split[1].Trim();
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

                    o_netConfig.serverPort = int.Parse(a_split[1].Trim());
                }
                else if (i == 3)
                {
                    if (!s_line.StartsWith("udpMulticastTTL"))
                    {
                        throw new Exception("Line #" + i + " does not start with 'udpMulticastTTL'");
                    }

                    string[] a_split = s_line.Split("=");

                    if (a_split.Length != 2)
                    {
                        throw new Exception("Invalid key value pair for 'udpMulticastTTL': '" + s_line + "'");
                    }

                    if (!ForestNET.Lib.Helper.IsInteger(a_split[1].Trim()))
                    {
                        throw new Exception("Invalid value for 'udpMulticastTTL': '" + a_split[1].Trim() + "' is not an integer");
                    }

                    o_netConfig.udpMulticastTTL = int.Parse(a_split[1].Trim());
                }
                else if (i == 4)
                {
                    if (!s_line.StartsWith("udpMulticastIp"))
                    {
                        throw new Exception("Line #" + i + " does not start with 'udpMulticastIp'");
                    }

                    string[] a_split = s_line.Split("=");

                    if (a_split.Length != 2)
                    {
                        throw new Exception("Invalid key value pair for 'udpMulticastIp': '" + s_line + "'");
                    }

                    if (ForestNET.Lib.Helper.IsStringEmpty(a_split[1].Trim()))
                    {
                        throw new Exception("Invalid empty value, for 'udpMulticastIp'");
                    }

                    o_netConfig.udpMulticastIp = a_split[1].Trim();
                }
                else if (i == 5)
                {
                    if (!s_line.StartsWith("udpMulticastPort"))
                    {
                        throw new Exception("Line #" + i + " does not start with 'udpMulticastPort'");
                    }

                    string[] a_split = s_line.Split("=");

                    if (a_split.Length != 2)
                    {
                        throw new Exception("Invalid key value pair for 'udpMulticastPort': '" + s_line + "'");
                    }

                    if (!ForestNET.Lib.Helper.IsInteger(a_split[1].Trim()))
                    {
                        throw new Exception("Invalid value for 'udpMulticastPort': '" + a_split[1].Trim() + "' is not an integer");
                    }

                    o_netConfig.udpMulticastPort = int.Parse(a_split[1].Trim());
                }
            }

            Console.WriteLine("++++++++++++++++++++++++++++++++");
            Console.WriteLine("+ NET chat lobby config read   +");
            Console.WriteLine("++++++++++++++++++++++++++++++++");

            Console.WriteLine("");

            Console.WriteLine("local ip" + "\t\t" + o_netConfig.localIp);
            Console.WriteLine("server port" + "\t\t" + o_netConfig.serverPort);
            Console.WriteLine("udp multicast TTL" + "\t\t" + o_netConfig.udpMulticastTTL);
            Console.WriteLine("udp multicast ip" + "\t\t" + o_netConfig.udpMulticastIp);
            Console.WriteLine("udp multicast port" + "\t\t" + o_netConfig.udpMulticastPort);

            Console.WriteLine("");

            return o_netConfig;
        }

        private static ForestNET.Lib.Net.Sock.Com.Config GetCommunicationConfig(string p_s_currentDirectory, ForestNET.Lib.Net.Sock.Com.Type p_e_comType, ForestNET.Lib.Net.Sock.Com.Cardinality p_e_comCardinality, string p_s_host, int p_i_port, string? p_s_localHost, int p_i_localPort, bool p_b_symmetricSecurity128, bool p_b_symmetricSecurity256, bool p_b_asymmetricSecurity, bool p_b_highSecurity, NetTest.AsymmetricModes p_e_asymmetricMode, bool p_b_useMarshalling, bool p_b_useMarshallingWholeObject, int p_i_marshallingDataLengthInBytes, bool p_b_marshallingUseProperties, string? p_s_marshallingOverrideMessageType, bool p_b_marshallingSystemUsesLittleEndian, string? p_s_thumbprintServerCertificate)
        {
            string s_resourcesDirectory = p_s_currentDirectory + "Resources" + ForestNET.Lib.IO.File.DIR + "com" + ForestNET.Lib.IO.File.DIR;

            if ((p_b_asymmetricSecurity) && (!ForestNET.Lib.IO.File.FolderExists(s_resourcesDirectory)))
            {
                throw new Exception("cannot find directory '" + s_resourcesDirectory + "' where files are needed for asymmetric security communication");
            }

            int i_comAmount = 1;
            int i_comMessageBoxLength = 1500;
            int i_comSenderTimeoutMs = 10000;
            int i_comReceiverTimeoutMs = 10000;
            int i_comSenderIntervalMs = 25;
            int i_comQueueTimeoutMs = 25;
            int i_comUDPReceiveAckTimeoutMs = 300;
            int i_comUDPSendAckTimeoutMs = 125;
            string s_comSecretPassphrase = "4sgsh0ni5uo90dw2bqhn66mokasicqcvjebb";

            ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig;

            if (p_e_comCardinality == ForestNET.Lib.Net.Sock.Com.Cardinality.EqualBidirectional)
            {
                o_communicationConfig = new(p_e_comType, p_e_comCardinality)
                {
                    SocketReceiveType = ForestNET.Lib.Net.Sock.Recv.ReceiveType.SERVER,
                    AmountSockets = 1,
                    AmountMessageBoxes = 2,
                    SenderTimeoutMilliseconds = i_comSenderTimeoutMs,
                    ReceiverTimeoutMilliseconds = i_comReceiverTimeoutMs,
                    SenderIntervalMilliseconds = i_comSenderIntervalMs,
                    QueueTimeoutMilliseconds = i_comQueueTimeoutMs,
                    UDPReceiveAckTimeoutMilliseconds = i_comUDPReceiveAckTimeoutMs,
                    UDPSendAckTimeoutMilliseconds = i_comUDPSendAckTimeoutMs
                };

                o_communicationConfig.AddMessageBoxLength(i_comMessageBoxLength);
                o_communicationConfig.AddMessageBoxLength(i_comMessageBoxLength);
            }
            else
            {
                o_communicationConfig = new(p_e_comType, p_e_comCardinality)
                {
                    SocketReceiveType = ForestNET.Lib.Net.Sock.Recv.ReceiveType.SERVER,
                    Amount = i_comAmount,
                    SenderTimeoutMilliseconds = i_comSenderTimeoutMs,
                    ReceiverTimeoutMilliseconds = i_comReceiverTimeoutMs,
                    SenderIntervalMilliseconds = i_comSenderIntervalMs,
                    QueueTimeoutMilliseconds = i_comQueueTimeoutMs,
                    UDPReceiveAckTimeoutMilliseconds = i_comUDPReceiveAckTimeoutMs,
                    UDPSendAckTimeoutMilliseconds = i_comUDPSendAckTimeoutMs
                };

                o_communicationConfig.AddMessageBoxLength(i_comMessageBoxLength);
            }

            o_communicationConfig.AddHostAndPort(new KeyValuePair<string, int>(p_s_host, p_i_port));

            if (!ForestNET.Lib.Helper.IsStringEmpty(p_s_localHost))
            {
                o_communicationConfig.LocalAddress = p_s_localHost;
            }

            if (p_i_localPort > 0)
            {
                o_communicationConfig.LocalPort = p_i_localPort;
            }

            if (p_b_symmetricSecurity128)
            {
                if (p_b_highSecurity)
                {
                    o_communicationConfig.CommunicationSecurity = ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_HIGH;
                }
                else
                {
                    o_communicationConfig.CommunicationSecurity = ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_LOW;
                }

                o_communicationConfig.CommonSecretPassphrase = s_comSecretPassphrase;
            }
            else if (p_b_symmetricSecurity256)
            {
                if (p_b_highSecurity)
                {
                    o_communicationConfig.CommunicationSecurity = ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_HIGH;
                }
                else
                {
                    o_communicationConfig.CommunicationSecurity = ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_LOW;
                }

                o_communicationConfig.CommonSecretPassphrase = s_comSecretPassphrase;
            }
            else if (p_b_asymmetricSecurity)
            {
                if (p_e_asymmetricMode == NetTest.AsymmetricModes.FileNoPassword)
                {
                    o_communicationConfig.PathToCertificateFile = s_resourcesDirectory + "server" + ForestNET.Lib.IO.File.DIR + "cert-server-no-pw.pfx";
                }
                else if (p_e_asymmetricMode == NetTest.AsymmetricModes.FileWithPassword)
                {
                    o_communicationConfig.PathToCertificateFile = s_resourcesDirectory + "server" + ForestNET.Lib.IO.File.DIR + "cert-server-with-pw.pfx";
                    o_communicationConfig.PathToCertificateFilePassword = "123456";
                }
                else if (p_e_asymmetricMode == NetTest.AsymmetricModes.ThumbprintInStore)
                {
                    o_communicationConfig.CertificateThumbprint = p_s_thumbprintServerCertificate;
                    o_communicationConfig.CertificateStoreName = System.Security.Cryptography.X509Certificates.StoreName.Root;
                    o_communicationConfig.CertificateStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser;
                }
                else if (p_e_asymmetricMode == NetTest.AsymmetricModes.ThumbprintInStoreAndClientRemoteCertificateName)
                {
                    o_communicationConfig.CertificateThumbprint = p_s_thumbprintServerCertificate;
                    o_communicationConfig.CertificateStoreName = System.Security.Cryptography.X509Certificates.StoreName.Root;
                    o_communicationConfig.CertificateStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser;
                    o_communicationConfig.ClientRemoteCertificateName = "forestNET Sever PW";
                }

                o_communicationConfig.CommunicationSecurity = ForestNET.Lib.Net.Sock.Com.Security.ASYMMETRIC;
            }

            o_communicationConfig.UseMarshalling = p_b_useMarshalling;
            o_communicationConfig.UseMarshallingWholeObject = p_b_useMarshallingWholeObject;
            o_communicationConfig.MarshallingDataLengthInBytes = p_i_marshallingDataLengthInBytes;
            o_communicationConfig.MarshallingUseProperties = p_b_marshallingUseProperties;
            o_communicationConfig.MarshallingOverrideMessageType = p_s_marshallingOverrideMessageType;
            o_communicationConfig.MarshallingSystemUsesLittleEndian = p_b_marshallingSystemUsesLittleEndian;

            o_communicationConfig.DebugNetworkTrafficOn = false;

            return o_communicationConfig;
        }

        private static void NetLobby(NetChatLobbyConfig p_o_config)
        {
            if (o_communicationLobby != null)
            {
                try
                {
                    o_communicationLobby.Stop();
                }
                catch (Exception)
                {

                }
            }

            o_communicationLobby = null;

            bool b_symmetricSecurity128 = false;
            bool b_symmetricSecurity256 = true;
            bool b_asymmetricSecurity = false;
            bool b_highSecurity = false;

            bool b_useMarshalling = true;
            bool b_useMarshallingWholeObject = false;
            int i_marshallingDataLengthInBytes = 2;
            bool b_marshallingUseProperties = false;
            string? s_marshallingOverrideMessageType = null;
            bool b_marshallingSystemUsesLittleEndian = false;

            try
            {
                /* interrupt and null task lobby if it is still running */
                if (o_taskLobby != null)
                {
                    o_cancellationTokenSourceLobby?.Cancel();
                    Thread.Sleep(500);
                    o_cancellationTokenSourceLobby = new();
                    o_taskLobby = null;
                }

                if (p_o_config.isServer)
                { /* SERVER */
                    ForestNET.Lib.Net.Sock.Com.Type e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND;
                    ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(p_o_config.currentDirectory, e_type, ForestNET.Lib.Net.Sock.Com.Cardinality.Equal, p_o_config.udpMulticastIp, p_o_config.udpMulticastPort, null, 0, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, NetTest.AsymmetricModes.None, b_useMarshalling, b_useMarshallingWholeObject, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian, null);
                    o_communicationConfig.UDPIsMulticastSocket = true;
                    o_communicationConfig.UDPMulticastTTL = p_o_config.udpMulticastTTL;
                    o_communicationLobby = new(o_communicationConfig);
                    o_communicationLobby.Start();

                    o_taskLobby = Task.Run(() =>
                    {
                        try
                        {
                            while (true)
                            {
                                while (!o_communicationLobby.Enqueue(
                                    p_o_config.chatRoom + "|" + p_o_config.localIp + ":" + p_o_config.serverPort
                                ))
                                {
                                    ForestNET.Lib.Global.ILogWarning("could not enqueue message");
                                }

                                ForestNET.Lib.Global.ILog("message enqueued: '" + p_o_config.chatRoom + "|" + p_o_config.localIp + ":" + p_o_config.serverPort + "'");

                                Thread.Sleep(1000);
                            }
                        }
                        catch (InvalidOperationException)
                        {
                            /* ignore if communication is not running */
                        }
                        catch (Exception o_exc)
                        {
                            ForestNET.Lib.Global.LogException(o_exc);
                        }
                    }, o_cancellationTokenSourceLobby?.Token ?? throw new NullReferenceException("CancellationTokenSource for Lobby is null"));
                }
                else
                { /* CLIENT */
                    ForestNET.Lib.Net.Sock.Com.Type e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE;
                    ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(p_o_config.currentDirectory, e_type, ForestNET.Lib.Net.Sock.Com.Cardinality.Equal, p_o_config.udpMulticastIp, p_o_config.udpMulticastPort, null, 0, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, NetTest.AsymmetricModes.None, b_useMarshalling, b_useMarshallingWholeObject, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian, null);
                    o_communicationConfig.UDPIsMulticastSocket = true;
                    o_communicationLobby = new(o_communicationConfig);
                    o_communicationLobby.Start();

                    o_taskLobby = Task.Run(() =>
                    {
                        try
                        {
                            while (true)
                            {
                                List<DateTime> a_deleteEntries = [];

                                foreach (KeyValuePair<DateTime, string> o_entry in m_clientLobbyEntries)
                                {
                                    if (DateTime.Now.Subtract(o_entry.Key).Seconds > 30)
                                    {
                                        a_deleteEntries.Add(o_entry.Key);
                                    }
                                }

                                if (a_deleteEntries.Count > 0)
                                {
                                    foreach (DateTime o_key in a_deleteEntries)
                                    {
                                        m_clientLobbyEntries.Remove(o_key);
                                    }
                                }

                                string? s_connectionInfo = null;

                                do
                                {
                                    s_connectionInfo = (string?)o_communicationLobby.Dequeue();

                                    if (s_connectionInfo != null)
                                    {
                                        ForestNET.Lib.Global.ILog("message received: '" + s_connectionInfo + "'");

                                        if (!s_connectionInfo.Contains(':'))
                                        {
                                            continue;
                                        }

                                        int i_readingPort = int.Parse(s_connectionInfo.Split(":")[1]);

                                        if (i_readingPort != p_o_config.serverPort)
                                        {
                                            continue;
                                        }

                                        if (m_clientLobbyEntries.ContainsValue(s_connectionInfo))
                                        {
                                            DateTime? o_key = null;

                                            foreach (KeyValuePair<DateTime, string> o_entry in m_clientLobbyEntries)
                                            {
                                                if (o_entry.Value.Equals(s_connectionInfo))
                                                {
                                                    o_key = o_entry.Key;
                                                }
                                            }

                                            if (o_key != null)
                                            {
                                                m_clientLobbyEntries.Remove(o_key ?? throw new NullReferenceException("DateTime instance is null"));
                                            }
                                        }

                                        m_clientLobbyEntries[DateTime.Now] = s_connectionInfo;
                                    }
                                } while (s_connectionInfo != null);

                                Thread.Sleep(1000);
                            }
                        }
                        catch (InvalidOperationException)
                        {
                            /* ignore if communication is not running */
                        }
                        catch (Exception o_exc)
                        {
                            ForestNET.Lib.Global.LogException(o_exc);
                        }
                    }, o_cancellationTokenSourceLobby?.Token ?? throw new NullReferenceException("CancellationTokenSource for Lobby is null"));
                }
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.LogException(o_exc);
            }
        }

        private static void NetChat(NetChatLobbyConfig p_o_config)
        {
            if (o_communicationChat != null)
            {
                try
                {
                    o_communicationChat.Stop();
                }
                catch (Exception)
                {

                }
            }

            o_communicationChat = null;

            bool b_symmetricSecurity128 = false;
            bool b_symmetricSecurity256 = true;
            bool b_asymmetricSecurity = false;
            bool b_highSecurity = false;

            bool b_useMarshalling = true;
            bool b_useMarshallingWholeObject = false;
            int i_marshallingDataLengthInBytes = 2;
            bool b_marshallingUseProperties = false;
            string? s_marshallingOverrideMessageType = null;
            bool b_marshallingSystemUsesLittleEndian = false;

            try
            {
                /* interrupt and null task chat receive if it is still running */
                if (o_taskChatReceive != null)
                {
                    o_cancellationTokenSourceChatReceive?.Cancel();
                    Thread.Sleep(500);
                    o_cancellationTokenSourceChatReceive = new();
                    o_taskChatReceive = null;
                }

                if (p_o_config.isServer)
                { /* SERVER */
                    ForestNET.Lib.Net.Sock.Com.Type e_type = ForestNET.Lib.Net.Sock.Com.Type.TCP_RECEIVE_WITH_ANSWER;
                    ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(p_o_config.currentDirectory, e_type, ForestNET.Lib.Net.Sock.Com.Cardinality.Equal, p_o_config.localIp, p_o_config.serverPort, null, 0, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, NetTest.AsymmetricModes.None, b_useMarshalling, b_useMarshallingWholeObject, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian, null);

                    ReceiveSocketTask o_receiveSocketTask = new();
                    o_communicationConfig.AddReceiveSocketTask(o_receiveSocketTask);

                    o_communicationChat = new(o_communicationConfig);
                    o_communicationChat.Start();
                }
                else
                { /* CLIENT */
                    ForestNET.Lib.Net.Sock.Com.Type e_type = ForestNET.Lib.Net.Sock.Com.Type.TCP_SEND_WITH_ANSWER;
                    ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(p_o_config.currentDirectory, e_type, ForestNET.Lib.Net.Sock.Com.Cardinality.Equal, p_o_config.hostIp, p_o_config.serverPort, null, 0, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, NetTest.AsymmetricModes.None, b_useMarshalling, b_useMarshallingWholeObject, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian, null);
                    o_communicationChat = new(o_communicationConfig);
                    o_communicationChat.Start();

                    o_taskChatReceive = Task.Run(() =>
                    {
                        try
                        {
                            while (true)
                            {
                                /* prepare request */
                                string? s_request = "";

                                if ((b_connected) && (o_messageBox.MessageAmount > 0))
                                {
                                    string? s_message = null;

                                    do
                                    {
                                        s_message = (string?)o_messageBox.DequeueObject();

                                        if (s_message != null)
                                        {
                                            s_request += s_message + "~";
                                        }
                                    } while (s_message != null);
                                }

                                s_request += ForestNET.Lib.Helper.ToISO8601UTC(DateTime.Now) + "|" + p_o_config.chatUser + "|%PING%";

                                /* send request */
                                while (!o_communicationChat.Enqueue(
                                    s_request
                                ))
                                {
                                    ForestNET.Lib.Global.ILogWarning("could not enqueue message");
                                }

                                ForestNET.Lib.Global.ILogFine("message enqueued");

                                /* wait for answer */
                                object? o_answer = o_communicationChat.DequeueWithWaitLoop(5000);

                                if (o_answer != null)
                                {
                                    /* evaluate answer */
                                    string[] a_messages = ((string)o_answer).Split('~');

                                    foreach (string s_message in a_messages)
                                    {
                                        ForestNET.Lib.Global.ILog("message received: '" + s_message + "'");

                                        if (!b_connected)
                                        {
                                            continue;
                                        }

                                        String[] a_messageParts = s_message.Split('|');

                                        if (a_messageParts.Length != 3)
                                        {
                                            continue;
                                        }

                                        DateTime o_ldtFoo = ForestNET.Lib.Helper.FromISO8601UTC(a_messageParts[0]);
                                        string s_foo = a_messageParts[1];
                                        string s_bar = a_messageParts[2];

                                        if (s_bar.Equals("%EXIT%"))
                                        {
                                            m_chatHistory[DateTime.Now] = new Tuple<string, string>(o_netConfig?.chatUser ?? "UNKNWON_USER", "%EXIT%");
                                            b_connected = false;
                                        }
                                        else if (s_bar.Equals("%PING%"))
                                        {
                                            b_connected = true;
                                            o_lastPing = DateTime.Now;
                                        }
                                        else
                                        {
                                            m_chatHistory[o_ldtFoo] = new Tuple<string, string>(s_foo, s_bar);
                                        }
                                    }
                                }
                                else
                                {
                                    ForestNET.Lib.Global.ILogWarning("could not receive any answer data");
                                }

                                Thread.Sleep(1000);
                            }
                        }
                        catch (InvalidOperationException)
                        {
                            /* ignore if communication is not running */
                        }
                        catch (Exception o_exc)
                        {
                            ForestNET.Lib.Global.LogException(o_exc);
                        }
                    }, o_cancellationTokenSourceChatReceive?.Token ?? throw new NullReferenceException("CancellationTokenSource for Chat Receive is null"));
                }
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.LogException(o_exc);
            }
        }

        private static void RenderChat(NetChatLobbyConfig p_o_config)
        {
            SortedDictionary<DateTime, Tuple<string, string>> m_sorted = new(m_chatHistory);

            foreach (KeyValuePair<DateTime, Tuple<string, string>> o_chatEntry in m_sorted)
            {
                if (o_chatEntry.Value.Item1.Equals(p_o_config.chatUser))
                {
                    Console.WriteLine(o_chatEntry.Key + " - " + o_chatEntry.Value.Item1);
                    Console.WriteLine(o_chatEntry.Value.Item2);
                }
                else
                {
                    Console.WriteLine("\t\t\t\t" + o_chatEntry.Key + " - " + o_chatEntry.Value.Item1);
                    Console.WriteLine("\t\t\t\t" + o_chatEntry.Value.Item2);
                }
            }
        }

        private static void ClearMessageBox()
        {
            if (o_messageBox.MessageAmount > 0)
            {
                object? o_foo;

                do
                {
                    o_foo = (object?)o_messageBox.DequeueObject();
                } while (o_foo != null);
            }
        }

        internal class ReceiveSocketTask : ForestNET.Lib.Net.Sock.Task.Task
        {
            /* parameterless constructor for TCP RunServer method */
            public ReceiveSocketTask() : base(ForestNET.Lib.Net.Sock.Type.TCP_TLS_SERVER)
            {

            }

            public ReceiveSocketTask(ForestNET.Lib.Net.Sock.Type p_e_type) : base(p_e_type)
            {

            }

            public override void CloneFromOtherTask(ForestNET.Lib.Net.Sock.Task.Task p_o_sourceTask)
            {
                this.CloneBasicProperties(p_o_sourceTask);
            }

            public override async Task RunTask()
            {
                try
                {
                    /* get request object */
                    string? s_request = (string?)this.RequestObject;

                    /* evaluate request */
                    if (s_request != null)
                    {
                        string[] a_messages = s_request.Split('~');

                        foreach (string s_message in a_messages)
                        {
                            ForestNET.Lib.Global.ILog("message received: '" + s_message + "'");

                            String[] a_messageParts = s_message.Split('|');

                            if (a_messageParts.Length != 3)
                            {
                                continue;
                            }

                            DateTime o_ldtFoo = ForestNET.Lib.Helper.FromISO8601UTC(a_messageParts[0]);
                            string s_foo = a_messageParts[1];
                            string s_bar = a_messageParts[2];

                            if (s_bar.Equals("%EXIT%"))
                            {
                                m_chatHistory[DateTime.Now] = new Tuple<string, string>(o_netConfig?.chatUser ?? "UNKNWON_USER", "%EXIT%");
                                b_connected = false;
                                o_lastPing = null;
                                return;
                            }
                            else if (s_bar.Equals("%PING%"))
                            {
                                b_connected = true;
                                o_lastPing = DateTime.Now;
                            }
                            else
                            {
                                m_chatHistory[o_ldtFoo] = new Tuple<string, string>(s_foo, s_bar);
                            }
                        }
                    }

                    string s_answer = "";

                    if ((b_connected) && (o_messageBox.MessageAmount > 0))
                    {
                        string? s_message = null;

                        do
                        {
                            s_message = (string?)o_messageBox.DequeueObject();

                            if (s_message != null)
                            {
                                s_answer += s_message + "~";
                            }
                        } while (s_message != null);
                    }

                    s_answer += ForestNET.Lib.Helper.ToISO8601UTC(DateTime.Now) + "|" + o_netConfig?.chatUser + "|%PING%";

                    /* set answer object */
                    this.AnswerObject = s_answer;
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.LogException(o_exc);
                }

                await DoNoting();
            }
        }
    }
}
