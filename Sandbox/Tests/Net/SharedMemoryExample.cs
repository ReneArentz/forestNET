namespace Sandbox.Tests.Net
{
    public class SharedMemoryExample : ForestNET.Lib.Net.Sock.Com.SharedMemory<SharedMemoryExample>
    {

        /* Fields */

#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable IDE0051 // Remove unused private members
        private int Id = 0;
        private string? UUID = null;
        private string? ShortText = null;
        private string? Text = null;
        private short SmallInt = 0;
        private int Int = 0;
        private long BigInt = 0;
        private DateTime? Timestamp = null;
        private DateTime? Date = null;
        private TimeSpan? Time = null;
        private DateTime? LocalDateTime = null;
        private DateTime? LocalDate = null;
        private TimeSpan? LocalTime = null;
        private double DoubleCol = 0.0d;
        private decimal Decimal = decimal.Zero;
        private bool Bool = false;
        private string? Text2 = null;
        private string? ShortText2 = null;
        private float FloatValue = 0.0f;
#pragma warning restore IDE0044 // Add readonly modifier
#pragma warning restore IDE0051 // Remove unused private members

        /* Properties */

        /* Methods */

        public SharedMemoryExample() : base()
        {

        }

        protected override void Init()
        {
            this.MirrorClass = typeof(SharedMemoryExample);
        }
    }
}
