namespace ForestNET.Lib.SQL
{
    /// <summary>
    /// Base class for any kind of sql connection to a database and fetching queries to it
    /// requires library/driver to connect to database system
    /// supported databases: see BaseGateway
    /// </summary>
    public abstract class Base
    {

        /* Fields */

        protected int i_amountQueries = 0;
        protected List<string> a_queries = [];
        protected string? s_query = null;
        protected string? s_tempConnectionString = null;

        /* Properties */

        public int AmountQueries
        {
            get
            {
                return this.i_amountQueries;
            }
        }

        public List<string> Queries
        {
            get
            {
                return this.a_queries;
            }
        }

        public bool SkipQueryLastInsertId { get; set; } = false;

        /* Methods */

        /// <summary>
        /// Test connection to database with current connection
        /// </summary>
        /// <returns>true - connection successful, false - connection failed</returns>
        abstract public bool TestConnection();

        /// <summary>
        /// Check if current connection is not closed
        /// </summary>
        /// <returns>true - connection is still valid, false - connection closed, a new one must be established to continue database communication</returns>
        abstract public bool IsClosed();

        /// <summary>
        /// Method for closing database connection, ignoring any sql exceptions
        /// </summary>
        abstract public void CloseConnection();

        /// <summary>
        /// Fetch a query or a amount of queries separated by QueryAbstract.s_querySeparator, with auto commit
        /// </summary>
        /// <param name="p_o_sqlQuery">query object of Query class</param>
        /// <returns>list of dictionaries, key(string) -> column name + value(object) -> column value of a database record</returns>
        /// <exception cref="AccessViolationException">exception accessing column type, column name or just column value of current result set record</exception>
        abstract public List<Dictionary<string, Object?>> FetchQuery(IQuery? p_o_sqlQuery);

        /// <summary>
        /// Fetch a query or a amount of queries separated by QueryAbstract.s_querySeparator
        /// </summary>
        /// <param name="p_o_sqlQuery">query object of Query class</param>
        /// <param name="p_b_autoCommit">commit flag: true - commit database after each execution of query object automatically, false - do not commit automatically</param>
        /// <returns>list of dictionaries, key(string) -> column name + value(object) -> column value of a database record</returns>
        /// <exception cref="AccessViolationException">exception accessing column type, column name or just column value of current result set record</exception>
        abstract public List<Dictionary<string, Object?>> FetchQuery(IQuery? p_o_sqlQuery, bool p_b_autoCommit);

        /// <summary>
        /// Manual commit current transaction
        /// </summary>
        /// <exception cref="AccessViolationException">could not commit in current database connection</exception>
        abstract public void ManualCommit();

        /// <summary>
        /// Manual rollback current transaction
        /// </summary>
        /// <exception cref="AccessViolationException">could not rollback in current database connection</exception>
        abstract public void ManualRollback();
    }
}
