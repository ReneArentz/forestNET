namespace ForestNET.Lib.SQL
{
    /// <summary>
    /// SQL class to generate an insert sql query based on column value pair objects, called by tostring method.
    /// </summary>
    public class Insert : QueryAbstract
    {

        /* Fields */

        /* Properties */

        public List<ColumnValue> ColumnValues { get; set; } = [];
        public Column? MSSQLLastInsertIdColumn { get; set; } = null;
        public Column? NoSQLMDBColumnAutoIncrement { get; set; } = null;

        /* Methods */

        /// <summary>
        /// constructor will be called with Query object, because all necessary database gateway, sql type and table information are part of Query class
        /// </summary>
        /// <param name="p_o_query">query object with table and sql type information</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object</exception>
        public Insert(IQuery? p_o_query) : base(p_o_query)
        {

        }

        /// <summary>
        /// create insert sql string query
        /// </summary>
        public override string ToString()
        {
            string s_foo = "";

            try
            {
                /* check if we have column value pair objects for insert query */
                if (this.ColumnValues.Count <= 0)
                {
                    throw new Exception("ColumnValues object list is empty");
                }

                string s_foo1 = "";
                string s_foo2 = "";

                bool b_exc = false;

                switch (this.BaseGateway)
                {
                    case BaseGateway.MARIADB:
                    case BaseGateway.SQLITE:
                    case BaseGateway.NOSQLMDB:
                        s_foo = "INSERT INTO " + "`" + this.Table + "`" + " (";
                        break;
                    case BaseGateway.MSSQL:
                        s_foo = "INSERT INTO " + "[" + this.Table + "]" + " (";
                        break;
                    case BaseGateway.ORACLE:
                    case BaseGateway.PGSQL:
                        s_foo = "INSERT INTO " + "\"" + this.Table + "\"" + " (";
                        break;
                    default:
                        b_exc = true;
                        break;
                }

                if (b_exc)
                {
                    throw new Exception("BaseGateway[" + this.BaseGateway + "] not implemented");
                }

                /* add all column value pairs to insert query */
                foreach (ColumnValue o_columnValue in this.ColumnValues)
                {
                    s_foo1 += o_columnValue.Column + ", ";
                    s_foo2 += o_columnValue.Value?.ToString() + ", ";
                }

                /* remove last ',' separator */
                s_foo1 = s_foo1.Substring(0, s_foo1.Length - 2);
                s_foo2 = s_foo2.Substring(0, s_foo2.Length - 2);

                s_foo += s_foo1;

                /* alternative if SELECT IDENT_CURRENT('table_name') is not working, use this insert query then with executeQuery and a ResultSet as return */
                if ((this.BaseGateway == BaseGateway.MSSQL) && (this.MSSQLLastInsertIdColumn != null))
                {
                    s_foo += ") OUTPUT [INSERTED].[" + this.MSSQLLastInsertIdColumn.ColumnStr + "] AS 'LastInsertId' VALUES (";
                }
                else
                {
                    s_foo += ") VALUES (";
                }

                s_foo += s_foo2;
                s_foo += ")";
            }
            catch (Exception o_exc)
            { /* just set exception as query return, so database interface will have an exception as well */
                s_foo = " >>>>> Insert class Exception: [" + o_exc + "] <<<<< ";
            }

            return s_foo;
        }
    }
}