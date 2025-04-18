namespace ForestNET.Lib.SQL
{
    /// <summary>
    /// SQL class to generate a select sql query based on column objects, join clauses, where clauses, order by clause and limit clause, called by ToString method.
    /// </summary>
    public class Select : QueryAbstract
    {

        /* Fields */

        /* Properties */

        public bool Distinct { get; set; } = false;
        public List<Column> Columns { get; set; } = [];
        public List<Join> Joins { get; set; } = [];
        public List<Where> Where { get; set; } = [];
        public List<Column> GroupBy { get; set; } = [];
        public List<Where> Having { get; set; } = [];
        public OrderBy? OrderBy { get; set; } = null;
        public Limit? Limit { get; set; } = null;

        /* Methods */

        /// <summary>
        /// constructor will be called with Query object, because all necessary database gateway, sql type and table information are part of Query class
        /// </summary>
        /// <param name="p_o_query">query object with table and sql type information</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object</exception>
        public Select(IQuery? p_o_query) : base(p_o_query)
        {

        }

        /// <summary>
        /// property method to know if there is a column in the select query statement which uses aggregation like MIN, MAX, COUNT etc.
        /// </summary>
        /// <returns>true - has aggregations, false - has no aggregations</returns>
        public bool HasColumnsWithAggregations()
        {
            foreach (Column o_column in this.Columns)
            {
                if (!ForestNET.Lib.Helper.IsStringEmpty(o_column.SqlAggregation))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// create select sql string query
        /// </summary>
        public override string ToString()
        {
            string s_foo;

            try
            {
                if (this.Columns.Count <= 0)
                {
                    throw new Exception("Columns object list is empty");
                }

                s_foo = "SELECT ";

                /* use DISTINCT expression */
                if (this.Distinct)
                {
                    s_foo += " DISTINCT ";
                }

                /* add all column with ', ' separator to query */
                foreach (Column o_column in this.Columns)
                {
                    s_foo += o_column.ToString() + ", ";
                }

                /* remove last ', ' separator */
                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                bool b_exc = false;

                /* add table depending on database gateway */
                switch (this.BaseGateway)
                {
                    case BaseGateway.MARIADB:
                    case BaseGateway.SQLITE:
                    case BaseGateway.NOSQLMDB:
                        s_foo += " FROM " + "`" + this.Table + "`";
                        break;
                    case BaseGateway.MSSQL:
                        s_foo += " FROM " + "[" + this.Table + "]";
                        break;
                    case BaseGateway.PGSQL:
                    case BaseGateway.ORACLE:
                        s_foo += " FROM " + "\"" + this.Table + "\"";
                        break;
                    default:
                        b_exc = true;
                        break;
                }

                if (b_exc)
                {
                    throw new Exception("BaseGateway[" + this.BaseGateway + "] not implemented");
                }

                /* add join clauses to query */
                if (this.Joins.Count > 0)
                {
                    foreach (Join o_join in this.Joins)
                    {
                        s_foo += " " + o_join.ToString();
                    }
                }

                /* add where clauses to query */
                if (this.Where.Count > 0)
                {
                    s_foo += " WHERE ";

                    foreach (Where o_where in this.Where)
                    {
                        s_foo += o_where.ToString();
                    }
                }

                /* add group by clauses to query */
                if (this.GroupBy.Count > 0)
                {
                    s_foo += " GROUP BY ";

                    foreach (Column o_groupBy in this.GroupBy)
                    {
                        s_foo += o_groupBy.ToString(false) + ", ";
                    }

                    /* remove last ', ' separator */
                    s_foo = s_foo.Substring(0, s_foo.Length - 2);

                    /* add having clauses to query */
                    if (this.Having.Count > 0)
                    {
                        s_foo += " HAVING ";

                        foreach (Where o_having in this.Having)
                        {
                            s_foo += o_having.ToString();
                        }
                    }
                }

                /* add order by clause to query */
                if ((this.OrderBy != null) && (this.OrderBy.Amount > 0))
                {
                    s_foo += this.OrderBy.ToString();
                }

                /* add limit clauses to query */
                if ((this.Limit != null) && (this.Limit.Interval != 0))
                {
                    s_foo += this.Limit.ToString();
                }
            }
            catch (Exception o_exc)
            { /* just set exception as query return, so database interface will have an exception as well */
                s_foo = " >>>>> Select class Exception: [" + o_exc + "] <<<<< ";
            }

            return s_foo;
        }
    }
}