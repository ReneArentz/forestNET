namespace Sandbox.Tests.Net
{
    public class NetTest
    {
        /**
		 * !!!!!!!!!!!!!!!!!
		 * it is maybe necessary to update server and client thumbprints in netConfig.txt
		 * use the .pfx files for local certificate import from script generation
		 * import certificates for current user and to 'Trusted Root Certification Authorities'
		 * !!!!!!!!!!!!!!!!!
		 */

        internal enum AsymmetricModes
        {
            None,
            FileNoPassword,
            FileWithPassword,
            ThumbprintInStore,
            ThumbprintInStoreAndClientRemoteCertificateName
        }

        private const string ThumbPrintServerCertificateName = "ForestNET Server PW";
        private const string ThumbPrintClientCertificateName = "ForestNET Client PW";

        /* sleep multiplier for test cycle executions */
        private static int i_sleepMultiplier = 8;

        public static void TestNetMenu(string p_s_currentDirectory)
        {
            NetTest.NetConfig o_netConfig = NetTest.ReadNetConfig(p_s_currentDirectory);

            o_netConfig.isServer = ((ForestNET.Lib.Console.ConsoleInputCharacter("Run as server[y|n]: ") ?? 'n') == 'y');

            Console.WriteLine("");

            int i_input;

            do
            {
                Console.WriteLine("++++++++++++++++++++++++++++++++");
                Console.WriteLine("+ test NET as " + ((o_netConfig.isServer) ? "Server" : "Client") + "           +");
                Console.WriteLine("++++++++++++++++++++++++++++++++");

                Console.WriteLine("");

                Console.WriteLine("[1] TCP Socket big file(100 MB) with progress bar");
                Console.WriteLine("[2] TCP Socket handshake");

                Console.WriteLine("[3] UDP Communication");
                Console.WriteLine("[4] UDP Multicast Communication");
                Console.WriteLine("[5] UDP Communication with Acknowledge");
                Console.WriteLine("[6] TCP Communication");

                Console.WriteLine("[7] TCP Communication with object transmission");
                Console.WriteLine("[8] TCP Communication with answer #1");
                Console.WriteLine("[9] TCP Communication with answer #2");

                Console.WriteLine("[10] UDP Communication unidirectional");
                Console.WriteLine("[11] UDP Communication with Acknowledge unidirectional");
                Console.WriteLine("[12] TCP Communication unidirectional");

                Console.WriteLine("[13] UDP Communication bidirectional");
                Console.WriteLine("[14] UDP Communication with Acknowledge bidirectional");
                Console.WriteLine("[15] TCP Communication bidirectional");

                Console.WriteLine("[16] UDP Communication marshalling small object");
                Console.WriteLine("[17] UDP Communication marshalling small object with Acknowledge");
                Console.WriteLine("[18] TCP Communication marshalling small object");
                Console.WriteLine("[19] UDP Communication marshalling object");
                Console.WriteLine("[20] UDP Communication marshalling object with Acknowledge");
                Console.WriteLine("[21] TCP Communication marshalling object");

                Console.WriteLine("[22] UDP Communication marshalling small object shared memory unidirectional");
                Console.WriteLine("[23] UDP Communication marshalling small object with Acknowledge shared memory unidirectional");
                Console.WriteLine("[24] TCP Communication marshalling small object shared memory unidirectional");
                Console.WriteLine("[25] UDP Communication marshalling object shared memory unidirectional");
                Console.WriteLine("[26] UDP Communication marshalling object with Acknowledge shared memory unidirectional");
                Console.WriteLine("[27] TCP Communication marshalling object shared memory unidirectional");

                Console.WriteLine("[28] UDP Communication marshalling small object shared memory bidirectional");
                Console.WriteLine("[29] UDP Communication marshalling small object with Acknowledge shared memory bidirectional");
                Console.WriteLine("[30] TCP Communication marshalling small object shared memory bidirectional");
                Console.WriteLine("[31] UDP Communication marshalling object shared memory bidirectional");
                Console.WriteLine("[32] UDP Communication marshalling object with Acknowledge shared memory bidirectional");
                Console.WriteLine("[33] TCP Communication marshalling object shared memory bidirectional");

                Console.WriteLine("[34] Run Tests #2 - #15 with current settings");
                Console.WriteLine("[0] quit");

                Console.WriteLine("");

                i_input = ForestNET.Lib.Console.ConsoleInputInteger("Enter menu number[1-34;0]: ", "Invalid input.", "Please enter a value[1-34;0].") ?? 0;

                /* do handshake before any test for synchronisation purpose, but not [2] and [4] */
                if ((i_input >= 1) && (i_input <= 34) && (!((i_input == 2) || (i_input == 4))))
                {
                    NetTest.NetTCPSockHandshakeTest(o_netConfig);
                }

                if (i_input == 1)
                {
                    NetTest.NetTCPSockBigFileTest(o_netConfig);
                }
                else if (i_input == 2)
                {
                    NetTest.NetTCPSockHandshakeTest(o_netConfig);
                }
                else if (i_input == 3)
                {
                    NetTest.NetCommunication(false, false, NetTest.AsymmetricModes.None, o_netConfig);
                }
                else if (i_input == 4)
                {
                    NetTest.NetCommunicationUDPMulticast(o_netConfig, true);
                }
                else if (i_input == 5)
                {
                    NetTest.NetCommunication(false, true, NetTest.AsymmetricModes.None, o_netConfig);
                }
                else if (i_input == 6)
                {
                    NetTest.NetCommunication(true, false, (o_netConfig.asymmetricSecurity) ? NetTest.AsymmetricModes.None : NetTest.AsymmetricModes.ThumbprintInStore, o_netConfig);
                }
                else if (i_input == 7)
                {
                    NetTest.NetCommunicationObjectTransmissionTCP((o_netConfig.asymmetricSecurity) ? NetTest.AsymmetricModes.None : NetTest.AsymmetricModes.FileNoPassword, o_netConfig);
                }
                else if (i_input == 8)
                {
                    NetTest.NetCommunicationWithAnswerOneTCP((o_netConfig.asymmetricSecurity) ? NetTest.AsymmetricModes.None : NetTest.AsymmetricModes.FileWithPassword, o_netConfig);
                }
                else if (i_input == 9)
                {
                    NetTest.NetCommunicationWithAnswerTwoTCP((o_netConfig.asymmetricSecurity) ? NetTest.AsymmetricModes.None : NetTest.AsymmetricModes.ThumbprintInStoreAndClientRemoteCertificateName, o_netConfig);
                }
                else if (i_input == 10)
                {
                    NetTest.NetCommunicationSharedMemoryUniDirectional(false, false, NetTest.AsymmetricModes.None, o_netConfig);
                }
                else if (i_input == 11)
                {
                    NetTest.NetCommunicationSharedMemoryUniDirectional(false, true, NetTest.AsymmetricModes.None, o_netConfig);
                }
                else if (i_input == 12)
                {
                    NetTest.NetCommunicationSharedMemoryUniDirectional(true, false, (o_netConfig.asymmetricSecurity) ? NetTest.AsymmetricModes.None : NetTest.AsymmetricModes.ThumbprintInStore, o_netConfig);
                }
                else if (i_input == 13)
                {
                    NetTest.NetCommunicationSharedMemoryBiDirectional(false, false, NetTest.AsymmetricModes.None, o_netConfig);
                }
                else if (i_input == 14)
                {
                    NetTest.NetCommunicationSharedMemoryBiDirectional(false, true, NetTest.AsymmetricModes.None, o_netConfig);
                }
                else if (i_input == 15)
                {
                    NetTest.NetCommunicationSharedMemoryBiDirectional(true, false, (o_netConfig.asymmetricSecurity) ? NetTest.AsymmetricModes.None : NetTest.AsymmetricModes.FileNoPassword, o_netConfig);
                }
                else if (i_input == 16)
                {
                    NetTest.NetCommunicationMarshallingObject(false, false, true, NetTest.AsymmetricModes.None, o_netConfig);
                }
                else if (i_input == 17)
                {
                    NetTest.NetCommunicationMarshallingObject(false, true, true, NetTest.AsymmetricModes.None, o_netConfig);
                }
                else if (i_input == 18)
                {
                    NetTest.NetCommunicationMarshallingObject(true, false, true, (o_netConfig.asymmetricSecurity) ? NetTest.AsymmetricModes.None : NetTest.AsymmetricModes.FileWithPassword, o_netConfig);
                }
                else if (i_input == 19)
                {
                    NetTest.NetCommunicationMarshallingObject(false, false, false, NetTest.AsymmetricModes.None, o_netConfig);
                }
                else if (i_input == 20)
                {
                    NetTest.NetCommunicationMarshallingObject(false, true, false, NetTest.AsymmetricModes.None, o_netConfig);
                }
                else if (i_input == 21)
                {
                    NetTest.NetCommunicationMarshallingObject(true, false, false, (o_netConfig.asymmetricSecurity) ? NetTest.AsymmetricModes.None : NetTest.AsymmetricModes.ThumbprintInStoreAndClientRemoteCertificateName, o_netConfig);
                }
                else if (i_input == 22)
                {
                    NetTest.NetCommunicationMarshallingSharedMemoryUniDirectional(false, false, true, NetTest.AsymmetricModes.None, o_netConfig);
                }
                else if (i_input == 23)
                {
                    NetTest.NetCommunicationMarshallingSharedMemoryUniDirectional(false, true, true, NetTest.AsymmetricModes.None, o_netConfig);
                }
                else if (i_input == 24)
                {
                    NetTest.NetCommunicationMarshallingSharedMemoryUniDirectional(true, false, true, (o_netConfig.asymmetricSecurity) ? NetTest.AsymmetricModes.None : NetTest.AsymmetricModes.ThumbprintInStore, o_netConfig);
                }
                else if (i_input == 25)
                {
                    NetTest.NetCommunicationMarshallingSharedMemoryUniDirectional(false, false, false, NetTest.AsymmetricModes.None, o_netConfig);
                }
                else if (i_input == 26)
                {
                    NetTest.NetCommunicationMarshallingSharedMemoryUniDirectional(false, true, false, NetTest.AsymmetricModes.None, o_netConfig);
                }
                else if (i_input == 27)
                {
                    NetTest.NetCommunicationMarshallingSharedMemoryUniDirectional(true, false, false, (o_netConfig.asymmetricSecurity) ? NetTest.AsymmetricModes.None : NetTest.AsymmetricModes.FileNoPassword, o_netConfig);
                }
                else if (i_input == 28)
                {
                    NetTest.NetCommunicationMarshallingSharedMemoryBiDirectional(false, false, true, NetTest.AsymmetricModes.None, o_netConfig);
                }
                else if (i_input == 29)
                {
                    NetTest.NetCommunicationMarshallingSharedMemoryBiDirectional(false, true, true, NetTest.AsymmetricModes.None, o_netConfig);
                }
                else if (i_input == 31)
                {
                    NetTest.NetCommunicationMarshallingSharedMemoryBiDirectional(true, false, true, (o_netConfig.asymmetricSecurity) ? NetTest.AsymmetricModes.None : NetTest.AsymmetricModes.FileWithPassword, o_netConfig);
                }
                else if (i_input == 31)
                {
                    NetTest.NetCommunicationMarshallingSharedMemoryBiDirectional(false, false, false, NetTest.AsymmetricModes.None, o_netConfig);
                }
                else if (i_input == 32)
                {
                    NetTest.NetCommunicationMarshallingSharedMemoryBiDirectional(false, true, false, NetTest.AsymmetricModes.None, o_netConfig);
                }
                else if (i_input == 33)
                {
                    NetTest.NetCommunicationMarshallingSharedMemoryBiDirectional(true, false, false, (o_netConfig.asymmetricSecurity) ? NetTest.AsymmetricModes.None : NetTest.AsymmetricModes.ThumbprintInStoreAndClientRemoteCertificateName, o_netConfig);
                }
                else if (i_input == 34)
                {
                    int i_timeoutMillisecondsForHandshake = 60000;

                    if (!o_netConfig.asymmetricSecurity)
                    {
                        NetTest.NetTCPSockHandshakeTest(o_netConfig, i_timeoutMillisecondsForHandshake);
                        NetTest.NetCommunication(false, false, NetTest.AsymmetricModes.None, o_netConfig);
                        NetTest.NetTCPSockHandshakeTest(o_netConfig, i_timeoutMillisecondsForHandshake);
                        NetTest.NetCommunication(false, true, NetTest.AsymmetricModes.None, o_netConfig);
                        NetTest.NetTCPSockHandshakeTest(o_netConfig, i_timeoutMillisecondsForHandshake);
                        NetTest.NetCommunicationUDPMulticast(o_netConfig, false);
                    }

                    NetTest.NetTCPSockHandshakeTest(o_netConfig, i_timeoutMillisecondsForHandshake);
                    NetTest.NetCommunication(true, false, (o_netConfig.asymmetricSecurity) ? NetTest.AsymmetricModes.None : NetTest.AsymmetricModes.ThumbprintInStore, o_netConfig);
                    NetTest.NetTCPSockHandshakeTest(o_netConfig, i_timeoutMillisecondsForHandshake);
                    NetTest.NetCommunicationObjectTransmissionTCP((o_netConfig.asymmetricSecurity) ? NetTest.AsymmetricModes.None : NetTest.AsymmetricModes.FileWithPassword, o_netConfig);
                    NetTest.NetTCPSockHandshakeTest(o_netConfig, i_timeoutMillisecondsForHandshake);
                    NetTest.NetCommunicationWithAnswerOneTCP((o_netConfig.asymmetricSecurity) ? NetTest.AsymmetricModes.None : NetTest.AsymmetricModes.FileNoPassword, o_netConfig);
                    NetTest.NetTCPSockHandshakeTest(o_netConfig, i_timeoutMillisecondsForHandshake);
                    NetTest.NetCommunicationWithAnswerTwoTCP((o_netConfig.asymmetricSecurity) ? NetTest.AsymmetricModes.None : NetTest.AsymmetricModes.ThumbprintInStoreAndClientRemoteCertificateName, o_netConfig);

                    if (!o_netConfig.asymmetricSecurity)
                    {
                        NetTest.NetTCPSockHandshakeTest(o_netConfig, i_timeoutMillisecondsForHandshake);
                        NetTest.NetCommunicationSharedMemoryUniDirectional(false, false, NetTest.AsymmetricModes.None, o_netConfig);
                        NetTest.NetTCPSockHandshakeTest(o_netConfig, i_timeoutMillisecondsForHandshake);
                        NetTest.NetCommunicationSharedMemoryUniDirectional(false, true, NetTest.AsymmetricModes.None, o_netConfig);
                    }

                    NetTest.NetTCPSockHandshakeTest(o_netConfig, i_timeoutMillisecondsForHandshake);
                    NetTest.NetCommunicationSharedMemoryUniDirectional(true, false, (o_netConfig.asymmetricSecurity) ? NetTest.AsymmetricModes.None : NetTest.AsymmetricModes.ThumbprintInStore, o_netConfig);

                    if (!o_netConfig.asymmetricSecurity)
                    {
                        NetTest.NetTCPSockHandshakeTest(o_netConfig, i_timeoutMillisecondsForHandshake);
                        NetTest.NetCommunicationSharedMemoryBiDirectional(false, false, NetTest.AsymmetricModes.None, o_netConfig);
                        NetTest.NetTCPSockHandshakeTest(o_netConfig, i_timeoutMillisecondsForHandshake);
                        NetTest.NetCommunicationSharedMemoryBiDirectional(false, true, NetTest.AsymmetricModes.None, o_netConfig);
                    }

                    NetTest.NetTCPSockHandshakeTest(o_netConfig, i_timeoutMillisecondsForHandshake);
                    NetTest.NetCommunicationSharedMemoryBiDirectional(true, false, (o_netConfig.asymmetricSecurity) ? NetTest.AsymmetricModes.None : NetTest.AsymmetricModes.FileWithPassword, o_netConfig);
                }

                if ((i_input >= 1) && (i_input <= 34))
                {
                    Console.WriteLine("");

                    ForestNET.Lib.Console.ConsoleInputString("Press any key to continue . . . ", true);

                    Console.WriteLine("");
                }

                Console.WriteLine("");

            } while (i_input != 0);
        }

        public class NetConfig
        {
            public bool isServer = false;
            public string serverIp = string.Empty;
            public int serverPort = 0;
            public string clientIp = string.Empty;
            public int clientPort = 0;
            public string serverBiIp = string.Empty;
            public int serverBiPort = 0;
            public string clientBiIp = string.Empty;
            public int clientBiPort = 0;
            public string clientLocalIp = string.Empty;
            public int clientLocalPort = 0;
            public int handshakePort = 0;
            public bool symmetricSecurity128 = false;
            public bool symmetricSecurity256 = false;
            public bool asymmetricSecurity = false;
            public bool highSecurity = false;
            public bool useMarshalling = false;
            public bool useMarshallingWholeObject = false;
            public int marshallingDataLengthInBytes = 0;
            public bool marshallingUseProperties = false;
            public string? marshallingOverrideMessageType;
            public bool marshallingSystemUsesLittleEndian = false;
            public int sleepMultiplier = 0;
            public string currentDirectory = string.Empty;
            public string thumbPrintServerCertificate = string.Empty;
            public string thumbPrintClientCertificate = string.Empty;
            public int udpMulticastTTL = 1;
        }

        public static NetConfig ReadNetConfig(String p_s_currentDirectory)
        {
            string s_currentDirectory = p_s_currentDirectory;
            string s_resourcesNetDirectory = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "Resources" + ForestNET.Lib.IO.File.DIR + "net" + ForestNET.Lib.IO.File.DIR;
            string s_netConfigFile = "netConfig.txt";

            if (!ForestNET.Lib.IO.File.Exists(s_resourcesNetDirectory + s_netConfigFile))
            {
                throw new Exception("file[" + s_resourcesNetDirectory + s_netConfigFile + "] does not exists");
            }

            ForestNET.Lib.IO.File o_netConfigFile = new(s_resourcesNetDirectory + s_netConfigFile, false);

            if (o_netConfigFile.FileLines < 11)
            {
                throw new Exception("invalid config file[" + s_resourcesNetDirectory + s_netConfigFile + "]; must have at least '11 lines', but has '" + o_netConfigFile.FileLines + " lines'");
            }

            NetTest.NetConfig o_netConfig = new()
            {
                symmetricSecurity128 = false,
                symmetricSecurity256 = false,
                asymmetricSecurity = false,
                highSecurity = false,
                useMarshalling = false,
                useMarshallingWholeObject = false,
                marshallingDataLengthInBytes = 1,
                marshallingUseProperties = false,
                marshallingOverrideMessageType = null,
                marshallingSystemUsesLittleEndian = false,
                sleepMultiplier = i_sleepMultiplier,
                currentDirectory = p_s_currentDirectory
            };

            for (int i = 1; i <= o_netConfigFile.FileLines; i++)
            {
                String s_line = o_netConfigFile.ReadLine(i);

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

                    o_netConfig.serverIp = a_split[1].Trim();
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

                    o_netConfig.clientIp = a_split[1].Trim();
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

                    o_netConfig.clientPort = int.Parse(a_split[1].Trim());
                }
                else if (i == 5)
                {
                    if (!s_line.StartsWith("serverBiIp"))
                    {
                        throw new Exception("Line #" + i + " does not start with 'serverBiIp'");
                    }

                    string[] a_split = s_line.Split("=");

                    if (a_split.Length != 2)
                    {
                        throw new Exception("Invalid key value pair for 'serverBiIp': '" + s_line + "'");
                    }

                    if (ForestNET.Lib.Helper.IsStringEmpty(a_split[1].Trim()))
                    {
                        throw new Exception("Invalid empty value, for 'serverBiIp'");
                    }

                    o_netConfig.serverBiIp = a_split[1].Trim();
                }
                else if (i == 6)
                {
                    if (!s_line.StartsWith("serverBiPort"))
                    {
                        throw new Exception("Line #" + i + " does not start with 'serverBiPort'");
                    }

                    string[] a_split = s_line.Split("=");

                    if (a_split.Length != 2)
                    {
                        throw new Exception("Invalid key value pair for 'serverBiPort': '" + s_line + "'");
                    }

                    if (!ForestNET.Lib.Helper.IsInteger(a_split[1].Trim()))
                    {
                        throw new Exception("Invalid value for 'serverBiPort': '" + a_split[1].Trim() + "' is not an integer");
                    }

                    o_netConfig.serverBiPort = int.Parse(a_split[1].Trim());
                }
                else if (i == 7)
                {
                    if (!s_line.StartsWith("clientBiIp"))
                    {
                        throw new Exception("Line #" + i + " does not start with 'clientBiIp'");
                    }

                    string[] a_split = s_line.Split("=");

                    if (a_split.Length != 2)
                    {
                        throw new Exception("Invalid key value pair for 'clientBiIp': '" + s_line + "'");
                    }

                    if (ForestNET.Lib.Helper.IsStringEmpty(a_split[1].Trim()))
                    {
                        throw new Exception("Invalid empty value, for 'clientBiIp'");
                    }

                    o_netConfig.clientBiIp = a_split[1].Trim();
                }
                else if (i == 8)
                {
                    if (!s_line.StartsWith("clientBiPort"))
                    {
                        throw new Exception("Line #" + i + " does not start with 'clientBiPort'");
                    }

                    string[] a_split = s_line.Split("=");

                    if (a_split.Length != 2)
                    {
                        throw new Exception("Invalid key value pair for 'clientBiPort': '" + s_line + "'");
                    }

                    if (!ForestNET.Lib.Helper.IsInteger(a_split[1].Trim()))
                    {
                        throw new Exception("Invalid value for 'clientBiPort': '" + a_split[1].Trim() + "' is not an integer");
                    }

                    o_netConfig.clientBiPort = int.Parse(a_split[1].Trim());
                }
                else if (i == 9)
                {
                    if (!s_line.StartsWith("clientLocalIp"))
                    {
                        throw new Exception("Line #" + i + " does not start with 'clientLocalIp'");
                    }

                    string[] a_split = s_line.Split("=");

                    if (a_split.Length != 2)
                    {
                        throw new Exception("Invalid key value pair for 'clientLocalIp': '" + s_line + "'");
                    }

                    if (ForestNET.Lib.Helper.IsStringEmpty(a_split[1].Trim()))
                    {
                        throw new Exception("Invalid empty value, for 'clientLocalIp'");
                    }

                    o_netConfig.clientLocalIp = a_split[1].Trim();
                }
                else if (i == 10)
                {
                    if (!s_line.StartsWith("clientLocalPort"))
                    {
                        throw new Exception("Line #" + i + " does not start with 'clientLocalPort'");
                    }

                    string[] a_split = s_line.Split("=");

                    if (a_split.Length != 2)
                    {
                        throw new Exception("Invalid key value pair for 'clientLocalPort': '" + s_line + "'");
                    }

                    if (!ForestNET.Lib.Helper.IsInteger(a_split[1].Trim()))
                    {
                        throw new Exception("Invalid value for 'clientLocalPort': '" + a_split[1].Trim() + "' is not an integer");
                    }

                    o_netConfig.clientLocalPort = int.Parse(a_split[1].Trim());
                }
                else if (i == 11)
                {
                    if (!s_line.StartsWith("handshakePort"))
                    {
                        throw new Exception("Line #" + i + " does not start with 'handshakePort'");
                    }

                    string[] a_split = s_line.Split("=");

                    if (a_split.Length != 2)
                    {
                        throw new Exception("Invalid key value pair for 'handshakePort': '" + s_line + "'");
                    }

                    if (!ForestNET.Lib.Helper.IsInteger(a_split[1].Trim()))
                    {
                        throw new Exception("Invalid value for 'handshakePort': '" + a_split[1].Trim() + "' is not an integer");
                    }

                    o_netConfig.handshakePort = int.Parse(a_split[1].Trim());
                }
                else if (i == 12)
                {
                    if (!s_line.StartsWith("symmetricSecurity128"))
                    {
                        throw new Exception("Line #" + i + " does not start with 'symmetricSecurity128'");
                    }

                    string[] a_split = s_line.Split("=");

                    if (a_split.Length != 2)
                    {
                        throw new Exception("Invalid key value pair for 'symmetricSecurity128': '" + s_line + "'");
                    }

                    o_netConfig.symmetricSecurity128 = Convert.ToBoolean(a_split[1].Trim());
                }
                else if (i == 13)
                {
                    if (!s_line.StartsWith("symmetricSecurity256"))
                    {
                        throw new Exception("Line #" + i + " does not start with 'symmetricSecurity256'");
                    }

                    string[] a_split = s_line.Split("=");

                    if (a_split.Length != 2)
                    {
                        throw new Exception("Invalid key value pair for 'symmetricSecurity256': '" + s_line + "'");
                    }

                    o_netConfig.symmetricSecurity256 = Convert.ToBoolean(a_split[1].Trim());
                }
                else if (i == 14)
                {
                    if (!s_line.StartsWith("asymmetricSecurity"))
                    {
                        throw new Exception("Line #" + i + " does not start with 'asymmetricSecurity'");
                    }

                    string[] a_split = s_line.Split("=");

                    if (a_split.Length != 2)
                    {
                        throw new Exception("Invalid key value pair for 'asymmetricSecurity': '" + s_line + "'");
                    }

                    o_netConfig.asymmetricSecurity = Convert.ToBoolean(a_split[1].Trim());
                }
                else if (i == 15)
                {
                    if (!s_line.StartsWith("highSecurity"))
                    {
                        throw new Exception("Line #" + i + " does not start with 'highSecurity'");
                    }

                    string[] a_split = s_line.Split("=");

                    if (a_split.Length != 2)
                    {
                        throw new Exception("Invalid key value pair for 'highSecurity': '" + s_line + "'");
                    }

                    o_netConfig.highSecurity = Convert.ToBoolean(a_split[1].Trim());
                }
                else if (i == 16)
                {
                    if (!s_line.StartsWith("useMarshalling"))
                    {
                        throw new Exception("Line #" + i + " does not start with 'useMarshalling'");
                    }

                    string[] a_split = s_line.Split("=");

                    if (a_split.Length != 2)
                    {
                        throw new Exception("Invalid key value pair for 'useMarshalling': '" + s_line + "'");
                    }

                    o_netConfig.useMarshalling = Convert.ToBoolean(a_split[1].Trim());
                }
                else if (i == 17)
                {
                    if (!s_line.StartsWith("useMarshallingWholeObject"))
                    {
                        throw new Exception("Line #" + i + " does not start with 'useMarshallingWholeObject'");
                    }

                    string[] a_split = s_line.Split("=");

                    if (a_split.Length != 2)
                    {
                        throw new Exception("Invalid key value pair for 'useMarshallingWholeObject': '" + s_line + "'");
                    }

                    o_netConfig.useMarshallingWholeObject = Convert.ToBoolean(a_split[1].Trim());
                }
                else if (i == 18)
                {
                    if (!s_line.StartsWith("marshallingDataLengthInBytes"))
                    {
                        throw new Exception("Line #" + i + " does not start with 'marshallingDataLengthInBytes'");
                    }

                    string[] a_split = s_line.Split("=");

                    if (a_split.Length != 2)
                    {
                        throw new Exception("Invalid key value pair for 'marshallingDataLengthInBytes': '" + s_line + "'");
                    }

                    if (!ForestNET.Lib.Helper.IsInteger(a_split[1].Trim()))
                    {
                        throw new Exception("Invalid value for 'marshallingDataLengthInBytes': '" + a_split[1].Trim() + "' is not an integer");
                    }

                    o_netConfig.marshallingDataLengthInBytes = int.Parse(a_split[1].Trim());
                }
                else if (i == 19)
                {
                    if (!s_line.StartsWith("marshallingUseProperties"))
                    {
                        throw new Exception("Line #" + i + " does not start with 'marshallingUseProperties'");
                    }

                    string[] a_split = s_line.Split("=");

                    if (a_split.Length != 2)
                    {
                        throw new Exception("Invalid key value pair for 'marshallingUseProperties': '" + s_line + "'");
                    }

                    o_netConfig.marshallingUseProperties = Convert.ToBoolean(a_split[1].Trim());
                }
                else if (i == 20)
                {
                    if (!s_line.StartsWith("marshallingOverrideMessageType"))
                    {
                        throw new Exception("Line #" + i + " does not start with 'marshallingOverrideMessageType'");
                    }

                    string[] a_split = s_line.Split("=");

                    if (a_split.Length != 2)
                    {
                        throw new Exception("Invalid key value pair for 'marshallingOverrideMessageType': '" + s_line + "'");
                    }

                    o_netConfig.marshallingOverrideMessageType = ((a_split[1].Trim().ToLower().Equals("null")) ? null : a_split[1].Trim());
                }
                else if (i == 21)
                {
                    if (!s_line.StartsWith("marshallingSystemUsesLittleEndian"))
                    {
                        throw new Exception("Line #" + i + " does not start with 'marshallingSystemUsesLittleEndian'");
                    }

                    string[] a_split = s_line.Split("=");

                    if (a_split.Length != 2)
                    {
                        throw new Exception("Invalid key value pair for 'marshallingSystemUsesLittleEndian': '" + s_line + "'");
                    }

                    o_netConfig.marshallingSystemUsesLittleEndian = Convert.ToBoolean(a_split[1].Trim());
                }
                else if (i == 22)
                {
                    if (!s_line.StartsWith("threadSleepMultiplier"))
                    {
                        throw new Exception("Line #" + i + " does not start with 'threadSleepMultiplier'");
                    }

                    string[] a_split = s_line.Split("=");

                    if (a_split.Length != 2)
                    {
                        throw new Exception("Invalid key value pair for 'threadSleepMultiplier': '" + s_line + "'");
                    }

                    if (!ForestNET.Lib.Helper.IsInteger(a_split[1].Trim()))
                    {
                        throw new Exception("Invalid value for 'threadSleepMultiplier': '" + a_split[1].Trim() + "' is not an integer");
                    }

                    o_netConfig.sleepMultiplier = int.Parse(a_split[1].Trim());
                }
                else if (i == 23)
                {
                    if (!s_line.StartsWith("thumbPrintServerCertificate"))
                    {
                        throw new Exception("Line #" + i + " does not start with 'thumbPrintServerCertificate'");
                    }

                    string[] a_split = s_line.Split("=");

                    if (a_split.Length != 2)
                    {
                        throw new Exception("Invalid key value pair for 'thumbPrintServerCertificate': '" + s_line + "'");
                    }

                    o_netConfig.thumbPrintServerCertificate = ((a_split[1].Trim().ToLower().Equals("null")) ? string.Empty : a_split[1].Trim());
                }
                else if (i == 24)
                {
                    if (!s_line.StartsWith("thumbPrintClientCertificate"))
                    {
                        throw new Exception("Line #" + i + " does not start with 'thumbPrintClientCertificate'");
                    }

                    string[] a_split = s_line.Split("=");

                    if (a_split.Length != 2)
                    {
                        throw new Exception("Invalid key value pair for 'thumbPrintClientCertificate': '" + s_line + "'");
                    }

                    o_netConfig.thumbPrintClientCertificate = ((a_split[1].Trim().ToLower().Equals("null")) ? string.Empty : a_split[1].Trim());
                }
                else if (i == 25)
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
            }

            Console.WriteLine("++++++++++++++++++++++++++++++++");
            Console.WriteLine("+ NET config read              +");
            Console.WriteLine("++++++++++++++++++++++++++++++++");

            Console.WriteLine("");

            Console.WriteLine("server ip" + "\t\t" + o_netConfig.serverIp);
            Console.WriteLine("server port" + "\t\t" + o_netConfig.serverPort);
            Console.WriteLine("client ip" + "\t\t" + o_netConfig.clientIp);
            Console.WriteLine("client port" + "\t\t" + o_netConfig.clientPort);
            Console.WriteLine("server bi ip" + "\t\t" + o_netConfig.serverBiIp);
            Console.WriteLine("server bi port" + "\t\t" + o_netConfig.serverBiPort);
            Console.WriteLine("client bi ip" + "\t\t" + o_netConfig.clientBiIp);
            Console.WriteLine("client bi port" + "\t\t" + o_netConfig.clientBiPort);
            Console.WriteLine("client local ip" + "\t\t" + o_netConfig.clientLocalIp);
            Console.WriteLine("client local port" + "\t" + o_netConfig.clientLocalPort);
            Console.WriteLine("handshake port" + "\t\t" + o_netConfig.handshakePort);
            Console.WriteLine("sym security 128" + "\t\t" + o_netConfig.symmetricSecurity128);
            Console.WriteLine("sym security 256" + "\t\t" + o_netConfig.symmetricSecurity256);
            Console.WriteLine("asym security" + "\t\t\t" + o_netConfig.asymmetricSecurity);
            Console.WriteLine("high sym security" + "\t\t" + o_netConfig.highSecurity);
            Console.WriteLine("use marshalling" + "\t\t\t" + o_netConfig.useMarshalling);
            Console.WriteLine("marshl. whole obj." + "\t\t" + o_netConfig.useMarshallingWholeObject);
            Console.WriteLine("marshl. data length" + "\t\t" + o_netConfig.marshallingDataLengthInBytes);
            Console.WriteLine("marshl. use properties" + "\t\t" + o_netConfig.marshallingUseProperties);
            Console.WriteLine("marshl. overr. mes. tpye" + "\t" + (o_netConfig.marshallingOverrideMessageType ?? "null"));
            Console.WriteLine("marshl. system LE" + "\t\t" + o_netConfig.marshallingSystemUsesLittleEndian);
            Console.WriteLine("thread sleep multiplier" + "\t\t" + o_netConfig.sleepMultiplier);
            Console.WriteLine("thumbprint server cert." + "\t\t" + o_netConfig.thumbPrintServerCertificate);
            Console.WriteLine("thumbprint client cert." + "\t\t" + o_netConfig.thumbPrintClientCertificate);
            Console.WriteLine("UDP Multicast TTL" + "\t\t" + o_netConfig.udpMulticastTTL);

            Console.WriteLine("");

            return o_netConfig;
        }

        private static ForestNET.Lib.Net.Sock.Com.Config GetCommunicationConfig(string p_s_currentDirectory, ForestNET.Lib.Net.Sock.Com.Type p_e_comType, ForestNET.Lib.Net.Sock.Com.Cardinality p_e_comCardinality, string p_s_host, int p_i_port, string? p_s_localHost, int p_i_localPort, bool p_b_symmetricSecurity128, bool p_b_symmetricSecurity256, bool p_b_asymmetricSecurity, bool p_b_highSecurity, AsymmetricModes p_e_asymmetricMode, bool p_b_useMarshalling, bool p_b_useMarshallingWholeObject, int p_i_marshallingDataLengthInBytes, bool p_b_marshallingUseProperties, string? p_s_marshallingOverrideMessageType, bool p_b_marshallingSystemUsesLittleEndian, string? p_s_thumbprintServerCertificate)
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
            string s_comSecretPassphrase = "z/?J}%KhZGr?6*rKJL,{-rf:^Necj~3M3Msj";

            ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = new(p_e_comType, p_e_comCardinality)
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
                if (p_e_asymmetricMode == AsymmetricModes.FileNoPassword)
                {
                    o_communicationConfig.PathToCertificateFile = s_resourcesDirectory + "server" + ForestNET.Lib.IO.File.DIR + "cert-server-no-pw.pfx";
                }
                else if (p_e_asymmetricMode == AsymmetricModes.FileWithPassword)
                {
                    o_communicationConfig.PathToCertificateFile = s_resourcesDirectory + "server" + ForestNET.Lib.IO.File.DIR + "cert-server-with-pw.pfx";
                    o_communicationConfig.PathToCertificateFilePassword = "123456";
                }
                else if (p_e_asymmetricMode == AsymmetricModes.ThumbprintInStore)
                {
                    o_communicationConfig.CertificateThumbprint = p_s_thumbprintServerCertificate;
                    o_communicationConfig.CertificateStoreName = System.Security.Cryptography.X509Certificates.StoreName.Root;
                    o_communicationConfig.CertificateStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser;
                }
                else if (p_e_asymmetricMode == AsymmetricModes.ThumbprintInStoreAndClientRemoteCertificateName)
                {
                    o_communicationConfig.CertificateThumbprint = p_s_thumbprintServerCertificate;
                    o_communicationConfig.CertificateStoreName = System.Security.Cryptography.X509Certificates.StoreName.Root;
                    o_communicationConfig.CertificateStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser;
                    o_communicationConfig.ClientRemoteCertificateName = ThumbPrintServerCertificateName;
                }

                o_communicationConfig.CommunicationSecurity = ForestNET.Lib.Net.Sock.Com.Security.ASYMMETRIC;
            }

            o_communicationConfig.UseMarshalling = p_b_useMarshalling;
            o_communicationConfig.UseMarshallingWholeObject = p_b_useMarshallingWholeObject;
            o_communicationConfig.MarshallingDataLengthInBytes = p_i_marshallingDataLengthInBytes;
            o_communicationConfig.MarshallingUseProperties = p_b_marshallingUseProperties;
            o_communicationConfig.MarshallingOverrideMessageType = p_s_marshallingOverrideMessageType;
            o_communicationConfig.MarshallingSystemUsesLittleEndian = p_b_marshallingSystemUsesLittleEndian;

            return o_communicationConfig;
        }

        private static void NetTCPSockBigFileTest(NetConfig p_o_config)
        {
            ForestNET.Lib.ConsoleProgressBar o_consoleProgressBar = new();

            ForestNET.Lib.Net.Sock.Task.Task.PostProgress del_postProgress = new(
                (int p_i_bytes, int p_i_totalBytes) =>
                {
                    o_consoleProgressBar.Report = (double)p_i_bytes / p_i_totalBytes;
                }
            );

            o_consoleProgressBar.Init();
            o_consoleProgressBar.Close();

            string s_currentDirectory = p_o_config.currentDirectory;

            if (p_o_config.isServer)
            { /* SERVER */
                string s_testDirectory = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "netTCPSockBigFileTestReceive" + ForestNET.Lib.IO.File.DIR;

                if (ForestNET.Lib.IO.File.FolderExists(s_testDirectory))
                {
                    ForestNET.Lib.IO.File.DeleteDirectory(s_testDirectory);
                }

                ForestNET.Lib.IO.File.CreateDirectory(s_testDirectory);
                if (!ForestNET.Lib.IO.File.FolderExists(s_testDirectory))
                {
                    throw new Exception("directory[" + s_testDirectory + "] does not exist");
                }

                string s_destinationFile = s_testDirectory + "destinationFile.txt";

                TCPSockBigFileTestTask o_serverTask = new(ForestNET.Lib.Net.Sock.Type.TCP_SERVER)
                {
                    BigFilePath = s_destinationFile,
                    ConsoleProgressBar = o_consoleProgressBar,
                    PostProgressDelegate = del_postProgress
                };

                ForestNET.Lib.Net.Sock.Recv.ReceiveTCP o_socketReceive = new(
                    ForestNET.Lib.Net.Sock.Recv.ReceiveType.SOCKET,      /* socket type */
                    p_o_config.serverIp,                                /* receiving address */
                    p_o_config.serverPort,                              /* receiving port */
                    o_serverTask,                                       /* server task */
                    30000,                                              /* timeout milliseconds */
                    -1,                                                 /* max. number of executions */
                    8192,                                               /* receive buffer size */
                    null                                                /* ssl context */
                );

                /* START SERVER */

                System.Threading.Tasks.Task o_taskServer = System.Threading.Tasks.Task.Run(async () =>
                {
                    await o_socketReceive.Run();
                });

                /* WAIT FOR SERVER TASK */

                o_taskServer.Wait();

                /* STOP SERVER */

                o_socketReceive.StopSocket();

                if (ForestNET.Lib.IO.File.FileLength(s_destinationFile) != 104857600)
                {
                    ForestNET.Lib.Global.LogException(new Exception("destination file length " + ForestNET.Lib.IO.File.FileLength(s_destinationFile) + " != 10485760"));
                }

                ForestNET.Lib.IO.File.DeleteDirectory(s_testDirectory);
                if (ForestNET.Lib.IO.File.FolderExists(s_testDirectory))
                {
                    throw new Exception("directory[" + s_testDirectory + "] does exist");
                }
            }
            else
            { /* CLIENT */
                string s_testDirectory = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "netTCPSockBigFileTestSend" + ForestNET.Lib.IO.File.DIR;
                string s_sourceFile;

                if (ForestNET.Lib.IO.File.Exists(s_currentDirectory + ForestNET.Lib.IO.File.DIR + "104857600_bytes.txt"))
                {
                    s_sourceFile = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "104857600_bytes.txt";
                }
                else
                {
                    if (ForestNET.Lib.IO.File.FolderExists(s_testDirectory))
                    {
                        ForestNET.Lib.IO.File.DeleteDirectory(s_testDirectory);
                    }

                    ForestNET.Lib.IO.File.CreateDirectory(s_testDirectory);

                    if (!ForestNET.Lib.IO.File.FolderExists(s_testDirectory))
                    {
                        throw new Exception("directory[" + s_testDirectory + "] does not exists");
                    }

                    s_sourceFile = s_testDirectory + "sourceFile.txt";

                    _ = new ForestNET.Lib.IO.File(s_sourceFile, true);

                    ForestNET.Lib.IO.File.ReplaceFileContent(s_sourceFile, ForestNET.Lib.IO.File.GenerateRandomFileContent_100MB(), System.Text.Encoding.ASCII);
                }

                if (ForestNET.Lib.IO.File.FileLength(s_sourceFile) != 104857600)
                {
                    throw new Exception("source file length != 104857600");
                }

                TCPSockBigFileTestTask o_clientTask = new(ForestNET.Lib.Net.Sock.Type.TCP_CLIENT)
                {
                    BigFilePath = s_sourceFile,
                    ConsoleProgressBar = o_consoleProgressBar,
                    PostProgressDelegate = del_postProgress
                };

                ForestNET.Lib.Net.Sock.Send.SendTCP o_socketSend = new(
                    p_o_config.serverIp,                                        /* destination address */
                    p_o_config.serverPort,                                      /* destination port */
                    o_clientTask,                                               /* client task */
                    30000,                                                      /* timeout milliseconds */
                    true,                                                       /* check if destination is reachable */
                    -1,                                                         /* max. number of executions */
                    25,                                                         /* interval for waiting for other communication side */
                    8192,                                                       /* buffer size */
                    p_o_config.clientLocalIp,                                   /* sender address */
                    0,                                                          /* sender port */
                    null                                                        /* ssl context */
                );

                /* START CLIENT */

                System.Threading.Tasks.Task o_taskClient = System.Threading.Tasks.Task.Run(async () =>
                {
                    await o_socketSend.Run();
                });

                /* WAIT FOR CLIENT TASK */

                o_taskClient.Wait();

                /* STOP CLIENT */

                o_socketSend.StopSocket();

                if (!ForestNET.Lib.IO.File.Exists(s_currentDirectory + ForestNET.Lib.IO.File.DIR + "104857600_bytes.txt"))
                {
                    ForestNET.Lib.IO.File.DeleteDirectory(s_testDirectory);

                    if (ForestNET.Lib.IO.File.FolderExists(s_testDirectory))
                    {
                        throw new Exception("directory[" + s_testDirectory + "] does exists");
                    }
                }
            }
        }

        internal class TCPSockBigFileTestTask : ForestNET.Lib.Net.Sock.Task.Task
        {
            public string? BigFilePath { get; set; }
            public ForestNET.Lib.ConsoleProgressBar? ConsoleProgressBar { get; set; }

            /* parameterless constructor for TCP RunServer method */
            public TCPSockBigFileTestTask() : base(ForestNET.Lib.Net.Sock.Type.TCP_SERVER)
            {

            }

            public TCPSockBigFileTestTask(ForestNET.Lib.Net.Sock.Type p_e_type) : base(p_e_type)
            {

            }

            public override void CloneFromOtherTask(ForestNET.Lib.Net.Sock.Task.Task p_o_sourceTask)
            {
                this.CloneBasicProperties(p_o_sourceTask);

                /* ignore exceptions if a property of source task has no valid value, we will keep it null */
                try { this.BigFilePath = ((TCPSockBigFileTestTask)p_o_sourceTask).BigFilePath; } catch (Exception) { /* NOP */ }
                try { this.ConsoleProgressBar = ((TCPSockBigFileTestTask)p_o_sourceTask).ConsoleProgressBar; } catch (Exception) { /* NOP */ }
            }

            public override async Task RunTask()
            {
                if (this.Type == ForestNET.Lib.Net.Sock.Type.TCP_SERVER)
                {
                    try
                    {
                        ForestNET.Lib.Global.ILog("S" + "\t" + "Handle incoming socket communication with " + this.ReceivingSocket?.RemoteEndPoint);

                        int i_amountBytes = await this.AmountBytesProtocol();
                        ForestNET.Lib.Global.ILog("S" + "\t" + "Amount bytes: " + i_amountBytes);

                        /* ------------------------------------------------------ */

                        if (i_amountBytes > 0)
                        {
                            this.ConsoleProgressBar?.Init("Receive data . . .", "Done.");
                            byte[] a_receivedData = await this.ReceiveBytes(i_amountBytes) ?? [];
                            this.ConsoleProgressBar?.Close();

                            using (FileStream o_fileStream = new(this.BigFilePath ?? throw new NullReferenceException("big file path is null"), FileMode.Create, FileAccess.Write))
                            {
                                o_fileStream.Write(a_receivedData, 0, a_receivedData.Length);
                            }

                            ForestNET.Lib.Global.ILog("S" + "\t" + "Wrote bytes into file");

                            /* ------------------------------------------------------ */

                            i_amountBytes = await this.AmountBytesProtocol(a_receivedData.Length);
                            ForestNET.Lib.Global.ILog("Amount bytes received: " + i_amountBytes);

                            this.Stop = true;
                        }
                        else
                        {
                            ForestNET.Lib.Global.ILog("Nothing received. Protocol for receiving length failed completely or was not intended (check availability call over TCP)");
                        }

                        ForestNET.Lib.Global.ILog("Socket communication closed");
                    }
                    catch (Exception o_exc)
                    {
                        ForestNET.Lib.Global.LogException(o_exc);
                    }
                }
                else if (this.Type == ForestNET.Lib.Net.Sock.Type.TCP_CLIENT)
                {
                    try
                    {
                        ForestNET.Lib.Global.ILog("C" + "\t" + "Starting socket communication");

                        byte[] a_data = File.ReadAllBytes(this.BigFilePath ?? throw new NullReferenceException("big file path is null"));

                        int i_amountBytes = await this.AmountBytesProtocol(a_data.Length);
                        ForestNET.Lib.Global.ILog("C" + "\t" + "Amount bytes: " + i_amountBytes);

                        /* ------------------------------------------------------ */

                        this.ConsoleProgressBar?.Init("Send data . . .", "Done.");
                        await this.SendBytes(a_data);
                        this.ConsoleProgressBar?.Close();
                        ForestNET.Lib.Global.ILog("C" + "\t" + "Sended data, amount of bytes: " + a_data.Length);

                        ForestNET.Lib.Global.ILog("waiting " + 25 * i_sleepMultiplier * 2 + "ms");
                        await System.Threading.Tasks.Task.Delay(25 * i_sleepMultiplier * 2);

                        /* ------------------------------------------------------ */

                        i_amountBytes = await this.AmountBytesProtocol();
                        ForestNET.Lib.Global.ILog("Amount bytes sended: " + i_amountBytes);

                        ForestNET.Lib.Global.ILog("Socket communication finished");

                        this.Stop = true;
                    }
                    catch (Exception o_exc)
                    {
                        ForestNET.Lib.Global.LogException(o_exc);
                    }
                }
            }
        }

        private static void NetTCPSockHandshakeTest(NetConfig p_o_config)
        {
            NetTCPSockHandshakeTest(p_o_config, 10000);
        }

        private static void NetTCPSockHandshakeTest(NetConfig p_o_config, int p_i_timeoutMilliseconds)
        {
            if (p_o_config.isServer)
            { /* SERVER */
                ForestNET.Lib.Net.Sock.Task.Recv.HandshakeReceive o_serverTask = new(ForestNET.Lib.Net.Sock.Type.TCP_SERVER);

                ForestNET.Lib.Net.Sock.Recv.ReceiveTCP o_socketReceive = new(
                    ForestNET.Lib.Net.Sock.Recv.ReceiveType.SOCKET,      /* socket type */
                    p_o_config.serverIp,                                /* receiving address */
                    p_o_config.handshakePort,                           /* receiving port */
                    o_serverTask,                                       /* server task */
                    p_i_timeoutMilliseconds,                            /* timeout milliseconds */
                    1,                                                  /* max. number of executions */
                    1500,                                               /* receive buffer size */
                    null                                                /* ssl context */
                );

                /* START SERVER */

                System.Threading.Tasks.Task o_taskServer = System.Threading.Tasks.Task.Run(async () =>
                {
                    await o_socketReceive.Run();
                });

                /* WAIT FOR SERVER TASK */

                o_taskServer.Wait();

                /* STOP SERVER */

                o_socketReceive.StopSocket();
            }
            else
            { /* CLIENT */
                ForestNET.Lib.Net.Sock.Task.Send.HandshakeSend o_clientTask = new(ForestNET.Lib.Net.Sock.Type.TCP_CLIENT);

                ForestNET.Lib.Net.Sock.Send.SendTCP o_socketSend = new(
                    p_o_config.serverIp,                                        /* destination address */
                    p_o_config.handshakePort,                                   /* destination port */
                    o_clientTask,                                               /* client task */
                    p_i_timeoutMilliseconds,                                    /* timeout milliseconds */
                    false,                                                      /* check if destination is reachable */
                    1,                                                          /* max. number of executions */
                    25,                                                         /* interval for waiting for other communication side */
                    1500,                                                       /* buffer size */
                    System.Net.Dns.GetHostName(),                               /* sender address */
                    0,                                                          /* sender port */
                    null                                                        /* ssl context */
                );

                /* START CLIENT */

                System.Threading.Tasks.Task o_taskClient = System.Threading.Tasks.Task.Run(async () =>
                {
                    await o_socketSend.Run();
                });

                /* WAIT FOR CLIENT TASK */

                o_taskClient.Wait();

                /* STOP CLIENT */

                o_socketSend.StopSocket();
            }
        }

        private static void NetCommunication(bool p_b_falseUDPtrueTCP, bool p_b_udpWithAck, AsymmetricModes p_e_asymmetricMode, NetConfig p_o_config)
        {
            i_sleepMultiplier = p_o_config.sleepMultiplier;
            int i_comDequeueWaitLoopTimeout = 5000;
            int i_iterations = 10;

            if (p_o_config.isServer)
            { /* SERVER */
                Task o_server = Task.Run(() =>
                {
                    try
                    {
                        List<string> a_serverLog = [];

                        ForestNET.Lib.Net.Sock.Com.Type e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE;

                        if ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck))
                        {
                            e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE_WITH_ACK;
                        }

                        if (p_b_falseUDPtrueTCP)
                        {
                            e_type = ForestNET.Lib.Net.Sock.Com.Type.TCP_RECEIVE;
                        }

                        ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(p_o_config.currentDirectory, e_type, ForestNET.Lib.Net.Sock.Com.Cardinality.Equal, p_o_config.serverIp, p_o_config.serverPort, null, 0, p_o_config.symmetricSecurity128, p_o_config.symmetricSecurity256, p_o_config.asymmetricSecurity, p_o_config.highSecurity, p_e_asymmetricMode, p_o_config.useMarshalling, p_o_config.useMarshallingWholeObject, p_o_config.marshallingDataLengthInBytes, p_o_config.marshallingUseProperties, p_o_config.marshallingOverrideMessageType, p_o_config.marshallingSystemUsesLittleEndian, p_o_config.thumbPrintServerCertificate);
                        ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                        o_communication.Start();

                        for (int i = 0; i < i_iterations; i++)
                        {
                            if (p_b_falseUDPtrueTCP)
                            {
                                string? s_message = (string?)o_communication.DequeueWithWaitLoop(i_comDequeueWaitLoopTimeout);

                                if (s_message != null)
                                {
                                    ForestNET.Lib.Global.ILogFine("#" + (i + 1) + " message(" + s_message.Length + ") received: '" + s_message + "'");
                                    a_serverLog.Add("#" + (i + 1) + " message(" + s_message.Length + ") received");
                                }
                                else
                                {
                                    ForestNET.Lib.Global.ILogWarning("could not receive any data");
                                }
                            }
                            else
                            {
                                DateTime? o_date = (DateTime?)o_communication.DequeueWithWaitLoop(i_comDequeueWaitLoopTimeout);

                                if (o_date != null)
                                {
                                    ForestNET.Lib.Global.ILogFine("#" + (i + 1) + " message received: '" + o_date?.ToString("dd.MM.yyyy HH:mm:ss") + "'");
                                    a_serverLog.Add("#" + (i + 1) + " message received");
                                }
                                else
                                {
                                    ForestNET.Lib.Global.ILogWarning("could not receive any data");
                                }
                            }
                        }

                        o_communication?.Stop();

                        /* CHECK LOG ENTRIES */

                        if (a_serverLog.Count != 10)
                        {
                            throw new Exception("server log has not '10' entries, but '" + a_serverLog.Count + "'");
                        }

                        if (p_b_falseUDPtrueTCP)
                        {
                            int i_expectedLength = 1438;

                            if (p_o_config.marshallingDataLengthInBytes == 1)
                            {
                                i_expectedLength = 215;
                            }

                            for (int i = 0; i < 10; i++)
                            {
                                if (!a_serverLog[i].StartsWith("#" + (i + 1) + " message(" + ((i == 9) ? (i_expectedLength + 1) : i_expectedLength) + ") received"))
                                {
                                    throw new Exception("server log entry does not start with '#" + (i + 1) + " message(" + ((i == 9) ? (i_expectedLength + 1) : i_expectedLength) + ") received:', but with '" + a_serverLog[i] + "'");
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                if (!a_serverLog[i].StartsWith("#" + (i + 1) + " message received"))
                                {
                                    throw new Exception("server log entry does not start with '#" + (i + 1) + " message received:', but with '" + a_serverLog[i] + "'");
                                }
                            }
                        }
                    }
                    catch (Exception o_exc)
                    {
                        ForestNET.Lib.Global.LogException(o_exc);
                    }
                });

                o_server.Wait();
            }
            else
            { /* CLIENT */
                Task o_client = Task.Run(() =>
                {
                    try
                    {
                        List<string> a_clientLog = [];

                        ForestNET.Lib.Net.Sock.Com.Type e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND;

                        if ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck))
                        {
                            e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND_WITH_ACK;
                        }

                        if (p_b_falseUDPtrueTCP)
                        {
                            e_type = ForestNET.Lib.Net.Sock.Com.Type.TCP_SEND;
                        }

                        ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(p_o_config.currentDirectory, e_type, ForestNET.Lib.Net.Sock.Com.Cardinality.Equal, p_o_config.serverIp, p_o_config.serverPort, ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck)) ? p_o_config.clientLocalIp : null, ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck)) ? p_o_config.clientLocalPort : 0, p_o_config.symmetricSecurity128, p_o_config.symmetricSecurity256, p_o_config.asymmetricSecurity, p_o_config.highSecurity, p_e_asymmetricMode, p_o_config.useMarshalling, p_o_config.useMarshallingWholeObject, p_o_config.marshallingDataLengthInBytes, p_o_config.marshallingUseProperties, p_o_config.marshallingOverrideMessageType, p_o_config.marshallingSystemUsesLittleEndian, p_o_config.thumbPrintServerCertificate);
                        ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                        o_communication.Start();

                        for (int i = 0; i < i_iterations; i++)
                        {
                            if (p_b_falseUDPtrueTCP)
                            {
                                string s_foo = (i + 1) + ": Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.   Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi. Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat.   Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat.";

                                if (p_o_config.marshallingDataLengthInBytes == 1)
                                {
                                    s_foo = (i + 1) + ": Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum.";
                                }

                                while (!o_communication.Enqueue(
                                    s_foo
                                ))
                                {
                                    ForestNET.Lib.Global.ILogWarning("could not enqueue message");
                                }
                            }
                            else
                            {
                                while (!o_communication.Enqueue(
                                    DateTime.Now
                                ))
                                {
                                    ForestNET.Lib.Global.ILogWarning("could not enqueue message");
                                }
                            }

                            ForestNET.Lib.Global.ILogFine("message enqueued");
                            a_clientLog.Add("message enqueued");

                            if (i == 4)
                            { /* additional delay for 25 milliseconds times sleep multiplier constant, after the 5th time enqueue has been executed */
                                Thread.Sleep(25 * i_sleepMultiplier);
                            }

                            Thread.Sleep(25 * i_sleepMultiplier + (((!p_b_falseUDPtrueTCP) && (p_o_config.highSecurity)) ? 150 : 0) + (((p_b_falseUDPtrueTCP) && ((p_o_config.highSecurity) || (p_o_config.asymmetricSecurity))) ? 25 : 0));
                        }

                        o_communication?.Stop();

                        /* CHECK LOG ENTRIES */

                        if (a_clientLog.Count != 10)
                        {
                            throw new Exception("client log has not '10' entries, but '" + a_clientLog.Count + "'");
                        }

                        if (p_b_falseUDPtrueTCP)
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                if (!a_clientLog[i].Equals("message enqueued"))
                                {
                                    throw new Exception("client log entry does not match with 'message enqueued', but is '" + a_clientLog[i] + "'");
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                if (!a_clientLog[i].Equals("message enqueued"))
                                {
                                    throw new Exception("client log entry does not match with 'message enqueued', but is '" + a_clientLog[i] + "'");
                                }
                            }
                        }
                    }
                    catch (Exception o_exc)
                    {
                        ForestNET.Lib.Global.LogException(o_exc);
                    }
                });

                o_client.Wait();
            }
        }

        private static void NetCommunicationUDPMulticast(NetConfig p_o_config, bool p_b_noAutomatic)
        {
            i_sleepMultiplier = p_o_config.sleepMultiplier;
            int i_comDequeueWaitLoopTimeout = 5000;
            int i_iterations = 10;

            if (!p_o_config.isServer)
            { /* CLIENT */
                Task o_client = Task.Run(() =>
                {
                    try
                    {
                        List<string> a_clientLog = [];

                        ForestNET.Lib.Net.Sock.Com.Type e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE;
                        ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(p_o_config.currentDirectory, e_type, ForestNET.Lib.Net.Sock.Com.Cardinality.Equal, p_o_config.serverIp, p_o_config.serverPort, null, 0, p_o_config.symmetricSecurity128, p_o_config.symmetricSecurity256, p_o_config.asymmetricSecurity, p_o_config.highSecurity, AsymmetricModes.None, p_o_config.useMarshalling, p_o_config.useMarshallingWholeObject, p_o_config.marshallingDataLengthInBytes, p_o_config.marshallingUseProperties, p_o_config.marshallingOverrideMessageType, p_o_config.marshallingSystemUsesLittleEndian, p_o_config.thumbPrintServerCertificate);
                        o_communicationConfig.UDPIsMulticastSocket = true;
                        ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                        o_communication.Start();

                        for (int i = 0; i < i_iterations; i++)
                        {
                            string? o_serverIp = (string?)o_communication.DequeueWithWaitLoop(i_comDequeueWaitLoopTimeout);

                            if (o_serverIp != null)
                            {
                                if (!p_b_noAutomatic)
                                {
                                    ForestNET.Lib.Global.ILogFine("#" + (i + 1) + " message received: '" + o_serverIp + "'");
                                    a_clientLog.Add("#" + (i + 1) + " message received");
                                }
                                else
                                {
                                    ForestNET.Lib.Global.ILog("#" + (i + 1) + " message received: '" + o_serverIp + "'");
                                }
                            }
                            else
                            {
                                ForestNET.Lib.Global.ILogWarning("could not receive any data");
                            }
                        }

                        o_communication?.Stop();

                        /* CHECK LOG ENTRIES */

                        if (!p_b_noAutomatic)
                        {
                            if (a_clientLog.Count != 10)
                            {
                                throw new Exception("client log has not '10' entries, but '" + a_clientLog.Count + "'");
                            }

                            for (int i = 0; i < 10; i++)
                            {
                                if (!a_clientLog[i].StartsWith("#" + (i + 1) + " message received"))
                                {
                                    throw new Exception("client log entry does not start with '#" + (i + 1) + " message received:', but with '" + a_clientLog[i] + "'");
                                }
                            }
                        }
                    }
                    catch (Exception o_exc)
                    {
                        ForestNET.Lib.Global.LogException(o_exc);
                    }
                });

                o_client.Wait();
            }
            else
            { /* SERVER */
                Task o_server = Task.Run(() =>
                {
                    try
                    {
                        List<string> a_serverLog = [];

                        ForestNET.Lib.Net.Sock.Com.Type e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND;
                        ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(p_o_config.currentDirectory, e_type, ForestNET.Lib.Net.Sock.Com.Cardinality.Equal, p_o_config.serverIp, p_o_config.serverPort, null, 0, p_o_config.symmetricSecurity128, p_o_config.symmetricSecurity256, p_o_config.asymmetricSecurity, p_o_config.highSecurity, AsymmetricModes.None, p_o_config.useMarshalling, p_o_config.useMarshallingWholeObject, p_o_config.marshallingDataLengthInBytes, p_o_config.marshallingUseProperties, p_o_config.marshallingOverrideMessageType, p_o_config.marshallingSystemUsesLittleEndian, p_o_config.thumbPrintServerCertificate);
                        o_communicationConfig.UDPIsMulticastSocket = true;
                        o_communicationConfig.UDPMulticastTTL = p_o_config.udpMulticastTTL;
                        ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                        o_communication.Start();

                        string s_ip = "COULD_NOT_GET_DEFAULT_IP";
                        List<KeyValuePair<string, string>> a_ips = ForestNET.Lib.Helper.GetNetworkInterfacesIpv4();

                        if (a_ips.Count > 0)
                        {
                            s_ip = a_ips[0].Value;
                        }

                        for (int i = 0; i < i_iterations; i++)
                        {
                            while (!o_communication.Enqueue(
                                "Join the server|" + s_ip
                            ))
                            {
                                ForestNET.Lib.Global.ILogWarning("could not enqueue message");
                            }

                            if (!p_b_noAutomatic)
                            {
                                ForestNET.Lib.Global.ILogFine("message enqueued");
                                a_serverLog.Add("message enqueued");
                            }
                            else
                            {
                                ForestNET.Lib.Global.ILog("message enqueued: '" + "Join the server|" + s_ip + "'");
                            }

                            if (i == 4)
                            { /* additional delay for 25 milliseconds times sleep multiplier constant, after the 5th time enqueue has been executed */
                                Thread.Sleep(25 * i_sleepMultiplier);
                            }

                            Thread.Sleep(25 * i_sleepMultiplier + ((p_o_config.highSecurity) ? 150 : 0));

                            if (p_b_noAutomatic)
                            {
                                Thread.Sleep(2500);
                            }
                        }

                        o_communication?.Stop();

                        /* CHECK LOG ENTRIES */

                        if (!p_b_noAutomatic)
                        {
                            if (a_serverLog.Count != 10)
                            {
                                throw new Exception("server log has not '10' entries, but '" + a_serverLog.Count + "'");
                            }


                            for (int i = 0; i < 10; i++)
                            {
                                if (!a_serverLog[i].Equals("message enqueued"))
                                {
                                    throw new Exception("server log entry does not match with 'message enqueued', but is '" + a_serverLog[i] + "'");
                                }
                            }
                        }
                    }
                    catch (Exception o_exc)
                    {
                        ForestNET.Lib.Global.LogException(o_exc);
                    }
                });

                o_server.Wait();
            }
        }

        private static void NetCommunicationObjectTransmissionTCP(AsymmetricModes p_e_asymmetricMode, NetConfig p_o_config)
        {
            i_sleepMultiplier = p_o_config.sleepMultiplier;
            int i_comDequeueWaitLoopTimeout = 5000;
            int i_iterations = 10;

            if (p_o_config.isServer)
            { /* SERVER */
                Task o_server = Task.Run(() =>
                {
                    try
                    {
                        List<string> a_serverLog = [];

                        ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(p_o_config.currentDirectory, ForestNET.Lib.Net.Sock.Com.Type.TCP_RECEIVE, ForestNET.Lib.Net.Sock.Com.Cardinality.Equal, p_o_config.serverIp, p_o_config.serverPort, null, 0, p_o_config.symmetricSecurity128, p_o_config.symmetricSecurity256, p_o_config.asymmetricSecurity, p_o_config.highSecurity, p_e_asymmetricMode, p_o_config.useMarshalling, p_o_config.useMarshallingWholeObject, p_o_config.marshallingDataLengthInBytes, p_o_config.marshallingUseProperties, p_o_config.marshallingOverrideMessageType, p_o_config.marshallingSystemUsesLittleEndian, p_o_config.thumbPrintServerCertificate);
                        ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                        o_communication.Start();

                        for (int i = 0; i < i_iterations; i++)
                        {
                            string? s_message = (string?)o_communication.DequeueWithWaitLoop(i_comDequeueWaitLoopTimeout);

                            if (s_message != null)
                            {
                                ForestNET.Lib.Global.ILogFine("#" + (i + 1) + " message(" + s_message.Length + ") received: '" + s_message + "'");
                                a_serverLog.Add("#" + (i + 1) + " message(" + s_message.Length + ") received");
                            }
                            else
                            {
                                ForestNET.Lib.Global.ILogWarning("could not receive any data");
                            }
                        }

                        o_communication?.Stop();

                        /* CHECK LOG ENTRIES */

                        if (a_serverLog.Count != 10)
                        {
                            throw new Exception("server log has not '10' entries, but '" + a_serverLog.Count + "'");
                        }

                        int i_expectedLength = 4308;

                        if (p_o_config.marshallingDataLengthInBytes == 1)
                        {
                            i_expectedLength = 254;
                        }

                        for (int i = 0; i < 10; i++)
                        {
                            if (!a_serverLog[i].StartsWith("#" + (i + 1) + " message(" + ((i == 9) ? (i_expectedLength + 1) : i_expectedLength) + ") received"))
                            {
                                throw new Exception("server log entry does not start with '#" + (i + 1) + " message(" + ((i == 9) ? (i_expectedLength + 1) : i_expectedLength) + ") received:', but with '" + a_serverLog[i] + "'");
                            }
                        }
                    }
                    catch (Exception o_exc)
                    {
                        ForestNET.Lib.Global.LogException(o_exc);
                    }
                });

                o_server.Wait();
            }
            else
            { /* CLIENT */
                Task o_client = Task.Run(() =>
                {
                    try
                    {
                        List<string> a_clientLog = [];

                        ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(p_o_config.currentDirectory, ForestNET.Lib.Net.Sock.Com.Type.TCP_SEND, ForestNET.Lib.Net.Sock.Com.Cardinality.Equal, p_o_config.serverIp, p_o_config.serverPort, null, 0, p_o_config.symmetricSecurity128, p_o_config.symmetricSecurity256, p_o_config.asymmetricSecurity, p_o_config.highSecurity, p_e_asymmetricMode, p_o_config.useMarshalling, p_o_config.useMarshallingWholeObject, p_o_config.marshallingDataLengthInBytes, p_o_config.marshallingUseProperties, p_o_config.marshallingOverrideMessageType, p_o_config.marshallingSystemUsesLittleEndian, p_o_config.thumbPrintServerCertificate);
                        ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                        o_communication.Start();

                        for (int i = 0; i < i_iterations; i++)
                        {
                            string s_foo = (i + 1) + ": Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.   Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi. Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat.   Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat.Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.   Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi. Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat.   Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat.Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.   Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi. Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat.   Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat.";

                            if (p_o_config.marshallingDataLengthInBytes == 1)
                            {
                                s_foo = (i + 1) + ": Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum, sed diam nonumy eirmod tempor invidun.";
                            }

                            while (!o_communication.Enqueue(
                                s_foo
                            ))
                            {
                                ForestNET.Lib.Global.ILogWarning("could not enqueue message");
                            }

                            ForestNET.Lib.Global.ILogFine("message enqueued");
                            a_clientLog.Add("message enqueued");

                            if (i == 4)
                            { /* additional delay for 25 milliseconds times sleep multiplier constant, after the 5th time enqueue has been executed */
                                Thread.Sleep(25 * i_sleepMultiplier);
                            }

                            Thread.Sleep(25 * i_sleepMultiplier + ((p_o_config.highSecurity && p_o_config.asymmetricSecurity) ? 25 : 0) + ((p_o_config.highSecurity && !p_o_config.asymmetricSecurity) ? 500 : 0));
                        }

                        o_communication?.Stop();

                        /* CHECK LOG ENTRIES */

                        if (a_clientLog.Count != 10)
                        {
                            throw new Exception("client log has not '10' entries, but '" + a_clientLog.Count + "'");
                        }

                        for (int i = 0; i < 10; i++)
                        {
                            if (!a_clientLog[i].Equals("message enqueued"))
                            {
                                throw new Exception("client log entry does not match with 'message enqueued', but is '" + a_clientLog[i] + "'");
                            }
                        }
                    }
                    catch (Exception o_exc)
                    {
                        ForestNET.Lib.Global.LogException(o_exc);
                    }
                });

                o_client.Wait();
            }
        }

        private static void NetCommunicationWithAnswerOneTCP(AsymmetricModes p_e_asymmetricMode, NetConfig p_o_config)
        {
            i_sleepMultiplier = p_o_config.sleepMultiplier;
            int i_comDequeueWaitLoopTimeout = 5000;
            int i_iterations = 10;

            if (p_o_config.isServer)
            { /* SERVER */
                Task o_server = Task.Run(() =>
                {
                    try
                    {
                        ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(p_o_config.currentDirectory, ForestNET.Lib.Net.Sock.Com.Type.TCP_RECEIVE_WITH_ANSWER, ForestNET.Lib.Net.Sock.Com.Cardinality.Equal, p_o_config.serverIp, p_o_config.serverPort, null, 0, p_o_config.symmetricSecurity128, p_o_config.symmetricSecurity256, p_o_config.asymmetricSecurity, p_o_config.highSecurity, p_e_asymmetricMode, p_o_config.useMarshalling, p_o_config.useMarshallingWholeObject, p_o_config.marshallingDataLengthInBytes, p_o_config.marshallingUseProperties, p_o_config.marshallingOverrideMessageType, p_o_config.marshallingSystemUsesLittleEndian, p_o_config.thumbPrintServerCertificate);

                        ReceiveSocketTaskOne o_receiveSocketTask = new();
                        o_receiveSocketTask.AddObject("<answer>");
                        o_receiveSocketTask.AddObject("</answer>");
                        o_communicationConfig.AddReceiveSocketTask(o_receiveSocketTask);

                        ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                        o_communication.Start();

                        Thread.Sleep(25 * i_sleepMultiplier * (i_iterations / 2) + ((p_o_config.highSecurity) ? 14000 : 3500));

                        o_communication?.Stop();
                    }
                    catch (Exception o_exc)
                    {
                        ForestNET.Lib.Global.LogException(o_exc);
                    }
                });

                o_server.Wait();
            }
            else
            { /* CLIENT */
                Task o_client = Task.Run(() =>
                {
                    try
                    {
                        List<string> a_serverLog = [];
                        List<string> a_clientLog = [];

                        ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(p_o_config.currentDirectory, ForestNET.Lib.Net.Sock.Com.Type.TCP_SEND_WITH_ANSWER, ForestNET.Lib.Net.Sock.Com.Cardinality.Equal, p_o_config.serverIp, p_o_config.serverPort, null, 0, p_o_config.symmetricSecurity128, p_o_config.symmetricSecurity256, p_o_config.asymmetricSecurity, p_o_config.highSecurity, p_e_asymmetricMode, p_o_config.useMarshalling, p_o_config.useMarshallingWholeObject, p_o_config.marshallingDataLengthInBytes, p_o_config.marshallingUseProperties, p_o_config.marshallingOverrideMessageType, p_o_config.marshallingSystemUsesLittleEndian, p_o_config.thumbPrintServerCertificate);
                        ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                        o_communication.Start();

                        for (int i = 0; i < i_iterations; i++)
                        {
                            int i_foo = i + 1;

                            while (!o_communication.Enqueue(
                                i_foo
                            ))
                            {
                                ForestNET.Lib.Global.ILogWarning("could not enqueue message");
                            }

                            ForestNET.Lib.Global.ILogFine("message enqueued");
                            a_clientLog.Add("message enqueued");

                            Object? o_answer = o_communication.DequeueWithWaitLoop(i_comDequeueWaitLoopTimeout);

                            if (o_answer != null)
                            {
                                ForestNET.Lib.Global.ILogFine("#" + (i + 1) + " message(" + o_answer.ToString()?.Length + ") received: '" + o_answer.ToString() + "'");
                                a_serverLog.Add("#" + (i + 1) + " message(" + o_answer.ToString()?.Length + ") received");
                            }
                            else
                            {
                                ForestNET.Lib.Global.ILogWarning("could not receive any answer data");
                            }

                            if (i == 4)
                            { /* additional delay for 25 milliseconds times sleep multiplier constant, after the 5th time enqueue has been executed */
                                Thread.Sleep(25 * i_sleepMultiplier);
                            }
                        }

                        o_communication?.Stop();

                        /* CHECK LOG ENTRIES */

                        if (a_serverLog.Count != 10)
                        {
                            throw new Exception("server log has not '10' entries, but '" + a_serverLog.Count + "'");
                        }

                        if (a_clientLog.Count != 10)
                        {
                            throw new Exception("client log has not '10' entries, but '" + a_clientLog.Count + "'");
                        }

                        List<int> a_expectedLength = [20, 20, 22, 21, 21, 20, 22, 22, 21, 20];

                        for (int i = 0; i < 10; i++)
                        {
                            if (!a_serverLog[i].StartsWith("#" + (i + 1) + " message(" + a_expectedLength[i] + ") received"))
                            {
                                throw new Exception("server log entry does not start with '#" + (i + 1) + " message(" + a_expectedLength[i] + ") received:', but with '" + a_serverLog[i] + "'");
                            }

                            if (!a_clientLog[i].Equals("message enqueued"))
                            {
                                throw new Exception("client log entry does not match with 'message enqueued', but is '" + a_clientLog[i] + "'");
                            }
                        }
                    }
                    catch (Exception o_exc)
                    {
                        ForestNET.Lib.Global.LogException(o_exc);
                    }
                });

                o_client.Wait();
            }
        }

        internal class ReceiveSocketTaskOne : ForestNET.Lib.Net.Sock.Task.Task
        {
            /* parameterless constructor for TCP RunServer method */
            public ReceiveSocketTaskOne() : base(ForestNET.Lib.Net.Sock.Type.TCP_TLS_SERVER)
            {

            }

            public ReceiveSocketTaskOne(ForestNET.Lib.Net.Sock.Type p_e_type) : base(p_e_type)
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
                    /* get objects of object list we we want to use them */
                    if ((this.Objects == null) && (this.Objects?.Count != 2))
                    {
                        throw new Exception("Objects for answer task are not available, size[" + (this.Objects?.Count ?? 0) + "] is not [2]");
                    }

                    /* get '<answer>' from instantiation */
                    string s_pre = (string)this.Objects[0];
                    /* get '</answer>' from instantiation */
                    string s_post = (string)this.Objects[1];

                    /* get request object as integer value */
                    int i_request = (int)(this.RequestObject ?? throw new NullReferenceException("Request object is null"));

                    /* handle request object */
                    string s_answer = i_request switch
                    {
                        1 => "one",
                        2 => "two",
                        3 => "three",
                        4 => "four",
                        5 => "five",
                        6 => "six",
                        7 => "seven",
                        8 => "eight",
                        9 => "nine",
                        10 => "ten",
                        _ => "",
                    };

                    /* set answer object */
                    this.AnswerObject = s_pre + s_answer + s_post;
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.LogException(o_exc);
                }

                await DoNoting();
            }
        }

        private static void NetCommunicationWithAnswerTwoTCP(AsymmetricModes p_e_asymmetricMode, NetConfig p_o_config)
        {
            i_sleepMultiplier = p_o_config.sleepMultiplier;
            int i_comDequeueWaitLoopTimeout = 5000;
            int i_iterations = 14;

            if (p_o_config.isServer)
            { /* SERVER */
                Task o_server = Task.Run(() =>
                {
                    try
                    {
                        ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(p_o_config.currentDirectory, ForestNET.Lib.Net.Sock.Com.Type.TCP_RECEIVE_WITH_ANSWER, ForestNET.Lib.Net.Sock.Com.Cardinality.Equal, p_o_config.serverIp, p_o_config.serverPort, null, 0, p_o_config.symmetricSecurity128, p_o_config.symmetricSecurity256, p_o_config.asymmetricSecurity, p_o_config.highSecurity, p_e_asymmetricMode, p_o_config.useMarshalling, p_o_config.useMarshallingWholeObject, p_o_config.marshallingDataLengthInBytes, p_o_config.marshallingUseProperties, p_o_config.marshallingOverrideMessageType, p_o_config.marshallingSystemUsesLittleEndian, p_o_config.thumbPrintServerCertificate);

                        ReceiveSocketTaskTwo o_receiveSocketTask = new();
                        o_receiveSocketTask.AddObject("<answer>");
                        o_receiveSocketTask.AddObject("</answer>");
                        o_communicationConfig.AddReceiveSocketTask(o_receiveSocketTask);

                        ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                        o_communication.Start();

                        Thread.Sleep(25 * i_sleepMultiplier * (i_iterations / 2) + ((p_o_config.highSecurity) ? 19000 : 4750));

                        o_communication?.Stop();
                    }
                    catch (Exception o_exc)
                    {
                        ForestNET.Lib.Global.LogException(o_exc);
                    }
                });

                o_server.Wait();
            }
            else
            { /* CLIENT */
                Task o_client = Task.Run(() =>
                {
                    try
                    {
                        List<string> a_serverLog = [];
                        List<string> a_clientLog = [];

                        ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(p_o_config.currentDirectory, ForestNET.Lib.Net.Sock.Com.Type.TCP_SEND_WITH_ANSWER, ForestNET.Lib.Net.Sock.Com.Cardinality.Equal, p_o_config.serverIp, p_o_config.serverPort, null, 0, p_o_config.symmetricSecurity128, p_o_config.symmetricSecurity256, p_o_config.asymmetricSecurity, p_o_config.highSecurity, p_e_asymmetricMode, p_o_config.useMarshalling, p_o_config.useMarshallingWholeObject, p_o_config.marshallingDataLengthInBytes, p_o_config.marshallingUseProperties, p_o_config.marshallingOverrideMessageType, p_o_config.marshallingSystemUsesLittleEndian, p_o_config.thumbPrintServerCertificate);
                        ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                        o_communication.Start();

                        for (int i = 0; i < i_iterations; i++)
                        {
                            while (!o_communication.Enqueue(
                            DateTime.Now.AddDays(i)
                        ))
                            {
                                ForestNET.Lib.Global.ILogWarning("could not enqueue message");
                            }

                            ForestNET.Lib.Global.ILogFine("message enqueued");
                            a_clientLog.Add("message enqueued");

                            Object? o_answer = o_communication.DequeueWithWaitLoop(i_comDequeueWaitLoopTimeout);

                            if (o_answer != null)
                            {
                                ForestNET.Lib.Global.ILogFine("#" + (i + 1) + " message(" + o_answer.ToString()?.Length + ") received: '" + o_answer.ToString() + "'");
                                a_serverLog.Add("#" + (i + 1) + " message(" + o_answer.ToString()?.Length + ") received - " + DateTime.Now.AddDays(i).ToString("yyyy-MM-dd"));
                            }
                            else
                            {
                                ForestNET.Lib.Global.ILogWarning("could not receive any answer data");
                            }

                            if (i == 6)
                            {
                                /* additional delay for 25 milliseconds times sleep multiplier constant, after the 7th time enqueue has been executed */
                                Thread.Sleep(25 * i_sleepMultiplier);
                            }
                        }

                        o_communication?.Stop();

                        /* CHECK LOG ENTRIES */

                        if (a_serverLog.Count != 14)
                        {
                            throw new Exception("server log has not '14' entries, but '" + a_serverLog.Count + "'");
                        }

                        if (a_clientLog.Count != 14)
                        {
                            throw new Exception("client log has not '14' entries, but '" + a_clientLog.Count + "'");
                        }

                        int i_expectedLength = 27;

                        for (int i = 0; i < 14; i++)
                        {
                            if (!a_serverLog[i].StartsWith("#" + (i + 1) + " message(" + i_expectedLength + ") received"))
                            {
                                throw new Exception("server log entry does not start with '#" + (i + 1) + " message(" + i_expectedLength + ") received:', but with '" + a_serverLog[i] + "'");
                            }

                            string s_foo = DateTime.Now.AddDays(i).ToString("yyyy-MM-dd");

                            if (!a_serverLog[i].Contains(s_foo))
                            {
                                throw new Exception("server log entry does not contain '" + s_foo + "', entry value: '" + a_serverLog[i] + "'");
                            }

                            if (!a_clientLog[i].Equals("message enqueued"))
                            {
                                throw new Exception("client log entry does not match with 'message enqueued', but is '" + a_clientLog[i] + "'");
                            }
                        }
                    }
                    catch (Exception o_exc)
                    {
                        ForestNET.Lib.Global.LogException(o_exc);
                    }
                });

                o_client.Wait();
            }
        }

        internal class ReceiveSocketTaskTwo : ForestNET.Lib.Net.Sock.Task.Task
        {
            /* parameterless constructor for TCP RunServer method */
            public ReceiveSocketTaskTwo() : base(ForestNET.Lib.Net.Sock.Type.TCP_TLS_SERVER)
            {

            }

            public ReceiveSocketTaskTwo(ForestNET.Lib.Net.Sock.Type p_e_type) : base(p_e_type)
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
                    /* get objects of object list we we want to use them */
                    if ((this.Objects == null) && (this.Objects?.Count != 2))
                    {
                        throw new Exception("Objects for answer task are not available, size[" + (this.Objects?.Count ?? 0) + "] is not [2]");
                    }

                    /* get '<answer>' from instantiation */
                    string s_pre = (string)this.Objects[0];
                    /* get '</answer>' from instantiation */
                    string s_post = (string)this.Objects[1];

                    /* get request object */
                    DateTime? o_request = (DateTime?)this.RequestObject;

                    /* set answer object */
                    if (o_request != null)
                    {
                        this.AnswerObject = s_pre + o_request?.ToString("yyyy-MM-dd") + s_post;
                    }
                    else
                    {
                        this.AnswerObject = s_pre + s_post;
                    }
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.LogException(o_exc);
                }

                await DoNoting();
            }
        }

        private static void NetCommunicationSharedMemoryUniDirectional(bool p_b_falseUDPtrueTCP, bool p_b_udpWithAck, AsymmetricModes p_e_asymmetricMode, NetConfig p_o_config)
        {
            i_sleepMultiplier = p_o_config.sleepMultiplier;

            /* expected result */
            string s_expectedResult = "BigInt = 546789546|Bool = True|Date = NULL|Decimal = 0|DoubleCol = 1,2345|FloatValue = 0|Id = 42|Int = 21|   LocalDate = 03.03.2003 00:00:00|LocalDateTime = NULL|LocalTime = NULL|ShortText = NULL|ShortText2 = NULL|SmallInt = 0|Text = Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.|Text2 = NULL|Time = NULL|Timestamp = NULL|UUID = a8dfc91d-ec7e-4a5f-9a9c-243edd91e271|";

            if (p_o_config.isServer)
            { /* SERVER */
                Task o_server = Task.Run(() =>
                {
                    try
                    {
                        List<string> a_expectedResults = [];
                        List<string> a_serverResults = [];

                        foreach (string s_foo in s_expectedResult.Split('|'))
                        {
                            a_expectedResults.Add(s_foo);
                        }

                        ForestNET.Lib.Net.Sock.Com.Type e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE;

                        if ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck))
                        {
                            e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE_WITH_ACK;
                        }

                        if (p_b_falseUDPtrueTCP)
                        {
                            e_type = ForestNET.Lib.Net.Sock.Com.Type.TCP_RECEIVE;
                        }

                        ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(p_o_config.currentDirectory, e_type, ForestNET.Lib.Net.Sock.Com.Cardinality.ManyMessageBoxesToOneSocket, p_o_config.serverIp, p_o_config.serverPort, null, 0, p_o_config.symmetricSecurity128, p_o_config.symmetricSecurity256, p_o_config.asymmetricSecurity, p_o_config.highSecurity, p_e_asymmetricMode, p_o_config.useMarshalling, p_o_config.useMarshallingWholeObject, p_o_config.marshallingDataLengthInBytes, p_o_config.marshallingUseProperties, p_o_config.marshallingOverrideMessageType, p_o_config.marshallingSystemUsesLittleEndian, p_o_config.thumbPrintServerCertificate);

                        SharedMemoryExample o_sharedMemoryExample = new();
                        o_sharedMemoryExample.InitiateMirrors().Wait();
                        o_communicationConfig.SharedMemory = o_sharedMemoryExample;
                        o_communicationConfig.SharedMemoryTimeoutMilliseconds = 10;

                        ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                        o_communication.Start();

                        int i_additionalTime = 25;

                        if (p_o_config.highSecurity)
                        {
                            if (p_b_falseUDPtrueTCP)
                            {
                                i_additionalTime = 60;
                            }
                            else
                            {
                                i_additionalTime = 90;
                            }
                        }

                        if ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck))
                        {
                            i_additionalTime += 225;
                        }

                        if ((p_b_falseUDPtrueTCP) && (p_o_config.asymmetricSecurity))
                        {
                            i_additionalTime = 45;
                        }

                        Thread.Sleep((25 * i_sleepMultiplier) + (i_additionalTime * 1));

                        Thread.Sleep((25 * i_sleepMultiplier) + (i_additionalTime * 4));

                        Thread.Sleep((25 * i_sleepMultiplier) + (i_additionalTime * 6));

                        Thread.Sleep((25 * i_sleepMultiplier) + (i_additionalTime * 3));

                        /* server result */
                        foreach (string s_foo in o_sharedMemoryExample.ReturnFields().Split('|'))
                        {
                            a_serverResults.Add(s_foo);
                        }

                        o_communication?.Stop();

                        /* CHECK LOG ENTRIES */

                        if (a_expectedResults.Count != a_serverResults.Count)
                        {
                            throw new Exception("server result has not the expected amount of fields '" + a_expectedResults.Count + "!='" + a_serverResults.Count);
                        }

                        int i_missingFieldsServer = 0;

                        for (int i = 0; i < a_expectedResults.Count; i++)
                        {
                            if (!a_expectedResults[i].Equals(a_serverResults[i]))
                            {
                                i_missingFieldsServer++;
                                ForestNET.Lib.Global.ILogFiner("server field result not equal expected result:\t" + a_serverResults[i] + "\t != \t" + a_expectedResults[i]);
                            }
                        }

                        if (i_missingFieldsServer > 0)
                        {
                            ForestNET.Lib.Global.ILog("uni " + ((p_b_falseUDPtrueTCP) ? "tcp" : "udp") + " server missing fields: " + i_missingFieldsServer);
                        }
                        else
                        {
                            ForestNET.Lib.Global.ILogFine("uni " + ((p_b_falseUDPtrueTCP) ? "tcp" : "udp") + " server missing fields: fine");
                        }

                        if (i_missingFieldsServer > 5)
                        {
                            throw new Exception(i_missingFieldsServer + " server fields not matching expected values, which is greater than '5'");
                        }
                    }
                    catch (Exception o_exc)
                    {
                        ForestNET.Lib.Global.LogException(o_exc);
                    }
                });

                o_server.Wait();
            }
            else
            { /* CLIENT */
                Task o_client = Task.Run(() =>
                {
                    try
                    {
                        List<string> a_expectedResults = [];
                        List<string> a_clientResults = [];

                        foreach (string s_foo in s_expectedResult.Split('|'))
                        {
                            a_expectedResults.Add(s_foo);
                        }

                        ForestNET.Lib.Net.Sock.Com.Type e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND;

                        if ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck))
                        {
                            e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND_WITH_ACK;
                        }

                        if (p_b_falseUDPtrueTCP)
                        {
                            e_type = ForestNET.Lib.Net.Sock.Com.Type.TCP_SEND;
                        }

                        ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(p_o_config.currentDirectory, e_type, ForestNET.Lib.Net.Sock.Com.Cardinality.ManyMessageBoxesToOneSocket, p_o_config.serverIp, p_o_config.serverPort, ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck)) ? p_o_config.clientLocalIp : null, ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck)) ? p_o_config.clientLocalPort : 0, p_o_config.symmetricSecurity128, p_o_config.symmetricSecurity256, p_o_config.asymmetricSecurity, p_o_config.highSecurity, p_e_asymmetricMode, p_o_config.useMarshalling, p_o_config.useMarshallingWholeObject, p_o_config.marshallingDataLengthInBytes, p_o_config.marshallingUseProperties, p_o_config.marshallingOverrideMessageType, p_o_config.marshallingSystemUsesLittleEndian, p_o_config.thumbPrintServerCertificate);

                        SharedMemoryExample o_sharedMemoryExample = new();
                        o_sharedMemoryExample.InitiateMirrors().Wait();
                        o_communicationConfig.SharedMemory = o_sharedMemoryExample;
                        o_communicationConfig.SharedMemoryTimeoutMilliseconds = 10;

                        ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                        o_communication.Start();

                        int i_additionalTime = 25;

                        if (p_o_config.highSecurity)
                        {
                            if (p_b_falseUDPtrueTCP)
                            {
                                i_additionalTime = 60;
                            }
                            else
                            {
                                i_additionalTime = 90;
                            }
                        }

                        if ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck))
                        {
                            i_additionalTime += 225;
                        }

                        if ((p_b_falseUDPtrueTCP) && (p_o_config.asymmetricSecurity))
                        {
                            i_additionalTime = 45;
                        }

                        Thread.Sleep((25 * i_sleepMultiplier) + (i_additionalTime * 1));

                        o_sharedMemoryExample.SetField("Text", "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.").Wait();
                        o_sharedMemoryExample.SetField("LocalDate", new DateTime(2003, 3, 3)).Wait();
                        o_sharedMemoryExample.SetField("Int", 13579).Wait();

                        Thread.Sleep((25 * i_sleepMultiplier) + (i_additionalTime * 4));

                        o_sharedMemoryExample.SetField("Id", 42).Wait();
                        o_sharedMemoryExample.SetField("UUID", "a8dfc91d-ec7e-4a5f-9a9c-243edd91e271").Wait();
                        o_sharedMemoryExample.SetField("Text", "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.").Wait();
                        o_sharedMemoryExample.SetField("DoubleCol", 1.2345d).Wait();
                        o_sharedMemoryExample.SetField("Bool", true).Wait();

                        Thread.Sleep((25 * i_sleepMultiplier) + (i_additionalTime * 6));

                        o_sharedMemoryExample.SetField("Int", 21).Wait();
                        o_sharedMemoryExample.SetField("BigInt", 546789546L).Wait();

                        Thread.Sleep((25 * i_sleepMultiplier) + (i_additionalTime * 3));

                        /* client result */
                        foreach (string s_foo in o_sharedMemoryExample.ReturnFields().Split('|'))
                        {
                            a_clientResults.Add(s_foo);
                        }

                        o_communication?.Stop();

                        /* CHECK LOG ENTRIES */

                        if (a_expectedResults.Count != a_clientResults.Count)
                        {
                            throw new Exception("client result has not the expected amount of fields '" + a_expectedResults.Count + "!='" + a_clientResults.Count);
                        }

                        int i_missingFieldsClient = 0;

                        for (int i = 0; i < a_expectedResults.Count; i++)
                        {
                            if (!a_expectedResults[i].Equals(a_clientResults[i]))
                            {
                                i_missingFieldsClient++;
                                ForestNET.Lib.Global.ILogFiner("client field result not equal expected result:\t" + a_clientResults[i] + "\t != \t" + a_expectedResults[i]);
                            }
                        }

                        if (i_missingFieldsClient > 5)
                        {
                            throw new Exception(i_missingFieldsClient + " client fields not matching expected values, which is greater than '5'");
                        }

                        if (i_missingFieldsClient > 0)
                        {
                            ForestNET.Lib.Global.ILog("uni " + ((p_b_falseUDPtrueTCP) ? "tcp" : "udp") + " client missing fields: " + i_missingFieldsClient);
                        }
                        else
                        {
                            ForestNET.Lib.Global.ILogFine("uni " + ((p_b_falseUDPtrueTCP) ? "tcp" : "udp") + " client missing fields: fine");
                        }
                    }
                    catch (Exception o_exc)
                    {
                        ForestNET.Lib.Global.LogException(o_exc);
                    }
                });

                o_client.Wait();
            }
        }

        private static void NetCommunicationSharedMemoryBiDirectional(bool p_b_falseUDPtrueTCP, bool p_b_udpWithAck, AsymmetricModes p_e_asymmetricMode, NetConfig p_o_config)
        {
            i_sleepMultiplier = p_o_config.sleepMultiplier;

            /* expected result */
            string s_expectedResult = "BigInt = 0|Bool = False|Date = NULL|Decimal = 0|DoubleCol = 5,4321|FloatValue = 2,114014|Id = 42|Int = 50791|LocalDate = 04.04.2004 00:00:00|LocalDateTime = NULL|LocalTime = NULL|ShortText = NULL|ShortText2 = Mission accomplished.|SmallInt = 0|Text = Nam liber tempor cum soluta nobis eleifend option congue nihil imperdiet doming id quod mazim placerat facer possim assum.|Text2 = At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.|Time = NULL|Timestamp = NULL|UUID = 26cf332e-3f23-4523-9911-60207c8db7fd|";

            if (p_o_config.isServer)
            { /* SERVER */
                Task o_server = Task.Run(() =>
                {
                    try
                    {
                        List<string> a_expectedResults = [];
                        List<string> a_serverResults = [];

                        foreach (string s_foo in s_expectedResult.Split('|'))
                        {
                            a_expectedResults.Add(s_foo);
                        }

                        ForestNET.Lib.Net.Sock.Com.Type e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE;

                        if ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck))
                        {
                            e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE_WITH_ACK;
                        }

                        if (p_b_falseUDPtrueTCP)
                        {
                            e_type = ForestNET.Lib.Net.Sock.Com.Type.TCP_RECEIVE;
                        }

                        ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(p_o_config.currentDirectory, e_type, ForestNET.Lib.Net.Sock.Com.Cardinality.ManyMessageBoxesToOneSocket, p_o_config.serverIp, p_o_config.serverPort, null, 0, p_o_config.symmetricSecurity128, p_o_config.symmetricSecurity256, p_o_config.asymmetricSecurity, p_o_config.highSecurity, p_e_asymmetricMode, p_o_config.useMarshalling, p_o_config.useMarshallingWholeObject, p_o_config.marshallingDataLengthInBytes, p_o_config.marshallingUseProperties, p_o_config.marshallingOverrideMessageType, p_o_config.marshallingSystemUsesLittleEndian, p_o_config.thumbPrintServerCertificate);

                        SharedMemoryExample o_sharedMemoryExample = new();
                        o_sharedMemoryExample.InitiateMirrors().Wait();
                        o_communicationConfig.SharedMemory = o_sharedMemoryExample;
                        o_communicationConfig.SharedMemoryTimeoutMilliseconds = 10;

                        if (p_o_config.asymmetricSecurity)
                        {
                            string? s_pathToCertificateFile = null;
                            string? s_pathToCertificateFilePassword = null;
                            string? s_certificateThumbprint = null;
                            System.Security.Cryptography.X509Certificates.StoreName? e_certificateStoreName = null;
                            System.Security.Cryptography.X509Certificates.StoreLocation? e_certificateStoreLocation = null;
                            string? s_clientRemoteCertificateName = null;
                            List<System.Security.Cryptography.X509Certificates.X509Certificate>? a_clientCertificateAllowList = null;

                            /* redo security settings for bidirectional side(client) */
                            if (p_e_asymmetricMode == AsymmetricModes.FileNoPassword)
                            {
                                s_pathToCertificateFile = p_o_config.currentDirectory + "Resources" + ForestNET.Lib.IO.File.DIR + "com" + ForestNET.Lib.IO.File.DIR + "client" + ForestNET.Lib.IO.File.DIR + "cert-client-no-pw.pfx";
                            }
                            else if (p_e_asymmetricMode == AsymmetricModes.FileWithPassword)
                            {
                                s_pathToCertificateFile = p_o_config.currentDirectory + "Resources" + ForestNET.Lib.IO.File.DIR + "com" + ForestNET.Lib.IO.File.DIR + "client" + ForestNET.Lib.IO.File.DIR + "cert-client-with-pw.pfx";
                                s_pathToCertificateFilePassword = "123456";
                            }
                            else if (p_e_asymmetricMode == AsymmetricModes.ThumbprintInStore)
                            {
                                s_certificateThumbprint = p_o_config.thumbPrintClientCertificate;
                                e_certificateStoreName = System.Security.Cryptography.X509Certificates.StoreName.Root;
                                e_certificateStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser;
                            }
                            else if (p_e_asymmetricMode == AsymmetricModes.ThumbprintInStoreAndClientRemoteCertificateName)
                            {
                                s_certificateThumbprint = p_o_config.thumbPrintClientCertificate;
                                e_certificateStoreName = System.Security.Cryptography.X509Certificates.StoreName.Root;
                                e_certificateStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser;
                                s_clientRemoteCertificateName = ThumbPrintClientCertificateName;
                            }

                            o_communicationConfig.SetSharedMemoryBidirectional(
                                new Dictionary<string, int> {
                                { p_o_config.serverBiIp, p_o_config.serverBiPort }
                                },
                                o_communicationConfig.SocketReceiveType ?? throw new NullReferenceException("socket receive type is null while setting shared memory bidirectional"),
                                s_pathToCertificateFile,
                                s_pathToCertificateFilePassword,
                                s_certificateThumbprint,
                                e_certificateStoreName,
                                e_certificateStoreLocation,
                                s_clientRemoteCertificateName,
                                a_clientCertificateAllowList
                            );
                        }
                        else
                        {
                            o_communicationConfig.SetSharedMemoryBidirectional(
                                new Dictionary<string, int> {
                                { p_o_config.serverBiIp, p_o_config.serverBiPort }
                                }
                            );
                        }

                        ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                        o_communication.Start();

                        int i_additionalTime = 25;

                        if (p_o_config.highSecurity)
                        {
                            if (p_b_falseUDPtrueTCP)
                            {
                                i_additionalTime = 60;
                            }
                            else
                            {
                                i_additionalTime = 90;
                            }
                        }

                        if ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck))
                        {
                            i_additionalTime += 225;
                        }

                        if ((p_b_falseUDPtrueTCP) && (p_o_config.asymmetricSecurity))
                        {
                            i_additionalTime = 45;
                        }

                        Thread.Sleep((25 * i_sleepMultiplier) + (i_additionalTime * 1));

                        Thread.Sleep((25 * i_sleepMultiplier) + (i_additionalTime * 4));

                        o_sharedMemoryExample.SetField("Int", 50791).Wait();

                        Thread.Sleep((25 * i_sleepMultiplier) + (i_additionalTime * 2));

                        Thread.Sleep((25 * i_sleepMultiplier) + (i_additionalTime * 6));

                        o_sharedMemoryExample.SetField("Id", 42).Wait();
                        o_sharedMemoryExample.SetField("ShortText2", "Mission accomplished.").Wait();
                        o_sharedMemoryExample.SetField("Text2", "At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.").Wait();
                        o_sharedMemoryExample.SetField("FloatValue", 2.114014f).Wait();

                        Thread.Sleep((25 * i_sleepMultiplier) + (i_additionalTime * 5));

                        /* server result */
                        foreach (string s_foo in o_sharedMemoryExample.ReturnFields().Split('|'))
                        {
                            a_serverResults.Add(s_foo);
                        }

                        o_communication?.Stop();

                        /* CHECK LOG ENTRIES */

                        if (a_expectedResults.Count != a_serverResults.Count)
                        {
                            throw new Exception("server result has not the expected amount of fields '" + a_expectedResults.Count + "!='" + a_serverResults.Count);
                        }

                        int i_missingFieldsServer = 0;

                        for (int i = 0; i < a_expectedResults.Count; i++)
                        {
                            if (!a_expectedResults[i].Equals(a_serverResults[i]))
                            {
                                i_missingFieldsServer++;
                                ForestNET.Lib.Global.ILogFiner("server field result not equal expected result:\t" + a_serverResults[i] + "\t != \t" + a_expectedResults[i]);
                            }
                        }

                        if (i_missingFieldsServer > 0)
                        {
                            ForestNET.Lib.Global.ILog("bi " + ((p_b_falseUDPtrueTCP) ? "tcp" : "udp") + " server missing fields: " + i_missingFieldsServer);
                        }
                        else
                        {
                            ForestNET.Lib.Global.ILogFine("bi " + ((p_b_falseUDPtrueTCP) ? "tcp" : "udp") + " server missing fields: fine");
                        }

                        if (i_missingFieldsServer > 5)
                        {
                            throw new Exception(i_missingFieldsServer + " server fields not matching expected values, which is greater than '5'");
                        }
                    }
                    catch (Exception o_exc)
                    {
                        ForestNET.Lib.Global.LogException(o_exc);
                    }
                });

                o_server.Wait();
            }
            else
            { /* CLIENT */
                Task o_client = Task.Run(() =>
                {
                    try
                    {
                        List<string> a_expectedResults = [];
                        List<string> a_clientResults = [];

                        foreach (string s_foo in s_expectedResult.Split('|'))
                        {
                            a_expectedResults.Add(s_foo);
                        }

                        ForestNET.Lib.Net.Sock.Com.Type e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND;

                        if ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck))
                        {
                            e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND_WITH_ACK;
                        }

                        if (p_b_falseUDPtrueTCP)
                        {
                            e_type = ForestNET.Lib.Net.Sock.Com.Type.TCP_SEND;
                        }

                        ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(p_o_config.currentDirectory, e_type, ForestNET.Lib.Net.Sock.Com.Cardinality.ManyMessageBoxesToOneSocket, p_o_config.serverIp, p_o_config.serverPort, ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck)) ? p_o_config.clientLocalIp : null, ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck)) ? p_o_config.clientLocalPort : 0, p_o_config.symmetricSecurity128, p_o_config.symmetricSecurity256, p_o_config.asymmetricSecurity, p_o_config.highSecurity, p_e_asymmetricMode, p_o_config.useMarshalling, p_o_config.useMarshallingWholeObject, p_o_config.marshallingDataLengthInBytes, p_o_config.marshallingUseProperties, p_o_config.marshallingOverrideMessageType, p_o_config.marshallingSystemUsesLittleEndian, p_o_config.thumbPrintServerCertificate);

                        SharedMemoryExample o_sharedMemoryExample = new();
                        o_sharedMemoryExample.InitiateMirrors().Wait();
                        o_communicationConfig.SharedMemory = o_sharedMemoryExample;
                        o_communicationConfig.SharedMemoryTimeoutMilliseconds = 10;

                        if (p_o_config.asymmetricSecurity)
                        {
                            string? s_pathToCertificateFile = null;
                            string? s_pathToCertificateFilePassword = null;
                            string? s_certificateThumbprint = null;
                            System.Security.Cryptography.X509Certificates.StoreName? e_certificateStoreName = null;
                            System.Security.Cryptography.X509Certificates.StoreLocation? e_certificateStoreLocation = null;
                            string? s_clientRemoteCertificateName = null;
                            List<System.Security.Cryptography.X509Certificates.X509Certificate>? a_clientCertificateAllowList = null;

                            /* redo security settings for bidirectional side(client) */
                            if (p_e_asymmetricMode == AsymmetricModes.FileNoPassword)
                            {
                                s_pathToCertificateFile = p_o_config.currentDirectory + "Resources" + ForestNET.Lib.IO.File.DIR + "com" + ForestNET.Lib.IO.File.DIR + "client" + ForestNET.Lib.IO.File.DIR + "cert-client-no-pw.pfx";
                            }
                            else if (p_e_asymmetricMode == AsymmetricModes.FileWithPassword)
                            {
                                s_pathToCertificateFile = p_o_config.currentDirectory + "Resources" + ForestNET.Lib.IO.File.DIR + "com" + ForestNET.Lib.IO.File.DIR + "client" + ForestNET.Lib.IO.File.DIR + "cert-client-with-pw.pfx";
                                s_pathToCertificateFilePassword = "123456";
                            }
                            else if (p_e_asymmetricMode == AsymmetricModes.ThumbprintInStore)
                            {
                                s_certificateThumbprint = p_o_config.thumbPrintClientCertificate;
                                e_certificateStoreName = System.Security.Cryptography.X509Certificates.StoreName.Root;
                                e_certificateStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser;
                            }
                            else if (p_e_asymmetricMode == AsymmetricModes.ThumbprintInStoreAndClientRemoteCertificateName)
                            {
                                s_certificateThumbprint = p_o_config.thumbPrintClientCertificate;
                                e_certificateStoreName = System.Security.Cryptography.X509Certificates.StoreName.Root;
                                e_certificateStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser;
                                s_clientRemoteCertificateName = ThumbPrintClientCertificateName;
                            }

                            o_communicationConfig.SetSharedMemoryBidirectional(
                                new Dictionary<string, int> {
                                { p_o_config.serverBiIp, p_o_config.serverBiPort }
                                },
                                o_communicationConfig.SocketReceiveType ?? throw new NullReferenceException("socket receive type is null while setting shared memory bidirectional"),
                                s_pathToCertificateFile,
                                s_pathToCertificateFilePassword,
                                s_certificateThumbprint,
                                e_certificateStoreName,
                                e_certificateStoreLocation,
                                s_clientRemoteCertificateName,
                                a_clientCertificateAllowList
                            );
                        }
                        else
                        {
                            o_communicationConfig.SetSharedMemoryBidirectional(
                                new Dictionary<string, int> {
                                { p_o_config.serverBiIp, p_o_config.serverBiPort }
                                }
                            );
                        }

                        ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                        o_communication.Start();

                        int i_additionalTime = 25;

                        if (p_o_config.highSecurity)
                        {
                            if (p_b_falseUDPtrueTCP)
                            {
                                i_additionalTime = 60;
                            }
                            else
                            {
                                i_additionalTime = 90;
                            }
                        }

                        if ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck))
                        {
                            i_additionalTime += 225;
                        }

                        if ((p_b_falseUDPtrueTCP) && (p_o_config.asymmetricSecurity))
                        {
                            i_additionalTime = 45;
                        }

                        Thread.Sleep((25 * i_sleepMultiplier) + (i_additionalTime * 1));

                        o_sharedMemoryExample.SetField("Text", "Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat.").Wait();
                        o_sharedMemoryExample.SetField("LocalDate", new DateTime(2004, 4, 4)).Wait();
                        o_sharedMemoryExample.SetField("Int", 24680).Wait();

                        Thread.Sleep((25 * i_sleepMultiplier) + (i_additionalTime * 4));

                        Thread.Sleep((25 * i_sleepMultiplier) + (i_additionalTime * 2));

                        o_sharedMemoryExample.SetField("Id", 21).Wait();
                        o_sharedMemoryExample.SetField("UUID", "26cf332e-3f23-4523-9911-60207c8db7fd").Wait();
                        o_sharedMemoryExample.SetField("Text", "Nam liber tempor cum soluta nobis eleifend option congue nihil imperdiet doming id quod mazim placerat facer possim assum.").Wait();
                        o_sharedMemoryExample.SetField("DoubleCol", 5.4321d).Wait();
                        o_sharedMemoryExample.SetField("Bool", false).Wait();

                        Thread.Sleep((25 * i_sleepMultiplier) + (i_additionalTime * 6));

                        Thread.Sleep((25 * i_sleepMultiplier) + (i_additionalTime * 5));

                        /* client result */
                        foreach (string s_foo in o_sharedMemoryExample.ReturnFields().Split('|'))
                        {
                            a_clientResults.Add(s_foo);
                        }

                        o_communication?.Stop();

                        /* CHECK LOG ENTRIES */

                        if (a_expectedResults.Count != a_clientResults.Count)
                        {
                            throw new Exception("client result has not the expected amount of fields '" + a_expectedResults.Count + "!='" + a_clientResults.Count);
                        }

                        int i_missingFieldsClient = 0;

                        for (int i = 0; i < a_expectedResults.Count; i++)
                        {
                            if (!a_expectedResults[i].Equals(a_clientResults[i]))
                            {
                                i_missingFieldsClient++;
                                ForestNET.Lib.Global.ILogFiner("client field result not equal expected result:\t" + a_clientResults[i] + "\t != \t" + a_expectedResults[i]);
                            }
                        }

                        if (i_missingFieldsClient > 5)
                        {
                            throw new Exception(i_missingFieldsClient + " client fields not matching expected values, which is greater than '5'");
                        }

                        if (i_missingFieldsClient > 0)
                        {
                            ForestNET.Lib.Global.ILog("bi " + ((p_b_falseUDPtrueTCP) ? "tcp" : "udp") + " client missing fields: " + i_missingFieldsClient);
                        }
                        else
                        {
                            ForestNET.Lib.Global.ILogFine("bi " + ((p_b_falseUDPtrueTCP) ? "tcp" : "udp") + " client missing fields: fine");
                        }
                    }
                    catch (Exception o_exc)
                    {
                        ForestNET.Lib.Global.LogException(o_exc);
                    }
                });

                o_client.Wait();
            }
        }

        private static void NetCommunicationMarshallingObject(bool p_b_falseUDPtrueTCP, bool p_b_udpWithAck, bool p_b_smallObject, AsymmetricModes p_e_asymmetricMode, NetConfig p_o_config)
        {
            i_sleepMultiplier = p_o_config.sleepMultiplier;
            int i_comDequeueWaitLoopTimeout = 5000;
            int i_iterations = 10;

            if (p_o_config.isServer)
            { /* SERVER */
                Task o_server = Task.Run(() =>
                {
                    try
                    {
                        List<Object> a_serverObjects = [];

                        ForestNET.Lib.Net.Sock.Com.Type e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE;

                        if ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck))
                        {
                            e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE_WITH_ACK;
                        }

                        if (p_b_falseUDPtrueTCP)
                        {
                            e_type = ForestNET.Lib.Net.Sock.Com.Type.TCP_RECEIVE;
                        }

                        ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(p_o_config.currentDirectory, e_type, ForestNET.Lib.Net.Sock.Com.Cardinality.Equal, p_o_config.serverIp, p_o_config.serverPort, null, 0, p_o_config.symmetricSecurity128, p_o_config.symmetricSecurity256, p_o_config.asymmetricSecurity, p_o_config.highSecurity, p_e_asymmetricMode, p_o_config.useMarshalling, p_o_config.useMarshallingWholeObject, p_o_config.marshallingDataLengthInBytes, p_o_config.marshallingUseProperties, p_o_config.marshallingOverrideMessageType, p_o_config.marshallingSystemUsesLittleEndian, p_o_config.thumbPrintServerCertificate);

                        if (p_b_falseUDPtrueTCP)
                        {
                            o_communicationConfig.ObjectTransmission = true;
                        }

                        ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                        o_communication.Start();

                        for (int i = 0; i < i_iterations; i++)
                        {
                            Object? o_object = o_communication.DequeueWithWaitLoop(i_comDequeueWaitLoopTimeout);

                            if (o_object != null)
                            {
                                if (!p_b_smallObject)
                                {
                                    /* correct the small deviation with string null and empty string - empty strings are always interpreted as null */
                                    (((Sandbox.Tests.Net.Msg.MessageObject)o_object).StringArray ?? [])[5] = "";
                                    ((Sandbox.Tests.Net.Msg.MessageObject)o_object).StringList[5] = "";
                                }

                                a_serverObjects.Add(o_object);
                                ForestNET.Lib.Global.ILogFine("#" + (i + 1) + " object received");
                            }
                            else
                            {
                                ForestNET.Lib.Global.ILogWarning("could not receive any data");
                            }
                        }

                        o_communication?.Stop();

                        /* CHECK LOG ENTRIES */

                        Object? o_foo = null;

                        if (p_b_smallObject)
                        {
                            o_foo = new Sandbox.Tests.Net.Msg.SmallMessageObject();
                            ((Sandbox.Tests.Net.Msg.SmallMessageObject)o_foo).InitAll();
                        }
                        else
                        {
                            o_foo = new Sandbox.Tests.Net.Msg.MessageObject();
                            ((Sandbox.Tests.Net.Msg.MessageObject)o_foo).InitAll();
                        }

                        if (!p_b_falseUDPtrueTCP)
                        {
                            if (a_serverObjects.Count <= 7)
                            {
                                throw new Exception("server object list must have at least '8' entries, but has '" + a_serverObjects.Count + "'");
                            }

                            for (int i = 0; i < a_serverObjects.Count; i++)
                            {
                                if (!ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_serverObjects[i], o_foo, true, false, false))
                                {
                                    throw new Exception("server object is not equal to expected object");
                                }
                            }
                        }
                        else
                        {
                            if (a_serverObjects.Count != 10)
                            {
                                throw new Exception("server object list has not '10' entries, but has '" + a_serverObjects.Count + "'");
                            }

                            for (int i = 0; i < 10; i++)
                            {
                                if (!ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_serverObjects[i], o_foo, true, false, false))
                                {
                                    throw new Exception("server object is not equal to expected object");
                                }
                            }
                        }
                    }
                    catch (Exception o_exc)
                    {
                        ForestNET.Lib.Global.LogException(o_exc);
                    }
                });

                o_server.Wait();
            }
            else
            { /* CLIENT */
                Task o_client = Task.Run(() =>
                {
                    try
                    {
                        List<Object> a_clientObjects = [];

                        ForestNET.Lib.Net.Sock.Com.Type e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND;

                        if ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck))
                        {
                            e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND_WITH_ACK;
                        }

                        if (p_b_falseUDPtrueTCP)
                        {
                            e_type = ForestNET.Lib.Net.Sock.Com.Type.TCP_SEND;
                        }

                        ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(p_o_config.currentDirectory, e_type, ForestNET.Lib.Net.Sock.Com.Cardinality.Equal, p_o_config.serverIp, p_o_config.serverPort, ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck)) ? p_o_config.clientLocalIp : null, ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck)) ? p_o_config.clientLocalPort : 0, p_o_config.symmetricSecurity128, p_o_config.symmetricSecurity256, p_o_config.asymmetricSecurity, p_o_config.highSecurity, p_e_asymmetricMode, p_o_config.useMarshalling, p_o_config.useMarshallingWholeObject, p_o_config.marshallingDataLengthInBytes, p_o_config.marshallingUseProperties, p_o_config.marshallingOverrideMessageType, p_o_config.marshallingSystemUsesLittleEndian, p_o_config.thumbPrintServerCertificate);

                        if (p_b_falseUDPtrueTCP)
                        {
                            o_communicationConfig.ObjectTransmission = true;
                        }

                        ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                        o_communication.Start();

                        for (int i = 0; i < i_iterations; i++)
                        {
                            Object? o_foo = null;

                            if (p_b_smallObject)
                            {
                                o_foo = new Sandbox.Tests.Net.Msg.SmallMessageObject();
                                ((Sandbox.Tests.Net.Msg.SmallMessageObject)o_foo).InitAll();
                            }
                            else
                            {
                                o_foo = new Sandbox.Tests.Net.Msg.MessageObject();
                                ((Sandbox.Tests.Net.Msg.MessageObject)o_foo).InitAll();
                            }

                            while (!o_communication.Enqueue(
                                o_foo
                            ))
                            {
                                ForestNET.Lib.Global.ILogWarning("could not enqueue object");
                            }

                            ForestNET.Lib.Global.ILogFine("object enqueued");
                            a_clientObjects.Add(o_foo);

                            if (i == 4)
                            { /* additional delay for 25 milliseconds times sleep multiplier constant, after the 5th time enqueue has been executed */
                                Thread.Sleep(25 * i_sleepMultiplier);
                            }

                            Thread.Sleep(25 * i_sleepMultiplier + (((p_b_falseUDPtrueTCP) && (p_o_config.highSecurity)) ? 750 : 0) + (((!p_b_falseUDPtrueTCP) && (p_o_config.highSecurity)) ? 50 : 0) + ((p_b_udpWithAck) ? 225 : 0));
                        }

                        o_communication?.Stop();

                        /* CHECK LOG ENTRIES */

                        Object? o_bar = null;

                        if (p_b_smallObject)
                        {
                            o_bar = new Sandbox.Tests.Net.Msg.SmallMessageObject();
                            ((Sandbox.Tests.Net.Msg.SmallMessageObject)o_bar).InitAll();
                        }
                        else
                        {
                            o_bar = new Sandbox.Tests.Net.Msg.MessageObject();
                            ((Sandbox.Tests.Net.Msg.MessageObject)o_bar).InitAll();
                        }

                        if (!p_b_falseUDPtrueTCP)
                        {
                            if (a_clientObjects.Count <= 7)
                            {
                                throw new Exception("client object list must have at least '8' entries, but has '" + a_clientObjects.Count + "'");
                            }

                            for (int i = 0; i < a_clientObjects.Count; i++)
                            {
                                if (!ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_clientObjects[i], o_bar, true, false, false))
                                {
                                    throw new Exception("client object is not equal to expected object");
                                }
                            }
                        }
                        else
                        {
                            if (a_clientObjects.Count != 10)
                            {
                                throw new Exception("client object list has not '10' entries, but has '" + a_clientObjects.Count + "'");
                            }

                            for (int i = 0; i < 10; i++)
                            {
                                if (!ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_clientObjects[i], o_bar, true, false, false))
                                {
                                    throw new Exception("client object is not equal to expected object");
                                }
                            }
                        }
                    }
                    catch (Exception o_exc)
                    {
                        ForestNET.Lib.Global.LogException(o_exc);
                    }
                });

                o_client.Wait();
            }
        }

        private static void NetCommunicationMarshallingSharedMemoryUniDirectional(bool p_b_falseUDPtrueTCP, bool p_b_udpWithAck, bool p_b_smallObject, AsymmetricModes p_e_asymmetricMode, NetConfig p_o_config)
        {
            i_sleepMultiplier = p_o_config.sleepMultiplier;

            /* expected result */
            string s_expectedResult = "_Bool = True|_BoolArray = NULL|_BoolList = []|_Byte = 0|_ByteArray = NULL|_ByteList = [1, 3, 5, 133, 42, 0, NULL, 102]|_Char = o|_CharArray = NULL|_CharList = [A, F, K, " + ((char)133) + ", U, " + ((char)0) + ", NULL, ó]|_Date = 04.03.2020 00:00:00|_DateArray = NULL|_DateList = []|_DateTime = NULL|_DateTimeArray = [04.03.2020 06:02:03, 08.06.2020 09:24:16, 16.12.2020 12:48:53, NULL]|_DateTimeList = []|_Decimal = 0|_DecimalArray = NULL|_DecimalList = []|_Double = 0|_DoubleArray = NULL|_DoubleList = [1,25, 3,5, 5,75, 10,10, -41,998, 0, NULL, 8798546,2154656]|_Float = 0|_FloatArray = [1,25, 3,5, 5,75, 10,10, 41,998, 0, 4984654,5]|_FloatList = []|_Integer = 0|_IntegerArray = [1, 3, 5, 536870954, 42, 0]|_IntegerList = [1, 3, 5, 536870954, -42, 0, NULL]|_LocalDate = NULL|_LocalDateArray = [04.03.2020 00:00:00, 08.06.2020 00:00:00, 16.12.2020 00:00:00, NULL]|_LocalDateList = [04.03.2020 00:00:00, 08.06.2020 00:00:00, 16.12.2020 00:00:00, NULL]|_LocalDateTime = 04.03.2020 06:02:03|_LocalDateTimeArray = NULL|_LocalDateTimeList = [04.03.2020 06:02:03, 08.06.2020 09:24:16, 16.12.2020 12:48:53, NULL]|_LocalTime = 01.01.1970 22:16:06|_LocalTimeArray = NULL|_LocalTimeList = []|_Long = 1170936177994235946|_LongArray = NULL|_LongList = [1, 3, 5, 1170936177994235946, -42, 0, NULL]|_Short = 16426|_ShortArray = NULL|_ShortList = []|_SignedByte = 0|_SignedByteArray = NULL|_SignedByteList = []|_String = Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.|_StringArray = NULL|_StringList = [Hello World 1!, Hello World 2!, Hello World 3!, Hello World 4!, Hello World 5!, NULL, NULL]|_Time = 01.01.1970 06:02:03|_TimeArray = NULL|_TimeList = []|_UnsignedInteger = 536870954|_UnsignedIntegerArray = NULL|_UnsignedIntegerList = []|_UnsignedLong = 0|_UnsignedLongArray = [1, 3, 5, 1170936177994235946, 42, 0]|_UnsignedLongList = []|_UnsignedShort = 16426|_UnsignedShortArray = NULL|_UnsignedShortList = []|";

            if (p_b_smallObject)
            {
                s_expectedResult = "_Bool = True|_Char = o|_DecimalArray = NULL|_IntegerArray = [1, 3, 5, 536870954, 42, 0]|_LocalDateArray = [04.03.2020 00:00:00, 08.06.2020 00:00:00, 16.12.2020 00:00:00, NULL]|_LocalDateTimeList = [04.03.2020 06:02:03, 08.06.2020 09:24:16, 16.12.2020 12:48:53, NULL]|_LocalTime = 01.01.1970 22:16:06|_LongList = [1, 3, 5, 1170936177994235946, -42, 0, NULL]|_String = Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.|";
            }

            if (p_o_config.isServer)
            { /* SERVER */
                Task o_server = Task.Run(() =>
                {
                    try
                    {
                        List<string> a_expectedResults = [];
                        List<string> a_serverResults = [];

                        foreach (string s_foo in s_expectedResult.Split('|'))
                        {
                            a_expectedResults.Add(s_foo);
                        }

                        ForestNET.Lib.Net.Sock.Com.Type e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE;

                        if ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck))
                        {
                            e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE_WITH_ACK;
                        }

                        if (p_b_falseUDPtrueTCP)
                        {
                            e_type = ForestNET.Lib.Net.Sock.Com.Type.TCP_RECEIVE;
                        }

                        ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(p_o_config.currentDirectory, e_type, ForestNET.Lib.Net.Sock.Com.Cardinality.ManyMessageBoxesToOneSocket, p_o_config.serverIp, p_o_config.serverPort, null, 0, p_o_config.symmetricSecurity128, p_o_config.symmetricSecurity256, p_o_config.asymmetricSecurity, p_o_config.highSecurity, p_e_asymmetricMode, p_o_config.useMarshalling, p_o_config.useMarshallingWholeObject, p_o_config.marshallingDataLengthInBytes, p_o_config.marshallingUseProperties, p_o_config.marshallingOverrideMessageType, p_o_config.marshallingSystemUsesLittleEndian, p_o_config.thumbPrintServerCertificate);
                        ForestNET.Lib.Net.Sock.Com.ISharedMemory o_sharedMemory;

                        if (!p_b_smallObject)
                        {
                            o_sharedMemory = new SharedMemoryMessageObject();
                            ((SharedMemoryMessageObject)o_sharedMemory).EmptyAll();
                            o_sharedMemory.InitiateMirrors().Wait();
                            o_communicationConfig.SharedMemory = o_sharedMemory;
                        }
                        else
                        {
                            o_sharedMemory = new SharedMemorySmallMessageObject();
                            ((SharedMemorySmallMessageObject)o_sharedMemory).EmptyAll();
                            o_sharedMemory.InitiateMirrors().Wait();
                            o_communicationConfig.SharedMemory = o_sharedMemory;
                        }

                        o_communicationConfig.SharedMemoryTimeoutMilliseconds = 10;

                        ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                        o_communication.Start();

                        int i_additionalTime = 25;

                        if (p_b_falseUDPtrueTCP)
                        { /* TCP */
                            if (p_o_config.highSecurity)
                            {
                                i_additionalTime *= 3;
                            }

                            if (p_o_config.asymmetricSecurity)
                            {
                                i_additionalTime *= 3;
                            }

                            if (p_o_config.marshallingDataLengthInBytes > 3)
                            {
                                i_additionalTime *= (p_o_config.marshallingDataLengthInBytes / 2);
                            }
                        }
                        else
                        { /* UDP */
                            if (p_o_config.highSecurity)
                            {
                                i_additionalTime *= 5;
                            }

                            if (p_b_udpWithAck)
                            {
                                i_additionalTime += 225;
                            }

                            i_additionalTime *= p_o_config.marshallingDataLengthInBytes;
                        }

                        Thread.Sleep((25 * i_sleepMultiplier) + i_additionalTime);

                        Thread.Sleep((25 * i_sleepMultiplier) + i_additionalTime);

                        Thread.Sleep((25 * i_sleepMultiplier) + i_additionalTime);

                        Thread.Sleep((25 * i_sleepMultiplier) + i_additionalTime);

                        /* server result */
                        foreach (string s_foo in o_sharedMemory.ReturnFields().Split('|'))
                        {
                            if (s_foo.Contains("Decimal"))
                            {
                                a_serverResults.Add(DeleteUnnecessaryZeroes(s_foo));
                            }
                            else
                            {
                                a_serverResults.Add(s_foo);
                            }
                        }

                        o_communication?.Stop();

                        /* CHECK LOG ENTRIES */

                        int i_missingFieldsServer = 0;

                        for (int i = 0; i < a_serverResults.Count; i++)
                        {
                            if (!a_expectedResults[i].Equals(a_serverResults[i]))
                            {
                                i_missingFieldsServer++;
                                ForestNET.Lib.Global.ILog("server field result not equal expected result:\t" + a_serverResults[i] + "\t != \t" + a_expectedResults[i]);
                            }
                        }

                        if (i_missingFieldsServer > 0)
                        {
                            ForestNET.Lib.Global.ILog("uni " + ((p_b_falseUDPtrueTCP) ? "tcp" : "udp") + ((p_b_udpWithAck) ? " - with ACK" : "") + ((p_b_smallObject) ? " - small" : "") + " server missing fields: " + i_missingFieldsServer);
                        }
                        else
                        {
                            ForestNET.Lib.Global.ILogFine("uni " + ((p_b_falseUDPtrueTCP) ? "tcp" : "udp") + ((p_b_udpWithAck) ? " - with ACK" : "") + ((p_b_smallObject) ? " - small" : "") + " server missing fields: fine");
                        }

                        if (i_missingFieldsServer > 4)
                        {
                            throw new Exception(i_missingFieldsServer + " server fields not matching expected values, which is greater than '4'");
                        }
                    }
                    catch (Exception o_exc)
                    {
                        ForestNET.Lib.Global.LogException(o_exc);
                    }
                });

                o_server.Wait();
            }
            else
            { /* CLIENT */
                Task o_client = Task.Run(() =>
                {
                    try
                    {
                        List<string> a_expectedResults = [];
                        List<string> a_clientResults = [];

                        foreach (string s_foo in s_expectedResult.Split('|'))
                        {
                            a_expectedResults.Add(s_foo);
                        }

                        ForestNET.Lib.Net.Sock.Com.Type e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND;

                        if ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck))
                        {
                            e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND_WITH_ACK;
                        }

                        if (p_b_falseUDPtrueTCP)
                        {
                            e_type = ForestNET.Lib.Net.Sock.Com.Type.TCP_SEND;
                        }

                        ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(p_o_config.currentDirectory, e_type, ForestNET.Lib.Net.Sock.Com.Cardinality.ManyMessageBoxesToOneSocket, p_o_config.serverIp, p_o_config.serverPort, ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck)) ? p_o_config.clientLocalIp : null, ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck)) ? p_o_config.clientLocalPort : 0, p_o_config.symmetricSecurity128, p_o_config.symmetricSecurity256, p_o_config.asymmetricSecurity, p_o_config.highSecurity, p_e_asymmetricMode, p_o_config.useMarshalling, p_o_config.useMarshallingWholeObject, p_o_config.marshallingDataLengthInBytes, p_o_config.marshallingUseProperties, p_o_config.marshallingOverrideMessageType, p_o_config.marshallingSystemUsesLittleEndian, p_o_config.thumbPrintServerCertificate);
                        ForestNET.Lib.Net.Sock.Com.ISharedMemory o_sharedMemory;

                        if (!p_b_smallObject)
                        {
                            o_sharedMemory = new SharedMemoryMessageObject();
                            ((SharedMemoryMessageObject)o_sharedMemory).EmptyAll();
                            o_sharedMemory.InitiateMirrors().Wait();
                            o_communicationConfig.SharedMemory = o_sharedMemory;
                        }
                        else
                        {
                            o_sharedMemory = new SharedMemorySmallMessageObject();
                            ((SharedMemorySmallMessageObject)o_sharedMemory).EmptyAll();
                            o_sharedMemory.InitiateMirrors().Wait();
                            o_communicationConfig.SharedMemory = o_sharedMemory;
                        }

                        o_communicationConfig.SharedMemoryTimeoutMilliseconds = 10;

                        ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                        o_communication.Start();

                        int i_additionalTime = 25;

                        if (p_b_falseUDPtrueTCP)
                        { /* TCP */
                            if (p_o_config.highSecurity)
                            {
                                i_additionalTime *= 3;
                            }

                            if (p_o_config.asymmetricSecurity)
                            {
                                i_additionalTime *= 3;
                            }

                            if (p_o_config.marshallingDataLengthInBytes > 3)
                            {
                                i_additionalTime *= (p_o_config.marshallingDataLengthInBytes / 2);
                            }
                        }
                        else
                        { /* UDP */
                            if (p_o_config.highSecurity)
                            {
                                i_additionalTime *= 5;
                            }

                            if (p_b_udpWithAck)
                            {
                                i_additionalTime += 225;
                            }

                            i_additionalTime *= p_o_config.marshallingDataLengthInBytes;
                        }

                        Thread.Sleep((25 * i_sleepMultiplier) + i_additionalTime);

                        if (!p_b_smallObject)
                        {
                            o_sharedMemory.SetField("_String", "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.").Wait();
                            o_sharedMemory.SetField("_LocalTime", new DateTime(1970, 1, 1, 15, 32, 03)).Wait();
                            o_sharedMemory.SetField("_IntegerArray", new int[] { 1, 3, 5, 536870954, 42, 0 }).Wait();
                            o_sharedMemory.SetField("_Short", (short)16426).Wait();
                            o_sharedMemory.SetField("_UnsignedShort", (ushort)16426).Wait();
                            o_sharedMemory.SetField("_IntegerList", new List<int?>() { 1, 3, 5, 536870954, -42, 0, null }).Wait();
                            o_sharedMemory.SetField("_UnsignedInteger", (uint)536870954).Wait();
                            o_sharedMemory.SetField("_Long", 1170936177994235946L).Wait();
                            o_sharedMemory.SetField("_UnsignedLongArray", new ulong[] { 1L, 3L, 5L, 1170936177994235946L, 42L, 0L }).Wait();
                            o_sharedMemory.SetField("_StringList", new List<string?>() { "Hello World 1!", "Hello World 2!", "Hello World 3!", "Hello World 4!", "Hello World 5!", "", null }).Wait();
                            o_sharedMemory.SetField("_Time", new DateTime(1970, 1, 1, 6, 2, 3)).Wait();
                        }
                        else
                        {
                            o_sharedMemory.SetField("_String", "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.").Wait();
                            o_sharedMemory.SetField("_LocalTime", new DateTime(1970, 1, 1, 15, 32, 03)).Wait();
                            o_sharedMemory.SetField("_IntegerArray", new int[] { 1, 3, 5, 536870954, 42, 0 }).Wait();
                        }

                        Thread.Sleep((25 * i_sleepMultiplier) + i_additionalTime);

                        if (!p_b_smallObject)
                        {
                            o_sharedMemory.SetField("_LongList", new List<long?>() { 1L, 3L, 5L, 1170936177994235946L, -42L, 0L, null }).Wait();
                            o_sharedMemory.SetField("_Char", 'o').Wait();
                            o_sharedMemory.SetField("_String", "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.").Wait();
                            o_sharedMemory.SetField("_LocalDateArray", new DateTime?[] { new(2020, 3, 4, 0, 0, 0), new(2020, 6, 8, 0, 0, 0), new(2020, 12, 16, 0, 0, 0), null }).Wait();
                            o_sharedMemory.SetField("_Bool", true).Wait();
                            o_sharedMemory.SetField("_ByteList", new List<byte?>() { (byte)1, (byte)3, (byte)5, (byte)133, (byte)42, (byte)0, null, (byte)102 }).Wait();
                            o_sharedMemory.SetField("_UnsignedByte", (byte)42).Wait();
                            o_sharedMemory.SetField("_CharList", new List<char?>() { (char)65, (char)70, (char)75, (char)133, (char)85, (char)0, null, (char)243 }).Wait();
                            o_sharedMemory.SetField("_FloatArray", new float[] { 1.25f, 3.5f, 5.75f, 10.1010f, 41.998f, 0.0f, 4984654.5498795465f }).Wait();
                            o_sharedMemory.SetField("_DoubleList", new List<double?>() { 1.25d, 3.5d, 5.75d, 10.1010d, -41.998d, 0.0d, null, 8798546.2154656d }).Wait();
                        }
                        else
                        {
                            o_sharedMemory.SetField("_LongList", new List<long?>() { 1L, 3L, 5L, 1170936177994235946L, -42L, 0L, null }).Wait();
                            o_sharedMemory.SetField("_Char", 'o').Wait();
                            o_sharedMemory.SetField("_String", "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.").Wait();
                            o_sharedMemory.SetField("_LocalDateArray", new DateTime?[] { new(2020, 3, 4, 0, 0, 0), new(2020, 6, 8, 0, 0, 0), new(2020, 12, 16, 0, 0, 0), null }).Wait();
                            o_sharedMemory.SetField("_Bool", true).Wait();
                        }

                        Thread.Sleep((25 * i_sleepMultiplier) + i_additionalTime);

                        if (!p_b_smallObject)
                        {
                            o_sharedMemory.SetField("_LocalDateTimeList", new List<DateTime?>() { new(2020, 3, 4, 6, 2, 3), new(2020, 6, 8, 9, 24, 16), new(2020, 12, 16, 12, 48, 53), null }).Wait();
                            o_sharedMemory.SetField("_LocalTime", new DateTime(1970, 1, 1, 22, 16, 06)).Wait();
                            o_sharedMemory.SetField("_Date", new DateTime(2020, 3, 4, 0, 0, 0)).Wait();
                            o_sharedMemory.SetField("_DateTimeArray", new DateTime?[] {
                            new(2020, 3, 4, 6, 2, 3),
                            new(2020, 6, 8, 9, 24, 16),
                            new(2020, 12, 16, 12, 48, 53),
                            null
                        }).Wait();
                            o_sharedMemory.SetField("_LocalDateList", new List<DateTime?>() { new(2020, 3, 4, 0, 0, 0), new(2020, 6, 8, 0, 0, 0), new(2020, 12, 16, 0, 0, 0), null }).Wait();
                            o_sharedMemory.SetField("_LocalDateTime", new DateTime(2020, 3, 4, 6, 2, 3)).Wait();
                        }
                        else
                        {
                            o_sharedMemory.SetField("_LocalDateTimeList", new List<DateTime?>() { new(2020, 3, 4, 6, 2, 3), new(2020, 6, 8, 9, 24, 16), new(2020, 12, 16, 12, 48, 53), null }).Wait();
                            o_sharedMemory.SetField("_LocalTime", new DateTime(1970, 1, 1, 22, 16, 06)).Wait();
                        }

                        Thread.Sleep((25 * i_sleepMultiplier) + i_additionalTime);

                        /* client result */
                        foreach (string s_foo in o_sharedMemory.ReturnFields().Split('|'))
                        {
                            a_clientResults.Add(s_foo);
                        }

                        o_communication?.Stop();

                        /* CHECK LOG ENTRIES */

                        int i_missingFieldsClient = 0;

                        for (int i = 0; i < a_clientResults.Count; i++)
                        {
                            if (!a_expectedResults[i].Equals(a_clientResults[i]))
                            {
                                i_missingFieldsClient++;
                                ForestNET.Lib.Global.ILogFiner("client field result not equal expected result:\t" + a_clientResults[i] + "\t != \t" + a_expectedResults[i]);
                            }
                        }

                        if (i_missingFieldsClient > 0)
                        {
                            ForestNET.Lib.Global.ILog("uni " + ((p_b_falseUDPtrueTCP) ? "tcp" : "udp") + ((p_b_udpWithAck) ? " - with ACK" : "") + ((p_b_smallObject) ? " - small" : "") + " client missing fields: " + i_missingFieldsClient);
                        }
                        else
                        {
                            ForestNET.Lib.Global.ILogFine("uni " + ((p_b_falseUDPtrueTCP) ? "tcp" : "udp") + ((p_b_udpWithAck) ? " - with ACK" : "") + ((p_b_smallObject) ? " - small" : "") + " client missing fields: fine");
                        }

                        if (i_missingFieldsClient > 4)
                        {
                            throw new Exception(i_missingFieldsClient + " client fields not matching expected values, which is greater than '4'");
                        }
                    }
                    catch (Exception o_exc)
                    {
                        ForestNET.Lib.Global.LogException(o_exc);
                    }
                });

                o_client.Wait();
            }
        }

        private static void NetCommunicationMarshallingSharedMemoryBiDirectional(bool p_b_falseUDPtrueTCP, bool p_b_udpWithAck, bool p_b_smallObject, AsymmetricModes p_e_asymmetricMode, NetConfig p_o_config)
        {
            i_sleepMultiplier = p_o_config.sleepMultiplier;

            /* expected result */
            string s_expectedResult = "_Bool = True|_BoolArray = NULL|_BoolList = []|_Byte = 0|_ByteArray = NULL|_ByteList = [1, 3, 5, 133, 42, 0, NULL, 102]|_Char = o|_CharArray = NULL|_CharList = [A, F, K, " + ((char)133) + ", U, " + ((char)0) + ", NULL, ó]|_Date = 04.03.2020 00:00:00|_DateArray = NULL|_DateList = []|_DateTime = NULL|_DateTimeArray = [04.03.2020 06:02:03, 08.06.2020 09:24:16, 16.12.2020 12:48:53, NULL]|_DateTimeList = []|_Decimal = 0,0|_DecimalArray = NULL|_DecimalList = []|_Double = 0|_DoubleArray = NULL|_DoubleList = [1,25, 3,5, 5,75, 10,101, -41,998, 0, NULL, 8798546,2154656]|_Float = 0|_FloatArray = [1,25, 3,5, 5,75, 10,101, 41,998, 0, 4984654,5]|_FloatList = []|_Integer = 0|_IntegerArray = [1, 3, 5, 536870954, 42, 0]|_IntegerList = [1, 3, 5, 536870954, -42, 0, NULL]|_LocalDate = NULL|_LocalDateArray = [04.03.2020 00:00:00, 08.06.2020 00:00:00, 16.12.2020 00:00:00, NULL]|_LocalDateList = [04.03.2020 00:00:00, 08.06.2020 00:00:00, 16.12.2020 00:00:00, NULL]|_LocalDateTime = 04.03.2020 06:02:03|_LocalDateTimeArray = NULL|_LocalDateTimeList = [04.03.2020 06:02:03, 08.06.2020 09:24:16, 16.12.2020 12:48:53, NULL]|_LocalTime = 01.01.1970 22:16:06|_LocalTimeArray = NULL|_LocalTimeList = []|_Long = 1170936177994235946|_LongArray = NULL|_LongList = [1, 3, 5, 1170936177994235946, -42, 0, NULL]|_Short = 16426|_ShortArray = NULL|_ShortList = []|_SignedByte = 42|_SignedByteArray = NULL|_SignedByteList = []|_String = Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.|_StringArray = NULL|_StringList = [Hello World 1!, Hello World 2!, Hello World 3!, Hello World 4!, Hello World 5!, NULL, NULL]|_Time = 01.01.1970 06:02:03|_TimeArray = NULL|_TimeList = []|_UnsignedInteger = 536870954|_UnsignedIntegerArray = NULL|_UnsignedIntegerList = []|_UnsignedLong = 0|_UnsignedLongArray = [1, 3, 5, 1170936177994235946, 42, 0]|_UnsignedLongList = []|_UnsignedShort = 16426|_UnsignedShortArray = NULL|_UnsignedShortList = []|";

            if (p_b_smallObject)
            {
                s_expectedResult = "_Bool = True|_Char = o|_DecimalArray = NULL|_IntegerArray = [1, 3, 5, 536870954, 42, 0]|_LocalDateArray = [04.03.2020 00:00:00, 08.06.2020 00:00:00, 16.12.2020 00:00:00, NULL]|_LocalDateTimeList = [04.03.2020 06:02:03, 08.06.2020 09:24:16, 16.12.2020 12:48:53, NULL]|_LocalTime = 01.01.1970 22:16:06|_LongList = [1, 3, 5, 1170936177994235946, -42, 0, NULL]|_String = Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.|";
            }

            if (p_o_config.isServer)
            { /* SERVER */
                Task o_server = Task.Run(() =>
                {
                    try
                    {
                        List<string> a_expectedResults = [];
                        List<string> a_serverResults = [];

                        foreach (string s_foo in s_expectedResult.Split('|'))
                        {
                            a_expectedResults.Add(s_foo);
                        }

                        ForestNET.Lib.Net.Sock.Com.Type e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE;

                        if ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck))
                        {
                            e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE_WITH_ACK;
                        }

                        if (p_b_falseUDPtrueTCP)
                        {
                            e_type = ForestNET.Lib.Net.Sock.Com.Type.TCP_RECEIVE;
                        }

                        ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(p_o_config.currentDirectory, e_type, ForestNET.Lib.Net.Sock.Com.Cardinality.ManyMessageBoxesToOneSocket, p_o_config.serverIp, p_o_config.serverPort, null, 0, p_o_config.symmetricSecurity128, p_o_config.symmetricSecurity256, p_o_config.asymmetricSecurity, p_o_config.highSecurity, p_e_asymmetricMode, p_o_config.useMarshalling, p_o_config.useMarshallingWholeObject, p_o_config.marshallingDataLengthInBytes, p_o_config.marshallingUseProperties, p_o_config.marshallingOverrideMessageType, p_o_config.marshallingSystemUsesLittleEndian, p_o_config.thumbPrintServerCertificate);
                        ForestNET.Lib.Net.Sock.Com.ISharedMemory o_sharedMemory;

                        if (!p_b_smallObject)
                        {
                            o_sharedMemory = new SharedMemoryMessageObject();
                            ((SharedMemoryMessageObject)o_sharedMemory).EmptyAll();
                            o_sharedMemory.InitiateMirrors().Wait();
                            o_communicationConfig.SharedMemory = o_sharedMemory;
                        }
                        else
                        {
                            o_sharedMemory = new SharedMemorySmallMessageObject();
                            ((SharedMemorySmallMessageObject)o_sharedMemory).EmptyAll();
                            o_sharedMemory.InitiateMirrors().Wait();
                            o_communicationConfig.SharedMemory = o_sharedMemory;
                        }

                        o_communicationConfig.SharedMemoryTimeoutMilliseconds = 10;

                        if (p_o_config.asymmetricSecurity)
                        {
                            string? s_pathToCertificateFile = null;
                            string? s_pathToCertificateFilePassword = null;
                            string? s_certificateThumbprint = null;
                            System.Security.Cryptography.X509Certificates.StoreName? e_certificateStoreName = null;
                            System.Security.Cryptography.X509Certificates.StoreLocation? e_certificateStoreLocation = null;
                            string? s_clientRemoteCertificateName = null;
                            List<System.Security.Cryptography.X509Certificates.X509Certificate>? a_clientCertificateAllowList = null;

                            /* redo security settings for bidirectional side(client) */
                            if (p_e_asymmetricMode == AsymmetricModes.FileNoPassword)
                            {
                                s_pathToCertificateFile = p_o_config.currentDirectory + "Resources" + ForestNET.Lib.IO.File.DIR + "com" + ForestNET.Lib.IO.File.DIR + "client" + ForestNET.Lib.IO.File.DIR + "cert-client-no-pw.pfx";
                            }
                            else if (p_e_asymmetricMode == AsymmetricModes.FileWithPassword)
                            {
                                s_pathToCertificateFile = p_o_config.currentDirectory + "Resources" + ForestNET.Lib.IO.File.DIR + "com" + ForestNET.Lib.IO.File.DIR + "client" + ForestNET.Lib.IO.File.DIR + "cert-client-with-pw.pfx";
                                s_pathToCertificateFilePassword = "123456";
                            }
                            else if (p_e_asymmetricMode == AsymmetricModes.ThumbprintInStore)
                            {
                                s_certificateThumbprint = p_o_config.thumbPrintClientCertificate;
                                e_certificateStoreName = System.Security.Cryptography.X509Certificates.StoreName.Root;
                                e_certificateStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser;
                            }
                            else if (p_e_asymmetricMode == AsymmetricModes.ThumbprintInStoreAndClientRemoteCertificateName)
                            {
                                s_certificateThumbprint = p_o_config.thumbPrintClientCertificate;
                                e_certificateStoreName = System.Security.Cryptography.X509Certificates.StoreName.Root;
                                e_certificateStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser;
                                s_clientRemoteCertificateName = ThumbPrintClientCertificateName;
                            }

                            o_communicationConfig.SetSharedMemoryBidirectional(
                                new Dictionary<string, int> {
                                { p_o_config.serverBiIp, p_o_config.serverBiPort }
                                },
                                o_communicationConfig.SocketReceiveType ?? throw new NullReferenceException("socket receive type is null while setting shared memory bidirectional"),
                                s_pathToCertificateFile,
                                s_pathToCertificateFilePassword,
                                s_certificateThumbprint,
                                e_certificateStoreName,
                                e_certificateStoreLocation,
                                s_clientRemoteCertificateName,
                                a_clientCertificateAllowList
                            );
                        }
                        else
                        {
                            o_communicationConfig.SetSharedMemoryBidirectional(
                                new Dictionary<string, int> {
                                { p_o_config.serverBiIp, p_o_config.serverBiPort }
                                }
                            );
                        }

                        ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                        o_communication.Start();

                        int i_additionalTime = 25;

                        if (p_b_falseUDPtrueTCP)
                        { /* TCP */
                            if (p_o_config.highSecurity)
                            {
                                i_additionalTime *= 3;
                            }

                            if (p_o_config.asymmetricSecurity)
                            {
                                i_additionalTime *= 3;
                            }

                            if (p_o_config.marshallingDataLengthInBytes > 3)
                            {
                                i_additionalTime *= (p_o_config.marshallingDataLengthInBytes / 2);
                            }
                        }
                        else
                        { /* UDP */
                            if (p_o_config.highSecurity)
                            {
                                i_additionalTime *= 5;
                            }

                            if (p_b_udpWithAck)
                            {
                                i_additionalTime += 225;
                            }

                            i_additionalTime *= p_o_config.marshallingDataLengthInBytes;
                        }

                        Thread.Sleep((25 * i_sleepMultiplier) + i_additionalTime);

                        Thread.Sleep((25 * i_sleepMultiplier) + i_additionalTime);

                        if (!p_b_smallObject)
                        {
                            o_sharedMemory.SetField("_LongList", new List<long?>() { 1L, 3L, 5L, 1170936177994235946L, -42L, 0L, null }).Wait();
                            o_sharedMemory.SetField("_Char", 'o').Wait();
                            o_sharedMemory.SetField("_String", "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.").Wait();
                            o_sharedMemory.SetField("_LocalDateArray", new DateTime?[] { new(2020, 3, 4, 0, 0, 0), new(2020, 6, 8, 0, 0, 0), new(2020, 12, 16, 0, 0, 0), null }).Wait();
                            o_sharedMemory.SetField("_Bool", true).Wait();
                            o_sharedMemory.SetField("_ByteList", new List<byte?>() { (byte)1, (byte)3, (byte)5, (byte)133, (byte)42, (byte)0, null, (byte)102 }).Wait();
                            o_sharedMemory.SetField("_SignedByte", (sbyte)42).Wait();
                            o_sharedMemory.SetField("_CharList", new List<char?>() { (char)65, (char)70, (char)75, (char)133, (char)85, (char)0, null, (char)243 }).Wait();
                            o_sharedMemory.SetField("_FloatArray", new float[] { 1.25f, 3.5f, 5.75f, 10.1010f, 41.998f, 0.0f, 4984654.5498795465f }).Wait();
                            o_sharedMemory.SetField("_DoubleList", new List<double?>() { 1.25d, 3.5d, 5.75d, 10.1010d, -41.998d, 0.0d, null, 8798546.2154656d }).Wait();
                        }
                        else
                        {
                            o_sharedMemory.SetField("_LongList", new List<long?>() { 1L, 3L, 5L, 1170936177994235946L, -42L, 0L, null }).Wait();
                            o_sharedMemory.SetField("_Char", 'o').Wait();
                            o_sharedMemory.SetField("_String", "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.").Wait();
                            o_sharedMemory.SetField("_LocalDateArray", new DateTime?[] { new(2020, 3, 4, 0, 0, 0), new(2020, 6, 8, 0, 0, 0), new(2020, 12, 16, 0, 0, 0), null }).Wait();
                            o_sharedMemory.SetField("_Bool", true).Wait();
                        }

                        Thread.Sleep((25 * i_sleepMultiplier) + i_additionalTime);

                        Thread.Sleep((25 * i_sleepMultiplier) + i_additionalTime);

                        /* server result */
                        foreach (string s_foo in o_sharedMemory.ReturnFields().Split('|'))
                        {
                            a_serverResults.Add(s_foo);
                        }

                        o_communication?.Stop();

                        /* CHECK LOG ENTRIES */

                        int i_missingFieldsServer = 0;

                        for (int i = 0; i < a_serverResults.Count; i++)
                        {
                            if (!a_expectedResults[i].Equals(a_serverResults[i]))
                            {
                                i_missingFieldsServer++;
                                ForestNET.Lib.Global.ILogFiner("server field result not equal expected result:\t" + a_serverResults[i] + "\t != \t" + a_expectedResults[i]);
                            }
                        }

                        if (i_missingFieldsServer > 0)
                        {
                            ForestNET.Lib.Global.ILog("bi " + ((p_b_falseUDPtrueTCP) ? "tcp" : "udp") + ((p_b_udpWithAck) ? " - with ACK" : "") + ((p_b_smallObject) ? " - small" : "") + " server missing fields: " + i_missingFieldsServer);
                        }
                        else
                        {
                            ForestNET.Lib.Global.ILogFine("bi " + ((p_b_falseUDPtrueTCP) ? "tcp" : "udp") + ((p_b_udpWithAck) ? " - with ACK" : "") + ((p_b_smallObject) ? " - small" : "") + " server missing fields: fine");
                        }

                        if (i_missingFieldsServer > 6)
                        {
                            throw new Exception(i_missingFieldsServer + " server fields not matching expected values, which is greater than '6'");
                        }
                    }
                    catch (Exception o_exc)
                    {
                        ForestNET.Lib.Global.LogException(o_exc);
                    }
                });

                o_server.Wait();
            }
            else
            { /* CLIENT */
                Task o_client = Task.Run(() =>
                {
                    try
                    {
                        List<string> a_expectedResults = [];
                        List<string> a_clientResults = [];

                        foreach (string s_foo in s_expectedResult.Split('|'))
                        {
                            a_expectedResults.Add(s_foo);
                        }

                        ForestNET.Lib.Net.Sock.Com.Type e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND;

                        if ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck))
                        {
                            e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND_WITH_ACK;
                        }

                        if (p_b_falseUDPtrueTCP)
                        {
                            e_type = ForestNET.Lib.Net.Sock.Com.Type.TCP_SEND;
                        }

                        ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(p_o_config.currentDirectory, e_type, ForestNET.Lib.Net.Sock.Com.Cardinality.ManyMessageBoxesToOneSocket, p_o_config.serverIp, p_o_config.serverPort, ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck)) ? p_o_config.clientLocalIp : null, ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck)) ? p_o_config.clientLocalPort : 0, p_o_config.symmetricSecurity128, p_o_config.symmetricSecurity256, p_o_config.asymmetricSecurity, p_o_config.highSecurity, p_e_asymmetricMode, p_o_config.useMarshalling, p_o_config.useMarshallingWholeObject, p_o_config.marshallingDataLengthInBytes, p_o_config.marshallingUseProperties, p_o_config.marshallingOverrideMessageType, p_o_config.marshallingSystemUsesLittleEndian, p_o_config.thumbPrintServerCertificate);
                        ForestNET.Lib.Net.Sock.Com.ISharedMemory o_sharedMemory;

                        if (!p_b_smallObject)
                        {
                            o_sharedMemory = new SharedMemoryMessageObject();
                            ((SharedMemoryMessageObject)o_sharedMemory).EmptyAll();
                            o_sharedMemory.InitiateMirrors().Wait();
                            o_communicationConfig.SharedMemory = o_sharedMemory;
                        }
                        else
                        {
                            o_sharedMemory = new SharedMemorySmallMessageObject();
                            ((SharedMemorySmallMessageObject)o_sharedMemory).EmptyAll();
                            o_sharedMemory.InitiateMirrors().Wait();
                            o_communicationConfig.SharedMemory = o_sharedMemory;
                        }

                        o_communicationConfig.SharedMemoryTimeoutMilliseconds = 10;

                        if (p_o_config.asymmetricSecurity)
                        {
                            string? s_pathToCertificateFile = null;
                            string? s_pathToCertificateFilePassword = null;
                            string? s_certificateThumbprint = null;
                            System.Security.Cryptography.X509Certificates.StoreName? e_certificateStoreName = null;
                            System.Security.Cryptography.X509Certificates.StoreLocation? e_certificateStoreLocation = null;
                            string? s_clientRemoteCertificateName = null;
                            List<System.Security.Cryptography.X509Certificates.X509Certificate>? a_clientCertificateAllowList = null;

                            /* redo security settings for bidirectional side(client) */
                            if (p_e_asymmetricMode == AsymmetricModes.FileNoPassword)
                            {
                                s_pathToCertificateFile = p_o_config.currentDirectory + "Resources" + ForestNET.Lib.IO.File.DIR + "com" + ForestNET.Lib.IO.File.DIR + "client" + ForestNET.Lib.IO.File.DIR + "cert-client-no-pw.pfx";
                            }
                            else if (p_e_asymmetricMode == AsymmetricModes.FileWithPassword)
                            {
                                s_pathToCertificateFile = p_o_config.currentDirectory + "Resources" + ForestNET.Lib.IO.File.DIR + "com" + ForestNET.Lib.IO.File.DIR + "client" + ForestNET.Lib.IO.File.DIR + "cert-client-with-pw.pfx";
                                s_pathToCertificateFilePassword = "123456";
                            }
                            else if (p_e_asymmetricMode == AsymmetricModes.ThumbprintInStore)
                            {
                                s_certificateThumbprint = p_o_config.thumbPrintClientCertificate;
                                e_certificateStoreName = System.Security.Cryptography.X509Certificates.StoreName.Root;
                                e_certificateStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser;
                            }
                            else if (p_e_asymmetricMode == AsymmetricModes.ThumbprintInStoreAndClientRemoteCertificateName)
                            {
                                s_certificateThumbprint = p_o_config.thumbPrintClientCertificate;
                                e_certificateStoreName = System.Security.Cryptography.X509Certificates.StoreName.Root;
                                e_certificateStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser;
                                s_clientRemoteCertificateName = ThumbPrintClientCertificateName;
                            }

                            o_communicationConfig.SetSharedMemoryBidirectional(
                                new Dictionary<string, int> {
                                { p_o_config.serverBiIp, p_o_config.serverBiPort }
                                },
                                o_communicationConfig.SocketReceiveType ?? throw new NullReferenceException("socket receive type is null while setting shared memory bidirectional"),
                                s_pathToCertificateFile,
                                s_pathToCertificateFilePassword,
                                s_certificateThumbprint,
                                e_certificateStoreName,
                                e_certificateStoreLocation,
                                s_clientRemoteCertificateName,
                                a_clientCertificateAllowList
                            );
                        }
                        else
                        {
                            o_communicationConfig.SetSharedMemoryBidirectional(
                                new Dictionary<string, int> {
                                { p_o_config.serverBiIp, p_o_config.serverBiPort }
                                }
                            );
                        }

                        ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                        o_communication.Start();

                        int i_additionalTime = 25;

                        if (p_b_falseUDPtrueTCP)
                        { /* TCP */
                            if (p_o_config.highSecurity)
                            {
                                i_additionalTime *= 3;
                            }

                            if (p_o_config.asymmetricSecurity)
                            {
                                i_additionalTime *= 3;
                            }

                            if (p_o_config.marshallingDataLengthInBytes > 3)
                            {
                                i_additionalTime *= (p_o_config.marshallingDataLengthInBytes / 2);
                            }
                        }
                        else
                        { /* UDP */
                            if (p_o_config.highSecurity)
                            {
                                i_additionalTime *= 5;
                            }

                            if (p_b_udpWithAck)
                            {
                                i_additionalTime += 225;
                            }

                            i_additionalTime *= p_o_config.marshallingDataLengthInBytes;
                        }

                        Thread.Sleep((25 * i_sleepMultiplier) + i_additionalTime);

                        if (!p_b_smallObject)
                        {
                            o_sharedMemory.SetField("_String", "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.");
                            o_sharedMemory.SetField("_LocalTime", new DateTime(1970, 1, 1, 15, 32, 03)).Wait();
                            o_sharedMemory.SetField("_IntegerArray", new int[] { 1, 3, 5, 536870954, 42, 0 }).Wait();
                            o_sharedMemory.SetField("_Short", (short)16426).Wait();
                            o_sharedMemory.SetField("_UnsignedShort", (ushort)16426).Wait();
                            o_sharedMemory.SetField("_IntegerList", new List<int?>() { 1, 3, 5, 536870954, -42, 0, null }).Wait();
                            o_sharedMemory.SetField("_UnsignedInteger", (uint)536870954).Wait();
                            o_sharedMemory.SetField("_Long", 1170936177994235946L).Wait();
                            o_sharedMemory.SetField("_UnsignedLongArray", new ulong[] { 1L, 3L, 5L, 1170936177994235946L, 42L, 0L }).Wait();
                            o_sharedMemory.SetField("_StringList", new List<string?>() { "Hello World 1!", "Hello World 2!", "Hello World 3!", "Hello World 4!", "Hello World 5!", "", null }).Wait();
                            o_sharedMemory.SetField("_Time", new DateTime(1970, 1, 1, 6, 2, 3)).Wait();
                        }
                        else
                        {
                            o_sharedMemory.SetField("_String", "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.").Wait();
                            o_sharedMemory.SetField("_LocalTime", new DateTime(1970, 1, 1, 15, 32, 03)).Wait();
                            o_sharedMemory.SetField("_IntegerArray", new int[] { 1, 3, 5, 536870954, 42, 0 }).Wait();
                        }

                        Thread.Sleep((25 * i_sleepMultiplier) + i_additionalTime);

                        Thread.Sleep((25 * i_sleepMultiplier) + i_additionalTime);

                        if (!p_b_smallObject)
                        {
                            o_sharedMemory.SetField("_LocalDateTimeList", new List<DateTime?>() { new(2020, 3, 4, 6, 2, 3), new(2020, 6, 8, 9, 24, 16), new(2020, 12, 16, 12, 48, 53), null }).Wait();
                            o_sharedMemory.SetField("_LocalTime", new DateTime(1970, 1, 1, 22, 16, 06)).Wait();
                            o_sharedMemory.SetField("_Date", new DateTime(2020, 3, 4, 0, 0, 0)).Wait();
                            o_sharedMemory.SetField("_DateTimeArray", new DateTime?[] {
                            new(2020, 3, 4, 6, 2, 3),
                            new(2020, 6, 8, 9, 24, 16),
                            new(2020, 12, 16, 12, 48, 53),
                            null
                        }).Wait();
                            o_sharedMemory.SetField("_LocalDateList", new List<DateTime?>() { new(2020, 3, 4, 0, 0, 0), new(2020, 6, 8, 0, 0, 0), new(2020, 12, 16, 0, 0, 0), null }).Wait();
                            o_sharedMemory.SetField("_LocalDateTime", new DateTime(2020, 3, 4, 6, 2, 3)).Wait();
                        }
                        else
                        {
                            o_sharedMemory.SetField("_LocalDateTimeList", new List<DateTime?>() { new(2020, 3, 4, 6, 2, 3), new(2020, 6, 8, 9, 24, 16), new(2020, 12, 16, 12, 48, 53), null }).Wait();
                            o_sharedMemory.SetField("_LocalTime", new DateTime(1970, 1, 1, 22, 16, 06)).Wait();
                        }

                        Thread.Sleep((25 * i_sleepMultiplier) + i_additionalTime);

                        /* client result */
                        foreach (string s_foo in o_sharedMemory.ReturnFields().Split('|'))
                        {
                            a_clientResults.Add(s_foo);
                        }

                        o_communication?.Stop();

                        /* CHECK LOG ENTRIES */

                        int i_missingFieldsClient = 0;

                        for (int i = 0; i < a_clientResults.Count; i++)
                        {
                            if (!a_expectedResults[i].Equals(a_clientResults[i]))
                            {
                                i_missingFieldsClient++;
                                ForestNET.Lib.Global.ILogFiner("client field result not equal expected result:\t" + a_clientResults[i] + "\t != \t" + a_expectedResults[i]);
                            }
                        }

                        if (i_missingFieldsClient > 0)
                        {
                            ForestNET.Lib.Global.ILog("bi " + ((p_b_falseUDPtrueTCP) ? "tcp" : "udp") + ((p_b_udpWithAck) ? " - with ACK" : "") + ((p_b_smallObject) ? " - small" : "") + " client missing fields: " + i_missingFieldsClient);
                        }
                        else
                        {
                            ForestNET.Lib.Global.ILogFine("bi " + ((p_b_falseUDPtrueTCP) ? "tcp" : "udp") + ((p_b_udpWithAck) ? " - with ACK" : "") + ((p_b_smallObject) ? " - small" : "") + " client missing fields: fine");
                        }

                        if (i_missingFieldsClient > 6)
                        {
                            throw new Exception(i_missingFieldsClient + " client fields not matching expected values, which is greater than '6'");
                        }
                    }
                    catch (Exception o_exc)
                    {
                        ForestNET.Lib.Global.LogException(o_exc);
                    }
                });

                o_client.Wait();
            }
        }

        private static string DeleteUnnecessaryZeroes(string p_s_input)
        {
            string s_foo = "";

            bool b_comma = false;

            for (int i = 0; i < p_s_input.Length; i++)
            {
                char c_foo = p_s_input[i];

                if ((c_foo == '0') && (b_comma))
                {
                    if ((p_s_input[i - 2] == ',') && ((p_s_input[i + 1] == ',') || (p_s_input[i + 1] == '|') || (p_s_input[i + 1] == ']')))
                    {
                        s_foo += p_s_input[i];
                        b_comma = false;
                    }
                    else
                    {
                        int i_old = i;

                        while (i < p_s_input.Length - 1)
                        {
                            if (p_s_input[++i] == '0')
                            {
                                continue;
                            }
                            else if ((p_s_input[i] == ',') || (p_s_input[i] == '|') || (p_s_input[i] == ']'))
                            {
                                if (s_foo[s_foo.Length - 2] == ',')
                                    s_foo += "0";

                                s_foo += p_s_input[i];

                                if (s_foo.EndsWith(",,"))
                                {
                                    s_foo = s_foo.Substring(0, s_foo.Length - 1);
                                    s_foo += "00,";

                                    if ((s_foo.EndsWith(" 0,00,")) || (s_foo.EndsWith("[0,00,")))
                                    {
                                        s_foo = s_foo.Substring(0, s_foo.Length - 4);
                                        s_foo += ",";
                                    }
                                }
                                else if (s_foo.EndsWith(",|"))
                                {
                                    s_foo = s_foo.Substring(0, s_foo.Length - 1);
                                    s_foo += "00|";

                                    if ((s_foo.EndsWith(" 0,00|")) || (s_foo.EndsWith("[0,00|")))
                                    {
                                        s_foo = s_foo.Substring(0, s_foo.Length - 4);
                                        s_foo += "|";
                                    }
                                }
                                else if (s_foo.EndsWith(",]"))
                                {
                                    s_foo = s_foo.Substring(0, s_foo.Length - 1);
                                    s_foo += "00]";

                                    if ((s_foo.EndsWith(" 0,00]")) || (s_foo.EndsWith("[0,00]")))
                                    {
                                        s_foo = s_foo.Substring(0, s_foo.Length - 4);
                                        s_foo += "]";
                                    }
                                }

                                b_comma = false;
                                break;
                            }
                            else
                            {
                                for (int j = i_old; j < i; j++)
                                {
                                    s_foo += p_s_input[j];
                                }

                                i_old = i;

                                if ((i < p_s_input.Length - 1) && (p_s_input[i + 1] == '0'))
                                {
                                    s_foo += p_s_input[i];
                                    i_old++;
                                }
                            }
                        }
                    }
                }
                else if ((c_foo == ',') && (!b_comma) && (p_s_input[i + 1] != ' '))
                {
                    b_comma = true;
                    s_foo += c_foo;
                }
                else if (((p_s_input[i] == ',') || (p_s_input[i] == '|') || (p_s_input[i] == ']')) && (b_comma))
                {
                    b_comma = false;
                    s_foo += c_foo;
                }
                else
                {
                    s_foo += c_foo;
                }
            }

            return s_foo;
        }
    }
}
