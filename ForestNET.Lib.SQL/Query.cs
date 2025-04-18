namespace ForestNET.Lib.SQL
{
    /// <summary>
    /// Main query class with table, base gateway and sql type information to create any kind of query to process with a database interface.
	/// [T] Sql query class definition of current class based on QueryAbstract class, e.g. Select extends ForestNET.Lib.SQL.QueryAbstract.
    /// </summary>
    public class Query<T> : IQuery where T : QueryAbstract
    {

        /* Fields */

        private readonly T? o_query = null;

        /* Properties */

        public BaseGateway BaseGateway { get; set; }
        public SqlType SqlType { get; set; }
        public string? Table { get; set; } = null;

        /* Methods */

        /// <summary>
        /// constructor of query class
        /// </summary>
        /// <param name="p_e_base">database gateway enumeration value</param>
        /// <param name="p_e_type">sql type enumeration value</param>
        /// <param name="p_s_table">table name</param>
        /// <exception cref="Exception">invalid database gateway or invalid sql type</exception>
        public Query(BaseGateway p_e_base, SqlType p_e_type, string p_s_table)
        {
            /* take over construct parameters */
            this.BaseGateway = p_e_base;
            this.SqlType = p_e_type;
            this.Table = p_s_table;

            bool b_exc = false;

            /* check base gateway parameter */
            switch (this.BaseGateway)
            {
                case BaseGateway.MARIADB:
                case BaseGateway.SQLITE:
                case BaseGateway.MSSQL:
                case BaseGateway.PGSQL:
                case BaseGateway.ORACLE:
                case BaseGateway.NOSQLMDB:
                    break;
                default:
                    b_exc = true;
                    break;
            }

            if (b_exc)
            {
                throw new ArgumentException("Invalid BaseGateway[" + this.BaseGateway + "]");
            }

            /* create query object */
            switch (this.SqlType)
            {
                case SqlType.SELECT:
                    this.o_query = (T)(QueryAbstract)new Select(this);
                    break;
                case SqlType.INSERT:
                    this.o_query = (T)(QueryAbstract)new Insert(this);
                    break;
                case SqlType.UPDATE:
                    this.o_query = (T)(QueryAbstract)new Update(this);
                    break;
                case SqlType.DELETE:
                    this.o_query = (T)(QueryAbstract)new Delete(this);
                    break;
                case SqlType.TRUNCATE:
                    this.o_query = (T)(QueryAbstract)new Truncate(this);
                    break;
                case SqlType.CREATE:
                    this.o_query = (T)(QueryAbstract)new Create(this);
                    break;
                case SqlType.ALTER:
                    this.o_query = (T)(QueryAbstract)new Alter(this);
                    break;
                case SqlType.DROP:
                    this.o_query = (T)(QueryAbstract)new Drop(this);
                    break;
                default:
                    b_exc = true;
                    break;
            }

            if (b_exc)
            {
                throw new ArgumentException("Invalid SqlType[" + this.SqlType + "]");
            }
        }

        /// <summary>
        ///  create sql string query
        /// </summary>
        public override string ToString()
        {
            if ((this.o_query != null) && (this.o_query.QueryString != null))
            { /* load query string if it is set */
                return this.o_query.QueryString;
            }
            else if (this.o_query != null)
            { /* use tostring to get query string */
                return this.o_query.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Get generic query object, e.g. Select or Create ...
        /// </summary>
        /// <typeparam name="T2">desired IQueryAbstract type of generic query object</typeparam>
        /// <returns>generic query object based on IQueryAbstract</returns>
        public T2? GetQuery<T2>() where T2 : IQueryAbstract
        {
            return (T2?)(IQueryAbstract?)this.o_query;
        }

        /// <summary>
        /// Set query string of generic query object, because sometimes it is easier this way
        /// </summary>
        /// <param name="p_s_query">sql query as string</param>
        /// <exception cref="ArgumentException">sql query parameter is null or empty</exception>
        /// <exception cref="NullReferenceException">generic query object is null</exception>
        public void SetQuery(string p_s_query)
        {
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_query))
            {
                throw new ArgumentException("Query parameter is null or empty");
            }

            if (this.o_query == null)
            {
                throw new NullReferenceException("Query property is null");
            }

            this.o_query.QueryString = p_s_query;
        }

        /// <summary>
        /// Get constraint type matching constraint parameter and database gateway enumeration value
        /// </summary>
        /// <param name="p_s_constraintType">constraint type parameter</param>
        /// <returns>constraint type matching database gateway</returns>
        /// <exception cref="ArgumentException">invalid database gateway or invalid constraint type</exception>
        public string ConstraintTypeAllocation(string p_s_constraintType)
        {
            bool b_exc = false;

            /* check base gateway parameter */
            switch (this.BaseGateway)
            {
                case BaseGateway.MARIADB:
                case BaseGateway.SQLITE:
                case BaseGateway.MSSQL:
                case BaseGateway.PGSQL:
                case BaseGateway.ORACLE:
                case BaseGateway.NOSQLMDB:
                    break;
                default:
                    b_exc = true;
                    break;
            }

            if (b_exc)
            {
                throw new ArgumentException("Invalid BaseGateway[" + this.BaseGateway + "]");
            }

            /* check constraint parameter */
            switch (p_s_constraintType)
            {
                case "NULL":
                case "NOT NULL":
                case "UNIQUE":
                case "PRIMARY KEY":
                case "DEFAULT":
                case "INDEX":
                case "AUTO_INCREMENT":
                    break;
                default:
                    b_exc = true;
                    break;
            }

            if (b_exc)
            {
                throw new ArgumentException("Invalid Constraint[" + p_s_constraintType + "]");
            }

            Dictionary<string, int> a_mapping = new()
            {
                { BaseGateway.MARIADB.ToString(), 0 },
                { BaseGateway.SQLITE.ToString(), 1 },
                { BaseGateway.MSSQL.ToString(), 2 },
                { BaseGateway.PGSQL.ToString(), 3 },
                { BaseGateway.ORACLE.ToString(), 4 },
                { BaseGateway.NOSQLMDB.ToString(), 5 },

                { "NULL", 0 },
                { "NOT NULL", 1 },
                { "UNIQUE", 2 },
                { "PRIMARY KEY", 3 },
                { "DEFAULT", 4 },
                { "INDEX", 5 },
                { "AUTO_INCREMENT", 6 }
            };

            string[][] a_allocation = new string[][] {
                    new string[] {"NULL", "NOT NULL", "UNIQUE", "PRIMARY KEY", "DEFAULT", "INDEX", "AUTO_INCREMENT"},			/* MARIADB */
					new string[] {"NULL", "NOT NULL", "UNIQUE", "PRIMARY KEY", "DEFAULT", "INDEX", "AUTOINCREMENT"},			/* SQLITE */
					new string[] {"NULL", "NOT NULL", "UNIQUE", "PRIMARY KEY", "DEFAULT", "INDEX", "IDENTITY(1,1)"},			/* MSSQL */
					new string[] {"NULL", "NOT NULL", "UNIQUE", "PRIMARY KEY", "DEFAULT", "INDEX", ""},							/* PGSQL */
					new string[] {"NULL", "NOT NULL", "UNIQUE", "PRIMARY KEY", "DEFAULT", "INDEX", ""},							/* ORACLE */
					new string[] {"NULL", "NOT NULL", "UNIQUE", "PRIMARY KEY", "DEFAULT", "INDEX", "AUTO_INCREMENT"}			/* MONGODB */
			};

            /* get constraint type of allocation matrix */
            return a_allocation[a_mapping[this.BaseGateway.ToString()]][a_mapping[p_s_constraintType]];
        }

        /// <summary>
        /// create sql string query as prepared statement, all values are replaced by '?'
        /// auto format DateTime, Date and Time values to sql conform strings
        /// </summary>
        /// <param name="p_e_base">database gateway enumeration value</param>
        /// <param name="p_s_query">sql query as string object</param>
        /// <param name="p_a_values">empty list of prepared statement values</param>
        /// <exception cref="ArgumentNullException">list of prepared statement values is null</exception>
        /// <exception cref="ArgumentException">list of prepared statement values is not empty</exception>
        public static string ConvertToPreparedStatementQuery(BaseGateway p_e_base, string p_s_query, List<KeyValuePair<string, Object>> p_a_values)
        {
            return ConvertToPreparedStatementQuery(p_e_base, p_s_query, p_a_values, true);
        }

        /// <summary>
        /// create sql string query as prepared statement, all values are replaced by '?'
        /// </summary>
        /// <param name="p_e_base">database gateway enumeration value</param>
        /// <param name="p_s_query">sql query as string object</param>
        /// <param name="p_a_values">empty list of prepared statement values</param>
        /// <param name="p_b_formatDateTimeValues">auto format DateTime, Date and Time values to sql conform strings</param>
        /// <exception cref="ArgumentNullException">list of prepared statement values is null</exception>
        /// <exception cref="ArgumentException">list of prepared statement values is not empty</exception>
        public static string ConvertToPreparedStatementQuery(BaseGateway p_e_base, string p_s_query, List<KeyValuePair<string, Object>> p_a_values, bool p_b_formatDateTimeValues)
        {
            string s_queryValueTag = "forestnetSQLValue";

            if (p_a_values.Count != 0)
            {
                throw new ArgumentException("List of prepared statement values must be empty");
            }

            /* PGSQL + MSSQL + ORACLE: you can only pass values to DML statements */
            if (((p_e_base == BaseGateway.PGSQL) || (p_e_base == BaseGateway.MSSQL) || (p_e_base == BaseGateway.ORACLE)) && ((p_s_query.StartsWith("CREATE TABLE")) || (p_s_query.StartsWith("ALTER TABLE"))))
            {
                if (ForestNET.Lib.Global.Instance.LogCompleteSqlQuery)
                {
                    ForestNET.Lib.Global.ILogFiner("query prepared statement: '" + p_s_query.Replace("[" + s_queryValueTag + "]", "'").Replace("[/" + s_queryValueTag + "]", "'") + "'");
                }

                return p_s_query.Replace("[" + s_queryValueTag + "]", "'").Replace("[/" + s_queryValueTag + "]", "'");
            }

            /* get values out of sql statement to pass them within a prepared statement */
            System.Text.RegularExpressions.Regex o_regex = new("\\[" + s_queryValueTag + "\\](.*?)\\[/" + s_queryValueTag + "\\]");
            System.Text.RegularExpressions.MatchCollection o_matchCollection = o_regex.Matches(p_s_query);

            if (o_matchCollection.Count > 0)
            {
                int i_valueNumber = 1;

                foreach (System.Text.RegularExpressions.Match o_match in o_matchCollection.Cast<System.Text.RegularExpressions.Match>())
                {
                    /* replace value with '@000XY' value number */
                    p_s_query = p_s_query.Replace(o_match.Value, "@" + i_valueNumber.ToString("D5"));
                    i_valueNumber++;

                    /* get value recognized by regex */
                    string s_value = o_match.Value;

                    /* remove query value tags */
                    if ((s_value.StartsWith("[" + s_queryValueTag + "]")) && (s_value.EndsWith("[/" + s_queryValueTag + "]")))
                    {
                        s_value = s_value.Replace("[" + s_queryValueTag + "]", "").Replace("[/" + s_queryValueTag + "]", "");
                    }

                    if ((s_value.ToLower().Equals("true")) || (s_value.ToLower().Equals("false")))
                    {
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("value isBool" + ((ForestNET.Lib.Global.Instance.LogCompleteSqlQuery) ? ": " + s_value : ""));

                        /* MSSQL stores bit columns as tinyint, in ORACLE we just store one character */
                        if ((p_e_base == BaseGateway.MSSQL) || (p_e_base == BaseGateway.ORACLE))
                        {
                            if (s_value.ToLower().Equals("true"))
                            {
                                if (p_e_base == BaseGateway.MSSQL) /* store 'true' as tinyint or short '1' */
                                {
                                    p_a_values.Add(new KeyValuePair<string, Object>("short", (Object)Int16.Parse("1")));
                                }
                                else if (p_e_base == BaseGateway.ORACLE) /* store 'true' as char(1) '1' */
                                {
                                    p_a_values.Add(new KeyValuePair<string, Object>("string", (Object)"1"));
                                }
                            }
                            else
                            {
                                if (p_e_base == BaseGateway.MSSQL) /* store 'false' as tinyint or short '0' */
                                {
                                    p_a_values.Add(new KeyValuePair<string, Object>("short", (Object)Int16.Parse("0")));
                                }
                                else if (p_e_base == BaseGateway.ORACLE) /* store 'false' as char(1) '0' */
                                {
                                    p_a_values.Add(new KeyValuePair<string, Object>("string", (Object)"0"));
                                }
                            }
                        }
                        else
                        {
                            p_a_values.Add(new KeyValuePair<string, Object>("bool", (Object)bool.Parse(s_value)));
                        }
                    }
                    else if (ForestNET.Lib.Helper.IsShort(s_value))
                    {
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("value isShort" + ((ForestNET.Lib.Global.Instance.LogCompleteSqlQuery) ? ": " + s_value : ""));
                        p_a_values.Add(new KeyValuePair<string, Object>("short", (Object)Int16.Parse(s_value)));
                    }
                    else if (ForestNET.Lib.Helper.IsInteger(s_value))
                    {
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("value isInteger" + ((ForestNET.Lib.Global.Instance.LogCompleteSqlQuery) ? ": " + s_value : ""));
                        p_a_values.Add(new KeyValuePair<string, Object>("integer", (Object)Int32.Parse(s_value)));
                    }
                    else if (ForestNET.Lib.Helper.IsLong(s_value))
                    {
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("value isLong" + ((ForestNET.Lib.Global.Instance.LogCompleteSqlQuery) ? ": " + s_value : ""));
                        p_a_values.Add(new KeyValuePair<string, Object>("long", (Object)Int64.Parse(s_value)));
                    }
                    else if (ForestNET.Lib.Helper.IsSByte(s_value))
                    {
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("value isSByte" + ((ForestNET.Lib.Global.Instance.LogCompleteSqlQuery) ? ": " + s_value : ""));
                        p_a_values.Add(new KeyValuePair<string, Object>("sbyte", (Object)SByte.Parse(s_value)));
                    }
                    else if ((s_value.StartsWith("[unsignedByte]")) && (s_value.EndsWith("[/unsignedByte]")) && (ForestNET.Lib.Helper.IsByte(s_value.Replace("[unsignedByte]", "").Replace("[/unsignedByte]", ""))))
                    {
                        s_value = s_value.Replace("[unsignedByte]", "").Replace("[/unsignedByte]", "");
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("value isByte" + ((ForestNET.Lib.Global.Instance.LogCompleteSqlQuery) ? ": " + s_value : ""));
                        p_a_values.Add(new KeyValuePair<string, Object>("byte", (Object)Byte.Parse(s_value)));
                    }
                    else if ((s_value.StartsWith("[unsignedShort]")) && (s_value.EndsWith("[/unsignedShort]")) && (ForestNET.Lib.Helper.IsUShort(s_value.Replace("[unsignedShort]", "").Replace("[/unsignedShort]", ""))))
                    {
                        s_value = s_value.Replace("[unsignedShort]", "").Replace("[/unsignedShort]", "");
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("value isUShort" + ((ForestNET.Lib.Global.Instance.LogCompleteSqlQuery) ? ": " + s_value : ""));
                        p_a_values.Add(new KeyValuePair<string, Object>("ushort", (Object)UInt16.Parse(s_value)));
                    }
                    else if ((s_value.StartsWith("[unsignedInt]")) && (s_value.EndsWith("[/unsignedInt]")) && (ForestNET.Lib.Helper.IsUInteger(s_value.Replace("[unsignedInt]", "").Replace("[/unsignedInt]", ""))))
                    {
                        s_value = s_value.Replace("[unsignedInt]", "").Replace("[/unsignedInt]", "");
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("value isUInteger" + ((ForestNET.Lib.Global.Instance.LogCompleteSqlQuery) ? ": " + s_value : ""));
                        p_a_values.Add(new KeyValuePair<string, Object>("uinteger", (Object)UInt32.Parse(s_value)));
                    }
                    else if ((s_value.StartsWith("[unsignedLong]")) && (s_value.EndsWith("[/unsignedLong]")) && (ForestNET.Lib.Helper.IsULong(s_value.Replace("[unsignedLong]", "").Replace("[/unsignedLong]", ""))))
                    {
                        s_value = s_value.Replace("[unsignedLong]", "").Replace("[/unsignedLong]", "");
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("value isULong" + ((ForestNET.Lib.Global.Instance.LogCompleteSqlQuery) ? ": " + s_value : ""));
                        p_a_values.Add(new KeyValuePair<string, Object>("ulong", (Object)UInt64.Parse(s_value)));
                    }
                    else if ((s_value.StartsWith("[forestnetFloat]")) && (s_value.EndsWith("[/forestnetFloat]")) && (ForestNET.Lib.Helper.IsFloat(s_value.Replace("[forestnetFloat]", "").Replace("[/forestnetFloat]", ""))))
                    {
                        s_value = s_value.Replace("[forestnetFloat]", "").Replace("[/forestnetFloat]", "");
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("value isFloat" + ((ForestNET.Lib.Global.Instance.LogCompleteSqlQuery) ? ": " + s_value : ""));
                        p_a_values.Add(new KeyValuePair<string, Object>("float", (Object)Single.Parse(s_value, System.Globalization.CultureInfo.InvariantCulture)));
                    }
                    else if ((s_value.StartsWith("[forestnetDouble]")) && (s_value.EndsWith("[/forestnetDouble]")) && (ForestNET.Lib.Helper.IsDouble(s_value.Replace("[forestnetDouble]", "").Replace("[/forestnetDouble]", ""))))
                    {
                        s_value = s_value.Replace("[forestnetDouble]", "").Replace("[/forestnetDouble]", "");
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("value isDouble" + ((ForestNET.Lib.Global.Instance.LogCompleteSqlQuery) ? ": " + s_value : ""));
                        p_a_values.Add(new KeyValuePair<string, Object>("double", (Object)Double.Parse(s_value, System.Globalization.CultureInfo.InvariantCulture)));
                    }
                    else if ((s_value.StartsWith("[forestnetDecimal]")) && (s_value.EndsWith("[/forestnetDecimal]")) && (ForestNET.Lib.Helper.IsDecimal(s_value.Replace("[forestnetDecimal]", "").Replace("[/forestnetDecimal]", ""))))
                    {
                        s_value = s_value.Replace("[forestnetDecimal]", "").Replace("[/forestnetDecimal]", "");
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("value isDecimal" + ((ForestNET.Lib.Global.Instance.LogCompleteSqlQuery) ? ": " + s_value : ""));
                        p_a_values.Add(new KeyValuePair<string, Object>("decimal", (Object)Decimal.Parse(s_value, System.Globalization.CultureInfo.InvariantCulture)));
                    }
                    else if (ForestNET.Lib.Helper.IsDate(s_value))
                    {
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("value isDate" + ((ForestNET.Lib.Global.Instance.LogCompleteSqlQuery) ? ": " + s_value : ""));

                        if (p_b_formatDateTimeValues)
                        {
                            p_a_values.Add(new KeyValuePair<string, Object>("localdate", (Object)ForestNET.Lib.Helper.FromDateString(s_value).ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)));
                        }
                        else
                        {
                            p_a_values.Add(new KeyValuePair<string, Object>("localdate", (Object)ForestNET.Lib.Helper.FromDateString(s_value)));
                        }
                    }
                    else if ((ForestNET.Lib.Helper.IsTime(s_value)) || ((s_value.StartsWith("1970-01-01")) && (ForestNET.Lib.Helper.IsTime(s_value.Substring(11)))))
                    {
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("value isTime" + ((ForestNET.Lib.Global.Instance.LogCompleteSqlQuery) ? ": " + s_value : ""));

                        if (s_value.StartsWith("1970-01-01"))
                        {
                            s_value = s_value.Substring(11);
                        }

                        if (p_b_formatDateTimeValues)
                        {
                            p_a_values.Add(new KeyValuePair<string, Object>("localtime", (Object)ForestNET.Lib.Helper.FromTimeString(s_value).ToString("hh\\:mm\\:ss")));
                        }
                        else
                        {
                            p_a_values.Add(new KeyValuePair<string, Object>("localtime", (Object)ForestNET.Lib.Helper.FromTimeString(s_value)));
                        }
                    }
                    else if ((!s_value.StartsWith("1970-01-01")) && (ForestNET.Lib.Helper.IsDateTime(s_value)))
                    {
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("value isDateTime" + ((ForestNET.Lib.Global.Instance.LogCompleteSqlQuery) ? ": " + s_value : ""));

                        if (p_b_formatDateTimeValues)
                        {
                            p_a_values.Add(new KeyValuePair<string, Object>("localdatetime", (Object)((p_e_base != BaseGateway.MSSQL) ? ForestNET.Lib.Helper.FromDateTimeString(s_value).ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) : ForestNET.Lib.Helper.FromDateTimeString(s_value).ToString("yyyy-MM-ddTHH:mm:ss", System.Globalization.CultureInfo.InvariantCulture))));
                        }
                        else
                        {
                            p_a_values.Add(new KeyValuePair<string, Object>("localdatetime", (Object)ForestNET.Lib.Helper.FromDateTimeString(s_value)));
                        }
                    }
                    else
                    {
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("value isString" + ((ForestNET.Lib.Global.Instance.LogCompleteSqlQuery) ? ": " + s_value : ""));
                        p_a_values.Add(new KeyValuePair<string, Object>("string", (Object)s_value));
                    }
                }
            }

            if (ForestNET.Lib.Global.Instance.LogCompleteSqlQuery)
            {
                ForestNET.Lib.Global.ILogFiner("query prepared statement: '" + p_s_query + "'");
            }

            return p_s_query;
        }

        /// <summary>
        /// create standard sql query
        /// </summary>
        /// <param name="p_s_preparedStatement">prepared statement</param>
        /// <param name="p_a_values">list of prepared statement values</param>
        public static string ConvertPreparedStatementSqlQueryToStandard(string p_s_preparedStatement, List<KeyValuePair<string, Object>> p_a_values)
        {
            int i_pointer;

            /* look for all parameters like '@000001', starting with '@' and a digit with the length of 5 */
            while ((i_pointer = System.Text.RegularExpressions.Regex.Match(p_s_preparedStatement, "@[0-9]{5}([^0-9]{1,}|$)").Index) > 0)
            {
                string s_before = p_s_preparedStatement.Substring(0, i_pointer);
                string s_after = p_s_preparedStatement.Substring(i_pointer + 6); /* skip '@' and a digit with the length of 5 */

                /* get index of value for values list */
                int i_indexOfValue = Convert.ToInt32(p_s_preparedStatement.Substring(i_pointer + 1, 5));

                /* our index of value starts with '1' in ConvertToPreparedStatementQuery method, so we must decrement it */
                if (i_indexOfValue > 0)
                {
                    i_indexOfValue--;
                }

                string s_type = (p_a_values.Count <= i_indexOfValue) ? "NO TYPE" : p_a_values[i_indexOfValue].Key;
                string s_value = (p_a_values.Count <= i_indexOfValue) ? "NO VALUE IN LIST" : p_a_values[i_indexOfValue].Value.ToString() ?? "";
                string s_quote = "'";

                /* use no quotes for digit, bool or null values */
                if (
                    (s_type.Equals("sbyte")) ||
                    (s_type.Equals("short")) ||
                    (s_type.Equals("integer")) ||
                    (s_type.Equals("long")) ||
                    (s_type.Equals("byte")) ||
                    (s_type.Equals("ushort")) ||
                    (s_type.Equals("uinteger")) ||
                    (s_type.Equals("ulong")) ||
                    (s_type.Equals("float")) ||
                    (s_type.Equals("double")) ||
                    (s_type.Equals("decimal")) ||
                    (s_type.Equals("bool")) ||
                    (s_value.Equals("true")) ||
                    (s_value.Equals("false")) ||
                    (s_value.Equals("NULL"))
                )
                {
                    s_quote = "";
                }

                /* use general decimal separator '.' for float, double and decimal */
                if ((s_type.Equals("float")) || (s_type.Equals("double")) || (s_type.Equals("decimal")))
                {
                    System.Globalization.NumberFormatInfo o_numberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.Clone() as System.Globalization.NumberFormatInfo ?? throw new NullReferenceException("could not retrieve number format info from current culture info");
                    o_numberFormatInfo.CurrencyGroupSeparator = "";
                    o_numberFormatInfo.NumberGroupSeparator = "";
                    o_numberFormatInfo.CurrencyDecimalSeparator = ".";
                    o_numberFormatInfo.NumberDecimalSeparator = ".";

                    if (s_type.Equals("float"))
                    {
                        s_value = ((float)p_a_values[i_indexOfValue].Value).ToString(o_numberFormatInfo);
                    }
                    else if (s_type.Equals("double"))
                    {
                        s_value = ((double)p_a_values[i_indexOfValue].Value).ToString(o_numberFormatInfo);
                    }
                    else if (s_type.Equals("decimal"))
                    {
                        s_value = ((decimal)p_a_values[i_indexOfValue].Value).ToString(o_numberFormatInfo);
                    }

                    /* if we have no decimal separator, it means we have a clear integer part - missing .0 at the end */
                    if (!s_value.Contains('.'))
                    {
                        /* add .0 at the end */
                        s_value += ".0";
                    }
                }

                /* string bool value to lower */
                if (s_type.Equals("bool"))
                {
                    s_value = s_value.ToLower();
                }

                p_s_preparedStatement = s_before + s_quote + s_value + s_quote + s_after;
            }

            return p_s_preparedStatement;
        }
    }
}