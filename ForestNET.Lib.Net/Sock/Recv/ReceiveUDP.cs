using System.Net.Sockets;

namespace ForestNET.Lib.Net.Sock.Recv
{
    /// <summary>
    /// Generic socket class to receive network traffic over a socket instance. Several methods supporting receiving data over UDP as server or socket instance.
    /// </summary>
    public class ReceiveUDP : ForestNET.Lib.Net.Sock.Socket
    {

        /* Fields */

        private int i_timeoutTaskPoolMilliseconds;

        /* Properties */

        private ReceiveType RecvType { get; set; }
        public int ServicePoolAmount { get; set; }
        public bool StopServer { get; set; }
        public int TimeoutTaskPoolMilliseconds
        {
            get
            {
                return this.i_timeoutTaskPoolMilliseconds;
            }
            set
            {
                /* check timeout value for task pool */
                if (value < 1)
                {
                    throw new ArgumentException("Queue timeout must be at least '1' millisecond, but was set to '" + value + "' millisecond(s)");
                }

                this.i_timeoutTaskPoolMilliseconds = value;
            }
        }
        public bool IsMulticastSocket { get; set; } = false;

        /* Methods */

        /// <summary>
        /// Create receive socket instance for UDP network connection, executing within a thread. Receive buffer size = 1500 bytes. Infinite thread executions of socket instance. 60 seconds timeout as default.
        /// </summary>
        /// <param name="p_e_type">receiving type of socket instance - SERVER or CLIENT</param>
        /// <param name="p_i_port">port where receiving socket instance shall listen</param>
        /// <param name="p_o_socketTask">socket task object with the core execution process of a socket</param>
        /// <exception cref="ArgumentException">invalid parameter values for port, receive buffer size or timeout</exception>
        /// <exception cref="ArgumentNullException">socket task parameter is null</exception>
        /// <exception cref="FormatException">issue local host name could not be resolved into an address</exception>
        public ReceiveUDP(ReceiveType p_e_type, int p_i_port, ForestNET.Lib.Net.Sock.Task.Task p_o_socketTask) :
            this(p_e_type, System.Net.Dns.GetHostName(), p_i_port, p_o_socketTask, 60000)
        {

        }

        /// <summary>
        /// Create receive socket instance for UDP network connection, executing within a thread. Receive buffer size = 1500 bytes. Infinite thread executions of socket instance. 60 seconds timeout as default.
        /// </summary>
        /// <param name="p_e_type">receiving type of socket instance - SERVER or CLIENT</param>
        /// <param name="p_s_host">address of receiving socket instance</param>
        /// <param name="p_i_port">port where receiving socket instance shall listen</param>
        /// <param name="p_o_socketTask">socket task object with the core execution process of a socket</param>
        /// <exception cref="ArgumentException">invalid parameter values for port, receive buffer size or timeout</exception>
        /// <exception cref="ArgumentNullException">socket task parameter is null</exception>
        /// <exception cref="FormatException">issue local host name could not be resolved into an address</exception>
        public ReceiveUDP(ReceiveType p_e_type, string p_s_host, int p_i_port, ForestNET.Lib.Net.Sock.Task.Task p_o_socketTask) :
            this(p_e_type, p_s_host, p_i_port, p_o_socketTask, 60000)
        {

        }

        /// <summary>
        /// Create receive socket instance for UDP network connection, executing within a thread. Receive buffer size = 1500 bytes. Infinite thread executions of socket instance.
        /// </summary>
        /// <param name="p_e_type">receiving type of socket instance - SERVER or CLIENT</param>
        /// <param name="p_i_port">port where receiving socket instance shall listen</param>
        /// <param name="p_o_socketTask">socket task object with the core execution process of a socket</param>
        /// <param name="p_i_timeoutMilliseconds">set timeout value for socket instance - how long the socket should wait for incoming data so it will not hold forever</param>
        /// <exception cref="ArgumentException">invalid parameter values for port, receive buffer size or timeout</exception>
        /// <exception cref="ArgumentNullException">socket task parameter is null</exception>
        /// <exception cref="FormatException">issue local host name could not be resolved into an address</exception>
        public ReceiveUDP(ReceiveType p_e_type, int p_i_port, ForestNET.Lib.Net.Sock.Task.Task p_o_socketTask, int p_i_timeoutMilliseconds) :
            this(p_e_type, System.Net.Dns.GetHostName(), p_i_port, p_o_socketTask, p_i_timeoutMilliseconds, -1)
        {

        }

        /// <summary>
        /// Create receive socket instance for UDP network connection, executing within a thread. Receive buffer size = 1500 bytes. Infinite thread executions of socket instance.
        /// </summary>
        /// <param name="p_e_type">receiving type of socket instance - SERVER or CLIENT</param>
        /// <param name="p_s_host">address of receiving socket instance</param>
        /// <param name="p_i_port">port where receiving socket instance shall listen</param>
        /// <param name="p_o_socketTask">socket task object with the core execution process of a socket</param>
        /// <param name="p_i_timeoutMilliseconds">set timeout value for socket instance - how long the socket should wait for incoming data so it will not hold forever</param>
        /// <exception cref="ArgumentException">invalid parameter values for port, receive buffer size or timeout</exception>
        /// <exception cref="ArgumentNullException">socket task parameter is null</exception>
        /// <exception cref="FormatException">issue local host name could not be resolved into an address</exception>
        public ReceiveUDP(ReceiveType p_e_type, string p_s_host, int p_i_port, ForestNET.Lib.Net.Sock.Task.Task p_o_socketTask, int p_i_timeoutMilliseconds) :
            this(p_e_type, p_s_host, p_i_port, p_o_socketTask, p_i_timeoutMilliseconds, -1)
        {

        }

        /// <summary>
        /// Create receive socket instance for UDP network connection, executing within a thread. Receive buffer size = 1500 bytes.
        /// </summary>
        /// <param name="p_e_type">receiving type of socket instance - SERVER or CLIENT</param>
        /// <param name="p_i_port">port where receiving socket instance shall listen</param>
        /// <param name="p_o_socketTask">socket task object with the core execution process of a socket</param>
        /// <param name="p_i_timeoutMilliseconds">set timeout value for socket instance - how long the socket should wait for incoming data so it will not hold forever</param>
        /// <param name="p_i_maxTerminations">set a max. value for thread executions of socket instance</param>
        /// <exception cref="ArgumentException">invalid parameter values for port, receive buffer size or timeout</exception>
        /// <exception cref="ArgumentNullException">socket task parameter is null</exception>
        /// <exception cref="FormatException">issue local host name could not be resolved into an address</exception>
        public ReceiveUDP(ReceiveType p_e_type, int p_i_port, ForestNET.Lib.Net.Sock.Task.Task p_o_socketTask, int p_i_timeoutMilliseconds, int p_i_maxTerminations) :
            this(p_e_type, System.Net.Dns.GetHostName(), p_i_port, p_o_socketTask, p_i_timeoutMilliseconds, p_i_maxTerminations, 1500)
        {

        }

        /// <summary>
        /// Create receive socket instance for UDP network connection, executing within a thread. Receive buffer size = 1500 bytes.
        /// </summary>
        /// <param name="p_e_type">receiving type of socket instance - SERVER or CLIENT</param>
        /// <param name="p_s_host">address of receiving socket instance</param>
        /// <param name="p_i_port">port where receiving socket instance shall listen</param>
        /// <param name="p_o_socketTask">socket task object with the core execution process of a socket</param>
        /// <param name="p_i_timeoutMilliseconds">set timeout value for socket instance - how long the socket should wait for incoming data so it will not hold forever</param>
        /// <param name="p_i_maxTerminations">set a max. value for thread executions of socket instance</param>
        /// <exception cref="ArgumentException">invalid parameter values for port, receive buffer size or timeout</exception>
        /// <exception cref="ArgumentNullException">socket task parameter is null</exception>
        /// <exception cref="FormatException">issue local host name could not be resolved into an address</exception>
        public ReceiveUDP(ReceiveType p_e_type, string p_s_host, int p_i_port, ForestNET.Lib.Net.Sock.Task.Task p_o_socketTask, int p_i_timeoutMilliseconds, int p_i_maxTerminations) :
            this(p_e_type, p_s_host, p_i_port, p_o_socketTask, p_i_timeoutMilliseconds, p_i_maxTerminations, 1500)
        {

        }

        /// <summary>
        /// Create receive socket instance for UDP network connection, executing within a thread.
        /// </summary>
        /// <param name="p_e_type">receiving type of socket instance - SERVER or CLIENT</param>
        /// <param name="p_i_port">port where receiving socket instance shall listen</param>
        /// <param name="p_o_socketTask">socket task object with the core execution process of a socket</param>
        /// <param name="p_i_timeoutMilliseconds">set timeout value for socket instance - how long the socket should wait for incoming data so it will not hold forever</param>
        /// <param name="p_i_maxTerminations">set a max. value for thread executions of socket instance</param>
        /// <param name="p_i_receiveBufferSize">receive buffer size for socket instance, e.g. ethernet packets have max. data length of 1500 bytes</param>
        /// <exception cref="ArgumentException">invalid parameter values for port, receive buffer size or timeout</exception>
        /// <exception cref="ArgumentNullException">socket task parameter is null</exception>
        /// <exception cref="FormatException">issue local host name could not be resolved into an address</exception>
        public ReceiveUDP(ReceiveType p_e_type, int p_i_port, ForestNET.Lib.Net.Sock.Task.Task p_o_socketTask, int p_i_timeoutMilliseconds, int p_i_maxTerminations, int p_i_receiveBufferSize) :
            this(p_e_type, System.Net.Dns.GetHostName(), p_i_port, p_o_socketTask, p_i_timeoutMilliseconds, p_i_maxTerminations, p_i_receiveBufferSize)
        {

        }

        /// <summary>
        /// Create receive socket instance for UDP network connection, executing within a thread.
        /// </summary>
        /// <param name="p_e_type">receiving type of socket instance - SERVER or CLIENT</param>
        /// <param name="p_s_host">address of receiving socket instance</param>
        /// <param name="p_i_port">port where receiving socket instance shall listen</param>
        /// <param name="p_o_socketTask">socket task object with the core execution process of a socket</param>
        /// <param name="p_i_timeoutMilliseconds">set timeout value for socket instance - how long the socket should wait for incoming data so it will not hold forever</param>
        /// <param name="p_i_maxTerminations">set a max. value for thread executions of socket instance</param>
        /// <param name="p_i_receiveBufferSize">receive buffer size for socket instance, e.g. ethernet packets have max. data length of 1500 bytes</param>
        /// <exception cref="ArgumentException">invalid parameter values for port, receive buffer size or timeout</exception>
        /// <exception cref="ArgumentNullException">socket task parameter is null</exception>
        /// <exception cref="FormatException">issue local host name could not be resolved into an address</exception>
        public ReceiveUDP(ReceiveType p_e_type, string p_s_host, int p_i_port, ForestNET.Lib.Net.Sock.Task.Task p_o_socketTask, int p_i_timeoutMilliseconds, int p_i_maxTerminations, int p_i_receiveBufferSize)
        {
            /* set variables with parameter and default values */
            this.RecvType = p_e_type;
            this.ServicePoolAmount = 0;
            this.Stop = false;
            this.StopServer = false;
            this.Terminations = 0;

            /* check port min. value */
            if (p_i_port < 1)
            {
                throw new ArgumentException("Receive port must be at least '1', but was set to '" + p_i_port + "'");
            }

            /* check port max. value */
            if (p_i_port > 65535)
            {
                throw new ArgumentException("Receive port must be lower equal '65535', but was set to '" + p_i_port + "'");
            }

            ForestNET.Lib.Global.ILogConfig("\t" + "set port '" + p_i_port + "'");

            /* check receive buffer size min. value */
            if (p_i_receiveBufferSize < 1)
            {
                throw new ArgumentException("Buffer size must be at least '1', but was set to '" + p_i_receiveBufferSize + "'");
            }
            else
            {
                this.BufferSize = p_i_receiveBufferSize;
            }

            ForestNET.Lib.Global.ILogConfig("\t" + "set receive buffer size '" + p_i_receiveBufferSize + "'");

            /* set socket timeout value if it is a positive integer value */
            if (p_i_timeoutMilliseconds > 0)
            {
                ForestNET.Lib.Global.ILogConfig("\t" + "set socket timeout value '" + p_i_timeoutMilliseconds + "'");

                this.TimeoutMilliseconds = p_i_timeoutMilliseconds;

                ForestNET.Lib.Global.ILogConfig("\t" + "set socket timeout task pool value '" + p_i_timeoutMilliseconds + "'");

                this.TimeoutTaskPoolMilliseconds = p_i_timeoutMilliseconds;
            }
            else
            {
                throw new ArgumentException("Receive timeout must be at least '1' millisecond, but was set to '" + p_i_timeoutMilliseconds + "' millisecond(s)");
            }

            if (p_i_maxTerminations < 0)
            {
                ForestNET.Lib.Global.ILogConfig("\t" + "there is no termination limit");

                /* there is no termination limit */
                this.MaxTerminations = -1;
            }
            else
            {
                ForestNET.Lib.Global.ILogConfig("\t" + "set max. termination limit '" + p_i_maxTerminations + "'");

                /* set max. termination limit */
                this.MaxTerminations = p_i_maxTerminations;
            }

            /* get local host value if host parameter is 'localhost' */
            if (p_s_host.ToLower().Equals("localhost"))
            {
                p_s_host = System.Net.Dns.GetHostName();

                ForestNET.Lib.Global.ILogConfig("\t" + "set host to local host value '" + p_s_host + "'");
            }

            try
            {
                ForestNET.Lib.Global.ILogConfig("\t" + "set socket address value with host and port parameter '" + p_s_host + ":" + p_i_port + "'");

                /* set socket address value with host and port parameter */
                this.SocketAddress = new(System.Net.IPAddress.Parse(p_s_host), p_i_port);
            }
            catch (FormatException o_exc)
            {
                throw new ArgumentException("Unknown host[" + p_s_host + "] - " + o_exc);
            }
            catch (ArgumentOutOfRangeException o_exc)
            {
                throw new ArgumentException("Unknown host[" + p_s_host + "] - " + o_exc);
            }

            /* set socket task from parameter */
            if (p_o_socketTask == null)
            {
                throw new ArgumentNullException(nameof(p_o_socketTask), "No socket task specified");
            }
            else
            {
                this.SocketTask = p_o_socketTask;
            }
        }

        /// <summary>
        /// Core execution process method of a socket. Receiving data as a simple socket or as a server instance.
        /// </summary>
        public override async System.Threading.Tasks.Task Run()
        {
            this.Stop = false;
            this.StopServer = false;

            if (this.RecvType == ReceiveType.SOCKET)
            {
                ForestNET.Lib.Global.ILogConfig("\t" + "run as receiving socket(UDP) instance");

                await this.RunSocket();
            }
            else if (this.RecvType == ReceiveType.SERVER)
            {
                ForestNET.Lib.Global.ILogConfig("\t" + "run as receiving server(UDP) instance");

                await this.RunServer();
            }
        }

        /// <summary>
        /// This method stops the socket and ends any network communication.
        /// </summary>
        public override void StopSocket()
        {
            this.Stop = true;
            this.StopServer = true;
            this.CancellationTokenSource?.Cancel();
        }

        /// <summary>
        /// Execute receive socket instance.
        /// </summary>
        private async System.Threading.Tasks.Task RunSocket()
        {
            try
            {
                /* check if socket task is set */
                if (this.SocketTask == null)
                {
                    throw new ArgumentNullException("There was no socket task specified");
                }

                /* check if socket address is set */
                if (this.SocketAddress == null)
                {
                    throw new ArgumentNullException("There was no socket address specified");
                }

                /* check for valid multicast address */
                if (this.IsMulticastSocket)
                {
                    if (this.SocketAddress.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        if (!ForestNET.Lib.Helper.IsIpv4MulticastAddress(this.SocketAddress.Address.ToString()))
                        {
                            throw new ArgumentException("Invalid Multicast address '" + this.SocketAddress.Address.ToString() + "'. Valid: 224.0.0.0 - 239.255.255.255");
                        }
                    }
                    else
                    {
                        if (!ForestNET.Lib.Helper.IsIpv6MulticastAddress(this.SocketAddress.Address.ToString()))
                        {
                            throw new ArgumentException("Invalid Multicast address '" + this.SocketAddress.Address.ToString() + "'.");
                        }
                    }
                }

                ForestNET.Lib.Global.ILogConfig(((this.SocketTask.CommunicationType != null) ? this.SocketTask.CommunicationType + " " : "") + "socket bind to " + this.SocketAddress);

                /* create receive socket and bind to configured address */
                this.SocketInstance = new(this.SocketAddress.AddressFamily, System.Net.Sockets.SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp);

                System.Net.IPAddress o_anyAddress = System.Net.IPAddress.Any;

                if (this.SocketAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    o_anyAddress = System.Net.IPAddress.IPv6Any;
                    this.SocketInstance.SetSocketOption(System.Net.Sockets.SocketOptionLevel.IPv6, System.Net.Sockets.SocketOptionName.ReuseAddress, true);
                }
                else
                {
                    this.SocketInstance.SetSocketOption(System.Net.Sockets.SocketOptionLevel.IP, System.Net.Sockets.SocketOptionName.ReuseAddress, true);
                }

                ForestNET.Lib.Global.ILogConfig("created receive socket and bind to configured address");

                /* set receive buffer size */
                this.SocketInstance.ReceiveBufferSize = this.BufferSize;
                /* set socket timeout value */
                this.SocketInstance.ReceiveTimeout = this.TimeoutMilliseconds;

                /* bind socket to address */
                if (!this.IsMulticastSocket)
                {
                    /* use socket address */
                    this.SocketInstance.Bind(this.SocketAddress);
                }
                else
                {
                    /* use any ip and socket address port for multicast */
                    this.SocketInstance.Bind(new System.Net.IPEndPoint(o_anyAddress, this.SocketAddress.Port));
                }

                /* create cancellation token source for all socket tasks */
                this.CancellationTokenSource = new();

                /* add membership for multicast */
                if (this.IsMulticastSocket)
                {
                    o_anyAddress = System.Net.IPAddress.Any;

                    if (this.SocketAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    {
                        o_anyAddress = System.Net.IPAddress.IPv6Any;
                        this.SocketInstance.SetSocketOption(System.Net.Sockets.SocketOptionLevel.IPv6, System.Net.Sockets.SocketOptionName.AddMembership, new System.Net.Sockets.IPv6MulticastOption(this.SocketAddress.Address));
                    }
                    else
                    {
                        this.SocketInstance.SetSocketOption(System.Net.Sockets.SocketOptionLevel.IP, System.Net.Sockets.SocketOptionName.AddMembership, new System.Net.Sockets.MulticastOption(this.SocketAddress.Address, o_anyAddress));
                    }
                }

                /* endless loop for our receiving socket instance */
                while (!this.Stop)
                {
                    /* create cancellation token source to interrupt socket instance, so we have not an endless loop */
                    CancellationTokenSource o_timeoutCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(new CancellationTokenSource(this.TimeoutMilliseconds).Token, this.CancellationTokenSource?.Token ?? default);

                    /* create byte array with expected receiving buffer size */
                    byte[] a_datagramPacketBytes = new byte[this.BufferSize];

                    try
                    {
                        ForestNET.Lib.Global.ILogFine("this.SocketInstance.ReceiveFromAsync");

                        /* socket instance waiting for connection */
                        System.Net.Sockets.SocketReceiveFromResult o_socketReceiveFromResult = await this.SocketInstance.ReceiveFromAsync(a_datagramPacketBytes, System.Net.Sockets.SocketFlags.None, new System.Net.IPEndPoint(o_anyAddress, 0), o_timeoutCancellationTokenSource.Token);

                        ForestNET.Lib.Global.ILogFine("ReceiveUDP-RunServer method - incoming connection [" + o_socketReceiveFromResult.RemoteEndPoint + "]");

                        /* pass socket instance for answering request to socket task */
                        this.SocketTask.Socket = this;
                        /* pass UDP source address to socket task */
                        this.SocketTask.UDPSourceAddress = (System.Net.IPEndPoint?)o_socketReceiveFromResult.RemoteEndPoint;
                        /* pass buffer size length to socket task */
                        this.SocketTask.BufferLength = this.BufferSize;
                        /* pass received UDP datagram bytes to socket task */
                        this.SocketTask.DatagramBytes = a_datagramPacketBytes;
                        /* set cancellation token for socket task */
                        this.SocketTask.CancellationToken = this.CancellationTokenSource?.Token;
                        /* run socket task */
                        await this.SocketTask.Run();

                        if (!this.Stop)
                        { /* set stop flag from socket task, if it is not already set, to end endless loop */
                            this.Stop = this.SocketTask.Stop;
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        ForestNET.Lib.Global.ILogFine("OperationCanceledException ReceiveUDP-RunSocket method - Terminations: " + (this.Terminations + 1) + "/" + this.MaxTerminations);
                    }
                    catch (System.IO.IOException o_exc)
                    {
                        if (!this.Stop)
                        { /* exception occurred without control stop */
                            ForestNET.Lib.Global.LogException("IOException ReceiveUDP-RunSocket method: ", o_exc);
                            ForestNET.Lib.Global.ILogSevere("\tTerminations: " + (this.Terminations + 1) + "/" + this.MaxTerminations);
                        }
                    }
                    finally
                    {
                        /* check amount of cycles in socket instance thread and if max. value for thread executions have been exceeded */
                        this.Terminations++;

                        if (this.MaxTerminations > -1)
                        {
                            if (this.Terminations >= this.MaxTerminations)
                            {
                                this.Stop = true;
                            }
                        }

                        ForestNET.Lib.Global.ILogFiner("request cycle completed by socket instance; stopped: " + this.Stop);
                    }
                }
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.LogException("Exception ReceiveUDP-RunSocket method: ", o_exc);
            }
            finally
            {
                /* drop membership for multicast */
                if ((this.IsMulticastSocket) && (this.SocketAddress != null))
                {
                    if (this.SocketAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    {
                        this.SocketInstance?.SetSocketOption(System.Net.Sockets.SocketOptionLevel.IPv6, System.Net.Sockets.SocketOptionName.DropMembership, new System.Net.Sockets.IPv6MulticastOption(this.SocketAddress.Address));
                    }
                    else
                    {
                        this.SocketInstance?.SetSocketOption(System.Net.Sockets.SocketOptionLevel.IP, System.Net.Sockets.SocketOptionName.DropMembership, new System.Net.Sockets.MulticastOption(this.SocketAddress.Address, System.Net.IPAddress.Any));
                    }
                }

                /* set cancel signal to all socket tasks */
                this.CancellationTokenSource?.Cancel();
                this.CancellationTokenSource = null;
                /* close socket */
                this.SocketInstance?.Close();
                this.SocketInstance = null;

                ForestNET.Lib.Global.ILogConfig("ReceiveUDP-RunSocket method stopped");
            }
        }

        /// <summary>
        /// Execute receive server instance.
        /// </summary>
        private async System.Threading.Tasks.Task RunServer()
        {
            try
            {
                /* check if socket task is set */
                if (this.SocketTask == null)
                {
                    throw new ArgumentNullException("There was no socket task specified");
                }

                /* check if socket address is set */
                if (this.SocketAddress == null)
                {
                    throw new ArgumentNullException("There was no socket address specified");
                }

                /* check for valid multicast address */
                if (this.IsMulticastSocket)
                {
                    if (this.SocketAddress.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        if (!ForestNET.Lib.Helper.IsIpv4MulticastAddress(this.SocketAddress.Address.ToString()))
                        {
                            throw new ArgumentException("Invalid Multicast address '" + this.SocketAddress.Address.ToString() + "'. Valid: 224.0.0.0 - 239.255.255.255");
                        }
                    }
                    else
                    {
                        if (!ForestNET.Lib.Helper.IsIpv6MulticastAddress(this.SocketAddress.Address.ToString()))
                        {
                            throw new ArgumentException("Invalid Multicast address '" + this.SocketAddress.Address.ToString() + "'.");
                        }
                    }
                }

                ForestNET.Lib.Global.ILogConfig(((this.SocketTask.CommunicationType != null) ? this.SocketTask.CommunicationType + " " : "") + "socket bind to " + this.SocketAddress);

                /* create receive server and bind to configured address */
                this.SocketInstance = new(this.SocketAddress.AddressFamily, System.Net.Sockets.SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp);

                System.Net.IPAddress o_anyAddress = System.Net.IPAddress.Any;

                if (this.SocketAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    o_anyAddress = System.Net.IPAddress.IPv6Any;
                    this.SocketInstance.SetSocketOption(System.Net.Sockets.SocketOptionLevel.IPv6, System.Net.Sockets.SocketOptionName.ReuseAddress, true);
                }
                else
                {
                    this.SocketInstance.SetSocketOption(System.Net.Sockets.SocketOptionLevel.IP, System.Net.Sockets.SocketOptionName.ReuseAddress, true);
                }

                ForestNET.Lib.Global.ILogConfig("created receive server and bind to configured address");

                /* set receive buffer size */
                this.SocketInstance.ReceiveBufferSize = this.BufferSize;
                /* set socket timeout value */
                this.SocketInstance.ReceiveTimeout = this.TimeoutMilliseconds;

                /* bind socket to address */
                if (!this.IsMulticastSocket)
                {
                    /* use socket address */
                    this.SocketInstance.Bind(this.SocketAddress);
                }
                else
                {
                    /* use any ip and socket address port for multicast */
                    this.SocketInstance.Bind(new System.Net.IPEndPoint(o_anyAddress, this.SocketAddress.Port));
                }

                /* create cancellation token source for all socket tasks */
                this.CancellationTokenSource = new();
                /* create semaphore for task pool, initially */
                SemaphoreSlim o_taskPool = new(0);

                /* add membership for multicast */
                if (this.IsMulticastSocket)
                {
                    o_anyAddress = System.Net.IPAddress.Any;

                    if (this.SocketAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    {
                        o_anyAddress = System.Net.IPAddress.IPv6Any;
                        this.SocketInstance.SetSocketOption(System.Net.Sockets.SocketOptionLevel.IPv6, System.Net.Sockets.SocketOptionName.AddMembership, new System.Net.Sockets.IPv6MulticastOption(this.SocketAddress.Address));
                    }
                    else
                    {
                        this.SocketInstance.SetSocketOption(System.Net.Sockets.SocketOptionLevel.IP, System.Net.Sockets.SocketOptionName.AddMembership, new System.Net.Sockets.MulticastOption(this.SocketAddress.Address, o_anyAddress));
                    }
                }

                /* check if we should reinstiate task pool with service pool amount value greater than '0' */
                if (this.ServicePoolAmount > 0)
                {
                    ForestNET.Lib.Global.ILogConfig("create task pool with amout of '" + this.ServicePoolAmount + "'");

                    o_taskPool = new(this.ServicePoolAmount);
                }

                /* endless loop for our receiving server instance */
                while (!this.StopServer)
                {
                    /* create cancellation token source to interrupt socket instance, so we have not an endless loop */
                    CancellationTokenSource o_timeoutCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(new CancellationTokenSource(this.TimeoutMilliseconds).Token, this.CancellationTokenSource?.Token ?? default);

                    /* create byte array with expected receiving buffer size */
                    byte[] a_datagramPacketBytes = new byte[this.BufferSize];

                    try
                    {
                        /* create new instance of socket task, because for every request we need a new one; otherwise we just overwrite the socket instance and do chaos to our communication and so on */
                        ForestNET.Lib.Net.Sock.Task.Task o_socketTaskInstance = (ForestNET.Lib.Net.Sock.Task.Task)(Activator.CreateInstance(this.SocketTask.GetType()) ?? throw new TypeLoadException("could not create new instance of socket task"));
                        o_socketTaskInstance.CloneFromOtherTask(this.SocketTask);

                        ForestNET.Lib.Global.ILogFine("this.SocketInstance.ReceiveFromAsync");

                        /* socket instance waiting for connection */
                        System.Net.Sockets.SocketReceiveFromResult o_socketReceiveFromResult = await this.SocketInstance.ReceiveFromAsync(a_datagramPacketBytes, System.Net.Sockets.SocketFlags.None, new System.Net.IPEndPoint(o_anyAddress, 0), o_timeoutCancellationTokenSource.Token);

                        ForestNET.Lib.Global.ILogFine("ReceiveUDP-RunServer method - incoming connection [" + o_socketReceiveFromResult.RemoteEndPoint + "]");

                        /* pass socket instance for answering request to socket task */
                        o_socketTaskInstance.Socket = this;
                        /* pass UDP source address to socket task */
                        o_socketTaskInstance.UDPSourceAddress = (System.Net.IPEndPoint?)o_socketReceiveFromResult.RemoteEndPoint;
                        /* pass buffer size length to socket task */
                        o_socketTaskInstance.BufferLength = this.BufferSize;
                        /* pass received UDP datagram bytes to socket task */
                        o_socketTaskInstance.DatagramBytes = a_datagramPacketBytes;
                        /* set cancellation token for socket task */
                        o_socketTaskInstance.CancellationToken = this.CancellationTokenSource?.Token;
                        /* run socket task as detached child task, but keep in touch by adding cancellation token */
                        _ = System.Threading.Tasks.Task.Factory.StartNew(
                            async () =>
                            {
                                /* only use task pool if service pool amount is greater than '0' */
                                if (this.ServicePoolAmount > 0)
                                {
                                    try
                                    {
                                        System.Threading.Tasks.Task o_taskWaitPool = o_taskPool.WaitAsync(this.CancellationTokenSource?.Token ?? default);
                                        System.Threading.Tasks.Task o_taskTimeoutPool = System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(this.TimeoutTaskPoolMilliseconds), this.CancellationTokenSource?.Token ?? default);

                                        System.Threading.Tasks.Task o_completedTaskPool = await System.Threading.Tasks.Task.WhenAny(o_taskWaitPool, o_taskTimeoutPool);

                                        if (o_completedTaskPool == o_taskWaitPool)
                                        {
                                            /* execute socket task instance */
                                            await o_socketTaskInstance.Run();
                                        }
                                        else
                                        {
                                            if (!o_taskTimeoutPool.IsCanceled && !o_taskTimeoutPool.IsCompleted && !o_taskTimeoutPool.IsFaulted)
                                            {
                                                o_taskTimeoutPool.Dispose();
                                            }

                                            throw new OperationCanceledException("timeout for waiting in task pool is over");
                                        }
                                    }
                                    finally
                                    {
                                        /* release one place in task pool */
                                        o_taskPool.Release();
                                    }
                                }
                                else /* otherwise just start socket task instance */
                                {
                                    await o_socketTaskInstance.Run();
                                }
                            },
                            this.CancellationTokenSource?.Token ?? default, TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach, TaskScheduler.Default
                        );
                    }
                    catch (OperationCanceledException)
                    {
                        ForestNET.Lib.Global.ILogFine("OperationCanceledException ReceiveUDP-RunServer method - Terminations: " + (this.Terminations + 1) + "/" + this.MaxTerminations);
                    }
                    catch (System.IO.IOException o_exc)
                    {
                        if (!this.StopServer)
                        { /* exception occurred without control stop */
                            ForestNET.Lib.Global.LogException("IOException ReceiveUDP-RunServer method: ", o_exc);
                            ForestNET.Lib.Global.ILogSevere("\tTerminations: " + (this.Terminations + 1) + "/" + this.MaxTerminations);
                        }
                    }
                    finally
                    {
                        /* check amount of cycles in server instance thread and if max. value for thread executions have been exceeded */
                        this.Terminations++;

                        if (this.MaxTerminations > -1)
                        {
                            if (this.Terminations >= this.MaxTerminations)
                            {
                                this.StopServer = true;
                            }
                        }

                        ForestNET.Lib.Global.ILogFiner("request passed to thread pool by server instance; stopped: " + this.StopServer);
                    }
                }
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.LogException("Exception ReceiveUDP-RunServer method: ", o_exc);
            }
            finally
            {
                /* drop membership for multicast */
                if ((this.IsMulticastSocket) && (this.SocketAddress != null))
                {
                    if (this.SocketAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    {
                        this.SocketInstance?.SetSocketOption(System.Net.Sockets.SocketOptionLevel.IPv6, System.Net.Sockets.SocketOptionName.DropMembership, new System.Net.Sockets.IPv6MulticastOption(this.SocketAddress.Address));
                    }
                    else
                    {
                        this.SocketInstance?.SetSocketOption(System.Net.Sockets.SocketOptionLevel.IP, System.Net.Sockets.SocketOptionName.DropMembership, new System.Net.Sockets.MulticastOption(this.SocketAddress.Address, System.Net.IPAddress.Any));
                    }
                }

                /* set cancel signal to all socket tasks */
                this.CancellationTokenSource?.Cancel();
                this.CancellationTokenSource = null;

                /* wait 2.5 seconds so cancel signal reaching all socket tasks */
                await System.Threading.Tasks.Task.Delay(2500);

                /* close socket */
                this.SocketInstance?.Close();
                this.SocketInstance = null;

                ForestNET.Lib.Global.ILogConfig("ReceiveUDP-RunServer method stopped");
            }
        }
    }
}