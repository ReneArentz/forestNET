using ForestNETLib.IO;
using StandTransMtdNum = ForestNETLib.IO.StandardTransposeMethods.Numbers;
using StandTransMtdFPN = ForestNETLib.IO.StandardTransposeMethods.FloatingPointNumbers;
using StandTransMtdDT = ForestNETLib.IO.StandardTransposeMethods.DateTime;

namespace ForestNETTests.IO
{
    public class FixedLengthRecordGroupHeaderData : FixedLengthRecord
    {

        /* Fields */

        public int FieldCustomerNumber = 0;
        public DateTime? FieldDate = null;
        public double FieldDoubleWithSeparator = 0.0d;

        /* Properties */

        /* Methods */

        public FixedLengthRecordGroupHeaderData() : base()
        {

        }

        protected override void Init()
        {
            this.FLRImageClass = typeof(FixedLengthRecordGroupHeaderData);

            int i = 1;

            this.Structure = new()
            {
                { i++, new StructureElement("+H+") },
                { i++, new StructureElement(" ++++++ Customer Number: ") },
                { i++, new StructureElement("CustomerNumber", 5, StandTransMtdNum.TransposeInteger, StandTransMtdNum.TransposeInteger) },
                { i++, new StructureElement(" ++++++ Date: ") },
                { i++, new StructureElement("Date", 8, StandTransMtdDT.TransposeDateTime_yyyymmdd, StandTransMtdDT.TransposeDateTime_yyyymmdd) },
                { i++, new StructureElement(" ++++++ DoubleWithSeparator: ") },
                { i++, new StructureElement("DoubleWithSeparator", 15, StandTransMtdFPN.TransposeDouble, StandTransMtdFPN.TransposeDouble, 0, 8, 6, ".", null) }, /* we must set position decimal separator to '0', because decimal separator is part of string */
                { i++, new StructureElement(" ++++++") }
            };

            this.Unique.Add("CustomerNumber");

            this.OrderBy.Add("CustomerNumber", true);
        }
    }
}
