namespace ForestNET.Lib.Net.Https.SOAP
{
    /// <summary>
    /// Encapsulation of a SOAP Fault message with code, message, actor and detail values.
    /// </summary>
    public class SoapFault
    {

        /* Fields */

        /* Properties */

        public string? Code { get; set; }
        public string? Message { get; set; }
        public string? Actor { get; set; }
        public string? Detail { get; set; }

        /* Methods */

        /// <summary>
        /// SOAP Fault constructor
        /// </summary>
        /// <param name="p_s_code">code value</param>
        /// <param name="p_s_message">message value</param>
        /// <param name="p_s_detail">detail value (optional)</param>
        /// <param name="p_s_actor">actor value (optional)</param>
        public SoapFault(string p_s_code, string p_s_message, string? p_s_detail, string? p_s_actor)
        {
            this.Code = p_s_code;
            this.Message = p_s_message;
            this.Detail = p_s_detail;
            this.Actor = p_s_actor;
        }

        /// <summary>
        /// Creates a string line with all of SOAP Fault's values
        /// </summary>
        public override string ToString()
        {
            string s_foo = "SoapFault:" + ForestNET.Lib.IO.File.NEWLINE;

            s_foo += "Code = " + this.Code + ForestNET.Lib.IO.File.NEWLINE;
            s_foo += "Message = " + this.Message + ForestNET.Lib.IO.File.NEWLINE;
            s_foo += "Detail = " + this.Detail + ForestNET.Lib.IO.File.NEWLINE;
            s_foo += "Actor = " + this.Actor;

            return s_foo;
        }

        /// <summary>
        /// Creates a complete xml SOAP Fault message for response purposes, with xml header, envelope and body
        /// </summary>
        /// <param name="p_s_encoding">encoding value for the xml header attribute</param>
        public string ToXML(string p_s_encoding)
        {
            string s_xml = "<?xml version=\"1.0\" encoding=\"" + p_s_encoding + "\" ?>" + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK;

            s_xml += "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">" + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK;
            s_xml += "\t<soap:Body>" + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK;
            s_xml += "\t\t<soap:Fault>" + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK;

            if (ForestNET.Lib.Helper.IsStringEmpty(this.Code))
            {
                s_xml += "\t\t\t<faultcode/>" + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK;
            }
            else
            {
                s_xml += "\t\t\t<faultcode>" + this.Code + "</faultcode>" + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK;
            }

            if (ForestNET.Lib.Helper.IsStringEmpty(this.Message))
            {
                s_xml += "\t\t\t<faultstring/>" + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK;
            }
            else
            {
                s_xml += "\t\t\t<faultstring>" + this.Message + "</faultstring>" + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK;
            }

            if (ForestNET.Lib.Helper.IsStringEmpty(this.Detail))
            {
                s_xml += "\t\t\t<detail/>" + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK;
            }
            else
            {
                s_xml += "\t\t\t<detail>" + this.Detail + "</detail>" + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK;
            }

            if (ForestNET.Lib.Helper.IsStringEmpty(this.Actor))
            {
                s_xml += "\t\t\t<faultactor/>" + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK;
            }
            else
            {
                s_xml += "\t\t\t<faultactor>" + this.Actor + "</faultactor>" + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK;
            }

            s_xml += "\t\t</soap:Fault>" + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK;
            s_xml += "\t</soap:Body>" + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK;
            s_xml += "</soap:Envelope>";

            return s_xml;
        }
    }
}