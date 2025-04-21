using System.Net;

namespace ForestNET.Lib.Net.Request
{
    /// <summary>
    /// Generating a http/https request to a webserver, supporting http(s) request types GET, POST and DOWNLOAD.
    /// </summary>
    public class Client
    {

        /* Delegates */

        /// <summary>
        /// delegate definition which can be instanced outside of Request class to post progress anywhere when a request download is running
        /// </summary>
        public delegate void PostProgress(double p_d_progress, string p_s_filename);

        /* Fields */

        public static readonly List<string> AllowedParameterTypes = ["sbyte", "short", "int", "long", "byte", "ushort", "uint", "ulong", "float", "double", "bool", "char", "string", "system.sbyte", "system.int16", "system.int32", "system.int64", "system.byte", "system.uint16", "system.uint32", "system.uint64", "system.single", "system.double", "system.boolean", "system.character", "system.string"];

        /* Properties */

        public RequestType Type { get; set; }
        public string Address { get; set; }
        public System.Text.Encoding Encoding { get; set; }
        public string? ProxyAddress { get; set; }
        public int ProxyPort { get; set; }
        public bool ProxyUseDefaultCredentials { get; set; }
        public string ProxyUser { get; set; }
        public string ProxyPassword { get; set; }
        public PostType? ContentType { get; set; }
        public string AuthenticationUser { get; set; }
        public string AuthenticationPassword { get; set; }
        public string? DownloadFilename { get; set; }
        public bool DownloadFileExtensionNeeded { get; set; }
        public string LineBreak { get; set; }
        public TimeSpan Timeout { get; set; }
        public bool OverwriteDownload { get; set; }
        public int ResponseCode { get; private set; }
        public string ResponseMessage { get; private set; }
        public bool UseLog { get; set; }
        public PostProgress? DelegatePostProgress { private get; set; }
        private Dictionary<string, Object> RequestParamters { get; set; }
        private Dictionary<string, string> Attachments { get; set; }

        /* Methods */

        /// <summary>
        /// Create http(s) request object with all necessary information before executing the request to a webserver, line break from ForestNET.Lib.IO.File.NEWLINE.
        /// standard encoding from ForestNET.Lib.IO.File.ENCODING.
        /// no post content type, but needed for POST request.
        /// no proxy
        /// </summary>
        /// <param name="p_e_type">type of web request: GET, POST or DOWNLOAD</param>
        /// <param name="p_s_address">address of https web server</param>
        public Client(RequestType p_e_type, string p_s_address) :
            this(p_e_type, p_s_address, System.Text.Encoding.GetEncoding(ForestNET.Lib.IO.File.ENCODING))
        {

        }

        /// <summary>
        /// Create http(s) request object with all necessary information before executing the request to a webserver, line break from ForestNET.Lib.IO.File.NEWLINE.
        /// no post content type, but needed for POST request.
        /// no proxy.
        /// </summary>
        /// <param name="p_e_type">type of web request: GET, POST or DOWNLOAD</param>
        /// <param name="p_s_address">address of https web server</param>
        /// <param name="p_o_encoding">encoding which will be used sending and receiving request data</param>
        public Client(RequestType p_e_type, string p_s_address, System.Text.Encoding p_o_encoding) :
            this(p_e_type, p_s_address, p_o_encoding, null)
        {

        }

        /// <summary>
        /// Create http(s) request object with all necessary information before executing the request to a webserver, line break from ForestNET.Lib.IO.File.NEWLINE.
        /// no proxy.
        /// </summary>
        /// <param name="p_e_type">type of web request: GET, POST or DOWNLOAD</param>
        /// <param name="p_s_address">address of https web server</param>
        /// <param name="p_o_encoding">encoding which will be used sending and receiving request data</param>
        /// <param name="p_e_contentType">specify post content type: HTMLATTACHMENTS, HTML or JSON</param>
        public Client(RequestType p_e_type, string p_s_address, System.Text.Encoding p_o_encoding, PostType? p_e_contentType) :
            this(p_e_type, p_s_address, p_o_encoding, p_e_contentType, "")
        {

        }

        /// <summary>
        /// Create http(s) request object with all necessary information before executing the request to a webserver, line break from ForestNET.Lib.IO.File.NEWLINE.
        /// standard encoding from ForestNET.Lib.IO.File.ENCODING.
        /// no proxy.
        /// </summary>
        /// <param name="p_e_type">type of web request: GET, POST or DOWNLOAD</param>
        /// <param name="p_s_address">address of https web server</param>
        /// <param name="p_e_contentType">specify post content type: HTMLATTACHMENTS, HTML or JSON</param>
        public Client(RequestType p_e_type, string p_s_address, PostType? p_e_contentType) :
            this(p_e_type, p_s_address, System.Text.Encoding.GetEncoding(ForestNET.Lib.IO.File.ENCODING), p_e_contentType, "")
        {

        }

        /// <summary>
        /// Create http(s) request object with all necessary information before executing the request to a webserver, line break from ForestNET.Lib.IO.File.NEWLINE.
        /// standard encoding from ForestNET.Lib.IO.File.ENCODING.
        /// no post content type, but needed for POST request.
        /// proxy port: 80
        /// </summary>
        /// <param name="p_e_type">type of web request: GET, POST or DOWNLOAD</param>
        /// <param name="p_s_address">address of https web server</param>
        /// <param name="p_s_proxyAddress">specify a proxy address which will be used for the web request</param>
        public Client(RequestType p_e_type, string p_s_address, string p_s_proxyAddress) :
            this(p_e_type, p_s_address, System.Text.Encoding.GetEncoding(ForestNET.Lib.IO.File.ENCODING), null, p_s_proxyAddress)
        {

        }

        /// <summary>
        /// Create http(s) request object with all necessary information before executing the request to a webserver, line break from ForestNET.Lib.IO.File.NEWLINE.
        /// standard encoding from ForestNET.Lib.IO.File.ENCODING.
        /// proxy port: 80.
        /// </summary>
        /// <param name="p_e_type">type of web request: GET, POST or DOWNLOAD</param>
        /// <param name="p_s_address">address of https web server</param>
        /// <param name="p_e_contentType">specify post content type: HTMLATTACHMENTS, HTML or JSON</param>
        /// <param name="p_s_proxyAddress">specify a proxy address which will be used for the web request</param>
        public Client(RequestType p_e_type, string p_s_address, PostType? p_e_contentType, string p_s_proxyAddress) :
            this(p_e_type, p_s_address, System.Text.Encoding.GetEncoding(ForestNET.Lib.IO.File.ENCODING), p_e_contentType, p_s_proxyAddress, 80)
        {

        }

        /// <summary>
        /// Create http(s) request object with all necessary information before executing the request to a webserver, line break from ForestNET.Lib.IO.File.NEWLINE.
        /// standard encoding from ForestNET.Lib.IO.File.ENCODING.
        /// no post content type, but needed for POST request.
        /// </summary>
        /// <param name="p_e_type">type of web request: GET, POST or DOWNLOAD</param>
        /// <param name="p_s_address">address of https web server</param>
        /// <param name="p_s_proxyAddress">specify a proxy address which will be used for the web request</param>
        /// <param name="p_i_proxyPort">specify a proxy port which will be used for the web request</param>
        public Client(RequestType p_e_type, string p_s_address, string p_s_proxyAddress, int p_i_proxyPort) :
            this(p_e_type, p_s_address, System.Text.Encoding.GetEncoding(ForestNET.Lib.IO.File.ENCODING), null, p_s_proxyAddress, p_i_proxyPort)
        {

        }

        /// <summary>
        /// Create http(s) request object with all necessary information before executing the request to a webserver, line break from ForestNET.Lib.IO.File.NEWLINE.
        /// standard encoding from ForestNET.Lib.IO.File.ENCODING.
        /// </summary>
        /// <param name="p_e_type">type of web request: GET, POST or DOWNLOAD</param>
        /// <param name="p_s_address">address of https web server</param>
        /// <param name="p_e_contentType">specify post content type: HTMLATTACHMENTS, HTML or JSON</param>
        /// <param name="p_s_proxyAddress">specify a proxy address which will be used for the web request</param>
        /// <param name="p_i_proxyPort">specify a proxy port which will be used for the web request</param>
        public Client(RequestType p_e_type, string p_s_address, PostType? p_e_contentType, string p_s_proxyAddress, int p_i_proxyPort) :
            this(p_e_type, p_s_address, System.Text.Encoding.GetEncoding(ForestNET.Lib.IO.File.ENCODING), p_e_contentType, p_s_proxyAddress, p_i_proxyPort)
        {

        }

        /// <summary>
        /// Create http(s) request object with all necessary information before executing the request to a webserver, line break from ForestNET.Lib.IO.File.NEWLINE.
        /// proxy port: 80.
        /// </summary>
        /// <param name="p_e_type">type of web request: GET, POST or DOWNLOAD</param>
        /// <param name="p_s_address">address of https web server</param>
        /// <param name="p_o_encoding">encoding which will be used sending and receiving request data</param>
        /// <param name="p_e_contentType">specify post content type: HTMLATTACHMENTS, HTML or JSON</param>
        /// <param name="p_s_proxyAddress">specify a proxy address which will be used for the web request</param>
        public Client(RequestType p_e_type, string p_s_address, System.Text.Encoding p_o_encoding, PostType? p_e_contentType, string p_s_proxyAddress) :
            this(p_e_type, p_s_address, p_o_encoding, p_e_contentType, p_s_proxyAddress, 80)
        {

        }

        /// <summary>
        /// Create http(s) request object with all necessary information before executing the request to a webserver, line break from ForestNET.Lib.IO.File.NEWLINE
        /// </summary>
        /// <param name="p_e_type">type of web request: GET, POST or DOWNLOAD</param>
        /// <param name="p_s_address">address of https web server</param>
        /// <param name="p_o_encoding">encoding which will be used sending and receiving request data</param>
        /// <param name="p_e_contentType">specify post content type: HTMLATTACHMENTS, HTML or JSON</param>
        /// <param name="p_s_proxyAddress">specify a proxy address which will be used for the web request</param>
        /// <param name="p_i_proxyPort">specify a proxy port which will be used for the web request</param>
        public Client(RequestType p_e_type, string p_s_address, System.Text.Encoding p_o_encoding, PostType? p_e_contentType, string p_s_proxyAddress, int p_i_proxyPort)
        {
            this.Type = p_e_type;
            this.Address = p_s_address;
            this.Encoding = p_o_encoding;
            this.ProxyAddress = p_s_proxyAddress;
            this.ProxyPort = p_i_proxyPort;
            this.ProxyUseDefaultCredentials = true;
            this.ProxyUser = "";
            this.ProxyPassword = "";
            this.ContentType = p_e_contentType;
            this.AuthenticationUser = "";
            this.AuthenticationPassword = "";
            this.DownloadFilename = null;
            this.DownloadFileExtensionNeeded = false;
            this.LineBreak = ForestNET.Lib.IO.File.NEWLINE;
            this.Timeout = new TimeSpan(0, 0, 30);
            this.OverwriteDownload = true;
            this.ResponseCode = -1;
            this.ResponseMessage = "REQUEST NOT SENT";
            this.UseLog = false;
            this.DelegatePostProgress = null;
            this.RequestParamters = [];
            this.Attachments = [];
        }

        /// <summary>
        /// adds request parameter to web request, if parameter type is an allowed type
        /// </summary>
        /// <param name="p_s_parameterName">type of request parameter, e.g. string, bool, or double</param>
        /// <param name="p_o_parameterValue">value of request parameter</param>
        /// <exception cref="ArgumentException">parameter type is not an allowed type</exception>
        public void AddRequestParameter(string p_s_parameterName, Object p_o_parameterValue)
        {
            if (!AllowedParameterTypes.Contains(p_o_parameterValue.GetType().Name.ToLower()))
            {
                throw new ArgumentException("Parameter[" + p_s_parameterName + "] with type[" + p_o_parameterValue.GetType().Name + "] is not an allowed parameter type: " + AllowedParameterTypes.ToString());
            }

            this.RequestParamters.Add(p_s_parameterName, p_o_parameterValue);

            if (this.UseLog) ForestNET.Lib.Global.ILogConfig("added request parameter: '" + p_s_parameterName + "' = '" + p_o_parameterValue + "'");
        }

        /// <summary>
        /// adds filepath to web request which will be attached for web request execution
        /// </summary>
        /// <param name="p_s_parameterName"></param>
        /// <param name="p_s_filePath"></param>
        public void AddAttachement(string p_s_parameterName, string p_s_filePath)
        {
            this.Attachments.Add(p_s_parameterName, p_s_filePath);

            if (this.UseLog) ForestNET.Lib.Global.ILogConfig("added attachment to request: '" + p_s_parameterName + "' = '" + p_s_filePath + "'");
        }

        /// <summary>
        /// execute web request and get response from web server as string message return value.
        /// get additional response code and message which will be stored in class properties.
        /// downloading a file will be combined with DownloadFilename property.
        /// </summary>
        /// <returns>string</returns>
        /// <exception cref="UriFormatException">invalid server address format or could not encode request parameter</exception>
        /// <exception cref="System.IO.IOException">could not establish connection or folder of download filename location does not exist</exception>
        /// <exception cref="ArgumentException">no download filename location specified, no file extension found, or no post content type specified for post web request</exception>
        public async Task<string> ExecuteWebRequest()
        {
            /* response variable */
            string s_response = "";

            /* check if we have any request parameters and web request type is 'GET' or 'DOWNLOAD' */
            if ((this.RequestParamters.Count > 0) && ((this.Type == RequestType.GET) || (this.Type == RequestType.DOWNLOAD)))
            {
                if (this.UseLog) ForestNET.Lib.Global.ILogConfig("gather request parameters with request type is 'GET' or 'DOWNLOAD'");

                /* string variable for request parameters */
                string s_requestParameters = "";

                /* iterate all request parameters with key->value pairs */
                foreach (KeyValuePair<string, Object> o_requestParameter in this.RequestParamters)
                {
                    /* add key->value pair request parameter url encoded to variable */
                    s_requestParameters += Uri.EscapeDataString(o_requestParameter.Key) + "=" + Uri.EscapeDataString(o_requestParameter.Value.ToString() ?? "") + "&";
                }

                /* if request parameters ends with '&' */
                if (s_requestParameters.EndsWith("&"))
                {
                    /* remove last '&' character from request parameters */
                    s_requestParameters = s_requestParameters.Substring(0, s_requestParameters.Length - 1);
                }

                if (this.UseLog) ForestNET.Lib.Global.ILogConfig("request parameters: '" + s_requestParameters + "'");

                /* add request parameters for type 'GET' or 'DOWNLOAD' to web request address */
                this.Address += "?" + s_requestParameters;
            }

            /* variable for proxy settings */
            WebProxy? o_proxy = null;

            /* if proxy address was set */
            if ((!ForestNET.Lib.Helper.IsStringEmpty(this.ProxyAddress)) && (this.ProxyAddress != null))
            {
                if (this.UseLog) ForestNET.Lib.Global.ILogConfig("create proxy object with address and port: '" + this.ProxyAddress + ":" + this.ProxyPort + "'");

                /* remove https:// from proxy address */
                if (this.ProxyAddress.StartsWith("https://"))
                {
                    this.ProxyAddress = this.ProxyAddress.Substring(8);
                }

                /* add http:// to proxy address if missing */
                if (!this.ProxyAddress.StartsWith("http://"))
                {
                    this.ProxyAddress = "http://" + this.ProxyAddress;
                }

                /* create proxy object with address and port */
                o_proxy = new WebProxy
                {
                    Address = new Uri($"{this.ProxyAddress}:{this.ProxyPort}"),
                    BypassProxyOnLocal = false,
                    UseDefaultCredentials = this.ProxyUseDefaultCredentials
                };
            }

            /* if we have proxy user and password settings */
            if ((o_proxy != null) && (!ForestNET.Lib.Helper.IsStringEmpty(this.ProxyUser)) && (!ForestNET.Lib.Helper.IsStringEmpty(this.ProxyPassword)))
            {
                if (this.UseLog) ForestNET.Lib.Global.ILogConfig("add user and password to proxy object '" + this.ProxyAddress + ":" + this.ProxyPort + "'");

                /* important to unset UseDefaultCredentials here, because if we do it after assign Credentials -> Credentials will be nulled */
                o_proxy.UseDefaultCredentials = false;

                /* credentials are given to the proxy server, not the web server */
                o_proxy.Credentials = new NetworkCredential(this.ProxyUser, this.ProxyPassword);
            }

            if (this.UseLog) ForestNET.Lib.Global.ILogConfig("create new url object with web request address: '" + this.Address + "'");

            /* create new url object with web request address */
            Uri o_url = new(this.Address);

            /* http client instance */
            HttpClient o_httpClient;

            /* proxy available */
            if (o_proxy != null)
            {
                if (this.UseLog) ForestNET.Lib.Global.ILogConfig("create http client instance with http client handler + proxy");

                /* http client handler for proxy */
                HttpClientHandler o_httpClientHandler = new()
                {
                    Proxy = o_proxy,
                    UseProxy = true
                };

                /* create http client instance with http client handler + proxy */
                o_httpClient = new(o_httpClientHandler, true)
                {
                    Timeout = this.Timeout
                };
            }
            else
            {
                if (this.UseLog) ForestNET.Lib.Global.ILogConfig("create http client instance; no proxy");

                /* create http client instance; no proxy */
                o_httpClient = new()
                {
                    Timeout = this.Timeout
                };
            }

            /* if authentication user and password were set */
            if ((!ForestNET.Lib.Helper.IsStringEmpty(this.AuthenticationUser)) && (!ForestNET.Lib.Helper.IsStringEmpty(this.AuthenticationPassword)))
            {
                /* create base64 authentication string and add it to web request properties */
                o_httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue(
                        "Basic", Convert.ToBase64String(
                            System.Text.ASCIIEncoding.ASCII.GetBytes(
                               $"{this.AuthenticationUser}:{this.AuthenticationPassword}"
                            )
                        )
                    );

                if (this.UseLog) ForestNET.Lib.Global.ILogConfig("created base64 authentication string and added it to web request properties");
            }

            /* variables for sending request and building post body */
            HttpRequestMessage? o_httpRequestMessage;

            if (this.Type == RequestType.GET)
            { /* web request type 'GET' */
                if (this.UseLog) ForestNET.Lib.Global.ILogConfig("use 'GET'");

                /* set request type to 'GET' */
                o_httpRequestMessage = new(HttpMethod.Get, o_url);
            }
            else if (this.Type == RequestType.DOWNLOAD)
            { /* web request type 'DOWNLOAD' */
                if (this.UseLog) ForestNET.Lib.Global.ILogConfig("use 'GET' for download execution");

                /* check if download filename location is set */
                if (ForestNET.Lib.Helper.IsStringEmpty(this.DownloadFilename))
                {
                    throw new ArgumentException("Please specify a download filename location");
                }

                /* check if target folder of download filename really exists */
                if (!ForestNET.Lib.IO.File.FolderExists((this.DownloadFilename ?? "").Substring(0, (this.DownloadFilename ?? "").LastIndexOf(ForestNET.Lib.IO.File.DIR))))
                {
                    throw new System.IO.IOException("Folder of download filename location[" + this.DownloadFilename + "] does not exist. Please create the download folder location before execution");
                }

                /* check if download filename has a file extension */
                if (this.DownloadFileExtensionNeeded)
                {
                    if (!ForestNET.Lib.IO.File.HasFileExtension(this.DownloadFilename ?? ""))
                    {
                        throw new ArgumentException("Download filename location[" + this.DownloadFilename + "] must have a valid file extension");
                    }
                }

                /* check if target download filename already exists, and if it can be overwritten */
                if (ForestNET.Lib.IO.File.Exists(this.DownloadFilename ?? ""))
                {
                    if (!this.OverwriteDownload)
                    {
                        throw new ArgumentException("File[" + this.DownloadFilename + "] already exists and will not be overwritten");
                    }
                    else
                    {
                        if (this.UseLog) ForestNET.Lib.Global.ILogConfig("delete file '" + this.DownloadFilename + "'");

                        ForestNET.Lib.IO.File.DeleteFile(this.DownloadFilename ?? "");
                    }
                }

                /* set request type to 'GET' for download execution */
                o_httpRequestMessage = new(HttpMethod.Get, o_url);
            }
            else if (this.Type == RequestType.POST)
            { /* web request type 'POST' */
                /* random value for https file upload protocol */
                string s_boundary = "------------" + Environment.TickCount.ToString("X4");

                if (this.UseLog) ForestNET.Lib.Global.ILogConfig("created random value(boundary) for http file upload protocol: '" + s_boundary + "'");

                if (this.UseLog) ForestNET.Lib.Global.ILogConfig("use 'POST'");

                /* set request type to 'POST' and other request properties */
                o_httpRequestMessage = new(HttpMethod.Post, o_url);
                o_httpRequestMessage.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue
                {
                    NoCache = true
                };

                /* check if content type is not null, necessary for type 'POST' */
                if (this.ContentType == null)
                {
                    throw new ArgumentException("No content type for POST web request specified");
                }

                /* ***** */
                /* start - sending post data for web request 'POST' */
                /* ***** */

                /* create multi part content post body with boundary or FormUrlEncodedContent with list of key value pairs */
                Dictionary<string, string> a_stringContent = [];
                MultipartFormDataContent o_multiPartContent = new(s_boundary);

                if (this.UseLog) ForestNET.Lib.Global.ILogConfig("start building post data");

                /* variable for web request 'POST' output stream for easy handling of adding post data, auto close of output stream when try block is closed */
                if (this.Attachments.Count > 0)
                {
                    if (this.UseLog) ForestNET.Lib.Global.ILogConfig("iterate all request parameters with key->value pairs");

                    /* iterate all request parameters with key->value pairs */
                    foreach (KeyValuePair<string, Object> o_requestParameter in this.RequestParamters)
                    {
                        /* add parameter name and value to post body */
                        o_multiPartContent.Add(new StringContent(o_requestParameter.Value.ToString() ?? ""), o_requestParameter.Key);

                        if (this.UseLog) ForestNET.Lib.Global.ILogFine("request parameter '" + o_requestParameter.Key + "' added");
                    }

                    if (this.UseLog) ForestNET.Lib.Global.ILogConfig("iterate all attachments with key->value pairs");

                    foreach (KeyValuePair<string, string> m_attachment in this.Attachments)
                    {
                        if ((ForestNET.Lib.IO.File.Exists(m_attachment.Value)) && (ForestNET.Lib.IO.File.IsFile(m_attachment.Value)))
                        {
                            /* copy file stream into post body, with field name and file name */
                            o_multiPartContent.Add(new StreamContent(System.IO.File.OpenRead(m_attachment.Value)), m_attachment.Key, new System.IO.FileInfo(m_attachment.Value.ToString()).Name);

                            if (this.UseLog) ForestNET.Lib.Global.ILogFine("attachment '" + m_attachment.Key + "' added");
                        }
                        else
                        {
                            ForestNET.Lib.Global.ILogWarning("attachment file '" + m_attachment.Key + "' does not exist or is not a file: '" + m_attachment.Value + "'");
                        }
                    }

                    /* set post body */
                    o_httpRequestMessage.Content = o_multiPartContent;
                }
                else
                { /* no attachments available */
                    /* check if we have any request parameters */
                    if (this.RequestParamters.Count > 0)
                    {
                        if (this.UseLog) ForestNET.Lib.Global.ILogConfig("iterate all request parameters with key->value pairs");

                        /* iterate all request parameters with key->value pairs */
                        foreach (KeyValuePair<string, Object> o_requestParameter in this.RequestParamters)
                        {
                            /* add parameter name and value to post body */
                            a_stringContent.Add(o_requestParameter.Key, o_requestParameter.Value.ToString() ?? "");


                            if (this.UseLog) ForestNET.Lib.Global.ILogFine("request parameter '" + o_requestParameter.Key + "' added");
                        }

                        if (this.UseLog) ForestNET.Lib.Global.ILogFine("added all request parameters to post body data");
                    }

                    /* set post body */
                    o_httpRequestMessage.Content = new FormUrlEncodedContent(a_stringContent);
                }

                if (this.UseLog) ForestNET.Lib.Global.ILogConfig("finished building post data");

                /* *** */
                /* end - sending post data of web request 'POST' */
                /* *** */
            }
            else
            {
                throw new ArgumentException("web request type '" + this.Type + "' is not supported");
            }

            /* ***** */
            /* start - reading response for web request */
            /* ***** */

            if (this.UseLog) ForestNET.Lib.Global.ILogConfig("starting receiving response data");

            HttpResponseMessage? o_httpResponseMessage = null;

            /* reading input stream into file output stream if web request is a 'DOWNLOAD' */
            if (this.Type == RequestType.DOWNLOAD)
            {
                if (this.UseLog) ForestNET.Lib.Global.ILogConfig("reading input stream into file output stream");

                try
                {
                    /* use input stream for web request response */
                    o_httpResponseMessage = await o_httpClient.GetAsync(o_url, HttpCompletionOption.ResponseHeadersRead);

                    if (this.UseLog) ForestNET.Lib.Global.ILogConfig("get content length");

                    /* get content length */
                    long l_contentLength = o_httpResponseMessage.Content.Headers.ContentLength ?? 0;

                    if (this.UseLog) ForestNET.Lib.Global.ILogConfig("content length: '" + l_contentLength + "'");

                    /* help variables to read and write stream instances */
                    byte[] a_buffer = new byte[1024];
                    int i_length = 0;
                    long l_sum = 0;

                    if (this.UseLog) ForestNET.Lib.Global.ILogConfig("get input stream");

                    /* get input stream */
                    Stream o_inputStream = await o_httpResponseMessage.Content.ReadAsStreamAsync();

                    /* create target file for download */
                    using (FileStream o_fileStream = File.Create(this.DownloadFilename ?? ""))
                    {
                        if (this.UseLog) ForestNET.Lib.Global.ILogConfig("read input stream until the end");

                        /* read input stream until the end */
                        while ((i_length = o_inputStream.Read(a_buffer)) > 0)
                        {
                            /* write in output file stream */
                            o_fileStream.Write(a_buffer, 0, i_length);

                            /* post progress */
                            if (this.DelegatePostProgress != null)
                            {
                                l_sum += i_length;
                                this.DelegatePostProgress((double)l_sum / l_contentLength, ((o_url.ToString().LastIndexOf("/") > 0) ? o_url.ToString().Substring(o_url.ToString().LastIndexOf("/") + 1) : o_url.ToString()));
                            }
                        }
                    }

                    if (this.UseLog) ForestNET.Lib.Global.ILogConfig("store information in string return value that download request has been saved to local file");

                    s_response += "File[" + ((o_url.ToString().LastIndexOf("/") > 0) ? o_url.ToString().Substring(o_url.ToString().LastIndexOf("/") + 1) : o_url.ToString()) + "] downloaded to '" + this.DownloadFilename + "'";
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.ILogWarning("exception occurred while waiting/reading response for/of web request");

                    /* exception occurred while waiting/reading response for/of web request */
                    s_response += "Error in web request - " + o_exc.Message;
                }
                finally
                {
                    if (o_httpResponseMessage != null)
                    {
                        if (this.UseLog) ForestNET.Lib.Global.ILogConfig("get response code and message");

                        /* get response code and message */
                        this.ResponseCode = (int)o_httpResponseMessage.StatusCode;
                        this.ResponseMessage = o_httpResponseMessage.ReasonPhrase ?? "";

                        if (this.UseLog) ForestNET.Lib.Global.ILogConfig("response code: '" + this.ResponseCode + "'");

                        if (this.UseLog) ForestNET.Lib.Global.ILogConfig("response message: '" + this.ResponseMessage + "'");
                    }
                }
            }
            else
            { /* reading input stream into string if web request is a 'GET' or 'POST' */
                if (this.UseLog) ForestNET.Lib.Global.ILogConfig("reading input stream into string return value");

                try
                {
                    /* execute web request and receive response */
                    if ((o_httpRequestMessage ?? throw new NullReferenceException("http request message is null")).Method == HttpMethod.Get)
                    {
                        o_httpResponseMessage = await o_httpClient.GetAsync(o_url, HttpCompletionOption.ResponseHeadersRead);
                    }
                    else
                    {
                        o_httpResponseMessage = await o_httpClient.PostAsync(o_url, o_httpRequestMessage.Content);
                    }

                    if (this.UseLog) ForestNET.Lib.Global.ILogConfig("get content length");

                    /* get content length */
                    long l_contentLength = o_httpResponseMessage.Content.Headers.ContentLength ?? 0;

                    if (this.UseLog) ForestNET.Lib.Global.ILogConfig("content length: '" + l_contentLength + "'");

                    /* help variable to count streamed/downloaded content length */
                    byte[] a_buffer = new byte[1024];
                    int i_length = 0;
                    long l_sum = 0;

                    if (this.UseLog) ForestNET.Lib.Global.ILogConfig("get input stream");

                    /* get input stream */
                    Stream o_inputStream = await o_httpResponseMessage.Content.ReadAsStreamAsync();

                    if (this.UseLog) ForestNET.Lib.Global.ILogConfig("read input stream until the end");

                    /* read input stream until the end */
                    while ((i_length = o_inputStream.Read(a_buffer)) > 0)
                    {
                        /* add buffer to string */
                        s_response += Encoding.GetString(a_buffer, 0, i_length);

                        /* post progress */
                        if (this.DelegatePostProgress != null)
                        {
                            l_sum += i_length;
                            this.DelegatePostProgress((double)l_sum / l_contentLength, ((o_url.ToString().LastIndexOf("/") > 0) ? o_url.ToString().Substring(o_url.ToString().LastIndexOf("/") + 1) : o_url.ToString()));
                        }
                    }
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.ILogWarning("exception occurred while waiting/reading response for/of web request");

                    /* exception occurred while waiting/reading response for/of web request */
                    s_response += "Error in web request - " + o_exc.Message;
                }
                finally
                {
                    if (o_httpResponseMessage != null)
                    {
                        if (this.UseLog) ForestNET.Lib.Global.ILogConfig("get response code and message");

                        /* get response code and message */
                        this.ResponseCode = (int)o_httpResponseMessage.StatusCode;
                        this.ResponseMessage = o_httpResponseMessage.ReasonPhrase ?? "";

                        if (this.UseLog) ForestNET.Lib.Global.ILogConfig("response code: '" + this.ResponseCode + "'");

                        if (this.UseLog) ForestNET.Lib.Global.ILogConfig("response message: '" + this.ResponseMessage + "'");
                    }
                }
            }

            if (this.UseLog) ForestNET.Lib.Global.ILogConfig("finished receiving response data");

            /* *** */
            /* end - reading response for web request */
            /* *** */

            if (this.UseLog) ForestNET.Lib.Global.ILogConfig("dispose http request message");
            o_httpRequestMessage?.Dispose();

            if (this.UseLog) ForestNET.Lib.Global.ILogConfig("dispose http client instance");
            o_httpClient?.Dispose();

            if (this.UseLog) ForestNET.Lib.Global.ILogConfig("return web request response");

            /* return web request response */
            return s_response;
        }
    }
}