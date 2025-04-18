namespace ForestNET.Lib.SQL
{
    /// <summary>
    /// SQL class to generate a truncate sql query, called by tostring method.
    /// </summary>
    public class Truncate : QueryAbstract
    {

        /* Fields */

        /* Properties */

        /* Methods */

        /// <summary>
        /// constructor will be called with Query object, because all necessary database gateway, sql type and table information are part of Query class
        /// </summary>
        /// <param name="p_o_query">query object with table and sql type information</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object</exception>
        public Truncate(IQuery? p_o_query) : base(p_o_query)
        {

        }

        /// <summary>
        /// create truncate sql string query
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
                    case BaseGateway.NOSQLMDB:
                        s_foo = "TRUNCATE TABLE " + "`" + this.Table + "`";
                        break;
                    case BaseGateway.SQLITE:
                        s_foo = "DELETE FROM " + "`" + this.Table + "`";
                        s_foo += this.QuerySeparator;
                        s_foo += "VACUUM";
                        break;
                    case BaseGateway.MSSQL:
                        s_foo = "TRUNCATE TABLE " + "[" + this.Table + "]";
                        break;
                    case BaseGateway.ORACLE:
                    case BaseGateway.PGSQL:
                        s_foo = "TRUNCATE TABLE " + "\"" + this.Table + "\"";
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
                s_foo = " >>>>> Truncate class Exception: [" + o_exc + "] <<<<< ";
            }

            return s_foo;
        }
    }
}