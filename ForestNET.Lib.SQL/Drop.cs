namespace ForestNET.Lib.SQL
{
    /// <summary>
    /// SQL class to generate a drop sql query with optional IF EXISTS property, called by tostring method.
    /// </summary>
    public class Drop : QueryAbstract
    {

        /* Fields */

        /* Properties */

        public bool IfExists { get; set; } = false;

        /* Methods */

        /// <summary>
        /// constructor will be called with Query object, because all necessary database gateway, sql type and table information are part of Query class
        /// </summary>
        /// <param name="p_o_query">query object with table and sql type information</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object</exception>
        public Drop(IQuery? p_o_query) : base(p_o_query)
        {

        }

        /// <summary>
        /// create drop sql string query
        /// </summary>
        public override string ToString()
        {
            string s_foo = "DROP TABLE ";

            /* add IF EXISTS to drop query, but not if database gateway is ORACLE */
            if ((this.IfExists) && (this.BaseGateway != BaseGateway.ORACLE))
            {
                s_foo += " IF EXISTS ";
            }

            try
            {
                bool b_exc = false;

                switch (this.BaseGateway)
                {
                    case BaseGateway.MARIADB:
                    case BaseGateway.SQLITE:
                    case BaseGateway.NOSQLMDB:
                        s_foo += "`" + this.Table + "`";
                        break;
                    case BaseGateway.MSSQL:
                        s_foo += "[" + this.Table + "]";
                        break;
                    case BaseGateway.ORACLE:
                    case BaseGateway.PGSQL:
                        s_foo += "\"" + this.Table + "\"";
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
                s_foo = " >>>>> Drop class Exception: [" + o_exc + "] <<<<< ";
            }

            return s_foo;
        }
    }
}