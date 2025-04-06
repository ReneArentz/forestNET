using ForestNET.Lib.IO;
using StandTransMtdFPN = ForestNET.Lib.IO.StandardTransposeMethods.FloatingPointNumbers;

namespace ForestNET.Tests.IO
{
    public class FixedLengthRecordSubtypeTwo : FixedLengthRecord
    {

        /* Fields */

        public double FieldDoubleValue = 0.0d;

        /* Properties */

        /* Methods */

        public FixedLengthRecordSubtypeTwo() : base()
        {

        }

        protected override void Init()
        {
            this.FLRImageClass = typeof(FixedLengthRecordSubtypeTwo);

            int i = 1;

            this.Structure = new()
            {
                { i++, new StructureElement("DoubleValue", 12, StandTransMtdFPN.TransposeDouble, StandTransMtdFPN.TransposeDoubleWithSign, 9, 8, 3, null, null) }, /* we must set position decimal separator, because of sign */
            };
        }
    }
}
