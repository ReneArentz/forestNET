namespace ForestNET.Lib.Net.Sock.Com
{
    /// <summary>
    /// Interface for shared memory class to use it as generic type in other classes
    /// </summary>
    public interface ISharedMemory
    {
        public int AmountFields();
        public System.Threading.Tasks.Task<Object> GetField(string p_s_field);
        public System.Threading.Tasks.Task<Object> GetField(string p_s_field, bool p_b_convertNullForTransport);
        public System.Threading.Tasks.Task SetField(string p_s_field, Object? o_value);
        public string ReturnFields();
        public int ReturnFieldNumber(string p_s_field);
        public string ReturnFieldName(int p_i_fieldNumber);
        public System.Threading.Tasks.Task<List<string>> GetChangedFields();
        public System.Threading.Tasks.Task<List<string>> GetChangedFields(bool p_b_bidirectionalMirror);
        public System.Threading.Tasks.Task<List<string>> GetChangedFields(bool p_b_updateMirror, bool p_b_bidirectionalMirror);
        public System.Threading.Tasks.Task UpdateMirror();
        public System.Threading.Tasks.Task UpdateMirror(bool p_b_updateBidirectionalMirror);
        public System.Threading.Tasks.Task UpdateWholeMirror(ISharedMemory p_o_object);
        public System.Threading.Tasks.Task UpdateWholeMirror(ISharedMemory p_o_object, bool p_b_updateBidirectionalMirror);
        public System.Threading.Tasks.Task InitiateMirrors();
        public System.Threading.Tasks.Task UpdateMirrorField(string p_s_field, bool p_b_updateBidirectionalMirror);
    }

    /// <summary>
    /// Abstract class to extend and watch any class in use to get to know which fields of a class are changed in time.
    /// <br />
    /// T - Class definition of inherited class, e.g. Test extends de.forestj.lib.net.sock.com.SharedMemory<Test.class>
    /// </summary>
    public abstract class SharedMemory<T> : ISharedMemory where T : ISharedMemory
    {

        /* Constants */

        private static readonly List<string> ReservedFields = ["ReservedFields", "a_fields", "o_mirror", "MirrorClass", "o_mirrorBidirectional", "o_readWriteLock", "AmountFields", "CancellationTokenSource"];

        /* Fields */

        private readonly List<string> a_fields = [];
        private T? o_mirror = default;
        private T? o_mirrorBidirectional = default;
        private readonly SemaphoreSlim o_readWriteLock = new(1);

        /* Properties */

        protected System.Type? MirrorClass = null;
        public CancellationTokenSource? CancellationTokenSource { get; set; }

        /* Methods */

        /// <summary>
        /// Superior constructor of a class inheriting abstract SharedMemory class
        /// </summary>
        /// <exception cref="ArgumentNullException">mirror class not set by Init method</exception>
        /// <exception cref="InvalidOperationException">field is a reserved field from abstract class | field is not private</exception>
        public SharedMemory()
        {
            this.Init();

            /* check if mirror class has been set in Init method */
            if (this.MirrorClass == null)
            {
                throw new ArgumentNullException("You must specify an image class within the Init-method.");
            }

            ForestNET.Lib.Global.ILogConfig("mirror class set for SharedMemory: '" + this.MirrorClass.FullName + "'");

            /* iterate each class field */
            foreach (System.Reflection.FieldInfo o_fieldInfo in this.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly))
            {
                /* check if a field in inheriting class is not colliding with a reserved field name of SharedMemory class */
                if (SharedMemory<T>.ReservedFields.Contains(o_fieldInfo.Name))
                {
                    throw new InvalidOperationException("Cannot use field[" + o_fieldInfo.Name + "]. Reserved field names " + string.Join(",", [.. SharedMemory<T>.ReservedFields.Select(x => x.ToString())]) + " are not accessible");
                }

                /* because of security reasons, each field which will be watched must be private only */
                if (!o_fieldInfo.IsPrivate)
                {
                    throw new InvalidOperationException("Field[" + o_fieldInfo.Name + "] must be private only");
                }

                /* add field to list to get to know when it's value is changed in time */
                this.a_fields.Add(o_fieldInfo.Name);

                ForestNET.Lib.Global.ILogConfig("added field '" + o_fieldInfo.Name + "' to watch list");
            }

            /* sort list of fields */
            this.a_fields.Sort();

            /* create cancellation token source */
            this.CancellationTokenSource = new();
        }

        /// <summary>
        /// Abstract Init function so any class inheriting from SharedMemory<T> must have this method.
        /// declaring mirror class individually for every inheritance for later use.
        /// </summary>
        abstract protected void Init();

        /// <summary>
        /// Method to get the amount of fields of shared memory class
        /// </summary>
        public int AmountFields()
        {
            return this.a_fields.Count;
        }

        /// <summary>
        /// Method to check if a field exists in inherited class
        /// </summary>
        /// <param name="p_s_field">field name</param>
        /// <returns>bool			true - field exist, false - field does not exist</returns>
        protected bool FieldExists(string p_s_field)
        {
            try
            {
                /* use other method index return value */
                return this.ReturnFieldNumber(p_s_field) >= 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Method to retrieve field value of current class
        /// </summary>
        /// <param name="p_s_field">field name</param>
        /// <returns>object type, nullable</returns>
        /// <exception cref="MissingFieldException">field does not exist</exception>
        /// <exception cref="FieldAccessException">cannot access field</exception>
        /// <exception cref="InvalidCastException">issue converting value into TimeSpan</exception>
        public async System.Threading.Tasks.Task<Object> GetField(string p_s_field)
        {
            return await this.GetField(p_s_field, false);
        }

        /// <summary>
        /// Method to retrieve field value of current class
        /// </summary>
        /// <param name="p_s_field">field name</param>
        /// <param name="p_b_convertNullForTransport">true - convert to NullValue class value to transport null values over the network, false - normal retrieve</param>
        /// <returns>object type, nullable</returns>
        /// <exception cref="MissingFieldException">field does not exist</exception>
        /// <exception cref="FieldAccessException">cannot access field</exception>
        /// <exception cref="InvalidCastException">issue converting value into TimeSpan</exception>
        public async System.Threading.Tasks.Task<Object> GetField(string p_s_field, bool p_b_convertNullForTransport)
        {
            try
            {
                /* set  lock, so only one thread at a time has access to a field */
                System.Threading.Tasks.Task o_taskReadLock = this.o_readWriteLock.WaitAsync(this.CancellationTokenSource?.Token ?? default);
                System.Threading.Tasks.Task o_taskTimeoutPool = System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(30000), this.CancellationTokenSource?.Token ?? default);

                System.Threading.Tasks.Task o_completedTask = await System.Threading.Tasks.Task.WhenAny(o_taskReadLock, o_taskTimeoutPool);

                if (o_completedTask == o_taskReadLock)
                {
                    if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("get field value from '" + p_s_field + "'");

                    /* get column info */
                    System.Reflection.FieldInfo o_fieldInfo = this.GetType().GetField(p_s_field, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly) ?? throw new MissingFieldException("Field '" + p_s_field + "' not found in class");
                    /* get column type */
                    System.Type? o_fieldType = o_fieldInfo.FieldType;
                    /* get type name */
                    string s_class = o_fieldType.FullName ?? "";

                    /* variable for class type */
                    System.Type o_classType = typeof(Object);

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
                    Object? o_foo = this.GetType().GetField(p_s_field, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly)?.GetValue(this);

                    /* handle cast */
                    if (o_foo != null)
                    {
                        if (!b_handleTimeSpan)
                        {
                            /* do cast only, if we have not a generic list or array as field type */
                            if (!(o_fieldType.IsGenericType || o_fieldType.IsArray))
                            {
                                /* cast object of column to target class type */
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

                    /* if casted object value is null, convert to NullValue class value to transport null values over the network */
                    if ((o_foo == null) && (p_b_convertNullForTransport))
                    {
                        o_foo = (Object?)new NullValue();
                    }

                    /* return casted object */
#pragma warning disable CS8603 // Possible null reference return.
                    return o_foo;
#pragma warning restore CS8603 // Possible null reference return.
                }
                else
                {
                    if (!o_taskTimeoutPool.IsCanceled && !o_taskTimeoutPool.IsCompleted && !o_taskTimeoutPool.IsFaulted)
                    {
                        o_taskTimeoutPool.Dispose();
                    }

                    throw new OperationCanceledException("timeout for reading field value[" + p_s_field + "]");
                }
            }
            finally
            {
                /* release lock */
                this.o_readWriteLock.Release();
            }
        }

        /// <summary>
        /// Method to set a field value of current class
        /// </summary>
        /// <param name="p_s_field">field name</param>
        /// <param name="p_o_value">object value which will be set as field value</param>
        /// <exception cref="MissingFieldException">field does not exist</exception>
        /// <exception cref="FieldAccessException">cannot access field</exception>
        public async System.Threading.Tasks.Task SetField(string p_s_field, Object? p_o_value)
        {
            try
            {
                /* set lock, so only one thread at a time has access to a field */
                System.Threading.Tasks.Task o_taskWriteLock = this.o_readWriteLock.WaitAsync(this.CancellationTokenSource?.Token ?? default);
                System.Threading.Tasks.Task o_taskTimeoutPool = System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(30000), this.CancellationTokenSource?.Token ?? default);

                System.Threading.Tasks.Task o_completedTask = await System.Threading.Tasks.Task.WhenAny(o_taskWriteLock, o_taskTimeoutPool);

                if (o_completedTask == o_taskWriteLock)
                {
                    if (p_o_value != null)
                    {
                        /* get parameter value type */
                        string? s_valueType = p_o_value.GetType().FullName;

                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("set field value for: '" + p_s_field + "'" + "\t\field type: " + GetType().GetField(p_s_field)?.GetType().FullName + "\t\tvalue type: " + s_valueType);

                        if ((s_valueType != null) && (s_valueType.Contains("string", StringComparison.CurrentCultureIgnoreCase)))
                        {
                            string? s_foo = p_o_value.ToString();

                            /* recognize empty null value */
                            if ((ForestNET.Lib.Helper.IsStringEmpty(s_foo)) || ((s_foo != null) && s_foo.Equals("NULL")))
                            {
                                p_o_value = null;
                            }
                        }
                    }

                    /* if object value is of instance NullValue, then we just have our null */
                    if (p_o_value is NullValue)
                    {
                        p_o_value = null;
                    }

                    /* set column value, accessing 'this' class and field with column name */
                    GetType().GetField(p_s_field, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly)?.SetValue(this, p_o_value);
                }
                else
                {
                    if (!o_taskTimeoutPool.IsCanceled && !o_taskTimeoutPool.IsCompleted && !o_taskTimeoutPool.IsFaulted)
                    {
                        o_taskTimeoutPool.Dispose();
                    }

                    throw new OperationCanceledException("timeout for writing field value[" + p_s_field + "]");
                }
            }
            finally
            {
                /* release lock */
                this.o_readWriteLock.Release();
            }
        }

        /// <summary>
        /// Easy method to return all fields with their values of inherited class
        /// </summary>
        /// <returns>string	a string line of all fields with their values "field_name = field_value|"</returns>
        public string ReturnFields()
        {
            string s_foo = "";

            /* iterate each field of inherited class */
            foreach (string s_field in this.a_fields)
            {
                /* check if field is not in the list of reserved field names of ShareMemory class */
                if (!SharedMemory<T>.ReservedFields.Contains(s_field))
                {
                    try
                    {
                        /* get field value */
                        Object? o_foo = this.GetField(s_field).Result;
                        /* set field string value to 'NULL', in case value is null */
                        string s_object = "NULL";

                        if (o_foo != null)
                        {
                            if (o_foo.GetType().IsArray)
                            {
                                s_object = "[";

                                Array a_array = (Array)o_foo;

                                foreach (Object? o_bar in a_array)
                                {
                                    s_object += o_bar?.ToString() ?? "NULL";
                                    s_object += ", ";
                                }

                                if (a_array.Length > 0)
                                {
                                    s_object = s_object.Substring(0, s_object.Length - 2);
                                }

                                s_object += "]";
                            }
                            else if (o_foo.GetType().IsGenericType)
                            {
                                s_object = "[";

                                System.Collections.IList a_list = (System.Collections.IList)o_foo;

                                foreach (Object? o_bar in a_list)
                                {
                                    s_object += o_bar?.ToString() ?? "NULL";
                                    s_object += ", ";
                                }

                                if (a_list.Count > 0)
                                {
                                    s_object = s_object.Substring(0, s_object.Length - 2);
                                }

                                s_object += "]";
                            }
                            else
                            {
                                /* if field value is not null, use tostring method */
                                s_object = o_foo.ToString() ?? "NULL";
                            }
                        }

                        /* add field name and its value to return string */
                        s_foo += s_field + " = " + s_object + "|";
                    }
                    catch (Exception)
                    {
                        /* just continue if field name or field value cannot be retrieved */
                        s_foo += s_field + " = COULD_NOT_RETRIEVE|";
                    }
                }
            }

            return s_foo;
        }

        /// <summary>
        /// Return field number of class which inherited abstract SharedMemory class with string parameter
        /// </summary>
        /// <param name="p_s_field">field name</param>
        /// <returns>-1 - field does not exist, 0..n - field number</returns>
        /// <exception cref="AccessViolationException">field is not accessible</exception>
        public int ReturnFieldNumber(string p_s_field)
        {
            int i_return = -1;

            /* check if field is not in the list of reserved field names of ShareMemory class */
            if (SharedMemory<T>.ReservedFields.Contains(p_s_field))
            {
                throw new AccessViolationException("Cannot get field[" + p_s_field + "]. Reserved field names " + string.Join(",", [.. SharedMemory<T>.ReservedFields.Select(x => x.ToString())]) + " are not accessible");
            }

            /* iterate each field of inherited class */
            for (int i = 0; i < this.a_fields.Count; i++)
            {
                /* field name must match parameter value */
                if (this.a_fields[i].Equals(p_s_field))
                {
                    i_return = i + 1; /* +1, because return value is initialized with -1 */
                    break;
                }
            }

            return i_return;
        }

        /// <summary>
        /// Return field name of class which inherited abstract SharedMemory class with index parameter
        /// </summary>
        /// <param name="p_i_fieldNumber">field number as index</param>
        /// <returns>field name</returns>
        /// <exception cref="IndexOutOfBoundsException">if the field number is out of range (index < 0 || index >= size())</exception>
        public string ReturnFieldName(int p_i_fieldNumber)
        {
            /* get field name by index field number */
            return this.a_fields[--p_i_fieldNumber];
        }

        /// <summary>
        /// Get a list of all fields of current class which have changed
        /// </summary>
        /// <returns>List of string</returns>
        /// <exception cref="Exception">any exception which can happen during creating a new instance</exception>
        /// <exception cref="MissingFieldException">a field does not exist</exception>
        /// <exception cref="AccessViolationException">cannot access field, must be public</exception>
        /// <exception cref="ClassCastException">if the object is not null and is not assignable to the type T</exception>
        public async System.Threading.Tasks.Task<List<string>> GetChangedFields()
        {
            return await this.GetChangedFields(false);
        }

        /// <summary>
        /// Get a list of all fields of current class which have changed
        /// </summary>
        /// <param name="p_b_updateMirror">true - update mirror object field values, so on next check we get the new changed fields</param>
        /// <returns>List of string</returns>
        /// <exception cref="Exception">any exception which can happen during creating a new instance</exception>
        /// <exception cref="MissingFieldException">a field does not exist</exception>
        /// <exception cref="AccessViolationException">cannot access field, must be public</exception>
        /// <exception cref="ClassCastException">if the object is not null and is not assignable to the type T</exception>
        public async System.Threading.Tasks.Task<List<string>> GetChangedFields(bool p_b_updateMirror)
        {
            return await this.GetChangedFields(p_b_updateMirror, false);
        }

        /// <summary>
        /// Get a list of all fields of current class which have changed
        /// </summary>
        /// <param name="p_b_updateMirror">true - update mirror object field values, so on next check we get the new changed fields</param>
        /// <param name="p_b_bidirectionalMirror">true - update bidirectional mirror object field values, so on next check we get the new changed fields</param>
        /// <returns>List of string</returns>
        /// <exception cref="Exception">any exception which can happen during creating a new instance</exception>
        /// <exception cref="MissingFieldException">a field does not exist</exception>
        /// <exception cref="AccessViolationException">cannot access field, must be public</exception>
        /// <exception cref="ClassCastException">if the object is not null and is not assignable to the type T</exception>
        public async System.Threading.Tasks.Task<List<string>> GetChangedFields(bool p_b_updateMirror, bool p_b_bidirectionalMirror)
        {
            if ((p_b_bidirectionalMirror) && (this.o_mirrorBidirectional == null))
            {
                /* bidirectional mirror object is null, we must update bidirectional mirror to create a new instance of it */
                await this.UpdateMirror(p_b_bidirectionalMirror);
            }
            else if (this.o_mirror == null)
            {
                /* mirror object is null, we must update mirror to create a new instance of it */
                await this.UpdateMirror();
            }

            List<string> a_changedFields = [];

            /* iterate each field of inherited class */
            for (int i = 0; i < this.a_fields.Count; i++)
            {
                /* check if field is not in the list of reserved field names of ShareMemory class */
                if (!SharedMemory<T>.ReservedFields.Contains(this.a_fields[i]))
                {
                    /* get field value object */
                    Object? o_foo = await this.GetField(this.a_fields[i], true);
                    Object? o_foo2;

                    if (p_b_bidirectionalMirror)
                    {
                        /* get field value object of bidirectional mirror object */
                        o_foo2 = (this.o_mirrorBidirectional == null) ? null : await this.o_mirrorBidirectional.GetField(this.a_fields[i], true);
                    }
                    else
                    {
                        /* get field value object of mirror object */
                        o_foo2 = (this.o_mirror == null) ? null : await this.o_mirror.GetField(this.a_fields[i], true);
                    }

                    /* if inherited field value object and (bidirectional) mirror field value object are not equal OR both are of not null or not instance of NullValue */
                    if (!(((o_foo != null) && (o_foo.Equals(o_foo2))) || ((o_foo == null) && (o_foo2 == null)) || ((o_foo is NullValue) && (o_foo2 is NullValue))))
                    {
                        /* add field to changed fields list */
                        a_changedFields.Add(this.a_fields[i]);
                    }
                }
            }

            /* update mirror or bidirectional mirror object field values, so on next check we get the new changed fields */
            if (p_b_updateMirror)
            {
                await this.UpdateMirror(p_b_bidirectionalMirror);
            }

            return a_changedFields;
        }

        /// <summary>
        /// Update mirror object fields with inherited class fields. If mirror field objects are null, new instances will be created
        /// </summary>
        /// <exception cref="Exception">any exception which can happen during creating a new instance</exception>
        /// <exception cref="MissingFieldException">a field does not exist</exception>
        /// <exception cref="AccessViolationException">cannot access field, must be public</exception>
        public async System.Threading.Tasks.Task UpdateMirror()
        {
            await this.UpdateMirror(false);
        }

        /// <summary>
        /// Update (bidirectional) mirror object fields with inherited class fields. If mirror field objects are null, new instances will be created
        /// </summary>
        /// <param name="p_b_updateBidirectionalMirror">true - update all fields of bidirectional mirror object, false - update all fields of mirror object</param>
        /// <exception cref="Exception">any exception which can happen during creating a new instance</exception>
        /// <exception cref="MissingFieldException">a field does not exist</exception>
        /// <exception cref="AccessViolationException">cannot access field, must be public</exception>
        public async System.Threading.Tasks.Task UpdateMirror(bool p_b_updateBidirectionalMirror)
        {
            /* placeholder variable for mirror or bidirectional mirror object */
            T? o_inheritedInstance;

            if (p_b_updateBidirectionalMirror)
            {
                /* set placeholder variable with bidirectional mirror object */
                o_inheritedInstance = this.o_mirrorBidirectional;
            }
            else
            {
                /* set placeholder variable with mirror object */
                o_inheritedInstance = this.o_mirror;
            }

            /* do we have to create new instance of (bidirectional) mirror object? */
            if (o_inheritedInstance == null)
            {
                try
                {
                    /* set lock, so only one thread at a time has access to a field */
                    System.Threading.Tasks.Task o_taskWriteLock = this.o_readWriteLock.WaitAsync(this.CancellationTokenSource?.Token ?? default);
                    System.Threading.Tasks.Task o_taskTimeoutPool = System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(30000), this.CancellationTokenSource?.Token ?? default);

                    System.Threading.Tasks.Task o_completedTask = await System.Threading.Tasks.Task.WhenAny(o_taskWriteLock, o_taskTimeoutPool);

                    if (o_completedTask == o_taskWriteLock)
                    {
                        /* check if mirror class is not null */
                        if (this.MirrorClass == null)
                        {
                            throw new NullReferenceException("You must specify a mirror class within the Init-method.");
                        }

                        /* create new instance of (bidirectional) mirror object */
                        o_inheritedInstance = (T?)Activator.CreateInstance(this.MirrorClass) ?? throw new NullReferenceException("mirror object is null");
                    }
                    else
                    {
                        if (!o_taskTimeoutPool.IsCanceled && !o_taskTimeoutPool.IsCompleted && !o_taskTimeoutPool.IsFaulted)
                        {
                            o_taskTimeoutPool.Dispose();
                        }

                        throw new OperationCanceledException("timeout for create new instance of (bidirectional) mirror object");
                    }
                }
                finally
                {
                    /* release lock */
                    this.o_readWriteLock.Release();
                }
            }

            /* iterate each field of inherited class */
            for (int i = 0; i < this.a_fields.Count; i++)
            {
                /* check if field is not in the list of reserved field names of ShareMemory class */
                if (!SharedMemory<T>.ReservedFields.Contains(this.a_fields[i]))
                {
                    /* update (bidirectional) mirror object field value with inherited class field value */
                    await o_inheritedInstance.SetField(this.a_fields[i], await this.GetField(this.a_fields[i]));
                }
            }

            if (p_b_updateBidirectionalMirror)
            {
                /* reverse placeholder variable with bidirectional mirror object */
                this.o_mirrorBidirectional = o_inheritedInstance;
            }
            else
            {
                /* reverse placeholder variable with mirror object */
                this.o_mirror = o_inheritedInstance;
            }
        }

        /// <summary>
        /// Update whole mirror object with inherited parameter object
        /// </summary>
        /// <param name="p_o_object">object as base for updating whole mirror</param>
        /// <exception cref="Exception">any exception which can happen during creating a new instance</exception>
        /// <exception cref="MissingFieldException">a field does not exist</exception>
        /// <exception cref="AccessViolationException">cannot access field, must be public</exception>
        public async System.Threading.Tasks.Task UpdateWholeMirror(ISharedMemory p_o_object)
        {
            await this.UpdateWholeMirror(p_o_object, false);
        }

        /// <summary>
        /// Update (bidirectional) mirror object with inherited parameter object
        /// </summary>
        /// <param name="p_o_object">object as base for updating whole (bidirectional) mirror</param>
        /// <param name="p_b_updateBidirectionalMirror">true - update bidirectional mirror object, false - update mirror object</param>
        /// <exception cref="Exception">any exception which can happen during creating a new instance</exception>
        /// <exception cref="MissingFieldException">a field does not exist</exception>
        /// <exception cref="AccessViolationException">cannot access field, must be public</exception>
        public async System.Threading.Tasks.Task UpdateWholeMirror(ISharedMemory p_o_object, bool p_b_updateBidirectionalMirror)
        {
            /* iterate all fields from shared memory object parameter */
            for (int i = 1; i <= p_o_object.AmountFields(); i++)
            {
                string s_field = p_o_object.ReturnFieldName(i);

                /* check if field exists in this current shared memory class */
                if (this.FieldExists(s_field))
                {
                    /* update field value */
                    await this.SetField(s_field, await p_o_object.GetField(s_field));
                }
            }

            /* update (bidirectional) mirror object */
            await this.UpdateMirror(p_b_updateBidirectionalMirror);
        }

        /// <summary>
        /// Initiate mirror and bidirectional mirror object fields with inherited class fields. If mirror field objects are null, new instances will be created
        /// </summary>
        /// <exception cref="Exception">any exception which can happen during creating a new instance</exception>
        /// <exception cref="MissingFieldException">a field does not exist</exception>
        /// <exception cref="AccessViolationException">cannot access field, must be public</exception>
        public async System.Threading.Tasks.Task InitiateMirrors()
        {
            /* initiate mirror object */
            if (this.o_mirror == null)
            {
                await this.UpdateMirror(false);
            }

            /* initiate bidirectional mirror object */
            if (this.o_mirrorBidirectional == null)
            {
                await this.UpdateMirror(true);
            }
        }

        /// <summary>
        /// Update (bidirectional) mirror object field value with inherited class field value by field name. If mirror field object is null a new instance will be created
        /// </summary>
        /// <param name="p_s_field">field name</param>
        /// <param name="p_b_updateBidirectionalMirror">true - update field of bidirectional mirror object, false - update field of mirror object</param>
        /// <exception cref="Exception">any exception which can happen during creating a new instance</exception>
        /// <exception cref="MissingFieldException">field does not exist</exception>
        /// <exception cref="AccessViolationException">cannot access field, must be public</exception>
        public async System.Threading.Tasks.Task UpdateMirrorField(string p_s_field, bool p_b_updateBidirectionalMirror)
        {
            /* check if field really exists in inherited class */
            if (!this.FieldExists(p_s_field))
            {
                throw new MissingFieldException("Field[" + p_s_field + "] does not exist");
            }

            /* check if field is not in the list of reserved field names of ShareMemory class */
            if (SharedMemory<T>.ReservedFields.Contains(p_s_field))
            {
                throw new AccessViolationException("Cannot set field[" + p_s_field + "]. Reserved field names " + string.Join(",", [.. SharedMemory<T>.ReservedFields.Select(x => x.ToString())]) + " are not accessible");
            }

            /* placeholder variable for mirror or bidirectional mirror object */
            T? o_inheritedInstance = default;

            if (p_b_updateBidirectionalMirror)
            {
                /* set placeholder variable with bidirectional mirror object */
                o_inheritedInstance = this.o_mirrorBidirectional;
            }
            else
            {
                /* set placeholder variable with mirror object */
                o_inheritedInstance = this.o_mirror;
            }

            /* do we have to create new instance of (bidirectional) mirror object? */
            if (o_inheritedInstance == null)
            {
                try
                {
                    /* set lock, so only one thread at a time has access to a field */
                    System.Threading.Tasks.Task o_taskWriteLock = this.o_readWriteLock.WaitAsync(this.CancellationTokenSource?.Token ?? default);
                    System.Threading.Tasks.Task o_taskTimeoutPool = System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(30000), this.CancellationTokenSource?.Token ?? default);

                    System.Threading.Tasks.Task o_completedTask = await System.Threading.Tasks.Task.WhenAny(o_taskWriteLock, o_taskTimeoutPool);

                    if (o_completedTask == o_taskWriteLock)
                    {
                        /* check if mirror class is not null */
                        if (this.MirrorClass == null)
                        {
                            throw new NullReferenceException("You must specify a mirror class within the Init-method.");
                        }

                        /* create new instance of (bidirectional) mirror object */
                        o_inheritedInstance = (T?)Activator.CreateInstance(this.MirrorClass) ?? throw new NullReferenceException("mirror object is null");
                    }
                    else
                    {
                        if (!o_taskTimeoutPool.IsCanceled && !o_taskTimeoutPool.IsCompleted && !o_taskTimeoutPool.IsFaulted)
                        {
                            o_taskTimeoutPool.Dispose();
                        }

                        throw new OperationCanceledException("timeout for create new instance of (bidirectional) mirror object");
                    }
                }
                finally
                {
                    /* release lock */
                    this.o_readWriteLock.Release();
                }
            }

            /* update (bidirectional) mirror object field value with inherited class field value */
            await o_inheritedInstance.SetField(p_s_field, await this.GetField(p_s_field));

            if (p_b_updateBidirectionalMirror)
            {
                /* reverse placeholder variable with bidirectional mirror object */
                this.o_mirrorBidirectional = o_inheritedInstance;
            }
            else
            {
                /* reverse placeholder variable with mirror object */
                this.o_mirror = o_inheritedInstance;
            }
        }
    }
}