using ForestNET.Lib.SQL;

namespace ForestNET.Tests.SQL
{
    public class BasePoolUnitTest
    {
        [Test]
        public void TestBasePool()
        {
            try
            {
                TestConfig.InitiateTestLogging();

                string s_currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'");
                string s_testDirectory = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "testBasePool" + ForestNET.Lib.IO.File.DIR;

                if (ForestNET.Lib.IO.File.FolderExists(s_testDirectory))
                {
                    ForestNET.Lib.IO.File.DeleteDirectory(s_testDirectory);
                }

                ForestNET.Lib.IO.File.CreateDirectory(s_testDirectory);
                Assert.That(
                    ForestNET.Lib.IO.File.FolderExists(s_testDirectory),
                    Is.True,
                    "directory[" + s_testDirectory + "] does not exist"
                );

                ForestNET.Lib.Global o_glob = ForestNET.Lib.Global.Instance;

                /* o_glob.LogCompleteSqlQuery(true); */

                foreach (KeyValuePair<string, int> o_entry in BaseCredentials.BaseGateways)
                {
                    try
                    {
                        o_glob.BaseGateway = Enum.Parse<BaseGateway>(o_entry.Key);
                        Console.WriteLine(DateTime.Now + " - " + o_glob.BaseGateway + " started");

                        /* create o_glob.Base connection and get credentials for base pool */
                        BaseCredentials o_baseCredentials = new(s_testDirectory);

                        /* prepare database for base pool test */
                        PrepareDB();

                        /* create base pool */
                        ForestNET.Lib.SQL.Pool.BasePool o_basePool = new(
                            (o_glob.BaseGateway == BaseGateway.SQLITE) ? 1 : 3,
                            new ForestNET.Lib.DateInterval("PT10S"),
                            1000,
                            o_baseCredentials.e_baseGatewayBaseThread,
                            o_baseCredentials.s_hostBaseThread,
                            o_baseCredentials.s_datasourceBaseThread,
                            o_baseCredentials.s_userBaseThread,
                            o_baseCredentials.s_passwordBaseThread
                        );

                        /* create result list */
                        List<Double> a_doubleList = [];

                        /* create class with runnable to use base pool */
                        ThreadUsingBasePool o_threadUsingBasePool = new(new List<Object>() { o_basePool, a_doubleList });
                        /* start base pool */
                        o_basePool.Start();

                        /* wait 11500 milliseconds so base pool will test connection */
                        Thread.Sleep(11500);

                        /* create and run tasks */
                        Parallel.For(0, 17, i =>
                        {
                            o_threadUsingBasePool.Run();
                        });

                        /* stop base pool */
                        o_basePool.Stop();

                        /* wait for base pool to be finished */
                        Thread.Sleep(1000);

                        /* recreate db connection */
                        o_baseCredentials = new(s_testDirectory);
                        /* clean up database */
                        CleanDB();

                        /* check result list */
                        double d_firstValue = a_doubleList[0];
                        /* we expect that each thread has calculated the same average result */
                        foreach (double d_value in a_doubleList)
                        {
                            Assert.That(d_value, Is.EqualTo(d_firstValue), "unexpected average value '" + d_value + "', expected '" + d_firstValue + "'");
                        }

                        Console.WriteLine(d_firstValue);
                        Console.WriteLine(DateTime.Now + " - " + o_glob.BaseGateway + " finished");
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        o_glob.Base?.CloseConnection();
                    }
                }

                ForestNET.Lib.IO.File.DeleteDirectory(s_testDirectory);
                Assert.That(
                    ForestNET.Lib.IO.File.FolderExists(s_testDirectory),
                    Is.False,
                    "directory[" + s_testDirectory + "] does exist"
                );
            }
            catch (Exception o_exc)
            {
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
        }

        private static void PrepareDB()
        {
            ForestNET.Lib.Global o_glob = ForestNET.Lib.Global.Instance;

            try
            {
                /* create table */
                Query<Create> o_queryCreate = new(o_glob.BaseGateway, SqlType.CREATE, "sys_forestnet_data");
                List<Dictionary<string, string>> a_columnsDefinition = [];

                Dictionary<string, string> o_properties = new()
                {
                    { "name", "Id" },
                    { "columnType", "integer [int]" },
                    { "constraints", "NOT NULL;PRIMARY KEY;AUTO_INCREMENT" }
                };
                a_columnsDefinition.Add(o_properties);

                /* we do not need Id column here, because object id _id is enough for this */
                if (o_glob.BaseGateway == BaseGateway.NOSQLMDB)
                {
                    a_columnsDefinition.Clear();
                }

                o_properties = new()
                {
                    { "name", "Value" },
                    { "columnType", "double" },
                    { "constraints", "NOT NULL" }
                };
                a_columnsDefinition.Add(o_properties);

                foreach (Dictionary<string, string> o_columnDefinition in a_columnsDefinition)
                {
                    ColumnStructure o_column = new(o_queryCreate)
                    {
                        Name = o_columnDefinition["name"],
                        AlterOperation = "ADD"
                    };
                    o_column.ColumnTypeAllocation(o_columnDefinition["columnType"]);

                    if (o_columnDefinition.TryGetValue("constraints", out string? s_value))
                    {
                        string[] a_constraints = s_value.Split(";");

                        for (int i = 0; i < a_constraints.Length; i++)
                        {
                            o_column.AddConstraint(o_queryCreate.ConstraintTypeAllocation(a_constraints[i]));

                            if ((a_constraints[i].CompareTo("DEFAULT") == 0) && (o_columnDefinition.TryGetValue("constraintDefaultValue", out string? s_bar)))
                            {
                                o_column.ConstraintDefaultValue = (Object)s_bar;
                            }
                        }
                    }

                    o_queryCreate.GetQuery<Create>()?.Columns.Add(o_column);
                }

                List<Dictionary<string, Object?>> a_result = o_glob.Base?.FetchQuery(o_queryCreate) ?? [];

                /* check table has been created */

                int i_expectedAffectedRows = 0;

                if (o_glob.BaseGateway == BaseGateway.NOSQLMDB)
                {
                    i_expectedAffectedRows = 1;
                }
                else if ((o_glob.BaseGateway == BaseGateway.MSSQL) || (o_glob.BaseGateway == BaseGateway.PGSQL) || (o_glob.BaseGateway == BaseGateway.ORACLE))
                {
                    i_expectedAffectedRows = -1;
                }

                Assert.That(
                    a_result, Has.Count.EqualTo(1),
                    "Result row amount of create query is not '1', it is '" + a_result.Count + "'"
                );

                KeyValuePair<string, Object?> o_resultEntry = a_result[0].First();

                Assert.That(
                    o_resultEntry.Key, Is.EqualTo("AffectedRows"),
                    "Result row key of create query is not 'AffectedRows', it is '" + o_resultEntry.Key + "'"
                );

                Assert.That(
                    Convert.ToInt32(o_resultEntry.Value), Is.EqualTo(i_expectedAffectedRows),
                    "Result row value of create query is not '" + i_expectedAffectedRows + "', it is '" + o_resultEntry.Value + "'"
                );

                /* fill table with data */

                int i_amount = 500_000;
                int i_amountRowValues = 10_000;

                if (o_glob.BaseGateway == BaseGateway.NOSQLMDB)
                { /* for NOSQLMDB we must use single statements, because batch execution is not supported now */
                    i_amount = 10_000;

                    /* deactivate auto commit and skip querying for last insert id */
                    (o_glob.Base ?? throw new NullReferenceException("Base is null")).SkipQueryLastInsertId = true;

                    for (int i = 0; i < i_amount; i++)
                    {
                        Query<Insert> o_queryInsert = new(o_glob.BaseGateway, SqlType.INSERT, "sys_forestnet_data");
                        o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "Value"), ForestNET.Lib.Helper.RandomDoubleRange(0.0, 100.0)));
                        o_glob.Base?.FetchQuery(o_queryInsert, false);
                    }
                }
                else if (o_glob.BaseGateway == BaseGateway.SQLITE)
                { /* SQLITE driver likes single statements as well, because batch execution is not supported now */
                    /* deactivate auto commit and skip querying for last insert id */
                    (o_glob.Base ?? throw new NullReferenceException("Base is null")).SkipQueryLastInsertId = true;

                    for (int i = 0; i < i_amount; i++)
                    {
                        Query<Insert> o_queryInsert = new(o_glob.BaseGateway, SqlType.INSERT, "sys_forestnet_data");
                        o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "Value"), ForestNET.Lib.Helper.RandomDoubleRange(0.0, 100.0)));
                        o_glob.Base?.FetchQuery(o_queryInsert, false);
                    }

                    o_glob.Base?.ManualCommit();
                }
                else
                { /* create insert batch statement for 10k values each statement */
                    Query<Insert> o_queryInsert = new(o_glob.BaseGateway, SqlType.INSERT, "sys_forestnet_data");
                    o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "Value"), ForestNET.Lib.Helper.RandomDoubleRange(0.0, 100.0)));

                    string s_query = o_queryInsert.ToString();
                    s_query = s_query.Substring(0, s_query.IndexOf("VALUES ") + 7);

                    if (o_glob.BaseGateway == BaseGateway.ORACLE)
                    { /* oracle must be different */
                        s_query = "INSERT ALL ";
                    }

                    System.Text.StringBuilder o_stringBuilder = new();
                    o_stringBuilder.Append(s_query);

                    if (o_glob.BaseGateway == BaseGateway.MSSQL)
                    { /* mssql does not like more than 999 values in one insert batch statement */
                        i_amountRowValues = 999;
                    }
                    else if (o_glob.BaseGateway == BaseGateway.ORACLE)
                    { /* oracle is really slow, so we just insert 1k values in one insert batch statement, and just doing 100k */
                        i_amount = 100_000;
                        i_amountRowValues = 1_000;
                    }

                    /* deactivate auto commit and skip querying for last insert id */
                    (o_glob.Base ?? throw new NullReferenceException("Base is null")).SkipQueryLastInsertId = true;

                    for (int i = 0; i < i_amount; i++)
                    {
                        if (o_glob.BaseGateway == BaseGateway.ORACLE)
                        { /* oracle must be different */
                            o_stringBuilder.Append("INTO \"" + o_queryInsert.Table + "\" VALUES (" + (i + 1) + ", " + ForestNET.Lib.Helper.RandomDoubleRange(0.0, 100.0).ToString().Replace(",", ".") + ") ");
                        }
                        else
                        {
                            o_stringBuilder.Append(("(" + ForestNET.Lib.Helper.RandomDoubleRange(0.0, 100.0) + ")").Replace(",", ".") + ",");
                        }

                        /* execute query after every i_amountRowValues'th value and the last one */
                        if (((i % i_amountRowValues == 0) || (i == (i_amount - 1))) && (i != 0))
                        {
                            string s_foo = o_stringBuilder.ToString();

                            if (o_glob.BaseGateway == BaseGateway.ORACLE)
                            { /* oracle must be different */
                                o_queryInsert.SetQuery(s_foo + "SELECT * FROM dual");
                            }
                            else
                            {
                                o_queryInsert.SetQuery(s_foo.Substring(0, s_foo.Length - 1) + ";");
                            }

                            o_glob.Base.FetchQuery(o_queryInsert, false);

                            o_stringBuilder = new();
                            o_stringBuilder.Append(s_query);
                        }
                    }

                    /* commit insert batch statement */
                    o_glob.Base.ManualCommit();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                o_glob.Base?.CloseConnection();
            }
        }

        private static void CleanDB()
        {
            ForestNET.Lib.Global o_glob = ForestNET.Lib.Global.Instance;

            try
            {
                /* drop table */
                Query<Drop> o_queryDrop = new(o_glob.BaseGateway, SqlType.DROP, "sys_forestnet_data");

                List<Dictionary<string, Object?>> a_result = o_glob.Base?.FetchQuery(o_queryDrop) ?? [];

                /* check that table has been dropped */

                int i_expectedAffectedRows = 0;

                /* nosqlmdb has expected value 1 */
                if (o_glob.BaseGateway == BaseGateway.NOSQLMDB)
                {
                    i_expectedAffectedRows = 1;
                }
                else if ((o_glob.BaseGateway == BaseGateway.MSSQL) || (o_glob.BaseGateway == BaseGateway.PGSQL) || (o_glob.BaseGateway == BaseGateway.ORACLE))
                {
                    i_expectedAffectedRows = -1;
                }

                Assert.That(
                    a_result, Has.Count.EqualTo(1),
                    "Result row amount of drop query #2 is not '1', it is '" + a_result.Count + "'"
                );

                KeyValuePair<string, Object?> o_resultEntry = a_result[0].First();

                Assert.That(
                    o_resultEntry.Key, Is.EqualTo("AffectedRows"),
                    "Result row key of drop query #2 is not 'AffectedRows', it is '" + o_resultEntry.Key + "'"
                );

                Assert.That(
                    Convert.ToInt32(o_resultEntry.Value), Is.EqualTo(i_expectedAffectedRows),
                    "Result row value of drop query #2 is not '" + i_expectedAffectedRows + "', it is '" + o_resultEntry.Value + "'"
                );
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                o_glob.Base?.CloseConnection();
            }
        }

        private class ThreadUsingBasePool : ForestNET.Lib.Runnable
        {

            private readonly List<Object> a_param = [];

            private ForestNET.Lib.SQL.Pool.BasePool BasePool()
            {
                return (ForestNET.Lib.SQL.Pool.BasePool)this.a_param[0];
            }

            private List<Double> DoubleList()
            {
                return (List<Double>)this.a_param[1];
            }

            public ThreadUsingBasePool(List<Object> p_a_param)
            {
                if (p_a_param.Count != 2)
                {
                    throw new ArgumentException("Parameter object list must have '2' elements");
                }

                if (p_a_param[0] is not ForestNET.Lib.SQL.Pool.BasePool)
                {
                    throw new ArgumentException("Parameter object list with object #1 must be a 'BasePool' object, but is '" + p_a_param[0].GetType().FullName + "'");
                }

                if (p_a_param[1] is not List<Double>)
                {
                    throw new ArgumentException("Parameter object list with object #1 must be a 'List' object, but is '" + p_a_param[1].GetType().FullName + "'");
                }

                this.a_param = p_a_param;
            }

            public override void Run()
            {
                try
                {
                    /* select all data from table */
                    Query<Select> o_querySelect = new(this.BasePool().BaseGateway, SqlType.SELECT, "sys_forestnet_data");
                    o_querySelect.GetQuery<Select>()?.Columns.Add(new Column(o_querySelect, "*"));

                    List<Dictionary<string, Object?>> a_rows = this.BasePool().FetchQuery(o_querySelect);

                    if (a_rows != null)
                    {
                        double d_sum = 0.0d;

                        /* iterate each row and sum up Value column */
                        foreach (Dictionary<string, Object?> o_row in a_rows)
                        {
                            d_sum += Convert.ToDouble(o_row["Value"]?.ToString());
                        }

                        /* get average value and add it to result list */
                        this.DoubleList().Add(d_sum / a_rows.Count);
                    }
                    else
                    {
                        ForestNET.Lib.Global.ILogWarning("Could not execute query and retrieve a result");
                    }
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.ILogSevere(o_exc.Message);
                }
            }
        }
    }
}
