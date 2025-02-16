using ForestNETLib.IO;
using StandTransMtd = ForestNETLib.IO.StandardTransposeMethods;
using StandTransMtdNum = ForestNETLib.IO.StandardTransposeMethods.Numbers;

namespace ForestNETTests.IO
{
    public class FixedLengthRecordSubtypeOne : FixedLengthRecord
    {

        /* Fields */

        public int FieldThreeDigitId = 0;
        public string? FieldShortText = null;

        /* Properties */

        /* Methods */

        public FixedLengthRecordSubtypeOne() : base()
        {

        }

        protected override void Init()
        {
            this.FLRImageClass = typeof(FixedLengthRecordSubtypeOne);

            int i = 1;

            this.Structure = new()
            {
                { i++, new StructureElement("ThreeDigitId", 3, StandTransMtdNum.TransposeInteger, StandTransMtdNum.TransposeInteger) },
                { i++, new StructureElement("ShortText", 10, StandTransMtd.TransposeString, StandTransMtd.TransposeString) },
            };

            this.Unique.Add("ThreeDigitId");
            this.AllowEmptyUniqueFields = true;
        }
    }
}
