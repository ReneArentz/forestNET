namespace ForestNET.Lib.Net.Sock.Task
{
    /// <summary>
    /// Abstract class for all kind of socket tasks. It will handle the core execution process of a socket with handling all the network traffic.
    /// Some general methods are added to work with network traffic over sockets.
    /// </summary>
    public abstract class Task
    {

        /* Constants */

        protected const byte BY_ACK_BYTE = (byte)0xA5; /* 1010 0101 */
        public const int RECEIVE_MAX_UNKNOWN_AMOUNT_IN_MIB = 16;
        public const int TOLERATING_DELAY_IN_MS = 25;
        public const int AMOUNT_CYCLES_TOLERATING_DELAY = 5;

        /* Delegates */

        /// <summary>
        /// interface delegate definition which can be instanced outside of sock.task.Task class to post progress anywhere of sending/receiving bytes
        /// but only for TCP sockets
        /// </summary>
        public delegate void PostProgress(int p_i_bytes, int p_i_totalBytes);

        /* Fields */

        private string? s_commonSecretPassphrase;
        private int i_bufferLength;
        private int i_receiveMaxUnknownAmountInMiB;
        private int i_amountCyclesToleratingDelay;
        private System.Net.Security.SslStream? o_sslStream = null;
        private System.Net.Sockets.NetworkStream? o_networkStream = null;
        private int i_queueTimeoutMilliseconds;
        private int i_udpReceiveACKTimeoutMilliseconds;
        private int i_udpSendACKTimeoutMilliseconds;
        private int i_marshallingDataLengthInBytes;
        private PostProgress? del_postProgress;

        /* Properties */

        public bool Stop { get; set; } = false;
        public System.Threading.CancellationToken? CancellationToken { protected get; set; }
        public ForestNET.Lib.Net.Sock.Type? Type { get; private set; }
        public ForestNET.Lib.Net.Sock.Com.Type? CommunicationType { get; private set; }
        public ForestNET.Lib.Net.Sock.Com.Cardinality? CommunicationCardinality { get; private set; }
        protected ForestNET.Lib.Net.Sock.Com.Security? CommunicationSecurity { get; private set; }
        protected ForestNET.Lib.Cryptography? Cryptography { get; private set; }
        public string? CommonSecretPassphrase
        {
            protected get
            {
                return this.s_commonSecretPassphrase;
            }
            /**
			 * @throws ArgumentException		invalid common secret passphrase length [null or length lower than 36 characters]
			 * @throws NoSuchAlgorithmException 	invalid key factory algorithm
			 * @throws InvalidKeySpecException 		invalid key specifications, length of common secret passphrase, salt, iteration value or key length option
			 */
            set
            {
                if ((value == null) || (ForestNET.Lib.Helper.IsStringEmpty(value)))
                {
                    throw new ArgumentException("You have not specified a common secret passphrase for using symmetric 128/256-bit communication security");
                }

                if (value.Length < 36)
                {
                    throw new ArgumentException("Common secret passphrase must have at least '36' characters, but has '" + value.Length + "' characters");
                }

                this.s_commonSecretPassphrase = value;

                if ((this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_LOW) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_LOW))
                {
                    if (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_LOW)
                    {
                        this.Cryptography = new(this.s_commonSecretPassphrase, ForestNET.Lib.Cryptography.KEY128BIT);
                    }
                    else if (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_LOW)
                    {
                        this.Cryptography = new(this.s_commonSecretPassphrase, ForestNET.Lib.Cryptography.KEY256BIT);
                    }
                }
            }
        }
        public int BufferLength
        {
            get
            {
                return this.i_bufferLength;
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("Buffer length must be at least '1', but was set to '" + value + "'");
                }
                else
                {
                    this.i_bufferLength = value;
                }
            }
        }
        public int ReceiveMaxUnknownAmountInMiB
        {
            get
            {
                return this.i_receiveMaxUnknownAmountInMiB;
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("Receive max. unknown amount in MiB must be at least '1', but was set to '" + value + "'");
                }
                else if (value > 999)
                {
                    throw new ArgumentException("Receive max. unknown amount in MiB must be lower than '999', but was set to '" + value + "'");
                }
                else
                {
                    this.i_receiveMaxUnknownAmountInMiB = value;
                }
            }
        }
        public int AmountCyclesToleratingDelay
        {
            get
            {
                return this.i_amountCyclesToleratingDelay;
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("Amount of cycles tolerating delay (1 cycle = " + TOLERATING_DELAY_IN_MS + "ms) must be at least '1', but was set to '" + value + "'");
                }
                else if (value > 100)
                {
                    throw new ArgumentException("Amount of cycles tolerating delay (1 cycle = " + TOLERATING_DELAY_IN_MS + "ms) must be lower than '100 (" + (TOLERATING_DELAY_IN_MS * 100) + "ms)', but was set to '" + value + "'");
                }
                else
                {
                    this.i_amountCyclesToleratingDelay = value;
                }
            }
        }
        public ForestNET.Lib.Net.Sock.Socket? Socket { get; set; }
        public System.Net.Sockets.Socket? ReceivingSocket { get; set; }
        public Stream Stream
        {
            get
            {
                if (this.o_sslStream != null)
                {
                    return this.o_sslStream;
                }
                else if (this.o_networkStream != null)
                {
                    return this.o_networkStream;
                }

                throw new NullReferenceException("No stream is set within socket task instance");
            }
            set
            {
                if (value is System.Net.Sockets.NetworkStream)
                {
                    this.o_networkStream = (System.Net.Sockets.NetworkStream?)value;
                }
                else if (value is System.Net.Security.SslStream)
                {
                    this.o_sslStream = (System.Net.Security.SslStream?)value;
                }
                else
                {
                    if (this.o_sslStream != null)
                    {
                        this.o_sslStream.Close();
                        this.o_sslStream.Dispose();
                        this.o_sslStream = null;
                    }
                    else if (this.o_networkStream != null)
                    {
                        this.o_networkStream.Close();
                        this.o_networkStream.Dispose();
                        this.o_networkStream = null;
                    }
                }
            }
        }
        public System.Net.Sockets.Socket? UDPSocket { protected get; set; }
        public System.Net.IPEndPoint? UDPSourceAddress { protected get; set; }
        public byte[]? DatagramBytes { protected get; set; }
        public int QueueTimeoutMilliseconds
        {
            get
            {
                return this.i_queueTimeoutMilliseconds;
            }
            set
            {
                /* check timeout value for queue */
                if (value < 1)
                {
                    throw new ArgumentException("Queue timeout must be at least '1' millisecond, but was set to '" + value + "' millisecond(s)");
                }

                this.i_queueTimeoutMilliseconds = value;
            }
        }
        public List<ForestNET.Lib.Net.Msg.MessageBox>? MessageBoxes { protected get; set; }
        public List<ForestNET.Lib.Net.Msg.MessageBox>? AnswerMessageBoxes { protected get; set; }
        public int UDPReceiveACKTimeoutMilliseconds
        {
            get
            {
                return this.i_udpReceiveACKTimeoutMilliseconds;
            }
            set
            {
                if (value > 0)
                {
                    this.i_udpReceiveACKTimeoutMilliseconds = value;
                }
                else
                {
                    throw new ArgumentException("UDP receive ACK timeout must be at least '1' millisecond, but was set to '" + value + "' millisecond(s)");
                }
            }
        }
        public int UDPSendACKTimeoutMilliseconds
        {
            get
            {
                return this.i_udpSendACKTimeoutMilliseconds;
            }
            set
            {
                if (value > 0)
                {
                    this.i_udpSendACKTimeoutMilliseconds = value;
                }
                else
                {
                    throw new ArgumentException("UDP send ACK timeout must be at least '1' millisecond, but was set to '" + value + "' millisecond(s)");
                }
            }
        }
        public bool ObjectTransmission { get; set; }
        public Task? ReceiveSocketTask { protected get; set; }
        public Object? AnswerObject { get; protected set; }
        public Object? RequestObject { protected get; set; }
        protected List<Object>? Objects { get; set; }
        public bool DebugNetworkTrafficOn { protected get; set; }
        public bool UseMarshalling { get; set; }
        public int MarshallingDataLengthInBytes
        {
            get
            {
                return this.i_marshallingDataLengthInBytes;
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("Data length in bytes must be at least '1', but was set to '" + value + "'");
                }

                if (value > 4)
                {
                    throw new ArgumentException("Data length in bytes must be lower equal '4', but was set to '" + value + "'");
                }

                this.i_marshallingDataLengthInBytes = value;
            }
        }
        public bool MarshallingUseProperties { get; set; }
        public string? MarshallingOverrideMessageType { get; set; }
        public bool MarshallingSystemUsesLittleEndian { get; set; }
        public PostProgress? PostProgressDelegate
        {
            get
            {
                return this.del_postProgress;
            }
            set
            {
                if ((this.Type == ForestNET.Lib.Net.Sock.Type.TCP_SERVER) || (this.Type == ForestNET.Lib.Net.Sock.Type.TCP_CLIENT) || (this.Type == ForestNET.Lib.Net.Sock.Type.TCP_TLS_SERVER) || (this.Type == ForestNET.Lib.Net.Sock.Type.TCP_TLS_CLIENT))
                {
                    this.del_postProgress = value;
                }
            }
        }

        /* Methods */

        /// <summary>
        /// Creating socket task instance
        /// </summary>
        public Task()
        {
            this.o_networkStream = null;
            this.o_sslStream = null;
            this.Stop = false;
            this.CancellationToken = null;
            this.Type = null;
            this.CommunicationType = null;
            this.CommunicationCardinality = null;
            this.CommunicationSecurity = null;
            this.s_commonSecretPassphrase = null;
            this.Cryptography = null;
            this.BufferLength = 1;
            this.ReceiveMaxUnknownAmountInMiB = RECEIVE_MAX_UNKNOWN_AMOUNT_IN_MIB;
            this.AmountCyclesToleratingDelay = AMOUNT_CYCLES_TOLERATING_DELAY;
            this.Socket = null;
            this.ReceivingSocket = null;
            this.UDPSocket = null;
            this.UDPSourceAddress = null;
            this.DatagramBytes = null;
            this.QueueTimeoutMilliseconds = 1;
            this.MessageBoxes = null;
            this.AnswerMessageBoxes = null;
            this.UDPReceiveACKTimeoutMilliseconds = 1;
            this.UDPSendACKTimeoutMilliseconds = 1;
            this.ObjectTransmission = false;
            this.ReceiveSocketTask = null;
            this.RequestObject = null;
            this.AnswerObject = null;
            this.Objects = null;
            this.DebugNetworkTrafficOn = false;
            this.UseMarshalling = false;
            this.MarshallingDataLengthInBytes = 1;
            this.MarshallingUseProperties = false;
            this.MarshallingOverrideMessageType = null;
            this.MarshallingSystemUsesLittleEndian = false;
            this.PostProgressDelegate = null;
        }

        /// <summary>
        /// Creating socket task instance with socket type parameter
        /// </summary>
        /// <param name="p_e_type">specifies socket type of socket task</param>
        public Task(ForestNET.Lib.Net.Sock.Type p_e_type)
        {
            this.o_networkStream = null;
            this.o_sslStream = null;
            this.Stop = false;
            this.CancellationToken = null;
            this.Type = p_e_type;
            this.CommunicationType = null;
            this.CommunicationCardinality = null;
            this.CommunicationSecurity = null;
            this.s_commonSecretPassphrase = null;
            this.Cryptography = null;
            this.BufferLength = 1;
            this.ReceiveMaxUnknownAmountInMiB = RECEIVE_MAX_UNKNOWN_AMOUNT_IN_MIB;
            this.AmountCyclesToleratingDelay = AMOUNT_CYCLES_TOLERATING_DELAY;
            this.Socket = null;
            this.ReceivingSocket = null;
            this.UDPSocket = null;
            this.UDPSourceAddress = null;
            this.DatagramBytes = null;
            this.QueueTimeoutMilliseconds = 1;
            this.MessageBoxes = null;
            this.AnswerMessageBoxes = null;
            this.UDPReceiveACKTimeoutMilliseconds = 1;
            this.UDPSendACKTimeoutMilliseconds = 1;
            this.ObjectTransmission = false;
            this.ReceiveSocketTask = null;
            this.RequestObject = null;
            this.AnswerObject = null;
            this.Objects = null;
            this.DebugNetworkTrafficOn = false;
            this.UseMarshalling = false;
            this.MarshallingDataLengthInBytes = 1;
            this.MarshallingUseProperties = false;
            this.MarshallingOverrideMessageType = null;
            this.MarshallingSystemUsesLittleEndian = false;
            this.PostProgressDelegate = null;
        }

        /// <summary>
        /// Creating socket task instance with all it's parameters and settings
        /// </summary>
        /// <param name="p_e_communicationType">specifies communication type of socket task</param>
        /// <param name="p_e_communicationCardinality">specifies communication cardinality of socket task</param>
        /// <param name="p_i_queueTimeoutMilliseconds">determine timeout in milliseconds for sending/receiving bytes</param>
        /// <exception cref="ArgumentException">invalid timeout value for queue</exception>
        public Task(ForestNET.Lib.Net.Sock.Com.Type? p_e_communicationType, ForestNET.Lib.Net.Sock.Com.Cardinality? p_e_communicationCardinality, int p_i_queueTimeoutMilliseconds) :
            this(p_e_communicationType, p_e_communicationCardinality, p_i_queueTimeoutMilliseconds, null)
        {

        }

        /// <summary>
        /// Creating socket task instance with all it's parameters and settings
        /// </summary>
        /// <param name="p_e_communicationType">specifies communication type of socket task</param>
        /// <param name="p_e_communicationCardinality">specifies communication cardinality of socket task</param>
        /// <param name="p_i_queueTimeoutMilliseconds">determine timeout in milliseconds for sending/receiving bytes</param>
        /// <param name="p_e_communicationSecurity">specifies communication security of socket task</param>
        /// <exception cref="ArgumentException">invalid timeout value for queue</exception>
        public Task(ForestNET.Lib.Net.Sock.Com.Type? p_e_communicationType, ForestNET.Lib.Net.Sock.Com.Cardinality? p_e_communicationCardinality, int p_i_queueTimeoutMilliseconds, ForestNET.Lib.Net.Sock.Com.Security? p_e_communicationSecurity) :
            this(p_e_communicationType, p_e_communicationCardinality, p_i_queueTimeoutMilliseconds, p_e_communicationSecurity, null)
        {

        }

        /// <summary>
        /// Creating socket task instance with all it's parameters and settings
        /// </summary>
        /// <param name="p_e_communicationType">specifies communication type of socket task</param>
        /// <param name="p_e_communicationCardinality">specifies communication cardinality of socket task</param>
        /// <param name="p_i_queueTimeoutMilliseconds">determine timeout in milliseconds for sending/receiving bytes</param>
        /// <param name="p_e_communicationSecurity">specifies communication security of socket task</param>
        /// <param name="p_itf_delegate">interface delegate to post progress of sending/receiving bytes</param>
        /// <exception cref="ArgumentException">invalid timeout value for queue</exception>
        public Task(ForestNET.Lib.Net.Sock.Com.Type? p_e_communicationType, ForestNET.Lib.Net.Sock.Com.Cardinality? p_e_communicationCardinality, int p_i_queueTimeoutMilliseconds, ForestNET.Lib.Net.Sock.Com.Security? p_e_communicationSecurity, PostProgress? p_itf_delegate) :
            this(p_e_communicationType, p_e_communicationCardinality, p_i_queueTimeoutMilliseconds, p_e_communicationSecurity, p_itf_delegate, false, 1, false, null, false)
        {

        }

        /// <summary>
        /// Creating socket task instance with all it's parameters and settings
        /// </summary>
        /// <param name="p_e_communicationType">specifies communication type of socket task</param>
        /// <param name="p_e_communicationCardinality">specifies communication cardinality of socket task</param>
        /// <param name="p_i_queueTimeoutMilliseconds">determine timeout in milliseconds for sending/receiving bytes</param>
        /// <param name="p_e_communicationSecurity">specifies communication security of socket task</param>
        /// <param name="p_itf_delegate">interface delegate to post progress of sending/receiving bytes</param>
        /// <param name="p_b_useMarshalling">true - use marshalling methods to transport data over network</param>
        /// <param name="p_i_marshallingDataLengthInBytes">set data length in bytes for marshalling, must be between [1..4]</param>
        /// <param name="p_b_marshallingUseProperties">true - access object parameter fields via properties</param>
        /// <param name="p_s_marshallingOverrideMessageType">override message type with this string and do not get it automatically from object, thus the type can be set generally from other systems with other programming languages</param>
        /// <param name="p_b_marshallingSystemUsesLittleEndian">(NOT IMPLEMENTED) true - current execution system uses little endian, false - current execution system uses big endian</param>
        /// <exception cref="ArgumentException">invalid timeout value for queue</exception>
        public Task(ForestNET.Lib.Net.Sock.Com.Type? p_e_communicationType, ForestNET.Lib.Net.Sock.Com.Cardinality? p_e_communicationCardinality, int p_i_queueTimeoutMilliseconds, ForestNET.Lib.Net.Sock.Com.Security? p_e_communicationSecurity, PostProgress? p_itf_delegate, bool p_b_useMarshalling, int p_i_marshallingDataLengthInBytes, bool p_b_marshallingUseProperties, string? p_s_marshallingOverrideMessageType, bool p_b_marshallingSystemUsesLittleEndian)
        {
            this.o_networkStream = null;
            this.o_sslStream = null;
            this.Stop = false;
            this.CancellationToken = null;
            this.Type = null;
            this.CommunicationType = p_e_communicationType;
            this.CommunicationCardinality = p_e_communicationCardinality;
            this.CommunicationSecurity = p_e_communicationSecurity;
            this.s_commonSecretPassphrase = null;
            this.Cryptography = null;
            this.BufferLength = 1;
            this.ReceiveMaxUnknownAmountInMiB = RECEIVE_MAX_UNKNOWN_AMOUNT_IN_MIB;
            this.AmountCyclesToleratingDelay = AMOUNT_CYCLES_TOLERATING_DELAY;
            this.Socket = null;
            this.ReceivingSocket = null;
            this.UDPSocket = null;
            this.UDPSourceAddress = null;
            this.DatagramBytes = null;
            this.i_queueTimeoutMilliseconds = p_i_queueTimeoutMilliseconds;
            this.MessageBoxes = null;
            this.AnswerMessageBoxes = null;
            this.UDPReceiveACKTimeoutMilliseconds = 1;
            this.UDPSendACKTimeoutMilliseconds = 1;
            this.ObjectTransmission = false;
            this.ReceiveSocketTask = null;
            this.RequestObject = null;
            this.AnswerObject = null;
            this.Objects = null;
            this.DebugNetworkTrafficOn = false;
            this.UseMarshalling = p_b_useMarshalling;
            this.MarshallingDataLengthInBytes = p_i_marshallingDataLengthInBytes;
            this.MarshallingUseProperties = p_b_marshallingUseProperties;
            this.MarshallingOverrideMessageType = p_s_marshallingOverrideMessageType;
            this.MarshallingSystemUsesLittleEndian = p_b_marshallingSystemUsesLittleEndian;
            this.PostProgressDelegate = null;

            /* check timeout value for queue */
            if (this.i_queueTimeoutMilliseconds < 1)
            {
                throw new ArgumentException("Queue timeout must be at least '1' millisecond, but was set to '" + this.i_queueTimeoutMilliseconds + "' millisecond(s)");
            }

            if (
                this.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND ||
                this.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND_WITH_ACK
            )
            {
                /* set socket type to UDP client */
                this.Type = ForestNET.Lib.Net.Sock.Type.UDP_CLIENT;
            }
            else if (
                this.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE ||
                this.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE_WITH_ACK
            )
            {
                /* set socket type to UDP server */
                this.Type = ForestNET.Lib.Net.Sock.Type.UDP_SERVER;
            }
            else if (
                this.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.TCP_SEND ||
                this.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.TCP_SEND_WITH_ANSWER
            )
            {
                /* set socket type to TCP client and accept delegate parameter */
                this.Type = ForestNET.Lib.Net.Sock.Type.TCP_CLIENT;
                this.PostProgressDelegate = p_itf_delegate;
            }
            else if (
                this.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.TCP_RECEIVE ||
                this.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.TCP_RECEIVE_WITH_ANSWER
            )
            {
                /* set socket type to TCP and accept delegate parameter */
                this.Type = ForestNET.Lib.Net.Sock.Type.TCP_SERVER;
                this.PostProgressDelegate = p_itf_delegate;
            }

            /* log receive socket task settings */
            ForestNET.Lib.Global.ILogConfig("created socket task");
            ForestNET.Lib.Global.ILogConfig("\t" + "communication type:" + "\t\t\t" + this.CommunicationType);
            ForestNET.Lib.Global.ILogConfig("\t" + "communication cardinality:" + "\t\t\t" + this.CommunicationCardinality);
            ForestNET.Lib.Global.ILogConfig("\t" + "queue timeout ms:" + "\t\t\t" + this.i_queueTimeoutMilliseconds);
            ForestNET.Lib.Global.ILogConfig("\t" + "communication security:" + "\t\t\t" + this.CommunicationSecurity);
        }

        /// <summary>
        /// abstract method to clone this socket task with another socket task instance
        /// </summary>
        /// <param name="p_o_sourceTask">another socket task instance as source for all it's parameters and settings</param>
        abstract public void CloneFromOtherTask(Task p_o_sourceTask);

        /// <summary>
        /// cloning basic properties of a socket task instance with information of another socket task instance
        /// </summary>
        /// <param name="p_o_sourceTask">another socket task instance as source for all it's parameters and settings</param>
        protected void CloneBasicProperties(Task p_o_sourceTask)
        {
            this.o_networkStream = null; /* no cloning necessary */
            this.o_sslStream = null; /* no cloning necessary */
            this.Stop = false;
            this.CancellationToken = null; /* no cloning necessary */
            this.Type = null;
            this.CommunicationType = null;
            this.CommunicationCardinality = null;
            this.CommunicationSecurity = null;
            this.s_commonSecretPassphrase = null;
            this.Cryptography = null;
            this.BufferLength = 1;
            this.ReceiveMaxUnknownAmountInMiB = RECEIVE_MAX_UNKNOWN_AMOUNT_IN_MIB;
            this.AmountCyclesToleratingDelay = AMOUNT_CYCLES_TOLERATING_DELAY;
            this.Socket = null; /* no cloning necessary */
            this.ReceivingSocket = null; /* no cloning necessary */
            this.UDPSocket = null; /* no cloning necessary */
            this.UDPSourceAddress = null; /* no cloning necessary */
            this.DatagramBytes = null; /* no cloning necessary */
            this.QueueTimeoutMilliseconds = 1;
            this.MessageBoxes = null;
            this.AnswerMessageBoxes = null;
            this.UDPReceiveACKTimeoutMilliseconds = 1;
            this.UDPSendACKTimeoutMilliseconds = 1;
            this.ObjectTransmission = false;
            this.ReceiveSocketTask = null;
            this.RequestObject = null; /* no cloning necessary */
            this.AnswerObject = null; /* no cloning necessary */
            this.Objects = null; /* no cloning necessary */
            this.DebugNetworkTrafficOn = false;
            this.UseMarshalling = false;
            this.MarshallingDataLengthInBytes = 1;
            this.MarshallingUseProperties = false;
            this.MarshallingOverrideMessageType = null;
            this.MarshallingSystemUsesLittleEndian = false;
            this.PostProgressDelegate = null; /* no cloning necessary */

            /* ignore exceptions if a property of source task has no valid value, we will keep it null */
            try { this.Type = p_o_sourceTask.Type; } catch (Exception) { /* NOP */ }
            try { this.CommunicationType = p_o_sourceTask.CommunicationType; } catch (Exception) { /* NOP */ }
            try { this.CommunicationCardinality = p_o_sourceTask.CommunicationCardinality; } catch (Exception) { /* NOP */ }
            try { this.CommunicationSecurity = p_o_sourceTask.CommunicationSecurity; } catch (Exception) { /* NOP */ }
            try { this.CommonSecretPassphrase = p_o_sourceTask.CommonSecretPassphrase; } catch (Exception) { /* NOP */ }
            try { this.BufferLength = p_o_sourceTask.BufferLength; } catch (Exception) { /* NOP */ }
            try { this.ReceiveMaxUnknownAmountInMiB = p_o_sourceTask.ReceiveMaxUnknownAmountInMiB; } catch (Exception) { /* NOP */ }
            try { this.QueueTimeoutMilliseconds = p_o_sourceTask.QueueTimeoutMilliseconds; } catch (Exception) { /* NOP */ }
            try { this.MessageBoxes = p_o_sourceTask.MessageBoxes; } catch (Exception) { /* NOP */ }
            try { this.AnswerMessageBoxes = p_o_sourceTask.AnswerMessageBoxes; } catch (Exception) { /* NOP */ }
            try { this.UDPReceiveACKTimeoutMilliseconds = p_o_sourceTask.UDPReceiveACKTimeoutMilliseconds; } catch (Exception) { /* NOP */ }
            try { this.UDPSendACKTimeoutMilliseconds = p_o_sourceTask.UDPSendACKTimeoutMilliseconds; } catch (Exception) { /* NOP */ }
            try { this.ObjectTransmission = p_o_sourceTask.ObjectTransmission; } catch (Exception) { /* NOP */ }
            try { this.ReceiveSocketTask = p_o_sourceTask.ReceiveSocketTask; } catch (Exception) { /* NOP */ }
            try { this.DebugNetworkTrafficOn = p_o_sourceTask.DebugNetworkTrafficOn; } catch (Exception) { /* NOP */ }
            try { this.UseMarshalling = p_o_sourceTask.UseMarshalling; } catch (Exception) { /* NOP */ }
            try { this.MarshallingDataLengthInBytes = p_o_sourceTask.MarshallingDataLengthInBytes; } catch (Exception) { /* NOP */ }
            try { this.MarshallingUseProperties = p_o_sourceTask.MarshallingUseProperties; } catch (Exception) { /* NOP */ }
            try { this.MarshallingOverrideMessageType = p_o_sourceTask.MarshallingOverrideMessageType; } catch (Exception) { /* NOP */ }
            try { this.MarshallingSystemUsesLittleEndian = p_o_sourceTask.MarshallingSystemUsesLittleEndian; } catch (Exception) { /* NOP */ }

            if (
                this.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND ||
                this.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND_WITH_ACK
            )
            {
                /* set socket type to UDP client */
                this.Type = ForestNET.Lib.Net.Sock.Type.UDP_CLIENT;
            }
            else if (
                this.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE ||
                this.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE_WITH_ACK
            )
            {
                /* set socket type to UDP server */
                this.Type = ForestNET.Lib.Net.Sock.Type.UDP_SERVER;
            }
            else if (
                this.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.TCP_SEND ||
                this.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.TCP_SEND_WITH_ANSWER
            )
            {
                /* set socket type to TCP client and accept delegate parameter */
                this.Type = ForestNET.Lib.Net.Sock.Type.TCP_CLIENT;
            }
            else if (
                this.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.TCP_RECEIVE ||
                this.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.TCP_RECEIVE_WITH_ANSWER
            )
            {
                /* set socket type to TCP and accept delegate parameter */
                this.Type = ForestNET.Lib.Net.Sock.Type.TCP_SERVER;
            }
        }

        /// <summary>
        /// abstract runTask method of socket task which can always vary depending on the implementation
        /// </summary>
        /// <exception cref="Exception">any exception of implementation that could happen will be caught</exception>
        abstract public System.Threading.Tasks.Task RunTask();

        /// <summary>
        /// Method which is doing nothing, but returning a completed task return value. This is practical if you implement a RunTask method where no await is used so far. So you can await DoNothing()
        /// </summary>
        public static System.Threading.Tasks.Task DoNoting()
        {
            return System.Threading.Tasks.Task.CompletedTask;
        }

        /// <summary>
        /// Core execution process method of a socket task, checking all settings before executing abstract RunTask() method
        /// </summary>
        /// <exception cref="ArgumentNullException">class property is not set</exception>
        /// <exception cref="ArgumentException">invalid value for buffer length [lower than 1] or common secret passphrase length [null or length lower than 36 characters]</exception>
        public async System.Threading.Tasks.Task Run()
        {
            try
            {
                ForestNET.Lib.Global.ILogFinest("net.sock.task.Task run-method called");

                if ((this.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE) || (this.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE_WITH_ACK))
                {
                    if (this.DatagramBytes == null)
                    {
                        throw new ArgumentNullException("Received datagram packet is null");
                    }

                    if (this.MessageBoxes == null)
                    {
                        throw new ArgumentNullException("Message boxes are null");
                    }
                }
                else if ((this.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND) || (this.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND_WITH_ACK))
                {
                    if (this.UDPSocket == null)
                    {
                        throw new ArgumentNullException("Datagram socket is null");
                    }

                    if (this.MessageBoxes == null)
                    {
                        throw new ArgumentNullException("Message boxes are null");
                    }
                }
                else if (this.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.TCP_RECEIVE)
                {
                    if (this.Socket == null)
                    {
                        throw new ArgumentNullException("TCP socket is null");
                    }

                    if (this.BufferLength < 1)
                    {
                        throw new ArgumentException("Buffer length must be at least '1', but was set to '" + this.BufferLength + "'");
                    }

                    if (this.MessageBoxes == null)
                    {
                        throw new ArgumentNullException("Message boxes are null");
                    }
                }
                else if (this.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.TCP_SEND)
                {
                    if (this.Socket == null)
                    {
                        throw new ArgumentNullException("TCP socket is null");
                    }

                    if (this.BufferLength < 1)
                    {
                        throw new ArgumentException("Buffer length must be at least '1', but was set to '" + this.BufferLength + "'");
                    }

                    if (this.MessageBoxes == null)
                    {
                        throw new ArgumentNullException("Message boxes are null");
                    }
                }
                else if (this.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.TCP_RECEIVE_WITH_ANSWER)
                {
                    if (this.Socket == null)
                    {
                        throw new ArgumentNullException("TCP socket is null");
                    }

                    if (this.BufferLength < 1)
                    {
                        throw new ArgumentException("Buffer length must be at least '1', but was set to '" + this.BufferLength + "'");
                    }

                    if (this.MessageBoxes == null)
                    {
                        throw new ArgumentNullException("Message boxes are null");
                    }

                    if (this.ReceiveSocketTask == null)
                    {
                        throw new ArgumentNullException("Receive socket task is null");
                    }
                }
                else if (this.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.TCP_SEND_WITH_ANSWER)
                {
                    if (this.Socket == null)
                    {
                        throw new ArgumentNullException("TCP socket is null");
                    }

                    if (this.BufferLength < 1)
                    {
                        throw new ArgumentException("Buffer length must be at least '1', but was set to '" + this.BufferLength + "'");
                    }

                    if (this.MessageBoxes == null)
                    {
                        throw new ArgumentNullException("Message boxes are null");
                    }

                    if (this.AnswerMessageBoxes == null)
                    {
                        throw new ArgumentNullException("Answer message boxes are null");
                    }
                }

                if ((this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_HIGH) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_HIGH) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_LOW) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_LOW))
                {
                    if ((this.CommonSecretPassphrase == null) || (ForestNET.Lib.Helper.IsStringEmpty(this.CommonSecretPassphrase)))
                    {
                        throw new ArgumentException("You have not specified a common secret passphrase for using symmetric 128/256-bit communication security");
                    }

                    if (this.CommonSecretPassphrase.Length < 36)
                    {
                        throw new ArgumentException("Common secret passphrase must have at least '36' characters, but has '" + this.CommonSecretPassphrase.Length + "' characters");
                    }

                    if ((this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_LOW) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_LOW))
                    {
                        if (this.Cryptography == null)
                        {
                            throw new ArgumentNullException("Cryptography object is not initialized");
                        }
                    }
                }

                try
                {
                    ForestNET.Lib.Global.ILogFinest("net.sock.task.Task execute abstract runTask-method");

                    /* execute abstract runTask method so the core execution of socket task can always vary depending on the implementation  */
                    await this.RunTask();
                }
                catch (TaskCanceledException o_exc)
                {
                    /* log task cancellation, but do not abort or throw this exception further up */
                    ForestNET.Lib.Global.ILogFine("socket task[" + this.GetType().FullName + "] was canceled: " + o_exc);
                }
                catch (Exception)
                {
                    /* handle any exception within abstract part of socket task, to have control of cancelation */
                    throw;
                }
            }
            catch (Exception o_exc)
            {
                /* handle any exception of socket task */
                ForestNET.Lib.Global.LogException(o_exc);
            }
        }

        /// <summary>
        /// Add message box to list 
        /// </summary>
        /// <param name="p_o_messageBox">message box instance</param>
        /// <exception cref="ArgumentNullException">message box parameter is null</exception>
        public void AddMessageBox(ForestNET.Lib.Net.Msg.MessageBox p_o_messageBox)
        {
            if (p_o_messageBox == null)
            {
                throw new ArgumentNullException(nameof(p_o_messageBox), "Message box parameter is null");
            }

            if (this.MessageBoxes == null)
            {
                this.MessageBoxes = [];
            }

            this.MessageBoxes.Add(p_o_messageBox);
        }

        /// <summary>
        /// Add answer message box to list 
        /// </summary>
        /// <param name="p_o_messageBox">answer message box instance</param>
        /// <exception cref="ArgumentNullException">answer message box parameter is null</exception>
        public void AddAnswerMessageBox(ForestNET.Lib.Net.Msg.MessageBox p_o_answerMessageBox)
        {
            if (p_o_answerMessageBox == null)
            {
                throw new ArgumentNullException(nameof(p_o_answerMessageBox), "Answer message box parameter is null");
            }

            if (this.AnswerMessageBoxes == null)
            {
                this.AnswerMessageBoxes = [];
            }

            this.AnswerMessageBoxes.Add(p_o_answerMessageBox);
        }

        /// <summary>
        /// Add object to list 
        /// </summary>
        /// <param name="p_o_messageBox">object instance</param>
        /// <exception cref="ArgumentNullException">object parameter is null</exception>
        public void AddObject(Object p_o_object)
        {
            if (p_o_object == null)
            {
                throw new ArgumentNullException(nameof(p_o_object), "Object parameter is null");
            }

            if (this.Objects == null)
            {
                this.Objects = [];
            }

            this.Objects.Add(p_o_object);
        }

        /// <summary>
        /// check if socket task has messages available in any configured message box
        /// </summary>
        /// <returns>true - message available, false - no message available in any message box, so maybe time for a timeout</returns>
        public bool MessagesAvailable()
        {
            /* there is not even message box array initialized, so returning false */
            if (this.MessageBoxes == null)
            {
                return false;
            }

            bool b_return = false;

            /* iterate each message box in array */
            foreach (ForestNET.Lib.Net.Msg.MessageBox o_messageBox in this.MessageBoxes)
            {
                /* return true if any message box has a message */
                if (o_messageBox.MessageAmount > 0)
                {
                    b_return = true;
                    break;
                }
            }

            return b_return;
        }

        /// <summary>
        /// Protocol method receive amount of bytes between two socket instances
        /// </summary>
        /// <returns>amount of bytes which will be send from other communication side</returns>
        /// <exception cref="ArgumentException">invalid amount of bytes parameter or byte array for encryption is empty</exception>
        /// <exception cref="InvalidProgramException">AmountBytesProtocol failed after several attempts</exception>
        /// <exception cref="InvalidOperationException">not implemented for UDP protocol</exception>
        /// <exception cref="Exception">any other Exception, see below in other classes for details</exception>
        protected async System.Threading.Tasks.Task<int> AmountBytesProtocol()
        {
            return await this.AmountBytesProtocol(0);
        }

        /// <summary>
        /// Protocol method to send or receive amount of bytes between two socket instances
        /// </summary>
        /// <param name="p_b_bytesAmount">amount of bytes we want to sent to other communication side, or 0 if we want to receive amount of bytes information</param>
        /// <returns>amount of bytes which will be send from other communication side</returns>
        /// <exception cref="ArgumentException">invalid amount of bytes parameter or byte array for encryption is empty</exception>
        /// <exception cref="InvalidProgramException">AmountBytesProtocol failed after several attempts</exception>
        /// <exception cref="InvalidOperationException">not implemented for UDP protocol</exception>
        /// <exception cref="Exception">any other Exception, see below in other classes for details</exception>
        protected async System.Threading.Tasks.Task<int> AmountBytesProtocol(int p_b_bytesAmount)
        {
            int i_return = -1;

            /* only TCP is supported */
            if ((this.Type == ForestNET.Lib.Net.Sock.Type.TCP_CLIENT) || (this.Type == ForestNET.Lib.Net.Sock.Type.TCP_SERVER) || (this.Type == ForestNET.Lib.Net.Sock.Type.TCP_TLS_SERVER) || (this.Type == ForestNET.Lib.Net.Sock.Type.TCP_TLS_CLIENT))
            {
                /* parameter value is lesser equal 0, then we want to receive amount of bytes incoming next */
                if (p_b_bytesAmount <= 0)
                {
                    /* help variables */
                    byte by_length = 0;
                    int i_maxAttempts = 40;
                    int i_attempts = 1;

                    /* receive bytes length, to know in how many bytes the transmission length is encoded */
                    do
                    {
                        /* byte array for receiving amount of bytes which hold transmission length */
                        byte[]? a_length;

                        /* receive length, expecting different amount of bytes because of encryption */
                        if ((this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_HIGH) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_HIGH))
                        {
                            a_length = await this.ReceiveBytes(33, 33);
                        }
                        else if ((this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_LOW) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_LOW))
                        {
                            a_length = await this.ReceiveBytes(17, 17);
                        }
                        else
                        {
                            a_length = await this.ReceiveBytes(1, 1);
                        }

                        /* check if we received any bytes */
                        if ((a_length != null) && (a_length.Length > 0))
                        {
                            /* log received bytes length if no encryption is active */
                            if ((this.CommunicationSecurity != ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_HIGH) && (this.CommunicationSecurity != ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_HIGH) && (this.CommunicationSecurity != ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_LOW) && (this.CommunicationSecurity != ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_LOW))
                            {
                                ForestNET.Lib.Global.ILogFiner("Receiving bytes length: " + ForestNET.Lib.Helper.PrintByteArray(a_length, false));
                            }

                            /* decrypt received bytes length if encryption is active */
                            if (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_HIGH)
                            {
                                a_length = ForestNET.Lib.Cryptography.Decrypt_AES_GCM(a_length, this.CommonSecretPassphrase ?? "", ForestNET.Lib.Cryptography.KEY128BIT);
                            }
                            else if (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_HIGH)
                            {
                                a_length = ForestNET.Lib.Cryptography.Decrypt_AES_GCM(a_length, this.CommonSecretPassphrase ?? "", ForestNET.Lib.Cryptography.KEY256BIT);
                            }
                            else if ((this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_LOW) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_LOW))
                            {
                                a_length = this.Cryptography?.Decrypt(a_length) ?? [];
                            }

                            /* log received bytes length if encryption is active */
                            if ((this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_HIGH) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_HIGH) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_LOW) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_LOW))
                            {
                                ForestNET.Lib.Global.ILogFiner("Receiving bytes length: " + ForestNET.Lib.Helper.PrintByteArray(a_length, false));
                            }

                            by_length = a_length[0];
                        }

                        /* bytes length must be greater than 0 */
                        if (by_length == 0)
                        {
                            ForestNET.Lib.Global.ILogFiner("Length[" + by_length + "] of transmission length must be greater than 0; retry after 25 millisecond");

                            /* wait 25 milliseconds to receive length again */
                            await System.Threading.Tasks.Task.Delay(25);

                            if (i_attempts >= i_maxAttempts)
                            { /* all attempts failed, so protocol for receiving length failed completely or was not intended (check availability call over TCP) */
                                ForestNET.Lib.Global.ILogWarning("AmountBytesProtocol " + i_attempts + " attempts");
                                return 0;
                            }
                        }

                        i_attempts++;
                    } while (by_length == 0);

                    /* ------------------------------------------------------ */

                    /* send acknowledge that bytes length has been received */
                    await this.SendACK();

                    /* ------------------------------------------------------ */

                    /* byte array for receiving transmission length */
                    byte[]? a_transmissionLength;

                    /* receive transmission length, expecting different amount of bytes because of encryption */
                    if ((this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_HIGH) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_HIGH))
                    {
                        a_transmissionLength = await this.ReceiveBytes(by_length + 28, by_length + 28);
                    }
                    else if ((this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_LOW) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_LOW))
                    {
                        a_transmissionLength = await this.ReceiveBytes(by_length + 16, by_length + 16);
                    }
                    else
                    {
                        a_transmissionLength = await this.ReceiveBytes(by_length, by_length);
                    }

                    /* log received transmission length if no encryption is active */
                    if ((this.CommunicationSecurity != ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_HIGH) && (this.CommunicationSecurity != ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_HIGH) && (this.CommunicationSecurity != ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_LOW) && (this.CommunicationSecurity != ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_LOW))
                    {
                        ForestNET.Lib.Global.ILogFiner("Receiving transmission length: " + ForestNET.Lib.Helper.PrintByteArray(a_transmissionLength, false));
                    }

                    /* decrypt received transmission length if encryption is active */
                    if (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_HIGH)
                    {
                        a_transmissionLength = ForestNET.Lib.Cryptography.Decrypt_AES_GCM(a_transmissionLength ?? [], this.CommonSecretPassphrase ?? "", ForestNET.Lib.Cryptography.KEY128BIT);
                    }
                    else if (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_HIGH)
                    {
                        a_transmissionLength = ForestNET.Lib.Cryptography.Decrypt_AES_GCM(a_transmissionLength ?? [], this.CommonSecretPassphrase ?? "", ForestNET.Lib.Cryptography.KEY256BIT);
                    }
                    else if ((this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_LOW) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_LOW))
                    {
                        a_transmissionLength = this.Cryptography?.Decrypt(a_transmissionLength ?? []);
                    }

                    /* log received transmission length if no encryption is active */
                    if ((this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_HIGH) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_HIGH) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_LOW) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_LOW))
                    {
                        ForestNET.Lib.Global.ILogFiner("Receiving transmission length: " + ForestNET.Lib.Helper.PrintByteArray(a_transmissionLength, false));
                    }

                    /* save transmission length as integer for return value */
                    i_return = ForestNET.Lib.Helper.ByteArrayToInt(a_transmissionLength);

                    /* ------------------------------------------------------ */

                    /* send acknowledge that transmission length has been received */
                    await this.SendACK();
                }
                else
                {
                    /* amount of bytes parameter must not exceed max. value of integer */
                    if (p_b_bytesAmount > 2147483646)
                    {
                        throw new ArgumentException("Max. amount of data bytes is '2.147.483.646 bytes (2,15 GB)', but parameter amount of input data bytes is '" + p_b_bytesAmount + "'");
                    }

                    /* byte array for sending bytes length of transmission length */
                    byte[] a_length = new byte[] { 1 };
                    /* amount of bytes for transmission length */
                    int i_length;

                    if (p_b_bytesAmount > 255)
                    {
                        a_length[0]++;
                    }

                    if (p_b_bytesAmount > 65535)
                    {
                        a_length[0]++;
                    }

                    if (p_b_bytesAmount > 16777215)
                    {
                        a_length[0]++;
                    }

                    /* update amount of bytes */
                    i_length = a_length[0];

                    ForestNET.Lib.Global.ILogFiner("Sending length: " + ForestNET.Lib.Helper.PrintByteArray(a_length, false));

                    /* encrypt amount of bytes of transmission length if encryption is active */
                    if (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_HIGH)
                    {
                        a_length = ForestNET.Lib.Cryptography.Encrypt_AES_GCM(a_length, this.CommonSecretPassphrase ?? "", ForestNET.Lib.Cryptography.KEY128BIT);
                    }
                    else if (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_HIGH)
                    {
                        a_length = ForestNET.Lib.Cryptography.Encrypt_AES_GCM(a_length, this.CommonSecretPassphrase ?? "", ForestNET.Lib.Cryptography.KEY256BIT);
                    }
                    else if ((this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_LOW) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_LOW))
                    {
                        a_length = this.Cryptography?.Encrypt(a_length) ?? [];
                    }

                    /* sending amount of bytes of transmission length */
                    await this.SendBytes(a_length, a_length.Length);

                    /* ------------------------------------------------------ */

                    /* receive acknowledge that bytes length has been received correctly */
                    await this.ReceiveACK();

                    /* ------------------------------------------------------ */

                    /* byte array for sending transmission length */
                    byte[] a_transmissionLength = ForestNET.Lib.Helper.IntToByteArray(p_b_bytesAmount) ?? [];

                    if (a_transmissionLength.Length != i_length)
                    {
                        throw new InvalidProgramException("Transmission length amount of bytes[" + a_transmissionLength.Length + "] is not equal to calculated amount of bytes for transmission length[" + i_length + "] before");
                    }

                    ForestNET.Lib.Global.ILogFiner("Sending transmission length: " + ForestNET.Lib.Helper.PrintByteArray(a_transmissionLength, false));

                    /* encrypt transmission length if encryption is active */
                    if (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_HIGH)
                    {
                        a_transmissionLength = ForestNET.Lib.Cryptography.Encrypt_AES_GCM(a_transmissionLength, this.CommonSecretPassphrase ?? "", ForestNET.Lib.Cryptography.KEY128BIT);
                    }
                    else if (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_HIGH)
                    {
                        a_transmissionLength = ForestNET.Lib.Cryptography.Encrypt_AES_GCM(a_transmissionLength, this.CommonSecretPassphrase ?? "", ForestNET.Lib.Cryptography.KEY256BIT);
                    }
                    else if ((this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_LOW) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_LOW))
                    {
                        a_transmissionLength = this.Cryptography?.Encrypt(a_transmissionLength) ?? [];
                    }

                    /* sending transmission length */
                    await this.SendBytes(a_transmissionLength, a_transmissionLength.Length);

                    /* ------------------------------------------------------ */

                    /* receive acknowledge that transmission length has been received correctly */
                    await this.ReceiveACK();
                }
            }
            else if ((this.Type == ForestNET.Lib.Net.Sock.Type.UDP_CLIENT) || (this.Type == ForestNET.Lib.Net.Sock.Type.UDP_SERVER))
            { /* UDP is not supported */
                throw new InvalidOperationException("Not implemented for UDP protocol");
            }
            else
            {
                throw new InvalidOperationException("Not implemented");
            }

            return i_return;
        }

        /// <summary>
        /// Sending acknowledge(ACK) to other communication side
        /// </summary>
        /// <exception cref="InvalidOperationException">not implemented for UDP protocol</exception>
        /// <exception cref="Exception">any other Exception, see below in other classes for details</exception>
        protected async System.Threading.Tasks.Task SendACK()
        {
            /* UDP is not supported */
            if ((this.Type != ForestNET.Lib.Net.Sock.Type.TCP_CLIENT) && (this.Type != ForestNET.Lib.Net.Sock.Type.TCP_SERVER) && (this.Type != ForestNET.Lib.Net.Sock.Type.TCP_TLS_SERVER) && (this.Type != ForestNET.Lib.Net.Sock.Type.TCP_TLS_CLIENT))
            {
                throw new InvalidOperationException("Not implemented for UDP protocol");
            }

            /* byte array for sending ACK */
            byte[] a_ack = new byte[] { BY_ACK_BYTE };

            ForestNET.Lib.Global.ILogFiner("Sending ACK: " + ForestNET.Lib.Helper.PrintByteArray(a_ack, false));

            /* encrypt ACK if encryption is active */
            if (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_HIGH)
            {
                a_ack = ForestNET.Lib.Cryptography.Encrypt_AES_GCM(a_ack, this.CommonSecretPassphrase ?? "", ForestNET.Lib.Cryptography.KEY128BIT);
            }
            else if (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_HIGH)
            {
                a_ack = ForestNET.Lib.Cryptography.Encrypt_AES_GCM(a_ack, this.CommonSecretPassphrase ?? "", ForestNET.Lib.Cryptography.KEY256BIT);
            }
            else if ((this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_LOW) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_LOW))
            {
                a_ack = this.Cryptography?.Encrypt(a_ack) ?? [];
            }

            /* sending ACK */
            await this.SendBytes(a_ack, a_ack.Length);
        }

        /// <summary>
        /// Receiving acknowledge(ACK) from other communication side
        /// </summary>
        /// <exception cref="InvalidOperationException">not implemented for UDP protocol</exception>
        /// <exception cref="Exception">any other Exception, see below in other classes for details</exception>
        protected async System.Threading.Tasks.Task ReceiveACK()
        {
            /* UDP is not supported */
            if ((this.Type != ForestNET.Lib.Net.Sock.Type.TCP_CLIENT) && (this.Type != ForestNET.Lib.Net.Sock.Type.TCP_SERVER) && (this.Type != ForestNET.Lib.Net.Sock.Type.TCP_TLS_SERVER) && (this.Type != ForestNET.Lib.Net.Sock.Type.TCP_TLS_CLIENT))
            {
                throw new InvalidOperationException("Not implemented for UDP protocol");
            }

            /* help variables */
            byte[]? a_ack;
            int i_maxAttempts = 40;
            int i_attempts = 1;

            do
            {
                /* receive ACK, expecting different amount of bytes because of encryption */
                if ((this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_HIGH) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_HIGH))
                {
                    a_ack = await this.ReceiveBytes(33, 33);
                }
                else if ((this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_LOW) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_LOW))
                {
                    a_ack = await this.ReceiveBytes(17, 17);
                }
                else
                {
                    a_ack = await this.ReceiveBytes(1, 1);
                }

                /* decrypt received ACK if encryption is active */
                if (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_HIGH)
                {
                    a_ack = ForestNET.Lib.Cryptography.Decrypt_AES_GCM(a_ack ?? [], this.CommonSecretPassphrase ?? "", ForestNET.Lib.Cryptography.KEY128BIT);
                }
                else if (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_HIGH)
                {
                    a_ack = ForestNET.Lib.Cryptography.Decrypt_AES_GCM(a_ack ?? [], this.CommonSecretPassphrase ?? "", ForestNET.Lib.Cryptography.KEY256BIT);
                }
                else if ((this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_LOW) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_LOW))
                {
                    a_ack = this.Cryptography?.Decrypt(a_ack ?? []) ?? [];
                }

                ForestNET.Lib.Global.ILogFiner("Received ACK: " + ForestNET.Lib.Helper.PrintByteArray(a_ack, false));

                /* received ACK must match expected ACK */
                if ((a_ack != null) && (a_ack[0] != BY_ACK_BYTE))
                {
                    ForestNET.Lib.Global.ILogWarning("Invalid ACK[" + ForestNET.Lib.Helper.PrintByteArray(a_ack, false).Trim() + "], must be [" + ForestNET.Lib.Helper.PrintByteArray(new byte[] { BY_ACK_BYTE }, false).Trim() + "]; retry after 25 milliseconds");

                    /* wait 25 milliseconds to receive length again */
                    await System.Threading.Tasks.Task.Delay(25);

                    if (i_attempts >= i_maxAttempts)
                    { /* all attempts failed, so protocol for receiving ACK failed completely */
                        throw new InvalidOperationException("Receiving ACK failed after " + i_attempts + " attempts");
                    }
                }

                i_attempts++;
            } while ((a_ack != null) && (a_ack[0] != BY_ACK_BYTE));
        }

        /// <summary>
        /// Send data to output stream of socket instance, using buffer length from Task instance
        /// </summary>
        /// <param name="p_a_data">content data which will be uploaded</param>
        /// <exception cref="InvalidOperationException">not implemented for UDP protocol</exception>
        /// <exception cref="System.IO.IOException">issue sending to output stream object instance</exception>
        protected async System.Threading.Tasks.Task SendBytes(byte[] p_a_data)
        {
            await this.SendBytes(p_a_data, this.i_bufferLength);
        }

        /// <summary>
        /// Send data to output stream of socket instance
        /// </summary>
        /// <param name="p_a_data">content data which will be uploaded</param>
        /// <param name="p_i_bufferLength">size of buffer which is used send the output stream</param>
        /// <exception cref="NullReferenceException">socket instance within task is null</exception>
        /// <exception cref="InvalidOperationException">not implemented for UDP protocol</exception>
        /// <exception cref="System.IO.IOException">issue sending to output stream object instance</exception>
        protected async System.Threading.Tasks.Task SendBytes(byte[] p_a_data, int p_i_bufferLength)
        {
            if (this.Socket == null)
            {
                throw new NullReferenceException("socket instance within task is null");
            }

            if (p_i_bufferLength < 1)
            {
                throw new System.IO.IOException("buffer length(sending) must be greater than 0, but it is '" + p_i_bufferLength + "'");
            }

            if ((this.Type == ForestNET.Lib.Net.Sock.Type.TCP_CLIENT) || (this.Type == ForestNET.Lib.Net.Sock.Type.TCP_SERVER) || (this.Type == ForestNET.Lib.Net.Sock.Type.TCP_TLS_SERVER) || (this.Type == ForestNET.Lib.Net.Sock.Type.TCP_TLS_CLIENT))
            {
                /* create sending buffer and help variables */
                int i_sendDataPointer = 0;
                byte[] a_buffer = new byte[p_i_bufferLength];

                /* create clean buffer */
                for (int i = 0; i < p_i_bufferLength; i++)
                {
                    a_buffer[i] = 0;
                }

                int i_cycles = (int)Math.Ceiling(((double)p_a_data.Length / (double)p_i_bufferLength));
                int i_sum = 0;
                int i_progressThreshold = 0;

                if (this.DebugNetworkTrafficOn) ForestNET.Lib.Global.ILogFiner("Iterate '" + i_cycles + "' cycles to transport '" + p_a_data.Length + "' bytes with '" + p_i_bufferLength + "' bytes buffer");

                /* iterate cycles to send bytes with buffer */
                for (int i = 0; i < i_cycles; i++)
                {
                    int i_bytesSend = 0;

                    /* copy data to our buffer until buffer length or overall data length has been reached */
                    for (int j = 0; j < p_i_bufferLength; j++)
                    {
                        if (i_sendDataPointer >= p_a_data.Length)
                        {
                            ForestNET.Lib.Global.ILogFinest("Send data pointer '" + i_sendDataPointer + "' >= amount of total bytes '" + p_a_data.Length + "' to transport");

                            break;
                        }

                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("Writing byte[" + ForestNET.Lib.Helper.PrintByteArray(new byte[] { p_a_data[i_sendDataPointer] }, false) + "] to buffer[" + j + "]");

                        a_buffer[j] = p_a_data[i_sendDataPointer++];
                        i_bytesSend++;
                    }

                    /* create cancellation token with stream write timeout */
                    System.Threading.CancellationToken o_token = new System.Threading.CancellationTokenSource(this.Stream.WriteTimeout).Token;

                    /* send to output stream with buffer counter value */
                    await this.Stream.WriteAsync(a_buffer.AsMemory(0, i_bytesSend), o_token);
                    await this.Stream.FlushAsync(o_token);

                    ForestNET.Lib.Global.ILogFinest("Sended data, cycle '" + (i + 1) + "' of '" + i_cycles + "', amount of bytes: " + i_bytesSend);

                    /* update delegate for progress or own console progress bar instance */
                    if (this.PostProgressDelegate != null)
                    {
                        i_sum += i_bytesSend;
                        i_progressThreshold += i_bytesSend;

                        /* only post progress if a threshold of 1KB has been made */
                        if (i_progressThreshold >= 102400)
                        {
                            this.PostProgressDelegate.Invoke(i_sum, p_a_data.Length);
                            i_progressThreshold -= 102400;
                        }
                    }
                }
            }
            else if ((this.Type == ForestNET.Lib.Net.Sock.Type.UDP_CLIENT) || (this.Type == ForestNET.Lib.Net.Sock.Type.UDP_SERVER))
            {
                throw new InvalidOperationException("Not implemented for UDP protocol");
            }
            else
            {
                throw new InvalidOperationException("Not implemented");
            }
        }

        /// <summary>
        /// Read data from input stream of socket instance, using buffer length from Task instance
        /// </summary>
        /// <param name="p_i_amountBytes">amount of bytes we expect to read from input stream</param>
        /// <returns>input stream content - array of bytes</returns>
        /// <exception cref="NullReferenceException">socket instance within task is null</exception>
        /// <exception cref="InvalidOperationException">not implemented for UDP protocol</exception>
        /// <exception cref="System.IO.IOException">issue reading from input stream object from socket instance</exception>
        protected async System.Threading.Tasks.Task<byte[]?> ReceiveBytes(int p_i_amountBytes)
        {
            return await ReceiveBytes(p_i_amountBytes, this.i_bufferLength);
        }

        /// <summary>
        /// Read data from input stream socket instance
        /// </summary>
        /// <param name="p_i_amountBytes">amount of bytes we expect to read from input stream</param>
        /// <param name="p_i_bufferLength">size of buffer which is used reading the input stream</param>
        /// <returns>input stream content - array of bytes</returns>
        /// <exception cref="NullReferenceException">socket instance within task is null</exception>
        /// <exception cref="InvalidOperationException">not implemented for UDP protocol</exception>
        /// <exception cref="System.IO.IOException">issue reading from input stream object from socket instance</exception>
        protected async System.Threading.Tasks.Task<byte[]?> ReceiveBytes(int p_i_amountBytes, int p_i_bufferLength)
        {
            return await ReceiveBytes(p_i_amountBytes, p_i_bufferLength, false);
        }

        /// <summary>
        /// Read data from input stream socket instance
        /// </summary>
        /// <param name="p_i_amountBytes">amount of bytes we expect to read from input stream</param>
        /// <param name="p_i_bufferLength">size of buffer which is used reading the input stream</param>
        /// <param name="p_b_recursive">true - receiveBytes is called from receiveBytes, false - receiveBytes is called from outside</param>
        /// <returns>input stream content - array of bytes</returns>
        /// <exception cref="NullReferenceException">socket instance within task is null</exception>
        /// <exception cref="InvalidOperationException">not implemented for UDP protocol</exception>
        /// <exception cref="System.IO.IOException">issue reading from input stream object from socket instance</exception>
        private async System.Threading.Tasks.Task<byte[]?> ReceiveBytes(int p_i_amountBytes, int p_i_bufferLength, bool p_b_recursive)
        {
            if (this.Socket == null)
            {
                throw new NullReferenceException("socket instance within task is null");
            }

            if (p_i_bufferLength < 1)
            {
                throw new System.IO.IOException("buffer length(receiving) must be greater than 0, but it is '" + p_i_bufferLength + "'");
            }

            byte[]? a_receivedData;

            if ((this.Type == ForestNET.Lib.Net.Sock.Type.TCP_CLIENT) || (this.Type == ForestNET.Lib.Net.Sock.Type.TCP_SERVER) || (this.Type == ForestNET.Lib.Net.Sock.Type.TCP_TLS_SERVER) || (this.Type == ForestNET.Lib.Net.Sock.Type.TCP_TLS_CLIENT))
            {
                /* if we do not know how much bytes we will read, we have to detect EOT ourselves */
                bool b_readUnknownAmount = false;

                /* if we could not get amount of bytes as parameter we try to get it from input stream */
                if (p_i_amountBytes <= 0)
                {
                    try
                    {
                        p_i_amountBytes = (int)this.Stream.Length;
                    }
                    catch (Exception)
                    {
                        /* this stream does not support seek operations */
                        p_i_amountBytes = -1;
                    }

                    /* if we still have no exact information how many bytes we will receive, we must read from input stream with configured max. limit */
                    if (p_i_amountBytes <= 0)
                    {
                        p_i_amountBytes = this.i_receiveMaxUnknownAmountInMiB * 1024 * 1024;
                        b_readUnknownAmount = true;
                    }
                }

                /* create receiving byte array, buffer and help variables */
                a_receivedData = new byte[p_i_amountBytes];
                int i_receivedDataPointer = 0;
                byte[] a_buffer = new byte[p_i_bufferLength];
                int i_cycles = (int)Math.Ceiling(((double)p_i_amountBytes / (double)p_i_bufferLength));
                int i_sum = 0;
                int i_sumProgress = 0;
                int i_progressThreshold = 0;

                if (this.DebugNetworkTrafficOn) ForestNET.Lib.Global.ILogFiner("Iterate '" + i_cycles + "' cycles to receive '" + p_i_amountBytes + "' bytes with '" + p_i_bufferLength + "' bytes buffer");

                /* iterate cycles to receive bytes with buffer */
                for (int i = 0; i < i_cycles; i++)
                {
                    int i_expectedBytes = p_i_bufferLength;
                    int i_bytesReaded = -1;

                    /* create cancellation token with stream read timeout */
                    System.Threading.CancellationToken o_token = new System.Threading.CancellationTokenSource(this.Stream.ReadTimeout).Token;

                    if (this.DebugNetworkTrafficOn) ForestNET.Lib.Global.ILogFiner("this.Stream.ReadAsync, expecting " + i_expectedBytes + " bytes with read timeout '" + this.Stream.ReadTimeout + " ms'");

                    /* read from input stream until amount of expected bytes has been reached, or we reached EOT */
                    while ((i_expectedBytes > 0) && ((i_bytesReaded = await this.Stream.ReadAsync(a_buffer.AsMemory(0, Math.Min(i_expectedBytes, a_buffer.Length)), o_token)) > 0))
                    {
                        if (this.DebugNetworkTrafficOn) ForestNET.Lib.Global.ILogFiner("this.Stream.ReadAsync, readed " + i_bytesReaded + " bytes of expected " + i_expectedBytes + " bytes");

                        i_expectedBytes -= i_bytesReaded;

                        if (i_bytesReaded < 0)
                        {
                            throw new InvalidOperationException("Could not receive data");
                        }
                        else
                        {
                            /* copy received bytes to return byte array value */
                            for (int j = 0; j < i_bytesReaded; j++)
                            {
                                if (i_receivedDataPointer >= a_receivedData.Length)
                                {
                                    if (this.DebugNetworkTrafficOn) ForestNET.Lib.Global.ILogFinest("Receive data pointer '" + i_receivedDataPointer + "' >= amount of total bytes '" + a_receivedData.Length + "' received");

                                    break;
                                }

                                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("Writing byte[" + ForestNET.Lib.Helper.PrintByteArray(new byte[] { a_buffer[j] }, false) + "] to receivedData[" + i_receivedDataPointer + "]");

                                a_receivedData[i_receivedDataPointer++] = a_buffer[j];
                            }

                            i_sum += i_bytesReaded;

                            if (this.DebugNetworkTrafficOn) ForestNET.Lib.Global.ILogFiner("Received data, cycle '" + (i + 1) + "' of '" + i_cycles + "', amount of bytes readed: " + i_bytesReaded + ", expected bytes: " + i_expectedBytes + ", sum: " + i_sum);
                        }

                        if (this.DebugNetworkTrafficOn) ForestNET.Lib.Global.ILogFiner("do we still miss some bytes after read? -> " + ((i_expectedBytes > 0) && (i_sum != p_i_amountBytes) && (!p_b_recursive)));

                        /* flag to know when tolerance loops are finished */
                        bool b_waitedTolerance = false;

                        /* only if current call is not recursive: we are still expecting some bytes to come, maybe incoming transfer with unknown amount of bytes and some delay in TCP transport */
                        if ((i_expectedBytes > 0) && (i_sum != p_i_amountBytes) && (!p_b_recursive))
                        {
                            if (this.DebugNetworkTrafficOn) ForestNET.Lib.Global.ILogFiner("(i_expectedBytes > 0) -> " + (i_expectedBytes > 0) + "\t(i_sum != p_i_amountBytes) -> " + (i_sum != p_i_amountBytes) + "\t(!p_b_recursive) -> " + (!p_b_recursive));

                            /* prepare receiving buffer for rest of expected bytes */
                            byte[] a_receivedAnotherTry;
                            /* save old timeout value for socket */
                            int i_oldTimeout = this.Stream.ReadTimeout;

                            if (this.DebugNetworkTrafficOn) ForestNET.Lib.Global.ILogFiner("set small timeout value for socket as " + TOLERATING_DELAY_IN_MS + " milliseconds");

                            /* set small timeout value for socket as TOLERATING_DELAY_IN_MS milliseconds */
                            this.Stream.ReadTimeout = TOLERATING_DELAY_IN_MS;

                            /* accept delay communication with a loop with small amount of iterations as tolerance, each loop with TOLERATING_DELAY_IN_MS milliseconds timeout */
                            for (int j = 0; j < this.i_amountCyclesToleratingDelay; j++)
                            {
                                try
                                {
                                    if (this.DebugNetworkTrafficOn) ForestNET.Lib.Global.ILogFiner("receive rest of expected bytes with " + (j + 1) + " cycle of " + this.i_amountCyclesToleratingDelay + " cycles");

                                    /* receive rest of expected bytes */
                                    a_receivedAnotherTry = await this.ReceiveBytes(i_expectedBytes, i_expectedBytes, true) ?? [];

                                    /* copy received bytes to received data buffer */
                                    for (int k = 0; k < a_receivedAnotherTry.Length; k++)
                                    {
                                        if (i_receivedDataPointer >= a_receivedData.Length)
                                        {
                                            if (this.DebugNetworkTrafficOn) ForestNET.Lib.Global.ILogFinest("Receive data pointer '" + i_receivedDataPointer + "' >= amount of total bytes '" + a_receivedData.Length + "' received");

                                            break;
                                        }

                                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("Writing byte[" + ForestNET.Lib.Helper.PrintByteArray(new byte[] { a_buffer[j] }, false) + "] to receivedData[" + i_receivedDataPointer + "]");

                                        a_receivedData[i_receivedDataPointer++] = a_receivedAnotherTry[k];
                                    }

                                    /* update variable for bytes readed, sum and expected bytes */
                                    i_bytesReaded += a_receivedAnotherTry.Length;
                                    i_sum += a_receivedAnotherTry.Length;
                                    i_expectedBytes -= a_receivedAnotherTry.Length;

                                    /* if amount of missing expected bytes have been received, we do not need to continue our tolerance loop */
                                    if (i_expectedBytes < 1)
                                    {
                                        if (this.DebugNetworkTrafficOn) ForestNET.Lib.Global.ILogFiner("amount of missing expected bytes have been received, we do not need to continue our tolerance loop");

                                        break;
                                    }
                                }
                                catch (Exception)
                                {
                                    /* ignore read timeout or any failed receive */
                                }
                            }

                            b_waitedTolerance = true;

                            /* restore old timeout value for socket */
                            this.Stream.ReadTimeout = i_oldTimeout;
                        }

                        /* we are still have expected bytes to receive and waited all tolerance cycles -> the transfer might be finished and we can break the while loop */
                        if ((i_expectedBytes > 0) && (b_waitedTolerance))
                        {
                            break;
                        }

                        /* if we reached our amount of bytes we expect to read, we can set expected bytes counter to zero */
                        if (i_sum == p_i_amountBytes)
                        {
                            i_expectedBytes = 0;
                        }
                    }

                    /* update delegate for progress or own console progress bar instance */
                    if (this.PostProgressDelegate != null)
                    {
                        i_sumProgress += i_bytesReaded;
                        i_progressThreshold += i_bytesReaded;

                        /* only post progress if a threshold of 1KB has been made */
                        if (i_progressThreshold >= 102400)
                        {
                            this.PostProgressDelegate.Invoke(i_sumProgress, p_i_amountBytes);
                            i_progressThreshold -= 102400;
                        }
                    }

                    /* we are still have expected bytes to receive and unknown amount of incoming data + tried tolerance delay or recursive call -> the transfer might be finished and we can break the for loop as well */
                    if ((i_expectedBytes > 0) && ((b_readUnknownAmount) || (p_b_recursive)))
                    {
                        break;
                    }
                }

                /* we have read less bytes than we expected */
                if (p_i_amountBytes > i_sum)
                {
                    ForestNET.Lib.Global.ILogFiner("We have read less bytes than we expected: '" + p_i_amountBytes + "' > '" + i_sum + "'");

                    /* new return byte array with correct length */
                    byte[] a_trimmedReceivedData = new byte[i_sum];

                    /* trim byte array data */
                    for (int i = 0; i < i_sum; i++)
                    {
                        a_trimmedReceivedData[i] = a_receivedData[i];
                    }

                    ForestNET.Lib.Global.ILogFiner("Created new return byte array with correct length: '" + i_sum + "'");

                    return a_trimmedReceivedData;
                }
            }
            else if ((this.Type == ForestNET.Lib.Net.Sock.Type.UDP_CLIENT) || (this.Type == ForestNET.Lib.Net.Sock.Type.UDP_SERVER))
            {
                throw new InvalidOperationException("Not implemented for UDP protocol");
            }
            else
            {
                throw new InvalidOperationException("Not implemented");
            }

            return a_receivedData;
        }

        /// <summary>
        /// method to log all information about message with log-level FINEST for message details and log-level MASS for each byte of message data content
        /// only available if b_debugNetworkTrafficOn is set to true
        /// </summary>
        /// <param name="p_a_messageBytes">byte array of message</param>
        /// <param name="p_o_message">message object</param>
        protected void DebugMessage(byte[] p_a_messageBytes, ForestNET.Lib.Net.Msg.Message p_o_message)
        {
            /* only if b_debugNetworkTrafficOn is set to true */
            if (this.DebugNetworkTrafficOn)
            {
                /* log message details */
                ForestNET.Lib.Global.ILogFinest("\tMessageBoxId: " + ForestNET.Lib.Helper.PrintByteArray(new byte[] { p_a_messageBytes[0], p_a_messageBytes[1] }, false));
                ForestNET.Lib.Global.ILogFinest("\tMessageAmount: " + ForestNET.Lib.Helper.PrintByteArray(new byte[] { p_a_messageBytes[2], p_a_messageBytes[3] }, false));
                ForestNET.Lib.Global.ILogFinest("\tMessageNumber: " + ForestNET.Lib.Helper.PrintByteArray(new byte[] { p_a_messageBytes[4], p_a_messageBytes[5] }, false));
                ForestNET.Lib.Global.ILogFinest("\tMessageTypeLength: " + ForestNET.Lib.Helper.PrintByteArray(new byte[] { p_a_messageBytes[6] }, false));

                byte[] a_stringBytes = new byte[p_a_messageBytes[6]];

                /* log each byte of message data content */
                for (int j = 0; j < p_a_messageBytes[6]; j++)
                {
                    a_stringBytes[j] = p_a_messageBytes[j + 7];
                    if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("\t\tMessageType Character[" + j + " + 7]: " + System.Text.Encoding.UTF8.GetString(new byte[] { p_a_messageBytes[j + 7] }) + " [" + ForestNET.Lib.Helper.PrintByteArray(new byte[] { p_a_messageBytes[j + 7] }, false) + "]");
                }

                ForestNET.Lib.Global.ILogFinest("\tMessageType: " + System.Text.Encoding.UTF8.GetString(a_stringBytes));
                ForestNET.Lib.Global.ILogFinest("\tMessageDataLength: " + p_o_message.DataLength);
            }
        }

        /// <summary>
        /// method to trim all zero bytes from an UDP datagram byte packet
        /// </summary>
        /// <param name="p_a_datagramBytes">datagram byte packet</param>
        /// <returns>trimmed UDP datagram bytes</returns>
        protected static byte[] UDPDatagramBytesTrimEnd(byte[]? p_a_datagramBytes)
        {
            Array.Resize(ref p_a_datagramBytes, (Array.FindLastIndex(p_a_datagramBytes ?? [], b => b != 0) + 1));
            return p_a_datagramBytes;
        }

        /// <summary>
        /// Method to use a tcp socket for bidirectional communication, using first message box for sending data and the second message box for receiving data
        /// </summary>
        /// <exception cref="Exception">any other Exception, see below in other classes for details</exception>
        protected async System.Threading.Tasks.Task TCPBidirectional()
        {
            /* check if message boxes list is initiated */
            if (this.MessageBoxes == null)
            {
                throw new ArgumentNullException("Message boxes are null");
            }

            /* keep socket task in endless loop for bidirectional communication, connection will be closed when communication object or socket task object is closed */
            while ((!(this.CancellationToken?.IsCancellationRequested ?? true)))
            {
                /* check first message box of socket task for messages to send */
                ForestNET.Lib.Net.Msg.MessageBox o_messageBox = this.MessageBoxes[0];

                /* get message of current message box */
                ForestNET.Lib.Net.Msg.Message? o_message = o_messageBox.CurrentMessage();

                /* if we got a message we want to send it */
                if (o_message != null)
                {
                    /* send TCP packet: just dequeue one message of current message box */
                    o_message = o_messageBox.DequeueMessage();

                    if (o_message != null)
                    {
                        /* send message with TCP protocol */
                        _ = await this.TCPSend(o_message);
                    }
                    else
                    {
                        throw new NullReferenceException("Could not dequeue message, result is null");
                    }
                }

                try
                {
                    /* receive TCP packet */
                    await this.TCPReceive();
                }
                catch (Exception)
                {
                    /* skip timeout */
                }
            }
        }

        /// <summary>
        /// Send one message from a message box over TCP to other communication side
        /// </summary>
        /// <param name="p_o_message">message object from a message box</param>
        /// <returns>true - data sent successfully, false - something went wrong</returns>
        /// <exception cref="Exception">any other Exception, see below in other classes for details</exception>
        protected async System.Threading.Tasks.Task<bool> TCPSend(ForestNET.Lib.Net.Msg.Message p_o_message)
        {
            /* convert complete message parameter into byte array */
            byte[] a_bytes = p_o_message.GetByteArrayFromMessage();

            ForestNET.Lib.Global.ILogFine("Send message to[" + this.Socket?.SocketAddress + "]: message number[" + p_o_message.MessageNumber + "] of [" + p_o_message.MessageAmount + "] with length[" + a_bytes.Length + "], data length[" + p_o_message.DataLength + "], data block length[" + p_o_message.DataBlockLength + "], message box id[" + p_o_message.MessageBoxId + "]");

            /* debug sending message if b_debugNetworkTrafficOn is set to true */
            this.DebugMessage(a_bytes, p_o_message);

            /* encrypt sending bytes if encryption is active */
            if (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_HIGH)
            {
                a_bytes = ForestNET.Lib.Cryptography.Encrypt_AES_GCM(a_bytes, this.CommonSecretPassphrase ?? "", ForestNET.Lib.Cryptography.KEY128BIT);
            }
            else if (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_HIGH)
            {
                a_bytes = ForestNET.Lib.Cryptography.Encrypt_AES_GCM(a_bytes, this.CommonSecretPassphrase ?? "", ForestNET.Lib.Cryptography.KEY256BIT);
            }
            else if ((this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_LOW) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_LOW))
            {
                a_bytes = this.Cryptography?.Encrypt(a_bytes) ?? [];
            }

            /* send bytes over TCP */
            await this.SendBytes(a_bytes);

            return true;
        }

        /// <summary>
        /// Method for handling receiving bytes from a TCP connection
        /// </summary>
        /// <exception cref="Exception">any other Exception, see below in other classes for details</exception>
        protected async System.Threading.Tasks.Task TCPReceive()
        {
            /* check if message boxes list is initiated */
            if (this.MessageBoxes == null)
            {
                throw new ArgumentNullException("Message boxes are null");
            }

            /* receive message data bytes */
            byte[]? a_receivedData = await this.ReceiveBytes(this.BufferLength);

            ForestNET.Lib.Global.ILogFiner("Received socket packet, length = " + (a_receivedData?.Length ?? 0) + " bytes");

            /* if we received no data we have no message information as well */
            if ((a_receivedData == null) || (a_receivedData.Length < 1))
            {
                return;
            }

            int i_decreaseForEncryption = 0;

            /* decrypt received bytes if encryption is active */
            if (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_HIGH)
            {
                a_receivedData = ForestNET.Lib.Cryptography.Decrypt_AES_GCM(a_receivedData, this.CommonSecretPassphrase ?? "", ForestNET.Lib.Cryptography.KEY128BIT);
                i_decreaseForEncryption = 28;
            }
            else if (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_HIGH)
            {
                a_receivedData = ForestNET.Lib.Cryptography.Decrypt_AES_GCM(a_receivedData, this.CommonSecretPassphrase ?? "", ForestNET.Lib.Cryptography.KEY256BIT);
                i_decreaseForEncryption = 28;
            }
            else if ((this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_LOW) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_LOW))
            {
                a_receivedData = this.Cryptography?.Decrypt(a_receivedData);
                i_decreaseForEncryption = 16;
            }

            ForestNET.Lib.Global.ILogFiner("received data (optional already decrypted), length = " + (a_receivedData?.Length ?? 0) + " bytes");

            /* create message object with message length and message data */
            ForestNET.Lib.Net.Msg.Message o_message = new(this.BufferLength - i_decreaseForEncryption);
            o_message.SetMessageFromByteArray(a_receivedData ?? []);

            ForestNET.Lib.Global.ILogFine("Received message from[" + this.ReceivingSocket?.RemoteEndPoint + "]: message number[" + o_message.MessageNumber + "] of [" + o_message.MessageAmount + "] with length[" + o_message.MessageLength + "], data length[" + o_message.DataLength + "], data block length[" + o_message.DataBlockLength + "], message box id[" + o_message.MessageBoxId + "]");

            /* debug received message if b_debugNetworkTrafficOn is set to true */
            this.DebugMessage(a_receivedData ?? [], o_message);

            int i_messageBoxId = 0;

            /* determine message box id if we have a cardinality with many message boxes and one socket */
            if (this.CommunicationCardinality == ForestNET.Lib.Net.Sock.Com.Cardinality.ManyMessageBoxesToOneSocket)
            {
                i_messageBoxId = o_message.MessageBoxId;

                /* check message box id min. value */
                if (i_messageBoxId < 1)
                {
                    throw new ArgumentException("Invalid receiving message. Message box id[" + i_messageBoxId + "] must be greater than [0].");
                }

                /* check message box id max. value */
                if (i_messageBoxId > this.MessageBoxes.Count)
                {
                    throw new ArgumentException("Invalid receiving message. Message box id[" + i_messageBoxId + "] is not available.");
                }

                i_messageBoxId--;
            }

            /* use second message box for receiving data for bidirectional communication */
            if (this.CommunicationCardinality == ForestNET.Lib.Net.Sock.Com.Cardinality.EqualBidirectional)
            {
                i_messageBoxId = 1;
            }

            /* enqueue message object */
            while (!this.MessageBoxes[i_messageBoxId].EnqueueMessage(o_message))
            {
                ForestNET.Lib.Global.ILogWarning("Could not enqueue message object, timeout for '" + this.QueueTimeoutMilliseconds + "' milliseconds");

                /* wait queue timeout length to enqueue message object again */
                await System.Threading.Tasks.Task.Delay(this.QueueTimeoutMilliseconds, this.CancellationToken ?? default);
            }
            ;
        }
    }
}