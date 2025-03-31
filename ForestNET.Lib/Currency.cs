namespace ForestNET.Lib
{
    /// <summary>
    /// Class to handle currency values consistent.
    /// </summary>
    public class Currency
    {

        /* Enumerations */

        /// <summary>
        /// currency description enumeration
        /// </summary>
        public enum CurrencyDescription
        {
            EUR, USD, GBP, YEN
        }

        /* Constants */

        /// <summary>
        /// currency symbols matching to currency description enumeration
        /// </summary>
        public static readonly string[] CurrencySymbols = { "€", "$", "£", "¥" };

        /* Fields */

        private decimal dec_decimal;
        private CurrencyDescription e_description;
        private System.Globalization.CultureInfo? o_cultureInfo;
        private char c_decimalSeparator;
        private char c_groupSeparator;
        private System.Globalization.NumberFormatInfo? o_numberFormatInfo;
        private int i_minimumIntegerDigits;
        private int i_minimumFractionDigits;

        /* Properties */

        public decimal Value
        {
            get
            {
                return this.dec_decimal;
            }
            set
            {
                this.UpdateCurrencySettings(value, this.Description, this.CultureInfo, this.DecimalSeparator, this.GroupSeparator, this.MinimumIntegerDigits, this.MinimumFractionDigits);
            }
        }
        public CurrencyDescription Description
        {
            get
            {
                return e_description;
            }
            set
            {
                this.UpdateCurrencySettings(this.Value, value, this.CultureInfo, this.DecimalSeparator, this.GroupSeparator, this.MinimumIntegerDigits, this.MinimumFractionDigits);
            }
        }
        public System.Globalization.CultureInfo? CultureInfo
        {
            get
            {
                return this.o_cultureInfo;
            }
            set
            {
                this.UpdateCurrencySettings(this.Value, this.Description, value, ' ', ' ', this.MinimumIntegerDigits, this.MinimumFractionDigits);
            }
        }
        public char DecimalSeparator
        {
            get
            {
                return this.c_decimalSeparator;
            }
            set
            {
                this.UpdateCurrencySettings(this.Value, this.Description, this.CultureInfo, value, this.GroupSeparator, this.MinimumIntegerDigits, this.MinimumFractionDigits);
            }
        }
        public char GroupSeparator
        {
            get
            {
                return this.c_groupSeparator;
            }
            set
            {
                this.UpdateCurrencySettings(this.Value, this.Description, this.CultureInfo, this.DecimalSeparator, value, this.MinimumIntegerDigits, this.MinimumFractionDigits);
            }
        }
        public int MinimumIntegerDigits
        {
            get
            {
                return this.i_minimumIntegerDigits;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Minimunm intger digits '" + value + "' must be a positive integer number");
                }

                this.UpdateCurrencySettings(this.Value, this.Description, this.CultureInfo, this.DecimalSeparator, this.GroupSeparator, value, this.MinimumFractionDigits);
            }
        }
        public int MinimumFractionDigits
        {
            get
            {
                return this.i_minimumFractionDigits;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Minimunm fraction digits '" + value + "' must be a positive integer number");
                }

                this.UpdateCurrencySettings(this.Value, this.Description, this.CultureInfo, this.DecimalSeparator, this.GroupSeparator, this.MinimumIntegerDigits, value);

            }
        }
        public bool UseCurrencySymbol { get; set; }

        /* Methods */

        /// <summary>
        /// creates currency object with default value 0.0.
        /// </summary>
        /// <param name="p_e_description">currency description enumeration parameter</param>
        /// <exception cref="ArgumentException">MinimumIntegerDigits or MinimumFractionDigits are not a positive integer number</exception>
        public Currency(CurrencyDescription p_e_description) : this(0.0m, p_e_description)
        {

        }

        /// <summary>
        /// creates currency object with german group('.') and decimal(',') separator.
        /// </summary>
        /// <param name="p_d_value">double value of currency object</param>
        /// <param name="p_e_description">currency description enumeration parameter</param>
        /// <exception cref="ArgumentException">MinimumIntegerDigits or MinimumFractionDigits are not a positive integer number</exception>
        public Currency(Decimal p_o_value, CurrencyDescription p_e_description)
        {
            this.UpdateCurrencySettings(p_o_value, p_e_description, null, ',', '.', 1, 2);
        }

        /// <summary>
        /// creates currency object with default value 0.0 and culture info parameter.
        /// </summary>
        /// <param name="p_e_description">currency description enumeration parameter</param>
        /// <param name="p_o_cultureInfo">culture info parameter</param>
        /// <exception cref="ArgumentException">MinimumIntegerDigits or MinimumFractionDigits are not a positive integer number</exception>
        public Currency(CurrencyDescription p_e_description, System.Globalization.CultureInfo? p_o_cultureInfo)
        {
            this.UpdateCurrencySettings(0.0m, p_e_description, p_o_cultureInfo, ' ', ' ', 1, 2);
        }

        /// <summary>
        /// creates currency object with culture info parameter.
        /// </summary>
        /// <param name="p_d_value">double value of currency object</param>
        /// <param name="p_e_description">currency description enumeration parameter</param>
        /// <param name="p_o_cultureInfo">culture info parameter</param>
        /// <exception cref="ArgumentException">MinimumIntegerDigits or MinimumFractionDigits are not a positive integer number</exception>
        public Currency(Decimal p_o_value, CurrencyDescription p_e_description, System.Globalization.CultureInfo? p_o_cultureInfo)
        {
            this.UpdateCurrencySettings(p_o_value, p_e_description, p_o_cultureInfo, ' ', ' ', 1, 2);
        }

        /// <summary>
        /// private update method to change all currency settings
        /// </summary>
        /// <param name="p_o_value">decimal value of currency object</param>
        /// <param name="p_e_description">currency description enumeration parameter</param>
        /// <param name="p_o_cultureInfo">currency culture info setting</param>
        /// <param name="p_c_decimalSeparator">character for decimal separation</param>
        /// <param name="p_c_groupSeparator">character for group separation</param>
        /// <param name="p_i_minimumIntegerDigits">integer value for minimum digits</param>
        /// <param name="p_i_minimumFractionDigits">integer value for minimum fraction digits</param>
        /// <exception cref="ArgumentException">MinimumIntegerDigits or MinimumFractionDigits are not a positive integer number</exception>
        private void UpdateCurrencySettings(
            Decimal p_o_value,
            CurrencyDescription p_e_description,
            System.Globalization.CultureInfo? p_o_cultureInfo,
            char p_c_decimalSeparator,
            char p_c_groupSeparator,
            int p_i_minimumIntegerDigits,
            int p_i_minimumFractionDigits
        )
        {
            this.dec_decimal = p_o_value;
            this.e_description = p_e_description;
            this.o_cultureInfo = p_o_cultureInfo;
            this.c_decimalSeparator = p_c_decimalSeparator;
            this.c_groupSeparator = p_c_groupSeparator;
            this.i_minimumIntegerDigits = p_i_minimumIntegerDigits;
            this.i_minimumFractionDigits = p_i_minimumFractionDigits;
            this.UseCurrencySymbol = false;

            if (this.o_cultureInfo != null)
            {
                this.o_numberFormatInfo = this.o_cultureInfo.NumberFormat;
                this.c_decimalSeparator = this.o_numberFormatInfo.NumberDecimalSeparator[0];
                this.c_groupSeparator = this.o_numberFormatInfo.NumberGroupSeparator[0];
                this.i_minimumFractionDigits = this.o_numberFormatInfo.NumberDecimalDigits;
            }
            else
            {
                this.o_numberFormatInfo = new System.Globalization.NumberFormatInfo
                {
                    CurrencyDecimalSeparator = this.c_decimalSeparator.ToString(),
                    CurrencyGroupSeparator = this.c_groupSeparator.ToString(),
                    NumberDecimalSeparator = this.c_decimalSeparator.ToString(),
                    NumberGroupSeparator = this.c_groupSeparator.ToString(),

                    CurrencyDecimalDigits = p_i_minimumFractionDigits,
                    NumberDecimalDigits = p_i_minimumFractionDigits
                };
            }
        }

        override public string ToString()
        {
            return this.ToString(MidpointRounding.ToEven);
        }

        public string ToString(MidpointRounding p_o_midpointRounding)
        {
            if (this.i_minimumIntegerDigits != 1) /* number format info does not support minimum integer digits before decimal separator, so we must do our own */
            {
                /* part of format string */
                string s_minimumDigits = "0";
                string s_minimumFractions = "0";

                /* get minimum integer digits for format string */
                for (int i = 1; i < this.i_minimumIntegerDigits - 1; i++)
                {
                    s_minimumDigits += "0";
                }

                /* get minimum fraction digits for format string */
                for (int i = 1; i < this.i_minimumFractionDigits; i++)
                {
                    s_minimumFractions += "0";
                }

                /* create our format string */
                string s_format = "{0:0," + s_minimumDigits + "." + s_minimumFractions + "}";
                /* create our string output, using our format string with invariant culture, and rounding our decimal value */
                /* additionaly we replace ',' and '.' with very rarely used replacements, so it is easier to change ',' to '.' and after that '.' to ','  */
                string s_foo = String.Format(s_format, Math.Round(this.dec_decimal, this.i_minimumFractionDigits, p_o_midpointRounding), System.Globalization.CultureInfo.InvariantCulture).Replace(",", "_,_").Replace(".", "_._");
                /* replace our group and decimal separators with our class settings */
                s_foo = s_foo.Replace("_._", this.c_groupSeparator.ToString()).Replace("_,_", this.c_decimalSeparator.ToString());
                /* return our value as string with currency symbol or currency description */
                return s_foo + " " + ((this.UseCurrencySymbol) ? Currency.CurrencySymbols[(int)this.e_description] : this.e_description.ToString());
            }
            else
            {
                /* return our value with number format info instance and rounding with currency symbol or currency description */
                return Math.Round(this.dec_decimal, this.i_minimumFractionDigits, p_o_midpointRounding).ToString("N", this.o_numberFormatInfo) + " " + ((this.UseCurrencySymbol) ? Currency.CurrencySymbols[(int)this.e_description] : this.e_description.ToString());
            }
        }
    }
}
