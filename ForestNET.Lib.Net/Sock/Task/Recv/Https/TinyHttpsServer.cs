namespace ForestNET.Lib.Net.Sock.Task.Recv.Https
{
    /// <summary>
    /// Task class to create a tiny https server. You can configure this instance as https, soap or rest server which can receive request and send responses to the clients. All internal data is handled in properties within Seed instance. Only TCP supported.
    /// Only HTTP/1.0 or HTTP/1.1.
    /// </summary>
    public class TinyHttpsServer : ForestNET.Lib.Net.Sock.Task.Task
    {

        /* Fields */

        /* Properties */

        public ForestNET.Lib.Net.Https.Seed? Seed { get; private set; }
        private ForestNET.Lib.Net.Https.Dynm.Dynamic? Dynamic { get; set; }
        private Object? SOAPResponse { get; set; }
        private string? SOAPTargetNamespace { get; set; }
        private string? SOAPNamespaceInitial { get; set; }

        /* Methods */

        /// <summary>
        /// Standard constructor, initializing all values
        /// </summary>
        public TinyHttpsServer() : base()
        {
            this.Seed = null;
            this.Dynamic = null;
            this.SOAPResponse = null;
            this.SOAPTargetNamespace = null;
            this.SOAPNamespaceInitial = null;
        }

        /// <summary>
        /// Creating tiny https server task instance with all it's settings via configuration parameter
        /// </summary>
        /// <param name="p_o_config">configuration instance parameter</param>
        /// <exception cref="ArgumentNullException">configuration instance parameter is null, or domain parameter from configuration is null or empty</exception>
        public TinyHttpsServer(ForestNET.Lib.Net.Https.Config p_o_config) : base(ForestNET.Lib.Net.Sock.Type.TCP_TLS_SERVER)
        {
            this.Seed = new ForestNET.Lib.Net.Https.Seed(p_o_config);
            this.Dynamic = null;
            this.SOAPResponse = null;
            this.SOAPTargetNamespace = null;
            this.SOAPNamespaceInitial = null;
            this.ReceiveMaxUnknownAmountInMiB = this.Seed.Config.MaxPayload;
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
            try { this.Seed = ((TinyHttpsServer)p_o_sourceTask).Seed; } catch (Exception) { /* NOP */ }
        }

        /// <summary>
        /// runTask method of receiving https/soap/rest requests
        /// </summary>
        /// <exception cref="Exception">any exception of implementation that could happen will be caught by abstract Task class, see details in protocol methods in de.forestj.lib.net.sock.task.Task</exception>
        public override async System.Threading.Tasks.Task RunTask()
        {
            /* only TCP supported */
            if ((this.Type == ForestNET.Lib.Net.Sock.Type.TCP_SERVER) || (this.Type == ForestNET.Lib.Net.Sock.Type.TCP_TLS_SERVER))
            {
                /* check if seed instance is set */
                if (this.Seed == null)
                {
                    throw new NullReferenceException("Seed instance is null");
                }

                try
                {
                    bool b_keepAlive = false;

                    /* repeat until keep alive is set to false */
                    do
                    {
                        /* re-initialize variables */
                        this.Seed = new ForestNET.Lib.Net.Https.Seed(this.Seed.Config);
                        this.Dynamic = null;
                        this.SOAPResponse = null;
                        this.SOAPTargetNamespace = null;
                        this.SOAPNamespaceInitial = null;

                        /* generate new salt value */
                        this.Seed.Salt = ForestNET.Lib.Helper.GenerateRandomString(8, ForestNET.Lib.Helper.ALPHANUMERIC_CHARACTERS);

                        if (b_keepAlive)
                        {
                            b_keepAlive = false;
                        }

                        /* check if Seed instance is available */
                        if (this.Seed == null)
                        {
                            throw new ArgumentException("Configuration for tiny https server is not specified");
                        }

                        /* check if domain property is configured */
                        if (ForestNET.Lib.Helper.IsStringEmpty(this.Seed.Config.Domain))
                        {
                            throw new ArgumentException("Configuration for tiny https server must specify a domain value");
                        }

                        /* get request source address and port */
                        System.Net.IPEndPoint o_requestSourceAddress = (System.Net.IPEndPoint)(this.ReceivingSocket?.RemoteEndPoint ?? throw new NullReferenceException("Remote end point of receiving socket is null or receiving socket is null"));
                        int i_requestSourcePort = o_requestSourceAddress.Port;

                        ForestNET.Lib.Global.ILog(this.Seed.Salt + " " + "handle incoming socket communication from " + o_requestSourceAddress);

                        ForestNET.Lib.Global.ILogFine(this.Seed.Salt + " " + "handle request");

                        /* handle incoming request */
                        this.Seed.ReturnCode = await this.HandleRequest(o_requestSourceAddress);

                        ForestNET.Lib.Global.ILogFine(this.Seed.Salt + " " + "handled request");

                        /* log POST data - because of DATA PROTECTION not active */
                        /*if (this.Seed.PostData.Count > 0)
                        {
                            ForestNET.Lib.Global.ILogMass(this.Seed.Salt);
                            ForestNET.Lib.Global.ILogMass(this.Seed.Salt + " " + "Post data");
                            ForestNET.Lib.Global.ILogMass(this.Seed.Salt);

                            foreach (KeyValuePair<string, string?> o_postDataEntry in this.Seed.PostData)
                            {
                                ForestNET.Lib.Global.ILogMass(this.Seed.Salt + " " + o_postDataEntry.Key + " = " + o_postDataEntry.Value);
                            }

                            ForestNET.Lib.Global.ILogMass(this.Seed.Salt);
                        }*/

                        /* log FILE data - because of DATA PROTECTION not active */
                        /*if (this.Seed.FileData.Count > 0)
                        {
                            ForestNET.Lib.Global.ILogMass(this.Seed.Salt);
                            ForestNET.Lib.Global.ILogMass(this.Seed.Salt + " " + "File data");
                            ForestNET.Lib.Global.ILogMass(this.Seed.Salt);

                            foreach (ForestNET.Lib.Net.Https.Dynm.FileData o_fileDataEntry in this.Seed.FileData)
                            {
                                ForestNET.Lib.Global.ILogMass(this.Seed.Salt + " " + o_fileDataEntry.FieldName + " | " + o_fileDataEntry.FileName + " | " + o_fileDataEntry.ContentType + " | " + o_fileDataEntry.Data?.Length);
                            }

                            ForestNET.Lib.Global.ILogMass(this.Seed.Salt);
                        }*/

                        ForestNET.Lib.Global.ILog(this.Seed.Salt + " " + "sending response [" + this.Seed.ReturnCode + "] for '" + this.Seed.RequestHeader.RequestPath + "' to [" + o_requestSourceAddress + "]");

                        await this.SendResponse();

                        ForestNET.Lib.Global.ILog(this.Seed.Salt + " " + "sent response");

                        if (!this.Seed.ResponseHeader.ConnectionKeepAlive)
                        {
                            /* closing connection, because we are not keeping the connection alive */
                            this.ReceivingSocket?.Shutdown(System.Net.Sockets.SocketShutdown.Both);
                            this.ReceivingSocket?.Close();

                            ForestNET.Lib.Global.ILogFine(this.Seed.Salt + " " + "socket communication closed with [" + i_requestSourcePort + "]");
                        }
                        else
                        {
                            /* not recommended */
                            b_keepAlive = true;
                        }
                    } while (b_keepAlive);
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.ILogSevere(this.Seed.Salt + " " + "Exception runTask: " + o_exc);

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
        /// handle incoming https/soap/rest request
        /// </summary>
        /// <param name="p_o_requestSourceAddress">IPEndPoint parameter object for ipv4 check</param>
        /// <returns>http status return code, e.g. 200 or 404</returns>
        private async System.Threading.Tasks.Task<int> HandleRequest(System.Net.IPEndPoint p_o_requestSourceAddress)
        {
            /* check if seed instance is set */
            if (this.Seed == null)
            {
                throw new NullReferenceException("Seed instance is null");
            }

            byte[]? a_receivedData;

            ForestNET.Lib.Global.ILogFine(this.Seed.Salt + " " + "receive data from client");

            /* receive https response */
            try
            {
                a_receivedData = await this.ReceiveBytes(-1);
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.ILogSevere(this.Seed.Salt + " " + "500 Internal Server Error: Exception while receiving data; " + o_exc);
                return 500;
            }

            ForestNET.Lib.Global.ILogFine(this.Seed.Salt + " " + "received data from client");

            /* check if request does not exceed max. payload */
            if ((a_receivedData != null) && (a_receivedData.Length >= this.Seed.Config.MaxPayload * 1024 * 1024))
            {
                ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "413 Payload Too Large: Reached max. payload of '" + ForestNET.Lib.Helper.FormatBytes(this.Seed.Config.MaxPayload * 1024 * 1024) + "'");
                return 413;
            }

            /* get byte position in request data, where we have the border between http header and body data */
            int i_borderHeaderFromContent = ForestNET.Lib.Net.Https.Dynm.Dynamic.GetNextLineBreak(a_receivedData ?? [], this.Seed.Config.InEncoding ?? System.Text.Encoding.UTF8, 0, true, ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK);

            /* check if we have two line breaks */
            if ((i_borderHeaderFromContent < 0) || (a_receivedData == null))
            {
                ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "400 Bad Request: HTTP request does not contain two line breaks");
                return 400;
            }
            else
            {
                i_borderHeaderFromContent += ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK.Length * 2;
            }

            /* check if we have a request body */
            if (i_borderHeaderFromContent < a_receivedData.Length)
            {
                /* define request body byte array */
                this.Seed.RequestBody = new byte[a_receivedData.Length - i_borderHeaderFromContent];

                /* get request body data */
                for (int i = i_borderHeaderFromContent; i < a_receivedData.Length; i++)
                {
                    this.Seed.RequestBody[i - i_borderHeaderFromContent] = a_receivedData[i];
                }
            }

            byte[] a_headerData = new byte[i_borderHeaderFromContent - 4];

            /* get request header data */
            for (int i = 0; i < i_borderHeaderFromContent - 4; i++)
            {
                a_headerData[i] = a_receivedData[i];
            }

            /* retrieve request header lines from header byte data, use incoming encoding setting */
            string[] a_requestHeaderLines = (this.Seed.Config.InEncoding ?? System.Text.Encoding.UTF8).GetString(a_headerData).Split(ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK);

            /* read request header, do not automatically return 301 in SOAP or REST mode */
            int i_foo = this.Seed.RequestHeader.ParseRequestHeader(a_requestHeaderLines, ((this.Seed.Config.Mode == ForestNET.Lib.Net.Https.Mode.SOAP) || (this.Seed.Config.Mode == ForestNET.Lib.Net.Https.Mode.REST)));

            /* if we have not return code 200, exit here */
            if (i_foo != 200)
            {
                return i_foo;
            }

            /* check incoming ip address */
            if (this.Seed.Config.AllowSourceList.Count > 0)
            {
                ForestNET.Lib.Global.ILogFine(this.Seed.Salt + " " + "check if incoming ip address '" + p_o_requestSourceAddress.Address + "' is allowed");

                /* check if we have a ipv4 address, because ipv6 is not implemented yet */
                if (p_o_requestSourceAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    bool b_allowed = false;

                    /* iterate each source address which is allowed */
                    foreach (string s_allowAddress in this.Seed.Config.AllowSourceList)
                    {
                        if (ForestNET.Lib.Helper.IsIpv4Address(s_allowAddress))
                        { /* only one ipv4 address */
                            if (p_o_requestSourceAddress.Address.ToString().Equals(s_allowAddress))
                            {
                                /* our incoming address matches an allowed address */
                                b_allowed = true;
                                break;
                            }
                        }
                        else if (ForestNET.Lib.Helper.IsIpv4AddressWithSuffix(s_allowAddress))
                        { /* ipv4 address with suffix */
                            if (ForestNET.Lib.Helper.IsIpv4WithinRange(p_o_requestSourceAddress.Address.ToString(), s_allowAddress))
                            {
                                /* our incoming address is within allowed range */
                                b_allowed = true;
                                break;
                            }
                        }
                    }

                    /* if incoming address is not allowed */
                    if (!b_allowed)
                    {
                        ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "403 Forbidden: HTTP request source address is not in allow list");
                        return 403;
                    }
                }
                else
                {
                    ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "403 Forbidden: HTTP request source address is not IPv4, compability with IPv4 allow list is not implemented");
                    return 403;
                }
            }

            /* check if accept-encoding value contains configured out encoding of tiny https server */
            if (!ForestNET.Lib.Helper.IsString(this.Seed.RequestHeader.AcceptCharset))
            {
                if (!Seed.RequestHeader.AcceptCharset.Contains((this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).WebName, StringComparison.CurrentCultureIgnoreCase))
                {
                    ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "400 Bad Request: HTTP request accept-encoding does not contain server response encoding '" + (this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).WebName + " != " + this.Seed.RequestHeader.AcceptCharset + "'");
                    return 400;
                }
            }

            /* check root directory */
            if (!ForestNET.Lib.Helper.IsStringEmpty(this.Seed.Config.RootDirectory))
            {
                /* check if root directory really exists */
                if (!ForestNET.Lib.IO.File.FolderExists(this.Seed.Config.RootDirectory ?? ""))
                {
                    ForestNET.Lib.Global.ILogSevere(this.Seed.Salt + " " + "500 Internal Server Error: root directory '" + this.Seed.Config.RootDirectory + "' does not exist");
                    return 500;
                }
            }
            else
            {
                /* root directory is required */
                ForestNET.Lib.Global.ILogSevere(this.Seed.Salt + " " + "500 Internal Server Error: no root directory configured for tiny https server");
                return 500;
            }

            /* check session directory */
            if (!ForestNET.Lib.Helper.IsStringEmpty(this.Seed.Config.SessionDirectory))
            {
                /* check if session directory really exists */
                if (!ForestNET.Lib.IO.File.FolderExists(this.Seed.Config.SessionDirectory ?? ""))
                {
                    ForestNET.Lib.Global.ILogSevere(this.Seed.Salt + " " + "500 Internal Server Error: session directory '" + this.Seed.Config.SessionDirectory + "' does not exist");
                    return 500;
                }

                /* check if session directory is not a sub directory of root directory, if root directory is configured */
                if ((!ForestNET.Lib.Helper.IsStringEmpty(this.Seed.Config.RootDirectory)) && (ForestNET.Lib.IO.File.IsSubDirectory(this.Seed.Config.SessionDirectory ?? "/a", this.Seed.Config.RootDirectory ?? "/b")))
                {
                    ForestNET.Lib.Global.ILogSevere(this.Seed.Salt + " " + "500 Internal Server Error: session directory '" + this.Seed.Config.SessionDirectory + "' is a sub directory of root '" + this.Seed.Config.RootDirectory + "'. This is not allowed because of security vulnerability");
                    return 500;
                }
            }
            else if (!this.Seed.Config.NotUsingCookies)
            {
                /* session directory is required if cookies are used */
                ForestNET.Lib.Global.ILogSevere(this.Seed.Salt + " " + "500 Internal Server Error: no session directory configured for tiny https server");
                return 500;
            }

            /* tiny https server support dynamic communication with GET and POST */
            if ((this.Seed.Config.Mode == ForestNET.Lib.Net.Https.Mode.DYNAMIC) || (this.Seed.Config.Mode == ForestNET.Lib.Net.Https.Mode.REST))
            {
                /* check if we accept session cookies */
                if (!this.Seed.Config.NotUsingCookies)
                {
                    ForestNET.Lib.Global.ILogFine(this.Seed.Salt + " " + "handle session cookies");

                    string s_cookieUUID = string.Empty;

                    /* check if our request has a session cookie */
                    if (!ForestNET.Lib.Helper.IsStringEmpty(this.Seed.RequestHeader.Cookie))
                    {
                        string s_cookie = this.Seed.RequestHeader.Cookie;

                        /* check if our cookie key for session UUID is available, otherwise there will be an new session cookie and a new session file */
                        if (s_cookie.Contains("forestAny-UUID"))
                        {
                            /* split cookie line with ';' to get our cookie key */
                            string[] a_cookieLine = s_cookie.Split(";");

                            /* iterate each cookie key value pair */
                            foreach (string s_foo in a_cookieLine)
                            {
                                /* store our cookie key value pair */
                                if (s_foo.Trim().StartsWith("forestAny-UUID"))
                                {
                                    s_cookie = s_foo.Trim();
                                    break;
                                }
                            }

                            /* check if it is really our cookie key value pair */
                            if (!s_cookie.Contains('='))
                            {
                                ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "400 Bad Request: cannot detect cookie key value pair with '='-sign in '" + this.Seed.RequestHeader.Cookie + "'");
                                return 400;
                            }

                            /* split cookie key and value by '=' */
                            string[] a_foo = s_cookie.Split("=");

                            /* cookie key name must be 'forestAny-UUID' */
                            if (!a_foo[0].Equals("forestAny-UUID"))
                            {
                                ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "400 Bad Request: expected cookie key 'forestAny-UUID' with UUID value, but received '" + s_cookie + "'");
                                return 400;
                            }

                            /* store cookie uuid value within variable */
                            s_cookieUUID = a_foo[1];
                        }
                    }

                    /* start setting cookie properties for response, cookie instance, http only and strict */
                    this.Seed.ResponseHeader.Cookie = new(s_cookieUUID, this.Seed.Config.SessionMaxAge, this.Seed.Config.Domain.Replace("https://", ""))
                    {
                        HttpOnly = true,
                        SameSite = ForestNET.Lib.Net.Https.Dynm.CookieSameSite.STRICT
                    };

                    /* control age of session files */
                    try
                    {
                        /* iterate each session file in session directory */
                        foreach (ForestNET.Lib.IO.ListingElement o_file in ForestNET.Lib.IO.File.ListDirectory(this.Seed.Config.SessionDirectory ?? "/a"))
                        {
                            /* get file name */
                            string s_sessionFileListing = o_file.FullName ?? throw new NullReferenceException("file name in listing is null");
                            /* open session file */
                            ForestNET.Lib.IO.File o_sessionFile = new(s_sessionFileListing, false);
                            /* read session age from line 1 */
                            DateTime o_cookieAge = ForestNET.Lib.Helper.FromISO8601UTC(o_sessionFile.ReadLine(1));

                            /* check if age of session file is to old -> delete file */
                            if ((this.Seed.Config.SessionMaxAge != null) && (o_cookieAge < DateTime.Now.AddSeconds(-1 * this.Seed.Config.SessionMaxAge.ToDurationInSeconds())))
                            {
                                ForestNET.Lib.Global.ILogFiner(this.Seed.Salt + " " + "delete session file '" + s_sessionFileListing + "', [" + o_cookieAge + "] < [" + DateTime.Now.AddSeconds(-1 * this.Seed.Config.SessionMaxAge.ToDurationInSeconds()) + "]");

                                ForestNET.Lib.IO.File.DeleteFile(s_sessionFileListing);
                            }
                        }
                    }
                    catch (Exception o_exc)
                    {
                        ForestNET.Lib.Global.ILogSevere(this.Seed.Salt + " " + "500 Internal Server Error: could not list session directory, control age of session file or delete session file, exception message: " + o_exc);
                        return 500;
                    }

                    /* create session file name by session directory and cookie uuid */
                    string s_sessionFile = this.Seed.Config.SessionDirectory + this.Seed.ResponseHeader.Cookie.CookieUUID + ".txt";

                    /* try to read session file */
                    try
                    {
                        /* check if session file exists, if not it was delete because of cookie age */
                        if (ForestNET.Lib.IO.File.Exists(s_sessionFile))
                        {
                            /* read session file */
                            ForestNET.Lib.IO.File o_sessionFile = new(s_sessionFile, false);

                            /* iterate each line, starting in line 2 */
                            for (int i = 2; i <= o_sessionFile.FileLines; i++)
                            {
                                /* read line */
                                string s_line = o_sessionFile.ReadLine(i);

                                if (s_line.Contains('='))
                                {
                                    /* split session line in key and value by '=' */
                                    string[] a_lineValues = s_line.Split('=');
                                    /* add session key and value to our internal session data map */
                                    this.Seed.SessionData.Add(a_lineValues[0].Replace("&equal;", "="), a_lineValues[1].Replace("&equal;", "="));
                                }
                            }
                        }
                    }
                    catch (Exception o_exc)
                    {
                        ForestNET.Lib.Global.ILogSevere(this.Seed.Salt + " " + "500 Internal Server Error: could not read/write session file '" + s_sessionFile + "', exception message: " + o_exc);
                        return 500;
                    }
                }
            }

            /* handle HTTP methods with the predicted modes */
            if ((this.Seed.RequestHeader.Method.Equals("GET")) && ((this.Seed.Config.Mode == ForestNET.Lib.Net.Https.Mode.NORMAL) || (this.Seed.Config.Mode == ForestNET.Lib.Net.Https.Mode.DYNAMIC)))
            {
                ForestNET.Lib.Global.ILog(this.Seed.Salt + " " + "GET '" + this.Seed.RequestHeader.RequestPath + "' from " + p_o_requestSourceAddress);

                /* prepare returning requested file */
                return this.HandleFileRequest();
            }
            else if ((this.Seed.RequestHeader.Method.Equals("POST")) && (this.Seed.Config.Mode == ForestNET.Lib.Net.Https.Mode.DYNAMIC))
            {
                ForestNET.Lib.Global.ILog(this.Seed.Salt + " " + "POST '" + this.Seed.RequestHeader.RequestPath + "' from " + p_o_requestSourceAddress);

                /* for POST request, we need body data after header lines */
                if ((this.Seed.RequestBody == null) || (this.Seed.RequestBody.Length < 1))
                {
                    ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "204 No Content: HTTPS request is null or empty");
                    return 204;
                }

                /* create dynamic instance */
                this.Dynamic = new ForestNET.Lib.Net.Https.Dynm.Dynamic(this.Seed);
                /* handle body data from request */
                i_foo = this.Dynamic.HandlePostRequest();

                if (i_foo != 200)
                { /* if something happened while handling body data, abort here */
                    return i_foo;
                }
                else
                { /* prepare returning requested file */
                    return this.HandleFileRequest();
                }
            }
            else if (((this.Seed.RequestHeader.Method.Equals("GET")) || (this.Seed.RequestHeader.Method.Equals("POST"))) && (this.Seed.Config.Mode == ForestNET.Lib.Net.Https.Mode.SOAP))
            {
                ForestNET.Lib.Global.ILog(this.Seed.Salt + " " + this.Seed.RequestHeader.Method + " '" + this.Seed.RequestHeader.RequestPath + "' from " + p_o_requestSourceAddress);

                /* check if we have received anything at all for SOAP POST request */
                if ((this.Seed.RequestHeader.Method.Equals("POST")) && ((this.Seed.RequestBody == null) || (this.Seed.RequestBody.Length < 1)))
                {
                    ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "204 No Content: SOAP request is null or empty");
                    return 204;
                }

                /* handling SOAP request */
                return this.HandleSOAPRequest();
            }
            else if (
                    (
                        (this.Seed.RequestHeader.Method.Equals("GET")) ||
                        (this.Seed.RequestHeader.Method.Equals("POST")) ||
                        (this.Seed.RequestHeader.Method.Equals("PUT")) ||
                        (this.Seed.RequestHeader.Method.Equals("DELETE"))
                    )
                &&
                    (this.Seed.Config.Mode == ForestNET.Lib.Net.Https.Mode.REST)
            )
            {
                ForestNET.Lib.Global.ILog(this.Seed.Salt + " " + this.Seed.RequestHeader.Method + " '" + this.Seed.RequestHeader.RequestPath + "' from " + p_o_requestSourceAddress);

                /* for GET request and REST mode, we already parsed request header and can continue directly with preparing the response */
                if (!this.Seed.RequestHeader.Method.Equals("GET"))
                {
                    bool b_skipHandlePost = false;

                    /* we have no body data in our request */
                    if ((this.Seed.RequestBody == null) || (this.Seed.RequestBody.Length < 1))
                    {
                        /* DELETE method does not force body data, but could work with it if the implementation wants to */
                        if (this.Seed.RequestHeader.Method.Equals("DELETE"))
                        {
                            b_skipHandlePost = true;
                        }
                        else
                        { /* POST and PUT need body data to proceed */
                            ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "204 No Content: REST request is null or empty");
                            return 204;
                        }
                    }

                    if (!b_skipHandlePost)
                    {
                        /* create dynamic instance */
                        this.Dynamic = new ForestNET.Lib.Net.Https.Dynm.Dynamic(this.Seed);
                        /* handle body data from request */
                        i_foo = this.Dynamic.HandlePostRequest();

                        /* if something happened while handling body data, abort here */
                        if (i_foo != 200)
                        {
                            return i_foo;
                        }
                    }
                }

                return 200;
            }
            else
            {
                ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "501 Not Implemented: HTTP method " + this.Seed.RequestHeader.Method + " with Mode " + this.Seed.Config.Mode + " not implemented");
                return 501;
            }
        }

        /// <summary>
        /// prepare returning requested file
        /// </summary>
        private int HandleFileRequest()
        {
            /* check if seed instance is set */
            if (this.Seed == null)
            {
                throw new NullReferenceException("Seed instance is null");
            }

            string? s_fileAbsolutePath;
            string? s_extension = null;

            if (ForestNET.Lib.Helper.IsStringEmpty(this.Seed.RequestHeader.File))
            { /* we have no file value in request header */
                if ((ForestNET.Lib.Helper.IsStringEmpty(this.Seed.RequestHeader.Path)) && (ForestNET.Lib.IO.File.Exists(this.Seed.Config.RootDirectory + "index.html")))
                {
                    /* our root directory contains a 'index.html' file */
                    s_fileAbsolutePath = this.Seed.Config.RootDirectory + "index.html";
                    s_extension = ".html";
                }
                else if ((ForestNET.Lib.Helper.IsStringEmpty(this.Seed.RequestHeader.Path)) && (ForestNET.Lib.IO.File.Exists(this.Seed.Config.RootDirectory + "index.htm")))
                {
                    /* our root directory contains a 'index.htm' file */
                    s_fileAbsolutePath = this.Seed.Config.RootDirectory + "index.htm";
                    s_extension = ".htm";
                }
                else if (ForestNET.Lib.IO.File.Exists(this.Seed.Config.RootDirectory + this.Seed.RequestHeader.Path + ForestNET.Lib.IO.File.DIR + "index.html"))
                {
                    /* our requested path contains a 'index.html' file */
                    s_fileAbsolutePath = this.Seed.Config.RootDirectory + this.Seed.RequestHeader.Path + ForestNET.Lib.IO.File.DIR + "index.html";
                    s_extension = ".html";
                }
                else if (ForestNET.Lib.IO.File.Exists(this.Seed.Config.RootDirectory + this.Seed.RequestHeader.Path + ForestNET.Lib.IO.File.DIR + "index.htm"))
                {
                    /* our requested path contains a 'index.htm' file */
                    s_fileAbsolutePath = this.Seed.Config.RootDirectory + this.Seed.RequestHeader.Path + ForestNET.Lib.IO.File.DIR + "index.htm";
                    s_extension = ".htm";
                }
                else
                {
                    ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "404 Not Found: Resource 'index.html' or 'index.htm' not found");
                    return 404;
                }

                /* update file value in request header */
                this.Seed.RequestHeader.File = "index" + s_extension;
            }
            else
            {
                bool b_hasAllowedExtension = false;

                /* check for valid extension of requested file */
                foreach (KeyValuePair<string, string> o_allowExtension in this.Seed.Config.AllowExtensionList)
                {
                    if (this.Seed.RequestHeader.File.EndsWith(o_allowExtension.Key))
                    {
                        /* file extension found in allow list */
                        b_hasAllowedExtension = true;
                        s_extension = o_allowExtension.Key;
                        break;
                    }
                }

                /* return 403 if file extension is forbidden */
                if (!b_hasAllowedExtension)
                {
                    ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "403 Forbidden: Resource '" + this.Seed.RequestHeader.Path + ForestNET.Lib.IO.File.DIR + this.Seed.RequestHeader.File + "'");
                    return 403;
                }

                if (ForestNET.Lib.IO.File.Exists(this.Seed.Config.RootDirectory + this.Seed.RequestHeader.Path + ForestNET.Lib.IO.File.DIR + this.Seed.RequestHeader.File))
                {
                    /* safe absolute path to requested file */
                    s_fileAbsolutePath = this.Seed.Config.RootDirectory + this.Seed.RequestHeader.Path + ForestNET.Lib.IO.File.DIR + this.Seed.RequestHeader.File;
                }
                else
                {
                    /* requested file does not exist */
                    ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "404 Not Found: Resource '" + this.Seed.RequestHeader.Path + ForestNET.Lib.IO.File.DIR + this.Seed.RequestHeader.File + "'");
                    return 404;
                }
            }

            /* get content type by file extension */
            string s_contentType = ForestNET.Lib.Net.Https.Config.KNOWN_EXTENSION_LIST[s_extension ?? string.Empty];

            /* check if requested file does not exceed max. payload */
            if (ForestNET.Lib.IO.File.FileLength(s_fileAbsolutePath) > this.Seed.Config.MaxPayload * 1024 * 1024)
            {
                ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "413 Payload Too Large: File length for answer is to long, max. payload is '" + ForestNET.Lib.Helper.FormatBytes(this.Seed.Config.MaxPayload * 1024 * 1024) + "'");
                return 413;
            }

            try
            {
                /* if we respond with a text|json|xml file we are using our outgoing encoding setting, read all bytes from file and add encoding value to the content type as well */
                if (s_contentType.StartsWith("text") || s_contentType.Contains("json") || s_contentType.Contains("xml"))
                {
                    this.Seed.ResponseHeader.ContentType = s_contentType + "; charset=" + (this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).WebName;
                    this.Seed.ResponseBody = ForestNET.Lib.IO.File.ReadAllBytes(s_fileAbsolutePath, this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8, this.Seed.Config.InEncoding ?? System.Text.Encoding.UTF8);
                    this.Seed.ResponseHeader.ContentLength = this.Seed.ResponseBody?.Length ?? 0;
                    this.Seed.ResponseHeader.LastModified = ForestNET.Lib.IO.File.LastModified(s_fileAbsolutePath);
                }
                else
                { /* just set content type ad read all bytes from file */
                    this.Seed.ResponseHeader.ContentType = s_contentType;
                    this.Seed.ResponseBody = ForestNET.Lib.IO.File.ReadAllBytes(s_fileAbsolutePath);
                    this.Seed.ResponseHeader.ContentLength = this.Seed.ResponseBody?.Length ?? 0;
                    this.Seed.ResponseHeader.LastModified = ForestNET.Lib.IO.File.LastModified(s_fileAbsolutePath);
                }
            }
            catch (System.IO.IOException o_exc)
            {
                ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "404 Not Found: Resource '" + s_fileAbsolutePath + "'; " + o_exc);
                return 404;
            }

            return 200;
        }

        /// <summary>
        /// handling incoming SOAP request
        /// </summary>
        private int HandleSOAPRequest()
        {
            /* check if seed instance is set */
            if (this.Seed == null)
            {
                throw new NullReferenceException("Seed instance is null");
            }

            /* check if wsdl configuration is available */
            if (this.Seed.Config.WSDL == null)
            {
                ForestNET.Lib.Global.ILogSevere(this.Seed.Salt + " " + "500 Internal Server Error: No WSDL configuration enabled for SOAP protocol");
                return 500;
            }

            if (this.Seed.RequestHeader.Method.Equals("GET"))
            { /* handling incoming SOAP GET request */
                /* iterate each service port configuration from wsdl */
                foreach (ForestNET.Lib.Net.Https.SOAP.WSDL.ServicePort o_servicePort in this.Seed.Config.WSDL.ServiceInstance.ServicePorts)
                {
                    /* get address location from service port, removing 'https://' prefix and host part */
                    string s_addressLocation = o_servicePort.AddressLocation.Replace("https://", "").Replace(this.Seed.RequestHeader.Host, "");

                    /* we only accept GET requests to wsdl or xsd schema file */
                    if ((this.Seed.RequestHeader.RequestPath.Equals(s_addressLocation + "?wsdl")) || (this.Seed.RequestHeader.RequestPath.Equals(s_addressLocation + "?WSDL")))
                    {
                        /* check if we want to get the wsdl configuration by '?wsdl' or '?WSDL' at the end of request path */
                        this.Seed.RequestHeader.Path = this.Seed.RequestHeader.File;
                        this.Seed.RequestHeader.File = this.Seed.Config.WSDL.WSDLFile;

                        return this.HandleFileRequest();
                    }
                    else if (this.Seed.RequestHeader.RequestPath.Equals(s_addressLocation + "/" + this.Seed.Config.WSDL.WSDLFile))
                    {
                        /* if wsdl file is directly requested */
                        this.Seed.RequestHeader.Path = s_addressLocation;
                        this.Seed.RequestHeader.File = this.Seed.Config.WSDL.WSDLFile;

                        return this.HandleFileRequest();
                    }
                    else if ((this.Seed.Config.WSDL.XSDFile != null) && (!ForestNET.Lib.Helper.IsStringEmpty(this.Seed.Config.WSDL.XSDFile)) && (this.Seed.RequestHeader.RequestPath.Equals(s_addressLocation + "/" + this.Seed.Config.WSDL.XSDFile)))
                    {
                        /* if xsd schema file is directly requested */
                        this.Seed.RequestHeader.Path = s_addressLocation;
                        this.Seed.RequestHeader.File = this.Seed.Config.WSDL.XSDFile;

                        return this.HandleFileRequest();
                    }
                }

                /* forbidden requests happened */
                ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "403 Forbidden: Request to '" + this.Seed.RequestHeader.RequestPath + "' is forbidden for SOAP web server (only access to .wsdl and .xsd allowed)");
                return 403;
            }
            else if (this.Seed.RequestHeader.Method.Equals("POST"))
            { /* handling incoming SOAP POST request */
                System.Text.Encoding o_inEncoding;

                /* check for incoming encoding value in request header */
                if (this.Seed.RequestHeader.ContentType.Contains("charset="))
                {
                    ForestNET.Lib.Global.ILogFine(this.Seed.Salt + " " + "read encoding encoding out of request header");

                    /* read encoding encoding out of request header */
                    string s_encoding = this.Seed.RequestHeader.ContentType.Substring(this.Seed.RequestHeader.ContentType.IndexOf("charset=") + 8);
                    s_encoding = s_encoding.ToLower();

                    /* delete double quotes */
                    if ((s_encoding.StartsWith("\"")) && (s_encoding.EndsWith("\"")))
                    {
                        s_encoding = s_encoding.Substring(1, s_encoding.Length - 1 - 1);
                    }

                    ForestNET.Lib.Global.ILogFine(this.Seed.Salt + " " + "try to create an encoding object with read encoding value: " + s_encoding);

                    /* try to create an encoding object with read encoding value */
                    try
                    {
                        o_inEncoding = System.Text.Encoding.GetEncoding(s_encoding);
                    }
                    catch (Exception o_exc)
                    {
                        ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "400 Bad Request: Invalid encoding '" + s_encoding + "' within request in HTTP header content type '" + this.Seed.RequestHeader.ContentType + "'; " + o_exc);
                        return 400;
                    }
                }
                else
                { /* use configured incoming encoding property */
                    o_inEncoding = this.Seed.Config.InEncoding ?? System.Text.Encoding.UTF8;
                }

                /* convert received SOAP request byte array to string with encoding object */
                string s_soapRequest = o_inEncoding.GetString(this.Seed.RequestBody ?? []);

                /* clean up xml file */
                s_soapRequest = new System.Text.RegularExpressions.Regex("[\\r\\n\\t]").Replace(s_soapRequest, "");
                s_soapRequest = new System.Text.RegularExpressions.Regex(">\\s*<").Replace(s_soapRequest, "><");
                s_soapRequest = new System.Text.RegularExpressions.Regex("<\\?(.*?)\\?>").Replace(s_soapRequest, "");
                s_soapRequest = new System.Text.RegularExpressions.Regex("<!--(.*?)-->").Replace(s_soapRequest, "");
                s_soapRequest = new System.Text.RegularExpressions.Regex("<soap:").Replace(s_soapRequest, "<");
                s_soapRequest = new System.Text.RegularExpressions.Regex("</soap:").Replace(s_soapRequest, "</");

                /* validate xml */
                System.Text.RegularExpressions.Regex o_regex = new("(<[^<>]*?<[^<>]*?>|<[^<>]*?>[^<>]*?>)");
                System.Text.RegularExpressions.MatchCollection o_matcher = o_regex.Matches(s_soapRequest);

                /* if regex-matcher has match, the xml-file is not valid */
                while (o_matcher.Count > 0)
                {
                    throw new ArgumentException("Invalid xml-file. Please check xml-file at \"" + o_matcher[0] + "\".");
                }

                List<string> a_xmlTags = [];

                /* add all xml-tags to a list for parsing */
                o_regex = new System.Text.RegularExpressions.Regex("(<[^<>/]*?/>)|(<[^<>/]*?>[^<>]*?</[^<>/]*?>)|(<[^<>]*?>)|(</[^<>/]*?>)");
                o_matcher = o_regex.Matches(s_soapRequest);

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
                    ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "400 Bad Request: SOAP request must have a 'Body'-tag");
                    return 400;
                }

                /* handle Body data from SOAP request */
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
                    ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "400 Bad Request: SOAP request does not closed the 'Body'-tag");
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
                    string s_namespaceInitial = a_soapBodyXmlTags[0].Substring(1, a_soapBodyXmlTags[0].IndexOf(":") - 1);
                    string s_namespace = a_soapBodyXmlTags[0].Substring(a_soapBodyXmlTags[0].IndexOf(s_namespaceInitial + "=\"") + s_namespaceInitial.Length + 2);
                    s_namespace = s_namespace.Substring(0, s_namespace.IndexOf("\""));

                    this.SOAPTargetNamespace = s_namespace;
                    this.SOAPNamespaceInitial = s_namespaceInitial;

                    /* check if target namespace from request matches our target namespace in our wsdl xsd schema */
                    if (((this.Seed.Config.WSDL.Schema.TargetNamespace == null) ^ (this.SOAPTargetNamespace == null)) || (!(this.Seed.Config.WSDL.Schema.TargetNamespace ?? string.Empty).Equals(this.SOAPTargetNamespace)))
                    {
                        ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "400 Bad Request: received SOAP target namespace '" + this.SOAPTargetNamespace + "' does not match with wsdl xsd schema target namespace '" + this.Seed.Config.WSDL.Schema.TargetNamespace + "'");
                        return 400;
                    }

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
                        a_soapBodyXmlTags[i] = a_soapBodyXmlTags[i].Replace("<" + s_namespaceInitial + ":", "<").Replace("</" + s_namespaceInitial + ":", "</");
                    }
                }
                else if (a_soapBodyXmlTags[0].Contains(" xmlns="))
                {
                    string s_namespace = a_soapBodyXmlTags[0].Substring(a_soapBodyXmlTags[0].IndexOf(" xmlns=\"") + 8);
                    s_namespace = s_namespace.Substring(0, s_namespace.IndexOf("\""));

                    this.SOAPTargetNamespace = s_namespace;

                    /* check if target namespace from request matches our target namespace in our wsdl xsd schema */
                    if (((this.Seed.Config.WSDL.Schema.TargetNamespace == null) ^ (this.SOAPTargetNamespace == null)) || (!(this.Seed.Config.WSDL.Schema.TargetNamespace ?? string.Empty).Equals(this.SOAPTargetNamespace)))
                    {
                        ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "400 Bad Request: received SOAP target namespace '" + this.SOAPTargetNamespace + "' does not match with wsdl xsd schema target namespace '" + this.Seed.Config.WSDL.Schema.TargetNamespace + "'");
                        return 400;
                    }

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

                string? s_inputMessageName;
                string? s_operationName;
                string? s_elementName;

                /* get input message name */
                if (a_soapBodyXmlTags[0].Contains(' '))
                {
                    s_inputMessageName = a_soapBodyXmlTags[0].Substring(1, a_soapBodyXmlTags[0].IndexOf(" ") - 1);
                }
                else
                {
                    s_inputMessageName = a_soapBodyXmlTags[0].Substring(1, a_soapBodyXmlTags[0].Length - 1 - 1);
                }

                /* check if input message name exists as a valid operation in our wsdl configuration */
                if (!this.Seed.Config.WSDL.ContainsOperationByInputMessagePartElementValue(s_inputMessageName))
                {
                    /* SOAP operation not found, so we generate a SOAP fault object for response */
                    this.SOAPResponse = (Object)new ForestNET.Lib.Net.Https.SOAP.SoapFault("soap:client", "Soap operation with input message name '" + s_inputMessageName + "' not found. Please verify your requests with wsdl file.", null, null);
                    return 200;
                }
                else
                { /* store operation name */
                    s_operationName = this.Seed.Config.WSDL.GetOperationByInputMessagePartElementValue(s_inputMessageName)?.PortTypeOperationName;
                }

                /* check if operation name exists within SOAP operation list of our wsdl configuration */
                if (!this.Seed.Config.WSDL.SOAPOperations.ContainsKey(s_operationName ?? string.Empty))
                {
                    this.SOAPResponse = (Object)new ForestNET.Lib.Net.Https.SOAP.SoapFault("soap:server", "No SOAP operation declared to process input message.", null, null);
                    return 200;
                }

                /* get input message element attribute value */
                s_elementName = this.Seed.Config.WSDL.GetOperationByInputMessagePartElementValue(s_inputMessageName)?.InputMessage.PartElement;

                /* replace input message value tags with input message element attribute value, because these are used for xml decoding based on xsd schema types in wsdl configuration */
                for (int i = 0; i < a_soapBodyXmlTags.Count; i++)
                {
                    if (a_soapBodyXmlTags[i].StartsWith("<" + s_inputMessageName))
                    {
                        a_soapBodyXmlTags[i] = a_soapBodyXmlTags[i].Replace("<" + s_inputMessageName, "<" + s_elementName);
                    }
                    else if (a_soapBodyXmlTags[i].StartsWith("</" + s_inputMessageName))
                    {
                        a_soapBodyXmlTags[i] = a_soapBodyXmlTags[i].Replace("</" + s_inputMessageName, "</" + s_elementName);
                    }
                }

                ForestNET.Lib.Global.ILogFine(this.Seed.Salt + " " + "replace input message value tags '" + s_inputMessageName + "' with input message element attribute value '" + s_elementName + "'");

                Object? o_soapRequest;

                ForestNET.Lib.Global.ILogFine(this.Seed.Salt + " " + "decode received and cleaned up SOAP xml data");

                /* decode received and cleaned up SOAP xml data to an object */
                try
                {
                    /* remember no fields from abstract are supported */
                    o_soapRequest = this.Seed.Config.WSDL.Schema.XmlDecode(a_soapBodyXmlTags);
                }
                catch (Exception o_exc)
                {
                    this.SOAPResponse = (Object)new ForestNET.Lib.Net.Https.SOAP.SoapFault("soap:server", "Exception while decoding received SOAP xml data.", o_exc.Message, null);
                    return 200;
                }

                /* get SOAP operation interface by operation name */
                ForestNET.Lib.Net.Https.SOAP.WSDL.SOAPOperationShell? o_soi = this.Seed.Config.WSDL.SOAPOperations[s_operationName ?? string.Empty];

                /* secure check if SOAP operation interface is not null */
                if (o_soi == null)
                {
                    this.SOAPResponse = (Object)new ForestNET.Lib.Net.Https.SOAP.SoapFault("soap:server", "SOAP operation '" + s_operationName + "' is not implemented by server.", null, null);
                    return 200;
                }

                /* execute implemented SOAP operation, giving our decoded received SOAP xml data and our Seed instance as parameter - expecting a SOAP response object for client request */
                try
                {
                    this.SOAPResponse = (Object?)o_soi.Invoke(o_soapRequest, this.Seed);
                }
                catch (Exception o_exc)
                {
                    this.SOAPResponse = (Object?)new ForestNET.Lib.Net.Https.SOAP.SoapFault("soap:server", "Exception occured while processing SOAP operation.", o_exc.Message, null);
                    return 200;
                }

                string? s_soapResponse;

                /* encode expected SOAP response object to an xml string */
                try
                {
                    s_soapResponse = this.Seed.Config.WSDL.Schema.XmlEncode(this.SOAPResponse);
                }
                catch (Exception o_exc)
                {
                    this.SOAPResponse = (Object)new ForestNET.Lib.Net.Https.SOAP.SoapFault("soap:server", "Exception while encoding return value xml data of SOAP operation.", o_exc.Message, null);
                    return 200;
                }

                /* remove xml header from SOAP response xml string */
                s_soapResponse = new System.Text.RegularExpressions.Regex("<\\?(.*?)\\?>\r\n").Replace(s_soapResponse, "");
                s_soapResponse = new System.Text.RegularExpressions.Regex("<\\?(.*?)\\?>\n").Replace(s_soapResponse, "");

                /* get output message value */
                string s_outputMessage = s_soapResponse.Substring(0, s_soapResponse.IndexOf(">") + 1);

                /* filter out any possible attributes */
                if (s_outputMessage.Contains(' '))
                {
                    s_outputMessage = s_outputMessage.Substring(1, s_outputMessage.IndexOf(" ") - 1);
                }
                else
                {
                    s_outputMessage = s_outputMessage.Substring(1, s_outputMessage.Length - 1 - 1);
                }

                /* get our portType operation object and thus the output message */
                if (!(this.Seed.Config.WSDL.GetOperationByInputMessagePartElementValue(s_inputMessageName)?.OutputMessage.PartElement ?? string.Empty).Equals(s_outputMessage))
                {
                    /* output message could not be found */
                    this.SOAPResponse = (Object)new ForestNET.Lib.Net.Https.SOAP.SoapFault("soap:server", "Invalid soap output message '" + s_outputMessage + "' within soap response body. Please verify server response with wsdl file.", null, null);
                    return 200;
                }
                else
                {
                    /* replace output message value tags with output message part element value, because client expecting this response based on xsd schema types in wsdl configuration */
                    string s_outputMessageElementValue = this.Seed.Config.WSDL.GetOperationByInputMessagePartElementValue(s_inputMessageName)?.OutputMessage.PartElement ?? string.Empty;
                    s_soapResponse = new System.Text.RegularExpressions.Regex("<" + s_outputMessage).Replace(s_soapResponse, "<" + s_outputMessageElementValue);
                    s_soapResponse = new System.Text.RegularExpressions.Regex("</" + s_outputMessage).Replace(s_soapResponse, "</" + s_outputMessageElementValue);

                    ForestNET.Lib.Global.ILogFine(this.Seed.Salt + " " + "replaced output message value tags '" + s_outputMessage + "' with output message part element value '" + s_outputMessageElementValue + "'");
                }

                /* store SOAP encoded xml response string as object for later use */
                this.SOAPResponse = (Object?)s_soapResponse;
            }
            else
            {
                ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "405 Method Not Allowed: HTTP method '" + this.Seed.RequestHeader.Method + "' not allowed for SOAP web server (only GET or POST)");
                return 405;
            }

            return 200;
        }

        /// <summary>
        /// send https/soap/rest response to received request
        /// </summary>
        private async System.Threading.Tasks.Task SendResponse()
        {
            /* check if seed instance is set */
            if (this.Seed == null)
            {
                throw new NullReferenceException("Seed instance is null");
            }

            if (this.Seed.ReturnCode != 200)
            { /* return code while handling request is NOT 'OK' */
                if (this.Seed.ReturnCode != 301)
                { /* prepare fail response */
                    this.Seed.ResponseBody = (this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).GetBytes("HTTP/1.1 " + this.Seed.ReturnCode + " " + ForestNET.Lib.Net.Https.ResponseHeader.RETURN_CODES[this.Seed.ReturnCode]);
                    this.Seed.ResponseHeader.ContentLength = this.Seed.ResponseBody.Length;
                    this.Seed.ResponseHeader.ContentType = ForestNET.Lib.Net.Https.Config.KNOWN_EXTENSION_LIST[".txt"] + "; charset=" + (this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).WebName;
                }
                else
                { /* prepare redirect response */
                    /* chrome wants two linebreaks at the end */
                    this.Seed.ResponseBody =
                        (this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).GetBytes(
                            "Location: " + this.Seed.Config.Domain + this.Seed.RequestHeader.RequestPath + "/" +
                            ForestNET.Lib.IO.File.NEWLINE + ForestNET.Lib.IO.File.NEWLINE
                        );
                }
            }
            else
            { /* return code while handling request is 'OK' */
                if ((this.Seed.Config.Mode == ForestNET.Lib.Net.Https.Mode.DYNAMIC) || (this.Seed.Config.Mode == ForestNET.Lib.Net.Https.Mode.REST))
                { /* respond with dynamic or REST content */
                    bool b_skipCookies = false;

                    /* do steps for rendering dynamic content if response content type starts with 'text/html' */
                    if ((this.Seed.Config.Mode == ForestNET.Lib.Net.Https.Mode.DYNAMIC) && (this.Seed.ResponseHeader.ContentType.StartsWith(ForestNET.Lib.Net.Https.Config.KNOWN_EXTENSION_LIST[".html"])))
                    {
                        /* create dynamic instance if it is null */
                        if (this.Dynamic == null)
                        {
                            this.Dynamic = new ForestNET.Lib.Net.Https.Dynm.Dynamic(this.Seed);
                        }

                        /* render dynamic content */
                        string s_dynamicReturn = this.Dynamic.RenderDynamic();

                        /* return value of rendering dynamic content is not 'OK' */
                        if (!s_dynamicReturn.Equals("OK"))
                        {
                            /* prepare fail response */
                            ForestNET.Lib.Global.ILogSevere(this.Seed.Salt + " " + "500 Internal Server Error - exception within forestAny code; " + s_dynamicReturn);
                            this.Seed.ReturnCode = 500;
                            this.Seed.ResponseBody = (this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).GetBytes(500 + " " + ForestNET.Lib.Net.Https.ResponseHeader.RETURN_CODES[500] + " - " + s_dynamicReturn);
                            this.Seed.ResponseHeader.ContentLength = this.Seed.ResponseBody.Length;
                            this.Seed.ResponseHeader.ContentType = ForestNET.Lib.Net.Https.Config.KNOWN_EXTENSION_LIST[".txt"] + "; charset=" + (this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).WebName;

                            /* skip cookies routine for response */
                            b_skipCookies = true;
                        }
                    }
                    else if (this.Seed.Config.Mode == ForestNET.Lib.Net.Https.Mode.REST)
                    { /* do steps for rendering REST content */
                        /* create dynamic instance if it is null */
                        if (this.Dynamic == null)
                        {
                            this.Dynamic = new ForestNET.Lib.Net.Https.Dynm.Dynamic(this.Seed);
                        }

                        /* render REST content */
                        string s_dynamicReturn = this.Dynamic.RenderREST();

                        /* return value of rendering REST content is not 'OK' */
                        if (!s_dynamicReturn.Equals("OK"))
                        {
                            if (s_dynamicReturn.StartsWith("400;"))
                            { /* bad request returned from REST implementation */
                                ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "400 Bad Request: " + s_dynamicReturn.Substring(4));
                                this.Seed.ReturnCode = 400;
                                this.Seed.ResponseBody = (this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).GetBytes(400 + " " + ForestNET.Lib.Net.Https.ResponseHeader.RETURN_CODES[400] + " - " + s_dynamicReturn.Substring(4));
                                this.Seed.ResponseHeader.ContentLength = this.Seed.ResponseBody.Length;
                                this.Seed.ResponseHeader.ContentType = ForestNET.Lib.Net.Https.Config.KNOWN_EXTENSION_LIST[".txt"] + "; charset=" + (this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).WebName;
                            }
                            else
                            { /* prepare fail response */
                                ForestNET.Lib.Global.ILogSevere(this.Seed.Salt + " " + "500 Internal Server Error - exception within handling REST implementation; " + s_dynamicReturn);
                                this.Seed.ReturnCode = 500;
                                this.Seed.ResponseBody = (this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).GetBytes(500 + " " + ForestNET.Lib.Net.Https.ResponseHeader.RETURN_CODES[500] + " - " + s_dynamicReturn);
                                this.Seed.ResponseHeader.ContentLength = this.Seed.ResponseBody.Length;
                                this.Seed.ResponseHeader.ContentType = ForestNET.Lib.Net.Https.Config.KNOWN_EXTENSION_LIST[".txt"] + "; charset=" + (this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).WebName;
                            }

                            /* skip cookies routine for response */
                            b_skipCookies = true;
                        }
                    }

                    /* execute cookies routine for response */
                    if ((!b_skipCookies) && (this.Seed.ResponseHeader.Cookie != null))
                    {
                        /* check if we accept session cookies by configuration value */
                        if (!this.Seed.Config.NotUsingCookies)
                        {
                            /* create session file name by session directory and cookie uuid in response header */
                            string s_sessionFile = this.Seed.Config.SessionDirectory + this.Seed.ResponseHeader.Cookie.CookieUUID + ".txt";

                            /* check if session file exists: true -> delete it */
                            if (ForestNET.Lib.IO.File.Exists(s_sessionFile))
                            {
                                ForestNET.Lib.IO.File.DeleteFile(s_sessionFile);
                            }

                            /* configuration value for session refresh is set */
                            if (this.Seed.Config.SessionRefresh)
                            {
                                /* create new session cookie uuid */
                                this.Seed.ResponseHeader.Cookie.CookieUUID = ForestNET.Lib.Helper.GenerateUUID();
                                /* update session file name because of session refresh */
                                s_sessionFile = this.Seed.Config.SessionDirectory + this.Seed.ResponseHeader.Cookie.CookieUUID + ".txt";
                            }

                            /* create session file */
                            ForestNET.Lib.IO.File o_sessionFile = new(s_sessionFile, true);
                            /* append first line with ISO-8601-UTC date-time string */
                            o_sessionFile.AppendLine(ForestNET.Lib.Helper.ToISO8601UTC(DateTime.Now));

                            /* check if current request has any session data */
                            if (this.Seed.SessionData.Count > 0)
                            {
                                /* iterate each session key and value pair */
                                foreach (KeyValuePair<string, string?> o_sessionDataEntry in this.Seed.SessionData)
                                {
                                    try
                                    {
                                        /* append session key and value pair to session file, divided by '='; replacing '=' within key and value with '&equal;' */
                                        o_sessionFile.AppendLine(o_sessionDataEntry.Key.Replace("=", "&equal;") + "=" + o_sessionDataEntry.Value?.Replace("=", "&equal;"));
                                    }
                                    catch (Exception)
                                    {
                                        /* prepare fail response, because session key and value pair could not be append to session file */
                                        ForestNET.Lib.Global.ILogSevere(this.Seed.Salt + " " + "500 Internal Server Error: could not append session key '" + o_sessionDataEntry.Key + "' with value to file '" + o_sessionFile.FileName + "'");
                                        this.Seed.ReturnCode = 500;
                                        this.Seed.ResponseBody = (this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).GetBytes(500 + " " + ForestNET.Lib.Net.Https.ResponseHeader.RETURN_CODES[500]);
                                        this.Seed.ResponseHeader.ContentLength = this.Seed.ResponseBody.Length;
                                        this.Seed.ResponseHeader.ContentType = ForestNET.Lib.Net.Https.Config.KNOWN_EXTENSION_LIST[".txt"] + "; charset=" + (this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).WebName;
                                    }
                                }
                            }
                        }
                    }
                }
                else if ((this.Seed.Config.Mode == ForestNET.Lib.Net.Https.Mode.SOAP) && (this.Seed.RequestHeader.Method.Equals("POST")))
                { /* respond with SOAP content if http method is POST */
                    if (this.SOAPResponse != null)
                    { /* SOAP response object is available */
                        string s_soapResponse;

                        if (this.SOAPResponse is ForestNET.Lib.Net.Https.SOAP.SoapFault o_soapFault)
                        {
                            /* SOAP response object is instance of SOAP-Fault and will be prepared as xml string */
                            s_soapResponse = o_soapFault.ToXML((this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).BodyName.ToUpper());
                        }
                        else
                        {
                            /* SOAP response object, xml string, must be bagged within SOAP Envelope */
                            s_soapResponse = "<?xml version=\"1.0\" encoding=\"" + (this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).BodyName.ToUpper() + "\" ?>" + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK;

                            s_soapResponse += "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">" + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK;
                            s_soapResponse += "\t<soap:Body>" + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK;

                            /* add SOAP response object xml string to response 'Body'-Tag */
                            string s_soapResponseXml = this.SOAPResponse.ToString() ?? "SOAP Response is null";

                            if ((!ForestNET.Lib.Helper.IsStringEmpty(this.SOAPTargetNamespace)) && (ForestNET.Lib.Helper.IsStringEmpty(this.SOAPNamespaceInitial)))
                            {
                                /* request has target namespace, but no namespace initial */

                                /* adding target namespace to xml data */
                                s_soapResponseXml = s_soapResponseXml.Substring(0, s_soapResponseXml.IndexOf(">")) + " xmlns=\"" + this.SOAPTargetNamespace + "\"" + s_soapResponseXml.Substring(s_soapResponseXml.IndexOf(">"));
                            }
                            else if ((!ForestNET.Lib.Helper.IsStringEmpty(this.SOAPTargetNamespace)) && (!ForestNET.Lib.Helper.IsStringEmpty(this.SOAPNamespaceInitial)))
                            {
                                /* request has target namespace and namespace initial */
                                s_soapResponseXml = s_soapResponseXml.Substring(0, s_soapResponseXml.IndexOf(">")) + " xmlns:" + this.SOAPNamespaceInitial + "=\"" + this.SOAPTargetNamespace + "\"" + s_soapResponseXml.Substring(s_soapResponseXml.IndexOf(">"));

                                /* adding target namespace and namespace initial to each xml-tag to xml data */
                                s_soapResponseXml = new System.Text.RegularExpressions.Regex("</").Replace(s_soapResponseXml, "</" + this.SOAPNamespaceInitial + ":");
                                s_soapResponseXml = new System.Text.RegularExpressions.Regex("<").Replace(s_soapResponseXml, "<" + this.SOAPNamespaceInitial + ":");
                                s_soapResponseXml = new System.Text.RegularExpressions.Regex("<" + this.SOAPNamespaceInitial + ":" + "/" + this.SOAPNamespaceInitial + ":").Replace(s_soapResponseXml, "</" + this.SOAPNamespaceInitial + ":");
                            }

                            /* clean up target namespace and namespace initial for next request */
                            this.SOAPTargetNamespace = null;
                            this.SOAPNamespaceInitial = null;

                            s_soapResponse += s_soapResponseXml;

                            s_soapResponse += "\t</soap:Body>" + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK;
                            s_soapResponse += "</soap:Envelope>";
                        }

                        /* convert SOAP response string to byte array with outgoing encoding settings */
                        this.Seed.ResponseBody = (this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).GetBytes(s_soapResponse);
                        /* update content length and content type within response header */
                        this.Seed.ResponseHeader.ContentLength = this.Seed.ResponseBody.Length;
                        this.Seed.ResponseHeader.ContentType = ForestNET.Lib.Net.Https.Config.KNOWN_EXTENSION_LIST[".xsd"] + "; charset=" + (this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).WebName;
                    }
                    else
                    {
                        /* prepare fail response, because session key and value pair could not be append to session file */
                        ForestNET.Lib.Global.ILogSevere(this.Seed.Salt + " " + "500 Internal Server Error: SOAP response object is null");
                        this.Seed.ReturnCode = 500;
                        this.Seed.ResponseBody = (this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).GetBytes(500 + " " + ForestNET.Lib.Net.Https.ResponseHeader.RETURN_CODES[500]);
                        this.Seed.ResponseHeader.ContentLength = this.Seed.ResponseBody.Length;
                        this.Seed.ResponseHeader.ContentType = ForestNET.Lib.Net.Https.Config.KNOWN_EXTENSION_LIST[".txt"] + "; charset=" + (this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).WebName;
                    }
                }

                /* with current de.forestj.lib.net.sock.recv.ReceiveTCP structure we cannot keep alive */
                /*if (!this.Seed.RequestHeader.ConnectionClose)
				{
					this.Seed.ResponseHeader.ConnectionKeepAlive = true;
				}*/
            }

            /* re-check if response does not exceed max. payload value */
            if ((this.Seed.ResponseBody != null) && (this.Seed.ResponseBody.Length > this.Seed.Config.MaxPayload * 1024 * 1024))
            {
                ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "413 Payload Too Large: File length for answer is to long, max. payload is '" + ForestNET.Lib.Helper.FormatBytes(this.Seed.Config.MaxPayload * 1024 * 1024) + "'");
                this.Seed.ReturnCode = 413;
                this.Seed.ResponseBody = (this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).GetBytes(413 + " " + ForestNET.Lib.Net.Https.ResponseHeader.RETURN_CODES[413]);
                this.Seed.ResponseHeader.ContentLength = this.Seed.ResponseBody.Length;
                this.Seed.ResponseHeader.ContentType = ForestNET.Lib.Net.Https.Config.KNOWN_EXTENSION_LIST[".txt"] + "; charset=" + (this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).WebName;
            }

            ForestNET.Lib.Global.ILogFine(this.Seed.Salt + " " + "add response header to https/REST/SOAP response");

            /* create string temporary variable for response header */
            string s_responseHeader = (this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).GetString(System.Text.Encoding.UTF8.GetBytes(this.Seed.ResponseHeader.ToString()));
            byte[] a_responseHeader = (this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).GetBytes(s_responseHeader);

            /* create byte array for response, including header and body length */
            byte[] a_foo = new byte[a_responseHeader.Length + ((this.Seed.ResponseBody != null) ? this.Seed.ResponseBody.Length : 0)];
            int i;

            /* copy each byte from response header to byte array */
            for (i = 0; i < a_responseHeader.Length; i++)
            {
                a_foo[i] = a_responseHeader[i];
            }

            if (this.Seed.ResponseBody != null)
            {
                ForestNET.Lib.Global.ILogFine(this.Seed.Salt + " " + "add body data to https/REST/SOAP response");

                /* copy each byte from response body to byte array */
                for (int j = 0; j < this.Seed.ResponseBody.Length; j++)
                {
                    a_foo[j + i] = this.Seed.ResponseBody[j];
                }
            }

            /* sending https/SOAP/REST response to client */
            await this.SendBytes(a_foo, a_foo.Length);
        }
    }
}