namespace Sandbox.Tests.Net.Https
{
    public class CalculatorImpl
    {
        public static Object? Add(Object? p_o_inputMessage, ForestNET.Lib.Net.Https.Seed p_o_seed)
        {
            _ = p_o_seed.Config.Domain;

            if (p_o_inputMessage == null)
            {
                return null;
            }

            Calculator.Add o_add = (Calculator.Add)p_o_inputMessage;

            return new Calculator.AddResult()
            {
                result = o_add.param1 + o_add.param2
            };
        }

        public static Object? Subtract(Object? p_o_inputMessage, ForestNET.Lib.Net.Https.Seed p_o_seed)
        {
            _ = p_o_seed.Config.Domain;

            if (p_o_inputMessage == null)
            {
                return null;
            }

            Calculator.Subtract o_sub = (Calculator.Subtract)p_o_inputMessage;

            return new Calculator.SubtractResult()
            {
                result = o_sub.param1 - o_sub.param2
            };
        }

        public static Object? Multiply(Object? p_o_inputMessage, ForestNET.Lib.Net.Https.Seed p_o_seed)
        {
            _ = p_o_seed.Config.Domain;

            if (p_o_inputMessage == null)
            {
                return null;
            }

            Calculator.Multiply o_mul = (Calculator.Multiply)p_o_inputMessage;

            return new Calculator.MultiplyResult()
            {
                result = o_mul.param1 * o_mul.param2
            };
        }

        public static Object? Divide(Object? p_o_inputMessage, ForestNET.Lib.Net.Https.Seed p_o_seed)
        {
            _ = p_o_seed.Config.Domain;

            if (p_o_inputMessage == null)
            {
                return null;
            }

            Calculator.Divide o_div = (Calculator.Divide)p_o_inputMessage;

            return new Calculator.DivideResult()
            {
                result = o_div.param1 / o_div.param2
            };
        }
    }
}
