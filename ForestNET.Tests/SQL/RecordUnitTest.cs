using ForestNET.Lib.SQL;

namespace ForestNET.Tests.SQL
{
    public class RecordUnitTest
    {
        [Test]
        public void TestBase()
        {
            try
            {
                TestConfig.InitiateTestLogging();

                string s_currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'");
                string s_testDirectory = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "testRecord" + ForestNET.Lib.IO.File.DIR;

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

                foreach (KeyValuePair<string, int> o_entry in BaseCredentials.BaseGateways)
                {
                    try
                    {
                        o_glob.BaseGateway = Enum.Parse<BaseGateway>(o_entry.Key);

                        /* create o_glob.Base connection and get credentials for base pool */
                        BaseCredentials o_baseCredentials = new(s_testDirectory);

                        if (o_glob.Base == null)
                        {
                            throw new NullReferenceException("global Base is null");
                        }

                        try
                        {
                            CleanupRecordTest(false);
                        }
                        catch (Exception)
                        {
                            /* does not matter */
                        }

                        PrepareRecordTest();
                        TestLanguageRecord();
                        TestDDLRecord();
                        CleanupRecordTest(true);
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

        private static void PrepareRecordTest()
        {
            ForestNET.Lib.Global o_glob = ForestNET.Lib.Global.Instance;

            /* #### CREATE ############################################################################################# */
            Query<Create> o_queryCreate = new(o_glob.BaseGateway, SqlType.CREATE, "sys_forestnet_language");
            /* #### Columns ############################################################################################ */
            List<Dictionary<string, string>> a_columnsDefinition = [];

            Dictionary<string, string> o_properties = new()
            {
                { "name", "Id" },
                { "columnType", "integer [int]" },
                { "constraints", "NOT NULL;PRIMARY KEY;AUTO_INCREMENT" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "UUID" },
                { "columnType", "text [36]" },
                { "constraints", "NOT NULL;UNIQUE" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "Code" },
                { "columnType", "text [36]" },
                { "constraints", "NOT NULL" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "Language" },
                { "columnType", "text [36]" },
                { "constraints", "NULL;DEFAULT" },
                { "constraintDefaultValue", "NULL" }
            };
            a_columnsDefinition.Add(o_properties);

            /* #### Query ############################################################################ */

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

            /* #### ALTER ########################################################################################### */
            Query<Alter> o_queryAlter = new(o_glob.BaseGateway, SqlType.ALTER, "sys_forestnet_language");
            /* #### Constraints ##################################################################################### */
            Constraint o_constraint = new(o_queryAlter, "UNIQUE", "sys_forestnet_language_unique", "", "ADD");
            o_constraint.Columns.Add("Code");
            o_constraint.Columns.Add("Language");

            o_queryAlter.GetQuery<Alter>()?.Constraints.Add(o_constraint);

            a_result = o_glob.Base?.FetchQuery(o_queryAlter) ?? [];

            i_expectedAffectedRows = 0;

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

            o_resultEntry = a_result[0].First();

            Assert.That(
                o_resultEntry.Key, Is.EqualTo("AffectedRows"),
                "Result row key of create query is not 'AffectedRows', it is '" + o_resultEntry.Key + "'"
            );

            Assert.That(
                Convert.ToInt32(o_resultEntry.Value), Is.EqualTo(i_expectedAffectedRows),
                "Result row value of create query is not '" + i_expectedAffectedRows + "', it is '" + o_resultEntry.Value + "'"
            );

            /* #### CREATE ############################################################################# */
            o_queryCreate = new(o_glob.BaseGateway, SqlType.CREATE, "sys_forestnet_testddl2");
            /* #### Columns ############################################################################ */
            a_columnsDefinition = [];

            o_properties = new()
            {
                { "name", "Id" },
                { "columnType", "integer [int]" },
                { "constraints", "NOT NULL;PRIMARY KEY;AUTO_INCREMENT" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "UUID" },
                { "columnType", "text [36]" },
                { "constraints", "NOT NULL;UNIQUE" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "ShortText" },
                { "columnType", "text [255]" },
                { "constraints", "NULL" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "Text" },
                { "columnType", "text" },
                { "constraints", "NULL" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "SmallInt" },
                { "columnType", "integer [small]" },
                { "constraints", "NULL" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "Int" },
                { "columnType", "integer [int]" },
                { "constraints", "NULL" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "BigInt" },
                { "columnType", "integer [big]" },
                { "constraints", "NULL" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "Timestamp" },
                { "columnType", "datetime" },
                { "constraints", "NULL;DEFAULT" },
                { "constraintDefaultValue", "1999-12-31 23:00:00" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "Date" },
                { "columnType", "datetime" },
                { "constraints", "NULL;DEFAULT" },
                { "constraintDefaultValue", "2004-04-04 00:00:00" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "Time" },
                { "columnType", "time" },
                { "constraints", "NULL" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "LocalDateTime" },
                { "columnType", "datetime" },
                { "constraints", "NULL;DEFAULT" },
                { "constraintDefaultValue", "CURRENT_TIMESTAMP" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "LocalDate" },
                { "columnType", "datetime" },
                { "constraints", "NULL" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "LocalTime" },
                { "columnType", "time" },
                { "constraints", "DEFAULT" },
                { "constraintDefaultValue", "12:24:46" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "DoubleCol" },
                { "columnType", "double" },
                { "constraints", "NULL" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "Decimal" },
                { "columnType", "decimal" },
                { "constraints", "NULL" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "Bool" },
                { "columnType", "bool" },
                { "constraints", "NULL" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "Text2" },
                { "columnType", "text [36]" },
                { "constraints", "DEFAULT" },
                { "constraintDefaultValue", "Das ist das Haus vom Nikolaus" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "ShortText2" },
                { "columnType", "text [255]" },
                { "constraints", "DEFAULT" },
                { "constraintDefaultValue", "NULL" }
            };
            a_columnsDefinition.Add(o_properties);

            /* #### Query ############################################################################ */

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

            a_result = o_glob.Base?.FetchQuery(o_queryCreate) ?? [];

            i_expectedAffectedRows = 0;

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

            o_resultEntry = a_result[0].First();

            Assert.That(
                o_resultEntry.Key, Is.EqualTo("AffectedRows"),
                "Result row key of create query is not 'AffectedRows', it is '" + o_resultEntry.Key + "'"
            );

            Assert.That(
                Convert.ToInt32(o_resultEntry.Value), Is.EqualTo(i_expectedAffectedRows),
                "Result row value of create query is not '" + i_expectedAffectedRows + "', it is '" + o_resultEntry.Value + "'"
            );

            /* #### INSERT ############################################################################ */

            List<Dictionary<string, string>> a_insertColumnsDefinition = [];

            Dictionary<string, string> o_insertProperties = new()
            {
                { "Id", "1" },
                { "UUID", "9230337b-6cd9-11e9-b874-1062e50d1fcb" },
                { "Code", "de-DE" },
                { "Language", "Deutsch, Deutschland" }
            };
            a_insertColumnsDefinition.Add(o_insertProperties);

            o_insertProperties = new()
            {
                { "Id", "2" },
                { "UUID", "942b5547-6cd9-11e9-b874-1062e50d1fcb" },
                { "Code", "en-US" },
                { "Language", "English, United States" }
            };
            a_insertColumnsDefinition.Add(o_insertProperties);

            o_insertProperties = new()
            {
                { "Id", "3" },
                { "UUID", "966996d3-6cd9-11e9-b874-1062e50d1fcb" },
                { "Code", "en-GB" },
                { "Language", "English, Großbritannien" }
            };
            a_insertColumnsDefinition.Add(o_insertProperties);

            o_insertProperties = new()
            {
                { "Id", "4" },
                { "UUID", "44c37d45-222f-11ea-80e3-c85b7608f0ba" },
                { "Code", "it-IT" },
                { "Language", "Italian, Italy" }
            };
            a_insertColumnsDefinition.Add(o_insertProperties);

            o_insertProperties = new()
            {
                { "Id", "5" },
                { "UUID", "4f4bc151-222f-11ea-80e3-c85b7608f0ba" },
                { "Code", "es-SP" },
                { "Language", "Spanisch, Spaniens" }
            };
            a_insertColumnsDefinition.Add(o_insertProperties);

            o_insertProperties = new()
            {
                { "Id", "6" },
                { "UUID", "5bb8176f-222f-11ea-80e3-c85b7608f0ba" },
                { "Code", "jp-JP" },
                { "Language", "Japanese, Japan" }
            };
            a_insertColumnsDefinition.Add(o_insertProperties);

            o_insertProperties = new()
            {
                { "Id", "7" },
                { "UUID", "fe01d7d6-2232-11ea-80e3-c85b7608f0ba" },
                { "Code", "cz-CZ" },
                { "Language", "Tschechisch, Tschechien" }
            };
            a_insertColumnsDefinition.Add(o_insertProperties);

            o_insertProperties = new()
            {
                { "Id", "8" },
                { "UUID", "176176e2-57ff-4cef-baf5-7e2597f2a520" },
                { "Code", "en-AU" },
                { "Language", "English, Australia" }
            };
            a_insertColumnsDefinition.Add(o_insertProperties);

            o_insertProperties = new()
            {
                { "Id", "9" },
                { "UUID", "d38e447d-de17-47be-a26a-f57423fc439f" },
                { "Code", "de-OE" },
                { "Language", "Deutsch, Österreich" }
            };
            a_insertColumnsDefinition.Add(o_insertProperties);

            int i_insertId = 1;

            foreach (Dictionary<string, string> o_insertColumnDefinition in a_insertColumnsDefinition)
            {
                Query<Insert> o_queryInsert = new(o_glob.BaseGateway, SqlType.INSERT, "sys_forestnet_language");
                /* #### Columns ############################################################################ */
                (o_queryInsert.GetQuery<Insert>() ?? throw new NullReferenceException("insert query is null")).NoSQLMDBColumnAutoIncrement = new Column(o_queryInsert, "Id");
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "UUID"), o_insertColumnDefinition["UUID"]));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "Code"), o_insertColumnDefinition["Code"]));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "Language"), o_insertColumnDefinition["Language"]));

                a_result = o_glob.Base?.FetchQuery(o_queryInsert) ?? [];

                Assert.That(
                    a_result, Has.Count.EqualTo(1),
                    "Result row amount of insert query is not '1', it is '" + a_result.Count + "'"
                );

                foreach (Dictionary<string, Object?> o_row in a_result)
                {
                    int j = 0;

                    foreach (KeyValuePair<string, Object?> o_column in o_row)
                    {
                        if (j == 0)
                        {
                            Assert.That(
                                o_column.Key, Is.EqualTo("AffectedRows"),
                                "Result row key of insert query is not 'AffectedRows', it is '" + o_column.Key + "'"
                            );

                            Assert.That(
                                o_column.Value, Is.EqualTo(1),
                                "Result row value of insert query is not '1'"
                            );
                        }
                        else
                        {
                            Assert.That(
                                o_column.Key, Is.EqualTo("LastInsertId"),
                                "Result row key of insert query is not 'LastInsertId', it is '" + o_column.Key + "'"
                            );

                            Assert.That(
                                o_column.Value, Is.EqualTo(i_insertId),
                                "Result row value of insert query is not '" + i_insertId + "'"
                            );
                        }

                        j++;
                    }
                }

                i_insertId++;
            }

            /* #### INSERT ############################################################################ */

            DateTime o_dateTime = new(2019, 1, 1, 0, 1, 1);
            DateTime o_date = new(2001, 1, 1);
            TimeSpan o_time = new(1, 1, 1);

            DateTime o_localDateTime = new(2019, 1, 1, 0, 1, 1);
            DateTime o_localDate = new(2001, 1, 1);
            TimeSpan o_localTime = new(1, 1, 1);

            List<Dictionary<string, object?>> a_insertColumns = [];

            Dictionary<string, object?> o_insertProp = new()
            {
                { "Id", 1 },
                { "UUID", "32f80a3a-ff6e-4cc0-bb8f-ce630595382b" },
                { "ShortText", "Datensatz Eins" },
                { "Text", "Die Handelsstreitigkeiten zwischen den USA und China sorgen für eine Art Umdenken auf beiden Seiten. Während US-Unternehmen chinesische Hardware meiden, tun dies chinesische Unternehmen wohl mittlerweile auch: So denken laut einem Bericht der Nachrichtenagentur Bloomberg viele chinesische Hersteller stark darüber nach, ihre IT-Infrastruktur von lokalen Unternehmen statt von den US-Konzernen Oracle und IBM zu kaufen. Für diese Unternehmen sei der asiatische Markt wichtig. 16 respektive mehr als 20 Prozent des Umsatzes stammen aus dieser Region." },
                { "SmallInt", 1 },
                { "Int", 10_001 },
                { "BigInt", 100001111L },
                { "Timestamp", o_dateTime },
                { "Date", o_date },
                { "Time", o_time },
                { "LocalDateTime", o_localDateTime },
                { "LocalDate", o_localDate },
                { "LocalTime", o_localTime },
                { "DoubleCol", 1.23456789d },
                { "Decimal", 12345678.90m },
                { "Bool", true },
                { "Text2", "Das ist das Haus vom Nikolaus #1" },
                { "ShortText2", "Eins Datensatz" }
            };
            a_insertColumns.Add(o_insertProp);

            o_dateTime = new(2019, 2, 2, 1, 2, 2);
            o_date = new(2002, 2, 2);
            o_time = new(2, 2, 2);

            o_localDateTime = new(2019, 2, 2, 1, 2, 2);
            o_localDate = new(2002, 2, 2);
            o_localTime = new(2, 2, 2);

            o_insertProp = new()
            {
                { "Id", 2 },
                { "UUID", "390e413a-09df-41e3-aafe-8a116197da7f" },
                { "ShortText", "Datensatz Zwei" },
                { "Text", "Und hier ein single quote \'. Und dann noch ein Backslash \\. Das Tech-Startup Pingcap ist eines der lokalen Unternehmen, die den Handelsstreit zu ihrem Vorteil nutzen, für lokale chinesische Produkte werben und selbst von US-Hardware wegmigrieren. Mehr als 300 Kunden betreut die Firma, darunter der Fahrradsharing-Dienst Mobike und der chinesische Smartphone-Hersteller Xiaomi. Piingcap bietet beispielsweise auf Mysql basierende Datenbanken wie TiDB an." },
                { "SmallInt", 2 },
                { "Int", 20_002 },
                { "BigInt", 200002222L },
                { "Timestamp", o_dateTime },
                { "Date", o_date },
                { "Time", o_time },
                { "LocalDateTime", o_localDateTime },
                { "LocalDate", o_localDate },
                { "LocalTime", o_localTime },
                { "DoubleCol", 12.3456789d },
                { "Decimal", 1234567.890m },
                { "Bool", false },
                { "Text2", "Das ist das Haus vom Nikolaus #2" },
                { "ShortText2", "Zwei Datensatz" }
            };
            a_insertColumns.Add(o_insertProp);

            o_dateTime = new(2019, 3, 3, 2, 3, 3);
            o_date = new(2003, 3, 3);
            o_time = new(3, 3, 3);

            o_localDateTime = new(2019, 3, 3, 2, 3, 3);
            o_localDate = new(2003, 3, 3);
            o_localTime = new(3, 3, 3);

            o_insertProp = new()
            {
                { "Id", 3 },
                { "UUID", "ab0f2622-57d4-406e-a866-41e1bc5e4a3a" },
                { "ShortText", "Datensatz Drei" },
                { "Text", "\"Viele Firmen, die auf Oracle und IBM gesetzt haben, dachten es sei noch ein entfernter Meilenstein, diese zu ersetzen\", sagt Pingcap-CEO Huang Dongxu. \"Wir schauen uns aber mittlerweile Plan B ernsthaft an\". Allerdings seien chinesische Unternehmen laut dem lokalen Analystenunternehmen UOB Kay Hian noch nicht ganz bereit, wettbewerbsfähige Chips zu produzieren. \"Wenn sie aber genug gereift sind, werden [viele Unternehmen, Anm. d. Red.] ausländische Chips mit den lokalen ersetzen\", sagt die Firma." },
                { "SmallInt", 3 },
                { "Int", 30_003 },
                { "BigInt", 300003333L },
                { "Timestamp", o_dateTime },
                { "Date", o_date },
                { "Time", o_time },
                { "LocalDateTime", o_localDateTime },
                { "LocalDate", o_localDate },
                { "LocalTime", o_localTime },
                { "DoubleCol", 123.456789d },
                { "Decimal", 123456.7890m },
                { "Bool", true },
                { "Text2", "Das ist das Haus vom Nikolaus #3" },
                { "ShortText2", "Drei Datensatz" }
            };
            a_insertColumns.Add(o_insertProp);

            o_dateTime = new(2019, 4, 4, 3, 4, 4);
            o_date = new(2004, 4, 4);
            o_time = new(4, 4, 4);

            o_localDateTime = new(2019, 4, 4, 3, 4, 4);
            o_localDate = new(2004, 4, 4);
            o_localTime = new(4, 4, 4);

            o_insertProp = new()
            {
                { "Id", 4 },
                { "UUID", "9df3326e-b061-45e0-a3a1-4691ce0a349e" },
                { "ShortText", "Datensatz Vier" },
                { "Text", "China migriert schneller von US-Hardware auf lokale Chips. Immer mehr chinesische Unternehmen wollen anscheinend von amerikanischen Produkten auf lokal hergestellte Hardware setzen. Davon betroffen sind beispielsweise IBM und Oracle, die einen großen Teil ihres Umsatzes in Asien machen. Noch sei die chinesische Technik aber nicht weit genug." },
                { "SmallInt", 4 },
                { "Int", 40_004 },
                { "BigInt", 400004444L },
                { "Timestamp", o_dateTime },
                { "Date", o_date },
                { "Time", o_time },
                { "LocalDateTime", o_localDateTime },
                { "LocalDate", o_localDate },
                { "LocalTime", o_localTime },
                { "DoubleCol", 1234.56789d },
                { "Decimal", 12345.67890m },
                { "Bool", false },
                { "Text2", "Das ist das Haus vom Nikolaus #4" },
                { "ShortText2", "Vier Datensatz" }
            };
            a_insertColumns.Add(o_insertProp);

            o_dateTime = new(2019, 5, 5, 4, 5, 5);
            o_date = new(2005, 5, 5);
            o_time = new(5, 5, 5);

            o_localDateTime = new(2019, 5, 5, 4, 5, 5);
            o_localDate = new(2005, 5, 5);
            o_localTime = new(5, 5, 5);

            o_insertProp = new()
            {
                { "Id", 5 },
                { "UUID", "1b851742-fc49-4fc4-b1c4-0b7cac6ae5af" },
                { "ShortText", "Datensatz Fünf" },
                { "Text", "Weder IBM noch Oracle haben Bloomberg auf eine Anfrage hin geantwortet. SQL INJECTION:\';DELETE FROM items;. Dass die US-Regierung China wirtschaftlich unter Druck setzt, könnte allerdings zu unerwünschten Ergebnissen und dem schnellen Verlust eines Marktes mit fast 1,4 Milliarden Einwohnern führen." },
                { "SmallInt", 5 },
                { "Int", 50_005 },
                { "BigInt", 500005555L },
                { "Timestamp", o_dateTime },
                { "Date", o_date },
                { "Time", o_time },
                { "LocalDateTime", o_localDateTime },
                { "LocalDate", o_localDate },
                { "LocalTime", o_localTime },
                { "DoubleCol", 12345.6789d },
                { "Decimal", 1234.567890m },
                { "Bool", true },
                { "Text2", "Das ist das Haus vom Nikolaus #5" },
                { "ShortText2", "Fünf Datensatz" }
            };
            a_insertColumns.Add(o_insertProp);

            i_insertId = 1;

            foreach (Dictionary<string, Object?> o_insertColumnDefinition in a_insertColumns)
            {
                Query<Insert> o_queryInsert = new(o_glob.BaseGateway, SqlType.INSERT, "sys_forestnet_testddl2");
                /* #### Columns ############################################################################ */

                (o_queryInsert.GetQuery<Insert>() ?? throw new NullReferenceException("insert query is null")).NoSQLMDBColumnAutoIncrement = new Column(o_queryInsert, "Id");
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "UUID"), o_insertColumnDefinition["UUID"]));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "ShortText"), o_insertColumnDefinition["ShortText"]));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "Text"), o_insertColumnDefinition["Text"]));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "SmallInt"), o_insertColumnDefinition["SmallInt"]));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "Int"), o_insertColumnDefinition["Int"]));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "BigInt"), o_insertColumnDefinition["BigInt"]));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "Timestamp"), o_insertColumnDefinition["Timestamp"]));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "Date"), o_insertColumnDefinition["Date"]));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "Time"), o_insertColumnDefinition["Time"]));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "LocalDateTime"), o_insertColumnDefinition["LocalDateTime"]));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "LocalDate"), o_insertColumnDefinition["LocalDate"]));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "LocalTime"), o_insertColumnDefinition["LocalTime"]));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "DoubleCol"), o_insertColumnDefinition["DoubleCol"]));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "Decimal"), o_insertColumnDefinition["Decimal"]));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "Bool"), o_insertColumnDefinition["Bool"]));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "Text2"), o_insertColumnDefinition["Text2"]));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "ShortText2"), o_insertColumnDefinition["ShortText2"]));

                a_result = o_glob.Base?.FetchQuery(o_queryInsert) ?? [];

                Assert.That(
                    a_result, Has.Count.EqualTo(1),
                    "Result row amount of insert query is not '1', it is '" + a_result.Count + "'"
                );

                foreach (Dictionary<string, Object?> o_row in a_result)
                {
                    int j = 0;

                    foreach (KeyValuePair<string, Object?> o_column in o_row)
                    {
                        if (j == 0)
                        {
                            Assert.That(
                                o_column.Key, Is.EqualTo("AffectedRows"),
                                "Result row key of insert query is not 'AffectedRows', it is '" + o_column.Key + "'"
                            );

                            Assert.That(
                                o_column.Value, Is.EqualTo(1),
                                "Result row value of insert query is not '1'"
                            );
                        }
                        else
                        {
                            Assert.That(
                                o_column.Key, Is.EqualTo("LastInsertId"),
                                "Result row key of insert query is not 'LastInsertId', it is '" + o_column.Key + "'"
                            );

                            Assert.That(
                                o_column.Value, Is.EqualTo(i_insertId),
                                "Result row value of insert query is not '" + i_insertId + "'"
                            );
                        }

                        j++;
                    }
                }

                i_insertId++;
            }
        }

        private static void CleanupRecordTest(bool p_b_checkResult)
        {
            ForestNET.Lib.Global o_glob = ForestNET.Lib.Global.Instance;

            Query<Drop> o_queryDrop = new(o_glob.BaseGateway, SqlType.DROP, "sys_forestnet_language");

            List<Dictionary<string, Object?>> a_result = o_glob.Base?.FetchQuery(o_queryDrop) ?? [];

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

            if (p_b_checkResult)
            {
                Assert.That(
                    a_result, Has.Count.EqualTo(1),
                    "Result row amount of drop query #1 is not '1', it is '" + a_result.Count + "'"
                );

                KeyValuePair<string, Object?> o_resultEntry = a_result[0].First();

                Assert.That(
                    o_resultEntry.Key, Is.EqualTo("AffectedRows"),
                    "Result row key of drop query #1 is not 'AffectedRows', it is '" + o_resultEntry.Key + "'"
                );

                Assert.That(
                    Convert.ToInt32(o_resultEntry.Value), Is.EqualTo(i_expectedAffectedRows),
                    "Result row value of drop query #1 is not '" + i_expectedAffectedRows + "', it is '" + o_resultEntry.Value + "'"
                );
            }

            o_queryDrop = new Query<Drop>(o_glob.BaseGateway, SqlType.DROP, "sys_forestnet_testddl2");

            a_result = o_glob.Base?.FetchQuery(o_queryDrop) ?? [];

            i_expectedAffectedRows = 0;

            /* nosqlmdb has expected value 1 */
            if (o_glob.BaseGateway == BaseGateway.NOSQLMDB)
            {
                i_expectedAffectedRows = 1;
            }
            else if ((o_glob.BaseGateway == BaseGateway.MSSQL) || (o_glob.BaseGateway == BaseGateway.PGSQL) || (o_glob.BaseGateway == BaseGateway.ORACLE))
            {
                i_expectedAffectedRows = -1;
            }

            if (p_b_checkResult)
            {
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
        }

        private static void TestLanguageRecord()
        {
            /* test GetRecord */
            LanguageRecord o_languageRecord = new();

            Assert.That(
                o_languageRecord.GetRecord([2]), Is.True,
                "Record with Id=2 not found"
            );

            Assert.That(o_languageRecord.ColumnId, Is.EqualTo(2), "ColumnId[" + o_languageRecord.ColumnId + "] is not equal to '" + 2 + "'");
            Assert.That(o_languageRecord.ColumnUUID, Is.EqualTo("942b5547-6cd9-11e9-b874-1062e50d1fcb"), "ColumnUUID[" + o_languageRecord.ColumnUUID + "] is not equal to '942b5547-6cd9-11e9-b874-1062e50d1fcb'");
            Assert.That(o_languageRecord.ColumnCode, Is.EqualTo("en-US"), "ColumnCode[" + o_languageRecord.ColumnCode + "] is not equal to 'en-US'");
            Assert.That(o_languageRecord.ColumnLanguage, Is.EqualTo("English, United States"), "ColumnLanguage[" + o_languageRecord.ColumnLanguage + "] is not equal to 'English, United States'");

            /* test GetOneRecord */

            o_languageRecord = new();

            Assert.That(
                o_languageRecord.GetOneRecord(["Code"], ["de-DE"]),
                "Record with Code='de-DE' not found"
            );

            Assert.That(o_languageRecord.ColumnId, Is.EqualTo(1), "ColumnId[" + o_languageRecord.ColumnId + "] is not equal to '" + 1 + "'");
            Assert.That(o_languageRecord.ColumnUUID, Is.EqualTo("9230337b-6cd9-11e9-b874-1062e50d1fcb"), "ColumnUUID[" + o_languageRecord.ColumnUUID + "] is not equal to '9230337b-6cd9-11e9-b874-1062e50d1fcb'");
            Assert.That(o_languageRecord.ColumnCode, Is.EqualTo("de-DE"), "ColumnCode[" + o_languageRecord.ColumnCode + "] is not equal to 'de-DE'");
            Assert.That(o_languageRecord.ColumnLanguage, Is.EqualTo("Deutsch, Deutschland"), "ColumnLanguage[" + o_languageRecord.ColumnLanguage + "] is not equal to 'Deutsch, Deutschland'");

            /* test GetRecords(true) */

            o_languageRecord = new();

            List<LanguageRecord> a_records = o_languageRecord.GetRecords(true);

            Assert.That(
                a_records, Has.Count.EqualTo(9),
                "GetRecords(true) result rows are not '9', but '" + a_records.Count + "'"
            );

            int i = 1;

            foreach (LanguageRecord a_record in a_records)
            {
                if (i == 1)
                {
                    Assert.That(a_record.ColumnId, Is.EqualTo(i), "ColumnId[" + a_record.ColumnId + "] is not equal to '" + i + "'");
                    Assert.That(a_record.ColumnUUID, Is.EqualTo("9230337b-6cd9-11e9-b874-1062e50d1fcb"), "ColumnUUID[" + a_record.ColumnUUID + "] is not equal to '9230337b-6cd9-11e9-b874-1062e50d1fcb'");
                    Assert.That(a_record.ColumnCode, Is.EqualTo("de-DE"), "ColumnCode[" + a_record.ColumnCode + "] is not equal to 'de-DE'");
                    Assert.That(a_record.ColumnLanguage, Is.EqualTo("Deutsch, Deutschland"), "ColumnLanguage[" + a_record.ColumnLanguage + "] is not equal to 'Deutsch, Deutschland'");
                }
                else if (i == 2)
                {
                    Assert.That(a_record.ColumnId, Is.EqualTo(i), "ColumnId[" + a_record.ColumnId + "] is not equal to '" + i + "'");
                    Assert.That(a_record.ColumnUUID, Is.EqualTo("942b5547-6cd9-11e9-b874-1062e50d1fcb"), "ColumnUUID[" + a_record.ColumnUUID + "] is not equal to '942b5547-6cd9-11e9-b874-1062e50d1fcb'");
                    Assert.That(a_record.ColumnCode, Is.EqualTo("en-US"), "ColumnCode[" + a_record.ColumnCode + "] is not equal to 'en-US'");
                    Assert.That(a_record.ColumnLanguage, Is.EqualTo("English, United States"), "ColumnLanguage[" + a_record.ColumnLanguage + "] is not equal to 'English, United States'");
                }
                else if (i == 3)
                {
                    Assert.That(a_record.ColumnId, Is.EqualTo(i), "ColumnId[" + a_record.ColumnId + "] is not equal to '" + i + "'");
                    Assert.That(a_record.ColumnUUID, Is.EqualTo("966996d3-6cd9-11e9-b874-1062e50d1fcb"), "ColumnUUID[" + a_record.ColumnUUID + "] is not equal to '966996d3-6cd9-11e9-b874-1062e50d1fcb'");
                    Assert.That(a_record.ColumnCode, Is.EqualTo("en-GB"), "ColumnCode[" + a_record.ColumnCode + "] is not equal to 'en-GB'");
                    Assert.That(a_record.ColumnLanguage, Is.EqualTo("English, Großbritannien"), "ColumnLanguage[" + a_record.ColumnLanguage + "] is not equal to 'English, Großbritannien'");
                }
                else if (i == 4)
                {
                    Assert.That(a_record.ColumnId, Is.EqualTo(i), "ColumnId[" + a_record.ColumnId + "] is not equal to '" + i + "'");
                    Assert.That(a_record.ColumnUUID, Is.EqualTo("44c37d45-222f-11ea-80e3-c85b7608f0ba"), "ColumnUUID[" + a_record.ColumnUUID + "] is not equal to '44c37d45-222f-11ea-80e3-c85b7608f0ba'");
                    Assert.That(a_record.ColumnCode, Is.EqualTo("it-IT"), "ColumnCode[" + a_record.ColumnCode + "] is not equal to 'it-IT'");
                    Assert.That(a_record.ColumnLanguage, Is.EqualTo("Italian, Italy"), "ColumnLanguage[" + a_record.ColumnLanguage + "] is not equal to 'Italian, Italy'");
                }
                else if (i == 5)
                {
                    Assert.That(a_record.ColumnId, Is.EqualTo(i), "ColumnId[" + a_record.ColumnId + "] is not equal to '" + i + "'");
                    Assert.That(a_record.ColumnUUID, Is.EqualTo("4f4bc151-222f-11ea-80e3-c85b7608f0ba"), "ColumnUUID[" + a_record.ColumnUUID + "] is not equal to '4f4bc151-222f-11ea-80e3-c85b7608f0ba'");
                    Assert.That(a_record.ColumnCode, Is.EqualTo("es-SP"), "ColumnCode[" + a_record.ColumnCode + "] is not equal to 'es-SP'");
                    Assert.That(a_record.ColumnLanguage, Is.EqualTo("Spanisch, Spaniens"), "ColumnLanguage[" + a_record.ColumnLanguage + "] is not equal to 'Spanisch, Spaniens'");
                }
                else if (i == 6)
                {
                    Assert.That(a_record.ColumnId, Is.EqualTo(i), "ColumnId[" + a_record.ColumnId + "] is not equal to '" + i + "'");
                    Assert.That(a_record.ColumnUUID, Is.EqualTo("5bb8176f-222f-11ea-80e3-c85b7608f0ba"), "ColumnUUID[" + a_record.ColumnUUID + "] is not equal to '5bb8176f-222f-11ea-80e3-c85b7608f0ba'");
                    Assert.That(a_record.ColumnCode, Is.EqualTo("jp-JP"), "ColumnCode[" + a_record.ColumnCode + "] is not equal to 'jp-JP'");
                    Assert.That(a_record.ColumnLanguage, Is.EqualTo("Japanese, Japan"), "ColumnLanguage[" + a_record.ColumnLanguage + "] is not equal to 'Japanese, Japan'");
                }
                else if (i == 7)
                {
                    Assert.That(a_record.ColumnId, Is.EqualTo(i), "ColumnId[" + a_record.ColumnId + "] is not equal to '" + i + "'");
                    Assert.That(a_record.ColumnUUID, Is.EqualTo("fe01d7d6-2232-11ea-80e3-c85b7608f0ba"), "ColumnUUID[" + a_record.ColumnUUID + "] is not equal to 'fe01d7d6-2232-11ea-80e3-c85b7608f0ba'");
                    Assert.That(a_record.ColumnCode, Is.EqualTo("cz-CZ"), "ColumnCode[" + a_record.ColumnCode + "] is not equal to 'cz-CZ'");
                    Assert.That(a_record.ColumnLanguage, Is.EqualTo("Tschechisch, Tschechien"), "ColumnLanguage[" + a_record.ColumnLanguage + "] is not equal to 'Tschechisch, Tschechien'");
                }
                else if (i == 8)
                {
                    Assert.That(a_record.ColumnId, Is.EqualTo(i), "ColumnId[" + a_record.ColumnId + "] is not equal to '" + i + "'");
                    Assert.That(a_record.ColumnUUID, Is.EqualTo("176176e2-57ff-4cef-baf5-7e2597f2a520"), "ColumnUUID[" + a_record.ColumnUUID + "] is not equal to '176176e2-57ff-4cef-baf5-7e2597f2a520'");
                    Assert.That(a_record.ColumnCode, Is.EqualTo("en-AU"), "ColumnCode[" + a_record.ColumnCode + "] is not equal to 'en-AU'");
                    Assert.That(a_record.ColumnLanguage, Is.EqualTo("English, Australia"), "ColumnLanguage[" + a_record.ColumnLanguage + "] is not equal to 'English, Australia'");
                }
                else if (i == 9)
                {
                    Assert.That(a_record.ColumnId, Is.EqualTo(i), "ColumnId[" + a_record.ColumnId + "] is not equal to '" + i + "'");
                    Assert.That(a_record.ColumnUUID, Is.EqualTo("d38e447d-de17-47be-a26a-f57423fc439f"), "ColumnUUID[" + a_record.ColumnUUID + "] is not equal to 'd38e447d-de17-47be-a26a-f57423fc439f'");
                    Assert.That(a_record.ColumnCode, Is.EqualTo("de-OE"), "ColumnCode[" + a_record.ColumnCode + "] is not equal to 'de-OE'");
                    Assert.That(a_record.ColumnLanguage, Is.EqualTo("Deutsch, Österreich"), "ColumnLanguage[" + a_record.ColumnLanguage + "] is not equal to 'Deutsch, Österreich'");
                }

                i++;
            }

            /* test change record object with other sort and filter */

            o_languageRecord = new();

            o_languageRecord.Columns.Add("Code");
            o_languageRecord.Columns.Add("Language");

            o_languageRecord.Sort.Add("Code", false);

            o_languageRecord.Filters.Add(new Filter("Id", 2, ">="));
            o_languageRecord.Filters.Add(new Filter("Id", 6, "<=", "AND"));
            o_languageRecord.Filters.Add(new Filter("Language", "%e%", "LIKE", "AND"));

            o_languageRecord.Interval = 2;
            o_languageRecord.Page = 2;

            a_records = o_languageRecord.GetRecords();

            Assert.That(
                a_records, Has.Count.EqualTo(2),
                "GetRecords result rows are not '2', but '" + a_records.Count + "'"
            );

            i = 1;

            foreach (LanguageRecord a_record in a_records)
            {
                if (i == 1)
                {
                    Assert.That(a_record.ColumnId, Is.EqualTo(0), "ColumnId[" + a_record.ColumnId + "] is not equal to '" + 0 + "'");
                    Assert.That(a_record.ColumnUUID, Is.EqualTo(null), "ColumnUUID[" + a_record.ColumnUUID + "] is not equal to 'null'");
                    Assert.That(a_record.ColumnCode, Is.EqualTo("en-US"), "ColumnCode[" + a_record.ColumnCode + "] is not equal to 'en-US'");
                    Assert.That(a_record.ColumnLanguage, Is.EqualTo("English, United States"), "ColumnLanguage[" + a_record.ColumnLanguage + "] is not equal to 'English, United States'");
                }
                else if (i == 2)
                {
                    Assert.That(a_record.ColumnId, Is.EqualTo(0), "ColumnId[" + a_record.ColumnId + "] is not equal to '" + 0 + "'");
                    Assert.That(a_record.ColumnUUID, Is.EqualTo(null), "ColumnUUID[" + a_record.ColumnUUID + "] is not equal to 'null'");
                    Assert.That(a_record.ColumnCode, Is.EqualTo("en-GB"), "ColumnCode[" + a_record.ColumnCode + "] is not equal to 'en-GB'");
                    Assert.That(a_record.ColumnLanguage, Is.EqualTo("English, Großbritannien"), "ColumnLanguage[" + a_record.ColumnLanguage + "] is not equal to 'English, Großbritannien'");
                }

                i++;
            }

            /* test InsertRecord */

            o_languageRecord = new();

            string s_temp = o_languageRecord.ColumnUUID = ForestNET.Lib.Helper.GenerateUUID();
            o_languageRecord.ColumnCode = "de-XX";
            o_languageRecord.ColumnLanguage = "DeleteMePlz";

            int i_foo = o_languageRecord.InsertRecord();

            Assert.That(
                i_foo, Is.EqualTo(10),
                "InsertRecord result is not '10', but '" + i_foo + "'"
            );

            o_languageRecord = new LanguageRecord();

            Assert.That(
                o_languageRecord.GetRecord([10]), Is.True,
                "Record with Id=10 not found"
            );

            Assert.That(o_languageRecord.ColumnId, Is.EqualTo(10), "ColumnId[" + o_languageRecord.ColumnId + "] is not equal to '" + 10 + "'");
            Assert.That(o_languageRecord.ColumnUUID, Is.EqualTo(s_temp), "ColumnUUID[" + o_languageRecord.ColumnUUID + "] is not equal to '" + s_temp + "'");
            Assert.That(o_languageRecord.ColumnCode, Is.EqualTo("de-XX"), "ColumnCode[" + o_languageRecord.ColumnCode + "] is not equal to 'de-XX'");
            Assert.That(o_languageRecord.ColumnLanguage, Is.EqualTo("DeleteMePlz"), "ColumnLanguage[" + o_languageRecord.ColumnLanguage + "] is not equal to 'DeleteMePlz'");

            /* test UpdateRecord with GetRecord */

            o_languageRecord = new();

            Assert.That(
                o_languageRecord.GetRecord([5]), Is.True,
                "Record with Id=5 not found"
            );

            o_languageRecord.ColumnLanguage = "Spanisch, Spanien";

            Assert.That(
                o_languageRecord.UpdateRecord(),
                Is.EqualTo(1),
                "UpdateRecord of record #5 was not successful"
            );

            o_languageRecord = new();

            Assert.That(
                o_languageRecord.GetRecord([5]), Is.True,
                "Record with Id=5 not found"
            );

            Assert.That(o_languageRecord.ColumnId, Is.EqualTo(5), "ColumnId[" + o_languageRecord.ColumnId + "] is not equal to '" + 5 + "'");
            Assert.That(o_languageRecord.ColumnUUID, Is.EqualTo("4f4bc151-222f-11ea-80e3-c85b7608f0ba"), "ColumnUUID[" + o_languageRecord.ColumnUUID + "] is not equal to '4f4bc151-222f-11ea-80e3-c85b7608f0ba'");
            Assert.That(o_languageRecord.ColumnCode, Is.EqualTo("es-SP"), "ColumnCode[" + o_languageRecord.ColumnCode + "] is not equal to 'es-SP'");
            Assert.That(o_languageRecord.ColumnLanguage, Is.EqualTo("Spanisch, Spanien"), "ColumnLanguage[" + o_languageRecord.ColumnLanguage + "] is not equal to 'Spanisch, Spanien'");

            /* test UpdateRecord with GetOneRecord */

            o_languageRecord = new();

            Assert.That(
                o_languageRecord.GetOneRecord(["Language"], ["Deutsch, Österreich"]), Is.True,
                "Record with Language='Deutsch, Österreich' not found"
            );

            o_languageRecord.ColumnLanguage = "German, Austria";

            Assert.That(
                o_languageRecord.UpdateRecord(),
                Is.EqualTo(1),
                "UpdateRecord of record #9 was not successful"
            );

            o_languageRecord = new();

            Assert.That(
                o_languageRecord.GetRecord([9]), Is.True,
                "Record with Id=9 not found"
            );

            Assert.That(o_languageRecord.ColumnId, Is.EqualTo(9), "ColumnId[" + o_languageRecord.ColumnId + "] is not equal to '" + 9 + "'");
            Assert.That(o_languageRecord.ColumnUUID, Is.EqualTo("d38e447d-de17-47be-a26a-f57423fc439f"), "ColumnUUID[" + o_languageRecord.ColumnUUID + "] is not equal to 'd38e447d-de17-47be-a26a-f57423fc439f'");
            Assert.That(o_languageRecord.ColumnCode, Is.EqualTo("de-OE"), "ColumnCode[" + o_languageRecord.ColumnCode + "] is not equal to 'de-OE'");
            Assert.That(o_languageRecord.ColumnLanguage, Is.EqualTo("German, Austria"), "ColumnLanguage[" + o_languageRecord.ColumnLanguage + "] is not equal to 'German, Austria'");

            /* test GetOneRecord but false result */

            o_languageRecord = new();

            Assert.That(
                o_languageRecord.GetOneRecord(["Code"], ["de-XF"]), Is.False,
                "Record with Code='de-XF' found"
            );

            /* test DeleteRecord with GetOneRecord but true result */

            o_languageRecord = new();

            Assert.That(
                o_languageRecord.GetOneRecord(["Code"], ["de-XX"]), Is.True,
                "Record with Code='de-XX' not found"
            );

            Assert.That(
                o_languageRecord.DeleteRecord(),
                Is.EqualTo(1),
                "DeleteRecord of record #10 was not successful"
            );

            /* test null record, UpdateRecord and GetOneRecord */

            int i_id = 11;

            /* nosqlmdb has id 10, because we deleted 10th record before */
            if (ForestNET.Lib.Global.Instance.BaseGateway == BaseGateway.NOSQLMDB)
            {
                i_id = 10;
            }

            o_languageRecord = new()
            {
                ColumnUUID = "804ce8e2-4951-4480-a0fa-e539f9303741",
                ColumnCode = "em-TY"
            };

            i_foo = o_languageRecord.InsertRecord();

            Assert.That(
                i_foo, Is.EqualTo(i_id),
                "InsertRecord result is not '" + i_id + "', but '" + i_foo + "'"
            );

            o_languageRecord = new();

            Assert.That(
                o_languageRecord.GetRecord([i_id]), Is.True,
                "Record with Id=" + i_id + " not found"
            );

            Assert.That(o_languageRecord.ColumnId, Is.EqualTo(i_id), "ColumnId[" + o_languageRecord.ColumnId + "] is not equal to '" + i_id + "'");
            Assert.That(o_languageRecord.ColumnUUID, Is.EqualTo("804ce8e2-4951-4480-a0fa-e539f9303741"), "ColumnUUID[" + o_languageRecord.ColumnUUID + "] is not equal to '804ce8e2-4951-4480-a0fa-e539f9303741'");
            Assert.That(o_languageRecord.ColumnCode, Is.EqualTo("em-TY"), "ColumnCode[" + o_languageRecord.ColumnCode + "] is not equal to 'em-TY'");
            Assert.That(o_languageRecord.ColumnLanguage, Is.EqualTo(null), "ColumnLanguage[" + o_languageRecord.ColumnLanguage + "] is not equal to 'null'");

            o_languageRecord.ColumnCode = "es-MX";

            Assert.That(
                o_languageRecord.UpdateRecord(),
                Is.EqualTo(1),
                "UpdateRecord of record #" + i_id + " was not successful"
            );

            o_languageRecord = new();

            Assert.That(
                o_languageRecord.GetRecord([i_id]), Is.True,
                "Record with Id=" + i_id + " not found"
            );

            Assert.That(o_languageRecord.ColumnId, Is.EqualTo(i_id), "ColumnId[" + o_languageRecord.ColumnId + "] is not equal to '" + i_id + "'");
            Assert.That(o_languageRecord.ColumnUUID, Is.EqualTo("804ce8e2-4951-4480-a0fa-e539f9303741"), "ColumnUUID[" + o_languageRecord.ColumnUUID + "] is not equal to '804ce8e2-4951-4480-a0fa-e539f9303741'");
            Assert.That(o_languageRecord.ColumnCode, Is.EqualTo("es-MX"), "ColumnCode[" + o_languageRecord.ColumnCode + "] is not equal to 'em-TY'");
            Assert.That(o_languageRecord.ColumnLanguage, Is.EqualTo(null), "ColumnLanguage[" + o_languageRecord.ColumnLanguage + "] is not equal to 'null'");

            /* test truncate */

            int i_expectedValue = 0;

            /* nosqlmdb has expected value 1 */
            if (ForestNET.Lib.Global.Instance.BaseGateway == BaseGateway.NOSQLMDB)
            {
                i_expectedValue = 1;
            }

            o_languageRecord = new LanguageRecord();

            Assert.That(
                o_languageRecord.TruncateTable(),
                Is.EqualTo(i_expectedValue),
                "TruncateTable was not successful"
            );
        }

        private static void TestDDLRecord()
        {
            /* test GetRecord */

            DDLRecord o_ddlRecord = new();

            Assert.That(
                o_ddlRecord.GetRecord([1]),
                "Record with Id=1 not found"
            );

            CheckDDLRecord(o_ddlRecord, 1);

            /* test GetOneRecord */

            o_ddlRecord = new();

            Assert.That(
                o_ddlRecord.GetOneRecord(["Int"], [20002]),
                "Record with Int=20002 not found"
            );

            CheckDDLRecord(o_ddlRecord, 2);

            /* test GetRecords(true) */

            o_ddlRecord = new();

            List<DDLRecord> a_records = o_ddlRecord.GetRecords(true);

            Assert.That(
                a_records, Has.Count.EqualTo(5),
                "GetRecords(true) result rows are not '5', but '" + a_records.Count + "'"
            );

            int i = 1;

            /* very strict unit test for expected types and values */

            foreach (DDLRecord a_record in a_records)
            {
                CheckDDLRecord(a_record, i);

                i++;
            }

            /* test change record object with other sort and filter */

            o_ddlRecord = new();

            o_ddlRecord.Columns.Add("ShortText");
            o_ddlRecord.Columns.Add("BigInt");
            o_ddlRecord.Columns.Add("Timestamp");
            o_ddlRecord.Columns.Add("Time");
            o_ddlRecord.Columns.Add("DoubleCol");
            o_ddlRecord.Columns.Add("Decimal");

            o_ddlRecord.Sort.Add("ShortText", false);

            o_ddlRecord.Filters.Add(new Filter("Bool", true, "="));

            o_ddlRecord.Interval = 2;
            o_ddlRecord.Page = 1;

            a_records = o_ddlRecord.GetRecords();

            Assert.That(
                a_records, Has.Count.EqualTo(2),
                "GetRecords result rows are not '2', but '" + a_records.Count + "'"
            );

            i = 10;

            foreach (DDLRecord a_record in a_records)
            {
                CheckDDLRecord(a_record, i);

                i++;
            }

            /* test InsertRecord */

            o_ddlRecord = new()
            {
                ColumnUUID = "6df565d2-30fa-48ab-9f53-83b041f7210e",
                ColumnShortText = "Datensatz Sechs",
                ColumnText = " Die Berliner Staatsanwaltschaft hat das erste justizielle Rechtshilfeersuchen erst am 6. Dezember der russischen Generalstaatsanwaltschaft äbersandt. Das geht aus einer schriftlichen Anfrage der Linken-Abgeordneten Sevim Dagdelen hervor, die dem ARD-Hauptstadtstudio exklusiv vorliegt. Ein zweites Rechtshilfeersuchen ist demnach am 10. Dezember äbersandt worden. Schon Tage zuvor, nämlich am 4. Dezember, hatte die Bundesrepublik zwei russische Diplomaten ausgewiesen, da die russische Seite die Zusammenarbeit bei der Aufklärung des Mordes verzägert und erschwert habe.",
                ColumnSmallInt = (short)6,
                ColumnInt = 60006,
                ColumnBigInt = 600006666L,
                ColumnTimestamp = new DateTime(2019, 6, 6, 5, 6, 6),
                ColumnDate = new DateTime(2006, 6, 6),
                ColumnTime = new TimeSpan(6, 6, 6),
                ColumnLocalDateTime = new DateTime(2019, 6, 6, 5, 6, 6),
                ColumnLocalDate = new DateTime(2006, 6, 6),
                ColumnLocalTime = new TimeSpan(6, 6, 6),
                ColumnDoubleCol = 123456.789d,
                ColumnDecimal = 123.456789m,
                ColumnBool = false,
                ColumnText2 = "Das ist das Haus vom Nikolaus #6",
                ColumnShortText2 = "Sechs Datensatz"
            };

            int i_foo = o_ddlRecord.InsertRecord();

            Assert.That(
                i_foo, Is.EqualTo(6),
                "InsertRecord result is not '6', but '" + i_foo + "'"
            );

            o_ddlRecord = new();

            Assert.That(
                o_ddlRecord.GetRecord([6]),
                "Record with Id=6 not found"
            );

            CheckDDLRecord(o_ddlRecord, 6);

            /* test UpdateRecord with GetRecord */

            o_ddlRecord = new();

            Assert.That(
                o_ddlRecord.GetRecord([3]),
                "Record with Id=3 not found"
            );

            o_ddlRecord.ColumnShortText = "Datensatz Drei geändert";

            Assert.That(
                o_ddlRecord.UpdateRecord(),
                Is.EqualTo(1),
                "UpdateRecord of record #3 was not successful"
            );

            o_ddlRecord = new();

            Assert.That(
                o_ddlRecord.GetRecord([3]),
                "Record with Id=3 not found"
            );

            CheckDDLRecord(o_ddlRecord, 3, true);

            /* test UpdateRecord with GetOneRecord */

            o_ddlRecord = new();

            Assert.That(
                o_ddlRecord.GetOneRecord(["Int"], [20002]),
                "Record with Int=20002 not found"
            );

            o_ddlRecord.ColumnShortText = "Datensatz Zwei geändert";

            Assert.That(
                o_ddlRecord.UpdateRecord(),
                Is.EqualTo(1),
                "UpdateRecord of record #2 was not successful"
            );

            o_ddlRecord = new();

            Assert.That(
                o_ddlRecord.GetRecord([2]),
                "Record with Id=2 not found"
            );

            CheckDDLRecord(o_ddlRecord, 2, true);

            /* test DeleteRecord with GetOneRecord but false result */

            o_ddlRecord = new();

            Assert.That(
                !o_ddlRecord.GetOneRecord(["Int"], [123456]),
                "Record with Int=123465 found"
            );

            /* test DeleteRecord with GetOneRecord but true result */

            o_ddlRecord = new();

            Assert.That(
                o_ddlRecord.GetOneRecord(["ShortText"], ["Datensatz Sechs"]),
                "Record with ShortText='Datensatz Sechs' not found"
            );

            Assert.That(
                o_ddlRecord.DeleteRecord(),
                Is.EqualTo(1),
                "DeleteRecord of record #6 was not successful"
            );

            /* test null record, UpdateRecord and GetOneRecord */

            int i_id = 7;

            /* nosqlmdb has id 6, because we deleted 6th record before */
            if (ForestNET.Lib.Global.Instance.BaseGateway == BaseGateway.NOSQLMDB)
            {
                i_id = 6;
            }

            o_ddlRecord = new()
            {
                ColumnUUID = "cf998a07-03c1-4ce1-a063-5f52a940c3a1"
            };

            i_foo = o_ddlRecord.InsertRecord();

            Assert.That(
                i_foo,
                Is.EqualTo(i_id),
                "InsertRecord result is not '" + i_id + "', but '" + i_foo + "'"
            );

            o_ddlRecord = new();

            Assert.That(
                o_ddlRecord.GetRecord([i_id]),
                "Record with Id=" + i_id + " not found"
            );

            CheckDDLRecord(o_ddlRecord, (ForestNET.Lib.Global.Instance.BaseGateway != BaseGateway.NOSQLMDB) ? i_id : i_id + 1);

            o_ddlRecord.ColumnShortText = "short text";

            Assert.That(
                o_ddlRecord.UpdateRecord(),
                Is.EqualTo(1),
                "UpdateRecord of record #" + i_id + " was not successful"
            );

            o_ddlRecord = new();

            Assert.That(
                o_ddlRecord.GetOneRecord(["ShortText"], ["short text"]),
                "Record with ShortText='short text' not found"
            );

            CheckDDLRecord(o_ddlRecord, (ForestNET.Lib.Global.Instance.BaseGateway != BaseGateway.NOSQLMDB) ? i_id : i_id + 1, true);

            /* test truncate */

            int i_expectedValue = 0;

            /* nosqlmdb has expected value 1 */
            if (ForestNET.Lib.Global.Instance.BaseGateway == BaseGateway.NOSQLMDB)
            {
                i_expectedValue = 1;
            }

            o_ddlRecord = new DDLRecord();

            Assert.That(
                o_ddlRecord.TruncateTable(),
                Is.EqualTo(i_expectedValue),
                "TruncateTable was not successful"
            );
        }

        private static void CheckDDLRecord(DDLRecord p_a_record, int p_i_id)
        {
            CheckDDLRecord(p_a_record, p_i_id, false);
        }

        private static void CheckDDLRecord(DDLRecord p_a_record, int p_i_id, bool p_b_changed)
        {
            DateTime? o_dateTime = null;
            DateTime? o_date = null;
            TimeSpan? o_time = null;

            DateTime? o_localDateTime = null;
            DateTime? o_localDate = null;
            TimeSpan? o_localTime = null;

            if (p_i_id == 1)
            {
                o_dateTime = new DateTime(2019, 1, 1, 0, 1, 1);
                o_date = new DateTime(2001, 1, 1);
                o_time = new TimeSpan(1, 1, 1);

                o_localDateTime = new DateTime(2019, 1, 1, 0, 1, 1);
                o_localDate = new DateTime(2001, 1, 1);
                o_localTime = new TimeSpan(1, 1, 1);
            }
            else if (p_i_id == 2)
            {
                o_dateTime = new DateTime(2019, 2, 2, 1, 2, 2);
                o_date = new DateTime(2002, 2, 2);
                o_time = new TimeSpan(2, 2, 2);

                o_localDateTime = new DateTime(2019, 2, 2, 1, 2, 2);
                o_localDate = new DateTime(2002, 2, 2);
                o_localTime = new TimeSpan(2, 2, 2);
            }
            else if (p_i_id == 3)
            {
                o_dateTime = new DateTime(2019, 3, 3, 2, 3, 3);
                o_date = new DateTime(2003, 3, 3);
                o_time = new TimeSpan(3, 3, 3);

                o_localDateTime = new DateTime(2019, 3, 3, 2, 3, 3);
                o_localDate = new DateTime(2003, 3, 3);
                o_localTime = new TimeSpan(3, 3, 3);
            }
            else if (p_i_id == 4)
            {
                o_dateTime = new DateTime(2019, 4, 4, 3, 4, 4);
                o_date = new DateTime(2004, 4, 4);
                o_time = new TimeSpan(4, 4, 4);

                o_localDateTime = new DateTime(2019, 4, 4, 3, 4, 4);
                o_localDate = new DateTime(2004, 4, 4);
                o_localTime = new TimeSpan(4, 4, 4);
            }
            else if (p_i_id == 5)
            {
                o_dateTime = new DateTime(2019, 5, 5, 4, 5, 5);
                o_date = new DateTime(2005, 5, 5);
                o_time = new TimeSpan(5, 5, 5);

                o_localDateTime = new DateTime(2019, 5, 5, 4, 5, 5);
                o_localDate = new DateTime(2005, 5, 5);
                o_localTime = new TimeSpan(5, 5, 5);
            }
            else if (p_i_id == 6)
            {
                o_dateTime = new DateTime(2019, 6, 6, 5, 6, 6);
                o_date = new DateTime(2006, 6, 6);
                o_time = new TimeSpan(6, 6, 6);

                o_localDateTime = new DateTime(2019, 6, 6, 5, 6, 6);
                o_localDate = new DateTime(2006, 6, 6);
                o_localTime = new TimeSpan(6, 6, 6);
            }
            else if (p_i_id == 7)
            {
                /* all null */
            }
            else if (p_i_id == 10)
            {
                o_dateTime = new DateTime(2019, 5, 5, 4, 5, 5);
                o_date = null;
                o_time = new TimeSpan(5, 5, 5);

                o_localDateTime = null;
                o_localDate = null;
                o_localTime = null;
            }
            else if (p_i_id == 11)
            {
                o_dateTime = new DateTime(2019, 1, 1, 0, 1, 1);
                o_date = null;
                o_time = new TimeSpan(1, 1, 1);

                o_localDateTime = null;
                o_localDate = null;
                o_localTime = null;
            }

            if (p_i_id == 1)
            {
                Assert.That(p_a_record.ColumnId, Is.EqualTo(p_i_id), "ColumnId[" + p_a_record.ColumnId + "] is not equal to '" + p_i_id + "'");
                Assert.That(p_a_record.ColumnUUID, Is.EqualTo("32f80a3a-ff6e-4cc0-bb8f-ce630595382b"), "ColumnUUID[" + p_a_record.ColumnUUID + "] is not equal to '32f80a3a-ff6e-4cc0-bb8f-ce630595382b'");
                Assert.That(p_a_record.ColumnShortText, Is.EqualTo("Datensatz Eins"), "ColumnShortText[" + p_a_record.ColumnShortText + "] is not equal to 'Datensatz Eins'");
                Assert.That(p_a_record.ColumnText, Is.EqualTo("Die Handelsstreitigkeiten zwischen den USA und China sorgen für eine Art Umdenken auf beiden Seiten. Während US-Unternehmen chinesische Hardware meiden, tun dies chinesische Unternehmen wohl mittlerweile auch: So denken laut einem Bericht der Nachrichtenagentur Bloomberg viele chinesische Hersteller stark darüber nach, ihre IT-Infrastruktur von lokalen Unternehmen statt von den US-Konzernen Oracle und IBM zu kaufen. Für diese Unternehmen sei der asiatische Markt wichtig. 16 respektive mehr als 20 Prozent des Umsatzes stammen aus dieser Region."), "ColumnText[" + p_a_record.ColumnText + "] is not equal to expected text");
                /* nosqlmdb does not support short or smallint, only int32 and long */
                Assert.That(p_a_record.ColumnSmallInt, Is.EqualTo((ForestNET.Lib.Global.Instance.BaseGateway == BaseGateway.NOSQLMDB) ? 1 : (short)1), "ColumnSmallInt[" + p_a_record.ColumnSmallInt + "] is not equal to '" + 1 + "'");
                Assert.That(p_a_record.ColumnInt, Is.EqualTo(10001), "ColumnInt[" + p_a_record.ColumnInt + "] is not equal to '10001'");
                Assert.That(p_a_record.ColumnBigInt, Is.EqualTo(100001111L), "ColumnBigInt[" + p_a_record.ColumnBigInt + "] is not equal to '100001111'");
                Assert.That(p_a_record.ColumnTimestamp, Is.EqualTo(o_dateTime), "ColumnTimestamp[" + p_a_record.ColumnTimestamp + "] is not equal to '" + o_dateTime + "'");
                Assert.That(p_a_record.ColumnDate, Is.EqualTo(o_date), "ColumnDate[" + p_a_record.ColumnDate + "] is not equal to '" + o_date + "'");
                Assert.That(p_a_record.ColumnTime, Is.EqualTo(o_time), "ColumnTime[" + p_a_record.ColumnTime + "] is not equal to '" + o_time + "'");
                Assert.That(p_a_record.ColumnLocalDateTime, Is.EqualTo(o_localDateTime), "ColumnLocalDateTime[" + p_a_record.ColumnLocalDateTime + "] is not equal to '" + o_localDateTime + "'");
                Assert.That(p_a_record.ColumnLocalDate, Is.EqualTo(o_localDate), "ColumnLocalDate[" + p_a_record.ColumnLocalDate + "] is not equal to '" + o_localDate + "'");
                Assert.That(p_a_record.ColumnLocalTime, Is.EqualTo(o_localTime), "ColumnLocalTime[" + p_a_record.ColumnLocalTime + "] is not equal to '" + o_localTime + "'");
                Assert.That(p_a_record.ColumnDoubleCol, Is.EqualTo(1.23456789d), "ColumnDoubleCol[" + p_a_record.ColumnDoubleCol + "] is not equal to '" + 1.23456789d + "'");
                Assert.That(p_a_record.ColumnDecimal, Is.EqualTo(12345678.90m), "ColumnDecimal[" + p_a_record.ColumnDecimal + "] is not equal to '" + 12345678.90m + "'");
                Assert.That(p_a_record.ColumnBool, Is.EqualTo(true), "ColumnBool[" + p_a_record.ColumnBool + "] is not equal to 'true'");
                Assert.That(p_a_record.ColumnText2, Is.EqualTo("Das ist das Haus vom Nikolaus #1"), "ColumnText2[" + p_a_record.ColumnText2 + "] is not equal to 'Das ist das Haus vom Nikolaus #1'");
                Assert.That(p_a_record.ColumnShortText2, Is.EqualTo("Eins Datensatz"), "ColumnShortText2[" + p_a_record.ColumnShortText2 + "] is not equal to 'Eins Datensatz'");
            }
            else if (p_i_id == 2)
            {
                Assert.That(p_a_record.ColumnId, Is.EqualTo(p_i_id), "ColumnId[" + p_a_record.ColumnId + "] is not equal to '" + p_i_id + "'");
                Assert.That(p_a_record.ColumnUUID, Is.EqualTo("390e413a-09df-41e3-aafe-8a116197da7f"), "ColumnUUID[" + p_a_record.ColumnUUID + "] is not equal to '390e413a-09df-41e3-aafe-8a116197da7f'");
                Assert.That(p_a_record.ColumnShortText, Is.EqualTo((p_b_changed) ? "Datensatz Zwei geändert" : "Datensatz Zwei"), "ColumnShortText[" + p_a_record.ColumnShortText + "] is not equal to 'Datensatz Zwei'");
                Assert.That(p_a_record.ColumnText, Is.EqualTo("Und hier ein single quote '. Und dann noch ein Backslash \\. Das Tech-Startup Pingcap ist eines der lokalen Unternehmen, die den Handelsstreit zu ihrem Vorteil nutzen, für lokale chinesische Produkte werben und selbst von US-Hardware wegmigrieren. Mehr als 300 Kunden betreut die Firma, darunter der Fahrradsharing-Dienst Mobike und der chinesische Smartphone-Hersteller Xiaomi. Piingcap bietet beispielsweise auf Mysql basierende Datenbanken wie TiDB an."), "ColumnText[" + p_a_record.ColumnText + "] is not equal to expected text");
                /* nosqlmdb does not support short or smallint, only int32 and long */
                Assert.That(p_a_record.ColumnSmallInt, Is.EqualTo((ForestNET.Lib.Global.Instance.BaseGateway == BaseGateway.NOSQLMDB) ? 2 : (short)2), "ColumnSmallInt[" + p_a_record.ColumnSmallInt + "] is not equal to '" + 2 + "'");
                Assert.That(p_a_record.ColumnInt, Is.EqualTo(20002), "ColumnInt[" + p_a_record.ColumnInt + "] is not equal to '20002'");
                Assert.That(p_a_record.ColumnBigInt, Is.EqualTo(200002222L), "ColumnBigInt[" + p_a_record.ColumnBigInt + "] is not equal to '200002222'");
                Assert.That(p_a_record.ColumnTimestamp, Is.EqualTo(o_dateTime), "ColumnTimestamp[" + p_a_record.ColumnTimestamp + "] is not equal to '" + o_dateTime + "'");
                Assert.That(p_a_record.ColumnDate, Is.EqualTo(o_date), "ColumnDate[" + p_a_record.ColumnDate + "] is not equal to '" + o_date + "'");
                Assert.That(p_a_record.ColumnTime, Is.EqualTo(o_time), "ColumnTime[" + p_a_record.ColumnTime + "] is not equal to '" + o_time + "'");
                Assert.That(p_a_record.ColumnLocalDateTime, Is.EqualTo(o_localDateTime), "ColumnLocalDateTime[" + p_a_record.ColumnLocalDateTime + "] is not equal to '" + o_localDateTime + "'");
                Assert.That(p_a_record.ColumnLocalDate, Is.EqualTo(o_localDate), "ColumnLocalDate[" + p_a_record.ColumnLocalDate + "] is not equal to '" + o_localDate + "'");
                Assert.That(p_a_record.ColumnLocalTime, Is.EqualTo(o_localTime), "ColumnLocalTime[" + p_a_record.ColumnLocalTime + "] is not equal to '" + o_localTime + "'");
                Assert.That(p_a_record.ColumnDoubleCol, Is.EqualTo(12.3456789d), "ColumnDoubleCol[" + p_a_record.ColumnDoubleCol + "] is not equal to '" + 12.3456789d + "'");
                Assert.That(p_a_record.ColumnDecimal, Is.EqualTo(1234567.890m), "ColumnDecimal[" + p_a_record.ColumnDecimal + "] is not equal to '" + 1234567.890m + "'");
                Assert.That(p_a_record.ColumnBool, Is.EqualTo(false), "ColumnBool[" + p_a_record.ColumnBool + "] is not equal to 'false'");
                Assert.That(p_a_record.ColumnText2, Is.EqualTo("Das ist das Haus vom Nikolaus #2"), "ColumnText2[" + p_a_record.ColumnText2 + "] is not equal to 'Das ist das Haus vom Nikolaus #2'");
                Assert.That(p_a_record.ColumnShortText2, Is.EqualTo("Zwei Datensatz"), "ColumnShortText2[" + p_a_record.ColumnShortText2 + "] is not equal to 'Zwei Datensatz'");
            }
            else if (p_i_id == 3)
            {
                Assert.That(p_a_record.ColumnId, Is.EqualTo(p_i_id), "ColumnId[" + p_a_record.ColumnId + "] is not equal to '" + p_i_id + "'");
                Assert.That(p_a_record.ColumnUUID, Is.EqualTo("ab0f2622-57d4-406e-a866-41e1bc5e4a3a"), "ColumnUUID[" + p_a_record.ColumnUUID + "] is not equal to 'ab0f2622-57d4-406e-a866-41e1bc5e4a3a'");
                Assert.That(p_a_record.ColumnShortText, Is.EqualTo((p_b_changed) ? "Datensatz Drei geändert" : "Datensatz Drei"), "ColumnShortText[" + p_a_record.ColumnShortText + "] is not equal to 'Datensatz Drei'");
                Assert.That(p_a_record.ColumnText, Is.EqualTo("\"Viele Firmen, die auf Oracle und IBM gesetzt haben, dachten es sei noch ein entfernter Meilenstein, diese zu ersetzen\", sagt Pingcap-CEO Huang Dongxu. \"Wir schauen uns aber mittlerweile Plan B ernsthaft an\". Allerdings seien chinesische Unternehmen laut dem lokalen Analystenunternehmen UOB Kay Hian noch nicht ganz bereit, wettbewerbsfähige Chips zu produzieren. \"Wenn sie aber genug gereift sind, werden [viele Unternehmen, Anm. d. Red.] ausländische Chips mit den lokalen ersetzen\", sagt die Firma."), "ColumnText[" + p_a_record.ColumnText + "] is not equal to expected text");
                /* nosqlmdb does not support short or smallint, only int32 and long */
                Assert.That(p_a_record.ColumnSmallInt, Is.EqualTo((ForestNET.Lib.Global.Instance.BaseGateway == BaseGateway.NOSQLMDB) ? 3 : (short)3), "ColumnSmallInt[" + p_a_record.ColumnSmallInt + "] is not equal to '" + 3 + "'");
                Assert.That(p_a_record.ColumnInt, Is.EqualTo(30003), "ColumnInt[" + p_a_record.ColumnInt + "] is not equal to '30003'");
                Assert.That(p_a_record.ColumnBigInt, Is.EqualTo(300003333L), "ColumnBigInt[" + p_a_record.ColumnBigInt + "] is not equal to '300003333'");
                Assert.That(p_a_record.ColumnTimestamp, Is.EqualTo(o_dateTime), "ColumnTimestamp[" + p_a_record.ColumnTimestamp + "] is not equal to '" + o_dateTime + "'");
                Assert.That(p_a_record.ColumnDate, Is.EqualTo(o_date), "ColumnDate[" + p_a_record.ColumnDate + "] is not equal to '" + o_date + "'");
                Assert.That(p_a_record.ColumnTime, Is.EqualTo(o_time), "ColumnTime[" + p_a_record.ColumnTime + "] is not equal to '" + o_time + "'");
                Assert.That(p_a_record.ColumnLocalDateTime, Is.EqualTo(o_localDateTime), "ColumnLocalDateTime[" + p_a_record.ColumnLocalDateTime + "] is not equal to '" + o_localDateTime + "'");
                Assert.That(p_a_record.ColumnLocalDate, Is.EqualTo(o_localDate), "ColumnLocalDate[" + p_a_record.ColumnLocalDate + "] is not equal to '" + o_localDate + "'");
                Assert.That(p_a_record.ColumnLocalTime, Is.EqualTo(o_localTime), "ColumnLocalTime[" + p_a_record.ColumnLocalTime + "] is not equal to '" + o_localTime + "'");
                Assert.That(p_a_record.ColumnDoubleCol, Is.EqualTo(123.456789d), "ColumnDoubleCol[" + p_a_record.ColumnDoubleCol + "] is not equal to '" + 123.456789d + "'");
                Assert.That(p_a_record.ColumnDecimal, Is.EqualTo(123456.7890m), "ColumnDecimal[" + p_a_record.ColumnDecimal + "] is not equal to '" + 123456.7890m + "'");
                Assert.That(p_a_record.ColumnBool, Is.EqualTo(true), "ColumnBool[" + p_a_record.ColumnBool + "] is not equal to 'true'");
                Assert.That(p_a_record.ColumnText2, Is.EqualTo("Das ist das Haus vom Nikolaus #3"), "ColumnText2[" + p_a_record.ColumnText2 + "] is not equal to 'Das ist das Haus vom Nikolaus #3'");
                Assert.That(p_a_record.ColumnShortText2, Is.EqualTo("Drei Datensatz"), "ColumnShortText2[" + p_a_record.ColumnShortText2 + "] is not equal to 'Drei Datensatz'");
            }
            else if (p_i_id == 4)
            {
                Assert.That(p_a_record.ColumnId, Is.EqualTo(p_i_id), "ColumnId[" + p_a_record.ColumnId + "] is not equal to '" + p_i_id + "'");
                Assert.That(p_a_record.ColumnUUID, Is.EqualTo("9df3326e-b061-45e0-a3a1-4691ce0a349e"), "ColumnUUID[" + p_a_record.ColumnUUID + "] is not equal to '9df3326e-b061-45e0-a3a1-4691ce0a349e'");
                Assert.That(p_a_record.ColumnShortText, Is.EqualTo("Datensatz Vier"), "ColumnShortText[" + p_a_record.ColumnShortText + "] is not equal to 'Datensatz Vier'");
                Assert.That(p_a_record.ColumnText, Is.EqualTo("China migriert schneller von US-Hardware auf lokale Chips. Immer mehr chinesische Unternehmen wollen anscheinend von amerikanischen Produkten auf lokal hergestellte Hardware setzen. Davon betroffen sind beispielsweise IBM und Oracle, die einen großen Teil ihres Umsatzes in Asien machen. Noch sei die chinesische Technik aber nicht weit genug."), "ColumnText[" + p_a_record.ColumnText + "] is not equal to expected text");
                /* nosqlmdb does not support short or smallint, only int32 and long */
                Assert.That(p_a_record.ColumnSmallInt, Is.EqualTo((ForestNET.Lib.Global.Instance.BaseGateway == BaseGateway.NOSQLMDB) ? 4 : (short)4), "ColumnSmallInt[" + p_a_record.ColumnSmallInt + "] is not equal to '" + 4 + "'");
                Assert.That(p_a_record.ColumnInt, Is.EqualTo(40004), "ColumnInt[" + p_a_record.ColumnInt + "] is not equal to '40004'");
                Assert.That(p_a_record.ColumnBigInt, Is.EqualTo(400004444L), "ColumnBigInt[" + p_a_record.ColumnBigInt + "] is not equal to '400004444'");
                Assert.That(p_a_record.ColumnTimestamp, Is.EqualTo(o_dateTime), "ColumnTimestamp[" + p_a_record.ColumnTimestamp + "] is not equal to '" + o_dateTime + "'");
                Assert.That(p_a_record.ColumnDate, Is.EqualTo(o_date), "ColumnDate[" + p_a_record.ColumnDate + "] is not equal to '" + o_date + "'");
                Assert.That(p_a_record.ColumnTime, Is.EqualTo(o_time), "ColumnTime[" + p_a_record.ColumnTime + "] is not equal to '" + o_time + "'");
                Assert.That(p_a_record.ColumnLocalDateTime, Is.EqualTo(o_localDateTime), "ColumnLocalDateTime[" + p_a_record.ColumnLocalDateTime + "] is not equal to '" + o_localDateTime + "'");
                Assert.That(p_a_record.ColumnLocalDate, Is.EqualTo(o_localDate), "ColumnLocalDate[" + p_a_record.ColumnLocalDate + "] is not equal to '" + o_localDate + "'");
                Assert.That(p_a_record.ColumnLocalTime, Is.EqualTo(o_localTime), "ColumnLocalTime[" + p_a_record.ColumnLocalTime + "] is not equal to '" + o_localTime + "'");
                Assert.That(p_a_record.ColumnDoubleCol, Is.EqualTo(1234.56789d), "ColumnDoubleCol[" + p_a_record.ColumnDoubleCol + "] is not equal to '" + 1234.56789d + "'");
                Assert.That(p_a_record.ColumnDecimal, Is.EqualTo(12345.67890m), "ColumnDecimal[" + p_a_record.ColumnDecimal + "] is not equal to '" + 12345.67890m + "'");
                Assert.That(p_a_record.ColumnBool, Is.EqualTo(false), "ColumnBool[" + p_a_record.ColumnBool + "] is not equal to 'false'");
                Assert.That(p_a_record.ColumnText2, Is.EqualTo("Das ist das Haus vom Nikolaus #4"), "ColumnText2[" + p_a_record.ColumnText2 + "] is not equal to 'Das ist das Haus vom Nikolaus #4'");
                Assert.That(p_a_record.ColumnShortText2, Is.EqualTo("Vier Datensatz"), "ColumnShortText2[" + p_a_record.ColumnShortText2 + "] is not equal to 'Vier Datensatz'");
            }
            else if (p_i_id == 5)
            {
                Assert.That(p_a_record.ColumnId, Is.EqualTo(p_i_id), "ColumnId[" + p_a_record.ColumnId + "] is not equal to '" + p_i_id + "'");
                Assert.That(p_a_record.ColumnUUID, Is.EqualTo("1b851742-fc49-4fc4-b1c4-0b7cac6ae5af"), "ColumnUUID[" + p_a_record.ColumnUUID + "] is not equal to '1b851742-fc49-4fc4-b1c4-0b7cac6ae5af'");
                Assert.That(p_a_record.ColumnShortText, Is.EqualTo("Datensatz Fünf"), "ColumnShortText[" + p_a_record.ColumnShortText + "] is not equal to 'Datensatz Fünf'");
                Assert.That(p_a_record.ColumnText, Is.EqualTo("Weder IBM noch Oracle haben Bloomberg auf eine Anfrage hin geantwortet. SQL INJECTION:';DELETE FROM items;. Dass die US-Regierung China wirtschaftlich unter Druck setzt, könnte allerdings zu unerwünschten Ergebnissen und dem schnellen Verlust eines Marktes mit fast 1,4 Milliarden Einwohnern führen."), "ColumnText[" + p_a_record.ColumnText + "] is not equal to expected text");
                /* nosqlmdb does not support short or smallint, only int32 and long */
                Assert.That(p_a_record.ColumnSmallInt, Is.EqualTo((ForestNET.Lib.Global.Instance.BaseGateway == BaseGateway.NOSQLMDB) ? 5 : (short)5), "ColumnSmallInt[" + p_a_record.ColumnSmallInt + "] is not equal to '" + 5 + "'");
                Assert.That(p_a_record.ColumnInt, Is.EqualTo(50005), "ColumnInt[" + p_a_record.ColumnInt + "] is not equal to '50005'");
                Assert.That(p_a_record.ColumnBigInt, Is.EqualTo(500005555L), "ColumnBigInt[" + p_a_record.ColumnBigInt + "] is not equal to '500005555'");
                Assert.That(p_a_record.ColumnTimestamp, Is.EqualTo(o_dateTime), "ColumnTimestamp[" + p_a_record.ColumnTimestamp + "] is not equal to '" + o_dateTime + "'");
                Assert.That(p_a_record.ColumnDate, Is.EqualTo(o_date), "ColumnDate[" + p_a_record.ColumnDate + "] is not equal to '" + o_date + "'");
                Assert.That(p_a_record.ColumnTime, Is.EqualTo(o_time), "ColumnTime[" + p_a_record.ColumnTime + "] is not equal to '" + o_time + "'");
                Assert.That(p_a_record.ColumnLocalDateTime, Is.EqualTo(o_localDateTime), "ColumnLocalDateTime[" + p_a_record.ColumnLocalDateTime + "] is not equal to '" + o_localDateTime + "'");
                Assert.That(p_a_record.ColumnLocalDate, Is.EqualTo(o_localDate), "ColumnLocalDate[" + p_a_record.ColumnLocalDate + "] is not equal to '" + o_localDate + "'");
                Assert.That(p_a_record.ColumnLocalTime, Is.EqualTo(o_localTime), "ColumnLocalTime[" + p_a_record.ColumnLocalTime + "] is not equal to '" + o_localTime + "'");
                Assert.That(p_a_record.ColumnDoubleCol, Is.EqualTo(12345.6789d), "ColumnDoubleCol[" + p_a_record.ColumnDoubleCol + "] is not equal to '" + 12345.6789d + "'");
                Assert.That(p_a_record.ColumnDecimal, Is.EqualTo(1234.567890m), "ColumnDecimal[" + p_a_record.ColumnDecimal + "] is not equal to '" + 1234.567890m + "'");
                Assert.That(p_a_record.ColumnBool, Is.EqualTo(true), "ColumnBool[" + p_a_record.ColumnBool + "] is not equal to 'true'");
                Assert.That(p_a_record.ColumnText2, Is.EqualTo("Das ist das Haus vom Nikolaus #5"), "ColumnText2[" + p_a_record.ColumnText2 + "] is not equal to 'Das ist das Haus vom Nikolaus #5'");
                Assert.That(p_a_record.ColumnShortText2, Is.EqualTo("Fünf Datensatz"), "ColumnShortText2[" + p_a_record.ColumnShortText2 + "] is not equal to 'Fünf Datensatz'");
            }
            else if (p_i_id == 6)
            {
                Assert.That(p_a_record.ColumnId, Is.EqualTo(p_i_id), "ColumnId[" + p_a_record.ColumnId + "] is not equal to '" + p_i_id + "'");
                Assert.That(p_a_record.ColumnUUID, Is.EqualTo("6df565d2-30fa-48ab-9f53-83b041f7210e"), "ColumnUUID[" + p_a_record.ColumnUUID + "] is not equal to '6df565d2-30fa-48ab-9f53-83b041f7210e'");
                Assert.That(p_a_record.ColumnShortText, Is.EqualTo("Datensatz Sechs"), "ColumnShortText[" + p_a_record.ColumnShortText + "] is not equal to 'Datensatz Sechs'");
                Assert.That(p_a_record.ColumnText, Is.EqualTo(" Die Berliner Staatsanwaltschaft hat das erste justizielle Rechtshilfeersuchen erst am 6. Dezember der russischen Generalstaatsanwaltschaft äbersandt. Das geht aus einer schriftlichen Anfrage der Linken-Abgeordneten Sevim Dagdelen hervor, die dem ARD-Hauptstadtstudio exklusiv vorliegt. Ein zweites Rechtshilfeersuchen ist demnach am 10. Dezember äbersandt worden. Schon Tage zuvor, nämlich am 4. Dezember, hatte die Bundesrepublik zwei russische Diplomaten ausgewiesen, da die russische Seite die Zusammenarbeit bei der Aufklärung des Mordes verzägert und erschwert habe."), "ColumnText[" + p_a_record.ColumnText + "] is not equal to expected text");
                /* nosqlmdb does not support short or smallint, only int32 and long */
                Assert.That(p_a_record.ColumnSmallInt, Is.EqualTo((ForestNET.Lib.Global.Instance.BaseGateway == BaseGateway.NOSQLMDB) ? 6 : (short)6), "ColumnSmallInt[" + p_a_record.ColumnSmallInt + "] is not equal to '" + 6 + "'");
                Assert.That(p_a_record.ColumnInt, Is.EqualTo(60006), "ColumnInt[" + p_a_record.ColumnInt + "] is not equal to '60006'");
                Assert.That(p_a_record.ColumnBigInt, Is.EqualTo(600006666L), "ColumnBigInt[" + p_a_record.ColumnBigInt + "] is not equal to '600006666'");
                Assert.That(p_a_record.ColumnTimestamp, Is.EqualTo(o_dateTime), "ColumnTimestamp[" + p_a_record.ColumnTimestamp + "] is not equal to '" + o_dateTime + "'");
                Assert.That(p_a_record.ColumnDate, Is.EqualTo(o_date), "ColumnDate[" + p_a_record.ColumnDate + "] is not equal to '" + o_date + "'");
                Assert.That(p_a_record.ColumnTime, Is.EqualTo(o_time), "ColumnTime[" + p_a_record.ColumnTime + "] is not equal to '" + o_time + "'");
                Assert.That(p_a_record.ColumnLocalDateTime, Is.EqualTo(o_localDateTime), "ColumnLocalDateTime[" + p_a_record.ColumnLocalDateTime + "] is not equal to '" + o_localDateTime + "'");
                Assert.That(p_a_record.ColumnLocalDate, Is.EqualTo(o_localDate), "ColumnLocalDate[" + p_a_record.ColumnLocalDate + "] is not equal to '" + o_localDate + "'");
                Assert.That(p_a_record.ColumnLocalTime, Is.EqualTo(o_localTime), "ColumnLocalTime[" + p_a_record.ColumnLocalTime + "] is not equal to '" + o_localTime + "'");
                Assert.That(p_a_record.ColumnDoubleCol, Is.EqualTo(123456.789d), "ColumnDoubleCol[" + p_a_record.ColumnDoubleCol + "] is not equal to '" + 123456.789d + "'");
                Assert.That(p_a_record.ColumnDecimal, Is.EqualTo(123.4567890m), "ColumnDecimal[" + p_a_record.ColumnDecimal + "] is not equal to '" + 123.4567890m + "'");
                Assert.That(p_a_record.ColumnBool, Is.EqualTo(false), "ColumnBool[" + p_a_record.ColumnBool + "] is not equal to 'false'");
                Assert.That(p_a_record.ColumnText2, Is.EqualTo("Das ist das Haus vom Nikolaus #6"), "ColumnText2[" + p_a_record.ColumnText2 + "] is not equal to 'Das ist das Haus vom Nikolaus #6'");
                Assert.That(p_a_record.ColumnShortText2, Is.EqualTo("Sechs Datensatz"), "ColumnShortText2[" + p_a_record.ColumnShortText2 + "] is not equal to 'Sechs Datensatz'");
            }
            else if (p_i_id == 7)
            {
                /* nosqlmdb has id 6, because we deleted 6th record before */
                Assert.That(p_a_record.ColumnId, Is.EqualTo(((ForestNET.Lib.Global.Instance.BaseGateway == BaseGateway.NOSQLMDB) ? 6 : 7)), "ColumnId[" + p_a_record.ColumnId + "] is not equal to '" + ((ForestNET.Lib.Global.Instance.BaseGateway == BaseGateway.NOSQLMDB) ? 6 : 7) + "'");
                Assert.That(p_a_record.ColumnUUID, Is.EqualTo("cf998a07-03c1-4ce1-a063-5f52a940c3a1"), "ColumnUUID[" + p_a_record.ColumnUUID + "] is not equal to '32f80a3a-ff6e-4cc0-bb8f-ce630595382b'");
                Assert.That(p_a_record.ColumnShortText, Is.EqualTo((!p_b_changed) ? null : "short text"), "ColumnShortText[" + p_a_record.ColumnShortText + "] is not equal to '" + ((!p_b_changed) ? null : "short text") + "'");
                Assert.That(p_a_record.ColumnText, Is.EqualTo(null), "ColumnText[" + p_a_record.ColumnText + "] is not equal to expected text");
                /* nosqlmdb does not support short or smallint, only int32 and long */
                Assert.That(p_a_record.ColumnSmallInt, Is.EqualTo((ForestNET.Lib.Global.Instance.BaseGateway == BaseGateway.NOSQLMDB) ? 0 : (short)0), "ColumnSmallInt[" + p_a_record.ColumnSmallInt + "] is not equal to '" + 0 + "'");
                Assert.That(p_a_record.ColumnInt, Is.EqualTo(0), "ColumnInt[" + p_a_record.ColumnInt + "] is not equal to '0'");
                Assert.That(p_a_record.ColumnBigInt, Is.EqualTo(0L), "ColumnBigInt[" + p_a_record.ColumnBigInt + "] is not equal to '0'");
                Assert.That(p_a_record.ColumnTimestamp, Is.EqualTo(null), "ColumnTimestamp[" + p_a_record.ColumnTimestamp + "] is not equal to 'null'");
                Assert.That(p_a_record.ColumnDate, Is.EqualTo(null), "ColumnDate[" + p_a_record.ColumnDate + "] is not equal to 'null'");
                Assert.That(p_a_record.ColumnTime, Is.EqualTo(null), "ColumnTime[" + p_a_record.ColumnTime + "] is not equal to 'null'");
                Assert.That(p_a_record.ColumnLocalDateTime, Is.EqualTo(null), "ColumnLocalDateTime[" + p_a_record.ColumnLocalDateTime + "] is not equal to 'null'");
                Assert.That(p_a_record.ColumnLocalDate, Is.EqualTo(null), "ColumnLocalDate[" + p_a_record.ColumnLocalDate + "] is not equal to 'null'");
                Assert.That(p_a_record.ColumnLocalTime, Is.EqualTo(null), "ColumnLocalTime[" + p_a_record.ColumnLocalTime + "] is not equal to 'null'");
                Assert.That(p_a_record.ColumnDoubleCol, Is.EqualTo(0.0d), "ColumnDoubleCol[" + p_a_record.ColumnDoubleCol + "] is not equal to '" + 0.0d + "'");
                Assert.That(p_a_record.ColumnDecimal, Is.EqualTo(0.0m), "ColumnDecimal[" + p_a_record.ColumnDecimal + "] is not equal to '" + 0.0m + "'");
                Assert.That(p_a_record.ColumnBool, Is.EqualTo(false), "ColumnBool[" + p_a_record.ColumnBool + "] is not equal to 'true'");
                Assert.That(p_a_record.ColumnText2, Is.EqualTo(null), "ColumnText2[" + p_a_record.ColumnText2 + "] is not equal to 'Das ist das Haus vom Nikolaus #1'");
                Assert.That(p_a_record.ColumnShortText2, Is.EqualTo(null), "ColumnShortText2[" + p_a_record.ColumnShortText2 + "] is not equal to 'Eins Datensatz'");
            }
            else if (p_i_id == 10)
            {
                Assert.That(p_a_record.ColumnId, Is.EqualTo(0), "ColumnId[" + p_a_record.ColumnId + "] is not equal to '" + 0 + "'");
                Assert.That(p_a_record.ColumnUUID, Is.EqualTo(null), "ColumnUUID[" + p_a_record.ColumnUUID + "] is not equal to 'null'");
                Assert.That(p_a_record.ColumnShortText, Is.EqualTo("Datensatz Fünf"), "ColumnShortText[" + p_a_record.ColumnShortText + "] is not equal to 'Datensatz Fünf'");
                Assert.That(p_a_record.ColumnText, Is.EqualTo(null), "ColumnText[" + p_a_record.ColumnText + "] is not equal to expected text");
                /* nosqlmdb does not support short or smallint, only int32 and long */
                Assert.That(p_a_record.ColumnSmallInt, Is.EqualTo((ForestNET.Lib.Global.Instance.BaseGateway == BaseGateway.NOSQLMDB) ? 0 : (short)0), "ColumnSmallInt[" + p_a_record.ColumnSmallInt + "] is not equal to '" + 0 + "'");
                Assert.That(p_a_record.ColumnInt, Is.EqualTo(0), "ColumnInt[" + p_a_record.ColumnInt + "] is not equal to '0'");
                Assert.That(p_a_record.ColumnBigInt, Is.EqualTo(500005555L), "ColumnBigInt[" + p_a_record.ColumnBigInt + "] is not equal to '500005555'");
                Assert.That(p_a_record.ColumnTimestamp, Is.EqualTo(o_dateTime), "ColumnTimestamp[" + p_a_record.ColumnTimestamp + "] is not equal to '" + o_dateTime + "'");
                Assert.That(p_a_record.ColumnDate, Is.EqualTo(o_date), "ColumnDate[" + p_a_record.ColumnDate + "] is not equal to '" + o_date + "'");
                Assert.That(p_a_record.ColumnTime, Is.EqualTo(o_time), "ColumnTime[" + p_a_record.ColumnTime + "] is not equal to '" + o_time + "'");
                Assert.That(p_a_record.ColumnLocalDateTime, Is.EqualTo(o_localDateTime), "ColumnLocalDateTime[" + p_a_record.ColumnLocalDateTime + "] is not equal to '" + o_localDateTime + "'");
                Assert.That(p_a_record.ColumnLocalDate, Is.EqualTo(o_localDate), "ColumnLocalDate[" + p_a_record.ColumnLocalDate + "] is not equal to '" + o_localDate + "'");
                Assert.That(p_a_record.ColumnLocalTime, Is.EqualTo(o_localTime), "ColumnLocalTime[" + p_a_record.ColumnLocalTime + "] is not equal to '" + o_localTime + "'");
                Assert.That(p_a_record.ColumnDoubleCol, Is.EqualTo(12345.6789d), "ColumnDoubleCol[" + p_a_record.ColumnDoubleCol + "] is not equal to '" + 12345.6789d + "'");
                Assert.That(p_a_record.ColumnDecimal, Is.EqualTo(1234.567890m), "ColumnDecimal[" + p_a_record.ColumnDecimal + "] is not equal to '" + 1234.567890m + "'");
                Assert.That(p_a_record.ColumnBool, Is.EqualTo(false), "ColumnBool[" + p_a_record.ColumnBool + "] is not equal to 'true'");
                Assert.That(p_a_record.ColumnText2, Is.EqualTo(null), "ColumnText2[" + p_a_record.ColumnText2 + "] is not equal to 'Das ist das Haus vom Nikolaus #1'");
                Assert.That(p_a_record.ColumnShortText2, Is.EqualTo(null), "ColumnShortText2[" + p_a_record.ColumnShortText2 + "] is not equal to 'Eins Datensatz'");
            }
            else if (p_i_id == 11)
            {
                Assert.That(p_a_record.ColumnId, Is.EqualTo(0), "ColumnId[" + p_a_record.ColumnId + "] is not equal to '" + 0 + "'");
                Assert.That(p_a_record.ColumnUUID, Is.EqualTo(null), "ColumnUUID[" + p_a_record.ColumnUUID + "] is not equal to 'null'");
                Assert.That(p_a_record.ColumnShortText, Is.EqualTo("Datensatz Eins"), "ColumnShortText[" + p_a_record.ColumnShortText + "] is not equal to 'Datensatz Eins'");
                Assert.That(p_a_record.ColumnText, Is.EqualTo(null), "ColumnText[" + p_a_record.ColumnText + "] is not equal to expected text");
                /* nosqlmdb does not support short or smallint, only int32 and long */
                Assert.That(p_a_record.ColumnSmallInt, Is.EqualTo((ForestNET.Lib.Global.Instance.BaseGateway == BaseGateway.NOSQLMDB) ? 0 : (short)0), "ColumnSmallInt[" + p_a_record.ColumnSmallInt + "] is not equal to '" + 0 + "'");
                Assert.That(p_a_record.ColumnInt, Is.EqualTo(0), "ColumnInt[" + p_a_record.ColumnInt + "] is not equal to '0'");
                Assert.That(p_a_record.ColumnBigInt, Is.EqualTo(100001111L), "ColumnBigInt[" + p_a_record.ColumnBigInt + "] is not equal to '100001111'");
                Assert.That(p_a_record.ColumnTimestamp, Is.EqualTo(o_dateTime), "ColumnTimestamp[" + p_a_record.ColumnTimestamp + "] is not equal to '" + o_dateTime + "'");
                Assert.That(p_a_record.ColumnDate, Is.EqualTo(o_date), "ColumnDate[" + p_a_record.ColumnDate + "] is not equal to '" + o_date + "'");
                Assert.That(p_a_record.ColumnTime, Is.EqualTo(o_time), "ColumnTime[" + p_a_record.ColumnTime + "] is not equal to '" + o_time + "'");
                Assert.That(p_a_record.ColumnLocalDateTime, Is.EqualTo(o_localDateTime), "ColumnLocalDateTime[" + p_a_record.ColumnLocalDateTime + "] is not equal to '" + o_localDateTime + "'");
                Assert.That(p_a_record.ColumnLocalDate, Is.EqualTo(o_localDate), "ColumnLocalDate[" + p_a_record.ColumnLocalDate + "] is not equal to '" + o_localDate + "'");
                Assert.That(p_a_record.ColumnLocalTime, Is.EqualTo(o_localTime), "ColumnLocalTime[" + p_a_record.ColumnLocalTime + "] is not equal to '" + o_localTime + "'");
                Assert.That(p_a_record.ColumnDoubleCol, Is.EqualTo(1.23456789d), "ColumnDoubleCol[" + p_a_record.ColumnDoubleCol + "] is not equal to '" + 1.23456789d + "'");
                Assert.That(p_a_record.ColumnDecimal, Is.EqualTo(12345678.90m), "ColumnDecimal[" + p_a_record.ColumnDecimal + "] is not equal to '" + 12345678.90m + "'");
                Assert.That(p_a_record.ColumnBool, Is.EqualTo(false), "ColumnBool[" + p_a_record.ColumnBool + "] is not equal to 'true'");
                Assert.That(p_a_record.ColumnText2, Is.EqualTo(null), "ColumnText2[" + p_a_record.ColumnText2 + "] is not equal to 'Das ist das Haus vom Nikolaus #1'");
                Assert.That(p_a_record.ColumnShortText2, Is.EqualTo(null), "ColumnShortText2[" + p_a_record.ColumnShortText2 + "] is not equal to 'Eins Datensatz'");
            }
        }
    }
}
