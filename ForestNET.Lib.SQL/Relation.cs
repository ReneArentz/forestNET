namespace ForestNET.Lib.SQL
{
    /// <summary>
    /// SQL class to generate a relation sql clause based on two column objects, operator and filter operator, called by ToString method.
    /// </summary>
    public class Relation : QueryAbstract
    {

        /* Fields */

        private string s_operator = "";
        private string s_filterOperator = "";

        /* Properties */

        public Column? ColumnLeft { get; set; } = null;
        public Column? ColumnRight { get; set; } = null;

        public bool BracketStart { get; set; } = false;
        public bool BracketEnd { get; set; } = false;

        public string Operator
        {
            set
            {
                bool b_accept = false;

                /* check if p_s_value is a valid sql operator */
                for (int i = 0; i < this.Operators.Length; i++)
                {
                    if (this.Operators[i] == value)
                    {
                        b_accept = true;
                    }
                }

                if (b_accept)
                {
                    this.s_operator = value;
                }
                else
                {
                    throw new ArgumentException("Value[" + value + "] is not in defined list[" + string.Join(", ", this.Operators) + "]");
                }
            }
        }

        public string FilterOperator
        {
            set
            {
                bool b_accept = false;

                /* check if p_s_value is a valid sql filter operator */
                for (int i = 0; i < this.FilterOperators.Length; i++)
                {
                    if (this.FilterOperators[i] == value)
                    {
                        b_accept = true;
                    }
                }

                if (b_accept)
                {
                    this.s_filterOperator = value;
                }
                else
                {
                    throw new ArgumentException("Value[" + value + "] is not in defined list[" + string.Join(", ", this.FilterOperators) + "]");
                }
            }
        }

        /* Methods */

        /// <summary>
        /// constructor will be called with Query object, because all necessary database gateway, sql type and table information are part of Query class
        /// </summary>
        /// <param name="p_o_query">query object with table and sql type information</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object or invalid operator or filter operator value</exception>
        public Relation(IQuery? p_o_query) :
            this(p_o_query, null, null, "", "", false, false)
        {

        }

        /// <summary>
        /// constructor will be called with Query object, because all necessary database gateway, sql type and table information are part of Query class
        /// </summary>
        /// <param name="p_o_query">query object with table and sql type information</param>
        /// <param name="p_o_columnLeft">left side column of relation</param>
        /// <param name="p_o_columnRight">right side column of relation</param>
        /// <param name="p_s_operator">operator between both column, e.g. '=', '<>', '>', ...</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object or invalid operator or filter operator value</exception>
        public Relation(IQuery? p_o_query, Column? p_o_columnLeft, Column? p_o_columnRight, string p_s_operator) :
            this(p_o_query, p_o_columnLeft, p_o_columnRight, p_s_operator, "", false, false)
        {

        }

        /// <summary>
        /// constructor will be called with Query object, because all necessary database gateway, sql type and table information are part of Query class
        /// </summary>
        /// <param name="p_o_query">query object with table and sql type information</param>
        /// <param name="p_o_columnLeft">left side column of relation</param>
        /// <param name="p_o_columnRight">right side column of relation</param>
        /// <param name="p_s_operator">operator between both column, e.g. '=', '<>', '>', ...</param>
        /// <param name="p_s_filterOperator">operator between multiple relation objects, e.g. 'AND', 'OR', ...</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object or invalid operator or filter operator value</exception>
        public Relation(IQuery? p_o_query, Column? p_o_columnLeft, Column? p_o_columnRight, string p_s_operator, string p_s_filterOperator) :
            this(p_o_query, p_o_columnLeft, p_o_columnRight, p_s_operator, p_s_filterOperator, false, false)
        {

        }

        /// <summary>
        /// constructor will be called with Query object, because all necessary database gateway, sql type and table information are part of Query class
        /// </summary>
        /// <param name="p_o_query">query object with table and sql type information</param>
        /// <param name="p_o_columnLeft">left side column of relation</param>
        /// <param name="p_o_columnRight">right side column of relation</param>
        /// <param name="p_s_operator">operator between both column, e.g. '=', '<>', '>', ...</param>
        /// <param name="p_s_filterOperator">operator between multiple relation objects, e.g. 'AND', 'OR', ...</param>
        /// <param name="p_b_bracketStart">flag to add '(' bracket before relation object</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object or invalid operator or filter operator value</exception>
        public Relation(IQuery? p_o_query, Column? p_o_columnLeft, Column? p_o_columnRight, string p_s_operator, string p_s_filterOperator, bool p_b_bracketStart) :
            this(p_o_query, p_o_columnLeft, p_o_columnRight, p_s_operator, p_s_filterOperator, p_b_bracketStart, false)
        {

        }

        /// <summary>
        /// constructor will be called with Query object, because all necessary database gateway, sql type and table information are part of Query class
        /// </summary>
        /// <param name="p_o_query">query object with table and sql type information</param>
        /// <param name="p_o_columnLeft">left side column of relation</param>
        /// <param name="p_o_columnRight">right side column of relation</param>
        /// <param name="p_s_operator">operator between both column, e.g. '=', '<>', '>', ...</param>
        /// <param name="p_s_filterOperator">operator between multiple relation objects, e.g. 'AND', 'OR', ...</param>
        /// <param name="p_b_bracketStart">flag to add '(' bracket before relation object</param>
        /// <param name="p_b_bracketEnd">flag to add ')' bracket after relation object</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object or invalid operator or filter operator value</exception>
        public Relation(IQuery? p_o_query, Column? p_o_columnLeft, Column? p_o_columnRight, string p_s_operator, string p_s_filterOperator, bool p_b_bracketStart, bool p_b_bracketEnd) :
            base(p_o_query)
        {
            if (p_o_columnLeft != null)
            {
                this.ColumnLeft = p_o_columnLeft;
            }

            if (p_o_columnRight != null)
            {
                this.ColumnRight = p_o_columnRight;
            }

            if (!ForestNET.Lib.Helper.IsStringEmpty(p_s_operator))
            {
                this.Operator = p_s_operator;
            }

            if (!ForestNET.Lib.Helper.IsStringEmpty(p_s_filterOperator))
            {
                this.FilterOperator = p_s_filterOperator;
            }

            if (p_b_bracketStart && p_b_bracketEnd)
            {
                throw new ArgumentException("Cannot add '(' and ')' bracket in the same relation object");
            }

            this.BracketStart = p_b_bracketStart;
            this.BracketEnd = p_b_bracketEnd;
        }

        /// <summary>
        /// create relation sql clause as string
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
                        /* add filter operator if it is not empty */
                        if (!ForestNET.Lib.Helper.IsStringEmpty(this.s_filterOperator))
                        {
                            s_foo = " " + this.s_filterOperator + " ";
                        }

                        if (this.BracketStart)
                        {
                            /* add '(' bracket */
                            s_foo += "(";
                        }

                        /* add relation to query */
                        s_foo += ((this.ColumnLeft?.ToString(false)) ?? throw new Exception("Column left for Relation is null")) +
                            " " + this.s_operator + " " +
                            ((this.ColumnRight?.ToString(false)) ?? throw new Exception("Column right for Relation is null"));

                        if (this.BracketEnd)
                        {
                            /* add ')' bracket */
                            s_foo += ")";
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
                s_foo = " >>>>> Relation class Exception: [" + o_exc + "] <<<<< ";
            }

            return s_foo;
        }
    }
}