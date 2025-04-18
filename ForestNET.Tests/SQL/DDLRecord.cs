namespace ForestNET.Tests.SQL
{
    public class DDLRecord : ForestNET.Lib.SQL.Record<DDLRecord>
    {

        /* Fields */

        public int ColumnId = 0;
        public string? ColumnUUID = null;
        public string? ColumnShortText = null;
        public string? ColumnText = null;
        public short ColumnSmallInt = 0;
        public int ColumnInt = 0;
        public long ColumnBigInt = 0;
        public DateTime? ColumnTimestamp = null;
        public DateTime? ColumnDate = null;
        public TimeSpan? ColumnTime = null;
        public DateTime? ColumnLocalDateTime = null;
        public DateTime? ColumnLocalDate = null;
        public TimeSpan? ColumnLocalTime = null;
        public double ColumnDoubleCol = 0d;
        public decimal ColumnDecimal = decimal.Zero;
        public bool ColumnBool = false;
        public string? ColumnText2 = null;
        public string? ColumnShortText2 = null;

        /* Properties */

        /* Methods */

        public DDLRecord() : base()
        {

        }

        protected override void Init()
        {
            this.RecordImageClass = typeof(DDLRecord);

            this.Table = "sys_forestnet_testddl2";
            this.Primary.Add("Id");
            this.Unique.Add("UUID");
            this.Unique.Add("ShortText");
            this.OrderBy.Add("SmallInt", true);
            this.Interval = 50;
        }
    }
}
