using MailKit;
using MailKit.Security;
using MailKit.Net.Smtp;
using MailKit.Net.Pop3;
using MailKit.Net.Imap;
using MailKit.Net.Proxy;
using MimeKit;

namespace ForestNET.Lib.Net.Mail
{
    /// <summary>
    /// Supported proxy types for mail connection.
    /// </summary>
    public enum ProxyType
    {
        HTTP, SOCKS4A, SOCKS5
    }

    /// <summary>
    /// Supported security options for mail connection.
    /// </summary>
    public enum SecurityOptions
    {
        None, StartTls, Tls
    }

    /// <summary>
    /// Mail Client class for a connection to a mail server.
    /// supported protocols: POP3, SMTP and IMAP.
    /// </summary>
    public class Client<T> : IDisposable where T : MailService
    {

        /* Delegates */

        /// <summary>
        /// interface delegate definition for implementing OAuth authentication in combination with OAuth2 flag
        /// </summary>
        public delegate Task OAuthAsync(MailService p_o_clientInstance, string p_s_user, string p_s_cid, string p_s_csecret, CancellationToken p_o_token);

        /* Fields */

        /* Properties */

        private bool LoggedIn { get; set; } = false;
        private CancellationTokenSource? TokenSource { get; set; } = null;
        private SmtpClient? SmtpSession { get; set; } = null;
        private Pop3Client? Pop3Session { get; set; } = null;
        private ImapClient? ImapSession { get; set; } = null;
        public string? MailServer { get; set; } = null;
        public int MailPort { get; set; } = 0;
        public string? MailUser { get; set; } = null;
        public string? MailPassword { private get; set; } = null;
        public SecurityOptions MailSecurityOption { private get; set; } = SecurityOptions.None;
        public string? SmtpServer { get; set; } = null;
        public int SmtpPort { get; set; } = 0;
        public string? SmtpUser { get; set; } = null;
        public string? SmtpPassword { private get; set; } = null;
        public SecurityOptions SmtpSecurityOption { private get; set; } = SecurityOptions.None;
        private bool Pop3 { get; set; } = false;
        public bool OAuth2 { get; set; } = false;
        public OAuthAsync? OAuthMethod { get; set; } = null;
        public string? CID { private get; set; } = null;
        public string? CSecret { private get; set; } = null;
        private Folder? CurrentFolder { get; set; } = null;
        private ProxyType? ProxyTypeGet { get; set; } = null;
        private ProxyType? ProxyTypeSend { get; set; } = null;
        private ProxyClient? ProxyClientGet { get; set; } = null;
        private ProxyClient? ProxyClientSend { get; set; } = null;
        private List<System.Security.Cryptography.X509Certificates.X509Certificate2> CertificateAllowList { get; set; } = [];

        private string GetSessionProtocol
        {
            get
            {
                return this.Pop3 ? "POP3" : "IMAP";
            }
        }
        private ImapClient GetSessionAsImapClient
        {
            get
            {
                return this.ImapSession ?? throw new NullReferenceException("Imap client instance is null");
            }
        }
        private Pop3Client GetSessionAsPop3Client
        {
            get
            {
                return this.Pop3Session ?? throw new NullReferenceException("Pop3 client instance is null");
            }
        }

        /* Methods */

        /// <summary>
        /// Constructor for a smtp client object, only for sending mails, no encrypted and secure data transfer
        /// </summary>
        /// <param name="p_s_smtpServer">host address for smtp server</param>
        /// <param name="p_i_smtpServerPort">port for smtp server</param>
        /// <param name="p_s_user">server user</param>
        /// <param name="p_s_password">server user password</param>
        /// <exception cref="ArgumentException">invalid or empty(null) parameter value</exception>
        public Client(string p_s_smtpServer, int p_i_smtpServerPort, string p_s_user, string p_s_password) :
            this(p_s_smtpServer, p_i_smtpServerPort, p_s_user, p_s_password, SecurityOptions.None)
        {

        }

        /// <summary>
        /// Constructor for a smtp client object, only for sending mails
        /// </summary>
        /// <param name="p_s_smtpServer">host address for smtp server</param>
        /// <param name="p_i_smtpServerPort">port for smtp server</param>
        /// <param name="p_s_user">server user</param>
        /// <param name="p_s_password">server user password</param>
        /// <param name="p_e_securityOption">security option for smtp server</param>
        /// <exception cref="ArgumentException">invalid or empty(null) parameter value</exception>
        public Client(string p_s_smtpServer, int p_i_smtpServerPort, string p_s_user, string p_s_password, SecurityOptions p_e_securityOption) :
            this(null, p_s_smtpServer, 1, p_i_smtpServerPort, null, null, p_s_user, p_s_password, SecurityOptions.None, p_e_securityOption)
        {

        }

        /// <summary>
        /// Constructor for a mail client object, using parameter for both mail and smtp server(could be identical), no encrypted and secure data transfer
        /// </summary>
        /// <param name="p_s_mailServer">host address for server</param>
        /// <param name="p_i_mailServerPort">port for mail server</param>
        /// <param name="p_i_smtpServerPort">port for smtp server</param>
        /// <param name="p_s_user">server user</param>
        /// <param name="p_s_password">server user password</param>
        /// <exception cref="ArgumentException">invalid or empty(null) parameter value</exception>
        public Client(string p_s_mailServer, int p_i_mailServerPort, int p_i_smtpServerPort, string p_s_user, string p_s_password) :
            this(p_s_mailServer, p_s_mailServer, p_i_mailServerPort, p_i_smtpServerPort, p_s_user, p_s_password, p_s_user, p_s_password, SecurityOptions.None)
        {

        }

        /// <summary>
        /// Constructor for a mail client object, using parameter for both mail and smtp server(could be identical)
        /// </summary>
        /// <param name="p_s_mailServer">host address for server</param>
        /// <param name="p_i_mailServerPort">port for mail server</param>
        /// <param name="p_i_smtpServerPort">port for smtp server</param>
        /// <param name="p_s_user">server user</param>
        /// <param name="p_s_password">server user password</param>
        /// <param name="p_s_trustStorePath">file path to truststore file</param>
        /// <param name="p_e_securityOption">security option for mail + smtp server</param>
        /// <exception cref="ArgumentException">invalid or empty(null) parameter value</exception>
        public Client(string p_s_mailServer, int p_i_mailServerPort, int p_i_smtpServerPort, string p_s_user, string p_s_password, SecurityOptions p_e_securityOption) :
            this(p_s_mailServer, p_s_mailServer, p_i_mailServerPort, p_i_smtpServerPort, p_s_user, p_s_password, p_s_user, p_s_password, p_e_securityOption, p_e_securityOption)
        {

        }

        /// <summary>
        /// Constructor for a mail client object, using separate parameter for mail server and smtp server
        /// </summary>
        /// <param name="p_s_mailServer">host address for mail server</param>
        /// <param name="p_s_smtpServer">host address for smtp server</param>
        /// <param name="p_i_mailServerPort">port for mail server</param>
        /// <param name="p_i_smtpServerPort">port for smtp server</param>
        /// <param name="p_s_mailServerUser">mail server user</param>
        /// <param name="p_s_mailServerPassword">mail server user password</param>
        /// <param name="p_s_smtpServerUser">smtp server user</param>
        /// <param name="p_s_smtpServerPassword">smtp server user password</param>
        /// <param name="p_e_securityOption">security option for mail + smtp server</param>
        /// <exception cref="ArgumentException">invalid or empty(null) parameter value</exception>
        public Client(
            string? p_s_mailServer,
            string? p_s_smtpServer,
            int p_i_mailServerPort,
            int p_i_smtpServerPort,
            string? p_s_mailServerUser,
            string? p_s_mailServerPassword,
            string? p_s_smtpServerUser,
            string? p_s_smtpServerPassword,
            SecurityOptions p_e_securityOption
        ) :
            this(p_s_mailServer, p_s_smtpServer, p_i_mailServerPort, p_i_smtpServerPort, p_s_mailServerUser, p_s_mailServerPassword, p_s_smtpServerUser, p_s_smtpServerPassword, p_e_securityOption, p_e_securityOption)
        {

        }

        /// <summary>
        /// Constructor for a mail client object, using separate parameter for mail server and smtp server - this constructor can be used for OAuth2 authentication
        /// </summary>
        /// <param name="p_s_mailServer">host address for mail server</param>
        /// <param name="p_s_smtpServer">host address for smtp server</param>
        /// <param name="p_i_mailServerPort">port for mail server</param>
        /// <param name="p_i_smtpServerPort">port for smtp server</param>
        /// <param name="p_s_serverUser">server user</param>
        /// <param name="p_e_securityOption">security option for mail + smtp server</param>
        /// <exception cref="ArgumentException">invalid or empty(null) parameter value</exception>
        public Client(
            string? p_s_mailServer,
            string p_s_smtpServer,
            int p_i_mailServerPort,
            int p_i_smtpServerPort,
            string p_s_serverUser,
            SecurityOptions p_e_securityOption
        ) :
            this(p_s_mailServer, p_s_smtpServer, p_i_mailServerPort, p_i_smtpServerPort, p_s_serverUser, null, p_s_serverUser, null, p_e_securityOption, p_e_securityOption)
        {

        }

        /// <summary>
        /// Constructor for a mail client object, using separate parameter for mail server and smtp server
        /// </summary>
        /// <param name="p_s_mailServer">host address for mail server</param>
        /// <param name="p_s_smtpServer">host address for smtp server</param>
        /// <param name="p_i_mailServerPort">port for mail server</param>
        /// <param name="p_i_smtpServerPort">port for smtp server</param>
        /// <param name="p_s_mailServerUser">mail server user</param>
        /// <param name="p_s_mailServerPassword">mail server user password</param>
        /// <param name="p_s_smtpServerUser">smtp server user</param>
        /// <param name="p_s_smtpServerPassword">smtp server user password</param>
        /// <param name="p_e_mailSecurityOption">security option for mail server</param>
        /// <param name="p_e_smtpSecurityOption">security option for smtp server</param>
        /// <exception cref="ArgumentException">invalid or empty(null) parameter value</exception>
        public Client(
            string? p_s_mailServer,
            string? p_s_smtpServer,
            int p_i_mailServerPort,
            int p_i_smtpServerPort,
            string? p_s_mailServerUser,
            string? p_s_mailServerPassword,
            string? p_s_smtpServerUser,
            string? p_s_smtpServerPassword,
            SecurityOptions p_e_mailSecurityOption,
            SecurityOptions p_e_smtpSecurityOption
        )
        {
            ForestNET.Lib.Global.ILogConfig("generic type of client class '" + this.GetType().GetGenericArguments()[0] + "'");

            /* get generic type of client class */
            if (this.GetType().GetGenericArguments()[0] == typeof(Pop3Client))
            {
                this.Pop3 = true;
            }

            /* start with current root folder Folder.INBOX */
            this.CurrentFolder = new Folder(Folder.INBOX);

            /* check mail server parameter if there is no smtp server parameter */
            if ((ForestNET.Lib.Helper.IsStringEmpty(p_s_mailServer)) && (ForestNET.Lib.Helper.IsStringEmpty(p_s_smtpServer)))
            {
                throw new ArgumentException("Empty value for \"Mail-Server\"-parameter and \"SMTP-Server\"");
            }

            /* check user parameter for mail and smtp server */
            if ((ForestNET.Lib.Helper.IsStringEmpty(p_s_mailServerUser)) && (ForestNET.Lib.Helper.IsStringEmpty(p_s_smtpServerUser)))
            {
                throw new ArgumentException("Empty value for \"User\"-parameter and \"SMTP-User\"-parameter");
            }

            /* check password parameter for mail and smtp server */
            /*if ((ForestNET.Lib.Helper.IsStringEmpty(p_s_mailServerPassword)) && (ForestNET.Lib.Helper.IsStringEmpty(p_s_smtpServerPassword)))
            {
                throw new ArgumentException("Empty value for \"Password\"-parameter and \"SMTP-Password\"-parameter");
            }*/

            /* check min value for mail server parameter */
            if (p_i_mailServerPort < 1)
            {
                throw new ArgumentException("Mail-Server Port must be at least '1', but was set to '" + p_i_mailServerPort + "'");
            }

            /* check max value for mail server parameter */
            if (p_i_mailServerPort > 65535)
            {
                throw new ArgumentException("Mail-Server Port must be lower equal '65535', but was set to '" + p_i_mailServerPort + "'");
            }

            /* check min value for smtp server parameter */
            if (p_i_smtpServerPort < 1)
            {
                throw new ArgumentException("SMTP-Server Port must be at least '1', but was set to '" + p_i_smtpServerPort + "'");
            }

            /* check max value for smtp server parameter */
            if (p_i_smtpServerPort > 65535)
            {
                throw new ArgumentException("SMTP-Server Port must be lower equal '65535', but was set to '" + p_i_smtpServerPort + "'");
            }

            this.MailServer = p_s_mailServer;
            this.MailPort = p_i_mailServerPort;
            this.MailUser = p_s_mailServerUser;
            this.MailPassword = p_s_mailServerPassword;
            this.MailSecurityOption = p_e_mailSecurityOption;

            this.SmtpServer = p_s_smtpServer;
            this.SmtpPort = p_i_smtpServerPort;
            this.SmtpUser = p_s_smtpServerUser;
            this.SmtpPassword = p_s_smtpServerPassword;
            this.SmtpSecurityOption = p_e_smtpSecurityOption;
        }

        /// <summary>
        /// Get cancellation token of CancellationTokenSource instance of our current mail client instance
        /// </summary>
        private CancellationToken Token()
        {
            return (this.TokenSource != null) ? this.TokenSource.Token : default;
        }

        /// <summary>
        /// Get client instance of current session, imap or pop3 - depending on generic type of this class
        /// </summary>
        /// <returns>imap or pop3 client instance</returns>
        /// <exception cref="NullReferenceException">client instance is null</exception>
        private T GetSession()
        {
            return (T)Convert.ChangeType(
                this.Pop3 ? this.Pop3Session ?? throw new NullReferenceException("Pop3 client instance is null") : this.ImapSession ?? throw new NullReferenceException("Imap client instance is null"),
                typeof(T)
            );
        }

        /// <summary>
        /// Set proxy profile settings for mail client instance for pop3/imap and smtp
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
        /// Set proxy profile settings for mail client instance for pop3/imap and smtp
        /// </summary>
        /// <param name="p_e_proxyType">supported proxy types: HTTP, SOCKS4A, SOCKS5</param>
        /// <param name="p_s_proxyHost">proxy host value</param>
        /// <param name="p_i_proxyPort">proxy port value</param>
        /// <param name="p_s_proxyUser">proxy user value</param>
        /// <param name="p_s_proxyPassword">proxy user password value</param>
        /// <exception cref="ArgumentException">no proxy host value, invalid proxy port number, missing user or password</exception>
        public void SetProxy(ProxyType p_e_proxyType, string p_s_proxyHost, int p_i_proxyPort, string p_s_proxyUser, string p_s_proxyPassword)
        {
            this.SetProxy(p_e_proxyType, p_s_proxyHost, p_i_proxyPort, false, p_s_proxyUser, p_s_proxyPassword);
        }

        /// <summary>
        /// Set proxy profile settings for mail client instance for pop3/imap and smtp
        /// </summary>
        /// <param name="p_e_proxyType">supported proxy types: HTTP, SOCKS4A, SOCKS5</param>
        /// <param name="p_s_proxyHost">proxy host value</param>
        /// <param name="p_i_proxyPort">proxy port value</param>
        /// <param name="p_b_useDefaultNetworkCredentials">true - using System.Net.CredentialCache.DefaultNetworkCredentials for authentication, false - using user and password value for authentication</param>
        /// <param name="p_s_proxyUser">proxy user value</param>
        /// <param name="p_s_proxyPassword">proxy user password value</param>
        /// <exception cref="ArgumentException">no proxy host value, invalid proxy port number, missing user or password</exception>
        private void SetProxy(ProxyType? p_e_proxyType, string p_s_proxyHost, int p_i_proxyPort, bool p_b_useDefaultNetworkCredentials, string? p_s_proxyUser, string? p_s_proxyPassword)
        {
            /* if proxy type parameter is null, we set proxy profile to null */
            if (p_e_proxyType == null)
            {
                this.ProxyTypeGet = null;
                this.ProxyClientGet = null;
                this.ProxyTypeSend = null;
                this.ProxyClientSend = null;
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

                /* save proxy type */
                this.ProxyTypeGet = p_e_proxyType;
                this.ProxyTypeSend = p_e_proxyType;

                System.Net.NetworkCredential o_networkCredential;

                /* using System.Net.CredentialCache.DefaultNetworkCredentials for authentication */
                if (p_b_useDefaultNetworkCredentials)
                {
                    o_networkCredential = System.Net.CredentialCache.DefaultNetworkCredentials;
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

                    o_networkCredential = new(p_s_proxyUser, p_s_proxyPassword);
                }

                if (p_e_proxyType == ProxyType.SOCKS4A)
                {
                    this.ProxyClientGet = new Socks4aClient(p_s_proxyHost, p_i_proxyPort, o_networkCredential);
                    this.ProxyClientSend = new Socks4aClient(p_s_proxyHost, p_i_proxyPort, o_networkCredential);
                }
                else if (p_e_proxyType == ProxyType.SOCKS5)
                {
                    this.ProxyClientGet = new Socks5Client(p_s_proxyHost, p_i_proxyPort, o_networkCredential);
                    this.ProxyClientSend = new Socks5Client(p_s_proxyHost, p_i_proxyPort, o_networkCredential);
                }
                else if (p_e_proxyType == ProxyType.HTTP)
                {
                    this.ProxyClientGet = new HttpProxyClient(p_s_proxyHost, p_i_proxyPort, o_networkCredential);
                    this.ProxyClientSend = new Socks5Client(p_s_proxyHost, p_i_proxyPort, o_networkCredential);
                }
            }
        }

        /// <summary>
        /// Set proxy profile settings for mail client instance for pop3/imap
        /// </summary>
        /// <param name="p_e_proxyType">supported proxy types: HTTP, SOCKS4A, SOCKS5</param>
        /// <param name="p_s_proxyHost">proxy host value</param>
        /// <param name="p_i_proxyPort">proxy port value</param>
        /// <param name="p_b_useDefaultNetworkCredentials">true - using System.Net.CredentialCache.DefaultNetworkCredentials for authentication, false - using user and password value for authentication</param>
        /// <exception cref="ArgumentException">no proxy host value, invalid proxy port number, missing user or password</exception>
        public void SetProxyGet(ProxyType p_e_proxyType, string p_s_proxyHost, int p_i_proxyPort, bool p_b_useDefaultNetworkCredentials)
        {
            this.SetProxyGet(p_e_proxyType, p_s_proxyHost, p_i_proxyPort, p_b_useDefaultNetworkCredentials, null, null);
        }

        /// <summary>
        /// Set proxy profile settings for mail client instance for pop3/imap
        /// </summary>
        /// <param name="p_e_proxyType">supported proxy types: HTTP, SOCKS4A, SOCKS5</param>
        /// <param name="p_s_proxyHost">proxy host value</param>
        /// <param name="p_i_proxyPort">proxy port value</param>
        /// <param name="p_s_proxyUser">proxy user value</param>
        /// <param name="p_s_proxyPassword">proxy user password value</param>
        /// <exception cref="ArgumentException">no proxy host value, invalid proxy port number, missing user or password</exception>
        public void SetProxyGet(ProxyType p_e_proxyType, string p_s_proxyHost, int p_i_proxyPort, string p_s_proxyUser, string p_s_proxyPassword)
        {
            this.SetProxyGet(p_e_proxyType, p_s_proxyHost, p_i_proxyPort, false, p_s_proxyUser, p_s_proxyPassword);
        }

        /// <summary>
        /// Set proxy profile settings for mail client instance for pop3/imap
        /// </summary>
        /// <param name="p_e_proxyType">supported proxy types: HTTP, SOCKS4A, SOCKS5</param>
        /// <param name="p_s_proxyHost">proxy host value</param>
        /// <param name="p_i_proxyPort">proxy port value</param>
        /// <param name="p_b_useDefaultNetworkCredentials">true - using System.Net.CredentialCache.DefaultNetworkCredentials for authentication, false - using user and password value for authentication</param>
        /// <param name="p_s_proxyUser">proxy user value</param>
        /// <param name="p_s_proxyPassword">proxy user password value</param>
        /// <exception cref="ArgumentException">no proxy host value, invalid proxy port number, missing user or password</exception>
        private void SetProxyGet(ProxyType? p_e_proxyType, string p_s_proxyHost, int p_i_proxyPort, bool p_b_useDefaultNetworkCredentials, string? p_s_proxyUser, string? p_s_proxyPassword)
        {
            /* if proxy type parameter is null, we set proxy profile to null */
            if (p_e_proxyType == null)
            {
                this.ProxyTypeGet = null;
                this.ProxyClientGet = null;
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

                /* save proxy type */
                this.ProxyTypeGet = p_e_proxyType;

                System.Net.NetworkCredential o_networkCredential;

                /* using System.Net.CredentialCache.DefaultNetworkCredentials for authentication */
                if (p_b_useDefaultNetworkCredentials)
                {
                    o_networkCredential = System.Net.CredentialCache.DefaultNetworkCredentials;
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

                    o_networkCredential = new(p_s_proxyUser, p_s_proxyPassword);
                }

                if (p_e_proxyType == ProxyType.SOCKS4A)
                {
                    this.ProxyClientGet = new Socks4aClient(p_s_proxyHost, p_i_proxyPort, o_networkCredential);
                }
                else if (p_e_proxyType == ProxyType.SOCKS5)
                {
                    this.ProxyClientGet = new Socks5Client(p_s_proxyHost, p_i_proxyPort, o_networkCredential);
                }
                else if (p_e_proxyType == ProxyType.HTTP)
                {
                    this.ProxyClientGet = new HttpProxyClient(p_s_proxyHost, p_i_proxyPort, o_networkCredential);
                }
            }
        }

        /// <summary>
        /// Set proxy profile settings for mail client instance for smtp
        /// </summary>
        /// <param name="p_e_proxyType">supported proxy types: HTTP, SOCKS4A, SOCKS5</param>
        /// <param name="p_s_proxyHost">proxy host value</param>
        /// <param name="p_i_proxyPort">proxy port value</param>
        /// <param name="p_b_useDefaultNetworkCredentials">true - using System.Net.CredentialCache.DefaultNetworkCredentials for authentication, false - using user and password value for authentication</param>
        /// <exception cref="ArgumentException">no proxy host value, invalid proxy port number, missing user or password</exception>
        public void SetProxySend(ProxyType p_e_proxyType, string p_s_proxyHost, int p_i_proxyPort, bool p_b_useDefaultNetworkCredentials)
        {
            this.SetProxySend(p_e_proxyType, p_s_proxyHost, p_i_proxyPort, p_b_useDefaultNetworkCredentials, null, null);
        }

        /// <summary>
        /// Set proxy profile settings for mail client instance for smtp
        /// </summary>
        /// <param name="p_e_proxyType">supported proxy types: HTTP, SOCKS4A, SOCKS5</param>
        /// <param name="p_s_proxyHost">proxy host value</param>
        /// <param name="p_i_proxyPort">proxy port value</param>
        /// <param name="p_s_proxyUser">proxy user value</param>
        /// <param name="p_s_proxyPassword">proxy user password value</param>
        /// <exception cref="ArgumentException">no proxy host value, invalid proxy port number, missing user or password</exception>
        public void SetProxySend(ProxyType p_e_proxyType, string p_s_proxyHost, int p_i_proxyPort, string p_s_proxyUser, string p_s_proxyPassword)
        {
            this.SetProxySend(p_e_proxyType, p_s_proxyHost, p_i_proxyPort, false, p_s_proxyUser, p_s_proxyPassword);
        }

        /// <summary>
        /// Set proxy profile settings for mail client instance for smtp
        /// </summary>
        /// <param name="p_e_proxyType">supported proxy types: HTTP, SOCKS4A, SOCKS5</param>
        /// <param name="p_s_proxyHost">proxy host value</param>
        /// <param name="p_i_proxyPort">proxy port value</param>
        /// <param name="p_b_useDefaultNetworkCredentials">true - using System.Net.CredentialCache.DefaultNetworkCredentials for authentication, false - using user and password value for authentication</param>
        /// <param name="p_s_proxyUser">proxy user value</param>
        /// <param name="p_s_proxyPassword">proxy user password value</param>
        /// <exception cref="ArgumentException">no proxy host value, invalid proxy port number, missing user or password</exception>
        private void SetProxySend(ProxyType? p_e_proxyType, string p_s_proxyHost, int p_i_proxyPort, bool p_b_useDefaultNetworkCredentials, string? p_s_proxyUser, string? p_s_proxyPassword)
        {
            /* if proxy type parameter is null, we set proxy profile to null */
            if (p_e_proxyType == null)
            {
                this.ProxyTypeSend = null;
                this.ProxyClientSend = null;
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

                /* save proxy type */
                this.ProxyTypeSend = p_e_proxyType;

                System.Net.NetworkCredential o_networkCredential;

                /* using System.Net.CredentialCache.DefaultNetworkCredentials for authentication */
                if (p_b_useDefaultNetworkCredentials)
                {
                    o_networkCredential = System.Net.CredentialCache.DefaultNetworkCredentials;
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

                    o_networkCredential = new(p_s_proxyUser, p_s_proxyPassword);
                }

                if (p_e_proxyType == ProxyType.SOCKS4A)
                {
                    this.ProxyClientSend = new Socks4aClient(p_s_proxyHost, p_i_proxyPort, o_networkCredential);
                }
                else if (p_e_proxyType == ProxyType.SOCKS5)
                {
                    this.ProxyClientSend = new Socks5Client(p_s_proxyHost, p_i_proxyPort, o_networkCredential);
                }
                else if (p_e_proxyType == ProxyType.HTTP)
                {
                    this.ProxyClientSend = new Socks5Client(p_s_proxyHost, p_i_proxyPort, o_networkCredential);
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
        /// Method to validate receiving certificate
        /// </summary>
        /// <param name="p_o_sender">sender object, probaly having host string name</param>
        /// <param name="p_o_certificate">received certificate</param>
        /// <param name="p_o_chain">certificate chain</param>
        /// <param name="p_e_sslPolicyErrors">enumeration of possible ssl policy errors</param>
        /// <returns>true - certificate validated, false - certificate is not accepted</returns>
        /// <exception cref="NullReferenceException">received certificate is null</exception>
        private bool MyTlsCertificateValidationCallback(object p_o_sender, System.Security.Cryptography.X509Certificates.X509Certificate? p_o_certificate, System.Security.Cryptography.X509Certificates.X509Chain? p_o_chain, System.Net.Security.SslPolicyErrors p_e_sslPolicyErrors)
        {
            /* no policy error, so we can return true */
            if (p_e_sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
                return true;

            /* read sender parameter as host string */
            string s_host = (string)p_o_sender;

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
                foreach (System.Security.Cryptography.X509Certificates.X509Certificate2 o_certificate in this.CertificateAllowList)
                {
                    if (o_certificate.GetRawCertDataString().Equals(o_receivedCertificate.GetRawCertDataString()))
                    {
                        ForestNET.Lib.Global.ILogConfig("certificate allow list match, accepting connection");
                        return true;
                    }
                }

                /* get certificate name */
                string s_certificateName = o_receivedCertificate.GetNameInfo(System.Security.Cryptography.X509Certificates.X509NameType.SimpleName, false);

                ForestNET.Lib.Global.ILogSevere("common name for the SSL certificate did not match '" + s_host + "'. Instead, it was " + s_certificateName);
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

        /// <summary>
        /// Starts mail client instance session with all settings
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.IO.IOException">error within logout method</exception>
        /// <exception cref="ArgumentException">invalid or empty(null) parameter/setting values</exception>
        /// <exception cref="AuthenticationException">could not authenticate with credentials</exception>
        /// <exception cref="ImapCommandException">exception with an imap command</exception>
        /// <exception cref="ImapProtocolException">exception wihin imap protocol</exception>
        /// <exception cref="Pop3CommandException">exception with a pop3 command</exception>
        /// <exception cref="Pop3ProtocolException">exception within pop3 command</exception>
        /// <exception cref="ServiceNotConnectedException">cannot connect to mail server</exception>
        /// <exception cref="MailKit.FolderNotFoundException">mail folder could not be found</exception>
        /// <exception cref="SmtpCommandException">exception with a smtp command</exception>
        /// <exception cref="SmtpProtocolException">exception within smtp command</exception>
        public async Task<bool> Login()
        {
            /* if current instance is logged in, we must re-login */
            if (this.LoggedIn)
            {
                ForestNET.Lib.Global.ILogConfig("current instance is logged in, we must re-login so logout procedure will be called");

                if (!await this.Logout())
                {
                    throw new System.IO.IOException("Error within Logout method");
                }
            }

            /* check mail server parameter if there is no smtp server parameter */
            if ((ForestNET.Lib.Helper.IsStringEmpty(this.MailServer)) && (ForestNET.Lib.Helper.IsStringEmpty(this.SmtpServer)))
            {
                throw new ArgumentException("Empty value for \"Mail-Server\"-parameter and \"SMTP-Server\"");
            }

            /* check user parameter for mail and smtp server */
            if ((ForestNET.Lib.Helper.IsStringEmpty(this.MailUser)) && (ForestNET.Lib.Helper.IsStringEmpty(this.SmtpUser)))
            {
                throw new ArgumentException("Empty value for \"User\"-parameter and \"SMTP-User\"-parameter");
            }

            /* check password parameter for mail and smtp server if we do not use OAuth2 */
            if ((ForestNET.Lib.Helper.IsStringEmpty(this.MailPassword)) && (ForestNET.Lib.Helper.IsStringEmpty(this.SmtpPassword)) && (!this.OAuth2))
            {
                throw new ArgumentException("Empty value for \"Password\"-parameter and \"SMTP-Password\"-parameter");
            }

            /* check min value for mail server parameter */
            if (this.MailPort < 1)
            {
                throw new ArgumentException("Mail-Server Port must be at least '1', but was set to '" + this.MailPort + "'");
            }

            /* check max value for mail server parameter */
            if (this.MailPort > 65535)
            {
                throw new ArgumentException("Mail-Server Port must be lower equal '65535', but was set to '" + this.MailPort + "'");
            }

            /* check min value for smtp server parameter */
            if (this.SmtpPort < 1)
            {
                throw new ArgumentException("SMTP-Server Port must be at least '1', but was set to '" + this.SmtpPort + "'");
            }

            /* check max value for smtp server parameter */
            if (this.SmtpPort > 65535)
            {
                throw new ArgumentException("SMTP-Server Port must be lower equal '65535', but was set to '" + this.SmtpPort + "'");
            }

            ForestNET.Lib.Global.ILogConfig("renew cancellation token source");

            /* renew cancellation token source */
            this.TokenSource = new();

            /* prepare mail session instance to get mails */
            if (!ForestNET.Lib.Helper.IsStringEmpty(this.MailServer))
            {
                ForestNET.Lib.Global.ILogConfig("prepare mail session instance to get mails");

                try
                {
                    /* if pop3 flag is set, use pop3 protocol */
                    if (this.Pop3)
                    {
                        this.Pop3Session = new Pop3Client();
                    }
                    else
                    { /* use imap protocol */
                        this.ImapSession = new ImapClient();
                    }

                    ForestNET.Lib.Global.ILogConfig("use ssl certificate validation callback for protocol " + this.GetSessionProtocol);

                    /* use ssl certificate validation callback */
                    this.GetSession().ServerCertificateValidationCallback = MyTlsCertificateValidationCallback;

                    /* iterate each X.509 certificate in our allow list  */
                    foreach (System.Security.Cryptography.X509Certificates.X509Certificate2 o_certificate in this.CertificateAllowList)
                    {
                        /* add certificate to client certificates collection */
                        this.GetSession().ClientCertificates ??= new System.Security.Cryptography.X509Certificates.X509Certificate2Collection();
                        this.GetSession().ClientCertificates.Add(o_certificate);
                    }

                    /* check if we have a proxy client for pop3 or imap */
                    if (this.ProxyClientGet != null)
                    {
                        ForestNET.Lib.Global.ILogConfig("using proxy client '" + this.ProxyTypeGet + "' for protocol " + this.GetSessionProtocol);

                        this.GetSession().ProxyClient = this.ProxyClientGet;
                    }

                    ForestNET.Lib.Global.ILogConfig("create mail session instance to get mails '" + this.MailServer + ":" + this.MailPort + "'");

                    /* create mail session instance to get mails */
                    if (this.MailSecurityOption == SecurityOptions.Tls)
                    { /* enable ssl on connect */
                        ForestNET.Lib.Global.ILogConfig("enable ssl on connect for protocol " + this.GetSessionProtocol);

                        await this.GetSession().ConnectAsync(this.MailServer, this.MailPort, MailKit.Security.SecureSocketOptions.SslOnConnect, this.Token());

                        /* check if we want to use OAuth2 for authentication and OAuth method is not null */
                        if ((this.OAuth2) && (this.OAuthMethod != null))
                        {
                            ForestNET.Lib.Global.ILogConfig("using OAuth2 for authentication");

                            /* check if all arguments for OAuth2 are available */
                            if ((this.CID == null) || (this.CSecret == null) || (ForestNET.Lib.Helper.IsStringEmpty(this.CID)) || (ForestNET.Lib.Helper.IsStringEmpty(this.CSecret)) || (this.MailServer == null) || (this.MailUser == null))
                            {
                                ForestNET.Lib.Global.ILogConfig("imap server address, imap user, client ID or client secret for OAuth2 is null or empty");
                                ForestNET.Lib.Global.ILogConfig("set OAuth flag to false");
                                this.OAuth2 = false;
                            }
                            else if (this.GetSession().AuthenticationMechanisms.Contains("OAUTHBEARER") || this.GetSession().AuthenticationMechanisms.Contains("XOAUTH2")) /* check if IMAP session support 'OAUTHBEARER' or 'XOAUTH2' */
                            {
                                this.OAuthMethod.Invoke(this.GetSessionAsImapClient, this.MailUser, this.CID, this.CSecret, this.Token()).GetAwaiter().GetResult();
                            }
                            else /* authentication mechanism in current IMAP session does not support 'OAUTHBEARER' or 'XOAUTH2' */
                            {
                                ForestNET.Lib.Global.ILogConfig("IMAP session does not support 'OAUTHBEARER' or 'XOAUTH2'");
                                ForestNET.Lib.Global.ILogConfig("set OAuth flag to false");
                                this.OAuth2 = false;
                            }
                        }
                    }
                    else if (this.MailSecurityOption == SecurityOptions.StartTls)
                    { /* enable starttls */
                        ForestNET.Lib.Global.ILogConfig("enable starttls for protocol " + this.GetSessionProtocol);

                        await this.GetSession().ConnectAsync(this.MailServer, this.MailPort, MailKit.Security.SecureSocketOptions.StartTls, this.Token());
                    }
                    else
                    { /* no receiving encryption */
                        ForestNET.Lib.Global.ILogConfig("no receiving encryption for protocol " + this.GetSessionProtocol);

                        await this.GetSession().ConnectAsync(this.MailServer, this.MailPort, MailKit.Security.SecureSocketOptions.None, this.Token());
                    }

                    ForestNET.Lib.Global.ILogConfig("set timeout to 30 seconds");

                    /* set timeout to 30 seconds */
                    this.GetSession().Timeout = 30000;

                    /* check if we not try or already tried to authenticate with OAuth2 */
                    if (!this.OAuth2)
                    {
                        ForestNET.Lib.Global.ILogConfig("authenticate with user '" + this.MailUser + "' and password");

                        /* authenticate with user and password */
                        await this.GetSession().AuthenticateAsync(this.MailUser, this.MailPassword, this.Token());
                    }
                }
                catch (AuthenticationException o_exc)
                {
                    this.Pop3Session = null; this.ImapSession = null;
                    throw new AuthenticationException("Invalid user name or password for " + this.GetSessionProtocol + "; " + o_exc.Message);
                }
                catch (ImapCommandException o_exc)
                {
                    this.Pop3Session = null; this.ImapSession = null;
                    throw new ImapCommandException(o_exc.Response, o_exc.ResponseText, "IMAP - Error trying to connect: " + o_exc.Message + "\t" + "CommandResponse: " + o_exc.Response + "\t" + "ResponseText: " + o_exc.ResponseText);
                }
                catch (ImapProtocolException o_exc)
                {
                    this.Pop3Session = null; this.ImapSession = null;
                    throw new ImapProtocolException("IMAP protocol error while trying to connect: " + o_exc.Message);
                }
                catch (Pop3CommandException o_exc)
                {
                    this.Pop3Session = null; this.ImapSession = null;
                    throw new Pop3CommandException("POP3 - Error trying to connect: " + o_exc.Message + "\t" + "StatusText: " + o_exc.StatusText);
                }
                catch (Pop3ProtocolException o_exc)
                {
                    this.Pop3Session = null; this.ImapSession = null;
                    throw new Pop3ProtocolException("POP3 protocol error while trying to connect: " + o_exc.Message);
                }
                catch (Exception o_exc)
                {
                    this.Pop3Session = null; this.ImapSession = null;
                    throw new ServiceNotConnectedException("Cannot connect to mail server with protocol '" + this.GetSessionProtocol + "'", o_exc);
                }

                ForestNET.Lib.Global.ILogConfig("use new session instance to connect to mail server and get folder structure from Folder.INBOX");

                /* use new session instance to connect to mail server and get folder structure from Folder.INBOX */
                try
                {
                    ForestNET.Lib.Global.ILogConfig("create default root folder");

                    /* create default root folder */
                    Folder o_defaultFolder = new(Folder.ROOT);

                    ForestNET.Lib.Global.ILogConfig("get any mail folder below root folder");

                    if (this.Pop3)
                    {
                        o_defaultFolder.AddChildren(Folder.INBOX);
                    }
                    else
                    {
                        IList<IMailFolder> a_folders = await this.GetSessionAsImapClient.GetFoldersAsync(this.GetSessionAsImapClient.PersonalNamespaces[0], false, this.Token());

                        /* get any mail folder below root folder */
                        foreach (IMailFolder o_folder in a_folders)
                        {
                            o_defaultFolder.AddChildren(o_folder.Name);
                        }
                    }

                    ForestNET.Lib.Global.ILogConfig("look for Folder.INBOX folder and set it as current folder");

                    /* look for Folder.INBOX folder and set it a s current folder */
                    Folder o_foo = o_defaultFolder.GetSubFolder(Folder.INBOX) ?? throw new MailKit.FolderNotFoundException("Folder '" + Folder.INBOX + "' not found");
                    this.CurrentFolder = o_foo;

                    /* if we found Folder.INBOX folder and using imap protocol */
                    if (!this.Pop3)
                    {
                        ForestNET.Lib.Global.ILogConfig("we found Folder.INBOX folder and using imap protocol. get all sub folders of Folder.INBOX");

                        /* get all sub folder of Folder.INBOX folder and add them as child items */
                        IList<IMailFolder> a_inboxSubFolders = await this.GetSessionAsImapClient.Inbox.GetSubfoldersAsync(false, this.Token());

                        foreach (IMailFolder o_folder in a_inboxSubFolders)
                        {
                            ForestNET.Lib.Global.ILogFine("add folder '" + o_folder.Name + "' as sub folder to Folder.INBOX");

                            this.CurrentFolder.AddChildren(o_folder.Name);
                        }
                    }
                }
                catch (MailKit.FolderNotFoundException o_exc)
                {
                    this.Pop3Session = null; this.ImapSession = null;
                    throw new MailKit.FolderNotFoundException("Folder '" + o_exc.FolderName + "' not found for '" + this.GetSessionProtocol + "'; " + o_exc.Message, o_exc.FolderName);
                }
                catch (ImapCommandException o_exc)
                {
                    this.Pop3Session = null; this.ImapSession = null;
                    throw new ImapCommandException(o_exc.Response, o_exc.ResponseText, "IMAP - Error trying to connect: " + o_exc.Message + "\t" + "CommandResponse: " + o_exc.Response + "\t" + "ResponseText: " + o_exc.ResponseText);
                }
                catch (ImapProtocolException o_exc)
                {
                    this.Pop3Session = null; this.ImapSession = null;
                    throw new ImapProtocolException("IMAP protocol error while trying to connect: " + o_exc.Message);
                }
                catch (Pop3CommandException o_exc)
                {
                    this.Pop3Session = null; this.ImapSession = null;
                    throw new Pop3CommandException("POP3 - Error trying to connect: " + o_exc.Message + "\t" + "StatusText: " + o_exc.StatusText);
                }
                catch (Pop3ProtocolException o_exc)
                {
                    this.Pop3Session = null; this.ImapSession = null;
                    throw new Pop3ProtocolException("POP3 protocol error while trying to connect: " + o_exc.Message);
                }
                catch (Exception o_exc)
                {
                    this.Pop3Session = null; this.ImapSession = null;
                    throw new ServiceNotConnectedException("Cannot call folder structure from mail server with protocol '" + this.GetSessionProtocol + "'", o_exc);
                }
            }
            else
            {
                ForestNET.Lib.Global.ILogConfig("no session for getting mails in use");
            }

            /* prepare mail session instance to send mails */
            if (!ForestNET.Lib.Helper.IsStringEmpty(this.SmtpServer))
            {
                ForestNET.Lib.Global.ILogConfig("prepare mail session instance to send mails");

                try
                {
                    /* prepare mail session instance to send mails */
                    this.SmtpSession = new SmtpClient();

                    ForestNET.Lib.Global.ILogConfig("use ssl certificate validation callback for protocol smtp");

                    /* use ssl certificate validation callback */
                    this.SmtpSession.ServerCertificateValidationCallback = MyTlsCertificateValidationCallback;

                    /* iterate each X.509 certificate in our allow list  */
                    foreach (System.Security.Cryptography.X509Certificates.X509Certificate2 o_certificate in this.CertificateAllowList)
                    {
                        /* add certificate to client certificates collection */
                        this.SmtpSession.ClientCertificates ??= new System.Security.Cryptography.X509Certificates.X509Certificate2Collection();
                        this.SmtpSession.ClientCertificates.Add(o_certificate);
                    }

                    /* check if we have a proxy client for smtp */
                    if (this.ProxyClientSend != null)
                    {
                        ForestNET.Lib.Global.ILogConfig("using proxy client '" + this.ProxyTypeSend + "' for protocol SMTP");

                        this.SmtpSession.ProxyClient = this.ProxyClientSend;
                    }

                    ForestNET.Lib.Global.ILogConfig("create smtp session instance to send mails '" + this.SmtpServer + ":" + this.SmtpPort + "'");

                    /* create mail session instance to get mails */
                    if (this.SmtpSecurityOption == SecurityOptions.Tls)
                    { /* enable ssl on connect */
                        ForestNET.Lib.Global.ILogConfig("enable ssl on connect for protocol smtp");

                        await this.SmtpSession.ConnectAsync(this.SmtpServer, this.SmtpPort, MailKit.Security.SecureSocketOptions.SslOnConnect, this.Token());

                        /* check if we want to use OAuth2 for authentication and OAuth method is not null */
                        if ((this.OAuth2) && (this.OAuthMethod != null))
                        {
                            ForestNET.Lib.Global.ILogConfig("using OAuth2 for authentication");

                            /* check if all arguments for OAuth2 are available */
                            if ((this.CID == null) || (this.CSecret == null) || (ForestNET.Lib.Helper.IsStringEmpty(this.CID)) || (ForestNET.Lib.Helper.IsStringEmpty(this.CSecret)) || (this.SmtpServer == null) || (this.SmtpUser == null))
                            {
                                ForestNET.Lib.Global.ILogConfig("smtp server address, smtp user, client ID or client secret for OAuth2 is null or empty");
                                ForestNET.Lib.Global.ILogConfig("set OAuth flag to false");
                                this.OAuth2 = false;
                            }
                            else if (this.SmtpSession.AuthenticationMechanisms.Contains("OAUTHBEARER") || this.SmtpSession.AuthenticationMechanisms.Contains("XOAUTH2")) /* check if SMTP session support 'OAUTHBEARER' or 'XOAUTH2' */
                            {
                                this.OAuthMethod.Invoke(this.SmtpSession, this.SmtpUser, this.CID, this.CSecret, this.Token()).GetAwaiter().GetResult();
                            }
                            else /* authentication mechanism in current SMTP session does not support 'OAUTHBEARER' or 'XOAUTH2' */
                            {
                                ForestNET.Lib.Global.ILogConfig("IMAP session does not support 'OAUTHBEARER' or 'XOAUTH2'");
                                ForestNET.Lib.Global.ILogConfig("set OAuth flag to false");
                                this.OAuth2 = false;
                            }
                        }
                    }
                    else if (this.SmtpSecurityOption == SecurityOptions.StartTls)
                    { /* enable starttls */
                        ForestNET.Lib.Global.ILogConfig("enable starttls for protocol smtp");

                        await this.SmtpSession.ConnectAsync(this.SmtpServer, this.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls, this.Token());
                    }
                    else
                    { /* no receiving encryption */
                        ForestNET.Lib.Global.ILogConfig("no receiving encryption for protocol smtp");

                        await this.SmtpSession.ConnectAsync(this.SmtpServer, this.SmtpPort, MailKit.Security.SecureSocketOptions.None, this.Token());
                    }

                    ForestNET.Lib.Global.ILogConfig("set timeout to 30 seconds");

                    /* set timeout to 30 seconds */
                    this.SmtpSession.Timeout = 30000;

                    /* check if we not try or already tried to authenticate with OAuth2 */
                    if (!this.OAuth2)
                    {
                        ForestNET.Lib.Global.ILogConfig("authenticate with user '" + this.SmtpUser + "' and password");

                        /* authenticate with user and password */
                        await this.SmtpSession.AuthenticateAsync(this.SmtpUser, this.SmtpPassword, this.Token());
                    }
                }
                catch (AuthenticationException o_exc)
                {
                    throw new AuthenticationException("Invalid user name or password for smtp; " + o_exc.Message);
                }
                catch (SmtpCommandException o_exc)
                {
                    throw new SmtpCommandException(o_exc.ErrorCode, o_exc.StatusCode, "SMTP - Error trying to connect: " + o_exc.Message + "\t" + "StatusCode: " + o_exc.StatusCode);
                }
                catch (SmtpProtocolException o_exc)
                {
                    throw new SmtpProtocolException("SMTP protocol error while trying to connect: " + o_exc.Message);
                }
            }
            else
            {
                ForestNET.Lib.Global.ILogConfig("no session for sending mails in use");
            }

            return this.LoggedIn = true;
        }

        /// <summary>
        /// Logout current client instance session
        /// </summary>
        /// <returns>true - logout complete, false - logout incomplete</returns>
        public async Task<bool> Logout()
        {
            this.LoggedIn = false;

            /* logout and disconnect pop3 connection */
            if (this.Pop3Session != null)
            {
                try
                {
                    try
                    {
                        ForestNET.Lib.Global.ILogConfig("disconnect pop3 connection");

                        /* disconnect pop3 connection */
                        await this.Pop3Session.DisconnectAsync(true, this.Token());
                    }
                    finally
                    {
                        ForestNET.Lib.Global.ILogConfig("call Cancel method of cancellation token source");

                        /* call Cancel method of cancellation token source */
                        this.TokenSource?.Cancel();

                        ForestNET.Lib.Global.ILogConfig("dispose pop3 client instance");

                        /* dispose pop3 client instance */
                        this.Pop3Session.Dispose();
                    }
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.ILogSevere("error with disconnect and dispose pop3 instance: " + o_exc);
                }
                finally
                {
                    ForestNET.Lib.Global.ILogConfig("set pop3 client instance to null");

                    /* set pop3 client instance to null */
                    this.Pop3Session = null;
                }
            }

            /* logout and disconnect imap connection */
            if (this.ImapSession != null)
            {
                try
                {
                    try
                    {
                        ForestNET.Lib.Global.ILogConfig("disconnect imap connection");

                        /* disconnect imap connection */
                        await this.ImapSession.DisconnectAsync(true, this.Token());
                    }
                    finally
                    {
                        /* if token source not already been canceled */
                        if ((this.TokenSource != null) && (!this.TokenSource.IsCancellationRequested))
                        {
                            ForestNET.Lib.Global.ILogConfig("call Cancel method of cancellation token source");

                            /* call Cancel method of cancellation token source */
                            this.TokenSource?.Cancel();
                        }

                        ForestNET.Lib.Global.ILogConfig("dispose imap client instance");

                        /* dispose imap client instance */
                        this.ImapSession.Dispose();
                    }
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.ILogSevere("error with disconnect and dispose imap instance: " + o_exc);
                }
                finally
                {
                    ForestNET.Lib.Global.ILogConfig("set imap client instance to null");

                    /* set imap client instance to null */
                    this.ImapSession = null;
                }
            }

            /* logout and disconnect smtp connection */
            if (this.SmtpSession != null)
            {
                try
                {
                    try
                    {
                        ForestNET.Lib.Global.ILogConfig("disconnect smtp connection");

                        /* disconnect smtp connection */
                        await this.SmtpSession.DisconnectAsync(true, this.Token());
                    }
                    finally
                    {
                        /* if token source not already been canceled */
                        if ((this.TokenSource != null) && (!this.TokenSource.IsCancellationRequested))
                        {
                            ForestNET.Lib.Global.ILogConfig("call Cancel method of cancellation token source");

                            /* call Cancel method of cancellation token source */
                            this.TokenSource?.Cancel();
                        }

                        ForestNET.Lib.Global.ILogConfig("dispose smtp client instance");

                        /* dispose smtp client instance */
                        this.SmtpSession.Dispose();
                    }
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.ILogSevere("error with disconnect and dispose smtp instance: " + o_exc);
                }
                finally
                {
                    ForestNET.Lib.Global.ILogConfig("set smtp client instance to null");

                    /* set smtp client instance to null */
                    this.SmtpSession = null;
                }
            }

            return (this.TokenSource != null) && this.TokenSource.IsCancellationRequested;
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
        /// Get current imap folder
        /// </summary>
        /// <returns>folder instance</returns>
        /// <exception cref="ArgumentException">pop3 is not supported</exception>
        public Folder? GetCurrentFolder()
        {
            if (this.Pop3)
            {
                throw new ArgumentException("POP3(s) protocol is not supported");
            }

            return this.CurrentFolder;
        }

        /// <summary>
        /// Send a mail message with smtp server
        /// </summary>
        /// <param name="p_o_mailMessage">mail message object which holds all necessary information incl. html content and/or attachments (both optional)</param>
        /// <exception cref="ArgumentException">smtp user is not set</exception>
        /// <exception cref="InvalidOperationException">we have no session instance for sending mails</exception>
        /// <exception cref="ApplicationException">could not validate mail address, could not create or set mime content or mime body part objects, issue with sending mail message with smtp server</exception>
        /// <exception cref="System.IO.IOException">attachment file does not exist on local system</exception>
        public async Task SendMessage(Message p_o_mailMessage)
        {
            ForestNET.Lib.Global.ILogFiner("check if we have a session instance for sending mails");

            /* check if we have a session instance for sending mails */
            if (this.SmtpSession == null)
            {
                throw new InvalidOperationException("mail client has no settings to send mails. cannot send a message");
            }

            /* create mime message object for sending mail */
            MimeMessage o_message = new();
            InternetAddress o_mailAddress;

            if (p_o_mailMessage.From != null)
            { /* check if we have a session instance for sending mails */
                ForestNET.Lib.Global.ILogFiner("check if we have a session instance for sending mails");

                try
                {
                    ForestNET.Lib.Global.ILogFiner("check mail address validity");

                    /* check mail address validity */
                    o_mailAddress = new MailboxAddress(p_o_mailMessage.From, p_o_mailMessage.From);
                }
                catch (ParseException o_exc)
                {
                    throw new ApplicationException("Invalid 'From' address", o_exc);
                }
            }
            else
            { /* auto fill from address from user and address value for smtp server */
                ForestNET.Lib.Global.ILogFiner("auto fill from address from user and address value for smtp server");

                ForestNET.Lib.Global.ILogFiner("check if we have a smtp user");

                /* check if we have a smtp user */
                if ((this.SmtpUser == null) && (ForestNET.Lib.Helper.IsStringEmpty(this.SmtpUser)))
                {
                    throw new ArgumentException("SMTP-User is empty");
                }

                try
                {
                    ForestNET.Lib.Global.ILogFiner("check mail address validity");

                    /* check mail address validity */
                    if ((this.SmtpUser ?? throw new ArgumentException("SMTP-User is empty")).Contains('@'))
                    {
                        o_mailAddress = new MailboxAddress(this.SmtpUser, this.SmtpUser);
                    }
                    else
                    {
                        o_mailAddress = new MailboxAddress(this.SmtpUser, this.SmtpUser + "@" + this.SmtpServer);
                    }
                }
                catch (ParseException o_exc)
                {
                    throw new ApplicationException("Invalid user address '" + this.SmtpUser + "' or '" + this.SmtpUser + "@" + this.SmtpServer + "'", o_exc);
                }
            }

            ForestNET.Lib.Global.ILogFiner("set from address of mime message object");

            /* set from address of mime message object */
            o_message.From.Add(o_mailAddress);

            ForestNET.Lib.Global.ILogFiner("add recipients to mime message object");

            /* add recipients to mime message object */
            if (p_o_mailMessage.ToList.Count > 0)
            {
                foreach (string s_to in p_o_mailMessage.ToList)
                {
                    o_message.To.Add(new MailboxAddress(s_to, s_to));
                }
            }

            ForestNET.Lib.Global.ILogFiner("add cc to mime message object");

            /* add cc to mime message object */
            if (p_o_mailMessage.CCList.Count > 0)
            {
                foreach (string s_cc in p_o_mailMessage.CCList)
                {
                    o_message.Cc.Add(new MailboxAddress(s_cc, s_cc));
                }
            }

            ForestNET.Lib.Global.ILogFiner("add bcc to mime message object");

            /* add bcc to mime message object */
            if (p_o_mailMessage.BCCList.Count > 0)
            {
                foreach (string s_bcc in p_o_mailMessage.BCCList)
                {
                    o_message.Bcc.Add(new MailboxAddress(s_bcc, s_bcc));
                }
            }

            ForestNET.Lib.Global.ILogFiner("add subject to mime message object");

            /* add subject to mime message object */
            o_message.Subject = p_o_mailMessage.Subject;

            /* create mime content variable */
            Multipart? o_mimeContent = null;

            /* if we have plain text content and html content */
            if ((!ForestNET.Lib.Helper.IsStringEmpty(p_o_mailMessage.Text)) && (!ForestNET.Lib.Helper.IsStringEmpty(p_o_mailMessage.Html)))
            {
                ForestNET.Lib.Global.ILogFiner("we have plain text content and html content");

                ForestNET.Lib.Global.ILogFiner("set mime content to 'alternative'");

                /* set mime content to 'alternative' */
                o_mimeContent = new Multipart("alternative");

                ForestNET.Lib.Global.ILogFiner("add plain text as mime body part");

                /* add plain text as mime body part */
                TextPart o_plain = new(MimeKit.Text.TextFormat.Plain)
                {
                    Text = p_o_mailMessage.Text
                };

                o_mimeContent.Add(o_plain);

                ForestNET.Lib.Global.ILogFiner("add html text as mime body part");

                /* add html text as mime body part */
                TextPart o_html = new(MimeKit.Text.TextFormat.Html)
                {
                    Text = p_o_mailMessage.Html
                };

                o_mimeContent.Add(o_html);

                ForestNET.Lib.Global.ILogFiner("set mime mail message content with mime content");

                /* set mime mail message content with mime content */
                o_message.Body = o_mimeContent;
            }
            else
            { /* we only have plain text content */
                ForestNET.Lib.Global.ILogFiner("we only have plain text content");

                ForestNET.Lib.Global.ILogFiner("set mime mail message content with plain text");

                /* set mime mail message content with plain text */
                o_message.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
                {
                    Text = p_o_mailMessage.Text
                };
            }

            if ((p_o_mailMessage.Attachments != null) && (p_o_mailMessage.Attachments.Count > 0))
            {
                ForestNET.Lib.Global.ILogFiner("we have attachments for mail message, set mime content parent to 'mixed'");

                /* we have attachments for mail message, set mime content parent to 'mixed' */
                Multipart o_mimeContentParent = new("mixed");

                if (o_mimeContent != null)
                { /* get mime content object plain text content and html content(optional) */
                    ForestNET.Lib.Global.ILogFiner("get mime content object plain text content and html content(optional)");

                    ForestNET.Lib.Global.ILogFiner("add mime content to mime body part of mixed mime multipart");

                    /* add mime content to mime body part of mixed mime multipart */
                    o_mimeContentParent.Add(o_mimeContent);
                }
                else
                { /* just get plain text mime content */
                    ForestNET.Lib.Global.ILogFiner("just get plain text mime content");

                    ForestNET.Lib.Global.ILogFiner("add plain text content from before to mime body part of mixed mime multipart");

                    /* add plain text content from before to mime body part of mixed mime multipart */
                    o_mimeContentParent.Add(
                        new TextPart(MimeKit.Text.TextFormat.Plain)
                        {
                            Text = p_o_mailMessage.Text
                        }
                    );
                }

                ForestNET.Lib.Global.ILogFiner("iterate each attachment");

                /* iterate each attachment */
                foreach (string s_filename in p_o_mailMessage.Attachments)
                {
                    ForestNET.Lib.Global.ILogFinest("check if file '" + s_filename + "' really exists on local system");

                    /* check if file really exists on local system */
                    if (!ForestNET.Lib.IO.File.Exists(s_filename))
                    {
                        throw new System.IO.IOException("File[" + s_filename + "] does not exist and cannot be attached to E-Mail-Message");
                    }

                    ForestNET.Lib.Global.ILogFinest("add body part with attachment content and file name");

                    /* add body part with attachment content and file name */
                    MimePart o_attachmentPart = new(MimeKit.ContentType.Parse(MimeTypes.GetMimeType(s_filename)))
                    {
                        Content = new MimeContent(File.OpenRead(s_filename), ContentEncoding.Default),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = Path.GetFileName(s_filename)
                    };

                    o_mimeContentParent.Add(o_attachmentPart);
                }

                ForestNET.Lib.Global.ILogFiner("overwrite mime mail message content with mixed mime multipart");

                /* overwrite mime mail message content with mixed mime multipart */
                o_message.Body = o_mimeContentParent;
            }

            ForestNET.Lib.Global.ILogFiner("send mime mail message");

            /* send mime mail message */
            _ = await this.SmtpSession.SendAsync(o_message, this.Token(), null);

            /* current mail client has imap(s) settings, so we add message to sent folder; pop3(s) is not supported */
            if (this.ImapSession != null)
            {
                ForestNET.Lib.Global.ILogFiner("current mail client has imap(s) settings, so we add message to sent folder; pop3(s) is not supported");

                try
                {
                    ForestNET.Lib.Global.ILogFiner("get sent folder below root folder");

                    /* get sent folder below root folder */
                    IMailFolder o_folder = await this.GetSessionAsImapClient.GetFolderAsync(Folder.SENT, this.Token());

                    ForestNET.Lib.Global.ILogFiner("append message to sent folder");

                    /* append message to sent folder */
                    UniqueId o_uniqueId = await o_folder.AppendAsync(new AppendRequest(o_message), this.Token()) ?? throw new ImapCommandException(ImapCommandResponse.Bad, "Could not add message to '" + Folder.SENT + "' folder");

                    ForestNET.Lib.Global.ILogFiner("reached SENT folder, open it with read write access");

                    /* reached SENT folder, open it with read write access */
                    FolderAccess e_folderAccess = await o_folder.OpenAsync(FolderAccess.ReadWrite, this.Token());

                    if (e_folderAccess != FolderAccess.ReadWrite)
                    {
                        throw new ImapCommandException(ImapCommandResponse.Bad, "Could not open '" + Folder.SENT + "' folder with read write access");
                    }

                    ForestNET.Lib.Global.ILogFiner("set SEEN flag for message with unqiueid '" + o_uniqueId + "'");

                    /* set SEEN flag for message */
                    await o_folder.AddFlagsAsync(o_uniqueId, MessageFlags.Seen, true, this.Token());

                    ForestNET.Lib.Global.ILogFiner("close sent folder");

                    /* close sent folder */
                    await o_folder.CloseAsync(false, this.Token());
                }
                catch (ImapCommandException)
                {
                    throw;
                }
                catch (Exception o_exc)
                {
                    throw new ImapCommandException(ImapCommandResponse.Bad, "Could not add message to '" + Folder.SENT + "' folder with protocol '" + this.GetSessionProtocol + "'; " + o_exc.Message);
                }
            }
        }

        /// <summary>
        /// Convert a mail flag to string flag value
        /// </summary>
        /// <param name="p_o_flag">mail flag value</param>
        /// <returns>string					part of ForestNET.Lib.Net.Mail.Message.FLAGS</returns>
        private static string FlagToString(MessageFlags p_e_flag)
        {
            if (p_e_flag.Equals(MessageFlags.Answered))
            {
                return Message.FLAGS[0];
            }
            else if (p_e_flag.Equals(MessageFlags.Deleted))
            {
                return Message.FLAGS[1];
            }
            else if (p_e_flag.Equals(MessageFlags.Draft))
            {
                return Message.FLAGS[2];
            }
            else if (p_e_flag.Equals(MessageFlags.Flagged))
            {
                return Message.FLAGS[3];
            }
            else if (p_e_flag.Equals(MessageFlags.Recent))
            {
                return Message.FLAGS[4];
            }
            else if (p_e_flag.Equals(MessageFlags.Seen))
            {
                return Message.FLAGS[5];
            }
            else if (p_e_flag.Equals(MessageFlags.UserDefined))
            {
                return Message.FLAGS[6];
            }
            else
            {
                ForestNET.Lib.Global.ILogWarning("Could not convert mail flag '" + p_e_flag.ToString() + "' - flag is unkown");

                return "UNKNOWN_FLAG";
            }
        }

        /// <summary>
        /// Converts a string flag value to a mail flag
        /// </summary>
        /// <param name="p_s_flag">string flag value - part of ForestNET.Lib.Net.Mail.Message.FLAGS</param>
        /// <returns>MessageFlags</returns>
        /// <exception cref="ArgumentException">parameter flag value is an unknown flag</exception>
        private static MessageFlags StringToFlag(string p_s_flag)
        {
            if (p_s_flag.Equals(Message.FLAGS[0]))
            {
                return MessageFlags.Answered;
            }
            else if (p_s_flag.Equals(Message.FLAGS[1]))
            {
                return MessageFlags.Deleted;
            }
            else if (p_s_flag.Equals(Message.FLAGS[2]))
            {
                return MessageFlags.Draft;
            }
            else if (p_s_flag.Equals(Message.FLAGS[3]))
            {
                return MessageFlags.Flagged;
            }
            else if (p_s_flag.Equals(Message.FLAGS[4]))
            {
                return MessageFlags.Recent;
            }
            else if (p_s_flag.Equals(Message.FLAGS[5]))
            {
                return MessageFlags.Seen;
            }
            else if (p_s_flag.Equals(Message.FLAGS[6]))
            {
                return MessageFlags.UserDefined;
            }
            else
            {
                throw new ArgumentException("UNKNOWN_FLAG");
            }
        }

        /// <summary>
        /// Get all messages from current mail folder, set seen flag of all messages we are reading from current folder, download all attachments from all messages to message objects, can be stored later to local system, do not set delete flag for any message
        /// </summary>
        /// <returns>list of mail message objects</returns>
        /// <exception cref="ArgumentException">current folder is Folder.ROOT where no messages are allowed</exception>
        /// <exception cref="Exception">connection failed to mail or smtp server, wrong folder path, could not open folder with read write access, could not get messages, could not wrap message, issue set/unset a flag of a message, issue closing folder</exception>
        /// <exception cref="System.IO.IOException">issue handling mime content or bodypart content, or could not retrieve attachment content to byte array output stream</exception>
        public async Task<List<Message>> GetMessages()
        {
            return await this.GetMessages(false);
        }

        /// <summary>
        /// Get all messages from current mail folder, set seen flag of all messages we are reading from current folder, download all attachments from all messages to message objects, can be stored later to local system
        /// </summary>
        /// <param name="p_b_setDeleted">true - set delete flag of all messages we are reading from current folder, false - do not set delete flag for any message</param>
        /// <returns>list of mail message objects</returns>
        /// <exception cref="ArgumentException">current folder is Folder.ROOT where no messages are allowed</exception>
        /// <exception cref="Exception">connection failed to mail or smtp server, wrong folder path, could not open folder with read write access, could not get messages, could not wrap message, issue set/unset a flag of a message, issue closing folder</exception>
        /// <exception cref="System.IO.IOException">issue handling mime content or bodypart content, or could not retrieve attachment content to byte array output stream</exception>
        public async Task<List<Message>> GetMessages(bool p_b_setDeleted)
        {
            return await this.GetMessages(null, p_b_setDeleted, false);
        }

        /// <summary>
        /// Get all messages from current mail folder, set seen flag of all messages we are reading from current folder
        /// </summary>
        /// <param name="p_b_setDeleted">true - set delete flag of all messages we are reading from current folder, false - do not set delete flag for any message</param>
        /// <param name="p_b_ignoreAttachments">true - ignore attachments of all messages, false - download all attachments from all messages to message objects, can be stored later to local system</param>
        /// <returns>list of mail message objects</returns>
        /// <exception cref="ArgumentException">current folder is Folder.ROOT where no messages are allowed</exception>
        /// <exception cref="Exception">connection failed to mail or smtp server, wrong folder path, could not open folder with read write access, could not get messages, could not wrap message, issue set/unset a flag of a message, issue closing folder</exception>
        /// <exception cref="System.IO.IOException">issue handling mime content or bodypart content, or could not retrieve attachment content to byte array output stream</exception>
        public async Task<List<Message>> GetMessages(bool p_b_setDeleted, bool p_b_ignoreAttachments)
        {
            return await this.GetMessages(null, p_b_setDeleted, p_b_ignoreAttachments, true);
        }

        /// <summary>
        /// Get all messages from current mail folder
        /// </summary>
        /// <param name="p_b_setDeleted">true - set delete flag of all messages we are reading from current folder, false - do not set delete flag for any message</param>
        /// <param name="p_b_ignoreAttachments">true - ignore attachments of all messages, false - download all attachments from all messages to message objects, can be stored later to local system</param>
        /// <param name="p_b_setSeen">true - set seen flag of all messages we are reading from current folder, false - unset seen flag if reading message auto set seen flag for message</param>
        /// <returns>list of mail message objects</returns>
        /// <exception cref="ArgumentException">current folder is Folder.ROOT where no messages are allowed</exception>
        /// <exception cref="Exception">connection failed to mail or smtp server, wrong folder path, could not open folder with read write access, could not get messages, could not wrap message, issue set/unset a flag of a message, issue closing folder</exception>
        /// <exception cref="System.IO.IOException">issue handling mime content or bodypart content, or could not retrieve attachment content to byte array output stream</exception>
        public async Task<List<Message>> GetMessages(bool p_b_setDeleted, bool p_b_ignoreAttachments, bool p_b_setSeen)
        {
            return await this.GetMessages(null, p_b_setDeleted, p_b_ignoreAttachments, p_b_setSeen);
        }

        /// <summary>
        /// Get all messages from target mail folder, set seen flag of all messages we are reading from target folder
        /// </summary>
        /// <param name="p_s_folderPath">target mail folder where we want to read all messages from</param>
        /// <param name="p_b_setDeleted">true - set delete flag of all messages we are reading from target folder, false - do not set delete flag for any message</param>
        /// <param name="p_b_ignoreAttachments">true - ignore attachments of all messages, false - download all attachments from all messages to message objects, can be stored later to local system</param>
        /// <returns>list of mail message objects</returns>
        /// <exception cref="ArgumentException">current folder is Folder.ROOT where no messages are allowed</exception>
        /// <exception cref="Exception">connection failed to mail or smtp server, wrong folder path, could not open folder with read write access, could not get messages, could not wrap message, issue set/unset a flag of a message, issue closing folder</exception>
        /// <exception cref="System.IO.IOException">issue handling mime content or bodypart content, or could not retrieve attachment content to byte array output stream</exception>
        public async Task<List<Message>> GetMessages(string? p_s_folderPath, bool p_b_setDeleted, bool p_b_ignoreAttachments)
        {
            return await this.GetMessages(p_s_folderPath, p_b_setDeleted, p_b_ignoreAttachments, true);
        }

        /// <summary>
        /// Get all messages from target mail folder
        /// </summary>
        /// <param name="p_s_folderPath">target mail folder where we want to read all messages from</param>
        /// <param name="p_b_setDeleted">true - set delete flag of all messages we are reading from target folder, false - do not set delete flag for any message</param>
        /// <param name="p_b_ignoreAttachments">true - ignore attachments of all messages, false - download all attachments from all messages to message objects, can be stored later to local system</param>
        /// <param name="p_b_setSeen">true - set seen flag of all messages we are reading from target folder, false - unset seen flag if reading message auto set seen flag for message</param>
        /// <returns>list of mail message objects</returns>
        /// <exception cref="ArgumentException">current folder is Folder.ROOT where no messages are allowed</exception>
        /// <exception cref="Exception">connection failed to mail server, wrong folder path, could not open folder with read write access, could not get messages, could not wrap message, issue set/unset a flag of a message, issue closing folder</exception>
        /// <exception cref="System.IO.IOException">issue handling mime content or bodypart content, or could not retrieve attachment content to byte array output stream</exception>
        public async Task<List<Message>> GetMessages(string? p_s_folderPath, bool p_b_setDeleted, bool p_b_ignoreAttachments, bool p_b_setSeen)
        {
            /* check if mail client is set for retrieving mails */
            if ((this.ImapSession == null) && (this.Pop3Session == null))
            {
                throw new InvalidOperationException("mail client has no settings to retrieve mails from server");
            }

            ForestNET.Lib.Global.ILogFine("check if current folder is not Folder.ROOT");

            /* check if current folder is not Folder.ROOT */
            if ((this.CurrentFolder == null) || (ForestNET.Lib.Helper.IsStringEmpty(this.CurrentFolder.Name)))
            {
                throw new ArgumentException("Current folder is default folder, please specifiy a sub folder like 'inbox' or 'trash'");
            }

            /* list of mail message objects */
            List<Message> a_return = [];

            try
            {
                if (!this.Pop3)
                { /* handle IMAP */
                    ForestNET.Lib.Global.ILogFine("get folders from top level");

                    /* target folder variable */
                    IMailFolder? o_targetFolder = null;

                    ForestNET.Lib.Global.ILogFine("get full path to current folder '" + this.CurrentFolder.Name + "'");

                    /* get full path to current folder */
                    string s_folderPath = this.CurrentFolder.GetFullPath();

                    /* if we have a target folder path, overwrite variable and not stay in current folder */
                    if ((p_s_folderPath != null) && (!ForestNET.Lib.Helper.IsStringEmpty(p_s_folderPath)))
                    {
                        ForestNET.Lib.Global.ILogFine("target folder path set, overwrite variable and not stay in current folder");

                        s_folderPath = p_s_folderPath;
                    }

                    ForestNET.Lib.Global.ILogFine("enter target folder '" + s_folderPath + "'");

                    /* enter target folder */
                    o_targetFolder = await this.GetSessionAsImapClient.GetFolderAsync(s_folderPath, this.Token());

                    if (o_targetFolder == null)
                    {
                        throw new ImapCommandException(ImapCommandResponse.Bad, "Could not reach target folder '" + s_folderPath + "'");
                    }

                    ForestNET.Lib.Global.ILogFine("reached target folder, open it with read write access");

                    /* reached target folder, open it with read write access */
                    FolderAccess e_folderAccess = await o_targetFolder.OpenAsync(FolderAccess.ReadWrite, this.Token());

                    if (e_folderAccess != FolderAccess.ReadWrite)
                    {
                        throw new ImapCommandException(ImapCommandResponse.Bad, "Could not open folder '" + o_targetFolder.Name + "' with read write access");
                    }

                    ForestNET.Lib.Global.ILogFine("get message summaries from target folder");

                    /* get message summaries from target folder */
                    IList<IMessageSummary> a_messages = await o_targetFolder.FetchAsync(0, -1, MessageSummaryItems.Full | MessageSummaryItems.UniqueId, this.Token());

                    /* check if we have no messages in target folder */
                    if (a_messages.Count < 1)
                    {
                        ForestNET.Lib.Global.ILogFine("no messages in target folder '" + o_targetFolder.Name + "'");
                    }

                    ForestNET.Lib.Global.ILogFine("iterate each mail message in target folder '" + o_targetFolder.Name + "'");

                    /* iterate each mail message in target folder */
                    foreach (IMessageSummary o_message in a_messages)
                    {
                        ForestNET.Lib.Global.ILogFiner("add message to return list with wrap method");

                        /* add message to return list with wrap method */
                        a_return.Add(await this.WrapMessage(await o_targetFolder.GetMessageAsync(o_message.UniqueId, this.Token(), null), o_message.Flags, p_b_ignoreAttachments));

                        /* set deleted flag if parameter is set */
                        if (p_b_setDeleted)
                        {
                            ForestNET.Lib.Global.ILogFiner("set deleted flag if parameter is set");

                            await o_targetFolder.AddFlagsAsync(o_message.UniqueId, MessageFlags.Deleted, true, this.Token());
                        }

                        if (p_b_setSeen)
                        { /* set seen flag if parameter is set */
                            ForestNET.Lib.Global.ILogFiner("set seen flag if parameter is set");

                            await o_targetFolder.AddFlagsAsync(o_message.UniqueId, MessageFlags.Seen, true, this.Token());
                        }
                    }

                    ForestNET.Lib.Global.ILogFine("close target folder, expunge all deleted messages if parameter is set");

                    /* close target folder, expunge all deleted messages if parameter is set */
                    await o_targetFolder.CloseAsync(p_b_setDeleted, this.Token());
                }
                else
                { /* handle POP3 */
                    ForestNET.Lib.Global.ILogFine("get messages from '" + Folder.INBOX + "'");

                    /* check if INBOX is empty */
                    if (this.GetSessionAsPop3Client.Count < 1)
                    {
                        ForestNET.Lib.Global.ILogFine("no messages in '" + Folder.INBOX + "'");

                        /* no messages in INBOX */
                        return a_return;
                    }

                    /* get messages from INBOX */
                    IList<MimeMessage> a_messages = await this.GetSessionAsPop3Client.GetMessagesAsync(0, this.GetSessionAsPop3Client.Count, this.Token());

                    ForestNET.Lib.Global.ILogFine("iterate each mail message in '" + Folder.INBOX + "'");

                    int i = 0;

                    /* iterate each mail message in INBOX */
                    foreach (MimeMessage o_message in a_messages)
                    {
                        ForestNET.Lib.Global.ILogFiner("add message to return list with wrap method");

                        /* add message to return list with wrap method */
                        a_return.Add(await this.WrapMessage(o_message, null, p_b_ignoreAttachments));

                        /* delete message if parameter is set */
                        if (p_b_setDeleted)
                        {
                            ForestNET.Lib.Global.ILogFiner("delete message if parameter is set");

                            await this.GetSessionAsPop3Client.DeleteMessageAsync(i, this.Token());
                        }

                        i++;
                    }
                }
            }
            catch (Exception o_exc)
            {
                throw new Exception("Exception while get all messages from '" + (p_s_folderPath ?? "INBOX") + "'; " + o_exc.Message);
            }

            /* return list of mail message objects */
            return a_return;
        }

        /// <summary>
        /// Wrap mime mail message to own mail message object with all necessary information
        /// </summary>
        /// <param name="p_o_message">mime mail message object where we retrieve all necessary information</param>
        /// <param name="p_e_messageFlags">message flags of current message, only supported with IMAP protocol</param>
        /// <param name="p_b_ignoreAttachments">true - ignore attachments of mail message, false - download all attachments of mail message, can be stored later to local system</param>
        /// <returns>ForestNET.Lib.Net.Mail.Message</returns>
        /// <exception cref="ArgumentException">parameters have invalid value or are empty</exception>
        /// <exception cref="Exception">issue retrieving information from mime mail message parameter</exception>
        /// <exception cref="System.IO.IOException">issue handling mime content or bodypart content, or could not retrieve attachment content to byte array output stream</exception>
        private async Task<Message> WrapMessage(MimeMessage p_o_message, MessageFlags? p_e_messageFlags, bool p_b_ignoreAttachments)
        {
            ForestNET.Lib.Global.ILogFiner("create mail message object with some standard settings");

            /* create mail message object with some standard settings */
            Message o_mailMessage = new("myself@localhost.com", "No subject available", "No plain text message available");

            /* temporary string list */
            List<string> a_foo = [];

            ForestNET.Lib.Global.ILogFiner("get all messages from 'from' property");

            /* get all messages from 'from' property */
            if ((p_o_message.From != null) && (p_o_message.From.Count > 0))
            {
                foreach (MailboxAddress o_address in p_o_message.From.Mailboxes)
                {
                    a_foo.Add(o_address.Address);
                }
            }

            ForestNET.Lib.Global.ILogFiner("adopt 'from' addresses to mail message object");

            /* adopt 'from' addresses to mail message object */
            o_mailMessage.FromList = a_foo;
            a_foo.Clear();

            ForestNET.Lib.Global.ILogFiner("get all messages from 'to' property");

            /* get all messages from 'to' property */
            if ((p_o_message.To != null) && (p_o_message.To.Count > 0))
            {
                foreach (MailboxAddress o_address in p_o_message.To.Mailboxes)
                {
                    a_foo.Add(o_address.Address);
                }
            }

            ForestNET.Lib.Global.ILogFiner("adopt 'to' addresses to mail message object");

            /* adopt 'to' addresses to mail message object */
            o_mailMessage.ToList = a_foo;
            a_foo.Clear();

            ForestNET.Lib.Global.ILogFiner("get all messages from 'cc' property");

            /* get all messages from 'cc' property */
            if ((p_o_message.Cc != null) && (p_o_message.Cc.Count > 0))
            {
                foreach (MailboxAddress o_address in p_o_message.Cc.Mailboxes)
                {
                    a_foo.Add(o_address.Address);
                }
            }

            ForestNET.Lib.Global.ILogFiner("adopt 'cc' addresses to mail message object");

            /* adopt 'cc' addresses to mail message object */
            o_mailMessage.CCList = a_foo;
            a_foo.Clear();

            ForestNET.Lib.Global.ILogFiner("get all messages from 'bcc' property");

            /* get all messages from 'bcc' property */
            if ((p_o_message.Bcc != null) && (p_o_message.Bcc.Count > 0))
            {
                foreach (MailboxAddress o_address in p_o_message.Bcc.Mailboxes)
                {
                    a_foo.Add(o_address.Address);
                }
            }

            ForestNET.Lib.Global.ILogFiner("adopt 'bcc' addresses to mail message object");

            /* adopt 'bcc' addresses to mail message object */
            o_mailMessage.BCCList = a_foo;
            a_foo.Clear();

            ForestNET.Lib.Global.ILogFiner("get mail message subject, send-timestamp and message-id");

            /* get mail message subject, send-timestamp and message-id */
            o_mailMessage.Subject = p_o_message.Subject;
            o_mailMessage.Send = p_o_message.Date.LocalDateTime;
            o_mailMessage.MessageId = p_o_message.MessageId;

            /* check if we have message flags for current message */
            if (p_e_messageFlags.HasValue)
            {
                ForestNET.Lib.Global.ILogFiner("create list of all possible mail message flags");

                /* create list of all possible mail message flags */
                List<MessageFlags> a_flags = [];

                /* fill list with valid flag values */
                foreach (string s_foo in Message.FLAGS)
                {
                    a_flags.Add(StringToFlag(s_foo));
                }

                ForestNET.Lib.Global.ILogFiner("check if mail message parameter has active flag set");

                /* check if mail message parameter has active flag set */
                foreach (MessageFlags o_flag in a_flags)
                {
                    if (p_e_messageFlags.Value.HasFlag(o_flag))
                    {
                        ForestNET.Lib.Global.ILogFinest("add flag '" + o_flag.ToString() + "' to mail message");

                        /* add flag to mail message */
                        o_mailMessage.AddFlag(FlagToString(o_flag));
                    }
                }
            }

            ForestNET.Lib.Global.ILogFiner("iterate all header elements");

            /* iterate all header elements */
            foreach (var o_headerLine in p_o_message.Headers)
            {
                ForestNET.Lib.Global.ILogFinest("check if header line is not empty");

                /* check if header line is not empty */
                if (o_headerLine != null)
                {
                    ForestNET.Lib.Global.ILogFinest("add header line to mail message");

                    /* add header line to mail message */
                    o_mailMessage.AddHeaderLine(o_headerLine.Field + ": " + o_headerLine.Value);

                    /* parse Received date time value */
                    if (o_headerLine.Field.Equals("Received"))
                    {
                        if (MimeKit.Utils.DateUtils.TryParse(o_headerLine.Value.Split(';').LastOrDefault()?.Trim(), out DateTimeOffset o_dateTimeOffset))
                        {
                            o_mailMessage.Received = o_dateTimeOffset.LocalDateTime;
                        }
                    }
                }
            }

            /* check if text body is set */
            if (!ForestNET.Lib.Helper.IsStringEmpty(p_o_message.TextBody))
            {
                ForestNET.Lib.Global.ILogFinest("read 'text/plain' body part");

                /* read 'text/plain' body part */
                o_mailMessage.Text = p_o_message.TextBody;
            }

            /* check if html body is set */
            if (!ForestNET.Lib.Helper.IsStringEmpty(p_o_message.HtmlBody))
            {
                ForestNET.Lib.Global.ILogFinest("read 'text/html' body part");

                /* read 'text/html' body part */
                o_mailMessage.Html = p_o_message.HtmlBody;
            }

            ForestNET.Lib.Global.ILogFiner("iterate each body part of mime message for attachments");

            /* iterate each body part of mime message for attachments */
            foreach (MimeEntity o_attachment in p_o_message.Attachments)
            {
                string s_attachmentName;
                MemoryStream o_memoryStream = new();

                if (o_attachment is MessagePart o_messagePart)
                { /* read attachment as message part */
                    s_attachmentName = o_attachment.ContentDisposition.FileName + Message.FIlENAMESEPARATOR + o_attachment.ContentType.ToString().Replace("\r\n", "").Replace("\n", "");

                    /* download attachment content */
                    if (!p_b_ignoreAttachments)
                    {
                        await o_messagePart.Message.WriteToAsync(o_memoryStream, this.Token());
                    }
                }
                else
                { /* read attachment as mime part */
                    MimePart o_mimePart = (MimePart)o_attachment;
                    s_attachmentName = o_mimePart.FileName + Message.FIlENAMESEPARATOR + o_mimePart.ContentType.ToString().Replace("\r\n", "").Replace("\n", "");

                    /* download attachment content */
                    if (!p_b_ignoreAttachments)
                    {
                        await o_mimePart.Content.DecodeToAsync(o_memoryStream, this.Token());
                    }
                }

                ForestNET.Lib.Global.ILogFinest("add attachment filename and content type '" + s_attachmentName + "' to mail message object");

                /* add attachment filename and content type to mail message object */
                o_mailMessage.AddAttachment(s_attachmentName);

                /* download attachment content */
                if (!p_b_ignoreAttachments)
                {
                    ForestNET.Lib.Global.ILogFinest("copy attachment content to byte array");

                    /* copy attachment content to byte array */
                    byte[] a_attachmentContent = o_memoryStream.ToArray();

                    ForestNET.Lib.Global.ILogFinest("add byte array to mail message object");

                    /* add byte array to mail message object */
                    o_mailMessage.AddAttachmentContent(a_attachmentContent);
                }

                ForestNET.Lib.Global.ILogFinest("close and dispose stream object");

                /* close and dispose stream object */
                o_memoryStream.Close(); o_memoryStream.Dispose();
            }

            ForestNET.Lib.Global.ILogFiner("return mail message object");

            /* return mail message object */
            return o_mailMessage;
        }

        /// <summary>
        /// Read data from input stream object instance
        /// </summary>
        /// <param name="p_o_inputStream">input stream object instance</param>
        /// <param name="p_i_bufferLength">size of buffer which is used reading the input stream</param>
        /// <param name="p_i_amountData">alternative value to inputstream.available()</param>
        /// <returns>input stream content - array of bytes</returns>
        /// <exception cref="System.IO.IOException">issue reading from input stream object instance</exception>
#pragma warning disable IDE0051 // Remove unused private members
        private async Task<byte[]> ReceiveBytes(Stream p_o_inputStream, int p_i_bufferLength, int p_i_amountData)
#pragma warning restore IDE0051 // Remove unused private members
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

            ForestNET.Lib.Global.ILogFinest("Iterate '" + i_cycles + "' cycles to receive '" + i_amountData + "' bytes with '" + p_i_bufferLength + "' bytes buffer");

            /* iterate cycle to receive bytes with buffer */
            for (int i = 0; i < i_cycles; i++)
            {
                int i_bytesReadCycle = 0;
                int i_expectedBytes = p_i_bufferLength;
                int i_bytesReading;

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
        /// Get amount of all messages from current mail folder
        /// </summary>
        /// <returns>amount of messages of current folder</returns>
        /// <exception cref="ArgumentException">current folder is Folder.ROOT where no messages are allowed</exception>
        /// <exception cref="Exception">connection failed to mail server, wrong folder path, could not open folder with read access, could not get messages, issue reading a flag of a message, issue closing folder</exception>
        public async Task<int> GetMessagesAmount()
        {
            return await this.GetMessagesAmount(null, false);
        }

        /// <summary>
        /// Get amount of messages from current mail folder
        /// </summary>
        /// <param name="p_b_unseenOnly">true - just get amount of unseen messages(IMAP only), false - get amount of all messages</param>
        /// <returns>amount of messages of current folder</returns>
        /// <exception cref="ArgumentException">current folder is Folder.ROOT where no messages are allowed</exception>
        /// <exception cref="Exception">connection failed to mail server, wrong folder path, could not open folder with read access, could not get messages, issue reading a flag of a message, issue closing folder</exception>
        public async Task<int> GetMessagesAmount(bool p_b_unseenOnly)
        {
            return await this.GetMessagesAmount(null, p_b_unseenOnly);
        }

        /// <summary>
        /// Get amount of all messages from target mail folder
        /// </summary>
        /// <param name="p_s_folderPath">target mail folder(IMAP only)</param>
        /// <returns>amount of messages of target folder</returns>
        /// <exception cref="ArgumentException">current folder is Folder.ROOT where no messages are allowed</exception>
        /// <exception cref="Exception">connection failed to mail server, wrong folder path, could not open folder with read access, could not get messages, issue reading a flag of a message, issue closing folder</exception>
        public async Task<int> GetMessagesAmount(string? p_s_folderPath)
        {
            return await this.GetMessagesAmount(p_s_folderPath, false);
        }

        /// <summary>
        /// Get amount of messages from target mail folder
        /// </summary>
        /// <param name="p_s_folderPath">target mail folder(IMAP only)</param>
        /// <param name="p_b_unseenOnly">true - just get amount of unseen messages(IMAP only), false - get amount of all messages</param>
        /// <returns>amount of messages of target folder</returns>
        /// <exception cref="ArgumentException">current folder is Folder.ROOT where no messages are allowed</exception>
        /// <exception cref="Exception">connection failed to mail server, wrong folder path, could not open folder with read access, could not get messages, issue reading a flag of a message, issue closing folder</exception>
        public async Task<int> GetMessagesAmount(string? p_s_folderPath, bool p_b_unseenOnly)
        {
            int i_return;

            /* check if mail client is set for retrieving mails */
            if ((this.ImapSession == null) && (this.Pop3Session == null))
            {
                throw new InvalidOperationException("mail client has no settings to retrieve mails from server");
            }

            ForestNET.Lib.Global.ILogFine("check if current folder is not Folder.ROOT");

            /* check if current folder is not Folder.ROOT */
            if ((this.CurrentFolder == null) || (ForestNET.Lib.Helper.IsStringEmpty(this.CurrentFolder.Name)))
            {
                throw new ArgumentException("Current folder is default folder, please specifiy a sub folder like 'inbox' or 'trash'");
            }

            try
            {
                if (!this.Pop3)
                { /* handle IMAP */
                    ForestNET.Lib.Global.ILogFine("get folders from top level");

                    /* target folder variable */
                    IMailFolder? o_targetFolder = null;

                    ForestNET.Lib.Global.ILogFine("get full path to current folder '" + this.CurrentFolder.Name + "'");

                    /* get full path to current folder */
                    string s_folderPath = this.CurrentFolder.GetFullPath();

                    /* if we have a target folder path, overwrite variable and not stay in current folder */
                    if ((p_s_folderPath != null) && (!ForestNET.Lib.Helper.IsStringEmpty(p_s_folderPath)))
                    {
                        ForestNET.Lib.Global.ILogFine("target folder path set, overwrite variable and not stay in current folder");

                        s_folderPath = p_s_folderPath;
                    }

                    ForestNET.Lib.Global.ILogFiner("enter target folder '" + s_folderPath + "'");

                    /* enter target folder */
                    o_targetFolder = await this.GetSessionAsImapClient.GetFolderAsync(s_folderPath, this.Token());

                    if (o_targetFolder == null)
                    {
                        throw new ImapCommandException(ImapCommandResponse.Bad, "Could not reach target folder '" + s_folderPath + "'");
                    }

                    ForestNET.Lib.Global.ILogFine("reached target folder, open it with read access");

                    /* reached target folder, open it with read access */
                    FolderAccess e_folderAccess = await o_targetFolder.OpenAsync(FolderAccess.ReadOnly, this.Token());

                    if (e_folderAccess != FolderAccess.ReadOnly)
                    {
                        throw new ImapCommandException(ImapCommandResponse.Bad, "Could not open folder '" + o_targetFolder.Name + "' with read access");
                    }

                    IList<IMessageSummary> a_messages = await o_targetFolder.FetchAsync(0, -1, MessageSummaryItems.Flags, this.Token());

                    /* count messages */
                    i_return = a_messages.Count;

                    /* check if we want amout of unseen messages */
                    if (p_b_unseenOnly)
                    {
                        ForestNET.Lib.Global.ILogFine("iterate each message in target folder '" + o_targetFolder.Name + "' to get amount of unseen messages");

                        /* iterate each message in target folder to get only unseen messages */
                        foreach (IMessageSummary o_message in a_messages)
                        {
                            /* if message has SEEN flag, decrease amount of target folder */
                            if ((o_message.Flags.HasValue) && (o_message.Flags.Value.HasFlag(MessageFlags.Seen)))
                            {
                                i_return--;
                            }
                        }
                    }

                    ForestNET.Lib.Global.ILogFine("close target folder");

                    /* close target folder */
                    await o_targetFolder.CloseAsync(false, this.Token());
                }
                else
                { /* handle POP3 */
                    ForestNET.Lib.Global.ILogFine("get messages count from '" + Folder.INBOX + "'");

                    i_return = this.GetSessionAsPop3Client.Count;
                }
            }
            catch (Exception o_exc)
            {
                throw new Exception("Exception while get amount of messages from '" + (p_s_folderPath ?? "INBOX") + "'; " + o_exc.Message);
            }

            return i_return;
        }

        /// <summary>
        /// Get message from current mail folder with unique message id, set seen flag to message(s) matching message id, download all attachments from message, can be stored later to local system
        /// pop3 protocol is not supported
        /// </summary>
        /// <param name="p_s_messageId">unique identifier for mail message</param>
        /// <returns>mail message object</returns>
        /// <exception cref="ArgumentException">current folder is Folder.ROOT where no messages are allowed, could not find message with id</exception>
        /// <exception cref="Exception">connection failed to mail server, wrong folder path, could not open folder with read write access, could not get messages, could not wrap message, issue set/unset a flag of a message, issue closing folder</exception>
        /// <exception cref="System.IO.IOException">issue handling mime content or bodypart content, or could not retrieve attachment content to byte array output stream</exception>
        public async Task<Message?> GetMessageById(string p_s_messageId)
        {
            return await this.GetMessageById(p_s_messageId, false);
        }

        /// <summary>
        /// Get message from current mail folder with unique message id, set seen flag to message(s) matching message id
        /// pop3 protocol is not supported
        /// </summary>
        /// <param name="p_s_messageId">unique identifier for mail message</param>
        /// <param name="p_b_ignoreAttachments">true - ignore attachments of message, false - download all attachments from message, can be stored later to local system</param>
        /// <returns>mail message object</returns>
        /// <exception cref="ArgumentException">current folder is Folder.ROOT where no messages are allowed, could not find message with id</exception>
        /// <exception cref="Exception">connection failed to mail server, wrong folder path, could not open folder with read write access, could not get messages, could not wrap message, issue set/unset a flag of a message, issue closing folder</exception>
        /// <exception cref="System.IO.IOException">issue handling mime content or bodypart content, or could not retrieve attachment content to byte array output stream</exception>
        public async Task<Message?> GetMessageById(string p_s_messageId, bool p_b_ignoreAttachments)
        {
            return await this.GetMessageById(null, p_s_messageId, p_b_ignoreAttachments, true);
        }

        /// <summary>
        /// Get message from target mail folder with unique message id
        /// pop3 protocol is not supported
        /// </summary>
        /// <param name="p_s_folderPath">target mail folder where we want to read all messages from</param>
        /// <param name="p_s_messageId">unique identifier for mail message</param>
        /// <param name="p_b_ignoreAttachments">true - ignore attachments of message, false - download all attachments from message, can be stored later to local system</param>
        /// <param name="p_b_setSeen">true - set seen flag to message(s) matching message id, false - unset seen flag if reading message auto set seen flag for message</param>
        /// <returns>mail message object</returns>
        /// <exception cref="ArgumentException">current folder is Folder.ROOT where no messages are allowed, could not find message with id</exception>
        /// <exception cref="Exception">connection failed to mail server, wrong folder path, could not open folder with read write access, could not get messages, could not wrap message, issue set/unset a flag of a message, issue closing folder</exception>
        /// <exception cref="System.IO.IOException">issue handling mime content or bodypart content, or could not retrieve attachment content to byte array output stream</exception>
        public async Task<Message?> GetMessageById(string? p_s_folderPath, string p_s_messageId, bool p_b_ignoreAttachments, bool p_b_setSeen)
        {
            /* return value */
            Message? o_mailMessage = null;

            /* POP3 is not supported */
            if (this.Pop3)
            {
                throw new ArgumentException("POP3(s) protocol is not supported");
            }

            /* check if mail client is set for retrieving mails */
            if (this.ImapSession == null)
            {
                throw new InvalidOperationException("mail client has no settings to retrieve mails from server");
            }

            ForestNET.Lib.Global.ILogFine("check if current folder is not Folder.ROOT");

            /* check if current folder is not Folder.ROOT */
            if ((this.CurrentFolder == null) || (ForestNET.Lib.Helper.IsStringEmpty(this.CurrentFolder.Name)))
            {
                throw new ArgumentException("Current folder is default folder, please specifiy a sub folder like 'inbox' or 'trash'");
            }

            try
            {
                ForestNET.Lib.Global.ILogFine("get folders from top level");

                /* target folder variable */
                IMailFolder? o_targetFolder = null;

                ForestNET.Lib.Global.ILogFine("get full path to current folder '" + this.CurrentFolder.Name + "'");

                /* get full path to current folder */
                string s_folderPath = this.CurrentFolder.GetFullPath();

                /* if we have a target folder path, overwrite variable and not stay in current folder */
                if ((p_s_folderPath != null) && (!ForestNET.Lib.Helper.IsStringEmpty(p_s_folderPath)))
                {
                    ForestNET.Lib.Global.ILogFine("target folder path set, overwrite variable and not stay in current folder");

                    s_folderPath = p_s_folderPath;
                }

                ForestNET.Lib.Global.ILogFine("enter target folder '" + s_folderPath + "'");

                /* enter target folder */
                o_targetFolder = await this.GetSessionAsImapClient.GetFolderAsync(s_folderPath, this.Token());

                if (o_targetFolder == null)
                {
                    throw new ImapCommandException(ImapCommandResponse.Bad, "Could not reach target folder '" + s_folderPath + "'");
                }

                /* usually open folder in read mode */
                FolderAccess e_usingFolderAccess = FolderAccess.ReadOnly;

                /* if we want to set flags, we need read write access */
                if (p_b_setSeen)
                {
                    e_usingFolderAccess = FolderAccess.ReadWrite;
                }

                ForestNET.Lib.Global.ILogFine("reached target folder, open it with read or write access, depending on parameter");

                /* reached target folder, open it with read or write access, depending on parameter */
                FolderAccess e_folderAccess = await o_targetFolder.OpenAsync(e_usingFolderAccess, this.Token());

                if (e_folderAccess != e_usingFolderAccess)
                {
                    throw new ImapCommandException(ImapCommandResponse.Bad, "Could not open folder '" + o_targetFolder.Name + "' with read or write access, depending on parameter");
                }

                ForestNET.Lib.Global.ILogFine("get messages with message-id or part of message-id '" + p_s_messageId + "' from target folder");

                /* get messages with message-id or part of message-id from target folder */
                IList<UniqueId> a_messages = await o_targetFolder.SearchAsync(MailKit.Search.SearchQuery.HeaderContains("Message-ID", p_s_messageId), this.Token());
                UniqueId? o_foundMessageUniqueId = null;

                /* check if we have no messages in target folder */
                if (a_messages.Count < 1)
                {
                    ForestNET.Lib.Global.ILogFine("no message found in target folder '" + o_targetFolder.Name + "' with message-id '" + p_s_messageId + "'");
                }
                else if (a_messages.Count > 1)
                {
                    ForestNET.Lib.Global.ILogFine("multiple messages found [" + a_messages.Count + "] in target folder '" + o_targetFolder.Name + "' with message-id '" + p_s_messageId + "'");
                    ForestNET.Lib.Global.ILogFine("take first found message in result list with UniqueId [" + a_messages[0] + "]");

                    /* multiple messages found, take first found message in result list */
                    o_foundMessageUniqueId = a_messages[0];
                }
                else
                {
                    ForestNET.Lib.Global.ILogFine("use UniqueId [" + a_messages[0] + "] from message with message-id '" + p_s_messageId + "'");

                    /* use UniqueId from found message with message-id */
                    o_foundMessageUniqueId = a_messages[0];
                }

                /* check if we found a UniqueId with message-id */
                if (o_foundMessageUniqueId != null)
                {
                    ForestNET.Lib.Global.ILogFiner("get message summary for flags");

                    /* get message summary for flags */
                    IList<IMessageSummary> a_messageSummary = await o_targetFolder.FetchAsync(new UniqueId[] { (UniqueId)o_foundMessageUniqueId }, MessageSummaryItems.Flags, this.Token());

                    ForestNET.Lib.Global.ILogFiner("get message with wrap method");

                    /* get message with wrap method */
                    o_mailMessage = await this.WrapMessage(await o_targetFolder.GetMessageAsync((UniqueId)o_foundMessageUniqueId, this.Token(), null), a_messageSummary[0].Flags, p_b_ignoreAttachments);

                    if (p_b_setSeen)
                    { /* set seen flag if parameter is set */
                        ForestNET.Lib.Global.ILogFiner("set seen flag if parameter is set");

                        await o_targetFolder.AddFlagsAsync((UniqueId)o_foundMessageUniqueId, MessageFlags.Seen, true, this.Token());
                    }
                }

                ForestNET.Lib.Global.ILogFine("close target folder, expunge all deleted messages if parameter is set");

                /* close target folder, expunge all deleted messages if parameter is set */
                await o_targetFolder.CloseAsync(false, this.Token());
            }
            catch (Exception o_exc)
            {
                throw new Exception("Exception while get message from '" + (p_s_folderPath ?? "INBOX") + "' with message-id '" + p_s_messageId + "'; " + o_exc.Message);
            }

            /* return mail message object */
            return o_mailMessage;
        }

        /// <summary>
        /// Move all mail messages from current folder to a target folder
        /// </summary>
        /// <param name="p_s_targetFolderPath">target mail folder where we want to move all messages to</param>
        /// <exception cref="ArgumentException">current folder or target folder is Folder.ROOT where no messages are allowed, invalid parameter for message id list, could not find any messages</exception>
        /// <exception cref="Exception">connection failed to mail server, wrong folder path, could not open folder with read write access, could not get messages, issue closing folder</exception>
        public async Task MoveAllMessages(string p_s_targetFolderPath)
        {
            await this.MoveMessages(null, p_s_targetFolderPath);
        }

        /// <summary>
        /// Move all mail messages from current folder to a target folder
        /// </summary>
        /// <param name="p_s_targetFolderPath">target mail folder where we want to move all messages to</param>
        /// <param name="p_s_flag">flag parameter sets flag for all messages in search result</param>
        /// <param name="p_b_state">state value for flag parameter (true or false)</param>
        /// <exception cref="ArgumentException">current folder or target folder is Folder.ROOT where no messages are allowed, invalid parameter for message id list, could not find any messages</exception>
        /// <exception cref="Exception">connection failed to mail server, wrong folder path, could not open folder with read write access, could not get messages, issue set/unset a flag of a message, issue closing folder</exception>
        public async Task MoveAllMessages(string p_s_targetFolderPath, string? p_s_flag, bool p_b_state)
        {
            await this.MoveMessages(null, p_s_targetFolderPath, p_s_flag, p_b_state);
        }

        /// <summary>
        /// Move mail messages from current folder to a target folder
        /// </summary>
        /// <param name="p_a_messageIds">filter by message id's in a list parameter value</param>
        /// <param name="p_s_targetFolderPath">target mail folder where we want to move all messages to</param>
        /// <exception cref="ArgumentException">current folder or target folder is Folder.ROOT where no messages are allowed, invalid parameter for message id list, could not find any messages</exception>
        /// <exception cref="Exception">connection failed to mail server, wrong folder path, could not open folder with read write access, could not get messages, issue closing folder</exception>
        public async Task MoveMessages(List<string>? p_a_messageIds, string p_s_targetFolderPath)
        {
            await this.MoveMessages(p_a_messageIds, p_s_targetFolderPath, null, false);
        }

        /// <summary>
        /// Move mail messages from current folder to a target folder
        /// </summary>
        /// <param name="p_a_messageIds">filter by message id's in a list parameter value</param>
        /// <param name="p_s_targetFolderPath">target mail folder where we want to move messages to</param>
        /// <param name="p_s_flag">flag parameter sets flag for all messages in search result</param>
        /// <param name="p_b_state">state value for flag parameter (true or false)</param>
        /// <exception cref="ArgumentException">current folder or target folder is Folder.ROOT where no messages are allowed, invalid parameter for message id list, could not find any messages</exception>
        /// <exception cref="Exception">connection failed to mail server, wrong folder path, could not open folder with read write access, could not get messages, issue set/unset a flag of a message, issue closing folder</exception>
        public async Task MoveMessages(List<string>? p_a_messageIds, string p_s_targetFolderPath, string? p_s_flag, bool p_b_state)
        {
            /* POP3 is not supported */
            if (this.Pop3)
            {
                throw new ArgumentException("POP3(s) protocol is not supported");
            }

            /* check if mail client is set for retrieving mails */
            if (this.ImapSession == null)
            {
                throw new InvalidOperationException("mail client has no settings to retrieve mails from server");
            }

            ForestNET.Lib.Global.ILogFine("check if current folder is not Folder.ROOT");

            /* check if current folder is not Folder.ROOT */
            if ((this.CurrentFolder == null) || (ForestNET.Lib.Helper.IsStringEmpty(this.CurrentFolder.Name)))
            {
                throw new ArgumentException("Current folder is default folder, please specifiy a sub folder like 'inbox' or 'trash'");
            }

            /* check if list for message id's is not empty */
            if ((p_a_messageIds != null) && (p_a_messageIds.Count < 1))
            {
                throw new ArgumentException("Empty message id list parameter");
            }

            ForestNET.Lib.Global.ILogFine("check if target folder is not Folder.ROOT");

            /* check if target folder is not Folder.ROOT */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_targetFolderPath))
            {
                throw new ArgumentException("Empty target folder path parameter");
            }

            /* target folder variable */
            IMailFolder? o_targetFolder;

            ForestNET.Lib.Global.ILogFine("enter target folder '" + p_s_targetFolderPath + "'");

            /* enter target folder */
            o_targetFolder = await this.GetSessionAsImapClient.GetFolderAsync(p_s_targetFolderPath, this.Token());

            if (o_targetFolder == null)
            {
                throw new ImapCommandException(ImapCommandResponse.Bad, "Could not reach target folder '" + p_s_targetFolderPath + "'");
            }

            /* current folder variable */
            IMailFolder? o_folder;

            ForestNET.Lib.Global.ILogFine("enter current folder '" + this.CurrentFolder.GetFullPath() + "'");

            /* enter current folder */
            o_folder = await this.GetSessionAsImapClient.GetFolderAsync(this.CurrentFolder.GetFullPath(), this.Token());

            if (o_folder == null)
            {
                throw new ImapCommandException(ImapCommandResponse.Bad, "Could not reach current folder '" + this.CurrentFolder.GetFullPath() + "'");
            }

            ForestNET.Lib.Global.ILogFine("reached current folder, open it with read write access");

            /* reached current folder, open it with read write access */
            FolderAccess e_currentFolderAccess = await o_folder.OpenAsync(FolderAccess.ReadWrite, this.Token());

            if (e_currentFolderAccess != FolderAccess.ReadWrite)
            {
                throw new ImapCommandException(ImapCommandResponse.Bad, "Could not open folder '" + o_folder.Name + "' with read write access");
            }

            /* temporary list value to store all messages */
            IList<IMessageSummary>? a_messages;

            if (p_a_messageIds != null)
            { /* use message id list to filter messages in current folder */
                ForestNET.Lib.Global.ILogFine("use message id list to filter messages in current folder");

                /* search query variable */
                MailKit.Search.SearchQuery? o_searchQuery = null;

                ForestNET.Lib.Global.ILogFine("create search query with message id list parameter size");

                /* create search query with message id list parameter size */
                foreach (string s_messageId in p_a_messageIds)
                {
                    if (o_searchQuery == null)
                    {
                        o_searchQuery = MailKit.Search.SearchQuery.HeaderContains("Message-ID", s_messageId);
                    }
                    else
                    {
                        o_searchQuery = o_searchQuery.Or(MailKit.Search.SearchQuery.HeaderContains("Message-ID", s_messageId));
                    }
                }

                ForestNET.Lib.Global.ILogFine("look for messages, get an array of messages back");

                /* look for messages, get an array of unique-id's for current folder back */
                IList<UniqueId> a_searchResult = await o_folder.SearchAsync(o_searchQuery, this.Token());

                ForestNET.Lib.Global.ILogFine("get search result messages from current folder");

                /* get search result messages from current folder */
                a_messages = await o_folder.FetchAsync(a_searchResult, MessageSummaryItems.UniqueId, this.Token());
            }
            else
            { /* get all message from current folder */
                ForestNET.Lib.Global.ILogFine("get all message from current folder");

                a_messages = await o_folder.FetchAsync(0, -1, MessageSummaryItems.UniqueId, this.Token());
            }

            if ((a_messages == null) || (a_messages.Count < 1))
            { /* message result is empty */
                throw new ArgumentException("Could not find any mail messages");
            }
            else
            {
                ForestNET.Lib.Global.ILogFine("move messages from current folder to target folder");

                /* move messages from current folder to target folder */
                foreach (IMessageSummary o_message in a_messages)
                {
                    /* if flag parameter is set, set/unset this flag for all messages in search result */
                    if (p_s_flag != null)
                    {
                        ForestNET.Lib.Global.ILogFine("flag parameter is set, set/unset this flag for all messages in search result");

                        if (p_b_state)
                        {
                            /* set flag of mail message */
                            await o_folder.AddFlagsAsync(o_message.UniqueId, StringToFlag(p_s_flag), true, this.Token());
                        }
                        else
                        {
                            /* unset flag of mail message */
                            await o_folder.RemoveFlagsAsync(o_message.UniqueId, StringToFlag(p_s_flag), true, this.Token());
                        }
                    }

                    UniqueId? o_movedUniqueId = await o_folder.MoveToAsync(o_message.UniqueId, o_targetFolder, this.Token());

                    ForestNET.Lib.Global.ILogFine("moved message with unique-id[" + o_message.UniqueId + "] to target folder; new unique-id[" + (o_movedUniqueId.HasValue ? o_movedUniqueId : "none") + "]");
                }
            }

            ForestNET.Lib.Global.ILogFine("close current folder, expunge all deleted messages");

            /* close current folder, expunge all deleted messages */
            await o_folder.CloseAsync(true, this.Token());
        }

        /// <summary>
        /// Set or unset flag value of a mail message found by message id in current mail folder
        /// </summary>
        /// <param name="p_s_messageId">unique identifier for mail message</param>
        /// <param name="p_s_flag">flag parameter sets flag for all messages in search result</param>
        /// <param name="p_b_state">state value for flag parameter (true or false)</param>
        /// <exception cref="ArgumentException">current folder or target folder is Folder.ROOT where no messages are allowed, invalid parameter for message id list, could not find any messages</exception>
        /// <exception cref="Exception">connection failed to mail server, wrong folder path, could not open folder with read write access, could not get messages, issue set/unset a flag of a message, issue closing folder</exception>
        private async Task SetFlag(string p_s_messageId, string p_s_flag, bool p_b_state)
        {
            /* POP3 is not supported */
            if (this.Pop3)
            {
                throw new ArgumentException("POP3(s) protocol is not supported");
            }

            /* check if mail client is set for retrieving mails */
            if (this.ImapSession == null)
            {
                throw new InvalidOperationException("mail client has no settings to retrieve mails from server");
            }

            ForestNET.Lib.Global.ILogFine("check if current folder is not Folder.ROOT");

            /* check if current folder is not Folder.ROOT */
            if ((this.CurrentFolder == null) || (ForestNET.Lib.Helper.IsStringEmpty(this.CurrentFolder.Name)))
            {
                throw new ArgumentException("Current folder is default folder, please specifiy a sub folder like 'inbox' or 'trash'");
            }

            /* current folder variable */
            IMailFolder? o_folder;

            ForestNET.Lib.Global.ILogFine("enter current folder '" + this.CurrentFolder.GetFullPath() + "'");

            /* enter current folder */
            o_folder = await this.GetSessionAsImapClient.GetFolderAsync(this.CurrentFolder.GetFullPath(), this.Token());

            if (o_folder == null)
            {
                throw new ImapCommandException(ImapCommandResponse.Bad, "Could not reach current folder '" + this.CurrentFolder.GetFullPath() + "'");
            }

            ForestNET.Lib.Global.ILogFine("reached current folder, open it with read write access");

            /* reached current folder, open it with read write access */
            FolderAccess e_currentFolderAccess = await o_folder.OpenAsync(FolderAccess.ReadWrite, this.Token());

            if (e_currentFolderAccess != FolderAccess.ReadWrite)
            {
                throw new ImapCommandException(ImapCommandResponse.Bad, "Could not open folder '" + o_folder.Name + "' with read write access");
            }

            /* temporary list value to store all messages */
            IList<IMessageSummary>? a_messages;

            ForestNET.Lib.Global.ILogFine("look for message, get an array of messages back");

            /* look for message, get an array of unique-id's for current folder back */
            IList<UniqueId> a_searchResult = await o_folder.SearchAsync(MailKit.Search.SearchQuery.HeaderContains("Message-ID", p_s_messageId), this.Token());

            ForestNET.Lib.Global.ILogFine("get search result messages from current folder");

            /* get search result messages from current folder */
            a_messages = await o_folder.FetchAsync(a_searchResult, MessageSummaryItems.UniqueId, this.Token());

            if ((a_messages == null) || (a_messages.Count < 1))
            { /* message result is empty */
                throw new ArgumentException("Could not find any mail messages");
            }
            else
            {
                ForestNET.Lib.Global.ILogFine("set/unset flag parameter for all messages in search result");

                /* set/unset flag parameter for all messages in search result */
                foreach (IMessageSummary o_message in a_messages)
                {
                    if (p_b_state)
                    {
                        /* set flag of mail message */
                        await o_folder.AddFlagsAsync(o_message.UniqueId, StringToFlag(p_s_flag), true, this.Token());
                    }
                    else
                    {
                        /* unset flag of mail message */
                        await o_folder.RemoveFlagsAsync(o_message.UniqueId, StringToFlag(p_s_flag), true, this.Token());
                    }
                }
            }

            ForestNET.Lib.Global.ILogFine("close current folder, expunge all deleted messages");

            /* close current folder, expunge all deleted messages */
            await o_folder.CloseAsync(true, this.Token());
        }

        /// <summary>
        /// Set seen flag value of a mail message found by message id in current mail folder
        /// </summary>
        /// <param name="p_s_messageId">unique identifier for mail message</param>
        /// <exception cref="ArgumentException">current folder or target folder is Folder.ROOT where no messages are allowed, invalid parameter for message id list, could not find any messages</exception>
        /// <exception cref="Exception">connection failed to mail server, wrong folder path, could not open folder with read write access, could not get messages, issue set/unset a flag of a message, issue closing folder</exception>
        public async Task SetSeen(string p_s_messageId)
        {
            await this.SetFlag(p_s_messageId, FlagToString(MessageFlags.Seen), true);
        }

        /// <summary>
        /// Unset seen flag value of a mail message found by message id in current mail folder
        /// </summary>
        /// <param name="p_s_messageId">unique identifier for mail message</param>
        /// <exception cref="ArgumentException">current folder or target folder is Folder.ROOT where no messages are allowed, invalid parameter for message id list, could not find any messages</exception>
        /// <exception cref="Exception">connection failed to mail server, wrong folder path, could not open folder with read write access, could not get messages, issue set/unset a flag of a message, issue closing folder</exception>
        public async Task UnsetSeen(string p_s_messageId)
        {
            await this.SetFlag(p_s_messageId, FlagToString(MessageFlags.Seen), false);
        }

        /// <summary>
        /// Set flagged flag value of a mail message found by message id in current mail folder
        /// </summary>
        /// <param name="p_s_messageId">unique identifier for mail message</param>
        /// <exception cref="ArgumentException">current folder or target folder is Folder.ROOT where no messages are allowed, invalid parameter for message id list, could not find any messages</exception>
        /// <exception cref="Exception">connection failed to mail server, wrong folder path, could not open folder with read write access, could not get messages, issue set/unset a flag of a message, issue closing folder</exception>
        public async Task SetFlagged(string p_s_messageId)
        {
            await this.SetFlag(p_s_messageId, FlagToString(MessageFlags.Flagged), true);
        }

        /// <summary>
        /// Unset flagged flag value of a mail message found by message id in current mail folder
        /// </summary>
        /// <param name="p_s_messageId">unique identifier for mail message</param>
        /// <exception cref="ArgumentException">current folder or target folder is Folder.ROOT where no messages are allowed, invalid parameter for message id list, could not find any messages</exception>
        /// <exception cref="Exception">connection failed to mail server, wrong folder path, could not open folder with read write access, could not get messages, issue set/unset a flag of a message, issue closing folder</exception>
        public async Task UnsetFlagged(string p_s_messageId)
        {
            await this.SetFlag(p_s_messageId, FlagToString(MessageFlags.Flagged), false);
        }

        /// <summary>
        /// Set answered flag value of a mail message found by message id in current mail folder
        /// </summary>
        /// <param name="p_s_messageId">unique identifier for mail message</param>
        /// <exception cref="ArgumentException">current folder or target folder is Folder.ROOT where no messages are allowed, invalid parameter for message id list, could not find any messages</exception>
        /// <exception cref="Exception">connection failed to mail server, wrong folder path, could not open folder with read write access, could not get messages, issue set/unset a flag of a message, issue closing folder</exception>
        public async Task SetAnswered(string p_s_messageId)
        {
            await this.SetFlag(p_s_messageId, FlagToString(MessageFlags.Answered), true);
        }

        /// <summary>
        /// Unset answered flag value of a mail message found by message id in current mail folder
        /// </summary>
        /// <param name="p_s_messageId">unique identifier for mail message</param>
        /// <exception cref="ArgumentException">current folder or target folder is Folder.ROOT where no messages are allowed, invalid parameter for message id list, could not find any messages</exception>
        /// <exception cref="Exception">connection failed to mail server, wrong folder path, could not open folder with read write access, could not get messages, issue set/unset a flag of a message, issue closing folder</exception>
        public async Task UnsetAnswered(string p_s_messageId)
        {
            await this.SetFlag(p_s_messageId, FlagToString(MessageFlags.Answered), false);
        }

        /// <summary>
        /// Set delete flag value of a mail message found by message id in current mail folder
        /// </summary>
        /// <param name="p_s_messageId">unique identifier for mail message</param>
        /// <exception cref="ArgumentException">current folder or target folder is Folder.ROOT where no messages are allowed, invalid parameter for message id list, could not find any messages</exception>
        /// <exception cref="Exception">connection failed to mail server, wrong folder path, could not open folder with read write access, could not get messages, issue set/unset a flag of a message, issue closing folder</exception>
        public async Task SetDelete(string p_s_messageId)
        {
            await this.SetFlag(p_s_messageId, FlagToString(MessageFlags.Deleted), true);
        }

        /// <summary>
        /// Unset delete flag value of a mail message found by message id in current mail folder
        /// </summary>
        /// <param name="p_s_messageId">unique identifier for mail message</param>
        /// <exception cref="ArgumentException">current folder or target folder is Folder.ROOT where no messages are allowed, invalid parameter for message id list, could not find any messages</exception>
        /// <exception cref="Exception">connection failed to mail server, wrong folder path, could not open folder with read write access, could not get messages, issue set/unset a flag of a message, issue closing folder</exception>
        public async Task UnsetDelete(string p_s_messageId)
        {
            await this.SetFlag(p_s_messageId, FlagToString(MessageFlags.Deleted), false);
        }

        /// <summary>
        /// Set recent flag value of a mail message found by message id in current mail folder
        /// </summary>
        /// <param name="p_s_messageId">unique identifier for mail message</param>
        /// <exception cref="ArgumentException">current folder or target folder is Folder.ROOT where no messages are allowed, invalid parameter for message id list, could not find any messages</exception>
        /// <exception cref="Exception">connection failed to mail server, wrong folder path, could not open folder with read write access, could not get messages, issue set/unset a flag of a message, issue closing folder</exception>
        public async Task SetRecent(string p_s_messageId)
        {
            await this.SetFlag(p_s_messageId, FlagToString(MessageFlags.Recent), true);
        }

        /// <summary>
        /// Unset recent flag value of a mail message found by message id in current mail folder
        /// </summary>
        /// <param name="p_s_messageId">unique identifier for mail message</param>
        /// <exception cref="ArgumentException">current folder or target folder is Folder.ROOT where no messages are allowed, invalid parameter for message id list, could not find any messages</exception>
        /// <exception cref="Exception">connection failed to mail server, wrong folder path, could not open folder with read write access, could not get messages, issue set/unset a flag of a message, issue closing folder</exception>
        public async Task UnsetRecent(string p_s_messageId)
        {
            await this.SetFlag(p_s_messageId, FlagToString(MessageFlags.Recent), false);
        }

        /// <summary>
        /// Delete/Expunge all messages in current mail folder
        /// </summary>
        /// <exception cref="ArgumentException">current folder or target folder is Folder.ROOT where no messages are allowed, invalid parameter for message id list, could not find any messages</exception>
        /// <exception cref="Exception">connection failed to mail server, wrong folder path, could not open folder with read write access, could not get messages, issue set/unset a flag of a message, issue closing folder</exception>
        public async Task ExpungeFolder()
        {
            await this.ExpungeFolder(null);
        }

        /// <summary>
        /// Delete/Expunge all messages in target mail folder
        /// </summary>
        /// <param name="p_s_folderPath">target mail folder where we want to delete all messages</param>
        /// <exception cref="ArgumentException">current folder or target folder is Folder.ROOT where no messages are allowed, invalid parameter for message id list, could not find any messages</exception>
        /// <exception cref="Exception">connection failed to mail server, wrong folder path, could not open folder with read write access, could not get messages, issue set/unset a flag of a message, issue closing folder</exception>
        public async Task ExpungeFolder(string? p_s_folderPath)
        {
            /* POP3 is not supported */
            if (this.Pop3)
            {
                throw new ArgumentException("POP3(s) protocol is not supported");
            }

            /* check if mail client is set for retrieving mails */
            if (this.ImapSession == null)
            {
                throw new InvalidOperationException("mail client has no settings to retrieve mails from server");
            }

            ForestNET.Lib.Global.ILogFine("check if current folder is not Folder.ROOT");

            /* check if current folder is not Folder.ROOT */
            if ((this.CurrentFolder == null) || (ForestNET.Lib.Helper.IsStringEmpty(this.CurrentFolder.Name)))
            {
                throw new ArgumentException("Current folder is default folder, please specifiy a sub folder like 'inbox' or 'trash'");
            }

            /* target folder variable */
            IMailFolder? o_targetFolder;

            ForestNET.Lib.Global.ILogFine("get full path to current folder '" + this.CurrentFolder.Name + "'");

            /* get full path to current folder */
            string s_folderPath = this.CurrentFolder.GetFullPath();

            /* if we have a target folder path, overwrite variable and not stay in current folder */
            if ((p_s_folderPath != null) && (!ForestNET.Lib.Helper.IsStringEmpty(p_s_folderPath)))
            {
                ForestNET.Lib.Global.ILogFine("target folder path set, overwrite variable and not stay in current folder");

                s_folderPath = p_s_folderPath;
            }

            ForestNET.Lib.Global.ILogFine("enter target folder '" + s_folderPath + "'");

            /* enter target folder */
            o_targetFolder = await this.GetSessionAsImapClient.GetFolderAsync(s_folderPath, this.Token());

            if (o_targetFolder == null)
            {
                throw new ImapCommandException(ImapCommandResponse.Bad, "Could not reach target folder '" + s_folderPath + "'");
            }

            ForestNET.Lib.Global.ILogFine("reached target folder, open it with read write access");

            /* reached target folder, open it with read write access */
            FolderAccess e_targetFolderAccess = await o_targetFolder.OpenAsync(FolderAccess.ReadWrite, this.Token());

            if (e_targetFolderAccess != FolderAccess.ReadWrite)
            {
                throw new ImapCommandException(ImapCommandResponse.Bad, "Could not open folder '" + o_targetFolder.Name + "' with read write access");
            }

            ForestNET.Lib.Global.ILogFine("get all messages from target folder");

            /* get search result messages from target folder */
            IList<IMessageSummary> a_messages = await o_targetFolder.FetchAsync(0, -1, MessageSummaryItems.UniqueId, this.Token());

            if ((a_messages == null) || (a_messages.Count < 1))
            { /* message result is empty */
                throw new ArgumentException("Could not find any mail messages");
            }
            else
            {
                ForestNET.Lib.Global.ILogFine("set deleted flag parameter for all messages in target folder");

                /* set deleted flag parameter for all messages in target folder */
                foreach (IMessageSummary o_message in a_messages)
                {
                    /* set deleted flag async */
                    await o_targetFolder.AddFlagsAsync(o_message.UniqueId, MessageFlags.Deleted, true, this.Token());
                }
            }

            ForestNET.Lib.Global.ILogFine("close target folder, expunge all deleted messages");

            /* close target folder, expunge all deleted messages */
            await o_targetFolder.CloseAsync(true, this.Token());
        }

        /// <summary>
        /// Change current mail folder to another mail folder, parent or sub folder
        /// </summary>
        /// <param name="p_s_folderName">folder name of a sub folder, or '..' for parent folder</param>
        /// <exception cref="ArgumentException">invalid target folder parameter or sub folder does not exist</exception>
        /// <exception cref="Exception">wrong folder path</exception>
        public async Task ChangeToFolder(string p_s_folderName)
        {
            /* POP3 is not supported */
            if (this.Pop3)
            {
                throw new ArgumentException("POP3(s) protocol is not supported");
            }

            /* check if mail client is set for retrieving mails */
            if (this.ImapSession == null)
            {
                throw new InvalidOperationException("mail client has no settings to retrieve mails from server");
            }

            ForestNET.Lib.Global.ILogFine("check if target folder parameter does not contain a '/' character");

            /* check if target folder parameter does not contain a '/' character */
            if (p_s_folderName.Contains('/'))
            {
                throw new ArgumentException("Invalid character '/' in parameter '" + p_s_folderName + "'");
            }

            if (p_s_folderName.Equals(".."))
            { /* go to parent mail folder */
                ForestNET.Lib.Global.ILogFine("go to parent mail folder");

                ForestNET.Lib.Global.ILogFine("check if we are not already at Folder.ROOT level");

                /* check if we are not already at Folder.ROOT level */
                if ((this.CurrentFolder != null) && (this.CurrentFolder.Parent != null))
                {
                    ForestNET.Lib.Global.ILogFine("change current folder to parent folder");

                    /* change current folder to parent folder */
                    this.CurrentFolder = this.CurrentFolder.Parent;
                }
            }
            else
            { /* go to a mail sub folder */
                ForestNET.Lib.Global.ILogFine("go to a mail sub folder");

                ForestNET.Lib.Global.ILogFine("get sub folder object");

                /* get sub folder object */
                Folder? o_foo = this.CurrentFolder?.GetSubFolder(p_s_folderName);

                /* check if sub folder really exists with target folder parameter */
                if (o_foo == null)
                {
                    ForestNET.Lib.Global.ILogFine("sub folder does not exist with target folder parameter");

                    throw new ArgumentException("Sub folder '" + p_s_folderName + "' does not exist under '" + ((this.CurrentFolder == null || ForestNET.Lib.Helper.IsStringEmpty(this.CurrentFolder.Name)) ? "/" : this.CurrentFolder.Name) + "'");
                }

                ForestNET.Lib.Global.ILogFine("change current folder to sub folder");

                /* change current folder to sub folder */
                this.CurrentFolder = o_foo;
            }

            /* check if current folder is null */
            if (this.CurrentFolder == null)
            {
                throw new Exception("current folder is null; gone wrong sub folder '" + p_s_folderName + "'");
            }

            /* current folder variable */
            IMailFolder? o_folder;

            ForestNET.Lib.Global.ILogFine("enter current folder '" + this.CurrentFolder.GetFullPath() + "'");

            /* enter current folder */
            o_folder = await this.GetSessionAsImapClient.GetFolderAsync(this.CurrentFolder.GetFullPath(), this.Token());

            if (o_folder == null)
            {
                throw new ImapCommandException(ImapCommandResponse.Bad, "Could not reach current folder '" + this.CurrentFolder.GetFullPath() + "'");
            }

            ForestNET.Lib.Global.ILogFine("get all sub folder of current folder and add them as child items");

            /* get all sub folder of current folder and add them as child items */
            IList<IMailFolder> a_subFolders = await o_folder.GetSubfoldersAsync(false, this.Token());

            foreach (IMailFolder o_subFolder in a_subFolders)
            {
                ForestNET.Lib.Global.ILogFine("add folder '" + o_subFolder.Name + "' as sub folder to current folder");

                this.CurrentFolder.AddChildren(o_subFolder.Name);
            }
        }

        /// <summary>
        /// Creates a sub folder at current mail folder
        /// </summary>
        /// <param name="p_s_folderName">folder name of a sub folder</param>
        /// <exception cref="ArgumentException">invalid target folder parameter or sub folder already exists</exception>
        /// <exception cref="Exception">connection failed to mail server, or wrong folder path or issue creating and subscribing sub folder</exception>
        public async Task CreateSubFolder(string p_s_folderName)
        {
            /* POP3 is not supported */
            if (this.Pop3)
            {
                throw new ArgumentException("POP3(s) protocol is not supported");
            }

            /* check if mail client is set for retrieving mails */
            if (this.ImapSession == null)
            {
                throw new InvalidOperationException("mail client has no settings to retrieve mails from server");
            }

            ForestNET.Lib.Global.ILogFine("check if current folder is not Folder.ROOT");

            /* check if current folder is not Folder.ROOT */
            if ((this.CurrentFolder == null) || (ForestNET.Lib.Helper.IsStringEmpty(this.CurrentFolder.Name)))
            {
                throw new ArgumentException("Current folder is default folder, please specifiy a sub folder like 'inbox' or 'trash'");
            }

            ForestNET.Lib.Global.ILogFine("check if target folder parameter does not contain a '/' character");

            /* check if target folder parameter does not contain a '/' character */
            if (p_s_folderName.Contains('/'))
            {
                throw new ArgumentException("Invalid character '/' in parameter '" + p_s_folderName + "'");
            }

            ForestNET.Lib.Global.ILogFine("check if target folder parameter is not equal '.' or '..'");

            /* check if target folder parameter is not equal '.' or '..' */
            if ((p_s_folderName.Equals(".")) || (p_s_folderName.Equals("..")))
            {
                throw new ArgumentException("Invalid sub folder name '" + p_s_folderName + "'");
            }

            ForestNET.Lib.Global.ILogFine("check if sub folder already exists");

            /* check if sub folder already exists */
            if (this.CurrentFolder?.GetSubFolder(p_s_folderName) != null)
            {
                throw new ArgumentException("Sub folder '" + p_s_folderName + "' does exist under '" + ((ForestNET.Lib.Helper.IsStringEmpty(this.CurrentFolder.Name)) ? "/" : this.CurrentFolder.Name) + "'");
            }

            /* current folder variable */
            IMailFolder? o_currentFolder;

            ForestNET.Lib.Global.ILogFine("get full path to current folder '" + (this.CurrentFolder?.Name ?? "Current Folder is Null") + "'");

            /* get full path to current folder */
            string s_folderPath = this.CurrentFolder?.GetFullPath() ?? Folder.INBOX;

            ForestNET.Lib.Global.ILogFine("enter current folder '" + this.CurrentFolder?.GetFullPath() + "'");

            /* enter current folder */
            o_currentFolder = await this.GetSessionAsImapClient.GetFolderAsync(s_folderPath, this.Token());

            if (o_currentFolder == null)
            {
                throw new ImapCommandException(ImapCommandResponse.Bad, "Could not reach current folder '" + s_folderPath + "'");
            }

            ForestNET.Lib.Global.ILogFine("reached current folder, create sub folder");

            /* create sub folder with holding folders and holding message settings */
            IMailFolder o_newSubFolder = await o_currentFolder.CreateAsync(p_s_folderName, true, this.Token());

            ForestNET.Lib.Global.ILogFine("set sub folder as subscribed for imap");

            /* set sub folder as subscribed for imap */
            await o_newSubFolder.SubscribeAsync(this.Token());

            ForestNET.Lib.Global.ILogFine("add sub folder to current folder as child item");

            /* add sub folder to current folder as child item */
            this.CurrentFolder?.AddChildren(p_s_folderName);
        }

        /// <summary>
        /// Deletes current mail folder, no other sub folders and messages allowed
        /// </summary>
        /// <exception cref="ArgumentException">invalid current folder parameter or sub folder(s) and message(s) still there</exception>
        /// <exception cref="Exception">connection failed to mail server, or wrong folder path or issue deleting and unsubscribing current folder</exception>
        public async Task DeleteFolder()
        {
            /* POP3 is not supported */
            if (this.Pop3)
            {
                throw new ArgumentException("POP3(s) protocol is not supported");
            }

            /* check if mail client is set for retrieving mails */
            if (this.ImapSession == null)
            {
                throw new InvalidOperationException("mail client has no settings to retrieve mails from server");
            }

            ForestNET.Lib.Global.ILogFine("check if current folder is not Folder.ROOT");

            /* check if current folder is not Folder.ROOT */
            if ((this.CurrentFolder == null) || (ForestNET.Lib.Helper.IsStringEmpty(this.CurrentFolder.Name)))
            {
                throw new ArgumentException("Current folder is default folder, please specifiy a sub folder like 'inbox' or 'trash'");
            }

            /* current folder variable */
            IMailFolder? o_currentFolder;

            ForestNET.Lib.Global.ILogFine("get full path to current folder '" + (this.CurrentFolder?.Name ?? "Current Folder is Null") + "'");

            /* get full path to current folder */
            string s_folderPath = this.CurrentFolder?.GetFullPath() ?? Folder.INBOX;

            ForestNET.Lib.Global.ILogFine("enter current folder '" + this.CurrentFolder?.GetFullPath() + "'");

            /* enter current folder */
            o_currentFolder = await this.GetSessionAsImapClient.GetFolderAsync(s_folderPath, this.Token());

            if (o_currentFolder == null)
            {
                throw new ImapCommandException(ImapCommandResponse.Bad, "Could not reach current folder '" + s_folderPath + "'");
            }

            ForestNET.Lib.Global.ILogFine("unsubscribe folder for imap");

            /* unsubscribe folder for imap */
            await o_currentFolder.UnsubscribeAsync(this.Token());

            ForestNET.Lib.Global.ILogFine("delete target folder");

            /* delete target folder */
            await o_currentFolder.DeleteAsync(this.Token());

            ForestNET.Lib.Global.ILogFine("clear all children of parent folder");

            /* clear all children of parent folder */
            this.CurrentFolder?.Parent?.ClearChildren();

            ForestNET.Lib.Global.ILogFine("change from current folder to parent folder");

            /* change from current folder to parent folder */
            await this.ChangeToFolder("..");
        }

        /// <summary>
        /// Rename a sub folder in current mail folder
        /// </summary>
        /// <param name="p_s_folderName">invalid target folder parameter or sub folder(s) and message(s) still there</param>
        /// <param name="p_s_newFolderName">new name for target folder</param>
        /// <exception cref="ArgumentException">invalid target folder parameter or invalid new folder name, target folder does not exist, or new folder name already exists as a sub folder</exception>
        /// <exception cref="Exception">connection failed to mail server, or wrong folder path or issue renaming sub folder</exception>
        public async Task RenameSubFolder(string p_s_folderName, string p_s_newFolderName)
        {
            /* POP3 is not supported */
            if (this.Pop3)
            {
                throw new ArgumentException("POP3(s) protocol is not supported");
            }

            /* check if mail client is set for retrieving mails */
            if (this.ImapSession == null)
            {
                throw new InvalidOperationException("mail client has no settings to retrieve mails from server");
            }

            ForestNET.Lib.Global.ILogFine("check if current folder is not Folder.ROOT");

            /* check if current folder is not Folder.ROOT */
            if ((this.CurrentFolder == null) || (ForestNET.Lib.Helper.IsStringEmpty(this.CurrentFolder.Name)))
            {
                throw new ArgumentException("Current folder is default folder, please specifiy a sub folder like 'inbox' or 'trash'");
            }

            ForestNET.Lib.Global.ILogFine("check if target folder parameter does not contain a '/' character");

            /* check if target folder parameter does not contain a '/' character */
            if (p_s_folderName.Contains('/'))
            {
                throw new ArgumentException("Invalid character '/' in parameter '" + p_s_folderName + "'");
            }

            ForestNET.Lib.Global.ILogFine("check if new folder name parameter does not contain a '/' character");

            /* check if new folder name parameter does not contain a '/' character */
            if (p_s_newFolderName.Contains('/'))
            {
                throw new ArgumentException("Invalid character '/' in parameter '" + p_s_newFolderName + "'");
            }

            ForestNET.Lib.Global.ILogFine("check if target folder parameter is not equal '.' or '..'");

            /* check if target folder parameter is not equal '.' or '..' */
            if ((p_s_folderName.Equals(".")) || (p_s_folderName.Equals("..")))
            {
                throw new ArgumentException("Invalid sub folder name '" + p_s_folderName + "'");
            }

            ForestNET.Lib.Global.ILogFine("check if new folder name parameter is not equal '.' or '..'");

            /* check if new folder name parameter is not equal '.' or '..' */
            if ((p_s_newFolderName.Equals(".")) || (p_s_newFolderName.Equals("..")))
            {
                throw new ArgumentException("Invalid sub folder name '" + p_s_newFolderName + "'");
            }

            ForestNET.Lib.Global.ILogFine("check if sub folder already exists");

            /* check if sub folder exists */
            if (this.CurrentFolder?.GetSubFolder(p_s_folderName) == null)
            {
                throw new ArgumentException("Sub folder '" + p_s_folderName + "' does not exist under '" + ((ForestNET.Lib.Helper.IsStringEmpty(this.CurrentFolder?.Name)) ? "/" : this.CurrentFolder?.Name) + "'");
            }

            ForestNET.Lib.Global.ILogFine("check if new folder name already exists");

            /* check if new folder name already exists */
            if (this.CurrentFolder?.GetSubFolder(p_s_newFolderName) != null)
            {
                throw new ArgumentException("Sub folder '" + p_s_newFolderName + "' does exist under '" + ((ForestNET.Lib.Helper.IsStringEmpty(this.CurrentFolder?.Name)) ? "/" : this.CurrentFolder?.Name) + "'");
            }

            /* current folder + parent folder variable */
            IMailFolder? o_currentFolder;
            IMailFolder? o_targetFolder;

            ForestNET.Lib.Global.ILogFine("get full path to current folder '" + (this.CurrentFolder?.Name ?? "Current Folder is Null") + "'");

            /* get full path to current folder */
            string s_currentFolderPath = this.CurrentFolder?.GetFullPath() ?? Folder.INBOX;

            ForestNET.Lib.Global.ILogFine("enter current folder '" + this.CurrentFolder?.GetFullPath() + "'");

            /* enter current folder */
            o_currentFolder = await this.GetSessionAsImapClient.GetFolderAsync(s_currentFolderPath, this.Token());

            if (o_currentFolder == null)
            {
                throw new ImapCommandException(ImapCommandResponse.Bad, "Could not reach current folder '" + s_currentFolderPath + "'");
            }

            ForestNET.Lib.Global.ILogFine("get full path to target folder '" + p_s_folderName + "'");

            /* get full path to current folder */
            string s_targetFolderPath = (this.CurrentFolder?.GetFullPath() ?? Folder.INBOX) + "/" + p_s_folderName;

            ForestNET.Lib.Global.ILogFine("get target folder '" + (this.CurrentFolder?.GetFullPath() ?? Folder.INBOX) + "/" + p_s_folderName + "'");

            /* get target folder */
            o_targetFolder = await this.GetSessionAsImapClient.GetFolderAsync(s_targetFolderPath, this.Token());

            if (o_targetFolder == null)
            {
                throw new ImapCommandException(ImapCommandResponse.Bad, "Could not reach target folder '" + s_targetFolderPath + "'");
            }

            ForestNET.Lib.Global.ILogFine("rename target sub folder to new sub folder name");

            /* rename target sub folder to new sub folder name */
            await o_targetFolder.RenameAsync(o_currentFolder, p_s_newFolderName, this.Token());

            ForestNET.Lib.Global.ILogFine("subscribe folder for imap");

            /* subscribe folder for imap */
            await o_targetFolder.SubscribeAsync(this.Token());

            ForestNET.Lib.Global.ILogFine("clear all children of parent folder");

            /* clear all children of parent folder */
            this.CurrentFolder?.ClearChildren();

            /* save current folder name */
            string s_currentFolder = this.CurrentFolder?.Name ?? Folder.INBOX;

            ForestNET.Lib.Global.ILogFine("change from current folder to parent folder");

            /* change from current folder to parent folder */
            await this.ChangeToFolder("..");

            ForestNET.Lib.Global.ILogFine("change back to current folder");

            /* change back to current folder */
            await this.ChangeToFolder(s_currentFolder);
        }
    }
}