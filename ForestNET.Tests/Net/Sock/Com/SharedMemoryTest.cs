namespace ForestNET.Tests.Net.Sock.Com
{
    public class SharedMemoryTest
    {
        [Test]
        public void TestSharedMemory()
        {
            try
            {
                TestConfig.InitiateTestLogging();

                SharedMemoryExample o_testSharedMemory = new();
                o_testSharedMemory.InitiateMirrors().Wait();

                /* ---------- ---------- ---------- ---------- ---------- */

                Assert.That(
                    o_testSharedMemory.ReturnFields(),
                    Is.EqualTo("BigInt = 0|Bool = False|Date = NULL|Decimal = 0|DoubleCol = 0|FloatValue = 0|Id = 0|Int = 0|LocalDate = NULL|LocalDateTime = NULL|LocalTime = NULL|ShortText = NULL|ShortText2 = NULL|SmallInt = 0|Text = NULL|Text2 = NULL|Time = NULL|Timestamp = NULL|UUID = NULL|"),
                    "shared memory ReturnFields returns wrong string"
                );

                /* ---------- ---------- ---------- ---------- ---------- */

                Assert.That(o_testSharedMemory.ReturnFieldName(8), Is.EqualTo("Int"), "ReturnFieldName('8') must be 'Int'");
                Assert.That(o_testSharedMemory.ReturnFieldNumber("Int"), Is.EqualTo(8), "ReturnFieldNumber('Int') must be '8'");
                Assert.That(o_testSharedMemory.ReturnFieldName(18), Is.EqualTo("Timestamp"), "ReturnFieldName('18') must be 'Timestamp'");
                Assert.That(o_testSharedMemory.ReturnFieldNumber("Timestamp"), Is.EqualTo(18), "ReturnFieldNumber('Timestamp') must be '18'");
                Assert.That(o_testSharedMemory.ReturnFieldName(1), Is.EqualTo("BigInt"), "ReturnFieldName('1') must be 'BigInt'");
                Assert.That(o_testSharedMemory.ReturnFieldNumber("BigInt"), Is.EqualTo(1), "ReturnFieldNumber('BigInt') must be '1'");

                /* ---------- ---------- ---------- ---------- ---------- */

                o_testSharedMemory.SetField("Text", "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.").Wait();
                o_testSharedMemory.SetField("Date", new DateTime(2020, 2, 2)).Wait();
                o_testSharedMemory.SetField("Int", 13579).Wait();

                List<string> a_changedFields = o_testSharedMemory.GetChangedFields(true).Result;

                Assert.That(a_changedFields, Has.Count.EqualTo(3), "changed fields amount is not '3', but '" + a_changedFields.Count + "'");

                int i = 0;

                foreach (string s_field in a_changedFields)
                {
                    if (i == 0)
                    {
                        Assert.That(s_field, Is.EqualTo("Date"), "Field is not 'Date', but '" + s_field + "'");
                        Assert.That(o_testSharedMemory.ReturnFieldNumber(s_field), Is.EqualTo(3), "Field number if 'Date' is not '3'");
                        Assert.That(o_testSharedMemory.GetField(s_field).Result, Is.EqualTo(new DateTime(2020, 2, 2)), "unexpected field value for 'Date'");
                    }
                    else if (i == 1)
                    {
                        Assert.That(s_field, Is.EqualTo("Int"), "Field is not 'Int', but '" + s_field + "'");
                        Assert.That(o_testSharedMemory.ReturnFieldNumber(s_field), Is.EqualTo(8), "Field number if 'Int' is not '8'");
                        Assert.That(o_testSharedMemory.GetField(s_field).Result, Is.EqualTo(13579), "unexpected field value for 'Int'");
                    }
                    else if (i == 2)
                    {
                        Assert.That(s_field, Is.EqualTo("Text"), "Field is not 'Text', but '" + s_field + "'");
                        Assert.That(o_testSharedMemory.ReturnFieldNumber(s_field), Is.EqualTo(15), "Field number if 'Text' is not '15'");
                        Assert.That(o_testSharedMemory.GetField(s_field).Result, Is.EqualTo("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."), "unexpected field value for 'Text'");
                    }

                    i++;
                }

                /* ---------- ---------- ---------- ---------- ---------- */

                o_testSharedMemory.SetField("SmallInt", (short)815).Wait();

                a_changedFields = o_testSharedMemory.GetChangedFields(true).Result;

                Assert.That(a_changedFields, Has.Count.EqualTo(1), "changed fields amount is not '1', but '" + a_changedFields.Count + "'");

                Assert.That(a_changedFields[0], Is.EqualTo("SmallInt"), "Field is not 'SmallInt', but '" + a_changedFields[0] + "'");
                Assert.That(o_testSharedMemory.ReturnFieldNumber(a_changedFields[0]), Is.EqualTo(14), "Field number if 'SmallInt' is not '14'");
                Assert.That(o_testSharedMemory.GetField(a_changedFields[0]).Result, Is.EqualTo(815), "unexpected field value for 'SmallInt'");

                /* ---------- ---------- ---------- ---------- ---------- */

                a_changedFields = o_testSharedMemory.GetChangedFields(true).Result;

                Assert.That(a_changedFields, Is.Empty, "changed fields amount is not '0', but '" + a_changedFields.Count + "'");

                /* ---------- ---------- ---------- ---------- ---------- */

                o_testSharedMemory.SetField("Id", (int)42).Wait();
                o_testSharedMemory.SetField("UUID", "8a9804b2-cbd0-11ec-9d64-0242ac120002").Wait();
                o_testSharedMemory.SetField("ShortText", "this is just a short text").Wait();
                o_testSharedMemory.SetField("BigInt", (long)154735207).Wait();
                o_testSharedMemory.SetField("Timestamp", new DateTime(2020, 2, 2, 22, 20, 12)).Wait();
                o_testSharedMemory.SetField("Time", new TimeSpan(22, 20, 12)).Wait();
                o_testSharedMemory.SetField("LocalDateTime", new DateTime(2019, 2, 2, 2, 2, 2)).Wait();
                o_testSharedMemory.SetField("LocalDate", new DateTime(2019, 2, 2)).Wait();
                o_testSharedMemory.SetField("LocalTime", new TimeSpan(2, 2, 2)).Wait();
                o_testSharedMemory.SetField("DoubleCol", 1337.0d).Wait();
                o_testSharedMemory.SetField("Decimal", 31415926.535897932384m).Wait();
                o_testSharedMemory.SetField("Bool", true).Wait();
                o_testSharedMemory.SetField("Text2", "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.").Wait();
                o_testSharedMemory.SetField("ShortText2", "another short text").Wait();
                o_testSharedMemory.SetField("FloatValue", 2.7182818284590f).Wait();

                a_changedFields = o_testSharedMemory.GetChangedFields(true, true).Result;

                Assert.That(a_changedFields, Has.Count.EqualTo(19), "changed fields amount is not '19', but '" + a_changedFields.Count + "'");

                i = 0;

                foreach (string s_field in a_changedFields)
                {
                    if (i == 0)
                    {
                        Assert.That(s_field, Is.EqualTo("BigInt"), "Field is not 'BigInt', but '" + s_field + "'");
                        Assert.That(o_testSharedMemory.ReturnFieldNumber(s_field), Is.EqualTo((i + 1)), "Field number if 'BigInt' is not '" + (i + 1) + "'");
                        Assert.That(o_testSharedMemory.GetField(s_field).Result, Is.EqualTo((long)154735207), "unexpected field value for 'BigInt'");
                    }
                    else if (i == 1)
                    {
                        Assert.That(s_field, Is.EqualTo("Bool"), "Field is not 'Bool', but '" + s_field + "'");
                        Assert.That(o_testSharedMemory.ReturnFieldNumber(s_field), Is.EqualTo((i + 1)), "Field number if 'Bool' is not '" + (i + 1) + "'");
                        Assert.That(o_testSharedMemory.GetField(s_field).Result, Is.True, "unexpected field value for 'Bool'");
                    }
                    else if (i == 2)
                    {
                        Assert.That(s_field, Is.EqualTo("Date"), "Field is not 'Date', but '" + s_field + "'");
                        Assert.That(o_testSharedMemory.ReturnFieldNumber(s_field), Is.EqualTo((i + 1)), "Field number if 'Date' is not '" + (i + 1) + "'");
                        Assert.That(o_testSharedMemory.GetField(s_field).Result, Is.EqualTo(new DateTime(2020, 2, 2)), "unexpected field value for 'Date'");
                    }
                    else if (i == 3)
                    {
                        Assert.That(s_field, Is.EqualTo("Decimal"), "Field is not 'Decimal', but '" + s_field + "'");
                        Assert.That(o_testSharedMemory.ReturnFieldNumber(s_field), Is.EqualTo((i + 1)), "Field number if 'Decimal' is not '" + (i + 1) + "'");
                        Assert.That(o_testSharedMemory.GetField(s_field).Result, Is.EqualTo(31415926.535897932384m), "unexpected field value for 'Decimal'");
                    }
                    else if (i == 4)
                    {
                        Assert.That(s_field, Is.EqualTo("DoubleCol"), "Field is not 'DoubleCol', but '" + s_field + "'");
                        Assert.That(o_testSharedMemory.ReturnFieldNumber(s_field), Is.EqualTo((i + 1)), "Field number if 'DoubleCol' is not '" + (i + 1) + "'");
                        Assert.That(o_testSharedMemory.GetField(s_field).Result, Is.EqualTo(1337.0d), "unexpected field value for 'DoubleCol'");
                    }
                    else if (i == 5)
                    {
                        Assert.That(s_field, Is.EqualTo("FloatValue"), "Field is not 'FloatValue', but '" + s_field + "'");
                        Assert.That(o_testSharedMemory.ReturnFieldNumber(s_field), Is.EqualTo((i + 1)), "Field number if 'FloatValue' is not '" + (i + 1) + "'");
                        Assert.That(o_testSharedMemory.GetField(s_field).Result, Is.EqualTo(2.7182818284590f), "unexpected field value for 'FloatValue'");
                    }
                    else if (i == 6)
                    {
                        Assert.That(s_field, Is.EqualTo("Id"), "Field is not 'Id', but '" + s_field + "'");
                        Assert.That(o_testSharedMemory.ReturnFieldNumber(s_field), Is.EqualTo((i + 1)), "Field number if 'Id' is not '" + (i + 1) + "'");
                        Assert.That(o_testSharedMemory.GetField(s_field).Result, Is.EqualTo(42), "unexpected field value for 'Id'");
                    }
                    else if (i == 7)
                    {
                        Assert.That(s_field, Is.EqualTo("Int"), "Field is not 'Int', but '" + s_field + "'");
                        Assert.That(o_testSharedMemory.ReturnFieldNumber(s_field), Is.EqualTo((i + 1)), "Field number if 'Text' is not '" + (i + 1) + "'");
                        Assert.That(o_testSharedMemory.GetField(s_field).Result, Is.EqualTo(13579), "unexpected field value for 'Int'");
                    }
                    else if (i == 8)
                    {
                        Assert.That(s_field, Is.EqualTo("LocalDate"), "Field is not 'LocalDate', but '" + s_field + "'");
                        Assert.That(o_testSharedMemory.ReturnFieldNumber(s_field), Is.EqualTo((i + 1)), "Field number if 'LocalDate' is not '" + (i + 1) + "'");
                        Assert.That(o_testSharedMemory.GetField(s_field).Result, Is.EqualTo(new DateTime(2019, 2, 2)), "unexpected field value for 'LocalDate'");
                    }
                    else if (i == 9)
                    {
                        Assert.That(s_field, Is.EqualTo("LocalDateTime"), "Field is not 'LocalDateTime', but '" + s_field + "'");
                        Assert.That(o_testSharedMemory.ReturnFieldNumber(s_field), Is.EqualTo((i + 1)), "Field number if 'LocalDateTime' is not '" + (i + 1) + "'");
                        Assert.That(o_testSharedMemory.GetField(s_field).Result, Is.EqualTo(new DateTime(2019, 2, 2, 2, 2, 2)), "unexpected field value for 'LocalDateTime'");
                    }
                    else if (i == 10)
                    {
                        Assert.That(s_field, Is.EqualTo("LocalTime"), "Field is not 'LocalTime', but '" + s_field + "'");
                        Assert.That(o_testSharedMemory.ReturnFieldNumber(s_field), Is.EqualTo((i + 1)), "Field number if 'LocalTime' is not '" + (i + 1) + "'");
                        Assert.That(o_testSharedMemory.GetField(s_field).Result, Is.EqualTo(new TimeSpan(2, 2, 2)), "unexpected field value for 'LocalTime'");
                    }
                    else if (i == 11)
                    {
                        Assert.That(s_field, Is.EqualTo("ShortText"), "Field is not 'ShortText', but '" + s_field + "'");
                        Assert.That(o_testSharedMemory.ReturnFieldNumber(s_field), Is.EqualTo((i + 1)), "Field number if 'ShortText' is not '" + (i + 1) + "'");
                        Assert.That(o_testSharedMemory.GetField(s_field).Result, Is.EqualTo("this is just a short text"), "unexpected field value for 'ShortText'");
                    }
                    else if (i == 12)
                    {
                        Assert.That(s_field, Is.EqualTo("ShortText2"), "Field is not 'ShortText2', but '" + s_field + "'");
                        Assert.That(o_testSharedMemory.ReturnFieldNumber(s_field), Is.EqualTo((i + 1)), "Field number if 'ShortText2' is not '" + (i + 1) + "'");
                        Assert.That(o_testSharedMemory.GetField(s_field).Result, Is.EqualTo("another short text"), "unexpected field value for 'ShortText2'");
                    }
                    else if (i == 13)
                    {
                        Assert.That(s_field, Is.EqualTo("SmallInt"), "Field is not 'SmallInt', but '" + s_field + "'");
                        Assert.That(o_testSharedMemory.ReturnFieldNumber(s_field), Is.EqualTo((i + 1)), "Field number if 'SmallInt' is not '" + (i + 1) + "'");
                        Assert.That(o_testSharedMemory.GetField(s_field).Result, Is.EqualTo((short)815), "unexpected field value for 'SmallInt'");
                    }
                    else if (i == 14)
                    {
                        Assert.That(s_field, Is.EqualTo("Text"), "Field is not 'Text', but '" + s_field + "'");
                        Assert.That(o_testSharedMemory.ReturnFieldNumber(s_field), Is.EqualTo((i + 1)), "Field number if 'Text' is not '" + (i + 1) + "'");
                        Assert.That(o_testSharedMemory.GetField(s_field).Result, Is.EqualTo("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."), "unexpected field value for 'Text'");
                    }
                    else if (i == 15)
                    {
                        Assert.That(s_field, Is.EqualTo("Text2"), "Field is not 'Text2', but '" + s_field + "'");
                        Assert.That(o_testSharedMemory.ReturnFieldNumber(s_field), Is.EqualTo((i + 1)), "Field number if 'Text2' is not '" + (i + 1) + "'");
                        Assert.That(o_testSharedMemory.GetField(s_field).Result, Is.EqualTo("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."), "unexpected field value for 'Text2'");
                    }
                    else if (i == 16)
                    {
                        Assert.That(s_field, Is.EqualTo("Time"), "Field is not 'Time', but '" + s_field + "'");
                        Assert.That(o_testSharedMemory.ReturnFieldNumber(s_field), Is.EqualTo((i + 1)), "Field number if 'Time' is not '" + (i + 1) + "'");
                        Assert.That(o_testSharedMemory.GetField(s_field).Result, Is.EqualTo(new TimeSpan(22, 20, 12)), "unexpected field value for 'Time'");
                    }
                    else if (i == 17)
                    {
                        Assert.That(s_field, Is.EqualTo("Timestamp"), "Field is not 'Timestamp', but '" + s_field + "'");
                        Assert.That(o_testSharedMemory.ReturnFieldNumber(s_field), Is.EqualTo((i + 1)), "Field number if 'Timestamp' is not '" + (i + 1) + "'");
                        Assert.That(o_testSharedMemory.GetField(s_field).Result, Is.EqualTo(new DateTime(2020, 2, 2, 22, 20, 12)), "unexpected field value for 'Timestamp'");
                    }
                    else if (i == 18)
                    {
                        Assert.That(s_field, Is.EqualTo("UUID"), "Field is not 'UUID', but '" + s_field + "'");
                        Assert.That(o_testSharedMemory.ReturnFieldNumber(s_field), Is.EqualTo((i + 1)), "Field number if 'UUID' is not '" + (i + 1) + "'");
                        Assert.That(o_testSharedMemory.GetField(s_field).Result, Is.EqualTo("8a9804b2-cbd0-11ec-9d64-0242ac120002"), "unexpected field value for 'UUID'");
                    }

                    i++;
                }
            }
            catch (Exception o_exc)
            {
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
        }
    }
}
