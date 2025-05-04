namespace ForestNET.Tests.Net.Sock.Https
{
    public class TestSeed : ForestNET.Lib.Net.Https.Dynm.ForestSeed
    {
        public override void PrepareContent()
        {
            /* check if Seed instance is availalbe */
            if (this.Seed == null)
            {
                throw new InvalidDataException("Seed instance is not available");
            }

            this.Seed.Temp.Add("key1", "value1");
            this.Seed.Temp.Add("key2", "value2");
            this.Seed.Temp.Add("key3", "value3");
            this.Seed.Temp.Add("key4", "value4");

            Dictionary<string, Object> a_foo1 = new() {
                { "0", "a" },
                { "1", "b" },
                { "2", "c" },
                { "3", "d" },
                { "4", "e" }
            };
            this.Seed.Temp.Add("list1", a_foo1);

            Dictionary<string, Object> a_subFoo1 = new() {
                { "0", "000" },
                { "1", "001" },
                { "2", "010" },
                { "3", "011" },
                { "4", "100" },
                { "5", "101" },
                { "6", "110" },
                { "7", "111" }
            };

            Dictionary<string, Object> a_subFoo2 = new() {
                { "0", 9 },
                { "1", 8 },
                { "2", 7 },
                { "3", 6 }
            };

            Dictionary<string, Object> a_subFoo3 = new() {
                { "0", true },
                { "1", false },
                { "2", false },
                { "3", true },
                { "4", true }
            };

            Dictionary<string, Object> a_foo2 = new() {
                { "0", a_subFoo1 },
                { "1", a_subFoo2 },
                { "2", a_subFoo3 }
            };

            this.Seed.Temp.Add("list2", a_foo2);

            this.Seed.Temp.Add("list3", new List<string>() { "one", "two", "three", "four", "five" });

            this.Seed.Temp.Add("set", new Dictionary<string, int>() { { "4", 8 }, { "3", 7 }, { "1", 5 }, { "2", 6 } }); /* Set in C# not supported by forestAny, so we just use a dictionary */

            this.Seed.Temp.Add("collection", new List<string>() { "z", "y", "x", "w" });

            Dictionary<string, Object> a_record1 = new() {
                { "name", "Max Mustermann" },
                { "street", "Berlin Street 1" },
                { "country", "DE" },
                { "age", 23 }
            };

            Dictionary<string, Object> a_record2 = new() {
                { "name", "Kim Day" },
                { "street", "London Street 1" },
                { "country", "GB" },
                { "age", 28 }
            };

            Dictionary<string, Object> a_record3 = new() {
                { "name", "Timothy Johnson" },
                { "street", "Washington Street 1" },
                { "country", "US" },
                { "age", 36 }
            };

            this.Seed.Temp.Add("records", new List<Object>() { a_record1, a_record2, a_record3 });

            if (this.Seed.PostData.Count > 0)
            {
                Dictionary<string, Object?> a_postData = [];

                foreach (KeyValuePair<string, string?> o_postDataEntry in this.Seed.PostData)
                {
                    a_postData.Add(o_postDataEntry.Key, o_postDataEntry.Value);
                    //ForestNET.Lib.Global.ILog(o_postDataEntry.Key + " = " + o_postDataEntry.Value);
                }

                this.Seed.Temp.Add("postdata", a_postData);
            }

            if (this.Seed.FileData.Count > 0)
            {
                Dictionary<string, Object?> a_fileData = [];

                foreach (ForestNET.Lib.Net.Https.Dynm.FileData o_fileDataEntry in this.Seed.FileData)
                {
                    a_fileData.Add("field", o_fileDataEntry.FieldName);
                    a_fileData.Add("filename", o_fileDataEntry.FileName);
                    a_fileData.Add("contenttype", o_fileDataEntry.ContentType);
                    a_fileData.Add("filelength", o_fileDataEntry.Data?.Length ?? 0);
                    //ForestNET.Lib.Global.ILog(o_fileDataEntry.FieldName + " | " + o_fileDataEntry.FileName + " | " + o_fileDataEntry.ContentType + " | " + o_fileDataEntry.Data?.Length);
                }

                this.Seed.Temp.Add("filedata", new List<Object>() { a_fileData });
            }
        }
    }
}
