using System.Text;

namespace ForestNETTests.Core
{
    public class HelperUnitTest
    {
        [Test]
        public void TestHelper()
        {
            /* log some info -> Microsoft.VisualStudio.TestTools.UnitTesting.Logging.Logger.LogMessage("text I want to log", new object[] { }); */

            Assert.That(
                ForestNETLib.Core.Helper.IsStringEmpty(null),
                Is.True,
                "null is not string empty"
            );
            Assert.That(
                ForestNETLib.Core.Helper.IsStringEmpty(""),
                Is.True,
                "empty string is not string empty"
            );
            Assert.That(
                ForestNETLib.Core.Helper.IsStringEmpty("  "),
                Is.True,
                "two white spaces is not string empty"
            );
            Assert.That(
                ForestNETLib.Core.Helper.IsStringEmpty("notEmpty"),
                Is.False,
                "'notEmpty' is string empty"
            );

            Assert.That(
                ForestNETLib.Core.Helper.IsString("test"),
                Is.True,
                "'test' is not string"
            );
            Assert.That(
                ForestNETLib.Core.Helper.IsString(123),
                Is.False,
                "123 is a string"
            );

            Assert.That(
                ForestNETLib.Core.Helper.IsShort("test"),
                Is.False,
                "'test' is a short"
            );
            Assert.That(
                ForestNETLib.Core.Helper.IsShort("123"),
                Is.True,
                "123 is not a short"
            );

            Assert.That(
                ForestNETLib.Core.Helper.IsInteger("test"),
                Is.False,
                "'test' is an integer"
            );
            Assert.That(
                ForestNETLib.Core.Helper.IsInteger("123550"),
                Is.True,
                "1234550 is not an integer"
            );

            Assert.That(
                ForestNETLib.Core.Helper.IsLong("test"),
                Is.False,
                "'test' is a long"
            );
            Assert.That(
                ForestNETLib.Core.Helper.IsLong("1235464545464654550"),
                Is.True,
                "1235464545464654550 is not a long"
            );

            Assert.That(
                ForestNETLib.Core.Helper.IsFloat("test"),
                Is.False,
                "'test' is a float"
            );
            Assert.That(
                ForestNETLib.Core.Helper.IsFloat("1235464.454644545464654550"),
                Is.True,
                "1235464.454644545464654550 is no a float"
            );

            Assert.That(
                ForestNETLib.Core.Helper.IsDouble("test"),
                Is.False,
                "'test' is a double"
            );
            Assert.That(
                ForestNETLib.Core.Helper.IsDouble("12354645.45446445464654550"),
                Is.True,
                "12354645.45446445464654550 is not a double"
            );

            Assert.That(
                ForestNETLib.Core.Helper.IsBoolean("1"),
                Is.False,
                "'1' is boolean true"
            );
            Assert.That(
                ForestNETLib.Core.Helper.IsBoolean("true"),
                Is.True,
                "'true' is not boolean true"
            );

            Assert.That(
                ForestNETLib.Core.Helper.MatchesRegex("123e4567-e89b-12Q3-a456-426614174000", "^[a-f0-9\\-]*$"),
                Is.False,
                "'123e4567-e89b-12___Q___3-a456-426614174000' matches regex '" + "^[a-f0-9\\-]*$" + "'"
            );
            Assert.That(
                ForestNETLib.Core.Helper.MatchesRegex("123e4567-e89b-12d3-a456-426614174000", "^[a-f0-9\\-]*$"),
                Is.True,
                "'123e4567-e89b-12d3-a456-426614174000' not matches regex '" + "^[a-f0-9\\-]*$" + "'"
            );

            Assert.That(
                ForestNETLib.Core.Helper.CountSubStrings("HelloabcdefgHelloabcdefgHelloHello", "Hallo"), Is.Not.EqualTo(4),
                "'Hallo' is 4 times in 'HelloabcdefgHelloabcdefgHelloHello'"
            );
            Assert.That(
                ForestNETLib.Core.Helper.CountSubStrings("HelloabcdefgHelloabcdefgHelloHello", "Hello"), Is.EqualTo(4),
                "'Hello' is not 4 times in 'HelloabcdefgHelloabcdefgHelloHello'"
            );

            Assert.That(
                ForestNETLib.Core.Helper.PowIntegers(2, 16), Is.EqualTo(65536),
                "Exponentiation of 2^16 is not 65536"
            );

            Assert.That(
                ForestNETLib.Core.Helper.PowIntegers(12, 5), Is.EqualTo(248832),
                "Exponentiation of 12^5 is not 248832"
            );

            Assert.That(
                ForestNETLib.Core.Helper.IsDate("31-03-2020"),
                Is.True,
                "'31-03-2020' is not a date"
            );
            Assert.That(
                ForestNETLib.Core.Helper.IsDate("03-31-2020"),
                Is.False,
                "'03-31-2020' is a date"
            );
            Assert.That(
                ForestNETLib.Core.Helper.IsDate("31.03.2020"),
                Is.True,
                "'31.03.2020' is not a date"
            );
            Assert.That(
                ForestNETLib.Core.Helper.IsDate("03.31.2020"),
                Is.False,
                "'03.31.2020' is a date"
            );
            Assert.That(
                ForestNETLib.Core.Helper.IsDate("31/03/2020"),
                Is.True,
                "'31/03/2020' is not a date"
            );
            Assert.That(
                ForestNETLib.Core.Helper.IsDate("31/13/2020"),
                Is.False,
                "'31/13/2020' is a date"
            );
            Assert.That(
                ForestNETLib.Core.Helper.IsDate("2020/03/31"),
                Is.True,
                "'2020/03/31' is not a date"
            );
            Assert.That(
                ForestNETLib.Core.Helper.IsDate("2020/13/31"),
                Is.False,
                "'2020/13/31' is a date"
            );
            Assert.That(
                ForestNETLib.Core.Helper.IsDate("2020-03-31"),
                Is.True,
                "'2020-03-31' is not a date"
            );
            Assert.That(
                ForestNETLib.Core.Helper.IsDate("2020-31-03"),
                Is.False,
                "'2020-31-03' is a date"
            );
            Assert.That(
                ForestNETLib.Core.Helper.IsDate("03/31/2020"),
                Is.True,
                "'03/31/2020' is not a date"
            );
            Assert.That(
                ForestNETLib.Core.Helper.IsDate("13/31/2020"),
                Is.False,
                "'13/31/2020' is a date"
            );
            Assert.That(
                ForestNETLib.Core.Helper.IsDate("2020/31/03"),
                Is.True,
                "'2020/31/03' is not a date"
            );
            Assert.That(
                ForestNETLib.Core.Helper.IsDate("2020/31/13"),
                Is.False,
                "'2020/31/13' is a date"
            );

            Assert.That(
                ForestNETLib.Core.Helper.IsTime("53:45:32"),
                Is.False,
                "'53:45:32' is a time"
            );
            Assert.That(
                ForestNETLib.Core.Helper.IsTime("13:45:32"),
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
                "2020/31/03 13:45",
                "31-03-2020 13:45:32.576",
                "31-03-2020T13:45:32.576",
                "31-03-2020 13:45:32.576Z",
                "31-03-2020T13:45:32.576Z"
            };

            foreach (string s_testTrue in a_testTrue)
            {
                Assert.That(
                    ForestNETLib.Core.Helper.IsDateTime(s_testTrue),
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
                    ForestNETLib.Core.Helper.IsDateTime(s_testFalse),
                    Is.False,
                    "'" + s_testFalse + "' is a date time"
                );
            }

            Assert.That(
                ForestNETLib.Core.Helper.IsDateInterval("P2DQ6Y"),
                Is.False,
                "'P2DQ6Y' is a date interval"
            );
            Assert.That(
                ForestNETLib.Core.Helper.IsDateInterval("P4Y"),
                Is.True,
                "'P4Y' is not a date interval"
            );
            Assert.That(
                ForestNETLib.Core.Helper.IsDateInterval("PT15S"),
                Is.True,
                "'PT15S' is not a date interval"
            );
            Assert.That(
                ForestNETLib.Core.Helper.IsDateInterval("P2DT2H3M55S"),
                Is.True,
                "'P2DT2H3M55S' is not a date interval"
            );

            try
            {
                bool b_isDaySavingTime = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);

                DateTime o_localDateTime = TimeZoneInfo.ConvertTime(TimeZoneInfo.ConvertTimeToUtc(new DateTime(2020, 03, 14, 06, 02, 03, DateTimeKind.Unspecified), TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time")), TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time"));
                Assert.That(
                    ForestNETLib.Core.Helper.ToISO8601UTC(o_localDateTime), Is.EqualTo("2020-03-14T05:02:03Z"),
                    "local date time object[" + ForestNETLib.Core.Helper.ToISO8601UTC(o_localDateTime) + "] is not equal with toISO8601UTC to '2020-03-14T05:02:03Z'"
                );
                Assert.That(
                    ForestNETLib.Core.Helper.ToISO8601UTC(o_localDateTime), Is.Not.EqualTo("2030-03-14T05:02:03Z"),
                    "local date time object[" + ForestNETLib.Core.Helper.ToISO8601UTC(o_localDateTime) + "] is equal with toISO8601UTC to '2030-03-14T05:02:03Z'"
                );

                Assert.That(
                    ForestNETLib.Core.Helper.ToRFC1123(o_localDateTime), Is.EqualTo("Sat, 14 Mar 2020 05:02:03 GMT"),
                    "local date time object[" + ForestNETLib.Core.Helper.ToRFC1123(o_localDateTime) + "] is not equal with toRFC1123 to 'Sat, 14 Mar 2020 05:02:03 GMT'"
                );
                Assert.That(
                    ForestNETLib.Core.Helper.ToRFC1123(o_localDateTime), Is.Not.EqualTo("Sat, 14 Mar 2030 05:02:03 GMT"),
                    "local date time object[" + ForestNETLib.Core.Helper.ToRFC1123(o_localDateTime) + "] is equal with toRFC1123 to 'Sat, 14 Mar 2030 05:02:03 GMT'"
                );

                if (b_isDaySavingTime)
                {
                    /* o_localDateTime = o_localDateTime.AddHours(-1); */
                }

                Assert.That(
                    ForestNETLib.Core.Helper.ToDateTimeString(o_localDateTime), Is.EqualTo("2020-03-14T06:02:03"),
                    "local date time object[" + ForestNETLib.Core.Helper.ToDateTimeString(o_localDateTime) + "] is not equal with toDateTimeString to '2020-03-14T06:02:03'"
                );
                Assert.That(
                    ForestNETLib.Core.Helper.ToDateTimeString(o_localDateTime), Is.Not.EqualTo("2030-03-14T06:02:03"),
                    "local date time object[" + ForestNETLib.Core.Helper.ToDateTimeString(o_localDateTime) + "] is equal with toDateTimeString to '2030-03-14T06:02:03'"
                );

                o_localDateTime = TimeZoneInfo.ConvertTime(TimeZoneInfo.ConvertTimeToUtc(new DateTime(2020, 03, 14, 06, 02, 00, DateTimeKind.Unspecified), TimeZoneInfo.Local), TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time"));
                Assert.That(
                    ForestNETLib.Core.Helper.ToISO8601UTC(o_localDateTime), Is.EqualTo("2020-03-14T05:02:00Z"),
                    "local date time object[" + ForestNETLib.Core.Helper.ToISO8601UTC(o_localDateTime) + "] is not equal with toISO8601UTC to '2020-03-14T05:02:00Z'"
                );
                Assert.That(
                    ForestNETLib.Core.Helper.ToISO8601UTC(o_localDateTime), Is.Not.EqualTo("2030-03-14T05:02:00Z"),
                    "local date time object[" + ForestNETLib.Core.Helper.ToISO8601UTC(o_localDateTime) + "] is equal with toISO8601UTC to '2030-03-14T05:02:00Z'"
                );

                Assert.That(
                    ForestNETLib.Core.Helper.ToRFC1123(o_localDateTime), Is.EqualTo("Sat, 14 Mar 2020 05:02:00 GMT"),
                    "local date time object[" + ForestNETLib.Core.Helper.ToRFC1123(o_localDateTime) + "] is not equal with toRFC1123 to 'Sat, 14 Mar 2020 05:02:00 GMT'"
                );
                Assert.That(
                    ForestNETLib.Core.Helper.ToRFC1123(o_localDateTime), Is.Not.EqualTo("Sat, 14 Mar 2030 05:02:00 GMT"),
                    "local date time object[" + ForestNETLib.Core.Helper.ToRFC1123(o_localDateTime) + "] is equal with toRFC1123 to 'Sat, 14 Mar 2030 05:02:00 GMT'"
                );

                if (b_isDaySavingTime)
                {
                    /* o_localDateTime = o_localDateTime.AddHours(-1); */
                }

                Assert.That(
                    ForestNETLib.Core.Helper.ToDateTimeString(o_localDateTime), Is.EqualTo("2020-03-14T06:02:00"),
                    "local date time object[" + ForestNETLib.Core.Helper.ToDateTimeString(o_localDateTime) + "] is not equal with toDateTimeString to '2020-03-14T06:02:00'"
                );
                Assert.That(
                    ForestNETLib.Core.Helper.ToDateTimeString(o_localDateTime), Is.Not.EqualTo("2030-03-14T06:02:00"),
                    "local date time object[" + ForestNETLib.Core.Helper.ToDateTimeString(o_localDateTime) + "] is equal with toDateTimeString to '2030-03-14T06:02:00'"
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
                    TimeZoneInfo.ConvertTime(TimeZoneInfo.ConvertTimeToUtc(new DateTime(2020, 03, 14, 06, 02, 03, DateTimeKind.Unspecified), TimeZoneInfo.Local), TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time")),
                    TimeZoneInfo.ConvertTime(TimeZoneInfo.ConvertTimeToUtc(new DateTime(2020, 03, 14, 06, 02, 03, 576, DateTimeKind.Unspecified), TimeZoneInfo.Local), TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time"))
                };

                a_testTrue = new string[] {
                    "14-03-2020 05:02",
                    "14-03-2020T05:02",
                    "14-03-2020 05:02:03",
                    "14-03-2020T05:02:03",
                    "14-03-2020T05:02:03Z",
                    "14-03-2020T05:02:03.576",

                    "14.03.2020 05:02",
                    "14.03.2020T05:02",
                    "14.03.2020 05:02:03",
                    "14.03.2020T05:02:03",
                    "14.03.2020T05:02:03Z",
                    "14.03.2020T05:02:03.576Z",

                    "14/03/2020 05:02",
                    "14/03/2020T05:02",
                    "14/03/2020 05:02:03",
                    "14/03/2020T05:02:03",
                    "14/03/2020T05:02:03Z",
                    "14/03/2020T05:02:03.576",

                    "03/14/2020 05:02",
                    "03/14/2020T05:02",
                    "03/14/2020 05:02:03",
                    "03/14/2020T05:02:03",
                    "03/14/2020T05:02:03Z",
                    "03/14/2020T05:02:03.576Z",

                    "2020-03-14 05:02",
                    "2020-03-14T05:02",
                    "2020-03-14 05:02:03",
                    "2020-03-14T05:02:03",
                    "2020-03-14T05:02:03Z",
                    "2020-03-14T05:02:03.576",

                    "2020/03/14 05:02",
                    "2020/03/14T05:02",
                    "2020/03/14 05:02:03",
                    "2020/03/14T05:02:03",
                    "2020/03/14T05:02:03Z",
                    "2020/03/14T05:02:03.576Z",

                    "2020/14/03 05:02",
                    "2020/14/03T05:02",
                    "2020/14/03 05:02:03",
                    "2020/14/03T05:02:03",
                    "2020/14/03T05:02:03Z",
                    "2020/14/03T05:02:03.576",
                };

                int i = 0;

                foreach (string s_testTrue in a_testTrue)
                {
                    Assert.That(
                        a_validLocalDateTime[i], Is.EqualTo(ForestNETLib.Core.Helper.FromISO8601UTC(s_testTrue)),
                        "'" + s_testTrue + "' fromISO8601UTC() is not equal local date tine object '" + a_validLocalDateTime[i] + "'"
                    );

                    Assert.That(
                        a_validLocalDateTime[i].AddHours(-1), Is.EqualTo(ForestNETLib.Core.Helper.FromDateTimeString(s_testTrue)),
                        "'" + s_testTrue + "' fromDateTimeString() is not equal local date tine object '" + a_validLocalDateTime[i] + "'"
                    );

                    if (i == 5)
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
                        o_validLocalDates[i], Is.EqualTo(ForestNETLib.Core.Helper.FromDateString(s_testTrue)),
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
                    new(05, 02, 03),
                    new(0, 05, 02, 03, 576)
                };

                string[] a_testTrue3rd = new string[] {
                    "05:02",
                    "05:02",
                    "05:02:03",
                    "05:02:03",
                    "05:02:03",
                    "05:02:03.576",
                    "05:02",
                    "05:02",
                    "05:02:03",
                    "05:02:03",
                    "05:02:03",
                    "05:02:03.576",
                    "05:02",
                    "05:02",
                    "05:02:03",
                    "05:02:03",
                    "05:02:03",
                    "05:02:03.576",
                    "05:02",
                    "05:02",
                    "05:02:03",
                    "05:02:03",
                    "05:02:03",
                    "05:02:03.576",
                    "05:02",
                    "05:02",
                    "05:02:03",
                    "05:02:03",
                    "05:02:03",
                    "05:02:03.576",
                    "05:02",
                    "05:02",
                    "05:02:03",
                    "05:02:03",
                    "05:02:03",
                    "05:02:03.576",
                    "05:02",
                    "05:02",
                    "05:02:03",
                    "05:02:03",
                    "05:02:03",
                    "05:02:03.576"
                };

                i = 0;

                foreach (string s_testTrue in a_testTrue3rd)
                {
                    Assert.That(
                        o_validLocalTimes[i], Is.EqualTo(ForestNETLib.Core.Helper.FromTimeString(s_testTrue)),
                        "'" + s_testTrue + "' fromTimeString() is not equal local date time object '" + o_validLocalTimes[i] + "'"
                    );

                    if (i == 5)
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
                    TimeZoneInfo.ConvertTime(TimeZoneInfo.ConvertTimeToUtc(new DateTime(2020, 03, 14, 06, 02, 03, DateTimeKind.Unspecified), TimeZoneInfo.Local), TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time")),
                    TimeZoneInfo.ConvertTime(TimeZoneInfo.ConvertTimeToUtc(new DateTime(2020, 03, 14, 06, 02, 03, 576, DateTimeKind.Unspecified), TimeZoneInfo.Local), TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time"))
                };

                a_testFalse = new string[] {
                    "03-14-2020 05:02",
                    "03-14-2020T05:02",
                    "03-14-2020 05:02:03",
                    "03-14-2020T05:02:03",
                    "03-14-2020T05:02:03Z",
                    "03-14-2020T05:02:03.576Z",
                    "14-03-2020 55:02",
                    "14-03-2020T55:02",
                    "14-03-2020 55:02:03",
                    "14-03-2020T55:02:03",
                    "14-03-2020T55:02:03Z",
                    "03-14-2020T05:02:03.5Z",

                    "03.14.2020 05:02",
                    "03.14.2020T05:02",
                    "03.14.2020 05:02:03",
                    "03.14.2020T05:02:03",
                    "03.14.2020T05:02:03Z",
                    "03.14.2020T05:02:03.576",
                    "14.03.2020 55:02",
                    "14.03.2020T55:02",
                    "14.03.2020 55:02:03",
                    "14.03.2020T55:02:03",
                    "14.03.2020T55:02:03Z",
                    "03.14.2020T05:02:03.5",

                    "14/13/2020 05:02",
                    "14/13/2020T05:02",
                    "14/13/2020 05:02:03",
                    "14/13/2020T05:02:03",
                    "14/13/2020T05:02:03Z",
                    "14/13/2020T05:02:03.576Z",
                    "14/03/2020 55:02",
                    "14/03/2020T55:02",
                    "14/03/2020 55:02:03",
                    "14/03/2020T55:02:03",
                    "14/03/2020T55:02:03Z",
                    "14/13/2020T05:02:03.5Z",

                    "13/14/2020 05:02",
                    "13/14/2020T05:02",
                    "13/14/2020 05:02:03",
                    "13/14/2020T05:02:03",
                    "13/14/2020T05:02:03Z",
                    "13/14/2020T05:02:03.576",
                    "03/14/2020 55:02",
                    "03/14/2020T55:02",
                    "03/14/2020 55:02:03",
                    "03/14/2020T55:02:03",
                    "03/14/2020T55:02:03Z",
                    "13/14/2020T05:02:03.5",

                    "2020-14-03 05:02",
                    "2020-14-03T05:02",
                    "2020-14-03 05:02:03",
                    "2020-14-03T05:02:03",
                    "2020-14-03T05:02:03Z",
                    "2020-14-03T05:02:03.576Z",
                    "2020-03-14 55:02",
                    "2020-03-14T55:02",
                    "2020-03-14 55:02:03",
                    "2020-03-14T55:02:03",
                    "2020-03-14T55:02:03Z",
                    "2020-14-03T05:02:03.5Z",

                    "2020/13/14 05:02",
                    "2020/13/14T05:02",
                    "2020/13/14 05:02:03",
                    "2020/13/14T05:02:03",
                    "2020/13/14T05:02:03Z",
                    "2020/13/14T05:02:03.576",
                    "2020/03/14 55:02",
                    "2020/03/14T55:02",
                    "2020/03/14 55:02:03",
                    "2020/03/14T55:02:03",
                    "2020/03/14T55:02:03Z",
                    "2020/13/14T05:02:03.5",

                    "2020/14/13 05:02",
                    "2020/14/13T05:02",
                    "2020/14/13 05:02:03",
                    "2020/14/13T05:02:03",
                    "2020/14/13T05:02:03Z",
                    "2020/14/13T05:02:03.576Z",
                    "2020/14/03 55:02",
                    "2020/14/03T55:02",
                    "2020/14/03 55:02:03",
                    "2020/14/03T55:02:03",
                    "2020/14/03T55:02:03Z",
                    "2020/14/13T05:02:03.5Z"
                };

                i = 0;

                foreach (string s_testFalse in a_testFalse)
                {
                    bool b_check = true;

                    try
                    {
                        ForestNETLib.Core.Helper.FromISO8601UTC(s_testFalse);
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
                        ForestNETLib.Core.Helper.FromDateTimeString(s_testFalse);
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
                        ForestNETLib.Core.Helper.FromDateString(s_testFalse);
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
                        ForestNETLib.Core.Helper.FromTimeString(s_testFalse);
                    }
                    catch (Exception)
                    {
                        b_check = false;
                    }

                    if (b_check)
                    {
                        Assert.Fail("'" + s_testFalse + "' fromTimeString() could be parsed to local time object '" + a_validLocalDateTime[i] + "'");
                    }

                    if (i == 5)
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
                int i_random = ForestNETLib.Core.Helper.RandomIntegerRange(1, 10);
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
                int i_random = ForestNETLib.Core.Helper.SecureRandomIntegerRange(1, 10);
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
                double d_random = ForestNETLib.Core.Helper.RandomDoubleRange(1.5d, 10.75d);
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

            byte[]? bytes = ForestNETLib.Core.Helper.ShortToByteArray((short)558);
            short sh_test = ForestNETLib.Core.Helper.ByteArrayToShort(bytes);
            Assert.That(
                sh_test, Is.EqualTo(558),
                "short to byte and back to short != 558"
            );
            Assert.That(
                sh_test, Is.Not.EqualTo(31630),
                "short to byte and back to short == 31630"
            );

            Assert.That(
                ForestNETLib.Core.Helper.PrintByteArray(bytes, false), Is.EqualTo("00000010 00101110"),
                "bytes of short are not matching printed byte array"
            );
            Assert.That(
                ForestNETLib.Core.Helper.PrintByteArray(bytes, true), Is.EqualTo("00000000 00000000 00000010 00101110"),
                "bytes of short are not matching printed byte array"
            );

            bytes = ForestNETLib.Core.Helper.ShortToByteArray((short)25134);
            sh_test = ForestNETLib.Core.Helper.ByteArrayToShort(bytes);
            Assert.That(
                sh_test, Is.EqualTo(25134),
                "short to byte and back to short != 25134"
            );
            Assert.That(
                sh_test, Is.Not.EqualTo(31630),
                "short to byte and back to short == 31630"
            );

            Assert.That(
                ForestNETLib.Core.Helper.PrintByteArray(bytes, false), Is.EqualTo("01100010 00101110"),
                "bytes of short are not matching printed byte array"
            );
            Assert.That(
                ForestNETLib.Core.Helper.PrintByteArray(bytes, true), Is.EqualTo("00000000 00000000 01100010 00101110"),
                "bytes of short are not matching printed byte array"
            );

            bytes = ForestNETLib.Core.Helper.IntToByteArray(916040294);
            int i_test = ForestNETLib.Core.Helper.ByteArrayToInt(bytes);
            Assert.That(
                i_test, Is.EqualTo(916040294),
                "int to byte and back to int != 916040294"
            );
            Assert.That(
                i_test, Is.Not.EqualTo(116040294),
                "int to byte and back to int == 116040294"
            );

            Assert.That(
                ForestNETLib.Core.Helper.PrintByteArray(bytes), Is.EqualTo("00110110 10011001 10101010 01100110"),
                "bytes of int are not matching printed byte array"
            );
            Assert.That(
                ForestNETLib.Core.Helper.PrintByteArray(bytes, false), Is.EqualTo("00110110 10011001 10101010 01100110"),
                "bytes of int are not matching printed byte array"
            );

            bytes = ForestNETLib.Core.Helper.IntToByteArray(10070630);
            i_test = ForestNETLib.Core.Helper.ByteArrayToInt(bytes);
            Assert.That(
                i_test, Is.EqualTo(10070630),
                "int to byte and back to int != 10070630"
            );
            Assert.That(
                i_test, Is.Not.EqualTo(90070630),
                "int to byte and back to int == 90070630"
            );

            Assert.That(
                ForestNETLib.Core.Helper.PrintByteArray(bytes), Is.EqualTo("00000000 10011001 10101010 01100110"),
                "bytes of int are not matching printed byte array"
            );
            Assert.That(
                ForestNETLib.Core.Helper.PrintByteArray(bytes, false), Is.EqualTo("10011001 10101010 01100110"),
                "bytes of int are not matching printed byte array"
            );

            bytes = ForestNETLib.Core.Helper.IntToByteArray(43622);
            i_test = ForestNETLib.Core.Helper.ByteArrayToInt(bytes);
            Assert.That(
                i_test, Is.EqualTo(43622),
                "int to byte and back to int != 43622"
            );
            Assert.That(
                i_test, Is.Not.EqualTo(13622),
                "int to byte and back to int == 13622"
            );

            Assert.That(
                ForestNETLib.Core.Helper.PrintByteArray(bytes), Is.EqualTo("00000000 00000000 10101010 01100110"),
                "bytes of int are not matching printed byte array"
            );
            Assert.That(
                ForestNETLib.Core.Helper.PrintByteArray(bytes, false), Is.EqualTo("10101010 01100110"),
                "bytes of int are not matching printed byte array"
            );

            bytes = ForestNETLib.Core.Helper.IntToByteArray(102);
            i_test = ForestNETLib.Core.Helper.ByteArrayToInt(bytes);
            Assert.That(
                i_test, Is.EqualTo(102),
                "int to byte and back to int != 102"
            );
            Assert.That(
                i_test, Is.Not.EqualTo(902),
                "int to byte and back to int == 902"
            );

            Assert.That(
                ForestNETLib.Core.Helper.PrintByteArray(bytes), Is.EqualTo("00000000 00000000 00000000 01100110"),
                "bytes of int are not matching printed byte array"
            );
            Assert.That(
                ForestNETLib.Core.Helper.PrintByteArray(bytes, false), Is.EqualTo("01100110"),
                "bytes of int are not matching printed byte array"
            );

            bytes = ForestNETLib.Core.Helper.LongToByteArray(9070052179665454);
            long l_test = ForestNETLib.Core.Helper.ByteArrayToLong(bytes);
            Assert.That(
                l_test, Is.EqualTo(9070052179665454),
                "long to byte and back to long != 9070052179665454"
            );
            Assert.That(
                l_test, Is.Not.EqualTo(4620756070607053358),
                "long to byte and back to long == 4620756070607053358"
            );

            Assert.That(
                ForestNETLib.Core.Helper.PrintByteArray(bytes, false), Is.EqualTo("00100000 00111001 00101010 00010110 01000011 01100010 00101110"),
                "bytes of long " + ForestNETLib.Core.Helper.PrintByteArray(bytes, false) + " are not matching printed byte array"
            );

            bytes = ForestNETLib.Core.Helper.LongToByteArray(3467834566000206382);
            l_test = ForestNETLib.Core.Helper.ByteArrayToLong(bytes);
            Assert.That(
                l_test, Is.EqualTo(3467834566000206382),
                "long to byte and back to long != 3467834566000206382"
            );
            Assert.That(
                l_test, Is.Not.EqualTo(4620756070607053358),
                "long to byte and back to long == 4620756070607053358"
            );

            Assert.That(
                ForestNETLib.Core.Helper.PrintByteArray(bytes, false), Is.EqualTo("00110000 00100000 00111001 00101010 00010110 01000011 01100010 00101110"),
                "bytes of long are not matching printed byte array"
            );

            bytes = ForestNETLib.Core.Helper.AmountToNByteArray(101458034, 4);
            l_test = ForestNETLib.Core.Helper.ByteArrayToLong(bytes);
            Assert.That(
                l_test, Is.EqualTo(101458034),
                "long to byte and back to long != 101458034"
            );
            Assert.That(
                l_test, Is.Not.EqualTo(4620756070607053358),
                "long to byte and back to long == 4620756070607053358"
            );

            Assert.That(
                ForestNETLib.Core.Helper.PrintByteArray(bytes, false), Is.EqualTo("00000110 00001100 00100000 01110010"),
                "bytes of long are not matching printed byte array"
            );

            bytes = ForestNETLib.Core.Helper.AmountToNByteArray(101458034, 8);
            l_test = ForestNETLib.Core.Helper.ByteArrayToLong(bytes);
            Assert.That(
                l_test, Is.EqualTo(101458034),
                "long to byte and back to long != 101458034"
            );
            Assert.That(
                l_test, Is.Not.EqualTo(4620756070607053358),
                "long to byte and back to long == 4620756070607053358"
            );

            Assert.That(
                ForestNETLib.Core.Helper.PrintByteArray(bytes, false), Is.EqualTo("00000000 00000000 00000000 00000000 00000110 00001100 00100000 01110010"),
                "bytes of long are not matching printed byte array"
            );

            Assert.That(
                ForestNETLib.Core.Helper.FormatBytes(2123456797), Is.EqualTo("2,12 GB"),
                "2123456797 is not 2,12 GB"
            );
            Assert.That(
                ForestNETLib.Core.Helper.FormatBytes(2123456797, true), Is.EqualTo("1,98 GiB"),
                "2123456797 is not 1,98 GiB"
            );

            Assert.That(
                ForestNETLib.Core.Helper.FormatBytes(129456797), Is.Not.EqualTo("2,12 GB"),
                "129456797 is 2,12 GB"
            );
            Assert.That(
                ForestNETLib.Core.Helper.FormatBytes(129456797, true), Is.Not.EqualTo("1,98 GiB"),
                "129456797 is 1,98 GiB"
            );

            Assert.That(
                ForestNETLib.Core.Helper.FormatBytes(126797), Is.EqualTo("126,8 KB"),
                "126797 is not 126,8 KB"
            );
            Assert.That(
                ForestNETLib.Core.Helper.FormatBytes(126797, true), Is.EqualTo("123,83 KiB"),
                "126797 is not 123,83 KiB"
            );

            Assert.That(
                ForestNETLib.Core.Helper.FormatBytes(33126797), Is.Not.EqualTo("126,8 KB"),
                "33126797 is 126,8 KB"
            );
            Assert.That(
                ForestNETLib.Core.Helper.FormatBytes(6797, true), Is.Not.EqualTo("123,83 KiB"),
                "6797 is 123,83 KiB"
            );

            try
            {
                Assert.That(
                    ForestNETLib.Core.Helper.HashByteArray("SHA-256", ForestNETLib.Core.Helper.IntToByteArray(43622)), Is.EqualTo("9F2778195BB08930F6455CA6C191D9DC25B77F33145141A2E89FAC794D5E7C47"),
                    "SHA-256 ist not '9F2778195BB08930F6455CA6C191D9DC25B77F33145141A2E89FAC794D5E7C47'"
                );
                Assert.That(
                    ForestNETLib.Core.Helper.HashByteArray("SHA-384", ForestNETLib.Core.Helper.IntToByteArray(10070630)), Is.EqualTo("4DD7D9B43AFD5CA29A794B59EE924F2226E7776C7D25E052060AB71DCD7254DA9FF5C342F8E943D85336D7D97BAD8CB9"),
                    "SHA-384 ist not '4DD7D9B43AFD5CA29A794B59EE924F2226E7776C7D25E052060AB71DCD7254DA9FF5C342F8E943D85336D7D97BAD8CB9'"
                );
                Assert.That(
                    ForestNETLib.Core.Helper.HashByteArray("SHA-512", ForestNETLib.Core.Helper.IntToByteArray(916040294)), Is.EqualTo("38C320515E85995FC7ACFEFD5126EBA8EDB6133E6E552565899534D03E8AF6D6FF9BB1C165C58BBB43AED8DE01FDD3FB0C9F4F6D384AD8BCD419421AC10AB9C1"),
                    "SHA-512 ist not '38C320515E85995FC7ACFEFD5126EBA8EDB6133E6E552565899534D03E8AF6D6FF9BB1C165C58BBB43AED8DE01FDD3FB0C9F4F6D384AD8BCD419421AC10AB9C1'"
                );

                Assert.That(
                    ForestNETLib.Core.Helper.HashByteArray("SHA-256", ForestNETLib.Core.Helper.IntToByteArray(43622)), Is.Not.EqualTo("9F2778195BB08930F6455CA6C191D9DC25C77F33145141A2E89FAC794D5E7C47"),
                    "SHA-256 is '9F2778195BB08930F6455CA6C191D9DC25C77F33145141A2E89FAC794D5E7C47'"
                );
                Assert.That(
                    ForestNETLib.Core.Helper.HashByteArray("SHA-384", ForestNETLib.Core.Helper.IntToByteArray(10070630)), Is.Not.EqualTo("4DD7D9B43AFD5CA29A794B59EE924F2226E7776C7D25E052065AB71DCD7254DA9FF5C342F8E943D85336D7D97BAD8CB9"),
                    "SHA-384 is '4DD7D9B43AFD5CA29A794B59EE924F2226E7776C7D25E052065AB71DCD7254DA9FF5C342F8E943D85336D7D97BAD8CB9'"
                );
                Assert.That(
                    ForestNETLib.Core.Helper.HashByteArray("SHA-512", ForestNETLib.Core.Helper.IntToByteArray(916040294)), Is.Not.EqualTo("38C320515E85995FC7ACFEFD5126EBA8EDB6133E6E552565899534D03E8AF6D6F39BB1C165C58BBB43AED8DE01FDD3FB0C9F4F6D384AD8BCD419421AC10AB9C1"),
                    "SHA-512 is '38C320515E85995FC7ACFEFD5126EBA8EDB6133E6E552565899534D03E8AF6D6F39BB1C165C58BBB43AED8DE01FDD3FB0C9F4F6D384AD8BCD419421AC10AB9C1'"
                );

                Assert.That(
                    ForestNETLib.Core.Helper.BytesToHexString(Encoding.ASCII.GetBytes("Das ist das Haus vom Nikolaus."), true), Is.EqualTo("0x44 0x61 0x73 0x20 0x69 0x73 0x74 0x20 0x64 0x61 0x73 0x20 0x48 0x61 0x75 0x73 0x20 0x76 0x6F 0x6D 0x20 0x4E 0x69 0x6B 0x6F 0x6C 0x61 0x75 0x73 0x2E"),
                    "BytesToHexString result for 'Das ist das Haus vom Nikolaus.' does not match expected value."
                );
                Assert.That(
                    ForestNETLib.Core.Helper.BytesToHexString(Encoding.ASCII.GetBytes("Das ist das Haus vom Nikolaus."), false), Is.EqualTo("4461732069737420646173204861757320766F6D204E696B6F6C6175732E"),
                    "BytesToHexString result for 'Das ist das Haus vom Nikolaus.' does not match expected value."
                );
                Assert.That(
                    ForestNETLib.Core.Helper.BytesToHexString(Encoding.GetEncoding("ISO-8859-1").GetBytes("ABCDEFGHIJKL€"), true), Is.EqualTo("0x41 0x42 0x43 0x44 0x45 0x46 0x47 0x48 0x49 0x4A 0x4B 0x4C 0x3F"),
                    "BytesToHexString result for 'ABCDEFGHIJKL€' does not match expected value for charset 'ISO-8859-1'."
                );
                Assert.That(
                    ForestNETLib.Core.Helper.BytesToHexString(Encoding.UTF8.GetBytes("ABCDEFGHIJKL€"), true), Is.EqualTo("0x41 0x42 0x43 0x44 0x45 0x46 0x47 0x48 0x49 0x4A 0x4B 0x4C 0xE2 0x82 0xAC"),
                    "BytesToHexString result for 'ABCDEFGHIJKL€' does not match expected value for charset 'UTF-8'."
                );
                Assert.That(
                    ForestNETLib.Core.Helper.BytesToHexString(Encoding.GetEncoding("UTF-16").GetBytes("ABCDEFGHIJKL€"), true), Is.EqualTo("0x41 0x00 0x42 0x00 0x43 0x00 0x44 0x00 0x45 0x00 0x46 0x00 0x47 0x00 0x48 0x00 0x49 0x00 0x4A 0x00 0x4B 0x00 0x4C 0x00 0xAC 0x20"),
                    "BytesToHexString result for 'ABCDEFGHIJKL€' does not match expected value for charset 'UTF-16'."
                );
                Assert.That(
                    ForestNETLib.Core.Helper.BytesToHexString(Encoding.Unicode.GetBytes("ABCDEFGHIJKL€"), true), Is.EqualTo("0x41 0x00 0x42 0x00 0x43 0x00 0x44 0x00 0x45 0x00 0x46 0x00 0x47 0x00 0x48 0x00 0x49 0x00 0x4A 0x00 0x4B 0x00 0x4C 0x00 0xAC 0x20"),
                    "BytesToHexString result for 'ABCDEFGHIJKL€' does not match expected value for charset 'Unicode'."
                );

                Assert.That(
                    new byte[18] { 0x44, 0x61, 0x73, 0x20, 0x69, 0x73, 0x74, 0x20, 0x64, 0x61, 0x73, 0x20, 0x48, 0x61, 0x75, 0x73, 0x20, 0x76 }.SequenceEqual(
                    ForestNETLib.Core.Helper.HexStringToBytes("0x44 0x61 0x73 0x20 0x69 0x73 0x74 0x20 0x64 0x61 0x73 0x20 0x48 0x61 0x75 0x73 0x20 0x76")),
                    Is.True,
                    "HexStringToBytes result for '0x44 0x61 0x73 0x20 0x69 0x73 0x74 0x20 0x64 0x61 0x73 0x20 0x48 0x61 0x75 0x73 0x20 0x76' does not match expected value."
                );
                Assert.That(
                    new byte[18] { 0x44, 0x61, 0x73, 0x20, 0x69, 0x73, 0x74, 0x20, 0x64, 0x61, 0x73, 0x20, 0x48, 0x61, 0x75, 0x73, 0x20, 0x76 }.SequenceEqual(
                    ForestNETLib.Core.Helper.HexStringToBytes("446173206973742064617320486175732076")),
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
                ForestNETLib.Core.Helper.JoinList(a_list1, ','), Is.EqualTo("a,bc,def,ghij,klmno"),
                "concated list '" + ForestNETLib.Core.Helper.JoinList(a_list1, ',') + "' is not equal 'a,bc,def,ghij,klmno'"
            );

            List<int> a_list2 = [.. new[] { 1, 23, 45, 678, 910111213 }];
            Assert.That(
                ForestNETLib.Core.Helper.JoinList(a_list2, ':'), Is.EqualTo("1:23:45:678:910111213"),
                "concated list '" + ForestNETLib.Core.Helper.JoinList(a_list1, ':') + "' is not equal '1:23:45:678:910111213'"
            );

            Assert.That(
                ForestNETLib.Core.Helper.GetIndexOfObjectInList([.. new[] { "two", "one", "three" }], "one"), Is.GreaterThan(0),
                "'one' not found and no index returned from array list"
            );
            Assert.That(
                ForestNETLib.Core.Helper.GetIndexOfObjectInList([.. new[] { "two", "four", "three" }], "one"), Is.LessThanOrEqualTo(0),
                "'one' found and index returned from array list"
            );

            Assert.That(
                ForestNETLib.Core.Helper.IsIndexValid([.. new[] { "two", "one", "three" }], 2),
                Is.True,
                "index '2' is not valid"
            );
            Assert.That(
                ForestNETLib.Core.Helper.IsIndexValid([.. new[] { "two", "four", "three" }], 42),
                Is.False,
                "index '42' is valid"
            );

            TimeSpan o_timeSpan = TimeSpan.FromSeconds(597845641);
            Assert.That(
                ForestNETLib.Core.Helper.FormatTimeSpan(o_timeSpan), Is.EqualTo("2767:48:14"),
                "duration is not '2767:48:14'"
            );
            Assert.That(
                ForestNETLib.Core.Helper.FormatTimeSpan(o_timeSpan), Is.Not.EqualTo("767:18:14"),
                "duration is '767:48:14'"
            );

            string s_random = ForestNETLib.Core.Helper.GenerateRandomString(32);

            Assert.That(
                s_random, Has.Length.EqualTo(32),
                "random generated string has not a length of 32 characters"
            );

            s_random = ForestNETLib.Core.Helper.GenerateRandomString(10, ForestNETLib.Core.Helper.DIGITS_CHARACTERS);

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

            s_random = ForestNETLib.Core.Helper.GenerateUUID();

            Assert.That(
                s_random, Has.Length.EqualTo(36),
                "random generated string has not a length of 36 characters"
            );

            string s_original = "ftps://user:password@expample.com:21";
            string s_disguised = ForestNETLib.Core.Helper.DisguiseSubstring(s_original, "//", "@", '*');

            Assert.That(
                s_disguised, Is.EqualTo("ftps://*************@expample.com:21"),
                "disguised string is not 'ftps://*************@expample.com:21', but '" + s_disguised + "'"
            );

            s_original = "This is just a \"test\"";
            s_disguised = ForestNETLib.Core.Helper.DisguiseSubstring(s_original, "\"", "\"", '+');

            Assert.That(
                s_disguised, Is.EqualTo("This is just a \"++++\""),
                "disguised string is not 'This is just a \"++++\"', but '" + s_disguised + "'"
            );

            s_original = "No disguise at all";
            s_disguised = ForestNETLib.Core.Helper.DisguiseSubstring(s_original, ".", "&", '_');

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

            uint[] a_range = ForestNETLib.Core.Helper.GetRangeOfSubnet("192.168.2.100/24");

            Assert.That(
                ForestNETLib.Core.Helper.Ipv4IntToString(a_range[0]), Is.EqualTo("192.168.2.1"),
                "conversion from ipv4 int to '192.168.2.1' failed, result is '" + ForestNETLib.Core.Helper.Ipv4IntToString(a_range[0]) + "'"
            );

            Assert.That(
                ForestNETLib.Core.Helper.Ipv4IntToString(a_range[1]), Is.EqualTo("192.168.2.254"),
                "conversion from ipv4 int to '192.168.2.254' failed, result is '" + ForestNETLib.Core.Helper.Ipv4IntToString(a_range[0]) + "'"
            );

            bool[] a_ipAddressesResults = new bool[] {
                false, true, false, true, false, true, false, true, true
            };

            for (int i = 0; i < a_ipAddressesResults.Length; i++)
            {
                Assert.That(
                    ForestNETLib.Core.Helper.IsIpv4Address(a_ipAddresses[i * 2]),
                    Is.True,
                    "'" + a_ipAddresses[i * 2] + "' is not an ipv4 address"
                );

                Assert.That(
                    ForestNETLib.Core.Helper.IsIpv4AddressWithSuffix(a_ipAddresses[(i * 2) + 1]),
                    Is.True,
                    "'" + a_ipAddresses[(i * 2) + 1] + "' is not an ipv4 address with suffix"
                );

                Assert.That(
                    ForestNETLib.Core.Helper.Ipv4IntToString(ForestNETLib.Core.Helper.Ipv4ToInt(a_ipAddresses[i * 2])), Is.EqualTo(a_ipAddresses[i * 2]),
                    "conversion from ipv4 int and back for '" + a_ipAddresses[i * 2] + "' failed"
                );

                Assert.That(
                    a_ipAddressesResults[i], Is.EqualTo(ForestNETLib.Core.Helper.IsIpv4WithinRange(a_ipAddresses[i * 2], a_ipAddresses[(i * 2) + 1])),
                    "ip '" + a_ipAddresses[i * 2] + "' is " + ((!a_ipAddressesResults[i]) ? "not" : "") + " within '" + a_ipAddresses[(i * 2) + 1] + "'"
                );
            }

            Assert.That(
                ForestNETLib.Core.Helper.IsIpv4MulticastAddress("225.4.228.87"),
                Is.True,
                "'225.4.228.87' is not an ipv4 multicast address"
            );

            Assert.That(
                ForestNETLib.Core.Helper.IsIpv4MulticastAddress("225.4.228.87/16"),
                Is.False,
                "'225.4.228.87/16' is not an ipv4 multicast address"
            );

            Assert.That(
                ForestNETLib.Core.Helper.IsIpv4MulticastAddress("192.4.228.87"),
                Is.False,
                "'192.4.228.87' is not an ipv4 multicast address"
            );

            Assert.That(
                ForestNETLib.Core.Helper.IsIpv4MulticastAddress("240.4.228.87"),
                Is.False,
                "'240.4.228.87' is not an ipv4 multicast address"
            );

            Assert.That(
                ForestNETLib.Core.Helper.IsIpv4MulticastAddress("239.4.228.87"),
                Is.True,
                "'239.4.228.87' is not an ipv4 multicast address"
            );

            Assert.That(
                ForestNETLib.Core.Helper.IsIpv6Address("2001:0db8:85a3:08d3:1319:8a2e:0370:7347"),
                Is.True,
                "'2001:0db8:85a3:08d3:1319:8a2e:0370:7347' is not an ipv6 address"
            );

            Assert.That(
                ForestNETLib.Core.Helper.IsIpv6Address("fe80::ca32:231b:f27e:b696"),
                Is.True,
                "'fe80::ca32:231b:f27e:b696' is not an ipv6 address"
            );

            Assert.That(
                ForestNETLib.Core.Helper.IsIpv6Address("FE80:CD00:0000:0CDE:1257:0000:211E:729C"),
                Is.True,
                "'FE80:CD00:0000:0CDE:1257:0000:211E:729C' is not an ipv6 address"
            );

            Assert.That(
                ForestNETLib.Core.Helper.IsIpv6Address("fe80::ca32:231b:f27z:b696"),
                Is.False,
                "'fe80::ca32:231b:f27z:b696' is an ipv6 address"
            );

            Assert.That(
                ForestNETLib.Core.Helper.IsIpv6MulticastAddress("FF05::342"),
                Is.True,
                "'FF05::342' is not an ipv6 multicast address"
            );

            Assert.That(
                ForestNETLib.Core.Helper.IsIpv6MulticastAddress("FF05:0:0:0:0:0:0:342"),
                Is.True,
                "'FF05:0:0:0:0:0:0:342' is not an ipv6 multicast address"
            );

            Assert.That(
                ForestNETLib.Core.Helper.IsIpv6MulticastAddress("ff02::2"),
                Is.True,
                "'ff02::2' is not an ipv6 multicast address"
            );

            Assert.That(
                ForestNETLib.Core.Helper.IsIpv6MulticastAddress("FE80:CD00:0000:0CDE:1257:0000:211E:729C"),
                Is.False,
                "'FE80:CD00:0000:0CDE:1257:0000:211E:729C' is an ipv6 multicast address"
            );

            try
            {
                CompareObject o_compareOne = new("One", 1, new DateTime(2020, 06, 21), new long[] { 42135792468, 21135792468, 12135792468, 14378135792468 }, [.. new short[] { 42, 21, 12, 14378 }], new SubCompareObject(42.125d, true, new Decimal[] { 1.602176634m, 8.8541878128m, 6.62607015m, 9.80665m, 3.14159265359m }, new DateTime(2020, 12, 21)));
                /* o_compareOne and o_compareTwo are identical */
                CompareObject o_compareTwo = new("One", 1, new DateTime(2020, 06, 21), new long[] { 42135792468, 21135792468, 12135792468, 14378135792468 }, [.. new short[] { 42, 21, 12, 14378 }], new SubCompareObject(42.125d, true, new Decimal[] { 1.602176634m, 8.8541878128m, 6.62607015m, 9.80665m, 3.14159265359m }, new DateTime(2020, 12, 21)));
                /* o_compareOne and o_compareThree are not identical -> see DateTime of CompareObject instance ... it is 12 and not 21 && see Boolean of SubCompareObject instance ... it is false and not true */
                CompareObject o_compareThree = new("One", 1, new DateTime(2020, 06, 12), new long[] { 42135792468, 21135792468, 12135792468, 14378135792468 }, [.. new short[] { 42, 21, 12, 14378 }], new SubCompareObject(42.125d, false, new Decimal[] { 1.602176634m, 8.8541878128m, 6.62607015m, 9.80665m, 3.14159265359m }, new DateTime(2020, 12, 21)));
                /* o_compareOne and o_compareFour are not identical, but only with deep comparison -> see DateTime of SubCompareObject instance ... it is 1920 and not 2020 */
                CompareObject o_compareFour = new("One", 1, new DateTime(2020, 06, 21), new long[] { 42135792468, 21135792468, 12135792468, 14378135792468 }, [.. new short[] { 42, 21, 12, 14378 }], new SubCompareObject(42.125d, true, new Decimal[] { 1.602176634m, 8.8541878128m, 6.62607015m, 9.80665m, 3.14159265359m }, new DateTime(1920, 12, 21)));

                CompareObjectProperties o_compareOneProp = new("One", 1, new DateTime(2020, 06, 21), new long[] { 42135792468, 21135792468, 12135792468, 14378135792468 }, [.. new short[] { 42, 21, 12, 14378 }], new SubCompareObjectProperties(42.125d, true, new Decimal[] { 1.602176634m, 8.8541878128m, 6.62607015m, 9.80665m, 3.14159265359m }, new DateTime(2020, 12, 21)));
                /* o_compareOneProp and o_compareTwoProp are identical */
                CompareObjectProperties o_compareTwoProp = new("One", 1, new DateTime(2020, 06, 21), new long[] { 42135792468, 21135792468, 12135792468, 14378135792468 }, [.. new short[] { 42, 21, 12, 14378 }], new SubCompareObjectProperties(42.125d, true, new Decimal[] { 1.602176634m, 8.8541878128m, 6.62607015m, 9.80665m, 3.14159265359m }, new DateTime(2020, 12, 21)));
                /* o_compareOneProp and o_compareThreeProp are not identical -> see DateTime of CompareObjectProperties instance ... it is 12 and not 21 && see Boolean of SubCompareObjectProperties instance ... it is false and not true */
                CompareObjectProperties o_compareThreeProp = new("One", 1, new DateTime(2020, 06, 12), new long[] { 42135792468, 21135792468, 12135792468, 14378135792468 }, [.. new short[] { 42, 21, 12, 14378 }], new SubCompareObjectProperties(42.125d, false, new Decimal[] { 1.602176634m, 8.8541878128m, 6.62607015m, 9.80665m, 3.14159265359m }, new DateTime(2020, 12, 21)));
                /* o_compareOneProp and o_compareFourProp are not identical, but only with deep comparison -> see DateTime of SubCompareObjectProperties instance ... it is 1920 and not 2020 */
                CompareObjectProperties o_compareFourProp = new("One", 1, new DateTime(2020, 06, 21), new long[] { 42135792468, 21135792468, 12135792468, 14378135792468 }, [.. new short[] { 42, 21, 12, 14378 }], new SubCompareObjectProperties(42.125d, true, new Decimal[] { 1.602176634m, 8.8541878128m, 6.62607015m, 9.80665m, 3.14159265359m }, new DateTime(1920, 12, 21)));

                List<CompareObject> o_listOne =
                [
                    o_compareOne,
                    o_compareTwo,
                    o_compareThree,
                    o_compareFour
                ];
                /* o_listOne and o_listTwo are identical */
                List<CompareObject> o_listTwo =
                [
                    o_compareOne,
                    o_compareTwo,
                    o_compareThree,
                    o_compareFour
                ];
                /* o_listOne and o_listThree are not identical -> see missing fourth element */
                List<CompareObject> o_listThree =
                [
                    o_compareOne,
                    o_compareTwo,
                    o_compareThree
                ];

                List<CompareObjectProperties> o_listOneProp =
                [
                    o_compareOneProp,
                    o_compareTwoProp,
                    o_compareThreeProp,
                    o_compareFourProp
                ];
                /* o_listOneProp and o_listTwoProp are identical */
                List<CompareObjectProperties> o_listTwoProp =
                [
                    o_compareOneProp,
                    o_compareTwoProp,
                    o_compareThreeProp,
                    o_compareFourProp
                ];
                /* o_listOne and o_listThreeProp are not identical -> see missing fourth element */
                List<CompareObjectProperties> o_listThreeProp =
                [
                    o_compareOneProp,
                    o_compareTwoProp,
                    o_compareThreeProp
                ];

                Assert.That(
                    ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(o_compareOne, o_compareTwo, false),
                    Is.True,
                    "o_compareOne[" + o_compareOne + "] is not equal to o_compareTwo[" + o_compareTwo + "]"
                );

                Assert.That(
                    ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(o_compareOneProp, o_compareTwoProp, true),
                    Is.True,
                    "o_compareOneProp[" + o_compareOneProp + "] is not equal to o_compareTwoProp[" + o_compareTwoProp + "]"
                );

                Assert.That(
                    ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(o_compareOne, o_compareTwo, false, false, true),
                    Is.True,
                    "o_compareOne[" + o_compareOne + "] is not equal to o_compareTwo[" + o_compareTwo + "]"
                );

                Assert.That(
                    ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(o_compareOneProp, o_compareTwoProp, true, false, true),
                    Is.True,
                    "o_compareOneProp[" + o_compareOneProp + "] is not equal to o_compareTwoProp[" + o_compareTwoProp + "]"
                );

                Assert.That(
                    ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(o_compareOne, o_compareThree, false),
                    Is.False,
                    "o_compareOne[" + o_compareOne + "] is equal to o_compareThree[" + o_compareThree + "]"
                );

                Assert.That(
                    ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(o_compareOneProp, o_compareThreeProp, true),
                    Is.False,
                    "o_compareOneProp[" + o_compareOneProp + "] is equal to o_compareThreeProp[" + o_compareThreeProp + "]"
                );

                Assert.That(
                    ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(o_compareOne, o_compareThree, false, false, true),
                    Is.False,
                    "o_compareOne[" + o_compareOne + "] is equal to o_compareThree[" + o_compareThree + "]"
                );

                Assert.That(
                    ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(o_compareOneProp, o_compareThreeProp, true, false, true),
                    Is.False,
                    "o_compareOneProp[" + o_compareOneProp + "] is equal to o_compareThreeProp[" + o_compareThreeProp + "]"
                );

                Assert.That(
                    ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(o_compareOne, o_compareFour, false),
                    Is.True,
                    "o_compareOne[" + o_compareOne + "] is not equal to o_compareFour[" + o_compareFour + "]"
                );

                Assert.That(
                    ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(o_compareOneProp, o_compareFourProp, true),
                    Is.True,
                    "o_compareOneProp[" + o_compareOneProp + "] is not equal to o_compareFourProp[" + o_compareFourProp + "]"
                );

                Assert.That(
                    ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(o_compareOne, o_compareFour, false, false, true),
                    Is.False,
                    "o_compareOne[" + o_compareOne + "] is equal to o_compareFour[" + o_compareFour + "]"
                );

                Assert.That(
                    ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(o_compareOneProp, o_compareFourProp, true, false, true),
                    Is.False,
                    "o_compareOneProp[" + o_compareOneProp + "] is equal to o_compareFourProp[" + o_compareFourProp + "]"
                );

                Assert.That(
                    ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(o_listOne, o_listTwo, false),
                    Is.True,
                    "o_listOne[" + o_listOne + "] is not equal to o_listTwo[" + o_listTwo + "]"
                );

                Assert.That(
                    ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(o_listOneProp, o_listTwoProp, true),
                    Is.True,
                    "o_listOneProp[" + o_listOneProp + "] is not equal to o_listTwoProp[" + o_listTwoProp + "]"
                );

                Assert.That(
                    ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(o_listOne, o_listTwo, false, false, true),
                    Is.True,
                    "o_listOne[" + o_listOne + "] is not equal to o_listTwo[" + o_listTwo + "]"
                );

                Assert.That(
                    ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(o_listOneProp, o_listTwoProp, true, false, true),
                    Is.True,
                    "o_listOneProp[" + o_listOneProp + "] is not equal to o_listTwoProp[" + o_listTwoProp + "]"
                );

                Assert.That(
                    ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(o_listOne, o_listThree, false),
                    Is.False,
                    "o_listOne[" + o_listOne + "] is equal to o_listThree[" + o_listThree + "]"
                );

                Assert.That(
                    ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(o_listOneProp, o_listThreeProp, true),
                    Is.False,
                    "o_listOneProp[" + o_listOneProp + "] is equal to o_listThreeProp[" + o_listThreeProp + "]"
                );

                Assert.That(
                    ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(o_listOne, o_listThree, false, false, true),
                    Is.False,
                    "o_listOne[" + o_listOne + "] is equal to o_listThree[" + o_listThree + "]"
                );

                Assert.That(
                    ForestNETLib.Core.Helper.ObjectsEqualUsingReflections(o_listOneProp, o_listThreeProp, true, false, true),
                    Is.False,
                    "o_listOneProp[" + o_listOneProp + "] is equal to o_listThreeProp[" + o_listThreeProp + "]"
                );
            }
            catch (Exception o_exc)
            {
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
        }
    }

    class CompareObject
    {
        public string ValueStr;
        public int ValueInt;
        public DateTime ValueDateTime;
        public long[] ValueLongArray;
        public List<short> ValueListShorts;
        public SubCompareObject ValueSubObject;

        public CompareObject(string p_s_value, int p_i_value, DateTime p_o_value, long[] p_as_value, List<short> p_a_value, SubCompareObject p_o_subValue)
        {
            this.ValueStr = p_s_value;
            this.ValueInt = p_i_value;
            this.ValueDateTime = p_o_value;
            this.ValueLongArray = p_as_value;
            this.ValueListShorts = p_a_value;
            this.ValueSubObject = p_o_subValue;
        }

        override public string ToString()
        {
            return this.ValueStr + "|" + this.ValueInt + "|" + this.ValueDateTime + "|" + String.Join(",", this.ValueLongArray) + "|" + ForestNETLib.Core.Helper.JoinList<short>(this.ValueListShorts, ',') + "|" + this.ValueSubObject;
        }
    }

    class SubCompareObject
    {
        public double ValueDbl;
        public bool ValueBool;
        public Decimal[] ValueDecimalArray;
        public DateTime ValueSubDateTime;

        public SubCompareObject(double p_d_value, bool p_b_value, Decimal[] p_a_value, DateTime p_o_value)
        {
            this.ValueDbl = p_d_value;
            this.ValueBool = p_b_value;
            this.ValueDecimalArray = p_a_value;
            this.ValueSubDateTime = p_o_value;
        }

        override public string ToString()
        {
            return this.ValueDbl + "|" + this.ValueBool + "|" + String.Join(",", this.ValueDecimalArray) + "|" + this.ValueSubDateTime;
        }
    }

    class CompareObjectProperties
    {
        public string ValueStr { get; set; }
        public int ValueInt { get; set; }
        public DateTime ValueDateTime { get; set; }
        public long[] ValueLongArray { get; set; }
        public List<short> ValueListShorts { get; set; }
        public SubCompareObjectProperties ValueSubObject { get; set; }

        public CompareObjectProperties(string p_s_value, int p_i_value, DateTime p_o_value, long[] p_as_value, List<short> p_a_value, SubCompareObjectProperties p_o_subValue)
        {
            this.ValueStr = p_s_value;
            this.ValueInt = p_i_value;
            this.ValueDateTime = p_o_value;
            this.ValueLongArray = p_as_value;
            this.ValueListShorts = p_a_value;
            this.ValueSubObject = p_o_subValue;
        }

        override public string ToString()
        {
            return this.ValueStr + "|" + this.ValueInt + "|" + this.ValueDateTime + "|" + String.Join(",", this.ValueLongArray) + "|" + ForestNETLib.Core.Helper.JoinList<short>(this.ValueListShorts, ',') + "|" + this.ValueSubObject;
        }
    }

    class SubCompareObjectProperties
    {
        public double ValueDbl { get; set; }
        public bool ValueBool { get; set; }
        public Decimal[] ValueDecimalArray { get; set; }
        public DateTime ValueSubDateTime { get; set; }

        public SubCompareObjectProperties(double p_d_value, bool p_b_value, Decimal[] p_a_value, DateTime p_o_value)
        {
            this.ValueDbl = p_d_value;
            this.ValueBool = p_b_value;
            this.ValueDecimalArray = p_a_value;
            this.ValueSubDateTime = p_o_value;
        }

        override public string ToString()
        {
            return this.ValueDbl + "|" + this.ValueBool + "|" + String.Join(",", this.ValueDecimalArray) + "|" + this.ValueSubDateTime;
        }
    }
}
