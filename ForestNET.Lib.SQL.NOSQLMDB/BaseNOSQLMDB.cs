using MongoDB.Driver;
using MongoDB.Bson;

namespace ForestNET.Lib.SQL.NOSQLMDB
{
    /// <summary>
    /// DISCLAIMER: the author and all supporters of this library are clearly opposed to the use of the name of this database technology.
    /// <br />
    /// Some path and class names must be used, but wherever possible we will use the name 'nosqlmdb' instead.
    /// <br /><br />
    /// BaseNOSQLMDB class for connection to a database and fetching queries to it.
    /// requires MongoDB.Driver.
    /// </summary>
    public class BaseNOSQLMDB : Base
    {

        /* Fields */

        private MongoClient? o_currentConnection = null;
        private IMongoDatabase? o_currentDatabase = null;
        private IClientSession? o_currentSession = null;
        private BsonDocument? o_result = null;
        private int i_lastInsertId = -1;

        /* Properties */

        /// <summary>
        /// Disable auto commit - useful if we use standalone mdb instance
        /// </summary>
        public bool DisableAutoCommit { get; set; } = false;

        /* Methods */

        /// <summary>
        /// BaseNOSQLMDB constructor, initiating database server connection
        /// </summary>
        /// <param name="p_s_host">address of database server, optional port specification e.g. 'localhost:3306'</param>
        /// <exception cref="ArgumentException">illegal database gateway or illegal host parameter</exception>
        /// <exception cref="AccessViolationException">error creating database server connection</exception>
        public BaseNOSQLMDB(string p_s_host) : this(p_s_host, "")
        {

        }

        /// <summary>
        /// BaseNOSQLMDB constructor, initiating database server connection
        /// </summary>
        /// <param name="p_s_host">address of database server, optional port specification e.g. 'localhost:3306'</param>
        /// <param name="p_s_datasource">parameter for selecting a database within target database server</param>
        /// <exception cref="ArgumentException">illegal database gateway or illegal host parameter</exception>
        /// <exception cref="AccessViolationException">error creating database server connection</exception>
        public BaseNOSQLMDB(string p_s_host, string p_s_datasource) : this(p_s_host, p_s_datasource, "", "")
        {

        }

        /// <summary>
        /// BaseNOSQLMDB constructor, initiating database server connection
        /// </summary>
        /// <param name="p_s_host">address of database server, optional port specification e.g. 'localhost:3306'</param>
        /// <param name="p_s_datasource">parameter for selecting a database within target database server</param>
        /// <param name="p_s_user">user name for database login</param>
        /// <exception cref="ArgumentException">illegal database gateway or illegal host parameter</exception>
        /// <exception cref="AccessViolationException">error creating database server connection</exception>
        public BaseNOSQLMDB(string p_s_host, string p_s_datasource, string p_s_user) : this(p_s_host, p_s_datasource, p_s_user, "")
        {

        }

        /// <summary>
        /// BaseNOSQLMDB constructor, initiating database server connection
        /// </summary>
        /// <param name="p_s_host">address of database server, optional port specification e.g. 'localhost:3306'</param>
        /// <param name="p_s_datasource">parameter for selecting a database within target database server</param>
        /// <param name="p_s_user">user name for database login</param>
        /// <param name="p_s_password">user's password for database login</param>
        /// <exception cref="ArgumentException">illegal database gateway or illegal host parameter</exception>
        /// <exception cref="AccessViolationException">error creating database server connection</exception>
        public BaseNOSQLMDB(string p_s_host, string p_s_datasource, string p_s_user, string p_s_password)
        {
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_host))
            {
                throw new ArgumentException("No base-host/base-file was selected");
            }

            /* choose database gateway and create new connection object with connection settings */
            try
            {
                ForestNET.Lib.Global.ILogConfig("try database connection with: '" + p_s_host + "' and database: '" + p_s_datasource + "'");

                string s_connectionString = "mongodb://";
                string s_host = "";
                string s_port = "";

                /* check if we use 'DisableAutoCommit' parameter */
                if (p_s_host.EndsWith("|DisableAutoCommit"))
                {
                    this.DisableAutoCommit = true;
                    /* remove 'DisableAutoCommit' part */
                    p_s_host = p_s_host.Substring(0, p_s_host.Length - 18);
                }
                else if (p_s_host.EndsWith("| DisableAutoCommit"))
                {
                    this.DisableAutoCommit = true;
                    /* remove 'DisableAutoCommit' part */
                    p_s_host = p_s_host.Substring(0, p_s_host.Length - 19);
                }
                else if (p_s_host.EndsWith(" |DisableAutoCommit"))
                {
                    this.DisableAutoCommit = true;
                    /* remove 'DisableAutoCommit' part */
                    p_s_host = p_s_host.Substring(0, p_s_host.Length - 19);
                }
                else if (p_s_host.EndsWith(" | DisableAutoCommit"))
                {
                    this.DisableAutoCommit = true;
                    /* remove 'DisableAutoCommit' part */
                    p_s_host = p_s_host.Substring(0, p_s_host.Length - 20);
                }

                /* split host parameter to host and port part */
                if (p_s_host.Contains(':'))
                {
                    string[] a_host = p_s_host.Split(':');
                    s_host = a_host[0];
                    s_port = a_host[1];
                }
                else /* we only have host part without a port */
                {
                    s_host = p_s_host;
                }

                /* add user and password to connection string */
                if ((!ForestNET.Lib.Helper.IsStringEmpty(p_s_user)) && (!ForestNET.Lib.Helper.IsStringEmpty(p_s_password)))
                {
                    s_connectionString += p_s_user + ":" + p_s_password + "@";
                }

                /* add server to connection string */
                s_connectionString += s_host;

                /* add port to connection string */
                if (!ForestNET.Lib.Helper.IsStringEmpty(s_port))
                {
                    s_connectionString += ":" + s_port;
                }

                /* create nosqlmdb connection with connection string */
                this.o_currentConnection = new(s_connectionString);
                this.o_currentSession = this.o_currentConnection.StartSession();

                if (p_s_datasource != null)
                {
                    ForestNET.Lib.Global.ILogConfig("get database '" + p_s_datasource + "'");
                    this.o_currentDatabase = this.o_currentConnection.GetDatabase(p_s_datasource);

                    if (!this.TestConnection())
                    {
                        throw new Exception("Collections could not be listed");
                    }

                    ForestNET.Lib.Global.ILogConfig("database connection to '" + BaseGateway.NOSQLMDB + "' established");
                }
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
                return (this.o_currentDatabase ?? throw new NullReferenceException("NOSQLMDB connection to database is null")).RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait(1000);
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
            return this.TestConnection();
        }

        /// <summary>
        /// Method for closing database connection, ignoring any sql exceptions
        /// </summary>
        public override void CloseConnection()
        {
            if (this.o_currentDatabase != null)
            {
                try { this.o_currentDatabase = null; } catch (Exception) { /* ignored */ }
            }

            if (this.o_currentSession != null)
            {
                try { this.o_currentSession.Dispose(); } catch (Exception) { /* ignored */ }
                this.o_currentSession = null;
            }

            if (this.o_currentConnection != null)
            {
                try { this.o_currentConnection = null; } catch (Exception) { /* ignored */ }
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();

            ForestNET.Lib.Global.ILogConfig("database connection is closed");
        }

        /// <summary>
        /// Method to convert json string to a list of bson documents (as Dictionary&lt;string, object?&gt;)
        /// </summary>
        /// <param name="p_s_json">json string</param>
        /// <returns>List&lt;Dictionary&lt;string, object?&gt;&gt; list of bson documents</returns>
        public static List<Dictionary<string, Object?>> JSONtoListOfBSONDocuments(string p_s_json)
        {
            List<Dictionary<string, Object?>> o_return = [];

            foreach (BsonDocument o_temp in BsonDocument.Parse("{ \"list\":" + p_s_json + "}").GetElement("list").Value.AsBsonArray.Cast<BsonDocument>())
            {
                Dictionary<string, Object?> o_tempDictionary = [];

                foreach (BsonElement o_entry in o_temp.Elements)
                {
                    o_tempDictionary.Add(o_entry.Name, o_entry.Value);
                }

                o_return.Add(o_tempDictionary);
            }

            return o_return;
        }

        /// <summary>
        /// Method to log all results of a nosqlmdb command within MASS level
        /// </summary>
        /// <param name="p_o_result">bson document object</param>
        private static void LogCommandResults(BsonDocument? p_o_result)
        {
            string s_empty = "                                        ";

            if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("command results:");

            if (p_o_result == null)
            {
                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("\tresult is null");
            }
            else
            {
                /* log result bson document */
                foreach (BsonElement o_entry in p_o_result.Elements)
                {
                    if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass(
                        "\t" +
                        o_entry.Name + /* key value */
                        s_empty.Substring(0, s_empty.Length - o_entry.Name.Length) + /* white spaces */
                        o_entry.Value.GetType().FullName + /* type value */
                        s_empty.Substring(0, s_empty.Length - o_entry.Value.GetType().FullName?.Length ?? 0) + /* white spaces */
                        o_entry.Value /* value */
                    );
                }
            }
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
        /// <exception cref="ArgumentException">multiple sql queries, separated by query separator are not supported for nosqlmdb library</exception>
        public override List<Dictionary<string, Object?>> FetchQuery(IQuery? p_o_sqlQuery, bool p_b_autoCommit)
        {
            /* check if we want to disable auto commit - useful if we use standalone mdb instance */
            if (this.DisableAutoCommit)
            {
                p_b_autoCommit = false;
            }

            /* prepare return value */
            List<Dictionary<string, Object?>> a_rows = [];

            bool b_ignoreResult = false;

            /* check if query BaseGateway matches BaseGateway of current connection */
            if (p_o_sqlQuery?.BaseGateway != BaseGateway.NOSQLMDB)
            {
                throw new AccessViolationException("Query has invalid BaseGateway setting: '" + p_o_sqlQuery?.BaseGateway + "(query)' != '" + BaseGateway.NOSQLMDB + "(base)'");
            }

            try
            {
                /* convert sql query object to string and check for query separator, not supported for nosqlmdb library */
                if (p_o_sqlQuery.ToString().Contains(p_o_sqlQuery.GetQuery<IQueryAbstract>()?.GetQuerySeparator ?? "query separator is null"))
                {
                    throw new ArgumentException("Multiple sql queries, separated by query separator are not supported for nosqlmdb library");
                }

                /* automatically start manual transaction */
                if (p_b_autoCommit)
                {
                    this.ManualStartTransaction();
                }

                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("transpose sql query to nosqlmdb command(s)");

                List<KeyValuePair<string, Object>> a_values = [];

                string s_queryFoo = Query<Column>.ConvertToPreparedStatementQuery(p_o_sqlQuery.BaseGateway, p_o_sqlQuery.ToString(), a_values, false);

                List<BsonDocument> a_commands = BaseNoSQLMDBTranspose.Transpose(p_o_sqlQuery) ?? throw new NullReferenceException("Sql query could not generate nosqlmdb commands");

                /* log nosqlmdb commands */
                if (ForestNET.Lib.Global.Instance.LogCompleteSqlQuery)
                {
                    foreach (BsonDocument o_command in a_commands)
                    {
                        ForestNET.Lib.Global.ILogFiner(o_command.ToJson());
                    }
                }

                if (p_o_sqlQuery.SqlType == SqlType.INSERT)
                { /* execute insert query */
                    try
                    {
                        BsonDocument o_insertCommand = this.GetInsertCommandWithAutoIncrement(a_commands) ?? throw new NullReferenceException("could not get insert command with auto increment");
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("execute insert command");
                        this.o_result = this.o_currentDatabase?.RunCommand<BsonDocument>(o_insertCommand);
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("insert command executed");
                        if ((this.o_result != null) && (!this.o_result.Contains("ok")) && ((Double)this.o_result["ok"] != 1.0d))
                        {
                            throw new Exception("Nosqlmdb command has no value for result key 'ok' or result key's value 'ok' is not '1.0'");
                        }

                        LogCommandResults(this.o_result);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        b_ignoreResult = true;
                    }
                }
                else
                { /* execute query */
                    try
                    {
                        /* iterate each command and execute it, getting bson document as result */
                        foreach (BsonDocument o_command in a_commands)
                        {
                            if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("execute command");
                            this.o_result = this.o_currentDatabase?.RunCommand<BsonDocument>(o_command);
                            if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("command executed");
                            if ((this.o_result != null) && (!this.o_result.Contains("ok")) && ((Double)this.o_result["ok"] != 1.0d))
                            {
                                throw new Exception("Nosqlmdb command has no value for result key 'ok' or result key's value 'ok' is not '1.0'");
                            }

                            LogCommandResults(this.o_result);
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        b_ignoreResult = true;
                    }
                }
            }
            catch (Exception o_exc)
            {
                /* if transaction flag is set, roll-back transaction */
                if (p_b_autoCommit)
                {
                    this.ManualRollback();
                }

                throw new AccessViolationException("Could not execute nosqlmdb command; " + o_exc);
            }

            /* check if query got a result */
            if ((this.o_result == null) && (!b_ignoreResult))
            {
                /* if transaction flag is set, roll-back transaction */
                if (p_b_autoCommit)
                {
                    this.ManualRollback();
                }

                throw new AccessViolationException("The query could not be executed or has no valid result.");
            }

            /* on SELECT query, prepare to return result rows */
            if (p_o_sqlQuery.SqlType == SqlType.SELECT)
            {
                try
                {
                    if (this.o_result != null)
                    {
                        BsonArray a_result = this.o_result["cursor"].AsBsonDocument["firstBatch"].AsBsonArray;

                        if (a_result.Count > 0)
                        {
                            if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("get collection meta data");
                            BsonDocument o_collectionMetaData = GetCollectionMetaData(a_result[0].AsBsonDocument, p_o_sqlQuery.Table ?? "table of query is null");

                            /* fetch nosqlmdb result into hash map array */
                            foreach (BsonDocument o_document in a_result.Cast<BsonDocument>())
                            {
                                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("fetch row");
                                a_rows.Add(FetchRow(o_document, o_collectionMetaData));
                            }
                        }
                    }
                }
                catch (Exception o_exc)
                {
                    throw new AccessViolationException("Could not fetch row, issue get metadata and column information; " + o_exc);
                }
            }
            else
            {
                try
                {
                    /* returning amount of affected rows by nosqlmdb commands (and last inserted id if insert statement) */
                    Dictionary<string, Object?> o_row = [];
                    if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("get affected rows");
                    double d_ok = Convert.ToDouble(this.o_result?["ok"]);
                    o_row.Add("AffectedRows", (int)d_ok);
                    ForestNET.Lib.Global.ILogFinest("affected rows: '" + o_row["AffectedRows"] + "'");

                    /* add last inserted id for result */
                    if (p_o_sqlQuery.SqlType == SqlType.INSERT)
                    {
                        ForestNET.Lib.Global.ILogFinest("store LastInsertId '" + this.i_lastInsertId + "' in result row");
                        o_row.Add("LastInsertId", this.i_lastInsertId);
                    }

                    /* overwrite affected rows value for UPDATE or DELETE queries with 'n' result key value */
                    if ((this.o_result != null) && ((p_o_sqlQuery.SqlType == SqlType.UPDATE) || (p_o_sqlQuery.SqlType == SqlType.DELETE)))
                    {
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("overwrite affected rows value for UPDATE or DELETE queries");
                        if (this.o_result.Contains("n"))
                        {
                            o_row["AffectedRows"] = Convert.ToInt32(this.o_result["n"]);
                            ForestNET.Lib.Global.ILogFinest("affected rows: '" + o_row["AffectedRows"] + "'");
                        }
                    }

                    if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("add row object to result list");
                    a_rows.Add(o_row);
                }
                catch (Exception o_exc)
                {
                    /* do not throw exception here, maybe only log this information */
                    ForestNET.Lib.Global.ILogSevere("Could not count affected rows or could not fetch last insert id; " + o_exc);
                }
            }

            /* if transaction flag is set, commit transaction */
            if (p_b_autoCommit)
            {
                this.ManualCommit();
            }

            this.o_result = null;
            this.i_lastInsertId = -1;

            return a_rows;
        }

        /// <summary>
        /// Method to get meta data information for collection columns like column type
        /// </summary>
        /// <param name="p_o_resultFirstDocument">first bson document of result for column key set</param>
        /// <param name="p_s_collection">collection name</param>
        /// <returns>result bson document with type information of columns in result set</returns>
        /// <exception cref="Exception">if type information could not be queried or has not been found</exception>
        private BsonDocument GetCollectionMetaData(BsonDocument p_o_resultFirstDocument, string p_s_collection)
        {
            BsonDocument? o_fieldTypes = null;
            BsonDocument? o_joinColumns = null;

            foreach (BsonElement o_entry in p_o_resultFirstDocument.Elements)
            {
                /* column name */
                string s_column = o_entry.Name;

                /* remove aggregation from name, which automatically was set as prefix in transpose class */
                foreach (string s_aggregation in new List<string>() { "AVG_", "COUNT_", "MAX_", "MIN_", "SUM_" })
                {
                    if (s_column.StartsWith(s_aggregation))
                    {
                        s_column = s_column.Substring(s_aggregation.Length);
                    }
                }

                /* skip '_id' column */
                if (s_column.Equals("_id"))
                {
                    continue;
                }

                /* handle columns which are retrieved from the join collection */
                if (s_column.StartsWith("join_"))
                {
                    o_joinColumns = GetCollectionMetaData(o_entry.Value.AsBsonDocument, s_column.Substring(5));
                    continue;
                }

                if (o_fieldTypes == null)
                {
                    o_fieldTypes = new BsonDocument(s_column + "Type", new BsonDocument("$type", "$" + s_column));
                }
                else
                {
                    o_fieldTypes.Add(s_column + "Type", new BsonDocument("$type", "$" + s_column));
                }
            }

            if (o_fieldTypes == null)
            {
                throw new Exception("Could not list columns for field types query on nosqlmdb library");
            }

            BsonDocument o_command = new BsonDocument()
            .Add("aggregate", p_s_collection)
            .Add("pipeline", new BsonArray() {
                new BsonDocument("$project", o_fieldTypes),
                new BsonDocument("$limit", 1),
                new BsonDocument("$skip", 0)
            })
            .Add("cursor", new BsonDocument()); /* needed for aggregate */

            if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass(o_command.ToJson());
            if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("execute command");

            BsonDocument? o_metadataResult = this.o_currentDatabase?.RunCommand<BsonDocument>(o_command);

            if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("command executed");

            if ((o_metadataResult == null) || ((!o_metadataResult.Contains("ok")) && (o_metadataResult["ok"].AsDouble != 1.0d)))
            {
                throw new Exception("Nosqlmdb command has no value for result key 'ok' or result key's value 'ok' is not '1.0'");
            }

            LogCommandResults(o_metadataResult);

            BsonArray a_result = o_metadataResult["cursor"].AsBsonDocument["firstBatch"].AsBsonArray;

            if (a_result.Count < 1)
            {
                throw new Exception("Could not get result for listing columns for field types query on nosqlmdb library");
            }

            /* add meta data information about the join columns as well */
            if (o_joinColumns != null)
            {
                foreach (BsonElement o_entry in o_joinColumns.Elements)
                {
                    /* skip '_id' column */
                    if (o_entry.Name.Equals("_id"))
                    {
                        continue;
                    }

                    a_result[0].AsBsonDocument.Add(o_entry.Name, o_entry.Value);
                }
            }

            return a_result[0].AsBsonDocument;
        }

        /// <summary>
        /// Method to create a record dictionary of current result set record
        /// </summary>
        /// <returns>record dictionary, key(string) -> column name + value(object) -> column value</returns>
        /// <exception cref="MySqlException">exception accessing column type, column name or just column value of current result set record</exception>
        private static Dictionary<string, Object?> FetchRow(BsonDocument p_o_document, BsonDocument p_o_collectionMetaData)
        {
            Dictionary<string, Object?> o_row = [];

            BsonDocument? o_adjustedDocument = null;

            /* get all column values on same bson document level */
            foreach (BsonElement o_entry in p_o_document.Elements)
            {
                /* handle join fields */
                if (o_entry.Name.StartsWith("join_"))
                {
                    foreach (BsonElement o_joinEntry in o_entry.Value.AsBsonDocument.Elements)
                    {
                        if (o_adjustedDocument == null)
                        {
                            o_adjustedDocument = new BsonDocument(o_joinEntry.Name, o_joinEntry.Value);
                        }
                        else
                        {
                            o_adjustedDocument.Add(o_joinEntry.Name, o_joinEntry.Value);
                        }
                    }

                    /* skip key object pair, starting with key 'join_' */
                    continue;
                }

                if (o_adjustedDocument == null)
                {
                    o_adjustedDocument = new BsonDocument(o_entry.Name, o_entry.Value);
                }
                else
                {
                    o_adjustedDocument.Add(o_entry.Name, o_entry.Value);
                }
            }

            /* could not retrieve bson document */
            if (o_adjustedDocument == null)
            {
                /* return empty row */
                return o_row;
            }

            /* iterate adjusted document to create row object */
            foreach (BsonElement o_entry in o_adjustedDocument.Elements)
            {
                /* column name */
                string s_column = o_entry.Name;

                /* remove aggregation from name, which automatically was set as prefix in transpose class */
                foreach (string s_aggregation in new List<string>() { "AVG_", "COUNT_", "MAX_", "MIN_", "SUM_" })
                {
                    if (s_column.StartsWith(s_aggregation))
                    {
                        s_column = s_column.Substring(s_aggregation.Length);
                    }
                }

                /* skip '_id' column */
                if (s_column.Equals("_id"))
                {
                    continue;
                }

                string s_type = "";

                if (!p_o_collectionMetaData.Contains(s_column + "Type"))
                {
                    throw new Exception("Could not determine type for field '" + s_column + "'");
                }
                else
                {
                    s_type = p_o_collectionMetaData[s_column + "Type"].ToString() ?? "";
                    if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("column >>> " + s_column + "Type = " + s_type + ((ForestNET.Lib.Global.Instance.LogCompleteSqlQuery) ? " >>> Value as string: " + ((o_entry.Value != null) ? o_entry.Value.ToString() : "null") : ""));
                }

                /* check if we have a bson null value or string value NULL */
                if ((o_entry.Value != null) && ((o_entry.Value.IsBsonNull) || ((o_entry.Value.ToString() ?? "").ToUpper().Equals("NULL"))))
                {
                    s_type = "null";
                }

                /* re-check type int and long, because column can be mixed type */
                if ((s_type.Equals("int")) || (s_type.Equals("long")))
                {
                    if ((o_entry.Value != null) && (o_entry.Value.IsDouble))
                    {
                        s_type = "double";
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("column >>> " + s_column + "Type = " + s_type + ", because value type is Double");
                    }
                    else if ((o_entry.Value != null) && (o_entry.Value.IsDecimal128))
                    {
                        s_type = "decimal";
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("column >>> " + s_column + "Type = " + s_type + ", because value type is BigDecimal");
                    }
                }

                bool b_exc = false;

                switch (s_type)
                {
                    case "int":
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("get column value with AsInt32");
                        o_row.Add(s_column, o_entry.Value?.AsInt32 ?? 0);
                        break;
                    case "long":
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("get column value with AsInt64");
                        o_row.Add(s_column, o_entry.Value?.AsInt64 ?? 0);
                        break;
                    case "date":
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("get column value with AsBsonDateTime");
                        o_row.Add(s_column, o_entry.Value?.AsBsonDateTime.ToLocalTime() ?? null);
                        break;
                    case "timestamp":
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("get column value with AsBsonTimestamp");
                        o_row.Add(s_column, o_entry.Value?.AsBsonTimestamp.ToLocalTime() ?? null);
                        break;
                    case "double":
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("get column value with AsDouble");
                        o_row.Add(s_column, o_entry.Value?.AsDouble ?? 0.0d);
                        break;
                    case "decimal":
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("get column value with AsDecimal");
                        o_row.Add(s_column, o_entry.Value?.AsDecimal ?? 0.0m);
                        break;
                    case "bool":
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("get column value with AsBoolean");
                        o_row.Add(s_column, o_entry.Value?.AsBoolean ?? false);
                        break;
                    case "string":
                    case "regex":
                        if ((o_entry.Value != null) && (ForestNET.Lib.Helper.IsTime(o_entry.Value.AsString)))
                        {
                            if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("get column value with AsString and Helper.FromTimeString");
                            o_row.Add(s_column, ForestNET.Lib.Helper.FromTimeString((o_entry.Value.AsString)));
                        }
                        else
                        {
                            if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("get column value with AsString");
                            o_row.Add(s_column, o_entry.Value?.AsString ?? null);
                        }
                        break;
                    case "null":
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("column value is NULL");
                        o_row.Add(s_column, null);
                        break;
                    default:
                        b_exc = true;
                        break;
                }

                if (b_exc)
                {
                    throw new Exception("Invalid type '" + s_type + "' for column '" + s_column + "'");
                }
            }

            return o_row;
        }

        /// <summary>
        /// Transpose insert commands to one simple insert command where autoincrement column value has already been incremented by other queries before that
        /// </summary>
        /// <param name="p_a_insertCommands">list of bson document commands as insert commands</param>
        /// <returns>one simple insert command with incremented value</returns>
        /// <exception cref="Exception">could not retrieve value of autoincrement column or value is not a numeric integer value</exception>
        private BsonDocument? GetInsertCommandWithAutoIncrement(List<BsonDocument> p_a_insertCommands)
        {
            BsonDocument? o_return = null;
            string? s_autoIncrementColumn = null;

            /* iterate each command and execute it, getting bson document as result */
            foreach (BsonDocument o_command in p_a_insertCommands)
            {
                if (o_command.Contains("autoincrement_collection"))
                {
                    s_autoIncrementColumn = o_command["autoincrement_column"].ToString();

                    /* create select query with autoincrement column, limit 1 and order by desc; fast query if there is an index on autoincrement column */
                    Query<Select> o_querySelectMaxAutoIncrement = new(BaseGateway.NOSQLMDB, SqlType.SELECT, o_command["autoincrement_collection"].ToString() ?? "table not in nosqlmdb commands");
                    Column o_autoIcrementColumn = new(o_querySelectMaxAutoIncrement, s_autoIncrementColumn ?? "auto increment column not in nosqlmdb commands");
                    o_querySelectMaxAutoIncrement.GetQuery<Select>()?.Columns.Add(o_autoIcrementColumn);
                    (o_querySelectMaxAutoIncrement.GetQuery<Select>() ?? throw new NullReferenceException("select query is null")).OrderBy = new OrderBy(o_querySelectMaxAutoIncrement, [o_autoIcrementColumn], [false]);
                    (o_querySelectMaxAutoIncrement.GetQuery<Select>() ?? throw new NullReferenceException("select query is null")).Limit = new Limit(o_querySelectMaxAutoIncrement, 0, 1);

                    /* transpose sql query to nosqlmdb command */
                    List<BsonDocument>? a_autoIncrementCommands = BaseNoSQLMDBTranspose.Transpose(o_querySelectMaxAutoIncrement);

                    /* check if we get only one nosqlmdb command */
                    if ((a_autoIncrementCommands == null) || (a_autoIncrementCommands.Count != 1))
                    {
                        throw new Exception("Nosqlmdb command for autoincrement query is null or has multiple commands '" + a_autoIncrementCommands?.Count + "', not '1'");
                    }

                    /* log nosqlmdb commands */
                    foreach (BsonDocument o_commandTemp in a_autoIncrementCommands)
                    {
                        ForestNET.Lib.Global.ILogFiner(o_commandTemp.ToJson());
                    }

                    if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("execute command");

                    /* execute nosqlmdb command */
                    BsonDocument? o_autoIncrementResult = this.o_currentDatabase?.RunCommand<BsonDocument>(a_autoIncrementCommands[0]);

                    if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("command executed");

                    /* check if nosqlmdb command returns a valid result */
                    if ((o_autoIncrementResult == null) || ((!o_autoIncrementResult.Contains("ok")) && (o_autoIncrementResult["ok"].AsDouble != 1.0d)))
                    {
                        throw new Exception("Nosqlmdb command result is null or has no value for result key 'ok' or result key's value 'ok' is not '1.0'");
                    }

                    LogCommandResults(o_autoIncrementResult["cursor"].AsBsonDocument);

                    BsonArray a_result = o_autoIncrementResult["cursor"].AsBsonDocument["firstBatch"].AsBsonArray;

                    if (a_result.Count < 1)
                    {
                        this.i_lastInsertId = 0;
                    }
                    else
                    {
                        string s_maybeAnInteger = a_result[0].AsBsonDocument[s_autoIncrementColumn].ToString() ?? "cold not read as string";

                        if (ForestNET.Lib.Helper.IsInteger(s_maybeAnInteger))
                        {
                            this.i_lastInsertId = Convert.ToInt32(s_maybeAnInteger);
                        }
                        else
                        {
                            throw new Exception("Autoincrement column '" + s_autoIncrementColumn + "' does not contain numeric integer values. Value '" + s_maybeAnInteger + "' cannot be incremented");
                        }
                    }

                    this.i_lastInsertId++;
                }
                else
                {
                    if ((s_autoIncrementColumn != null) && (this.i_lastInsertId > 0))
                    {

                        BsonArray a_insertDocuments = o_command["documents"].AsBsonArray;

                        foreach (BsonDocument o_document in a_insertDocuments.Cast<BsonDocument>())
                        {
                            if ((o_document.Contains(s_autoIncrementColumn)) && (o_document[s_autoIncrementColumn].Equals("FORESTJ_REPLACE_AUTOINCREMENT_VALUE")))
                            {
                                o_document[s_autoIncrementColumn] = this.i_lastInsertId;
                            }
                        }

                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass(o_command.ToJson());
                    }

                    /* take command as insert command return value */
                    o_return = o_command;
                }
            }

            return o_return;
        }

        /// <summary>
        /// Manual commit current transaction
        /// </summary>
        /// <exception cref="NullReferenceException">current session is null</exception>
        /// <exception cref="AccessViolationException">could not commit in current database connection</exception>
        public override void ManualCommit()
        {
            if (this.o_currentSession == null)
            {
                throw new NullReferenceException("current session is null");
            }

            try
            {
                this.o_currentSession.CommitTransaction();

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
        /// <exception cref="NullReferenceException">current session is null</exception>
        /// <exception cref="AccessViolationException">could not rollback in current database connection</exception>
        public override void ManualRollback()
        {
            if (this.o_currentSession == null)
            {
                throw new NullReferenceException("current session is null");
            }

            try
            {
                this.o_currentSession.AbortTransaction();

                ForestNET.Lib.Global.ILogFinest("manual rollback executed");
            }
            catch (Exception o_exc)
            {
                throw new AccessViolationException("Could not rollback transaction; " + o_exc);
            }
        }

        /// <summary>
        /// Manual start current transaction
        /// </summary>
        /// <exception cref="NullReferenceException">current session is null</exception>
        /// <exception cref="AccessViolationException">could not start transaction</exception>
        public void ManualStartTransaction()
        {
            if (this.o_currentSession == null)
            {
                throw new NullReferenceException("current session is null");
            }

            try
            {
                this.o_currentSession.StartTransaction();

                ForestNET.Lib.Global.ILogFinest("manual started transaction");
            }
            catch (Exception o_exc)
            {
                throw new AccessViolationException("Could not start transaction; " + o_exc);
            }
        }
    }
}
