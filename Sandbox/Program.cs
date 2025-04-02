using ForestNET.Lib;
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
        Console.WriteLine("[2] test ConsoleProgressBar");
        Console.WriteLine("[3] test MemoryInfo");
        Console.WriteLine("[4] test Sorts");
        Console.WriteLine("[0] quit");

        Console.WriteLine("");

        i_input = ForestNET.Lib.Console.ConsoleInputInteger("Enter menu number[1-13;0]: ", "Invalid input.", "Please enter a value[1-13;0].") ?? 0;

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

        if ((i_input >= 1) && (i_input <= 13))
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