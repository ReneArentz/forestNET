namespace ForestNET.Tests.Net.Sock.Com
{
    public class CommunicationTest
    {
        /**
		 * !!!!!!!!!!!!!!!!!
		 * it is maybe necessary to update server and client thumbprints
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

        private const string ThumbPrintServerCertificate = "c17c7d53d9cc389d2de9c557e9436b2668bdab32";
        private const string ThumbPrintServerCertificateName = "forestNET Server PW";
        private const string ThumbPrintClientCertificate = "832f4237e13b4c6a91c2691fb0f6eeb3396013a1";
        private const string ThumbPrintClientCertificateName = "forestNET Client PW";

        /* sleep multiplier for test cycle executions */
        private const int i_sleepMultiplier = 15;

        /* counter for total test fails, but we will tolerate some */
        private static int i_fails = 0;

#pragma warning disable IDE0044 // Add readonly modifier
        private static string s_currentDirectory = string.Empty;
#pragma warning restore IDE0044 // Add readonly modifier

        [Test]
        public void TestCommunicationMultipleTimes()
        {
            try
            {
                TestConfig.InitiateTestLogging();

                int i_iterations = 1;

                for (int i = 0; i < i_iterations; i++)
                {
                    if (i_iterations > 1)
                    {
                        Console.WriteLine("#" + (i + 1) + " - " + DateTime.Now);
                    }

                    TestCommunication();

                    try
                    {
                        /* 10 milliseconds to close any connections */
                        Thread.Sleep(10);
                    }
                    catch (Exception e)
                    {
                        ForestNET.Lib.Global.LogException(e);
                    }
                }

                if (i_fails >= 8)
                {
                    throw new Exception("overall fails '" + i_fails + "' are greater than '8'");
                }

                if (i_iterations > 1)
                {
                    ForestNET.Lib.Global.ILogWarning("INFO: Iteration #" + i_iterations + " completed");
                }

                Thread.Sleep(2500);

                ForestNET.Lib.Global.ILogWarning(">>>>>> End TestCommunication <<<<<<");
            }
            catch (Exception o_exc)
            {
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
        }

        private static void TestCommunication()
        {
            bool b_skipUDP = false;
            bool b_skipTCP = false;
            bool b_skipMarshalling = false;
            bool b_skipUDPMarshalling = false;
            bool b_skipTCPMarshalling = false;

            bool b_enhancedUDP = false;
            bool b_enhancedTCP = false;

            if (!b_skipUDP)
            {
                TestUDP(b_enhancedUDP, false, 2);
            }

            if (!b_skipTCP)
            {
                TestTCP(b_enhancedTCP, false, 2);
            }

            if (!b_skipMarshalling)
            {
                TestMarshalling(b_skipUDPMarshalling, b_skipTCPMarshalling, b_enhancedUDP, b_enhancedTCP);
            }
        }

        private static void TestUDP(bool p_b_enhancedUDP, bool p_b_useMarshalling, int p_i_marshallingDataLengthInBytes)
        {
            bool b_symmetricSecurity128 = false;
            bool b_symmetricSecurity256 = false;
            bool b_asymmetricSecurity = false;
            bool b_highSecurity = false;

            bool b_useMarshalling = p_b_useMarshalling;
            bool b_useMarshallingWholeObject = false;
            int i_marshallingDataLengthInBytes = p_i_marshallingDataLengthInBytes;
            bool b_marshallingUseProperties = false;
            string? s_marshallingOverrideMessageType = null;
            bool b_marshallingSystemUsesLittleEndian = false;

            int i_iterationsUDP = 1;

            if (p_b_enhancedUDP)
            {
                i_iterationsUDP = 5;
            }

            for (int i = 0; i < i_iterationsUDP; i++)
            {
                if (i == 0)
                {
                    ForestNET.Lib.Global.ILogWarning("INFO: no security");
                }
                else if (i == 1)
                {
                    b_symmetricSecurity128 = true;
                    b_symmetricSecurity256 = false;
                    b_asymmetricSecurity = false;
                    b_highSecurity = false;
                    ForestNET.Lib.Global.ILogWarning("INFO: 128-bit security");
                }
                else if (i == 2)
                {
                    b_symmetricSecurity128 = true;
                    b_symmetricSecurity256 = false;
                    b_asymmetricSecurity = false;
                    b_highSecurity = true;
                    ForestNET.Lib.Global.ILogWarning("INFO: 128-bit 'high' security");
                }
                else if (i == 3)
                {
                    b_symmetricSecurity128 = false;
                    b_symmetricSecurity256 = true;
                    b_asymmetricSecurity = false;
                    b_highSecurity = false;
                    ForestNET.Lib.Global.ILogWarning("INFO: 256-bit security");
                }
                else if (i == 4)
                {
                    b_symmetricSecurity128 = false;
                    b_symmetricSecurity256 = true;
                    b_asymmetricSecurity = false;
                    b_highSecurity = true;
                    ForestNET.Lib.Global.ILogWarning("INFO: 256-bit 'high' security");
                }

                ForestNET.Lib.Global.ILogWarning("INFO: testCommunication UDP" + ((b_useMarshalling) ? " marshalling" : ""));
                TestCommunication(false, false, "127.0.0.1", 8080, "127.0.0.1", 8080, null, 0, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, AsymmetricModes.None, b_useMarshalling, b_useMarshallingWholeObject, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);

                ForestNET.Lib.Global.ILogWarning("INFO: testCommunication UDP with ACK" + ((b_useMarshalling) ? " marshalling" : ""));
                TestCommunication(false, true, "127.0.0.1", 8080, "127.0.0.1", 8080, "127.0.0.1", 42333, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, AsymmetricModes.None, b_useMarshalling, b_useMarshallingWholeObject, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);

                ForestNET.Lib.Global.ILogWarning("INFO: testCommunication UDP with Multicast" + ((b_useMarshalling) ? " marshalling" : ""));
                TestCommunicationUDPMulticast(false, "239.255.1.2", 12080, "239.255.1.2", 12080, null, 0, b_symmetricSecurity128, b_symmetricSecurity256, b_highSecurity, b_useMarshalling, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);

                ForestNET.Lib.Global.ILogWarning("INFO: testCommunication UDP with IPv6 Multicast" + ((b_useMarshalling) ? " marshalling" : ""));
                TestCommunicationUDPMulticast(true, "FF05:0:0:0:0:0:0:342", 12080, "FF05:0:0:0:0:0:0:342", 12080, null, 0, b_symmetricSecurity128, b_symmetricSecurity256, b_highSecurity, b_useMarshalling, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);

                if (p_b_enhancedUDP)
                {
                    ForestNET.Lib.Global.ILogWarning("INFO: testCommunicationSharedMemoryUniDirectional UDP" + ((b_useMarshalling) ? " marshalling" : ""));
                    TestCommunicationSharedMemoryUniDirectional(false, false, "127.0.0.1", 8080, "127.0.0.1", 8080, null, 0, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, AsymmetricModes.None, b_useMarshalling, b_useMarshallingWholeObject, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);

                    ForestNET.Lib.Global.ILogWarning("INFO: testCommunicationSharedMemoryUniDirectional UDP with ACK" + ((b_useMarshalling) ? " marshalling" : ""));
                    TestCommunicationSharedMemoryUniDirectional(false, true, "127.0.0.1", 8080, "127.0.0.1", 8080, "127.0.0.1", 42333, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, AsymmetricModes.None, b_useMarshalling, b_useMarshallingWholeObject, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);

                    ForestNET.Lib.Global.ILogWarning("INFO: testCommunicationSharedMemoryBiDirectional UDP" + ((b_useMarshalling) ? " marshalling" : ""));
                    TestCommunicationSharedMemoryBiDirectional(false, false, "127.0.0.1", 8080, "127.0.0.1", 8080, "127.0.0.1", 8081, "127.0.0.1", 8081, null, 0, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, AsymmetricModes.None, b_useMarshalling, b_useMarshallingWholeObject, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);

                    ForestNET.Lib.Global.ILogWarning("INFO: testCommunicationSharedMemoryBiDirectional UDP with ACK" + ((b_useMarshalling) ? " marshalling" : ""));
                    TestCommunicationSharedMemoryBiDirectional(false, true, "127.0.0.1", 8080, "127.0.0.1", 8080, "127.0.0.1", 8081, "127.0.0.1", 8081, "127.0.0.1", 42333, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, AsymmetricModes.None, b_useMarshalling, b_useMarshallingWholeObject, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);
                }
            }
        }

        private static void TestTCP(bool p_b_enhancedTCP, bool p_b_useMarshalling, int p_i_marshallingDataLengthInBytes)
        {
            bool b_symmetricSecurity128 = false;
            bool b_symmetricSecurity256 = false;
            bool b_asymmetricSecurity = false;
            bool b_highSecurity = false;

            bool b_useMarshalling = p_b_useMarshalling;
            bool b_useMarshallingWholeObject = false;
            int i_marshallingDataLengthInBytes = p_i_marshallingDataLengthInBytes;
            bool b_marshallingUseProperties = false;
            string? s_marshallingOverrideMessageType = null;
            bool b_marshallingSystemUsesLittleEndian = false;

            AsymmetricModes e_asymmetricMode;

            for (int i = 0; i < 6; i++)
            {
                if (i == 0)
                {
                    ForestNET.Lib.Global.ILogWarning("INFO: no security");

                    if (!p_b_enhancedTCP)
                    {
                        i = 4; /* jump directly to asymmetric security on next iteration */
                    }
                }
                else if (i == 1)
                {
                    b_symmetricSecurity128 = true;
                    b_symmetricSecurity256 = false;
                    b_asymmetricSecurity = false;
                    b_highSecurity = false;
                    ForestNET.Lib.Global.ILogWarning("INFO: 128-bit security");
                }
                else if (i == 2)
                {
                    b_symmetricSecurity128 = true;
                    b_symmetricSecurity256 = false;
                    b_asymmetricSecurity = false;
                    b_highSecurity = true;
                    ForestNET.Lib.Global.ILogWarning("INFO: 128-bit 'high' security");
                }
                else if (i == 3)
                {
                    b_symmetricSecurity128 = false;
                    b_symmetricSecurity256 = true;
                    b_asymmetricSecurity = false;
                    b_highSecurity = false;
                    ForestNET.Lib.Global.ILogWarning("INFO: 256-bit security");
                }
                else if (i == 4)
                {
                    b_symmetricSecurity128 = false;
                    b_symmetricSecurity256 = true;
                    b_asymmetricSecurity = false;
                    b_highSecurity = true;
                    ForestNET.Lib.Global.ILogWarning("INFO: 256-bit 'high' security");
                }
                else if (i == 5)
                {
                    b_symmetricSecurity128 = false;
                    b_symmetricSecurity256 = false;
                    b_asymmetricSecurity = true;
                    b_highSecurity = false;
                    ForestNET.Lib.Global.ILogWarning("INFO: asymmetric security");
                }

#pragma warning disable IDE0059 // Unnecessary assignment of a value
                e_asymmetricMode = AsymmetricModes.FileNoPassword;
#pragma warning restore IDE0059 // Unnecessary assignment of a value

                ForestNET.Lib.Global.ILogWarning("INFO: testCommunication TCP Bidirectional" + ((b_useMarshalling) ? " marshalling" : ""));
                TestCommunicationTCPBidirectional("127.0.0.1", 8080, "127.0.0.1", 8080, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, b_useMarshalling, b_useMarshallingWholeObject, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);

                ForestNET.Lib.Global.ILogWarning("INFO: testCommunication TCP HandshakeTask" + ((b_useMarshalling) ? " marshalling" : ""));
                TestCommunicationTCPHandshakeTask("127.0.0.1", 8080, "127.0.0.1", 8080, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, b_useMarshalling, b_useMarshallingWholeObject, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);

                e_asymmetricMode = AsymmetricModes.ThumbprintInStore;

                ForestNET.Lib.Global.ILogWarning("INFO: testCommunication TCP" + ((b_useMarshalling) ? " marshalling" : ""));
                TestCommunication(true, false, "127.0.0.1", 8080, "127.0.0.1", 8080, null, 0, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, e_asymmetricMode, b_useMarshalling, b_useMarshallingWholeObject, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);

                e_asymmetricMode = AsymmetricModes.FileNoPassword;

                ForestNET.Lib.Global.ILogWarning("INFO: testCommunicationObjectTransmission TCP" + ((b_useMarshalling) ? " marshalling" : ""));
                TestCommunicationObjectTransmissionTCP("127.0.0.1", 8080, "127.0.0.1", 8080, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, e_asymmetricMode, b_useMarshalling, b_useMarshallingWholeObject, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);

                e_asymmetricMode = AsymmetricModes.FileWithPassword;

                ForestNET.Lib.Global.ILogWarning("INFO: testCommunicationWithAnswerOne TCP" + ((b_useMarshalling) ? " marshalling" : ""));
                TestCommunicationWithAnswerOneTCP("127.0.0.1", 8080, "127.0.0.1", 8080, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, e_asymmetricMode, b_useMarshalling, b_useMarshallingWholeObject, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);

                e_asymmetricMode = AsymmetricModes.ThumbprintInStoreAndClientRemoteCertificateName;

                ForestNET.Lib.Global.ILogWarning("INFO: testCommunicationWithAnswerTwo TCP" + ((b_useMarshalling) ? " marshalling" : ""));
                TestCommunicationWithAnswerTwoTCP("127.0.0.1", 8080, "127.0.0.1", 8080, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, e_asymmetricMode, b_useMarshalling, b_useMarshallingWholeObject, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);

                e_asymmetricMode = AsymmetricModes.FileWithPassword;

                ForestNET.Lib.Global.ILogWarning("INFO: testCommunicationSharedMemoryUniDirectional TCP" + ((b_useMarshalling) ? " marshalling" : ""));
                TestCommunicationSharedMemoryUniDirectional(true, false, "127.0.0.1", 8080, "127.0.0.1", 8080, null, 0, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, e_asymmetricMode, b_useMarshalling, b_useMarshallingWholeObject, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);

                e_asymmetricMode = AsymmetricModes.ThumbprintInStore;

                ForestNET.Lib.Global.ILogWarning("INFO: testCommunicationSharedMemoryBiDirectional TCP" + ((b_useMarshalling) ? " marshalling" : ""));
                TestCommunicationSharedMemoryBiDirectional(true, false, "127.0.0.1", 8080, "127.0.0.1", 8080, "127.0.0.1", 8081, "127.0.0.1", 8081, null, 0, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, e_asymmetricMode, b_useMarshalling, b_useMarshallingWholeObject, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);
            }
        }

        private static void TestMarshalling(bool p_b_skipUDPMarshalling, bool p_b_skipTCPMarshalling, bool p_b_enhancedUDP, bool p_b_enhancedTCP)
        {
            bool b_enhancedMarshalling = false;
            bool b_marshallingOnlyDataLengthTwo = true;
            bool b_skipUniAndBiDirectional = true;
            int i_marshallingDataLengthInBytes = 1;

            int i_marshallingStart = 0;
            int i_marshallingEnd = 4;

            if (b_marshallingOnlyDataLengthTwo)
            {
                i_marshallingStart = 1;
                i_marshallingEnd = 2;
            }

            for (int i = i_marshallingStart; i < i_marshallingEnd; i++)
            {
                bool b_symmetricSecurity128 = false;
                bool b_symmetricSecurity256 = false;
                bool b_asymmetricSecurity = false;
                bool b_highSecurity = false;

                AsymmetricModes e_asymmetricMode;

                if (i == 0)
                {
                    i_marshallingDataLengthInBytes = 1;
                    ForestNET.Lib.Global.ILogWarning("INFO: Marshalling - data length in bytes = 1");
                }
                else if (i == 1)
                {
                    i_marshallingDataLengthInBytes = 2;
                    ForestNET.Lib.Global.ILogWarning("INFO: Marshalling - data length in bytes = 2");
                }
                else if (i == 2)
                {
                    i_marshallingDataLengthInBytes = 3;
                    ForestNET.Lib.Global.ILogWarning("INFO: Marshalling - data length in bytes = 3");
                }
                else if (i == 3)
                {
                    i_marshallingDataLengthInBytes = 4;
                    ForestNET.Lib.Global.ILogWarning("INFO: Marshalling - data length in bytes = 4");
                }

                if (!p_b_skipUDPMarshalling)
                {
                    TestUDP(p_b_enhancedUDP, true, i_marshallingDataLengthInBytes);
                }

                if (!p_b_skipTCPMarshalling)
                {
                    TestTCP(p_b_enhancedTCP, true, i_marshallingDataLengthInBytes);
                }

                for (int j = 0; j < 6; j++)
                {
                    if (j == 0)
                    {
                        ForestNET.Lib.Global.ILogWarning("INFO: no security");

                        if (!b_enhancedMarshalling)
                        {
                            j = 4; /* jump directly to asymmetric security on next iteration */
                        }
                    }
                    else if (j == 1)
                    {
                        b_symmetricSecurity128 = true;
                        b_symmetricSecurity256 = false;
                        b_asymmetricSecurity = false;
                        b_highSecurity = false;
                        ForestNET.Lib.Global.ILogWarning("INFO: 128-bit security");
                    }
                    else if (j == 2)
                    {
                        b_symmetricSecurity128 = true;
                        b_symmetricSecurity256 = false;
                        b_asymmetricSecurity = false;
                        b_highSecurity = true;
                        ForestNET.Lib.Global.ILogWarning("INFO: 128-bit 'high' security");
                    }
                    else if (j == 3)
                    {
                        b_symmetricSecurity128 = false;
                        b_symmetricSecurity256 = true;
                        b_asymmetricSecurity = false;
                        b_highSecurity = false;
                        ForestNET.Lib.Global.ILogWarning("INFO: 256-bit security");
                    }
                    else if (j == 4)
                    {
                        b_symmetricSecurity128 = false;
                        b_symmetricSecurity256 = true;
                        b_asymmetricSecurity = false;
                        b_highSecurity = true;
                        ForestNET.Lib.Global.ILogWarning("INFO: 256-bit 'high' security");
                    }
                    else if (j == 5)
                    {
                        b_symmetricSecurity128 = false;
                        b_symmetricSecurity256 = false;
                        b_asymmetricSecurity = true;
                        b_highSecurity = false;
                        ForestNET.Lib.Global.ILogWarning("INFO: asymmetric security");
                    }

                    bool b_marshallingUseProperties = true;
                    string? s_marshallingOverrideMessageType = null;
                    bool b_marshallingSystemUsesLittleEndian = false;

                    /* ****************** */
                    /* Marshalling Object */
                    /* ****************** */

                    /* skip UDP with asymmetric security */
                    if (!b_asymmetricSecurity)
                    {
                        e_asymmetricMode = AsymmetricModes.None;

                        ForestNET.Lib.Global.ILogWarning("INFO: testCommunicationMarshallingObject UDP - small object");
                        TestCommunicationMarshallingObject(false, false, true, "127.0.0.1", 8080, "127.0.0.1", 8080, null, 0, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, e_asymmetricMode, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);

                        ForestNET.Lib.Global.ILogWarning("INFO: testCommunicationMarshallingObject UDP - normal object");
                        TestCommunicationMarshallingObject(false, false, false, "127.0.0.1", 8080, "127.0.0.1", 8080, null, 0, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, e_asymmetricMode, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);

                        ForestNET.Lib.Global.ILogWarning("INFO: testCommunicationMarshallingObject UDP with ACK - small object");
                        TestCommunicationMarshallingObject(false, true, true, "127.0.0.1", 8080, "127.0.0.1", 8080, "127.0.0.1", 42333, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, e_asymmetricMode, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);

                        ForestNET.Lib.Global.ILogWarning("INFO: testCommunicationMarshallingObject UDP with ACK - normal object");
                        TestCommunicationMarshallingObject(false, true, false, "127.0.0.1", 8080, "127.0.0.1", 8080, "127.0.0.1", 42333, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, e_asymmetricMode, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);
                    }

                    e_asymmetricMode = AsymmetricModes.ThumbprintInStore;

                    ForestNET.Lib.Global.ILogWarning("INFO: testCommunicationMarshallingObject TCP - small object");
                    TestCommunicationMarshallingObject(true, false, true, "127.0.0.1", 8080, "127.0.0.1", 8080, null, 0, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, e_asymmetricMode, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);

                    e_asymmetricMode = AsymmetricModes.FileNoPassword;

                    ForestNET.Lib.Global.ILogWarning("INFO: testCommunicationMarshallingObject TCP");
                    TestCommunicationMarshallingObject(true, false, false, "127.0.0.1", 8080, "127.0.0.1", 8080, null, 0, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, e_asymmetricMode, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);

                    e_asymmetricMode = AsymmetricModes.None;

                    if (!b_skipUniAndBiDirectional)
                    {
                        /* **************************************** */
                        /* Marshalling Whole Object Uni Directional */
                        /* **************************************** */

                        /* shared memory objects only have private fields, so we must use property methods to access these fields */
                        b_marshallingUseProperties = true;

                        bool b_smallObject = true;
                        bool b_massChangeAtEnd = false;

                        /* skip UDP with asymmetric security */
                        if (!b_asymmetricSecurity)
                        {
                            ForestNET.Lib.Global.ILogWarning("INFO: testCommunicationMarshallingSharedMemoryUniDirectional UDP - small object");
                            TestCommunicationMarshallingSharedMemoryUniDirectional(false, b_smallObject, b_massChangeAtEnd, false, "127.0.0.1", 8080, "127.0.0.1", 8080, null, 0, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, e_asymmetricMode, true, true, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);

                            b_massChangeAtEnd = true;

                            ForestNET.Lib.Global.ILogWarning("INFO: testCommunicationMarshallingSharedMemoryUniDirectional UDP - small object - mass change at end");
                            TestCommunicationMarshallingSharedMemoryUniDirectional(false, b_smallObject, b_massChangeAtEnd, false, "127.0.0.1", 8080, "127.0.0.1", 8080, null, 0, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, e_asymmetricMode, true, true, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);

                            b_smallObject = false;
                            b_massChangeAtEnd = false;

                            ForestNET.Lib.Global.ILogWarning("INFO: testCommunicationMarshallingSharedMemoryUniDirectional UDP");
                            TestCommunicationMarshallingSharedMemoryUniDirectional(false, b_smallObject, b_massChangeAtEnd, false, "127.0.0.1", 8080, "127.0.0.1", 8080, null, 0, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, e_asymmetricMode, true, true, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);

                            b_massChangeAtEnd = true;

                            ForestNET.Lib.Global.ILogWarning("INFO: testCommunicationMarshallingSharedMemoryUniDirectional UDP - mass change at end");
                            TestCommunicationMarshallingSharedMemoryUniDirectional(false, b_smallObject, b_massChangeAtEnd, false, "127.0.0.1", 8080, "127.0.0.1", 8080, null, 0, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, e_asymmetricMode, true, true, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);

                            b_smallObject = true;
                            b_massChangeAtEnd = false;

                            ForestNET.Lib.Global.ILogWarning("INFO: testCommunicationMarshallingSharedMemoryUniDirectional UDP with ACK - small object");
                            TestCommunicationMarshallingSharedMemoryUniDirectional(false, b_smallObject, b_massChangeAtEnd, true, "127.0.0.1", 8080, "127.0.0.1", 8080, "127.0.0.1", 42333, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, e_asymmetricMode, true, true, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);

                            b_massChangeAtEnd = true;

                            ForestNET.Lib.Global.ILogWarning("INFO: testCommunicationMarshallingSharedMemoryUniDirectional UDP with ACK - small object - mass change at end");
                            TestCommunicationMarshallingSharedMemoryUniDirectional(false, b_smallObject, b_massChangeAtEnd, true, "127.0.0.1", 8080, "127.0.0.1", 8080, "127.0.0.1", 42333, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, e_asymmetricMode, true, true, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);

                            b_smallObject = true;
                            b_massChangeAtEnd = false;
                        }

                        e_asymmetricMode = AsymmetricModes.FileWithPassword;

                        ForestNET.Lib.Global.ILogWarning("INFO: testCommunicationMarshallingSharedMemoryUniDirectional TCP - small object");
                        TestCommunicationMarshallingSharedMemoryUniDirectional(true, b_smallObject, b_massChangeAtEnd, false, "127.0.0.1", 8080, "127.0.0.1", 8080, null, 0, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, e_asymmetricMode, true, true, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);

                        b_massChangeAtEnd = true;
                        e_asymmetricMode = AsymmetricModes.ThumbprintInStoreAndClientRemoteCertificateName;

                        ForestNET.Lib.Global.ILogWarning("INFO: testCommunicationMarshallingSharedMemoryUniDirectional TCP - small object - mass change at end");
                        TestCommunicationMarshallingSharedMemoryUniDirectional(true, b_smallObject, b_massChangeAtEnd, false, "127.0.0.1", 8080, "127.0.0.1", 8080, null, 0, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, e_asymmetricMode, true, true, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);

                        b_smallObject = false;
                        b_massChangeAtEnd = false;
                        e_asymmetricMode = AsymmetricModes.FileNoPassword;

                        ForestNET.Lib.Global.ILogWarning("INFO: testCommunicationMarshallingSharedMemoryUniDirectional TCP");
                        TestCommunicationMarshallingSharedMemoryUniDirectional(true, b_smallObject, b_massChangeAtEnd, false, "127.0.0.1", 8080, "127.0.0.1", 8080, null, 0, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, e_asymmetricMode, true, true, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);

                        b_massChangeAtEnd = true;
                        e_asymmetricMode = AsymmetricModes.ThumbprintInStore;

                        ForestNET.Lib.Global.ILogWarning("INFO: testCommunicationMarshallingSharedMemoryUniDirectional TCP - mass change at end");
                        TestCommunicationMarshallingSharedMemoryUniDirectional(true, b_smallObject, b_massChangeAtEnd, false, "127.0.0.1", 8080, "127.0.0.1", 8080, null, 0, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, e_asymmetricMode, true, true, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);

                        e_asymmetricMode = AsymmetricModes.None;

                        /* *************************************** */
                        /* Marshalling Whole Object Bi Directional */
                        /* *************************************** */

                        b_smallObject = true;

                        /* skip UDP with asymmetric security */
                        if (!b_asymmetricSecurity)
                        {
                            ForestNET.Lib.Global.ILogWarning("INFO: testCommunicationMarshallingSharedMemoryBiDirectional UDP - small object");
                            TestCommunicationMarshallingSharedMemoryBiDirectional(false, b_smallObject, false, "127.0.0.1", 8080, "127.0.0.1", 8080, "127.0.0.1", 8081, "127.0.0.1", 8081, null, 0, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, e_asymmetricMode, true, true, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);

                            b_smallObject = false;

                            ForestNET.Lib.Global.ILogWarning("INFO: testCommunicationMarshallingSharedMemoryBiDirectional UDP");
                            TestCommunicationMarshallingSharedMemoryBiDirectional(false, b_smallObject, false, "127.0.0.1", 8080, "127.0.0.1", 8080, "127.0.0.1", 8081, "127.0.0.1", 8081, null, 0, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, e_asymmetricMode, true, true, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);

                            b_smallObject = true;

                            ForestNET.Lib.Global.ILogWarning("INFO: testCommunicationMarshallingSharedMemoryBiDirectional UDP with ACK - small object");
                            TestCommunicationMarshallingSharedMemoryBiDirectional(false, b_smallObject, true, "127.0.0.1", 8080, "127.0.0.1", 8080, "127.0.0.1", 8081, "127.0.0.1", 8081, "127.0.0.1", 42333, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, e_asymmetricMode, true, true, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);

                            b_smallObject = true;
                        }

                        e_asymmetricMode = AsymmetricModes.FileWithPassword;

                        ForestNET.Lib.Global.ILogWarning("INFO: testCommunicationMarshallingSharedMemoryBiDirectional TCP - small object");
                        TestCommunicationMarshallingSharedMemoryBiDirectional(true, b_smallObject, false, "127.0.0.1", 8080, "127.0.0.1", 8080, "127.0.0.1", 8081, "127.0.0.1", 8081, null, 0, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, e_asymmetricMode, true, true, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);

                        b_smallObject = false;
                        e_asymmetricMode = AsymmetricModes.ThumbprintInStore;

                        ForestNET.Lib.Global.ILogWarning("INFO: testCommunicationMarshallingSharedMemoryBiDirectional TCP");
                        TestCommunicationMarshallingSharedMemoryBiDirectional(true, b_smallObject, false, "127.0.0.1", 8080, "127.0.0.1", 8080, "127.0.0.1", 8081, "127.0.0.1", 8081, null, 0, b_symmetricSecurity128, b_symmetricSecurity256, b_asymmetricSecurity, b_highSecurity, e_asymmetricMode, true, true, i_marshallingDataLengthInBytes, b_marshallingUseProperties, s_marshallingOverrideMessageType, b_marshallingSystemUsesLittleEndian);
                    }
                }
            }
        }

        private static ForestNET.Lib.Net.Sock.Com.Config GetCommunicationConfig(ForestNET.Lib.Net.Sock.Com.Type p_e_comType, ForestNET.Lib.Net.Sock.Com.Cardinality p_e_comCardinality, string p_s_host, int p_i_port, string? p_s_localHost, int p_i_localPort, bool p_b_symmetricSecurity128, bool p_b_symmetricSecurity256, bool p_b_asymmetricSecurity, bool p_b_highSecurity, AsymmetricModes p_e_asymmetricMode, bool p_b_useMarshalling, bool p_b_useMarshallingWholeObject, int p_i_marshallingDataLengthInBytes, bool p_b_marshallingUseProperties, string? p_s_marshallingOverrideMessageType, bool p_b_marshallingSystemUsesLittleEndian)
        {
            string s_resourcesDirectory = s_currentDirectory + "Resources" + ForestNET.Lib.IO.File.DIR + "com" + ForestNET.Lib.IO.File.DIR;

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
                    o_communicationConfig.CertificateThumbprint = ThumbPrintServerCertificate;
                    o_communicationConfig.CertificateStoreName = System.Security.Cryptography.X509Certificates.StoreName.Root;
                    o_communicationConfig.CertificateStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser;
                }
                else if (p_e_asymmetricMode == AsymmetricModes.ThumbprintInStoreAndClientRemoteCertificateName)
                {
                    o_communicationConfig.CertificateThumbprint = ThumbPrintServerCertificate;
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

        private static void TestCommunication(bool p_b_falseUDPtrueTCP, bool p_b_udpWithAck, string p_s_serverHost, int p_i_serverPort, string p_s_clientHost, int p_i_clientPort, string? p_s_clientLocalHost, int p_i_clientLocalPort, bool p_b_symmetricSecurity128, bool p_b_symmetricSecurity256, bool p_b_asymmetricSecurity, bool p_b_highSecurity, AsymmetricModes p_e_asymmetricMode, bool p_b_useMarshalling, bool p_b_useMarshallingWholeObject, int p_i_marshallingDataLengthInBytes, bool p_b_marshallingUseProperties, string? p_s_marshallingOverrideMessageType, bool p_b_marshallingSystemUsesLittleEndian)
        {
            List<string> a_serverLog = [];
            List<string> a_clientLog = [];

            int i_comDequeueWaitLoopTimeout = 5000;
            int i_iterations = 10;

            /* SERVER */

            Task o_server = Task.Run(() =>
            {
                try
                {
                    ForestNET.Lib.Net.Sock.Com.Type e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE;

                    if ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck))
                    {
                        e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE_WITH_ACK;
                    }

                    if (p_b_falseUDPtrueTCP)
                    {
                        e_type = ForestNET.Lib.Net.Sock.Com.Type.TCP_RECEIVE;
                    }

                    ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(e_type, ForestNET.Lib.Net.Sock.Com.Cardinality.Equal, p_s_serverHost, p_i_serverPort, null, 0, p_b_symmetricSecurity128, p_b_symmetricSecurity256, p_b_asymmetricSecurity, p_b_highSecurity, p_e_asymmetricMode, p_b_useMarshalling, p_b_useMarshallingWholeObject, p_i_marshallingDataLengthInBytes, p_b_marshallingUseProperties, p_s_marshallingOverrideMessageType, p_b_marshallingSystemUsesLittleEndian);
                    ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                    o_communication.Start();

                    for (int i = 0; i < i_iterations; i++)
                    {
                        if (p_b_falseUDPtrueTCP)
                        {
                            string? s_message = (string?)o_communication.DequeueWithWaitLoop(i_comDequeueWaitLoopTimeout);

                            if (s_message != null)
                            {
                                ForestNET.Lib.Global.ILog("#" + (i + 1) + " message(" + s_message.Length + ") received: '" + s_message + "'");
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
                                ForestNET.Lib.Global.ILog("#" + (i + 1) + " message received: '" + o_date?.ToString("dd.MM.yyyy HH:mm:ss") + "'");
                                a_serverLog.Add("#" + (i + 1) + " message received");
                            }
                            else
                            {
                                ForestNET.Lib.Global.ILogWarning("could not receive any data");
                            }
                        }
                    }

                    o_communication?.Stop();
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.LogException(o_exc);
                    Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
                }
            });

            /* CLIENT */

            Task o_client = Task.Run(() =>
            {
                try
                {
                    ForestNET.Lib.Net.Sock.Com.Type e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND;

                    if ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck))
                    {
                        e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND_WITH_ACK;
                    }

                    if (p_b_falseUDPtrueTCP)
                    {
                        e_type = ForestNET.Lib.Net.Sock.Com.Type.TCP_SEND;
                    }

                    ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(e_type, ForestNET.Lib.Net.Sock.Com.Cardinality.Equal, p_s_clientHost, p_i_clientPort, p_s_clientLocalHost, p_i_clientLocalPort, p_b_symmetricSecurity128, p_b_symmetricSecurity256, p_b_asymmetricSecurity, p_b_highSecurity, p_e_asymmetricMode, p_b_useMarshalling, p_b_useMarshallingWholeObject, p_i_marshallingDataLengthInBytes, p_b_marshallingUseProperties, p_s_marshallingOverrideMessageType, p_b_marshallingSystemUsesLittleEndian);
                    ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                    o_communication.Start();

                    for (int i = 0; i < i_iterations; i++)
                    {
                        if (p_b_falseUDPtrueTCP)
                        {
                            string s_foo = (i + 1) + ": Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.   Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi. Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat.   Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat.";

                            if (p_i_marshallingDataLengthInBytes == 1)
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

                        ForestNET.Lib.Global.ILog("message enqueued");
                        a_clientLog.Add("message enqueued");

                        if (i == 4)
                        { /* additional delay for 25 milliseconds times sleep multiplier constant, after the 5th time enqueue has been executed */
                            Thread.Sleep(25 * i_sleepMultiplier);
                        }

                        Thread.Sleep(25 * i_sleepMultiplier + (((!p_b_falseUDPtrueTCP) && (p_b_highSecurity)) ? 150 : 0) + (((p_b_falseUDPtrueTCP) && ((p_b_highSecurity) || (p_b_asymmetricSecurity))) ? 25 : 0));
                    }

                    o_communication?.Stop();
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.LogException(o_exc);
                    Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
                }
            });

            /* WAIT SERVER + CLIENT */

            Task.WaitAll(o_server, o_client);

            /* CHECK LOG ENTRIES */

            Assert.That(a_serverLog, Has.Count.EqualTo(10), "server log has not '10' entries, but '" + a_serverLog.Count + "'");
            Assert.That(a_clientLog, Has.Count.EqualTo(10), "client log has not '10' entries, but '" + a_clientLog.Count + "'");

            if (p_b_falseUDPtrueTCP)
            {
                int i_expectedLength = 1438;

                if (p_i_marshallingDataLengthInBytes == 1)
                {
                    i_expectedLength = 215;
                }

                for (int i = 0; i < 10; i++)
                {
                    Assert.That(a_serverLog[i], Does.StartWith("#" + (i + 1) + " message(" + ((i == 9) ? (i_expectedLength + 1) : i_expectedLength) + ") received"), "server log entry does not start with '#" + (i + 1) + " message(" + ((i == 9) ? (i_expectedLength + 1) : i_expectedLength) + ") received:', but with '" + a_serverLog[i] + "'");
                    Assert.That(a_clientLog[i], Is.EqualTo("message enqueued"), "client log entry does not match with 'message enqueued', but is '" + a_clientLog[i] + "'");
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    Assert.That(a_serverLog[i], Does.StartWith("#" + (i + 1) + " message received"), "server log entry does not start with '#" + (i + 1) + " message received:', but with '" + a_serverLog[i] + "'");
                    Assert.That(a_clientLog[i], Is.EqualTo("message enqueued"), "client log entry does not match with 'message enqueued', but is '" + a_clientLog[i] + "'");
                }
            }
        }

        private static void TestCommunicationUDPMulticast(bool p_b_falseIPv4trueIPv6, string p_s_serverHost, int p_i_serverPort, string p_s_clientHost, int p_i_clientPort, string? p_s_clientLocalHost, int p_i_clientLocalPort, bool p_b_symmetricSecurity128, bool p_b_symmetricSecurity256, bool p_b_highSecurity, bool p_b_useMarshalling, int p_i_marshallingDataLengthInBytes, bool p_b_marshallingUseProperties, string? p_s_marshallingOverrideMessageType, bool p_b_marshallingSystemUsesLittleEndian)
        {
            List<string> a_serverLog = [];
            List<string> a_clientLog = [];

            int i_comDequeueWaitLoopTimeout = 5000;
            int i_iterations = 10;

            /* SERVER */

            Task o_server = Task.Run(() =>
            {
                try
                {
                    ForestNET.Lib.Net.Sock.Com.Type e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND;

                    ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(e_type, ForestNET.Lib.Net.Sock.Com.Cardinality.Equal, p_s_clientHost, p_i_clientPort, p_s_clientLocalHost, p_i_clientLocalPort, p_b_symmetricSecurity128, p_b_symmetricSecurity256, false, p_b_highSecurity, AsymmetricModes.None, p_b_useMarshalling, false, p_i_marshallingDataLengthInBytes, p_b_marshallingUseProperties, p_s_marshallingOverrideMessageType, p_b_marshallingSystemUsesLittleEndian);
                    o_communicationConfig.UDPIsMulticastSocket = true;
                    o_communicationConfig.UDPMulticastTTL = 1;
                    ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                    o_communication.Start();

                    string s_ip = "COULD_NOT_GET_DEFAULT_IP";
                    List<KeyValuePair<string, string>> a_ips = [];

                    if (p_b_falseIPv4trueIPv6)
                    {
                        a_ips = ForestNET.Lib.Helper.GetNetworkInterfacesIpv6();
                    }
                    else
                    {
                        a_ips = ForestNET.Lib.Helper.GetNetworkInterfacesIpv4();
                    }

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

                        ForestNET.Lib.Global.ILog("message enqueued");
                        a_serverLog.Add("message enqueued");

                        if (i == 4)
                        { /* additional delay for 25 milliseconds times sleep multiplier constant, after the 5th time enqueue has been executed */
                            Thread.Sleep(25 * i_sleepMultiplier);
                        }

                        Thread.Sleep(25 * i_sleepMultiplier + ((p_b_highSecurity) ? 150 : 0));
                    }

                    o_communication?.Stop();
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.LogException(o_exc);
                    Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
                }
            });

            /* CLIENT */

            Task o_client = Task.Run(() =>
            {
                try
                {
                    ForestNET.Lib.Net.Sock.Com.Type e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE;
                    ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(e_type, ForestNET.Lib.Net.Sock.Com.Cardinality.Equal, p_s_serverHost, p_i_serverPort, null, 0, p_b_symmetricSecurity128, p_b_symmetricSecurity256, false, p_b_highSecurity, AsymmetricModes.None, p_b_useMarshalling, false, p_i_marshallingDataLengthInBytes, p_b_marshallingUseProperties, p_s_marshallingOverrideMessageType, p_b_marshallingSystemUsesLittleEndian);
                    o_communicationConfig.UDPIsMulticastSocket = true;
                    ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                    o_communication.Start();

                    for (int i = 0; i < i_iterations; i++)
                    {
                        string? o_serverIp = (string?)o_communication.DequeueWithWaitLoop(i_comDequeueWaitLoopTimeout);

                        if (o_serverIp != null)
                        {
                            ForestNET.Lib.Global.ILog("#" + (i + 1) + " message received: '" + o_serverIp + "'");
                            a_clientLog.Add("#" + (i + 1) + " message received");
                        }
                        else
                        {
                            ForestNET.Lib.Global.ILogWarning("could not receive any data");
                        }
                    }

                    o_communication?.Stop();
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.LogException(o_exc);
                    Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
                }
            });

            /* WAIT SERVER + CLIENT */

            Task.WaitAll(o_server, o_client);

            /* CHECK LOG ENTRIES */

            Assert.That(a_serverLog, Has.Count.EqualTo(10), "server log has not '10' entries, but '" + a_serverLog.Count + "'");
            Assert.That(a_clientLog, Has.Count.EqualTo(10), "client log has not '10' entries, but '" + a_clientLog.Count + "'");

            for (int i = 0; i < 10; i++)
            {
                Assert.That(a_clientLog[i], Does.StartWith("#" + (i + 1) + " message received"), "server log entry does not start with '#" + (i + 1) + " message received:', but with '" + a_clientLog[i] + "'");
                Assert.That(a_serverLog[i], Is.EqualTo("message enqueued"), "client log entry does not match with 'message enqueued', but is '" + a_serverLog[i] + "'");
            }
        }

        private static void TestCommunicationTCPBidirectional(string p_s_serverHost, int p_i_serverPort, string p_s_clientHost, int p_i_clientPort, bool p_b_symmetricSecurity128, bool p_b_symmetricSecurity256, bool p_b_asymmetricSecurity, bool p_b_highSecurity, bool p_b_useMarshalling, bool p_b_useMarshallingWholeObject, int p_i_marshallingDataLengthInBytes, bool p_b_marshallingUseProperties, string? p_s_marshallingOverrideMessageType, bool p_b_marshallingSystemUsesLittleEndian)
        {
            List<string> a_serverLog = [];
            List<string> a_clientLog = [];

            int i_comDequeueWaitLoopTimeout = 2500;
            int i_iterations = 10;

            /* SERVER */

            Task o_server = Task.Run(() =>
            {
                try
                {
                    ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(ForestNET.Lib.Net.Sock.Com.Type.TCP_RECEIVE, ForestNET.Lib.Net.Sock.Com.Cardinality.EqualBidirectional, p_s_serverHost, p_i_serverPort, null, 0, p_b_symmetricSecurity128, p_b_symmetricSecurity256, p_b_asymmetricSecurity, p_b_highSecurity, AsymmetricModes.None, p_b_useMarshalling, p_b_useMarshallingWholeObject, p_i_marshallingDataLengthInBytes, p_b_marshallingUseProperties, p_s_marshallingOverrideMessageType, p_b_marshallingSystemUsesLittleEndian);
                    o_communicationConfig.ReceiverTimeoutMilliseconds = 100;
                    o_communicationConfig.SenderTimeoutMilliseconds = 100;
                    ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                    o_communication.Start();

                    for (int i = 0; i < i_iterations; i++)
                    {
                        string? s_message = (string?)o_communication.DequeueWithWaitLoop(i_comDequeueWaitLoopTimeout);

                        if (s_message != null)
                        {
                            ForestNET.Lib.Global.ILog("server: #" + (i + 1) + " message(" + s_message.Length + ") received: '" + s_message + "'");
                            a_serverLog.Add("server: #" + (i + 1) + " message(" + s_message.Length + ") received");
                        }
                        else
                        {
                            ForestNET.Lib.Global.ILogWarning("server: could not receive any data");
                        }

                        string s_foo = "server: " + (i + 1) + ": Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.";

                        while (!o_communication.Enqueue(
                            s_foo
                        ))
                        {
                            ForestNET.Lib.Global.ILogWarning("server: could not enqueue message");
                        }

                        ForestNET.Lib.Global.ILog("server: message enqueued");
                        a_serverLog.Add("server: message enqueued");

                        if (i == 4)
                        { /* additional delay for 25 milliseconds times sleep multiplier constant, after the 5th time enqueue has been executed */
                            Thread.Sleep(25 * i_sleepMultiplier);

                            s_foo = "server: " + (i + 1) + ": Additional Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.";

                            while (!o_communication.Enqueue(
                                s_foo
                            ))
                            {
                                ForestNET.Lib.Global.ILogWarning("server: could not enqueue message");
                            }

                            ForestNET.Lib.Global.ILog("server: additional message enqueued");
                            a_serverLog.Add("server: additional message enqueued");
                        }

                        Thread.Sleep(25 * i_sleepMultiplier + ((p_b_highSecurity) ? 25 : 0));
                    }

                    Thread.Sleep(250);

                    o_communication?.Stop();
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.LogException(o_exc);
                    Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
                }
            });

            /* CLIENT */

            Task o_client = Task.Run(() =>
            {
                try
                {
                    ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(ForestNET.Lib.Net.Sock.Com.Type.TCP_SEND, ForestNET.Lib.Net.Sock.Com.Cardinality.EqualBidirectional, p_s_clientHost, p_i_clientPort, null, 0, p_b_symmetricSecurity128, p_b_symmetricSecurity256, p_b_asymmetricSecurity, p_b_highSecurity, AsymmetricModes.None, p_b_useMarshalling, p_b_useMarshallingWholeObject, p_i_marshallingDataLengthInBytes, p_b_marshallingUseProperties, p_s_marshallingOverrideMessageType, p_b_marshallingSystemUsesLittleEndian);
                    o_communicationConfig.ReceiverTimeoutMilliseconds = 100;
                    o_communicationConfig.SenderTimeoutMilliseconds = 100;
                    ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                    o_communication.Start();

                    for (int i = 0; i < i_iterations; i++)
                    {
                        string s_foo = "client: " + (i + 1) + ": Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.";

                        while (!o_communication.Enqueue(
                            s_foo
                        ))
                        {
                            ForestNET.Lib.Global.ILogWarning("client: could not enqueue message");
                        }

                        ForestNET.Lib.Global.ILog("client: message enqueued");
                        a_clientLog.Add("client: message enqueued");

                        string? s_message = (string?)o_communication.DequeueWithWaitLoop(i_comDequeueWaitLoopTimeout);

                        if (s_message != null)
                        {
                            ForestNET.Lib.Global.ILog("client: #" + (i + 1) + " message(" + s_message.Length + ") received: '" + s_message + "'");
                            a_clientLog.Add("client: #" + (i + 1) + " message(" + s_message.Length + ") received");
                        }
                        else
                        {
                            ForestNET.Lib.Global.ILogWarning("client: could not receive any data");
                        }

                        if (i == 4)
                        { /* additional delay for 25 milliseconds times sleep multiplier constant, after the 5th time enqueue has been executed */
                            Thread.Sleep(25 * i_sleepMultiplier);
                        }

                        if (i == 9)
                        { /* receive additional message */
                            s_message = (string?)o_communication.DequeueWithWaitLoop(i_comDequeueWaitLoopTimeout);

                            if (s_message != null)
                            {
                                ForestNET.Lib.Global.ILog("client: #" + (i + 2) + " message(" + s_message.Length + ") received: '" + s_message + "'");
                                a_clientLog.Add("client: #" + (i + 2) + " message(" + s_message.Length + ") received");
                            }
                            else
                            {
                                ForestNET.Lib.Global.ILogWarning("client: could not receive any data");
                            }
                        }

                        Thread.Sleep(25 * i_sleepMultiplier + ((p_b_highSecurity) ? 25 : 0));
                    }

                    Thread.Sleep(250);

                    o_communication?.Stop();
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.LogException(o_exc);
                    Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
                }
            });

            /* WAIT SERVER + CLIENT */

            Task.WaitAll(o_server, o_client);

            /* CHECK LOG ENTRIES */

            Assert.That(a_serverLog, Has.Count.EqualTo(21), "server log has not '21' entries, but '" + a_serverLog.Count + "'");
            Assert.That(a_clientLog, Has.Count.EqualTo(21), "client log has not '21' entries, but '" + a_clientLog.Count + "'");

            int i_foo = 166;
            int j = 0;

            for (int i = 0; i < 21; i++)
            {
                if (i == 2)
                {
                    j--;
                }

                if ((i - j) == 10)
                {
                    i_foo++;
                }

                Assert.That(a_serverLog[i], Does.StartWith("server: #" + ((i != 0) ? (i - j) : (i + 1)) + " message(" + i_foo + ") received"), "server log entry does not start with 'server: #" + ((i != 0) ? (i - j) : (i + 1)) + " message(" + i_foo + ") received:', but with '" + a_serverLog[i] + "'");
                Assert.That(a_serverLog[i + 1], Is.EqualTo("server: message enqueued"), "server log entry does not match with 'server: message enqueued', but is '" + a_serverLog[i + 1] + "'");
                i++;

                if (i == 9)
                {
                    Assert.That(a_serverLog[i + 1], Is.EqualTo("server: additional message enqueued"), "server log entry does not match with 'server: additional message enqueued', but is '" + a_serverLog[i + 1] + "'");
                    i++;
                    j++;
                }

                j++;
            }

            i_foo = 166;
            j = 0;

            for (int i = 0; i < 20; i++)
            {
                if (i == 2)
                {
                    j--;
                }

                Assert.That(a_clientLog[i], Is.EqualTo("client: message enqueued"), "client log entry does not match with 'client: message enqueued', but is '" + a_clientLog[i] + "'");
                Assert.That(a_clientLog[i + 1], Does.StartWith("client: #" + ((i != 0) ? (i - j) : (i + 1)) + " message(" + (((i + 1) == 11) ? (i_foo + 11) : i_foo) + ") received"), "client log entry does not start with 'client: #" + ((i != 0) ? (i - j) : (i + 1)) + " message(" + (((i + 1) == 11) ? (i_foo + 11) : i_foo) + ") received:', but with '" + a_clientLog[i + 1] + "'");
                i++;

                if (i == 19)
                {
                    Assert.That(a_clientLog[i + 1], Does.StartWith("client: #" + ((i != 0) ? (i - j) : (i + 1)) + " message(167) received"), "client log entry does not start with 'client: #" + ((i != 0) ? (i - j) : (i + 1)) + " message(167) received:', but with '" + a_clientLog[i + 1] + "'");
                    i++;
                }

                j++;
            }
        }

        private static void TestCommunicationTCPHandshakeTask(string p_s_serverHost, int p_i_serverPort, string p_s_clientHost, int p_i_clientPort, bool p_b_symmetricSecurity128, bool p_b_symmetricSecurity256, bool p_b_asymmetricSecurity, bool p_b_highSecurity, bool p_b_useMarshalling, bool p_b_useMarshallingWholeObject, int p_i_marshallingDataLengthInBytes, bool p_b_marshallingUseProperties, string? p_s_marshallingOverrideMessageType, bool p_b_marshallingSystemUsesLittleEndian)
        {
            List<string> a_serverLog = [];
            List<string> a_clientLog = [];

            /* SERVER */

            Task o_server = Task.Run(() =>
            {
                try
                {
                    ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(ForestNET.Lib.Net.Sock.Com.Type.TCP_RECEIVE, ForestNET.Lib.Net.Sock.Com.Cardinality.Equal, p_s_serverHost, p_i_serverPort, null, 0, p_b_symmetricSecurity128, p_b_symmetricSecurity256, p_b_asymmetricSecurity, p_b_highSecurity, AsymmetricModes.None, p_b_useMarshalling, p_b_useMarshallingWholeObject, p_i_marshallingDataLengthInBytes, p_b_marshallingUseProperties, p_s_marshallingOverrideMessageType, p_b_marshallingSystemUsesLittleEndian);

                    ForestNET.Lib.Net.Sock.Task.Task? o_socketTask = null;

                    if (p_b_asymmetricSecurity)
                    {
                        o_socketTask = new ForestNET.Lib.Net.Sock.Task.Recv.HandshakeReceive(ForestNET.Lib.Net.Sock.Type.TCP_TLS_SERVER);
                    }
                    else
                    {
                        o_socketTask = new ForestNET.Lib.Net.Sock.Task.Recv.HandshakeReceive(ForestNET.Lib.Net.Sock.Type.TCP_SERVER);
                    }

                    ((ForestNET.Lib.Net.Sock.Task.Recv.HandshakeReceive)o_socketTask).Endless = true;
                    ((ForestNET.Lib.Net.Sock.Task.Recv.HandshakeReceive)o_socketTask).TaskIntervalMilliseconds = 1000;
                    ((ForestNET.Lib.Net.Sock.Task.Recv.HandshakeReceive)o_socketTask).AdditionalExecutionDelegate = () =>
                    {
                        a_serverLog.Add("additonal execution from receiver");
                    };

                    o_communicationConfig.AddSocketTask(o_socketTask);

                    ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                    o_communication.Start();

                    Thread.Sleep(5000);

                    o_communication?.Stop();
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.LogException(o_exc);
                    Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
                }
            });

            /* CLIENT */

            Task o_client = Task.Run(() =>
            {
                try
                {
                    ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(ForestNET.Lib.Net.Sock.Com.Type.TCP_SEND, ForestNET.Lib.Net.Sock.Com.Cardinality.Equal, p_s_clientHost, p_i_clientPort, null, 0, p_b_symmetricSecurity128, p_b_symmetricSecurity256, p_b_asymmetricSecurity, p_b_highSecurity, AsymmetricModes.None, p_b_useMarshalling, p_b_useMarshallingWholeObject, p_i_marshallingDataLengthInBytes, p_b_marshallingUseProperties, p_s_marshallingOverrideMessageType, p_b_marshallingSystemUsesLittleEndian);

                    ForestNET.Lib.Net.Sock.Task.Task? o_socketTask = null;

                    if (p_b_asymmetricSecurity)
                    {
                        o_socketTask = new ForestNET.Lib.Net.Sock.Task.Send.HandshakeSend(ForestNET.Lib.Net.Sock.Type.TCP_TLS_CLIENT);
                    }
                    else
                    {
                        o_socketTask = new ForestNET.Lib.Net.Sock.Task.Send.HandshakeSend(ForestNET.Lib.Net.Sock.Type.TCP_CLIENT);
                    }

                    ((ForestNET.Lib.Net.Sock.Task.Send.HandshakeSend)o_socketTask).Endless = true;
                    ((ForestNET.Lib.Net.Sock.Task.Send.HandshakeSend)o_socketTask).TaskIntervalMilliseconds = 1000;
                    ((ForestNET.Lib.Net.Sock.Task.Send.HandshakeSend)o_socketTask).AdditionalExecutionDelegate = () =>
                    {
                        a_clientLog.Add("additonal execution from sender");
                    };

                    o_communicationConfig.AddSocketTask(o_socketTask);

                    ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                    o_communication.Start();

                    Thread.Sleep(5000);

                    o_communication?.Stop();
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.LogException(o_exc);
                    Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
                }
            });

            /* WAIT SERVER + CLIENT */

            Task.WaitAll(o_server, o_client);

            Thread.Sleep(25 * i_sleepMultiplier);

            /* CHECK LOG ENTRIES */

            Assert.That(a_serverLog, Has.Count.EqualTo(5), "server log has not '5' entries, but '" + a_serverLog.Count + "'");
            Assert.That(a_clientLog, Has.Count.EqualTo(5), "client log has not '5' entries, but '" + a_clientLog.Count + "'");

            for (int i = 0; i < 5; i++)
            {
                Assert.That(a_serverLog[i], Is.EqualTo("additonal execution from receiver"), "server log entry does not match with 'additonal execution from receiver', but is '" + a_serverLog[i] + "'");
                Assert.That(a_clientLog[i], Is.EqualTo("additonal execution from sender"), "client log entry does not match with 'additonal execution from sender', but is '" + a_clientLog[i] + "'");
            }
        }

        private static void TestCommunicationObjectTransmissionTCP(string p_s_serverHost, int p_i_serverPort, string p_s_clientHost, int p_i_clientPort, bool p_b_symmetricSecurity128, bool p_b_symmetricSecurity256, bool p_b_asymmetricSecurity, bool p_b_highSecurity, AsymmetricModes p_e_asymmetricMode, bool p_b_useMarshalling, bool p_b_useMarshallingWholeObject, int p_i_marshallingDataLengthInBytes, bool p_b_marshallingUseProperties, string? p_s_marshallingOverrideMessageType, bool p_b_marshallingSystemUsesLittleEndian)
        {
            List<string> a_serverLog = [];
            List<string> a_clientLog = [];

            int i_comDequeueWaitLoopTimeout = 5000;
            int i_iterations = 10;

            /* SERVER */

            Task o_server = Task.Run(() =>
            {
                try
                {
                    ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(ForestNET.Lib.Net.Sock.Com.Type.TCP_RECEIVE, ForestNET.Lib.Net.Sock.Com.Cardinality.Equal, p_s_serverHost, p_i_serverPort, null, 0, p_b_symmetricSecurity128, p_b_symmetricSecurity256, p_b_asymmetricSecurity, p_b_highSecurity, p_e_asymmetricMode, p_b_useMarshalling, p_b_useMarshallingWholeObject, p_i_marshallingDataLengthInBytes, p_b_marshallingUseProperties, p_s_marshallingOverrideMessageType, p_b_marshallingSystemUsesLittleEndian);
                    o_communicationConfig.ObjectTransmission = true;
                    ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                    o_communication.Start();

                    for (int i = 0; i < i_iterations; i++)
                    {
                        string? s_message = (string?)o_communication.DequeueWithWaitLoop(i_comDequeueWaitLoopTimeout);

                        if (s_message != null)
                        {
                            ForestNET.Lib.Global.ILog("#" + (i + 1) + " message(" + s_message.Length + ") received: '" + s_message + "'");
                            a_serverLog.Add("#" + (i + 1) + " message(" + s_message.Length + ") received");
                        }
                        else
                        {
                            ForestNET.Lib.Global.ILogWarning("could not receive any data");
                        }
                    }

                    o_communication?.Stop();
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.LogException(o_exc);
                    Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
                }
            });

            /* CLIENT */

            Task o_client = Task.Run(() =>
            {
                try
                {
                    ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(ForestNET.Lib.Net.Sock.Com.Type.TCP_SEND, ForestNET.Lib.Net.Sock.Com.Cardinality.Equal, p_s_clientHost, p_i_clientPort, null, 0, p_b_symmetricSecurity128, p_b_symmetricSecurity256, p_b_asymmetricSecurity, p_b_highSecurity, p_e_asymmetricMode, p_b_useMarshalling, p_b_useMarshallingWholeObject, p_i_marshallingDataLengthInBytes, p_b_marshallingUseProperties, p_s_marshallingOverrideMessageType, p_b_marshallingSystemUsesLittleEndian);
                    o_communicationConfig.ObjectTransmission = true;
                    ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                    o_communication.Start();

                    for (int i = 0; i < i_iterations; i++)
                    {
                        string s_foo = (i + 1) + ": Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.   Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi. Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat.   Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat.Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.   Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi. Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat.   Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat.Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.   Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi. Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat.   Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat.";

                        if (p_i_marshallingDataLengthInBytes == 1)
                        {
                            s_foo = (i + 1) + ": Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum, sed diam nonumy eirmod tempor invidun.";
                        }

                        while (!o_communication.Enqueue(
                            s_foo
                        ))
                        {
                            ForestNET.Lib.Global.ILogWarning("could not enqueue message");
                        }

                        ForestNET.Lib.Global.ILog("message enqueued");
                        a_clientLog.Add("message enqueued");

                        if (i == 4)
                        { /* additional delay for 25 milliseconds times sleep multiplier constant, after the 5th time enqueue has been executed */
                            Thread.Sleep(25 * i_sleepMultiplier);
                        }

                        Thread.Sleep(25 * i_sleepMultiplier + ((p_b_highSecurity && p_b_asymmetricSecurity) ? 25 : 0) + ((p_b_highSecurity && !p_b_asymmetricSecurity) ? 500 : 0));
                    }

                    o_communication?.Stop();
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.LogException(o_exc);
                    Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
                }
            });

            /* WAIT SERVER + CLIENT */

            Task.WaitAll(o_server, o_client);

            /* CHECK LOG ENTRIES */

            Assert.That(a_serverLog, Has.Count.EqualTo(10), "server log has not '10' entries, but '" + a_serverLog.Count + "'");
            Assert.That(a_clientLog, Has.Count.EqualTo(10), "client log has not '10' entries, but '" + a_clientLog.Count + "'");

            int i_expectedLength = 4308;

            if (p_i_marshallingDataLengthInBytes == 1)
            {
                i_expectedLength = 254;
            }

            for (int i = 0; i < 10; i++)
            {
                Assert.That(a_serverLog[i], Does.StartWith("#" + (i + 1) + " message(" + ((i == 9) ? (i_expectedLength + 1) : i_expectedLength) + ") received"), "server log entry does not start with '#" + (i + 1) + " message(" + ((i == 9) ? (i_expectedLength + 1) : i_expectedLength) + ") received:', but with '" + a_serverLog[i] + "'");
                Assert.That(a_clientLog[i], Is.EqualTo("message enqueued"), "client log entry does not match with 'message enqueued', but is '" + a_clientLog[i] + "'");
            }
        }

        private static void TestCommunicationWithAnswerOneTCP(string p_s_serverHost, int p_i_serverPort, string p_s_clientHost, int p_i_clientPort, bool p_b_symmetricSecurity128, bool p_b_symmetricSecurity256, bool p_b_asymmetricSecurity, bool p_b_highSecurity, AsymmetricModes p_e_asymmetricMode, bool p_b_useMarshalling, bool p_b_useMarshallingWholeObject, int p_i_marshallingDataLengthInBytes, bool p_b_marshallingUseProperties, string? p_s_marshallingOverrideMessageType, bool p_b_marshallingSystemUsesLittleEndian)
        {
            List<string> a_serverLog = [];
            List<string> a_clientLog = [];

            int i_comDequeueWaitLoopTimeout = 5000;
            int i_iterations = 10;

            /* SERVER */

            Task o_server = Task.Run(() =>
            {
                try
                {
                    ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(ForestNET.Lib.Net.Sock.Com.Type.TCP_RECEIVE_WITH_ANSWER, ForestNET.Lib.Net.Sock.Com.Cardinality.Equal, p_s_serverHost, p_i_serverPort, null, 0, p_b_symmetricSecurity128, p_b_symmetricSecurity256, p_b_asymmetricSecurity, p_b_highSecurity, p_e_asymmetricMode, p_b_useMarshalling, p_b_useMarshallingWholeObject, p_i_marshallingDataLengthInBytes, p_b_marshallingUseProperties, p_s_marshallingOverrideMessageType, p_b_marshallingSystemUsesLittleEndian);

                    ReceiveSocketTaskOne o_receiveSocketTask = new();
                    o_receiveSocketTask.AddObject("<answer>");
                    o_receiveSocketTask.AddObject("</answer>");
                    o_communicationConfig.AddReceiveSocketTask(o_receiveSocketTask);

                    ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                    o_communication.Start();

                    Thread.Sleep(25 * i_sleepMultiplier * (i_iterations / 2) + ((p_b_highSecurity) ? 14000 : 0));

                    o_communication?.Stop();
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.LogException(o_exc);
                    Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
                }
            });

            /* CLIENT */

            Task o_client = Task.Run(() =>
            {
                try
                {
                    ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(ForestNET.Lib.Net.Sock.Com.Type.TCP_SEND_WITH_ANSWER, ForestNET.Lib.Net.Sock.Com.Cardinality.Equal, p_s_clientHost, p_i_clientPort, null, 0, p_b_symmetricSecurity128, p_b_symmetricSecurity256, p_b_asymmetricSecurity, p_b_highSecurity, p_e_asymmetricMode, p_b_useMarshalling, p_b_useMarshallingWholeObject, p_i_marshallingDataLengthInBytes, p_b_marshallingUseProperties, p_s_marshallingOverrideMessageType, p_b_marshallingSystemUsesLittleEndian);
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

                        ForestNET.Lib.Global.ILog("message enqueued");
                        a_clientLog.Add("message enqueued");

                        Object? o_answer = o_communication.DequeueWithWaitLoop(i_comDequeueWaitLoopTimeout);

                        if (o_answer != null)
                        {
                            ForestNET.Lib.Global.ILog("#" + (i + 1) + " message(" + o_answer.ToString()?.Length + ") received: '" + o_answer.ToString() + "'");
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
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.LogException(o_exc);
                    Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
                }
            });

            /* WAIT SERVER + CLIENT */

            Task.WaitAll(o_server, o_client);

            /* CHECK LOG ENTRIES */

            Assert.That(a_serverLog, Has.Count.EqualTo(10), "server log has not '10' entries, but '" + a_serverLog.Count + "'");
            Assert.That(a_clientLog, Has.Count.EqualTo(10), "client log has not '10' entries, but '" + a_clientLog.Count + "'");

            List<int> a_expectedLength = [20, 20, 22, 21, 21, 20, 22, 22, 21, 20];

            for (int i = 0; i < 10; i++)
            {
                Assert.That(a_serverLog[i], Does.StartWith("#" + (i + 1) + " message(" + a_expectedLength[i] + ") received"), "server log entry does not start with '#" + (i + 1) + " message(" + a_expectedLength[i] + ") received:', but with '" + a_serverLog[i] + "'");
                Assert.That(a_clientLog[i], Is.EqualTo("message enqueued"), "client log entry does not match with 'message enqueued', but is '" + a_clientLog[i] + "'");
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
                    Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
                }

                await DoNoting();
            }
        }

        private static void TestCommunicationWithAnswerTwoTCP(string p_s_serverHost, int p_i_serverPort, string p_s_clientHost, int p_i_clientPort, bool p_b_symmetricSecurity128, bool p_b_symmetricSecurity256, bool p_b_asymmetricSecurity, bool p_b_highSecurity, AsymmetricModes p_e_asymmetricMode, bool p_b_useMarshalling, bool p_b_useMarshallingWholeObject, int p_i_marshallingDataLengthInBytes, bool p_b_marshallingUseProperties, string? p_s_marshallingOverrideMessageType, bool p_b_marshallingSystemUsesLittleEndian)
        {
            List<string> a_serverLog = [];
            List<string> a_clientLog = [];

            int i_comDequeueWaitLoopTimeout = 5000;
            int i_iterations = 14;

            /* SERVER */

            Task o_server = Task.Run(() =>
            {
                try
                {
                    ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(ForestNET.Lib.Net.Sock.Com.Type.TCP_RECEIVE_WITH_ANSWER, ForestNET.Lib.Net.Sock.Com.Cardinality.Equal, p_s_serverHost, p_i_serverPort, null, 0, p_b_symmetricSecurity128, p_b_symmetricSecurity256, p_b_asymmetricSecurity, p_b_highSecurity, p_e_asymmetricMode, p_b_useMarshalling, p_b_useMarshallingWholeObject, p_i_marshallingDataLengthInBytes, p_b_marshallingUseProperties, p_s_marshallingOverrideMessageType, p_b_marshallingSystemUsesLittleEndian);

                    ReceiveSocketTaskTwo o_receiveSocketTask = new();
                    o_receiveSocketTask.AddObject("<answer>");
                    o_receiveSocketTask.AddObject("</answer>");
                    o_communicationConfig.AddReceiveSocketTask(o_receiveSocketTask);

                    ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                    o_communication.Start();

                    Thread.Sleep(25 * i_sleepMultiplier * (i_iterations / 2) + ((p_b_highSecurity) ? 19000 : 0));

                    o_communication?.Stop();
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.LogException(o_exc);
                    Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
                }
            });

            /* CLIENT */

            Task o_client = Task.Run(() =>
            {
                try
                {
                    ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(ForestNET.Lib.Net.Sock.Com.Type.TCP_SEND_WITH_ANSWER, ForestNET.Lib.Net.Sock.Com.Cardinality.Equal, p_s_clientHost, p_i_clientPort, null, 0, p_b_symmetricSecurity128, p_b_symmetricSecurity256, p_b_asymmetricSecurity, p_b_highSecurity, p_e_asymmetricMode, p_b_useMarshalling, p_b_useMarshallingWholeObject, p_i_marshallingDataLengthInBytes, p_b_marshallingUseProperties, p_s_marshallingOverrideMessageType, p_b_marshallingSystemUsesLittleEndian);
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

                        ForestNET.Lib.Global.ILog("message enqueued");
                        a_clientLog.Add("message enqueued");

                        Object? o_answer = o_communication.DequeueWithWaitLoop(i_comDequeueWaitLoopTimeout);

                        if (o_answer != null)
                        {
                            ForestNET.Lib.Global.ILog("#" + (i + 1) + " message(" + o_answer.ToString()?.Length + ") received: '" + o_answer.ToString() + "'");
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
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.LogException(o_exc);
                    Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
                }
            });

            /* WAIT SERVER + CLIENT */

            Task.WaitAll(o_server, o_client);

            /* CHECK LOG ENTRIES */

            Assert.That(a_serverLog, Has.Count.EqualTo(14), "server log has not '10' entries, but '" + a_serverLog.Count + "'");
            Assert.That(a_clientLog, Has.Count.EqualTo(14), "client log has not '10' entries, but '" + a_clientLog.Count + "'");

            int i_expectedLength = 27;

            for (int i = 0; i < 14; i++)
            {
                Assert.That(a_serverLog[i], Does.StartWith("#" + (i + 1) + " message(" + i_expectedLength + ") received"), "server log entry does not start with '#" + (i + 1) + " message(" + i_expectedLength + ") received:', but with '" + a_serverLog[i] + "'");
                string s_foo = DateTime.Now.AddDays(i).ToString("yyyy-MM-dd");
                Assert.That(a_serverLog[i], Does.Contain(s_foo), "server log entry does not contain '" + s_foo + "', entry value: '" + a_serverLog[i] + "'");
                Assert.That(a_clientLog[i], Is.EqualTo("message enqueued"), "client log entry does not match with 'message enqueued', but is '" + a_clientLog[i] + "'");
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
                    Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
                }

                await DoNoting();
            }
        }

        private static void TestCommunicationSharedMemoryUniDirectional(bool p_b_falseUDPtrueTCP, bool p_b_udpWithAck, string p_s_serverHost, int p_i_serverPort, string p_s_clientHost, int p_i_clientPort, string? p_s_clientLocalHost, int p_i_clientLocalPort, bool p_b_symmetricSecurity128, bool p_b_symmetricSecurity256, bool p_b_asymmetricSecurity, bool p_b_highSecurity, AsymmetricModes p_e_asymmetricMode, bool p_b_useMarshalling, bool p_b_useMarshallingWholeObject, int p_i_marshallingDataLengthInBytes, bool p_b_marshallingUseProperties, string? p_s_marshallingOverrideMessageType, bool p_b_marshallingSystemUsesLittleEndian)
        {
            List<string> a_expectedResults = [];
            List<string> a_serverResults = [];
            List<string> a_clientResults = [];

            /* expected result */
            string s_expectedResult = "BigInt = 546789546|Bool = True|Date = NULL|Decimal = 0|DoubleCol = 1,2345|FloatValue = 0|Id = 42|Int = 21|   LocalDate = 03.03.2003 00:00:00|LocalDateTime = NULL|LocalTime = NULL|ShortText = NULL|ShortText2 = NULL|SmallInt = 0|Text = Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.|Text2 = NULL|Time = NULL|Timestamp = NULL|UUID = a8dfc91d-ec7e-4a5f-9a9c-243edd91e271|";

            foreach (string s_foo in s_expectedResult.Split('|'))
            {
                a_expectedResults.Add(s_foo);
            }

            /* SERVER */

            Task o_server = Task.Run(() =>
            {
                try
                {
                    ForestNET.Lib.Net.Sock.Com.Type e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE;

                    if ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck))
                    {
                        e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE_WITH_ACK;
                    }

                    if (p_b_falseUDPtrueTCP)
                    {
                        e_type = ForestNET.Lib.Net.Sock.Com.Type.TCP_RECEIVE;
                    }

                    ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(e_type, ForestNET.Lib.Net.Sock.Com.Cardinality.ManyMessageBoxesToOneSocket, p_s_serverHost, p_i_serverPort, null, 0, p_b_symmetricSecurity128, p_b_symmetricSecurity256, p_b_asymmetricSecurity, p_b_highSecurity, p_e_asymmetricMode, p_b_useMarshalling, p_b_useMarshallingWholeObject, p_i_marshallingDataLengthInBytes, p_b_marshallingUseProperties, p_s_marshallingOverrideMessageType, p_b_marshallingSystemUsesLittleEndian);

                    SharedMemoryExample o_sharedMemoryExample = new();
                    o_sharedMemoryExample.InitiateMirrors().Wait();
                    o_communicationConfig.SharedMemory = o_sharedMemoryExample;
                    o_communicationConfig.SharedMemoryTimeoutMilliseconds = 10;

                    ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                    o_communication.Start();

                    int i_additionalTime = 25;

                    if (p_b_highSecurity)
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

                    if ((p_b_falseUDPtrueTCP) && (p_b_asymmetricSecurity))
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
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.LogException(o_exc);
                    Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
                }
            });

            /* CLIENT */

            Task o_client = Task.Run(() =>
            {
                try
                {
                    ForestNET.Lib.Net.Sock.Com.Type e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND;

                    if ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck))
                    {
                        e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND_WITH_ACK;
                    }

                    if (p_b_falseUDPtrueTCP)
                    {
                        e_type = ForestNET.Lib.Net.Sock.Com.Type.TCP_SEND;
                    }

                    ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(e_type, ForestNET.Lib.Net.Sock.Com.Cardinality.ManyMessageBoxesToOneSocket, p_s_clientHost, p_i_clientPort, p_s_clientLocalHost, p_i_clientLocalPort, p_b_symmetricSecurity128, p_b_symmetricSecurity256, p_b_asymmetricSecurity, p_b_highSecurity, p_e_asymmetricMode, p_b_useMarshalling, p_b_useMarshallingWholeObject, p_i_marshallingDataLengthInBytes, p_b_marshallingUseProperties, p_s_marshallingOverrideMessageType, p_b_marshallingSystemUsesLittleEndian);

                    SharedMemoryExample o_sharedMemoryExample = new();
                    o_sharedMemoryExample.InitiateMirrors().Wait();
                    o_communicationConfig.SharedMemory = o_sharedMemoryExample;
                    o_communicationConfig.SharedMemoryTimeoutMilliseconds = 10;

                    ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                    o_communication.Start();

                    int i_additionalTime = 25;

                    if (p_b_highSecurity)
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

                    if ((p_b_falseUDPtrueTCP) && (p_b_asymmetricSecurity))
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
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.LogException(o_exc);
                    Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
                }
            });

            /* WAIT SERVER + CLIENT */

            Task.WaitAll(o_server, o_client);

            /* CHECK LOG ENTRIES */

            Assert.That(a_expectedResults, Has.Count.EqualTo(a_serverResults.Count), "server result has not the expected amount of fields '" + a_expectedResults.Count + "!='" + a_serverResults.Count);
            Assert.That(a_expectedResults, Has.Count.EqualTo(a_clientResults.Count), "client result has not the expected amount of fields '" + a_expectedResults.Count + "!='" + a_clientResults.Count);

            int i_missingFieldsServer = 0;
            int i_missingFieldsClient = 0;

            for (int i = 0; i < a_expectedResults.Count; i++)
            {
                if (!a_expectedResults[i].Equals(a_serverResults[i]))
                {
                    i_missingFieldsServer++;
                    //Console.WriteLine("server field result not equal expected result:\t" + a_serverResults[i] + "\t != \t" + a_expectedResults[i]);
                }

                if (!a_expectedResults[i].Equals(a_clientResults[i]))
                {
                    i_missingFieldsClient++;
                    //Console.WriteLine("client field result not equal expected result:\t" + a_clientResults[i] + "\t != \t" + a_expectedResults[i]);
                }
            }

            Assert.That(i_missingFieldsServer, Is.LessThanOrEqualTo(5), i_missingFieldsServer + " server fields not matching expected values");
            Assert.That(i_missingFieldsClient, Is.LessThanOrEqualTo(5), i_missingFieldsClient + " client fields not matching expected values");

            //if (i_missingFieldsServer > 0)
            //{
            //    Console.WriteLine("uni " + ((p_b_falseUDPtrueTCP) ? "tcp" : "udp") + " server: " + i_missingFieldsServer);
            //}
            //else
            //{
            //    Console.WriteLine("uni " + ((p_b_falseUDPtrueTCP) ? "tcp" : "udp") + " server: fine");
            //}

            //if (i_missingFieldsClient > 0)
            //{
            //    Console.WriteLine("uni " + ((p_b_falseUDPtrueTCP) ? "tcp" : "udp") + " client: " + i_missingFieldsClient);
            //}
            //else
            //{
            //    Console.WriteLine("uni " + ((p_b_falseUDPtrueTCP) ? "tcp" : "udp") + " client: fine");
            //}
        }

        private static void TestCommunicationSharedMemoryBiDirectional(bool p_b_falseUDPtrueTCP, bool p_b_udpWithAck, string p_s_serverHost, int p_i_serverPort, string p_s_clientHost, int p_i_clientPort, string p_s_biServerHost, int p_i_biServerPort, string p_s_biClientHost, int p_i_biClientPort, string? p_s_clientLocalHost, int p_i_clientLocalPort, bool p_b_symmetricSecurity128, bool p_b_symmetricSecurity256, bool p_b_asymmetricSecurity, bool p_b_highSecurity, AsymmetricModes p_e_asymmetricMode, bool p_b_useMarshalling, bool p_b_useMarshallingWholeObject, int p_i_marshallingDataLengthInBytes, bool p_b_marshallingUseProperties, string? p_s_marshallingOverrideMessageType, bool p_b_marshallingSystemUsesLittleEndian)
        {
            string s_resourcesDirectory = s_currentDirectory + "Resources" + ForestNET.Lib.IO.File.DIR + "com" + ForestNET.Lib.IO.File.DIR;

            List<string> a_expectedResults = [];
            List<string> a_serverResults = [];
            List<string> a_clientResults = [];

            /* expected result */
            string s_expectedResult = "BigInt = 0|Bool = False|Date = NULL|Decimal = 0|DoubleCol = 5,4321|FloatValue = 2,114014|Id = 42|Int = 50791|LocalDate = 04.04.2004 00:00:00|LocalDateTime = NULL|LocalTime = NULL|ShortText = NULL|ShortText2 = Mission accomplished.|SmallInt = 0|Text = Nam liber tempor cum soluta nobis eleifend option congue nihil imperdiet doming id quod mazim placerat facer possim assum.|Text2 = At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.|Time = NULL|Timestamp = NULL|UUID = 26cf332e-3f23-4523-9911-60207c8db7fd|";

            foreach (string s_foo in s_expectedResult.Split('|'))
            {
                a_expectedResults.Add(s_foo);
            }

            /* SERVER */

            Task o_server = Task.Run(() =>
            {
                try
                {
                    ForestNET.Lib.Net.Sock.Com.Type e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE;

                    if ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck))
                    {
                        e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE_WITH_ACK;
                    }

                    if (p_b_falseUDPtrueTCP)
                    {
                        e_type = ForestNET.Lib.Net.Sock.Com.Type.TCP_RECEIVE;
                    }

                    ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(e_type, ForestNET.Lib.Net.Sock.Com.Cardinality.ManyMessageBoxesToOneSocket, p_s_serverHost, p_i_serverPort, null, 0, p_b_symmetricSecurity128, p_b_symmetricSecurity256, p_b_asymmetricSecurity, p_b_highSecurity, p_e_asymmetricMode, p_b_useMarshalling, p_b_useMarshallingWholeObject, p_i_marshallingDataLengthInBytes, p_b_marshallingUseProperties, p_s_marshallingOverrideMessageType, p_b_marshallingSystemUsesLittleEndian);

                    SharedMemoryExample o_sharedMemoryExample = new();
                    o_sharedMemoryExample.InitiateMirrors().Wait();
                    o_communicationConfig.SharedMemory = o_sharedMemoryExample;
                    o_communicationConfig.SharedMemoryTimeoutMilliseconds = 10;

                    if (p_b_asymmetricSecurity)
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
                            s_pathToCertificateFile = s_resourcesDirectory + "client" + ForestNET.Lib.IO.File.DIR + "cert-client-no-pw.pfx";
                        }
                        else if (p_e_asymmetricMode == AsymmetricModes.FileWithPassword)
                        {
                            s_pathToCertificateFile = s_resourcesDirectory + "client" + ForestNET.Lib.IO.File.DIR + "cert-client-with-pw.pfx";
                            s_pathToCertificateFilePassword = "123456";
                        }
                        else if (p_e_asymmetricMode == AsymmetricModes.ThumbprintInStore)
                        {
                            s_certificateThumbprint = ThumbPrintClientCertificate;
                            e_certificateStoreName = System.Security.Cryptography.X509Certificates.StoreName.Root;
                            e_certificateStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser;
                        }
                        else if (p_e_asymmetricMode == AsymmetricModes.ThumbprintInStoreAndClientRemoteCertificateName)
                        {
                            s_certificateThumbprint = ThumbPrintClientCertificate;
                            e_certificateStoreName = System.Security.Cryptography.X509Certificates.StoreName.Root;
                            e_certificateStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser;
                            s_clientRemoteCertificateName = ThumbPrintClientCertificateName;
                        }

                        o_communicationConfig.SetSharedMemoryBidirectional(
                            new Dictionary<string, int> {
                                { p_s_biServerHost, p_i_biServerPort }
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
                                { p_s_biServerHost, p_i_biServerPort }
                            }
                        );
                    }

                    ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                    o_communication.Start();

                    int i_additionalTime = 25;

                    if (p_b_highSecurity)
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

                    if ((p_b_falseUDPtrueTCP) && (p_b_asymmetricSecurity))
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
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.LogException(o_exc);
                    Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
                }
            });

            /* CLIENT */

            Task o_client = Task.Run(() =>
            {
                try
                {
                    ForestNET.Lib.Net.Sock.Com.Type e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND;

                    if ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck))
                    {
                        e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND_WITH_ACK;
                    }

                    if (p_b_falseUDPtrueTCP)
                    {
                        e_type = ForestNET.Lib.Net.Sock.Com.Type.TCP_SEND;
                    }

                    ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(e_type, ForestNET.Lib.Net.Sock.Com.Cardinality.ManyMessageBoxesToOneSocket, p_s_clientHost, p_i_clientPort, p_s_clientLocalHost, p_i_clientLocalPort, p_b_symmetricSecurity128, p_b_symmetricSecurity256, p_b_asymmetricSecurity, p_b_highSecurity, p_e_asymmetricMode, p_b_useMarshalling, p_b_useMarshallingWholeObject, p_i_marshallingDataLengthInBytes, p_b_marshallingUseProperties, p_s_marshallingOverrideMessageType, p_b_marshallingSystemUsesLittleEndian);

                    SharedMemoryExample o_sharedMemoryExample = new();
                    o_sharedMemoryExample.InitiateMirrors().Wait();
                    o_communicationConfig.SharedMemory = o_sharedMemoryExample;
                    o_communicationConfig.SharedMemoryTimeoutMilliseconds = 10;

                    if (p_b_asymmetricSecurity)
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
                            s_pathToCertificateFile = s_resourcesDirectory + "client" + ForestNET.Lib.IO.File.DIR + "cert-client-no-pw.pfx";
                        }
                        else if (p_e_asymmetricMode == AsymmetricModes.FileWithPassword)
                        {
                            s_pathToCertificateFile = s_resourcesDirectory + "client" + ForestNET.Lib.IO.File.DIR + "cert-client-with-pw.pfx";
                            s_pathToCertificateFilePassword = "123456";
                        }
                        else if (p_e_asymmetricMode == AsymmetricModes.ThumbprintInStore)
                        {
                            s_certificateThumbprint = ThumbPrintClientCertificate;
                            e_certificateStoreName = System.Security.Cryptography.X509Certificates.StoreName.Root;
                            e_certificateStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser;
                        }
                        else if (p_e_asymmetricMode == AsymmetricModes.ThumbprintInStoreAndClientRemoteCertificateName)
                        {
                            s_certificateThumbprint = ThumbPrintClientCertificate;
                            e_certificateStoreName = System.Security.Cryptography.X509Certificates.StoreName.Root;
                            e_certificateStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser;
                            s_clientRemoteCertificateName = ThumbPrintClientCertificateName;
                        }

                        o_communicationConfig.SetSharedMemoryBidirectional(
                            new Dictionary<string, int> {
                                { p_s_biClientHost, p_i_biClientPort }
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
                                { p_s_biClientHost, p_i_biClientPort }
                            }
                        );
                    }

                    ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                    o_communication.Start();

                    int i_additionalTime = 25;

                    if (p_b_highSecurity)
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

                    if ((p_b_falseUDPtrueTCP) && (p_b_asymmetricSecurity))
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
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.LogException(o_exc);
                    Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
                }
            });

            /* WAIT SERVER + CLIENT */

            Task.WaitAll(o_server, o_client);

            /* CHECK LOG ENTRIES */

            Assert.That(a_expectedResults, Has.Count.EqualTo(a_serverResults.Count), "server result has not the expected amount of fields '" + a_expectedResults.Count + "!='" + a_serverResults.Count);
            Assert.That(a_expectedResults, Has.Count.EqualTo(a_clientResults.Count), "client result has not the expected amount of fields '" + a_expectedResults.Count + "!='" + a_clientResults.Count);

            int i_missingFieldsServer = 0;
            int i_missingFieldsClient = 0;

            for (int i = 0; i < a_expectedResults.Count; i++)
            {
                if (!a_expectedResults[i].Equals(a_serverResults[i]))
                {
                    i_missingFieldsServer++;
                    //Console.WriteLine("server field result not equal expected result:\t" + a_serverResults[i] + "\t != \t" + a_expectedResults[i]);
                }

                if (!a_expectedResults[i].Equals(a_clientResults[i]))
                {
                    i_missingFieldsClient++;
                    //Console.WriteLine("client field result not equal expected result:\t" + a_clientResults[i] + "\t != \t" + a_expectedResults[i]);
                }
            }

            Assert.That(i_missingFieldsServer, Is.LessThanOrEqualTo(4), i_missingFieldsServer + " server fields not matching expected values");
            Assert.That(i_missingFieldsClient, Is.LessThanOrEqualTo(4), i_missingFieldsClient + " client fields not matching expected values");

            //if (i_missingFieldsServer > 0)
            //{
            //    Console.WriteLine("bi " + ((p_b_falseUDPtrueTCP) ? "tcp" : "udp") + " server: " + i_missingFieldsServer);
            //}
            //else
            //{
            //    Console.WriteLine("bi " + ((p_b_falseUDPtrueTCP) ? "tcp" : "udp") + " server: fine");
            //}

            //if (i_missingFieldsClient > 0)
            //{
            //    Console.WriteLine("bi " + ((p_b_falseUDPtrueTCP) ? "tcp" : "udp") + " client: " + i_missingFieldsClient);
            //}
            //else
            //{
            //    Console.WriteLine("bi " + ((p_b_falseUDPtrueTCP) ? "tcp" : "udp") + " client: fine");
            //}
        }

        private static void TestCommunicationMarshallingObject(bool p_b_falseUDPtrueTCP, bool p_b_udpWithAck, bool p_b_smallObject, string p_s_serverHost, int p_i_serverPort, string p_s_clientHost, int p_i_clientPort, string? p_s_clientLocalHost, int p_i_clientLocalPort, bool p_b_symmetricSecurity128, bool p_b_symmetricSecurity256, bool p_b_asymmetricSecurity, bool p_b_highSecurity, AsymmetricModes p_e_asymmetricMode, int p_i_marshallingDataLengthInBytes, bool p_b_marshallingUseProperties, string? p_s_marshallingOverrideMessageType, bool p_b_marshallingSystemUsesLittleEndian)
        {
            List<Object> a_serverObjects = [];
            List<Object> a_clientObjects = [];

            int i_comDequeueWaitLoopTimeout = 5000;
            int i_iterations = 10;

            /* SERVER */

            Task o_server = Task.Run(() =>
            {
                try
                {
                    ForestNET.Lib.Net.Sock.Com.Type e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE;

                    if ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck))
                    {
                        e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE_WITH_ACK;
                    }

                    if (p_b_falseUDPtrueTCP)
                    {
                        e_type = ForestNET.Lib.Net.Sock.Com.Type.TCP_RECEIVE;
                    }

                    ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(e_type, ForestNET.Lib.Net.Sock.Com.Cardinality.Equal, p_s_serverHost, p_i_serverPort, null, 0, p_b_symmetricSecurity128, p_b_symmetricSecurity256, p_b_asymmetricSecurity, p_b_highSecurity, p_e_asymmetricMode, true, false, p_i_marshallingDataLengthInBytes, p_b_marshallingUseProperties, p_s_marshallingOverrideMessageType, p_b_marshallingSystemUsesLittleEndian);

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
                                (((ForestNET.Tests.Net.Msg.MessageObject)o_object).StringArray ?? [])[5] = "";
                                ((ForestNET.Tests.Net.Msg.MessageObject)o_object).StringList[5] = "";
                            }

                            a_serverObjects.Add(o_object);
                            ForestNET.Lib.Global.ILog("#" + (i + 1) + " object received");
                        }
                        else
                        {
                            ForestNET.Lib.Global.ILogWarning("could not receive any data");
                        }
                    }

                    o_communication?.Stop();
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.LogException(o_exc);
                    Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
                }
            });

            /* CLIENT */

            Task o_client = Task.Run(() =>
            {
                try
                {
                    ForestNET.Lib.Net.Sock.Com.Type e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND;

                    if ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck))
                    {
                        e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND_WITH_ACK;
                    }

                    if (p_b_falseUDPtrueTCP)
                    {
                        e_type = ForestNET.Lib.Net.Sock.Com.Type.TCP_SEND;
                    }

                    ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(e_type, ForestNET.Lib.Net.Sock.Com.Cardinality.Equal, p_s_clientHost, p_i_clientPort, p_s_clientLocalHost, p_i_clientLocalPort, p_b_symmetricSecurity128, p_b_symmetricSecurity256, p_b_asymmetricSecurity, p_b_highSecurity, p_e_asymmetricMode, true, false, p_i_marshallingDataLengthInBytes, p_b_marshallingUseProperties, p_s_marshallingOverrideMessageType, p_b_marshallingSystemUsesLittleEndian);

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
                            o_foo = new ForestNET.Tests.Net.Msg.SmallMessageObject();
                            ((ForestNET.Tests.Net.Msg.SmallMessageObject)o_foo).InitAll();
                        }
                        else
                        {
                            o_foo = new ForestNET.Tests.Net.Msg.MessageObject();
                            ((ForestNET.Tests.Net.Msg.MessageObject)o_foo).InitAll();
                        }

                        while (!o_communication.Enqueue(
                            o_foo
                        ))
                        {
                            ForestNET.Lib.Global.ILogWarning("could not enqueue object");
                        }

                        ForestNET.Lib.Global.ILog("object enqueued");
                        a_clientObjects.Add(o_foo);

                        if (i == 4)
                        { /* additional delay for 25 milliseconds times sleep multiplier constant, after the 5th time enqueue has been executed */
                            Thread.Sleep(25 * i_sleepMultiplier);
                        }

                        Thread.Sleep(25 * i_sleepMultiplier + (((p_b_falseUDPtrueTCP) && (p_b_highSecurity)) ? 750 : 0) + (((!p_b_falseUDPtrueTCP) && (p_b_highSecurity)) ? 50 : 0) + ((p_b_udpWithAck) ? 225 : 0));
                    }

                    o_communication?.Stop();
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.LogException(o_exc);
                    Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
                }
            });

            /* WAIT SERVER + CLIENT */

            Task.WaitAll(o_server, o_client);

            /* CHECK LOG ENTRIES */

            if (p_b_udpWithAck)
            {
                int i_limit = 7;

                if (!p_b_smallObject)
                {
                    i_limit = 4;
                }

                Assert.That(a_serverObjects, Has.Count.GreaterThan(i_limit), "server object list must have at least '" + (i_limit + 1) + "' entries, but '" + a_serverObjects.Count + "'");
                Assert.That(a_clientObjects, Has.Count.GreaterThan(i_limit), "client object list must have at least '" + (i_limit + 1) + "' entries, but '" + a_clientObjects.Count + "'");

                for (int i = 0; i < a_serverObjects.Count; i++)
                {
                    Assert.That(ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_serverObjects[i], a_clientObjects[i], true, false, false), "server object and client object are not equal");
                }
            }
            else
            {
                int i_limit = 9;

                if (!p_b_smallObject)
                {
                    i_limit = 6;
                }

                Assert.That(a_serverObjects, Has.Count.GreaterThan(i_limit), "server object list must have at least '" + (i_limit + 1) + "' entries, but '" + a_serverObjects.Count + "'");
                Assert.That(a_clientObjects, Has.Count.GreaterThan(i_limit), "client object list must have at least '" + (i_limit + 1) + "' entries, but '" + a_clientObjects.Count + "'");

                for (int i = 0; i < a_serverObjects.Count; i++)
                {
                    Assert.That(ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_serverObjects[i], a_clientObjects[i], true, false, false), "server object and client object are not equal");
                }
            }
        }

        private static void TestCommunicationMarshallingSharedMemoryUniDirectional(bool p_b_falseUDPtrueTCP, bool p_b_smallObject, bool p_b_massChangeAtEnd, bool p_b_udpWithAck, string p_s_serverHost, int p_i_serverPort, string p_s_clientHost, int p_i_clientPort, string? p_s_clientLocalHost, int p_i_clientLocalPort, bool p_b_symmetricSecurity128, bool p_b_symmetricSecurity256, bool p_b_asymmetricSecurity, bool p_b_highSecurity, AsymmetricModes p_e_asymmetricMode, bool p_b_useMarshalling, bool p_b_useMarshallingWholeObject, int p_i_marshallingDataLengthInBytes, bool p_b_marshallingUseProperties, string? p_s_marshallingOverrideMessageType, bool p_b_marshallingSystemUsesLittleEndian)
        {
            List<string> a_expectedResults = [];
            List<string> a_serverResults = [];
            List<string> a_clientResults = [];

            /* expected result */
            string s_expectedResult = "_Bool = True|_BoolArray = NULL|_BoolList = []|_Byte = 0|_ByteArray = NULL|_ByteList = [1, 3, 5, 133, 42, 0, NULL, 102]|_Char = o|_CharArray = NULL|_CharList = [A, F, K, " + ((char)133) + ", U, " + ((char)0) + ", NULL, ó]|_Date = 04.03.2020 00:00:00|_DateArray = NULL|_DateList = []|_DateTime = NULL|_DateTimeArray = [04.03.2020 06:02:03, 08.06.2020 09:24:16, 16.12.2020 12:48:53, NULL]|_DateTimeList = []|_Decimal = 0|_DecimalArray = NULL|_DecimalList = []|_Double = 0|_DoubleArray = NULL|_DoubleList = [1,25, 3,5, 5,75, 10,10, -41,998, 0, NULL, 8798546,2154656]|_Float = 0|_FloatArray = [1,25, 3,5, 5,75, 10,10, 41,998, 0, 4984654,5]|_FloatList = []|_Integer = 0|_IntegerArray = [1, 3, 5, 536870954, 42, 0]|_IntegerList = [1, 3, 5, 536870954, -42, 0, NULL]|_LocalDate = NULL|_LocalDateArray = [04.03.2020 00:00:00, 08.06.2020 00:00:00, 16.12.2020 00:00:00, NULL]|_LocalDateList = [04.03.2020 00:00:00, 08.06.2020 00:00:00, 16.12.2020 00:00:00, NULL]|_LocalDateTime = 04.03.2020 06:02:03|_LocalDateTimeArray = NULL|_LocalDateTimeList = [04.03.2020 06:02:03, 08.06.2020 09:24:16, 16.12.2020 12:48:53, NULL]|_LocalTime = 01.01.1970 22:16:06|_LocalTimeArray = NULL|_LocalTimeList = []|_Long = 1170936177994235946|_LongArray = NULL|_LongList = [1, 3, 5, 1170936177994235946, -42, 0, NULL]|_Short = 16426|_ShortArray = NULL|_ShortList = []|_SignedByte = 0|_SignedByteArray = NULL|_SignedByteList = []|_String = Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.|_StringArray = NULL|_StringList = [Hello World 1!, Hello World 2!, Hello World 3!, Hello World 4!, Hello World 5!, NULL, NULL]|_Time = 01.01.1970 06:02:03|_TimeArray = NULL|_TimeList = []|_UnsignedInteger = 536870954|_UnsignedIntegerArray = NULL|_UnsignedIntegerList = []|_UnsignedLong = 0|_UnsignedLongArray = [1, 3, 5, 1170936177994235946, 42, 0]|_UnsignedLongList = []|_UnsignedShort = 16426|_UnsignedShortArray = NULL|_UnsignedShortList = []|";

            if (p_b_smallObject)
            {
                s_expectedResult = "_Bool = True|_Char = o|_DecimalArray = NULL|_IntegerArray = [1, 3, 5, 536870954, 42, 0]|_LocalDateArray = [04.03.2020 00:00:00, 08.06.2020 00:00:00, 16.12.2020 00:00:00, NULL]|_LocalDateTimeList = [04.03.2020 06:02:03, 08.06.2020 09:24:16, 16.12.2020 12:48:53, NULL]|_LocalTime = 01.01.1970 22:16:06|_LongList = [1, 3, 5, 1170936177994235946, -42, 0, NULL]|_String = Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.|";
            }

            if (p_b_massChangeAtEnd)
            {
                if (p_b_smallObject)
                {
                    s_expectedResult = "_Bool = True|_Char = ò|_DecimalArray = [578875020153,73804901109397, -36,151686185423327, 71740124,12171120119, -2043204985254,1196, 0, 601,9924]|_IntegerArray = [1, 3, 5, 536870954, -42, 0]|_LocalDateArray = [04.03.2020 00:00:00, 08.06.2020 00:00:00, 16.12.2020 00:00:00, NULL]|_LocalDateTimeList = [04.03.2020 06:02:03, 08.06.2020 09:24:16, 16.12.2020 12:48:53, NULL]|_LocalTime = 01.01.1970 06:02:03|_LongList = [1, 3, 5, 1170936177994235946, -42, 0, NULL]|_String = Hello World!|";
                }
                else
                {
                    s_expectedResult = "_Bool = True|_BoolArray = [True, False, True, False, True]|_BoolList = [True, False, True, False, True, NULL]|_Byte = 42|_ByteArray = [1, 3, 5, 133, 42, 0, 102]|_ByteList = [1, 3, 5, 133, 42, 0, NULL, 102]|_Char = ò|_CharArray = [A, F, K, " + ((char)133) + ", U, " + ((char)0) + ", ó]|_CharList = [A, F, K, " + ((char)133) + ", U, " + ((char)0) + ", NULL, ó]|_Date = 04.03.2020 00:00:00|_DateArray = [04.03.2020 00:00:00, 08.06.2020 00:00:00, 16.12.2020 00:00:00, NULL]|_DateList = [04.03.2020 00:00:00, 08.06.2020 00:00:00, 16.12.2020 00:00:00, NULL]|_DateTime = 04.03.2020 06:02:03|_DateTimeArray = [04.03.2020 06:02:03, 08.06.2020 09:24:16, 16.12.2020 12:48:53, NULL]|_DateTimeList = [04.03.2020 06:02:03, 08.06.2020 09:24:16, 16.12.2020 12:48:53, NULL]|_Decimal = -268435477,6710886925|_DecimalArray = [578875020153,73804901109397, -36,151686185423327, 71740124,12171120119, -2043204985254,1196, 0, 601,9924]|_DecimalList = [578875020153,73804901109397, -36,151686185423327, 71740124,12171120119, -2043204985254,1196, 0, 601,9924]|_Double = 42,75|_DoubleArray = [1,25, 3,5, 5,75, 10,101, -41,998, 0, 8798546,2154656]|_DoubleList = [1,25, 3,5, 5,75, 10,101, -41,998, 0, NULL, 8798546,2154656]|_Float = 42,25|_FloatArray = [1,25, 3,5, 5,75, 10,101, -41,998, 0, 4984654,5]|_FloatList = [1,25, 3,5, 5,75, 10,101, -41,998, 0, NULL, 4984654,5]|_Integer = 536870954|_IntegerArray = [1, 3, 5, 536870954, -42, 0]|_IntegerList = [1, 3, 5, 536870954, -42, 0, NULL]|_LocalDate = 04.03.2020 00:00:00|_LocalDateArray = [04.03.2020 00:00:00, 08.06.2020 00:00:00, 16.12.2020 00:00:00, NULL]|_LocalDateList = [04.03.2020 00:00:00, 08.06.2020 00:00:00, 16.12.2020 00:00:00, NULL]|_LocalDateTime = 04.03.2020 06:02:03|_LocalDateTimeArray = [04.03.2020 06:02:03, 08.06.2020 09:24:16, 16.12.2020 12:48:53, NULL]|_LocalDateTimeList = [04.03.2020 06:02:03, 08.06.2020 09:24:16, 16.12.2020 12:48:53, NULL]|_LocalTime = 01.01.1970 06:02:03|_LocalTimeArray = [01.01.1970 06:02:03, 01.01.1970 09:24:16, 01.01.1970 12:48:53, NULL]|_LocalTimeList = [01.01.1970 06:02:03, 01.01.1970 09:24:16, 01.01.1970 12:48:53, NULL]|_Long = 1170936177994235946|_LongArray = [1, 3, 5, 1170936177994235946, -42, 0]|_LongList = [1, 3, 5, 1170936177994235946, -42, 0, NULL]|_Short = 16426|_ShortArray = [1, 3, 5, 16426, -42, 0]|_ShortList = [1, 3, 5, 10, -42, 0, NULL]|_SignedByte = 42|_SignedByteArray = [1, 3, 5, -10, 42, 0, -102]|_SignedByteList = [1, 3, 5, -10, 42, 0, NULL, -102]|_String = Hello World!|_StringArray = [Hello World 1!, Hello World 2!, Hello World 3!, Hello World 4!, Hello World 5!, , NULL]|_StringList = [Hello World 1!, Hello World 2!, Hello World 3!, Hello World 4!, Hello World 5!, , NULL]|_Time = 01.01.1970 06:02:03|_TimeArray = [01.01.1970 06:02:03, 01.01.1970 09:24:16, 01.01.1970 12:48:53, NULL]|_TimeList = [01.01.1970 06:02:03, 01.01.1970 09:24:16, 01.01.1970 12:48:53, NULL]|_UnsignedInteger = 536870954|_UnsignedIntegerArray = [1, 3, 5, 536870954, 42, 0]|_UnsignedIntegerList = [1, 3, 5, 536870954, 42, 0, NULL]|_UnsignedLong = 1170936177994235946|_UnsignedLongArray = [1, 3, 5, 1170936177994235946, 42, 0]|_UnsignedLongList = [1, 3, 5, 1170936177994235946, 42, 0, NULL]|_UnsignedShort = 16426|_UnsignedShortArray = [1, 3, 5, 16426, 42, 0]|_UnsignedShortList = [1, 3, 5, 16426, 42, 0, NULL]|";
                }
            }

            foreach (string s_foo in s_expectedResult.Split('|'))
            {
                a_expectedResults.Add(s_foo);
            }

            /* SERVER */

            Task o_server = Task.Run(() =>
            {
                try
                {
                    ForestNET.Lib.Net.Sock.Com.Type e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE;

                    if ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck))
                    {
                        e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE_WITH_ACK;
                    }

                    if (p_b_falseUDPtrueTCP)
                    {
                        e_type = ForestNET.Lib.Net.Sock.Com.Type.TCP_RECEIVE;
                    }

                    ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(e_type, ForestNET.Lib.Net.Sock.Com.Cardinality.ManyMessageBoxesToOneSocket, p_s_serverHost, p_i_serverPort, null, 0, p_b_symmetricSecurity128, p_b_symmetricSecurity256, p_b_asymmetricSecurity, p_b_highSecurity, p_e_asymmetricMode, p_b_useMarshalling, p_b_useMarshallingWholeObject, p_i_marshallingDataLengthInBytes, p_b_marshallingUseProperties, p_s_marshallingOverrideMessageType, p_b_marshallingSystemUsesLittleEndian);
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
                        if (p_b_highSecurity)
                        {
                            i_additionalTime *= 3;
                        }

                        if (p_b_asymmetricSecurity)
                        {
                            i_additionalTime *= 3;
                        }

                        if (p_b_massChangeAtEnd)
                        {
                            /* nothing */
                        }

                        if (p_i_marshallingDataLengthInBytes > 3)
                        {
                            i_additionalTime *= (p_i_marshallingDataLengthInBytes / 2);
                        }
                    }
                    else
                    { /* UDP */
                        if (p_b_highSecurity)
                        {
                            i_additionalTime *= 5;
                        }

                        if (p_b_udpWithAck)
                        {
                            i_additionalTime += 225;
                        }

                        if (p_b_massChangeAtEnd)
                        {
                            i_additionalTime += i_additionalTime / 2;
                        }

                        i_additionalTime *= p_i_marshallingDataLengthInBytes;
                    }

                    Thread.Sleep((25 * i_sleepMultiplier) + i_additionalTime);

                    Thread.Sleep((25 * i_sleepMultiplier) + i_additionalTime);

                    Thread.Sleep((25 * i_sleepMultiplier) + i_additionalTime);

                    Thread.Sleep((25 * i_sleepMultiplier) + i_additionalTime);

                    if (p_b_massChangeAtEnd)
                    {
                        Thread.Sleep((25 * i_sleepMultiplier) + i_additionalTime);
                    }

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
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.LogException(o_exc);
                    Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
                }
            });

            /* CLIENT */

            Task o_client = Task.Run(() =>
            {
                try
                {
                    ForestNET.Lib.Net.Sock.Com.Type e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND;

                    if ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck))
                    {
                        e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND_WITH_ACK;
                    }

                    if (p_b_falseUDPtrueTCP)
                    {
                        e_type = ForestNET.Lib.Net.Sock.Com.Type.TCP_SEND;
                    }

                    ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(e_type, ForestNET.Lib.Net.Sock.Com.Cardinality.ManyMessageBoxesToOneSocket, p_s_clientHost, p_i_clientPort, p_s_clientLocalHost, p_i_clientLocalPort, p_b_symmetricSecurity128, p_b_symmetricSecurity256, p_b_asymmetricSecurity, p_b_highSecurity, p_e_asymmetricMode, p_b_useMarshalling, p_b_useMarshallingWholeObject, p_i_marshallingDataLengthInBytes, p_b_marshallingUseProperties, p_s_marshallingOverrideMessageType, p_b_marshallingSystemUsesLittleEndian);
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
                        if (p_b_highSecurity)
                        {
                            i_additionalTime *= 3;
                        }

                        if (p_b_asymmetricSecurity)
                        {
                            i_additionalTime *= 3;
                        }

                        if (p_b_massChangeAtEnd)
                        {
                            /* nothing */
                        }

                        if (p_i_marshallingDataLengthInBytes > 3)
                        {
                            i_additionalTime *= (p_i_marshallingDataLengthInBytes / 2);
                        }
                    }
                    else
                    { /* UDP */
                        if (p_b_highSecurity)
                        {
                            i_additionalTime *= 5;
                        }

                        if (p_b_udpWithAck)
                        {
                            i_additionalTime += 225;
                        }

                        if (p_b_massChangeAtEnd)
                        {
                            i_additionalTime += i_additionalTime / 2;
                        }

                        i_additionalTime *= p_i_marshallingDataLengthInBytes;
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

                    if (p_b_massChangeAtEnd)
                    {
                        if (!p_b_smallObject)
                        {
                            ((SharedMemoryMessageObject)o_sharedMemory).InitAll();
                        }
                        else
                        {
                            ((SharedMemorySmallMessageObject)o_sharedMemory).InitAll();
                        }

                        Thread.Sleep((25 * i_sleepMultiplier) + i_additionalTime);
                    }

                    /* client result */
                    foreach (string s_foo in o_sharedMemory.ReturnFields().Split('|'))
                    {
                        a_clientResults.Add(s_foo);
                    }

                    o_communication?.Stop();
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.LogException(o_exc);
                    Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
                }
            });

            /* WAIT SERVER + CLIENT */

            Task.WaitAll(o_server, o_client);

            /* CHECK LOG ENTRIES */

            int i_missingFieldsServer = 0;
            int i_missingFieldsClient = 0;

            for (int i = 0; i < a_serverResults.Count; i++)
            {
                if (!a_expectedResults[i].Equals(a_serverResults[i]))
                {
                    i_missingFieldsServer++;
                    //Console.WriteLine("server field result not equal expected result:\t" + a_serverResults[i] + "\t != \t" + a_expectedResults[i]);
                }
            }

            for (int i = 0; i < a_clientResults.Count; i++)
            {
                if (!a_expectedResults[i].Equals(a_clientResults[i]))
                {
                    i_missingFieldsClient++;
                    //Console.WriteLine("client field result not equal expected result:\t" + a_clientResults[i] + "\t != \t" + a_expectedResults[i]);
                }
            }

            if (i_missingFieldsServer > 10)
            {
                i_fails++;
            }
            else if (i_missingFieldsClient > 10)
            {
                i_fails++;
            }
            else
            {
                Assert.That(i_missingFieldsServer, Is.LessThanOrEqualTo(5), i_missingFieldsServer + " server fields not matching expected values");
                Assert.That(i_missingFieldsClient, Is.LessThanOrEqualTo(5), i_missingFieldsClient + " client fields not matching expected values");

                if (i_missingFieldsServer > 0)
                {
                    //Console.WriteLine("uni " + ((p_b_falseUDPtrueTCP) ? "tcp" : "udp") + ((p_b_udpWithAck) ? " - with ACK" : "") + ((p_b_smallObject) ? " - small" : "") + ((p_b_massChangeAtEnd) ? " - mass" : "") + " server: " + i_missingFieldsServer);
                }
                else
                {
                    //Console.WriteLine("uni " + ((p_b_falseUDPtrueTCP) ? "tcp" : "udp") + ((p_b_udpWithAck) ? " - with ACK" : "") + ((p_b_smallObject) ? " - small" : "") + ((p_b_massChangeAtEnd) ? " - mass" : "") + " server: fine");
                }

                if (i_missingFieldsClient > 0)
                {
                    //Console.WriteLine("uni " + ((p_b_falseUDPtrueTCP) ? "tcp" : "udp") + ((p_b_udpWithAck) ? " - with ACK" : "") + ((p_b_smallObject) ? " - small" : "") + ((p_b_massChangeAtEnd) ? " - mass" : "") + " client: " + i_missingFieldsClient);
                }
                else
                {
                    //Console.WriteLine("uni " + ((p_b_falseUDPtrueTCP) ? "tcp" : "udp") + ((p_b_udpWithAck) ? " - with ACK" : "") + ((p_b_smallObject) ? " - small" : "") + ((p_b_massChangeAtEnd) ? " - mass" : "") + " client: fine");
                }
            }
        }

        private static void TestCommunicationMarshallingSharedMemoryBiDirectional(bool p_b_falseUDPtrueTCP, bool p_b_smallObject, bool p_b_udpWithAck, string p_s_serverHost, int p_i_serverPort, string p_s_clientHost, int p_i_clientPort, string p_s_biServerHost, int p_i_biServerPort, string p_s_biClientHost, int p_i_biClientPort, string? p_s_clientLocalHost, int p_i_clientLocalPort, bool p_b_symmetricSecurity128, bool p_b_symmetricSecurity256, bool p_b_asymmetricSecurity, bool p_b_highSecurity, AsymmetricModes p_e_asymmetricMode, bool p_b_useMarshalling, bool p_b_useMarshallingWholeObject, int p_i_marshallingDataLengthInBytes, bool p_b_marshallingUseProperties, string? p_s_marshallingOverrideMessageType, bool p_b_marshallingSystemUsesLittleEndian)
        {
            string s_resourcesDirectory = s_currentDirectory + "Resources" + ForestNET.Lib.IO.File.DIR + "com" + ForestNET.Lib.IO.File.DIR;

            List<string> a_expectedResults = [];
            List<string> a_serverResults = [];
            List<string> a_clientResults = [];

            /* expected result */
            string s_expectedResult = "_Bool = True|_BoolArray = NULL|_BoolList = []|_Byte = 0|_ByteArray = NULL|_ByteList = [1, 3, 5, 133, 42, 0, NULL, 102]|_Char = o|_CharArray = NULL|_CharList = [A, F, K, " + ((char)133) + ", U, " + ((char)0) + ", NULL, ó]|_Date = 04.03.2020 00:00:00|_DateArray = NULL|_DateList = []|_DateTime = NULL|_DateTimeArray = [04.03.2020 06:02:03, 08.06.2020 09:24:16, 16.12.2020 12:48:53, NULL]|_DateTimeList = []|_Decimal = 0,0|_DecimalArray = NULL|_DecimalList = []|_Double = 0|_DoubleArray = NULL|_DoubleList = [1,25, 3,5, 5,75, 10,101, -41,998, 0, NULL, 8798546,2154656]|_Float = 0|_FloatArray = [1,25, 3,5, 5,75, 10,101, 41,998, 0, 4984654,5]|_FloatList = []|_Integer = 0|_IntegerArray = [1, 3, 5, 536870954, 42, 0]|_IntegerList = [1, 3, 5, 536870954, -42, 0, NULL]|_LocalDate = NULL|_LocalDateArray = [04.03.2020 00:00:00, 08.06.2020 00:00:00, 16.12.2020 00:00:00, NULL]|_LocalDateList = [04.03.2020 00:00:00, 08.06.2020 00:00:00, 16.12.2020 00:00:00, NULL]|_LocalDateTime = 04.03.2020 06:02:03|_LocalDateTimeArray = NULL|_LocalDateTimeList = [04.03.2020 06:02:03, 08.06.2020 09:24:16, 16.12.2020 12:48:53, NULL]|_LocalTime = 01.01.1970 22:16:06|_LocalTimeArray = NULL|_LocalTimeList = []|_Long = 1170936177994235946|_LongArray = NULL|_LongList = [1, 3, 5, 1170936177994235946, -42, 0, NULL]|_Short = 16426|_ShortArray = NULL|_ShortList = []|_SignedByte = 42|_SignedByteArray = NULL|_SignedByteList = []|_String = Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.|_StringArray = NULL|_StringList = [Hello World 1!, Hello World 2!, Hello World 3!, Hello World 4!, Hello World 5!, NULL, NULL]|_Time = 01.01.1970 06:02:03|_TimeArray = NULL|_TimeList = []|_UnsignedInteger = 536870954|_UnsignedIntegerArray = NULL|_UnsignedIntegerList = []|_UnsignedLong = 0|_UnsignedLongArray = [1, 3, 5, 1170936177994235946, 42, 0]|_UnsignedLongList = []|_UnsignedShort = 16426|_UnsignedShortArray = NULL|_UnsignedShortList = []|";

            if (p_b_smallObject)
            {
                s_expectedResult = "_Bool = True|_Char = o|_DecimalArray = NULL|_IntegerArray = [1, 3, 5, 536870954, 42, 0]|_LocalDateArray = [04.03.2020 00:00:00, 08.06.2020 00:00:00, 16.12.2020 00:00:00, NULL]|_LocalDateTimeList = [04.03.2020 06:02:03, 08.06.2020 09:24:16, 16.12.2020 12:48:53, NULL]|_LocalTime = 01.01.1970 22:16:06|_LongList = [1, 3, 5, 1170936177994235946, -42, 0, NULL]|_String = Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.|";
            }

            foreach (string s_foo in s_expectedResult.Split('|'))
            {
                a_expectedResults.Add(s_foo);
            }

            /* SERVER */

            Task o_server = Task.Run(() =>
            {
                try
                {
                    ForestNET.Lib.Net.Sock.Com.Type e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE;

                    if ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck))
                    {
                        e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE_WITH_ACK;
                    }

                    if (p_b_falseUDPtrueTCP)
                    {
                        e_type = ForestNET.Lib.Net.Sock.Com.Type.TCP_RECEIVE;
                    }

                    ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(e_type, ForestNET.Lib.Net.Sock.Com.Cardinality.ManyMessageBoxesToOneSocket, p_s_serverHost, p_i_serverPort, null, 0, p_b_symmetricSecurity128, p_b_symmetricSecurity256, p_b_asymmetricSecurity, p_b_highSecurity, p_e_asymmetricMode, p_b_useMarshalling, p_b_useMarshallingWholeObject, p_i_marshallingDataLengthInBytes, p_b_marshallingUseProperties, p_s_marshallingOverrideMessageType, p_b_marshallingSystemUsesLittleEndian);
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

                    if (p_b_asymmetricSecurity)
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
                            s_pathToCertificateFile = s_resourcesDirectory + "client" + ForestNET.Lib.IO.File.DIR + "cert-client-no-pw.pfx";
                        }
                        else if (p_e_asymmetricMode == AsymmetricModes.FileWithPassword)
                        {
                            s_pathToCertificateFile = s_resourcesDirectory + "client" + ForestNET.Lib.IO.File.DIR + "cert-client-with-pw.pfx";
                            s_pathToCertificateFilePassword = "123456";
                        }
                        else if (p_e_asymmetricMode == AsymmetricModes.ThumbprintInStore)
                        {
                            s_certificateThumbprint = ThumbPrintClientCertificate;
                            e_certificateStoreName = System.Security.Cryptography.X509Certificates.StoreName.Root;
                            e_certificateStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser;
                        }
                        else if (p_e_asymmetricMode == AsymmetricModes.ThumbprintInStoreAndClientRemoteCertificateName)
                        {
                            s_certificateThumbprint = ThumbPrintClientCertificate;
                            e_certificateStoreName = System.Security.Cryptography.X509Certificates.StoreName.Root;
                            e_certificateStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser;
                            s_clientRemoteCertificateName = ThumbPrintClientCertificateName;
                        }

                        o_communicationConfig.SetSharedMemoryBidirectional(
                            new Dictionary<string, int> {
                                { p_s_biServerHost, p_i_biServerPort }
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
                                { p_s_biServerHost, p_i_biServerPort }
                            }
                        );
                    }

                    ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                    o_communication.Start();

                    int i_additionalTime = 25;

                    if (p_b_falseUDPtrueTCP)
                    { /* TCP */
                        if (p_b_highSecurity)
                        {
                            i_additionalTime *= 3;
                        }

                        if (p_b_asymmetricSecurity)
                        {
                            i_additionalTime *= 3;
                        }

                        if (p_i_marshallingDataLengthInBytes > 3)
                        {
                            i_additionalTime *= (p_i_marshallingDataLengthInBytes / 2);
                        }
                    }
                    else
                    { /* UDP */
                        if (p_b_highSecurity)
                        {
                            i_additionalTime *= 5;
                        }

                        if (p_b_udpWithAck)
                        {
                            i_additionalTime += 225;
                        }

                        i_additionalTime *= p_i_marshallingDataLengthInBytes;
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
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.LogException(o_exc);
                    Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
                }
            });

            /* CLIENT */

            Task o_client = Task.Run(() =>
            {
                try
                {
                    ForestNET.Lib.Net.Sock.Com.Type e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND;

                    if ((!p_b_falseUDPtrueTCP) && (p_b_udpWithAck))
                    {
                        e_type = ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND_WITH_ACK;
                    }

                    if (p_b_falseUDPtrueTCP)
                    {
                        e_type = ForestNET.Lib.Net.Sock.Com.Type.TCP_SEND;
                    }

                    ForestNET.Lib.Net.Sock.Com.Config o_communicationConfig = GetCommunicationConfig(e_type, ForestNET.Lib.Net.Sock.Com.Cardinality.ManyMessageBoxesToOneSocket, p_s_clientHost, p_i_clientPort, p_s_clientLocalHost, p_i_clientLocalPort, p_b_symmetricSecurity128, p_b_symmetricSecurity256, p_b_asymmetricSecurity, p_b_highSecurity, p_e_asymmetricMode, p_b_useMarshalling, p_b_useMarshallingWholeObject, p_i_marshallingDataLengthInBytes, p_b_marshallingUseProperties, p_s_marshallingOverrideMessageType, p_b_marshallingSystemUsesLittleEndian);
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

                    if (p_b_asymmetricSecurity)
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
                            s_pathToCertificateFile = s_resourcesDirectory + "client" + ForestNET.Lib.IO.File.DIR + "cert-client-no-pw.pfx";
                        }
                        else if (p_e_asymmetricMode == AsymmetricModes.FileWithPassword)
                        {
                            s_pathToCertificateFile = s_resourcesDirectory + "client" + ForestNET.Lib.IO.File.DIR + "cert-client-with-pw.pfx";
                            s_pathToCertificateFilePassword = "123456";
                        }
                        else if (p_e_asymmetricMode == AsymmetricModes.ThumbprintInStore)
                        {
                            s_certificateThumbprint = ThumbPrintClientCertificate;
                            e_certificateStoreName = System.Security.Cryptography.X509Certificates.StoreName.Root;
                            e_certificateStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser;
                        }
                        else if (p_e_asymmetricMode == AsymmetricModes.ThumbprintInStoreAndClientRemoteCertificateName)
                        {
                            s_certificateThumbprint = ThumbPrintClientCertificate;
                            e_certificateStoreName = System.Security.Cryptography.X509Certificates.StoreName.Root;
                            e_certificateStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser;
                            s_clientRemoteCertificateName = ThumbPrintClientCertificateName;
                        }

                        o_communicationConfig.SetSharedMemoryBidirectional(
                            new Dictionary<string, int> {
                                { p_s_biClientHost, p_i_biClientPort }
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
                                { p_s_biClientHost, p_i_biClientPort }
                            }
                        );
                    }

                    ForestNET.Lib.Net.Sock.Com.Communication o_communication = new(o_communicationConfig);
                    o_communication.Start();

                    int i_additionalTime = 25;

                    if (p_b_falseUDPtrueTCP)
                    { /* TCP */
                        if (p_b_highSecurity)
                        {
                            i_additionalTime *= 3;
                        }

                        if (p_b_asymmetricSecurity)
                        {
                            i_additionalTime *= 3;
                        }

                        if (p_i_marshallingDataLengthInBytes > 3)
                        {
                            i_additionalTime *= (p_i_marshallingDataLengthInBytes / 2);
                        }
                    }
                    else
                    { /* UDP */
                        if (p_b_highSecurity)
                        {
                            i_additionalTime *= 5;
                        }

                        if (p_b_udpWithAck)
                        {
                            i_additionalTime += 225;
                        }

                        i_additionalTime *= p_i_marshallingDataLengthInBytes;
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
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.LogException(o_exc);
                    Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
                }
            });

            /* WAIT SERVER + CLIENT */

            Task.WaitAll(o_server, o_client);

            /* CHECK LOG ENTRIES */

            int i_missingFieldsServer = 0;
            int i_missingFieldsClient = 0;

            for (int i = 0; i < a_serverResults.Count; i++)
            {
                if (!a_expectedResults[i].Equals(a_serverResults[i]))
                {
                    i_missingFieldsServer++;
                    //Console.WriteLine("server field result not equal expected result:\t" + a_serverResults[i] + "\t != \t" + a_expectedResults[i]);
                }
            }

            for (int i = 0; i < a_clientResults.Count; i++)
            {
                if (!a_expectedResults[i].Equals(a_clientResults[i]))
                {
                    i_missingFieldsClient++;
                    //Console.WriteLine("client field result not equal expected result:\t" + a_clientResults[i] + "\t != \t" + a_expectedResults[i]);
                }
            }

            if (i_missingFieldsServer > 10)
            {
                i_fails++;
            }
            else if (i_missingFieldsClient > 10)
            {
                i_fails++;
            }
            else
            {
                Assert.That(i_missingFieldsServer, Is.LessThanOrEqualTo(5), i_missingFieldsServer + " server fields not matching expected values");
                Assert.That(i_missingFieldsClient, Is.LessThanOrEqualTo(5), i_missingFieldsClient + " client fields not matching expected values");

                if (i_missingFieldsServer > 0)
                {
                    //Console.WriteLine("bi " + ((p_b_falseUDPtrueTCP) ? "tcp" : "udp") + ((p_b_udpWithAck) ? " - with ACK" : "") + ((p_b_smallObject) ? " - small" : "") + " server: " + i_missingFieldsServer);
                }
                else
                {
                    //Console.WriteLine("bi " + ((p_b_falseUDPtrueTCP) ? "tcp" : "udp") + ((p_b_udpWithAck) ? " - with ACK" : "") + ((p_b_smallObject) ? " - small" : "") + ((p_b_massChangeAtEnd) ? " - mass" : "") + " server: fine");
                }

                if (i_missingFieldsClient > 0)
                {
                    //Console.WriteLine("bi " + ((p_b_falseUDPtrueTCP) ? "tcp" : "udp") + ((p_b_udpWithAck) ? " - with ACK" : "") + ((p_b_smallObject) ? " - small" : "") + " client: " + i_missingFieldsClient);
                }
                else
                {
                    //Console.WriteLine("bi " + ((p_b_falseUDPtrueTCP) ? "tcp" : "udp") + ((p_b_udpWithAck) ? " - with ACK" : "") + ((p_b_smallObject) ? " - small" : "") + ((p_b_massChangeAtEnd) ? " - mass" : "") + " client: fine");
                }
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
