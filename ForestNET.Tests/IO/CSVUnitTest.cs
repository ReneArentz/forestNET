namespace ForestNET.Tests.IO
{
    public class CSVUnitTest
    {
        [Test]
        public void TestCSV()
        {
            try
            {
                TestConfig.InitiateTestLogging();

                string s_currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'");
                string s_file = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "file.csv";

                ForestNET.Lib.IO.CSV o_csv = new(";", "*");

                if (ForestNET.Lib.IO.File.Exists(s_file)) ForestNET.Lib.IO.File.DeleteFile(s_file);
                Assert.That(ForestNET.Lib.IO.File.Exists(s_file), Is.False, "file[" + s_file + "] does exist");

                CSVUnitTest.CSVClass(o_csv, s_file);

                if (ForestNET.Lib.IO.File.Exists(s_file)) ForestNET.Lib.IO.File.DeleteFile(s_file);
                Assert.That(ForestNET.Lib.IO.File.Exists(s_file), Is.False, "file[" + s_file + "] does exist");

                CSVUnitTest.CSVClassArray(o_csv, s_file);

                if (ForestNET.Lib.IO.File.Exists(s_file)) ForestNET.Lib.IO.File.DeleteFile(s_file);
                Assert.That(ForestNET.Lib.IO.File.Exists(s_file), Is.False, "file[" + s_file + "] does exist");

                CSVUnitTest.CSVInnerClass(o_csv, s_file);

                if (ForestNET.Lib.IO.File.Exists(s_file)) ForestNET.Lib.IO.File.DeleteFile(s_file);
                Assert.That(ForestNET.Lib.IO.File.Exists(s_file), Is.False, "file[" + s_file + "] does exist");

                CSVUnitTest.CSVInnerClassArray(o_csv, s_file);

                if (ForestNET.Lib.IO.File.Exists(s_file)) ForestNET.Lib.IO.File.DeleteFile(s_file);
                Assert.That(ForestNET.Lib.IO.File.Exists(s_file), Is.False, "file[" + s_file + "] does exist");

                CSVUnitTest.CSVOtherInnerClass(o_csv, s_file);

                if (ForestNET.Lib.IO.File.Exists(s_file)) ForestNET.Lib.IO.File.DeleteFile(s_file);
                Assert.That(ForestNET.Lib.IO.File.Exists(s_file), Is.False, "file[" + s_file + "] does exist");

                CSVUnitTest.CSVOtherInnerClassArray(o_csv, s_file);

                if (ForestNET.Lib.IO.File.Exists(s_file)) ForestNET.Lib.IO.File.DeleteFile(s_file);
                Assert.That(ForestNET.Lib.IO.File.Exists(s_file), Is.False, "file[" + s_file + "] does exist");
            }
            catch (Exception o_exc)
            {
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
        }

        private static void CSVClass(ForestNET.Lib.IO.CSV p_o_csv, string p_s_file)
        {
            ForestNET.Tests.SQL.AllTypesRecord o_recordOut = new()
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

            ForestNET.Lib.IO.File o_file = p_o_csv.EncodeCSV(o_recordOut, p_s_file, true, true);

            ForestNET.Tests.SQL.AllTypesRecord o_recordIn = new();
            p_o_csv.DecodeCSV(p_s_file, ref o_recordIn);

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(o_recordOut, o_recordIn, p_o_csv.UseProperties),
                Is.True,
                "output object class is not equal to input object class"
            );

            Assert.That(
                p_o_csv.PrintCSV(o_recordIn), Is.EqualTo(o_file.FileContent),
                "output object class csv string is not equal to input object class csv string"
            );
        }

        private static void CSVClassArray(ForestNET.Lib.IO.CSV p_o_csv, string p_s_file)
        {
            List<ForestNET.Tests.SQL.AllTypesRecord> a_recordsOut = [];

            ForestNET.Tests.SQL.AllTypesRecord o_recordTemp = new()
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
                ColumnLocalTime = default,
                ColumnLocalDate = new DateTime(2002, 2, 2),
                ColumnLocalDateTime = new DateTime(2019, 2, 2, 2, 2, 2),
                ColumnDoubleCol = 1.23456789d,
                ColumnDecimal = 12345678.9m,
                ColumnBool = true,
                ColumnText2 = "Das ist das Haus vom Nikolaus* #1",
                ColumnShortText2 = "Eins Datensatz"
            };

            a_recordsOut.Add(o_recordTemp);

            o_recordTemp = new ForestNET.Tests.SQL.AllTypesRecord
            {
                ColumnId = 2,
                ColumnUUID = "bb1824ce-72b8-4aad-bb12-dbc448682437",
                ColumnShortText = "Datensatz Zwei",
                ColumnText = "Die Handelsstreitigkeiten zwischen den; \"USA und China\" sorgen für eine Art Umdenken auf beiden Seiten. Während US-Unternehmen chinesische Hardware meiden, tun dies chinesische Unternehmen wohl mittlerweile auch: So denken laut einem Bericht der Nachrichtenagentur Bloomberg viele chinesische Hersteller stark darüber nach, ihre IT-Infrastruktur von lokalen Unternehmen statt von den US-Konzernen Oracle und IBM zu kaufen. Für diese Unternehmen sei der asiatische Markt wichtig. 16 respektive mehr als 20 Prozent des Umsatzes stammen aus dieser Region.",
                ColumnSmallInt = 2,
                ColumnInt = 20002,
                ColumnBigInt = 200002222,
                ColumnTimestamp = Convert.ToDateTime("02.02.2019 02:02:02"),
                ColumnDate = Convert.ToDateTime("02.02.2002"),
                ColumnTime = TimeSpan.Parse("02:02:02"),
                ColumnLocalTime = new TimeSpan(3, 3, 3),
                ColumnLocalDate = new DateTime(2003, 3, 3),
                ColumnLocalDateTime = default,
                ColumnDoubleCol = 12.3456789d,
                ColumnDecimal = 1234567.89m,
                ColumnBool = false,
                ColumnText2 = "Das ist das Haus vom Nikolaus* #2",
                ColumnShortText2 = "Zwei Datensatz"
            };

            a_recordsOut.Add(o_recordTemp);

            o_recordTemp = new ForestNET.Tests.SQL.AllTypesRecord
            {
                ColumnId = 3,
                ColumnUUID = "cb1824ce-72b8-4aad-bb12-dbc448682437",
                ColumnShortText = "Datensatz Drei",
                ColumnText = "Die Handelsstreitigkeiten zwischen den; \"USA und China\" sorgen für eine Art Umdenken auf beiden Seiten. Während US-Unternehmen chinesische Hardware meiden, tun dies chinesische Unternehmen wohl mittlerweile auch: So denken laut einem Bericht der Nachrichtenagentur Bloomberg viele chinesische Hersteller stark darüber nach, ihre IT-Infrastruktur von lokalen Unternehmen statt von den US-Konzernen Oracle und IBM zu kaufen. Für diese Unternehmen sei der asiatische Markt wichtig. 16 respektive mehr als 20 Prozent des Umsatzes stammen aus dieser Region.",
                ColumnSmallInt = 3,
                ColumnInt = 30003,
                ColumnBigInt = 300003333,
                ColumnTimestamp = Convert.ToDateTime("03.03.2019 03:03:03"),
                ColumnDate = Convert.ToDateTime("03.03.2003"),
                ColumnTime = TimeSpan.Parse("03:03:03"),
                ColumnLocalTime = new TimeSpan(4, 4, 4),
                ColumnLocalDate = default,
                ColumnLocalDateTime = new DateTime(2019, 4, 4, 4, 4, 4),
                ColumnDoubleCol = 123.456789d,
                ColumnDecimal = 123456.789m,
                ColumnBool = true,
                ColumnText2 = "Das ist das Haus vom Nikolaus* #3",
                ColumnShortText2 = "Drei Datensatz"
            };

            a_recordsOut.Add(o_recordTemp);

            p_o_csv.ReturnArrayElementNullNotZero = true;

            ForestNET.Lib.IO.File o_file = p_o_csv.EncodeCSV(a_recordsOut, p_s_file, true, true);

            List<ForestNET.Tests.SQL.AllTypesRecord> a_recordsIn = [];
            p_o_csv.DecodeCSV(p_s_file, ref a_recordsIn);

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_recordsOut, a_recordsIn, p_o_csv.UseProperties),
                Is.True,
                "output object class array is not equal to input object class array"
            );

            Assert.That(
                o_file.FileContent, Has.Length.EqualTo(p_o_csv.PrintCSV(a_recordsIn).Length),
                "output object class array csv string length is not equal to input object class array csv string length"
            );

            p_o_csv.ReturnArrayElementNullNotZero = false;
        }

        private static void CSVInnerClass(ForestNET.Lib.IO.CSV p_o_csv, string p_s_file)
        {
            ForestNET.Tests.IO.Data.ShipItem o_shipItemOut = new()
            {
                Title = "Item #1",
                Note = "high; value",
                ManufacturedTime = new TimeSpan(12, 6, 3),
                Quantity = 2,
                Price = 500.2m,
                Currency = "USD",
                Skonto = 12.50d,
                SomeDecimals =
                new decimal[] {
                    1.602176634m,
                    8.8541878128m,
                    6.62607015m,
                    9.80665m,
                    3.14159265359m
                },
                ShipItemInfo = new ForestNET.Tests.IO.Data.ShipItemInfo()
            };
            o_shipItemOut.ShipItemInfo.Development = "Development 2.2";
            o_shipItemOut.ShipItemInfo.Implementation = "Implementation 2.2";

            p_o_csv.UseProperties = true;

            ForestNET.Lib.IO.File o_file = p_o_csv.EncodeCSV(o_shipItemOut, p_s_file, true, true);

            ForestNET.Tests.IO.Data.ShipItem o_shipItemIn = new();
            p_o_csv.DecodeCSV(p_s_file, ref o_shipItemIn);

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(o_shipItemOut, o_shipItemIn, p_o_csv.UseProperties),
                Is.True,
                "output object inner class is not equal to input object inner class"
            );

            Assert.That(
                p_o_csv.PrintCSV(o_shipItemIn), Is.EqualTo(o_file.FileContent),
                "output object inner class csv string is not equal to input object inner class csv string"
            );

            p_o_csv.UseProperties = false;
        }

        private static void CSVInnerClassArray(ForestNET.Lib.IO.CSV p_o_csv, string p_s_file)
        {
            List<ForestNET.Tests.IO.Data.SimpleClass> a_dataOut =
            [
                new("Record #1 Value A", "Record #1 Value B", "Record #1 Value C"),
                new("Record #2 Value A", "Record #2 Value B", "", [1, 2, -3, -4]),
                new("\"Record #3; Value A", "null", "Record #3 Value C", [9, 8, -7, -6], new float[] { 42.0f, 21.25f, 54987.456999f }),
                new("Record #4 Value A", "Record $4 ;Value B \"", null, [16, 32, 0, 128, 0])
            ];

            p_o_csv.ReturnArrayElementNullNotZero = true;
            p_o_csv.UseProperties = true;

            ForestNET.Lib.IO.File o_file = p_o_csv.EncodeCSV(a_dataOut, p_s_file, true, true);

            List<ForestNET.Tests.IO.Data.SimpleClass> a_dataIn = [];
            p_o_csv.DecodeCSV(p_s_file, ref a_dataIn);

            /* float precision is really bad */
            if (a_dataOut[2].ValueE != null)
            {
                float[] a_foo = a_dataOut[2].ValueE ?? [];

                if (a_foo.Length > 2)
                {
                    a_foo[2] = 54987.46f;
                }
            }

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_dataOut, a_dataIn, p_o_csv.UseProperties),
                Is.True,
                "output object inner class array is not equal to input object inner class array"
            );

            Assert.That(
                p_o_csv.PrintCSV(a_dataIn), Is.EqualTo(o_file.FileContent),
                "output object inner class array csv string is not equal to input object inner class array csv string"
            );

            p_o_csv.ReturnArrayElementNullNotZero = false;
            p_o_csv.UseProperties = false;
        }

        private static void CSVOtherInnerClass(ForestNET.Lib.IO.CSV p_o_csv, string p_s_file)
        {
            ForestNET.Tests.IO.Data.ShipOrder o_shipOrderOut = new()
            {
                OrderId = "ORD0001",
                OrderPerson = "Jon Doe",
                OrderDate = new DateTime(2020, 1, 25),
                OverallPrice = 388.95f,
                SomeBools = new bool[] { false, true, false }
            };

            p_o_csv.UseProperties = true;

            ForestNET.Lib.IO.File o_file = p_o_csv.EncodeCSV(o_shipOrderOut, p_s_file, true, true);

            Data.ShipOrder o_shipOrderIn = new();
            p_o_csv.DecodeCSV(p_s_file, ref o_shipOrderIn);

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(o_shipOrderOut, o_shipOrderIn, p_o_csv.UseProperties),
                Is.True,
                "output object other inner class is not equal to input object other inner class"
            );

            Assert.That(
                p_o_csv.PrintCSV(o_shipOrderIn), Is.EqualTo(o_file.FileContent),
                "output object other inner class csv string is not equal to input object other inner class csv string"
            );

            p_o_csv.UseProperties = false;
        }

        private static void CSVOtherInnerClassArray(ForestNET.Lib.IO.CSV p_o_csv, string p_s_file)
        {
            ForestNET.Tests.IO.Data.ShipItem o_shipItem1 = new()
            {
                Title = "Item #1",
                Note = "----",
                ManufacturedTime = new TimeSpan(13, 31, 0),
                Quantity = 15,
                Price = 5.25m,
                Currency = "EUR",
                Skonto = 2.15d,
                ShipItemInfo = new ForestNET.Tests.IO.Data.ShipItemInfo()
            };
            o_shipItem1.ShipItemInfo.Development = "Development 1.1";

            ForestNET.Tests.IO.Data.ShipItem o_shipItem2 = new()
            {
                Title = "Item #2",
                Note = "be careful",
                ManufacturedTime = new TimeSpan(20, 15, 33),
                Quantity = 35,
                Price = 1.88m,
                Currency = "EUR",
                Skonto = 5.00d,
                ShipItemInfo = new ForestNET.Tests.IO.Data.ShipItemInfo()
            };
            o_shipItem2.ShipItemInfo.Development = "Development 1.2";
            o_shipItem2.ShipItemInfo.Implementation = "Implementation 1.2";

            ForestNET.Tests.IO.Data.ShipItem o_shipItem3 = new()
            {
                Title = "Item #3",
                Note = "store cold",
                ManufacturedTime = new TimeSpan(3, 7, 12),
                Quantity = 5,
                Price = 12.23m,
                Currency = "USD",
                Skonto = 7.86d,
                ShipItemInfo = new ForestNET.Tests.IO.Data.ShipItemInfo()
            };
            o_shipItem3.ShipItemInfo.Construction = "Construction 1.3";

            List<ForestNET.Tests.IO.Data.ShipItem> a_dataShipItemOut =
            [
                o_shipItem1,
                o_shipItem2,
                o_shipItem3
            ];

            p_o_csv.UseProperties = true;

            ForestNET.Lib.IO.File o_file = p_o_csv.EncodeCSV(a_dataShipItemOut, p_s_file, true, true);

            List<ForestNET.Tests.IO.Data.ShipItem> a_dataShipItemIn = [];
            p_o_csv.DecodeCSV(p_s_file, ref a_dataShipItemIn);

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(a_dataShipItemOut, a_dataShipItemIn, p_o_csv.UseProperties),
                Is.True,
                "output object other inner class array is not equal to input object other inner class array"
            );

            Assert.That(
                p_o_csv.PrintCSV(a_dataShipItemIn), Is.EqualTo(o_file.FileContent),
                "output object other inner class array csv string is not equal to input object other inner class array csv string"
            );

            p_o_csv.UseProperties = false;
        }
    }
}
