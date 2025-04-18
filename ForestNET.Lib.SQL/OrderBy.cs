namespace ForestNET.Lib.SQL
{
    /// <summary>
    /// SQL class to generate an order by sql clause based on a list of columns and sort directions, called by ToString method.
    /// </summary>
    public class OrderBy : QueryAbstract
    {

        /* Fields */

        /* Properties */

        public List<Column> Columns { get; private set; } = [];
        public List<bool> Directions { get; private set; } = [];

        public int Amount
        {
            get
            {
                return this.Columns.Count;
            }
        }

        /* Methods */

        /// <summary>
        /// constructor will be called with Query object, because all necessary database gateway, sql type and table information are part of Query class
        /// </summary>
        /// <param name="p_o_query">query object with table and sql type information</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object</exception>
        public OrderBy(IQuery? p_o_query) : this(p_o_query, null, null)
        {

        }

        /// <summary>
        /// constructor will be called with Query object, because all necessary database gateway, sql type and table information are part of Query class
        /// </summary>
        /// <param name="p_o_query">query object with table and sql type information</param>
        /// <param name="p_a_columns">list of columns for order by clause</param>
        /// <param name="p_a_directions">list of directions for order by clause, true - ASC, false - DESC</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object</exception>
        public OrderBy(IQuery? p_o_query, List<Column>? p_a_columns, List<bool>? p_a_directions) : base(p_o_query)
        {
            if ((p_a_columns != null) && (p_a_directions != null))
            {
                if (p_a_columns.Count != p_a_directions.Count)
                {
                    throw new ArgumentException("Both parameter lists do not have the same size");
                }

                /* assume list of columns */
                foreach (Column o_column in p_a_columns)
                {
                    this.Columns.Add(o_column);
                }

                /* assume list of directions */
                foreach (bool b_direction in p_a_directions)
                {
                    this.Directions.Add(b_direction);
                }
            }
            else if ((p_a_columns != null) && (p_a_directions == null))
            {
                /* assume list of columns, set direction always ASC */
                foreach (Column o_column in p_a_columns)
                {
                    this.Columns.Add(o_column);
                    this.Directions.Add(true);
                }
            }
        }

        public void AddColumn(Column p_o_column)
        {
            this.AddColumn(p_o_column, true);
        }

        public void AddColumn(Column p_o_column, bool p_b_direction)
        {
            this.Columns.Add(p_o_column);
            this.Directions.Add(p_b_direction);
        }

        /// <summary>
        /// create order by sql clause as string
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
                    case BaseGateway.MSSQL:
                    case BaseGateway.PGSQL:
                    case BaseGateway.ORACLE:
                    case BaseGateway.NOSQLMDB:
                        if (this.Columns.Count <= 0)
                        {
                            throw new Exception("Columns object list is empty");
                        }

                        s_foo = " ORDER BY ";
                        int i = -1;

                        /* add each column with direction ASC or DESC */
                        foreach (Column o_column in this.Columns)
                        {
                            s_foo += o_column;

                            if (this.Directions[++i])
                            {
                                s_foo += " ASC";
                            }
                            else
                            {
                                s_foo += " DESC";
                            }

                            /* add ', ' separator */
                            if (i < (this.Columns.Count - 1))
                            {
                                s_foo += ", ";
                            }
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
                s_foo = " >>>>> OrderBy class Exception: [" + o_exc + "] <<<<< ";
            }

            return s_foo;
        }
    }
}