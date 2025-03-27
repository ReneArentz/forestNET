using System.Text;

namespace ForestNET.Tests.Core
{
    public class HelperUnitTest
    {
        [Test]
        public void TestHelper()
        {
            /* log some info -> Microsoft.VisualStudio.TestTools.UnitTesting.Logging.Logger.LogMessage("text I want to log", new object[] { }); */

            Assert.That(
                ForestNET.Lib.Helper.IsStringEmpty(null),
                Is.True,
                "null is not string empty"
            );
            Assert.That(
                ForestNET.Lib.Helper.IsStringEmpty(""),
                Is.True,
                "empty string is not string empty"
            );
            Assert.That(
                ForestNET.Lib.Helper.IsStringEmpty("  "),
                Is.True,
                "two white spaces is not string empty"
            );
            Assert.That(
                ForestNET.Lib.Helper.IsStringEmpty("notEmpty"),
                Is.False,
                "'notEmpty' is string empty"
            );

            Assert.That(
                ForestNET.Lib.Helper.IsString("test"),
                Is.True,
                "'test' is not string"
            );
            Assert.That(
                ForestNET.Lib.Helper.IsString(123),
                Is.False,
                "123 is a string"
            );

            Assert.That(
                ForestNET.Lib.Helper.IsShort("test"),
                Is.False,
                "'test' is a short"
            );
            Assert.That(
                ForestNET.Lib.Helper.IsShort("123"),
                Is.True,
                "123 is not a short"
            );

            Assert.That(
                ForestNET.Lib.Helper.IsInteger("test"),
                Is.False,
                "'test' is an integer"
            );
            Assert.That(
                ForestNET.Lib.Helper.IsInteger("123550"),
                Is.True,
                "1234550 is not an integer"
            );

            Assert.That(
                ForestNET.Lib.Helper.IsLong("test"),
                Is.False,
                "'test' is a long"
            );
            Assert.That(
                ForestNET.Lib.Helper.IsLong("1235464545464654550"),
                Is.True,
                "1235464545464654550 is not a long"
            );

            Assert.That(
                ForestNET.Lib.Helper.IsFloat("test"),
                Is.False,
                "'test' is a float"
            );
            Assert.That(
                ForestNET.Lib.Helper.IsFloat("1235464.454644545464654550"),
                Is.True,
                "1235464.454644545464654550 is no a float"
            );

            Assert.That(
                ForestNET.Lib.Helper.IsDouble("test"),
                Is.False,
                "'test' is a double"
            );
            Assert.That(
                ForestNET.Lib.Helper.IsDouble("12354645.45446445464654550"),
                Is.True,
                "12354645.45446445464654550 is not a double"
            );

            Assert.That(
                ForestNET.Lib.Helper.IsBoolean("1"),
                Is.False,
                "'1' is boolean true"
            );
            Assert.That(
                ForestNET.Lib.Helper.IsBoolean("true"),
                Is.True,
                "'true' is not boolean true"
            );

            Assert.That(
                ForestNET.Lib.Helper.MatchesRegex("123e4567-e89b-12Q3-a456-426614174000", "^[a-f0-9\\-]*$"),
                Is.False,
                "'123e4567-e89b-12___Q___3-a456-426614174000' matches regex '" + "^[a-f0-9\\-]*$" + "'"
            );
            Assert.That(
                ForestNET.Lib.Helper.MatchesRegex("123e4567-e89b-12d3-a456-426614174000", "^[a-f0-9\\-]*$"),
                Is.True,
                "'123e4567-e89b-12d3-a456-426614174000' not matches regex '" + "^[a-f0-9\\-]*$" + "'"
            );

            Assert.That(
                ForestNET.Lib.Helper.CountSubStrings("HelloabcdefgHelloabcdefgHelloHello", "Hallo"), Is.Not.EqualTo(4),
                "'Hallo' is 4 times in 'HelloabcdefgHelloabcdefgHelloHello'"
            );
            Assert.That(
                ForestNET.Lib.Helper.CountSubStrings("HelloabcdefgHelloabcdefgHelloHello", "Hello"), Is.EqualTo(4),
                "'Hello' is not 4 times in 'HelloabcdefgHelloabcdefgHelloHello'"
            );

            Assert.That(
                ForestNET.Lib.Helper.PowIntegers(2, 16), Is.EqualTo(65536),
                "Exponentiation of 2^16 is not 65536"
            );

            Assert.That(
                ForestNET.Lib.Helper.PowIntegers(12, 5), Is.EqualTo(248832),
                "Exponentiation of 12^5 is not 248832"
            );

            Assert.That(
                ForestNET.Lib.Helper.IsDate("31-03-2020"),
                Is.True,
                "'31-03-2020' is not a date"
            );
            Assert.That(
                ForestNET.Lib.Helper.IsDate("03-31-2020"),
                Is.False,
                "'03-31-2020' is a date"
            );
            Assert.That(
                ForestNET.Lib.Helper.IsDate("31.03.2020"),
                Is.True,
                "'31.03.2020' is not a date"
            );
            Assert.That(
                ForestNET.Lib.Helper.IsDate("03.31.2020"),
                Is.False,
                "'03.31.2020' is a date"
            );
            Assert.That(
                ForestNET.Lib.Helper.IsDate("31/03/2020"),
                Is.True,
                "'31/03/2020' is not a date"
            );
            Assert.That(
                ForestNET.Lib.Helper.IsDate("31/13/2020"),
                Is.False,
                "'31/13/2020' is a date"
            );
            Assert.That(
                ForestNET.Lib.Helper.IsDate("2020/03/31"),
                Is.True,
                "'2020/03/31' is not a date"
            );
            Assert.That(
                ForestNET.Lib.Helper.IsDate("2020/13/31"),
                Is.False,
                "'2020/13/31' is a date"
            );
            Assert.That(
                ForestNET.Lib.Helper.IsDate("2020-03-31"),
                Is.True,
                "'2020-03-31' is not a date"
            );
            Assert.That(
                ForestNET.Lib.Helper.IsDate("2020-31-03"),
                Is.False,
                "'2020-31-03' is a date"
            );
            Assert.That(
                ForestNET.Lib.Helper.IsDate("03/31/2020"),
                Is.True,
                "'03/31/2020' is not a date"
            );
            Assert.That(
                ForestNET.Lib.Helper.IsDate("13/31/2020"),
                Is.False,
                "'13/31/2020' is a date"
            );
            Assert.That(
                ForestNET.Lib.Helper.IsDate("2020/31/03"),
                Is.True,
                "'2020/31/03' is not a date"
            );
            Assert.That(
                ForestNET.Lib.Helper.IsDate("2020/31/13"),
                Is.False,
                "'2020/31/13' is a date"
            );

            Assert.That(
                ForestNET.Lib.Helper.IsTime("53:45:32"),
                Is.False,
                "'53:45:32' is a time"
            );
            Assert.That(
                ForestNET.Lib.Helper.IsTime("13:45:32"),
                Is.True,
                "'13:45:32' is not a time"
            );

            string[] a_testTrue = new string[] {
                "31-03-2020 13:45:32",
                "31-03-2020T13:45:32",
                "31-03-2020 13:45:32.998",
                "31-03-2020T13:45:32.332",
                "31-03-2020 13:45:32Z",
                "31-03-2020T13:45:32Z",
                "31-03-2020 13:45",
                "31.03.2020 13:45:32",
                "31.03.2020 13:45:32.998",
                "31.03.2020 13:45",
                "31/03/2020 13:45:32",
                "31/03/2020 13:45:32.332",
                "31/03/2020 13:45",
                "2020/03/31 13:45:32",
                "2020/03/31 13:45:32.998",
                "2020/03/31 13:45",
                "2020-03-31 13:45:32",
                "2020-03-31 13:45:32.332",
                "2020-03-31 13:45",
                "03/31/2020 13:45:32",
                "03/31/2020 13:45:32.998",
                "03/31/2020 13:45",
                "2020/31/03 13:45:32",
                "2020/31/03 13:45:32.332",
                "2020/31/03 13:45"
            };

            foreach (string s_testTrue in a_testTrue)
            {
                Assert.That(
                    ForestNET.Lib.Helper.IsDateTime(s_testTrue),
                    Is.True,
                    "'" + s_testTrue + "' is not a date time"
                );
            }

            string[] a_testFalse = new string[] {
                "31-03-202013:45:32",
                "03-31-2020 13:45:32",
                "03-31-2020 13:45",
                "31-03-2020 53:45:32",
                "03.31.2020 13:45:32",
                "03.31.2020 13:45",
                "31.03.2020 53:45:32",
                "31/13/2020 13:45:32",
                "31/13/2020 13:45",
                "31/03/2020 53:45:32",
                "2020/13/31 13:45:32",
                "2020/13/31 13:45",
                "2020/03/31 53:45:32",
                "2020-31-03 13:45:32",
                "2020-31-03 13:45",
                "2020-03-31 53:45:32",
                "13/31/2020 13:45:32",
                "13/31/2020 13:45",
                "03/31/2020 53:45:32",
                "2020/31/13 13:45:32",
                "2020/31/13 13:45",
                "2020/31/03 53:45:32",
                "03/31/2020 53:45:32.998",
                "2020/31/13 13:45:32.332",
            };

            foreach (string s_testFalse in a_testFalse)
            {
                Assert.That(
                    ForestNET.Lib.Helper.IsDateTime(s_testFalse),
                    Is.False,
                    "'" + s_testFalse + "' is a date time"
                );
            }

            Assert.That(
                ForestNET.Lib.Helper.IsDateInterval("P2DQ6Y"),
                Is.False,
                "'P2DQ6Y' is a date interval"
            );
            Assert.That(
                ForestNET.Lib.Helper.IsDateInterval("P4Y"),
                Is.True,
                "'P4Y' is not a date interval"
            );
            Assert.That(
                ForestNET.Lib.Helper.IsDateInterval("PT15S"),
                Is.True,
                "'PT15S' is not a date interval"
            );
            Assert.That(
                ForestNET.Lib.Helper.IsDateInterval("P2DT2H3M55S"),
                Is.True,
                "'P2DT2H3M55S' is not a date interval"
            );

            try
            {
                bool b_isDaySavingTime = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);

                DateTime o_localDateTime = TimeZoneInfo.ConvertTime(TimeZoneInfo.ConvertTimeToUtc(new DateTime(2020, 03, 14, 06, 02, 03, DateTimeKind.Unspecified), TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time")), TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time"));
                Assert.That(
                    ForestNET.Lib.Helper.ToISO8601UTC(o_localDateTime), Is.EqualTo("2020-03-14T05:02:03Z"),
                    "local date time object[" + ForestNET.Lib.Helper.ToISO8601UTC(o_localDateTime) + "] is not equal with toISO8601UTC to '2020-03-14T05:02:03Z'"
                );
                Assert.That(
                    ForestNET.Lib.Helper.ToISO8601UTC(o_localDateTime), Is.Not.EqualTo("2030-03-14T05:02:03Z"),
                    "local date time object[" + ForestNET.Lib.Helper.ToISO8601UTC(o_localDateTime) + "] is equal with toISO8601UTC to '2030-03-14T05:02:03Z'"
                );

                Assert.That(
                    ForestNET.Lib.Helper.ToRFC1123(o_localDateTime), Is.EqualTo("Sat, 14 Mar 2020 05:02:03 GMT"),
                    "local date time object[" + ForestNET.Lib.Helper.ToRFC1123(o_localDateTime) + "] is not equal with toRFC1123 to 'Sat, 14 Mar 2020 05:02:03 GMT'"
                );
                Assert.That(
                    ForestNET.Lib.Helper.ToRFC1123(o_localDateTime), Is.Not.EqualTo("Sat, 14 Mar 2030 05:02:03 GMT"),
                    "local date time object[" + ForestNET.Lib.Helper.ToRFC1123(o_localDateTime) + "] is equal with toRFC1123 to 'Sat, 14 Mar 2030 05:02:03 GMT'"
                );

                if (b_isDaySavingTime)
                {
                    /* o_localDateTime = o_localDateTime.AddHours(-1); */
                }

                Assert.That(
                    ForestNET.Lib.Helper.ToDateTimeString(o_localDateTime), Is.EqualTo("2020-03-14T06:02:03"),
                    "local date time object[" + ForestNET.Lib.Helper.ToDateTimeString(o_localDateTime) + "] is not equal with toDateTimeString to '2020-03-14T06:02:03'"
                );
                Assert.That(
                    ForestNET.Lib.Helper.ToDateTimeString(o_localDateTime), Is.Not.EqualTo("2030-03-14T06:02:03"),
                    "local date time object[" + ForestNET.Lib.Helper.ToDateTimeString(o_localDateTime) + "] is equal with toDateTimeString to '2030-03-14T06:02:03'"
                );

                o_localDateTime = TimeZoneInfo.ConvertTime(TimeZoneInfo.ConvertTimeToUtc(new DateTime(2020, 03, 14, 06, 02, 00, DateTimeKind.Unspecified), TimeZoneInfo.Local), TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time"));
                Assert.That(
                    ForestNET.Lib.Helper.ToISO8601UTC(o_localDateTime), Is.EqualTo("2020-03-14T05:02:00Z"),
                    "local date time object[" + ForestNET.Lib.Helper.ToISO8601UTC(o_localDateTime) + "] is not equal with toISO8601UTC to '2020-03-14T05:02:00Z'"
                );
                Assert.That(
                    ForestNET.Lib.Helper.ToISO8601UTC(o_localDateTime), Is.Not.EqualTo("2030-03-14T05:02:00Z"),
                    "local date time object[" + ForestNET.Lib.Helper.ToISO8601UTC(o_localDateTime) + "] is equal with toISO8601UTC to '2030-03-14T05:02:00Z'"
                );

                Assert.That(
                    ForestNET.Lib.Helper.ToRFC1123(o_localDateTime), Is.EqualTo("Sat, 14 Mar 2020 05:02:00 GMT"),
                    "local date time object[" + ForestNET.Lib.Helper.ToRFC1123(o_localDateTime) + "] is not equal with toRFC1123 to 'Sat, 14 Mar 2020 05:02:00 GMT'"
                );
                Assert.That(
                    ForestNET.Lib.Helper.ToRFC1123(o_localDateTime), Is.Not.EqualTo("Sat, 14 Mar 2030 05:02:00 GMT"),
                    "local date time object[" + ForestNET.Lib.Helper.ToRFC1123(o_localDateTime) + "] is equal with toRFC1123 to 'Sat, 14 Mar 2030 05:02:00 GMT'"
                );

                if (b_isDaySavingTime)
                {
                    /* o_localDateTime = o_localDateTime.AddHours(-1); */
                }

                Assert.That(
                    ForestNET.Lib.Helper.ToDateTimeString(o_localDateTime), Is.EqualTo("2020-03-14T06:02:00"),
                    "local date time object[" + ForestNET.Lib.Helper.ToDateTimeString(o_localDateTime) + "] is not equal with toDateTimeString to '2020-03-14T06:02:00'"
                );
                Assert.That(
                    ForestNET.Lib.Helper.ToDateTimeString(o_localDateTime), Is.Not.EqualTo("2030-03-14T06:02:00"),
                    "local date time object[" + ForestNET.Lib.Helper.ToDateTimeString(o_localDateTime) + "] is equal with toDateTimeString to '2030-03-14T06:02:00'"
                );
            }
            catch (Exception o_exc)
            {
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }

            try
            {
                DateTime[] a_validLocalDateTime = new DateTime[] {
                    TimeZoneInfo.ConvertTime(TimeZoneInfo.ConvertTimeToUtc(new DateTime(2020, 03, 14, 06, 02, 00, DateTimeKind.Unspecified), TimeZoneInfo.Local), TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time")),
                    TimeZoneInfo.ConvertTime(TimeZoneInfo.ConvertTimeToUtc(new DateTime(2020, 03, 14, 06, 02, 00, DateTimeKind.Unspecified), TimeZoneInfo.Local), TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time")),
                    TimeZoneInfo.ConvertTime(TimeZoneInfo.ConvertTimeToUtc(new DateTime(2020, 03, 14, 06, 02, 03, DateTimeKind.Unspecified), TimeZoneInfo.Local), TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time")),
                    TimeZoneInfo.ConvertTime(TimeZoneInfo.ConvertTimeToUtc(new DateTime(2020, 03, 14, 06, 02, 03, DateTimeKind.Unspecified), TimeZoneInfo.Local), TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time")),
                    TimeZoneInfo.ConvertTime(TimeZoneInfo.ConvertTimeToUtc(new DateTime(2020, 03, 14, 06, 02, 03, DateTimeKind.Unspecified), TimeZoneInfo.Local), TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time"))
                };

                a_testTrue = new string[] {
                    "14-03-2020 05:02",
                    "14-03-2020T05:02",
                    "14-03-2020 05:02:03",
                    "14-03-2020T05:02:03",
                    "14-03-2020T05:02:03Z",
                    "14.03.2020 05:02",
                    "14.03.2020T05:02",
                    "14.03.2020 05:02:03",
                    "14.03.2020T05:02:03",
                    "14.03.2020T05:02:03Z",
                    "14/03/2020 05:02",
                    "14/03/2020T05:02",
                    "14/03/2020 05:02:03",
                    "14/03/2020T05:02:03",
                    "14/03/2020T05:02:03Z",
                    "03/14/2020 05:02",
                    "03/14/2020T05:02",
                    "03/14/2020 05:02:03",
                    "03/14/2020T05:02:03",
                    "03/14/2020T05:02:03Z",
                    "2020-03-14 05:02",
                    "2020-03-14T05:02",
                    "2020-03-14 05:02:03",
                    "2020-03-14T05:02:03",
                    "2020-03-14T05:02:03Z",
                    "2020/03/14 05:02",
                    "2020/03/14T05:02",
                    "2020/03/14 05:02:03",
                    "2020/03/14T05:02:03",
                    "2020/03/14T05:02:03Z",
                    "2020/14/03 05:02",
                    "2020/14/03T05:02",
                    "2020/14/03 05:02:03",
                    "2020/14/03T05:02:03",
                    "2020/14/03T05:02:03Z"
                };

                int i = 0;

                foreach (string s_testTrue in a_testTrue)
                {
                    Assert.That(
                        a_validLocalDateTime[i], Is.EqualTo(ForestNET.Lib.Helper.FromISO8601UTC(s_testTrue)),
                        "'" + s_testTrue + "' fromISO8601UTC() is not equal local date tine object '" + a_validLocalDateTime[i] + "'"
                    );

                    Assert.That(
                        a_validLocalDateTime[i].AddHours(-1), Is.EqualTo(ForestNET.Lib.Helper.FromDateTimeString(s_testTrue)),
                        "'" + s_testTrue + "' fromDateTimeString() is not equal local date tine object '" + a_validLocalDateTime[i] + "'"
                    );

                    if (i == 4)
                    {
                        i = 0;
                    }
                    else
                    {
                        i++;
                    }
                }

                DateTime[] o_validLocalDates = new DateTime[] {
                    TimeZoneInfo.ConvertTime(TimeZoneInfo.ConvertTimeToUtc(new DateTime(2020, 03, 14, 00, 00, 00, DateTimeKind.Unspecified), TimeZoneInfo.Local), TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time")),
                    TimeZoneInfo.ConvertTime(TimeZoneInfo.ConvertTimeToUtc(new DateTime(2020, 03, 14, 00, 00, 00, DateTimeKind.Unspecified), TimeZoneInfo.Local), TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time")),
                    TimeZoneInfo.ConvertTime(TimeZoneInfo.ConvertTimeToUtc(new DateTime(2020, 03, 14, 00, 00, 00, DateTimeKind.Unspecified), TimeZoneInfo.Local), TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time")),
                    TimeZoneInfo.ConvertTime(TimeZoneInfo.ConvertTimeToUtc(new DateTime(2020, 03, 14, 00, 00, 00, DateTimeKind.Unspecified), TimeZoneInfo.Local), TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time")),
                    TimeZoneInfo.ConvertTime(TimeZoneInfo.ConvertTimeToUtc(new DateTime(2020, 03, 14, 00, 00, 00, DateTimeKind.Unspecified), TimeZoneInfo.Local), TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time"))
                };

                string[] a_testTrue2nd = new string[] {
                    "14-03-2020",
                    "14-03-2020",
                    "14-03-2020",
                    "14-03-2020",
                    "14-03-2020",
                    "14.03.2020",
                    "14.03.2020",
                    "14.03.2020",
                    "14.03.2020",
                    "14.03.2020",
                    "14/03/2020",
                    "14/03/2020",
                    "14/03/2020",
                    "14/03/2020",
                    "14/03/2020",
                    "03/14/2020",
                    "03/14/2020",
                    "03/14/2020",
                    "03/14/2020",
                    "03/14/2020",
                    "2020-03-14",
                    "2020-03-14",
                    "2020-03-14",
                    "2020-03-14",
                    "2020-03-14",
                    "2020/03/14",
                    "2020/03/14",
                    "2020/03/14",
                    "2020/03/14",
                    "2020/03/14",
                    "2020/14/03",
                    "2020/14/03",
                    "2020/14/03",
                    "2020/14/03",
                    "2020/14/03"
                };

                i = 0;

                foreach (string s_testTrue in a_testTrue2nd)
                {
                    Assert.That(
                        o_validLocalDates[i], Is.EqualTo(ForestNET.Lib.Helper.FromDateString(s_testTrue)),
                        "'" + s_testTrue + "' fromDateString() is not equal local date tine object '" + o_validLocalDates[i] + "'"
                    );

                    if (i == 4)
                    {
                        i = 0;
                    }
                    else
                    {
                        i++;
                    }
                }

                TimeSpan[] o_validLocalTimes = new TimeSpan[] {
                    new(05, 02, 00),
                    new(05, 02, 00),
                    new(05, 02, 03),
                    new(05, 02, 03),
                    new(05, 02, 03)
                };

                string[] a_testTrue3rd = new string[] {
                    "05:02",
                    "05:02",
                    "05:02:03",
                    "05:02:03",
                    "05:02:03",
                    "05:02",
                    "05:02",
                    "05:02:03",
                    "05:02:03",
                    "05:02:03",
                    "05:02",
                    "05:02",
                    "05:02:03",
                    "05:02:03",
                    "05:02:03",
                    "05:02",
                    "05:02",
                    "05:02:03",
                    "05:02:03",
                    "05:02:03",
                    "05:02",
                    "05:02",
                    "05:02:03",
                    "05:02:03",
                    "05:02:03",
                    "05:02",
                    "05:02",
                    "05:02:03",
                    "05:02:03",
                    "05:02:03",
                    "05:02",
                    "05:02",
                    "05:02:03",
                    "05:02:03",
                    "05:02:03"
                };

                i = 0;

                foreach (string s_testTrue in a_testTrue3rd)
                {
                    Assert.That(
                        o_validLocalTimes[i], Is.EqualTo(ForestNET.Lib.Helper.FromTimeString(s_testTrue)),
                        "'" + s_testTrue + "' fromTimeString() is not equal local date time object '" + o_validLocalTimes[i] + "'"
                    );

                    if (i == 4)
                    {
                        i = 0;
                    }
                    else
                    {
                        i++;
                    }
                }

                a_validLocalDateTime = new DateTime[] {
                    TimeZoneInfo.ConvertTime(TimeZoneInfo.ConvertTimeToUtc(new DateTime(2020, 03, 14, 06, 02, 00, DateTimeKind.Unspecified), TimeZoneInfo.Local), TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time")),
                    TimeZoneInfo.ConvertTime(TimeZoneInfo.ConvertTimeToUtc(new DateTime(2020, 03, 14, 06, 02, 00, DateTimeKind.Unspecified), TimeZoneInfo.Local), TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time")),
                    TimeZoneInfo.ConvertTime(TimeZoneInfo.ConvertTimeToUtc(new DateTime(2020, 03, 14, 06, 02, 03, DateTimeKind.Unspecified), TimeZoneInfo.Local), TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time")),
                    TimeZoneInfo.ConvertTime(TimeZoneInfo.ConvertTimeToUtc(new DateTime(2020, 03, 14, 06, 02, 03, DateTimeKind.Unspecified), TimeZoneInfo.Local), TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time")),
                    TimeZoneInfo.ConvertTime(TimeZoneInfo.ConvertTimeToUtc(new DateTime(2020, 03, 14, 06, 02, 03, DateTimeKind.Unspecified), TimeZoneInfo.Local), TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time"))
                };

                a_testFalse = new string[] {
                    "03-14-2020 05:02",
                    "03-14-2020T05:02",
                    "03-14-2020 05:02:03",
                    "03-14-2020T05:02:03",
                    "03-14-2020T05:02:03Z",
                    "14-03-2020 55:02",
                    "14-03-2020T55:02",
                    "14-03-2020 55:02:03",
                    "14-03-2020T55:02:03",
                    "14-03-2020T55:02:03Z",

                    "03.14.2020 05:02",
                    "03.14.2020T05:02",
                    "03.14.2020 05:02:03",
                    "03.14.2020T05:02:03",
                    "03.14.2020T05:02:03Z",
                    "14.03.2020 55:02",
                    "14.03.2020T55:02",
                    "14.03.2020 55:02:03",
                    "14.03.2020T55:02:03",
                    "14.03.2020T55:02:03Z",

                    "14/13/2020 05:02",
                    "14/13/2020T05:02",
                    "14/13/2020 05:02:03",
                    "14/13/2020T05:02:03",
                    "14/13/2020T05:02:03Z",
                    "14/03/2020 55:02",
                    "14/03/2020T55:02",
                    "14/03/2020 55:02:03",
                    "14/03/2020T55:02:03",
                    "14/03/2020T55:02:03Z",

                    "13/14/2020 05:02",
                    "13/14/2020T05:02",
                    "13/14/2020 05:02:03",
                    "13/14/2020T05:02:03",
                    "13/14/2020T05:02:03Z",
                    "03/14/2020 55:02",
                    "03/14/2020T55:02",
                    "03/14/2020 55:02:03",
                    "03/14/2020T55:02:03",
                    "03/14/2020T55:02:03Z",

                    "2020-14-03 05:02",
                    "2020-14-03T05:02",
                    "2020-14-03 05:02:03",
                    "2020-14-03T05:02:03",
                    "2020-14-03T05:02:03Z",
                    "2020-03-14 55:02",
                    "2020-03-14T55:02",
                    "2020-03-14 55:02:03",
                    "2020-03-14T55:02:03",
                    "2020-03-14T55:02:03Z",

                    "2020/13/14 05:02",
                    "2020/13/14T05:02",
                    "2020/13/14 05:02:03",
                    "2020/13/14T05:02:03",
                    "2020/13/14T05:02:03Z",
                    "2020/03/14 55:02",
                    "2020/03/14T55:02",
                    "2020/03/14 55:02:03",
                    "2020/03/14T55:02:03",
                    "2020/03/14T55:02:03Z",

                    "2020/14/13 05:02",
                    "2020/14/13T05:02",
                    "2020/14/13 05:02:03",
                    "2020/14/13T05:02:03",
                    "2020/14/13T05:02:03Z",
                    "2020/14/03 55:02",
                    "2020/14/03T55:02",
                    "2020/14/03 55:02:03",
                    "2020/14/03T55:02:03",
                    "2020/14/03T55:02:03Z"
                };

                i = 0;

                foreach (string s_testFalse in a_testFalse)
                {
                    bool b_check = true;

                    try
                    {
                        ForestNET.Lib.Helper.FromISO8601UTC(s_testFalse);
                    }
                    catch (Exception)
                    {
                        b_check = false;
                    }

                    if (b_check)
                    {
                        Assert.Fail("'" + s_testFalse + "' fromISO8601UTC() could be parsed to local date time object '" + a_validLocalDateTime[i] + "'");
                    }

                    try
                    {
                        ForestNET.Lib.Helper.FromDateTimeString(s_testFalse);
                    }
                    catch (Exception)
                    {
                        b_check = false;
                    }

                    if (b_check)
                    {
                        Assert.Fail("'" + s_testFalse + "' fromDateTimeString() could be parsed to local date time object '" + a_validLocalDateTime[i] + "'");
                    }

                    try
                    {
                        ForestNET.Lib.Helper.FromDateString(s_testFalse);
                    }
                    catch (Exception)
                    {
                        b_check = false;
                    }

                    if (b_check)
                    {
                        Assert.Fail("'" + s_testFalse + "' fromDateString() could be parsed to local date object '" + a_validLocalDateTime[i] + "'");
                    }

                    try
                    {
                        ForestNET.Lib.Helper.FromTimeString(s_testFalse);
                    }
                    catch (Exception)
                    {
                        b_check = false;
                    }

                    if (b_check)
                    {
                        Assert.Fail("'" + s_testFalse + "' fromTimeString() could be parsed to local time object '" + a_validLocalDateTime[i] + "'");
                    }

                    if (i == 4)
                    {
                        i = 0;
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            catch (Exception o_exc)
            {
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }

            for (int i = 0; i < 1000; i++)
            {
                int i_random = ForestNET.Lib.Helper.RandomIntegerRange(1, 10);
                Assert.That(
                    i_random >= 1 && i_random <= 10,
                    Is.True,
                    "random integer is not between 1..10"
                );
                Assert.That(
                    i_random < 1 && i_random > 10,
                    Is.False,
                    "random integer is not between 1..10"
                );
            }

            for (int i = 0; i < 1000; i++)
            {
                int i_random = ForestNET.Lib.Helper.SecureRandomIntegerRange(1, 10);
                Assert.That(
                    i_random >= 1 && i_random <= 10,
                    Is.True,
                    "random integer is not between 1..10"
                );
                Assert.That(
                    i_random < 1 && i_random > 10,
                    Is.False,
                    "random integer is not between 1..10"
                );
            }

            for (int i = 0; i < 1000; i++)
            {
                double d_random = ForestNET.Lib.Helper.RandomDoubleRange(1.5d, 10.75d);
                Assert.That(
                    d_random.CompareTo(1.5d) >= 0 && d_random.CompareTo(10.75d) < 1,
                    Is.True,
                    "random double is not between 1.5 .. 10.75"
                );
                Assert.That(
                    d_random.CompareTo(1.5d) < 0 && d_random.CompareTo(10.75d) >= 1,
                    Is.False,
                    "random double is not between 1.5 .. 10.75"
                );
            }

            byte[]? bytes = ForestNET.Lib.Helper.ShortToByteArray((short)558);
            short sh_test = ForestNET.Lib.Helper.ByteArrayToShort(bytes);
            Assert.That(
                sh_test, Is.EqualTo(558),
                "short to byte and back to short != 558"
            );
            Assert.That(
                sh_test, Is.Not.EqualTo(31630),
                "short to byte and back to short == 31630"
            );

            Assert.That(
                ForestNET.Lib.Helper.PrintByteArray(bytes, false), Is.EqualTo("00000010 00101110"),
                "bytes of short are not matching printed byte array"
            );
            Assert.That(
                ForestNET.Lib.Helper.PrintByteArray(bytes, true), Is.EqualTo("00000000 00000000 00000010 00101110"),
                "bytes of short are not matching printed byte array"
            );

            bytes = ForestNET.Lib.Helper.ShortToByteArray((short)25134);
            sh_test = ForestNET.Lib.Helper.ByteArrayToShort(bytes);
            Assert.That(
                sh_test, Is.EqualTo(25134),
                "short to byte and back to short != 25134"
            );
            Assert.That(
                sh_test, Is.Not.EqualTo(31630),
                "short to byte and back to short == 31630"
            );

            Assert.That(
                ForestNET.Lib.Helper.PrintByteArray(bytes, false), Is.EqualTo("01100010 00101110"),
                "bytes of short are not matching printed byte array"
            );
            Assert.That(
                ForestNET.Lib.Helper.PrintByteArray(bytes, true), Is.EqualTo("00000000 00000000 01100010 00101110"),
                "bytes of short are not matching printed byte array"
            );

            bytes = ForestNET.Lib.Helper.IntToByteArray(916040294);
            int i_test = ForestNET.Lib.Helper.ByteArrayToInt(bytes);
            Assert.That(
                i_test, Is.EqualTo(916040294),
                "int to byte and back to int != 916040294"
            );
            Assert.That(
                i_test, Is.Not.EqualTo(116040294),
                "int to byte and back to int == 116040294"
            );

            Assert.That(
                ForestNET.Lib.Helper.PrintByteArray(bytes), Is.EqualTo("00110110 10011001 10101010 01100110"),
                "bytes of int are not matching printed byte array"
            );
            Assert.That(
                ForestNET.Lib.Helper.PrintByteArray(bytes, false), Is.EqualTo("00110110 10011001 10101010 01100110"),
                "bytes of int are not matching printed byte array"
            );

            bytes = ForestNET.Lib.Helper.IntToByteArray(10070630);
            i_test = ForestNET.Lib.Helper.ByteArrayToInt(bytes);
            Assert.That(
                i_test, Is.EqualTo(10070630),
                "int to byte and back to int != 10070630"
            );
            Assert.That(
                i_test, Is.Not.EqualTo(90070630),
                "int to byte and back to int == 90070630"
            );

            Assert.That(
                ForestNET.Lib.Helper.PrintByteArray(bytes), Is.EqualTo("00000000 10011001 10101010 01100110"),
                "bytes of int are not matching printed byte array"
            );
            Assert.That(
                ForestNET.Lib.Helper.PrintByteArray(bytes, false), Is.EqualTo("10011001 10101010 01100110"),
                "bytes of int are not matching printed byte array"
            );

            bytes = ForestNET.Lib.Helper.IntToByteArray(43622);
            i_test = ForestNET.Lib.Helper.ByteArrayToInt(bytes);
            Assert.That(
                i_test, Is.EqualTo(43622),
                "int to byte and back to int != 43622"
            );
            Assert.That(
                i_test, Is.Not.EqualTo(13622),
                "int to byte and back to int == 13622"
            );

            Assert.That(
                ForestNET.Lib.Helper.PrintByteArray(bytes), Is.EqualTo("00000000 00000000 10101010 01100110"),
                "bytes of int are not matching printed byte array"
            );
            Assert.That(
                ForestNET.Lib.Helper.PrintByteArray(bytes, false), Is.EqualTo("10101010 01100110"),
                "bytes of int are not matching printed byte array"
            );

            bytes = ForestNET.Lib.Helper.IntToByteArray(102);
            i_test = ForestNET.Lib.Helper.ByteArrayToInt(bytes);
            Assert.That(
                i_test, Is.EqualTo(102),
                "int to byte and back to int != 102"
            );
            Assert.That(
                i_test, Is.Not.EqualTo(902),
                "int to byte and back to int == 902"
            );

            Assert.That(
                ForestNET.Lib.Helper.PrintByteArray(bytes), Is.EqualTo("00000000 00000000 00000000 01100110"),
                "bytes of int are not matching printed byte array"
            );
            Assert.That(
                ForestNET.Lib.Helper.PrintByteArray(bytes, false), Is.EqualTo("01100110"),
                "bytes of int are not matching printed byte array"
            );

            bytes = ForestNET.Lib.Helper.LongToByteArray(9070052179665454);
            long l_test = ForestNET.Lib.Helper.ByteArrayToLong(bytes);
            Assert.That(
                l_test, Is.EqualTo(9070052179665454),
                "long to byte and back to long != 9070052179665454"
            );
            Assert.That(
                l_test, Is.Not.EqualTo(4620756070607053358),
                "long to byte and back to long == 4620756070607053358"
            );

            Assert.That(
                ForestNET.Lib.Helper.PrintByteArray(bytes, false), Is.EqualTo("00100000 00111001 00101010 00010110 01000011 01100010 00101110"),
                "bytes of long " + ForestNET.Lib.Helper.PrintByteArray(bytes, false) + " are not matching printed byte array"
            );

            bytes = ForestNET.Lib.Helper.LongToByteArray(3467834566000206382);
            l_test = ForestNET.Lib.Helper.ByteArrayToLong(bytes);
            Assert.That(
                l_test, Is.EqualTo(3467834566000206382),
                "long to byte and back to long != 3467834566000206382"
            );
            Assert.That(
                l_test, Is.Not.EqualTo(4620756070607053358),
                "long to byte and back to long == 4620756070607053358"
            );

            Assert.That(
                ForestNET.Lib.Helper.PrintByteArray(bytes, false), Is.EqualTo("00110000 00100000 00111001 00101010 00010110 01000011 01100010 00101110"),
                "bytes of long are not matching printed byte array"
            );

            bytes = ForestNET.Lib.Helper.AmountToNByteArray(101458034, 4);
            l_test = ForestNET.Lib.Helper.ByteArrayToLong(bytes);
            Assert.That(
                l_test, Is.EqualTo(101458034),
                "long to byte and back to long != 101458034"
            );
            Assert.That(
                l_test, Is.Not.EqualTo(4620756070607053358),
                "long to byte and back to long == 4620756070607053358"
            );

            Assert.That(
                ForestNET.Lib.Helper.PrintByteArray(bytes, false), Is.EqualTo("00000110 00001100 00100000 01110010"),
                "bytes of long are not matching printed byte array"
            );

            bytes = ForestNET.Lib.Helper.AmountToNByteArray(101458034, 8);
            l_test = ForestNET.Lib.Helper.ByteArrayToLong(bytes);
            Assert.That(
                l_test, Is.EqualTo(101458034),
                "long to byte and back to long != 101458034"
            );
            Assert.That(
                l_test, Is.Not.EqualTo(4620756070607053358),
                "long to byte and back to long == 4620756070607053358"
            );

            Assert.That(
                ForestNET.Lib.Helper.PrintByteArray(bytes, false), Is.EqualTo("00000000 00000000 00000000 00000000 00000110 00001100 00100000 01110010"),
                "bytes of long are not matching printed byte array"
            );

            Assert.That(
                ForestNET.Lib.Helper.FormatBytes(2123456797), Is.EqualTo("2,12 GB"),
                "2123456797 is not 2,12 GB"
            );
            Assert.That(
                ForestNET.Lib.Helper.FormatBytes(2123456797, true), Is.EqualTo("1,98 GiB"),
                "2123456797 is not 1,98 GiB"
            );

            Assert.That(
                ForestNET.Lib.Helper.FormatBytes(129456797), Is.Not.EqualTo("2,12 GB"),
                "129456797 is 2,12 GB"
            );
            Assert.That(
                ForestNET.Lib.Helper.FormatBytes(129456797, true), Is.Not.EqualTo("1,98 GiB"),
                "129456797 is 1,98 GiB"
            );

            Assert.That(
                ForestNET.Lib.Helper.FormatBytes(126797), Is.EqualTo("126,8 KB"),
                "126797 is not 126,8 KB"
            );
            Assert.That(
                ForestNET.Lib.Helper.FormatBytes(126797, true), Is.EqualTo("123,83 KiB"),
                "126797 is not 123,83 KiB"
            );

            Assert.That(
                ForestNET.Lib.Helper.FormatBytes(33126797), Is.Not.EqualTo("126,8 KB"),
                "33126797 is 126,8 KB"
            );
            Assert.That(
                ForestNET.Lib.Helper.FormatBytes(6797, true), Is.Not.EqualTo("123,83 KiB"),
                "6797 is 123,83 KiB"
            );

            try
            {
                Assert.That(
                    ForestNET.Lib.Helper.HashByteArray("SHA-256", ForestNET.Lib.Helper.IntToByteArray(43622)), Is.EqualTo("9F2778195BB08930F6455CA6C191D9DC25B77F33145141A2E89FAC794D5E7C47"),
                    "SHA-256 ist not '9F2778195BB08930F6455CA6C191D9DC25B77F33145141A2E89FAC794D5E7C47'"
                );
                Assert.That(
                    ForestNET.Lib.Helper.HashByteArray("SHA-384", ForestNET.Lib.Helper.IntToByteArray(10070630)), Is.EqualTo("4DD7D9B43AFD5CA29A794B59EE924F2226E7776C7D25E052060AB71DCD7254DA9FF5C342F8E943D85336D7D97BAD8CB9"),
                    "SHA-384 ist not '4DD7D9B43AFD5CA29A794B59EE924F2226E7776C7D25E052060AB71DCD7254DA9FF5C342F8E943D85336D7D97BAD8CB9'"
                );
                Assert.That(
                    ForestNET.Lib.Helper.HashByteArray("SHA-512", ForestNET.Lib.Helper.IntToByteArray(916040294)), Is.EqualTo("38C320515E85995FC7ACFEFD5126EBA8EDB6133E6E552565899534D03E8AF6D6FF9BB1C165C58BBB43AED8DE01FDD3FB0C9F4F6D384AD8BCD419421AC10AB9C1"),
                    "SHA-512 ist not '38C320515E85995FC7ACFEFD5126EBA8EDB6133E6E552565899534D03E8AF6D6FF9BB1C165C58BBB43AED8DE01FDD3FB0C9F4F6D384AD8BCD419421AC10AB9C1'"
                );

                Assert.That(
                    ForestNET.Lib.Helper.HashByteArray("SHA-256", ForestNET.Lib.Helper.IntToByteArray(43622)), Is.Not.EqualTo("9F2778195BB08930F6455CA6C191D9DC25C77F33145141A2E89FAC794D5E7C47"),
                    "SHA-256 is '9F2778195BB08930F6455CA6C191D9DC25C77F33145141A2E89FAC794D5E7C47'"
                );
                Assert.That(
                    ForestNET.Lib.Helper.HashByteArray("SHA-384", ForestNET.Lib.Helper.IntToByteArray(10070630)), Is.Not.EqualTo("4DD7D9B43AFD5CA29A794B59EE924F2226E7776C7D25E052065AB71DCD7254DA9FF5C342F8E943D85336D7D97BAD8CB9"),
                    "SHA-384 is '4DD7D9B43AFD5CA29A794B59EE924F2226E7776C7D25E052065AB71DCD7254DA9FF5C342F8E943D85336D7D97BAD8CB9'"
                );
                Assert.That(
                    ForestNET.Lib.Helper.HashByteArray("SHA-512", ForestNET.Lib.Helper.IntToByteArray(916040294)), Is.Not.EqualTo("38C320515E85995FC7ACFEFD5126EBA8EDB6133E6E552565899534D03E8AF6D6F39BB1C165C58BBB43AED8DE01FDD3FB0C9F4F6D384AD8BCD419421AC10AB9C1"),
                    "SHA-512 is '38C320515E85995FC7ACFEFD5126EBA8EDB6133E6E552565899534D03E8AF6D6F39BB1C165C58BBB43AED8DE01FDD3FB0C9F4F6D384AD8BCD419421AC10AB9C1'"
                );

                Assert.That(
                    ForestNET.Lib.Helper.BytesToHexString(Encoding.ASCII.GetBytes("Das ist das Haus vom Nikolaus."), true), Is.EqualTo("0x44 0x61 0x73 0x20 0x69 0x73 0x74 0x20 0x64 0x61 0x73 0x20 0x48 0x61 0x75 0x73 0x20 0x76 0x6F 0x6D 0x20 0x4E 0x69 0x6B 0x6F 0x6C 0x61 0x75 0x73 0x2E"),
                    "BytesToHexString result for 'Das ist das Haus vom Nikolaus.' does not match expected value."
                );
                Assert.That(
                    ForestNET.Lib.Helper.BytesToHexString(Encoding.ASCII.GetBytes("Das ist das Haus vom Nikolaus."), false), Is.EqualTo("4461732069737420646173204861757320766F6D204E696B6F6C6175732E"),
                    "BytesToHexString result for 'Das ist das Haus vom Nikolaus.' does not match expected value."
                );
                Assert.That(
                    ForestNET.Lib.Helper.BytesToHexString(Encoding.GetEncoding("ISO-8859-1").GetBytes("ABCDEFGHIJKL€"), true), Is.EqualTo("0x41 0x42 0x43 0x44 0x45 0x46 0x47 0x48 0x49 0x4A 0x4B 0x4C 0x3F"),
                    "BytesToHexString result for 'ABCDEFGHIJKL€' does not match expected value for charset 'ISO-8859-1'."
                );
                Assert.That(
                    ForestNET.Lib.Helper.BytesToHexString(Encoding.UTF8.GetBytes("ABCDEFGHIJKL€"), true), Is.EqualTo("0x41 0x42 0x43 0x44 0x45 0x46 0x47 0x48 0x49 0x4A 0x4B 0x4C 0xE2 0x82 0xAC"),
                    "BytesToHexString result for 'ABCDEFGHIJKL€' does not match expected value for charset 'UTF-8'."
                );
                Assert.That(
                    ForestNET.Lib.Helper.BytesToHexString(Encoding.GetEncoding("UTF-16").GetBytes("ABCDEFGHIJKL€"), true), Is.EqualTo("0x41 0x00 0x42 0x00 0x43 0x00 0x44 0x00 0x45 0x00 0x46 0x00 0x47 0x00 0x48 0x00 0x49 0x00 0x4A 0x00 0x4B 0x00 0x4C 0x00 0xAC 0x20"),
                    "BytesToHexString result for 'ABCDEFGHIJKL€' does not match expected value for charset 'UTF-16'."
                );
                Assert.That(
                    ForestNET.Lib.Helper.BytesToHexString(Encoding.Unicode.GetBytes("ABCDEFGHIJKL€"), true), Is.EqualTo("0x41 0x00 0x42 0x00 0x43 0x00 0x44 0x00 0x45 0x00 0x46 0x00 0x47 0x00 0x48 0x00 0x49 0x00 0x4A 0x00 0x4B 0x00 0x4C 0x00 0xAC 0x20"),
                    "BytesToHexString result for 'ABCDEFGHIJKL€' does not match expected value for charset 'Unicode'."
                );

                Assert.That(
                    "Das ist das Haus v"u8.ToArray().SequenceEqual(
                    ForestNET.Lib.Helper.HexStringToBytes("0x44 0x61 0x73 0x20 0x69 0x73 0x74 0x20 0x64 0x61 0x73 0x20 0x48 0x61 0x75 0x73 0x20 0x76")),
                    Is.True,
                    "HexStringToBytes result for '0x44 0x61 0x73 0x20 0x69 0x73 0x74 0x20 0x64 0x61 0x73 0x20 0x48 0x61 0x75 0x73 0x20 0x76' does not match expected value."
                );
                Assert.That(
                    "Das ist das Haus v"u8.ToArray().SequenceEqual(
                    ForestNET.Lib.Helper.HexStringToBytes("446173206973742064617320486175732076")),
                    Is.True,
                    "HexStringToBytes result for '446173206973742064617320486175732076' does not match expected value."
                );
            }
            catch (Exception o_exc)
            {
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }

            List<string> a_list1 = [.. new[] { "a", "bc", "def", "ghij", "klmno" }];
            Assert.That(
                ForestNET.Lib.Helper.JoinList(a_list1, ','), Is.EqualTo("a,bc,def,ghij,klmno"),
                "concated list '" + ForestNET.Lib.Helper.JoinList(a_list1, ',') + "' is not equal 'a,bc,def,ghij,klmno'"
            );

            List<int> a_list2 = [.. new[] { 1, 23, 45, 678, 910111213 }];
            Assert.That(
                ForestNET.Lib.Helper.JoinList(a_list2, ':'), Is.EqualTo("1:23:45:678:910111213"),
                "concated list '" + ForestNET.Lib.Helper.JoinList(a_list1, ':') + "' is not equal '1:23:45:678:910111213'"
            );

            Assert.That(
                ForestNET.Lib.Helper.GetIndexOfObjectInList([.. new[] { "two", "one", "three" }], "one"), Is.GreaterThan(0),
                "'one' not found and no index returned from array list"
            );
            Assert.That(
                ForestNET.Lib.Helper.GetIndexOfObjectInList([.. new[] { "two", "four", "three" }], "one"), Is.LessThanOrEqualTo(0),
                "'one' found and index returned from array list"
            );

            Assert.That(
                ForestNET.Lib.Helper.IsIndexValid([.. new[] { "two", "one", "three" }], 2),
                Is.True,
                "index '2' is not valid"
            );
            Assert.That(
                ForestNET.Lib.Helper.IsIndexValid([.. new[] { "two", "four", "three" }], 42),
                Is.False,
                "index '42' is valid"
            );

            TimeSpan o_timeSpan = TimeSpan.FromSeconds(597845641);
            Assert.That(
                ForestNET.Lib.Helper.FormatTimeSpan(o_timeSpan), Is.EqualTo("2767:48:14"),
                "duration is not '2767:48:14'"
            );
            Assert.That(
                ForestNET.Lib.Helper.FormatTimeSpan(o_timeSpan), Is.Not.EqualTo("767:18:14"),
                "duration is '767:48:14'"
            );

            string s_random = ForestNET.Lib.Helper.GenerateRandomString(32);

            Assert.That(
                s_random, Has.Length.EqualTo(32),
                "random generated string has not a length of 32 characters"
            );

            s_random = ForestNET.Lib.Helper.GenerateRandomString(10, ForestNET.Lib.Helper.DIGITS_CHARACTERS);

            Assert.That(
                s_random, Has.Length.EqualTo(10),
                "random generated string has not a length of 10 characters"
            );

            for (int i = 0; i < s_random.Length; i++)
            {
                Assert.That(
                    char.IsDigit(s_random[i]),
                    Is.True,
                    "random generated string, only digits, has a character which is not a digit: " + s_random
                );
            }

            s_random = ForestNET.Lib.Helper.GenerateUUID();

            Assert.That(
                s_random, Has.Length.EqualTo(36),
                "random generated string has not a length of 36 characters"
            );

            string s_original = "ftps://user:password@expample.com:21";
            string s_disguised = ForestNET.Lib.Helper.DisguiseSubstring(s_original, "//", "@", '*');

            Assert.That(
                s_disguised, Is.EqualTo("ftps://*************@expample.com:21"),
                "disguised string is not 'ftps://*************@expample.com:21', but '" + s_disguised + "'"
            );

            s_original = "This is just a \"test\"";
            s_disguised = ForestNET.Lib.Helper.DisguiseSubstring(s_original, "\"", "\"", '+');

            Assert.That(
                s_disguised, Is.EqualTo("This is just a \"++++\""),
                "disguised string is not 'This is just a \"++++\"', but '" + s_disguised + "'"
            );

            s_original = "No disguise at all";
            s_disguised = ForestNET.Lib.Helper.DisguiseSubstring(s_original, ".", "&", '_');

            Assert.That(
                s_disguised, Is.EqualTo(s_original),
                "disguised string is not '" + s_original + "', but '" + s_disguised + "'"
            );

            string[] a_ipAddresses = new string[] {
                "192.168.2.100",
                "157.166.224.26/10",
                "157.128.64.254",
                "157.166.224.26/10",
                "10.10.128.23",
                "207.4.228.64/27",
                "207.4.228.87",
                "207.4.228.64/27",
                "192.168.2.1",
                "152.107.20.137/21",
                "152.107.20.13",
                "152.107.20.137/21",
                "10.10.10.255",
                "3.184.239.36/17",
                "3.184.202.202",
                "3.184.239.36/17",
                "192.168.2.100",
                "192.168.2.100/24"
            };

            uint[] a_range = ForestNET.Lib.Helper.GetRangeOfSubnet("192.168.2.100/24");

            Assert.That(
                ForestNET.Lib.Helper.Ipv4IntToString(a_range[0]), Is.EqualTo("192.168.2.1"),
                "conversion from ipv4 int to '192.168.2.1' failed, result is '" + ForestNET.Lib.Helper.Ipv4IntToString(a_range[0]) + "'"
            );

            Assert.That(
                ForestNET.Lib.Helper.Ipv4IntToString(a_range[1]), Is.EqualTo("192.168.2.254"),
                "conversion from ipv4 int to '192.168.2.254' failed, result is '" + ForestNET.Lib.Helper.Ipv4IntToString(a_range[0]) + "'"
            );

            bool[] a_ipAddressesResults = new bool[] {
                false, true, false, true, false, true, false, true, true
            };

            for (int i = 0; i < a_ipAddressesResults.Length; i++)
            {
                Assert.That(
                    ForestNET.Lib.Helper.IsIpv4Address(a_ipAddresses[i * 2]),
                    Is.True,
                    "'" + a_ipAddresses[i * 2] + "' is not an ipv4 address"
                );

                Assert.That(
                    ForestNET.Lib.Helper.IsIpv4AddressWithSuffix(a_ipAddresses[(i * 2) + 1]),
                    Is.True,
                    "'" + a_ipAddresses[(i * 2) + 1] + "' is not an ipv4 address with suffix"
                );

                Assert.That(
                    ForestNET.Lib.Helper.Ipv4IntToString(ForestNET.Lib.Helper.Ipv4ToInt(a_ipAddresses[i * 2])), Is.EqualTo(a_ipAddresses[i * 2]),
                    "conversion from ipv4 int and back for '" + a_ipAddresses[i * 2] + "' failed"
                );

                Assert.That(
                    a_ipAddressesResults[i], Is.EqualTo(ForestNET.Lib.Helper.IsIpv4WithinRange(a_ipAddresses[i * 2], a_ipAddresses[(i * 2) + 1])),
                    "ip '" + a_ipAddresses[i * 2] + "' is " + ((!a_ipAddressesResults[i]) ? "not" : "") + " within '" + a_ipAddresses[(i * 2) + 1] + "'"
                );
            }
        }
    }
}
