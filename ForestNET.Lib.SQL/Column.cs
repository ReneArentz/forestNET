namespace ForestNET.Lib.SQL
{
    /// <summary>
    /// Column class with column, name(alias) and aggregation properties.
    /// </summary>
    public class Column : QueryAbstract
    {

        /* Fields */

        private string s_sqlAggregation = "";

        /* Properties */

        public string ColumnStr { get; set; } = "";

        public string Name { get; set; } = "";

        public bool IsJoinTable { get; set; } = false;

        /// <summary>
        /// Define a aggregation which should be used with this column, after column class has been initiated
        /// Must match with valid sql aggregation from QueryAbstract
        /// </summary>
        /// <param name="value">aggregation parameter value, e.g. 'COUNT'</param>
        /// <exception cref="ArgumentException">invalid aggregation parameter value</exception>
        public string SqlAggregation
        {
            get
            {
                return this.s_sqlAggregation;
            }
            set
            {
                bool b_accept = false;

                for (int i = 0; i < this.SqlAggregations.Length; i++)
                {
                    if (this.SqlAggregations[i] == value)
                    {
                        b_accept = true;
                    }
                }

                if (b_accept)
                {
                    this.s_sqlAggregation = value;
                }
                else
                {
                    throw new ArgumentException("Value[" + value + "] is not in defined list[" + string.Join(", ", this.SqlAggregations) + "]");
                }
            }
        }

        /* Methods */

        /// <summary>
        /// Column constructor, need at least query object as parameter for table information
        /// </summary>
        /// <param name="p_o_query">query object with database gateway and table information</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object</exception>
        public Column(IQuery? p_o_query) : this(p_o_query, "", "", "")
        {

        }

        /// <summary>
        /// Column constructor, need at least query object as parameter for table information
        /// </summary>
        /// <param name="p_o_query">query object with database gateway and table information</param>
        /// <param name="p_s_column">column name in database table schema</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object</exception>
        public Column(IQuery? p_o_query, string p_s_column) : this(p_o_query, p_s_column, "", "")
        {

        }

        /// <summary>
        /// Column constructor, need at least query object as parameter for table information
        /// </summary>
        /// <param name="p_o_query">query object with database gateway and table information</param>
        /// <param name="p_s_column">column name in database table schema</param>
        /// <param name="p_s_name">alias for column</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object</exception>
        public Column(IQuery? p_o_query, string p_s_column, string p_s_name) : this(p_o_query, p_s_column, p_s_name, "")
        {

        }

        /// <summary>
        /// Column constructor, need at least query object as parameter for table information
        /// </summary>
        /// <param name="p_o_query">query object with database gateway and table information</param>
        /// <param name="p_s_column">column name in database table schema</param>
        /// <param name="p_s_name">alias for column</param>
        /// <param name="p_s_sqlAggregation">aggregation parameter value, e.g. 'COUNT'</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object</exception>
        public Column(IQuery? p_o_query, string p_s_column, string p_s_name, string p_s_sqlAggregation) : base(p_o_query)
        {
            if (!ForestNET.Lib.Helper.IsStringEmpty(p_s_column))
                this.ColumnStr = p_s_column;

            if (!ForestNET.Lib.Helper.IsStringEmpty(p_s_name))
                this.Name = p_s_name;

            if (!ForestNET.Lib.Helper.IsStringEmpty(p_s_sqlAggregation))
                this.SqlAggregation = p_s_sqlAggregation;
        }

        /// <summary>
        /// create column part of a sql string query
        /// </summary>
        public override string ToString()
        {
            return this.ToString(true);
        }

        /// <summary>
        /// create column part of a sql string query
        /// </summary>
        public string ToString(bool p_b_printName)
        {
            string s_foo = "";

            try
            {
                bool b_exc = false;
                bool b_exc2 = false;

                switch (this.BaseGateway)
                {
                    case BaseGateway.MARIADB:
                    case BaseGateway.NOSQLMDB:
                        switch (this.SqlType)
                        {
                            case SqlType.SELECT:
                                if (this.ColumnStr == "*")
                                {
                                    s_foo = "*";
                                }
                                else
                                {
                                    if (!ForestNET.Lib.Helper.IsStringEmpty(this.Table))
                                    {
                                        s_foo = "`" + this.Table + "`" + "." + "`" + this.ColumnStr + "`";
                                    }
                                    else
                                    {
                                        s_foo = "`" + this.ColumnStr + "`";
                                    }
                                }

                                if (!ForestNET.Lib.Helper.IsStringEmpty(this.s_sqlAggregation))
                                {
                                    s_foo = this.s_sqlAggregation + "(" + s_foo + ")";
                                }

                                if ((p_b_printName) && (!ForestNET.Lib.Helper.IsStringEmpty(this.Name)))
                                {
                                    s_foo = s_foo + " AS" + " '" + this.Name + "'";
                                }
                                break;
                            case SqlType.INSERT:
                            case SqlType.UPDATE:
                            case SqlType.DELETE:
                                if (!ForestNET.Lib.Helper.IsStringEmpty(this.Table))
                                {
                                    s_foo = "`" + this.Table + "`" + "." + "`" + this.ColumnStr + "`";
                                }
                                else
                                {
                                    s_foo = "`" + this.ColumnStr + "`";
                                }
                                break;
                            default:
                                b_exc2 = true;
                                break;
                        }

                        if (b_exc2)
                        {
                            throw new Exception("SqlType[" + this.SqlType + "] not implemented");
                        }
                        break;
                    case BaseGateway.SQLITE:
                        switch (this.SqlType)
                        {
                            case SqlType.SELECT:
                                if (this.ColumnStr == "*")
                                {
                                    s_foo = "*";
                                }
                                else
                                {
                                    if (!ForestNET.Lib.Helper.IsStringEmpty(this.Table))
                                    {
                                        s_foo = "`" + this.Table + "`" + "." + "`" + this.ColumnStr + "`";
                                    }
                                    else
                                    {
                                        s_foo = "`" + this.ColumnStr + "`";
                                    }
                                }

                                if (!ForestNET.Lib.Helper.IsStringEmpty(this.s_sqlAggregation))
                                {
                                    s_foo = this.s_sqlAggregation + "(" + s_foo + ")";
                                }

                                if ((p_b_printName) && (!ForestNET.Lib.Helper.IsStringEmpty(this.Name)))
                                {
                                    s_foo = s_foo + " AS" + " '" + this.Name + "'";
                                }
                                break;
                            case SqlType.INSERT:
                            case SqlType.UPDATE:
                                s_foo = "`" + this.ColumnStr + "`";
                                break;
                            case SqlType.DELETE:
                                if (!ForestNET.Lib.Helper.IsStringEmpty(this.Table))
                                {
                                    s_foo = "`" + this.Table + "`" + "." + "`" + this.ColumnStr + "`";
                                }
                                else
                                {
                                    s_foo = "`" + this.ColumnStr + "`";
                                }
                                break;
                            default:
                                b_exc2 = true;
                                break;
                        }

                        if (b_exc2)
                        {
                            throw new Exception("SqlType[" + this.SqlType + "] not implemented");
                        }
                        break;
                    case BaseGateway.MSSQL:
                        switch (this.SqlType)
                        {
                            case SqlType.SELECT:
                                if (this.ColumnStr == "*")
                                {
                                    s_foo = "*";
                                }
                                else
                                {
                                    if (!ForestNET.Lib.Helper.IsStringEmpty(this.Table))
                                    {
                                        s_foo = "[" + this.Table + "]" + "." + "[" + this.ColumnStr + "]";
                                    }
                                    else
                                    {
                                        s_foo = "[" + this.ColumnStr + "]";
                                    }
                                }

                                if (!ForestNET.Lib.Helper.IsStringEmpty(this.s_sqlAggregation))
                                {
                                    s_foo = this.s_sqlAggregation + "(" + s_foo + ")";
                                }

                                if ((p_b_printName) && (!ForestNET.Lib.Helper.IsStringEmpty(this.Name)))
                                {
                                    s_foo = s_foo + " AS" + " '" + this.Name + "'";
                                }
                                break;
                            case SqlType.INSERT:
                            case SqlType.UPDATE:
                            case SqlType.DELETE:
                                if (!ForestNET.Lib.Helper.IsStringEmpty(this.Table))
                                {
                                    s_foo = "[" + this.Table + "]" + "." + "[" + this.ColumnStr + "]";
                                }
                                else
                                {
                                    s_foo = "[" + this.ColumnStr + "]";
                                }
                                break;
                            default:
                                b_exc2 = true;
                                break;
                        }

                        if (b_exc2)
                        {
                            throw new Exception("SqlType[" + this.SqlType + "] not implemented");
                        }
                        break;
                    case BaseGateway.PGSQL:
                    case BaseGateway.ORACLE:
                        switch (this.SqlType)
                        {
                            case SqlType.SELECT:
                                if (this.ColumnStr == "*")
                                {
                                    s_foo = "*";
                                }
                                else
                                {
                                    if (!ForestNET.Lib.Helper.IsStringEmpty(this.Table))
                                    {
                                        s_foo = "\"" + this.Table + "\"" + "." + "\"" + this.ColumnStr + "\"";
                                    }
                                    else
                                    {
                                        s_foo = "\"" + this.ColumnStr + "\"";
                                    }
                                }

                                if (!ForestNET.Lib.Helper.IsStringEmpty(this.s_sqlAggregation))
                                {
                                    s_foo = this.s_sqlAggregation + "(" + s_foo + ")";
                                }

                                if ((p_b_printName) && (!ForestNET.Lib.Helper.IsStringEmpty(this.Name)))
                                {
                                    s_foo = s_foo + " AS" + " \"" + this.Name + "\"";
                                }
                                break;
                            case SqlType.INSERT:
                            case SqlType.UPDATE:
                                s_foo = "\"" + this.ColumnStr + "\"";
                                break;
                            case SqlType.DELETE:
                                if (!ForestNET.Lib.Helper.IsStringEmpty(this.Table))
                                {
                                    s_foo = "\"" + this.Table + "\"" + "." + "\"" + this.ColumnStr + "\"";
                                }
                                else
                                {
                                    s_foo = "\"" + this.ColumnStr + "\"";
                                }
                                break;
                            default:
                                b_exc2 = true;
                                break;
                        }

                        if (b_exc2)
                        {
                            throw new Exception("SqlType[" + this.SqlType + "] not implemented");
                        }
                        break;
                    default:
                        b_exc = true;
                        break;
                }

                if (b_exc)
                {
                    throw new Exception("BaseGateway[" + this.BaseGateway + "] not implemented");
                }
            }
            catch (Exception o_exc)
            { /* just set exception as query return, so database interface will have an exception as well */
                s_foo = " >>>>> Column class Exception: [" + o_exc + "] <<<<< ";
            }

            return s_foo;
        }
    }
}