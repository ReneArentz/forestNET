namespace Sandbox.Tests.Net.Https
{
    public class NumberConv
    {
        public class NumberToWords
        {
            public long ubiNum;

            public override string ToString()
            {
                return "ubiNum = " + this.ubiNum;
            }
        }

        public class NumberToWordsResponse
        {
            public string? NumberToWordsResult;

            public override string ToString()
            {
                return "NumberToWordsResult = " + this.NumberToWordsResult;
            }
        }

        public class NumberToDollars
        {
            public decimal dNum;

            public override string ToString()
            {
                return "dNum = " + this.dNum;
            }
        }

        public class NumberToDollarsResponse
        {
            public string? NumberToDollarsResult;

            public override string ToString()
            {
                return "NumberToDollarsResult = " + this.NumberToDollarsResult;
            }
        }

    }
}
