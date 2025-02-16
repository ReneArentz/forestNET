namespace ForestNETLib.IO
{
    /// <summary>
    /// Interface for fixed length record class to use it as generic type in other classes
    /// </summary>
    public interface IFixedLengthRecord
    {
        public List<string> Unique { get; set; }
        public Dictionary<string, bool> OrderBy { get; set; }
        public int KnownOverallLength { get; set; }
        public bool AllowEmptyUniqueFields { get; set; }

        public void ClearUniqueTemp();
        public bool FieldExists(string p_s_field);
        public Object? GetFieldValue(string p_s_field);
        public void SetFieldValue(string p_s_field, Object? o_value);
        public string ReturnFields();
        public IFixedLengthRecord ReadFieldsFromString(string p_s_line);
        public string WriteFieldsToString();
    }

    /// <summary>
    /// Abstract fixed length record class - automatically detecting fields and handling fixed length of each field
    /// </summary>
    public abstract class FixedLengthRecord : IFixedLengthRecord
    {

        /* Delegates */

        /// <summary>
        /// delegate which can be implemented as standard transpose method to transpose a string value to an object of your choice
        /// </summary>
        public delegate Object? TransposeValueByRead(string? p_s_value);

        /// <summary>
        /// delegate which can be implemented as standard transpose method to transpose an object to a string value with a specific format
        /// </summary>
        public delegate string TransposeValueByWrite(Object? p_o_value, int p_i_length);

        /// <summary>
        /// delegate which can be implemented as standard transpose method to transpose a floating point number value to an object of your choice
        /// </summary>
        public delegate Object? TransposeValueByReadFPN(string? p_s_value, int p_i_positionDecimalSeparator);

        /// <summary>
        /// delegate which can be implemented as standard transpose method to transpose a floating point number to a string value with a specific format
        /// </summary>
        public delegate string TransposeValueByWriteFPN(Object? p_o_value, int p_i_amountDigits, int p_i_amountFractionDigits, string? p_s_decimalSeparator, string? p_s_groupSeparator);

        /* Fields */

        protected Dictionary<int, StructureElement>? Structure = null;
        private readonly Dictionary<string, List<Object?>> a_uniqueTemp = [];

        protected Type FLRImageClass;

        /* Properties */

        public List<string> Unique { get; set; } = [];
        public Dictionary<string, bool> OrderBy { get; set; } = [];
        public int KnownOverallLength { get; set; } = 0;
        public bool AllowEmptyUniqueFields { get; set; } = false;

        /* Methods */

        /// <summary>
        /// Fixed length record class constructor, initiating all values and properties and checks if all necessary restrictions are set
        /// </summary>
        /// <exception cref="ArgumentNullException">fixed length record image class, unique or order by have no values</exception>
        /// <exception cref="MissingFieldException">given field values in structure, unique or order by do not exists is current class</exception>
        public FixedLengthRecord()
        {
            /* first call init function to get initial values and properties from inherited class */
            this.Init();

            /* check fixed length record image class is not null */
            if (this.FLRImageClass == null)
            {
                throw new ArgumentNullException("You must specify a fixed length record image class within the Init-method.");
            }

            /* check structure is not empty */
            if ((this.Structure == null) || (this.Structure.Count < 1))
            {
                throw new ArgumentException("You must specify a structure within the Init-method.");
            }

            /* iterate each structure element */
            foreach (KeyValuePair<int, StructureElement> o_structureElement in this.Structure)
            {
                /* we have a structure element with a subtype */
                if (o_structureElement.Value.SubType != null)
                {
                    /* length of structure element must be '0' */
                    if (o_structureElement.Value.Length != 0)
                    {
                        throw new ArgumentException("Length of structure element with subtype is not '0', but is '" + o_structureElement.Value.Length + "'.");
                    }

                    /* check subtype amount value */
                    if (o_structureElement.Value.SubTypeAmount < 1)
                    {
                        throw new ArgumentException("Amount of structure element with subtype must be greater than '0', but is '" + o_structureElement.Value.SubTypeAmount + "'.");
                    }

                    /* create new instance of subtype in fixed length record class */
                    IFixedLengthRecord o_subtype = (IFixedLengthRecord?)Activator.CreateInstance(o_structureElement.Value.SubType) ?? throw new InvalidOperationException("Could not create subtype object from structure element '" + o_structureElement.Value.SubType.FullName + "', maybe it does not inherit FixedLengthRecord class");

                    /* add sum of amount of subtype objects for overall length */
                    this.KnownOverallLength += (o_subtype.KnownOverallLength * o_structureElement.Value.SubTypeAmount);

                    /* update subtype known overall length for later use */
                    if (o_structureElement.Value.SubTypeKnownOverallLength == -1)
                    {
                        o_structureElement.Value.SubTypeKnownOverallLength = o_subtype.KnownOverallLength;
                    }
                }

                /* sum up the field lengths */
                this.KnownOverallLength += o_structureElement.Value.Length;
            }

            /* check if each field in unique list really exists in current class */
            foreach (string s_unique in this.Unique)
            {
                /* it is possible that a unique constraint exists of multiple columns, separated by semicolon */
                if (s_unique.Contains(';'))
                {
                    string[] a_uniques = s_unique.Split(";");

                    /* iterate each unique field */
                    for (int i = 0; i < a_uniques.Length; i++)
                    {
                        /* check if unique field really exists */
                        if (!this.FieldExists(a_uniques[i]))
                        {
                            throw new MissingFieldException("Unique[" + s_unique + "] is not a field of current record class.");
                        }
                    }
                }
                else
                {
                    /* check if unique field really exists */
                    if (!this.FieldExists(s_unique))
                    {
                        throw new MissingFieldException("Unique[" + s_unique + "] is not a field of current record class.");
                    }
                }
            }

            /* check if each field in order by list really exists in current class */
            foreach (KeyValuePair<string, bool> o_orderByField in this.OrderBy)
            {
                if (!this.FieldExists(o_orderByField.Key))
                {
                    throw new MissingFieldException("OrderBy[" + o_orderByField.Key + "] is not a field of current record class.");
                }
            }

            if (ForestNETLib.Core.Global.IsILevel((byte)ForestNETLib.Log.Level.MASS)) ForestNETLib.Core.Global.ILogMass("created fixed length record object");
        }

        /// <summary>
        /// Abstract Init function so any class inheriting from FixedLengthRecord<T> must have this method
        /// declaring structure of the fixed length record, unique, order by and most of all flr image class
        /// </summary>
        protected abstract void Init();

        /// <summary>
        ///  clear unique temp map
        /// </summary>
        public void ClearUniqueTemp()
        {
            this.a_uniqueTemp.Clear();
        }

        /// <summary>
        /// Method to check if a field exists in current fixed length record class
        /// </summary>
        /// <param name="p_s_field">colum name</param>
        /// <returns>bool = true - field exist, false - field does not exist</returns>
        public bool FieldExists(string p_s_field)
        {
            /* return value */
            bool b_found = false;

            /* iterate each field of current fixed length record class */
            foreach (System.Reflection.FieldInfo o_fieldInfo in this.GetType().GetFields())
            {
                /* check if field starts with 'Field', but is not equal to 'Fields' */
                if ((o_fieldInfo.Name.StartsWith("Field")) && (o_fieldInfo.Name.CompareTo("Fields") != 0))
                {
                    /* field name without 'Field' prefix must match parameter value */
                    if (o_fieldInfo.Name.Substring(5).CompareTo(p_s_field) == 0)
                    {
                        /* set return value to true */
                        b_found = true;
                    }
                }
            }

            return b_found;
        }

        /// <summary>
        /// Method to retrieve field value of current fixed length record class
        /// </summary>
        /// <param name="p_s_field">field name, will be changed to 'Field' + p_s_field</param>
        /// <returns>unknown object type until method in use</returns>
        /// <exception cref="MissingFieldException">field does not exist</exception>
        /// <exception cref="FieldAccessException">cannot access field, must be public</exception>
        public Object? GetFieldValue(string p_s_field)
        {
            p_s_field = "Field" + p_s_field;

            if (ForestNETLib.Core.Global.IsILevel((byte)ForestNETLib.Log.Level.MASS)) ForestNETLib.Core.Global.ILogMass("get field value from '" + p_s_field + "'");

            /* get field info */
            System.Reflection.FieldInfo o_fieldInfo = this.GetType().GetField(p_s_field) ?? throw new NullReferenceException("Field '" + p_s_field + "' not found in class");
            /* get field type */
            Type? o_fieldType = o_fieldInfo.FieldType;
            /* get type name */
            string s_class = o_fieldType.FullName ?? "";

            /* variable for class type */
            Type o_classType = typeof(Object);

            /* switch class value by class name for casting */
            switch (s_class.ToLower())
            {
                case "bool":
                case "system.boolean":
                    o_classType = typeof(Boolean);
                    break;
                case "byte":
                case "unsignedbyte":
                case "system.byte":
                    o_classType = typeof(Byte);
                    break;
                case "sbyte":
                case "system.sbyte":
                    o_classType = typeof(SByte);
                    break;
                case "char":
                case "system.char":
                    o_classType = typeof(Char);
                    break;
                case "float":
                case "system.single":
                    o_classType = typeof(Single);
                    break;
                case "double":
                case "system.double":
                    o_classType = typeof(Double);
                    break;
                case "short":
                case "system.int16":
                    o_classType = typeof(Int16);
                    break;
                case "int":
                case "system.int32":
                    o_classType = typeof(Int32);
                    break;
                case "long":
                case "system.int64":
                    o_classType = typeof(Int64);
                    break;
                case "ushort":
                case "system.uint16":
                    o_classType = typeof(UInt16);
                    break;
                case "uint":
                case "system.uint32":
                    o_classType = typeof(UInt32);
                    break;
                case "ulong":
                case "system.uint64":
                    o_classType = typeof(UInt64);
                    break;
                case "string":
                case "system.string":
                    o_classType = typeof(string);
                    break;
                case "time":
                case "system.timespan":
                    o_classType = typeof(TimeSpan);
                    break;
                case "date":
                case "datetime":
                case "system.datetime":
                    o_classType = typeof(DateTime);
                    break;
                case "decimal":
                case "system.decimal":
                    o_classType = typeof(Decimal);
                    break;
            }

            bool b_handleTimeSpan = false;

            /* check for DateTime or TimeSpan */
            if (o_classType == typeof(Object))
            {
                if ((s_class.Contains("date", StringComparison.CurrentCultureIgnoreCase)) || (s_class.Contains("datetime", StringComparison.CurrentCultureIgnoreCase)))
                {
                    /* set class type for DateTime */
                    o_classType = typeof(DateTime);
                }
                else if ((s_class.Contains("time", StringComparison.CurrentCultureIgnoreCase)) || (s_class.Contains("timespan", StringComparison.CurrentCultureIgnoreCase)))
                {
                    /* set flag for TimeSpan */
                    b_handleTimeSpan = true;
                }
            }

            /* get field object value */
            Object? o_foo = this.GetType().GetField(p_s_field)?.GetValue(this);

            /* handle cast */
            if (o_foo != null)
            {
                if (!b_handleTimeSpan)
                {
                    /* do cast only, if we have not a generic list or array as field type */
                    if (!(o_fieldType.IsGenericType || o_fieldType.IsArray))
                    {
                        /* cast object of field to target class type */
                        o_foo = Convert.ChangeType(o_foo, o_classType);
                    }
                }
                else
                {
                    /* we must handle TimeSpan, because it has not IConvertible */
                    System.ComponentModel.TypeConverter o_typeConverter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(TimeSpan));

                    try
                    {
                        /* use type convert instead of Convert.ChangeType */
                        o_foo = o_typeConverter.ConvertFrom(o_foo.ToString() ?? "null");
                    }
                    catch (Exception o_exc)
                    {
                        throw new InvalidCastException("Can not convert TimeSpan with type converter: " + o_exc);
                    }
                }
            }

            /* return casted object */
            return o_foo;
        }

        /// <summary>
        /// Method to set a field value of current fixed record record class
        /// </summary>
        /// <param name="p_s_field">field name, will be changed to 'Field' + p_s_field</param>
        /// <param name="o_value">object value which will be set as field value</param>
        /// <exception cref="MissingFieldException">field does not exist</exception>
        /// <exception cref="FieldAccessException">cannot access field, must be public</exception>
        public void SetFieldValue(string p_s_field, Object? o_value)
        {
            p_s_field = "Field" + p_s_field;

            if (o_value != null)
            {
                /* get parameter value type */
                string? s_valueType = o_value.GetType().FullName;

                if (ForestNETLib.Core.Global.IsILevel((byte)ForestNETLib.Log.Level.MASS)) ForestNETLib.Core.Global.ILogMass("set field value for: '" + p_s_field + "'" + "\t\tfield type: " + GetType().GetField(p_s_field)?.GetType().FullName + "\t\tvalue type: " + s_valueType);

                if ((s_valueType != null) && (s_valueType.Contains("string", StringComparison.CurrentCultureIgnoreCase)))
                {
                    string? s_foo = o_value.ToString();

                    /* recognize empty null value */
                    if ((ForestNETLib.Core.Helper.IsStringEmpty(s_foo)) || ((s_foo != null) && s_foo.Equals("NULL")))
                    {
                        o_value = null;
                    }
                }
            }

            /* set field value, accessing 'this' class and field with field name */
            GetType().GetField(p_s_field)?.SetValue(this, o_value);
        }

        /// <summary>
        /// Easy method to return all fields with their values of current fixed length record class
        /// </summary>
        /// <returns>a string line of all fields with their values "field_name = field_value|"</returns>
        public string ReturnFields()
        {
            string s_foo = "";

            /* iterate each field of current fixed length record class */
            foreach (System.Reflection.FieldInfo o_fieldInfo in this.GetType().GetFields())
            {
                /* check if field starts with 'Field', but is not equal to 'Fields' */
                if ((o_fieldInfo.Name.StartsWith("Field")) && (o_fieldInfo.Name.CompareTo("Fields") != 0))
                {
                    try
                    {
                        string s_object;

                        if ((o_fieldInfo.FieldType.IsGenericType) && (o_fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>)))
                        {
                            /* get generic list type of first element of field type */
                            Type o_genericType = o_fieldInfo.FieldType.GenericTypeArguments[0];
                            /* create generic list type */
                            Type o_genericListType = typeof(List<>).MakeGenericType(o_genericType);
                            /* create instance of generic list with generic list type and handle it as IList */
                            System.Collections.IList o_genericList = Activator.CreateInstance(o_genericListType) as System.Collections.IList ?? throw new NullReferenceException("Could not create a generic list with type '" + o_genericListType.FullName + "'");

                            /* get generic list from field value */
                            o_genericList = (System.Collections.IList)(o_fieldInfo.GetValue(this) ?? new List<IFixedLengthRecord>());

                            s_object = "";

                            /* iterate each generic list element to retrieve the fields */
                            foreach (IFixedLengthRecord o_listElement in o_genericList)
                            {
                                s_object += "{" + o_listElement.ReturnFields() + "}, ";
                            }

                            s_object = "[" + s_object.Substring(0, s_object.Length - 2) + "]";
                        }
                        else
                        {
                            /* set field string value to 'NULL', in case value is null */
                            s_object = o_fieldInfo.GetValue(this)?.ToString() ?? "NULL";
                        }

                        /* add field name and its value to return string */
                        s_foo += o_fieldInfo.Name.Substring(5) + " = " + s_object + "|";
                    }
                    catch (Exception)
                    {
                        /* just continue if field name or field value cannot be retrieved */
                        s_foo += o_fieldInfo.Name.Substring(5) + " = COULD_NOT_RETRIEVE|";
                    }
                }
            }

            return s_foo;
        }

        /// <summary>
        /// Assume field values of a string line to fields of current fixed length record class and its record image
        /// </summary>
        /// <param name="p_s_line">fields and their values in a string line</param>
        /// <exception cref="ArgumentException">line length does not match known overall length of fixed length record</exception>
        /// <exception cref="InvalidCastException">we could not parse a value to predicted field object type</exception>
        /// <exception cref="System.Reflection.TargetInvocationException">if the underlying constructor throws an exception</exception>
        /// <exception cref="MissingFieldException">field does not exist</exception>
        /// <exception cref="FieldAccessException">cannot access field, must be public</exception>
        /// <exception cref="MissingMethodException">could not retrieve declared constructor</exception>
        /// <exception cref="TypeLoadException">class cannot be located</exception>
        /// <exception cref="InvalidOperationException">field value for unique field already exists</exception>
        public IFixedLengthRecord ReadFieldsFromString(string p_s_line)
        {
            /* create new instance of fixed length record class */
            IFixedLengthRecord o_temp = (IFixedLengthRecord?)Activator.CreateInstance(this.FLRImageClass) ?? throw new InvalidOperationException("Could not create object from flr image class '" + this.FLRImageClass.FullName + "', maybe it does not inherit FixedLengthRecord class");

            /* check if line length matches known overall length */
            if (p_s_line.Length != this.KnownOverallLength)
            {
                throw new ArgumentException("Line length parameter '" + p_s_line.Length + "' does not match known overall length of all flr fields '" + this.KnownOverallLength + "'");
            }

            /* check structure is not empty */
            if ((this.Structure == null) || (this.Structure.Count < 1))
            {
                throw new ArgumentException("You must specify a structure within the Init-method.");
            }

            int i_position = 0;

            /* iterate each structure element */
            foreach (KeyValuePair<int, StructureElement> o_structureElement in this.Structure)
            {
                /* check if we have a constant field */
                if (!ForestNETLib.Core.Helper.IsStringEmpty(o_structureElement.Value.Constant))
                {
                    /* increase position pointer */
                    i_position += o_structureElement.Value.Length;

                    /* go on with next structure element */
                    continue;
                }

                /* we have a structure element with a subtype -> generic list */
                if (o_structureElement.Value.SubType != null)
                {
                    /* check subtype known overall length */
                    if (o_structureElement.Value.SubTypeKnownOverallLength <= 0)
                    {
                        throw new ArgumentException("Subtype known overall length has not been set, it is '" + o_structureElement.Value.SubTypeKnownOverallLength + "'.");
                    }

                    /* get element type name of generic list */
                    string s_elementTypeName = o_structureElement.Value.SubType.AssemblyQualifiedName ?? throw new NullReferenceException("Could not get element type name with '" + o_structureElement.Value.SubType.GetType() + "'");
                    /* get element type of first element of object list */
                    Type o_elementType = Type.GetType(s_elementTypeName) ?? throw new NullReferenceException("Could not retrieve object type with '" + s_elementTypeName + "'");
                    /* create generic list type with element type */
                    Type o_genericListType = typeof(List<>).MakeGenericType(o_elementType);
                    /* create instance of generic list with generic list type and handle it as IList */
                    System.Collections.IList o_genericList = Activator.CreateInstance(o_genericListType) as System.Collections.IList ?? throw new NullReferenceException("Could not create a generic list with type '" + o_genericListType.FullName + "'");

                    /* iterate expected amount of generic list field values */
                    for (int i = 0; i < o_structureElement.Value.SubTypeAmount; i++)
                    {
                        /* get generic list field value out of line parameter */
                        string s_value = p_s_line.Substring(i_position, o_structureElement.Value.SubTypeKnownOverallLength);
                        /* increase position pointer */
                        i_position += o_structureElement.Value.SubTypeKnownOverallLength;

                        /* create instance of subtype object */
                        IFixedLengthRecord o_subtypeInstance = (IFixedLengthRecord?)Activator.CreateInstance(o_structureElement.Value.SubType) ?? throw new InvalidOperationException("Could not create subtype object from structure element '" + o_structureElement.Value.SubType.FullName + "', maybe it does not inherit FixedLengthRecord class");
                        /* read fields from substring line for subtype */
                        o_subtypeInstance = o_subtypeInstance.ReadFieldsFromString(s_value);

                        /* check unique fields of subtype instance */
                        if (o_subtypeInstance.Unique.Count > 0)
                        {
                            /* iterate unique keys */
                            foreach (string s_unique in o_subtypeInstance.Unique)
                            {
                                /* retrieve unique field value variable */
                                Object? o_fieldValue = o_subtypeInstance.GetFieldValue(s_unique);
                                /* unique key name for subtype */
                                string s_uniqueName = "__Subtype__" + s_unique;

                                /* empty field values in unique fields are allowed and can be skipped */
                                if (o_subtypeInstance.AllowEmptyUniqueFields)
                                {
                                    /* field value is null or an empty string */
                                    if ((o_fieldValue == null) || (o_fieldValue.ToString()?.Trim().Length < 1))
                                    {
                                        continue;
                                    }

                                    try
                                    {
                                        /* field value can be parsed to int and is equal to zero */
                                        if (Convert.ToInt32((o_fieldValue.ToString() ?? "")) == 0)
                                        {
                                            continue;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        /* nothing to do */
                                    }
                                }

                                /* unique values are stored within temp map for current field */
                                if (this.a_uniqueTemp.ContainsKey(s_uniqueName))
                                {
                                    /* check if field value already exists */
                                    if (this.a_uniqueTemp[s_uniqueName].Contains(o_fieldValue))
                                    {
                                        string s_foo = s_unique;

                                        if (s_foo.StartsWith("__Subtype__"))
                                        {
                                            s_foo = s_foo.Substring(11);
                                        }

                                        throw new InvalidOperationException("field value for unique field '" + s_foo + "' already exists");
                                    }
                                    else
                                    {
                                        /* add field value to unique temp list */
                                        this.a_uniqueTemp[s_uniqueName].Add(o_fieldValue);
                                    }
                                }
                                else
                                { /* no values for this fields stored so far */
                                    /* create unique temp list for field and add value */
                                    this.a_uniqueTemp[s_uniqueName] =
                                    [
                                        o_fieldValue
                                    ];
                                }
                            }
                        }

                        /* add instance to generic list */
                        o_genericList.Add(o_subtypeInstance);
                    }

                    /* clear all unique temp values for subtypes */
                    List<string> a_keyList = [.. this.a_uniqueTemp.Keys];

                    foreach (string s_key in a_keyList)
                    {
                        if (s_key.StartsWith("__Subtype__"))
                        {
                            this.a_uniqueTemp.Remove(s_key);
                        }
                    }

                    /* set generic list value */
                    o_temp.SetFieldValue(o_structureElement.Value.Field ?? "null", o_genericList);
                }
                else /* we have a normal structure element */
                {
                    /* field value variable */
                    Object? o_fieldValue = null;
                    /* get field value out of line parameter */
                    string s_value = p_s_line.Substring(i_position, o_structureElement.Value.Length);
                    /* increase position pointer */
                    i_position += o_structureElement.Value.Length;

                    /* check if we have a transpose method available */
                    if ((o_structureElement.Value.ReadTranspose == null) && (o_structureElement.Value.ReadFPNTranspose == null))
                    {
                        throw new MissingMethodException("Structure element[" + o_structureElement.Value.Field + "] has no read transpose method pointer");
                    }

                    /* check if field does not contain only white spaces or only '0' */
                    if ((!ForestNETLib.Core.Helper.IsStringEmpty(s_value.Trim())) && (!ForestNETLib.Core.Helper.MatchesRegex(s_value, "^[0]+$")))
                    {
                        if (ForestNETLib.Core.Global.IsILevel((byte)ForestNETLib.Log.Level.MASS)) ForestNETLib.Core.Global.ILogMass("field '" + o_structureElement.Value.Field + "(" + i_position + ".." + (i_position + o_structureElement.Value.Length) + ")'\t\tunique '" + (this.Unique.Contains(o_structureElement.Value.Field ?? "") ? "yes" : "no ") + "'\t\tvalue '" + s_value + "'\t\tconverted value '" + ((o_structureElement.Value.ReadTranspose == null) ? o_structureElement.Value.ReadFPNTranspose?.Invoke(s_value, o_structureElement.Value.PositionDecimalSeparator) : o_structureElement.Value.ReadTranspose.Invoke(s_value)) + "'");

                        /* get field value */
                        o_fieldValue = ((o_structureElement.Value.ReadTranspose == null) ? o_structureElement.Value.ReadFPNTranspose?.Invoke(s_value, o_structureElement.Value.PositionDecimalSeparator) : o_structureElement.Value.ReadTranspose.Invoke(s_value));
                    }
                    else if ((!ForestNETLib.Core.Helper.IsStringEmpty(s_value.Trim())) && (ForestNETLib.Core.Helper.MatchesRegex(s_value, "^[0]+$")))
                    { /* value has only character '0' */
                        /* get class type */
                        Type o_type = this.GetType().GetField("Field" + o_structureElement.Value.Field)?.GetType() ?? throw new MissingFieldException("Field '" + o_structureElement.Value.Field + "' could not be found to retrieve");

                        /* set null if we have a date/time object */
                        if ((o_type.FullName != null) && ((o_type.FullName.Contains("datetime", StringComparison.CurrentCultureIgnoreCase)) || (o_type.FullName.Contains("timespan", StringComparison.CurrentCultureIgnoreCase))))
                        {
                            if (ForestNETLib.Core.Global.IsILevel((byte)ForestNETLib.Log.Level.MASS)) ForestNETLib.Core.Global.ILogMass("field '" + o_structureElement.Value.Field + "(" + i_position + ".." + (i_position + o_structureElement.Value.Length) + ")'\t\tunique '" + (this.Unique.Contains(o_structureElement.Value.Field ?? "") ? "yes" : "no ") + "'\t\tvalue '" + s_value + "'\t\tconverted value 'null'");

                            /* keep field value as 'null' */

                        }
                        else
                        { /* proceed normal */
                            if (ForestNETLib.Core.Global.IsILevel((byte)ForestNETLib.Log.Level.MASS)) ForestNETLib.Core.Global.ILogMass("field '" + o_structureElement.Value.Field + "(" + i_position + ".." + (i_position + o_structureElement.Value.Length) + ")'\t\tunique '" + (this.Unique.Contains(o_structureElement.Value.Field ?? "") ? "yes" : "no ") + "'\t\tvalue '" + s_value + "'\t\tconverted value '" + ((o_structureElement.Value.ReadTranspose == null) ? o_structureElement.Value.ReadFPNTranspose?.Invoke(s_value, o_structureElement.Value.PositionDecimalSeparator) : o_structureElement.Value.ReadTranspose.Invoke(s_value)) + "'");

                            /* get field value */
                            o_fieldValue = ((o_structureElement.Value.ReadTranspose == null) ? o_structureElement.Value.ReadFPNTranspose?.Invoke(s_value, o_structureElement.Value.PositionDecimalSeparator) : o_structureElement.Value.ReadTranspose.Invoke(s_value));
                        }
                    }
                    else
                    { /* field value contains only white spaces, we can set it to null */
                        if (ForestNETLib.Core.Global.IsILevel((byte)ForestNETLib.Log.Level.MASS)) ForestNETLib.Core.Global.ILogMass("field '" + o_structureElement.Value.Field + "(" + i_position + ".." + (i_position + o_structureElement.Value.Length) + ")'\t\tunique '" + (this.Unique.Contains(o_structureElement.Value.Field ?? "") ? "yes" : "no ") + "'\t\tvalue '" + s_value + "'\t\tconverted value 'null'");

                        /* keep field value as 'null' */
                    }

                    /* flag to skip unique fields check */
                    bool b_skipUniqueCheck = false;

                    /* empty field values in unique fields are allowed and can be skipped */
                    if (this.AllowEmptyUniqueFields)
                    {
                        /* field value is null or an empty string */
                        if ((o_fieldValue == null) || (o_fieldValue.ToString()?.Trim().Length < 1))
                        {
                            b_skipUniqueCheck = true;
                        }

                        try
                        {
                            /* field value can be parsed to int and is equal to zero */
                            if (Convert.ToInt32((o_fieldValue?.ToString() ?? "")) == 0)
                            {
                                b_skipUniqueCheck = true;
                            }
                        }
                        catch (Exception)
                        {
                            /* nothing to do */
                        }
                    }

                    /* check unique fields */
                    if ((!b_skipUniqueCheck) && (o_structureElement.Value.Field != null) && (this.Unique.Contains(o_structureElement.Value.Field)))
                    {
                        /* unique values are stored within temp map for current field */
                        if (this.a_uniqueTemp.ContainsKey(o_structureElement.Value.Field))
                        {
                            /* check if field value already exists */
                            if (this.a_uniqueTemp[o_structureElement.Value.Field].Contains(o_fieldValue))
                            {
                                string s_foo = o_structureElement.Value.Field;

                                if (s_foo.StartsWith("__Subtype__"))
                                {
                                    s_foo = s_foo.Substring(11);
                                }

                                throw new InvalidOperationException("field value for unique field '" + s_foo + "' already exists");
                            }
                            else
                            {
                                /* add field value to unique temp list */
                                this.a_uniqueTemp[o_structureElement.Value.Field].Add(o_fieldValue);
                            }
                        }
                        else
                        { /* no values for this fields stored so far */
                            /* create unique temp list for field and add value */
                            this.a_uniqueTemp[o_structureElement.Value.Field] =
                            [
                                o_fieldValue
                            ];
                        }
                    }

                    /* set field value */
                    o_temp.SetFieldValue(o_structureElement.Value.Field ?? "null", o_fieldValue);
                }
            }

            return o_temp;
        }

        /// <summary>
        /// Writes all fields of fixed length record class into one string line, based on transpose write methods of structure elements and it's properties
        /// </summary>
        /// <returns>one string line with all field values</returns>
        /// <exception cref="MissingFieldException">field does not exist</exception>
        /// <exception cref="FieldAccessException">cannot access field, must be public</exception>
        public string WriteFieldsToString()
        {
            string s_foo = "";

            /* check structure is not empty */
            if ((this.Structure == null) || (this.Structure.Count < 1))
            {
                throw new ArgumentException("You must specify a structure within the Init-method.");
            }

            /* iterate each structure element */
            foreach (KeyValuePair<int, StructureElement> o_structureElement in this.Structure)
            {
                /* we have a structure element with a subtype -> generic list */
                if (o_structureElement.Value.SubType != null)
                {
                    /* check subtype known overall length */
                    if (o_structureElement.Value.SubTypeKnownOverallLength <= 0)
                    {
                        throw new ArgumentException("Subtype known overall length has not been set, it is '" + o_structureElement.Value.SubTypeKnownOverallLength + "'.");
                    }

                    /* get field name value */
                    string s_field = o_structureElement.Value.Field ?? throw new ArgumentException("Field property of structure element has no value");
                    /* get generic list as object */
                    Object? o_foo = this.GetFieldValue(s_field) ?? throw new NullReferenceException("Generic list field value is null");
                    /* cast current object as list with unknown generic type */
                    List<IFixedLengthRecord> a_objects = [.. ((System.Collections.IEnumerable)o_foo).Cast<IFixedLengthRecord>()];

                    /* iterate all configured amount of subtype instances */
                    for (int i = 0; i < o_structureElement.Value.SubTypeAmount; i++)
                    {
                        /* check if we have an instance in our iteration */
                        if (i < a_objects.Count)
                        {
                            /* add list element to current line of fixed length record */
                            s_foo += a_objects[i].WriteFieldsToString();
                        }
                        else /* we have empty instances */
                        {
                            /* create new instance of subtype in fixed length record class */
                            IFixedLengthRecord o_subtype = (IFixedLengthRecord?)Activator.CreateInstance(o_structureElement.Value.SubType) ?? throw new InvalidOperationException("Could not create subtype object from structure element '" + o_structureElement.Value.SubType.FullName + "', maybe it does not inherit FixedLengthRecord class");

                            /* add empty list element to current line of fixed length record */
                            s_foo += o_subtype.WriteFieldsToString();
                        }
                    }
                }
                else if (!ForestNETLib.Core.Helper.IsStringEmpty(o_structureElement.Value.Constant)) /* check if we have a constant field */
                {
                    /* add constant to line */
                    s_foo += o_structureElement.Value.Constant;
                }
                else if ((o_structureElement.Value.WriteTranspose == null) && (o_structureElement.Value.WriteFPNTranspose != null))
                {
                    /* get field name value */
                    string s_field = o_structureElement.Value.Field ?? throw new ArgumentException("Field property of structure element has no value");

                    /* use transpose method to get floating point number field value */
                    s_foo += o_structureElement.Value.WriteFPNTranspose.Invoke(
                        this.GetFieldValue(s_field),
                        o_structureElement.Value.AmountDigits,
                        o_structureElement.Value.AmountFractionDigits,
                        o_structureElement.Value.DecimalSeparator,
                        o_structureElement.Value.GroupSeparator
                    );
                }
                else if (o_structureElement.Value.WriteTranspose != null)
                {
                    /* get field name value */
                    string s_field = o_structureElement.Value.Field ?? throw new ArgumentException("Field property of structure element has no value");

                    /* use transpose method to get field value */
                    s_foo += o_structureElement.Value.WriteTranspose.Invoke(this.GetFieldValue(s_field), o_structureElement.Value.Length);
                }
            }

            return s_foo;
        }

        /* Internal Classes */

        /// <summary>
        /// Encapsulation of a flr's structure element
        /// </summary>
        public class StructureElement
        {

            /* Fields */

            /* Properties */

            public string? Constant { get; set; }
            public string? Field { get; set; }
            public int Length { get; set; }
            public TransposeValueByRead? ReadTranspose { get; set; }
            public TransposeValueByWrite? WriteTranspose { get; set; }
            public TransposeValueByReadFPN? ReadFPNTranspose { get; set; }
            public TransposeValueByWriteFPN? WriteFPNTranspose { get; set; }
            public int PositionDecimalSeparator { get; set; }
            public int AmountDigits { get; set; }
            public int AmountFractionDigits { get; set; }
            public string? DecimalSeparator { get; set; }
            public string? GroupSeparator { get; set; }
            public int SubTypeAmount { get; set; }
            public Type? SubType { get; set; }
            public int SubTypeKnownOverallLength { get; set; } = -1;

            /* Methods */

            /// <summary>
            /// StructureElement constructor
            /// </summary>
            /// <param name="p_s_constant">constant value within fixed length record</param>
            public StructureElement(string p_s_constant) :
                this(null, p_s_constant.Length, null, null, null, null, -1, -1, -1, null, null, p_s_constant, -1, null)
            {

            }

            /// <summary>
            /// StructureElement constructor
            /// </summary>
            /// <param name="p_s_field">name of field</param>
            /// <param name="p_i_length">length of field within flr</param>
            public StructureElement(string p_s_field, int p_i_length) :
                this(p_s_field, p_i_length, null, null, null, null, -1, -1, -1, null, null, null, -1, null)
            {

            }

            /// <summary>
            /// StructureElement constructor
            /// </summary>
            /// <param name="p_s_field">name of field</param>
            /// <param name="p_i_amountSubtypes">amount of subtype objects within flr</param>
            /// <param name="p_o_subType">subtype class</param>
            public StructureElement(string p_s_field, int p_i_amountSubtypes, Type p_o_subType) :
                this(p_s_field, 0, null, null, null, null, -1, -1, -1, null, null, null, p_i_amountSubtypes, p_o_subType)
            {

            }

            /// <summary>
            /// StructureElement constructor
            /// </summary>
            /// <param name="p_s_field">name of field</param>
            /// <param name="p_i_length">length of field within flr</param>
            /// <param name="p_del_readTranspose">method delegate to transpose a string value to an object of your choice</param>
            /// <param name="p_del_writeTranspose">method delegate to transpose an object to a string value with a specific format</param>
            public StructureElement(string p_s_field, int p_i_length, TransposeValueByRead p_del_readTranspose, TransposeValueByWrite p_del_writeTranspose) :
                this(p_s_field, p_i_length, p_del_readTranspose, p_del_writeTranspose, null, null, -1, -1, -1, null, null, null, -1, null)
            {

            }

            /// <summary>
            /// StructureElement constructor
            /// </summary>
            /// <param name="p_s_field">name of field</param>
            /// <param name="p_i_length">length of field within flr</param>
            /// <param name="p_del_readFPNTranspose">method delegate to transpose a string value to a floating point number</param>
            /// <param name="p_del_writeFPNTranspose">method delegate to transpose a floating point number to a string value with a specific format</param>
            public StructureElement(string p_s_field, int p_i_length, TransposeValueByReadFPN p_del_readFPNTranspose, TransposeValueByWriteFPN p_del_writeFPNTranspose) :
                this(p_s_field, p_i_length, null, null, p_del_readFPNTranspose, p_del_writeFPNTranspose, -1, -1, -1, null, null, null, -1, null)
            {

            }

            /// <summary>
            /// StructureElement constructor
            /// </summary>
            /// <param name="p_s_field">name of field</param>
            /// <param name="p_i_length">length of field within flr</param>
            /// <param name="p_del_readFPNTranspose">method delegate to transpose a string value to a floating point number</param>
            /// <param name="p_del_writeFPNTranspose">method delegate to transpose a floating point number to a string value with a specific format</param>
            /// <param name="p_i_positionDecimalSeparator">position of decimal separator within a floating point number string (read only)</param>
            public StructureElement(string p_s_field, int p_i_length, TransposeValueByReadFPN p_del_readFPNTranspose, TransposeValueByWriteFPN p_del_writeFPNTranspose, int p_i_positionDecimalSeparator) :
                this(p_s_field, p_i_length, null, null, p_del_readFPNTranspose, p_del_writeFPNTranspose, p_i_positionDecimalSeparator, -1, -1, null, null, null, -1, null)
            {

            }

            /// <summary>
            /// StructureElement constructor
            /// </summary>
            /// <param name="p_s_field">name of field</param>
            /// <param name="p_i_length">length of field within flr</param>
            /// <param name="p_del_readFPNTranspose">method delegate to transpose a string value to a floating point number</param>
            /// <param name="p_del_writeFPNTranspose">method delegate to transpose a floating point number to a string value with a specific format</param>
            /// <param name="p_i_amountDigits">amount of digits for a floating point number (write only)</param>
            /// <param name="p_i_amountFractionDigits">amount of fractional digits for a floating point number (write only)</param>
            public StructureElement(string p_s_field, int p_i_length, TransposeValueByReadFPN p_del_readFPNTranspose, TransposeValueByWriteFPN p_del_writeFPNTranspose, int p_i_amountDigits, int p_i_amountFractionDigits) :
                this(p_s_field, p_i_length, null, null, p_del_readFPNTranspose, p_del_writeFPNTranspose, p_i_amountDigits, p_i_amountDigits, p_i_amountFractionDigits, null, null, null, -1, null)
            {

            }

            /// <summary>
            /// StructureElement constructor
            /// </summary>
            /// <param name="p_s_field">name of field</param>
            /// <param name="p_i_length">length of field within flr</param>
            /// <param name="p_del_readFPNTranspose">method delegate to transpose a string value to a floating point number</param>
            /// <param name="p_del_writeFPNTranspose">method delegate to transpose a floating point number to a string value with a specific format</param>
            /// <param name="p_i_positionDecimalSeparator">position of decimal separator within a floating point number string (read only)</param>
            /// <param name="p_i_amountDigits">amount of digits for a floating point number (write only)</param>
            /// <param name="p_i_amountFractionDigits">amount of fractional digits for a floating point number (write only)</param>
            public StructureElement(string p_s_field, int p_i_length, TransposeValueByReadFPN p_del_readFPNTranspose, TransposeValueByWriteFPN p_del_writeFPNTranspose, int p_i_positionDecimalSeparator, int p_i_amountDigits, int p_i_amountFractionDigits) :
                this(p_s_field, p_i_length, null, null, p_del_readFPNTranspose, p_del_writeFPNTranspose, p_i_positionDecimalSeparator, p_i_amountDigits, p_i_amountFractionDigits, null, null, null, -1, null)
            {

            }

            /// <summary>
            /// StructureElement constructor, use '$' for decimal and group separator to use system separators
            /// </summary>
            /// <param name="p_s_field">name of field</param>
            /// <param name="p_i_length">length of field within flr</param>
            /// <param name="p_del_readFPNTranspose">method delegate to transpose a string value to a floating point number</param>
            /// <param name="p_del_writeFPNTranspose">method delegate to transpose a floating point number to a string value with a specific format</param>
            /// <param name="p_i_amountDigits">amount of digits for a floating point number (write only)</param>
            /// <param name="p_i_amountFractionDigits">amount of fractional digits for a floating point number (write only)</param>
            /// <param name="p_s_decimalSeparator">string for decimal separator, use '$' for system settings</param>
            /// <param name="p_s_groupSeparator">string for group separator, use '$' for system settings</param>
            public StructureElement(string p_s_field, int p_i_length, TransposeValueByReadFPN p_del_readFPNTranspose, TransposeValueByWriteFPN p_del_writeFPNTranspose, int p_i_amountDigits, int p_i_amountFractionDigits, string? p_s_decimalSeparator, string? p_s_groupSeparator) :
                this(p_s_field, p_i_length, null, null, p_del_readFPNTranspose, p_del_writeFPNTranspose, p_i_amountDigits, p_i_amountDigits, p_i_amountFractionDigits, p_s_decimalSeparator, p_s_groupSeparator, null, -1, null)
            {

            }

            /// <summary>
            /// StructureElement constructor, use '$' for decimal and group separator to use system separators
            /// </summary>
            /// <param name="p_s_field">name of field</param>
            /// <param name="p_i_length">length of field within flr</param>
            /// <param name="p_del_readFPNTranspose">method delegate to transpose a string value to a floating point number</param>
            /// <param name="p_del_writeFPNTranspose">method delegate to transpose a floating point number to a string value with a specific format</param>
            /// <param name="p_i_positionDecimalSeparator">position of decimal separator within a floating point number string (read only)</param>
            /// <param name="p_i_amountDigits">amount of digits for a floating point number (write only)</param>
            /// <param name="p_i_amountFractionDigits">amount of fractional digits for a floating point number (write only)</param>
            /// <param name="p_s_decimalSeparator">string for decimal separator, use '$' for system settings</param>
            /// <param name="p_s_groupSeparator">string for group separator, use '$' for system settings</param>
            public StructureElement(string p_s_field, int p_i_length, TransposeValueByReadFPN p_del_readFPNTranspose, TransposeValueByWriteFPN p_del_writeFPNTranspose, int p_i_positionDecimalSeparator, int p_i_amountDigits, int p_i_amountFractionDigits, string? p_s_decimalSeparator, string? p_s_groupSeparator) :
                this(p_s_field, p_i_length, null, null, p_del_readFPNTranspose, p_del_writeFPNTranspose, p_i_positionDecimalSeparator, p_i_amountDigits, p_i_amountFractionDigits, p_s_decimalSeparator, p_s_groupSeparator, null, -1, null)
            {

            }

            /// <summary>
            /// StructureElement constructor, use '$' for decimal and group separator to use system separators
            /// </summary>
            /// <param name="p_s_field">name of field</param>
            /// <param name="p_i_length">length of field within flr</param>
            /// <param name="p_del_readTranspose">method delegate to transpose a string value to an object of your choice</param>
            /// <param name="p_del_writeTranspose">method delegate to transpose an object to a string value with a specific format</param>
            /// <param name="p_del_readFPNTranspose">method delegate to transpose a string value to a floating point number</param>
            /// <param name="p_del_writeFPNTranspose">method delegate to transpose a floating point number to a string value with a specific format</param>
            /// <param name="p_i_positionDecimalSeparator">position of decimal separator within a floating point number string (read only)</param>
            /// <param name="p_i_amountDigits">amount of digits for a floating point number (write only)</param>
            /// <param name="p_i_amountFractionDigits">amount of fractional digits for a floating point number (write only)</param>
            /// <param name="p_s_decimalSeparator">string for decimal separator, use '$' for system settings</param>
            /// <param name="p_s_groupSeparator">string for group separator, use '$' for system settings</param>
            /// <param name="p_s_constant">constant value within fixed length record</param>
            public StructureElement(
                string? p_s_field,
                int p_i_length,
                TransposeValueByRead? p_del_readTranspose,
                TransposeValueByWrite? p_del_writeTranspose,
                TransposeValueByReadFPN? p_del_readFPNTranspose,
                TransposeValueByWriteFPN? p_del_writeFPNTranspose,
                int p_i_positionDecimalSeparator,
                int p_i_amountDigits,
                int p_i_amountFractionDigits,
                string? p_s_decimalSeparator,
                string? p_s_groupSeparator,
                string? p_s_constant,
                int p_i_subTypeAmount,
                Type? p_o_subType
            )
            {
                this.Field = p_s_field;
                this.Length = p_i_length;
                this.ReadTranspose = p_del_readTranspose;
                this.WriteTranspose = p_del_writeTranspose;
                this.ReadFPNTranspose = p_del_readFPNTranspose;
                this.WriteFPNTranspose = p_del_writeFPNTranspose;
                this.PositionDecimalSeparator = p_i_positionDecimalSeparator;
                this.AmountDigits = p_i_amountDigits;
                this.AmountFractionDigits = p_i_amountFractionDigits;
                this.DecimalSeparator = p_s_decimalSeparator;
                this.GroupSeparator = p_s_groupSeparator;
                this.Constant = p_s_constant;
                this.SubTypeAmount = p_i_subTypeAmount;
                this.SubType = p_o_subType;
            }

            //// <summary>
            /// Checks if structure element object is equal to another structure element object
            /// </summary>
            /// <param name="p_o_structureElement">structure element object for comparison</param>
            /// <returns>true - equal, false - not equal</returns>
            public bool IsEqual(StructureElement p_o_structureElement)
            {
                if (
                    ((this.Constant ?? "").Equals(p_o_structureElement.Constant ?? "")) &&
                    ((this.Field ?? "").Equals(p_o_structureElement.Field ?? "")) &&
                    (this.Length == p_o_structureElement.Length) &&
                    (this.PositionDecimalSeparator == p_o_structureElement.PositionDecimalSeparator) &&
                    (this.AmountDigits == p_o_structureElement.AmountDigits) &&
                    (this.AmountFractionDigits == p_o_structureElement.AmountFractionDigits) &&
                    ((this.DecimalSeparator ?? "").Equals(p_o_structureElement.DecimalSeparator ?? "")) &&
                    ((this.GroupSeparator ?? "").Equals(p_o_structureElement.GroupSeparator ?? "")) &&
                    (this.SubTypeAmount == p_o_structureElement.SubTypeAmount) &&
                    (((this.SubType == null) && (p_o_structureElement.SubType == null)) || ((this.SubType != null) && (this.SubType.Equals(p_o_structureElement.SubType))))
                )
                {
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Return all structure element fields in one string
            /// </summary>
            public override string ToString()
            {
                string s_foo = "Structure Element: ";

                foreach (System.Reflection.PropertyInfo o_property in this.GetType().GetProperties())
                {
                    string s_value = "";

                    try
                    {
                        s_value = o_property.GetValue(this)?.ToString() ?? "null";
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
    }
}