using System.Net.Sockets;

namespace ForestNET.Lib.Net.Sock.Send
{
    /// <summary>
    /// Generic socket class to send network traffic over a socket instance. Several methods supporting sending data over UDP.
    /// </summary>
    public class SendUDP : ForestNET.Lib.Net.Sock.Socket
    {

        /* Fields */

        /* Properties */

        private System.Net.IPEndPoint? SocketLocalAddress { get; set; }
        private int IntervalMilliseconds { get; set; }
        public int Timeout { get { return this.TimeoutMilliseconds; } set { this.TimeoutMilliseconds = value; } }
        public bool IsMulticastSocket { get; set; } = false;
        public int MulticastTTL { get; set; } = 1;

        /* Methods */

        /// <summary>
        /// Create send socket instance for UDP network connection, executing within a thread. Auto determine local address of this socket instance. Buffer size = 1500 bytes.  Execute socket thread just once. 60 seconds timeout as default.
        /// </summary>
        /// <param name="p_s_host">address of destination for socket instance</param>
        /// <param name="p_i_port">port of destination for socket instance</param>
        /// <param name="p_o_socketReceiveTask">socket task object with the core execution process of a socket</param>
        /// <exception cref="ArgumentException">invalid parameter values for ports, buffer size or timeout</exception>
        /// <exception cref="ArgumentNullException">socket task parameter is null</exception>
        /// <exception cref="FormatException">issue local host name could not be resolved into an address</exception>
        public SendUDP(string p_s_host, int p_i_port, ForestNET.Lib.Net.Sock.Task.Task p_o_socketReceiveTask) :
            this(p_s_host, p_i_port, p_o_socketReceiveTask, 60000)
        {

        }

        /// <summary>
        /// Create send socket instance for UDP network connection, executing within a thread. Auto determine local address of this socket instance. Buffer size = 1500 bytes.  Execute socket thread just once.
        /// </summary>
        /// <param name="p_s_host">address of destination for socket instance</param>
        /// <param name="p_i_port">port of destination for socket instance</param>
        /// <param name="p_o_socketReceiveTask">socket task object with the core execution process of a socket</param>
        /// <param name="p_i_intervalMilliseconds">interval for waiting for other communication side or new data to send</param>
        /// <exception cref="ArgumentException">invalid parameter values for ports, buffer size or timeout</exception>
        /// <exception cref="ArgumentNullException">socket task parameter is null</exception>
        /// <exception cref="FormatException">issue local host name could not be resolved into an address</exception>
        public SendUDP(string p_s_host, int p_i_port, ForestNET.Lib.Net.Sock.Task.Task p_o_socketReceiveTask, int p_i_intervalMilliseconds) :
            this(p_s_host, p_i_port, p_o_socketReceiveTask, p_i_intervalMilliseconds, 1)
        {

        }

        /// <summary>
        /// Create send socket instance for UDP network connection, executing within a thread. Auto determine local address of this socket instance. Buffer size = 1500 bytes.
        /// </summary>
        /// <param name="p_s_host">address of destination for socket instance</param>
        /// <param name="p_i_port">port of destination for socket instance</param>
        /// <param name="p_o_socketReceiveTask">socket task object with the core execution process of a socket</param>
        /// <param name="p_i_intervalMilliseconds">interval for waiting for other communication side or new data to send</param>
        /// <param name="p_i_maxTerminations">set a max. value for thread executions of socket instance</param>
        /// <exception cref="ArgumentException">invalid parameter values for ports, buffer size or timeout</exception>
        /// <exception cref="ArgumentNullException">socket task parameter is null</exception>
        /// <exception cref="FormatException">issue local host name could not be resolved into an address</exception>
        public SendUDP(string p_s_host, int p_i_port, ForestNET.Lib.Net.Sock.Task.Task p_o_socketReceiveTask, int p_i_intervalMilliseconds, int p_i_maxTerminations) :
            this(p_s_host, p_i_port, p_o_socketReceiveTask, p_i_intervalMilliseconds, p_i_maxTerminations, 1500)
        {

        }

        /// <summary>
        /// Create send socket instance for UDP network connection, executing within a thread. Auto determine local address of this socket instance.
        /// </summary>
        /// <param name="p_s_host">address of destination for socket instance</param>
        /// <param name="p_i_port">port of destination for socket instance</param>
        /// <param name="p_o_socketReceiveTask">socket task object with the core execution process of a socket</param>
        /// <param name="p_i_intervalMilliseconds">interval for waiting for other communication side or new data to send</param>
        /// <param name="p_i_maxTerminations">set a max. value for thread executions of socket instance</param>
        /// <param name="p_i_bufferSize">buffer size for socket instance, e.g. ethernet packets have max. data length of 1500 bytes</param>
        /// <exception cref="ArgumentException">invalid parameter values for ports, buffer size or timeout</exception>
        /// <exception cref="ArgumentNullException">socket task parameter is null</exception>
        /// <exception cref="FormatException">issue local host name could not be resolved into an address</exception>
        public SendUDP(string p_s_host, int p_i_port, ForestNET.Lib.Net.Sock.Task.Task p_o_socketReceiveTask, int p_i_intervalMilliseconds, int p_i_maxTerminations, int p_i_bufferSize) :
            this(p_s_host, p_i_port, p_o_socketReceiveTask, p_i_intervalMilliseconds, p_i_maxTerminations, p_i_bufferSize, System.Net.Dns.GetHostName(), 0)
        {

        }

        /// <summary>
        /// Create send socket instance for UDP network connection, executing within a thread. Auto determine local address of this socket instance.
        /// </summary>
        /// <param name="p_s_host">address of destination for socket instance</param>
        /// <param name="p_i_port">port of destination for socket instance</param>
        /// <param name="p_o_socketReceiveTask">socket task object with the core execution process of a socket</param>
        /// <param name="p_i_intervalMilliseconds">interval for waiting for other communication side or new data to send</param>
        /// <param name="p_i_maxTerminations">set a max. value for thread executions of socket instance</param>
        /// <param name="p_i_bufferSize">buffer size for socket instance, e.g. ethernet packets have max. data length of 1500 bytes</param>
        /// <param name="p_i_localPort">local port of this socket instance</param>
        /// <exception cref="ArgumentException">invalid parameter values for ports, buffer size or timeout</exception>
        /// <exception cref="ArgumentNullException">socket task parameter is null</exception>
        /// <exception cref="FormatException">issue local host name could not be resolved into an address</exception>
        public SendUDP(string p_s_host, int p_i_port, ForestNET.Lib.Net.Sock.Task.Task p_o_socketReceiveTask, int p_i_intervalMilliseconds, int p_i_maxTerminations, int p_i_bufferSize, int p_i_localPort) :
            this(p_s_host, p_i_port, p_o_socketReceiveTask, p_i_intervalMilliseconds, p_i_maxTerminations, p_i_bufferSize, System.Net.Dns.GetHostName(), p_i_localPort)
        {

        }

        /// <summary>
        /// Create send socket instance for UDP network connection, executing within a thread.
        /// </summary>
        /// <param name="p_s_host">address of destination for socket instance</param>
        /// <param name="p_i_port">port of destination for socket instance</param>
        /// <param name="p_o_socketReceiveTask">socket task object with the core execution process of a socket</param>
        /// <param name="p_i_intervalMilliseconds">interval for waiting for other communication side or new data to send</param>
        /// <param name="p_i_maxTerminations">set a max. value for thread executions of socket instance</param>
        /// <param name="p_i_bufferSize">buffer size for socket instance, e.g. ethernet packets have max. data length of 1500 bytes</param>
        /// <param name="p_s_localAddress">local address of this socket instance</param>
        /// <param name="p_i_localPort">local port of this socket instance</param>
        /// <exception cref="ArgumentException">invalid parameter values for ports, buffer size or timeout</exception>
        /// <exception cref="ArgumentNullException">socket task parameter is null</exception>
        /// <exception cref="FormatException">issue local host name could not be resolved into an address</exception>
        public SendUDP(string p_s_host, int p_i_port, ForestNET.Lib.Net.Sock.Task.Task p_o_socketReceiveTask, int p_i_intervalMilliseconds, int p_i_maxTerminations, int p_i_bufferSize, string p_s_localAddress, int p_i_localPort)
        {
            /* set variables with parameter and default values */
            this.Stop = false;
            this.Terminations = 0;

            /* check port min. value */
            if (p_i_port < 1)
            {
                throw new ArgumentException("Send port must be at least '1', but was set to '" + p_i_port + "'");
            }

            /* check port max. value */
            if (p_i_port > 65535)
            {
                throw new ArgumentException("Send port must be lower equal '65535', but was set to '" + p_i_port + "'");
            }

            ForestNET.Lib.Global.ILogConfig("\t" + "set port '" + p_i_port + "'");

            if (p_i_intervalMilliseconds < 0)
            {
                ForestNET.Lib.Global.ILogConfig("\t" + "there is no interval for waiting for other communication side or new data to send");

                /* there is no interval for waiting for other communication side or new data to send */
                this.IntervalMilliseconds = 0;
            }
            else
            {
                ForestNET.Lib.Global.ILogConfig("\t" + "set interval '" + p_i_intervalMilliseconds + "' for waiting for other communication side or new data to send");

                /* set interval for waiting for other communication side or new data to send */
                this.IntervalMilliseconds = p_i_intervalMilliseconds;
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

            /* check buffer size min. value */
            if (p_i_bufferSize < 1)
            {
                throw new ArgumentException("Buffer size must be at least '1', but was set to '" + p_i_bufferSize + "'");
            }
            else
            {
                this.BufferSize = p_i_bufferSize;
            }

            ForestNET.Lib.Global.ILogConfig("\t" + "set buffer size '" + p_i_bufferSize + "'");

            /* check local port parameter for receiving answer of request */
            if (p_i_localPort > 0)
            {
                /* check local port min. value */
                if (p_i_localPort < 1)
                {
                    throw new ArgumentException("Local send port must be at least '1', but was set to '" + p_i_localPort + "'");
                }

                /* check local port max. value */
                if (p_i_localPort > 65535)
                {
                    throw new ArgumentException("Local send port must be lower equal '65535', but was set to '" + p_i_localPort + "'");
                }

                ForestNET.Lib.Global.ILogConfig("\t" + "set socket local address instance '" + p_s_localAddress + ":" + p_i_localPort + "'");

                /* set socket local address instance */
                this.SocketLocalAddress = new(System.Net.IPAddress.Parse(p_s_localAddress), p_i_localPort);
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
            if (p_o_socketReceiveTask == null)
            {
                throw new ArgumentNullException(nameof(p_o_socketReceiveTask), "No socket task specified");
            }
            else
            {
                this.SocketTask = p_o_socketReceiveTask;
            }
        }

        /// <summary>
        /// Core execution process method of a socket. Sending data as a simple socket instance.
        /// </summary>
        public override async System.Threading.Tasks.Task Run()
        {
            this.Stop = false;
            await this.RunSocket();
        }

        /// <summary>
        /// This method stops the socket and ends any network communication.
        /// </summary>
        public override void StopSocket()
        {
            this.Stop = true;
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

                /* check for valid multicast address and valid TTL */
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

                    if (this.MulticastTTL < 0)
                    {
                        throw new ArgumentException("Multicast TTL must be at least '1', but is set to '" + this.MulticastTTL + "'");
                    }
                }

                ForestNET.Lib.Global.ILogConfig(((this.SocketTask.CommunicationType != null) ? this.SocketTask.CommunicationType + " " : "") + "socket connect to " + this.SocketAddress);

                /* endless loop for our sending socket instance */
                while (!this.Stop)
                {
                    ForestNET.Lib.Global.ILogFine("prepare socket instance for connection");

                    /* prepare socket instance for connection */
                    this.SocketInstance = new(this.SocketAddress.AddressFamily, System.Net.Sockets.SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp);

                    ForestNET.Lib.Global.ILogFine("created send socket");

                    /* set multicast TTL */
                    if (this.IsMulticastSocket)
                    {
                        if (this.SocketAddress.AddressFamily == AddressFamily.InterNetworkV6)
                        {
                            this.SocketInstance.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.MulticastTimeToLive, this.MulticastTTL);
                        }
                        else
                        {
                            this.SocketInstance.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, this.MulticastTTL);
                        }
                    }

                    /* check if we have a local address */
                    if (this.SocketLocalAddress != null)
                    {
                        ForestNET.Lib.Global.ILogFine("bind socket instance to configured local address");

                        /* bind socket instance to configured local address */
                        try
                        {
                            this.SocketInstance.Bind(this.SocketLocalAddress);
                        }
                        catch (Exception)
                        {
                            ForestNET.Lib.Global.ILogSevere("could not bind socket instance to '" + this.SocketLocalAddress + "'");
                        }
                    }

                    /* set receive and send buffer size */
                    this.SocketInstance.ReceiveBufferSize = this.BufferSize;
                    this.SocketInstance.SendBufferSize = this.BufferSize;
                    /* set socket timeout value */
                    this.SocketInstance.ReceiveTimeout = this.TimeoutMilliseconds;
                    this.SocketInstance.SendTimeout = this.TimeoutMilliseconds;
                    /* create cancellation token source for all socket tasks */
                    this.CancellationTokenSource = new();

                    try
                    {
                        ForestNET.Lib.Global.ILogFine("this.SocketInstance.ConnectAsync to " + this.SocketAddress);

                        System.Threading.Tasks.Task o_taskSocket = this.SocketInstance.ConnectAsync(this.SocketAddress);
                        System.Threading.Tasks.Task o_taskTimeout = System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(this.TimeoutMilliseconds), this.CancellationTokenSource?.Token ?? default);

                        System.Threading.Tasks.Task o_completedTask = await System.Threading.Tasks.Task.WhenAny(o_taskSocket, o_taskTimeout);

                        if (o_completedTask == o_taskSocket)
                        {
                            ForestNET.Lib.Global.ILogFine("SocketSendUDP-RunSocket method - socket connected");

                            /* pass socket instance for answering request to socket task */
                            this.SocketTask.Socket = this;
                            /* pass UDP socket to socket task */
                            this.SocketTask.UDPSocket = this.SocketInstance;
                            /* pass buffer size length to socket task */
                            this.SocketTask.BufferLength = this.BufferSize;
                            /* pass empty UDP datagram packet to socket task */
                            this.SocketTask.DatagramBytes = [];
                            /* set cancellation token for socket task */
                            this.SocketTask.CancellationToken = this.CancellationTokenSource?.Token;
                            /* run socket task */
                            await this.SocketTask.Run();

                            if (!this.Stop)
                            { /* set stop flag from socket task, if it is not already set, to end endless loop */
                                this.Stop = this.SocketTask.Stop;
                            }
                        }
                        else
                        {
                            if (!o_taskTimeout.IsCanceled && !o_taskTimeout.IsCompleted && !o_taskTimeout.IsFaulted)
                            {
                                o_taskTimeout.Dispose();
                            }

                            ForestNET.Lib.Global.ILogFine("Timeout SocketSendUDP-RunSocket method - Terminations: " + (this.Terminations + 1) + "/" + this.MaxTerminations);
                        }
                    }
                    catch (ArgumentException o_exc)
                    {
                        ForestNET.Lib.Global.LogException("ArgumentException SocketSendUDP-RunSocket method: ", o_exc);
                        ForestNET.Lib.Global.ILogSevere("\tTerminations: " + (this.Terminations + 1) + "/" + this.MaxTerminations);
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

                        /* if socket instance still is running, wait configured milliseconds for other communication side before closing communication and execute another try */
                        if (!this.Stop)
                        {
                            if (this.IntervalMilliseconds > 0)
                            {
                                await System.Threading.Tasks.Task.Delay(this.IntervalMilliseconds);
                            }
                        }

                        ForestNET.Lib.Global.ILogFiner("request cycle completed by socket instance; stopped: " + this.Stop);

                        /* set cancel signal to all socket tasks */
                        this.CancellationTokenSource?.Cancel();
                        this.CancellationTokenSource = null;

                        /* close socket */
                        try
                        {
                            this.SocketInstance?.Shutdown(System.Net.Sockets.SocketShutdown.Send);
                            this.SocketInstance?.Close();
                            this.SocketInstance = null;
                        }
                        catch (Exception)
                        {
                            /* ignore disposed exception, for the case that the socket has been closed within socket task */
                        }
                    }
                }
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.LogException("Exception SendUDP-RunSocket method: ", o_exc);
            }

            ForestNET.Lib.Global.ILogConfig("SendUDP-RunSocket method stopped");
        }
    }
}