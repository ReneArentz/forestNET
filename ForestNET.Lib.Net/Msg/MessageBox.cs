namespace ForestNET.Lib.Net.Msg
{
    /// <summary>
    /// Network message box which can hold a huge number of messages. Functionality corresponds to that of a queue with reentrant lock.
    /// A complete network message or an entire object can be enqueued/dequeued to the message box.
    /// All data will be serialized into utf8 bytes, so objects need to support System.Text.Json.JsonSerializer.SerializeToUtf8Bytes. Alternatively a forestNET implementation of marshalling can be used.
    /// </summary>
    public class MessageBox
    {

        /* Fields */

        private readonly Object o_lock = new();
        private readonly System.Collections.Generic.Queue<Message> o_messageQueue;

        /* Properties */

        public int MessageBoxId { get; private set; }
        public int MessageLength { get; private set; }
        public int Limit { get; private set; }
        public int TimeoutMilliseconds { get; set; } = 1000;
        public bool IsLocked { get; private set; }
        public int MessageAmount
        {
            get
            {
                if (this.o_messageQueue != null)
                {
                    return this.o_messageQueue.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

        /* Methods */

        /// <summary>
        /// Message box constructor, no limit
        /// </summary>
        /// <param name="p_i_messageBoxId">id integer value for message box</param>
        /// <param name="p_i_messageLength">message length integer value</param>
        /// <exception cref="ArgumentException">invalid parameter value for message box id, message length</exception>
        public MessageBox(int p_i_messageBoxId, int p_i_messageLength) :
            this(p_i_messageBoxId, p_i_messageLength, 0)
        {

        }

        /// <summary>
        /// Message box constructor
        /// </summary>
        /// <param name="p_i_messageBoxId">id integer value for message box</param>
        /// <param name="p_i_messageLength">message length integer value</param>
        /// <param name="p_i_limit">limit amount for messages within message box</param>
        /// <exception cref="ArgumentException">invalid parameter value for message box id, message length or limit value</exception>
        public MessageBox(int p_i_messageBoxId, int p_i_messageLength, int p_i_limit)
        {
            /* check message box id parameter */
            if (p_i_messageBoxId < 1)
            {
                throw new ArgumentException("Message box id paramter[" + p_i_messageBoxId + "] must be a positive integer");
            }

            /* check message length parameter */
            if (!Message.MSGLENGTHS.Contains(p_i_messageLength))
            {
                throw new ArgumentException("Invalid message length[" + p_i_messageLength + "] parameter. Valid values are [" + Message.MSGLENGTHS_STR + "]");
            }

            /* check limit parameter, zero for unlimited */
            if (p_i_limit < 0)
            {
                throw new ArgumentException("Limit paramter[" + p_i_limit + "] must be a positive integer or zero");
            }

            this.MessageBoxId = p_i_messageBoxId;
            this.MessageLength = p_i_messageLength;
            this.Limit = p_i_limit;

            /* initialize queue */
            this.o_messageQueue = new();
        }

        /// <summary>
        /// Enqueue object to message box queue
        /// </summary>
        /// <param name="p_o_object">object parameter, must support System.Text.Json.JsonSerializer.SerializeToUtf8Bytes</param>
        /// <returns>true - object enqueued as message, false - limit of message queue reached or exception occurred</returns>
        /// <exception cref="NotSupportedException">object parameter could not be serialized with System.Text.Json.JsonSerializer.SerializeToUtf8Bytes</exception>
        public bool EnqueueObject(Object p_o_object)
        {
            /* check message box limit */
            if ((this.Limit > 0) && (this.o_messageQueue.Count >= this.Limit))
            {
                ForestNET.Lib.Global.ILogFinest("message box limit reached: " + this.Limit);

                return false;
            }

            /* variable for return value */
            bool b_return = true;

            /* atomic lock */
            bool b_atomicLock = false;

            /* try enqueue object with monitor lock */
            try
            {
                /* try lock message box */
                Monitor.TryEnter(this.o_lock, this.TimeoutMilliseconds, ref b_atomicLock);

                /* we locked message box */
                if (b_atomicLock)
                {
                    this.IsLocked = true;

                    ForestNET.Lib.Global.ILogFinest("message box locked by '" + Environment.CurrentManagedThreadId + "' for EnqueueObject");

                    /* read object data and serialize it to utf8 byte array */
                    byte[] a_bytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(p_o_object);

                    /* get data block length for network message */
                    int i_dataBlockLength = Message.CalculateDataBlockLength(this.MessageLength);

                    /* calculate how many messages we need to transport object with network messages */
                    int i_messages = 1 + (a_bytes.Length / i_dataBlockLength);

                    /* check if amount of messages for object will not exceed message box limit */
                    if ((this.Limit > 0) && ((this.o_messageQueue.Count + i_messages) >= this.Limit))
                    {
                        ForestNET.Lib.Global.ILogFinest("message box limit '" + this.Limit + "' reached with '" + i_messages + "' message(s) for object");

                        b_return = false;
                    }
                    else
                    {
                        /* split object data into several messages */
                        for (int i = 0; i < i_messages; i++)
                        {
                            /* create network message object with message length property of message box */
                            Message o_message = new(this.MessageLength)
                            {
                                /* set message information, like message box id, amount of messages, message number and type */
                                MessageBoxId = this.MessageBoxId,
                                MessageAmount = i_messages,
                                MessageNumber = (i + 1),
                                Type = p_o_object.GetType().AssemblyQualifiedName ?? "type AssemblyQualifiedName is null"
                            };

                            /* create byte array for part of object data */
                            byte[] a_data = new byte[i_dataBlockLength];
                            int j = 0;

                            /* iterate all data bytes until we reached message data block length */
                            for (j = 0; j < i_dataBlockLength; j++)
                            {
                                /* reached last byte? -> break */
                                if (j + (i * i_dataBlockLength) >= a_bytes.Length)
                                {
                                    break;
                                }

                                /* copy data byte to byte array */
                                a_data[j] = a_bytes[j + (i * i_dataBlockLength)];
                            }

                            /* give byte array as data part to network message object */
                            o_message.Data = a_data;

                            /* part of object data may not need complete data block length, especially the last message or just one message, so this is not obsolete because the for loop before could be break before j == i_dataBlockLength - 1 */
                            if (j != i_dataBlockLength - 1)
                            {
                                /* update message data length */
                                o_message.DataLength = j;
                            }

                            /* add network message to message box queue */
                            this.o_messageQueue.Enqueue(o_message);
                        }
                    }
                }
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.ILogWarning("could not enqueue object: " + o_exc);

                b_return = false;
            }
            finally
            {
                /* we locked message box */
                if (b_atomicLock)
                {
                    ForestNET.Lib.Global.ILogFinest("message box unlocked by '" + Environment.CurrentManagedThreadId + "' for EnqueueObject");

                    /* unlock message box */
                    Monitor.Exit(this.o_lock);
                    this.IsLocked = false;
                    b_atomicLock = false;
                }
                else
                {
                    /* this thread did not locked message box and so could not enqueue an object */
                    b_return = false;
                }
            }

            return b_return;
        }

        /// <summary>
        /// Enqueue and marshall object with all fields of primitive types or supported types to message box queue. Transfering data as big endian. Handle data as big endian. Do not use properties to retrieve values. 1 byte is used used to marshall the length of data.
        /// </summary>
        /// <param name="p_o_object">object parameter</param>
        /// <returns>true - object enqueued as message, false - limit of message queue reached or exception occurred</returns>
        /// <exception cref="ArgumentNullException">parameter object is null</exception>
        /// <exception cref="ArgumentException">data length in bytes must be between 1..4</exception>
        /// <exception cref="NotSupportedException">little endian system data is NOT IMPLEMENTED</exception>
        /// <exception cref="MissingMemberException">could not retrieve member type by object member</exception>
        /// <exception cref="System.Reflection.TargetException">could not retrieve value from member instancec</exception>
        /// <exception cref="MemberAccessException">could not access value from meber, access violation</exception>
        public bool EnqueueObjectWithMarshalling(Object p_o_object)
        {
            return this.EnqueueObjectWithMarshalling(p_o_object, 1);
        }

        /// <summary>
        /// Enqueue and marshall object with all fields of primitive types or supported types to message box queue. Transfering data as big endian. Handle data as big endian. Do not use properties to retrieve values.
        /// </summary>
        /// <param name="p_o_object">object parameter</param>
        /// <param name="p_i_dataLengthInBytes">define how many bytes are used to marshall the length of data</param>
        /// <returns>true - object enqueued as message, false - limit of message queue reached or exception occurred</returns>
        /// <exception cref="ArgumentNullException">parameter object is null</exception>
        /// <exception cref="ArgumentException">data length in bytes must be between 1..4</exception>
        /// <exception cref="NotSupportedException">little endian system data is NOT IMPLEMENTED</exception>
        /// <exception cref="MissingMemberException">could not retrieve member type by object member</exception>
        /// <exception cref="System.Reflection.TargetException">could not retrieve value from member instancec</exception>
        /// <exception cref="MemberAccessException">could not access value from meber, access violation</exception>
        public bool EnqueueObjectWithMarshalling(Object p_o_object, int p_i_dataLengthInBytes)
        {
            return this.EnqueueObjectWithMarshalling(p_o_object, p_i_dataLengthInBytes, false);
        }

        /// <summary>
        /// Enqueue and marshall object with all fields of primitive types or supported types to message box queue. Transfering data as big endian. Handle data as big endian.
        /// </summary>
        /// <param name="p_o_object">object parameter</param>
        /// <param name="p_i_dataLengthInBytes">define how many bytes are used to marshall the length of data</param>
        /// <param name="p_b_useProperties">access object parameter fields via properties</param>
        /// <returns>true - object enqueued as message, false - limit of message queue reached or exception occurred</returns>
        /// <exception cref="ArgumentNullException">parameter object is null</exception>
        /// <exception cref="ArgumentException">data length in bytes must be between 1..4</exception>
        /// <exception cref="NotSupportedException">little endian system data is NOT IMPLEMENTED</exception>
        /// <exception cref="MissingMemberException">could not retrieve member type by object member</exception>
        /// <exception cref="System.Reflection.TargetException">could not retrieve value from member instancec</exception>
        /// <exception cref="MemberAccessException">could not access value from meber, access violation</exception>
        public bool EnqueueObjectWithMarshalling(Object p_o_object, int p_i_dataLengthInBytes, bool p_b_useProperties)
        {
            return this.EnqueueObjectWithMarshalling(p_o_object, p_i_dataLengthInBytes, p_b_useProperties, null);
        }

        /// <summary>
        /// Enqueue and marshall object with all fields of primitive types or supported types to message box queue. Transfering data as big endian. Handle data as big endian.
        /// </summary>
        /// <param name="p_o_object">object parameter</param>
        /// <param name="p_i_dataLengthInBytes">define how many bytes are used to marshall the length of data</param>
        /// <param name="p_b_useProperties">access object parameter fields via properties</param>
        /// <param name="p_s_overrideMessageType">override message type with this string and do not get it automatically from object, thus the type can be set generally from other systems with other programming languages</param>
        /// <returns>true - object enqueued as message, false - limit of message queue reached or exception occurred</returns>
        /// <exception cref="ArgumentNullException">parameter object is null</exception>
        /// <exception cref="ArgumentException">data length in bytes must be between 1..4</exception>
        /// <exception cref="NotSupportedException">little endian system data is NOT IMPLEMENTED</exception>
        /// <exception cref="MissingMemberException">could not retrieve member type by object member</exception>
        /// <exception cref="System.Reflection.TargetException">could not retrieve value from member instancec</exception>
        /// <exception cref="MemberAccessException">could not access value from meber, access violation</exception>
        public bool EnqueueObjectWithMarshalling(Object p_o_object, int p_i_dataLengthInBytes, bool p_b_useProperties, string? p_s_overrideMessageType)
        {
            return this.EnqueueObjectWithMarshalling(p_o_object, p_i_dataLengthInBytes, p_b_useProperties, p_s_overrideMessageType, false);
        }

        /// <summary>
        /// Enqueue and marshall object with all fields of primitive types or supported types to message box queue. Transfering data as big endian.
        /// </summary>
        /// <param name="p_o_object">object parameter</param>
        /// <param name="p_i_dataLengthInBytes">define how many bytes are used to marshall the length of data</param>
        /// <param name="p_b_useProperties">access object parameter fields via properties</param>
        /// <param name="p_s_overrideMessageType">override message type with this string and do not get it automatically from object, thus the type can be set generally from other systems with other programming languages</param>
        /// <param name="p_b_systemUsesLittleEndian">true - current execution system uses little endian, false - current execution system uses big endian</param>
        /// <returns>true - object enqueued as message, false - limit of message queue reached or exception occurred</returns>
        /// <exception cref="ArgumentNullException">parameter object is null</exception>
        /// <exception cref="ArgumentException">data length in bytes must be between 1..4</exception>
        /// <exception cref="NotSupportedException">little endian system data is NOT IMPLEMENTED</exception>
        /// <exception cref="MissingMemberException">could not retrieve member type by object member</exception>
        /// <exception cref="System.Reflection.TargetException">could not retrieve value from member instancec</exception>
        /// <exception cref="MemberAccessException">could not access value from meber, access violation</exception>
        public bool EnqueueObjectWithMarshalling(Object p_o_object, int p_i_dataLengthInBytes, bool p_b_useProperties, string? p_s_overrideMessageType, bool p_b_systemUsesLittleEndian)
        {
            /* check message box limit */
            if ((this.Limit > 0) && (this.o_messageQueue.Count >= this.Limit))
            {
                ForestNET.Lib.Global.ILogFinest("message box limit reached: " + this.Limit);

                return false;
            }

            /* variable for return value */
            bool b_return = true;

            /* atomic lock */
            bool b_atomicLock = false;

            /* try enqueue object with marshalling and monitor lock */
            try
            {
                /* try lock message box */
                Monitor.TryEnter(this.o_lock, this.TimeoutMilliseconds, ref b_atomicLock);

                /* we locked message box */
                if (b_atomicLock)
                {
                    this.IsLocked = true;

                    ForestNET.Lib.Global.ILogFinest("message box locked by '" + Environment.CurrentManagedThreadId + "' for EnqueueObjectWithMarshalling");

                    /* call marshall object method */
                    byte[] a_bytes = Marshall.MarshallObject(p_o_object, p_i_dataLengthInBytes, p_b_useProperties, p_b_systemUsesLittleEndian);

                    ForestNET.Lib.Global.ILogFiner("marshalled object to " + a_bytes.Length + " bytes");

                    /* get data block length for network message */
                    int i_dataBlockLength = Message.CalculateDataBlockLength(this.MessageLength);

                    /* calculate how many messages we need to transport object with network messages */
                    int i_messages = 1 + (a_bytes.Length / i_dataBlockLength);

                    /* check if amount of messages for object will not exceed message box limit */
                    if ((this.Limit > 0) && ((this.o_messageQueue.Count + i_messages) >= this.Limit))
                    {
                        ForestNET.Lib.Global.ILogFinest("message box limit '" + this.Limit + "' reached with '" + i_messages + "' message(s) for object");

                        b_return = false;
                    }
                    else
                    {
                        /* split object data into several messages */
                        for (int i = 0; i < i_messages; i++)
                        {
                            /* create network message object with message length property of message box */
                            Message o_message = new(this.MessageLength)
                            {
                                /* set message information, like message box id, amount of messages, message number */
                                MessageBoxId = this.MessageBoxId,
                                MessageAmount = i_messages,
                                MessageNumber = (i + 1),
                            };

                            /* get universal primitive type name if available */
                            if (p_o_object == null)
                            {
                                o_message.Type = "null";
                            }
                            else if (p_o_object.GetType() == typeof(bool))
                            {
                                o_message.Type = "bool";
                            }
                            else if (p_o_object.GetType() == typeof(bool[]))
                            {
                                o_message.Type = "bool[]";
                            }
                            else if (p_o_object.GetType() == typeof(byte))
                            {
                                o_message.Type = "byte";
                            }
                            else if (p_o_object.GetType() == typeof(byte[]))
                            {
                                o_message.Type = "byte[]";
                            }
                            else if (p_o_object.GetType() == typeof(sbyte))
                            {
                                o_message.Type = "sbyte";
                            }
                            else if (p_o_object.GetType() == typeof(sbyte[]))
                            {
                                o_message.Type = "sbyte[]";
                            }
                            else if (p_o_object.GetType() == typeof(char))
                            {
                                o_message.Type = "char";
                            }
                            else if (p_o_object.GetType() == typeof(char[]))
                            {
                                o_message.Type = "char[]";
                            }
                            else if (p_o_object.GetType() == typeof(float))
                            {
                                o_message.Type = "float";
                            }
                            else if (p_o_object.GetType() == typeof(float[]))
                            {
                                o_message.Type = "float[]";
                            }
                            else if (p_o_object.GetType() == typeof(double))
                            {
                                o_message.Type = "double";
                            }
                            else if (p_o_object.GetType() == typeof(double[]))
                            {
                                o_message.Type = "double[]";
                            }
                            else if (p_o_object.GetType() == typeof(short))
                            {
                                o_message.Type = "short";
                            }
                            else if (p_o_object.GetType() == typeof(short[]))
                            {
                                o_message.Type = "short[]";
                            }
                            else if (p_o_object.GetType() == typeof(ushort))
                            {
                                o_message.Type = "ushort";
                            }
                            else if (p_o_object.GetType() == typeof(ushort[]))
                            {
                                o_message.Type = "ushort[]";
                            }
                            else if (p_o_object.GetType() == typeof(int))
                            {
                                o_message.Type = "int";
                            }
                            else if (p_o_object.GetType() == typeof(int[]))
                            {
                                o_message.Type = "int[]";
                            }
                            else if (p_o_object.GetType() == typeof(uint))
                            {
                                o_message.Type = "uint";
                            }
                            else if (p_o_object.GetType() == typeof(uint[]))
                            {
                                o_message.Type = "uint[]";
                            }
                            else if (p_o_object.GetType() == typeof(long))
                            {
                                o_message.Type = "long";
                            }
                            else if (p_o_object.GetType() == typeof(long[]))
                            {
                                o_message.Type = "long[]";
                            }
                            else if (p_o_object.GetType() == typeof(ulong))
                            {
                                o_message.Type = "ulong";
                            }
                            else if (p_o_object.GetType() == typeof(ulong[]))
                            {
                                o_message.Type = "ulong[]";
                            }
                            else if (p_o_object.GetType() == typeof(string))
                            {
                                o_message.Type = "string";
                            }
                            else if ((p_o_object.GetType() == typeof(string[])) || (p_o_object.GetType() == typeof(string?[])))
                            {
                                o_message.Type = "string[]";
                            }
                            else if ((p_o_object.GetType() == typeof(DateTime)) || (p_o_object.GetType() == typeof(DateTime?)))
                            {
                                o_message.Type = "DateTime";
                            }
                            else if ((p_o_object.GetType() == typeof(DateTime[])) || (p_o_object.GetType() == typeof(DateTime?[])))
                            {
                                o_message.Type = "DateTime[]";
                            }
                            else if (p_o_object.GetType() == typeof(decimal))
                            {
                                o_message.Type = "decimal";
                            }
                            else if (p_o_object.GetType() == typeof(decimal[]))
                            {
                                o_message.Type = "decimal[]";
                            }
                            else
                            {
                                if (p_s_overrideMessageType != null)
                                {
                                    /* override message type with parameter, thus the type can be set generally from other systems with other programming languages */
                                    o_message.Type = p_s_overrideMessageType;
                                }
                                else
                                {
                                    /* no primitive type -> set assembly qualified name */
                                    o_message.Type = p_o_object.GetType().AssemblyQualifiedName ?? "type AssemblyQualifiedName is null";
                                }
                            }

                            /* create byte array for part of object data */
                            byte[] a_data = new byte[i_dataBlockLength];
                            int j = 0;

                            /* iterate all data bytes until we reached message data block length */
                            for (j = 0; j < i_dataBlockLength; j++)
                            {
                                /* reached last byte? -> break */
                                if (j + (i * i_dataBlockLength) >= a_bytes.Length)
                                {
                                    break;
                                }

                                /* copy data byte to byte array */
                                a_data[j] = a_bytes[j + (i * i_dataBlockLength)];
                            }

                            /* give byte array as data part to network message object */
                            o_message.Data = a_data;

                            /* part of object data may not need complete data block length, especially the last message or just one message, so this is not obsolete because the for loop before could be break before j == i_dataBlockLength - 1 */
                            if (j != i_dataBlockLength - 1)
                            {
                                /* update message data length */
                                o_message.DataLength = j;
                            }

                            /* add network message to message box queue */
                            this.o_messageQueue.Enqueue(o_message);
                        }
                    }
                }
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.ILogWarning("could not enqueue object: " + o_exc);

                b_return = false;
            }
            finally
            {
                /* we locked message box */
                if (b_atomicLock)
                {
                    ForestNET.Lib.Global.ILogFinest("message box unlocked by '" + Environment.CurrentManagedThreadId + "' for EnqueueObjectWithMarshalling");

                    /* unlock message box */
                    Monitor.Exit(this.o_lock);
                    this.IsLocked = false;
                    b_atomicLock = false;
                }
                else
                {
                    /* this thread did not locked message box and so could not enqueue an object */
                    b_return = false;
                }
            }

            return b_return;
        }

        /// <summary>
        /// Enqueue message object to message box queue
        /// </summary>
        /// <param name="p_o_message">network message object</param>
        /// <returns>true - message enqueued, false - limit of message queue reached or exception occurred</returns>
        public bool EnqueueMessage(Message p_o_message)
        {
            /* check message box limit */
            if ((this.Limit > 0) && (this.o_messageQueue.Count >= this.Limit))
            {
                ForestNET.Lib.Global.ILogFinest("message box limit reached: " + this.Limit);

                return false;
            }

            /* message object length does not correspond to message box message length */
            if (p_o_message.MessageLength != this.MessageLength)
            {
                ForestNET.Lib.Global.ILogFinest("message object length '" + p_o_message.MessageLength + "' does not correspond to message box message length '" + this.MessageLength + "'");

                return false;
            }

            /* variable for return value */
            bool b_return = true;

            /* atomic lock */
            bool b_atomicLock = false;

            /* try enqueue message with monitor lock */
            try
            {
                /* try lock message box */
                Monitor.TryEnter(this.o_lock, this.TimeoutMilliseconds, ref b_atomicLock);

                /* we locked message box */
                if (b_atomicLock)
                {
                    this.IsLocked = true;

                    ForestNET.Lib.Global.ILogFinest("message box locked by '" + Environment.CurrentManagedThreadId + "' for EnqueueObjectWithMarshalling");

                    /* add network message to message box queue */
                    this.o_messageQueue.Enqueue(p_o_message);
                }
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.ILogWarning("could not enqueue message: " + o_exc);

                b_return = false;
            }
            finally
            {
                /* we locked message box */
                if (b_atomicLock)
                {
                    ForestNET.Lib.Global.ILogFinest("message box unlocked by '" + Environment.CurrentManagedThreadId + "' for EnqueueMessage");

                    /* unlock message box */
                    Monitor.Exit(this.o_lock);
                    this.IsLocked = false;
                    b_atomicLock = false;
                }
                else
                {
                    /* this thread did not locked message box and so could not enqueue an object */
                    b_return = false;
                }
            }

            return b_return;
        }

        /// <summary>
        /// Retrieves, but does not remove, the head message of message box queue
        /// </summary>
        /// <returns>network message object</returns>
        public Message? CurrentMessage()
        {
            Message? o_return = null;

            /* check if message box queue is not empty */
            if (this.o_messageQueue.Count <= 0)
            {
                return o_return;
            }

            /* atomic lock */
            bool b_atomicLock = false;

            /* try peek message with monitor lock */
            try
            {
                /* try lock message box */
                Monitor.TryEnter(this.o_lock, this.TimeoutMilliseconds, ref b_atomicLock);

                /* we locked message box */
                if (b_atomicLock)
                {
                    this.IsLocked = true;

                    ForestNET.Lib.Global.ILogFinest("message box locked by '" + Environment.CurrentManagedThreadId + "' for CurrentMessage");

                    /* get head message of message box queue */
                    o_return = this.o_messageQueue.Peek();
                }
            }
            catch (Exception o_exc)
            {
                o_return = null;

                ForestNET.Lib.Global.ILogWarning("could not peek message: " + o_exc);
            }
            finally
            {
                /* we locked message box */
                if (b_atomicLock)
                {
                    ForestNET.Lib.Global.ILogFinest("message box unlocked by '" + Environment.CurrentManagedThreadId + "' for CurrentMessage");

                    /* unlock message box */
                    Monitor.Exit(this.o_lock);
                    this.IsLocked = false;
                    b_atomicLock = false;
                }
                else
                {
                    /* this thread did not locked message box and so could not get current message */
                    o_return = null;
                }
            }

            /* return head message */
            return o_return;
        }

        /// <summary>
        /// Dequeue object from message box queue
        /// </summary>
        /// <returns>object casted to it's original type - null if transmission is not complete, data could not be merged, or original class type cannot be found</returns>
        public Object? DequeueObject()
        {
            /* return value, standard = null */
            Object? o_return = null;

            /* check if message box queue is not empty */
            if (this.o_messageQueue.Count <= 0)
            {
                return o_return;
            }

            /* atomic lock */
            bool b_atomicLock = false;

            /* create variables to dequeue object from message box */
            Message? o_message = null;
            int i_messageDataLength = 0;
            List<byte> a_bytes = [];
            bool b_enoughMessages = true;

            /* try dequeue object with monitor lock */
            try
            {
                /* try lock message box */
                Monitor.TryEnter(this.o_lock, this.TimeoutMilliseconds, ref b_atomicLock);

                /* we locked message box */
                if (b_atomicLock)
                {
                    this.IsLocked = true;

                    ForestNET.Lib.Global.ILogFinest("message box locked by '" + Environment.CurrentManagedThreadId + "' for DequeueObject");

                    /* get head message of message box queue */
                    o_message = this.o_messageQueue.Peek();

                    /* get amount of message we need to merge object together */
                    int i_messageAmount = o_message.MessageAmount;

                    /* check if we have enough messages in our message box queue for merging */
                    if (i_messageAmount > this.o_messageQueue.Count)
                    {
                        ForestNET.Lib.Global.ILogFiner("not enough messages[" + i_messageAmount + " > " + this.o_messageQueue.Count + "], transmission not complete");

                        b_enoughMessages = false;
                    }
                    else
                    {
                        /* dequeue until all messages for object have been retrieved */
                        do
                        {
                            /* dequeue message from message box queue */
                            o_message = this.o_messageQueue.Dequeue();

                            /* calculate overall data length for object */
                            i_messageDataLength += o_message.DataLength;

                            /* gather bytes */
                            for (int i = 0; i < o_message.DataLength; i++)
                            {
                                /* add byte to dynamic byte list */
                                a_bytes.Add(o_message.Data[i]);
                            }
                        } while ((i_messageAmount - o_message.MessageNumber) != 0);
                    }
                }
            }
            catch (Exception o_exc)
            {
                o_return = null;

                ForestNET.Lib.Global.ILogWarning("could not dequeue object: " + o_exc);
            }
            finally
            {
                /* we locked message box */
                if (b_atomicLock)
                {
                    ForestNET.Lib.Global.ILogFinest("message box unlocked by '" + Environment.CurrentManagedThreadId + "' for DequeueObject");

                    /* unlock message box */
                    Monitor.Exit(this.o_lock);
                    this.IsLocked = false;
                    b_atomicLock = false;
                }
                else
                {
                    /* this thread did not locked message box and so could not dequeue object */
                    o_return = null;
                }
            }

            /* not enough messages for merging object together, returning null */
            if (!b_enoughMessages)
            {
                return o_return;
            }

            /* create byte array which holds all object's byte data */
            byte[] a_messageBytes = new byte[i_messageDataLength];
            int i_bytePosition = 0;

            /* copy content bytes from dynamic byte list to byte array */
            foreach (byte by_byte in a_bytes)
            {
                a_messageBytes[i_bytePosition++] = by_byte;
            }

            /* read byte array to object to complete merging object data */
            try
            {
                /* get message type */
                string s_type = o_message?.Type ?? "missing message instance for type";
                /* get origin type of enqueued data */
                Type o_targetType = Type.GetType(s_type) ?? throw new NullReferenceException("Could not retrieve object type with '" + s_type + "'");
                /* deserialize data */
                o_return = System.Text.Json.JsonSerializer.Deserialize(a_messageBytes, o_targetType);
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.ILogWarning("could not read merged object data: " + o_exc);

                o_return = null;
            }

            /* return dequeued object */
            return o_return;
        }

        /// <summary>
        /// Dequeue object from message box queue and unmarshall it's data. Receiving data as big endian. Handle data as big endian. Do not use properties to retrieve field values.
        /// </summary>
        /// <returns>object casted to it's original type - null if transmission is not complete, data could not be merged, or original class type cannot be found</returns>
        public Object? DequeueObjectWithMarshalling()
        {
            return this.DequeueObjectWithMarshalling(false);
        }

        /// <summary>
        /// Dequeue object from message box queue and unmarshall it's data. Receiving data as big endian. Handle data as big endian.
        /// </summary>
        /// <param name="p_b_useProperties">access object parameter fields via properties</param>
        /// <returns>object casted to it's original type - null if transmission is not complete, data could not be merged, or original class type cannot be found</returns>
        public Object? DequeueObjectWithMarshalling(bool p_b_useProperties)
        {
            return this.DequeueObjectWithMarshalling(p_b_useProperties, false);
        }

        /// <summary>
        /// Dequeue object from message box queue and unmarshall it's data. Receiving data as big endian.
        /// </summary>
        /// <param name="p_b_useProperties">access object parameter fields via properties</param>
        /// <param name="p_b_systemUsesLittleEndian">true - current execution system uses little endian, false - current execution system uses big endian</param>
        /// <returns>object casted to it's original type - null if transmission is not complete, data could not be merged, or original class type cannot be found</returns>
        public Object? DequeueObjectWithMarshalling(bool p_b_useProperties, bool p_b_systemUsesLittleEndian)
        {
            /* return value, standard = null */
            Object? o_return = null;

            /* check if message box queue is not empty */
            if (this.o_messageQueue.Count <= 0)
            {
                return o_return;
            }

            /* atomic lock */
            bool b_atomicLock = false;

            /* create variables to dequeue object from message box */
            Message? o_message = null;
            int i_messageDataLength = 0;
            List<byte> a_bytes = [];
            bool b_enoughMessages = true;

            /* try dequeue object with marshalling and monitor lock */
            try
            {
                /* try lock message box */
                Monitor.TryEnter(this.o_lock, this.TimeoutMilliseconds, ref b_atomicLock);

                /* we locked message box */
                if (b_atomicLock)
                {
                    this.IsLocked = true;

                    ForestNET.Lib.Global.ILogFinest("message box locked by '" + Environment.CurrentManagedThreadId + "' for DequeueObjectWithMarshalling");

                    /* get head message of message box queue */
                    o_message = this.o_messageQueue.Peek();

                    /* get amount of message we need to merge object together */
                    int i_messageAmount = o_message.MessageAmount;

                    /* check if we have enough messages in our message box queue for merging */
                    if (i_messageAmount > this.o_messageQueue.Count)
                    {
                        ForestNET.Lib.Global.ILogFiner("not enough messages[" + i_messageAmount + " > " + this.o_messageQueue.Count + "], transmission not complete");

                        b_enoughMessages = false;
                    }
                    else
                    {
                        /* dequeue until all messages for object have been retrieved */
                        do
                        {
                            /* dequeue message from message box queue */
                            o_message = this.o_messageQueue.Dequeue();

                            /* calculate overall data length for object */
                            i_messageDataLength += o_message.DataLength;

                            /* gather bytes */
                            for (int i = 0; i < o_message.DataLength; i++)
                            {
                                /* add byte to dynamic byte list */
                                a_bytes.Add(o_message.Data[i]);
                            }
                        } while ((i_messageAmount - o_message.MessageNumber) != 0);
                    }
                }
            }
            catch (Exception o_exc)
            {
                o_return = null;

                ForestNET.Lib.Global.ILogWarning("could not dequeue object: " + o_exc);
            }
            finally
            {
                /* we locked message box */
                if (b_atomicLock)
                {
                    ForestNET.Lib.Global.ILogFinest("message box unlocked by '" + Environment.CurrentManagedThreadId + "' for DequeueObjectWithMarshalling");

                    /* unlock message box */
                    Monitor.Exit(this.o_lock);
                    this.IsLocked = false;
                    b_atomicLock = false;
                }
                else
                {
                    /* this thread did not locked message box and so could not dequeue object */
                    o_return = null;
                }
            }

            /* not enough messages for merging object together, returning null */
            if (!b_enoughMessages)
            {
                return o_return;
            }

            /* create byte array which holds all object's byte data */
            byte[] a_messageBytes = new byte[i_messageDataLength];
            int i_bytePosition = 0;

            /* copy content bytes from dynamic byte list to byte array */
            foreach (byte by_byte in a_bytes)
            {
                a_messageBytes[i_bytePosition++] = by_byte;
            }

            /* return dequeued unmarshalled object */
            return MessageBox.UnmarshallObjectFromMessage(o_message?.Type ?? "missing message instance for type", a_messageBytes, p_b_useProperties, p_b_systemUsesLittleEndian);
        }

        /// <summary>
        /// Unmarshall object from message instance an it's content byte array.
        /// </summary>
        /// <param name="p_s_type">type string of message instance</param>
        /// <param name="p_a_messageBytes">message content as array of bytes</param>
        /// <param name="p_b_useProperties">access object parameter fields via properties</param>
        /// <param name="p_b_systemUsesLittleEndian">true - current execution system uses little endian, false - current execution system uses big endian</param>
        /// <returns>object casted to it's original type - null if transmission is not complete, data could not be merged, or original class type cannot be found</returns>
        public static Object? UnmarshallObjectFromMessage(string p_s_type, byte[] p_a_messageBytes, bool p_b_useProperties, bool p_b_systemUsesLittleEndian)
        {
            /* return variable */
            Object? o_return = null;

            /* generic class object */
            Type o_targetType;
            string s_type = p_s_type;

            /* get class name and create new instance for return object by string object type value */
            try
            {
                /* map primitive types to class types */
                if (s_type.Equals("null"))
                {
                    return null;
                }
                else if (s_type.Equals("bool"))
                {
                    o_targetType = typeof(bool);
                }
                else if (s_type.Equals("bool[]"))
                {
                    o_targetType = typeof(bool[]);
                }
                else if (s_type.Equals("byte"))
                {
                    o_targetType = typeof(byte);
                }
                else if (s_type.Equals("byte[]"))
                {
                    o_targetType = typeof(byte[]);
                }
                else if (s_type.Equals("sbyte"))
                {
                    o_targetType = typeof(sbyte);
                }
                else if (s_type.Equals("sbyte[]"))
                {
                    o_targetType = typeof(sbyte[]);
                }
                else if (s_type.Equals("char"))
                {
                    o_targetType = typeof(char);
                }
                else if (s_type.Equals("char[]"))
                {
                    o_targetType = typeof(char[]);
                }
                else if (s_type.Equals("float"))
                {
                    o_targetType = typeof(float);
                }
                else if (s_type.Equals("float[]"))
                {
                    o_targetType = typeof(float[]);
                }
                else if (s_type.Equals("double"))
                {
                    o_targetType = typeof(double);
                }
                else if (s_type.Equals("double[]"))
                {
                    o_targetType = typeof(double[]);
                }
                else if (s_type.Equals("short"))
                {
                    o_targetType = typeof(short);
                }
                else if (s_type.Equals("short[]"))
                {
                    o_targetType = typeof(short[]);
                }
                else if (s_type.Equals("ushort"))
                {
                    o_targetType = typeof(ushort);
                }
                else if (s_type.Equals("ushort[]"))
                {
                    o_targetType = typeof(ushort[]);
                }
                else if (s_type.Equals("int"))
                {
                    o_targetType = typeof(int);
                }
                else if (s_type.Equals("int[]"))
                {
                    o_targetType = typeof(int[]);
                }
                else if (s_type.Equals("uint"))
                {
                    o_targetType = typeof(uint);
                }
                else if (s_type.Equals("uint[]"))
                {
                    o_targetType = typeof(uint[]);
                }
                else if (s_type.Equals("long"))
                {
                    o_targetType = typeof(long);
                }
                else if (s_type.Equals("long[]"))
                {
                    o_targetType = typeof(long[]);
                }
                else if (s_type.Equals("ulong"))
                {
                    o_targetType = typeof(ulong);
                }
                else if (s_type.Equals("ulong[]"))
                {
                    o_targetType = typeof(ulong[]);
                }
                else if (s_type.Equals("string"))
                {
                    o_targetType = typeof(string);
                }
                else if (s_type.Equals("string[]"))
                {
                    o_targetType = typeof(string[]);
                }
                else if ((s_type.Equals("DateTime")) || (s_type.Equals("LocalTime")) || (s_type.Equals("LocalDate")) || (s_type.Equals("Date"))) /* because of java types */
                {
                    o_targetType = typeof(DateTime);
                }
                else if ((s_type.Equals("DateTime[]")) || (s_type.Equals("LocalTime[]")) || (s_type.Equals("LocalDate[]")) || (s_type.Equals("Date[]"))) /* because of java types */
                {
                    o_targetType = typeof(DateTime?[]);
                }
                else if (s_type.Equals("decimal"))
                {
                    o_targetType = typeof(decimal);
                }
                else if (s_type.Equals("decimal[]"))
                {
                    o_targetType = typeof(decimal[]);
                }
                else
                {
                    /* get origin type of enqueued data */
                    o_targetType = Type.GetType(s_type) ?? throw new NullReferenceException("Could not retrieve object type with '" + s_type + "'");
                }
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.ILogFinest("could not find type with '" + s_type + "': " + o_exc);

                try
                {
                    /* maybe this is a general type name from another system with another language */
                    string? s_namespace = null;

                    /* check if we may have a namespace in general type string */
                    if (s_type.Contains('.'))
                    {
                        /* split between namespace and type */
                        s_namespace = s_type.Substring(0, s_type.LastIndexOf("."));
                        s_type = s_type.Substring(s_type.LastIndexOf(".") + 1);

                        ForestNET.Lib.Global.ILogFinest("look for class '" + s_type + "' and namespace '" + s_namespace + "' in our assemblies");

                        /* look for class and namespace in our assemblies */
                        o_targetType = (from a in AppDomain.CurrentDomain.GetAssemblies()
                                        from t in a.GetTypes()
                                        where t.Name == s_type && t.Namespace == s_namespace
                                        select t).FirstOrDefault() ?? throw new InvalidOperationException("class '" + s_type + "' and namespace '" + s_namespace + "' could not be found in our assemblies");
                    }
                    else
                    {
                        ForestNET.Lib.Global.ILogFinest("look for class '" + s_type + "' in our assemblies");

                        /* look for class in our assemblies */
                        o_targetType = (from a in AppDomain.CurrentDomain.GetAssemblies()
                                        from t in a.GetTypes()
                                        where t.Name == s_type
                                        select t).FirstOrDefault() ?? throw new InvalidOperationException("class '" + s_type + "' could not be found in our assemblies"); ;
                    }
                }
                catch (Exception o_exc2)
                {
                    ForestNET.Lib.Global.ILogFinest("could not find 'general' type in our assemblies: " + o_exc2);

                    return null;
                }
            }

            /* unmarshall byte array to object to complete merging object data */
            try
            {
                ForestNET.Lib.Global.ILogFiner("unmarshall object from " + p_a_messageBytes.Length + " bytes");

                o_return = Marshall.UnmarshallObject(o_targetType, p_a_messageBytes, p_b_useProperties, p_b_systemUsesLittleEndian);
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.ILogWarning("could not unmarshall merged object data: " + o_exc);
                o_return = null;
            }

            return o_return;
        }

        /// <summary>
        /// Dequeue message from message box queue
        /// </summary>
        /// <returns>network message object</returns>
        public Message? DequeueMessage()
        {
            Message? o_return = null;

            /* check if message box queue is not empty */
            if (this.o_messageQueue.Count <= 0)
            {
                return o_return;
            }

            /* atomic lock */
            bool b_atomicLock = false;

            /* try dequeue message with monitor lock */
            try
            {
                /* try lock message box */
                Monitor.TryEnter(this.o_lock, this.TimeoutMilliseconds, ref b_atomicLock);

                /* we locked message box */
                if (b_atomicLock)
                {
                    this.IsLocked = true;

                    ForestNET.Lib.Global.ILogFinest("message box locked by '" + Environment.CurrentManagedThreadId + "' for DequeueMessage");

                    /* dequeue message from message box queue */
                    o_return = this.o_messageQueue.Dequeue();
                }
            }
            catch (Exception o_exc)
            {
                o_return = null;

                ForestNET.Lib.Global.ILogWarning("could not dequeue message: " + o_exc);
            }
            finally
            {
                /* we locked message box */
                if (b_atomicLock)
                {
                    ForestNET.Lib.Global.ILogFinest("message box unlocked by '" + Environment.CurrentManagedThreadId + "' for DequeueMessage");

                    /* unlock message box */
                    Monitor.Exit(this.o_lock);
                    this.IsLocked = false;
                    b_atomicLock = false;
                }
                else
                {
                    /* this thread did not locked message box and so could not dequeue message */
                    o_return = null;
                }
            }

            /* return dequeued message */
            return o_return;
        }
    }
}
