namespace Sandbox.Tests.Net.Msg
{
    public class SmallMessageObject
    {

        /* Fields */

        /* Properties */

        public bool Bool { get; set; }
        public char Char { get; set; }
        public int[]? IntegerArray { get; set; }
        public List<long?> LongList { get; set; }
        public string? String { get; set; }
        public DateTime? LocalTime { get; set; }
        public DateTime?[]? LocalDateArray { get; set; }
        public List<DateTime?> LocalDateTimeList { get; set; }
        public decimal[]? DecimalArray { get; set; }

        /* Methods */

        public SmallMessageObject()
        {
            LongList = [];
            LocalDateTimeList = [];
        }

        public void InitAll()
        {
            Bool = true;
            Char = (char)242;
            IntegerArray = new int[] { 1, 3, 5, 536870954, -42, 0 };
            LongList = [1L, 3L, 5L, 1170936177994235946L, -42L, 0L, null];
            String = "Hello World!";
            LocalTime = new(1970, 1, 1, 6, 2, 3);
            LocalDateArray = new DateTime?[] { new(2020, 3, 4, 0, 0, 0), new(2020, 6, 8, 0, 0, 0), new(2020, 12, 16, 0, 0, 0), null };
            LocalDateTimeList = [new(2020, 3, 4, 6, 2, 3), new(2020, 6, 8, 9, 24, 16), new(2020, 12, 16, 12, 48, 53), null];
            DecimalArray = new decimal[] { +578875020153.73804901109397m, -36.151686185423327m, +71740124.12171120119m, -2043204985254.1196m, 0m, +601.9924m };
        }

        public void EmptyAll()
        {
            Bool = false;
            Char = (char)0;
            IntegerArray = null;
            LongList = [];
            String = null;
            LocalTime = null;
            LocalDateArray = null;
            LocalDateTimeList = [];
            DecimalArray = null;
        }

        override public string ToString()
        {
            string s_foo = "";

            s_foo += Bool;

            s_foo += " | " + Char;

            if (IntegerArray != null && IntegerArray.Length > 0)
            {
                s_foo += " | [ ";

                foreach (int i_foo in IntegerArray)
                {
                    s_foo += i_foo + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            if (LongList != null && LongList.Count > 0)
            {
                s_foo += " | [ ";

                for (int i = 0; i < LongList.Count; i++)
                {
                    s_foo += LongList[i] + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            if (String != null && String.Length == 0)
            {
                s_foo += " | null";
            }
            else
            {
                s_foo += " | " + String;
            }

            if (LocalTime != null)
            {
                s_foo += " | " + LocalTime?.ToString("HH:mm:ss");
            }

            if (LocalDateArray != null && LocalDateArray.Length > 0)
            {
                s_foo += " | [ ";

                foreach (DateTime? o_foo in LocalDateArray)
                {
                    s_foo += o_foo?.ToString("yyyy-MM-dd") + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            if (LocalDateTimeList != null && LocalDateTimeList.Count > 0)
            {
                s_foo += " | [ ";

                for (int i = 0; i < LocalDateTimeList.Count; i++)
                {
                    s_foo += LocalDateTimeList[i]?.ToString("yyyy-MM-dd'T'HH:mm:ss") + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            if (DecimalArray != null && DecimalArray.Length > 0)
            {
                s_foo += " | [ ";

                foreach (decimal o_foo in DecimalArray)
                {
                    s_foo += o_foo + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            return s_foo;
        }
    }
}
