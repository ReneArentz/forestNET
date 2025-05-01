namespace ForestNET.Lib.Net.Sock.Recv
{
    /// <summary>
    /// Generic socket class to receive network traffic over a socket instance. Several methods supporting receiving data over TCP (optional with TLS) as server or socket instance.
    /// </summary>
    public class ReceiveTCP : ForestNET.Lib.Net.Sock.Socket
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

        /* Methods */

        /// <summary>
        /// Create receive socket instance for TCP network connection, executing within a thread. Receive buffer size = 1500 bytes. Infinite thread executions of socket instance. 60 seconds timeout as default.
        /// </summary>
        /// <param name="p_e_type">receiving type of socket instance - SERVER or CLIENT</param>
        /// <param name="p_i_port">port where receiving socket instance shall listen</param>
        /// <param name="p_o_socketTask">socket task object with the core execution process of a socket</param>
        /// <exception cref="ArgumentException">invalid parameter values for port, receive buffer size or timeout</exception>
        /// <exception cref="ArgumentNullException">socket task parameter is null</exception>
        /// <exception cref="System.IO.IOException">issue creating TCP or TCP with TLS socket instance</exception>
        public ReceiveTCP(ReceiveType p_e_type, int p_i_port, ForestNET.Lib.Net.Sock.Task.Task p_o_socketTask) :
            this(p_e_type, System.Net.Dns.GetHostName(), p_i_port, p_o_socketTask, 60000)
        {

        }

        /// <summary>
        /// Create receive socket instance for TCP network connection, executing within a thread. Receive buffer size = 1500 bytes. Infinite thread executions of socket instance. 60 seconds timeout as default.
        /// </summary>
        /// <param name="p_e_type">receiving type of socket instance - SERVER or CLIENT</param>
        /// <param name="p_s_host">address of receiving socket instance</param>
        /// <param name="p_i_port">port where receiving socket instance shall listen</param>
        /// <param name="p_o_socketTask">socket task object with the core execution process of a socket</param>
        /// <exception cref="ArgumentException">invalid parameter values for port, receive buffer size or timeout</exception>
        /// <exception cref="ArgumentNullException">socket task parameter is null</exception>
        /// <exception cref="System.IO.IOException">issue creating TCP or TCP with TLS socket instance</exception>
        public ReceiveTCP(ReceiveType p_e_type, string p_s_host, int p_i_port, ForestNET.Lib.Net.Sock.Task.Task p_o_socketTask) :
            this(p_e_type, p_s_host, p_i_port, p_o_socketTask, 60000)
        {

        }

        /// <summary>
        /// Create receive socket instance for TCP network connection, executing within a thread. Receive buffer size = 1500 bytes. Infinite thread executions of socket instance.
        /// </summary>
        /// <param name="p_e_type">receiving type of socket instance - SERVER or CLIENT</param>
        /// <param name="p_i_port">port where receiving socket instance shall listen</param>
        /// <param name="p_o_socketTask">socket task object with the core execution process of a socket</param>
        /// <param name="p_i_timeoutMilliseconds">set timeout value for socket instance - how long the socket should wait for incoming data so it will not hold forever</param>
        /// <exception cref="ArgumentException">invalid parameter values for port, receive buffer size or timeout</exception>
        /// <exception cref="ArgumentNullException">socket task parameter is null</exception>
        /// <exception cref="System.IO.IOException">issue creating TCP or TCP with TLS socket instance</exception>
        public ReceiveTCP(ReceiveType p_e_type, int p_i_port, ForestNET.Lib.Net.Sock.Task.Task p_o_socketTask, int p_i_timeoutMilliseconds) :
            this(p_e_type, System.Net.Dns.GetHostName(), p_i_port, p_o_socketTask, p_i_timeoutMilliseconds, -1)
        {

        }

        /// <summary>
        /// Create receive socket instance for TCP network connection, executing within a thread. Receive buffer size = 1500 bytes. Infinite thread executions of socket instance.
        /// </summary>
        /// <param name="p_e_type">receiving type of socket instance - SERVER or CLIENT</param>
        /// <param name="p_s_host">address of receiving socket instance</param>
        /// <param name="p_i_port">port where receiving socket instance shall listen</param>
        /// <param name="p_o_socketTask">socket task object with the core execution process of a socket</param>
        /// <param name="p_i_timeoutMilliseconds">set timeout value for socket instance - how long the socket should wait for incoming data so it will not hold forever</param>
        /// <exception cref="ArgumentException">invalid parameter values for port, receive buffer size or timeout</exception>
        /// <exception cref="ArgumentNullException">socket task parameter is null</exception>
        /// <exception cref="System.IO.IOException">issue creating TCP or TCP with TLS socket instance</exception>
        public ReceiveTCP(ReceiveType p_e_type, string p_s_host, int p_i_port, ForestNET.Lib.Net.Sock.Task.Task p_o_socketTask, int p_i_timeoutMilliseconds) :
            this(p_e_type, p_s_host, p_i_port, p_o_socketTask, p_i_timeoutMilliseconds, -1)
        {

        }

        /// <summary>
        /// Create receive socket instance for TCP network connection, executing within a thread. Receive buffer size = 1500 bytes.
        /// </summary>
        /// <param name="p_e_type">receiving type of socket instance - SERVER or CLIENT</param>
        /// <param name="p_i_port">port where receiving socket instance shall listen</param>
        /// <param name="p_o_socketTask">socket task object with the core execution process of a socket</param>
        /// <param name="p_i_timeoutMilliseconds">set timeout value for socket instance - how long the socket should wait for incoming data so it will not hold forever</param>
        /// <param name="p_i_maxTerminations">set a max. value for thread executions of socket instance</param>
        /// <exception cref="ArgumentException">invalid parameter values for port, receive buffer size or timeout</exception>
        /// <exception cref="ArgumentNullException">socket task parameter is null</exception>
        /// <exception cref="System.IO.IOException">issue creating TCP or TCP with TLS socket instance</exception>
        public ReceiveTCP(ReceiveType p_e_type, int p_i_port, ForestNET.Lib.Net.Sock.Task.Task p_o_socketTask, int p_i_timeoutMilliseconds, int p_i_maxTerminations) :
            this(p_e_type, System.Net.Dns.GetHostName(), p_i_port, p_o_socketTask, p_i_timeoutMilliseconds, p_i_maxTerminations, 1500, null)
        {

        }

        /// <summary>
        /// Create receive socket instance for TCP network connection, executing within a thread. Receive buffer size = 1500 bytes.
        /// </summary>
        /// <param name="p_e_type">receiving type of socket instance - SERVER or CLIENT</param>
        /// <param name="p_s_host">address of receiving socket instance</param>
        /// <param name="p_i_port">port where receiving socket instance shall listen</param>
        /// <param name="p_o_socketTask">socket task object with the core execution process of a socket</param>
        /// <param name="p_i_timeoutMilliseconds">set timeout value for socket instance - how long the socket should wait for incoming data so it will not hold forever</param>
        /// <param name="p_i_maxTerminations">set a max. value for thread executions of socket instance</param>
        /// <exception cref="ArgumentException">invalid parameter values for port, receive buffer size or timeout</exception>
        /// <exception cref="ArgumentNullException">socket task parameter is null</exception>
        /// <exception cref="System.IO.IOException">issue creating TCP or TCP with TLS socket instance</exception>
        public ReceiveTCP(ReceiveType p_e_type, string p_s_host, int p_i_port, ForestNET.Lib.Net.Sock.Task.Task p_o_socketTask, int p_i_timeoutMilliseconds, int p_i_maxTerminations) :
            this(p_e_type, p_s_host, p_i_port, p_o_socketTask, p_i_timeoutMilliseconds, p_i_maxTerminations, 1500, null)
        {

        }

        /// <summary>
        /// Create receive socket instance for TCP network connection, executing within a thread.
        /// </summary>
        /// <param name="p_e_type">receiving type of socket instance - SERVER or CLIENT</param>
        /// <param name="p_i_port">port where receiving socket instance shall listen</param>
        /// <param name="p_o_socketTask">socket task object with the core execution process of a socket</param>
        /// <param name="p_i_timeoutMilliseconds">set timeout value for socket instance - how long the socket should wait for incoming data so it will not hold forever</param>
        /// <param name="p_i_maxTerminations">set a max. value for thread executions of socket instance</param>
        /// <param name="p_i_receiveBufferSize">receive buffer size for socket instance, e.g. ethernet packets have max. data length of 1500 bytes</param>
        /// <exception cref="ArgumentException">invalid parameter values for port, receive buffer size or timeout</exception>
        /// <exception cref="ArgumentNullException">socket task parameter is null</exception>
        /// <exception cref="System.IO.IOException">issue creating TCP or TCP with TLS socket instance</exception>
        public ReceiveTCP(ReceiveType p_e_type, int p_i_port, ForestNET.Lib.Net.Sock.Task.Task p_o_socketTask, int p_i_timeoutMilliseconds, int p_i_maxTerminations, int p_i_receiveBufferSize) :
            this(p_e_type, System.Net.Dns.GetHostName(), p_i_port, p_o_socketTask, p_i_timeoutMilliseconds, p_i_maxTerminations, p_i_receiveBufferSize, null)
        {

        }

        /// <summary>
        /// Create receive socket instance for TCP network connection, executing within a thread.
        /// </summary>
        /// <param name="p_e_type">receiving type of socket instance - SERVER or CLIENT</param>
        /// <param name="p_i_port">port where receiving socket instance shall listen</param>
        /// <param name="p_o_socketTask">socket task object with the core execution process of a socket</param>
        /// <param name="p_i_timeoutMilliseconds">set timeout value for socket instance - how long the socket should wait for incoming data so it will not hold forever</param>
        /// <param name="p_i_maxTerminations">set a max. value for thread executions of socket instance</param>
        /// <param name="p_i_receiveBufferSize">receive buffer size for socket instance, e.g. ethernet packets have max. data length of 1500 bytes</param>
        /// <param name="p_s_pathToServerCertificate">path to server certificate file(.pfx) for TLS receive socket instance</param>
        /// <exception cref="ArgumentException">invalid parameter values for port, receive buffer size or timeout</exception>
        /// <exception cref="ArgumentNullException">socket task parameter is null</exception>
        /// <exception cref="System.IO.IOException">issue creating TCP or TCP with TLS socket instance</exception>
        public ReceiveTCP(ReceiveType p_e_type, int p_i_port, ForestNET.Lib.Net.Sock.Task.Task p_o_socketTask, int p_i_timeoutMilliseconds, int p_i_maxTerminations, int p_i_receiveBufferSize, string? p_s_pathToServerCertificate) :
            this(p_e_type, System.Net.Dns.GetHostName(), p_i_port, p_o_socketTask, p_i_timeoutMilliseconds, p_i_maxTerminations, p_i_receiveBufferSize, p_s_pathToServerCertificate)
        {

        }

        /// <summary>
        /// Create receive socket instance for TCP network connection, executing within a thread.
        /// </summary>
        /// <param name="p_e_type">receiving type of socket instance - SERVER or CLIENT</param>
        /// <param name="p_s_host">address of receiving socket instance</param>
        /// <param name="p_i_port">port where receiving socket instance shall listen</param>
        /// <param name="p_o_socketTask">socket task object with the core execution process of a socket</param>
        /// <param name="p_i_timeoutMilliseconds">set timeout value for socket instance - how long the socket should wait for incoming data so it will not hold forever</param>
        /// <param name="p_i_maxTerminations">set a max. value for thread executions of socket instance</param>
        /// <param name="p_i_receiveBufferSize">receive buffer size for socket instance, e.g. ethernet packets have max. data length of 1500 bytes</param>
        /// <param name="p_s_pathToServerCertificate">path to server certificate file(.pfx) for TLS receive socket instance</param>
        /// <exception cref="ArgumentException">invalid parameter values for port, receive buffer size or timeout</exception>
        /// <exception cref="ArgumentNullException">socket task parameter is null</exception>
        /// <exception cref="System.IO.IOException">issue creating TCP or TCP with TLS socket instance</exception>
        public ReceiveTCP(ReceiveType p_e_type, string p_s_host, int p_i_port, ForestNET.Lib.Net.Sock.Task.Task p_o_socketTask, int p_i_timeoutMilliseconds, int p_i_maxTerminations, int p_i_receiveBufferSize, string? p_s_pathToServerCertificate) :
            this(p_e_type, p_s_host, p_i_port, p_o_socketTask, p_i_timeoutMilliseconds, p_i_maxTerminations, p_i_receiveBufferSize, p_s_pathToServerCertificate, null)
        {

        }

        /// <summary>
        /// Create receive socket instance for TCP network connection, executing within a thread.
        /// </summary>
        /// <param name="p_e_type">receiving type of socket instance - SERVER or CLIENT</param>
        /// <param name="p_s_host">address of receiving socket instance</param>
        /// <param name="p_i_port">port where receiving socket instance shall listen</param>
        /// <param name="p_o_socketTask">socket task object with the core execution process of a socket</param>
        /// <param name="p_i_timeoutMilliseconds">set timeout value for socket instance - how long the socket should wait for incoming data so it will not hold forever</param>
        /// <param name="p_i_maxTerminations">set a max. value for thread executions of socket instance</param>
        /// <param name="p_i_receiveBufferSize">receive buffer size for socket instance, e.g. ethernet packets have max. data length of 1500 bytes</param>
        /// <param name="p_s_pathToServerCertificate">path to server certificate file(.pfx) for TLS receive socket instance</param>
        /// <param name="p_s_certificatePassword">password to access certificate file(.pfx)</param>
        /// <exception cref="ArgumentException">invalid parameter values for port, receive buffer size or timeout</exception>
        /// <exception cref="ArgumentNullException">socket task parameter is null</exception>
        /// <exception cref="System.IO.IOException">issue creating TCP or TCP with TLS socket instance</exception>
        public ReceiveTCP(ReceiveType p_e_type, string p_s_host, int p_i_port, ForestNET.Lib.Net.Sock.Task.Task p_o_socketTask, int p_i_timeoutMilliseconds, int p_i_maxTerminations, int p_i_receiveBufferSize, string? p_s_pathToServerCertificate, string? p_s_certificatePassword)
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
            if (p_i_receiveBufferSize < 4)
            {
                throw new ArgumentException("Receive buffer size must be at least '4', but was set to '" + p_i_receiveBufferSize + "'");
            }
            else
            {
                /* set receive buffer size */
                this.BufferSize = p_i_receiveBufferSize;

                ForestNET.Lib.Global.ILogConfig("\t" + "set receive buffer size '" + p_i_receiveBufferSize + "'");
            }

            if (p_s_pathToServerCertificate != null)
            { /* create server certificate instance */
                ForestNET.Lib.Global.ILogConfig("\t" + "create server certificate instance");

                if (p_s_certificatePassword != null)
                {
                    this.ServerCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(p_s_pathToServerCertificate, p_s_certificatePassword);
                }
                else
                {
                    this.ServerCertificate = System.Security.Cryptography.X509Certificates.X509Certificate.CreateFromCertFile(p_s_pathToServerCertificate);
                }
            }

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
                ForestNET.Lib.Global.ILogConfig("run as receiving socket(TCP) instance");

                await this.RunSocket();
            }
            else if (this.RecvType == ReceiveType.SERVER)
            {
                ForestNET.Lib.Global.ILogConfig("run as receiving server(TCP) instance");

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

                ForestNET.Lib.Global.ILogConfig(((this.SocketTask.CommunicationType != null) ? this.SocketTask.CommunicationType + " " : "") + "socket bind to " + this.SocketAddress);

                /* create receive socket and bind to configured address */
                this.SocketInstance = new(this.SocketAddress.AddressFamily, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);

                ForestNET.Lib.Global.ILogConfig("created receive socket and bind to configured address");

                /* set receive and send buffer size */
                this.SocketInstance.ReceiveBufferSize = this.BufferSize;
                this.SocketInstance.SendBufferSize = this.BufferSize;
                /* set socket timeout value */
                this.SocketInstance.ReceiveTimeout = this.TimeoutMilliseconds;
                this.SocketInstance.SendTimeout = this.TimeoutMilliseconds;
                /* bind socket instance to configured address */
                this.SocketInstance.Bind(this.SocketAddress);
                /* places socket instance into listening state */
                this.SocketInstance.Listen(100);
                /* create cancellation token source for all socket tasks */
                this.CancellationTokenSource = new();

                /* endless loop for our receiving socket instance */
                while (!this.Stop)
                {
                    try
                    {
                        ForestNET.Lib.Global.ILogFine("this.SocketInstance.AcceptAsync");

                        System.Threading.Tasks.Task<System.Net.Sockets.Socket> o_taskSocket = this.SocketInstance.AcceptAsync();
                        System.Threading.Tasks.Task o_taskTimeout = System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(this.TimeoutMilliseconds), this.CancellationTokenSource?.Token ?? default);

                        System.Threading.Tasks.Task o_completedTask = await System.Threading.Tasks.Task.WhenAny(o_taskSocket, o_taskTimeout);

                        if (o_completedTask == o_taskSocket)
                        {
                            ForestNET.Lib.Global.ILogFine("ReceiveTCP-RunSocket method - incoming connection [" + o_taskSocket.Result.RemoteEndPoint + "]");

                            /* socket variable for using communication after connection is made - it is part of the socket task to close receiving socket */
                            this.SocketTask.ReceivingSocket = o_taskSocket.Result;
                            /* set receive and send buffer size */
                            this.SocketTask.ReceivingSocket.ReceiveBufferSize = this.BufferSize;
                            this.SocketTask.ReceivingSocket.SendBufferSize = this.BufferSize;
                            /* set socket timeout value */
                            this.SocketTask.ReceivingSocket.ReceiveTimeout = this.TimeoutMilliseconds;
                            this.SocketTask.ReceivingSocket.SendTimeout = this.TimeoutMilliseconds;

                            if (this.ServerCertificate != null)
                            { /* tls socket instance waiting for connection */
                                System.IO.Stream o_stream = new System.Net.Sockets.NetworkStream(this.SocketTask.ReceivingSocket);
                                this.SocketTask.Stream = new System.Net.Security.SslStream(o_stream, false);

                                /* temp variable as SslStream */
                                System.Net.Security.SslStream o_sslStream = (System.Net.Security.SslStream)this.SocketTask.Stream;

                                o_sslStream.AuthenticateAsServer(this.ServerCertificate, false, System.Security.Authentication.SslProtocols.Tls13, true); /* | System.Security.Authentication.SslProtocols.Tls | System.Security.Authentication.SslProtocols.Tls11 | System.Security.Authentication.SslProtocols.Tls12 */

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
                            else
                            { /* socket instance waiting for connection */
                                this.SocketTask.Stream = new System.Net.Sockets.NetworkStream(this.SocketTask.ReceivingSocket);
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

                            ForestNET.Lib.Global.ILogFine("Timeout ReceiveTCP-RunSocket method - Terminations: " + (this.Terminations + 1) + "/" + this.MaxTerminations);
                        }
                    }
                    catch (OperationCanceledException o_exc)
                    {
                        ForestNET.Lib.Global.ILogFiner("OperationCanceledException ReceiveTCP-RunSocket method: " + o_exc + " - Terminations: " + (this.Terminations + 1) + "/" + this.MaxTerminations);
                    }
                    catch (System.IO.IOException o_exc)
                    {
                        if (!this.Stop)
                        { /* exception occurred without control stop */
                            ForestNET.Lib.Global.LogException("IOException ReceiveTCP-RunSocket method: ", o_exc);
                            ForestNET.Lib.Global.ILogSevere("\tTerminations: " + (this.Terminations + 1) + "/" + this.MaxTerminations);
                        }
                    }
                    catch (System.Security.Authentication.AuthenticationException o_exc)
                    {
                        if (!this.StopServer)
                        { /* exception occurred without control stop */
                            ForestNET.Lib.Global.LogException("AuthenticationException ReceiveTCP-RunSocket method: ", o_exc);

                            if (o_exc.InnerException != null)
                            {
                                ForestNET.Lib.Global.LogException("AuthenticationException InnerException: ", o_exc.InnerException);
                            }

                            ForestNET.Lib.Global.ILogSevere("\tTerminations: " + (this.Terminations + 1) + "/" + this.MaxTerminations);
                        }
                    }
                    finally
                    {
                        try
                        {
                            /* by assigning MemoryStream we execute property set method to dispose/null network- or ssl-stream */
                            this.SocketTask.Stream = new MemoryStream();
                        }
                        catch (Exception)
                        {
                            /* ignore disposed exception, for the case that the stream has been closed within socket task */
                        }

                        try
                        {
                            /* close receiving socket for socket task instance */
                            this.SocketTask.ReceivingSocket?.Shutdown(System.Net.Sockets.SocketShutdown.Both);
                            this.SocketTask.ReceivingSocket?.Close();
                        }
                        catch (Exception)
                        {
                            /* ignore disposed exception, for the case that the socket has been closed within socket task */
                        }

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
                ForestNET.Lib.Global.LogException("Exception ReceiveTCP-RunSocket method: ", o_exc);
            }
            finally
            {
                /* set cancel signal to all socket tasks */
                this.CancellationTokenSource?.Cancel();
                this.CancellationTokenSource = null;
                /* close socket */
                this.SocketInstance?.Close();
                this.SocketInstance = null;

                ForestNET.Lib.Global.ILogConfig("ReceiveTCP-RunSocket method stopped");
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

                ForestNET.Lib.Global.ILogConfig(((this.SocketTask.CommunicationType != null) ? this.SocketTask.CommunicationType + " " : "") + "socket bind to " + this.SocketAddress);

                /* create receive socket and bind to configured address */
                this.SocketInstance = new(this.SocketAddress.AddressFamily, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);

                ForestNET.Lib.Global.ILogConfig("created receive socket and bind to configured address");

                /* set receive and send buffer size */
                this.SocketInstance.ReceiveBufferSize = this.BufferSize;
                this.SocketInstance.SendBufferSize = this.BufferSize;
                /* set socket timeout value */
                this.SocketInstance.ReceiveTimeout = this.TimeoutMilliseconds;
                this.SocketInstance.SendTimeout = this.TimeoutMilliseconds;
                /* bind socket instance to configured address */
                this.SocketInstance.Bind(this.SocketAddress);
                /* places socket instance into listening state */
                this.SocketInstance.Listen(100);
                /* create cancellation token source for all socket tasks */
                this.CancellationTokenSource = new();
                /* create semaphore for task pool, initially */
                SemaphoreSlim o_taskPool = new(0);

                /* check if we should reinstiate task pool with service pool amount value greater than '0' */
                if (this.ServicePoolAmount > 0)
                {
                    ForestNET.Lib.Global.ILogConfig("create task pool with amout of '" + this.ServicePoolAmount + "'");

                    o_taskPool = new(this.ServicePoolAmount);
                }

                /* endless loop for our receiving server instance */
                while (!this.StopServer)
                {
                    try
                    {
                        /* create new instance of socket task, because for every request we need a new one; otherwise we just overwrite the socket instance and do chaos to our communication and so on */
                        ForestNET.Lib.Net.Sock.Task.Task o_socketTaskInstance = (ForestNET.Lib.Net.Sock.Task.Task)(Activator.CreateInstance(this.SocketTask.GetType()) ?? throw new TypeLoadException("could not create new instance of socket task"));
                        o_socketTaskInstance.CloneFromOtherTask(this.SocketTask);

                        ForestNET.Lib.Global.ILogFine("this.SocketInstance.AcceptAsync");

                        System.Threading.Tasks.Task<System.Net.Sockets.Socket> o_taskSocket = this.SocketInstance.AcceptAsync();
                        System.Threading.Tasks.Task o_taskTimeout = System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(this.TimeoutMilliseconds), this.CancellationTokenSource?.Token ?? default);

                        System.Threading.Tasks.Task o_completedTask = await System.Threading.Tasks.Task.WhenAny(o_taskSocket, o_taskTimeout);

                        if (o_completedTask == o_taskSocket)
                        {
                            ForestNET.Lib.Global.ILogFine("ReceiveTCP-RunServer method - incoming connection [" + o_taskSocket.Result.RemoteEndPoint + "]");

                            /* socket variable for using communication after connection is made - it is part of the socket task to close receiving socket */
                            o_socketTaskInstance.ReceivingSocket = o_taskSocket.Result;
                            /* set receive and send buffer size */
                            o_socketTaskInstance.ReceivingSocket.ReceiveBufferSize = this.BufferSize;
                            o_socketTaskInstance.ReceivingSocket.SendBufferSize = this.BufferSize;
                            /* set socket timeout value */
                            o_socketTaskInstance.ReceivingSocket.ReceiveTimeout = this.TimeoutMilliseconds;
                            o_socketTaskInstance.ReceivingSocket.SendTimeout = this.TimeoutMilliseconds;

                            if (this.ServerCertificate != null)
                            { /* tls socket instance waiting for connection */
                                System.IO.Stream o_stream = new System.Net.Sockets.NetworkStream(o_socketTaskInstance.ReceivingSocket);
                                o_socketTaskInstance.Stream = new System.Net.Security.SslStream(o_stream, false);

                                /* temp variable as SslStream */
                                System.Net.Security.SslStream o_sslStream = (System.Net.Security.SslStream)o_socketTaskInstance.Stream;

                                o_sslStream.AuthenticateAsServer(this.ServerCertificate, false, System.Security.Authentication.SslProtocols.Tls13, true); /* | System.Security.Authentication.SslProtocols.Tls | System.Security.Authentication.SslProtocols.Tls11 | System.Security.Authentication.SslProtocols.Tls12 */

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
                            else
                            { /* socket instance waiting for connection */
                                o_socketTaskInstance.Stream = new System.Net.Sockets.NetworkStream(o_socketTaskInstance.ReceivingSocket);
                            }

                            /* set timeout values from receiving socket instance to stream instance */
                            o_socketTaskInstance.Stream.ReadTimeout = o_socketTaskInstance.ReceivingSocket.ReceiveTimeout;
                            o_socketTaskInstance.Stream.WriteTimeout = o_socketTaskInstance.ReceivingSocket.SendTimeout;
                            /* pass socket instance for answering request to socket task */
                            o_socketTaskInstance.Socket = this;
                            /* pass buffer size length to socket task */
                            o_socketTaskInstance.BufferLength = this.BufferSize;
                            /* set cancellation token for socket task */
                            o_socketTaskInstance.CancellationToken = this.CancellationTokenSource?.Token;
                            /* run socket task as detached child task, but keep in touch by adding cancellation token and waiting in task pool */
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

                                                throw new OperationCanceledException("timeout for waiting in task pool is over. receiving socket is closed");
                                            }
                                        }
                                        finally
                                        {
                                            try
                                            {
                                                /* by assigning MemoryStream we execute property set method to dispose/null network- or ssl-stream */
                                                o_socketTaskInstance.Stream = new MemoryStream();
                                            }
                                            catch (Exception)
                                            {
                                                /* ignore disposed exception, for the case that the stream has been closed within socket task */
                                            }

                                            try
                                            {
                                                /* close receiving socket for current socket task instance */
                                                o_socketTaskInstance.ReceivingSocket?.Shutdown(System.Net.Sockets.SocketShutdown.Both);
                                                o_socketTaskInstance.ReceivingSocket?.Close();
                                            }
                                            catch (Exception)
                                            {
                                                /* ignore disposed exception, for the case that the socket has been closed within socket task */
                                            }

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
                        else
                        {
                            if (!o_taskTimeout.IsCanceled && !o_taskTimeout.IsCompleted && !o_taskTimeout.IsFaulted)
                            {
                                o_taskTimeout.Dispose();
                            }

                            ForestNET.Lib.Global.ILogFine("Timeout ReceiveTCP-RunServer method - Terminations: " + (this.Terminations + 1) + "/" + this.MaxTerminations);
                        }
                    }
                    catch (System.IO.IOException o_exc)
                    {
                        if (!this.StopServer)
                        { /* exception occurred without control stop */
                            ForestNET.Lib.Global.LogException("IOException ReceiveTCP-RunServer method: ", o_exc);
                            ForestNET.Lib.Global.ILogSevere("\tTerminations: " + (this.Terminations + 1) + "/" + this.MaxTerminations);
                        }
                    }
                    catch (System.Security.Authentication.AuthenticationException o_exc)
                    {
                        if (!this.StopServer)
                        { /* exception occurred without control stop */
                            ForestNET.Lib.Global.LogException("AuthenticationException ReceiveTCP-RunServer method: ", o_exc);

                            if (o_exc.InnerException != null)
                            {
                                ForestNET.Lib.Global.LogException("AuthenticationException InnerException: ", o_exc.InnerException);
                            }

                            ForestNET.Lib.Global.ILogSevere("\tTerminations: " + (this.Terminations + 1) + "/" + this.MaxTerminations);
                        }
                    }
                    catch (OperationCanceledException o_exc)
                    {
                        ForestNET.Lib.Global.ILogFiner("OperationCanceledException ReceiveTCP-RunServer method: " + o_exc + " - Terminations: " + (this.Terminations + 1) + "/" + this.MaxTerminations);
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
                ForestNET.Lib.Global.LogException("Exception ReceiveTCP-RunServer method: ", o_exc);
            }
            finally
            {
                /* set cancel signal to all socket tasks */
                this.CancellationTokenSource?.Cancel();
                this.CancellationTokenSource = null;

                /* wait 2.5 seconds so cancel signal reaching all socket tasks */
                await System.Threading.Tasks.Task.Delay(2500);

                /* close socket */
                this.SocketInstance?.Close();
                this.SocketInstance = null;

                ForestNET.Lib.Global.ILogConfig("ReceiveTCP-RunServer method stopped");
            }
        }
    }
}