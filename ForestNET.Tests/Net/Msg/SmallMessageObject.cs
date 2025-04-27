namespace ForestNET.Tests.Net.Msg
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
            this.LongList = [];
            this.LocalDateTimeList = [];
        }

        public void InitAll()
        {
            this.Bool = true;
            this.Char = (char)242;
            this.IntegerArray = new int[] { 1, 3, 5, 536870954, -42, 0 };
            this.LongList = [1L, 3L, 5L, 1170936177994235946L, -42L, 0L, null];
            this.String = "Hello World!";
            this.LocalTime = new(1970, 1, 1, 6, 2, 3);
            this.LocalDateArray = new DateTime?[] { new(2020, 3, 4, 0, 0, 0), new(2020, 6, 8, 0, 0, 0), new(2020, 12, 16, 0, 0, 0), null };
            this.LocalDateTimeList = [new(2020, 3, 4, 6, 2, 3), new(2020, 6, 8, 9, 24, 16), new(2020, 12, 16, 12, 48, 53), null];
            this.DecimalArray = new decimal[] { +578875020153.73804901109397m, -36.151686185423327m, +71740124.12171120119m, -2043204985254.1196m, 0m, +601.9924m };
        }

        public void EmptyAll()
        {
            this.Bool = false;
            this.Char = (char)0;
            this.IntegerArray = null;
            this.LongList = [];
            this.String = null;
            this.LocalTime = null;
            this.LocalDateArray = null;
            this.LocalDateTimeList = [];
            this.DecimalArray = null;
        }

        override public string ToString()
        {
            string s_foo = "";

            s_foo += this.Bool;

            s_foo += " | " + this.Char;

            if ((this.IntegerArray != null) && (this.IntegerArray.Length > 0))
            {
                s_foo += " | [ ";

                foreach (int i_foo in this.IntegerArray)
                {
                    s_foo += i_foo + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            if ((this.LongList != null) && (this.LongList.Count > 0))
            {
                s_foo += " | [ ";

                for (int i = 0; i < this.LongList.Count; i++)
                {
                    s_foo += this.LongList[i] + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            if ((this.String != null) && (this.String.Length == 0))
            {
                s_foo += " | null";
            }
            else
            {
                s_foo += " | " + this.String;
            }

            if (this.LocalTime != null)
            {
                s_foo += " | " + this.LocalTime?.ToString("HH:mm:ss");
            }

            if ((this.LocalDateArray != null) && (this.LocalDateArray.Length > 0))
            {
                s_foo += " | [ ";

                foreach (DateTime? o_foo in this.LocalDateArray)
                {
                    s_foo += o_foo?.ToString("yyyy-MM-dd") + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            if ((this.LocalDateTimeList != null) && (this.LocalDateTimeList.Count > 0))
            {
                s_foo += " | [ ";

                for (int i = 0; i < this.LocalDateTimeList.Count; i++)
                {
                    s_foo += this.LocalDateTimeList[i]?.ToString("yyyy-MM-dd'T'HH:mm:ss") + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            if ((this.DecimalArray != null) && (this.DecimalArray.Length > 0))
            {
                s_foo += " | [ ";

                foreach (decimal o_foo in this.DecimalArray)
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