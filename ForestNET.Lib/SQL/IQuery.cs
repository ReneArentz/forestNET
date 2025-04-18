namespace ForestNET.Lib.SQL
{
    /// <summary>
    /// Interface for query class to use it as generic type in other classes
    /// </summary>
    public interface IQuery
    {
        public BaseGateway BaseGateway { get; set; }
        public SqlType SqlType { get; set; }
        public string? Table { get; set; }

        public T2? GetQuery<T2>() where T2 : IQueryAbstract;

        public void SetQuery(string p_s_query);

        public string ToString();

        public string ConstraintTypeAllocation(string p_s_constraintType);
    }
}
