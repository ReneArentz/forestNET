namespace ForestNET.Lib.Net.SQL.Pool
{
    /// <summary>
    /// Configuration class for tiny https/soap/rest server object. All configurable settings are listed and adjustable in this class. Please look on the comments of the set-property-methods to see further details.
    /// Supports a base pool instance.
    /// </summary>
    public class HttpsConfig : ForestNET.Lib.Net.Https.Config
    {

        /* Fields */

        private ForestNET.Lib.SQL.Pool.BasePool? o_basePool;

        /* Properties */

        /// <summary>
        /// Base Pool object instance
        /// </summary>
        /// <exception cref="ArgumentNullException">Base Pool object instance parameter is null</exception>
        public ForestNET.Lib.SQL.Pool.BasePool? BasePool
        {
            get
            {
                return this.o_basePool;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "Base Pool object instance is null");
                }

                this.o_basePool = value;
            }
        }

        /* Methods */

        /// <summary>
        /// Constructor of configuration class. Using NORMAL mode and SERVER type. All other settings are adjusted by set-property-methods
        /// </summary>
        /// <param name="p_s_domain">determine domain value for tiny https server configuration</param>
        /// <exception cref="ArgumentException">domain parameter value does not start with 'https://'</exception>
        public HttpsConfig(string p_s_domain) :
            this(p_s_domain, Https.Mode.NORMAL, ForestNET.Lib.Net.Sock.Recv.ReceiveType.SERVER)
        {

        }

        /// <summary>
        /// Constructor of configuration class. Using SERVER type. All other settings are adjusted by set-property-methods
        /// </summary>
        /// <param name="p_s_domain">determine domain value for tiny https server configuration</param>
        /// <param name="p_e_mode">determine mode for tiny https server: NORMAL, DYNAMIC, SOAP or REST</param>
        /// <exception cref="ArgumentException">domain parameter value does not start with 'https://'</exception>
        public HttpsConfig(string p_s_domain, Https.Mode p_e_mode) :
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
        public HttpsConfig(string p_s_domain, Https.Mode p_e_mode, ForestNET.Lib.Net.Sock.Recv.ReceiveType p_e_receiveType) :
            base(p_s_domain, p_e_mode, p_e_receiveType)
        {
            this.o_basePool = null;
        }
    }
}
