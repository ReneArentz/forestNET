namespace ForestNET.Lib.SQL
{
    /// <summary>
    /// SQL class to generate a join sql clause based on join type and relation objects, called by ToString method.
    /// </summary>
    public class Join : QueryAbstract
    {

        /* Fields */

        private string s_joinType = "";

        /* Properties */

        public List<Relation> Relations { get; set; } = [];

        /// <summary>
        /// set join type, depending if string value matching a valid join type defined in QueryAbstract class
        /// </summary>
        /// <param name="p_s_value">string value for join type</param>
        /// <exception cref="ArgumentException">invalid join type found</exception>
        public string JoinType
        {
            set
            {
                bool b_accept = false;

                /* check if p_s_value is a valid sql join type */
                for (int i = 0; i < this.JoinTypes.Length; i++)
                {
                    if (this.JoinTypes[i] == value)
                    {
                        b_accept = true;
                    }
                }

                if (b_accept)
                {
                    this.s_joinType = value;
                }
                else
                {
                    throw new ArgumentException("Value[" + value + "] is not in defined list[" + string.Join(", ", this.JoinTypes) + "]");
                }
            }
        }

        /* Methods */

        /// <summary>
        /// constructor will be called with Query object, because all necessary database gateway, sql type and table information are part of Query class
        /// </summary>
        /// <param name="p_o_query">query object with table and sql type information</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object</exception>
        public Join(IQuery? p_o_query) : this(p_o_query, "")
        {

        }

        /// <summary>
        /// constructor will be called with Query object, because all necessary database gateway, sql type and table information are part of Query class
        /// </summary>
        /// <param name="p_o_query">query object with table and sql type information</param>
        /// <param name="p_s_joinType">sets join type of join sql object</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object or invalid join type found</exception>
        public Join(IQuery? p_o_query, string p_s_joinType) : base(p_o_query)
        {
            if (!ForestNET.Lib.Helper.IsStringEmpty(p_s_joinType))
                this.JoinType = p_s_joinType;
        }

        /// <summary>
        /// create join sql clause as string
        /// </summary>
        public override string ToString()
        {
            string s_foo = "";

            try
            {
                /* check if we have relation objects for join clause */
                if (this.Relations.Count == 0)
                {
                    throw new Exception("Relation object list is empty");
                }

                /* only select sql type allowed */
                if (this.SqlType != SqlType.SELECT)
                {
                    throw new Exception("SqlType must be SELECT, but is '" + this.SqlType + "'");
                }

                bool b_exc = false;

                switch (this.BaseGateway)
                {
                    case BaseGateway.MARIADB:
                    case BaseGateway.SQLITE:
                    case BaseGateway.NOSQLMDB:
                        s_foo = this.s_joinType + " " + "`" + this.Table + "`";
                        break;
                    case BaseGateway.MSSQL:
                        s_foo = this.s_joinType + " " + "[" + this.Table + "]";
                        break;
                    case BaseGateway.PGSQL:
                    case BaseGateway.ORACLE:
                        s_foo = this.s_joinType + " " + "\"" + this.Table + "\"";
                        break;
                    default:
                        b_exc = true;
                        break;
                }

                if (b_exc)
                {
                    throw new Exception("BaseGateway[" + this.BaseGateway + "] not implemented");
                }

                s_foo += " ON ";

                /* add all relation object to join clause */
                foreach (Relation o_relation in this.Relations)
                {
                    s_foo += o_relation.ToString();
                }
            }
            catch (Exception o_exc)
            { /* just set exception as query return, so database interface will have an exception as well */
                s_foo = " >>>>> Join class Exception: [" + o_exc + "] <<<<< ";
            }

            return s_foo;
        }
    }
}