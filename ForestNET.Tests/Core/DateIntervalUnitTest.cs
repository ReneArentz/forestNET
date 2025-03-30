namespace ForestNET.Tests.Core
{
    public class DateIntervalUnitTest
    {
        [Test]
        public void TestDateInterval()
        {
            ForestNET.Lib.DateInterval o_dateInterval;

            try
            {
                o_dateInterval = new("P1Y2M3DT4H5M6S");

                Assert.That(
                    o_dateInterval.ToString(), Is.EqualTo("1 Jahr(e) 2 Monat(e) 3 Tag(e) 4 Stunde(n) 5 Minute(n) 6 Sekunde(n)"),
                    "!= 1 Jahr(e) 2 Monat(e) 3 Tag(e) 4 Stunde(n) 5 Minute(n) 6 Sekunde(n)"
                );
                Assert.That(
                    o_dateInterval.ToDuration(), Is.EqualTo(273906000),
                    "!= 273906000"
                );

                Assert.That(
                    o_dateInterval.ToString(), Is.Not.EqualTo("1 Jahr 6 Sekund"),
                    "== 1 Jahr 6 Sekund"
                );
                Assert.That(
                    o_dateInterval.ToDuration(), Is.Not.EqualTo(1234),
                    "== 1234"
                );

                o_dateInterval = new("P1Y2M3D");

                Assert.That(
                    o_dateInterval.ToString(), Is.EqualTo("1 Jahr(e) 2 Monat(e) 3 Tag(e)"),
                    "!= 1 Jahr(e) 2 Monat(e) 3 Tag(e)"
                );
                Assert.That(
                    o_dateInterval.ToDuration(), Is.EqualTo(259200000),
                    "!= 259200000"
                );

                Assert.That(
                    o_dateInterval.ToString(), Is.Not.EqualTo("1 Jahr 6 Sekund"),
                    "== 1 Jahr 6 Sekund"
                );
                Assert.That(
                    o_dateInterval.ToDuration(), Is.Not.EqualTo(1234),
                    "== 1234"
                );

                o_dateInterval = new("PT4H5M6S");

                Assert.That(
                    o_dateInterval.ToString(), Is.EqualTo("4 Stunde(n) 5 Minute(n) 6 Sekunde(n)"),
                    "!= 4 Stunde(n) 5 Minute(n) 6 Sekunde(n)"
                );
                Assert.That(
                    o_dateInterval.ToDuration(), Is.EqualTo(14706000),
                    "!= 14706000"
                );

                Assert.That(
                    o_dateInterval.ToString(), Is.Not.EqualTo("1 Jahr 6 Sekund"),
                    "== 1 Jahr 6 Sekund"
                );
                Assert.That(
                    o_dateInterval.ToDuration(), Is.Not.EqualTo(1234),
                    "== 1234"
                );
            }
            catch (Exception o_exc)
            {
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }

            try
            {
                o_dateInterval = new(null);
            }
            catch (ArgumentException o_exc)
            {
                Assert.That(
                    o_exc.Message, Is.EqualTo("String encoded date interval parameter is null or empty."),
                    "wrong exception"
                );
            }

            try
            {
                o_dateInterval = new("");
            }
            catch (ArgumentException o_exc)
            {
                Assert.That(
                    o_exc.Message, Is.EqualTo("String encoded date interval parameter is null or empty."),
                    "wrong exception"
                );
            }

            try
            {
                o_dateInterval = new("PT2Mq");
            }
            catch (ArgumentException o_exc)
            {
                Assert.That(
                    o_exc.Message, Is.EqualTo("Parameter[PT2Mq] does not match date interval format."),
                    "wrong exception"
                );
            }

            o_dateInterval = new("P1Y");

            try
            {
                o_dateInterval = new("PT2Mq");
            }
            catch (ArgumentException)
            {
                o_dateInterval.SetDateInterval(new("PT2M"));

                Assert.That(
                    o_dateInterval.ToString(), Is.EqualTo("2 Minute(n)"),
                    "!= 2 Minute(n)"
                );
                Assert.That(
                    o_dateInterval.ToDuration(), Is.EqualTo(120000),
                    "!= 120000"
                );

                Assert.That(
                    o_dateInterval.ToString(), Is.Not.EqualTo("1 Jahr 6 Sekund"),
                    "== 1 Jahr 6 Sekund"
                );
                Assert.That(
                    o_dateInterval.ToDuration(), Is.Not.EqualTo(1234),
                    "== 1234"
                );
            }

            try
            {
                o_dateInterval = new("P1Y2M3DT4H5M6S");

                Assert.That(
                    o_dateInterval.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-GB")), Is.EqualTo("1 year(s) 2 month(s) 3 day(s) 4 hour(s) 5 minute(s) 6 second(s)"),
                    "!= 1 year(s) 2 month(s) 3 day(s) 4 hour(s) 5 minute(s) 6 second(s)"
                );
                Assert.That(
                    o_dateInterval.ToDuration(), Is.EqualTo(273906000),
                    "!= 273906000"
                );

                Assert.That(
                    o_dateInterval.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-GB")), Is.Not.EqualTo("1 Jahr 6 Sekund"),
                    "== 1 Jahr 6 Sekund"
                );
                Assert.That(
                    o_dateInterval.ToDuration(), Is.Not.EqualTo(1234),
                    "== 1234"
                );

                o_dateInterval = new("P1Y2M3D");

                Assert.That(
                    o_dateInterval.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-GB")), Is.EqualTo("1 year(s) 2 month(s) 3 day(s)"),
                    "!= 1 year(s) 2 month(s) 3 day(s)"
                );
                Assert.That(
                    o_dateInterval.ToDuration(), Is.EqualTo(259200000),
                    "!= 259200000"
                );

                Assert.That(
                    o_dateInterval.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-GB")), Is.Not.EqualTo("1 Jahr 6 Sekund"),
                    "== 1 Jahr 6 Sekund"
                );
                Assert.That(
                    o_dateInterval.ToDuration(), Is.Not.EqualTo(1234),
                    "== 1234"
                );

                o_dateInterval = new("PT4H5M6S");

                Assert.That(
                    o_dateInterval.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-GB")), Is.EqualTo("4 hour(s) 5 minute(s) 6 second(s)"),
                    "!= 4 hour(s) 5 minute(s) 6 second(s)"
                );
                Assert.That(
                    o_dateInterval.ToDuration(), Is.EqualTo(14706000),
                    "!= 14706000"
                );

                Assert.That(
                    o_dateInterval.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-GB")), Is.Not.EqualTo("1 Jahr 6 Sekund"),
                    "== 1 Jahr 6 Sekund"
                );
                Assert.That(
                    o_dateInterval.ToDuration(), Is.Not.EqualTo(1234),
                    "== 1234"
                );
            }
            catch (ArgumentException o_exc)
            {
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
            catch (Exception o_exc)
            {
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }

            try
            {
                o_dateInterval = new("P1Y2M3DT4H5M6S", System.Globalization.CultureInfo.GetCultureInfo("en-GB"));

                Assert.That(
                    o_dateInterval.ToString(), Is.EqualTo("1 year(s) 2 month(s) 3 day(s) 4 hour(s) 5 minute(s) 6 second(s)"),
                    "!= 1 year(s) 2 month(s) 3 day(s) 4 hour(s) 5 minute(s) 6 second(s)"
                );
                Assert.That(
                    o_dateInterval.ToDuration(), Is.EqualTo(273906000),
                    "!= 273906000"
                );

                Assert.That(
                    o_dateInterval.ToString(), Is.Not.EqualTo("1 Jahr 6 Sekund"),
                    "== 1 Jahr 6 Sekund"
                );
                Assert.That(
                    o_dateInterval.ToDuration(), Is.Not.EqualTo(1234),
                    "== 1234"
                );

                o_dateInterval = new("P1Y2M3D", System.Globalization.CultureInfo.GetCultureInfo("en-GB"));

                Assert.That(
                    o_dateInterval.ToString(), Is.EqualTo("1 year(s) 2 month(s) 3 day(s)"),
                    "!= 1 year(s) 2 month(s) 3 day(s)"
                );
                Assert.That(
                    o_dateInterval.ToDuration(), Is.EqualTo(259200000),
                    "!= 259200000"
                );

                Assert.That(
                    o_dateInterval.ToString(), Is.Not.EqualTo("1 Jahr 6 Sekund"),
                    "== 1 Jahr 6 Sekund"
                );
                Assert.That(
                    o_dateInterval.ToDuration(), Is.Not.EqualTo(1234),
                    "== 1234"
                );

                o_dateInterval = new("PT4H5M6S", System.Globalization.CultureInfo.GetCultureInfo("en-GB"));

                Assert.That(
                    o_dateInterval.ToString(), Is.EqualTo("4 hour(s) 5 minute(s) 6 second(s)"),
                    "!= 4 hour(s) 5 minute(s) 6 second(s)"
                );
                Assert.That(
                    o_dateInterval.ToDuration(), Is.EqualTo(14706000),
                    "!= 14706000"
                );

                Assert.That(
                    o_dateInterval.ToString(), Is.Not.EqualTo("1 Jahr 6 Sekund"),
                    "== 1 Jahr 6 Sekund"
                );
                Assert.That(
                    o_dateInterval.ToDuration(), Is.Not.EqualTo(1234),
                    "== 1234"
                );
            }
            catch (Exception o_exc)
            {
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
        }
    }
}
