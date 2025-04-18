namespace ForestNET.Lib.SQL
{
    /// <summary>
    /// SQL class to generate a delete sql query with optional where objects, called by tostring method.
    /// </summary>
    public class Delete : QueryAbstract
    {

        /* Fields */

        /* Properties */

        public List<Where> Where { get; set; } = [];

        /* Methods */

        /// <summary>
        /// constructor will be called with Query object, because all necessary database gateway, sql type and table information are part of Query class
        /// </summary>
        /// <param name="p_o_query">query object with table and sql type information</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object</exception>
        public Delete(IQuery? p_o_query) : base(p_o_query)
        {

        }

        /// <summary>
        /// create delete sql string query
        /// </summary>
        public override string ToString()
        {
            string s_foo = "";

            try
            {
                bool b_exc = false;

                switch (this.BaseGateway)
                {
                    case BaseGateway.MARIADB:
                    case BaseGateway.SQLITE:
                    case BaseGateway.NOSQLMDB:
                        s_foo = "DELETE FROM " + "`" + this.Table + "`";
                        break;
                    case BaseGateway.MSSQL:
                        s_foo = "DELETE FROM " + "[" + this.Table + "]";
                        break;
                    case BaseGateway.ORACLE:
                    case BaseGateway.PGSQL:
                        s_foo = "DELETE FROM " + "\"" + this.Table + "\"";
                        break;
                    default:
                        b_exc = true;
                        break;
                }

                if (b_exc)
                {
                    throw new Exception("BaseGateway[" + this.BaseGateway + "] not implemented");
                }

                /* add where clause if where objects are available */
                if (this.Where.Count > 0)
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
                s_foo = " >>>>> Delete class Exception: [" + o_exc + "] <<<<< ";
            }

            return s_foo;
        }
    }
}