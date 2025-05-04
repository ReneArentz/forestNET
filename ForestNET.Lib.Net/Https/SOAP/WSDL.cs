namespace ForestNET.Lib.Net.Https.SOAP
{
    /// <summary>
    /// WSDL class to parse a wsdl file and having all information within class instances and stored properties available after parsing.
    /// </summary>
    public class WSDL
    {

        /* Delegates */

        /// <summary>
        /// Empty method shell for a SOAP operation method
        /// </summary>
        /// <param name="p_o_inputMessage">soap input message as parameter</param>
        /// <param name="p_o_seed">seed instance as parameter</param>
        /// <returns>soap output message as parameter, will be controlled by tiny SOAP server if it matches the WSDL</returns>
        /// <exception cref="Exception">any kind of exception which can happen during the operation</exception>
        public delegate Object? SOAPOperationShell(Object? p_o_inputMessage, ForestNET.Lib.Net.Https.Seed p_o_seed);

        /* Fields */

        private readonly string s_path;

        /* Properties */

        public string WSDLFile { get; private set; }
        public string? XSDFile { get; private set; }
        public string Documentation { get; private set; }
        public ForestNET.Lib.IO.XML Schema { get; private set; }
        public List<Message> Messages { get; private set; }
        public List<PortTypeOperation> PortTypeOperations { get; private set; }
        public List<Binding> Bindings { get; private set; }
        public Service ServiceInstance { get; private set; }
        public Dictionary<string, SOAPOperationShell?> SOAPOperations { get; private set; }

        /* Methods */

        /// <summary>
        /// WSDL constructor, giving a wsdl file as information input
        /// </summary>
        /// <param name="p_s_file">full-path to wsdl schema file</param>
        /// <exception cref="ArgumentException">value/structure within wsdl schema file invalid</exception>
        /// <exception cref="ArgumentNullException">wsdl schema, root node is null</exception>
        /// <exception cref="System.IO.IOException">cannot access or open wsdl file and it's content</exception>
        public WSDL(string p_s_file)
        {
            this.s_path = string.Empty;
            this.WSDLFile = string.Empty;
            this.XSDFile = null;
            this.Documentation = string.Empty;
            this.SOAPOperations = [];

            /* check if file exists */
            if (!ForestNET.Lib.IO.File.Exists(p_s_file))
            {
                throw new ArgumentException("File[" + p_s_file + "] does not exist.");
            }

            /* get path of wsdl and possible xsd schema location */
            if (p_s_file.Contains('/'))
            { /* unix directory separator */
                this.s_path = p_s_file.Substring(0, p_s_file.LastIndexOf("/") + 1);
                this.WSDLFile = p_s_file.Substring(p_s_file.LastIndexOf("/") + 1);
            }
            else if (p_s_file.Contains('\\'))
            { /* windows directory separator */
                this.s_path = p_s_file.Substring(0, p_s_file.LastIndexOf("\\") + 1);
                this.WSDLFile = p_s_file.Substring(p_s_file.LastIndexOf("\\") + 1);
            }

            /* open wsdl-schema file */
            ForestNET.Lib.IO.File o_file = new(p_s_file, false);

            System.Text.StringBuilder o_stringBuilder = new();

            /* read all wsdl schema file lines to one string builder */
            foreach (string s_line in o_file.FileContentAsList ?? [])
            {
                o_stringBuilder.Append(s_line);
            }

            /* read all wsdl-schema file lines and delete all line-wraps and tabs and values only containing white spaces */
            string s_wsdl = o_stringBuilder.ToString();
            s_wsdl = new System.Text.RegularExpressions.Regex("[\\r\\n\\t]").Replace(s_wsdl, "");
            s_wsdl = new System.Text.RegularExpressions.Regex(">\\s*<").Replace(s_wsdl, "><");

            /* do not allow 'soap' or 'soap12' namespace tags, only SOAP 1.1 in a reduced primitive implementation is allowed */
            if ((s_wsdl.Contains("<soap:")) && (s_wsdl.Contains("<soap12:")))
            {
                throw new ArgumentException("SOAP for forestNET can only handle SOAP 1.1 in a reduced primitive implementation.");
            }

            /* clean up wsdl-schema */
            s_wsdl = new System.Text.RegularExpressions.Regex("<\\?(.*?)\\?>").Replace(s_wsdl, "");
            s_wsdl = new System.Text.RegularExpressions.Regex("<!--(.*?)-->").Replace(s_wsdl, "");
            s_wsdl = new System.Text.RegularExpressions.Regex("<xs:annotation>(.*?)</xs:annotation>").Replace(s_wsdl, "");
            s_wsdl = new System.Text.RegularExpressions.Regex("<soap:").Replace(s_wsdl, "<");
            s_wsdl = new System.Text.RegularExpressions.Regex("</soap:").Replace(s_wsdl, "</");
            s_wsdl = new System.Text.RegularExpressions.Regex("<wsdl:").Replace(s_wsdl, "<");
            s_wsdl = new System.Text.RegularExpressions.Regex("</wsdl:").Replace(s_wsdl, "</");

            /* make documentation tags readable for this class */
            s_wsdl = new System.Text.RegularExpressions.Regex("<documentation>").Replace(s_wsdl, "<documentation value=\"");
            s_wsdl = new System.Text.RegularExpressions.Regex("</documentation>").Replace(s_wsdl, "\"/>");

            /* validate wsdl */
            System.Text.RegularExpressions.Regex o_regex = new("(<[^<>]*?<[^<>]*?>|<[^<>]*?>[^<>]*?>)");
            System.Text.RegularExpressions.MatchCollection o_matcher = o_regex.Matches(s_wsdl);

            /* if regex-matcher has match, the wsdl-schema is not valid */
            while (o_matcher.Count > 0)
            {
                throw new ArgumentException("Invalid wsdl-schema. Please check wsdl-schema at \"" + o_matcher[0] + "\".");
            }

            List<string> a_wsdlTags = [];

            /* add all wsdl-schema-tags to a list for parsing */
            o_regex = new System.Text.RegularExpressions.Regex("<[^<>]*?>");
            o_matcher = o_regex.Matches(s_wsdl);

            if (o_matcher.Count > 0)
            {
                for (int i = 0; i < o_matcher.Count; i++)
                {
                    a_wsdlTags.Add(o_matcher[i].ToString());
                }
            }

            /* check if wsdl-schema starts with <definitions>-tag */
            if (!a_wsdlTags[0].StartsWith("<definitions", StringComparison.CurrentCultureIgnoreCase))
            {
                throw new ArgumentException("wsdl-schema must start with <definitions>-tag");
            }

            this.Messages = [];
            this.PortTypeOperations = [];
            this.Bindings = [];

            /* call method to parse wsdl */
            this.ParseWSDL(a_wsdlTags, 0, a_wsdlTags.Count);

            /* check if root is null */
            if (this.Schema == null)
            {
                throw new NullReferenceException("Schema is null after parsing wsdl");
            }

            /* check if root is null */
            if (this.ServiceInstance == null)
            {
                throw new NullReferenceException("Service instance is null after parsing wsdl");
            }
        }

        /// <summary>
        /// Parsing complete wsdl file
        /// </summary>
        /// <param name="p_a_wsdlTags">all wsdl xml/xsd-tags</param>
        /// <param name="p_i_min">xml/xsd-tag in parameter list where we start</param>
        /// <param name="p_i_max">xml/xsd-tag in parameter list where we stop</param>
        /// <exception cref="ArgumentException">invalid xml/xsd-tag found within wsdl file</exception>
        /// <exception cref="System.IO.IOException">cannot access or open xsd file and it's content for schema location import</exception>
        private void ParseWSDL(List<string> p_a_wsdlTags, int p_i_min, int p_i_max)
        {
            if (p_i_min == 0)
            { /* expecting <definitions> */
                /* check if we have <definitions> */
                if (!p_a_wsdlTags[0].StartsWith("<definitions"))
                {
                    throw new ArgumentException("Invalid wsdl document. Expected <wsdl:definitions>-tag, but found \"" + p_a_wsdlTags[0] + "\".");
                }

                /* check if <definitions> will be closed */
                if (!WSDL.LookForEndTag(p_a_wsdlTags[0], p_a_wsdlTags, 1, p_i_max - 1))
                {
                    throw new ArgumentException("Invalid wsdl document. <wsdl:definitions>-tag is not closed in wsdl file");
                }

                /* go to next xml/xsd-tag */
                p_i_min++;
            }

            if (p_i_min == 1)
            { /* expecting <documentation /> or <types> */
                if ((p_a_wsdlTags[1].StartsWith("<documentation")) && (p_a_wsdlTags[1].EndsWith("/>")))
                { /* check if we have <documentation /> */
                    /* parse <documentation /> tag */
                    this.Documentation = WSDL.ParseDocumentationTag(p_a_wsdlTags[1]);

                    /* parse <types> */
                    p_i_min = this.ParseTypes(p_a_wsdlTags, (p_i_min + 2), p_i_max);
                }
                else if (p_a_wsdlTags[1].Equals("<types>"))
                { /* check if we have <types> */
                    /* parse <types> */
                    p_i_min = this.ParseTypes(p_a_wsdlTags, (p_i_min + 1), p_i_max);
                }
            }

            /* check if parsing until now was valid */
            if (p_i_min < 0)
            {
                throw new ArgumentException("Invalid wsdl document. <types> could not be parsed");
            }

            /* parse <message> tags */
            p_i_min = this.ParseMessages(p_a_wsdlTags, p_i_min, p_i_max);

            /* parse <portType> with <operation> tags */
            p_i_min = this.ParsePortType(p_a_wsdlTags, p_i_min, p_i_max);

            /* parse <binding> tags */
            do
            {
                p_i_min = this.ParseBinding(p_a_wsdlTags, p_i_min, p_i_max);
            } while (p_a_wsdlTags[p_i_min].StartsWith("<binding"));

            /* parse <service> with <port> tags */
            this.ParseService(p_a_wsdlTags, p_i_min, p_i_max);
        }

        /// <summary>
        /// method to check if a xml/xsd-tag will be closed within wsdl file
        /// </summary>
        /// <param name="p_s_wsdlTag">xml/xsd-tag which must be closed</param>
        /// <param name="p_a_wsdlTags">list of following xml/xsd-tags</param>
        /// <param name="p_i_min">xml/xsd-tag in parameter list where we start</param>
        /// <param name="p_i_max">xml/xsd-tag in parameter list where we stop</param>
        private static bool LookForEndTag(string p_s_wsdlTag, List<string> p_a_wsdlTags, int p_i_min, int p_i_max)
        {
            /* get xml/xsd-tag definition name */
            if (p_s_wsdlTag.IndexOf(" ") > 1)
            {
                /* get xml/xsd-tag definition name until first appearance of a whitespace */
                p_s_wsdlTag = p_s_wsdlTag.Substring(1, p_s_wsdlTag.IndexOf(" ") - 1);
            }
            else
            {
                /* get xml/xsd-tag definition name until end of tag */
                p_s_wsdlTag = p_s_wsdlTag.Substring(1, p_s_wsdlTag.IndexOf(">") - 1);
            }

            /* iterate each following xml/xsd-tag */
            for (int i_min = p_i_min; i_min <= p_i_max; i_min++)
            {
                /* if we find our definition name as closing tag */
                if (p_a_wsdlTags[i_min].Equals("</" + p_s_wsdlTag + ">"))
                {
                    /* we have our end tag */
                    return true;
                }
            }

            /* no end tag found */
            return false;
        }

        /// <summary>
        /// get documentation value out of xml/xsd-tag
        /// </summary>
        /// <param name="p_s_wsdlTag">xml/xsd-tag with documenation value</param>
        /// <exception cref="ArgumentException">Invalid wsdl-tag xs:attribute without a documentation value</exception>
        private static string ParseDocumentationTag(string p_s_wsdlTag)
        {
            /* read documentation tag */
            System.Text.RegularExpressions.Regex o_regex = new("value=\"([^\"]*)\"");
            System.Text.RegularExpressions.Match o_matcher = o_regex.Match(p_s_wsdlTag);

            if (o_matcher.Success)
            {
                /* look for and find value attribute */
                return o_matcher.Groups[0].Value.Substring(7, o_matcher.Groups[0].Value.Length - 1 - 7);
            }
            else
            {
                /* no documentation attribute found */
                throw new ArgumentException("Invalid wsdl-tag xs:attribute without a documentation value");
            }
        }

        /// <summary>
        /// parsing types with schema xsd or schema location import
        /// </summary>
        /// <param name="p_a_wsdlTags">list of following xml/xsd-tags</param>
        /// <param name="p_i_min">xml/xsd-tag in parameter list where we start</param>
        /// <param name="p_i_max">xml/xsd-tag in parameter list where we stop</param>
        /// <exception cref="ArgumentException">invalid xml/xsd-tag found within wsdl file</exception>
        /// <exception cref="System.IO.IOException">cannot access or open xsd file and it's content for schema location import</exception>
        private int ParseTypes(List<string> p_a_wsdlTags, int p_i_min, int p_i_max)
        {
            /* check if <types> will be closed */
            if (!WSDL.LookForEndTag("<types>", p_a_wsdlTags, p_i_min, p_i_max))
            {
                throw new ArgumentException("Invalid wsdl document. <types>-tag is not closed in wsdl file");
            }

            /* expect <xs:schema> or <schema> tag */
            if (!((p_a_wsdlTags[p_i_min].StartsWith("<xs:schema ")) || (p_a_wsdlTags[p_i_min].EndsWith("schema>"))))
            {
                throw new ArgumentException("Invalid wsdl document. <xs:schema>-tag not detected after <types>-tag");
            }

            if (p_a_wsdlTags[p_i_min + 1].StartsWith("<xs:import "))
            { /* import xsd schema from other file */
                /* read schemaLocation attribute */
                System.Text.RegularExpressions.Regex o_regex = new("schemaLocation=\"([^\"]*)\"");
                System.Text.RegularExpressions.Match o_matcher = o_regex.Match(p_a_wsdlTags[p_i_min + 1]);

                if (o_matcher.Success)
                {
                    string s_schemaLocation = o_matcher.Groups[0].Value.Substring(16, o_matcher.Groups[0].Value.Length - 1 - 16);

                    /* remove './' from schema location value */
                    if (s_schemaLocation.StartsWith("./"))
                    {
                        s_schemaLocation = s_schemaLocation.Substring(2);
                    }

                    /* xsd schema must be in the same folder as wsdl file */
                    if ((s_schemaLocation.Contains('/')) || (s_schemaLocation.Contains('\\')))
                    {
                        throw new ArgumentException("Invalid wsdl document. xsd schema in types, <xs:import>-tag has invalid schema location. xsd-file must stay next to wsdl-file");
                    }

                    /* store xsd file value */
                    this.XSDFile = s_schemaLocation;

                    /* create xml instance and store it */
                    this.Schema = new ForestNET.Lib.IO.XML(this.s_path + s_schemaLocation);

                    /* find end of <types> part */
                    for (int i = p_i_min + 2; i < p_i_max; i++)
                    {
                        if (p_a_wsdlTags[i].Equals("</types>"))
                        {
                            /* return xml/xsd-tag position to continue parsing */
                            return i + 1;
                        }
                    }
                }
                else
                {
                    /* no schemaLocation attribute found */
                    throw new ArgumentException("Invalid wsdl-tag xs:import without a schemaLocation attribute value");
                }
            }
            else
            { /* read xsd schema tags */
                List<string> a_schemaLines = [];
                int i_endOfTypes = -1;

                for (int i = p_i_min; i < p_i_max; i++)
                {
                    /* find end of <types> part */
                    if (p_a_wsdlTags[i].Equals("</types>"))
                    {
                        i_endOfTypes = i;
                        break;
                    }

                    /* add xml/xsd-tag to our schema */
                    a_schemaLines.Add(p_a_wsdlTags[i]);
                }

                /* create xml instance and store it */
                this.Schema = new ForestNET.Lib.IO.XML(a_schemaLines);

                /* return xml/xsd-tag position to continue parsing */
                return (i_endOfTypes + 1);
            }

            return -1;
        }

        /// <summary>
        /// parsing messages tags
        /// </summary>
        /// <param name="p_a_wsdlTags">list of following xml/xsd-tags</param>
        /// <param name="p_i_min">xml/xsd-tag in parameter list where we start</param>
        /// <param name="p_i_max">xml/xsd-tag in parameter list where we stop</param>
        /// <exception cref="ArgumentException">invalid xml/xsd-tag found within wsdl file</exception>
        private int ParseMessages(List<string> p_a_wsdlTags, int p_i_min, int p_i_max)
        {
            int i_endOfMessages = -1;

            /* find end of message tags */
            for (int i = p_i_min; i < p_i_max; i++)
            {
                if (p_a_wsdlTags[i].StartsWith("<portType"))
                {
                    i_endOfMessages = i - 1;
                    break;
                }
            }

            /* could not find next <portType> after message tags */
            if (i_endOfMessages < 0)
            {
                throw new ArgumentException("Invalid wsdl document. <wsdl:message> tags could not be parsed");
            }

            /* iterate all message xml/xsd-tags */
            for (int i = p_i_min; i <= i_endOfMessages; i++)
            {
                string s_messageName;
                string s_messagePartName;
                string s_messagePartElement;

                if (p_a_wsdlTags[i].StartsWith("<message"))
                {
                    /* read name tag */
                    System.Text.RegularExpressions.Regex o_regex = new("name=\"([^\"]*)\"");
                    System.Text.RegularExpressions.Match o_matcher = o_regex.Match(p_a_wsdlTags[i]);

                    if (o_matcher.Success)
                    {
                        /* store message name */
                        s_messageName = o_matcher.Groups[0].Value.Substring(6, o_matcher.Groups[0].Value.Length - 1 - 6);
                    }
                    else
                    {
                        /* no name attribute found */
                        throw new ArgumentException("Invalid <wsdl:message>-tag without a name attribute");
                    }

                    /* look for end of message tag */
                    if (!WSDL.LookForEndTag(p_a_wsdlTags[i], p_a_wsdlTags, i + 1, i_endOfMessages))
                    {
                        throw new ArgumentException("Invalid wsdl document. <wsdl:message>-tag is not closed in wsdl file");
                    }

                    i++;

                    /* expect <part> tag */
                    if (p_a_wsdlTags[i].StartsWith("<part"))
                    {
                        /* read name tag */
                        o_regex = new("name=\"([^\"]*)\"");
                        o_matcher = o_regex.Match(p_a_wsdlTags[i]);

                        if (o_matcher.Success)
                        {
                            /* store part name of message */
                            s_messagePartName = o_matcher.Groups[0].Value.Substring(6, o_matcher.Groups[0].Value.Length - 1 - 6);

                            /* part name of message must be 'parameters' */
                            if (!s_messagePartName.Equals("parameters"))
                            {
                                throw new ArgumentException("<wsdl:part>-tag name attribute must have the value 'parameters'");
                            }
                        }
                        else
                        {
                            /* no name attribute found */
                            throw new ArgumentException("Invalid <wsdl:part>-tag without a name attribute");
                        }

                        /* read element tag */
                        o_regex = new("element=\"([^\"]*)\"");
                        o_matcher = o_regex.Match(p_a_wsdlTags[i]);

                        if (o_matcher.Success)
                        {
                            /* store part element of message */
                            s_messagePartElement = o_matcher.Groups[0].Value.Substring(9, o_matcher.Groups[0].Value.Length - 1 - 9);

                            /* add new message object to our wsdl instance */
                            this.Messages.Add(new Message(s_messageName, s_messagePartName, s_messagePartElement));
                        }
                        else
                        {
                            /* no element attribute found */
                            throw new ArgumentException("Invalid <wsdl:part>-tag without an element attribute");
                        }

                        if (p_a_wsdlTags[i].EndsWith("/>"))
                        { /* <part> tag was self closing */
                            i--;
                        }
                        else if (!WSDL.LookForEndTag(p_a_wsdlTags[i], p_a_wsdlTags, i + 1, i_endOfMessages))
                        { /* check if <part> tag is closed */
                            throw new ArgumentException("Invalid wsdl document. <wsdl:part>-tag is not closed in wsdl file");
                        }

                        i++;
                        i++;
                    }
                    else
                    {
                        /* no <wsdl:part>-tag found */
                        throw new ArgumentException("No <wsdl:part>-tag found");
                    }
                }
                else
                {
                    /* no <wsdl:message>-tag found */
                    throw new ArgumentException("No <wsdl:message>-tag found");
                }
            }

            /* return xml/xsd-tag position to continue parsing */
            return i_endOfMessages + 1;
        }

        /// <summary>
        /// parsing portType tag and all it's operation tags
        /// </summary>
        /// <param name="p_a_wsdlTags">list of following xml/xsd-tags</param>
        /// <param name="p_i_min">xml/xsd-tag in parameter list where we start</param>
        /// <param name="p_i_max">xml/xsd-tag in parameter list where we stop</param>
        /// <exception cref="ArgumentException">invalid xml/xsd-tag found within wsdl file</exception>
        private int ParsePortType(List<string> p_a_wsdlTags, int p_i_min, int p_i_max)
        {
            int i_endOfPortTypes = -1;

            /* find end of portType tags */
            for (int i = p_i_min; i < p_i_max; i++)
            {
                if (p_a_wsdlTags[i].StartsWith("<binding"))
                {
                    i_endOfPortTypes = i - 1;
                    break;
                }
            }

            /* could not find next <binding> after portType tag */
            if (i_endOfPortTypes < 0)
            {
                throw new ArgumentException("Invalid wsdl document. <wsdl:portType> tags could not be parsed");
            }

            string s_portTypeName;

            /* first one must be our portType tag */
            if (p_a_wsdlTags[p_i_min].StartsWith("<portType"))
            {
                /* read name tag */
                System.Text.RegularExpressions.Regex o_regex = new("name=\"([^\"]*)\"");
                System.Text.RegularExpressions.Match o_matcher = o_regex.Match(p_a_wsdlTags[p_i_min]);

                if (o_matcher.Success)
                {
                    /* store portType name */
                    s_portTypeName = o_matcher.Groups[0].Value.Substring(6, o_matcher.Groups[0].Value.Length - 1 - 6);
                }
                else
                {
                    /* no name attribute found */
                    throw new ArgumentException("Invalid <wsdl:portType>-tag without a name attribute");
                }

                /* look for end of portType tag */
                if (!WSDL.LookForEndTag(p_a_wsdlTags[p_i_min], p_a_wsdlTags, p_i_min + 1, i_endOfPortTypes))
                {
                    throw new ArgumentException("Invalid wsdl document. <wsdl:portType>-tag is not closed in wsdl file");
                }
            }
            else
            {
                /* no <wsdl:portType>-tag found */
                throw new ArgumentException("No <wsdl:portType>-tag found");
            }

            /* iterate following tags which will include all operations of portType */
            for (int i = p_i_min + 1; i < i_endOfPortTypes; i++)
            {
                string s_portTypeOperationName;
                string s_inputMessage;
                string s_outputMessage;
                Message? o_inputMessage = null;
                Message? o_outputMessage = null;
                string? s_portTypeDocumentation = null;

                /* expect operation tag */
                if (p_a_wsdlTags[i].StartsWith("<operation"))
                {
                    /* read name tag */
                    System.Text.RegularExpressions.Regex o_regex = new("name=\"([^\"]*)\"");
                    System.Text.RegularExpressions.Match o_matcher = o_regex.Match(p_a_wsdlTags[i]);

                    if (o_matcher.Success)
                    {
                        /* store operation name */
                        s_portTypeOperationName = o_matcher.Groups[0].Value.Substring(6, o_matcher.Groups[0].Value.Length - 1 - 6);
                    }
                    else
                    {
                        /* no name attribute found */
                        throw new ArgumentException("Invalid <wsdl:operation>-tag without a name attribute");
                    }

                    /* look for end of operation tag */
                    if (!WSDL.LookForEndTag(p_a_wsdlTags[i], p_a_wsdlTags, i + 1, i_endOfPortTypes))
                    {
                        throw new ArgumentException("Invalid wsdl document. <wsdl:operation>-tag is not closed in wsdl file");
                    }

                    i++;

                    /* handle and parse possible documentation tag */
                    if (p_a_wsdlTags[i].StartsWith("<documentation"))
                    {
                        s_portTypeDocumentation = WSDL.ParseDocumentationTag(p_a_wsdlTags[i]);
                        i++;
                    }

                    /* expect input tag */
                    if (p_a_wsdlTags[i].StartsWith("<input"))
                    {
                        /* read message tag */
                        o_regex = new("message=\"([^\"]*)\"");
                        o_matcher = o_regex.Match(p_a_wsdlTags[i]);

                        if (o_matcher.Success)
                        {
                            /* store input message */
                            s_inputMessage = o_matcher.Groups[0].Value.Substring(9, o_matcher.Groups[0].Value.Length - 1 - 9);

                            /* check if we find our message value in our message list */
                            foreach (Message o_message in this.Messages)
                            {
                                if (o_message.Name.Equals(s_inputMessage))
                                {
                                    o_inputMessage = o_message;
                                }
                            }

                            /* if message value does not exist in our message list, it is unknown */
                            if (o_inputMessage == null)
                            {
                                throw new ArgumentException("<wsdl:input>-tag has unknown message value '" + s_inputMessage + "'");
                            }
                        }
                        else
                        {
                            /* no message attribute found */
                            throw new ArgumentException("Invalid <wsdl:input>-tag without a message attribute");
                        }

                        if (p_a_wsdlTags[i].EndsWith("/>"))
                        { /* <input> tag was self closing */
                            i--;
                        }
                        else if (!WSDL.LookForEndTag(p_a_wsdlTags[i], p_a_wsdlTags, i + 1, i_endOfPortTypes))
                        { /* check if <input> tag is closed */
                            throw new ArgumentException("Invalid wsdl document. <wsdl:input>-tag is not closed in wsdl file");
                        }

                        i++;
                        i++;
                    }
                    else
                    {
                        /* no <wsdl:input>-tag found */
                        throw new ArgumentException("No <wsdl:input>-tag found");
                    }

                    /* expect output tag */
                    if (p_a_wsdlTags[i].StartsWith("<output"))
                    {
                        /* read message tag */
                        o_regex = new("message=\"([^\"]*)\"");
                        o_matcher = o_regex.Match(p_a_wsdlTags[i]);

                        if (o_matcher.Success)
                        {
                            /* store output message */
                            s_outputMessage = o_matcher.Groups[0].Value.Substring(9, o_matcher.Groups[0].Value.Length - 1 - 9);

                            /* check if we find our message value in our message list */
                            foreach (Message o_message in this.Messages)
                            {
                                if (o_message.Name.Equals(s_outputMessage))
                                {
                                    o_outputMessage = o_message;
                                }
                            }

                            /* if message value does not exist in our message list, it is unknown */
                            if (o_outputMessage == null)
                            {
                                throw new ArgumentException("<wsdl:output>-tag has unknown message value '" + s_outputMessage + "'");
                            }
                        }
                        else
                        {
                            /* no message attribute found */
                            throw new ArgumentException("Invalid <wsdl:output>-tag without a message attribute");
                        }

                        if (p_a_wsdlTags[i].EndsWith("/>"))
                        { /* <output> tag was self closing */
                            i--;
                        }
                        else if (!WSDL.LookForEndTag(p_a_wsdlTags[i], p_a_wsdlTags, i + 1, i_endOfPortTypes))
                        { /* check if <output> tag is closed */
                            throw new ArgumentException("Invalid wsdl document. <wsdl:output>-tag is not closed in wsdl file");
                        }

                        i++;
                        i++;
                    }
                    else
                    {
                        /* no <wsdl:output>-tag found */
                        throw new ArgumentException("No <wsdl:output>-tag found");
                    }
                }
                else
                {
                    /* no <wsdl:message>-tag found */
                    throw new ArgumentException("No <wsdl:operation>-tag found");
                }

                /* add portType object to our list, with name, operation name, input and output message */
                this.PortTypeOperations.Add(new PortTypeOperation(s_portTypeName, s_portTypeOperationName, o_inputMessage, o_outputMessage, s_portTypeDocumentation));
                /* store portType operation name as one of our SOAP operation which must be implemented */
                this.SOAPOperations[s_portTypeOperationName] = null;
            }

            /* return xml/xsd-tag position to continue parsing */
            return i_endOfPortTypes + 1;
        }

        /// <summary>
        /// parsing binding tags and all it's operation input and output tags
        /// </summary>
        /// <param name="p_a_wsdlTags">list of following xml/xsd-tags</param>
        /// <param name="p_i_min">xml/xsd-tag in parameter list where we start</param>
        /// <param name="p_i_max">xml/xsd-tag in parameter list where we stop</param>
        /// <exception cref="ArgumentException">invalid xml/xsd-tag found within wsdl file</exception>
        private int ParseBinding(List<string> p_a_wsdlTags, int p_i_min, int p_i_max)
        {
            int i_endOfBinding = -1;
            bool b_bindingEndingOnce = false;

            /* check if soap:binding has closed itself, so we can continue with wsdl:binding */
            if (p_a_wsdlTags[p_i_min + 1].EndsWith("/>"))
            {
                b_bindingEndingOnce = true;
            }

            /* find end of binding part */
            for (int i = p_i_min; i < p_i_max; i++)
            {
                /* found binding closing tag */
                if (p_a_wsdlTags[i].StartsWith("</binding"))
                {
                    /* ignore closed soap:binding tag */
                    if (!b_bindingEndingOnce)
                    {
                        b_bindingEndingOnce = true;
                        continue;
                    }

                    i_endOfBinding = i;
                    break;
                }
            }

            /* could not find end of <binding> part */
            if (i_endOfBinding < 0)
            {
                throw new ArgumentException("Invalid wsdl document. <wsdl:binding> tags could not be parsed");
            }

            string s_bindingName;
            string s_bindingType;
            List<PortTypeOperation> a_bindingPortTypeOperations = [];

            /* expect wsdl binding tag */
            if (p_a_wsdlTags[p_i_min].StartsWith("<binding"))
            {
                /* read name tag */
                System.Text.RegularExpressions.Regex o_regex = new("name=\"([^\"]*)\"");
                System.Text.RegularExpressions.Match o_matcher = o_regex.Match(p_a_wsdlTags[p_i_min]);

                if (o_matcher.Success)
                {
                    /* store binding name */
                    s_bindingName = o_matcher.Groups[0].Value.Substring(6, o_matcher.Groups[0].Value.Length - 1 - 6);
                }
                else
                {
                    /* no name attribute found */
                    throw new ArgumentException("Invalid <wsdl:binding>-tag without a name attribute");
                }

                /* read name tag */
                o_regex = new("type=\"([^\"]*)\"");
                o_matcher = o_regex.Match(p_a_wsdlTags[p_i_min]);

                if (o_matcher.Success)
                {
                    /* store binding type */
                    s_bindingType = o_matcher.Groups[0].Value.Substring(6, o_matcher.Groups[0].Value.Length - 1 - 6);
                    bool b_found = false;

                    /* check if binding type exists in our portType list */
                    foreach (PortTypeOperation o_foo in this.PortTypeOperations)
                    {
                        if (o_foo.PortTypeName.Equals(s_bindingType))
                        {
                            b_found = true;
                            break;
                        }
                    }

                    /* if binding type has not been found in our portType list */
                    if (!b_found)
                    {
                        throw new ArgumentException("<wsdl:binding>-tag has unknown type value '" + s_bindingType + "' within wsdl document");
                    }
                }
                else
                {
                    /* no type attribute found */
                    throw new ArgumentException("Invalid <wsdl:binding>-tag without a type attribute");
                }

                /* check if wsdl binding tag will be closed */
                if (!WSDL.LookForEndTag(p_a_wsdlTags[p_i_min], p_a_wsdlTags, p_i_min + 1, i_endOfBinding))
                {
                    throw new ArgumentException("Invalid wsdl document. <wsdl:binding>-tag is not closed in wsdl file");
                }
            }
            else
            {
                /* no <wsdl:binding>-tag found */
                throw new ArgumentException("No <wsdl:binding>-tag found");
            }

            /* expect soap binding tag */
            if (p_a_wsdlTags[p_i_min + 1].StartsWith("<binding"))
            {
                /* read name tag */
                System.Text.RegularExpressions.Regex o_regex = new("style=\"([^\"]*)\"");
                System.Text.RegularExpressions.Match o_matcher = o_regex.Match(p_a_wsdlTags[p_i_min + 1]);

                if (o_matcher.Success)
                {
                    string s_foo = o_matcher.Groups[0].Value.Substring(7, o_matcher.Groups[0].Value.Length - 1 - 7);

                    /* name attribute of soap binding must be 'document' */
                    if (!s_foo.Equals("document"))
                    {
                        throw new ArgumentException("Invalid wsdl document. <soap:binding>-tag has not a style attribute with value 'document', but '" + s_foo + "'");
                    }
                }
                else
                {
                    /* no style attribute found */
                    throw new ArgumentException("Invalid <wsdl:binding>-tag without a style attribute");
                }

                /* read transport tag */
                o_regex = new("transport=\"([^\"]*)\"");
                o_matcher = o_regex.Match(p_a_wsdlTags[p_i_min + 1]);

                if (o_matcher.Success)
                {
                    string s_foo = o_matcher.Groups[0].Value.Substring(11, o_matcher.Groups[0].Value.Length - 1 - 11);

                    /* transport attribute of soap binding must be 'http://schemas.xmlsoap.org/soap/http' */
                    if (!s_foo.Equals("http://schemas.xmlsoap.org/soap/http"))
                    {
                        throw new ArgumentException("Invalid wsdl document. <soap:binding>-tag has not a transport attribute with value 'http://schemas.xmlsoap.org/soap/http'");
                    }
                }
                else
                {
                    /* no transport attribute found */
                    throw new ArgumentException("Invalid <wsdl:binding>-tag without a transport attribute");
                }

                /* check if soap binding tag will be closed */
                if ((!p_a_wsdlTags[p_i_min + 1].EndsWith("/>")) && (!WSDL.LookForEndTag(p_a_wsdlTags[p_i_min + 1], p_a_wsdlTags, p_i_min + 2, i_endOfBinding)))
                {
                    throw new ArgumentException("Invalid wsdl document. <soap:binding>-tag is not closed in wsdl file");
                }
            }
            else
            {
                /* no <soap:binding>-tag found */
                throw new ArgumentException("No <soap:binding>-tag found");
            }

            int i_operationsStart = p_i_min + 2;

            /* check when next binding operation tag is starting */
            if (!p_a_wsdlTags[p_i_min + 1].EndsWith("/>"))
            {
                i_operationsStart++;
            }

            /* iterate following tags for wsdl operation and soap operation */
            for (int i = i_operationsStart; i < i_endOfBinding; i++)
            {
                string s_bindingOperationName;

                /* expect wsdl binding tag */
                if (p_a_wsdlTags[i].StartsWith("<operation"))
                {
                    /* read name tag */
                    System.Text.RegularExpressions.Regex o_regex = new("name=\"([^\"]*)\"");
                    System.Text.RegularExpressions.Match o_matcher = o_regex.Match(p_a_wsdlTags[i]);

                    if (o_matcher.Success)
                    {
                        /* store binding operation name */
                        s_bindingOperationName = o_matcher.Groups[0].Value.Substring(6, o_matcher.Groups[0].Value.Length - 1 - 6);

                        /* look for our portType operation with our binding operation name and add it to our binding list */
                        foreach (PortTypeOperation o_foo in this.PortTypeOperations)
                        {
                            if (o_foo.PortTypeOperationName.Equals(s_bindingOperationName))
                            {
                                a_bindingPortTypeOperations.Add(o_foo);
                            }
                        }
                    }
                    else
                    {
                        /* no name attribute found */
                        throw new ArgumentException("Invalid <wsdl:operation>-tag without a name attribute");
                    }

                    /* check if wsdl binding tag is closed */
                    if (!WSDL.LookForEndTag(p_a_wsdlTags[i], p_a_wsdlTags, i + 1, i_endOfBinding))
                    {
                        throw new ArgumentException("Invalid wsdl document. <wsdl:operation>-tag is not closed in wsdl file");
                    }

                    i++;
                }
                else
                {
                    /* no <wsdl:operation>-tag found */
                    throw new ArgumentException("No <wsdl:operation>-tag found");
                }

                /* expect soap operation tag */
                if (p_a_wsdlTags[i].StartsWith("<operation"))
                {
                    /* read name tag */
                    System.Text.RegularExpressions.Regex o_regex = new("style=\"([^\"]*)\"");
                    System.Text.RegularExpressions.Match o_matcher = o_regex.Match(p_a_wsdlTags[i]);

                    if (o_matcher.Success)
                    {
                        string s_foo = o_matcher.Groups[0].Value.Substring(7, o_matcher.Groups[0].Value.Length - 1 - 7);

                        /* soap operation tag attribute style must be 'document' */
                        if (!s_foo.Equals("document"))
                        {
                            throw new ArgumentException("Invalid wsdl document. <soap:operation>-tag has not a style attribute with value 'document', but '" + s_foo + "'");
                        }
                    }
                    else
                    {
                        /* no style attribute found */
                        throw new ArgumentException("Invalid <soap:operation>-tag without a style attribute");
                    }

                    if (p_a_wsdlTags[i].EndsWith("/>"))
                    { /* soap operation tag was self-closing */
                        i--;
                    }
                    else if (!WSDL.LookForEndTag(p_a_wsdlTags[i], p_a_wsdlTags, i + 1, i_endOfBinding))
                    { /* check if soap operation tag is closed */
                        throw new ArgumentException("Invalid wsdl document. <soap:operation>-tag is not closed in wsdl file");
                    }

                    i++;
                    i++;
                }
                else
                {
                    /* no <soap:operation>-tag found */
                    throw new ArgumentException("No <soap:operation>-tag found");
                }

                /* expect input tag */
                if (p_a_wsdlTags[i].StartsWith("<input"))
                {
                    /* check if input tag is closed */
                    if (!WSDL.LookForEndTag(p_a_wsdlTags[i], p_a_wsdlTags, i + 1, i_endOfBinding))
                    {
                        throw new ArgumentException("Invalid wsdl document. <wsdl:input>-tag is not closed in wsdl file");
                    }

                    i++;
                }
                else
                {
                    /* no <wsdl:input>-tag found */
                    throw new ArgumentException("No <wsdl:input>-tag found");
                }

                /* expect body tag */
                if (p_a_wsdlTags[i].StartsWith("<body"))
                {
                    /* read use tag */
                    System.Text.RegularExpressions.Regex o_regex = new("use=\"([^\"]*)\"");
                    System.Text.RegularExpressions.Match o_matcher = o_regex.Match(p_a_wsdlTags[i]);

                    if (o_matcher.Success)
                    {
                        string s_foo = o_matcher.Groups[0].Value.Substring(5, o_matcher.Groups[0].Value.Length - 1 - 5);

                        /* body tag use attribute must be 'literal' */
                        if (!s_foo.Equals("literal"))
                        {
                            throw new ArgumentException("Invalid wsdl document. <soap:body>-tag has not a use attribute with value 'literal', but '" + s_foo + "'");
                        }
                    }
                    else
                    {
                        /* no use attribute found */
                        throw new ArgumentException("Invalid <soap:body>-tag without a use attribute");
                    }

                    if (p_a_wsdlTags[i].EndsWith("/>"))
                    { /* body tag was self-closing */
                        i--;
                    }
                    else if (!WSDL.LookForEndTag(p_a_wsdlTags[i], p_a_wsdlTags, i + 1, i_endOfBinding))
                    { /* check if body tag is closed */
                        throw new ArgumentException("Invalid wsdl document. <soap:body>-tag is not closed in wsdl file");
                    }

                    i++;
                    i++;
                    i++;
                }
                else
                {
                    /* no <soap:body>-tag found */
                    throw new ArgumentException("No <soap:body>-tag found");
                }

                /* expect output tag */
                if (p_a_wsdlTags[i].StartsWith("<output"))
                {
                    /* check if output tag is closed */
                    if (!WSDL.LookForEndTag(p_a_wsdlTags[i], p_a_wsdlTags, i + 1, i_endOfBinding))
                    {
                        throw new ArgumentException("Invalid wsdl document. <wsdl:output>-tag is not closed in wsdl file");
                    }

                    i++;
                }
                else
                {
                    /* no <wsdl:output>-tag found */
                    throw new ArgumentException("No <wsdl:output>-tag found");
                }

                /* expect body tag */
                if (p_a_wsdlTags[i].StartsWith("<body"))
                {
                    /* read use tag */
                    System.Text.RegularExpressions.Regex o_regex = new("use=\"([^\"]*)\"");
                    System.Text.RegularExpressions.Match o_matcher = o_regex.Match(p_a_wsdlTags[i]);

                    if (o_matcher.Success)
                    {
                        string s_foo = o_matcher.Groups[0].Value.Substring(5, o_matcher.Groups[0].Value.Length - 1 - 5);

                        /* body tag use attribute must be 'literal' */
                        if (!s_foo.Equals("literal"))
                        {
                            throw new ArgumentException("Invalid wsdl document. <soap:body>-tag has not a use attribute with value 'literal', but '" + s_foo + "'");
                        }
                    }
                    else
                    {
                        /* no use attribute found */
                        throw new ArgumentException("Invalid <soap:body>-tag without a use attribute");
                    }

                    if (p_a_wsdlTags[i].EndsWith("/>"))
                    { /* body tag was self-closing */
                        i--;
                    }
                    else if (!WSDL.LookForEndTag(p_a_wsdlTags[i], p_a_wsdlTags, i + 1, i_endOfBinding))
                    { /* check if body tag is closed */
                        throw new ArgumentException("Invalid wsdl document. <soap:body>-tag is not closed in wsdl file");
                    }

                    i++;
                    i++;
                    i++;
                }
                else
                {
                    /* no <soap:body>-tag found */
                    throw new ArgumentException("No <soap:body>-tag found");
                }

                /* add new binding instance with name and portType operations list */
                this.Bindings.Add(new Binding(s_bindingName, a_bindingPortTypeOperations));
            }

            /* return xml/xsd-tag position to continue parsing */
            return i_endOfBinding + 1;
        }

        /// <summary>
        /// parsing servoce tag and all it's port tags
        /// </summary>
        /// <param name="p_a_wsdlTags">list of following xml/xsd-tags</param>
        /// <param name="p_i_min">xml/xsd-tag in parameter list where we start</param>
        /// <param name="p_i_max">xml/xsd-tag in parameter list where we stop</param>
        /// <exception cref="ArgumentException">invalid xml/xsd-tag found within wsdl file</exception>
        private void ParseService(List<string> p_a_wsdlTags, int p_i_min, int p_i_max)
        {
            string s_serviceName;
            string? s_serviceDocumentation = null;
            List<ServicePort> a_servicePorts = [];

            /* expect service tag */
            if (p_a_wsdlTags[p_i_min].StartsWith("<service"))
            {
                /* read name tag */
                System.Text.RegularExpressions.Regex o_regex = new("name=\"([^\"]*)\"");
                System.Text.RegularExpressions.Match o_matcher = o_regex.Match(p_a_wsdlTags[p_i_min]);

                if (o_matcher.Success)
                {
                    /* store service name */
                    s_serviceName = o_matcher.Groups[0].Value.Substring(6, o_matcher.Groups[0].Value.Length - 1 - 6);
                }
                else
                {
                    /* no name attribute found */
                    throw new ArgumentException("Invalid <wsdl:service>-tag without a name attribute");
                }

                /* check if service tag is closed */
                if (!WSDL.LookForEndTag(p_a_wsdlTags[p_i_min], p_a_wsdlTags, p_i_min + 1, p_i_max))
                {
                    throw new ArgumentException("Invalid wsdl document. <wsdl:service>-tag is not closed in wsdl file");
                }
            }
            else
            {
                /* no <wsdl:service>-tag found */
                throw new ArgumentException("No <wsdl:service>-tag found");
            }

            /* iterate all following tags */
            for (int i = p_i_min + 1; i < p_i_max; i++)
            {
                string s_servicePortName;
                string s_servicePortBinding;
                string s_serviceAddressLocation;
                Binding? o_serviceBinding = null;

                /* abort for loop if closing service tag is found */
                if (p_a_wsdlTags[i].Equals("</service>"))
                {
                    break;
                }

                /* parse optional documentation tag */
                if (p_a_wsdlTags[i].StartsWith("<documentation"))
                {
                    s_serviceDocumentation = WSDL.ParseDocumentationTag(p_a_wsdlTags[i]);
                    i++;
                }

                /* expect port tag */
                if (p_a_wsdlTags[i].StartsWith("<port"))
                {
                    /* read name tag */
                    System.Text.RegularExpressions.Regex o_regex = new("name=\"([^\"]*)\"");
                    System.Text.RegularExpressions.Match o_matcher = o_regex.Match(p_a_wsdlTags[i]);

                    if (o_matcher.Success)
                    {
                        /* store service port name */
                        s_servicePortName = o_matcher.Groups[0].Value.Substring(6, o_matcher.Groups[0].Value.Length - 1 - 6);
                    }
                    else
                    {
                        /* no name attribute found */
                        throw new ArgumentException("Invalid <wsdl:port>-tag without a name attribute");
                    }

                    /* read binding tag */
                    o_regex = new("binding=\"([^\"]*)\"");
                    o_matcher = o_regex.Match(p_a_wsdlTags[i]);

                    if (o_matcher.Success)
                    {
                        /* store service port binding name */
                        s_servicePortBinding = o_matcher.Groups[0].Value.Substring(9, o_matcher.Groups[0].Value.Length - 1 - 9);

                        /* check our binding list if we find service port binding name there */
                        foreach (Binding o_binding in this.Bindings)
                        {
                            if (o_binding.Name.Equals(s_servicePortBinding))
                            {
                                o_serviceBinding = o_binding;
                                break;
                            }
                        }

                        /* service port binding not found, although we have parsed binding part */
                        if (o_serviceBinding == null)
                        {
                            throw new ArgumentException("<wsdl_port>-tag has unknown binding value '" + s_servicePortBinding + "' within wsdl document");
                        }
                    }
                    else
                    {
                        /* no binding attribute found */
                        throw new ArgumentException("Invalid <wsdl:port>-tag without a binding attribute");
                    }

                    /* check if port tag is closed */
                    if (!WSDL.LookForEndTag(p_a_wsdlTags[i], p_a_wsdlTags, i + 1, p_i_max))
                    {
                        throw new ArgumentException("Invalid wsdl document. <wsdl:port>-tag is not closed in wsdl file");
                    }

                    i++;
                }
                else
                {
                    /* no <wsdl:port>-tag found */
                    throw new ArgumentException("No <wsdl:port>-tag found");
                }

                /* expect address tag */
                if (p_a_wsdlTags[i].StartsWith("<address"))
                {
                    /* read location tag */
                    System.Text.RegularExpressions.Regex o_regex = new("location=\"([^\"]*)\"");
                    System.Text.RegularExpressions.Match o_matcher = o_regex.Match(p_a_wsdlTags[i]);

                    if (o_matcher.Success)
                    {
                        /* store service address location */
                        s_serviceAddressLocation = o_matcher.Groups[0].Value.Substring(10, o_matcher.Groups[0].Value.Length - 1 - 10);
                    }
                    else
                    {
                        /* no location attribute found */
                        throw new ArgumentException("Invalid <soap:address>-tag without a location attribute");
                    }

                    if (p_a_wsdlTags[i].EndsWith("/>"))
                    { /* address tag was self-closing */
                        i--;
                    }
                    else if (!WSDL.LookForEndTag(p_a_wsdlTags[i], p_a_wsdlTags, i + 1, p_i_max))
                    { /* check if address tag is closed */
                        throw new ArgumentException("Invalid wsdl document. <soap:address>-tag is not closed in wsdl file");
                    }

                    i++;
                    i++;
                }
                else
                {
                    /* no <soap:address>-tag found */
                    throw new ArgumentException("No <soap:address>-tag found");
                }

                /* add new service port instance to list */
                a_servicePorts.Add(new ServicePort(s_servicePortName, s_serviceAddressLocation, o_serviceBinding));
            }

            /* store new service instane with documentation(optional) and service port list */
            this.ServiceInstance = new(s_serviceName, s_serviceDocumentation, a_servicePorts);
        }

        /// <summary>
        /// set SOAP operation with key name and SOAP operation interface. key names are defined by PortType Operation name
        /// </summary>
        /// <param name="p_s_soapOperation">key value of SOAP operation</param>
        /// <param name="p_o_soi">SOAP operation interface, which implements a method which can be executed</param>
        /// <exception cref="ArgumentNullException">key value or SOAP operation interface parameter are null or empty</exception>
        /// <exception cref="ArgumentException">SOAP operation does not exist in WSDL list</exception>
        public void AddSOAPOperation(string? p_s_soapOperation, SOAPOperationShell? p_o_soi)
        {
            /* check if key parameter is not null or empty */
            if ((p_s_soapOperation == null) || (ForestNET.Lib.Helper.IsStringEmpty(p_s_soapOperation)))
            {
                throw new ArgumentNullException(nameof(p_s_soapOperation), "SOAP operation name parameter is empty or null");
            }

            /* check if SOAP operation interface parameter is not null */
            if (p_o_soi == null)
            {
                throw new ArgumentNullException(nameof(p_o_soi), "SOAP operation parameter is null");
            }

            /* check if key value already exists in SOAP operation map, origin is PortType Operation names in WSDL */
            if (!this.SOAPOperations.ContainsKey(p_s_soapOperation))
            {
                throw new ArgumentException("SOAP operation does not exist in WSDL list");
            }

            /* add SOAP operation with it's key value */
            this.SOAPOperations[p_s_soapOperation] = p_o_soi;
        }

        /// <summary>
        /// check if message name exists in our port type operation list
        /// </summary>
        /// <param name="p_s_inputMessagePartElementValue">input message name from incoming/outgoing SOAP message</param>
        /// <exception cref="ArgumentNullException">input message part element value parameter is null or empty</exception>
        public bool ContainsOperationByInputMessagePartElementValue(string p_s_inputMessagePartElementValue)
        {
            /* check input message part element value parameter */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_inputMessagePartElementValue))
            {
                throw new ArgumentNullException(nameof(p_s_inputMessagePartElementValue), "input message part element value parameter is empty or null");
            }

            /* iterate each portType operation */
            foreach (PortTypeOperation o_operation in this.PortTypeOperations)
            {
                /* input message part element value parameter must match with operation input message name */
                if ((o_operation.InputMessage.PartElement ?? "").Equals(p_s_inputMessagePartElementValue))
                {
                    /* found portType operation */
                    return true;
                }
            }

            /* portType operation not found by input message part element value */
            return false;
        }

        /// <summary>
        /// return portType operation object with input message part element value from our port type operation list
        /// </summary>
        /// <param name="p_s_inputMessagePartElementValue">input message part element value from incoming/outgoing SOAP message</param>
        /// <exception cref="ArgumentNullException">input message part element value parameter is null or empty</exception>
        public PortTypeOperation? GetOperationByInputMessagePartElementValue(string p_s_inputMessagePartElementValue)
        {
            /* check input message part element value parameter */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_inputMessagePartElementValue))
            {
                throw new ArgumentNullException(nameof(p_s_inputMessagePartElementValue), "input message part element value parameter is empty or null");
            }

            /* iterate each portType operation */
            foreach (PortTypeOperation o_operation in this.PortTypeOperations)
            {
                /* input message part element value parameter must match with operation input message name */
                if ((o_operation.InputMessage.PartElement ?? "").Equals(p_s_inputMessagePartElementValue))
                {
                    /* return found portType operation */
                    return o_operation;
                }
            }

            /* portType operation not found by input message part element value */
            return null;
        }

        /* Internal Classes */

        /// <summary>
        /// Encapsulation of wsdl message object and tostring method for debug usage
        /// </summary>
        public class Message
        {

            /* Fields */

            /* Properties */

            public string Name { get; set; }
            public string PartName { get; set; }
            public string? PartElement { get; set; }

            /* Methods */

            public Message(string p_s_name, string p_s_partElement) :
                this(p_s_name, p_s_partElement, null)
            {

            }

            public Message(string p_s_name, string p_s_partName, string? p_s_partElement)
            {
                this.Name = p_s_name;
                this.PartName = p_s_partName;
                this.PartElement = p_s_partElement;
            }

            public override string ToString()
            {
                string s_foo = "\t\t\t\t";

                s_foo += "MessageName = " + this.Name;
                s_foo += " | PartName = " + this.PartName;
                s_foo += " | PartElement = " + this.PartElement;

                return s_foo;
            }
        }

        /// <summary>
        /// Encapsulation of wsdl portType operation object and tostring method for debug usage
        /// </summary>
        public class PortTypeOperation
        {

            /* Fields */

            /* Properties */

            public string PortTypeName { get; set; }
            public string PortTypeOperationName { get; set; }
            public string? Documentation { get; set; }
            public Message InputMessage { get; set; }
            public Message OutputMessage { get; set; }

            /* Methods */

            public PortTypeOperation(string p_s_portTypeName, string p_s_portTypeOperationName, Message p_o_inputMessage, Message p_o_outputMessage) :
                this(p_s_portTypeName, p_s_portTypeOperationName, p_o_inputMessage, p_o_outputMessage, null)
            {

            }

            public PortTypeOperation(string p_s_portTypeName, string p_s_portTypeOperationName, Message p_o_inputMessage, Message p_o_outputMessage, string? p_s_documentation)
            {
                this.PortTypeName = p_s_portTypeName;
                this.PortTypeOperationName = p_s_portTypeOperationName;
                this.InputMessage = p_o_inputMessage;
                this.OutputMessage = p_o_outputMessage;
                this.Documentation = p_s_documentation;
            }

            public override string ToString()
            {
                string s_foo = "\t\t\t";

                s_foo += "PortTypeName = " + this.PortTypeName;
                s_foo += " | PortTypeOperationName = " + this.PortTypeOperationName;
                s_foo += " | Documentation = " + this.Documentation;
                s_foo += " | InputMessage = [" + ForestNET.Lib.IO.File.NEWLINE;
                s_foo += this.InputMessage.ToString();
                s_foo += ForestNET.Lib.IO.File.NEWLINE + "\t\t\t" + "]";
                s_foo += " | OutputMessage = [" + ForestNET.Lib.IO.File.NEWLINE;
                s_foo += this.OutputMessage.ToString();
                s_foo += ForestNET.Lib.IO.File.NEWLINE + "\t\t\t" + "]" + ForestNET.Lib.IO.File.NEWLINE;

                return s_foo;
            }
        }

        /// <summary>
        /// Encapsulation of wsdl binding object and tostring method for debug usage
        /// </summary>
        public class Binding
        {

            /* Fields */

            /* Properties */

            public string Name { get; set; }
            public List<PortTypeOperation> Operations { get; set; }

            /* Methods */

            public Binding(string p_s_name, List<PortTypeOperation> p_a_operations)
            {
                this.Name = p_s_name;
                this.Operations = p_a_operations;
            }

            public override string ToString()
            {
                string s_foo = "\t\t";

                s_foo += "BindingName = " + this.Name;
                s_foo += " | PortTypeOperations = [" + ForestNET.Lib.IO.File.NEWLINE;

                foreach (PortTypeOperation o_foo in this.Operations)
                {
                    s_foo += o_foo.ToString();
                }

                s_foo += ForestNET.Lib.IO.File.NEWLINE + "\t\t" + "]";

                return s_foo;
            }
        }

        /// <summary>
        /// Encapsulation of wsdl service object and tostring method for debug usage
        /// </summary>
        public class Service
        {

            /* Fields */

            /* Properties */

            public string Name { get; set; }
            public string? Documentation { get; set; }
            public List<ServicePort> ServicePorts { get; set; }

            /* Methods */

            public Service(string p_s_name, string? p_s_documentation, List<ServicePort> p_a_servicePorts)
            {
                this.Name = p_s_name;
                this.Documentation = p_s_documentation;
                this.ServicePorts = p_a_servicePorts;
            }

            public override string ToString()
            {
                string s_foo = "";

                s_foo += "ServiceName = " + this.Name;
                s_foo += " | Documentation = " + this.Documentation;
                s_foo += " | ServicePorts = [" + ForestNET.Lib.IO.File.NEWLINE;

                foreach (ServicePort o_foo in this.ServicePorts)
                {
                    s_foo += o_foo.ToString() + ForestNET.Lib.IO.File.NEWLINE;
                }

                s_foo += ForestNET.Lib.IO.File.NEWLINE + "]";

                return s_foo;
            }
        }

        /// <summary>
        /// Encapsulation of wsdl service port object and tostring method for debug usage
        /// </summary>
        public class ServicePort
        {

            /* Fields */

            /* Properties */

            public string Name { get; set; }
            public string AddressLocation { get; set; }
            public Binding Binding { get; set; }

            /* Methods */

            public ServicePort(string p_s_name, string p_s_addressLocation, Binding p_o_binding)
            {
                this.Name = p_s_name;
                this.AddressLocation = p_s_addressLocation;
                this.Binding = p_o_binding;
            }

            public override string ToString()
            {
                string s_foo = "\t";

                s_foo += "ServicePortName = " + this.Name;
                s_foo += " | AddressLocation = " + this.AddressLocation;
                s_foo += " | Binding = [" + ForestNET.Lib.IO.File.NEWLINE + this.Binding.ToString() + ForestNET.Lib.IO.File.NEWLINE + "\t" + "]";

                return s_foo;
            }
        }
    }
}