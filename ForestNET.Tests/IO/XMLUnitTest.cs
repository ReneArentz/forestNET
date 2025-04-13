namespace ForestNET.Tests.IO
{
    public class XMLUnitTest
    {
        [Test]
        public void TestXML()
        {
            try
            {
                TestConfig.InitiateTestLogging();

                string s_currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'");
                string s_testDirectory = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "testXML" + ForestNET.Lib.IO.File.DIR;

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

                XmlValidate();
                XmlVirtualFiles();
                XmlObject(s_testDirectory, "_0");
                XmlClass(s_testDirectory, "TestXSDSchemaClass.xsd", "_1");
                XmlArray(s_testDirectory, "TestXSDSchemaSimpleClassArray.xsd", "_A");
                XmlArray(s_testDirectory, "TestXSDSchemaSimpleClassObjectMultiReferences.xsd", "_B");
                XmlArray(s_testDirectory, "TestXSDSchemaSimpleClassObjectOneReference.xsd", "_C");
                XmlArray(s_testDirectory, "TestXSDSchemaSimpleClassNoReferences.xsd", "_D");
                XmlComplex(s_testDirectory, true, false, false, false, false, false, false, "_E", true);
                XmlComplex(s_testDirectory, false, true, false, false, false, false, false, "_F", true);
                XmlComplex(s_testDirectory, false, false, true, false, false, false, false, "_G", true);
                XmlComplex(s_testDirectory, false, false, false, true, false, false, false, "_H", true);
                XmlComplex(s_testDirectory, false, false, false, false, true, false, false, "_I", true);
                XmlComplex(s_testDirectory, false, false, false, false, false, true, false, "_J", true);
                XmlComplex(s_testDirectory, false, false, false, false, false, false, true, "_K", true);
                XmlComplex(s_testDirectory, true, false, false, false, false, false, false, "_L", false);
                XmlComplex(s_testDirectory, false, true, false, false, false, false, false, "_M", false);
                XmlComplex(s_testDirectory, false, false, true, false, false, false, false, "_N", false);
                XmlComplex(s_testDirectory, false, false, false, true, false, false, false, "_O", false);
                XmlComplex(s_testDirectory, false, false, false, false, true, false, false, "_P", false);
                XmlComplex(s_testDirectory, false, false, false, false, false, true, false, "_Q", false);
                XmlComplex(s_testDirectory, false, false, false, false, false, false, true, "_R", false);
                XmlComplexPart(s_testDirectory, true, false, false, "_S", false);
                XmlComplexPart(s_testDirectory, false, true, false, "_T", false);
                XmlComplexPart(s_testDirectory, false, false, true, "_U", false);
                XmlComplexPart(s_testDirectory, true, false, false, "_V", true);
                XmlComplexPart(s_testDirectory, false, true, false, "_W", true);
                XmlComplexPart(s_testDirectory, false, false, true, "_X", true);
                XmlMultipleUseOfOneSchemaObject(s_testDirectory);

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

        private static void XmlValidate()
        {
            string s_resourcesDirectory = Environment.CurrentDirectory + ForestNET.Lib.IO.File.DIR + "Resources" + ForestNET.Lib.IO.File.DIR;
            string s_xsdSchemaFile = s_resourcesDirectory + "xml" + ForestNET.Lib.IO.File.DIR + "TestXSDSchemaDividedClassRoot.xsd";
            string s_xmlFile = s_resourcesDirectory + "xml" + ForestNET.Lib.IO.File.DIR + "ValidateXML.xml";
            string s_invalidXmlFile = s_resourcesDirectory + "xml" + ForestNET.Lib.IO.File.DIR + "ValidateXMLIsInvalid.xml";
            string s_partXmlFile = s_resourcesDirectory + "xml" + ForestNET.Lib.IO.File.DIR + "ValidateXMLPart.xml";

            Assert.That(
                new ForestNET.Lib.IO.XML(s_xsdSchemaFile).ValidateAgainstSchema(s_xmlFile),
                Is.True,
                "file 'ValidateXML.xml' is not valid with schema 'TestXSDSchemaDividedClassRoot.xsd'"
            );

            bool b_check = true;

            try
            {
                Assert.That(
                    new ForestNET.Lib.IO.XML(s_xsdSchemaFile).ValidateAgainstSchema(s_invalidXmlFile),
                    Is.False,
                    "file 'ValidateXMLIsInvalid.xml' is valid with schema 'TestXSDSchemaDividedClassRoot.xsd'"
                );
            }
            catch (Exception)
            {
                b_check = false;
            }

            if (b_check)
            {
                Assert.Fail("file 'ValidateXMLIsInvalid.xml' is valid with schema 'TestXSDSchemaDividedClassRoot.xsd'");
            }

            Assert.That(
                new ForestNET.Lib.IO.XML(s_xsdSchemaFile).ValidateAgainstSchema(s_partXmlFile),
                Is.True,
                "file 'ValidateXML.xml' is not valid with schema 'TestXSDSchemaDividedClassRoot.xsd'"
            );
        }

        private static void XmlVirtualFiles()
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

            ForestNET.Lib.IO.XML.XSDElement o_xsdSchema = new("AllTypesRecord")
            {
                Type = "object",
                Mapping = "ForestNET.Tests.SQL.AllTypesRecord, ForestNET.Tests"
            };

            ForestNET.Lib.IO.XML.XSDElement o_xmlElement = new("Id")
            {
                Type = "integer",
                Mapping = "ColumnId"
            };

            o_xsdSchema.Children.Add(o_xmlElement);

            o_xmlElement = new ForestNET.Lib.IO.XML.XSDElement("UUID")
            {
                Type = "string",
                Mapping = "ColumnUUID"
            };

            o_xsdSchema.Children.Add(o_xmlElement);

            o_xmlElement = new ForestNET.Lib.IO.XML.XSDElement("ShortText")
            {
                Type = "string",
                Mapping = "ColumnShortText"
            };

            o_xsdSchema.Children.Add(o_xmlElement);

            o_xmlElement = new ForestNET.Lib.IO.XML.XSDElement("Text")
            {
                Type = "string",
                Mapping = "ColumnText"
            };

            o_xsdSchema.Children.Add(o_xmlElement);

            o_xmlElement = new ForestNET.Lib.IO.XML.XSDElement("SmallInt")
            {
                Type = "short",
                Mapping = "ColumnSmallInt"
            };

            o_xsdSchema.Children.Add(o_xmlElement);

            o_xmlElement = new ForestNET.Lib.IO.XML.XSDElement("Int")
            {
                Type = "integer",
                Mapping = "ColumnInt"
            };

            o_xsdSchema.Children.Add(o_xmlElement);

            o_xmlElement = new ForestNET.Lib.IO.XML.XSDElement("BigInt")
            {
                Type = "long",
                Mapping = "ColumnBigInt"
            };

            o_xsdSchema.Children.Add(o_xmlElement);

            o_xmlElement = new ForestNET.Lib.IO.XML.XSDElement("LocalDateTime")
            {
                Type = "datetime",
                Mapping = "ColumnLocalDateTime"
            };

            o_xsdSchema.Children.Add(o_xmlElement);

            o_xmlElement = new ForestNET.Lib.IO.XML.XSDElement("LocalDate")
            {
                Type = "date",
                Mapping = "ColumnLocalDate"
            };

            o_xsdSchema.Children.Add(o_xmlElement);

            o_xmlElement = new ForestNET.Lib.IO.XML.XSDElement("LocalTime")
            {
                Type = "time",
                Mapping = "ColumnLocalTime"
            };

            o_xsdSchema.Children.Add(o_xmlElement);

            ForestNET.Lib.IO.XML o_xml = new(o_xsdSchema);

            string s_xmlEncoded = o_xml.XmlEncode(o_recordOut);

            List<string> a_fileLines = [.. s_xmlEncoded.Split(Environment.NewLine)];

            ForestNET.Tests.SQL.AllTypesRecord? o_recordIn = (ForestNET.Tests.SQL.AllTypesRecord?)o_xml.XmlDecode(a_fileLines);

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(o_recordOut, o_recordIn, o_xml.UseProperties),
                Is.True,
                "output object class is not equal to input object class"
            );

            o_xml.UseISO8601UTC = false;
            s_xmlEncoded = o_xml.XmlEncode(o_recordOut);
            a_fileLines = [.. s_xmlEncoded.Split(Environment.NewLine)];

            o_recordIn = (ForestNET.Tests.SQL.AllTypesRecord?)o_xml.XmlDecode(a_fileLines);

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(o_recordOut, o_recordIn, o_xml.UseProperties),
                Is.True,
                "output object class is not equal to input object class"
            );

            string s_virtualJsonFile = ""
                + "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + Environment.NewLine
            + "<AllTypesRecord>" + Environment.NewLine
            + "	<Id>42</Id>" + Environment.NewLine
            + "	<UUID>421824ce-72b8-4aad-bb12-dbc448682437</UUID>" + Environment.NewLine
            + "	<ShortText>Datensatz 42</ShortText>" + Environment.NewLine
            + "	<Text>Die Handelsstreitigkeiten zwischen den; \"42 und 42\" sorgen für eine Art Umdenken auf beiden Seiten.</Text>" + Environment.NewLine
            + "	<SmallInt>-42</SmallInt>" + Environment.NewLine
            + "	<Int>40002</Int>" + Environment.NewLine
            + "	<BigInt>400001112</BigInt>" + Environment.NewLine
            + "	<LocalDateTime>02.02.2042 02:02:02</LocalDateTime>" + Environment.NewLine
            + "	<LocalDate>02.02.2042</LocalDate>" + Environment.NewLine
            + "	<LocalTime>12:20:42</LocalTime>" + Environment.NewLine
            + "</AllTypesRecord>";

            o_recordOut = new ForestNET.Tests.SQL.AllTypesRecord
            {
                ColumnId = 42,
                ColumnUUID = "421824ce-72b8-4aad-bb12-dbc448682437",
                ColumnShortText = "Datensatz 42",
                ColumnText = "Die Handelsstreitigkeiten zwischen den; \"42 und 42\" sorgen für eine Art Umdenken auf beiden Seiten.",
                ColumnSmallInt = -42,
                ColumnInt = 40002,
                ColumnBigInt = 400001112,
                ColumnLocalDateTime = new DateTime(2042, 2, 2, 2, 2, 2),
                ColumnLocalDate = new DateTime(2042, 2, 2),
                ColumnLocalTime = new DateTime(1900, 1, 1, 12, 20, 42).TimeOfDay
            };

            a_fileLines = [.. s_virtualJsonFile.Split(Environment.NewLine)];

            o_recordIn = (ForestNET.Tests.SQL.AllTypesRecord?)o_xml.XmlDecode(a_fileLines);

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(o_recordOut, o_recordIn, o_xml.UseProperties),
                Is.True,
                "output object class is not equal to input object class"
            );
        }

        private static void XmlObject(string p_s_testDirectory, string p_s_fileNameSuffix)
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
                ColumnLocalDateTime = new DateTime(2019, 2, 2, 2, 2, 2),
                ColumnLocalDate = new DateTime(2022, 2, 2).Date,
                ColumnLocalTime = new TimeSpan(12, 20, 22),
                ColumnTimestamp = Convert.ToDateTime("02.02.2020 22:20:12"),
                ColumnDate = Convert.ToDateTime("02.02.2020"),
                ColumnTime = Convert.ToDateTime("01.01.1900 22:20:12").TimeOfDay,
                ColumnText2 = ""
            };

            ForestNET.Lib.IO.XML.XSDElement o_xsdSchema = new("AllTypesRecord")
            {
                Type = "object",
                Mapping = "ForestNET.Tests.SQL.AllTypesRecord, ForestNET.Tests"
            };

            ForestNET.Lib.IO.XML.XSDElement o_xmlElement = new("Id")
            {
                Type = "integer",
                Mapping = "ColumnId"
            };

            o_xsdSchema.Children.Add(o_xmlElement);

            o_xmlElement = new ForestNET.Lib.IO.XML.XSDElement("UUID")
            {
                Type = "string",
                Mapping = "ColumnUUID"
            };

            o_xsdSchema.Children.Add(o_xmlElement);

            o_xmlElement = new ForestNET.Lib.IO.XML.XSDElement("ShortText")
            {
                Type = "string",
                Mapping = "ColumnShortText"
            };

            o_xsdSchema.Children.Add(o_xmlElement);

            o_xmlElement = new ForestNET.Lib.IO.XML.XSDElement("Text")
            {
                Type = "string",
                Mapping = "ColumnText"
            };

            o_xsdSchema.Children.Add(o_xmlElement);

            o_xmlElement = new ForestNET.Lib.IO.XML.XSDElement("SmallInt")
            {
                Type = "integer",
                Mapping = "ColumnSmallInt"
            };

            o_xsdSchema.Children.Add(o_xmlElement);

            o_xmlElement = new ForestNET.Lib.IO.XML.XSDElement("Int")
            {
                Type = "integer",
                Mapping = "ColumnInt"
            };

            o_xsdSchema.Children.Add(o_xmlElement);

            o_xmlElement = new ForestNET.Lib.IO.XML.XSDElement("BigInt")
            {
                Type = "integer",
                Mapping = "ColumnBigInt"
            };

            o_xsdSchema.Children.Add(o_xmlElement);

            o_xmlElement = new ForestNET.Lib.IO.XML.XSDElement("LocalDateTime")
            {
                Type = "datetime",
                Mapping = "ColumnLocalDateTime"
            };

            o_xsdSchema.Children.Add(o_xmlElement);

            o_xmlElement = new ForestNET.Lib.IO.XML.XSDElement("LocalDate")
            {
                Type = "date",
                Mapping = "ColumnLocalDate"
            };

            o_xsdSchema.Children.Add(o_xmlElement);

            o_xmlElement = new ForestNET.Lib.IO.XML.XSDElement("LocalTime")
            {
                Type = "time",
                Mapping = "ColumnLocalTime"
            };

            o_xsdSchema.Children.Add(o_xmlElement);

            o_xmlElement = new ForestNET.Lib.IO.XML.XSDElement("Timestamp")
            {
                Type = "datetime",
                Mapping = "ColumnTimestamp"
            };

            o_xsdSchema.Children.Add(o_xmlElement);

            o_xmlElement = new ForestNET.Lib.IO.XML.XSDElement("Date")
            {
                Type = "date",
                Mapping = "ColumnDate"
            };

            o_xsdSchema.Children.Add(o_xmlElement);

            o_xmlElement = new ForestNET.Lib.IO.XML.XSDElement("Time")
            {
                Type = "time",
                Mapping = "ColumnTime"
            };

            o_xsdSchema.Children.Add(o_xmlElement);

            o_xmlElement = new ForestNET.Lib.IO.XML.XSDElement("Empty")
            {
                Type = "string",
                Mapping = "ColumnText2",
                MinOccurs = 0
            };

            o_xsdSchema.Children.Add(o_xmlElement);

            ForestNET.Lib.IO.XML o_xml = new(o_xsdSchema);

            string s_fileSimpleClass = p_s_testDirectory + "TestXMLObject" + p_s_fileNameSuffix + ".xml";

            _ = o_xml.XmlEncode(o_recordOut, s_fileSimpleClass, true);

            ForestNET.Tests.SQL.AllTypesRecord? o_recordIn = (ForestNET.Tests.SQL.AllTypesRecord?)o_xml.XmlDecode(s_fileSimpleClass);

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(o_recordOut, o_recordIn, o_xml.UseProperties),
                Is.True,
                "output object class is not equal to input object class"
            );

            o_xml.PrintEmptystring = false;
            o_xml.UseISO8601UTC = false;
            _ = o_xml.XmlEncode(o_recordOut, s_fileSimpleClass, true);
            o_recordIn = (ForestNET.Tests.SQL.AllTypesRecord?)o_xml.XmlDecode(s_fileSimpleClass);

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(o_recordOut, o_recordIn, o_xml.UseProperties),
                Is.True,
                "output object class is not equal to input object class"
            );
        }

        private static void XmlClass(string p_s_testDirectory, string p_s_xsdSchemaFileName, string p_s_fileNameSuffix)
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
                ColumnShortText2 = ""
            };

            string s_resourcesDirectory = Environment.CurrentDirectory + ForestNET.Lib.IO.File.DIR + "Resources" + ForestNET.Lib.IO.File.DIR;
            string s_xsdSchemaFile = s_resourcesDirectory + "xml" + ForestNET.Lib.IO.File.DIR + p_s_xsdSchemaFileName;

            ForestNET.Lib.IO.XML o_xml = new(s_xsdSchemaFile);

            string s_fileSimpleClass = p_s_testDirectory + "TestXMLClass" + p_s_fileNameSuffix + ".xml";

            _ = o_xml.XmlEncode(o_recordOut, s_fileSimpleClass, true);

            ForestNET.Tests.SQL.AllTypesRecord? o_recordIn = (ForestNET.Tests.SQL.AllTypesRecord?)o_xml.XmlDecode(s_fileSimpleClass);

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(o_recordOut, o_recordIn, o_xml.UseProperties),
                Is.True,
                "output object class is not equal to input object class"
            );

            o_xml.PrintEmptystring = false;
            o_xml.UseISO8601UTC = false;
            _ = o_xml.XmlEncode(o_recordOut, s_fileSimpleClass, true);
            o_recordIn = (ForestNET.Tests.SQL.AllTypesRecord?)o_xml.XmlDecode(s_fileSimpleClass);

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(o_recordOut, o_recordIn, o_xml.UseProperties),
                Is.True,
                "output object class is not equal to input object class"
            );
        }

        private static void XmlArray(string p_s_testDirectory, string p_s_xsdSchemaFileName, string p_s_fileNameSuffix)
        {
            List<ForestNET.Tests.IO.Data.SimpleClass> a_dataOut =
            [
                new("Record #1 Value A", "Record #1 Value B", "Record #1 Value C"),
                new("Record #2 Value A", "Record #2 Value B", "", [1, 2, -3, -4]),
                new("Record #3; Value A", "null", "Record #3 Value C", [9, 8, -7, -6], new float[] { 42.0f, 21.25f, 54987.456999f }),
                new("Record 4 Value A", "Record $4 ;Value B \"", null, [16, 32, int.MaxValue, 128, 0], new float[] { 21.0f, 10.625f })
            ];

            ForestNET.Tests.IO.Data.SimpleClassCollection o_collectionOut = new(a_dataOut);

            string[] a_out = new string[a_dataOut.Count];
            string[] a_in = new string[a_dataOut.Count];
            int i_cnt = 0;

            foreach (ForestNET.Tests.IO.Data.SimpleClass o_simpleClassObject in a_dataOut)
            {
                a_out[i_cnt++] = o_simpleClassObject.ToString();
            }

            string s_resourcesDirectory = Environment.CurrentDirectory + ForestNET.Lib.IO.File.DIR + "Resources" + ForestNET.Lib.IO.File.DIR;
            string s_xsdSchemaFile = s_resourcesDirectory + "xml" + ForestNET.Lib.IO.File.DIR + p_s_xsdSchemaFileName;
            ForestNET.Lib.IO.XML o_xml = new(s_xsdSchemaFile)
            {
                UseProperties = true
            };

            string s_fileSimpleClass = p_s_testDirectory + "TestXMLSimpleClass" + p_s_fileNameSuffix + ".xml";

            if (p_s_xsdSchemaFileName.Equals("TestXSDSchemaSimpleClassArray.xsd"))
            {
                _ = o_xml.XmlEncode(a_dataOut, s_fileSimpleClass, true);
            }
            else
            {
                _ = o_xml.XmlEncode(o_collectionOut, s_fileSimpleClass, true);
            }

            ForestNET.Tests.IO.Data.SimpleClassCollection? o_collectionIn = null;
            List<ForestNET.Tests.IO.Data.SimpleClass>? a_dataIn;

            if (p_s_xsdSchemaFileName.Equals("TestXSDSchemaSimpleClassArray.xsd"))
            {
                List<ForestNET.Tests.IO.Data.SimpleClass>? a_foo = [.. ((System.Collections.IEnumerable)(o_xml.XmlDecode(s_fileSimpleClass) ?? new List<ForestNET.Tests.IO.Data.SimpleClass>())).Cast<ForestNET.Tests.IO.Data.SimpleClass>()];
                a_dataIn = a_foo;
            }
            else
            {
                o_collectionIn = (ForestNET.Tests.IO.Data.SimpleClassCollection?)o_xml.XmlDecode(s_fileSimpleClass);
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

            if (!p_s_xsdSchemaFileName.Equals("TestXSDSchemaSimpleClassArray.xsd"))
            {
                Assert.That(
                    ForestNET.Lib.Helper.ObjectsEqualUsingReflections(o_collectionOut, o_collectionIn, o_xml.UseProperties),
                    Is.True,
                    "output object inner class collection is not equal to input object inner class collection"
                );
            }

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_dataOut, a_dataIn, o_xml.UseProperties),
                Is.True,
                "output object inner class array is not equal to input object inner class array"
            );

            for (int i = 0; i < a_dataOut.Count; i++)
            {
                Assert.That(
                    ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_dataOut[i], a_dataIn?[i], o_xml.UseProperties, false, true),
                    Is.True,
                    "output object inner class array element object is not equal to input object inner class array element object"
                );

                Assert.That(
                    a_out[i], Has.Length.EqualTo(a_in[i].Length),
                    "output object inner class array element string length is not equal to input object inner class array element string length"
                );
            }
        }

        private static void XmlComplex(string p_s_testDirectory, bool p_b_classRoot, bool p_b_listRoot, bool p_b_listRootOnlyComplex, bool p_b_dividedClassRoot, bool p_b_dividedListRoot, bool p_b_dividedListRootOnlyComplex, bool p_b_dividedListRootOnlyComplexNoRef, string p_s_fileNameSuffix, bool p_b_useISO8601UTC)
        {
            List<ForestNET.Tests.IO.Data.ShipOrder> a_shipOrdersOut = ForestNET.Tests.IO.Data.GenerateData();

            /* Remove ShipFrom of 2nd ShipOrder-record in ShipMoreInfo, because xsd-schemas only accept two object within ShipMoreInfo */
            (a_shipOrdersOut[1].ShipMoreInfo ?? throw new NullReferenceException("ShipMoreInfo instance in 2nd ShipOrder-record is null")).ShipFrom = null;

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

            string s_xsdSchemaFile = "";

            string s_resourcesDirectory = Environment.CurrentDirectory + ForestNET.Lib.IO.File.DIR + "Resources" + ForestNET.Lib.IO.File.DIR;

            if (p_b_classRoot)
            {
                s_xsdSchemaFile = s_resourcesDirectory + "xml" + ForestNET.Lib.IO.File.DIR + "TestXSDSchemaClassRoot.xsd";
            }
            else if (p_b_listRoot)
            {
                s_xsdSchemaFile = s_resourcesDirectory + "xml" + ForestNET.Lib.IO.File.DIR + "TestXSDSchemaListRoot.xsd";
            }
            else if (p_b_listRootOnlyComplex)
            {
                s_xsdSchemaFile = s_resourcesDirectory + "xml" + ForestNET.Lib.IO.File.DIR + "TestXSDSchemaListRootOnlyComplex.xsd";
            }
            else if (p_b_dividedClassRoot)
            {
                s_xsdSchemaFile = s_resourcesDirectory + "xml" + ForestNET.Lib.IO.File.DIR + "TestXSDSchemaDividedClassRoot.xsd";
            }
            else if (p_b_dividedListRoot)
            {
                s_xsdSchemaFile = s_resourcesDirectory + "xml" + ForestNET.Lib.IO.File.DIR + "TestXSDSchemaDividedListRoot.xsd";
            }
            else if (p_b_dividedListRootOnlyComplex)
            {
                s_xsdSchemaFile = s_resourcesDirectory + "xml" + ForestNET.Lib.IO.File.DIR + "TestXSDSchemaDividedListRootOnlyComplex.xsd";
            }
            else if (p_b_dividedListRootOnlyComplexNoRef)
            {
                s_xsdSchemaFile = s_resourcesDirectory + "xml" + ForestNET.Lib.IO.File.DIR + "TestXSDSchemaDividedListRootOnlyComplexNoRef.xsd";
            }

            ForestNET.Lib.IO.XML o_xml = new(s_xsdSchemaFile)
            {
                UseProperties = true,
                UseISO8601UTC = p_b_useISO8601UTC

            };

            string s_file = p_s_testDirectory + "TestXML" + p_s_fileNameSuffix + ".xml";

            if ((p_b_classRoot) || (p_b_dividedClassRoot))
            {
                _ = o_xml.XmlEncode(o_shipOrderCollectionOut, s_file, true);
            }
            else
            {
                _ = o_xml.XmlEncode(a_shipOrdersOut, s_file, true);
            }

            List<ForestNET.Tests.IO.Data.ShipOrder>? a_shipOrdersIn = null;
            ForestNET.Tests.IO.Data.ShipOrderCollection? o_shipOrderCollectionIn = null;

            if ((p_b_classRoot) || (p_b_dividedClassRoot))
            {
                o_shipOrderCollectionIn = (ForestNET.Tests.IO.Data.ShipOrderCollection?)o_xml.XmlDecode(s_file);

                if (o_shipOrderCollectionIn != null)
                {
                    a_shipOrdersIn = o_shipOrderCollectionIn.ShipOrders;
                    o_shipOrderCollectionIn.ShipOrders = a_shipOrdersIn;
                    o_shipOrderCollectionIn.OrderAmount = a_shipOrdersIn.Count;
                }
            }
            else
            {
                List<ForestNET.Tests.IO.Data.ShipOrder> a_foo = [.. ((System.Collections.IEnumerable)(o_xml.XmlDecode(s_file) ?? new List<ForestNET.Tests.IO.Data.ShipOrder>())).Cast<ForestNET.Tests.IO.Data.ShipOrder>()];
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

            if ((p_b_classRoot) || (p_b_dividedClassRoot))
            {
                Assert.That(
                    ForestNET.Lib.Helper.ObjectsEqualUsingReflections(o_shipOrderCollectionOut, o_shipOrderCollectionIn, o_xml.UseProperties),
                    Is.True,
                    "output object inner class collection is not equal to input object inner class collection"
                );
            }

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_shipOrdersOut, a_shipOrdersIn, o_xml.UseProperties),
                Is.True,
                "output object inner class array is not equal to input object inner class array"
            );

            if (a_shipOrdersIn != null)
            {
                for (int i = 0; i < a_shipOrdersIn.Count; i++)
                {
                    Assert.That(
                        ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_shipOrdersOut[i], a_shipOrdersIn[i], o_xml.UseProperties, false, true),
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

        private static void XmlComplexPart(string p_s_testDirectory, bool p_b_dividedClassRoot, bool p_b_dividedListRoot, bool p_b_dividedListRootOnlyComplex, string p_s_fileNameSuffix, bool p_b_useISO8601UTC)
        {
            List<ForestNET.Tests.IO.Data.ShipOrder> a_shipOrdersOut = ForestNET.Tests.IO.Data.GenerateData();

            /* Remove ShipFrom of 2nd ShipOrder-record in ShipMoreInfo, because xsd-schemas only accept two object within ShipMoreInfo */
            (a_shipOrdersOut[1].ShipMoreInfo ?? throw new NullReferenceException("ShipMoreInfo instance in 2nd ShipOrder-record is null")).ShipFrom = null;

            ForestNET.Tests.IO.Data.ShipItem o_shipItemOut = a_shipOrdersOut[0].ShipItems[1];
            ForestNET.Tests.IO.Data.ShipMoreInfo o_shipMoreInfoOut = a_shipOrdersOut[1].ShipMoreInfo ?? throw new NullReferenceException("ShipMoreInfo instance in 2nd ShipOrder-record is null");
            List<ForestNET.Tests.IO.Data.ShipItem> a_shipItemsOut = a_shipOrdersOut[2].ShipItems;

            string s_xsdSchemaFile = "";
            string? s_out = null;
            string? s_in = null;

            string s_resourcesDirectory = Environment.CurrentDirectory + ForestNET.Lib.IO.File.DIR + "Resources" + ForestNET.Lib.IO.File.DIR;

            if (p_b_dividedClassRoot)
            {
                s_xsdSchemaFile = s_resourcesDirectory + "xml" + ForestNET.Lib.IO.File.DIR + "TestXSDSchemaDividedClassRoot.xsd";
                s_out = o_shipItemOut.ToString();
            }
            else if (p_b_dividedListRoot)
            {
                s_xsdSchemaFile = s_resourcesDirectory + "xml" + ForestNET.Lib.IO.File.DIR + "TestXSDSchemaDividedListRoot.xsd";
                s_out = o_shipMoreInfoOut.ToString();
            }
            else if (p_b_dividedListRootOnlyComplex)
            {
                s_xsdSchemaFile = s_resourcesDirectory + "xml" + ForestNET.Lib.IO.File.DIR + "TestXSDSchemaDividedListRootOnlyComplex.xsd";

                foreach (ForestNET.Tests.IO.Data.ShipItem o_foo in a_shipItemsOut)
                {
                    s_out += o_foo.ToString() + "|";
                }
            }

            ForestNET.Lib.IO.XML o_xml = new(s_xsdSchemaFile)
            {
                UseProperties = true,
                UseISO8601UTC = p_b_useISO8601UTC

            };

            string s_file = p_s_testDirectory + "TestXML" + p_s_fileNameSuffix + ".xml";

            if (p_b_dividedClassRoot)
            {
                _ = o_xml.XmlEncode(o_shipItemOut, s_file, true);
                o_xml.ValidateAgainstSchema(s_file);
                ForestNET.Tests.IO.Data.ShipItem? o_shipItemIn = (ForestNET.Tests.IO.Data.ShipItem?)o_xml.XmlDecode(s_file);
                s_in = o_shipItemIn?.ToString();

                Assert.That(
                    ForestNET.Lib.Helper.ObjectsEqualUsingReflections(o_shipItemOut, o_shipItemIn, o_xml.UseProperties),
                    Is.True,
                    "output object inner class is not equal to input object inner class"
                );
            }
            else if (p_b_dividedListRoot)
            {
                _ = o_xml.XmlEncode(o_shipMoreInfoOut, s_file, true);
                o_xml.ValidateAgainstSchema(s_file);
                ForestNET.Tests.IO.Data.ShipMoreInfo? o_shipMoreInfoIn = (ForestNET.Tests.IO.Data.ShipMoreInfo?)o_xml.XmlDecode(s_file);
                s_in = o_shipMoreInfoIn?.ToString();

                Assert.That(
                    ForestNET.Lib.Helper.ObjectsEqualUsingReflections(o_shipMoreInfoOut, o_shipMoreInfoIn, o_xml.UseProperties),
                    Is.True,
                    "output object inner class is not equal to input object inner class"
                );
            }
            else if (p_b_dividedListRootOnlyComplex)
            {
                _ = o_xml.XmlEncode(a_shipItemsOut, s_file, true);

                o_xml.ValidateAgainstSchema(s_file);
                List<ForestNET.Tests.IO.Data.ShipItem> a_shipItemsIn = [.. ((System.Collections.IEnumerable)(o_xml.XmlDecode(s_file) ?? new List<ForestNET.Tests.IO.Data.ShipItem>())).Cast<ForestNET.Tests.IO.Data.ShipItem>()];

                foreach (ForestNET.Tests.IO.Data.ShipItem o_foo in a_shipItemsIn)
                {
                    s_in += o_foo.ToString() + "|";
                }

                Assert.That(
                    ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_shipItemsOut, a_shipItemsIn, o_xml.UseProperties),
                    Is.True,
                    "output object inner class collection is not equal to input object inner class collection"
                );
            }

            /* we must delete unnecessary zeroes at the end of numeric values */
            s_in = DeleteUnnecessaryZeroes(s_in ?? "");

            if ((s_out != null) && (s_in != null))
            {
                Assert.That(
                    s_out, Has.Length.EqualTo(s_in.Length),
                    "output object string is not equal to input object string"
                );
            }
        }

        private static void XmlMultipleUseOfOneSchemaObject(string p_s_testDirectory)
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
            string s_xsdSchemaFile = s_resourcesDirectory + "xml" + ForestNET.Lib.IO.File.DIR + "TestXSDSchemaSimpleClassObjectOneReference.xsd";
            ForestNET.Lib.IO.XML o_xml = new(s_xsdSchemaFile)
            {
                UseProperties = true
            };

            string s_fileSimpleClass = p_s_testDirectory + "TestXMLSimpleClass_Y.xml";
            _ = o_xml.XmlEncode(o_collectionOut, s_fileSimpleClass, true);

            ForestNET.Tests.IO.Data.SimpleClassCollection? o_collectionIn;
            List<ForestNET.Tests.IO.Data.SimpleClass>? a_dataIn;

            o_collectionIn = (ForestNET.Tests.IO.Data.SimpleClassCollection?)o_xml.XmlDecode(s_fileSimpleClass);
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
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(o_collectionOut, o_collectionIn, o_xml.UseProperties),
                Is.True,
                "output object inner class collection is not equal to input object inner class collection"
            );

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_dataOut, a_dataIn, o_xml.UseProperties),
                Is.True,
                "output object inner class array is not equal to input object inner class array"
            );

            for (int i = 0; i < a_dataOut.Count; i++)
            {
                Assert.That(
                    ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_dataOut[i], a_dataIn?[i], o_xml.UseProperties, false, true),
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

            s_fileSimpleClass = p_s_testDirectory + "TestXMLSimpleClass_Z.xml";
            _ = o_xml.XmlEncode(o_collectionOut, s_fileSimpleClass, true);

            o_collectionIn = (ForestNET.Tests.IO.Data.SimpleClassCollection?)o_xml.XmlDecode(s_fileSimpleClass);
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
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(o_collectionOut, o_collectionIn, o_xml.UseProperties),
                Is.True,
                "output object inner class collection is not equal to input object inner class collection"
            );

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_dataOut, a_dataIn, o_xml.UseProperties),
                Is.True,
                "output object inner class array is not equal to input object inner class array"
            );

            for (int i = 0; i < a_dataOut.Count; i++)
            {
                Assert.That(
                    ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_dataOut[i], a_dataIn?[i], o_xml.UseProperties, false, true),
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

            /* Remove ShipFrom of 2nd ShipOrder-record in ShipMoreInfo, because xsd-schemas only accept two object within ShipMoreInfo */
            (a_shipOrdersOut[1].ShipMoreInfo ?? throw new NullReferenceException("ShipMoreInfo instance in 2nd ShipOrder-record is null")).ShipFrom = null;

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

            s_xsdSchemaFile = s_resourcesDirectory + "xml" + ForestNET.Lib.IO.File.DIR + "TestXSDSchemaDividedClassRoot.xsd";

            o_xml = new ForestNET.Lib.IO.XML(s_xsdSchemaFile)
            {
                UseProperties = true
            };

            string s_file = p_s_testDirectory + "TestXML_Alpha.xml";
            _ = o_xml.XmlEncode(o_shipOrderCollectionOut, s_file, true);

            List<ForestNET.Tests.IO.Data.ShipOrder>? a_shipOrdersIn = null;
            ForestNET.Tests.IO.Data.ShipOrderCollection? o_shipOrderCollectionIn;

            o_shipOrderCollectionIn = (ForestNET.Tests.IO.Data.ShipOrderCollection?)o_xml.XmlDecode(s_file);

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
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(o_shipOrderCollectionOut, o_shipOrderCollectionIn, o_xml.UseProperties),
                Is.True,
                "output object inner class collection is not equal to input object inner class collection"
            );

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_shipOrdersOut, a_shipOrdersIn, o_xml.UseProperties),
                Is.True,
                "output object inner class array is not equal to input object inner class array"
            );

            if (a_shipOrdersIn != null)
            {
                for (int i = 0; i < a_shipOrdersIn.Count; i++)
                {
                    Assert.That(
                        ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_shipOrdersOut[i], a_shipOrdersIn[i], o_xml.UseProperties, false, true),
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

            /* Remove ShipFrom of 2nd ShipOrder-record in ShipMoreInfo, because xsd-schemas only accept two object within ShipMoreInfo */
            (a_shipOrdersOut[1].ShipMoreInfo ?? throw new NullReferenceException("ShipMoreInfo instance in 2nd ShipOrder-record is null")).ShipFrom = null;

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

            s_file = p_s_testDirectory + "TestXML_Beta.xml";
            _ = o_xml.XmlEncode(o_shipOrderCollectionOut, s_file, true);

            o_shipOrderCollectionIn = (ForestNET.Tests.IO.Data.ShipOrderCollection?)o_xml.XmlDecode(s_file);

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
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(o_shipOrderCollectionOut, o_shipOrderCollectionIn, o_xml.UseProperties),
                Is.True,
                "output object inner class collection is not equal to input object inner class collection"
            );

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_shipOrdersOut, a_shipOrdersIn, o_xml.UseProperties),
                Is.True,
                "output object inner class array is not equal to input object inner class array"
            );

            if (a_shipOrdersIn != null)
            {
                for (int i = 0; i < a_shipOrdersIn.Count; i++)
                {
                    Assert.That(
                        ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_shipOrdersOut[i], a_shipOrdersIn[i], o_xml.UseProperties, false, true),
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
