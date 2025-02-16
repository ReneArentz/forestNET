using ForestNETLib.IO;
using StandTransMtd = ForestNETLib.IO.StandardTransposeMethods;
using StandTransMtdNum = ForestNETLib.IO.StandardTransposeMethods.Numbers;
using StandTransMtdFPN = ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers;
using StandTransMtdDT = ForestNETLib.IO.StandardTransposeMethods.DateTime;
using StandTransMtdTS = ForestNETLib.IO.StandardTransposeMethods.TimeSpan;

namespace ForestNETTests.IO
{
    public class FixedLengthRecordData : FixedLengthRecord
    {

        /* Fields */

        public int FieldId = 0;
        public string? FieldUUID = null;
        public string? FieldShortText = null;
        public string? FieldText = null;
        public short FieldSmallInt = 0;
        public int FieldInt = 0;
        public long FieldBigInt = 0;
        public DateTime? FieldTimestamp = null;
        public DateTime? FieldDate = null;
        public TimeSpan? FieldTime = null;
        public TimeSpan? FieldLocalTime = null;
        public DateTime? FieldLocalDate = null;
        public DateTime? FieldLocalDateTime = null;
        public byte FieldByteCol = 0x00;
        public float FieldFloatCol = 0.0f;
        public double FieldDoubleCol = 0.0d;
        public decimal FieldDecimal = 0.0m;
        public bool FieldBool = false;
        public string? FieldText2 = null;
        public string? FieldShortText2 = null;

        /* Properties */

        /* Methods */

        public FixedLengthRecordData() : base()
        {

        }

        protected override void Init()
        {
            this.FLRImageClass = typeof(FixedLengthRecordData);

            int i = 1;

            this.Structure = new()
            {
                { i++, new StructureElement("000") },
                { i++, new StructureElement("Id", 3, StandTransMtdNum.TransposeInteger, StandTransMtdNum.TransposeInteger) },
                { i++, new StructureElement("UUID", 36, StandTransMtd.TransposeString, StandTransMtd.TransposeString) },
                { i++, new StructureElement("ShortText", 16, StandTransMtd.TransposeString, StandTransMtd.TransposeString) },
                { i++, new StructureElement("Text", 64, StandTransMtd.TransposeString, StandTransMtd.TransposeString) },
                { i++, new StructureElement("SmallInt", 6, StandTransMtdNum.TransposeShort, StandTransMtdNum.TransposeShort) },
                { i++, new StructureElement("Int", 10, StandTransMtdNum.TransposeInteger, StandTransMtdNum.TransposeInteger) },
                { i++, new StructureElement("BigInt", 19, StandTransMtdNum.TransposeLong, StandTransMtdNum.TransposeLong) },
                { i++, new StructureElement("Timestamp", 14, StandTransMtdDT.TransposeDateTime_yyyymmddhhiiss, StandTransMtdDT.TransposeDateTime_yyyymmddhhiiss) },
                { i++, new StructureElement("Date", 8, StandTransMtdDT.TransposeDateTime_yyyymmdd, StandTransMtdDT.TransposeDateTime_yyyymmddhhiiss) },
                { i++, new StructureElement("Time", 6, StandTransMtdTS.TransposeTimeSpan_hhiiss, StandTransMtdTS.TransposeTimeSpan_hhiiss) },
                { i++, new StructureElement("LocalDateTime", 20, StandTransMtdDT.TransposeDateTime_ISO8601, StandTransMtdDT.TransposeDateTime_ISO8601) },
                { i++, new StructureElement("LocalDate", 10, StandTransMtdDT.TransposeDateTime_yyyymmdd_ISO, StandTransMtdDT.TransposeDateTime_yyyymmdd_ISO) },
                { i++, new StructureElement("LocalTime", 8, StandTransMtdTS.TransposeTimeSpan_hhiiss_Colon, StandTransMtdTS.TransposeTimeSpan_hhiiss_Colon) },
                { i++, new StructureElement("ByteCol", 3, StandTransMtdNum.TransposeByte, StandTransMtdNum.TransposeByte) },
                { i++, new StructureElement("FloatCol", 11, StandTransMtdFPN.TransposeFloat, StandTransMtdFPN.TransposeFloat, 9, 2, null, null) },
                { i++, new StructureElement("DoubleCol", 19, StandTransMtdFPN.TransposeDouble, StandTransMtdFPN.TransposeDoubleWithSign, 13, 12, 6, null, null) }, /* we must set position decimal separator, because of sign */
                { i++, new StructureElement("Decimal", 23, StandTransMtdFPN.TransposeDecimal, StandTransMtdFPN.TransposeDecimal, 9, 14, null, null) },
                { i++, new StructureElement("Bool", 1, StandTransMtd.TransposeBoolean, StandTransMtd.TransposeBoolean) },
                { i++, new StructureElement("Text2", 32, StandTransMtd.TransposeString, StandTransMtd.TransposeString) },
                { i++, new StructureElement("ShortText2", 8, StandTransMtd.TransposeString, StandTransMtd.TransposeString) }
            };

            this.Unique.Add("Id");
            this.Unique.Add("UUID");
            this.Unique.Add("ShortText");

            this.OrderBy.Add("SmallInt", true);
        }
    }
}
