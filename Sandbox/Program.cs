﻿using ForestNET.Lib;
using Console = System.Console;

Console.WriteLine("Sandbox started . . . " + Environment.NewLine);

try
{
    string s_currentDirectory = (System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'")) + System.IO.Path.DirectorySeparatorChar;
    Console.WriteLine(s_currentDirectory + Environment.NewLine);

    ForestNET.Lib.Global.Instance.InternalLogControl = (byte)ForestNET.Lib.Log.Level.SEVERE + (byte)ForestNET.Lib.Log.Level.WARNING;

    //ForestNET.Lib.IO.File o_logFile = new(s_currentDirectory + ForestNET.Lib.IO.File.DIR + "test.log", !ForestNET.Lib.IO.File.Exists(s_currentDirectory + ForestNET.Lib.IO.File.DIR + "test.log"));
    //o_logFile.TruncateContent();
    //List<string> a_logLines =
    //[
    //    "UseFileLogging = true",
    //    "UseConsoleLogging = false",
    //    "MinimumLevel = WARNING",
    //    "FileLoggerConfigurationElement = Log,InternalLog;%LocalState;test.log"
    //];

    List<string> a_logLines =
    [
        "UseFileLogging = false",
        "UseConsoleLogging = true",
        "MinimumLevel = WARNING"
    ];

    ForestNET.Lib.Global.Instance.ResetLogFromLines(a_logLines);

    int i_input = 0;

    do
    {
        Console.WriteLine("+++++++++++++++++++++++++++++++++");
        Console.WriteLine("+ forestNET Library Sandbox     +");
        Console.WriteLine("+++++++++++++++++++++++++++++++++");

        Console.WriteLine("");

        Console.WriteLine("[1] test Console");
        Console.WriteLine("[2] test ConsoleProgressBar");
        Console.WriteLine("[3] test MemoryInfo");
        Console.WriteLine("[4] test Sorts");
        Console.WriteLine("[5] test ZipProgressBar");
        Console.WriteLine("[6] test AI");
        Console.WriteLine("[7] test WebRequestProgressBar");
        Console.WriteLine("[8] test FTPS");
        Console.WriteLine("[9] test SFTP");
        Console.WriteLine("[10] test Mail");
        Console.WriteLine("[11] test Net");
        Console.WriteLine("[12] test TinyHttps");
        Console.WriteLine("[13] list active network interfaces");
        Console.WriteLine("[14] net chat lobby");
        Console.WriteLine("[0] quit");

        Console.WriteLine("");

        i_input = ForestNET.Lib.Console.ConsoleInputInteger("Enter menu number[1-14;0]: ", "Invalid input.", "Please enter a value[1-14;0].") ?? 0;

        Console.WriteLine("");

        if (i_input == 1)
        {
            Sandbox.Tests.ConsoleTest.TestConsole();
        }
        else if (i_input == 2)
        {
            Sandbox.Tests.ConsoleProgressBarTest.TestConsoleProgressBar();
        }
        else if (i_input == 3)
        {
            Sandbox.Tests.MemoryInfoTest.TestMemoryInfo();
        }
        else if (i_input == 4)
        {
            Sandbox.Tests.SortsTest.TestSorts();
        }
        else if (i_input == 5)
        {
            Sandbox.Tests.ZipTest.TestZipProgressBar();
        }
        else if (i_input == 6)
        {
            Sandbox.Tests.AI.AITest.TestAIMenu(s_currentDirectory);
        }
        else if (i_input == 7)
        {
            Sandbox.Tests.Net.Request.WebRequestTest.TestWebRequest("https://corretto.aws/downloads/latest/amazon-corretto-17-x64-windows-jdk.zip");
        }
        else if (i_input == 8)
        {
            Sandbox.Tests.Net.FTPS.FTPSTest.TestFTPS("172.28.234.246", 12221, "user", "user", "/");
        }
        else if (i_input == 9)
        {
            Sandbox.Tests.Net.SFTP.SFTPTest.TestSFTP("172.28.234.246", 2222, "user", "/");
        }
        else if (i_input == 10)
        {
            Sandbox.Tests.Net.Mail.MailTest.TestMail("172.18.2.75");
        }
        else if (i_input == 11)
        {
            Sandbox.Tests.Net.NetTest.TestNetMenu(s_currentDirectory);
        }
        else if (i_input == 12)
        {
            Sandbox.Tests.Net.Https.HttpsTest.TestTinyHttpsMenu(s_currentDirectory);
        }
        else if (i_input == 13)
        {
            Console.WriteLine("IPv4:");

            foreach (KeyValuePair<string, string> o_activeNetworkInterface in ForestNET.Lib.Helper.GetNetworkInterfacesIpv4())
            {
                Console.WriteLine(o_activeNetworkInterface.Key + "\t" + o_activeNetworkInterface.Value);
            }

            Console.WriteLine("");
            Console.WriteLine("IPv6:");

            foreach (KeyValuePair<string, string> o_activeNetworkInterface in ForestNET.Lib.Helper.GetNetworkInterfacesIpv6())
            {
                Console.WriteLine(o_activeNetworkInterface.Key + "\t" + o_activeNetworkInterface.Value);
            }
        }
        else if (i_input == 14)
        {
            Sandbox.Tests.Net.NetChatLobbyTest.TestNetChatLobbyMenu(s_currentDirectory);
        }

        if ((i_input >= 1) && (i_input <= 14))
        {
            Console.WriteLine("");

            ForestNET.Lib.Console.ConsoleInputString("Press any key to continue . . . ", true);

            Console.WriteLine("");
        }

        Console.WriteLine("");

    } while (i_input != 0);
}
catch (Exception o_exc)
{
    Global.LogException(o_exc);

    Console.WriteLine("");

    ForestNET.Lib.Console.ConsoleInputString("Press any key to continue . . . ", true);

    Console.WriteLine("");
}
finally
{
    Global.Instance.Dispose();
}

Console.WriteLine("Sandbox finished . . . ");

//Console.WriteLine("");
//ForestNET.Lib.Console.ConsoleInputString("Press any key to continue . . . ", true);
//Console.WriteLine("");