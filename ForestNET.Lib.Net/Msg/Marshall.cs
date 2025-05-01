namespace ForestNET.Lib.Net.Msg
{
    /// <summary>
    /// Static methods for marshalling objects with fields of primitive types of supported types.
    /// </summary>
    public class Marshall
    {

        /* Fields */

        private static readonly List<Type> a_allowedTypes = [
            typeof(bool),
            typeof(bool[]),

            typeof(byte),
            typeof(byte[]),

            typeof(sbyte),
            typeof(sbyte[]),

            typeof(char),
            typeof(char[]),

            typeof(float),
            typeof(float[]),

            typeof(double),
            typeof(double[]),

            typeof(short),
            typeof(short[]),

            typeof(ushort),
            typeof(ushort[]),

            typeof(int),
            typeof(int[]),

            typeof(uint),
            typeof(uint[]),

            typeof(long),
            typeof(long[]),

            typeof(ulong),
            typeof(ulong[]),

            typeof(string),
            typeof(string[]),
            typeof(string?[]),

            typeof(DateTime),
            typeof(DateTime?),
            typeof(DateTime[]),
            typeof(DateTime?[]),

            typeof(decimal),
            typeof(decimal[]),
        ];

        /* Properties */

        /* Methods */

        /// <summary>
        /// Marshall object with all fields of primitive types or supported types. Transfering data as big endian. Handle data as big endian. Do not use properties to retrieve values. 1 byte is used used to marshall the length of data.
        /// </summary>
        /// <param name="p_o_object">object parameter</param>
        /// <returns>byte array of marshalled object</returns>
        /// <exception cref="ArgumentNullException">parameter object is null</exception>
        /// <exception cref="ArgumentException">data length in bytes must be between 1..4</exception>
        /// <exception cref="NotSupportedException">little endian system data is NOT IMPLEMENTED</exception>
        /// <exception cref="MissingMemberException">could not retrieve member type by object member</exception>
        /// <exception cref="System.Reflection.TargetException">could not retrieve value from member instancec</exception>
        /// <exception cref="MemberAccessException">could not access value from meber, access violation</exception>
        public static byte[] MarshallObject(Object? p_o_object)
        {
            return Marshall.MarshallObject(p_o_object, 1);
        }

        /// <summary>
        /// Marshall object with all fields of primitive types or supported types. Transfering data as big endian. Handle data as big endian. Do not use properties to retrieve values.
        /// </summary>
        /// <param name="p_o_object">object parameter</param>
        /// <param name="p_i_dataLengthInBytes">define how many bytes are used to marshall the length of data</param>
        /// <returns>byte array of marshalled object</returns>
        /// <exception cref="ArgumentNullException">parameter object is null</exception>
        /// <exception cref="ArgumentException">data length in bytes must be between 1..4</exception>
        /// <exception cref="NotSupportedException">little endian system data is NOT IMPLEMENTED</exception>
        /// <exception cref="MissingMemberException">could not retrieve member type by object member</exception>
        /// <exception cref="System.Reflection.TargetException">could not retrieve value from member instancec</exception>
        /// <exception cref="MemberAccessException">could not access value from meber, access violation</exception>
        public static byte[] MarshallObject(Object? p_o_object, int p_i_dataLengthInBytes)
        {
            return Marshall.MarshallObject(p_o_object, p_i_dataLengthInBytes, false);
        }

        /// <summary>
        /// Marshall object with all fields of primitive types or supported types. Transfering data as big endian. Handle data as big endian.
        /// </summary>
        /// <param name="p_o_object">object parameter</param>
        /// <param name="p_i_dataLengthInBytes">define how many bytes are used to marshall the length of data</param>
        /// <param name="p_b_useProperties">access object values via properties</param>
        /// <returns>byte array of marshalled object</returns>
        /// <exception cref="ArgumentNullException">parameter object is null</exception>
        /// <exception cref="ArgumentException">data length in bytes must be between 1..4</exception>
        /// <exception cref="NotSupportedException">little endian system data is NOT IMPLEMENTED</exception>
        /// <exception cref="MissingMemberException">could not retrieve member type by object member</exception>
        /// <exception cref="System.Reflection.TargetException">could not retrieve value from member instancec</exception>
        /// <exception cref="MemberAccessException">could not access value from meber, access violation</exception>
        public static byte[] MarshallObject(Object? p_o_object, int p_i_dataLengthInBytes, bool p_b_useProperties)
        {
            return Marshall.MarshallObject(p_o_object, p_i_dataLengthInBytes, p_b_useProperties, false);
        }

        /// <summary>
        /// Marshall object with all fields of primitive types or supported types. Transfering data as big endian.
        /// </summary>
        /// <param name="p_o_object">object parameter</param>
        /// <param name="p_i_dataLengthInBytes">define how many bytes are used to marshall the length of data</param>
        /// <param name="p_b_useProperties">access object values via properties</param>
        /// <param name="p_b_systemUsesLittleEndian">(NOT IMPLEMENTED) true - current execution system uses little endian, false - current execution system uses big endian</param>
        /// <returns>byte array of marshalled object</returns>
        /// <exception cref="ArgumentNullException">parameter object is null</exception>
        /// <exception cref="ArgumentException">data length in bytes must be between 1..4, object parameter is of type List or Dictionary</exception>
        /// <exception cref="NotSupportedException">little endian system data is NOT IMPLEMENTED</exception>
        /// <exception cref="MissingMemberException">could not retrieve member type by object member</exception>
        /// <exception cref="System.Reflection.TargetException">could not retrieve value from member instancec</exception>
        /// <exception cref="MemberAccessException">could not access value from meber, access violation</exception>
        public static byte[] MarshallObject(Object? p_o_object, int p_i_dataLengthInBytes, bool p_b_useProperties, bool p_b_systemUsesLittleEndian)
        {
            /* little endian system data is not supported right now */
            if (p_b_systemUsesLittleEndian)
            {
                throw new NotSupportedException("Supporting little endian system data is NOT IMPLEMENTED");
            }

            /* check if object parameter is not null */
            if (p_o_object == null)
            {
                ForestNET.Lib.Global.ILogWarning("Object parameter is null - sending 5 zero bytes");
                /* if object parameter is null, sending 5 zero bytes */
                return new byte[] { 0, 0, 0, 0, 0 };
            }

            /* check object parameter type */
            if ((p_o_object.GetType().IsGenericType) && ((p_o_object.GetType().GetGenericTypeDefinition().IsAssignableTo(typeof(System.Collections.IList))) || (p_o_object.GetType().GetGenericTypeDefinition().IsAssignableTo(typeof(System.Collections.IDictionary)))))
            {
                throw new ArgumentException("Object parameter is of type IList or IDictionary");
            }

            /* data length parameter must be between 1..4 */
            if ((p_i_dataLengthInBytes < 1) || (p_i_dataLengthInBytes > 4))
            {
                throw new ArgumentException("Data length in bytes parameter must be between 1..4");
            }

            /* dynamic byte list of complete marshalled object */
            List<byte> a_marshalledObject = [];
            int i_amountFields = 0;

            if (Marshall.a_allowedTypes.Contains(p_o_object.GetType()))
            { /* handle primitive supported type */
                /* dynamic byte list for field data length and data */
                List<byte> a_dataLengthAndData = [];
                short sh_arrayAmount = 0;
                byte by_type = 255;
                string s_name = "";

                /* marshall primitive type or array of primitve type */
                if (!MarshallDataByMemberInformation(p_o_object.GetType(), null, s_name, ref by_type, p_o_object.GetType().IsArray, false, p_o_object, p_i_dataLengthInBytes, ref a_dataLengthAndData, ref sh_arrayAmount))
                {
                    throw new ArgumentException("array elements for field/property '" + s_name + "' exceeds max. supported amount of elements(65535)");
                }

                /* check if a valid type could be identified */
                if (by_type == 255)
                {
                    /* type is not supported */
                    throw new ArgumentException("Object parameter is not of a supported type '" + p_o_object.GetType() + "'");
                }

                /* check if field name is not longer than 255 characters, otherwise we must skip the field */
                if (s_name.Length > 255)
                {
                    throw new ArgumentException("name '" + s_name + "' is longer than 255 characters, which is not supported");
                }

                /* add name length to dynamic byte list */
                a_marshalledObject.Add((byte)0);

                /* add type with endian info, array flag, and data length bits (2) */
                long l_typeWithAdditionalInfo = 0;
                l_typeWithAdditionalInfo |= (long)by_type;

                /* set information for little endian */
                if (p_b_systemUsesLittleEndian)
                {
                    l_typeWithAdditionalInfo |= 0x8000;
                }
                else
                {
                    l_typeWithAdditionalInfo &= ~(0x8000);
                }

                /* set information for array flag */
                if (p_o_object.GetType().IsArray)
                {
                    l_typeWithAdditionalInfo |= 0x4000;
                }
                else
                {
                    l_typeWithAdditionalInfo &= ~(0x4000);
                }

                /* set information for data length */
                if (p_i_dataLengthInBytes == 1)
                {
                    l_typeWithAdditionalInfo &= ~(0x3000);
                }
                else if (p_i_dataLengthInBytes == 2)
                {
                    l_typeWithAdditionalInfo |= 0x1000;
                }
                else if (p_i_dataLengthInBytes == 3)
                {
                    l_typeWithAdditionalInfo |= 0x2000;
                }
                else if (p_i_dataLengthInBytes == 4)
                {
                    l_typeWithAdditionalInfo |= 0x3000;
                }

                /* create byte array out of marshalled information with the length of '2' */
                byte[] a_info = ForestNET.Lib.Helper.AmountToNByteArray(l_typeWithAdditionalInfo, 2) ?? [];
                ForestNET.Lib.Global.ILogFiner("marshalled information bytes(2): " + ForestNET.Lib.Helper.PrintByteArray(a_info, false));

                /* add marshalled information to dynamic byte list */
                ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(a_info, ref a_marshalledObject);

                /* create byte array out of marshalled array amount with the length of '2' */
                byte[] a_arrayAmount = ForestNET.Lib.Helper.AmountToNByteArray(sh_arrayAmount, 2) ?? [];
                ForestNET.Lib.Global.ILogFiner("marshalled array amount(2): " + ForestNET.Lib.Helper.PrintByteArray(a_arrayAmount, false));

                /* add marshalled array amount to dynamic byte list */
                ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(a_arrayAmount, ref a_marshalledObject);

                if (a_dataLengthAndData.Count > 0)
                {
                    /* create byte array out of data length and data */
                    byte[] a_bytesDataLengthAndData = new byte[a_dataLengthAndData.Count];

                    for (int i = 0; i < a_dataLengthAndData.Count; i++)
                    {
                        a_bytesDataLengthAndData[i] = a_dataLengthAndData[i];
                    }

                    ForestNET.Lib.Global.ILogFiner("marshalled data length and data: " + ForestNET.Lib.Helper.PrintByteArray(a_bytesDataLengthAndData, false));

                    /* add marshalled data length and data to dynamic byte list */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(a_bytesDataLengthAndData, ref a_marshalledObject);
                }

                string s_empty = "                              ";
                string s_type = p_o_object.GetType().FullName ?? "no type";

                if (s_type.Length > s_empty.Length)
                {
                    s_type = s_type.Substring(0, s_empty.Length);
                }

                ForestNET.Lib.Global.ILogFiner(
                    "marshalled field/property: " +
                    s_empty.Substring(0, s_empty.Length - 18) +
                    s_name +
                    s_empty.Substring(0, s_empty.Length - s_name.Length) +
                    "" +
                    s_empty.Substring(0, s_empty.Length - 0) +
                    s_type +
                    s_empty.Substring(0, s_empty.Length - s_type.Length) +
                    "" +
                    s_empty.Substring(0, s_empty.Length - 0)
                );
            }
            else
            { /* handle object class */
                /* iterate all fields or properties of parameter object */
                foreach (System.Reflection.MemberInfo o_memberInfo in p_o_object.GetType().GetMembers())
                {
                    /* skip empty member info instance */
                    if (o_memberInfo == null)
                    {
                        continue;
                    }

                    /* skip member if it is not a field and we only want to iterate fields */
                    if ((!p_b_useProperties) && (o_memberInfo.MemberType != System.Reflection.MemberTypes.Field))
                    {
                        continue;
                    }

                    /* skip member if it is not a property and we only want to iterate properties */
                    if ((p_b_useProperties) && (o_memberInfo.MemberType != System.Reflection.MemberTypes.Property))
                    {
                        continue;
                    }

                    /* gather all field/property information */
                    string s_name = o_memberInfo.Name;
                    string s_isPublic = "Public: " + ((p_b_useProperties) ? ((System.Reflection.PropertyInfo)o_memberInfo).CanRead : ((System.Reflection.FieldInfo)o_memberInfo).IsPublic);
                    Type o_type = ((p_b_useProperties) ? ((System.Reflection.PropertyInfo)o_memberInfo).PropertyType : ((System.Reflection.FieldInfo)o_memberInfo).FieldType);
                    Type? o_genericType = null;
                    short sh_arrayAmount = 0;
                    byte by_type = 255;

                    ForestNET.Lib.Global.ILogFiner("iterate all " + ((p_b_useProperties) ? "properties" : "fields") + " - name '" + o_memberInfo.Name + "' with type '" + o_type.FullName + "'");

                    /* check if field/property is a list or dictionary */
                    if ((o_type.IsGenericType) && ((o_type.GetGenericTypeDefinition().IsAssignableTo(typeof(System.Collections.IList))) || (o_type.GetGenericTypeDefinition().IsAssignableTo(typeof(System.Collections.IDictionary)))))
                    {
                        /* generic dictionary must have just two parameterized type declaration */
                        if (
                            (o_type.GetGenericTypeDefinition().IsAssignableTo(typeof(System.Collections.IDictionary))) &&
                            (o_type.GenericTypeArguments.Length == 2)
                        )
                        {
                            /* parameterized type declaration of key part of dictionary must be 'int' */
                            if (o_type.GenericTypeArguments[0] != typeof(int))
                            {
                                continue;
                            }

                            o_genericType = o_type.GenericTypeArguments[1];
                        }
                        else if ((o_type.GetGenericTypeDefinition().IsAssignableTo(typeof(System.Collections.IList))) && (o_type.GenericTypeArguments.Length == 1))
                        {
                            /* generic list must have just one parameterized type declaration */
                            o_genericType = o_type.GenericTypeArguments[0];
                        }
                        else
                        { /* parameterized type declaration which are not equal '1' is not supported, so we must skip this field */
                            ForestNET.Lib.Global.ILogFiner("parameterized type declaration which are not equal '1' is not supported, so we must skip this field '" + s_name + "'");
                            continue;
                        }
                    }

                    /* check if field name is not longer than 255 characters, otherwise we must skip the field */
                    if (s_name.Length > 255)
                    {
                        ForestNET.Lib.Global.ILogFiner("field name '" + s_name + "' is longer than 255 characters, which is not supported, so we must skip this field");
                        continue;
                    }

                    /* dynamic byte list for field data length and data */
                    List<byte> a_dataLengthAndData = [];

                    /* help variable for accessing object field */
                    Object? o_value = null;

                    /* call field directly to get object data values */
                    try
                    {
                        if (p_b_useProperties)
                        {
                            o_value = ((System.Reflection.PropertyInfo)o_memberInfo).GetValue(p_o_object);
                        }
                        else
                        {
                            o_value = ((System.Reflection.FieldInfo)o_memberInfo).GetValue(p_o_object);
                        }
                    }
                    catch (Exception o_exc)
                    {
                        throw new MemberAccessException("Access violation for field/property[" + o_memberInfo.Name + "], type[" + o_type.FullName + "]: " + o_exc);
                    }

                    /* marshalling data by field type information */
                    try
                    {
                        if (!MarshallDataByMemberInformation(o_type, o_genericType, s_name, ref by_type, o_type.IsArray, o_type.IsGenericType && o_genericType != null, o_value, p_i_dataLengthInBytes, ref a_dataLengthAndData, ref sh_arrayAmount))
                        {
                            /* invalid type of generic list or invalid amount of elements, so we must skipt this field */
                            continue;
                        }
                    }
                    catch (InvalidCastException o_exc)
                    {
                        /* cast on field was not successful, so we must skip this field */
                        ForestNET.Lib.Global.ILogWarning("cast on field '" + s_name + "' was not successful, so we must skip this field: " + o_exc);
                        continue;
                    }

                    if (by_type == 255)
                    {
                        /* type of field is not supported for marshalling, so we must skip this field */
                        ForestNET.Lib.Global.ILogFine("type of field '" + s_name + "' is not supported for marshalling, so we must skip this field");
                        continue;
                    }

                    i_amountFields++;

                    if (i_amountFields > 65535)
                    {
                        /* amount of fields exceeds max. supported value, so we must skip this field */
                        ForestNET.Lib.Global.ILogWarning("amount of fields '" + i_amountFields + "' exceeds max. supported value(65535), so we must skip this field");
                        continue;
                    }

                    /* add name length to dynamic byte list */
                    a_marshalledObject.Add((byte)s_name.Length);

                    /* get string bytes as utf-8 */
                    byte[] by_fooo = System.Text.Encoding.UTF8.GetBytes(s_name);

                    /* add name to dynamic byte list */
                    foreach (byte by_byte in by_fooo)
                    {
                        a_marshalledObject.Add(by_byte);
                    }

                    /* add type with endian info, array flag, and data length bits (2) */
                    long l_typeWithAdditionalInfo = 0;
                    l_typeWithAdditionalInfo |= (long)by_type;

                    /* set information for little endian */
                    if (p_b_systemUsesLittleEndian)
                    {
                        l_typeWithAdditionalInfo |= 0x8000;
                    }
                    else
                    {
                        l_typeWithAdditionalInfo &= ~(0x8000);
                    }

                    /* set information for array flag */
                    if ((o_type.IsArray) || (o_type.IsGenericType && o_genericType != null))
                    {
                        l_typeWithAdditionalInfo |= 0x4000;
                    }
                    else
                    {
                        l_typeWithAdditionalInfo &= ~(0x4000);
                    }

                    /* set information for data length */
                    if (p_i_dataLengthInBytes == 1)
                    {
                        l_typeWithAdditionalInfo &= ~(0x3000);
                    }
                    else if (p_i_dataLengthInBytes == 2)
                    {
                        l_typeWithAdditionalInfo |= 0x1000;
                    }
                    else if (p_i_dataLengthInBytes == 3)
                    {
                        l_typeWithAdditionalInfo |= 0x2000;
                    }
                    else if (p_i_dataLengthInBytes == 4)
                    {
                        l_typeWithAdditionalInfo |= 0x3000;
                    }

                    /* create byte array out of marshalled information with the length of '2' */
                    byte[] a_info = ForestNET.Lib.Helper.AmountToNByteArray(l_typeWithAdditionalInfo, 2) ?? [];
                    ForestNET.Lib.Global.ILogFiner("marshalled information bytes(2): " + ForestNET.Lib.Helper.PrintByteArray(a_info, false));

                    /* add marshalled information to dynamic byte list */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(a_info, ref a_marshalledObject);

                    /* create byte array out of marshalled array amount with the length of '2' */
                    byte[] a_arrayAmount = ForestNET.Lib.Helper.AmountToNByteArray(sh_arrayAmount, 2) ?? [];
                    ForestNET.Lib.Global.ILogFiner("marshalled array amount(2): " + ForestNET.Lib.Helper.PrintByteArray(a_arrayAmount, false));

                    /* add marshalled array amount to dynamic byte list */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(a_arrayAmount, ref a_marshalledObject);

                    if (a_dataLengthAndData.Count > 0)
                    {
                        /* create byte array out of data length and data */
                        byte[] a_bytesDataLengthAndData = new byte[a_dataLengthAndData.Count];

                        for (int i = 0; i < a_dataLengthAndData.Count; i++)
                        {
                            a_bytesDataLengthAndData[i] = a_dataLengthAndData[i];
                        }

                        ForestNET.Lib.Global.ILogFiner("marshalled data length and data: " + ForestNET.Lib.Helper.PrintByteArray(a_bytesDataLengthAndData, false));

                        /* add marshalled data length and data to dynamic byte list */
                        ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(a_bytesDataLengthAndData, ref a_marshalledObject);
                    }

                    string s_empty = "                              ";
                    string s_type = o_type.FullName ?? "no type";
                    string s_genericType = o_genericType?.FullName ?? "no generic type";

                    if (s_type.Length > s_empty.Length)
                    {
                        s_type = s_type.Substring(0, s_empty.Length);
                    }

                    if (s_genericType.Length > s_empty.Length)
                    {
                        s_genericType = s_genericType.Substring(0, s_empty.Length);
                    }

                    ForestNET.Lib.Global.ILogFiner(
                        "marshalled field/property: " +
                        s_empty.Substring(0, s_empty.Length - 18) +
                        s_name +
                        s_empty.Substring(0, s_empty.Length - s_name.Length) +
                        s_isPublic +
                        s_empty.Substring(0, s_empty.Length - s_isPublic.Length) +
                        s_type +
                        s_empty.Substring(0, s_empty.Length - s_type.Length) +
                        s_genericType +
                        s_empty.Substring(0, s_empty.Length - s_genericType.Length)
                    );
                }
            }

            /* create byte array out of amount of marshalled fields with the length of '2' */
            byte[] a_fields = ForestNET.Lib.Helper.AmountToNByteArray(i_amountFields, 2) ?? [];
            ForestNET.Lib.Global.ILogFiner("amount of marshalled fields(2): " + ForestNET.Lib.Helper.PrintByteArray(a_fields, false));

            byte[] a_return = new byte[a_fields.Length + a_marshalledObject.Count];

            /* assume amount of fields to return array */
            a_return[0] = a_fields[0];
            a_return[1] = a_fields[1];

            /* assume marshalled object to return array */
            for (int i = 0; i < a_marshalledObject.Count; i++)
            {
                a_return[i + 2] = a_marshalledObject[i];
            }

            return a_return;
        }

        /// <summary>
        /// Marshalling data by field or object information
        /// </summary>
        /// <param name="p_o_type">expected type of data which should be marshalled</param>
        /// <param name="p_o_genericType">expected type of generic list data which should be marshalled</param>
        /// <param name="p_s_name">name of the current object field or empty string for primitive object only</param>
        /// <param name="p_by_type">returning recognized type as byte value for marshalled data</param>
        /// <param name="p_b_isArray">true - marshalled data is an array, false - marshalled data is not an array</param>
        /// <param name="p_b_isGenericList">true - marshalled data is generic list, false - marshalled data is not a generic list</param>
        /// <param name="p_o_value">value object of data which should be marshalled</param>
        /// <param name="p_i_dataLengthInBytes">define how many bytes are used to marshall the length of data</param>
        /// <param name="p_a_dataLengthAndData">array of bytes where we want to marshall our content</param>
        /// <param name="p_sh_arrayAmount">returning amount of array elements as byte value for marshalled data</param>
        /// <returns>true - marshalling was successful, false - exceeds max. supported amount of array elements or invalid type of generic list or invalid amount of elements</returns>
        private static bool MarshallDataByMemberInformation(Type p_o_type, Type? p_o_genericType, string p_s_name, ref byte p_by_type, bool p_b_isArray, bool p_b_isGenericList, Object? p_o_value, int p_i_dataLengthInBytes, ref List<byte> p_a_dataLengthAndData, ref short p_sh_arrayAmount)
        {
            if ((p_o_type == typeof(bool)) || (p_o_type == typeof(bool[])) || (p_o_genericType == typeof(bool)) || (p_o_genericType == typeof(bool?)))
            {
                p_by_type = 0;

                if ((!p_b_isArray) && (!p_b_isGenericList))
                {
                    bool b_foo = (bool)(p_o_value ?? false);

                    /* add data length */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(1L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                    /* add data */
                    if (b_foo)
                    {
                        p_a_dataLengthAndData.Add((byte)1);
                    }
                    else
                    {
                        p_a_dataLengthAndData.Add((byte)0);
                    }
                }
                else if (p_b_isArray)
                {
                    bool[]? a_foo = (bool[]?)p_o_value;

                    if (a_foo != null)
                    {
                        if (a_foo.Length > 65535)
                        {
                            /* array elements for this field/property exceeds max. supported amount of elements, so we must skip this field/property */
                            ForestNET.Lib.Global.ILogWarning("array elements for field/property '" + p_s_name + "' exceeds max. supported amount of elements(65535), so we must skip this field/property");
                            return false;
                        }

                        p_sh_arrayAmount = (short)a_foo.Length;

                        foreach (bool b_foo in a_foo)
                        {
                            /* add data length */
                            ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(1L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                            /* add data */
                            if (b_foo)
                            {
                                p_a_dataLengthAndData.Add((byte)1);
                            }
                            else
                            {
                                p_a_dataLengthAndData.Add((byte)0);
                            }
                        }
                    }
                }
                else if (p_b_isGenericList)
                {
                    if ((p_sh_arrayAmount = Marshall.IterateGenericListOrDictionary(typeof(bool), p_o_value, ref p_a_dataLengthAndData, p_i_dataLengthInBytes)) < 0)
                    {
                        /* invalid type of generic list or invalid amount of elements, so we must skip this field/property */
                        ForestNET.Lib.Global.ILogWarning("invalid type of generic list or invalid amount of elements for field/property '" + p_s_name + "' - [generic type = '" + p_o_genericType + "'], so we must skip this field/property");
                        return false;
                    }
                }
            }
            else if ((p_o_type == typeof(byte)) || (p_o_type == typeof(byte[])) || (p_o_genericType == typeof(byte)) || (p_o_genericType == typeof(byte?)))
            {
                p_by_type = 1;

                if ((!p_b_isArray) && (!p_b_isGenericList))
                {
                    byte by_foo = (byte)(p_o_value ?? (byte)0);

                    /* add data length */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(1L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                    /* add data */
                    p_a_dataLengthAndData.Add(by_foo);
                }
                else if (p_b_isArray)
                {
                    byte[]? a_foo = (byte[]?)p_o_value;

                    if (a_foo != null)
                    {
                        if (a_foo.Length > 65535)
                        {
                            /* array elements for this field/property exceeds max. supported amount of elements, so we must skip this field/property */
                            ForestNET.Lib.Global.ILogWarning("array elements for field/property '" + p_s_name + "' exceeds max. supported amount of elements(65535), so we must skip this field/property");
                            return false;
                        }

                        p_sh_arrayAmount = (short)a_foo.Length;

                        foreach (byte by_foo in a_foo)
                        {
                            /* add data length */
                            ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(1L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                            /* add data */
                            p_a_dataLengthAndData.Add(by_foo);
                        }
                    }
                }
                else if (p_b_isGenericList)
                {
                    if ((p_sh_arrayAmount = Marshall.IterateGenericListOrDictionary(typeof(byte), p_o_value, ref p_a_dataLengthAndData, p_i_dataLengthInBytes)) < 0)
                    {
                        /* invalid type of generic list or invalid amount of elements, so we must skip this field/property */
                        ForestNET.Lib.Global.ILogWarning("invalid type of generic list or invalid amount of elements for field/property '" + p_s_name + "' - [generic type = '" + p_o_genericType + "'], so we must skip this field/property");
                        return false;
                    }
                }
            }
            else if ((p_o_type == typeof(sbyte)) || (p_o_type == typeof(sbyte[])) || (p_o_genericType == typeof(sbyte)) || (p_o_genericType == typeof(sbyte?)))
            {
                p_by_type = 2;

                if ((!p_b_isArray) && (!p_b_isGenericList))
                {
                    sbyte sby_foo = (sbyte)(p_o_value ?? (sbyte)0);

                    /* add data length */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(1L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                    /* add data */
                    p_a_dataLengthAndData.Add((byte)sby_foo);
                }
                else if (p_b_isArray)
                {
                    sbyte[]? a_foo = (sbyte[]?)p_o_value;

                    if (a_foo != null)
                    {
                        if (a_foo.Length > 65535)
                        {
                            /* array elements for this field/property exceeds max. supported amount of elements, so we must skip this field/property */
                            ForestNET.Lib.Global.ILogWarning("array elements for field/property '" + p_s_name + "' exceeds max. supported amount of elements(65535), so we must skip this field/property");
                            return false;
                        }

                        p_sh_arrayAmount = (short)a_foo.Length;

                        foreach (sbyte sby_foo in a_foo)
                        {
                            /* add data length */
                            ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(1L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                            /* add data */
                            p_a_dataLengthAndData.Add((byte)sby_foo);
                        }
                    }
                }
                else if (p_b_isGenericList)
                {
                    if ((p_sh_arrayAmount = Marshall.IterateGenericListOrDictionary(typeof(sbyte), p_o_value, ref p_a_dataLengthAndData, p_i_dataLengthInBytes)) < 0)
                    {
                        /* invalid type of generic list or invalid amount of elements, so we must skip this field/property */
                        ForestNET.Lib.Global.ILogWarning("invalid type of generic list or invalid amount of elements for field/property '" + p_s_name + "' - [generic type = '" + p_o_genericType + "'], so we must skip this field/property");
                        return false;
                    }
                }
            }
            else if ((p_o_type == typeof(char)) || (p_o_type == typeof(char[])) || (p_o_genericType == typeof(char)) || (p_o_genericType == typeof(char?)))
            {
                p_by_type = 3;

                if ((!p_b_isArray) && (!p_b_isGenericList))
                {
                    char c_foo = (char)(p_o_value ?? (char)0);

                    /* add data length */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(1L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                    /* add data */
                    p_a_dataLengthAndData.Add((byte)c_foo);
                }
                else if (p_b_isArray)
                {
                    char[]? a_foo = (char[]?)p_o_value;

                    if (a_foo == null)
                    {
                        p_sh_arrayAmount = 0;
                    }
                    else
                    {
                        if (a_foo.Length > 65535)
                        {
                            /* array elements for this field/property exceeds max. supported amount of elements, so we must skip this field/property */
                            ForestNET.Lib.Global.ILogWarning("array elements for field/property '" + p_s_name + "' exceeds max. supported amount of elements(65535), so we must skip this field/property");
                            return false;
                        }

                        p_sh_arrayAmount = (short)a_foo.Length;

                        foreach (char c_foo in a_foo)
                        {
                            /* add data length */
                            ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(1L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                            /* add data */
                            p_a_dataLengthAndData.Add((byte)c_foo);
                        }
                    }
                }
                else if (p_b_isGenericList)
                {
                    if ((p_sh_arrayAmount = Marshall.IterateGenericListOrDictionary(typeof(char), p_o_value, ref p_a_dataLengthAndData, p_i_dataLengthInBytes)) < 0)
                    {
                        /* invalid type of generic list or invalid amount of elements, so we must skip this field/property */
                        ForestNET.Lib.Global.ILogWarning("invalid type of generic list or invalid amount of elements for field/property '" + p_s_name + "' - [generic type = '" + p_o_genericType + "'], so we must skip this field/property");
                        return false;
                    }
                }
            }
            else if ((p_o_type == typeof(float)) || (p_o_type == typeof(float[])) || (p_o_genericType == typeof(float)) || (p_o_genericType == typeof(float?)))
            {
                p_by_type = 4;

                if ((!p_b_isArray) && (!p_b_isGenericList))
                {
                    float f_foo = (float)(p_o_value ?? 0.0f);

                    /* add data length */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(4L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                    /* add data */
                    int i_bits = BitConverter.SingleToInt32Bits(f_foo);
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.IntToByteArray(i_bits) ?? [], ref p_a_dataLengthAndData, 4);
                }
                else if (p_b_isArray)
                {
                    float[]? a_foo = (float[]?)p_o_value;

                    if (a_foo != null)
                    {
                        if (a_foo.Length > 65535)
                        {
                            /* array elements for this field/property exceeds max. supported amount of elements, so we must skip this field/property */
                            ForestNET.Lib.Global.ILogWarning("array elements for field/property '" + p_s_name + "' exceeds max. supported amount of elements(65535), so we must skip this field/property");
                            return false;
                        }

                        p_sh_arrayAmount = (short)a_foo.Length;

                        foreach (float f_foo in a_foo)
                        {
                            /* add data length */
                            ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(4L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                            /* add data */
                            int i_bits = BitConverter.SingleToInt32Bits(f_foo);
                            ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.IntToByteArray(i_bits) ?? [], ref p_a_dataLengthAndData, 4);
                        }
                    }
                }
                else if (p_b_isGenericList)
                {
                    if ((p_sh_arrayAmount = Marshall.IterateGenericListOrDictionary(typeof(float), p_o_value, ref p_a_dataLengthAndData, p_i_dataLengthInBytes)) < 0)
                    {
                        /* invalid type of generic list or invalid amount of elements, so we must skip this field/property */
                        ForestNET.Lib.Global.ILogWarning("invalid type of generic list or invalid amount of elements for field/property '" + p_s_name + "' - [generic type = '" + p_o_genericType + "'], so we must skip this field/property");
                        return false;
                    }
                }
            }
            else if ((p_o_type == typeof(double)) || (p_o_type == typeof(double[])) || (p_o_genericType == typeof(double)) || (p_o_genericType == typeof(double?)))
            {
                p_by_type = 5;

                if ((!p_b_isArray) && (!p_b_isGenericList))
                {
                    double d_foo = (double)(p_o_value ?? 0.0d);

                    /* add data length */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(8L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                    /* add data */
                    long l_bits = BitConverter.DoubleToInt64Bits(d_foo);
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.LongToByteArray(l_bits) ?? [], ref p_a_dataLengthAndData, 8);
                }
                else if (p_b_isArray)
                {
                    double[]? a_foo = (double[]?)p_o_value;

                    if (a_foo != null)
                    {
                        if (a_foo.Length > 65535)
                        {
                            /* array elements for this field/property exceeds max. supported amount of elements, so we must skip this field/property */
                            ForestNET.Lib.Global.ILogWarning("array elements for field/property '" + p_s_name + "' exceeds max. supported amount of elements(65535), so we must skip this field/property");
                            return false;
                        }

                        p_sh_arrayAmount = (short)a_foo.Length;

                        foreach (double d_foo in a_foo)
                        {
                            /* add data length */
                            ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(8L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                            /* add data */
                            long l_bits = BitConverter.DoubleToInt64Bits(d_foo);
                            ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.LongToByteArray(l_bits) ?? [], ref p_a_dataLengthAndData, 8);
                        }
                    }
                }
                else if (p_b_isGenericList)
                {
                    if ((p_sh_arrayAmount = Marshall.IterateGenericListOrDictionary(typeof(double), p_o_value, ref p_a_dataLengthAndData, p_i_dataLengthInBytes)) < 0)
                    {
                        /* invalid type of generic list or invalid amount of elements, so we must skip this field/property */
                        ForestNET.Lib.Global.ILogWarning("invalid type of generic list or invalid amount of elements for field/property '" + p_s_name + "' - [generic type = '" + p_o_genericType + "'], so we must skip this field/property");
                        return false;
                    }
                }
            }
            else if ((p_o_type == typeof(short)) || (p_o_type == typeof(short[])) || (p_o_genericType == typeof(short)) || (p_o_genericType == typeof(short?)))
            {
                p_by_type = 6;

                if ((!p_b_isArray) && (!p_b_isGenericList))
                {
                    short sh_foo = (short)(p_o_value ?? (short)0);

                    /* add data length */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(2L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                    /* add data */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.ShortToByteArray(sh_foo) ?? [], ref p_a_dataLengthAndData, 2);
                }
                else if (p_b_isArray)
                {
                    short[]? a_foo = (short[]?)p_o_value;

                    if (a_foo != null)
                    {
                        if (a_foo.Length > 65535)
                        {
                            /* array elements for this field/property exceeds max. supported amount of elements, so we must skip this field/property */
                            ForestNET.Lib.Global.ILogWarning("array elements for field/property '" + p_s_name + "' exceeds max. supported amount of elements(65535), so we must skip this field/property");
                            return false;
                        }

                        p_sh_arrayAmount = (short)a_foo.Length;

                        foreach (short sh_foo in a_foo)
                        {
                            /* add data length */
                            ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(2L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                            /* add data */
                            ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.ShortToByteArray(sh_foo) ?? [], ref p_a_dataLengthAndData, 2);
                        }
                    }
                }
                else if (p_b_isGenericList)
                {
                    if ((p_sh_arrayAmount = Marshall.IterateGenericListOrDictionary(typeof(short), p_o_value, ref p_a_dataLengthAndData, p_i_dataLengthInBytes)) < 0)
                    {
                        /* invalid type of generic list or invalid amount of elements, so we must skip this field/property */
                        ForestNET.Lib.Global.ILogWarning("invalid type of generic list or invalid amount of elements for field/property '" + p_s_name + "' - [generic type = '" + p_o_genericType + "'], so we must skip this field/property");
                        return false;
                    }
                }
            }
            else if ((p_o_type == typeof(ushort)) || (p_o_type == typeof(ushort[])) || (p_o_genericType == typeof(ushort)) || (p_o_genericType == typeof(ushort?)))
            {
                p_by_type = 7;

                if ((!p_b_isArray) && (!p_b_isGenericList))
                {
                    ushort ush_foo = (ushort)(p_o_value ?? (ushort)0);

                    /* add data length */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(2L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                    /* add data */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.ShortToByteArray((short)ush_foo) ?? [], ref p_a_dataLengthAndData, 2);
                }
                else if (p_b_isArray)
                {
                    ushort[]? a_foo = (ushort[]?)p_o_value;

                    if (a_foo != null)
                    {
                        if (a_foo.Length > 65535)
                        {
                            /* array elements for this field/property exceeds max. supported amount of elements, so we must skip this field/property */
                            ForestNET.Lib.Global.ILogWarning("array elements for field/property '" + p_s_name + "' exceeds max. supported amount of elements(65535), so we must skip this field/property");
                            return false;
                        }

                        p_sh_arrayAmount = (short)a_foo.Length;

                        foreach (ushort ush_foo in a_foo)
                        {
                            /* add data length */
                            ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(2L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                            /* add data */
                            ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.ShortToByteArray((short)ush_foo) ?? [], ref p_a_dataLengthAndData, 2);
                        }
                    }
                }
                else if (p_b_isGenericList)
                {
                    if ((p_sh_arrayAmount = Marshall.IterateGenericListOrDictionary(typeof(ushort), p_o_value, ref p_a_dataLengthAndData, p_i_dataLengthInBytes)) < 0)
                    {
                        /* invalid type of generic list or invalid amount of elements, so we must skip this field/property */
                        ForestNET.Lib.Global.ILogWarning("invalid type of generic list or invalid amount of elements for field/property '" + p_s_name + "' - [generic type = '" + p_o_genericType + "'], so we must skip this field/property");
                        return false;
                    }
                }
            }
            else if ((p_o_type == typeof(int)) || (p_o_type == typeof(int[])) || (p_o_genericType == typeof(int)) || (p_o_genericType == typeof(int?)))
            {
                p_by_type = 8;

                if ((!p_b_isArray) && (!p_b_isGenericList))
                {
                    int i_foo = (int)(p_o_value ?? 0);

                    /* add data length */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(4L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                    /* add data */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.IntToByteArray(i_foo) ?? [], ref p_a_dataLengthAndData, 4);
                }
                else if (p_b_isArray)
                {
                    int[]? a_foo = (int[]?)p_o_value;

                    if (a_foo != null)
                    {
                        if (a_foo.Length > 65535)
                        {
                            /* array elements for this field/property exceeds max. supported amount of elements, so we must skip this field/property */
                            ForestNET.Lib.Global.ILogWarning("array elements for field/property '" + p_s_name + "' exceeds max. supported amount of elements(65535), so we must skip this field/property");
                            return false;
                        }

                        p_sh_arrayAmount = (short)a_foo.Length;

                        foreach (int i_foo in a_foo)
                        {
                            /* add data length */
                            ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(4L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                            /* add data */
                            ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.IntToByteArray(i_foo) ?? [], ref p_a_dataLengthAndData, 4);
                        }
                    }
                }
                else if (p_b_isGenericList)
                {
                    if ((p_sh_arrayAmount = Marshall.IterateGenericListOrDictionary(typeof(int), p_o_value, ref p_a_dataLengthAndData, p_i_dataLengthInBytes)) < 0)
                    {
                        /* invalid type of generic list or invalid amount of elements, so we must skip this field/property */
                        ForestNET.Lib.Global.ILogWarning("invalid type of generic list or invalid amount of elements for field/property '" + p_s_name + "' - [generic type = '" + p_o_genericType + "'], so we must skip this field/property");
                        return false;
                    }
                }
            }
            else if ((p_o_type == typeof(uint)) || (p_o_type == typeof(uint[])) || (p_o_genericType == typeof(uint)) || (p_o_genericType == typeof(uint?)))
            {
                p_by_type = 9;

                if ((!p_b_isArray) && (!p_b_isGenericList))
                {
                    uint ui_foo = (uint)(p_o_value ?? 0);

                    /* add data length */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(4L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                    /* add data */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.IntToByteArray((int)ui_foo) ?? [], ref p_a_dataLengthAndData, 4);
                }
                else if (p_b_isArray)
                {
                    uint[]? a_foo = (uint[]?)p_o_value;

                    if (a_foo != null)
                    {
                        if (a_foo.Length > 65535)
                        {
                            /* array elements for this field/property exceeds max. supported amount of elements, so we must skip this field/property */
                            ForestNET.Lib.Global.ILogWarning("array elements for field/property '" + p_s_name + "' exceeds max. supported amount of elements(65535), so we must skip this field/property");
                            return false;
                        }

                        p_sh_arrayAmount = (short)a_foo.Length;

                        foreach (uint ui_foo in a_foo)
                        {
                            /* add data length */
                            ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(4L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                            /* add data */
                            ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.IntToByteArray((int)ui_foo) ?? [], ref p_a_dataLengthAndData, 4);
                        }
                    }
                }
                else if (p_b_isGenericList)
                {
                    if ((p_sh_arrayAmount = Marshall.IterateGenericListOrDictionary(typeof(uint), p_o_value, ref p_a_dataLengthAndData, p_i_dataLengthInBytes)) < 0)
                    {
                        /* invalid type of generic list or invalid amount of elements, so we must skip this field/property */
                        ForestNET.Lib.Global.ILogWarning("invalid type of generic list or invalid amount of elements for field/property '" + p_s_name + "' - [generic type = '" + p_o_genericType + "'], so we must skip this field/property");
                        return false;
                    }
                }
            }
            else if ((p_o_type == typeof(long)) || (p_o_type == typeof(long[])) || (p_o_genericType == typeof(long)) || (p_o_genericType == typeof(long?)))
            {
                p_by_type = 10;

                if ((!p_b_isArray) && (!p_b_isGenericList))
                {
                    long l_foo = (long)(p_o_value ?? 0L);

                    /* add data length */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(8L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                    /* add data */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.LongToByteArray(l_foo) ?? [], ref p_a_dataLengthAndData, 8);
                }
                else if (p_b_isArray)
                {
                    long[]? a_foo = (long[]?)p_o_value;

                    if (a_foo != null)
                    {
                        if (a_foo.Length > 65535)
                        {
                            /* array elements for this field/property exceeds max. supported amount of elements, so we must skip this field/property */
                            ForestNET.Lib.Global.ILogWarning("array elements for field/property '" + p_s_name + "' exceeds max. supported amount of elements(65535), so we must skip this field/property");
                            return false;
                        }

                        p_sh_arrayAmount = (short)a_foo.Length;

                        foreach (long l_foo in a_foo)
                        {
                            /* add data length */
                            ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(8L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                            /* add data */
                            ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.LongToByteArray(l_foo) ?? [], ref p_a_dataLengthAndData, 8);
                        }
                    }
                }
                else if (p_b_isGenericList)
                {
                    if ((p_sh_arrayAmount = Marshall.IterateGenericListOrDictionary(typeof(long), p_o_value, ref p_a_dataLengthAndData, p_i_dataLengthInBytes)) < 0)
                    {
                        /* invalid type of generic list or invalid amount of elements, so we must skip this field/property */
                        ForestNET.Lib.Global.ILogWarning("invalid type of generic list or invalid amount of elements for field/property '" + p_s_name + "' - [generic type = '" + p_o_genericType + "'], so we must skip this field/property");
                        return false;
                    }
                }
            }
            else if ((p_o_type == typeof(ulong)) || (p_o_type == typeof(ulong[])) || (p_o_genericType == typeof(ulong)) || (p_o_genericType == typeof(ulong?)))
            {
                p_by_type = 11;

                if ((!p_b_isArray) && (!p_b_isGenericList))
                {
                    ulong ul_foo = (ulong)(p_o_value ?? 0L);

                    /* add data length */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(8L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                    /* add data */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.LongToByteArray((long)ul_foo) ?? [], ref p_a_dataLengthAndData, 8);
                }
                else if (p_b_isArray)
                {
                    ulong[]? a_foo = (ulong[]?)p_o_value;

                    if (a_foo != null)
                    {
                        if (a_foo.Length > 65535)
                        {
                            /* array elements for this field/property exceeds max. supported amount of elements, so we must skip this field/property */
                            ForestNET.Lib.Global.ILogWarning("array elements for field/property '" + p_s_name + "' exceeds max. supported amount of elements(65535), so we must skip this field/property");
                            return false;
                        }

                        p_sh_arrayAmount = (short)a_foo.Length;

                        foreach (ulong ul_foo in a_foo)
                        {
                            /* add data length */
                            ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(8L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                            /* add data */
                            ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.LongToByteArray((long)ul_foo) ?? [], ref p_a_dataLengthAndData, 8);
                        }
                    }
                }
                else if (p_b_isGenericList)
                {
                    if ((p_sh_arrayAmount = Marshall.IterateGenericListOrDictionary(typeof(ulong), p_o_value, ref p_a_dataLengthAndData, p_i_dataLengthInBytes)) < 0)
                    {
                        /* invalid type of generic list or invalid amount of elements, so we must skip this field/property */
                        ForestNET.Lib.Global.ILogWarning("invalid type of generic list or invalid amount of elements for field/property '" + p_s_name + "' - [generic type = '" + p_o_genericType + "'], so we must skip this field/property");
                        return false;
                    }
                }
            }
            else if ((p_o_type == typeof(string)) || (p_o_type == typeof(string[])) || (p_o_type == typeof(string?[])) || (p_o_genericType == typeof(string)))
            {
                p_by_type = 12;

                if ((!p_b_isArray) && (!p_b_isGenericList))
                {
                    string? s_foo = (string?)p_o_value;

                    if (s_foo != null)
                    {
                        /* get string bytes as utf-8 */
                        byte[] by_foo = System.Text.Encoding.UTF8.GetBytes(s_foo);

                        /* add data length */
                        ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(by_foo.Length, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                        /* add data */
                        foreach (byte by_byte in by_foo)
                        {
                            p_a_dataLengthAndData.Add(by_byte);
                        }
                    }
                    else
                    {
                        /* add data length '0' */
                        ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(0, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);
                    }
                }
                else if (p_b_isArray)
                {
                    string?[]? a_foo = (string?[]?)p_o_value;

                    if (a_foo != null)
                    {
                        if (a_foo.Length > 65535)
                        {
                            /* array elements for this field/property exceeds max. supported amount of elements, so we must skip this field/property */
                            ForestNET.Lib.Global.ILogWarning("array elements for field/property '" + p_s_name + "' exceeds max. supported amount of elements(65535), so we must skip this field/property");
                            return false;
                        }

                        p_sh_arrayAmount = (short)a_foo.Length;

                        foreach (string? s_foo in a_foo)
                        {
                            if (s_foo != null)
                            {
                                /* get string bytes as utf-8 */
                                byte[] by_foo = System.Text.Encoding.UTF8.GetBytes(s_foo);

                                /* add data length */
                                ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(by_foo.Length, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                                /* add data */
                                foreach (byte by_byte in by_foo)
                                {
                                    p_a_dataLengthAndData.Add(by_byte);
                                }
                            }
                            else
                            {
                                /* add data length '0' */
                                ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(0, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);
                            }
                        }
                    }
                }
                else if (p_b_isGenericList)
                {
                    if ((p_sh_arrayAmount = Marshall.IterateGenericListOrDictionary(typeof(string), p_o_value, ref p_a_dataLengthAndData, p_i_dataLengthInBytes)) < 0)
                    {
                        /* invalid type of generic list or invalid amount of elements, so we must skip this field/property */
                        ForestNET.Lib.Global.ILogWarning("invalid type of generic list or invalid amount of elements for field/property '" + p_s_name + "' - [generic type = '" + p_o_genericType + "'], so we must skip this field/property");
                        return false;
                    }
                }
            }
            else if ((p_o_type == typeof(DateTime)) || (p_o_type == typeof(DateTime[])) || (p_o_genericType == typeof(DateTime)))
            {
                p_by_type = 13;

                if ((!p_b_isArray) && (!p_b_isGenericList))
                {
                    System.DateTime o_foo = (System.DateTime)(p_o_value ?? DateTime.MinValue);

                    string s_foo = ForestNET.Lib.Helper.ToISO8601UTC(o_foo);

                    /* get string bytes as utf-8 */
                    byte[] by_foo = System.Text.Encoding.UTF8.GetBytes(s_foo);

                    /* add data length */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(by_foo.Length, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                    /* add data */
                    foreach (byte by_byte in by_foo)
                    {
                        p_a_dataLengthAndData.Add(by_byte);
                    }
                }
                else if (p_b_isArray)
                {
                    System.DateTime[]? a_foo = (System.DateTime[]?)p_o_value;

                    if (a_foo != null)
                    {
                        if (a_foo.Length > 65535)
                        {
                            /* array elements for this field/property exceeds max. supported amount of elements, so we must skip this field/property */
                            ForestNET.Lib.Global.ILogWarning("array elements for field/property '" + p_s_name + "' exceeds max. supported amount of elements(65535), so we must skip this field/property");
                            return false;
                        }

                        p_sh_arrayAmount = (short)a_foo.Length;

                        for (int i = 0; i < a_foo.Length; i++)
                        {
                            System.DateTime o_foo = a_foo[i];

                            string s_foo = ForestNET.Lib.Helper.ToISO8601UTC(o_foo);

                            /* get string bytes as utf-8 */
                            byte[] by_foo = System.Text.Encoding.UTF8.GetBytes(s_foo);

                            /* add data length */
                            ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(by_foo.Length, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                            /* add data */
                            foreach (byte by_byte in by_foo)
                            {
                                p_a_dataLengthAndData.Add(by_byte);
                            }
                        }
                    }
                }
                else if (p_b_isGenericList)
                {
                    if ((p_sh_arrayAmount = Marshall.IterateGenericListOrDictionary(typeof(DateTime), p_o_value, ref p_a_dataLengthAndData, p_i_dataLengthInBytes)) < 0)
                    {
                        /* invalid type of generic list or invalid amount of elements, so we must skip this field/property */
                        ForestNET.Lib.Global.ILogWarning("invalid type of generic list or invalid amount of elements for field/property '" + p_s_name + "' - [generic type = '" + p_o_genericType + "'], so we must skip this field/property");
                        return false;
                    }
                }
            }
            else if ((p_o_type == typeof(DateTime?)) || (p_o_type == typeof(DateTime?[])) || (p_o_genericType == typeof(DateTime?)))
            {
                p_by_type = 13;

                if ((!p_b_isArray) && (!p_b_isGenericList))
                {
                    System.DateTime? o_foo = (System.DateTime?)p_o_value;

                    if (o_foo != null)
                    {
                        string s_foo = ForestNET.Lib.Helper.ToISO8601UTC(o_foo ?? DateTime.MinValue);

                        /* get string bytes as utf-8 */
                        byte[] by_foo = System.Text.Encoding.UTF8.GetBytes(s_foo);

                        /* add data length */
                        ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(by_foo.Length, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                        /* add data */
                        foreach (byte by_byte in by_foo)
                        {
                            p_a_dataLengthAndData.Add(by_byte);
                        }
                    }
                    else
                    {
                        /* add data length '0' */
                        ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(0, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);
                    }
                }
                else if (p_b_isArray)
                {
                    System.DateTime?[]? a_foo = (System.DateTime?[]?)p_o_value;

                    if (a_foo != null)
                    {
                        if (a_foo.Length > 65535)
                        {
                            /* array elements for this field/property exceeds max. supported amount of elements, so we must skip this field/property */
                            ForestNET.Lib.Global.ILogWarning("array elements for field/property '" + p_s_name + "' exceeds max. supported amount of elements(65535), so we must skip this field/property");
                            return false;
                        }

                        p_sh_arrayAmount = (short)a_foo.Length;

                        for (int i = 0; i < a_foo.Length; i++)
                        {
                            System.DateTime? o_foo = a_foo[i];

                            if (o_foo != null)
                            {
                                string s_foo = ForestNET.Lib.Helper.ToISO8601UTC(o_foo ?? DateTime.MinValue);

                                /* get string bytes as utf-8 */
                                byte[] by_foo = System.Text.Encoding.UTF8.GetBytes(s_foo);

                                /* add data length */
                                ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(by_foo.Length, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                                /* add data */
                                foreach (byte by_byte in by_foo)
                                {
                                    p_a_dataLengthAndData.Add(by_byte);
                                }
                            }
                            else
                            {
                                /* add data length '0' */
                                ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(0, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);
                            }
                        }
                    }
                }
                else if (p_b_isGenericList)
                {
                    if ((p_sh_arrayAmount = Marshall.IterateGenericListOrDictionary(typeof(DateTime?), p_o_value, ref p_a_dataLengthAndData, p_i_dataLengthInBytes)) < 0)
                    {
                        /* invalid type of generic list or invalid amount of elements, so we must skip this field/property */
                        ForestNET.Lib.Global.ILogWarning("invalid type of generic list or invalid amount of elements for field/property '" + p_s_name + "' - [generic type = '" + p_o_genericType + "'], so we must skip this field/property");
                        return false;
                    }
                }
            }
            else if ((p_o_type == typeof(decimal)) || (p_o_type == typeof(decimal[])) || (p_o_genericType == typeof(decimal)) || (p_o_genericType == typeof(decimal?)))
            {
                p_by_type = 14;

                if ((!p_b_isArray) && (!p_b_isGenericList))
                {
                    decimal o_foo = (decimal)(p_o_value ?? 0.0m);

                    string s_foo = ForestNET.Lib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimalWithSign(o_foo, 13, 20, null, null);

                    /* get string bytes as utf-8 */
                    byte[] by_foo = System.Text.Encoding.UTF8.GetBytes(s_foo);

                    /* add data length */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(by_foo.Length, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                    /* add data */
                    foreach (byte by_byte in by_foo)
                    {
                        p_a_dataLengthAndData.Add(by_byte);
                    }
                }
                else if (p_b_isArray)
                {
                    decimal[]? a_foo = (decimal[]?)p_o_value;

                    if (a_foo != null)
                    {
                        if (a_foo.Length > 65535)
                        {
                            /* array elements for this field/property exceeds max. supported amount of elements, so we must skip this field/property */
                            ForestNET.Lib.Global.ILogWarning("array elements for field/property '" + p_s_name + "' exceeds max. supported amount of elements(65535), so we must skip this field/property");
                            return false;
                        }

                        p_sh_arrayAmount = (short)a_foo.Length;

                        for (int i = 0; i < a_foo.Length; i++)
                        {
                            decimal o_foo = a_foo[i];

                            string s_foo = ForestNET.Lib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimalWithSign(o_foo, 13, 20, null, null);

                            /* get string bytes as utf-8 */
                            byte[] by_foo = System.Text.Encoding.UTF8.GetBytes(s_foo);

                            /* add data length */
                            ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(by_foo.Length, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                            /* add data */
                            foreach (byte by_byte in by_foo)
                            {
                                p_a_dataLengthAndData.Add(by_byte);
                            }
                        }
                    }
                }
                else if (p_b_isGenericList)
                {
                    if ((p_sh_arrayAmount = Marshall.IterateGenericListOrDictionary(typeof(decimal), p_o_value, ref p_a_dataLengthAndData, p_i_dataLengthInBytes)) < 0)
                    {
                        /* invalid type of generic list or invalid amount of elements, so we must skip this field/property */
                        ForestNET.Lib.Global.ILogWarning("invalid type of generic list or invalid amount of elements for field/property '" + p_s_name + "' - [generic type = '" + p_o_genericType + "'], so we must skip this field/property");
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Handle generic list after retrieving field/property value from an object
        /// </summary>
        /// <param name="p_o_genericClassType">tells us the parameterized type of the generic list</param>
        /// <param name="p_o_value">object value where we have our generic list, only Collection, Set and Map are supported</param>
        /// <param name="p_a_dataLengthAndData">array of bytes where we want to marshall our generic list's content</param>
        /// <param name="p_i_dataLengthInBytes">define how many bytes are used to marshall the length of data</param>
        /// <returns>amount of elements in generic list or -1 if class type is not supported, object value is not a Collection, Set or Map, amount of elements exceeds max. supported value (65535)</returns>
        private static short IterateGenericListOrDictionary(Type p_o_genericClassType, Object? p_o_value, ref List<byte> p_a_dataLengthAndData, int p_i_dataLengthInBytes)
        {
            short sh_return;

            /* check field/property value parameter */
            if (p_o_value == null)
            {
                /* no generic list available -> null */
                return 0;
            }

            if (p_o_value.GetType().GetGenericTypeDefinition().IsAssignableTo(typeof(System.Collections.IList)))
            { /* we have a list for iteration */
                /* cast field/property value to generic list with object? type */
                System.Collections.IList a_tempList = (System.Collections.IList)p_o_value;
                List<Object?> a_foo = [];

                foreach (Object? o_value in a_tempList)
                {
                    a_foo.Add(o_value);
                }

                if (a_foo.Count > 65535)
                {
                    /* array elements for this field/property with generic list exceeds max. supported amount of elements, so we must skip this field/property */
                    ForestNET.Lib.Global.ILogWarning("array elements for field/property with generic list exceeds max. supported amount of elements(65535), so we must skip this field/property");
                    return -1;
                }

                sh_return = (short)a_foo.Count;

                /* iterate each collection object */
                foreach (Object? o_foo in a_foo)
                {
                    if (!Marshall.MarshallElementOfGenericListOrDictionary(p_o_genericClassType, o_foo, ref p_a_dataLengthAndData, p_i_dataLengthInBytes))
                    {
                        /* class type not supported for marshalling */
                        return -1;
                    }
                }
            }
            else if (p_o_value.GetType().GetGenericTypeDefinition().IsAssignableTo(typeof(System.Collections.IDictionary)))
            { /* we have a dictionary for iteration */
                /* cast field/property value to generic dictionary with integer as key and object? as value type */
                System.Collections.IDictionary m_tempDictionary = (System.Collections.IDictionary)p_o_value;
                Dictionary<int, Object?> a_foo = [];

                foreach (System.Collections.DictionaryEntry o_keyAndValue in m_tempDictionary)
                {
                    /* check if key is an integer */
                    if (!ForestNET.Lib.Helper.IsInteger(o_keyAndValue.Key.ToString() ?? "null"))
                    {
                        ForestNET.Lib.Global.ILogWarning("could not cast field/property to dictionary. key '" + (o_keyAndValue.Key.ToString() ?? "null") + "' is not an integer");
                        return -1;
                    }

                    a_foo.Add(Convert.ToInt32(o_keyAndValue.Key), o_keyAndValue.Value);
                }

                if (a_foo.Count > 65535)
                {
                    /* array elements for this field/property with generic list exceeds max. supported amount of elements, so we must skip this field/property */
                    ForestNET.Lib.Global.ILogWarning("array elements for field/property with generic list exceeds max. supported amount of elements(65535), so we must skip this field/property");
                    return -1;
                }

                sh_return = (short)a_foo.Count;

                /* iterate each dictionary object, ordered dictionary by key before */
                foreach (KeyValuePair<int, Object?> kv_foo in a_foo.OrderBy(kv => kv.Key))
                {
                    if (!Marshall.MarshallElementOfGenericListOrDictionary(p_o_genericClassType, kv_foo.Value, ref p_a_dataLengthAndData, p_i_dataLengthInBytes))
                    {
                        /* class type not supported for marshalling */
                        return -1;
                    }
                }
            }
            else
            {
                /* field/property with generic list is not of type Set, Collection or Map, so we must skip this field/property */
                ForestNET.Lib.Global.ILogFiner("field/property with generic list is not of type Set, Collection or Map, so we must skip this field/property");

                return -1;
            }

            return sh_return;
        }

        /// <summary>
        /// Handle element of generic list or map and marshall data content
        /// </summary>
        /// <param name="p_o_genericClassType">tells us the parameterized type of the generic list</param>
        /// <param name="p_o_foo">element of generic list or map</param>
        /// <param name="p_a_dataLengthAndData">array of bytes where we want to marshall our generic list's content</param>
        /// <param name="p_i_dataLengthInBytes">define how many bytes are used to marshall the length of data</param>
        /// <returns>true - element could be marshalled, false - class type of generic list or map not supported for marshalling</returns>
        private static bool MarshallElementOfGenericListOrDictionary(Type p_o_genericClassType, Object? p_o_foo, ref List<byte> p_a_dataLengthAndData, int p_i_dataLengthInBytes)
        {
            if (p_o_genericClassType == typeof(bool))
            {
                if (p_o_foo == null)
                {
                    /* add data length '0' */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(0L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);
                }
                else
                {
                    /* add data length */

                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(1L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                    /* add data */
                    if ((bool)p_o_foo)
                    {
                        p_a_dataLengthAndData.Add((byte)1);
                    }
                    else
                    {
                        p_a_dataLengthAndData.Add((byte)0);
                    }
                }
            }
            else if (p_o_genericClassType == typeof(byte))
            {
                if (p_o_foo == null)
                {
                    /* add data length '0' */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(0L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);
                }
                else
                {
                    /* add data length */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(1L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                    /* add data */
                    p_a_dataLengthAndData.Add((byte)p_o_foo);
                }
            }
            else if (p_o_genericClassType == typeof(sbyte))
            {
                if (p_o_foo == null)
                {
                    /* add data length '0' */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(0L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);
                }
                else
                {
                    /* add data length */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(1L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                    /* add data */
                    p_a_dataLengthAndData.Add((byte)(0xFF & (sbyte)p_o_foo));
                }
            }
            else if (p_o_genericClassType == typeof(char))
            {
                if (p_o_foo == null)
                {
                    /* add data length '0' */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(0L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);
                }
                else
                {
                    /* add data length */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(1L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                    /* add data */
                    p_a_dataLengthAndData.Add((byte)(0xFF & (char)p_o_foo));
                }
            }
            else if (p_o_genericClassType == typeof(float))
            {
                if (p_o_foo == null)
                {
                    /* add data length '0' */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(0L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);
                }
                else
                {
                    /* add data length */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(4L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                    /* add data */
                    int i_bits = BitConverter.SingleToInt32Bits((float)p_o_foo);
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.IntToByteArray(i_bits) ?? [], ref p_a_dataLengthAndData, 4);
                }
            }
            else if (p_o_genericClassType == typeof(double))
            {
                if (p_o_foo == null)
                {
                    /* add data length '0' */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(0L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);
                }
                else
                {
                    /* add data length */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(8L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                    /* add data */
                    long l_bits = BitConverter.DoubleToInt64Bits((double)p_o_foo);
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.LongToByteArray(l_bits) ?? [], ref p_a_dataLengthAndData, 8);
                }
            }
            else if (p_o_genericClassType == typeof(short))
            {
                if (p_o_foo == null)
                {
                    /* add data length '0' */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(0L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);
                }
                else
                {
                    /* add data length */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(2L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                    /* add data */
                    short sh_foo = (short)p_o_foo;
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.ShortToByteArray(sh_foo) ?? [], ref p_a_dataLengthAndData, 2);
                }
            }
            else if (p_o_genericClassType == typeof(ushort))
            {
                if (p_o_foo == null)
                {
                    /* add data length '0' */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(0L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);
                }
                else
                {
                    /* add data length */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(2L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                    /* add data */
                    ushort ush_foo = (ushort)p_o_foo;
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.ShortToByteArray((short)ush_foo) ?? [], ref p_a_dataLengthAndData, 2);
                }
            }
            else if (p_o_genericClassType == typeof(int))
            {
                if (p_o_foo == null)
                {
                    /* add data length '0' */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(0L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);
                }
                else
                {
                    /* add data length */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(4L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                    /* add data */
                    int i_foo = (int)p_o_foo;
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.IntToByteArray(i_foo) ?? [], ref p_a_dataLengthAndData, 4);
                }
            }
            else if (p_o_genericClassType == typeof(uint))
            {
                if (p_o_foo == null)
                {
                    /* add data length '0' */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(0L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);
                }
                else
                {
                    /* add data length */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(4L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                    /* add data */
                    uint ui_foo = (uint)p_o_foo;
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.IntToByteArray((int)ui_foo) ?? [], ref p_a_dataLengthAndData, 4);
                }
            }
            else if (p_o_genericClassType == typeof(long))
            {
                if (p_o_foo == null)
                {
                    /* add data length '0' */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(0L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);
                }
                else
                {
                    /* add data length */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(8L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                    /* add data */
                    long l_foo = (long)p_o_foo;
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.LongToByteArray(l_foo) ?? [], ref p_a_dataLengthAndData, 8);
                }
            }
            else if (p_o_genericClassType == typeof(ulong))
            {
                if (p_o_foo == null)
                {
                    /* add data length '0' */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(0L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);
                }
                else
                {
                    /* add data length */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(8L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                    /* add data */
                    ulong ul_foo = (ulong)p_o_foo;
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.LongToByteArray((long)ul_foo) ?? [], ref p_a_dataLengthAndData, 8);
                }
            }
            else if (p_o_genericClassType == typeof(string))
            {
                string? s_foo = (string?)p_o_foo;

                if (s_foo != null)
                {
                    /* get string bytes as utf-8 */
                    byte[] by_foo = System.Text.Encoding.UTF8.GetBytes(s_foo);

                    /* add data length */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(by_foo.Length, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                    /* add data */
                    foreach (byte by_byte in by_foo)
                    {
                        p_a_dataLengthAndData.Add(by_byte);
                    }
                }
                else
                {
                    /* add data length '0' */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(0L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);
                }
            }
            else if (p_o_genericClassType == typeof(DateTime))
            {
                System.DateTime o_bar = (System.DateTime)(p_o_foo ?? DateTime.MinValue);

                string s_foo = ForestNET.Lib.Helper.ToISO8601UTC(o_bar);

                /* get string bytes as utf-8 */
                byte[] by_foo = System.Text.Encoding.UTF8.GetBytes(s_foo);

                /* add data length */
                ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(by_foo.Length, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                /* add data */
                foreach (byte by_byte in by_foo)
                {
                    p_a_dataLengthAndData.Add(by_byte);
                }
            }
            else if (p_o_genericClassType == typeof(DateTime?))
            {
                System.DateTime? o_bar = (System.DateTime?)p_o_foo;

                if (p_o_foo != null)
                {
                    string s_foo = ForestNET.Lib.Helper.ToISO8601UTC(o_bar ?? System.DateTime.MinValue);

                    /* get string bytes as utf-8 */
                    byte[] by_foo = System.Text.Encoding.UTF8.GetBytes(s_foo);

                    /* add data length */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(by_foo.Length, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                    /* add data */
                    foreach (byte by_byte in by_foo)
                    {
                        p_a_dataLengthAndData.Add(by_byte);
                    }
                }
                else
                {
                    /* add data length '0' */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(0L, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);
                }
            }
            else if (p_o_genericClassType == typeof(decimal))
            {
                decimal o_bar = (decimal)(p_o_foo ?? 0.0m);

                string s_foo = ForestNET.Lib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimalWithSign(o_bar, 13, 20, null, null);

                /* get string bytes as utf-8 */
                byte[] by_foo = System.Text.Encoding.UTF8.GetBytes(s_foo);

                /* add data length */
                ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.Helper.AmountToNByteArray(by_foo.Length, p_i_dataLengthInBytes) ?? [], ref p_a_dataLengthAndData);

                /* add data */
                foreach (byte by_byte in by_foo)
                {
                    p_a_dataLengthAndData.Add(by_byte);
                }
            }
            else
            {
                /* class type not supported for marshalling, so we will skip this field/property */
                return false;
            }

            return true;
        }

        /// <summary>
        /// Unmarshall object with all fields of primitive types or supported types. Handling data in array parameter as big endian. Not using properties but fields to retrieve values.
        /// </summary>
        /// <param name="p_o_type">type information for casting returning object</param>
        /// <param name="p_a_data">array of data as bytes which will be unmarshalled</param>
        /// <returns>new instance of object with p_o_class information parameter</returns>
        /// <exception cref="NullReferenceException">class information or data parameter are not set</exception>
        /// <exception cref="TypeLoadException">could not create new instance of return object; class that declares the underlying constructor represents an abstract class</exception>
        /// <exception cref="MemberAccessException">could not create new instance of return object; constructor object is enforcing language access control and the underlying constructor is inaccessible</exception>
        /// <exception cref="System.Reflection.TargetInvocationException">could not create new instance of return object; the underlying constructor throws an exception</exception>
        /// <exception cref="MemberAccessException">could not create new instance of return object; constructor object is enforcing language access control and the underlying constructor is inaccessible</exception>
        /// <exception cref="IndexOutOfRangeException">data accessed with invalid index</exception>
        /// <exception cref="ArgumentException">object parameter is not of a supported type || illegal or missing arguments for constructor</exception>
        /// <exception cref="NotSupportedException">little endian system data is NOT IMPLEMENTED || could not create new instance of return object; security vulnerabilites while calling the constructor or it is just not supported</exception>
        /// <exception cref="FieldAccessException">could not access field to set it's value</exception>
        /// <exception cref="MissingMemberException">could not find a property or field by member, using member info</exception>
        /// <exception cref="System.Reflection.TargetException">could not invoke property or field from object to set the value</exception>
        /// <exception cref="MethodAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">cannot convert string to datetime type or transposing decimal, details in exception message</exception>
        public static Object? UnmarshallObject(Type p_o_type, byte[] p_a_data)
        {
            return Marshall.UnmarshallObject(p_o_type, p_a_data, false);
        }

        /// <summary>
        /// Unmarshall object with all fields of primitive types or supported types. Handling data in array parameter as big endian.
        /// </summary>
        /// <param name="p_o_type">type information for casting returning object</param>
        /// <param name="p_a_data">array of data as bytes which will be unmarshalled</param>
        /// <param name="p_b_useProperties">access object parameter values via properties</param>
        /// <returns>new instance of object with p_o_class information parameter</returns>
        /// <exception cref="NullReferenceException">class information or data parameter are not set</exception>
        /// <exception cref="TypeLoadException">could not create new instance of return object; class that declares the underlying constructor represents an abstract class</exception>
        /// <exception cref="MemberAccessException">could not create new instance of return object; constructor object is enforcing language access control and the underlying constructor is inaccessible</exception>
        /// <exception cref="System.Reflection.TargetInvocationException">could not create new instance of return object; the underlying constructor throws an exception</exception>
        /// <exception cref="MemberAccessException">could not create new instance of return object; constructor object is enforcing language access control and the underlying constructor is inaccessible</exception>
        /// <exception cref="IndexOutOfRangeException">data accessed with invalid index</exception>
        /// <exception cref="ArgumentException">object parameter is not of a supported type || illegal or missing arguments for constructor</exception>
        /// <exception cref="NotSupportedException">little endian system data is NOT IMPLEMENTED || could not create new instance of return object; security vulnerabilites while calling the constructor or it is just not supported</exception>
        /// <exception cref="FieldAccessException">could not access field to set it's value</exception>
        /// <exception cref="MissingMemberException">could not find a property or field by member, using member info</exception>
        /// <exception cref="System.Reflection.TargetException">could not invoke property or field from object to set the value</exception>
        /// <exception cref="MethodAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">cannot convert string to datetime type or transposing decimal, details in exception message</exception>
        public static Object? UnmarshallObject(Type p_o_type, byte[] p_a_data, bool p_b_useProperties)
        {
            return Marshall.UnmarshallObject(p_o_type, p_a_data, p_b_useProperties, false);
        }

        /// <summary>
        /// Unmarshall object with all fields of primitive types or supported types. Handling data in array parameter as big endian.
        /// </summary>
        /// <param name="p_o_type">type information for casting returning object</param>
        /// <param name="p_a_data">array of data as bytes which will be unmarshalled</param>
        /// <param name="p_b_useProperties">access object parameter values via properties</param>
        /// <param name="p_b_systemUsesLittleEndian">(NOT IMPLEMENTED) true - current execution system uses little endian, false - current execution system uses big endian</param>
        /// <returns>new instance of object with p_o_class information parameter</returns>
        /// <exception cref="NullReferenceException">class information or data parameter are not set</exception>
        /// <exception cref="TypeLoadException">could not create new instance of return object; class that declares the underlying constructor represents an abstract class</exception>
        /// <exception cref="MemberAccessException">could not create new instance of return object; constructor object is enforcing language access control and the underlying constructor is inaccessible</exception>
        /// <exception cref="System.Reflection.TargetInvocationException">could not create new instance of return object; the underlying constructor throws an exception</exception>
        /// <exception cref="MemberAccessException">could not create new instance of return object; constructor object is enforcing language access control and the underlying constructor is inaccessible</exception>
        /// <exception cref="IndexOutOfRangeException">data accessed with invalid index</exception>
        /// <exception cref="ArgumentException">object parameter is not of a supported type || illegal or missing arguments for constructor</exception>
        /// <exception cref="NotSupportedException">little endian system data is NOT IMPLEMENTED || could not create new instance of return object; security vulnerabilites while calling the constructor or it is just not supported</exception>
        /// <exception cref="FieldAccessException">could not access field to set it's value</exception>
        /// <exception cref="MissingMemberException">could not find a property or field by member, using member info</exception>
        /// <exception cref="System.Reflection.TargetException">could not invoke property or field from object to set the value</exception>
        /// <exception cref="MethodAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">cannot convert string to datetime type or transposing decimal, details in exception message</exception>
        public static Object? UnmarshallObject(Type p_o_type, byte[] p_a_data, bool p_b_useProperties, bool p_b_systemUsesLittleEndian)
        {
            /* return object */
            Object? o_return = null;

            /* little endian system data is not supported right now */
            if (p_b_systemUsesLittleEndian)
            {
                throw new NotSupportedException("Supporting little endian system data is NOT IMPLEMENTED");
            }

            /* check if we have a generic type parameter */
            if (p_o_type == null)
            {
                throw new NullReferenceException("generic type parameter is null");
            }

            /* check if we have any data to unmarshall */
            if (p_a_data == null)
            {
                throw new NullReferenceException("data parameter is null");
            }

            /* if we have byte array of 5 elements and all are zero, then we can return null */
            if ((p_a_data.Length == 5) && (p_a_data[0] == 0) && (p_a_data[1] == 0) && (p_a_data[2] == 0) && (p_a_data[3] == 0) && (p_a_data[4] == 0))
            {
                return null;
            }

            /* check first two bytes for amount of fields - if they are both zero, we have not an object with fields but just a primitive supported variable */
            if ((p_a_data.Length > 1) && (p_a_data[0] == 0) && (p_a_data[1] == 0))
            {
                /* if we expect ForestNET.Lib.Net.Sock.Com.NullValue class as primitive, we return null */
                if (p_o_type == typeof(ForestNET.Lib.Net.Sock.Com.NullValue))
                {
                    return null;
                }

                if (Marshall.a_allowedTypes.Contains(p_o_type))
                { /* handle primitive supported type */
                    try
                    {
                        int i_bytePointer = 2;

                        /* read 1 byte name length */
                        int i_nameLength = (int)ForestNET.Lib.Helper.ByteArrayToShort(new byte[] { p_a_data[i_bytePointer++] });

                        string s_fieldName = "";

                        /* read name bytes if length greater than zero */
                        if (i_nameLength > 0)
                        {
                            /* read bytes for name */
                            byte[] a_nameBytes = new byte[i_nameLength];

                            for (int j = 0; j < i_nameLength; j++)
                            {
                                a_nameBytes[j] = p_a_data[i_bytePointer++];
                            }

                            /* store field name */
                            s_fieldName = System.Text.Encoding.UTF8.GetString(a_nameBytes);
                        }

                        /* read 2 bytes info */
                        byte[] a_info = new byte[2];

                        for (int j = 0; j < 2; j++)
                        {
                            a_info[j] = p_a_data[i_bytePointer++];
                        }

                        short sh_info = ForestNET.Lib.Helper.ByteArrayToShort(a_info);

                        /* get little endian bit */
                        bool b_littleEndianFlag = ((sh_info & 0x8000) == 0x8000);

                        /* get array flag bit */
                        bool b_arrayFlag = ((sh_info & 0x4000) == 0x4000);

                        /* get data length amount (2 bits) */
                        int i_dataLengthAmount = 0;

                        if (((sh_info & 0x2000) == 0) && ((sh_info & 0x1000) == 0))
                        {
                            i_dataLengthAmount = 1;
                        }
                        else if (((sh_info & 0x2000) == 0) && ((sh_info & 0x1000) == 0x1000))
                        {
                            i_dataLengthAmount = 2;
                        }
                        else if (((sh_info & 0x2000) == 0x2000) && ((sh_info & 0x1000) == 0))
                        {
                            i_dataLengthAmount = 3;
                        }
                        else if (((sh_info & 0x2000) == 0x2000) && ((sh_info & 0x1000) == 0x1000))
                        {
                            i_dataLengthAmount = 4;
                        }

                        /* get type (12 bits) */
                        int i_type = (sh_info & 0x0FFF);

                        /* read 2 bytes array amount */
                        byte[] a_arrayAmount = new byte[2];

                        for (int j = 0; j < 2; j++)
                        {
                            a_arrayAmount[j] = p_a_data[i_bytePointer++];
                        }

                        int i_arrayAmount = ForestNET.Lib.Helper.ByteArrayToShort(a_arrayAmount);

                        string s_empty = "                              ";

                        ForestNET.Lib.Global.ILogFiner(
                            s_fieldName +
                            s_empty.Substring(0, s_empty.Length - s_fieldName.Length) +
                            b_littleEndianFlag +
                            s_empty.Substring(0, s_empty.Length - ((b_littleEndianFlag) ? 4 : 5)) +
                            b_arrayFlag +
                            s_empty.Substring(0, s_empty.Length - ((b_arrayFlag) ? 4 : 5)) +
                            i_dataLengthAmount +
                            s_empty.Substring(0, s_empty.Length - Convert.ToString(i_dataLengthAmount).Length) +
                            i_type +
                            s_empty.Substring(0, s_empty.Length - Convert.ToString(i_type).Length) +
                            i_arrayAmount +
                            s_empty.Substring(0, s_empty.Length - Convert.ToString(i_arrayAmount).Length)
                        );

                        List<byte[]> a_dataList = [];

                        /* array amount > 0 ? */
                        if (i_arrayAmount > 0)
                        {
                            /* for k -> array amount */
                            for (int k = 0; k < i_arrayAmount; k++)
                            {
                                /* read data length (1-4 bytes, depending on data length amount) */
                                byte[] a_dataLength = new byte[i_dataLengthAmount];

                                for (int j = 0; j < i_dataLengthAmount; j++)
                                {
                                    a_dataLength[j] = p_a_data[i_bytePointer++];
                                }

                                int i_dataLength = ForestNET.Lib.Helper.ByteArrayToInt(a_dataLength);

                                /* read data with data length */
                                byte[] a_data = new byte[i_dataLength];

                                for (int j = 0; j < i_dataLength; j++)
                                {
                                    a_data[j] = p_a_data[i_bytePointer++];
                                }

                                a_dataList.Add(a_data);
                            }
                        }
                        else if (!b_arrayFlag)
                        { /* only continue read if we are sure that it is not an array and we have array amount < 1 */
                            /* read data length (1-4 bytes, depending on data length amount) */
                            byte[] a_dataLength = new byte[i_dataLengthAmount];

                            for (int j = 0; j < i_dataLengthAmount; j++)
                            {
                                a_dataLength[j] = p_a_data[i_bytePointer++];
                            }

                            int i_dataLength = ForestNET.Lib.Helper.ByteArrayToInt(a_dataLength);

                            /* read data with data length */
                            byte[] a_data = new byte[i_dataLength];

                            for (int j = 0; j < i_dataLength; j++)
                            {
                                a_data[j] = p_a_data[i_bytePointer++];
                            }

                            a_dataList.Add(a_data);
                        }

                        /* unmarshall data and add it directly to return object */
                        UnmarshallDataWithMemberInformation(i_type, ref o_return, null, false, b_arrayFlag, false, a_dataList);
                    }
                    catch (IndexOutOfRangeException o_exc)
                    {
                        ForestNET.Lib.Global.ILogWarning("data accessed with invalid index: " + o_exc);
                        throw new IndexOutOfRangeException("data accessed with invalid index: " + o_exc);
                    }
                    catch (Exception o_exc)
                    {
                        ForestNET.Lib.Global.ILogSevere("error while set return variable: " + o_exc);
                        throw;
                    }
                }
                else
                {
                    /* type is not supported */
                    throw new ArgumentException("Object parameter is not of a supported type '" + p_o_type + "'");
                }
            }
            else
            { /* expect object with fields */
                /* create new instance of return object and handle any exception which can occur */
                try
                {
                    o_return = Activator.CreateInstance(p_o_type);
                }
                catch (TypeLoadException o_exc)
                {
                    throw new TypeLoadException("could not create new instance of return object; class that declares the underlying constructor represents an abstract class: " + o_exc);
                }
                catch (MemberAccessException o_exc)
                {
                    throw new MemberAccessException("could not create new instance of return object; constructor object is enforcing language access control and the underlying constructor is inaccessible: " + o_exc);
                }
                catch (ArgumentException o_exc)
                {
                    throw new ArgumentException("could not create new instance of return object; illegal or missing arguments for constructor: " + o_exc);
                }
                catch (System.Reflection.TargetInvocationException o_exc)
                {
                    throw new System.Reflection.TargetInvocationException("could not create new instance of return object; the underlying constructor throws an exception", o_exc);
                }
                catch (NotSupportedException o_exc)
                {
                    throw new NotSupportedException("could not create new instance of return object; security vulnerabilites while calling the constructor or it is just not supported: " + o_exc);
                }

                try
                {
                    /* read amount of fields */
                    int i_amountFields = (int)ForestNET.Lib.Helper.ByteArrayToShort(new byte[] { p_a_data[0], p_a_data[1] });
                    int i_bytePointer = 2;

                    for (int i = 0; i < i_amountFields; i++)
                    {
                        /* read 1 byte name length */
                        int i_nameLength = (int)ForestNET.Lib.Helper.ByteArrayToShort(new byte[] { p_a_data[i_bytePointer++] });

                        /* read bytes for name */
                        byte[] a_nameBytes = new byte[i_nameLength];

                        for (int j = 0; j < i_nameLength; j++)
                        {
                            a_nameBytes[j] = p_a_data[i_bytePointer++];
                        }

                        /* store field/property name */
                        string s_name = System.Text.Encoding.UTF8.GetString(a_nameBytes);

                        /* read 2 bytes info */
                        byte[] a_info = new byte[2];

                        for (int j = 0; j < 2; j++)
                        {
                            a_info[j] = p_a_data[i_bytePointer++];
                        }

                        short sh_info = ForestNET.Lib.Helper.ByteArrayToShort(a_info);

                        /* get little endian bit */
                        bool b_littleEndianFlag = ((sh_info & 0x8000) == 0x8000);

                        /* get array flag bit */
                        bool b_arrayFlag = ((sh_info & 0x4000) == 0x4000);

                        /* get data length amount (2 bits) */
                        int i_dataLengthAmount = 0;

                        if (((sh_info & 0x2000) == 0) && ((sh_info & 0x1000) == 0))
                        {
                            i_dataLengthAmount = 1;
                        }
                        else if (((sh_info & 0x2000) == 0) && ((sh_info & 0x1000) == 0x1000))
                        {
                            i_dataLengthAmount = 2;
                        }
                        else if (((sh_info & 0x2000) == 0x2000) && ((sh_info & 0x1000) == 0))
                        {
                            i_dataLengthAmount = 3;
                        }
                        else if (((sh_info & 0x2000) == 0x2000) && ((sh_info & 0x1000) == 0x1000))
                        {
                            i_dataLengthAmount = 4;
                        }

                        /* get type (12 bits) */
                        int i_type = (sh_info & 0x0FFF);

                        /* read 2 bytes array amount */
                        byte[] a_arrayAmount = new byte[2];

                        for (int j = 0; j < 2; j++)
                        {
                            a_arrayAmount[j] = p_a_data[i_bytePointer++];
                        }

                        int i_arrayAmount = ForestNET.Lib.Helper.ByteArrayToShort(a_arrayAmount);

                        string s_empty = "                              ";

                        ForestNET.Lib.Global.ILogFiner(
                            s_name +
                            s_empty.Substring(0, s_empty.Length - s_name.Length) +
                            b_littleEndianFlag +
                            s_empty.Substring(0, s_empty.Length - ((b_littleEndianFlag) ? 4 : 5)) +
                            b_arrayFlag +
                            s_empty.Substring(0, s_empty.Length - ((b_arrayFlag) ? 4 : 5)) +
                            i_dataLengthAmount +
                            s_empty.Substring(0, s_empty.Length - Convert.ToString(i_dataLengthAmount).Length) +
                            i_type +
                            s_empty.Substring(0, s_empty.Length - Convert.ToString(i_type).Length) +
                            i_arrayAmount +
                            s_empty.Substring(0, s_empty.Length - Convert.ToString(i_arrayAmount).Length)
                        );

                        List<byte[]> a_dataList = [];

                        /* array amount > 0 ? */
                        if (i_arrayAmount > 0)
                        {
                            /* for k -> array amount */
                            for (int k = 0; k < i_arrayAmount; k++)
                            {
                                /* read data length (1-4 bytes, depending on data length amount) */
                                byte[] a_dataLength = new byte[i_dataLengthAmount];

                                for (int j = 0; j < i_dataLengthAmount; j++)
                                {
                                    a_dataLength[j] = p_a_data[i_bytePointer++];
                                }

                                int i_dataLength = ForestNET.Lib.Helper.ByteArrayToInt(a_dataLength);

                                /* read data with data length */
                                byte[] a_data = new byte[i_dataLength];

                                for (int j = 0; j < i_dataLength; j++)
                                {
                                    a_data[j] = p_a_data[i_bytePointer++];
                                }

                                a_dataList.Add(a_data);
                            }
                        }
                        else if (!b_arrayFlag)
                        { /* only continue read if we are sure that it is not an array and we have array amount < 1 */
                            /* read data length (1-4 bytes, depending on data length amount) */
                            byte[] a_dataLength = new byte[i_dataLengthAmount];

                            for (int j = 0; j < i_dataLengthAmount; j++)
                            {
                                a_dataLength[j] = p_a_data[i_bytePointer++];
                            }

                            int i_dataLength = ForestNET.Lib.Helper.ByteArrayToInt(a_dataLength);

                            /* read data with data length */
                            byte[] a_data = new byte[i_dataLength];

                            for (int j = 0; j < i_dataLength; j++)
                            {
                                a_data[j] = p_a_data[i_bytePointer++];
                            }

                            a_dataList.Add(a_data);
                        }

                        /* get member reflection */
                        System.Reflection.MemberInfo[] a_memberInfo = o_return?.GetType().GetMember(s_name) ?? [];

                        /* check if member could be found */
                        if (a_memberInfo.Length != 1)
                        {
                            throw new MissingMemberException("there are multiple members by name '" + s_name + "'");
                        }

                        /* get member info */
                        System.Reflection.MemberInfo o_memberInfo = a_memberInfo[0];

                        /* holding information about member info */
                        Type o_type = ((p_b_useProperties) ? ((System.Reflection.PropertyInfo)o_memberInfo).PropertyType : ((System.Reflection.FieldInfo)o_memberInfo).FieldType);
                        string s_type = o_type.FullName ?? "type full name is null";

                        /* check if field/property is a list or dictionary */
                        if ((o_type.IsGenericType) && ((o_type.GetGenericTypeDefinition().IsAssignableTo(typeof(System.Collections.IList))) || (o_type.GetGenericTypeDefinition().IsAssignableTo(typeof(System.Collections.IDictionary)))))
                        {
                            /* generic dictionary must have just two parameterized type declaration */
                            if (
                                (o_type.GetGenericTypeDefinition().IsAssignableTo(typeof(System.Collections.IDictionary))) &&
                                (o_type.GenericTypeArguments.Length == 2)
                            )
                            {
                                /* parameterized type declaration of key part of dictionary must be 'int' */
                                if (o_type.GenericTypeArguments[0] != typeof(int))
                                {
                                    ForestNET.Lib.Global.ILogFiner("parameterized type declaration of key part of dictionary must be 'int', so we must skip this field '" + s_name + "'");
                                    continue;
                                }
                            }
                            else if (o_type.GenericTypeArguments.Length != 1)
                            { /* parameterized type declaration which are not equal '1' is not supported, so we must skip this field */
                                ForestNET.Lib.Global.ILogFiner("parameterized type declaration which are not equal '1' is not supported, so we must skip this field '" + s_name + "'");
                                continue;
                            }
                        }

                        /* unmarshall data with member information */
                        UnmarshallDataWithMemberInformation(i_type, ref o_return, o_memberInfo, p_b_useProperties, b_arrayFlag, o_type.IsGenericType, a_dataList);
                    }
                }
                catch (IndexOutOfRangeException o_exc)
                {
                    ForestNET.Lib.Global.ILogWarning("data accessed with invalid index: " + o_exc);

                    throw new IndexOutOfRangeException("data accessed with invalid index: " + o_exc);
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.ILogSevere("error while set a field/property value: " + o_exc);
                    throw;
                }
            }

            return o_return;
        }

        /// <summary>
        /// Unmarshalling data by field or object information
        /// </summary>
        /// <param name="p_i_type">read marshalled type of data</param>
        /// <param name="p_o_return">object parameter where data will be added into object fields</param>
        /// <param name="p_o_memberInfo">all information about the generic list or map field</param>
        /// <param name="p_b_useProperties">access object parameter values via properties</param>
        /// <param name="p_b_arrayFlag">marshalled data is of type array</param>
        /// <param name="p_b_isGenericList">marshalled data is of type generic list</param>
        /// <param name="p_a_dataList">marshalled data as list of multiple byte arrays for primitive objects, arrays and generic lists</param>
        /// <exception cref="FieldAccessException">could not access field to set it's value</exception>
        /// <exception cref="MissingMemberException">could not find a property or field by member, using member info</exception>
        /// <exception cref="System.Reflection.TargetException">could not invoke property or field from object to set the value</exception>
        /// <exception cref="MethodAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">cannot convert string to datetime type or transposing decimal, details in exception message</exception>
        private static void UnmarshallDataWithMemberInformation(int p_i_type, ref object? p_o_return, System.Reflection.MemberInfo? p_o_memberInfo, bool p_b_useProperties, bool p_b_arrayFlag, bool p_b_isGenericList, List<byte[]> p_a_dataList)
        {
            /* pass data to object fields */
            if (p_i_type == 0)
            { /* data for type bool */
                if (!p_b_arrayFlag)
                { /* just a field */
                    /* set field value */
                    if (p_a_dataList[0][0] == 0x1)
                    {
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, (bool)true, p_b_useProperties);
                    }
                    else
                    {
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, (bool)false, p_b_useProperties);
                    }
                }
                else if ((!p_b_isGenericList) && (p_b_arrayFlag))
                { /* usual array */
                    if (p_a_dataList.Count < 1)
                    { /* set null if we have no data for usual array */
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, null, p_b_useProperties);
                    }
                    else
                    {
                        /* prepare usual array */
                        bool[] a_temp = new bool[p_a_dataList.Count];

                        /* counter */
                        int i_cnt = 0;

                        foreach (byte[] a_byteChunks in p_a_dataList)
                        {
                            /* set array value */
                            if (a_byteChunks[0] == 0x1)
                            {
                                a_temp[i_cnt++] = true;
                            }
                            else
                            {
                                a_temp[i_cnt++] = false;
                            }
                        }

                        /* set field value */
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, (bool[])a_temp, p_b_useProperties);
                    }
                }
                else if ((p_b_isGenericList) && (p_b_arrayFlag))
                { /* dynamic generic list */
                    Marshall.SetGenericList(typeof(bool), ref p_o_return, p_o_memberInfo, p_a_dataList, p_b_useProperties);
                }
            }
            else if (p_i_type == 1)
            { /* data for type Byte */
                if (!p_b_arrayFlag)
                { /* just a field */
                    /* set field value */
                    Marshall.SetValue(ref p_o_return, p_o_memberInfo, (byte)p_a_dataList[0][0], p_b_useProperties);
                }
                else if ((!p_b_isGenericList) && (p_b_arrayFlag))
                { /* usual array */
                    if (p_a_dataList.Count < 1)
                    { /* set null if we have no data for usual array */
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, null, p_b_useProperties);
                    }
                    else
                    {
                        /* prepare usual array */
                        byte[] a_temp = new byte[p_a_dataList.Count];

                        /* counter */
                        int i_cnt = 0;

                        foreach (byte[] a_byteChunks in p_a_dataList)
                        {
                            /* set array value */
                            a_temp[i_cnt++] = a_byteChunks[0];
                        }

                        /* set field value */
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, (byte[])a_temp, p_b_useProperties);
                    }
                }
                else if ((p_b_isGenericList) && (p_b_arrayFlag))
                { /* dynamic generic list */
                    Marshall.SetGenericList(typeof(byte), ref p_o_return, p_o_memberInfo, p_a_dataList, p_b_useProperties);
                }
            }
            else if (p_i_type == 2)
            { /* data for type SByte */
                if (!p_b_arrayFlag)
                { /* just a field */
                    /* set field value */
                    Marshall.SetValue(ref p_o_return, p_o_memberInfo, (sbyte)p_a_dataList[0][0], p_b_useProperties);
                }
                else if ((!p_b_isGenericList) && (p_b_arrayFlag))
                { /* usual array */
                    if (p_a_dataList.Count < 1)
                    { /* set null if we have no data for usual array */
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, null, p_b_useProperties);
                    }
                    else
                    {
                        /* prepare usual array */
                        sbyte[] a_temp = new sbyte[p_a_dataList.Count];

                        /* counter */
                        int i_cnt = 0;

                        foreach (byte[] a_byteChunks in p_a_dataList)
                        {
                            /* set array value */
                            a_temp[i_cnt++] = (sbyte)a_byteChunks[0];
                        }

                        /* set field value */
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, (sbyte[])a_temp, p_b_useProperties);
                    }
                }
                else if ((p_b_isGenericList) && (p_b_arrayFlag))
                { /* dynamic generic list */
                    Marshall.SetGenericList(typeof(sbyte), ref p_o_return, p_o_memberInfo, p_a_dataList, p_b_useProperties);
                }
            }
            else if (p_i_type == 3)
            { /* data for type Character */
                if (!p_b_arrayFlag)
                { /* just a field */
                    /* set field value */
                    char c_temp = (char)(0xFF & p_a_dataList[0][0]);
                    Marshall.SetValue(ref p_o_return, p_o_memberInfo, (char)c_temp, p_b_useProperties);
                }
                else if ((!p_b_isGenericList) && (p_b_arrayFlag))
                { /* usual array */
                    if (p_a_dataList.Count < 1)
                    { /* set null if we have no data for usual array */
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, null, p_b_useProperties);
                    }
                    else
                    {
                        /* prepare usual array */
                        char[] a_temp = new char[p_a_dataList.Count];

                        /* counter */
                        int i_cnt = 0;

                        foreach (byte[] a_byteChunks in p_a_dataList)
                        {
                            /* set array value */
                            a_temp[i_cnt++] = (char)(0xFF & a_byteChunks[0]);
                        }

                        /* set field value */
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, (char[])a_temp, p_b_useProperties);
                    }
                }
                else if ((p_b_isGenericList) && (p_b_arrayFlag))
                { /* dynamic generic list */
                    Marshall.SetGenericList(typeof(char), ref p_o_return, p_o_memberInfo, p_a_dataList, p_b_useProperties);
                }
            }
            else if (p_i_type == 4)
            { /* data for type Float */
                if (!p_b_arrayFlag)
                { /* just a field */
                    /* set field value */
                    Marshall.SetValue(ref p_o_return, p_o_memberInfo, (float)BitConverter.Int32BitsToSingle(ForestNET.Lib.Helper.ByteArrayToInt(p_a_dataList[0])), p_b_useProperties);
                }
                else if ((!p_b_isGenericList) && (p_b_arrayFlag))
                { /* usual array */
                    if (p_a_dataList.Count < 1)
                    { /* set null if we have no data for usual array */
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, null, p_b_useProperties);
                    }
                    else
                    {
                        /* prepare usual array */
                        float[] a_temp = new float[p_a_dataList.Count];

                        /* counter */
                        int i_cnt = 0;

                        foreach (byte[] a_byteChunks in p_a_dataList)
                        {
                            /* set array value */
                            a_temp[i_cnt++] = BitConverter.Int32BitsToSingle(ForestNET.Lib.Helper.ByteArrayToInt(a_byteChunks));
                        }

                        /* set field value */
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, (float[])a_temp, p_b_useProperties);
                    }
                }
                else if ((p_b_isGenericList) && (p_b_arrayFlag))
                { /* dynamic generic list */
                    Marshall.SetGenericList(typeof(float), ref p_o_return, p_o_memberInfo, p_a_dataList, p_b_useProperties);
                }
            }
            else if (p_i_type == 5)
            { /* data for type Double */
                if (!p_b_arrayFlag)
                { /* just a field */
                    /* set field value */
                    Marshall.SetValue(ref p_o_return, p_o_memberInfo, (double)BitConverter.Int64BitsToDouble(ForestNET.Lib.Helper.ByteArrayToLong(p_a_dataList[0])), p_b_useProperties);
                }
                else if ((!p_b_isGenericList) && (p_b_arrayFlag))
                { /* usual array */
                    if (p_a_dataList.Count < 1)
                    { /* set null if we have no data for usual array */
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, null, p_b_useProperties);
                    }
                    else
                    {
                        /* prepare usual array */
                        double[] a_temp = new double[p_a_dataList.Count];

                        /* counter */
                        int i_cnt = 0;

                        foreach (byte[] a_byteChunks in p_a_dataList)
                        {
                            /* set array value */
                            a_temp[i_cnt++] = BitConverter.Int64BitsToDouble(ForestNET.Lib.Helper.ByteArrayToLong(a_byteChunks));
                        }

                        /* set field value */
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, (double[])a_temp, p_b_useProperties);
                    }
                }
                else if ((p_b_isGenericList) && (p_b_arrayFlag))
                { /* dynamic generic list */
                    Marshall.SetGenericList(typeof(double), ref p_o_return, p_o_memberInfo, p_a_dataList, p_b_useProperties);
                }
            }
            else if (p_i_type == 6)
            { /* data for type Short */
                if (!p_b_arrayFlag)
                { /* just a field */
                    /* set field value */
                    Marshall.SetValue(ref p_o_return, p_o_memberInfo, (short)ForestNET.Lib.Helper.ByteArrayToShort(p_a_dataList[0]), p_b_useProperties);
                }
                else if ((!p_b_isGenericList) && (p_b_arrayFlag))
                { /* usual array */
                    if (p_a_dataList.Count < 1)
                    { /* set null if we have no data for usual array */
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, null, p_b_useProperties);
                    }
                    else
                    {
                        /* prepare usual array */
                        short[] a_temp = new short[p_a_dataList.Count];

                        /* counter */
                        int i_cnt = 0;

                        foreach (byte[] a_byteChunks in p_a_dataList)
                        {
                            /* set array value */
                            a_temp[i_cnt++] = ForestNET.Lib.Helper.ByteArrayToShort(a_byteChunks);
                        }

                        /* set field value */
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, (short[])a_temp, p_b_useProperties);
                    }
                }
                else if ((p_b_isGenericList) && (p_b_arrayFlag))
                { /* dynamic generic list */
                    Marshall.SetGenericList(typeof(short), ref p_o_return, p_o_memberInfo, p_a_dataList, p_b_useProperties);
                }
            }
            else if (p_i_type == 7)
            { /* data for type Unsigned Short */
                if (!p_b_arrayFlag)
                { /* just a field */
                    /* set field value */
                    Marshall.SetValue(ref p_o_return, p_o_memberInfo, (ushort)ForestNET.Lib.Helper.ByteArrayToShort(p_a_dataList[0]), p_b_useProperties);
                }
                else if ((!p_b_isGenericList) && (p_b_arrayFlag))
                { /* usual array */
                    if (p_a_dataList.Count < 1)
                    { /* set null if we have no data for usual array */
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, null, p_b_useProperties);
                    }
                    else
                    {
                        /* prepare usual array */
                        ushort[] a_temp = new ushort[p_a_dataList.Count];

                        /* counter */
                        int i_cnt = 0;

                        foreach (byte[] a_byteChunks in p_a_dataList)
                        {
                            /* set array value */
                            a_temp[i_cnt++] = (ushort)ForestNET.Lib.Helper.ByteArrayToShort(a_byteChunks);
                        }

                        /* set field value */
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, (ushort[])a_temp, p_b_useProperties);
                    }
                }
                else if ((p_b_isGenericList) && (p_b_arrayFlag))
                { /* dynamic generic list */
                    Marshall.SetGenericList(typeof(ushort), ref p_o_return, p_o_memberInfo, p_a_dataList, p_b_useProperties);
                }
            }
            else if (p_i_type == 8)
            { /* data for type Integer */
                if (!p_b_arrayFlag)
                { /* just a field */
                    /* set field value */
                    Marshall.SetValue(ref p_o_return, p_o_memberInfo, (int)ForestNET.Lib.Helper.ByteArrayToInt(p_a_dataList[0]), p_b_useProperties);
                }
                else if ((!p_b_isGenericList) && (p_b_arrayFlag))
                { /* usual array */
                    if (p_a_dataList.Count < 1)
                    { /* set null if we have no data for usual array */
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, null, p_b_useProperties);
                    }
                    else
                    {
                        /* prepare usual array */
                        int[] a_temp = new int[p_a_dataList.Count];

                        /* counter */
                        int i_cnt = 0;

                        foreach (byte[] a_byteChunks in p_a_dataList)
                        {
                            /* set array value */
                            a_temp[i_cnt++] = ForestNET.Lib.Helper.ByteArrayToInt(a_byteChunks);
                        }

                        /* set field value */
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, (int[])a_temp, p_b_useProperties);
                    }
                }
                else if ((p_b_isGenericList) && (p_b_arrayFlag))
                { /* dynamic generic list */
                    Marshall.SetGenericList(typeof(int), ref p_o_return, p_o_memberInfo, p_a_dataList, p_b_useProperties);
                }
            }
            else if (p_i_type == 9)
            { /* data for type Unsigned Integer */
                if (!p_b_arrayFlag)
                { /* just a field */
                    /* set field value */
                    Marshall.SetValue(ref p_o_return, p_o_memberInfo, (uint)ForestNET.Lib.Helper.ByteArrayToInt(p_a_dataList[0]), p_b_useProperties);
                }
                else if ((!p_b_isGenericList) && (p_b_arrayFlag))
                { /* usual array */
                    if (p_a_dataList.Count < 1)
                    { /* set null if we have no data for usual array */
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, null, p_b_useProperties);
                    }
                    else
                    {
                        /* prepare usual array */
                        uint[] a_temp = new uint[p_a_dataList.Count];

                        /* counter */
                        int i_cnt = 0;

                        foreach (byte[] a_byteChunks in p_a_dataList)
                        {
                            /* set array value */
                            a_temp[i_cnt++] = (uint)ForestNET.Lib.Helper.ByteArrayToInt(a_byteChunks);
                        }

                        /* set field value */
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, (uint[])a_temp, p_b_useProperties);
                    }
                }
                else if ((p_b_isGenericList) && (p_b_arrayFlag))
                { /* dynamic generic list */
                    Marshall.SetGenericList(typeof(uint), ref p_o_return, p_o_memberInfo, p_a_dataList, p_b_useProperties);
                }
            }
            else if (p_i_type == 10)
            { /* data for type Long */
                if (!p_b_arrayFlag)
                { /* just a field */
                    /* set field value */
                    Marshall.SetValue(ref p_o_return, p_o_memberInfo, (long)ForestNET.Lib.Helper.ByteArrayToLong(p_a_dataList[0]), p_b_useProperties);
                }
                else if ((!p_b_isGenericList) && (p_b_arrayFlag))
                { /* usual array */
                    if (p_a_dataList.Count < 1)
                    { /* set null if we have no data for usual array */
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, null, p_b_useProperties);
                    }
                    else
                    {
                        /* prepare usual array */
                        long[] a_temp = new long[p_a_dataList.Count];

                        /* counter */
                        int i_cnt = 0;

                        foreach (byte[] a_byteChunks in p_a_dataList)
                        {
                            /* set array value */
                            a_temp[i_cnt++] = ForestNET.Lib.Helper.ByteArrayToLong(a_byteChunks);
                        }

                        /* set field value */
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, (long[])a_temp, p_b_useProperties);
                    }
                }
                else if ((p_b_isGenericList) && (p_b_arrayFlag))
                { /* dynamic generic list */
                    Marshall.SetGenericList(typeof(long), ref p_o_return, p_o_memberInfo, p_a_dataList, p_b_useProperties);
                }
            }
            else if (p_i_type == 11)
            { /* data for type Unsigned Long */
                if (!p_b_arrayFlag)
                { /* just a field */
                    /* set field value */
                    Marshall.SetValue(ref p_o_return, p_o_memberInfo, (ulong)ForestNET.Lib.Helper.ByteArrayToLong(p_a_dataList[0]), p_b_useProperties);
                }
                else if ((!p_b_isGenericList) && (p_b_arrayFlag))
                { /* usual array */
                    if (p_a_dataList.Count < 1)
                    { /* set null if we have no data for usual array */
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, null, p_b_useProperties);
                    }
                    else
                    {
                        /* prepare usual array */
                        ulong[] a_temp = new ulong[p_a_dataList.Count];

                        /* counter */
                        int i_cnt = 0;

                        foreach (byte[] a_byteChunks in p_a_dataList)
                        {
                            /* set array value */
                            a_temp[i_cnt++] = (ulong)ForestNET.Lib.Helper.ByteArrayToLong(a_byteChunks);
                        }

                        /* set field value */
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, (ulong[])a_temp, p_b_useProperties);
                    }
                }
                else if ((p_b_isGenericList) && (p_b_arrayFlag))
                { /* dynamic generic list */
                    Marshall.SetGenericList(typeof(ulong), ref p_o_return, p_o_memberInfo, p_a_dataList, p_b_useProperties);
                }
            }
            else if (p_i_type == 12)
            { /* data for type string */
                if (!p_b_arrayFlag)
                { /* just a field */
                    /* set field value */
                    Marshall.SetValue(ref p_o_return, p_o_memberInfo, (string?)System.Text.Encoding.UTF8.GetString(p_a_dataList[0]), p_b_useProperties);
                }
                else if ((!p_b_isGenericList) && (p_b_arrayFlag))
                { /* usual array */
                    if (p_a_dataList.Count < 1)
                    { /* set null if we have no data for usual array */
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, null, p_b_useProperties);
                    }
                    else
                    {
                        /* prepare usual array */
                        string?[] a_temp = new string?[p_a_dataList.Count];

                        /* counter */
                        int i_cnt = 0;

                        foreach (byte[] a_byteChunks in p_a_dataList)
                        {
                            if (a_byteChunks.Length < 1)
                            {
                                /* data has no length, so we add null */
                                a_temp[i_cnt++] = null;
                            }
                            else
                            {
                                /* set array value */
                                a_temp[i_cnt++] = System.Text.Encoding.UTF8.GetString(a_byteChunks);
                            }
                        }

                        /* set field value */
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, (string?[])a_temp, p_b_useProperties);
                    }
                }
                else if ((p_b_isGenericList) && (p_b_arrayFlag))
                { /* dynamic generic list */
                    Marshall.SetGenericList(typeof(string), ref p_o_return, p_o_memberInfo, p_a_dataList, p_b_useProperties);
                }
            }
            else if (p_i_type == 13)
            { /* data for type DateTime */
                if (!p_b_arrayFlag)
                { /* just a field */
                    if (p_a_dataList[0].Length == 0)
                    { /* no data for field, set it to null */
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, null, p_b_useProperties);
                    }
                    else
                    { /* set field value */
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, (DateTime?)ForestNET.Lib.Helper.FromISO8601UTC(System.Text.Encoding.UTF8.GetString(p_a_dataList[0])), p_b_useProperties);
                    }
                }
                else if ((!p_b_isGenericList) && (p_b_arrayFlag))
                { /* usual array */
                    if (p_a_dataList.Count < 1)
                    { /* set null if we have no data for usual array */
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, null, p_b_useProperties);
                    }
                    else
                    { /* retrieve array data */
                        /* prepare usual array */
                        DateTime?[] a_temp = new DateTime?[p_a_dataList.Count];

                        /* counter */
                        int i_cnt = 0;

                        foreach (byte[] a_byteChunks in p_a_dataList)
                        {
                            if (a_byteChunks.Length < 1)
                            {
                                /* data has no length, so we add null */
                                a_temp[i_cnt++] = null;
                            }
                            else
                            {
                                /* set array value */
                                a_temp[i_cnt++] = ForestNET.Lib.Helper.FromISO8601UTC(System.Text.Encoding.UTF8.GetString(a_byteChunks));
                            }
                        }

                        /* set field value */
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, (DateTime?[])a_temp, p_b_useProperties);
                    }
                }
                else if ((p_b_isGenericList) && (p_b_arrayFlag))
                { /* dynamic generic list */
                    /* retrieve list data */
                    Marshall.SetGenericList(typeof(DateTime?), ref p_o_return, p_o_memberInfo, p_a_dataList, p_b_useProperties);
                }
            }
            else if (p_i_type == 14)
            { /* data for type decimal */
                if (!p_b_arrayFlag)
                { /* just a field */
                    if (p_a_dataList[0].Length == 0)
                    { /* no data for field, set it to null */
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, 0.0m, p_b_useProperties);
                    }
                    else
                    { /* set field value */
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, (decimal)(ForestNET.Lib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimal(System.Text.Encoding.UTF8.GetString(p_a_dataList[0]), 14) ?? 0.0m), p_b_useProperties);
                    }
                }
                else if ((!p_b_isGenericList) && (p_b_arrayFlag))
                { /* usual array */
                    if (p_a_dataList.Count < 1)
                    { /* set null if we have no data for usual array */
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, null, p_b_useProperties);
                    }
                    else
                    { /* retrieve array data */
                        /* prepare usual array */
                        decimal[] a_temp = new decimal[p_a_dataList.Count];

                        /* counter */
                        int i_cnt = 0;

                        foreach (byte[] a_byteChunks in p_a_dataList)
                        {
                            if (a_byteChunks.Length < 1)
                            {
                                /* data has no length, so we add null */
                                a_temp[i_cnt++] = 0.0m;
                            }
                            else
                            {
                                /* set array value */
                                a_temp[i_cnt++] = (decimal)(ForestNET.Lib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimal(System.Text.Encoding.UTF8.GetString(a_byteChunks), 14) ?? 0.0m);
                            }
                        }

                        /* set field value */
                        Marshall.SetValue(ref p_o_return, p_o_memberInfo, (decimal[])a_temp, p_b_useProperties);
                    }
                }
                else if ((p_b_isGenericList) && (p_b_arrayFlag))
                { /* dynamic generic list */
                    /* retrieve list data */
                    Marshall.SetGenericList(typeof(decimal), ref p_o_return, p_o_memberInfo, p_a_dataList, p_b_useProperties);
                }
            }
            else
            {
                throw new NotSupportedException("Type with number '" + p_i_type + "' is not supported");
            }
        }

        /// <summary>
        /// Method to set field/property of an object with simple object value, so no cast will be done, parameter object must have the get correct type before
        /// </summary>
        /// <param name="p_o_return">object parameter where data will be added into object fields</param>
        /// <param name="p_o_memberInfo">member info instance to set property or field value</param>
        /// <param name="p_o_objectValue">object value for property field</param>
        /// <param name="p_b_useProperties">access object parameter values via properties</param>
        /// <exception cref="FieldAccessException">could not access field to set it's value</exception>
        /// <exception cref="MissingMemberException">could not find a property or field by member, using member info</exception>
        /// <exception cref="System.Reflection.TargetException">could not invoke property or field from object to set the value</exception>
        /// <exception cref="MethodAccessException">could not invoke method, access violation</exception>
        private static void SetValue(ref Object? p_o_return, System.Reflection.MemberInfo? p_o_memberInfo, Object? p_o_objectValue, bool p_b_useProperties)
        {
            /* directly assign object value to p_o_return */
            if (p_o_memberInfo == null)
            {
                p_o_return = p_o_objectValue;
            }
            else if (p_b_useProperties) /* use properties only to set object data values */
            {
                /* set value to property of current object */
                ((System.Reflection.PropertyInfo)p_o_memberInfo).SetValue(p_o_return, p_o_objectValue);
            }
            else
            {
                /* set value to field of current object */
                ((System.Reflection.FieldInfo)p_o_memberInfo).SetValue(p_o_return, p_o_objectValue);
            }
        }

        /// <summary>
        /// Setting generic list field after received data, each element stored within a single byte array
        /// </summary>
        /// <param name="p_o_genericType">tells us the parameterized type of the generic list or map</param>
        /// <param name="p_o_return">object instance where we want to set the generic list field</param>
        /// <param name="p_o_memberInfo">all information about the generic list or dictionary by member info</param>
        /// <param name="p_a_dataList">array of bytes where we want to unmarshall our generic list's content</param>
        /// <param name="p_b_useProperties">access object parameter values via properties</param>
        /// <exception cref="FieldAccessException">could not access field to set it's value</exception>
        /// <exception cref="MissingMemberException">could not find a property or field by member, using member info</exception>
        /// <exception cref="System.Reflection.TargetException">could not invoke property or field from object to set the value</exception>
        /// <exception cref="MethodAccessException">could not invoke method, access violation</exception>
        private static void SetGenericList(Type p_o_genericType, ref Object? p_o_return, System.Reflection.MemberInfo? p_o_memberInfo, List<byte[]> p_a_dataList, bool p_b_useProperties)
        {
            /* check that member info is available */
            if (p_o_memberInfo == null)
            {
                throw new NullReferenceException("cannot set generic list if member info is null");
            }

            /* get generic list type and if it is a list or a dictionary */
            Type o_type = ((p_b_useProperties) ? ((System.Reflection.PropertyInfo)p_o_memberInfo).PropertyType : ((System.Reflection.FieldInfo)p_o_memberInfo).FieldType);
            bool b_isList = o_type.GetGenericTypeDefinition().IsAssignableTo(typeof(System.Collections.IList));
            bool b_isDictionary = o_type.GetGenericTypeDefinition().IsAssignableTo(typeof(System.Collections.IDictionary));

            if ((!b_isList) && (!b_isDictionary))
            {
                throw new NotSupportedException("member of object is not a generic list and not a generic dictionary");
            }

            /* variable for generic list */
            Object? o_foo = null;

            if (p_o_genericType == typeof(bool))
            {
                if (b_isList)
                {
                    /* prepare list */
                    List<bool?> a_temp = [];

                    for (int i = 0; i < p_a_dataList.Count; i++)
                    {
                        /* set list value */
                        if (p_a_dataList[i].Length < 1)
                        {
                            /* we have an array element with no data, so we add a null */
                            a_temp.Add(null);
                        }
                        else if (p_a_dataList[i][0] == 0x1)
                        {
                            a_temp.Add(true);
                        }
                        else
                        {
                            a_temp.Add(false);
                        }
                    }

                    /* give list to object */
                    o_foo = a_temp;
                }
                else if (b_isDictionary)
                {
                    /* prepare dictionary */
                    Dictionary<int, bool?> a_temp = [];

                    /* counter */
                    int i_cnt = 0;

                    for (int i = 0; i < p_a_dataList.Count; i++)
                    {
                        /* set map value */
                        if (p_a_dataList[i].Length < 1)
                        {
                            /* we have an array element with no data, so we add a null */
                            a_temp.Add(i_cnt++, null);
                        }
                        else if (p_a_dataList[i][0] == 0x1)
                        {
                            a_temp.Add(i_cnt++, true);
                        }
                        else
                        {
                            a_temp.Add(i_cnt++, false);
                        }
                    }

                    /* give dictionary to object */
                    o_foo = a_temp;
                }

                /* set field/property generic value */
                Marshall.SetValue(ref p_o_return, p_o_memberInfo, o_foo, p_b_useProperties);
            }
            else if (p_o_genericType == typeof(byte))
            {
                if (b_isList)
                {
                    /* prepare list */
                    List<byte?> a_temp = [];

                    for (int i = 0; i < p_a_dataList.Count; i++)
                    {
                        /* set list value */
                        if (p_a_dataList[i].Length < 1)
                        {
                            /* we have an array element with no data, so we add a null */
                            a_temp.Add(null);
                        }
                        else
                        {
                            a_temp.Add(p_a_dataList[i][0]);
                        }
                    }

                    /* give list to object */
                    o_foo = a_temp;
                }
                else if (b_isDictionary)
                {
                    /* prepare dictionary */
                    Dictionary<int, byte?> a_temp = [];

                    /* counter */
                    int i_cnt = 0;

                    for (int i = 0; i < p_a_dataList.Count; i++)
                    {
                        /* set map value */
                        if (p_a_dataList[i].Length < 1)
                        {
                            /* we have an array element with no data, so we add a null */
                            a_temp.Add(i_cnt++, null);
                        }
                        else
                        {
                            a_temp.Add(i_cnt++, p_a_dataList[i][0]);
                        }
                    }

                    /* give dictionary to object */
                    o_foo = a_temp;
                }

                /* set field/property generic value */
                Marshall.SetValue(ref p_o_return, p_o_memberInfo, o_foo, p_b_useProperties);
            }
            else if (p_o_genericType == typeof(sbyte))
            {
                if (b_isList)
                {
                    /* prepare list */
                    List<sbyte?> a_temp = [];

                    for (int i = 0; i < p_a_dataList.Count; i++)
                    {
                        /* set list value */
                        if (p_a_dataList[i].Length < 1)
                        {
                            /* we have an array element with no data, so we add a null */
                            a_temp.Add(null);
                        }
                        else
                        {
                            a_temp.Add((sbyte)p_a_dataList[i][0]);
                        }
                    }

                    /* give list to object */
                    o_foo = a_temp;
                }
                else if (b_isDictionary)
                {
                    /* prepare dictionary */
                    Dictionary<int, sbyte?> a_temp = [];

                    /* counter */
                    int i_cnt = 0;

                    for (int i = 0; i < p_a_dataList.Count; i++)
                    {
                        /* set map value */
                        if (p_a_dataList[i].Length < 1)
                        {
                            /* we have an array element with no data, so we add a null */
                            a_temp.Add(i_cnt++, null);
                        }
                        else
                        {
                            a_temp.Add(i_cnt++, (sbyte)p_a_dataList[i][0]);
                        }
                    }

                    /* give dictionary to object */
                    o_foo = a_temp;
                }

                /* set field/property generic value */
                Marshall.SetValue(ref p_o_return, p_o_memberInfo, o_foo, p_b_useProperties);
            }
            else if (p_o_genericType == typeof(char))
            {
                if (b_isList)
                {
                    /* prepare list */
                    List<char?> a_temp = [];

                    for (int i = 0; i < p_a_dataList.Count; i++)
                    {
                        /* set list value */
                        if (p_a_dataList[i].Length < 1)
                        {
                            /* we have an array element with no data, so we add a null */
                            a_temp.Add(null);
                        }
                        else
                        {
                            a_temp.Add((char)(0xFF & p_a_dataList[i][0]));
                        }
                    }

                    /* give list to object */
                    o_foo = a_temp;
                }
                else if (b_isDictionary)
                {
                    /* prepare dictionary */
                    Dictionary<int, char?> a_temp = [];

                    /* counter */
                    int i_cnt = 0;

                    for (int i = 0; i < p_a_dataList.Count; i++)
                    {
                        /* set map value */
                        if (p_a_dataList[i].Length < 1)
                        {
                            /* we have an array element with no data, so we add a null */
                            a_temp.Add(i_cnt++, null);
                        }
                        else
                        {
                            a_temp.Add(i_cnt++, (char)(0xFF & p_a_dataList[i][0]));
                        }
                    }

                    /* give dictionary to object */
                    o_foo = a_temp;
                }

                /* set field/property generic value */
                Marshall.SetValue(ref p_o_return, p_o_memberInfo, o_foo, p_b_useProperties);
            }
            else if (p_o_genericType == typeof(float))
            {
                if (b_isList)
                {
                    /* prepare list */
                    List<float?> a_temp = [];

                    for (int i = 0; i < p_a_dataList.Count; i++)
                    {
                        /* set list value */
                        if (p_a_dataList[i].Length < 1)
                        {
                            /* we have an array element with no data, so we add a null */
                            a_temp.Add(null);
                        }
                        else
                        {
                            a_temp.Add(BitConverter.Int32BitsToSingle(ForestNET.Lib.Helper.ByteArrayToInt(p_a_dataList[i])));
                        }
                    }

                    /* give list to object */
                    o_foo = a_temp;
                }
                else if (b_isDictionary)
                {
                    /* prepare dictionary */
                    Dictionary<int, float?> a_temp = [];

                    /* counter */
                    int i_cnt = 0;

                    for (int i = 0; i < p_a_dataList.Count; i++)
                    {
                        /* set map value */
                        if (p_a_dataList[i].Length < 1)
                        {
                            /* we have an array element with no data, so we add a null */
                            a_temp.Add(i_cnt++, null);
                        }
                        else
                        {
                            a_temp.Add(i_cnt++, BitConverter.Int32BitsToSingle(ForestNET.Lib.Helper.ByteArrayToInt(p_a_dataList[i])));
                        }
                    }

                    /* give dictionary to object */
                    o_foo = a_temp;
                }

                /* set field/property generic value */
                Marshall.SetValue(ref p_o_return, p_o_memberInfo, o_foo, p_b_useProperties);
            }
            else if (p_o_genericType == typeof(double))
            {
                if (b_isList)
                {
                    /* prepare list */
                    List<double?> a_temp = [];

                    for (int i = 0; i < p_a_dataList.Count; i++)
                    {
                        /* set list value */
                        if (p_a_dataList[i].Length < 1)
                        {
                            /* we have an array element with no data, so we add a null */
                            a_temp.Add(null);
                        }
                        else
                        {
                            a_temp.Add(BitConverter.Int64BitsToDouble(ForestNET.Lib.Helper.ByteArrayToLong(p_a_dataList[i])));
                        }
                    }

                    /* give list to object */
                    o_foo = a_temp;
                }
                else if (b_isDictionary)
                {
                    /* prepare dictionary */
                    Dictionary<int, double?> a_temp = [];

                    /* counter */
                    int i_cnt = 0;

                    for (int i = 0; i < p_a_dataList.Count; i++)
                    {
                        /* set map value */
                        if (p_a_dataList[i].Length < 1)
                        {
                            /* we have an array element with no data, so we add a null */
                            a_temp.Add(i_cnt++, null);
                        }
                        else
                        {
                            a_temp.Add(i_cnt++, BitConverter.Int64BitsToDouble(ForestNET.Lib.Helper.ByteArrayToLong(p_a_dataList[i])));
                        }
                    }

                    /* give dictionary to object */
                    o_foo = a_temp;
                }

                /* set field/property generic value */
                Marshall.SetValue(ref p_o_return, p_o_memberInfo, o_foo, p_b_useProperties);
            }
            else if (p_o_genericType == typeof(short))
            {
                if (b_isList)
                {
                    /* prepare list */
                    List<short?> a_temp = [];

                    for (int i = 0; i < p_a_dataList.Count; i++)
                    {
                        /* set list value */
                        if (p_a_dataList[i].Length < 1)
                        {
                            /* we have an array element with no data, so we add a null */
                            a_temp.Add(null);
                        }
                        else
                        {
                            a_temp.Add(ForestNET.Lib.Helper.ByteArrayToShort(p_a_dataList[i]));
                        }
                    }

                    /* give list to object */
                    o_foo = a_temp;
                }
                else if (b_isDictionary)
                {
                    /* prepare dictionary */
                    Dictionary<int, short?> a_temp = [];

                    /* counter */
                    int i_cnt = 0;

                    for (int i = 0; i < p_a_dataList.Count; i++)
                    {
                        /* set map value */
                        if (p_a_dataList[i].Length < 1)
                        {
                            /* we have an array element with no data, so we add a null */
                            a_temp.Add(i_cnt++, null);
                        }
                        else
                        {
                            a_temp.Add(i_cnt++, ForestNET.Lib.Helper.ByteArrayToShort(p_a_dataList[i]));
                        }
                    }

                    /* give dictionary to object */
                    o_foo = a_temp;
                }

                /* set field/property generic value */
                Marshall.SetValue(ref p_o_return, p_o_memberInfo, o_foo, p_b_useProperties);
            }
            else if (p_o_genericType == typeof(ushort))
            {
                if (b_isList)
                {
                    /* prepare list */
                    List<ushort?> a_temp = [];

                    for (int i = 0; i < p_a_dataList.Count; i++)
                    {
                        /* set list value */
                        if (p_a_dataList[i].Length < 1)
                        {
                            /* we have an array element with no data, so we add a null */
                            a_temp.Add(null);
                        }
                        else
                        {
                            a_temp.Add((ushort)ForestNET.Lib.Helper.ByteArrayToShort(p_a_dataList[i]));
                        }
                    }

                    /* give list to object */
                    o_foo = a_temp;
                }
                else if (b_isDictionary)
                {
                    /* prepare dictionary */
                    Dictionary<int, ushort?> a_temp = [];

                    /* counter */
                    int i_cnt = 0;

                    for (int i = 0; i < p_a_dataList.Count; i++)
                    {
                        /* set map value */
                        if (p_a_dataList[i].Length < 1)
                        {
                            /* we have an array element with no data, so we add a null */
                            a_temp.Add(i_cnt++, null);
                        }
                        else
                        {
                            a_temp.Add(i_cnt++, (ushort)ForestNET.Lib.Helper.ByteArrayToShort(p_a_dataList[i]));
                        }
                    }

                    /* give dictionary to object */
                    o_foo = a_temp;
                }

                /* set field/property generic value */
                Marshall.SetValue(ref p_o_return, p_o_memberInfo, o_foo, p_b_useProperties);
            }
            else if (p_o_genericType == typeof(int))
            {
                if (b_isList)
                {
                    /* prepare list */
                    List<int?> a_temp = [];

                    for (int i = 0; i < p_a_dataList.Count; i++)
                    {
                        /* set list value */
                        if (p_a_dataList[i].Length < 1)
                        {
                            /* we have an array element with no data, so we add a null */
                            a_temp.Add(null);
                        }
                        else
                        {
                            a_temp.Add(ForestNET.Lib.Helper.ByteArrayToInt(p_a_dataList[i]));
                        }
                    }

                    /* give list to object */
                    o_foo = a_temp;
                }
                else if (b_isDictionary)
                {
                    /* prepare dictionary */
                    Dictionary<int, int?> a_temp = [];

                    /* counter */
                    int i_cnt = 0;

                    for (int i = 0; i < p_a_dataList.Count; i++)
                    {
                        /* set map value */
                        if (p_a_dataList[i].Length < 1)
                        {
                            /* we have an array element with no data, so we add a null */
                            a_temp.Add(i_cnt++, null);
                        }
                        else
                        {
                            a_temp.Add(i_cnt++, ForestNET.Lib.Helper.ByteArrayToInt(p_a_dataList[i]));
                        }
                    }

                    /* give dictionary to object */
                    o_foo = a_temp;
                }

                /* set field/property generic value */
                Marshall.SetValue(ref p_o_return, p_o_memberInfo, o_foo, p_b_useProperties);
            }
            else if (p_o_genericType == typeof(uint))
            {
                if (b_isList)
                {
                    /* prepare list */
                    List<uint?> a_temp = [];

                    for (int i = 0; i < p_a_dataList.Count; i++)
                    {
                        /* set list value */
                        if (p_a_dataList[i].Length < 1)
                        {
                            /* we have an array element with no data, so we add a null */
                            a_temp.Add(null);
                        }
                        else
                        {
                            a_temp.Add((uint)ForestNET.Lib.Helper.ByteArrayToInt(p_a_dataList[i]));
                        }
                    }

                    /* give list to object */
                    o_foo = a_temp;
                }
                else if (b_isDictionary)
                {
                    /* prepare dictionary */
                    Dictionary<int, uint?> a_temp = [];

                    /* counter */
                    int i_cnt = 0;

                    for (int i = 0; i < p_a_dataList.Count; i++)
                    {
                        /* set map value */
                        if (p_a_dataList[i].Length < 1)
                        {
                            /* we have an array element with no data, so we add a null */
                            a_temp.Add(i_cnt++, null);
                        }
                        else
                        {
                            a_temp.Add(i_cnt++, (uint)ForestNET.Lib.Helper.ByteArrayToInt(p_a_dataList[i]));
                        }
                    }

                    /* give dictionary to object */
                    o_foo = a_temp;
                }

                /* set field/property generic value */
                Marshall.SetValue(ref p_o_return, p_o_memberInfo, o_foo, p_b_useProperties);
            }
            else if (p_o_genericType == typeof(long))
            {
                if (b_isList)
                {
                    /* prepare list */
                    List<long?> a_temp = [];

                    for (int i = 0; i < p_a_dataList.Count; i++)
                    {
                        /* set list value */
                        if (p_a_dataList[i].Length < 1)
                        {
                            /* we have an array element with no data, so we add a null */
                            a_temp.Add(null);
                        }
                        else
                        {
                            a_temp.Add(ForestNET.Lib.Helper.ByteArrayToLong(p_a_dataList[i]));
                        }
                    }

                    /* give list to object */
                    o_foo = a_temp;
                }
                else if (b_isDictionary)
                {
                    /* prepare dictionary */
                    Dictionary<int, long?> a_temp = [];

                    /* counter */
                    int i_cnt = 0;

                    for (int i = 0; i < p_a_dataList.Count; i++)
                    {
                        /* set map value */
                        if (p_a_dataList[i].Length < 1)
                        {
                            /* we have an array element with no data, so we add a null */
                            a_temp.Add(i_cnt++, null);
                        }
                        else
                        {
                            a_temp.Add(i_cnt++, ForestNET.Lib.Helper.ByteArrayToLong(p_a_dataList[i]));
                        }
                    }

                    /* give dictionary to object */
                    o_foo = a_temp;
                }

                /* set field/property generic value */
                Marshall.SetValue(ref p_o_return, p_o_memberInfo, o_foo, p_b_useProperties);
            }
            else if (p_o_genericType == typeof(ulong))
            {
                if (b_isList)
                {
                    /* prepare list */
                    List<ulong?> a_temp = [];

                    for (int i = 0; i < p_a_dataList.Count; i++)
                    {
                        /* set list value */
                        if (p_a_dataList[i].Length < 1)
                        {
                            /* we have an array element with no data, so we add a null */
                            a_temp.Add(null);
                        }
                        else
                        {
                            a_temp.Add((ulong)ForestNET.Lib.Helper.ByteArrayToLong(p_a_dataList[i]));
                        }
                    }

                    /* give list to object */
                    o_foo = a_temp;
                }
                else if (b_isDictionary)
                {
                    /* prepare dictionary */
                    Dictionary<int, ulong?> a_temp = [];

                    /* counter */
                    int i_cnt = 0;

                    for (int i = 0; i < p_a_dataList.Count; i++)
                    {
                        /* set map value */
                        if (p_a_dataList[i].Length < 1)
                        {
                            /* we have an array element with no data, so we add a null */
                            a_temp.Add(i_cnt++, null);
                        }
                        else
                        {
                            a_temp.Add(i_cnt++, (ulong)ForestNET.Lib.Helper.ByteArrayToLong(p_a_dataList[i]));
                        }
                    }

                    /* give dictionary to object */
                    o_foo = a_temp;
                }

                /* set field/property generic value */
                Marshall.SetValue(ref p_o_return, p_o_memberInfo, o_foo, p_b_useProperties);
            }
            else if (p_o_genericType == typeof(string))
            {
                if (b_isList)
                {
                    /* prepare list */
                    List<string?> a_temp = [];

                    for (int i = 0; i < p_a_dataList.Count; i++)
                    {
                        /* set list value */
                        if (p_a_dataList[i].Length < 1)
                        {
                            /* we have an array element with no data, so we add a null */
                            a_temp.Add(null);
                        }
                        else
                        {
                            a_temp.Add(System.Text.Encoding.UTF8.GetString(p_a_dataList[i]));
                        }
                    }

                    /* give list to object */
                    o_foo = a_temp;
                }
                else if (b_isDictionary)
                {
                    /* prepare dictionary */
                    Dictionary<int, string?> a_temp = [];

                    /* counter */
                    int i_cnt = 0;

                    for (int i = 0; i < p_a_dataList.Count; i++)
                    {
                        /* set map value */
                        if (p_a_dataList[i].Length < 1)
                        {
                            /* we have an array element with no data, so we add a null */
                            a_temp.Add(i_cnt++, null);
                        }
                        else
                        {
                            a_temp.Add(i_cnt++, System.Text.Encoding.UTF8.GetString(p_a_dataList[i]));
                        }
                    }

                    /* give dictionary to object */
                    o_foo = a_temp;
                }

                /* set field/property generic value */
                Marshall.SetValue(ref p_o_return, p_o_memberInfo, o_foo, p_b_useProperties);
            }
            else if ((p_o_genericType == typeof(DateTime)) || (p_o_genericType == typeof(DateTime?)))
            {
                if (b_isList)
                {
                    /* prepare list */
                    List<DateTime?> a_temp = [];

                    for (int i = 0; i < p_a_dataList.Count; i++)
                    {
                        /* set list value */
                        if (p_a_dataList[i].Length < 1)
                        {
                            /* we have an array element with no data, so we add a null */
                            a_temp.Add(null);
                        }
                        else
                        {
                            a_temp.Add(ForestNET.Lib.Helper.FromISO8601UTC(System.Text.Encoding.UTF8.GetString(p_a_dataList[i])));
                        }
                    }

                    /* give list to object */
                    o_foo = a_temp;
                }
                else if (b_isDictionary)
                {
                    /* prepare dictionary */
                    Dictionary<int, DateTime?> a_temp = [];

                    /* counter */
                    int i_cnt = 0;

                    for (int i = 0; i < p_a_dataList.Count; i++)
                    {
                        /* set map value */
                        if (p_a_dataList[i].Length < 1)
                        {
                            /* we have an array element with no data, so we add a null */
                            a_temp.Add(i_cnt++, null);
                        }
                        else
                        {
                            a_temp.Add(i_cnt++, ForestNET.Lib.Helper.FromISO8601UTC(System.Text.Encoding.UTF8.GetString(p_a_dataList[i])));
                        }
                    }

                    /* give dictionary to object */
                    o_foo = a_temp;
                }

                /* set field/property generic value */
                Marshall.SetValue(ref p_o_return, p_o_memberInfo, o_foo, p_b_useProperties);
            }
            else if (p_o_genericType == typeof(decimal))
            {
                if (b_isList)
                {
                    /* prepare list */
                    List<decimal?> a_temp = [];

                    for (int i = 0; i < p_a_dataList.Count; i++)
                    {
                        /* set list value */
                        if (p_a_dataList[i].Length < 1)
                        {
                            /* we have an array element with no data, so we add a null */
                            a_temp.Add(null);
                        }
                        else
                        {
                            a_temp.Add((decimal)(ForestNET.Lib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimal(System.Text.Encoding.UTF8.GetString(p_a_dataList[i]), 14) ?? 0.0m));
                        }
                    }

                    /* give list to object */
                    o_foo = a_temp;
                }
                else if (b_isDictionary)
                {
                    /* prepare dictionary */
                    Dictionary<int, decimal?> a_temp = [];

                    /* counter */
                    int i_cnt = 0;

                    for (int i = 0; i < p_a_dataList.Count; i++)
                    {
                        /* set map value */
                        if (p_a_dataList[i].Length < 1)
                        {
                            /* we have an array element with no data, so we add a null */
                            a_temp.Add(i_cnt++, null);
                        }
                        else
                        {
                            a_temp.Add(i_cnt++, (decimal)(ForestNET.Lib.IO.StandardTransposeMethods.FloatingPointNumbers.TransposeDecimal(System.Text.Encoding.UTF8.GetString(p_a_dataList[i]), 14) ?? 0.0m));
                        }
                    }

                    /* give dictionary to object */
                    o_foo = a_temp;
                }

                /* set field/property generic value */
                Marshall.SetValue(ref p_o_return, p_o_memberInfo, o_foo, p_b_useProperties);
            }
            else
            {
                /* class type of generic list/dictionary not supported for unmarshalling, so we will not set this field/property */
                ForestNET.Lib.Global.ILogWarning("class type of generic list/dictionary not supported for unmarshalling, so we will not set this field/property '" + p_o_memberInfo.Name + "'");
            }
        }
    }
}
