namespace ForestNET.Lib.Net.Sock.Com
{
    /// <summary>
    /// Communication class for network data exchange. All configurable settings are adjusted by communication config class which is the only parameter in the constructor.
    /// All internal functionality will be handled by an internal thread pool which will run all socket instances with it's socket tasks, or shared memory instances in combination with sockets.
    /// Special features like handling shared memory objects and bidirectional shared memory network data exchange can be configured, so that all changes in fields of inherited class of shared memory object are transferred over the network to the other communication side and vice versa automatically.
    /// </summary>
    public class Communication
    {

        /* Constants */

        private const int DEQUEUE_WAIT_MILLISECONDS = 25;

        /* Fields */

        private readonly Config o_config;
        private readonly List<ForestNET.Lib.Net.Msg.MessageBox> a_messageBoxes;
        private readonly List<ForestNET.Lib.Net.Sock.Socket> a_sockets;
        private readonly List<ForestNET.Lib.Net.Msg.MessageBox> a_answerMessageBoxes;
        private SharedMemoryTask? o_sharedMemoryTask;
        private readonly Communication? o_communicationBidirectional;

        /* Properties */

        public bool Running { get; private set; }
        private CancellationTokenSource? CancellationTokenSource { get; set; }

        /* Methods */

        /// <summary>
        /// Constructor for communication class
        /// </summary>
        /// <param name="p_o_config">all configurable settings are adjusted by communication config class which is the only parameter</param>
        /// <exception cref="InvalidOperationException">settings in config lead to an invalid state and will not be accepted</exception>
        /// <exception cref="ArgumentException">settings in config have invalid values and will not be accepted</exception>
        /// <exception cref="ArgumentNullException">settings in config have no value which are needed for initiating communication</exception>
        /// <exception cref="System.IO.IOException">issue creating TCP or TCP with TLS socket instance</exception>
		/// <exception cref="FormatException">issue local host name could not be resolved into an address</exception>
        public Communication(Config p_o_config)
        {
            /* set some default values */
            this.o_config = p_o_config;
            this.o_sharedMemoryTask = null;
            this.o_communicationBidirectional = null;
            this.Running = false;

            /* if shared memory object is set */
            if (this.o_config.SharedMemory != null)
            {
                /* check for valid cardinality */
                if ((this.o_config.Cardinality == Cardinality.OneMessageBoxToManySockets) || (this.o_config.Cardinality == Cardinality.EqualBidirectional))
                {
                    throw new InvalidOperationException("Cannot use cardinality[" + this.o_config.Cardinality + "] with shared memory");
                }

                /* check for valid communication type */
                if ((this.o_config.CommunicationType == Type.TCP_RECEIVE_WITH_ANSWER) || (this.o_config.CommunicationType == Type.TCP_SEND_WITH_ANSWER))
                {
                    throw new InvalidOperationException("Cannot use communication type[" + Type.TCP_RECEIVE_WITH_ANSWER + "|" + Type.TCP_RECEIVE_WITH_ANSWER + "] with shared memory");
                }
            }

            /* check if bidirectional cardinality is used with tcp standard send or receive */
            if (this.o_config.Cardinality == Cardinality.EqualBidirectional)
            {
                if ((this.o_config.CommunicationType != Type.TCP_RECEIVE) && (this.o_config.CommunicationType != Type.TCP_SEND))
                {
                    throw new InvalidOperationException("Cannot use cardinality[" + Cardinality.EqualBidirectional + "] only with communication type[" + Type.TCP_RECEIVE + "|" + Type.TCP_SEND + "], but not with [" + this.o_config.CommunicationType + "]");
                }
            }

            /* check for valid cardinality in combination with amount of message boxes and amount of sockets */
            if ((this.o_config.Cardinality == Cardinality.Equal) && (this.o_config.AmountMessageBoxes != this.o_config.AmountSockets))
            {
                throw new InvalidOperationException("With cardinality[Equal] amount of message boxes[" + this.o_config.AmountMessageBoxes + "] must equal amount of sockets[" + this.o_config.AmountSockets + "]");
            }
            else if ((this.o_config.Cardinality == Cardinality.EqualBidirectional) && (this.o_config.AmountMessageBoxes != (this.o_config.AmountSockets * 2)))
            {
                throw new InvalidOperationException("With cardinality[Equal-Bidirectional] amount of message boxes[" + this.o_config.AmountMessageBoxes + "] must be twice as much as amount of sockets[" + this.o_config.AmountSockets + "]");
            }
            else if ((this.o_config.Cardinality == Cardinality.OneMessageBoxToManySockets) && ((this.o_config.AmountMessageBoxes != 1) || (this.o_config.AmountMessageBoxes >= this.o_config.AmountSockets)))
            {
                throw new InvalidOperationException("With cardinality[One-MessageBox-To-Many-Sockets] amount of message boxes[" + this.o_config.AmountMessageBoxes + "] must be [1] and lower than amount of sockets[" + this.o_config.AmountSockets + "]");
            }
            else if ((this.o_config.Cardinality == Cardinality.ManyMessageBoxesToOneSocket) && ((this.o_config.AmountSockets != 1) || (this.o_config.AmountMessageBoxes <= this.o_config.AmountSockets)))
            {
                throw new InvalidOperationException("With cardinality[Many-MessageBoxes-To-One-Socket] amount of message boxes[" + this.o_config.AmountMessageBoxes + "] must greater than amount of sockets[" + this.o_config.AmountSockets + "] and amount of sockets must be [1]");
            }

            /* check amount message boxes value */
            if (this.o_config.AmountMessageBoxes < 1)
            {
                throw new ArgumentException("Amount of message boxes must be at least '1', but was set to '" + this.o_config.AmountMessageBoxes + "'");
            }

            /* check amount sockets value */
            if (this.o_config.AmountSockets < 1)
            {
                throw new ArgumentException("Amount of sockets must be at least '1', but was set to '" + this.o_config.AmountSockets + "'");
            }

            /* check amount of message box length values and amount of message boxes, must match */
            if (this.o_config.MessageBoxLengths?.Count != this.o_config.AmountMessageBoxes)
            {
                throw new ArgumentException("Amount of message box lengths[" + this.o_config.MessageBoxLengths?.Count + "] is not equal amount of message boxes[" + this.o_config.AmountMessageBoxes + "]");
            }

            /* check amount of host addresses and amount of sockets, must match */
            if (this.o_config.Hosts?.Count != this.o_config.AmountSockets)
            {
                throw new ArgumentException("Amount of hosts and ports[" + this.o_config.Hosts?.Count + "] is not equal amount of sockets[" + this.o_config.AmountSockets + "]");
            }

            /* check amount of ports and amount of sockets, must match */
            if (this.o_config.Ports?.Count != this.o_config.AmountSockets)
            {
                throw new ArgumentException("Amount of hosts and ports[" + this.o_config.Ports?.Count + "] is not equal amount of sockets[" + this.o_config.AmountSockets + "]");
            }

            /* if socket tasks have been determined */
            if (this.o_config.SocketTasks != null)
            {
                /* check amount of sockets tasks and amount of sockets, must match */
                if (this.o_config.SocketTasks.Count != this.o_config.AmountSockets)
                {
                    throw new ArgumentException("Amount of socket tasks[" + this.o_config.SocketTasks?.Count + "] is not equal amount of sockets[" + this.o_config.AmountSockets + "]");
                }

                /* if interface delegates have been determined */
                if (this.o_config.PostProgresses != null)
                {
                    /* check amount of sockets tasks and amount of interface delegates, must match */
                    if (this.o_config.SocketTasks.Count != this.o_config.PostProgresses.Count)
                    {
                        throw new ArgumentException("Amount of socket tasks[" + this.o_config.SocketTasks.Count + "] is not equal amount of interface delegates[" + this.o_config.PostProgresses.Count + "]");
                    }
                }
            }

            /* create instances of message boxes list and sockets list */
            this.a_messageBoxes = [];
            this.a_sockets = [];
            this.a_answerMessageBoxes = [];

            if ( /* receiving side */
                this.o_config.CommunicationType == Type.UDP_RECEIVE ||
                this.o_config.CommunicationType == Type.UDP_RECEIVE_WITH_ACK ||
                this.o_config.CommunicationType == Type.TCP_RECEIVE ||
                this.o_config.CommunicationType == Type.TCP_RECEIVE_WITH_ANSWER
            )
            {
                /* check if socket receive type is not null */
                if (this.o_config.SocketReceiveType == null)
                {
                    throw new ArgumentNullException("Socket receive type[null], must be set for communication type[" + this.o_config.CommunicationType + "]");
                }

                /* log internal warning if receiver timeout is lower than 1 second */
                if ((this.o_config.ReceiverTimeoutMilliseconds < 1000) && (this.o_config.Cardinality != Cardinality.EqualBidirectional))
                {
                    ForestNET.Lib.Global.ILogWarning("Receiver timeout milliseconds[" + this.o_config.ReceiverTimeoutMilliseconds + "] for receiving socket is lower equal '1000 milliseconds'");
                }

                /* log internal warning if queue timeout is greater than 100 millisecond */
                if (this.o_config.QueueTimeoutMilliseconds > 100)
                {
                    ForestNET.Lib.Global.ILogWarning("Queue timeout milliseconds[" + this.o_config.QueueTimeoutMilliseconds + "] for receiving socket is greater equal '100 milliseconds'");
                }

                if (this.o_config.CommunicationType == Type.UDP_RECEIVE_WITH_ACK)
                {
                    /* if socket is receiving UDP data with acknowledge message, log internal warning if UDP send acknowledge timeout is greater than 1 second */
                    if (this.o_config.UDPSendAckTimeoutMilliseconds > 1000)
                    {
                        ForestNET.Lib.Global.ILogWarning("UDP send ACK timeout milliseconds[" + this.o_config.UDPSendAckTimeoutMilliseconds + "] for receiving is greater equal '1000 milliseconds'");
                    }
                }
            }
            else if ( /* sending side */
                this.o_config.CommunicationType == Type.UDP_SEND ||
                this.o_config.CommunicationType == Type.UDP_SEND_WITH_ACK ||
                this.o_config.CommunicationType == Type.TCP_SEND ||
                this.o_config.CommunicationType == Type.TCP_SEND_WITH_ANSWER
            )
            {
                if (
                    this.o_config.CommunicationType == Type.TCP_SEND ||
                    this.o_config.CommunicationType == Type.TCP_SEND_WITH_ANSWER
                )
                {
                    /* if socket is sending TCP (with direct answer), log internal warning if sender timeout is lower than 1 second */
                    if ((this.o_config.SenderTimeoutMilliseconds < 1000) && (this.o_config.Cardinality != Cardinality.EqualBidirectional))
                    {
                        ForestNET.Lib.Global.ILogWarning("Sender timeout milliseconds[" + this.o_config.SenderTimeoutMilliseconds + "] for sending socket is lower equal '1000 milliseconds'");
                    }
                }

                /* log internal warning if queue timeout is greater than 100 millisecond */
                if (this.o_config.QueueTimeoutMilliseconds > 100)
                {
                    ForestNET.Lib.Global.ILogWarning("Queue timeout milliseconds[" + this.o_config.QueueTimeoutMilliseconds + "] for sending socket is greater equal '100 milliseconds'");
                }

                if (this.o_config.CommunicationType == Type.UDP_SEND_WITH_ACK)
                {
                    /* if socket is sending UDP data with acknowledge message, log internal warning if UDP receive acknowledge timeout is greater than 1 second */
                    if (this.o_config.UDPReceiveAckTimeoutMilliseconds > 1000)
                    {
                        ForestNET.Lib.Global.ILogWarning("UDP receive ACK timeout milliseconds[" + this.o_config.UDPReceiveAckTimeoutMilliseconds + "] for sending is greater equal '1000 milliseconds'");
                    }
                }
            }

            int i_decreaseForEncryption = 0;

            /* check for length correction if manual encryption is active */
            if ((this.o_config.CommunicationSecurity == Security.SYMMETRIC_128_BIT_HIGH) || (this.o_config.CommunicationSecurity == Security.SYMMETRIC_256_BIT_HIGH))
            {
                i_decreaseForEncryption = 28;
            }
            else if ((this.o_config.CommunicationSecurity == Security.SYMMETRIC_128_BIT_LOW) || (this.o_config.CommunicationSecurity == Security.SYMMETRIC_256_BIT_LOW))
            {
                /* log internal warning if using low communication security with WAN or DMZ or network segments not fully under control */
                ForestNET.Lib.Global.ILogWarning("Vulnerable communication warning! Do not use [" + Security.SYMMETRIC_128_BIT_LOW + ", " + Security.SYMMETRIC_256_BIT_LOW + "] communication security with WAN or DMZ applications or any other network segments where you do not have complete control!");
                i_decreaseForEncryption = 16;
            }

            /* create message box instances */
            for (int i = 0; i < this.o_config.AmountMessageBoxes; i++)
            {
                this.a_messageBoxes.Add(new(i + 1, (this.o_config.MessageBoxLengths[i] - i_decreaseForEncryption)));
                this.a_answerMessageBoxes.Add(new(i + 1, (this.o_config.MessageBoxLengths[i] - i_decreaseForEncryption)));
            }

            /* iterate for each planned socket instance */
            for (int i = 0; i < this.o_config.AmountSockets; i++)
            {
                /* create variable for socket task */
                ForestNET.Lib.Net.Sock.Task.Task? o_socketTask = null;

                if (this.o_config.SocketTasks != null)
                { /* socket tasks are determined in the config */
                    if ( /* UDP communication type */
                        this.o_config.CommunicationType == Type.UDP_SEND ||
                        this.o_config.CommunicationType == Type.UDP_RECEIVE ||
                        this.o_config.CommunicationType == Type.UDP_SEND_WITH_ACK ||
                        this.o_config.CommunicationType == Type.UDP_RECEIVE_WITH_ACK
                    )
                    {
                        /* check that all socket tasks are of type UDP */
                        if ((this.o_config.SocketTasks[i].Type != ForestNET.Lib.Net.Sock.Type.UDP_CLIENT) && (this.o_config.SocketTasks[i].Type != ForestNET.Lib.Net.Sock.Type.UDP_SERVER))
                        {
                            throw new InvalidOperationException("Socket type[" + this.o_config.SocketTasks[i].Type + "] of socket task #" + (i + 1) + " must be [UDP] for communication type[" + this.o_config.CommunicationType + "]");
                        }
                    }
                    else if ( /* TCP communication type */
                        this.o_config.CommunicationType == Type.TCP_SEND ||
                        this.o_config.CommunicationType == Type.TCP_RECEIVE ||
                        this.o_config.CommunicationType == Type.TCP_SEND_WITH_ANSWER ||
                        this.o_config.CommunicationType == Type.TCP_RECEIVE_WITH_ANSWER
                    )
                    {
                        /* check that all socket tasks are of type TCP */
                        if ((this.o_config.SocketTasks[i].Type != ForestNET.Lib.Net.Sock.Type.TCP_CLIENT) && (this.o_config.SocketTasks[i].Type != ForestNET.Lib.Net.Sock.Type.TCP_SERVER) && (this.o_config.SocketTasks[i].Type != ForestNET.Lib.Net.Sock.Type.TCP_TLS_CLIENT) && (this.o_config.SocketTasks[i].Type != ForestNET.Lib.Net.Sock.Type.TCP_TLS_SERVER))
                        {
                            throw new InvalidOperationException("Socket type[" + this.o_config.SocketTasks[i].Type + "] of socket task #" + (i + 1) + " must be [TCP] for communication type[" + this.o_config.CommunicationType + "]");
                        }
                    }

                    /* set socket task variable */
                    o_socketTask = this.o_config.SocketTasks[i];

                    /* if interface delegates have been determined and communication type is TCP only */
                    if (
                        (this.o_config.PostProgresses != null)
                        &&
                        (
                            this.o_config.CommunicationType == Type.TCP_SEND ||
                            this.o_config.CommunicationType == Type.TCP_RECEIVE ||
                            this.o_config.CommunicationType == Type.TCP_SEND_WITH_ANSWER ||
                            this.o_config.CommunicationType == Type.TCP_RECEIVE_WITH_ANSWER
                        )
                    )
                    {
                        /* set interface delegate for socket task from determined list */
                        o_socketTask.PostProgressDelegate = this.o_config.PostProgresses[i];
                    }
                }
                else
                {
                    if ( /* receiving side */
                        this.o_config.CommunicationType == Type.UDP_RECEIVE ||
                        this.o_config.CommunicationType == Type.UDP_RECEIVE_WITH_ACK ||
                        this.o_config.CommunicationType == Type.TCP_RECEIVE ||
                        this.o_config.CommunicationType == Type.TCP_RECEIVE_WITH_ANSWER
                    )
                    {
                        if (this.o_config.CommunicationSecurity == Security.ASYMMETRIC)
                        {
                            ForestNET.Lib.Global.ILogConfig("create receiving socket task instance with ssl/tls server socket and asymmetric security");

                            /* create receiving socket task instance with ssl/tls server socket and asymmetric security */
                            o_socketTask = new ForestNET.Lib.Net.Sock.Task.Recv.Receive(this.o_config.CommunicationType, this.o_config.Cardinality, this.o_config.QueueTimeoutMilliseconds, this.o_config.CommunicationSecurity);
                        }
                        else
                        {
                            ForestNET.Lib.Global.ILogConfig("create receiving socket task instance with server socket");

                            /* create receiving socket task instance with server socket */
                            o_socketTask = new ForestNET.Lib.Net.Sock.Task.Recv.Receive(this.o_config.CommunicationType, this.o_config.Cardinality, this.o_config.QueueTimeoutMilliseconds, this.o_config.CommunicationSecurity);
                        }

                        ForestNET.Lib.Global.ILogConfig("\t" + "set UDP send ACK timeout value:" + "\t\t\t" + this.o_config.UDPSendAckTimeoutMilliseconds);

                        /* set UDP send ACK timeout value */
                        o_socketTask.UDPSendACKTimeoutMilliseconds = this.o_config.UDPSendAckTimeoutMilliseconds;

                        /* check if we receive TCP requests and answer directly */
                        if (this.o_config.CommunicationType == Type.TCP_RECEIVE_WITH_ANSWER)
                        {
                            /* check if receive socket tasks are available */
                            if ((this.o_config.ReceiveSocketTasks == null) || (this.o_config.ReceiveSocketTasks.Count == 0))
                            {
                                throw new ArgumentNullException("No receive socket task(s) specified for communication type[" + this.o_config.CommunicationType + "]");
                            }
                            else
                            {
                                /* check if enough receive socket tasks are available */
                                if (i > (this.o_config.ReceiveSocketTasks.Count - 1))
                                {
                                    throw new ArgumentNullException("Could not load receive socket task for socket #" + i);
                                }
                                else
                                {
                                    /* set receive socket task */
                                    o_socketTask.ReceiveSocketTask = this.o_config.ReceiveSocketTasks[i];
                                }
                            }
                        }
                    }
                    else if ( /* sending side */
                        this.o_config.CommunicationType == Type.UDP_SEND ||
                        this.o_config.CommunicationType == Type.UDP_SEND_WITH_ACK ||
                        this.o_config.CommunicationType == Type.TCP_SEND ||
                        this.o_config.CommunicationType == Type.TCP_SEND_WITH_ANSWER
                    )
                    {
                        if (this.o_config.CommunicationSecurity == Security.ASYMMETRIC)
                        {
                            ForestNET.Lib.Global.ILogConfig("create sending socket task instance with ssl/tls socket and asymmetric security");

                            /* create sending socket task instance with ssl/tls socket and asymmetric security */
                            o_socketTask = new ForestNET.Lib.Net.Sock.Task.Send.Send(this.o_config.CommunicationType, this.o_config.Cardinality, this.o_config.QueueTimeoutMilliseconds, this.o_config.CommunicationSecurity);
                        }
                        else
                        {
                            ForestNET.Lib.Global.ILogConfig("create sending socket task instance with socket");

                            /* create sending socket task instance with socket */
                            o_socketTask = new ForestNET.Lib.Net.Sock.Task.Send.Send(this.o_config.CommunicationType, this.o_config.Cardinality, this.o_config.QueueTimeoutMilliseconds, this.o_config.CommunicationSecurity);
                        }

                        ForestNET.Lib.Global.ILogConfig("\t" + "set UDP receive ACK timeout value:" + "\t\t\t" + this.o_config.UDPReceiveAckTimeoutMilliseconds);

                        /* set UDP receive ACK timeout value */
                        o_socketTask.UDPReceiveACKTimeoutMilliseconds = this.o_config.UDPReceiveAckTimeoutMilliseconds;
                    }

                    /* if interface delegates have been determined and communication type is TCP only */
                    if (
                        (o_socketTask != null)
                        &&
                        (this.o_config.PostProgresses != null)
                        &&
                        (this.o_config.PostProgresses.Count == 1)
                        &&
                        (
                            this.o_config.CommunicationType == Type.TCP_SEND ||
                            this.o_config.CommunicationType == Type.TCP_RECEIVE ||
                            this.o_config.CommunicationType == Type.TCP_SEND_WITH_ANSWER ||
                            this.o_config.CommunicationType == Type.TCP_RECEIVE_WITH_ANSWER
                        )
                    )
                    {
                        /* set interface delegate for sending socket task from determined list - first element */
                        o_socketTask.PostProgressDelegate = this.o_config.PostProgresses[0];
                    }
                }

                /* check if socket task has been initiated */
                if (o_socketTask == null)
                {
                    throw new NullReferenceException("socket task instance is null");
                }

                ForestNET.Lib.Global.ILogConfig("\t" + "set object transmission flag:" + "\t\t\t" + this.o_config.ObjectTransmission);

                /* set object transmission flag */
                o_socketTask.ObjectTransmission = this.o_config.ObjectTransmission;

                ForestNET.Lib.Global.ILogConfig("\t" + "set flag for debug network traffic:" + "\t\t\t" + this.o_config.DebugNetworkTrafficOn);

                /* set flag for debug network traffic */
                o_socketTask.DebugNetworkTrafficOn = this.o_config.DebugNetworkTrafficOn;

                ForestNET.Lib.Global.ILogConfig("\t" + "set flag for using marshalling:" + "\t\t\t" + this.o_config.UseMarshalling);

                /* set flag for using marshalling */
                o_socketTask.UseMarshalling = this.o_config.UseMarshalling;

                ForestNET.Lib.Global.ILogConfig("\t" + "set marshalling data length in bytes [1..4]:" + "\t\t\t" + this.o_config.MarshallingDataLengthInBytes);

                /* set marshalling data length in bytes [1..4] */
                o_socketTask.MarshallingDataLengthInBytes = this.o_config.MarshallingDataLengthInBytes;

                ForestNET.Lib.Global.ILogConfig("\t" + "set marshalling use property methods flag:" + "\t\t\t" + this.o_config.MarshallingUseProperties);

                /* set marshalling use property methods flag */
                o_socketTask.MarshallingUseProperties = this.o_config.MarshallingUseProperties;

                ForestNET.Lib.Global.ILogConfig("\t" + "set marshalling override messag etype:" + "\t\t\t" + this.o_config.MarshallingOverrideMessageType);

                /* set marshalling override message type */
                o_socketTask.MarshallingOverrideMessageType = this.o_config.MarshallingOverrideMessageType;

                ForestNET.Lib.Global.ILogConfig("\t" + "set marshalling little endian flag:" + "\t\t\t" + this.o_config.MarshallingSystemUsesLittleEndian);

                /* set marshalling little endian flag */
                o_socketTask.MarshallingSystemUsesLittleEndian = this.o_config.MarshallingSystemUsesLittleEndian;

                /* if we use manual security */
                if ((this.o_config.CommunicationSecurity == Security.SYMMETRIC_128_BIT_HIGH) || (this.o_config.CommunicationSecurity == Security.SYMMETRIC_256_BIT_HIGH) || (this.o_config.CommunicationSecurity == Security.SYMMETRIC_128_BIT_LOW) || (this.o_config.CommunicationSecurity == Security.SYMMETRIC_256_BIT_LOW))
                {
                    /* check if common secret passphrase is set */
                    if ((this.o_config.CommonSecretPassphrase == null) || (ForestNET.Lib.Helper.IsStringEmpty(this.o_config.CommonSecretPassphrase)))
                    {
                        throw new ArgumentNullException("Common secret passphrase is not specified for communication security[" + this.o_config.CommunicationSecurity + "]");
                    }

                    /* check common secret passphrase min. length */
                    if (this.o_config.CommonSecretPassphrase.Length < 36)
                    {
                        throw new ArgumentException("Common secret passphrase must have at least '36' characters, but has '" + this.o_config.CommonSecretPassphrase.Length + "' characters");
                    }

                    ForestNET.Lib.Global.ILogConfig("\t" + "set common secret passphrase for manual manual security: " + this.o_config.CommunicationSecurity);

                    /* set common secret passphrase */
                    o_socketTask.CommonSecretPassphrase = this.o_config.CommonSecretPassphrase;
                }

                /* temp lists to pass instantiated message boxes to receiving/sending socket task */
                List<ForestNET.Lib.Net.Msg.MessageBox> a_tempMessageBoxes = [];
                List<ForestNET.Lib.Net.Msg.MessageBox> a_tempAnswerMessageBoxes = [];
                int i_bufferSize = 1;

                /* add message box and answer message box instance to socket task based on communication cardinality + get buffer size from message box instance */
                if (this.o_config.Cardinality == Cardinality.Equal)
                { /* i -> i */
                    i_bufferSize = this.a_messageBoxes[i].MessageLength;

                    a_tempMessageBoxes.Add(this.a_messageBoxes[i]);
                    o_socketTask.MessageBoxes = a_tempMessageBoxes;

                    a_tempAnswerMessageBoxes.Add(this.a_answerMessageBoxes[i]);
                    o_socketTask.AnswerMessageBoxes = a_tempAnswerMessageBoxes;
                }
                else if (this.o_config.Cardinality == Cardinality.EqualBidirectional)
                { /* each socket and each socket task should have two message boxes, one for sending and one for receiving */
                    i_bufferSize = this.a_messageBoxes[0].MessageLength;

                    /* check that each message box has the same buffer size for receiving/sending socket task */
                    foreach (ForestNET.Lib.Net.Msg.MessageBox o_messageBox in this.a_messageBoxes)
                    {
                        if (o_messageBox.MessageLength != i_bufferSize)
                        {
                            throw new ArgumentException("With cardinality[" + this.o_config.Cardinality + "] all message boxes must have the same message length[" + i_bufferSize + "], but found message length[" + o_messageBox.MessageLength + "]");
                        }
                    }

                    /* add one message box for sending and one for receiving from the overall message box list from config */
                    a_tempMessageBoxes.Add(this.a_messageBoxes[(i * 2)]);
                    a_tempMessageBoxes.Add(this.a_messageBoxes[(i * 2) + 1]);
                    o_socketTask.MessageBoxes = a_tempMessageBoxes;

                    o_socketTask.AnswerMessageBoxes = this.a_answerMessageBoxes;
                }
                else if (this.o_config.Cardinality == Cardinality.OneMessageBoxToManySockets)
                { /* first instantiated message box in list */
                    i_bufferSize = this.a_messageBoxes[0].MessageLength;

                    a_tempMessageBoxes.Add(this.a_messageBoxes[0]);
                    o_socketTask.MessageBoxes = a_tempMessageBoxes;

                    a_tempAnswerMessageBoxes.Add(this.a_answerMessageBoxes[0]);
                    o_socketTask.AnswerMessageBoxes = a_tempAnswerMessageBoxes;
                }
                else if (this.o_config.Cardinality == Cardinality.ManyMessageBoxesToOneSocket)
                { /* complete message box list */
                    i_bufferSize = this.a_messageBoxes[0].MessageLength;

                    /* check that each message box has the same buffer size for receiving/sending socket task */
                    foreach (ForestNET.Lib.Net.Msg.MessageBox o_messageBox in this.a_messageBoxes)
                    {
                        if (o_messageBox.MessageLength != i_bufferSize)
                        {
                            throw new ArgumentException("With cardinality[" + this.o_config.Cardinality + "] all message boxes must have the same message length[" + i_bufferSize + "], but found message length[" + o_messageBox.MessageLength + "]");
                        }
                    }

                    o_socketTask.MessageBoxes = this.a_messageBoxes;
                    o_socketTask.AnswerMessageBoxes = this.a_answerMessageBoxes;
                }
                else
                {
                    throw new InvalidOperationException("Unknown communication cardinality within communication constructor: " + this.o_config.Cardinality);
                }

                ForestNET.Lib.Global.ILogConfig("read buffer size '" + i_bufferSize + "' from message box instance for socket creation");

                /* remember length correction if manual encryption is active */
                if ((this.o_config.CommunicationSecurity == Security.SYMMETRIC_128_BIT_HIGH) || (this.o_config.CommunicationSecurity == Security.SYMMETRIC_256_BIT_HIGH) || (this.o_config.CommunicationSecurity == Security.SYMMETRIC_128_BIT_LOW) || (this.o_config.CommunicationSecurity == Security.SYMMETRIC_256_BIT_LOW))
                {
                    i_bufferSize += i_decreaseForEncryption;
                }

                if ((this.o_config.CommunicationType == Type.UDP_RECEIVE) || (this.o_config.CommunicationType == Type.UDP_RECEIVE_WITH_ACK))
                {
                    ForestNET.Lib.Global.ILogConfig("create UDP receive socket with all adjusted settings incl. socket task");

                    /* create UDP receive socket with all adjusted settings incl. socket task */
                    ForestNET.Lib.Net.Sock.Recv.ReceiveUDP o_socket = new(
                        this.o_config.SocketReceiveType ?? ForestNET.Lib.Net.Sock.Recv.ReceiveType.SOCKET,
                        this.o_config.Hosts[i],
                        this.o_config.Ports[i],
                        o_socketTask,
                        this.o_config.ReceiverTimeoutMilliseconds,
                        this.o_config.MaxTerminations,
                        i_bufferSize
                    );

                    ForestNET.Lib.Global.ILogConfig("\t" + "set fixed amount of threads for thread pool instance in socket object: '" + this.o_config.SocketServicePoolAmount + "'");

                    /* set fixed amount of threads for thread pool instance in socket object */
                    o_socket.ServicePoolAmount = this.o_config.SocketServicePoolAmount;

                    /* set multicast flag from config, only for upd receive NOT with ack */
                    if ((this.o_config.UDPIsMulticastSocket) && (this.o_config.CommunicationType == Type.UDP_RECEIVE))
                    {
                        o_socket.IsMulticastSocket = this.o_config.UDPIsMulticastSocket;

                        ForestNET.Lib.Global.ILogConfig("\t" + "set multicast flag in socket object: '" + this.o_config.UDPIsMulticastSocket + "'");
                    }

                    ForestNET.Lib.Global.ILogConfig("add socket to list");

                    /* add socket to list */
                    this.a_sockets.Add(o_socket);
                }
                else if ((this.o_config.CommunicationType == Type.UDP_SEND) || (this.o_config.CommunicationType == Type.UDP_SEND_WITH_ACK))
                {
                    ForestNET.Lib.Global.ILogConfig("create UDP send socket with all adjusted settings incl. socket task");

                    /* create UDP send socket with all adjusted settings incl. socket task */
                    ForestNET.Lib.Net.Sock.Send.SendUDP o_socket = new(
                        this.o_config.Hosts[i],
                        this.o_config.Ports[i],
                        o_socketTask,
                        this.o_config.SenderIntervalMilliseconds,
                        this.o_config.MaxTerminations,
                        i_bufferSize,
                        this.o_config.LocalAddress ?? System.Net.Dns.GetHostName(),
                        this.o_config.LocalPort
                    );

                    /* check if we have a sender timeout */
                    if (this.o_config.SenderTimeoutMilliseconds > 0)
                    {
                        /* need to set timeout for connect async to udp target */
                        o_socket.Timeout = this.o_config.SenderTimeoutMilliseconds;
                    }

                    /* set multicast flag and TTL from config, only for upd receive NOT with ack */
                    if ((this.o_config.UDPIsMulticastSocket) && (this.o_config.CommunicationType == Type.UDP_SEND))
                    {
                        o_socket.IsMulticastSocket = this.o_config.UDPIsMulticastSocket;

                        ForestNET.Lib.Global.ILogConfig("\t" + "set multicast flag in socket object: '" + this.o_config.UDPIsMulticastSocket + "'");

                        o_socket.MulticastTTL = this.o_config.UDPMulticastTTL;

                        ForestNET.Lib.Global.ILogConfig("\t" + "set multicast TTL in socket object: '" + this.o_config.UDPMulticastTTL + "'");
                    }

                    ForestNET.Lib.Global.ILogConfig("add socket to list");

                    /* add socket to list */
                    this.a_sockets.Add(o_socket);
                }
                else if ((this.o_config.CommunicationType == Type.TCP_RECEIVE) || (this.o_config.CommunicationType == Type.TCP_RECEIVE_WITH_ANSWER))
                {
                    ForestNET.Lib.Net.Sock.Recv.ReceiveTCP? o_socket = null;

                    if (this.o_config.CommunicationSecurity == Security.ASYMMETRIC)
                    {
                        ForestNET.Lib.Global.ILogConfig("create TCP receive socket with all adjusted settings incl. socket task and asymmetric security");

                        /* create TCP receive socket with all adjusted settings incl. socket task and asymmetric security */
                        o_socket = new(
                            this.o_config.SocketReceiveType ?? ForestNET.Lib.Net.Sock.Recv.ReceiveType.SOCKET,
                            this.o_config.Hosts[i],
                            this.o_config.Ports[i],
                            o_socketTask,
                            this.o_config.ReceiverTimeoutMilliseconds,
                            this.o_config.MaxTerminations,
                            i_bufferSize,
                            this.o_config.PathToCertificateFile,
                            this.o_config.PathToCertificateFilePassword
                        );

                        /* check if we shall search certificate in x509 store on this machine, but only if path to certificate file is not used */
                        if ((this.o_config.PathToCertificateFile == null) && (this.o_config.CertificateThumbprint != null) && (this.o_config.CertificateStoreName != null) && (this.o_config.CertificateStoreLocation != null))
                        {
                            ForestNET.Lib.Global.ILogConfig("find and open x509 store [" + this.o_config.CertificateStoreName.Value + ", " + this.o_config.CertificateStoreLocation.Value + "] with thumbprint[" + this.o_config.CertificateThumbprint + "]");

                            /* find and open x509 store */
                            System.Security.Cryptography.X509Certificates.X509Store o_x509Store = new(this.o_config.CertificateStoreName.Value, this.o_config.CertificateStoreLocation.Value);
                            o_x509Store.Open(System.Security.Cryptography.X509Certificates.OpenFlags.ReadOnly);
                            /* get certificate by thumbprint */
                            o_socket.ServerCertificate = o_x509Store.Certificates.Single(o_foo => o_foo.Thumbprint.Equals(this.o_config.CertificateThumbprint, StringComparison.CurrentCultureIgnoreCase));
                        }
                    }
                    else
                    {
                        ForestNET.Lib.Global.ILogConfig("create TCP receive socket with all adjusted settings incl. socket task");

                        /* create TCP receive socket with all adjusted settings incl. socket task */
                        o_socket = new(
                            this.o_config.SocketReceiveType ?? ForestNET.Lib.Net.Sock.Recv.ReceiveType.SOCKET,
                            this.o_config.Hosts[i],
                            this.o_config.Ports[i],
                            o_socketTask,
                            this.o_config.ReceiverTimeoutMilliseconds,
                            this.o_config.MaxTerminations,
                            i_bufferSize,
                            null
                        );
                    }

                    ForestNET.Lib.Global.ILogConfig("\t" + "set fixed amount of threads for thread pool instance in socket object: '" + this.o_config.SocketServicePoolAmount + "'");

                    /* set fixed amount of threads for thread pool instance in socket object */
                    o_socket.ServicePoolAmount = this.o_config.SocketServicePoolAmount;

                    ForestNET.Lib.Global.ILogConfig("add socket to list");

                    /* add socket to list */
                    this.a_sockets.Add(o_socket);
                }
                else if ((this.o_config.CommunicationType == Type.TCP_SEND) || (this.o_config.CommunicationType == Type.TCP_SEND_WITH_ANSWER))
                {
                    ForestNET.Lib.Net.Sock.Send.SendTCP? o_socket = null;

                    if (this.o_config.CommunicationSecurity == Security.ASYMMETRIC)
                    {

                        ForestNET.Lib.Global.ILogConfig("create TCP send socket with all adjusted settings incl. socket task and asymmetric security");

                        /* create TCP send socket with all adjusted settings incl. socket task and asymmetric security */
                        o_socket = new(
                            this.o_config.Hosts[i],
                            this.o_config.Ports[i],
                            o_socketTask,
                            this.o_config.SenderTimeoutMilliseconds,
                            this.o_config.CheckReachability,
                            this.o_config.MaxTerminations,
                            this.o_config.SenderIntervalMilliseconds,
                            i_bufferSize,
                            this.o_config.LocalAddress ?? System.Net.Dns.GetHostName(),
                            this.o_config.LocalPort,
                            this.o_config.ClientCertificateAllowList
                        );

                        /* check if we shall open single certificate file from disk */
                        if ((this.o_config.ClientCertificateAllowList == null) && (this.o_config.PathToCertificateFile != null))
                        {
                            ForestNET.Lib.Global.ILogConfig("open certificate from file disk[" + this.o_config.PathToCertificateFile + "]");

                            /* open certificate file with password */
                            if (this.o_config.PathToCertificateFilePassword != null)
                            {
                                o_socket.AddClientCertificateAllowList(new System.Security.Cryptography.X509Certificates.X509Certificate2(this.o_config.PathToCertificateFile, this.o_config.PathToCertificateFilePassword));
                            }
                            else
                            {
                                /* open certificate file */
                                o_socket.AddClientCertificateAllowList(System.Security.Cryptography.X509Certificates.X509Certificate.CreateFromCertFile(this.o_config.PathToCertificateFile));
                            }
                        }
                        else if ((this.o_config.ClientCertificateAllowList == null) && (this.o_config.CertificateThumbprint != null) && (this.o_config.CertificateStoreName != null) && (this.o_config.CertificateStoreLocation != null))
                        { /* check if we shall search certificate in x509 store on this machine, but only if path to certificate file is not used */
                            ForestNET.Lib.Global.ILogConfig("find and open x509 store [" + this.o_config.CertificateStoreName.Value + ", " + this.o_config.CertificateStoreLocation.Value + "] with thumbprint[" + this.o_config.CertificateThumbprint + "]");

                            /* find and open x509 store */
                            System.Security.Cryptography.X509Certificates.X509Store o_x509Store = new(this.o_config.CertificateStoreName.Value, this.o_config.CertificateStoreLocation.Value);
                            o_x509Store.Open(System.Security.Cryptography.X509Certificates.OpenFlags.ReadOnly);
                            /* get certificate by thumbprint */
                            o_socket.AddClientCertificateAllowList(o_x509Store.Certificates.Single(o_foo => o_foo.Thumbprint.Equals(this.o_config.CertificateThumbprint, StringComparison.CurrentCultureIgnoreCase)));
                        }
                        else if ((this.o_config.ClientCertificateAllowList == null) && (this.o_config.ClientRemoteCertificateName != null))
                        { /* set remote certificate name for client */
                            ForestNET.Lib.Global.ILogConfig("set remote certificate name for client - [" + this.o_config.ClientRemoteCertificateName + "]");

                            o_socket.RemoteCertificateName = this.o_config.ClientRemoteCertificateName;
                        }
                    }
                    else
                    {
                        ForestNET.Lib.Global.ILogConfig("create TCP send socket with all adjusted settings incl. socket task");

                        /* create TCP send socket with all adjusted settings incl. socket task */
                        o_socket = new(
                            this.o_config.Hosts[i],
                            this.o_config.Ports[i],
                            o_socketTask,
                            this.o_config.SenderTimeoutMilliseconds,
                            this.o_config.CheckReachability,
                            this.o_config.MaxTerminations,
                            this.o_config.SenderIntervalMilliseconds,
                            i_bufferSize,
                            this.o_config.LocalAddress ?? System.Net.Dns.GetHostName(),
                            this.o_config.LocalPort,
                            null
                        );
                    }

                    ForestNET.Lib.Global.ILogConfig("add socket to list");

                    /* add socket to list */
                    this.a_sockets.Add(o_socket);
                }
            }

            /* if a configuration for shared memory bidirectional network communication exchange has been set */
            if (this.o_config.SharedMemoryBidirectional != null)
            {
                ForestNET.Lib.Global.ILogConfig("create new communication instance for shared memory bidirectional network communication exchange");

                /* create new communication instance for shared memory bidirectional network communication exchange */
                this.o_communicationBidirectional = new Communication(this.o_config.SharedMemoryBidirectional);
            }
        }

        /// <summary>
        /// Start network data exchange communication and give any socket object into thread pool + start optional shared memory thread with thread pool
        /// </summary>
        /// <exception cref="Exception">any exception which can happen during creating a new instance of instance to be watched</exception>
        /// <exception cref="ArgumentNullException">communication object or instance to watch object parameter is null</exception>
        /// <exception cref="ArgumentException">invalid timeout value</exception>
		/// <exception cref="MissingMemberException">could not retrieve member type by object member</exception>
		/// <exception cref="MemberAccessException">could not access value from meber, access violation</exception>
        /// <exception cref="InvalidOperationException">communication is already running or no sockets are configured</exception>
        public void Start()
        {
            /* create cancellation token source for all tasks */
            this.CancellationTokenSource = new();

            ForestNET.Lib.Global.ILog("Start communication, " + this.o_config.CommunicationType);

            /* check if communication is already running */
            if (this.Running)
            {
                throw new InvalidOperationException("Communication is running. Please stop communication first");
            }

            /* set running flag */
            this.Running = true;

            /* check socket size */
            if (this.a_sockets.Count == 0)
            {
                throw new InvalidOperationException("no sockets are configured");
            }

            /* if shared memory object is set */
            if (this.o_config.SharedMemory != null)
            {
                ForestNET.Lib.Global.ILogConfig("create shared memory thread");

                /* create shared memory thread */
                this.o_sharedMemoryTask = new(
                    this,
                    this.o_config.SharedMemory,
                    this.o_config.SharedMemoryTimeoutMilliseconds,
                    this.o_config.CommunicationType,
                    this.o_config.SharedMemoryIntervalCompleteRefresh,
                    this.o_config.IsSharedMemoryBidirectionalConfigSet,
                    this.o_config.UseMarshallingWholeObject
                );

                ForestNET.Lib.Global.ILogFine("add shared memory task to execution");

                /* add shared memory task to execution */
                _ = System.Threading.Tasks.Task.Factory.StartNew(async () => await this.o_sharedMemoryTask.RunTask(), this.CancellationTokenSource.Token, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent, TaskScheduler.Default);

                /* need to wait 10 ms after task has been created an started */
                System.Threading.Tasks.Task.Delay(10).Wait();
            }

            for (int i = 0; i < this.a_sockets.Count; i++)
            {
                ForestNET.Lib.Global.ILogFine("add socket object #" + (i + 1) + " task to execution [" + this.a_sockets[i].GetType().FullName + "]");

                /* add socket object #i task to execution */
                _ = System.Threading.Tasks.Task.Factory.StartNew(async () => await this.a_sockets[i].Run(), this.CancellationTokenSource.Token, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent, TaskScheduler.Default);

                /* need to wait 10 ms after task has been created an started */
                System.Threading.Tasks.Task.Delay(10).Wait();
            }

            if (this.o_communicationBidirectional != null)
            {
                ForestNET.Lib.Global.ILogFine("start bidirectional communication");

                /* start bidirectional communication */
                this.o_communicationBidirectional.Start();
            }
        }

        /// <summary>
        /// Stop network data exchange communication
        /// </summary>
        /// <exception cref="InvalidOperationException">communication is not running</exception>
        public void Stop()
        {
            ForestNET.Lib.Global.ILog("Stop communication, " + this.o_config.CommunicationType);

            /* check if communication is not running */
            if (!this.Running)
            {
                throw new InvalidOperationException("Communication is not running. Please start communication first");
            }

            /* unset running flag */
            this.Running = false;

            if (this.o_sharedMemoryTask != null)
            {
                ForestNET.Lib.Global.ILogFine("stop shared memory task");

                /* stop shared memory task */
                this.o_sharedMemoryTask.Stop();
            }

            /* iterate each socket instance */
            for (int i = 0; i < this.a_sockets.Count; i++)
            {
                ForestNET.Lib.Global.ILogFine("stop socket instance #" + (i + 1));

                /* stop socket instance */
                this.a_sockets[i].StopSocket();
            }

            /* wait 3.5 seconds so all can be shut down */
            Thread.Sleep(3500);

            if (this.o_communicationBidirectional != null)
            {
                ForestNET.Lib.Global.ILogFine("stop bidirectional communication");

                /* stop bidirectional communication */
                this.o_communicationBidirectional.Stop();
            }

            /* set cancel signal to all tasks */
            this.CancellationTokenSource?.Cancel();
            this.CancellationTokenSource = null;
        }

        /// <summary>
        /// Enqueue object into first message box
        /// </summary>
        /// <param name="p_o_object">object which will be enqueued</param>
        /// <returns>true - success, false - failure</returns>
        /// <exception cref="InvalidOperationException">communication is not running, wrong communication type</exception>
        /// <exception cref="ArgumentNullException">if marshalling true = parameter object is null</exception>
        /// <exception cref="ArgumentException">invalid message box id parameter, or if marshalling true = data length in bytes must be between 1..4</exception>
        /// <exception cref="NotSupportedException">if marshalling true = little endian system data is NOT IMPLEMENTED</exception>
        /// <exception cref="MissingMemberException">if marshalling true = could not retrieve member type by object member</exception>
        /// <exception cref="System.Reflection.TargetException">if marshalling true = could not retrieve value from member instancec</exception>
        /// <exception cref="MemberAccessException">if marshalling true = could not access value from meber, access violation</exception>
        public bool Enqueue(Object p_o_object)
        {
            return this.Enqueue(1, p_o_object);
        }

        /// <summary>
        /// Enqueue object into message box with id parameter
        /// </summary>
        /// <param name="p_i_messageBoxId">message box id parameter</param>
        /// <param name="p_o_object">object which will be enqueued</param>
        /// <returns>true - success, false - failure</returns>
        /// <exception cref="InvalidOperationException">communication is not running, wrong communication type</exception>
        /// <exception cref="ArgumentNullException">if marshalling true = parameter object is null</exception>
        /// <exception cref="ArgumentException">invalid message box id parameter, or if marshalling true = data length in bytes must be between 1..4</exception>
        /// <exception cref="NotSupportedException">if marshalling true = little endian system data is NOT IMPLEMENTED</exception>
        /// <exception cref="MissingMemberException">if marshalling true = could not retrieve member type by object member</exception>
        /// <exception cref="System.Reflection.TargetException">if marshalling true = could not retrieve value from member instancec</exception>
        /// <exception cref="MemberAccessException">if marshalling true = could not access value from meber, access violation</exception>
        public bool Enqueue(int p_i_messageBoxId, Object p_o_object)
        {
            /* check if communication is not running */
            if (!this.Running)
            {
                throw new InvalidOperationException("Communication is not running. Please start communication first");
            }

            /* if communication is running a TCP server with direct answer, we will never enqueue any network data */
            if (this.o_config.CommunicationType == Type.TCP_RECEIVE_WITH_ANSWER)
            {
                throw new InvalidOperationException("Cannot enqueue object if this side is running with communication type[" + this.o_config.CommunicationType + "].");
            }

            /* check message box id min. value */
            if (p_i_messageBoxId < 1)
            {
                throw new ArgumentException("Invalid message box. Message box id[" + p_i_messageBoxId + "] must be greater than [0].");
            }

            /* check message box id max. value */
            if (p_i_messageBoxId > this.a_messageBoxes.Count)
            {
                throw new ArgumentException("Invalid message box. Message box id[" + p_i_messageBoxId + "] is not available.");
            }

            ForestNET.Lib.Global.ILogFine("execute enqueue object method on message box #" + p_i_messageBoxId);

            /* decrease message box id, because we give a parameter with value > 0 */
            p_i_messageBoxId--;

            if (this.o_config.UseMarshalling)
            {
                /* execute enqueue object method on message box with marshalling */
                return this.a_messageBoxes[p_i_messageBoxId].EnqueueObjectWithMarshalling(p_o_object, this.o_config.MarshallingDataLengthInBytes, this.o_config.MarshallingUseProperties, this.o_config.MarshallingOverrideMessageType, this.o_config.MarshallingSystemUsesLittleEndian);
            }
            else
            {
                /* execute enqueue object method on message box */
                return this.a_messageBoxes[p_i_messageBoxId].EnqueueObject(p_o_object);
            }
        }

        /// <summary>
        /// Dequeue object from first message box
        /// </summary>
        /// <returns>object received from network data exchange communication or null</returns>
        /// <exception cref="InvalidOperationException">communication is not running or wrong communication type</exception>
        /// <exception cref="ArgumentException">invalid message box id parameter, or if marshalling true = data length in bytes must be between 1..4</exception>
        /// <exception cref="ArgumentNullException">if marshalling true = parameter object is null || illegal or missing arguments for constructor</exception>
		/// <exception cref="NullReferenceException">if marshalling true = class information or data parameter are not set</exception>
        /// <exception cref="TypeLoadException">if marshalling true = could not create new instance of return object; class that declares the underlying constructor represents an abstract class</exception>
        /// <exception cref="MemberAccessException">if marshalling true = could not create new instance of return object; constructor object is enforcing language access control and the underlying constructor is inaccessible</exception>
        /// <exception cref="System.Reflection.TargetInvocationException">if marshalling true = could not create new instance of return object; the underlying constructor throws an exception</exception>
        /// <exception cref="MemberAccessException">if marshalling true = could not create new instance of return object; constructor object is enforcing language access control and the underlying constructor is inaccessible</exception>
        /// <exception cref="IndexOutOfRangeException">if marshalling true = data accessed with invalid index</exception>
        /// <exception cref="NotSupportedException">if marshalling true = little endian system data is NOT IMPLEMENTED || could not create new instance of return object; security vulnerabilites while calling the constructor or it is just not supported</exception>
        /// <exception cref="FieldAccessException">if marshalling true = could not access field to set it's value</exception>
        /// <exception cref="MissingMemberException">if marshalling true = could not find a property or field by member, using member info</exception>
        /// <exception cref="System.Reflection.TargetException">if marshalling true = could not invoke property or field from object to set the value</exception>
        /// <exception cref="MethodAccessException">if marshalling true = could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">if marshalling true = cannot convert string to datetime type or transposing decimal, details in exception message</exception>
        public Object? Dequeue()
        {
            /* for tcp bidirectional communication we use second message box for receiving data */
            if (this.o_config.Cardinality == Cardinality.EqualBidirectional)
            {
                return this.Dequeue(2);
            }
            else
            {
                return this.Dequeue(1);
            }
        }

        /// <summary>
        /// Dequeue object from message box with id parameter
        /// </summary>
        /// <param name="p_i_messageBoxId">message box id parameter</param>
        /// <returns>object received from network data exchange communication or null</returns>
        /// <exception cref="InvalidOperationException">communication is not running or wrong communication type</exception>
        /// <exception cref="ArgumentException">invalid message box id parameter, or if marshalling true = data length in bytes must be between 1..4</exception>
        /// <exception cref="ArgumentNullException">if marshalling true = parameter object is null || illegal or missing arguments for constructor</exception>
		/// <exception cref="NullReferenceException">if marshalling true = class information or data parameter are not set</exception>
        /// <exception cref="TypeLoadException">if marshalling true = could not create new instance of return object; class that declares the underlying constructor represents an abstract class</exception>
        /// <exception cref="MemberAccessException">if marshalling true = could not create new instance of return object; constructor object is enforcing language access control and the underlying constructor is inaccessible</exception>
        /// <exception cref="System.Reflection.TargetInvocationException">if marshalling true = could not create new instance of return object; the underlying constructor throws an exception</exception>
        /// <exception cref="MemberAccessException">if marshalling true = could not create new instance of return object; constructor object is enforcing language access control and the underlying constructor is inaccessible</exception>
        /// <exception cref="IndexOutOfRangeException">if marshalling true = data accessed with invalid index</exception>
        /// <exception cref="NotSupportedException">if marshalling true = little endian system data is NOT IMPLEMENTED || could not create new instance of return object; security vulnerabilites while calling the constructor or it is just not supported</exception>
        /// <exception cref="FieldAccessException">if marshalling true = could not access field to set it's value</exception>
        /// <exception cref="MissingMemberException">if marshalling true = could not find a property or field by member, using member info</exception>
        /// <exception cref="System.Reflection.TargetException">if marshalling true = could not invoke property or field from object to set the value</exception>
        /// <exception cref="MethodAccessException">if marshalling true = could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">if marshalling true = cannot convert string to datetime type or transposing decimal, details in exception message</exception>
		public Object? Dequeue(int p_i_messageBoxId)
        {
            /* check if communication is not running */
            if (!this.Running)
            {
                throw new InvalidOperationException("Communication is not running. Please start communication first");
            }

            /* if communication is running a TCP server with direct answer, we will never dequeue any network data */
            if (this.o_config.CommunicationType == Type.TCP_RECEIVE_WITH_ANSWER)
            {
                throw new InvalidOperationException("Cannot dequeue object if this side is running with communication type[" + this.o_config.CommunicationType + "].");
            }

            /* check message box id min. value */
            if (p_i_messageBoxId < 1)
            {
                throw new ArgumentException("Invalid message box. Message box id[" + p_i_messageBoxId + "] must be greater than [0].");
            }

            /* check message box id max. value */
            if (p_i_messageBoxId > this.a_messageBoxes.Count)
            {
                throw new ArgumentException("Invalid message box. Message box id[" + p_i_messageBoxId + "] is not available.");
            }

            /* decrease message box id, because we give a parameter with value > 0 */
            p_i_messageBoxId--;

            if (this.o_config.CommunicationType == Type.TCP_SEND_WITH_ANSWER)
            {
                if (this.o_config.UseMarshalling)
                {
                    /* if we expect direct answer from other communication side, dequeue from answer message box with marshalling */
                    return this.a_answerMessageBoxes[p_i_messageBoxId].DequeueObjectWithMarshalling(this.o_config.MarshallingUseProperties, this.o_config.MarshallingSystemUsesLittleEndian);
                }
                else
                {
                    /* if we expect direct answer from other communication side, dequeue from answer message box */
                    return this.a_answerMessageBoxes[p_i_messageBoxId].DequeueObject();
                }
            }
            else
            {
                if (this.o_config.UseMarshalling)
                {
                    /* dequeue from message box with id parameter with marshalling */
                    return this.a_messageBoxes[p_i_messageBoxId].DequeueObjectWithMarshalling(this.o_config.MarshallingUseProperties, this.o_config.MarshallingSystemUsesLittleEndian);
                }
                else
                {
                    /* dequeue from message box with id parameter */
                    return this.a_messageBoxes[p_i_messageBoxId].DequeueObject();
                }
            }
        }

        /// <summary>
        /// Dequeue object from first message box, do several attempts with an overall timeout
        /// </summary>
        /// <param name="p_i_timeoutMilliseconds">try this amount of time to receive data</param>
        /// <returns>object received from network data exchange communication or null</returns>
        /// <exception cref="InvalidOperationException">communication is not running or wrong communication type</exception>
        /// <exception cref="ArgumentException">invalid message box id parameter, or if marshalling true = data length in bytes must be between 1..4</exception>
        /// <exception cref="ArgumentNullException">if marshalling true = parameter object is null || illegal or missing arguments for constructor</exception>
		/// <exception cref="NullReferenceException">if marshalling true = class information or data parameter are not set</exception>
        /// <exception cref="TypeLoadException">if marshalling true = could not create new instance of return object; class that declares the underlying constructor represents an abstract class</exception>
        /// <exception cref="MemberAccessException">if marshalling true = could not create new instance of return object; constructor object is enforcing language access control and the underlying constructor is inaccessible</exception>
        /// <exception cref="System.Reflection.TargetInvocationException">if marshalling true = could not create new instance of return object; the underlying constructor throws an exception</exception>
        /// <exception cref="MemberAccessException">if marshalling true = could not create new instance of return object; constructor object is enforcing language access control and the underlying constructor is inaccessible</exception>
        /// <exception cref="IndexOutOfRangeException">if marshalling true = data accessed with invalid index</exception>
        /// <exception cref="NotSupportedException">if marshalling true = little endian system data is NOT IMPLEMENTED || could not create new instance of return object; security vulnerabilites while calling the constructor or it is just not supported</exception>
        /// <exception cref="FieldAccessException">if marshalling true = could not access field to set it's value</exception>
        /// <exception cref="MissingMemberException">if marshalling true = could not find a property or field by member, using member info</exception>
        /// <exception cref="System.Reflection.TargetException">if marshalling true = could not invoke property or field from object to set the value</exception>
        /// <exception cref="MethodAccessException">if marshalling true = could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">if marshalling true = cannot convert string to datetime type or transposing decimal, details in exception message</exception>
        public Object? DequeueWithWaitLoop(int p_i_timeoutMilliseconds)
        {
            /* for tcp bidirectional communication we use second message box for receiving data */
            if (this.o_config.Cardinality == Cardinality.EqualBidirectional)
            {
                return this.DequeueWithWaitLoop(p_i_timeoutMilliseconds, 2);
            }
            else
            {
                return this.DequeueWithWaitLoop(p_i_timeoutMilliseconds, 1);
            }
        }

        /// <summary>
        /// Dequeue object from message box with id parameter, do several attempts with an overall timeout
        /// </summary>
        /// <param name="p_i_timeoutMilliseconds">try this amount of time to receive data</param>
        /// <param name="p_i_messageBoxId">message box id parameter</param>
        /// <returns>object received from network data exchange communication or null</returns>
        /// <exception cref="InvalidOperationException">communication is not running or wrong communication type</exception>
        /// <exception cref="ArgumentException">invalid message box id parameter, or if marshalling true = data length in bytes must be between 1..4</exception>
        /// <exception cref="ArgumentNullException">if marshalling true = parameter object is null || illegal or missing arguments for constructor</exception>
		/// <exception cref="NullReferenceException">if marshalling true = class information or data parameter are not set</exception>
        /// <exception cref="TypeLoadException">if marshalling true = could not create new instance of return object; class that declares the underlying constructor represents an abstract class</exception>
        /// <exception cref="MemberAccessException">if marshalling true = could not create new instance of return object; constructor object is enforcing language access control and the underlying constructor is inaccessible</exception>
        /// <exception cref="System.Reflection.TargetInvocationException">if marshalling true = could not create new instance of return object; the underlying constructor throws an exception</exception>
        /// <exception cref="MemberAccessException">if marshalling true = could not create new instance of return object; constructor object is enforcing language access control and the underlying constructor is inaccessible</exception>
        /// <exception cref="IndexOutOfRangeException">if marshalling true = data accessed with invalid index</exception>
        /// <exception cref="NotSupportedException">if marshalling true = little endian system data is NOT IMPLEMENTED || could not create new instance of return object; security vulnerabilites while calling the constructor or it is just not supported</exception>
        /// <exception cref="FieldAccessException">if marshalling true = could not access field to set it's value</exception>
        /// <exception cref="MissingMemberException">if marshalling true = could not find a property or field by member, using member info</exception>
        /// <exception cref="System.Reflection.TargetException">if marshalling true = could not invoke property or field from object to set the value</exception>
        /// <exception cref="MethodAccessException">if marshalling true = could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">if marshalling true = cannot convert string to datetime type or transposing decimal, details in exception message</exception>
        public Object? DequeueWithWaitLoop(int p_i_timeoutMilliseconds, int p_i_messageBoxId)
        {
            Object? o_return = null;

            int j = 0;

            while (o_return == null)
            {
                /* check if data has been received */
                o_return = this.Dequeue(p_i_messageBoxId);

                if (o_return == null)
                {
                    Thread.Sleep(DEQUEUE_WAIT_MILLISECONDS);

                    j++;

                    /* try for amount of time parameter (ms) to receive data */
                    if (j > (p_i_timeoutMilliseconds / DEQUEUE_WAIT_MILLISECONDS))
                    {
                        /* time has elapsed */
                        break;
                    }
                }
                else
                {
                    /* log information how many cycles of wait has been executed */
                    ForestNET.Lib.Global.ILogFiner("waited '" + j + "' cycles for receiving data with a wait ratio(ms) of: " + p_i_timeoutMilliseconds + " / " + DEQUEUE_WAIT_MILLISECONDS + " = " + (p_i_timeoutMilliseconds / DEQUEUE_WAIT_MILLISECONDS));
                }
            }

            return o_return;
        }
    }
}