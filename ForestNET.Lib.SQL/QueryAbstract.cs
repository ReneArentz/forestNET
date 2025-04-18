namespace ForestNET.Lib.SQL
{
    /// <summary>
    /// Abstract query class holding all necessary information about operators, filters, aggregations, index constraints, alter operations.
    /// </summary>
    public abstract class QueryAbstract : IQueryAbstract
    {

        /* Fields */

        protected readonly string[] Operators = { "=", "<", "<=", ">", ">=", "<>", "LIKE", "NOT LIKE", "IN", "NOT IN", "IS", "IS NOT" };
        protected readonly string[] FilterOperators = { "AND", "OR", "XOR" };
        protected readonly string[] JoinTypes = { "INNER JOIN", "NATURAL JOIN", "CROSS JOIN", "OUTER JOIN", "LEFT OUTER JOIN", "RIGHT OUTER JOIN", "FULL OUTER JOIN" };
        protected readonly string[] SqlAggregations = { "AVG", "COUNT", "MAX", "MIN", "SUM" };
        protected readonly string[] SqlIndexConstraints = { "UNIQUE", "PRIMARY KEY", "INDEX" };
        protected readonly string[] AlterOperations = { "ADD", "CHANGE", "DROP" };
        public readonly string QuerySeparator = "::forestnetSQLQuerySeparator::";
        public readonly string QueryValueTag = "forestnetSQLValue";
        protected string[] SqlColumnTypes = [];
        protected string[] SqlConstraints = [];

        /* Properties */

        public string? QueryString { get; set; } = null;
        public BaseGateway BaseGateway { get; set; }
        public SqlType SqlType { get; set; }
        public string? Table { get; set; } = null;
        public string GetQuerySeparator { get { return this.QuerySeparator; } }
        public string GetQueryValueTag { get { return this.QueryValueTag; } }

        /* Methods */

        /// <summary>
        /// Abstract constructor with query parameter
        /// </summary>
        /// <param name="p_o_query">query object parameter</param>
        /// <exception cref="Exception">invalid database gateway value from query parameter object</exception>
        public QueryAbstract(IQuery? p_o_query)
        {
            if (p_o_query != null)
            {
                this.BaseGateway = p_o_query.BaseGateway;
                this.SqlType = p_o_query.SqlType;
                this.Table = p_o_query.Table;
                this.Init();
            }
        }

        /// <summary>
        /// Init function declaring sql column types and valid constraints for selected database gateway
        /// </summary>
        /// <exception cref="ArgumentException">invalid database gateway value</exception>
        protected void Init()
        {
            List<string> a_sqlColumnTypes = [];
            List<string> a_sqlConstraints = [];
            bool b_exc = false;

            switch (this.BaseGateway)
            {
                case BaseGateway.MARIADB:
                    a_sqlColumnTypes = ["VARCHAR", "TEXT", "SMALLINT", "INT", "BIGINT", "TIMESTAMP", "TIME", "DOUBLE", "DECIMAL", "BIT"];
                    a_sqlConstraints = ["NULL", "NOT NULL", "UNIQUE", "PRIMARY KEY", "DEFAULT", "INDEX", "AUTO_INCREMENT", "SIGNED", "UNSIGNED"];
                    break;
                case BaseGateway.SQLITE:
                    a_sqlColumnTypes = ["varchar", "text", "smallint", "integer", "bigint", "datetime", "time", "double", "decimal", "bit"];
                    a_sqlConstraints = ["NULL", "NOT NULL", "UNIQUE", "PRIMARY KEY", "DEFAULT", "INDEX", "AUTOINCREMENT"];
                    break;
                case BaseGateway.MSSQL:
                    a_sqlColumnTypes = ["nvarchar", "text", "smallint", "int", "bigint", "datetime", "time", "float", "decimal", "bit"];
                    a_sqlConstraints = ["NULL", "NOT NULL", "UNIQUE", "PRIMARY KEY", "DEFAULT", "INDEX", "IDENTITY(1,1)"];
                    break;
                case BaseGateway.PGSQL:
                    a_sqlColumnTypes = ["varchar", "text", "smallint", "integer", "bigint", "timestamp", "time", "double precision", "decimal", "boolean", "serial"];
                    a_sqlConstraints = ["NULL", "NOT NULL", "UNIQUE", "PRIMARY KEY", "DEFAULT", "INDEX", ""];
                    break;
                case BaseGateway.ORACLE:
                    a_sqlColumnTypes = ["VARCHAR2", "CLOB", "DOUBLE PRECISION", "BINARY_FLOAT", "LONG", "TIMESTAMP", "INTERVAL DAY(0) TO SECOND(0)", "BINARY_DOUBLE", "NUMBER", "CHAR"];
                    a_sqlConstraints = ["NULL", "NOT NULL", "UNIQUE", "PRIMARY KEY", "DEFAULT", "INDEX", ""];
                    break;
                case BaseGateway.NOSQLMDB:
                    a_sqlColumnTypes = ["VARCHAR", "TEXT", "SMALLINT", "INTEGER", "BIGINT", "TIMESTAMP", "TIME", "DOUBLE", "DECIMAL", "BOOL"];
                    a_sqlConstraints = ["NULL", "NOT NULL", "UNIQUE", "PRIMARY KEY", "DEFAULT", "INDEX", "AUTO_INCREMENT"];
                    break;
                default:
                    b_exc = true;
                    break;
            }

            if ((b_exc) || (a_sqlColumnTypes.Count == 0) || (a_sqlConstraints.Count == 0))
            {
                throw new ArgumentException("SQL column types for BaseGateway[" + BaseGateway + "] not implemented");
            }

            /* assume sql column types to local field */
            this.SqlColumnTypes = new string[a_sqlColumnTypes.Count];
            int i = 0;

            foreach (string s_sqlColumnType in a_sqlColumnTypes)
            {
                this.SqlColumnTypes[i++] = s_sqlColumnType;
            }

            /* assume sql constraints to local field */
            this.SqlConstraints = new string[a_sqlConstraints.Count];
            i = 0;

            foreach (string s_sqlConstraint in a_sqlConstraints)
            {
                this.SqlConstraints[i++] = s_sqlConstraint;
            }
        }

        /// <summary>
        /// Method for parsing a value to a query, preventing sql-injection
        /// </summary>
        /// <param name="p_o_value">value object</param>
        /// <returns>object, but in general returning always a string casted as object</returns>
        /// <exception cref="ArgumentException">could not parse DateTime to string or invalid database gateway</exception>
        public Object ParseValue(Object? p_o_value)
        {
            /* temp variables */
            string s_foo = string.Empty;

            /* return flag for string */
            bool b_returnString = false;

            /* if parameter is null we just return 'NULL' string */
            if (p_o_value == null)
            {
                s_foo = "NULL";
                b_returnString = true;
            }

            /* if object parameter is a string or an instance of DateTime */
            if ((p_o_value is string) || (p_o_value is DateTime))
            {
                /* return value as string */
                b_returnString = true;

                if (p_o_value is string)
                { /* if it is a string call ToString object method, if length is 0 then set value to 'NULL' */
                    s_foo = p_o_value.ToString() ?? "";

                    if (s_foo.Length == 0)
                    {
                        s_foo = "NULL";
                    }
                }
                else if (p_o_value is DateTime dt_foo1)
                { /* check if object parameter is instance of DateTime */
                    if (dt_foo1.TimeOfDay == TimeSpan.Zero)
                    {
                        /* create date string */
                        s_foo = dt_foo1.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        /* create datetime string */
                        s_foo = dt_foo1.ToString("dd.MM.yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    }
                }

                bool b_exc = false;

                /* date conversion for sql query [any date] | [any date time] to [yyyy-MM-dd(T|' ')hh:mm:ss] */
                switch (this.BaseGateway)
                {
                    case BaseGateway.MARIADB:
                    case BaseGateway.SQLITE:
                    case BaseGateway.PGSQL:
                    case BaseGateway.NOSQLMDB:
                        if (ForestNET.Lib.Helper.IsDate(s_foo))
                        {
                            DateTime o_localDate = ForestNET.Lib.Helper.FromDateString(s_foo);
                            s_foo = o_localDate.Year.ToString("D4") + "-" + o_localDate.Month.ToString("D2") + "-" + o_localDate.Day.ToString("D2");
                        }
                        else if (ForestNET.Lib.Helper.IsDateTime(s_foo))
                        {
                            DateTime o_localDateTime = ForestNET.Lib.Helper.FromDateTimeString(s_foo);
                            s_foo = o_localDateTime.Year.ToString("D4") + "-" + o_localDateTime.Month.ToString("D2") + "-" + o_localDateTime.Day.ToString("D2") + " " + o_localDateTime.Hour.ToString("D2") + ":" + o_localDateTime.Minute.ToString("D2") + ":" + o_localDateTime.Second.ToString("D2");
                        }
                        break;
                    case BaseGateway.ORACLE:
                        if ((s_foo.StartsWith("01.01.1970 ")) || (ForestNET.Lib.Helper.IsTime(s_foo)))
                        { /* recognize time value */
                            /* cut off '01.01.1970 ' from start */
                            if (s_foo.StartsWith("01.01.1970 "))
                            {
                                s_foo = s_foo.Substring(11);
                            }

                            /* use TO_DSINTERVAL function */
                            return (Object)("TO_DSINTERVAL([" + this.QueryValueTag + "]+0 " + s_foo + "[/" + this.QueryValueTag + "])");
                        }
                        else if (ForestNET.Lib.Helper.IsDate(s_foo))
                        {
                            DateTime o_localDate = ForestNET.Lib.Helper.FromDateString(s_foo);
                            s_foo = o_localDate.Year.ToString("D4") + "-" + o_localDate.Month.ToString("D2") + "-" + o_localDate.Day.ToString("D2");

                            /* use TO_DATE function */
                            return (Object)("TO_DATE([" + this.QueryValueTag + "]" + s_foo + "[/" + this.QueryValueTag + "], 'yyyy-mm-dd')");
                        }
                        else if (ForestNET.Lib.Helper.IsDateTime(s_foo))
                        {
                            DateTime o_localDateTime = ForestNET.Lib.Helper.FromDateTimeString(s_foo);
                            s_foo = o_localDateTime.Year.ToString("D4") + "-" + o_localDateTime.Month.ToString("D2") + "-" + o_localDateTime.Day.ToString("D2") + " " + o_localDateTime.Hour.ToString("D2") + ":" + o_localDateTime.Minute.ToString("D2") + ":" + o_localDateTime.Second.ToString("D2");

                            /* use TO_DATE function */
                            return (Object)("TO_DATE([" + this.QueryValueTag + "]" + s_foo + "[/" + this.QueryValueTag + "], 'yyyy-mm-dd hh24:mi:ss')");
                        }
                        break;
                    case BaseGateway.MSSQL:
                        if (ForestNET.Lib.Helper.IsDate(s_foo))
                        {
                            DateTime o_localDate = ForestNET.Lib.Helper.FromDateTimeString(s_foo);
                            s_foo = o_localDate.Year.ToString("D4") + "-" + o_localDate.Month.ToString("D2") + "-" + o_localDate.Day.ToString("D2");
                        }
                        else if (ForestNET.Lib.Helper.IsDateTime(s_foo))
                        {
                            DateTime o_localDateTime = ForestNET.Lib.Helper.FromDateTimeString(s_foo);
                            s_foo = o_localDateTime.Year.ToString("D4") + "-" + o_localDateTime.Month.ToString("D2") + "-" + o_localDateTime.Day.ToString("D2") + "T" + o_localDateTime.Hour.ToString("D2") + ":" + o_localDateTime.Minute.ToString("D2") + ":" + o_localDateTime.Second.ToString("D2");
                        }
                        break;
                    default:
                        b_exc = true;
                        break;
                }

                if (b_exc)
                {
                    throw new ArgumentException("BaseGateway[" + this.BaseGateway + "] not implemented");
                }
            }

            /* cast TimeSpan value */
            if (p_o_value is TimeSpan ts_foo)
            {
                s_foo = ts_foo.ToString("hh\\:mm\\:ss");

                if (this.BaseGateway == BaseGateway.ORACLE)
                {
                    return (Object)("TO_DSINTERVAL([" + this.QueryValueTag + "]+0 " + s_foo + "[/" + this.QueryValueTag + "])");
                }

                b_returnString = true;
            }

            /* cast bool value */
            if (p_o_value is Boolean b_foo)
            {
                return (Object)("[" + this.QueryValueTag + "]" + b_foo + "[/" + this.QueryValueTag + "]");
            }

            /* cast sbyte value */
            if (p_o_value is SByte sby_foo)
            {
                return (Object)("[" + this.QueryValueTag + "]" + sby_foo + "[/" + this.QueryValueTag + "]");
            }

            /* cast short value */
            if (p_o_value is short sh_foo)
            {
                return (Object)("[" + this.QueryValueTag + "]" + sh_foo + "[/" + this.QueryValueTag + "]");
            }

            /* cast integer value */
            if (p_o_value is int i_foo)
            {
                return (Object)("[" + this.QueryValueTag + "]" + i_foo + "[/" + this.QueryValueTag + "]");
            }

            /* cast long value */
            if (p_o_value is long l_foo)
            {
                return (Object)("[" + this.QueryValueTag + "]" + l_foo + "[/" + this.QueryValueTag + "]");
            }

            /* cast byte value */
            if (p_o_value is Byte by_foo)
            {
                return (Object)("[" + this.QueryValueTag + "][unsignedByte]" + by_foo + "[/unsignedByte][/" + this.QueryValueTag + "]");
            }

            /* cast ushort value */
            if (p_o_value is ushort ush_foo)
            {
                return (Object)("[" + this.QueryValueTag + "][unsignedShort]" + ush_foo + "[/unsignedShort][/" + this.QueryValueTag + "]");
            }

            /* cast uinteger value */
            if (p_o_value is uint ui_foo)
            {
                return (Object)("[" + this.QueryValueTag + "][unsignedInt]" + ui_foo + "[/unsignedInt][/" + this.QueryValueTag + "]");
            }

            /* cast ulong value */
            if (p_o_value is ulong ul_foo)
            {
                return (Object)("[" + this.QueryValueTag + "][unsignedLong]" + ul_foo + "[/unsignedLong][/" + this.QueryValueTag + "]");
            }

            /* cast float value */
            if (p_o_value is Single f_foo)
            {
                System.Globalization.NumberFormatInfo o_numberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.Clone() as System.Globalization.NumberFormatInfo ?? throw new NullReferenceException("could not retrieve number format info from current culture info");
                o_numberFormatInfo.CurrencyGroupSeparator = "";
                o_numberFormatInfo.NumberGroupSeparator = "";
                o_numberFormatInfo.CurrencyDecimalSeparator = ".";
                o_numberFormatInfo.NumberDecimalSeparator = ".";

                return (Object)("[" + this.QueryValueTag + "][forestnetFloat]" + f_foo.ToString(o_numberFormatInfo) + "[/forestnetFloat][/" + this.QueryValueTag + "]");
            }

            /* cast double value */
            if (p_o_value is Double d_foo)
            {
                System.Globalization.NumberFormatInfo o_numberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.Clone() as System.Globalization.NumberFormatInfo ?? throw new NullReferenceException("could not retrieve number format info from current culture info");
                o_numberFormatInfo.CurrencyGroupSeparator = "";
                o_numberFormatInfo.NumberGroupSeparator = "";
                o_numberFormatInfo.CurrencyDecimalSeparator = ".";
                o_numberFormatInfo.NumberDecimalSeparator = ".";

                return (Object)("[" + this.QueryValueTag + "][forestnetDouble]" + d_foo.ToString(o_numberFormatInfo) + "[/forestnetDouble][/" + this.QueryValueTag + "]");
            }

            /* cast double value */
            if (p_o_value is Decimal dec_foo)
            {
                System.Globalization.NumberFormatInfo o_numberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.Clone() as System.Globalization.NumberFormatInfo ?? throw new NullReferenceException("could not retrieve number format info from current culture info");
                o_numberFormatInfo.CurrencyGroupSeparator = "";
                o_numberFormatInfo.NumberGroupSeparator = "";
                o_numberFormatInfo.CurrencyDecimalSeparator = ".";
                o_numberFormatInfo.NumberDecimalSeparator = ".";

                return (Object)("[" + this.QueryValueTag + "][forestnetDecimal]" + dec_foo.ToString(o_numberFormatInfo) + "[/forestnetDecimal][/" + this.QueryValueTag + "]");
            }

            /* return string with value tag */
            if (b_returnString)
            {
                s_foo = s_foo.Replace("[" + this.QueryValueTag + "]", "").Replace("[/" + this.QueryValueTag + "]", "");
                return (Object)("[" + this.QueryValueTag + "]" + s_foo + "[/" + this.QueryValueTag + "]");
            }

            /* return empty string */
            return (Object)"";
        }

        /// <summary>
        /// Abstract ToString function so any query class inheriting from QueryAbstract must have this method
        /// </summary>
        public override abstract string ToString();
    }
}