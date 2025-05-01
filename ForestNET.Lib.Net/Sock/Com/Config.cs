namespace ForestNET.Lib.Net.Sock.Com
{
    /// <summary>
    /// Configuration class for communication object for network communication exchange. All configurable settings are listed and adjustable in this class. Please look on the comments of the set-property-methods to see further details.
    /// Special features in handling shared memory objects and bidirectional shared memory settings, so that all changes in fields of inherited class of shared memory object are transferred over the network to the other communication side automatically.
    /// </summary>
    public class Config
    {

        /* Fields */

        private Type e_communicationType;
        private ForestNET.Lib.Net.Sock.Recv.ReceiveType? e_socketReceiveType;
        private Cardinality e_cardinality;

        private Security? e_security;
        private string? s_commonSecretPassphrase;
        private string? s_pathToCertificateFile;
        private string? s_pathToCertificateFilePassword;
        private string? s_certificateThumbprint;
        private System.Security.Cryptography.X509Certificates.StoreName? e_certificateStoreName;
        private System.Security.Cryptography.X509Certificates.StoreLocation? e_certificateStoreLocation;
        private string? s_clientRemoteCertificateName;
        private List<System.Security.Cryptography.X509Certificates.X509Certificate>? a_clientCertificateAllowList;

        private int i_amountMessageBoxes;
        private int i_amountSockets;
        private List<int>? a_messageBoxLengths;
        private List<string>? a_hosts;
        private List<int>? a_ports;

        private List<ForestNET.Lib.Net.Sock.Task.Task>? a_socketTasks;

        private int i_socketServicePoolAmount;
        private int i_receiverTimeoutMilliseconds;
        private int i_senderTimeoutMilliseconds;
        private bool b_checkReachability;
        private int i_maxTerminations;
        private int i_senderIntervalMilliseconds;
        private int i_queueTimeoutMilliseconds;

        private int i_udpReceiveAckTimeoutMilliseconds;
        private int i_udpSendAckTimeoutMilliseconds;

        private bool b_udpIsMulticastSocket;
        private int i_udpMulticastTTL;

        private string? s_localAddress;
        private int i_localPort;

        private bool b_objectTransmission;

        private List<ForestNET.Lib.Net.Sock.Task.Task>? a_receiveSocketTasks;

        private ISharedMemory? o_sharedMemory;
        private int i_sharedMemoryTimeoutMilliseconds;
        private ForestNET.Lib.DateInterval? o_sharedMemoryIntervalCompleteRefresh;
        private List<string>? a_biHosts;
        private List<int>? a_biPorts;
        private Config? o_sharedMemoryBidirectionalConfig;
        private bool b_sharedMemoryBidirectionalConfigSet;

        private bool b_closed;

        private bool b_debugNetworkTrafficOn;

        private bool b_useMarshalling;
        private bool b_useMarshallingWholeObject;
        private int i_marshallingDataLengthInBytes;
        private bool b_marshallingUseProperties;
        private string? s_marshallingOverrideMessageType;
        private bool b_marshallingSystemUsesLittleEndian;

        private List<ForestNET.Lib.Net.Sock.Task.Task.PostProgress>? del_postProgresses;

        /* Properties */

        public Type CommunicationType
        {
            get
            {
                return this.e_communicationType;
            }
            private set
            {
                this.e_communicationType = value;
            }
        }
        /// <summary>
        /// determine receive type for all sockets: SOCKET or SERVER
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been closed, because bidirectional setting was finished</exception>
        public ForestNET.Lib.Net.Sock.Recv.ReceiveType? SocketReceiveType
        {
            get
            {
                return this.e_socketReceiveType;
            }
            set
            {
                this.CheckClosed();
                this.e_socketReceiveType = value;
            }
        }
        public Cardinality Cardinality
        {
            get
            {
                return this.e_cardinality;
            }
            private set
            {
                this.e_cardinality = value;
            }
        }
        /// <summary>
        /// determine communication security for socket
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished</exception>
        /// <exception cref="ArgumentException">communication security is not supported for UDP communication</exception>
        public Security? CommunicationSecurity
        {
            get
            {
                return this.e_security;
            }
            set
            {
                this.CheckClosed();

                if ((value == Security.ASYMMETRIC) && (
                    this.e_communicationType == Type.UDP_SEND ||
                    this.e_communicationType == Type.UDP_RECEIVE ||
                    this.e_communicationType == Type.UDP_SEND_WITH_ACK ||
                    this.e_communicationType == Type.UDP_RECEIVE_WITH_ACK
                ))
                {
                    throw new ArgumentException("Cannot use communication security[" + Security.ASYMMETRIC + "] with communication type[" + this.e_communicationType + "]");
                }

                this.e_security = value;
            }
        }
        /// <summary>
        /// common secret passphrase for low security communication
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished</exception>
        /// <exception cref="ArgumentException">secret passphrase is null or not long enough (at least 36 characters)</exception>
        public string? CommonSecretPassphrase
        {
            get
            {
                return this.s_commonSecretPassphrase;
            }
            set
            {
                this.CheckClosed();

                if ((value == null) || (ForestNET.Lib.Helper.IsStringEmpty(value)))
                {
                    throw new ArgumentException("Common secret passphrase is null");
                }

                if (value.Length < 36)
                {
                    throw new ArgumentException("Common secret passphrase must have at least '36' characters, but has '" + value.Length + "' characters");
                }

                this.s_commonSecretPassphrase = value;
            }
        }
        /// <summary>
        /// path to certificate file for asymmetric security communication
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished</exception>
        /// <exception cref="ArgumentException">path to certificate file is null or file does not exist</exception>
        public string? PathToCertificateFile
        {
            get
            {
                return this.s_pathToCertificateFile;
            }
            set
            {
                this.CheckClosed();

                if ((value == null) || (ForestNET.Lib.Helper.IsStringEmpty(value)))
                {
                    throw new ArgumentException("Path to certificate file is null");
                }

                if (!ForestNET.Lib.IO.File.Exists(value))
                {
                    throw new ArgumentException("Certificate file[" + value + "] does not exist");
                }

                this.s_pathToCertificateFile = value;
            }
        }
        /// <summary>
        /// password to access certificate file for asymmetric security communication
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished</exception>
        /// <exception cref="ArgumentException">password is null</exception>
        public string? PathToCertificateFilePassword
        {
            get
            {
                return this.s_pathToCertificateFilePassword;
            }
            set
            {
                this.CheckClosed();

                if ((value == null) || (ForestNET.Lib.Helper.IsStringEmpty(value)))
                {
                    throw new ArgumentException("Path to certificate file is null");
                }

                this.s_pathToCertificateFilePassword = value;
            }
        }
        /// <summary>
        /// certificate thumbprint to find certificate in internal store for asymmetric security communication
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished</exception>
        /// <exception cref="ArgumentException">certificate thumbprint is null</exception>
        public string? CertificateThumbprint
        {
            get
            {
                return this.s_certificateThumbprint;
            }
            set
            {
                this.CheckClosed();

                if ((value == null) || (ForestNET.Lib.Helper.IsStringEmpty(value)))
                {
                    throw new ArgumentException("Certificate thumbprint file is null");
                }

                this.s_certificateThumbprint = value;
            }
        }
        /// <summary>
        /// certificate store name to find certificate in internal store for asymmetric security communication
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished</exception>
        /// <exception cref="ArgumentException">certificate store name is null</exception>
        public System.Security.Cryptography.X509Certificates.StoreName? CertificateStoreName
        {
            get
            {
                return this.e_certificateStoreName;
            }
            set
            {
                this.CheckClosed();
                this.e_certificateStoreName = value ?? throw new ArgumentException("Certificate store name is null");
            }
        }
        /// <summary>
        /// certificate store location to find certificate in internal store for asymmetric security communication
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished</exception>
        /// <exception cref="ArgumentException">certificate store location is null</exception>
        public System.Security.Cryptography.X509Certificates.StoreLocation? CertificateStoreLocation
        {
            get
            {
                return this.e_certificateStoreLocation;
            }
            set
            {
                this.CheckClosed();
                this.e_certificateStoreLocation = value ?? throw new ArgumentException("Certificate store location is null");
            }
        }
        /// <summary>
        /// certificate name which client receive during communication for asymmetric security
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished</exception>
        /// <exception cref="ArgumentException">client remote certificate name is null</exception>
        public string? ClientRemoteCertificateName
        {
            get
            {
                return this.s_clientRemoteCertificateName;
            }
            set
            {
                this.CheckClosed();

                if ((value == null) || (ForestNET.Lib.Helper.IsStringEmpty(value)))
                {
                    throw new ArgumentException("Client remote certificate name is null");
                }

                this.s_clientRemoteCertificateName = value;
            }
        }
        /// <summary>
        /// client certificate allow list for asymmetric security communication
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished</exception>
        /// <exception cref="ArgumentException">client certificate allow list is null</exception>
        public List<System.Security.Cryptography.X509Certificates.X509Certificate>? ClientCertificateAllowList
        {
            get
            {
                return this.a_clientCertificateAllowList;
            }
            set
            {
                this.CheckClosed();
                this.a_clientCertificateAllowList = value ?? throw new ArgumentException("Client certificate allow list is null");
            }
        }
        /// <summary>
        /// determine amount of sockets and message boxes for communication
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished or shared memory object has been set</exception>
        /// <exception cref="ArgumentException">amount parameter must be greater than '0'</exception>
        public int Amount
        {
            get
            {
                if (this.i_amountMessageBoxes != this.i_amountSockets)
                {
                    return 0;
                }
                else
                {
                    return this.i_amountMessageBoxes;
                }
            }
            set
            {
                this.CheckClosed();
                this.CheckSharedMemoryIsset("amount");

                if (value < 1)
                {
                    throw new ArgumentException("Amount must be at least '1', but was set to '" + value + "'");
                }

                this.i_amountMessageBoxes = value;
                this.i_amountSockets = value;
            }
        }
        /// <summary>
        /// determine amount of message boxes for communication
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished or shared memory object has been set</exception>
        /// <exception cref="ArgumentException">amount parameter must be greater than '0'</exception>
        public int AmountMessageBoxes
        {
            get
            {
                return this.i_amountMessageBoxes;
            }
            set
            {
                this.CheckClosed();
                this.CheckSharedMemoryIsset("amount message boxes");

                if (value < 1)
                {
                    throw new ArgumentException("Amount of message boxes must be at least '1', but was set to '" + value + "'");
                }

                this.i_amountMessageBoxes = value;
            }
        }
        /// <summary>
        /// determine amount of sockets for communication
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished or shared memory object has been set</exception>
        /// <exception cref="ArgumentException">amount parameter must be greater than '0'</exception>
        public int AmountSockets
        {
            get
            {
                return this.i_amountSockets;
            }
            set
            {
                this.CheckClosed();
                this.CheckSharedMemoryIsset("amount sockets");

                if (value < 1)
                {
                    throw new ArgumentException("Amount of sockets must be at least '1', but was set to '" + value + "'");
                }

                this.i_amountSockets = value;
            }
        }
        /// <summary>
        /// list of lengths for all message boxes [1500 or 8192]
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished</exception>
        /// <exception cref="ArgumentNullException">parameter is null</exception>
        /// <exception cref="ArgumentException">message box length must be '1500' or '8192'</exception>
        public List<int>? MessageBoxLengths
        {
            get
            {
                return this.a_messageBoxLengths;
            }
            set
            {
                this.CheckClosed();

                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "List of message box lengths is null");
                }

                /* check each message box length in parameter list */
                foreach (int i in value)
                {
                    if ((i != 1500) && (i != 8192))
                    {
                        throw new ArgumentException("Invalid message box length[" + i + "] in parameter list. Valid values are [1500, 8192]");
                    }
                }

                this.a_messageBoxLengths = value;
            }
        }
        public List<string>? Hosts
        {
            get
            {
                return this.a_hosts;
            }
            private set
            {
                this.a_hosts = value;
            }
        }
        public List<int>? Ports
        {
            get
            {
                return this.a_ports;
            }
            private set
            {
                this.a_ports = value;
            }
        }
        /// <summary>
        /// list of pairs, hosts and ports parameter list for every socket, auto check for duplicates
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished</exception>
        /// <exception cref="ArgumentNullException">parameter value is null or a host value is null</exception>
        /// <exception cref="ArgumentException">invalid port value, must be between 1..65535</exception>
        /// <exception cref="InvalidDataException">duplicate found in parameter list</exception>
        public Dictionary<string, int>? HostsAndPorts
        {
            set
            {
                this.CheckClosed();

                /* check parameter value */
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "List parameter of hosts and ports for every socket is null");
                }

                this.a_hosts = [];
                this.a_ports = [];

                List<string> a_checkDuplicates = [];
                int i = 0;

                /* iterate each pair of host and port in parameter list */
                foreach (KeyValuePair<string, int> o_pair in value)
                {
                    i++;

                    /* check if host value is not empty */
                    if (ForestNET.Lib.Helper.IsStringEmpty(o_pair.Key))
                    {
                        throw new ArgumentNullException("Host #" + i + " is null");
                    }

                    /* check port min. value */
                    if (o_pair.Value < 1)
                    {
                        throw new ArgumentException("Port #" + i + " must be at least '1', but was set to '" + o_pair.Value + "'");
                    }

                    /* check port max. value */
                    if (o_pair.Value > 65535)
                    {
                        throw new ArgumentException("Port #" + i + " must be lower equal '65535', but was set to '" + o_pair.Value + "'");
                    }

                    string s_duplicate = o_pair.Key + ":" + o_pair.Value;

                    /* check for existing duplicate */
                    if (a_checkDuplicates.Contains(s_duplicate))
                    {
                        this.a_hosts = [];
                        this.a_ports = [];
                        throw new InvalidDataException("Duplicate found[" + s_duplicate + "] in list of target hosts and ports which is not allowed");
                    }

                    /* add value to temp list for checking duplicates */
                    a_checkDuplicates.Add(s_duplicate);

                    this.a_hosts.Add(o_pair.Key);
                    this.a_ports.Add(o_pair.Value);
                }
            }
        }
        /// <summary>
        /// list of socket tasks for every socket instance in communication
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished</exception>
        /// <exception cref="ArgumentNullException">parameter value is null</exception>
        public List<ForestNET.Lib.Net.Sock.Task.Task>? SocketTasks
        {
            get
            {
                return this.a_socketTasks;
            }
            set
            {
                this.CheckClosed();

                /* check parameter value */
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "List of socket tasks parameter is null");
                }

                int i = 1;

                /* check if any socket task in parameter list is null */
                foreach (ForestNET.Lib.Net.Sock.Task.Task o_foo in value)
                {
                    if (o_foo == null)
                    {
                        throw new ArgumentNullException("Socket task #" + i++ + " in parameter list is null");
                    }
                }

                this.a_socketTasks = value;
            }
        }
        /// <summary>
        /// integer value for fixed amount of tasks for task pool instance of socket object
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished</exception>
        /// <exception cref="ArgumentException">negative integer value as parameter</exception>
        public int SocketServicePoolAmount
        {
            get
            {
                return this.i_socketServicePoolAmount;
            }
            set
            {
                this.CheckClosed();

                if (value < 0)
                {
                    throw new ArgumentException("Socket executor service pool amount must be at least '0', but was set to '" + value + "'");
                }

                this.i_socketServicePoolAmount = value;
            }
        }
        /// <summary>
        /// integer value for receiver timeout in milliseconds of socket object - how long will a receive socket block execution
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished</exception>
        /// <exception cref="ArgumentException">invalid parameter value</exception>
        public int ReceiverTimeoutMilliseconds
        {
            get
            {
                return this.i_receiverTimeoutMilliseconds;
            }
            set
            {
                this.CheckClosed();

                if (value < 1)
                {
                    throw new ArgumentException("Receiver timeout must be at least '1' millisecond, but was set to '" + value + "' millisecond(s)");
                }

                this.i_receiverTimeoutMilliseconds = value;
            }
        }
        /// <summary>
        /// integer value for sender timeout in milliseconds of socket object - how long will a sender socket will wait until connection has been established
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished</exception>
        /// <exception cref="ArgumentException">invalid parameter value</exception>
        public int SenderTimeoutMilliseconds
        {
            get
            {
                return this.i_senderTimeoutMilliseconds;
            }
            set
            {
                this.CheckClosed();

                if (value < 1)
                {
                    throw new ArgumentException("Sender timeout must be at least '1' millisecond, but was set to '" + value + "' millisecond(s)");
                }

                this.i_senderTimeoutMilliseconds = value;
            }
        }
        /// <summary>
        /// only for sending socket instances<br/>
        /// true - check if destination is reachable, false - do not check reachability
        /// </summary>
        public bool CheckReachability
        {
            get
            {
                return this.b_checkReachability;
            }
            set
            {
                this.b_checkReachability = value;
            }
        }
        /// <summary>
        /// set a max. value for thread executions of socket instance
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished</exception>
        public int MaxTerminations
        {
            get
            {
                return this.i_maxTerminations;
            }
            set
            {
                this.CheckClosed();

                if (value < 0)
                {
                    value = -1;
                }

                this.i_maxTerminations = value;
            }
        }
        /// <summary>
        /// interval for waiting for other communication side or new data to send
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished</exception>
        public int SenderIntervalMilliseconds
        {
            get
            {
                return this.i_senderIntervalMilliseconds;
            }
            set
            {
                this.CheckClosed();

                if (value < 1)
                {
                    value = 0;
                }

                this.i_senderIntervalMilliseconds = value;
            }
        }
        /// <summary>
        /// determine timeout in milliseconds for sending/receiving bytes in socket task instances
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished</exception>
        /// <exception cref="ArgumentException">invalid parameter value</exception>
        public int QueueTimeoutMilliseconds
        {
            get
            {
                return this.i_queueTimeoutMilliseconds;
            }
            set
            {
                this.CheckClosed();

                if (value < 1)
                {
                    throw new ArgumentException("Queue timeout must be at least '1' millisecond, but was set to '" + value + "' millisecond(s)");
                }

                this.i_queueTimeoutMilliseconds = value;
            }
        }
        /// <summary>
        /// determine timeout in milliseconds for sending socket task to receive an acknowledge message via UDP
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished</exception>
        /// <exception cref="ArgumentException">invalid parameter value</exception>
        public int UDPReceiveAckTimeoutMilliseconds
        {
            get
            {
                return this.i_udpReceiveAckTimeoutMilliseconds;
            }
            set
            {
                this.CheckClosed();

                if (value < 1)
                {
                    throw new ArgumentException("UDP receive ACK timeout must be at least '1' millisecond, but was set to '" + value + "' millisecond(s)");
                }

                this.i_udpReceiveAckTimeoutMilliseconds = value;
            }
        }
        /// <summary>
        /// determine timeout in milliseconds for receiving socket task before sending an acknowledge message via UDP - wait some time so the other side of communication can prepare to receive UDP acknowledge message
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished</exception>
        /// <exception cref="ArgumentException">invalid parameter value</exception>
        public int UDPSendAckTimeoutMilliseconds
        {
            get
            {
                return this.i_udpSendAckTimeoutMilliseconds;
            }
            set
            {
                this.CheckClosed();

                if (value < 1)
                {
                    throw new ArgumentException("UDP send ACK timeout must be at least '1' millisecond, but was set to '" + value + "' millisecond(s)");
                }

                this.i_udpSendAckTimeoutMilliseconds = value;
            }
        }
        /// <summary>
        /// true - UDP socket will act as multicast receiver or sender, false - normal UDP socket
        /// </summary>
        public bool UDPIsMulticastSocket
        {
            get
            {
                return this.b_udpIsMulticastSocket;
            }
            set
            {
                this.b_udpIsMulticastSocket = value;
            }
        }
        /// <summary>
        /// only for sending socket instances<br/>
        /// By specifying the TTL, we can therefore limit the lifetime of the packet and thus the distance it can travel. Default is '1'.
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished</exception>
        /// <exception cref="ArgumentException">invalid parameter value</exception>
        public int UDPMulticastTTL
        {
            get
            {
                return this.i_udpMulticastTTL;
            }
            set
            {
                this.CheckClosed();

                if (value < 1)
                {
                    throw new ArgumentException("UDP multicast TTL must be at least '1', but was set to '" + value + "'");
                }

                this.i_udpMulticastTTL = value;
            }
        }
        /// <summary>
        /// set local address for sending socket instances
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished</exception>
        public string? LocalAddress
        {
            get
            {
                return this.s_localAddress;
            }
            set
            {
                this.CheckClosed();
                this.s_localAddress = value;
            }
        }
        /// <summary>
        /// set local port for sending socket instances, must be between [1..65535]
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished</exception>
        /// <exception cref="ArgumentException">invalid port value, must be between 1..65535</exception>
        public int LocalPort
        {
            get
            {
                return this.i_localPort;
            }
            set
            {
                this.CheckClosed();

                if (value < 1)
                {
                    throw new ArgumentException("Local port must be at least '1', but was set to '" + value + "'");
                }

                if (value > 65535)
                {
                    throw new ArgumentException("Local port must be lower equal '65535', but was set to '" + value + "'");
                }

                this.i_localPort = value;
            }
        }
        /// <summary>
        /// true - send/receive TCP packets, but a whole object, so several messages of a message box until the object has been transferred, false - send/receive TCP packets
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished</exception>
        public bool ObjectTransmission
        {
            get
            {
                return this.b_objectTransmission;
            }
            set
            {
                this.CheckClosed();
                this.b_objectTransmission = value;
            }
        }
        /// <summary>
        /// true - show detailed network traffic in internal log
        /// </summary>
        public bool DebugNetworkTrafficOn
        {
            get
            {
                return this.b_debugNetworkTrafficOn;
            }
            set
            {
                this.CheckClosed();
                this.b_debugNetworkTrafficOn = value;
            }
        }
        /// <summary>
        /// true - use marshalling methods to transport data over network
        /// </summary>
        public bool UseMarshalling
        {
            get
            {
                return this.b_useMarshalling;
            }
            set
            {
                this.b_useMarshalling = value;
            }
        }
        /// <summary>
        /// true - use marshalling methods for whole parameter object to transport data over network, especially with shared memory all fields will be transported within a cycle
        /// </summary>
        public bool UseMarshallingWholeObject
        {
            get
            {
                return this.b_useMarshallingWholeObject;
            }
            set
            {
                this.b_useMarshallingWholeObject = value;
            }
        }
        /// <summary>
        /// set data length in bytes for marshalling, must be between [1..4]
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished</exception>
        /// <exception cref="ArgumentException">invalid value, must be between 1..4</exception>
        public int MarshallingDataLengthInBytes
        {
            get
            {
                return this.i_marshallingDataLengthInBytes;
            }
            set
            {
                this.CheckClosed();

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
        /// <summary>
        /// true - access object parameter fields via properties
        /// </summary>
        public bool MarshallingUseProperties
        {
            get
            {
                return this.b_marshallingUseProperties;
            }
            set
            {
                this.b_marshallingUseProperties = value;
            }
        }
        /// <summary>
        /// override message type with this string and do not get it automatically from object, thus the type can be set generally from other systems with other programming languages
        /// </summary>
        public string? MarshallingOverrideMessageType
        {
            get
            {
                return this.s_marshallingOverrideMessageType;
            }
            set
            {
                this.s_marshallingOverrideMessageType = value;
            }
        }
        /// <summary>
        /// (NOT IMPLEMENTED) true - current execution system uses little endian, false - current execution system uses big endian
        /// </summary>
        public bool MarshallingSystemUsesLittleEndian
        {
            get
            {
                return this.b_marshallingSystemUsesLittleEndian;
            }
            set
            {
                this.b_marshallingSystemUsesLittleEndian = value;
            }
        }
        /// <summary>
        /// list of interface delegates to post progress of sending/receiving bytes for every socket task
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished</exception>
        public List<ForestNET.Lib.Net.Sock.Task.Task.PostProgress>? PostProgresses
        {
            get
            {
                return this.del_postProgresses;
            }
            set
            {
                this.CheckClosed();
                this.del_postProgresses = value;
            }
        }
        /// <summary>
        /// list of receiving socket tasks to receive bytes as answer directly after sent a request to the other communication side
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished OR invalid communication type for receive socket tasks, must be TCP_RECEIVE_WITH_ANSWER</exception>
        public List<ForestNET.Lib.Net.Sock.Task.Task>? ReceiveSocketTasks
        {
            get
            {
                return this.a_receiveSocketTasks;
            }
            set
            {
                this.CheckClosed();

                if (this.e_communicationType != Type.TCP_RECEIVE_WITH_ANSWER)
                {
                    throw new InvalidOperationException("Can only use communication type[" + Type.TCP_RECEIVE_WITH_ANSWER + "] with receive socket tasks");
                }

                this.a_receiveSocketTasks = value;
            }
        }
        /// <summary>
        /// set shared memory object for communication network exchange. Each field if inherited class of shared memory instance will have it's own message box for sending/receiving
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished OR invalid communication type for shared memory, must not be TCP_RECEIVE_WITH_ANSWER | TCP_SEND_WITH_ANSWER OR invalid cardinality = OneMessageBoxToManySockets</exception>
        /// <exception cref="ArgumentNullException">parameter is null</exception>
        /// <exception cref="ArgumentException">invalid message box length configured</exception>
        public ISharedMemory? SharedMemory
        {
            get
            {
                return this.o_sharedMemory;
            }
            set
            {
                this.CheckClosed();

                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "Shared memory parameter is null");
                }

                /* check if message box length has been configured */
                if ((this.a_messageBoxLengths == null) || (this.a_messageBoxLengths.Count == 0))
                {
                    throw new ArgumentException("You must specify at least one message box length, but it has '0' entries or is null");
                }

                /* check for communication type */
                if ((this.e_communicationType == Type.TCP_RECEIVE_WITH_ANSWER) || (this.e_communicationType == Type.TCP_SEND_WITH_ANSWER))
                {
                    throw new InvalidOperationException("Cannot use communication type[" + Type.TCP_RECEIVE_WITH_ANSWER + "|" + Type.TCP_SEND_WITH_ANSWER + "] with shared memory");
                }

                /* check communication cardinality */
                if (this.e_cardinality == Cardinality.OneMessageBoxToManySockets)
                {
                    throw new InvalidOperationException("Cannot use cardinality[" + this.e_cardinality + "] with shared memory, we must have one message box for each field in inherited class of shared memory object");
                }

                this.o_sharedMemory = value;

                if (!this.b_useMarshallingWholeObject)
                {
                    /* each shared memory field gets own message box and own socket */
                    this.i_amountMessageBoxes = this.o_sharedMemory.AmountFields();
                    this.i_amountSockets = this.o_sharedMemory.AmountFields();
                }
                else
                {
                    /* shared memory will be transferred as whole object, so we only need one message box and one socket */
                    this.i_amountMessageBoxes = 1;
                    this.i_amountSockets = 1;

                    /* update cardinality to equal */
                    this.e_cardinality = Cardinality.Equal;
                }

                /* correct the amount of sockets value to 1 */
                if (this.e_cardinality == Cardinality.ManyMessageBoxesToOneSocket)
                {
                    this.i_amountSockets = 1;
                }

                /* we must multiply configured message box length value for each field in inherited class of shared memory object, if we do not use whole object transfer */
                if ((!this.b_useMarshallingWholeObject) && (this.a_messageBoxLengths.Count != this.o_sharedMemory.AmountFields()))
                {
                    int i_temp = this.a_messageBoxLengths[0];

                    this.a_messageBoxLengths = [];

                    for (int i = 0; i < this.o_sharedMemory.AmountFields(); i++)
                    {
                        this.a_messageBoxLengths.Add(i_temp);
                    }
                }

                /* clear any configured interface delegates */
                this.del_postProgresses = null;
            }
        }
        /// <summary>
        /// set timeout value for thread instance - how long the shared memory thread should wait after incoming/outgoing data cycle
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished</exception>
        /// <exception cref="ArgumentException">invalid parameter value</exception>
        public int SharedMemoryTimeoutMilliseconds
        {
            get
            {
                return this.i_sharedMemoryTimeoutMilliseconds;
            }
            set
            {
                this.CheckClosed();

                if (value < 1)
                {
                    throw new ArgumentException("Shared memory timeout must be at least '1' millisecond, but was set to '" + value + "' millisecond(s)");
                }

                this.i_sharedMemoryTimeoutMilliseconds = value;

                if (this.o_sharedMemoryBidirectionalConfig != null)
                {
                    this.o_sharedMemoryBidirectionalConfig.i_sharedMemoryTimeoutMilliseconds = value;
                }
            }
        }
        /// <summary>
        /// set interval object for sender cycle. At the end of the interval all field values will be send to other communication side
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished</exception>
        /// <exception cref="ArgumentNullException">parameter is null</exception>
        public ForestNET.Lib.DateInterval? SharedMemoryIntervalCompleteRefresh
        {
            get
            {
                return this.o_sharedMemoryIntervalCompleteRefresh;
            }
            set
            {
                this.CheckClosed();

                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "Shared memory interval for complete refresh parameter is null");
                }

                this.o_sharedMemoryIntervalCompleteRefresh = value;

                if (this.o_sharedMemoryBidirectionalConfig != null)
                {
                    this.o_sharedMemoryBidirectionalConfig.o_sharedMemoryIntervalCompleteRefresh = value;
                }
            }
        }
        public List<string>? BiHosts
        {
            get
            {
                return this.a_biHosts;
            }
            private set
            {
                this.a_biHosts = value;
            }
        }
        public List<int>? BiPorts
        {
            get
            {
                return this.a_biPorts;
            }
            private set
            {
                this.a_biPorts = value;
            }
        }
        public Config? SharedMemoryBidirectional
        {
            get
            {
                return this.o_sharedMemoryBidirectionalConfig;
            }
            private set
            {
                this.o_sharedMemoryBidirectionalConfig = value;
            }
        }
        public bool IsSharedMemoryBidirectionalConfigSet
        {
            get
            {
                return this.b_sharedMemoryBidirectionalConfigSet;
            }
        }

        /* Methods */

        /// <summary>
        /// Constructor of configuration class. All other settings are adjusted by properties.
        /// </summary>
        /// <param name="p_e_communicationType">communication type enumeration to determine if this communication instance will send or receive network data</param>
        /// <param name="p_e_cardinality">specifies communication cardinality of communication instance</param>
        public Config(Type p_e_communicationType, Cardinality p_e_cardinality)
        {
            this.e_communicationType = p_e_communicationType;
            this.e_socketReceiveType = null;
            this.e_cardinality = p_e_cardinality;

            this.e_security = null;
            this.s_commonSecretPassphrase = null;
            this.s_pathToCertificateFile = null;
            this.s_pathToCertificateFilePassword = null;
            this.s_certificateThumbprint = null;
            this.e_certificateStoreName = null;
            this.e_certificateStoreLocation = null;
            this.s_clientRemoteCertificateName = null;
            this.a_clientCertificateAllowList = null;

            this.i_amountMessageBoxes = 0;
            this.i_amountSockets = 0;
            this.a_messageBoxLengths = null;
            this.a_hosts = null;
            this.a_ports = null;

            this.a_socketTasks = null;

            this.i_socketServicePoolAmount = 0;
            this.i_receiverTimeoutMilliseconds = 1;
            this.i_senderTimeoutMilliseconds = 1;
            this.b_checkReachability = false;
            this.i_maxTerminations = -1;
            this.i_senderIntervalMilliseconds = 0;
            this.i_queueTimeoutMilliseconds = 1;

            this.i_udpReceiveAckTimeoutMilliseconds = 1;
            this.i_udpSendAckTimeoutMilliseconds = 1;

            this.b_udpIsMulticastSocket = false;
            this.i_udpMulticastTTL = 1;

            this.s_localAddress = null;
            this.i_localPort = 0;

            this.b_objectTransmission = false;

            this.a_receiveSocketTasks = null;

            this.o_sharedMemory = null;
            this.i_sharedMemoryTimeoutMilliseconds = 1;
            this.o_sharedMemoryIntervalCompleteRefresh = null;
            this.a_biHosts = null;
            this.a_biPorts = null;
            this.o_sharedMemoryBidirectionalConfig = null;
            this.b_sharedMemoryBidirectionalConfigSet = false;

            this.b_closed = false;

            this.b_debugNetworkTrafficOn = false;

            this.b_useMarshalling = false;
            this.b_useMarshallingWholeObject = false;
            this.i_marshallingDataLengthInBytes = 1;
            this.b_marshallingUseProperties = false;
            this.s_marshallingOverrideMessageType = null;
            this.b_marshallingSystemUsesLittleEndian = false;

            this.del_postProgresses = null;
        }

        /// <summary>
        /// Adding message box length to list
        /// </summary>
        /// <param name="p_i_value">new value for list of message box lengths [1500 or 8192]</param>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished</exception>
        /// <exception cref="ArgumentException">message box length must be '1500' or '8192'</exception>
        public void AddMessageBoxLength(int p_i_value)
        {
            this.CheckClosed();

            if ((p_i_value != 1500) && (p_i_value != 8192))
            {
                throw new ArgumentException("Invalid message box length[" + p_i_value + "] parameter. Valid values are [1500, 8192]");
            }

            /* create new list of message box lengths if it is null */
            if (this.a_messageBoxLengths == null)
            {
                this.a_messageBoxLengths = [];
            }

            this.a_messageBoxLengths.Add(p_i_value);
        }

        /// <summary>
        /// Adding host and port pair to list
        /// </summary>
        /// <param name="p_p_value">new pair value of host and port parameter for configuration list for every socket, auto check for duplicates</param>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished</exception>
        /// <exception cref="ArgumentNullException">parameter value is null or a host value is null</exception>
        /// <exception cref="ArgumentException">invalid port value, must be between 1..65535</exception>
        /// <exception cref="InvalidDataException">duplicate found in parameter list</exception>
        public void AddHostAndPort(KeyValuePair<string, int> p_p_value)
        {
            this.CheckClosed();

            /* check if host value is not empty */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_p_value.Key))
            {
                throw new ArgumentNullException(nameof(p_p_value), "Host is null");
            }

            /* check port min. value */
            if (p_p_value.Value < 1)
            {
                throw new ArgumentException("Port must be at least '1', but was set to '" + p_p_value.Value + "'");
            }

            /* check port max. value */
            if (p_p_value.Value > 65535)
            {
                throw new ArgumentException("Port must be lower equal '65535', but was set to '" + p_p_value.Value + "'");
            }

            /* create new hosts list instance if it is null */
            if (this.a_hosts == null)
            {
                this.a_hosts = [];
            }

            /* create new ports list instance if it is null */
            if (this.a_ports == null)
            {
                this.a_ports = [];
            }

            List<string> a_checkDuplicates = [];

            /* gather all existing hosts and ports in config settings */
            for (int i = 0; i < this.a_hosts.Count; i++)
            {
                string s_foo = this.a_hosts[i] + ":" + this.a_ports[i];
                a_checkDuplicates.Add(s_foo);
            }

            string s_duplicate = p_p_value.Key + ":" + p_p_value.Value;

            /* check for existing duplicate */
            if (a_checkDuplicates.Contains(s_duplicate))
            {
                throw new InvalidDataException("New entry would be a duplicate[" + s_duplicate + "] in list of target hosts and ports which is not allowed");
            }

            this.a_hosts.Add(p_p_value.Key);
            this.a_ports.Add(p_p_value.Value);
        }

        /// <summary>
        /// Adding socket task to list
        /// </summary>
        /// <param name="p_o_value">socket task for a socket instance in communication which will be added to config list of socket tasks</param>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished</exception>
        /// <exception cref="ArgumentNullException">parameter value is null</exception>
        public void AddSocketTask(ForestNET.Lib.Net.Sock.Task.Task p_o_value)
        {
            this.CheckClosed();

            /* check parameter value */
            if (p_o_value == null)
            {
                throw new ArgumentNullException(nameof(p_o_value), "Socket task parameter is null");
            }

            /* create new socket task list instance if it is null */
            if (this.a_socketTasks == null)
            {
                this.a_socketTasks = [];
            }

            this.a_socketTasks.Add(p_o_value);
        }

        /// <summary>
        /// Adding delegate of post progress to list
        /// </summary>
        /// <param name="p_del_postProgress">interface delegate to post progress of sending/receiving bytes for socket task list</param>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished</exception>
        /// <exception cref="ArgumentNullException">parameter is null</exception>
        public void AddDelegate(ForestNET.Lib.Net.Sock.Task.Task.PostProgress? p_del_postProgress)
        {
            this.CheckClosed();

            if (p_del_postProgress == null)
            {
                throw new ArgumentNullException(nameof(p_del_postProgress), "Delegate parameter is null");
            }

            /* create new interface delegate list instance if it is null */
            if (this.del_postProgresses == null)
            {
                this.del_postProgresses = [];
            }

            this.del_postProgresses.Add(p_del_postProgress);
        }

        /// <summary>
        /// Adding receive socket task to list
        /// </summary>
        /// <param name="p_o_value">receiving socket task to receive bytes as answer directly after sent a request to the other communication side for socket task list</param>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished OR invalid communication type for receive socket tasks, must be TCP_RECEIVE_WITH_ANSWER</exception>
        /// <exception cref="ArgumentNullException">parameter is null</exception>
        public void AddReceiveSocketTask(ForestNET.Lib.Net.Sock.Task.Task? p_o_value)
        {
            this.CheckClosed();

            if (p_o_value == null)
            {
                throw new ArgumentNullException(nameof(p_o_value), "Receive socket task parameter is null");
            }

            /* create new receive socket task list instance if it is null */
            if (this.a_receiveSocketTasks == null)
            {
                this.a_receiveSocketTasks = [];
            }

            this.a_receiveSocketTasks.Add(p_o_value);
        }

        /// <summary>
        /// Prepare all settings for shared memory bidirectional network communication exchange, so that all changes in fields of inherited class of shared memory object are transferred over the network to the other communication side automatically.
        /// Receive type will be SERVER.
        /// </summary>
        /// <param name="p_a_value">list of pairs, hosts and ports parameter list for every socket which will communicate with the other communication side, auto check for duplicates</param>
        /// <exception cref="InvalidOperationException">shared memory object is not configured in current communication config instance</exception>
        /// <exception cref="InvalidDataException">duplicate found in list of target hosts and ports which is not allowed</exception>
        /// <exception cref="ArgumentNullException">parameter is null or has no entries, host is null</exception>
        /// <exception cref="ArgumentException">invalid port value, must be between 1..65535</exception>
        public void SetSharedMemoryBidirectional(Dictionary<string, int> p_a_value)
        {
            this.SetSharedMemoryBidirectional(p_a_value, ForestNET.Lib.Net.Sock.Recv.ReceiveType.SERVER);
        }

        /// <summary>
        /// Prepare all settings for shared memory bidirectional network communication exchange, so that all changes in fields of inherited class of shared memory object are transferred over the network to the other communication side automatically
        /// </summary>
        /// <param name="p_a_value">dictionary of pairs, hosts and ports parameter list for every socket which will communicate with the other communication side, auto check for duplicates</param>
        /// <param name="p_e_socketReceiveType">determine receive type for all sockets receiving message from the other communication side: SOCKET or SERVER</param>
        /// <exception cref="InvalidOperationException">shared memory object is not configured in current communication config instance</exception>
        /// <exception cref="InvalidDataException">duplicate found in list of target hosts and ports which is not allowed</exception>
        /// <exception cref="ArgumentNullException">parameter is null or has no entries, host is null</exception>
        /// <exception cref="ArgumentException">invalid port value, must be between 1..65535</exception>
        public void SetSharedMemoryBidirectional(Dictionary<string, int> p_a_value, ForestNET.Lib.Net.Sock.Recv.ReceiveType p_e_socketReceiveType)
        {
            this.SetSharedMemoryBidirectional(p_a_value, p_e_socketReceiveType, null, null, null, null, null, null, null);
        }

        /// <summary>
        /// Prepare all settings for shared memory bidirectional network communication exchange, so that all changes in fields of inherited class of shared memory object are transferred over the network to the other communication side automatically
        /// </summary>
        /// <param name="p_a_value">dictionary of pairs, hosts and ports parameter list for every socket which will communicate with the other communication side, auto check for duplicates</param>
        /// <param name="p_e_socketReceiveType">determine receive type for all sockets receiving message from the other communication side: SOCKET or SERVER</param>
        /// <param name="p_s_pathToCertificateFile">path to certificate file for asymmetric security communication of client side where we will send data</param>
        /// <param name="p_s_pathToCertificateFilePassword">password to access certificate file for asymmetric security communication of client side where we will send data</param>
        /// <param name="p_s_certificateThumbprint">certificate thumbprint to find client certificate in internal store for asymmetric security communication of client side where we will send data</param>
        /// <param name="p_e_certificateStoreName">certificate store name to find client certificate in internal store for asymmetric security communication of client side where we will send data</param>
        /// <param name="p_e_certificateStoreLocation">certificate store location to find cleint certificate in internal store for asymmetric security communication of client side where we will send data</param>
        /// <param name="p_s_clientRemoteCertificateName">certificate name which our bidirectional client receive during communication for asymmetric security of client side where we will receive data from</param>
        /// <param name="p_a_clientCertificateAllowList">client certificate allow list for asymmetric security communication of client side where we will receive data from</param>
        /// <exception cref="InvalidOperationException">shared memory object is not configured in current communication config instance</exception>
        /// <exception cref="InvalidDataException">duplicate found in list of target hosts and ports which is not allowed</exception>
        /// <exception cref="ArgumentNullException">parameter is null or has no entries, host is null</exception>
        /// <exception cref="ArgumentException">invalid port value, must be between 1..65535</exception>
        public void SetSharedMemoryBidirectional(Dictionary<string, int> p_a_value, ForestNET.Lib.Net.Sock.Recv.ReceiveType p_e_socketReceiveType, string? p_s_pathToCertificateFile, string? p_s_pathToCertificateFilePassword, string? p_s_certificateThumbprint, System.Security.Cryptography.X509Certificates.StoreName? p_e_certificateStoreName, System.Security.Cryptography.X509Certificates.StoreLocation? p_e_certificateStoreLocation, string? p_s_clientRemoteCertificateName, List<System.Security.Cryptography.X509Certificates.X509Certificate>? p_a_clientCertificateAllowList)
        {
            /* check shared memory object setting */
            if (this.o_sharedMemory == null)
            {
                throw new InvalidOperationException("Shared memory instance not specified");
            }

            /* check parameter */
            if (p_a_value == null)
            {
                throw new ArgumentNullException(nameof(p_a_value), "Parameter value is 'null' for bidirectional hosts and ports");
            }

            /* check parameter entries */
            if (p_a_value.Count < 1)
            {
                throw new ArgumentNullException(nameof(p_a_value), "Please specify at least '1' entry for bidirectional hosts and ports");
            }

            this.a_biHosts = [];
            this.a_biPorts = [];

            List<string> a_checkDuplicates = [];
            int i = 0;

            /* iterate each pair of host and port in parameter list */
            foreach (KeyValuePair<string, int> o_pair in p_a_value)
            {
                i++;

                /* check if host value is not empty */
                if (ForestNET.Lib.Helper.IsStringEmpty(o_pair.Key))
                {
                    throw new ArgumentNullException("Host #" + i + " is null");
                }

                /* check port min. value */
                if (o_pair.Value < 1)
                {
                    throw new ArgumentException("Port #" + i + " must be at least '1', but was set to '" + o_pair.Value + "'");
                }

                /* check port max. value */
                if (o_pair.Value > 65535)
                {
                    throw new ArgumentException("Port #" + i + " must be lower equal '65535', but was set to '" + o_pair.Value + "'");
                }

                string s_duplicate = o_pair.Key + ":" + o_pair.Value;

                /* check for existing duplicate */
                if (a_checkDuplicates.Contains(s_duplicate))
                {
                    this.a_biHosts = [];
                    this.a_biPorts = [];
                    throw new InvalidDataException("Duplicate found[" + s_duplicate + "] in list of target hosts and ports which is not allowed");
                }

                /* add value to temp list for checking duplicates */
                a_checkDuplicates.Add(s_duplicate);

                this.a_biHosts.Add(o_pair.Key);
                this.a_biPorts.Add(o_pair.Value);
            }

            /* set same communication type as this config */
            Type e_biCommunicationType = this.e_communicationType;

            /* set opposite communication type */
            if (this.e_communicationType == Type.UDP_RECEIVE)
            {
                e_biCommunicationType = Type.UDP_SEND;
            }
            else if (this.e_communicationType == Type.UDP_SEND)
            {
                e_biCommunicationType = Type.UDP_RECEIVE;
            }
            else if (this.e_communicationType == Type.UDP_RECEIVE_WITH_ACK)
            {
                e_biCommunicationType = Type.UDP_SEND_WITH_ACK;
            }
            else if (this.e_communicationType == Type.UDP_SEND_WITH_ACK)
            {
                e_biCommunicationType = Type.UDP_RECEIVE_WITH_ACK;
            }
            else if (this.e_communicationType == Type.TCP_RECEIVE)
            {
                e_biCommunicationType = Type.TCP_SEND;
            }
            else if (this.e_communicationType == Type.TCP_SEND)
            {
                e_biCommunicationType = Type.TCP_RECEIVE;
            }

            /* create config for shared memory bidirectional network communication exchange */
            this.o_sharedMemoryBidirectionalConfig = new(e_biCommunicationType, this.e_cardinality)
            {
                /* adopt all settings except receive type and ssl context list */
                e_socketReceiveType = p_e_socketReceiveType,

                e_security = this.e_security,
                s_commonSecretPassphrase = this.s_commonSecretPassphrase,
                s_pathToCertificateFile = p_s_pathToCertificateFile ?? this.s_pathToCertificateFile,
                s_pathToCertificateFilePassword = p_s_pathToCertificateFilePassword ?? this.s_pathToCertificateFilePassword,
                s_certificateThumbprint = p_s_certificateThumbprint ?? this.s_certificateThumbprint,
                e_certificateStoreName = p_e_certificateStoreName ?? this.e_certificateStoreName,
                e_certificateStoreLocation = p_e_certificateStoreLocation ?? this.e_certificateStoreLocation,
                s_clientRemoteCertificateName = p_s_clientRemoteCertificateName ?? this.s_clientRemoteCertificateName,
                a_clientCertificateAllowList = p_a_clientCertificateAllowList ?? this.a_clientCertificateAllowList,

                i_amountMessageBoxes = this.i_amountMessageBoxes,
                i_amountSockets = this.i_amountSockets,
                a_messageBoxLengths = this.a_messageBoxLengths,
                a_hosts = this.a_biHosts,
                a_ports = this.a_biPorts,

                a_socketTasks = this.a_socketTasks,

                i_socketServicePoolAmount = this.i_socketServicePoolAmount,
                i_receiverTimeoutMilliseconds = this.i_receiverTimeoutMilliseconds,
                i_senderTimeoutMilliseconds = this.i_senderTimeoutMilliseconds,
                b_checkReachability = this.b_checkReachability,
                i_maxTerminations = this.i_maxTerminations,
                i_senderIntervalMilliseconds = this.i_senderIntervalMilliseconds,
                i_queueTimeoutMilliseconds = this.i_queueTimeoutMilliseconds,

                i_udpReceiveAckTimeoutMilliseconds = this.i_udpReceiveAckTimeoutMilliseconds,
                i_udpSendAckTimeoutMilliseconds = this.i_udpSendAckTimeoutMilliseconds,

                b_udpIsMulticastSocket = this.b_udpIsMulticastSocket,
                i_udpMulticastTTL = this.i_udpMulticastTTL,

                s_localAddress = this.s_localAddress,
                i_localPort = this.i_localPort,

                b_objectTransmission = this.b_objectTransmission,

                a_receiveSocketTasks = this.a_receiveSocketTasks,

                o_sharedMemory = this.o_sharedMemory,
                i_sharedMemoryTimeoutMilliseconds = this.i_sharedMemoryTimeoutMilliseconds,
                o_sharedMemoryIntervalCompleteRefresh = this.o_sharedMemoryIntervalCompleteRefresh,
                a_biHosts = null,
                a_biPorts = null,
                o_sharedMemoryBidirectionalConfig = null,

                /* set flag that shared memory bidirectional config has been set */
                b_sharedMemoryBidirectionalConfigSet = true,

                b_debugNetworkTrafficOn = this.b_debugNetworkTrafficOn,

                b_useMarshalling = this.b_useMarshalling,
                b_useMarshallingWholeObject = this.b_useMarshallingWholeObject,
                i_marshallingDataLengthInBytes = this.i_marshallingDataLengthInBytes,
                b_marshallingUseProperties = this.b_marshallingUseProperties,
                s_marshallingOverrideMessageType = this.s_marshallingOverrideMessageType,
                b_marshallingSystemUsesLittleEndian = this.b_marshallingSystemUsesLittleEndian
            };

            /* set flag that configuration has been closed and cannot be adjusted anymore */
            this.b_closed = true;
            this.o_sharedMemoryBidirectionalConfig.b_closed = this.b_closed;
        }

        /// <summary>
        /// internal method to check if configuration has been closed and cannot be adjusted anymore
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because bidirectional setting was finished</exception>
        private void CheckClosed()
        {
            if (this.b_closed)
            {
                throw new InvalidOperationException("Config has been closed, because bidirectional setting was finished");
            }
        }

        /// <summary>
        /// internal method to check if configuration has been closed and cannot be adjusted anymore, because shared memory object will handle these settings
        /// </summary>
        /// <exception cref="InvalidOperationException">config has been close, because shared memory object will handle these settings</exception>
        private void CheckSharedMemoryIsset(string p_s_involvedSetting)
        {
            if (this.o_sharedMemory != null)
            {
                throw new InvalidOperationException("Configuration of " + p_s_involvedSetting + " is closed. Controlled by shared memory object.");
            }
        }
    }
}