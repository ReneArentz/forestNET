namespace Sandbox.Tests.Net.Https
{
    public class PersonRecord : ForestNET.Lib.SQL.Record<PersonRecord>
    {

        /* Fields */

        public int ColumnId = 0;
        public int ColumnPersonalIdentificationNumber = 0;
        public string? ColumnName = null;
        public int ColumnAge = 0;
        public string? ColumnCity = null;
        public string? ColumnCountry = null;

        /* Properties */

        /* Methods */

        public PersonRecord() : base()
        {

        }

        protected override void Init()
        {
            this.RecordImageClass = typeof(PersonRecord);

            this.Table = "sys_forestnet_person";
            this.Primary.Add("Id");
            this.Unique.Add("PersonalIdentificationNumber");
            this.OrderBy.Add("PersonalIdentificationNumber", true);
            this.Interval = 50;
        }
    }
}
