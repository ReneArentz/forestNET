namespace Sandbox.Tests.Net.Msg
{
    public class MessageObject
    {

        /* Fields */

        /* Properties */

        public bool Bool { get; set; }
        public bool[]? BoolArray { get; set; }
        public List<bool?> BoolList { get; set; }

        public byte Byte { get; set; }
        public byte[]? ByteArray { get; set; }
        public List<byte?> ByteList { get; set; }

        public sbyte SignedByte { get; set; }
        public sbyte[]? SignedByteArray { get; set; }
        public List<sbyte?> SignedByteList { get; set; }

        public char Char { get; set; }
        public char[]? CharArray { get; set; }
        public List<char?> CharList { get; set; }

        public float Float { get; set; }
        public float[]? FloatArray { get; set; }
        public List<float?> FloatList { get; set; }

        public double Double { get; set; }
        public double[]? DoubleArray { get; set; }
        public List<double?> DoubleList { get; set; }

        public short Short { get; set; }
        public short[]? ShortArray { get; set; }
        public List<short?> ShortList { get; set; }

        public ushort UnsignedShort { get; set; }
        public ushort[]? UnsignedShortArray { get; set; }
        public List<ushort?> UnsignedShortList { get; set; }

        public int Integer { get; set; }
        public int[]? IntegerArray { get; set; }
        public List<int?> IntegerList { get; set; }

        public uint UnsignedInteger { get; set; }
        public uint[]? UnsignedIntegerArray { get; set; }
        public List<uint?> UnsignedIntegerList { get; set; }

        public long Long { get; set; }
        public long[]? LongArray { get; set; }
        public List<long?> LongList { get; set; }

        public ulong UnsignedLong { get; set; }
        public ulong[]? UnsignedLongArray { get; set; }
        public List<ulong?> UnsignedLongList { get; set; }

        public string? String { get; set; }
        public string?[]? StringArray { get; set; }
        public List<string?> StringList { get; set; }

        public DateTime? Time { get; set; }
        public DateTime?[]? TimeArray { get; set; }
        public List<DateTime?> TimeList { get; set; }

        public DateTime? Date { get; set; }
        public DateTime?[]? DateArray { get; set; }
        public List<DateTime?> DateList { get; set; }

        public DateTime? DateTime { get; set; }
        public DateTime?[]? DateTimeArray { get; set; }
        public List<DateTime?> DateTimeList { get; set; }

        public DateTime? LocalTime { get; set; }
        public DateTime?[]? LocalTimeArray { get; set; }
        public List<DateTime?> LocalTimeList { get; set; }

        public DateTime? LocalDate { get; set; }
        public DateTime?[]? LocalDateArray { get; set; }
        public List<DateTime?> LocalDateList { get; set; }

        public DateTime? LocalDateTime { get; set; }
        public DateTime?[]? LocalDateTimeArray { get; set; }
        public List<DateTime?> LocalDateTimeList { get; set; }

        public decimal Decimal { get; set; }
        public decimal[]? DecimalArray { get; set; }
        public List<decimal?> DecimalList { get; set; }

        /* Methods */

        public MessageObject()
        {
            BoolList = [];
            ByteList = [];
            SignedByteList = [];
            CharList = [];
            FloatList = [];
            DoubleList = [];
            ShortList = [];
            UnsignedShortList = [];
            IntegerList = [];
            UnsignedIntegerList = [];
            LongList = [];
            UnsignedLongList = [];
            StringList = [];
            TimeList = [];
            DateList = [];
            DateTimeList = [];
            LocalTimeList = [];
            LocalDateList = [];
            LocalDateTimeList = [];
            DecimalList = [];
        }

        public void InitAll()
        {
            Bool = true;
            BoolArray = new bool[] { true, false, true, false, true };
            BoolList = [true, false, true, false, true, null];

            Byte = 42;
            ByteArray = new byte[] { 1, 3, 5, 133, 42, 0, 102 };
            ByteList = [1, 3, 5, 133, 42, 0, null, 102];

            SignedByte = 42;
            SignedByteArray = new sbyte[] { 1, 3, 5, -10, 42, 0, -102 };
            SignedByteList = [1, 3, 5, -10, 42, 0, null, -102];

            Char = (char)242;
            CharArray = new char[] { (char)65, (char)70, (char)75, (char)133, (char)85, (char)0, (char)243 };
            CharList = [(char)65, (char)70, (char)75, (char)133, (char)85, (char)0, null, (char)243];

            Float = 42.25f;
            FloatArray = new float[] { 1.25f, 3.5f, 5.75f, 10.1010f, -41.998f, 0.0f, 4984654.5498795465f };
            FloatList = [1.25f, 3.5f, 5.75f, 10.1010f, -41.998f, 0.0f, null, 4984654.5498795465f];

            Double = 42.75d;
            DoubleArray = new double[] { 1.25d, 3.5d, 5.75d, 10.1010d, -41.998d, 0.0d, 8798546.2154656d };
            DoubleList = [1.25d, 3.5d, 5.75d, 10.1010d, -41.998d, 0.0d, null, 8798546.2154656d];

            Short = 16426;
            ShortArray = new short[] { 1, 3, 5, 16426, -42, 0 };
            ShortList = [1, 3, 5, 10, -42, 0, null];

            UnsignedShort = 16426;
            UnsignedShortArray = new ushort[] { 1, 3, 5, 16426, 42, 0 };
            UnsignedShortList = [1, 3, 5, 16426, 42, 0, null];

            Integer = 536870954;
            IntegerArray = new int[] { 1, 3, 5, 536870954, -42, 0 };
            IntegerList = [1, 3, 5, 536870954, -42, 0, null];

            UnsignedInteger = 536870954;
            UnsignedIntegerArray = new uint[] { 1, 3, 5, 536870954, 42, 0 };
            UnsignedIntegerList = [1, 3, 5, 536870954, 42, 0, null];

            Long = 1170936177994235946L;
            LongArray = new long[] { 1L, 3L, 5L, 1170936177994235946L, -42L, 0L };
            LongList = [1L, 3L, 5L, 1170936177994235946L, -42L, 0L, null];

            UnsignedLong = 1170936177994235946L;
            UnsignedLongArray = new ulong[] { 1L, 3L, 5L, 1170936177994235946L, 42L, 0L };
            UnsignedLongList = [1L, 3L, 5L, 1170936177994235946L, 42L, 0L, null];

            String = "Hello World!";
            StringArray = new string?[] { "Hello World 1!", "Hello World 2!", "Hello World 3!", "Hello World 4!", "Hello World 5!", "", null };
            StringList = ["Hello World 1!", "Hello World 2!", "Hello World 3!", "Hello World 4!", "Hello World 5!", "", null];

            Time = new(1970, 1, 1, 6, 2, 3);
            TimeArray = new DateTime?[] {
                new(1970, 1, 1, 6, 2, 3),
                new(1970, 1, 1, 9, 24, 16),
                new(1970, 1, 1, 12, 48, 53),
                null
            };
            TimeList = [
                new(1970, 1, 1, 6, 2, 3),
                new(1970, 1, 1, 9, 24, 16),
                new(1970, 1, 1, 12, 48, 53),
                null
            ];

            Date = new(2020, 3, 4, 0, 0, 0);
            DateArray = new DateTime?[] {
                new(2020, 3, 4, 0, 0, 0),
                new(2020, 6, 8, 0, 0, 0),
                new(2020, 12, 16, 0, 0, 0),
                null
            };
            DateList = [
                new(2020, 3, 4, 0, 0, 0),
                new(2020, 6, 8, 0, 0, 0),
                new(2020, 12, 16, 0, 0, 0),
                null
            ];

            DateTime = new(2020, 3, 4, 6, 2, 3);
            DateTimeArray = new DateTime?[] {
                new(2020, 3, 4, 6, 2, 3),
                new(2020, 6, 8, 9, 24, 16),
                new(2020, 12, 16, 12, 48, 53),
                null
            };
            DateTimeList = [
                new(2020, 3, 4, 6, 2, 3),
                new(2020, 6, 8, 9, 24, 16),
                new(2020, 12, 16, 12, 48, 53),
                null
            ];

            LocalTime = new(1970, 1, 1, 6, 2, 3);
            LocalTimeArray = new DateTime?[] { new(1970, 1, 1, 6, 2, 3), new(1970, 1, 1, 9, 24, 16), new(1970, 1, 1, 12, 48, 53), null };
            LocalTimeList = [new(1970, 1, 1, 6, 2, 3), new(1970, 1, 1, 9, 24, 16), new(1970, 1, 1, 12, 48, 53), null];

            LocalDate = new(2020, 3, 4, 0, 0, 0);
            LocalDateArray = new DateTime?[] { new(2020, 3, 4, 0, 0, 0), new(2020, 6, 8, 0, 0, 0), new(2020, 12, 16, 0, 0, 0), null };
            LocalDateList = [new(2020, 3, 4, 0, 0, 0), new(2020, 6, 8, 0, 0, 0), new(2020, 12, 16, 0, 0, 0), null];

            LocalDateTime = new(2020, 3, 4, 6, 2, 3);
            LocalDateTimeArray = new DateTime?[] { new(2020, 3, 4, 6, 2, 3), new(2020, 6, 8, 9, 24, 16), new(2020, 12, 16, 12, 48, 53), null };
            LocalDateTimeList = [new(2020, 3, 4, 6, 2, 3), new(2020, 6, 8, 9, 24, 16), new(2020, 12, 16, 12, 48, 53), null];

            Decimal = -268435477.6710886925m;
            DecimalArray = new decimal[] { +578875020153.73804901109397m, -36.151686185423327m, +71740124.12171120119m, -2043204985254.1196m, 0m, +601.9924m };
            DecimalList = [+578875020153.73804901109397m, -36.151686185423327m, +71740124.12171120119m, -2043204985254.1196m, 0m, +601.9924m];
        }

        public void EmptyAll()
        {
            Bool = false;
            BoolArray = null;
            BoolList = [];

            Byte = 0;
            ByteArray = null;
            ByteList = [];

            SignedByte = 0;
            SignedByteArray = null;
            SignedByteList = [];

            Char = (char)0;
            CharArray = null;
            CharList = [];

            Float = 0f;
            FloatArray = null;
            FloatList = [];

            Double = 0d;
            DoubleArray = null;
            DoubleList = [];

            Short = 0;
            ShortArray = null;
            ShortList = [];

            UnsignedShort = 0;
            UnsignedShortArray = null;
            UnsignedShortList = [];

            Integer = 0;
            IntegerArray = null;
            IntegerList = [];

            UnsignedInteger = 0;
            UnsignedIntegerArray = null;
            UnsignedIntegerList = [];

            Long = 0L;
            LongArray = null;
            LongList = [];

            UnsignedLong = 0L;
            UnsignedLongArray = null;
            UnsignedLongList = [];

            String = null;
            StringArray = null;
            StringList = [];

            Time = null;
            TimeArray = null;
            TimeList = [];

            Date = null;
            DateArray = null;
            DateList = [];

            DateTime = null;
            DateTimeArray = null;
            DateTimeList = [];

            LocalTime = null;
            LocalTimeArray = null;
            LocalTimeList = [];

            LocalDate = null;
            LocalDateArray = null;
            LocalDateList = [];

            LocalDateTime = null;
            LocalDateTimeArray = null;
            LocalDateTimeList = [];

            Decimal = 0.0m;
            DecimalArray = null;
            DecimalList = [];
        }

        override public string ToString()
        {
            string s_foo = "";

            s_foo += Bool;

            if (BoolArray != null && BoolArray.Length > 0)
            {
                s_foo += " | [ ";

                foreach (bool b_foo in BoolArray)
                {
                    s_foo += b_foo + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            if (BoolList != null && BoolList.Count > 0)
            {
                s_foo += " | [ ";

                for (int i = 0; i < BoolList.Count; i++)
                {
                    s_foo += BoolList[i] + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }



            s_foo += " | " + Byte;

            if (ByteArray != null && ByteArray.Length > 0)
            {
                s_foo += " | [ ";

                foreach (byte by_foo in ByteArray)
                {
                    s_foo += by_foo + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            if (ByteList != null && ByteList.Count > 0)
            {
                s_foo += " | [ ";

                for (int i = 0; i < ByteList.Count; i++)
                {
                    s_foo += ByteList[i] + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }



            s_foo += " | " + SignedByte;

            if (SignedByteArray != null && SignedByteArray.Length > 0)
            {
                s_foo += " | [ ";

                foreach (sbyte sby_foo in SignedByteArray)
                {
                    s_foo += sby_foo + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            if (SignedByteList != null && SignedByteList.Count > 0)
            {
                s_foo += " | [ ";

                for (int i = 0; i < SignedByteList.Count; i++)
                {
                    s_foo += SignedByteList[i] + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }



            s_foo += " | " + Char;

            if (CharArray != null && CharArray.Length > 0)
            {
                s_foo += " | [ ";

                foreach (char c_foo in CharArray)
                {
                    s_foo += c_foo + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            if (CharList != null && CharList.Count > 0)
            {
                s_foo += " | [ ";

                for (int i = 0; i < CharList.Count; i++)
                {
                    s_foo += CharList[i] + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }



            s_foo += " | " + Float;

            if (FloatArray != null && FloatArray.Length > 0)
            {
                s_foo += " | [ ";

                foreach (float f_foo in FloatArray)
                {
                    s_foo += f_foo + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            if (FloatList != null && FloatList.Count > 0)
            {
                s_foo += " | [ ";

                for (int i = 0; i < FloatList.Count; i++)
                {
                    s_foo += FloatList[i] + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }



            s_foo += " | " + Double;

            if (DoubleArray != null && DoubleArray.Length > 0)
            {
                s_foo += " | [ ";

                foreach (double d_foo in DoubleArray)
                {
                    s_foo += d_foo + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            if (DoubleList != null && DoubleList.Count > 0)
            {
                s_foo += " | [ ";

                for (int i = 0; i < DoubleList.Count; i++)
                {
                    s_foo += DoubleList[i] + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }



            s_foo += " | " + Short;

            if (ShortArray != null && ShortArray.Length > 0)
            {
                s_foo += " | [ ";

                foreach (short sh_foo in ShortArray)
                {
                    s_foo += sh_foo + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            if (ShortList != null && ShortList.Count > 0)
            {
                s_foo += " | [ ";

                for (int i = 0; i < ShortList.Count; i++)
                {
                    s_foo += ShortList[i] + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }



            s_foo += " | " + UnsignedShort;

            if (UnsignedShortArray != null && UnsignedShortArray.Length > 0)
            {
                s_foo += " | [ ";

                foreach (ushort ush_foo in UnsignedShortArray)
                {
                    s_foo += ush_foo + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            if (UnsignedShortList != null && UnsignedShortList.Count > 0)
            {
                s_foo += " | [ ";

                for (int i = 0; i < UnsignedShortList.Count; i++)
                {
                    s_foo += UnsignedShortList[i] + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }



            s_foo += " | " + Integer;

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

            if (IntegerList != null && IntegerList.Count > 0)
            {
                s_foo += " | [ ";

                for (int i = 0; i < IntegerList.Count; i++)
                {
                    s_foo += IntegerList[i] + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }



            s_foo += " | " + UnsignedInteger;

            if (UnsignedIntegerArray != null && UnsignedIntegerArray.Length > 0)
            {
                s_foo += " | [ ";

                foreach (uint ui_foo in UnsignedIntegerArray)
                {
                    s_foo += ui_foo + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            if (UnsignedIntegerList != null && UnsignedIntegerList.Count > 0)
            {
                s_foo += " | [ ";

                for (int i = 0; i < UnsignedIntegerList.Count; i++)
                {
                    s_foo += UnsignedIntegerList[i] + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }



            s_foo += " | " + Long;

            if (LongArray != null && LongArray.Length > 0)
            {
                s_foo += " | [ ";

                foreach (long l_foo in LongArray)
                {
                    s_foo += l_foo + ", ";
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



            s_foo += " | " + UnsignedLong;

            if (UnsignedLongArray != null && UnsignedLongArray.Length > 0)
            {
                s_foo += " | [ ";

                foreach (ulong ul_foo in UnsignedLongArray)
                {
                    s_foo += ul_foo + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            if (UnsignedLongList != null && UnsignedLongList.Count > 0)
            {
                s_foo += " | [ ";

                for (int i = 0; i < UnsignedLongList.Count; i++)
                {
                    s_foo += UnsignedLongList[i] + ", ";
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

            if (StringArray != null && StringArray.Length > 0)
            {
                s_foo += " | [ ";

                foreach (string? s_foo2 in StringArray)
                {
                    if (s_foo2 != null && s_foo2.Length == 0)
                    {
                        s_foo += "null, ";
                    }
                    else
                    {
                        s_foo += s_foo2 + ", ";
                    }
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            if (StringList != null && StringList.Count > 0)
            {
                s_foo += " | [ ";

                for (int i = 0; i < StringList.Count; i++)
                {
                    if (StringList[i] != null && StringList[i]?.Length == 0)
                    {
                        s_foo += "null, ";
                    }
                    else
                    {
                        s_foo += StringList[i] + ", ";
                    }
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }



            if (Time != null)
            {
                s_foo += " | " + Time?.ToString("HH:mm:ss");
            }

            if (TimeArray != null && TimeArray.Length > 0)
            {
                s_foo += " | [ ";

                foreach (DateTime? o_foo in TimeArray)
                {
                    if (o_foo == null)
                    {
                        s_foo += "null, ";
                    }
                    else
                    {
                        s_foo += o_foo?.ToString("HH:mm:ss") + ", ";
                    }
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            if (TimeList != null && TimeList.Count > 0)
            {
                s_foo += " | [ ";

                for (int i = 0; i < TimeList.Count; i++)
                {
                    if (TimeList[i] == null)
                    {
                        s_foo += "null, ";
                    }
                    else
                    {
                        s_foo += TimeList[i]?.ToString("HH:mm:ss") + ", ";
                    }
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }



            if (Date != null)
            {
                s_foo += " | " + Date?.ToString("yyyy-MM-dd");
            }

            if (DateArray != null && DateArray.Length > 0)
            {
                s_foo += " | [ ";

                foreach (DateTime? o_foo in DateArray)
                {
                    if (o_foo == null)
                    {
                        s_foo += "null, ";
                    }
                    else
                    {
                        s_foo += o_foo?.ToString("yyyy-MM-dd") + ", ";
                    }
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            if (DateList != null && DateList.Count > 0)
            {
                s_foo += " | [ ";

                for (int i = 0; i < DateList.Count; i++)
                {
                    if (DateList[i] == null)
                    {
                        s_foo += "null, ";
                    }
                    else
                    {
                        s_foo += DateList[i]?.ToString("yyyy-MM-dd") + ", ";
                    }
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }



            if (DateTime != null)
            {
                s_foo += " | " + DateTime?.ToString("yyyy-MM-dd'T'HH:mm:ss");
            }

            if (DateTimeArray != null && DateTimeArray.Length > 0)
            {
                s_foo += " | [ ";

                foreach (DateTime? o_foo in DateTimeArray)
                {
                    if (o_foo == null)
                    {
                        s_foo += "null, ";
                    }
                    else
                    {
                        s_foo += o_foo?.ToString("yyyy-MM-dd'T'HH:mm:ss") + ", ";
                    }
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            if (DateTimeList != null && DateTimeList.Count > 0)
            {
                s_foo += " | [ ";

                for (int i = 0; i < DateTimeList.Count; i++)
                {
                    if (DateTimeList[i] == null)
                    {
                        s_foo += "null, ";
                    }
                    else
                    {
                        s_foo += DateTimeList[i]?.ToString("yyyy-MM-dd'T'HH:mm:ss") + ", ";
                    }
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }



            if (LocalTime != null)
            {
                s_foo += " | " + LocalTime?.ToString("HH:mm:ss");
            }

            if (LocalTimeArray != null && LocalTimeArray.Length > 0)
            {
                s_foo += " | [ ";

                foreach (DateTime? o_foo in LocalTimeArray)
                {
                    s_foo += o_foo?.ToString("HH:mm:ss") + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            if (LocalTimeList != null && LocalTimeList.Count > 0)
            {
                s_foo += " | [ ";

                for (int i = 0; i < LocalTimeList.Count; i++)
                {
                    s_foo += LocalTimeList[i]?.ToString("HH:mm:ss") + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }



            if (LocalDate != null)
            {
                s_foo += " | " + LocalDate?.ToString("yyyy-MM-dd");
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

            if (LocalDateList != null && LocalDateList.Count > 0)
            {
                s_foo += " | [ ";

                for (int i = 0; i < LocalDateList.Count; i++)
                {
                    s_foo += LocalDateList[i]?.ToString("yyyy-MM-dd") + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }



            if (LocalDateTime != null)
            {
                s_foo += " | " + LocalDateTime?.ToString("yyyy-MM-dd'T'HH:mm:ss");
            }

            if (LocalDateTimeArray != null && LocalDateTimeArray.Length > 0)
            {
                s_foo += " | [ ";

                foreach (DateTime? o_foo in LocalDateTimeArray)
                {
                    s_foo += o_foo?.ToString("yyyy-MM-dd'T'HH:mm:ss") + ", ";
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



            s_foo += " | " + Decimal;

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

            if (DecimalList != null && DecimalList.Count > 0)
            {
                s_foo += " | [ ";

                for (int i = 0; i < DecimalList.Count; i++)
                {
                    s_foo += (DecimalList[i] ?? 0.0m) + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            return s_foo;
        }
    }
}
