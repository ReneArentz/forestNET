namespace ForestNET.Lib.Net.Https
{
    /// <summary>
    /// HTTP Response Header class which can hold all necessary minimal header information for a response header and a parsing method to parse received response to this object class.
    /// </summary>
    public class ResponseHeader
    {

        /* Constants */

        public const string SERVER = "forestNET Tiny Https Server 1.0";
        public static readonly Dictionary<int, string> RETURN_CODES = new() {
            { 200, "OK" },
            { 204, "No Content" },
            { 301, "Moved Permanently" },
            { 400, "Bad Request" },
            { 401, "Unauthorized" },
            { 403, "Forbidden" },
            { 404, "Not Found" },
            { 405, "Method Not Allowed" },
            { 413, "Payload Too Large" },
            { 414, "URI Too Long" },
            { 415, "Unsupported Media Type" },
            { 421, "Misdirected Request" },
            { 500, "Internal Server Error" },
            { 501, "Not Implemented" },
            { 505, "HTTP Version Not Supported" }
        };

        /* Fields */

        private int i_returnCode;
        private string s_returnMessage;
        private int i_contentLength;
        private string s_contentType;
        private ForestNET.Lib.Net.Https.Dynm.Cookie? o_cookie;
        private DateTime? o_lastModified;
        private bool b_connectionKeepAlive;
        private string s_boundary;
        private string s_server;
        private string? s_keepAlive;

        /* Properties */

        /// <summary>
        /// A valid HTTP return code
        /// </summary>
        /// <exception cref="ArgumentException">invalid HTTP return code, or a return code which is not intended by forestNET</exception>
        public int ReturnCode
        {
            get
            {
                return this.i_returnCode;
            }
            set
            {
                /* check return code for response */
                if (!ResponseHeader.RETURN_CODES.ContainsKey(value))
                {
                    throw new ArgumentException("Invalid return code '" + value + "', use one of these return codes: [" + ForestNET.Lib.Helper.JoinList(ResponseHeader.RETURN_CODES.Values.ToList(), '|') + "]");
                }

                this.i_returnCode = value;
            }
        }
        public string ReturnMessage
        {
            get
            {
                return this.s_returnMessage;
            }
            set
            {
                this.s_returnMessage = value;
            }
        }
        public int ContentLength
        {
            get
            {
                return this.i_contentLength;
            }
            set
            {
                this.i_contentLength = value;
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
        public ForestNET.Lib.Net.Https.Dynm.Cookie? Cookie
        {
            get
            {
                return this.o_cookie;
            }
            set
            {
                this.o_cookie = value;
            }
        }
        public DateTime? LastModified
        {
            get
            {
                return this.o_lastModified;
            }
            set
            {
                this.o_lastModified = value;
            }
        }
        public bool ConnectionKeepAlive
        {
            get
            {
                return this.b_connectionKeepAlive;
            }
            set
            {
                this.b_connectionKeepAlive = value;
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
        public string Server
        {
            get
            {
                return this.s_server;
            }
            private set
            {
                this.s_server = value;
            }
        }
        public string? KeepAlive
        {
            get
            {
                return this.s_keepAlive;
            }
            private set
            {
                this.s_keepAlive = value;
            }
        }

        /* Methods */

        /// <summary>
        /// Response header constructor, initialize basic values
        /// </summary>
        public ResponseHeader()
        {
            this.i_contentLength = -1;
            this.s_contentType = "";
            this.o_cookie = null;
            this.o_lastModified = null;
            this.b_connectionKeepAlive = false;
            this.s_boundary = "";
            this.s_server = "";
            this.s_keepAlive = null;
            this.s_returnMessage = "";
        }

        /// <summary>
        /// Parse response http header to this instance and fill all recognized properties
        /// </summary>
        /// <param name="p_a_responseHeaderLines">all http response header lines</param>
        /// <returns>http status code</returns>
        public int ParseResponseHeader(string[] p_a_responseHeaderLines)
        {
            /* check if http response header lines are not empty, otherwise we have an empty message */
            if ((p_a_responseHeaderLines == null) || (p_a_responseHeaderLines.Length < 1))
            {
                ForestNET.Lib.Global.ILogWarning("400 Bad Request: No response message available");
                return 400;
            }

            string s_firstLine = p_a_responseHeaderLines[0];

            /* first line must contain 2 white spaces */
            if (!s_firstLine.Contains(' '))
            {
                ForestNET.Lib.Global.ILogWarning("400 Bad Request: Invalid first line of response '" + s_firstLine + "'");
                return 400;
            }

            /* split all values from first line */
            string[] a_firstLine = s_firstLine.Split(" ");

            /* first line must contain 3 values: protocol, return code and return message */
            if (a_firstLine.Length < 3)
            {
                ForestNET.Lib.Global.ILogWarning("400 Bad Request: Invalid amount of data in first line of response '" + s_firstLine + "', must be 'HTTP-Protocol Return-Code Return-Message'");
                return 400;
            }

            /* check if http protocol is 1.0 or 1.1 */
            if (!((a_firstLine[0].Equals("HTTP/1.1")) || (a_firstLine[0].Equals("HTTP/1.0"))))
            {
                ForestNET.Lib.Global.ILogWarning("505 HTTP Version Not Supported: Unsupported HTTP-Protocol in response '" + a_firstLine[0] + "', must be [HTTP/1.0|HTTP/1.1]'");
                return 505;
            }

            /* check if return code is an integer value */
            if (!ForestNET.Lib.Helper.IsInteger(a_firstLine[1]))
            {
                ForestNET.Lib.Global.ILogWarning("400 Bad Request: Invalid value for 'Return-Code' response header '" + a_firstLine[1] + "', must be an integer value'");
                return 400;
            }

            /* get return code */
            this.i_returnCode = int.Parse(a_firstLine[1]);

            /* get return message from the seconds white space on forward 'HTTP/1.1 200 ...' */
            this.s_returnMessage = s_firstLine.Substring(s_firstLine.IndexOf(" ", 12)).Trim();

            /* iterate all other response header lines */
            foreach (string s_responseHeaderLine in p_a_responseHeaderLines)
            {
                /* header name and value are separated by a ':' */
                if (s_responseHeaderLine.Contains(':'))
                {
                    /* get header name */
                    string s_headerName = s_responseHeaderLine.Substring(0, s_responseHeaderLine.IndexOf(":")).Trim().ToLower();
                    /* get header value */
                    string s_headerValue = s_responseHeaderLine.Substring(s_responseHeaderLine.IndexOf(":") + 1).Trim();

                    switch (s_headerName)
                    {
                        case "connection":
                            if (s_headerValue.ToLower().Equals("close"))
                            {
                                this.b_connectionKeepAlive = false;
                            }
                            else
                            {
                                this.b_connectionKeepAlive = true;
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
                                ForestNET.Lib.Global.ILogWarning("400 Bad Request: Invalid value for 'Content-Length' response header '" + s_headerValue + "', must be an integer value'");
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
                        case "set-cookie":
                            /* parse cookie line which can have a lot of values and settings, using Cookie class */
                            ForestNET.Lib.Net.Https.Dynm.Cookie o_cookie = new(null, null);

                            /* split cookie line with ';' */
                            string[] a_foo = s_headerValue.Split(";");
                            bool b_first = false;

                            /* iterate all cookie values and settings */
                            foreach (string s_foo in a_foo)
                            {
                                /* cookie key and value are separated by '=' */
                                string[] a_foo2 = s_foo.Split("=");

                                if (!b_first)
                                {
                                    /* set key and value and store this pair as usual http cookie */
                                    o_cookie.Key = a_foo2[0].Trim();
                                    o_cookie.Value = a_foo2[1];
                                    o_cookie.AddHTTPCookie(o_cookie.Key, o_cookie.Value);
                                    b_first = true;
                                }
                                else
                                {
                                    /* parse all other values and settings */
                                    switch (a_foo2[0].Trim().ToLower())
                                    {
                                        case "domain":
                                            o_cookie.Domain = a_foo2[1];
                                            break;
                                        case "path":
                                            o_cookie.Path = a_foo2[1];
                                            break;
                                        case "secure":
                                            o_cookie.Secure = true;
                                            break;
                                        case "httponly":
                                            o_cookie.HttpOnly = true;
                                            break;
                                        case "samesite":
                                            ForestNET.Lib.Net.Https.Dynm.CookieSameSite e_sameSite = ForestNET.Lib.Net.Https.Dynm.CookieSameSite.NONE;

                                            if (a_foo2[1].Trim().ToLower().Equals("none"))
                                            {
                                                e_sameSite = ForestNET.Lib.Net.Https.Dynm.CookieSameSite.NONE;
                                            }
                                            else if (a_foo2[1].Trim().ToLower().Equals("lax"))
                                            {
                                                e_sameSite = ForestNET.Lib.Net.Https.Dynm.CookieSameSite.LAX;
                                            }
                                            else if (a_foo2[1].Trim().ToLower().Equals("strict"))
                                            {
                                                e_sameSite = ForestNET.Lib.Net.Https.Dynm.CookieSameSite.STRICT;
                                            }

                                            o_cookie.SameSite = e_sameSite;
                                            break;
                                        case "expires":
                                            o_cookie.Expires = a_foo2[1];
                                            break;
                                        case "max-age":
                                            /* check if max-age is a valid long value */
                                            if (!ForestNET.Lib.Helper.IsLong(a_foo2[1]))
                                            {
                                                ForestNET.Lib.Global.ILogWarning("400 Bad Request: Invalid value for 'Max-Age' response header cookie '" + s_foo + "', must be a long value'");
                                                return 400;
                                            }
                                            else
                                            {
                                                o_cookie.MaxAgeAsLong = long.Parse(a_foo2[1]);
                                            }
                                            break;
                                        default:
                                            /* or just add another key and value pair as usual http cookie */
                                            o_cookie.AddHTTPCookie(a_foo2[0].Trim(), a_foo2[1]);
                                            break;
                                    }
                                }
                            }

                            /* store all cookie information to property */
                            this.o_cookie = o_cookie;
                            break;
                        case "server":
                            this.s_server = s_headerValue;
                            break;
                        case "keep_alive":
                            this.s_keepAlive = s_headerValue;
                            break;
                    }
                }
            }

            /* http response header passed successfully, return 200 */
            return 200;
        }

        /// <summary>
        /// this method will return a http 1.0/1.1 valid response header
        /// </summary>
        public override string ToString()
        {
            return this.ToString(false);
        }

        /// <summary>
        /// this method will return a http 1.0/1.1 valid response header
        /// </summary>
        /// <param name="p_b_received">true - print response header information of a received header</param>
        public string ToString(bool p_b_received)
        {
            string s_foo = "";

            /* first line of response header: protocol, return code and return message */
            s_foo += "HTTP/1.1 " + this.i_returnCode + " " + ResponseHeader.RETURN_CODES[this.i_returnCode] + Config.HTTP_LINEBREAK;

            /* if this is a 301 response, we return this header immediately */
            if (this.i_returnCode == 301)
            {
                return s_foo;
            }

            /* add Date, Allow and Cache-Control */
            s_foo += "Date: " + ForestNET.Lib.Helper.ToRFC1123(DateTime.Now) + Config.HTTP_LINEBREAK;
            s_foo += "Allow: GET, POST, PUT, DELETE" + Config.HTTP_LINEBREAK;
            s_foo += "Cache-Control: no-cache" + Config.HTTP_LINEBREAK;

            /* keep alive flag is set, but not recommended with current ReceiveTCP structure */
            if (this.b_connectionKeepAlive)
            {
                s_foo += "Connection: Keep-Alive" + Config.HTTP_LINEBREAK;

                if (!p_b_received)
                {
                    s_foo += "Keep-Alive: timeout=5, max=10" + Config.HTTP_LINEBREAK;
                }
            }
            else
            {
                /* set connection to close */
                s_foo += "Connection: Close" + Config.HTTP_LINEBREAK;
            }

            /* add content length and content type */
            if (this.i_contentLength > 0)
            {
                s_foo += "Content-Length: " + this.i_contentLength + Config.HTTP_LINEBREAK;
                s_foo += "Content-Type: " + this.s_contentType + Config.HTTP_LINEBREAK;
            }

            /* call cookie tostring method to add cookie to response header */
            if (this.o_cookie != null)
            {
                s_foo += this.o_cookie.ToString() + Config.HTTP_LINEBREAK;
            }

            /* add last modified to response header */
            if (this.o_lastModified != null)
            {
                s_foo += "Last-Modified: " + ForestNET.Lib.Helper.ToRFC1123(this.o_lastModified ?? throw new NullReferenceException("Last modified is null")) + Config.HTTP_LINEBREAK;
            }

            if (p_b_received)
            { /* show keep alive and server values from received response header */
                if (this.s_keepAlive != null)
                {
                    s_foo += "Keep-Alive: " + this.s_keepAlive + Config.HTTP_LINEBREAK;
                }

                s_foo += "Server: " + this.s_server + Config.HTTP_LINEBREAK;
            }
            else
            {
                /* set server constant for own response header */
                s_foo += "Server: " + ResponseHeader.SERVER + Config.HTTP_LINEBREAK;
            }

            /* add line break, so we have two line breaks after http response header - this will always separate response header from response body(POST) */
            s_foo += Config.HTTP_LINEBREAK;

            return s_foo;
        }
    }
}