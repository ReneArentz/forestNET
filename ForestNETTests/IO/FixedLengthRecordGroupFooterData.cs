using ForestNETLib.IO;
using StandTransMtdNum = ForestNETLib.IO.StandardTransposeMethods.Numbers;

namespace ForestNETTests.IO
{
    public class FixedLengthRecordGroupFooterData : FixedLengthRecord
    {

        /* Fields */

        public int FieldAmountRecords = 0;
        public int FieldSumInt = 0;

        /* Properties */

        /* Methods */

        public FixedLengthRecordGroupFooterData() : base()
        {

        }

        protected override void Init()
        {
            this.FLRImageClass = typeof(FixedLengthRecordGroupFooterData);

            int i = 1;

            this.Structure = new()
            {
                { i++, new StructureElement("+F+") },
                { i++, new StructureElement(" ++++++ Amount Records: ") },
                { i++, new StructureElement("AmountRecords", 6, StandTransMtdNum.TransposeInteger, StandTransMtdNum.TransposeInteger) },
                { i++, new StructureElement(" ++++++ Sum Int Divide By 2: ") },
                { i++, new StructureElement("SumInt", 12, StandTransMtdNum.TransposeInteger, StandTransMtdNum.TransposeInteger) },
                { i++, new StructureElement(" ++++++") }
            };
        }
    }
}
