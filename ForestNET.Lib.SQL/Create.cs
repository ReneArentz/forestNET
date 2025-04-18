namespace ForestNET.Lib.SQL
{
    /// <summary>
    /// SQL class to generate a create sql query based on column structures, called by tostring method.
    /// </summary>
    public class Create : QueryAbstract
    {

        /* Fields */

        /* Properties */

        public List<ColumnStructure> Columns { get; private set; } = [];

        /* Methods */

        /// <summary>
        /// constructor will be called with Query object, because all necessary database gateway, sql type and table information are part of Query class
        /// </summary>
        /// <param name="p_o_query">query object with table and sql type information</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object</exception>
        public Create(IQuery? p_o_query) : base(p_o_query)
        {

        }

        /// <summary>
        /// create 'create' sql string query
        /// </summary>
        public override string ToString()
        {
            string s_foo = "";

            try
            {
                /* check if we have column for create query, not necessary for MONGODB */
                if ((this.BaseGateway != BaseGateway.NOSQLMDB) && (this.Columns.Count <= 0))
                {
                    throw new Exception("ColumnsStructure object list is empty");
                }

                bool b_exc = false;

                switch (this.BaseGateway)
                {
                    case BaseGateway.MARIADB:
                    case BaseGateway.SQLITE:
                    case BaseGateway.NOSQLMDB:
                        s_foo = "CREATE TABLE " + "`" + this.Table + "`" + " (";

                        /* add columns structures to create query */
                        foreach (ColumnStructure o_columnStructure in this.Columns)
                        {
                            s_foo += o_columnStructure.ToString() + ", ";
                        }

                        /* remove last ',' separator */
                        s_foo = s_foo.Substring(0, s_foo.Length - 2);
                        s_foo += ")";
                        break;
                    case BaseGateway.MSSQL:
                        s_foo = "CREATE TABLE " + "[" + this.Table + "]" + " (";

                        /* add columns structures to create query */
                        foreach (ColumnStructure o_columnStructure in this.Columns)
                        {
                            s_foo += o_columnStructure.ToString() + ", ";
                        }

                        /* remove last ',' separator */
                        s_foo = s_foo.Substring(0, s_foo.Length - 2);
                        s_foo += ")";
                        break;
                    case BaseGateway.ORACLE:
                    case BaseGateway.PGSQL:
                        s_foo = "CREATE TABLE " + "\"" + this.Table + "\"" + " (";

                        /* add columns structures to create query */
                        foreach (ColumnStructure o_columnStructure in this.Columns)
                        {
                            s_foo += o_columnStructure.ToString() + ", ";
                        }

                        /* remove last ',' separator */
                        s_foo = s_foo.Substring(0, s_foo.Length - 2);
                        s_foo += ")";
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
                s_foo = " >>>>> Create class Exception: [" + o_exc + "] <<<<< ";
            }

            return s_foo;
        }
    }
}