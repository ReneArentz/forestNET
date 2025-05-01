namespace Sandbox.Tests.Net
{
    public class SharedMemoryMessageObject : ForestNET.Lib.Net.Sock.Com.SharedMemory<SharedMemoryMessageObject>
    {

        /* Fields */

        private bool _Bool;
        private bool[]? _BoolArray;
        private List<bool?> _BoolList;

        private byte _Byte;
        private byte[]? _ByteArray;
        private List<byte?> _ByteList;

        private sbyte _SignedByte;
        private sbyte[]? _SignedByteArray;
        private List<sbyte?> _SignedByteList;

        private char _Char;
        private char[]? _CharArray;
        private List<char?> _CharList;

        private float _Float;
        private float[]? _FloatArray;
        private List<float?> _FloatList;

        private double _Double;
        private double[]? _DoubleArray;
        private List<double?> _DoubleList;

        private short _Short;
        private short[]? _ShortArray;
        private List<short?> _ShortList;

        private ushort _UnsignedShort;
        private ushort[]? _UnsignedShortArray;
        private List<ushort?> _UnsignedShortList;

        private int _Integer;
        private int[]? _IntegerArray;
        private List<int?> _IntegerList;

        private uint _UnsignedInteger;
        private uint[]? _UnsignedIntegerArray;
        private List<uint?> _UnsignedIntegerList;

        private long _Long;
        private long[]? _LongArray;
        private List<long?> _LongList;

        private ulong _UnsignedLong;
        private ulong[]? _UnsignedLongArray;
        private List<ulong?> _UnsignedLongList;

        private string? _String;
        private string?[]? _StringArray;
        private List<string?> _StringList;

        private DateTime? _Time;
        private DateTime?[]? _TimeArray;
        private List<DateTime?> _TimeList;

        private DateTime? _Date;
        private DateTime?[]? _DateArray;
        private List<DateTime?> _DateList;

        private DateTime? _DateTime;
        private DateTime?[]? _DateTimeArray;
        private List<DateTime?> _DateTimeList;

        private DateTime? _LocalTime;
        private DateTime?[]? _LocalTimeArray;
        private List<DateTime?> _LocalTimeList;

        private DateTime? _LocalDate;
        private DateTime?[]? _LocalDateArray;
        private List<DateTime?> _LocalDateList;

        private DateTime? _LocalDateTime;
        private DateTime?[]? _LocalDateTimeArray;
        private List<DateTime?> _LocalDateTimeList;

        private decimal _Decimal;
        private decimal[]? _DecimalArray;
        private List<decimal?> _DecimalList;

        /* Properties */

        public bool Bool { get { return _Bool; } set { _Bool = value; } }
        public bool[]? BoolArray { get { return _BoolArray; } set { _BoolArray = value; } }
        public List<bool?> BoolList { get { return _BoolList; } set { _BoolList = value; } }

        public byte Byte { get { return _Byte; } set { _Byte = value; } }
        public byte[]? ByteArray { get { return _ByteArray; } set { _ByteArray = value; } }
        public List<byte?> ByteList { get { return _ByteList; } set { _ByteList = value; } }

        public sbyte SignedByte { get { return _SignedByte; } set { _SignedByte = value; } }
        public sbyte[]? SignedByteArray { get { return _SignedByteArray; } set { _SignedByteArray = value; } }
        public List<sbyte?> SignedByteList { get { return _SignedByteList; } set { _SignedByteList = value; } }

        public char Char { get { return _Char; } set { _Char = value; } }
        public char[]? CharArray { get { return _CharArray; } set { _CharArray = value; } }
        public List<char?> CharList { get { return _CharList; } set { _CharList = value; } }

        public float Float { get { return _Float; } set { _Float = value; } }
        public float[]? FloatArray { get { return _FloatArray; } set { _FloatArray = value; } }
        public List<float?> FloatList { get { return _FloatList; } set { _FloatList = value; } }

        public double Double { get { return _Double; } set { _Double = value; } }
        public double[]? DoubleArray { get { return _DoubleArray; } set { _DoubleArray = value; } }
        public List<double?> DoubleList { get { return _DoubleList; } set { _DoubleList = value; } }

        public short Short { get { return _Short; } set { _Short = value; } }
        public short[]? ShortArray { get { return _ShortArray; } set { _ShortArray = value; } }
        public List<short?> ShortList { get { return _ShortList; } set { _ShortList = value; } }

        public ushort UnsignedShort { get { return _UnsignedShort; } set { _UnsignedShort = value; } }
        public ushort[]? UnsignedShortArray { get { return _UnsignedShortArray; } set { _UnsignedShortArray = value; } }
        public List<ushort?> UnsignedShortList { get { return _UnsignedShortList; } set { _UnsignedShortList = value; } }

        public int Integer { get { return _Integer; } set { _Integer = value; } }
        public int[]? IntegerArray { get { return _IntegerArray; } set { _IntegerArray = value; } }
        public List<int?> IntegerList { get { return _IntegerList; } set { _IntegerList = value; } }

        public uint UnsignedInteger { get { return _UnsignedInteger; } set { _UnsignedInteger = value; } }
        public uint[]? UnsignedIntegerArray { get { return _UnsignedIntegerArray; } set { _UnsignedIntegerArray = value; } }
        public List<uint?> UnsignedIntegerList { get { return _UnsignedIntegerList; } set { _UnsignedIntegerList = value; } }

        public long Long { get { return _Long; } set { _Long = value; } }
        public long[]? LongArray { get { return _LongArray; } set { _LongArray = value; } }
        public List<long?> LongList { get { return _LongList; } set { _LongList = value; } }

        public ulong UnsignedLong { get { return _UnsignedLong; } set { _UnsignedLong = value; } }
        public ulong[]? UnsignedLongArray { get { return _UnsignedLongArray; } set { _UnsignedLongArray = value; } }
        public List<ulong?> UnsignedLongList { get { return _UnsignedLongList; } set { _UnsignedLongList = value; } }

        public string? String { get { return _String; } set { _String = value; } }
        public string?[]? StringArray { get { return _StringArray; } set { _StringArray = value; } }
        public List<string?> StringList { get { return _StringList; } set { _StringList = value; } }

        public DateTime? Time { get { return _Time; } set { _Time = value; } }
        public DateTime?[]? TimeArray { get { return _TimeArray; } set { _TimeArray = value; } }
        public List<DateTime?> TimeList { get { return _TimeList; } set { _TimeList = value; } }

        public DateTime? Date { get { return _Date; } set { _Date = value; } }
        public DateTime?[]? DateArray { get { return _DateArray; } set { _DateArray = value; } }
        public List<DateTime?> DateList { get { return _DateList; } set { _DateList = value; } }

        public DateTime? DateTime { get { return _DateTime; } set { _DateTime = value; } }
        public DateTime?[]? DateTimeArray { get { return _DateTimeArray; } set { _DateTimeArray = value; } }
        public List<DateTime?> DateTimeList { get { return _DateTimeList; } set { _DateTimeList = value; } }

        public DateTime? LocalTime { get { return _LocalTime; } set { _LocalTime = value; } }
        public DateTime?[]? LocalTimeArray { get { return _LocalTimeArray; } set { _LocalTimeArray = value; } }
        public List<DateTime?> LocalTimeList { get { return _LocalTimeList; } set { _LocalTimeList = value; } }

        public DateTime? LocalDate { get { return _LocalDate; } set { _LocalDate = value; } }
        public DateTime?[]? LocalDateArray { get { return _LocalDateArray; } set { _LocalDateArray = value; } }
        public List<DateTime?> LocalDateList { get { return _LocalDateList; } set { _LocalDateList = value; } }

        public DateTime? LocalDateTime { get { return _LocalDateTime; } set { _LocalDateTime = value; } }
        public DateTime?[]? LocalDateTimeArray { get { return _LocalDateTimeArray; } set { _LocalDateTimeArray = value; } }
        public List<DateTime?> LocalDateTimeList { get { return _LocalDateTimeList; } set { _LocalDateTimeList = value; } }

        public decimal Decimal { get { return _Decimal; } set { _Decimal = value; } }
        public decimal[]? DecimalArray { get { return _DecimalArray; } set { _DecimalArray = value; } }
        public List<decimal?> DecimalList { get { return _DecimalList; } set { _DecimalList = value; } }

        /* Methods */

        public SharedMemoryMessageObject()
        {
            this._BoolList = [];
            this._ByteList = [];
            this._SignedByteList = [];
            this._CharList = [];
            this._FloatList = [];
            this._DoubleList = [];
            this._ShortList = [];
            this._UnsignedShortList = [];
            this._IntegerList = [];
            this._UnsignedIntegerList = [];
            this._LongList = [];
            this._UnsignedLongList = [];
            this._StringList = [];
            this._TimeList = [];
            this._DateList = [];
            this._DateTimeList = [];
            this._LocalTimeList = [];
            this._LocalDateList = [];
            this._LocalDateTimeList = [];
            this._DecimalList = [];
        }

        protected override void Init()
        {
            this.MirrorClass = typeof(SharedMemoryMessageObject);
        }

        public void InitAll()
        {
            this.Bool = true;
            this.BoolArray = new bool[] { true, false, true, false, true };
            this.BoolList = [true, false, true, false, true, null];

            this.Byte = (byte)42;
            this.ByteArray = new byte[] { 1, 3, 5, (byte)133, 42, 0, (byte)102 };
            this.ByteList = [(byte)1, (byte)3, (byte)5, (byte)133, (byte)42, (byte)0, null, (byte)102];

            this.SignedByte = (sbyte)42;
            this.SignedByteArray = new sbyte[] { 1, 3, 5, (sbyte)-10, 42, 0, (sbyte)-102 };
            this.SignedByteList = [(sbyte)1, (sbyte)3, (sbyte)5, (sbyte)-10, (sbyte)42, (sbyte)0, null, (sbyte)-102];

            this.Char = (char)242;
            this.CharArray = new char[] { (char)65, (char)70, (char)75, (char)133, (char)85, (char)0, (char)243 };
            this.CharList = [(char)65, (char)70, (char)75, (char)133, (char)85, (char)0, null, (char)243];

            this.Float = 42.25f;
            this.FloatArray = new float[] { 1.25f, 3.5f, 5.75f, 10.1010f, -41.998f, 0.0f, 4984654.5498795465f };
            this.FloatList = [1.25f, 3.5f, 5.75f, 10.1010f, -41.998f, 0.0f, null, 4984654.5498795465f];

            this.Double = 42.75d;
            this.DoubleArray = new double[] { 1.25d, 3.5d, 5.75d, 10.1010d, -41.998d, 0.0d, 8798546.2154656d };
            this.DoubleList = [1.25d, 3.5d, 5.75d, 10.1010d, -41.998d, 0.0d, null, 8798546.2154656d];

            this.Short = (short)16426;
            this.ShortArray = new short[] { 1, 3, 5, 16426, -42, 0 };
            this.ShortList = [(short)1, (short)3, (short)5, (short)10, (short)-42, (short)0, null];

            this.UnsignedShort = (ushort)16426;
            this.UnsignedShortArray = new ushort[] { 1, 3, 5, 16426, 42, 0 };
            this.UnsignedShortList = [(ushort)1, (ushort)3, (ushort)5, (ushort)16426, (ushort)42, (ushort)0, null];

            this.Integer = 536870954;
            this.IntegerArray = new int[] { 1, 3, 5, 536870954, -42, 0 };
            this.IntegerList = [1, 3, 5, 536870954, -42, 0, null];

            this.UnsignedInteger = 536870954;
            this.UnsignedIntegerArray = new uint[] { 1, 3, 5, 536870954, 42, 0 };
            this.UnsignedIntegerList = [1, 3, 5, 536870954, 42, 0, null];

            this.Long = 1170936177994235946L;
            this.LongArray = new long[] { 1L, 3L, 5L, 1170936177994235946L, -42L, 0L };
            this.LongList = [1L, 3L, 5L, 1170936177994235946L, -42L, 0L, null];

            this.UnsignedLong = 1170936177994235946L;
            this.UnsignedLongArray = new ulong[] { 1L, 3L, 5L, 1170936177994235946L, 42L, 0L };
            this.UnsignedLongList = [1L, 3L, 5L, 1170936177994235946L, 42L, 0L, null];

            this.String = "Hello World!";
            this.StringArray = new string?[] { "Hello World 1!", "Hello World 2!", "Hello World 3!", "Hello World 4!", "Hello World 5!", "", null };
            this.StringList = ["Hello World 1!", "Hello World 2!", "Hello World 3!", "Hello World 4!", "Hello World 5!", "", null];

            this.Time = new(1970, 1, 1, 6, 2, 3);
            this.TimeArray = new DateTime?[] {
                new(1970, 1, 1, 6, 2, 3),
                new(1970, 1, 1, 9, 24, 16),
                new(1970, 1, 1, 12, 48, 53),
                null
            };
            this.TimeList = [
                new(1970, 1, 1, 6, 2, 3),
                new(1970, 1, 1, 9, 24, 16),
                new(1970, 1, 1, 12, 48, 53),
                null
            ];

            this.Date = new(2020, 3, 4, 0, 0, 0);
            this.DateArray = new DateTime?[] {
                new(2020, 3, 4, 0, 0, 0),
                new(2020, 6, 8, 0, 0, 0),
                new(2020, 12, 16, 0, 0, 0),
                null
            };
            this.DateList = [
                new(2020, 3, 4, 0, 0, 0),
                new(2020, 6, 8, 0, 0, 0),
                new(2020, 12, 16, 0, 0, 0),
                null
            ];

            this.DateTime = new(2020, 3, 4, 6, 2, 3);
            this.DateTimeArray = new DateTime?[] {
                new(2020, 3, 4, 6, 2, 3),
                new(2020, 6, 8, 9, 24, 16),
                new(2020, 12, 16, 12, 48, 53),
                null
            };
            this.DateTimeList = [
                new(2020, 3, 4, 6, 2, 3),
                new(2020, 6, 8, 9, 24, 16),
                new(2020, 12, 16, 12, 48, 53),
                null
            ];

            this.LocalTime = new(1970, 1, 1, 6, 2, 3);
            this.LocalTimeArray = new DateTime?[] { new(1970, 1, 1, 6, 2, 3), new(1970, 1, 1, 9, 24, 16), new(1970, 1, 1, 12, 48, 53), null };
            this.LocalTimeList = [new(1970, 1, 1, 6, 2, 3), new(1970, 1, 1, 9, 24, 16), new(1970, 1, 1, 12, 48, 53), null];

            this.LocalDate = new(2020, 3, 4, 0, 0, 0);
            this.LocalDateArray = new DateTime?[] { new(2020, 3, 4, 0, 0, 0), new(2020, 6, 8, 0, 0, 0), new(2020, 12, 16, 0, 0, 0), null };
            this.LocalDateList = [new(2020, 3, 4, 0, 0, 0), new(2020, 6, 8, 0, 0, 0), new(2020, 12, 16, 0, 0, 0), null];

            this.LocalDateTime = new(2020, 3, 4, 6, 2, 3);
            this.LocalDateTimeArray = new DateTime?[] { new(2020, 3, 4, 6, 2, 3), new(2020, 6, 8, 9, 24, 16), new(2020, 12, 16, 12, 48, 53), null };
            this.LocalDateTimeList = [new(2020, 3, 4, 6, 2, 3), new(2020, 6, 8, 9, 24, 16), new(2020, 12, 16, 12, 48, 53), null];

            this.Decimal = -268435477.6710886925m;
            this.DecimalArray = new decimal[] { +578875020153.73804901109397m, -36.151686185423327m, +71740124.12171120119m, -2043204985254.1196m, 0m, +601.9924m };
            this.DecimalList = [+578875020153.73804901109397m, -36.151686185423327m, +71740124.12171120119m, -2043204985254.1196m, 0m, +601.9924m];
        }

        public void EmptyAll()
        {
            this.Bool = false;
            this.BoolArray = null;
            this.BoolList = [];

            this.Byte = (byte)0;
            this.ByteArray = null;
            this.ByteList = [];

            this.SignedByte = (sbyte)0;
            this.SignedByteArray = null;
            this.SignedByteList = [];

            this.Char = (char)0;
            this.CharArray = null;
            this.CharList = [];

            this.Float = 0f;
            this.FloatArray = null;
            this.FloatList = [];

            this.Double = 0d;
            this.DoubleArray = null;
            this.DoubleList = [];

            this.Short = (short)0;
            this.ShortArray = null;
            this.ShortList = [];

            this.UnsignedShort = (ushort)0;
            this.UnsignedShortArray = null;
            this.UnsignedShortList = [];

            this.Integer = 0;
            this.IntegerArray = null;
            this.IntegerList = [];

            this.UnsignedInteger = (uint)0;
            this.UnsignedIntegerArray = null;
            this.UnsignedIntegerList = [];

            this.Long = 0L;
            this.LongArray = null;
            this.LongList = [];

            this.UnsignedLong = (ulong)0L;
            this.UnsignedLongArray = null;
            this.UnsignedLongList = [];

            this.String = null;
            this.StringArray = null;
            this.StringList = [];

            this.Time = null;
            this.TimeArray = null;
            this.TimeList = [];

            this.Date = null;
            this.DateArray = null;
            this.DateList = [];

            this.DateTime = null;
            this.DateTimeArray = null;
            this.DateTimeList = [];

            this.LocalTime = null;
            this.LocalTimeArray = null;
            this.LocalTimeList = [];

            this.LocalDate = null;
            this.LocalDateArray = null;
            this.LocalDateList = [];

            this.LocalDateTime = null;
            this.LocalDateTimeArray = null;
            this.LocalDateTimeList = [];

            this.Decimal = 0.0m;
            this.DecimalArray = null;
            this.DecimalList = [];
        }

        override public string ToString()
        {
            string s_foo = "";

            s_foo += this.Bool;

            if ((this.BoolArray != null) && (this.BoolArray.Length > 0))
            {
                s_foo += " | [ ";

                foreach (bool b_foo in this.BoolArray)
                {
                    s_foo += b_foo + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            if ((this.BoolList != null) && (this.BoolList.Count > 0))
            {
                s_foo += " | [ ";

                for (int i = 0; i < this.BoolList.Count; i++)
                {
                    s_foo += this.BoolList[i] + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }



            s_foo += " | " + this.Byte;

            if ((this.ByteArray != null) && (this.ByteArray.Length > 0))
            {
                s_foo += " | [ ";

                foreach (byte by_foo in this.ByteArray)
                {
                    s_foo += by_foo + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            if ((this.ByteList != null) && (this.ByteList.Count > 0))
            {
                s_foo += " | [ ";

                for (int i = 0; i < this.ByteList.Count; i++)
                {
                    s_foo += this.ByteList[i] + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }



            s_foo += " | " + this.SignedByte;

            if ((this.SignedByteArray != null) && (this.SignedByteArray.Length > 0))
            {
                s_foo += " | [ ";

                foreach (sbyte sby_foo in this.SignedByteArray)
                {
                    s_foo += sby_foo + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            if ((this.SignedByteList != null) && (this.SignedByteList.Count > 0))
            {
                s_foo += " | [ ";

                for (int i = 0; i < this.SignedByteList.Count; i++)
                {
                    s_foo += this.SignedByteList[i] + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }



            s_foo += " | " + this.Char;

            if ((this.CharArray != null) && (this.CharArray.Length > 0))
            {
                s_foo += " | [ ";

                foreach (char c_foo in this.CharArray)
                {
                    s_foo += c_foo + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            if ((this.CharList != null) && (this.CharList.Count > 0))
            {
                s_foo += " | [ ";

                for (int i = 0; i < this.CharList.Count; i++)
                {
                    s_foo += this.CharList[i] + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }



            s_foo += " | " + this.Float;

            if ((this.FloatArray != null) && (this.FloatArray.Length > 0))
            {
                s_foo += " | [ ";

                foreach (float f_foo in this.FloatArray)
                {
                    s_foo += f_foo + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            if ((this.FloatList != null) && (this.FloatList.Count > 0))
            {
                s_foo += " | [ ";

                for (int i = 0; i < this.FloatList.Count; i++)
                {
                    s_foo += this.FloatList[i] + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }



            s_foo += " | " + this.Double;

            if ((this.DoubleArray != null) && (this.DoubleArray.Length > 0))
            {
                s_foo += " | [ ";

                foreach (double d_foo in this.DoubleArray)
                {
                    s_foo += d_foo + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            if ((this.DoubleList != null) && (this.DoubleList.Count > 0))
            {
                s_foo += " | [ ";

                for (int i = 0; i < this.DoubleList.Count; i++)
                {
                    s_foo += this.DoubleList[i] + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }



            s_foo += " | " + this.Short;

            if ((this.ShortArray != null) && (this.ShortArray.Length > 0))
            {
                s_foo += " | [ ";

                foreach (short sh_foo in this.ShortArray)
                {
                    s_foo += sh_foo + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            if ((this.ShortList != null) && (this.ShortList.Count > 0))
            {
                s_foo += " | [ ";

                for (int i = 0; i < this.ShortList.Count; i++)
                {
                    s_foo += this.ShortList[i] + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }



            s_foo += " | " + this.UnsignedShort;

            if ((this.UnsignedShortArray != null) && (this.UnsignedShortArray.Length > 0))
            {
                s_foo += " | [ ";

                foreach (ushort ush_foo in this.UnsignedShortArray)
                {
                    s_foo += ush_foo + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            if ((this.UnsignedShortList != null) && (this.UnsignedShortList.Count > 0))
            {
                s_foo += " | [ ";

                for (int i = 0; i < this.UnsignedShortList.Count; i++)
                {
                    s_foo += this.UnsignedShortList[i] + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }



            s_foo += " | " + this.Integer;

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

            if ((this.IntegerList != null) && (this.IntegerList.Count > 0))
            {
                s_foo += " | [ ";

                for (int i = 0; i < this.IntegerList.Count; i++)
                {
                    s_foo += this.IntegerList[i] + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }



            s_foo += " | " + this.UnsignedInteger;

            if ((this.UnsignedIntegerArray != null) && (this.UnsignedIntegerArray.Length > 0))
            {
                s_foo += " | [ ";

                foreach (uint ui_foo in this.UnsignedIntegerArray)
                {
                    s_foo += ui_foo + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            if ((this.UnsignedIntegerList != null) && (this.UnsignedIntegerList.Count > 0))
            {
                s_foo += " | [ ";

                for (int i = 0; i < this.UnsignedIntegerList.Count; i++)
                {
                    s_foo += this.UnsignedIntegerList[i] + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }



            s_foo += " | " + this.Long;

            if ((this.LongArray != null) && (this.LongArray.Length > 0))
            {
                s_foo += " | [ ";

                foreach (long l_foo in this.LongArray)
                {
                    s_foo += l_foo + ", ";
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



            s_foo += " | " + this.UnsignedLong;

            if ((this.UnsignedLongArray != null) && (this.UnsignedLongArray.Length > 0))
            {
                s_foo += " | [ ";

                foreach (ulong ul_foo in this.UnsignedLongArray)
                {
                    s_foo += ul_foo + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            if ((this.UnsignedLongList != null) && (this.UnsignedLongList.Count > 0))
            {
                s_foo += " | [ ";

                for (int i = 0; i < this.UnsignedLongList.Count; i++)
                {
                    s_foo += this.UnsignedLongList[i] + ", ";
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

            if ((this.StringArray != null) && (this.StringArray.Length > 0))
            {
                s_foo += " | [ ";

                foreach (string? s_foo2 in this.StringArray)
                {
                    if ((s_foo2 != null) && (s_foo2.Length == 0))
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

            if ((this.StringList != null) && (this.StringList.Count > 0))
            {
                s_foo += " | [ ";

                for (int i = 0; i < this.StringList.Count; i++)
                {
                    if ((this.StringList[i] != null) && (this.StringList[i]?.Length == 0))
                    {
                        s_foo += "null, ";
                    }
                    else
                    {
                        s_foo += this.StringList[i] + ", ";
                    }
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }



            if (this.Time != null)
            {
                s_foo += " | " + this.Time?.ToString("HH:mm:ss");
            }

            if ((this.TimeArray != null) && (this.TimeArray.Length > 0))
            {
                s_foo += " | [ ";

                foreach (DateTime? o_foo in this.TimeArray)
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

            if ((this.TimeList != null) && (this.TimeList.Count > 0))
            {
                s_foo += " | [ ";

                for (int i = 0; i < this.TimeList.Count; i++)
                {
                    if (this.TimeList[i] == null)
                    {
                        s_foo += "null, ";
                    }
                    else
                    {
                        s_foo += this.TimeList[i]?.ToString("HH:mm:ss") + ", ";
                    }
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }



            if (this.Date != null)
            {
                s_foo += " | " + this.Date?.ToString("yyyy-MM-dd");
            }

            if ((this.DateArray != null) && (this.DateArray.Length > 0))
            {
                s_foo += " | [ ";

                foreach (DateTime? o_foo in this.DateArray)
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

            if ((this.DateList != null) && (this.DateList.Count > 0))
            {
                s_foo += " | [ ";

                for (int i = 0; i < this.DateList.Count; i++)
                {
                    if (this.DateList[i] == null)
                    {
                        s_foo += "null, ";
                    }
                    else
                    {
                        s_foo += this.DateList[i]?.ToString("yyyy-MM-dd") + ", ";
                    }
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }



            if (this.DateTime != null)
            {
                s_foo += " | " + this.DateTime?.ToString("yyyy-MM-dd'T'HH:mm:ss");
            }

            if ((this.DateTimeArray != null) && (this.DateTimeArray.Length > 0))
            {
                s_foo += " | [ ";

                foreach (DateTime? o_foo in this.DateTimeArray)
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

            if ((this.DateTimeList != null) && (this.DateTimeList.Count > 0))
            {
                s_foo += " | [ ";

                for (int i = 0; i < this.DateTimeList.Count; i++)
                {
                    if (this.DateTimeList[i] == null)
                    {
                        s_foo += "null, ";
                    }
                    else
                    {
                        s_foo += this.DateTimeList[i]?.ToString("yyyy-MM-dd'T'HH:mm:ss") + ", ";
                    }
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }



            if (this.LocalTime != null)
            {
                s_foo += " | " + this.LocalTime?.ToString("HH:mm:ss");
            }

            if ((this.LocalTimeArray != null) && (this.LocalTimeArray.Length > 0))
            {
                s_foo += " | [ ";

                foreach (DateTime? o_foo in this.LocalTimeArray)
                {
                    s_foo += o_foo?.ToString("HH:mm:ss") + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            if ((this.LocalTimeList != null) && (this.LocalTimeList.Count > 0))
            {
                s_foo += " | [ ";

                for (int i = 0; i < this.LocalTimeList.Count; i++)
                {
                    s_foo += this.LocalTimeList[i]?.ToString("HH:mm:ss") + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }



            if (this.LocalDate != null)
            {
                s_foo += " | " + this.LocalDate?.ToString("yyyy-MM-dd");
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

            if ((this.LocalDateList != null) && (this.LocalDateList.Count > 0))
            {
                s_foo += " | [ ";

                for (int i = 0; i < this.LocalDateList.Count; i++)
                {
                    s_foo += this.LocalDateList[i]?.ToString("yyyy-MM-dd") + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }



            if (this.LocalDateTime != null)
            {
                s_foo += " | " + this.LocalDateTime?.ToString("yyyy-MM-dd'T'HH:mm:ss");
            }

            if ((this.LocalDateTimeArray != null) && (this.LocalDateTimeArray.Length > 0))
            {
                s_foo += " | [ ";

                foreach (DateTime? o_foo in this.LocalDateTimeArray)
                {
                    s_foo += o_foo?.ToString("yyyy-MM-dd'T'HH:mm:ss") + ", ";
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



            s_foo += " | " + this.Decimal;

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

            if ((this.DecimalList != null) && (this.DecimalList.Count > 0))
            {
                s_foo += " | [ ";

                for (int i = 0; i < this.DecimalList.Count; i++)
                {
                    s_foo += (this.DecimalList[i] ?? 0.0m) + ", ";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                s_foo += " ]";
            }

            return s_foo;
        }
    }
}
