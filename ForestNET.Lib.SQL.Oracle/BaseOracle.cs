using Oracle.ManagedDataAccess.Client;

namespace ForestNET.Lib.SQL.Oracle
{
    /// <summary>
    /// BaseOracle class for sql connection to a database and fetching queries to it.
    /// requires ADO.NET(Oracle.ManagedDataAccess.Core).
    /// </summary>
    public class BaseOracle : Base
    {

        /* Fields */

        private OracleConnection? o_currentConnection = null;
        private OracleCommand? o_currentStatement = null;
        private OracleTransaction? o_currentTransaction = null;
        private OracleDataReader? o_currentResult = null;

        /* Properties */

        /* Methods */

        /// <summary>
        /// BaseOracle constructor, initiating database server connection
        /// </summary>
        /// <param name="p_s_host">address of database server, optional port specification e.g. 'localhost:3306'</param>
        /// <exception cref="ArgumentException">illegal database gateway or illegal host parameter</exception>
        /// <exception cref="AccessViolationException">error creating database server connection</exception>
        public BaseOracle(string p_s_host) : this(p_s_host, "")
        {

        }

        /// <summary>
        /// BaseOracle constructor, initiating database server connection
        /// </summary>
        /// <param name="p_s_host">address of database server, optional port specification e.g. 'localhost:3306'</param>
        /// <param name="p_s_instance">parameter for selecting a database instance within target database server</param>
        /// <exception cref="ArgumentException">illegal database gateway or illegal host parameter</exception>
        /// <exception cref="AccessViolationException">error creating database server connection</exception>
        public BaseOracle(string p_s_host, string p_s_instance) : this(p_s_host, p_s_instance, "", "")
        {

        }

        /// <summary>
        /// BaseOracle constructor, initiating database server connection
        /// </summary>
        /// <param name="p_s_host">address of database server, optional port specification e.g. 'localhost:3306'</param>
        /// <param name="p_s_instance">parameter for selecting a database instance within target database server</param>
        /// <param name="p_s_user">user name for database login</param>
        /// <exception cref="ArgumentException">illegal database gateway or illegal host parameter</exception>
        /// <exception cref="AccessViolationException">error creating database server connection</exception>
        public BaseOracle(string p_s_host, string p_s_instance, string p_s_user) : this(p_s_host, p_s_instance, p_s_user, "")
        {

        }

        /// <summary>
        /// BaseOracle constructor, initiating database server connection
        /// </summary>
        /// <param name="p_s_host">address of database server, optional port specification e.g. 'localhost:3306'</param>
        /// <param name="p_s_instance">parameter for selecting a database instance within target database server</param>
        /// <param name="p_s_user">user name for database login</param>
        /// <param name="p_s_password">user's password for database login</param>
        /// <exception cref="ArgumentException">illegal database gateway or illegal host parameter</exception>
        /// <exception cref="AccessViolationException">error creating database server connection</exception>
        public BaseOracle(string p_s_host, string p_s_instance, string p_s_user, string p_s_password)
        {
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_host))
            {
                throw new ArgumentException("No base-host/base-file was selected");
            }

            /* choose database gateway and create new connection object with connection settings */
            try
            {
                ForestNET.Lib.Global.ILogConfig("try database connection with: '" + p_s_host + "' and database: '" + p_s_instance + "'");

                string s_connectionString = "";
                string s_host = "";
                string s_port = "";
                string s_instance = "";

                /* split host parameter to host and port part and instance part */
                if ((p_s_host.Contains(':')) && (!p_s_host.Contains('/')))
                { /* only host and port part */
                    string[] a_host = p_s_host.Split(':');
                    s_host = a_host[0];
                    s_port = a_host[1];
                }
                else if ((!p_s_host.Contains(':')) && (p_s_host.Contains('/')))
                { /* only host and instance part */
                    string[] a_host = p_s_host.Split('\\');
                    s_host = a_host[0];
                    s_instance = a_host[1];
                }
                else if ((p_s_host.Contains(':')) && (p_s_host.Contains('/')))
                { /* host, port and instance part */
                    string[] a_host = p_s_host.Split(':');
                    s_host = a_host[0];
                    s_port = a_host[1];

                    string[] a_host2 = p_s_host.Split('/');
                    s_instance = a_host2[1];
                }
                else /* we only have host part without a port or instance */
                {
                    s_host = p_s_host;
                }

                /* add server to connection string */
                s_connectionString += "Data Source = " + s_host;

                /* add port to connection string */
                if (!ForestNET.Lib.Helper.IsStringEmpty(s_port))
                {
                    s_connectionString += "," + s_port;
                }

                /* add oracle instance to connection string */
                if (!ForestNET.Lib.Helper.IsStringEmpty(p_s_instance))
                {
                    s_connectionString += "/" + p_s_instance;
                }
                else if (!ForestNET.Lib.Helper.IsStringEmpty(s_instance))
                {
                    s_connectionString += "/" + s_instance;
                }

                s_connectionString += ";";

                /* add user to connection string */
                if (!ForestNET.Lib.Helper.IsStringEmpty(p_s_user))
                {
                    s_connectionString += "User ID = " + p_s_user + ";";
                }

                /* add password to connection string */
                if (!ForestNET.Lib.Helper.IsStringEmpty(p_s_password))
                {
                    s_connectionString += "Password = " + p_s_password + ";";
                }

                /* remove last ';' character */
                s_connectionString = s_connectionString.Substring(0, s_connectionString.Length - 1);

                /* create oracle connection with connection string */
                this.o_currentConnection = new(s_connectionString);
                this.o_currentConnection.Open();
                this.o_currentTransaction = this.o_currentConnection.BeginTransaction();

                ForestNET.Lib.Global.ILogConfig("database connection to '" + BaseGateway.ORACLE + "' established");
            }
            catch (Exception o_exc)
            {
                throw new AccessViolationException("The connection to the database is not possible; " + o_exc);
            }
        }

        /// <summary>
        /// Test connection to database with current connection
        /// </summary>
        /// <returns>true - connection successful, false - connection failed</returns>
        public override bool TestConnection()
        {
            try
            {
                this.o_currentStatement = this.o_currentConnection?.CreateCommand();
                (this.o_currentStatement ?? throw new NullReferenceException("could not create and use sql statement")).CommandText = "SELECT 1 FROM DUAL";
                this.o_currentStatement.Transaction = this.o_currentTransaction;

                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("test connection with '" + this.o_currentStatement.CommandText + "'");

                using OracleDataReader o_tempResult = this.o_currentStatement.ExecuteReader();
                if (o_tempResult.Read())
                {
                    return true;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Check if current connection is not closed
        /// </summary>
        /// <returns>true - connection is still valid, false - connection closed, a new one must be established to continue database communication</returns>
        public override bool IsClosed()
        {
            return this.o_currentConnection?.State == System.Data.ConnectionState.Closed;
        }

        /// <summary>
        /// Method for closing database connection, ignoring any sql exceptions
        /// </summary>
        public override void CloseConnection()
        {
            if (this.o_currentResult != null)
            {
                try { this.o_currentResult.Close(); } catch (Exception) { /* ignored */ }
                this.o_currentResult = null;
            }

            if (this.o_currentTransaction != null)
            {
                try { this.o_currentTransaction.Dispose(); } catch (Exception) { /* ignored */ }
                this.o_currentTransaction = null;
            }

            if (this.o_currentStatement != null)
            {
                try { this.o_currentStatement.Cancel(); this.o_currentStatement.Dispose(); } catch (Exception) { /* ignored */ }
                this.o_currentStatement = null;
            }

            if (this.o_currentConnection != null)
            {
                try { OracleConnection.ClearPool(this.o_currentConnection); this.o_currentConnection.Close(); this.o_currentConnection.Dispose(); } catch (Exception) { /* ignored */ }
                this.o_currentConnection = null;
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();

            ForestNET.Lib.Global.ILogConfig("database connection is closed");
        }

        /// <summary>
        /// Fetch a query or a amount of queries separated by QueryAbstract.QuerySeparator, with auto commit
        /// </summary>
        /// <param name="p_o_sqlQuery">query object of Query class</param>
        /// <returns>list of hash maps, key(string) -> column name + value(object) -> column value of a database record</returns>
        /// <exception cref="AccessViolationException">exception accessing column type, column name or just column value of current result set record</exception>
        public override List<Dictionary<string, Object?>> FetchQuery(IQuery? p_o_sqlQuery)
        {
            return this.FetchQuery(p_o_sqlQuery, true);
        }

        /// <summary>
        /// Fetch a query or a amount of queries separated by QueryAbstract.QuerySeparator
        /// </summary>
        /// <param name="p_o_sqlQuery">query object of Query class</param>
        /// <param name="p_b_autoCommit">commit flag: true - commit database after each execution of query object automatically, false - do not commit automatically</param>
        /// <returns>list of hash maps, key(string) -> column name + value(object) -> column value of a database record</returns>
        /// <exception cref="AccessViolationException">exception accessing column type, column name or just column value of current result set record</exception>
        public override List<Dictionary<string, Object?>> FetchQuery(IQuery? p_o_sqlQuery, bool p_b_autoCommit)
        {
            if (this.o_currentConnection == null)
            {
                throw new NullReferenceException("connection object is null");
            }

            /* prepare return value */
            List<Dictionary<string, Object?>> a_rows = [];

            bool b_ignoreResult = false;
            int i_nonQueryResult = -1;

            /* check if query BaseGateway matches BaseGateway of current connection */
            if (p_o_sqlQuery?.BaseGateway != BaseGateway.ORACLE)
            {
                throw new AccessViolationException("Query has invalid BaseGateway setting: '" + p_o_sqlQuery?.BaseGateway + "(query)' != '" + BaseGateway.ORACLE + "(base)'");
            }

            /* convert sql query object to string and execute sql query */
            this.s_query = p_o_sqlQuery.ToString();

            /* remove query separator at the end of query string */
            if (this.s_query.EndsWith(p_o_sqlQuery.GetQuery<IQueryAbstract>()?.GetQuerySeparator ?? "query separator is null"))
            {
                this.s_query = this.s_query.Substring(0, this.s_query.Length - p_o_sqlQuery.GetQuery<IQueryAbstract>()?.GetQuerySeparator.Length ?? 0);
            }

            try
            {
                /* check if we have multiple queries in one statement */
                string[] a_queries = this.s_query.Split(p_o_sqlQuery.GetQuery<IQueryAbstract>()?.GetQuerySeparator ?? "query separator is null");

                /* execute every query one by one */
                for (int i = 0; i < a_queries.Length; i++)
                {
                    if (!ForestNET.Lib.Helper.IsStringEmpty(a_queries[i]))
                    {
                        /* fetch user input values out of query for prepared statement and safe execution */
                        List<KeyValuePair<string, Object>> a_values = [];
                        string s_queryFoo = Query<Column>.ConvertToPreparedStatementQuery(BaseGateway.ORACLE, a_queries[i].ToString(), a_values, false);

                        /* store query for exception purpose */
                        this.s_query = Query<Column>.ConvertPreparedStatementSqlQueryToStandard(s_queryFoo, a_values);

                        /* log whole query on finer level - warning has been written in the log as setting this flag true, because with insecure log access this is a vulnerability! */
                        if (ForestNET.Lib.Global.Instance.LogCompleteSqlQuery)
                        {
                            ForestNET.Lib.Global.ILogFiner("sql query: '" + this.s_query + "'");
                        }

                        /* with respect to oracle, using ':' with parameter name instead of '@' */
                        s_queryFoo = s_queryFoo.Replace("@", ":");

                        /* prepare query */
                        this.o_currentStatement = this.o_currentConnection.CreateCommand();
                        this.o_currentStatement.CommandText = s_queryFoo;
                        this.o_currentStatement.InitialLONGFetchSize = -1; /* somehow we need this if we want to read long columns and use GetString with OracleDataReader */
                        this.o_currentStatement.Transaction = this.o_currentTransaction;

                        /* prepare query */
                        this.o_currentStatement.Prepare();

                        int i_indexOfValue = 1;

                        /* pass values to sql statement */
                        foreach (KeyValuePair<string, Object> o_valuePair in a_values)
                        {
                            string s_type = o_valuePair.Key;
                            Object o_foo = o_valuePair.Value;

                            if (s_type.Equals("bool"))
                            {
                                ForestNET.Lib.Global.ILogFinest("add 'bool' value to current statement" + ((ForestNET.Lib.Global.Instance.LogCompleteSqlQuery) ? " [" + o_foo.ToString() + "]" : ""));
                                this.o_currentStatement.Parameters.Add(new OracleParameter("" + i_indexOfValue.ToString("D5"), Convert.ToBoolean(o_foo)));
                            }
                            else if (s_type.Equals("sbyte"))
                            {
                                ForestNET.Lib.Global.ILogFinest("add 'sbyte' value to current statement" + ((ForestNET.Lib.Global.Instance.LogCompleteSqlQuery) ? " [" + o_foo.ToString() + "]" : ""));
                                this.o_currentStatement.Parameters.Add(new OracleParameter("" + i_indexOfValue.ToString("D5"), Convert.ToSByte(o_foo)));
                            }
                            else if (s_type.Equals("short"))
                            {
                                ForestNET.Lib.Global.ILogFinest("add 'short' value to current statement" + ((ForestNET.Lib.Global.Instance.LogCompleteSqlQuery) ? " [" + o_foo.ToString() + "]" : ""));
                                this.o_currentStatement.Parameters.Add(new OracleParameter("" + i_indexOfValue.ToString("D5"), Convert.ToInt16(o_foo)));
                            }
                            else if (s_type.Equals("integer"))
                            {
                                ForestNET.Lib.Global.ILogFinest("add 'integer' value to current statement" + ((ForestNET.Lib.Global.Instance.LogCompleteSqlQuery) ? " [" + o_foo.ToString() + "]" : ""));
                                this.o_currentStatement.Parameters.Add(new OracleParameter("" + i_indexOfValue.ToString("D5"), Convert.ToInt32(o_foo)));
                            }
                            else if (s_type.Equals("long"))
                            {
                                ForestNET.Lib.Global.ILogFinest("add 'long' value to current statement" + ((ForestNET.Lib.Global.Instance.LogCompleteSqlQuery) ? " [" + o_foo.ToString() + "]" : ""));
                                this.o_currentStatement.Parameters.Add(new OracleParameter("" + i_indexOfValue.ToString("D5"), Convert.ToInt64(o_foo)));
                            }
                            else if (s_type.Equals("byte"))
                            {
                                ForestNET.Lib.Global.ILogFinest("add 'byte' value to current statement" + ((ForestNET.Lib.Global.Instance.LogCompleteSqlQuery) ? " [" + o_foo.ToString() + "]" : ""));
                                this.o_currentStatement.Parameters.Add(new OracleParameter("" + i_indexOfValue.ToString("D5"), Convert.ToByte(o_foo)));
                            }
                            else if (s_type.Equals("ushort"))
                            {
                                ForestNET.Lib.Global.ILogFinest("add 'ushort' value to current statement" + ((ForestNET.Lib.Global.Instance.LogCompleteSqlQuery) ? " [" + o_foo.ToString() + "]" : ""));
                                this.o_currentStatement.Parameters.Add(new OracleParameter("" + i_indexOfValue.ToString("D5"), Convert.ToUInt16(o_foo)));
                            }
                            else if (s_type.Equals("uinteger"))
                            {
                                ForestNET.Lib.Global.ILogFinest("add 'uinteger' value to current statement" + ((ForestNET.Lib.Global.Instance.LogCompleteSqlQuery) ? " [" + o_foo.ToString() + "]" : ""));
                                this.o_currentStatement.Parameters.Add(new OracleParameter("" + i_indexOfValue.ToString("D5"), Convert.ToUInt32(o_foo)));
                            }
                            else if (s_type.Equals("ulong"))
                            {
                                ForestNET.Lib.Global.ILogFinest("add 'ulong' value to current statement" + ((ForestNET.Lib.Global.Instance.LogCompleteSqlQuery) ? " [" + o_foo.ToString() + "]" : ""));
                                this.o_currentStatement.Parameters.Add(new OracleParameter("" + i_indexOfValue.ToString("D5"), Convert.ToUInt64(o_foo)));
                            }
                            else if (s_type.Equals("float"))
                            {
                                ForestNET.Lib.Global.ILogFinest("add 'float' value to current statement" + ((ForestNET.Lib.Global.Instance.LogCompleteSqlQuery) ? " [" + o_foo.ToString() + "]" : ""));
                                this.o_currentStatement.Parameters.Add(new OracleParameter("" + i_indexOfValue.ToString("D5"), Convert.ToSingle(o_foo)));
                            }
                            else if (s_type.Equals("double"))
                            {
                                ForestNET.Lib.Global.ILogFinest("add 'double' value to current statement" + ((ForestNET.Lib.Global.Instance.LogCompleteSqlQuery) ? " [" + o_foo.ToString() + "]" : ""));
                                this.o_currentStatement.Parameters.Add(new OracleParameter("" + i_indexOfValue.ToString("D5"), Convert.ToDouble(o_foo)));
                            }
                            else if (s_type.Equals("decimal"))
                            {
                                ForestNET.Lib.Global.ILogFinest("add 'decimal' value to current statement" + ((ForestNET.Lib.Global.Instance.LogCompleteSqlQuery) ? " [" + o_foo.ToString() + "]" : ""));
                                this.o_currentStatement.Parameters.Add(new OracleParameter("" + i_indexOfValue.ToString("D5"), Convert.ToDecimal(o_foo)));
                            }
                            else if (s_type.Equals("localdate"))
                            { /* use "yyyy-MM-dd" string */
                                ForestNET.Lib.Global.ILogFinest("add 'localdate' value to current statement" + ((ForestNET.Lib.Global.Instance.LogCompleteSqlQuery) ? " [" + o_foo.ToString() + "]" : ""));
                                this.o_currentStatement.Parameters.Add(new OracleParameter("" + i_indexOfValue.ToString("D5"), Convert.ToDateTime(o_foo).ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)));
                            }
                            else if (s_type.Equals("localdatetime"))
                            { /* use "yyyy-MM-dd HH:mm:ss" string */
                                ForestNET.Lib.Global.ILogFinest("add 'localdatetime' value to current statement" + ((ForestNET.Lib.Global.Instance.LogCompleteSqlQuery) ? " [" + o_foo.ToString() + "]" : ""));
                                this.o_currentStatement.Parameters.Add(new OracleParameter("" + i_indexOfValue.ToString("D5"), Convert.ToDateTime(o_foo).ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)));
                            }
                            else if (s_type.Equals("localtime"))
                            { /* use "hh\\:mm\\:ss" string */
                                ForestNET.Lib.Global.ILogFinest("add 'localtime' value to current statement" + ((ForestNET.Lib.Global.Instance.LogCompleteSqlQuery) ? " [" + o_foo.ToString() + "]" : ""));
                                this.o_currentStatement.Parameters.Add(new OracleParameter("" + i_indexOfValue.ToString("D5"), ((TimeSpan)o_foo).ToString("hh\\:mm\\:ss", System.Globalization.CultureInfo.InvariantCulture)));
                            }
                            else if (s_type.Equals("string"))
                            {
                                ForestNET.Lib.Global.ILogFinest("add 'string' value to current statement" + ((ForestNET.Lib.Global.Instance.LogCompleteSqlQuery) ? " [" + o_foo.ToString() + "]" : ""));

                                if (((o_foo.ToString() ?? "").ToLower().Equals("true")) || ((o_foo.ToString() ?? "").ToLower().Equals("false")))
                                {
                                    this.o_currentStatement.Parameters.Add(new OracleParameter("" + i_indexOfValue.ToString("D5"), Convert.ToBoolean(o_foo)));
                                }
                                else if ((o_foo.ToString() ?? "").ToUpper().Equals("NULL"))
                                {
                                    this.o_currentStatement.Parameters.Add(new OracleParameter("" + i_indexOfValue.ToString("D5"), null));
                                }
                                else
                                {
                                    this.o_currentStatement.Parameters.Add(new OracleParameter("" + i_indexOfValue.ToString("D5"), Convert.ToString(o_foo)));
                                }
                            }
                            else
                            {
                                ForestNET.Lib.Global.ILogFinest("add 'object' value to current statement" + ((ForestNET.Lib.Global.Instance.LogCompleteSqlQuery) ? " [" + o_foo.ToString() + "]" : ""));
                                this.o_currentStatement.Parameters.Add(new OracleParameter("" + i_indexOfValue.ToString("D5"), o_foo));
                            }

                            i_indexOfValue++;
                        }

                        /* execute query */
                        if (p_o_sqlQuery.SqlType == SqlType.SELECT)
                        {
                            if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("execute prepared statement");
                            this.o_currentResult = this.o_currentStatement.ExecuteReader();
                            if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("prepared statement executed");
                        }
                        else
                        {
                            try
                            {
                                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("execute prepared update statement");
                                i_nonQueryResult = this.o_currentStatement.ExecuteNonQuery();
                                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("prepared update statement executed");
                            }
                            catch (OracleException)
                            {
                                /* oracle and alter query only support executeQuery method */
                                if (p_o_sqlQuery.SqlType == SqlType.ALTER)
                                {
                                    if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("execute prepared alter statement for " + BaseGateway.ORACLE);
                                    this.o_currentResult = this.o_currentStatement.ExecuteReader();
                                    if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("prepared alter statement executed for " + BaseGateway.ORACLE);
                                }
                                else
                                {
                                    throw;
                                }
                            }
                            finally
                            {
                                b_ignoreResult = true;
                            }
                        }
                    }
                    else
                    {
                        ForestNET.Lib.Global.ILogFine("empty query string skipped, query #" + i);
                    }
                }
            }
            catch (OracleException o_exc)
            {
                /* if commit flag is set, roll-back transaction */
                if (p_b_autoCommit)
                {
                    try
                    {
                        this.o_currentStatement?.Transaction?.Rollback();
                        this.o_currentTransaction = this.o_currentConnection.BeginTransaction();
                    }
                    catch (OracleException o_exc2)
                    {
                        throw new AccessViolationException("Could not rollback transaction; " + o_exc2);
                    }
                }

                throw new AccessViolationException("The query could not be executed; " + o_exc + ";" + Environment.NewLine + "Query=" + this.s_query);
            }

            /* check if query got a result */
            if ((this.o_currentResult == null) && (!b_ignoreResult))
            {
                /* if commit flag is set, roll-back transaction */
                if (p_b_autoCommit)
                {
                    try
                    {
                        this.o_currentStatement?.Transaction?.Rollback();
                        this.o_currentTransaction = this.o_currentConnection.BeginTransaction();
                    }
                    catch (OracleException o_exc)
                    {
                        throw new AccessViolationException("Could not rollback transaction; " + o_exc);
                    }
                }

                throw new AccessViolationException("The query could not be executed or has no valid result.");
            }

            /* on SELECT query, prepare to return result rows */
            if ((p_o_sqlQuery.SqlType == SqlType.SELECT) && (this.o_currentResult != null))
            {
                try
                {
                    /* fetch sql result into hash map array */
                    while (this.o_currentResult.Read())
                    {
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("fetch row");
                        a_rows.Add(this.FetchRow());
                    }
                }
                catch (OracleException o_exc)
                {
                    throw new AccessViolationException("Could not fetch row, issue get metadata and column information; " + o_exc + ";" + Environment.NewLine + "Query=" + this.s_query);
                }
            }
            else
            {
                /* sql query to get last insert id */
                string s_lastInsertIdQuery = "SELECT DATA_DEFAULT FROM USER_TAB_COLUMNS WHERE TABLE_NAME = '" + p_o_sqlQuery.Table + "'";

                try
                {
                    if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("get update count");

                    /* returning amount of affected rows by sql query (and last inserted id if insert statement) */
                    Dictionary<string, Object?> o_row = new()
                    {
                        { "AffectedRows", i_nonQueryResult }
                    };

                    ForestNET.Lib.Global.ILogFinest("update count is '" + o_row["AffectedRows"] + "'");

                    int i_lastInsertId = -1;

                    /* returning last inserted id, some are simple with ADO.NET support, others are very complex */
                    if ((p_o_sqlQuery.SqlType == SqlType.INSERT) && (!this.SkipQueryLastInsertId))
                    {
                        /* we need to find the column in ORACLE with has a sequence */
                        this.o_currentStatement = this.o_currentConnection.CreateCommand();
                        this.o_currentStatement.CommandText = s_lastInsertIdQuery;
                        this.o_currentStatement.InitialLONGFetchSize = -1; /* somehow we need this if we use 'SELECT DATA_DEFAULT FROM USER_TAB_COLUMNS...' */
                        this.o_currentStatement.Transaction = this.o_currentTransaction;
                        this.o_currentStatement.Prepare();

                        string? s_sequence = null;

                        /* iterate each column result */
                        using (OracleDataReader o_tempResult = this.o_currentStatement.ExecuteReader())
                        {
                            while (o_tempResult.Read())
                            {
                                /* skip null field */
                                if (o_tempResult.IsDBNull(0))
                                {
                                    continue;
                                }

                                string s_columnDefaultValue = o_tempResult.GetString(0);

                                /* sequence ends with '.nextval' */
                                if (s_columnDefaultValue.ToLower().EndsWith(".nextval"))
                                {
                                    /* get sequence name, e.g. "SYSTEM"."ISEQ$$_74013".nextval */
                                    s_sequence = s_columnDefaultValue.Replace(".nextval", "");
                                    /* we always consider that the first column in table with a sequence is a value we want to have as last inserted identifier value */
                                    break;
                                }
                            }
                        }

                        /* continue if a sequence name has been found */
                        if (s_sequence != null)
                        {
                            /* get CURRVAL of found sequence */
                            s_lastInsertIdQuery = "SELECT " + s_sequence + ".CURRVAL FROM DUAL";
                            this.o_currentStatement = this.o_currentConnection.CreateCommand();
                            this.o_currentStatement.CommandText = s_lastInsertIdQuery;
                            this.o_currentStatement.Transaction = this.o_currentTransaction;
                            this.o_currentStatement.Prepare();

                            /* sequence must be of type integer/number */
                            using OracleDataReader o_tempResult = this.o_currentStatement.ExecuteReader();
                            if (o_tempResult.Read())
                            {
                                i_lastInsertId = Convert.ToInt32(o_tempResult.GetDecimal(0));
                            }
                        }

                        ForestNET.Lib.Global.ILogFinest("store LastInsertId '" + i_lastInsertId + "' in result row");
                        o_row.Add("LastInsertId", i_lastInsertId);
                    }

                    if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("add row object to result list");
                    a_rows.Add(o_row);
                }
                catch (OracleException o_exc)
                {
                    /* do not throw exception here, maybe only log this information */
                    ForestNET.Lib.Global.ILogSevere("Could not count affected rows or could not fetch last insert id; " + o_exc + ";" + Environment.NewLine + "Query=" + s_lastInsertIdQuery);
                }
            }

            /* close used data reader */
            this.o_currentResult?.Close();
            this.o_currentResult = null;

            /* if commit flag is set and we have not a select query, commit transaction */
            if ((p_b_autoCommit) && (p_o_sqlQuery.SqlType != SqlType.SELECT))
            {
                try
                {
                    this.o_currentStatement?.Transaction?.Commit();
                    this.o_currentTransaction = this.o_currentConnection.BeginTransaction();
                }
                catch (OracleException o_exc)
                {
                    throw new AccessViolationException("Could not commit transaction; " + o_exc);
                }
            }

            return a_rows;
        }

        /// <summary>
        /// Method to create a record dictionary of current result set record
        /// </summary>
        /// <returns>record dictionary, key(string) -> column name + value(object) -> column value</returns>
        /// <exception cref="OracleException">exception accessing column type, column name or just column value of current result set record</exception>
        private Dictionary<string, Object?> FetchRow()
        {
            /* our return record dictionary object */
            Dictionary<string, Object?> o_row = [];

            /* check if current result is null */
            if (this.o_currentResult == null)
            {
                throw new NullReferenceException("Current result is null - oracle data reader cannot be read");
            }

            /* iterate all columns */
            for (int i = 0; i < this.o_currentResult.FieldCount; i++)
            {
                string s_name = this.o_currentResult.GetName(i);
                string s_dataTypeName = this.o_currentResult.GetDataTypeName(i);

                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("column >>> " + s_name + " | " + s_dataTypeName);

                /* check if value is null */
                if (this.o_currentResult.IsDBNull(i))
                {
                    s_dataTypeName = "null";
                }

                /* call the correct get-methods from current result into our record dictionary object */
                switch (s_dataTypeName.ToLower())
                {
                    case "varchar":
                    case "varchar2":
                    case "text":
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("get column value with OracleDataReader.GetString");
                        o_row.Add(s_name, this.o_currentResult.GetString(i));
                        break;
                    case "byte":
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("get column value with OracleDataReader.GetByte");
                        o_row.Add(s_name, this.o_currentResult.GetByte(i));
                        break;
                    case "int32":
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("get column value with OracleDataReader.GetInt32");
                        /* read integer as short, because NUMBER(5) in oracle is integer, but we use it as short storage */
                        o_row.Add(s_name, Convert.ToInt16(this.o_currentResult.GetInt32(i)));
                        break;
                    case "int64":
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("get column value with OracleDataReader.GetInt64");
                        /* read long as integer, because NUMBER(10) in oracle is long, but we use it as integer storage */
                        o_row.Add(s_name, Convert.ToInt32(this.o_currentResult.GetInt64(i)));
                        break;
                    case "long":
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("get column value with OracleDataReader.GetString");
                        /* I do not know why, but oracle returns long columns as system.string, so we need to convert it to int64 */
                        /* empty string is 0 */
                        string s_long = this.o_currentResult.GetString(i);
                        o_row.Add(s_name, Convert.ToInt64(s_long.Length == 0 ? "0" : s_long));
                        break;
                    case "datetime":
                    case "timestamp":
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("get column value with OracleDataReader.GetDateTime");
                        DateTime o_dateTimeFoo = DateTime.MinValue;

                        if (!(this.o_currentResult.GetValue(i).ToString() ?? "").ToLower().Equals("null"))
                        {
                            o_dateTimeFoo = this.o_currentResult.GetDateTime(i);
                        }

                        if (o_dateTimeFoo.Equals(DateTime.MinValue))
                        {
                            o_row.Add(s_name, null);
                        }
                        else
                        {
                            o_row.Add(s_name, o_dateTimeFoo);
                        }
                        break;
                    case "intervalds":
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("get column value with OracleDataReader.GetTimeSpan");
                        TimeSpan o_timeSpanFoo = TimeSpan.MinValue;

                        if (!(this.o_currentResult.GetValue(i).ToString() ?? "").ToLower().Equals("null"))
                        {
                            o_timeSpanFoo = this.o_currentResult.GetTimeSpan(i);
                        }

                        if (o_timeSpanFoo.Equals(TimeSpan.MinValue))
                        {
                            o_row.Add(s_name, null);
                        }
                        else
                        {
                            o_row.Add(s_name, o_timeSpanFoo);
                        }
                        break;
                    case "binaryfloat":
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("get column value with OracleDataReader.GetFloat");
                        o_row.Add(s_name, this.o_currentResult.GetFloat(i));
                        break;
                    case "binarydouble":
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("get column value with OracleDataReader.GetDouble");
                        o_row.Add(s_name, this.o_currentResult.GetDouble(i));
                        break;
                    case "decimal":
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("get column value with OracleDataReader.GetDecimal");
                        decimal dec_foo = this.o_currentResult.GetDecimal(i);

                        if (ForestNET.Lib.Helper.IsInteger(dec_foo.ToString()))
                        { /* read decimal as integer, because oracle returns all number columns as .net decimal */
                            o_row.Add(s_name, Convert.ToInt32(dec_foo));
                        }
                        else if (ForestNET.Lib.Helper.IsLong(dec_foo.ToString()))
                        { /* read decimal as long, because oracle returns all number columns as .net decimal */
                            o_row.Add(s_name, Convert.ToInt64(dec_foo));
                        }
                        else
                        { /* just read as decimal */
                            o_row.Add(s_name, dec_foo);
                        }
                        break;
                    case "char":
                    case "bit":
                    case "bool":
                    case "boolean":
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("get column value with OracleDataReader.GetString");
                        /* we store boolean in oracle with CAHR(1), so we use GetString */
                        string s_foo = this.o_currentResult.GetString(i);

                        if (s_foo.Equals("1"))
                        {
                            /* store 'true' columns value */
                            o_row.Add(s_name, true);
                        }
                        else
                        {
                            /* store 'false' columns value */
                            o_row.Add(s_name, false);
                        }
                        break;
                    case "null":
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("get column value as null");
                        o_row.Add(s_name, null);
                        break;
                    default:
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("get column value as object");
                        o_row.Add(s_name, this.o_currentResult.GetValue(i));
                        break;
                }
            }

            return o_row;
        }

        /// <summary>
        /// Manual commit current transaction
        /// </summary>
        /// <exception cref="NullReferenceException">current transaction is null</exception>
        /// <exception cref="AccessViolationException">could not commit in current database connection</exception>
        public override void ManualCommit()
        {
            if (this.o_currentTransaction == null)
            {
                throw new NullReferenceException("current transaction is null");
            }

            try
            {
                this.o_currentTransaction.Commit();

                ForestNET.Lib.Global.ILogFinest("manual commit executed");
            }
            catch (Exception o_exc)
            {
                throw new AccessViolationException("Could not commit transaction; " + o_exc);
            }
        }

        /// <summary>
        /// Manual rollback current transaction
        /// </summary>
        /// <exception cref="NullReferenceException">current transaction is null</exception>
        /// <exception cref="AccessViolationException">could not rollback in current database connection</exception>
        public override void ManualRollback()
        {
            if (this.o_currentTransaction == null)
            {
                throw new NullReferenceException("current transaction is null");
            }

            try
            {
                this.o_currentTransaction.Rollback();

                ForestNET.Lib.Global.ILogFinest("manual rollback executed");
            }
            catch (Exception o_exc)
            {
                throw new AccessViolationException("Could not rollback transaction; " + o_exc);
            }
        }
    }
}
