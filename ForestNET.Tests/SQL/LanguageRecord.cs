namespace ForestNET.Tests.SQL
{
    public class LanguageRecord : ForestNET.Lib.SQL.Record<LanguageRecord>
    {

        /* Fields */

        public int ColumnId = 0;
        public string? ColumnUUID = null;
        public string? ColumnCode = null;
        public string? ColumnLanguage = null;

        /* Properties */

        /* Methods */

        public LanguageRecord() : base()
        {

        }

        protected override void Init()
        {
            this.RecordImageClass = typeof(LanguageRecord);

            this.Table = "sys_forestnet_language";
            this.Primary.Add("Id");
            this.Unique.Add("UUID");
            this.Unique.Add("Code");
            this.OrderBy.Add("Id", true);
            this.Interval = 50;
        }
    }
}
