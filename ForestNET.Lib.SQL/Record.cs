namespace ForestNET.Lib.SQL
{
    /// <summary>
    /// Interface for record class to use it as generic type in other classes
    /// </summary>
    public interface IRecord
    {
        public Object? GetColumnValue(string p_s_column);
        public void SetColumnValue(string p_s_column, Object? o_value);
        public string ReturnColumns();
        public void TakeOverRow(Dictionary<string, Object?> o_row);
    }

    /// <summary>
    /// Abstract record class - automatically detecting column fields and give easy DML methods usage in connection with a global used base property 'ForestNET.Lib.Global.Instance.Base'
    /// <br />
    /// T - Class definition of current class, e.g. Test extends ForestNET.Lib.SQL.Record&lt;Test&gt;
    /// </summary>
    public abstract class Record<T> : IRecord where T : IRecord
    {

        /* Delegates */

        /// <summary>
        /// interface delegate definition which can be instanced outside of Record class to process sql queries with another Base instead of Global.Base
        /// </summary>
        public delegate List<Dictionary<string, Object?>> OtherBaseSourceImplementation(IQuery? p_o_query);

        /* Fields */

        protected string Table;
        protected List<string> Primary = [];
        protected List<string> Unique = [];
        protected Dictionary<string, bool> OrderBy = [];

        protected int Start = 0;
        public int Interval = 50;
        public int Page = 1;
        protected int AmountRecords = 0;

        public List<string> Columns = [];
        public List<Filter> Filters = [];
        public Dictionary<string, bool> Sort = [];
        private T? o_recordImage = default;
        protected Type? RecordImageClass = null;

        /* Properties */

        public OtherBaseSourceImplementation? OtherBaseSource { get; set; } = null;

        /* Methods */

        /// <summary>
        /// Record class constructor, initiating all values and properties and checks if all necessary restrictions are set
        /// </summary>
        /// <exception cref="ArgumentNullException">record image class, primary, unique or order by have no values</exception>
        /// <exception cref="ArgumentException">table name is invalid(empty)</exception>
        /// <exception cref="MissingFieldException">given field values in primary, unique or order by do not exists is current class</exception>
        public Record()
        {
            this.Table = string.Empty;

            /* first call init function to get initial values and properties from inherited class */
            this.Init();

            /* check record image class is not null */
            if (this.RecordImageClass == null)
            {
                throw new ArgumentNullException("You must specify a record image class within the Init-method.");
            }

            /* check table is not empty */
            if (ForestNET.Lib.Helper.IsStringEmpty(this.Table))
            {
                throw new ArgumentException("You must specify a table within the Init-method.");
            }

            /* check primary values are available */
            if (this.Primary.Count < 1)
            {
                throw new ArgumentNullException("You must specify at least one primary within the Init-method.");
            }

            /* check if each field in primary list really exists in current class */
            foreach (string s_primary in this.Primary)
            {
                if (!this.FieldExists(s_primary))
                {
                    throw new MissingFieldException("Primary[" + s_primary + "] is not a field of current record class.");
                }
            }

            /* check unique values are available */
            if (this.Unique.Count < 1)
            {
                throw new ArgumentNullException("You must specify at least one unique constraint within the Init-method.");
            }

            /* check if each field in unique list really exists in current class */
            foreach (string s_unique in this.Unique)
            {
                /* it is possible that a unique constraint exists of multiple columns, separated by semicolon */
                if (s_unique.Contains(';'))
                {
                    string[] a_uniques = s_unique.Split(';');

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

            /* check order by values are available */
            if (this.OrderBy.Count < 1)
            {
                throw new ArgumentNullException("You must specify at least one order by within the Init-method.");
            }

            /* check if each field in order by list really exists in current class */
            foreach (string s_field in this.OrderBy.Keys)
            {
                if (!this.FieldExists(s_field))
                {
                    throw new MissingFieldException("OrderBy[" + s_field + "] is not a field of current record class.");
                }
            }

            if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("created record object with table '" + this.Table + "'");
        }

        /// <summary>
        /// Abstract Init function so any class inheriting from Record&lt;T&gt; must have this method
        /// declaring table, start, interval, primary, unique, order by and most of all record image class
        /// </summary>
        abstract protected void Init();

        /// <summary>
        /// Easy static method to return stored rows in a linked hash map out in a string which can be shown anywhere
        /// </summary>
        /// <param name="p_a_rows">stored rows in a linked hash map</param>
        /// <returns>a string with table header and all row column values</returns>*/
        public static string PrintRows(List<Dictionary<string, Object?>> p_a_rows)
        {
            /* string builder variable to store all column names and column values */
            System.Text.StringBuilder o_stringBuilder = new();

            /* check if parameter has any values */
            if ((p_a_rows != null) && (p_a_rows.Count > 0))
            {
                /* flag for printing header */
                bool b_once = false;

                /* iterate each row */
                foreach (Dictionary<string, Object?> o_row in p_a_rows)
                {
                    if (!b_once)
                    {
                        /* print header and all column names */
                        foreach (string s_column in o_row.Keys)
                        {
                            o_stringBuilder.Append(s_column + " | ");
                        }

                        o_stringBuilder.Append(ForestNET.Lib.IO.File.NEWLINE);
                        b_once = true;
                    }

                    /* print row column values */
                    foreach (Object? o_value in o_row.Values)
                    {
                        o_stringBuilder.Append(o_value + " | ");
                    }

                    o_stringBuilder.Append(ForestNET.Lib.IO.File.NEWLINE);
                }
            }
            else
            { /* parameter is null or has no rows */
                o_stringBuilder.Append("No rows available");
            }

            return o_stringBuilder.ToString();
        }

        /// <summary>
        /// Method to retrieve column value of current record class
        /// </summary>
        /// <param name="p_s_column">column name, will be changed to 'Column' + p_s_column</param>
        /// <returns>object type, nullable</returns>
        /// <exception cref="MissingFieldException">column field does not exist</exception>
        /// <exception cref="FieldAccessException">cannot access column field, must be public</exception>
        /// <exception cref="InvalidCastException">issue converting value into TimeSpan</exception>
        public Object? GetColumnValue(string p_s_column)
        {
            p_s_column = "Column" + p_s_column;

            if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("get column value from '" + p_s_column + "'");

            /* get column info */
            System.Reflection.FieldInfo o_fieldInfo = this.GetType().GetField(p_s_column) ?? throw new NullReferenceException("Column '" + p_s_column + "' not found in class");
            /* get column type */
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

            /* get column object value */
            Object? o_foo = this.GetType().GetField(p_s_column)?.GetValue(this);

            /* handle cast */
            if (o_foo != null)
            {
                if (!b_handleTimeSpan)
                {
                    /* cast object of column to target class type */
                    o_foo = Convert.ChangeType(o_foo, o_classType);
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
        /// Method to set a column value of current record class
        /// </summary>
        /// <param name="p_s_column">column name, will be changed to 'Column' + p_s_column</param>
        /// <param name="o_value">object value which will be set as column value</param>
        /// <exception cref="MissingFieldException">column field does not exist</exception>
        /// <exception cref="FieldAccessException">cannot access column field, must be public</exception>
        public void SetColumnValue(string p_s_column, Object? o_value)
        {
            p_s_column = "Column" + p_s_column;

            if (o_value != null)
            {
                /* get parameter value type */
                string? s_valueType = o_value.GetType().FullName;

                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("set column value for: '" + p_s_column + "'" + "\t\tcolumn type: " + GetType().GetField(p_s_column)?.GetType().FullName + "\t\tvalue type: " + s_valueType);

                if ((s_valueType != null) && (s_valueType.Contains("string", StringComparison.CurrentCultureIgnoreCase)))
                {
                    string? s_foo = o_value.ToString();

                    /* recognize empty null value */
                    if ((ForestNET.Lib.Helper.IsStringEmpty(s_foo)) || ((s_foo != null) && s_foo.Equals("NULL")))
                    {
                        o_value = null;
                    }
                }
                else if ((ForestNET.Lib.Global.Instance.BaseGateway == BaseGateway.NOSQLMDB) && (s_valueType != null) && ((GetType().GetField(p_s_column)?.FieldType.FullName ?? "").Contains("int16", StringComparison.CurrentCultureIgnoreCase)) && ((s_valueType.Contains("int32", StringComparison.CurrentCultureIgnoreCase)) || (s_valueType.Contains("int64", StringComparison.CurrentCultureIgnoreCase))))
                {
                    /* nosqlmdb does not support short or smallint, only int32 and long - so we must transpose int/long value to short value */
                    o_value = (Object?)Convert.ToInt16(o_value.ToString());
                }
            }

            /* set column value, accessing 'this' class and field with column name */
            GetType().GetField(p_s_column)?.SetValue(this, o_value);
        }

        /// <summary>
        /// Easy method to return all columns with their values of current record class
        /// </summary>
        /// <returns>a string line of all columns with their values "column_name = column_value|"</returns>
        public string ReturnColumns()
        {
            string s_foo = "";

            /* iterate each column of current record class */
            foreach (System.Reflection.FieldInfo o_fieldInfo in this.GetType().GetFields())
            {
                /* check if column starts with 'Column', but is not equal to 'Column' */
                if ((o_fieldInfo.Name.StartsWith("Column")) && (o_fieldInfo.Name.CompareTo("Columns") != 0))
                {
                    try
                    {
                        /* set column string value to 'NULL', in case value is null */
                        string s_object = o_fieldInfo.GetValue(this)?.ToString() ?? "NULL";

                        /* add column name and its value to return string */
                        s_foo += o_fieldInfo.Name.Substring(6) + " = " + s_object + "|";
                    }
                    catch (Exception)
                    {
                        /* just continue if column name or column value cannot be retrieved */
                        s_foo += o_fieldInfo.Name.Substring(6) + " = COULD_NOT_RETRIEVE|";
                    }
                }
            }

            return s_foo;
        }

        /// <summary>
        /// Assume column values of a linked hash map row record to columns of current record class and its record image
        /// </summary>
        /// <param name="o_row">columns and their values in a linked hash map</param>
        /// <exception cref="MissingFieldException">column field does not exist</exception>
        /// <exception cref="TypeLoadException">if the class that declares the underlying constructor represents an abstract class</exception>
        /// <exception cref="FieldAccessException">cannot access column field, must be public</exception>
        /// <exception cref="MissingMethodException">could not retrieve declared constructor</exception>
        /// <exception cref="ArgumentException">class cannot be located</exception>
        public void TakeOverRow(Dictionary<string, Object?> o_row)
        {
            /* create new instance of inherited class */

            /* check record image class is not null */
            if (this.RecordImageClass == null)
            {
                throw new NullReferenceException("You must specify a record image class within the Init-method.");
            }
            else
            {
                /* create new instance of inherited class */
                this.o_recordImage = (T?)Activator.CreateInstance(this.RecordImageClass);
            }

            if (this.o_recordImage == null)
            {
                throw new NullReferenceException("Record image object is null");
            }

            /* iterate each column in row */
            foreach (KeyValuePair<string, Object?> o_entry in o_row)
            {
                try
                {
                    if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("check if field exists '" + o_entry.Key + "'");

                    /* check if column exists as field in current record class */
                    if (FieldExists(o_entry.Key))
                    {
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("set column value for record object");

                        /* set column value in current record class */
                        this.SetColumnValue(o_entry.Key, o_entry.Value);

                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("set column value for record image");

                        /* set column value in record image */
                        this.o_recordImage.SetColumnValue(o_entry.Key, o_entry.Value);
                    }
                    else
                    {
                        ForestNET.Lib.Global.ILogWarning(o_entry.Key + " does not exist");
                    }
                }
                catch (Exception o_exc)
                {
                    /* just continue if field does not exist */
                    ForestNET.Lib.Global.ILogWarning(o_entry.Value?.ToString() + " ;;; cannot set '" + o_entry.Key + "' with type '" + ((o_entry.Value == null) ? "null" : o_entry.Value.GetType().FullName) + "' - " + o_exc);
                }
            }
        }

        /// <summary>
        /// Method to check if a field exists in current record class
        /// </summary>
        /// <param name="p_s_field">colum name</param>
        /// <returns>true - field exist, false - field does not exist</returns>
        protected bool FieldExists(string p_s_field)
        {
            /* return value */
            bool b_found = false;

            /* iterate each field of current record class */
            foreach (System.Reflection.FieldInfo o_fieldInfo in this.GetType().GetFields())
            {
                /* check if field starts with 'Column', but is not equal to 'Columns' */
                if ((o_fieldInfo.Name.StartsWith("Column")) && (o_fieldInfo.Name.CompareTo("Columns") != 0))
                {
                    /* field name without 'Field' prefix must match parameter value */
                    if (o_fieldInfo.Name.Substring(6).CompareTo(p_s_field) == 0)
                    {
                        /* set return value to true */
                        b_found = true;
                    }
                }
            }

            return b_found;
        }

        /// <summary>
        /// Get a record by its current primary values of inherited class and assume record columns to public properties
        /// </summary>
        /// <returns>true - record has been found, false - no record has been found</returns>
        /// <exception cref="ArgumentException">list of primary values as parameter have not the same amount as list of primary columns</exception>
        /// <exception cref="AccessViolationException">exception accessing column type, column name or just column value of current result set record</exception>
        /// <exception cref="MissingFieldException">column field does not exist</exception>
        /// <exception cref="TypeLoadException">if the class that declares the underlying constructor represents an abstract class</exception>
        /// <exception cref="FieldAccessException">cannot access column field, must be public</exception>
        /// <exception cref="MissingMethodException">could not retrieve declared constructor</exception>
        public bool GetRecord()
        {
            return this.GetRecord(null);
        }

        /// <summary>
        /// Get a record by parameter primary values and assume record columns to public properties
        /// </summary>
        /// <param name="p_a_primaryValues">list of primary values</param>
        /// <returns>true - record has been found, false - no record has been found</returns>
        /// <exception cref="ArgumentException">list of primary values as parameter have not the same amount as list of primary columns</exception>
        /// <exception cref="AccessViolationException">exception accessing column type, column name or just column value of current result set record</exception>
        /// <exception cref="MissingFieldException">column field does not exist</exception>
        /// <exception cref="TypeLoadException">if the class that declares the underlying constructor represents an abstract class</exception>
        /// <exception cref="FieldAccessException">cannot access column field, must be public</exception>
        /// <exception cref="MissingMethodException">could not retrieve declared constructor</exception>
        public bool GetRecord(List<Object>? p_a_primaryValues)
        {
            /* if we have primary values as parameter it must be the same size as primary fields existing */
            if ((p_a_primaryValues != null) && (this.Primary.Count != p_a_primaryValues.Count))
            {
                throw new ArgumentException("Primary input values and primary fields are not of the same amount", nameof(p_a_primaryValues));
            }

            ForestNET.Lib.Global.ILogFiner("create query select object");

            /* create select query */
            Query<Select> o_querySelect = new(ForestNET.Lib.Global.Instance.BaseGateway, SqlType.SELECT, this.Table);
            /* just select all columns */
            o_querySelect.GetQuery<Select>()?.Columns.Add(new Column(o_querySelect, "*"));

            ForestNET.Lib.Global.ILogFiner("created query select object");

            /* iterate each primary field */
            for (int i = 0; i < this.Primary.Count; i++)
            {
                ForestNET.Lib.Global.ILogFinest("create where clause object with '" + this.Primary[i] + "'");

                /* create where clause for each primary field, get value from parameter list or from inherited class */
                Where o_where = new(o_querySelect, new Column(o_querySelect, this.Primary[i]), ((p_a_primaryValues != null) ? p_a_primaryValues[i] : this.GetColumnValue(this.Primary[i])), "=");

                ForestNET.Lib.Global.ILogFinest("created where clause object");

                /* add 'AND' filter operator for concatenating all where clauses */
                if (i != 0)
                {
                    o_where.FilterOperator = "AND";
                }

                /* add where clause to select query */
                o_querySelect.GetQuery<Select>()?.Where.Add(o_where);

                ForestNET.Lib.Global.ILogFinest("added where clause object to query");
            }

            ForestNET.Lib.Global.ILogFiner("execute query");

            /* execute select query */
            List<Dictionary<string, Object?>> a_rows = ((this.OtherBaseSource != null) ? this.OtherBaseSource.Invoke(o_querySelect) : ForestNET.Lib.Global.Instance.Base?.FetchQuery(o_querySelect)) ?? [];

            ForestNET.Lib.Global.ILogFiner("executed query");

            if (a_rows.Count == 1)
            { /* if return amount is equal 1, take over result to current record object */
                ForestNET.Lib.Global.ILogFiner("take over result to current record object");
                this.TakeOverRow(a_rows[0]);
                ForestNET.Lib.Global.ILogFiner("result values adopted to current record object");
                return true;
            }
            else
            { /* return amount is not equal 1, record has not been found */
                ForestNET.Lib.Global.ILogFiner("record has not been found");
                return false;
            }
        }

        /// <summary>
        /// Get one record, independent of primary key, by parameter individual unique key and parameter values to that unique key and assume record columns to public properties
        /// </summary>
        /// <param name="p_a_unique">parameter unique key which can be individual and contain all columns of inherited class</param>
        /// <param name="p_a_values">parameter values to unique key</param>
        /// <returns>true - record has been found, false - no record has been found</returns>
        /// <exception cref="ArgumentException">a column does not exist in unique key or list of unique key values have not the same amount as list of unique columns</exception>
        /// <exception cref="AccessViolationException">exception accessing column type, column name or just column value of current result set record</exception>
        /// <exception cref="MissingFieldException">column field does not exist</exception>
        /// <exception cref="TypeLoadException">if the class that declares the underlying constructor represents an abstract class</exception>
        /// <exception cref="FieldAccessException">cannot access column field, must be public</exception>
        /// <exception cref="MissingMethodException">could not retrieve declared constructor</exception>
        public bool GetOneRecord(List<string> p_a_unique, List<Object> p_a_values)
        {
            ForestNET.Lib.Global.ILogFiner("check if each column in unique key really exists as column");

            /* check if each column in unique key really exists as column in inherited class */
            foreach (string s_unique in p_a_unique)
            {
                ForestNET.Lib.Global.ILogFinest("check column '" + s_unique + "'");

                if (!this.FieldExists(s_unique))
                {
                    throw new ArgumentException("Column[" + s_unique + "] does not exist");
                }
            }

            ForestNET.Lib.Global.ILogFiner("each column in unique key really exists");

            /* save old primary key */
            List<string> OldPrimary = this.Primary;

            ForestNET.Lib.Global.ILogFiner("set unique key as temporarily primary key");

            /* set unique key as temporarily primary key */
            this.Primary = p_a_unique;

            /* get record with parameter value list */
            bool b_foo = this.GetRecord(p_a_values);

            ForestNET.Lib.Global.ILogFiner("restore old primary key");

            /* restore old primary key */
            this.Primary = OldPrimary;

            /* return result of GetRecord */
            return b_foo;
        }

        /// <summary>
        /// Get multiple amount of records with paging, select can be combined with optional columns, filter and order by lists within record class which are automatically part for each inherited class
        /// </summary>
        /// <returns>all records in a result list</returns>
        /// <exception cref="ArgumentException">a column does not exist in optional columns, filter or order by lists</exception>
        /// <exception cref="FieldAccessException">exception accessing column type, column name or just column value of current result set record</exception>
        /// <exception cref="AccessViolationException">exception accessing column type, column name or just column value of current result set record</exception>
        /// <exception cref="MissingFieldException">column field does not exist</exception>
        /// <exception cref="TypeLoadException">if the class that declares the underlying constructor represents an abstract class</exception>
        /// <exception cref="MissingMethodException">could not retrieve declared constructor</exception>
        public List<T> GetRecords()
        {
            return this.GetRecords(false);
        }

        /// <summary>
        /// Get multiple amount of records, select can be combined with optional columns, filter and order by lists within record class which are automatically part for each inherited class
        /// supports paging
        /// </summary>
        /// <param name="p_b_unlimited">true - return all records, false - use paging with page and interval properties</param>
        /// <returns>all records in a result list</returns>
        /// <exception cref="ArgumentException">a column does not exist in optional columns, filter or order by lists</exception>
        /// <exception cref="FieldAccessException">exception accessing column type, column name or just column value of current result set record</exception>
        /// <exception cref="AccessViolationException">exception accessing column type, column name or just column value of current result set record</exception>
        /// <exception cref="MissingFieldException">column field does not exist</exception>
        /// <exception cref="TypeLoadException">if the class that declares the underlying constructor represents an abstract class</exception>
        /// <exception cref="MissingMethodException">could not retrieve declared constructor</exception>
        public List<T> GetRecords(bool p_b_unlimited)
        {
            ForestNET.Lib.Global.ILogFiner("create query select object");

            /* create select query object */
            Query<Select> o_querySelect = new(ForestNET.Lib.Global.Instance.BaseGateway, SqlType.SELECT, this.Table);

            ForestNET.Lib.Global.ILogFiner("created query select object");

            /* adding optional columns for the query if they are set */
            if (this.Columns.Count > 0)
            {
                ForestNET.Lib.Global.ILogFiner("check if each column in Columns property really exists");

                /* iterate each column */
                foreach (string s_column in this.Columns)
                {
                    ForestNET.Lib.Global.ILogFinest("check column '" + s_column + "'");

                    /* check if each column really exists as column in inherited class */
                    if (!this.FieldExists(s_column))
                    {
                        throw new ArgumentException("Column[" + s_column + "] does not exist");
                    }

                    ForestNET.Lib.Global.ILogFinest("add column to query select");

                    /* add column to select query */
                    o_querySelect.GetQuery<Select>()?.Columns.Add(new Column(o_querySelect, s_column));

                    ForestNET.Lib.Global.ILogFinest("added column to query select");
                }

                ForestNET.Lib.Global.ILogFiner("each column in Columns property really exists");
            }
            else
            {
                ForestNET.Lib.Global.ILogFiner("just select all columns with '*'");

                /* just select all columns */
                o_querySelect.GetQuery<Select>()?.Columns.Add(new Column(o_querySelect, "*"));
            }

            /* implement optional filter */
            if (this.Filters.Count > 0)
            {
                /* flag if where clause has been started */
                bool b_initWhere = false;

                ForestNET.Lib.Global.ILogFiner("check if each column in Filters property really exists");

                /* iterate each filter column */
                foreach (Filter o_filter in this.Filters)
                {
                    ForestNET.Lib.Global.ILogFinest("check column '" + o_filter.Column + "'");

                    /* check if each filter column really exists as column in inherited class */
                    if (!this.FieldExists(o_filter.Column))
                    {
                        throw new ArgumentException("Filter Column[" + o_filter.Column + "] does not exist");
                    }

                    ForestNET.Lib.Global.ILogFinest("add column to new where clause object");

                    /* create where clause object */
                    Where o_where = new(o_querySelect, new Column(o_querySelect, o_filter.Column), o_filter.Value, o_filter.Operator);

                    ForestNET.Lib.Global.ILogFinest("added column to new where clause object");

                    if (b_initWhere)
                    { /* set filter operator if where clause has been started */
                        /* add filter operator if it is not empty */
                        if (!ForestNET.Lib.Helper.IsStringEmpty(o_filter.FilterOperator))
                        {
                            o_where.FilterOperator = o_filter.FilterOperator ?? "";
                        }
                    }

                    /* at least here, where clause has been started */
                    b_initWhere = true;

                    ForestNET.Lib.Global.ILogFinest("add where clause object to query select");

                    /* add where clause to select query */
                    o_querySelect.GetQuery<Select>()?.Where.Add(o_where);

                    ForestNET.Lib.Global.ILogFinest("added where clause object to query select");
                }

                ForestNET.Lib.Global.ILogFiner("each column in Filters property really exists");
            }

            /* temporarily order by map */
            Dictionary<string, bool> m_temp = [];

            if (this.Sort.Count > 0)
            { /* implement order by from optional sort hash map */
                ForestNET.Lib.Global.ILogFiner("implement order by from optional sort hash map");
                m_temp = this.Sort;
            }
            else if (this.OrderBy.Count > 0)
            { /* implement order by from defined order by hash map from inherited class */
                ForestNET.Lib.Global.ILogFiner("implement order by from defined order by hash map from inherited class");
                m_temp = this.OrderBy;
            }

            /* if we have an optional sort hash map or defined order by hash map from inherited class */
            if (m_temp.Count > 0)
            {
                /* temp lists to implement order by to select query */
                List<Column> a_columns = [];
                List<string> a_stringColumns = [];
                List<bool> a_directions = [];

                /* assume all sort columns and directions from temporarily order by map, based on optional sort hash map or definition from inherited class */
                foreach (KeyValuePair<string, bool> o_entry in m_temp)
                {
                    a_stringColumns.Add(o_entry.Key);
                    a_directions.Add(o_entry.Value);
                }

                ForestNET.Lib.Global.ILogFiner("check if each column in OrderBy property really exists");

                /* iterate each sort column */
                foreach (string s_column in a_stringColumns)
                {
                    ForestNET.Lib.Global.ILogFinest("check column '" + s_column + "'");

                    /* check if each sort column really exists as column in inherited class */
                    if (!this.FieldExists(s_column))
                    {
                        throw new ArgumentException("Sort Column[" + s_column + "] does not exist");
                    }
                    else
                    {
                        ForestNET.Lib.Global.ILogFinest("add column to temp column list");

                        /* assume sort column to temp column list */
                        a_columns.Add(new Column(o_querySelect, s_column));

                        ForestNET.Lib.Global.ILogFinest("added column to temp column list");
                    }
                }

                ForestNET.Lib.Global.ILogFiner("each column in OrderBy property really exists");

                ForestNET.Lib.Global.ILogFiner("create order by object for query select");

                /* add order by to select query */
                (o_querySelect.GetQuery<Select>() ?? throw new NullReferenceException("select query is null")).OrderBy = new OrderBy(o_querySelect, a_columns, a_directions);

                ForestNET.Lib.Global.ILogFiner("created order by object for query select");
            }

            /* implement limit if not unlimited is set */
            if (!p_b_unlimited)
            {
                /* page must be at least '1' */
                if (this.Page < 1)
                {
                    ForestNET.Lib.Global.ILogFiner("set Page to '1'");

                    this.Page = 1;
                }

                if (this.Page > 1)
                { /* if page is greater than 1, we need to calculate a new start value */
                    ForestNET.Lib.Global.ILogFiner("Page > '1'");

                    /* get amount of record */
                    this.AmountRecords = this.GetCount();

                    ForestNET.Lib.Global.ILogFiner("calculate pages to show all records");

                    /* calculate pages to show all records */
                    int i_pages = this.AmountRecords / this.Interval + ((this.AmountRecords % this.Interval == 0) ? 0 : 1);

                    ForestNET.Lib.Global.ILogFiner("calculated pages: '" + i_pages + "'");

                    /* page cannot be greater than amount of calculated pages for all records */
                    if (this.Page >= i_pages)
                    {
                        this.Page = i_pages;
                    }

                    ForestNET.Lib.Global.ILogFiner("calculate new start value");

                    /* calculate new start value */
                    this.Start = (this.Page - 1) * this.Interval;

                    ForestNET.Lib.Global.ILogFiner("calculated start: '" + this.Start + "'");
                }

                ForestNET.Lib.Global.ILogFiner("create limit object for query select with start '" + this.Start + "' and interval '" + this.Interval + "'");

                /* add limit cause to select query */
                (o_querySelect.GetQuery<Select>() ?? throw new NullReferenceException("select query is null")).Limit = new Limit(o_querySelect, this.Start, this.Interval);
            }

            /* result list */
            List<T> a_result = [];

            ForestNET.Lib.Global.ILogFiner("execute query");

            /* execute select queries and get all rows */
            List<Dictionary<string, Object?>> a_rows = ((this.OtherBaseSource != null) ? this.OtherBaseSource.Invoke(o_querySelect) : ForestNET.Lib.Global.Instance.Base?.FetchQuery(o_querySelect)) ?? [];

            ForestNET.Lib.Global.ILogFiner("executed query");

            ForestNET.Lib.Global.ILogFiner("iterate each result record");

            /* iterate each result record */
            foreach (Dictionary<string, Object?> o_row in a_rows)
            {
                try
                {
                    ForestNET.Lib.Global.ILogFinest("take over result to current record object");

                    if (this.RecordImageClass == null)
                    {
                        throw new NullReferenceException("record image class is null");
                    }

                    /* create new instance of inherited class */
                    T o_temp = (T)(Activator.CreateInstance(this.RecordImageClass) ?? throw new NullReferenceException("could not create new instance for row with record image class '" + this.RecordImageClass + "'"));

                    ForestNET.Lib.Global.ILogFinest("take over result to new instance of inherited class");

                    /* assume result in new instance */
                    o_temp.TakeOverRow(o_row);

                    ForestNET.Lib.Global.ILogFinest("result values adopted to new instance of inherited class");

                    ForestNET.Lib.Global.ILogFinest("add new instance to result list");
                    a_result.Add(o_temp);

                    ForestNET.Lib.Global.ILogFinest("added new instance to result list");
                }
                catch (Exception o_exc)
                {
                    /* skip record if we cannot assume result in a new instance */
                    ForestNET.Lib.Global.ILogFinest("skipped record if we cannot adopt result in a new instance; " + o_exc);
                }
            }
            ;

            ForestNET.Lib.Global.ILogFiner("return result list");

            /* return result list */
            return a_result;
        }

        /// <summary>
        /// Get amount of records of current table declared in inherited class, select can be combined with optional filter list
        /// </summary>
        /// <returns>records count 0 .. x or -1 if an error occurred</returns>
        /// <exception cref="ArgumentException">a column does not exist in optional filter list</exception>
        /// <exception cref="FieldAccessException">exception accessing column type, column name or just column value of current result set record</exception>
        /// <exception cref="AccessViolationException">exception accessing column type, column name or just column value of current result set record</exception>
        /// <exception cref="MissingFieldException">column field does not exist</exception>
        /// <exception cref="TypeLoadException">if the class that declares the underlying constructor represents an abstract class</exception>
        /// <exception cref="MissingMethodException">could not retrieve declared constructor</exception>
        public int GetCount()
        {
            ForestNET.Lib.Global.ILogFiner("create query select object");

            /* create select query */
            Query<Select> o_querySelect = new(ForestNET.Lib.Global.Instance.BaseGateway, SqlType.SELECT, this.Table);

            ForestNET.Lib.Global.ILogFiner("created query select object");

            ForestNET.Lib.Global.ILogFiner("add 'COUNT(*) AS AmountRecord' to query select object");

            /* add count aggregation for amount of records */
            o_querySelect.GetQuery<Select>()?.Columns.Add(new Column(o_querySelect, "*", "AmountRecords", "COUNT"));

            ForestNET.Lib.Global.ILogFiner("added 'COUNT(*) AS AmountRecord' to query select object");

            /* implement optional filter */
            if (this.Filters.Count > 0)
            {
                /* flag if where clause has been started */
                bool b_initWhere = false;

                ForestNET.Lib.Global.ILogFiner("check if each column in Filters property really exists");

                /* iterate each filter column */
                foreach (Filter o_filter in this.Filters)
                {
                    ForestNET.Lib.Global.ILogFinest("check column '" + o_filter.Column + "'");

                    /* check if each filter column really exists as column in inherited class */
                    if (!this.FieldExists(o_filter.Column))
                    {
                        throw new ArgumentException("Filter Column[" + o_filter.Column + "] does not exist");
                    }

                    ForestNET.Lib.Global.ILogFinest("add column to new where clause object");

                    /* create where clause object */
                    Where o_where = new(o_querySelect, new Column(o_querySelect, o_filter.Column), o_filter.Value, o_filter.Operator);

                    ForestNET.Lib.Global.ILogFinest("added column to new where clause object");

                    if (b_initWhere)
                    { /* set filter operator if where clause has been started */
                        /* add filter operator if it is not empty */
                        if (!ForestNET.Lib.Helper.IsStringEmpty(o_filter.FilterOperator))
                        {
                            o_where.FilterOperator = o_filter.FilterOperator ?? "";
                        }
                    }

                    /* at least here, where clause has been started */
                    b_initWhere = true;

                    ForestNET.Lib.Global.ILogFinest("add where clause object to query select");

                    /* add where clause to select query */
                    o_querySelect.GetQuery<Select>()?.Where.Add(o_where);

                    ForestNET.Lib.Global.ILogFinest("added where clause object to query select");
                }

                ForestNET.Lib.Global.ILogFiner("each column in Filters property really exists");
            }

            ForestNET.Lib.Global.ILogFiner("execute query");

            /* execute select query and get result */
            List<Dictionary<string, Object?>> a_rows = ((this.OtherBaseSource != null) ? this.OtherBaseSource.Invoke(o_querySelect) : ForestNET.Lib.Global.Instance.Base?.FetchQuery(o_querySelect)) ?? [];

            ForestNET.Lib.Global.ILogFiner("executed query");

            if (ForestNET.Lib.Global.Instance.BaseGateway != BaseGateway.NOSQLMDB)
            {
                ForestNET.Lib.Global.ILogFiner("result must be exactly one row");

                /* result must be exactly one row */
                if (a_rows.Count == 1)
                {
                    ForestNET.Lib.Global.ILogFiner("get 'AmountRecords' or 'amountrecords' as result count value");

                    /* get 'AmountRecords' or 'amountrecords' as result count value */
                    if (a_rows[0].TryGetValue("AmountRecords", out object? s_bar))
                    {
                        return Convert.ToInt32(s_bar?.ToString());
                    }
                    else if (a_rows[0].TryGetValue("amountrecords", out object? s_baz))
                    {
                        return Convert.ToInt32(s_baz?.ToString());
                    }
                    else
                    { /* could not find column 'AmountRecords', so we return -1 as error */
                        ForestNET.Lib.Global.ILogFiner("could not find column 'AmountRecords', so we return -1 as error");

                        return -1;
                    }
                }
                else
                { /* false result, so we return -1 as error */
                    ForestNET.Lib.Global.ILogFiner("false result, so we return -1 as error");

                    return -1;
                }
            }
            else
            {
                ForestNET.Lib.Global.ILogFiner("just return rows size for nosqlmdb");

                /* just return rows size for nosqlmdb */
                return a_rows.Count;
            }
        }

        /// <summary>
        /// Inserts a record with current columns of inherited class, skip primary fields/columns
        /// checking primary key and all unique key violation which are given as information within inherited class
        /// </summary>
        /// <returns>primary 'LastInsertId', but when this is not available then the amount of 'AffectedRows', -1 as error</returns>
        /// <exception cref="InvalidOperationException">primary key or unique key violation occurred</exception>
        /// <exception cref="ArgumentException">a column does not exist in optional filter list</exception>
        /// <exception cref="FieldAccessException">exception accessing column type, column name or just column value of current result set record</exception>
        /// <exception cref="AccessViolationException">exception accessing column type, column name or just column value of current result set record</exception>
        /// <exception cref="MissingFieldException">column field does not exist</exception>
        /// <exception cref="TypeLoadException">if the class that declares the underlying constructor represents an abstract class</exception>
        /// <exception cref="MissingMethodException">could not retrieve declared constructor</exception>
        public int InsertRecord()
        {
            return InsertRecord(false);
        }

        /// <summary>
        /// Inserts a record with current columns of inherited class
        /// checking primary key and all unique key violation which are given as information within inherited class
        /// </summary>
        /// <param name="p_b_withPrimary">true - set values for primary fields/columns as well, false - skip primary fields/columns</param>
        /// <returns>primary 'LastInsertId', but when this is not available then the amount of 'AffectedRows', -1 as error</returns>
        /// <exception cref="InvalidOperationException">primary key or unique key violation occurred</exception>
        /// <exception cref="ArgumentException">a column does not exist in optional filter list</exception>
        /// <exception cref="FieldAccessException">exception accessing column type, column name or just column value of current result set record</exception>
        /// <exception cref="AccessViolationException">exception accessing column type, column name or just column value of current result set record</exception>
        /// <exception cref="MissingFieldException">column field does not exist</exception>
        /// <exception cref="TypeLoadException">if the class that declares the underlying constructor represents an abstract class</exception>
        /// <exception cref="MissingMethodException">could not retrieve declared constructor</exception>
        public int InsertRecord(bool p_b_withPrimary)
        {
            /* check uniqueness of record within table for insert query */
            int i_return;

            /* check primary columns if we insert record with primary value */
            if (p_b_withPrimary)
            {
                ForestNET.Lib.Global.ILogFiner("insert record with primary value");

                ForestNET.Lib.Global.ILogFiner("create a backup of current filter list");

                /* create a backup of current filter list */
                List<Filter> a_backupFilters = this.Filters;
                /* clear current filter list */
                this.Filters.Clear();

                ForestNET.Lib.Global.ILogFiner("iterate each primary field/column");

                /* iterate each primary field/column */
                foreach (string s_primary in this.Primary)
                {
                    try
                    {
                        ForestNET.Lib.Global.ILogFinest("add primary field/column to Filters property list: '" + s_primary + "'");

                        /* add primary field/column with it's value to filter list */
                        this.Filters.Add(new Filter(s_primary, this.GetColumnValue(s_primary) ?? "NULL", "=", "AND"));

                        ForestNET.Lib.Global.ILogFinest("added primary field/column to Filters property list");
                    }
                    catch (Exception o_exc)
                    {
                        /* skip a field/column if value cannot be retrieved */
                        ForestNET.Lib.Global.ILogFinest("skipped a field/column if value cannot be retrieved: '" + s_primary + "'; " + o_exc);
                    }
                }
                ;

                ForestNET.Lib.Global.ILogFiner("get amount of records with current primary key and it's values");

                /* get amount of records with current primary key and it's values */
                i_return = this.GetCount();

                ForestNET.Lib.Global.ILogFiner("amount: '" + i_return + "'");

                ForestNET.Lib.Global.ILogFiner("restore filter list with backup");

                /* restore filter list with backup */
                this.Filters = a_backupFilters;

                /* if amount of records is greater than zero, we cannot insert that record */
                if (i_return > 0)
                {
                    ForestNET.Lib.Global.ILogFiner("Primary key violation occurred, create primary key and it's values to throw an exception");

                    string s_primaries = "";
                    string s_primaryValues = "";

                    /* gather all primary fields/columns and it's values */
                    foreach (string s_primary in this.Primary)
                    {
                        s_primaries += s_primary + ", ";
                        s_primaryValues += this.GetColumnValue(s_primary) ?? "NULL" + ", ";
                    }

                    /* remove last ', ' separator */
                    if (s_primaries.Length > 1)
                    {
                        s_primaries = s_primaries.Substring(0, (s_primaries.Length - 2));
                        s_primaryValues = s_primaryValues.Substring(0, (s_primaryValues.Length - 2));
                    }

                    /* create an exception that we cannot insert a record, because of primary key violation */
                    throw new InvalidOperationException("Primary key violation - primary key[" + s_primaries + "](" + s_primaryValues + ") already exists for [" + this.Table + "]");
                }
            }

            ForestNET.Lib.Global.ILogFiner("check unique constraints");

            /* check unique constraints */
            foreach (string s_unique in this.Unique)
            {
                ForestNET.Lib.Global.ILogFinest("check unique constraint: '" + s_unique + "'");

                ForestNET.Lib.Global.ILogFinest("create a backup of current filter list");

                /* create a backup of current filter list */
                List<Filter> a_backupFilters = this.Filters;
                /* clear current filter list */
                this.Filters.Clear();

                /* it is possible that a unique constraint exists of multiple columns, separated by semicolon */
                if (s_unique.Contains(';'))
                {
                    string[] a_uniques = s_unique.Split(";");

                    /* iterate each unique key field/column */
                    for (int i = 0; i < a_uniques.Length; i++)
                    {
                        /* add unique field/column with it's value to filter list */
                        this.Filters.Add(new Filter(a_uniques[i], this.GetColumnValue(a_uniques[i]) ?? "NULL", "=", "AND"));
                    }
                }
                else
                {
                    /* add unique field/column with it's value to filter list */
                    this.Filters.Add(new Filter(s_unique, this.GetColumnValue(s_unique) ?? "NULL", "=", "AND"));
                }

                ForestNET.Lib.Global.ILogFinest("get amount of records with current unique key and it's values");

                /* get amount of records with current unique key and it's values */
                i_return = this.GetCount();

                ForestNET.Lib.Global.ILogFinest("amount: '" + i_return + "'");

                ForestNET.Lib.Global.ILogFinest("restore filter list with backup");

                /* restore filter list with backup */
                this.Filters = a_backupFilters;

                /* if amount of records is greater than zero, we cannot insert that record */
                if (i_return > 0)
                {
                    ForestNET.Lib.Global.ILogFinest("Unique key violation occurred, create unique key and it's values to throw an exception");

                    string s_uniqueValues = "";

                    /* it is possible that a unique constraint exists of multiple columns, separated by semicolon */
                    if (s_unique.Contains(';'))
                    {
                        /* split unique key */
                        string[] a_uniques = s_unique.Split(";");

                        /* iterate each unique field/column */
                        for (int i = 0; i < a_uniques.Length; i++)
                        {
                            /* gather all unique field/column values */
                            s_uniqueValues = this.GetColumnValue(s_unique)?.ToString() ?? "NULL" + ", ";
                        }

                        /* remove last ', ' separator */
                        if (s_uniqueValues.Length > 1)
                        {
                            s_uniqueValues = s_uniqueValues.Substring(0, (s_uniqueValues.Length - 2));
                        }
                    }
                    else
                    {
                        /* add unique field/column value */
                        s_uniqueValues = this.GetColumnValue(s_unique)?.ToString() ?? "NULL";
                    }

                    /* create an exception that we cannot insert a record, because of unique key violation */
                    throw new InvalidOperationException("Unique key violation - unique constraint invalid for [" + s_unique + "](" + s_uniqueValues + ") in table [" + this.Table + "]; unique key already exists");
                }
            }

            ForestNET.Lib.Global.ILogFiner("create insert query");

            /* create insert query */
            Query<Insert> o_queryInsert = new(ForestNET.Lib.Global.Instance.BaseGateway, SqlType.INSERT, this.Table);

            ForestNET.Lib.Global.ILogFiner("created insert query");

            /* for nosqlmdb we must set 'Id' as auto increment column */
            if ((ForestNET.Lib.Global.Instance.BaseGateway == BaseGateway.NOSQLMDB) && (!p_b_withPrimary) && (this.Primary.Contains("Id")) && (this.Primary.Count == 1))
            {
                ForestNET.Lib.Global.ILogFiner("for nosqlmdb we must set 'Id' as auto increment column");

                (o_queryInsert.GetQuery<Insert>() ?? throw new NullReferenceException("insert query is null")).NoSQLMDBColumnAutoIncrement = new Column(o_queryInsert, "Id");
            }

            ForestNET.Lib.Global.ILogFiner("read out column fields to get values for insert query");

            /* read out column fields to get values for insert query */
            foreach (System.Reflection.FieldInfo o_fieldInfo in this.GetType().GetFields())
            {
                /* check if field starts with 'Column', but is not equal to 'Columns' */
                if ((o_fieldInfo.Name.StartsWith("Column")) && (o_fieldInfo.Name.CompareTo("Columns") != 0))
                {
                    /* get field name without 'Column' prefix */
                    string s_column = o_fieldInfo.Name.Substring(6);

                    ForestNET.Lib.Global.ILogFinest("check if field/column '" + s_column + "' is not part of primary key OR p_b_withPrimary = '" + p_b_withPrimary + "'");

                    /* check if field/column is not part of primary key or we explicitly allow these fields/columns as well */
                    if ((!this.Primary.Contains(s_column)) || (p_b_withPrimary))
                    {
                        ForestNET.Lib.Global.ILogFinest("add field/column '" + s_column + "' to column value pair list of insert query");

                        /* add field/column to column value pair list of insert query */
                        o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, s_column), this.GetColumnValue(s_column)));

                        ForestNET.Lib.Global.ILogFinest("added field/column to column value pair list of insert query");
                    }
                }
            }

            ForestNET.Lib.Global.ILogFiner("execute insert query");

            /* execute insert query and get result */
            List<Dictionary<string, Object?>> a_rows = ((this.OtherBaseSource != null) ? this.OtherBaseSource.Invoke(o_queryInsert) : ForestNET.Lib.Global.Instance.Base?.FetchQuery(o_queryInsert)) ?? [];

            ForestNET.Lib.Global.ILogFiner("insert query executed");

            /* result must be exactly one row */
            if (a_rows.Count == 1)
            {
                if (a_rows[0].TryGetValue("LastInsertId", out object? o_foo))
                { /* check if 'LastInsertId' is available */
                    ForestNET.Lib.Global.ILogFiner("return 'LastInsertId' value");

                    /* return 'LastInsertId' value */
                    return Convert.ToInt32(o_foo?.ToString());
                }
                else if (a_rows[0].TryGetValue("LastInsertId", out object? o_bar))
                { /* check if 'AffectedRows' is available */
                    ForestNET.Lib.Global.ILogFiner("return 0 if 'AffectedRows' value is lower than one");

                    /* return 0 if 'AffectedRows' value is lower than one */
                    if (Convert.ToInt32(o_bar?.ToString()) < 1)
                    {
                        return 0;
                    }
                    else
                    {
                        ForestNET.Lib.Global.ILogFiner("return 'AffectedRows' value");

                        /* return 'AffectedRows' value */
                        return Convert.ToInt32(o_bar?.ToString());
                    }
                }
                else
                { /* if we have not 'LastInsertId' and not 'AffectedRows', return -1 as error */
                    ForestNET.Lib.Global.ILogFiner("if we have not 'LastInsertId' and not 'AffectedRows', return -1 as error");

                    return -1;
                }
            }
            else
            { /* result is not just one row, return -1 as error */
                ForestNET.Lib.Global.ILogFiner("result is not just one row, return -1 as error");

                return -1;
            }
        }

        /// <summary>
        /// Updates a record with current columns of inherited class, with unique fields/columns as well
        /// checking primary key and all unique key violation which are given as information within inherited class
        /// </summary>
        /// <returns>primary 'AffectedRows' amount value, -1 as error</returns>
        /// <exception cref="InvalidOperationException">primary key or unique key violation occurred</exception>
        /// <exception cref="ArgumentException">a column does not exist in optional filter list</exception>
        /// <exception cref="FieldAccessException">exception accessing column type, column name or just column value of current result set record</exception>
        /// <exception cref="AccessViolationException">exception accessing column type, column name or just column value of current result set record</exception>
        /// <exception cref="MissingFieldException">column field does not exist</exception>
        /// <exception cref="TypeLoadException">if the class that declares the underlying constructor represents an abstract class</exception>
        /// <exception cref="MissingMethodException">could not retrieve declared constructor</exception>
        public int UpdateRecord()
        {
            return UpdateRecord(true);
        }

        /// <summary>
        /// Updates a record with current columns of inherited class
        /// checking primary key and all unique key violation which are given as information within inherited class
        /// </summary>
        /// <param name="p_b_withUnique">true - set values for unique fields/columns as well, false - skip unique fields/columns</param>
        /// <returns>primary 'AffectedRows' amount value, -1 as error</returns>
        /// <exception cref="InvalidOperationException">primary key or unique key violation occurred</exception>
        /// <exception cref="ArgumentException">a column does not exist in optional filter list</exception>
        /// <exception cref="FieldAccessException">exception accessing column type, column name or just column value of current result set record</exception>
        /// <exception cref="AccessViolationException">exception accessing column type, column name or just column value of current result set record</exception>
        /// <exception cref="MissingFieldException">column field does not exist</exception>
        /// <exception cref="TypeLoadException">if the class that declares the underlying constructor represents an abstract class</exception>
        /// <exception cref="MissingMethodException">could not retrieve declared constructor</exception>
        public int UpdateRecord(bool p_b_withUnique)
        {
            /* flag to determine if anything has changed on the record, preventing unnecessary update queries */
            bool b_field_has_changed = false;

            /* check if record image is loaded */
            if (this.o_recordImage == null)
            {
                throw new InvalidOperationException("RecordImage not loaded");
            }

            ForestNET.Lib.Global.ILogFiner("check if any fields/columns has changed compared to the record image");

            /* check if any fields/columns has changed compared to the record image */
            foreach (System.Reflection.FieldInfo o_fieldInfo in this.GetType().GetFields())
            {
                /* check if field starts with 'Column', but is not equal to 'Columns' */
                if ((o_fieldInfo.Name.StartsWith("Column")) && (o_fieldInfo.Name.CompareTo("Columns") != 0))
                {
                    /* get field name without 'Column' prefix */
                    string s_column = o_fieldInfo.Name.Substring(6);

                    ForestNET.Lib.Global.ILogFinest("check if field/column '" + s_column + "' has changed");

                    /* compare field/column value of current record and stored image record as this record was retrieved at least once */
                    if (!(this.GetColumnValue(s_column) ?? "NULL").Equals(this.o_recordImage.GetColumnValue(s_column) ?? "NULL"))
                    {
                        ForestNET.Lib.Global.ILogFinest("field/column '" + s_column + "' has changed");

                        /* values are not equal, so at least one field has changed */
                        b_field_has_changed = true;
                        /* break here because one difference is sufficient */
                        break;
                    }
                }
            }

            /* there is nothing to change if flag is false */
            if (!b_field_has_changed)
            {
                ForestNET.Lib.Global.ILogFiner("there is nothing to change");

                return 0;
            }

            /* flag to determine if primary key has changed */
            bool b_primaryKeyChanged = false;

            ForestNET.Lib.Global.ILogFiner("iterate each primary field/column and check if value has changed compared to the record image");

            /* iterate each primary field/column */
            foreach (string s_primary in this.Primary)
            {
                ForestNET.Lib.Global.ILogFinest("check if primary field/column '" + s_primary + "' has changed");

                /* compare field/column value of current record and stored image record as this record was retrieved at least once */
                if (!(this.GetColumnValue(s_primary) ?? "NULL").Equals(this.o_recordImage.GetColumnValue(s_primary) ?? "NULL"))
                {
                    ForestNET.Lib.Global.ILogFinest("primary field/column '" + s_primary + "' has changed");

                    /* values are not equal, so at least one primary field has changed */
                    b_primaryKeyChanged = true;
                    /* break here because one difference is sufficient */
                    break;
                }
            }

            /* check primary key if it has changed */
            if (b_primaryKeyChanged)
            {
                ForestNET.Lib.Global.ILogFiner("create a backup of current filter list");

                /* create a backup of current filter list */
                List<Filter> a_backupFilters = this.Filters;
                /* clear current filter list */
                this.Filters.Clear();

                ForestNET.Lib.Global.ILogFiner("iterate each primary field/column");

                /* iterate each primary field/column */
                foreach (string s_primary in this.Primary)
                {
                    try
                    {
                        ForestNET.Lib.Global.ILogFinest("add primary field/column to Filters property list: '" + s_primary + "'");

                        /* add primary field/column with it's value to filter list */
                        this.Filters.Add(new Filter(s_primary, this.GetColumnValue(s_primary) ?? "NULL", "=", "AND"));

                        ForestNET.Lib.Global.ILogFinest("added primary field/column to Filters property list");
                    }
                    catch (Exception o_exc)
                    {
                        /* skip a field/column if value cannot be retrieved */
                        ForestNET.Lib.Global.ILogFinest("skipped a field/column if value cannot be retrieved: '" + s_primary + "'; " + o_exc);
                    }
                }
                ;

                ForestNET.Lib.Global.ILogFiner("get amount of records with current primary key and it's values");

                /* get amount of records with current primary key and it's values */
                int i_return = this.GetCount();

                ForestNET.Lib.Global.ILogFiner("amount: '" + i_return + "'");

                ForestNET.Lib.Global.ILogFiner("restore filter list with backup");

                /* restore filter list with backup */
                this.Filters = a_backupFilters;

                /* if amount of records is greater than zero, we cannot insert that record */
                if (i_return > 0)
                {
                    ForestNET.Lib.Global.ILogFiner("Primary key violation occurred, create primary key and it's values to throw an exception");

                    string s_primaries = "";
                    string s_primaryValues = "";

                    /* gather all primary fields/columns and it's values */
                    foreach (string s_primary in this.Primary)
                    {
                        s_primaries += s_primary + ", ";
                        s_primaryValues += this.GetColumnValue(s_primary)?.ToString() + ", ";
                    }

                    /* remove last ', ' separator */
                    if (s_primaries.Length > 1)
                    {
                        s_primaries = s_primaries.Substring(0, (s_primaries.Length - 2));
                        s_primaryValues = s_primaryValues.Substring(0, (s_primaryValues.Length - 2));
                    }

                    /* create an exception that we cannot update the record, because of primary key violation */
                    throw new InvalidOperationException("Primary key violation - primary key[" + s_primaries + "](" + s_primaryValues + ") already exists for [" + this.Table + "]");
                }
            }

            /* flag to determine if a unique key has changed */
            bool b_uniqueChanged = false;
            /* gather unique keys in this list */
            List<string> a_checkUniques = [];

            ForestNET.Lib.Global.ILogFiner("iterate each unique key to check if any values changed compared to the record image");

            /* iterate each unique key */
            foreach (string s_unique in this.Unique)
            {
                /* it is possible that a unique constraint exists of multiple columns, separated by semicolon */
                if (s_unique.Contains(';'))
                {
                    string[] a_uniques = s_unique.Split(";");

                    /* iterate each unique key field/column */
                    for (int i = 0; i < a_uniques.Length; i++)
                    {
                        /* compare field/column value of current record and stored image record as this record was retrieved at least once */
                        if (!(this.GetColumnValue(a_uniques[i]) ?? "NULL").Equals(this.o_recordImage.GetColumnValue(a_uniques[i]) ?? "NULL"))
                        {
                            ForestNET.Lib.Global.ILogFiner("unique constraint values changed: '" + s_unique + "'");

                            /* add unique key to list */
                            a_checkUniques.Add(s_unique);
                            /* set flag */
                            b_uniqueChanged = true;
                            /* do not break because we want to check all unique keys */
                        }
                    }
                }
                else
                {
                    /* compare field/column value of current record and stored image record as this record was retrieved at least once */
                    if (!(this.GetColumnValue(s_unique) ?? "NULL").Equals(this.o_recordImage.GetColumnValue(s_unique) ?? "NULL"))
                    {
                        ForestNET.Lib.Global.ILogFiner("unique constraint value changed: '" + s_unique + "'");

                        /* add unique key to list */
                        a_checkUniques.Add(s_unique);
                        /* set flag */
                        b_uniqueChanged = true;
                        /* do not break because we want to check all unique keys */
                    }
                }
            }

            /* check unique keys if at least one has changed values */
            if (b_uniqueChanged)
            {
                ForestNET.Lib.Global.ILogFiner("check unique constraints where value(s) has been changed");

                foreach (string s_unique in a_checkUniques)
                {
                    ForestNET.Lib.Global.ILogFinest("check unique constraint: '" + s_unique + "'");

                    ForestNET.Lib.Global.ILogFinest("create a backup of current filter list");

                    /* create a backup of current filter list */
                    List<Filter> a_backupFilters = this.Filters;
                    /* clear current filter list */
                    this.Filters.Clear();

                    /* it is possible that a unique constraint exists of multiple columns, separated by semicolon */
                    if (s_unique.Contains(';'))
                    {
                        string[] a_uniques = s_unique.Split(";");

                        /* iterate each unique key field/column */
                        for (int i = 0; i < a_uniques.Length; i++)
                        {
                            /* add unique field/column with it's value to filter list */
                            this.Filters.Add(new Filter(a_uniques[i], this.GetColumnValue(a_uniques[i]) ?? "NULL", "=", "AND"));
                        }
                    }
                    else
                    {
                        /* add unique field/column with it's value to filter list */
                        this.Filters.Add(new Filter(s_unique, this.GetColumnValue(s_unique) ?? "NULL", "=", "AND"));
                    }

                    ForestNET.Lib.Global.ILogFinest("get amount of records with current unique key and it's values");

                    /* get amount of records with current unique key and it's values */
                    int i_return = this.GetCount();

                    ForestNET.Lib.Global.ILogFinest("amount: '" + i_return + "'");

                    ForestNET.Lib.Global.ILogFinest("restore filter list with backup");

                    /* restore filter list with backup */
                    this.Filters = a_backupFilters;

                    /* if amount of records is greater than zero, we cannot insert that record */
                    if (i_return > 0)
                    {
                        ForestNET.Lib.Global.ILogFinest("Unique key violation occurred, create unique key and it's values to throw an exception");

                        string s_uniqueValues = "";

                        /* it is possible that a unique constraint exists of multiple columns, separated by semicolon */
                        if (s_unique.Contains(';'))
                        {
                            /* split unique key */
                            string[] a_uniques = s_unique.Split(";");

                            /* iterate each unique field/column */
                            for (int i = 0; i < a_uniques.Length; i++)
                            {
                                /* gather all unique field/column values */
                                s_uniqueValues = (this.GetColumnValue(s_unique) ?? "NULL") + ", ";
                            }

                            /* remove last ', ' separator */
                            if (s_uniqueValues.Length > 1)
                            {
                                s_uniqueValues = s_uniqueValues.Substring(0, (s_uniqueValues.Length - 2));
                            }
                        }
                        else
                        {
                            /* add unique field/column value */
                            s_uniqueValues = this.GetColumnValue(s_unique)?.ToString() ?? "NULL";
                        }

                        /* create an exception that we cannot update the record, because of unique key violation */
                        throw new InvalidOperationException("Unique key violation - unique constraint invalid for [" + s_unique + "](" + s_uniqueValues + ") in table [" + this.Table + "]; unique key already exists");
                    }
                }
            }

            ForestNET.Lib.Global.ILogFiner("create update query");

            /* create update query */
            Query<Update> o_queryUpdate = new(ForestNET.Lib.Global.Instance.BaseGateway, SqlType.UPDATE, this.Table);

            ForestNET.Lib.Global.ILogFiner("created update query");

            /* read out column fields to get values for update query */
            foreach (System.Reflection.FieldInfo o_fieldInfo in this.GetType().GetFields())
            {
                /* check if field starts with 'Column', but is not equal to 'Columns' */
                if ((o_fieldInfo.Name.StartsWith("Column")) && (o_fieldInfo.Name.CompareTo("Columns") != 0))
                {
                    /* get field name without 'Column' prefix */
                    string s_column = o_fieldInfo.Name.Substring(6);

                    ForestNET.Lib.Global.ILogFinest("check if field/column '" + s_column + "' is not part of primary key AND p_b_withUnique = '" + p_b_withUnique + "' OR not part of a unique key");

                    /* check if field/column is not part of primary key, because these fields should not be touched within an update normally */
                    /* check also for field/column not part if a unique or we explicitly allow these fields/columns with p_b_withUnique parameter */
                    if ((!this.Primary.Contains(s_column)) && ((p_b_withUnique) || (!this.Unique.Contains(s_column))))
                    {
                        ForestNET.Lib.Global.ILogFinest("add field/column '" + s_column + "' to column value pair list of update query");

                        /* add field/column to column value pair list of update query */
                        o_queryUpdate.GetQuery<Update>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryUpdate, s_column), this.GetColumnValue(s_column)));

                        ForestNET.Lib.Global.ILogFinest("added field/column to column value pair list of update query");
                    }
                }
            }

            /* flag if where clause has been started */
            bool b_initWhere = false;

            ForestNET.Lib.Global.ILogFiner("take primary key fields for the update filter");

            /* take primary key fields for the update filter */
            foreach (string s_primary in this.Primary)
            {
                ForestNET.Lib.Global.ILogFinest("create where clause object with primary key field '" + s_primary + "'");

                Where o_where = new(o_queryUpdate, new Column(o_queryUpdate, s_primary), this.GetColumnValue(s_primary), "=");

                ForestNET.Lib.Global.ILogFinest("created where clause object");

                if (b_initWhere)
                { /* set filter operator if where clause has been started */
                    /* add 'AND' filter operator */
                    o_where.FilterOperator = "AND";
                }

                /* at least here where clause has been started */
                b_initWhere = true;

                ForestNET.Lib.Global.ILogFinest("add where clause object to query update");

                /* add where clause to update query */
                o_queryUpdate.GetQuery<Update>()?.Where.Add(o_where);

                ForestNET.Lib.Global.ILogFinest("added where clause object to query update");
            }

            ForestNET.Lib.Global.ILogFiner("execute query update");

            /* execute update query and get result */
            List<Dictionary<string, Object?>> a_rows = ((this.OtherBaseSource != null) ? this.OtherBaseSource.Invoke(o_queryUpdate) : ForestNET.Lib.Global.Instance.Base?.FetchQuery(o_queryUpdate)) ?? [];

            ForestNET.Lib.Global.ILogFiner("executed query update");

            /* result must be exactly one row */
            if (a_rows.Count == 1)
            {
                ForestNET.Lib.Global.ILogFiner("result must be exactly one row");

                if (a_rows[0].ContainsKey("AffectedRows"))
                { /* check if 'AffectedRows' is available */
                    /* return 0 if 'AffectedRows' value is lower than one */
                    if (Convert.ToInt32(a_rows[0]["AffectedRows"]?.ToString()) < 1)
                    {
                        ForestNET.Lib.Global.ILogFiner("return 0 if 'AffectedRows' value is lower than one");

                        return 0;
                    }
                    else
                    {
                        ForestNET.Lib.Global.ILogFiner("return 'AffectedRows' value");

                        /* return 'AffectedRows' value */
                        return Convert.ToInt32(a_rows[0]["AffectedRows"]?.ToString());
                    }
                }
                else
                { /* if we have not 'AffectedRows', return -1 as error */
                    ForestNET.Lib.Global.ILogFiner("if we have not 'AffectedRows', return -1 as error");

                    return -1;
                }
            }
            else
            { /* result is not just one row, return -1 as error */
                ForestNET.Lib.Global.ILogFiner("result is not just one row, return -1 as error");

                return -1;
            }
        }

        /// <summary>
        /// Deletes a record with current primary columns of inherited class
        /// </summary>
        /// <returns>primary 'AffectedRows' amount value, -1 as error</returns>
        /// <exception cref="FieldAccessException">exception accessing column type, column name or just column value of current result set record</exception>
        /// <exception cref="AccessViolationException">exception accessing column type, column name or just column value of current result set record</exception>
        /// <exception cref="MissingFieldException">column field does not exist</exception>
        /// <exception cref="TypeLoadException">if the class that declares the underlying constructor represents an abstract class</exception>
        /// <exception cref="MissingMethodException">could not retrieve declared constructor</exception>
        public int DeleteRecord()
        {
            ForestNET.Lib.Global.ILogFiner("create query delete");

            /* create delete query */
            Query<Delete> o_queryDelete = new(ForestNET.Lib.Global.Instance.BaseGateway, SqlType.DELETE, this.Table);

            ForestNET.Lib.Global.ILogFiner("created query delete");

            /* flag if where clause has been started */
            bool b_initWhere = false;

            ForestNET.Lib.Global.ILogFiner("take primary key fields for the delete filter");

            /* take primary key fields for the delete filter */
            foreach (string s_primary in this.Primary)
            {
                ForestNET.Lib.Global.ILogFinest("create where clause object with primary key field '" + s_primary + "'");

                Where o_where = new(o_queryDelete, new Column(o_queryDelete, s_primary), this.GetColumnValue(s_primary), "=");

                ForestNET.Lib.Global.ILogFinest("created where clause object");

                if (b_initWhere)
                { /* set filter operator if where clause has been started */
                    /* add 'AND' filter operator */
                    o_where.FilterOperator = "AND";
                }

                /* at least here where clause has been started */
                b_initWhere = true;

                ForestNET.Lib.Global.ILogFinest("add where clause object to query delete");

                /* add where clause to delete query */
                o_queryDelete.GetQuery<Delete>()?.Where.Add(o_where);

                ForestNET.Lib.Global.ILogFinest("added where clause object to query delete");
            }

            ForestNET.Lib.Global.ILogFiner("execute query delete");

            /* execute delete query and get result */
            List<Dictionary<string, Object?>> a_rows = ((this.OtherBaseSource != null) ? this.OtherBaseSource.Invoke(o_queryDelete) : ForestNET.Lib.Global.Instance.Base?.FetchQuery(o_queryDelete)) ?? [];

            ForestNET.Lib.Global.ILogFiner("executed query delete");

            /* result must be exactly one row */
            if (a_rows.Count == 1)
            {
                ForestNET.Lib.Global.ILogFiner("check if 'AffectedRows' is available");

                if (a_rows[0].ContainsKey("AffectedRows"))
                { /* check if 'AffectedRows' is available */
                    /* return 0 if 'AffectedRows' value is lower than one */
                    if (Convert.ToInt32(a_rows[0]["AffectedRows"]?.ToString()) < 1)
                    {
                        ForestNET.Lib.Global.ILogFiner("return 0 if 'AffectedRows' value is lower than one");

                        return 0;
                    }
                    else
                    {
                        ForestNET.Lib.Global.ILogFiner("return 'AffectedRows' value");

                        /* return 'AffectedRows' value */
                        return Convert.ToInt32(a_rows[0]["AffectedRows"]?.ToString());
                    }
                }
                else
                { /* if we have not 'AffectedRows', return -1 as error */
                    ForestNET.Lib.Global.ILogFiner("if we have not 'AffectedRows', return -1 as error");

                    return -1;
                }
            }
            else
            { /* result is not just one row, return -1 as error */
                ForestNET.Lib.Global.ILogFiner("result is not just one row, return -1 as error");

                return -1;
            }
        }

        /// <summary>
        /// USE WITH CAUTION<br /><br />
        /// Truncates a whole table with information of inherited class
        /// </summary>
        /// <returns>primary 'AffectedRows' amount value, -1 as error</returns>
        /// <exception cref="FieldAccessException">exception accessing column type, column name or just column value of current result set record</exception>
        /// <exception cref="AccessViolationException">exception accessing column type, column name or just column value of current result set record</exception>
        /// <exception cref="MissingFieldException">column field does not exist</exception>
        /// <exception cref="TypeLoadException">if the class that declares the underlying constructor represents an abstract class</exception>
        /// <exception cref="MissingMethodException">could not retrieve declared constructor</exception>
        public int TruncateTable()
        {
            ForestNET.Lib.Global.ILogFiner("create query truncate");

            /* create truncate query */
            Query<Truncate> o_queryTruncate = new(ForestNET.Lib.Global.Instance.BaseGateway, SqlType.TRUNCATE, this.Table);

            ForestNET.Lib.Global.ILogFiner("created query truncate");

            ForestNET.Lib.Global.ILogFiner("execute query truncate");

            /* execute truncate query and get result */
            List<Dictionary<string, Object?>> a_rows = ((this.OtherBaseSource != null) ? this.OtherBaseSource.Invoke(o_queryTruncate) : ForestNET.Lib.Global.Instance.Base?.FetchQuery(o_queryTruncate)) ?? [];

            ForestNET.Lib.Global.ILogFiner("executed query truncate");

            /* result must be exactly one row */
            if (a_rows.Count == 1)
            {
                ForestNET.Lib.Global.ILogFiner("check if 'AffectedRows' is available");

                if (a_rows[0].ContainsKey("AffectedRows"))
                { /* check if 'AffectedRows' is available */
                    /* return 0 if 'AffectedRows' value is lower than one */
                    if (Convert.ToInt32(a_rows[0]["AffectedRows"]?.ToString()) < 1)
                    {
                        ForestNET.Lib.Global.ILogFiner("return 0 if 'AffectedRows' value is lower than one");

                        return 0;
                    }
                    else
                    {
                        ForestNET.Lib.Global.ILogFiner("return 'AffectedRows' value");

                        /* return 'AffectedRows' value */
                        return Convert.ToInt32(a_rows[0]["AffectedRows"]?.ToString());
                    }
                }
                else
                { /* if we have not 'AffectedRows', return -1 as error */
                    ForestNET.Lib.Global.ILogFiner("if we have not 'AffectedRows', return -1 as error");

                    return -1;
                }
            }
            else
            { /* result is not just one row, return -1 as error */
                ForestNET.Lib.Global.ILogFiner("result is not just one row, return -1 as error");

                return -1;
            }
        }
    }
}