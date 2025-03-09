//using System.Collections.;

namespace ForestNETTests.IO
{
    public class YAMLUnitTest
    {
        [Test]
        public void TestYAML()
        {
            try
            {
                TestConfig.InitiateTestLogging();

                string s_currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'");
                string s_testDirectory = s_currentDirectory + ForestNETLib.IO.File.DIR + "testYAML" + ForestNETLib.IO.File.DIR;

                if (ForestNETLib.IO.File.FolderExists(s_testDirectory))
                {
                    ForestNETLib.IO.File.DeleteDirectory(s_testDirectory);
                }

                ForestNETLib.IO.File.CreateDirectory(s_testDirectory);
                Assert.That(
                    ForestNETLib.IO.File.FolderExists(s_testDirectory),
                    Is.True,
                    "directory[" + s_testDirectory + "] does not exist"
                );

                YamlValidate();
                YamlVirtualFiles();
                YamlObject(s_testDirectory, "_0");
                YamlClass(s_testDirectory, "TestYAMLSchemaClass.yaml", "_1");
                YamlArray(s_testDirectory, "TestYAMLSchemaSimpleClassArray.yaml", "_A");
                YamlArray(s_testDirectory, "TestYAMLSchemaSimpleClassObjectMultiReferences.yaml", "_B");
                YamlArray(s_testDirectory, "TestYAMLSchemaSimpleClassObjectOneReference.yaml", "_C");
                YamlArray(s_testDirectory, "TestYAMLSchemaSimpleClassNoReferences.yaml", "_D");
                YamlComplex(s_testDirectory, true, false, false, false, "_E");
                YamlComplex(s_testDirectory, false, true, false, false, "_F");
                YamlComplex(s_testDirectory, false, false, true, false, "_G");
                YamlComplex(s_testDirectory, false, false, false, true, "_H");
                YamlMultipleUseOfOneSchemaObject(s_testDirectory);

                ForestNETLib.IO.File.DeleteDirectory(s_testDirectory);
                Assert.That(
                    ForestNETLib.IO.File.FolderExists(s_testDirectory),
                    Is.False,
                    "directory[" + s_testDirectory + "] does exist"
                );
            }
            catch (Exception o_exc)
            {
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
        }

        private static void YamlValidate()
        {
            string s_resourcesDirectory = Environment.CurrentDirectory + ForestNETLib.IO.File.DIR + "Resources" + ForestNETLib.IO.File.DIR;
            string s_yamlSchemaFile = s_resourcesDirectory + "yaml" + ForestNETLib.IO.File.DIR + "TestYAMLSchemaClassRootWithRef.yaml";
            string s_yamlFile = s_resourcesDirectory + "yaml" + ForestNETLib.IO.File.DIR + "ValidateYAML.yaml";
            string s_invalidYamlFile = s_resourcesDirectory + "yaml" + ForestNETLib.IO.File.DIR + "ValidateYAMLIsInvalid.yaml";

            Assert.That(
                new ForestNETLib.IO.YAML(s_yamlSchemaFile, 4).ValidateAgainstSchema(s_yamlFile),
                    Is.True,
                "file 'ValidateYAML.yaml' is not valid with schema 'TestYAMLSchemaClassRootWithRef.yaml'"
            );

            bool b_check = true;

            try
            {
                Assert.That(
                    new ForestNETLib.IO.YAML(s_yamlSchemaFile, 4).ValidateAgainstSchema(s_invalidYamlFile),
                    Is.False,
                    "file 'ValidateYAMLIsInvalid.yaml' is valid with schema 'TestYAMLSchemaClassRootWithRef.yaml'"
                );
            }
            catch (Exception)
            {
                b_check = false;
            }

            if (b_check)
            {
                Assert.Fail("file 'ValidateYAMLIsInvalid.yaml' is valid with schema 'TestYAMLSchemaClassRootWithRef.yaml'");
            }
        }

        private static void YamlVirtualFiles()
        {
            ForestNETTests.SQL.AllTypesRecord? o_recordOut = new()
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

            ForestNETLib.IO.YAML.YAMLElement o_yamlSchema = new("Root")
            {
                Type = "object",
                MappingClass = "ForestNETTests.SQL.AllTypesRecord, ForestNETTests"
            };

            ForestNETLib.IO.YAML.YAMLElement o_yamlElement = new("Id")
            {
                Type = "integer",
                MappingClass = "ColumnId",
                Required = true
            };

            o_yamlSchema.Children.Add(o_yamlElement);

            o_yamlElement = new ForestNETLib.IO.YAML.YAMLElement("UUID")
            {
                Type = "string",
                MappingClass = "ColumnUUID",
                Required = true
            };

            o_yamlSchema.Children.Add(o_yamlElement);

            o_yamlElement = new ForestNETLib.IO.YAML.YAMLElement("ShortText")
            {
                Type = "string",
                MappingClass = "ColumnShortText",
                Required = true
            };

            o_yamlSchema.Children.Add(o_yamlElement);

            o_yamlElement = new ForestNETLib.IO.YAML.YAMLElement("Text")
            {
                Type = "string",
                MappingClass = "ColumnText"
            };

            o_yamlSchema.Children.Add(o_yamlElement);

            o_yamlElement = new ForestNETLib.IO.YAML.YAMLElement("SmallInt")
            {
                Type = "integer",
                MappingClass = "ColumnSmallInt"
            };

            o_yamlSchema.Children.Add(o_yamlElement);

            o_yamlElement = new ForestNETLib.IO.YAML.YAMLElement("Int")
            {
                Type = "integer",
                MappingClass = "ColumnInt"
            };

            o_yamlSchema.Children.Add(o_yamlElement);

            o_yamlElement = new ForestNETLib.IO.YAML.YAMLElement("BigInt")
            {
                Type = "integer",
                MappingClass = "ColumnBigInt"
            };

            o_yamlSchema.Children.Add(o_yamlElement);

            o_yamlElement = new ForestNETLib.IO.YAML.YAMLElement("LocalDateTime")
            {
                Type = "string",
                MappingClass = "ColumnLocalDateTime"
            };

            o_yamlSchema.Children.Add(o_yamlElement);

            ForestNETLib.IO.YAML o_yaml = new(o_yamlSchema, 4);

            string s_yamlEncoded = o_yaml.YamlEncode(o_recordOut);

            List<string> a_fileLines = [];

            foreach (string s_line in s_yamlEncoded.Split(Environment.NewLine))
            {
                a_fileLines.Add(s_line);
            }

            ForestNETTests.SQL.AllTypesRecord? o_recordIn = (ForestNETTests.SQL.AllTypesRecord?)o_yaml.YamlDecode(a_fileLines);

            Assert.That(
                ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(o_recordOut, o_recordIn, o_yaml.UseProperties),
                Is.True,
                "output object class is not equal to input object class"
            );

            string s_virtualYamlFile = ""
                + "---" + Environment.NewLine
                + "Id: 42" + Environment.NewLine
                + "UUID: \"421824ce-72b8-4aad-bb12-dbc448682437\"" + Environment.NewLine
                + "ShortText: \"Datensatz 42\"" + Environment.NewLine
                + "Text: \"Die Handelsstreitigkeiten zwischen den; \"42 und 42\" sorgen für eine Art Umdenken auf beiden Seiten.\"" + Environment.NewLine
                + "SmallInt: -42" + Environment.NewLine
                + "Int: 40002" + Environment.NewLine
                + "BigInt: 400001112" + Environment.NewLine
                + "LocalDateTime: \"2042-02-02T01:02:02Z\"" + Environment.NewLine
                + "..." + Environment.NewLine;

            o_recordOut = new()
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

            a_fileLines = [.. s_virtualYamlFile.Split(Environment.NewLine)];

            o_recordIn = (ForestNETTests.SQL.AllTypesRecord?)o_yaml.YamlDecode(a_fileLines);

            Assert.That(
                ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(o_recordOut, o_recordIn, o_yaml.UseProperties),
                Is.True,
                "output object class is not equal to input object class"
            );
        }

        private static void YamlObject(string p_s_testDirectory, string p_s_fileNameSuffix)
        {
            ForestNETTests.SQL.AllTypesRecord? o_recordOut = new()
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


            ForestNETLib.IO.YAML.YAMLElement o_yamlSchema = new("Root")
            {
                Type = "object",
                MappingClass = "ForestNETTests.SQL.AllTypesRecord, ForestNETTests"
            };

            ForestNETLib.IO.YAML.YAMLElement o_yamlElement = new("Id")
            {
                Type = "integer",
                MappingClass = "ColumnId",
                Required = true
            };

            o_yamlSchema.Children.Add(o_yamlElement);

            o_yamlElement = new ForestNETLib.IO.YAML.YAMLElement("UUID")
            {
                Type = "string",
                MappingClass = "ColumnUUID",
                Required = true
            };

            o_yamlSchema.Children.Add(o_yamlElement);

            o_yamlElement = new ForestNETLib.IO.YAML.YAMLElement("ShortText")
            {
                Type = "string",
                MappingClass = "ColumnShortText",
                Required = true
            };

            o_yamlSchema.Children.Add(o_yamlElement);

            o_yamlElement = new ForestNETLib.IO.YAML.YAMLElement("Text")
            {
                Type = "string",
                MappingClass = "ColumnText"
            };

            o_yamlSchema.Children.Add(o_yamlElement);

            o_yamlElement = new ForestNETLib.IO.YAML.YAMLElement("SmallInt")
            {
                Type = "integer",
                MappingClass = "ColumnSmallInt"
            };

            o_yamlSchema.Children.Add(o_yamlElement);

            o_yamlElement = new ForestNETLib.IO.YAML.YAMLElement("Int")
            {
                Type = "integer",
                MappingClass = "ColumnInt"
            };

            o_yamlSchema.Children.Add(o_yamlElement);

            o_yamlElement = new ForestNETLib.IO.YAML.YAMLElement("BigInt")
            {
                Type = "integer",
                MappingClass = "ColumnBigInt"
            };

            o_yamlSchema.Children.Add(o_yamlElement);

            o_yamlElement = new ForestNETLib.IO.YAML.YAMLElement("LocalDateTime")
            {
                Type = "string",
                MappingClass = "ColumnLocalDateTime"
            };

            o_yamlSchema.Children.Add(o_yamlElement);

            ForestNETLib.IO.YAML o_yaml = new(o_yamlSchema, 4);

            string s_fileSimpleClass = p_s_testDirectory + "TestYAMLObject" + p_s_fileNameSuffix + ".yaml";

            _ = o_yaml.YamlEncode(o_recordOut, s_fileSimpleClass, true);

            ForestNETTests.SQL.AllTypesRecord? o_recordIn = (ForestNETTests.SQL.AllTypesRecord?)o_yaml.YamlDecode(s_fileSimpleClass);

            Assert.That(
                ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(o_recordOut, o_recordIn, o_yaml.UseProperties),
                Is.True,
                "output object class is not equal to input object class"
            );
        }

        private static void YamlClass(string p_s_testDirectory, string p_s_yamlSchemaFileName, string p_s_fileNameSuffix)
        {
            ForestNETTests.SQL.AllTypesRecord? o_recordOut = new()
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

            string s_resourcesDirectory = Environment.CurrentDirectory + ForestNETLib.IO.File.DIR + "Resources" + ForestNETLib.IO.File.DIR;
            string s_yamlSchemaFile = s_resourcesDirectory + "yaml" + ForestNETLib.IO.File.DIR + p_s_yamlSchemaFileName;

            ForestNETLib.IO.YAML o_yaml = new(s_yamlSchemaFile, 4);

            string s_fileSimpleClass = p_s_testDirectory + "TestYAMLClass" + p_s_fileNameSuffix + ".yaml";

            _ = o_yaml.YamlEncode(o_recordOut, s_fileSimpleClass, true);

            ForestNETTests.SQL.AllTypesRecord? o_recordIn = (ForestNETTests.SQL.AllTypesRecord?)o_yaml.YamlDecode(s_fileSimpleClass);

            Assert.That(
                ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(o_recordOut, o_recordIn, o_yaml.UseProperties),
                Is.True,
                "output object class is not equal to input object class"
            );
        }

        private static void YamlArray(string p_s_testDirectory, string p_s_yamlSchemaFileName, string p_s_fileNameSuffix)
        {
            List<ForestNETTests.IO.Data.SimpleClass>? a_dataOut =
            [
                new("Record #1 Value A", "Record #1 Value B", "Record #1 Value C"),
                new("Record #2 Value A", "Record #2 Value B", "", [1, 2, -3, -4]),
                new("Record #3; Value A", "null", "Record #3 Value C", [9, 8, -7, -6], new float[] { 42.0f, 21.25f, 54987.456999f }),
                new("Record 4 Value A", "Record $4 ;Value B \"", null, [16, 32, int.MaxValue, 128, 0], new float[] { 21.0f, 10.625f })
            ];

            ForestNETTests.IO.Data.SimpleClassCollection? o_collectionOut = new(a_dataOut);

            string[] a_out = new string[a_dataOut.Count];
            string[] a_in = new string[a_dataOut.Count];
            int i_cnt = 0;

            foreach (ForestNETTests.IO.Data.SimpleClass o_simpleClassObject in a_dataOut)
            {
                a_out[i_cnt++] = o_simpleClassObject.ToString();
            }

            string s_resourcesDirectory = Environment.CurrentDirectory + ForestNETLib.IO.File.DIR + "Resources" + ForestNETLib.IO.File.DIR;
            string s_yamlSchemaFile = s_resourcesDirectory + "yaml" + ForestNETLib.IO.File.DIR + p_s_yamlSchemaFileName;
            ForestNETLib.IO.YAML o_yaml = new(s_yamlSchemaFile, 4)
            {
                UseProperties = true
            };

            string s_fileSimpleClass = p_s_testDirectory + "TestYAMLSimpleClass" + p_s_fileNameSuffix + ".yaml";

            if (p_s_yamlSchemaFileName.Equals("TestYAMLSchemaSimpleClassArray.yaml"))
            {
                _ = o_yaml.YamlEncode(a_dataOut, s_fileSimpleClass, true);
            }
            else
            {
                _ = o_yaml.YamlEncode(o_collectionOut, s_fileSimpleClass, true);
            }

            ForestNETTests.IO.Data.SimpleClassCollection? o_collectionIn = null;
            List<ForestNETTests.IO.Data.SimpleClass>? a_dataIn;

            if (p_s_yamlSchemaFileName.Equals("TestYAMLSchemaSimpleClassArray.yaml"))
            {
                List<ForestNETTests.IO.Data.SimpleClass>? a_foo = [.. ((System.Collections.IEnumerable)(o_yaml.YamlDecode(s_fileSimpleClass) ?? new List<ForestNETTests.IO.Data.SimpleClass>())).Cast<ForestNETTests.IO.Data.SimpleClass>()];
                a_dataIn = a_foo;
            }
            else
            {
                o_collectionIn = (ForestNETTests.IO.Data.SimpleClassCollection?)o_yaml.YamlDecode(s_fileSimpleClass);
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
                foreach (ForestNETTests.IO.Data.SimpleClass o_simpleClassObject in a_dataIn)
                {
                    a_in[i_cnt++] = o_simpleClassObject.ToString();
                }
            }

            if (!p_s_yamlSchemaFileName.Equals("TestYAMLSchemaSimpleClassArray.yaml"))
            {
                Assert.That(
                    ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(o_collectionOut, o_collectionIn, o_yaml.UseProperties),
                    Is.True,
                    "output object inner class collection is not equal to input object inner class collection"
                );
            }

            Assert.That(
                ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(a_dataOut, a_dataIn, o_yaml.UseProperties),
                Is.True,
                "output object inner class array is not equal to input object inner class array"
            );

            for (int i = 0; i < a_dataOut.Count; i++)
            {
                Assert.That(
                    ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(a_dataOut[i], a_dataIn?[i], o_yaml.UseProperties, false, true),
                    Is.True,
                    "output object inner class array element object is not equal to input object inner class array element object"
                );

                Assert.That(
                    a_out[i], Has.Length.EqualTo(a_in[i].Length),
                    "output object inner class array element string length is not equal to input object inner class array element string length"
                );
            }
        }

        private static void YamlComplex(string p_s_testDirectory, bool p_b_classRoot, bool p_b_listRoot, bool p_b_classRootWithRef, bool p_b_listRootWithRef, string p_s_fileNameSuffix)
        {
            List<ForestNETTests.IO.Data.ShipOrder> a_shipOrdersOut = ForestNETTests.IO.Data.GenerateData();

            ForestNETTests.IO.Data.ShipOrderCollection o_shipOrderCollectionOut = new()
            {
                ShipOrders = a_shipOrdersOut,
                OrderAmount = a_shipOrdersOut.Count
            };

            string[] a_out = new string[a_shipOrdersOut.Count];
            string[] a_in = new string[a_shipOrdersOut.Count];
            int i_cnt = 0;

            foreach (ForestNETTests.IO.Data.ShipOrder o_shipOrderObject in a_shipOrdersOut)
            {
                a_out[i_cnt++] = o_shipOrderObject.ToString();
            }

            string s_yamlSchemaFile = "";

            string s_resourcesDirectory = Environment.CurrentDirectory + ForestNETLib.IO.File.DIR + "Resources" + ForestNETLib.IO.File.DIR;

            if (p_b_classRoot)
            {
                s_yamlSchemaFile = s_resourcesDirectory + "yaml" + ForestNETLib.IO.File.DIR + "TestYAMLSchemaClassRoot.yaml";
            }
            else if (p_b_listRoot)
            {
                s_yamlSchemaFile = s_resourcesDirectory + "yaml" + ForestNETLib.IO.File.DIR + "TestYAMLSchemaListRoot.yaml";
            }
            else if (p_b_classRootWithRef)
            {
                s_yamlSchemaFile = s_resourcesDirectory + "yaml" + ForestNETLib.IO.File.DIR + "TestYAMLSchemaClassRootWithRef.yaml";
            }
            else if (p_b_listRootWithRef)
            {
                s_yamlSchemaFile = s_resourcesDirectory + "yaml" + ForestNETLib.IO.File.DIR + "TestYAMLSchemaListRootWithRef.yaml";
            }

            ForestNETLib.IO.YAML o_yaml = new(s_yamlSchemaFile, 4)
            {
                UseProperties = true
            };

            string s_file = p_s_testDirectory + "TestYAML" + p_s_fileNameSuffix + ".yaml";

            if ((p_b_classRoot) || (p_b_classRootWithRef))
            {
                _ = o_yaml.YamlEncode(o_shipOrderCollectionOut, s_file, true);
            }
            else
            {
                _ = o_yaml.YamlEncode(a_shipOrdersOut, s_file, true);
            }

            List<ForestNETTests.IO.Data.ShipOrder>? a_shipOrdersIn = null;
            ForestNETTests.IO.Data.ShipOrderCollection? o_shipOrderCollectionIn = null;

            if ((p_b_classRoot) || (p_b_classRootWithRef))
            {
                o_shipOrderCollectionIn = (ForestNETTests.IO.Data.ShipOrderCollection?)o_yaml.YamlDecode(s_file);

                if (o_shipOrderCollectionIn != null)
                {
                    a_shipOrdersIn = o_shipOrderCollectionIn.ShipOrders;
                    o_shipOrderCollectionIn.ShipOrders = a_shipOrdersIn;
                    o_shipOrderCollectionIn.OrderAmount = a_shipOrdersIn.Count;
                }
            }
            else
            {
                List<ForestNETTests.IO.Data.ShipOrder> a_foo = [.. ((System.Collections.IEnumerable)(o_yaml.YamlDecode(s_file) ?? new List<ForestNETTests.IO.Data.ShipOrder>())).Cast<ForestNETTests.IO.Data.ShipOrder>()];
                a_shipOrdersIn = a_foo;
            }

            i_cnt = 0;

            if (a_shipOrdersIn != null)
            {
                foreach (ForestNETTests.IO.Data.ShipOrder o_shipOrderObject in a_shipOrdersIn)
                {
                    a_in[i_cnt++] = o_shipOrderObject.ToString();
                }
            }

            if ((p_b_classRoot) || (p_b_classRootWithRef))
            {
                Assert.That(
                    ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(o_shipOrderCollectionOut, o_shipOrderCollectionIn, o_yaml.UseProperties),
                    Is.True,
                    "output object inner class collection is not equal to input object inner class collection"
                );
            }

            Assert.That(
                ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(a_shipOrdersOut, a_shipOrdersIn, o_yaml.UseProperties),
                Is.True,
                "output object inner class array is not equal to input object inner class array"
            );

            if (a_shipOrdersIn != null)
            {
                for (int i = 0; i < a_shipOrdersIn.Count; i++)
                {
                    Assert.That(
                        ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(a_shipOrdersOut[i], a_shipOrdersIn[i], o_yaml.UseProperties, false, true),
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
                s_file = s_resourcesDirectory + "yaml" + ForestNETLib.IO.File.DIR + "TestYAMLPrimitiveArrayOneLine.yaml";
                a_in = new string[a_shipOrdersOut.Count];

                List<ForestNETTests.IO.Data.ShipOrder> a_foo = [.. ((System.Collections.IEnumerable)(o_yaml.YamlDecode(s_file) ?? new List<ForestNETTests.IO.Data.ShipOrder>())).Cast<ForestNETTests.IO.Data.ShipOrder>()];
                a_shipOrdersIn = a_foo;

                i_cnt = 0;

                foreach (ForestNETTests.IO.Data.ShipOrder o_shipOrderObject in a_shipOrdersIn)
                {
                    a_in[i_cnt++] = o_shipOrderObject.ToString();
                }

                Assert.That(
                    ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(a_shipOrdersOut, a_shipOrdersIn, o_yaml.UseProperties),
                    Is.True,
                    "output object inner class array is not equal to input object inner class array"
                );

                for (int i = 0; i < a_shipOrdersIn.Count; i++)
                {
                    Assert.That(
                        ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(a_shipOrdersOut[i], a_shipOrdersIn[i], o_yaml.UseProperties, false, true),
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

        private static void YamlMultipleUseOfOneSchemaObject(string p_s_testDirectory)
        {

            /* Simple #1 */

            List<ForestNETTests.IO.Data.SimpleClass> a_dataOut =
            [
                new("Record #1 Value A", "Record #1 Value B", "Record #1 Value C"),
                new("Record #2 Value A", "Record #2 Value B", "", [1, 2, -3, -4]),
                new("Record #3; Value A", "null", "Record #3 Value C", [9, 8, -7, -6]),
                new("Record #4 Value A", "Record $4 ;Value B \"", null, [16, 32, int.MaxValue, 128, 0])
            ];

            ForestNETTests.IO.Data.SimpleClassCollection o_collectionOut = new(a_dataOut);

            string[] a_out = new string[a_dataOut.Count];
            string[] a_in = new string[a_dataOut.Count];
            int i_cnt = 0;

            foreach (Data.SimpleClass o_simpleClassObject in a_dataOut)
            {
                a_out[i_cnt++] = o_simpleClassObject.ToString();
            }

            string s_resourcesDirectory = Environment.CurrentDirectory + ForestNETLib.IO.File.DIR + "Resources" + ForestNETLib.IO.File.DIR;
            string s_yamlSchemaFile = s_resourcesDirectory + "yaml" + ForestNETLib.IO.File.DIR + "TestYAMLSchemaSimpleClassObjectMultiReferences.yaml";
            ForestNETLib.IO.YAML o_yaml = new(s_yamlSchemaFile, 4)
            {
                UseProperties = true
            };

            string s_fileSimpleClass = p_s_testDirectory + "TestYAMLSimpleClass_I.yaml";
            _ = o_yaml.YamlEncode(o_collectionOut, s_fileSimpleClass, true);

            ForestNETTests.IO.Data.SimpleClassCollection? o_collectionIn;
            List<ForestNETTests.IO.Data.SimpleClass>? a_dataIn;

            o_collectionIn = (ForestNETTests.IO.Data.SimpleClassCollection?)o_yaml.YamlDecode(s_fileSimpleClass);
            a_dataIn = o_collectionIn?.SimpleClasses;

            i_cnt = 0;

            if (a_dataIn != null)
            {
                foreach (ForestNETTests.IO.Data.SimpleClass o_simpleClassObject in a_dataIn)
                {
                    a_in[i_cnt++] = o_simpleClassObject.ToString();
                }
            }

            Assert.That(
                ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(o_collectionOut, o_collectionIn, o_yaml.UseProperties),
                Is.True,
                "output object inner class collection is not equal to input object inner class collection"
            );

            Assert.That(
                ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(a_dataOut, a_dataIn, o_yaml.UseProperties),
                Is.True,
                "output object inner class array is not equal to input object inner class array"
            );

            for (int i = 0; i < a_dataOut.Count; i++)
            {
                Assert.That(
                    ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(a_dataOut[i], a_dataIn?[i], o_yaml.UseProperties, false, true),
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

            o_collectionOut = new(a_dataOut);

            a_out = new string[a_dataOut.Count];
            a_in = new string[a_dataOut.Count];
            i_cnt = 0;

            foreach (ForestNETTests.IO.Data.SimpleClass o_simpleClassObject in a_dataOut)
            {
                a_out[i_cnt++] = o_simpleClassObject.ToString();
            }

            s_fileSimpleClass = p_s_testDirectory + "TestYAMLSimpleClass_J.yaml";
            _ = o_yaml.YamlEncode(o_collectionOut, s_fileSimpleClass, true);

            o_collectionIn = (ForestNETTests.IO.Data.SimpleClassCollection?)o_yaml.YamlDecode(s_fileSimpleClass);
            a_dataIn = o_collectionIn?.SimpleClasses;

            i_cnt = 0;

            if (a_dataIn != null)
            {
                foreach (ForestNETTests.IO.Data.SimpleClass o_simpleClassObject in a_dataIn)
                {
                    a_in[i_cnt++] = o_simpleClassObject.ToString();
                }
            }

            Assert.That(
                ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(o_collectionOut, o_collectionIn, o_yaml.UseProperties),
                Is.True,
                "output object inner class collection is not equal to input object inner class collection"
            );

            Assert.That(
                ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(a_dataOut, a_dataIn, o_yaml.UseProperties),
                Is.True,
                "output object inner class array is not equal to input object inner class array"
            );

            for (int i = 0; i < a_dataOut.Count; i++)
            {
                Assert.That(
                    ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(a_dataOut[i], a_dataIn?[i], o_yaml.UseProperties, false, true),
                    Is.True,
                    "output object inner class array element object is not equal to input object inner class array element object"
                );

                Assert.That(
                    a_out[i], Has.Length.EqualTo(a_in[i].Length),
                    "output object inner class array element toString is not equal to input object inner class array element toString"
                );
            }

            /* Complex #1 */

            List<ForestNETTests.IO.Data.ShipOrder> a_shipOrdersOut = ForestNETTests.IO.Data.GenerateData();

            ForestNETTests.IO.Data.ShipOrderCollection o_shipOrderCollectionOut = new()
            {
                ShipOrders = a_shipOrdersOut,
                OrderAmount = a_shipOrdersOut.Count
            };

            a_out = new string[a_shipOrdersOut.Count];
            a_in = new string[a_shipOrdersOut.Count];
            i_cnt = 0;

            foreach (ForestNETTests.IO.Data.ShipOrder o_shipOrderObject in a_shipOrdersOut)
            {
                a_out[i_cnt++] = o_shipOrderObject.ToString();
            }

            s_yamlSchemaFile = s_resourcesDirectory + "yaml" + ForestNETLib.IO.File.DIR + "TestYAMLSchemaClassRootWithRef.yaml";

            o_yaml = new ForestNETLib.IO.YAML(s_yamlSchemaFile, 4)
            {
                UseProperties = true
            };

            string s_file = p_s_testDirectory + "TestYAML_K.yaml";
            _ = o_yaml.YamlEncode(o_shipOrderCollectionOut, s_file, true);

            List<ForestNETTests.IO.Data.ShipOrder>? a_shipOrdersIn = null;
            ForestNETTests.IO.Data.ShipOrderCollection? o_shipOrderCollectionIn;

            o_shipOrderCollectionIn = (ForestNETTests.IO.Data.ShipOrderCollection?)o_yaml.YamlDecode(s_file);

            if (o_shipOrderCollectionIn != null)
            {
                a_shipOrdersIn = o_shipOrderCollectionIn.ShipOrders;
                o_shipOrderCollectionIn.ShipOrders = a_shipOrdersIn;
                o_shipOrderCollectionIn.OrderAmount = a_shipOrdersIn.Count;
            }

            i_cnt = 0;

            if (a_shipOrdersIn != null)
            {
                foreach (ForestNETTests.IO.Data.ShipOrder o_shipOrderObject in a_shipOrdersIn)
                {
                    a_in[i_cnt++] = o_shipOrderObject.ToString();
                }
            }

            Assert.That(
                ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(o_shipOrderCollectionOut, o_shipOrderCollectionIn, o_yaml.UseProperties),
                Is.True,
                "output object inner class collection is not equal to input object inner class collection"
            );

            Assert.That(
                ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(a_shipOrdersOut, a_shipOrdersIn, o_yaml.UseProperties),
                Is.True,
                "output object inner class array is not equal to input object inner class array"
            );

            if (a_shipOrdersIn != null)
            {
                for (int i = 0; i < a_shipOrdersIn.Count; i++)
                {
                    Assert.That(
                        ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(a_shipOrdersOut[i], a_shipOrdersIn[i], o_yaml.UseProperties, false, true),
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

            a_shipOrdersOut = ForestNETTests.IO.Data.GenerateData();

            foreach (ForestNETTests.IO.Data.ShipOrder o_shipOrderObject in a_shipOrdersOut)
            {
                o_shipOrderObject.OrderId += " #2";

                foreach (ForestNETTests.IO.Data.ShipItem o_shipItemObject in o_shipOrderObject.ShipItems)
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

            foreach (ForestNETTests.IO.Data.ShipOrder o_shipOrderObject in a_shipOrdersOut)
            {
                a_out[i_cnt++] = o_shipOrderObject.ToString();
            }

            s_file = p_s_testDirectory + "TestYAML_L.yaml";
            _ = o_yaml.YamlEncode(o_shipOrderCollectionOut, s_file, true);

            o_shipOrderCollectionIn = (ForestNETTests.IO.Data.ShipOrderCollection?)o_yaml.YamlDecode(s_file);

            if (o_shipOrderCollectionIn != null)
            {
                a_shipOrdersIn = o_shipOrderCollectionIn.ShipOrders;
                o_shipOrderCollectionIn.ShipOrders = a_shipOrdersIn;
                o_shipOrderCollectionIn.OrderAmount = a_shipOrdersIn.Count;
            }

            i_cnt = 0;

            if (a_shipOrdersIn != null)
            {
                foreach (ForestNETTests.IO.Data.ShipOrder o_shipOrderObject in a_shipOrdersIn)
                {
                    a_in[i_cnt++] = o_shipOrderObject.ToString();
                }
            }

            Assert.That(
                ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(o_shipOrderCollectionOut, o_shipOrderCollectionIn, o_yaml.UseProperties),
                Is.True,
                "output object inner class collection is not equal to input object inner class collection"
            );

            Assert.That(
                ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(a_shipOrdersOut, a_shipOrdersIn, o_yaml.UseProperties),
                Is.True,
                "output object inner class array is not equal to input object inner class array"
            );

            if (a_shipOrdersIn != null)
            {
                for (int i = 0; i < a_shipOrdersIn.Count; i++)
                {
                    Assert.That(
                        ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(a_shipOrdersOut[i], a_shipOrdersIn[i], o_yaml.UseProperties, false, true),
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
