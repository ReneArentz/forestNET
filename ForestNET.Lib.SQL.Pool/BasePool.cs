namespace ForestNET.Lib.SQL.Pool
{
    /// <summary>
    /// Base pool class for creating a pool of sql connections to a database and keeping these alive. Well known structure to have several connections ready for a service application.<br />
    /// Using Base class for sql connections and fetching queries. see -> Base.<br />
    /// Requires ADO.NET or NoSQL MDB driver.<br />
    /// supported databases: see -> BaseGateway.
    /// </summary>
    public class BasePool
    {

        /* Fields */

        private bool b_running;
        private System.Threading.CancellationTokenSource? o_tokenSource;
        private int i_maxAmount;
        private ForestNET.Lib.DateInterval o_timeoutInterval = new("PT10S");
        private int i_timeoutMilliseconds;
        private List<BaseThread> a_baseThreads = [];
        private readonly string s_host;
        private readonly string s_datasource;
        private readonly string s_user;
        private readonly string s_password;

        /* Properties */

        public BaseGateway BaseGateway { get; private set; }

        public int MaxAmount
        {
            get
            {
                return this.i_maxAmount;
            }
            set
            {
                /* check if base pool is already running */
                if (this.b_running)
                {
                    throw new SystemException("Base pool is running. Please stop base pool first");
                }

                /* check if max amount value will be at least '1' */
                if (value < 1)
                {
                    throw new ArgumentException("Executor service pool max. amount must be at least '1', but was set to '" + value + "'");
                }

                this.i_maxAmount = value;

                this.a_baseThreads?.Clear();

                /* create new array of base threads, because max amount has been changed now */
                this.a_baseThreads = [];
            }
        }

        public ForestNET.Lib.DateInterval TimeoutInterval
        {
            get
            {
                return this.o_timeoutInterval ?? throw new NullReferenceException("TimeoutInterval is null");
            }
            set
            {
                /* check if base pool is already running */
                if (this.b_running)
                {
                    throw new SystemException("Base pool is running. Please stop base pool first");
                }

                this.o_timeoutInterval = value ?? throw new NullReferenceException("timeout interval parameter is null");
            }
        }

        public int TimeoutMilliseconds
        {
            get
            {
                return this.i_maxAmount;
            }
            set
            {
                /* check if base pool is already running */
                if (this.b_running)
                {
                    throw new SystemException("Base pool is running. Please stop base pool first");
                }

                /* check if timeout in milliseconds value will be at least '1' */
                if (value < 1)
                {
                    throw new ArgumentException("Timeout in milliseconds must be at least '1', but was set to '" + value + "'");
                }

                this.i_timeoutMilliseconds = value;
            }
        }

        /* Methods */

        /// <summary>
        /// Constructor of base pool class, settings all parameters for sql connection to database + settings: timeout interval for keep-alive and timeout(ms) for base thread in general.
        /// amount of base threads will be '1'.
        /// timeout milliseconds will be '1000'.
        /// timeout interval for base threads will be '60 sec'.
        /// </summary>
        /// <param name="p_e_baseGateway">enumeration value of BaseGateway class, specifies which database interface will be used with JDBC</param>
        /// <param name="p_s_host">address of database server, optional port specification e.g. 'localhost:3306'</param>
        /// <param name="p_s_datasource">parameter for selecting a database within target database server</param>
        /// <param name="p_s_user">user name for database login</param>
        /// <param name="p_s_password">user's password for database login</param>
        /// <exception cref="SystemException">base pool is already running</exception>
        /// <exception cref="ArgumentException">invalid parameter for max amount, timeout interval or timeout milliseconds parameter</exception>
        public BasePool(BaseGateway p_e_baseGateway, string p_s_host, string p_s_datasource, string p_s_user, string p_s_password)
            : this(1, new ForestNET.Lib.DateInterval("PT60S"), 1000, p_e_baseGateway, p_s_host, p_s_datasource, p_s_user, p_s_password)
        {

        }

        /// <summary>
        /// Constructor of base pool class, settings all parameters for sql connection to database + settings: timeout interval for keep-alive and timeout(ms) for base thread in general.
        /// amount of base threads will be '1'.
        /// timeout milliseconds will be '1000'.
        /// </summary>
        /// <param name="p_o_timeoutInterval">timeout interval for thread idle - if thread is idle for this interval, database connection will be tested and renewed if necessary</param>
        /// <param name="p_e_baseGateway">enumeration value of BaseGateway class, specifies which database interface will be used with JDBC</param>
        /// <param name="p_s_host">address of database server, optional port specification e.g. 'localhost:3306'</param>
        /// <param name="p_s_datasource">parameter for selecting a database within target database server</param>
        /// <param name="p_s_user">user name for database login</param>
        /// <param name="p_s_password">user's password for database login</param>
        /// <exception cref="SystemException">base pool is already running</exception>
        /// <exception cref="ArgumentException">invalid parameter for max amount, timeout interval or timeout milliseconds parameter</exception>
        public BasePool(ForestNET.Lib.DateInterval p_o_timeoutInterval, BaseGateway p_e_baseGateway, string p_s_host, string p_s_datasource, string p_s_user, string p_s_password)
            : this(1, p_o_timeoutInterval, 1000, p_e_baseGateway, p_s_host, p_s_datasource, p_s_user, p_s_password)
        {

        }

        /// <summary>
        /// Constructor of base pool class, settings all parameters for sql connection to database + settings: timeout interval for keep-alive and timeout(ms) for base thread in general.
        /// amount of base threads will be '1'.
        /// </summary>
        /// <param name="p_o_timeoutInterval">timeout interval for thread idle - if thread is idle for this interval, database connection will be tested and renewed if necessary</param>
        /// <param name="p_i_timeoutMilliseconds">timeout in milliseconds for base thread waiting in idle</param>
        /// <param name="p_e_baseGateway">enumeration value of BaseGateway class, specifies which database interface will be used with JDBC</param>
        /// <param name="p_s_host">address of database server, optional port specification e.g. 'localhost:3306'</param>
        /// <param name="p_s_datasource">parameter for selecting a database within target database server</param>
        /// <param name="p_s_user">user name for database login</param>
        /// <param name="p_s_password">user's password for database login</param>
        /// <exception cref="SystemException">base pool is already running</exception>
        /// <exception cref="ArgumentException">invalid parameter for max amount, timeout interval or timeout milliseconds parameter</exception>
        public BasePool(ForestNET.Lib.DateInterval p_o_timeoutInterval, int p_i_timeoutMilliseconds, BaseGateway p_e_baseGateway, string p_s_host, string p_s_datasource, string p_s_user, string p_s_password)
            : this(1, p_o_timeoutInterval, p_i_timeoutMilliseconds, p_e_baseGateway, p_s_host, p_s_datasource, p_s_user, p_s_password)
        {

        }

        /// <summary>
        /// Constructor of base pool class, settings all parameters for sql connection to database + settings: amount of base threads, timeout interval for keep-alive and timeout(ms) for base thread in general.
        /// </summary>
        /// <param name="p_i_maxAmount">integer value for fixed amount of threads for base thread pool instance</param>
        /// <param name="p_o_timeoutInterval">timeout interval for thread idle - if thread is idle for this interval, database connection will be tested and renewed if necessary</param>
        /// <param name="p_i_timeoutMilliseconds">timeout in milliseconds for base thread waiting in idle</param>
        /// <param name="p_e_baseGateway">enumeration value of BaseGateway class, specifies which database interface will be used with JDBC</param>
        /// <param name="p_s_host">address of database server, optional port specification e.g. 'localhost:3306'</param>
        /// <param name="p_s_datasource">parameter for selecting a database within target database server</param>
        /// <param name="p_s_user">user name for database login</param>
        /// <param name="p_s_password">user's password for database login</param>
        /// <exception cref="SystemException">base pool is already running</exception>
        /// <exception cref="ArgumentException">invalid parameter for max amount, timeout interval or timeout milliseconds parameter</exception>
        public BasePool(int p_i_maxAmount, ForestNET.Lib.DateInterval p_o_timeoutInterval, int p_i_timeoutMilliseconds, BaseGateway p_e_baseGateway, string p_s_host, string p_s_datasource, string p_s_user, string p_s_password)
        {
            this.b_running = false;
            this.MaxAmount = p_i_maxAmount;
            this.TimeoutInterval = p_o_timeoutInterval;
            this.TimeoutMilliseconds = p_i_timeoutMilliseconds;
            this.BaseGateway = p_e_baseGateway;
            this.s_host = p_s_host;
            this.s_datasource = p_s_datasource;
            this.s_user = p_s_user;
            this.s_password = p_s_password;
        }

        /// <summary>
        /// Starting base pool, creating thread pool instance and all base threads which will connect to (no)sql database
        /// </summary>
        /// <exception cref="SystemException">base pool is already running</exception>
        /// <exception cref="ArgumentException">illegal database gateway or illegal host parameter</exception>
        /// <exception cref="AccessViolationException">error creating database server connection</exception>
        public async void Start()
        {
            ForestNET.Lib.Global.ILog("Starting base pool");

            /* check if base pool is already running */
            if (b_running)
            {
                throw new SystemException("Base pool is running. Please stop base pool first");
            }

            ForestNET.Lib.Global.ILogConfig("set running flag");

            /* set running flag */
            this.b_running = true;

            ForestNET.Lib.Global.ILogConfig("create cancellation token and it's source and create a fixed amount of threads: '" + this.i_maxAmount + "'");

            /* create cancellation token and it's source */
            this.o_tokenSource = new System.Threading.CancellationTokenSource();
            CancellationToken o_token = this.o_tokenSource.Token;

            for (int i = 0; i < this.i_maxAmount; i++)
            {
                this.a_baseThreads.Add(new BaseThread(this.BaseGateway, this.s_host, this.s_datasource, this.s_user, this.s_password, this.o_timeoutInterval, this.i_timeoutMilliseconds, o_token));

                ForestNET.Lib.Global.ILogConfig("added base thread object #" + (i + 1) + " to task pool for execution");

                /* add base thread object #i to task pool for execution */
                _ = Task.Factory.StartNew(() => this.a_baseThreads[i].Run(), o_token, TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);

                /* need to wait 10 ms after task has been created an started */
                await Task.Delay(10);
            }
        }

        /// <summary>
        /// Stopping base pool, stopping each base thread and shutting down base thread pool within 5 seconds
        /// </summary>
        /// <exception cref="SystemException">base pool is not running</exception>
        public void Stop()
        {
            ForestNET.Lib.Global.ILog("Stopping base pool");

            /* check if communication is not running */
            if (!b_running)
            {
                throw new SystemException("Base pool is not running. Please start base pool first");
            }

            ForestNET.Lib.Global.ILogConfig("unset running flag");

            /* unset running flag */
            this.b_running = false;

            ForestNET.Lib.Global.ILogConfig("stop each base thread");

            /* stop each base thread */
            for (int i = 0; i < this.i_maxAmount; i++)
            {
                this.a_baseThreads[i].StopThread();
            }

            ForestNET.Lib.Global.ILogConfig("shutdown base thread pool by using cancellation token");

            /* shutdown thread pool */
            try
            {
                this.o_tokenSource?.Cancel();
            }
            catch (Exception)
            {
                /* nothing to do */
            }

            ForestNET.Lib.Global.ILog("Base pool stopped");
        }

        /// <summary>
        /// Fetch a query or a amount of queries separated by QueryAbstract.s_querySeparator, with auto commit
        /// </summary>
        /// <param name="p_o_sqlQuery">query object of Query class</param>
        /// <returns>list of dictionaries, key(string) -> column name + value(object) -> column value of a database record</returns>
        /// <exception cref="AccessViolationException">exception accessing column type, column name or just column value of current result set record</exception>
        /// <exception cref="SystemException">base pool is not running</exception>
        public List<Dictionary<string, Object?>> FetchQuery(IQuery? p_o_sqlQuery)
        {
            return this.FetchQuery(p_o_sqlQuery, true);
        }

        /// <summary>
        /// Fetch a query or a amount of queries separated by QueryAbstract.s_querySeparator
        /// </summary>
        /// <param name="p_o_sqlQuery">query object of Query class</param>
        /// <param name="p_b_autoCommit">commit flag: true - commit database after each execution of query object automatically, false - do not commit automatically</param>
        /// <returns>list of dictionaries, key(string) -> column name + value(object) -> column value of a database record</returns>
        /// <exception cref="AccessViolationException">exception accessing column type, column name or just column value of current result set record</exception>
        /// <exception cref="SystemException">base pool is not running</exception>
        public List<Dictionary<string, Object?>> FetchQuery(IQuery? p_o_sqlQuery, bool p_b_autoCommit)
        {
            /* check if communication is not running */
            if (!b_running)
            {
                throw new SystemException("Base pool is not running. Please start base pool first");
            }

            /* prepare return value */
            List<Dictionary<string, Object?>> a_rows = [];
            int i_maxAttempts = 30;
            int i_attempts = 1;

            /* execute query with one base thread object in base pool */
            do
            {
                /* iterate each base thread to find one which is not locked */
                for (int i = 0; i < this.i_maxAmount; i++)
                {
                    /* check if base thread is not locked */
                    if (!this.a_baseThreads[i].IsLocked)
                    {
                        try
                        {
                            ForestNET.Lib.Global.ILogFiner("using base thread #" + (i + 1) + " for " + Environment.CurrentManagedThreadId);
                            /* execute sql query with base thread */
                            a_rows = this.a_baseThreads[i].FetchQuery(p_o_sqlQuery, p_b_autoCommit);
                            break;
                        }
                        catch (ArgumentNullException o_exc)
                        {
                            ForestNET.Lib.Global.ILogFiner("Base connection object is null on thread #" + (i + 1) + ": " + o_exc);
                        }
                        catch (System.AccessViolationException o_exc)
                        {
                            ForestNET.Lib.Global.ILogFiner("Query issue on thread #" + (i + 1) + ": " + o_exc);
                        }
                        catch (Exception o_exc)
                        {
                            ForestNET.Lib.Global.ILogFiner("Could not lock thread #" + (i + 1) + ": " + o_exc);
                        }
                    }
                }

                /* check if we have no result after iterating each base thread */
                if (a_rows.Count < 1)
                {
                    ForestNET.Lib.Global.ILogFinest("attempt #" + i_attempts + " failed for " + Environment.CurrentManagedThreadId);

                    /* wait 1000 milliseconds */
                    Thread.Sleep(1000);

                    if (i_attempts >= i_maxAttempts)
                    { /* all attempts failed, so could not execute query and retrieve a result */
                        ForestNET.Lib.Global.ILogWarning("Could not execute query and retrieve a result, all threads are busy or connection was lost");
                        break;
                    }
                }

                i_attempts++;
            } while (a_rows.Count < 1);

            /* return result */
            return a_rows;
        }

        /// <summary>
        /// Internal base thread class, implements runnable for thread execution, which hold a database connection object to fetch a sql query and return result rows
        /// </summary>
        private class BaseThread : ForestNET.Lib.Runnable
        {

            /* Fields */

            private readonly Object o_lock = new();
            private Base o_baseConnection;
            private long l_currentTimestamp = 0;
            private readonly BaseGateway e_baseGateway;
            private readonly string s_host;
            private readonly string s_datasource;
            private readonly string s_user;
            private readonly string s_password;
            private readonly ForestNET.Lib.DateInterval o_timeoutInterval;
            private readonly int i_timeoutMilliseconds;
            private readonly CancellationToken o_cancellationToken;

            /* Properties */

            public bool IsLocked
            {
                get; private set;
            }

            /* Methods */

            /// <summary>
            /// Base thread constructor, initiating database server connection
            /// </summary>
            /// <param name="p_e_baseGateway">enumeration value of BaseGateway class, specifies which database interface will be used with JDBC</param>
            /// <param name="p_s_host">address of database server, optional port specification e.g. 'localhost:3306'</param>
            /// <param name="p_s_datasource">parameter for selecting a database within target database server</param>
            /// <param name="p_s_user">user name for database login</param>
            /// <param name="p_s_password">user's password for database login</param>
            /// <param name="p_o_timeoutInterval">timeout interval for thread idle - if thread is idle for this interval, database connection will be tested and renewed if necessary</param>
            /// <param name="p_i_timeoutMilliseconds">timeout in milliseconds for base thread waiting in idle</param>
            /// <exception cref="ArgumentException">illegal database gateway or illegal host parameter</exception>
            /// <exception cref="AccessViolationException">error creating database server connection</exception>
            public BaseThread(BaseGateway p_e_baseGateway, string p_s_host, string p_s_datasource, string p_s_user, string p_s_password, ForestNET.Lib.DateInterval p_o_timeoutInterval, int p_i_timeoutMilliseconds, CancellationToken p_o_cancellationToken)
            {
                this.e_baseGateway = p_e_baseGateway;
                this.s_host = p_s_host;
                this.s_datasource = p_s_datasource;
                this.s_user = p_s_user;
                this.s_password = p_s_password;
                this.o_timeoutInterval = p_o_timeoutInterval;
                this.i_timeoutMilliseconds = p_i_timeoutMilliseconds;
                this.o_cancellationToken = p_o_cancellationToken;

                if (this.e_baseGateway == BaseGateway.MARIADB)
                {
                    this.o_baseConnection = new MariaDB.BaseMariaDB(this.s_host, this.s_datasource, this.s_user, this.s_password);
                }
                else if (this.e_baseGateway == BaseGateway.SQLITE)
                {
                    this.o_baseConnection = new SQLite.BaseSQLite(this.s_host);
                }
                else if (this.e_baseGateway == BaseGateway.MSSQL)
                {
                    this.o_baseConnection = new MSSQL.BaseMSSQL(this.s_host, this.s_datasource, this.s_user, this.s_password);
                }
                else if (this.e_baseGateway == BaseGateway.PGSQL)
                {
                    this.o_baseConnection = new PGSQL.BasePGSQL(this.s_host, this.s_datasource, this.s_user, this.s_password);
                }
                else if (this.e_baseGateway == BaseGateway.ORACLE)
                {
                    this.o_baseConnection = new Oracle.BaseOracle(this.s_host, this.s_datasource, this.s_user, this.s_password);
                }
                else if (this.e_baseGateway == BaseGateway.NOSQLMDB)
                {
                    this.o_baseConnection = new NOSQLMDB.BaseNOSQLMDB(this.s_host, this.s_datasource, this.s_user, this.s_password);
                }
                else
                {
                    throw new ArgumentException("Invalid BaseGateway '" + this.e_baseGateway + "'; BaseGateway is not implemented for BasePool");
                }
            }

            /// <summary>
            /// Base thread run method which will test connection with database connection object after defined interval
            /// </summary>
            public override void Run()
            {
                /* update timestamp for base thread */
                this.l_currentTimestamp = Environment.TickCount64;

                /* run routine in while loop while stop flag has not been set with stop-method */
                while (!this.Stop)
                {
                    try
                    {
                        /* wait timeout here */
                        Thread.Sleep(this.i_timeoutMilliseconds);
                    }
                    catch (Exception)
                    {
                        /* nothing to do, sleep just got interrupted */
                    }

                    /* check if timeout interval run out and base thread can be locked */
                    if (((Environment.TickCount64 - this.o_timeoutInterval.ToDuration()) >= this.l_currentTimestamp))
                    {
                        /* atomic lock */
                        bool b_atomicLock = false;

                        try
                        {
                            /* try lock base thread */
                            Monitor.TryEnter(this.o_lock, this.i_timeoutMilliseconds, ref b_atomicLock);

                            /* we locked base thread */
                            if (b_atomicLock)
                            {
                                this.IsLocked = true;

                                ForestNET.Lib.Global.ILogFiner("base thread timeout occurred. base thread locked itself. keep connection alive by testing connection of current connection object");

                                /* keep connection alive by testing connection of current connection object */
                                if (!this.o_baseConnection.TestConnection())
                                {
                                    /* closing database connection to clear connection object */
                                    this.o_baseConnection.CloseConnection();

                                    /* renew connection object */
                                    if (this.e_baseGateway == BaseGateway.MARIADB)
                                    {
                                        this.o_baseConnection = new MariaDB.BaseMariaDB(this.s_host, this.s_datasource, this.s_user, this.s_password);
                                    }
                                    else if (this.e_baseGateway == BaseGateway.SQLITE)
                                    {
                                        this.o_baseConnection = new SQLite.BaseSQLite(this.s_host);
                                    }
                                    else if (this.e_baseGateway == BaseGateway.MSSQL)
                                    {
                                        this.o_baseConnection = new MSSQL.BaseMSSQL(this.s_host, this.s_datasource, this.s_user, this.s_password);
                                    }
                                    else if (this.e_baseGateway == BaseGateway.PGSQL)
                                    {
                                        this.o_baseConnection = new PGSQL.BasePGSQL(this.s_host, this.s_datasource, this.s_user, this.s_password);
                                    }
                                    else if (this.e_baseGateway == BaseGateway.ORACLE)
                                    {
                                        this.o_baseConnection = new Oracle.BaseOracle(this.s_host, this.s_datasource, this.s_user, this.s_password);
                                    }
                                    else if (this.e_baseGateway == BaseGateway.NOSQLMDB)
                                    {
                                        this.o_baseConnection = new NOSQLMDB.BaseNOSQLMDB(this.s_host, this.s_datasource, this.s_user, this.s_password);
                                    }
                                    else
                                    {
                                        throw new ArgumentException("Invalid BaseGateway '" + this.e_baseGateway + "'; BaseGateway is not implemented for BasePool");
                                    }
                                }
                            }
                        }
                        catch (Exception o_exc)
                        {
                            ForestNET.Lib.Global.ILogSevere("exception within base thread timeout, testing connection: " + o_exc);
                            break;
                        }
                        finally
                        {
                            /* we locked base thread */
                            if (b_atomicLock)
                            {
                                /* update timestamp for base thread */
                                this.l_currentTimestamp = Environment.TickCount64;

                                ForestNET.Lib.Global.ILogFiner("base thread unlocked itself for TestConnection or Reconnect");

                                /* unlock base thread */
                                Monitor.Exit(this.o_lock);
                                this.IsLocked = false;
                                b_atomicLock = false;
                            }
                        }
                    }

                    /* check if base thread has been canceled */
                    if (this.o_cancellationToken.IsCancellationRequested)
                    {
                        this.StopThread();
                    }
                }

                /* base thread is coming to an end, closing database connection */
                this.o_baseConnection.CloseConnection();
            }

            /// <summary>
            /// Stopping base thread by setting stop flag
            /// </summary>
            public void StopThread()
            {
                this.Stop = true;
            }

            /// <summary>
            /// Fetch a query or a amount of queries separated by QueryAbstract.s_querySeparator
            /// </summary>
            /// <param name="p_o_sqlQuery">query object of Query class</param>
            /// <param name="p_b_autoCommit">commit flag: true - commit database after each execution of query object automatically, false - do not commit automatically</param>
            /// <returns>list of dictionaries, key(string) -> column name + value(object) -> column value of a database record</returns>
            /// <exception cref="NullReferenceException">base connection object is null</exception>
            /// <exception cref="AccessViolationException">exception accessing column type, column name or just column value of current result set record</exception>
            public List<Dictionary<string, Object?>> FetchQuery(IQuery? p_o_sqlQuery, bool p_b_autoCommit)
            {
                /* return value */
                List<Dictionary<string, Object?>> a_rows = [];

                /* check if base connection is not null */
                if (o_baseConnection == null)
                {
                    throw new NullReferenceException("base connection object is null");
                }

                /* atomic lock */
                bool b_atomicLock = false;

                try
                {
                    /* try lock base thread */
                    Monitor.TryEnter(this.o_lock, this.i_timeoutMilliseconds, ref b_atomicLock);

                    /* we locked base thread */
                    if (b_atomicLock)
                    {
                        this.IsLocked = true;

                        ForestNET.Lib.Global.ILogFiner("base thread locked by '" + Environment.CurrentManagedThreadId + "' for FetchQuery");

                        /* update timestamp for base thread */
                        this.l_currentTimestamp = Environment.TickCount64;

                        /* execute sql query with base connection object, using auto commit */
                        a_rows = this.o_baseConnection.FetchQuery(p_o_sqlQuery, p_b_autoCommit);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    /* we locked base thread */
                    if (b_atomicLock)
                    {
                        /* update timestamp for base thread */
                        this.l_currentTimestamp = Environment.TickCount64;

                        ForestNET.Lib.Global.ILogFiner("base thread unlocked by '" + Environment.CurrentManagedThreadId + "' for FetchQuery");

                        /* unlock base thread */
                        Monitor.Exit(this.o_lock);
                        this.IsLocked = false;
                        b_atomicLock = false;
                    }
                }

                return a_rows;
            }
        }
    }
}