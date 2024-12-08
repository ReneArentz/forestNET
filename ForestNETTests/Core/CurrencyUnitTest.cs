namespace ForestNETTests.Core
{
    public class CurrencyUnitTest
    {
        [Test]
        public void TestCurrency()
        {
            ForestNETLib.Core.Currency o_currency = new(ForestNETLib.Core.Currency.CurrencyDescription.EUR);
            Assert.That(
                o_currency.ToString(), Is.EqualTo("0,00 EUR"),
                o_currency.ToString() + " != '0,00 EUR'"
            );

            o_currency = new(12.34m, ForestNETLib.Core.Currency.CurrencyDescription.EUR);
            Assert.That(
                o_currency.ToString(), Is.EqualTo("12,34 EUR"),
                o_currency.ToString() + " != '12,34 EUR'"
            );

            o_currency = new(12.3m, ForestNETLib.Core.Currency.CurrencyDescription.EUR);
            Assert.That(
                o_currency.ToString(), Is.EqualTo("12,30 EUR"),
                o_currency.ToString() + " != '12,30 EUR'"
            );

            o_currency = new(.3m, ForestNETLib.Core.Currency.CurrencyDescription.EUR);
            Assert.That(
                o_currency.ToString(), Is.EqualTo("0,30 EUR"),
                o_currency.ToString() + " != '0,30 EUR'"
            );

            o_currency = new(0.3m, ForestNETLib.Core.Currency.CurrencyDescription.EUR);
            Assert.That(
                o_currency.ToString(), Is.EqualTo("0,30 EUR"),
                o_currency.ToString() + " != '0,30 EUR'"
            );

            o_currency = new(.34m, ForestNETLib.Core.Currency.CurrencyDescription.EUR);
            Assert.That(
                o_currency.ToString(), Is.EqualTo("0,34 EUR"),
                o_currency.ToString() + " != '0,34 EUR'"
            );

            o_currency = new(0.34m, ForestNETLib.Core.Currency.CurrencyDescription.EUR);
            Assert.That(
                o_currency.ToString(), Is.EqualTo("0,34 EUR"),
                o_currency.ToString() + " != '0,34 EUR'"
            );

            o_currency = new(1234.98765m, ForestNETLib.Core.Currency.CurrencyDescription.EUR);
            Assert.That(
                o_currency.ToString(), Is.EqualTo("1.234,99 EUR"),
                o_currency.ToString() + " != '1.234,99 EUR'"
            );

            o_currency = new(302020101234.56789m, ForestNETLib.Core.Currency.CurrencyDescription.EUR);
            Assert.That(
                o_currency.ToString(), Is.EqualTo("302.020.101.234,57 EUR"),
                o_currency.ToString() + " != '302.020.101.234,57 EUR'"
            );

            o_currency = new(12.34m, ForestNETLib.Core.Currency.CurrencyDescription.GBP);
            Assert.That(
                o_currency.ToString(), Is.EqualTo("12,34 GBP"),
                o_currency.ToString() + " != '12,34 GBP'"
            );

            o_currency = new(9834.12m, ForestNETLib.Core.Currency.CurrencyDescription.GBP)
            {
                CultureInfo = System.Globalization.CultureInfo.GetCultureInfo("en-GB", true)
            };
            /*
             * I don't know why, but on my VM machine, NumberDecimalDigits from en-GB, en-US or de-DE culture seems to be '3', vor InvariantCulture(no CultureInfo is set) it is '2'
             * maybe in future this bug will be fixed or we must set MinimumFractionDigits property to '2' manually
             */
            Assert.That(
                o_currency.ToString(), Is.EqualTo("9,834.120 GBP"),
                o_currency.ToString() + " != '9,834.120 GBP'"
            );

            o_currency = new(1112.34m, ForestNETLib.Core.Currency.CurrencyDescription.USD, System.Globalization.CultureInfo.GetCultureInfo("en-US"));
            /*
             * I don't know why, but on my VM machine, NumberDecimalDigits from en-GB, en-US or de-DE culture seems to be '3', vor InvariantCulture(no CultureInfo is set) it is '2'
             * maybe in future this bug will be fixed or we must set MinimumFractionDigits property to '2' manually
             */
            Assert.That(
                o_currency.ToString(), Is.EqualTo("1,112.340 USD"),
                o_currency.ToString() + " != '1,112.340 USD'"
            );

            o_currency = new(12.34m, ForestNETLib.Core.Currency.CurrencyDescription.USD)
            {
                DecimalSeparator = '.',
                GroupSeparator = ','
            };
            Assert.That(
                o_currency.ToString(), Is.EqualTo("12.34 USD"),
                o_currency.ToString() + " != '12.34 USD'"
            );

            o_currency = new(1234.98765m, ForestNETLib.Core.Currency.CurrencyDescription.USD)
            {
                DecimalSeparator = '.',
                GroupSeparator = ','
            };
            Assert.That(
                o_currency.ToString(), Is.EqualTo("1,234.99 USD"),
                o_currency.ToString() + " != '1,234.99 USD'"
            );

            o_currency = new(302020101234.56789m, ForestNETLib.Core.Currency.CurrencyDescription.USD)
            {
                DecimalSeparator = '.',
                GroupSeparator = '_'
            };
            Assert.That(
                o_currency.ToString(), Is.EqualTo("302_020_101_234.57 USD"),
                o_currency.ToString() + " != '302_020_101_234.57 USD'"
            );


            o_currency = new(12.34m, ForestNETLib.Core.Currency.CurrencyDescription.YEN);
            Assert.That(
                o_currency.ToString(), Is.EqualTo("12,34 YEN"),
                o_currency.ToString() + " != '12,34 YEN'"
            );

            o_currency = new(12.34m, ForestNETLib.Core.Currency.CurrencyDescription.YEN)
            {
                MinimumIntegerDigits = 6
            };
            Assert.That(
                o_currency.ToString(), Is.EqualTo("000.012,34 YEN"),
                o_currency.ToString() + " != '000.012,34 YEN'"
            );

            o_currency = new(12.12345678909m, ForestNETLib.Core.Currency.CurrencyDescription.YEN)
            {
                MinimumIntegerDigits = 9,
                MinimumFractionDigits = 10
            };
            Assert.That(
                o_currency.ToString(), Is.EqualTo("000.000.012,1234567891 YEN"),
                o_currency.ToString() + " != '000.000.012,1234567891 YEN'"
            );

            o_currency = new(12.123456785m, ForestNETLib.Core.Currency.CurrencyDescription.YEN)
            {
                MinimumIntegerDigits = 9,
                MinimumFractionDigits = 8
            };
            Assert.That(
                o_currency.ToString(MidpointRounding.ToEven), Is.EqualTo("000.000.012,12345678 YEN"),
                o_currency.ToString(MidpointRounding.ToEven) + " != '000.000.012,12345678 YEN'"
            );

            o_currency = new(12.34m, ForestNETLib.Core.Currency.CurrencyDescription.USD)
            {
                DecimalSeparator = '.',
                GroupSeparator = ',',
                UseCurrencySymbol = true
            };
            Assert.That(
                o_currency.ToString(), Is.EqualTo("12.34 $"),
                o_currency.ToString() + " != '12.34 $'"
            );
        }
    }
}
