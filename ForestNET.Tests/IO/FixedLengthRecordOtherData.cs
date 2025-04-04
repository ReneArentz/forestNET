using ForestNET.Lib.IO;
using StandTransMtd = ForestNET.Lib.IO.StandardTransposeMethods;
using StandTransMtdNum = ForestNET.Lib.IO.StandardTransposeMethods.Numbers;
using StandTransMtdDT = ForestNET.Lib.IO.StandardTransposeMethods.DateTime;
using StandTransMtdTS = ForestNET.Lib.IO.StandardTransposeMethods.TimeSpan;

namespace ForestNET.Tests.IO
{
    public class FixedLengthRecordOtherData : FixedLengthRecord
    {

        /* Fields */

        public string? FieldStringId = null;
        public int FieldInt = 0;
        public DateTime? FieldTimestamp = null;
        public DateTime? FieldDate = null;
        public TimeSpan? FieldTime = null;
        public string? FieldShortText = null;

        /* Properties */

        /* Methods */

        public FixedLengthRecordOtherData() : base()
        {

        }

        protected override void Init()
        {
            this.FLRImageClass = typeof(FixedLengthRecordOtherData);

            int i = 1;

            this.Structure = new()
            {
                { i++, new StructureElement("100") },
                { i++, new StructureElement("StringId", 6, StandTransMtd.TransposeString, StandTransMtd.TransposeString) },
                { i++, new StructureElement("Int", 8, StandTransMtdNum.TransposeInteger, StandTransMtdNum.TransposeInteger) },
                { i++, new StructureElement("Timestamp", 14, StandTransMtdDT.TransposeDateTime_yyyymmddhhiiss, StandTransMtdDT.TransposeDateTime_yyyymmddhhiiss) },
                { i++, new StructureElement("Date", 8, StandTransMtdDT.TransposeDateTime_yyyymmdd, StandTransMtdDT.TransposeDateTime_yyyymmddhhiiss) },
                { i++, new StructureElement("Time", 6, StandTransMtdTS.TransposeTimeSpan_hhiiss, StandTransMtdTS.TransposeTimeSpan_hhiiss) },
                { i++, new StructureElement("ShortText", 13, StandTransMtd.TransposeString, StandTransMtd.TransposeString) }
            };

            this.Unique.Add("StringId");

            this.OrderBy.Add("StringId", true);
        }
    }
}
