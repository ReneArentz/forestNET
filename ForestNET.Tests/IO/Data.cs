using System.Reflection;

namespace ForestNET.Tests.IO
{
    /// <summary>
    /// Collection of test data classes and generate method for testing fake real data
    /// </summary>
    public class Data
    {
        public static List<ShipOrder> GenerateData()
        {
            List<ShipOrder> a_shipOrders = [];

            try
            {
                ShipOrder o_shipOrder = new()
                {
                    OrderId = "ORD0001",
                    OrderPerson = "Jon Doe",
                    OrderDate = new DateTime(2020, 1, 25),
                    OverallPrice = 388.95f,
                    SomeBools = new bool[] { false, true, false }
                };

                ShipTo o_shipTo = new()
                {
                    Name = "John Smith",
                    Street = "First Ave.",
                    Number = 23,
                    City = "New York",
                    Country = "United States",
                    Delivered = new DateTime(2020, 2, 3, 8, 27, 15),
                    Stored = TimeSpan.Parse("08:45:00"),
                    HighPriority = false
                };

                o_shipOrder.ShipTo = o_shipTo;

                ShipMoreInfo o_shipMoreInfo = new();

                ShipFrom o_shipFrom = new()
                {
                    DeliveredBy = "Francois Montpellier",
                    DeliveredCountry = "France",
                    ShipVia = 10,
                    ShipRegistered = new DateTime(2020, 2, 3, 9, 30, 0)
                };

                o_shipMoreInfo.ShipFrom = o_shipFrom;

                o_shipOrder.ShipMoreInfo = o_shipMoreInfo;

                ShipItem o_shipItem1 = new()
                {
                    Title = "Item #1",
                    Note = "----",
                    ManufacturedTime = new TimeSpan(8, 42, 21),
                    Quantity = 15,
                    Price = 5.25m,
                    Currency = "EUR",
                    Skonto = 2.15d,
                    SomeDecimals =
                    new decimal[] {
                        1.602176634m,
                        8.8541878128m,
                        6.62607015m,
                        9.80665m,
                        3.14159265359m
                    },
                    ShipItemInfo = new ShipItemInfo()
                };
                o_shipItem1.ShipItemInfo.Development = "Development 1.1";

                ShipItem o_shipItem2 = new()
                {
                    Title = "Item #2",
                    Note = "be careful",
                    ManufacturedTime = new TimeSpan(13, 1, 0),
                    Quantity = 35,
                    Price = 1.88m,
                    Currency = "EUR",
                    Skonto = 5.00d,
                    SomeDecimals =
                    new decimal[] {
                        1.602176634m,
                        6.62607015m,
                        3.14159265359m
                    },
                    ShipItemInfo = new ShipItemInfo()
                };
                o_shipItem2.ShipItemInfo.Development = "Development 1.2";
                o_shipItem2.ShipItemInfo.Implementation = "Implementation 1.2";

                ShipItem o_shipItem3 = new()
                {
                    Title = "Item #3",
                    Note = "store cold",
                    ManufacturedTime = new TimeSpan(18, 59, 59),
                    Quantity = 5,
                    Price = 12.23m,
                    Currency = "USD",
                    Skonto = 7.86d,
                    SomeDecimals =
                    new decimal[] {
                        8.8541878128m,
                        9.80665m
                    },
                    ShipItemInfo = new ShipItemInfo()
                };
                o_shipItem3.ShipItemInfo.Construction = "Construction 1.3";

                o_shipOrder.ShipItems.Add(o_shipItem1);
                o_shipOrder.ShipItems.Add(o_shipItem2);
                o_shipOrder.ShipItems.Add(o_shipItem3);

                a_shipOrders.Add(o_shipOrder);

                o_shipOrder = new ShipOrder
                {
                    OrderId = "ORD0002",
                    OrderPerson = "Linda Williams",
                    OrderDate = new DateTime(2020, 1, 13),
                    OverallPrice = 1589.41f,
                    SomeBools = new bool[] { true, false, true }
                };

                o_shipTo = new ShipTo
                {
                    Name = "Elizabeth Miller",
                    Street = "Old Street",
                    Number = 2,
                    City = "Hamburg",
                    Country = "Germany",
                    Delivered = new DateTime(2020, 1, 16, 14, 2, 55),
                    Stored = TimeSpan.Parse("14:15:00"),
                    HighPriority = true
                };

                o_shipOrder.ShipTo = o_shipTo;

                o_shipMoreInfo = new ShipMoreInfo
                {
                    MoreNote = "Note #2"
                };

                o_shipFrom = new ShipFrom
                {
                    DeliveredBy = "Hans Kunz",
                    DeliveredCountry = "Österreich",
                    ShipVia = 5,
                    ShipRegistered = new DateTime(2020, 1, 16, 14, 2, 55)
                };

                o_shipMoreInfo.ShipFrom = o_shipFrom;

                ShipSite o_shipSite = new()
                {
                    SiteName = "Site Q",
                    Building = "Building W3"
                };

                o_shipMoreInfo.ShipSite = o_shipSite;

                ShipLocation o_shipLocation = new()
                {
                    LocName = "Location A",
                    LocFloor = 5,
                    LocRack = 99
                };

                o_shipMoreInfo.ShipLocation = o_shipLocation;

                o_shipOrder.ShipMoreInfo = o_shipMoreInfo;

                o_shipItem1 = new ShipItem
                {
                    Title = "Item #1",
                    Note = "high value",
                    ManufacturedTime = new TimeSpan(23, 0, 0),
                    Quantity = 2,
                    Price = 500.00m,
                    Currency = "USD",
                    Skonto = 0.00d,
                    SomeDecimals =
                    new decimal[] {
                        1.602176634m,
                        8.8541878128m,
                        6.62607015m,
                        9.80665m,
                        3.14159265359m
                    },
                    ShipItemInfo = new ShipItemInfo()
                };
                o_shipItem1.ShipItemInfo.Development = "Development 2.2";
                o_shipItem1.ShipItemInfo.Implementation = "Implementation 2.2";

                o_shipItem2 = new ShipItem
                {
                    Title = "Item #2",
                    Note = "----",
                    ManufacturedTime = new TimeSpan(2, 1, 2),
                    Quantity = 64,
                    Price = 18.55m,
                    Currency = "USD",
                    Skonto = 0.20d,
                    ShipItemInfo = new ShipItemInfo()
                };

                o_shipItem3 = new ShipItem
                {
                    Title = "Item #3",
                    Note = "store dark",
                    ManufacturedTime = new TimeSpan(6, 0, 30),
                    Quantity = 19,
                    Price = 5.87m,
                    Currency = "EUR",
                    Skonto = 9.55d,
                    SomeDecimals =
                    new decimal[] {
                        1.602176634m,
                        decimal.Zero
                    },
                    ShipItemInfo = new ShipItemInfo()
                };
                o_shipItem3.ShipItemInfo.Construction = "Construction 2.3";

                o_shipOrder.ShipItems.Add(o_shipItem1);
                o_shipOrder.ShipItems.Add(o_shipItem2);
                o_shipOrder.ShipItems.Add(o_shipItem3);

                a_shipOrders.Add(o_shipOrder);

                o_shipOrder = new ShipOrder
                {
                    OrderId = "ORD0003",
                    OrderPerson = "David Davis",
                    OrderDate = new DateTime(2020, 1, 28),
                    OverallPrice = 651.33f
                };

                o_shipTo = new ShipTo
                {
                    Name = "Jennifer Garcia",
                    Street = "New Street",
                    Number = 89,
                    City = "London",
                    Country = "United Kingdom",
                    Delivered = new DateTime(2020, 2, 6, 3, 55, 0),
                    Stored = TimeSpan.Parse("04:00:00"),
                    HighPriority = false
                };

                o_shipOrder.ShipTo = o_shipTo;

                o_shipMoreInfo = new ShipMoreInfo
                {
                    MoreNote = "Note #3"
                };

                o_shipFrom = new ShipFrom
                {
                    DeliveredBy = "Jakub Kowalski",
                    DeliveredCountry = "Polska",
                    ShipVia = 30,
                    ShipRegistered = new DateTime(2020, 1, 06, 21, 35, 3)
                };

                o_shipMoreInfo.ShipFrom = o_shipFrom;

                o_shipSite = new ShipSite
                {
                    SiteName = "Site A",
                    Building = "Building C.22"
                };

                o_shipMoreInfo.ShipSite = o_shipSite;

                o_shipOrder.ShipMoreInfo = o_shipMoreInfo;

                o_shipItem1 = new ShipItem
                {
                    Title = "Item #1",
                    Note = "be careful",
                    ManufacturedTime = new TimeSpan(0, 0, 0),
                    Quantity = 20,
                    Price = 50.10m,
                    Currency = "EUR",
                    Skonto = 0.10d,
                    SomeDecimals =
                    new decimal[] {
                        9.80665m,
                        3.14159265359m
                    },
                    ShipItemInfo = new ShipItemInfo()
                };
                o_shipItem1.ShipItemInfo.Construction = "Construction 3.1";

                o_shipItem2 = new ShipItem
                {
                    Title = "Item #2",
                    Note = "----",
                    ManufacturedTime = new TimeSpan(8, 42, 21),
                    Quantity = 32,
                    Price = 29.65m,
                    Currency = "EUR",
                    Skonto = 0.00d,
                    SomeDecimals =
                    new decimal[] {
                        1.602176634m,
                        6.62607015m,
                        3.14159265359m
                    },
                    ShipItemInfo = new ShipItemInfo()
                };
                o_shipItem2.ShipItemInfo.Construction = "Construction 3.2";
                o_shipItem2.ShipItemInfo.Development = "Development 3.2";

                o_shipItem3 = new ShipItem
                {
                    Title = "Item #3",
                    Note = "----",
                    ManufacturedTime = new TimeSpan(4, 21, 12),
                    Quantity = 120,
                    Price = 2.50m,
                    Currency = "EUR",
                    Skonto = 10.90d,
                    SomeDecimals =
                    new decimal[] {
                        decimal.Zero
                    },
                    ShipItemInfo = new ShipItemInfo()
                };
                o_shipItem3.ShipItemInfo.Construction = "Construction 3.3";

                o_shipOrder.ShipItems.Add(o_shipItem1);
                o_shipOrder.ShipItems.Add(o_shipItem2);
                o_shipOrder.ShipItems.Add(o_shipItem3);

                a_shipOrders.Add(o_shipOrder);
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.LogException(o_exc);
            }

            return a_shipOrders;
        }

        public class ShipFrom
        {

            /* Fields */

            private string? s_DeliveredBy;
            private string? s_DeliveredCountry;
            private int i_ShipVia;
            private DateTime o_ShipRegistered;

            /* Properties */

            public string? DeliveredBy
            {
                get { return this.s_DeliveredBy; }
                set { this.s_DeliveredBy = value; }
            }
            public string? DeliveredCountry
            {
                get { return this.s_DeliveredCountry; }
                set { this.s_DeliveredCountry = value; }
            }
            public int ShipVia
            {
                get { return this.i_ShipVia; }
                set { this.i_ShipVia = value; }
            }
            public DateTime ShipRegistered
            {
                get { return this.o_ShipRegistered; }
                set { this.o_ShipRegistered = value; }
            }

            /* Methods */

            public ShipFrom()
            {

            }

            override public string ToString()
            {
                string s_foo = "";

                foreach (PropertyInfo o_property in this.GetType().GetProperties())
                {
                    string s_value = o_property.GetValue(this)?.ToString() ?? "null";
                    s_foo += o_property.Name + " = " + s_value + "|";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 1);

                return s_foo;
            }
        }

        public class ShipItem
        {

            /* Fields */

            private string? s_Title;
            private string? s_Note;
            private TimeSpan o_ManufacturedTime;
            private int i_Quantity;
            private decimal d_Price = decimal.Zero;
            private string? s_Currency;
            private double d_Skonto;
            private decimal[]? a_SomeDecimals;
            private ShipItemInfo? o_ShipItemInfo;

            /* Properties */

            public string? Title
            {
                get { return s_Title; }
                set { this.s_Title = value; }
            }
            public string? Note
            {
                get { return s_Note; }
                set { this.s_Note = value; }
            }
            public TimeSpan ManufacturedTime
            {
                get { return o_ManufacturedTime; }
                set { this.o_ManufacturedTime = value; }
            }
            public int Quantity
            {
                get { return i_Quantity; }
                set { this.i_Quantity = value; }
            }
            public decimal Price
            {
                get { return d_Price; }
                set { this.d_Price = value; }
            }
            public string? Currency
            {
                get { return s_Currency; }
                set { this.s_Currency = value; }
            }
            public double Skonto
            {
                get { return d_Skonto; }
                set { this.d_Skonto = value; }
            }
            public decimal[]? SomeDecimals
            {
                get { return a_SomeDecimals; }
                set { this.a_SomeDecimals = value; }
            }
            public ShipItemInfo? ShipItemInfo
            {
                get { return o_ShipItemInfo; }
                set { this.o_ShipItemInfo = value; }
            }

            /* Methods */

            public ShipItem()
            {

            }

            override public string ToString()
            {
                string s_foo = "";

                foreach (PropertyInfo o_property in this.GetType().GetProperties())
                {
                    string s_value = "";

                    try
                    {
                        if (o_property.PropertyType == typeof(ShipItemInfo))
                        {
                            s_value = "[" + o_property.GetValue(this)?.ToString() ?? "null" + "]";
                        }
                        else if (o_property.PropertyType.IsArray)
                        {
                            s_value = ForestNET.Lib.Helper.PrintArrayList([.. (decimal[])(o_property.GetValue(this) ?? new List<decimal>())]);
                        }
                        else
                        {
                            s_value = o_property.GetValue(this)?.ToString() ?? "null";
                        }

                    }
                    catch (Exception)
                    {
                        s_value = "null";
                    }

                    s_foo += o_property.Name + " = " + s_value + "|";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 1);

                return s_foo;
            }
        }

        public class ShipItemInfo
        {

            /* Fields */

            private string? s_Development;
            private string? s_Construction;
            private string? s_Implementation;

            /* Properties */

            public string? Development
            {
                get { return this.s_Development; }
                set { this.s_Development = value; }
            }
            public string? Construction
            {
                get { return this.s_Construction; }
                set { this.s_Construction = value; }
            }
            public string? Implementation
            {
                get { return this.s_Implementation; }
                set { this.s_Implementation = value; }
            }

            /* Methods */

            public ShipItemInfo()
            {

            }

            override public string ToString()
            {
                string s_foo = "";

                foreach (PropertyInfo o_property in this.GetType().GetProperties())
                {
                    string s_value = o_property.GetValue(this)?.ToString() ?? "null";
                    s_foo += o_property.Name + " = " + s_value + "|";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 1);

                return s_foo;
            }
        }

        public class ShipLocation
        {

            /* Fields */

            private string? s_LocName;
            private int s_LocFloor;
            private int s_LocRack;

            /* Properties */

            public string? LocName
            {
                get { return this.s_LocName; }
                set { this.s_LocName = value; }
            }
            public int LocFloor
            {
                get { return this.s_LocFloor; }
                set { this.s_LocFloor = value; }
            }
            public int LocRack
            {
                get { return this.s_LocRack; }
                set { this.s_LocRack = value; }
            }

            /* Methods */

            public ShipLocation()
            {

            }

            override public string ToString()
            {
                string s_foo = "";

                foreach (PropertyInfo o_property in this.GetType().GetProperties())
                {
                    string s_value = o_property.GetValue(this)?.ToString() ?? "null";
                    s_foo += o_property.Name + " = " + s_value + "|";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 1);

                return s_foo;
            }
        }

        public class ShipMoreInfo
        {

            /* Fields */

            private string? s_MoreNote;
            private ShipFrom? o_ShipFrom;
            private ShipSite? o_ShipSite;
            private ShipLocation? o_ShipLocation;

            /* Properties */

            public string? MoreNote
            {
                get { return this.s_MoreNote; }
                set { this.s_MoreNote = value; }
            }
            public ShipFrom? ShipFrom
            {
                get { return this.o_ShipFrom; }
                set { this.o_ShipFrom = value; }
            }
            public ShipSite? ShipSite
            {
                get { return this.o_ShipSite; }
                set { this.o_ShipSite = value; }
            }
            public ShipLocation? ShipLocation
            {
                get { return this.o_ShipLocation; }
                set { this.o_ShipLocation = value; }
            }

            /* Methods */

            public ShipMoreInfo()
            {

            }

            override public string ToString()
            {
                string s_foo = "";

                foreach (PropertyInfo o_property in this.GetType().GetProperties())
                {
                    string s_value = "";

                    try
                    {
                        if ((o_property.PropertyType == typeof(ShipFrom)) || (o_property.PropertyType == typeof(ShipSite)) || (o_property.PropertyType == typeof(ShipLocation)))
                        {
                            s_value = "[" + o_property.GetValue(this)?.ToString() ?? "null" + "]";
                        }
                        else
                        {
                            s_value = o_property.GetValue(this)?.ToString() ?? "null";
                        }

                    }
                    catch (Exception)
                    {
                        s_value = "null";
                    }

                    s_foo += o_property.Name + " = " + s_value + "|";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 1);

                return s_foo;
            }
        }

        public class ShipSite
        {

            /* Fields */

            private string? s_SiteName;
            private string? s_Building;

            /* Properties */

            public string? SiteName
            {
                get { return this.s_SiteName; }
                set { this.s_SiteName = value; }
            }
            public string? Building
            {
                get { return this.s_Building; }
                set { this.s_Building = value; }
            }

            /* Methods */

            public ShipSite()
            {

            }

            override public string ToString()
            {
                string s_foo = "";

                foreach (PropertyInfo o_property in this.GetType().GetProperties())
                {
                    string s_value = o_property.GetValue(this)?.ToString() ?? "null";
                    s_foo += o_property.Name + " = " + s_value + "|";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 1);

                return s_foo;
            }
        }

        public class ShipTo
        {

            /* Fields */

            private string? s_Name;
            private string? s_Street;
            private int i_Number;
            private string? s_City;
            private string? s_Country;
            private DateTime o_Delivered;
            private TimeSpan o_Stored;
            private bool b_HighPriority;

            /* Properties */

            public string? Name
            {
                get { return this.s_Name; }
                set { this.s_Name = value; }
            }
            public string? Street
            {
                get { return this.s_Street; }
                set { this.s_Street = value; }
            }
            public int Number
            {
                get { return this.i_Number; }
                set { this.i_Number = value; }
            }
            public string? City
            {
                get { return this.s_City; }
                set { this.s_City = value; }
            }
            public string? Country
            {
                get { return this.s_Country; }
                set { this.s_Country = value; }
            }
            public DateTime Delivered
            {
                get { return this.o_Delivered; }
                set { this.o_Delivered = value; }
            }
            public TimeSpan Stored
            {
                get { return this.o_Stored; }
                set { this.o_Stored = value; }
            }
            public bool HighPriority
            {
                get { return this.b_HighPriority; }
                set { this.b_HighPriority = value; }
            }

            /* Methods */

            public ShipTo()
            {

            }

            override public string ToString()
            {
                string s_foo = "";

                foreach (PropertyInfo o_property in this.GetType().GetProperties())
                {
                    string s_value = o_property.GetValue(this)?.ToString() ?? "null";
                    s_foo += o_property.Name + " = " + s_value + "|";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 1);

                return s_foo;
            }
        }

        public class ShipOrder
        {

            /* Fields */

            private string? s_OrderId;
            private DateTime o_OrderDate;
            private float f_OverallPrice;
            private string? s_OrderPerson;
            private ShipTo? o_ShipTo;
            private ShipMoreInfo? o_ShipMoreInfo;
            private bool[]? a_SomeBools;
            private List<ShipItem> a_ShipItems = [];

            /* Properties */

            public string? OrderId
            {
                get { return this.s_OrderId; }
                set { this.s_OrderId = value; }
            }
            public DateTime OrderDate
            {
                get { return this.o_OrderDate; }
                set { this.o_OrderDate = value; }
            }
            public float OverallPrice
            {
                get { return this.f_OverallPrice; }
                set { this.f_OverallPrice = value; }
            }
            public string? OrderPerson
            {
                get { return this.s_OrderPerson; }
                set { this.s_OrderPerson = value; }
            }
            public ShipTo? ShipTo
            {
                get { return this.o_ShipTo; }
                set { this.o_ShipTo = value; }
            }
            public ShipMoreInfo? ShipMoreInfo
            {
                get { return this.o_ShipMoreInfo; }
                set { this.o_ShipMoreInfo = value; }
            }
            public bool[]? SomeBools
            {
                get { return this.a_SomeBools; }
                set { this.a_SomeBools = value; }
            }
            public List<ShipItem> ShipItems
            {
                get { return this.a_ShipItems; }
                set { this.a_ShipItems = value; }
            }

            /* Methods */

            public ShipOrder()
            {

            }

            override public string ToString()
            {
                string s_foo = "";

                foreach (PropertyInfo o_property in this.GetType().GetProperties())
                {
                    string s_value = "";

                    try
                    {
                        if ((o_property.PropertyType == typeof(ShipTo)) || (o_property.PropertyType == typeof(ShipMoreInfo)))
                        {
                            s_value = "[" + o_property.GetValue(this)?.ToString() ?? "null" + "]";
                        }
                        else if (o_property.PropertyType.IsArray)
                        {
                            s_value = ForestNET.Lib.Helper.PrintArrayList([.. (bool[])(o_property.GetValue(this) ?? new List<bool>())]);
                        }
                        else if ((o_property.PropertyType.IsGenericType) && (o_property.PropertyType.GetGenericTypeDefinition() == typeof(List<>)))
                        {
                            s_value = ForestNET.Lib.Helper.PrintArrayList((List<ShipItem>)(o_property.GetValue(this) ?? new List<ShipItem>()));
                        }
                        else
                        {
                            s_value = o_property.GetValue(this)?.ToString() ?? "null";
                        }

                    }
                    catch (Exception)
                    {
                        s_value = "null";
                    }

                    s_foo += o_property.Name + " = " + s_value + "|";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 1);

                return s_foo;
            }
        }

        public class ShipOrderCollection
        {

            /* Fields */

            private int i_OrderAmount;
            private List<ShipOrder> a_ShipOrders = [];

            /* Properties */

            public int OrderAmount
            {
                get { return i_OrderAmount; }
                set { this.i_OrderAmount = value; }
            }
            public List<ShipOrder> ShipOrders
            {
                get { return a_ShipOrders; }
                set { this.a_ShipOrders = value; }
            }

            /* Methods */

            public ShipOrderCollection()
            {

            }

            override public string ToString()
            {
                string s_foo = "";

                foreach (PropertyInfo o_property in this.GetType().GetProperties())
                {
                    string s_value = "";

                    try
                    {
                        if ((o_property.PropertyType.IsGenericType) && (o_property.PropertyType.GetGenericTypeDefinition() == typeof(List<>)))
                        {
                            s_value = ForestNET.Lib.Helper.PrintArrayList((List<ShipOrder>)(o_property.GetValue(this) ?? new List<ShipOrder>()));
                        }
                        else
                        {
                            s_value = o_property.GetValue(this)?.ToString() ?? "null";
                        }

                    }
                    catch (Exception)
                    {
                        s_value = "null";
                    }

                    s_foo += o_property.Name + " = " + s_value + "|";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 1);

                return s_foo;
            }
        }

        public class SimpleClass : IConvertible
        {

            /* Fields */

            /* Properties */

            public string? ValueA { get; set; }
            public string? ValueB { get; set; }
            public string? ValueC { get; set; }
            public List<int> ValueD { get; set; } = [];
            public float[]? ValueE { get; set; }

            /* Methods */

            public SimpleClass()
            {

            }

            public SimpleClass(string p_s_valueA, string p_s_valueB, string? p_s_valueC) : this(p_s_valueA, p_s_valueB, p_s_valueC, null)
            {

            }

            public SimpleClass(string p_s_valueA, string p_s_valueB, string? p_s_valueC, List<int>? p_a_valueD) : this(p_s_valueA, p_s_valueB, p_s_valueC, p_a_valueD, null)
            {

            }

            public SimpleClass(string p_s_valueA, string p_s_valueB, string? p_s_valueC, List<int>? p_a_valueD, float[]? p_a_valueE)
            {
                this.ValueA = p_s_valueA;
                this.ValueB = p_s_valueB;
                this.ValueC = p_s_valueC;

                if (p_a_valueD != null)
                {
                    this.ValueD = p_a_valueD;
                }

                if (p_a_valueE != null)
                {
                    this.ValueE = p_a_valueE;
                }
            }

            override public string ToString()
            {
                string s_foo = "";

                foreach (FieldInfo o_field in this.GetType().GetFields())
                {
                    string s_value = "";

                    try
                    {
                        if (o_field.FieldType.IsArray)
                        {
                            s_value = ForestNET.Lib.Helper.PrintArrayList([.. (float[])(o_field.GetValue(this) ?? new List<float>())]);
                        }
                        else if ((o_field.FieldType.IsGenericType) && (o_field.FieldType.GetGenericTypeDefinition() == typeof(List<>)))
                        {
                            s_value = ForestNET.Lib.Helper.PrintArrayList((List<int>)(o_field.GetValue(this) ?? new List<int>()));
                        }
                        else
                        {
                            s_value = o_field.GetValue(this)?.ToString() ?? "null";
                        }

                    }
                    catch (Exception)
                    {
                        s_value = "null";
                    }

                    s_foo += o_field.Name + " = " + s_value + "|";
                }

                foreach (PropertyInfo o_property in this.GetType().GetProperties())
                {
                    string s_value = "";

                    try
                    {
                        if (o_property.PropertyType.IsArray)
                        {
                            s_value = ForestNET.Lib.Helper.PrintArrayList([.. (float[])(o_property.GetValue(this) ?? new List<float>())]);
                        }
                        else if ((o_property.PropertyType.IsGenericType) && (o_property.PropertyType.GetGenericTypeDefinition() == typeof(List<>)))
                        {
                            s_value = ForestNET.Lib.Helper.PrintArrayList((List<int>)(o_property.GetValue(this) ?? new List<int>()));
                        }
                        else
                        {
                            s_value = o_property.GetValue(this)?.ToString() ?? "null";
                        }

                    }
                    catch (Exception)
                    {
                        s_value = "null";
                    }

                    s_foo += o_property.Name + " = " + s_value + "|";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 1);

                return s_foo;
            }

            public TypeCode GetTypeCode()
            {
                return TypeCode.Object;
            }

            public bool ToBoolean(IFormatProvider? provider)
            {
                throw new NotImplementedException();
            }

            public byte ToByte(IFormatProvider? provider)
            {
                throw new NotImplementedException();
            }

            public char ToChar(IFormatProvider? provider)
            {
                throw new NotImplementedException();
            }

            public DateTime ToDateTime(IFormatProvider? provider)
            {
                throw new NotImplementedException();
            }

            public decimal ToDecimal(IFormatProvider? provider)
            {
                throw new NotImplementedException();
            }

            public double ToDouble(IFormatProvider? provider)
            {
                throw new NotImplementedException();
            }

            public short ToInt16(IFormatProvider? provider)
            {
                throw new NotImplementedException();
            }

            public int ToInt32(IFormatProvider? provider)
            {
                throw new NotImplementedException();
            }

            public long ToInt64(IFormatProvider? provider)
            {
                throw new NotImplementedException();
            }

            public sbyte ToSByte(IFormatProvider? provider)
            {
                throw new NotImplementedException();
            }

            public float ToSingle(IFormatProvider? provider)
            {
                throw new NotImplementedException();
            }

            public string ToString(IFormatProvider? provider)
            {
                throw new NotImplementedException();
            }

            public object ToType(Type conversionType, IFormatProvider? provider)
            {
                return this;
            }

            public ushort ToUInt16(IFormatProvider? provider)
            {
                throw new NotImplementedException();
            }

            public uint ToUInt32(IFormatProvider? provider)
            {
                throw new NotImplementedException();
            }

            public ulong ToUInt64(IFormatProvider? provider)
            {
                throw new NotImplementedException();
            }
        }

        public class SimpleClassCollection : IConvertible
        {

            /* Fields */

            /* Properties */

            public List<SimpleClass> SimpleClasses { get; set; } = [];

            /* Methods */

            public SimpleClassCollection()
            {

            }

            public SimpleClassCollection(List<SimpleClass> p_a_data)
            {
                this.SimpleClasses = p_a_data;
            }

            override public string ToString()
            {
                string s_foo = "";

                foreach (FieldInfo o_field in this.GetType().GetFields())
                {
                    string s_value = "";

                    try
                    {
                        if ((o_field.FieldType.IsGenericType) && (o_field.FieldType.GetGenericTypeDefinition() == typeof(List<>)))
                        {
                            s_value = ForestNET.Lib.Helper.PrintArrayList((List<SimpleClass>)(o_field.GetValue(this) ?? new List<SimpleClass>()));
                        }
                        else
                        {
                            s_value = o_field.GetValue(this)?.ToString() ?? "null";
                        }

                    }
                    catch (Exception)
                    {
                        s_value = "null";
                    }

                    s_foo += o_field.Name + " = " + s_value + "|";
                }

                foreach (PropertyInfo o_property in this.GetType().GetProperties())
                {
                    string s_value = "";

                    try
                    {
                        if ((o_property.PropertyType.IsGenericType) && (o_property.PropertyType.GetGenericTypeDefinition() == typeof(List<>)))
                        {
                            s_value = ForestNET.Lib.Helper.PrintArrayList((List<SimpleClass>)(o_property.GetValue(this) ?? new List<SimpleClass>()));
                        }
                        else
                        {
                            s_value = o_property.GetValue(this)?.ToString() ?? "null";
                        }

                    }
                    catch (Exception)
                    {
                        s_value = "null";
                    }

                    s_foo += o_property.Name + " = " + s_value + "|";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 1);

                return s_foo;
            }

            public TypeCode GetTypeCode()
            {
                return TypeCode.Object;
            }

            public bool ToBoolean(IFormatProvider? provider)
            {
                throw new NotImplementedException();
            }

            public byte ToByte(IFormatProvider? provider)
            {
                throw new NotImplementedException();
            }

            public char ToChar(IFormatProvider? provider)
            {
                throw new NotImplementedException();
            }

            public DateTime ToDateTime(IFormatProvider? provider)
            {
                throw new NotImplementedException();
            }

            public decimal ToDecimal(IFormatProvider? provider)
            {
                throw new NotImplementedException();
            }

            public double ToDouble(IFormatProvider? provider)
            {
                throw new NotImplementedException();
            }

            public short ToInt16(IFormatProvider? provider)
            {
                throw new NotImplementedException();
            }

            public int ToInt32(IFormatProvider? provider)
            {
                throw new NotImplementedException();
            }

            public long ToInt64(IFormatProvider? provider)
            {
                throw new NotImplementedException();
            }

            public sbyte ToSByte(IFormatProvider? provider)
            {
                throw new NotImplementedException();
            }

            public float ToSingle(IFormatProvider? provider)
            {
                throw new NotImplementedException();
            }

            public string ToString(IFormatProvider? provider)
            {
                throw new NotImplementedException();
            }

            public object ToType(Type conversionType, IFormatProvider? provider)
            {
                return this;
            }

            public ushort ToUInt16(IFormatProvider? provider)
            {
                throw new NotImplementedException();
            }

            public uint ToUInt32(IFormatProvider? provider)
            {
                throw new NotImplementedException();
            }

            public ulong ToUInt64(IFormatProvider? provider)
            {
                throw new NotImplementedException();
            }
        }
    }
}
