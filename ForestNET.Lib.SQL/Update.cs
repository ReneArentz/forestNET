namespace ForestNET.Lib.SQL
{
    /// <summary>
    /// SQL class to generate a update sql query based on column value pair objects and where clauses, called by ToString method.
    /// </summary>
    public class Update : QueryAbstract
    {

        /* Fields */

        /* Properties */

        public List<ColumnValue> ColumnValues { get; set; } = [];
        public List<Where> Where { get; set; } = [];

        /* Methods */

        /// <summary>
        /// constructor will be called with Query object, because all necessary database gateway, sql type and table information are part of Query class
        /// </summary>
        /// <param name="p_o_query">query object with table and sql type information</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object</exception>
        public Update(IQuery? p_o_query) : base(p_o_query)
        {

        }

        /// <summary>
        /// create update sql string query
        /// </summary>
        public override string ToString()
        {
            return ToString(true);
        }

        /// <summary>
        /// create update sql string query
        /// </summary>
        /// <param name="p_b_returnWithWhere">true - with where clause, false - without where clause</param>
        public string ToString(bool p_b_returnWithWhere)
        {
            string s_foo = "";

            try
            {
                /* check if we have any column value pair objects for update query */
                if (this.ColumnValues.Count <= 0)
                {
                    throw new Exception("ColumnValues object list is empty");
                }

                /* check if we have any where clauses, so we do not update all records by accident */
                if (this.Where.Count <= 0)
                {
                    throw new Exception("Where object list is empty");
                }

                bool b_exc = false;

                switch (this.BaseGateway)
                {
                    case BaseGateway.MARIADB:
                    case BaseGateway.SQLITE:
                    case BaseGateway.NOSQLMDB:
                        s_foo = "UPDATE " + "`" + this.Table + "`" + " SET ";
                        break;
                    case BaseGateway.MSSQL:
                        s_foo = "UPDATE " + "[" + this.Table + "]" + " SET ";
                        break;
                    case BaseGateway.ORACLE:
                    case BaseGateway.PGSQL:
                        s_foo = "UPDATE " + "\"" + this.Table + "\"" + " SET ";
                        break;
                    default:
                        b_exc = true;
                        break;
                }

                if (b_exc)
                {
                    throw new Exception("BaseGateway[" + this.BaseGateway + "] not implemented");
                }

                /* add all column value pairs to update query */
                foreach (ColumnValue o_columnValue in this.ColumnValues)
                {
                    s_foo += ((o_columnValue.Column?.ToString()) ?? throw new Exception("Column for Update is null")) +
                        " = " +
                        ((o_columnValue.Value?.ToString()) ?? throw new Exception("Value for Update is null")) +
                        ", ";
                }

                /* remove last ', ' separator */
                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                /* add all where clauses to update query */
                if ((this.Where.Count > 0) && (p_b_returnWithWhere))
                {
                    s_foo += " WHERE ";

                    foreach (Where o_where in this.Where)
                    {
                        s_foo += o_where.ToString();
                    }
                }
            }
            catch (Exception o_exc)
            { /* just set exception as query return, so database interface will have an exception as well */
                s_foo = " >>>>> Update class Exception: [" + o_exc + "] <<<<< ";
            }

            return s_foo;
        }
    }
}