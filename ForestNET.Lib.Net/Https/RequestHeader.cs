namespace ForestNET.Lib.Net.Https
{
    /// <summary>
    /// HTTP Request Header class which can hold all necessary minimal header information for a request header and a parsing method to parse received request to this object class.
    /// </summary>
    public class RequestHeader
    {

        /* Constants */

        public const string CLIENT = "forestNET Tiny Https Client 1.0";

        /* Fields */

        private readonly string s_domain;
        private string s_method;
        private string s_requestPath;
        private string s_convertedPath;
        private string s_path;
        private string s_file;
        private Dictionary<string, string?> a_parameters;
        private string s_protocol;
        private bool b_connectionClosed;
        private int i_contentLength;
        private string s_contentType;
        private string s_boundary;
        private string s_cookie;
        private string s_host;
        private int i_port;
        private string s_referrer;
        private string s_userAgent;
        private string s_accept;
        private string s_acceptCharset;
        private bool b_noCacheControl;
        private string s_authorization;

        /* Properties */

        /// <summary>
        /// Set method by string parameter
        /// </summary>
        /// <exception cref="ArgumentException">invalid http method or invalid parameter value</exception>
        public string Method
        {
            get
            {
                return this.s_method;
            }
            set
            {
                if (!new string[] { "GET", "POST", "PUT", "DELETE", "DOWNLOAD" }.Contains(value))
                {
                    throw new ArgumentException("HTTP-Method must be [GET, POST, PUT, DELETE or DOWNLOAD], but is set to '" + value + "'");
                }

                this.s_method = value;
            }
        }
        /// <summary>
        /// Set method by request type parameter
        /// </summary>
        /// <exception cref="ArgumentException">not implemented request type parameter</exception>
        public ForestNET.Lib.Net.Request.RequestType MethodByRequestType
        {
            set
            {
                bool b_exc = false;

                switch (value)
                {
                    case ForestNET.Lib.Net.Request.RequestType.DELETE:
                        this.s_method = "DELETE";
                        break;
                    case ForestNET.Lib.Net.Request.RequestType.DOWNLOAD:
                        this.s_method = "DOWNLOAD";
                        break;
                    case ForestNET.Lib.Net.Request.RequestType.GET:
                        this.s_method = "GET";
                        break;
                    case ForestNET.Lib.Net.Request.RequestType.POST:
                        this.s_method = "POST";
                        break;
                    case ForestNET.Lib.Net.Request.RequestType.PUT:
                        this.s_method = "PUT";
                        break;
                    default:
                        b_exc = true;
                        break;
                }

                if (b_exc)
                {
                    throw new ArgumentException("HTTP-Method must be [GET, POST, PUT, DELETE or DOWNLOAD], but used unimplemented method '" + value + "'");
                }
            }
        }
        public string RequestPath
        {
            get
            {
                return this.s_requestPath;
            }
            set
            {
                this.s_requestPath = value;
            }
        }
        public string ConvertedPath
        {
            get
            {
                return this.s_convertedPath;
            }
            private set
            {
                this.s_convertedPath = value;
            }
        }
        public string Path
        {
            get
            {
                return this.s_path;
            }
            set
            {
                this.s_path = value;
            }
        }
        public string File
        {
            get
            {
                return this.s_file;
            }
            set
            {
                this.s_file = value;
            }
        }
        public Dictionary<string, string?> Parameters
        {
            get
            {
                return this.a_parameters;
            }
            private set
            {
                this.a_parameters = value;
            }
        }
        public string Protocol
        {
            get
            {
                return this.s_protocol;
            }
            set
            {
                this.s_protocol = value;
            }
        }
        public bool ConnectionClose
        {
            get
            {
                return this.b_connectionClosed;
            }
            set
            {
                this.b_connectionClosed = value;
            }
        }
        /// <summary>
        /// Content-Length of request
        /// </summary>
        /// <exception cref="ArgumentException">content-length must be a positive integer value</exception>
        public int ContentLength
        {
            get
            {
                return this.i_contentLength;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Content-Length must be a positive integer value");
                }
                else
                {
                    this.i_contentLength = value;
                }
            }
        }
        public string ContentType
        {
            get
            {
                return this.s_contentType;
            }
            set
            {
                this.s_contentType = value;
            }
        }
        public string Boundary
        {
            get
            {
                return this.s_boundary;
            }
            private set
            {
                this.s_boundary = value;
            }
        }
        public string Cookie
        {
            get
            {
                return this.s_cookie;
            }
            set
            {
                this.s_cookie = value;
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
                this.s_host = value;
            }
        }
        public int Port
        {
            get
            {
                return this.i_port;
            }
            set
            {
                this.i_port = value;
            }
        }
        public string Referrer
        {
            get
            {
                return this.s_referrer;
            }
            private set
            {
                this.s_referrer = value;
            }
        }
        public string UserAgent
        {
            get
            {
                return this.s_userAgent;
            }
            set
            {
                this.s_userAgent = value;
            }
        }
        public string Accept
        {
            get
            {
                return this.s_accept;
            }
            set
            {
                this.s_accept = value;
            }
        }
        public string AcceptCharset
        {
            get
            {
                return this.s_acceptCharset;
            }
            set
            {
                this.s_acceptCharset = value;
            }
        }
        public bool NoCacheControl
        {
            get
            {
                return this.b_noCacheControl;
            }
            set
            {
                this.b_noCacheControl = value;
            }
        }
        public string Authorization
        {
            get
            {
                return this.s_authorization;
            }
            set
            {
                this.s_authorization = value;
            }
        }

        /* Methods */

        /// <summary>
        /// Request header constructor
        /// </summary>
        /// <param name="p_s_domain">set domain value for reading referrer correctly</param>
        /// <exception cref="ArgumentNullException">domain parameter is null or empty</exception>
        public RequestHeader(string p_s_domain)
        {
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_domain))
            {
                throw new ArgumentNullException(nameof(p_s_domain), "Domain parameter is null or empty");
            }

            this.s_domain = p_s_domain;
            this.s_method = "";
            this.s_requestPath = "";
            this.s_convertedPath = "";
            this.s_path = "";
            this.s_file = "";
            this.a_parameters = [];
            this.s_protocol = "";
            this.b_connectionClosed = false;
            this.i_contentLength = -1;
            this.s_contentType = "";
            this.s_boundary = "";
            this.s_cookie = "";
            this.s_host = "";
            this.i_port = -1;
            this.s_referrer = "";
            this.s_userAgent = "";
            this.s_accept = "";
            this.s_acceptCharset = "";
            this.b_noCacheControl = true;
            this.s_authorization = "";
        }

        /// <summary>
        /// Parse request http header to this instance and fill all recognized properties
        /// </summary>
        /// <param name="p_a_requestHeaderLines">all http request header lines</param>
        /// <returns>http status code</returns>
        public int ParseRequestHeader(string[] p_a_requestHeaderLines)
        {
            return this.ParseRequestHeader(p_a_requestHeaderLines, false);
        }

        /// <summary>
        /// Parse request http header to this instance and fill all recognized properties
        /// </summary>
        /// <param name="p_a_requestHeaderLines">all http request header lines</param>
        /// <param name="p_b_no301">true - request is not a file, so we just add a '/' at the end and redirect with 301</param>
        /// <returns>http status code</returns>
        public int ParseRequestHeader(string[] p_a_requestHeaderLines, bool p_b_no301)
        {
            /* check if http request header lines are not empty, otherwise we have an empty message */
            if ((p_a_requestHeaderLines == null) || (p_a_requestHeaderLines.Length < 1))
            {
                ForestNET.Lib.Global.ILogWarning("400 Bad Request: No request message available");
                return 400;
            }

            string s_firstLine = p_a_requestHeaderLines[0];

            /* first line must contain 2 white spaces */
            if (!s_firstLine.Contains(' '))
            {
                ForestNET.Lib.Global.ILogWarning("400 Bad Request: Invalid first line of request '" + s_firstLine + "'");
                return 400;
            }

            /* split all values from first line */
            string[] a_firstLine = s_firstLine.Split(" ");

            /* first line must contain 3 values: method, path and protocol */
            if (a_firstLine.Length != 3)
            {
                ForestNET.Lib.Global.ILogWarning("400 Bad Request: Invalid amount of data in first line of request '" + s_firstLine + "', must be 'HTTP-Method Path HTTP-Protocol'");
                return 400;
            }

            /* check http method value */
            if (a_firstLine[0].Equals("GET"))
            {
                this.s_method = "GET";
            }
            else if (a_firstLine[0].Equals("POST"))
            {
                this.s_method = "POST";
            }
            else if (a_firstLine[0].Equals("PUT"))
            {
                this.s_method = "PUT";
            }
            else if (a_firstLine[0].Equals("DELETE"))
            {
                this.s_method = "DELETE";
            }
            else
            {
                ForestNET.Lib.Global.ILogWarning("405 Method Not Allowed: Invalid HTTP-Method in request '" + a_firstLine[0] + "', must be [GET|POST|PUT|DELETE]'");
                return 405;
            }

            /* check if http protocol is 1.0 or 1.1 */
            if ((a_firstLine[2].Equals("HTTP/1.1")) || (a_firstLine[2].Equals("HTTP/1.0")))
            {
                this.s_protocol = a_firstLine[2];
            }
            else
            {
                ForestNET.Lib.Global.ILogWarning("505 HTTP Version Not Supported: Unsupported HTTP-Protocol in request '" + a_firstLine[2] + "', must be [HTTP/1.0|HTTP/1.1]'");
                return 505;
            }


            /* assume request path from first line */
            this.s_requestPath = a_firstLine[1];

            /* request path must start with '/' */
            if (!this.s_requestPath.StartsWith("/"))
            {
                ForestNET.Lib.Global.ILogWarning("400 Bad Request: Request path does not start with '/'");
                return 400;
            }

            /* iterate all other request header lines */
            foreach (string s_requestHeaderLine in p_a_requestHeaderLines)
            {
                /* header name and value are separated by a ':' */
                if (s_requestHeaderLine.Contains(':'))
                {
                    /* get header name */
                    string s_headerName = s_requestHeaderLine.Substring(0, s_requestHeaderLine.IndexOf(":")).Trim().ToLower();
                    /* get header value */
                    string s_headerValue = s_requestHeaderLine.Substring(s_requestHeaderLine.IndexOf(":") + 1, s_requestHeaderLine.Length - (s_requestHeaderLine.IndexOf(":") + 1)).Trim();

                    switch (s_headerName)
                    {
                        case "connection":
                            if (s_headerValue.ToLower().Equals("close"))
                            {
                                this.b_connectionClosed = true;
                            }
                            break;
                        case "content-length":
                            /* parse content length to an integer value */
                            if (ForestNET.Lib.Helper.IsInteger(s_headerValue))
                            {
                                this.i_contentLength = int.Parse(s_headerValue);
                            }
                            else
                            {
                                ForestNET.Lib.Global.ILogWarning("400 Bad Request: Invalid value for 'Content-Length' request header '" + s_headerValue + "', must be an integer value'");
                                return 400;
                            }
                            break;
                        case "content-type":
                            /* recognize boundary value for POST requests */
                            if (s_headerValue.Contains("boundary"))
                            {
                                this.s_contentType = s_headerValue.Substring(0, s_headerValue.IndexOf(";")).Trim();
                                this.s_boundary = s_headerValue.Substring(s_headerValue.IndexOf("boundary=") + 9).Trim();
                            }
                            else
                            {
                                this.s_contentType = s_headerValue;
                            }
                            break;
                        case "cookie":
                            this.s_cookie = s_headerValue;
                            break;
                        case "host":
                            this.s_host = s_headerValue;
                            break;
                        case "referer":
                            this.s_referrer = s_headerValue;
                            break;
                        case "user-agent":
                            this.s_userAgent = s_headerValue;
                            break;
                        case "accept":
                            this.s_accept = s_headerValue;
                            break;
                        case "accept-charset":
                            this.s_acceptCharset = s_headerValue;
                            break;
                        case "authorization":
                            this.s_authorization = s_headerValue;
                            break;
                    }
                }
            }

            /* convert request path to a system directory path */
            this.s_convertedPath = this.s_requestPath.Replace('/', ForestNET.Lib.IO.File.DIR);

            /* do we request an element within a path or do we stay on top-level? */
            if (!this.s_convertedPath.Equals(ForestNET.Lib.IO.File.DIR))
            {
                /* remove first directory character */
                this.s_convertedPath = this.s_convertedPath.Substring(1);

                string s_tempPath = "";
                string s_tempFile;
                string s_tempParameters;

                /* fill temp path from start of converted path to last folder */
                if (this.s_convertedPath.Contains(ForestNET.Lib.IO.File.DIR))
                {
                    s_tempPath = this.s_convertedPath.Substring(0, this.s_convertedPath.LastIndexOf(ForestNET.Lib.IO.File.DIR));
                }

                /* remove last directory character if converted path ends with 'wsdl\' or 'WSDL\' */
                if ((this.s_convertedPath.EndsWith("wsdl" + ForestNET.Lib.IO.File.DIR)) || (this.s_convertedPath.EndsWith("WSDL" + ForestNET.Lib.IO.File.DIR)))
                {
                    this.s_convertedPath = this.s_convertedPath.Substring(0, this.s_convertedPath.Length - 1);
                }

                /* check for request parameters in request url */
                if (this.s_convertedPath.Contains('?'))
                {
                    /* read file and parameters from path */
                    s_tempFile = this.s_convertedPath.Substring(this.s_convertedPath.LastIndexOf(ForestNET.Lib.IO.File.DIR) + 1, this.s_convertedPath.LastIndexOf("?") - (this.s_convertedPath.LastIndexOf(ForestNET.Lib.IO.File.DIR) + 1));
                    s_tempParameters = this.s_convertedPath.Substring(this.s_convertedPath.LastIndexOf("?") + 1);

                    string[] a_parameters;

                    /* check if we have multiple request parameters divided by '&' or just one key value parameter */
                    if (this.s_convertedPath.Contains('&'))
                    {
                        a_parameters = s_tempParameters.Split("&");
                    }
                    else
                    {
                        a_parameters = new string[] { s_tempParameters };
                    }

                    /* iterate each request parameter */
                    foreach (string s_parameterPair in a_parameters)
                    {
                        /* key and value are divided by '=' */
                        if (s_parameterPair.Contains('='))
                        {
                            /* get request parameter key and value */
                            string s_parameterKey = s_parameterPair.Substring(0, s_parameterPair.IndexOf("="));
                            string s_parameterValue = s_parameterPair.Substring(s_parameterPair.IndexOf("=") + 1);

                            /* decode request parameter key and value, because they could be encoded, e.g. ' ' -> '%20' */
                            try
                            {
                                s_parameterKey = System.Web.HttpUtility.UrlDecode(s_parameterKey, System.Text.Encoding.UTF8);
                                s_parameterValue = System.Web.HttpUtility.UrlDecode(s_parameterValue, System.Text.Encoding.UTF8);
                            }
                            catch (Exception o_exc)
                            {
                                ForestNET.Lib.Global.ILogSevere("500 Internal Server Error: could not decode URI parameter pair with encoding 'UTF-8': " + o_exc);
                                return 500;
                            }

                            this.a_parameters.Add(s_parameterKey, s_parameterValue);
                        }
                        else
                        { /* or the key has just no value -> null */
                            this.a_parameters.Add(s_parameterPair, null);
                        }
                    }
                }
                else
                { /* no request parameters, just read request file from converted path */
                    s_tempFile = this.s_convertedPath.Substring(this.s_convertedPath.LastIndexOf(ForestNET.Lib.IO.File.DIR) + 1);
                }

                /* check if we automatically do a redirect with 301, if parameter is false, file from converted path is empty, file from converted path has no '.' and request parameter wsdl/WSDL does not exist in request */
                if ((!p_b_no301) && (!ForestNET.Lib.Helper.IsStringEmpty(s_tempFile)) && (!s_tempFile.Contains('.')) && (!this.a_parameters.ContainsKey("wsdl")) && (!this.a_parameters.ContainsKey("WSDL")))
                {
                    ForestNET.Lib.Global.ILogWarning("301 Moved Permanently: Request is not a file '" + s_tempFile + "', so we just add a '/' at the end and redirect");
                    return 301;
                }

                /* decode request path and file, because they could be encoded, e.g. ' ' -> '%20' */
                try
                {
                    this.s_path = System.Web.HttpUtility.UrlDecode(s_tempPath, System.Text.Encoding.UTF8);
                    this.s_file = System.Web.HttpUtility.UrlDecode(s_tempFile, System.Text.Encoding.UTF8);
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.ILogSevere("500 Internal Server Error: could not decode URI path and file with encoding 'UTF-8': " + o_exc);
                    return 500;
                }

                /* if referer in header is not empty and does not contain a '.' at the end */
                if ((!ForestNET.Lib.Helper.IsStringEmpty(this.s_referrer)) && (!this.s_referrer.Substring(this.s_referrer.LastIndexOf("/")).Contains('.')))
                {
                    /* get relative path without domain */
                    string s_relativePath = this.s_referrer.Substring(this.s_domain.Length);

                    /* remove first '/' character */
                    if (s_relativePath.StartsWith("/"))
                    {
                        s_relativePath = s_relativePath.Substring(1);
                    }

                    /* add '/' at the end if it is missing */
                    if (!s_relativePath.EndsWith("/"))
                    {
                        s_relativePath += "/";
                    }

                    /* convert relative path to a system directory path */
                    s_relativePath = s_relativePath.Replace('/', ForestNET.Lib.IO.File.DIR);

                    /* if it is not starting with relative path, change path value with relative value */
                    if (!this.s_path.StartsWith(s_relativePath))
                    {
                        this.s_path = s_relativePath + this.s_path;
                    }
                }
            }

            /* check if request path URI is not to long */
            if (this.s_requestPath.Length > 2048)
            {
                ForestNET.Lib.Global.ILogWarning("414 URI Too Long: Request uri to long[" + this.s_requestPath.Length + "]");
                return 414;
            }

            /* http request header passed successfully, return 200 */
            return 200;
        }

        /// <summary>
        /// this method will return a http 1.0/1.1 valid request header
        /// </summary>
        public override string ToString()
        {
            string s_foo = "";
            string s_foo2 = this.s_method;

            if (this.s_method.Equals("DOWNLOAD"))
            {
                s_foo2 = "GET";
            }

            /* create first line with method, path and protocol */
            s_foo += s_foo2 + " " + ((!ForestNET.Lib.Helper.IsStringEmpty(this.s_requestPath)) ? this.s_requestPath : "/") + " HTTP/1.1" + Config.HTTP_LINEBREAK;

            /* add accept value if it is not empty */
            if (!ForestNET.Lib.Helper.IsStringEmpty(this.s_accept))
            {
                s_foo += "Accept: " + this.s_accept + Config.HTTP_LINEBREAK;
            }

            /* add accept charset value if it is not empty */
            if (!ForestNET.Lib.Helper.IsStringEmpty(this.s_acceptCharset))
            {
                s_foo += "Accept-Charset: " + this.s_acceptCharset + Config.HTTP_LINEBREAK;
            }

            /* set no cache control if it is set to true */
            if (this.b_noCacheControl)
            {
                s_foo += "Cache-Control: no-cache" + Config.HTTP_LINEBREAK;
            }

            /* set connection to 'keep-alive' or 'close' */
            s_foo += "Connection: " + ((!this.b_connectionClosed) ? "keep-alive" : "close") + Config.HTTP_LINEBREAK;

            /* add further keep-alive settings */
            if (!this.b_connectionClosed)
            {
                s_foo += "Keep-Alive: timeout=5, max=10" + Config.HTTP_LINEBREAK;
            }

            /* add host value, is required for any request */
            if (ForestNET.Lib.Helper.IsStringEmpty(this.s_host))
            {
                throw new ArgumentException("No host value specified for https client header");
            }
            else
            {
                s_foo += "Host: " + this.s_host + Config.HTTP_LINEBREAK;
            }

            /* add user agent value, from properties or from constant */
            if (ForestNET.Lib.Helper.IsStringEmpty(this.s_userAgent))
            {
                s_foo += "User-Agent: " + RequestHeader.CLIENT + Config.HTTP_LINEBREAK;
            }
            else
            {
                s_foo += "User-Agent: " + this.s_userAgent + Config.HTTP_LINEBREAK;
            }

            /* add content type value if it is not empty */
            if (!ForestNET.Lib.Helper.IsStringEmpty(this.s_contentType))
            {
                s_foo += "Content-Type: " + this.s_contentType + Config.HTTP_LINEBREAK;
            }

            /* add content length if it is greater equal zero */
            if (this.i_contentLength >= 0)
            {
                s_foo += "Content-Length: " + this.i_contentLength + Config.HTTP_LINEBREAK;
            }

            /* add cookie value if it is not empty */
            if (!ForestNET.Lib.Helper.IsStringEmpty(this.s_cookie))
            {
                s_foo += "Cookie: " + this.s_cookie + Config.HTTP_LINEBREAK;
            }

            /* add authorization value if it is not empty */
            if (!ForestNET.Lib.Helper.IsStringEmpty(this.s_authorization))
            {
                s_foo += "Authorization: " + this.s_authorization + Config.HTTP_LINEBREAK;
            }

            /* add line break, so we have two line breaks after http request header - this will always separate request header from request body(POST) */
            s_foo += Config.HTTP_LINEBREAK;

            return s_foo;
        }
    }
}