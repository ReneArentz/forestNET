using ForestNETLib.IO;
using StandTransMtd = ForestNETLib.IO.StandardTransposeMethods;
using StandTransMtdNum = ForestNETLib.IO.StandardTransposeMethods.Numbers;
using StandTransMtdFPN = ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers;

namespace ForestNETTests.IO
{
    public class FixedLengthRecordAnotherData : FixedLengthRecord
    {

        /* Fields */

        public string? FieldStringId = null;
        public float FieldFloatCol = 0.0f;
        public double FieldDoubleCol = 0.0d;
        public decimal FieldDecimal = 0.0m;
        public int FieldInt = 0;

        /* Properties */

        /* Methods */

        public FixedLengthRecordAnotherData() : base()
        {

        }

        protected override void Init()
        {
            this.FLRImageClass = typeof(FixedLengthRecordAnotherData);

            int i = 1;

            this.Structure = new()
            {
                { i++, new StructureElement("200") },
                { i++, new StructureElement("StringId", 6, StandTransMtd.TransposeString, StandTransMtd.TransposeString) },
                { i++, new StructureElement("FloatCol", 11, StandTransMtdFPN.TransposeFloat, StandTransMtdFPN.TransposeFloat, 9, 2, null, null) },
                { i++, new StructureElement("DoubleCol", 19, StandTransMtdFPN.TransposeDouble, StandTransMtdFPN.TransposeDoubleWithSign, 13, 12, 6, null, null) }, /* we must set position decimal separator, because of sign */
                { i++, new StructureElement("Decimal", 23, StandTransMtdFPN.TransposeDecimal, StandTransMtdFPN.TransposeDecimal, 9, 14, null, null) },
                { i++, new StructureElement("Int", 10, StandTransMtdNum.TransposeInteger, StandTransMtdNum.TransposeInteger) }
            };

            this.Unique.Add("StringId");

            this.OrderBy.Add("StringId", true);
        }
    }
}
