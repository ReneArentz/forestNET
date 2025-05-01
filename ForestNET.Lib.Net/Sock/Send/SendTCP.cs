namespace ForestNET.Lib.Net.Sock.Send
{
    /// <summary>
    /// Generic socket class to send network traffic over a socket instance. Several methods supporting sending data over TCP (optional with TLS).
    /// </summary>
    public class SendTCP : ForestNET.Lib.Net.Sock.Socket
    {

        /* Fields */

        /* Properties */

        private System.Net.IPEndPoint? SocketLocalAddress { get; set; }
        private int IntervalMilliseconds { get; set; }
        public bool UsingProxy { get; set; }
        private System.Net.Sockets.Socket? ProxySocket { get; set; }
        public string? RemoteCertificateName { get; set; }
        public string? ServerNameIndicator { get; set; }

        /* Methods */

        /// <summary>
        /// Create send socket instance for TCP network connection, executing within a thread. Auto determine local address of this socket instance. Buffer size = 1500 bytes. No interval for waiting for other communication side or new data to send. Execute socket thread just once. Do not check reachability. 60 seconds timeout as default.
        /// </summary>
        /// <param name="p_s_host">address of destination for socket instance</param>
        /// <param name="p_i_port">port of destination for socket instance</param>
        /// <param name="p_o_socketReceiveTask">socket task object with the core execution process of a socket</param>
        /// <exception cref="ArgumentException">invalid parameter values for ports, buffer size or timeout</exception>
        /// <exception cref="InvalidOperationException">destination host is not reachable with that host and port parameter</exception>
        /// <exception cref="ArgumentNullException">socket task parameter is null</exception>
        /// <exception cref="FormatException">issue local host name could not be resolved into an address</exception>
        public SendTCP(string p_s_host, int p_i_port, ForestNET.Lib.Net.Sock.Task.Task p_o_socketReceiveTask) :
            this(p_s_host, p_i_port, p_o_socketReceiveTask, 60000)
        {

        }

        /// <summary>
        /// Create send socket instance for TCP network connection, executing within a thread. Auto determine local address of this socket instance. Buffer size = 1500 bytes. No interval for waiting for other communication side or new data to send. Execute socket thread just once. Do not check reachability.
        /// </summary>
        /// <param name="p_s_host">address of destination for socket instance</param>
        /// <param name="p_i_port">port of destination for socket instance</param>
        /// <param name="p_o_socketReceiveTask">socket task object with the core execution process of a socket</param>
        /// <param name="p_i_timeoutMilliseconds">set timeout value for socket instance - how long the socket should wait for incoming data so it will not hold forever</param>
        /// <exception cref="ArgumentException">invalid parameter values for ports, buffer size or timeout</exception>
        /// <exception cref="InvalidOperationException">destination host is not reachable with that host and port parameter</exception>
        /// <exception cref="ArgumentNullException">socket task parameter is null</exception>
        /// <exception cref="FormatException">issue local host name could not be resolved into an address</exception>
        public SendTCP(string p_s_host, int p_i_port, ForestNET.Lib.Net.Sock.Task.Task p_o_socketReceiveTask, int p_i_timeoutMilliseconds) :
            this(p_s_host, p_i_port, p_o_socketReceiveTask, p_i_timeoutMilliseconds, false)
        {

        }

        /// <summary>
        /// Create send socket instance for TCP network connection, executing within a thread. Auto determine local address of this socket instance. Buffer size = 1500 bytes. No interval for waiting for other communication side or new data to send. Execute socket thread just once.
        /// </summary>
        /// <param name="p_s_host">address of destination for socket instance</param>
        /// <param name="p_i_port">port of destination for socket instance</param>
        /// <param name="p_o_socketReceiveTask">socket task object with the core execution process of a socket</param>
        /// <param name="p_i_timeoutMilliseconds">set timeout value for socket instance - how long the socket should wait for incoming data so it will not hold forever</param>
        /// <param name="p_b_checkReachability">true - check if destination is reachable, false - do not check reachability</param>
        /// <exception cref="ArgumentException">invalid parameter values for ports, buffer size or timeout</exception>
        /// <exception cref="InvalidOperationException">destination host is not reachable with that host and port parameter</exception>
        /// <exception cref="ArgumentNullException">socket task parameter is null</exception>
        /// <exception cref="FormatException">issue local host name could not be resolved into an address</exception>
        public SendTCP(string p_s_host, int p_i_port, ForestNET.Lib.Net.Sock.Task.Task p_o_socketReceiveTask, int p_i_timeoutMilliseconds, bool p_b_checkReachability) :
            this(p_s_host, p_i_port, p_o_socketReceiveTask, p_i_timeoutMilliseconds, p_b_checkReachability, 1)
        {

        }

        /// <summary>
        /// Create send socket instance for TCP network connection, executing within a thread. Auto determine local address of this socket instance. Buffer size = 1500 bytes. No interval for waiting for other communication side or new data to send.
        /// </summary>
        /// <param name="p_s_host">address of destination for socket instance</param>
        /// <param name="p_i_port">port of destination for socket instance</param>
        /// <param name="p_o_socketReceiveTask">socket task object with the core execution process of a socket</param>
        /// <param name="p_i_timeoutMilliseconds">set timeout value for socket instance - how long the socket should wait for incoming data so it will not hold forever</param>
        /// <param name="p_b_checkReachability">true - check if destination is reachable, false - do not check reachability</param>
        /// <param name="p_i_maxTerminations">set a max. value for thread executions of socket instance</param>
        /// <exception cref="ArgumentException">invalid parameter values for ports, buffer size or timeout</exception>
        /// <exception cref="InvalidOperationException">destination host is not reachable with that host and port parameter</exception>
        /// <exception cref="ArgumentNullException">socket task parameter is null</exception>
        /// <exception cref="FormatException">issue local host name could not be resolved into an address</exception>
        public SendTCP(string p_s_host, int p_i_port, ForestNET.Lib.Net.Sock.Task.Task p_o_socketReceiveTask, int p_i_timeoutMilliseconds, bool p_b_checkReachability, int p_i_maxTerminations) :
            this(p_s_host, p_i_port, p_o_socketReceiveTask, p_i_timeoutMilliseconds, p_b_checkReachability, p_i_maxTerminations, 0)
        {

        }

        /// <summary>
        /// Create send socket instance for TCP network connection, executing within a thread. Auto determine local address of this socket instance. Buffer size = 1500 bytes.
        /// </summary>
        /// <param name="p_s_host">address of destination for socket instance</param>
        /// <param name="p_i_port">port of destination for socket instance</param>
        /// <param name="p_o_socketReceiveTask">socket task object with the core execution process of a socket</param>
        /// <param name="p_i_timeoutMilliseconds">set timeout value for socket instance - how long the socket should wait for incoming data so it will not hold forever</param>
        /// <param name="p_b_checkReachability">true - check if destination is reachable, false - do not check reachability</param>
        /// <param name="p_i_maxTerminations">set a max. value for thread executions of socket instance</param>
        /// <param name="p_i_intervalMilliseconds">interval for waiting for other communication side or new data to send</param>
        /// <exception cref="ArgumentException">invalid parameter values for ports, buffer size or timeout</exception>
        /// <exception cref="InvalidOperationException">destination host is not reachable with that host and port parameter</exception>
        /// <exception cref="ArgumentNullException">socket task parameter is null</exception>
        /// <exception cref="FormatException">issue local host name could not be resolved into an address</exception>
        public SendTCP(string p_s_host, int p_i_port, ForestNET.Lib.Net.Sock.Task.Task p_o_socketReceiveTask, int p_i_timeoutMilliseconds, bool p_b_checkReachability, int p_i_maxTerminations, int p_i_intervalMilliseconds) :
            this(p_s_host, p_i_port, p_o_socketReceiveTask, p_i_timeoutMilliseconds, p_b_checkReachability, p_i_maxTerminations, p_i_intervalMilliseconds, 1500)
        {

        }

        /// <summary>
        /// Create send socket instance for TCP network connection, executing within a thread. Auto determine local address of this socket instance.
        /// </summary>
        /// <param name="p_s_host">address of destination for socket instance</param>
        /// <param name="p_i_port">port of destination for socket instance</param>
        /// <param name="p_o_socketReceiveTask">socket task object with the core execution process of a socket</param>
        /// <param name="p_i_timeoutMilliseconds">set timeout value for socket instance - how long the socket should wait for incoming data so it will not hold forever</param>
        /// <param name="p_b_checkReachability">true - check if destination is reachable, false - do not check reachability</param>
        /// <param name="p_i_maxTerminations">set a max. value for thread executions of socket instance</param>
        /// <param name="p_i_intervalMilliseconds">interval for waiting for other communication side or new data to send</param>
        /// <param name="p_i_bufferSize">buffer size for socket instance, e.g. ethernet packets have max. data length of 1500 bytes</param>
        /// <exception cref="ArgumentException">invalid parameter values for ports, buffer size or timeout</exception>
        /// <exception cref="InvalidOperationException">destination host is not reachable with that host and port parameter</exception>
        /// <exception cref="ArgumentNullException">socket task parameter is null</exception>
        /// <exception cref="FormatException">issue local host name could not be resolved into an address</exception>
        public SendTCP(string p_s_host, int p_i_port, ForestNET.Lib.Net.Sock.Task.Task p_o_socketReceiveTask, int p_i_timeoutMilliseconds, bool p_b_checkReachability, int p_i_maxTerminations, int p_i_intervalMilliseconds, int p_i_bufferSize) :
            this(p_s_host, p_i_port, p_o_socketReceiveTask, p_i_timeoutMilliseconds, p_b_checkReachability, p_i_maxTerminations, p_i_intervalMilliseconds, p_i_bufferSize, System.Net.Dns.GetHostName(), 0)
        {

        }

        /// <summary>
        /// Create send socket instance for TCP network connection, executing within a thread. Auto determine local address of this socket instance.
        /// </summary>
        /// <param name="p_s_host">address of destination for socket instance</param>
        /// <param name="p_i_port">port of destination for socket instance</param>
        /// <param name="p_o_socketReceiveTask">socket task object with the core execution process of a socket</param>
        /// <param name="p_i_timeoutMilliseconds">set timeout value for socket instance - how long the socket should wait for incoming data so it will not hold forever</param>
        /// <param name="p_b_checkReachability">true - check if destination is reachable, false - do not check reachability</param>
        /// <param name="p_i_maxTerminations">set a max. value for thread executions of socket instance</param>
        /// <param name="p_i_intervalMilliseconds">interval for waiting for other communication side or new data to send</param>
        /// <param name="p_i_bufferSize">buffer size for socket instance, e.g. ethernet packets have max. data length of 1500 bytes</param>
        /// <param name="p_i_localPort">local port of this socket instance</param>
        /// <exception cref="ArgumentException">invalid parameter values for ports, buffer size or timeout</exception>
        /// <exception cref="InvalidOperationException">destination host is not reachable with that host and port parameter</exception>
        /// <exception cref="ArgumentNullException">socket task parameter is null</exception>
        /// <exception cref="FormatException">issue local host name could not be resolved into an address</exception>
        public SendTCP(string p_s_host, int p_i_port, ForestNET.Lib.Net.Sock.Task.Task p_o_socketReceiveTask, int p_i_timeoutMilliseconds, bool p_b_checkReachability, int p_i_maxTerminations, int p_i_intervalMilliseconds, int p_i_bufferSize, int p_i_localPort) :
            this(p_s_host, p_i_port, p_o_socketReceiveTask, p_i_timeoutMilliseconds, p_b_checkReachability, p_i_maxTerminations, p_i_intervalMilliseconds, p_i_bufferSize, System.Net.Dns.GetHostName(), p_i_localPort)
        {

        }

        /// <summary>
        /// Create send socket instance for TCP network connection, executing within a thread.
        /// </summary>
        /// <param name="p_s_host">address of destination for socket instance</param>
        /// <param name="p_i_port">port of destination for socket instance</param>
        /// <param name="p_o_socketReceiveTask">socket task object with the core execution process of a socket</param>
        /// <param name="p_i_timeoutMilliseconds">set timeout value for socket instance - how long the socket should wait for incoming data so it will not hold forever</param>
        /// <param name="p_b_checkReachability">true - check if destination is reachable, false - do not check reachability</param>
        /// <param name="p_i_maxTerminations">set a max. value for thread executions of socket instance</param>
        /// <param name="p_i_intervalMilliseconds">interval for waiting for other communication side or new data to send</param>
        /// <param name="p_i_bufferSize">buffer size for socket instance, e.g. ethernet packets have max. data length of 1500 bytes</param>
        /// <param name="p_s_localAddress">local address of this socket instance</param>
        /// <param name="p_i_localPort">local port of this socket instance</param>
        /// <exception cref="ArgumentException">invalid parameter values for ports, buffer size or timeout</exception>
        /// <exception cref="InvalidOperationException">destination host is not reachable with that host and port parameter</exception>
        /// <exception cref="ArgumentNullException">socket task parameter is null</exception>
        /// <exception cref="FormatException">issue local host name could not be resolved into an address</exception>
        public SendTCP(string p_s_host, int p_i_port, ForestNET.Lib.Net.Sock.Task.Task p_o_socketReceiveTask, int p_i_timeoutMilliseconds, bool p_b_checkReachability, int p_i_maxTerminations, int p_i_intervalMilliseconds, int p_i_bufferSize, string p_s_localAddress, int p_i_localPort) :
            this(p_s_host, p_i_port, p_o_socketReceiveTask, p_i_timeoutMilliseconds, p_b_checkReachability, p_i_maxTerminations, p_i_intervalMilliseconds, p_i_bufferSize, p_s_localAddress, p_i_localPort, null)
        {

        }

        /// <summary>
        /// Create send socket instance for TCP network connection, executing within a thread.
        /// </summary>
        /// <param name="p_s_host">address of destination for socket instance</param>
        /// <param name="p_i_port">port of destination for socket instance</param>
        /// <param name="p_o_socketReceiveTask">socket task object with the core execution process of a socket</param>
        /// <param name="p_i_timeoutMilliseconds">set timeout value for socket instance - how long the socket should wait for incoming data so it will not hold forever</param>
        /// <param name="p_b_checkReachability">true - check if destination is reachable, false - do not check reachability</param>
        /// <param name="p_i_maxTerminations">set a max. value for thread executions of socket instance</param>
        /// <param name="p_i_intervalMilliseconds">interval for waiting for other communication side or new data to send</param>
        /// <param name="p_i_bufferSize">buffer size for socket instance, e.g. ethernet packets have max. data length of 1500 bytes</param>
        /// <param name="p_s_localAddress">local address of this socket instance</param>
        /// <param name="p_i_localPort">local port of this socket instance</param>
        /// <param name="p_a_clientCertificateAllowList">own client certificate allow list for socket instance</param>
        /// <exception cref="ArgumentException">invalid parameter values for ports, buffer size or timeout</exception>
        /// <exception cref="InvalidOperationException">destination host is not reachable with that host and port parameter</exception>
        /// <exception cref="ArgumentNullException">socket task parameter is null</exception>
        /// <exception cref="FormatException">issue local host name could not be resolved into an address</exception>
        public SendTCP(string p_s_host, int p_i_port, ForestNET.Lib.Net.Sock.Task.Task p_o_socketReceiveTask, int p_i_timeoutMilliseconds, bool p_b_checkReachability, int p_i_maxTerminations, int p_i_intervalMilliseconds, int p_i_bufferSize, string p_s_localAddress, int p_i_localPort, List<System.Security.Cryptography.X509Certificates.X509Certificate>? p_a_clientCertificateAllowList)
        {
            /* set variables with parameter and default values */
            this.Stop = false;
            this.UsingProxy = false;
            this.Terminations = 0;
            this.ClientCertificateAllowList = p_a_clientCertificateAllowList;
            this.RemoteCertificateName = null;
            this.ProxySocket = null;

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

            /* check buffer size min. value */
            if (p_i_bufferSize < 4)
            {
                throw new ArgumentException("Buffer size must be at least '4', but was set to '" + p_i_bufferSize + "'");
            }
            else
            {
                this.BufferSize = p_i_bufferSize;
            }

            ForestNET.Lib.Global.ILogConfig("\t" + "set buffer size '" + p_i_bufferSize + "'");

            /* set socket timeout value if it is a positive integer value */
            if (p_i_timeoutMilliseconds > 0)
            {
                ForestNET.Lib.Global.ILogConfig("\t" + "set socket timeout value '" + p_i_timeoutMilliseconds + "'");

                this.TimeoutMilliseconds = p_i_timeoutMilliseconds;
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

            /* check local port parameter for receiving answer of request */
            if (p_i_localPort > 0)
            {
                /* check local port min. value */
                if (p_i_localPort < 1)
                {
                    throw new ArgumentException("Local send port must be at least '1', but was set to '" + p_i_port + "'");
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

            /* check destination reachability */
            if ((p_b_checkReachability) && (!this.IsReachable().GetAwaiter().GetResult()))
            {
                throw new InvalidOperationException("Host[" + this.SocketAddress + "] is not reachable");
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
        /// Check destination reachability with host and port settings.
        /// </summary>
        /// <returns>true - destination is reachable, false - destination is not reachable, maybe wrong host or port parameter</returns>
        private async System.Threading.Tasks.Task<bool> IsReachable()
        {
            try
            {
                /* check if socket address is set */
                if (this.SocketAddress == null)
                {
                    throw new ArgumentNullException("There was no socket address specified");
                }

                ForestNET.Lib.Global.ILogConfig("create socket to check reachability");

                /* socket object */
                System.Net.Sockets.Socket o_tempSocket = new(this.SocketAddress.AddressFamily, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);

                ForestNET.Lib.Global.ILogConfig("connect to destination[" + this.SocketAddress + "] to check reachability");

                System.Threading.Tasks.Task o_taskSocket = o_tempSocket.ConnectAsync(this.SocketAddress);
                System.Threading.Tasks.Task o_taskTimeout = System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(this.TimeoutMilliseconds));

                System.Threading.Tasks.Task o_completedTask = await System.Threading.Tasks.Task.WhenAny(o_taskSocket, o_taskTimeout);

                if (o_completedTask != o_taskSocket)
                {
                    if (!o_taskTimeout.IsCanceled && !o_taskTimeout.IsCompleted && !o_taskTimeout.IsFaulted)
                    {
                        o_taskTimeout.Dispose();
                    }

                    /* timeout occured so destination is NOT reachable */
                    throw new System.Net.Sockets.SocketException(10060);
                }

                /* close socket */
                o_tempSocket.Shutdown(System.Net.Sockets.SocketShutdown.Send);
                o_tempSocket.Close();

                ForestNET.Lib.Global.ILogConfig("no exception occurred so destination is reachable");

                /* no exception occurred so destination is reachable */
                return true;
            }
            catch (System.Net.Sockets.SocketException o_exc)
            {
                ForestNET.Lib.Global.ILogConfig("exception occurred during connect so destination is not reachable: " + o_exc);

                return false;
            }
            catch (System.IO.IOException o_exc)
            {
                ForestNET.Lib.Global.ILogConfig("io exception occurred so destination is not reachable: " + o_exc);

                return false;
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.ILogConfig("general exception occurred so destination is not reachable: " + o_exc);

                return false;
            }
        }

        /// <summary>
        /// Override destination host and port settings, so we can send requests to different destinations with one sending socket instance
        /// </summary>
        /// <param name="p_s_host">address of destination for socket instance</param>
        /// <param name="p_i_port">port of destination for socket instance</param>
        /// <param name="p_b_checkReachability">true - using proxy as destination, false - not using proxy</param>
        /// <param name="p_b_checkReachability">true - check if destination is reachable, false - do not check reachability</param>
        /// <exception cref="ArgumentException">invalid parameter values for ports, buffer size or timeout</exception>
        /// <exception cref="InvalidOperationException">destination host is not reachable with that host and port parameter</exception>
        /// <exception cref="FormatException">issue local host name could not be resolved into an address</exception>
        public void OverrideDestinationAddress(string p_s_host, int p_i_port, bool p_b_usingProxy, bool p_b_checkReachability)
        {
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

            /* get host value if host parameter is 'localhost' */
            if (p_s_host.ToLower().Equals("localhost"))
            {
                p_s_host = System.Net.Dns.GetHostName();

                ForestNET.Lib.Global.ILogConfig("\t" + "set host to local host value '" + p_s_host + "'");
            }

            try
            {
                ForestNET.Lib.Global.ILogConfig("\t" + "override socket address value with host and port parameter '" + p_s_host + ":" + p_i_port + "'");

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

            /* check destination reachability */
            if ((p_b_checkReachability) && (!this.IsReachable().GetAwaiter().GetResult()))
            {
                throw new InvalidOperationException("Host[" + this.SocketAddress + "] is not reachable");
            }

            this.UsingProxy = p_b_usingProxy;
        }

        /// <summary>
        /// Upgrade current socket(proxy socket) to a TLS socket connection
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.IO.IOException">issue creating TCP or TCP with TLS socket instance</exception>
        public void UpgradeCurrentSocketToTLS(string p_s_host, int p_i_port, bool p_b_checkReachability)
        {
            /* check if socket task is set */
            if (this.SocketTask == null)
            {
                throw new NullReferenceException("There was no socket task specified");
            }

            /* check if socket task socket is set */
            if (this.SocketTask.Socket == null)
            {
                throw new NullReferenceException("There was no socket within socket task specified");
            }

            if (this.SocketTask?.Stream is not System.Net.Sockets.NetworkStream)
            {
                throw new InvalidOperationException("Can only upgrade TCP-Client socket without TLS to TLS socket");
            }

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

            /* get host value if host parameter is 'localhost' */
            if (p_s_host.ToLower().Equals("localhost"))
            {
                p_s_host = System.Net.Dns.GetHostName();

                ForestNET.Lib.Global.ILogConfig("\t" + "set host to local host value '" + p_s_host + "'");
            }

            try
            {
                ForestNET.Lib.Global.ILogConfig("\t" + "override socket address value with host and port parameter '" + p_s_host + ":" + p_i_port + "'");

                try
                {
                    /* resolve proxy host ip */
                    this.SocketAddress = new(System.Net.IPAddress.Parse(System.Net.Dns.GetHostAddresses(new Uri("http://" + p_s_host).Host)[0].ToString()), p_i_port);
                }
                catch (Exception)
                {
                    /* just check for valid ip address */
                    this.SocketAddress = new(System.Net.IPAddress.Parse(p_s_host), p_i_port);
                }
            }
            catch (FormatException o_exc)
            {
                throw new ArgumentException("Unknown host[" + p_s_host + "] - " + o_exc);
            }
            catch (ArgumentOutOfRangeException o_exc)
            {
                throw new ArgumentException("Unknown host[" + p_s_host + "] - " + o_exc);
            }

            /* check destination reachability */
            if ((p_b_checkReachability) && (!this.IsReachable().GetAwaiter().GetResult()))
            {
                throw new InvalidOperationException("Host[" + this.SocketAddress + "] is not reachable");
            }

            /* if we have destination port 443, we want upgrade to tls */
            if (p_i_port == 443)
            {
                /* check if we have a remote certificate name stored in our machine, so we want to use TLS */
                if (this.RemoteCertificateName != null)
                {
                    this.SocketTask.Stream = new System.Net.Security.SslStream(
                        this.SocketTask.Stream,
                        false,
                        new System.Net.Security.RemoteCertificateValidationCallback(this.MyTlsCertificateValidationCallback),
                        null
                    );

                    /* temp variable as SslStream */
                    System.Net.Security.SslStream o_sslStream = (System.Net.Security.SslStream)this.SocketTask.Stream;

                    /* get target host from socket address or remote certificate name */
                    string s_targetHost = this.RemoteCertificateName ?? this.SocketAddress.Address.ToString();

                    /* use SNI as target host if 'INSERT_HOST_FOR_TLS' is set and server name indicator has a value */
                    if ((s_targetHost.Equals("INSERT_HOST_FOR_TLS")) && (this.ServerNameIndicator != null) && (!ForestNET.Lib.Helper.IsStringEmpty(this.ServerNameIndicator)))
                    {
                        s_targetHost = this.ServerNameIndicator;
                    }

                    /* authenticate SslStream as client */
                    o_sslStream.AuthenticateAsClient(s_targetHost);

                    ForestNET.Lib.Global.ILogConfig("using TLS");

                    /* log properties and settings for the tls stream */
                    if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("log properties and settings for the tls stream");
                    if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("\t" + "CipherAlgorithm: '" + o_sslStream.CipherAlgorithm + "' CipherStrength: '" + o_sslStream.CipherStrength + "'");
                    if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("\t" + "HashAlgorithm: '" + o_sslStream.HashAlgorithm + "' HashStrength: '" + o_sslStream.HashStrength + "'");
                    if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("\t" + "KeyExchangeAlgorithm: '" + o_sslStream.KeyExchangeAlgorithm + "' KeyExchangeStrength: '" + o_sslStream.KeyExchangeStrength + "'");
                    if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("\t" + "SslProtocol: '" + o_sslStream.SslProtocol + "'");

                    if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("\t" + "IsAuthenticated: '" + o_sslStream.IsAuthenticated + "' IsServer: '" + o_sslStream.IsServer + "'");
                    if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("\t" + "IsSigned: '" + o_sslStream.IsSigned + "'");
                    if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("\t" + "IsEncrypted: '" + o_sslStream.IsEncrypted + "'");

                    if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("\t" + "CanRead: '" + o_sslStream.CanRead + "' CanWrite: '" + o_sslStream.CanWrite + "'");
                    if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("\t" + "CanTimeout: '" + o_sslStream.CanTimeout + "'");

                    if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("\t" + "CheckCertRevocationStatus: '" + o_sslStream.CheckCertRevocationStatus + "'");

                    if (o_sslStream.LocalCertificate != null)
                    {
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("\t" + "LocalCertificte - Subject: '" + o_sslStream.LocalCertificate?.Subject + "' EffectiveDateString: '" + o_sslStream.LocalCertificate?.GetEffectiveDateString() + "' ExpirationDateString: '" + o_sslStream.LocalCertificate?.GetExpirationDateString() + "'");
                    }
                    else
                    {
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("\t" + "LocalCertificte is null.");
                    }

                    if (o_sslStream.RemoteCertificate != null)
                    {
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("\t" + "RemoteCertificate - Subject: '" + o_sslStream.RemoteCertificate?.Subject + "' EffectiveDateString: '" + o_sslStream.RemoteCertificate?.GetEffectiveDateString() + "' ExpirationDateString: '" + o_sslStream.RemoteCertificate?.GetExpirationDateString() + "'");
                    }
                    else
                    {
                        if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("\t" + "RemoteCertificate is null.");
                    }
                }
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

                ForestNET.Lib.Global.ILogConfig(((this.SocketTask.CommunicationType != null) ? this.SocketTask.CommunicationType + " " : "") + "socket connect to " + this.SocketAddress);

                /* remind current time in milliseconds */
                long l_foo = Environment.TickCount64;

                /* counter for max. exception terminations */
                int i_exceptionTerminations = 1;

                /* endless loop for our sending socket instance */
                while (!this.Stop)
                {
                    /* sending over TCP default or TCP with direct answer communication type, but not with Equal-Bidirectional */
                    if (((this.SocketTask.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.TCP_SEND) || (this.SocketTask.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.TCP_SEND_WITH_ANSWER)) && (this.SocketTask.CommunicationCardinality != ForestNET.Lib.Net.Sock.Com.Cardinality.EqualBidirectional))
                    {
                        /* if we have no data available to be sent wait interval value for new data */
                        if (!this.SocketTask.MessagesAvailable())
                        {
                            if (this.IntervalMilliseconds > 0)
                            {
                                /* wait interval value */
                                await System.Threading.Tasks.Task.Delay(this.IntervalMilliseconds);
                            }

                            /* continue to next loop iteration */
                            continue;
                        }
                    }

                    ForestNET.Lib.Global.ILogFine("prepare socket instance for connection");

                    /* create send socket */
                    this.SocketInstance = new(this.SocketAddress.AddressFamily, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);

                    ForestNET.Lib.Global.ILogFine("created send socket");

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
                            ForestNET.Lib.Global.ILogFine("SocketSendTCP-RunSocket method - socket connected");

                            /* wrap send socket into network stream */
                            this.SocketTask.Stream = new System.Net.Sockets.NetworkStream(this.SocketInstance);

                            /* check if we have certificates in our allow list or remote certificate name stored in our machine, so we want to use TLS */
                            if ((this.ClientCertificateAllowList != null) || (this.RemoteCertificateName != null))
                            {
                                this.SocketTask.Stream = new System.Net.Security.SslStream(
                                    this.SocketTask.Stream,
                                    false,
                                    new System.Net.Security.RemoteCertificateValidationCallback(this.MyTlsCertificateValidationCallback),
                                    null
                                );

                                /* temp variable as SslStream */
                                System.Net.Security.SslStream o_sslStream = (System.Net.Security.SslStream)this.SocketTask.Stream;

                                /* get target host from socket address or remote certificate name */
                                string s_targetHost = this.RemoteCertificateName ?? this.SocketAddress.Address.ToString();

                                /* use SNI as target host if 'INSERT_HOST_FOR_TLS' is set and server name indicator has a value */
                                if ((s_targetHost.Equals("INSERT_HOST_FOR_TLS")) && (this.ServerNameIndicator != null) && (!ForestNET.Lib.Helper.IsStringEmpty(this.ServerNameIndicator)))
                                {
                                    s_targetHost = this.ServerNameIndicator;
                                }

                                /* authenticate SslStream as client */
                                o_sslStream.AuthenticateAsClient(s_targetHost);

                                ForestNET.Lib.Global.ILogConfig("using TLS");

                                /* log properties and settings for the tls stream */
                                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("log properties and settings for the tls stream");
                                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("\t" + "CipherAlgorithm: '" + o_sslStream.CipherAlgorithm + "' CipherStrength: '" + o_sslStream.CipherStrength + "'");
                                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("\t" + "HashAlgorithm: '" + o_sslStream.HashAlgorithm + "' HashStrength: '" + o_sslStream.HashStrength + "'");
                                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("\t" + "KeyExchangeAlgorithm: '" + o_sslStream.KeyExchangeAlgorithm + "' KeyExchangeStrength: '" + o_sslStream.KeyExchangeStrength + "'");
                                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("\t" + "SslProtocol: '" + o_sslStream.SslProtocol + "'");

                                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("\t" + "IsAuthenticated: '" + o_sslStream.IsAuthenticated + "' IsServer: '" + o_sslStream.IsServer + "'");
                                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("\t" + "IsSigned: '" + o_sslStream.IsSigned + "'");
                                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("\t" + "IsEncrypted: '" + o_sslStream.IsEncrypted + "'");

                                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("\t" + "CanRead: '" + o_sslStream.CanRead + "' CanWrite: '" + o_sslStream.CanWrite + "'");
                                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("\t" + "CanTimeout: '" + o_sslStream.CanTimeout + "'");

                                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("\t" + "CheckCertRevocationStatus: '" + o_sslStream.CheckCertRevocationStatus + "'");

                                if (o_sslStream.LocalCertificate != null)
                                {
                                    if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("\t" + "LocalCertificte - Subject: '" + o_sslStream.LocalCertificate?.Subject + "' EffectiveDateString: '" + o_sslStream.LocalCertificate?.GetEffectiveDateString() + "' ExpirationDateString: '" + o_sslStream.LocalCertificate?.GetExpirationDateString() + "'");
                                }
                                else
                                {
                                    if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("\t" + "LocalCertificte is null.");
                                }

                                if (o_sslStream.RemoteCertificate != null)
                                {
                                    if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("\t" + "RemoteCertificate - Subject: '" + o_sslStream.RemoteCertificate?.Subject + "' EffectiveDateString: '" + o_sslStream.RemoteCertificate?.GetEffectiveDateString() + "' ExpirationDateString: '" + o_sslStream.RemoteCertificate?.GetExpirationDateString() + "'");
                                }
                                else
                                {
                                    if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("\t" + "RemoteCertificate is null.");
                                }
                            }

                            /* set timeout values from socket instance to stream instance */
                            this.SocketTask.Stream.ReadTimeout = this.SocketInstance.ReceiveTimeout;
                            this.SocketTask.Stream.WriteTimeout = this.SocketInstance.SendTimeout;
                            /* pass socket instance for answering request to socket task */
                            this.SocketTask.Socket = this;
                            /* pass buffer size length to socket task */
                            this.SocketTask.BufferLength = this.BufferSize;
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

                            ForestNET.Lib.Global.ILogFine("Timeout SocketSendTCP-RunSocket method - Terminations: " + (this.Terminations + 1) + "/" + this.MaxTerminations);
                        }
                    }
                    catch (Exception o_exc)
                    {
                        if ((o_exc is System.IO.IOException) || (o_exc is System.Net.Sockets.SocketException))
                        {
                            /* ForestNET.Lib.Global.LogException("IOException or SocketException SendTCP-RunSocket method: ", o_exc); */
                            ForestNET.Lib.Global.ILogWarning("IOException or SocketException SendTCP-runSocket method: " + o_exc.Message + " - Terminations: " + (this.Terminations + 1) + "/" + this.MaxTerminations + " - Expected Terminations: " + i_exceptionTerminations + "/10");

                            /* check if current time in milliseconds minus reminder is greater than the configured socket timeout, in that case we stop the socket instance */
                            if ((Environment.TickCount64 - l_foo) > this.TimeoutMilliseconds)
                            {
                                ForestNET.Lib.Global.ILogFiner("current time in milliseconds minus reminder is greater than the configured socket timeout, in that case we stop the socket instance");

                                this.Stop = true;
                            }

                            /* ignore max. terminations if this socket instance should just try once, to prevent stopping the socket instance - maybe the server or a proxy are very slow */
                            if ((this.MaxTerminations == 1) && ((this.Terminations + 1) >= this.MaxTerminations))
                            {
                                ForestNET.Lib.Global.ILogFiner("ignore max. terminations if this socket instance should just try once, to prevent stopping the socket instance");

                                this.Terminations = -1;
                                this.Stop = false;
                                i_exceptionTerminations++;
                            }

                            /* if our exceptionally carried out attempts nevertheless fail more than 10 times, we draw the line here */
                            if (i_exceptionTerminations > 10)
                            {
                                ForestNET.Lib.Global.ILogWarning("our exceptionally carried out attempts for socket instance which should just try one, nevertheless fail more than 10 times - stopping instance now");
                                this.Stop = true;
                            }
                        }
                        else
                        {
                            ForestNET.Lib.Global.LogException("Exception SendTCP-RunSocket method: ", o_exc);
                            ForestNET.Lib.Global.ILogSevere("\tTerminations: " + (this.Terminations + 1) + "/" + this.MaxTerminations);

                            throw;
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

                        /* if socket instance still is running, wait configured milliseconds for other communication side before closing communication and execute another try */
                        if (!this.Stop)
                        {
                            if ((this.IntervalMilliseconds > 0) && (this.SocketInstance.Connected))
                            {
                                await System.Threading.Tasks.Task.Delay(this.IntervalMilliseconds);
                            }
                        }

                        /* close communication of proxy socket instance */
                        try
                        {
                            if (this.ProxySocket != null)
                            {
                                ForestNET.Lib.Global.ILogFiner("close communication of proxy socket instance, connected: " + this.ProxySocket.Connected);

                                this.ProxySocket.Shutdown(System.Net.Sockets.SocketShutdown.Both);
                                this.ProxySocket.Close();
                                this.ProxySocket = null;
                            }
                        }
                        catch (Exception)
                        {
                            /* ignore disposed exception, for the case that the proxy socket has been closed within socket task */
                        }

                        /* close token source */
                        try
                        {
                            ForestNET.Lib.Global.ILogFiner("close token source");

                            /* set cancel signal to all socket tasks */
                            this.CancellationTokenSource?.Cancel();
                            this.CancellationTokenSource = null;
                        }
                        catch (Exception)
                        {
                            /* ignore exception */
                        }

                        /* close socket task stream */
                        try
                        {
                            ForestNET.Lib.Global.ILogFiner("close socket task stream");

                            /* by assigning MemoryStream we execute property set method to null network- or ssl-stream */
                            this.SocketTask.Stream = new MemoryStream();
                        }
                        catch (Exception)
                        {
                            /* ignore disposed exception, for the case that the stream has been closed within socket task */
                        }

                        /* close communication and socket instance */
                        try
                        {
                            ForestNET.Lib.Global.ILogFiner("close communication and socket instance, connected: " + this.SocketInstance.Connected);

                            /* close socket */
                            this.SocketInstance?.Shutdown(System.Net.Sockets.SocketShutdown.Both);
                            this.SocketInstance?.Close();
                            this.SocketInstance = null;
                        }
                        catch (Exception)
                        {
                            /* ignore disposed exception, for the case that the socket has been closed within socket task */
                        }

                        ForestNET.Lib.Global.ILogFiner("request cycle completed by socket instance; stopped: " + this.Stop);
                    }
                }
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.LogException("Exception SendTCP-RunSocket method: ", o_exc);
            }

            ForestNET.Lib.Global.ILogConfig("SendTCP-RunSocket method stopped");
        }
    }
}