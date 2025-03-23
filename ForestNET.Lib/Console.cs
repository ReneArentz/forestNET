namespace ForestNET.Lib
{
    /// <summary>
    /// Collection of static methods to support and handle user input with console.
    /// </summary>
    public class Console
    {
        /// <summary>
        /// get string line from console input
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a string line in console</param>
        /// <returns>string line</returns>
        /// <exception cref="ArgumentException">Missing value for input error message</exception>
        private static string? DoInput(string p_s_caption)
        {
            return DoInput(p_s_caption, false);
        }

        /// <summary>
        /// get string line from console input
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a string line in console</param>
        /// <param name="p_b_acceptEmptyString">enter an empty string will not return an error message</param>
        /// <returns>string line</returns>
        /// <exception cref="ArgumentException">Missing value for input error message</exception>
        private static string? DoInput(string p_s_caption, bool p_b_acceptEmptyString)
        {
            return DoInput(p_s_caption, p_b_acceptEmptyString, "Please enter a value.");
        }

        /// <summary>
        /// get string line from console input
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a string line in console</param>
        /// <param name="p_b_acceptEmptyString">enter an empty string will not return an error message</param>
        /// <param name="p_s_inputErrorMessage">overwrite standard english error message</param>
        /// <returns>string line</returns>
        /// <exception cref="ArgumentException">Missing value for input error message</exception>
        private static string? DoInput(string p_s_caption, bool p_b_acceptEmptyString, string p_s_inputErrorMessage)
        {
            if (Helper.IsStringEmpty(p_s_inputErrorMessage))
            {
                throw new ArgumentException("Please specify an input error message.");
            }

            string? s_foo;
            bool b_loop = false;

            do
            {
                /* print caption text before user can enter a string line in console */
                System.Console.Write(p_s_caption);

                /* read string line from console input */
                s_foo = System.Console.ReadLine();

                /* accept empty string flag */
                if (!p_b_acceptEmptyString)
                {
                    /* if string line input is empty, print error message */
                    if (Helper.IsStringEmpty(s_foo))
                    {
                        b_loop = true;
                        System.Console.Write(p_s_inputErrorMessage + Environment.NewLine);
                    }
                    else
                    {
                        /* exit input loop */
                        b_loop = false;
                    }
                }
            } while (b_loop);

            return s_foo;
        }

        /// <summary>
        /// get password line from console input, using '*' as console input mask
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a password string in console</param>
        /// <returns>password string</returns>
        /// <exception cref="ArgumentException">Missing value for input error message</exception>
        private static string? DoInputPassword(string p_s_caption)
        {
            return DoInputPassword(p_s_caption, "Please enter a password.");
        }

        /// <summary>
        /// get password line from console input, using '*' as console input mask
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a password string in console</param>
        /// <param name="p_s_inputErrorMessage">overwrite standard english error message</param>
        /// <returns>password string</returns>
        /// <exception cref="ArgumentException">Missing value for input error message</exception>
        private static string? DoInputPassword(string p_s_caption, string p_s_inputErrorMessage)
        {
            return DoInputPassword(p_s_caption, p_s_inputErrorMessage, '*');
        }

        /// <summary>
        /// get password line from console input
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a password string in console</param>
        /// <param name="p_s_inputErrorMessage">overwrite standard english error message</param>
        /// <param name="p_c_mask">mask character which will be used to mask user console input</param>
        /// <returns>password string</returns>
        /// <exception cref="ArgumentException">Missing value for input error message</exception>
        private static string? DoInputPassword(string p_s_caption, string p_s_inputErrorMessage, char p_c_mask)
        {
            if (Helper.IsStringEmpty(p_s_inputErrorMessage))
            {
                throw new ArgumentException("Please specify an input error message.");
            }

            string? s_foo = "";
            bool b_loop = true;

            while (b_loop)
            {
                /* print caption text before user can enter a password string in console */
                System.Console.Write(p_s_caption);

                try
                {
                    ConsoleKey o_key;

                    do
                    {
                        ConsoleKeyInfo o_consoleKeyInfo = System.Console.ReadKey(intercept: true);
                        o_key = o_consoleKeyInfo.Key;

                        if (o_key == ConsoleKey.Backspace && s_foo.Length > 0)
                        {
                            System.Console.Write("\b \b");
                            s_foo = s_foo.Substring(0, s_foo.Length - 1);
                        }
                        else if (!char.IsControl(o_consoleKeyInfo.KeyChar))
                        {
                            System.Console.Write(p_c_mask);
                            s_foo += o_consoleKeyInfo.KeyChar;
                        }
                    } while (o_key != ConsoleKey.Enter);
                }
                catch (Exception)
                {
                    /* if exception catched, keep string line empty */
                    s_foo = "";
                    /* exit input loop */
                    break;
                }
                finally
                {
                    System.Console.WriteLine();
                }

                /* if string line input is empty, print error message */
                if (Helper.IsStringEmpty(s_foo))
                {
                    System.Console.WriteLine(p_s_inputErrorMessage);
                    b_loop = true;
                }
                else
                {
                    b_loop = false;
                }
            }

            return s_foo;
        }

        /// <summary>
        /// get console input string value
        /// </summary>
        /// <returns>string value</returns>
        public static string? ConsoleInputString()
        {
            return ConsoleInputString("");
        }

        /// <summary>
        /// get console input string value
        /// </summary>
        /// <param name="p_b_acceptEmptyString">enter an empty string will not return an error message</param>
        /// <returns>string value</returns>
        public static string? ConsoleInputString(bool p_b_acceptEmptyString)
        {
            return ConsoleInputString("", p_b_acceptEmptyString);
        }

        /// <summary>
        /// get console input string value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a input string value</param>
        /// <returns>string value</returns>
        public static string? ConsoleInputString(string p_s_caption)
        {
            return DoInput(p_s_caption);
        }

        /// <summary>
        /// get console input string value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a input string value</param>
        /// <param name="p_b_acceptEmptyString">enter an empty string will not return an error message</param>
        /// <returns>string value</returns>
        public static string? ConsoleInputString(string p_s_caption, bool p_b_acceptEmptyString)
        {
            return DoInput(p_s_caption, p_b_acceptEmptyString);
        }

        /// <summary>
        /// get console input string value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a input string value</param>
        /// <param name="p_b_acceptEmptyString">enter an empty string will not return an error message</param>
        /// <param name="p_s_inputErrorMessage">overwrite standard english input error message</param>
        /// <returns>string value</returns>
        /// <exception cref="ArgumentException">Missing value for input error message</exception>
        public static string? ConsoleInputString(string p_s_caption, bool p_b_acceptEmptyString, string p_s_inputErrorMessage)
        {
            return DoInput(p_s_caption, p_b_acceptEmptyString, p_s_inputErrorMessage);
        }

        /// <summary>
        /// get console input password string value
        /// </summary>
        /// <returns>password string value</returns>
        public static string? ConsoleInputPassword()
        {
            return ConsoleInputPassword("");
        }

        /// <summary>
        /// get console input password string value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a input password string value</param>
        /// <returns>password string value</returns>
        public static string? ConsoleInputPassword(string p_s_caption)
        {
            return DoInputPassword(p_s_caption);
        }

        /// <summary>
        /// get console input password string value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a input password string value</param>
        /// <param name="p_s_inputErrorMessage">overwrite standard english input error message</param>
        /// <returns>password string value</returns>
        /// <exception cref="ArgumentException">Missing value for input error message</exception>
        public static string? ConsoleInputPassword(string p_s_caption, string p_s_inputErrorMessage)
        {
            return DoInputPassword(p_s_caption, p_s_inputErrorMessage);
        }

        /// <summary>
        /// get console input password string value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a input password string value</param>
        /// <param name="p_s_inputErrorMessage">overwrite standard english input error message</param>
        /// <param name="p_c_mask">mask character which will be used to mask user console input</param>
        /// <returns>password string value</returns>
        /// <exception cref="ArgumentException">Missing value for input error message</exception>
        public static string? ConsoleInputPassword(string p_s_caption, string p_s_inputErrorMessage, char p_c_mask)
        {
            return DoInputPassword(p_s_caption, p_s_inputErrorMessage, p_c_mask);
        }

        /// <summary>
        /// get console input character value
        /// </summary>
        /// <returns>character value</returns>
        public static char? ConsoleInputCharacter()
        {
            return ConsoleInputCharacter("");
        }

        /// <summary>
        /// get console input character value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a input character value</param>
        /// <returns>character value</returns>
        public static char? ConsoleInputCharacter(string p_s_caption)
        {
            /* use doInput to get input string line */
            string? s_foo = DoInput(p_s_caption);

            if (s_foo != null)
            {
                /* then just take the first character as input value */
                return s_foo.ToCharArray()[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// get console input numeric string value
        /// </summary>
        /// <returns>numeric string value</returns>
        public static string? ConsoleInputNumericString()
        {
            return ConsoleInputNumericString("");
        }

        /// <summary>
        /// get console input numeric string value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter an input numeric string value</param>
        /// <returns>numeric string value</returns>
        public static string? ConsoleInputNumericString(string p_s_caption)
        {
            return ConsoleInputNumericString(p_s_caption, "The entered value is not numeric[" + int.MinValue + " .. " + int.MaxValue + "].");
        }

        /// <summary>
        /// get console input numeric string value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter an input numeric string value</param>
        /// <param name="p_s_errorMessage">overwrite standard english error message</param>
        /// <returns>numeric string value</returns>
        /// <exception cref="ArgumentException">error message parameter is null or empty</exception>
        public static string? ConsoleInputNumericString(string p_s_caption, string p_s_errorMessage)
        {
            return ConsoleInputNumericString(p_s_caption, p_s_errorMessage, "Please enter a value.");
        }

        /// <summary>
        /// get console input numeric string value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter an input numeric string value</param>
        /// <param name="p_s_errorMessage">overwrite standard english error message</param>
        /// <param name="p_s_inputErrorMessage">overwrite standard english input error message</param>
        /// <returns>numeric string value</returns>
        /// <exception cref="ArgumentException">error message or input error message parameter is null or empty</exception>
        public static string? ConsoleInputNumericString(string p_s_caption, string p_s_errorMessage, string p_s_inputErrorMessage)
        {
            if (Helper.IsStringEmpty(p_s_errorMessage))
            {
                throw new ArgumentException("Please specify an error message.");
            }

            if (Helper.IsStringEmpty(p_s_inputErrorMessage))
            {
                throw new ArgumentException("Please specify an input error message.");
            }

            string? s_foo = DoInput(p_s_caption, false, p_s_inputErrorMessage);

            /* check if input string value is a valid integer value */
            while ((s_foo != null) && (!Helper.IsInteger(s_foo)))
            {
                System.Console.WriteLine(p_s_errorMessage);
                s_foo = DoInput(p_s_caption, false, p_s_inputErrorMessage);
            }

            return s_foo;
        }

        /// <summary>
        /// get console input value
        /// </summary>
        /// <returns>integer value</returns>
        public static int? ConsoleInputInteger()
        {
            return ConsoleInputInteger("");
        }

        /// <summary>
        /// get console input value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter an input value</param>
        /// <returns>integer value</returns>
        public static int? ConsoleInputInteger(string p_s_caption)
        {
            return Convert.ToInt32(ConsoleInputNumericString(p_s_caption));
        }

        /// <summary>
        /// get console input value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter an input value</param>
        /// <param name="p_s_errorMessage">overwrite standard english error message</param>
        /// <returns>integer value</returns>
        /// <exception cref="ArgumentException">error message parameter is null or empty</exception>
        public static int? ConsoleInputInteger(string p_s_caption, string p_s_errorMessage)
        {
            return Convert.ToInt32(ConsoleInputNumericString(p_s_caption, p_s_errorMessage));
        }

        /// <summary>
        /// get console input value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter an input value</param>
        /// <param name="p_s_errorMessage">overwrite standard english error message</param>
        /// <param name="p_s_inputErrorMessage">overwrite standard english input error message</param>
        /// <returns>integer value</returns>
        /// <exception cref="ArgumentException">error message or input error message parameter is null or empty</exception>
        public static int? ConsoleInputInteger(string p_s_caption, string p_s_errorMessage, string p_s_inputErrorMessage)
        {
            return Convert.ToInt32(ConsoleInputNumericString(p_s_caption, p_s_errorMessage, p_s_inputErrorMessage));
        }

        /// <summary>
        /// get console short value
        /// </summary>
        /// <returns>short value</returns>
        public static short? ConsoleInputShort()
        {
            return ConsoleInputShort("");
        }

        /// <summary>
        /// get console short value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a short value</param>
        /// <returns>short value</returns>
        public static short? ConsoleInputShort(string p_s_caption)
        {
            return ConsoleInputShort(p_s_caption, "The entered value is not of type short[" + short.MinValue + " .. " + short.MaxValue + "].");
        }

        /// <summary>
        /// get console short value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a short value</param>
        /// <param name="p_s_errorMessage">overwrite standard english error message</param>
        /// <returns>short value</returns>
        /// <exception cref="ArgumentException">error message parameter is null or empty</exception>
        public static short? ConsoleInputShort(string p_s_caption, string p_s_errorMessage)
        {
            return ConsoleInputShort(p_s_caption, p_s_errorMessage, "Please enter a value.");
        }

        /// <summary>
        /// get console short value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a short value</param>
        /// <param name="p_s_errorMessage">overwrite standard english error message</param>
        /// <param name="p_s_inputErrorMessage">overwrite standard english input error message</param>
        /// <returns>short value</returns>
        /// <exception cref="ArgumentException">error message or input error message parameter is null or empty</exception>
        public static short? ConsoleInputShort(string p_s_caption, string p_s_errorMessage, string p_s_inputErrorMessage)
        {
            if (Helper.IsStringEmpty(p_s_errorMessage))
            {
                throw new ArgumentException("Please specify an error message.");
            }

            if (Helper.IsStringEmpty(p_s_inputErrorMessage))
            {
                throw new ArgumentException("Please specify an input error message.");
            }

            string? s_foo = DoInput(p_s_caption, false, p_s_inputErrorMessage);

            /* check if input string value is a valid short value */
            while ((s_foo != null) && (!Helper.IsShort(s_foo)))
            {
                System.Console.WriteLine(p_s_errorMessage);
                s_foo = DoInput(p_s_caption, false, p_s_inputErrorMessage);
            }

            return Convert.ToInt16(s_foo);
        }

        /// <summary>
        /// get console long value
        /// </summary>
        /// <returns>long value</returns>
        public static long? ConsoleInputLong()
        {
            return ConsoleInputLong("");
        }

        /// <summary>
        /// get console long value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a long value</param>
        /// <returns>long value</returns>
        public static long? ConsoleInputLong(string p_s_caption)
        {
            return ConsoleInputLong(p_s_caption, "The entered value is not of type long[" + long.MinValue + " .. " + long.MaxValue + "].");
        }

        /// <summary>
        /// get console long value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a long value</param>
        /// <param name="p_s_errorMessage">overwrite standard english error message</param>
        /// <returns>long value</returns>
        /// <exception cref="ArgumentException">error message parameter is null or empty</exception>
        public static long? ConsoleInputLong(string p_s_caption, string p_s_errorMessage)
        {
            return ConsoleInputLong(p_s_caption, p_s_errorMessage, "Please enter a value.");
        }

        /// <summary>
        /// get console long value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a long value</param>
        /// <param name="p_s_errorMessage">overwrite standard english error message</param>
        /// <param name="p_s_inputErrorMessage">overwrite standard english input error message</param>
        /// <returns>long value</returns>
        /// <exception cref="ArgumentException">error message or input error message parameter is null or empty</exception>
        public static long? ConsoleInputLong(string p_s_caption, string p_s_errorMessage, string p_s_inputErrorMessage)
        {
            if (Helper.IsStringEmpty(p_s_errorMessage))
            {
                throw new ArgumentException("Please specify an error message.");
            }

            if (Helper.IsStringEmpty(p_s_inputErrorMessage))
            {
                throw new ArgumentException("Please specify an input error message.");
            }

            string? s_foo = DoInput(p_s_caption, false, p_s_inputErrorMessage);

            /* check if input string value is a valid long value */
            while ((s_foo != null) && (!Helper.IsLong(s_foo)))
            {
                System.Console.WriteLine(p_s_errorMessage);
                s_foo = DoInput(p_s_caption, false, p_s_inputErrorMessage);
            }

            return Convert.ToInt64(s_foo);
        }

        /// <summary>
        /// get console float value
        /// </summary>
        /// <returns>float value</returns>
        public static float? ConsoleInputFloat()
        {
            return ConsoleInputFloat("");
        }

        /// <summary>
        /// get console float value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a float value</param>
        /// <returns>long value</returns>
        public static float? ConsoleInputFloat(string p_s_caption)
        {
            return ConsoleInputFloat(p_s_caption, "The entered value is not of type float.");
        }

        /// <summary>
        /// get console float value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a float value</param>
        /// <param name="p_s_errorMessage">overwrite standard english error message</param>
        /// <returns>long value</returns>
        /// <exception cref="ArgumentException">error message parameter is null or empty</exception>
        public static float? ConsoleInputFloat(string p_s_caption, string p_s_errorMessage)
        {
            return ConsoleInputFloat(p_s_caption, p_s_errorMessage, "Please enter a value.");
        }

        /// <summary>
        /// get console float value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a float value</param>
        /// <param name="p_s_errorMessage">overwrite standard english error message</param>
        /// <param name="p_s_inputErrorMessage">overwrite standard english input error message</param>
        /// <returns>float value</returns>
        /// <exception cref="ArgumentException">error message or input error message parameter is null or empty</exception>
        public static float? ConsoleInputFloat(string p_s_caption, string p_s_errorMessage, string p_s_inputErrorMessage)
        {
            if (Helper.IsStringEmpty(p_s_errorMessage))
            {
                throw new ArgumentException("Please specify an error message.");
            }

            if (Helper.IsStringEmpty(p_s_inputErrorMessage))
            {
                throw new ArgumentException("Please specify an input error message.");
            }

            string? s_foo = DoInput(p_s_caption, false, p_s_inputErrorMessage);

            /* check if input string value is a valid float value */
            while ((s_foo != null) && (!Helper.IsFloat(s_foo)))
            {
                System.Console.WriteLine(p_s_errorMessage);
                s_foo = DoInput(p_s_caption, false, p_s_inputErrorMessage);
            }

            return Convert.ToSingle(s_foo?.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// get console double value
        /// </summary>
        /// <returns>double value</returns>
        public static double? ConsoleInputDouble()
        {
            return ConsoleInputDouble("");
        }

        /// <summary>
        /// get console double value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a double value</param>
        /// <returns>double value</returns>
        public static double? ConsoleInputDouble(string p_s_caption)
        {
            return ConsoleInputDouble(p_s_caption, "The entered value is not of type double.");
        }

        /// <summary>
        /// get console double value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a double value</param>
        /// <param name="p_s_errorMessage">overwrite standard english error message</param>
        /// <returns>double value</returns>
        /// <exception cref="ArgumentException">error message parameter is null or empty</exception>
        public static double? ConsoleInputDouble(string p_s_caption, string p_s_errorMessage)
        {
            return ConsoleInputDouble(p_s_caption, p_s_errorMessage, "Please enter a value.");
        }

        /// <summary>
        /// get console double value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a double value</param>
        /// <param name="p_s_errorMessage">overwrite standard english error message</param>
        /// <param name="p_s_inputErrorMessage">overwrite standard english input error message</param>
        /// <returns>double value</returns>
        /// <exception cref="ArgumentException">error message or input error message parameter is null or empty</exception>
        public static double? ConsoleInputDouble(string p_s_caption, string p_s_errorMessage, string p_s_inputErrorMessage)
        {
            if (Helper.IsStringEmpty(p_s_errorMessage))
            {
                throw new ArgumentException("Please specify an error message.");
            }

            if (Helper.IsStringEmpty(p_s_inputErrorMessage))
            {
                throw new ArgumentException("Please specify an input error message.");
            }

            string? s_foo = DoInput(p_s_caption, false, p_s_inputErrorMessage);

            /* check if input string value is a valid double value */
            while ((s_foo != null) && (!Helper.IsDouble(s_foo)))
            {
                System.Console.WriteLine(p_s_errorMessage);
                s_foo = DoInput(p_s_caption, false, p_s_inputErrorMessage);
            }

            return Convert.ToDouble(s_foo?.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// get console boolean value
        /// </summary>
        /// <returns>boolean value</returns>
        public static bool? ConsoleInputBoolean()
        {
            return ConsoleInputBoolean("");
        }

        /// <summary>
        /// get console boolean value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a boolean value</param>
        /// <returns>boolean value</returns>
        public static bool? ConsoleInputBoolean(string p_s_caption)
        {
            return Helper.IsBoolean(DoInput(p_s_caption) ?? "");
        }

        /// <summary>
        /// get console date string value
        /// </summary>
        /// <returns>date string value</returns>
        public static string? ConsoleInputDate()
        {
            return ConsoleInputDate("", "", "The entered value is not of type date.");
        }

        /// <summary>
        /// get console date string value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a date string value</param>
        /// <returns>date string value</returns>
        public static string? ConsoleInputDate(string p_s_caption)
        {
            return ConsoleInputDate(p_s_caption, "", "The entered value is not of type date.");
        }

        /// <summary>
        /// get console date string value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a date string value</param>
        /// <param name="p_s_errorMessage">overwrite standard english error message</param>
        /// <param name="p_s_regex">manual regex criteria</param>
        /// <returns>date string value</returns>
        /// <exception cref="ArgumentException">error message parameter is null or empty</exception>
        public static string? ConsoleInputDate(string p_s_caption, string p_s_regex, string p_s_errorMessage)
        {
            return ConsoleInputDate(p_s_caption, p_s_regex, p_s_errorMessage, "Please enter a value.");
        }

        /// <summary>
        /// get console date string value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a date string value</param>
        /// <param name="p_s_errorMessage">overwrite standard english error message</param>
        /// <param name="p_s_inputErrorMessage">overwrite standard english input error message</param>
        /// <param name="p_s_regex">manual regex criteria</param>
        /// <returns>date string value</returns>
        /// <exception cref="ArgumentException">error message or input error message parameter is null or empty</exception>
        public static string? ConsoleInputDate(string p_s_caption, string p_s_regex, string p_s_errorMessage, string p_s_inputErrorMessage)
        {
            if (Helper.IsStringEmpty(p_s_errorMessage))
            {
                throw new ArgumentException("Please specify an error message.");
            }

            if (Helper.IsStringEmpty(p_s_inputErrorMessage))
            {
                throw new ArgumentException("Please specify an input error message.");
            }

            string? s_foo = DoInput(p_s_caption, false, p_s_inputErrorMessage);

            if (Helper.IsStringEmpty(p_s_regex))
            { /* use standard date check */
                while ((s_foo != null) && (!Helper.IsDate(s_foo)))
                {
                    System.Console.WriteLine(p_s_errorMessage);
                    s_foo = DoInput(p_s_caption, false, p_s_inputErrorMessage);
                }
            }
            else
            {
                /* use manual regex criteria */
                while ((s_foo != null) && (!Helper.MatchesRegex(s_foo, p_s_regex)))
                {
                    System.Console.WriteLine(p_s_errorMessage);
                    s_foo = DoInput(p_s_caption, false, p_s_inputErrorMessage);
                }
            }

            return s_foo;
        }

        /// <summary>
        /// get console time string value
        /// </summary>
        /// <returns>time string value</returns>
        public static string? ConsoleInputTime()
        {
            return ConsoleInputTime("", "", "The entered value is not of type time.");
        }

        /// <summary>
        /// get console time string value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a time string value</param>
        /// <returns>time string value</returns>
        public static string? ConsoleInputTime(string p_s_caption)
        {
            return ConsoleInputTime(p_s_caption, "", "The entered value is not of type time.");
        }

        /// <summary>
        /// get console time string value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a time string value</param>
        /// <param name="p_s_errorMessage">overwrite standard english error message</param>
        /// <param name="p_s_regex">manual regex criteria</param>
        /// <returns>time string value</returns>
        /// <exception cref="ArgumentException">error message parameter is null or empty</exception>
        public static string? ConsoleInputTime(string p_s_caption, string p_s_regex, string p_s_errorMessage)
        {
            return ConsoleInputTime(p_s_caption, p_s_regex, p_s_errorMessage, "Please enter a value.");
        }

        /// <summary>
        /// get console time string value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a time string value</param>
        /// <param name="p_s_errorMessage">overwrite standard english error message</param>
        /// <param name="p_s_inputErrorMessage">overwrite standard english input error message</param>
        /// <param name="p_s_regex">manual regex criteria</param>
        /// <returns>time string value</returns>
        /// <exception cref="ArgumentException">error message or input error message parameter is null or empty</exception>
        public static string? ConsoleInputTime(string p_s_caption, string p_s_regex, string p_s_errorMessage, string p_s_inputErrorMessage)
        {
            if (Helper.IsStringEmpty(p_s_errorMessage))
            {
                throw new ArgumentException("Please specify an error message.");
            }

            if (Helper.IsStringEmpty(p_s_inputErrorMessage))
            {
                throw new ArgumentException("Please specify an input error message.");
            }

            string? s_foo = DoInput(p_s_caption, false, p_s_inputErrorMessage);

            if (Helper.IsStringEmpty(p_s_regex))
            { /* use standard time check */
                while ((s_foo != null) && (!Helper.IsTime(s_foo)))
                {
                    System.Console.WriteLine(p_s_errorMessage);
                    s_foo = DoInput(p_s_caption, false, p_s_inputErrorMessage);
                }
            }
            else
            {
                /* use manual regex criteria */
                while ((s_foo != null) && (!Helper.MatchesRegex(s_foo, p_s_regex)))
                {
                    System.Console.WriteLine(p_s_errorMessage);
                    s_foo = DoInput(p_s_caption, false, p_s_inputErrorMessage);
                }
            }

            return s_foo;
        }

        /// <summary>
        /// get console date time string value
        /// </summary>
        /// <returns>date time string value</returns>
        public static string? ConsoleInputDateTime()
        {
            return ConsoleInputDateTime("", "", "The entered value is not of type date time.");
        }

        /// <summary>
        /// get console date time string value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a date time string value</param>
        /// <returns>date time string value</returns>
        public static string? ConsoleInputDateTime(string p_s_caption)
        {
            return ConsoleInputDateTime(p_s_caption, "", "The entered value is not of type date time.");
        }

        /// <summary>
        /// get console date time string value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a date time string value</param>
        /// <param name="p_s_errorMessage">overwrite standard english error message</param>
        /// <param name="p_s_regex">manual regex criteria</param>
        /// <returns>date time string value</returns>
        /// <exception cref="ArgumentException">error message parameter is null or empty</exception>
        public static string? ConsoleInputDateTime(string p_s_caption, string p_s_regex, string p_s_errorMessage)
        {
            return ConsoleInputDateTime(p_s_caption, p_s_regex, p_s_errorMessage, "Please enter a value.");
        }

        /// <summary>
        /// get console date time string value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a date time string value</param>
        /// <param name="p_s_errorMessage">overwrite standard english error message</param>
        /// <param name="p_s_inputErrorMessage">overwrite standard english input error message</param>
        /// <param name="p_s_regex">manual regex criteria</param>
        /// <returns>date time string value</returns>
        /// <exception cref="ArgumentException">error message or input error message parameter is null or empty</exception>
        public static string? ConsoleInputDateTime(string p_s_caption, string p_s_regex, string p_s_errorMessage, string p_s_inputErrorMessage)
        {
            if (Helper.IsStringEmpty(p_s_errorMessage))
            {
                throw new ArgumentException("Please specify an error message.");
            }

            if (Helper.IsStringEmpty(p_s_inputErrorMessage))
            {
                throw new ArgumentException("Please specify an input error message.");
            }

            string? s_foo = DoInput(p_s_caption, false, p_s_inputErrorMessage);

            if (Helper.IsStringEmpty(p_s_regex))
            { /* use standard date time check */
                while ((s_foo != null) && (!Helper.IsDateTime(s_foo)))
                {
                    System.Console.WriteLine(p_s_errorMessage);
                    s_foo = DoInput(p_s_caption, false, p_s_inputErrorMessage);
                }
            }
            else
            {
                /* use manual regex criteria */
                while ((s_foo != null) && (!Helper.MatchesRegex(s_foo, p_s_regex)))
                {
                    System.Console.WriteLine(p_s_errorMessage);
                    s_foo = DoInput(p_s_caption, false, p_s_inputErrorMessage);
                }
            }

            return s_foo;
        }

        /// <summary>
        /// get console date interval string value
        /// </summary>
        /// <returns>date interval string value</returns>
        public static string? ConsoleInputDateInterval()
        {
            return ConsoleInputDateInterval("", "", "The entered value is not of type date interval.");
        }

        /// <summary>
        /// get console date interval string value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a date interval string value</param>
        /// <returns>date interval string value</returns>
        public static string? ConsoleInputDateInterval(string p_s_caption)
        {
            return ConsoleInputDateInterval(p_s_caption, "", "The entered value is not of type date interval.");
        }

        /// <summary>
        /// get console date interval string value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a date interval string value</param>
        /// <param name="p_s_errorMessage">overwrite standard english error message</param>
        /// <param name="p_s_regex">manual regex criteria</param>
        /// <returns>date interval string value</returns>
        /// <exception cref="ArgumentException">error message parameter is null or empty</exception>
        public static string? ConsoleInputDateInterval(string p_s_caption, string p_s_regex, string p_s_errorMessage)
        {
            return ConsoleInputDateInterval(p_s_caption, p_s_regex, p_s_errorMessage, "Please enter a value.");
        }

        /// <summary>
        /// get console date interval string value
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a date interval string value</param>
        /// <param name="p_s_errorMessage">overwrite standard english error message</param>
        /// <param name="p_s_inputErrorMessage">overwrite standard english input error message</param>
        /// <param name="p_s_regex">manual regex criteria</param>
        /// <returns>date interval string value</returns>
        /// <exception cref="ArgumentException">error message or input error message parameter is null or empty</exception>
        public static string? ConsoleInputDateInterval(string p_s_caption, string p_s_regex, string p_s_errorMessage, string p_s_inputErrorMessage)
        {
            if (Helper.IsStringEmpty(p_s_errorMessage))
            {
                throw new ArgumentException("Please specify an error message.");
            }

            if (Helper.IsStringEmpty(p_s_inputErrorMessage))
            {
                throw new ArgumentException("Please specify an input error message.");
            }

            string? s_foo = DoInput(p_s_caption, false, p_s_inputErrorMessage);

            if (Helper.IsStringEmpty(p_s_regex))
            { /* use standard date interval check */
                while ((s_foo != null) && (!Helper.IsDateInterval(s_foo)))
                {
                    System.Console.WriteLine(p_s_errorMessage);
                    s_foo = DoInput(p_s_caption, false, p_s_inputErrorMessage);
                }
            }
            else
            {
                /* use manual regex criteria */
                while ((s_foo != null) && (!Helper.MatchesRegex(s_foo, p_s_regex)))
                {
                    System.Console.WriteLine(p_s_errorMessage);
                    s_foo = DoInput(p_s_caption, false, p_s_inputErrorMessage);
                }
            }

            return s_foo;
        }

        /// <summary>
        /// get console string value with regex criteria
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a string value</param>
        /// <param name="p_s_regex">manual regex criteria</param>
        /// <returns>string value matching regex expression</returns>
        /// <exception cref="ArgumentException">regex parameter is null or empty</exception>
        public static string? ConsoleInputRegex(string p_s_caption, string p_s_regex)
        {
            return ConsoleInputRegex(p_s_caption, p_s_regex, "The entered value does not match the regular expression criteria.");
        }

        /// <summary>
        /// get console string value with regex criteria
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a string value</param>
        /// <param name="p_s_regex">manual regex criteria</param>
        /// <param name="p_s_errorMessage">overwrite standard english error message</param>
        /// <returns>string value matching regex expression</returns>
        /// <exception cref="ArgumentException">error message or regex parameter is null or empty</exception>
        public static string? ConsoleInputRegex(string p_s_caption, string p_s_regex, string p_s_errorMessage)
        {
            return ConsoleInputRegex(p_s_caption, p_s_regex, p_s_errorMessage, "Please enter a value.");
        }

        /// <summary>
        /// get console string value with regex criteria
        /// </summary>
        /// <param name="p_s_caption">caption text before user can enter a string value</param>
        /// <param name="p_s_regex">manual regex criteria</param>
        /// <param name="p_s_errorMessage">overwrite standard english error message</param>
        /// <param name="p_s_inputErrorMessage">overwrite standard english input error message</param>
        /// <returns>string value matching regex expression</returns>
        /// <exception cref="ArgumentException">error message or regex or input error message parameter is null or empty</exception>
        public static string? ConsoleInputRegex(string p_s_caption, string p_s_regex, string p_s_errorMessage, string p_s_inputErrorMessage)
        {
            if (Helper.IsStringEmpty(p_s_regex))
            {
                throw new ArgumentException("Please specify a regular expression criteria.");
            }

            if (Helper.IsStringEmpty(p_s_errorMessage))
            {
                throw new ArgumentException("Please specify an error message.");
            }

            if (Helper.IsStringEmpty(p_s_inputErrorMessage))
            {
                throw new ArgumentException("Please specify an input error message.");
            }

            string? s_foo = DoInput(p_s_caption, false, p_s_inputErrorMessage);

            while ((s_foo != null) && (!Helper.MatchesRegex(s_foo, p_s_regex)))
            {
                System.Console.WriteLine(p_s_errorMessage);
                s_foo = DoInput(p_s_caption, false, p_s_inputErrorMessage);

            }

            return s_foo;
        }
    }
}
