namespace ForestNET.Lib.SQL
{
    /// <summary>
    /// Filter class holding basic information like column and filter value, but also operator and/or filter operator.
    /// </summary>
    public class Filter
    {

        /* Fields */

        /* Properties */

        public string Column;
        public Object Value;
        public string Operator;
        public string? FilterOperator;

        /* Methods */

        /// <summary>
        /// constructor leaving filter operator as null
        /// </summary>
        /// <param name="p_s_column">column name in table structure</param>
        /// <param name="p_o_value">filter value, could be any kind</param>
        /// <param name="p_s_operator">operator between column and value, e.g. '=', '<>', '>', ...</param>
        public Filter(string p_s_column, Object p_o_value, string p_s_operator) : this(p_s_column, p_o_value, p_s_operator, null)
        {

        }

        /// <summary>
        /// constructor which sets all class properties
        /// </summary>
        /// <param name="p_s_column">column name in table structure</param>
        /// <param name="p_o_value">filter value, could be any kind</param>
        /// <param name="p_s_operator">operator between column and value, e.g. '=', '<>', '>', ...</param>
        /// <param name="p_s_filterOperator">operator between multiple filter objects, e.g. 'AND', 'OR', ...</param>
        public Filter(string p_s_column, Object p_o_value, string p_s_operator, string? p_s_filterOperator)
        {
            this.Column = p_s_column;
            this.Value = p_o_value;
            this.Operator = p_s_operator;
            this.FilterOperator = p_s_filterOperator;
        }
    }
}
