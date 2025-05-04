namespace ForestNET.Lib.Net.Sock.Task.Send.Https
{
    /// <summary>
    /// Task class to create a tiny https client. You can create https, soap or rest requests and receive the response to it on properties within Seed instance. Only TCP supported.
    /// Only HTTP/1.0 or HTTP/1.1.
    /// </summary>
    public class TinyHttpsClient : ForestNET.Lib.Net.Sock.Task.Task
    {

        /* Constants */

        public static readonly List<System.Type> ALLOWED_PARAMETER_TYPES = [
            typeof(bool),
            typeof(byte),
            typeof(sbyte),
            typeof(char),
            typeof(float),
            typeof(double),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(string),
            typeof(decimal),
        ];

        /* Fields */

        private int i_proxyPort;

        /* Properties */

        public ForestNET.Lib.Net.Https.Seed? Seed { get; private set; }
        private bool RequestSet { get; set; }
        private ForestNET.Lib.Net.Request.RequestType? RequestType { get; set; }
        public Dictionary<string, Object> RequestParameters { get; private set; }
        private Dictionary<string, string> Attachments { get; set; }
        public string? DownloadFilename { get; set; }
        public bool DownloadFileExtensionNeeded { get; set; }
        public bool UseLog { get; set; }
        public string? ProxyAddress { get; set; }
        public int ProxyPort
        {
            get
            {
                return this.i_proxyPort;
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

                this.i_proxyPort = value;
            }
        }
        public ForestNET.Lib.Net.Request.PostType? ContentType { get; set; }
        public string? AuthenticationUser { get; set; }
        public string? AuthenticationPassword { get; set; }
        public string? ProxyAuthenticationUser { get; set; }
        public string? ProxyAuthenticationPassword { get; set; }
        public int ReturnCode
        {
            get
            {
                return this.Seed?.ResponseHeader.ReturnCode ?? -1;
            }
        }
        public string? ReturnMessage
        {
            get
            {
                return this.Seed?.ResponseHeader.ReturnMessage;
            }
        }
        public byte[]? ResponseBody
        {
            get
            {
                return this.Seed?.ResponseBody;
            }
        }
        public string? Response
        {
            get
            {
                return (this.Seed?.Config.InEncoding ?? System.Text.Encoding.UTF8).GetString(this.Seed?.ResponseBody ?? []);
            }
        }
        public Object? SOAPRequest { private get; set; }
        public Object? SOAPResponse { get; private set; }
        public ForestNET.Lib.Net.Https.SOAP.SoapFault? SOAPFault { get; private set; }
        private ForestNET.Lib.Net.Https.Dynm.Cookie? TempCookie { get; set; }
        public ForestNET.Lib.Net.Sock.Task.Task.PostProgress? PostProgressClient { private get; set; }
        private string? RemoteCertificateNameBkp { get; set; }

        /* Methods */

        /// <summary>
        /// Standard constructor, initializing all values
        /// </summary>
        public TinyHttpsClient() : base()
        {
            this.Seed = null;
            this.RequestSet = false;
            this.RequestType = null;
            this.RequestParameters = [];
            this.Attachments = [];
            this.DownloadFilename = null;
            this.DownloadFileExtensionNeeded = false;
            this.UseLog = false;
            this.ProxyAddress = null;
            this.i_proxyPort = -1;
            this.ContentType = null;
            this.AuthenticationUser = null;
            this.AuthenticationPassword = null;
            this.ProxyAuthenticationUser = null;
            this.ProxyAuthenticationPassword = null;
            this.SOAPRequest = null;
            this.SOAPResponse = null;
            this.SOAPFault = null;
            this.TempCookie = null;
            this.PostProgressDelegate = null;
            this.RemoteCertificateNameBkp = null;
        }

        /// <summary>
        /// Creating tiny https client task instance with all it's settings via configuration parameter
        /// </summary>
        /// <param name="p_o_config">configuration instance parameter</param>
        /// <exception cref="ArgumentNullException">configuration instance parameter is null, or domain parameter from configuration is null or empty</exception>
        public TinyHttpsClient(ForestNET.Lib.Net.Https.Config p_o_config) : base(ForestNET.Lib.Net.Sock.Type.TCP_TLS_CLIENT)
        {
            this.Seed = new ForestNET.Lib.Net.Https.Seed(p_o_config);
            this.RequestSet = false;
            this.RequestType = null;
            this.RequestParameters = [];
            this.Attachments = [];
            this.DownloadFilename = null;
            this.DownloadFileExtensionNeeded = false;
            this.UseLog = false;
            this.ProxyAddress = null;
            this.i_proxyPort = -1;
            this.ContentType = null;
            this.AuthenticationUser = null;
            this.AuthenticationPassword = null;
            this.ProxyAuthenticationUser = null;
            this.ProxyAuthenticationPassword = null;
            this.SOAPRequest = null;
            this.SOAPResponse = null;
            this.SOAPFault = null;
            this.TempCookie = null;
            this.PostProgressDelegate = null;
            this.RemoteCertificateNameBkp = null;

            /* set Task debug network traffic flag from configuration parameter */
            this.DebugNetworkTrafficOn = this.Seed.Config.DebugNetworkTrafficOn;
        }

        /// <summary>
        /// Method to clone this socket task with another socket task instance
        /// </summary>
        /// <param name="p_o_sourceTask">another socket task instance as source for all it's parameters and settings</param>
        public override void CloneFromOtherTask(ForestNET.Lib.Net.Sock.Task.Task p_o_sourceTask)
        {
            this.CloneBasicProperties(p_o_sourceTask);

            /* ignore exceptions if a property of source task has no valid value, we will keep it null */
            try { this.Seed = ((TinyHttpsClient)p_o_sourceTask).Seed; } catch (Exception) { /* NOP */ }
        }

        /// <summary>
        /// Set destination address, primarily this method is for SOAP mode as the request type will be auto set to POST
        /// </summary>
        /// <param name="p_s_url">destination address</param>
        /// <exception cref="ArgumentNullException">destination address parameter is null or empty</exception>
        /// <exception cref="ArgumentException">no Seed instance available</exception>
        /// <exception cref="FormatException">issue local host name could not be resolved into an address</exception>
        public void SetRequest(string p_s_url)
        {
            this.SetRequest(p_s_url, null);
        }

        /// <summary>
        /// Set destination address, primarily this method is for SOAP mode as the request type will be auto set to POST, where SOAP request object will be set as well
        /// </summary>
        /// <param name="p_s_url">destination address</param>
        /// <param name="p_o_soapRequest">SOAP request object which can be used with an XML instance to create a xml-valid SOAP request</param>
        /// <exception cref="InvalidOperationException">client is not in SOAP mode</exception>
        /// <exception cref="ArgumentNullException">destination address parameter is null or empty</exception>
        /// <exception cref="ArgumentException">no Seed instance available</exception>
        /// <exception cref="FormatException">issue local host name could not be resolved into an address</exception>
        public void SetSOAPRequest(string p_s_url, Object p_o_soapRequest)
        {
            /* check if Seed instance is available */
            if (this.Seed == null)
            {
                throw new ArgumentException("Configuration for tiny https client is not specified");
            }

            /* check if our client is in SOAP mode */
            if (this.Seed.Config.Mode != ForestNET.Lib.Net.Https.Mode.SOAP)
            {
                throw new InvalidOperationException("Mode '" + this.Seed.Config.Mode + "' of " + ForestNET.Lib.Net.Https.RequestHeader.CLIENT + " is not '" + ForestNET.Lib.Net.Https.Mode.SOAP + "'");
            }

            /* set SOAP request object */
            this.SOAPRequest = p_o_soapRequest;
            /* set other request settings */
            this.SetRequest(p_s_url);
        }

        /// <summary>
        /// Set destination address and request type of tiny https client
        /// </summary>
        /// <param name="p_s_url">destination address</param>
        /// <param name="p_e_requestType">request type or http method</param>
        /// <exception cref="ArgumentNullException">destination address or request type parameter is null or empty</exception>
        /// <exception cref="ArgumentException">no Seed instance available or request type parameter not implemented</exception>
        /// <exception cref="FormatException">issue local host name could not be resolved into an address</exception>
        public void SetRequest(string p_s_url, ForestNET.Lib.Net.Request.RequestType? p_e_requestType)
        {
            /* check if Seed instance is available */
            if (this.Seed == null)
            {
                throw new ArgumentException("Configuration for tiny https client is not specified");
            }

            /* check destination address parameter */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_url))
            {
                throw new ArgumentNullException(nameof(p_s_url), "URL parameter is null or empty");
            }

            /* re-initialize values */
            this.Seed = new ForestNET.Lib.Net.Https.Seed(this.Seed.Config);
            this.RequestParameters = [];
            this.Attachments = [];
            this.DownloadFilename = null;
            this.DownloadFileExtensionNeeded = false;
            this.ContentType = null;
            this.AuthenticationUser = null;
            this.AuthenticationPassword = null;
            this.RemoteCertificateNameBkp = null;

            /*
			this.ProxyAddress = null;
            this.i_proxyPort = -1;
            this.ProxyAuthenticationUser = null;
            this.ProxyAuthenticationPassword = null;
            this.SOAPRequest = null;
            this.SOAPResponse = null;
            this.SOAPFault = null;
            this.TempCookie = null;
			this.PostProgressDelegate = null;
            */

            /* check request type parameter, auto set on POST if our task instance is in SOAP mode */
            if ((p_e_requestType == null) && (this.Seed.Config.Mode == ForestNET.Lib.Net.Https.Mode.SOAP))
            {
                p_e_requestType = ForestNET.Lib.Net.Request.RequestType.POST;
            }
            else if (p_e_requestType == null)
            {
                throw new ArgumentNullException(nameof(p_e_requestType), "Request type parameter is null");
            }

            this.RequestType = p_e_requestType;
            this.Seed.RequestHeader.MethodByRequestType = p_e_requestType ?? throw new ArgumentNullException(nameof(p_e_requestType), "Request type parameter is null");

            int i_port;

            /* check if destination address starts correctly */
            if (p_s_url.StartsWith("http://"))
            { /* remove 'http://' from start */
                this.Seed.RequestHeader.Protocol = "http://";
                p_s_url = p_s_url.Substring(7);
                i_port = 80;
            }
            else if (p_s_url.StartsWith("https://"))
            { /* remove 'https://' from start */
                this.Seed.RequestHeader.Protocol = "https://";
                p_s_url = p_s_url.Substring(8);
                i_port = 443;
            }
            else
            {
                throw new ArgumentException("URL parameter must start with 'http://' or 'https://'");
            }

            string s_requestPath = "/";
            string s_host;

            /* get request path and host part */
            if (p_s_url.Contains('/'))
            {
                s_requestPath = p_s_url.Substring(p_s_url.IndexOf("/"));
                s_host = p_s_url.Substring(0, p_s_url.IndexOf("/"));
            }
            else
            {
                s_host = p_s_url;
            }

            /* read optional port value within destination address */
            if (s_host.Contains(':'))
            {
                /* split by ':' */
                string[] a_foo = s_host.Split(":");
                /* set host part */
                s_host = a_foo[0];

                /* check if port value is an integer */
                if (!ForestNET.Lib.Helper.IsInteger(a_foo[1]))
                {
                    throw new ArgumentException("Invalid value '" + a_foo[1] + "' for destination port, must be an integer number between 1 and 65535");
                }
                else
                {
                    /* store port value */
                    i_port = int.Parse(a_foo[1]);

                    /* check port min. value */
                    if (i_port < 1)
                    {
                        throw new ArgumentException("Port must be at least '1', but was set to '" + i_port + "'");
                    }

                    /* check port max. value */
                    if (i_port > 65535)
                    {
                        throw new ArgumentException("Port must be lower equal '65535', but was set to '" + i_port + "'");
                    }
                }
            }

            /* overwrite request settings */
            this.Seed.RequestHeader.RequestPath = s_requestPath;
            this.Seed.RequestHeader.Host = s_host;
            this.Seed.RequestHeader.Port = i_port;
            this.Seed.RequestHeader.ConnectionClose = true;

            /* if we want to set SNI with host value if RemoteCertificateName equals 'INSERT_HOST_FOR_TLS' */
            if ((this.Seed.Config.SendingSocketInstance != null) && ((this.Seed.Config.SendingSocketInstance.RemoteCertificateName ?? string.Empty).Equals("INSERT_HOST_FOR_TLS")))
            {
                this.Seed.Config.SendingSocketInstance.ServerNameIndicator = s_host;
            }

            /* resolve host ip */
            s_host = System.Net.IPAddress.Parse(System.Net.Dns.GetHostAddresses(s_host)[0].ToString()).ToString();

            /* override destination settings of our sending socket instance */
            this.Seed.Config.SendingSocketInstance?.OverrideDestinationAddress(s_host, i_port, false, false);

            /* inject cookies from last request */
            if ((this.Seed.Config.ClientUseCookiesFromPreviousRequest) && (this.TempCookie != null))
            {
                this.Seed.RequestHeader.Cookie = this.TempCookie.ClientCookieToString();
            }

            /* now request is set and can be called with executeRequest() */
            this.RequestSet = true;
        }

        /// <summary>
        /// Adds request parameter to web request, if parameter type is an allowed type
        /// </summary>
        /// <param name="p_s_parameterName">type of request parameter, e.g. string, bool, or double</param>
        /// <param name="p_o_parameterValue">value of request parameter</param>
        /// <exception cref="ArgumentException">parameter type is not an allowed type</exception>
        public void AddRequestParameter(string p_s_parameterName, Object p_o_parameterValue)
        {
            /* check if object value has allowed type */
            if (!TinyHttpsClient.ALLOWED_PARAMETER_TYPES.Contains(p_o_parameterValue.GetType()))
            {
                throw new ArgumentException("Parameter[" + p_s_parameterName + "] with type[" + p_o_parameterValue.GetType().FullName + "] is not an allowed parameter type: " + string.Join(", ", TinyHttpsClient.ALLOWED_PARAMETER_TYPES));
            }

            /* add request parameter name and value */
            this.RequestParameters.Add(p_s_parameterName, p_o_parameterValue);

            if (this.UseLog) ForestNET.Lib.Global.ILogConfig("added request parameter: '" + p_s_parameterName + "' = '" + p_o_parameterValue + "'");
        }

        /// <summary>
        /// Adds filepath to web request which will be attached for web request execution
        /// </summary>
        /// <param name="p_s_parameterName">file name for request</param>
        /// <param name="p_s_filePath">local file path of attachment</param>
        public void AddAttachement(string p_s_parameterName, string p_s_filePath)
        {
            /* add request parameter attachment name and filepath */
            this.Attachments.Add(p_s_parameterName, p_s_filePath);

            if (this.UseLog) ForestNET.Lib.Global.ILogConfig("added attachment to request: '" + p_s_parameterName + "' = '" + p_s_filePath + "'");
        }

        /// <summary>
        /// Execute a request, by running sending socket instance.
        /// Optional proxy settings available for execution.
        /// </summary>
        /// <exception cref="InvalidOperationException">flag not set that the request can be executed, you must first call setRequest(...), or 	destination host is not reachable</exception>
        /// <exception cref="FormatException">issue local host name could not be resolved into an address</exception>
        public async System.Threading.Tasks.Task ExecuteRequest()
        {
            /* check if Seed instance is available */
            if (this.Seed == null)
            {
                throw new ArgumentException("Configuration for tiny https client is not specified");
            }

            /* check if all necessary request information has been set */
            if (!this.RequestSet)
            {
                throw new InvalidOperationException("Missing request parameters, e.g. host address and port. Please use the setRequest(...) method.");
            }

            /* check for authentication */
            if ((!ForestNET.Lib.Helper.IsStringEmpty(this.AuthenticationUser)) && (!ForestNET.Lib.Helper.IsStringEmpty(this.AuthenticationPassword)))
            {
                /* set authentication user and password as base64 for request */
                this.Seed.RequestHeader.Authorization = "Basic " + Convert.ToBase64String(
                    System.Text.ASCIIEncoding.ASCII.GetBytes(
                        $"{this.AuthenticationUser}:{this.AuthenticationPassword}"
                    )
                );

                if (this.UseLog) ForestNET.Lib.Global.ILogConfig("created base64 authentication string and added it to web request properties");
            }

            /* check if we have proxy address and port settings */
            if ((this.ProxyAddress != null) && (!ForestNET.Lib.Helper.IsStringEmpty(this.ProxyAddress)) && (this.ProxyPort > 0))
            {
                string s_proxyHost;

                try
                {
                    /* resolve proxy host ip */
                    s_proxyHost = System.Net.IPAddress.Parse(System.Net.Dns.GetHostAddresses(new Uri(this.ProxyAddress).Host)[0].ToString()).ToString();
                }
                catch (Exception)
                {
                    /* just check for valid ip address */
                    s_proxyHost = System.Net.IPAddress.Parse(this.ProxyAddress).ToString();
                }

                /* prepare communication with proxy server */
                this.Seed.Config.SendingSocketInstance?.OverrideDestinationAddress(s_proxyHost, this.ProxyPort, true, false);

                if (this.UseLog) ForestNET.Lib.Global.ILogConfig("changed socket destination to proxy address: '" + this.ProxyAddress + ":" + this.ProxyPort + "'");

                /* store remote certificate name as backup for proxy CONNECT command */
                if (this.Seed.Config.SendingSocketInstance != null)
                {
                    this.RemoteCertificateNameBkp = this.Seed.Config.SendingSocketInstance.RemoteCertificateName;
                    this.Seed.Config.SendingSocketInstance.RemoteCertificateName = null;
                }
            }

            /* check if sending socket instance is available */
            if (this.Seed.Config.SendingSocketInstance == null)
            {
                throw new NullReferenceException("Sending socket instance is null");
            }

            /* run socket sending instance */
            await this.Seed.Config.SendingSocketInstance.Run();
        }

        /// <summary>
        /// runTask method for sending https/soap/rest request
        /// </summary>
        /// <exception cref="Exception">any exception of implementation that could happen will be caught by abstract Task class, see details in protocol methods in de.forestj.lib.net.sock.task.Task</exception>
        public override async System.Threading.Tasks.Task RunTask()
        {
            /* check if Seed instance is availalbe */
            if (this.Seed == null)
            {
                throw new ArgumentException("Configuration for tiny https client is not specified");
            }

            /* only TCP supported */
            if ((this.Type == ForestNET.Lib.Net.Sock.Type.TCP_CLIENT) || (this.Type == ForestNET.Lib.Net.Sock.Type.TCP_TLS_CLIENT))
            {
                try
                {
                    /* do optional proxy routine */
                    await this.OptionalProxy();

                    byte[]? a_requestData = null;

                    /* check mode and use individual routine to prepare request */
                    if (this.Seed.Config.Mode == ForestNET.Lib.Net.Https.Mode.NORMAL)
                    {
                        /* prepare https request */
                        a_requestData = this.HandleNormal();
                    }
                    else if (this.Seed.Config.Mode == ForestNET.Lib.Net.Https.Mode.DYNAMIC)
                    {
                        throw new InvalidOperationException("Client mode 'dynamic' is not implemented");
                    }
                    else if (this.Seed.Config.Mode == ForestNET.Lib.Net.Https.Mode.SOAP)
                    {
                        /* prepare SOAP request */
                        a_requestData = this.HandleSOAP();
                    }
                    else if (this.Seed.Config.Mode == ForestNET.Lib.Net.Https.Mode.REST)
                    {
                        /* prepare REST request */
                        a_requestData = this.HandleREST();
                    }

                    /* check if we have prepared request data to sent */
                    if (a_requestData == null)
                    {
                        throw new InvalidOperationException("Request data is null, cannot send http(s) request");
                    }

                    if (this.UseLog) ForestNET.Lib.Global.ILogFine("sending https/SOAP/REST request to destination");

                    /* sending https/SOAP/REST request to destination */
                    await this.SendBytes(a_requestData, a_requestData.Length);

                    if (this.UseLog) ForestNET.Lib.Global.ILogFine("sent https/SOAP/REST request to destination");

                    int i_returnCode = 500;

                    if (this.UseLog) ForestNET.Lib.Global.ILogFine("receive https/SOAP/REST response");

                    /* use individual routine to handle response */
                    if (this.Seed.Config.Mode == ForestNET.Lib.Net.Https.Mode.NORMAL)
                    {
                        /* handle https response */
                        i_returnCode = await this.HandleNormalResponse();
                    }
                    else if (this.Seed.Config.Mode == ForestNET.Lib.Net.Https.Mode.DYNAMIC)
                    {
                        throw new InvalidOperationException("Client mode 'dynamic' is not implemented");
                    }
                    else if (this.Seed.Config.Mode == ForestNET.Lib.Net.Https.Mode.SOAP)
                    {
                        /* handle SOAP response */
                        i_returnCode = await this.HandleSOAPResponse();
                    }
                    else if (this.Seed.Config.Mode == ForestNET.Lib.Net.Https.Mode.REST)
                    {
                        /* handle REST response */
                        i_returnCode = await this.HandleRESTResponse();
                    }

                    /* set return code from handled response */
                    this.Seed.ResponseHeader.ReturnCode = i_returnCode;
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.ILogSevere("Exception runTask: " + o_exc);

                    if (this.Seed.Config.PrintExceptionStracktrace)
                    {
                        ForestNET.Lib.Global.LogException(o_exc);
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
        /// do optional proxy routine
        /// </summary>
        private async System.Threading.Tasks.Task OptionalProxy()
        {
            /* check if Seed instance is availalbe */
            if (this.Seed == null)
            {
                throw new ArgumentException("Configuration for tiny https client is not specified");
            }

            /* only do optional proxy routine if we have a proxy address and port */
            if ((!ForestNET.Lib.Helper.IsStringEmpty(this.ProxyAddress)) && (this.ProxyPort > 0))
            {
                if (this.UseLog) ForestNET.Lib.Global.ILogFine("prepare proxy CONNECT header");

                /* prepare CONNECT header */
                string s_proxyConnectHeader = "";
                s_proxyConnectHeader += "CONNECT " + this.Seed.RequestHeader.Host + ":" + this.Seed.RequestHeader.Port + " HTTP/1.1" + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK;
                s_proxyConnectHeader += "Host: " + this.Seed.RequestHeader.Host + ":" + this.Seed.RequestHeader.Port + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK;

                /* check for authentication for proxy */
                if ((!ForestNET.Lib.Helper.IsStringEmpty(this.ProxyAuthenticationUser)) && (!ForestNET.Lib.Helper.IsStringEmpty(this.ProxyAuthenticationPassword)))
                {
                    /* set authentication user and password as base64 for CONNECT header */
                    s_proxyConnectHeader += "Proxy-Authorization: Basic " +
                    Convert.ToBase64String(
                        System.Text.ASCIIEncoding.ASCII.GetBytes(
                            $"{this.ProxyAuthenticationUser}:{this.ProxyAuthenticationPassword}"
                        )
                    )
                    + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK;

                    if (this.UseLog) ForestNET.Lib.Global.ILogFine("created base64 proxy authentication string and added it to proxy request");
                }

                /* add additional line break */
                s_proxyConnectHeader += ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK;

                /* use outgoing encoding configuration */
                s_proxyConnectHeader = (this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).GetString(System.Text.Encoding.UTF8.GetBytes(s_proxyConnectHeader));
                byte[] a_proxyConnectHeader = (this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).GetBytes(s_proxyConnectHeader);

                /* create proxy request byte array */
                byte[] a_proxyRequest = new byte[a_proxyConnectHeader.Length];
                int i;

                for (i = 0; i < a_proxyConnectHeader.Length; i++)
                {
                    a_proxyRequest[i] = a_proxyConnectHeader[i];
                }

                if (this.UseLog) ForestNET.Lib.Global.ILogFine("sending request to proxy");

                /* send proxy CONNECT request */
                await this.SendBytes(a_proxyRequest, a_proxyRequest.Length);

                if (this.UseLog) ForestNET.Lib.Global.ILogFine("sent request to proxy");

                byte[]? a_responseData = null;

                if (this.UseLog) ForestNET.Lib.Global.ILogFine("receive proxy response");

                /* receive proxy CONNECT response */
                try
                {
                    a_responseData = await this.ReceiveBytes(-1);
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.ILogSevere("500 Internal Server Error: Exception while receiving data; " + o_exc);
                    this.Seed.ResponseHeader.ReturnCode = 500;
                }

                /* check if we've got a response */
                if ((a_responseData == null) || (a_responseData.Length < 1))
                {
                    ForestNET.Lib.Global.ILogWarning("204 No Content: HTTP request from proxy is null or empty");
                    this.Seed.ResponseHeader.ReturnCode = 204;
                }

                /* check max. payload of response */
                if ((a_responseData != null) && (a_responseData.Length >= this.Seed.Config.MaxPayload * 1024 * 1024))
                {
                    ForestNET.Lib.Global.ILogWarning("413 Payload Too Large: Reached max. payload of '" + ForestNET.Lib.Helper.FormatBytes(this.Seed.Config.MaxPayload * 1024 * 1024) + "'");
                    this.Seed.ResponseHeader.ReturnCode = 413;
                }

                /* convert response byte array to lower case string */
                string s_foo = System.Text.Encoding.UTF8.GetString(a_responseData ?? []).ToLower();

                /* we expect code '200', message 'ok' and the text 'connection established' */
                if (!((s_foo.Contains("200")) && ((s_foo.Contains("ok")) || (s_foo.Contains("connection established")))))
                {
                    ForestNET.Lib.Global.ILogSevere("500 Internal Server Error: Proxy server returns unexpected answer; " + System.Text.Encoding.UTF8.GetString(a_responseData ?? []).Trim());
                    this.Seed.ResponseHeader.ReturnCode = 500;
                    this.Seed.ResponseHeader.ReturnMessage = "Proxy server returns unexpected answer; " + System.Text.Encoding.UTF8.GetString(a_responseData ?? []).Trim();
                    return;
                }

                if (this.UseLog) ForestNET.Lib.Global.ILogFine("proxy response: " + System.Text.Encoding.UTF8.GetString(a_responseData ?? []));

                if (this.Seed.Config.SendingSocketInstance != null)
                {
                    /* restore remote certificate name after proxy response */
                    this.Seed.Config.SendingSocketInstance.RemoteCertificateName = this.RemoteCertificateNameBkp;

                    /* upgrade current socket(proxy socket) to a TLS socket connection */
                    this.Seed.Config.SendingSocketInstance.UpgradeCurrentSocketToTLS(this.Seed.RequestHeader.Host, this.Seed.RequestHeader.Port, true);
                }
            }
        }

        /// <summary>
        /// prepare https request
        /// </summary>
        /// <exception cref="ArgumentException">invalid download filename location or invalid content type</exception>
        /// <exception cref="System.IO.IOException">issues to check download filename location, url encoding data or reading all bytes from an attachment</exception>
        private byte[] HandleNormal()
        {
            /* check if Seed instance is availalbe */
            if (this.Seed == null)
            {
                throw new ArgumentException("Configuration for tiny https client is not specified");
            }

            /* dynamic byte array for POST/PUT/DELETE request */
            List<byte> a_postBytes = [];

            /* check if we have any request parameters and web request type is 'GET' or 'DOWNLOAD' */
            if ((this.RequestParameters.Count > 0) && ((this.RequestType == ForestNET.Lib.Net.Request.RequestType.GET) || (this.RequestType == ForestNET.Lib.Net.Request.RequestType.DOWNLOAD)))
            {
                if (this.UseLog) ForestNET.Lib.Global.ILogFine("gather request parameters with request type is 'GET' or 'DOWNLOAD'");

                /* string variable for request parameters */
                string s_requestParameters = "";

                /* iterate all request parameters with key->value pairs */
                foreach (KeyValuePair<string, Object> m_requestParameter in this.RequestParameters)
                {
                    /* add key->value pair request parameter url encoded to variable */
                    s_requestParameters += System.Web.HttpUtility.UrlEncode(m_requestParameter.Key, this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8) + "=" + System.Web.HttpUtility.UrlEncode(m_requestParameter.Value.ToString(), this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8) + "&";
                }

                /* if request parameters ends with '&' */
                if (s_requestParameters.EndsWith("&"))
                {
                    /* remove last '&' character from request parameters */
                    s_requestParameters = s_requestParameters.Substring(0, s_requestParameters.Length - 1);
                }

                if (this.UseLog) ForestNET.Lib.Global.ILogFine("request parameters: '" + s_requestParameters + "'");

                /* add request parameters for type 'GET' or 'DOWNLOAD' to request path */
                this.Seed.RequestHeader.RequestPath = this.Seed.RequestHeader.RequestPath + "?" + s_requestParameters;
            }

            if (this.RequestType == ForestNET.Lib.Net.Request.RequestType.DOWNLOAD)
            { /* web request type 'DOWNLOAD' */
                /* check if download filename location is set */
                if (ForestNET.Lib.Helper.IsStringEmpty(this.DownloadFilename))
                {
                    throw new ArgumentException("Please specify a download filename location");
                }

                /* check if download filename location exists */
                if (!ForestNET.Lib.IO.File.FolderExists((this.DownloadFilename?.Substring(0, this.DownloadFilename.LastIndexOf(ForestNET.Lib.IO.File.DIR))) ?? ""))
                {
                    throw new System.IO.IOException("Folder of download filename location[" + this.DownloadFilename + "] does not exist. Please create the download folder location before execution");
                }

                if (this.DownloadFileExtensionNeeded)
                { /* check if download filename location has a file extension */
                    if (!ForestNET.Lib.IO.File.HasFileExtension(this.DownloadFilename ?? ""))
                    {
                        throw new ArgumentException("Download filename location[" + this.DownloadFilename + "] must have a valid file extension");
                    }
                }
            }
            else if (
                (this.RequestType == ForestNET.Lib.Net.Request.RequestType.POST) ||
                (this.RequestType == ForestNET.Lib.Net.Request.RequestType.PUT) ||
                (
                    (this.RequestType == ForestNET.Lib.Net.Request.RequestType.DELETE) && (this.ContentType != null)
                )
            )
            { /* web request type 'POST', 'PUT' or 'DELETE', but only 'DELETE' if content type is not null */
                /* random value for https file upload protocol */
                string s_boundary = "------------" + Environment.TickCount.ToString("X4");

                if (this.UseLog) ForestNET.Lib.Global.ILogFine("created random value(boundary) for http file upload protocol: '" + s_boundary + "'");

                /* check if content type is not null, necessary for type 'POST', 'PUT' */
                if (this.ContentType == null)
                {
                    throw new ArgumentException("No content type for '" + this.RequestType + "' web request specified");
                }

                /* check if we have any attachments for web request 'POST' */
                if (this.Attachments.Count > 0)
                {
                    /* set content type with random boundary */
                    this.Seed.RequestHeader.ContentType = ForestNET.Lib.Net.Request.PostType.HTMLATTACHMENTS + "; boundary=" + s_boundary;
                }
                else
                {
                    /* set normal content type */
                    this.Seed.RequestHeader.ContentType = this.ContentType.ToString() ?? throw new NullReferenceException("content type is null or empty");
                }

                if (this.UseLog) ForestNET.Lib.Global.ILogFine("set content type for web request '" + this.RequestType + "': '" + this.Seed.RequestHeader.ContentType + "'");

                /* ***** */
                /* start - preparing post data for web request 'POST' */
                /* ***** */

                if (this.UseLog) ForestNET.Lib.Global.ILogFine("start preparing post data");

                /* attachments available */
                if (this.Attachments.Count > 0)
                {
                    if (this.UseLog) ForestNET.Lib.Global.ILogFine("iterate all request parameters with key->value pairs");

                    /* iterate all request parameters with key->value pairs */
                    foreach (KeyValuePair<string, Object> m_requestParameter in this.RequestParameters)
                    {
                        /* start new post data with two hyphens, boundary and line break */
                        ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList((this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).GetBytes("--" + s_boundary + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK), ref a_postBytes);
                        /* add content disposition and parameter name */
                        ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList((this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).GetBytes("Content-Disposition: form-data; name=\"" + m_requestParameter.Key + "\"" + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK), ref a_postBytes);
                        /* add post data value and complete post data element */
                        ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList((this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).GetBytes(ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK + m_requestParameter.Value.ToString() + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK), ref a_postBytes);

                        if (this.UseLog) ForestNET.Lib.Global.ILogFiner("request parameter '" + m_requestParameter.Key + "' added");
                    }

                    if (this.UseLog) ForestNET.Lib.Global.ILogFine("iterate all attachments with key->value pairs");

                    foreach (KeyValuePair<string, string> m_attachment in this.Attachments)
                    {
                        if ((ForestNET.Lib.IO.File.Exists(m_attachment.Value)) && (ForestNET.Lib.IO.File.IsFile(m_attachment.Value)))
                        {
                            /* start new post data with two hyphens, boundary and line break */
                            ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList((this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).GetBytes("--" + s_boundary + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK), ref a_postBytes);
                            /* add content disposition, parameter name and file name */
                            ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList((this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).GetBytes("Content-Disposition: form-data; name=\"" + m_attachment.Key + "\"; filename=\"" + System.IO.Path.GetFileName(m_attachment.Value) + "\"" + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK), ref a_postBytes);
                            /* add content type by iterating known extension list with file name */
                            string s_fileGuessedContentType = string.Empty;

                            /* iterate known extension list */
                            foreach (KeyValuePair<string, string> o_knownExtension in ForestNET.Lib.Net.Https.Config.KNOWN_EXTENSION_LIST)
                            {
                                if (System.IO.Path.GetFileName(m_attachment.Value).EndsWith(o_knownExtension.Key))
                                {
                                    s_fileGuessedContentType = o_knownExtension.Value;
                                }
                            }

                            /* check if we found a valid extensions */
                            if (ForestNET.Lib.Helper.IsStringEmpty(s_fileGuessedContentType))
                            {
                                throw new ArgumentException("Extension of file '" + System.IO.Path.GetFileName(m_attachment.Value) + "' is not valid");
                            }

                            ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList((this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).GetBytes("Content-Type: " + s_fileGuessedContentType + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK), ref a_postBytes);
                            /* add line break and flush print writer which indicates that file data is incoming */
                            ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList((this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).GetBytes(ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK), ref a_postBytes);
                            /* copy file stream into dynamic byte list */
                            ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList(ForestNET.Lib.IO.File.ReadAllBytes(m_attachment.Value) ?? [], ref a_postBytes);
                            /* add line break and indicate end of boundary */
                            ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList((this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).GetBytes(ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK), ref a_postBytes);

                            if (this.UseLog) ForestNET.Lib.Global.ILogFiner("attachment '" + m_attachment.Key + "' added with guessed content type: '" + s_fileGuessedContentType + "'");
                        }
                        else
                        {
                            ForestNET.Lib.Global.ILogWarning("attachment file '" + m_attachment.Key + "' does not exist or is not a file: '" + m_attachment.Value + "'");
                        }
                    }

                    /* add end of HTMLATTACHMENTS(multipart/form-data) */
                    ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList((this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).GetBytes("--" + s_boundary + "--" + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK), ref a_postBytes);
                }
                else
                { /* no attachments available */
                    /* check if we have any request parameters */
                    if (this.RequestParameters.Count > 0)
                    {
                        if (this.UseLog) ForestNET.Lib.Global.ILogFine("iterate all request parameters with key->value pairs");

                        string s_postBody = "";

                        /* check amount of post request parameters */
                        if (this.RequestParameters.Count > 0)
                        {
                            /* iterate all request parameters with key->value pairs */
                            foreach (KeyValuePair<string, Object> m_requestParameter in this.RequestParameters)
                            {
                                /* add parameter name and value to post body */
                                s_postBody += System.Web.HttpUtility.UrlEncode(m_requestParameter.Key, this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8) + "=" + System.Web.HttpUtility.UrlEncode(m_requestParameter.Value.ToString(), this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8) + "&";

                                if (this.UseLog) ForestNET.Lib.Global.ILogFiner("request parameter '" + m_requestParameter.Key + "' added");
                            }

                            /* remove last '&' */
                            s_postBody = s_postBody.Substring(0, (s_postBody.Length - 1));

                            /* add post body data to dynamic byte list */
                            ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList((this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).GetBytes(s_postBody), ref a_postBytes);
                        }
                        else
                        {
                            /* add empty post body data to dynamic byte list */
                            ForestNET.Lib.Helper.AddStaticByteArrayToDynamicByteList((this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).GetBytes(s_postBody + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK), ref a_postBytes);
                        }

                        if (this.UseLog) ForestNET.Lib.Global.ILogFine("added all request parameters to post body data");
                    }
                }

                if (this.UseLog) ForestNET.Lib.Global.ILogFine("finished preparing post data");

                if (this.UseLog) ForestNET.Lib.Global.ILogFine("update content-length to '" + a_postBytes.Count + "'");

                this.Seed.RequestHeader.ContentLength = a_postBytes.Count;

                /* *** */
                /* end - preparing post data of web request 'POST' */
                /* *** */
            }

            /* ***** */
            /* start - preparing complete web request as byte array */
            /* ***** */

            if (this.UseLog) ForestNET.Lib.Global.ILogFine("add request header to https/REST request");

            /* create string temporary variable for https request header */
            byte[] a_requestHeader = (this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).GetBytes(this.Seed.RequestHeader.ToString());

            /* create byte array for https request, including header and body length */
            byte[] a_foo = new byte[a_requestHeader.Length + ((a_postBytes.Count > 0) ? a_postBytes.Count : 0)];
            int i;

            /* copy each byte from request header to byte array */
            for (i = 0; i < a_requestHeader.Length; i++)
            {
                a_foo[i] = a_requestHeader[i];
            }

            if (a_postBytes.Count > 0)
            {
                if (this.UseLog) ForestNET.Lib.Global.ILogFine("add body data to https/REST request");

                /* copy each byte from post body to byte array */
                for (int j = 0; j < a_postBytes.Count; j++)
                {
                    a_foo[j + i] = a_postBytes[j];
                }
            }

            /* ***** */
            /* end - preparing complete web request as byte array */
            /* ***** */

            /* return prepared https request as byte array for sending with TCP */
            return a_foo;
        }

        /// <summary>
        /// prepare SOAP request
        /// </summary>
        /// <exception cref="ArgumentNullException">object for SOAP request or wsdl instance are not available</exception>
        /// <exception cref="ArgumentException">content-length must be a positive integer</exception>
        /// <exception cref="System.IO.IOException">exception while encoding sending SOAP xml data</exception>
        private byte[] HandleSOAP()
        {
            /* check if Seed instance is availalbe */
            if (this.Seed == null)
            {
                throw new ArgumentException("Configuration for tiny https client is not specified");
            }

            /* check if object for SOAP request is available */
            if (this.SOAPRequest == null)
            {
                throw new ArgumentNullException("No SOAP request object available for request");
            }

            /* check if wsdl instance is available in configuration */
            if (this.Seed.Config.WSDL == null)
            {
                throw new ArgumentNullException("No WSDL configuration enabled for SOAP protocol");
            }

            string s_soapXml;

            /* encode object for SOAP request to xml data */
            try
            {
                if (this.UseLog) ForestNET.Lib.Global.ILogFine("encoding object for SOAP request to xml data");

                s_soapXml = this.Seed.Config.WSDL.Schema.XmlEncode(this.SOAPRequest);
                s_soapXml = new System.Text.RegularExpressions.Regex("<\\?(.*?)\\?>").Replace(s_soapXml, "");

                if (this.UseLog) ForestNET.Lib.Global.ILogFine("encoded object for SOAP request to xml data");
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.ILogSevere("500 Internal Server Error: Exception while encoding sending SOAP xml data; " + o_exc);
                throw new System.IO.IOException("Exception while encoding sending SOAP xml data; " + o_exc);
            }

            /* wsdl types schema has target namespace */
            if (!ForestNET.Lib.Helper.IsStringEmpty(this.Seed.Config.WSDL.Schema.TargetNamespace))
            {
                /* adding target namespace to xml data */
                s_soapXml = s_soapXml.Substring(0, s_soapXml.IndexOf(">")) + " xmlns=\"" + this.Seed.Config.WSDL.Schema.TargetNamespace + "\"" + s_soapXml.Substring(s_soapXml.IndexOf(">"));
            }

            /* no support for namespace initial on client side, we have no setting or property where we hold this information */
            /*} else if ( (!ForestNET.Lib.Helper.IsStringEmpty(this.Seed.Config.WSDL.Schema.TargetNamespace)) && (!ForestNET.Lib.Helper.IsStringEmpty( NamespaceInitial )) ) {
				/* wsdl types schema has target namespace and namespace initial */

            /* adding target namespace and namespace initial to each xml-tag to xml data */
            /*s_soapXml = s_soapXml.Substring(0, s_soapXml.IndexOf(">")) + " xmlns:" + this.Seed.Config.WSDL.Schema.NamespaceVariable + "=\"" + this.Seed.Config.WSDL.Schema.TargetNamespace + "\"" + s_soapXml.Substring(s_soapXml.IndexOf(">"));

            s_soapXml = new System.Text.RegularExpressions.Regex("</").Replace(s_soapXml, "</" + this.Seed.Config.WSDL.Schema.NamespaceVariable + ":");
            s_soapXml = new System.Text.RegularExpressions.Regex("<").Replace(s_soapXml, "<" + this.Seed.Config.WSDL.Schema.NamespaceVariable + ":");
            s_soapXml = new System.Text.RegularExpressions.Regex("<" + this.Seed.Config.WSDL.Schema.NamespaceVariable + ":" + "/" + this.Seed.Config.WSDL.Schema.NamespaceVariable + ":").Replace(s_soapXml, "</" + this.Seed.Config.WSDL.Schema.NamespaceVariable + ":");
        }*/

            /* prepare SOAP xml request */
            string s_xml = "<?xml version=\"1.0\" encoding=\"" + (this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).BodyName.ToUpper() + "\" ?>" + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK;

            s_xml += "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">" + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK;
            s_xml += "\t<soap:Body>" + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK;

            /* add SOAP xml encoded data */
            s_xml += s_soapXml + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK;

            s_xml += "\t</soap:Body>" + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK;
            s_xml += "</soap:Envelope>";

            /* convert SOAP xml string to byte array */
            byte[] a_soapRequest = (this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).GetBytes(s_xml);
            /* set content type and content length */
            this.Seed.RequestHeader.ContentType = ForestNET.Lib.Net.Https.Config.KNOWN_EXTENSION_LIST[".xsd"] + "; charset=" + (this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).WebName;
            this.Seed.RequestHeader.ContentLength = a_soapRequest.Length;

            if (this.UseLog) ForestNET.Lib.Global.ILogFine("add request header to SOAP request");

            /* create string temporary variable for SOAP request header */
            byte[] a_requestHeader = (this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).GetBytes(this.Seed.RequestHeader.ToString());

            /* create byte array for SOAP request, including header and SOAP body length */
            byte[] a_foo = new byte[a_requestHeader.Length + a_soapRequest.Length];
            int i;

            /* copy each byte from request header to byte array */
            for (i = 0; i < a_requestHeader.Length; i++)
            {
                a_foo[i] = a_requestHeader[i];
            }

            if (this.UseLog) ForestNET.Lib.Global.ILogFine("add body data to SOAP request");

            /* copy each byte from SOAP body to byte array */
            for (int j = 0; j < a_soapRequest.Length; j++)
            {
                a_foo[j + i] = a_soapRequest[j];
            }

            /* return prepared SOAP request as byte array for sending with TCP */
            return a_foo;
        }

        /// <summary>
        /// prepare REST request
        /// </summary>
        /// <exception cref="ArgumentException">invalid download filename location or invalid content type</exception>
        /// <exception cref="System.IO.IOException">issues to check download filename location, url encoding data or reading all bytes from an attachment</exception>
        private byte[] HandleREST()
        {
            /* using same method as for https request */
            return this.HandleNormal();
        }

        /// <summary>
        /// receive response
        /// </summary>
        private async System.Threading.Tasks.Task<int> ReceiveResponse()
        {
            /* check if Seed instance is availalbe */
            if (this.Seed == null)
            {
                throw new ArgumentException("Configuration for tiny https client is not specified");
            }

            byte[]? a_responseData;

            if (this.UseLog) ForestNET.Lib.Global.ILogFine("receive data from destination");

            /* receive https response */
            try
            {
                a_responseData = await this.ReceiveBytes(-1);
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.ILogSevere("500 Internal Server Error: Exception while receiving data; " + o_exc);
                return 500;
            }

            /* check if we have received anything at all */
            if ((a_responseData == null) || (a_responseData.Length < 1))
            {
                ForestNET.Lib.Global.ILogWarning("204 No Content: HTTP request is null or empty");
                return 204;
            }

            if (this.UseLog) ForestNET.Lib.Global.ILogFine("received data from destination");

            /* check if response does not exceed max. payload */
            if (a_responseData.Length >= this.Seed.Config.MaxPayload * 1024 * 1024)
            {
                ForestNET.Lib.Global.ILogWarning("413 Payload Too Large: Reached max. payload of '" + ForestNET.Lib.Helper.FormatBytes(this.Seed.Config.MaxPayload * 1024 * 1024) + "'");
                return 413;
            }

            /* get byte position in response data, where we have the border between http header and body data */
            int i_borderHeaderFromContent = ForestNET.Lib.Net.Https.Dynm.Dynamic.GetNextLineBreak(a_responseData, this.Seed.Config.InEncoding, 0, true, ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK);

            /* check if we have two line breaks */
            if (i_borderHeaderFromContent < 0)
            {
                ForestNET.Lib.Global.ILogWarning("400 Bad Request: HTTP response does not contain two line breaks");
                return 400;
            }
            else
            {
                i_borderHeaderFromContent += ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK.Length * 2;
            }

            /* check if we have a response body */
            if (i_borderHeaderFromContent < a_responseData.Length)
            {
                /* define response body byte array */
                this.Seed.ResponseBody = new byte[a_responseData.Length - i_borderHeaderFromContent];

                /* get response body data */
                for (int i = i_borderHeaderFromContent; i < a_responseData.Length; i++)
                {
                    this.Seed.ResponseBody[i - i_borderHeaderFromContent] = a_responseData[i];
                }
            }

            byte[] a_headerData = new byte[i_borderHeaderFromContent - 4];

            /* get response header data */
            for (int i = 0; i < i_borderHeaderFromContent - 4; i++)
            {
                a_headerData[i] = a_responseData[i];
            }

            /* retrieve response header lines from header byte data, use incoming encoding setting */
            string[] a_responseHeaderLines = (this.Seed.Config.InEncoding ?? System.Text.Encoding.UTF8).GetString(a_headerData).Split(ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK);

            /* read response header */
            int i_foo = this.Seed.ResponseHeader.ParseResponseHeader(a_responseHeaderLines);

            /* maybe we received header data, but our body data is still missing (content length > 0) */
            if ((i_foo == 200) && (this.Seed.ResponseBody == null) && (this.Seed.ResponseHeader.ContentLength > 0))
            {

                if (this.UseLog) ForestNET.Lib.Global.ILogFine("try to receive body data again, cause header told us we have content length: " + this.Seed.ResponseHeader.ContentLength);

                /* check if content length value from received header does not exceed max. payload value */
                if (this.Seed.ResponseHeader.ContentLength >= this.Seed.Config.MaxPayload * 1024 * 1024)
                {
                    ForestNET.Lib.Global.ILogWarning("413 Payload Too Large: Reached max. payload of '" + ForestNET.Lib.Helper.FormatBytes(this.Seed.Config.MaxPayload * 1024 * 1024) + "'");
                    return 413;
                }

                if (this.UseLog) ForestNET.Lib.Global.ILogFine("receive data from destination");

                /* do another receive, now with content length as parameter */
                try
                {
                    a_responseData = await this.ReceiveBytes(this.Seed.ResponseHeader.ContentLength);
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.ILogSevere("500 Internal Server Error: Exception while receiving data; " + o_exc);
                    return 500;
                }

                /* check if we have received body data */
                if ((a_responseData == null) || (a_responseData.Length < 1))
                {
                    ForestNET.Lib.Global.ILogWarning("204 No Content: HTTP request body is null or empty");
                    return 204;
                }

                if (this.UseLog) ForestNET.Lib.Global.ILogFine("received data from destination");

                /* define response body byte array */
                this.Seed.ResponseBody = new byte[a_responseData.Length];

                /* get response body data */
                for (int i = 0; i < a_responseData.Length; i++)
                {
                    this.Seed.ResponseBody[i] = a_responseData[i];
                }
            }

            /* store received cookies in current client instance for next request */
            if (this.Seed.Config.ClientUseCookiesFromPreviousRequest)
            {
                this.TempCookie = this.Seed.ResponseHeader.Cookie;
            }

            /* if we have not return code 200, exit here */
            if (i_foo != 200)
            {
                return i_foo;
            }

            /* return received http status code */
            return this.Seed.ResponseHeader.ReturnCode;
        }

        /// <summary>
        /// handle https response
        /// </summary>
        private async System.Threading.Tasks.Task<int> HandleNormalResponse()
        {
            /* check if Seed instance is availalbe */
            if (this.Seed == null)
            {
                throw new ArgumentException("Configuration for tiny https client is not specified");
            }

            /* receive https response */
            int i_foo = await this.ReceiveResponse();

            /* if we have not return code 200, exit here */
            if (i_foo != 200)
            {
                return i_foo;
            }

            if (this.RequestType == ForestNET.Lib.Net.Request.RequestType.DOWNLOAD)
            {
                if (this.UseLog) ForestNET.Lib.Global.ILogFine("writing received body data into file output stream");

                /* check if download filename is null */
                if (this.DownloadFilename == null)
                {
                    throw new NullReferenceException("Download filename is null");
                }

                /* write downloaded bytes to file */
                try
                {
                    /* open file */
                    ForestNET.Lib.IO.File o_fileDownload = new(this.DownloadFilename, true);

                    /* write received body data to file */
                    o_fileDownload.ReplaceContent(this.Seed.ResponseBody ?? []);

                    /* update return message */
                    this.Seed.ResponseHeader.ReturnMessage = "File[" + ((this.Seed.RequestHeader.RequestPath.LastIndexOf("/") > 0) ? this.Seed.RequestHeader.RequestPath.Substring(this.Seed.RequestHeader.RequestPath.LastIndexOf("/") + 1) : this.Seed.RequestHeader.RequestPath) + "] downloaded to '" + this.DownloadFilename + "'";
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.ILogWarning("exception occurred while writing response of web request to a file");

                    /* exception occurred while writing response of web request to a file */
                    this.Seed.ResponseHeader.ReturnCode = 500;
                    this.Seed.ResponseHeader.ReturnMessage = "Error in web request - " + o_exc;
                }
            }

            /* return received http status code */
            return this.Seed.ResponseHeader.ReturnCode;
        }

        /// <summary>
        /// handle SOAP response
        /// </summary>
        private async System.Threading.Tasks.Task<int> HandleSOAPResponse()
        {
            /* check if Seed instance is availalbe */
            if (this.Seed == null)
            {
                throw new ArgumentException("Configuration for tiny https client is not specified");
            }

            /* receive SOAP response */
            int i_foo = await this.ReceiveResponse();

            /* if we have not return code 200, exit here */
            if (i_foo != 200)
            {
                return i_foo;
            }

            if (this.Seed.ResponseBody != null)
            { /* continue if we have received SOAP xml data */
                System.Text.Encoding o_inEncoding;

                /* check for incoming charset value in response header */
                if (this.Seed.ResponseHeader.ContentType.Contains("charset="))
                {
                    if (this.UseLog) ForestNET.Lib.Global.ILogFine("read charset encoding out of response header");

                    /* read charset encoding out of response header */
                    string s_encoding = this.Seed.ResponseHeader.ContentType.Substring(this.Seed.RequestHeader.ContentType.IndexOf("charset=") + 8);
                    s_encoding = s_encoding.ToLower();

                    /* delete double quotes */
                    if ((s_encoding.StartsWith("\"")) && (s_encoding.EndsWith("\"")))
                    {
                        s_encoding = s_encoding.Substring(1, s_encoding.Length - 1);
                    }

                    if (this.UseLog) ForestNET.Lib.Global.ILogFine("try to create an encoding object with read charset encoding value: " + s_encoding);

                    /* try to create an encoding object with read charset encoding value */
                    try
                    {
                        o_inEncoding = System.Text.Encoding.GetEncoding(s_encoding);
                    }
                    catch (Exception o_exc)
                    {
                        ForestNET.Lib.Global.ILogWarning("400 Bad Request: Invalid encoding '" + s_encoding + "' within response in HTTP header content type '" + this.Seed.ResponseHeader.ContentType + "'; " + o_exc);
                        return 400;
                    }
                }
                else
                { /* use configured incoming encoding property */
                    o_inEncoding = this.Seed.Config.InEncoding ?? System.Text.Encoding.UTF8;
                }

                /* convert received SOAP response byte array to string with charset object */
                string s_soapResponse = o_inEncoding.GetString(this.Seed.ResponseBody);

                /* clean up xml file */
                s_soapResponse = new System.Text.RegularExpressions.Regex("[\\r\\n\\t]").Replace(s_soapResponse, "");
                s_soapResponse = new System.Text.RegularExpressions.Regex(">\\s*<").Replace(s_soapResponse, "><");
                s_soapResponse = new System.Text.RegularExpressions.Regex("<\\?(.*?)\\?>").Replace(s_soapResponse, "");
                s_soapResponse = new System.Text.RegularExpressions.Regex("<!--(.*?)-->").Replace(s_soapResponse, "");
                s_soapResponse = new System.Text.RegularExpressions.Regex("<soap:").Replace(s_soapResponse, "<");
                s_soapResponse = new System.Text.RegularExpressions.Regex("</soap:").Replace(s_soapResponse, "</");

                /* validate xml */
                System.Text.RegularExpressions.Regex o_regex = new("(<[^<>]*?<[^<>]*?>|<[^<>]*?>[^<>]*?>)");
                System.Text.RegularExpressions.MatchCollection o_matcher = o_regex.Matches(s_soapResponse);

                /* if regex-matcher has match, the xml-file is not valid */
                while (o_matcher.Count > 0)
                {
                    throw new ArgumentException("Invalid xml-file. Please check xml-file at \"" + o_matcher[0] + "\".");
                }

                List<string> a_xmlTags = [];

                /* add all xml-tags to a list for parsing */
                o_regex = new System.Text.RegularExpressions.Regex("(<[^<>/]*?/>)|(<[^<>/]*?>[^<>]*?</[^<>/]*?>)|(<[^<>]*?>)|(</[^<>/]*?>)");
                o_matcher = o_regex.Matches(s_soapResponse);

                if (o_matcher.Count > 0)
                {
                    for (int i = 0; i < o_matcher.Count; i++)
                    {
                        /* xml-tag must start with '<' and ends with '>' */
                        if ((!o_matcher[i].ToString().StartsWith("<")) && (!o_matcher[i].ToString().EndsWith(">")))
                        {
                            throw new ArgumentException("Invalid xml-tag. Please check xml-file at \"" + o_matcher[i].ToString() + "\".");
                        }

                        a_xmlTags.Add(o_matcher[i].ToString());
                    }
                }

                /* first element must be 'Envelope' */
                if (!a_xmlTags[0].StartsWith("<Envelope"))
                {
                    ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "400 Bad Request: SOAP request must start with 'Envelope'-tag");
                    return 400;
                }

                int i_bodyTag = -1;

                /* look for next 'Body'-tag within 'Envelope' */
                for (int i = 1; i < a_xmlTags.Count; i++)
                {
                    if (a_xmlTags[i].Equals("<Body>"))
                    {
                        i_bodyTag = i;
                        break;
                    }
                }

                /* if we found no 'Body'-tag, abort here */
                if (i_bodyTag < 0)
                {
                    ForestNET.Lib.Global.ILogWarning("400 Bad Request: SOAP response must have a 'Body'-tag");
                    return 400;
                }

                /* reset return variables */
                this.SOAPFault = null;
                this.SOAPResponse = null;

                /* found 'Fault'-tag within 'Body' so we do not receive our expected SOAP response, but a SOAP fault response */
                if (a_xmlTags[i_bodyTag + 1].StartsWith("<Fault"))
                {
                    int i_faultEnd = -1;

                    /* look for 'Fault'-ending-tag */
                    for (int i = i_bodyTag + 1; i < a_xmlTags.Count; i++)
                    {
                        if (a_xmlTags[i].Equals("</Fault>"))
                        {
                            i_faultEnd = i;
                            break;
                        }
                    }

                    /* if 'Fault'-tag has not been closed, abort here */
                    if (i_faultEnd < 0)
                    {
                        ForestNET.Lib.Global.ILogWarning("400 Bad Request: SOAP response does not closed the 'Fault'-tag");
                        return 400;
                    }

                    string? s_code = null;
                    string? s_message = null;
                    string? s_detail = null;
                    string? s_actor = null;

                    /* read 'Fault'-children-tags */
                    for (int i = i_bodyTag + 2; i < i_faultEnd; i++)
                    {
                        if (a_xmlTags[i].StartsWith("<faultcode>"))
                        { /* read fault code */
                            s_code = a_xmlTags[i].Substring(11, a_xmlTags[i].IndexOf("</faultcode>") - 11);
                        }
                        else if (a_xmlTags[i].StartsWith("<faultstring>"))
                        { /* read fault string */
                            s_message = a_xmlTags[i].Substring(13, a_xmlTags[i].IndexOf("</faultstring>") - 13);
                        }
                        else if (a_xmlTags[i].StartsWith("<detail>"))
                        { /* read fault detail */
                            s_detail = a_xmlTags[i].Substring(8, a_xmlTags[i].IndexOf("</detail>") - 8);
                        }
                        else if (a_xmlTags[i].StartsWith("<faultactor>"))
                        { /* read fault actor */
                            s_actor = a_xmlTags[i].Substring(12, a_xmlTags[i].IndexOf("</faultactor>") - 12);
                        }
                    }

                    if (this.UseLog) ForestNET.Lib.Global.ILogFine("create and store SOAP fault object");

                    /* create and store SOAP fault object for later use */
                    this.SOAPFault = new ForestNET.Lib.Net.Https.SOAP.SoapFault(s_code ?? throw new NullReferenceException("faultcode could not be read from SOAP response"), s_message ?? throw new NullReferenceException("faultstring could not be read from SOAP response"), s_detail, s_actor);
                }
                else
                { /* handle 'Body' data from SOAP response */
                    int i_bodyEnd = -1;

                    /* look for 'Body'-ending-tag */
                    for (int i = i_bodyTag + 1; i < a_xmlTags.Count; i++)
                    {
                        if (a_xmlTags[i].Equals("</Body>"))
                        {
                            i_bodyEnd = i;
                            break;
                        }
                    }

                    /* if 'Body'-tag has not been closed, abort here */
                    if (i_bodyEnd < 0)
                    {
                        ForestNET.Lib.Global.ILogWarning("400 Bad Request: SOAP response does not closed the 'Body'-tag");
                        return 400;
                    }

                    List<string> a_soapBodyXmlTags = [];

                    /* gather all SOAP xml tags in our list */
                    for (int i = i_bodyTag + 1; i < i_bodyEnd; i++)
                    {
                        a_soapBodyXmlTags.Add(a_xmlTags[i]);
                    }

                    /* clean up xml namespace stuff */
                    if (a_soapBodyXmlTags[0].Contains(" xmlns:"))
                    {
                        string s_namespaceVar = a_soapBodyXmlTags[0].Substring(1, a_soapBodyXmlTags[0].IndexOf(":") - 1);

                        /* remove xmlns attribute */
                        if (a_soapBodyXmlTags[0].Substring(a_soapBodyXmlTags[0].IndexOf(" xmlns:") + 7).Contains(' '))
                        {
                            string s_one = a_soapBodyXmlTags[0].Substring(0, a_soapBodyXmlTags[0].IndexOf(" xmlns:"));
                            string s_two = a_soapBodyXmlTags[0].Substring(a_soapBodyXmlTags[0].IndexOf(" xmlns:") + 7);

                            a_soapBodyXmlTags[0] = s_one + s_two.Substring(s_two.IndexOf(" "));
                        }
                        else
                        {
                            string s_one = a_soapBodyXmlTags[0].Substring(0, a_soapBodyXmlTags[0].IndexOf(" xmlns:"));
                            string s_two = a_soapBodyXmlTags[0].Substring(a_soapBodyXmlTags[0].IndexOf(" xmlns:"));

                            a_soapBodyXmlTags[0] = s_one + s_two.Substring(s_two.IndexOf(">"));
                        }

                        for (int i = 0; i < a_soapBodyXmlTags.Count; i++)
                        {
                            a_soapBodyXmlTags[i] = a_soapBodyXmlTags[i].Replace("<" + s_namespaceVar + ":", "<").Replace("</" + s_namespaceVar + ":", "</");
                        }
                    }
                    else if (a_soapBodyXmlTags[0].Contains(" xmlns="))
                    {
                        /* remove xmlns attribute */
                        if (a_soapBodyXmlTags[0].Substring(a_soapBodyXmlTags[0].IndexOf(" xmlns=") + 7).Contains(' '))
                        {
                            string s_one = a_soapBodyXmlTags[0].Substring(0, a_soapBodyXmlTags[0].IndexOf(" xmlns="));
                            string s_two = a_soapBodyXmlTags[0].Substring(a_soapBodyXmlTags[0].IndexOf(" xmlns=") + 7);

                            a_soapBodyXmlTags[0] = s_one + s_two.Substring(s_two.IndexOf(" "));
                        }
                        else
                        {
                            string s_one = a_soapBodyXmlTags[0].Substring(0, a_soapBodyXmlTags[0].IndexOf(" xmlns="));
                            string s_two = a_soapBodyXmlTags[0].Substring(a_soapBodyXmlTags[0].IndexOf(" xmlns="));

                            a_soapBodyXmlTags[0] = s_one + s_two.Substring(s_two.IndexOf(">"));
                        }
                    }

                    if (this.UseLog) ForestNET.Lib.Global.ILogFine("decode received and cleaned up SOAP xml data");

                    /* decode received and cleaned up SOAP xml data to an object and store it as SOAP response property */
                    try
                    {
                        this.SOAPResponse = this.Seed.Config.WSDL?.Schema.XmlDecode(a_soapBodyXmlTags);
                    }
                    catch (Exception o_exc)
                    {
                        ForestNET.Lib.Global.ILogSevere("500 Internal Server Error: Exception while decoding received SOAP xml data; " + o_exc);
                        return 500;
                    }
                }
            }
            else
            {
                ForestNET.Lib.Global.ILogSevere("500 Internal Server Error: no SOAP xml data received");
                return 500;
            }

            /* return received http status code */
            return this.Seed.ResponseHeader.ReturnCode;
        }

        /// <summary>
        /// handle REST response
        /// </summary>
        private async System.Threading.Tasks.Task<int> HandleRESTResponse()
        {
            /* using same method as for https response */
            return await this.HandleNormalResponse();
        }
    }
}