﻿using ForestNETLib.Core;
using Console = System.Console;

Console.WriteLine("Sandbox started . . . " + Environment.NewLine);

try
{
    string s_currentDirectory = (System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'")) + System.IO.Path.DirectorySeparatorChar;
    Console.WriteLine(s_currentDirectory + Environment.NewLine);

    int i_input = 0;

    do
    {
        Console.WriteLine("+++++++++++++++++++++++++++++++++");
        Console.WriteLine("+ forestNET Library Sandbox     +");
        Console.WriteLine("+++++++++++++++++++++++++++++++++");

        Console.WriteLine("");

        Console.WriteLine("[1] test Console");
        Console.WriteLine("[0] quit");

        Console.WriteLine("");

        i_input = ForestNETLib.Core.Console.ConsoleInputInteger("Enter menu number[1-11;0]: ", "Invalid input.", "Please enter a value[1-11;0].") ?? 0;

        Console.WriteLine("");

        if (i_input == 1)
        {
            Sandbox.Tests.ConsoleTest.TestConsole();
        }
        
        if ((i_input >= 1) && (i_input <= 12))
        {
            Console.WriteLine("");

            ForestNETLib.Core.Console.ConsoleInputString("Press any key to continue . . . ", true);

            Console.WriteLine("");
        }

        Console.WriteLine("");

    } while (i_input != 0);
}
catch (Exception)
{
    //Global.LogException(o_exc);

    Console.WriteLine("");

    ForestNETLib.Core.Console.ConsoleInputString("Press any key to continue . . . ", true);

    Console.WriteLine("");
}
finally
{
    Global.Instance.Dispose();
}

Console.WriteLine("Sandbox finished . . . ");

//Console.WriteLine("");
//ForestNETLib.Core.Console.ConsoleInputString("Press any key to continue . . . ", true);
//Console.WriteLine("");