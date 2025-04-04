namespace ForestNET.Lib.IO
{
    /// <summary>
    /// Collection of standard methods to transpose strings to primitive types and vice versa
    /// </summary>
    public class StandardTransposeMethods
    {
        /// <summary>
        /// Creates empty value string
        /// </summary>
        /// <param name="p_s_emptyCharacter">character where empty value will be based on, e.g. ' ' or '0'</param>
        /// <param name="p_i_length">target length of empty value string</param>
        /// <returns>empty value string</returns>
        /// <exception cref="ArgumentException">parameter for empty character must be only '1' character long</exception>
        private static string EmptyValue(string p_s_emptyCharacter, int p_i_length)
        {
            /* check empty character parameter */
            if (p_s_emptyCharacter.Length != 1)
            {
                throw new ArgumentException("Parameter for empty character must be only '1' character long");
            }

            string s_foo = "";

            /* iterate until length has been reached */
            for (int i = 0; i < p_i_length; i++)
            {
                /* add character parameter to empty value string */
                s_foo += p_s_emptyCharacter[0];
            }

            return s_foo;
        }

        /* string */

        /// <summary>
        /// Transpose string value
        /// </summary>
        /// <param name="p_s_value">string value</param>
        /// <returns>string type</returns>
        /// <exception cref="InvalidCastException">cannot cast string</exception>
        public static Object? TransposeString(string? p_s_value)
        {
            /* return string value */
            return Convert.ToString(p_s_value);
        }

        /// <summary>
        /// Transpose object to string value
        /// </summary>
        /// <param name="p_o_value">object value</param>
        /// <param name="p_i_length">set length for string value</param>
        /// <returns>string value</returns>
        /// <exception cref="InvalidCastException">cannot cast parameter to string type</exception>
        public static string TransposeString(Object? p_o_value, int p_i_length)
        {
            /* if parameter is null, return empty value string */
            if (p_o_value == null)
            {
                return EmptyValue(" ", p_i_length);
            }

            /* return string with set length */
            return (p_o_value.ToString() ?? EmptyValue(" ", p_i_length)).Substring(0, p_i_length);
        }

        /* boolean */

        /// <summary>
        /// Transpose bool string value to bool value
        /// </summary>
        /// <param name="p_s_value">string value</param>
        /// <returns>bool type</returns>
        /// <exception cref="InvalidCastException">cannot cast string to bool type</exception>
        public static Object? TransposeBoolean(string? p_s_value)
        {
            /* recognize '1', 'true', 'y' and 'j' as bool true, otherwise we recognize false */
            if ((p_s_value != null) && ((p_s_value.Equals("1")) || (p_s_value.Equals("true")) || (p_s_value.Equals("y")) || (p_s_value.Equals("j"))))
            {
                p_s_value = "true";
            }
            else
            {
                p_s_value = "false";
            }

            /* return bool value */
            return Convert.ToBoolean(p_s_value);
        }

        /// <summary>
        /// Transpose bool to string value
        /// </summary>
        /// <param name="p_o_value">bool value</param>
        /// <param name="p_i_length">set length for string value</param>
        /// <returns>string value</returns>
        /// <exception cref="InvalidCastException">cannot cast parameter to bool type</exception>
        public static string TransposeBoolean(Object? p_o_value, int p_i_length)
        {
            /* handle bool with set string length of '1' */
            if (p_i_length == 1)
            {
                /* return string with set length */
                if ((p_o_value != null) && (Convert.ToBoolean(p_o_value)))
                {
                    return "1";
                }
                else
                {
                    return "0";
                }
            }
            else
            {
                return "0";
            }
        }

        public class Numbers
        {
            /* Byte */

            /// <summary>
            /// Transpose byte string value to byte type
            /// </summary>
            /// <param name="p_s_value">byte string value</param>
            /// <returns>byte type</returns>
            /// <exception cref="InvalidCastException">cannot cast string value to byte type</exception>
            public static Object? TransposeByte(string? p_s_value)
            {
                /* check byte value */
                if (!ForestNET.Lib.Helper.IsByte(p_s_value ?? "null"))
                {
                    throw new InvalidCastException("Value '" + p_s_value + "' cannot be cast to Byte");
                }

                /* return byte value */
                return Convert.ToByte(p_s_value);
            }

            /// <summary>
            /// Transpose byte value to byte string value
            /// </summary>
            /// <param name="p_o_value">byte value</param>
            /// <param name="p_i_length">set length for byte string value</param>
            /// <returns>byte string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to byte type</exception>
            public static string TransposeByte(Object? p_o_value, int p_i_length)
            {
                /* check if parameter is a byte */
                if (p_o_value is Byte)
                {
                    /* create our format string */
                    string s_format = "{0:"; for (int i = 0; i < p_i_length; i++) s_format += "0"; s_format += "}";
                    /* return byte string with set length */
                    return String.Format(s_format, p_o_value, System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    throw new InvalidCastException("Parameter '" + p_o_value + "' is not a Byte");
                }
            }

            /* SByte */

            /// <summary>
            /// Transpose signed byte string value to byte type
            /// </summary>
            /// <param name="p_s_value">signed byte string value</param>
            /// <returns>signed byte type</returns>
            /// <exception cref="InvalidCastException">cannot cast string value to signed byte type</exception>
            public static Object? TransposeSignedByte(string? p_s_value)
            {
                /* check signed byte value */
                if (!ForestNET.Lib.Helper.IsSByte(p_s_value ?? "null"))
                {
                    throw new InvalidCastException("Value '" + p_s_value + "' cannot be cast to SByte");
                }

                /* return signed byte value */
                return Convert.ToSByte(p_s_value);
            }

            /// <summary>
            /// Transpose signed byte value to signed byte string value
            /// </summary>
            /// <param name="p_o_value">signed byte value</param>
            /// <param name="p_i_length">set length for signed byte string value</param>
            /// <returns>signed byte string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to signed byte type</exception>
            public static string TransposeSignedByte(Object? p_o_value, int p_i_length)
            {
                /* check if parameter is a sbyte */
                if (p_o_value is SByte)
                {
                    /* create our format string */
                    string s_format = "{0:"; for (int i = 0; i < p_i_length; i++) s_format += "0"; s_format += "}";
                    /* return signed byte string with set length */
                    return String.Format(s_format, p_o_value, System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    throw new InvalidCastException("Parameter '" + p_o_value + "' is not a SByte");
                }
            }

            /// <summary>
            /// Transpose signed byte string value with sign to signed byte type
            /// </summary>
            /// <param name="p_s_value">signed byte string value with sign</param>
            /// <returns>signed byte type</returns>
            /// <exception cref="InvalidCastException">cannot cast string value to signed byte type</exception>
            public static Object? TransposeSignedByteWithSign(string? p_s_value)
            {
                sbyte by_multiply = (sbyte)1;

                p_s_value ??= "null";

                /* check sign */
                if (p_s_value.StartsWith("+"))
                {
                    /* remove '+' sign */
                    p_s_value = p_s_value.Substring(1);
                }
                else if (p_s_value.StartsWith("-"))
                {
                    /* remove '-' sign */
                    p_s_value = p_s_value.Substring(1);
                    /* set multiply sign to '-1' */
                    by_multiply = (sbyte)-1;
                }

                /* check byte value */
                if (!ForestNET.Lib.Helper.IsSByte(p_s_value))
                {
                    throw new InvalidCastException("Value '" + p_s_value + "' cannot be cast to SByte");
                }

                /* return byte value with sign */
                return (sbyte)(Convert.ToSByte(p_s_value) * by_multiply);
            }

            /// <summary>
            /// Transpose signed byte value to signed byte string value with sign
            /// </summary>
            /// <param name="p_o_value">signed byte value</param>
            /// <param name="p_i_length">set length for signed byte string value</param>
            /// <returns>signed byte string value with sign</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to signed byte type</exception>
            public static string TransposeSignedByteWithSign(Object? p_o_value, int p_i_length)
            {
                /* check if parameter is a byte */
                if (p_o_value is SByte)
                {
                    /* create our format string, length - 1 because of sign at the start */
                    string s_format = "{0:"; for (int i = 0; i < p_i_length - 1; i++) s_format += "0"; s_format += "}";

                    /* check if byte value is negative */
                    if ((sbyte)p_o_value < 0)
                    {
                        /* return signed byte string with negative sign */
                        return "-" + String.Format(s_format, (sbyte)p_o_value * (sbyte)-1, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        /* return signed byte string with positive sign */
                        return "+" + String.Format(s_format, (sbyte)p_o_value, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                else
                {
                    throw new InvalidCastException("Parameter '" + p_o_value + "' is not a SByte");
                }
            }

            /* Short */

            /// <summary>
            /// Transpose short string value to short type
            /// </summary>
            /// <param name="p_s_value">short string value</param>
            /// <returns>short type</returns>
            /// <exception cref="InvalidCastException">cannot cast string value to short type</exception>
            public static Object? TransposeShort(string? p_s_value)
            {
                /* check short value */
                if (!ForestNET.Lib.Helper.IsShort(p_s_value ?? "null"))
                {
                    throw new InvalidCastException("Value '" + p_s_value + "' cannot be cast to Short");
                }

                /* return short value */
                return Convert.ToInt16(p_s_value);
            }

            /// <summary>
            /// Transpose short value to short string value
            /// </summary>
            /// <param name="p_o_value">short value</param>
            /// <param name="p_i_length">set length for short string value</param>
            /// <returns>short string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to short type</exception>
            public static string TransposeShort(Object? p_o_value, int p_i_length)
            {
                /* check if parameter is a short */
                if (p_o_value is Int16)
                {
                    /* create our format string */
                    string s_format = "{0:"; for (int i = 0; i < p_i_length; i++) s_format += "0"; s_format += "}";
                    /* return short string with set length */
                    return String.Format(s_format, p_o_value, System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    throw new InvalidCastException("Parameter '" + p_o_value + "' is not a Short");
                }
            }

            /// <summary>
            /// Transpose short string value with sign to short type
            /// </summary>
            /// <param name="p_s_value">short string value with sign</param>
            /// <returns>short type</returns>
            /// <exception cref="InvalidCastException">cannot cast string value to short type</exception>
            public static Object? TransposeShortWithSign(string? p_s_value)
            {
                short sh_multiply = (short)1;

                p_s_value ??= "null";

                /* check sign */
                if (p_s_value.StartsWith("+"))
                {
                    /* remove '+' sign */
                    p_s_value = p_s_value.Substring(1);
                }
                else if (p_s_value.StartsWith("-"))
                {
                    /* remove '-' sign */
                    p_s_value = p_s_value.Substring(1);
                    /* set multiply sign to '-1' */
                    sh_multiply = (short)-1;
                }

                /* check short value */
                if (!ForestNET.Lib.Helper.IsShort(p_s_value))
                {
                    throw new InvalidCastException("Value '" + p_s_value + "' cannot be cast to Short");
                }

                /* return short value with sign */
                return (short)(Convert.ToInt16(p_s_value) * sh_multiply);
            }

            /// <summary>
            /// Transpose short value to short string value with sign
            /// </summary>
            /// <param name="p_o_value">short value</param>
            /// <param name="p_i_length">set length for short string value</param>
            /// <returns>short string value with sign</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to short type</exception>
            public static string TransposeShortWithSign(Object? p_o_value, int p_i_length)
            {
                /* check if parameter is a short */
                if (p_o_value is Int16)
                {
                    /* create our format string, length - 1 because of sign at the start */
                    string s_format = "{0:"; for (int i = 0; i < p_i_length - 1; i++) s_format += "0"; s_format += "}";

                    /* check if short value is negative */
                    if ((short)p_o_value < 0)
                    {
                        /* return short string with negative sign and set length */
                        return "-" + String.Format(s_format, (short)p_o_value * (short)-1, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        /* return short string with positive sign and set length */
                        return "+" + String.Format(s_format, p_o_value, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                else
                {
                    throw new InvalidCastException("Parameter '" + p_o_value + "' is not a Short");
                }
            }

            /* Integer */

            /// <summary>
            /// Transpose integer string value to integer type
            /// </summary>
            /// <param name="p_s_value">integer string value</param>
            /// <returns>integer type</returns>
            /// <exception cref="InvalidCastException">cannot cast string value to integer type</exception>
            public static Object? TransposeInteger(string? p_s_value)
            {
                /* check integer value */
                if (!ForestNET.Lib.Helper.IsInteger(p_s_value ?? "null"))
                {
                    throw new InvalidCastException("Value '" + p_s_value + "' cannot be cast to Integer");
                }

                /* return integer value */
                return Convert.ToInt32(p_s_value);
            }

            /// <summary>
            /// Transpose integer value to integer string value
            /// </summary>
            /// <param name="p_o_value">integer value</param>
            /// <param name="p_i_length">set length for integer string value</param>
            /// <returns>integer string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to integer type</exception>
            public static string TransposeInteger(Object? p_o_value, int p_i_length)
            {
                /* check if parameter is an integer */
                if (p_o_value is Int32)
                {
                    /* create our format string */
                    string s_format = "{0:"; for (int i = 0; i < p_i_length; i++) s_format += "0"; s_format += "}";
                    /* return integer string with set length */
                    return String.Format(s_format, p_o_value, System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    throw new InvalidCastException("Parameter '" + p_o_value + "' is not an Integer");
                }
            }

            /// <summary>
            /// Transpose integer string value with sign to integer type
            /// </summary>
            /// <param name="p_s_value">integer string value with sign</param>
            /// <returns>integer type</returns>
            /// <exception cref="InvalidCastException">cannot cast string value to integer type</exception>
            public static Object? TransposeIntegerWithSign(string? p_s_value)
            {
                int i_multiply = 1;

                p_s_value ??= "null";

                /* check sign */
                if (p_s_value.StartsWith("+"))
                {
                    /* remove '+' sign */
                    p_s_value = p_s_value.Substring(1);
                }
                else if (p_s_value.StartsWith("-"))
                {
                    /* remove '-' sign */
                    p_s_value = p_s_value.Substring(1);
                    /* set multiply sign to '-1' */
                    i_multiply = -1;
                }

                /* check integer value */
                if (!ForestNET.Lib.Helper.IsInteger(p_s_value))
                {
                    throw new InvalidCastException("Value '" + p_s_value + "' cannot be cast to Integer");
                }

                /* return integer value with sign */
                return (Convert.ToInt32(p_s_value) * i_multiply);
            }

            /// <summary>
            /// Transpose integer value to integer string value with sign
            /// </summary>
            /// <param name="p_o_value">integer value</param>
            /// <param name="p_i_length">set length for integer string value</param>
            /// <returns>integer string value with sign</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to integer type</exception>
            public static string TransposeIntegerWithSign(Object? p_o_value, int p_i_length)
            {
                /* check if parameter is an integer */
                if (p_o_value is Int32)
                {
                    /* create our format string, length - 1 because of sign at the start */
                    string s_format = "{0:"; for (int i = 0; i < p_i_length - 1; i++) s_format += "0"; s_format += "}";

                    /* check if integer value is negative */
                    if ((int)p_o_value < 0)
                    {
                        /* return integer string with negative sign and set length */
                        return "-" + String.Format(s_format, (int)p_o_value * (int)-1, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        /* return integer string with positive sign and set length */
                        return "+" + String.Format(s_format, p_o_value, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                else
                {
                    throw new InvalidCastException("Parameter '" + p_o_value + "' is not an Integer");
                }
            }

            /* Long */

            /// <summary>
            /// Transpose long string value to long type
            /// </summary>
            /// <param name="p_s_value">long string value</param>
            /// <returns>long type</returns>
            /// <exception cref="InvalidCastException">cannot cast string value to long type</exception>
            public static Object? TransposeLong(string? p_s_value)
            {
                /* check long value */
                if (!ForestNET.Lib.Helper.IsLong(p_s_value ?? "null"))
                {
                    throw new InvalidCastException("Value '" + p_s_value + "' cannot be cast to Long");
                }

                /* return long value */
                return Convert.ToInt64(p_s_value);
            }

            /// <summary>
            /// Transpose long value to long string value
            /// </summary>
            /// <param name="p_o_value">long value</param>
            /// <param name="p_i_length">set length for long string value</param>
            /// <returns>long string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to long type</exception>
            public static string TransposeLong(Object? p_o_value, int p_i_length)
            {
                /* check if parameter is a long */
                if (p_o_value is Int64)
                {
                    /* create our format string */
                    string s_format = "{0:"; for (int i = 0; i < p_i_length; i++) s_format += "0"; s_format += "}";
                    /* return long string with set length */
                    return String.Format(s_format, p_o_value, System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    throw new InvalidCastException("Parameter '" + p_o_value + "' is not a Long");
                }
            }

            /// <summary>
            /// Transpose long string value with sign to long type
            /// </summary>
            /// <param name="p_s_value">long string value with sign</param>
            /// <returns>long type</returns>
            /// <exception cref="InvalidCastException">cannot cast string value to long type</exception>
            public static Object? TransposeLongWithSign(string? p_s_value)
            {
                long l_multiply = (long)1;

                p_s_value ??= "null";

                /* check sign */
                if (p_s_value.StartsWith("+"))
                {
                    /* remove '+' sign */
                    p_s_value = p_s_value.Substring(1);
                }
                else if (p_s_value.StartsWith("-"))
                {
                    /* remove '-' sign */
                    p_s_value = p_s_value.Substring(1);
                    /* set multiply sign to '-1' */
                    l_multiply = (long)-1;
                }

                /* check long value */
                if (!ForestNET.Lib.Helper.IsLong(p_s_value))
                {
                    throw new InvalidCastException("Value '" + p_s_value + "' cannot be cast to Long");
                }

                /* return long value with sign */
                return (Convert.ToInt64(p_s_value) * l_multiply);
            }

            /// <summary>
            /// Transpose long value to long string value with sign
            /// </summary>
            /// <param name="p_o_value">long value</param>
            /// <param name="p_i_length">set length for long string value</param>
            /// <returns>long string value with sign</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to long type</exception>
            public static string TransposeLongWithSign(Object? p_o_value, int p_i_length)
            {
                /* check if parameter is a long */
                if (p_o_value is Int64)
                {
                    /* create our format string, length - 1 because of sign at the start */
                    string s_format = "{0:"; for (int i = 0; i < p_i_length - 1; i++) s_format += "0"; s_format += "}";

                    /* check if long value is negative */
                    if ((long)p_o_value < 0)
                    {
                        /* return long string with negative sign and set length */
                        return "-" + String.Format(s_format, (long)p_o_value * (long)-1, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        /* return long string with positive sign and set length */
                        return "+" + String.Format(s_format, p_o_value, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                else
                {
                    throw new InvalidCastException("Parameter '" + p_o_value + "' is not a Long");
                }
            }

            /* Unsigned Short */

            /// <summary>
            /// Transpose unsigned short string value to short type
            /// </summary>
            /// <param name="p_s_value">unsigned short string value</param>
            /// <returns>unsigned short type</returns>
            /// <exception cref="InvalidCastException">cannot cast string value to unsigned short type</exception>
            public static Object? TransposeUnsignedShort(string? p_s_value)
            {
                /* check ushort value */
                if (!ForestNET.Lib.Helper.IsUShort(p_s_value ?? "null"))
                {
                    throw new InvalidCastException("Value '" + p_s_value + "' cannot be cast to UShort");
                }

                /* return ushort value */
                return Convert.ToUInt16(p_s_value);
            }

            /// <summary>
            /// Transpose unsigned short value to unsigned short string value
            /// </summary>
            /// <param name="p_o_value">unsigned short value</param>
            /// <param name="p_i_length">set length for unsigned short string value</param>
            /// <returns>unsigned short string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to unsigned short type</exception>
            public static string TransposeUnsignedShort(Object? p_o_value, int p_i_length)
            {
                /* check if parameter is a ushort */
                if (p_o_value is UInt16)
                {
                    /* create our format string */
                    string s_format = "{0:"; for (int i = 0; i < p_i_length; i++) s_format += "0"; s_format += "}";
                    /* return ushort string with set length */
                    return String.Format(s_format, p_o_value, System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    throw new InvalidCastException("Parameter '" + p_o_value + "' is not a UShort");
                }
            }

            /// <summary>
            /// Transpose unsigned short string value with sign to unsigned short type
            /// </summary>
            /// <param name="p_s_value">unsigned short string value with sign</param>
            /// <returns>unsigned short type</returns>
            /// <exception cref="InvalidCastException">cannot cast string value to unsigned short type</exception>
            public static Object? TransposeUnsignedShortWithSign(string? p_s_value)
            {
                /* check ushort value */
                if (!ForestNET.Lib.Helper.IsUShort(p_s_value ?? "null"))
                {
                    throw new InvalidCastException("Value '" + p_s_value + "' cannot be cast to UShort");
                }

                /* return ushort value with sign */
                return Convert.ToUInt16(p_s_value);
            }

            /// <summary>
            /// Transpose unsigned short value to unsigned short string value with sign
            /// </summary>
            /// <param name="p_o_value">unsigned short value</param>
            /// <param name="p_i_length">set length for unsigned short string value</param>
            /// <returns>unsigned short string value with sign</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to unsigned short type</exception>
            public static string TransposeUnsignedShortWithSign(Object? p_o_value, int p_i_length)
            {
                /* check if parameter is a ushort */
                if (p_o_value is UInt16)
                {
                    /* create our format string, length - 1 because of sign at the start */
                    string s_format = "{0:"; for (int i = 0; i < p_i_length - 1; i++) s_format += "0"; s_format += "}";

                    /* return ushort string with positive sign and set length */
                    return "+" + String.Format(s_format, p_o_value, System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    throw new InvalidCastException("Parameter '" + p_o_value + "' is not a UShort");
                }
            }

            /* Unsigned Integer */

            /// <summary>
            /// Transpose unsigned integer string value to unsigned integer type
            /// </summary>
            /// <param name="p_s_value">unsigned integer string value</param>
            /// <returns>unsigned integer type</returns>
            /// <exception cref="InvalidCastException">cannot cast string value to unsigned integer type</exception>
            public static Object? TransposeUnsignedInteger(string? p_s_value)
            {
                /* check uinteger value */
                if (!ForestNET.Lib.Helper.IsUInteger(p_s_value ?? "null"))
                {
                    throw new InvalidCastException("Value '" + p_s_value + "' cannot be cast to UInteger");
                }

                /* return uinteger value */
                return Convert.ToUInt32(p_s_value);
            }

            /// <summary>
            /// Transpose unsigned integer value to unsigned integer string value
            /// </summary>
            /// <param name="p_o_value">unsigned integer value</param>
            /// <param name="p_i_length">set length for unsigned integer string value</param>
            /// <returns>unsigned integer string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to unsigned integer type</exception>
            public static string TransposeUnsignedInteger(Object? p_o_value, int p_i_length)
            {
                /* check if parameter is an uinteger */
                if (p_o_value is UInt32)
                {
                    /* create our format string */
                    string s_format = "{0:"; for (int i = 0; i < p_i_length; i++) s_format += "0"; s_format += "}";
                    /* return uinteger string with set length */
                    return String.Format(s_format, p_o_value, System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    throw new InvalidCastException("Parameter '" + p_o_value + "' is not an UInteger");
                }
            }

            /// <summary>
            /// Transpose unsigned integer string value with sign to unsigned integer type
            /// </summary>
            /// <param name="p_s_value">unsigned integer string value with sign</param>
            /// <returns>unsigned integer type</returns>
            /// <exception cref="InvalidCastException">cannot cast string value to unsigned integer type</exception>
            public static Object? TransposeUnsignedIntegerWithSign(string? p_s_value)
            {
                /* check uinteger value */
                if (!ForestNET.Lib.Helper.IsUInteger(p_s_value ?? "null"))
                {
                    throw new InvalidCastException("Value '" + p_s_value + "' cannot be cast to UInteger");
                }

                /* return uinteger value with sign */
                return Convert.ToUInt32(p_s_value);
            }

            /// <summary>
            /// Transpose unsigned integer value to unsigned integer string value with sign
            /// </summary>
            /// <param name="p_o_value">unsigned integer value</param>
            /// <param name="p_i_length">set length for unsigned integer string value</param>
            /// <returns>unsigned integer string value with sign</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to unsigned integer type</exception>
            public static string TransposeUnsignedIntegerWithSign(Object? p_o_value, int p_i_length)
            {
                /* check if parameter is an uinteger */
                if (p_o_value is UInt32)
                {
                    /* create our format string, length - 1 because of sign at the start */
                    string s_format = "{0:"; for (int i = 0; i < p_i_length - 1; i++) s_format += "0"; s_format += "}";

                    /* return uinteger string with positive sign and set length */
                    return "+" + String.Format(s_format, p_o_value, System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    throw new InvalidCastException("Parameter '" + p_o_value + "' is not an UInteger");
                }
            }

            /* Unsigned Long */

            /// <summary>
            /// Transpose unsigned long string value to unsigned long type
            /// </summary>
            /// <param name="p_s_value">unsigned long string value</param>
            /// <returns>unsigned long type</returns>
            /// <exception cref="InvalidCastException">cannot cast string value to unsigned long type</exception>
            public static Object? TransposeUnsignedLong(string? p_s_value)
            {
                /* check ulong value */
                if (!ForestNET.Lib.Helper.IsULong(p_s_value ?? "null"))
                {
                    throw new InvalidCastException("Value '" + p_s_value + "' cannot be cast to ULong");
                }

                /* return ulong value */
                return Convert.ToUInt64(p_s_value);
            }

            /// <summary>
            /// Transpose unsigned long value to unsigned long string value
            /// </summary>
            /// <param name="p_o_value">unsigned long value</param>
            /// <param name="p_i_length">set length for unsigned long string value</param>
            /// <returns>unsigned long string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to unsigned long type</exception>
            public static string TransposeUnsignedLong(Object? p_o_value, int p_i_length)
            {
                /* check if parameter is a ulong */
                if (p_o_value is UInt64)
                {
                    /* create our format string */
                    string s_format = "{0:"; for (int i = 0; i < p_i_length; i++) s_format += "0"; s_format += "}";
                    /* return ulong string with set length */
                    return String.Format(s_format, p_o_value, System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    throw new InvalidCastException("Parameter '" + p_o_value + "' is not a ULong");
                }
            }

            /// <summary>
            /// Transpose unsigned long string value with sign to unsigned long type
            /// </summary>
            /// <param name="p_s_value">unsigned long string value with sign</param>
            /// <returns>unsigned long type</returns>
            /// <exception cref="InvalidCastException">cannot cast string value to unsigned long type</exception>
            public static Object? TransposeUnsignedLongWithSign(string? p_s_value)
            {
                /* check ulong value */
                if (!ForestNET.Lib.Helper.IsULong(p_s_value ?? "null"))
                {
                    throw new InvalidCastException("Value '" + p_s_value + "' cannot be cast to ULong");
                }

                /* return ulong value with sign */
                return Convert.ToUInt64(p_s_value);
            }

            /// <summary>
            /// Transpose unsigned long value to unsigned long string value with sign
            /// </summary>
            /// <param name="p_o_value">unsigned long value</param>
            /// <param name="p_i_length">set length for unsigned long string value</param>
            /// <returns>unsigned long string value with sign</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to unsigned long type</exception>
            public static string TransposeUnsignedLongWithSign(Object? p_o_value, int p_i_length)
            {
                /* check if parameter is a ulong */
                if (p_o_value is UInt64)
                {
                    /* create our format string, length - 1 because of sign at the start */
                    string s_format = "{0:"; for (int i = 0; i < p_i_length - 1; i++) s_format += "0"; s_format += "}";

                    /* return ulong string with positive sign and set length */
                    return "+" + String.Format(s_format, p_o_value, System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    throw new InvalidCastException("Parameter '" + p_o_value + "' is not a ULong");
                }
            }
        }

        /* System.DateTime */

        public class DateTime
        {
            /// <summary>
            /// Transpose any string value to date time type
            /// </summary>
            /// <param name="p_s_value">any string value</param>
            /// <param name="p_s_format">format string for date time parser</param>
            /// <returns>date time type</returns>
            /// <exception cref="InvalidCastException">cannot cast string value to date time type or invalid format parameter</exception>
            private static System.DateTime? TransposeDateTime_any(string? p_s_value)
            {
                if (p_s_value == null)
                {
                    return null;
                }

                if (p_s_value.Equals("00000000000000")) /* recognize empty date time */
                {
                    return null;
                }
                else if (p_s_value.Equals("00000000")) /* recognize empty date */
                {
                    return null;
                }
                else
                {
                    return ForestNET.Lib.Helper.FromDateTimeString(p_s_value);
                }
            }

            /// <summary>
            /// Transpose date time value to any string value
            /// </summary>
            /// <param name="p_o_value">date time value</param>
            /// <param name="p_i_length">set length for any string value</param>
            /// <param name="p_s_format">format string for date time parser</param>
            /// <returns>any string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to date time type or invalid format parameter</exception>
            private static string TransposeDateTime_any(Object? p_o_value, int p_i_length, string p_s_format)
            {
                /* if parameter is null, return empty value string */
                if (p_o_value == null)
                {
                    /* use zero as parameter character for these specific formats */
                    if ((p_s_format == "yyyyMMddHHmmssfff") || (p_s_format == "yyyyMMddHHmmss") || (p_s_format == "yyyyMMdd") || (p_s_format == "yyyyMMdd") || (p_s_format == "HHmmss") || (p_s_format == "HHmm"))
                    {
                        return EmptyValue("0", p_i_length);
                    }
                    else
                    {
                        return EmptyValue(" ", p_i_length);
                    }
                }

                /* check if parameter is a DateTime */
                if (p_o_value is System.DateTime o_foo)
                {
                    /* return date time string with set length */
                    return o_foo.ToString(p_s_format, System.Globalization.CultureInfo.InvariantCulture).Substring(0, p_i_length);
                }
                else
                {
                    throw new InvalidCastException("Parameter '" + p_o_value + "' is not a DateTime");
                }
            }

            /// <summary>
            /// Transpose ISO8601 timestamp string value to date time type
            /// </summary>
            /// <param name="p_s_value">ISO8601 timestamp string value</param>
            /// <returns>date time type</returns>
            /// <exception cref="InvalidCastException">cannot cast ISO8601 timestamp string value to date time type</exception>
            public static Object? TransposeDateTime_ISO8601(string? p_s_value)
            {
                if (p_s_value == null)
                {
                    return null;
                }
                else
                {
                    return ForestNET.Lib.Helper.FromISO8601UTC(p_s_value);
                }
            }

            /// <summary>
            /// Transpose date time value to ISO8601 timestamp string value
            /// </summary>
            /// <param name="p_o_value">date time value</param>
            /// <param name="p_i_length">set length for timestamp string value</param>
            /// <returns>ISO8601 timestamp string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to date time type</exception>
            public static string TransposeDateTime_ISO8601(Object? p_o_value, int p_i_length)
            {
                return TransposeDateTime_ISO8601(p_o_value, p_i_length, false);
            }

            /// <summary>
            /// Transpose date time value to ISO8601 timestamp string value, where date time value has no 'T' separator
            /// </summary>
            /// <param name="p_o_value">date time value</param>
            /// <param name="p_i_length">set length for timestamp string value</param>
            /// <returns>ISO8601 timestamp string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to date time type</exception>
            public static string TransposeDateTime_ISO8601_NoSeparator(Object? p_o_value, int p_i_length)
            {
                return TransposeDateTime_ISO8601(p_o_value, p_i_length, true);
            }

            /// <summary>
            /// Transpose date time value to ISO8601 timestamp string value
            /// </summary>
            /// <param name="p_o_value">date time value</param>
            /// <param name="p_i_length">set length for timestamp string value</param>
            /// <param name="p_b_noSeparator">true - no date and time separator 'T', false - normal ISO8601 format</param>
            /// <returns>ISO8601 timestamp string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to date time type</exception>
            private static string TransposeDateTime_ISO8601(Object? p_o_value, int p_i_length, bool p_b_noSeparator)
            {
                /* if parameter is null, return empty value string */
                if (p_o_value == null)
                {
                    return EmptyValue(" ", p_i_length);
                }

                /* check if parameter is a DateTime */
                if (p_o_value is System.DateTime o_foo)
                {
                    string s_replaceSeparator = "T";

                    /* no date and time separator 'T' */
                    if (p_b_noSeparator)
                    {
                        s_replaceSeparator = " ";
                    }

                    /* return date time string with set length */
                    return ForestNET.Lib.Helper.ToISO8601UTC(o_foo).Substring(0, p_i_length).Replace("T", s_replaceSeparator);
                }
                else
                {
                    throw new InvalidCastException("Parameter '" + p_o_value + "' is not a DateTime");
                }
            }

            /// <summary>
            /// Transpose RFC1123 timestamp string value to date time type
            /// </summary>
            /// <param name="p_s_value">RFC1123 timestamp string value</param>
            /// <returns>date time type</returns>
            /// <exception cref="InvalidCastException">cannot cast RFC1123 timestamp string value to date time type</exception>
            public static Object? TransposeDateTime_RFC1123(string? p_s_value)
            {
                if (p_s_value == null)
                {
                    return null;
                }
                else
                {
                    /* read rfc1123 date time stirng */
                    System.DateTime o_foo = System.DateTime.ParseExact(p_s_value, System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.RFC1123Pattern, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None);
                    /* convert it to local time from utc source point */
                    return System.DateTime.SpecifyKind(o_foo, DateTimeKind.Utc).ToLocalTime();
                }
            }

            /// <summary>
            /// Transpose date time value to RFC1123 timestamp string value
            /// </summary>
            /// <param name="p_o_value">date time value</param>
            /// <param name="p_i_length">set length for RFC1123 timestamp string value</param>
            /// <returns>RFC1123 timestamp string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to date time type</exception>
            public static string TransposeDateTime_RFC1123(Object? p_o_value, int p_i_length)
            {
                /* if parameter is null, return empty value string */
                if (p_o_value == null)
                {
                    return EmptyValue(" ", p_i_length);
                }

                /* check if parameter is a DateTime */
                if (p_o_value is System.DateTime o_foo)
                {
                    /* return date time string with set length */
                    return ForestNET.Lib.Helper.ToRFC1123(o_foo).Substring(0, p_i_length);
                }
                else
                {
                    throw new InvalidCastException("Parameter '" + p_o_value + "' is not a DateTime");
                }
            }

            /// <summary>
            /// Transpose timestamp string value to date time type
            /// </summary>
            /// <param name="p_s_value">timestamp string value</param>
            /// <returns>date time type</returns>
            /// <exception cref="InvalidCastException">cannot cast timestamp string value to date time type</exception>
            public static Object? TransposeDateTime_yyyymmddhhiiss(string? p_s_value)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_s_value);
            }

            /// <summary>
            /// Transpose date time value to timestamp string value
            /// </summary>
            /// <param name="p_o_value">date time value</param>
            /// <param name="p_i_length">set length for timestamp string value</param>
            /// <returns>timestamp string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to date time type</exception>
            public static string TransposeDateTime_yyyymmddhhiiss(Object? p_o_value, int p_i_length)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_o_value, p_i_length, "yyyyMMddHHmmss");
            }

            /// <summary>
            /// Transpose timestamp string('yyyy-MM-dd HH:mm:ss') value to local date time type
            /// </summary>
            /// <param name="p_s_value">timestamp string value</param>
            /// <returns>local date time type</returns>
            /// <exception cref="ClassCastException">cannot cast timestamp string value to local date time type</exception>
            public static Object? TransposeDateTime_yyyymmddhhiiss_ISO(string? p_s_value)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_s_value);
            }

            /// <summary>
            /// Transpose local date time value to timestamp string value 'yyyy-MM-dd HH:mm:ss'
            /// </summary>
            /// <param name="p_o_value">local date time value</param>
            /// <param name="p_i_length">set length for timestamp string value</param>
            /// <returns>timestamp string value</returns>
            /// <exception cref="ClassCastException">cannot cast parameter to local date time type</exception>
            public static string TransposeDateTime_yyyymmddhhiiss_ISO(Object? p_o_value, int p_i_length)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_o_value, p_i_length, "yyyy-MM-dd HH:mm:ss");
            }

            /// <summary>
            /// Transpose DMY timestamp with dot string value to date time type
            /// </summary>
            /// <param name="p_s_value">DMY timestamp with dot string value</param>
            /// <returns>date time type</returns>
            /// <exception cref="InvalidCastException">cannot cast DMY timestamp with dot string value to date time type</exception>
            public static Object? TransposeDateTime_ddmmyyyyhhiiss_Dot(string? p_s_value)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_s_value);
            }

            /// <summary>
            /// Transpose date time value to DMY timestamp with dot string value
            /// </summary>
            /// <param name="p_o_value">date time value</param>
            /// <param name="p_i_length">set length for DMY timestamp with dot string value</param>
            /// <returns>DMY timestamp with dot string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to date time type</exception>
            public static string TransposeDateTime_ddmmyyyyhhiiss_Dot(Object? p_o_value, int p_i_length)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_o_value, p_i_length, "dd.MM.yyyy HH:mm:ss");
            }

            /// <summary>
            /// Transpose DMY timestamp with slash string value to date time type
            /// </summary>
            /// <param name="p_s_value">DMY timestamp with slash string value</param>
            /// <returns>date time type</returns>
            /// <exception cref="InvalidCastException">cannot cast DMY timestamp with slash string value to date time type</exception>
            public static Object? TransposeDateTime_ddmmyyyyhhiiss_Slash(string? p_s_value)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_s_value);
            }

            /// <summary>
            /// Transpose date time value to DMY timestamp with slash string value
            /// </summary>
            /// <param name="p_o_value">date time value</param>
            /// <param name="p_i_length">set length for DMY timestamp with slash string value</param>
            /// <returns>DMY timestamp with slash string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to date time type</exception>
            public static string TransposeDateTime_ddmmyyyyhhiiss_Slash(Object? p_o_value, int p_i_length)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_o_value, p_i_length, "dd/MM/yyyy HH:mm:ss");
            }

            /// <summary>
            /// Transpose YMD timestamp with dot string value to date time type
            /// </summary>
            /// <param name="p_s_value">YMD timestamp with dot string value</param>
            /// <returns>date time type</returns>
            /// <exception cref="InvalidCastException">cannot cast YMD timestamp with dot string value to date time type</exception>
            public static Object? TransposeDateTime_yyyymmddhhiiss_Dot(string? p_s_value)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_s_value);
            }

            /// <summary>
            /// Transpose date time value to YMD timestamp with dot string value
            /// </summary>
            /// <param name="p_o_value">date time value</param>
            /// <param name="p_i_length">set length for DMY timestamp with dot string value</param>
            /// <returns>YMD timestamp with dot string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to date time type</exception>
            public static string TransposeDateTime_yyyymmddhhiiss_Dot(Object? p_o_value, int p_i_length)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_o_value, p_i_length, "yyyy.MM.dd HH:mm:ss");
            }

            /// <summary>
            /// Transpose YMD timestamp with slash string value to date time type
            /// </summary>
            /// <param name="p_s_value">YMD timestamp with slash string value</param>
            /// <returns>date time type</returns>
            /// <exception cref="InvalidCastException">cannot cast YMD timestamp with slash string value to date time type</exception>
            public static Object? TransposeDateTime_yyyymmddhhiiss_Slash(string? p_s_value)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_s_value);
            }

            /// <summary>
            /// Transpose date time value to YMD timestamp with slash string value
            /// </summary>
            /// <param name="p_o_value">date time value</param>
            /// <param name="p_i_length">set length for DMY timestamp with slash string value</param>
            /// <returns>YMD timestamp with slash string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to date time type</exception>
            public static string TransposeDateTime_yyyymmddhhiiss_Slash(Object? p_o_value, int p_i_length)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_o_value, p_i_length, "yyyy/MM/dd HH:mm:ss");
            }

            /// <summary>
            /// Transpose MDY timestamp with dot string value to date time type
            /// </summary>
            /// <param name="p_s_value">MDY timestamp with dot string value</param>
            /// <returns>date time type</returns>
            /// <exception cref="InvalidCastException">cannot cast MDY timestamp with dot string value to date time type</exception>
            public static Object? TransposeDateTime_mmddyyyyhhiiss_Dot(string? p_s_value)
            {
                if (p_s_value == null)
                {
                    return null;
                }

                /* to support MDY we must switch month and day value */
                String s_month = p_s_value.Substring(0, 2);
                String s_day = p_s_value.Substring(3, 5 - 3);
                p_s_value = s_day + "." + s_month + p_s_value.Substring(5);

                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_s_value);
            }

            /// <summary>
            /// Transpose date time value to MDY timestamp with dot string value
            /// </summary>
            /// <param name="p_o_value">date time value</param>
            /// <param name="p_i_length">set length for MDY timestamp with dot string value</param>
            /// <returns>MDY timestamp with dot string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to date time type</exception>
            public static string TransposeDateTime_mmddyyyyhhiiss_Dot(Object? p_o_value, int p_i_length)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_o_value, p_i_length, "MM.dd.yyyy HH:mm:ss");
            }

            /// <summary>
            /// Transpose MDY timestamp with slash string value to date time type
            /// </summary>
            /// <param name="p_s_value">MDY timestamp with slash string value</param>
            /// <returns>date time type</returns>
            /// <exception cref="InvalidCastException">cannot cast MDY timestamp with slash string value to date time type</exception>
            public static Object? TransposeDateTime_mmddyyyyhhiiss_Slash(string? p_s_value)
            {
                if (p_s_value == null)
                {
                    return null;
                }

                /* to support MDY we must switch month and day value */
                String s_month = p_s_value.Substring(0, 2);
                String s_day = p_s_value.Substring(3, 5 - 3);
                p_s_value = s_day + "/" + s_month + p_s_value.Substring(5);

                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_s_value);
            }

            /// <summary>
            /// Transpose date time value to MDY timestamp with slash string value
            /// </summary>
            /// <param name="p_o_value">date time value</param>
            /// <param name="p_i_length">set length for MDY timestamp with slash string value</param>
            /// <returns>MDY timestamp with slash string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to date time type</exception>
            public static string TransposeDateTime_mmddyyyyhhiiss_Slash(Object? p_o_value, int p_i_length)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_o_value, p_i_length, "MM/dd/yyyy HH:mm:ss");
            }

            /// <summary>
            /// Transpose date string value to date time type
            /// </summary>
            /// <param name="p_s_value">date string value</param>
            /// <returns>date time type</returns>
            /// <exception cref="InvalidCastException">cannot cast date string value to date time type</exception>
            public static Object? TransposeDateTime_yyyymmdd(string? p_s_value)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_s_value);
            }

            /// <summary>
            /// Transpose date time value to date string value
            /// </summary>
            /// <param name="p_o_value">date time value</param>
            /// <param name="p_i_length">set length for date string value</param>
            /// <returns>date string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to date time type</exception>
            public static string TransposeDateTime_yyyymmdd(Object? p_o_value, int p_i_length)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_o_value, p_i_length, "yyyyMMdd");
            }

            /// <summary>
            /// Transpose date string value to local date type
            /// </summary>
            /// <param name="p_s_value">date string value</param>
            /// <returns>local date type</returns>
            /// <exception cref="ClassCastException">cannot cast date string value to local date type</exception>
            public static Object? TransposeDateTime_yyyymmdd_ISO(string? p_s_value)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_s_value);
            }

            /// <summary>
            /// Transpose local date value to date string value
            /// </summary>
            /// <param name="p_o_value">local date value</param>
            /// <param name="p_i_length">set length for date string value</param>
            /// <returns>date string value</returns>
            /// <exception cref="ClassCastException">cannot cast parameter to local date type</exception>
            public static string TransposeDateTime_yyyymmdd_ISO(Object? p_o_value, int p_i_length)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_o_value, p_i_length, "yyyy-MM-dd");
            }

            /// <summary>
            /// Transpose DMY date with dot string value to date time type
            /// </summary>
            /// <param name="p_s_value">DMY date with dot string value</param>
            /// <returns>date time type</returns>
            /// <exception cref="InvalidCastException">cannot cast DMY date with dot string value to date time type</exception>
            public static Object? TransposeDateTime_ddmmyyyy_Dot(string? p_s_value)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_s_value);
            }

            /// <summary>
            /// Transpose date time value to DMY date with dot string value
            /// </summary>
            /// <param name="p_o_value">date time value</param>
            /// <param name="p_i_length">set length for DMY date with dot string value</param>
            /// <returns>DMY date with dot string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to date time type</exception>
            public static string TransposeDateTime_ddmmyyyy_Dot(Object? p_o_value, int p_i_length)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_o_value, p_i_length, "dd.MM.yyyy");
            }

            /// <summary>
            /// Transpose DMY date with slash string value to date time type
            /// </summary>
            /// <param name="p_s_value">DMY date with slash string value</param>
            /// <returns>date time type</returns>
            /// <exception cref="InvalidCastException">cannot cast DMY date with slash string value to date time type</exception>
            public static Object? TransposeDateTime_ddmmyyyy_Slash(string? p_s_value)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_s_value);
            }

            /// <summary>
            /// Transpose date time value to DMY date with slash string value
            /// </summary>
            /// <param name="p_o_value">date time value</param>
            /// <param name="p_i_length">set length for DMY date with slash string value</param>
            /// <returns>DMY date with slash string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to date time type</exception>
            public static string TransposeDateTime_ddmmyyyy_Slash(Object? p_o_value, int p_i_length)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_o_value, p_i_length, "dd/MM/yyyy");
            }

            /// <summary>
            /// Transpose YMD date with dot string value to date time type
            /// </summary>
            /// <param name="p_s_value">YMD date with dot string value</param>
            /// <returns>date time type</returns>
            /// <exception cref="InvalidCastException">cannot cast YMD date with dot string value to date time type</exception>
            public static Object? TransposeDateTime_yyyymmdd_Dot(string? p_s_value)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_s_value);
            }

            /// <summary>
            /// Transpose date time value to YMD date with dot string value
            /// </summary>
            /// <param name="p_o_value">date time value</param>
            /// <param name="p_i_length">set length for DMY date with dot string value</param>
            /// <returns>YMD date with dot string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to date time type</exception>
            public static string TransposeDateTime_yyyymmdd_Dot(Object? p_o_value, int p_i_length)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_o_value, p_i_length, "yyyy.MM.dd");
            }

            /// <summary>
            /// Transpose YMD date with slash string value to date time type
            /// </summary>
            /// <param name="p_s_value">YMD date with slash string value</param>
            /// <returns>date time type</returns>
            /// <exception cref="InvalidCastException">cannot cast YMD date with slash string value to date time type</exception>
            public static Object? TransposeDateTime_yyyymmdd_Slash(string? p_s_value)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_s_value);
            }

            /// <summary>
            /// Transpose date time value to YMD date with slash string value
            /// </summary>
            /// <param name="p_o_value">date time value</param>
            /// <param name="p_i_length">set length for DMY date with slash string value</param>
            /// <returns>YMD date with slash string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to date time type</exception>
            public static string TransposeDateTime_yyyymmdd_Slash(Object? p_o_value, int p_i_length)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_o_value, p_i_length, "yyyy/MM/dd");
            }

            /// <summary>
            /// Transpose MDY date with dot string value to date time type
            /// </summary>
            /// <param name="p_s_value">MDY date with dot string value</param>
            /// <returns>date time type</returns>
            /// <exception cref="InvalidCastException">cannot cast MDY date with dot string value to date time type</exception>
            public static Object? TransposeDateTime_mmddyyyy_Dot(string? p_s_value)
            {
                if (p_s_value == null)
                {
                    return null;
                }

                /* to support MDY we must switch month and day value */
                String s_month = p_s_value.Substring(0, 2);
                String s_day = p_s_value.Substring(3, 5 - 3);
                p_s_value = s_day + "." + s_month + p_s_value.Substring(5);

                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_s_value);
            }

            /// <summary>
            /// Transpose date time value to MDY date with dot string value
            /// </summary>
            /// <param name="p_o_value">date time value</param>
            /// <param name="p_i_length">set length for MDY date with dot string value</param>
            /// <returns>MDY date with dot string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to date time type</exception>
            public static string TransposeDateTime_mmddyyyy_Dot(Object? p_o_value, int p_i_length)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_o_value, p_i_length, "MM.dd.yyyy");
            }

            /// <summary>
            /// Transpose MDY date with slash string value to date time type
            /// </summary>
            /// <param name="p_s_value">MDY date with slash string value</param>
            /// <returns>date time type</returns>
            /// <exception cref="InvalidCastException">cannot cast MDY date with slash string value to date time type</exception>
            public static Object? TransposeDateTime_mmddyyyy_Slash(string? p_s_value)
            {
                if (p_s_value == null)
                {
                    return null;
                }

                /* to support MDY we must switch month and day value */
                String s_month = p_s_value.Substring(0, 2);
                String s_day = p_s_value.Substring(3, 5 - 3);
                p_s_value = s_day + "/" + s_month + p_s_value.Substring(5);

                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_s_value);
            }

            /// <summary>
            /// Transpose date time value to MDY date with slash string value
            /// </summary>
            /// <param name="p_o_value">date time value</param>
            /// <param name="p_i_length">set length for MDY date with slash string value</param>
            /// <returns>MDY date with slash string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to date time type</exception>
            public static string TransposeDateTime_mmddyyyy_Slash(Object? p_o_value, int p_i_length)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_o_value, p_i_length, "MM/dd/yyyy");
            }

            /// <summary>
            /// Transpose time string value to date time type
            /// </summary>
            /// <param name="p_s_value">time string value</param>
            /// <returns>date time type</returns>
            /// <exception cref="InvalidCastException">cannot cast time string value to date time type</exception>
            public static Object? TransposeDateTime_hhiiss(string? p_s_value)
            {
                if (p_s_value == null)
                {
                    return null;
                }

                System.TimeSpan o_timeSpan = ForestNET.Lib.Helper.FromTimeString(p_s_value);

                return new System.DateTime(1, 1, 1) + o_timeSpan;
            }

            /// <summary>
            /// Transpose date time value to time string value
            /// </summary>
            /// <param name="p_o_value">date time value</param>
            /// <param name="p_i_length">set length for time string value</param>
            /// <returns>time string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to date time type</exception>
            public static string TransposeDateTime_hhiiss(Object? p_o_value, int p_i_length)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_o_value, p_i_length, "HHmmss");
            }

            /// <summary>
            /// Transpose time string value to date time type
            /// </summary>
            /// <param name="p_s_value">time string value</param>
            /// <returns>date time type</returns>
            /// <exception cref="InvalidCastException">cannot cast time string value to date time type</exception>
            public static Object? TransposeDateTime_hhii(string? p_s_value)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_hhiiss(p_s_value);
            }

            /// <summary>
            /// Transpose date time value to time string value
            /// </summary>
            /// <param name="p_o_value">date time value</param>
            /// <param name="p_i_length">set length for time string value</param>
            /// <returns>time string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to date time type</exception>
            public static string TransposeDateTime_hhii(Object? p_o_value, int p_i_length)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_o_value, p_i_length, "HHmm");
            }

            /// <summary>
            /// Transpose time string with colon value to date time type
            /// </summary>
            /// <param name="p_s_value">time string with colon value</param>
            /// <returns>date time type</returns>
            /// <exception cref="InvalidCastException">cannot cast time string with colon value to date time type</exception>
            public static Object? TransposeDateTime_hhiiss_Colon(string? p_s_value)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_hhiiss(p_s_value);
            }

            /// <summary>
            /// Transpose date time value to time string with colon value
            /// </summary>
            /// <param name="p_o_value">date time value</param>
            /// <param name="p_i_length">set length for time string with colon value</param>
            /// <returns>time string with colon value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to date time type</exception>
            public static string TransposeDateTime_hhiiss_Colon(Object? p_o_value, int p_i_length)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_o_value, p_i_length, "HH:mm:ss");
            }

            /// <summary>
            /// Transpose time string with colon value to date time type
            /// </summary>
            /// <param name="p_s_value">time string with colon value</param>
            /// <returns>date time type</returns>
            /// <exception cref="InvalidCastException">cannot cast time string with colon value to date time type</exception>
            public static Object? TransposeDateTime_hhii_Colon(string? p_s_value)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_hhiiss(p_s_value);
            }

            /// <summary>
            /// Transpose date time value to time string with colon value
            /// </summary>
            /// <param name="p_o_value">date time value</param>
            /// <param name="p_i_length">set length for time string with colon value</param>
            /// <returns>time string with colon value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to date time type</exception>
            public static string TransposeDateTime_hhii_Colon(Object? p_o_value, int p_i_length)
            {
                return StandardTransposeMethods.DateTime.TransposeDateTime_any(p_o_value, p_i_length, "HH:mm");
            }
        }

        /* System.TimeSpan */

        public class TimeSpan
        {
            /// <summary>
            /// Transpose time string value to TimeSpan type
            /// </summary>
            /// <param name="p_s_value">time string value</param>
            /// <returns>TimeSpan type</returns>
            /// <exception cref="InvalidCastException">cannot cast time string value to TimeSpan type</exception>
            private static System.TimeSpan? TransposeTimeSpan_any(string? p_s_value)
            {
                if (p_s_value == null)
                {
                    return null;
                }

                if (p_s_value.Equals("000000")) /* recognize empty time */
                {
                    return null;
                }
                else
                {
                    return ForestNET.Lib.Helper.FromTimeString(p_s_value);
                }
            }

            /// <summary>
            /// Transpose TimeSpan value to time string value
            /// </summary>
            /// <param name="p_o_value">TimeSpan value</param>
            /// <param name="p_i_length">set length for time string value</param>
            /// <param name="p_s_format">format string for TimeSpan parser</param>
            /// <returns>time string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to TimeSpan type</exception>
            private static string TransposeTimeSpan_any(Object? p_o_value, int p_i_length, string p_s_format)
            {
                /* if parameter is null, return empty value string */
                if (p_o_value == null)
                {
                    /* use zero as parameter character for these specific formats */
                    if ((p_s_format == "yyyyMMddHHmmssfff") || (p_s_format == "yyyyMMddHHmmss") || (p_s_format == "yyyyMMdd") || (p_s_format == "yyyyMMdd") || (p_s_format == "hhmmss") || (p_s_format == "hhmm"))
                    {
                        return EmptyValue("0", p_i_length);
                    }
                    else
                    {
                        return EmptyValue(" ", p_i_length);
                    }
                }

                /* check if parameter is a TimeSpan */
                if (p_o_value is System.TimeSpan o_foo)
                {
                    /* return date time string with set length */
                    return o_foo.ToString(p_s_format).Substring(0, p_i_length);
                }
                else
                {
                    throw new InvalidCastException("Parameter '" + p_o_value + "' is not a TimeSpan");
                }
            }

            /// <summary>
            /// Transpose time string value to TimeSpan type
            /// </summary>
            /// <param name="p_s_value">time string value</param>
            /// <returns>TimeSpan type</returns>
            /// <exception cref="InvalidCastException">cannot cast time string value to TimeSpan type</exception>
            public static Object? TransposeTimeSpan_hhiiss(string? p_s_value)
            {
                return StandardTransposeMethods.TimeSpan.TransposeTimeSpan_any(p_s_value);
            }

            /// <summary>
            /// Transpose TimeSpan value to time string value
            /// </summary>
            /// <param name="p_o_value">TimeSpan value</param>
            /// <param name="p_i_length">set length for time string value</param>
            /// <returns>time string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to TimeSpan type</exception>
            public static string TransposeTimeSpan_hhiiss(Object? p_o_value, int p_i_length)
            {
                return StandardTransposeMethods.TimeSpan.TransposeTimeSpan_any(p_o_value, p_i_length, "hhmmss");
            }

            /// <summary>
            /// Transpose time string value to TimeSpan type
            /// </summary>
            /// <param name="p_s_value">time string value</param>
            /// <returns>TimeSpan type</returns>
            /// <exception cref="InvalidCastException">cannot cast time string value to TimeSpan type</exception>
            public static Object? TransposeTimeSpan_hhii(string? p_s_value)
            {
                return StandardTransposeMethods.TimeSpan.TransposeTimeSpan_any(p_s_value);
            }

            /// <summary>
            /// Transpose TimeSpan value to time string value
            /// </summary>
            /// <param name="p_o_value">TimeSpan value</param>
            /// <param name="p_i_length">set length for time string value</param>
            /// <returns>time string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to TimeSpan type</exception>
            public static string TransposeTimeSpan_hhii(Object? p_o_value, int p_i_length)
            {
                return StandardTransposeMethods.TimeSpan.TransposeTimeSpan_any(p_o_value, p_i_length, "hhmm");
            }

            /// <summary>
            /// Transpose time string with colon value to TimeSpan type
            /// </summary>
            /// <param name="p_s_value">time string with colon value</param>
            /// <returns>TimeSpan type</returns>
            /// <exception cref="InvalidCastException">cannot cast time string with colon value to TimeSpan type</exception>
            public static Object? TransposeTimeSpan_hhiiss_Colon(string? p_s_value)
            {
                return StandardTransposeMethods.TimeSpan.TransposeTimeSpan_any(p_s_value);
            }

            /// <summary>
            /// Transpose TimeSpan value to time string with colon value
            /// </summary>
            /// <param name="p_o_value">TimeSpan value</param>
            /// <param name="p_i_length">set length for time string with colon value</param>
            /// <returns>time string with colon value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to TimeSpan type</exception>
            public static string TransposeTimeSpan_hhiiss_Colon(Object? p_o_value, int p_i_length)
            {
                return StandardTransposeMethods.TimeSpan.TransposeTimeSpan_any(p_o_value, p_i_length, "hh\\:mm\\:ss");
            }

            /// <summary>
            /// Transpose time string with colon value to TimeSpan type
            /// </summary>
            /// <param name="p_s_value">time string with colon value</param>
            /// <returns>TimeSpan type</returns>
            /// <exception cref="InvalidCastException">cannot cast time string with colon value to TimeSpan type</exception>
            public static Object? TransposeTimeSpan_hhii_Colon(string? p_s_value)
            {
                return StandardTransposeMethods.TimeSpan.TransposeTimeSpan_any(p_s_value);
            }

            /// <summary>
            /// Transpose TimeSpan value to time string with colon value
            /// </summary>
            /// <param name="p_o_value">TimeSpan value</param>
            /// <param name="p_i_length">set length for time string with colon value</param>
            /// <returns>time string with colon value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to TimeSpan type</exception>
            public static string TransposeTimeSpan_hhii_Colon(Object? p_o_value, int p_i_length)
            {
                return StandardTransposeMethods.TimeSpan.TransposeTimeSpan_any(p_o_value, p_i_length, "hh\\:mm");
            }
        }

        public class FloatingPointNumbers
        {
            /* Float */

            /// <summary>
            /// Transpose float string value to float type
            /// </summary>
            /// <param name="p_s_value">float string value</param>
            /// <param name="i_positionDecimalSeparator">position of decimal separator</param>
            /// <returns>float type</returns>
            /// <exception cref="InvalidCastException">cannot cast string value to float type</exception>
            public static Object? TransposeFloat(string? p_s_value, int p_i_positionDecimalSeparator)
            {
                if (p_s_value == null)
                {
                    return null;
                }

                /* check if string value only has '0' characters with optional '+' or '-' sign at the start */
                if ((ForestNET.Lib.Helper.MatchesRegex(p_s_value, "^[0]+$")) || (ForestNET.Lib.Helper.MatchesRegex(p_s_value, "^\\+[0]+$")) || (ForestNET.Lib.Helper.MatchesRegex(p_s_value, "^-[0]+$")))
                {
                    /* use '0.0' as string value from here */
                    p_s_value = "0.0";
                }
                else if (p_i_positionDecimalSeparator > 0)
                { /* check if we have an optional position of decimal separator */
                    /* insert decimal separator */
                    p_s_value = p_s_value.Substring(0, p_i_positionDecimalSeparator) + "." + p_s_value.Substring(p_i_positionDecimalSeparator);
                }

                /* check float value */
                if (!ForestNET.Lib.Helper.IsFloat(p_s_value))
                {
                    throw new InvalidCastException("Value '" + p_s_value + "' cannot be cast to Float");
                }

                /* replace decimal separator */
                p_s_value = p_s_value.Replace(',', '.');

                /* return float value */
                return Convert.ToSingle(p_s_value, System.Globalization.CultureInfo.InvariantCulture);
            }

            /// <summary>
            /// Transpose float value to float string value, using system separators
            /// </summary>
            /// <param name="p_o_value">float value</param>
            /// <param name="p_i_amountDigits">amount of digits for a floating point number (write only)</param>
            /// <param name="p_i_amountFractionDigits">amount of fractional digits for a floating point number (write only)</param>
            /// <returns>float string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to float type</exception>
            public static string TransposeFloat(Object? p_o_value, int p_i_amountDigits, int p_i_amountFractionDigits)
            {
                return StandardTransposeMethods.FloatingPointNumbers.TransposeFloat(p_o_value, p_i_amountDigits, p_i_amountFractionDigits, "$", "$");
            }

            /// <summary>
            /// Transpose float value to float string value, use '$' for decimal separator to use system settings, no grouping
            /// </summary>
            /// <param name="p_o_value">float value</param>
            /// <param name="p_i_amountDigits">amount of digits for a floating point number (write only)</param>
            /// <param name="p_i_amountFractionDigits">amount of fractional digits for a floating point number (write only)</param>
            /// <param name="p_s_decimalSeparator">string for decimal separator</param>
            /// <returns>float string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to float type</exception>
            public static string TransposeFloat(Object? p_o_value, int p_i_amountDigits, int p_i_amountFractionDigits, string? p_s_decimalSeparator)
            {
                return StandardTransposeMethods.FloatingPointNumbers.TransposeFloat(p_o_value, p_i_amountDigits, p_i_amountFractionDigits, p_s_decimalSeparator, null);
            }

            /// <summary>
            /// Transpose float value to float string value, use '$' for decimal and group separator to use system separators
            /// </summary>
            /// <param name="p_o_value">float value</param>
            /// <param name="p_i_amountDigits">amount of digits for a floating point number (write only)</param>
            /// <param name="p_i_amountFractionDigits">amount of fractional digits for a floating point number (write only)</param>
            /// <param name="p_s_decimalSeparator">string for decimal separator</param>
            /// <param name="p_s_groupSeparator">string for group separator</param>
            /// <returns>float string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to float type</exception>
            public static string TransposeFloat(Object? p_o_value, int p_i_amountDigits, int p_i_amountFractionDigits, string? p_s_decimalSeparator, string? p_s_groupSeparator)
            {
                /* if parameter is null, use object value as 0 */
                if (p_o_value == null)
                {
                    p_o_value = (Object)0.0f;
                }

                /* check if parameter is a float */
                if (p_o_value is Single f_foo)
                {
                    /* overwrite decimal separator if it is not empty and set to '$' */
                    if ((!ForestNET.Lib.Helper.IsStringEmpty(p_s_decimalSeparator)) && ((p_s_decimalSeparator ?? "").Equals("$")))
                    {
                        p_s_decimalSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                    }

                    /* overwrite group separator if it is not empty and set to '$' */
                    if ((!ForestNET.Lib.Helper.IsStringEmpty(p_s_groupSeparator)) && ((p_s_groupSeparator ?? "").Equals("$")))
                    {
                        p_s_groupSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
                    }

                    if (p_i_amountDigits != 1) /* number format info does not support minimum integer digits before decimal separator, so we must do our own */
                    {
                        /* part of format string */
                        string s_minimumDigits = "0";
                        string s_minimumFractions = ".0";

                        /* get minimum integer digits for format string */
                        for (int i = 1; i < p_i_amountDigits - 1; i++)
                        {
                            s_minimumDigits += "0";
                        }

                        /* get minimum fraction digits for format string */
                        for (int i = 1; i < p_i_amountFractionDigits; i++)
                        {
                            s_minimumFractions += "0";
                        }

                        /* clear fraction part if parameter value is lower than 1 */
                        if (p_i_amountFractionDigits < 1)
                        {
                            s_minimumFractions = "";
                        }

                        /* create our format string */
                        string s_format = "{0:0," + s_minimumDigits + s_minimumFractions + ";0," + s_minimumDigits + s_minimumFractions + ";0," + s_minimumDigits + s_minimumFractions + "}";
                        /* create our string output, using our format string with invariant culture, and rounding our decimal value */
                        /* additionaly we replace ',' and '.' with very rarely used replacements, so it is easier to change ',' to '.' and after that '.' to ','  */
                        string s_foo = String.Format(s_format, Math.Round(f_foo, p_i_amountFractionDigits, MidpointRounding.ToEven), System.Globalization.CultureInfo.InvariantCulture).Replace(",", "_,_").Replace(".", "_._");
                        /* replace our group and decimal separators with our class settings */
                        s_foo = s_foo.Replace("_._", p_s_groupSeparator ?? "").Replace("_,_", p_s_decimalSeparator ?? "");
                        /* return our value as string */
                        return s_foo;
                    }
                    else
                    {
                        /* create number format info */
                        System.Globalization.NumberFormatInfo o_numberFormatInfo = new()
                        {
                            CurrencyDecimalSeparator = p_s_decimalSeparator ?? "",
                            CurrencyGroupSeparator = p_s_groupSeparator ?? "",
                            NumberDecimalSeparator = p_s_decimalSeparator ?? "",
                            NumberGroupSeparator = p_s_groupSeparator ?? "",

                            CurrencyDecimalDigits = p_i_amountFractionDigits,
                            NumberDecimalDigits = p_i_amountFractionDigits,

                            PositiveSign = "",
                            NegativeSign = ""
                        };

                        /* return our value with number format info instance and rounding */
                        return Math.Round(f_foo, p_i_amountFractionDigits, MidpointRounding.ToEven).ToString("N", o_numberFormatInfo);
                    }
                }
                else
                {
                    throw new InvalidCastException("Parameter '" + p_o_value + "' is not a Float");
                }
            }

            /// <summary>
            /// Transpose float value to float string value, using system separators
            /// </summary>
            /// <param name="p_o_value">float value</param>
            /// <param name="p_i_amountDigits">amount of digits for a floating point number (write only)</param>
            /// <param name="p_i_amountFractionDigits">amount of fractional digits for a floating point number (write only)</param>
            /// <returns>float string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to float type</exception>
            public static string TransposeFloatWithSign(Object? p_o_value, int p_i_amountDigits, int p_i_amountFractionDigits)
            {
                return StandardTransposeMethods.FloatingPointNumbers.TransposeFloatWithSign(p_o_value, p_i_amountDigits, p_i_amountFractionDigits, "$", "$");
            }

            /// <summary>
            /// Transpose float value to float string value, use '$' for decimal separator to use system settings, no grouping
            /// </summary>
            /// <param name="p_o_value">float value</param>
            /// <param name="p_i_amountDigits">amount of digits for a floating point number (write only)</param>
            /// <param name="p_i_amountFractionDigits">amount of fractional digits for a floating point number (write only)</param>
            /// <param name="p_s_decimalSeparator">string for decimal separator</param>
            /// <returns>float string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to float type</exception>
            public static string TransposeFloatWithSign(Object? p_o_value, int p_i_amountDigits, int p_i_amountFractionDigits, string? p_s_decimalSeparator)
            {
                return StandardTransposeMethods.FloatingPointNumbers.TransposeFloatWithSign(p_o_value, p_i_amountDigits, p_i_amountFractionDigits, p_s_decimalSeparator, null);
            }

            /// <summary>
            /// Transpose float value to float string value, use '$' for decimal and group separator to use system separators
            /// </summary>
            /// <param name="p_o_value">float value</param>
            /// <param name="p_i_amountDigits">amount of digits for a floating point number (write only)</param>
            /// <param name="p_i_amountFractionDigits">amount of fractional digits for a floating point number (write only)</param>
            /// <param name="p_s_decimalSeparator">string for decimal separator</param>
            /// <param name="p_s_groupSeparator">string for group separator</param>
            /// <returns>float string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to float type</exception>
            public static string TransposeFloatWithSign(Object? p_o_value, int p_i_amountDigits, int p_i_amountFractionDigits, string? p_s_decimalSeparator, string? p_s_groupSeparator)
            {
                /* if parameter is null, use object value as 0 */
                if (p_o_value == null)
                {
                    p_o_value = (Object)0.0f;
                }

                /* check if parameter is a float */
                if (p_o_value is Single f_foo)
                {
                    /* overwrite decimal separator if it is not empty and set to '$' */
                    if ((!ForestNET.Lib.Helper.IsStringEmpty(p_s_decimalSeparator)) && ((p_s_decimalSeparator ?? "").Equals("$")))
                    {
                        p_s_decimalSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                    }

                    /* overwrite group separator if it is not empty and set to '$' */
                    if ((!ForestNET.Lib.Helper.IsStringEmpty(p_s_groupSeparator)) && ((p_s_groupSeparator ?? "").Equals("$")))
                    {
                        p_s_groupSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
                    }

                    if (p_i_amountDigits != 1) /* number format info does not support minimum integer digits before decimal separator, so we must do our own */
                    {
                        /* part of format string */
                        string s_minimumDigits = "0";
                        string s_minimumFractions = ".0";

                        /* get minimum integer digits for format string */
                        for (int i = 1; i < p_i_amountDigits - 1; i++)
                        {
                            s_minimumDigits += "0";
                        }

                        /* get minimum fraction digits for format string */
                        for (int i = 1; i < p_i_amountFractionDigits; i++)
                        {
                            s_minimumFractions += "0";
                        }

                        /* clear fraction part if parameter value is lower than 1 */
                        if (p_i_amountFractionDigits < 1)
                        {
                            s_minimumFractions = "";
                        }

                        /* create our format string */
                        string s_format = "{0:+0," + s_minimumDigits + s_minimumFractions + ";-0," + s_minimumDigits + s_minimumFractions + ";+0," + s_minimumDigits + s_minimumFractions + "}";
                        /* create our string output, using our format string with invariant culture, and rounding our decimal value */
                        /* additionaly we replace ',' and '.' with very rarely used replacements, so it is easier to change ',' to '.' and after that '.' to ','  */
                        string s_foo = String.Format(s_format, Math.Round(f_foo, p_i_amountFractionDigits, MidpointRounding.ToEven), System.Globalization.CultureInfo.InvariantCulture).Replace(",", "_,_").Replace(".", "_._");
                        /* replace our group and decimal separators with our class settings */
                        s_foo = s_foo.Replace("_._", p_s_groupSeparator ?? "").Replace("_,_", p_s_decimalSeparator ?? "");
                        /* return our value as string */
                        return s_foo;
                    }
                    else
                    {
                        /* create number format info */
                        System.Globalization.NumberFormatInfo o_numberFormatInfo = new()
                        {
                            CurrencyDecimalSeparator = p_s_decimalSeparator ?? "",
                            CurrencyGroupSeparator = p_s_groupSeparator ?? "",
                            NumberDecimalSeparator = p_s_decimalSeparator ?? "",
                            NumberGroupSeparator = p_s_groupSeparator ?? "",

                            CurrencyDecimalDigits = p_i_amountFractionDigits,
                            NumberDecimalDigits = p_i_amountFractionDigits,

                            PositiveSign = "+",
                            NegativeSign = "-"
                        };

                        /* return our value with number format info instance and rounding */
                        return Math.Round(f_foo, p_i_amountFractionDigits, MidpointRounding.ToEven).ToString("N", o_numberFormatInfo);
                    }
                }
                else
                {
                    throw new InvalidCastException("Parameter '" + p_o_value + "' is not a Float");
                }
            }

            /* Double */

            /// <summary>
            /// Transpose double string value to double type
            /// </summary>
            /// <param name="p_s_value">double string value</param>
            /// <param name="i_positionDecimalSeparator">position of decimal separator</param>
            /// <returns>double type</returns>
            /// <exception cref="InvalidCastException">cannot cast string value to double type</exception>
            public static Object? TransposeDouble(string? p_s_value, int p_i_positionDecimalSeparator)
            {
                if (p_s_value == null)
                {
                    return null;
                }

                /* check if string value only has '0' characters with optional '+' or '-' sign at the start */
                if ((ForestNET.Lib.Helper.MatchesRegex(p_s_value, "^[0]+$")) || (ForestNET.Lib.Helper.MatchesRegex(p_s_value, "^\\+[0]+$")) || (ForestNET.Lib.Helper.MatchesRegex(p_s_value, "^-[0]+$")))
                {
                    /* use '0.0' as string value from here */
                    p_s_value = "0.0";
                }
                else if (p_i_positionDecimalSeparator > 0)
                { /* check if we have an optional position of decimal separator */
                    /* insert decimal separator */
                    p_s_value = p_s_value.Substring(0, p_i_positionDecimalSeparator) + "." + p_s_value.Substring(p_i_positionDecimalSeparator);
                }

                /* check double value */
                if (!ForestNET.Lib.Helper.IsDouble(p_s_value))
                {
                    throw new InvalidCastException("Value '" + p_s_value + "' cannot be cast to Double");
                }

                /* replace decimal separator */
                p_s_value = p_s_value.Replace(',', '.');

                /* return double value */
                return Convert.ToDouble(p_s_value, System.Globalization.CultureInfo.InvariantCulture);
            }

            /// <summary>
            /// Transpose double value to double string value, using system separators
            /// </summary>
            /// <param name="p_o_value">double value</param>
            /// <param name="p_i_amountDigits">amount of digits for a floating point number (write only)</param>
            /// <param name="p_i_amountFractionDigits">amount of fractional digits for a floating point number (write only)</param>
            /// <returns>double string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to double type</exception>
            public static string TransposeDouble(Object? p_o_value, int p_i_amountDigits, int p_i_amountFractionDigits)
            {
                return StandardTransposeMethods.FloatingPointNumbers.TransposeDouble(p_o_value, p_i_amountDigits, p_i_amountFractionDigits, "$", "$");
            }

            /// <summary>
            /// Transpose double value to double string value, use '$' for decimal separator to use system settings, no grouping
            /// </summary>
            /// <param name="p_o_value">double value</param>
            /// <param name="p_i_amountDigits">amount of digits for a floating point number (write only)</param>
            /// <param name="p_i_amountFractionDigits">amount of fractional digits for a floating point number (write only)</param>
            /// <param name="p_s_decimalSeparator">string for decimal separator</param>
            /// <returns>double string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to double type</exception>
            public static string TransposeDouble(Object? p_o_value, int p_i_amountDigits, int p_i_amountFractionDigits, string? p_s_decimalSeparator)
            {
                return StandardTransposeMethods.FloatingPointNumbers.TransposeDouble(p_o_value, p_i_amountDigits, p_i_amountFractionDigits, p_s_decimalSeparator, null);
            }

            /// <summary>
            /// Transpose double value to double string value, use '$' for decimal and group separator to use system separators
            /// </summary>
            /// <param name="p_o_value">double value</param>
            /// <param name="p_i_amountDigits">amount of digits for a floating point number (write only)</param>
            /// <param name="p_i_amountFractionDigits">amount of fractional digits for a floating point number (write only)</param>
            /// <param name="p_s_decimalSeparator">string for decimal separator</param>
            /// <param name="p_s_groupSeparator">string for group separator</param>
            /// <returns>double string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to double type</exception>
            public static string TransposeDouble(Object? p_o_value, int p_i_amountDigits, int p_i_amountFractionDigits, string? p_s_decimalSeparator, string? p_s_groupSeparator)
            {
                /* if parameter is null, use object value as 0 */
                if (p_o_value == null)
                {
                    p_o_value = (Object)0.0d;
                }

                /* check if parameter is a double */
                if (p_o_value is Double d_foo)
                {
                    /* overwrite decimal separator if it is not empty and set to '$' */
                    if ((!ForestNET.Lib.Helper.IsStringEmpty(p_s_decimalSeparator)) && ((p_s_decimalSeparator ?? "").Equals("$")))
                    {
                        p_s_decimalSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                    }

                    /* overwrite group separator if it is not empty and set to '$' */
                    if ((!ForestNET.Lib.Helper.IsStringEmpty(p_s_groupSeparator)) && ((p_s_groupSeparator ?? "").Equals("$")))
                    {
                        p_s_groupSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
                    }

                    if (p_i_amountDigits != 1) /* number format info does not support minimum integer digits before decimal separator, so we must do our own */
                    {
                        /* part of format string */
                        string s_minimumDigits = "0";
                        string s_minimumFractions = ".0";

                        /* get minimum integer digits for format string */
                        for (int i = 1; i < p_i_amountDigits - 1; i++)
                        {
                            s_minimumDigits += "0";
                        }

                        /* get minimum fraction digits for format string */
                        for (int i = 1; i < p_i_amountFractionDigits; i++)
                        {
                            s_minimumFractions += "0";
                        }

                        /* clear fraction part if parameter value is lower than 1 */
                        if (p_i_amountFractionDigits < 1)
                        {
                            s_minimumFractions = "";
                        }

                        /* create our format string */
                        string s_format = "{0:0," + s_minimumDigits + s_minimumFractions + ";0," + s_minimumDigits + s_minimumFractions + ";0," + s_minimumDigits + s_minimumFractions + "}";
                        /* create our string output, using our format string with invariant culture, and rounding our decimal value */
                        /* additionaly we replace ',' and '.' with very rarely used replacements, so it is easier to change ',' to '.' and after that '.' to ','  */
                        string s_foo = String.Format(s_format, Math.Round(d_foo, p_i_amountFractionDigits, MidpointRounding.ToEven), System.Globalization.CultureInfo.InvariantCulture).Replace(",", "_,_").Replace(".", "_._");
                        /* replace our group and decimal separators with our class settings */
                        s_foo = s_foo.Replace("_._", p_s_groupSeparator ?? "").Replace("_,_", p_s_decimalSeparator ?? "");
                        /* return our value as string */
                        return s_foo;
                    }
                    else
                    {
                        /* create number format info */
                        System.Globalization.NumberFormatInfo o_numberFormatInfo = new()
                        {
                            CurrencyDecimalSeparator = p_s_decimalSeparator ?? "",
                            CurrencyGroupSeparator = p_s_groupSeparator ?? "",
                            NumberDecimalSeparator = p_s_decimalSeparator ?? "",
                            NumberGroupSeparator = p_s_groupSeparator ?? "",

                            CurrencyDecimalDigits = p_i_amountFractionDigits,
                            NumberDecimalDigits = p_i_amountFractionDigits,

                            PositiveSign = "",
                            NegativeSign = ""
                        };

                        /* return our value with number format info instance and rounding */
                        return Math.Round(d_foo, p_i_amountFractionDigits, MidpointRounding.ToEven).ToString("N", o_numberFormatInfo);
                    }
                }
                else
                {
                    throw new InvalidCastException("Parameter '" + p_o_value + "' is not a Double");
                }
            }

            /// <summary>
            /// Transpose double value to double string value, using system separators
            /// </summary>
            /// <param name="p_o_value">double value</param>
            /// <param name="p_i_amountDigits">amount of digits for a floating point number (write only)</param>
            /// <param name="p_i_amountFractionDigits">amount of fractional digits for a floating point number (write only)</param>
            /// <returns>double string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to double type</exception>
            public static string TransposeDoubleWithSign(Object? p_o_value, int p_i_amountDigits, int p_i_amountFractionDigits)
            {
                return StandardTransposeMethods.FloatingPointNumbers.TransposeDoubleWithSign(p_o_value, p_i_amountDigits, p_i_amountFractionDigits, "$", "$");
            }

            /// <summary>
            /// Transpose double value to double string value, use '$' for decimal separator to use system settings, no grouping
            /// </summary>
            /// <param name="p_o_value">double value</param>
            /// <param name="p_i_amountDigits">amount of digits for a floating point number (write only)</param>
            /// <param name="p_i_amountFractionDigits">amount of fractional digits for a floating point number (write only)</param>
            /// <param name="p_s_decimalSeparator">string for decimal separator</param>
            /// <returns>double string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to double type</exception>
            public static string TransposeDoubleWithSign(Object? p_o_value, int p_i_amountDigits, int p_i_amountFractionDigits, string? p_s_decimalSeparator)
            {
                return StandardTransposeMethods.FloatingPointNumbers.TransposeDoubleWithSign(p_o_value, p_i_amountDigits, p_i_amountFractionDigits, p_s_decimalSeparator, null);
            }

            /// <summary>
            /// Transpose double value to double string value, use '$' for decimal and group separator to use system separators
            /// </summary>
            /// <param name="p_o_value">double value</param>
            /// <param name="p_i_amountDigits">amount of digits for a floating point number (write only)</param>
            /// <param name="p_i_amountFractionDigits">amount of fractional digits for a floating point number (write only)</param>
            /// <param name="p_s_decimalSeparator">string for decimal separator</param>
            /// <param name="p_s_groupSeparator">string for group separator</param>
            /// <returns>double string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to double type</exception>
            public static string TransposeDoubleWithSign(Object? p_o_value, int p_i_amountDigits, int p_i_amountFractionDigits, string? p_s_decimalSeparator, string? p_s_groupSeparator)
            {
                /* if parameter is null, use object value as 0 */
                if (p_o_value == null)
                {
                    p_o_value = (Object)0.0d;
                }

                /* check if parameter is a double */
                if (p_o_value is Double d_foo)
                {
                    /* overwrite decimal separator if it is not empty and set to '$' */
                    if ((!ForestNET.Lib.Helper.IsStringEmpty(p_s_decimalSeparator)) && ((p_s_decimalSeparator ?? "").Equals("$")))
                    {
                        p_s_decimalSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                    }

                    /* overwrite group separator if it is not empty and set to '$' */
                    if ((!ForestNET.Lib.Helper.IsStringEmpty(p_s_groupSeparator)) && ((p_s_groupSeparator ?? "").Equals("$")))
                    {
                        p_s_groupSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
                    }

                    if (p_i_amountDigits != 1) /* number format info does not support minimum integer digits before decimal separator, so we must do our own */
                    {
                        /* part of format string */
                        string s_minimumDigits = "0";
                        string s_minimumFractions = ".0";

                        /* get minimum integer digits for format string */
                        for (int i = 1; i < p_i_amountDigits - 1; i++)
                        {
                            s_minimumDigits += "0";
                        }

                        /* get minimum fraction digits for format string */
                        for (int i = 1; i < p_i_amountFractionDigits; i++)
                        {
                            s_minimumFractions += "0";
                        }

                        /* clear fraction part if parameter value is lower than 1 */
                        if (p_i_amountFractionDigits < 1)
                        {
                            s_minimumFractions = "";
                        }

                        /* create our format string */
                        string s_format = "{0:+0," + s_minimumDigits + s_minimumFractions + ";-0," + s_minimumDigits + s_minimumFractions + ";+0," + s_minimumDigits + s_minimumFractions + "}";
                        /* create our string output, using our format string with invariant culture, and rounding our decimal value */
                        /* additionaly we replace ',' and '.' with very rarely used replacements, so it is easier to change ',' to '.' and after that '.' to ','  */
                        string s_foo = String.Format(s_format, Math.Round(d_foo, p_i_amountFractionDigits, MidpointRounding.ToEven), System.Globalization.CultureInfo.InvariantCulture).Replace(",", "_,_").Replace(".", "_._");
                        /* replace our group and decimal separators with our class settings */
                        s_foo = s_foo.Replace("_._", p_s_groupSeparator ?? "").Replace("_,_", p_s_decimalSeparator ?? "");
                        /* return our value as string */
                        return s_foo;
                    }
                    else
                    {
                        /* create number format info */
                        System.Globalization.NumberFormatInfo o_numberFormatInfo = new()
                        {
                            CurrencyDecimalSeparator = p_s_decimalSeparator ?? "",
                            CurrencyGroupSeparator = p_s_groupSeparator ?? "",
                            NumberDecimalSeparator = p_s_decimalSeparator ?? "",
                            NumberGroupSeparator = p_s_groupSeparator ?? "",

                            CurrencyDecimalDigits = p_i_amountFractionDigits,
                            NumberDecimalDigits = p_i_amountFractionDigits,

                            PositiveSign = "+",
                            NegativeSign = "-"
                        };

                        /* return our value with number format info instance and rounding */
                        return Math.Round(d_foo, p_i_amountFractionDigits, MidpointRounding.ToEven).ToString("N", o_numberFormatInfo);
                    }
                }
                else
                {
                    throw new InvalidCastException("Parameter '" + p_o_value + "' is not a Double");
                }
            }

            /* Decimal */

            /// <summary>
            /// Transpose decimal string value to decimal type
            /// </summary>
            /// <param name="p_s_value">decimal string value</param>
            /// <param name="i_positionDecimalSeparator">position of decimal separator</param>
            /// <returns>decimal type</returns>
            /// <exception cref="InvalidCastException">cannot cast string value to decimal type</exception>
            public static Object? TransposeDecimal(string? p_s_value, int p_i_positionDecimalSeparator)
            {
                if (p_s_value == null)
                {
                    return null;
                }

                /* check if string value only has '0' characters with optional '+' or '-' sign at the start */
                if ((ForestNET.Lib.Helper.MatchesRegex(p_s_value, "^[0]+$")) || (ForestNET.Lib.Helper.MatchesRegex(p_s_value, "^\\+[0]+$")) || (ForestNET.Lib.Helper.MatchesRegex(p_s_value, "^-[0]+$")))
                {
                    /* use '0.0' as string value from here */
                    p_s_value = "0.0";
                }
                else if (p_i_positionDecimalSeparator > 0)
                { /* check if we have an optional position of decimal separator */
                    /* insert decimal separator */
                    p_s_value = p_s_value.Substring(0, p_i_positionDecimalSeparator) + "." + p_s_value.Substring(p_i_positionDecimalSeparator);
                }

                /* check decimal value */
                if (!ForestNET.Lib.Helper.IsDecimal(p_s_value))
                {
                    throw new InvalidCastException("Value '" + p_s_value + "' cannot be cast to Decimal");
                }

                /* replace decimal separator */
                p_s_value = p_s_value.Replace(',', '.');

                /* return decimal value */
                return Convert.ToDecimal(p_s_value, System.Globalization.CultureInfo.InvariantCulture);
            }

            /// <summary>
            /// Transpose decimal value to decimal string value, using system separators
            /// </summary>
            /// <param name="p_o_value">decimal value</param>
            /// <param name="p_i_amountDigits">amount of digits for a floating point number (write only)</param>
            /// <param name="p_i_amountFractionDigits">amount of fractional digits for a floating point number (write only)</param>
            /// <returns>decimal string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to decimal type</exception>
            public static string TransposeDecimal(Object? p_o_value, int p_i_amountDigits, int p_i_amountFractionDigits)
            {
                return StandardTransposeMethods.FloatingPointNumbers.TransposeDecimal(p_o_value, p_i_amountDigits, p_i_amountFractionDigits, "$", "$");
            }

            /// <summary>
            /// Transpose decimal value to decimal string value, use '$' for decimal separator to use system settings, no grouping
            /// </summary>
            /// <param name="p_o_value">decimal value</param>
            /// <param name="p_i_amountDigits">amount of digits for a floating point number (write only)</param>
            /// <param name="p_i_amountFractionDigits">amount of fractional digits for a floating point number (write only)</param>
            /// <param name="p_s_decimalSeparator">string for decimal separator</param>
            /// <returns>decimal string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to decimal type</exception>
            public static string TransposeDecimal(Object? p_o_value, int p_i_amountDigits, int p_i_amountFractionDigits, string? p_s_decimalSeparator)
            {
                return StandardTransposeMethods.FloatingPointNumbers.TransposeDecimal(p_o_value, p_i_amountDigits, p_i_amountFractionDigits, p_s_decimalSeparator, null);
            }

            /// <summary>
            /// Transpose decimal value to decimal string value, use '$' for decimal and group separator to use system separators
            /// </summary>
            /// <param name="p_o_value">decimal value</param>
            /// <param name="p_i_amountDigits">amount of digits for a floating point number (write only)</param>
            /// <param name="p_i_amountFractionDigits">amount of fractional digits for a floating point number (write only)</param>
            /// <param name="p_s_decimalSeparator">string for decimal separator</param>
            /// <param name="p_s_groupSeparator">string for group separator</param>
            /// <returns>decimal string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to decimal type</exception>
            public static string TransposeDecimal(Object? p_o_value, int p_i_amountDigits, int p_i_amountFractionDigits, string? p_s_decimalSeparator, string? p_s_groupSeparator)
            {
                /* if parameter is null, use object value as 0 */
                if (p_o_value == null)
                {
                    p_o_value = (Object)0.0m;
                }

                /* check if parameter is a decimal */
                if (p_o_value is Decimal dec_foo)
                {
                    /* overwrite decimal separator if it is not empty and set to '$' */
                    if ((!ForestNET.Lib.Helper.IsStringEmpty(p_s_decimalSeparator)) && ((p_s_decimalSeparator ?? "").Equals("$")))
                    {
                        p_s_decimalSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                    }

                    /* overwrite group separator if it is not empty and set to '$' */
                    if ((!ForestNET.Lib.Helper.IsStringEmpty(p_s_groupSeparator)) && ((p_s_groupSeparator ?? "").Equals("$")))
                    {
                        p_s_groupSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
                    }

                    if (p_i_amountDigits != 1) /* number format info does not support minimum integer digits before decimal separator, so we must do our own */
                    {
                        /* part of format string */
                        string s_minimumDigits = "0";
                        string s_minimumFractions = ".0";

                        /* get minimum integer digits for format string */
                        for (int i = 1; i < p_i_amountDigits - 1; i++)
                        {
                            s_minimumDigits += "0";
                        }

                        /* get minimum fraction digits for format string */
                        for (int i = 1; i < p_i_amountFractionDigits; i++)
                        {
                            s_minimumFractions += "0";
                        }

                        /* clear fraction part if parameter value is lower than 1 */
                        if (p_i_amountFractionDigits < 1)
                        {
                            s_minimumFractions = "";
                        }

                        /* create our format string */
                        string s_format = "{0:0," + s_minimumDigits + s_minimumFractions + ";0," + s_minimumDigits + s_minimumFractions + ";0," + s_minimumDigits + s_minimumFractions + "}";
                        /* create our string output, using our format string with invariant culture, and rounding our decimal value */
                        /* additionaly we replace ',' and '.' with very rarely used replacements, so it is easier to change ',' to '.' and after that '.' to ','  */
                        string s_foo = String.Format(s_format, Math.Round(dec_foo, p_i_amountFractionDigits, MidpointRounding.ToEven), System.Globalization.CultureInfo.InvariantCulture).Replace(",", "_,_").Replace(".", "_._");
                        /* replace our group and decimal separators with our class settings */
                        s_foo = s_foo.Replace("_._", p_s_groupSeparator ?? "").Replace("_,_", p_s_decimalSeparator ?? "");
                        /* return our value as string */
                        return s_foo;
                    }
                    else
                    {
                        /* create number format info */
                        System.Globalization.NumberFormatInfo o_numberFormatInfo = new()
                        {
                            CurrencyDecimalSeparator = p_s_decimalSeparator ?? "",
                            CurrencyGroupSeparator = p_s_groupSeparator ?? "",
                            NumberDecimalSeparator = p_s_decimalSeparator ?? "",
                            NumberGroupSeparator = p_s_groupSeparator ?? "",

                            CurrencyDecimalDigits = p_i_amountFractionDigits,
                            NumberDecimalDigits = p_i_amountFractionDigits,

                            PositiveSign = "",
                            NegativeSign = ""
                        };

                        /* return our value with number format info instance and rounding */
                        return Math.Round(dec_foo, p_i_amountFractionDigits, MidpointRounding.ToEven).ToString("N", o_numberFormatInfo);
                    }
                }
                else
                {
                    throw new InvalidCastException("Parameter '" + p_o_value + "' is not a Decimal");
                }
            }

            /// <summary>
            /// Transpose decimal value to decimal string value, using system separators
            /// </summary>
            /// <param name="p_o_value">decimal value</param>
            /// <param name="p_i_amountDigits">amount of digits for a floating point number (write only)</param>
            /// <param name="p_i_amountFractionDigits">amount of fractional digits for a floating point number (write only)</param>
            /// <returns>decimal string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to decimal type</exception>
            public static string TransposeDecimalWithSign(Object? p_o_value, int p_i_amountDigits, int p_i_amountFractionDigits)
            {
                return StandardTransposeMethods.FloatingPointNumbers.TransposeDecimalWithSign(p_o_value, p_i_amountDigits, p_i_amountFractionDigits, "$", "$");
            }

            /// <summary>
            /// Transpose decimal value to decimal string value, use '$' for decimal separator to use system settings, no grouping
            /// </summary>
            /// <param name="p_o_value">decimal value</param>
            /// <param name="p_i_amountDigits">amount of digits for a floating point number (write only)</param>
            /// <param name="p_i_amountFractionDigits">amount of fractional digits for a floating point number (write only)</param>
            /// <param name="p_s_decimalSeparator">string for decimal separator</param>
            /// <returns>decimal string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to decimal type</exception>
            public static string TransposeDecimalWithSign(Object? p_o_value, int p_i_amountDigits, int p_i_amountFractionDigits, string? p_s_decimalSeparator)
            {
                return StandardTransposeMethods.FloatingPointNumbers.TransposeDecimalWithSign(p_o_value, p_i_amountDigits, p_i_amountFractionDigits, p_s_decimalSeparator, null);
            }

            /// <summary>
            /// Transpose decimal value to decimal string value, use '$' for decimal and group separator to use system separators
            /// </summary>
            /// <param name="p_o_value">decimal value</param>
            /// <param name="p_i_amountDigits">amount of digits for a floating point number (write only)</param>
            /// <param name="p_i_amountFractionDigits">amount of fractional digits for a floating point number (write only)</param>
            /// <param name="p_s_decimalSeparator">string for decimal separator</param>
            /// <param name="p_s_groupSeparator">string for group separator</param>
            /// <returns>decimal string value</returns>
            /// <exception cref="InvalidCastException">cannot cast parameter to decimal type</exception>
            public static string TransposeDecimalWithSign(Object? p_o_value, int p_i_amountDigits, int p_i_amountFractionDigits, string? p_s_decimalSeparator, string? p_s_groupSeparator)
            {
                /* if parameter is null, use object value as 0 */
                if (p_o_value == null)
                {
                    p_o_value = (Object)0.0m;
                }

                /* check if parameter is a decimal */
                if (p_o_value is Decimal dec_foo)
                {
                    /* overwrite decimal separator if it is not empty and set to '$' */
                    if ((!ForestNET.Lib.Helper.IsStringEmpty(p_s_decimalSeparator)) && ((p_s_decimalSeparator ?? "").Equals("$")))
                    {
                        p_s_decimalSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                    }

                    /* overwrite group separator if it is not empty and set to '$' */
                    if ((!ForestNET.Lib.Helper.IsStringEmpty(p_s_groupSeparator)) && ((p_s_groupSeparator ?? "").Equals("$")))
                    {
                        p_s_groupSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
                    }

                    if (p_i_amountDigits != 1) /* number format info does not support minimum integer digits before decimal separator, so we must do our own */
                    {
                        /* part of format string */
                        string s_minimumDigits = "0";
                        string s_minimumFractions = ".0";

                        /* get minimum integer digits for format string */
                        for (int i = 1; i < p_i_amountDigits - 1; i++)
                        {
                            s_minimumDigits += "0";
                        }

                        /* get minimum fraction digits for format string */
                        for (int i = 1; i < p_i_amountFractionDigits; i++)
                        {
                            s_minimumFractions += "0";
                        }

                        /* clear fraction part if parameter value is lower than 1 */
                        if (p_i_amountFractionDigits < 1)
                        {
                            s_minimumFractions = "";
                        }

                        /* create our format string */
                        string s_format = "{0:+0," + s_minimumDigits + s_minimumFractions + ";-0," + s_minimumDigits + s_minimumFractions + ";+0," + s_minimumDigits + s_minimumFractions + "}";
                        /* create our string output, using our format string with invariant culture, and rounding our decimal value */
                        /* additionaly we replace ',' and '.' with very rarely used replacements, so it is easier to change ',' to '.' and after that '.' to ','  */
                        string s_foo = String.Format(s_format, Math.Round(dec_foo, p_i_amountFractionDigits, MidpointRounding.ToEven), System.Globalization.CultureInfo.InvariantCulture).Replace(",", "_,_").Replace(".", "_._");
                        /* replace our group and decimal separators with our class settings */
                        s_foo = s_foo.Replace("_._", p_s_groupSeparator ?? "").Replace("_,_", p_s_decimalSeparator ?? "");
                        /* return our value as string */
                        return s_foo;
                    }
                    else
                    {
                        /* create number format info */
                        System.Globalization.NumberFormatInfo o_numberFormatInfo = new()
                        {
                            CurrencyDecimalSeparator = p_s_decimalSeparator ?? "",
                            CurrencyGroupSeparator = p_s_groupSeparator ?? "",
                            NumberDecimalSeparator = p_s_decimalSeparator ?? "",
                            NumberGroupSeparator = p_s_groupSeparator ?? "",

                            CurrencyDecimalDigits = p_i_amountFractionDigits,
                            NumberDecimalDigits = p_i_amountFractionDigits,

                            PositiveSign = "+",
                            NegativeSign = "-"
                        };

                        /* return our value with number format info instance and rounding */
                        return Math.Round(dec_foo, p_i_amountFractionDigits, MidpointRounding.ToEven).ToString("N", o_numberFormatInfo);
                    }
                }
                else
                {
                    throw new InvalidCastException("Parameter '" + p_o_value + "' is not a Decimal");
                }
            }
        }
    }
}