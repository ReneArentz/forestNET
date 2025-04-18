namespace ForestNET.Lib.SQL
{
    /// <summary>
    /// Column value class with column and value property.
    /// </summary>
    public class ColumnValue : QueryAbstract
    {

        /* Fields */

        /* Properties */

        public Column? Column { get; set; } = null;
        public Object? Value { get; set; } = null;

        /* Methods */

        /// <summary>
        /// Constructor setting column and value to null
        /// </summary>
        /// <exception cref="ArgumentException">could not parse DateTime to string or invalid database gateway</exception>
        public ColumnValue() : this(null, null)
        {

        }

        /// <summary>
        /// Constructor setting column and value with parameters
        /// </summary>
        /// <param name="p_o_column">parameter column object</param>
        /// <param name="p_o_value">parmeter value object</param>
        /// <exception cref="ArgumentException">could not parse DateTime to string or invalid database gateway</exception>
        public ColumnValue(Column? p_o_column, Object? p_o_value) : base(null)
        { /* nothing to give to parent abstract class */

            if (p_o_column == null)
            {
                throw new NullReferenceException("Column parameter is null");
            }

            /* set necessary information from column object */
            this.BaseGateway = p_o_column.BaseGateway;
            this.SqlType = p_o_column.SqlType;
            this.Table = p_o_column.Table;

            this.Column = p_o_column;
            /* parse value with special method */
            this.Value = this.ParseValue(p_o_value);
        }

        /// <summary>
        /// need to override tostring method, just returning an empty string
        /// </summary>
        public override string ToString()
        {
            return string.Empty;
        }
    }
}