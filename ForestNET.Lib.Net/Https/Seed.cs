namespace ForestNET.Lib.Net.Https
{
    /// <summary>
    /// Seed class is the core of forestJ tiny server application, which holds all necessary data about request header, response header, body data, post data, post file data, session data, central configuration settings, temporary object list for dynamic handling of all requests.
    /// </summary>
    public class Seed
    {

        /* Fields */

        private Config o_config;
        private RequestHeader o_requestHeader;
        private ResponseHeader o_responseHeader;
        private byte[]? a_requestBody;
        private byte[]? a_responseBody;
        private int i_returnCode;

        private Dictionary<string, string?> a_postData;
        private List<ForestNET.Lib.Net.Https.Dynm.FileData> a_fileData;
        private Dictionary<string, string?> a_sessionData;

        private System.Text.StringBuilder o_buffer;
        private Dictionary<string, Object?> a_temp;

        private string? s_salt;

        /* Properties */

        /// <summary>
        /// Configuration instance parameter
        /// </summary>
        /// <exception cref="ArgumentException">configuration instance parameter is null</exception>
        public Config Config
        {
            get { return o_config; }
            private set
            {
                this.o_config = value ?? throw new ArgumentNullException(nameof(value), "Config parameter is null");
            }
        }
        /// <summary>
        /// salt value for log purpose
        /// </summary>
        /// <exception cref="ArgumentException">salt parameter value is null or empty</exception>
        public string? Salt
        {
            get
            {
                return this.s_salt;
            }
            set
            {
                if (ForestNET.Lib.Helper.IsStringEmpty(value))
                {
                    throw new ArgumentNullException(nameof(value), "Salt parameter is null or empty");
                }

                this.s_salt = value;
            }
        }
        public RequestHeader RequestHeader
        {
            get { return this.o_requestHeader; }
            private set { this.o_requestHeader = value; }
        }
        public ResponseHeader ResponseHeader
        {
            get { return this.o_responseHeader; }
            private set { this.o_responseHeader = value; }
        }
        public byte[]? RequestBody
        {
            get { return this.a_requestBody; }
            set { this.a_requestBody = value; }
        }
        public byte[]? ResponseBody
        {
            get { return this.a_responseBody; }
            set { this.a_responseBody = value; }
        }
        public int ReturnCode
        {
            get
            {
                return this.i_returnCode;
            }
            set
            {
                this.i_returnCode = value;
                this.o_responseHeader.ReturnCode = this.i_returnCode;
            }
        }
        public Dictionary<string, string?> PostData
        {
            get { return this.a_postData; }
            private set { this.a_postData = value; }
        }
        public List<ForestNET.Lib.Net.Https.Dynm.FileData> FileData
        {
            get { return this.a_fileData; }
            private set { this.a_fileData = value; }
        }
        public Dictionary<string, string?> SessionData
        {
            get { return this.a_sessionData; }
            private set { this.a_sessionData = value; }
        }
        public System.Text.StringBuilder Buffer
        {
            get { return this.o_buffer; }
            private set { this.o_buffer = value; }
        }
        public Dictionary<string, Object?> Temp
        {
            get { return this.a_temp; }
            private set { this.a_temp = value; }
        }

        /* Methods */

        /// <summary>
        /// Seed constructor, initializing all settings, request header instance, response header instance and lists
        /// </summary>
        /// <param name="p_o_config">configuration instance parameter</param>
        /// <exception cref="ArgumentNullException">configuration instance parameter is null, or domain parameter from configuration is null or empty</exception>
        public Seed(Config p_o_config)
        {
            this.o_config = p_o_config;
            this.o_requestHeader = new RequestHeader(this.Config.Domain);
            this.o_responseHeader = new ResponseHeader();
            this.a_requestBody = null;
            this.a_responseBody = null;
            this.i_returnCode = -1;

            this.a_postData = [];
            this.a_fileData = [];
            this.a_sessionData = [];

            this.o_buffer = new();
            this.a_temp = [];

            this.s_salt = null;
        }
    }
}