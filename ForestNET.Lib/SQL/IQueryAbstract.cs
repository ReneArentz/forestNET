namespace ForestNET.Lib.SQL
{
    /// <summary>
    /// Interface for query abstract class to use it as generic type in other classes
    /// </summary>
    public interface IQueryAbstract
    {
        public string? QueryString { get; set; }
        public BaseGateway BaseGateway { get; set; }
        public SqlType SqlType { get; set; }
        public string? Table { get; set; }
        public string GetQuerySeparator { get; }
        public string GetQueryValueTag { get; }

        public Object ParseValue(Object? p_o_value);
        public string ToString();
    }
}
