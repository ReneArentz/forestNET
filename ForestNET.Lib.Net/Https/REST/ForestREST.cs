namespace ForestNET.Lib.Net.Https.REST
{
    /// <summary>
    /// Abstract class for ForestRest implementation. Inheriting class has to implement all methods to handle http methods GET, POST, PUT and DELETE.
    /// </summary>
    public abstract class ForestREST
    {

        /* Fields */

        /* Properties */

        protected ForestNET.Lib.Net.Https.Seed? Seed
        {
            get; set;
        }
        public string? ResponseContentType
        {
            get; set;
        }

        /// <summary>
        /// Set response content type by using file extension value
        /// </summary>
        /// <param name="p_s_fileExtension">file extension value</param>
        /// <exception cref="ArgumentException">invalid file extension value is not listed within known extension list within ForestNET.Lib.Net.Https.Config class</exception>
        public void SetResponseContentTypeByFileExtension(string? p_s_fileExtension)
        {
            /* check file extension parameter */
            if ((p_s_fileExtension == null) || (ForestNET.Lib.Helper.IsStringEmpty(p_s_fileExtension)))
            {
                this.ResponseContentType = null;
                return;
            }

            string? s_extension = null;


            /* check for valid extension in configured allowed list */
            if (this.Seed != null)
            {
                foreach (KeyValuePair<string, string> o_allowExtension in this.Seed.Config.AllowExtensionList)
                {
                    if (o_allowExtension.Key.EndsWith(p_s_fileExtension))
                    {
                        /* file extension found in allow list */
                        s_extension = o_allowExtension.Key;
                        break;
                    }
                }
            }

            /* return 403 if file extension is forbidden */
            if ((s_extension == null) || (ForestNET.Lib.Helper.IsStringEmpty(s_extension)))
            {
                throw new ArgumentNullException("Invalid file extension value '" + p_s_fileExtension + "' which is not allowed for response");
            }

            /* get content type by file extension */
            this.ResponseContentType = ForestNET.Lib.Net.Https.Config.KNOWN_EXTENSION_LIST[s_extension];
        }

        /* Methods */

        /// <summary>
        /// The core rest method which takes a seed instance as parameter, so anything within handling http methods has access to all request/response/other resources
        /// </summary>
        /// <param name="p_o_value">seed instance object</param>
        /// <returns>returning response for rest request as string</returns>
        /// <exception cref="Exception">any exception which occurred while handling http methods</exception>
        public string HandleREST(ForestNET.Lib.Net.Https.Seed p_o_value)
        {
            this.Seed = p_o_value;
            this.ResponseContentType = null;

            if (this.Seed.RequestHeader.Method.Equals("GET"))
            {
                return this.HandleGET();
            }
            else if (this.Seed.RequestHeader.Method.Equals("POST"))
            {
                return this.HandlePOST();
            }
            else if (this.Seed.RequestHeader.Method.Equals("PUT"))
            {
                return this.HandlePUT();
            }
            else if (this.Seed.RequestHeader.Method.Equals("DELETE"))
            {
                return this.HandleDELETE();
            }

            throw new Exception("HTTP method " + this.Seed.RequestHeader.Method + " with Mode " + this.Seed.Config.Mode + " not implemented");
        }

        /// <summary>
        /// Handling http GET method
        /// </summary>
        /// <returns>returning response for rest GET request as string</returns>
        /// <exception cref="Exception">any exception which occurred while handling http GET method</exception>
        abstract public string HandleGET();

        /// <summary>
        /// Handling http POST method
        /// </summary>
        /// <returns>returning response for rest POST request as string</returns>
        /// <exception cref="Exception">any exception which occurred while handling http POST method</exception>
        abstract public string HandlePOST();

        /// <summary>
        /// Handling http PUT method
        /// </summary>
        /// <returns>returning response for rest PUT request as string</returns>
        /// <exception cref="Exception">any exception which occurred while handling http PUT method</exception>
        abstract public string HandlePUT();

        /// <summary>
        /// Handling http DELETE method
        /// </summary>
        /// <returns>returning response for rest DELETE request as string</returns>
        /// <exception cref="Exception">any exception which occurred while handling http DELETE method</exception>
        abstract public string HandleDELETE();
    }
}
