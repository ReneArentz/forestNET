using System.Text;

namespace ForestNETLib.IO
{
    /// <summary>
    /// CSV class to encode and decode c# objects to csv files.
    /// line break, delimiter and array delimiter changeable.
    /// access to object fields can be directly on public fields or with public properties on private fields.
    /// NOTE: mostly only primitive types supported for encoding and decoding.
    /// </summary>
    public class CSV
    {

        /* Fields */

        private string s_delimiter;
        private string s_arrayDelimiter;
        private string s_lineBreak;
        private bool b_returnArrayElementNullNotZero;
        private Encoding o_encoding;
        public readonly string[] a_allowedTypes = new string[] { "System.Byte", "System.Int16", "System.Int32", "System.Int64", "System.Single", "System.Double", "System.Decimal", "System.Char", "System.Boolean", "System.String", "System.DateTime", "System.TimeSpan", "System.SByte", "System.UInt16", "System.UInt32", "System.UInt64", "System.Object" };

        /* Properties */

        public string Delimiter
        {
            get
            {
                return this.s_delimiter;
            }
            set
            {
                if (value.Length != 1)
                {
                    throw new ArgumentException("Delimiter must have length(1) - ['" + value + "' -> length(" + value.Length + ")]");
                }

                this.s_delimiter = value;
                ForestNETLib.Core.Global.ILogConfig("updated delimiter to '" + this.s_delimiter + "'");
            }
        }

        public string ArrayDelimiter
        {
            get
            {
                return this.s_arrayDelimiter;
            }
            set
            {
                if (value.Length != 1)
                {
                    throw new ArgumentException("ArrayDelimiter must have length(1) - ['" + value + "' -> length(" + value.Length + ")]");
                }

                this.s_arrayDelimiter = value;
                ForestNETLib.Core.Global.ILogConfig("updated array delimiter to '" + this.s_arrayDelimiter + "'");
            }
        }

        /// <summary>
        /// Determine line break characters for reading and writing csv files
        /// </summary>
        public string LineBreak
        {
            get
            {
                return this.s_lineBreak;
            }
            set
            {
                if (value.Length < 1)
                {
                    throw new ArgumentException("Line break must have at least a length of 1, but length is '" + value.Length + "'");
                }

                this.s_lineBreak = value;
                ForestNETLib.Core.Global.ILogConfig("updated line break to [" + ForestNETLib.Core.Helper.BytesToHexString(this.o_encoding.GetBytes(this.s_lineBreak), true) + "]");
            }
        }

        /// <summary>
        /// Determine if properties shall be used handling Objects.
        /// </summary>
        public bool UseProperties
        {
            get; set;
        }

        /// <summary>
        /// Determine if you want to return empty values as null or 0
        /// </summary>
        public bool ReturnArrayElementNullNotZero
        {
            get
            {
                return this.b_returnArrayElementNullNotZero;
            }
            set
            {
                this.b_returnArrayElementNullNotZero = value;
            }
        }

        public Encoding Encoding
        {
            get
            {
                return this.o_encoding;
            }
            set
            {
                this.o_encoding = value;
                ForestNETLib.Core.Global.ILogConfig("updated encoding to '" + this.o_encoding.ToString() + "'");
            }
        }

        /* Methods */

        /// <summary>
        /// Constructor, setting field delimiter as ',' and array element delimiter as '~', standard charset UTF-8
        /// </summary>
        /// <exception cref="ArgumentException">invalid length of field or array element delimiter</exception>
        public CSV() : this(",", "~")
        {

        }

        /// <summary>
        /// Constructor, setting field delimiter as parameter and array element delimiter as '~', standard charset UTF-8
        /// </summary>
        /// <param name="p_s_delimiter">field delimiter e.g. ',' or ';'</param>
        /// <exception cref="ArgumentException">invalid length of field or array element delimiter</exception>
        public CSV(string p_s_delimiter) : this(p_s_delimiter, "~")
        {

        }

        /// <summary>
        /// Constructor, standard charset UTF-8
        /// </summary>
        /// <param name="p_s_delimiter">field delimiter e.g. ',' or ';'</param>
        /// <param name="p_s_arrayDelimiter">array element delimiter e.g. '~'</param>
        /// <exception cref="ArgumentException">invalid length of field or array element delimiter</exception>
        public CSV(string p_s_delimiter, string p_s_arrayDelimiter)
        {
            this.o_encoding = Encoding.UTF8;
            this.s_delimiter = p_s_delimiter;
            this.s_arrayDelimiter = p_s_arrayDelimiter;
            this.s_lineBreak = ForestNETLib.IO.File.NEWLINE;
            this.UseProperties = false;
            this.ReturnArrayElementNullNotZero = false;
        }

        /// <summary>
        /// Method to encode a object to a csv content string
        /// supported types:
        /// byte, short, int, long, float, double, decimal, char, boolean, string,
        /// DateTime, TimeSpan, Object, sbyte, ushot, uint, ulong
        /// </summary>
        /// <param name="p_o_object">any object which will be encoded to a csv content string</param>
        /// <returns>string</returns>
        /// <exception cref="MemberAccessException">could not invoke member, access violation</exception>
        /// <exception cref="ArgumentException">invalid type parameter</exception>
        public string PrintCSV(Object p_o_object)
        {
            return this.PrintCSV(p_o_object, true);
        }

        /// <summary>
        /// Method to encode a object to a csv content string
        /// supported types:
        /// byte, short, int, long, float, double, decimal, char, boolean, string,
        /// DateTime, TimeSpan, Object, sbyte, ushot, uint, ulong
        /// </summary>
        /// <param name="p_o_object">any object which will be encoded to a csv content string</param>
        /// <param name="p_b_generateHeaderLine">true - add first line as header line, false - no header line</param>
        /// <returns>string</returns>
        /// <exception cref="MemberAccessException">could not invoke member, access violation</exception>
        /// <exception cref="ArgumentException">invalid type parameter</exception>
        public string PrintCSV(Object p_o_object, bool p_b_generateHeaderLine)
        {
            string s_csv = "";

            /* check if parameter object is an array list */
            if ((p_o_object.GetType() != null) && (p_o_object.GetType().IsGenericType) && (p_o_object.GetType().GetGenericTypeDefinition() == typeof(List<>)))
            {
                /* cast parameter object as list with unknown generic type */
                List<Object> a_objects = [.. ((System.Collections.IEnumerable)p_o_object).Cast<Object>()];

                /* only execute if we have more than one array element */
                if (a_objects.Count > 0)
                {
                    /* add header line */
                    if (p_b_generateHeaderLine)
                    {
                        s_csv += this.GenerateHeaderLine(a_objects[0]);
                    }

                    /* iterate objects in list and encode data to csv recursively */
                    for (int i = 0; i < a_objects.Count; i++)
                    {
                        /* skip fields which are also a generic list to avoid recursion overflow */
                        if ((a_objects[i].GetType() != null) && (a_objects[i].GetType().IsGenericType) && (a_objects[i].GetType().GetGenericTypeDefinition() == typeof(List<>)))
                        {
                            continue;
                        }

                        /* encode array object element */
                        s_csv += this.PrintCSV(a_objects[i], false);
                    }

                    /* remove last line break in csv file */
                    s_csv = s_csv.Substring(0, s_csv.Length - this.s_lineBreak.Length);
                }
            }
            else
            { /* handle parameter object as standard object */
                /* add header line */
                if (p_b_generateHeaderLine)
                {
                    s_csv += this.GenerateHeaderLine(p_o_object);
                }

                s_csv += this.EncodeObject(p_o_object);
            }

            /* return csv string and add line break */
            return s_csv + this.s_lineBreak;
        }

        /* encoding data to csv */

        /// <summary>
        /// Methods to encode an object to a csv file, printing header line, not overwriting existing csv file
        /// supported types:
        /// byte, short, int, long, float, double, decimal, char, boolean, string,
        /// DateTime, TimeSpan, TimeSpan, Object, sbyte, ushot, uint, ulong
        /// </summary>
        /// <param name="p_o_object">object parameter which will be encoded</param>
        /// <param name="p_s_csvFile">full path to csv file</param>
        /// <returns>ForestNETLib.IO.File object</returns>
        /// <exception cref="System.IO.IOException">exception creating csv file or replacing content of csv file</exception>
        /// <exception cref="MemberAccessException">could not invoke member, access violation</exception>
        /// <exception cref="ArgumentException">invalid type parameter</exception>
        public File EncodeCSV(Object p_o_object, string p_s_csvFile)
        {
            return this.EncodeCSV(p_o_object, p_s_csvFile, true, false);
        }

        /// <summary>
        /// Methods to encode an object to a csv file, not overwriting existing csv file
        /// supported types:
        /// byte, short, int, long, float, double, decimal, char, boolean, string,
        /// DateTime, TimeSpan, Object, sbyte, ushot, uint, ulong
        /// </summary>
        /// <param name="p_o_object">object parameter which will be encoded</param>
        /// <param name="p_s_csvFile">full path to csv file</param>
        /// <param name="p_b_printHeader">true - add first line as header line, false - no header line</param>
        /// <returns>ForestNETLib.IO.File object</returns>
        /// <exception cref="System.IO.IOException">exception creating csv file or replacing content of csv file</exception>
        /// <exception cref="MemberAccessException">could not invoke member, access violation</exception>
        /// <exception cref="ArgumentException">invalid type parameter</exception>
        public File EncodeCSV(Object p_o_object, string p_s_csvFile, bool p_b_printHeader)
        {
            return this.EncodeCSV(p_o_object, p_s_csvFile, p_b_printHeader, false);
        }

        /// <summary>
        /// Methods to encode an object to a csv file
        /// supported types:
        /// byte, short, int, long, float, double, decimal, char, boolean, string,
        /// DateTime, TimeSpan, Object, sbyte, ushot, uint, ulong
        /// </summary>
        /// <param name="p_o_object">object parameter which will be encoded</param>
        /// <param name="p_s_csvFile">full path to csv file</param>
        /// <param name="p_b_printHeader">true - add first line as header line, false - no header line</param>
        /// <param name="p_b_overwrite">true - overwrite existing csv file, false - keep existing csv file</param>
        /// <returns>ForestNETLib.IO.File object</returns>
        /// <exception cref="System.IO.IOException">exception creating csv file or replacing content of csv file</exception>
        /// <exception cref="MemberAccessException">could not invoke member, access violation</exception>
        /// <exception cref="ArgumentException">invalid type parameter</exception>
        public File EncodeCSV(Object p_o_object, string p_s_csvFile, bool p_b_printHeader, bool p_b_overwrite)
        {
            string s_csv = "";

            /* check if parameter object is an array list */
            if ((p_o_object.GetType() != null) && (p_o_object.GetType().IsGenericType) && (p_o_object.GetType().GetGenericTypeDefinition() == typeof(List<>)))
            {
                /* cast current object as list with unknown generic type */
                List<Object> a_objects = [.. ((System.Collections.IEnumerable)p_o_object).Cast<Object>()];

                /* only execute if we have more than one array element */
                if (a_objects.Count > 0)
                {
                    /* add header line */
                    if (p_b_printHeader)
                    {
                        s_csv += this.GenerateHeaderLine(a_objects[0]);
                    }

                    /* iterate objects in list and encode data to csv recursively */
                    for (int i = 0; i < a_objects.Count; i++)
                    {
                        /* encode array object element and add line break */
                        s_csv += this.EncodeObject(a_objects[i]) + this.s_lineBreak;
                    }

                    /* remove last line break in csv file */
                    s_csv = s_csv.Substring(0, s_csv.Length - this.s_lineBreak.Length);
                }
            }
            else
            {
                /* add header line */
                if (p_b_printHeader)
                {
                    s_csv += this.GenerateHeaderLine(p_o_object);
                }

                /* encode object */
                s_csv += this.EncodeObject(p_o_object);
            }

            /* if file does not exist we must create an new file */
            if (!File.Exists(p_s_csvFile))
            {
                if (p_b_overwrite)
                {
                    p_b_overwrite = false;
                }
            }

            /* open (new) file */
            ForestNETLib.IO.File o_file = new(p_s_csvFile, this.o_encoding, !p_b_overwrite, ForestNETLib.IO.File.NEWLINE);

            /* save csv encoded data into file */
            o_file.ReplaceContent(s_csv);

            /* return file object */
            return o_file;
        }

        /// <summary>
        /// Generate header line for csv file
        /// supported types:
        /// byte, short, int, long, float, double, decimal, char, boolean, string,
        /// DateTime, TimeSpan, Object, sbyte, ushot, uint, ulong
        /// </summary>
        /// <param name="p_o_object">object parameter to get object field names for header fields</param>
        /// <returns>csv header line</returns>
        private string GenerateHeaderLine(Object p_o_object)
        {
            string s_csv = "";

            if (!this.UseProperties) /* read all fields of object parameter */
            {
                /* iterate all fields of parameter object */
                foreach (System.Reflection.FieldInfo o_fieldInfo in p_o_object.GetType().GetFields())
                {
                    /* skip empty field info instance */
                    if (o_fieldInfo == null)
                    {
                        continue;
                    }

                    /* skip empty field type */
                    if (o_fieldInfo.FieldType == null)
                    {
                        continue;
                    }

                    ForestNETLib.Core.Global.ILogFiner("iterate all fields - field '" + o_fieldInfo.Name + "' with type '" + o_fieldInfo.FieldType.FullName + "'");

                    /* if field of parameter object is of type list */
                    if ((o_fieldInfo.FieldType.IsGenericType) && (o_fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>)))
                    {
                        ForestNETLib.Core.Global.ILogFiner("iterate all fields - array element type '" + o_fieldInfo.FieldType.GenericTypeArguments[0].FullName + "'");

                        /* if type of field list is not contained in the list of allowed types, skip this field */
                        if (!a_allowedTypes.Contains(o_fieldInfo.FieldType.GenericTypeArguments[0].FullName))
                        {
                            ForestNETLib.Core.Global.ILogFinest("skip generic list field - type '" + o_fieldInfo.FieldType.GenericTypeArguments[0].FullName + "' not allowed");
                            continue;
                        }
                    }
                    else if ((o_fieldInfo.FieldType.IsArray) && (!a_allowedTypes.Contains(o_fieldInfo.FieldType.GetElementType()?.FullName))) /* if field of parameter object is array and array type is not allowed */
                    {
                        ForestNETLib.Core.Global.ILogFinest("skip array field - type '" + o_fieldInfo.FieldType.GetElementType()?.FullName + "' not allowed");
                        continue;
                    }
                    else if ((!o_fieldInfo.FieldType.IsArray) && (!a_allowedTypes.Contains(o_fieldInfo.FieldType.FullName))) /* if field type of parameter object is not contained in the list of allowed types, skip this field */
                    {
                        ForestNETLib.Core.Global.ILogFinest("skip field - type '" + o_fieldInfo.FieldType.FullName + "' not allowed");
                        continue;
                    }
                    else if ((o_fieldInfo.IsStatic) || (o_fieldInfo.IsLiteral && !o_fieldInfo.IsInitOnly)) /* if field is static or const, skip this field */
                    {
                        ForestNETLib.Core.Global.ILogFinest("skip field - field is static or const");
                        continue;
                    }

                    /* add csv header name and csv delimiter to csv line */
                    s_csv += o_fieldInfo.Name + this.s_delimiter;
                }
            }
            else /* read all properties of object parameter */
            {
                /* iterate all properties of parameter object */
                foreach (System.Reflection.PropertyInfo o_propertyInfo in p_o_object.GetType().GetProperties())
                {
                    /* skip empty property info instance */
                    if (o_propertyInfo == null)
                    {
                        continue;
                    }

                    /* skip empty property type */
                    if (o_propertyInfo.PropertyType == null)
                    {
                        continue;
                    }

                    ForestNETLib.Core.Global.ILogFiner("iterate all properties - property '" + o_propertyInfo.Name + "' with type '" + o_propertyInfo.PropertyType.FullName + "'");

                    /* if property of parameter object is of type list */
                    if ((o_propertyInfo.PropertyType.IsGenericType) && (o_propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(List<>)))
                    {
                        ForestNETLib.Core.Global.ILogFiner("iterate all properties - array element type '" + o_propertyInfo.PropertyType.GenericTypeArguments[0].FullName + "'");

                        /* if type of property list is not contained in the list of allowed types, skip this property */
                        if (!a_allowedTypes.Contains(o_propertyInfo.PropertyType.GenericTypeArguments[0].FullName))
                        {
                            ForestNETLib.Core.Global.ILogFinest("skip generic list property - type '" + o_propertyInfo.PropertyType.GenericTypeArguments[0].FullName + "' not allowed");
                            continue;
                        }
                    }
                    else if ((o_propertyInfo.PropertyType.IsArray) && (!a_allowedTypes.Contains(o_propertyInfo.PropertyType.GetElementType()?.FullName))) /* if property array type of parameter object is not contained in the list of allowed types, skip this field */
                    {
                        ForestNETLib.Core.Global.ILogFinest("skip array property - type '" + o_propertyInfo.PropertyType.GetElementType()?.FullName + "' not allowed");
                        continue;
                    }
                    else if ((!o_propertyInfo.PropertyType.IsArray) && (!a_allowedTypes.Contains(o_propertyInfo.PropertyType.FullName))) /* if property type of parameter object is not contained in the list of allowed types, skip this property */
                    {
                        ForestNETLib.Core.Global.ILogFinest("skip property - type '" + o_propertyInfo.PropertyType.FullName + "' not allowed");
                        continue;
                    }
                    else if (!o_propertyInfo.CanRead) /* if property value cannot be read */
                    {
                        ForestNETLib.Core.Global.ILogFinest("skip property '" + o_propertyInfo.Name + "' - type '" + o_propertyInfo.PropertyType.FullName + "' - cannot read it's value");
                        continue;
                    }

                    /* add csv header name and csv delimiter to csv line */
                    s_csv += o_propertyInfo.Name + this.s_delimiter;
                }
            }

            /* remove last csv delimiter in csv line */
            s_csv = s_csv.Substring(0, s_csv.Length - this.s_delimiter.Length);

            /* return csv line with line break */
            return s_csv + this.s_lineBreak;
        }

        /// <summary>
        /// Encode object to csv data string
        /// supported types:
        /// byte, short, int, long, float, double, decimal, char, boolean, string,
        /// DateTime, TimeSpan, Object, sbyte, ushot, uint, ulong
        /// </summary>
        /// <param name="p_o_object">object parameter to get object field names for header fields</param>
        /// <returns>csv content string</returns>
        /// <exception cref="MemberAccessException">could not retrieve field or property, access violation</exception>
        /// <exception cref="ArgumentException">invalid type parameter</exception>
        private string EncodeObject(Object p_o_object)
        {
            string s_csv = "";

            if (!this.UseProperties) /* read all fields of object parameter */
            {
                s_csv += this.EncodeByFields(p_o_object);
            }
            else /* read all properties of object parameter */
            {
                s_csv += this.EncodeByProperties(p_o_object);
            }

            /* remove last csv delimiter in csv line */
            s_csv = s_csv.Substring(0, s_csv.Length - this.s_delimiter.Length);

            return s_csv;
        }

        /// <summary>
        /// Encode object to csv data string by using it's fields
        /// supported types:
        /// byte, short, int, long, float, double, decimal, char, boolean, string,
        /// DateTime, TimeSpan, Object, sbyte, ushot, uint, ulong
        /// </summary>
        /// <param name="p_o_object">object parameter to get object field names for header fields</param>
        /// <returns>csv content string</returns>
        /// <exception cref="MemberAccessException">could not retrieve field, access violation</exception>
        /// <exception cref="ArgumentException">invalid type parameter</exception>
        private string EncodeByFields(Object p_o_object)
        {
            string s_csv = "";

            /* iterate all fields of parameter object */
            foreach (System.Reflection.FieldInfo o_fieldInfo in p_o_object.GetType().GetFields())
            {
                /* skip empty field info instance */
                if (o_fieldInfo == null)
                {
                    continue;
                }

                /* skip empty field type */
                if (o_fieldInfo.FieldType == null)
                {
                    continue;
                }

                ForestNETLib.Core.Global.ILogFiner("iterate all fields - field '" + o_fieldInfo.Name + "' with type '" + o_fieldInfo.FieldType.FullName + "'");

                /* if field of parameter object is of type list */
                if ((o_fieldInfo.FieldType.IsGenericType) && (o_fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>)))
                {
                    /* if type of field list is not contained in the list of allowed types, skip this field */
                    if (!a_allowedTypes.Contains(o_fieldInfo.FieldType.GenericTypeArguments[0].FullName))
                    {
                        ForestNETLib.Core.Global.ILogFinest("skip generic list field - type '" + o_fieldInfo.FieldType.GenericTypeArguments[0].FullName + "' not allowed");
                        continue;
                    }
                }
                else if ((o_fieldInfo.FieldType.IsArray) && (!a_allowedTypes.Contains(o_fieldInfo.FieldType.GetElementType()?.FullName))) /* if field of parameter object is array and array type is not allowed, skip this field */
                {
                    ForestNETLib.Core.Global.ILogFinest("skip array field - type '" + o_fieldInfo.FieldType.GetElementType()?.FullName + "' not allowed");
                    continue;
                }
                else if ((!o_fieldInfo.FieldType.IsArray) && (!a_allowedTypes.Contains(o_fieldInfo.FieldType.FullName))) /* if field type of parameter object is not contained in the list of allowed types, skip this field */
                {
                    ForestNETLib.Core.Global.ILogFinest("skip field - type '" + o_fieldInfo.FieldType.FullName + "' not allowed");
                    continue;
                }
                else if ((o_fieldInfo.IsStatic) || (o_fieldInfo.IsLiteral && !o_fieldInfo.IsInitOnly)) /* if field is static or const, skip this field */
                {
                    ForestNETLib.Core.Global.ILogFinest("skip field - field is static or const");
                    continue;
                }

                /* help variable for accessing object field */
                Object? o_object = null;

                /* call field directly to get object data values */
                try
                {
                    o_object = o_fieldInfo.GetValue(p_o_object);
                }
                catch (Exception o_exc)
                {
                    throw new MemberAccessException("Access violation for field[" + o_fieldInfo.Name + "], type[" + o_fieldInfo.FieldType.FullName + "]: " + o_exc.ToString());
                }

                /* if help variable got access to object field */
                if (o_object != null)
                {
                    /* if field of parameter object is of type list */
                    if ((o_fieldInfo.FieldType.IsGenericType) && (o_fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>)))
                    {
                        /* cast current field of parameter object as list with unknown generic type */
                        List<Object> a_objects = [.. ((System.Collections.IEnumerable)o_object).Cast<Object>()];

                        /* help variable for storing list values */
                        string s_arrayValues = "";

                        /* only execute if we have more than one array element */
                        if (a_objects.Count > 0)
                        {
                            /* iterate objects in list */
                            for (int i = 0; i < a_objects.Count; i++)
                            {
                                /* encode array object element and add csv array delimiter */
                                s_arrayValues += this.ObjectValueToString(a_objects[i], o_fieldInfo.FieldType?.GenericTypeArguments[0]?.FullName) + this.s_arrayDelimiter;
                            }

                            /* remove last csv array delimiter in array values */
                            s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                        }

                        /* if array values contains csv delimiter or a double quote, we have to escape array values and escape double quotes within */
                        if ((s_arrayValues.Contains(this.s_delimiter)) || (s_arrayValues.Contains('"')))
                        {
                            s_arrayValues = "\"" + s_arrayValues.Replace("\"", "\"\"") + "\"";
                        }

                        /* add array values and csv delimiter as new csv column */
                        s_csv += s_arrayValues + this.s_delimiter;
                    }
                    else if (o_fieldInfo.FieldType.IsArray)
                    {
                        /* handle usual arrays */

                        /* help variable for storing list values */
                        string s_arrayValues = "";

                        /* get array type as string to lower */
                        string s_arrayType = o_fieldInfo.FieldType.GetElementType()?.FullName?.ToLower() ?? "";

                        /* handle allowed array types */
                        if (s_arrayType.Equals("system.boolean"))
                        {
                            /* cast current field of parameter object as array */
                            bool[] a_objects = (bool[])o_object;

                            /* only execute if we have more than one array element */
                            if (a_objects.Length > 0)
                            {
                                /* iterate objects in list */
                                for (int i = 0; i < a_objects.Length; i++)
                                {
                                    /* encode array object element and add csv array delimiter */
                                    s_arrayValues += this.ObjectValueToString(a_objects[i], o_fieldInfo.FieldType.GetElementType()?.FullName) + this.s_arrayDelimiter;
                                }

                                /* remove last csv array delimiter in array values */
                                s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                            }
                        }
                        else if (s_arrayType.Equals("system.byte"))
                        {
                            /* cast current field of parameter object as array */
                            byte[] a_objects = (byte[])o_object;

                            /* only execute if we have more than one array element */
                            if (a_objects.Length > 0)
                            {
                                /* iterate objects in list */
                                for (int i = 0; i < a_objects.Length; i++)
                                {
                                    /* encode array object element and add csv array delimiter */
                                    s_arrayValues += this.ObjectValueToString(a_objects[i], o_fieldInfo.FieldType.GetElementType()?.FullName) + this.s_arrayDelimiter;
                                }

                                /* remove last csv array delimiter in array values */
                                s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                            }
                        }
                        else if (s_arrayType.Equals("system.sbyte"))
                        {
                            /* cast current field of parameter object as array */
                            sbyte[] a_objects = (sbyte[])o_object;

                            /* only execute if we have more than one array element */
                            if (a_objects.Length > 0)
                            {
                                /* iterate objects in list */
                                for (int i = 0; i < a_objects.Length; i++)
                                {
                                    /* encode array object element and add csv array delimiter */
                                    s_arrayValues += this.ObjectValueToString(a_objects[i], o_fieldInfo.FieldType.GetElementType()?.FullName) + this.s_arrayDelimiter;
                                }

                                /* remove last csv array delimiter in array values */
                                s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                            }
                        }
                        else if (s_arrayType.Equals("system.char"))
                        {
                            /* cast current field of parameter object as array */
                            char[] a_objects = (char[])o_object;

                            /* only execute if we have more than one array element */
                            if (a_objects.Length > 0)
                            {
                                /* iterate objects in list */
                                for (int i = 0; i < a_objects.Length; i++)
                                {
                                    /* encode array object element and add csv array delimiter */
                                    s_arrayValues += this.ObjectValueToString(a_objects[i], o_fieldInfo.FieldType.GetElementType()?.FullName) + this.s_arrayDelimiter;
                                }

                                /* remove last csv array delimiter in array values */
                                s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                            }
                        }
                        else if (s_arrayType.Equals("system.single"))
                        {
                            /* cast current field of parameter object as array */
                            float[] a_objects = (float[])o_object;

                            /* only execute if we have more than one array element */
                            if (a_objects.Length > 0)
                            {
                                /* iterate objects in list */
                                for (int i = 0; i < a_objects.Length; i++)
                                {
                                    /* encode array object element and add csv array delimiter */
                                    s_arrayValues += this.ObjectValueToString(a_objects[i], o_fieldInfo.FieldType.GetElementType()?.FullName) + this.s_arrayDelimiter;
                                }

                                /* remove last csv array delimiter in array values */
                                s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                            }
                        }
                        else if (s_arrayType.Equals("system.double"))
                        {
                            /* cast current field of parameter object as array */
                            double[] a_objects = (double[])o_object;

                            /* only execute if we have more than one array element */
                            if (a_objects.Length > 0)
                            {
                                /* iterate objects in list */
                                for (int i = 0; i < a_objects.Length; i++)
                                {
                                    /* encode array object element and add csv array delimiter */
                                    s_arrayValues += this.ObjectValueToString(a_objects[i], o_fieldInfo.FieldType.GetElementType()?.FullName) + this.s_arrayDelimiter;
                                }

                                /* remove last csv array delimiter in array values */
                                s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                            }
                        }
                        else if (s_arrayType.Equals("system.int16"))
                        {
                            /* cast current field of parameter object as array */
                            short[] a_objects = (short[])o_object;

                            /* only execute if we have more than one array element */
                            if (a_objects.Length > 0)
                            {
                                /* iterate objects in list */
                                for (int i = 0; i < a_objects.Length; i++)
                                {
                                    /* encode array object element and add csv array delimiter */
                                    s_arrayValues += this.ObjectValueToString(a_objects[i], o_fieldInfo.FieldType.GetElementType()?.FullName) + this.s_arrayDelimiter;
                                }

                                /* remove last csv array delimiter in array values */
                                s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                            }
                        }
                        else if (s_arrayType.Equals("system.int32"))
                        {
                            /* cast current field of parameter object as array */
                            int[] a_objects = (int[])o_object;

                            /* only execute if we have more than one array element */
                            if (a_objects.Length > 0)
                            {
                                /* iterate objects in list */
                                for (int i = 0; i < a_objects.Length; i++)
                                {
                                    /* encode array object element and add csv array delimiter */
                                    s_arrayValues += this.ObjectValueToString(a_objects[i], o_fieldInfo.FieldType.GetElementType()?.FullName) + this.s_arrayDelimiter;
                                }

                                /* remove last csv array delimiter in array values */
                                s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                            }
                        }
                        else if (s_arrayType.Equals("system.int64"))
                        {
                            /* cast current field of parameter object as array */
                            long[] a_objects = (long[])o_object;

                            /* only execute if we have more than one array element */
                            if (a_objects.Length > 0)
                            {
                                /* iterate objects in list */
                                for (int i = 0; i < a_objects.Length; i++)
                                {
                                    /* encode array object element and add csv array delimiter */
                                    s_arrayValues += this.ObjectValueToString(a_objects[i], o_fieldInfo.FieldType.GetElementType()?.FullName) + this.s_arrayDelimiter;
                                }

                                /* remove last csv array delimiter in array values */
                                s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                            }
                        }
                        else if (s_arrayType.Equals("system.uint16"))
                        {
                            /* cast current field of parameter object as array */
                            ushort[] a_objects = (ushort[])o_object;

                            /* only execute if we have more than one array element */
                            if (a_objects.Length > 0)
                            {
                                /* iterate objects in list */
                                for (int i = 0; i < a_objects.Length; i++)
                                {
                                    /* encode array object element and add csv array delimiter */
                                    s_arrayValues += this.ObjectValueToString(a_objects[i], o_fieldInfo.FieldType.GetElementType()?.FullName) + this.s_arrayDelimiter;
                                }

                                /* remove last csv array delimiter in array values */
                                s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                            }
                        }
                        else if (s_arrayType.Equals("system.uint32"))
                        {
                            /* cast current field of parameter object as array */
                            uint[] a_objects = (uint[])o_object;

                            /* only execute if we have more than one array element */
                            if (a_objects.Length > 0)
                            {
                                /* iterate objects in list */
                                for (int i = 0; i < a_objects.Length; i++)
                                {
                                    /* encode array object element and add csv array delimiter */
                                    s_arrayValues += this.ObjectValueToString(a_objects[i], o_fieldInfo.FieldType.GetElementType()?.FullName) + this.s_arrayDelimiter;
                                }

                                /* remove last csv array delimiter in array values */
                                s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                            }
                        }
                        else if (s_arrayType.Equals("system.uint64"))
                        {
                            /* cast current field of parameter object as array */
                            ulong[] a_objects = (ulong[])o_object;

                            /* only execute if we have more than one array element */
                            if (a_objects.Length > 0)
                            {
                                /* iterate objects in list */
                                for (int i = 0; i < a_objects.Length; i++)
                                {
                                    /* encode array object element and add csv array delimiter */
                                    s_arrayValues += this.ObjectValueToString(a_objects[i], o_fieldInfo.FieldType.GetElementType()?.FullName) + this.s_arrayDelimiter;
                                }

                                /* remove last csv array delimiter in array values */
                                s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                            }
                        }
                        else if (s_arrayType.Equals("system.string"))
                        {
                            /* cast current field of parameter object as array */
                            string[] a_objects = (string[])o_object;

                            /* only execute if we have more than one array element */
                            if (a_objects.Length > 0)
                            {
                                /* iterate objects in list */
                                for (int i = 0; i < a_objects.Length; i++)
                                {
                                    /* encode array object element and add csv array delimiter */
                                    s_arrayValues += this.ObjectValueToString(a_objects[i], o_fieldInfo.FieldType.GetElementType()?.FullName) + this.s_arrayDelimiter;
                                }

                                /* remove last csv array delimiter in array values */
                                s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                            }
                        }
                        else if (s_arrayType.Equals("system.datetime"))
                        {
                            /* cast current field of parameter object as array */
                            System.DateTime[] a_objects = (System.DateTime[])o_object;

                            /* only execute if we have more than one array element */
                            if (a_objects.Length > 0)
                            {
                                /* iterate objects in list */
                                for (int i = 0; i < a_objects.Length; i++)
                                {
                                    /* encode array object element and add csv array delimiter */
                                    s_arrayValues += this.ObjectValueToString(a_objects[i], o_fieldInfo.FieldType.GetElementType()?.FullName) + this.s_arrayDelimiter;
                                }

                                /* remove last csv array delimiter in array values */
                                s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                            }
                        }
                        else if (s_arrayType.Equals("system.timespan"))
                        {
                            /* cast current field of parameter object as array */
                            System.TimeSpan[] a_objects = (System.TimeSpan[])o_object;

                            /* only execute if we have more than one array element */
                            if (a_objects.Length > 0)
                            {
                                /* iterate objects in list */
                                for (int i = 0; i < a_objects.Length; i++)
                                {
                                    /* encode array object element and add csv array delimiter */
                                    s_arrayValues += this.ObjectValueToString(a_objects[i], o_fieldInfo.FieldType.GetElementType()?.FullName) + this.s_arrayDelimiter;
                                }

                                /* remove last csv array delimiter in array values */
                                s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                            }
                        }
                        else if (s_arrayType.Equals("system.decimal"))
                        {
                            /* cast current field of parameter object as array */
                            System.Decimal[] a_objects = (System.Decimal[])o_object;

                            /* only execute if we have more than one array element */
                            if (a_objects.Length > 0)
                            {
                                /* iterate objects in list */
                                for (int i = 0; i < a_objects.Length; i++)
                                {
                                    /* encode array object element and add csv array delimiter */
                                    s_arrayValues += this.ObjectValueToString(a_objects[i], o_fieldInfo.FieldType.GetElementType()?.FullName) + this.s_arrayDelimiter;
                                }

                                /* remove last csv array delimiter in array values */
                                s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                            }
                        }
                        else if (s_arrayType.Equals("system.object"))
                        {
                            /* cast current field of parameter object as array */
                            System.Object[] a_objects = (System.Object[])o_object;

                            /* only execute if we have more than one array element */
                            if (a_objects.Length > 0)
                            {
                                /* iterate objects in list */
                                for (int i = 0; i < a_objects.Length; i++)
                                {
                                    /* encode array object element and add csv array delimiter */
                                    s_arrayValues += this.ObjectValueToString(a_objects[i], o_fieldInfo.FieldType.GetElementType()?.FullName) + this.s_arrayDelimiter;
                                }

                                /* remove last csv array delimiter in array values */
                                s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                            }
                        }

                        /* if array values contains csv delimiter or a double quote, we have to escape array values and escape double quotes within */
                        if ((s_arrayValues.Contains(this.s_delimiter)) || (s_arrayValues.Contains('"')))
                        {
                            s_arrayValues = "\"" + s_arrayValues.Replace("\"", "\"\"") + "\"";
                        }

                        /* add array values and csv delimiter as new csv column */
                        s_csv += s_arrayValues + this.s_delimiter;
                    }
                    else
                    {
                        /* add object value and csv delimiter as new csv column */
                        s_csv += this.ObjectValueToString(o_object, o_fieldInfo.FieldType.FullName) + this.s_delimiter;
                    }
                }
                else
                {
                    /* add empty value and csv delimiter as new csv column */
                    s_csv += this.s_delimiter;
                }
            }

            return s_csv;
        }

        /// <summary>
        /// Encode object to csv data string by using it's properties
        /// supported types:
        /// byte, short, int, long, float, double, decimal, char, boolean, string,
        /// DateTime, TimeSpan, Object, sbyte, ushot, uint, ulong
        /// </summary>
        /// <param name="p_o_object">object parameter to get object field names for header fields</param>
        /// <returns>csv content string</returns>
        /// <exception cref="MemberAccessException">could not retrieve property, access violation</exception>
        /// <exception cref="ArgumentException">invalid type parameter</exception>
        private string EncodeByProperties(Object p_o_object)
        {
            string s_csv = "";

            /* iterate all properties of parameter object */
            foreach (System.Reflection.PropertyInfo o_propertyInfo in p_o_object.GetType().GetProperties())
            {
                /* skip empty property info instance */
                if (o_propertyInfo == null)
                {
                    continue;
                }

                /* skip empty property type */
                if (o_propertyInfo.PropertyType == null)
                {
                    continue;
                }

                ForestNETLib.Core.Global.ILogFiner("iterate all properties - property '" + o_propertyInfo.Name + "' with type '" + o_propertyInfo.PropertyType.FullName + "'");

                /* if property of parameter object is of type list */
                if ((o_propertyInfo.PropertyType.IsGenericType) && (o_propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(List<>)))
                {
                    ForestNETLib.Core.Global.ILogFiner("iterate all properties - array element type '" + o_propertyInfo.PropertyType.GenericTypeArguments[0].FullName + "'");

                    /* if type of property list is not contained in the list of allowed types, skip this property */
                    if (!a_allowedTypes.Contains(o_propertyInfo.PropertyType.GenericTypeArguments[0].FullName))
                    {
                        ForestNETLib.Core.Global.ILogFinest("skip generic list property - type '" + o_propertyInfo.PropertyType.GenericTypeArguments[0].FullName + "' not allowed");
                        continue;
                    }
                }
                else if ((o_propertyInfo.PropertyType.IsArray) && (!a_allowedTypes.Contains(o_propertyInfo.PropertyType.GetElementType()?.FullName))) /* if property array type of parameter object is not contained in the list of allowed types, skip this field */
                {
                    ForestNETLib.Core.Global.ILogFinest("skip array property - type '" + o_propertyInfo.PropertyType.GetElementType()?.FullName + "' not allowed");
                    continue;
                }
                else if ((!o_propertyInfo.PropertyType.IsArray) && (!a_allowedTypes.Contains(o_propertyInfo.PropertyType.FullName))) /* if property type of parameter object is not contained in the list of allowed types, skip this property */
                {
                    ForestNETLib.Core.Global.ILogFinest("skip property - type '" + o_propertyInfo.PropertyType.FullName + "' not allowed");
                    continue;
                }
                else if (!o_propertyInfo.CanRead) /* if property value cannot be read */
                {
                    ForestNETLib.Core.Global.ILogFinest("skip property '" + o_propertyInfo.Name + "' - type '" + o_propertyInfo.PropertyType.FullName + "' - cannot read it's value");
                    continue;
                }

                /* help variable for accessing object field */
                Object? o_object = null;

                /* call property directly to get object data values */
                try
                {
                    o_object = o_propertyInfo.GetValue(p_o_object);
                }
                catch (Exception o_exc)
                {
                    throw new MemberAccessException("Access violation for property[" + o_propertyInfo.Name + "], type[" + o_propertyInfo.PropertyType.FullName + "]: " + o_exc.ToString());
                }

                /* if help variable got access to object field */
                if (o_object != null)
                {
                    /* if property of parameter object is of type list */
                    if ((o_propertyInfo.PropertyType.IsGenericType) && (o_propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(List<>)))
                    {
                        /* cast current field of parameter object as list with unknown generic type */
                        List<Object> a_objects = [.. ((System.Collections.IEnumerable)o_object).Cast<Object>()];

                        /* help variable for storing list values */
                        string s_arrayValues = "";

                        /* only execute if we have more than one array element */
                        if (a_objects.Count > 0)
                        {
                            /* iterate objects in list */
                            for (int i = 0; i < a_objects.Count; i++)
                            {
                                /* encode array object element and add csv array delimiter */
                                s_arrayValues += this.ObjectValueToString(a_objects[i], o_propertyInfo.PropertyType.GenericTypeArguments[0].FullName) + this.s_arrayDelimiter;
                            }

                            /* remove last csv array delimiter in array values */
                            s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                        }

                        /* if array values contains csv delimiter or a double quote, we have to escape array values and escape double quotes within */
                        if ((s_arrayValues.Contains(this.s_delimiter)) || (s_arrayValues.Contains('"')))
                        {
                            s_arrayValues = "\"" + s_arrayValues.Replace("\"", "\"\"") + "\"";
                        }

                        /* add array values and csv delimiter as new csv column */
                        s_csv += s_arrayValues + this.s_delimiter;
                    }
                    else if (o_propertyInfo.PropertyType.IsArray)
                    {
                        /* handle usual arrays */

                        /* help variable for storing list values */
                        string s_arrayValues = "";

                        /* get array type as string to lower */
                        string s_arrayType = o_propertyInfo.PropertyType.GetElementType()?.FullName?.ToLower() ?? "";

                        /* handle allowed array types */
                        if (s_arrayType.Equals("system.boolean"))
                        {
                            /* cast current field of parameter object as array */
                            bool[] a_objects = (bool[])o_object;

                            /* only execute if we have more than one array element */
                            if (a_objects.Length > 0)
                            {
                                /* iterate objects in list */
                                for (int i = 0; i < a_objects.Length; i++)
                                {
                                    /* encode array object element and add csv array delimiter */
                                    s_arrayValues += this.ObjectValueToString(a_objects[i], o_propertyInfo.PropertyType.GetElementType()?.FullName) + this.s_arrayDelimiter;
                                }

                                /* remove last csv array delimiter in array values */
                                s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                            }
                        }
                        else if (s_arrayType.Equals("system.byte"))
                        {
                            /* cast current field of parameter object as array */
                            byte[] a_objects = (byte[])o_object;

                            /* only execute if we have more than one array element */
                            if (a_objects.Length > 0)
                            {
                                /* iterate objects in list */
                                for (int i = 0; i < a_objects.Length; i++)
                                {
                                    /* encode array object element and add csv array delimiter */
                                    s_arrayValues += this.ObjectValueToString(a_objects[i], o_propertyInfo.PropertyType.GetElementType()?.FullName) + this.s_arrayDelimiter;
                                }

                                /* remove last csv array delimiter in array values */
                                s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                            }
                        }
                        else if (s_arrayType.Equals("system.sbyte"))
                        {
                            /* cast current field of parameter object as array */
                            sbyte[] a_objects = (sbyte[])o_object;

                            /* only execute if we have more than one array element */
                            if (a_objects.Length > 0)
                            {
                                /* iterate objects in list */
                                for (int i = 0; i < a_objects.Length; i++)
                                {
                                    /* encode array object element and add csv array delimiter */
                                    s_arrayValues += this.ObjectValueToString(a_objects[i], o_propertyInfo.PropertyType.GetElementType()?.FullName) + this.s_arrayDelimiter;
                                }

                                /* remove last csv array delimiter in array values */
                                s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                            }
                        }
                        else if (s_arrayType.Equals("system.char"))
                        {
                            /* cast current field of parameter object as array */
                            char[] a_objects = (char[])o_object;

                            /* only execute if we have more than one array element */
                            if (a_objects.Length > 0)
                            {
                                /* iterate objects in list */
                                for (int i = 0; i < a_objects.Length; i++)
                                {
                                    /* encode array object element and add csv array delimiter */
                                    s_arrayValues += this.ObjectValueToString(a_objects[i], o_propertyInfo.PropertyType.GetElementType()?.FullName) + this.s_arrayDelimiter;
                                }

                                /* remove last csv array delimiter in array values */
                                s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                            }
                        }
                        else if (s_arrayType.Equals("system.single"))
                        {
                            /* cast current field of parameter object as array */
                            float[] a_objects = (float[])o_object;

                            /* only execute if we have more than one array element */
                            if (a_objects.Length > 0)
                            {
                                /* iterate objects in list */
                                for (int i = 0; i < a_objects.Length; i++)
                                {
                                    /* encode array object element and add csv array delimiter */
                                    s_arrayValues += this.ObjectValueToString(a_objects[i], o_propertyInfo.PropertyType.GetElementType()?.FullName) + this.s_arrayDelimiter;
                                }

                                /* remove last csv array delimiter in array values */
                                s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                            }
                        }
                        else if (s_arrayType.Equals("system.double"))
                        {
                            /* cast current field of parameter object as array */
                            double[] a_objects = (double[])o_object;

                            /* only execute if we have more than one array element */
                            if (a_objects.Length > 0)
                            {
                                /* iterate objects in list */
                                for (int i = 0; i < a_objects.Length; i++)
                                {
                                    /* encode array object element and add csv array delimiter */
                                    s_arrayValues += this.ObjectValueToString(a_objects[i], o_propertyInfo.PropertyType.GetElementType()?.FullName) + this.s_arrayDelimiter;
                                }

                                /* remove last csv array delimiter in array values */
                                s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                            }
                        }
                        else if (s_arrayType.Equals("system.int16"))
                        {
                            /* cast current field of parameter object as array */
                            short[] a_objects = (short[])o_object;

                            /* only execute if we have more than one array element */
                            if (a_objects.Length > 0)
                            {
                                /* iterate objects in list */
                                for (int i = 0; i < a_objects.Length; i++)
                                {
                                    /* encode array object element and add csv array delimiter */
                                    s_arrayValues += this.ObjectValueToString(a_objects[i], o_propertyInfo.PropertyType.GetElementType()?.FullName) + this.s_arrayDelimiter;
                                }

                                /* remove last csv array delimiter in array values */
                                s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                            }
                        }
                        else if (s_arrayType.Equals("system.int32"))
                        {
                            /* cast current field of parameter object as array */
                            int[] a_objects = (int[])o_object;

                            /* only execute if we have more than one array element */
                            if (a_objects.Length > 0)
                            {
                                /* iterate objects in list */
                                for (int i = 0; i < a_objects.Length; i++)
                                {
                                    /* encode array object element and add csv array delimiter */
                                    s_arrayValues += this.ObjectValueToString(a_objects[i], o_propertyInfo.PropertyType.GetElementType()?.FullName) + this.s_arrayDelimiter;
                                }

                                /* remove last csv array delimiter in array values */
                                s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                            }
                        }
                        else if (s_arrayType.Equals("system.int64"))
                        {
                            /* cast current field of parameter object as array */
                            long[] a_objects = (long[])o_object;

                            /* only execute if we have more than one array element */
                            if (a_objects.Length > 0)
                            {
                                /* iterate objects in list */
                                for (int i = 0; i < a_objects.Length; i++)
                                {
                                    /* encode array object element and add csv array delimiter */
                                    s_arrayValues += this.ObjectValueToString(a_objects[i], o_propertyInfo.PropertyType.GetElementType()?.FullName) + this.s_arrayDelimiter;
                                }

                                /* remove last csv array delimiter in array values */
                                s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                            }
                        }
                        else if (s_arrayType.Equals("system.uint16"))
                        {
                            /* cast current field of parameter object as array */
                            ushort[] a_objects = (ushort[])o_object;

                            /* only execute if we have more than one array element */
                            if (a_objects.Length > 0)
                            {
                                /* iterate objects in list */
                                for (int i = 0; i < a_objects.Length; i++)
                                {
                                    /* encode array object element and add csv array delimiter */
                                    s_arrayValues += this.ObjectValueToString(a_objects[i], o_propertyInfo.PropertyType.GetElementType()?.FullName) + this.s_arrayDelimiter;
                                }

                                /* remove last csv array delimiter in array values */
                                s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                            }
                        }
                        else if (s_arrayType.Equals("system.uint32"))
                        {
                            /* cast current field of parameter object as array */
                            uint[] a_objects = (uint[])o_object;

                            /* only execute if we have more than one array element */
                            if (a_objects.Length > 0)
                            {
                                /* iterate objects in list */
                                for (int i = 0; i < a_objects.Length; i++)
                                {
                                    /* encode array object element and add csv array delimiter */
                                    s_arrayValues += this.ObjectValueToString(a_objects[i], o_propertyInfo.PropertyType.GetElementType()?.FullName) + this.s_arrayDelimiter;
                                }

                                /* remove last csv array delimiter in array values */
                                s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                            }
                        }
                        else if (s_arrayType.Equals("system.uint64"))
                        {
                            /* cast current field of parameter object as array */
                            ulong[] a_objects = (ulong[])o_object;

                            /* only execute if we have more than one array element */
                            if (a_objects.Length > 0)
                            {
                                /* iterate objects in list */
                                for (int i = 0; i < a_objects.Length; i++)
                                {
                                    /* encode array object element and add csv array delimiter */
                                    s_arrayValues += this.ObjectValueToString(a_objects[i], o_propertyInfo.PropertyType.GetElementType()?.FullName) + this.s_arrayDelimiter;
                                }

                                /* remove last csv array delimiter in array values */
                                s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                            }
                        }
                        else if (s_arrayType.Equals("system.string"))
                        {
                            /* cast current field of parameter object as array */
                            string[] a_objects = (string[])o_object;

                            /* only execute if we have more than one array element */
                            if (a_objects.Length > 0)
                            {
                                /* iterate objects in list */
                                for (int i = 0; i < a_objects.Length; i++)
                                {
                                    /* encode array object element and add csv array delimiter */
                                    s_arrayValues += this.ObjectValueToString(a_objects[i], o_propertyInfo.PropertyType.GetElementType()?.FullName) + this.s_arrayDelimiter;
                                }

                                /* remove last csv array delimiter in array values */
                                s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                            }
                        }
                        else if (s_arrayType.Equals("system.datetime"))
                        {
                            /* cast current field of parameter object as array */
                            System.DateTime[] a_objects = (System.DateTime[])o_object;

                            /* only execute if we have more than one array element */
                            if (a_objects.Length > 0)
                            {
                                /* iterate objects in list */
                                for (int i = 0; i < a_objects.Length; i++)
                                {
                                    /* encode array object element and add csv array delimiter */
                                    s_arrayValues += this.ObjectValueToString(a_objects[i], o_propertyInfo.PropertyType.GetElementType()?.FullName) + this.s_arrayDelimiter;
                                }

                                /* remove last csv array delimiter in array values */
                                s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                            }
                        }
                        else if (s_arrayType.Equals("system.timespan"))
                        {
                            /* cast current field of parameter object as array */
                            System.TimeSpan[] a_objects = (System.TimeSpan[])o_object;

                            /* only execute if we have more than one array element */
                            if (a_objects.Length > 0)
                            {
                                /* iterate objects in list */
                                for (int i = 0; i < a_objects.Length; i++)
                                {
                                    /* encode array object element and add csv array delimiter */
                                    s_arrayValues += this.ObjectValueToString(a_objects[i], o_propertyInfo.PropertyType.GetElementType()?.FullName) + this.s_arrayDelimiter;
                                }

                                /* remove last csv array delimiter in array values */
                                s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                            }
                        }
                        else if (s_arrayType.Equals("system.decimal"))
                        {
                            /* cast current field of parameter object as array */
                            System.Decimal[] a_objects = (System.Decimal[])o_object;

                            /* only execute if we have more than one array element */
                            if (a_objects.Length > 0)
                            {
                                /* iterate objects in list */
                                for (int i = 0; i < a_objects.Length; i++)
                                {
                                    /* encode array object element and add csv array delimiter */
                                    s_arrayValues += this.ObjectValueToString(a_objects[i], o_propertyInfo.PropertyType.GetElementType()?.FullName) + this.s_arrayDelimiter;
                                }

                                /* remove last csv array delimiter in array values */
                                s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                            }
                        }
                        else if (s_arrayType.Equals("system.object"))
                        {
                            /* cast current field of parameter object as array */
                            System.Object[] a_objects = (System.Object[])o_object;

                            /* only execute if we have more than one array element */
                            if (a_objects.Length > 0)
                            {
                                /* iterate objects in list */
                                for (int i = 0; i < a_objects.Length; i++)
                                {
                                    /* encode array object element and add csv array delimiter */
                                    s_arrayValues += this.ObjectValueToString(a_objects[i], o_propertyInfo.PropertyType.GetElementType()?.FullName) + this.s_arrayDelimiter;
                                }

                                /* remove last csv array delimiter in array values */
                                s_arrayValues = s_arrayValues.Substring(0, s_arrayValues.Length - this.s_arrayDelimiter.Length);
                            }
                        }

                        /* if array values contains csv delimiter or a double quote, we have to escape array values and escape double quotes within */
                        if ((s_arrayValues.Contains(this.s_delimiter)) || (s_arrayValues.Contains('"')))
                        {
                            s_arrayValues = "\"" + s_arrayValues.Replace("\"", "\"\"") + "\"";
                        }

                        /* add array values and csv delimiter as new csv column */
                        s_csv += s_arrayValues + this.s_delimiter;
                    }
                    else
                    {
                        /* add object value and csv delimiter as new csv column */
                        s_csv += this.ObjectValueToString(o_object, o_propertyInfo.PropertyType.FullName) + this.s_delimiter;
                    }
                }
                else
                {
                    /* add empty value and csv delimiter as new csv column */
                    s_csv += this.s_delimiter;
                }
            }

            return s_csv;
        }

        /// <summary>
        /// Encode object value to a string value so it can be appended to csv data string
        /// escaping double quotes within a value
        /// supported types:
        /// byte, short, int, long, float, double, decimal, char, boolean, string,
        /// DateTime, TimeSpan, Object, sbyte, ushot, uint, ulong
        /// </summary>
        /// <param name="p_o_object">object parameter to encode value</param>
        /// <param name="p_s_type">object parameter type as string value which can be null</param>
        /// <returns>string</returns>
        /// <exception cref="ArgumentException">invalid type parameter</exception>
        private string ObjectValueToString(Object p_o_object, string? p_s_type)
        {
            string s_foo = "";

            if ((p_o_object != null) && (p_s_type != null))
            {
                /* transpose primitive type to class type */
                switch (p_s_type.ToLower())
                {
                    case "byte":
                        p_s_type = "System.Byte";
                        break;
                    case "short":
                        p_s_type = "System.Int16";
                        break;
                    case "int":
                        p_s_type = "System.Int32";
                        break;
                    case "long":
                        p_s_type = "System.Int64";
                        break;
                    case "float":
                        p_s_type = "System.Single";
                        break;
                    case "double":
                        p_s_type = "System.Double";
                        break;
                    case "bool":
                        p_s_type = "System.Boolean";
                        break;
                    case "char":
                        p_s_type = "System.Char";
                        break;
                    case "string":
                        p_s_type = "System.String";
                        break;
                    case "date":
                    case "datetime":
                        p_s_type = "System.DateTime";
                        break;
                    case "time":
                        p_s_type = "System.TimeSpan";
                        break;
                    case "decimal":
                        p_s_type = "System.Decimal";
                        break;
                    case "sbyte":
                        p_s_type = "System.SByte";
                        break;
                    case "ushort":
                        p_s_type = "System.UInt16";
                        break;
                    case "uint":
                        p_s_type = "System.UInt32";
                        break;
                    case "ulong":
                        p_s_type = "System.UInt64";
                        break;
                    case "object":
                        p_s_type = "System.Object";
                        break;
                }

                /* cast object data to string value by class type */
                if (p_s_type.Equals("System.String"))
                {
                    string? s_fooo = p_o_object.ToString();

                    if (s_fooo != null)
                    {
                        s_foo = s_fooo;
                    }
                }
                else if (p_s_type.Equals("System.Byte"))
                {
                    System.Byte o_foo = Convert.ToByte(p_o_object);
                    s_foo = o_foo.ToString();
                }
                else if (p_s_type.Equals("System.Int16"))
                {
                    short o_foo = Convert.ToInt16(p_o_object);
                    s_foo = o_foo.ToString();
                }
                else if (p_s_type.Equals("System.Int32"))
                {
                    int o_foo = Convert.ToInt32(p_o_object);
                    s_foo = o_foo.ToString();
                }
                else if (p_s_type.Equals("System.Int64"))
                {
                    long o_foo = Convert.ToInt64(p_o_object);
                    s_foo = o_foo.ToString();
                }
                else if (p_s_type.Equals("System.Single"))
                {
                    float o_foo = Convert.ToSingle(p_o_object);
                    s_foo = o_foo.ToString("0.000000000", new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                }
                else if (p_s_type.Equals("System.Double"))
                {
                    double o_foo = Convert.ToDouble(p_o_object);
                    s_foo = o_foo.ToString("0.00000000000000000", new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                }
                else if (p_s_type.Equals("System.Boolean"))
                {
                    bool o_foo = Convert.ToBoolean(p_o_object);
                    s_foo = o_foo.ToString();
                }
                else if (p_s_type.Equals("System.Char"))
                {
                    char o_foo = Convert.ToChar(p_o_object);
                    s_foo = o_foo.ToString();
                }
                else if (p_s_type.Equals("System.Decimal"))
                {
                    decimal o_foo = Convert.ToDecimal(p_o_object);
                    s_foo = o_foo.ToString("0.00000000000000000000000000000", new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                }
                else if (p_s_type.Equals("System.DateTime"))
                {
                    DateTime o_foo = Convert.ToDateTime(p_o_object);
                    s_foo = ForestNETLib.Core.Helper.ToISO8601UTC(o_foo);
                }
                else if (p_s_type.Equals("System.TimeSpan"))
                {
                    DateTime o_foo = new DateTime(1900, 1, 1).Date + (TimeSpan)p_o_object;
                    s_foo = ForestNETLib.Core.Helper.ToISO8601UTC(o_foo);
                }
                else if (p_s_type.Equals("System.SByte"))
                {
                    sbyte o_foo = Convert.ToSByte(p_o_object);
                    s_foo = o_foo.ToString();
                }
                else if (p_s_type.Equals("System.UInt16"))
                {
                    ushort o_foo = Convert.ToUInt16(p_o_object);
                    s_foo = o_foo.ToString();
                }
                else if (p_s_type.Equals("System.UInt32"))
                {
                    uint o_foo = Convert.ToUInt32(p_o_object);
                    s_foo = o_foo.ToString();
                }
                else if (p_s_type.Equals("System.UInt64"))
                {
                    ulong o_foo = Convert.ToUInt64(p_o_object);
                    s_foo = o_foo.ToString();
                }
                else if (p_s_type.Equals("System.Object"))
                {
                    string? s_fooo = p_o_object.ToString();

                    if (s_fooo != null)
                    {
                        s_foo = s_fooo;
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid type[" + p_s_type + "] for " + p_o_object.GetType().FullName);
                }
            }

            /* if value contains csv delimiter or a double quote, we have to escape the value and escape double quotes within */
            if ((s_foo.Contains(this.s_delimiter)) || (s_foo.Contains('"')))
            {
                if (s_foo.Contains('"'))
                {
                    s_foo = "\"" + s_foo.Replace("\"", "\"\"") + "\"";
                }
                else
                {
                    s_foo = "\"" + s_foo + "\"";
                }
            }

            return s_foo;
        }

        /* decoding csv to data */

        /// <summary>
        /// Decode csv file to an object given as parameter
        /// supported types:
        /// byte, short, int, long, float, double, decimal, char, boolean, string,
        /// DateTime, TimeSpan, Object, sbyte, ushot, uint, ulong
        /// </summary>
        /// <param name="p_s_csvFile">full path to csv file</param>
        /// <param name="p_o_object">object parameter where csv data will be decoded and cast into object fields</param>
        /// <exception cref="ArgumentException">Invalid object or csv file path parameters, invalid type parameter, or invalid value length for conversion</exception>
        /// <exception cref="System.IO.IOException">exception reading csv file</exception>
        /// <exception cref="FormatException">exception parsing LocalDateTime, LocalDate or LocalTime value</exception>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="ArgumentNullException">array element class could not be found</exception>
        public void DecodeCSV<T>(string p_s_csvFile, ref T p_o_object)
        {
            /* check if parameter object has a value */
            if (p_o_object == null)
            {
                throw new ArgumentException("Cannot decode csv file with empty object = null");
            }

            /* check if file exists */
            if (!File.Exists(p_s_csvFile))
            {
                throw new ArgumentException("CSV file[" + p_s_csvFile + "] does not exist.");
            }

            /* open csv file */
            ForestNETLib.IO.File o_file = new(p_s_csvFile, this.o_encoding, false, ForestNETLib.IO.File.NEWLINE);

            /* read all csv file lines */
            string s_csv = o_file.FileContent;

            /* validate csv file content */
            this.ValidateCSV(s_csv);

            /* decode csv file content */
            this.DecodeCSVFile(s_csv, ref p_o_object);
        }

        /// <summary>
        /// Check content of csv file if all delimiters and quotes are logical right and can be read for decoding
        /// </summary>
        /// <param name="p_s_csv">string content of a csv file</param>
        /// <exception cref="InvalidOperationException">delimiter is not correct</exception>
        private void ValidateCSV(string p_s_csv)
        {
            /* state variable if we are currently in a double quoted value or not */
            bool b_doubleQuoteActive = false;

            /* help variable if an escaped double quote is at start or end of the column */
            bool b_tripleDoubleQuote = false;

            /* iterate csv for each character */
            for (int i = 0; i < p_s_csv.Length; i++)
            {
                /* if we found a not unescaped double quote */
                if (
                    ((i == 0) && (i != p_s_csv.Length - 1) && (p_s_csv[i] == '"') && (p_s_csv[i + 1] != '"')) ||
                    (
                        ((i != 0) && (p_s_csv[i] == '"') && (p_s_csv[i - 1] != '"')) &&
                        ((i != p_s_csv.Length - 1) && (p_s_csv[i] == '"') && (p_s_csv[i + 1] != '"'))
                    ) ||
                    (b_tripleDoubleQuote)
                )
                {
                    /* if we are at the end of a double quoted value */
                    if (b_doubleQuoteActive)
                    {
                        /* unset state */
                        b_doubleQuoteActive = false;

                        /* check if after a double quoted value we have a new delimiter */
                        if (p_s_csv[i + 1] != this.s_delimiter[0])
                        {
                            throw new InvalidOperationException("Expected '" + this.s_delimiter + "' delimiter, but found '" + p_s_csv[i + 1] + "' after a double quoted value '\"'");
                        }
                    }
                    else
                    {
                        /* we have a new double quoted value incoming, set state */
                        b_doubleQuoteActive = true;
                    }

                    /* unset flag */
                    if (b_tripleDoubleQuote)
                    {
                        b_tripleDoubleQuote = false;
                    }
                }

                /* set flag if an escaped double quote is at start or end of the column */
                if ((i != 0) && (i != p_s_csv.Length - 1) && (p_s_csv[i - 1] == '"') && (p_s_csv[i] == '"') && (p_s_csv[i + 1] == '"'))
                {
                    b_tripleDoubleQuote = true;
                }
            }
        }

        /// <summary>
        /// Decode csv string content to an object given as parameter
        /// supported types:
        /// byte, short, int, long, float, double, decimal, char, boolean, string,
        /// DateTime, TimeSpan, Object, sbyte, ushot, uint, ulong
        /// </summary>
        /// <param name="p_s_csvContent">content of a csv file</param>
        /// <param name="p_o_object">object parameter where csv data will be decoded and cast into object fields</param>
        /// <exception cref="NullReferenceException">cannot decode to csv file if parameter object is 'null' or other instance which must be used has a null value</exception>
        /// <exception cref="ArgumentException">invalid type parameter, or invalid value length for conversion</exception>
        /// <exception cref="FormatException">exception parsing LocalDateTime, LocalDate or LocalTime value</exception>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="ArgumentNullException">array element class could not be found</exception>
        private void DecodeCSVFile<T>(string p_s_csvContent, ref T p_o_object)
        {
            /* check if parameter object is not null */
            if (p_o_object == null)
            {
                throw new NullReferenceException("cannot decode to csv file if parameter object is 'null'");
            }

            /* flag for handling generic list */
            bool b_genericList = false;

            /* variable to hold generic list type */
            Type? o_elementType = null;

            /* check if we want to handle a generic list and parameter object is a generic list */
            if ((p_o_object.GetType().IsGenericType) && (p_o_object.GetType().GetGenericTypeDefinition() == typeof(List<>)))
            {
                /* store generic list type */
                o_elementType = p_o_object.GetType().GenericTypeArguments[0];
                /* set flag */
                b_genericList = true;
            }

            /* split csv file content by line break into multiple lines and save it into a string array */
            string[] a_csvLines = p_s_csvContent.Split(this.s_lineBreak);

            /* check if csv file content is not empty */
            if (a_csvLines.Length > 0)
            {
                /* help variable for starting decoding csv file lines */
                int i_start = 0;

                /* header line */
                string s_headerLine;

                /* if parameter array element class is set, generate header line based on this class */
                if (b_genericList)
                {
                    /* check if element type is not null */
                    if (o_elementType == null)
                    {
                        throw new NullReferenceException("Could not create instance by element type 'null'");
                    }

                    /* create object instance */
                    object? o_foo = Activator.CreateInstance(o_elementType) ?? throw new NullReferenceException("Could not create instance by element type '" + o_elementType?.FullName + "'");

                    /* generate header line with class declared constructor instance */
                    s_headerLine = this.GenerateHeaderLine(o_foo).Replace(this.s_lineBreak, "");
                }
                else
                {
                    /* generate header line out of parameter object */
                    s_headerLine = this.GenerateHeaderLine(p_o_object).Replace(this.s_lineBreak, "");
                }

                /* retrieve field names by splitting header line with csv delimiter */
                string[] a_fieldNames = s_headerLine.Split(this.s_delimiter);

                ForestNETLib.Core.Global.ILogFiner("Generated header line:\t" + s_headerLine);
                ForestNETLib.Core.Global.ILogFiner("First line from file:\t" + a_csvLines[0]);

                /* compare generated header line with first line of csv file content */
                if (a_csvLines[0].Equals(s_headerLine))
                {
                    /* generated header line matches with first line, so we can skip this line */
                    i_start++;
                }

                /* iterate each line of csv file content */
                for (int i = i_start; i < a_csvLines.Length; i++)
                {
                    /* check if we have an empty line */
                    if (ForestNETLib.Core.Helper.IsStringEmpty(a_csvLines[i]))
                    {
                        /* go to next line */
                        continue;
                    }

                    /* array to store csv columns of a csv line */
                    List<string> a_csvColumns = [];

                    object? o_element = null;

                    /* parameter array element class is set */
                    if (b_genericList)
                    {
                        /* check if element type is not null */
                        if (o_elementType == null)
                        {
                            throw new NullReferenceException("Could not create instance by element type 'null'");
                        }

                        /* create a new object instance with parameter array element class and overwrite p_o_object */
                        o_element = Activator.CreateInstance(o_elementType);
                    }

                    /* state variable if we are currently in a double quoted value or not */
                    bool b_doubleQuoteActive = false;

                    /* help variable to store a csv column */
                    string s_csvColumn = "";

                    /* help variable if an escaped double quote is at start or end of the column */
                    bool b_tripleDoubleQuote = false;

                    /* iterate csv for each character */
                    for (int j = 0; j < a_csvLines[i].Length; j++)
                    {
                        /* if we found a not unescaped double quote */
                        if (
                            ((j == 0) && (j != a_csvLines[i].Length - 1) && (a_csvLines[i][j] == '"') && (a_csvLines[i][j + 1] != '"')) ||
                            (
                                ((j != 0) && (a_csvLines[i][j] == '"') && (a_csvLines[i][j - 1] != '"')) &&
                                ((j != a_csvLines[i].Length - 1) && (a_csvLines[i][j] == '"') && (a_csvLines[i][j + 1] != '"'))
                            ) ||
                            (b_tripleDoubleQuote)
                        )
                        {
                            /* if we are at the end of a double quoted value */
                            if (b_doubleQuoteActive)
                            {
                                /* unset state */
                                b_doubleQuoteActive = false;
                            }
                            else
                            {
                                /* we have a new double quoted value incoming, set state */
                                b_doubleQuoteActive = true;
                            }

                            /* unset flag */
                            if (b_tripleDoubleQuote)
                            {
                                b_tripleDoubleQuote = false;
                            }
                        }

                        /* set flag if an escaped double quote is at start or end of the column */
                        if ((j != 0) && (j != a_csvLines[i].Length - 1) && (a_csvLines[i][j - 1] == '"') && (a_csvLines[i][j] == '"') && (a_csvLines[i][j + 1] == '"'))
                        {
                            b_tripleDoubleQuote = true;
                        }

                        /* if current character is csv delimiter, or the last character of csv line and not a character within a double quoted value */
                        if (((a_csvLines[i][j] == this.s_delimiter[0]) || (j == a_csvLines[i].Length - 1)) && (!b_doubleQuoteActive))
                        {
                            /* if current character is the last character of csv line and not csv delimiter */
                            if ((j == a_csvLines[i].Length - 1) && (a_csvLines[i][j] != this.s_delimiter[0]))
                            {
                                /* add character to current csv column */
                                s_csvColumn += a_csvLines[i][j];
                            }

                            /* add csv column to csv column line array */
                            a_csvColumns.Add(s_csvColumn);

                            /* clear help variable */
                            s_csvColumn = "";

                            /* if last csv column has no value and there is just a delimiter */
                            if ((j == a_csvLines[i].Length - 1) && (a_csvLines[i][j] == this.s_delimiter[0]))
                            {
                                /* add empty csv column to csv column line array */
                                a_csvColumns.Add(s_csvColumn);
                            }
                        }
                        else
                        {
                            /* add character to current csv column */
                            s_csvColumn += a_csvLines[i][j];
                        }
                    }

                    int k = 0;

                    /* iterate all csv columns retrieved from current csv line */
                    foreach (string s_csvColumnValue in a_csvColumns)
                    {
                        /* help variable, because we may need to overwrite iteration value */
                        string s_foo = s_csvColumnValue;

                        /* if csv column value is a double quoted value */
                        if ((s_foo.StartsWith("\"")) && (s_foo.EndsWith("\"")))
                        {
                            /* remove double quote from start and end of csv column value */
                            s_foo = s_foo.Substring(1, s_foo.Length - 2);
                        }

                        /* if csv column value contains double double quotes '""' */
                        if (s_foo.Contains("\"\""))
                        {
                            /* replace all double double quotes '""' with single double quotes '"' */
                            s_foo = s_foo.Replace("\"\"", "\"");
                        }

                        ForestNETLib.Core.Global.ILogFiner("Field name '" + a_fieldNames[k] + "' of line '" + (i + 1) + "' gets value '" + s_foo + "'");

                        /* parameter array element class is set */
                        if (b_genericList)
                        {
                            /* set current csv column value into element object with property name of generated header line */
                            this.SetValue(a_fieldNames[k++], o_element, s_foo);
                        }
                        else
                        {
                            /* set current csv column value into parameter object with property name of generated header line */
                            this.SetValue(a_fieldNames[k++], p_o_object, s_foo);
                        }
                    }

                    /* parameter array element class is set */
                    if (b_genericList)
                    {
                        /* invoke generic list 'Add' method to add element to list */
                        _ = ((System.Collections.IList)p_o_object).Add(o_element);
                    }
                }
            }
        }

        /// <summary>
        /// Method to set property field of an object with value of csv file record
        /// supported types:
        /// byte, short, int, long, float, double, decimal, char, boolean, string,
        /// DateTime, TimeSpan, Object, sbyte, ushot, uint, ulong
        /// </summary>
        /// <param name="p_s_fieldName">property field name</param>
        /// <param name="p_o_object">object parameter where csv data will be decoded and cast into object fields</param>
        /// <param name="p_s_objectValue">string value of csv file record</param>
        /// <exception cref="NullReferenceException">instance which must be used has a null value</exception>
        /// <exception cref="ArgumentException">invalid type parameter, or invalid value length for conversion</exception>
        /// <exception cref="FormatException">exception parsing LocalDateTime, LocalDate or LocalTime value</exception>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        private void SetValue(string p_s_fieldName, Object? p_o_object, string p_s_objectValue)
        {
            /* check if object parameter is not null */
            if (p_o_object == null)
            {
                return;
            }

            if ((p_o_object.GetType().IsGenericType) && (p_o_object.GetType().GetGenericTypeDefinition() == typeof(List<>))) /* check if parameter object is a generic list */
            {
                /* check if object value is not empty */
                if (!ForestNETLib.Core.Helper.IsStringEmpty(p_s_objectValue))
                {
                    /* if type of generic list is contained in the list of allowed types */
                    if (a_allowedTypes.Contains(p_o_object.GetType().GenericTypeArguments[0].FullName))
                    {
                        /* help variable for setting object field */
                        List<Object?> a_foo = [];

                        /* check if object value contains csv array delimiter */
                        if (p_s_objectValue.Contains(this.s_arrayDelimiter))
                        {
                            /* split array element values by csv array delimiter */
                            string[] a_arrayElementValues = p_s_objectValue.Split(this.s_arrayDelimiter);

                            /* iterate all array element values */
                            foreach (string s_arrayElementValue in a_arrayElementValues)
                            {
                                /* cast array element value to object and add it to list */
                                a_foo.Add(this.StringToObjectValue(s_arrayElementValue, p_o_object.GetType()?.GenericTypeArguments[0]?.FullName));
                            }
                        }
                        else
                        {
                            /* cast array element value to object and add it to list, without splitting by csv array delimiter */
                            a_foo.Add(this.StringToObjectValue(p_s_objectValue, p_o_object.GetType()?.GenericTypeArguments[0]?.FullName));
                        }

                        /* check if we use properties to set object data values */
                        if (this.UseProperties)
                        {
                            /* property info for accessing generic list */
                            System.Reflection.PropertyInfo? o_propertyInfo;

                            /* try to get access to property info */
                            try
                            {
                                o_propertyInfo = p_o_object.GetType()?.GetProperty(p_s_fieldName);

                                if (o_propertyInfo == null)
                                {
                                    throw new Exception("property info is null");
                                }
                            }
                            catch (Exception o_exc)
                            {
                                throw new MissingMemberException("Class instance property[" + p_s_fieldName + "] does not exist for class instance(" + p_o_object.GetType().FullName + "): " + o_exc.Message);
                            }

                            /* check if we can access generic list property */
                            if (!o_propertyInfo.CanRead)
                            {
                                throw new MemberAccessException("Cannot read property from class; instance property[" + p_s_fieldName + "] does not exist for class instance(" + p_o_object.GetType().FullName + ")");
                            }

                            /* get generic list property */
                            object o_foo = o_propertyInfo.GetValue(p_o_object) ?? throw new NullReferenceException("generic list property of current element is 'null'");

                            /* iterate each object in list */
                            foreach (Object? o_bar in a_foo)
                            {
                                /* get generic list property of current element and add our list object value */
                                ((List<Object?>)o_foo).Add(o_bar);
                            }
                        }
                        else
                        {
                            /* field info for accessing generic list */
                            System.Reflection.FieldInfo? o_fieldInfo;

                            /* try to get access to field info */
                            try
                            {
                                o_fieldInfo = p_o_object.GetType()?.GetField(p_s_fieldName);

                                if (o_fieldInfo == null)
                                {
                                    throw new Exception("field info is null");
                                }
                            }
                            catch (Exception o_exc)
                            {
                                throw new MissingMemberException("Class instance field[" + p_s_fieldName + "] does not exist for class instance(" + p_o_object.GetType().FullName + "): " + o_exc.Message);
                            }

                            /* check if we can access generic list field */
                            if (!o_fieldInfo.IsPublic)
                            {
                                throw new MemberAccessException("Cannot read field from class; instance field[" + p_s_fieldName + "] does not exist for class instance(" + p_o_object.GetType().FullName + ")");
                            }

                            /* get generic list field */
                            object o_foo = o_fieldInfo.GetValue(p_o_object) ?? throw new NullReferenceException("generic list field of current element is 'null'");

                            /* iterate each object in list */
                            foreach (Object? o_bar in a_foo)
                            {
                                /* get generic list property of current element and add our list object value */
                                ((List<Object?>)o_foo).Add(o_bar);
                            }
                        }
                    }
                }
            }
            else /* handle parameter object value as generic list, primitive array or standard object */
            {
                /* check if we use properties to set object data values */
                if (this.UseProperties)
                {
                    /* property info for accessing propert */
                    System.Reflection.PropertyInfo? o_propertyInfo;

                    /* try to get access to property info */
                    try
                    {
                        o_propertyInfo = p_o_object.GetType().GetProperty(p_s_fieldName);

                        if (o_propertyInfo == null)
                        {
                            throw new Exception("property info is null");
                        }
                    }
                    catch (Exception o_exc)
                    {
                        throw new MissingMemberException("Class instance property[" + p_s_fieldName + "] does not exist for class instance(" + p_o_object.GetType().FullName + "): " + o_exc.Message);
                    }

                    /* check if we can access property */
                    if (!o_propertyInfo.CanWrite)
                    {
                        throw new MemberAccessException("Cannot write property from class; instance property[" + p_s_fieldName + "] does not exist for class instance(" + p_o_object.GetType().FullName + ")");
                    }

                    /* check if property/field of parameter object is a generic list */
                    if ((o_propertyInfo.PropertyType.IsGenericType) && (o_propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(List<>)))
                    {
                        /* set value for generic list */
                        this.SetValueForArraysOrLists(p_s_fieldName, p_o_object, p_s_objectValue, false);
                    }
                    else if (o_propertyInfo.PropertyType.IsArray) /* check if property/field of parameter object is a primitive array */
                    {
                        /* set value for primitive array */
                        this.SetValueForArraysOrLists(p_s_fieldName, p_o_object, p_s_objectValue, true);
                    }
                    else
                    {
                        /* set property with our object value */
                        o_propertyInfo.SetValue(p_o_object, this.StringToObjectValue(p_s_objectValue, o_propertyInfo.PropertyType.FullName));
                    }
                }
                else
                {
                    /* field info for accessing field */
                    System.Reflection.FieldInfo? o_fieldInfo;

                    /* try to get access to field info */
                    try
                    {
                        o_fieldInfo = p_o_object.GetType().GetField(p_s_fieldName);

                        if (o_fieldInfo == null)
                        {
                            throw new Exception("field info is null");
                        }
                    }
                    catch (Exception o_exc)
                    {
                        throw new MissingMemberException("Class instance field[" + p_s_fieldName + "] does not exist for class instance(" + p_o_object.GetType().FullName + "): " + o_exc.Message);
                    }

                    /* check if we can access field */
                    if (!o_fieldInfo.IsPublic)
                    {
                        throw new MemberAccessException("Cannot read field from class; instance field[" + p_s_fieldName + "] does not exist for class instance(" + p_o_object.GetType().FullName + ")");
                    }

                    /* check if property/field of parameter object is a generic list */
                    if ((o_fieldInfo.FieldType.IsGenericType) && (o_fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>)))
                    {
                        /* set value for generic list */
                        this.SetValueForArraysOrLists(p_s_fieldName, p_o_object, p_s_objectValue, false);
                    }
                    else if (o_fieldInfo.FieldType.IsArray) /* check if property/field of parameter object is a primitive array */
                    {
                        /* set value for primitive array */
                        this.SetValueForArraysOrLists(p_s_fieldName, p_o_object, p_s_objectValue, true);
                    }
                    else
                    {
                        /* set field with our object value */
                        o_fieldInfo.SetValue(p_o_object, this.StringToObjectValue(p_s_objectValue, o_fieldInfo.FieldType.FullName));
                    }
                }
            }
        }

        /// <summary>
        /// Method to set property field array or generic list of an object with value of csv file record
        /// supported types:
        /// byte, short, int, long, float, double, decimal, char, boolean, string,
        /// DateTime, TimeSpan, Object, sbyte, ushot, uint, ulong
        /// </summary>
        /// <param name="p_s_fieldName">property field name</param>
        /// <param name="p_o_object">object parameter where csv data will be decoded and cast into object fields</param>
        /// <param name="p_s_objectValue">string value of csv file record</param>
        /// <param name="p_s_objectValue">true - set primitive array, false - set generic list</param>
        /// <exception cref="ArgumentException">invalid type parameter, or invalid value length for conversion</exception>
        /// <exception cref="FormatException">exception parsing LocalDateTime, LocalDate or LocalTime value</exception>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        private void SetValueForArraysOrLists(string p_s_fieldName, Object p_o_object, string p_s_objectValue, bool p_b_forArray)
        {
            /* check if object value is not empty */
            if (!ForestNETLib.Core.Helper.IsStringEmpty(p_s_objectValue))
            {
                /* array/list type variable */
                string? s_arrayType;

                /* get array/list type as string from property or field */
                if (this.UseProperties)
                {
                    /* property info for accessing property type */
                    System.Reflection.PropertyInfo? o_propertyInfo = p_o_object.GetType().GetProperty(p_s_fieldName) ?? throw new NullReferenceException("Cannot get " + ((p_b_forArray) ? "array" : "generic list") + " property '" + p_s_fieldName + "' with property info: null");

                    if (p_b_forArray)
                    {
                        s_arrayType = o_propertyInfo.PropertyType.GetElementType()?.FullName;
                    }
                    else
                    {
                        s_arrayType = o_propertyInfo.PropertyType.GenericTypeArguments[0].FullName;
                    }
                }
                else
                {
                    /* field info for accessing field type */
                    System.Reflection.FieldInfo? o_fieldInfo = p_o_object.GetType().GetField(p_s_fieldName) ?? throw new NullReferenceException("Cannot get " + ((p_b_forArray) ? "array" : "generic list") + " field '" + p_s_fieldName + "' with field info: null");

                    if (p_b_forArray)
                    {
                        s_arrayType = o_fieldInfo.FieldType.GetElementType()?.FullName;
                    }
                    else
                    {
                        s_arrayType = o_fieldInfo.FieldType.GenericTypeArguments[0].FullName;
                    }
                }

                /* check if array type is null */
                if (s_arrayType == null)
                {
                    ForestNETLib.Core.Global.ILogFiner("Field/Property " + ((p_b_forArray) ? "array" : "list") + " '" + p_s_fieldName + "' has element type 'null' which is not allowed. Will be skipped.");
                    return;
                }

                /* if type of array is not contained in the list of allowed types */
                if (!a_allowedTypes.Contains(s_arrayType))
                {
                    ForestNETLib.Core.Global.ILogFiner("Field/Property " + ((p_b_forArray) ? "array" : "list") + " '" + p_s_fieldName + "' has element type '" + s_arrayType + "' which is not allowed. Will be skipped.");
                    return;
                }

                /* help variable for setting object field/property */
                List<Object?> a_foo = [];

                /* check if object value contains csv array delimiter */
                if (p_s_objectValue.Contains(this.s_arrayDelimiter))
                {
                    /* split array element values by csv array delimiter */
                    string[] a_arrayElementValues = p_s_objectValue.Split(this.s_arrayDelimiter);

                    /* iterate all array element values */
                    foreach (string s_arrayElementValue in a_arrayElementValues)
                    {
                        /* cast array element value to object and add it to generic list */
                        a_foo.Add(this.StringToObjectValue(s_arrayElementValue, s_arrayType));
                    }
                }
                else
                {
                    /* cast array element value to object and add it to generic list, without splitting by csv array delimiter */
                    a_foo.Add(this.StringToObjectValue(p_s_objectValue, s_arrayType));
                }

                /* lower array type */
                s_arrayType = s_arrayType.ToLower();

                /* check if we use properties to set object data values */
                if (this.UseProperties)
                {
                    /* property info for accessing generic list */
                    System.Reflection.PropertyInfo? o_propertyInfo = p_o_object.GetType().GetProperty(p_s_fieldName) ?? throw new NullReferenceException("Cannot get " + ((p_b_forArray) ? "array" : "generic list") + " property '" + p_s_fieldName + "' with property info: null");

                    /* check if we can access generic list property */
                    if (!o_propertyInfo.CanWrite)
                    {
                        throw new MemberAccessException("Cannot write property from class; instance property[" + p_s_fieldName + "] does not exist for class instance(" + p_o_object.GetType().FullName + ")");
                    }

                    /* handle allowed array types */
                    if (s_arrayType.Equals("system.boolean"))
                    {
                        /* create primitive array */
                        bool[] a_objects = new bool[a_foo.Count];

                        /* transport values from generic list to primitive array */
                        for (int i = 0; i < a_foo.Count; i++)
                        {
                            a_objects[i] = (bool?)a_foo[i] ?? default;
                        }

                        if (p_b_forArray)
                        {
                            /* set primitive array to our parameter object */
                            o_propertyInfo.SetValue(p_o_object, a_objects);
                        }
                        else
                        {
                            /* set generic list to our parameter object, quick convert with ToList<>() */
                            o_propertyInfo.SetValue(p_o_object, a_objects.ToList());
                        }
                    }
                    else if (s_arrayType.Equals("system.byte"))
                    {
                        /* create primitive array */
                        byte[] a_objects = new byte[a_foo.Count];

                        /* transport values from generic list to primitive array */
                        for (int i = 0; i < a_foo.Count; i++)
                        {
                            a_objects[i] = (byte?)a_foo[i] ?? default;
                        }

                        if (p_b_forArray)
                        {
                            /* set primitive array to our parameter object */
                            o_propertyInfo.SetValue(p_o_object, a_objects);
                        }
                        else
                        {
                            /* set generic list to our parameter object, quick convert with ToList<>() */
                            o_propertyInfo.SetValue(p_o_object, a_objects.ToList());
                        }
                    }
                    else if (s_arrayType.Equals("system.sbyte"))
                    {
                        /* create primitive array */
                        sbyte[] a_objects = new sbyte[a_foo.Count];

                        /* transport values from generic list to primitive array */
                        for (int i = 0; i < a_foo.Count; i++)
                        {
                            a_objects[i] = (sbyte?)a_foo[i] ?? default;
                        }

                        if (p_b_forArray)
                        {
                            /* set primitive array to our parameter object */
                            o_propertyInfo.SetValue(p_o_object, a_objects);
                        }
                        else
                        {
                            /* set generic list to our parameter object, quick convert with ToList<>() */
                            o_propertyInfo.SetValue(p_o_object, a_objects.ToList());
                        }
                    }
                    else if (s_arrayType.Equals("system.char"))
                    {
                        /* create primitive array */
                        char[] a_objects = new char[a_foo.Count];

                        /* transport values from generic list to primitive array */
                        for (int i = 0; i < a_foo.Count; i++)
                        {
                            a_objects[i] = (char?)a_foo[i] ?? default;
                        }

                        if (p_b_forArray)
                        {
                            /* set primitive array to our parameter object */
                            o_propertyInfo.SetValue(p_o_object, a_objects);
                        }
                        else
                        {
                            /* set generic list to our parameter object, quick convert with ToList<>() */
                            o_propertyInfo.SetValue(p_o_object, a_objects.ToList());
                        }
                    }
                    else if (s_arrayType.Equals("system.single"))
                    {
                        /* create primitive array */
                        float[] a_objects = new float[a_foo.Count];

                        /* transport values from generic list to primitive array */
                        for (int i = 0; i < a_foo.Count; i++)
                        {
                            a_objects[i] = (float?)a_foo[i] ?? default;
                        }

                        if (p_b_forArray)
                        {
                            /* set primitive array to our parameter object */
                            o_propertyInfo.SetValue(p_o_object, a_objects);
                        }
                        else
                        {
                            /* set generic list to our parameter object, quick convert with ToList<>() */
                            o_propertyInfo.SetValue(p_o_object, a_objects.ToList());
                        }
                    }
                    else if (s_arrayType.Equals("system.double"))
                    {
                        /* create primitive array */
                        double[] a_objects = new double[a_foo.Count];

                        /* transport values from generic list to primitive array */
                        for (int i = 0; i < a_foo.Count; i++)
                        {
                            a_objects[i] = (double?)a_foo[i] ?? default;
                        }

                        if (p_b_forArray)
                        {
                            /* set primitive array to our parameter object */
                            o_propertyInfo.SetValue(p_o_object, a_objects);
                        }
                        else
                        {
                            /* set generic list to our parameter object, quick convert with ToList<>() */
                            o_propertyInfo.SetValue(p_o_object, a_objects.ToList());
                        }
                    }
                    else if (s_arrayType.Equals("system.int16"))
                    {
                        /* create primitive array */
                        short[] a_objects = new short[a_foo.Count];

                        /* transport values from generic list to primitive array */
                        for (int i = 0; i < a_foo.Count; i++)
                        {
                            a_objects[i] = (short?)a_foo[i] ?? default;
                        }

                        if (p_b_forArray)
                        {
                            /* set primitive array to our parameter object */
                            o_propertyInfo.SetValue(p_o_object, a_objects);
                        }
                        else
                        {
                            /* set generic list to our parameter object, quick convert with ToList<>() */
                            o_propertyInfo.SetValue(p_o_object, a_objects.ToList());
                        }
                    }
                    else if (s_arrayType.Equals("system.int32"))
                    {
                        /* create primitive array */
                        int[] a_objects = new int[a_foo.Count];

                        /* transport values from generic list to primitive array */
                        for (int i = 0; i < a_foo.Count; i++)
                        {
                            a_objects[i] = (int?)a_foo[i] ?? default;
                        }

                        if (p_b_forArray)
                        {
                            /* set primitive array to our parameter object */
                            o_propertyInfo.SetValue(p_o_object, a_objects);
                        }
                        else
                        {
                            /* set generic list to our parameter object, quick convert with ToList<>() */
                            o_propertyInfo.SetValue(p_o_object, a_objects.ToList());
                        }
                    }
                    else if (s_arrayType.Equals("system.int64"))
                    {
                        /* create primitive array */
                        long[] a_objects = new long[a_foo.Count];

                        /* transport values from generic list to primitive array */
                        for (int i = 0; i < a_foo.Count; i++)
                        {
                            a_objects[i] = (long?)a_foo[i] ?? default;
                        }

                        if (p_b_forArray)
                        {
                            /* set primitive array to our parameter object */
                            o_propertyInfo.SetValue(p_o_object, a_objects);
                        }
                        else
                        {
                            /* set generic list to our parameter object, quick convert with ToList<>() */
                            o_propertyInfo.SetValue(p_o_object, a_objects.ToList());
                        }
                    }
                    else if (s_arrayType.Equals("system.uint16"))
                    {
                        /* create primitive array */
                        ushort[] a_objects = new ushort[a_foo.Count];

                        /* transport values from generic list to primitive array */
                        for (int i = 0; i < a_foo.Count; i++)
                        {
                            a_objects[i] = (ushort?)a_foo[i] ?? default;
                        }

                        if (p_b_forArray)
                        {
                            /* set primitive array to our parameter object */
                            o_propertyInfo.SetValue(p_o_object, a_objects);
                        }
                        else
                        {
                            /* set generic list to our parameter object, quick convert with ToList<>() */
                            o_propertyInfo.SetValue(p_o_object, a_objects.ToList());
                        }
                    }
                    else if (s_arrayType.Equals("system.uint32"))
                    {
                        /* create primitive array */
                        uint[] a_objects = new uint[a_foo.Count];

                        /* transport values from generic list to primitive array */
                        for (int i = 0; i < a_foo.Count; i++)
                        {
                            a_objects[i] = (uint?)a_foo[i] ?? default;
                        }

                        if (p_b_forArray)
                        {
                            /* set primitive array to our parameter object */
                            o_propertyInfo.SetValue(p_o_object, a_objects);
                        }
                        else
                        {
                            /* set generic list to our parameter object, quick convert with ToList<>() */
                            o_propertyInfo.SetValue(p_o_object, a_objects.ToList());
                        }
                    }
                    else if (s_arrayType.Equals("system.uint64"))
                    {
                        /* create primitive array */
                        ulong[] a_objects = new ulong[a_foo.Count];

                        /* transport values from generic list to primitive array */
                        for (int i = 0; i < a_foo.Count; i++)
                        {
                            a_objects[i] = (ulong?)a_foo[i] ?? default;
                        }

                        if (p_b_forArray)
                        {
                            /* set primitive array to our parameter object */
                            o_propertyInfo.SetValue(p_o_object, a_objects);
                        }
                        else
                        {
                            /* set generic list to our parameter object, quick convert with ToList<>() */
                            o_propertyInfo.SetValue(p_o_object, a_objects.ToList());
                        }
                    }
                    else if (s_arrayType.Equals("system.string"))
                    {
                        /* create primitive array */
                        string?[] a_objects = new string?[a_foo.Count];

                        /* transport values from generic list to primitive array */
                        for (int i = 0; i < a_foo.Count; i++)
                        {
                            a_objects[i] = (string?)a_foo[i];
                        }

                        if (p_b_forArray)
                        {
                            /* set primitive array to our parameter object */
                            o_propertyInfo.SetValue(p_o_object, a_objects);
                        }
                        else
                        {
                            /* set generic list to our parameter object, quick convert with ToList<>() */
                            o_propertyInfo.SetValue(p_o_object, a_objects.ToList());
                        }
                    }
                    else if (s_arrayType.Equals("system.datetime"))
                    {
                        /* create primitive array */
                        DateTime[] a_objects = new DateTime[a_foo.Count];

                        /* transport values from generic list to primitive array */
                        for (int i = 0; i < a_foo.Count; i++)
                        {
                            a_objects[i] = (DateTime?)a_foo[i] ?? default;
                        }

                        if (p_b_forArray)
                        {
                            /* set primitive array to our parameter object */
                            o_propertyInfo.SetValue(p_o_object, a_objects);
                        }
                        else
                        {
                            /* set generic list to our parameter object, quick convert with ToList<>() */
                            o_propertyInfo.SetValue(p_o_object, a_objects.ToList());
                        }
                    }
                    else if (s_arrayType.Equals("system.timespan"))
                    {
                        /* create primitive array */
                        TimeSpan[] a_objects = new TimeSpan[a_foo.Count];

                        /* transport values from generic list to primitive array */
                        for (int i = 0; i < a_foo.Count; i++)
                        {
                            a_objects[i] = (TimeSpan?)a_foo[i] ?? default;
                        }

                        if (p_b_forArray)
                        {
                            /* set primitive array to our parameter object */
                            o_propertyInfo.SetValue(p_o_object, a_objects);
                        }
                        else
                        {
                            /* set generic list to our parameter object, quick convert with ToList<>() */
                            o_propertyInfo.SetValue(p_o_object, a_objects.ToList());
                        }
                    }
                    else if (s_arrayType.Equals("system.decimal"))
                    {
                        /* create primitive array */
                        decimal[] a_objects = new decimal[a_foo.Count];

                        /* transport values from generic list to primitive array */
                        for (int i = 0; i < a_foo.Count; i++)
                        {
                            a_objects[i] = (decimal?)a_foo[i] ?? default;
                        }

                        if (p_b_forArray)
                        {
                            /* set primitive array to our parameter object */
                            o_propertyInfo.SetValue(p_o_object, a_objects);
                        }
                        else
                        {
                            /* set generic list to our parameter object, quick convert with ToList<>() */
                            o_propertyInfo.SetValue(p_o_object, a_objects.ToList());
                        }
                    }
                    else if (s_arrayType.Equals("system.object"))
                    {
                        /* create primitive array */
                        Object?[] a_objects = new Object?[a_foo.Count];

                        /* transport values from generic list to primitive array */
                        for (int i = 0; i < a_foo.Count; i++)
                        {
                            a_objects[i] = a_foo[i];
                        }

                        if (p_b_forArray)
                        {
                            /* set primitive array to our parameter object */
                            o_propertyInfo.SetValue(p_o_object, a_objects);
                        }
                        else
                        {
                            /* set generic list to our parameter object, quick convert with ToList<>() */
                            o_propertyInfo.SetValue(p_o_object, a_objects.ToList());
                        }
                    }
                }
                else
                {
                    /* field info for accessing generic list */
                    System.Reflection.FieldInfo? o_fieldInfo = p_o_object.GetType().GetField(p_s_fieldName) ?? throw new NullReferenceException("Cannot get " + ((p_b_forArray) ? "array" : "generic list") + " field '" + p_s_fieldName + "' with field info: null");

                    /* check if we can access generic list field */
                    if (!o_fieldInfo.IsPublic)
                    {
                        throw new MemberAccessException("Cannot read field from class; instance field[" + p_s_fieldName + "] does not exist for class instance(" + p_o_object.GetType().FullName + ")");
                    }

                    /* handle allowed array types */
                    if (s_arrayType.Equals("system.boolean"))
                    {
                        /* create primitive array */
                        bool[] a_objects = new bool[a_foo.Count];

                        /* transport values from generic list to primitive array */
                        for (int i = 0; i < a_foo.Count; i++)
                        {
                            a_objects[i] = (bool?)a_foo[i] ?? default;
                        }

                        if (p_b_forArray)
                        {
                            /* set primitive array to our parameter object */
                            o_fieldInfo.SetValue(p_o_object, a_objects);
                        }
                        else
                        {
                            /* set generic list to our parameter object, quick convert with ToList<>() */
                            o_fieldInfo.SetValue(p_o_object, a_objects.ToList());
                        }
                    }
                    else if (s_arrayType.Equals("system.byte"))
                    {
                        /* create primitive array */
                        byte[] a_objects = new byte[a_foo.Count];

                        /* transport values from generic list to primitive array */
                        for (int i = 0; i < a_foo.Count; i++)
                        {
                            a_objects[i] = (byte?)a_foo[i] ?? default;
                        }

                        if (p_b_forArray)
                        {
                            /* set primitive array to our parameter object */
                            o_fieldInfo.SetValue(p_o_object, a_objects);
                        }
                        else
                        {
                            /* set generic list to our parameter object, quick convert with ToList<>() */
                            o_fieldInfo.SetValue(p_o_object, a_objects.ToList());
                        }
                    }
                    else if (s_arrayType.Equals("system.sbyte"))
                    {
                        /* create primitive array */
                        sbyte[] a_objects = new sbyte[a_foo.Count];

                        /* transport values from generic list to primitive array */
                        for (int i = 0; i < a_foo.Count; i++)
                        {
                            a_objects[i] = (sbyte?)a_foo[i] ?? default;
                        }

                        if (p_b_forArray)
                        {
                            /* set primitive array to our parameter object */
                            o_fieldInfo.SetValue(p_o_object, a_objects);
                        }
                        else
                        {
                            /* set generic list to our parameter object, quick convert with ToList<>() */
                            o_fieldInfo.SetValue(p_o_object, a_objects.ToList());
                        }
                    }
                    else if (s_arrayType.Equals("system.char"))
                    {
                        /* create primitive array */
                        char[] a_objects = new char[a_foo.Count];

                        /* transport values from generic list to primitive array */
                        for (int i = 0; i < a_foo.Count; i++)
                        {
                            a_objects[i] = (char?)a_foo[i] ?? default;
                        }

                        if (p_b_forArray)
                        {
                            /* set primitive array to our parameter object */
                            o_fieldInfo.SetValue(p_o_object, a_objects);
                        }
                        else
                        {
                            /* set generic list to our parameter object, quick convert with ToList<>() */
                            o_fieldInfo.SetValue(p_o_object, a_objects.ToList());
                        }
                    }
                    else if (s_arrayType.Equals("system.single"))
                    {
                        /* create primitive array */
                        float[] a_objects = new float[a_foo.Count];

                        /* transport values from generic list to primitive array */
                        for (int i = 0; i < a_foo.Count; i++)
                        {
                            a_objects[i] = (float?)a_foo[i] ?? default;
                        }

                        if (p_b_forArray)
                        {
                            /* set primitive array to our parameter object */
                            o_fieldInfo.SetValue(p_o_object, a_objects);
                        }
                        else
                        {
                            /* set generic list to our parameter object, quick convert with ToList<>() */
                            o_fieldInfo.SetValue(p_o_object, a_objects.ToList());
                        }
                    }
                    else if (s_arrayType.Equals("system.double"))
                    {
                        /* create primitive array */
                        double[] a_objects = new double[a_foo.Count];

                        /* transport values from generic list to primitive array */
                        for (int i = 0; i < a_foo.Count; i++)
                        {
                            a_objects[i] = (double?)a_foo[i] ?? default;
                        }

                        if (p_b_forArray)
                        {
                            /* set primitive array to our parameter object */
                            o_fieldInfo.SetValue(p_o_object, a_objects);
                        }
                        else
                        {
                            /* set generic list to our parameter object, quick convert with ToList<>() */
                            o_fieldInfo.SetValue(p_o_object, a_objects.ToList());
                        }
                    }
                    else if (s_arrayType.Equals("system.int16"))
                    {
                        /* create primitive array */
                        short[] a_objects = new short[a_foo.Count];

                        /* transport values from generic list to primitive array */
                        for (int i = 0; i < a_foo.Count; i++)
                        {
                            a_objects[i] = (short?)a_foo[i] ?? default;
                        }

                        if (p_b_forArray)
                        {
                            /* set primitive array to our parameter object */
                            o_fieldInfo.SetValue(p_o_object, a_objects);
                        }
                        else
                        {
                            /* set generic list to our parameter object, quick convert with ToList<>() */
                            o_fieldInfo.SetValue(p_o_object, a_objects.ToList());
                        }
                    }
                    else if (s_arrayType.Equals("system.int32"))
                    {
                        /* create primitive array */
                        int[] a_objects = new int[a_foo.Count];

                        /* transport values from generic list to primitive array */
                        for (int i = 0; i < a_foo.Count; i++)
                        {
                            a_objects[i] = (int?)a_foo[i] ?? default;
                        }

                        if (p_b_forArray)
                        {
                            /* set primitive array to our parameter object */
                            o_fieldInfo.SetValue(p_o_object, a_objects);
                        }
                        else
                        {
                            /* set generic list to our parameter object, quick convert with ToList<>() */
                            o_fieldInfo.SetValue(p_o_object, a_objects.ToList());
                        }
                    }
                    else if (s_arrayType.Equals("system.int64"))
                    {
                        /* create primitive array */
                        long[] a_objects = new long[a_foo.Count];

                        /* transport values from generic list to primitive array */
                        for (int i = 0; i < a_foo.Count; i++)
                        {
                            a_objects[i] = (long?)a_foo[i] ?? default;
                        }

                        if (p_b_forArray)
                        {
                            /* set primitive array to our parameter object */
                            o_fieldInfo.SetValue(p_o_object, a_objects);
                        }
                        else
                        {
                            /* set generic list to our parameter object, quick convert with ToList<>() */
                            o_fieldInfo.SetValue(p_o_object, a_objects.ToList());
                        }
                    }
                    else if (s_arrayType.Equals("system.uint16"))
                    {
                        /* create primitive array */
                        ushort[] a_objects = new ushort[a_foo.Count];

                        /* transport values from generic list to primitive array */
                        for (int i = 0; i < a_foo.Count; i++)
                        {
                            a_objects[i] = (ushort?)a_foo[i] ?? default;
                        }

                        if (p_b_forArray)
                        {
                            /* set primitive array to our parameter object */
                            o_fieldInfo.SetValue(p_o_object, a_objects);
                        }
                        else
                        {
                            /* set generic list to our parameter object, quick convert with ToList<>() */
                            o_fieldInfo.SetValue(p_o_object, a_objects.ToList());
                        }
                    }
                    else if (s_arrayType.Equals("system.uint32"))
                    {
                        /* create primitive array */
                        uint[] a_objects = new uint[a_foo.Count];

                        /* transport values from generic list to primitive array */
                        for (int i = 0; i < a_foo.Count; i++)
                        {
                            a_objects[i] = (uint?)a_foo[i] ?? default;
                        }

                        if (p_b_forArray)
                        {
                            /* set primitive array to our parameter object */
                            o_fieldInfo.SetValue(p_o_object, a_objects);
                        }
                        else
                        {
                            /* set generic list to our parameter object, quick convert with ToList<>() */
                            o_fieldInfo.SetValue(p_o_object, a_objects.ToList());
                        }
                    }
                    else if (s_arrayType.Equals("system.uint64"))
                    {
                        /* create primitive array */
                        ulong[] a_objects = new ulong[a_foo.Count];

                        /* transport values from generic list to primitive array */
                        for (int i = 0; i < a_foo.Count; i++)
                        {
                            a_objects[i] = (ulong?)a_foo[i] ?? default;
                        }

                        if (p_b_forArray)
                        {
                            /* set primitive array to our parameter object */
                            o_fieldInfo.SetValue(p_o_object, a_objects);
                        }
                        else
                        {
                            /* set generic list to our parameter object, quick convert with ToList<>() */
                            o_fieldInfo.SetValue(p_o_object, a_objects.ToList());
                        }
                    }
                    else if (s_arrayType.Equals("system.string"))
                    {
                        /* create primitive array */
                        string?[] a_objects = new string?[a_foo.Count];

                        /* transport values from generic list to primitive array */
                        for (int i = 0; i < a_foo.Count; i++)
                        {
                            a_objects[i] = (string?)a_foo[i];
                        }

                        if (p_b_forArray)
                        {
                            /* set primitive array to our parameter object */
                            o_fieldInfo.SetValue(p_o_object, a_objects);
                        }
                        else
                        {
                            /* set generic list to our parameter object, quick convert with ToList<>() */
                            o_fieldInfo.SetValue(p_o_object, a_objects.ToList());
                        }
                    }
                    else if (s_arrayType.Equals("system.datetime"))
                    {
                        /* create primitive array */
                        DateTime[] a_objects = new DateTime[a_foo.Count];

                        /* transport values from generic list to primitive array */
                        for (int i = 0; i < a_foo.Count; i++)
                        {
                            a_objects[i] = (DateTime?)a_foo[i] ?? default;
                        }

                        if (p_b_forArray)
                        {
                            /* set primitive array to our parameter object */
                            o_fieldInfo.SetValue(p_o_object, a_objects);
                        }
                        else
                        {
                            /* set generic list to our parameter object, quick convert with ToList<>() */
                            o_fieldInfo.SetValue(p_o_object, a_objects.ToList());
                        }
                    }
                    else if (s_arrayType.Equals("system.timespan"))
                    {
                        /* create primitive array */
                        TimeSpan[] a_objects = new TimeSpan[a_foo.Count];

                        /* transport values from generic list to primitive array */
                        for (int i = 0; i < a_foo.Count; i++)
                        {
                            a_objects[i] = (TimeSpan?)a_foo[i] ?? default;
                        }

                        if (p_b_forArray)
                        {
                            /* set primitive array to our parameter object */
                            o_fieldInfo.SetValue(p_o_object, a_objects);
                        }
                        else
                        {
                            /* set generic list to our parameter object, quick convert with ToList<>() */
                            o_fieldInfo.SetValue(p_o_object, a_objects.ToList());
                        }
                    }
                    else if (s_arrayType.Equals("system.decimal"))
                    {
                        /* create primitive array */
                        decimal[] a_objects = new decimal[a_foo.Count];

                        /* transport values from generic list to primitive array */
                        for (int i = 0; i < a_foo.Count; i++)
                        {
                            a_objects[i] = (decimal?)a_foo[i] ?? default;
                        }

                        if (p_b_forArray)
                        {
                            /* set primitive array to our parameter object */
                            o_fieldInfo.SetValue(p_o_object, a_objects);
                        }
                        else
                        {
                            /* set generic list to our parameter object, quick convert with ToList<>() */
                            o_fieldInfo.SetValue(p_o_object, a_objects.ToList());
                        }
                    }
                    else if (s_arrayType.Equals("system.object"))
                    {
                        /* create primitive array */
                        Object?[] a_objects = new Object?[a_foo.Count];

                        /* transport values from generic list to primitive array */
                        for (int i = 0; i < a_foo.Count; i++)
                        {
                            a_objects[i] = a_foo[i];
                        }

                        if (p_b_forArray)
                        {
                            /* set primitive array to our parameter object */
                            o_fieldInfo.SetValue(p_o_object, a_objects);
                        }
                        else
                        {
                            /* set generic list to our parameter object, quick convert with ToList<>() */
                            o_fieldInfo.SetValue(p_o_object, a_objects.ToList());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Convert a string value from a csv file record to an object to decode it into an object
        /// supported types:
        /// byte, short, int, long, float, double, decimal, char, boolean, string,
        /// DateTime, TimeSpan, Object, sbyte, ushot, uint, ulong
        /// </summary>
        /// <param name="p_s_value">string value from a csv file record</param>
        /// <param name="p_s_type">type of destination object field</param>
        /// <returns>Object</returns>
        /// <exception cref="ArgumentException">invalid type parameter, or invalid value length for conversion</exception>
        /// <exception cref="FormatException">exception parsing LocalDateTime, LocalDate or LocalTime value</exception>
        private Object? StringToObjectValue(string p_s_value, string? p_s_type)
        {
            Object? o_foo = null;

            /* return null if type parameter is null */
            if (p_s_type == null)
            {
                return o_foo;
            }

            /* transpose primitive type to class type */
            switch (p_s_type.ToLower())
            {
                case "byte":
                    p_s_type = "System.Byte";
                    break;
                case "short":
                    p_s_type = "System.Int16";
                    break;
                case "int":
                    p_s_type = "System.Int32";
                    break;
                case "long":
                    p_s_type = "System.Int64";
                    break;
                case "float":
                    p_s_type = "System.Single";
                    break;
                case "double":
                    p_s_type = "System.Double";
                    break;
                case "bool":
                    p_s_type = "System.Boolean";
                    break;
                case "char":
                    p_s_type = "System.Char";
                    break;
                case "string":
                    p_s_type = "System.String";
                    break;
                case "date":
                case "datetime":
                    p_s_type = "System.DateTime";
                    break;
                case "time":
                    p_s_type = "System.TimeSpan";
                    break;
                case "decimal":
                    p_s_type = "System.Decimal";
                    break;
                case "sbyte":
                    p_s_type = "System.SByte";
                    break;
                case "ushort":
                    p_s_type = "System.UInt16";
                    break;
                case "uint":
                    p_s_type = "System.UInt32";
                    break;
                case "ulong":
                    p_s_type = "System.UInt64";
                    break;
                case "object":
                    p_s_type = "System.Object";
                    break;
            }

            /* check if value is not empty */
            if (!p_s_value.Equals(""))
            {
                if (p_s_type.Equals("System.String"))
                {
                    o_foo = p_s_value;
                }
                else if (p_s_type.Equals("System.Byte"))
                {
                    o_foo = Convert.ToByte(p_s_value);
                }
                else if (p_s_type.Equals("System.Int16"))
                {
                    o_foo = Convert.ToInt16(p_s_value);
                }
                else if (p_s_type.Equals("System.Int32"))
                {
                    o_foo = Convert.ToInt32(p_s_value);
                }
                else if (p_s_type.Equals("System.Int64"))
                {
                    o_foo = Convert.ToInt64(p_s_value);
                }
                else if (p_s_type.Equals("System.Single"))
                {
                    o_foo = Convert.ToSingle(p_s_value, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                }
                else if (p_s_type.Equals("System.Double"))
                {
                    o_foo = Convert.ToDouble(p_s_value, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                }
                else if (p_s_type.Equals("System.Boolean"))
                {
                    o_foo = Convert.ToBoolean(p_s_value);
                }
                else if (p_s_type.Equals("System.Char"))
                {
                    if (p_s_value.Length != 1)
                    {
                        throw new ArgumentException("Value must have length(1) for character - ['" + p_s_value + "' -> length(" + p_s_value.Length + ")]");
                    }

                    o_foo = (char)p_s_value[0];
                }
                else if (p_s_type.Equals("System.DateTime"))
                {
                    /* handle default DateTime */
                    if (p_s_value.Equals("0001-01-01T00:00:00Z"))
                    {
                        o_foo = default(DateTime);
                    }
                    else
                    {
                        o_foo = ForestNETLib.Core.Helper.FromISO8601UTC(p_s_value);
                    }
                }
                else if (p_s_type.Equals("System.TimeSpan"))
                {
                    /* handle default TimeSpan */
                    if (p_s_value.Equals("0001-01-01T00:00:00Z"))
                    {
                        o_foo = default(TimeSpan);
                    }
                    else
                    {
                        o_foo = ForestNETLib.Core.Helper.FromISO8601UTC(p_s_value).TimeOfDay;
                    }
                }
                else if (p_s_type.Equals("System.Decimal"))
                {
                    o_foo = Convert.ToDecimal(p_s_value, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                }
                else if (p_s_type.Equals("System.SByte"))
                {
                    o_foo = Convert.ToSByte(p_s_value);
                }
                else if (p_s_type.Equals("System.UInt16"))
                {
                    o_foo = Convert.ToUInt16(p_s_value);
                }
                else if (p_s_type.Equals("System.UInt32"))
                {
                    o_foo = Convert.ToUInt32(p_s_value);
                }
                else if (p_s_type.Equals("System.UInt64"))
                {
                    o_foo = Convert.ToUInt64(p_s_value);
                }
                else if (p_s_type.Equals("System.Object"))
                {
                    o_foo = (object)p_s_value;
                }
                else
                {
                    throw new ArgumentException("Invalid type[" + p_s_type + "] for value[" + p_s_value + "]");
                }
            }
            else
            { /* if value is empty */
                if (p_s_type.Equals("System.String"))
                {
                    o_foo = "";
                }
                else if (p_s_type.Equals("System.Byte"))
                {
                    if (this.b_returnArrayElementNullNotZero)
                    {
                        o_foo = null;
                    }
                    else
                    {
                        o_foo = (byte)0;
                    }
                }
                else if (p_s_type.Equals("System.Int16"))
                {
                    if (this.b_returnArrayElementNullNotZero)
                    {
                        o_foo = null;
                    }
                    else
                    {
                        o_foo = (short)0;
                    }
                }
                else if (p_s_type.Equals("System.Int32"))
                {
                    if (this.b_returnArrayElementNullNotZero)
                    {
                        o_foo = null;
                    }
                    else
                    {
                        o_foo = (int)0;
                    }
                }
                else if (p_s_type.Equals("System.Int64"))
                {
                    if (this.b_returnArrayElementNullNotZero)
                    {
                        o_foo = null;
                    }
                    else
                    {
                        o_foo = (long)0;
                    }
                }
                else if (p_s_type.Equals("System.Single"))
                {
                    if (this.b_returnArrayElementNullNotZero)
                    {
                        o_foo = null;
                    }
                    else
                    {
                        o_foo = 0.0f;
                    }
                }
                else if (p_s_type.Equals("System.Double"))
                {
                    if (this.b_returnArrayElementNullNotZero)
                    {
                        o_foo = null;
                    }
                    else
                    {
                        o_foo = 0.0d;
                    }
                }
                else if (p_s_type.Equals("System.Boolean"))
                {
                    if (this.b_returnArrayElementNullNotZero)
                    {
                        o_foo = null;
                    }
                    else
                    {
                        o_foo = false;
                    }
                }
                else if (p_s_type.Equals("System.Char"))
                {
                    if (this.b_returnArrayElementNullNotZero)
                    {
                        o_foo = null;
                    }
                    else
                    {
                        o_foo = (char)0;
                    }
                }
                else if (p_s_type.Equals("System.DateTime"))
                {
                    o_foo = null;
                }
                else if (p_s_type.Equals("System.TimeSpan"))
                {
                    o_foo = null;
                }
                else if (p_s_type.Equals("System.Decimal"))
                {
                    if (this.b_returnArrayElementNullNotZero)
                    {
                        o_foo = null;
                    }
                    else
                    {
                        o_foo = System.Decimal.Zero;
                    }
                }
                else if (p_s_type.Equals("System.SByte"))
                {
                    if (this.b_returnArrayElementNullNotZero)
                    {
                        o_foo = null;
                    }
                    else
                    {
                        o_foo = (sbyte)0;
                    }
                }
                else if (p_s_type.Equals("System.UInt16"))
                {
                    if (this.b_returnArrayElementNullNotZero)
                    {
                        o_foo = null;
                    }
                    else
                    {
                        o_foo = (ushort)0;
                    }
                }
                else if (p_s_type.Equals("System.UInt32"))
                {
                    if (this.b_returnArrayElementNullNotZero)
                    {
                        o_foo = null;
                    }
                    else
                    {
                        o_foo = (uint)0;
                    }
                }
                else if (p_s_type.Equals("System.UInt64"))
                {
                    if (this.b_returnArrayElementNullNotZero)
                    {
                        o_foo = null;
                    }
                    else
                    {
                        o_foo = (ulong)0;
                    }
                }
                else if (p_s_type.Equals("System.Object"))
                {
                    o_foo = null;
                }
            }

            return o_foo;
        }
    }
}
