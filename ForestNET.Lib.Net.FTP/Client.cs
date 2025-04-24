using FluentFTP;

namespace ForestNET.Lib.Net.FTP
{
    /// <summary>
    /// Supported proxy types for ftp(s) connection.
    /// </summary>
    public enum ProxyType
    {
        HTTP, SOCKS4A, SOCKS5
    }

    /// <summary>
    /// FTP Client class for a connection to a ftp(s) server. Async wrapper for FluentFTP library.
    /// Methods work like the known commands in most CLI implementations, but will always use the complete path to a file or directory from root '/'.
    /// </summary>
    public class Client : IDisposable
    {

        /* Constants */

        public const int BUFFERSIZE = 8192;

        /* Delegates */

        /// <summary>
        /// interface delegate definition which can be instanced outside of ftp.Client class to post progress anywhere of download/upload methods
        /// </summary>
        public delegate void PostProgress(double p_d_progress);

        /// <summary>
        /// interface delegate definition which can be instanced outside of ftp.Client class to post progress anywhere of amount of files been processed for download/upload
        /// </summary>
        public delegate void PostProgressFolder(int p_i_filesProcessed, int p_i_files);

        /* Fields */

        private AsyncFtpClient? o_asyncFTPClient = null;
        private readonly FtpConfig o_config;
        private FtpProxyProfile? o_proxyProfile = null;
        private CancellationTokenSource? o_tokenSource = null;

        private bool b_useEncryption = false;
        private bool b_useBinary = true;
        private bool b_loggedIn = false;

        private string s_host = string.Empty;
        private int i_port = 0;
        private string s_user = string.Empty;
        private string s_password = string.Empty;

        private ProxyType? e_proxyType = null;

        private int i_dirSum;
        private int i_dirFiles;

        /* Properties */

        private AsyncFtpClient FtpClient
        {
            get
            {
                return this.o_asyncFTPClient ?? throw new NullReferenceException("Async ftp client instance is null");
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
                /* check if ftp(s) host is not empty */
                if (ForestNET.Lib.Helper.IsStringEmpty(value))
                {
                    throw new ArgumentException("FTP host is null or empty");
                }

                /* host address must start with 'ftp://' or 'ftps://' */
                if ((!value.StartsWith("ftp://")) && (!value.StartsWith("ftps://")))
                {
                    throw new ArgumentException("FTP host does not start with 'ftp://' or 'ftps://'");
                }
                else
                {
                    if (value.StartsWith("ftps://"))
                    {
                        value = value.Substring(7);
                        this.b_useEncryption = true;
                    }
                    else
                    {
                        value = value.Substring(6);
                        this.b_useEncryption = false;
                    }
                }

                this.s_host = value;
            }
        }
        public int Port
        {
            get { return this.i_port; }
            set
            {
                /* check valid ftp(s) port number */
                if (value < 1)
                {
                    throw new ArgumentException("FTP port must be at least '1', but was set to '" + value + "'");
                }

                /* check valid ftp(s) port number */
                if (value > 65535)
                {
                    throw new ArgumentException("FTP port must be lower equal '65535', but was set to '" + value + "'");
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
                /* check if ftp(s) user is not empty */
                if (ForestNET.Lib.Helper.IsStringEmpty(value))
                {
                    throw new ArgumentException("FTP user is null or empty");
                }

                this.s_user = value;
            }
        }
        public string Password
        {
            get
            {
                return this.s_password;
            }
            set
            {
                /* check if ftp(s) password is not empty */
                if (ForestNET.Lib.Helper.IsStringEmpty(value))
                {
                    throw new ArgumentException("FTP password is null or empty");
                }

                this.s_password = value;
            }
        }
        public System.Text.Encoding Encoding { get; set; } = System.Text.Encoding.UTF8;
        public bool PreferMLSD { get; set; } = false;
        public bool UseBinary
        {
            get
            {
                return this.b_useBinary;
            }
            set
            {
                this.b_useBinary = value;

                if (this.b_useBinary)
                {
                    this.o_config.DownloadDataType = FtpDataType.Binary;
                    this.o_config.ListingDataType = FtpDataType.Binary;
                    this.o_config.UploadDataType = FtpDataType.Binary;
                }
                else
                {
                    this.o_config.DownloadDataType = FtpDataType.ASCII;
                    this.o_config.ListingDataType = FtpDataType.ASCII;
                    this.o_config.UploadDataType = FtpDataType.ASCII;
                }
            }
        }
        public bool KeepAlive
        {
            get
            {
                return this.o_config.SocketKeepAlive;
            }
            set
            {
                this.o_config.SocketKeepAlive = value;
            }
        }
        public int BufferSize
        {
            get
            {
                return this.o_config.LocalFileBufferSize;
            }
            set
            {
                /* check valid buffer size min. */
                if (value < 1)
                {
                    throw new ArgumentException("FTP buffer size must be at least '1', but was set to '" + value + "'");
                }

                /* check valid buffer size max. */
                if (value > 32768)
                {
                    throw new ArgumentException("FTP buffer size must be lower equal '32768', but was set to '" + value + "'");
                }

                this.o_config.LocalFileBufferSize = value;
            }
        }
        public int Timeout
        {
            get
            {
                return this.o_config.DataConnectionConnectTimeout;
            }
            set
            {
                /* check valid buffer size min. */
                if (value < 0)
                {
                    throw new ArgumentException("FTP timeout (ms) must be at least '0', but was set to '" + value + "'");
                }

                /* check valid buffer size max. */
                if (value > 60000)
                {
                    throw new ArgumentException("FTP timeout (ms) must be lower equal '60000', but was set to '" + value + "'");
                }

                this.o_config.ConnectTimeout = value;
                this.o_config.DataConnectionConnectTimeout = value;
                this.o_config.DataConnectionReadTimeout = value;
                this.o_config.ReadTimeout = value;
            }
        }
        public bool UsePassiveMode
        {
            get
            {
                return this.o_config.DataConnectionType == FtpDataConnectionType.AutoPassive ||
                    this.o_config.DataConnectionType == FtpDataConnectionType.PASV ||
                    this.o_config.DataConnectionType == FtpDataConnectionType.PASVEX ||
                    this.o_config.DataConnectionType == FtpDataConnectionType.PASVUSE;
            }
            set
            {
                if (value)
                {
                    this.o_config.DataConnectionType = FtpDataConnectionType.AutoPassive;
                }
                else
                {
                    this.o_config.DataConnectionType = FtpDataConnectionType.AutoActive;
                }
            }
        }
        public bool UseExplicitEncryptionMode
        {
            get
            {
                return this.o_config.EncryptionMode == FtpEncryptionMode.Explicit;
            }
            set
            {
                if (value)
                {
                    this.o_config.EncryptionMode = FtpEncryptionMode.Explicit;
                }
                else
                {
                    this.o_config.EncryptionMode = FtpEncryptionMode.Implicit;
                }
            }
        }
        private List<System.Security.Cryptography.X509Certificates.X509Certificate2> CertificateAllowList { get; set; } = [];
        public PostProgress? DelegatePostProgress { private get; set; } = null;
        public PostProgressFolder? DelegatePostProgressFolder { private get; set; } = null;
        public string? FtpReplyCode { get; private set; }
        public string? FtpReply { get; private set; }
        public ForestNET.Lib.ConsoleProgressBar? ConsoleProgressBar { private get; set; }

        /* Methods */

        /// <summary>
        /// Constructor for a ftp(s) client object, using uri parameter specifications.
        /// use passive mode, using Client.BUFFERSIZE(8192) for input/output buffer, binary mode and timeout of 15 seconds
        /// </summary>
        /// <param name="p_s_uri">ftp(s) uri, must start with 'ftp://' or 'ftps://' and contain host, port, user and password information</param>
        /// <exception cref="ArgumentException">invalid ftp(s) uri</exception>
        /// <exception cref="System.IO.IOException">issues starting connection to the ftp(s) server</exception>
        public Client(string p_s_uri) :
            this(p_s_uri, true, Client.BUFFERSIZE)
        {

        }

        /// <summary>
        /// Constructor for a ftp(s) client object, using uri parameter specifications.
        /// using Client.BUFFERSIZE(8192) for input/output buffer, binary mode and timeout of 15 seconds
        /// </summary>
        /// <param name="p_s_uri">ftp(s) uri, must start with 'ftp://' or 'ftps://' and contain host, port, user and password information</param>
        /// <param name="p_b_passiveMode">true - using passive mode, false - using active mode</param>
        /// <exception cref="ArgumentException">invalid ftp(s) uri</exception>
        /// <exception cref="System.IO.IOException">issues starting connection to the ftp(s) server</exception>
        public Client(string p_s_uri, bool p_b_passiveMode) :
            this(p_s_uri, p_b_passiveMode, Client.BUFFERSIZE)
        {

        }

        /// <summary>
        /// Constructor for a ftp(s) client object, using uri parameter specifications.
        /// using binary mode and timeout of 15 seconds
        /// </summary>
        /// <param name="p_s_uri">ftp(s) uri, must start with 'ftp://' or 'ftps://' and contain host, port, user and password information</param>
        /// <param name="p_b_passiveMode">true - using passive mode, false - using active mode</param>
        /// <param name="p_i_bufferSize">specify buffer size for sending and receiving data</param>
        /// <exception cref="ArgumentException">invalid ftp(s) uri</exception>
        /// <exception cref="System.IO.IOException">issues starting connection to the ftp(s) server</exception>
        public Client(string p_s_uri, bool p_b_passiveMode, int p_i_bufferSize)
        {
            this.o_config = new();

            /* check if ftp(s) uri is valid */
            if (!ForestNET.Lib.Helper.MatchesRegex(p_s_uri, "(ftps://|ftp://)([a-zA-Z0-9-_]{4,}):([^\\s]{4,})@([a-zA-Z0-9-_\\.]{4,}):([0-9]{2,5})"))
            {
                throw new ArgumentException("Invalid ftp(s) uri. Valid ftp(s) uri would be 'ftp(s)://user:password@example.com:21'");
            }

            ForestNET.Lib.Global.ILogConfig("using uri: " + ForestNET.Lib.Helper.DisguiseSubstring(p_s_uri, "//", "@", '*'));

            /* variable for protocol */
            string s_protocol = "ftp://";

            /* get protocol from uri */
            if (p_s_uri.StartsWith("ftps://"))
            {
                s_protocol = "ftps://";
            }

            /* remove protocol from uri */
            p_s_uri = p_s_uri.Substring(s_protocol.Length);

            /* get user and password value */
            string[] a_uriParts = p_s_uri.Split("@");
            string s_tempUser = a_uriParts[0].Substring(0, a_uriParts[0].IndexOf(":"));
            string s_tempPassword = a_uriParts[0].Substring(a_uriParts[0].IndexOf(":") + 1);

            string s_tempHost = a_uriParts[1];
            int i_tempPort;

            /* remove last '/' character */
            if ((s_tempHost.Contains('/')) && (s_tempHost.Substring(s_tempHost.IndexOf("/")).Equals("/")))
            {
                s_tempHost = s_tempHost.Substring(0, (s_tempHost.Length - 1));
            }

            /* get host-address and port from uri */
            i_tempPort = Convert.ToInt32(s_tempHost.Substring(s_tempHost.IndexOf(":") + 1));
            s_tempHost = s_tempHost.Substring(0, s_tempHost.IndexOf(":"));

            ForestNET.Lib.Global.ILogConfig("retrieved user, password, host-address and port from uri");

            this.Host = s_protocol + s_tempHost;
            this.Port = i_tempPort;
            this.User = s_tempUser;
            this.Password = s_tempPassword;
            this.UsePassiveMode = p_b_passiveMode;
            this.BufferSize = p_i_bufferSize;
            this.UseBinary = true;
            this.Timeout = 15000;
            this.KeepAlive = false;
        }

        /// <summary>
        /// Constructor for a ftp(s) client object, using separate host, port, user and password parameter specifications.
        /// using Client.BUFFERSIZE(8192) for input/output buffer, passive mode, binary mode and timeout of 15 seconds
        /// </summary>
        /// <param name="p_s_host">ftp(s) host value, must start with 'ftp://' or 'ftps://'</param>
        /// <param name="p_i_port">ftp(s) port value</param>
        /// <param name="p_s_user">user value</param>
        /// <param name="p_s_password">user password value</param>
        /// <exception cref="ArgumentException">wrong ftp(s) host value, invalid ftp(s) port number, missing user or password</exception>
        /// <exception cref="System.IO.IOException">issues starting connection to the ftp(s) server</exception>
        public Client(string p_s_host, int p_i_port, string p_s_user, string p_s_password) :
            this(p_s_host, p_i_port, p_s_user, p_s_password, true)
        {

        }

        /// <summary>
        /// Constructor for a ftp(s) client object, using separate host, port, user and password parameter specifications.
        /// using Client.BUFFERSIZE(8192) for input/output buffer, binary mode and timeout of 15 seconds
        /// </summary>
        /// <param name="p_s_host">ftp(s) host value, must start with 'ftp://' or 'ftps://'</param>
        /// <param name="p_i_port">ftp(s) port value</param>
        /// <param name="p_s_user">user value</param>
        /// <param name="p_s_password">user password value</param>
        /// <param name="p_b_passiveMode">true - using passive mode, false - using active mode</param>
        /// <exception cref="ArgumentException">wrong ftp(s) host value, invalid ftp(s) port number, missing user or password</exception>
        /// <exception cref="System.IO.IOException">issues starting connection to the ftp(s) server</exception>
        public Client(string p_s_host, int p_i_port, string p_s_user, string p_s_password, bool p_b_passiveMode) :
            this(p_s_host, p_i_port, p_s_user, p_s_password, p_b_passiveMode, Client.BUFFERSIZE)
        {

        }

        /// <summary>
        /// Constructor for a ftp(s) client object, using separate host, port, user and password parameter specifications.
        /// using binary mode and timeout of 15 seconds
        /// </summary>
        /// <param name="p_s_host">ftp(s) host value, must start with 'ftp://' or 'ftps://'</param>
        /// <param name="p_i_port">ftp(s) port value</param>
        /// <param name="p_s_user">user value</param>
        /// <param name="p_s_password">user password value</param>
        /// <param name="p_b_passiveMode">true - using passive mode, false - using active mode</param>
        /// <param name="p_i_bufferSize">specify buffer size for sending and receiving data</param>
        /// <exception cref="ArgumentException">wrong ftp(s) host value, invalid ftp(s) port number, missing user or password</exception>
        /// <exception cref="System.IO.IOException">issues starting connection to the ftp(s) server</exception>
        public Client(string p_s_host, int p_i_port, string p_s_user, string p_s_password, bool p_b_passiveMode, int p_i_bufferSize)
        {
            this.o_config = new();

            /* check if host parameter is not null or empty */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_host))
            {
                throw new ArgumentException("FTP host is null or empty");
            }

            ForestNET.Lib.Global.ILogConfig("set user, password, host-address and port");

            this.Host = p_s_host;
            this.Port = p_i_port;
            this.User = p_s_user;
            this.Password = p_s_password;
            this.UsePassiveMode = p_b_passiveMode;
            this.BufferSize = p_i_bufferSize;
            this.UseBinary = true;
            this.Timeout = 15000;
            this.KeepAlive = false;
        }

        /// <summary>
        /// Set ftp proxy profile settings for ftp(s) client instance
        /// </summary>
        /// <param name="p_e_proxyType">supported proxy types: HTTP, SOCKS4A, SOCKS5</param>
        /// <param name="p_s_proxyHost">proxy host value</param>
        /// <param name="p_i_proxyPort">proxy port value</param>
        /// <param name="p_b_useDefaultNetworkCredentials">true - using System.Net.CredentialCache.DefaultNetworkCredentials for authentication, false - using user and password value for authentication</param>
        /// <exception cref="ArgumentException">no proxy host value, invalid proxy port number, missing user or password</exception>
        public void SetProxy(ProxyType p_e_proxyType, string p_s_proxyHost, int p_i_proxyPort, bool p_b_useDefaultNetworkCredentials)
        {
            this.SetProxy(p_e_proxyType, p_s_proxyHost, p_i_proxyPort, p_b_useDefaultNetworkCredentials, null, null);
        }

        /// <summary>
        /// Set ftp proxy profile settings for ftp(s) client instance
        /// </summary>
        /// <param name="p_e_proxyType">supported proxy types: HTTP, SOCKS4A, SOCKS5</param>
        /// <param name="p_s_proxyHost">proxy host value</param>
        /// <param name="p_i_proxyPort">proxy port value</param>
        /// <param name="p_s_user">proxy user value</param>
        /// <param name="p_s_password">proxy user password value</param>
        /// <exception cref="ArgumentException">no proxy host value, invalid proxy port number, missing user or password</exception>
        public void SetProxy(ProxyType p_e_proxyType, string p_s_proxyHost, int p_i_proxyPort, string p_s_proxyUser, string p_s_proxyPassword)
        {
            this.SetProxy(p_e_proxyType, p_s_proxyHost, p_i_proxyPort, false, p_s_proxyUser, p_s_proxyPassword);
        }

        /// <summary>
        /// Set ftp proxy profile settings for ftp(s) client instance
        /// </summary>
        /// <param name="p_e_proxyType">supported proxy types: HTTP, SOCKS4A, SOCKS5</param>
        /// <param name="p_s_proxyHost">proxy host value</param>
        /// <param name="p_i_proxyPort">proxy port value</param>
        /// <param name="p_b_useDefaultNetworkCredentials">true - using System.Net.CredentialCache.DefaultNetworkCredentials for authentication, false - using user and password value for authentication</param>
        /// <param name="p_s_user">proxy user value</param>
        /// <param name="p_s_password">proxy user password value</param>
        /// <exception cref="ArgumentException">no proxy host value, invalid proxy port number, missing user or password</exception>
        private void SetProxy(ProxyType? p_e_proxyType, string p_s_proxyHost, int p_i_proxyPort, bool p_b_useDefaultNetworkCredentials, string? p_s_proxyUser, string? p_s_proxyPassword)
        {
            /* if proxy type parameter is null, we set proxy profile to null */
            if (p_e_proxyType == null)
            {
                this.e_proxyType = null;
                this.o_proxyProfile = null;
            }
            else
            {
                /* check if ftp(s) proxy host is not empty */
                if (ForestNET.Lib.Helper.IsStringEmpty(p_s_proxyHost))
                {
                    throw new ArgumentException("FTP proxy host is null or empty");
                }

                /* check valid ftp(s) proxy port number */
                if (p_i_proxyPort < 1)
                {
                    throw new ArgumentException("FTP proxy port must be at least '1', but was set to '" + p_i_proxyPort + "'");
                }

                /* check valid ftp(s) proxy port number */
                if (p_i_proxyPort > 65535)
                {
                    throw new ArgumentException("FTP proxy port must be lower equal '65535', but was set to '" + p_i_proxyPort + "'");
                }

                /* create new proxy profile */
                this.e_proxyType = p_e_proxyType;
                this.o_proxyProfile = new()
                {
                    ProxyHost = p_s_proxyHost,
                    ProxyPort = p_i_proxyPort
                };

                /* using System.Net.CredentialCache.DefaultNetworkCredentials for authentication */
                if (p_b_useDefaultNetworkCredentials)
                {
                    this.o_proxyProfile.ProxyCredentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                }
                else
                { /* using user and password value for authentication */
                    /* check if proxy user is not empty */
                    if (ForestNET.Lib.Helper.IsStringEmpty(p_s_proxyUser))
                    {
                        throw new ArgumentException("FTP proxy user is null or empty");
                    }

                    /* check if proxy password is not empty */
                    if (ForestNET.Lib.Helper.IsStringEmpty(p_s_proxyPassword))
                    {
                        throw new ArgumentException("FTP proxy password is null or empty");
                    }

                    this.o_proxyProfile.ProxyCredentials = new(p_s_proxyUser, p_s_proxyPassword);
                }
            }
        }

        /// <summary>
        /// Add X.509 certificate to ftp(s) client instance allow list, so only ftps connections with these certificates will be allowed
        /// </summary>
        /// <param name="p_o_certificate">X.509 certificate</param>
        public void AddCertificateToAllowList(System.Security.Cryptography.X509Certificates.X509Certificate2 p_o_certificate)
        {
            this.CertificateAllowList.Add(p_o_certificate);
        }

        /// <summary>
        /// Clears certificate allow liste of ftp(s) client instance
        /// </summary>
        public void ClearCertificateAllowList()
        {
            this.CertificateAllowList.Clear();
        }

        /// <summary>
        /// Validate a ftps certificate from current connection, by searching it in our X.509 certificate allow list
        /// </summary>
        /// <param name="p_o_control">instance of the current ftp(s) client</param>
        /// <param name="p_o_eventArgs">ftps event arguments with X.509 certificate of current connection</param>
        /// <exception cref="System.Security.SecurityException">X.509 certificate of current connection was not found in certificate allow list</exception>
        private void ValidateCertificatesFromAllowList(FluentFTP.Client.BaseClient.BaseFtpClient p_o_control, FtpSslValidationEventArgs p_o_eventArgs)
        {
            /* iterate each X.509 certificate in our allow list  */
            foreach (System.Security.Cryptography.X509Certificates.X509Certificate2 o_certificate in this.CertificateAllowList)
            {
                /* check if we have no policy errors and certificate raw data of current connection matching our allow listed certificate */
                if (p_o_eventArgs.PolicyErrors == System.Net.Security.SslPolicyErrors.None || p_o_eventArgs.Certificate.GetRawCertDataString().Equals(o_certificate.GetRawCertDataString()))
                {
                    /* certicate matched, connection validated */
                    p_o_eventArgs.Accept = true;
                }
            }

            /* we did not find a match */
            if (!p_o_eventArgs.Accept)
            {
                /* X.509 certificate of current connection was not found in certificate allow list */
                throw new System.Security.SecurityException($"{p_o_eventArgs.PolicyErrors}{Environment.NewLine}{p_o_eventArgs.Certificate}");
            }
        }

        /// <summary>
        /// Get cancellation token of CancellationTokenSource instance of our current ftp(s) client instance
        /// </summary>
        private CancellationToken Token()
        {
            return (this.o_tokenSource != null) ? this.o_tokenSource.Token : default;
        }

        /// <summary>
        /// Create ftp(s) client object and start connection to ftp(s) server, entering passive/active mode, possible trust manager and/or ssl context will be adapted as well.
        /// set file transfer mode and file type to binary file type.
        /// </summary>
        /// <exception cref="System.IO.IOException">issues starting connection to the ftp(s) server</exception>
        public async Task<bool> Login()
        {
            /* if current instance is logged in, we must re-login */
            if (this.b_loggedIn)
            {
                ForestNET.Lib.Global.ILogConfig("current instance is logged in, we must re-login so logout procedure will be called");

                if (!await this.Logout())
                {
                    throw new System.IO.IOException("FTP-Error within Logout method");
                }
            }

            if (this.e_proxyType == null)
            {
                ForestNET.Lib.Global.ILogConfig("create async ftp client instance");
                this.o_asyncFTPClient = new AsyncFtpClient();
            }
            else if (this.e_proxyType == ProxyType.HTTP)
            {
                ForestNET.Lib.Global.ILogConfig("create async ftp client instance with http proxy");
                this.o_asyncFTPClient = new FluentFTP.Proxy.AsyncProxy.AsyncFtpClientHttp11Proxy(this.o_proxyProfile);
            }
            else if (this.e_proxyType == ProxyType.SOCKS4A)
            {
                ForestNET.Lib.Global.ILogConfig("create async ftp client instance with SOCKS4A proxy");
                this.o_asyncFTPClient = new FluentFTP.Proxy.AsyncProxy.AsyncFtpClientSocks4aProxy(this.o_proxyProfile);
            }
            else if (this.e_proxyType == ProxyType.SOCKS5)
            {
                ForestNET.Lib.Global.ILogConfig("create async ftp client instance with SOCKS5 proxy");
                this.o_asyncFTPClient = new FluentFTP.Proxy.AsyncProxy.AsyncFtpClientSocks5Proxy(this.o_proxyProfile);
            }

            try
            {
                /* check if we use encryption */
                if (this.b_useEncryption)
                {
                    /* only allow TLS 1.3 */
                    this.o_config.SslProtocols = System.Security.Authentication.SslProtocols.Tls13; // | System.Security.Authentication.SslProtocols.Tls | System.Security.Authentication.SslProtocols.Tls11 | System.Security.Authentication.SslProtocols.Tls12;
                    /* set data connection encryption to true */
                    this.o_config.DataConnectionEncryption = true;

                    ForestNET.Lib.Global.ILogConfig("use encryption with mode '" + this.o_config.EncryptionMode + "' and ssl protocols '" + this.o_config.SslProtocols + "'");

                    /* add all certificates from allow list */
                    foreach (System.Security.Cryptography.X509Certificates.X509Certificate2 o_certificate in this.CertificateAllowList)
                    {
                        this.o_config.ClientCertificates.Add(o_certificate);
                    }
                }

                ForestNET.Lib.Global.ILogConfig("set configuration settings for async ftp client");
                this.FtpClient.Config = this.o_config;

                /* set ftp client host, port, credentials and encoding */
                this.FtpClient.Host = this.Host;
                this.FtpClient.Port = this.Port;
                this.FtpClient.Credentials = new System.Net.NetworkCredential(this.User, this.Password);
                this.FtpClient.Encoding = this.Encoding;

                ForestNET.Lib.Global.ILogConfig("set ftp client host[" + this.FtpClient.Host + "], port[" + this.FtpClient.Port + "], with user[" + this.FtpClient.Credentials.UserName + "] and encoding[" + this.FtpClient.Encoding + "]");

                /* check if we use encryption */
                if (this.b_useEncryption)
                {
                    /* add method to validate certificates for ftps connection */
                    this.FtpClient.ValidateCertificate += new FtpSslValidation(ValidateCertificatesFromAllowList);
                }

                ForestNET.Lib.Global.ILogConfig("renew cancellation token source");

                /* renew cancellation token source */
                this.o_tokenSource = new();

                ForestNET.Lib.Global.ILogConfig("establish connection");

                /* establish connection */
                await this.FtpClient.Connect(this.Token());

                ForestNET.Lib.Global.ILogConfig("retrieve login result");

                /* get ftp(s) reply code and reply message */
                this.ReadLastReply(null);

                /* retrieve login result */
                this.b_loggedIn = await this.FtpClient.IsStillConnected(10000, this.Token());

                /* login result is not successful */
                if (!this.b_loggedIn)
                {
                    throw new System.IO.IOException("FTP-Error: " + this.FtpReply);
                }

                await this.FtpClient.SetWorkingDirectory("/", this.Token());

                if (this.FtpClient.LastReply.Code != "250")
                {
                    throw new System.IO.IOException("FTP-Error with path [/]: " + this.FtpReply);
                }
            }
            catch (System.IO.IOException)
            {
                /* exception occurred, so we disconnect the connection and clean up variables */
                await this.FtpClient.Disconnect(this.Token());
                this.b_loggedIn = false;
                this.o_asyncFTPClient = null;
                throw;
            }

            return this.b_loggedIn = true;
        }

        /// <summary>
        /// Logout and disconnect all ftp(s) connections and null client objects
        /// </summary>
        public async Task<bool> Logout()
        {
            this.b_loggedIn = false;

            /* logout and disconnect ftp connection */
            if (this.o_asyncFTPClient != null)
            {
                try
                {
                    try
                    {
                        ForestNET.Lib.Global.ILogConfig("disconnect ftp connection");

                        /* disconnect ftp connection */
                        await this.FtpClient.Disconnect(this.Token());

                        /* get ftp(s) reply code and reply message */
                        this.ReadLastReply(null);
                    }
                    finally
                    {
                        ForestNET.Lib.Global.ILogConfig("call Cancel method of cancellation token source");

                        /* call Cancel method of cancellation token source */
                        this.o_tokenSource?.Cancel();

                        ForestNET.Lib.Global.ILogConfig("dispose ftp client instance");

                        /* dispose ftp client instance */
                        this.FtpClient.Dispose();
                    }
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.ILogSevere("error with disconnect and dispose ftp instance: " + o_exc);
                }
                finally
                {
                    ForestNET.Lib.Global.ILogConfig("set ftp client instance to null");

                    /* set ftp client instance to null */
                    this.o_asyncFTPClient = null;
                }
            }

            return (this.o_tokenSource != null) && this.o_tokenSource.IsCancellationRequested;
        }

        /// <summary>
        /// Dispose current ftp(s) client instance, executing logout method
        /// </summary>
        public void Dispose()
        {
            _ = this.Logout().Result;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// get ftp(s) reply code and reply message and update class properties and add to internal log with FINE level
        /// </summary>
        private void ReadLastReply(FtpReply? p_o_manualReply)
        {
            FtpReply o_reply;

            /* get ftp(s) reply code and reply message */
            if (p_o_manualReply != null)
            {
                o_reply = (FtpReply)p_o_manualReply;
            }
            else
            {
                o_reply = this.FtpClient.LastReply;
            }

            /* update class properties */
            this.FtpReply = "Error: " + o_reply.ErrorMessage + "|Message: " + o_reply.Message + "|Info: " + string.Join("; ", o_reply.InfoMessages);
            this.FtpReplyCode = o_reply.Code;

            /* add to internal log with FINE level */
            ForestNET.Lib.Global.ILogFine("reply code and message: " + this.FtpReplyCode + " - " + this.FtpReply);
        }

        /// <summary>
        /// Creates a directory on ftp(s) server
        /// </summary>
        /// <param name="p_s_dir">directory with path to be created</param>
        /// <returns>true - directory has been created, false - exception occurred</returns>
        /// <exception cref="System.IO.IOException">issue with make directory command</exception>
        public async Task<bool> MkDir(string p_s_dir)
        {
            return await this.MkDir(p_s_dir, true);
        }

        /// <summary>
        /// Creates a directory on ftp(s) server
        /// </summary>
        /// <param name="p_s_dir">directory with path to be created</param>
        /// <param name="p_b_autoCreate">true - create all directories until target directory automatically, false - expect that the complete path to target directory already exists</param>
        /// <returns>true - directory has been created, false - exception occurred</returns>
        /// <exception cref="System.IO.IOException">issue with make directory command</exception>
        public async Task<bool> MkDir(string p_s_dir, bool p_b_autoCreate)
        {
            /* check directory path is null or empty */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_dir))
            {
                ForestNET.Lib.Global.ILogFine("directory path is null or empty");

                return false;
            }

            /* replace all backslashes with slash */
            p_s_dir = p_s_dir.Trim().Replace('\\', '/');

            /* directory path must start with root '/' */
            if (!p_s_dir.StartsWith("/"))
            {
                p_s_dir = "/" + p_s_dir;
            }

            /* remove '/' character if directory parameter ends with it */
            if (p_s_dir.EndsWith("/"))
            {
                p_s_dir = p_s_dir.Substring(0, p_s_dir.Length - 1);
            }

            bool b_foo = false;

            try
            {
                /* check if directory already exists */
                if (!await this.DirectoryExists(p_s_dir))
                {
                    /* create path to target directory if we are not at root directory */
                    if ((p_b_autoCreate) && (p_s_dir.Contains('/')))
                    {
                        ForestNET.Lib.Global.ILogFine("create path to target directory on ftp(s) server");

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
                            if (await this.DirectoryExists(s_foo))
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
                            b_foo = await this.FtpClient.CreateDirectory("/" + a_createDirectories[i], this.Token());
                        }
                    }

                    ForestNET.Lib.Global.ILogFine("create directory '/" + p_s_dir + "'");

                    /* create directory */
                    b_foo = await this.FtpClient.CreateDirectory("/" + p_s_dir, this.Token());
                }
            }
            catch (Exception o_exc)
            {
                /* handle exception */
                ForestNET.Lib.Global.ILogSevere("FTP(S) error with creating directory; " + o_exc);

                b_foo = false;
            }

            /* get ftp(s) reply code and reply message */
            this.ReadLastReply(null);

            /* return result of last make directory command */
            return b_foo;
        }

        /// <summary>
        /// List all entries of a target ftp(s) directory, not listing any sub directories. Using LIST.
        /// </summary>
        /// <param name="p_s_dir">path to target ftp(s) directory</param>
        /// <param name="p_b_hideDirectories">true - won't list sub directories, false - list sub directories in result</param>
        /// <param name="p_b_showTempFiles">true - list temp files with '.lock' extension at the end, false - won't list temporary files with '.lock' extension</param>
        /// <returns>list of Entry object(s) on target ftp(s) directory</returns>
        public async Task<List<Entry>> Ls(string p_s_dir, bool p_b_hideDirectories, bool p_b_showTempFiles)
        {
            return await this.Ls(p_s_dir, p_b_hideDirectories, p_b_showTempFiles, this.PreferMLSD);
        }

        /// <summary>
        /// List all entries of a target ftp(s) directory, not listing any sub directories.
        /// </summary>
        /// <param name="p_s_dir">path to target ftp(s) directory</param>
        /// <param name="p_b_hideDirectories">true - won't list sub directories, false - list sub directories in result</param>
        /// <param name="p_b_showTempFiles">true - list temp files with '.lock' extension at the end, false - won't list temporary files with '.lock' extension</param>
        /// <param name="p_b_useMLSD">true - list directory with MLSD, fasle - list directory with LIST</param>
        /// <returns>list of Entry object(s) on target ftp(s) directory</returns>
        public async Task<List<Entry>> Ls(string p_s_dir, bool p_b_hideDirectories, bool p_b_showTempFiles, bool p_b_useMLSD)
        {
            return await this.Ls(p_s_dir, p_b_hideDirectories, p_b_showTempFiles, p_b_useMLSD, false);
        }

        /// <summary>
        /// List all entries of a target ftp(s) directory.
        /// </summary>
        /// <param name="p_s_dir">path to target ftp(s) directory</param>
        /// <param name="p_b_hideDirectories">true - won't list sub directories, false - list sub directories in result</param>
        /// <param name="p_b_showTempFiles">true - list temp files with '.lock' extension at the end, false - won't list temporary files with '.lock' extension</param>
        /// <param name="p_b_useMLSD">true - list directory with MLSD, fasle - list directory with LIST</param>
        /// <param name="p_b_recursive">true - include all sub directories in result, false - stay in target ftp(s) directory</param>
        /// <returns>list of Entry object(s) on target ftp(s) directory</returns>
        public async Task<List<Entry>> Ls(string p_s_dir, bool p_b_hideDirectories, bool p_b_showTempFiles, bool p_b_useMLSD, bool p_b_recursive)
        {
            List<Entry> a_list = [];

            /* check if directory path is null or empty */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_dir))
            {
                ForestNET.Lib.Global.ILogFiner("directory path is null or empty");

                return a_list;
            }

            /* replace all backslashes with slash */
            p_s_dir = p_s_dir.Trim().Replace('\\', '/');

            ForestNET.Lib.Global.ILogFiner("get all directories and files of directory path");

            /* get all directories and files of directory path */
            FtpListItem[] a_elements = await this.ListFTPFiles(p_s_dir, p_b_useMLSD);

            if (a_elements.Length > 0)
            {
                /* iterate each directory element */
                foreach (FtpListItem o_dirElement in a_elements)
                {
                    /* check with parameters if we want to list directories and temporary files with '.lock' ending */
                    if (
                        (o_dirElement != null) &&
                        (!o_dirElement.Name.Equals(".")) &&
                        (!o_dirElement.Name.Equals("..")) &&
                        (!((o_dirElement.Type == FtpObjectType.Directory) && (p_b_hideDirectories))) &&
                        (!((o_dirElement.Name.EndsWith(".lock")) && (!p_b_showTempFiles)))
                    )
                    {
                        ForestNET.Lib.Global.ILogFinest("create new ftp(s) entry object with all directory element information for: '" + o_dirElement.Name + "'");

                        /* create new ftp(s) entry object with all directory element information and add it to return list value */
                        Entry o_ftpEntry = new(o_dirElement.Name, o_dirElement.RawGroup, o_dirElement.RawOwner, ((p_s_dir == "/") ? "" : p_s_dir) + "/", o_dirElement.RawPermissions, o_dirElement.Size, o_dirElement.Modified, o_dirElement.Type == FtpObjectType.Directory);

                        ForestNET.Lib.Global.ILogFinest("add directory element to return list value");

                        /* add directory element to return list value */
                        a_list.Add(o_ftpEntry);
                    }

                    /* check if we want to list sub directories */
                    if ((p_b_recursive) && (o_dirElement != null) && (!o_dirElement.Name.Equals(".")) && (!o_dirElement.Name.Equals("..")) && (o_dirElement.Type == FtpObjectType.Directory))
                    {
                        ForestNET.Lib.Global.ILogFinest("get a list of all directory elements in sub directory for: '" + o_dirElement.Name + "'");

                        /* get a list of all directory elements in sub directory */
                        List<Entry> a_recursiveResult = await this.Ls(((p_s_dir == "/") ? "" : p_s_dir) + "/" + o_dirElement.Name, p_b_hideDirectories, p_b_showTempFiles, p_b_useMLSD, p_b_recursive);

                        ForestNET.Lib.Global.ILogFinest("add directory elements to return list value");

                        /* add directory elements to return list value */
                        foreach (Entry o_recursiveResult in a_recursiveResult)
                        {
                            a_list.Add(o_recursiveResult);
                        }
                    }
                }
            }
            else
            {
                ForestNET.Lib.Global.ILogFiner("directory path has no elements");
            }

            return a_list;
        }

        /// <summary>
        /// Using MLSD or LIST command to get a list of all entries on target ftp(s) directory, depending on parameter p_b_useMLSD
        /// </summary>
        /// <param name="p_s_dir">path to target ftp(s) directory</param>
        /// <param name="p_b_useMLSD">true - list directory with MLSD, fasle - list directory with LIST</param>
        /// <returns>array of FtpListItem[] objects</returns>
        private async Task<FtpListItem[]> ListFTPFiles(string p_s_dir, bool p_b_useMLSD)
        {
            FtpListItem[] a_elements = [];

            try
            {
                ForestNET.Lib.Global.ILogFiner((p_b_useMLSD) ? "execute MLSD with directory path" : "execute LIST with directory path");

                /* get list of directory elements */
                a_elements = await this.FtpClient.GetListing(p_s_dir, (p_b_useMLSD) ? FtpListOption.Auto : FtpListOption.ForceList, this.Token());

                /* get ftp(s) reply code and reply message */
                this.ReadLastReply(null);
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.LogException(o_exc);
            }

            /* return list of directory elements */
            return a_elements;
        }

        /// <summary>
        /// Get element instance within a ftp(s) directory if name matches and it is not a temporary file
        /// </summary>
        /// <param name="p_s_path">path to target element on ftp(s) server</param>
        /// <returns>FtpListItem instance with further information properties or null value</returns>
        private async Task<FtpListItem?> GetElement(string p_s_path)
        {
            return await this.GetElement(p_s_path, this.PreferMLSD);
        }

        /// <summary>
        /// Get element instance within a ftp(s) directory if name matches and it is not a temporary file
        /// </summary>
        /// <param name="p_s_path">path to target element on ftp(s) server</param>
        /// <param name="p_b_useMLSD">true - list directory with MLSD, fasle - list directory with LIST</param>
        /// <returns>FtpListItem instance with further information properties or null value</returns>
        private async Task<FtpListItem?> GetElement(string p_s_path, bool p_b_useMLSD)
        {
            /* check if file path is null or empty */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_path))
            {
                ForestNET.Lib.Global.ILogFiner("path is null or empty");

                return null;
            }

            /* replace all backslashes with slash */
            p_s_path = p_s_path.Trim().Replace('\\', '/');

            /* path must start with root '/' */
            if (!p_s_path.StartsWith("/"))
            {
                p_s_path = "/" + p_s_path;
            }

            /* remove '/' character if path parameter ends with it */
            if (p_s_path.EndsWith("/"))
            {
                p_s_path = p_s_path.Substring(0, p_s_path.Length - 1);
            }

            /* get path and element from parameter */
            string s_path = p_s_path.Substring(0, p_s_path.LastIndexOf("/"));
            string s_element = p_s_path.Substring(p_s_path.LastIndexOf("/") + 1);

            /* get all directories and files of file path */
            FtpListItem[] a_elements = await this.ListFTPFiles(s_path, p_b_useMLSD);

            if (a_elements.Length > 0)
            {
                /* check each directory element to find what we are looking for */
                foreach (FtpListItem o_dirElement in a_elements)
                {
                    ForestNET.Lib.Global.ILogFinest(o_dirElement.Name + " == " + s_element);

                    /* element name must match */
                    if (
                        (o_dirElement != null) &&
                        (!o_dirElement.Name.Equals(".")) &&
                        (!o_dirElement.Name.Equals("..")) &&
                        (o_dirElement.Name.Equals(s_element))
                    )
                    {
                        ForestNET.Lib.Global.ILogFinest("found element");

                        /* found element */
                        return o_dirElement;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Check if a file at target file path on ftp(s) server exists.
        /// </summary>
        /// <param name="p_s_filePath">path to target file on ftp(s) server</param>
        /// <returns>true - exists, false - does not exist</returns>
        public async Task<bool> FileExists(string p_s_filePath)
        {
            FtpListItem? o_foo = await this.GetElement(p_s_filePath);

            if ((o_foo != null) && (o_foo.Type == FtpObjectType.File))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if a directory at target dir path on ftp(s) server exists.
        /// </summary>
        /// <param name="p_s_directoryPath">path to target directory on ftp(s) server</param>
        /// <returns>true - exists, false - does not exist</returns>
        public async Task<bool> DirectoryExists(string p_s_directoryPath)
        {
            FtpListItem? o_foo = await this.GetElement(p_s_directoryPath);

            if ((o_foo != null) && (o_foo.Type == FtpObjectType.Directory))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get file length of a file on ftp(s) server.
        /// </summary>
        /// <param name="p_s_filePath">path to target file on ftp(s) server</param>
        /// <returns>file size as long value</returns>
        public async Task<long> GetLength(string p_s_filePath)
        {
            ForestNET.Lib.Global.ILogFiner("check if file exists to get file length");

            /* check if file exists to get file length */
            FtpListItem? o_foo = await this.GetElement(p_s_filePath);

            if ((o_foo != null) && (o_foo.Type == FtpObjectType.File))
            {
                return o_foo.Size;
            }
            else
            {
                ForestNET.Lib.Global.ILogFiner("file does not exist, returning -1");

                return -1;
            }
        }

        /// <summary>
        /// Deletes a file on ftp(s) server.
        /// </summary>
        /// <param name="p_s_filePath">path to target file on ftp(s) server</param>
        /// <returns>true - deleted, false - issue with deletion</returns>
        public async Task<bool> Delete(string p_s_filePath)
        {
            return await this.Delete(p_s_filePath, false);
        }

        /// <summary>
        /// Deletes a file on ftp(s) server.
        /// </summary>
        /// <param name="p_s_filePath">path to target file on ftp(s) server</param>
        /// <param name="p_b_batch">method in internal batch call, no check if file still exists on ftp(s) server</param>
        /// <returns>true - deleted, false - issue with deletion</returns>
        private async Task<bool> Delete(string p_s_filePath, bool p_b_batch)
        {
            /* check if file path is null or empty */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_filePath))
            {
                ForestNET.Lib.Global.ILogFine("file path is null or empty");

                return false;
            }

            /* replace all backslashes with slash */
            p_s_filePath = p_s_filePath.Trim().Replace('\\', '/');

            /* if this is not a batch call, we check if file really exists on ftp(s) server */
            if ((!p_b_batch) && (!await this.FileExists(p_s_filePath)))
            {
                ForestNET.Lib.Global.ILogWarning("File '" + p_s_filePath + "' does not exist on ftp(s) server");

                return false;
            }

            bool b_foo = false;

            /* delete file */
            try
            {
                ForestNET.Lib.Global.ILogFine("delete file: '" + p_s_filePath + "'");

                await this.FtpClient.DeleteFile(p_s_filePath, this.Token());
                b_foo = true;
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.ILogSevere("could not delete file: " + o_exc);
            }

            /* get ftp(s) reply code and reply message */
            this.ReadLastReply(null);

            return b_foo;
        }

        /// <summary>
        /// Remove a directory on ftp(s) server with all it's sub directories and elements.
        /// </summary>
        /// <param name="p_s_dir">path to target directory on ftp(s) server</param>
        /// <returns>true - deleted, false - issue with deletion</returns>
        public async Task<bool> RmDir(string p_s_dir)
        {
            return await this.RmDir(p_s_dir, false);
        }

        /// <summary>
        /// Remove a directory on ftp(s) server with all it's sub directories and elements.
        /// </summary>
        /// <param name="p_s_dir">path to target directory on ftp(s) server</param>
        /// <param name="p_b_batch">method in internal batch call, no check if directory really exists on ftp(s) server</param>
        /// <returns>true - deleted, false - issue with deletion</returns>
        private async Task<bool> RmDir(string p_s_dir, bool p_b_batch)
        {
            /* check if directory path is null or empty */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_dir))
            {
                ForestNET.Lib.Global.ILogFine("directory path is null or empty");

                return false;
            }

            /* replace all backslashes with slash */
            p_s_dir = p_s_dir.Trim().Replace('\\', '/');

            /* if this is not a batch call, we check if directory really exists on ftp(s) server */
            if ((!p_b_batch) && (!await this.DirectoryExists(p_s_dir)))
            {
                ForestNET.Lib.Global.ILogWarning("Directory '" + p_s_dir + "' does not exist on ftp(s) server");

                return false;
            }

            /* get all directories and files of directory path */
            FtpListItem[] a_elements = await this.ListFTPFiles(p_s_dir, this.PreferMLSD);

            bool b_return = true;

            /* if directory path has elements */
            if (a_elements.Length > 0)
            {
                /* count files of directory and all sub directories for delegate, but only if it is not a batch call */
                if ((this.DelegatePostProgressFolder != null) && (!p_b_batch))
                {
                    ForestNET.Lib.Global.ILogFiner("count files of directory and all sub directories for delegate");

                    this.i_dirFiles = (await this.Ls(p_s_dir, true, false, this.PreferMLSD, true)).Count;

                    ForestNET.Lib.Global.ILogFiner("elements to be deleted: " + this.i_dirFiles);
                }

                ForestNET.Lib.Global.ILogFine("iterate each directory element");

                /* iterate each directory element */
                foreach (FtpListItem o_dirElement in a_elements)
                {
                    if ((o_dirElement != null) && (!o_dirElement.Name.Equals(".")) && (!o_dirElement.Name.Equals("..")))
                    {
                        if (o_dirElement.Type == FtpObjectType.Directory)
                        {
                            ForestNET.Lib.Global.ILogFinest("directory element[" + o_dirElement.Name + "] is a directory, so we call Rmdir recursively as batch call");

                            /* directory element is a directory, so we call RmDir recursively as batch call */
                            b_return &= await this.RmDir(p_s_dir + "/" + o_dirElement.Name);
                        }
                        else if ((o_dirElement.Type == FtpObjectType.File) && (!o_dirElement.Name.EndsWith(".lock")))
                        {
                            ForestNET.Lib.Global.ILogFinest("directory element[" + o_dirElement.Name + "] is a file, so we delete the file as batch call");

                            /* directory element is a file, so we delete the file as batch call */
                            b_return &= await this.Delete(p_s_dir + "/" + o_dirElement.Name, true);

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

            /* iteration finished, we can reset delete counter */
            if ((this.DelegatePostProgressFolder != null) && (!p_b_batch))
            {
                ForestNET.Lib.Global.ILogFiner("iteration finished, we can reset delete counter");

                this.i_dirSum = 0;
                this.i_dirFiles = 0;
            }

            /* delete directory */
            try
            {
                ForestNET.Lib.Global.ILogFine("delete directory: '" + p_s_dir + "'");

                await this.FtpClient.DeleteDirectory(p_s_dir, this.Token());

                b_return &= true;
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.ILogSevere("could not delete directory: " + o_exc);

                b_return = false;
            }

            /* get ftp(s) reply code and reply message */
            this.ReadLastReply(null);

            return b_return;
        }

        /// <summary>
        /// Rename directory or file on ftp(s) server, do not delete existing files with new name.
        /// </summary>
        /// <param name="p_s_old">name of target directory or file on ftp(s) server</param>
        /// <param name="p_s_new">new name for directory or file</param>
        /// <returns>true - rename successful, false - issue with rename command</returns>
        public Task<bool> Rename(string p_s_old, string p_s_new)
        {
            return this.Rename(p_s_old, p_s_new, false);
        }

        /// <summary>
        /// Rename directory or file on ftp(s) server.
        /// </summary>
        /// <param name="p_s_old">name of target directory or file on ftp(s) server</param>
        /// <param name="p_s_new">new name for directory or file</param>
        /// <param name="p_b_overwrite">if a file already exists with new name, delete it</param>
        /// <returns>true - rename successful, false - issue with rename command</returns>
        public async Task<bool> Rename(string p_s_old, string p_s_new, bool p_b_overwrite)
        {
            /* check if old and new name are empty or both are equal */
            if ((ForestNET.Lib.Helper.IsStringEmpty(p_s_old)) || (ForestNET.Lib.Helper.IsStringEmpty(p_s_new)) || (p_s_old.Trim().Equals(p_s_new.Trim())))
            {
                ForestNET.Lib.Global.ILogFine("old or new name are empty or both are equal");

                return false;
            }

            /* get old element */
            FtpListItem? o_oldElement = await this.GetElement(p_s_old);

            /* get new element */
            FtpListItem? o_newElement = await this.GetElement(p_s_new);

            /* check if directory or file exists */
            if (o_oldElement == null)
            {
                ForestNET.Lib.Global.ILogWarning("Directory or File '" + p_s_old + "' does not exist on ftp(s) server");

                return false;
            }

            /* in case we want to overwrite it anyway we delete file with the new file name */
            if ((!p_b_overwrite) && (o_newElement != null))
            { /* directory or file already exists and we do not want to overwrite */
                ForestNET.Lib.Global.ILogWarning("Directory or file '" + p_s_new + "' already exists on ftp(s) server");

                return false;
            }
            else if ((o_newElement != null) && (o_newElement.Type == FtpObjectType.File))
            { /* file already exists so we delete it */
                ForestNET.Lib.Global.ILogFine("delete file with new name");

                /* delete file with new name */
                if (!(await this.Delete(p_s_new)))
                {
                    ForestNET.Lib.Global.ILogFine("could not delete file with new name '" + p_s_new + "'");

                    /* could not delete file with new name */
                    return false;
                }

                ForestNET.Lib.Global.ILogFine("deleted file with new name");
            }
            else if ((o_newElement != null) && (o_newElement.Type == FtpObjectType.Directory))
            { /* directory cannot be overwritten */
                ForestNET.Lib.Global.ILogWarning("Directory '" + p_s_new + "' cannot be overwritten on ftp(s) server. Please delete target directory first");

                return false;
            }

            /* replace all backslashes with slash */
            p_s_old = p_s_old.Trim().Replace('\\', '/');
            p_s_new = p_s_new.Trim().Replace('\\', '/');

            bool b_foo;

            /* rename directory or file */
            try
            {
                ForestNET.Lib.Global.ILogFine("rename element: '" + p_s_old + "' to '" + p_s_new + "'");

                if (((o_newElement != null) && (o_newElement.Type == FtpObjectType.File)) || (o_oldElement.Type == FtpObjectType.File))
                {
                    /* rename file */
                    b_foo = await this.FtpClient.MoveFile(p_s_old, p_s_new, FtpRemoteExists.Overwrite, this.Token());
                }
                else
                {
                    /* rename directory */
                    b_foo = await this.FtpClient.MoveDirectory(p_s_old, p_s_new, FtpRemoteExists.Overwrite, this.Token());
                }
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.ILogSevere("could not rename element: " + o_exc);

                b_foo = false;
            }

            /* get ftp(s) reply code and reply message */
            this.ReadLastReply(null);

            return b_foo;
        }

        /// <summary>
        /// Download content from a file on the ftp(s) server
        /// </summary>
        /// <param name="p_s_filePathSourceFtp">path to target file on ftp(s) server</param>
        /// <returns>file content - array of bytes</returns>
        /// <exception cref="ArgumentException">invalid path to target file</exception>
        /// <exception cref="System.IO.IOException">issue reading file stream or get amount of content length from input stream</exception>
        public async Task<byte[]?> Download(string p_s_filePathSourceFtp)
        {
            /* check file path parameter */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_filePathSourceFtp))
            {
                throw new ArgumentException("Please specify a file path for download");
            }

            /* replace all backslashes with slash */
            p_s_filePathSourceFtp = p_s_filePathSourceFtp.Trim().Replace('\\', '/');

            /* variable for filename without path and file size */
            string s_filename;
            int i_fileSize;

            /* we check if file really exists and retrieve file size */
            ForestNET.Lib.Global.ILogFine("check if file really exists");

            /* check if file really exists */
            FtpListItem? o_ftpFile = await this.GetElement(p_s_filePathSourceFtp);

            if (o_ftpFile == null)
            { /* file not found */
                throw new System.IO.IOException("File '" + p_s_filePathSourceFtp + "' does not exist on ftp(s) server");
            }
            else
            { /* retrieve file name and file size */
                ForestNET.Lib.Global.ILogFine("retrieve file name and file size");

                s_filename = o_ftpFile.Name;
                i_fileSize = (int)o_ftpFile.Size;

                ForestNET.Lib.Global.ILogFine("retrieved file name '" + s_filename + "' and file size '" + i_fileSize + "'");
            }

            byte[]? a_data = null;

            /* retrieve file stream */
            using (Stream o_inputStream = await this.FtpClient.OpenRead(p_s_filePathSourceFtp, (this.UseBinary) ? FtpDataType.Binary : FtpDataType.ASCII, 0, false, this.Token()))
            {
                /* check if stream object is available */
                if (!o_inputStream.CanRead)
                {
                    ForestNET.Lib.Global.ILogWarning("could not create input stream of file '" + p_s_filePathSourceFtp + "'");

                    return null;
                }

                /* initialize console progress bar */
                this.ConsoleProgressBar?.Init("Downloading . . .", "Download finished ['" + s_filename + "']", s_filename);

                ForestNET.Lib.Global.ILogFiner("receiving bytes from input stream");

                /* receiving bytes from input stream */
                a_data = await this.ReceiveBytes(o_inputStream, this.BufferSize, i_fileSize);

                ForestNET.Lib.Global.ILogFiner("received bytes from input stream");
            }

            /* get a return value, but no content so we set data to null */
            if ((a_data != null) && (a_data.Length <= 0))
            {
                a_data = null;
            }

            /* get ftp(s) reply code and reply message */
            this.ReadLastReply(null);

            /* close console progress bar */
            this.ConsoleProgressBar?.Close();

            return a_data;
        }

        /// <summary>
        /// Download content from a file on the ftp(s) server to a local file, local file will be overwritten.
        /// </summary>
        /// <param name="p_s_filePathSourceFtp">path to target file on ftp(s) server</param>
        /// <param name="p_s_filePathDestinationLocal">path to destination file on local system</param>
        /// <returns>true - download successful, false - download failed</returns>
        /// <exception cref="ArgumentException">invalid path to target file, invalid path to local system</exception>
        /// <exception cref="System.IO.IOException">destination directory could not be created, issue reading file stream or issue writing to local system</exception>
        public async Task<bool> Download(string p_s_filePathSourceFtp, string p_s_filePathDestinationLocal)
        {
            return await this.Download(p_s_filePathSourceFtp, p_s_filePathDestinationLocal, true);
        }

        /// <summary>
        /// Download content from a file on the ftp(s) server to a local file.
        /// </summary>
        /// <param name="p_s_filePathSourceFtp">path to target file on ftp(s) server</param>
        /// <param name="p_s_filePathDestinationLocal">path to destination file on local system</param>
        /// <param name="p_b_overwrite">true - if local file already exists, delete it, false - if local file already exists do not download and return true</param>
        /// <returns>true - download successful, false - download failed</returns>
        /// <exception cref="ArgumentException">invalid path to target file, invalid path to local system</exception>
        /// <exception cref="System.IO.IOException">destination directory could not be created, issue reading file stream or issue writing to local system</exception>
        public async Task<bool> Download(string p_s_filePathSourceFtp, string p_s_filePathDestinationLocal, bool p_b_overwrite)
        {
            /* if file already exists locally and we do not want to overwrite it, just return true */
            if ((ForestNET.Lib.IO.File.Exists(p_s_filePathDestinationLocal)) && (!p_b_overwrite))
            {
                ForestNET.Lib.Global.ILogFine("file already exists locally and we do not want to overwrite it");

                return true;
            }

            /* download file content to byte array from ftp(s) server */
            byte[]? a_data = await this.Download(p_s_filePathSourceFtp);

            /* download failed */
            if (a_data == null)
            {
                ForestNET.Lib.Global.ILogWarning("download failed");

                return false;
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
                ForestNET.Lib.Global.ILogFine("create local directory '" + p_s_filePathDestinationLocal.Substring(0, p_s_filePathDestinationLocal.LastIndexOf(ForestNET.Lib.IO.File.DIR)) + "' if it not exists");

                ForestNET.Lib.IO.File.CreateDirectory(p_s_filePathDestinationLocal.Substring(0, p_s_filePathDestinationLocal.LastIndexOf(ForestNET.Lib.IO.File.DIR)), true);
            }

            ForestNET.Lib.Global.ILogFine("write downloaded bytes to local file '" + p_s_filePathDestinationLocal + "'");

            /* write downloaded bytes to local file */
            await System.IO.File.WriteAllBytesAsync(p_s_filePathDestinationLocal, a_data, this.Token());

            return true;
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

                ForestNET.Lib.Global.ILogFinest("p_o_inputStream.ReadAsync, expecting " + i_expectedBytes + " bytes");

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
        /// Upload local file to a file on ftp(s) server, no append.
        /// </summary>
        /// <param name="p_s_filePathSourceLocal">path to source file on local system</param>
        /// <param name="p_s_filePathDestinationFtp">path to destination file on ftp(s) server</param>
        /// <returns>true - upload successful, false - upload failed</returns>
        /// <exception cref="ArgumentException">invalid path to local file, or invalid path to destination file</exception>
        /// <exception cref="System.IO.IOException">could not auto create target directory, upload content data, rename destination file, reading local file or delete existing file</exception>
        public async Task<bool> Upload(string p_s_filePathSourceLocal, string p_s_filePathDestinationFtp)
        {
            return await this.Upload(p_s_filePathSourceLocal, p_s_filePathDestinationFtp, false);
        }

        /// <summary>
        /// Upload local file to a file on ftp(s) server, append mode possible.
        /// </summary>
        /// <param name="p_s_filePathSourceLocal">path to source file on local system</param>
        /// <param name="p_s_filePathDestinationFtp">path to destination file on ftp(s) server</param>
        /// <param name="p_b_append">true - append content data to existing file, false - overwrite file</param>
        /// <returns>true - upload successful, false - upload failed</returns>
        /// <exception cref="ArgumentException">invalid path to local file, or invalid path to destination file</exception>
        /// <exception cref="System.IO.IOException">could not auto create target directory, upload content data, rename destination file, reading local file or delete existing file</exception>
        public async Task<bool> Upload(string p_s_filePathSourceLocal, string p_s_filePathDestinationFtp, bool p_b_append)
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
                ForestNET.Lib.Global.ILogWarning("no bytes retrieved from local file, cannot continue with upload");

                return false;
            }

            /* start upload procedure */
            return await this.Upload(a_data, p_s_filePathDestinationFtp, p_b_append);
        }

        /// <summary>
        /// Upload content data to a file on ftp(s) server, append mode possible.
        /// </summary>
        /// <param name="p_a_data">content data as byte array</param>
        /// <param name="p_s_filePathDestinationFtp">path to destination file on ftp(s) server</param>
        /// <param name="p_b_append">true - append content data to existing file, false - overwrite file</param>
        /// <returns>true - upload successful, false - upload failed</returns>
        /// <exception cref="ArgumentException">invalid content data, or invalid path to destination file</exception>
        /// <exception cref="System.IO.IOException">could not auto create target directory, upload content data, rename destination file or delete existing file</exception>
        public async Task<bool> Upload(byte[] p_a_data, string p_s_filePathDestinationFtp, bool p_b_append)
        {
            /* byte array parameter is null or has no content */
            if ((p_a_data == null) || (p_a_data.Length == 0))
            {
                throw new ArgumentException("Please specify data for upload");
            }

            /* file path parameter for upload is null or empty */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_filePathDestinationFtp))
            {
                throw new ArgumentException("Please specify a local file path for upload");
            }

            /* replace all backslashes with slash */
            p_s_filePathDestinationFtp = p_s_filePathDestinationFtp.Trim().Replace('\\', '/');

            /* file path for destination on ftp(s) server must start with root '/' */
            if (!p_s_filePathDestinationFtp.StartsWith("/"))
            {
                p_s_filePathDestinationFtp = "/" + p_s_filePathDestinationFtp;
            }

            /* file path for destination on ftp(s) server must not start with '.lock' */
            if (p_s_filePathDestinationFtp.EndsWith(".lock"))
            {
                throw new ArgumentException("Filename for upload must not end with '.lock'");
            }

            /* variable for filename on upload */
            string s_filename;

            /* separate destination path and file name */
            if (p_s_filePathDestinationFtp.LastIndexOf("/") > 0)
            {
                string s_path = p_s_filePathDestinationFtp.Substring(0, p_s_filePathDestinationFtp.LastIndexOf("/"));
                s_filename = p_s_filePathDestinationFtp.Substring(p_s_filePathDestinationFtp.LastIndexOf("/") + 1);

                /* if this is no batch call, check if destination path exists */
                if (!await this.DirectoryExists(s_path))
                {
                    ForestNET.Lib.Global.ILogFine("destination path does not exist, so we create it");

                    /* destination path does not exist, so we create it */
                    if (!(await this.MkDir(s_path)))
                    {
                        ForestNET.Lib.Global.ILogSevere("could not create directory '" + s_path + "'");

                        return false;
                    }
                }
            }
            else
            { /* we are at root level */
                s_filename = p_s_filePathDestinationFtp.Substring(1);
            }

            bool b_return = false;
            bool b_exists = false;

            /* check if destination file on ftp(s) server already exists */
            if (await this.FileExists(p_s_filePathDestinationFtp))
            {
                ForestNET.Lib.Global.ILogFine("destination file on ftp(s) server already exists");

                b_exists = true;
            }
            else
            {
                ForestNET.Lib.Global.ILogFine("destination file on ftp(s) server does not exist, set append parameter to 'false'");

                p_b_append = false;
            }

            if (!p_b_append)
            { /* we are uploading a new file to the ftp(s) server */
                /* delete file on ftp(s) server if it already exists */
                if ((b_exists) && (!await this.Delete(p_s_filePathDestinationFtp)))
                {
                    throw new System.IO.IOException("FTP-Error with file [" + p_s_filePathDestinationFtp + "], cannot delete");
                }

                ForestNET.Lib.Global.ILogFine("create file '" + p_s_filePathDestinationFtp + "' on ftp(s) server with '.lock' as extension");

                /* create file on ftp(s) server with '.lock' as extension */
                using (Stream o_outputStream = await this.FtpClient.OpenWrite(p_s_filePathDestinationFtp + ".lock", (this.UseBinary) ? FtpDataType.Binary : FtpDataType.ASCII, false, this.Token()))
                {
                    /* check if stream object is available */
                    if (!o_outputStream.CanWrite)
                    {
                        ForestNET.Lib.Global.ILogWarning("could not create output stream to file '" + p_s_filePathDestinationFtp + ".lock'");

                        return false;
                    }

                    /* initialize console progress bar */
                    this.ConsoleProgressBar?.Init("Uploading . . .", "Upload finished ['" + s_filename + "']", s_filename);

                    /* sending bytes to output stream */
                    b_return &= await this.SendBytes(o_outputStream, p_a_data, this.BufferSize);
                }

                /* get ftp(s) reply code and reply message */
                this.ReadLastReply(null);

                /* close console progress bar */
                this.ConsoleProgressBar?.Close();

                ForestNET.Lib.Global.ILogFine("remove '.lock' extension");

                /* remove '.lock' extension */
                b_return = await this.FtpClient.MoveFile(p_s_filePathDestinationFtp + ".lock", p_s_filePathDestinationFtp, FtpRemoteExists.Overwrite, this.Token());
            }
            else
            { /* we are appending data to an existing file on the ftp(s) server */
                ForestNET.Lib.Global.ILogFine("rename existing file with '.lock' extension");

                /* rename existing file with '.lock' extension */
                if (!(await this.FtpClient.MoveFile(p_s_filePathDestinationFtp, p_s_filePathDestinationFtp + ".lock", FtpRemoteExists.Overwrite, this.Token())))
                {
                    throw new System.IO.IOException("FTP-Error with file [" + p_s_filePathDestinationFtp + "], cannot rename with '.lock' extension");
                }

                ForestNET.Lib.Global.ILogFine("append bytes[" + p_a_data.Length + "] to file '" + p_s_filePathDestinationFtp + "' on ftp(s) server with '.lock' as extension");

                /* append bytes to file on ftp(s) server with '.lock' as extension  */
                using (Stream o_outputStream = await this.FtpClient.OpenAppend(p_s_filePathDestinationFtp + ".lock", (this.UseBinary) ? FtpDataType.Binary : FtpDataType.ASCII, false, this.Token()))
                {
                    /* check if stream object is available */
                    if (!o_outputStream.CanWrite)
                    {
                        ForestNET.Lib.Global.ILogWarning("could not create output stream to file '" + p_s_filePathDestinationFtp + ".lock'");

                        return false;
                    }

                    /* initialize console progress bar */
                    this.ConsoleProgressBar?.Init("Uploading . . .", "Upload finished ['" + s_filename + "']", s_filename);

                    /* sending bytes to output stream */
                    b_return &= await this.SendBytes(o_outputStream, p_a_data, this.BufferSize);
                }

                /* get ftp(s) reply code and reply message */
                this.ReadLastReply(null);

                /* close console progress bar */
                this.ConsoleProgressBar?.Close();

                ForestNET.Lib.Global.ILogFine("remove '.lock' extension");

                /* remove '.lock' extension */
                b_return = await this.FtpClient.MoveFile(p_s_filePathDestinationFtp + ".lock", p_s_filePathDestinationFtp, FtpRemoteExists.Overwrite, this.Token());
            }

            return b_return;
        }

        /// <summary>
        /// Send data to output stream object instance
        /// </summary>
        /// <param name="p_o_outputStream">output stream object instance</param>
        /// <param name="p_a_data">content data which will be uploaded</param>
        /// <param name="p_i_bufferLength">size of buffer which is used send the output stream</param>
        /// <exception cref="System.IO.IOException">issue sending to output stream object instance</exception>
        private async Task<bool> SendBytes(Stream p_o_outputStream, byte[] p_a_data, int p_i_bufferLength)
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

            return true;
        }

        /// <summary>
        /// Download a complete folder from ftp(s) server to local system, not downloading any sub directories and it's files and not overwriting files and re-download them.
        /// </summary>
        /// <param name="p_s_sourceDirectoryFtp">path of source directory on ftp(s) server</param>
        /// <param name="p_s_destinationDirectoryLocal">path to destination directory on local system</param>
        /// <exception cref="ArgumentException">invalid path to source or destination directory</exception>
        /// <exception cref="System.IO.IOException">destination directory could not be created, issue reading file stream or issue writing to local system</exception>
        public async Task DownloadFolder(string p_s_sourceDirectoryFtp, string p_s_destinationDirectoryLocal)
        {
            await this.DownloadFolder(p_s_sourceDirectoryFtp, p_s_destinationDirectoryLocal, false, false);
        }

        /// <summary>
        /// Download a complete folder from ftp(s) server to local system, not downloading any sub directories and it's files.
        /// </summary>
        /// <param name="p_s_sourceDirectoryFtp">path of source directory on ftp(s) server</param>
        /// <param name="p_s_destinationDirectoryLocal">path to destination directory on local system</param>
        /// <param name="p_b_overwrite">true - if local file or directory already exists, delete it, false - if local file or directory already exists do not download and return true</param>
        /// <exception cref="ArgumentException">invalid path to source or destination directory</exception>
        /// <exception cref="System.IO.IOException">destination directory could not be created, issue reading file stream or issue writing to local system</exception>
        public async Task DownloadFolder(string p_s_sourceDirectoryFtp, string p_s_destinationDirectoryLocal, bool p_b_overwrite)
        {
            await this.DownloadFolder(p_s_sourceDirectoryFtp, p_s_destinationDirectoryLocal, p_b_overwrite, false);
        }

        /// <summary>
        /// Download a complete folder from ftp(s) server to local system.
        /// </summary>
        /// <param name="p_s_sourceDirectoryFtp">path of source directory on ftp(s) server</param>
        /// <param name="p_s_destinationDirectoryLocal">path to destination directory on local system</param>
        /// <param name="p_b_overwrite">true - if local file or directory already exists, delete it, false - if local file or directory already exists do not download and return true</param>
        /// <param name="p_b_recursive">true - include all sub directories and it's files, false - stay in source ftp(s) directory</param>
        /// <exception cref="ArgumentException">invalid path to source or destination directory</exception>
        /// <exception cref="System.IO.IOException">destination directory could not be created, issue reading file stream or issue writing to local system</exception>
        public async Task DownloadFolder(string p_s_sourceDirectoryFtp, string p_s_destinationDirectoryLocal, bool p_b_overwrite, bool p_b_recursive)
        {
            await this.DownloadFolder(p_s_sourceDirectoryFtp, p_s_destinationDirectoryLocal, p_b_overwrite, p_b_recursive, false);
        }

        /// <summary>
        /// Download a complete folder from ftp(s) server to local system.
        /// </summary>
        /// <param name="p_s_sourceDirectoryFtp">path of source directory on ftp(s) server</param>
        /// <param name="p_s_destinationDirectoryLocal">path to destination directory on local system</param>
        /// <param name="p_b_overwrite">true - if local file or directory already exists, delete it, false - if local file or directory already exists do not download and return true</param>
        /// <param name="p_b_recursive">true - include all sub directories and it's files, false - stay in source ftp(s) directory</param>
        /// <param name="p_b_batch">method in internal batch call, no check if source directory really exists on ftp(s) server</param>
        /// <exception cref="ArgumentException">invalid path to source or destination directory</exception>
        /// <exception cref="System.IO.IOException">destination directory could not be created, issue reading file stream or issue writing to local system</exception>
        private async Task DownloadFolder(string p_s_sourceDirectoryFtp, string p_s_destinationDirectoryLocal, bool p_b_overwrite, bool p_b_recursive, bool p_b_batch)
        {
            /* check source directory parameter for download from ftp(s) server */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_sourceDirectoryFtp))
            {
                throw new ArgumentException("Please specify a source directory for download from ftp(s) server, parameter is 'null'");
            }

            /* replace all backslashes with slash */
            p_s_sourceDirectoryFtp = p_s_sourceDirectoryFtp.Trim().Replace('\\', '/');

            /* check destination directory parameter for download directory on local system */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_destinationDirectoryLocal))
            {
                throw new ArgumentException("Please specify a destination directory for download on local system, parameter is 'null'");
            }

            /* delete last DIR character from local destination directory */
            if (p_s_destinationDirectoryLocal.EndsWith(ForestNET.Lib.IO.File.DIR))
            {
                p_s_destinationDirectoryLocal = p_s_destinationDirectoryLocal.Substring(0, (p_s_destinationDirectoryLocal.Length - 1));
            }

            /* not a batch call, check if directory on ftp(s) server really exists */
            if ((!p_b_batch) && (!await this.DirectoryExists(p_s_sourceDirectoryFtp)))
            {
                throw new ArgumentException("Please specify a valid source directory[" + p_s_sourceDirectoryFtp + "] for download from the ftp(s) server");
            }

            /* if local destination directory does not exist, create it with auto create */
            if (!ForestNET.Lib.IO.File.IsDirectory(p_s_destinationDirectoryLocal))
            {
                ForestNET.Lib.Global.ILogFine("local destination directory '" + p_s_destinationDirectoryLocal.Substring(p_s_destinationDirectoryLocal.LastIndexOf(ForestNET.Lib.IO.File.DIR)) + "' does not exist, create it with auto create");

                ForestNET.Lib.IO.File.CreateDirectory(p_s_destinationDirectoryLocal, true);
            }

            /* get all directories and files of directory path */
            FtpListItem[] a_elements = await this.ListFTPFiles(p_s_sourceDirectoryFtp, this.PreferMLSD);

            if (a_elements != null)
            {
                /* count files of directory and all sub directories for delegate, but only if it is not a batch call */
                if ((this.DelegatePostProgressFolder != null) && (!p_b_batch))
                {
                    ForestNET.Lib.Global.ILogFiner("count files of directory and all sub directories for delegate");

                    this.i_dirFiles = (await this.Ls(p_s_sourceDirectoryFtp, true, false, this.PreferMLSD, p_b_recursive)).Count;

                    ForestNET.Lib.Global.ILogFiner("elements to be downloaded: " + this.i_dirFiles);
                }

                /* iterate each directory element */
                foreach (FtpListItem o_dirElement in a_elements)
                {
                    /* directory element must be not null and not a temporary file */
                    if ((o_dirElement != null) && (!o_dirElement.Name.EndsWith(".lock")) && (!o_dirElement.Name.Equals(".")) && (!o_dirElement.Name.Equals("..")))
                    {
                        if (o_dirElement.Type == FtpObjectType.Directory)
                        { /* directory element is a directory */
                            /* create local directory with the same name if it does not exist */
                            if (!ForestNET.Lib.IO.File.IsDirectory(p_s_destinationDirectoryLocal))
                            {
                                ForestNET.Lib.Global.ILogFiner("create local directory: ." + ForestNET.Lib.IO.File.DIR + o_dirElement.Name);

                                ForestNET.Lib.IO.File.CreateDirectory(p_s_destinationDirectoryLocal + ForestNET.Lib.IO.File.DIR + o_dirElement.Name);
                            }

                            /* recursive flag set, download all sub directories and its elements with recursive batch call */
                            if (p_b_recursive)
                            {
                                ForestNET.Lib.Global.ILogFiner("download sub directory: '" + o_dirElement.Name + "' to '" + p_s_destinationDirectoryLocal + ForestNET.Lib.IO.File.DIR + o_dirElement.Name + "'");

                                await this.DownloadFolder(p_s_sourceDirectoryFtp + "/" + o_dirElement.Name, p_s_destinationDirectoryLocal + ForestNET.Lib.IO.File.DIR + o_dirElement.Name, p_b_overwrite, p_b_recursive);
                            }
                        }
                        else
                        { /* directory element is a file */
                            /* download file if it does not exist locally, file size does not match, or we want to do a clean download */
                            if ((!ForestNET.Lib.IO.File.Exists(p_s_destinationDirectoryLocal + ForestNET.Lib.IO.File.DIR + o_dirElement.Name)) || (ForestNET.Lib.IO.File.FileLength(p_s_destinationDirectoryLocal + ForestNET.Lib.IO.File.DIR + o_dirElement.Name) != o_dirElement.Size) || (p_b_overwrite))
                            {
                                bool b_retry = true;
                                int i_attempts = 1;
                                int i_maxAttempts = 40;

                                do
                                {
                                    if (!(await this.Download(p_s_sourceDirectoryFtp + "/" + o_dirElement.Name, p_s_destinationDirectoryLocal + ForestNET.Lib.IO.File.DIR + o_dirElement.Name, p_b_overwrite)))
                                    {
                                        ForestNET.Lib.Global.ILogWarning("could not download file '" + p_s_sourceDirectoryFtp + "/" + o_dirElement.Name + "'");
                                    }
                                    else
                                    {
                                        ForestNET.Lib.Global.ILogFiner("downloaded file '" + p_s_destinationDirectoryLocal + ForestNET.Lib.IO.File.DIR + o_dirElement.Name + "'");
                                        b_retry = false;
                                    }

                                    try
                                    {
                                        Thread.Sleep(25);
                                    }
                                    catch (Exception)
                                    {
                                        /* nothing to do */
                                    }

                                    if (i_attempts++ >= i_maxAttempts)
                                    {
                                        throw new System.IO.IOException("Downloading file failed, after " + i_attempts + " attempts");
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

            /* iteration finished, we can reset download counter */
            if ((this.DelegatePostProgressFolder != null) && (!p_b_batch))
            {
                this.i_dirSum = 0;
                this.i_dirFiles = 0;
            }
        }

        /// <summary>
        /// Upload a complete folder from local system to a ftp(s) server, not uploading any sub directories and it's files.
        /// </summary>
        /// <param name="p_s_sourceDirectoryLocal">path to source directory on local system</param>
        /// <param name="p_s_destinationdirectoryFtp">path of destination directory on ftp(s) server</param>
        /// <exception cref="ArgumentException">invalid path to source or destination directory</exception>
        /// <exception cref="System.IO.IOException">could not auto create target directory, upload content data, rename destination file, reading local file or delete existing file</exception>
        public async Task UploadFolder(string p_s_sourceDirectoryLocal, string p_s_destinationdirectoryFtp)
        {
            await this.UploadFolder(p_s_sourceDirectoryLocal, p_s_destinationdirectoryFtp, false);
        }

        /// <summary>
        /// Upload a complete folder from local system to a ftp(s) server.
        /// </summary>
        /// <param name="p_s_sourceDirectoryLocal">path to source directory on local system</param>
        /// <param name="p_s_destinationdirectoryFtp">path of destination directory on ftp(s) server</param>
        /// <param name="p_b_recursive">true - include all sub directories and it's files, false - stay in source directory on local system</param>
        /// <exception cref="ArgumentException">invalid path to source or destination directory</exception>
        /// <exception cref="System.IO.IOException">could not auto create target directory, upload content data, rename destination file, reading local file or delete existing file</exception>
        public async Task UploadFolder(string p_s_sourceDirectoryLocal, string p_s_destinationdirectoryFtp, bool p_b_recursive)
        {
            /* check file path parameter for upload directory on local system */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_sourceDirectoryLocal))
            {
                throw new ArgumentException("Please specify a source directory for upload on the local system");
            }

            /* check file path parameter for upload directory on ftp(s) server */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_destinationdirectoryFtp))
            {
                throw new ArgumentException("Please specify a destination directory for upload on the ftp(s) server");
            }

            /* replace all backslashes with slash */
            p_s_destinationdirectoryFtp = p_s_destinationdirectoryFtp.Trim().Replace('\\', '/');

            /* check if file path parameter on local system really exists */
            if (!ForestNET.Lib.IO.File.IsDirectory(p_s_sourceDirectoryLocal))
            {
                throw new ArgumentException("Please specify a valid source directory for upload, directory '" + p_s_sourceDirectoryLocal + "' does not exist");
            }

            /* check if directory exists on ftp(s) server */
            if (!await this.DirectoryExists(p_s_destinationdirectoryFtp))
            {
                ForestNET.Lib.Global.ILogFine("directory '" + p_s_destinationdirectoryFtp + "' does not exist, must be created on ftp(s) server");

                /* create directory on ftp(s) server */
                if (!await this.MkDir(p_s_destinationdirectoryFtp))
                {
                    throw new System.IO.IOException("Could not create destination directory[" + p_s_destinationdirectoryFtp + "]");
                }
            }

            /* list all directory elements on local system, optional with all sub directories */
            List<ForestNET.Lib.IO.ListingElement> a_list = ForestNET.Lib.IO.File.ListDirectory(p_s_sourceDirectoryLocal, p_b_recursive);

            /* count all elements which must be uploaded for delegate upload counter */
            if (this.DelegatePostProgressFolder != null)
            {
                ForestNET.Lib.Global.ILogFine("count all elements which must be uploaded for delegate upload counter");

                this.i_dirFiles = a_list.Count;

                /* do not count directory elements, because these will be created automatically */
                foreach (ForestNET.Lib.IO.ListingElement o_listingElement in a_list)
                {
                    if (o_listingElement.IsDirectory)
                    {
                        this.i_dirFiles--;
                    }
                }

                ForestNET.Lib.Global.ILogFine("elements to be uploaded: " + this.i_dirFiles);
            }

            /* iterate each directory element */
            foreach (ForestNET.Lib.IO.ListingElement o_listingElement in a_list)
            {
                /* just get directory or file name as temporary variable */
                string s_foo = o_listingElement.FullName?.Substring(p_s_sourceDirectoryLocal.Length) ?? "";

                /* replace all backslashes with slash */
                s_foo = s_foo.Replace('\\', '/');

                ForestNET.Lib.Global.ILogFiner("destination directory: '" + p_s_destinationdirectoryFtp + "' and new remote element: '" + s_foo + "'");

                if (o_listingElement.IsDirectory)
                { /* local element is directory */
                    /* create directory on ftp(s) server */
                    if (!await this.DirectoryExists(p_s_destinationdirectoryFtp + "/" + s_foo))
                    {
                        if (!await this.MkDir(p_s_destinationdirectoryFtp + "/" + s_foo))
                        {
                            throw new System.IO.IOException("could not create directory '" + p_s_destinationdirectoryFtp + "/" + s_foo + "'");
                        }

                        ForestNET.Lib.Global.ILogFiner("created directory: " + p_s_destinationdirectoryFtp + "/" + s_foo);
                    }
                }
                else
                { /* local element is file */
                    /* upload file, will be overwritten(delete and re-upload) if it already exists */
                    if (await this.Upload(o_listingElement.FullName ?? "", p_s_destinationdirectoryFtp + "/" + s_foo, false))
                    {
                        ForestNET.Lib.Global.ILogFiner("uploaded file  : " + p_s_destinationdirectoryFtp + "/" + s_foo);
                    }
                    else
                    {
                        ForestNET.Lib.Global.ILogWarning("could not upload file  : " + p_s_destinationdirectoryFtp + "/" + s_foo);
                    }

                    /* increase upload counter and call delegate */
                    if (this.DelegatePostProgressFolder != null)
                    {
                        this.i_dirSum++;
                        this.DelegatePostProgressFolder.Invoke(this.i_dirSum, this.i_dirFiles);

                        ForestNET.Lib.Global.ILogFine("uploaded: " + this.i_dirSum + "/" + this.i_dirFiles);
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