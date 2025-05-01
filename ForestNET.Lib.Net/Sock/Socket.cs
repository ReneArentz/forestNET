namespace ForestNET.Lib.Net.Sock
{
    /// <summary>
    /// Abstract socket class for creating all kind of sockets within forestNET-lib framework.
    /// General class properties for all kind of sockets available.
    /// </summary>
    public abstract class Socket
    {

        /* Fields */

        /* Properties */

        public bool Stop { get; set; } = false;
        protected Type Type { get; set; }
        protected int TimeoutMilliseconds { get; set; }
        protected CancellationTokenSource? CancellationTokenSource { get; set; }
        protected int BufferSize { get; set; }
        protected int MaxTerminations { get; set; }
        protected int Terminations { get; set; }
        protected System.Net.Sockets.Socket? SocketInstance { get; set; }
        public System.Net.IPEndPoint? SocketAddress { get; protected set; }
        public System.Security.Cryptography.X509Certificates.X509Certificate? ServerCertificate { protected get; set; }
        public List<System.Security.Cryptography.X509Certificates.X509Certificate>? ClientCertificateAllowList { protected get; set; } = [];
        protected ForestNET.Lib.Net.Sock.Task.Task? SocketTask { get; set; } = null;

        /* Methods */

        /// <summary>
        /// Core execution process method of a socket. Either receiving or sending network data as server or client.
        /// </summary>
        abstract public System.Threading.Tasks.Task Run();

        /// <summary>
        /// This method stops the socket and ends any network communication.
        /// </summary>
        abstract public void StopSocket();

        /// <summary>
        /// Add X.509 certificate to socket instance allow list, so only connections with these certificates will be allowed
        /// </summary>
        /// <param name="p_o_certificate">X.509 certificate</param>
        public void AddClientCertificateAllowList(System.Security.Cryptography.X509Certificates.X509Certificate p_o_certificate)
        {
            if (this.ClientCertificateAllowList == null)
            {
                this.ClientCertificateAllowList = [];
            }

            this.ClientCertificateAllowList.Add(p_o_certificate);
        }

        /// <summary>
        /// Clears certificate allow list of socket instance
        /// </summary>
        public void ClearClientCertificateAllowList()
        {
            this.ClientCertificateAllowList?.Clear();
        }

        /// <summary>
        /// Method to validate receiving certificate on server side
        /// </summary>
        /// <param name="p_o_sender">sender object, probaly having host string name</param>
        /// <param name="p_o_certificate">received certificate</param>
        /// <param name="p_o_chain">certificate chain</param>
        /// <param name="p_e_sslPolicyErrors">enumeration of possible ssl policy errors</param>
        /// <returns>true - certificate validated, false - certificate is not accepted</returns>
        /// <exception cref="NullReferenceException">received certificate is null</exception>
        protected bool MyTlsCertificateValidationCallback(object p_o_sender, System.Security.Cryptography.X509Certificates.X509Certificate? p_o_certificate, System.Security.Cryptography.X509Certificates.X509Chain? p_o_chain, System.Net.Security.SslPolicyErrors p_e_sslPolicyErrors)
        {
            /* no policy error, so we can return true */
            if (p_e_sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
                return true;

            /* read sender parameter as host string */
            string s_host = p_o_sender.ToString() ?? "sender object has no string value";

            /* check if policy error hit because of remote certificate not available */
            if ((p_e_sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateNotAvailable) != 0)
            {
                /* means that the remote certificate is unavailable */
                ForestNET.Lib.Global.ILogSevere("SSL certificate was not available for " + s_host);
                return false;
            }

            /* check if we cannot find remote certificate on our own system */
            if ((p_e_sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateNameMismatch) != 0)
            {
                /* get received certificate */
                System.Security.Cryptography.X509Certificates.X509Certificate2 o_receivedCertificate = (System.Security.Cryptography.X509Certificates.X509Certificate2)(p_o_certificate ?? throw new NullReferenceException("received certificate is null"));

                /* iterate each X.509 certificate in our allow list  */
                foreach (System.Security.Cryptography.X509Certificates.X509Certificate o_certificate in this.ClientCertificateAllowList ?? [])
                {
                    if (o_certificate.GetRawCertDataString().Equals(o_receivedCertificate.GetRawCertDataString()))
                    {
                        ForestNET.Lib.Global.ILogFine("certificate allow list match, accepting connection");
                        return true;
                    }
                }

                /* get certificate name */
                string s_certificateName = o_receivedCertificate.GetNameInfo(System.Security.Cryptography.X509Certificates.X509NameType.SimpleName, false);

                /* overwrite host variable if we have ssl stream */
                if (s_host.ToLower().Equals("system.net.security.sslstream"))
                {
                    System.Net.Security.SslStream o_sslStream = (System.Net.Security.SslStream)p_o_sender;
                    s_host = o_sslStream.TargetHostName;
                }

                /* certificate validated, because we trust if only RemoteCertificateNameMismatch occurs and our target host is 'ANY_CERTIFICATE_ACCEPT' */
                if (s_host.Equals("ANY_CERTIFICATE_ACCEPT"))
                {
                    ForestNET.Lib.Global.ILogWarning("certificate validated, because we trust if only RemoteCertificateNameMismatch occurs; certificate name[" + s_certificateName + "]");
                    return true;
                }

                ForestNET.Lib.Global.ILogSevere("common name for the SSL certificate did not match '" + s_host + "'. Instead, it was certificate name[" + s_certificateName + "] | subject[" + o_receivedCertificate.Subject + "] | friendly name[" + o_receivedCertificate.FriendlyName + "]");
                return false;
            }

            /* errors left in chain */
            ForestNET.Lib.Global.ILogSevere("SSL certificate for the server could not be validated for the following reasons:");

            /* render chain */
            if (p_o_chain != null)
            {
                /* iterate each chain element */
                foreach (var element in p_o_chain.ChainElements)
                {
                    /* first element's certificate will be the server's SSL certificate */
                    if (element.ChainElementStatus.Length == 0)
                        continue;

                    ForestNET.Lib.Global.ILogSevere("\u2022 " + element.Certificate.Subject);

                    /* log chain elements */
                    foreach (var error in element.ChainElementStatus)
                    {
                        /* log error status info, because it is 'human-readable' */
                        ForestNET.Lib.Global.ILogSevere("\t\u2022 " + error.StatusInformation);
                    }
                }
            }

            return false;
        }
    }
}
