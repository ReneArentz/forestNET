namespace ForestNET.Lib.Net.Https
{
    /// <summary>
    /// Configuration class for tiny https/soap/rest server object. All configurable settings are listed and adjustable in this class. Please look on the comments of the set-property-methods to see further details.
    /// </summary>
    public class Config
    {

        /* Constants */

        public const string HTTP_LINEBREAK = "\r\n";
        public const string IN_ENCODING = "UTF-8";
        public const string OUT_ENCODING = "UTF-8";
        public static readonly Dictionary<string, string> KNOWN_EXTENSION_LIST = new() {
            { ".avif", "image/avif" },
            { ".bmp", "image/bmp" },
            { ".css", "text/css" },
            { ".csv", "text/csv" },
            { ".eot", "application/vnd.ms-fontobject" },
            { ".gif", "image/gif" },
            { ".htm", "text/html" },
            { ".html", "text/html" },
            { ".ico", "image/x-icon" },
            { ".jpeg", "image/jpeg" },
            { ".jpg", "image/jpeg" },
            { ".js", "text/javascript" },
            { ".json", "application/json" },
            { ".jsonld", "application/ld+json" },
            { ".otf", "font/otf" },
            { ".pdf", "application/pdf" },
            { ".png", "image/png" },
            { ".rtf", "application/rtf" },
            { ".svg", "image/svg+xml" },
            { ".tif", "image/tiff" },
            { ".tiff", "image/tiff" },
            { ".ttf", "font/ttf" },
            { ".txt", "text/plain" },
            { ".webp", "image/webp" },
            { ".woff", "font/woff" },
            { ".woff2", "font/woff2" },
            { ".wsdl", "text/xml" },
            { ".xhtml", "application/xhtml+xml" },
            { ".xml", "application/xml" },
            { ".xsd", "text/xml" },
            { ".xslt", "text/xml" }
        };

        /* Fields */

        private Mode e_mode;
        private ForestNET.Lib.Net.Sock.Recv.ReceiveType e_socketReceiveType;
        private int i_servicePoolAmount;
        private string s_domain;
        private string? s_host;
        private int i_port;
        private int i_timeoutMilliseconds;
        private int i_maxTerminations;
        private int i_maxPayload;
        private int i_amountCyclesToleratingDelay;
        private bool b_debugNetworkTrafficOn;
        private bool b_printExceptionStracktrace;

        private bool b_checkReachability;
        private int i_intervalMilliseconds;
        private ForestNET.Lib.Net.Sock.Send.SendTCP? o_sendingSocketInstanceForHttpClient;

        private string? s_rootDirectory;
        private List<string> a_allowSourceList;
        private bool b_notUsingCookies;
        private bool b_clientUseCookiesFromPreviousRequest;
        private string? s_sessionDirectory;
        private ForestNET.Lib.DateInterval? o_sessionMaxAge;
        private bool b_sessionRefresh;
        private Dictionary<string, string> a_allowExtensionList;

        private System.Text.Encoding? o_inEncoding;
        private System.Text.Encoding? o_outEncoding;

        private ForestNET.Lib.Net.Https.Dynm.ForestSeed? o_forestSeed;
        private ForestNET.Lib.Net.Https.SOAP.WSDL? o_wsdl;
        private ForestNET.Lib.Net.Https.REST.ForestREST? o_forestREST;

        /* Properties */

        /// <summary>
        /// determine mode for tiny https server: NORMAL, DYNAMIC, SOAP or REST
        /// </summary>
        public Mode Mode
        {
            get
            {
                return this.e_mode;
            }
            set
            {
                this.e_mode = value;
            }
        }
        /// <summary>
        /// determine receive type for all sockets: SOCKET or SERVER
        /// </summary>
        public ForestNET.Lib.Net.Sock.Recv.ReceiveType SocketReceiveType
        {
            get
            {
                return this.e_socketReceiveType;
            }
            set
            {
                this.e_socketReceiveType = value;
            }
        }
        /// <summary>
        /// integer value for fixed amount of tasks for task pool instance of tiny https server object
        /// </summary>
        /// <exception cref="ArgumentException">negative integer value as parameter</exception>
        public int SocketServicePoolAmount
        {
            get
            {
                return this.i_servicePoolAmount;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Socket executor service pool amount must be at least '0', but was set to '" + value + "'");
                }

                this.i_servicePoolAmount = value;
            }
        }
        /// <summary>
        /// domain parameter value
        /// </summary>
        /// <exception cref="ArgumentException">domain parameter value does not start with 'https://'</exception>
		/// <exception cref="ArgumentNullException">domain parameter value is null</exception>
        public string Domain
        {
            get
            {
                return this.s_domain;
            }
            set
            {
                /* check if domain value is not empty */
                if ((value == null) || (ForestNET.Lib.Helper.IsStringEmpty(value)))
                {
                    throw new ArgumentNullException(nameof(value), "Domain parameter is null");
                }

                /* check if domain value starts with https:// */
                if (!value.StartsWith("https://"))
                {
                    throw new ArgumentException("Domain parameter does not start with 'https://'");
                }

                this.s_domain = value;
            }
        }
        /// <summary>
        /// host parameter value
        /// </summary>
        /// <exception cref="ArgumentException">host parameter value is null or empty</exception>
        public string? Host
        {
            get
            {
                return this.s_host;
            }
            set
            {
                /* check if host value is not empty */
                if ((value == null) || (ForestNET.Lib.Helper.IsStringEmpty(value)))
                {
                    throw new ArgumentException("Host parameter is null or empty");
                }

                this.s_host = value;
            }
        }
        /// <summary>
        /// post parameter value
        /// </summary>
        /// <exception cref="ArgumentException">invalid port parameter</exception>
        public int Port
        {
            get
            {
                return this.i_port;
            }
            set
            {
                /* check port min. value */
                if (value < 1)
                {
                    throw new ArgumentException("Port must be at least '1', but was set to '" + value + "'");
                }

                /* check port max. value */
                if (value > 65535)
                {
                    throw new ArgumentException("Port must be lower equal '65535', but was set to '" + value + "'");
                }

                this.i_port = value;
            }
        }
        /// <summary>
        /// integer value for timeout in milliseconds of socket object - how long will a receive socket block execution
        /// </summary>
        /// <exception cref="ArgumentException">invalid timeout parameter</exception>
        public int TimeoutMilliseconds
        {
            get
            {
                return this.i_timeoutMilliseconds;
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("Receiver timeout must be at least '1' millisecond, but was set to '" + value + "' millisecond(s)");
                }

                this.i_timeoutMilliseconds = value;
            }
        }
        /// <summary>
        /// set a max. value for thread executions of socket instance
        /// </summary>
        public int MaxTerminations
        {
            get
            {
                return this.i_maxTerminations;
            }
            set
            {
                if (value < 0)
                {
                    value = -1;
                }

                this.i_maxTerminations = value;
            }
        }
        /// <summary>
        /// set a max. value for receiving payload in MiB (1..999)
        /// </summary>
        /// <exception cref="ArgumentException">invalid max payload parameter</exception>
        public int MaxPayload
        {
            get
            {
                return this.i_maxPayload;
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
                    this.i_maxPayload = value;
                }
            }
        }
        /// <summary>
        /// set amount of cycles tolerating delay (1[50ms]..100[5000ms]) while receiving data
        /// </summary>
        /// <exception cref="ArgumentException">invalid delay parameter</exception>
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
                    throw new ArgumentException("Amount of cycles tolerating delay (1 cycle = 50ms) must be at least '1', but was set to '" + value + "'");
                }
                else if (value > 100)
                {
                    throw new ArgumentException("Amount of cycles tolerating delay (1 cycle = 50ms) must be lower than '100 (5000ms)', but was set to '" + value + "'");
                }
                else
                {
                    this.i_amountCyclesToleratingDelay = value;
                }
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
                this.b_debugNetworkTrafficOn = value;
            }
        }
        /// <summary>
        /// true - show detailed of occurred exception of serve/client task
        /// </summary>
        public bool PrintExceptionStracktrace
        {
            get
            {
                return this.b_printExceptionStracktrace;
            }
            set
            {
                this.b_printExceptionStracktrace = value;
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
        /// integer value for interval in milliseconds of socket object - how long will a sender socket wait for new data
        /// </summary>
        /// <exception cref="ArgumentException">invalid parameter value</exception>
        public int IntervalMilliseconds
        {
            get
            {
                return this.i_intervalMilliseconds;
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("Sender interval must be at least '1' millisecond, but was set to '" + value + "' millisecond(s)");
                }

                this.i_intervalMilliseconds = value;
            }
        }
        /// <summary>
        /// sending socket instance parameter value
        /// </summary>
        /// <exception cref="ArgumentNullException">parameter value is null</exception>
        public ForestNET.Lib.Net.Sock.Send.SendTCP? SendingSocketInstance
        {
            get
            {
                return this.o_sendingSocketInstanceForHttpClient;
            }
            set
            {
                /* check if sending socket instance is null */
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "Sending socket instance parameter is null");
                }

                this.o_sendingSocketInstanceForHttpClient = value;
            }
        }

        /// <summary>
        /// root directory parameter value
        /// </summary>
        /// <exception cref="ArgumentNullException">parameter value is null</exception>
        public string? RootDirectory
        {
            get
            {
                return this.s_rootDirectory;
            }
            set
            {
                /* check if root directory value is not empty */
                if ((value == null) || (ForestNET.Lib.Helper.IsStringEmpty(value)))
                {
                    throw new ArgumentNullException(nameof(value), "Root directory parameter is null");
                }

                if (!value.EndsWith(ForestNET.Lib.IO.File.DIR))
                {
                    value += ForestNET.Lib.IO.File.DIR;
                }

                this.s_rootDirectory = value;
            }
        }
        /// <summary>
        /// instantiated allow source list object
        /// </summary>
        /// <exception cref="ArgumentNullException">allow list parameter is null</exception>
        public List<string> AllowSourceList
        {
            get
            {
                return this.a_allowSourceList;
            }
            set
            {
                this.a_allowSourceList = value ?? throw new ArgumentNullException(nameof(value), "Allow source list parameter is null");
            }
        }
        /// <summary>
        /// true - session cookies will not be used and are not available
        /// </summary>
        public bool NotUsingCookies
        {
            get
            {
                return this.b_notUsingCookies;
            }
            set
            {
                this.b_notUsingCookies = value;
            }
        }
        /// <summary>
        /// true - use session cookies from previous client request, only client
        /// </summary>
        public bool ClientUseCookiesFromPreviousRequest
        {
            get
            {
                return this.b_clientUseCookiesFromPreviousRequest;
            }
            set
            {
                this.b_clientUseCookiesFromPreviousRequest = value;
            }
        }
        /// <summary>
        /// session directory parameter value
        /// </summary>
        /// <exception cref="ArgumentNullException">parameter value is null</exception>
        public string? SessionDirectory
        {
            get
            {
                return this.s_sessionDirectory;
            }
            set
            {
                /* check if root directory value is not empty */
                if ((value == null) || (ForestNET.Lib.Helper.IsStringEmpty(value)))
                {
                    throw new ArgumentNullException(nameof(value), "Session directory parameter is null");
                }

                this.s_sessionDirectory = value;
            }
        }
        /// <summary>
        /// cookie max. age parameter value, if parameter is null -> default value is '10 minutes'
        /// </summary>
        /// <exception cref="ArgumentException">parameter value is greater than '7 days'</exception>
        public ForestNET.Lib.DateInterval? SessionMaxAge
        {
            get
            {
                return this.o_sessionMaxAge;
            }
            set
            {
                if (value != null)
                {
                    if (value.ToDurationInSeconds() > new ForestNET.Lib.DateInterval("P7D").ToDurationInSeconds())
                    {
                        throw new ArgumentException("MaxAge setting for cookies must not be greater than '7 day(s)', but was set to '" + value.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-GB")) + "'");
                    }
                }
                else
                {
                    /* use default max. age of 10 minutes */
                    value = new ForestNET.Lib.DateInterval("PT10M");
                }

                this.o_sessionMaxAge = value;
            }
        }
        /// <summary>
        /// true - session uuid will be refreshed on every request
        /// </summary>
        public bool SessionRefresh
        {
            get
            {
                return this.b_sessionRefresh;
            }
            set
            {
                this.b_sessionRefresh = value;
            }
        }
        /// <summary>
        /// instantiated allow extension list object
        /// </summary>
        /// <exception cref="ArgumentNullException">allow extension list parameter is null</exception>
        public Dictionary<string, string> AllowExtensionList
        {
            get
            {
                return this.a_allowExtensionList;
            }
            set
            {
                this.a_allowExtensionList = value ?? throw new ArgumentNullException(nameof(value), "Allow extension list parameter is null");
            }
        }

        /// <summary>
        /// in encoding object
        /// </summary>
        /// <exception cref="ArgumentNullException">in encoding parameter is null</exception>
        public System.Text.Encoding? InEncoding
        {
            get
            {
                return this.o_inEncoding;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "Incoming encoding parameter is null");
                }

                this.o_inEncoding = value;
            }
        }
        /// <summary>
        /// set in encoding object instance by string name of encoding
        /// </summary>
        /// <exception cref="ArgumentNullException">in encoding parameter is null or invalid value for getting encoding instance</exception>
        public string InEncodingByName
        {
            set
            {
                if ((value == null) || (ForestNET.Lib.Helper.IsStringEmpty(value)))
                {
                    throw new ArgumentNullException(nameof(value), "Incoming encoding parameter is null");
                }

                this.o_inEncoding = System.Text.Encoding.GetEncoding(value);
            }
        }
        /// <summary>
        /// out encoding object
        /// </summary>
        /// <exception cref="ArgumentNullException">out encoding parameter is null</exception>
        public System.Text.Encoding? OutEncoding
        {
            get
            {
                return this.o_outEncoding;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "Outgoing encoding parameter is null");
                }

                this.o_outEncoding = value;
            }
        }
        /// <summary>
        /// set out encoding object instance by string name of encoding
        /// </summary>
        /// <exception cref="ArgumentNullException">out encoding parameter is null or invalid value for getting encoding instance</exception>
        public string OutEncodingByName
        {
            set
            {
                if ((value == null) || (ForestNET.Lib.Helper.IsStringEmpty(value)))
                {
                    throw new ArgumentNullException(nameof(value), "Outgoing encoding parameter is null");
                }

                this.o_outEncoding = System.Text.Encoding.GetEncoding(value);
            }
        }

        /// <summary>
        /// index branch object instance
        /// </summary>
        /// <exception cref="ArgumentNullException">index branch object instance parameter is null</exception>
        public ForestNET.Lib.Net.Https.Dynm.ForestSeed? ForestSeed
        {
            get
            {
                return this.o_forestSeed;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "Index branch object instance is null");
                }

                this.o_forestSeed = value;
            }
        }
        /// <summary>
        /// wsdl object instance
        /// </summary>
        /// <exception cref="ArgumentNullException">wsdl object instance parameter is null</exception>
        public ForestNET.Lib.Net.Https.SOAP.WSDL? WSDL
        {
            get
            {
                return this.o_wsdl;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "WSDL object instance is null");
                }

                this.o_wsdl = value;
            }
        }
        /// <summary>
        /// REST object instance
        /// </summary>
        /// <exception cref="ArgumentNullException">REST object instance parameter is null</exception>
        public ForestNET.Lib.Net.Https.REST.ForestREST? ForestREST
        {
            get
            {
                return this.o_forestREST;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "REST object instance is null");
                }

                this.o_forestREST = value;
            }
        }

        /* Methods */

        /// <summary>
        /// Constructor of configuration class. Using NORMAL mode and SERVER type. All other settings are adjusted by set-property-methods
        /// </summary>
        /// <param name="p_s_domain">determine domain value for tiny https server configuration</param>
        /// <exception cref="ArgumentException">domain parameter value does not start with 'https://'</exception>
        public Config(string p_s_domain) :
            this(p_s_domain, Mode.NORMAL, ForestNET.Lib.Net.Sock.Recv.ReceiveType.SERVER)
        {

        }

        /// <summary>
        /// Constructor of configuration class. Using SERVER type. All other settings are adjusted by set-property-methods
        /// </summary>
        /// <param name="p_s_domain">determine domain value for tiny https server configuration</param>
        /// <param name="p_e_mode">determine mode for tiny https server: NORMAL, DYNAMIC, SOAP or REST</param>
        /// <exception cref="ArgumentException">domain parameter value does not start with 'https://'</exception>
        public Config(string p_s_domain, Mode p_e_mode) :
            this(p_s_domain, p_e_mode, ForestNET.Lib.Net.Sock.Recv.ReceiveType.SERVER)
        {

        }

        /// <summary>
        /// Constructor of configuration class. All other settings are adjusted by set-property-methods
        /// </summary>
        /// <param name="p_s_domain">determine domain value for tiny https server configuration</param>
        /// <param name="p_e_mode">determine mode for tiny https server: NORMAL, DYNAMIC, SOAP or REST</param>
        /// <param name="p_e_receiveType">determine receive type for socket: SOCKET or SERVER</param>
        /// <exception cref="ArgumentException">domain parameter value does not start with 'https://'</exception>
        public Config(string p_s_domain, Mode p_e_mode, ForestNET.Lib.Net.Sock.Recv.ReceiveType p_e_receiveType)
        {
            this.e_mode = p_e_mode;
            this.e_socketReceiveType = p_e_receiveType;
            this.i_servicePoolAmount = 0;
            this.s_domain = string.Empty;
            this.Domain = p_s_domain;
            this.s_host = null;
            this.i_port = 0;
            this.i_timeoutMilliseconds = 1;
            this.i_maxTerminations = -1;
            this.i_maxPayload = ForestNET.Lib.Net.Sock.Task.Task.RECEIVE_MAX_UNKNOWN_AMOUNT_IN_MIB;
            this.i_amountCyclesToleratingDelay = ForestNET.Lib.Net.Sock.Task.Task.AMOUNT_CYCLES_TOLERATING_DELAY;
            this.b_debugNetworkTrafficOn = false;
            this.b_printExceptionStracktrace = false;

            this.s_rootDirectory = null;
            this.a_allowSourceList = [];
            this.b_notUsingCookies = false;
            this.b_clientUseCookiesFromPreviousRequest = true;
            this.s_sessionDirectory = null;
            this.o_sessionMaxAge = null;
            this.b_sessionRefresh = false;

            this.b_checkReachability = false;
            this.i_intervalMilliseconds = 1;
            this.o_sendingSocketInstanceForHttpClient = null;

            this.o_inEncoding = null;
            this.o_outEncoding = null;

            this.o_forestSeed = null;
            this.o_wsdl = null;
            this.o_forestREST = null;

            this.a_allowExtensionList = [];
            this.AllowExtensionList = Config.KNOWN_EXTENSION_LIST;
            this.InEncodingByName = Config.IN_ENCODING;
            this.OutEncodingByName = Config.OUT_ENCODING;
        }
    }
}