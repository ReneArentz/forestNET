namespace ForestNET.Tests.IO
{
    public class JSONUnitTest
    {
        [Test]
        public void TestJSON()
        {
            try
            {
                TestConfig.InitiateTestLogging();

                string s_currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'");
                string s_testDirectory = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "testJSON" + ForestNET.Lib.IO.File.DIR;

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

                JsonValidate();
                JsonVirtualFiles();
                JsonObject(s_testDirectory, "_0");
                JsonClass(s_testDirectory, "TestJSONSchemaClass.json", "_1");
                JsonArray(s_testDirectory, "TestJSONSchemaSimpleClassArray.json", "_A");
                JsonArray(s_testDirectory, "TestJSONSchemaSimpleClassObjectMultiReferences.json", "_B");
                JsonArray(s_testDirectory, "TestJSONSchemaSimpleClassObjectOneReference.json", "_C");
                JsonArray(s_testDirectory, "TestJSONSchemaSimpleClassNoReferences.json", "_D");
                JsonComplex(s_testDirectory, true, false, false, false, "_E");
                JsonComplex(s_testDirectory, false, true, false, false, "_F");
                JsonComplex(s_testDirectory, false, false, true, false, "_G");
                JsonComplex(s_testDirectory, false, false, false, true, "_H");
                JsonMultipleUseOfOneSchemaObject(s_testDirectory);

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

        private static void JsonValidate()
        {
            string s_resourcesDirectory = Environment.CurrentDirectory + ForestNET.Lib.IO.File.DIR + "Resources" + ForestNET.Lib.IO.File.DIR;
            string s_jsonSchemaFile = s_resourcesDirectory + "json" + ForestNET.Lib.IO.File.DIR + "TestJSONSchemaClassRootWithRef.json";
            string s_jsonFile = s_resourcesDirectory + "json" + ForestNET.Lib.IO.File.DIR + "ValidateJSON.json";
            string s_invalidJsonFile = s_resourcesDirectory + "json" + ForestNET.Lib.IO.File.DIR + "ValidateJSONIsInvalid.json";

            Assert.That(
                new ForestNET.Lib.IO.JSON(s_jsonSchemaFile).ValidateAgainstSchema(s_jsonFile),
                Is.True,
                "file 'ValidateJSON.json' is not valid with schema 'TestJSONSchemaClassRootWithRef.json'"
            );

            bool b_check = true;

            try
            {
                Assert.That(
                    new ForestNET.Lib.IO.JSON(s_jsonSchemaFile).ValidateAgainstSchema(s_invalidJsonFile),
                    Is.False,
                    "file 'ValidateJSONIsInvalid.json' is valid with schema 'TestJSONSchemaClassRootWithRef.json'"
                );
            }
            catch (Exception)
            {
                b_check = false;
            }

            if (b_check)
            {
                Assert.Fail("file 'ValidateJSONIsInvalid.json' is valid with schema 'TestJSONSchemaClassRootWithRef.json'");
            }
        }

        private static void JsonVirtualFiles()
        {
            ForestNET.Tests.SQL.AllTypesRecord? o_recordOut = new()
            {
                ColumnId = 1,
                ColumnUUID = "ab1824ce-72b8-4aad-bb12-dbc448682437",
                ColumnShortText = "Datensatz Eins",
                ColumnText = "Die Handelsstreitigkeiten zwischen den; \"USA und China\" sorgen für eine Art Umdenken auf beiden Seiten. Während US-Unternehmen chinesische Hardware meiden, tun dies chinesische Unternehmen wohl mittlerweile auch: So denken laut einem Bericht der Nachrichtenagentur Bloomberg viele chinesische Hersteller stark darüber nach, ihre IT-Infrastruktur von lokalen Unternehmen statt von den US-Konzernen Oracle und IBM zu kaufen. Für diese Unternehmen sei der asiatische Markt wichtig. 16 respektive mehr als 20 Prozent des Umsatzes stammen aus dieser Region.",
                ColumnSmallInt = 1,
                ColumnInt = 10001,
                ColumnBigInt = 100001111,
                ColumnLocalDateTime = new DateTime(2019, 2, 2, 2, 2, 2)
            };

            ForestNET.Lib.IO.JSON.JSONElement o_jsonSchema = new("Root")
            {
                Type = "object",
                MappingClass = "ForestNET.Tests.SQL.AllTypesRecord, ForestNET.Tests"
            };

            ForestNET.Lib.IO.JSON.JSONElement o_jsonElement = new("Id")
            {
                Type = "integer",
                MappingClass = "ColumnId",
                Required = true
            };

            o_jsonSchema.Children.Add(o_jsonElement);

            o_jsonElement = new ForestNET.Lib.IO.JSON.JSONElement("UUID")
            {
                Type = "string",
                MappingClass = "ColumnUUID",
                Required = true
            };

            o_jsonSchema.Children.Add(o_jsonElement);

            o_jsonElement = new ForestNET.Lib.IO.JSON.JSONElement("ShortText")
            {
                Type = "string",
                MappingClass = "ColumnShortText",
                Required = true
            };

            o_jsonSchema.Children.Add(o_jsonElement);

            o_jsonElement = new ForestNET.Lib.IO.JSON.JSONElement("Text")
            {
                Type = "string",
                MappingClass = "ColumnText"
            };

            o_jsonSchema.Children.Add(o_jsonElement);

            o_jsonElement = new ForestNET.Lib.IO.JSON.JSONElement("SmallInt")
            {
                Type = "integer",
                MappingClass = "ColumnSmallInt"
            };

            o_jsonSchema.Children.Add(o_jsonElement);

            o_jsonElement = new ForestNET.Lib.IO.JSON.JSONElement("Int")
            {
                Type = "integer",
                MappingClass = "ColumnInt"
            };

            o_jsonSchema.Children.Add(o_jsonElement);

            o_jsonElement = new ForestNET.Lib.IO.JSON.JSONElement("BigInt")
            {
                Type = "integer",
                MappingClass = "ColumnBigInt"
            };

            o_jsonSchema.Children.Add(o_jsonElement);

            o_jsonElement = new ForestNET.Lib.IO.JSON.JSONElement("LocalDateTime")
            {
                Type = "string",
                MappingClass = "ColumnLocalDateTime"
            };

            o_jsonSchema.Children.Add(o_jsonElement);

            ForestNET.Lib.IO.JSON o_json = new(o_jsonSchema);

            string s_jsonEncoded = o_json.JsonEncode(o_recordOut);

            List<string> a_fileLines = [.. s_jsonEncoded.Split(Environment.NewLine)];

            ForestNET.Tests.SQL.AllTypesRecord? o_recordIn = (ForestNET.Tests.SQL.AllTypesRecord?)o_json.JsonDecode(a_fileLines);

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(o_recordOut, o_recordIn, o_json.UseProperties),
                Is.True,
                "output object class is not equal to input object class"
            );

            string s_virtualJsonFile = ""
                + "{" + Environment.NewLine
                + "	\"Id\": 42," + Environment.NewLine
                + "	\"UUID\": \"421824ce-72b8-4aad-bb12-dbc448682437\"," + Environment.NewLine
                + "	\"ShortText\": \"Datensatz 42\"," + Environment.NewLine
                + "	\"Text\": \"Die Handelsstreitigkeiten zwischen den; \\\"42 und 42\\\" sorgen für eine Art Umdenken auf beiden Seiten.\"," + Environment.NewLine
                + "	\"SmallInt\": -42," + Environment.NewLine
                + "	\"Int\": 40002," + Environment.NewLine
                + "	\"BigInt\": 400001112," + Environment.NewLine
                + "	\"LocalDateTime\": \"2042-02-02T01:02:02Z\"" + Environment.NewLine
                + "}" + Environment.NewLine;

            o_recordOut = new ForestNET.Tests.SQL.AllTypesRecord
            {
                ColumnId = 42,
                ColumnUUID = "421824ce-72b8-4aad-bb12-dbc448682437",
                ColumnShortText = "Datensatz 42",
                ColumnText = "Die Handelsstreitigkeiten zwischen den; \"42 und 42\" sorgen für eine Art Umdenken auf beiden Seiten.",
                ColumnSmallInt = -42,
                ColumnInt = 40002,
                ColumnBigInt = 400001112,
                ColumnLocalDateTime = new DateTime(2042, 2, 2, 2, 2, 2)
            };

            a_fileLines = [.. s_virtualJsonFile.Split(Environment.NewLine)];

            o_recordIn = (ForestNET.Tests.SQL.AllTypesRecord?)o_json.JsonDecode(a_fileLines);

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(o_recordOut, o_recordIn, o_json.UseProperties),
                Is.True,
                "output object class is not equal to input object class"
            );
        }

        private static void JsonObject(string p_s_testDirectory, string p_s_fileNameSuffix)
        {
            ForestNET.Tests.SQL.AllTypesRecord? o_recordOut = new()
            {
                ColumnId = 1,
                ColumnUUID = "ab1824ce-72b8-4aad-bb12-dbc448682437",
                ColumnShortText = "Datensatz Eins",
                ColumnText = "Die Handelsstreitigkeiten zwischen den; \"USA und China\" sorgen für eine Art Umdenken auf beiden Seiten. Während US-Unternehmen chinesische Hardware meiden, tun dies chinesische Unternehmen wohl mittlerweile auch: So denken laut einem Bericht der Nachrichtenagentur Bloomberg viele chinesische Hersteller stark darüber nach, ihre IT-Infrastruktur von lokalen Unternehmen statt von den US-Konzernen Oracle und IBM zu kaufen. Für diese Unternehmen sei der asiatische Markt wichtig. 16 respektive mehr als 20 Prozent des Umsatzes stammen aus dieser Region.",
                ColumnSmallInt = 1,
                ColumnInt = 10001,
                ColumnBigInt = 100001111,
                ColumnLocalDateTime = new DateTime(2019, 2, 2, 2, 2, 2)
            };


            ForestNET.Lib.IO.JSON.JSONElement o_jsonSchema = new("Root")
            {
                Type = "object",
                MappingClass = "ForestNET.Tests.SQL.AllTypesRecord, ForestNET.Tests"
            };

            ForestNET.Lib.IO.JSON.JSONElement o_jsonElement = new("Id")
            {
                Type = "integer",
                MappingClass = "ColumnId",
                Required = true
            };

            o_jsonSchema.Children.Add(o_jsonElement);

            o_jsonElement = new ForestNET.Lib.IO.JSON.JSONElement("UUID")
            {
                Type = "string",
                MappingClass = "ColumnUUID",
                Required = true
            };

            o_jsonSchema.Children.Add(o_jsonElement);

            o_jsonElement = new ForestNET.Lib.IO.JSON.JSONElement("ShortText")
            {
                Type = "string",
                MappingClass = "ColumnShortText",
                Required = true
            };

            o_jsonSchema.Children.Add(o_jsonElement);

            o_jsonElement = new ForestNET.Lib.IO.JSON.JSONElement("Text")
            {
                Type = "string",
                MappingClass = "ColumnText"
            };

            o_jsonSchema.Children.Add(o_jsonElement);

            o_jsonElement = new ForestNET.Lib.IO.JSON.JSONElement("SmallInt")
            {
                Type = "integer",
                MappingClass = "ColumnSmallInt"
            };

            o_jsonSchema.Children.Add(o_jsonElement);

            o_jsonElement = new ForestNET.Lib.IO.JSON.JSONElement("Int")
            {
                Type = "integer",
                MappingClass = "ColumnInt"
            };

            o_jsonSchema.Children.Add(o_jsonElement);

            o_jsonElement = new ForestNET.Lib.IO.JSON.JSONElement("BigInt")
            {
                Type = "integer",
                MappingClass = "ColumnBigInt"
            };

            o_jsonSchema.Children.Add(o_jsonElement);

            o_jsonElement = new ForestNET.Lib.IO.JSON.JSONElement("LocalDateTime")
            {
                Type = "string",
                MappingClass = "ColumnLocalDateTime"
            };

            o_jsonSchema.Children.Add(o_jsonElement);

            ForestNET.Lib.IO.JSON o_json = new(o_jsonSchema);

            string s_fileSimpleClass = p_s_testDirectory + "TestJSONObject" + p_s_fileNameSuffix + ".json";

            _ = o_json.JsonEncode(o_recordOut, s_fileSimpleClass, true);

            ForestNET.Tests.SQL.AllTypesRecord? o_recordIn = (ForestNET.Tests.SQL.AllTypesRecord?)o_json.JsonDecode(s_fileSimpleClass);

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(o_recordOut, o_recordIn, o_json.UseProperties),
                Is.True,
                "output object class is not equal to input object class"
            );
        }

        private static void JsonClass(string p_s_testDirectory, string p_s_jsonSchemaFileName, string p_s_fileNameSuffix)
        {
            ForestNET.Tests.SQL.AllTypesRecord? o_recordOut = new()
            {
                ColumnId = 1,
                ColumnUUID = "ab1824ce-72b8-4aad-bb12-dbc448682437",
                ColumnShortText = "Datensatz Eins",
                ColumnText = "Die Handelsstreitigkeiten zwischen den; \"USA und China\" sorgen für eine Art Umdenken auf beiden Seiten. Während US-Unternehmen chinesische Hardware meiden, tun dies chinesische Unternehmen wohl mittlerweile auch: So denken laut einem Bericht der Nachrichtenagentur Bloomberg viele chinesische Hersteller stark darüber nach, ihre IT-Infrastruktur von lokalen Unternehmen statt von den US-Konzernen Oracle und IBM zu kaufen. Für diese Unternehmen sei der asiatische Markt wichtig. 16 respektive mehr als 20 Prozent des Umsatzes stammen aus dieser Region.",
                ColumnSmallInt = 1,
                ColumnInt = 10001,
                ColumnBigInt = 100001111,
                ColumnTimestamp = Convert.ToDateTime("01.01.2019 01:01:01"),
                ColumnDate = Convert.ToDateTime("01.01.2001"),
                ColumnTime = TimeSpan.Parse("01:01:01"),
                ColumnLocalTime = new TimeSpan(2, 2, 2),
                ColumnLocalDate = new DateTime(2002, 2, 2),
                ColumnLocalDateTime = new DateTime(2019, 2, 2, 2, 2, 2),
                ColumnDoubleCol = 1.23456789d,
                ColumnDecimal = 12345678.9m,
                ColumnBool = true,
                ColumnText2 = "Das ist das Haus vom Nikolaus* #1",
                ColumnShortText2 = "Eins Datensatz"
            };

            string s_resourcesDirectory = Environment.CurrentDirectory + ForestNET.Lib.IO.File.DIR + "Resources" + ForestNET.Lib.IO.File.DIR;
            string s_jsonSchemaFile = s_resourcesDirectory + "json" + ForestNET.Lib.IO.File.DIR + p_s_jsonSchemaFileName;

            ForestNET.Lib.IO.JSON o_json = new(s_jsonSchemaFile);

            string s_fileSimpleClass = p_s_testDirectory + "TestJSONClass" + p_s_fileNameSuffix + ".json";

            _ = o_json.JsonEncode(o_recordOut, s_fileSimpleClass, true);

            ForestNET.Tests.SQL.AllTypesRecord? o_recordIn = (ForestNET.Tests.SQL.AllTypesRecord?)o_json.JsonDecode(s_fileSimpleClass);

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(o_recordOut, o_recordIn, o_json.UseProperties),
                Is.True,
                "output object class is not equal to input object class"
            );
        }

        private static void JsonArray(string p_s_testDirectory, string p_s_jsonSchemaFileName, string p_s_fileNameSuffix)
        {
            List<ForestNET.Tests.IO.Data.SimpleClass>? a_dataOut =
            [
                new("Record #1 Value A", "Record #1 Value B", "Record #1 Value C"),
                new("Record #2 Value A", "Record #2 Value B", "", [1, 2, -3, -4]),
                new("Record #3; Value A", "null", "Record #3 Value C", [9, 8, -7, -6], new float[] { 42.0f, 21.25f, 54987.456999f }),
                new("Record 4 Value A", "Record $4 ;Value B \"", null, [16, 32, int.MaxValue, 128, 0], new float[] { 21.0f, 10.625f })
            ];

            ForestNET.Tests.IO.Data.SimpleClassCollection? o_collectionOut = new(a_dataOut);

            string[] a_out = new string[a_dataOut.Count];
            string[] a_in = new string[a_dataOut.Count];
            int i_cnt = 0;

            foreach (ForestNET.Tests.IO.Data.SimpleClass o_simpleClassObject in a_dataOut)
            {
                a_out[i_cnt++] = o_simpleClassObject.ToString();
            }

            string s_resourcesDirectory = Environment.CurrentDirectory + ForestNET.Lib.IO.File.DIR + "Resources" + ForestNET.Lib.IO.File.DIR;
            string s_jsonSchemaFile = s_resourcesDirectory + "json" + ForestNET.Lib.IO.File.DIR + p_s_jsonSchemaFileName;
            ForestNET.Lib.IO.JSON o_json = new(s_jsonSchemaFile)
            {
                UseProperties = true
            };

            string s_fileSimpleClass = p_s_testDirectory + "TestJSONSimpleClass" + p_s_fileNameSuffix + ".json";

            if (p_s_jsonSchemaFileName.Equals("TestJSONSchemaSimpleClassArray.json"))
            {
                _ = o_json.JsonEncode(a_dataOut, s_fileSimpleClass, true);
            }
            else
            {
                _ = o_json.JsonEncode(o_collectionOut, s_fileSimpleClass, true);
            }

            ForestNET.Tests.IO.Data.SimpleClassCollection? o_collectionIn = null;
            List<ForestNET.Tests.IO.Data.SimpleClass>? a_dataIn;

            if (p_s_jsonSchemaFileName.Equals("TestJSONSchemaSimpleClassArray.json"))
            {
                List<ForestNET.Tests.IO.Data.SimpleClass>? a_foo = [.. ((System.Collections.IEnumerable)(o_json.JsonDecode(s_fileSimpleClass) ?? new List<ForestNET.Tests.IO.Data.SimpleClass>())).Cast<ForestNET.Tests.IO.Data.SimpleClass>()];
                a_dataIn = a_foo;
            }
            else
            {
                o_collectionIn = (ForestNET.Tests.IO.Data.SimpleClassCollection?)o_json.JsonDecode(s_fileSimpleClass);
                a_dataIn = o_collectionIn?.SimpleClasses;
            }

            /* float precision is really bad */
            if (a_dataOut[2].ValueE != null)
            {
                float[] a_foo = a_dataOut[2].ValueE ?? [];

                if (a_foo.Length > 2)
                {
                    a_foo[2] = 54987.46f;
                }

                a_out[2] = a_dataOut[2].ToString();
            }

            i_cnt = 0;

            if (a_dataIn != null)
            {
                foreach (ForestNET.Tests.IO.Data.SimpleClass o_simpleClassObject in a_dataIn)
                {
                    a_in[i_cnt++] = o_simpleClassObject.ToString();
                }
            }

            if (!p_s_jsonSchemaFileName.Equals("TestJSONSchemaSimpleClassArray.json"))
            {
                Assert.That(
                    ForestNET.Lib.Helper.ObjectsEqualUsingReflections(o_collectionOut, o_collectionIn, o_json.UseProperties),
                    Is.True,
                    "output object inner class collection is not equal to input object inner class collection"
                );
            }

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_dataOut, a_dataIn, o_json.UseProperties),
                Is.True,
                "output object inner class array is not equal to input object inner class array"
            );

            for (int i = 0; i < a_dataOut.Count; i++)
            {
                Assert.That(
                    ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_dataOut[i], a_dataIn?[i], o_json.UseProperties, false, true),
                    Is.True,
                    "output object inner class array element object is not equal to input object inner class array element object"
                );

                Assert.That(
                    a_out[i], Has.Length.EqualTo(a_in[i].Length),
                    "output object inner class array element string length is not equal to input object inner class array element string length"
                );
            }
        }

        private static void JsonComplex(string p_s_testDirectory, bool p_b_classRoot, bool p_b_listRoot, bool p_b_classRootWithRef, bool p_b_listRootWithRef, string p_s_fileNameSuffix)
        {
            List<ForestNET.Tests.IO.Data.ShipOrder> a_shipOrdersOut = ForestNET.Tests.IO.Data.GenerateData();

            ForestNET.Tests.IO.Data.ShipOrderCollection o_shipOrderCollectionOut = new()
            {
                ShipOrders = a_shipOrdersOut,
                OrderAmount = a_shipOrdersOut.Count
            };

            string[] a_out = new string[a_shipOrdersOut.Count];
            string[] a_in = new string[a_shipOrdersOut.Count];
            int i_cnt = 0;

            foreach (ForestNET.Tests.IO.Data.ShipOrder o_shipOrderObject in a_shipOrdersOut)
            {
                a_out[i_cnt++] = o_shipOrderObject.ToString();
            }

            string s_jsonSchemaFile = "";

            string s_resourcesDirectory = Environment.CurrentDirectory + ForestNET.Lib.IO.File.DIR + "Resources" + ForestNET.Lib.IO.File.DIR;

            if (p_b_classRoot)
            {
                s_jsonSchemaFile = s_resourcesDirectory + "json" + ForestNET.Lib.IO.File.DIR + "TestJSONSchemaClassRoot.json";
            }
            else if (p_b_listRoot)
            {
                s_jsonSchemaFile = s_resourcesDirectory + "json" + ForestNET.Lib.IO.File.DIR + "TestJSONSchemaListRoot.json";
            }
            else if (p_b_classRootWithRef)
            {
                s_jsonSchemaFile = s_resourcesDirectory + "json" + ForestNET.Lib.IO.File.DIR + "TestJSONSchemaClassRootWithRef.json";
            }
            else if (p_b_listRootWithRef)
            {
                s_jsonSchemaFile = s_resourcesDirectory + "json" + ForestNET.Lib.IO.File.DIR + "TestJSONSchemaListRootWithRef.json";
            }

            ForestNET.Lib.IO.JSON o_json = new(s_jsonSchemaFile)
            {
                UseProperties = true
            };

            string s_file = p_s_testDirectory + "TestJSON" + p_s_fileNameSuffix + ".json";

            if ((p_b_classRoot) || (p_b_classRootWithRef))
            {
                _ = o_json.JsonEncode(o_shipOrderCollectionOut, s_file, true);
            }
            else
            {
                _ = o_json.JsonEncode(a_shipOrdersOut, s_file, true);
            }

            List<ForestNET.Tests.IO.Data.ShipOrder>? a_shipOrdersIn = null;
            ForestNET.Tests.IO.Data.ShipOrderCollection? o_shipOrderCollectionIn = null;

            if ((p_b_classRoot) || (p_b_classRootWithRef))
            {
                o_shipOrderCollectionIn = (ForestNET.Tests.IO.Data.ShipOrderCollection?)o_json.JsonDecode(s_file);

                if (o_shipOrderCollectionIn != null)
                {
                    a_shipOrdersIn = o_shipOrderCollectionIn.ShipOrders;
                    o_shipOrderCollectionIn.ShipOrders = a_shipOrdersIn;
                    o_shipOrderCollectionIn.OrderAmount = a_shipOrdersIn.Count;
                }
            }
            else
            {
                List<ForestNET.Tests.IO.Data.ShipOrder> a_foo = [.. ((System.Collections.IEnumerable)(o_json.JsonDecode(s_file) ?? new List<ForestNET.Tests.IO.Data.ShipOrder>())).Cast<ForestNET.Tests.IO.Data.ShipOrder>()];
                a_shipOrdersIn = a_foo;
            }

            i_cnt = 0;

            if (a_shipOrdersIn != null)
            {
                foreach (ForestNET.Tests.IO.Data.ShipOrder o_shipOrderObject in a_shipOrdersIn)
                {
                    a_in[i_cnt++] = o_shipOrderObject.ToString();
                }
            }

            if ((p_b_classRoot) || (p_b_classRootWithRef))
            {
                Assert.That(
                    ForestNET.Lib.Helper.ObjectsEqualUsingReflections(o_shipOrderCollectionOut, o_shipOrderCollectionIn, o_json.UseProperties),
                    Is.True,
                    "output object inner class collection is not equal to input object inner class collection"
                );
            }

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_shipOrdersOut, a_shipOrdersIn, o_json.UseProperties),
                Is.True,
                "output object inner class array is not equal to input object inner class array"
            );

            if (a_shipOrdersIn != null)
            {
                for (int i = 0; i < a_shipOrdersIn.Count; i++)
                {
                    Assert.That(
                        ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_shipOrdersOut[i], a_shipOrdersIn[i], o_json.UseProperties, false, true),
                        Is.True,
                        "output object inner class array element object is not equal to input object inner class array element object"
                    );

                    /* we must delete unnecessary zeroes at the end of numeric values */
                    a_in[i] = DeleteUnnecessaryZeroes(a_in[i]);

                    Assert.That(
                        a_out[i], Has.Length.EqualTo(a_in[i].Length),
                        "output object inner class array element string is not equal to input object inner class array element string"
                    );
                }
            }

            if (p_b_listRootWithRef)
            {
                s_file = s_resourcesDirectory + "json" + ForestNET.Lib.IO.File.DIR + "TestJSONPrimitiveArrayOneLine.json";
                a_in = new string[a_shipOrdersOut.Count];

                List<ForestNET.Tests.IO.Data.ShipOrder> a_foo = [.. ((System.Collections.IEnumerable)(o_json.JsonDecode(s_file) ?? new List<ForestNET.Tests.IO.Data.ShipOrder>())).Cast<ForestNET.Tests.IO.Data.ShipOrder>()];
                a_shipOrdersIn = a_foo;

                i_cnt = 0;

                foreach (ForestNET.Tests.IO.Data.ShipOrder o_shipOrderObject in a_shipOrdersIn)
                {
                    a_in[i_cnt++] = o_shipOrderObject.ToString();
                }

                Assert.That(
                    ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_shipOrdersOut, a_shipOrdersIn, o_json.UseProperties),
                    Is.True,
                    "output object inner class array is not equal to input object inner class array"
                );

                for (int i = 0; i < a_shipOrdersIn.Count; i++)
                {
                    Assert.That(
                        ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_shipOrdersOut[i], a_shipOrdersIn[i], o_json.UseProperties, false, true),
                        Is.True,
                        "output object inner class array element object is not equal to input object inner class array element object"
                    );

                    /* we must delete unnecessary zeroes at the end of numeric values */
                    a_in[i] = DeleteUnnecessaryZeroes(a_in[i]);

                    Assert.That(
                        a_out[i], Has.Length.EqualTo(a_in[i].Length),
                        "output object inner class array element string is not equal to input object inner class array element string"
                    );
                }
            }
        }

        private static void JsonMultipleUseOfOneSchemaObject(string p_s_testDirectory)
        {

            /* Simple #1 */

            List<ForestNET.Tests.IO.Data.SimpleClass> a_dataOut =
            [
                new("Record #1 Value A", "Record #1 Value B", "Record #1 Value C"),
                new("Record #2 Value A", "Record #2 Value B", "", [1, 2, -3, -4]),
                new("Record #3; Value A", "null", "Record #3 Value C", [9, 8, -7, -6]),
                new("Record #4 Value A", "Record $4 ;Value B \"", null, [16, 32, int.MaxValue, 128, 0])
            ];

            ForestNET.Tests.IO.Data.SimpleClassCollection o_collectionOut = new(a_dataOut);

            string[] a_out = new string[a_dataOut.Count];
            string[] a_in = new string[a_dataOut.Count];
            int i_cnt = 0;

            foreach (Data.SimpleClass o_simpleClassObject in a_dataOut)
            {
                a_out[i_cnt++] = o_simpleClassObject.ToString();
            }

            string s_resourcesDirectory = Environment.CurrentDirectory + ForestNET.Lib.IO.File.DIR + "Resources" + ForestNET.Lib.IO.File.DIR;
            string s_jsonSchemaFile = s_resourcesDirectory + "json" + ForestNET.Lib.IO.File.DIR + "TestJSONSchemaSimpleClassObjectMultiReferences.json";
            ForestNET.Lib.IO.JSON o_json = new(s_jsonSchemaFile)
            {
                UseProperties = true
            };

            string s_fileSimpleClass = p_s_testDirectory + "TestJSONSimpleClass_I.json";
            _ = o_json.JsonEncode(o_collectionOut, s_fileSimpleClass, true);

            ForestNET.Tests.IO.Data.SimpleClassCollection? o_collectionIn;
            List<ForestNET.Tests.IO.Data.SimpleClass>? a_dataIn;

            o_collectionIn = (ForestNET.Tests.IO.Data.SimpleClassCollection?)o_json.JsonDecode(s_fileSimpleClass);
            a_dataIn = o_collectionIn?.SimpleClasses;

            i_cnt = 0;

            if (a_dataIn != null)
            {
                foreach (ForestNET.Tests.IO.Data.SimpleClass o_simpleClassObject in a_dataIn)
                {
                    a_in[i_cnt++] = o_simpleClassObject.ToString();
                }
            }

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(o_collectionOut, o_collectionIn, o_json.UseProperties),
                Is.True,
                "output object inner class collection is not equal to input object inner class collection"
            );

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_dataOut, a_dataIn, o_json.UseProperties),
                Is.True,
                "output object inner class array is not equal to input object inner class array"
            );

            for (int i = 0; i < a_dataOut.Count; i++)
            {
                Assert.That(
                    ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_dataOut[i], a_dataIn?[i], o_json.UseProperties, false, true),
                    Is.True,
                    "output object inner class array element object is not equal to input object inner class array element object"
                );

                Assert.That(
                    a_out[i], Has.Length.EqualTo(a_in[i].Length),
                    "output object inner class array element toString is not equal to input object inner class array element toString"
                );
            }

            /* Simple #2 */

            a_dataOut =
            [
                new("Record #1 Value A", "Record #1 Value B", "Record #1 Value C"),
                new("Record #4 Value A", "Record $4 ;Value B \"", null, [16, 32, int.MaxValue, 128, 0]),
                new("Record #3; Value A", "null", "Record #3 Value C", [9, 8, -7, -6]),
                new("Record #2 Value A", "Record #2 Value B", "", [1, 2, -3, -4])
            ];

            o_collectionOut = new ForestNET.Tests.IO.Data.SimpleClassCollection(a_dataOut);

            a_out = new string[a_dataOut.Count];
            a_in = new string[a_dataOut.Count];
            i_cnt = 0;

            foreach (ForestNET.Tests.IO.Data.SimpleClass o_simpleClassObject in a_dataOut)
            {
                a_out[i_cnt++] = o_simpleClassObject.ToString();
            }

            s_fileSimpleClass = p_s_testDirectory + "TestJSONSimpleClass_J.json";
            _ = o_json.JsonEncode(o_collectionOut, s_fileSimpleClass, true);

            o_collectionIn = (ForestNET.Tests.IO.Data.SimpleClassCollection?)o_json.JsonDecode(s_fileSimpleClass);
            a_dataIn = o_collectionIn?.SimpleClasses;

            i_cnt = 0;

            if (a_dataIn != null)
            {
                foreach (ForestNET.Tests.IO.Data.SimpleClass o_simpleClassObject in a_dataIn)
                {
                    a_in[i_cnt++] = o_simpleClassObject.ToString();
                }
            }

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(o_collectionOut, o_collectionIn, o_json.UseProperties),
                Is.True,
                "output object inner class collection is not equal to input object inner class collection"
            );

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_dataOut, a_dataIn, o_json.UseProperties),
                Is.True,
                "output object inner class array is not equal to input object inner class array"
            );

            for (int i = 0; i < a_dataOut.Count; i++)
            {
                Assert.That(
                    ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_dataOut[i], a_dataIn?[i], o_json.UseProperties, false, true),
                    Is.True,
                    "output object inner class array element object is not equal to input object inner class array element object"
                );

                Assert.That(
                    a_out[i], Has.Length.EqualTo(a_in[i].Length),
                    "output object inner class array element toString is not equal to input object inner class array element toString"
                );
            }

            /* Complex #1 */

            List<ForestNET.Tests.IO.Data.ShipOrder> a_shipOrdersOut = ForestNET.Tests.IO.Data.GenerateData();

            ForestNET.Tests.IO.Data.ShipOrderCollection o_shipOrderCollectionOut = new()
            {
                ShipOrders = a_shipOrdersOut,
                OrderAmount = a_shipOrdersOut.Count
            };

            a_out = new string[a_shipOrdersOut.Count];
            a_in = new string[a_shipOrdersOut.Count];
            i_cnt = 0;

            foreach (ForestNET.Tests.IO.Data.ShipOrder o_shipOrderObject in a_shipOrdersOut)
            {
                a_out[i_cnt++] = o_shipOrderObject.ToString();
            }

            s_jsonSchemaFile = s_resourcesDirectory + "json" + ForestNET.Lib.IO.File.DIR + "TestJSONSchemaClassRootWithRef.json";

            o_json = new ForestNET.Lib.IO.JSON(s_jsonSchemaFile)
            {
                UseProperties = true
            };

            string s_file = p_s_testDirectory + "TestJSON_K.json";
            _ = o_json.JsonEncode(o_shipOrderCollectionOut, s_file, true);

            List<ForestNET.Tests.IO.Data.ShipOrder>? a_shipOrdersIn = null;
            ForestNET.Tests.IO.Data.ShipOrderCollection? o_shipOrderCollectionIn;

            o_shipOrderCollectionIn = (ForestNET.Tests.IO.Data.ShipOrderCollection?)o_json.JsonDecode(s_file);

            if (o_shipOrderCollectionIn != null)
            {
                a_shipOrdersIn = o_shipOrderCollectionIn.ShipOrders;
                o_shipOrderCollectionIn.ShipOrders = a_shipOrdersIn;
                o_shipOrderCollectionIn.OrderAmount = a_shipOrdersIn.Count;
            }

            i_cnt = 0;

            if (a_shipOrdersIn != null)
            {
                foreach (ForestNET.Tests.IO.Data.ShipOrder o_shipOrderObject in a_shipOrdersIn)
                {
                    a_in[i_cnt++] = o_shipOrderObject.ToString();
                }
            }

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(o_shipOrderCollectionOut, o_shipOrderCollectionIn, o_json.UseProperties),
                Is.True,
                "output object inner class collection is not equal to input object inner class collection"
            );

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_shipOrdersOut, a_shipOrdersIn, o_json.UseProperties),
                Is.True,
                "output object inner class array is not equal to input object inner class array"
            );

            if (a_shipOrdersIn != null)
            {
                for (int i = 0; i < a_shipOrdersIn.Count; i++)
                {
                    Assert.That(
                        ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_shipOrdersOut[i], a_shipOrdersIn[i], o_json.UseProperties, false, true),
                        Is.True,
                        "output object inner class array element object is not equal to input object inner class array element object"
                    );

                    /* we must delete unnecessary zeroes at the end of numeric values */
                    a_in[i] = DeleteUnnecessaryZeroes(a_in[i]);

                    Assert.That(
                        a_out[i], Has.Length.EqualTo(a_in[i].Length),
                        "output object inner class array element string is not equal to input object inner class array element string"
                    );
                }
            }

            /* Complex #2 */

            a_shipOrdersOut = ForestNET.Tests.IO.Data.GenerateData();

            foreach (ForestNET.Tests.IO.Data.ShipOrder o_shipOrderObject in a_shipOrdersOut)
            {
                o_shipOrderObject.OrderId += " #2";

                foreach (ForestNET.Tests.IO.Data.ShipItem o_shipItemObject in o_shipOrderObject.ShipItems)
                {
                    o_shipItemObject.Note += " #2";
                }
            }

            o_shipOrderCollectionOut = new()
            {
                ShipOrders = a_shipOrdersOut,
                OrderAmount = a_shipOrdersOut.Count
            };

            a_out = new string[a_shipOrdersOut.Count];
            a_in = new string[a_shipOrdersOut.Count];
            i_cnt = 0;

            foreach (ForestNET.Tests.IO.Data.ShipOrder o_shipOrderObject in a_shipOrdersOut)
            {
                a_out[i_cnt++] = o_shipOrderObject.ToString();
            }

            s_file = p_s_testDirectory + "TestJSON_L.json";
            _ = o_json.JsonEncode(o_shipOrderCollectionOut, s_file, true);

            o_shipOrderCollectionIn = (ForestNET.Tests.IO.Data.ShipOrderCollection?)o_json.JsonDecode(s_file);

            if (o_shipOrderCollectionIn != null)
            {
                a_shipOrdersIn = o_shipOrderCollectionIn.ShipOrders;
                o_shipOrderCollectionIn.ShipOrders = a_shipOrdersIn;
                o_shipOrderCollectionIn.OrderAmount = a_shipOrdersIn.Count;
            }

            i_cnt = 0;

            if (a_shipOrdersIn != null)
            {
                foreach (ForestNET.Tests.IO.Data.ShipOrder o_shipOrderObject in a_shipOrdersIn)
                {
                    a_in[i_cnt++] = o_shipOrderObject.ToString();
                }
            }

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(o_shipOrderCollectionOut, o_shipOrderCollectionIn, o_json.UseProperties),
                Is.True,
                "output object inner class collection is not equal to input object inner class collection"
            );

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_shipOrdersOut, a_shipOrdersIn, o_json.UseProperties),
                Is.True,
                "output object inner class array is not equal to input object inner class array"
            );

            if (a_shipOrdersIn != null)
            {
                for (int i = 0; i < a_shipOrdersIn.Count; i++)
                {
                    Assert.That(
                        ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_shipOrdersOut[i], a_shipOrdersIn[i], o_json.UseProperties, false, true),
                        Is.True,
                        "output object inner class array element object is not equal to input object inner class array element object"
                    );

                    /* we must delete unnecessary zeroes at the end of numeric values */
                    a_in[i] = DeleteUnnecessaryZeroes(a_in[i]);

                    Assert.That(
                        a_out[i], Has.Length.EqualTo(a_in[i].Length),
                        "output object inner class array element string is not equal to input object inner class array element string"
                    );
                }
            }
        }

        private static string DeleteUnnecessaryZeroes(string p_s_input)
        {
            string s_foo = "";

            bool b_comma = false;

            for (int i = 0; i < p_s_input.Length; i++)
            {
                char c_foo = p_s_input[i];

                if ((c_foo == '0') && (b_comma))
                {
                    if ((p_s_input[i - 2] == ',') && ((p_s_input[i + 1] == ',') || (p_s_input[i + 1] == '|') || (p_s_input[i + 1] == ']')))
                    {
                        s_foo += p_s_input[i];
                        b_comma = false;
                    }
                    else
                    {
                        int i_old = i;

                        while (i < p_s_input.Length - 1)
                        {
                            if (p_s_input[++i] == '0')
                            {
                                continue;
                            }
                            else if ((p_s_input[i] == ',') || (p_s_input[i] == '|') || (p_s_input[i] == ']'))
                            {
                                if (s_foo[s_foo.Length - 2] == ',')
                                    s_foo += "0";

                                s_foo += p_s_input[i];

                                if (s_foo.EndsWith(",,"))
                                {
                                    s_foo = s_foo.Substring(0, s_foo.Length - 1);
                                    s_foo += "00,";

                                    if ((s_foo.EndsWith(" 0,00,")) || (s_foo.EndsWith("[0,00,")))
                                    {
                                        s_foo = s_foo.Substring(0, s_foo.Length - 4);
                                        s_foo += ",";
                                    }
                                }
                                else if (s_foo.EndsWith(",|"))
                                {
                                    s_foo = s_foo.Substring(0, s_foo.Length - 1);
                                    s_foo += "00|";

                                    if ((s_foo.EndsWith(" 0,00|")) || (s_foo.EndsWith("[0,00|")))
                                    {
                                        s_foo = s_foo.Substring(0, s_foo.Length - 4);
                                        s_foo += "|";
                                    }
                                }
                                else if (s_foo.EndsWith(",]"))
                                {
                                    s_foo = s_foo.Substring(0, s_foo.Length - 1);
                                    s_foo += "00]";

                                    if ((s_foo.EndsWith(" 0,00]")) || (s_foo.EndsWith("[0,00]")))
                                    {
                                        s_foo = s_foo.Substring(0, s_foo.Length - 4);
                                        s_foo += "]";
                                    }
                                }

                                b_comma = false;
                                break;
                            }
                            else
                            {
                                for (int j = i_old; j < i; j++)
                                {
                                    s_foo += p_s_input[j];
                                }

                                i_old = i;

                                if ((i < p_s_input.Length - 1) && (p_s_input[i + 1] == '0'))
                                {
                                    s_foo += p_s_input[i];
                                    i_old++;
                                }
                            }
                        }
                    }
                }
                else if ((c_foo == ',') && (!b_comma) && (p_s_input[i + 1] != ' '))
                {
                    b_comma = true;
                    s_foo += c_foo;
                }
                else if (((p_s_input[i] == ',') || (p_s_input[i] == '|') || (p_s_input[i] == ']')) && (b_comma))
                {
                    b_comma = false;
                    s_foo += c_foo;
                }
                else
                {
                    s_foo += c_foo;
                }
            }

            return s_foo;
        }
    }
}
