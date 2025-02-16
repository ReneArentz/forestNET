using ForestNETLib.IO;
using StandTransMtd = ForestNETLib.IO.StandardTransposeMethods;
using StandTransMtdDT = ForestNETLib.IO.StandardTransposeMethods.DateTime;

namespace ForestNETTests.IO
{
    public class FixedLengthRecordDataWithSubtypes : FixedLengthRecord
    {

        /* Fields */

        public DateTime? FieldDate = null;
        public List<FixedLengthRecordSubtypeOne>? FieldListOnes = null;
        public List<FixedLengthRecordSubtypeTwo>? FieldListTwos = null;
        public string? FieldLastNotice = null;

        /* Properties */

        /* Methods */

        public FixedLengthRecordDataWithSubtypes() : base()
        {

        }

        protected override void Init()
        {
            this.FLRImageClass = typeof(FixedLengthRecordDataWithSubtypes);

            int i = 1;

            this.Structure = new()
            {
                { i++, new StructureElement("300") },
                { i++, new StructureElement("Date", 8, StandTransMtdDT.TransposeDateTime_yyyymmdd, StandTransMtdDT.TransposeDateTime_yyyymmdd) },
                { i++, new StructureElement("ListOnes", 11, typeof(FixedLengthRecordSubtypeOne)) },
                { i++, new StructureElement("ListTwos", 4, typeof(FixedLengthRecordSubtypeTwo)) },
                { i++, new StructureElement("LastNotice", 15, StandTransMtd.TransposeString, StandTransMtd.TransposeString) },
            };

            this.Unique.Add("Date");
        }
    }
}
