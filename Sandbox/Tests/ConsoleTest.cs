namespace Sandbox.Tests
{
    public class ConsoleTest
    {
        public static void TestConsole()
        {
            // Boolean
            Console.WriteLine(Environment.NewLine + Environment.NewLine + Environment.NewLine + "consoleInputBoolean - press enter - enter invalid value - enter valid value");
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputBoolean());
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputBoolean("consoleInputBoolean: "));

            // Character
            Console.WriteLine(Environment.NewLine + Environment.NewLine + Environment.NewLine + "consoleInputCharacter - press enter - enter invalid value - enter valid value");
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputCharacter());
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputCharacter("consoleInputCharacter: "));

            // Date
            Console.WriteLine(Environment.NewLine + Environment.NewLine + Environment.NewLine + "consoleInputDate - press enter - enter invalid value - enter valid value");
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputDate());
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputDate("consoleInputDate: "));
            try
            {
                ForestNETLib.Core.Console.ConsoleInputDate("consoleInputDate: ", "", "");
            }
            catch (ArgumentException o_exc)
            {
                Console.WriteLine("consoleInputDate - Expected ArgumentException: " + o_exc.Message);
            }
            try
            {
                ForestNETLib.Core.Console.ConsoleInputDate("consoleInputDate: ", "", "No Date.", "");
            }
            catch (ArgumentException o_exc)
            {
                Console.WriteLine("consoleInputDate - Expected ArgumentException: " + o_exc.Message);
            }
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputDate("consoleInputDate: ", "", "No Date."));
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputDate("consoleInputDate: ", "", "No Date.", "Please enter a value."));
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputDate("consoleInputDate [a-f0-9]*: ", "^[a-f0-9]*$", "No Date.", "Please enter a value."));

            // Time
            Console.WriteLine(Environment.NewLine + Environment.NewLine + Environment.NewLine + "consoleInputTime - press enter - enter invalid value - enter valid value");
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputTime());
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputTime("consoleInputTime: "));
            try
            {
                ForestNETLib.Core.Console.ConsoleInputTime("consoleInputTime: ", "", "");
            }
            catch (ArgumentException o_exc)
            {
                Console.WriteLine("consoleInputTime - Expected ArgumentException: " + o_exc.Message);
            }
            try
            {
                ForestNETLib.Core.Console.ConsoleInputTime("consoleInputTime: ", "", "No Time.", "");
            }
            catch (ArgumentException o_exc)
            {
                Console.WriteLine("consoleInputTime - Expected ArgumentException: " + o_exc.Message);
            }
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputTime("consoleInputTime: ", "", "No Time."));
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputTime("consoleInputTime: ", "", "No Time.", "Please enter a value."));
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputTime("consoleInputTime [a-f0-9]*: ", "^[a-f0-9]*$", "No Time.", "Please enter a value."));

            // DateTime
            Console.WriteLine(Environment.NewLine + Environment.NewLine + Environment.NewLine + "consoleInputDateTime - press enter - enter invalid value - enter valid value");
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputDateTime());
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputDateTime("consoleInputDateTime: "));
            try
            {
                ForestNETLib.Core.Console.ConsoleInputDateTime("consoleInputDateTime: ", "", "");
            }
            catch (ArgumentException o_exc)
            {
                Console.WriteLine("consoleInputDateTime - Expected ArgumentException: " + o_exc.Message);
            }
            try
            {
                ForestNETLib.Core.Console.ConsoleInputDateTime("consoleInputDateTime: ", "", "No DateTime.", "");
            }
            catch (ArgumentException o_exc)
            {
                Console.WriteLine("consoleInputDateTime - Expected ArgumentException: " + o_exc.Message);
            }
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputDateTime("consoleInputDateTime: ", "", "No DateTime."));
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputDateTime("consoleInputDateTime: ", "", "No DateTime.", "Please enter a value."));
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputDateTime("consoleInputDateTime [a-f0-9]*: ", "^[a-f0-9]*$", "No DateTime.", "Please enter a value."));

            // DateInterval
            Console.WriteLine(Environment.NewLine + Environment.NewLine + Environment.NewLine + "consoleInputDateInterval - press enter - enter invalid value - enter valid value");
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputDateInterval());
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputDateInterval("consoleInputDateInterval: "));
            try
            {
                ForestNETLib.Core.Console.ConsoleInputDateInterval("consoleInputDateInterval: ", "", "");
            }
            catch (ArgumentException o_exc)
            {
                Console.WriteLine("consoleInputDateInterval - Expected ArgumentException: " + o_exc.Message);
            }
            try
            {
                ForestNETLib.Core.Console.ConsoleInputDateInterval("consoleInputDateInterval: ", "", "No DateTime.", "");
            }
            catch (ArgumentException o_exc)
            {
                Console.WriteLine("consoleInputDateInterval - Expected ArgumentException: " + o_exc.Message);
            }
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputDateInterval("consoleInputDateInterval: ", "", "No DateInterval."));
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputDateInterval("consoleInputDateInterval: ", "", "No DateInterval.", "Please enter a value."));
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputDateInterval("consoleInputDateInterval [a-f0-9]*: ", "^[a-f0-9]*$", "No DateInterval.", "Please enter a value."));

            // Float
            Console.WriteLine(Environment.NewLine + Environment.NewLine + Environment.NewLine + "consoleInputFloat - press enter - enter invalid value - enter valid value");
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputFloat());
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputFloat("consoleInputFloat: "));
            try
            {
                ForestNETLib.Core.Console.ConsoleInputFloat("consoleInputFloat: ", "");
            }
            catch (ArgumentException o_exc)
            {
                Console.WriteLine("consoleInputFloat - Expected ArgumentException: " + o_exc.Message);
            }
            try
            {
                ForestNETLib.Core.Console.ConsoleInputFloat("consoleInputFloat: ", "No Float.", "");
            }
            catch (ArgumentException o_exc)
            {
                Console.WriteLine("consoleInputFloat - Expected ArgumentException: " + o_exc.Message);
            }
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputFloat("consoleInputFloat: ", "No Float."));
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputFloat("consoleInputFloat: ", "No Float.", "Please enter a value."));

            // Double
            Console.WriteLine(Environment.NewLine + Environment.NewLine + Environment.NewLine + "consoleInputDouble - press enter - enter invalid value - enter valid value");
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputDouble());
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputDouble("consoleInputDouble: "));
            try
            {
                ForestNETLib.Core.Console.ConsoleInputDouble("consoleInputDouble: ", "");
            }
            catch (ArgumentException o_exc)
            {
                Console.WriteLine("consoleInputDouble - Expected ArgumentException: " + o_exc.Message);
            }
            try
            {
                ForestNETLib.Core.Console.ConsoleInputDouble("consoleInputDouble: ", "No Double.", "");
            }
            catch (ArgumentException o_exc)
            {
                Console.WriteLine("consoleInputDouble - Expected ArgumentException: " + o_exc.Message);
            }
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputDouble("consoleInputDouble: ", "No Double."));
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputDouble("consoleInputDouble: ", "No Double.", "Please enter a value."));

            // Short    	
            Console.WriteLine(Environment.NewLine + Environment.NewLine + Environment.NewLine + "consoleInputShort - press enter - enter invalid value - enter valid value");
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputShort());
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputShort("consoleInputShort: "));
            try
            {
                ForestNETLib.Core.Console.ConsoleInputShort("consoleInputShort: ", "");
            }
            catch (ArgumentException o_exc)
            {
                Console.WriteLine("consoleInputShort - Expected ArgumentException: " + o_exc.Message);
            }
            try
            {
                ForestNETLib.Core.Console.ConsoleInputShort("consoleInputShort: ", "No Short.", "");
            }
            catch (ArgumentException o_exc)
            {
                Console.WriteLine("consoleInputShort - Expected ArgumentException: " + o_exc.Message);
            }
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputShort("consoleInputShort: ", "No Short."));
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputShort("consoleInputShort: ", "No Short.", "Please enter a value."));

            // Integer
            Console.WriteLine(Environment.NewLine + Environment.NewLine + Environment.NewLine + "consoleInputInteger - press enter - enter invalid value - enter valid value");
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputInteger());
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputInteger("consoleInputInteger: "));
            try
            {
                ForestNETLib.Core.Console.ConsoleInputInteger("consoleInputInteger: ", "");
            }
            catch (ArgumentException o_exc)
            {
                Console.WriteLine("consoleInputInteger - Expected ArgumentException: " + o_exc.Message);
            }
            try
            {
                ForestNETLib.Core.Console.ConsoleInputInteger("consoleInputInteger: ", "No Integer.", "");
            }
            catch (ArgumentException o_exc)
            {
                Console.WriteLine("consoleInputInteger - Expected ArgumentException: " + o_exc.Message);
            }
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputInteger("consoleInputInteger: ", "No Integer."));
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputInteger("consoleInputInteger: ", "No Integer.", "Please enter a value."));

            // Long
            Console.WriteLine(Environment.NewLine + Environment.NewLine + Environment.NewLine + "consoleInputLong - press enter - enter invalid value - enter valid value");
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputLong());
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputLong("consoleInputLong: "));
            try
            {
                ForestNETLib.Core.Console.ConsoleInputLong("consoleInputLong: ", "");
            }
            catch (ArgumentException o_exc)
            {
                Console.WriteLine("consoleInputLong - Expected ArgumentException: " + o_exc.Message);
            }
            try
            {
                ForestNETLib.Core.Console.ConsoleInputLong("consoleInputLong: ", "No Long.", "");
            }
            catch (ArgumentException o_exc)
            {
                Console.WriteLine("consoleInputLong - Expected ArgumentException: " + o_exc.Message);
            }
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputLong("consoleInputLong: ", "No Long."));
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputLong("consoleInputLong: ", "No Long.", "Please enter a value."));

            // Numeric String
            Console.WriteLine(Environment.NewLine + Environment.NewLine + Environment.NewLine + "consoleInputNumericString - press enter - enter invalid value - enter valid value");
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputNumericString());
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputNumericString("consoleInputNumericString: "));
            try
            {
                ForestNETLib.Core.Console.ConsoleInputNumericString("consoleInputNumericString: ", "");
            }
            catch (ArgumentException o_exc)
            {
                Console.WriteLine("consoleInputNumericString - Expected ArgumentException: " + o_exc.Message);
            }
            try
            {
                ForestNETLib.Core.Console.ConsoleInputNumericString("consoleInputNumericString: ", "No NumericString.", "");
            }
            catch (ArgumentException o_exc)
            {
                Console.WriteLine("consoleInputNumericString - Expected ArgumentException: " + o_exc.Message);
            }
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputNumericString("consoleInputNumericString: ", "No NumericString."));
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputNumericString("consoleInputNumericString: ", "No NumericString.", "Please enter a value."));

            // String
            Console.WriteLine(Environment.NewLine + Environment.NewLine + Environment.NewLine + "consoleInputString - press enter - enter invalid value - enter valid value");
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputString());
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputString("consoleInputString: "));
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputString(true));
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputString("consoleInputString: ", false));
            try
            {
                ForestNETLib.Core.Console.ConsoleInputString("consoleInputString: ", false, "");
            }
            catch (ArgumentException o_exc)
            {
                Console.WriteLine("consoleInputString - Expected ArgumentException: " + o_exc.Message);
            }
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputString("consoleInputString: ", false, "Please enter a value."));

            // String Password
            Console.WriteLine(Environment.NewLine + Environment.NewLine + Environment.NewLine + "consoleInputPassword - press enter - enter invalid value - enter valid value");
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputPassword());
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputPassword("consoleInputPassword: "));
            try
            {
                ForestNETLib.Core.Console.ConsoleInputPassword("consoleInputPassword: ", "");
            }
            catch (ArgumentException o_exc)
            {
                Console.WriteLine("consoleInputPassword - Expected ArgumentException: " + o_exc.Message);
            }
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputPassword("consoleInputPassword: ", "Please enter a value."));

            // Regex
            Console.WriteLine(Environment.NewLine + Environment.NewLine + Environment.NewLine + "consoleInputRegex [a-f0-9]* - press enter - enter invalid value - enter valid value");
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputRegex("consoleInputRegex [a-f0-9]*: ", "^[a-f0-9]*$"));
            try
            {
                ForestNETLib.Core.Console.ConsoleInputRegex("consoleInputRegex: ", "");
            }
            catch (ArgumentException o_exc)
            {
                Console.WriteLine("consoleInputRegex - Expected ArgumentException: " + o_exc.Message);
            }
            try
            {
                ForestNETLib.Core.Console.ConsoleInputRegex("consoleInputRegex [a-f0-9]*: ", "^[a-f0-9]*$", "");
            }
            catch (ArgumentException o_exc)
            {
                Console.WriteLine("consoleInputRegex - Expected ArgumentException: " + o_exc.Message);
            }
            try
            {
                ForestNETLib.Core.Console.ConsoleInputRegex("consoleInputRegex [a-f0-9]*: ", "^[a-f0-9]*$", "No Regex.", "");
            }
            catch (ArgumentException o_exc)
            {
                Console.WriteLine("consoleInputRegex - Expected ArgumentException: " + o_exc.Message);
            }
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputRegex("consoleInputRegex [a-f0-9]*: ", "^[a-f0-9]*$", "Does not match criteria."));
            Console.WriteLine(ForestNETLib.Core.Console.ConsoleInputRegex("consoleInputRegex [a-f0-9]*: ", "^[a-f0-9]*$", "Does not match criteria.", "Please enter a value."));
        }
    }
}
