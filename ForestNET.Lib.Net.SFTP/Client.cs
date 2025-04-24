using Renci.SshNet;

namespace ForestNET.Lib.Net.SFTP
{
    /// <summary>
    /// SFTP Client class for a connection to a sftp server. SSH tunneling supported.
    /// Authentication with user and password, or with key authentication file possible.
    /// Methods work like the known commands in most CLI implementations, but will always use the complete path to a file or directory from root '/'.
    /// </summary>
    public class Client : IDisposable
    {

        /* Constants */

        public const uint BUFFERSIZE = 8192;

        /* Delegates */

        /// <summary>
        /// interface delegate definition which can be instanced outside of sftp.Client class to post progress anywhere of download/upload methods
        /// </summary>
        public delegate void PostProgress(double p_d_progress);

        /// <summary>
        /// interface delegate definition which can be instanced outside of sftp.Client class to post progress anywhere of amount of files been processed for download/upload
        /// </summary>
        public delegate void PostProgressFolder(int p_i_filesProcessed, int p_i_files);

        /* Fields */

        private SftpClient? o_sftpClient = null;
        private CancellationTokenSource? o_tokenSource = null;

        private bool b_loggedIn = false;

        private string s_host = string.Empty;
        private int i_port = 0;
        private string s_user = string.Empty;
        private string? s_password = null;
        private System.Text.Encoding o_encoding = System.Text.Encoding.UTF8;
        private TimeSpan? o_keepAliveInterval = null;
        private uint i_bufferSize;
        private TimeSpan o_timeout;
        private string? s_filePathAuthentication = null;
        private string? s_filePathKnownHosts = null;
        private bool b_strictHostChecking = true;

        private ProxyTypes? e_proxyType = null;
        private string s_proxyHost = string.Empty;
        private int i_proxyPort = 0;
        private string s_proxyUser = string.Empty;
        private string s_proxyPassword = string.Empty;

        private int i_dirSum;
        private int i_dirFiles;

        private SshClient? o_sshClient = null;
        private ForwardedPortLocal? o_forwardedPortLocal = null;
        private string s_tunnelHost = string.Empty;
        private int i_tunnelLocalPort = 0;
        private int i_tunnelPort = 0;
        private string s_tunnelUser = string.Empty;
        private string? s_tunnelPassword = null;
        private string? s_tunnelFilePathAuthentication = null;

        private ProxyTypes? e_tunnelProxyType = null;
        private string s_tunnelProxyHost = string.Empty;
        private int i_tunnelProxyPort = 0;
        private string s_tunnelProxyUser = string.Empty;
        private string s_tunnelProxyPassword = string.Empty;

        /* Properties */

        private SftpClient SftpClient
        {
            get
            {
                return this.o_sftpClient ?? throw new NullReferenceException("sftp client instance is null");
            }
        }
        public string Host
        {
            get
            {
                return this.s_host;
            }
            set
            {
                /* check if sftp host is not empty */
                if (ForestNET.Lib.Helper.IsStringEmpty(value))
                {
                    throw new ArgumentException("SFTP host is null or empty");
                }

                if (value.StartsWith("sftp://"))
                {
                    value = value.Substring(7);
                }

                this.s_host = value;
            }
        }
        public int Port
        {
            get { return this.i_port; }
            set
            {
                /* check valid sftp port number */
                if (value < 1)
                {
                    throw new ArgumentException("SFTP port must be at least '1', but was set to '" + value + "'");
                }

                /* check valid sftp port number */
                if (value > 65535)
                {
                    throw new ArgumentException("SFTP port must be lower equal '65535', but was set to '" + value + "'");
                }

                this.i_port = value;
            }
        }
        public string User
        {
            get
            {
                return this.s_user;
            }
            set
            {
                /* check if sftp user is not empty */
                if (ForestNET.Lib.Helper.IsStringEmpty(value))
                {
                    throw new ArgumentException("SFTP user is null or empty");
                }

                this.s_user = value;
            }
        }
        public string? Password
        {
            get => this.s_password;
            set => this.s_password = value;
        }
        public System.Text.Encoding Encoding
        {
            get => this.o_encoding;
            set => this.o_encoding = value;
        }
        public TimeSpan? KeepAliveInterval
        {
            get => this.o_keepAliveInterval;
            set => this.o_keepAliveInterval = value;
        }
        public uint BufferSize
        {
            get
            {
                return this.i_bufferSize;
            }
            set
            {
                /* check valid buffer size min. */
                if (value < 1)
                {
                    throw new ArgumentException("SFTP buffer size must be at least '1', but was set to '" + value + "'");
                }

                /* check valid buffer size max. */
                if (value > 32768)
                {
                    throw new ArgumentException("SFTP buffer size must be lower equal '32768', but was set to '" + value + "'");
                }

                this.i_bufferSize = value;
            }
        }
        public TimeSpan Timeout
        {
            get
            {
                return this.o_timeout;
            }
            set
            {
                /* check valid buffer size min. */
                if (value.Milliseconds < 0)
                {
                    throw new ArgumentException("SFTP timeout (ms) must be at least '0', but was set to '" + value + "'");
                }

                /* check valid buffer size max. */
                if (value.Milliseconds > 60000)
                {
                    throw new ArgumentException("SFTP timeout (ms) must be lower equal '60000', but was set to '" + value + "'");
                }

                this.o_timeout = value;
            }
        }
        public string? FilePathAuthentication
        {
            get => this.s_filePathAuthentication;
            set => this.s_filePathAuthentication = value;
        }
        public string? FilePathKnownHosts
        {
            get => this.s_filePathKnownHosts;
            set => this.s_filePathKnownHosts = value;
        }
        public bool StrictHostChecking
        {
            get => this.b_strictHostChecking;
            set => this.b_strictHostChecking = value;
        }
        public ProxyTypes? ProxyType
        {
            get => this.e_proxyType;
            set
            {
                if (value == null)
                {
                    this.e_proxyType = null;
                    this.s_proxyHost = string.Empty;
                    this.i_proxyPort = 0;
                    this.s_proxyUser = string.Empty;
                    this.s_proxyPassword = string.Empty;
                }
                else
                {
                    this.e_proxyType = value;
                }
            }
        }
        public string ProxyHost
        {
            get => this.s_proxyHost;
            set => this.s_proxyHost = value;
        }
        public int ProxyPort
        {
            get => this.i_proxyPort;
            set
            {
                /* check valid sftp proxy port number */
                if (value < 1)
                {
                    throw new ArgumentException("SFTP proxy port must be at least '1', but was set to '" + value + "'");
                }

                /* check valid sftp proxy port number */
                if (value > 65535)
                {
                    throw new ArgumentException("SFTP proxy port must be lower equal '65535', but was set to '" + value + "'");
                }

                this.i_proxyPort = value;
            }
        }
        public string ProxyUser
        {
            get => this.s_proxyUser;
            set => this.s_proxyUser = value;
        }
        public string ProxyPassword
        {
            get => this.s_proxyPassword;
            set => this.s_proxyPassword = value;
        }
        public PostProgress? DelegatePostProgress { private get; set; } = null;
        public PostProgressFolder? DelegatePostProgressFolder { private get; set; } = null;
        public ForestNET.Lib.ConsoleProgressBar? ConsoleProgressBar { private get; set; }
        public string TunnelHost
        {
            get
            {
                return this.s_tunnelHost;
            }
            set
            {
                /* check if tunnel host is not empty */
                if (ForestNET.Lib.Helper.IsStringEmpty(value))
                {
                    throw new ArgumentException("Tunnel host is null or empty");
                }

                this.s_tunnelHost = value;
            }
        }
        public int TunnelLocalPort
        {
            get { return this.i_tunnelLocalPort; }
            set
            {
                /* check valid tunnel local port number */
                if (value < 1)
                {
                    throw new ArgumentException("Tunnel local port must be at least '1', but was set to '" + value + "'");
                }

                /* check valid tunnel local port number */
                if (value > 65535)
                {
                    throw new ArgumentException("Tunnel local port must be lower equal '65535', but was set to '" + value + "'");
                }

                this.i_tunnelLocalPort = value;
            }
        }
        public int TunnelPort
        {
            get { return this.i_tunnelPort; }
            set
            {
                /* check valid tunnel port number */
                if (value < 1)
                {
                    throw new ArgumentException("Tunnel port must be at least '1', but was set to '" + value + "'");
                }

                /* check valid tunnel port number */
                if (value > 65535)
                {
                    throw new ArgumentException("Tunnel port must be lower equal '65535', but was set to '" + value + "'");
                }

                this.i_tunnelPort = value;
            }
        }
        public string TunnelUser
        {
            get
            {
                return this.s_tunnelUser;
            }
            set
            {
                /* check if tunnel user is not empty */
                if (ForestNET.Lib.Helper.IsStringEmpty(value))
                {
                    throw new ArgumentException("Tunnel user is null or empty");
                }

                this.s_tunnelUser = value;
            }
        }
        public string? TunnelPassword
        {
            get => this.s_tunnelPassword;
            set => this.s_tunnelPassword = value;
        }
        public string? TunnelFilePathAuthentication
        {
            get => this.s_tunnelFilePathAuthentication;
            set => this.s_tunnelFilePathAuthentication = value;
        }
        public ProxyTypes? TunnelProxyType
        {
            get => this.e_tunnelProxyType;
            set
            {
                if (value == null)
                {
                    this.e_tunnelProxyType = null;
                    this.s_tunnelProxyHost = string.Empty;
                    this.i_tunnelProxyPort = 0;
                    this.s_tunnelProxyUser = string.Empty;
                    this.s_tunnelProxyPassword = string.Empty;
                }
                else
                {
                    this.e_proxyType = value;
                }
            }
        }
        public string TunnelProxyHost
        {
            get => this.s_tunnelProxyHost;
            set => this.s_tunnelProxyHost = value;
        }
        public int TunnelProxyPort
        {
            get => this.i_tunnelProxyPort;
            set
            {
                /* check valid tunnel proxy port number */
                if (value < 1)
                {
                    throw new ArgumentException("Tunnel proxy port must be at least '1', but was set to '" + value + "'");
                }

                /* check valid tunnel proxy port number */
                if (value > 65535)
                {
                    throw new ArgumentException("Tunnel proxy port must be lower equal '65535', but was set to '" + value + "'");
                }

                this.i_tunnelProxyPort = value;
            }
        }
        public string TunnelProxyUser
        {
            get => this.s_tunnelProxyUser;
            set => this.s_tunnelProxyUser = value;
        }
        public string TunnelProxyPassword
        {
            get => this.s_tunnelProxyPassword;
            set => this.s_tunnelProxyPassword = value;
        }

        /* Methods */

        /// <summary>
        /// Constructor for a sftp client object, using uri parameter specifications.
        /// using Client.BUFFERSIZE(8192) for input/output buffer.
        /// </summary>
        /// <param name="p_s_uri">sftp uri, must start with 'sftp://' and contain host, port, user and password information</param>
        /// <exception cref="ArgumentException">invalid sftp uri</exception>
        /// <exception cref="System.IO.IOException">issues starting connection to the sftp server</exception>
        public Client(string p_s_uri) :
            this(p_s_uri, null)
        {

        }

        /// <summary>
        /// Constructor for a sftp client object, using uri parameter specifications.
        /// using Client.BUFFERSIZE(8192) for input/output buffer, check server connection with known hosts.
        /// </summary>
        /// <param name="p_s_uri">sftp uri, must start with 'sftp://' and contain host, port, user and password information</param>
        /// <param name="p_s_filePathKnownHosts">file path to known hosts file</param>
        /// <exception cref="ArgumentException">invalid sftp uri</exception>
        /// <exception cref="System.IO.IOException">issues starting connection to the sftp server</exception>
        public Client(string p_s_uri, string? p_s_filePathKnownHosts) :
            this(p_s_uri, p_s_filePathKnownHosts, Client.BUFFERSIZE, true)
        {

        }

        /// <summary>
        /// Constructor for a sftp client object, using uri parameter specifications, check server connection with known hosts
        /// </summary>
        /// <param name="p_s_uri">sftp uri, must start with 'sftp://' and contain host, port, user and password information</param>
        /// <param name="p_s_filePathKnownHosts">file path to known hosts file</param>
        /// <param name="p_i_bufferSize">specify buffer size for sending and receiving data</param>
        /// <exception cref="ArgumentException">invalid sftp uri</exception>
        /// <exception cref="System.IO.IOException">issues starting connection to the sftp server</exception>
        public Client(string p_s_uri, string? p_s_filePathKnownHosts, uint p_i_bufferSize) :
            this(p_s_uri, p_s_filePathKnownHosts, p_i_bufferSize, true)
        {

        }

        /// <summary>
        /// Constructor for a sftp client object, using uri parameter specifications
        /// </summary>
        /// <param name="p_s_uri">sftp uri, must start with 'sftp://' and contain host, port, user and password information</param>
        /// <param name="p_s_filePathKnownHosts">file path to known hosts file</param>
        /// <param name="p_i_bufferSize">specify buffer size for sending and receiving data</param>
        /// <param name="p_b_strictHostKeyChecking">true - check server connection with known hosts, false - ignore unknown server fingerprint (insecure)</param>
        /// <exception cref="ArgumentException">invalid sftp uri</exception>
        /// <exception cref="System.IO.IOException">issues starting connection to the sftp server</exception>
        public Client(string p_s_uri, string? p_s_filePathKnownHosts, uint p_i_bufferSize, bool p_b_strictHostChecking)
        {
            /* check if sftp uri is valid */
            if (!ForestNET.Lib.Helper.MatchesRegex(p_s_uri, "(sftp://)([a-zA-Z0-9-_]{4,}):([^\\s]{4,})@([a-zA-Z0-9-_\\.]{4,}):([0-9]{2,5})"))
            {
                throw new ArgumentException("Invalid sftp uri. Valid sftp uri would be 'sftp://user:password@example.com(:22)'");
            }

            ForestNET.Lib.Global.ILogConfig("using uri: " + ForestNET.Lib.Helper.DisguiseSubstring(p_s_uri, "//", "@", '*'));

            string s_protocol = "sftp://";

            /* recognize protocol at start of uri and remove it from uri parameter */
            if (p_s_uri.StartsWith(s_protocol))
            {
                p_s_uri = p_s_uri.Substring(7);
            }

            /* get user and password value */
            string[] a_uriParts = p_s_uri.Split("@");
            string s_tempUser = a_uriParts[0].Substring(0, a_uriParts[0].IndexOf(":"));
            string s_tempPassword = a_uriParts[0].Substring(a_uriParts[0].IndexOf(":") + 1);

            string s_tempHost = a_uriParts[1];


            /* remove last '/' character and get host-address */
            if ((s_tempHost.Contains('/')) && (s_tempHost.Substring(s_tempHost.IndexOf("/")).Equals("/")))
            {
                s_tempHost = s_tempHost.Substring(0, (s_tempHost.Length - 1));
            }

            /* get port from uri if it is available */
            int i_tempPort = 22;

            if (s_tempHost.Contains(':'))
            {
                i_tempPort = Convert.ToInt32(s_tempHost.Substring(s_tempHost.IndexOf(":") + 1));
                s_tempHost = s_tempHost.Substring(0, s_tempHost.IndexOf(":"));
            }

            ForestNET.Lib.Global.ILogConfig("retrieved user, password, host-address and port from uri");

            this.Host = s_protocol + s_tempHost;
            this.Port = i_tempPort;
            this.User = s_tempUser;
            this.Password = s_tempPassword;
            this.BufferSize = p_i_bufferSize;
            this.Timeout = TimeSpan.FromSeconds(15);
            this.Encoding = System.Text.Encoding.UTF8;
            /* standard: no keep alive this.KeepAlive = TimeSpan.FromMinutes(1);*/
            this.FilePathKnownHosts = p_s_filePathKnownHosts;
            this.StrictHostChecking = p_b_strictHostChecking;
        }

        /// <summary>
        /// Constructor for a sftp client object, using separate host, port and user parameter specifications.
        /// using Client.BUFFERSIZE(8192) for input/output buffer.
        /// </summary>
        /// <param name="p_s_host">sftp host value, must start with 'sftp://'</param>
        /// <param name="p_i_port">sftp port value</param>
        /// <param name="p_s_user">user value</param>
        /// <param name="p_s_filePathAuthentication">file path to authentication file with private key</param>
        /// <exception cref="ArgumentException">wrong sftp host value, invalid sftp port number, missing user or password</exception>
        /// <exception cref="System.IO.IOException">issues starting connection to the sftp server</exception>
        public Client(string p_s_host, int p_i_port, string p_s_user, string? p_s_filePathAuthentication) :
            this(p_s_host, p_i_port, p_s_user, p_s_filePathAuthentication, null)
        {

        }

        /// <summary>
        /// Constructor for a sftp client object, using separate host, port, user and password parameter specifications.
        /// using Client.BUFFERSIZE(8192) for input/output buffer.
        /// </summary>
        /// <param name="p_s_host">sftp host value, must start with 'sftp://'</param>
        /// <param name="p_i_port">sftp port value</param>
        /// <param name="p_s_user">user value</param>
        /// <param name="p_s_filePathAuthentication">file path to authentication file with private key</param>
        /// <param name="p_s_password">password for authentication file (null for none)</param>
        /// <exception cref="ArgumentException">wrong sftp host value, invalid sftp port number, missing user or password</exception>
        /// <exception cref="System.IO.IOException">issues starting connection to the sftp server</exception>
        public Client(string p_s_host, int p_i_port, string p_s_user, string? p_s_filePathAuthentication, string? p_s_password) :
            this(p_s_host, p_i_port, p_s_user, p_s_password, p_s_filePathAuthentication, null)
        {

        }

        /// <summary>
        /// Constructor for a sftp client object, using separate host, port, user and authentication file parameter specifications.
        /// using Client.BUFFERSIZE(8192) for input/output buffer.
        /// </summary>
        /// <param name="p_s_host">sftp host value, must start with 'sftp://'</param>
        /// <param name="p_i_port">sftp port value</param>
        /// <param name="p_s_user">user value</param>
        /// <param name="p_s_filePathAuthentication">file path to authentication file with private key</param>
        /// <param name="p_s_password">password for authentication file (null for none)</param>
        /// <param name="p_s_filePathKnownHosts">file path to known hosts file</param>
        /// <exception cref="ArgumentException">wrong sftp host value, invalid sftp port number, missing user or password</exception>
        /// <exception cref="System.IO.IOException">issues starting connection to the sftp server</exception>
        public Client(string p_s_host, int p_i_port, string p_s_user, string? p_s_filePathAuthentication, string? p_s_password, string? p_s_filePathKnownHosts) :
            this(p_s_host, p_i_port, p_s_user, p_s_password, p_s_filePathAuthentication, p_s_filePathKnownHosts, Client.BUFFERSIZE)
        {

        }

        /// <summary>
        /// Constructor for a sftp client object, using separate host, port, user and authentication file parameter specifications.
        /// using Client.BUFFERSIZE(8192) for input/output buffer, check server connection with known hosts.
        /// </summary>
        /// <param name="p_s_host">sftp host value, must start with 'sftp://'</param>
        /// <param name="p_i_port">sftp port value</param>
        /// <param name="p_s_user">user value</param>
        /// <param name="p_s_filePathAuthentication">file path to authentication file with private key</param>
        /// <param name="p_s_password">password for authentication file (null for none)</param>
        /// <param name="p_s_filePathKnownHosts">file path to known hosts file</param>
        /// <param name="p_i_bufferSize">specify buffer size for sending and receiving data</param>
        /// <exception cref="ArgumentException">wrong sftp host value, invalid sftp port number, missing user or password</exception>
        /// <exception cref="System.IO.IOException">issues starting connection to the sftp server</exception>
        public Client(string p_s_host, int p_i_port, string p_s_user, string? p_s_filePathAuthentication, string? p_s_password, string? p_s_filePathKnownHosts, uint p_i_bufferSize) :
            this(p_s_host, p_i_port, p_s_user, p_s_password, p_s_filePathAuthentication, p_s_filePathKnownHosts, p_i_bufferSize, true)
        {

        }

        /// <summary>
        /// Constructor for a sftp client object, using separate host, port, user, password, authentication file, known hosts file and buffer size parameter specifications
        /// </summary>
        /// <param name="p_s_host">sftp host value, must start with 'sftp://'</param>
        /// <param name="p_i_port">sftp port value</param>
        /// <param name="p_s_user">user value</param>
        /// <param name="p_s_filePathAuthentication">file path to authentication file with private key</param>
        /// <param name="p_s_password">password for authentication file (null for none)</param>
        /// <param name="p_s_filePathKnownHosts">file path to known hosts file</param>
        /// <param name="p_i_bufferSize">specify buffer size for sending and receiving data</param>
        /// <param name="p_b_strictHostChecking">true - check server connection with known hosts, false - ignore unknown server fingerprint (insecure)</param>
        /// <exception cref="ArgumentException">wrong sftp host value, invalid sftp port number, missing user or password</exception>
        /// <exception cref="System.IO.IOException">issues starting connection to the sftp server</exception>
        public Client(string p_s_host, int p_i_port, string p_s_user, string? p_s_filePathAuthentication, string? p_s_password, string? p_s_filePathKnownHosts, uint p_i_bufferSize, bool p_b_strictHostChecking)
        {
            /* check if host parameter is not null or empty */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_host))
            {
                throw new ArgumentException("SFTP host is null or empty");
            }

            ForestNET.Lib.Global.ILogConfig("set user, password, host-address and port");

            this.Host = p_s_host;
            this.Port = p_i_port;
            this.User = p_s_user;
            this.Password = p_s_password;
            this.BufferSize = p_i_bufferSize;
            this.Timeout = TimeSpan.FromSeconds(15);
            this.Encoding = System.Text.Encoding.UTF8;
            /* standard: no keep alive this.KeepAlive = TimeSpan.FromMinutes(1);*/
            this.FilePathAuthentication = p_s_filePathAuthentication;
            this.FilePathKnownHosts = p_s_filePathKnownHosts;
            this.StrictHostChecking = p_b_strictHostChecking;
        }

        /// <summary>
        /// Get cancellation token of CancellationTokenSource instance of our current sftp client instance
        /// </summary>
        private CancellationToken Token()
        {
            return (this.o_tokenSource != null) ? this.o_tokenSource.Token : default;
        }

        /// <summary>
        /// Create sftp client object and start connection to sftp server
        /// </summary>
        /// <exception cref="System.IO.IOException">issues starting connection to the sftp server</exception>
        public async Task<bool> Login()
        {
            /* if current instance is logged in, we must re-login */
            if (this.b_loggedIn)
            {
                ForestNET.Lib.Global.ILogConfig("current instance is logged in, we must re-login so logout procedure will be called");

                if (!this.Logout())
                {
                    throw new System.IO.IOException("SFTP-Error within Logout method");
                }
            }

            try
            {
                ForestNET.Lib.Global.ILogConfig("renew cancellation token source");

                /* renew cancellation token source */
                this.o_tokenSource = new();

                /*
                 * ssh tunnel part
                 */

                /* check if we want to connect to sftp instance via ssh tunnel */
                if (!ForestNET.Lib.Helper.IsStringEmpty(this.TunnelHost))
                {
                    /* password or file with private key for ssh tunnel are necessary */
                    if ((ForestNET.Lib.Helper.IsStringEmpty(this.TunnelPassword)) && (ForestNET.Lib.Helper.IsStringEmpty(this.TunnelFilePathAuthentication)))
                    {
                        throw new ArgumentException("Tunnel password or file path with authentication file is null or empty");
                    }

                    ForestNET.Lib.Global.ILogConfig("start configuration steps to create a ssh tunnel session");

                    /* list of authentication methods for ssh tunnel */
                    List<AuthenticationMethod> a_tunnelAuthenticationMethods = [];

                    /* check if parameter is a file path to authentication file */
                    if ((this.TunnelFilePathAuthentication != null) && (!ForestNET.Lib.Helper.IsStringEmpty(this.TunnelFilePathAuthentication)) && (ForestNET.Lib.IO.File.Exists(this.TunnelFilePathAuthentication)))
                    {
                        ForestNET.Lib.Global.ILogConfig("add authentication file '" + this.TunnelFilePathAuthentication + "' as identity for ssh tunnel");

                        PrivateKeyFile o_privateKey;

                        /* use password to access authentication private key for ssh tunnel */
                        if (!ForestNET.Lib.Helper.IsStringEmpty(this.TunnelPassword))
                        {
                            /* create private key instance and use password to access authentication private key for ssh tunnel */
                            o_privateKey = new(TunnelFilePathAuthentication, this.TunnelPassword);
                        }
                        else
                        { /* private key without password */
                            /* create private key instance for ssh tunnel */
                            o_privateKey = new(TunnelFilePathAuthentication);
                        }

                        /* add authentication file method for ssh tunnel */
                        a_tunnelAuthenticationMethods.Add(new PrivateKeyAuthenticationMethod(this.TunnelUser, new[] { o_privateKey }));
                    }
                    else
                    { /* use user and password authentication for ssh tunnel */
                        a_tunnelAuthenticationMethods.Add(new PasswordAuthenticationMethod(this.TunnelUser, this.TunnelPassword));
                    }

                    /* variable for ssh tunnel configuration instance */
                    ConnectionInfo o_sshConfig;

                    /* create ssh tunnel configuration instance with proxy settings */
                    if (this.TunnelProxyType != null)
                    {
                        ForestNET.Lib.Global.ILogConfig("create ssh tunnel configuration with '" + this.TunnelUser + "' at '" + this.TunnelHost + ":" + this.TunnelPort + "' over proxy '" + this.TunnelProxyHost + ":" + this.TunnelProxyPort + "'");

                        o_sshConfig = new(this.TunnelHost, this.TunnelPort, this.TunnelUser, this.TunnelProxyType ?? throw new NullReferenceException("Tunnel proxy type is null"), this.TunnelProxyHost, this.TunnelProxyPort, this.TunnelProxyUser, this.TunnelProxyPassword, [.. a_tunnelAuthenticationMethods]);
                    }
                    else
                    { /* create ssh tunnel configuration instance without proxy */
                        ForestNET.Lib.Global.ILogConfig("create ssh tunnel configuration with '" + this.TunnelUser + "' at '" + this.TunnelHost + ":" + this.TunnelPort + "'");

                        o_sshConfig = new(this.TunnelHost, this.TunnelPort, this.TunnelUser, [.. a_tunnelAuthenticationMethods]);
                    }

                    ForestNET.Lib.Global.ILogConfig("set timeout '" + this.Timeout.TotalMilliseconds + " ms' for ssh tunnel");

                    /* set timeout for ssh tunnel */
                    o_sshConfig.Timeout = this.Timeout;

                    /* create ssh tunnel instance */
                    this.o_sshClient = new(o_sshConfig);

                    if (this.KeepAliveInterval != null)
                    {
                        ForestNET.Lib.Global.ILogConfig("set keep alive interval '" + this.KeepAliveInterval + "' for ssh tunnel");

                        /* set keep alive interval for ssh tunnel */
                        this.o_sshClient.KeepAliveInterval = (TimeSpan)this.KeepAliveInterval;
                    }

                    ForestNET.Lib.Global.ILogConfig("set operation timeout '" + this.Timeout.TotalMilliseconds + " ms' for ssh tunnel");

                    /* allow any host connection (insecure) */
                    if (!this.StrictHostChecking)
                    {
                        ForestNET.Lib.Global.ILogConfig("allow any host connection fro ssh tunnel");
                        ForestNET.Lib.Global.LogWarning("ForestNET.Lib.Net.SFTP.Client strict host checking been set to 'false'");
                        ForestNET.Lib.Global.LogWarning("Ignoring unknown server fingerprint with ssh connection, using this is a security vulnerability!");

                        /* trust any host connection */
                        this.o_sshClient.HostKeyReceived += (o_sender, o_hostKeyEventArgs) => { o_hostKeyEventArgs.CanTrust = true; };
                    }
                    else if (!ForestNET.Lib.Helper.IsStringEmpty(this.FilePathKnownHosts))
                    { /* use known hosts file */
                        ForestNET.Lib.Global.ILogConfig("use known hosts file '" + this.FilePathKnownHosts + "' for host key check of ssh tunnel");

                        this.o_sshClient.HostKeyReceived += (o_sender, o_hostKeyEventArgs) => this.ConfirmReceivedHostKey(o_sender, o_hostKeyEventArgs);
                    }

                    ForestNET.Lib.Global.ILogConfig("start connection to ssh tunnel");

                    /* start connection to sftp host */
                    await this.o_sshClient.ConnectAsync(this.Token());

                    ForestNET.Lib.Global.ILogConfig("register local port '" + this.TunnelLocalPort + "' forwarding for ssh tunnel to connect to '" + this.Host + ":" + this.Port + "'");

                    /* register local port forwarding for ssh tunnel */
                    this.o_forwardedPortLocal = new("localhost", (uint)this.TunnelLocalPort, this.Host, (uint)this.Port);

                    /* handle port forwarding exceptions */
                    this.o_forwardedPortLocal.Exception += delegate (object? sender, Renci.SshNet.Common.ExceptionEventArgs e)
                    {
                        /* log port forwarding exception */
                        ForestNET.Lib.Global.LogException(e.Exception);
                        /* logout current session and set all to null */
                        this.Logout();
                    };

                    ForestNET.Lib.Global.ILogConfig("add forwarding local port to ssh client session");

                    /* add forwarding local port to ssh client session */
                    this.o_sshClient.AddForwardedPort(this.o_forwardedPortLocal);

                    /* start port forwarding */
                    this.o_forwardedPortLocal.Start();
                }

                /*
                 * sftp part
                 */

                /* password or file with private key necessary */
                if ((ForestNET.Lib.Helper.IsStringEmpty(this.Password)) && (ForestNET.Lib.Helper.IsStringEmpty(this.FilePathAuthentication)))
                {
                    throw new ArgumentException("SFTP password or file path with authentication file is null or empty");
                }

                ForestNET.Lib.Global.ILogConfig("start configuration steps to create a sftp session");

                /* list of authentication methods for sftp */
                List<AuthenticationMethod> a_authenticationMethods = [];

                /* check if parameter is a file path to authentication file */
                if ((this.FilePathAuthentication != null) && (!ForestNET.Lib.Helper.IsStringEmpty(this.FilePathAuthentication)) && (ForestNET.Lib.IO.File.Exists(this.FilePathAuthentication)))
                {
                    ForestNET.Lib.Global.ILogConfig("add authentication file '" + this.FilePathAuthentication + "' as identity");

                    PrivateKeyFile o_privateKey;

                    /* use password to access authentication private key */
                    if (!ForestNET.Lib.Helper.IsStringEmpty(this.Password))
                    {
                        /* create private key instance and use password to access authentication private key */
                        o_privateKey = new(FilePathAuthentication, this.Password);
                    }
                    else
                    { /* private key without password */
                        /* create private key instance */
                        o_privateKey = new(FilePathAuthentication);
                    }

                    /* add authentication file method */
                    a_authenticationMethods.Add(new PrivateKeyAuthenticationMethod(this.User, new[] { o_privateKey }));
                }
                else
                { /* use user and password authentication */
                    a_authenticationMethods.Add(new PasswordAuthenticationMethod(this.User, this.Password));
                }

                /* variable for sftp configuration instance */
                ConnectionInfo o_sftpConfig;

                /* check if we create sftp connection over ssh tunnel */
                if ((this.o_sshClient != null) && (this.o_sshClient.IsConnected))
                {
                    /* create sftp configuration instance with proxy settings over ssh tunnel */
                    if (this.ProxyType != null)
                    {
                        ForestNET.Lib.Global.ILogConfig("create sftp configuration with '" + this.User + "' at 'localhost:" + this.TunnelLocalPort + "' over proxy '" + this.ProxyHost + ":" + this.ProxyPort + "'");

                        o_sftpConfig = new("localhost", this.TunnelLocalPort, this.User, this.ProxyType ?? throw new NullReferenceException("Proxy type is null"), this.ProxyHost, this.ProxyPort, this.ProxyUser, this.ProxyPassword, [.. a_authenticationMethods]);
                    }
                    else
                    { /* create sftp configuration instance without proxy over ssh tunnel */
                        ForestNET.Lib.Global.ILogConfig("create sftp configuration with '" + this.User + "' at 'localhost:" + this.TunnelLocalPort + "'");

                        o_sftpConfig = new("localhost", this.TunnelLocalPort, this.User, [.. a_authenticationMethods]);
                    }
                }
                else
                {
                    /* create sftp configuration instance with proxy settings */
                    if (this.ProxyType != null)
                    {
                        ForestNET.Lib.Global.ILogConfig("create sftp configuration with '" + this.User + "' at '" + this.Host + ":" + this.Port + "' over proxy '" + this.ProxyHost + ":" + this.ProxyPort + "'");

                        o_sftpConfig = new(this.Host, this.Port, this.User, this.ProxyType ?? throw new NullReferenceException("Proxy type is null"), this.ProxyHost, this.ProxyPort, this.ProxyUser, this.ProxyPassword, [.. a_authenticationMethods]);
                    }
                    else
                    { /* create sftp configuration instance without proxy */
                        ForestNET.Lib.Global.ILogConfig("create sftp configuration with '" + this.User + "' at '" + this.Host + ":" + this.Port + "'");

                        o_sftpConfig = new(this.Host, this.Port, this.User, [.. a_authenticationMethods]);
                    }
                }

                ForestNET.Lib.Global.ILogConfig("set encoding '" + this.Encoding + "'");

                /* set encoding */
                o_sftpConfig.Encoding = this.Encoding;

                ForestNET.Lib.Global.ILogConfig("set timeout '" + this.Timeout.TotalMilliseconds + " ms'");

                /* set timeout */
                o_sftpConfig.Timeout = this.Timeout;

                /* create sftp client instance */
                this.o_sftpClient = new(o_sftpConfig);

                ForestNET.Lib.Global.ILogConfig("set buffer size '" + this.BufferSize + "'");

                /* set buffer size */
                this.SftpClient.BufferSize = this.BufferSize;

                if (this.KeepAliveInterval != null)
                {
                    ForestNET.Lib.Global.ILogConfig("set keep alive interval '" + this.KeepAliveInterval + "'");

                    /* set keep alive interval */
                    this.SftpClient.KeepAliveInterval = (TimeSpan)this.KeepAliveInterval;
                }

                ForestNET.Lib.Global.ILogConfig("set operation timeout '" + this.Timeout.TotalMilliseconds + " ms'");

                /* set operation timeout */
                this.SftpClient.OperationTimeout = this.Timeout;

                /* allow any host connection (insecure) */
                if (!this.StrictHostChecking)
                {
                    ForestNET.Lib.Global.ILogConfig("allow any host connection");
                    ForestNET.Lib.Global.LogWarning("ForestNET.Lib.Net.SFTP.Client strict host checking been set to 'false'");
                    ForestNET.Lib.Global.LogWarning("Ignoring unknown server fingerprint with sftp connection, using this is a security vulnerability!");

                    /* trust any host connection */
                    this.SftpClient.HostKeyReceived += (o_sender, o_hostKeyEventArgs) => { o_hostKeyEventArgs.CanTrust = true; };
                }
                else if (!ForestNET.Lib.Helper.IsStringEmpty(this.FilePathKnownHosts))
                { /* use known hosts file */
                    ForestNET.Lib.Global.ILogConfig("use known hosts file '" + this.FilePathKnownHosts + "' for host key check");

                    this.SftpClient.HostKeyReceived += (o_sender, o_hostKeyEventArgs) => this.ConfirmReceivedHostKey(o_sender, o_hostKeyEventArgs);
                }

                ForestNET.Lib.Global.ILogConfig("start connection to sftp host");

                /* start connection to sftp host */
                await this.SftpClient.ConnectAsync(this.Token());
            }
            catch (Exception o_exc)
            {
                /* handle exception when connection failed - logout and throw exception */
                this.Logout();
                throw new System.IO.IOException(
                    "SFTP-Error: cannot connect with '" + this.s_user + "' at '" + this.s_host + ":" + this.i_port + "'" +
                    ((!ForestNET.Lib.Helper.IsStringEmpty(this.TunnelHost)) ? " - using ssh tunnel with '" + this.TunnelUser + "' at '" + this.TunnelHost + ":" + this.TunnelPort + "' over forwarding port '" + this.TunnelLocalPort + "'" : "") +
                    "; " + o_exc
                );
            }

            try
            {
                ForestNET.Lib.Global.ILogConfig("retrieve login result");

                /* retrieve login result */
                this.b_loggedIn = this.SftpClient.IsConnected;

                /* check login result */
                if (!this.b_loggedIn)
                {
                    ForestNET.Lib.Global.ILogWarning("sftp client instance is not connected");

                    throw new System.IO.IOException("issue getting login result");
                }
            }
            catch (Exception o_exc)
            {
                /* handle exception when connection failed - logout and throw exception */
                this.Logout();
                throw new System.IO.IOException("SFTP-Error: cannot connect with '" + this.s_user + "' at '" + this.s_host + ":" + this.i_port + "'; " + o_exc);
            }

            return this.b_loggedIn = true;
        }

        /// <summary>
        /// Confirming received SHA-256 finger print from current sftp connection
        /// </summary>
        /// <param name="p_o_sender">source object of the event</param>
        /// <param name="p_o_hostKeyEventArgs">sftp event arguments with SHA-256 finger print of current connection</param>
        private void ConfirmReceivedHostKey(object? p_o_sender, Renci.SshNet.Common.HostKeyEventArgs p_o_hostKeyEventArgs)
        {
            /* get current sha256 finger print as base64 string */
            string s_currentBase64FingerPrint = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(p_o_hostKeyEventArgs.FingerPrintSHA256));

            /* get host from current instance */
            string s_host = this.Host;

            /* set tunnel host from current instance if our sender object is of type ssh client */
            if ((p_o_sender != null) && ((p_o_sender.ToString() ?? string.Empty).Contains("sshclient", StringComparison.CurrentCultureIgnoreCase)))
            {
                s_host = this.TunnelHost;
            }

            ForestNET.Lib.Global.ILogConfig("Connection with:\t" + s_host + " " + p_o_hostKeyEventArgs.HostKeyName + " " + s_currentBase64FingerPrint);

            /* there is no known hosts file, so we cannot confirm or trust any host connection */
            if (this.FilePathKnownHosts == null)
            {
                p_o_hostKeyEventArgs.CanTrust = false;
            }
            else
            {
                /* first we deny any trust at all */
                p_o_hostKeyEventArgs.CanTrust = false;

                /* try to open known hosts file */
                ForestNET.Lib.IO.File o_knownHostsFile = new(this.FilePathKnownHosts, false);

                /* iterate each line in known hosts file */
                foreach (string s_line in o_knownHostsFile.FileContentAsList ?? [])
                {
                    ForestNET.Lib.Global.ILogConfig("compare current connection with:\t" + s_line);

                    /* check if known hosts line matches with current connection */
                    if (s_line.Equals(s_host + " " + p_o_hostKeyEventArgs.HostKeyName + " " + s_currentBase64FingerPrint))
                    {
                        ForestNET.Lib.Global.ILogConfig("known hosts match found -> confirm host key");

                        /* confirm host key */
                        p_o_hostKeyEventArgs.CanTrust = true;
                        break;
                    }
                }
            }

            /* log warning that we cannot trust connection */
            if (!p_o_hostKeyEventArgs.CanTrust)
            {
                ForestNET.Lib.Global.ILogWarning("cannot trust connection known_hosts file 'null' or entry not found;\t" + s_host + " " + p_o_hostKeyEventArgs.HostKeyName + " " + s_currentBase64FingerPrint);
            }
        }

        /// <summary>
        /// Logout and disconnect sftp connection/session and null client object
        /// </summary>
        public bool Logout()
        {
            this.b_loggedIn = false;

            /* logout and disconnect sftp connection */
            if (this.o_sftpClient != null)
            {
                try
                {
                    try
                    {
                        ForestNET.Lib.Global.ILogConfig("disconnect sftp connection");

                        /* disconnect sftp connection */
                        this.SftpClient.Disconnect();
                    }
                    finally
                    {
                        ForestNET.Lib.Global.ILogConfig("call Cancel method of cancellation token source");

                        /* call Cancel method of cancellation token source */
                        this.o_tokenSource?.Cancel();

                        ForestNET.Lib.Global.ILogConfig("dispose sftp client instance");

                        /* dispose sftp client instance */
                        this.SftpClient.Dispose();
                    }
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.ILogSevere("error with disconnect and dispose sftp instance: " + o_exc);
                }
                finally
                {
                    ForestNET.Lib.Global.ILogConfig("set sftp client instance to null");

                    /* set sftp client instance to null */
                    this.o_sftpClient = null;
                }
            }

            /* logout and disconnect ssh tunnel connection */
            if (this.o_sshClient != null)
            {
                try
                {
                    try
                    {
                        ForestNET.Lib.Global.ILogConfig("disconnect ssh tunnel connection");

                        /* disconnect ssh tunnel connection */
                        this.o_sshClient.Disconnect();
                    }
                    finally
                    {
                        /* if token source not already been canceled */
                        if ((this.o_tokenSource != null) && (!this.o_tokenSource.IsCancellationRequested))
                        {
                            ForestNET.Lib.Global.ILogConfig("call Cancel method of cancellation token source");

                            /* call Cancel method of cancellation token source */
                            this.o_tokenSource?.Cancel();
                        }

                        ForestNET.Lib.Global.ILogConfig("dispose ssh tunnel client instance");

                        /* dispose ssh tunnel client instance */
                        this.o_sshClient.Dispose();
                    }
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.ILogSevere("error with disconnect and dispose ssh tunnel instance: " + o_exc);
                }
                finally
                {
                    ForestNET.Lib.Global.ILogConfig("set ssh client instance to null");

                    /* set ssh tunnel client instance to null */
                    this.o_sshClient = null;
                }
            }

            return (this.o_tokenSource != null) && this.o_tokenSource.IsCancellationRequested;
        }

        /// <summary>
        /// Dispose current sftp client instance, executing logout method
        /// </summary>
        public void Dispose()
        {
            _ = this.Logout();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Creates a directory on sftp server, create all directories until target directory automatically
        /// </summary>
        /// <param name="p_s_dir">directory with path to be created</param>
        /// <exception cref="System.IO.IOException">issue with make directory command</exception>
        public void MkDir(string p_s_directory)
        {
            this.MkDir(p_s_directory, true);
        }

        /// <summary>
        /// Creates a directory on sftp server
        /// </summary>
        /// <param name="p_s_dir">directory with path to be created</param>
        /// <param name="p_b_autoCreate">true - create all directories until target directory automatically, false - expect that the complete path to target directory already exists</param>
        /// <exception cref="System.IO.IOException">issue with make directory command</exception>
        public void MkDir(string p_s_dir, bool p_b_autoCreate)
        {
            /* check directory path is null or empty */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_dir))
            {
                throw new ArgumentException("directory path is null or empty");
            }

            /* replace all backslashes with slash */
            p_s_dir = p_s_dir.Trim().Replace('\\', '/');

            /* directory path must start with root '/' */
            if (p_s_dir.StartsWith("/"))
            {
                p_s_dir = p_s_dir.Substring(1);
            }

            /* remove '/' character if directory parameter ends with it */
            if (p_s_dir.EndsWith("/"))
            {
                p_s_dir = p_s_dir.Substring(0, p_s_dir.Length - 1);
            }

            try
            {
                /* check if directory already exists */
                if (!this.DirectoryExists(p_s_dir))
                {
                    /* create path to target directory if we are not at root directory */
                    if ((p_b_autoCreate) && (p_s_dir.Contains('/')))
                    {
                        ForestNET.Lib.Global.ILogFine("create path to target directory on sftp server");

                        /* get all directories in path as a list */
                        string[] a_foo = p_s_dir.Substring(0, p_s_dir.LastIndexOf('/')).Split("/");
                        List<string> a_createDirectories = [];

                        /* for each directory in path to target directory */
                        for (int i = 0; i < a_foo.Length; i++)
                        {
                            string s_foo = "";

                            for (int j = 0; j < (a_foo.Length - i); j++)
                            {
                                s_foo += a_foo[j] + "/";
                            }

                            /* reduce directory path */
                            s_foo = s_foo.Substring(0, s_foo.Length - 1);

                            /* if a directory already exists we can break the loop here */
                            if (this.DirectoryExists(s_foo))
                            {
                                break;
                            }

                            /* add directory path to list */
                            a_createDirectories.Add(s_foo);
                        }

                        ForestNET.Lib.Global.ILogFine("create each directory within path in reverse");

                        /* create each directory within path in reverse */
                        for (int i = (a_createDirectories.Count - 1); i >= 0; i--)
                        {
                            ForestNET.Lib.Global.ILogFiner("create directory '/" + a_createDirectories[i] + "'");

                            /* create directory within target path */
                            this.SftpClient.CreateDirectory("/" + a_createDirectories[i]);
                        }
                    }

                    ForestNET.Lib.Global.ILogFine("create directory '/" + p_s_dir + "'");

                    /* create directory */
                    this.SftpClient.CreateDirectory("/" + p_s_dir);
                }
            }
            catch (Exception o_exc)
            {
                /* handle sftp exception */
                throw new System.IO.IOException("SFTP error with creating directory", o_exc);
            }
        }

        /// <summary>
        /// List all entries of a target sftp directory, not recursive - stay in target sftp directory
        /// </summary>
        /// <param name="p_s_dir">path to target sftp directory</param>
        /// <param name="p_b_hideDirectories">true - won't list sub directories, false - list sub directories in result</param>
        /// <param name="p_b_showTempFiles">true - list temp files with '.lock' extension at the end, false - won't list temporary files with '.lock' extension</param>
        /// <param name="p_b_showHiddenFiles">true - list temp files starting with '.', false - won't list files starting with '.'</param>
        /// <returns>list of Entry object(s) on target sftp directory</returns>
        /// <exception cref="System.IO.IOException">issue with list directory command</exception>
        public async Task<List<Entry>> Ls(string p_s_dir, bool p_b_hideDirectories, bool p_b_showTempFiles, bool p_b_showHiddenFiles)
        {
            return await this.Ls(p_s_dir, p_b_hideDirectories, p_b_showTempFiles, p_b_showHiddenFiles, false);
        }

        /// <summary>
        /// List all entries of a target sftp directory
        /// </summary>
        /// <param name="p_s_dir">path to target sftp directory</param>
        /// <param name="p_b_hideDirectories">true - won't list sub directories, false - list sub directories in result</param>
        /// <param name="p_b_showTempFiles">true - list temp files with '.lock' extension at the end, false - won't list temporary files with '.lock' extension</param>
        /// <param name="p_b_showHiddenFiles">true - list temp files starting with '.', false - won't list files starting with '.'</param>
        /// <param name="p_b_recursive">true - include all sub directories in result, false - stay in target sftp directory</param>
        /// <returns>list of Entry object(s) on target sftp directory</returns>
        /// <exception cref="System.IO.IOException">issue with list directory command</exception>
        public async Task<List<Entry>> Ls(string p_s_dir, bool p_b_hideDirectories, bool p_b_showTempFiles, bool p_b_showHiddenFiles, bool p_b_recursive)
        {
            List<Entry> a_list = [];

            /* check if directory path is null or empty */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_dir))
            {
                throw new ArgumentException("directory path is null or empty");
            }

            /* replace all backslashes with slash */
            p_s_dir = p_s_dir.Trim().Replace('\\', '/');

            ForestNET.Lib.Global.ILogFine("list all elements with directory path");

            try
            {
                ForestNET.Lib.Global.ILogFine("list all elements with directory path and iterate each directory element");

                /* iterate each directory element */
                await foreach (Renci.SshNet.Sftp.ISftpFile o_dirElement in this.SftpClient.ListDirectoryAsync(p_s_dir, this.Token()))
                {
                    if (
                        (o_dirElement != null) &&
                        (!o_dirElement.Name.Equals(".")) &&
                        (!o_dirElement.Name.Equals("..")) &&
                        (!((o_dirElement.IsDirectory) && (p_b_hideDirectories))) &&
                        (!((o_dirElement.Name.EndsWith(".lock")) && (!p_b_showTempFiles))) &&
                        (!((o_dirElement.Name.StartsWith(".")) && (!p_b_showHiddenFiles)))

                    )
                    {
                        ForestNET.Lib.Global.ILogFiner("create new sftp entry object with all directory element information for: '" + o_dirElement.Name + "'");

                        /* get entry permissions */
                        string s_permissions = "" +
                            (o_dirElement.OwnerCanRead ? "r" : "-") +
                            (o_dirElement.OwnerCanWrite ? "w" : "-") +
                            (o_dirElement.OwnerCanExecute ? "x" : "-") +
                            (o_dirElement.GroupCanRead ? "r" : "-") +
                            (o_dirElement.GroupCanWrite ? "w" : "-") +
                            (o_dirElement.GroupCanExecute ? "x" : "-") +
                            (o_dirElement.OthersCanRead ? "r" : "-") +
                            (o_dirElement.OthersCanWrite ? "w" : "-") +
                            (o_dirElement.OthersCanExecute ? "x" : "-")
                        ;

                        /* create new sftp entry object with all directory element information and add it to return list value */
                        Entry o_ftpEntry = new(
                            o_dirElement.Name,
                            o_dirElement.GroupId.ToString(),
                            o_dirElement.UserId.ToString(),
                            p_s_dir + "/",
                            s_permissions,
                            o_dirElement.Length,
                            o_dirElement.LastWriteTime,
                            o_dirElement.IsDirectory
                        );

                        ForestNET.Lib.Global.ILogFiner("add directory element to return list value");

                        /* add directory element to return list value */
                        a_list.Add(o_ftpEntry);
                    }

                    /* check if we want to list sub directories */
                    if ((p_b_recursive) && (o_dirElement != null) && (!o_dirElement.Name.Equals(".")) && (!o_dirElement.Name.Equals("..")) && (o_dirElement.IsDirectory))
                    {
                        ForestNET.Lib.Global.ILogFiner("get a list of all directory elements in sub directory for: '" + o_dirElement.Name + "'");

                        /* get a list of all directory elements in sub directory */
                        List<Entry> a_recursiveResult = await this.Ls(p_s_dir + "/" + o_dirElement.Name, p_b_hideDirectories, p_b_showTempFiles, p_b_showHiddenFiles, p_b_recursive);

                        ForestNET.Lib.Global.ILogFiner("add directory elements to return list value");

                        /* add directory elements to return list value */
                        foreach (Entry o_recursiveResult in a_recursiveResult)
                        {
                            a_list.Add(o_recursiveResult);
                        }
                    }
                }
            }
            catch (Exception o_exc)
            {
                /* handle sftp exception */
                throw new System.IO.IOException("SFTP error with list directory", o_exc);
            }

            return a_list;
        }

        /// <summary>
        /// Retrieve sftp attributes of an element on sftp server
        /// </summary>
        /// <param name="p_s_path">path to target sftp file or directory</param>
        /// <returns>sftp attributes object or null if it could not be found</returns>
        private Renci.SshNet.Sftp.SftpFileAttributes? GetSFTPAttr(string p_s_path)
        {
            /* return value */
            Renci.SshNet.Sftp.SftpFileAttributes? o_sftpAttrs = null;

            /* check path parameter */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_path))
            {
                /* path parameter is null or empty, so we return null as result */
                return o_sftpAttrs;
            }

            /* replace all backslashes with slash */
            p_s_path = p_s_path.Trim().Replace('\\', '/');

            /* directory path must start with root '/' */
            if (!p_s_path.StartsWith("/"))
            {
                p_s_path = "/" + p_s_path;
            }

            try
            {
                ForestNET.Lib.Global.ILogFinest("retrieve sftp attributes from '" + p_s_path + "'");

                /* retrieve sftp attributes */
                o_sftpAttrs = this.SftpClient.GetAttributes(p_s_path);
            }
            catch (Exception)
            {
                /* could not retrieve sftp attributes, so we return null as result */
                return null;
            }

            return o_sftpAttrs;
        }

        /// <summary>
        /// Check if a file at target file path on sftp server exists
        /// </summary>
        /// <param name="p_s_filePath">path to target file on sftp server</param>
        /// <returns>true - exists, false - does not exist</returns>
        public bool FileExists(string p_s_filePath)
        {
            /* if path parameter ends with '/' we assume it is a directory */
            if (p_s_filePath.EndsWith("/"))
            {
                /* check if directory exists */
                return this.DirectoryExists(p_s_filePath);
            }

            if (this.GetSFTPAttr(p_s_filePath) == null)
            { /* we cannot receive sftp attributes from path, it does not exist */
                return false;
            }
            else
            { /* received sftp attributes, path exists */
                return true;
            }
        }

        /// <summary>
        /// Check if a directory at target dir path on sftp server exists
        /// </summary>
        /// <param name="p_s_directoryPath">path to target directory on sftp server</param>
        /// <returns>true - exists, false - does not exist</returns>
        public bool DirectoryExists(string p_s_directoryPath)
        {
            /* get sftp attributes from directory path */
            Renci.SshNet.Sftp.SftpFileAttributes? o_sftpAttrs = this.GetSFTPAttr(p_s_directoryPath);

            if (o_sftpAttrs == null)
            { /* we cannot receive sftp attributes from path, it does not exist */
                return false;
            }
            else
            { /* received sftp attributes, return directory attribute */
                return o_sftpAttrs.IsDirectory;
            }
        }

        /// <summary>
        /// Get file length of a file on sftp server
        /// </summary>
        /// <param name="p_s_filePath">path to target file on sftp server</param>
        /// <returns>file size as long value</returns>
        public long GetLength(string p_s_filePath)
        {
            /* get sftp attributes from file path */
            Renci.SshNet.Sftp.SftpFileAttributes? o_sftpAttrs = this.GetSFTPAttr(p_s_filePath);

            if ((o_sftpAttrs == null) || (o_sftpAttrs.IsDirectory))
            { /* we cannot receive sftp attributes from path, or path is a directory */
                return -1;
            }
            else
            { /* return size/length attribute */
                return o_sftpAttrs.Size;
            }
        }

        /// <summary>
        /// Deletes a file on sftp server
        /// </summary>
        /// <param name="p_s_filePath">path to target file on sftp server</param>
        /// <exception cref="ArgumentException">invalid path to target file or file does not exist on sftp server</exception>
        /// <exception cref="System.IO.IOException">issue with delete command</exception>
        public async Task Delete(string p_s_filePath)
        {
            await this.Delete(p_s_filePath, false);
        }

        /// <summary>
        /// Deletes a file on sftp server
        /// </summary>
        /// <param name="p_s_filePath">path to target file on sftp server</param>
        /// <param name="p_b_batch">method in internal batch call, no check if file still exists on sftp server</param>
        /// <exception cref="ArgumentException">invalid path to target file or file does not exist on sftp server</exception>
        /// <exception cref="System.IO.IOException">issue with delete command</exception>
        private async Task Delete(string p_s_filePath, bool p_b_batch)
        {
            /* check if file path is null or empty */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_filePath))
            {
                throw new ArgumentException("file path is null or empty");
            }

            /* replace all backslashes with slash */
            p_s_filePath = p_s_filePath.Trim().Replace('\\', '/');

            /* if this is not a batch call, we check if file really exists on sftp server */
            if ((!p_b_batch) && (!this.FileExists(p_s_filePath)))
            {
                throw new ArgumentException("File '" + p_s_filePath + "' does not exist on sftp server");
            }

            try
            {
                ForestNET.Lib.Global.ILogFine("delete file: '" + p_s_filePath + "'");

                /* delete file */
                await this.SftpClient.DeleteFileAsync(p_s_filePath, this.Token());
            }
            catch (Exception o_exc)
            {
                /* handle sftp exception */
                throw new System.IO.IOException("SFTP error with removing file", o_exc);
            }
        }

        /// <summary>
        /// Remove a directory on sftp server with all it's sub directories and elements
        /// </summary>
        /// <param name="p_s_dir">path to target directory on sftp server</param>
        /// <exception cref="ArgumentException">invalid path to target directory or directory does not exist on sftp server</exception>
        /// <exception cref="System.IO.IOException">issue with rmdir command</exception>
        public async Task RmDir(string p_s_dir)
        {
            await this.RmDir(p_s_dir, false);
        }

        /// <summary>
        /// Remove a directory on sftp server with all it's sub directories and elements
        /// </summary>
        /// <param name="p_s_dir">path to target directory on sftp server</param>
        /// <param name="p_b_batch">method in internal batch call, no check if directory really exists on sftp server</param>
        /// <exception cref="ArgumentException">invalid path to target directory or directory does not exist on sftp server</exception>
        /// <exception cref="System.IO.IOException">issue with rmdir command</exception>
        private async Task RmDir(string p_s_dir, bool p_b_batch)
        {
            /* check if directory path is null or empty */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_dir))
            {
                throw new ArgumentException("directory path is null or empty");
            }

            /* replace all backslashes with slash */
            p_s_dir = p_s_dir.Trim().Replace('\\', '/');

            /* if this is not a batch call, we check if directory really exists on sftp server */
            if ((!p_b_batch) && (!this.DirectoryExists(p_s_dir)))
            {
                throw new ArgumentException("directory '" + p_s_dir + "' does not exist on sftp server");
            }

            try
            {
                /* count files of directory and all sub directories for delegate, but only if it is not a batch call */
                if ((this.DelegatePostProgressFolder != null) && (!p_b_batch))
                {
                    ForestNET.Lib.Global.ILogFiner("count files of directory and all sub directories for delegate");

                    this.i_dirFiles = this.Ls(p_s_dir, true, false, true, true).Result.Count;

                    ForestNET.Lib.Global.ILogFiner("elements to be deleted: " + this.i_dirFiles);
                }

                /* iterate each directory element */
                await foreach (Renci.SshNet.Sftp.ISftpFile o_dirElement in this.SftpClient.ListDirectoryAsync(p_s_dir, this.Token()))
                {
                    /* exclude null, '.', '..', and does not end with '.lock' */
                    if ((o_dirElement != null) && (!o_dirElement.Name.Equals(".")) && (!o_dirElement.Name.Equals("..")) && (!o_dirElement.Name.EndsWith(".lock")))
                    {
                        if (o_dirElement.IsDirectory)
                        {
                            ForestNET.Lib.Global.ILogFinest("directory element[" + o_dirElement.Name + "] is a directory, so we call Rmdir recursively as batch call");

                            /* directory element is a directory, so we call RmDir recursively as batch call */
                            await this.RmDir(p_s_dir + "/" + o_dirElement.Name);
                        }
                        else
                        {
                            ForestNET.Lib.Global.ILogFinest("directory element[" + o_dirElement.Name + "] is a file, so we delete the file as batch call");

                            /* directory element is a file, so we delete the file as batch call */
                            await this.Delete(p_s_dir + "/" + o_dirElement.Name, true);

                            /* increase delete counter and call delegate */
                            if (this.DelegatePostProgressFolder != null)
                            {
                                ForestNET.Lib.Global.ILogFinest("increase delete counter and call delegate");

                                this.i_dirSum++;
                                this.DelegatePostProgressFolder.Invoke(this.i_dirSum, this.i_dirFiles);

                                ForestNET.Lib.Global.ILogFiner("deleted: " + this.i_dirSum + "/" + this.i_dirFiles);
                            }
                        }
                    }
                }
            }
            catch (Exception o_exc)
            {
                /* handle sftp exception */
                throw new System.IO.IOException("SFTP error with list directory", o_exc);
            }

            /* iteration finished, we can reset delete counter */
            if ((this.DelegatePostProgressFolder != null) && (!p_b_batch))
            {
                ForestNET.Lib.Global.ILogFiner("iteration finished, we can reset delete counter");

                this.i_dirSum = 0;
                this.i_dirFiles = 0;
            }

            try
            {
                ForestNET.Lib.Global.ILogFine("delete directory: '" + p_s_dir + "'");

                /* delete directory */
                this.SftpClient.DeleteDirectory(p_s_dir);
            }
            catch (Exception o_exc)
            {
                /* handle sftp exception */
                throw new System.IO.IOException("SFTP error with removing directory", o_exc);
            }
        }

        /// <summary>
        /// Rename directory or file on sftp server, do not delete existing files with new name
        /// </summary>
        /// <param name="p_s_old">name of target directory or file on sftp server</param>
        /// <param name="p_s_new">new name for directory or file</param>
        /// <returns>true - rename successful, false - issue with rename command</returns>
        /// <exception cref="ArgumentException">invalid path to target file or directory or file or directory does not exist on sftp server</exception>
        /// <exception cref="System.IO.IOException">issue with rename command</exception>
        public async Task<bool> Rename(string p_s_old, string p_s_new)
        {
            return await this.Rename(p_s_old, p_s_new, false);
        }

        /// <summary>
        /// Rename directory or file on sftp server
        /// </summary>
        /// <param name="p_s_old">name of target directory or file on sftp server</param>
        /// <param name="p_s_new">new name for directory or file</param>
        /// <param name="p_b_overwrite">if a file already exists with new name, delete it</param>
        /// <returns>true - rename successful, false - issue with rename command</returns>
        /// <exception cref="ArgumentException">invalid path to target file or directory or file or directory does not exist on sftp server</exception>
        /// <exception cref="System.IO.IOException">issue with rename command</exception>
        public async Task<bool> Rename(string p_s_old, string p_s_new, bool p_b_overwrite)
        {
            return await this.Rename(p_s_old, p_s_new, p_b_overwrite, false);
        }

        /// <summary>
        /// Rename directory or file on sftp server
        /// </summary>
        /// <param name="p_s_old">name of target directory or file on sftp server</param>
        /// <param name="p_s_new">new name for directory or file</param>
        /// <param name="p_b_overwrite">if a file already exists with new name, delete it</param>
        /// <param name="p_b_batch">method in internal batch call, no check if directory really exists on sftp server</param>
        /// <returns>true - rename successful, false - issue with rename command</returns>
        /// <exception cref="ArgumentException">invalid path to target file or directory or file or directory does not exist on sftp server</exception>
        /// <exception cref="System.IO.IOException">issue with rename command</exception>
        private async Task<bool> Rename(string p_s_old, string p_s_new, bool p_b_overwrite, bool p_b_batch)
        {
            /* check if old and new name are empty or both are equal */
            if ((ForestNET.Lib.Helper.IsStringEmpty(p_s_old)) || (ForestNET.Lib.Helper.IsStringEmpty(p_s_new)) || (p_s_old.Trim().Equals(p_s_new.Trim())))
            {
                throw new ArgumentException("Old or new name are empty or both are equal");
            }

            /* get old element */
            Renci.SshNet.Sftp.SftpFileAttributes? o_oldElement = this.GetSFTPAttr(p_s_old);

            /* get new element */
            Renci.SshNet.Sftp.SftpFileAttributes? o_newElement = this.GetSFTPAttr(p_s_new);

            /* check if directory or file exists */
            if (o_oldElement == null)
            {
                ForestNET.Lib.Global.ILogWarning("Directory or File '" + p_s_old + "' does not exist on sftp server");

                return false;
            }

            /* in case we want to overwrite it anyway we delete file with the new file name */
            if ((!p_b_overwrite) && (o_newElement != null))
            { /* directory or file already exists */
                ForestNET.Lib.Global.ILogWarning("Directory or file '" + p_s_new + "' already exists on sftp server");

                return false;
            }
            else if ((o_newElement != null) && (o_newElement.IsRegularFile))
            { /* file already exists so we delete it */
                ForestNET.Lib.Global.ILogFine("delete file with new name");

                /* delete file with new name */
                try
                {
                    await this.Delete(p_s_new, true);
                }
                catch (Exception)
                {
                    ForestNET.Lib.Global.ILogFine("could not delete file with new name '" + p_s_new + "'");

                    /* could not delete file with new name */
                    return false;
                }

                ForestNET.Lib.Global.ILogFine("deleted file with new name");
            }
            else if ((o_newElement != null) && (o_newElement.IsDirectory))
            { /* directory cannot be overwritten */
                ForestNET.Lib.Global.ILogWarning("Directory '" + p_s_new + "' cannot be overwritten on sftp server. Please delete target directory first");

                return false;
            }

            /* replace all backslashes with slash */
            p_s_old = p_s_old.Trim().Replace('\\', '/');
            p_s_new = p_s_new.Trim().Replace('\\', '/');

            try
            {
                ForestNET.Lib.Global.ILogFine("rename element: '" + p_s_old + "' to '" + p_s_new + "'");

                if (((o_newElement != null) && (o_newElement.IsRegularFile)) || (o_oldElement.IsRegularFile))
                {
                    /* rename file */
                    await this.SftpClient.RenameFileAsync(p_s_old, p_s_new, this.Token());
                }
                else
                {
                    /* rename/move directory */

                    ForestNET.Lib.Global.ILogFine("create target directory '" + p_s_new + "'");

                    /* create target directory */
                    this.MkDir(p_s_new, true);

                    ForestNET.Lib.Global.ILogFine("get list of all elements in source directory '" + p_s_old + "'");

                    /* get list of all elements in source directory */
                    await foreach (Renci.SshNet.Sftp.ISftpFile o_dirElement in this.SftpClient.ListDirectoryAsync(p_s_old, this.Token()))
                    {
                        /* exclude null, '.' and '..' */
                        if ((o_dirElement != null) && (!o_dirElement.Name.Equals(".")) && (!o_dirElement.Name.Equals("..")))
                        {
                            ForestNET.Lib.Global.ILogFiner("move sub-directory or file '" + p_s_new + "/" + o_dirElement.Name + "'");

                            /* move sub-directory or file with recursion */
                            await this.Rename(o_dirElement.FullName, p_s_new + "/" + o_dirElement.Name, true, true);
                        }
                    }

                    /* we must delete the old directory tree */
                    if (!p_b_batch)
                    {
                        await this.RmDir(p_s_old, true);
                    }
                }
            }
            catch (Exception o_exc)
            {
                /* handle sftp exception */
                throw new System.IO.IOException("SFTP error with renaming element", o_exc);
            }

            return true;
        }

        /// <summary>
        /// Download content from a file on the sftp server
        /// </summary>
        /// <param name="p_s_filePathSourceSftp">path to target file on sftp server</param>
        /// <returns>file content - array of bytes</returns>
        /// <exception cref="ArgumentException">invalid path to target file or file size could not be retrieved</exception>
        /// <exception cref="System.IO.IOException">issue reading file stream or get amount of content length from input stream</exception>
        public async Task<byte[]?> Download(string p_s_filePathSourceSftp)
        {
            /* check file path parameter */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_filePathSourceSftp))
            {
                throw new ArgumentException("Please specify a file path for download");
            }

            /* replace all backslashes with slash */
            p_s_filePathSourceSftp = p_s_filePathSourceSftp.Trim().Replace('\\', '/');

            /* retrieve file length on sftp server, at we check if file really exists */
            long l_fileSize = this.GetLength(p_s_filePathSourceSftp);

            if (l_fileSize < 0)
            { /* file does not exist on sftp server */
                throw new ArgumentException("File '" + p_s_filePathSourceSftp + "' does not exist on sftp server");
            }

            byte[]? a_data = null;

            /* retrieve file stream */
            using (Stream o_inputStream = await this.SftpClient.OpenAsync(p_s_filePathSourceSftp, FileMode.Open, FileAccess.Read, this.Token()))
            {
                /* check if stream object is available */
                if (!o_inputStream.CanRead)
                {
                    ForestNET.Lib.Global.ILogWarning("could not create input stream of file '" + p_s_filePathSourceSftp + "'");

                    return null;
                }

                /* initialize console progress bar */
                this.ConsoleProgressBar?.Init("Downloading . . .", "Download finished ['" + p_s_filePathSourceSftp.Substring(p_s_filePathSourceSftp.LastIndexOf("/") + 1) + "']", p_s_filePathSourceSftp.Substring(p_s_filePathSourceSftp.LastIndexOf("/") + 1));

                ForestNET.Lib.Global.ILogFiner("receiving bytes from input stream");

                /* receiving bytes from input stream */
                a_data = await this.ReceiveBytes(o_inputStream, (int)this.BufferSize, (int)l_fileSize);

                ForestNET.Lib.Global.ILogFiner("received bytes from input stream");
            }

            /* get a return value, but no content so we set data to null */
            if ((a_data != null) && (a_data.Length <= 0))
            {
                a_data = null;
            }

            /* close console progress bar */
            this.ConsoleProgressBar?.Close();

            return a_data;
        }

        /// <summary>
        /// Download content from a file on the sftp server to a local file, local file will be overwritten
        /// </summary>
        /// <param name="p_s_filePathSourceSftp">path to target file on sftp server</param>
        /// <param name="p_s_filePathDestinationLocal">path to destination file on local system</param>
        /// <exception cref="ArgumentException">invalid path to target file, invalid path to local system</exception>
        /// <exception cref="System.IO.IOException">destination directory could not be created, issue reading file stream or issue writing to local system</exception>
        public async Task Download(string p_s_filePathSourceSftp, string p_s_filePathDestinationLocal)
        {
            await this.Download(p_s_filePathSourceSftp, p_s_filePathDestinationLocal, true);
        }

        /// <summary>
        /// Download content from a file on the sftp server to a local file
        /// </summary>
        /// <param name="p_s_filePathSourceSftp">path to target file on sftp server</param>
        /// <param name="p_s_filePathDestinationLocal">path to destination file on local system</param>
        /// <param name="p_b_overwrite">true - if local file already exists, delete it, false - if local file already exists do not download and return true</param>
        /// <exception cref="ArgumentException">invalid path to target file, invalid path to local system</exception>
        /// <exception cref="System.IO.IOException">destination directory could not be created, issue reading file stream or issue writing to local system</exception>
        public async Task Download(string p_s_filePathSourceSftp, string p_s_filePathDestinationLocal, bool p_b_overwrite)
        {
            /* download file content to byte array from sftp server */
            byte[]? a_data = await this.Download(p_s_filePathSourceSftp);

            /* download failed */
            if (a_data == null)
            {
                ForestNET.Lib.Global.ILogWarning("download failed");

                return;
            }

            /* check if we must delete old local file */
            if ((ForestNET.Lib.IO.File.Exists(p_s_filePathDestinationLocal)) && (p_b_overwrite))
            {
                ForestNET.Lib.Global.ILogFine("delete old local file");

                /* delete old local file */
                ForestNET.Lib.IO.File.DeleteFile(p_s_filePathDestinationLocal);
            }

            /* create local directory if it not exists */
            if (!ForestNET.Lib.IO.File.IsDirectory(p_s_filePathDestinationLocal.Substring(0, p_s_filePathDestinationLocal.LastIndexOf(ForestNET.Lib.IO.File.DIR))))
            {
                ForestNET.Lib.Global.ILogFine("create local directory if it not exists");

                ForestNET.Lib.IO.File.CreateDirectory(p_s_filePathDestinationLocal.Substring(0, p_s_filePathDestinationLocal.LastIndexOf(ForestNET.Lib.IO.File.DIR)), true);
            }

            ForestNET.Lib.Global.ILogFine("write downloaded bytes to local file '" + p_s_filePathDestinationLocal + "'");

            /* write downloaded bytes to local file */
            await System.IO.File.WriteAllBytesAsync(p_s_filePathDestinationLocal, a_data, this.Token());
        }

        /// <summary>
        /// Read data from input stream object instance
        /// </summary>
        /// <param name="p_o_inputStream">input stream object instance</param>
        /// <param name="p_i_bufferLength">size of buffer which is used reading the input stream</param>
        /// <param name="p_i_amountData">alternative value to inputstream.available()</param>
        /// <returns>input stream content - array of bytes</returns>
        /// <exception cref="System.IO.IOException">issue reading from input stream object instance</exception>
        private async Task<byte[]> ReceiveBytes(Stream p_o_inputStream, int p_i_bufferLength, int p_i_amountData)
        {
            /* try to get amount of data from input stream parameter */
            int i_amountData = (int)p_o_inputStream.Length;

            /* if we could not get amount of data from input stream parameter or it is to low, take value from other parameter */
            if ((i_amountData <= 0) || (i_amountData < p_i_amountData))
            {
                i_amountData = p_i_amountData;
            }

            /* create receiving byte array, buffer and help variables */
            byte[] a_receivedData = new byte[i_amountData];
            int i_receivedDataPointer = 0;
            byte[] a_buffer = new byte[p_i_bufferLength];
            int i_cycles = (int)Math.Ceiling(((double)i_amountData / (double)p_i_bufferLength));
            int i_sum = 0;
            int i_sumProgress = 0;

            ForestNET.Lib.Global.ILogFinest("Iterate '" + i_cycles + "' cycles to receive '" + i_amountData + "' bytes with '" + p_i_bufferLength + "' bytes buffer");

            /* iterate cycles to receive bytes with buffer */
            for (int i = 0; i < i_cycles; i++)
            {
                int i_bytesReadCycle = 0;
                int i_expectedBytes = p_i_bufferLength;
                int i_bytesReading = -1;

                ForestNET.Lib.Global.ILogFinest("p_o_inputStream.read, expecting " + i_expectedBytes + " bytes");

                /* read from input stream until amount of expected bytes has been reached */
                while ((i_expectedBytes > 0) && ((i_bytesReading = await p_o_inputStream.ReadAsync(a_buffer.AsMemory(0, Math.Min(i_expectedBytes, a_buffer.Length)), this.Token())) > 0))
                {
                    ForestNET.Lib.Global.ILogFinest("this.o_inputStream.read, readed " + i_bytesReading + " bytes of expected " + i_expectedBytes + " bytes");

                    i_expectedBytes -= i_bytesReading;

                    if (i_bytesReading < 0)
                    {
                        throw new InvalidOperationException("Could not receive data");
                    }
                    else
                    {
                        /* copy received bytes to return byte array value */
                        for (int j = 0; j < i_bytesReading; j++)
                        {
                            if (i_receivedDataPointer >= a_receivedData.Length)
                            {
                                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("Receive data pointer '" + i_receivedDataPointer + "' >= amount of total bytes '" + a_receivedData.Length + "' received");

                                break;
                            }

                            a_receivedData[i_receivedDataPointer++] = a_buffer[j];
                            i_bytesReadCycle++;
                        }

                        i_sum += i_bytesReading;

                        ForestNET.Lib.Global.ILogFinest("Received data, cycle '" + (i + 1) + "' of '" + i_cycles + "', amount of bytes: " + i_bytesReadCycle);
                    }
                }

                /* update delegate for progress or own console progress bar instance */
                if ((this.DelegatePostProgress != null) || (this.ConsoleProgressBar != null))
                {
                    i_sumProgress += i_bytesReading;

                    this.DelegatePostProgress?.Invoke((double)i_sumProgress / i_amountData);

                    if (this.ConsoleProgressBar is not null) { this.ConsoleProgressBar.Report = (double)i_sumProgress / i_amountData; }
                }
            }

            /* we have read less bytes than we expected */
            if (i_amountData > i_sum)
            {
                ForestNET.Lib.Global.ILogFinest("We have read less bytes than we expected: '" + i_amountData + "' > '" + i_sum + "'");

                /* new return byte array with correct length */
                byte[] a_trimmedReceivedData = new byte[i_sum];

                /* trim byte array data */
                for (int i = 0; i < i_sum; i++)
                {
                    a_trimmedReceivedData[i] = a_receivedData[i];
                }

                ForestNET.Lib.Global.ILogFinest("Created new return byte array with correct length: '" + i_sum + "'");

                return a_trimmedReceivedData;
            }

            return a_receivedData;
        }

        /// <summary>
        /// Upload local file to a file on sftp server, no append
        /// </summary>
        /// <param name="p_s_filePathSourceLocal">path to source file on local system</param>
        /// <param name="p_s_filePathDestinationSftp">path to destination file on sftp server</param>
        /// <returns>true - upload successful, false - upload failed</returns>
        /// <exception cref="ArgumentException">invalid path to local file, or invalid path to destination file</exception>
        /// <exception cref="System.IO.IOException">could not auto create target directory, upload content data, rename destination file, reading local file or delete existing file</exception>
        public async Task Upload(string p_s_filePathSourceLocal, string p_s_filePathDestinationSftp)
        {
            await this.Upload(p_s_filePathSourceLocal, p_s_filePathDestinationSftp, false);
        }

        /// <summary>
        /// Upload local file to a file on sftp server, append mode possible
        /// </summary>
        /// <param name="p_s_filePathSourceLocal">path to source file on local system</param>
        /// <param name="p_s_filePathDestinationSftp">path to destination file on sftp server</param>
        /// <param name="p_b_append">true - append content data to existing file, false - overwrite file</param>
        /// <returns>true - upload successful, false - upload failed</returns>
        /// <exception cref="ArgumentException">invalid path to local file, or invalid path to destination file</exception>
        /// <exception cref="System.IO.IOException">could not auto create target directory, upload content data, rename destination file, reading local file or delete existing file</exception>
        public async Task Upload(string p_s_filePathSourceLocal, string p_s_filePathDestinationSftp, bool p_b_append)
        {
            /* check file path for upload source and if file really exists locally */
            if ((ForestNET.Lib.Helper.IsStringEmpty(p_s_filePathSourceLocal)) || (!ForestNET.Lib.IO.File.Exists(p_s_filePathSourceLocal)))
            {
                throw new ArgumentException("Please specify a valid source data file for upload");
            }

            /* create byte array for upload procedure */
            byte[]? a_data;

            ForestNET.Lib.Global.ILogFine("read all bytes from source file");

            /* read all bytes from source file */
            try
            {
                a_data = await System.IO.File.ReadAllBytesAsync(p_s_filePathSourceLocal, this.Token());
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.ILogWarning("could not convert byte array output stream: " + o_exc);

                a_data = null;
            }

            /* no bytes retrieved from local file, cannot continue with upload */
            if (a_data == null)
            {
                throw new ArgumentException("no bytes retrieved from local file, cannot continue with upload");
            }

            /* start upload procedure */
            await this.Upload(a_data, p_s_filePathDestinationSftp, p_b_append);
        }

        /// <summary>
        /// Upload content data to a file on sftp server, append mode possible
        /// </summary>
        /// <param name="p_a_data">content data as byte array</param>
        /// <param name="p_s_filePathDestinationSftp">path to destination file on sftp server</param>
        /// <param name="p_b_append">true - append content data to existing file, false - overwrite file</param>
        /// <returns>true - upload successful, false - upload failed</returns>
        /// <exception cref="ArgumentException">invalid content data, or invalid path to destination file</exception>
        /// <exception cref="System.IO.IOException">could not auto create target directory, upload content data, rename destination file or delete existing file</exception>
        public async Task Upload(byte[] p_a_data, string p_s_filePathDestinationSftp, bool p_b_append)
        {
            /* byte array parameter is null or has no content */
            if ((p_a_data == null) || (p_a_data.Length == 0))
            {
                throw new ArgumentException("Please specify data for upload");
            }

            /* file path parameter for upload is null or empty */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_filePathDestinationSftp))
            {
                throw new ArgumentException("Please specify a local file path for upload");
            }

            /* replace all backslashes with slash */
            p_s_filePathDestinationSftp = p_s_filePathDestinationSftp.Trim().Replace('\\', '/');

            /* file path for destination on sftp server must start with root '/' */
            if (!p_s_filePathDestinationSftp.StartsWith("/"))
            {
                p_s_filePathDestinationSftp = "/" + p_s_filePathDestinationSftp;
            }

            /* file path for destination on sftp server must not start with '.lock' */
            if (p_s_filePathDestinationSftp.EndsWith(".lock"))
            {
                throw new ArgumentException("Filename for upload must not end with '.lock'");
            }

            /* variable for filename on upload */
            string s_filename;

            /* separate destination path and file name */
            if (p_s_filePathDestinationSftp.LastIndexOf("/") > 0)
            {
                string s_path = p_s_filePathDestinationSftp.Substring(0, p_s_filePathDestinationSftp.LastIndexOf("/"));
                s_filename = p_s_filePathDestinationSftp.Substring(p_s_filePathDestinationSftp.LastIndexOf("/") + 1);

                /* if this is no batch call, check if destination path exists */
                if (!this.DirectoryExists(s_path))
                {
                    ForestNET.Lib.Global.ILogFine("destination path does not exist, so we create it");

                    /* destination path does not exist, so we create it */
                    this.MkDir(s_path);
                }
            }
            else
            { /* we are at root level */
                s_filename = p_s_filePathDestinationSftp.Substring(1);
            }

            bool b_exists = false;

            /* check if destination file on sftp server already exists */
            if (this.FileExists(p_s_filePathDestinationSftp))
            {
                ForestNET.Lib.Global.ILogFine("destination file on sftp server already exists");

                b_exists = true;
            }
            else
            {
                ForestNET.Lib.Global.ILogFine("destination file on sftp server does not exist, set append parameter to 'false'");

                p_b_append = false;
            }

            if (!p_b_append)
            { /* we are uploading a new file to the sftp server */
                /* delete file on sftp server if it already exists */
                if (b_exists)
                {
                    await this.Delete(p_s_filePathDestinationSftp, true);
                }

                ForestNET.Lib.Global.ILogFine("create file '" + p_s_filePathDestinationSftp + "' on sftp server with '.lock' as extension");

                /* create file on sftp server with '.lock' as extension */
                using (Stream o_outputStream = await this.SftpClient.OpenAsync(p_s_filePathDestinationSftp + ".lock", FileMode.CreateNew, FileAccess.ReadWrite, this.Token()))
                {
                    /* check if stream object is available */
                    if (!o_outputStream.CanWrite)
                    {
                        throw new System.IO.IOException("could not create output stream to file '" + p_s_filePathDestinationSftp + ".lock'");
                    }

                    /* initialize console progress bar */
                    this.ConsoleProgressBar?.Init("Uploading . . .", "Upload finished ['" + s_filename + "']", s_filename);

                    /* sending bytes to output stream */
                    await this.SendBytes(o_outputStream, p_a_data, (int)this.BufferSize);
                }

                /* close console progress bar */
                this.ConsoleProgressBar?.Close();

                ForestNET.Lib.Global.ILogFine("remove '.lock' extension");

                /* remove '.lock' extension */
                await this.SftpClient.RenameFileAsync(p_s_filePathDestinationSftp + ".lock", p_s_filePathDestinationSftp, this.Token());
            }
            else
            { /* we are appending data to an existing file on the sftp server */
                ForestNET.Lib.Global.ILogFine("rename existing file with '.lock' extension");

                /* rename existing file with '.lock' extension */
                await this.SftpClient.RenameFileAsync(p_s_filePathDestinationSftp, p_s_filePathDestinationSftp + ".lock", this.Token());

                ForestNET.Lib.Global.ILogFine("append bytes[" + p_a_data.Length + "] to file '" + p_s_filePathDestinationSftp + "' on sftp server with '.lock' as extension");

                /* append bytes to file on sftp server with '.lock' as extension  */
                using (Stream o_outputStream = await this.SftpClient.OpenAsync(p_s_filePathDestinationSftp + ".lock", FileMode.Append, FileAccess.Write, this.Token()))
                {
                    /* check if stream object is available */
                    if (!o_outputStream.CanWrite)
                    {
                        throw new System.IO.IOException("could not create output stream to file '" + p_s_filePathDestinationSftp + ".lock'");
                    }

                    /* initialize console progress bar */
                    this.ConsoleProgressBar?.Init("Uploading . . .", "Upload finished ['" + s_filename + "']", s_filename);

                    /* sending bytes to output stream */
                    await this.SendBytes(o_outputStream, p_a_data, (int)this.BufferSize);
                }

                /* close console progress bar */
                this.ConsoleProgressBar?.Close();

                ForestNET.Lib.Global.ILogFine("remove '.lock' extension");

                /* remove '.lock' extension */
                await this.SftpClient.RenameFileAsync(p_s_filePathDestinationSftp + ".lock", p_s_filePathDestinationSftp, this.Token());
            }
        }

        /// <summary>
        /// Send data to output stream object instance
        /// </summary>
        /// <param name="p_o_outputStream">output stream object instance</param>
        /// <param name="p_a_data">content data which will be uploaded</param>
        /// <param name="p_i_bufferLength">size of buffer which is used send the output stream</param>
        /// <exception cref="System.IO.IOException">issue sending to output stream object instance</exception>
        private async Task SendBytes(Stream p_o_outputStream, byte[] p_a_data, int p_i_bufferLength)
        {
            /* create sending buffer and help variables */
            int i_sendDataPointer = 0;
            byte[] a_buffer = new byte[p_i_bufferLength];
            int i_cycles = (int)Math.Ceiling(((double)p_a_data.Length / (double)p_i_bufferLength));
            int i_sum = 0;

            ForestNET.Lib.Global.ILogFinest("Iterate '" + i_cycles + "' cycles to transport '" + p_a_data.Length + "' bytes with '" + p_i_bufferLength + "' bytes buffer");

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

                /* send to output stream with buffer counter value */
                await p_o_outputStream.WriteAsync(a_buffer.AsMemory(0, i_bytesSend));
                await p_o_outputStream.FlushAsync();

                ForestNET.Lib.Global.ILogFinest("Sended data, cycle '" + (i + 1) + "' of '" + i_cycles + "', amount of bytes: " + i_bytesSend);

                /* update delegate for progress or own console progress bar instance */
                if ((this.DelegatePostProgress != null) || (this.ConsoleProgressBar != null))
                {
                    i_sum += i_bytesSend;

                    this.DelegatePostProgress?.Invoke((double)i_sum / p_a_data.Length);

                    if (this.ConsoleProgressBar != null) { this.ConsoleProgressBar.Report = (double)i_sum / p_a_data.Length; }
                }
            }
        }

        /// <summary>
        /// Download a complete folder from sftp server to local system, not downloading any sub directories and it's files not overwriting files and re-download them
        /// </summary>
        /// <param name="p_s_sourceDirectorySftp">path of source directory on sftp server</param>
        /// <param name="p_s_destinationDirectoryLocal">path to destination directory on local system</param>
        /// <exception cref="ArgumentException">invalid path to source or destination directory</exception>
        /// <exception cref="System.IO.IOException">destination directory could not be created, issue reading file stream or issue writing to local system</exception>
        public async Task DownloadFolder(string p_s_sourceDirectorySftp, string p_s_destinationDirectoryLocal)
        {
            await this.DownloadFolder(p_s_sourceDirectorySftp, p_s_destinationDirectoryLocal, false, false);
        }

        /// <summary>
        /// Download a complete folder from sftp server to local system, not downloading any sub directories and it's files
        /// </summary>
        /// <param name="p_s_sourceDirectorySftp">path of source directory on sftp server</param>
        /// <param name="p_s_destinationDirectoryLocal">path to destination directory on local system</param>
        /// <param name="p_b_overwrite">true - if local file or directory already exists, delete it, false - if local file or directory already exists do not download and return true</param>
        /// <exception cref="ArgumentException">invalid path to source or destination directory</exception>
        /// <exception cref="System.IO.IOException">destination directory could not be created, issue reading file stream or issue writing to local system</exception>
        public async Task DownloadFolder(string p_s_sourceDirectorySftp, string p_s_destinationDirectoryLocal, bool p_b_overwrite)
        {
            await this.DownloadFolder(p_s_sourceDirectorySftp, p_s_destinationDirectoryLocal, p_b_overwrite, false);
        }

        /// <summary>
        /// Download a complete folder from sftp server to local system
        /// </summary>
        /// <param name="p_s_sourceDirectorySftp">path of source directory on sftp server</param>
        /// <param name="p_s_destinationDirectoryLocal">path to destination directory on local system</param>
        /// <param name="p_b_overwrite">true - if local file or directory already exists, delete it, false - if local file or directory already exists do not download and return true</param>
        /// <param name="p_b_recursive">true - include all sub directories and it's files, false - stay in source sftp directory</param>
        /// <exception cref="ArgumentException">invalid path to source or destination directory</exception>
        /// <exception cref="System.IO.IOException">destination directory could not be created, issue reading file stream or issue writing to local system</exception>
        public async Task DownloadFolder(string p_s_sourceDirectorySftp, string p_s_destinationDirectoryLocal, bool p_b_overwrite, bool p_b_recursive)
        {
            await this.DownloadFolder(p_s_sourceDirectorySftp, p_s_destinationDirectoryLocal, p_b_overwrite, p_b_recursive, false);
        }

        /// <summary>
        /// Download a complete folder from sftp server to local system
        /// </summary>
        /// <param name="p_s_sourceDirectorySftp">path of source directory on sftp server</param>
        /// <param name="p_s_destinationDirectoryLocal">path to destination directory on local system</param>
        /// <param name="p_b_overwrite">true - if local file or directory already exists, delete it, false - if local file or directory already exists do not download and return true</param>
        /// <param name="p_b_recursive">true - include all sub directories and it's files, false - stay in source sftp directory</param>
        /// <param name="p_b_batch">method in internal batch call, no check if source directory really exists on sftp server</param>
        /// <exception cref="ArgumentException">invalid path to source or destination directory</exception>
        /// <exception cref="System.IO.IOException">destination directory could not be created, issue reading file stream or issue writing to local system</exception>
        private async Task DownloadFolder(string p_s_sourceDirectorySftp, string p_s_destinationDirectoryLocal, bool p_b_overwrite, bool p_b_recursive, bool p_b_batch)
        {
            /* check source directory parameter for download from sftp server */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_sourceDirectorySftp))
            {
                throw new ArgumentException("Please specify a source directory for download from sftp server, parameter is 'null'");
            }

            /* replace all backslashes with slash */
            p_s_sourceDirectorySftp = p_s_sourceDirectorySftp.Trim().Replace('\\', '/');

            /* check destination directory parameter for download directory on local system */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_destinationDirectoryLocal))
            {
                throw new ArgumentException("Please specify a destination directory for download on local system, parameter is 'null'");
            }

            /* not a batch call, check if directory on sftp server really exists */
            if ((!p_b_batch) && (!this.DirectoryExists(p_s_sourceDirectorySftp)))
            {
                throw new ArgumentException("Please specify a valid source directory[" + p_s_sourceDirectorySftp + "] for download from the sftp server");
            }

            /* if local destination directory does not exist, create it with auto create */
            if (!ForestNET.Lib.IO.File.IsDirectory(p_s_destinationDirectoryLocal))
            {
                ForestNET.Lib.Global.ILogFine("local destination directory does not exist, create it with auto create");

                ForestNET.Lib.IO.File.CreateDirectory(p_s_destinationDirectoryLocal, true);
            }

            try
            {
                /* count files of directory and all sub directories for delegate, but only if it is not a batch call */
                if ((this.DelegatePostProgressFolder != null) && (!p_b_batch))
                {
                    ForestNET.Lib.Global.ILogFiner("count files of directory and all sub directories for delegate");

                    this.i_dirFiles = this.Ls(p_s_sourceDirectorySftp, true, false, true, p_b_recursive).Result.Count;

                    ForestNET.Lib.Global.ILogFiner("elements to be downloaded: " + this.i_dirFiles);
                }

                /* get all directories and files of directory path and iterate each directory element */
                await foreach (Renci.SshNet.Sftp.ISftpFile o_dirElement in this.SftpClient.ListDirectoryAsync(p_s_sourceDirectorySftp, this.Token()))
                {
                    /* directory element must be not null, not a temporary file, not '.' and not '..' */
                    if ((o_dirElement != null) && (!o_dirElement.Name.EndsWith(".lock")) && (!o_dirElement.Name.Equals(".")) && (!o_dirElement.Name.Equals("..")))
                    {
                        if (o_dirElement.IsDirectory)
                        { /* directory element is a directory */
                            /* create local directory with the same name if it does not exist */
                            if (!ForestNET.Lib.IO.File.IsDirectory(p_s_destinationDirectoryLocal))
                            {
                                ForestNET.Lib.Global.ILogFiner("create local directory: " + p_s_destinationDirectoryLocal + ForestNET.Lib.IO.File.DIR + o_dirElement.Name);

                                ForestNET.Lib.IO.File.CreateDirectory(p_s_destinationDirectoryLocal + ForestNET.Lib.IO.File.DIR + o_dirElement.Name);
                            }

                            /* recursive flag set, download all sub directories and its elements with recursive batch call */
                            if (p_b_recursive)
                            {
                                ForestNET.Lib.Global.ILogFiner("download sub directory: " + o_dirElement.Name);

                                await this.DownloadFolder(p_s_sourceDirectorySftp + "/" + o_dirElement.Name, p_s_destinationDirectoryLocal + ForestNET.Lib.IO.File.DIR + o_dirElement.Name, p_b_overwrite, p_b_recursive, true);
                            }
                        }
                        else
                        { /* directory element is a file */
                            /* download file if it does not exist locally, file size does not match, or we want to do a clean download */
                            if ((!ForestNET.Lib.IO.File.Exists(p_s_destinationDirectoryLocal + ForestNET.Lib.IO.File.DIR + o_dirElement.Name)) || (p_b_overwrite))
                            {
                                /* try to download file multiple times if there may be insignificant connection issues */
                                bool b_retry = true;
                                int i_attempts = 1;
                                int i_maxAttempts = 10;

                                do
                                {
                                    await this.Download(p_s_sourceDirectorySftp + "/" + o_dirElement.Name, p_s_destinationDirectoryLocal + ForestNET.Lib.IO.File.DIR + o_dirElement.Name, p_b_overwrite);

                                    if (ForestNET.Lib.IO.File.Exists(p_s_destinationDirectoryLocal + ForestNET.Lib.IO.File.DIR + o_dirElement.Name))
                                    {
                                        ForestNET.Lib.Global.ILogFiner("downloaded file: " + p_s_destinationDirectoryLocal + ForestNET.Lib.IO.File.DIR + o_dirElement.Name);

                                        b_retry = false;
                                    }

                                    if (b_retry)
                                    {
                                        try
                                        {
                                            Thread.Sleep(250);
                                        }
                                        catch (Exception)
                                        {
                                            /* nothing to do */
                                        }
                                    }

                                    if (i_attempts++ >= i_maxAttempts)
                                    {
                                        throw new System.IO.IOException("Downloading file '" + p_s_destinationDirectoryLocal + ForestNET.Lib.IO.File.DIR + o_dirElement.Name + "' failed, after " + i_attempts + " attempts");
                                    }
                                } while (b_retry);
                            }

                            /* increase download counter and call delegate */
                            if (this.DelegatePostProgressFolder != null)
                            {
                                this.i_dirSum++;
                                this.DelegatePostProgressFolder.Invoke(this.i_dirSum, this.i_dirFiles);

                                ForestNET.Lib.Global.ILogFiner("downloaded: " + this.i_dirSum + "/" + this.i_dirFiles);
                            }
                        }
                    }
                }
            }
            catch (Exception o_exc)
            {
                /* handle sftp exception */
                throw new System.IO.IOException("SFTP error with list directory", o_exc);
            }

            /* iteration finished, we can reset download counter */
            if ((this.DelegatePostProgressFolder != null) && (!p_b_batch))
            {
                this.i_dirSum = 0;
                this.i_dirFiles = 0;
            }
        }

        /// <summary>
        /// Upload a complete folder from local system to a sftp server, not uploading any sub directories and it's files
        /// </summary>
        /// <param name="p_s_sourceDirectoryLocal">path to source directory on local system</param>
        /// <param name="p_s_destinationDirectorySftp">path of destination directory on sftp server</param>
        /// <exception cref="ArgumentException">invalid path to source or destination directory</exception>
        /// <exception cref="System.IO.IOException">could not auto create target directory, upload content data, rename destination file, reading local file or delete existing file</exception>
        public async Task UploadFolder(string p_s_sourceDirectoryLocal, string p_s_destinationDirectorySftp)
        {
            await this.UploadFolder(p_s_sourceDirectoryLocal, p_s_destinationDirectorySftp, false);
        }

        /// <summary>
        /// Upload a complete folder from local system to a sftp server
        /// </summary>
        /// <param name="p_s_sourceDirectoryLocal">path to source directory on local system</param>
        /// <param name="p_s_destinationDirectorySftp">path of destination directory on sftp server</param>
        /// <param name="p_b_recursive">true - include all sub directories and it's files, false - stay in source directory on local system</param>
        /// <exception cref="ArgumentException">invalid path to source or destination directory</exception>
        /// <exception cref="System.IO.IOException">could not auto create target directory, upload content data, rename destination file, reading local file or delete existing file</exception>
        public async Task UploadFolder(string p_s_sourceDirectoryLocal, string p_s_destinationDirectorySftp, bool p_b_recursive)
        {
            /* check file path parameter for upload directory on local system */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_sourceDirectoryLocal))
            {
                throw new ArgumentException("Please specify a source directory for upload on the local system");
            }

            /* check file path parameter for upload directory on sftp server */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_destinationDirectorySftp))
            {
                throw new ArgumentException("Please specify a destination directory for upload on the sftp server");
            }

            /* replace all backslashes with slash */
            p_s_destinationDirectorySftp = p_s_destinationDirectorySftp.Trim().Replace('\\', '/');

            /* check if file path parameter on local system really exists */
            if (!ForestNET.Lib.IO.File.IsDirectory(p_s_sourceDirectoryLocal))
            {
                throw new ArgumentException("Please specify a valid source directory for upload, directory '" + p_s_sourceDirectoryLocal + "' does not exist");
            }

            /* create directory on sftp server if it does not exist, MkDir will check that */
            this.MkDir(p_s_destinationDirectorySftp, true);

            /* list all directory elements on local system, optional with all sub directories */
            List<ForestNET.Lib.IO.ListingElement> a_list = ForestNET.Lib.IO.File.ListDirectory(p_s_sourceDirectoryLocal, p_b_recursive);

            /* count all elements which must be uploaded for delegate upload counter */
            if (this.DelegatePostProgressFolder != null)
            {
                ForestNET.Lib.Global.ILogFiner("count all elements which must be uploaded for delegate upload counter");

                this.i_dirFiles = a_list.Count;

                /* do not count directory elements, because these will be created automatically */
                foreach (ForestNET.Lib.IO.ListingElement o_listingElement in a_list)
                {
                    if (o_listingElement.IsDirectory)
                    {
                        this.i_dirFiles--;
                    }
                }


                ForestNET.Lib.Global.ILogFiner("elements to be uploaded: " + this.i_dirFiles);
            }

            /* iterate each directory element */
            foreach (ForestNET.Lib.IO.ListingElement o_listingElement in a_list)
            {
                /* just get directory or file name as temporary variable */
                string s_foo = (o_listingElement.FullName ?? throw new System.IO.IOException("full name is null")).Substring(p_s_sourceDirectoryLocal.Length);

                /* replace all backslashes with slash */
                s_foo = s_foo.Replace('\\', '/');

                ForestNET.Lib.Global.ILogFiner("destination directory: '" + p_s_destinationDirectorySftp + "' and new remote element: '" + s_foo + "'");

                if (o_listingElement.IsDirectory)
                { /* local element is directory */
                    /* create directory on sftp server if it does not exist, MkDir will check that */
                    this.MkDir(p_s_destinationDirectorySftp + "/" + s_foo, true);

                    ForestNET.Lib.Global.ILogFiner("created directory: " + p_s_destinationDirectorySftp + "/" + s_foo);
                }
                else
                { /* local element is file */
                    /* upload file, will be overwritten(delete and re-upload) if it already exists */
                    await this.Upload((o_listingElement.FullName ?? throw new System.IO.IOException("full name is null")), p_s_destinationDirectorySftp + "/" + s_foo, false);

                    ForestNET.Lib.Global.ILogFiner("uploaded file  : " + p_s_destinationDirectorySftp + "/" + s_foo);

                    /* increase upload counter and call delegate */
                    if (this.DelegatePostProgressFolder != null)
                    {
                        this.i_dirSum++;
                        this.DelegatePostProgressFolder.Invoke(this.i_dirSum, this.i_dirFiles);

                        ForestNET.Lib.Global.ILogFiner("uploaded: " + this.i_dirSum + "/" + this.i_dirFiles);
                    }
                }
            }

            /* iteration finished, we can reset upload counter */
            if (this.DelegatePostProgressFolder != null)
            {
                this.i_dirSum = 0;
                this.i_dirFiles = 0;
            }
        }
    }
}