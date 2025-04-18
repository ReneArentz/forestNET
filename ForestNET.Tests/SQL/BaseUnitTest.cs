using ForestNET.Lib.SQL;

namespace ForestNET.Tests.SQL
{
    public class BaseUnitTest
    {
        [Test]
        public void TestBase()
        {
            try
            {
                TestConfig.InitiateTestLogging();

                string s_currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'");
                string s_testDirectory = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "testBase" + ForestNET.Lib.IO.File.DIR;

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

                DateTime o_dateTime = new(2003, 12, 15, 8, 33, 3);
                DateTime o_date = new(2009, 6, 29);
                TimeSpan o_time = new(11, 1, 43);

                DateTime o_localDateTime = new(2010, 9, 2, 5, 55, 13);
                DateTime o_localDate = new(2018, 11, 16);
                TimeSpan o_localTime = new(17, 42, 23);

                foreach (KeyValuePair<string, int> o_entry in BaseCredentials.BaseGateways)
                {
                    int i = -1;

                    try
                    {
                        o_glob.BaseGateway = Enum.Parse<BaseGateway>(o_entry.Key);

                        /* create o_glob.Base connection and get credentials for base pool */
                        BaseCredentials o_baseCredentials = new(s_testDirectory);

                        if (o_glob.Base == null)
                        {
                            throw new NullReferenceException("global Base is null");
                        }

                        int i_end = 23;

                        if (o_glob.BaseGateway == BaseGateway.NOSQLMDB)
                        {
                            i_end = 28;
                        }

                        for (i = 1; i <= i_end; i++)
                        {
                            /* other order for nosqlmdb */
                            if (o_glob.BaseGateway == BaseGateway.NOSQLMDB)
                            {
                                if ((i == 3) || (i == 8))
                                {
                                    continue;
                                }

                                if (i == 12)
                                {
                                    i = 8;
                                }
                                else if (i == 13)
                                {
                                    i = 3;
                                }
                                else if (i >= 14)
                                {
                                    i -= 2;
                                }

                                if (i == 24)
                                {
                                    string s_resourcesDirectory = Environment.CurrentDirectory + ForestNET.Lib.IO.File.DIR + "Resources" + ForestNET.Lib.IO.File.DIR + "nosqlmdb" + ForestNET.Lib.IO.File.DIR;

                                    /* insert category records */
                                    ForestNET.Lib.IO.File o_file = new(s_resourcesDirectory + "categories.json");
                                    List<Dictionary<string, Object?>> a_documents = ForestNET.Lib.SQL.NOSQLMDB.BaseNOSQLMDB.JSONtoListOfBSONDocuments(o_file.FileContent);

                                    Query<Create> o_queryCreate = new(o_glob.BaseGateway, SqlType.CREATE, "sys_forestnet_categories");
                                    _ = o_glob.Base.FetchQuery(o_queryCreate, false);

                                    foreach (Dictionary<string, Object?> o_insertObject in a_documents)
                                    {
                                        Query<Insert> o_queryInsert = new(o_glob.BaseGateway, SqlType.INSERT, "sys_forestnet_categories");

                                        foreach (KeyValuePair<string, Object?> o_categoryEntry in o_insertObject)
                                        {
                                            o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, o_categoryEntry.Key), o_categoryEntry.Key.Equals("CategoryID") ? Convert.ToInt32(o_categoryEntry.Value) : o_categoryEntry.Value?.ToString()));
                                        }

                                        _ = o_glob.Base.FetchQuery(o_queryInsert, false);
                                    }

                                    /* insert product records */
                                    o_file = new(s_resourcesDirectory + "products.json");
                                    a_documents = ForestNET.Lib.SQL.NOSQLMDB.BaseNOSQLMDB.JSONtoListOfBSONDocuments(o_file.FileContent);

                                    o_queryCreate = new(o_glob.BaseGateway, SqlType.CREATE, "sys_forestnet_products");
                                    _ = o_glob.Base.FetchQuery(o_queryCreate, false);

                                    foreach (Dictionary<string, Object?> o_insertObject in a_documents)
                                    {
                                        Query<Insert> o_queryInsert = new(o_glob.BaseGateway, SqlType.INSERT, "sys_forestnet_products");

                                        foreach (KeyValuePair<string, Object?> o_categoryEntry in o_insertObject)
                                        {
                                            object? o_value = null;

                                            if (o_categoryEntry.Key.Equals("Price"))
                                            {
                                                o_value = Convert.ToDecimal(o_categoryEntry.Value);
                                            }
                                            else if ((o_categoryEntry.Key.Equals("ProductName")) || (o_categoryEntry.Key.Equals("Unit")))
                                            {
                                                o_value = o_categoryEntry.Value?.ToString();
                                            }
                                            else
                                            {
                                                o_value = Convert.ToInt32(o_categoryEntry.Value);
                                            }

                                            o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, o_categoryEntry.Key), o_value));
                                        }

                                        _ = o_glob.Base.FetchQuery(o_queryInsert, false);
                                    }
                                }
                            }

                            bool b_autoCommit = true;

                            /* standalone servers do not support transactions */
                            if (o_glob.BaseGateway == BaseGateway.NOSQLMDB)
                            {
                                b_autoCommit = false;
                            }

                            IQuery? o_query = QueryUnitTest.TestQueryGenerator(i);
                            List<Dictionary<string, Object?>> a_result = o_glob.Base.FetchQuery(o_query, b_autoCommit);

                            if (i <= 8)
                            {
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
                                    a_result,
                                    Has.Count.EqualTo(1),
                                    "Result row amount of query #" + i + " is not '1', it is '" + a_result.Count + "'"
                                );

                                Dictionary<string, Object?> o_resultEntry = a_result[0];

                                Assert.That(
                                    o_resultEntry.ContainsKey("AffectedRows"),
                                    Is.True,
                                    "Result row key of query #" + i + " does not contain 'AffectedRows' as key"
                                );

                                Assert.That(
                                    Convert.ToInt32(o_resultEntry["AffectedRows"]),
                                    Is.EqualTo(i_expectedAffectedRows),
                                    "Result row value of query #" + i + " is not '" + i_expectedAffectedRows + "', it is '" + Convert.ToInt32(o_resultEntry["AffectedRows"]) + "'"
                                );
                            }
                            else if ((i >= 9) && (i <= 11))
                            {
                                Assert.That(
                                    a_result,
                                    Has.Count.EqualTo(1),
                                    "Result row amount of query #" + i + " is not '1', it is '" + a_result.Count + "'"
                                );

                                foreach (Dictionary<string, Object?> o_row in a_result)
                                {
                                    int j = 0;

                                    foreach (KeyValuePair<string, Object?> o_column in o_row.ToList())
                                    {
                                        if (j == 0)
                                        {
                                            Assert.That(
                                                o_column.Key,
                                                Is.EqualTo("AffectedRows"),
                                                "Result row key of query #" + i + " does not contain 'AffectedRows' as key"
                                            );

                                            Assert.That(
                                                Convert.ToInt32(o_column.Value),
                                                Is.EqualTo(1),
                                                "Result row value of query #" + i + " 'AffectedRows' is not '1', it is '" + o_column.Value + "'"
                                            );
                                        }
                                        else
                                        {
                                            Assert.That(
                                                o_column.Key,
                                                Is.EqualTo("LastInsertId"),
                                                "Result row key of query #" + i + " does not contain 'LastInsertId' as key"
                                            );

                                            int i_lastInsertId = 1;

                                            if (i == 10)
                                            {
                                                i_lastInsertId = 2;
                                            }
                                            else if (i == 11)
                                            {
                                                i_lastInsertId = 3;
                                            }

                                            Assert.That(
                                                Convert.ToInt32(o_column.Value),
                                                Is.EqualTo(i_lastInsertId),
                                                "Result row value of query #" + i + " 'LastInsertId' is not '" + i_lastInsertId + "', it is '" + o_column.Value + "'"
                                            );
                                        }

                                        j++;
                                    }
                                }
                            }
                            else if (i == 12)
                            {
                                Assert.That(
                                    a_result,
                                    Has.Count.EqualTo(0),
                                    "Result row amount of query #" + i + " is not '0', it is '" + a_result.Count + "'"
                                );
                            }
                            else if (i == 13)
                            {
                                Assert.That(
                                    a_result,
                                    Has.Count.EqualTo(1),
                                    "Result row amount of query #" + i + " is not '1', it is '" + a_result.Count + "'"
                                );

                                Dictionary<string, Object?> o_resultEntry = a_result[0];

                                Assert.That(
                                    o_resultEntry.ContainsKey("AffectedRows"),
                                    Is.True,
                                    "Result row key of query #" + i + " does not contain 'AffectedRows' as key"
                                );

                                Assert.That(
                                    Convert.ToInt32(o_resultEntry["AffectedRows"]),
                                    Is.EqualTo(3),
                                    "Result row value of query #" + i + " is not '3', it is '" + Convert.ToInt32(o_resultEntry["AffectedRows"]) + "'"
                                );
                            }
                            else if ((i >= 14) && (i <= 15))
                            {
                                Assert.That(
                                    a_result,
                                    Has.Count.EqualTo(0),
                                    "Result row amount of query #" + i + " is not '0', it is '" + a_result.Count + "'"
                                );
                            }
                            else if (i == 16)
                            {
                                Assert.That(
                                    a_result,
                                    Has.Count.EqualTo(3),
                                    "Result row amount of query #" + i + " is not '3', it is '" + a_result.Count + "'"
                                );

                                /* very strict unit test for expected types and values */

                                int j = 0;

                                foreach (Dictionary<string, Object?> o_row in a_result)
                                {
                                    int k = 0;

                                    foreach (KeyValuePair<string, Object?> o_column in o_row.ToList())
                                    {
                                        Object? o_object = o_column.Value;
                                        int l = 0;

                                        if (k == l++)
                                        { /* Id */
                                            if (j == 0)
                                            {
                                                Assert.That(Convert.ToInt32(o_object), Is.EqualTo(1), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '1'");
                                            }
                                            else if (j == 1)
                                            {
                                                Assert.That(Convert.ToInt32(o_object), Is.EqualTo(2), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '2'");
                                            }
                                            else if (j == 2)
                                            {
                                                Assert.That(Convert.ToInt32(o_object), Is.EqualTo(3), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '3'");
                                            }
                                        }
                                        else if (k == l++)
                                        { /* UUID */
                                            if (j == 0)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("123e4567-e89b-42d3-a456-556642440000"), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '123e4567-e89b-42d3-a456-556642440000'");
                                            }
                                            else if (j == 1)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("223e4567-e89b-42d3-a456-556642440000"), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '223e4567-e89b-42d3-a456-556642440000'");
                                            }
                                            else if (j == 2)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("323e4567-e89b-42d3-a456-556642440000"), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '323e4567-e89b-42d3-a456-556642440000'");
                                            }
                                        }
                                        else if (k == l++)
                                        { /* ShortText */
                                            Assert.That(Convert.ToString(o_object), Is.EqualTo("Wert"), "object[" + o_object?.ToString() ?? "null" + "] is not equal to 'Wert'");
                                        }
                                        else if (k == l++)
                                        { /* Text */
                                            Assert.That(Convert.ToString(o_object), Is.EqualTo("Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua."), "object[" + o_object?.ToString() ?? "null" + "] is not equal to 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.'");
                                        }
                                        else if (k == l++)
                                        { /* Short */
                                            /* nosqlmdb does not support short or smallint, only int32 and long */
                                            if (o_glob.BaseGateway == BaseGateway.NOSQLMDB)
                                            {
                                                if (j == 0)
                                                {
                                                    Assert.That(Convert.ToInt32(o_object), Is.EqualTo(123), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '123'");
                                                }
                                                else if (j == 1)
                                                {
                                                    Assert.That(Convert.ToInt32(o_object), Is.EqualTo(223), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '223'");
                                                }
                                                else if (j == 2)
                                                {
                                                    Assert.That(Convert.ToInt32(o_object), Is.EqualTo(323), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '323'");
                                                }
                                            }
                                            else
                                            {
                                                if (j == 0)
                                                {
                                                    Assert.That(Convert.ToInt16(o_object), Is.EqualTo((short)123), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '123'");
                                                }
                                                else if (j == 1)
                                                {
                                                    Assert.That(Convert.ToInt16(o_object), Is.EqualTo((short)223), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '223'");
                                                }
                                                else if (j == 2)
                                                {
                                                    Assert.That(Convert.ToInt16(o_object), Is.EqualTo((short)323), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '323'");
                                                }
                                            }
                                        }
                                        else if (k == l++)
                                        { /* Int */
                                            Assert.That(Convert.ToInt32(o_object), Is.EqualTo(1337), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '1337'");
                                        }
                                        else if (k == l++)
                                        { /* Long */
                                            if (j == 0)
                                            {
                                                Assert.That(Convert.ToInt64(o_object), Is.EqualTo(1234567890123L), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '1234567890123'");
                                            }
                                            else if (j == 1)
                                            {
                                                Assert.That(Convert.ToInt64(o_object), Is.EqualTo(2234567890123L), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '2234567890123'");
                                            }
                                            else if (j == 2)
                                            {
                                                Assert.That(Convert.ToInt64(o_object), Is.EqualTo(3234567890123L), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '3234567890123'");
                                            }
                                        }
                                        else if (k == l++)
                                        { /* DateTime */
                                            Assert.That(Convert.ToDateTime(o_object), Is.EqualTo(o_dateTime), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '" + o_dateTime + "'");
                                        }
                                        else if (k == l++)
                                        { /* Date */
                                            Assert.That(Convert.ToDateTime(o_object), Is.EqualTo(o_date), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '" + o_date + "'");
                                        }
                                        else if (k == l++)
                                        { /* Time */
                                            Assert.That((TimeSpan)(o_object ?? TimeSpan.MinValue), Is.EqualTo(o_time), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '" + o_time + "'");
                                        }
                                        else if (k == l++)
                                        { /* LocalDateTime */
                                            Assert.That(Convert.ToDateTime(o_object), Is.EqualTo(o_localDateTime), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '" + o_localDateTime + "'");
                                        }
                                        else if (k == l++)
                                        { /* LocalDate */
                                            Assert.That(Convert.ToDateTime(o_object), Is.EqualTo(o_localDate), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '" + o_localDate + "'");
                                        }
                                        else if (k == l++)
                                        { /* LocalTime */
                                            Assert.That((TimeSpan)(o_object ?? TimeSpan.MinValue), Is.EqualTo(o_localTime), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '" + o_localTime + "'");
                                        }
                                        else if (k == l++)
                                        { /* DoubleCol */
                                            /* because we store float in a double column, oracle has it's troubles */
                                            if (o_glob.BaseGateway == BaseGateway.ORACLE)
                                            {
                                                /* round double to 4 fraction digits to even */
                                                Assert.That(Math.Round(Convert.ToDouble(o_object), 4, MidpointRounding.ToEven), Is.EqualTo(Math.Round(35.6700d, 4, MidpointRounding.ToEven)), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '" + 35.67f + "'");
                                            }
                                            else
                                            {
                                                Assert.That(Convert.ToDouble(o_object), Is.EqualTo(35.67f), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '" + 35.67f + "'");
                                            }
                                        }
                                        else if (k == l++)
                                        { /* Decimal */
                                            Assert.That(Convert.ToDecimal(o_object), Is.EqualTo(2.718281828m), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '" + 2.718281828m + "'");
                                        }
                                        else if (k == l++)
                                        { /* Bool */
                                            string s_objectValue = o_object?.ToString() ?? "null";

                                            /* convert '1' to 'true' for PGSQL */
                                            if (o_glob.BaseGateway == BaseGateway.PGSQL)
                                            {
                                                if (s_objectValue.Equals("1"))
                                                {
                                                    s_objectValue = "true";
                                                }
                                            }

                                            if (j == 0)
                                            {
                                                Assert.That(Convert.ToBoolean(o_object), Is.EqualTo(true), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '" + true + "'");
                                            }
                                            else if (j == 1)
                                            {
                                                Assert.That(Convert.ToBoolean(o_object), Is.EqualTo(false), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '" + false + "'");
                                            }
                                            else if (j == 2)
                                            {
                                                Assert.That(Convert.ToBoolean(o_object), Is.EqualTo(true), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '" + true + "'");
                                            }
                                        }
                                        else if (k == l++)
                                        { /* Text2Changed */
                                            /* because we renamed Text2Changed to Text2 in nosqlmdb it is at the end, JSON is unordered by RFC default */
                                            if (o_glob.BaseGateway == BaseGateway.NOSQLMDB)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("another short text"), "object[" + o_object?.ToString() ?? "null" + "] is not equal to 'another short text'");
                                            }
                                            else
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet."), "object[" + o_object?.ToString() ?? "null" + "] is not equal to 'At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.'");
                                            }
                                        }
                                        else if (k == l++)
                                        { /* ShortText2 */
                                            /* because we renamed Text2Changed to Text2 in nosqlmdb it is at the end, JSON is unordered by RFC default */
                                            if (o_glob.BaseGateway == BaseGateway.NOSQLMDB)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet."), "object[" + o_object?.ToString() ?? "null" + "] is not equal to 'At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.'");
                                            }
                                            else
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("another short text"), "object[" + o_object?.ToString() ?? "null" + "] is not equal to 'another short text'");
                                            }
                                        }
                                        else if (k == l++)
                                        { /* ShortText3 */
                                            /* only nosqlmdb has ShortText3 column */
                                            if (o_glob.BaseGateway == BaseGateway.NOSQLMDB)
                                            {
                                                Assert.That(o_object, Is.EqualTo(null), "object[" + o_object?.ToString() ?? "null" + "] is not equal to 'null'");
                                            }
                                        }
                                        else if (k == l++)
                                        { /* Text3 */
                                            /* only nosqlmdb has Text3 column */
                                            if (o_glob.BaseGateway == BaseGateway.NOSQLMDB)
                                            {
                                                Assert.That(o_object, Is.EqualTo(null), "object[" + o_object?.ToString() ?? "null" + "] is not equal to 'null'");
                                            }
                                        }

                                        k++;
                                    }

                                    j++;
                                }

                            }
                            else if ((i >= 17) && (i <= 23))
                            {
                                int i_expectedAffectedRows = 0;

                                if ((o_glob.BaseGateway == BaseGateway.SQLITE) && ((i >= 18) && (i <= 20)))
                                {
                                    i_expectedAffectedRows = 3;
                                }

                                if (((o_glob.BaseGateway == BaseGateway.MSSQL) || (o_glob.BaseGateway == BaseGateway.PGSQL) || (o_glob.BaseGateway == BaseGateway.ORACLE)) && ((i >= 18) && (i <= 23)))
                                {
                                    i_expectedAffectedRows = -1;
                                }

                                if ((o_glob.BaseGateway == BaseGateway.NOSQLMDB) && ((i >= 18) && (i <= 23)))
                                {
                                    i_expectedAffectedRows = 1;
                                }

                                Assert.That(
                                    a_result,
                                    Has.Count.EqualTo(1),
                                    "Result row amount of query #" + i + " is not '1', it is '" + a_result.Count + "'"
                                );

                                Dictionary<string, Object?> o_resultEntry = a_result[0];

                                Assert.That(
                                    o_resultEntry.ContainsKey("AffectedRows"),
                                    Is.True,
                                    "Result row key of query #" + i + " does not contain 'AffectedRows' as key"
                                );

                                Assert.That(
                                    Convert.ToInt32(o_resultEntry["AffectedRows"]),
                                    Is.EqualTo(i_expectedAffectedRows),
                                    "Result row value of query #" + i + " is not '" + i_expectedAffectedRows + "', it is '" + Convert.ToInt32(o_resultEntry["AffectedRows"]) + "'"
                                );
                            }
                            else if (i == 24)
                            {
                                Assert.That(
                                    a_result,
                                    Has.Count.EqualTo(16),
                                    "Result row amount of query #" + i + " is not '16', it is '" + a_result.Count + "'"
                                );

                                int[] a_rowNums = new int[] { 1, 5, 9, 12 };

                                foreach (int i_rowNum in a_rowNums)
                                {
                                    int i_rowPointer = 0;

                                    foreach (KeyValuePair<string, Object?> o_column in a_result[i_rowNum].ToList())
                                    {
                                        Object? o_object = o_column.Value;

                                        if (i_rowPointer == 0)
                                        { /* ProductID */
                                            if (i_rowNum == 1)
                                            {
                                                Assert.That(Convert.ToInt32(o_object), Is.EqualTo(64), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '64'");
                                            }
                                            else if (i_rowNum == 5)
                                            {
                                                Assert.That(Convert.ToInt32(o_object), Is.EqualTo(73), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '73");
                                            }
                                            else if (i_rowNum == 9)
                                            {
                                                Assert.That(Convert.ToInt32(o_object), Is.EqualTo(54), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '54'");
                                            }
                                            else if (i_rowNum == 12)
                                            {
                                                Assert.That(Convert.ToInt32(o_object), Is.EqualTo(57), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '57'");
                                            }
                                        }
                                        else if (i_rowPointer == 1)
                                        { /* ProductName */
                                            if (i_rowNum == 1)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("Wimmers gute Semmelknödel"), "object[" + o_object?.ToString() ?? "null" + "] is not equal to 'Wimmers gute Semmelknödel'");
                                            }
                                            else if (i_rowNum == 5)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("Röd Kaviar"), "object[" + o_object?.ToString() ?? "null" + "] is not equal to 'Röd Kaviar'");
                                            }
                                            else if (i_rowNum == 9)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("Tourtière"), "object[" + o_object?.ToString() ?? "null" + "] is not equal to 'Tourtière'");
                                            }
                                            else if (i_rowNum == 12)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("Ravioli Angelo"), "object[" + o_object?.ToString() ?? "null" + "] is not equal to 'Ravioli Angelo'");
                                            }
                                        }
                                        else if (i_rowPointer == 2)
                                        { /* SupplierID */
                                            if (i_rowNum == 1)
                                            {
                                                Assert.That(Convert.ToInt32(o_object), Is.EqualTo(12), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '12'");
                                            }
                                            else if (i_rowNum == 5)
                                            {
                                                Assert.That(Convert.ToInt32(o_object), Is.EqualTo(17), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '17'");
                                            }
                                            else if (i_rowNum == 9)
                                            {
                                                Assert.That(Convert.ToInt32(o_object), Is.EqualTo(25), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '25'");
                                            }
                                            else if (i_rowNum == 12)
                                            {
                                                Assert.That(Convert.ToInt32(o_object), Is.EqualTo(26), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '26'");
                                            }
                                        }
                                        else if (i_rowPointer == 3)
                                        { /* CategoryID */
                                            if (i_rowNum == 1)
                                            {
                                                Assert.That(Convert.ToInt32(o_object), Is.EqualTo(5), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '5'");
                                            }
                                            else if (i_rowNum == 5)
                                            {
                                                Assert.That(Convert.ToInt32(o_object), Is.EqualTo(8), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '8'");
                                            }
                                            else if (i_rowNum == 9)
                                            {
                                                Assert.That(Convert.ToInt32(o_object), Is.EqualTo(6), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '6'");
                                            }
                                            else if (i_rowNum == 12)
                                            {
                                                Assert.That(Convert.ToInt32(o_object), Is.EqualTo(5), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '5'");
                                            }
                                        }
                                        else if (i_rowPointer == 4)
                                        { /* Unit */
                                            if (i_rowNum == 1)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("20 bags x 4 pieces"), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '20 bags x 4 pieces'");
                                            }
                                            else if (i_rowNum == 5)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("24 - 150 g jars"), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '24 - 150 g jars'");
                                            }
                                            else if (i_rowNum == 9)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("16 pies"), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '16 pies'");
                                            }
                                            else if (i_rowNum == 12)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("24 - 250 g pkgs."), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '24 - 250 g pkgs.'");
                                            }
                                        }
                                        else if (i_rowPointer == 5)
                                        { /* Price */
                                            if (i_rowNum == 1)
                                            {
                                                Assert.That(Convert.ToDouble(o_object), Is.EqualTo(33.25d), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '" + 33.25d + "'");
                                            }
                                            else if (i_rowNum == 5)
                                            {
                                                Assert.That(Convert.ToInt32(o_object), Is.EqualTo(15), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '15'");
                                            }
                                            else if (i_rowNum == 9)
                                            {
                                                Assert.That(Convert.ToDouble(o_object), Is.EqualTo(7.45d), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '" + 7.45d + "'");
                                            }
                                            else if (i_rowNum == 12)
                                            {
                                                Assert.That(Convert.ToDouble(o_object), Is.EqualTo(19.5d), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '" + 19.5d + "'");
                                            }
                                        }
                                        else if (i_rowPointer == 5)
                                        { /* CategoryName */
                                            if (i_rowNum == 1)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("Grains/Cereals"), "object[" + o_object?.ToString() ?? "null" + "] is not equal to 'Grains/Cereals'");
                                            }
                                            else if (i_rowNum == 5)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("Seafood"), "object[" + o_object?.ToString() ?? "null" + "] is not equal to 'Seafood'");
                                            }
                                            else if (i_rowNum == 9)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("Meat/Poultry"), "object[" + o_object?.ToString() ?? "null" + "] is not equal to 'Meat/Poultry'");
                                            }
                                            else if (i_rowNum == 12)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("Grains/Cereals"), "object[" + o_object?.ToString() ?? "null" + "] is not equal to 'Grains/Cereals'");
                                            }
                                        }
                                        else if (i_rowPointer == 5)
                                        { /* Description */
                                            if (i_rowNum == 1)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("Breads, crackers, pasta, and cereal"), "object[" + o_object?.ToString() ?? "null" + "] is not equal to 'Breads, crackers, pasta, and cereal'");
                                            }
                                            else if (i_rowNum == 5)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("Seaweed and fish"), "object[" + o_object?.ToString() ?? "null" + "] is not equal to 'Seaweed and fish'");
                                            }
                                            else if (i_rowNum == 9)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("Prepared meats"), "object[" + o_object?.ToString() ?? "null" + "] is not equal to 'Prepared meats'");
                                            }
                                            else if (i_rowNum == 12)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("Breads, crackers, pasta, and cereal"), "object[" + o_object?.ToString() ?? "null" + "] is not equal to 'Breads, crackers, pasta, and cereal'");
                                            }
                                        }

                                        i_rowPointer++;
                                    }
                                }
                            }
                            else if (i == 25)
                            {
                                Assert.That(
                                    a_result,
                                    Has.Count.EqualTo(7),
                                    "Result row amount of query #" + i + " is not '7', it is '" + a_result.Count + "'"
                                );

                                int[] a_rowNums = new int[] { 0, 2, 5 };

                                foreach (int i_rowNum in a_rowNums)
                                {
                                    int i_rowPointer = 0;

                                    foreach (KeyValuePair<string, Object?> o_column in a_result[i_rowNum].ToList())
                                    {
                                        Object? o_object = o_column.Value;

                                        if (i_rowPointer == 0)
                                        { /* ProductName */
                                            if (i_rowNum == 0)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("Raclette Courdavault"), "object[" + o_object?.ToString() ?? "null" + "] is not equal to 'Raclette Courdavault'");
                                            }
                                            else if (i_rowNum == 2)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("Manjimup Dried Apples"), "object[" + o_object?.ToString() ?? "null" + "] is not equal to 'Manjimup Dried Apples'");
                                            }
                                            else if (i_rowNum == 5)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("Carnarvon Tigers"), "object[" + o_object?.ToString() ?? "null" + "] is not equal to 'Carnarvon Tigers'");
                                            }
                                        }
                                        else if (i_rowPointer == 1)
                                        { /* SupplierID */
                                            if (i_rowNum == 0)
                                            {
                                                Assert.That(Convert.ToInt32(o_object), Is.EqualTo(28), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '28'");
                                            }
                                            else if (i_rowNum == 2)
                                            {
                                                Assert.That(Convert.ToInt32(o_object), Is.EqualTo(24), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '24'");
                                            }
                                            else if (i_rowNum == 5)
                                            {
                                                Assert.That(Convert.ToInt32(o_object), Is.EqualTo(7), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '7'");
                                            }
                                        }
                                        else if (i_rowPointer == 2)
                                        { /* Unit */
                                            if (i_rowNum == 0)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("5 kg pkg."), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '5 kg pkg.'");
                                            }
                                            else if (i_rowNum == 2)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("50 - 300 g pkgs."), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '50 - 300 g pkgs.'");
                                            }
                                            else if (i_rowNum == 5)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("16 kg pkg."), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '16 kg pkg.'");
                                            }
                                        }
                                        else if (i_rowPointer == 3)
                                        { /* Price */
                                            if (i_rowNum == 0)
                                            {
                                                Assert.That(Convert.ToInt32(o_object), Is.EqualTo(55), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '55'");
                                            }
                                            else if (i_rowNum == 2)
                                            {
                                                Assert.That(Convert.ToInt32(o_object), Is.EqualTo(53), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '53'");
                                            }
                                            else if (i_rowNum == 5)
                                            {
                                                Assert.That(Convert.ToDouble(o_object), Is.EqualTo(62.5d), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '" + 62.5d + "'");
                                            }
                                        }
                                        else if (i_rowPointer == 4)
                                        { /* ProductID */
                                            if (i_rowNum == 0)
                                            {
                                                Assert.That(Convert.ToInt32(o_object), Is.EqualTo(2), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '2'");
                                            }
                                            else if (i_rowNum == 2)
                                            {
                                                Assert.That(Convert.ToInt32(o_object), Is.EqualTo(3), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '3'");
                                            }
                                            else if (i_rowNum == 5)
                                            {
                                                Assert.That(Convert.ToInt32(o_object), Is.EqualTo(5), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '5'");
                                            }
                                        }

                                        i_rowPointer++;
                                    }
                                }
                            }
                            else if (i == 26)
                            {
                                Assert.That(
                                    a_result,
                                    Has.Count.EqualTo(4),
                                    "Result row amount of query #" + i + " is not '4', it is '" + a_result.Count + "'"
                                );

                                int[] a_rowNums = new int[] { 1, 3 };

                                foreach (int i_rowNum in a_rowNums)
                                {
                                    int i_rowPointer = 0;

                                    foreach (KeyValuePair<string, Object?> o_column in a_result[i_rowNum].ToList())
                                    {
                                        Object? o_object = o_column.Value;

                                        if (i_rowPointer == 0)
                                        { /* ProductName */
                                            if (i_rowNum == 1)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("Queso Cabrales"), "object[" + o_object?.ToString() ?? "null" + "] is not equal to 'Queso Cabrales'");
                                            }
                                            else if (i_rowNum == 3)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("Sirop d'érable"), "object[" + o_object?.ToString() ?? "null" + "] is not equal to 'Sirop d'érable'");
                                            }
                                        }
                                        else if (i_rowPointer == 1)
                                        { /* SupplierID */
                                            if (i_rowNum == 1)
                                            {
                                                Assert.That(Convert.ToInt32(o_object), Is.EqualTo(5), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '5'");
                                            }
                                            else if (i_rowNum == 3)
                                            {
                                                Assert.That(Convert.ToInt32(o_object), Is.EqualTo(29), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '29'");
                                            }
                                        }
                                        else if (i_rowPointer == 2)
                                        { /* CategoryID */
                                            if (i_rowNum == 1)
                                            {
                                                Assert.That(Convert.ToInt32(o_object), Is.EqualTo(4), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '4'");
                                            }
                                            else if (i_rowNum == 3)
                                            {
                                                Assert.That(Convert.ToInt32(o_object), Is.EqualTo(2), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '2'");
                                            }
                                        }
                                        else if (i_rowPointer == 3)
                                        { /* Unit */
                                            if (i_rowNum == 1)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("1 kg pkg."), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '1 kg pkg.'");
                                            }
                                            else if (i_rowNum == 3)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("24 - 500 ml bottles"), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '24 - 500 ml bottles'");
                                            }
                                        }
                                        else if (i_rowPointer == 4)
                                        { /* CategoryName */
                                            if (i_rowNum == 1)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("Dairy Products"), "object[" + o_object?.ToString() ?? "null" + "] is not equal to 'Dairy Products'");
                                            }
                                            else if (i_rowNum == 3)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("Condiments"), "object[" + o_object?.ToString() ?? "null" + "] is not equal to 'Condiments'");
                                            }
                                        }
                                        else if (i_rowPointer == 5)
                                        { /* Description */
                                            if (i_rowNum == 1)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("Cheeses"), "object[" + o_object?.ToString() ?? "null" + "] is not equal to 'Cheeses'");
                                            }
                                            else if (i_rowNum == 3)
                                            {
                                                Assert.That(Convert.ToString(o_object), Is.EqualTo("Sweet and savory sauces, relishes, spreads, and seasonings"), "object[" + o_object?.ToString() ?? "null" + "] is not equal to 'Sweet and savory sauces, relishes, spreads, and seasonings'");
                                            }
                                        }
                                        else if (i_rowPointer == 6)
                                        { /* Price */
                                            if (i_rowNum == 1)
                                            {
                                                Assert.That(Convert.ToInt32(o_object), Is.EqualTo(21), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '21'");
                                            }
                                            else if (i_rowNum == 3)
                                            {
                                                Assert.That(Convert.ToDouble(o_object), Is.EqualTo(28.5d), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '" + 28.5d + "'");
                                            }
                                        }
                                        else if (i_rowPointer == 7)
                                        { /* ProductID */
                                            if (i_rowNum == 1)
                                            {
                                                Assert.That(Convert.ToInt32(o_object), Is.EqualTo(2), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '2'");
                                            }
                                            else if (i_rowNum == 3)
                                            {
                                                Assert.That(Convert.ToInt32(o_object), Is.EqualTo(2), "object[" + o_object?.ToString() ?? "null" + "] is not equal to '2'");
                                            }
                                        }

                                        i_rowPointer++;
                                    }
                                }
                            }

                            /* re-order for nosqlmdb for for loop increase */
                            if (o_glob.BaseGateway == BaseGateway.NOSQLMDB)
                            {
                                if (i == 8)
                                {
                                    i = 12;
                                }
                                else if (i == 3)
                                {
                                    i = 13;
                                }
                                else if (i >= 12)
                                {
                                    i += 2;
                                }

                                if (i == 28)
                                {
                                    /* delete sys_forestnet_categories and sys_forestnet_products tables */
                                    Query<Drop> o_queryDrop = new(o_glob.BaseGateway, SqlType.DROP, "sys_forestnet_categories");
                                    List<Dictionary<string, Object?>> a_resultDrop = o_glob.Base.FetchQuery(o_queryDrop, false);
                                    o_queryDrop = new(o_glob.BaseGateway, SqlType.DROP, "sys_forestnet_products");
                                    a_resultDrop = o_glob.Base.FetchQuery(o_queryDrop, false);
                                }
                            }
                        }
                    }
                    catch (Exception o_exc)
                    {
                        Assert.Fail("Exception Query #" + i + ": " + o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
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
    }
}
