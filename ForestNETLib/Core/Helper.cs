namespace ForestNETLib.Core
{
    /// <summary>
    /// Collection of static methods to help most often used program sequences which are helpful.
    /// </summary>
    public class Helper
    {
        /* Constants */

        public const string UPPERCASE_CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string LOWERCASE_CHARACTERS = "abcdefghijklmnopqrstuvwxyz";
        public const string DIGITS_CHARACTERS = "0123456789";
        public const string ALPHANUMERIC_CHARACTERS = UPPERCASE_CHARACTERS + LOWERCASE_CHARACTERS + DIGITS_CHARACTERS;

        /* Fields */

        /* Properties */

        /* Methods */

        /// <summary>
        /// checks if a given string is empty
        /// </summary>
        /// <param name="p_s_string">String parameter variable</param>
        /// <returns>true - String is empty, false - String is not empty</returns>
        public static bool IsStringEmpty(string? p_s_string)
        {
            return (p_s_string == null || p_s_string.Trim().Length == 0);
        }

        /// <summary>
        /// checks if a given object is a string
        /// </summary>
        /// <param name="p_o_object">Object parameter variable</param>
        /// <returns>true - Object is String, false - Object is not String</returns>
        public static bool IsString(Object p_o_object)
        {
            if (p_o_object is string)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// checks if a given string is a byte
        /// </summary>
        /// <param name="p_s_string">String parameter variable</param>
        /// <returns>true - String is Byte, false - String is not Byte</returns>
        public static bool IsByte(string p_s_string)
        {
            try
            {
                byte.Parse(p_s_string);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// checks if a given string is a signed byte
        /// </summary>
        /// <param name="p_s_string">String parameter variable</param>
        /// <returns>true - String is SByte, false - String is not SByte</returns>
        public static bool IsSByte(string p_s_string)
        {
            try
            {
                sbyte.Parse(p_s_string);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// checks if a given string is a short
        /// </summary>
        /// <param name="p_s_string">String parameter variable</param>
        /// <returns>true - String is Short, false - String is not Short</returns>
        public static bool IsShort(string p_s_string)
        {
            try
            {
                short.Parse(p_s_string);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// checks if a given string is an integer
        /// </summary>
        /// <param name="p_s_string">String parameter variable</param>
        /// <returns>true - String is Integer, false - String is not Integer</returns>
        public static bool IsInteger(string p_s_string)
        {
            try
            {
                int.Parse(p_s_string);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// checks if a given string is a long
        /// </summary>
        /// <param name="p_s_string">String parameter variable</param>
        /// <returns>true - String is Long, false - String is not Long</returns>
        public static bool IsLong(string p_s_string)
        {
            try
            {
                long.Parse(p_s_string);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// checks if a given string is an unsigned short
        /// </summary>
        /// <param name="p_s_string">String parameter variable</param>
        /// <returns>true - String is UShort, false - String is not UShort</returns>
        public static bool IsUShort(string p_s_string)
        {
            try
            {
                ushort.Parse(p_s_string);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// checks if a given string is an unsigned integer
        /// </summary>
        /// <param name="p_s_string">String parameter variable</param>
        /// <returns>true - String is UInteger, false - String is not UInteger</returns>
        public static bool IsUInteger(string p_s_string)
        {
            try
            {
                uint.Parse(p_s_string);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// checks if a given string is an unsigned long
        /// </summary>
        /// <param name="p_s_string">String parameter variable</param>
        /// <returns>true - String is ULong, false - String is not ULong</returns>
        public static bool IsULong(string p_s_string)
        {
            try
            {
                ulong.Parse(p_s_string);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// checks if a given string is a float
        /// </summary>
        /// <param name="p_s_string">String parameter variable</param>
        /// <returns>true - String is Float, false - String is not Float</returns>
        public static bool IsFloat(string p_s_string)
        {
            try
            {
                p_s_string = p_s_string.Replace(',', '.');
                float.Parse(p_s_string);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// checks if a given string is a double
        /// </summary>
        /// <param name="p_s_string">String parameter variable</param>
        /// <returns>true - String is Double, false - String is not Double</returns>
        public static bool IsDouble(string p_s_string)
        {
            try
            {
                p_s_string = p_s_string.Replace(',', '.');
                Double.Parse(p_s_string);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// checks if a given string is a decimal
        /// </summary>
        /// <param name="p_s_string">String parameter variable</param>
        /// <returns>true - String is Decimal, false - String is not Decimal</returns>
        public static bool IsDecimal(string p_s_string)
        {
            try
            {
                p_s_string = p_s_string.Replace('.', ',');
                Decimal.Parse(p_s_string);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// checks if a given string is a boolean
        /// </summary>
        /// <param name="p_s_string">String parameter variable</param>
        /// <returns>true - String is Boolean, false - String is not Boolean</returns>
        public static bool IsBoolean(string p_s_string)
        {
            try
            {
                Boolean.Parse(p_s_string);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// checks if a given string matches a regular expression
        /// </summary>
        /// <param name="p_s_string">String parameter variable</param>
        /// <param name="p_s_string">String regular expression parameter variable</param>
        /// <returns>true - String matches Regex, false - String not matches Regex</returns>
        public static bool MatchesRegex(string p_s_string, string p_s_regex)
        {
            return System.Text.RegularExpressions.Regex.Match(p_s_string, p_s_regex).Success;
        }

        /// <summary>
        /// returns amount of sub strings within a string
        /// </summary>
        /// <param name="p_s_string">String parameter variable</param>
        /// <param name="p_s_string">Sub-String parameter variable</param>
        /// <returns>amount of sub string within string parameter</returns>
        public static int CountSubStrings(string p_s_string, string p_s_subString)
        {
            return new System.Text.RegularExpressions.Regex(p_s_subString).Matches(p_s_string).Count;
        }

        /// <summary>
        /// returns eponentiation value of two integers
        /// </summary>
        /// <param name="p_i_base">integer base value</param>
        /// <param name="p_i_exponent">integer exponent value</param>
        /// <returns>integer value</returns>
        /// <exception cref="ArgumentException">negative exponent parameter is not allowed</exception>
        public static int PowIntegers(int p_i_base, int p_i_exponent)
        {
            /* check for negative exponent parameter */
            if (p_i_exponent < 0)
            {
                throw new ArgumentException("Negative exponent not allowed.");
            }

            /* if exponent is 0, result is 1 */
            if (p_i_exponent < 1)
            {
                return 1;
            }

            /* result variable */
            int i = 1;

            /* execute until shift returns in overflow -> 0 */
            while (p_i_exponent != 0)
            {
                /* if our exponent has first bit position set */
                if ((p_i_exponent & 1) == 1)
                {
                    /* add current exponentiation value to our result */
                    i *= p_i_base;
                }

                /* do exponentiation */
                p_i_base *= p_i_base;
                /* shift exponent */
                p_i_exponent >>= 1;
            }

            return i;
        }

        /// <summary>
        /// checks if a given string matches a date format
        /// 						valid formats: [dd-MM-yyyy], [dd.MM.yyyy], [dd/MM/yyyy], [yyyy/MM/dd], [yyyy-MM-dd], [MM/dd/yyyy], [yyyy/dd/MM], [yyyy.MM.dd], [yyyyMMdd]
        /// </summary>
        /// <param name="p_s_string">String parameter variable</param>
        /// <returns>true - String matches a date format, false - String does not match a date format</returns>
        public static bool IsDate(string p_s_string)
        {
            /* patterns for date formats */
            string[] a_formats = {
                "dd-MM-yyyy",
                "dd.MM.yyyy",
                "dd/MM/yyyy",
                "yyyy/MM/dd",
                "yyyy-MM-dd",
                "MM/dd/yyyy",
                "yyyy/dd/MM",
                "yyyy.MM.dd",
                "yyyyMMdd"
            };

            return DateTime.TryParseExact(p_s_string, a_formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out _);
        }

        /// <summary>
        /// checks if a given string matches a time format
        /// 						valid formats: [h:mm:ss.fff], [h:mm:ss], [h:mm], [hmmss], [hmm], [hmmssfff]
        /// </summary>
        /// <param name="p_s_string">String parameter variable</param>
        /// <returns>true - String matches time format, false - String not matches time format</returns>
        public static bool IsTime(string p_s_string)
        {
            /* patterns for time formats */
            string[] a_formats = {
                "h\\:mm\\:ss\\.fff",
                "h\\:mm\\:ss",
                "h\\:mm",
                "hmmss",
                "hmm",
                "hmmssfff"
            };

            return TimeSpan.TryParseExact(p_s_string, a_formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.TimeSpanStyles.None, out _);
        }

        /// <summary>
        /// checks if a given string matches a date time format
        /// 						valid date part formats: [dd-MM-yyyy], [dd.MM.yyyy], [dd/MM/yyyy], [yyyy/MM/dd], [yyyy-MM-dd], [MM/dd/yyyy], [yyyy/dd/MM], [yyyy.MM.dd], [yyyyMMdd]
        /// 						valid time part formats: [h:mm:ss.fff], [h:mm:ss], [h:mm], [hmmss], [hmm], [hmmssfff]
        /// </summary>
        /// <param name="p_s_string">String parameter variable</param>
        /// <returns>true - String matches date time format, false - String not matches date time format</returns>
        public static bool IsDateTime(string p_s_string)
        {
            string[] a_dateAndTimeParts;

            if (p_s_string.Contains('T'))
            {
                /* date and time are separated by 'T' */
                a_dateAndTimeParts = p_s_string.Split("T");

                if (a_dateAndTimeParts.Length != 2)
                {
                    /* only one 'T' in parameter string value is valid */
                    return false;
                }
            }
            else if (p_s_string.Contains(' '))
            {
                /* date and time are separated by ' ' */
                a_dateAndTimeParts = p_s_string.Split(" ");

                if (a_dateAndTimeParts.Length != 2)
                {
                    /* only one ' ' in parameter string value is valid */
                    return false;
                }
            }
            else if ((p_s_string.Length == 14) && (ForestNETLib.Core.Helper.MatchesRegex(p_s_string, "[0-9]+")))
            { /* 14 digits in a row -> yyyyMMddHHmmss */
                /* separate date and time part */
                a_dateAndTimeParts = new String[2];
                a_dateAndTimeParts[0] = p_s_string.Substring(0, 8); /* date part yyyyMMdd */
                a_dateAndTimeParts[1] = p_s_string.Substring(8); /* time part HHmmss */
            }
            else if ((p_s_string.Length == 17) && (ForestNETLib.Core.Helper.MatchesRegex(p_s_string, "[0-9]+")))
            { /* 17 digits in a row -> yyyyMMddHHmmssfff */
                /* separate date and time part */
                a_dateAndTimeParts = new String[2];
                a_dateAndTimeParts[0] = p_s_string.Substring(0, 8); /* date part yyyyMMdd */
                a_dateAndTimeParts[1] = p_s_string.Substring(8); /* time part HHmmssfff */
            }
            else if ((p_s_string.Length == 8) && (ForestNETLib.Core.Helper.MatchesRegex(p_s_string, "[0-9]+")))
            { /* 8 digits in a row -> yyyyMMdd, no time part */
                /* separate date and time part */
                a_dateAndTimeParts = new String[2];
                a_dateAndTimeParts[0] = p_s_string; /* date part yyyyMMdd */
                a_dateAndTimeParts[1] = "000000"; /* time part empty */
            }
            else if (p_s_string.Length == 10)
            { /* 10 characters -> any date format, no time part */
                /* separate date and time part */
                a_dateAndTimeParts = new String[2];
                a_dateAndTimeParts[0] = p_s_string; /* date part */
                a_dateAndTimeParts[1] = "00:00:00"; /* time part empty */
            }
            else
            {
                /* date and time are not separated by 'T' or ' ' */
                return false;
            }

            /* remove UTC 'Z' at the end of time part */
            if (a_dateAndTimeParts[1].EndsWith("Z"))
            {
                a_dateAndTimeParts[1] = a_dateAndTimeParts[1].Substring(0, a_dateAndTimeParts[1].Length - 1);
            }

            /* check if both parts are matching date and time format */
            if (IsDate(a_dateAndTimeParts[0]) && IsTime(a_dateAndTimeParts[1]))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// checks if a given string matches a date interval regular expression
        /// </summary>
        /// <param name="p_s_string">String parameter variable</param>
        /// <returns>true - String matches date interval, false - String not matches date interval</returns>
        public static bool IsDateInterval(string p_s_string)
        {
            return MatchesRegex(p_s_string, "^((P(((\\d)+Y(\\d)+M((\\d)+(W|D))?)|((\\d)+(Y|M)(\\d)+(W|D))|((\\d)+(Y|M|W|D)))T(((\\d)+H(\\d)+M(\\d)+S)|((\\d)+H(\\d)+(M|S))|((\\d)+M(\\d)+S)|((\\d)+(H|M|S))))|(PT(((\\d)+H(\\d)+M(\\d)+S)|((\\d)+H(\\d)+(M|S))|((\\d)+M(\\d)+S)|((\\d)+(H|M|S))))|(P(((\\d)+Y(\\d)+M((\\d)+(W|D))?)|((\\d)+(Y|M)(\\d)+(W|D))|((\\d)+(Y|M|W|D)))))$");
        }

        /// <summary>
        /// convert a local date time object to a RFC 1123 conform date string
        /// </summary>
        /// <param name="p_o_date">DateTime parameter variable</param>
        /// <returns>RFC 1123 conform date string</returns>
        public static string ToRFC1123(DateTime p_o_datetime)
        {
            return p_o_datetime.ToUniversalTime().ToString("r", System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// convert a local date time object to a iso-8601 utc conform date string
        /// </summary>
        /// <param name="p_o_date">DateTime parameter variable</param>
        /// <returns>iso-8601 utc conform date string</returns>
        public static string ToISO8601UTC(DateTime p_o_datetime)
        {
            return string.Concat(p_o_datetime.ToUniversalTime().ToString("s", System.Globalization.CultureInfo.InvariantCulture), "Z");
        }

        /// <summary>
        /// convert a iso-8601 utc conform date string to a local date time object
        /// </summary>
        /// <param name="p_s_string">iso-8601 utc conform date string</param>
        /// <returns>local date time object</returns>
        /// <exception cref="ArgumentException">cannot convert string, details in exception message</exception>
        public static DateTime FromISO8601UTC(string p_s_string)
        {
            string[] a_dateAndTimeParts;

            if (p_s_string.Contains('T'))
            {
                /* date and time are separated by 'T' */
                a_dateAndTimeParts = p_s_string.Split("T");

                if (a_dateAndTimeParts.Length != 2)
                {
                    /* only one 'T' in parameter string value is valid */
                    throw new ArgumentException("Invalid date time string[" + p_s_string + "]; only one 'T' in parameter string value is valid");
                }
            }
            else if (p_s_string.Contains(' '))
            {
                /* date and time are separated by ' ' */
                a_dateAndTimeParts = p_s_string.Split(" ");

                if (a_dateAndTimeParts.Length != 2)
                {
                    /* only one ' ' in parameter string value is valid */
                    throw new ArgumentException("Invalid date time string[" + p_s_string + "]; only one ' ' in parameter string value is valid");
                }
            }
            else
            {
                if (IsDate(p_s_string))
                {
                    /* if we only have a date part, we add '00:00:00' as time part */
                    p_s_string += "T00:00:00";

                    /* date and time are separated by 'T' */
                    a_dateAndTimeParts = p_s_string.Split("T");

                    if (a_dateAndTimeParts.Length != 2)
                    {
                        /* only one 'T' in parameter string value is valid */
                        throw new ArgumentException("Invalid date time string[" + p_s_string + "]; only one 'T' in parameter string value is valid");
                    }
                }
                else
                {
                    /* date and time are not separated by 'T' or ' ' */
                    throw new ArgumentException("Invalid date time string[" + p_s_string + "]; date and time are not separated by 'T' or ' '");
                }
            }

            /* remove milliseconds of time part */
            if (a_dateAndTimeParts[1].Contains('.'))
            {
                a_dateAndTimeParts[1] = a_dateAndTimeParts[1].Substring(0, a_dateAndTimeParts[1].IndexOf("."));
            }

            /* remove UTC 'Z' at the end of time part */
            if (a_dateAndTimeParts[1].EndsWith("Z"))
            {
                a_dateAndTimeParts[1] = a_dateAndTimeParts[1].Substring(0, a_dateAndTimeParts[1].Length - 1);
            }

            /* if digit only date value is '00000000', we must have DateTime.MinValue '00010101' */
            if (a_dateAndTimeParts[0].Equals("00000000"))
            {
                a_dateAndTimeParts[0] = "00010101";
            }

            /* check if both parts are matching date and time format */
            if (!(IsDate(a_dateAndTimeParts[0]) && IsTime(a_dateAndTimeParts[1])))
            {
                throw new ArgumentException("Invalid date time string[" + p_s_string + "]; both parts are not matching date(" + IsDate(a_dateAndTimeParts[0]) + ") and time(" + IsTime(a_dateAndTimeParts[1]) + ") format");
            }

            /* add last two second digits '00' to parameter value, because DateTime class needs this */
            if (a_dateAndTimeParts[1].Length == 5) /* time format is only 'HH:mm' */
            {
                /* add last two second digits '00' */
                a_dateAndTimeParts[1] = a_dateAndTimeParts[1] + ":00";
            }

            /* patterns for date formats */
            string[] a_formats = {
                "dd-MM-yyyy HH:mm:ss",
                "dd.MM.yyyy HH:mm:ss",
                "dd/MM/yyyy HH:mm:ss",
                "yyyy/MM/dd HH:mm:ss",
                "yyyy-MM-dd HH:mm:ss",
                "MM/dd/yyyy HH:mm:ss",
                "yyyy/dd/MM HH:mm:ss",
                "yyyy.MM.dd HH:mm:ss",
                "yyyyMMdd HHmmss"
            };

            DateTime o_foo = DateTime.ParseExact(a_dateAndTimeParts[0] + " " + a_dateAndTimeParts[1], a_formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None);

            return DateTime.SpecifyKind(o_foo, DateTimeKind.Utc).ToLocalTime();
        }

        /// <summary>
        /// convert a local date time object to a iso-8601 conform date time string, but keep local timezone - no 'Z'
        /// </summary>
        /// <param name="p_o_date">DateTime parameter variable</param>
        /// <returns>iso-8601 conform date time string</returns>
        public static string ToDateTimeString(DateTime p_o_datetime)
        {
            return p_o_datetime.ToString("s");
        }

        /// <summary>
        /// convert a conform date time string to a local date time object, but keep local timezone - no 'Z'
        /// </summary>
        /// <param name="p_s_string">conform date time string</param>
        /// <returns>local date time object</returns>
        /// <exception cref="ArgumentException">cannot convert string, details in exception message</exception>
        public static DateTime FromDateTimeString(string p_s_string)
        {
            string[] a_dateAndTimeParts;

            if (p_s_string.Contains('T'))
            {
                /* date and time are separated by 'T' */
                a_dateAndTimeParts = p_s_string.Split("T");

                if (a_dateAndTimeParts.Length != 2)
                {
                    /* only one 'T' in parameter string value is valid */
                    throw new ArgumentException("Invalid date time string[" + p_s_string + "]; only one 'T' in parameter string value is valid");
                }
            }
            else if (p_s_string.Contains(' '))
            {
                /* date and time are separated by ' ' */
                a_dateAndTimeParts = p_s_string.Split(" ");

                if (a_dateAndTimeParts.Length != 2)
                {
                    /* only one ' ' in parameter string value is valid */
                    throw new ArgumentException("Invalid date time string[" + p_s_string + "]; only one ' ' in parameter string value is valid");
                }
            }
            else if ((p_s_string.Length == 14) && (ForestNETLib.Core.Helper.MatchesRegex(p_s_string, "[0-9]+")) ) { /* 14 digits in a row -> yyyyMMddHHmmss */
                /* separate date and time part */
			    a_dateAndTimeParts = new String[2];
			    a_dateAndTimeParts[0] = p_s_string.Substring(0, 8); /* date part yyyyMMdd */
			    a_dateAndTimeParts[1] = p_s_string.Substring(8); /* time part HHmmss */
		    }
            else if ((p_s_string.Length == 17) && (ForestNETLib.Core.Helper.MatchesRegex(p_s_string, "[0-9]+")))
            { /* 17 digits in a row -> yyyyMMddHHmmssfff */
                /* separate date and time part */
                a_dateAndTimeParts = new String[2];
                a_dateAndTimeParts[0] = p_s_string.Substring(0, 8); /* date part yyyyMMdd */
                a_dateAndTimeParts[1] = p_s_string.Substring(8); /* time part HHmmssfff */
            }
            else if ((p_s_string.Length == 8) && (ForestNETLib.Core.Helper.MatchesRegex(p_s_string, "[0-9]+")))
            { /* 8 digits in a row -> yyyyMMdd, no time part */
                /* separate date and time part */
                a_dateAndTimeParts = new String[2];
                a_dateAndTimeParts[0] = p_s_string; /* date part yyyyMMdd */
                a_dateAndTimeParts[1] = "000000"; /* time part empty */
            }
            else if (p_s_string.Length == 10)
            { /* 10 characters -> any date format, no time part */
                /* separate date and time part */
                a_dateAndTimeParts = new String[2];
                a_dateAndTimeParts[0] = p_s_string; /* date part */
                a_dateAndTimeParts[1] = "00:00:00"; /* time part empty */
            }
            else
            {
                /* date and time are not separated by 'T' or ' ' */
                throw new ArgumentException("Invalid date time string[" + p_s_string + "]; date and time are not separated by 'T' or ' '");
            }

            /* remove UTC 'Z' at the end of time part */
            if (a_dateAndTimeParts[1].EndsWith("Z"))
            {
                a_dateAndTimeParts[1] = a_dateAndTimeParts[1].Substring(0, a_dateAndTimeParts[1].Length - 1);
            }

            /* if digit only date value is '00000000', we must have DateTime.MinValue '00010101' */
            if (a_dateAndTimeParts[0].Equals("00000000"))
            {
                a_dateAndTimeParts[0] = "00010101";
            }

            /* check if both parts are matching date and time format */
            if (!(IsDate(a_dateAndTimeParts[0]) && IsTime(a_dateAndTimeParts[1])))
            {
                throw new ArgumentException("Invalid date time string[" + p_s_string + "]; both parts are not matching date(" + IsDate(a_dateAndTimeParts[0]) + ") and time(" + IsTime(a_dateAndTimeParts[1]) + ") format");
            }

            /* add last two second digits '00' to parameter value, because DateTime class needs this */
            if (a_dateAndTimeParts[1].Length == 5) /* time format is only 'HH:mm' */
            {
                /* add last two second digits '00' */
                a_dateAndTimeParts[1] = a_dateAndTimeParts[1] + ":00";
            }

            /* patterns for date formats */
            string[] a_formats = {
                "dd-MM-yyyy HH:mm:ss",
                "dd.MM.yyyy HH:mm:ss",
                "dd/MM/yyyy HH:mm:ss",
                "yyyy/MM/dd HH:mm:ss",
                "yyyy-MM-dd HH:mm:ss",
                "MM/dd/yyyy HH:mm:ss",
                "yyyy/dd/MM HH:mm:ss",
                "yyyy.MM.dd HH:mm:ss",
                "yyyyMMdd HHmmss",
                "dd-MM-yyyy HH:mm:ss.fff",
                "dd.MM.yyyy HH:mm:ss.fff",
                "dd/MM/yyyy HH:mm:ss.fff",
                "yyyy/MM/dd HH:mm:ss.fff",
                "yyyy-MM-dd HH:mm:ss.fff",
                "MM/dd/yyyy HH:mm:ss.fff",
                "yyyy/dd/MM HH:mm:ss.fff",
                "yyyy.MM.dd HH:mm:ss.fff",
                "yyyyMMdd HHmmssfff"
            };

            return DateTime.ParseExact(a_dateAndTimeParts[0] + " " + a_dateAndTimeParts[1], a_formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None);
        }

        /// <summary>
        /// convert a conform date string to a local date object, but keep local timezone - no 'Z'
        /// </summary>
        /// <param name="p_s_string">conform date string</param>
        /// <returns>local date object</returns>
        /// <exception cref="ArgumentException">cannot convert string, details in exception message</exception>
        public static DateTime FromDateString(string p_s_string)
        {
            /* check if parameter matching date format */
            if (!IsDate(p_s_string))
            {
                throw new ArgumentException("Invalid date string[" + p_s_string + "]; not matching date format");
            }

            /* patterns for date formats */
            string[] a_formats = {
                "dd-MM-yyyy",
                "dd.MM.yyyy",
                "dd/MM/yyyy",
                "yyyy/MM/dd",
                "yyyy-MM-dd",
                "MM/dd/yyyy",
                "yyyy/dd/MM",
                "yyyy.MM.dd",
                "yyyyMMdd"
            };

            return DateTime.ParseExact(p_s_string, a_formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None);
        }

        /// <summary>
        /// convert a conform time string to a local time object, but keep local timezone - no 'Z'
        /// </summary>
        /// <param name="p_s_string">conform time string</param>
        /// <returns>local time object</returns>
        /// <exception cref="ArgumentException">cannot convert string, details in exception message</exception>
        public static TimeSpan FromTimeString(string p_s_string)
        {
            /* check if parameter matching time format */
            if (!IsTime(p_s_string))
            {
                throw new ArgumentException("Invalid time string[" + p_s_string + "]; not matching time format");
            }

            /* patterns for date formats */
            string[] a_formats = {
                "h\\:mm\\:ss\\.fff",
                "h\\:mm\\:ss",
                "h\\:mm",
                "hmmss",
                "hmm",
                "hmmssfff"
            };

            TimeSpan.TryParseExact(p_s_string, a_formats, System.Globalization.CultureInfo.InvariantCulture, out TimeSpan o_foo);

            return o_foo;
        }

        /// <summary>
        /// creates a secure random integer value between min..max parameter value
        /// </summary>
        /// <param name="p_i_min">minimal integer range</param>
        /// <param name="p_i_max">maximal integer range</param>
        /// <returns>secure random integer value</returns>
        /// <exception cref="ArgumentException">invalid min. and max. parameter value</exception>
        public static int SecureRandomIntegerRange(int p_i_min, int p_i_max)
        {
            if ((p_i_min < 0) || (p_i_max < 0))
            {
                throw new ArgumentException("Min. and max. value parameter do not match. Min. value(" + p_i_min + ") and max. value(" + p_i_max + ") must both be a positive integer number.");
            }

            if (p_i_min >= p_i_max)
            {
                throw new ArgumentException("Min. and max. value parameter do not match. Min. value(" + p_i_min + ") >= max. value(" + p_i_max + ")");
            }

            int i_result = 0;

            /* get our secure random number generator instance */
            using (System.Security.Cryptography.RandomNumberGenerator o_randomNumberGenerator = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                /* prepare byte array of four bytes -> int */
                byte[] a_byte = new byte[4];

                do
                {
                    /* generate random byte values for all 32 bits */
                    o_randomNumberGenerator.GetBytes(a_byte);
                    /* convert our random bytes to integer value; at this moment the random value is to big because of all 32 bits */
                    i_result = ByteArrayToInt(a_byte);

                    /* amount of bits which are interesting, considerung max. value */
                    int i_bits = 0;

                    /* iterate all 31 - 0 bits, not 32 because we are not supporting unsigned integer here */
                    for (int i = 0; i < 32; i++)
                    {
                        int i_foo = 0;

                        if (i == 31) /* signed integer boundary */
                        {
                            i_foo = PowIntegers(2, i) - 1;
                        }
                        else
                        {
                            i_foo = PowIntegers(2, i);
                        }

                        /* if we have exceed our max. value, we have our amount of bits which are interesting for us */
                        if (i_foo > p_i_max)
                        {
                            /* remember amout of bits */
                            i_bits = i;
                            break;
                        }
                    }

                    /* mask variable */
                    int i_mask = 0;

                    /* set bits in our mask variable by amount of bits which are interesting for us */
                    for (int i = 0; i < i_bits; i++)
                    {
                        i_mask = (i_mask & 0x7FFF_FFFF) << 1;
                        i_mask++;
                    }

                    /* now use maks on our random value to reduce it to our desired range */
                    i_result &= i_mask;

                    /* check if our random value is in our desired range, otherwise do again */
                } while ((i_result < p_i_min) || (i_result > p_i_max));
            }

            return i_result;
        }

        /// <summary>
        /// creates a random integer value between min..max parameter value
        /// </summary>
        /// <param name="p_i_min">minimal integer range</param>
        /// <param name="p_i_max">maximal integer range</param>
        /// <returns>random integer value</returns>
        /// <exception cref="ArgumentException">invalid min. and max. parameter value</exception>
        public static int RandomIntegerRange(int p_i_min, int p_i_max)
        {
            if ((p_i_min < 0) || (p_i_max < 0))
            {
                throw new ArgumentException("Min. and max. value parameter do not match. Min. value(" + p_i_min + ") and max. value(" + p_i_max + ") must both be a positive integer number.");
            }

            if (p_i_min >= p_i_max)
            {
                throw new ArgumentException("Min. and max. value parameter do not match. Min. value(" + p_i_min + ") >= max. value(" + p_i_max + ")");
            }

            int r = (int)Math.Round((new Random().NextDouble() * ((Convert.ToDouble(p_i_max) - Convert.ToDouble(p_i_min)) + 1.0)) + Convert.ToDouble(p_i_min));
            return (r > p_i_max) ? p_i_max : r;
        }

        /// <summary>
        /// creates a random double value between min..max parameter value
        /// </summary>
        /// <param name="p_i_min">minimal double range</param>
        /// <param name="p_i_max">maximal double range</param>
        /// <returns>random double value</returns>
        /// <exception cref="ArgumentException">invalid min. and max. parameter value</exception>
        public static double RandomDoubleRange(double p_d_min, double p_d_max)
        {
            if ((p_d_min < 0.0d) || (p_d_max < 0.0d))
            {
                throw new ArgumentException("Min. and max. value parameter do not match. Min. value(" + p_d_min + ") and max. value(" + p_d_max + ") must both be a positive double number.");
            }

            if (p_d_min >= p_d_max)
            {
                throw new ArgumentException("Min. and max. value parameter do not match. Min. value(" + p_d_min + ") >= max. value(" + p_d_max + ")");
            }

            return ((new Random().NextDouble() * (p_d_max - p_d_min)) + p_d_min);
        }

        /// <summary>
        /// converts a byte array to a short
        /// </summary>
        /// <param name="p_a_bytes">byte array, length 1..2 or return value will be 0</param>
        /// <returns>short value, 0 if parameter is null</returns>
        public static short ByteArrayToShort(byte[]? p_a_bytes)
        {
            short sh_return = 0;

            if (p_a_bytes != null)
            {
                if ((p_a_bytes.Length > 0) && (p_a_bytes.Length < 3))
                {
                    for (int i = p_a_bytes.Length; i > 0; i--)
                    {
                        sh_return |= (short)((0x00FF << ((p_a_bytes.Length - i) * 8)) & (p_a_bytes[i - 1] << ((p_a_bytes.Length - i) * 8)));
                    }
                }
            }

            return sh_return;
        }

        /// <summary>
        /// converts a byte array to an integer
        /// </summary>
        /// <param name="p_a_bytes">byte array, length 1..4 or return value will be 0</param>
        /// <returns>integer value, 0 if parameter is null</returns>
        public static int ByteArrayToInt(byte[]? p_a_bytes)
        {
            int i_return = 0;

            if (p_a_bytes != null)
            {
                if ((p_a_bytes.Length > 0) && (p_a_bytes.Length < 5))
                {
                    for (int i = p_a_bytes.Length; i > 0; i--)
                    {
                        i_return |= ((0x000000FF << ((p_a_bytes.Length - i) * 8)) & (p_a_bytes[i - 1] << ((p_a_bytes.Length - i) * 8)));
                    }
                }
            }

            return i_return;
        }

        /// <summary>
        /// converts a byte array to a long
        /// </summary>
        /// <param name="p_a_bytes">byte array, length 1..8 or return value will be 0</param>
        /// <returns>long value, 0 if parameter is null</returns>
        public static long ByteArrayToLong(byte[]? p_a_bytes)
        {
            long l_return = 0;

            if (p_a_bytes != null)
            {
                if ((p_a_bytes.Length > 0) && (p_a_bytes.Length < 9))
                {
                    for (int i = p_a_bytes.Length; i > 0; i--)
                    {
                        l_return |= (((long)0x00000000000000FF << ((p_a_bytes.Length - i) * 8)) & ((long)p_a_bytes[i - 1] << ((p_a_bytes.Length - i) * 8)));
                    }
                }
            }

            return l_return;
        }

        /// <summary>
        /// converts a short to a byte array
        /// </summary>
        /// <param name="p_sh_integer">short value</param>
        /// <returns>byte array, length 1..2 - null if invalid short value</returns>
        public static byte[]? ShortToByteArray(short p_sh_short)
        {
            byte[]? a_return = null;

            if ((p_sh_short >= short.MinValue) && (p_sh_short <= short.MaxValue))
            {
                byte by_arrayLength = 1;

                if ((p_sh_short > 255) || (p_sh_short < 0))
                {
                    by_arrayLength++;
                }

                a_return = new byte[by_arrayLength];

                for (int i = by_arrayLength; i > 0; i--)
                {
                    a_return[by_arrayLength - i] |= (byte)(0x00FF & (p_sh_short >> ((i - 1) * 8)));
                }
            }

            return a_return;
        }

        /// <summary>
        /// converts an integer to a byte array
        /// </summary>
        /// <param name="p_i_integer">integer value</param>
        /// <returns>byte array, length 1..4 - null if invalid integer value</returns>
        public static byte[]? IntToByteArray(int p_i_integer)
        {
            byte[]? a_return = null;

            if ((p_i_integer >= int.MinValue) && (p_i_integer <= int.MaxValue))
            {
                byte by_arrayLength = 1;

                if ((p_i_integer > 255) || (p_i_integer < 0))
                {
                    by_arrayLength++;
                }

                if ((p_i_integer > 65535) || (p_i_integer < 0))
                {
                    by_arrayLength++;
                }

                if ((p_i_integer > 16777215) || (p_i_integer < 0))
                {
                    by_arrayLength++;
                }

                a_return = new byte[by_arrayLength];

                for (int i = by_arrayLength; i > 0; i--)
                {
                    a_return[by_arrayLength - i] |= (byte)(0x000000FF & (p_i_integer >> ((i - 1) * 8)));
                }
            }

            return a_return;
        }

        /// <summary>
        /// converts a long to a byte array
        /// </summary>
        /// <param name="p_l_long">long value</param>
        /// <returns>byte array, length 1..8 - null if invalid long value</returns>
        public static byte[]? LongToByteArray(long p_l_long)
        {
            byte[]? a_return = null;

            if ((p_l_long >= long.MinValue) && (p_l_long <= long.MaxValue))
            {
                byte by_arrayLength = 1;

                if ((p_l_long > 255) || (p_l_long < 0))
                {
                    by_arrayLength++;
                }

                if ((p_l_long > 65535) || (p_l_long < 0))
                {
                    by_arrayLength++;
                }

                if ((p_l_long > 16777215) || (p_l_long < 0))
                {
                    by_arrayLength++;
                }

                if ((p_l_long > 4294967295) || (p_l_long < 0))
                {
                    by_arrayLength++;
                }

                if ((p_l_long > 1099511627775) || (p_l_long < 0))
                {
                    by_arrayLength++;
                }

                if ((p_l_long > 281474976710655) || (p_l_long < 0))
                {
                    by_arrayLength++;
                }

                if ((p_l_long > 72057594037927935) || (p_l_long < 0))
                {
                    by_arrayLength++;
                }

                a_return = new byte[by_arrayLength];

                for (int i = by_arrayLength; i > 0; i--)
                {
                    a_return[by_arrayLength - i] |= (byte)(0x00000000000000FF & (p_l_long >> ((i - 1) * 8)));
                }
            }

            return a_return;
        }

        /// <summary>
        /// converts a long amount to a byte array with at least N bytes
        /// </summary>
        /// <param name="p_l_amount">amount long value</param>
        /// <param name="p_i_nBytes">amount of bytes which must be returned at least</param>
        /// <returns>byte array, length 1..8 - null if invalid long value</returns>
        /// <exception cref="ArgumentException">amount long value must be a positive long value, or amount of bytes parameter is not between 1..8</exception>
        /// <exception cref="InvalidOperationException">cannot handle amount of bytes parameter with amount long value parameter</exception>
        public static byte[]? AmountToNByteArray(long p_l_amount, int p_i_nBytes)
        {
            if (p_l_amount < 0)
            {
                throw new ArgumentException("Amount parameter must be a positive long value");
            }

            if ((p_i_nBytes < 1) || (p_i_nBytes > 8))
            {
                throw new ArgumentException("N bytes parameter must be between 1..8");
            }

            if ((p_i_nBytes == 1) && (p_l_amount > 255))
            {
                throw new InvalidOperationException("Can not handle an amount of '" + p_l_amount + "' greater than '255' with '1' byte");
            }

            if ((p_i_nBytes == 2) && (p_l_amount > 65535))
            {
                throw new InvalidOperationException("Can not handle an amount of '" + p_l_amount + "' greater than '65535' with '2' byte");
            }

            if ((p_i_nBytes == 3) && (p_l_amount > 16777215))
            {
                throw new InvalidOperationException("Can not handle an amount of '" + p_l_amount + "' greater than '16777215' with '3' byte");
            }

            if ((p_i_nBytes == 4) && (p_l_amount > 4294967295))
            {
                throw new InvalidOperationException("Can not handle an amount of '" + p_l_amount + "' greater than '4294967295' with '4' byte");
            }

            if ((p_i_nBytes == 5) && (p_l_amount > 1099511627775))
            {
                throw new InvalidOperationException("Can not handle an amount of '" + p_l_amount + "' greater than '1099511627775' with '5' byte");
            }

            if ((p_i_nBytes == 6) && (p_l_amount > 281474976710655))
            {
                throw new InvalidOperationException("Can not handle an amount of '" + p_l_amount + "' greater than '281474976710655' with '6' byte");
            }

            if ((p_i_nBytes == 7) && (p_l_amount > 72057594037927935))
            {
                throw new InvalidOperationException("Can not handle an amount of '" + p_l_amount + "' greater than '72057594037927935' with '7' byte");
            }

            if ((p_i_nBytes == 8) && (p_l_amount > 9223372036854775807))
            {
                throw new InvalidOperationException("Can not handle an amount of '" + p_l_amount + "' greater than '9223372036854775807' with '8' byte");
            }

            byte[] a_return = new byte[p_i_nBytes];
            byte[]? a_longToByteArray = ForestNETLib.Core.Helper.LongToByteArray(p_l_amount);

            for (int i = p_i_nBytes; i > 0; i--)
            {
                if ((a_longToByteArray == null) || ((a_longToByteArray.Length - i) < 0))
                {
                    a_return[p_i_nBytes - i] = (byte)0;
                }
                else
                {
                    a_return[p_i_nBytes - i] = a_longToByteArray[a_longToByteArray.Length - i];
                }
            }

            return a_return;
        }

        /// <summary>
        /// converts a byte array to an string, showing all four bytes
        /// </summary>
        /// <param name="p_a_bytes">byte array, length 1..4 or return value will be empty string</param>
        /// <returns>String value</returns>
        public static string PrintByteArray(byte[]? p_a_bytes)
        {
            return PrintByteArray(p_a_bytes, true);
        }

        /// <summary>
        /// converts a byte array to an string, showing all four bytes
        /// </summary>
        /// <param name="p_a_bytes">byte array, length 1..4 or return value will be empty string</param>
        /// <param name="p_b_fourBytes">true - showing all four bytes, false - showing bytes which are significant (!= 0)</param>
        /// <returns>String value</returns>
        public static string PrintByteArray(byte[]? p_a_bytes, bool p_b_fourBytes)
        {
            string s_return = "";

            if (p_a_bytes != null)
            {
                if ((p_a_bytes.Length < 4) && (p_b_fourBytes))
                {
                    for (int i = p_a_bytes.Length; i < 4; i++)
                    {
                        s_return += "00000000 ";
                    }
                }

                foreach (byte by_byte in p_a_bytes)
                {
                    s_return += Convert.ToString(by_byte, 2).PadLeft(8, '0') + " ";
                }

                if (s_return.Length > 0)
                {
                    s_return = s_return.Substring(0, s_return.Length - 1);
                }
            }

            return s_return;
        }

        /// <summary>
        /// prints a generic list's elements surrounded by '[' + ']' and separated by ','
        /// </summary>
        /// <param name="p_a_list">generic list</param>
        /// <returns>String value</returns>
        public static string PrintArrayList<T>(List<T> p_a_list)
        {
            string s_return = "";

            foreach (object? o_element in p_a_list)
            {
                s_return += (o_element?.ToString() ?? "null") + ", ";
            }

            if (s_return.Length > 1)
            {
                s_return = s_return.Substring(0, s_return.Length - 2);
            }

            return "[" + s_return + "]";
        }

        /// <summary>
        /// format a long value which represents high number of bytes to a string without binary prefix (KB, MB, GB, ...)
        /// </summary>
        /// <param name="p_l_bytes">long value</param>
        /// <returns>String value</returns>
        public static string FormatBytes(long p_l_bytes)
        {
            return FormatBytes(p_l_bytes, false);
        }

        /// <summary>
        /// format a long value which represents high number of bytes to a string without binary prefix (KB, MB, GB, ...)
        /// </summary>
        /// <param name="p_l_bytes">long value</param>
        /// <param name="p_b_binaryPrefix">true - binary prefix (KiB, MiB, GiB, ...), false - no binary prefix (KB, BM, GB, ...)</param>
        /// <returns>String value</returns>
        public static string FormatBytes(long p_l_bytes, bool p_b_binaryPrefix)
        {
            bool b_negative = false;

            if (p_l_bytes < 0)
            {
                p_l_bytes *= -1;
                b_negative = true;
            }

            if (p_b_binaryPrefix)
            {
                string[] a_units = new string[] { "B", "KiB", "MiB", "GiB", "TiB", "PiB" };

                if (p_l_bytes == 0)
                {
                    return "0 " + a_units[0];
                }

                double d_bytes = Convert.ToDouble(p_l_bytes);
                double d_foo = Math.Log(p_l_bytes) / Math.Log(1024d);
                d_foo = Math.Floor(d_foo);
                int i = Convert.ToInt32(d_foo);
                d_foo = Math.Pow(1024d, d_foo);
                d_foo = d_bytes / d_foo;

                if (i >= a_units.Length)
                {
                    return (((b_negative) ? "-" : "") + d_foo.ToString("###0.##"));
                }
                else
                {
                    return (((b_negative) ? "-" : "") + d_foo.ToString("###0.##") + " " + a_units[i]);
                }
            }
            else
            {
                string[] a_units = new string[] { "B", "KB", "MB", "GB", "TB", "PB" };

                if (p_l_bytes == 0)
                {
                    return "0 " + a_units[0];
                }

                double d_bytes = Convert.ToDouble(p_l_bytes);
                double d_foo = Math.Log(p_l_bytes) / Math.Log(1000d);
                d_foo = Math.Floor(d_foo);
                int i = Convert.ToInt32(d_foo);
                d_foo = Math.Pow(1000d, d_foo);
                d_foo = d_bytes / d_foo;

                if (i >= a_units.Length)
                {
                    return (((b_negative) ? "-" : "") + d_foo.ToString("###0.##"));
                }
                else
                {
                    return (((b_negative) ? "-" : "") + d_foo.ToString("###0.##") + " " + a_units[i]);
                }
            }
        }

        /// <summary>
        /// Add all bytes from a static byte array to a dynamic byte list
        /// </summary>
        /// <param name="p_a_bytes">static byte array</param>
        /// <param name="p_a_dynamicByteList">(ref) dynamic byte list</param>
        /// <exception cref="ArgumentNullException">static byte array or dynamic byte list parameters is null</exception>
        /// <exception cref="ArgumentException">amount of static bytes is greater than amount of determined fixed bytes</exception>
        public static void AddStaticByteArrayToDynamicByteList(byte[] p_a_bytes, ref List<byte> p_a_dynamicByteList)
        {
            Helper.AddStaticByteArrayToDynamicByteList(p_a_bytes, ref p_a_dynamicByteList, 0);
        }

        /// <summary>
        /// Add all bytes from a static byte array to a dynamic byte list
        /// </summary>
        /// <param name="p_a_bytes">static byte array</param>
        /// <param name="p_a_dynamicByteList">(ref) dynamic byte list</param>
        /// <param name="p_i_fixedAmountOfBytes">determine fixed amount of bytes which should be add to dynamic byte list</param>
        /// <exception cref="ArgumentNullException">static byte array or dynamic byte list parameters is null</exception>
        /// <exception cref="ArgumentException">amount of static bytes is greater than amount of determined fixed bytes</exception>
        public static void AddStaticByteArrayToDynamicByteList(byte[] p_a_bytes, ref List<byte> p_a_dynamicByteList, int p_i_fixedAmountOfBytes)
        {
            if ((p_a_bytes == null) || (p_a_bytes.Length < 1))
            {
                throw new ArgumentNullException(nameof(p_a_bytes), "Static byte array parameter is null or has no elements");
            }

            if (p_a_dynamicByteList == null)
            {
                throw new ArgumentNullException(nameof(p_a_dynamicByteList), "Dynamic byte list parameter is null");
            }

            if (p_i_fixedAmountOfBytes > 0)
            {
                if (p_a_bytes.Length > p_i_fixedAmountOfBytes)
                {
                    throw new ArgumentException("Amount of static bytes '" + p_a_bytes.Length + "' is greater than amount of determined fixed bytes '" + p_i_fixedAmountOfBytes + "'");
                }

                for (int i = 0; i < (p_i_fixedAmountOfBytes - p_a_bytes.Length); i++)
                {
                    p_a_dynamicByteList.Add((byte)0);
                }
            }

            foreach (byte by in p_a_bytes)
            {
                p_a_dynamicByteList.Add(by);
            }
        }

        /// <summary>
        /// hash byte array with hash-algorithm
        /// </summary>
        /// <param name="p_s_algorithm">hash-algorithm: 'SHA-256', 'SHA-384', 'SHA-512'</param>
        /// <param name="p_a_bytes">byte array</param>
        /// <returns>String value</returns>
        /// <exception cref="ArgumentException">if hash-algorithm is not 'SHA-256', 'SHA-384' or 'SHA-512'</exception>
        public static string? HashByteArray(string p_s_algorithm, byte[]? p_a_bytes)
        {
            if (p_a_bytes == null)
            {
                return null;
            }

            if (p_s_algorithm.Equals("SHA-256"))
            {
                return string.Concat(System.Security.Cryptography.SHA256.HashData(p_a_bytes).Select(x => x.ToString("X2")));
            }
            else if (p_s_algorithm.Equals("SHA-384"))
            {
                return string.Concat(System.Security.Cryptography.SHA384.HashData(p_a_bytes).Select(x => x.ToString("X2")));
            }
            else if (p_s_algorithm.Equals("SHA-512"))
            {
                return string.Concat(System.Security.Cryptography.SHA512.HashData(p_a_bytes).Select(x => x.ToString("X2")));
            }
            else
            {
                throw new ArgumentException("Invalid algorithm '" + p_s_algorithm + "', please use a valid algorithm['" + string.Join("', '", (new string[] { "SHA-256", "SHA-384", "SHA-512" })) + "']");
            }
        }

        /// <summary>
        /// convert byte array to hex string
        /// </summary>
        /// <param name="p_a_hashbytes">byte array</param>
        /// <param name="p_b_printEachByteAsHex">true - print each byte as hex value</param>
        /// <returns>String value</returns>
        public static string BytesToHexString(byte[] p_a_hashbytes, bool p_b_printEachByteAsHex)
        {
            if (!p_b_printEachByteAsHex)
            {
                return BitConverter.ToString(p_a_hashbytes).Replace("-", "").Trim();
            }

            string s_foo = "";

            foreach (byte by_hashbyte in p_a_hashbytes)
            {
                s_foo += "0x" + by_hashbyte.ToString("X2") + " ";
            }

            s_foo = s_foo.Substring(0, s_foo.Length - 1);

            return s_foo.Trim();
        }

        /// <summary>
        /// converts a hex string in format '0x7A 0x5', '0x7A0x5', '0X7A 0x5', '0X7A0X5' or '7A5' to a byte array
        /// </summary>
        /// <param name="p_s_string">hex string parameter</param>
        /// <returns>converted byte array</returns>
        /// <exception cref="ArgumentException">hex string parameter has not an even length of values</exception>
        /// <exception cref="FormatException">invalid hex string found and could not parse to a byte value</exception>
        public static byte[] HexStringToBytes(string p_s_string)
        {
            /* replace all whitepaces and writing of hex values to get the single values only */
            p_s_string = p_s_string.Replace(" ", "").Replace("0x", "").Replace("0X", "");

            /* check if our hex string has an even length and not '0x7A 0x5' or '7A5' */
            if (p_s_string.Length % 2 != 0)
            {
                throw new ArgumentException("Hex string has not an even length of values: " + p_s_string.Length);
            }

            /* prepare our return byte array */
            byte[] a_return = new byte[p_s_string.Length / 2];
            /* index for our return byte array in upcoming loop */
            int j = 0;

            /* iterate hex string */
            for (int i = 0; i < p_s_string.Length; i++)
            {
                /* get hex string byte value with two characters, incrementing i while that */
                string s_foo = "" + p_s_string[i] + p_s_string[++i];

                try
                {
                    /* parse hex string to byte value */
                    byte by_foo = byte.Parse(s_foo, System.Globalization.NumberStyles.HexNumber);
                    /* add byte to our return byte array */
                    a_return[j++] = by_foo;
                }
                catch (Exception o_exc)
                {
                    /* invalid hex string found */
                    throw new FormatException("Invalid hex string found '" + s_foo + "' - " + o_exc.Message);
                }
            }

            /* return byte array */
            return a_return;
        }

        /// <summary>
        /// concatenate all generic list elements within a string, separated by delimiter char
        /// </summary>
        /// <param name="p_a_list">generic list</param>
        /// <param name="p_c_delimiter">delimiter</param>
        /// <returns>integer index of search object, -1 if object has not been found</returns>
        public static string JoinList<T>(List<T> p_a_list, char p_c_delimiter)
        {
            String s_foo = "";

            /* check if list is not null and has at least one element */
            if ((p_a_list == null) || (p_a_list.Count < 1))
            {
                return s_foo;
            }

            /* concatenate all list elements with delimiter between */
            foreach (T o_foo in p_a_list)
            {
                s_foo += (o_foo?.ToString() ?? "null") + p_c_delimiter;
            }

            /* check if concatenated string ends with delimiter */
            if (s_foo.EndsWith("" + p_c_delimiter))
            {
                /* delete last delimiter */
                s_foo = s_foo.Substring(0, s_foo.Length - 1);
            }

            /* return concatenated string */
            return s_foo;
        }

        /// <summary>
        /// get index of object in a generic list, not duplicate safe
        /// </summary>
        /// <param name="p_a_list">generic list</param>
        /// <param name="p_o_search">object to be searched</param>
        /// <returns>integer index of search object, -1 if object has not been found</returns>
        /// <exception cref="ArgumentNullException">search parameter object is null, which is not supported</exception>
        public static int GetIndexOfObjectInList<T>(List<T> p_a_list, T p_o_search)
        {
            int i_index = -1;
            bool b_found = false;

            if (p_o_search == null)
            {
                throw new ArgumentNullException(nameof(p_o_search), "We cannot search for 'null' object in list");
            }

            if (p_a_list != null)
            {
                foreach (T o_foo in p_a_list)
                {
                    i_index++;

                    if (o_foo == null)
                    {
                        continue;
                    }

                    if (o_foo.Equals(p_o_search))
                    {
                        b_found = true;
                        break;
                    }
                }

                if (!b_found)
                {
                    i_index = -1;
                }
            }

            return i_index;
        }

        /// <summary>
        /// check if an index is valid for a generic list
        /// </summary>
        /// <param name="p_a_list">generic list</param>
        /// <param name="p_i_index">integer index</param>
        /// <returns>true - parameter index is valid, false - parameter index is not valid</returns>
        /// <exception cref="ArgumentException"></exception>
        public static bool IsIndexValid<T>(List<T> p_a_list, int p_i_index)
        {
            if (p_a_list == null)
            {
                throw new ArgumentException("List parameter is null.");
            }

            if ((p_i_index > -1) && (p_i_index < p_a_list.Count))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// format a TimeSpan object value to a string '(-)HH:mm:ss'
        /// </summary>
        /// <param name="p_o_timeSpan">TimeSpan value</param>
        /// <returns>TimeSpan string value</returns>
        /// <exception cref="ArgumentException"></exception>
        public static string FormatTimeSpan(TimeSpan p_o_timeSpan)
        {
            long l_seconds = Convert.ToInt64(p_o_timeSpan.TotalSeconds);
            long l_absSeconds = Math.Abs(l_seconds);

            string s_positive = string.Format(
                "{0}:{1}:{2}",
                l_absSeconds / 3600 / 60,
                l_absSeconds / 3600 % 60,
                l_absSeconds % 3600 / 60
            );

            return ((l_seconds < 0) ? ("-" + s_positive) : s_positive);
        }

        /// <summary>
        /// generates a random string using ALPHANUMERIC_CHARACTERS
        /// </summary>
        /// <param name="p_i_length">length of the generated random string</param>
        /// <param name="p_s_validCharacters">a string of all valid characters which should be used within random generation</param>
        /// <returns>String</returns>
        public static string GenerateRandomString(int p_i_length)
        {
            return GenerateRandomString(p_i_length, ALPHANUMERIC_CHARACTERS);
        }

        /// <summary>
        /// generates a random string
        /// you can use Helper constants like ALPHANUMERIC_CHARACTERS, UPPERCASE_CHARACTERS, LOWERCASE_CHARACTERS or DIGITS_CHARACTERS to specify which characters should be used
        /// </summary>
        /// <param name="p_i_length">length of the generated random string</param>
        /// <param name="p_s_validCharacters">a string of all valid characters which should be used within random generation</param>
        /// <returns>String</returns>
        public static string GenerateRandomString(int p_i_length, string p_s_validCharacters)
        {
            /* create a buffer for secure generate characters */
            char[] a_buffer = new char[p_i_length];
            /* create an array of valid characters for secure generation */
            char[] a_validCharacters = p_s_validCharacters.ToCharArray();

            /* for each character which should be generate to length p_i_length */
            for (int i = 0; i < a_buffer.Length; i++)
            {
                /* generate a new character out of our valid character array */
                a_buffer[i] = a_validCharacters[SecureRandomIntegerRange(0, a_validCharacters.Length - 1)];
            }

            /* return generate character array buffer as string */
            return new string(a_buffer);
        }

        /// <summary>
        /// generates a random uuid string with 32 hexadecimal characters and 4 hyphens '-'
        /// </summary>
        /// <returns>String</returns>
        public static string GenerateUUID()
        {
            string s_foo = GenerateRandomString(32, string.Concat(DIGITS_CHARACTERS, LOWERCASE_CHARACTERS.AsSpan(0, 6)));
            return s_foo.Substring(0, 8) + "-" + s_foo.Substring(8, 4) + "-" + s_foo.Substring(12, 4) + "-" + s_foo.Substring(16, 4) + "-" + s_foo.Substring(20);
        }

        /// <summary>
        /// disguise a substring within a string, e.g. 'ftps://user:password@example.com:21' to 'ftps://+++++++++++++@example.com:21'
        /// but only the occurrence of first index of start value and last index of end value
        /// </summary>
        /// <param name="p_s_value">string where a part of will be disguised</param>
        /// <param name="p_s_start">recognisable string value where disguise should start after it</param>
        /// <param name="p_s_end">recognisable string value where disguise should end</param>
        /// <param name="p_c_disguiseCharacter">disguise character which will replace normal characters in found substring</param>
        /// <returns>string with disguised substring</returns>
        /// <exception cref="ArgumentException">string parameters are null or empty</exception>
        public static string DisguiseSubstring(string p_s_value, string p_s_start, string p_s_end, char p_c_disguiseCharacter)
        {
            string s_return = "";

            if (IsStringEmpty(p_s_value))
            {
                throw new ArgumentException("String value where substring should be disguised is empty or null");
            }

            if (IsStringEmpty(p_s_start))
            {
                throw new ArgumentException("String value where start of substring is recognized is empty or null");
            }

            if (IsStringEmpty(p_s_end))
            {
                throw new ArgumentException("String value where end of substring is recognized is empty or null");
            }

            /* get index where start of substring is recognized */
            int i_startDisguise = p_s_value.IndexOf(p_s_start) + p_s_start.Length;
            /* get index where end of substring is recognized */
            int i_endDisguise = p_s_value.LastIndexOf(p_s_end);

            /* iterate each character in string parameter */
            for (int i = 0; i < p_s_value.Length; i++)
            {
                if ((i < i_startDisguise) || (i >= i_endDisguise))
                { /* if we are not within substring, just assume normal characters of string */
                    s_return += p_s_value[i];
                }
                else
                { /* if we are within substring, replace normal characters of string with disguise character */
                    s_return += p_c_disguiseCharacter;
                }
            }

            /* return string with disguised substring */
            return s_return;
        }

        /// <summary>
        /// checks if a given string matches an ipv4 address
        /// </summary>
        /// <param name="p_s_string">String parameter variable</param>
        /// <returns>true - String matches IPv4, false - String not matches IPv4</returns>
        public static bool IsIpv4Address(string p_s_string)
        {
            return MatchesRegex(p_s_string, "^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");
        }

        /// <summary>
        /// checks if a given string matches an ipv4 address with suffix
        /// </summary>
        /// <param name="p_s_string">String parameter variable</param>
        /// <returns>true - String matches IPv4 with suffix, false - String not matches IPv4 with suffix</returns>
        public static bool IsIpv4AddressWithSuffix(string p_s_string)
        {
            return MatchesRegex(p_s_string, "^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)/(?:[0-9]|1[0-9]|2[0-9]|3[0-2]?)$");
        }

        /// <summary>
        /// convert an ipv4 address to integer value
        /// </summary>
        /// <param name="p_s_ipv4">String parameter variable with ipv4 address as value</param>
        /// <returns>converted integer value</returns>
        /// <exception cref="ArgumentException">parameter value is null or empty or invalid ipv4 address value</exception>
        public static uint Ipv4ToInt(string p_s_ipv4)
        {
            if (IsStringEmpty(p_s_ipv4))
            {
                throw new ArgumentException("Parameter value for IPv4 address to be checked is empty or null");
            }

            if (!IsIpv4Address(p_s_ipv4))
            {
                throw new ArgumentException("IPv4 parameter value '" + p_s_ipv4 + "' is not a valid IPv4 address");
            }

            string[] a_ipOctets = p_s_ipv4.Split(".");

            uint i_fourth = (uint)((Convert.ToByte(a_ipOctets[0]) << 24) & 0xFF000000);
            uint i_third = (uint)((Convert.ToByte(a_ipOctets[1]) << 16) & 0x00FF0000);
            uint i_second = (uint)((Convert.ToByte(a_ipOctets[2]) << 8) & 0x0000FF00);
            uint i_first = (uint)((Convert.ToByte(a_ipOctets[3]) << 0) & 0x000000FF);

            return i_fourth + i_third + i_second + i_first;
        }

        /// <summary>
        /// get lowest and highest ipv4 address(as integers) from subnet(ipv4 address with suffix) parameter, excluding net- and broadcast-address
        /// </summary>
        /// <param name="p_s_subnet">subnet as ipv4 address with suffix</param>
        /// <returns>two integer values as array, first element is lowest ip, seconds element is highest ip</returns>
        /// <exception cref="ArgumentException">parameter value is null or empty, invalid ipv4 address value or invalid suffix value</exception>
        public static uint[] GetRangeOfSubnet(string p_s_subnet)
        {

            uint[] a_return = new uint[2];

            /* check parameter value */
            if (IsStringEmpty(p_s_subnet))
            {
                throw new ArgumentException("Parameter value for IPv4 address with CIDR part is empty or null");
            }

            /* check parameter notation */
            if (!p_s_subnet.Contains('/'))
            {
                throw new ArgumentException("Invalid parameter value '" + p_s_subnet + "' for IPv4 address with Suffix part is empty or null");
            }

            /* split parameter value into ip and suffix parts */
            string[] a_cidrParts = p_s_subnet.Split("/");
            string s_ipv4 = a_cidrParts[0];
            string s_suffix = a_cidrParts[1];

            /* check ip part */
            if (!IsIpv4Address(s_ipv4))
            {
                throw new ArgumentException("IPv4 part '" + s_ipv4 + "' is not a valid IPv4 address");
            }

            /*  check suffix value */
            if (!IsInteger(s_suffix))
            {
                throw new ArgumentException("Suffix part '" + s_suffix + "' is not an integer");
            }

            /* check min. and max. range of suffix value */
            if ((int.Parse(s_suffix) < 0) || (int.Parse(s_suffix) > 32))
            {
                throw new ArgumentException("Suffix part '" + s_suffix + "' must be between '0..32'");
            }

            /* convert ip part to int */
            uint i_ip = Ipv4ToInt(s_ipv4);

            /* convert suffix part to netmask integer value */
            uint i_mask = uint.MaxValue << 32 - int.Parse(s_suffix);

            /* calculate lowest ip value, excluding net address */
            a_return[0] = (i_ip & i_mask) + 1;

            /* calculate highest ip value, excluding broadcast address */
            a_return[1] = a_return[0] + (~i_mask) - 2;

            return a_return;
        }

        /// <summary>
        /// convert ipv4 address from integer value to string value
        /// </summary>
        /// <param name="p_i_ipv4">Integer parameter variable with ipv4 as value</param>
        /// <returns>ipv4 address as string value or null if conversion failed</returns>
        public static string? Ipv4IntToString(uint p_i_ipv4)
        {
            byte[] by_ip = new byte[4];
            by_ip[0] = (byte)((p_i_ipv4 >> 24) & 0xFF);
            by_ip[1] = (byte)((p_i_ipv4 >> 16) & 0xFF);
            by_ip[2] = (byte)((p_i_ipv4 >> 8) & 0xFF);
            by_ip[3] = (byte)((p_i_ipv4 >> 0) & 0xFF);

            try
            {
                return new System.Net.IPAddress(by_ip).ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// check if ipv4 address is within range of a subnet
        /// </summary>
        /// <param name="p_s_ipv4">ipv4 address which will be checked</param>
        /// <param name="p_s_subnet">subnet as ipv4 address with suffix</param>
        /// <returns>true - ipv4 address is within subnet, false - ipv4 address is not within subnet</returns>
        /// <exception cref="ArgumentException">parameter value is null or empty, invalid ipv4 address value or invalid suffix value</exception>
        public static bool IsIpv4WithinRange(string p_s_ipv4, string p_s_subnet)
        {
            bool b_foo = false;

            /* check ipv4 address */
            uint i_ip = Ipv4ToInt(p_s_ipv4);

            /* check if subnet parameter is not empty */
            if (IsStringEmpty(p_s_subnet))
            {
                throw new ArgumentException("String value for IPv4 address with Suffix part is empty or null");
            }

            if (p_s_subnet.Contains('/'))
            {
                /* found suffix part, so we can calculate first and last ipv4 address */
                uint[] a_ipRange = GetRangeOfSubnet(p_s_subnet);
                b_foo = ((i_ip >= a_ipRange[0]) && (i_ip <= a_ipRange[1]));
            }
            else
            {
                /* no suffix part, so both ipv4 parameters must match as integers */
                if (i_ip == Ipv4ToInt(p_s_subnet))
                {
                    b_foo = true;
                }
            }

            return b_foo;
        }

        /// <summary>
        /// shuffles a generic list based on the Fisher-Yates shuffle
        /// </summary>
        /// <typeparam name="T">type of elements within list parameter</typeparam>
        /// <param name="p_a_list">generic list parameter</param>
        public static void ShuffleList<T>(IList<T> p_a_list)
        {
            /* set help variables */
            int i_result;
            int i_min = 0;
            int i_max = p_a_list.Count;

            /* do until end counter is equal or lower one */
            while (i_max > 1)
            {
                /* decrement end counter */
                i_max--;

                /* prepare byte array of four bytes -> int */
                byte[] a_byte = new byte[4];

                do
                {
                    /* generate random byte values for all 32 bits */
                    ForestNETLib.Core.Global.Instance.RandomNumberGenerator.GetBytes(a_byte);
                    /*using (var o_foo = System.Security.Cryptography.RandomNumberGenerator.Create())
                        o_foo.GetBytes(a_byte);*/

                    /* convert our random bytes to integer value; at this moment the random value is to big because of all 32 bits */
                    i_result = ByteArrayToInt(a_byte);

                    /* amount of bits which are interesting, considerung max. value */
                    int i_bits = 0;

                    /* iterate all 31 - 0 bits, not 32 because we are not supporting unsigned integer here */
                    for (int i = 0; i < 32; i++)
                    {
                        int i_foo;

                        if (i == 31) /* signed integer boundary */
                        {
                            i_foo = PowIntegers(2, i) - 1;
                        }
                        else
                        {
                            i_foo = PowIntegers(2, i);
                        }

                        /* if we have exceed our max. value, we have our amount of bits which are interesting for us */
                        if (i_foo > i_max)
                        {
                            /* remember amout of bits */
                            i_bits = i;
                            break;
                        }
                    }

                    /* mask variable */
                    int i_mask = 0;

                    /* set bits in our mask variable by amount of bits which are interesting for us */
                    for (int i = 0; i < i_bits; i++)
                    {
                        i_mask = (i_mask & 0x7FFF_FFFF) << 1;
                        i_mask++;
                    }

                    /* now use maks on our random value to reduce it to our desired range */
                    i_result &= i_mask;

                    /* check if our random value is in our desired range, otherwise do again */
                } while ((i_result < i_min) || (i_result > i_max));

                /* swap last element with element at random number */
                T o_value = p_a_list[i_result];
                p_a_list[i_result] = p_a_list[i_max];
                p_a_list[i_max] = o_value;
            }
        }
    }
}
