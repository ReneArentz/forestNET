namespace ForestNET.Lib
{
    /// <summary>
    /// Class to represent a date interval from string input.
    /// Can transform object to readable string, multi-language capable.
    /// Transformation to millisecond duration value available.
    /// </summary>
    public class DateInterval
    {

        /* Fields */

        private int y;
        private int m;
        private int d;
        private int h;
        private int i;
        private int s;

        /* Properties */

        public System.Globalization.CultureInfo? CultureInfo { get; set; }

        /* Methods */

        /// <summary>
        /// creates date interval object
        /// </summary>
        /// <param name="p_s_dateInterval">String encoded date interval e.g. P1Y2M3DT4H5M6S</param>
        /// <exception cref="">IllegalArgumentException</exception>
        public DateInterval(string? p_s_dateInterval) : this(p_s_dateInterval, null)
        {

        }

        /// <summary>
        /// creates date interval object
        /// </summary>
        /// <param name="p_s_dateInterval">String encoded date interval e.g. P1Y2M3DT4H5M6S</param>
        /// <param name="p_o_cultureInfo">culture info object for language settings</param>
        /// <exception cref="">ArgumentException</exception>
        public DateInterval(string? p_s_dateInterval, System.Globalization.CultureInfo? p_o_cultureInfo)
        {
            this.y = 0;
            this.m = 0;
            this.d = 0;
            this.h = 0;
            this.i = 0;
            this.s = 0;

            this.CultureInfo = p_o_cultureInfo;

            /* check if string parameter has a value */
            if ((Helper.IsStringEmpty(p_s_dateInterval)) || (p_s_dateInterval == null))
            {
                throw new ArgumentException("String encoded date interval parameter is null or empty.");
            }

            /* check if string parameter is a valid date interval format string */
            if (!Helper.IsDateInterval(p_s_dateInterval))
            {
                throw new ArgumentException("Parameter[" + p_s_dateInterval + "] does not match date interval format.");
            }

            /* split date interval parameter into info values */
            string[] a_info = System.Text.RegularExpressions.Regex.Split(p_s_dateInterval, @"\d*");
            System.Collections.Generic.List<string> a_infoList = [];

            /* only take elements which are not empty */
            for (int k = 0; k < a_info.Length; k++)
            {
                if (!Helper.IsStringEmpty(a_info[k]))
                {
                    a_infoList.Add(a_info[k]);
                }
            }

            /* split date interval parameter into digit values */
            string[] a_values = System.Text.RegularExpressions.Regex.Split(p_s_dateInterval, @"[A-Za-z]{1}");
            System.Collections.Generic.List<string> a_valuesList = [];

            /* only take elements which are not empty */
            for (int k = 0; k < a_values.Length; k++)
            {
                if (!Helper.IsStringEmpty(a_values[k]))
                {
                    a_valuesList.Add(a_values[k]);
                }
            }

            int j = 0;
            string s_mode = "";

            /* disassemble each info for setting each date interval value */
            foreach (string s_char in a_infoList)
            {
                switch (s_char)
                {
                    case "P":
                        s_mode = "date";
                        break;
                    case "T":
                        s_mode = "time";
                        break;

                    case "Y":
                        this.y = int.Parse(a_valuesList[j]);
                        j++;
                        break;
                    case "D":
                        this.d = int.Parse(a_valuesList[j]);
                        j++;
                        break;

                    case "H":
                        this.h = int.Parse(a_valuesList[j]);
                        j++;
                        break;
                    case "S":
                        this.s = int.Parse(a_valuesList[j]);
                        j++;
                        break;
                }

                switch (s_char)
                {
                    case "M":
                        if (s_mode == "date")
                        {
                            this.m = int.Parse(a_valuesList[j]);
                            j++;
                        }
                        else if (s_mode == "time")
                        {
                            this.i = int.Parse(a_valuesList[j]);
                            j++;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// returns millisecond duration long number of a date interval object
        /// </summary>
        /// <returns>long value</returns>
        public long ToDuration()
        {
            return
                    this.d * 24 * 60 * 60 * 1000 +
                    this.h * 60 * 60 * 1000 +
                    this.i * 60 * 1000 +
                    this.s * 1000;
        }

        /// <summary>
        /// returns duration long number in seconds of a date interval object
        /// </summary>
        /// <returns>long value</returns>
        public long ToDurationInSeconds()
        {
            return
                    this.d * 24 * 60 * 60 +
                    this.h * 60 * 60 +
                    this.i * 60 +
                    this.s;
        }

        /// <summary>
        /// assume date interval data from another date interval object
        /// </summary>
        /// <param name="p_o_dateInterval">date interval object</param>
        public void SetDateInterval(DateInterval p_o_dateInterval)
        {
            this.y = p_o_dateInterval.y;
            this.m = p_o_dateInterval.m;
            this.d = p_o_dateInterval.d;
            this.h = p_o_dateInterval.h;
            this.i = p_o_dateInterval.i;
            this.s = p_o_dateInterval.s;
        }

        /// <summary>
        /// return date interval object as DE string if culture info parameter is null, other culture settings need to be implemented
        /// </summary>
        /// <returns>String</returns>
        override public string ToString()
        {
            return this.ToString(this.CultureInfo);
        }

        /// <summary>
        /// return date interval object as string
        /// 						must contain 6 elements for 'year', 'month', 'day', 'hour', 'minute', 'second'
        /// </summary>
        /// <param name="p_a_definitions">direct definition of date interval words</param>
        /// <exception cref="ArgumentException">defintion array parameter does not contain 6 elements</exception>
        /// <returns>string</returns>
        public string ToString(string[] p_a_definitions)
        {
            string s_foo = "";

            if (p_a_definitions.Length != 6)
            {
                throw new ArgumentException("Parameter for direct defintion of date interval must contain '6' words.");
            }

            if (this.y != 0)
            {
                s_foo += this.y + " " + p_a_definitions[0] + " ";
            }

            if (this.m != 0)
            {
                s_foo += this.m + " " + p_a_definitions[1] + " ";
            }

            if (this.d != 0)
            {
                s_foo += this.d + " " + p_a_definitions[2] + " ";
            }

            if (this.h != 0)
            {
                s_foo += this.h + " " + p_a_definitions[3] + " ";
            }

            if (this.i != 0)
            {
                s_foo += this.i + " " + p_a_definitions[4] + " ";
            }

            if (this.s != 0)
            {
                s_foo += this.s + " " + p_a_definitions[5] + " ";
            }

            return s_foo.Trim();
        }

        /// <summary>
        /// return date interval object as DE string if culture info parameter is null, other culture settings need to be implemented
        /// </summary>
        /// <param name="p_o_cultureInfo">culture info object for language settings</param>
        /// <returns>string</returns>
        public string ToString(System.Globalization.CultureInfo? p_o_cultureInfo)
        {
            string s_foo = "";

            string[] a_word = new string[6];

            if ((p_o_cultureInfo == null) || (p_o_cultureInfo.DisplayName.StartsWith("de", StringComparison.CurrentCultureIgnoreCase)))
            {
                a_word[0] = "Jahr(e)";
                a_word[1] = "Monat(e)";
                a_word[2] = "Tag(e)";
                a_word[3] = "Stunde(n)";
                a_word[4] = "Minute(n)";
                a_word[5] = "Sekunde(n)";
            }
            else if (p_o_cultureInfo.DisplayName.StartsWith("en", StringComparison.CurrentCultureIgnoreCase))
            {
                a_word[0] = "year(s)";
                a_word[1] = "month(s)";
                a_word[2] = "day(s)";
                a_word[3] = "hour(s)";
                a_word[4] = "minute(s)";
                a_word[5] = "second(s)";
            }

            if (this.y != 0)
            {
                s_foo += this.y + " " + a_word[0] + " ";
            }

            if (this.m != 0)
            {
                s_foo += this.m + " " + a_word[1] + " ";
            }

            if (this.d != 0)
            {
                s_foo += this.d + " " + a_word[2] + " ";
            }

            if (this.h != 0)
            {
                s_foo += this.h + " " + a_word[3] + " ";
            }

            if (this.i != 0)
            {
                s_foo += this.i + " " + a_word[4] + " ";
            }

            if (this.s != 0)
            {
                s_foo += this.s + " " + a_word[5] + " ";
            }

            return s_foo.Trim();
        }
    }
}
