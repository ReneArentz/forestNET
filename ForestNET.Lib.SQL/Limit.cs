namespace ForestNET.Lib.SQL
{
    /// <summary>
    /// SQL class to generate a limit sql clause based on a start and interval value, called by ToString method.
    /// </summary>
    public class Limit : QueryAbstract
    {

        /* Fields */

        /* Properties */

        public int Start { get; set; } = 0;

        public int Interval { get; set; } = 0;

        /* Methods */

        /// <summary>
        /// constructor will be called with Query object, because all necessary database gateway, sql type and table information are part of Query class
        /// </summary>
        /// <param name="p_o_query">query object with table and sql type information</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object</exception>
        public Limit(IQuery? p_o_query) : this(p_o_query, 0, 0)
        {

        }

        /// <summary>
        /// constructor will be called with Query object, because all necessary database gateway, sql type and table information are part of Query class
        /// </summary>
        /// <param name="p_o_query">query object with table and sql type information</param>
        /// <param name="p_i_start">start integer value for interval</param>
        /// <param name="p_i_interval">interval integer value</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object</exception>
        public Limit(IQuery? p_o_query, int p_i_start, int p_i_interval) : base(p_o_query)
        {
            this.Start = p_i_start;
            this.Interval = p_i_interval;
        }

        /// <summary>
        /// create limit sql clause as string
        /// </summary>
        public override string ToString()
        {
            string s_foo = "";
            bool b_exc = false;

            try
            {
                /* only select sql type allowed */
                if (this.SqlType != SqlType.SELECT)
                {
                    throw new Exception("SqlType must be SELECT, but is '" + this.SqlType + "'");
                }

                switch (this.BaseGateway)
                {
                    case BaseGateway.MARIADB:
                    case BaseGateway.SQLITE:
                    case BaseGateway.NOSQLMDB:
                        s_foo = " LIMIT " + this.Start + ", " + this.Interval;
                        break;
                    case BaseGateway.MSSQL:
                    case BaseGateway.ORACLE:
                        s_foo = " OFFSET " + this.Start + " ROWS FETCH NEXT " + this.Interval + " ROWS ONLY";
                        break;
                    case BaseGateway.PGSQL:
                        s_foo = " LIMIT " + this.Interval + " OFFSET " + this.Start;
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
                s_foo = " >>>>> Limit class Exception: [" + o_exc + "] <<<<< ";
            }

            return s_foo;
        }
    }
}