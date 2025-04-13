namespace ForestNET.Lib.IO
{
    /// <summary>
    /// XML class to encode and decode c# objects to xml files with help of a xsd schema file/data.
    /// access to object fields can be directly on public fields or with public property methods (getXX setXX) on private fields.
    /// NOTE: mostly only primitive types supported for encoding and decoding.
    /// </summary>
    public class XML
    {

        /* Fields */

        private XSDElement? o_currentElement;
        private List<XSDElement> a_elementDefinitons;
        private List<XSDAttribute> a_attributeDefinitions;
        private List<KeyValuePair<int, int>> a_temp;
        private List<XSDElement> a_dividedElements;
        private string s_lineBreak = ForestNET.Lib.IO.File.NEWLINE;
        private int i_level;
        private bool b_printEmptystring;
        private bool b_useISO8601UTC;
        private bool b_ignoreMapping;
        private string s_dateTimeFormat = "dd.MM.yyyy HH:mm:ss";
        private string s_dateFormat = "dd.MM.yyyy";
        private string s_timeFormat = "HH:mm:ss";

        /* Properties */

        public XSDElement? Root
        {
            get; set;
        }

        /// <summary>
        /// Determine line break characters for reading and writing xml files
        /// </summary>
        public string LineBreak
        {
            get
            {
                return this.s_lineBreak;
            }
            set
            {
                if (value.Length < 1)
                {
                    throw new ArgumentException("Line break must have at least a length of 1, but length is '" + value.Length + "'");
                }

                this.s_lineBreak = value;
                ForestNET.Lib.Global.ILogConfig("updated line break to [" + ForestNET.Lib.Helper.BytesToHexString(System.Text.Encoding.UTF8.GetBytes(this.s_lineBreak), true) + "]");
            }
        }

        /// <summary>
        /// Determine if properties shall be used handling Objects.
        /// </summary>
        public bool UseProperties
        {
            get; set;
        }

        /// <summary>
        /// If a string value is empty for a xml-element, set '&#x200B;' (zero-width space) as it's value
        /// </summary>
        public bool PrintEmptystring
        {
            get
            {
                return this.b_printEmptystring;
            }
            set
            {
                this.b_printEmptystring = value;
                ForestNET.Lib.Global.ILogConfig("updates print empty string to '" + this.b_printEmptystring + "'");
            }
        }

        /// <summary>
        /// Determine if you want to use only ISO 8601 UTC timestamps within xml files
        /// </summary>
        public bool UseISO8601UTC
        {
            get
            {
                return this.b_useISO8601UTC;
            }
            set
            {
                this.b_useISO8601UTC = value;
                ForestNET.Lib.Global.ILogConfig("updates use ISO 8601 UTC to '" + this.b_useISO8601UTC + "'");
            }
        }

        /// <summary>
        /// Determine if you want to ignore required mapping attribute values to xsd-tags
        /// </summary>
        public bool IgnoreMapping
        {
            get
            {
                return this.b_ignoreMapping;
            }
            set
            {
                this.b_ignoreMapping = value;
                ForestNET.Lib.Global.ILogConfig("updates ignore mapping to '" + this.b_ignoreMapping + "'");
            }
        }

        /// <summary>
        /// Determine date time format if not using ISO 8601 UTC
        /// </summary>
        public string DateTimeFormat
        {
            get
            {
                return this.s_dateTimeFormat;
            }
            set
            {
                this.s_dateTimeFormat = value;
                ForestNET.Lib.Global.ILogConfig("updates date time format to '" + this.s_dateTimeFormat + "'");
            }
        }

        /// <summary>
        /// Determine date format if not using ISO 8601 UTC
        /// </summary>
        public string DateFormat
        {
            get
            {
                return this.s_dateFormat;
            }
            set
            {
                this.s_dateFormat = value;
                ForestNET.Lib.Global.ILogConfig("updates date format to '" + this.s_dateFormat + "'");
            }
        }

        /// <summary>
        /// Determine time format if not using ISO 8601 UTC
        /// </summary>
        public string TimeFormat
        {
            get
            {
                return this.s_timeFormat;
            }
            set
            {
                this.s_timeFormat = value;
                ForestNET.Lib.Global.ILogConfig("updates time format to '" + this.s_timeFormat + "'");
            }
        }

        public string? TargetNamespace { get; private set; }

        /* Methods */

        /// <summary>
        /// Empty XML constructor
        /// </summary>
        public XML()
        {
            this.a_elementDefinitons = [];
            this.a_attributeDefinitions = [];
            this.a_temp = [];
            this.a_dividedElements = [];
            this.LineBreak = ForestNET.Lib.IO.File.NEWLINE;
            this.UseProperties = false;
            this.PrintEmptystring = true;
            this.UseISO8601UTC = true;
            this.IgnoreMapping = false;
            this.DateTimeFormat = "dd.MM.yyyy HH:mm:ss";
            this.DateFormat = "dd.MM.yyyy";
            this.TimeFormat = "HH:mm:ss";

            this.i_level = 0;
            this.TargetNamespace = null;
        }

        /// <summary>
        /// XML constructor, giving a schema xsd element object as schema for encoding and decoding xml data
        /// </summary>
        /// <param name="p_o_schemaRoot">xsd element object as root schema node</param>
        /// <exception cref="ArgumentException">invalid parameters for constructor</exception>
        public XML(XSDElement p_o_schemaRoot)
        {
            this.a_elementDefinitons = [];
            this.a_attributeDefinitions = [];
            this.a_temp = [];
            this.a_dividedElements = [];
            this.LineBreak = ForestNET.Lib.IO.File.NEWLINE;
            this.UseProperties = false;
            this.PrintEmptystring = true;
            this.UseISO8601UTC = true;
            this.IgnoreMapping = false;
            this.DateTimeFormat = "dd.MM.yyyy HH:mm:ss";
            this.DateFormat = "dd.MM.yyyy";
            this.TimeFormat = "HH:mm:ss";

            this.i_level = 0;
            this.TargetNamespace = null;

            this.Root = p_o_schemaRoot ?? throw new ArgumentException("xsd element parameter for schema is null");
        }

        /// <summary>
        /// XML constructor, giving file lines of a schema file as dynamic list for encoding and decoding xml data
        /// </summary>
        /// <param name="p_a_xsdSchemaLines">file lines of schema as dynamic list</param>
        /// <exception cref="ArgumentException">value/structure within xsd schema file invalid</exception>
        /// <exception cref="ArgumentNullException">xsd schema, root node is null</exception>
        /// <exception cref="System.IO.IOException">cannot access or open xsd file and it's content</exception>
        public XML(List<string> p_a_xsdSchemaLines)
        {
            this.a_elementDefinitons = [];
            this.a_attributeDefinitions = [];
            this.a_temp = [];
            this.a_dividedElements = [];
            this.LineBreak = ForestNET.Lib.IO.File.NEWLINE;
            this.UseProperties = false;
            this.PrintEmptystring = true;
            this.UseISO8601UTC = true;
            this.IgnoreMapping = false;
            this.DateTimeFormat = "dd.MM.yyyy HH:mm:ss";
            this.DateFormat = "dd.MM.yyyy";
            this.TimeFormat = "HH:mm:ss";

            this.i_level = 0;
            this.TargetNamespace = null;

            /* read all xsd-schema file lines as xsd schema */
            this.SetSchema(p_a_xsdSchemaLines);
        }

        /// <summary>
        /// XML constructor, giving a schema file as orientation for encoding and decoding xml data
        /// </summary>
        /// <param name="p_s_file">full-path to xsd schema file</param>
        /// <exception cref="ArgumentException">value/structure within xsd schema file invalid</exception>
        /// <exception cref="NullReferenceException">xsd schema, root node is null</exception>
        /// <exception cref="System.IO.IOException">cannot access or open xsd file and it's content</exception>
        public XML(string p_s_file)
        {
            this.a_elementDefinitons = [];
            this.a_attributeDefinitions = [];
            this.a_temp = [];
            this.a_dividedElements = [];
            this.LineBreak = ForestNET.Lib.IO.File.NEWLINE;
            this.UseProperties = false;
            this.PrintEmptystring = true;
            this.UseISO8601UTC = true;
            this.IgnoreMapping = false;
            this.DateTimeFormat = "dd.MM.yyyy HH:mm:ss";
            this.DateFormat = "dd.MM.yyyy";
            this.TimeFormat = "HH:mm:ss";

            this.i_level = 0;
            this.TargetNamespace = null;

            /* check if file exists */
            if (!File.Exists(p_s_file))
            {
                throw new ArgumentException("File[" + p_s_file + "] does not exist.");
            }

            /* open xsd-schema file */
            File o_file = new(p_s_file, false);

            /* load file content into string */
            string s_fileContent = o_file.FileContent;

            List<string> a_fileLines =
            [
                /* load file content lines into array */
                .. s_fileContent.Split(this.s_lineBreak),
            ];

            /* read all xsd-schema file lines as xsd schema */
            this.SetSchema(a_fileLines);
        }

        /// <summary>
        /// Method to set schema elements, afterwards each xml constructor has read their input
        /// </summary>
        private void SetSchema(List<string> p_a_xsdSchemaLines)
        {
            System.Text.StringBuilder o_stringBuilder = new();

            /* read all xsd schema file lines to one string builder */
            foreach (string s_line in p_a_xsdSchemaLines)
            {
                o_stringBuilder.Append(s_line);
            }

            /* read all xsd-schema file lines and delete all line-wraps and tabs and values only containing white spaces */
            string s_xsd = o_stringBuilder.ToString();
            s_xsd = new System.Text.RegularExpressions.Regex("[\\r\\n\\t]").Replace(s_xsd, "");
            s_xsd = new System.Text.RegularExpressions.Regex(">\\s*<").Replace(s_xsd, "><");

            /* clean up xsd-schema */
            s_xsd = new System.Text.RegularExpressions.Regex("<\\?(.*?)\\?>").Replace(s_xsd, "");
            s_xsd = new System.Text.RegularExpressions.Regex("<!--(.*?)-->").Replace(s_xsd, "");

            /* look for targetNamespace in schema-tag */
            if (s_xsd.StartsWith("<xs:schema"))
            {
                string s_foo = s_xsd.Substring(0, s_xsd.IndexOf(">"));

                if (s_foo.Contains("targetnamespace=\""))
                {
                    s_foo = s_foo.Substring(s_foo.IndexOf("targetnamespace=\"") + 17);
                    s_foo = s_foo.Substring(0, s_foo.IndexOf("\""));

                    this.TargetNamespace = s_foo;
                }
                else if (s_foo.Contains("targetNamespace=\""))
                {
                    s_foo = s_foo.Substring(s_foo.IndexOf("targetNamespace=\"") + 17);
                    s_foo = s_foo.Substring(0, s_foo.IndexOf("\""));

                    this.TargetNamespace = s_foo;
                }
            }

            /* remove schema and annotation tags */
            s_xsd = new System.Text.RegularExpressions.Regex("<xs:schema(.*?)>").Replace(s_xsd, "");
            s_xsd = new System.Text.RegularExpressions.Regex("</xs:schema>").Replace(s_xsd, "");
            s_xsd = new System.Text.RegularExpressions.Regex("<xs:annotation>(.*?)</xs:annotation>").Replace(s_xsd, "");

            if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("cleaned up xsd-schema: " + this.s_lineBreak + s_xsd);

            /* validate xsd */
            System.Text.RegularExpressions.Regex o_regex = new("(<[^<>]*?<[^<>]*?>|<[^<>]*?>[^<>]*?>)");
            System.Text.RegularExpressions.MatchCollection o_matcher = o_regex.Matches(s_xsd);

            /* if regex-matcher has match, the xsd-schema is not valid */
            if (o_matcher.Count > 0)
            {
                throw new ArgumentException("Invalid xsd-schema. Please check xsd-schema at \"" + o_matcher[0] + "\".");
            }

            ForestNET.Lib.Global.ILogConfig("split xsd-schema into xsd-elements");

            List<string> a_xsdTags = [];

            /* add all xsd-schema-tags to a list for parsing */
            o_regex = new System.Text.RegularExpressions.Regex("<[^<>]*?>");
            o_matcher = o_regex.Matches(s_xsd);

            if (o_matcher.Count > 0)
            {
                for (int i = 0; i < o_matcher.Count; i++)
                {
                    a_xsdTags.Add(o_matcher[i].ToString());
                }
            }

            /* check if xsd-schema starts with xs:element */
            if ((!a_xsdTags[0].StartsWith("<xs:element", StringComparison.CurrentCultureIgnoreCase)) && (!a_xsdTags[0].StartsWith("<xs:complextype", StringComparison.CurrentCultureIgnoreCase)))
            {
                throw new ArgumentException("xsd-schema must start with <xs:element>-tag or with <xs:complexType>-tag.");
            }

            /* difference between xsd-schema-tree and xsd-schema-divided-definitions */
            if (!a_xsdTags[0].EndsWith("/>"))
            {
                ParseXSDSchemaTree(a_xsdTags, 0, a_xsdTags.Count - 1, XSDType.Element);
                ForestNET.Lib.Global.ILogConfig("parsed xsd-schema as xsd-schema-tree");
            }
            else
            {
                ParseXSDSchemaDivided(a_xsdTags, 0, a_xsdTags.Count - 1);
                ForestNET.Lib.Global.ILogConfig("parsed xsd-schema as xsd-schema-divided-definitions");
            }

            /* check if root is null */
            if (this.Root == null)
            {
                throw new NullReferenceException("Root node is null");
            }

            /* check if root has any children */
            if (this.Root.Children.Count == 0)
            {
                throw new NullReferenceException("Root node has no children[size=" + this.Root.Children.Count + "]");
            }

            if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("xsd-schema: " + this.s_lineBreak + this.Root);
        }

        /// <summary>
        /// Returns root element of xsd schema as string output and all of its children
        /// </summary>
        override public string ToString()
        {
            string s_foo = "";

            s_foo += this.Root?.ToString();

            return s_foo;
        }

        /// <summary>
        /// Generate indentation string for xml specification
        /// </summary>
        /// <returns>indentation string</returns>
        private string PrintIndentation()
        {
            string s_foo = "";

            for (int i = 0; i < this.i_level; i++)
            {
                s_foo += "\t";
            }

            return s_foo;
        }

        /* parsing XSD schema */

        /// <summary>
        /// Analyze xsd element value and get a unique value type from it
        /// </summary>
        /// <param name="p_a_xsdTags">xsd element value as string</param>
        /// <param name="p_i_line">line number of xsd element</param>
        /// <returns>unique xsd value type</returns>
        /// <exception cref="ArgumentException">invalid value or xsd value type could not be determined</exception>
        private static XSDType GetXSDType(List<string> p_a_xsdTags, int p_i_line)
        {
            XSDType e_xsdTagType = XSDType.RestrictionItem;

            /* get xsd type */
            if (p_a_xsdTags[p_i_line].Contains("<xs:element "))
            {
                e_xsdTagType = XSDType.Element;
            }
            else if ((p_a_xsdTags[p_i_line].Contains("<xs:complexType")) || (p_a_xsdTags[p_i_line].Contains("<xs:complextype")))
            {
                e_xsdTagType = XSDType.ComplexType;
            }
            else if (p_a_xsdTags[p_i_line].Contains("<xs:sequence"))
            {
                e_xsdTagType = XSDType.Sequence;
            }
            else if (p_a_xsdTags[p_i_line].Contains("xs:attribute "))
            {
                e_xsdTagType = XSDType.Attribute;
            }
            else if (p_a_xsdTags[p_i_line].Contains("<xs:choice"))
            {
                e_xsdTagType = XSDType.Choice;
            }
            else if ((p_a_xsdTags[p_i_line].Contains("<xs:simpleType")) || (p_a_xsdTags[p_i_line].Contains("<xs:simpletype")))
            {
                e_xsdTagType = XSDType.SimpleType;
            }
            else if ((p_a_xsdTags[p_i_line].Contains("<xs:simpleContent")) || (p_a_xsdTags[p_i_line].Contains("<xs:simplecontent")))
            {
                e_xsdTagType = XSDType.SimpleContent;
            }
            else if (p_a_xsdTags[p_i_line].Contains("<xs:restriction"))
            {
                e_xsdTagType = XSDType.Restriction;
            }
            else if (p_a_xsdTags[p_i_line].Contains("<xs:extension"))
            {
                e_xsdTagType = XSDType.Extension;
            }
            else if (
                (p_a_xsdTags[p_i_line].Contains("<xs:minExclusive")) ||
                (p_a_xsdTags[p_i_line].Contains("<xs:maxExclusive")) ||
                (p_a_xsdTags[p_i_line].Contains("<xs:minInclusive")) ||
                (p_a_xsdTags[p_i_line].Contains("<xs:maxInclusive")) ||
                (p_a_xsdTags[p_i_line].Contains("<xs:totalDigits")) ||
                (p_a_xsdTags[p_i_line].Contains("<xs:fractionDigits")) ||
                (p_a_xsdTags[p_i_line].Contains("<xs:length")) ||
                (p_a_xsdTags[p_i_line].Contains("<xs:minLength")) ||
                (p_a_xsdTags[p_i_line].Contains("<xs:maxLength")) ||
                (p_a_xsdTags[p_i_line].Contains("<xs:enumeration")) ||
                (p_a_xsdTags[p_i_line].Contains("<xs:whiteSpace")) ||
                (p_a_xsdTags[p_i_line].Contains("<xs:pattern"))
            )
            {
                e_xsdTagType = XSDType.RestrictionItem;
            }
            else if (p_a_xsdTags[p_i_line].Contains("/xs:element>"))
            {
                e_xsdTagType = XSDType.Element;
            }
            else if ((p_a_xsdTags[p_i_line].Contains("/xs:complexType>")) || (p_a_xsdTags[p_i_line].Contains("/xs:complextype>")))
            {
                e_xsdTagType = XSDType.ComplexType;
            }
            else if (p_a_xsdTags[p_i_line].Contains("/xs:sequence>"))
            {
                e_xsdTagType = XSDType.Sequence;
            }
            else if (p_a_xsdTags[p_i_line].Contains("/xs:attribute>"))
            {
                e_xsdTagType = XSDType.Attribute;
            }
            else if (p_a_xsdTags[p_i_line].Contains("/xs:choice>"))
            {
                e_xsdTagType = XSDType.Choice;
            }
            else if ((p_a_xsdTags[p_i_line].Contains("/xs:simpleType>")) || (p_a_xsdTags[p_i_line].Contains("/xs:simpletype>")))
            {
                e_xsdTagType = XSDType.SimpleType;
            }
            else if ((p_a_xsdTags[p_i_line].Contains("/xs:simpleContent>")) || (p_a_xsdTags[p_i_line].Contains("/xs:simplecontent>")))
            {
                e_xsdTagType = XSDType.SimpleContent;
            }
            else if (p_a_xsdTags[p_i_line].Contains("/xs:restriction>"))
            {
                e_xsdTagType = XSDType.Restriction;
            }
            else if (p_a_xsdTags[p_i_line].Contains("/xs:extension>"))
            {
                e_xsdTagType = XSDType.Extension;
            }

            return e_xsdTagType;
        }

        /// <summary>
        /// Parse xsd content to xsd object structure as schema tree view, based on XSDElement, XSDAttribute and XSDRestriction
        /// </summary>
        /// <param name="p_a_xsdTags">list of xsd element tags</param>
        /// <param name="p_i_min">pointer where to start line iteration</param>
        /// <param name="p_i_max">pointer where to end line iteration</param>
        /// <param name="p_e_xsdParentTagType">xsd type of parent xsd element</param>
        /// <exception cref="ArgumentException">value or structure within xsd file lines invalid</exception>
        /// <exception cref="ArgumentNullException">value within xsd content missing or min. amount not available</exception>
        private void ParseXSDSchemaTree(List<string> p_a_xsdTags, int p_i_min, int p_i_max, XSDType p_e_xsdParentTagType)
        {
            XSDType e_xsdTagTypeBefore = XSDType.RestrictionItem;
            int i_max = p_i_max;
            bool b_parsed = false;
            bool b_oneLinerBefore = false;

            /* iterate all elements */
            for (int i_min = p_i_min; i_min <= i_max; i_min++)
            {
                /* get xsd type */
                XSDType e_xsdTagType = XML.GetXSDType(p_a_xsdTags, i_min);

                bool b_simpleType = false;
                bool b_simpleContent = false;

                /* we found a xs:element, xs:complexType or xs:simpleType with interlacing */
                if (!p_a_xsdTags[i_min].EndsWith("/>"))
                {
                    /* check if we have a simpleType or a simpleContent */
                    if ((e_xsdTagType == XSDType.Element) || (e_xsdTagType == XSDType.ComplexType) || (e_xsdTagType == XSDType.SimpleType))
                    {
                        /* conditions for simpleType */
                        if ((e_xsdTagType == XSDType.SimpleType) || (XML.GetXSDType(p_a_xsdTags, i_min + 1) == XSDType.SimpleType))
                        {
                            b_simpleType = true;
                        }

                        /* conditions for simpleContent */
                        if ((XML.GetXSDType(p_a_xsdTags, i_min + 1) == XSDType.SimpleContent) || ((XML.GetXSDType(p_a_xsdTags, i_min + 1) == XSDType.ComplexType) && (XML.GetXSDType(p_a_xsdTags, i_min + 2) == XSDType.SimpleContent)))
                        {
                            b_simpleContent = true;
                        }
                    }

                    ForestNET.Lib.Global.ILogFiner(p_a_xsdTags[i_min] + " ... parsed=" + b_parsed + " ... current=" + e_xsdTagType + " ... before=" + e_xsdTagTypeBefore + " ... parent=" + p_e_xsdParentTagType + " ... oneLinerBefore=" + b_oneLinerBefore);

                    /* if we have another xs:element with interlacing we may need another recursion, if it was already parsed and its not simpleType and not simpleContent */
                    if ((b_parsed) && ((e_xsdTagType == XSDType.Element) || (((e_xsdTagTypeBefore != XSDType.Element) || (b_oneLinerBefore)) && (e_xsdTagType == XSDType.ComplexType))) && (!b_simpleType) && (!b_simpleContent))
                    {
                        int i_oldMax = i_max;
                        int i_tempMin = i_min + 1;
                        int i_level = 0;

                        /* look for end of nested xs:element tag */
                        while (
                                ((e_xsdTagType == XSDType.Element) && (!p_a_xsdTags[i_tempMin].ToLower().Equals("</xs:element>"))) ||
                                ((e_xsdTagType == XSDType.ComplexType) && (!p_a_xsdTags[i_tempMin].ToLower().Equals("</xs:complextype>"))) ||
                                (i_level != 0)
                            )
                        {
                            if (e_xsdTagType == XSDType.Element)
                            {
                                /* handle other interlacing in current nested xs:element tag */
                                if ((p_a_xsdTags[i_tempMin].StartsWith("<xs:element", StringComparison.CurrentCultureIgnoreCase)) && (!p_a_xsdTags[i_tempMin].EndsWith("/>")))
                                {
                                    i_level++;
                                }
                                else if (p_a_xsdTags[i_tempMin].ToLower().Equals("</xs:element>"))
                                {
                                    i_level--;
                                }
                            }
                            else if (e_xsdTagType == XSDType.ComplexType)
                            {
                                /* handle other interlacing in current nested xs:complexType tag */
                                if ((p_a_xsdTags[i_tempMin].StartsWith("<xs:complextype", StringComparison.CurrentCultureIgnoreCase)) && (!p_a_xsdTags[i_tempMin].EndsWith("/>")))
                                {
                                    i_level++;
                                }
                                else if (p_a_xsdTags[i_tempMin].ToLower().Equals("</xs:complextype>"))
                                {
                                    i_level--;
                                }
                            }

                            if (i_tempMin == i_max)
                            {
                                /* forbidden state - interlacing is not valid in xsd-schema */
                                throw new ArgumentException("Invalid nested xsd-tag xs:element at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                            }

                            i_tempMin++;
                        }

                        /* save current element to reset it after recursion */
                        XSDElement? o_oldCurrentElement = this.o_currentElement;

                        ForestNET.Lib.Global.ILogFiner("interlacing with parent=" + e_xsdTagType);
                        ForestNET.Lib.Global.ILogFiner(i_min + " ... " + i_tempMin);
                        ForestNET.Lib.Global.ILogFiner(p_a_xsdTags[i_min] + " ... " + p_a_xsdTags[i_tempMin]);

                        /* parse interlacing recursive */
                        ParseXSDSchemaTree(p_a_xsdTags, i_min, i_tempMin, e_xsdTagType);

                        /* reset current element */
                        this.o_currentElement = o_oldCurrentElement;

                        i_min = i_tempMin;
                        i_max = i_oldMax;

                        continue;
                    }

                    /* overwrite value for end tag pointer */
                    int i_nestedMax = -1;

                    /* store attributes of complex element type until sequence end tag was found */
                    if (e_xsdTagType == XSDType.Sequence)
                    {
                        while (!p_a_xsdTags[i_max].ToLower().Equals("</xs:sequence>"))
                        {
                            if (p_a_xsdTags[i_max].StartsWith("<xs:attribute", StringComparison.CurrentCultureIgnoreCase))
                            {
                                ForestNET.Lib.Global.ILogFinest("\t\tAttribute: " + i_max + " to Current Element: " + this.o_currentElement?.Name + " - - - parent: " + p_e_xsdParentTagType);
                                ForestNET.Lib.Global.ILogFinest("\t\tAttribute: " + p_a_xsdTags[i_max] + " to Current Element: " + this.o_currentElement?.Name + " - - - parent: " + p_e_xsdParentTagType);

                                /* parse xs:attribute */
                                XSDAttribute o_xsdAttribute;

                                /* check if we have an attribute with simpleType */
                                if (XML.GetXSDType(p_a_xsdTags, i_max + 1) == XSDType.SimpleType)
                                {
                                    o_xsdAttribute = this.ParseXSDAttributeWithSimpleType(p_a_xsdTags, i_max);
                                }
                                else
                                {
                                    o_xsdAttribute = this.ParseXSDAttribute(p_a_xsdTags, i_max);
                                }

                                /* add xs:attribute to current element */
                                this.o_currentElement?.Attributes.Add(o_xsdAttribute);
                            }

                            i_max--;
                        }
                    }
                    else if (e_xsdTagType == XSDType.Choice)
                    { /* handle choice */
                        ForestNET.Lib.Global.ILogFiner("\tChoice: " + i_min + " to Current Element: " + this.o_currentElement?.Name);
                        ForestNET.Lib.Global.ILogFiner("\tChoice: " + p_a_xsdTags[i_min] + " to Current Element: " + this.o_currentElement?.Name);

                        /* parse xs:choice */
                        XSDElement o_xsdChoice = XML.ParseXSDChoice(p_a_xsdTags, i_min);

                        /* check that current element is not null */
                        if (this.o_currentElement == null)
                        {
                            throw new NullReferenceException("Cannot set choice flag if current element is 'null'");
                        }

                        /* set choice flag for current element */
                        this.o_currentElement.Choice = true;

                        /* set minOccurs for current element */
                        if (o_xsdChoice.MinOccurs != 1)
                        {
                            this.o_currentElement.MinOccurs = o_xsdChoice.MinOccurs;
                        }

                        /* set maxOccurs for current element */
                        if (o_xsdChoice.MaxOccurs != 1)
                        {
                            this.o_currentElement.MaxOccurs = o_xsdChoice.MaxOccurs;
                        }
                    }
                    else if (b_simpleType)
                    { /* handle simpleType */
                        ForestNET.Lib.Global.ILogFiner("\tSimpleType: " + i_min + " to Current Element: " + this.o_currentElement?.Name);
                        ForestNET.Lib.Global.ILogFiner("\tSimpleType: " + p_a_xsdTags[i_min] + p_a_xsdTags[i_min + 1] + " to Current Element: " + this.o_currentElement?.Name);

                        /* parse simpleType */
                        i_nestedMax = this.ParseXSDSimpleType(p_a_xsdTags, i_min, i_max);

                        /* set xml tag pointer to skip processed simpleType tag */
                        i_min = i_nestedMax;

                        ForestNET.Lib.Global.ILogFiner("after");
                        ForestNET.Lib.Global.ILogFiner("\t" + i_min + " ... " + i_max);
                        ForestNET.Lib.Global.ILogFiner("\t" + p_a_xsdTags[i_min] + " ... " + p_a_xsdTags[i_max]);
                    }
                    else if (b_simpleContent)
                    {  /* handle simpleContent */
                        ForestNET.Lib.Global.ILogFiner("\tSimpleContent: " + i_min + " to Current Element: " + this.o_currentElement?.Name);
                        ForestNET.Lib.Global.ILogFiner("\tSimpleContent: " + p_a_xsdTags[i_min] + p_a_xsdTags[i_min + 1] + p_a_xsdTags[i_min + 2] + " to Current Element: " + this.o_currentElement?.Name);

                        /* parse simpleContent */
                        i_nestedMax = this.ParseXSDSimpleContent(p_a_xsdTags, i_min, i_max);

                        /* set xml tag pointer to skip processed simpleContent tag */
                        i_min = i_nestedMax;

                        ForestNET.Lib.Global.ILogFiner("after");
                        ForestNET.Lib.Global.ILogFiner("\t" + i_min + " ... " + i_max);
                        ForestNET.Lib.Global.ILogFiner("\t" + p_a_xsdTags[i_min] + " ... " + p_a_xsdTags[i_max]);
                    }

                    /* set end tag pointer */
                    int i_endTagPointer = i_max;

                    /* overwrite end tag pointer if we had a nested simpleType or simpleContent */
                    if (i_nestedMax > 0)
                    {
                        i_endTagPointer = i_nestedMax;
                    }

                    ForestNET.Lib.Global.ILogFiner("endTagPointer");
                    ForestNET.Lib.Global.ILogFiner("\t\t" + i_min + " ... " + i_endTagPointer);
                    ForestNET.Lib.Global.ILogFiner("\t\t" + p_a_xsdTags[i_min] + " ... " + p_a_xsdTags[i_endTagPointer]);

                    ForestNET.Lib.Global.ILogFiner("after");
                    ForestNET.Lib.Global.ILogFiner("\t" + i_min + " ... " + i_max);
                    ForestNET.Lib.Global.ILogFiner("\t" + p_a_xsdTags[i_min] + " ... " + p_a_xsdTags[i_max]);

                    /* if we still have no closing tag, then our xsd schema is invalid */
                    if (!p_a_xsdTags[i_endTagPointer].StartsWith("</"))
                    {
                        throw new ArgumentException("Invalid xsd-tag(" + p_a_xsdTags[i_endTagPointer] + ") is not closed in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                    }

                    /* get xsd end type */
                    XSDType e_xsdEndTagType = XML.GetXSDType(p_a_xsdTags, i_endTagPointer);

                    /* xsd type and xsd close type must match */
                    if (e_xsdTagType != e_xsdEndTagType)
                    {
                        throw new ArgumentException("Invalid xsd-tag-type(" + p_a_xsdTags[i_endTagPointer] + ") for closing in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                    }
                    else
                    {
                        /* if we had a no nested simpleType or simpleContent, decrease xml end tag pointer */
                        if (i_nestedMax < 0)
                        {
                            i_max--;
                        }
                    }

                    b_oneLinerBefore = false;
                }
                else
                {
                    b_oneLinerBefore = true;
                }

                ForestNET.Lib.Global.ILogFiner(p_a_xsdTags[i_min] + " ... parsed=" + b_parsed + " ... current=" + e_xsdTagType + " ... before=" + e_xsdTagTypeBefore + " ... parent=" + p_e_xsdParentTagType + " ... oneLinerBefore=" + b_oneLinerBefore);

                /* if tag is of type element, complex type or attribute if parent type is complex */
                if (((e_xsdTagType == XSDType.Element) || (((e_xsdTagTypeBefore != XSDType.Element) || (b_oneLinerBefore)) && (e_xsdTagType == XSDType.ComplexType)) || ((p_e_xsdParentTagType == XSDType.ComplexType) && (e_xsdTagType == XSDType.Attribute))) && (!b_simpleType) && (!b_simpleContent))
                {
                    if (((e_xsdTagType == XSDType.Element) || (((e_xsdTagTypeBefore != XSDType.Element) || (b_oneLinerBefore)) && (e_xsdTagType == XSDType.ComplexType))))
                    {
                        if (!p_a_xsdTags[i_min].EndsWith("/>"))
                        {
                            ForestNET.Lib.Global.ILogFiner("\tnew Current Element: " + i_min);
                            ForestNET.Lib.Global.ILogFiner("\tnew Current Element: " + p_a_xsdTags[i_min]);

                            /* parse xs:element */
                            XSDElement o_xsdElement = this.ParseXSDElement(p_a_xsdTags, i_min, true);

                            /* if current element is set, add xs:element as child to current element */
                            if (this.o_currentElement != null)
                            {
                                ForestNET.Lib.Global.ILogFiner("\t\tto Current Element: " + this.o_currentElement.Name);

                                this.o_currentElement.Children.Add(o_xsdElement);
                            }

                            /* set xs:element as current element */
                            this.o_currentElement = o_xsdElement;

                            /* set root if its is null */
                            if (this.Root == null)
                            {
                                this.Root = this.o_currentElement;
                            }
                        }
                        else
                        {
                            ForestNET.Lib.Global.ILogFiner("\tElement: " + i_min + " to Current Element: " + this.o_currentElement?.Name);
                            ForestNET.Lib.Global.ILogFiner("\tElement: " + p_a_xsdTags[i_min] + " to Current Element: " + this.o_currentElement?.Name);

                            /* parse xs:element */
                            XSDElement o_xsdElement = this.ParseXSDElement(p_a_xsdTags, i_min);

                            /* library does not support multiple occurrences of the same xml element without a list definition */
                            if (o_xsdElement.MaxOccurs > 1)
                            {
                                throw new ArgumentException("Library does not support multiple occurrences of the same xml element without a list definition in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                            }

                            /* add xs:element to current element */
                            this.o_currentElement?.Children.Add(o_xsdElement);
                        }
                    }
                    else if (e_xsdTagType == XSDType.Attribute)
                    {
                        ForestNET.Lib.Global.ILogFiner("\t\tAttribute: " + i_min + " to Current Element: " + this.o_currentElement?.Name + " - - - parent: " + p_e_xsdParentTagType);
                        ForestNET.Lib.Global.ILogFiner("\t\tAttribute: " + p_a_xsdTags[i_min] + " to Current Element: " + this.o_currentElement?.Name + " - - - parent: " + p_e_xsdParentTagType);

                        /* parse xs:attribute */
                        XSDAttribute o_xsdAttribute;

                        /* check if we have an attribute with simpleType */
                        if (XML.GetXSDType(p_a_xsdTags, i_min + 1) == XSDType.SimpleType)
                        {
                            o_xsdAttribute = this.ParseXSDAttributeWithSimpleType(p_a_xsdTags, i_min);
                        }
                        else
                        {
                            o_xsdAttribute = this.ParseXSDAttribute(p_a_xsdTags, i_min);
                        }

                        /* add xs:attribute to current element */
                        this.o_currentElement?.Attributes.Add(o_xsdAttribute);
                    }
                }
                else if ((p_e_xsdParentTagType != XSDType.ComplexType) && (e_xsdTagType == XSDType.Attribute))
                {
                    /* if parent type is not complex but we have an attribute tag, it is invalid */
                    throw new ArgumentException("Invalid xsd-attribute-tag in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                }

                /* save parent xsd type */
                ForestNET.Lib.Global.ILogFiner("\tset tag before: " + e_xsdTagType + " - - - old before: " + e_xsdTagTypeBefore);

                e_xsdTagTypeBefore = e_xsdTagType;

                b_parsed = true;
            }
        }

        /// <summary>
        /// Parse xsd content to xsd object structure as schema with divided definitions, based on XSDElement, XSDAttribute and XSDRestriction
        /// </summary>
        /// <param name="p_a_xsdTags">list of xsd element tags</param>
        /// <param name="p_i_min">pointer where to start line iteration</param>
        /// <param name="p_i_max">pointer where to end line iteration</param>
        /// <exception cref="ArgumentException">value or structure within xsd file lines invalid</exception>
        /// <exception cref="ArgumentNullException">value within xsd content missing or min. amount not available</exception>
        private void ParseXSDSchemaDivided(List<string> p_a_xsdTags, int p_i_min, int p_i_max)
        {
            int i_max = p_i_max;

            /* get all element and attribute definitions of xsd schema */
            p_i_min = ParseXSDSchemaDividedDefinitions(p_a_xsdTags, p_i_min);

            /* iterate all elements */
            for (int i_min = p_i_min; i_min <= i_max; i_min++)
            {
                /* get xsd type */
                XSDType e_xsdTagType = XML.GetXSDType(p_a_xsdTags, i_min);

                /* first xsd tag must be of type element or complex type and not close itself */
                if (!p_a_xsdTags[i_min].EndsWith("/>"))
                {
                    /* first xsd tag is type xs:element or xs:complexType */
                    if ((e_xsdTagType == XSDType.Element) || (e_xsdTagType == XSDType.ComplexType))
                    {
                        /* parse xs:element */
                        XSDElement o_xsdElement = this.ParseXSDElement(p_a_xsdTags, i_min, true);

                        /* add element definition to list */
                        this.a_elementDefinitons.Add(o_xsdElement);

                        /* set found element as current element */
                        this.o_currentElement = o_xsdElement;

                        int i_oldMax = i_max;
                        int i_tempMin = i_min + 1;
                        int i_level = 0;

                        /* look for end of nested xs:element tag */
                        while (
                            ((e_xsdTagType == XSDType.Element) && (!p_a_xsdTags[i_tempMin].ToLower().Equals("</xs:element>"))) ||
                            ((e_xsdTagType == XSDType.ComplexType) && (!p_a_xsdTags[i_tempMin].ToLower().Equals("</xs:complextype>"))) ||
                            (i_level != 0)
                        )
                        {
                            if (e_xsdTagType == XSDType.Element)
                            {
                                /* handle other interlacing in current nested xs:element tag */
                                if ((p_a_xsdTags[i_tempMin].StartsWith("<xs:element", StringComparison.CurrentCultureIgnoreCase)) && (!p_a_xsdTags[i_tempMin].EndsWith("/>")))
                                {
                                    i_level++;
                                }
                                else if (p_a_xsdTags[i_tempMin].ToLower().Equals("</xs:element>"))
                                {
                                    i_level--;
                                }
                            }
                            else if (e_xsdTagType == XSDType.ComplexType)
                            {
                                /* handle other interlacing in current nested xs:complexType tag */
                                if ((p_a_xsdTags[i_tempMin].StartsWith("<xs:complextype", StringComparison.CurrentCultureIgnoreCase)) && (!p_a_xsdTags[i_tempMin].EndsWith("/>")))
                                {
                                    i_level++;
                                }
                                else if (p_a_xsdTags[i_tempMin].ToLower().Equals("</xs:complextype>"))
                                {
                                    i_level--;
                                }
                            }

                            if (i_tempMin == i_max)
                            {
                                /* forbidden state - interlacing is not valid in xsd-schema */
                                throw new ArgumentException("Invalid nested xsd-tag xs:element at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                            }

                            i_tempMin++;
                        }

                        ForestNET.Lib.Global.ILogFiner("interlacing");
                        ForestNET.Lib.Global.ILogFiner(i_min + " ... " + i_tempMin);
                        ForestNET.Lib.Global.ILogFiner(p_a_xsdTags[i_min] + " ... " + p_a_xsdTags[i_tempMin]);

                        /* parse found complex xs:element or xs:complexType tag in detail */
                        ParseXSDSchemaDividedElement(p_a_xsdTags, i_min, i_tempMin);

                        i_min = i_tempMin;
                        i_max = i_oldMax;
                        continue;
                    }

                    /* decrease xml end tag counter until sequence end tag was found */
                    if (e_xsdTagType == XSDType.Sequence)
                    {
                        while (!p_a_xsdTags[i_max].ToLower().Equals("</xs:sequence>"))
                        {
                            i_max--;
                        }
                    }

                    /* if we still have no closing tag, then our xsd schema is invalid */
                    if (!p_a_xsdTags[i_max].StartsWith("</"))
                    {
                        throw new ArgumentException("Invalid xsd-tag is not closed in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                    }

                    /* get xsd end type */
                    XSDType e_xsdEndTagType = XML.GetXSDType(p_a_xsdTags, i_max);

                    /* xsd type and xsd close type must match */
                    if (e_xsdTagType != e_xsdEndTagType)
                    {
                        throw new ArgumentException("Invalid xsd-tag-type for closing in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                    }
                    else
                    {
                        i_max--;
                    }
                }
                else
                {
                    /* other one line xsd-tags are not allowed in divided xsd-schema */
                    throw new ArgumentException("Invalid xsd-tag in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                }
            }

            this.Root = this.o_currentElement;
        }

        /// <summary>
        /// Parse xsd definitions within schema, based on XSDElement, XSDAttribute and XSDRestriction
        /// </summary>
        /// <param name="p_a_xsdTags">list of xsd element tags</param>
        /// <param name="p_i_min">pointer where to start line iteration</param>
        /// <exception cref="ArgumentException">value or structure within xsd file lines invalid</exception>
        /// <exception cref="ArgumentNullException">value within xsd content missing or min. amount not available</exception>
        private int ParseXSDSchemaDividedDefinitions(List<string> p_a_xsdTags, int p_i_min)
        {
            int i_return_min = p_i_min;
            bool b_definitionElementsClosed = false;

            /* iterate all elements */
            for (int i_min = p_i_min; i_min <= p_a_xsdTags.Count - 1; i_min++)
            {
                /* get xsd type */
                XSDType e_xsdTagType = XML.GetXSDType(p_a_xsdTags, i_min);

                /* xsd tags must close themselves, otherwise we have no more definitions */
                if (p_a_xsdTags[i_min].EndsWith("/>"))
                {
                    if ((e_xsdTagType == XSDType.Element) && (!b_definitionElementsClosed))
                    {
                        ForestNET.Lib.Global.ILogFiner("add element reference of: " + p_a_xsdTags[i_min]);

                        /* parse xs:element */
                        XSDElement o_xsdElement = this.ParseXSDElement(p_a_xsdTags, i_min);

                        /* library does not support multiple occurrences of the same xml element without a list definition */
                        if (o_xsdElement.MaxOccurs > 1)
                        {
                            throw new ArgumentException("Library does not support multiple occurrences of the same xml element without a list definition in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                        }

                        /* check if xs:element definition already exists */
                        if (this.XsdElementDefinitionExist(o_xsdElement.Name))
                        {
                            throw new ArgumentException("Invalid xsd-element-definition (duplicate) in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                        }

                        /* add element definition to list */
                        this.a_elementDefinitons.Add(o_xsdElement);
                    }

                    if (e_xsdTagType == XSDType.Attribute)
                    {
                        ForestNET.Lib.Global.ILogFiner("add attribute reference of: " + p_a_xsdTags[i_min]);

                        /* parse xs:attribute */
                        XSDAttribute o_xsdAttribute = this.ParseXSDAttribute(p_a_xsdTags, i_min);

                        /* check if xs:attribute definition already exists */
                        if (this.XsdAttributeDefinitionExist(o_xsdAttribute.Name))
                        {
                            throw new ArgumentException("Invalid xsd-attribute-definition (duplicate) in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                        }

                        /* add attribute definition to list */
                        this.a_attributeDefinitions.Add(o_xsdAttribute);
                    }
                    else
                    {
                        if (b_definitionElementsClosed)
                        {
                            throw new ArgumentException("Invalid xsd-attribute-definition in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                        }
                    }

                    /* check if elements definitions are closed and attribute definitions are found */
                    if ((e_xsdTagType == XSDType.Attribute) && (!b_definitionElementsClosed))
                    {
                        b_definitionElementsClosed = true;
                    }
                }
                else if ((!p_a_xsdTags[i_min].EndsWith("/>")) && (e_xsdTagType == XSDType.Element) && (XML.GetXSDType(p_a_xsdTags, i_min + 1) == XSDType.SimpleType))
                { /* handle element definition with simpleType */
                    ForestNET.Lib.Global.ILogFiner("add element reference with simpleType of: " + p_a_xsdTags[i_min]);

                    /* parse xs:element with simpleType */
                    XSDElement o_xsdElement = this.ParseXSDElementWithSimpleType(p_a_xsdTags, i_min);

                    /* library does not support multiple occurrences of the same xml element without a list definition */
                    if (o_xsdElement.MaxOccurs > 1)
                    {
                        throw new ArgumentException("Library does not support multiple occurrences of the same xml element without a list definition in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                    }

                    /* check if xs:element definition already exists */
                    if (this.XsdElementDefinitionExist(o_xsdElement.Name))
                    {
                        throw new ArgumentException("Invalid xsd-element-definition (duplicate) in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                    }

                    /* add element definition to list */
                    this.a_elementDefinitons.Add(o_xsdElement);

                    /* end tag pointer for nested xs:element */
                    int i_tempMax = i_min;

                    /* find end of element interlacing */
                    while (!p_a_xsdTags[i_tempMax].StartsWith("</xs:element>", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (i_tempMax == p_i_min + 100000)
                        {
                            /* forbidden state - interlacing is not valid in xsd-schema */
                            throw new ArgumentException("Invalid nested xsd-tag xs:restriction at(" + (p_i_min + 1) + ".-element) \"" + p_a_xsdTags[p_i_min] + "\".");
                        }

                        i_tempMax++;
                    }

                    /* set new xsd tag pointer to skip xs:element interlacing */
                    i_min = i_tempMax;
                }
                else if ((!p_a_xsdTags[i_min].EndsWith("/>")) && (e_xsdTagType == XSDType.Attribute) && (XML.GetXSDType(p_a_xsdTags, i_min + 1) == XSDType.SimpleType))
                { /* handle attribute definition with simpleType */
                    ForestNET.Lib.Global.ILogFiner("add attribute reference with simpleType of: " + p_a_xsdTags[i_min]);

                    /* parse xs:attribute with simpleType */
                    XSDAttribute o_xsdAttribute = this.ParseXSDAttributeWithSimpleType(p_a_xsdTags, i_min);

                    /* check if xs:attribute definition already exists */
                    if (this.XsdAttributeDefinitionExist(o_xsdAttribute.Name))
                    {
                        throw new ArgumentException("Invalid xsd-attribute-definition (duplicate) in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                    }

                    /* add attribute definition to list */
                    this.a_attributeDefinitions.Add(o_xsdAttribute);

                    /* end tag pointer for nested xs:attribute */
                    int i_tempMax = i_min;

                    /* find end of attribute interlacing */
                    while (!p_a_xsdTags[i_tempMax].StartsWith("</xs:attribute>", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (i_tempMax == p_i_min + 100000)
                        {
                            /* forbidden state - interlacing is not valid in xsd-schema */
                            throw new ArgumentException("Invalid nested xsd-tag xs:restriction at(" + (p_i_min + 1) + ".-element) \"" + p_a_xsdTags[p_i_min] + "\".");
                        }

                        i_tempMax++;
                    }

                    /* set new xsd tag pointer to skip xs:attribute interlacing */
                    i_min = i_tempMax;
                }
                else
                {
                    /* no more definitions available */
                    i_return_min = i_min;
                    break;
                }
            }

            return i_return_min;
        }

        /// <summary>
        /// Parse xsd content after divided definitions has been parsed, based on XSDElement, XSDAttribute and XSDRestriction
        /// </summary>
        /// <param name="p_a_xsdTags">list of xsd element tags</param>
        /// <param name="p_i_min">pointer where to start line iteration</param>
        /// <param name="p_i_max">pointer where to end line iteration</param>
        /// <exception cref="ArgumentException">value or structure within xsd file lines invalid</exception>
        /// <exception cref="ArgumentNullException">value within xsd content missing or min. amount not available</exception>
        private void ParseXSDSchemaDividedElement(List<string> p_a_xsdTags, int p_i_min, int p_i_max)
        {
            int i_max = p_i_max;
            bool b_simpleContent = false;
            bool b_simpleType = false;

            /* iterate all elements */
            for (int i_min = p_i_min; i_min <= i_max; i_min++)
            {
                /* get xsd type */
                XSDType e_xsdTagType = XML.GetXSDType(p_a_xsdTags, i_min);

                /* check for xs:simpleContent and xs:simpleType */
                if (((e_xsdTagType == XSDType.ComplexType) && (XML.GetXSDType(p_a_xsdTags, i_min + 1) == XSDType.SimpleContent)) ||
                    ((e_xsdTagType == XSDType.Element) && (XML.GetXSDType(p_a_xsdTags, i_min + 1) == XSDType.ComplexType) && (XML.GetXSDType(p_a_xsdTags, i_min + 2) == XSDType.SimpleContent)))
                {
                    b_simpleContent = true;
                }

                if (((e_xsdTagType == XSDType.Element) && (XML.GetXSDType(p_a_xsdTags, i_min + 1) == XSDType.SimpleType)) || (e_xsdTagType == XSDType.SimpleType))
                {
                    b_simpleType = true;
                }

                if (!p_a_xsdTags[i_min].EndsWith("/>"))
                {
                    /* first xsd tag is type xs:element or xs:complexType */
                    if (((e_xsdTagType == XSDType.Element) && (!b_simpleContent) && (!b_simpleType)) || ((e_xsdTagType == XSDType.ComplexType) && (!b_simpleContent)))
                    {
                        i_max--;
                        continue;
                    }

                    /* overwrite value for end tag pointer */
                    int i_nestedMax = -1;

                    ForestNET.Lib.Global.ILogFiner("before");
                    ForestNET.Lib.Global.ILogFiner("\t" + i_min + " ... " + i_max);
                    ForestNET.Lib.Global.ILogFiner("\t" + p_a_xsdTags[i_min] + " ... " + p_a_xsdTags[i_max]);

                    /* save attributes in complex type until sequence end tag found */
                    if (e_xsdTagType == XSDType.Sequence)
                    {
                        while (!p_a_xsdTags[i_max].ToLower().Equals("</xs:sequence>"))
                        {
                            if (p_a_xsdTags[i_max].StartsWith("<xs:attribute", StringComparison.CurrentCultureIgnoreCase))
                            {
                                System.Text.RegularExpressions.Regex o_regex = new("ref=\"([^\"]*)\"");
                                System.Text.RegularExpressions.Match o_matcherRef = o_regex.Match(p_a_xsdTags[i_max]);

                                o_regex = new System.Text.RegularExpressions.Regex("name=\"([^\"]*)\"");
                                System.Text.RegularExpressions.Match o_matcherName = o_regex.Match(p_a_xsdTags[i_max]);

                                o_regex = new System.Text.RegularExpressions.Regex("type=\"([^\"]*)\"");
                                System.Text.RegularExpressions.Match o_matcherType = o_regex.Match(p_a_xsdTags[i_max]);

                                if (o_matcherRef.Success)
                                { /* we have an attribute element with reference */
                                    string s_referenceName = o_matcherRef.Value.Substring(5, o_matcherRef.Value.Length - 1 - 5);

                                    /* check if we have a duplicate */
                                    if (this.GetXSDAttribute(s_referenceName) != null)
                                    {
                                        throw new ArgumentNullException("Invalid xsd-tag xs:attribute (duplicate) at(" + (i_max + 1) + ".-element) \"" + p_a_xsdTags[i_max] + "\".");
                                    }

                                    if (this.XsdAttributeDefinitionExist(s_referenceName))
                                    {
                                        ForestNET.Lib.Global.ILogFinest("add and check attribute reference = " + s_referenceName);

                                        XSDAttribute o_xsdAttribute = this.GetXSDAttributeDefinition(s_referenceName) ?? throw new NullReferenceException("xsd attribute definition not found with reference '" + s_referenceName + "'");

                                        /* read name attribute out of xs:attribute tag */
                                        if (o_matcherName.Success)
                                        {
                                            string s_name = o_matcherName.Value.Substring(6, o_matcherName.Value.Length - 1 - 6);

                                            /* overwrite name value from reference, because it is dominant from the usage point */
                                            o_xsdAttribute.Name = s_name;
                                        }

                                        /* add xs:attribute object to current element */
                                        this.o_currentElement?.Attributes.Add(o_xsdAttribute);
                                    }
                                    else
                                    {
                                        throw new ArgumentException("Invalid xsd-tag xs:attribute with unknown reference at(" + (i_max + 1) + ".-element) \"" + p_a_xsdTags[i_max] + "\".");
                                    }
                                }
                                else if ((o_matcherName.Success) && (o_matcherType.Success))
                                { /* we have an attribute element with name and type */
                                    /* read name and type attribute values of xs:attribute tag */
                                    string s_name = o_matcherName.Value.Substring(6, o_matcherName.Value.Length - 1 - 6);
                                    string s_type = o_matcherType.Value.Substring(6, o_matcherType.Value.Length - 1 - 6);

                                    /* check if we have a duplicate */
                                    if (this.GetXSDAttribute(s_type) != null)
                                    {
                                        throw new ArgumentNullException("Invalid xsd-tag xs:attribute (duplicate) at(" + (i_max + 1) + ".-element) \"" + p_a_xsdTags[i_max] + "\".");
                                    }

                                    if (this.XsdAttributeDefinitionExist(s_type))
                                    {
                                        ForestNET.Lib.Global.ILogFinest("add and check attribute type reference = " + s_type);

                                        XSDAttribute o_xsdAttribute = this.GetXSDAttributeDefinition(s_type) ?? throw new NullReferenceException("xsd attribute definition not found with type '" + s_type + "'");

                                        /* overwrite name value from reference, because it is dominant from the usage point */
                                        o_xsdAttribute.Name = s_name;

                                        /* add xs:attribute object to current element */
                                        this.o_currentElement?.Attributes.Add(o_xsdAttribute);
                                    }
                                    else
                                    {
                                        throw new ArgumentException("Invalid xsd-tag xs:attribute with unknown type reference at(" + (i_max + 1) + ".-element) \"" + p_a_xsdTags[i_max] + "\".");
                                    }
                                }
                                else
                                {
                                    throw new ArgumentNullException("Invalid xsd-tag xs:attribute without a reference at(" + (i_max + 1) + ".-element) \"" + p_a_xsdTags[i_max] + "\".");
                                }
                            }

                            i_max--;
                        }
                    }
                    else if (e_xsdTagType == XSDType.Choice)
                    { /* identify choice tag for current element */
                        ForestNET.Lib.Global.ILogFiner("\tChoice: " + i_min + " to Current Element: " + this.o_currentElement?.Name);
                        ForestNET.Lib.Global.ILogFiner("\tChoice: " + p_a_xsdTags[i_min] + " to Current Element: " + this.o_currentElement?.Name);

                        /* parse xs:choice */
                        XSDElement o_xsdChoice = XML.ParseXSDChoice(p_a_xsdTags, i_min);

                        /* check that current element is not null */
                        if (this.o_currentElement == null)
                        {
                            throw new NullReferenceException("Cannot set choice flag if current element is 'null'");
                        }

                        /* set choice flag for current element */
                        this.o_currentElement.Choice = true;

                        /* set minOccurs for current element */
                        if (o_xsdChoice.MinOccurs != 1)
                        {
                            this.o_currentElement.MinOccurs = o_xsdChoice.MinOccurs;
                        }

                        /* set maxOccurs for current element */
                        if (o_xsdChoice.MaxOccurs != 1)
                        {
                            this.o_currentElement.MaxOccurs = o_xsdChoice.MaxOccurs;
                        }
                    }
                    else if (b_simpleType)
                    { /* handle xs:simpleType */
                        /* parse xs:simpleType */
                        i_nestedMax = this.ParseXSDSimpleType(p_a_xsdTags, i_min, i_max);

                        /* set xml tag pointer to skip processed simpleType tag */
                        i_min = i_nestedMax;

                        b_simpleType = false;

                        ForestNET.Lib.Global.ILogFiner("after");
                        ForestNET.Lib.Global.ILogFiner("\t" + i_min + " ... " + i_max);
                        ForestNET.Lib.Global.ILogFiner("\t" + p_a_xsdTags[i_min] + " ... " + p_a_xsdTags[i_max]);
                    }
                    else if (b_simpleContent)
                    { /* handle xs:simpleContent */
                        /* parse xs:simpleContent */
                        i_nestedMax = this.ParseXSDSimpleContent(p_a_xsdTags, i_min, i_max);

                        /* set xml tag pointer to skip processed simpleContent tag */
                        i_min = i_nestedMax;

                        b_simpleContent = false;

                        ForestNET.Lib.Global.ILogFiner("after");
                        ForestNET.Lib.Global.ILogFiner("\t" + i_min + " ... " + i_max);
                        ForestNET.Lib.Global.ILogFiner("\t" + p_a_xsdTags[i_min] + " ... " + p_a_xsdTags[i_max]);
                    }

                    /* set end tag pointer */
                    int i_endTagPointer = i_max;

                    /* overwrite end tag pointer if we had a nested simpleType or simpleContent */
                    if (i_nestedMax > 0)
                    {
                        i_endTagPointer = i_nestedMax;
                    }

                    ForestNET.Lib.Global.ILogFiner("endTagPointer");
                    ForestNET.Lib.Global.ILogFiner("\t\t" + i_min + " ... " + i_endTagPointer);
                    ForestNET.Lib.Global.ILogFiner("\t\t" + p_a_xsdTags[i_min] + " ... " + p_a_xsdTags[i_endTagPointer]);

                    ForestNET.Lib.Global.ILogFiner("after");
                    ForestNET.Lib.Global.ILogFiner("\t" + i_min + " ... " + i_max);
                    ForestNET.Lib.Global.ILogFiner("\t" + p_a_xsdTags[i_min] + " ... " + p_a_xsdTags[i_max]);

                    /* if we still have no closing tag, then our xsd schema is invalid */
                    if (!p_a_xsdTags[i_endTagPointer].StartsWith("</"))
                    {
                        throw new ArgumentException("Invalid xsd-tag(" + p_a_xsdTags[i_endTagPointer] + ") is not closed in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                    }

                    /* get xsd end type */
                    XSDType e_xsdEndTagType = XML.GetXSDType(p_a_xsdTags, i_endTagPointer);

                    /* xsd type and xsd close type must match */
                    if (e_xsdTagType != e_xsdEndTagType)
                    {
                        throw new ArgumentException("Invalid xsd-tag-type \"" + p_a_xsdTags[i_endTagPointer] + "\" for closing in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                    }
                    else
                    {
                        /* if we had a no nested simpleType or simpleContent, decrease xml end tag pointer */
                        if (i_nestedMax < 0)
                        {
                            i_max--;
                        }
                    }
                }
                else
                {
                    ForestNET.Lib.Global.ILogFiner("\tElement: " + i_min + " ... " + i_max);
                    ForestNET.Lib.Global.ILogFiner("\tElement: " + p_a_xsdTags[i_min] + " ... " + p_a_xsdTags[i_max]);

                    if ((e_xsdTagType == XSDType.Element) || (e_xsdTagType == XSDType.ComplexType))
                    {
                        System.Text.RegularExpressions.Regex o_regex = new("ref=\"([^\"]*)\"");
                        System.Text.RegularExpressions.Match o_matcher = o_regex.Match(p_a_xsdTags[i_min]);

                        o_regex = new System.Text.RegularExpressions.Regex("name=\"([^\"]*)\"");
                        System.Text.RegularExpressions.Match o_matcherName = o_regex.Match(p_a_xsdTags[i_min]);

                        o_regex = new System.Text.RegularExpressions.Regex("type=\"xs:([^\"]*)\"");
                        System.Text.RegularExpressions.Match o_matcherPrimitiveType = o_regex.Match(p_a_xsdTags[i_min]);

                        o_regex = new System.Text.RegularExpressions.Regex("type=\"([^\"]*)\"");
                        System.Text.RegularExpressions.Match o_matcherType = o_regex.Match(p_a_xsdTags[i_min]);

                        bool b_nameAttributeFound = o_matcherName.Success;

                        if (o_matcher.Success)
                        { /* must have a reference attribute */
                            string s_referenceName = o_matcher.Value.Substring(5, o_matcher.Value.Length - 1 - 5);

                            /* check if we have a duplicate */
                            if (this.GetXSDElement(s_referenceName) != null)
                            {
                                throw new ArgumentException("Invalid xsd-tag xs:element (duplicate) at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                            }

                            if (this.XsdElementDefinitionExist(s_referenceName))
                            {
                                ForestNET.Lib.Global.ILogFiner("add and check element reference = " + s_referenceName);

                                XSDElement o_xsdElement = this.GetXSDElementDefinition(s_referenceName) ?? throw new NullReferenceException("xsd element definition not found with reference '" + s_referenceName + "'");

                                /* read name attribute out of xs:element tag */
                                if (o_matcherName.Success)
                                {
                                    string s_name = o_matcherName.Value.Substring(6, o_matcherName.Value.Length - 1 - 6);

                                    /* overwrite name value from reference, because it is dominant from the usage point */
                                    o_xsdElement.Name = s_name;
                                }

                                /* read minOccurs attribute out of xs:element tag */
                                o_regex = new System.Text.RegularExpressions.Regex("minOccurs=\"([^\"]*)\"");
                                o_matcher = o_regex.Match(p_a_xsdTags[i_min]);

                                if (o_matcher.Success)
                                {
                                    o_xsdElement.MinOccurs = int.Parse(o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11));
                                }

                                /* read maxOccurs attribute out of xs:element tag */
                                o_regex = new System.Text.RegularExpressions.Regex("maxOccurs=\"([^\"]*)\"");
                                o_matcher = o_regex.Match(p_a_xsdTags[i_min]);

                                if (o_matcher.Success)
                                {
                                    if (!o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11).Equals("unbounded"))
                                    {
                                        o_xsdElement.MaxOccurs = int.Parse(o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11));
                                    }
                                    else if (o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11).Equals("unbounded"))
                                    {
                                        o_xsdElement.MaxOccurs = -1;
                                    }
                                }

                                /* library does not support multiple occurrences of the same xml element without a list definition */
                                if ((!o_xsdElement.Choice) && (this.o_currentElement != null) && (!this.o_currentElement.Mapping.Contains("ArrayList(")) && (o_xsdElement.MaxOccurs > 1))
                                {
                                    throw new ArgumentException("Library does not support multiple occurrences of the same xml element without a list definition in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                                }

                                /* add xs:element to current element */
                                this.o_currentElement?.Children.Add(o_xsdElement);
                            }
                            else
                            {
                                throw new ArgumentException("Invalid xsd-tag xs:element with unknown reference at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                            }
                        }
                        else if ((b_nameAttributeFound) && (o_matcherPrimitiveType.Success))
                        { /* must have a name and primitive type attribute which are equal */
                            string s_name = o_matcherName.Value.Substring(6, o_matcherName.Value.Length - 1 - 6);
                            string s_type = o_matcherPrimitiveType.Value.Substring(9, o_matcherPrimitiveType.Value.Length - 1 - 9);

                            if (!s_name.Equals(s_type))
                            {
                                throw new ArgumentException("Invalid xsd-tag xs:element without reference at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\", and name and primitive type are not equal ['" + s_name + "' != '" + s_type + "']");
                            }

                            ForestNET.Lib.Global.ILogFiner("create and read xsd-element as array element definition = " + s_type);

                            XSDElement o_xsdElement = new(s_name, s_type)
                            {
                                /* set xsd-element as array */
                                IsArray = true
                            };

                            /* read minOccurs attribute out of xs:element tag */
                            o_regex = new System.Text.RegularExpressions.Regex("minOccurs=\"([^\"]*)\"");
                            o_matcher = o_regex.Match(p_a_xsdTags[i_min]);

                            if (o_matcher.Success)
                            {
                                o_xsdElement.MinOccurs = int.Parse(o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11));
                            }

                            /* read maxOccurs attribute out of xs:element tag */
                            o_regex = new System.Text.RegularExpressions.Regex("maxOccurs=\"([^\"]*)\"");
                            o_matcher = o_regex.Match(p_a_xsdTags[i_min]);

                            if (o_matcher.Success)
                            {
                                if (!o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11).Equals("unbounded"))
                                {
                                    o_xsdElement.MaxOccurs = int.Parse(o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11));
                                }
                                else if (o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11).Equals("unbounded"))
                                {
                                    o_xsdElement.MaxOccurs = -1;
                                }
                            }

                            /* add xs:element to current element */
                            this.o_currentElement?.Children.Add(o_xsdElement);
                        }
                        else if ((b_nameAttributeFound) && (o_matcherType.Success))
                        { /* must have a name and type attribute, where type attribute value is used as reference */
                            /* read name and type attribute value */
                            string s_name = o_matcherName.Value.Substring(6, o_matcherName.Value.Length - 1 - 6);
                            string s_typeName = o_matcherType.Value.Substring(6, o_matcherType.Value.Length - 1 - 6);

                            /* check if we have a duplicate */
                            if (this.GetXSDElement(s_typeName) != null)
                            {
                                throw new ArgumentException("Invalid xsd-tag xs:element (duplicate) at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                            }

                            if (this.XsdElementDefinitionExist(s_typeName))
                            {
                                ForestNET.Lib.Global.ILogFiner("add and check element reference = " + s_typeName);
                                XSDElement o_xsdElement = this.GetXSDElementDefinition(s_typeName) ?? throw new NullReferenceException("xsd element definition not found with type name '" + s_typeName + "'");

                                /* overwrite name value from reference, because it is dominant from the usage point */
                                o_xsdElement.Name = s_name;

                                /* read minOccurs attribute out of xs:element tag */
                                o_regex = new System.Text.RegularExpressions.Regex("minOccurs=\"([^\"]*)\"");
                                o_matcher = o_regex.Match(p_a_xsdTags[i_min]);

                                if (o_matcher.Success)
                                {
                                    o_xsdElement.MinOccurs = int.Parse(o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11));
                                }

                                /* read maxOccurs attribute out of xs:element tag */
                                o_regex = new System.Text.RegularExpressions.Regex("maxOccurs=\"([^\"]*)\"");
                                o_matcher = o_regex.Match(p_a_xsdTags[i_min]);

                                if (o_matcher.Success)
                                {
                                    if (!o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11).Equals("unbounded"))
                                    {
                                        o_xsdElement.MaxOccurs = int.Parse(o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11));
                                    }
                                    else if (o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11).Equals("unbounded"))
                                    {
                                        o_xsdElement.MaxOccurs = -1;
                                    }
                                }

                                /* library does not support multiple occurrences of the same xml element without a list definition */
                                if ((!o_xsdElement.Choice) && (this.o_currentElement != null) && (!this.o_currentElement.Mapping.Contains("ArrayList(")) && (o_xsdElement.MaxOccurs > 1))
                                {
                                    throw new ArgumentException("Library does not support multiple occurrences of the same xml element without a list definition in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                                }

                                /* add xs:element to current element */
                                this.o_currentElement?.Children.Add(o_xsdElement);
                            }
                            else
                            {
                                throw new ArgumentException("Invalid xsd-tag xs:element with unknown type as reference at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                            }
                        }
                        else
                        {
                            throw new ArgumentException("Invalid xsd-tag xs:element without a reference at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Invalid xsd-tag in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                    }
                }
            }
        }

        /// <summary>
        /// Parse xsd element based on XSDElement
        /// </summary>
        /// <param name="p_a_xsdTags">list of xsd element tags</param>
        /// <param name="p_i_min">pointer where to start line iteration</param>
        /// <exception cref="ArgumentException">value or structure within xsd file lines invalid</exception>
        private XSDElement ParseXSDElement(List<string> p_a_xsdTags, int p_i_min)
        {
            return ParseXSDElement(p_a_xsdTags, p_i_min, false);
        }

        /// <summary>
        /// Parse xsd content after divided definitions has been parsed, based on XSDElement, XSDAttribute and XSDRestriction
        /// </summary>
        /// <param name="p_a_xsdTags">list of xsd element tags</param>
        /// <param name="p_i_min">pointer where to start line iteration</param>
        /// <param name="p_b_ignoreType">true - ignore attribute value of xsd element, false - check for valid attribute type of xsd element</param>
        /// <exception cref="ArgumentException">value or structure within xsd file lines invalid</exception>
        private XSDElement ParseXSDElement(List<string> p_a_xsdTags, int p_i_min, bool p_b_ignoreType)
        {
            XSDElement o_xsdElement;

            /* read name attribute out of xs:element tag */
            System.Text.RegularExpressions.Regex o_regex = new("name=\"([^\"]*)\"");
            System.Text.RegularExpressions.Match o_matcher = o_regex.Match(p_a_xsdTags[p_i_min]);

            if (o_matcher.Success)
            {
                string s_name = o_matcher.Value.Substring(6, o_matcher.Value.Length - 1 - 6);

                if (!ForestNET.Lib.Helper.MatchesRegex(s_name, "[a-zA-Z0-9-_]*"))
                {
                    throw new ArgumentException("Invalid schema element name '" + s_name + "', invalid characters. Following characters are allowed: [a-z], [A-Z], [0-9], [-] and [_]");
                }

                o_xsdElement = new XSDElement(s_name);
            }
            else
            {
                /* no name attribute found */
                throw new ArgumentException("Invalid xsd-tag xs:element without a name at(" + (p_i_min + 1) + ".-element) \"" + p_a_xsdTags[p_i_min] + "\".");
            }

            if (!p_b_ignoreType)
            {
                /* read type attribute out of xs:element tag */
                o_regex = new System.Text.RegularExpressions.Regex("type=\"xs:([^\"]*)\"");
                o_matcher = o_regex.Match(p_a_xsdTags[p_i_min]);

                if (o_matcher.Success)
                {
                    o_xsdElement.Type = o_matcher.Value.Substring(9, o_matcher.Value.Length - 1 - 9);
                }
                else
                {
                    if (this.b_ignoreMapping)
                    {
                        /* read type attribute out of xs:element tag as reference */
                        o_regex = new System.Text.RegularExpressions.Regex("type=\"([^\"]*)\"");
                        o_matcher = o_regex.Match(p_a_xsdTags[p_i_min]);

                        if (o_matcher.Success)
                        {
                            o_xsdElement.Type = o_matcher.Value.Substring(6, o_matcher.Value.Length - 1 - 6);
                        }
                        else
                        {
                            /* no type attribute found */
                            throw new ArgumentException("Invalid xsd-tag xs:element without a type at(" + (p_i_min + 1) + ".-element) \"" + p_a_xsdTags[p_i_min] + "\".");
                        }
                    }
                    else
                    {
                        /* no type attribute found */
                        throw new ArgumentException("Invalid xsd-tag xs:element without a type at(" + (p_i_min + 1) + ".-element) \"" + p_a_xsdTags[p_i_min] + "\".");
                    }
                }
            }

            /* read mapping attribute out of xs:element tag */
            o_regex = new System.Text.RegularExpressions.Regex("mapping=\"([^\"]*)\"");
            o_matcher = o_regex.Match(p_a_xsdTags[p_i_min]);

            if (o_matcher.Success)
            {
                o_xsdElement.Mapping = o_matcher.Value.Substring(9, o_matcher.Value.Length - 1 - 9);
            }
            else
            {
                if (o_xsdElement.Name.Equals(o_xsdElement.Type))
                {
                    List<string> a_invalidNames = [ "string", "duration", "hexbinary", "base64binary", "anyuri", "normalizedstring", "token",
                            "language", "name", "ncname", "nmtoken", "id", "idref", "entity", "integer", "int", "positiveinteger", "nonpositiveinteger", "negativeinteger",
                            "nonnegativeinteger", "byte", "unsignedint", "unsignedbyte", "boolean", "duration", "date", "time", "datetime", "decimal", "double", "float", "short", "long" ];

                    if (!a_invalidNames.Contains(o_xsdElement.Name))
                    {
                        if (!this.b_ignoreMapping)
                        { /* it is fine that there is no valid name with type if we want to ignore mapping */
                            throw new ArgumentException("Invalid name for xsd-element as array '" + o_xsdElement.Name + "'");
                        }
                    }
                    else
                    {
                        o_xsdElement.IsArray = true;
                    }
                }
                else
                {
                    if (!this.b_ignoreMapping)
                    { /* it is fine that there is no mapping attribute if we want to ignore mapping */
                        /* no mapping attribute found */
                        throw new ArgumentException("Invalid xsd-tag xs:element without a mapping at(" + (p_i_min + 1) + ".-element) \"" + p_a_xsdTags[p_i_min] + "\".");
                    }
                }
            }

            /* read minOccurs attribute out of xs:element tag */
            o_regex = new System.Text.RegularExpressions.Regex("minOccurs=\"([^\"]*)\"");
            o_matcher = o_regex.Match(p_a_xsdTags[p_i_min]);

            if (o_matcher.Success)
            {
                o_xsdElement.MinOccurs = int.Parse(o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11));
            }

            /* read maxOccurs attribute out of xs:element tag */
            o_regex = new System.Text.RegularExpressions.Regex("maxOccurs=\"([^\"]*)\"");
            o_matcher = o_regex.Match(p_a_xsdTags[p_i_min]);

            if (o_matcher.Success)
            {
                if (!o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11).Equals("unbounded"))
                {
                    o_xsdElement.MaxOccurs = int.Parse(o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11));
                }
                else if (o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11).Equals("unbounded"))
                {
                    o_xsdElement.MaxOccurs = -1;
                }
            }

            return o_xsdElement;
        }

        /// <summary>
        /// Parse xsd element with a simple type within, based on XSDElement
        /// </summary>
        /// <param name="p_a_xsdTags">list of xsd element tags</param>
        /// <param name="p_i_min">pointer where to start line iteration</param>
        /// <exception cref="ArgumentException">value or structure within xsd file lines invalid</exception>
        private XSDElement ParseXSDElementWithSimpleType(List<string> p_a_xsdTags, int p_i_min)
        {
            XSDElement o_xsdElement;

            /* read name attribute out of xs:element tag */
            System.Text.RegularExpressions.Regex o_regex = new("name=\"([^\"]*)\"");
            System.Text.RegularExpressions.Match o_matcher = o_regex.Match(p_a_xsdTags[p_i_min]);

            if (o_matcher.Success)
            {
                string s_name = o_matcher.Value.Substring(6, o_matcher.Value.Length - 1 - 6);

                if (!ForestNET.Lib.Helper.MatchesRegex(s_name, "[a-zA-Z0-9-_]*"))
                {
                    throw new ArgumentException("Invalid schema element name '" + s_name + "', invalid characters. Following characters are allowed: [a-z], [A-Z], [0-9], [-] and [_]");
                }

                o_xsdElement = new XSDElement(s_name)
                {
                    SimpleType = true
                };
            }
            else
            {
                /* no name attribute found */
                throw new ArgumentException("Invalid xsd-tag xs:element without a name at(" + (p_i_min + 1) + ".-element) \"" + p_a_xsdTags[p_i_min] + "\".");
            }

            /* read mapping attribute out of xs:element tag */
            o_regex = new System.Text.RegularExpressions.Regex("mapping=\"([^\"]*)\"");
            o_matcher = o_regex.Match(p_a_xsdTags[p_i_min]);

            if (o_matcher.Success)
            {
                o_xsdElement.Mapping = o_matcher.Value.Substring(9, o_matcher.Value.Length - 1 - 9);
            }
            else
            {
                if (o_xsdElement.Name.Equals(o_xsdElement.Type))
                {
                    List<string> a_invalidNames = [ "string", "duration", "hexbinary", "base64binary", "anyuri", "normalizedstring", "token",
                            "language", "name", "ncname", "nmtoken", "id", "idref", "entity", "integer", "int", "positiveinteger", "nonpositiveinteger", "negativeinteger",
                            "nonnegativeinteger", "byte", "unsignedint", "unsignedbyte", "boolean", "duration", "date", "time", "datetime", "decimal", "double", "float", "short", "long" ];

                    if (!a_invalidNames.Contains(o_xsdElement.Name))
                    {
                        throw new ArgumentException("Invalid name for xsd-element as array '" + o_xsdElement.Name + "'");
                    }

                    o_xsdElement.IsArray = true;
                }
                else
                {
                    if (!this.b_ignoreMapping)
                    { /* it is fine that there is no mapping attribute if we want to ignore mapping */
                        /* no mapping attribute found */
                        throw new ArgumentException("Invalid xsd-tag xs:element without a mapping at(" + (p_i_min + 1) + ".-element) \"" + p_a_xsdTags[p_i_min] + "\".");
                    }
                }
            }

            /* read minOccurs attribute out of xs:element tag */
            o_regex = new System.Text.RegularExpressions.Regex("minOccurs=\"([^\"]*)\"");
            o_matcher = o_regex.Match(p_a_xsdTags[p_i_min]);

            if (o_matcher.Success)
            {
                o_xsdElement.MinOccurs = int.Parse(o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11));
            }

            /* read maxOccurs attribute out of xs:element tag */
            o_regex = new System.Text.RegularExpressions.Regex("maxOccurs=\"([^\"]*)\"");
            o_matcher = o_regex.Match(p_a_xsdTags[p_i_min]);

            if (o_matcher.Success)
            {
                if (!o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11).Equals("unbounded"))
                {
                    o_xsdElement.MaxOccurs = int.Parse(o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11));
                }
                else if (o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11).Equals("unbounded"))
                {
                    o_xsdElement.MaxOccurs = -1;
                }
            }

            /* check if we have a restriction tag */
            if ((XML.GetXSDType(p_a_xsdTags, p_i_min + 1) != XSDType.Restriction) && (XML.GetXSDType(p_a_xsdTags, p_i_min + 2) != XSDType.Restriction))
            {
                /* no restriction tag found for attribute with simpleType */
                throw new ArgumentException("Invalid xsd-tag xs:attribute with xs:simpleType without a xs:restriction at(" + (p_i_min + 1) + ".-element) \"" + p_a_xsdTags[p_i_min] + "\".");
            }

            /* read base attribute out of xs:restriction tag */
            o_regex = new System.Text.RegularExpressions.Regex("base=\"xs:([^\"]*)\"");
            o_matcher = o_regex.Match(p_a_xsdTags[p_i_min + 2]);

            if (o_matcher.Success)
            {
                ForestNET.Lib.Global.ILogFiner("found restriction = \"" + p_a_xsdTags[p_i_min + 2] + "\"");
                o_xsdElement.Type = o_matcher.Value.Substring(9, o_matcher.Value.Length - 1 - 9);
                o_xsdElement.Restriction = true;
            }
            else
            {
                /* read base attribute out of xs:restriction tag */
                o_regex = new System.Text.RegularExpressions.Regex("base=\"xs:([^\"]*)\"");
                o_matcher = o_regex.Match(p_a_xsdTags[p_i_min + 1]);

                if (o_matcher.Success)
                {
                    ForestNET.Lib.Global.ILogFiner("found restriction = \"" + p_a_xsdTags[p_i_min + 1] + "\"");
                    o_xsdElement.Type = o_matcher.Value.Substring(9, o_matcher.Value.Length - 1 - 9);
                    o_xsdElement.Restriction = true;
                }
                else
                {
                    /* no type attribute found */
                    throw new ArgumentException("Invalid xsd-tag xs:restriction without a valid base-type at(" + (p_i_min + 2) + ".-element) \"" + p_a_xsdTags[p_i_min + 1] + "\".");
                }
            }

            /* start tag pointer for restriction items within xs:simpleType */
            int i_tempStart = p_i_min + 3;

            /* end tag pointer for nested xs:simpleType */
            int i_tempMax = p_i_min + 2;

            /* check next nested tag within xs:simpleType, if next tag is already a xs:restriction tag(not starting with xs:element), we must decrement temp start and temp max */
            if (XML.GetXSDType(p_a_xsdTags, p_i_min + 1) == XSDType.Restriction)
            {
                if (p_a_xsdTags[p_i_min + 1].EndsWith("/>"))
                {
                    /* just a restriction as one line, without any restriction items - can return xsd element here */
                    return o_xsdElement;
                }
                else
                {
                    i_tempStart--;
                    i_tempMax--;
                }
            }

            try
            {
                /* find end of restriction interlacing */
                while (!p_a_xsdTags[i_tempMax].StartsWith("</xs:restriction>", StringComparison.CurrentCultureIgnoreCase))
                {
                    i_tempMax++;
                }
            }
            catch (Exception)
            {
                /* forbidden state - interlacing is not valid in xsd-schema */
                throw new ArgumentException("Invalid nested xsd-tag xs:restriction at(" + (p_i_min + 1) + ".-element) \"" + p_a_xsdTags[p_i_min] + "\".");
            }

            ForestNET.Lib.Global.ILogFiner("iterate simpleType");
            ForestNET.Lib.Global.ILogFiner("\t\t" + (i_tempStart) + " ... " + (i_tempMax - 1));
            ForestNET.Lib.Global.ILogFiner("\t\t" + p_a_xsdTags[i_tempStart] + " ... " + p_a_xsdTags[i_tempMax - 1]);

            /* parse content of xs:simpleType */
            for (int i_tempMin = i_tempStart; i_tempMin <= (i_tempMax - 1); i_tempMin++)
            {
                /* get xsd type */
                XSDType e_xsdSimpleTypeTagType = XML.GetXSDType(p_a_xsdTags, i_tempMin);

                if (e_xsdSimpleTypeTagType == XSDType.RestrictionItem)
                {
                    ForestNET.Lib.Global.ILogFinest("found restriction item = \"" + p_a_xsdTags[i_tempMin] + "\"");
                    /* add restriction to xs:simpleType element */
                    o_xsdElement.Restrictions.Add(XML.ParseXSDRestrictionItem(p_a_xsdTags, i_tempMin));
                }
            }

            return o_xsdElement;
        }

        /// <summary>
        /// Parse xsd complex type with a simple content within, based on XSDElement
        /// </summary>
        /// <param name="p_a_xsdTags">list of xsd element tags</param>
        /// <param name="p_i_min">pointer where to start line iteration</param>
        /// <exception cref="ArgumentException">value or structure within xsd file lines invalid</exception>
        private XSDElement? ParseXSDComplexTypeWithSimpleContent(List<string> p_a_xsdTags, int p_i_min)
        {
            /* identify simpleContent tag for complexType element */
            ForestNET.Lib.Global.ILogFiner("found simpleContent: " + p_a_xsdTags[p_i_min]);

            XSDElement o_xsdElement = new();
            bool b_foundName = false;
            bool b_foundMapping = false;

            /* read name attribute out of xs:element tag */
            System.Text.RegularExpressions.Regex o_regex = new("name=\"([^\"]*)\"");
            System.Text.RegularExpressions.Match o_matcher = o_regex.Match(p_a_xsdTags[p_i_min]);

            if (o_matcher.Success)
            {
                string s_name = o_matcher.Value.Substring(6, o_matcher.Value.Length - 1 - 6);

                if (!ForestNET.Lib.Helper.MatchesRegex(s_name, "[a-zA-Z0-9-_]*"))
                {
                    throw new ArgumentException("Invalid schema element name '" + s_name + "', invalid characters. Following characters are allowed: [a-z], [A-Z], [0-9], [-] and [_]");
                }

                o_xsdElement.Name = s_name;
                b_foundName = true;
            }

            /* read mapping attribute out of xs:element tag */
            o_regex = new System.Text.RegularExpressions.Regex("mapping=\"([^\"]*)\"");
            o_matcher = o_regex.Match(p_a_xsdTags[p_i_min]);

            if (o_matcher.Success)
            {
                o_xsdElement.Mapping = o_matcher.Value.Substring(9, o_matcher.Value.Length - 1 - 9);
                b_foundMapping = true;
            }

            /* read minOccurs attribute out of xs:element tag */
            o_regex = new System.Text.RegularExpressions.Regex("minOccurs=\"([^\"]*)\"");
            o_matcher = o_regex.Match(p_a_xsdTags[p_i_min]);

            if (o_matcher.Success)
            {
                o_xsdElement.MinOccurs = int.Parse(o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11));
            }

            /* read maxOccurs attribute out of xs:element tag */
            o_regex = new System.Text.RegularExpressions.Regex("maxOccurs=\"([^\"]*)\"");
            o_matcher = o_regex.Match(p_a_xsdTags[p_i_min]);

            if (o_matcher.Success)
            {
                if (!o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11).Equals("unbounded"))
                {
                    o_xsdElement.MaxOccurs = int.Parse(o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11));
                }
                else if (o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11).Equals("unbounded"))
                {
                    o_xsdElement.MaxOccurs = -1;
                }
            }

            /* start tag pointer for nested xs:simpleContent */
            int i_tempMin = p_i_min;

            /* find start of simpleContent complexType interlacing */
            if (!p_a_xsdTags[i_tempMin].StartsWith("<xs:complextype", StringComparison.CurrentCultureIgnoreCase))
            {
                /* forbidden state - interlacing is not valid in xsd-schema */
                throw new ArgumentException("Invalid nested xsd-tag xs:simpleContent not starting with xs:complexType at(" + (p_i_min + 1) + ".-element) \"" + p_a_xsdTags[p_i_min] + "\".");
            }

            /* end tag pointer for nested xs:simpleContent */
            int i_tempMax = p_i_min;

            /* find end of simpleType interlacing */
            while (!p_a_xsdTags[i_tempMax].StartsWith("</xs:complextype", StringComparison.CurrentCultureIgnoreCase))
            {
                if (i_tempMax == (p_a_xsdTags.Count - 1))
                {
                    /* forbidden state - interlacing is not valid in xsd-schema */
                    throw new ArgumentException("Invalid nested xsd-tag xs:simpleContent at(" + (p_i_min + 1) + ".-element) \"" + p_a_xsdTags[p_i_min] + "\".");
                }

                i_tempMax++;
            }

            ForestNET.Lib.Global.ILogFiner("iterate simpleContent");
            ForestNET.Lib.Global.ILogFiner("\t\t" + (i_tempMin + 1) + " ... " + (i_tempMax - 1));
            ForestNET.Lib.Global.ILogFiner("\t\t" + p_a_xsdTags[i_tempMin + 1] + " ... " + p_a_xsdTags[i_tempMax - 1]);

            /* parse content of xs:simpleContent */
            for (++i_tempMin; i_tempMin <= (i_tempMax - 1); i_tempMin++)
            {
                /* get xsd type */
                XSDType e_xsdSimpleContentTagType = XML.GetXSDType(p_a_xsdTags, i_tempMin);

                if (e_xsdSimpleContentTagType == XSDType.Extension)
                {
                    ForestNET.Lib.Global.ILogFinest("found extension = \"" + p_a_xsdTags[i_tempMin] + "\"");

                    bool b_base_processed = false;

                    /* read base attribute out of xs:extension tag as primitive type */
                    o_regex = new System.Text.RegularExpressions.Regex("base=\"xs:([^\"]*)\"");
                    o_matcher = o_regex.Match(p_a_xsdTags[i_tempMin]);

                    if (o_matcher.Success)
                    {
                        /* check for name attribute of xs:element tag if we read a primitive type for simple content */
                        if (!b_foundName)
                        {
                            /* no name attribute found */
                            throw new ArgumentException("Invalid xsd-tag xs:element without a name at(" + (p_i_min + 1) + ".-element) \"" + p_a_xsdTags[p_i_min] + "\".");
                        }

                        ForestNET.Lib.Global.ILogFinest("found primitive type: " + o_matcher.Value.Substring(9, o_matcher.Value.Length - 1 - 9));

                        o_xsdElement.Type = o_matcher.Value.Substring(9, o_matcher.Value.Length - 1 - 9);
                        b_base_processed = true;
                    }
                    else
                    {
                        /* read base attribute out of xs:extension tag as reference type */
                        o_regex = new System.Text.RegularExpressions.Regex("base=\"([^\"]*)\"");
                        o_matcher = o_regex.Match(p_a_xsdTags[i_tempMin]);

                        if (o_matcher.Success)
                        {
                            ForestNET.Lib.Global.ILogFinest("found reference type: " + o_matcher.Value.Substring(6, o_matcher.Value.Length - 1 - 6));

                            /* found reference value */
                            string s_referenceName = o_matcher.Value.Substring(6, o_matcher.Value.Length - 1 - 6);

                            /* xsd object for reference */
                            XSDElement? o_xsdElementReference = null;

                            if (this.XsdElementDefinitionExist(s_referenceName))
                            { /* check if we have a reference within our element definition list */
                                o_xsdElementReference = this.GetXSDElementDefinition(s_referenceName);
                            }
                            else if (this.SchemaElementDefinitionExist(s_referenceName))
                            { /* check if we have a reference within our schema definition list */
                                o_xsdElementReference = this.GetSchemaElementDefinition(s_referenceName);
                            }

                            /* check if we found a reference */
                            if (o_xsdElementReference != null)
                            {
                                ForestNET.Lib.Global.ILogFinest("add element reference = " + s_referenceName);

                                /* set type value from reference name */
                                o_xsdElement.Type = s_referenceName;

                                /* overwrite name value */
                                if (!b_foundName)
                                {
                                    o_xsdElement.Name = o_xsdElementReference.Name;
                                }

                                /* overwrite mapping value */
                                if (!b_foundMapping)
                                {
                                    o_xsdElement.Mapping = o_xsdElementReference.Mapping;
                                }

                                /* library does not support multiple occurrences of the same xml element without a list definition */
                                if ((!o_xsdElement.Choice) && (!o_xsdElement.Mapping.Contains("ArrayList(")) && (o_xsdElement.MaxOccurs > 1))
                                {
                                    throw new ArgumentException("Library does not support multiple occurrences of the same xml element without a list definition in xsd-schema at(" + (p_i_min + 1) + ".-element) \"" + p_a_xsdTags[p_i_min] + "\".");
                                }
                            }
                            else
                            {
                                /* we do not have reference in definition pool */
                                return null;
                            }

                            b_base_processed = true;
                        }
                    }

                    if (!b_base_processed)
                    {
                        throw new ArgumentException("Invalid xsd-tag xs:extension without a valid base-type at(" + (i_tempMin + 1) + ".-element) \"" + p_a_xsdTags[i_tempMin] + "\".");
                    }
                }

                if (!p_a_xsdTags[i_tempMin].EndsWith("/>"))
                {
                    if ((e_xsdSimpleContentTagType == XSDType.Attribute) && (XML.GetXSDType(p_a_xsdTags, i_tempMin + 1) == XSDType.SimpleType))
                    {
                        /* parse xs:attribute with simpleType */
                        XSDAttribute o_xsdAttribute = this.ParseXSDAttributeWithSimpleType(p_a_xsdTags, i_tempMin);

                        /* add attribute definition to list */
                        this.a_attributeDefinitions.Add(o_xsdAttribute);

                        XSDAttribute o_xsdAttributeReplace = new()
                        {
                            Reference = o_xsdAttribute.Name
                        };

                        /* add xs:attribute object replacement to current complexType element */
                        o_xsdElement.Attributes.Add(o_xsdAttributeReplace);
                    }

                    /* if we still have no closing tag, then our xsd schema is invalid */
                    if (!p_a_xsdTags[i_tempMax].StartsWith("</"))
                    {
                        throw new ArgumentException("Invalid xsd-tag is not closed in xsd-schema at(" + (i_tempMin + 1) + ".-element) \"" + p_a_xsdTags[i_tempMin] + "\".");
                    }

                    /* get xsd end type */
                    XSDType e_xsdSimpleContentEndTagType = XML.GetXSDType(p_a_xsdTags, (i_tempMax - 1));

                    /* xsd type and xsd close type must match */
                    if (e_xsdSimpleContentTagType != e_xsdSimpleContentEndTagType)
                    {
                        throw new ArgumentException("Invalid xsd-tag-type \"" + p_a_xsdTags[i_tempMax - 1] + "\" for closing in xsd-schema at(" + (i_tempMin + 1) + ".-element) \"" + p_a_xsdTags[i_tempMin] + "\".");
                    }
                    else
                    {
                        i_tempMax--;
                    }
                }
                else
                {
                    if (e_xsdSimpleContentTagType == XSDType.Attribute)
                    {
                        ForestNET.Lib.Global.ILogFinest("found attribute item = \"" + p_a_xsdTags[i_tempMin] + "\"");

                        o_regex = new System.Text.RegularExpressions.Regex("ref=\"([^\"]*)\"");
                        o_matcher = o_regex.Match(p_a_xsdTags[i_tempMin]);

                        o_regex = new System.Text.RegularExpressions.Regex("name=\"([^\"]*)\"");
                        System.Text.RegularExpressions.Match o_matcherName = o_regex.Match(p_a_xsdTags[i_tempMin]);

                        o_regex = new System.Text.RegularExpressions.Regex("type=\"([^\"]*)\"");
                        System.Text.RegularExpressions.Match o_matcherType = o_regex.Match(p_a_xsdTags[i_tempMin]);

                        if (o_matcher.Success)
                        { /* we have an attribute element with reference */
                            string s_referenceName = o_matcher.Value.Substring(5, o_matcher.Value.Length - 1 - 5);

                            /* check if we have a duplicate */
                            if (this.GetXSDAttribute(s_referenceName, o_xsdElement) != null)
                            {
                                throw new ArgumentException("Invalid xsd-tag xs:attribute (duplicate) at(" + (i_tempMin + 1) + ".-element) \"" + p_a_xsdTags[i_tempMin] + "\".");
                            }

                            if (this.XsdAttributeDefinitionExist(s_referenceName))
                            {
                                ForestNET.Lib.Global.ILogFinest("add and check attribute reference = " + s_referenceName);

                                XSDAttribute o_xsdAttribute = this.GetXSDAttributeDefinition(s_referenceName) ?? throw new NullReferenceException("xsd attribute definition not found with reference '" + s_referenceName + "'");

                                /* read name attribute out of xs:attribute tag */
                                if ((o_matcherName.Success))
                                {
                                    string s_name = o_matcherName.Value.Substring(6, o_matcherName.Value.Length - 1 - 6);

                                    /* overwrite name value from reference, because it is dominant from the usage point */
                                    o_xsdAttribute.Name = s_name;
                                }

                                /* add xs:attribute to xsd element */
                                o_xsdElement.Attributes.Add(o_xsdAttribute);
                            }
                            else
                            {
                                /* we do not have reference in definition pool */
                                return null;
                            }
                        }
                        else if ((o_matcherName.Success) && (o_matcherType.Success))
                        { /* we have an attribute element with name and type */
                            /* read name and type attribute values of xs:attribute tag */
                            string s_name = o_matcherName.Value.Substring(6, o_matcherName.Value.Length - 1 - 6);
                            string s_type = o_matcherType.Value.Substring(6, o_matcherType.Value.Length - 1 - 6);

                            /* check if we have a duplicate */
                            if (this.GetXSDAttribute(s_type, o_xsdElement) != null)
                            {
                                throw new ArgumentNullException("Invalid xsd-tag xs:attribute (duplicate) at(" + (i_tempMin + 1) + ".-element) \"" + p_a_xsdTags[i_tempMin] + "\".");
                            }

                            /* xsd object for reference */
                            XSDElement? o_xsdElementReference = null;

                            if (this.XsdElementDefinitionExist(s_type))
                            { /* check if we have a reference within our element definition list */
                                o_xsdElementReference = this.GetXSDElementDefinition(s_type);
                            }
                            else if (this.SchemaElementDefinitionExist(s_type))
                            { /* check if we have a reference within our schema definition list */
                                o_xsdElementReference = this.GetSchemaElementDefinition(s_type);
                            }

                            /* check if we found an attribute reference */
                            if (this.XsdAttributeDefinitionExist(s_type))
                            {
                                ForestNET.Lib.Global.ILogFinest("add and check attribute type reference = " + s_type);

                                XSDAttribute o_xsdAttribute = this.GetXSDAttributeDefinition(s_type) ?? throw new NullReferenceException("xsd attribute definition not found with type '" + s_type + "'");

                                /* overwrite name value from reference, because it is dominant from the usage point */
                                o_xsdAttribute.Name = s_name;

                                /* add xs:attribute object to xsd element */
                                o_xsdElement.Attributes.Add(o_xsdAttribute);
                            }
                            else if (o_xsdElementReference != null)
                            { /* check if we found another reference */
                                ForestNET.Lib.Global.ILogFinest("add element reference = " + s_type);

                                XSDAttribute o_xsdAttribute = new(s_name);

                                /* read required attribute out of xs:attribute tag */
                                o_regex = new System.Text.RegularExpressions.Regex("use=\"required\"");
                                o_matcher = o_regex.Match(p_a_xsdTags[i_tempMin]);

                                if (o_matcher.Success)
                                {
                                    o_xsdAttribute.Required = true;
                                }

                                /* read default attribute out of xs:attribute tag */
                                o_regex = new System.Text.RegularExpressions.Regex("default=\"([^\"]*)\"");
                                o_matcher = o_regex.Match(p_a_xsdTags[i_tempMin]);

                                if (o_matcher.Success)
                                {
                                    o_xsdAttribute.Default = o_matcher.Value.Substring(9, o_matcher.Value.Length - 1 - 9);
                                }

                                /* read fixed attribute out of xs:attribute tag */
                                o_regex = new System.Text.RegularExpressions.Regex("fixed=\"([^\"]*)\"");
                                o_matcher = o_regex.Match(p_a_xsdTags[i_tempMin]);

                                if (o_matcher.Success)
                                {
                                    o_xsdAttribute.Fixed = o_matcher.Value.Substring(7, o_matcher.Value.Length - 1 - 7);
                                }

                                /* set type value from reference name */
                                o_xsdAttribute.Type = o_xsdElementReference.Name;

                                /* assume restrictions from reference */
                                foreach (XSDRestriction o_restriction in o_xsdElementReference.Restrictions)
                                {
                                    o_xsdAttribute.Restrictions.Add(o_restriction);
                                }

                                /* add xs:attribute object to xsd element */
                                o_xsdElement.Attributes.Add(o_xsdAttribute);
                            }
                            else
                            {
                                /* we do not have reference in definition pool */
                                return null;
                            }
                        }
                        else
                        {
                            /* parse xs:attribute */
                            XSDAttribute o_xsdAttribute = this.ParseXSDAttribute(p_a_xsdTags, i_tempMin);

                            /* add attribute definition to list */
                            this.a_attributeDefinitions.Add(o_xsdAttribute);

                            XSDAttribute o_xsdAttributeReplace = new()
                            {
                                Reference = o_xsdAttribute.Name
                            };

                            /* add xs:attribute object replacement to current complexType element */
                            o_xsdElement.Attributes.Add(o_xsdAttributeReplace);
                        }
                    }
                }
            }

            o_xsdElement.SimpleContent = true;

            return o_xsdElement;
        }

        /// <summary>
        /// Parse xsd attribute based on XSDAttribute
        /// </summary>
        /// <param name="p_a_xsdTags">list of xsd element tags</param>
        /// <param name="p_i_min">pointer where to start line iteration</param>
        /// <exception cref="ArgumentException">value or structure within xsd file lines invalid</exception>
        private XSDAttribute ParseXSDAttribute(List<string> p_a_xsdTags, int p_i_min)
        {
            XSDAttribute o_xsdAttribute;

            /* read name attribute out of xs:attribute tag */
            System.Text.RegularExpressions.Regex o_regex = new("name=\"([^\"]*)\"");
            System.Text.RegularExpressions.Match o_matcher = o_regex.Match(p_a_xsdTags[p_i_min]);

            if (o_matcher.Success)
            {
                string s_name = o_matcher.Value.Substring(6, o_matcher.Value.Length - 1 - 6);

                if (!ForestNET.Lib.Helper.MatchesRegex(s_name, "[a-zA-Z0-9-_]*"))
                {
                    throw new ArgumentException("Invalid schema attribute name '" + s_name + "', invalid characters. Following characters are allowed: [a-z], [A-Z], [0-9], [-] and [_]");
                }

                o_xsdAttribute = new XSDAttribute(s_name);
            }
            else
            {
                /* no name attribute found */
                throw new ArgumentException("Invalid xsd-tag xs:attribute without a name at(" + (p_i_min + 1) + ".-element) \"" + p_a_xsdTags[p_i_min] + "\".");
            }

            /* read type attribute out of xs:attribute tag */
            o_regex = new System.Text.RegularExpressions.Regex("type=\"xs:([^\"]*)\"");
            o_matcher = o_regex.Match(p_a_xsdTags[p_i_min]);

            if (o_matcher.Success)
            {
                o_xsdAttribute.Type = o_matcher.Value.Substring(9, o_matcher.Value.Length - 1 - 9);
            }
            else
            {
                /* no type attribute found */
                throw new ArgumentException("Invalid xsd-tag xs:attribute without a type at(" + (p_i_min + 1) + ".-element) \"" + p_a_xsdTags[p_i_min] + "\".");
            }

            /* read mapping attribute out of xs:attribute tag */
            o_regex = new System.Text.RegularExpressions.Regex("mapping=\"([^\"]*)\"");
            o_matcher = o_regex.Match(p_a_xsdTags[p_i_min]);

            if (o_matcher.Success)
            {
                o_xsdAttribute.Mapping = o_matcher.Value.Substring(9, o_matcher.Value.Length - 1 - 9);
            }
            else
            {
                if (!this.b_ignoreMapping)
                { /* it is fine that there is no mapping attribute if we want to ignore mapping */
                    /* no mapping attribute found */
                    throw new ArgumentException("Invalid xsd-tag xs:attribute without a mapping at(" + (p_i_min + 1) + ".-element) \"" + p_a_xsdTags[p_i_min] + "\".");
                }
            }

            /* read required attribute out of xs:attribute tag */
            o_regex = new System.Text.RegularExpressions.Regex("use=\"required\"");
            o_matcher = o_regex.Match(p_a_xsdTags[p_i_min]);

            if (o_matcher.Success)
            {
                o_xsdAttribute.Required = true;
            }

            /* read default attribute out of xs:attribute tag */
            o_regex = new System.Text.RegularExpressions.Regex("default=\"([^\"]*)\"");
            o_matcher = o_regex.Match(p_a_xsdTags[p_i_min]);

            if (o_matcher.Success)
            {
                o_xsdAttribute.Default = o_matcher.Value.Substring(9, o_matcher.Value.Length - 1 - 9);
            }

            /* read fixed attribute out of xs:attribute tag */
            o_regex = new System.Text.RegularExpressions.Regex("fixed=\"([^\"]*)\"");
            o_matcher = o_regex.Match(p_a_xsdTags[p_i_min]);

            if (o_matcher.Success)
            {
                o_xsdAttribute.Fixed = o_matcher.Value.Substring(7, o_matcher.Value.Length - 1 - 7);
            }

            return o_xsdAttribute;
        }

        /// <summary>
        /// Parse xsd attribute with a simple type within, based on XSDAttribute
        /// </summary>
        /// <param name="p_a_xsdTags">list of xsd element tags</param>
        /// <param name="p_i_min">pointer where to start line iteration</param>
        /// <exception cref="ArgumentException">value or structure within xsd file lines invalid</exception>
        private XSDAttribute ParseXSDAttributeWithSimpleType(List<string> p_a_xsdTags, int p_i_min)
        {
            XSDAttribute o_xsdAttribute;

            /* read name attribute out of xs:attribute tag */
            System.Text.RegularExpressions.Regex o_regex = new("name=\"([^\"]*)\"");
            System.Text.RegularExpressions.Match o_matcher = o_regex.Match(p_a_xsdTags[p_i_min]);

            if (o_matcher.Success)
            {
                string s_name = o_matcher.Value.Substring(6, o_matcher.Value.Length - 1 - 6);

                if (!ForestNET.Lib.Helper.MatchesRegex(s_name, "[a-zA-Z0-9-_]*"))
                {
                    throw new ArgumentException("Invalid schema attribute name '" + s_name + "', invalid characters. Following characters are allowed: [a-z], [A-Z], [0-9], [-] and [_]");
                }

                o_xsdAttribute = new XSDAttribute(s_name)
                {
                    SimpleType = true
                };
            }
            else
            {
                /* no name attribute found */
                throw new ArgumentException("Invalid xsd-tag xs:attribute without a name at(" + (p_i_min + 1) + ".-element) \"" + p_a_xsdTags[p_i_min] + "\".");
            }

            /* read mapping attribute out of xs:attribute tag */
            o_regex = new System.Text.RegularExpressions.Regex("mapping=\"([^\"]*)\"");
            o_matcher = o_regex.Match(p_a_xsdTags[p_i_min]);

            if (o_matcher.Success)
            {
                o_xsdAttribute.Mapping = o_matcher.Value.Substring(9, o_matcher.Value.Length - 1 - 9);
            }
            else
            {
                if (!this.b_ignoreMapping)
                { /* it is fine that there is no mapping attribute if we want to ignore mapping */
                    /* no mapping attribute found */
                    throw new ArgumentException("Invalid xsd-tag xs:attribute without a mapping at(" + (p_i_min + 1) + ".-element) \"" + p_a_xsdTags[p_i_min] + "\".");
                }
            }

            /* read required attribute out of xs:attribute tag */
            o_regex = new System.Text.RegularExpressions.Regex("use=\"required\"");
            o_matcher = o_regex.Match(p_a_xsdTags[p_i_min]);

            if (o_matcher.Success)
            {
                o_xsdAttribute.Required = true;
            }

            /* read default attribute out of xs:attribute tag */
            o_regex = new System.Text.RegularExpressions.Regex("default=\"([^\"]*)\"");
            o_matcher = o_regex.Match(p_a_xsdTags[p_i_min]);

            if (o_matcher.Success)
            {
                o_xsdAttribute.Default = o_matcher.Value.Substring(9, o_matcher.Value.Length - 1 - 9);
            }

            /* read fixed attribute out of xs:attribute tag */
            o_regex = new System.Text.RegularExpressions.Regex("fixed=\"([^\"]*)\"");
            o_matcher = o_regex.Match(p_a_xsdTags[p_i_min]);

            if (o_matcher.Success)
            {
                o_xsdAttribute.Fixed = o_matcher.Value.Substring(7, o_matcher.Value.Length - 1 - 7);
            }

            /* check if we have a restriction tag */
            if (XML.GetXSDType(p_a_xsdTags, p_i_min + 2) != XSDType.Restriction)
            {
                /* no restriction tag found for attribute with simpleType */
                throw new ArgumentException("Invalid xsd-tag xs:attribute with xs:simpleType without a xs:restriction at(" + (p_i_min + 1) + ".-element) \"" + p_a_xsdTags[p_i_min] + "\".");
            }

            ForestNET.Lib.Global.ILogFiner("found restriction = \"" + p_a_xsdTags[p_i_min + 2] + "\"");

            /* read base attribute out of xs:restriction tag */
            o_regex = new System.Text.RegularExpressions.Regex("base=\"xs:([^\"]*)\"");
            o_matcher = o_regex.Match(p_a_xsdTags[p_i_min + 2]);

            if (o_matcher.Success)
            {
                o_xsdAttribute.Type = o_matcher.Value.Substring(9, o_matcher.Value.Length - 1 - 9);
                o_xsdAttribute.Restriction = true;
            }
            else
            {
                /* no type attribute found */
                throw new ArgumentException("Invalid xsd-tag xs:restriction without a valid base-type at(" + (p_i_min + 3) + ".-element) \"" + p_a_xsdTags[p_i_min + 2] + "\".");
            }

            /* end tag pointer for nested xs:simpleType */
            int i_tempMax = p_i_min + 2;

            try
            {
                /* find end of restriction interlacing */
                while (!p_a_xsdTags[i_tempMax].StartsWith("</xs:restriction>", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (p_a_xsdTags[i_tempMax].Equals("</xs:attribute>"))
                    {
                        /* reached end of our attribte */
                        break;
                    }

                    i_tempMax++;
                }
            }
            catch (Exception)
            {
                /* forbidden state - interlacing is not valid in xsd-schema */
                throw new ArgumentException("Invalid nested xsd-tag xs:restriction at(" + (p_i_min + 1) + ".-element) \"" + p_a_xsdTags[p_i_min] + "\".");
            }

            ForestNET.Lib.Global.ILogFiner("iterate simpleType");
            ForestNET.Lib.Global.ILogFiner("\t\t" + (p_i_min + 2) + " ... " + (i_tempMax - 1));
            ForestNET.Lib.Global.ILogFiner("\t\t" + p_a_xsdTags[p_i_min + 2] + " ... " + p_a_xsdTags[i_tempMax - 1]);

            /* parse content of xs:simpleType */
            for (int i_tempMin = p_i_min + 2; i_tempMin <= (i_tempMax - 1); i_tempMin++)
            {
                /* get xsd type */
                XSDType e_xsdSimpleTypeTagType = XML.GetXSDType(p_a_xsdTags, i_tempMin);

                if (e_xsdSimpleTypeTagType == XSDType.RestrictionItem)
                {
                    ForestNET.Lib.Global.ILogFinest("found restriction item = \"" + p_a_xsdTags[i_tempMin] + "\"");

                    /* add restriction to xs:simpleType element */
                    o_xsdAttribute.Restrictions.Add(XML.ParseXSDRestrictionItem(p_a_xsdTags, i_tempMin));
                }
            }

            return o_xsdAttribute;
        }

        /// <summary>
        /// Parse xsd choice based on XSDElement
        /// </summary>
        /// <param name="p_a_xsdTags">list of xsd element tags</param>
        /// <param name="p_i_min">pointer where to start line iteration</param>
        /// <exception cref="ArgumentException">value or structure within xsd file lines invalid</exception>
        private static XSDElement ParseXSDChoice(List<string> p_a_xsdTags, int p_i_min)
        {
            XSDElement o_xsdElement = new();

            /* read minOccurs attribute out of xs:element tag */
            System.Text.RegularExpressions.Regex o_regex = new("minOccurs=\"([^\"]*)\"");
            System.Text.RegularExpressions.Match o_matcher = o_regex.Match(p_a_xsdTags[p_i_min]);

            if (o_matcher.Success)
            {
                o_xsdElement.MinOccurs = int.Parse(o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11));
            }

            /* read maxOccurs attribute out of xs:element tag */
            o_regex = new System.Text.RegularExpressions.Regex("maxOccurs=\"([^\"]*)\"");
            o_matcher = o_regex.Match(p_a_xsdTags[p_i_min]);

            if (o_matcher.Success)
            {
                if (!o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11).Equals("unbounded"))
                {
                    o_xsdElement.MaxOccurs = int.Parse(o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11));
                }
            }

            return o_xsdElement;
        }

        /// <summary>
        /// Parse xsd restriction based on XSDRestriction
        /// </summary>
        /// <param name="p_a_xsdTags">list of xsd element tags</param>
        /// <param name="p_i_min">pointer where to start line iteration</param>
        /// <exception cref="ArgumentException">value or structure within xsd file lines invalid</exception>
        private static XSDRestriction ParseXSDRestrictionItem(List<string> p_a_xsdTags, int p_i_min)
        {
            XSDRestriction o_xsdRestriction;

            /* read name attribute out of xs:attribute tag */
            System.Text.RegularExpressions.Regex o_regex = new("xs:([^<>\\s]*)");
            System.Text.RegularExpressions.Match o_matcher = o_regex.Match(p_a_xsdTags[p_i_min]);

            if (o_matcher.Success)
            {
                o_xsdRestriction = new XSDRestriction(o_matcher.Value.Substring(3));
            }
            else
            {
                /* no restriction name found */
                throw new ArgumentException("Invalid xsd-tag xs:restriction without a valid restriction name at(" + (p_i_min + 1) + ".-element) \"" + p_a_xsdTags[p_i_min] + "\".");
            }

            /* read type attribute out of xs:attribute tag */
            o_regex = new System.Text.RegularExpressions.Regex("value=\"([^\"]*)\"");
            o_matcher = o_regex.Match(p_a_xsdTags[p_i_min]);

            if (o_matcher.Success)
            {
                string s_value = o_matcher.Value.Substring(7, o_matcher.Value.Length - 1 - 7);

                if (ForestNET.Lib.Helper.IsInteger(s_value))
                {
                    o_xsdRestriction.IntValue = int.Parse(s_value);
                }
                else
                {
                    o_xsdRestriction.StrValue = s_value;
                }
            }
            else
            {
                /* no type attribute found */
                throw new ArgumentException("Invalid xsd-tag xs:restriction without a value at(" + (p_i_min + 1) + ".-element) \"" + p_a_xsdTags[p_i_min] + "\".");
            }

            return o_xsdRestriction;
        }

        /// <summary>
        /// Parse xsd simple type based on XSDElement
        /// </summary>
        /// <param name="p_a_xsdTags">list of xsd element tags</param>
        /// <param name="p_i_min">pointer where to start line iteration</param>
        /// <param name="p_i_max">pointer where to end line iteration</param>
        /// <exception cref="ArgumentException">value or structure within xsd file lines invalid</exception>
        private int ParseXSDSimpleType(List<string> p_a_xsdTags, int p_i_min, int p_i_max)
        {
            /* return xml tag pointer value */
            int i_nestedMax;

            /* identify simpleType tag for current element */
            ForestNET.Lib.Global.ILogFiner("found simpleType: " + p_a_xsdTags[p_i_min] + " to Current Element: " + this.o_currentElement?.Name);

            XSDElement o_xsdElement = this.ParseXSDElement(p_a_xsdTags, p_i_min, true);
            o_xsdElement.Restriction = true;

            /* start tag pointer for nested xs:simpleType */
            int i_tempMin = p_i_min;

            bool b_simpleTypeFirst = true;

            /* find start of simpleType interlacing */
            while (!p_a_xsdTags[i_tempMin].StartsWith("<xs:simpletype", StringComparison.CurrentCultureIgnoreCase))
            {
                if (i_tempMin == p_i_max)
                {
                    /* forbidden state - interlacing is not valid in xsd-schema */
                    throw new ArgumentException("Invalid nested xsd-tag xs:simpleType at(" + (p_i_min + 1) + ".-element) \"" + p_a_xsdTags[p_i_min] + "\".");
                }

                b_simpleTypeFirst = false;
                i_tempMin++;
            }

            /* end tag pointer for nested xs:simpleType */
            int i_tempMax = p_i_min;

            /* find end of simpleType interlacing */
            while (!p_a_xsdTags[i_tempMax].StartsWith("</xs:simpletype", StringComparison.CurrentCultureIgnoreCase))
            {
                if (i_tempMax == p_i_max)
                {
                    /* forbidden state - interlacing is not valid in xsd-schema */
                    throw new ArgumentException("Invalid nested xsd-tag xs:simpleType at(" + (p_i_min + 1) + ".-element) \"" + p_a_xsdTags[p_i_min] + "\".");
                }

                i_tempMax++;
            }

            /* set overwrite value for end tag pointer */
            i_nestedMax = i_tempMax;

            if (!b_simpleTypeFirst)
            {
                i_nestedMax++;
            }

            ForestNET.Lib.Global.ILogFiner("iterate simpleType");
            ForestNET.Lib.Global.ILogFiner("\t\t" + (i_tempMin + 1) + " ... " + (i_tempMax - 1));
            ForestNET.Lib.Global.ILogFiner("\t\t" + p_a_xsdTags[i_tempMin + 1] + " ... " + p_a_xsdTags[i_tempMax - 1]);

            /* parse content of xs:simpleType */
            for (++i_tempMin; i_tempMin < (i_tempMax - 1); i_tempMin++)
            {
                /* get xsd type */
                XSDType e_xsdSimpleTypeTagType = XML.GetXSDType(p_a_xsdTags, i_tempMin);

                if (e_xsdSimpleTypeTagType == XSDType.Restriction)
                {
                    ForestNET.Lib.Global.ILogFiner("found restriction = \"" + p_a_xsdTags[i_tempMin] + "\"");

                    /* read base attribute out of xs:restriction tag */
                    System.Text.RegularExpressions.Regex o_regex = new("base=\"xs:([^\"]*)\"");
                    System.Text.RegularExpressions.Match o_matcher = o_regex.Match(p_a_xsdTags[i_tempMin]);

                    if (o_matcher.Success)
                    {
                        o_xsdElement.Type = o_matcher.Value.Substring(9, o_matcher.Value.Length - 1 - 9);
                    }
                    else
                    {
                        /* no type attribute found */
                        throw new ArgumentException("Invalid xsd-tag xs:restriction without a valid base-type at(" + (i_tempMin + 1) + ".-element) \"" + p_a_xsdTags[i_tempMin] + "\".");
                    }
                }

                if (!p_a_xsdTags[i_tempMin].EndsWith("/>"))
                {
                    /* if we still have no closing tag, then our xsd schema is invalid */
                    if (!p_a_xsdTags[i_tempMax].StartsWith("</"))
                    {
                        throw new ArgumentException("Invalid xsd-tag is not closed in xsd-schema at(" + (i_tempMin + 1) + ".-element) \"" + p_a_xsdTags[i_tempMin] + "\".");
                    }

                    /* get xsd end type */
                    XSDType e_xsdSimpleTypeEndTagType = XML.GetXSDType(p_a_xsdTags, (i_tempMax - 1));

                    /* xsd type and xsd close type must match */
                    if (e_xsdSimpleTypeTagType != e_xsdSimpleTypeEndTagType)
                    {
                        throw new ArgumentException("Invalid xsd-tag-type \"" + p_a_xsdTags[i_tempMax - 1] + "\" for closing in xsd-schema at(" + (i_tempMin + 1) + ".-element) \"" + p_a_xsdTags[i_tempMin] + "\".");
                    }
                }
                else
                {
                    if (e_xsdSimpleTypeTagType == XSDType.RestrictionItem)
                    {
                        ForestNET.Lib.Global.ILogFinest("found restriction item = \"" + p_a_xsdTags[i_tempMin] + "\"");

                        /* add restriction to xs:simpleType element */
                        o_xsdElement.Restrictions.Add(XML.ParseXSDRestrictionItem(p_a_xsdTags, i_tempMin));
                    }
                }
            }

            /* add xs:simpleType as xs:element to current element */
            this.o_currentElement?.Children.Add(o_xsdElement);

            /* return xml tag pointer to skip processed simpleType tag */
            return i_nestedMax;
        }

        /// <summary>
        /// Parse xsd simple content based on XSDElement
        /// </summary>
        /// <param name="p_a_xsdTags">list of xsd element tags</param>
        /// <param name="p_i_min">pointer where to start line iteration</param>
        /// <param name="p_i_max">pointer where to end line iteration</param>
        /// <exception cref="ArgumentException">value or structure within xsd file lines invalid</exception>
        private int ParseXSDSimpleContent(List<string> p_a_xsdTags, int p_i_min, int p_i_max)
        {
            /* return xml tag pointer value */
            int i_nestedMax;

            /* identify simpleContent tag for current element */
            ForestNET.Lib.Global.ILogFiner("found simpleContent: " + p_a_xsdTags[p_i_min] + " to Current Element: " + this.o_currentElement?.Name);

            XSDElement o_xsdElement = new();
            bool b_foundName = false;
            bool b_foundMapping = false;

            /* read name attribute out of xs:element tag */
            System.Text.RegularExpressions.Regex o_regex = new("name=\"([^\"]*)\"");
            System.Text.RegularExpressions.Match o_matcher = o_regex.Match(p_a_xsdTags[p_i_min]);

            if (o_matcher.Success)
            {
                string s_name = o_matcher.Value.Substring(6, o_matcher.Value.Length - 1 - 6);

                if (!ForestNET.Lib.Helper.MatchesRegex(s_name, "[a-zA-Z0-9-_]*"))
                {
                    throw new ArgumentException("Invalid schema element name '" + s_name + "', invalid characters. Following characters are allowed: [a-z], [A-Z], [0-9], [-] and [_]");
                }

                o_xsdElement.Name = s_name;
                b_foundName = true;
            }

            /* read mapping attribute out of xs:element tag */
            o_regex = new System.Text.RegularExpressions.Regex("mapping=\"([^\"]*)\"");
            o_matcher = o_regex.Match(p_a_xsdTags[p_i_min]);

            if (o_matcher.Success)
            {
                o_xsdElement.Mapping = o_matcher.Value.Substring(9, o_matcher.Value.Length - 1 - 9);
                b_foundMapping = true;
            }
            else
            {
                if ((!ForestNET.Lib.Helper.IsStringEmpty(o_xsdElement.Name)) && (o_xsdElement.Name.Equals(o_xsdElement.Type)))
                {
                    List<string> a_invalidNames = [ "string", "duration", "hexbinary", "base64binary", "anyuri", "normalizedstring", "token",
                            "language", "name", "ncname", "nmtoken", "id", "idref", "entity", "integer", "int", "positiveinteger", "nonpositiveinteger", "negativeinteger",
                            "nonnegativeinteger", "byte", "unsignedint", "unsignedbyte", "boolean", "duration", "date", "time", "datetime", "decimal", "double", "float", "short", "long" ];

                    if (!a_invalidNames.Contains(o_xsdElement.Name))
                    {
                        throw new ArgumentException("Invalid name for xsd-element as array '" + o_xsdElement.Name + "'");
                    }

                    o_xsdElement.IsArray = true;
                }
            }

            /* read minOccurs attribute out of xs:element tag */
            o_regex = new System.Text.RegularExpressions.Regex("minOccurs=\"([^\"]*)\"");
            o_matcher = o_regex.Match(p_a_xsdTags[p_i_min]);

            if (o_matcher.Success)
            {
                o_xsdElement.MinOccurs = int.Parse(o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11));
            }

            /* read maxOccurs attribute out of xs:element tag */
            o_regex = new System.Text.RegularExpressions.Regex("maxOccurs=\"([^\"]*)\"");
            o_matcher = o_regex.Match(p_a_xsdTags[p_i_min]);

            if (o_matcher.Success)
            {
                if (!o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11).Equals("unbounded"))
                {
                    o_xsdElement.MaxOccurs = int.Parse(o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11));
                }
                else if (o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11).Equals("unbounded"))
                {
                    o_xsdElement.MaxOccurs = -1;
                }
            }

            /* start tag pointer for nested xs:simpleContent */
            int i_tempMin = p_i_min;

            bool b_complexTypeFirst = true;

            /* find start of simpleContent complexType interlacing */
            while (!p_a_xsdTags[i_tempMin].StartsWith("<xs:complextype", StringComparison.CurrentCultureIgnoreCase))
            {
                if (i_tempMin == p_i_max)
                {
                    /* forbidden state - interlacing is not valid in xsd-schema */
                    throw new ArgumentException("Invalid nested xsd-tag xs:simpleType at(" + (p_i_min + 1) + ".-element) \"" + p_a_xsdTags[p_i_min] + "\".");
                }

                b_complexTypeFirst = false;
                i_tempMin++;
            }

            /* end tag pointer for nested xs:simpleContent */
            int i_tempMax = p_i_min;

            /* find end of simpleType interlacing */
            while (!p_a_xsdTags[i_tempMax].StartsWith("</xs:complextype", StringComparison.CurrentCultureIgnoreCase))
            {
                if (i_tempMax == p_i_max)
                {
                    /* forbidden state - interlacing is not valid in xsd-schema */
                    throw new ArgumentException("Invalid nested xsd-tag xs:simpleContent at(" + (p_i_min + 1) + ".-element) \"" + p_a_xsdTags[p_i_min] + "\".");
                }

                i_tempMax++;
            }

            /* set overwrite value for end tag pointer */
            i_nestedMax = i_tempMax;

            if (!b_complexTypeFirst)
            {
                i_nestedMax++;
            }

            ForestNET.Lib.Global.ILogFiner("iterate simpleContent");
            ForestNET.Lib.Global.ILogFiner("\t\t" + (i_tempMin + 1) + " ... " + (i_tempMax - 1));
            ForestNET.Lib.Global.ILogFiner("\t\t" + p_a_xsdTags[i_tempMin + 1] + " ... " + p_a_xsdTags[i_tempMax - 1]);

            /* parse content of xs:simpleContent */
            for (++i_tempMin; i_tempMin <= (i_tempMax - 1); i_tempMin++)
            {
                /* get xsd type */
                XSDType e_xsdSimpleContentTagType = XML.GetXSDType(p_a_xsdTags, i_tempMin);

                if (e_xsdSimpleContentTagType == XSDType.Extension)
                {
                    ForestNET.Lib.Global.ILogFinest("found extension = \"" + p_a_xsdTags[i_tempMin] + "\"");

                    bool b_base_processed = false;

                    /* read base attribute out of xs:extension tag as primitive type */
                    o_regex = new System.Text.RegularExpressions.Regex("base=\"xs:([^\"]*)\"");
                    o_matcher = o_regex.Match(p_a_xsdTags[i_tempMin]);

                    if (o_matcher.Success)
                    {
                        /* check for name attribute of xs:element tag if we read a primitive type for simple content */
                        if (!b_foundName)
                        {
                            /* no name attribute found */
                            throw new ArgumentException("Invalid xsd-tag xs:element without a name at(" + (p_i_min + 1) + ".-element) \"" + p_a_xsdTags[p_i_min] + "\".");
                        }

                        ForestNET.Lib.Global.ILogFinest("found primitive type: " + o_matcher.Value.Substring(9, o_matcher.Value.Length - 1 - 9));

                        o_xsdElement.Type = o_matcher.Value.Substring(9, o_matcher.Value.Length - 1 - 9);
                        b_base_processed = true;
                    }
                    else
                    {
                        /* read base attribute out of xs:extension tag as reference type */
                        o_regex = new System.Text.RegularExpressions.Regex("base=\"([^\"]*)\"");
                        o_matcher = o_regex.Match(p_a_xsdTags[i_tempMin]);

                        if (o_matcher.Success)
                        {
                            ForestNET.Lib.Global.ILogFinest("found reference type: " + o_matcher.Value.Substring(6, o_matcher.Value.Length - 1 - 6));

                            string s_referenceName = o_matcher.Value.Substring(6, o_matcher.Value.Length - 1 - 6);

                            /* check if we have a duplicate */
                            if (this.GetXSDElement(s_referenceName) != null)
                            {
                                throw new ArgumentException("Invalid xsd-tag xs:element (duplicate) at(" + (p_i_min + 1) + ".-element) \"" + p_a_xsdTags[p_i_min] + "\".");
                            }

                            if (this.XsdElementDefinitionExist(s_referenceName))
                            {
                                ForestNET.Lib.Global.ILogFinest("add and check element reference = " + s_referenceName);

                                XSDElement o_xsdElementReference = this.GetXSDElementDefinition(s_referenceName) ?? throw new NullReferenceException("xsd element definition not found with reference '" + s_referenceName + "'");

                                /* overwrite type value */
                                o_xsdElement.Type = o_xsdElementReference.Type;

                                /* overwrite name value */
                                if (!b_foundName)
                                {
                                    o_xsdElement.Name = o_xsdElementReference.Name;
                                }

                                /* overwrite mapping value */
                                if (!b_foundMapping)
                                {
                                    o_xsdElement.Mapping = o_xsdElementReference.Mapping;
                                }

                                /* library does not support multiple occurrences of the same xml element without a list definition */
                                if ((!o_xsdElement.Choice) && (this.o_currentElement != null) && (!this.o_currentElement.Mapping.Contains("ArrayList(")) && (o_xsdElement.MaxOccurs > 1))
                                {
                                    throw new ArgumentException("Library does not support multiple occurrences of the same xml element without a list definition in xsd-schema at(" + (p_i_min + 1) + ".-element) \"" + p_a_xsdTags[p_i_min] + "\".");
                                }
                            }
                            else
                            {
                                throw new ArgumentException("Invalid xsd-tag xs:element with unknown reference at(" + (p_i_min + 1) + ".-element) \"" + p_a_xsdTags[p_i_min] + "\".");
                            }

                            b_base_processed = true;
                        }
                    }

                    if (!b_base_processed)
                    {
                        throw new ArgumentException("Invalid xsd-tag xs:extension without a valid base-type at(" + (i_tempMin + 1) + ".-element) \"" + p_a_xsdTags[i_tempMin] + "\".");
                    }
                }

                if (!p_a_xsdTags[i_tempMin].EndsWith("/>"))
                {
                    if ((e_xsdSimpleContentTagType == XSDType.Attribute) && (XML.GetXSDType(p_a_xsdTags, i_tempMin + 1) == XSDType.SimpleType))
                    {
                        /* parse xs:attribute with simpleType */
                        XSDAttribute o_xsdAttribute = this.ParseXSDAttributeWithSimpleType(p_a_xsdTags, i_tempMin);

                        /* add xs:attribute to xsd element */
                        o_xsdElement.Attributes.Add(o_xsdAttribute);
                    }

                    /* if we still have no closing tag, then our xsd schema is invalid */
                    if (!p_a_xsdTags[i_tempMax].StartsWith("</"))
                    {
                        throw new ArgumentException("Invalid xsd-tag is not closed in xsd-schema at(" + (i_tempMin + 1) + ".-element) \"" + p_a_xsdTags[i_tempMin] + "\".");
                    }

                    /* get xsd end type */
                    XSDType e_xsdSimpleContentEndTagType = XML.GetXSDType(p_a_xsdTags, (i_tempMax - 1));

                    /* xsd type and xsd close type must match */
                    if (e_xsdSimpleContentTagType != e_xsdSimpleContentEndTagType)
                    {
                        throw new ArgumentException("Invalid xsd-tag-type \"" + p_a_xsdTags[i_tempMax - 1] + "\" for closing in xsd-schema at(" + (i_tempMin + 1) + ".-element) \"" + p_a_xsdTags[i_tempMin] + "\".");
                    }
                    else
                    {
                        i_tempMax--;
                    }
                }
                else
                {
                    if (e_xsdSimpleContentTagType == XSDType.Attribute)
                    {
                        ForestNET.Lib.Global.ILogFinest("found attribute item = \"" + p_a_xsdTags[i_tempMin] + "\"");
                        o_regex = new System.Text.RegularExpressions.Regex("ref=\"([^\"]*)\"");
                        o_matcher = o_regex.Match(p_a_xsdTags[i_tempMin]);

                        o_regex = new System.Text.RegularExpressions.Regex("name=\"([^\"]*)\"");
                        System.Text.RegularExpressions.Match o_matcherName = o_regex.Match(p_a_xsdTags[i_tempMin]);

                        o_regex = new System.Text.RegularExpressions.Regex("type=\"([^\"]*)\"");
                        System.Text.RegularExpressions.Match o_matcherType = o_regex.Match(p_a_xsdTags[i_tempMin]);

                        if (o_matcher.Success)
                        { /* we have an attribute element with reference */
                            string s_referenceName = o_matcher.Value.Substring(5, o_matcher.Value.Length - 1 - 5);

                            /* check if we have a duplicate */
                            if (this.GetXSDAttribute(s_referenceName) != null)
                            {
                                throw new ArgumentException("Invalid xsd-tag xs:attribute (duplicate) at(" + (i_tempMin + 1) + ".-element) \"" + p_a_xsdTags[i_tempMin] + "\".");
                            }

                            if (this.XsdAttributeDefinitionExist(s_referenceName))
                            {
                                ForestNET.Lib.Global.ILogFinest("add and check attribute reference = " + s_referenceName);

                                XSDAttribute o_xsdAttribute = this.GetXSDAttributeDefinition(s_referenceName) ?? throw new NullReferenceException("xsd attribute definition not found with reference '" + s_referenceName + "'");

                                /* read name attribute out of xs:attribute tag */
                                if ((o_matcherName.Success))
                                {
                                    string s_name = o_matcherName.Value.Substring(6, o_matcherName.Value.Length - 1 - 6);

                                    /* overwrite name value from reference, because it is dominant from the usage point */
                                    o_xsdAttribute.Name = s_name;
                                }

                                /* add xs:attribute to xsd element */
                                o_xsdElement.Attributes.Add(o_xsdAttribute);
                            }
                            else
                            {
                                throw new ArgumentException("Invalid xsd-tag xs:attribute with unknown reference at(" + (i_tempMin + 1) + ".-element) \"" + p_a_xsdTags[i_tempMin] + "\".");
                            }
                        }
                        else if ((o_matcherName.Success) && (o_matcherType.Success))
                        { /* we have an attribute element with name and type */
                            /* read name and type attribute values of xs:attribute tag */
                            string s_name = o_matcherName.Value.Substring(6, o_matcherName.Value.Length - 1 - 6);
                            string s_type = o_matcherType.Value.Substring(6, o_matcherType.Value.Length - 1 - 6);

                            /* check if we have a duplicate */
                            if (this.GetXSDAttribute(s_type) != null)
                            {
                                throw new ArgumentNullException("Invalid xsd-tag xs:attribute (duplicate) at(" + (i_tempMin + 1) + ".-element) \"" + p_a_xsdTags[i_tempMin] + "\".");
                            }

                            if (this.XsdAttributeDefinitionExist(s_type))
                            {
                                ForestNET.Lib.Global.ILogFinest("add and check attribute type reference = " + s_type);

                                XSDAttribute o_xsdAttribute = this.GetXSDAttributeDefinition(s_type) ?? throw new NullReferenceException("xsd attribute definition not found with type '" + s_type + "'");

                                /* overwrite name value from reference, because it is dominant from the usage point */
                                o_xsdAttribute.Name = s_name;

                                /* add xs:attribute object to xsd element */
                                o_xsdElement.Attributes.Add(o_xsdAttribute);
                            }
                            else
                            {
                                throw new ArgumentException("Invalid xsd-tag xs:attribute with unknown type reference at(" + (i_tempMin + 1) + ".-element) \"" + p_a_xsdTags[i_tempMin] + "\".");
                            }
                        }
                        else
                        {
                            /* parse xs:attribute */
                            XSDAttribute o_xsdAttribute = this.ParseXSDAttribute(p_a_xsdTags, i_tempMin);

                            /* add xs:attribute to xsd element */
                            o_xsdElement.Attributes.Add(o_xsdAttribute);
                        }
                    }
                }
            }

            /* add xs:simpleContent as xs:element to current element */
            this.o_currentElement?.Children.Add(o_xsdElement);

            return i_nestedMax;
        }

        /// <summary>
        /// Check if a xsd element definition exists by reference name
        /// </summary>
        /// <param name="p_s_referenceName">reference name of xsd element definition</param>
        /// <returns>true - definition exists, false - definition does not exist</returns>
        private bool XsdElementDefinitionExist(string p_s_referenceName)
        {
            bool b_found = false;

            foreach (XSDElement o_xsdElement in this.a_elementDefinitons)
            {
                if (o_xsdElement.Name.Equals(p_s_referenceName))
                {
                    b_found = true;
                }
            }

            return b_found;
        }

        /// <summary>
        /// Check if a xsd element definition exists by element object as clone
        /// </summary>
        /// <param name="p_o_xsdElement">xsd element object</param>
        /// <returns>true - definition exists as clone, false - definition does not exist as clone, but with other values</returns>
        private bool XsdElementDefinitionExistAsClone(XSDElement p_o_xsdElement)
        {
            bool b_found = false;

            foreach (XSDElement o_xsdElement in this.a_elementDefinitons)
            {
                if ((o_xsdElement.Name.Equals(p_o_xsdElement.Name)) && (o_xsdElement.IsEqual(p_o_xsdElement)))
                {
                    b_found = true;
                }
            }

            return b_found;
        }

        /// <summary>
        /// Get xsd element definition exists by reference name
        /// </summary>
        /// <param name="p_s_referenceName">reference name of xsd element definition</param>
        /// <returns>xsd element object as xsd definition</returns>
        private XSDElement? GetXSDElementDefinition(string p_s_referenceName)
        {
            XSDElement? o_xsdElement = null;

            foreach (XSDElement o_xsdElementObject in this.a_elementDefinitons)
            {
                if (o_xsdElementObject.Name.Equals(p_s_referenceName))
                {
                    o_xsdElement = o_xsdElementObject;
                }
            }

            return o_xsdElement;
        }

        /// <summary>
        /// Get xsd element exists by reference name
        /// </summary>
        /// <param name="p_s_referenceName">reference name of xsd element</param>
        /// <returns>xsd element object</returns>
        private XSDElement? GetXSDElement(string p_s_referenceName)
        {
            return this.GetXSDElement(p_s_referenceName, null);
        }

        /// <summary>
        /// Get xsd element exists by reference name
        /// </summary>
        /// <param name="p_s_referenceName">reference name of xsd element</param>
        /// <param name="p_o_xsdElement">look within xsd element object parameter</param>
        /// <returns>xsd element object</returns>
        private XSDElement? GetXSDElement(string p_s_referenceName, XSDElement? p_o_xsdElement)
        {
            XSDElement? o_xsdElement = null;

            if (p_o_xsdElement == null)
            {
                p_o_xsdElement = this.o_currentElement;
            }

            if (p_o_xsdElement != null)
            {
                foreach (XSDElement o_xsdElementObject in p_o_xsdElement.Children)
                {
                    if (o_xsdElementObject.Name.Equals(p_s_referenceName))
                    {
                        o_xsdElement = o_xsdElementObject.Clone();
                    }
                }
            }

            return o_xsdElement;
        }

        /// <summary>
        /// Check if a xsd attribute definition exists by reference name
        /// </summary>
        /// <param name="p_s_referenceName">reference name of xsd attribute definition</param>
        /// <returns>true - attribute exists, false - attribute does not exist</returns>
        private bool XsdAttributeDefinitionExist(string p_s_referenceName)
        {
            bool b_found = false;

            foreach (XSDAttribute o_xsdAttribute in this.a_attributeDefinitions)
            {
                if (o_xsdAttribute.Name.Equals(p_s_referenceName))
                {
                    b_found = true;
                }
            }

            return b_found;
        }

        /// <summary>
        /// Check if a xsd attribute definition exists by attribute object as clone
        /// </summary>
        /// <param name="p_o_xsdAttribute">xsd attribute object</param>
        /// <returns>true - attribute exists as clone, false - attribute does not exist as clone, but with other values</returns>
        private bool XsdAttributeDefinitionExistAsClone(XSDAttribute p_o_xsdAttribute)
        {
            bool b_found = false;

            foreach (XSDAttribute o_xsdAttribute in this.a_attributeDefinitions)
            {
                if ((o_xsdAttribute.Name.Equals(p_o_xsdAttribute.Name)) && (o_xsdAttribute.IsEqual(p_o_xsdAttribute)))
                {
                    b_found = true;
                }
            }

            return b_found;
        }

        /// <summary>
        /// Get xsd attribute definition exists by reference name
        /// </summary>
        /// <param name="p_s_referenceName">reference name of xsd attribute definition</param>
        /// <returns>xsd attribute object as xsd attribute definition</returns>
        private XSDAttribute? GetXSDAttributeDefinition(string p_s_referenceName)
        {
            XSDAttribute? o_xsdAttribute = null;

            foreach (XSDAttribute o_xsdAttributeObject in this.a_attributeDefinitions)
            {
                if (o_xsdAttributeObject.Name.Equals(p_s_referenceName))
                {
                    o_xsdAttribute = o_xsdAttributeObject;
                }
            }

            return o_xsdAttribute;
        }

        /// <summary>
        /// Get xsd attribute exists by reference name
        /// </summary>
        /// <param name="p_s_referenceName">reference name of xsd attribute</param>
        /// <param name="p_o_xsdElement">look within xsd element object parameter</param>
        /// <returns>xsd attribute object</returns>
        private XSDAttribute? GetXSDAttribute(string p_s_referenceName)
        {
            return this.GetXSDAttribute(p_s_referenceName, null);
        }

        /// <summary>
        /// Get xsd attribute exists by reference name
        /// </summary>
        /// <param name="p_s_referenceName">reference name of xsd attribute</param>
        /// <returns>xsd attribute object</returns>
        private XSDAttribute? GetXSDAttribute(string p_s_referenceName, XSDElement? p_o_xsdElement)
        {
            XSDAttribute? o_xsdAttribute = null;

            if (p_o_xsdElement == null)
            {
                p_o_xsdElement = this.o_currentElement;
            }

            if (p_o_xsdElement != null)
            {
                foreach (XSDAttribute o_xsdAttributeObject in p_o_xsdElement.Attributes)
                {
                    if (o_xsdAttributeObject.Name.Equals(p_s_referenceName))
                    {
                        o_xsdAttribute = o_xsdAttributeObject.Clone();
                    }
                }
            }

            return o_xsdAttribute;
        }

        /* simplify chaotic divided schema */

        /// <summary>
        /// (ALPHA - NOT TESTED - MIGHT NOT WORK) Simplifies a chaotic divided xsd schema file and sort it to ordered element, attributes and complex type tags
        /// </summary>
        /// <param name="p_s_source">full-path to xsd schema source file</param>
        /// <param name="p_s_destination">full-path to xsd schema destination file, will be overwritten if it exists</param>
        /// <exception cref="ArgumentException">value/structure within xsd schema file invalid</exception>
        /// <exception cref="ArgumentNullException">xsd schema not possible, node is null or a duplicate</exception>
        /// <exception cref="System.IO.IOException">cannot access or open xsd file and it's content</exception>
        public void SimplifyChaoticDividedSchemaFile(string p_s_source, string p_s_destination)
        {
            /* initialize helper lists */
            this.a_elementDefinitons = [];
            this.a_attributeDefinitions = [];
            this.a_temp = [];
            this.a_dividedElements = [];

            this.i_level = 0;

            /* check if file exists */
            if (!File.Exists(p_s_source))
            {
                throw new ArgumentException("File[" + p_s_source + "] does not exist.");
            }

            /* open xsd-schema source file */
            File o_file = new(p_s_source, false);

            System.Text.StringBuilder o_stringBuilder = new();

            /* read all xsd schema file lines to one string builder */
            foreach (string s_line in o_file.FileContentAsList ?? throw new FileLoadException("File[" + p_s_source + "] has no content."))
            {
                o_stringBuilder.Append(s_line);
            }

            /* read all xsd-schema file lines and delete all line-wraps and tabs and values only containing white spaces */
            string s_xsd = o_stringBuilder.ToString();
            s_xsd = new System.Text.RegularExpressions.Regex("[\\r\\n\\t]").Replace(s_xsd, "");
            s_xsd = new System.Text.RegularExpressions.Regex(">\\s*<").Replace(s_xsd, "><");

            /* clean up xsd-schema */
            s_xsd = new System.Text.RegularExpressions.Regex("<\\?(.*?)\\?>").Replace(s_xsd, "");
            s_xsd = new System.Text.RegularExpressions.Regex("<!--(.*?)-->").Replace(s_xsd, "");

            /* look for targetNamespace in schema-tag */
            if (s_xsd.StartsWith("<xs:schema"))
            {
                string s_foo = s_xsd.Substring(0, s_xsd.IndexOf(">"));

                if (s_foo.Contains("targetnamespace=\""))
                {
                    s_foo = s_foo.Substring(s_foo.IndexOf("targetnamespace=\"") + 17);
                    s_foo = s_foo.Substring(0, s_foo.IndexOf("\""));

                    this.TargetNamespace = s_foo;
                }
                else if (s_foo.Contains("targetNamespace=\""))
                {
                    s_foo = s_foo.Substring(s_foo.IndexOf("targetNamespace=\"") + 17);
                    s_foo = s_foo.Substring(0, s_foo.IndexOf("\""));

                    this.TargetNamespace = s_foo;
                }
            }

            /* remove schema and annotation tags */
            s_xsd = new System.Text.RegularExpressions.Regex("<xs:schema(.*?)>").Replace(s_xsd, "");
            s_xsd = new System.Text.RegularExpressions.Regex("</xs:schema>").Replace(s_xsd, "");
            s_xsd = new System.Text.RegularExpressions.Regex("<xs:annotation>(.*?)</xs:annotation>").Replace(s_xsd, "");

            if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("cleaned up xsd-schema: " + this.s_lineBreak + s_xsd);

            /* validate xsd */
            System.Text.RegularExpressions.Regex o_regex = new("(<[^<>]*?<[^<>]*?>|<[^<>]*?>[^<>]*?>)");
            System.Text.RegularExpressions.MatchCollection o_matcher = o_regex.Matches(s_xsd);

            /* if regex-matcher has match, the xsd-schema is not valid */
            if (o_matcher.Count > 0)
            {
                throw new ArgumentException("Invalid xsd-schema. Please check xsd-schema at \"" + o_matcher[0] + "\".");
            }

            ForestNET.Lib.Global.ILogConfig("split xsd-schema into xsd-elements");

            List<string> a_xsdTags = [];

            /* add all xsd-schema-tags to a list for parsing */
            o_regex = new System.Text.RegularExpressions.Regex("<[^<>]*?>");
            o_matcher = o_regex.Matches(s_xsd);

            if (o_matcher.Count > 0)
            {
                for (int i = 0; i < o_matcher.Count; i++)
                {
                    a_xsdTags.Add(o_matcher[i].ToString());
                }
            }

            /* list for indexes of closing tags for deletion */
            List<int> a_deleteTags = [];

            /* check for element definitions which have not any interlacing content, so these can be converted to one-liner */
            for (int i_min = 0; i_min <= a_xsdTags.Count - 1; i_min++)
            {
                if (i_min > 0)
                {
                    if ((a_xsdTags[i_min].Equals("</xs:element>")) && (a_xsdTags[i_min - 1].StartsWith("<xs:element")))
                    {
                        /* make element definition one liner */
                        a_xsdTags[i_min - 1] = a_xsdTags[i_min - 1].Substring(0, a_xsdTags[i_min - 1].Length - 1) + "/>";
                        /* remember element closing tag for deletion */
                        a_deleteTags.Add(i_min);
                    }
                }
            }

            int i_count = 0;

            /* iterate all deletion indexes */
            foreach (int i_delete in a_deleteTags)
            {
                /* use overall count to delete the correct index in this for each loop */
                a_xsdTags.RemoveAt(i_delete - (i_count++));
            }

            /* check if xsd-schema starts with xs:element */
            if ((!a_xsdTags[0].StartsWith("<xs:element", StringComparison.CurrentCultureIgnoreCase)) && (!a_xsdTags[0].StartsWith("<xs:complextype", StringComparison.CurrentCultureIgnoreCase)))
            {
                throw new ArgumentException("xsd-schema must start with <xs:element>-tag or with <xs:complexType>-tag.");
            }

            /* simplify chaotic divided schema */
            SimplifyChaoticDividedSchema(a_xsdTags, 0, a_xsdTags.Count - 1);

            ForestNET.Lib.Global.ILogConfig("simplified xsd-schema");

            /* build new xsd schema document content */
            o_stringBuilder = this.PrintOutSimplifiedDividedSchema();

            /* check if destination file exists, if true -> delete it */
            if (File.Exists(p_s_destination))
            {
                File.DeleteFile(p_s_destination);
            }

            /* create xsd-schema destination file */
            File o_fileDestination = new(p_s_destination, true);

            /* write xsd schema document content to file */
            o_fileDestination.ReplaceContent(o_stringBuilder.ToString());
        }

        /// <summary>
        /// Build new xsd schema document content
        /// </summary>
        /// <returns>stringBuilder instance with document content</returns>
        private System.Text.StringBuilder PrintOutSimplifiedDividedSchema()
        {
            /* list of primitive xsd types */
            List<string> a_primitiveTypes = [ "string", "duration", "hexbinary", "base64binary", "anyuri", "normalizedstring", "token",
                    "language", "name", "ncname", "nmtoken", "id", "idref", "entity", "integer", "int", "positiveinteger", "nonpositiveinteger", "negativeinteger",
                    "nonnegativeinteger", "byte", "unsignedint", "unsignedbyte", "boolean", "duration", "date", "time", "datetime", "decimal", "double", "float", "short", "long" ];

            System.Text.StringBuilder o_stringBuilder = new();

            /* xml document tag */
            o_stringBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>").Append(this.s_lineBreak);

            /* add schema tag */
            o_stringBuilder.Append("<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" elementFormDefault=\"qualified\" attributeFormDefault=\"unqualified\">").Append(this.s_lineBreak);
            o_stringBuilder.Append(this.s_lineBreak);

            /* definition of simple elements */
            o_stringBuilder.Append("<!-- definition of simple elements -->").Append(this.s_lineBreak);

            foreach (XSDElement o_element in this.a_elementDefinitons)
            {
                if (o_element.SimpleType)
                { /* element with simple type */
                    o_stringBuilder.Append("<xs:simpleType name=\"" + o_element.Name + "\"");

                    if (!ForestNET.Lib.Helper.IsStringEmpty(o_element.Mapping))
                    {
                        o_stringBuilder.Append(" mapping=\"" + o_element.Mapping + "\"");
                    }

                    if (o_element.MinOccurs != 1)
                    {
                        o_stringBuilder.Append(" minOccurs=\"" + o_element.MinOccurs + "\"");
                    }

                    if (o_element.MaxOccurs < 0)
                    {
                        o_stringBuilder.Append(" maxOccurs=\"unbounded\"");
                    }
                    else if (o_element.MaxOccurs != 1)
                    {
                        o_stringBuilder.Append(" maxOccurs=\"" + o_element.MaxOccurs + "\"");
                    }

                    o_stringBuilder.Append('>').Append(this.s_lineBreak);

                    if (o_element.Restrictions.Count > 0)
                    {
                        o_stringBuilder.Append("\t" + "<xs:restriction base=\"xs:" + o_element.Type + "\">").Append(this.s_lineBreak);

                        foreach (XSDRestriction o_restriction in o_element.Restrictions)
                        {
                            o_stringBuilder.Append("\t\t" + "<xs:" + o_restriction.Name + " value=\"" + ((!ForestNET.Lib.Helper.IsStringEmpty(o_restriction.StrValue)) ? o_restriction.StrValue : o_restriction.IntValue.ToString()) + "\"/>").Append(this.s_lineBreak);
                        }

                        o_stringBuilder.Append("\t" + "</xs:restriction>").Append(this.s_lineBreak);
                    }
                    else
                    {
                        o_stringBuilder.Append("\t" + "<xs:restriction base=\"xs:" + o_element.Type + "\"/>").Append(this.s_lineBreak);
                    }

                    o_stringBuilder.Append("</xs:simpleType>").Append(this.s_lineBreak);
                }
                else
                { /* element one liner */
                    o_stringBuilder.Append("<xs:element name=\"" + o_element.Name + "\" type=\"xs:" + o_element.Type + "\"");

                    if (!ForestNET.Lib.Helper.IsStringEmpty(o_element.Mapping))
                    {
                        o_stringBuilder.Append(" mapping=\"" + o_element.Mapping + "\"");
                    }

                    if (o_element.MinOccurs != 1)
                    {
                        o_stringBuilder.Append(" minOccurs=\"" + o_element.MinOccurs + "\"");
                    }

                    if (o_element.MaxOccurs < 0)
                    {
                        o_stringBuilder.Append(" maxOccurs=\"unbounded\"");
                    }
                    else if (o_element.MaxOccurs != 1)
                    {
                        o_stringBuilder.Append(" maxOccurs=\"" + o_element.MaxOccurs + "\"");
                    }

                    o_stringBuilder.Append("/>").Append(this.s_lineBreak);
                }
            }

            /* definition of attributes */
            o_stringBuilder.Append(this.s_lineBreak);
            o_stringBuilder.Append("<!-- definition of attributes -->").Append(this.s_lineBreak);

            foreach (XSDAttribute o_attribute in this.a_attributeDefinitions)
            {
                /* attribute with simple type */
                if (o_attribute.SimpleType)
                {
                    o_stringBuilder.Append("<xs:attribute name=\"" + o_attribute.Name + "\"");

                    if (!ForestNET.Lib.Helper.IsStringEmpty(o_attribute.Mapping))
                    {
                        o_stringBuilder.Append(" mapping=\"" + o_attribute.Mapping + "\"");
                    }

                    if (o_attribute.Required)
                    {
                        o_stringBuilder.Append(" use=\"required\"");
                    }

                    if (!ForestNET.Lib.Helper.IsStringEmpty(o_attribute.Default))
                    {
                        o_stringBuilder.Append(" default=\"" + o_attribute.Default + "\"");
                    }

                    if (!ForestNET.Lib.Helper.IsStringEmpty(o_attribute.Fixed))
                    {
                        o_stringBuilder.Append(" fixed=\"" + o_attribute.Fixed + "\"");
                    }

                    o_stringBuilder.Append('>').Append(this.s_lineBreak);
                    o_stringBuilder.Append("\t" + "<xs:simpleType>").Append(this.s_lineBreak);

                    if (o_attribute.Restrictions.Count > 0)
                    {
                        o_stringBuilder.Append("\t\t" + "<xs:restriction base=\"xs:" + o_attribute.Type + "\">").Append(this.s_lineBreak);

                        foreach (XSDRestriction o_restriction in o_attribute.Restrictions)
                        {
                            o_stringBuilder.Append("\t\t\t" + "<xs:" + o_restriction.Name + " value=\"" + ((!ForestNET.Lib.Helper.IsStringEmpty(o_restriction.StrValue)) ? o_restriction.StrValue : o_restriction.IntValue.ToString()) + "\"/>").Append(this.s_lineBreak);
                        }

                        o_stringBuilder.Append("\t\t" + "</xs:restriction>").Append(this.s_lineBreak);
                    }
                    else
                    {
                        o_stringBuilder.Append("\t\t" + "<xs:restriction base=\"xs:" + o_attribute.Type + "\"/>").Append(this.s_lineBreak);
                    }

                    o_stringBuilder.Append("\t" + "</xs:simpleType>").Append(this.s_lineBreak);
                    o_stringBuilder.Append("</xs:attribute>").Append(this.s_lineBreak);
                }
                else
                { /* attribute one liner */
                    o_stringBuilder.Append("<xs:attribute name=\"" + o_attribute.Name + "\" type=\"xs:" + o_attribute.Type + "\"");

                    if (!ForestNET.Lib.Helper.IsStringEmpty(o_attribute.Mapping))
                    {
                        o_stringBuilder.Append(" mapping=\"" + o_attribute.Mapping + "\"");
                    }

                    if (o_attribute.Required)
                    {
                        o_stringBuilder.Append(" use=\"required\"");
                    }

                    if (!ForestNET.Lib.Helper.IsStringEmpty(o_attribute.Default))
                    {
                        o_stringBuilder.Append(" default=\"" + o_attribute.Default + "\"");
                    }

                    if (!ForestNET.Lib.Helper.IsStringEmpty(o_attribute.Fixed))
                    {
                        o_stringBuilder.Append(" fixed=\"" + o_attribute.Fixed + "\"");
                    }

                    o_stringBuilder.Append("/>").Append(this.s_lineBreak);
                }
            }

            /* definition of complex types */
            o_stringBuilder.Append(this.s_lineBreak);
            o_stringBuilder.Append("<!-- definition of complex types -->").Append(this.s_lineBreak);

            foreach (XSDElement o_complexType in this.a_dividedElements)
            {
                /* complex type with simple content */
                if (o_complexType.SimpleContent)
                {
                    o_stringBuilder.Append("<xs:complexType name=\"" + o_complexType.Name + "\"");

                    if (!ForestNET.Lib.Helper.IsStringEmpty(o_complexType.Mapping))
                    {
                        o_stringBuilder.Append(" mapping=\"" + o_complexType.Mapping + "\"");
                    }

                    if (o_complexType.MinOccurs != 1)
                    {
                        o_stringBuilder.Append(" minOccurs=\"" + o_complexType.MinOccurs + "\"");
                    }

                    if (o_complexType.MaxOccurs < 0)
                    {
                        o_stringBuilder.Append(" maxOccurs=\"unbounded\"");
                    }
                    else if (o_complexType.MaxOccurs != 1)
                    {
                        o_stringBuilder.Append(" maxOccurs=\"" + o_complexType.MaxOccurs + "\"");
                    }

                    o_stringBuilder.Append('>').Append(this.s_lineBreak);

                    o_stringBuilder.Append("\t" + "<xs:simpleContent>").Append(this.s_lineBreak);

                    if (o_complexType.Attributes.Count > 0)
                    {
                        o_stringBuilder.Append("\t\t" + "<xs:extension base=\"" + o_complexType.Type + "\">").Append(this.s_lineBreak);

                        /* attributes to simple content */
                        foreach (XSDAttribute o_attribute in o_complexType.Attributes)
                        {
                            if (!ForestNET.Lib.Helper.IsStringEmpty(o_attribute.Reference))
                            {
                                if (!ForestNET.Lib.Helper.IsStringEmpty(o_attribute.Name))
                                {
                                    o_stringBuilder.Append("\t\t\t" + "<xs:attribute name=\"" + o_attribute.Name + "\" type=\"" + o_attribute.Reference + "\"/>").Append(this.s_lineBreak);
                                }
                                else
                                {
                                    o_stringBuilder.Append("\t\t\t" + "<xs:attribute ref=\"" + o_attribute.Reference + "\"/>").Append(this.s_lineBreak);
                                }
                            }
                            else
                            {
                                o_stringBuilder.Append("\t\t\t" + "<xs:attribute name=\"" + o_attribute.Name + "\" type=\"" + o_attribute.Type + "\"");

                                if (!ForestNET.Lib.Helper.IsStringEmpty(o_attribute.Mapping))
                                {
                                    o_stringBuilder.Append(" mapping=\"" + o_attribute.Mapping + "\"");
                                }

                                if (o_attribute.Required)
                                {
                                    o_stringBuilder.Append(" use=\"required\"");
                                }

                                if (!ForestNET.Lib.Helper.IsStringEmpty(o_attribute.Default))
                                {
                                    o_stringBuilder.Append(" default=\"" + o_attribute.Default + "\"");
                                }

                                if (!ForestNET.Lib.Helper.IsStringEmpty(o_attribute.Fixed))
                                {
                                    o_stringBuilder.Append(" fixed=\"" + o_attribute.Fixed + "\"");
                                }

                                o_stringBuilder.Append("/>").Append(this.s_lineBreak);
                            }
                        }

                        o_stringBuilder.Append("\t\t" + "</xs:extension>").Append(this.s_lineBreak);
                    }
                    else
                    {
                        o_stringBuilder.Append("\t\t" + "<xs:extension base=\"" + o_complexType.Type + "\"/>").Append(this.s_lineBreak);
                    }

                    o_stringBuilder.Append("\t" + "</xs:simpleContent>").Append(this.s_lineBreak);

                    o_stringBuilder.Append("</xs:complexType>").Append(this.s_lineBreak).Append(this.s_lineBreak);
                }
                else
                { /* default complex type */
                    o_stringBuilder.Append("<xs:complexType name=\"" + o_complexType.Name + "\"");

                    if (!ForestNET.Lib.Helper.IsStringEmpty(o_complexType.Mapping))
                    {
                        o_stringBuilder.Append(" mapping=\"" + o_complexType.Mapping + "\"");
                    }

                    if (!o_complexType.Choice)
                    {
                        if (o_complexType.MinOccurs != 1)
                        {
                            o_stringBuilder.Append(" minOccurs=\"" + o_complexType.MinOccurs + "\"");
                        }

                        if (o_complexType.MaxOccurs < 0)
                        {
                            o_stringBuilder.Append(" maxOccurs=\"unbounded\"");
                        }
                        else if (o_complexType.MaxOccurs != 1)
                        {
                            o_stringBuilder.Append(" maxOccurs=\"" + o_complexType.MaxOccurs + "\"");
                        }
                    }

                    o_stringBuilder.Append('>').Append(this.s_lineBreak);

                    o_stringBuilder.Append("\t" + "<xs:sequence");

                    if (o_complexType.SequenceMinOccurs != 1)
                    {
                        o_stringBuilder.Append(" minOccurs=\"" + o_complexType.SequenceMinOccurs + "\"");
                    }

                    if (o_complexType.SequenceMaxOccurs < 0)
                    {
                        o_stringBuilder.Append(" maxOccurs=\"unbounded\"");
                    }
                    else if (o_complexType.SequenceMaxOccurs != 1)
                    {
                        o_stringBuilder.Append(" maxOccurs=\"" + o_complexType.SequenceMaxOccurs + "\"");
                    }

                    o_stringBuilder.Append('>').Append(this.s_lineBreak);

                    string s_additionalIndent = "";

                    if (o_complexType.Choice)
                    {
                        s_additionalIndent = "\t";

                        o_stringBuilder.Append("\t\t" + "<xs:choice");

                        if (o_complexType.MinOccurs != 1)
                        {
                            o_stringBuilder.Append(" minOccurs=\"" + o_complexType.MinOccurs + "\"");
                        }

                        if (o_complexType.MaxOccurs < 0)
                        {
                            o_stringBuilder.Append(" maxOccurs=\"unbounded\"");
                        }
                        else if (o_complexType.MaxOccurs != 1)
                        {
                            o_stringBuilder.Append(" maxOccurs=\"" + o_complexType.MaxOccurs + "\"");
                        }

                        o_stringBuilder.Append('>').Append(this.s_lineBreak);
                    }

                    /* elements in sequence of complex type */
                    foreach (XSDElement o_element in o_complexType.Children)
                    {
                        if (!ForestNET.Lib.Helper.IsStringEmpty(o_element.Reference))
                        {
                            if (!ForestNET.Lib.Helper.IsStringEmpty(o_element.Name))
                            {
                                o_stringBuilder.Append(s_additionalIndent + "\t\t" + "<xs:element name=\"" + o_element.Name + "\" type=\"" + o_element.Reference + "\"");
                            }
                            else
                            {
                                o_stringBuilder.Append(s_additionalIndent + "\t\t" + "<xs:element ref=\"" + o_element.Reference + "\"");
                            }
                        }
                        else
                        {
                            if ((!ForestNET.Lib.Helper.IsStringEmpty(o_element.Name)) && (!ForestNET.Lib.Helper.IsStringEmpty(o_element.Type)) && (!a_primitiveTypes.Contains(o_element.Type.ToLower())))
                            {
                                o_stringBuilder.Append(s_additionalIndent + "\t\t" + "<xs:element name=\"" + o_element.Name + "\" type=\"" + o_element.Type + "\"");
                            }
                            else
                            {
                                o_stringBuilder.Append(s_additionalIndent + "\t\t" + "<xs:element ref=\"" + o_element.Name + "\"");
                            }
                        }

                        if (o_element.MinOccurs != 1)
                        {
                            o_stringBuilder.Append(" minOccurs=\"" + o_element.MinOccurs + "\"");
                        }

                        if (o_element.MaxOccurs < 0)
                        {
                            o_stringBuilder.Append(" maxOccurs=\"unbounded\"");
                        }
                        else if (o_element.MaxOccurs != 1)
                        {
                            o_stringBuilder.Append(" maxOccurs=\"" + o_element.MaxOccurs + "\"");
                        }

                        o_stringBuilder.Append("/>").Append(this.s_lineBreak);
                    }

                    if (o_complexType.Choice)
                    {
                        o_stringBuilder.Append("\t\t" + "</xs:choice>").Append(this.s_lineBreak);
                    }

                    o_stringBuilder.Append("\t" + "</xs:sequence>").Append(this.s_lineBreak);

                    /* attributes after sequence of complex type */
                    foreach (XSDAttribute o_attribute in o_complexType.Attributes)
                    {
                        if (!ForestNET.Lib.Helper.IsStringEmpty(o_attribute.Reference))
                        {
                            if (!ForestNET.Lib.Helper.IsStringEmpty(o_attribute.Name))
                            {
                                o_stringBuilder.Append("\t\t\t" + "<xs:attribute name=\"" + o_attribute.Name + "\" type=\"" + o_attribute.Reference + "\"/>").Append(this.s_lineBreak);
                            }
                            else
                            {
                                o_stringBuilder.Append("\t\t\t" + "<xs:attribute ref=\"" + o_attribute.Reference + "\"/>").Append(this.s_lineBreak);
                            }
                        }
                        else
                        {
                            o_stringBuilder.Append("\t" + "<xs:attribute name=\"" + o_attribute.Name + "\" type=\"" + o_attribute.Type + "\"");

                            if (!ForestNET.Lib.Helper.IsStringEmpty(o_attribute.Mapping))
                            {
                                o_stringBuilder.Append(" mapping=\"" + o_attribute.Mapping + "\"");
                            }

                            if (o_attribute.Required)
                            {
                                o_stringBuilder.Append(" use=\"required\"");
                            }

                            if (!ForestNET.Lib.Helper.IsStringEmpty(o_attribute.Default))
                            {
                                o_stringBuilder.Append(" default=\"" + o_attribute.Default + "\"");
                            }

                            if (!ForestNET.Lib.Helper.IsStringEmpty(o_attribute.Fixed))
                            {
                                o_stringBuilder.Append(" fixed=\"" + o_attribute.Fixed + "\"");
                            }

                            o_stringBuilder.Append("/>").Append(this.s_lineBreak);
                        }
                    }

                    o_stringBuilder.Append("</xs:complexType>").Append(this.s_lineBreak).Append(this.s_lineBreak);
                }
            }

            /* close schema tag */
            o_stringBuilder.Append("</xs:schema>").Append(this.s_lineBreak);

            return o_stringBuilder;
        }

        /// <summary>
        /// Main method to simplify a chatoic divided schema, looking for definitions and complex type elements
        /// </summary>
        /// <param name="p_a_xsdTags">list of xsd element tags</param>
        /// <param name="p_i_min">pointer where to start line iteration</param>
        /// <param name="p_i_max">pointer where to end line iteration</param>
        /// <exception cref="ArgumentException">value or structure within xsd file lines invalid, not all references could be resolved</exception>
        /// <exception cref="ArgumentNullException">value within xsd content missing or min. amount not available</exception>
        private void SimplifyChaoticDividedSchema(List<string> p_a_xsdTags, int p_i_min, int p_i_max)
        {
            int i_max = p_i_max;

            int i_complexTypes = 1;
            int i_definitions = 1;

            /* iterate all elements */
            for (int i_min = p_i_min; i_min <= i_max; i_min++)
            {
                /* get xsd type */
                XSDType e_xsdTagType = XML.GetXSDType(p_a_xsdTags, i_min);

                /* first xsd tag must be of type element, complex type or simple type and not close itself */
                if (!p_a_xsdTags[i_min].EndsWith("/>"))
                {
                    /* 
	    			 * check if we have a simpleType xsd tag or
	    			 * an xs:element tag with a simple type or
	    			 * an xs:attribute tag with a simple type or
	    			 * an xs:complexType tag with a simple content in it,
	    			 * this is like a one-liner element-definition or attribute-definition */
                    if (
                        (e_xsdTagType == XSDType.SimpleType) ||
                        ((e_xsdTagType == XSDType.Element) && (XML.GetXSDType(p_a_xsdTags, i_min + 1) == XSDType.SimpleType)) ||
                        ((e_xsdTagType == XSDType.Attribute) && (XML.GetXSDType(p_a_xsdTags, i_min + 1) == XSDType.SimpleType)) ||
                        ((e_xsdTagType == XSDType.ComplexType) && (XML.GetXSDType(p_a_xsdTags, i_min + 1) == XSDType.SimpleContent))
                    )
                    {
                        int i_tempMin = i_min + 1;

                        if (e_xsdTagType == XSDType.SimpleType)
                        { /* parse simple type */
                            ForestNET.Lib.Global.ILogFiner("add element reference with simpleType of: " + p_a_xsdTags[i_min]);

                            /* parse xs:element with simpleType */
                            XSDElement o_xsdElement = this.ParseXSDElementWithSimpleType(p_a_xsdTags, i_min);

                            /* library does not support multiple occurrences of the same xml element without a list definition */
                            if (o_xsdElement.MaxOccurs > 1)
                            {
                                throw new ArgumentException("Library does not support multiple occurrences of the same xml element without a list definition in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                            }

                            /* skip flag for adding object to list */
                            bool b_skip = false;

                            /* check if xs:element definition already exists */
                            if (this.XsdElementDefinitionExist(o_xsdElement.Name))
                            {
                                if (!this.XsdElementDefinitionExistAsClone(o_xsdElement))
                                {
                                    throw new ArgumentException("Invalid xsd-element-definition (duplicate) in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                                }
                                else
                                {
                                    b_skip = true;
                                }
                            }

                            if (!b_skip)
                            {
                                /* add element definition to list */
                                this.a_elementDefinitons.Add(o_xsdElement);
                            }

                            /* look for end of nested xs:simpleType tag */
                            while ((e_xsdTagType == XSDType.SimpleType) && (!p_a_xsdTags[i_tempMin].ToLower().Equals("</xs:simpletype>")))
                            {
                                if (i_tempMin == i_max)
                                {
                                    /* forbidden state - interlacing is not valid in xsd-schema */
                                    throw new ArgumentException("Invalid nested xsd-tag xs:simpleType at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                                }

                                i_tempMin++;
                            }
                        }
                        else if (e_xsdTagType == XSDType.Element)
                        { /* parse xs:element with simple type */
                            /* parse xs:element with simple type */
                            XSDElement o_xsdElement = this.ParseXSDElementWithSimpleType(p_a_xsdTags, i_min);

                            /* skip flag for adding object to list */
                            bool b_skip = false;

                            /* check if xs:element definition already exists */
                            if (this.XsdElementDefinitionExist(o_xsdElement.Name))
                            {
                                if (!this.XsdElementDefinitionExistAsClone(o_xsdElement))
                                {
                                    throw new ArgumentException("Invalid xsd-element-definition (duplicate) in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                                }
                                else
                                {
                                    b_skip = true;
                                }
                            }

                            if (!b_skip)
                            {
                                /* add element definition to list */
                                this.a_elementDefinitons.Add(o_xsdElement);
                            }

                            /* look for end of nested xs:element tag */
                            while ((e_xsdTagType == XSDType.Element) && (!p_a_xsdTags[i_tempMin].ToLower().Equals("</xs:element>")))
                            {
                                if (i_tempMin == i_max)
                                {
                                    /* forbidden state - interlacing is not valid in xsd-schema */
                                    throw new ArgumentException("Invalid nested xsd-tag xs:element at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                                }

                                i_tempMin++;
                            }
                        }
                        else if (e_xsdTagType == XSDType.Attribute)
                        { /* parse xs:attribute with simple type */
                            /* parse xs:attribute with simple type */
                            XSDAttribute o_xsdAttribute = this.ParseXSDAttributeWithSimpleType(p_a_xsdTags, i_min);

                            /* skip flag for adding object to list */
                            bool b_skip = false;

                            /* check if xs:attribute definition already exists */
                            if (this.XsdAttributeDefinitionExist(o_xsdAttribute.Name))
                            {
                                if (!this.XsdAttributeDefinitionExistAsClone(o_xsdAttribute))
                                {
                                    throw new ArgumentException("Invalid xsd-attribute-definition (duplicate) in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                                }
                                else
                                {
                                    b_skip = true;
                                }
                            }

                            if (!b_skip)
                            {
                                /* add element definition to list */
                                this.a_attributeDefinitions.Add(o_xsdAttribute);
                            }

                            /* look for end of nested xs:element tag */
                            while ((e_xsdTagType == XSDType.Attribute) && (!p_a_xsdTags[i_tempMin].ToLower().Equals("</xs:attribute>")))
                            {
                                if (i_tempMin == i_max)
                                {
                                    /* forbidden state - interlacing is not valid in xsd-schema */
                                    throw new ArgumentException("Invalid nested xsd-tag xs:attribute at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                                }

                                i_tempMin++;
                            }
                        }
                        else if (e_xsdTagType == XSDType.ComplexType)
                        { /* parse xs:complexType with simple content */
                            /* parse xs:complexType with simple content */
                            XSDElement? o_xsdElement = this.ParseXSDComplexTypeWithSimpleContent(p_a_xsdTags, i_min);

                            /* look for end of nested xs:complexType tag */
                            while ((e_xsdTagType == XSDType.ComplexType) && (!p_a_xsdTags[i_tempMin].ToLower().Equals("</xs:complextype>")))
                            {
                                if (i_tempMin == i_max)
                                {
                                    /* forbidden state - interlacing is not valid in xsd-schema */
                                    throw new ArgumentException("Invalid nested xsd-tag xs:complexType at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                                }

                                i_tempMin++;
                            }

                            if (o_xsdElement != null)
                            {
                                /* skip flag for adding object to list */
                                bool b_skip = false;

                                /* check if xs:element definition already exists */
                                if (this.SchemaElementDefinitionExist(o_xsdElement.Name))
                                {
                                    if (!this.SchemaElementDefinitionExistAsClone(o_xsdElement))
                                    {
                                        throw new ArgumentException("Invalid xsd-element-definition (duplicate) in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                                    }
                                    else
                                    {
                                        b_skip = true;
                                    }
                                }

                                if (!b_skip)
                                {
                                    /* add element definition to list */
                                    this.a_dividedElements.Add(o_xsdElement);
                                }
                            }
                            else
                            {
                                /* store xsd start tag index and end tag index for later iteration */
                                this.a_temp.Add(new KeyValuePair<int, int>(i_min, i_tempMin));
                            }
                        }

                        i_min = i_tempMin;
                        i_definitions++;

                        continue;
                    }
                    else if ((e_xsdTagType == XSDType.Element) || (e_xsdTagType == XSDType.ComplexType))
                    { /* xsd tag is type xs:element or xs:complexType */
                        /* parse xs:element */
                        XSDElement o_xsdElement = this.ParseXSDElement(p_a_xsdTags, i_min, true);

                        int i_oldMax = i_max;
                        int i_tempMin = i_min + 1;
                        int i_level = 0;

                        /* look for end of nested xs:element or xs:complexType tag */
                        while (
                            ((e_xsdTagType == XSDType.Element) && (!p_a_xsdTags[i_tempMin].ToLower().Equals("</xs:element>"))) ||
                            ((e_xsdTagType == XSDType.ComplexType) && (!p_a_xsdTags[i_tempMin].ToLower().Equals("</xs:complextype>"))) ||
                            (i_level != 0)
                        )
                        {
                            if (e_xsdTagType == XSDType.Element)
                            {
                                /* handle other interlacing in current nested xs:element tag */
                                if ((p_a_xsdTags[i_tempMin].StartsWith("<xs:element", StringComparison.CurrentCultureIgnoreCase)) && (!p_a_xsdTags[i_tempMin].EndsWith("/>")))
                                {
                                    i_level++;
                                }
                                else if (p_a_xsdTags[i_tempMin].ToLower().Equals("</xs:element>"))
                                {
                                    i_level--;
                                }
                            }
                            else if (e_xsdTagType == XSDType.ComplexType)
                            {
                                /* handle other interlacing in current nested xs:complexType tag */
                                if ((p_a_xsdTags[i_tempMin].StartsWith("<xs:complextype", StringComparison.CurrentCultureIgnoreCase)) && (!p_a_xsdTags[i_tempMin].EndsWith("/>")))
                                {
                                    i_level++;
                                }
                                else if (p_a_xsdTags[i_tempMin].ToLower().Equals("</xs:complextype>"))
                                {
                                    i_level--;
                                }
                            }

                            if (i_level > 1)
                            {
                                /* forbidden state - interlacing is to deep.not valid in xsd-schema */
                                throw new ArgumentException("Invalid nested xsd-tag xs:element at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\". This interlacing goes to deep for a divided xsd-schema.");
                            }

                            if (i_tempMin == i_max)
                            {
                                /* forbidden state - interlacing is not valid in xsd-schema */
                                throw new ArgumentException("Invalid nested xsd-tag xs:element at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                            }

                            i_tempMin++;
                        }

                        ForestNET.Lib.Global.ILogFiner("interlacing");
                        ForestNET.Lib.Global.ILogFiner(i_min + " ... " + i_tempMin);
                        ForestNET.Lib.Global.ILogFiner(p_a_xsdTags[i_min] + " ... " + p_a_xsdTags[i_tempMin]);

                        /* parse found complex xs:element or xs:complexType tag in detail */
                        if (ParseXSDSchemaDividedElement(p_a_xsdTags, i_min, i_tempMin, o_xsdElement))
                        {
                            if ((this.SchemaElementDefinitionExist(o_xsdElement.Name)) && (!this.SchemaElementDefinitionExistAsClone(o_xsdElement)))
                            {
                                throw new ArgumentException("Invalid xsd-tag xs:element or xs:complexType already exists within schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                            }
                            else
                            {
                                /* last check if element does not already exists in definition list */
                                if (!this.SchemaElementDefinitionExistAsClone(o_xsdElement))
                                {
                                    this.a_dividedElements.Add(o_xsdElement);
                                }
                            }
                        }
                        else
                        {
                            /* store xsd start tag index and end tag index for later iteration */
                            this.a_temp.Add(new KeyValuePair<int, int>(i_min, i_tempMin));
                        }

                        i_min = i_tempMin;
                        i_max = i_oldMax;

                        i_complexTypes++;

                        continue;
                    }

                    /* decrease xml end tag counter until sequence end tag was found */
                    if (e_xsdTagType == XSDType.Sequence)
                    {
                        while (!p_a_xsdTags[i_max].ToLower().Equals("</xs:sequence>"))
                        {
                            i_max--;
                        }
                    }

                    /* if we still have no closing tag, then our xsd schema is invalid */
                    if (!p_a_xsdTags[i_max].StartsWith("</"))
                    {
                        throw new ArgumentException("Invalid xsd-tag is not closed in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                    }

                    /* get xsd end type */
                    XSDType e_xsdEndTagType = XML.GetXSDType(p_a_xsdTags, i_max);

                    /* xsd type and xsd close type must match */
                    if (e_xsdTagType != e_xsdEndTagType)
                    {
                        throw new ArgumentException("Invalid xsd-tag-type for closing in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                    }
                    else
                    {
                        i_max--;
                    }

                    i_complexTypes++;
                }
                else
                {
                    /* list of primitive xsd types */
                    List<string> a_primitiveTypes = [ "string", "duration", "hexbinary", "base64binary", "anyuri", "normalizedstring", "token",
                            "language", "name", "ncname", "nmtoken", "id", "idref", "entity", "integer", "int", "positiveinteger", "nonpositiveinteger", "negativeinteger",
                            "nonnegativeinteger", "byte", "unsignedint", "unsignedbyte", "boolean", "duration", "date", "time", "datetime", "decimal", "double", "float", "short", "long" ];

                    if (e_xsdTagType == XSDType.Element)
                    {
                        ForestNET.Lib.Global.ILogFiner("add element reference of: " + p_a_xsdTags[i_min]);

                        /* parse xs:element */
                        XSDElement o_xsdElement = this.ParseXSDElement(p_a_xsdTags, i_min);

                        /* check if element defintion on level 0 has a primitive type */
                        if (!a_primitiveTypes.Contains(o_xsdElement.Type.ToLower()))
                        {
                            throw new ArgumentException("Library does not support usage of non-primitive xsd types on element definitions on level '0' in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                        }

                        /* library does not support multiple occurrences of the same xml element without a list definition */
                        if (o_xsdElement.MaxOccurs > 1)
                        {
                            throw new ArgumentException("Library does not support multiple occurrences of the same xml element without a list definition in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                        }

                        /* skip flag for adding object to list */
                        bool b_skip = false;

                        /* check if xs:element definition already exists */
                        if (this.XsdElementDefinitionExist(o_xsdElement.Name))
                        {
                            if (!this.XsdElementDefinitionExistAsClone(o_xsdElement))
                            {
                                throw new ArgumentException("Invalid xsd-element-definition (duplicate) in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                            }
                            else
                            {
                                b_skip = true;
                            }
                        }

                        if (!b_skip)
                        {
                            /* add element definition to list */
                            this.a_elementDefinitons.Add(o_xsdElement);
                        }
                    }
                    else if (e_xsdTagType == XSDType.Attribute)
                    {
                        ForestNET.Lib.Global.ILogFiner("add attribute reference of: " + p_a_xsdTags[i_min]);

                        /* parse xs:attribute */
                        XSDAttribute o_xsdAttribute = this.ParseXSDAttribute(p_a_xsdTags, i_min);

                        /* check if element defintion on level 0 has a primitive type */
                        if (!a_primitiveTypes.Contains(o_xsdAttribute.Type.ToLower()))
                        {
                            throw new ArgumentException("Library does not support usage of non-primitive xsd types on element definitions on level '0' in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                        }

                        /* skip flag for adding object to list */
                        bool b_skip = false;

                        /* check if xs:attribute definition already exists */
                        if (this.XsdAttributeDefinitionExist(o_xsdAttribute.Name))
                        {
                            if (!this.XsdAttributeDefinitionExistAsClone(o_xsdAttribute))
                            {
                                throw new ArgumentException("Invalid xsd-attribute-definition (duplicate) in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                            }
                            else
                            {
                                b_skip = true;
                            }
                        }

                        if (!b_skip)
                        {
                            /* add attribute definition to list */
                            this.a_attributeDefinitions.Add(o_xsdAttribute);
                        }
                    }
                    else
                    {
                        /* could not parse xsd-definition */
                        throw new ArgumentException("Invalid xsd-definition in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                    }

                    i_definitions++;
                }
            }

            ForestNET.Lib.Global.ILogFiner("found definitions: " + i_definitions);
            ForestNET.Lib.Global.ILogFiner("found complex types: " + i_complexTypes);
            ForestNET.Lib.Global.ILogFiner("");
            ForestNET.Lib.Global.ILogFiner("element definitions: " + this.a_elementDefinitons.Count);
            ForestNET.Lib.Global.ILogFiner("attribute definitions: " + this.a_attributeDefinitions.Count);
            ForestNET.Lib.Global.ILogFiner("divided elements: " + this.a_dividedElements.Count);
            ForestNET.Lib.Global.ILogFiner("unresolved references: " + this.a_temp.Count);
            ForestNET.Lib.Global.ILogFiner("");

            /* max. iteration for unresolved references is 100 */
            for (int i_foo = 0; i_foo < 10; i_foo++)
            {
                /* hash map for still unresolved references */
                List<KeyValuePair<int, int>> a_anotherTemp = [];

                /* iterate each unresolved reference */
                foreach (KeyValuePair<int, int> o_entry in this.a_temp)
                {
                    /* parse xs:element again */
                    XSDElement o_xsdElement = this.ParseXSDElement(p_a_xsdTags, o_entry.Key, true);

                    /* parse found complex xs:element or xs:complexType tag in detail */
                    if (ParseXSDSchemaDividedElement(p_a_xsdTags, o_entry.Key, o_entry.Value, o_xsdElement))
                    {
                        if ((this.SchemaElementDefinitionExist(o_xsdElement.Name)) && (!this.SchemaElementDefinitionExistAsClone(o_xsdElement)))
                        {
                            throw new ArgumentException("Invalid xsd-tag xs:element or xs:complexType already exists within schema at(" + (o_entry.Key + 1) + ".-element) \"" + p_a_xsdTags[o_entry.Key] + "\".");
                        }
                        else
                        {
                            /* last check if element does not already exists in definition list */
                            if (!this.SchemaElementDefinitionExistAsClone(o_xsdElement))
                            {
                                this.a_dividedElements.Add(o_xsdElement);
                            }
                        }
                    }
                    else
                    {
                        /* store xsd start tag index and end tag index for later iteration */
                        a_anotherTemp.Add(new KeyValuePair<int, int>(o_entry.Key, o_entry.Value));
                    }
                }

                /* clear list for unresolved references */
                this.a_temp.Clear();

                /* add all still unresolved references */
                foreach (KeyValuePair<int, int> o_entry in a_anotherTemp)
                {
                    a_temp.Add(new KeyValuePair<int, int>(o_entry.Key, o_entry.Value));
                }

                /* clear hash map for still unresolved references */
                a_anotherTemp.Clear();

                ForestNET.Lib.Global.ILogFinest((i_foo + 1) + ". divided elements: " + this.a_dividedElements.Count);
                ForestNET.Lib.Global.ILogFinest((i_foo + 1) + ". unresolved references: " + this.a_temp.Count);

                /* if we have no more unresolved references, we can break iteration */
                if (this.a_temp.Count < 1)
                {
                    break;
                }
            }

            /* check if all references could be resolved */
            if (this.a_temp.Count > 0)
            {
                throw new ArgumentException("Not all references could be resolved, invalid xsd schema.");
            }

            ForestNET.Lib.Global.ILogFiner("found definitions: " + i_definitions);
            ForestNET.Lib.Global.ILogFiner("found complex types: " + i_complexTypes);
            ForestNET.Lib.Global.ILogFiner("");
            ForestNET.Lib.Global.ILogFiner("element definitions: " + this.a_elementDefinitons.Count);
            ForestNET.Lib.Global.ILogFiner("attribute definitions: " + this.a_attributeDefinitions.Count);
            ForestNET.Lib.Global.ILogFiner("divided elements: " + this.a_dividedElements.Count);
            ForestNET.Lib.Global.ILogFiner("unresolved references: " + this.a_temp.Count);
            ForestNET.Lib.Global.ILogFiner("");
        }

        /// <summary>
        /// Parse xsd content, based on XSDElement, XSDAttribute and XSDRestriction in relation to parend xsd element
        /// </summary>
        /// <param name="p_a_xsdTags">list of xsd element tags</param>
        /// <param name="p_i_min">pointer where to start line iteration</param>
        /// <param name="p_i_max">pointer where to end line iteration</param>
        /// <param name="p_o_xsdParentElement">parent xsd element object</param>
        /// <returns>true - parent xsd element object can be used for xsd schema list, false - still some references within this xsd element are not resolved</returns>
        /// <exception cref="ArgumentException">value or structure within xsd file lines invalid</exception>
        /// <exception cref="ArgumentNullException">value within xsd content missing or min. amount not available</exception>
        private bool ParseXSDSchemaDividedElement(List<string> p_a_xsdTags, int p_i_min, int p_i_max, XSDElement p_o_xsdParentElement)
        {
            int i_max = p_i_max;
            bool b_simpleContent = false;
            bool b_simpleType = false;
            bool b_handledFirstElementInterleaving = false;

            /* iterate all elements */
            for (int i_min = p_i_min; i_min <= i_max; i_min++)
            {
                /* get xsd type */
                XSDType e_xsdTagType = XML.GetXSDType(p_a_xsdTags, i_min);

                /* check for xs:simpleContent and xs:simpleType */
                if (((e_xsdTagType == XSDType.ComplexType) && (XML.GetXSDType(p_a_xsdTags, i_min + 1) == XSDType.SimpleContent)) ||
                    ((e_xsdTagType == XSDType.Element) && (XML.GetXSDType(p_a_xsdTags, i_min + 1) == XSDType.ComplexType) && (XML.GetXSDType(p_a_xsdTags, i_min + 2) == XSDType.SimpleContent)))
                {
                    b_simpleContent = true;
                }

                if (((e_xsdTagType == XSDType.Element) && (XML.GetXSDType(p_a_xsdTags, i_min + 1) == XSDType.SimpleType)) || (e_xsdTagType == XSDType.SimpleType))
                {
                    b_simpleType = true;
                }

                if (!p_a_xsdTags[i_min].EndsWith("/>"))
                {
                    /* first xsd tag is type xs:element or xs:complexType */
                    if ((!b_handledFirstElementInterleaving) && ((e_xsdTagType == XSDType.Element) && (!b_simpleContent) && (!b_simpleType)) || ((e_xsdTagType == XSDType.ComplexType) && (!b_simpleContent)))
                    {
                        i_max--;
                        continue;
                    }
                    else
                    {
                        b_handledFirstElementInterleaving = true;
                    }

                    /* overwrite value for end tag pointer */
                    int i_nestedMax = -1;

                    ForestNET.Lib.Global.ILogFiner("before");
                    ForestNET.Lib.Global.ILogFiner("\t" + i_min + " ... " + i_max);
                    ForestNET.Lib.Global.ILogFiner("\t" + p_a_xsdTags[i_min] + " ... " + p_a_xsdTags[i_max]);

                    /* save attributes in complex type until sequence end tag found */
                    if (e_xsdTagType == XSDType.Sequence)
                    {
                        /* read minOccurs attribute out of xs:sequence tag */
                        System.Text.RegularExpressions.Regex o_regex = new("minOccurs=\"([^\"]*)\"");
                        System.Text.RegularExpressions.Match o_matcher = o_regex.Match(p_a_xsdTags[i_min]);

                        if (o_matcher.Success)
                        {
                            p_o_xsdParentElement.SequenceMinOccurs = int.Parse(o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11));
                        }

                        /* read maxOccurs attribute out of xs:sequence tag */
                        o_regex = new System.Text.RegularExpressions.Regex("maxOccurs=\"([^\"]*)\"");
                        o_matcher = o_regex.Match(p_a_xsdTags[i_min]);

                        if (o_matcher.Success)
                        {
                            if (!o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11).Equals("unbounded"))
                            {
                                p_o_xsdParentElement.SequenceMaxOccurs = int.Parse(o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11));
                            }
                            else
                            {
                                p_o_xsdParentElement.SequenceMaxOccurs = -1;
                            }
                        }

                        while (!p_a_xsdTags[i_max].ToLower().Equals("</xs:sequence>"))
                        {
                            if (p_a_xsdTags[i_max].StartsWith("<xs:attribute", StringComparison.CurrentCultureIgnoreCase))
                            {
                                o_regex = new System.Text.RegularExpressions.Regex("ref=\"([^\"]*)\"");
                                System.Text.RegularExpressions.Match o_matcherRef = o_regex.Match(p_a_xsdTags[i_max]);

                                o_regex = new System.Text.RegularExpressions.Regex("name=\"([^\"]*)\"");
                                System.Text.RegularExpressions.Match o_matcherName = o_regex.Match(p_a_xsdTags[i_max]);

                                o_regex = new System.Text.RegularExpressions.Regex("type=\"([^\"]*)\"");
                                System.Text.RegularExpressions.Match o_matcherType = o_regex.Match(p_a_xsdTags[i_max]);

                                if (o_matcherRef.Success)
                                { /* we have an attribute element with reference */
                                    string s_referenceName = o_matcherRef.Value.Substring(5, o_matcherRef.Value.Length - 1 - 5);

                                    /* check if we have a duplicate */
                                    if (this.GetXSDAttribute(s_referenceName, p_o_xsdParentElement) != null)
                                    {
                                        throw new ArgumentNullException("Invalid xsd-tag xs:attribute (duplicate) at(" + (i_max + 1) + ".-element) \"" + p_a_xsdTags[i_max] + "\".");
                                    }

                                    if (this.XsdAttributeDefinitionExist(s_referenceName))
                                    {
                                        ForestNET.Lib.Global.ILogFinest("add and check attribute reference = " + s_referenceName);

                                        XSDAttribute o_xsdAttribute = (this.GetXSDAttributeDefinition(s_referenceName) ?? throw new NullReferenceException("xsd attribute definition not found with reference '" + s_referenceName + "'")).Clone();

                                        /* read name attribute out of xs:attribute tag */
                                        if ((o_matcherName.Success))
                                        {
                                            string s_name = o_matcherName.Value.Substring(6, o_matcherName.Value.Length - 1 - 6);

                                            /* overwrite name value from reference, because it is dominant from the usage point */
                                            o_xsdAttribute.Name = s_name;
                                        }

                                        /* add xs:attribute object to parent element */
                                        p_o_xsdParentElement.Attributes.Add(o_xsdAttribute);
                                    }
                                    else
                                    {
                                        /* attribute definition does not exist */
                                        return false;
                                    }
                                }
                                else if ((o_matcherName.Success) && (o_matcherType.Success))
                                { /* we have an attribute element with name and type */
                                    /* read name and type attribute values of xs:attribute tag */
                                    string s_name = o_matcherName.Value.Substring(6, o_matcherName.Value.Length - 1 - 6);
                                    string s_type = o_matcherType.Value.Substring(6, o_matcherType.Value.Length - 1 - 6);

                                    /* check if we have a duplicate */
                                    if (this.GetXSDAttribute(s_type, p_o_xsdParentElement) != null)
                                    {
                                        throw new ArgumentNullException("Invalid xsd-tag xs:attribute (duplicate) at(" + (i_max + 1) + ".-element) \"" + p_a_xsdTags[i_max] + "\".");
                                    }

                                    if (this.XsdAttributeDefinitionExist(s_type))
                                    {
                                        ForestNET.Lib.Global.ILogFinest("add and check attribute type reference = " + s_type);

                                        XSDAttribute o_xsdAttribute = (this.GetXSDAttributeDefinition(s_type) ?? throw new NullReferenceException("xsd attribute definition not found with type '" + s_type + "'")).Clone();

                                        /* overwrite name value from reference, because it is dominant from the usage point */
                                        o_xsdAttribute.Name = s_name;

                                        /* add xs:attribute object to parent element */
                                        p_o_xsdParentElement.Attributes.Add(o_xsdAttribute);
                                    }
                                    else if (this.XsdElementDefinitionExist(s_type))
                                    {
                                        ForestNET.Lib.Global.ILogFinest("add and check element type reference = " + s_type);

                                        XSDElement o_xsdElementReference = (this.GetXSDElementDefinition(s_type) ?? throw new NullReferenceException("xsd element definition not found with type '" + s_type + "'")).Clone();

                                        XSDAttribute o_xsdAttribute = new(s_name);

                                        /* read required attribute out of xs:attribute tag */
                                        o_regex = new System.Text.RegularExpressions.Regex("use=\"required\"");
                                        o_matcher = o_regex.Match(p_a_xsdTags[i_max]);

                                        if (o_matcher.Success)
                                        {
                                            o_xsdAttribute.Required = true;
                                        }

                                        /* read default attribute out of xs:attribute tag */
                                        o_regex = new System.Text.RegularExpressions.Regex("default=\"([^\"]*)\"");
                                        o_matcher = o_regex.Match(p_a_xsdTags[i_max]);

                                        if (o_matcher.Success)
                                        {
                                            o_xsdAttribute.Default = o_matcher.Value.Substring(9, o_matcher.Value.Length - 1 - 9);
                                        }

                                        /* read fixed attribute out of xs:attribute tag */
                                        o_regex = new System.Text.RegularExpressions.Regex("fixed=\"([^\"]*)\"");
                                        o_matcher = o_regex.Match(p_a_xsdTags[i_max]);

                                        if (o_matcher.Success)
                                        {
                                            o_xsdAttribute.Fixed = o_matcher.Value.Substring(7, o_matcher.Value.Length - 1 - 7);
                                        }

                                        /* overwrite type value from reference, because it is dominant from the usage point */
                                        o_xsdAttribute.Type = o_xsdElementReference.Type;

                                        /* assume restrictions from reference */
                                        foreach (XSDRestriction o_restriction in o_xsdElementReference.Restrictions)
                                        {
                                            o_xsdAttribute.Restrictions.Add(o_restriction);
                                        }

                                        /* add xs:attribute object to parent xsd element */
                                        p_o_xsdParentElement.Attributes.Add(o_xsdAttribute);
                                    }
                                    else
                                    {
                                        /* we do not have reference in definition pool */
                                        return false;
                                    }
                                }
                                else
                                {
                                    if (XML.GetXSDType(p_a_xsdTags, (i_max + 1)) == XSDType.SimpleType)
                                    {
                                        /* parse xs:attribute with simple type */
                                        XSDAttribute o_xsdAttribute = this.ParseXSDAttributeWithSimpleType(p_a_xsdTags, i_max);

                                        /* skip flag for adding object to list */
                                        bool b_skip = false;

                                        /* check if xs:attribute definition already exists */
                                        if (this.XsdAttributeDefinitionExist(o_xsdAttribute.Name))
                                        {
                                            if (!this.XsdAttributeDefinitionExistAsClone(o_xsdAttribute))
                                            {
                                                throw new ArgumentException("Invalid xsd-attribute-definition (duplicate) in xsd-schema at(" + (i_max + 1) + ".-element) \"" + p_a_xsdTags[i_max] + "\".");
                                            }
                                            else
                                            {
                                                b_skip = true;
                                            }
                                        }

                                        if (!b_skip)
                                        {
                                            /* add attribute definition to list */
                                            this.a_attributeDefinitions.Add(o_xsdAttribute);
                                        }

                                        XSDAttribute o_xsdAttributeReplace = new()
                                        {
                                            Reference = o_xsdAttribute.Name
                                        };

                                        /* add xs:attribute object replacement to parent element */
                                        p_o_xsdParentElement.Attributes.Add(o_xsdAttributeReplace);
                                    }
                                    else
                                    {
                                        throw new ArgumentNullException("Invalid xsd-tag xs:attribute without a reference or name and type at(" + (i_max + 1) + ".-element) \"" + p_a_xsdTags[i_max] + "\".");
                                    }
                                }
                            }

                            i_max--;
                        }
                    }
                    else if (e_xsdTagType == XSDType.Choice)
                    { /* identify choice tag for parent element */
                        ForestNET.Lib.Global.ILogFiner("\tChoice: " + i_min + " to Parent Element: " + p_o_xsdParentElement.Name);
                        ForestNET.Lib.Global.ILogFiner("\tChoice: " + p_a_xsdTags[i_min] + " to Parent Element: " + p_o_xsdParentElement.Name);

                        /* parse xs:choice */
                        XSDElement o_xsdChoice = XML.ParseXSDChoice(p_a_xsdTags, i_min);

                        /* set choice flag for parent element */
                        p_o_xsdParentElement.Choice = true;

                        /* set minOccurs for parent element */
                        if (o_xsdChoice.MinOccurs != 1)
                        {
                            p_o_xsdParentElement.MinOccurs = o_xsdChoice.MinOccurs;
                        }

                        /* set maxOccurs for parent element */
                        if (o_xsdChoice.MaxOccurs != 1)
                        {
                            p_o_xsdParentElement.MaxOccurs = o_xsdChoice.MaxOccurs;
                        }
                    }
                    else if (b_simpleType)
                    { /* handle xs:simpleType */
                        /* parse xs:simpleType */
                        XSDElement o_xsdElement = this.ParseXSDElementWithSimpleType(p_a_xsdTags, i_min);

                        /* skip flag for adding object to list */
                        bool b_skip = false;

                        /* check if xs:element definition already exists */
                        if (this.XsdElementDefinitionExist(o_xsdElement.Name))
                        {
                            if (!this.XsdElementDefinitionExistAsClone(o_xsdElement))
                            {
                                throw new ArgumentException("Invalid xsd-element-definition (duplicate) in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                            }
                            else
                            {
                                b_skip = true;
                            }
                        }

                        if (!b_skip)
                        {
                            /* add element definition to list */
                            this.a_elementDefinitons.Add(o_xsdElement);
                        }

                        XSDElement o_xsdElementReplace = new()
                        {
                            Reference = o_xsdElement.Name
                        };

                        /* add xs:attribute object replacement to parent element */
                        p_o_xsdParentElement.Children.Add(o_xsdElementReplace);

                        bool b_simpleTypeFirst = true;

                        /* find start of simpleType interlacing */
                        if (!p_a_xsdTags[i_min].StartsWith("<xs:simpletype", StringComparison.CurrentCultureIgnoreCase))
                        {
                            b_simpleTypeFirst = false;
                        }

                        /* end tag pointer for nested xs:simpleType */
                        int i_tempMax = i_min;

                        /* find end of simpleType interlacing */
                        while (!p_a_xsdTags[i_tempMax].StartsWith("</xs:simpletype", StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (i_tempMax == p_i_max)
                            {
                                /* forbidden state - interlacing is not valid in xsd-schema */
                                throw new ArgumentException("Invalid nested xsd-tag xs:simpleType at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                            }

                            i_tempMax++;
                        }

                        /* set overwrite value for end tag pointer */
                        i_nestedMax = i_tempMax;

                        if (!b_simpleTypeFirst)
                        {
                            i_nestedMax++;
                        }

                        /* set xml tag pointer to skip processed simpleType tag */
                        i_min = i_nestedMax;

                        b_simpleType = false;

                        ForestNET.Lib.Global.ILogFiner("after");
                        ForestNET.Lib.Global.ILogFiner("\t" + i_min + " ... " + i_max);
                        ForestNET.Lib.Global.ILogFiner("\t" + p_a_xsdTags[i_min] + " ... " + p_a_xsdTags[i_max]);
                    }
                    else if (b_simpleContent)
                    { /* handle xs:simpleContent */
                        /* parse xs:simpleContent */
                        XSDElement? o_xsdElement = this.ParseXSDComplexTypeWithSimpleContent(p_a_xsdTags, i_min);

                        bool b_simpleContentFirst = true;

                        /* find start of simpleContent interlacing */
                        if (!p_a_xsdTags[i_min].StartsWith("<xs:simplecontent", StringComparison.CurrentCultureIgnoreCase))
                        {
                            b_simpleContentFirst = false;
                        }

                        /* end tag pointer for nested xs:simpleContent */
                        int i_tempMax = i_min;

                        /* find end of simpleContent interlacing */
                        while (!p_a_xsdTags[i_tempMax].StartsWith("</xs:simplecontent", StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (i_tempMax == p_i_max)
                            {
                                /* forbidden state - interlacing is not valid in xsd-schema */
                                throw new ArgumentException("Invalid nested xsd-tag xs:simpleContent at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                            }

                            i_tempMax++;
                        }

                        /* set overwrite value for end tag pointer */
                        i_nestedMax = i_tempMax;

                        if (!b_simpleContentFirst)
                        {
                            i_nestedMax++;
                        }

                        if (o_xsdElement != null)
                        {
                            /* overwrite parent xsd element with xsd element simple content if simple content name and parent xsd element name are equal */
                            if (p_o_xsdParentElement.Name.Equals(o_xsdElement.Name))
                            {
                                p_o_xsdParentElement.Name = o_xsdElement.Name;
                                p_o_xsdParentElement.Type = o_xsdElement.Type;
                                p_o_xsdParentElement.Mapping = o_xsdElement.Mapping;
                                p_o_xsdParentElement.Reference = o_xsdElement.Reference;
                                p_o_xsdParentElement.MinOccurs = o_xsdElement.MinOccurs;
                                p_o_xsdParentElement.MaxOccurs = o_xsdElement.MaxOccurs;
                                p_o_xsdParentElement.Choice = o_xsdElement.Choice;
                                p_o_xsdParentElement.Restriction = o_xsdElement.Restriction;
                                p_o_xsdParentElement.IsArray = o_xsdElement.IsArray;
                                p_o_xsdParentElement.SequenceMinOccurs = o_xsdElement.SequenceMinOccurs;
                                p_o_xsdParentElement.SequenceMaxOccurs = o_xsdElement.SequenceMaxOccurs;
                                p_o_xsdParentElement.SimpleType = o_xsdElement.SimpleType;
                                p_o_xsdParentElement.SimpleContent = o_xsdElement.SimpleContent;

                                foreach (XSDElement o_element in o_xsdElement.Children)
                                {
                                    p_o_xsdParentElement.Children.Add(o_element.Clone());
                                }

                                foreach (XSDAttribute o_attribute in o_xsdElement.Attributes)
                                {
                                    p_o_xsdParentElement.Attributes.Add(o_attribute.Clone());
                                }

                                foreach (XSDRestriction o_restriction in o_xsdElement.Restrictions)
                                {
                                    p_o_xsdParentElement.Restrictions.Add(o_restriction.Clone());
                                }
                            }
                            else
                            { /* add xsd element with simple content to schema list and add a reference to parent xsd element as child object */
                                if ((this.SchemaElementDefinitionExist(o_xsdElement.Name)) && (!this.SchemaElementDefinitionExistAsClone(o_xsdElement)))
                                {
                                    throw new ArgumentException("Invalid xsd-tag xs:element or xs:complexType already exists within schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                                }
                                else
                                {
                                    /* last check if element does not already exists in definition list */
                                    if (!this.SchemaElementDefinitionExistAsClone(o_xsdElement))
                                    {
                                        this.a_dividedElements.Add(o_xsdElement);
                                    }

                                    /* create new xsd element as reference replacement */
                                    XSDElement o_xsdElementReplace = new()
                                    {
                                        Reference = o_xsdElement.Name
                                    };

                                    /* add xs:element object replacement to parent element */
                                    p_o_xsdParentElement.Children.Add(o_xsdElementReplace);
                                }
                            }
                        }
                        else
                        {
                            /* could not parse simple content, maybe because of unresolved references */
                            return false;
                        }

                        /* set xml tag pointer to skip processed simpleContent tag */
                        i_min = i_nestedMax;

                        b_simpleContent = false;

                        ForestNET.Lib.Global.ILogFiner("after");
                        ForestNET.Lib.Global.ILogFiner("\t" + i_min + " ... " + i_max);
                        ForestNET.Lib.Global.ILogFiner("\t" + p_a_xsdTags[i_min] + " ... " + p_a_xsdTags[i_max]);
                    }
                    else if ((e_xsdTagType == XSDType.Element) || (e_xsdTagType == XSDType.ComplexType))
                    { /* handle interlacing xs:element or xs:complexType tag (no one-liner), because we support this level and replace it with a reference */
                        /* parse xs:element */
                        XSDElement o_xsdElement = this.ParseXSDElement(p_a_xsdTags, i_min, true);

                        int i_tempMin = i_min + 1;
                        int i_level = 0;

                        /* look for end of nested xs:element or xs:complexType tag */
                        while (
                            ((e_xsdTagType == XSDType.Element) && (!p_a_xsdTags[i_tempMin].ToLower().Equals("</xs:element>"))) ||
                            ((e_xsdTagType == XSDType.ComplexType) && (!p_a_xsdTags[i_tempMin].ToLower().Equals("</xs:complextype>"))) ||
                            (i_level != 0)
                        )
                        {
                            if (e_xsdTagType == XSDType.Element)
                            {
                                /* handle other interlacing in current nested xs:element tag */
                                if ((p_a_xsdTags[i_tempMin].StartsWith("<xs:element", StringComparison.CurrentCultureIgnoreCase)) && (!p_a_xsdTags[i_tempMin].EndsWith("/>")))
                                {
                                    i_level++;
                                }
                                else if (p_a_xsdTags[i_tempMin].ToLower().Equals("</xs:element>"))
                                {
                                    i_level--;
                                }
                            }
                            else if (e_xsdTagType == XSDType.ComplexType)
                            {
                                /* handle other interlacing in current nested xs:complexType tag */
                                if ((p_a_xsdTags[i_tempMin].StartsWith("<xs:complextype", StringComparison.CurrentCultureIgnoreCase)) && (!p_a_xsdTags[i_tempMin].EndsWith("/>")))
                                {
                                    i_level++;
                                }
                                else if (p_a_xsdTags[i_tempMin].ToLower().Equals("</xs:complextype>"))
                                {
                                    i_level--;
                                }
                            }

                            if (i_tempMin == i_max)
                            {
                                /* forbidden state - interlacing is not valid in xsd-schema */
                                throw new ArgumentException("Invalid nested xsd-tag xs:element at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                            }

                            i_tempMin++;
                        }

                        ForestNET.Lib.Global.ILogFiner("interlacing");
                        ForestNET.Lib.Global.ILogFiner(i_min + " ... " + i_tempMin);
                        ForestNET.Lib.Global.ILogFiner(p_a_xsdTags[i_min] + " ... " + p_a_xsdTags[i_tempMin]);

                        /* parse found complex xs:element or xs:complexType tag in detail */
                        if (ParseXSDSchemaDividedElement(p_a_xsdTags, i_min, i_tempMin, o_xsdElement))
                        {
                            if ((this.SchemaElementDefinitionExist(o_xsdElement.Name)) && (!this.SchemaElementDefinitionExistAsClone(o_xsdElement)))
                            {
                                throw new ArgumentException("Invalid xsd-tag xs:element or xs:complexType already exists within schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                            }
                            else
                            {
                                /* last check if element does not already exists in definition list */
                                if (!this.SchemaElementDefinitionExistAsClone(o_xsdElement))
                                {
                                    this.a_dividedElements.Add(o_xsdElement);
                                }

                                /* create new xsd element as reference replacement */
                                XSDElement o_xsdElementReplace = new()
                                {
                                    Reference = o_xsdElement.Name
                                };

                                /* add xs:element object replacement to parent element */
                                p_o_xsdParentElement.Children.Add(o_xsdElementReplace);
                            }
                        }
                        else
                        {
                            /* found element could not be parsed, maybe because of unresolved references */
                            return false;
                        }

                        i_min = i_tempMin;

                        /* set overwrite value for end tag pointer */
                        i_nestedMax = i_tempMin;
                    }

                    /* set end tag pointer */
                    int i_endTagPointer = i_max;

                    /* overwrite end tag pointer if we had a nested simpleType or simpleContent */
                    if (i_nestedMax > 0)
                    {
                        i_endTagPointer = i_nestedMax;
                    }

                    ForestNET.Lib.Global.ILogFiner("endTagPointer");
                    ForestNET.Lib.Global.ILogFiner("\t\t" + i_min + " ... " + i_endTagPointer);
                    ForestNET.Lib.Global.ILogFiner("\t\t" + p_a_xsdTags[i_min] + " ... " + p_a_xsdTags[i_endTagPointer]);

                    ForestNET.Lib.Global.ILogFiner("after");
                    ForestNET.Lib.Global.ILogFiner("\t" + i_min + " ... " + i_max);
                    ForestNET.Lib.Global.ILogFiner("\t" + p_a_xsdTags[i_min] + " ... " + p_a_xsdTags[i_max]);

                    /* if we still have no closing tag, then our xsd schema is invalid */
                    if (!p_a_xsdTags[i_endTagPointer].StartsWith("</"))
                    {
                        throw new ArgumentException("Invalid xsd-tag(" + p_a_xsdTags[i_endTagPointer] + ") is not closed in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                    }

                    /* get xsd end type */
                    XSDType e_xsdEndTagType = XML.GetXSDType(p_a_xsdTags, i_endTagPointer);

                    /* xsd type and xsd close type must match */
                    if (e_xsdTagType != e_xsdEndTagType)
                    {
                        throw new ArgumentException("Invalid xsd-tag-type \"" + p_a_xsdTags[i_endTagPointer] + "\" for closing in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                    }
                    else
                    {
                        /* if we had a no nested simpleType or simpleContent, decrease xml end tag pointer */
                        if (i_nestedMax < 0)
                        {
                            i_max--;
                        }
                    }
                }
                else
                {
                    ForestNET.Lib.Global.ILogFiner("\tElement: " + i_min + " ... " + i_max);
                    ForestNET.Lib.Global.ILogFiner("\tElement: " + p_a_xsdTags[i_min] + " ... " + p_a_xsdTags[i_max]);

                    if ((e_xsdTagType == XSDType.Element) || (e_xsdTagType == XSDType.ComplexType))
                    {
                        System.Text.RegularExpressions.Regex o_regex = new("ref=\"([^\"]*)\"");
                        System.Text.RegularExpressions.Match o_matcher = o_regex.Match(p_a_xsdTags[i_min]);

                        o_regex = new System.Text.RegularExpressions.Regex("name=\"([^\"]*)\"");
                        System.Text.RegularExpressions.Match o_matcherName = o_regex.Match(p_a_xsdTags[i_min]);

                        o_regex = new System.Text.RegularExpressions.Regex("type=\"xs:([^\"]*)\"");
                        System.Text.RegularExpressions.Match o_matcherPrimitiveType = o_regex.Match(p_a_xsdTags[i_min]);

                        o_regex = new System.Text.RegularExpressions.Regex("type=\"([^\"]*)\"");
                        System.Text.RegularExpressions.Match o_matcherType = o_regex.Match(p_a_xsdTags[i_min]);

                        bool b_nameAttributeFound = o_matcherName.Success;

                        if (o_matcher.Success)
                        { /* must have a reference attribute */
                            string s_referenceName = o_matcher.Value.Substring(5, o_matcher.Value.Length - 1 - 5);

                            /* check if we have a duplicate */
                            if (this.GetXSDElement(s_referenceName, p_o_xsdParentElement) != null)
                            {
                                throw new ArgumentException("Invalid xsd-tag xs:element (duplicate) at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                            }

                            /* xsd object for reference */
                            XSDElement? o_xsdElementReference = null;

                            if (this.XsdElementDefinitionExist(s_referenceName))
                            { /* check if we have a reference within our element definition list */
                                o_xsdElementReference = this.GetXSDElementDefinition(s_referenceName)?.Clone();
                            }
                            else if (this.SchemaElementDefinitionExist(s_referenceName))
                            { /* check if we have a reference within our schema definition list */
                                o_xsdElementReference = this.GetSchemaElementDefinition(s_referenceName);
                            }

                            /* check if we found a reference */
                            if (o_xsdElementReference != null)
                            {
                                ForestNET.Lib.Global.ILogFinest("add element reference = " + s_referenceName);

                                /* read name attribute out of xs:element tag */
                                if (o_matcherName.Success)
                                {
                                    string s_name = o_matcherName.Value.Substring(6, o_matcherName.Value.Length - 1 - 6);

                                    /* overwrite name value from reference, because it is dominant from the usage point */
                                    o_xsdElementReference.Name = s_name;
                                }

                                /* read minOccurs attribute out of xs:element tag */
                                o_regex = new System.Text.RegularExpressions.Regex("minOccurs=\"([^\"]*)\"");
                                o_matcher = o_regex.Match(p_a_xsdTags[i_min]);

                                if (o_matcher.Success)
                                {
                                    o_xsdElementReference.MinOccurs = int.Parse(o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11));
                                }

                                /* read maxOccurs attribute out of xs:element tag */
                                o_regex = new System.Text.RegularExpressions.Regex("maxOccurs=\"([^\"]*)\"");
                                o_matcher = o_regex.Match(p_a_xsdTags[i_min]);

                                if (o_matcher.Success)
                                {
                                    if (!o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11).Equals("unbounded"))
                                    {
                                        o_xsdElementReference.MaxOccurs = int.Parse(o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11));
                                    }
                                    else if (o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11).Equals("unbounded"))
                                    {
                                        o_xsdElementReference.MaxOccurs = -1;
                                    }
                                }

                                /* library does not support multiple occurrences of the same xml element without a list definition */
                                if ((!o_xsdElementReference.Choice) && (!p_o_xsdParentElement.Mapping.Contains("ArrayList(")) && (o_xsdElementReference.MaxOccurs > 1))
                                {
                                    throw new ArgumentException("Library does not support multiple occurrences of the same xml element without a list definition in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                                }

                                /* add xs:element to parent element */
                                p_o_xsdParentElement.Children.Add(o_xsdElementReference);
                            }
                            else
                            {
                                /* we do not have reference in definition pool */
                                return false;
                            }
                        }
                        else if ((b_nameAttributeFound) && (o_matcherPrimitiveType.Success))
                        { /* must have a name and primitive type attribute which are equal */
                            string s_name = o_matcherName.Value.Substring(6, o_matcherName.Value.Length - 1 - 6);
                            string s_type = o_matcherPrimitiveType.Value.Substring(9, o_matcherPrimitiveType.Value.Length - 1 - 9);

                            if (!s_name.Equals(s_type))
                            {
                                XSDElement o_xsdElement = new(s_name, s_type);

                                /* read mapping attribute out of xs:element tag */
                                o_regex = new System.Text.RegularExpressions.Regex("mapping=\"([^\"]*)\"");
                                System.Text.RegularExpressions.Match o_matcherMapping = o_regex.Match(p_a_xsdTags[i_min]);

                                if (o_matcherMapping.Success)
                                {
                                    o_xsdElement.Mapping = o_matcherMapping.Value.Substring(9, o_matcherMapping.Value.Length - 1 - 9);
                                }

                                /* skip flag for adding object to list */
                                bool b_skip = false;

                                /* check if xs:element definition already exists */
                                if (this.XsdElementDefinitionExist(o_xsdElement.Name))
                                {
                                    if (!this.XsdElementDefinitionExistAsClone(o_xsdElement))
                                    {
                                        throw new ArgumentException("Invalid xsd-element-definition (duplicate) in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                                    }
                                    else
                                    {
                                        b_skip = true;
                                    }
                                }

                                if (!b_skip)
                                {
                                    /* add element definition to list */
                                    this.a_elementDefinitons.Add(o_xsdElement);
                                }

                                XSDElement o_xsdElementReplace = new()
                                {
                                    Reference = o_xsdElement.Name
                                };

                                /* add xs:attribute object replacement to parent element */
                                p_o_xsdParentElement.Children.Add(o_xsdElementReplace);
                            }
                            else
                            {
                                ForestNET.Lib.Global.ILogFiner("create and read xsd-element as array element definition = " + s_type);
                                XSDElement o_xsdElement = new(s_name, s_type)
                                {
                                    /* set xsd-element as array */
                                    IsArray = true
                                };

                                /* read minOccurs attribute out of xs:element tag */
                                o_regex = new System.Text.RegularExpressions.Regex("minOccurs=\"([^\"]*)\"");
                                o_matcher = o_regex.Match(p_a_xsdTags[i_min]);

                                if (o_matcher.Success)
                                {
                                    o_xsdElement.MinOccurs = int.Parse(o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11));
                                }

                                /* read maxOccurs attribute out of xs:element tag */
                                o_regex = new System.Text.RegularExpressions.Regex("maxOccurs=\"([^\"]*)\"");
                                o_matcher = o_regex.Match(p_a_xsdTags[i_min]);

                                if (o_matcher.Success)
                                {
                                    if (!o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11).Equals("unbounded"))
                                    {
                                        o_xsdElement.MaxOccurs = int.Parse(o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11));
                                    }
                                    else if (o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11).Equals("unbounded"))
                                    {
                                        o_xsdElement.MaxOccurs = -1;
                                    }
                                }

                                /* add xs:element to parent element */
                                p_o_xsdParentElement.Children.Add(o_xsdElement);
                            }
                        }
                        else if ((b_nameAttributeFound) && (o_matcherType.Success))
                        { /* must have a name and type attribute, where type attribute value is used as reference */
                            /* read name and type attribute value */
                            string s_name = o_matcherName.Value.Substring(6, o_matcherName.Value.Length - 1 - 6);
                            string s_typeName = o_matcherType.Value.Substring(6, o_matcherType.Value.Length - 1 - 6);

                            /* check if we have a duplicate */
                            if (this.GetXSDElement(s_typeName, p_o_xsdParentElement) != null)
                            {
                                throw new ArgumentException("Invalid xsd-tag xs:element (duplicate) at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                            }

                            /* xsd object for reference */
                            XSDElement? o_xsdElementReference = null;

                            if (this.XsdElementDefinitionExist(s_typeName))
                            { /* check if we have a reference within our element definition list */
                                o_xsdElementReference = this.GetXSDElementDefinition(s_typeName)?.Clone();
                            }
                            else if (this.SchemaElementDefinitionExist(s_typeName))
                            { /* check if we have a reference within our schema definition list */
                                o_xsdElementReference = this.GetSchemaElementDefinition(s_typeName);
                            }

                            /* check if we found a reference */
                            if (o_xsdElementReference != null)
                            {
                                ForestNET.Lib.Global.ILogFinest("add element reference = " + s_typeName);

                                /* overwrite name value from reference, because it is dominant from the usage point */
                                o_xsdElementReference.Name = s_name;

                                /* even overwrite type value, because all we wanted is to check if the reference exists */
                                o_xsdElementReference.Type = s_typeName;

                                /* read minOccurs attribute out of xs:element tag */
                                o_regex = new System.Text.RegularExpressions.Regex("minOccurs=\"([^\"]*)\"");
                                o_matcher = o_regex.Match(p_a_xsdTags[i_min]);

                                if (o_matcher.Success)
                                {
                                    o_xsdElementReference.MinOccurs = int.Parse(o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11));
                                }

                                /* read maxOccurs attribute out of xs:element tag */
                                o_regex = new System.Text.RegularExpressions.Regex("maxOccurs=\"([^\"]*)\"");
                                o_matcher = o_regex.Match(p_a_xsdTags[i_min]);

                                if (o_matcher.Success)
                                {
                                    if (!o_matcher.Value.Substring(11, o_matcher.Value.Length - 1).Equals("unbounded"))
                                    {
                                        o_xsdElementReference.MaxOccurs = int.Parse(o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11));
                                    }
                                    else if (o_matcher.Value.Substring(11, o_matcher.Value.Length - 1 - 11).Equals("unbounded"))
                                    {
                                        o_xsdElementReference.MaxOccurs = -1;
                                    }
                                }

                                /* library does not support multiple occurrences of the same xml element without a list definition */
                                if ((!o_xsdElementReference.Choice) && (!p_o_xsdParentElement.Mapping.Contains("ArrayList(")) && (o_xsdElementReference.MaxOccurs > 1))
                                {
                                    throw new ArgumentException("Library does not support multiple occurrences of the same xml element without a list definition in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                                }

                                /* add xs:element to parent element */
                                p_o_xsdParentElement.Children.Add(o_xsdElementReference);
                            }
                            else
                            {
                                /* we do not have reference in definition pool */
                                return false;
                            }
                        }
                        else
                        {
                            throw new ArgumentException("Invalid xsd-tag xs:element without a reference at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Invalid xsd-tag in xsd-schema at(" + (i_min + 1) + ".-element) \"" + p_a_xsdTags[i_min] + "\".");
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Check if a schema element definition exists by reference name
        /// </summary>
        /// <param name="p_s_referenceName">reference name of schema element definition</param>
        /// <returns>true - definition exists, false - definition does not exist</returns>
        private bool SchemaElementDefinitionExist(string p_s_referenceName)
        {
            bool b_found = false;

            foreach (XSDElement o_xsdElement in this.a_dividedElements)
            {
                if (o_xsdElement.Name.Equals(p_s_referenceName))
                {
                    b_found = true;
                }
            }

            return b_found;
        }

        /// <summary>
        /// Check if a schema definition exists by element object as clone
        /// </summary>
        /// <param name="p_o_xsdElement">xsd element object</param>
        /// <returns>true - schema exists as clone, false - schema does not exist as clone, but with other values</returns>
        private bool SchemaElementDefinitionExistAsClone(XSDElement p_o_xsdElement)
        {
            bool b_found = false;

            foreach (XSDElement o_xsdElement in this.a_dividedElements)
            {
                if ((o_xsdElement.Name.Equals(p_o_xsdElement.Name)) && (o_xsdElement.IsEqual(p_o_xsdElement)))
                {
                    b_found = true;
                }
            }

            return b_found;
        }

        /// <summary>
        /// Get schema element definition exists by reference name
        /// </summary>
        /// <param name="p_s_referenceName">reference name of schema element definition</param>
        /// <returns>xsd element object as schema definition</returns>
        private XSDElement? GetSchemaElementDefinition(string p_s_referenceName)
        {
            XSDElement? o_xsdElement = null;

            foreach (XSDElement o_xsdElementObject in this.a_dividedElements)
            {
                if (o_xsdElementObject.Name.Equals(p_s_referenceName))
                {
                    o_xsdElement = o_xsdElementObject.Clone();
                }
            }

            return o_xsdElement;
        }

        /* encoding data to XML with XSD schema */

        /// <summary>
        /// Encode c# object to a xml content string
        /// </summary>
        /// <param name="p_o_object">source c# object to encode xml information</param>
        /// <returns>encoded xml information from c# object as string</returns>
        /// <exception cref="System.IO.IOException">cannot create or access destination xml file</exception>
        /// <exception cref="ArgumentNullException">value in schema or expected element is not available</exception>
        /// <exception cref="ArgumentException">condition failed for decoding xml correctly</exception>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        public string XmlEncode(Object? p_o_object)
        {
            if (this.Root == null)
            {
                throw new NullReferenceException("Cannot encode data. Schema is null.");
            }

            if (p_o_object == null)
            {
                throw new ArgumentNullException(nameof(p_o_object), "Cannot encode data. Parameter object is null.");
            }

            /* set level for PrintIdentation to zero */
            this.i_level = 0;

            /* add xml header to file */
            string s_xml = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" + this.s_lineBreak;

            XSDElement o_xsdElement = this.Root;

            /* maybe input object type is not the expected root, but just a sub tree of a valid xsd schema, so we look for it in the element definitions */
            if (!(p_o_object.GetType().FullName ?? "").Equals(o_xsdElement.Mapping))
            {
                string s_typeName = p_o_object.GetType().FullName + ", " + p_o_object.GetType().Assembly.GetName().Name;
                bool b_list = false;

                /* check if object which will be encoded is actually a List */
                if ((p_o_object.GetType().IsGenericType) && (p_o_object.GetType().GetGenericTypeDefinition() == typeof(List<>)))
                {
                    List<Object> o_temp = [.. ((System.Collections.IEnumerable)p_o_object).Cast<Object>()];

                    /* thus we must find the real class of an list object */
                    foreach (Object o_foo in o_temp)
                    {
                        if (o_foo != null)
                        {
                            s_typeName = o_foo.GetType().FullName + ", " + o_foo.GetType().Assembly.GetName().Name;
                            b_list = true;
                            break;
                        }
                    }
                }

                foreach (XSDElement o_temp in this.a_elementDefinitons)
                {
                    /* find xml element as element definition of xsd schema */
                    if (
                        (!b_list) && (s_typeName.Equals(o_temp.Mapping))
                    ||
                        (b_list) && ((o_temp.Mapping.Contains('(')) && (o_temp.Mapping.Contains(')')) && (s_typeName.Equals(o_temp.Mapping.Substring(o_temp.Mapping.IndexOf("(") + 1, o_temp.Mapping.IndexOf(")") - (o_temp.Mapping.IndexOf("(") + 1)))))
                    )
                    {
                        ForestNET.Lib.Global.ILogFiner("object type " + s_typeName + ", is not the correct root element of schema, but a valid sub tree, so take this sub tree as entry point for encoding");

                        /* update schema element to encode xml tree recursively with that sub tree of xsd schema */
                        o_xsdElement = o_temp;
                        break;
                    }
                }
            }

            /* encode data to xml recursive */
            s_xml += this.XmlEncodeRecursive(o_xsdElement, p_o_object);

            if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("Encoded XML:" + this.s_lineBreak + s_xml);

            /* return xml content string */
            return s_xml;
        }

        /// <summary>
        /// Encode c# object to a xml file, keep existing xml file
        /// </summary>
        /// <param name="p_o_object">source c# object to encode xml information</param>
        /// <param name="p_s_xmlFile">destination xml file to save encoded xml information</param>
        /// <returns>file object with encoded xml content</returns>
        /// <exception cref="System.IO.IOException">cannot create or access destination xml file</exception>
        /// <exception cref="ArgumentNullException">value in schema or expected element is not available</exception>
        /// <exception cref="ArgumentException">condition failed for decoding xml correctly</exception>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        /// <exception cref="FormatException">exception parsing string to target type</exception>
        public void XmlEncode(Object? p_o_object, string p_s_xmlFile)
        {
            this.XmlEncode(p_o_object, p_s_xmlFile, false);
        }

        /// <summary>
        /// Encode c# object to a xml file
        /// </summary>
        /// <param name="p_o_object">source c# object to encode xml information</param>
        /// <param name="p_s_xmlFile">destination xml file to save encoded xml information</param>
        /// <param name="p_b_overwrite">true - overwrite existing xml file, false - keep existing xml file</param>
        /// <returns>file object with encoded xml content</returns>
        /// <exception cref="System.IO.IOException">cannot create or access destination xml file</exception>
        /// <exception cref="ArgumentNullException">value in schema or expected element is not available</exception>
        /// <exception cref="ArgumentException">condition failed for decoding xml correctly</exception>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        /// <exception cref="FormatException">exception parsing string to target type</exception>
        public File XmlEncode(Object? p_o_object, string p_s_xmlFile, bool p_b_overwrite)
        {
            /* encode c# object to xml string */
            string s_xml = this.XmlEncode(p_o_object);

            /* if file does not exist we must create an new file */
            if (!File.Exists(p_s_xmlFile))
            {
                if (p_b_overwrite)
                {
                    p_b_overwrite = false;
                }
            }

            /* open (new) file */
            File o_file = new(p_s_xmlFile, !p_b_overwrite);
            /* save xml encoded data into file */
            o_file.ReplaceContent(s_xml);

            ForestNET.Lib.Global.ILogFiner("written encoded xml to file");

            /* return file object */
            return o_file;
        }

        /// <summary>
        /// Recursive method to encode c# object and it's fields to a xml string
        /// </summary>
        /// <param name="p_o_xsdElement">current xml schema element with additional information for encoding</param>
        /// <param name="p_o_object">source c# object to encode xml information</param>
        /// <returns>encoded xml information from c# object as string</returns>
        /// <exception cref="ArgumentNullException">value in schema or expected element is not available</exception>
        /// <exception cref="ArgumentException">condition failed for decoding xml correctly</exception>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        /// <exception cref="FormatException">exception parsing string to target type</exception>
        private string XmlEncodeRecursive(XSDElement p_o_xsdElement, Object? p_o_object)
        {
            string s_xml = "";

            /* start xml tag */
            s_xml += this.PrintIndentation() + "<" + p_o_xsdElement.Name;

            /* check if current xml element should have attributes */
            if (p_o_xsdElement.Attributes.Count > 0)
            {
                /* iterate all xml attributes */
                foreach (XSDAttribute o_xsdAttribute in p_o_xsdElement.Attributes)
                {
                    /* get value for xml attribute */
                    string s_attributeValue = this.GetAttributeValue(o_xsdAttribute, p_o_object);

                    /* if attribute is required but value is empty, throw exception */
                    if ((o_xsdAttribute.Required) && (ForestNET.Lib.Helper.IsStringEmpty(s_attributeValue)))
                    {
                        throw new NullReferenceException("Missing attribute value for xs:attribute[" + p_o_object?.GetType().Name + "." + o_xsdAttribute.Name + "]");
                    }

                    /* if attribute value is not empty, add it to xml element */
                    if (!ForestNET.Lib.Helper.IsStringEmpty(s_attributeValue))
                    {
                        s_xml += " " + o_xsdAttribute.Name + "=\"" + s_attributeValue + "\"";
                    }
                }
            }

            /* end tag */
            s_xml += ">";
            s_xml += this.s_lineBreak;

            /* if we have xs:element definition with no type, exact one child element and mapping contains ":" and mapping does NOT end with "[]"
			 * we have to iterate a list of object and must print multiple xml elements
			 * otherwise we have usual xs:element definitions for current element
			 */
            if ((ForestNET.Lib.Helper.IsStringEmpty(p_o_xsdElement.Type)) && (p_o_xsdElement.Mapping.Contains(':')) && (p_o_xsdElement.Children.Count == 1) && (!p_o_xsdElement.Mapping.EndsWith("[]")))
            {
                /* cast current object as list with unknown generic type */
                List<Object?> a_objects = [.. ((System.Collections.IEnumerable)(p_o_object ?? new List<Object?>())).Cast<Object?>()];

                /* check minOccurs attribute and current list size */
                if ((p_o_xsdElement.Children[0].MinOccurs > 0) && (a_objects.Count == 0))
                {
                    throw new ArgumentException("Not enough [" + p_o_xsdElement.Children[0].Name + "] objects. The minimum number is " + p_o_xsdElement.Children[0].MinOccurs);
                }

                /* check maxOccurs attribute and current list size */
                if ((p_o_xsdElement.Children[0].MaxOccurs >= 0) && (a_objects.Count > p_o_xsdElement.Children[0].MaxOccurs))
                {
                    throw new ArgumentException("Too many [" + p_o_xsdElement.Children[0].Name + "] objects. The maximum number is " + p_o_xsdElement.Children[0].MaxOccurs);
                }

                /* increase level for PrintIdentation */
                this.i_level++;

                /* iterate objects in list and encode data to xml recursively */
                for (int i = 0; i < a_objects.Count; i++)
                {
                    s_xml += this.XmlEncodeRecursive(p_o_xsdElement.Children[0], a_objects[i]);
                }

                /* decrease level for PrintIdentation */
                this.i_level--;
            }
            else
            {
                /* increase level for PrintIdentation */
                this.i_level++;

                /* create choice counter */
                int i_choiceCnt = 0;

                /* iterate children elements of current xsd element, but not if mapping is not null and ends with "[]" -> primitive array */
                if ((ForestNET.Lib.Helper.IsStringEmpty(p_o_xsdElement.Mapping)) || ((!ForestNET.Lib.Helper.IsStringEmpty(p_o_xsdElement.Mapping)) && (!p_o_xsdElement.Mapping.EndsWith("[]"))))
                {
                    /* iterate all xs:elements of current element */
                    foreach (XSDElement o_xsdElement in p_o_xsdElement.Children)
                    {
                        /* if xs:element has no type we may have a special object definition or another list of objects */
                        if (ForestNET.Lib.Helper.IsStringEmpty(o_xsdElement.Type))
                        {
                            /* variable for object data */
                            Object? o_object = null;

                            /* get field type */
                            string s_fieldType = o_xsdElement.Mapping;

                            if (s_fieldType.Equals("_skipLevel_"))
                            {
                                o_object = p_o_object;
                            }
                            else
                            {
                                /* remove enclosure of field type if it exists */
                                if (o_xsdElement.Mapping.Contains(':'))
                                {
                                    s_fieldType = s_fieldType.Substring(0, s_fieldType.IndexOf(":"));
                                }
                                else
                                {
                                    /* remove assembly part */
                                    if (s_fieldType.Contains(','))
                                    {
                                        s_fieldType = s_fieldType.Substring(0, s_fieldType.IndexOf(","));
                                    }

                                    /* remove package prefix */
                                    if (s_fieldType.Contains('.'))
                                    {
                                        s_fieldType = s_fieldType.Substring(s_fieldType.LastIndexOf(".") + 1, s_fieldType.Length - (s_fieldType.LastIndexOf(".") + 1));
                                    }

                                    /* remove internal class prefix */
                                    if (s_fieldType.Contains('+'))
                                    {
                                        s_fieldType = s_fieldType.Substring(s_fieldType.LastIndexOf("+") + 1, s_fieldType.Length - (s_fieldType.LastIndexOf("+") + 1));
                                    }
                                }

                                /* check if we use property methods with invoke to get object data values */
                                if (this.UseProperties)
                                {
                                    /* property info for accessing property */
                                    System.Reflection.PropertyInfo? o_propertyInfo;

                                    /* try to get access to property info */
                                    try
                                    {
                                        o_propertyInfo = p_o_object?.GetType().GetProperty(s_fieldType);

                                        if (o_propertyInfo == null)
                                        {
                                            throw new Exception("property info is null");
                                        }
                                    }
                                    catch (Exception o_exc)
                                    {
                                        throw new MissingMemberException("Class instance property[" + s_fieldType + "] does not exist for class instance(" + p_o_object?.GetType().FullName + "): " + o_exc.Message);
                                    }

                                    /* check if we can access property */
                                    if (!o_propertyInfo.CanRead)
                                    {
                                        throw new MemberAccessException("Cannot write property from class; instance property[" + s_fieldType + "] does not exist for class instance(" + p_o_object?.GetType().FullName + ")");
                                    }

                                    /* get object data of current property */
                                    o_object = o_propertyInfo.GetValue(p_o_object);
                                }
                                else
                                {
                                    /* field info for accessing field */
                                    System.Reflection.FieldInfo? o_fieldInfo;

                                    /* try to get access to field info */
                                    try
                                    {
                                        o_fieldInfo = p_o_object?.GetType().GetField(s_fieldType);

                                        if (o_fieldInfo == null)
                                        {
                                            throw new Exception("field info is null");
                                        }
                                    }
                                    catch (Exception o_exc)
                                    {
                                        throw new MissingMemberException("Class instance field[" + s_fieldType + "] does not exist for class instance(" + p_o_object?.GetType().FullName + "): " + o_exc.Message);
                                    }

                                    /* check if we can access field */
                                    if (!o_fieldInfo.IsPublic)
                                    {
                                        throw new MemberAccessException("Cannot read field from class; instance field[" + s_fieldType + "] does not exist for class instance(" + p_o_object?.GetType().FullName + ")");
                                    }

                                    /* get object data of current field */
                                    o_object = o_fieldInfo.GetValue(p_o_object);
                                }
                            }

                            /* check if object is not null, but ignore if parent element is not a choice tag or we have not a primitive array in mapping */
                            if ((o_object == null) && (!p_o_xsdElement.Choice) && (!o_xsdElement.Mapping.EndsWith("[]")))
                            {
                                throw new ArgumentNullException(s_fieldType + " has no value in xs:element " + o_xsdElement.Name + "(" + p_o_object?.GetType().FullName + ")");
                            }

                            /* do not continue recursion, when object is null and parent element is a choice tag */
                            if (!((o_object == null) && (p_o_xsdElement.Choice)))
                            {
                                /* check choice counter */
                                if ((p_o_xsdElement.MaxOccurs >= 0) && (++i_choiceCnt > p_o_xsdElement.MaxOccurs))
                                {
                                    throw new ArgumentException(s_fieldType + " has to many objects(" + i_choiceCnt + ") in xs:choice " + p_o_xsdElement.Name + "(" + p_o_object?.GetType().FullName + "), maximum = " + p_o_xsdElement.MaxOccurs);
                                }

                                /* encode object data to xml recursively */
                                s_xml += this.XmlEncodeRecursive(o_xsdElement, o_object);
                            }
                        }
                        else
                        { /* otherwise we have xs:elements with primitive types */
                            /* get value for xml element */
                            string s_elementValue = this.GetElementValue(o_xsdElement, p_o_object);

                            /* check minOccurs attribute of xml element and if value is empty */
                            if ((o_xsdElement.MinOccurs > 0) && (ForestNET.Lib.Helper.IsStringEmpty(s_elementValue)))
                            {
                                if (p_o_xsdElement.Choice)
                                {
                                    continue;
                                }
                                else
                                {
                                    throw new ArgumentException("Missing element value for xs:element[" + p_o_object?.GetType().FullName + "." + o_xsdElement.Name + "]");
                                }
                            }

                            /* check choice counter */
                            if (p_o_xsdElement.Choice)
                            {
                                if ((p_o_xsdElement.MaxOccurs >= 0) && (++i_choiceCnt > p_o_xsdElement.MaxOccurs) && (!ForestNET.Lib.Helper.IsStringEmpty(s_elementValue)))
                                {
                                    throw new ArgumentException(p_o_xsdElement.Name + " has to many objects(" + i_choiceCnt + ") in xs:choice " + p_o_object?.GetType().FullName + "." + o_xsdElement.Name + ", maximum = " + p_o_xsdElement.MaxOccurs);
                                }
                            }

                            /* start xml tag */
                            s_xml += this.PrintIndentation() + "<" + o_xsdElement.Name;

                            /* check if current xml element should have attributes */
                            if (o_xsdElement.Attributes.Count > 0)
                            {
                                /* iterate all xml attributes */
                                foreach (XSDAttribute o_xsdAttribute in o_xsdElement.Attributes)
                                {
                                    /* get value for xml attribute */
                                    string s_attributeValue = this.GetAttributeValue(o_xsdAttribute, p_o_object);

                                    /* if attribute is required but value is empty, throw exception */
                                    if ((o_xsdAttribute.Required) && (ForestNET.Lib.Helper.IsStringEmpty(s_attributeValue)))
                                    {
                                        throw new ArgumentException("Missing attribute value for xs:attribute[" + p_o_object?.GetType().FullName + "." + o_xsdAttribute.Name + "]");
                                    }

                                    /* if attribute value is not empty, add it to xml element */
                                    if (!ForestNET.Lib.Helper.IsStringEmpty(s_attributeValue))
                                    {
                                        s_xml += " " + o_xsdAttribute.Name + "=\"" + s_attributeValue + "\"";
                                    }
                                }
                            }

                            if (!ForestNET.Lib.Helper.IsStringEmpty(s_elementValue))
                            {
                                /* end tag */
                                s_xml += ">";
                                /* write value in xml element tag */
                                s_xml += s_elementValue;
                                /* end xml tag */
                                s_xml += "</" + o_xsdElement.Name + ">";
                            }
                            else
                            {
                                /* close tag without a value */
                                s_xml += "/>";
                            }

                            s_xml += this.s_lineBreak;
                        }
                    }
                }

                /* check choice counter for minimum objects */
                if (p_o_xsdElement.Choice)
                {
                    if (i_choiceCnt < p_o_xsdElement.MinOccurs)
                    {
                        throw new ArgumentException(p_o_xsdElement.Name + " has to few objects(" + i_choiceCnt + ") in xs:choice, minimum = " + p_o_xsdElement.MinOccurs);
                    }
                }

                /* check if we have an array element */
                if (p_o_xsdElement.IsArray)
                {
                    /* get value for xml element */
                    string s_elementValue = this.GetElementValue(p_o_xsdElement, p_o_object);

                    /* check minOccurs attribute of xml element and if value is empty */
                    if ((p_o_xsdElement.MinOccurs > 0) && (ForestNET.Lib.Helper.IsStringEmpty(s_elementValue)))
                    {
                        throw new ArgumentException("Missing element value for xs:element[" + p_o_object?.GetType().FullName + "." + p_o_xsdElement.Name + "]");
                    }

                    /* remove auto line break */
                    s_xml = s_xml.Substring(0, s_xml.Length - this.s_lineBreak.Length);

                    /* only write not null element values */
                    if (!ForestNET.Lib.Helper.IsStringEmpty(s_elementValue))
                    {
                        /* write value in xml element tag */
                        s_xml += s_elementValue;
                        /* close xml element tag */
                        s_xml += "</" + p_o_xsdElement.Name + ">";
                    }
                    else
                    {
                        /* write empty string or empty xml tag */
                        if (this.b_printEmptystring)
                        {
                            /* write empty string value in xml element tag */
                            s_xml += "&#x200B;";
                            /* close xml element tag */
                            s_xml += "</" + p_o_xsdElement.Name + ">";
                        }
                        else
                        {
                            this.i_level--;
                            s_xml = this.PrintIndentation() + "<" + p_o_xsdElement.Name + "/>";
                            this.i_level++;
                        }
                    }
                }
                else if ((p_o_object != null) && (!ForestNET.Lib.Helper.IsStringEmpty(p_o_xsdElement.Mapping)) && (p_o_xsdElement.Mapping.EndsWith("[]")))
                { /* handle primitive array elements */
                    /* get primitive array type */
                    string s_primitiveArrayType = p_o_xsdElement.Mapping;

                    /* get second part of mapping value as primitive array type */
                    if (s_primitiveArrayType.Contains(':'))
                    {
                        s_primitiveArrayType = s_primitiveArrayType.Split(":")[1];
                    }

                    /* remove '[]' from type value */
                    if (s_primitiveArrayType.EndsWith("[]"))
                    {
                        s_primitiveArrayType = s_primitiveArrayType.Substring(0, s_primitiveArrayType.Length - 2);
                    }

                    /* lower primitive array type */
                    s_primitiveArrayType = s_primitiveArrayType.ToLower();

                    /* handle primitive array in object parameter */
                    if ((s_primitiveArrayType.Equals("bool")) || (s_primitiveArrayType.Equals("boolean")) || (s_primitiveArrayType.Equals("system.boolean")))
                    {
                        /* cast current field of parameter object as array */
                        bool[] a_objects = (bool[])p_o_object;

                        /* only execute if we have more than one array element */
                        if (a_objects.Length > 0)
                        {
                            /* iterate objects in primitive array and encode element */
                            for (int i = 0; i < a_objects.Length; i++)
                            {
                                /* encode primitive array object to xml recursively */
                                s_xml += this.XmlEncodeRecursive(p_o_xsdElement.Children[0], (Object)a_objects[i]);
                            }
                        }
                    }
                    else if ((s_primitiveArrayType.Equals("unsignedbyte")) || (s_primitiveArrayType.Equals("byte")) || (s_primitiveArrayType.Equals("system.byte")))
                    {
                        /* cast current field of parameter object as array */
                        byte[] a_objects = (byte[])p_o_object;

                        /* only execute if we have more than one array element */
                        if (a_objects.Length > 0)
                        {
                            /* iterate objects in primitive array and encode element */
                            for (int i = 0; i < a_objects.Length; i++)
                            {
                                /* encode primitive array object to xml recursively */
                                s_xml += this.XmlEncodeRecursive(p_o_xsdElement.Children[0], (Object)a_objects[i]);
                            }
                        }
                    }
                    else if ((s_primitiveArrayType.Equals("sbyte")) || (s_primitiveArrayType.Equals("system.sbyte")))
                    {
                        /* cast current field of parameter object as array */
                        sbyte[] a_objects = (sbyte[])p_o_object;

                        /* only execute if we have more than one array element */
                        if (a_objects.Length > 0)
                        {
                            /* iterate objects in primitive array and encode element */
                            for (int i = 0; i < a_objects.Length; i++)
                            {
                                /* encode primitive array object to xml recursively */
                                s_xml += this.XmlEncodeRecursive(p_o_xsdElement.Children[0], (Object)a_objects[i]);
                            }
                        }
                    }
                    else if ((s_primitiveArrayType.Equals("char")) || (s_primitiveArrayType.Equals("system.char")))
                    {
                        /* cast current field of parameter object as array */
                        char[] a_objects = (char[])p_o_object;

                        /* only execute if we have more than one array element */
                        if (a_objects.Length > 0)
                        {
                            /* iterate objects in primitive array and encode element */
                            for (int i = 0; i < a_objects.Length; i++)
                            {
                                /* encode primitive array object to xml recursively */
                                s_xml += this.XmlEncodeRecursive(p_o_xsdElement.Children[0], (Object)a_objects[i]);
                            }
                        }
                    }
                    else if ((s_primitiveArrayType.Equals("float")) || (s_primitiveArrayType.Equals("system.single")))
                    {
                        /* cast current field of parameter object as array */
                        float[] a_objects = (float[])p_o_object;

                        /* only execute if we have more than one array element */
                        if (a_objects.Length > 0)
                        {
                            /* iterate objects in primitive array and encode element */
                            for (int i = 0; i < a_objects.Length; i++)
                            {
                                /* encode primitive array object to xml recursively */
                                s_xml += this.XmlEncodeRecursive(p_o_xsdElement.Children[0], (Object)a_objects[i]);
                            }
                        }
                    }
                    else if ((s_primitiveArrayType.Equals("double")) || (s_primitiveArrayType.Equals("system.double")))
                    {
                        /* cast current field of parameter object as array */
                        double[] a_objects = (double[])p_o_object;

                        /* only execute if we have more than one array element */
                        if (a_objects.Length > 0)
                        {
                            /* iterate objects in primitive array and encode element */
                            for (int i = 0; i < a_objects.Length; i++)
                            {
                                /* encode primitive array object to xml recursively */
                                s_xml += this.XmlEncodeRecursive(p_o_xsdElement.Children[0], (Object)a_objects[i]);
                            }
                        }
                    }
                    else if ((s_primitiveArrayType.Equals("short")) || (s_primitiveArrayType.Equals("system.int16")))
                    {
                        /* cast current field of parameter object as array */
                        short[] a_objects = (short[])p_o_object;

                        /* only execute if we have more than one array element */
                        if (a_objects.Length > 0)
                        {
                            /* iterate objects in primitive array and encode element */
                            for (int i = 0; i < a_objects.Length; i++)
                            {
                                /* encode primitive array object to xml recursively */
                                s_xml += this.XmlEncodeRecursive(p_o_xsdElement.Children[0], (Object)a_objects[i]);
                            }
                        }
                    }
                    else if ((s_primitiveArrayType.Equals("int")) || (s_primitiveArrayType.Equals("system.int32")))
                    {
                        /* cast current field of parameter object as array */
                        int[] a_objects = (int[])p_o_object;

                        /* only execute if we have more than one array element */
                        if (a_objects.Length > 0)
                        {
                            /* iterate objects in primitive array and encode element */
                            for (int i = 0; i < a_objects.Length; i++)
                            {
                                /* encode primitive array object to xml recursively */
                                s_xml += this.XmlEncodeRecursive(p_o_xsdElement.Children[0], (Object)a_objects[i]);
                            }
                        }
                    }
                    else if ((s_primitiveArrayType.Equals("long")) || (s_primitiveArrayType.Equals("system.int64")))
                    {
                        /* cast current field of parameter object as array */
                        long[] a_objects = (long[])p_o_object;

                        /* only execute if we have more than one array element */
                        if (a_objects.Length > 0)
                        {
                            /* iterate objects in primitive array and encode element */
                            for (int i = 0; i < a_objects.Length; i++)
                            {
                                /* encode primitive array object to xml recursively */
                                s_xml += this.XmlEncodeRecursive(p_o_xsdElement.Children[0], (Object)a_objects[i]);
                            }
                        }
                    }
                    else if ((s_primitiveArrayType.Equals("ushort")) || (s_primitiveArrayType.Equals("system.uint16")))
                    {
                        /* cast current field of parameter object as array */
                        ushort[] a_objects = (ushort[])p_o_object;

                        /* only execute if we have more than one array element */
                        if (a_objects.Length > 0)
                        {
                            /* iterate objects in primitive array and encode element */
                            for (int i = 0; i < a_objects.Length; i++)
                            {
                                /* encode primitive array object to xml recursively */
                                s_xml += this.XmlEncodeRecursive(p_o_xsdElement.Children[0], (Object)a_objects[i]);
                            }
                        }
                    }
                    else if ((s_primitiveArrayType.Equals("uint")) || (s_primitiveArrayType.Equals("system.uint32")))
                    {
                        /* cast current field of parameter object as array */
                        uint[] a_objects = (uint[])p_o_object;

                        /* only execute if we have more than one array element */
                        if (a_objects.Length > 0)
                        {
                            /* iterate objects in primitive array and encode element */
                            for (int i = 0; i < a_objects.Length; i++)
                            {
                                /* encode primitive array object to xml recursively */
                                s_xml += this.XmlEncodeRecursive(p_o_xsdElement.Children[0], (Object)a_objects[i]);
                            }
                        }
                    }
                    else if ((s_primitiveArrayType.Equals("ulong")) || (s_primitiveArrayType.Equals("system.uint64")))
                    {
                        /* cast current field of parameter object as array */
                        ulong[] a_objects = (ulong[])p_o_object;

                        /* only execute if we have more than one array element */
                        if (a_objects.Length > 0)
                        {
                            /* iterate objects in primitive array and encode element */
                            for (int i = 0; i < a_objects.Length; i++)
                            {
                                /* encode primitive array object to xml recursively */
                                s_xml += this.XmlEncodeRecursive(p_o_xsdElement.Children[0], (Object)a_objects[i]);
                            }
                        }
                    }
                    else if ((s_primitiveArrayType.Equals("string")) || (s_primitiveArrayType.Equals("system.string")))
                    {
                        /* cast current field of parameter object as array */
                        string[] a_objects = (string[])p_o_object;

                        /* only execute if we have more than one array element */
                        if (a_objects.Length > 0)
                        {
                            /* iterate objects in primitive array and encode element */
                            for (int i = 0; i < a_objects.Length; i++)
                            {
                                /* encode primitive array object to xml recursively */
                                s_xml += this.XmlEncodeRecursive(p_o_xsdElement.Children[0], (Object)a_objects[i]);
                            }
                        }
                    }
                    else if ((s_primitiveArrayType.Equals("time")) || (s_primitiveArrayType.Equals("system.timespan")))
                    {
                        /* cast current field of parameter object as array */
                        TimeSpan[] a_objects = (TimeSpan[])p_o_object;

                        /* only execute if we have more than one array element */
                        if (a_objects.Length > 0)
                        {
                            /* iterate objects in primitive array and encode element */
                            for (int i = 0; i < a_objects.Length; i++)
                            {
                                /* encode primitive array object to xml recursively */
                                s_xml += this.XmlEncodeRecursive(p_o_xsdElement.Children[0], (Object)a_objects[i]);
                            }
                        }
                    }
                    else if ((s_primitiveArrayType.Equals("date")) || (s_primitiveArrayType.Equals("datetime")) || (s_primitiveArrayType.Equals("system.datetime")))
                    {
                        /* cast current field of parameter object as array */
                        DateTime[] a_objects = (DateTime[])p_o_object;

                        /* only execute if we have more than one array element */
                        if (a_objects.Length > 0)
                        {
                            /* iterate objects in primitive array and encode element */
                            for (int i = 0; i < a_objects.Length; i++)
                            {
                                /* encode primitive array object to xml recursively */
                                s_xml += this.XmlEncodeRecursive(p_o_xsdElement.Children[0], (Object)a_objects[i]);
                            }
                        }
                    }
                    else if ((s_primitiveArrayType.Equals("decimal")) || (s_primitiveArrayType.Equals("system.decimal")))
                    {
                        /* cast current field of parameter object as array */
                        decimal[] a_objects = (decimal[])p_o_object;

                        /* only execute if we have more than one array element */
                        if (a_objects.Length > 0)
                        {
                            /* iterate objects in primitive array and encode element */
                            for (int i = 0; i < a_objects.Length; i++)
                            {
                                /* encode primitive array object to xml recursively */
                                s_xml += this.XmlEncodeRecursive(p_o_xsdElement.Children[0], (Object)a_objects[i]);
                            }
                        }
                    }
                }

                /* decrease level for PrintIdentation */
                this.i_level--;
            }

            /* render closing tag if we are not rendering an array element */
            if (!p_o_xsdElement.IsArray)
            {
                /* check if current element has any value output */
                if (s_xml.EndsWith("<" + p_o_xsdElement.Name + ">" + this.s_lineBreak))
                {
                    /* remove tag opening */
                    s_xml = s_xml.Substring(0, (s_xml.Length - (p_o_xsdElement.Name.Length + 2 + this.s_lineBreak.Length)));
                    /* close tag without a value */
                    s_xml += "<" + p_o_xsdElement.Name + "/>";
                }
                else
                {
                    /* end xml tag */
                    s_xml += this.PrintIndentation() + "</" + p_o_xsdElement.Name + ">";
                }
            }

            /* add line break */
            s_xml += this.s_lineBreak;

            return s_xml;
        }

        /// <summary>
        /// Get value of object to generate xml element with value
        /// </summary>
        /// <param name="p_o_xsdElement">xsd element object with mapping class information</param>
        /// <param name="p_o_object">object to access fields via direct public access or public access to property methods (getXX and setXX)</param>
        /// <returns>casted field value of object as object</returns>
        /// <exception cref="ArgumentException">invalid value for xml element</exception>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        /// <exception cref="FormatException">exception parsing string to target type</exception>
        private string GetElementValue(XSDElement p_o_xsdElement, Object? p_o_object)
        {
            /* variable for object data */
            Object? o_value;

            /* do not retrieve value from field or property method, because we already have array element as object in the parameter variable */
            if (!p_o_xsdElement.IsArray)
            {
                /* retrieve field information out of schema element */
                string s_fieldType = p_o_xsdElement.Mapping;

                /* remove assembly part from mapping or mapping class */
                if (s_fieldType.Contains(','))
                {
                    s_fieldType = s_fieldType.Substring(0, s_fieldType.IndexOf(","));
                }

                /* check if we use property methods with invoke to get object data values */
                if (this.UseProperties)
                {
                    /* property info for accessing property */
                    System.Reflection.PropertyInfo? o_propertyInfo;

                    /* try to get access to property info */
                    try
                    {
                        o_propertyInfo = p_o_object?.GetType().GetProperty(s_fieldType);

                        if (o_propertyInfo == null)
                        {
                            throw new Exception("property info is null");
                        }
                    }
                    catch (Exception o_exc)
                    {
                        throw new MissingMemberException("Class instance property[" + s_fieldType + "] does not exist for class instance(" + p_o_object?.GetType().FullName + "): " + o_exc.Message);
                    }

                    /* check if we can access property */
                    if (!o_propertyInfo.CanRead)
                    {
                        throw new MemberAccessException("Cannot write property from class; instance property[" + s_fieldType + "] does not exist for class instance(" + p_o_object?.GetType().FullName + ")");
                    }

                    /* get object data of current property */
                    o_value = o_propertyInfo.GetValue(p_o_object);
                }
                else
                {
                    /* field info for accessing field */
                    System.Reflection.FieldInfo? o_fieldInfo;

                    /* try to get access to field info */
                    try
                    {
                        o_fieldInfo = p_o_object?.GetType().GetField(s_fieldType);

                        if (o_fieldInfo == null)
                        {
                            throw new Exception("field info is null");
                        }
                    }
                    catch (Exception o_exc)
                    {
                        throw new MissingMemberException("Class instance field[" + s_fieldType + "] does not exist for class instance(" + p_o_object?.GetType().FullName + "): " + o_exc.Message);
                    }

                    /* check if we can access field */
                    if (!o_fieldInfo.IsPublic)
                    {
                        throw new MemberAccessException("Cannot read field from class; instance field[" + s_fieldType + "] does not exist for class instance(" + p_o_object?.GetType().FullName + ")");
                    }

                    /* get object data of current field */
                    o_value = o_fieldInfo.GetValue(p_o_object);
                }
            }
            else
            {
                /* we already have array element as object in the parameter variable */
                o_value = p_o_object;
            }

            /* cast object data to string value, based on xsd attribute type */
            string s_foo = this.CastStringFromObject(o_value, p_o_xsdElement.Type);

            /* check if xs:element has any restrictions */
            if (p_o_xsdElement.Restriction)
            {
                bool b_enumerationFound = false;
                bool b_enumerationReturnValue = false;

                foreach (XSDRestriction o_xsdRestriction in p_o_xsdElement.Restrictions)
                {
                    if (o_xsdRestriction.Name.ToLower().Equals("enumeration"))
                    {
                        b_enumerationFound = true;
                    }

                    b_enumerationReturnValue = this.CheckRestriction(s_foo, o_xsdRestriction, p_o_xsdElement.Type);

                    if ((b_enumerationFound) && (b_enumerationReturnValue))
                    {
                        break;
                    }
                }

                if ((b_enumerationFound) && (!b_enumerationReturnValue))
                {
                    throw new ArgumentException("Element[" + p_o_xsdElement.Name + "] with value[" + s_foo + "] does not match enumaration restrictions defined in xsd-schema for this xs:element");
                }
            }

            /* return value */
            return s_foo;
        }

        /// <summary>
        /// Get value of object to generate xml attribute with value
        /// </summary>
        /// <param name="p_o_xsdAttribute">xsd attribute object with mapping class information</param>
        /// <param name="p_o_object">object to access fields via direct public access or public access to property methods (getXX and setXX)</param>
        /// <returns>casted field value of object as object</returns>
        /// <exception cref="ArgumentException">invalid value for xml attribute</exception>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        /// <exception cref="FormatException">exception parsing string to target type</exception>
        private string GetAttributeValue(XSDAttribute p_o_xsdAttribute, Object? p_o_object)
        {
            /* if attribute has fixed value, return fixed value */
            if (!ForestNET.Lib.Helper.IsStringEmpty(p_o_xsdAttribute.Fixed))
            {
                return p_o_xsdAttribute.Fixed;
            }

            /* variable for object data */
            Object? o_value;

            /* retrieve field information out of schema element */
            string s_fieldType = p_o_xsdAttribute.Mapping;

            /* remove assembly part from mapping or mapping class */
            if (s_fieldType.Contains(','))
            {
                s_fieldType = s_fieldType.Substring(0, s_fieldType.IndexOf(","));
            }

            /* check if we use property methods with invoke to get object data values */
            if (this.UseProperties)
            {
                /* property info for accessing property */
                System.Reflection.PropertyInfo? o_propertyInfo;

                /* try to get access to property info */
                try
                {
                    o_propertyInfo = p_o_object?.GetType().GetProperty(s_fieldType);

                    if (o_propertyInfo == null)
                    {
                        throw new Exception("property info is null");
                    }
                }
                catch (Exception o_exc)
                {
                    throw new MissingMemberException("Class instance property[" + s_fieldType + "] does not exist for class instance(" + p_o_object?.GetType().FullName + "): " + o_exc.Message);
                }

                /* check if we can access property */
                if (!o_propertyInfo.CanRead)
                {
                    throw new MemberAccessException("Cannot write property from class; instance property[" + s_fieldType + "] does not exist for class instance(" + p_o_object?.GetType().FullName + ")");
                }

                /* get object data of current property */
                o_value = o_propertyInfo.GetValue(p_o_object);
            }
            else
            {
                /* field info for accessing field */
                System.Reflection.FieldInfo? o_fieldInfo;

                /* try to get access to field info */
                try
                {
                    o_fieldInfo = p_o_object?.GetType().GetField(s_fieldType);

                    if (o_fieldInfo == null)
                    {
                        throw new Exception("field info is null");
                    }
                }
                catch (Exception o_exc)
                {
                    throw new MissingMemberException("Class instance field[" + s_fieldType + "] does not exist for class instance(" + p_o_object?.GetType().FullName + "): " + o_exc.Message);
                }

                /* check if we can access field */
                if (!o_fieldInfo.IsPublic)
                {
                    throw new MemberAccessException("Cannot read field from class; instance field[" + s_fieldType + "] does not exist for class instance(" + p_o_object?.GetType().FullName + ")");
                }

                /* get object data of current field */
                o_value = o_fieldInfo.GetValue(p_o_object);
            }

            /* cast object data to string value, based on xsd attribute type */
            string s_foo = this.CastStringFromObject(o_value, p_o_xsdAttribute.Type);

            /* check if xs:attribute has any restrictions, when value is not empty */
            if ((p_o_xsdAttribute.Restriction) && (!ForestNET.Lib.Helper.IsStringEmpty(s_foo)))
            {
                bool b_enumerationFound = false;
                bool b_enumerationReturnValue = false;

                foreach (XSDRestriction o_xsdRestriction in p_o_xsdAttribute.Restrictions)
                {
                    if (o_xsdRestriction.Name.ToLower().Equals("enumeration"))
                    {
                        b_enumerationFound = true;
                    }

                    b_enumerationReturnValue = this.CheckRestriction(s_foo, o_xsdRestriction, p_o_xsdAttribute.Type);

                    if ((b_enumerationFound) && (b_enumerationReturnValue))
                    {
                        break;
                    }
                }

                if ((b_enumerationFound) && (!b_enumerationReturnValue))
                {
                    throw new ArgumentException("Attribute[" + p_o_xsdAttribute.Name + "] with value[" + s_foo + "] does not match enumaration restrictions defined in xsd-schema for this xs:attribute");
                }
            }

            /* if object data is empty, but we have a default value, return default value */
            if ((!ForestNET.Lib.Helper.IsStringEmpty(p_o_xsdAttribute.Default)) && (ForestNET.Lib.Helper.IsStringEmpty(s_foo)))
            {
                s_foo = p_o_xsdAttribute.Default;
            }

            return s_foo;
        }

        /// <summary>
        /// Method to cast an object value to a string value for encoding xml data
        /// </summary>
        /// <param name="p_o_object">object value which will be casted to string</param>
        /// <param name="p_s_type">type as string to distinguish</param>
        /// <returns>casted object value as string</returns>
        /// <exception cref="ArgumentException">invalid or empty type value</exception>
        private string CastStringFromObject(Object? p_o_object, string? p_s_type)
        {
            string s_foo = "";

            if (p_o_object != null)
            {
                if (p_s_type == null)
                {
                    throw new ArgumentException("Invalid type[null] for " + p_o_object.GetType().FullName);
                }

                p_s_type = p_s_type.ToLower();

                if (p_s_type.Equals("boolean"))
                {
                    bool o_foo = Convert.ToBoolean(p_o_object);
                    s_foo = o_foo.ToString().ToLower();
                }
                else if ((p_s_type.Equals("string")) || (p_s_type.Equals("duration")))
                {
                    s_foo = p_o_object.ToString() ?? "";

                    if ((s_foo.Length == 0) && (this.b_printEmptystring))
                    {
                        s_foo = "&#x200B;";
                    }
                }
                else if ((p_s_type.Equals("date")) || (p_s_type.Equals("time")) || (p_s_type.Equals("datetime")))
                {
                    if (p_o_object.GetType() == typeof(DateTime))
                    {
                        DateTime o_foo = Convert.ToDateTime(p_o_object);

                        if (this.b_useISO8601UTC)
                        {
                            s_foo = ForestNET.Lib.Helper.ToISO8601UTC(o_foo);
                        }
                        else if (p_s_type.Equals("date"))
                        {
                            s_foo = o_foo.ToString(this.s_dateFormat, System.Globalization.CultureInfo.InvariantCulture);
                        }
                        else if (p_s_type.Equals("datetime"))
                        {
                            s_foo = o_foo.ToString(this.s_dateTimeFormat, System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }
                    else if (p_o_object.GetType() == typeof(TimeSpan))
                    {
                        TimeSpan o_foo = (TimeSpan)p_o_object;
                        DateTime o_fooo = default;
                        o_fooo += o_foo;

                        if (this.b_useISO8601UTC)
                        {
                            s_foo = ForestNET.Lib.Helper.ToISO8601UTC(o_fooo);
                        }
                        else if (p_s_type.Equals("time"))
                        {
                            s_foo = o_fooo.ToString(this.s_timeFormat, System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Illegal object type '" + p_o_object.GetType().FullName + "' for type '" + p_s_type + "'");
                    }
                }
                else if (p_s_type.Equals("decimal"))
                {
                    if (p_o_object.GetType() == typeof(Single))
                    {
                        float o_foo = Convert.ToSingle(p_o_object);
                        s_foo = o_foo.ToString("0.000000000", new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    }
                    else if (p_o_object.GetType() == typeof(Double))
                    {
                        double o_foo = Convert.ToDouble(p_o_object);
                        s_foo = o_foo.ToString("0.00000000000000000", new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    }
                    else
                    {
                        decimal o_foo = Convert.ToDecimal(p_o_object);
                        s_foo = o_foo.ToString("0.00000000000000000000000000000", new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    }

                    if ((s_foo.Equals("0")) || (s_foo.Equals("0.0")))
                    {
                        s_foo = "0.0";
                    }
                }
                else if (p_s_type.Equals("double"))
                {
                    double o_foo = Convert.ToDouble(p_o_object);
                    s_foo = o_foo.ToString("0.00000000000000000", new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                }
                else if (p_s_type.Equals("float"))
                {
                    float o_foo = Convert.ToSingle(p_o_object);
                    s_foo = o_foo.ToString("0.000000000", new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                }
                else if (p_s_type.Equals("short"))
                {
                    if (p_o_object.GetType() == typeof(short))
                    {
                        short o_foo = Convert.ToInt16(p_o_object);
                        s_foo = o_foo.ToString();
                    }
                    else if (p_o_object.GetType() == typeof(ushort))
                    {
                        ushort o_foo = Convert.ToUInt16(p_o_object);
                        s_foo = o_foo.ToString();
                    }
                }
                else if (p_s_type.Equals("long"))
                {
                    if (p_o_object.GetType() == typeof(long))
                    {
                        long o_foo = Convert.ToInt64(p_o_object);
                        s_foo = o_foo.ToString();
                    }
                    else if (p_o_object.GetType() == typeof(ulong))
                    {
                        ulong o_foo = Convert.ToUInt64(p_o_object);
                        s_foo = o_foo.ToString();
                    }
                }
                else if ((p_s_type.Equals("integer")) || (p_s_type.Equals("int")) || (p_s_type.Equals("positiveinteger")) || (p_s_type.Equals("unsignedshort")) || (p_s_type.Equals("unsignedint")) || (p_s_type.Equals("unsignedinteger")) || (p_s_type.Equals("unsignedlong")) || (p_s_type.Equals("byte")) || (p_s_type.Equals("sbyte")) || (p_s_type.Equals("unsignedbyte")))
                {
                    if (p_o_object.GetType() == typeof(short))
                    {
                        short o_foo = Convert.ToInt16(p_o_object);
                        s_foo = o_foo.ToString();
                    }
                    else if (p_o_object.GetType() == typeof(int))
                    {
                        int o_foo = Convert.ToInt32(p_o_object);
                        s_foo = o_foo.ToString();
                    }
                    else if (p_o_object.GetType() == typeof(long))
                    {
                        long o_foo = Convert.ToInt64(p_o_object);
                        s_foo = o_foo.ToString();
                    }
                    else if (p_o_object.GetType() == typeof(ushort))
                    {
                        ushort o_foo = Convert.ToUInt16(p_o_object);
                        s_foo = o_foo.ToString();
                    }
                    else if (p_o_object.GetType() == typeof(uint))
                    {
                        uint o_foo = Convert.ToUInt32(p_o_object);
                        s_foo = o_foo.ToString();
                    }
                    else if (p_o_object.GetType() == typeof(ulong))
                    {
                        ulong o_foo = Convert.ToUInt64(p_o_object);
                        s_foo = o_foo.ToString();
                    }
                    else if (p_o_object.GetType() == typeof(byte))
                    {
                        byte o_foo = Convert.ToByte(p_o_object);
                        s_foo = o_foo.ToString();
                    }
                    else if (p_o_object.GetType() == typeof(sbyte))
                    {
                        sbyte o_foo = Convert.ToSByte(p_o_object);
                        s_foo = o_foo.ToString();
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid type[" + p_s_type + "] for " + p_o_object.GetType().FullName);
                }
            }

            return s_foo;
        }

        /// <summary>
        /// Check if xsd element restriction is valid with current value
        /// timestamp comparison in restrictions(datetime, date and time) will always be executed with DateTime class with internal conversion of it's values
        /// </summary>
        /// <param name="p_s_value">string value for xsd element restriction, can be casted to integer as well</param>
        /// <param name="p_o_xsdRestriction">xsd restriction object which holds all restriction information</param>
        /// <param name="p_s_type">xml element type</param>
        /// <exception cref="ArgumentException">unknown restriction name, restriction error or invalid type from xsd element object</exception>
        /// <exception cref="FormatException">exception parsing string to target type</exception>
        private bool CheckRestriction(string? p_s_value, XSDRestriction p_o_xsdRestriction, string? p_s_type)
        {
            /* check if parameter value is null */
            if (p_s_value == null)
            {
                throw new ArgumentException("Restriction error: value[null] is null for " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.StrValue + "]");
            }

            /* check if type of yaml parameter is null */
            if (p_s_type == null)
            {
                throw new ArgumentException("Restriction error: value[" + p_s_value + "] for " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.StrValue + "] cannot be used, because type parameter is 'null'");
            }

            bool b_enumerationReturnValue = false;

            List<string> a_stringTypes = ["string", "duration", "hexbinary", "base64binary", "anyuri", "normalizedstring", "token", "language", "name", "ncname", "nmtoken", "id", "idref", "entity"];
            List<string> a_integerTypes = ["integer", "int", "positiveinteger", "nonpositiveinteger", "negativeinteger", "nonnegativeinteger", "byte", "unsignedint", "unsignedbyte"];
            string p_s_typeLower = p_s_type.ToLower();

            if (p_o_xsdRestriction.Name.ToLower().Equals("minexclusive"))
            {
                if (p_s_typeLower.Equals("boolean"))
                {
                    throw new ArgumentException("Cannot use " + p_o_xsdRestriction.Name + " restriction on type: " + p_s_type);
                }
                else if (a_stringTypes.Contains(p_s_typeLower))
                {
                    throw new ArgumentException("Cannot use " + p_o_xsdRestriction.Name + " restriction on type: " + p_s_type);
                }
                else if (p_s_typeLower.Equals("date"))
                {
                    DateTime o_value = ForestNET.Lib.Helper.FromDateString(p_s_value).Date + new DateTime(1900, 1, 1, 0, 0, 0).TimeOfDay;
                    DateTime o_restriction = ForestNET.Lib.Helper.FromDateString(p_o_xsdRestriction.StrValue).Date + new DateTime(1900, 1, 1, 0, 0, 0).TimeOfDay;

                    if (this.b_useISO8601UTC)
                    {
                        o_value = ForestNET.Lib.Helper.FromISO8601UTC(p_s_value);
                    }

                    int i_compare = o_value.CompareTo(o_restriction);

                    if (i_compare == -1)
                    {
                        throw new ArgumentException("Value[" + o_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.StrValue + "]");
                    }
                }
                else if (p_s_typeLower.Equals("time"))
                {
                    DateTime o_value = new DateTime(1900, 1, 1, 0, 0, 0).Date + ForestNET.Lib.Helper.FromTimeString(p_s_value);
                    DateTime o_restriction = new DateTime(1900, 1, 1, 0, 0, 0).Date + ForestNET.Lib.Helper.FromTimeString(p_o_xsdRestriction.StrValue);

                    if (this.b_useISO8601UTC)
                    {
                        o_value = ForestNET.Lib.Helper.FromISO8601UTC(p_s_value);
                    }

                    int i_compare = o_value.CompareTo(o_restriction);

                    if (i_compare == -1)
                    {
                        throw new ArgumentException("Value[" + o_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.StrValue + "]");
                    }
                }
                else if (p_s_typeLower.Equals("datetime"))
                {
                    DateTime o_value = ForestNET.Lib.Helper.FromDateTimeString(p_s_value);
                    DateTime o_restriction = ForestNET.Lib.Helper.FromDateTimeString(p_o_xsdRestriction.StrValue);

                    if (this.b_useISO8601UTC)
                    {
                        o_value = ForestNET.Lib.Helper.FromISO8601UTC(p_s_value);
                    }

                    int i_compare = o_value.CompareTo(o_restriction);

                    if (i_compare == -1)
                    {
                        throw new ArgumentException("Value[" + o_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.StrValue + "]");
                    }
                }
                else if (p_s_typeLower.Equals("decimal"))
                {
                    decimal o_value = Convert.ToDecimal(p_s_value, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    decimal o_restriction = Convert.ToDecimal(p_o_xsdRestriction.StrValue, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });

                    int i_compare = o_value.CompareTo(o_restriction);

                    if (i_compare == -1)
                    {
                        throw new ArgumentException("Value[" + o_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.StrValue + "]");
                    }
                }
                else if (p_s_typeLower.Equals("double"))
                {
                    double o_value = Convert.ToDouble(p_s_value, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    double o_restriction = Convert.ToDouble(p_o_xsdRestriction.StrValue, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });

                    int i_compare = o_value.CompareTo(o_restriction);

                    if (i_compare == -1)
                    {
                        throw new ArgumentException("Value[" + o_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.StrValue + "]");
                    }
                }
                else if (p_s_typeLower.Equals("float"))
                {
                    float o_value = Convert.ToSingle(p_s_value, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    float o_restriction = Convert.ToSingle(p_o_xsdRestriction.StrValue, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });

                    int i_compare = o_value.CompareTo(o_restriction);

                    if (i_compare == -1)
                    {
                        throw new ArgumentException("Value[" + o_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.StrValue + "]");
                    }
                }
                else if (a_integerTypes.Contains(p_s_typeLower))
                {
                    int i_value = int.Parse(p_s_value);
                    int i_restriction = p_o_xsdRestriction.IntValue;

                    int i_compare = i_value.CompareTo(i_restriction);

                    if (i_compare == -1)
                    {
                        throw new ArgumentException("Value[" + i_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.IntValue + "]");
                    }
                }
                else if (p_s_typeLower.Equals("long") || p_s_typeLower.Equals("unsignedlong"))
                {
                    long l_value = long.Parse(p_s_value);
                    long l_restriction = long.Parse(p_o_xsdRestriction.StrValue);

                    int i_compare = l_value.CompareTo(l_restriction);

                    if (i_compare == -1)
                    {
                        throw new ArgumentException("Value[" + l_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.StrValue + "]");
                    }
                }
                else if (p_s_typeLower.Equals("short") || p_s_typeLower.Equals("unsignedshort"))
                {
                    short sh_value = short.Parse(p_s_value);
                    short sh_restriction = short.Parse(p_o_xsdRestriction.StrValue);

                    int i_compare = sh_value.CompareTo(sh_restriction);

                    if (i_compare == -1)
                    {
                        throw new ArgumentException("Value[" + sh_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.IntValue + "]");
                    }
                }
            }
            else if (p_o_xsdRestriction.Name.ToLower().Equals("maxexclusive"))
            {
                if (p_s_typeLower.Equals("boolean"))
                {
                    throw new ArgumentException("Cannot use " + p_o_xsdRestriction.Name + " restriction on type: " + p_s_type);
                }
                else if (a_stringTypes.Contains(p_s_typeLower))
                {
                    throw new ArgumentException("Cannot use " + p_o_xsdRestriction.Name + " restriction on type: " + p_s_type);
                }
                else if (p_s_typeLower.Equals("date"))
                {
                    DateTime o_value = ForestNET.Lib.Helper.FromDateString(p_s_value).Date + new DateTime(1900, 1, 1, 0, 0, 0).TimeOfDay;
                    DateTime o_restriction = ForestNET.Lib.Helper.FromDateString(p_o_xsdRestriction.StrValue).Date + new DateTime(1900, 1, 1, 0, 0, 0).TimeOfDay;

                    if (this.b_useISO8601UTC)
                    {
                        o_value = ForestNET.Lib.Helper.FromISO8601UTC(p_s_value);
                    }

                    int i_compare = o_value.CompareTo(o_restriction);

                    if (i_compare == 1)
                    {
                        throw new ArgumentException("Value[" + o_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.StrValue + "]");
                    }
                }
                else if (p_s_typeLower.Equals("time"))
                {
                    DateTime o_value = new DateTime(1900, 1, 1, 0, 0, 0).Date + ForestNET.Lib.Helper.FromTimeString(p_s_value);
                    DateTime o_restriction = new DateTime(1900, 1, 1, 0, 0, 0).Date + ForestNET.Lib.Helper.FromTimeString(p_o_xsdRestriction.StrValue);

                    if (this.b_useISO8601UTC)
                    {
                        o_value = ForestNET.Lib.Helper.FromISO8601UTC(p_s_value);
                    }

                    int i_compare = o_value.CompareTo(o_restriction);

                    if (i_compare == 1)
                    {
                        throw new ArgumentException("Value[" + o_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.StrValue + "]");
                    }
                }
                else if (p_s_typeLower.Equals("datetime"))
                {
                    DateTime o_value = ForestNET.Lib.Helper.FromDateTimeString(p_s_value);
                    DateTime o_restriction = ForestNET.Lib.Helper.FromDateTimeString(p_o_xsdRestriction.StrValue);

                    if (this.b_useISO8601UTC)
                    {
                        o_value = ForestNET.Lib.Helper.FromISO8601UTC(p_s_value);
                    }

                    int i_compare = o_value.CompareTo(o_restriction);

                    if (i_compare == 1)
                    {
                        throw new ArgumentException("Value[" + o_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.StrValue + "]");
                    }
                }
                else if (p_s_typeLower.Equals("decimal"))
                {
                    decimal o_value = Convert.ToDecimal(p_s_value, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    decimal o_restriction = Convert.ToDecimal(p_o_xsdRestriction.StrValue, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });

                    int i_compare = o_value.CompareTo(o_restriction);

                    if (i_compare == 1)
                    {
                        throw new ArgumentException("Value[" + o_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.StrValue + "]");
                    }
                }
                else if (p_s_typeLower.Equals("double"))
                {
                    double o_value = Convert.ToDouble(p_s_value, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    double o_restriction = Convert.ToDouble(p_o_xsdRestriction.StrValue, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });

                    int i_compare = o_value.CompareTo(o_restriction);

                    if (i_compare == 1)
                    {
                        throw new ArgumentException("Value[" + o_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.StrValue + "]");
                    }
                }
                else if (p_s_typeLower.Equals("float"))
                {
                    float o_value = Convert.ToSingle(p_s_value, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    float o_restriction = Convert.ToSingle(p_o_xsdRestriction.StrValue, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });

                    int i_compare = o_value.CompareTo(o_restriction);

                    if (i_compare == 1)
                    {
                        throw new ArgumentException("Value[" + o_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.StrValue + "]");
                    }
                }
                else if (a_integerTypes.Contains(p_s_typeLower))
                {
                    int i_value = int.Parse(p_s_value);
                    int i_restriction = p_o_xsdRestriction.IntValue;

                    int i_compare = i_value.CompareTo(i_restriction);

                    if (i_compare == 1)
                    {
                        throw new ArgumentException("Value[" + i_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.IntValue + "]");
                    }
                }
                else if (p_s_typeLower.Equals("long") || p_s_typeLower.Equals("unsignedlong"))
                {
                    long l_value = long.Parse(p_s_value);
                    long l_restriction = long.Parse(p_o_xsdRestriction.StrValue);

                    int i_compare = l_value.CompareTo(l_restriction);

                    if (i_compare == 1)
                    {
                        throw new ArgumentException("Value[" + l_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.StrValue + "]");
                    }
                }
                else if (p_s_typeLower.Equals("short") || p_s_typeLower.Equals("unsignedshort"))
                {
                    short sh_value = short.Parse(p_s_value);
                    short sh_restriction = short.Parse(p_o_xsdRestriction.StrValue);

                    int i_compare = sh_value.CompareTo(sh_restriction);

                    if (i_compare == 1)
                    {
                        throw new ArgumentException("Value[" + sh_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.IntValue + "]");
                    }
                }
            }
            else if (p_o_xsdRestriction.Name.ToLower().Equals("mininclusive"))
            {
                if (p_s_typeLower.Equals("boolean"))
                {
                    throw new ArgumentException("Cannot use " + p_o_xsdRestriction.Name + " restriction on type: " + p_s_type);
                }
                else if (a_stringTypes.Contains(p_s_typeLower))
                {
                    throw new ArgumentException("Cannot use " + p_o_xsdRestriction.Name + " restriction on type: " + p_s_type);
                }
                else if (p_s_typeLower.Equals("date"))
                {
                    DateTime o_value = ForestNET.Lib.Helper.FromDateString(p_s_value).Date + new DateTime(1900, 1, 1, 0, 0, 0).TimeOfDay;
                    DateTime o_restriction = ForestNET.Lib.Helper.FromDateString(p_o_xsdRestriction.StrValue).Date + new DateTime(1900, 1, 1, 0, 0, 0).TimeOfDay;

                    if (this.b_useISO8601UTC)
                    {
                        o_value = ForestNET.Lib.Helper.FromISO8601UTC(p_s_value);
                    }

                    int i_compare = o_value.CompareTo(o_restriction);

                    if (i_compare < 1)
                    {
                        throw new ArgumentException("Value[" + o_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.StrValue + "]");
                    }
                }
                else if (p_s_typeLower.Equals("time"))
                {
                    DateTime o_value = new DateTime(1900, 1, 1, 0, 0, 0).Date + ForestNET.Lib.Helper.FromTimeString(p_s_value);
                    DateTime o_restriction = new DateTime(1900, 1, 1, 0, 0, 0).Date + ForestNET.Lib.Helper.FromTimeString(p_o_xsdRestriction.StrValue);

                    if (this.b_useISO8601UTC)
                    {
                        o_value = ForestNET.Lib.Helper.FromISO8601UTC(p_s_value);
                    }

                    int i_compare = o_value.CompareTo(o_restriction);

                    if (i_compare < 1)
                    {
                        throw new ArgumentException("Value[" + o_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.StrValue + "]");
                    }
                }
                else if (p_s_typeLower.Equals("datetime"))
                {
                    DateTime o_value = ForestNET.Lib.Helper.FromDateTimeString(p_s_value);
                    DateTime o_restriction = ForestNET.Lib.Helper.FromDateTimeString(p_o_xsdRestriction.StrValue);

                    if (this.b_useISO8601UTC)
                    {
                        o_value = ForestNET.Lib.Helper.FromISO8601UTC(p_s_value);
                    }

                    int i_compare = o_value.CompareTo(o_restriction);

                    if (i_compare < 1)
                    {
                        throw new ArgumentException("Value[" + o_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.StrValue + "]");
                    }
                }
                else if (p_s_typeLower.Equals("decimal"))
                {
                    decimal o_value = Convert.ToDecimal(p_s_value, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    decimal o_restriction = Convert.ToDecimal(p_o_xsdRestriction.StrValue, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });

                    int i_compare = o_value.CompareTo(o_restriction);

                    if (i_compare < 1)
                    {
                        throw new ArgumentException("Value[" + o_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.StrValue + "]");
                    }
                }
                else if (p_s_typeLower.Equals("double"))
                {
                    double o_value = Convert.ToDouble(p_s_value, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    double o_restriction = Convert.ToDouble(p_o_xsdRestriction.StrValue, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });

                    int i_compare = o_value.CompareTo(o_restriction);

                    if (i_compare < 1)
                    {
                        throw new ArgumentException("Value[" + o_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.StrValue + "]");
                    }
                }
                else if (p_s_typeLower.Equals("float"))
                {
                    float o_value = Convert.ToSingle(p_s_value, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    float o_restriction = Convert.ToSingle(p_o_xsdRestriction.StrValue, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });

                    int i_compare = o_value.CompareTo(o_restriction);

                    if (i_compare < 1)
                    {
                        throw new ArgumentException("Value[" + o_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.StrValue + "]");
                    }
                }
                else if (a_integerTypes.Contains(p_s_typeLower))
                {
                    int i_value = int.Parse(p_s_value);
                    int i_restriction = p_o_xsdRestriction.IntValue;

                    int i_compare = i_value.CompareTo(i_restriction);

                    if (i_compare < 1)
                    {
                        throw new ArgumentException("Value[" + i_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.IntValue + "]");
                    }
                }
                else if (p_s_typeLower.Equals("long") || p_s_typeLower.Equals("unsignedlong"))
                {
                    long l_value = long.Parse(p_s_value);
                    long l_restriction = long.Parse(p_o_xsdRestriction.StrValue);

                    int i_compare = l_value.CompareTo(l_restriction);

                    if (i_compare < 1)
                    {
                        throw new ArgumentException("Value[" + l_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.StrValue + "]");
                    }
                }
                else if (p_s_typeLower.Equals("short") || p_s_typeLower.Equals("unsignedshort"))
                {
                    short sh_value = short.Parse(p_s_value);
                    short sh_restriction = short.Parse(p_o_xsdRestriction.StrValue);

                    int i_compare = sh_value.CompareTo(sh_restriction);

                    if (i_compare < 1)
                    {
                        throw new ArgumentException("Value[" + sh_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.IntValue + "]");
                    }
                }
            }
            else if (p_o_xsdRestriction.Name.ToLower().Equals("maxinclusive"))
            {
                if (p_s_typeLower.Equals("boolean"))
                {
                    throw new ArgumentException("Cannot use " + p_o_xsdRestriction.Name + " restriction on type: " + p_s_type);
                }
                else if (a_stringTypes.Contains(p_s_typeLower))
                {
                    throw new ArgumentException("Cannot use " + p_o_xsdRestriction.Name + " restriction on type: " + p_s_type);
                }
                else if (p_s_typeLower.Equals("date"))
                {
                    DateTime o_value = ForestNET.Lib.Helper.FromDateString(p_s_value).Date + new DateTime(1900, 1, 1, 0, 0, 0).TimeOfDay;
                    DateTime o_restriction = ForestNET.Lib.Helper.FromDateString(p_o_xsdRestriction.StrValue).Date + new DateTime(1900, 1, 1, 0, 0, 0).TimeOfDay;

                    if (this.b_useISO8601UTC)
                    {
                        o_value = ForestNET.Lib.Helper.FromISO8601UTC(p_s_value);
                    }

                    int i_compare = o_value.CompareTo(o_restriction);

                    if (i_compare > -1)
                    {
                        throw new ArgumentException("Value[" + o_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.StrValue + "]");
                    }
                }
                else if (p_s_typeLower.Equals("time"))
                {
                    DateTime o_value = new DateTime(1900, 1, 1, 0, 0, 0).Date + ForestNET.Lib.Helper.FromTimeString(p_s_value);
                    DateTime o_restriction = new DateTime(1900, 1, 1, 0, 0, 0).Date + ForestNET.Lib.Helper.FromTimeString(p_o_xsdRestriction.StrValue);

                    if (this.b_useISO8601UTC)
                    {
                        o_value = ForestNET.Lib.Helper.FromISO8601UTC(p_s_value);
                    }

                    int i_compare = o_value.CompareTo(o_restriction);

                    if (i_compare > -1)
                    {
                        throw new ArgumentException("Value[" + o_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.StrValue + "]");
                    }
                }
                else if (p_s_typeLower.Equals("datetime"))
                {
                    DateTime o_value = ForestNET.Lib.Helper.FromDateTimeString(p_s_value);
                    DateTime o_restriction = ForestNET.Lib.Helper.FromDateTimeString(p_o_xsdRestriction.StrValue);

                    if (this.b_useISO8601UTC)
                    {
                        o_value = ForestNET.Lib.Helper.FromISO8601UTC(p_s_value);
                    }

                    int i_compare = o_value.CompareTo(o_restriction);

                    if (i_compare > -1)
                    {
                        throw new ArgumentException("Value[" + o_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.StrValue + "]");
                    }
                }
                else if (p_s_typeLower.Equals("decimal"))
                {
                    decimal o_value = Convert.ToDecimal(p_s_value, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    decimal o_restriction = Convert.ToDecimal(p_o_xsdRestriction.StrValue, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });

                    int i_compare = o_value.CompareTo(o_restriction);

                    if (i_compare > -1)
                    {
                        throw new ArgumentException("Value[" + o_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.StrValue + "]");
                    }
                }
                else if (p_s_typeLower.Equals("double"))
                {
                    double o_value = Convert.ToDouble(p_s_value, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    double o_restriction = Convert.ToDouble(p_o_xsdRestriction.StrValue, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });

                    int i_compare = o_value.CompareTo(o_restriction);

                    if (i_compare > -1)
                    {
                        throw new ArgumentException("Value[" + o_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.StrValue + "]");
                    }
                }
                else if (p_s_typeLower.Equals("float"))
                {
                    float o_value = Convert.ToSingle(p_s_value, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    float o_restriction = Convert.ToSingle(p_o_xsdRestriction.StrValue, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });

                    int i_compare = o_value.CompareTo(o_restriction);

                    if (i_compare > -1)
                    {
                        throw new ArgumentException("Value[" + o_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.StrValue + "]");
                    }
                }
                else if (a_integerTypes.Contains(p_s_typeLower))
                {
                    int i_value = int.Parse(p_s_value);
                    int i_restriction = p_o_xsdRestriction.IntValue;

                    int i_compare = i_value.CompareTo(i_restriction);

                    if (i_compare > -1)
                    {
                        throw new ArgumentException("Value[" + i_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.IntValue + "]");
                    }
                }
                else if (p_s_typeLower.Equals("long") || p_s_typeLower.Equals("unsignedlong"))
                {
                    long l_value = long.Parse(p_s_value);
                    long l_restriction = long.Parse(p_o_xsdRestriction.StrValue);

                    int i_compare = l_value.CompareTo(l_restriction);

                    if (i_compare > -1)
                    {
                        throw new ArgumentException("Value[" + l_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.StrValue + "]");
                    }
                }
                else if (p_s_typeLower.Equals("short") || p_s_typeLower.Equals("unsignedshort"))
                {
                    short sh_value = short.Parse(p_s_value);
                    short sh_restriction = short.Parse(p_o_xsdRestriction.StrValue);

                    int i_compare = sh_value.CompareTo(sh_restriction);

                    if (i_compare > -1)
                    {
                        throw new ArgumentException("Value[" + sh_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.IntValue + "]");
                    }
                }
            }
            else if (p_o_xsdRestriction.Name.ToLower().Equals("totaldigits"))
            {
                if (p_s_typeLower.Equals("decimal") || p_s_typeLower.Equals("double") || p_s_typeLower.Equals("float"))
                {
                    string s_foo = p_s_value;

                    if (s_foo.StartsWith("+") || s_foo.StartsWith("-"))
                    {
                        s_foo = s_foo.Substring(1);
                    }

                    if (s_foo.Contains('.'))
                    {
                        s_foo = s_foo.Substring(0, s_foo.IndexOf("."));
                    }
                    else if (s_foo.Contains(','))
                    {
                        s_foo = s_foo.Substring(0, s_foo.IndexOf(","));
                    }

                    int i_length = s_foo.Length;

                    if (i_length > p_o_xsdRestriction.IntValue)
                    {
                        throw new ArgumentException("Value[" + p_s_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.IntValue + "]");
                    }
                }
                else if (a_integerTypes.Contains(p_s_typeLower) || p_s_typeLower.Equals("long") || p_s_typeLower.Equals("unsignedlong") || p_s_typeLower.Equals("short") || p_s_typeLower.Equals("unsignedshort"))
                {
                    int i_length = p_s_value.Length;

                    if (p_s_value.StartsWith("+") || p_s_value.StartsWith("-"))
                    {
                        i_length--;
                    }

                    if (i_length > p_o_xsdRestriction.IntValue)
                    {
                        throw new ArgumentException("Value[" + p_s_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.IntValue + "]");
                    }
                }
                else
                {
                    throw new ArgumentException("Cannot use " + p_o_xsdRestriction.Name + " restriction on type: " + p_s_type);
                }
            }
            else if (p_o_xsdRestriction.Name.ToLower().Equals("fractiondigits"))
            {
                if (p_s_typeLower.Equals("decimal") || p_s_typeLower.Equals("double") || p_s_typeLower.Equals("float"))
                {
                    string s_foo = p_s_value;

                    if (s_foo.Contains('.'))
                    {
                        s_foo = s_foo.Substring(s_foo.IndexOf(".") + 1);
                    }
                    else if (p_s_value.Contains(','))
                    {
                        s_foo = s_foo.Substring(s_foo.IndexOf(",") + 1);
                    }

                    int i_length = s_foo.Length;

                    if (i_length > p_o_xsdRestriction.IntValue)
                    {
                        throw new ArgumentException("Value[" + p_s_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.IntValue + "]");
                    }
                }
                else
                {
                    throw new ArgumentException("Cannot use " + p_o_xsdRestriction.Name + " restriction on type: " + p_s_type);
                }
            }
            else if (p_o_xsdRestriction.Name.ToLower().Equals("length"))
            {
                if (a_stringTypes.Contains(p_s_typeLower))
                {
                    if (p_s_value.Length != p_o_xsdRestriction.IntValue)
                    {
                        throw new ArgumentException("Value[" + p_s_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.IntValue + "]");
                    }
                }
                else
                {
                    throw new ArgumentException("Cannot use " + p_o_xsdRestriction.Name + " restriction on type: " + p_s_type);
                }
            }
            else if (p_o_xsdRestriction.Name.ToLower().Equals("minlength"))
            {
                if (a_stringTypes.Contains(p_s_typeLower))
                {
                    if (p_s_value.Length < p_o_xsdRestriction.IntValue)
                    {
                        throw new ArgumentException("Value[" + p_s_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.IntValue + "]");
                    }
                }
                else
                {
                    throw new ArgumentException("Cannot use " + p_o_xsdRestriction.Name + " restriction on type: " + p_s_type);
                }
            }
            else if (p_o_xsdRestriction.Name.ToLower().Equals("maxlength"))
            {
                if (a_stringTypes.Contains(p_s_typeLower))
                {
                    if (p_s_value.Length > p_o_xsdRestriction.IntValue)
                    {
                        throw new ArgumentException("Value[" + p_s_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.IntValue + "]");
                    }
                }
                else
                {
                    throw new ArgumentException("Cannot use " + p_o_xsdRestriction.Name + " restriction on type: " + p_s_type);
                }
            }
            else if (p_o_xsdRestriction.Name.ToLower().Equals("enumeration"))
            {
                if (p_o_xsdRestriction.StrValue.Equals(p_s_value))
                {
                    b_enumerationReturnValue = true;
                }
            }
            else if (p_o_xsdRestriction.Name.ToLower().Equals("whiteSpace"))
            {
                throw new ArgumentException("Cannot use " + p_o_xsdRestriction.Name + " restriction on type: " + p_s_type);
            }
            else if (p_o_xsdRestriction.Name.ToLower().Equals("pattern"))
            {
                if (!ForestNET.Lib.Helper.MatchesRegex(p_s_value, p_o_xsdRestriction.StrValue))
                {
                    throw new ArgumentException("Value[" + p_s_value + "] does not match " + p_o_xsdRestriction.Name + " restriction[" + p_o_xsdRestriction.StrValue + "]");
                }
            }
            else
            {
                throw new ArgumentException("Unknown Restriction: " + p_o_xsdRestriction.Name);
            }

            return b_enumerationReturnValue;
        }

        /* validate XML data with XSD schema */

        /// <summary>
        /// Validate xml file
        /// </summary>
        /// <param name="p_s_xmlFile">full-path to xml file</param>
        /// <returns>true - content of xml file is valid, false - content of xml file is invalid</returns>
        /// <exception cref="ArgumentException">xml file does not exist</exception>
        /// <exception cref="System.IO.IOException">cannot read xml file content</exception>
        /// <exception cref="ArgumentNullException">empty schema, empty xml file or root node after parsing xml content</exception>
        /// <exception cref="FormatException">exception parsing string to target type</exception>
        public bool ValidateAgainstSchema(string p_s_xmlFile)
        {
            /* check root field */
            if (this.Root == null)
            {
                throw new NullReferenceException("Cannot decode data. Root is null.");
            }

            /* check if file exists */
            if (!File.Exists(p_s_xmlFile))
            {
                throw new ArgumentNullException(nameof(p_s_xmlFile), "XML file[" + p_s_xmlFile + "] does not exist.");
            }

            /* open xml file */
            File o_file = new(p_s_xmlFile, false);

            /* load file content into string */
            string s_fileContent = o_file.FileContent;

            List<string> a_fileLines = [];

            /* load file content lines into array */
            foreach (string s_line in s_fileContent.Split(this.s_lineBreak))
            {
                a_fileLines.Add(s_line);
            }

            ForestNET.Lib.Global.ILogFiner("read all lines from xml file '" + p_s_xmlFile + "'");

            /* validate xml file lines */
            return ValidateAgainstSchema(a_fileLines);
        }

        /// <summary>
        /// Validate xml content
        /// </summary>
        /// <param name="p_a_xmlTags">xml lines</param>
        /// <returns>true - xml content is valid, false - xml content is invalid</returns>
        /// <exception cref="ArgumentException">xml file does not exist</exception>
        /// <exception cref="System.IO.IOException">cannot read xml file content</exception>
        /// <exception cref="ArgumentNullException">empty schema, empty xml file or root node after parsing xml content</exception>
        /// <exception cref="FormatException">exception parsing string to target type</exception>
        public bool ValidateAgainstSchema(List<string> p_a_xmlTags)
        {
            /* check root field */
            if (this.Root == null)
            {
                throw new NullReferenceException("Cannot decode data. Schema is null.");
            }

            ForestNET.Lib.Global.ILogFiner("read all lines: '" + p_a_xmlTags.Count + "'");

            System.Text.StringBuilder o_stringBuilder = new();

            /* read all xml schema file lines to one string builder */
            foreach (string s_line in p_a_xmlTags)
            {
                o_stringBuilder.Append(s_line);
            }

            /* read all xml lines */
            string s_xml = o_stringBuilder.ToString();
            s_xml = new System.Text.RegularExpressions.Regex("[\\r\\n\\t]").Replace(s_xml, "");
            s_xml = new System.Text.RegularExpressions.Regex(">\\s*<").Replace(s_xml, "><");

            /* clean up xml file */
            s_xml = new System.Text.RegularExpressions.Regex("<\\?(.*?)\\?>").Replace(s_xml, "");
            s_xml = new System.Text.RegularExpressions.Regex("<!--(.*?)-->").Replace(s_xml, "");

            ForestNET.Lib.Global.ILogFinest("cleaned up xml file lines");

            /* validate xml */
            System.Text.RegularExpressions.Regex o_regex = new("(<[^<>]*?<[^<>]*?>|<[^<>]*?>[^<>]*?>)");
            System.Text.RegularExpressions.MatchCollection o_matcher = o_regex.Matches(s_xml);

            /* if regex-matcher has match, the xml file is not valid */
            if (o_matcher.Count > 0)
            {
                throw new ArgumentException("Invalid xml-file. Please check xml-file at \"" + o_matcher[0] + "\".");
            }

            List<string> a_xmlTags = [];

            /* add all xml-tags to a list for parsing */
            o_regex = new System.Text.RegularExpressions.Regex("(<[^<>/]*?/>)|(<[^<>/]*?>[^<>]*?</[^<>/]*?>)|(<[^<>/]*?>)|(</[^<>/]*?>)");
            o_matcher = o_regex.Matches(s_xml);

            /* save all xml tags in one array */
            if (o_matcher.Count > 0)
            {
                for (int i = 0; i < o_matcher.Count; i++)
                {
                    string s_xmlTag = o_matcher[i].ToString();

                    if ((!s_xmlTag.StartsWith("<")) && (!s_xmlTag.EndsWith(">")))
                    {
                        throw new ArgumentException("Invalid xml-tag. Please check xml-file at \"" + s_xmlTag + "\".");
                    }

                    a_xmlTags.Add(s_xmlTag);
                }
            }

            /* validate xml tree recursively */
            XML.ValidateXMLDocument(a_xmlTags, 0, a_xmlTags.Count - 1);

            ForestNET.Lib.Global.ILogFinest("validated xml content lines");

            if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("XML-Schema:" + ForestNET.Lib.IO.File.NEWLINE + this.Root);
            if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("XML:" + ForestNET.Lib.IO.File.NEWLINE + s_xml);

            /* validate xml tree recursively */
            return this.ValidateAgainstSchemaRecursive(a_xmlTags, 0, a_xmlTags.Count - 1, this.Root);
        }

        /// <summary>
        /// Recursive method to decode xml string to a c# object and it's fields
        /// </summary>
        /// <param name="p_a_xmlTags">xml lines</param>
        /// <param name="p_i_min">pointer where to start line iteration</param>
        /// <param name="p_i_max">pointer where to end line iteration</param>
        /// <param name="p_o_xsdElement">current xsd element of schema with information to decode xml data</param>
        /// <returns>decoded xml information as c# object</returns>
        /// <exception cref="ArgumentNullException">value in schema or expected element is not available</exception>
        /// <exception cref="ArgumentException">condition failed for decoding xml correctly</exception>
        /// <exception cref="FormatException">exception parsing string to target type</exception>
        private bool ValidateAgainstSchemaRecursive(List<string> p_a_xmlTags, int p_i_min, int p_i_max, XSDElement p_o_xsdElement)
        {
            bool b_return = true;

            /* check xml tag pointer */
            if (p_i_min > p_i_max)
            {
                throw new ArgumentException("Xml tag pointer overflow(" + p_i_min + " >= " + p_i_max + ").");
            }

            /* get xml type */
            XMLType e_xmlType = XML.GetXMLType(p_a_xmlTags[p_i_min]);

            /* variable for xml element name */
            string s_xmlElementName;

            /* get xml element name */
            if ((e_xmlType == XMLType.BeginWithAttributes) || (e_xmlType == XMLType.ElementWithAttributes) || (e_xmlType == XMLType.EmptyWithAttributes))
            {
                s_xmlElementName = p_a_xmlTags[p_i_min].Substring(1, p_a_xmlTags[p_i_min].IndexOf(" ") - 1);
            }
            else if (e_xmlType == XMLType.Empty)
            {
                s_xmlElementName = p_a_xmlTags[p_i_min].Substring(1, p_a_xmlTags[p_i_min].IndexOf("/") - 1);
            }
            else
            {
                s_xmlElementName = p_a_xmlTags[p_i_min].Substring(1, p_a_xmlTags[p_i_min].IndexOf(">") - 1);
            }

            /* check if xml element name is set */
            if (ForestNET.Lib.Helper.IsStringEmpty(s_xmlElementName))
            {
                throw new ArgumentNullException("No xml element name in xml file at(" + (p_i_min + 2) + ".-element) \"" + p_a_xmlTags[p_i_min] + "\".");
            }

            /* check if we expect current xml element */
            if (!s_xmlElementName.Equals(p_o_xsdElement.Name))
            {
                /* maybe it is not the expected root, but just a sub tree of a valid xsd schema, so we look for it in the element definitions */
                foreach (XSDElement o_temp in this.a_elementDefinitons)
                {
                    /* find xml element as element definition of xsd schema */
                    if (s_xmlElementName.Equals(o_temp.Name))
                    {
                        ForestNET.Lib.Global.ILogFiner("xs:element " + p_o_xsdElement.Name + ", is not root element of schema, but a valid sub tree, so take this sub tree as entry point for validation");

                        /* validate xml tree recursively with sub tree of xsd schema */
                        return this.ValidateAgainstSchemaRecursive(p_a_xmlTags, p_i_min, p_i_max, o_temp);
                    }
                }

                throw new ArgumentException(s_xmlElementName + " with type(" + e_xmlType + ") is not expected xs:element " + p_o_xsdElement.Name + " at(" + (p_i_min + 2) + ".-element) \"" + p_a_xmlTags[p_i_min] + "\".");
            }

            /* if we have xs:element definition with no type, exact one child element and mapping contains ":"
			 * we have to iterate a list of object and must print multiple xml elements
			 * otherwise we have usual xs:element definitions for current element
			 */
            if ((ForestNET.Lib.Helper.IsStringEmpty(p_o_xsdElement.Type)) && (p_o_xsdElement.Mapping.Contains(':')) && (p_o_xsdElement.Children.Count == 1))
            {
                /* check if current xml element has interlacing */
                if (!(e_xmlType == XMLType.BeginNoAttributes))
                {
                    /* if list has no children, check minOccurs attribute of xs:element child definition */
                    if (p_o_xsdElement.Children[0].MinOccurs != 0)
                    {
                        throw new ArgumentNullException("List with [" + p_o_xsdElement.Children[0].Name + "] xml tags is empty, minimum = " + p_o_xsdElement.Children[0].MinOccurs);
                    }
                }

                ForestNET.Lib.Global.ILogFiner("xs:element " + p_o_xsdElement.Name + " with no type, exact one child element as definition and list as mapping");

                int i_tempMin = p_i_min + 1;
                int i_level = 0;

                /* look for end of nested xml element tag */
                while ((!XML.GetXMLType(p_a_xmlTags[i_tempMin]).Equals(XMLType.Close)) || (i_level != 0))
                {
                    if ((XML.GetXMLType(p_a_xmlTags[i_tempMin]).Equals(XMLType.BeginNoAttributes)) || (XML.GetXMLType(p_a_xmlTags[i_tempMin]).Equals(XMLType.BeginWithAttributes)))
                    {
                        i_level++;
                    }
                    else if (XML.GetXMLType(p_a_xmlTags[i_tempMin]).Equals(XMLType.Close))
                    {
                        i_level--;
                    }

                    if (i_tempMin >= p_i_max)
                    {
                        /* forbidden state - interlacing is not valid in xml file */
                        throw new ArgumentException("Invalid nested xml element at(" + (p_i_min + 2) + ".-element) \"" + p_a_xmlTags[p_i_min] + "\".");
                    }

                    i_tempMin++;
                }

                ForestNET.Lib.Global.ILogFiner("\tfound interlacing xml element(" + (p_i_min + 2) + " to " + (i_tempMin + 2) + ") - " + p_a_xmlTags[p_i_min] + " ... " + p_a_xmlTags[i_tempMin]);

                /* create occurrence counter */
                int i_occurCnt = 0;

                int i_tempMin2 = p_i_min + 2;

                /* we have to iterate a list of objects, so we must check if there is another nested element */
                /* if the end of nested element not corresponds to end of interlacing, we continue to look for another nested element */
                while (i_tempMin2 < (i_tempMin - 1))
                {
                    i_tempMin2 = p_i_min + 2;
                    i_level = 0;

                    /* look for end of another nested xml element tag */
                    while ((!XML.GetXMLType(p_a_xmlTags[i_tempMin2]).Equals(XMLType.Close)) || (i_level != 0))
                    {
                        if ((XML.GetXMLType(p_a_xmlTags[i_tempMin2]).Equals(XMLType.BeginNoAttributes)) || (XML.GetXMLType(p_a_xmlTags[i_tempMin2]).Equals(XMLType.BeginWithAttributes)))
                        {
                            i_level++;
                        }
                        else if (XML.GetXMLType(p_a_xmlTags[i_tempMin2]).Equals(XMLType.Close))
                        {
                            i_level--;
                        }

                        if (i_tempMin2 >= p_i_max)
                        {
                            /* forbidden state - interlacing is not valid in xml file */
                            throw new ArgumentException("Invalid nested xml element at(" + (i_tempMin + 1 + 2) + ".-element) \"" + p_a_xmlTags[i_tempMin + 1] + "\".");
                        }

                        i_tempMin2++;
                    }

                    ForestNET.Lib.Global.ILogFiner("\t\tfound list xml element(" + (p_i_min + 3) + " to " + (i_tempMin2 + 2) + ") - " + p_a_xmlTags[p_i_min + 1] + " ... " + p_a_xmlTags[i_tempMin2]);

                    /* handle nested xml element recursively */
                    b_return = this.ValidateAgainstSchemaRecursive(p_a_xmlTags, ++p_i_min, i_tempMin, p_o_xsdElement.Children[0]);

                    /* add return object of recursion to list */
                    if (b_return)
                    {
                        if (p_o_xsdElement.Children[0].IsArray)
                        {
                            /* increase occur counter with list size */
                            i_occurCnt += (i_tempMin - p_i_min);
                        }
                        else
                        {
                            /* increase occur counter */
                            i_occurCnt++;
                        }
                    }

                    /* update xml tag pointer */
                    p_i_min = i_tempMin2;
                }

                /* update xml tag pointer */
                p_i_min = i_tempMin;

                ForestNET.Lib.Global.ILogFiner("\tend interlacing, continue from " + (p_i_min + 2) + " to " + (p_i_max + 2));

                /* check minOccurs attribute of child xs:element of current list */
                if (p_o_xsdElement.Children[0].MinOccurs > i_occurCnt)
                {
                    throw new ArgumentException("Not enough [" + p_o_xsdElement.Children[0].Name + "] xml tags, minimum = " + p_o_xsdElement.Children[0].MinOccurs);
                }

                /* check maxOccurs attribute of child xs:element of current list */
                if ((p_o_xsdElement.Children[0].MaxOccurs >= 0) && (i_occurCnt > p_o_xsdElement.Children[0].MaxOccurs))
                {
                    throw new ArgumentException("Too many [" + p_o_xsdElement.Children[0].Name + "] xml tags, maximum = " + p_o_xsdElement.Children[0].MaxOccurs);
                }
            }
            else if (p_o_xsdElement.Name.Equals(p_o_xsdElement.Type))
            { /* handle array elements as xml element tags */
                /* create occurrence counter */
                int i_occurCnt = 0;

                for (int i = p_i_min; i < p_i_max; i++)
                {
                    /* get value if xml element is not empty */
                    if (!((e_xmlType == XMLType.Empty) || (e_xmlType == XMLType.EmptyWithAttributes)))
                    {
                        /* get value of xml element */
                        System.Text.RegularExpressions.Regex o_regex = new("(<" + p_o_xsdElement.Name + ">)([^<>]*?)(</" + p_o_xsdElement.Name + ">)");
                        string[] o_matcher = o_regex.Split(p_a_xmlTags[i]);

                        if (o_matcher.Length > 2)
                        {
                            string s_elementValue = o_matcher[2];

                            /* check if xs:element has any restrictions */
                            if (p_o_xsdElement.Restriction)
                            {
                                bool b_enumerationFound = false;
                                bool b_enumerationReturnValue = false;

                                foreach (XSDRestriction o_xsdRestriction in p_o_xsdElement.Restrictions)
                                {
                                    if (o_xsdRestriction.Name.ToLower().Equals("enumeration"))
                                    {
                                        b_enumerationFound = true;
                                    }

                                    b_enumerationReturnValue = this.CheckRestriction(s_elementValue, o_xsdRestriction, p_o_xsdElement.Type);

                                    if ((b_enumerationFound) && (b_enumerationReturnValue))
                                    {
                                        break;
                                    }
                                }

                                if ((b_enumerationFound) && (!b_enumerationReturnValue))
                                {
                                    throw new ArgumentException("Element[" + p_o_xsdElement.Name + "] with value[" + s_elementValue + "] does not match enumaration restrictions defined in xsd-schema for this xs:element");
                                }
                            }

                            /* check if element has value and if element value is not empty string value */
                            if ((!ForestNET.Lib.Helper.IsStringEmpty(s_elementValue)) && (!((this.b_printEmptystring) && (s_elementValue.ToString().Equals("&#x200B;")))))
                            {
                                /* cast string value to object and set returned value flag */
                                _ = this.CastObjectFromString(s_elementValue, p_o_xsdElement.Type);
                                i_occurCnt++;
                            }
                        }
                    }

                    ForestNET.Lib.Global.ILogFinest("\t\t\t\t\tarray element[" + p_o_xsdElement.Name + "]; list size=" + i_occurCnt);
                }

                /* check minOccurs attribute of xs:element */
                if (p_o_xsdElement.MinOccurs > i_occurCnt)
                {
                    throw new ArgumentException("Not enough [" + p_o_xsdElement.Name + "] xml tags, minimum = " + p_o_xsdElement.MinOccurs);
                }

                /* check maxOccurs attribute of xs:element */
                if ((p_o_xsdElement.MaxOccurs >= 0) && (i_occurCnt > p_o_xsdElement.MaxOccurs))
                {
                    throw new ArgumentException("Too many [" + p_o_xsdElement.Name + "] xml tags, maximum = " + p_o_xsdElement.MaxOccurs);
                }
            }
            else
            {
                /* flag if current element returned attributes */
                bool b_returnedAttributes = false;

                /* check if current xml element should have attributes */
                if (p_o_xsdElement.Attributes.Count > 0)
                {
                    bool b_required = false;

                    /* check if one attribute of the xml element is required */
                    foreach (XSDAttribute o_xsdAttribute in p_o_xsdElement.Attributes)
                    {
                        if (o_xsdAttribute.Required)
                        {
                            b_required = true;
                        }
                    }

                    /* check if current xml element has attributes */
                    if (((e_xmlType != XMLType.BeginWithAttributes) && (e_xmlType != XMLType.ElementWithAttributes) && (e_xmlType != XMLType.EmptyWithAttributes)) && (b_required))
                    {
                        throw new ArgumentException(s_xmlElementName + " has no attributes and is not compatible with xs:element (" + p_o_xsdElement.Name + "[" + p_o_xsdElement.Type + "])");
                    }

                    /* retrieve attributes */
                    b_returnedAttributes = this.ParseXMLAttributesForValidation(p_a_xmlTags, p_i_min, p_o_xsdElement);
                }

                /* create choice counter */
                int i_choiceCnt = 0;

                /* iterate all children of current xs:element */
                foreach (XSDElement o_xsdElement in p_o_xsdElement.Children)
                {
                    ForestNET.Lib.Global.ILogFinest("\t\t\titerate children of " + p_o_xsdElement.Name + ", choice=" + p_o_xsdElement.Choice + " ... child[" + o_xsdElement.Name + "] with type[" + o_xsdElement.Type + "] and attributes count[" + o_xsdElement.Attributes.Count + "]");

                    /* if xs:element has no primitive type we may have a special object definition */
                    if (ForestNET.Lib.Helper.IsStringEmpty(o_xsdElement.Type))
                    {
                        /* increase xml tag pointer */
                        p_i_min++;

                        ForestNET.Lib.Global.ILogFinest("\t\t\t\tchild[" + o_xsdElement.Name + "] compare to current xml tag[" + p_a_xmlTags[p_i_min] + "]");

                        /* xml tag must match with expected xs:element name */
                        if (!p_a_xmlTags[p_i_min].StartsWith("<" + o_xsdElement.Name))
                        {
                            /* if we have a choice scope or child min. occurs is lower than 1, go to next xs:element */
                            if ((p_o_xsdElement.Choice) || (o_xsdElement.MinOccurs < 1))
                            {
                                p_i_min--;
                                continue;
                            }

                            /* xml tag does not match with xs:element name */
                            throw new ArgumentException(p_a_xmlTags[p_i_min] + " is not expected xs:element " + o_xsdElement.Name + " for recursion");
                        }

                        /* check choice counter */
                        if ((p_o_xsdElement.Choice) && (++i_choiceCnt > p_o_xsdElement.MaxOccurs))
                        {
                            throw new ArgumentException(p_o_xsdElement.Type + " has to many objects(" + i_choiceCnt + ") in xs:choice " + p_o_xsdElement.Name + "(" + p_o_xsdElement.Type + "), maximum = " + p_o_xsdElement.MaxOccurs);
                        }

                        int i_tempMin = p_i_min + 1;
                        int i_level = 0;

                        ForestNET.Lib.Global.ILogFinest(p_i_min + "\t\t\tlook for recursion borders for [" + o_xsdElement.Name + "] from " + (i_tempMin + 1));

                        /* look for end of nested xml element tag */
                        while ((!XML.GetXMLType(p_a_xmlTags[i_tempMin]).Equals(XMLType.Close)) || (i_level != 0))
                        {
                            ForestNET.Lib.Global.ILogFinest("\t\t\t" + i_level + "\t" + p_a_xmlTags[i_tempMin]);

                            if ((XML.GetXMLType(p_a_xmlTags[i_tempMin]).Equals(XMLType.BeginNoAttributes)) || (XML.GetXMLType(p_a_xmlTags[i_tempMin]).Equals(XMLType.BeginWithAttributes)))
                            {
                                i_level++;
                            }
                            else if (XML.GetXMLType(p_a_xmlTags[i_tempMin]).Equals(XMLType.Close))
                            {
                                i_level--;
                            }

                            if (i_tempMin >= p_i_max)
                            {
                                /* forbidden state - interlacing is not valid in xml file */
                                throw new ArgumentException("Invalid nested xml element at(" + (p_i_min + 2) + ".-element) \"" + p_a_xmlTags[p_i_min] + "\".");
                            }

                            i_tempMin++;
                        }

                        ForestNET.Lib.Global.ILogFinest("\t\t\t" + o_xsdElement.Name + " has no primitive type -> new recursion (" + (p_i_min + 2) + " to " + (i_tempMin + 2) + ") - " + p_a_xmlTags[p_i_min] + " ... " + p_a_xmlTags[i_tempMin]);

                        /* handle xml element recursively */
                        b_return = this.ValidateAgainstSchemaRecursive(p_a_xmlTags, p_i_min, i_tempMin, o_xsdElement);

                        p_i_min = i_tempMin;

                        ForestNET.Lib.Global.ILogFinest("\t\t\tend recursion with [" + o_xsdElement.Type + "], continue " + (p_i_min + 2) + " to " + (p_i_max + 2));
                    }
                    else
                    { /* otherwise we have xs:elements with primitive types */
                        /* flag if current element returned value */
                        bool b_returnedValue = false;

                        /* get value if xml element is not empty */
                        if (!((e_xmlType == XMLType.Empty) || (e_xmlType == XMLType.EmptyWithAttributes)))
                        {
                            /* get value for xml element */
                            if (b_returnedAttributes)
                            {
                                b_returnedValue = this.ParseXMLElementForValidation(p_a_xmlTags, ++p_i_min, o_xsdElement, "(<" + o_xsdElement.Name + "[^<>]*?>)([^<>]*?)(</" + o_xsdElement.Name + ">)");
                            }
                            else
                            {
                                b_returnedValue = this.ParseXMLElementForValidation(p_a_xmlTags, ++p_i_min, o_xsdElement, "(<" + o_xsdElement.Name + ">)([^<>]*?)(</" + o_xsdElement.Name + ">)");
                            }
                        }

                        ForestNET.Lib.Global.ILogFinest("\t\t\t\tchild[" + o_xsdElement.Name + "] returned value=" + b_returnedValue);

                        /* check minOccurs attribute of xml element and if value is empty */
                        if ((o_xsdElement.MinOccurs > 0) && (!b_returnedValue))
                        {
                            if (p_o_xsdElement.Choice)
                            {
                                p_i_min--;
                                continue;
                            }
                            else
                            {
                                throw new ArgumentNullException("Missing element value for xs:element[" + o_xsdElement.Name + "]{" + o_xsdElement.Type + "}");
                            }
                        }

                        /* check choice counter */
                        if (p_o_xsdElement.Choice)
                        {
                            if ((++i_choiceCnt > p_o_xsdElement.MaxOccurs) && (b_returnedValue))
                            {
                                throw new ArgumentException(p_o_xsdElement.Name + " has to many objects(" + i_choiceCnt + ") in xs:choice " + p_o_xsdElement.Type + "." + o_xsdElement.Name + ", maximum = " + p_o_xsdElement.MaxOccurs);
                            }
                        }

                        /* check if child xml element has attributes, because of possible simpleContent */
                        if (o_xsdElement.Attributes.Count > 0)
                        {
                            bool b_required = false;

                            /* check if one attribute of the xml element is required */
                            foreach (XSDAttribute o_xsdAttribute in p_o_xsdElement.Attributes)
                            {
                                if (o_xsdAttribute.Required)
                                {
                                    b_required = true;
                                }
                            }

                            /* check if current xml element has attributes */
                            if (((e_xmlType != XMLType.BeginWithAttributes) && (e_xmlType != XMLType.ElementWithAttributes) && (e_xmlType != XMLType.EmptyWithAttributes)) && (b_required))
                            {
                                throw new ArgumentException(o_xsdElement.Name + " has no attributes");
                            }

                            /* retrieve attributes */
                            b_returnedAttributes = this.ParseXMLAttributesForValidation(p_a_xmlTags, p_i_min, o_xsdElement);
                        }
                    }
                }

                /* check choice counter for minimum objects */
                if (p_o_xsdElement.Choice)
                {
                    if (i_choiceCnt < p_o_xsdElement.MinOccurs)
                    {
                        throw new ArgumentException(p_o_xsdElement.Name + " has to few objects(" + i_choiceCnt + ") in xs:choice, minimum = " + p_o_xsdElement.MinOccurs);
                    }
                }
            }

            ForestNET.Lib.Global.ILogFiner("\treturn " + b_return);
            return b_return;
        }

        /// <summary>
        /// Parse xml element value for validation
        /// </summary>
        /// <param name="p_a_xmlTags">xml lines</param>
        /// <param name="p_i_min">pointer where to start line iteration</param>
        /// <param name="p_o_xsdElement">current xml schema element</param>
        /// <param name="p_s_regexPattern">regex pattern where xml line must match</param>
        /// <returns>true - xml element has a value, false - could not parse value for xml element</returns>
        /// <exception cref="ArgumentException">element with value does not match structure or restrictions</exception>
        /// <exception cref="FormatException">exception parsing string to target type</exception>
        private bool ParseXMLElementForValidation(List<string> p_a_xmlTags, int p_i_min, XSDElement p_o_xsdElement, string p_s_regexPattern)
        {
            /* return value */
            bool b_hasValue = false;

            /* get value of xml element */
            System.Text.RegularExpressions.Regex o_regex = new(p_s_regexPattern);
            string[] o_matcher = o_regex.Split(p_a_xmlTags[p_i_min]);

            if (o_matcher.Length > 2)
            {
                string s_elementValue = o_matcher[2];

                /* check if element has value */
                if (!ForestNET.Lib.Helper.IsStringEmpty(s_elementValue))
                {
                    b_hasValue = true;
                }

                /* check if xs:element has any restrictions */
                if (p_o_xsdElement.Restriction)
                {
                    bool b_enumerationFound = false;
                    bool b_enumerationReturnValue = false;

                    foreach (XSDRestriction o_xsdRestriction in p_o_xsdElement.Restrictions)
                    {
                        if (o_xsdRestriction.Name.ToLower().Equals("enumeration"))
                        {
                            b_enumerationFound = true;
                        }

                        b_enumerationReturnValue = this.CheckRestriction(s_elementValue, o_xsdRestriction, p_o_xsdElement.Type);

                        if ((b_enumerationFound) && (b_enumerationReturnValue))
                        {
                            break;
                        }
                    }

                    if ((b_enumerationFound) && (!b_enumerationReturnValue))
                    {
                        throw new ArgumentException("Element[" + p_o_xsdElement.Name + "] with value[" + s_elementValue + "] does not match enumaration restrictions defined in xsd-schema for this xs:element");
                    }
                }

                _ = this.CastObjectFromStringForValidation(s_elementValue, p_o_xsdElement.Type);
            }

            return b_hasValue;
        }

        /// <summary>
        /// Parse xml attribute value for validation
        /// </summary>
        /// <param name="p_a_xmlTags">xml lines</param>
        /// <param name="p_i_min">pointer where to start line iteration</param>
        /// <param name="p_o_xsdElement">current xml schema element</param>
        /// <returns>true - xml attribute has a value, false - could not parse value for xml attribute</returns>
        /// <exception cref="ArgumentException">element with value does not match structure or restrictions</exception>
        /// <exception cref="FormatException">exception parsing string to target type</exception>
        private bool ParseXMLAttributesForValidation(List<string> p_a_xmlTags, int p_i_min, XSDElement p_o_xsdElement)
        {
            /* return value */
            bool b_hasAttributes = false;

            /* get attributes of xml element */
            System.Text.RegularExpressions.Regex o_regex = new("[^<>\\s/=]*?=\"[^<>/\"]*?\"");
            System.Text.RegularExpressions.MatchCollection o_matcher = o_regex.Matches(p_a_xmlTags[p_i_min]);

            if (o_matcher.Count > 0)
            {
                for (int i = 0; i < o_matcher.Count; i++)
                {
                    string s_attribute = o_matcher[i].ToString();
                    string s_attributeName = s_attribute.Substring(0, s_attribute.IndexOf("="));
                    bool b_found = false;

                    foreach (XSDAttribute o_xsdAttribute in p_o_xsdElement.Attributes)
                    {
                        if (s_attributeName.Equals(o_xsdAttribute.Name))
                        {
                            string s_attributeValue = s_attribute.Substring(s_attribute.IndexOf("=") + 1);
                            s_attributeValue = s_attributeValue.Substring(1, s_attributeValue.Length - 1 - 1);
                            ForestNET.Lib.Global.ILogFiner("\t\t\t\tfound Attribute [" + s_attributeName + "] with value=" + s_attributeValue);

                            /* if attribute is required but value is empty, throw exception */
                            if ((o_xsdAttribute.Required) && (ForestNET.Lib.Helper.IsStringEmpty(s_attributeValue)))
                            {
                                throw new ArgumentException("Missing attribute value for xs:attribute[" + o_xsdAttribute.Name + "." + o_xsdAttribute.Type + "]");
                            }

                            /* check if xs:element has any restrictions */
                            if (o_xsdAttribute.Restriction)
                            {
                                bool b_enumerationFound = false;
                                bool b_enumerationReturnValue = false;

                                foreach (XSDRestriction o_xsdRestriction in o_xsdAttribute.Restrictions)
                                {
                                    if (o_xsdRestriction.Name.ToLower().Equals("enumeration"))
                                    {
                                        b_enumerationFound = true;
                                    }

                                    b_enumerationReturnValue = this.CheckRestriction(s_attributeValue, o_xsdRestriction, o_xsdAttribute.Type);

                                    if ((b_enumerationFound) && (b_enumerationReturnValue))
                                    {
                                        break;
                                    }
                                }

                                if ((b_enumerationFound) && (!b_enumerationReturnValue))
                                {
                                    throw new ArgumentException("Attribute[" + o_xsdAttribute.Name + "] with value[" + s_attributeValue + "] does not match enumaration restrictions defined in xsd-schema for this xs:attribute");
                                }
                            }

                            Object o_foo = this.CastObjectFromStringForValidation(s_attributeValue, o_xsdAttribute.Type) ?? throw new ArgumentException("Attribute value '" + s_attributeValue + "' with type '" + o_xsdAttribute.Type + "' retunred null for validation");

                            b_found = true;
                            b_hasAttributes = true;
                        }
                    }

                    if (!b_found)
                    {
                        throw new ArgumentException("Xml attribute[" + s_attributeName + "] not expected and not availalbe in xsd-schema at(" + (p_i_min + 2) + ".-element) \"" + p_a_xmlTags[p_i_min] + "\".");
                    }
                }
            }

            return b_hasAttributes;
        }

        /// <summary>
        /// Convert a string value from a xml element to an object to decode it into an object
        /// </summary>
        /// <param name="p_s_value">string value of xml element from file</param>
        /// <param name="p_s_type">type of destination object field, conform to xsd schema</param>
        /// <returns>casted object value from string</returns>
        /// <exception cref="ArgumentException">invalid type parameter</exception>
        /// <exception cref="FormatException">exception parsing string to target type</exception>
        private Object? CastObjectFromStringForValidation(string? p_s_value, string? p_s_type)
        {
            Object? o_foo = null;

            /* return null if value or type parameter is null */
            if ((p_s_value == null) || (p_s_type == null))
            {
                return o_foo;
            }

            if (!ForestNET.Lib.Helper.IsStringEmpty(p_s_value))
            {
                p_s_type = p_s_type.ToLower();

                List<string> a_stringTypes = ["string", "duration", "hexbinary", "base64binary", "anyuri", "normalizedstring", "token", "language", "name", "ncname", "nmtoken", "id", "idref", "entity"];
                List<string> a_integerTypes = ["integer", "int", "positiveinteger", "nonpositiveinteger", "negativeinteger", "nonnegativeinteger", "unsignedint", "unsignedinteger"];

                if (p_s_type.Equals("boolean"))
                {
                    o_foo = Convert.ToBoolean(p_s_value);
                }
                else if (a_stringTypes.Contains(p_s_type))
                {
                    if ((p_s_value.Equals("&#x200B;")) && (this.b_printEmptystring))
                    {
                        p_s_value = "";
                    }

                    o_foo = p_s_value;
                }
                else if (p_s_type.Equals("datetime"))
                {
                    if (!ForestNET.Lib.Helper.IsDateTime(p_s_value))
                    {
                        throw new ArgumentException("Illegal value '" + p_s_value + "' for 'datetime'");
                    }

                    if (this.b_useISO8601UTC)
                    {
                        o_foo = ForestNET.Lib.Helper.FromISO8601UTC(p_s_value);
                    }
                    else
                    {
                        o_foo = ForestNET.Lib.Helper.FromDateTimeString(p_s_value);
                    }
                }
                else if (p_s_type.Equals("date"))
                {
                    if (this.b_useISO8601UTC)
                    {
                        if (!ForestNET.Lib.Helper.IsDateTime(p_s_value))
                        {
                            throw new ArgumentException("Illegal value '" + p_s_value + "' for 'date'");
                        }

                        o_foo = ForestNET.Lib.Helper.FromISO8601UTC(p_s_value).Date;
                    }
                    else
                    {
                        if (!ForestNET.Lib.Helper.IsDate(p_s_value))
                        {
                            throw new ArgumentException("Illegal value '" + p_s_value + "' for 'date'");
                        }

                        o_foo = ForestNET.Lib.Helper.FromDateString(p_s_value);
                    }
                }
                else if (p_s_type.Equals("time"))
                {
                    if (this.b_useISO8601UTC)
                    {
                        if (!ForestNET.Lib.Helper.IsDateTime(p_s_value))
                        {
                            throw new ArgumentException("Illegal value '" + p_s_value + "' for 'time'");
                        }

                        o_foo = ForestNET.Lib.Helper.FromISO8601UTC(p_s_value).TimeOfDay;
                    }
                    else
                    {
                        if (!ForestNET.Lib.Helper.IsTime(p_s_value))
                        {
                            throw new ArgumentException("Illegal value '" + p_s_value + "' for 'time'");
                        }

                        o_foo = ForestNET.Lib.Helper.FromTimeString(p_s_value);
                    }
                }
                else if (p_s_type.Equals("decimal"))
                {
                    o_foo = Convert.ToDecimal(p_s_value, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                }
                else if (p_s_type.Equals("double"))
                {
                    o_foo = Convert.ToDouble(p_s_value, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                }
                else if (p_s_type.Equals("float"))
                {
                    o_foo = Convert.ToSingle(p_s_value, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                }
                else if (p_s_type.Equals("sbyte"))
                {
                    o_foo = Convert.ToSByte(p_s_value);
                }
                else if (p_s_type.Equals("byte"))
                {
                    o_foo = Convert.ToByte(p_s_value);
                }
                else if (p_s_type.Equals("unsignedbyte"))
                {
                    o_foo = Convert.ToByte(p_s_value);
                }
                else if (p_s_type.Equals("short"))
                {
                    o_foo = Convert.ToInt16(p_s_value);
                }
                else if (p_s_type.Equals("unsignedshort"))
                {
                    o_foo = Convert.ToUInt16(p_s_value);
                }
                else if (p_s_type.Equals("long"))
                {
                    o_foo = Convert.ToInt64(p_s_value);
                }
                else if (p_s_type.Equals("unsignedlong"))
                {
                    o_foo = Convert.ToUInt64(p_s_value);
                }
                else if (a_integerTypes.Contains(p_s_type))
                {
                    if ((p_s_type.Equals("unsignedint")) || (p_s_type.Equals("unsignedinteger")))
                    {
                        o_foo = Convert.ToUInt32(p_s_value);
                    }
                    else
                    {
                        o_foo = Convert.ToInt32(p_s_value);
                    }
                }
                else if (p_s_type.Equals("long") || p_s_type.Equals("unsignedlong"))
                {
                    o_foo = Convert.ToInt64(p_s_value);
                }
                else if (p_s_type.Equals("short") || p_s_type.Equals("unsignedshort"))
                {
                    o_foo = Convert.ToInt16(p_s_value);
                }
                else
                {
                    throw new ArgumentException("Invalid type[" + p_s_type + "] for " + p_s_value);
                }
            }

            return o_foo;
        }

        /* decoding XML to data with XSD schema */

        /// <summary>
        /// Decode xml file to an c# object
        /// </summary>
        /// <param name="p_s_xmlFile">full-path to xml file</param>
        /// <returns>xml decoded c# object</returns>
        /// <exception cref="ArgumentException">xml file does not exist</exception>
        /// <exception cref="System.IO.IOException">cannot read xml file content</exception>
        /// <exception cref="ArgumentNullException">empty schema, empty xml file or root node after parsing xml content</exception>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        /// <exception cref="FormatException">exception parsing string to target type</exception>
        public Object? XmlDecode(string p_s_xmlFile)
        {
            /* check root field */
            if (this.Root == null)
            {
                throw new NullReferenceException("Cannot decode data. Root is null.");
            }

            /* check if file exists */
            if (!File.Exists(p_s_xmlFile))
            {
                throw new ArgumentNullException(nameof(p_s_xmlFile), "XML file[" + p_s_xmlFile + "] does not exist.");
            }

            /* open xml file */
            File o_file = new(p_s_xmlFile, false);

            /* load file content into string */
            string s_fileContent = o_file.FileContent;

            List<string> a_fileLines =
            [
                /* load file content lines into array */
                .. s_fileContent.Split(this.s_lineBreak),
            ];

            ForestNET.Lib.Global.ILogFiner("read all lines from xml file '" + p_s_xmlFile + "'");

            /* decode xml file lines */
            return XmlDecode(a_fileLines);
        }

        /// <summary>
        /// Decode xml file to an c# object
        /// </summary>
        /// <param name="p_a_xmlTags">xml lines</param>
        /// <returns>xml decoded c# object</returns>
        /// <exception cref="ArgumentException">xml file does not exist</exception>
        /// <exception cref="System.IO.IOException">cannot read xml file content</exception>
        /// <exception cref="ArgumentNullException">empty schema, empty xml file or root node after parsing xml content</exception>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        /// <exception cref="FormatException">exception parsing string to target type</exception>
        public Object? XmlDecode(List<string> p_a_xmlTags)
        {
            Object? o_foo = null;

            /* check root field */
            if (this.Root == null)
            {
                throw new NullReferenceException("Cannot decode data. Schema is null.");
            }

            ForestNET.Lib.Global.ILogFiner("read all lines: '" + p_a_xmlTags.Count + "'");

            System.Text.StringBuilder o_stringBuilder = new();

            /* read all xml schema file lines to one string builder */
            foreach (string s_line in p_a_xmlTags)
            {
                o_stringBuilder.Append(s_line);
            }

            /* read all xml lines */
            string s_xml = o_stringBuilder.ToString();
            s_xml = new System.Text.RegularExpressions.Regex("[\\r\\n\\t]").Replace(s_xml, "");
            s_xml = new System.Text.RegularExpressions.Regex(">\\s*<").Replace(s_xml, "><");

            /* clean up xml file */
            s_xml = new System.Text.RegularExpressions.Regex("<\\?(.*?)\\?>").Replace(s_xml, "");
            s_xml = new System.Text.RegularExpressions.Regex("<!--(.*?)-->").Replace(s_xml, "");

            ForestNET.Lib.Global.ILogFinest("cleaned up xml file lines");

            /* validate xml */
            System.Text.RegularExpressions.Regex o_regex = new("(<[^<>]*?<[^<>]*?>|<[^<>]*?>[^<>]*?>)");
            System.Text.RegularExpressions.MatchCollection o_matcher = o_regex.Matches(s_xml);

            /* if regex-matcher has match, the xml file is not valid */
            while (o_matcher.Count > 0)
            {
                throw new ArgumentException("Invalid xml-file. Please check xml-file at \"" + o_matcher[0] + "\".");
            }

            List<string> a_xmlTags = [];

            /* add all xml-tags to a list for parsing */
            o_regex = new System.Text.RegularExpressions.Regex("(<[^<>/]*?/>)|(<[^<>/]*?>[^<>]*?</[^<>/]*?>)|(<[^<>/]*?>)|(</[^<>/]*?>)");
            o_matcher = o_regex.Matches(s_xml);

            /* save all xml tags in one array */
            if (o_matcher.Count > 0)
            {
                for (int i = 0; i < o_matcher.Count; i++)
                {
                    string s_xmlTag = o_matcher[i].ToString();

                    if ((!s_xmlTag.StartsWith("<")) && (!s_xmlTag.EndsWith(">")))
                    {
                        throw new ArgumentException("Invalid xml-tag. Please check xml-file at \"" + s_xmlTag + "\".");
                    }

                    a_xmlTags.Add(s_xmlTag);
                }
            }

            /* validate xml tree recursively */
            XML.ValidateXMLDocument(a_xmlTags, 0, a_xmlTags.Count - 1);

            ForestNET.Lib.Global.ILogFinest("validated xml content lines");

            if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("XML-Schema:" + ForestNET.Lib.IO.File.NEWLINE + this.Root);
            if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("XML:" + ForestNET.Lib.IO.File.NEWLINE + s_xml);

            /* decode xml tree recursively */
            o_foo = this.XmlDecodeRecursive(a_xmlTags, 0, a_xmlTags.Count - 1, o_foo, this.Root);

            ForestNET.Lib.Global.ILogFinest("decoded xml content lines");

            return o_foo;
        }

        /// <summary>
        /// Check if given xml file or xml lines are valid
        /// </summary>
        /// <param name="p_a_xmlTags">xml lines</param>
        /// <param name="p_i_min">pointer where to start line iteration</param>
        /// <param name="p_i_max">pointer where to end line iteration</param>
        /// <exception cref="ArgumentNullException">value in schema or expected element is not available</exception>
        /// <exception cref="ArgumentException">validation failed for decoding xml document correctly, maybe type is not valid</exception>
        private static void ValidateXMLDocument(List<string> p_a_xmlTags, int p_i_min, int p_i_max)
        {
            bool b_parsed = false;

            /* iterate all elements */
            for (int i_min = p_i_min; i_min <= p_i_max; i_min++)
            {
                /* get xml type */
                XMLType e_xmlType = XML.GetXMLType(p_a_xmlTags[i_min]);

                /* variable for xml element name */
                string s_xmlElementName;

                /* get xml element name */
                if ((e_xmlType == XMLType.BeginWithAttributes) || (e_xmlType == XMLType.ElementWithAttributes) || (e_xmlType == XMLType.EmptyWithAttributes))
                {
                    s_xmlElementName = p_a_xmlTags[i_min].Substring(1, p_a_xmlTags[i_min].IndexOf(" ") - 1);
                }
                else if (e_xmlType == XMLType.Empty)
                {
                    s_xmlElementName = p_a_xmlTags[i_min].Substring(1, p_a_xmlTags[i_min].IndexOf("/") - 1);
                }
                else
                {
                    s_xmlElementName = p_a_xmlTags[i_min].Substring(1, p_a_xmlTags[i_min].IndexOf(">") - 1);
                }

                /* check if xml element name is set */
                if (ForestNET.Lib.Helper.IsStringEmpty(s_xmlElementName))
                {
                    throw new ArgumentNullException("No xml element name in xml file at(" + (i_min + 2) + ".-element) \"" + p_a_xmlTags[i_min] + "\".");
                }

                /* we found a xml element with interlacing */
                if ((e_xmlType == XMLType.BeginNoAttributes) || (e_xmlType == XMLType.BeginWithAttributes))
                {
                    /* if we have another xs:element with interlacing we may need another recursion, if it was already parsed */
                    if (b_parsed)
                    {
                        int i_oldMax = p_i_max;
                        int i_tempMin = i_min + 1;
                        int i_level = 0;

                        /* look for end of nested xml element tag */
                        while ((!XML.GetXMLType(p_a_xmlTags[i_tempMin]).Equals(XMLType.Close)) || (i_level != 0))
                        {
                            if ((XML.GetXMLType(p_a_xmlTags[i_tempMin]).Equals(XMLType.BeginNoAttributes)) || (XML.GetXMLType(p_a_xmlTags[i_tempMin]).Equals(XMLType.BeginWithAttributes)))
                            {
                                i_level++;
                            }
                            else if (XML.GetXMLType(p_a_xmlTags[i_tempMin]).Equals(XMLType.Close))
                            {
                                i_level--;
                            }

                            if (i_tempMin == p_i_max)
                            {
                                /* forbidden state - interlacing is not valid in xml file */
                                throw new ArgumentException("Invalid nested xml element at(" + (i_min + 2) + ".-element) \"" + p_a_xmlTags[i_min] + "\".");
                            }

                            i_tempMin++;
                        }

                        ForestNET.Lib.Global.ILogFiner("found interlacing xml element(" + (i_min + 2) + ") - " + p_a_xmlTags[i_min]);

                        /* validate interlacing recursive */
                        XML.ValidateXMLDocument(p_a_xmlTags, i_min, i_tempMin);

                        i_min = i_tempMin;
                        p_i_max = i_oldMax;
                        continue;
                    }

                    /* if we have no closing tag, then our xml file is invalid */
                    if (!XML.GetXMLType(p_a_xmlTags[p_i_max]).Equals(XMLType.Close))
                    {
                        throw new ArgumentException("Invalid xml element is not closed in xml file at(" + (i_min + 2) + ".-element) \"" + p_a_xmlTags[i_min] + "\".");
                    }

                    /* get xml end type */
                    _ = XML.GetXMLType(p_a_xmlTags[p_i_max]);

                    /* get xml close element name */
                    string s_xmlEndElementName = p_a_xmlTags[p_i_max].Substring(2, p_a_xmlTags[p_i_max].IndexOf(">") - 2);

                    /* xml element name and xml close element name must match */
                    if (!s_xmlElementName.Equals(s_xmlEndElementName))
                    {
                        throw new ArgumentException("Xml-tag has no valid closing tag in xml file at(" + (i_min + 2) + ".-element) \"" + p_a_xmlTags[i_min] + "\".");
                    }
                    else
                    {
                        p_i_max--;
                    }
                }

                b_parsed = true;
            }
        }

        /// <summary>
        /// Recursive method to decode xml string to a c# object and it's fields
        /// </summary>
        /// <param name="p_a_xmlTags">xml lines</param>
        /// <param name="p_i_min">pointer where to start line iteration</param>
        /// <param name="p_i_max">pointer where to end line iteration</param>
        /// <param name="p_o_object">destination c# object to decode xml information</param>
        /// <param name="p_o_xsdElement">current xsd element of schema with information to decode xml data</param>
        /// <returns>decoded xml information as c# object</returns>
        /// <exception cref="ArgumentNullException">value in schema or expected element is not available</exception>
        /// <exception cref="ArgumentException">condition failed for decoding xml correctly</exception>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        /// <exception cref="FormatException">exception parsing string to target type</exception>
        private Object? XmlDecodeRecursive(List<string> p_a_xmlTags, int p_i_min, int p_i_max, Object? p_o_object, XSDElement p_o_xsdElement)
        {
            /* check xml tag pointer */
            if (p_i_min > p_i_max)
            {
                throw new ArgumentException("Xml tag pointer overflow(" + p_i_min + " >= " + p_i_max + ").");
            }

            /* get xml type */
            XMLType e_xmlType = XML.GetXMLType(p_a_xmlTags[p_i_min]);

            /* variable for xml element name */
            string s_xmlElementName;

            /* get xml element name */
            if ((e_xmlType == XMLType.BeginWithAttributes) || (e_xmlType == XMLType.ElementWithAttributes) || (e_xmlType == XMLType.EmptyWithAttributes))
            {
                s_xmlElementName = p_a_xmlTags[p_i_min].Substring(1, p_a_xmlTags[p_i_min].IndexOf(" ") - 1);
            }
            else if (e_xmlType == XMLType.Empty)
            {
                s_xmlElementName = p_a_xmlTags[p_i_min].Substring(1, p_a_xmlTags[p_i_min].IndexOf("/") - 1);
            }
            else
            {
                s_xmlElementName = p_a_xmlTags[p_i_min].Substring(1, p_a_xmlTags[p_i_min].IndexOf(">") - 1);
            }

            /* check if xml element name is set */
            if (ForestNET.Lib.Helper.IsStringEmpty(s_xmlElementName))
            {
                throw new ArgumentNullException("No xml element name in xml file at(" + (p_i_min + 2) + ".-element) \"" + p_a_xmlTags[p_i_min] + "\".");
            }

            /* check if we expect current xml element */
            if (!s_xmlElementName.Equals(p_o_xsdElement.Name))
            {
                /* maybe it is not the expected root, but just a sub tree of a valid xsd schema, so we look for it in the element definitions */
                foreach (XSDElement o_temp in this.a_elementDefinitons)
                {
                    /* find xml element as element definition of xsd schema */
                    if (s_xmlElementName.Equals(o_temp.Name))
                    {
                        ForestNET.Lib.Global.ILogFiner("xs:element " + p_o_xsdElement.Name + ", is not root element of schema, but a valid sub tree, so take this sub tree as entry point for decoding");

                        /* decode xml recursively with sub tree of xsd schema */
                        return this.XmlDecodeRecursive(p_a_xmlTags, p_i_min, p_i_max, p_o_object, o_temp);
                    }
                }

                throw new ArgumentException(s_xmlElementName + " with type(" + e_xmlType + ") is not expected xs:element " + p_o_xsdElement.Name + " at(" + (p_i_min + 2) + ".-element) \"" + p_a_xmlTags[p_i_min] + "\".");
            }

            /* if we have xs:element definition with no type, exact one child element and mapping contains ":"
			 * we have to iterate a list of object and must print multiple xml elements
			 * otherwise we have usual xs:element definitions for current element
			 */
            if ((ForestNET.Lib.Helper.IsStringEmpty(p_o_xsdElement.Type)) && (p_o_xsdElement.Mapping.Contains(':')) && (p_o_xsdElement.Children.Count == 1))
            {
                /* check if current xml element has interlacing */
                if (!(e_xmlType == XMLType.BeginNoAttributes))
                {
                    /* if list has no children, check minOccurs attribute of xs:element child definition */
                    if (p_o_xsdElement.Children[0].MinOccurs != 0)
                    {
                        throw new ArgumentNullException("List with [" + p_o_xsdElement.Children[0].Name + "] xml tags is empty, minimum = " + p_o_xsdElement.Children[0].MinOccurs);
                    }
                }

                ForestNET.Lib.Global.ILogFiner("xs:element " + p_o_xsdElement.Name + " with no type, exact one child element as definition and list as mapping");

                /* hold parent object value for primitive arrays */
                Object? o_parentObject = p_o_object;

                /* create or retrieve object list data */
                if (p_o_object == null)
                { /* create a new object instance of xml element */
                    ForestNET.Lib.Global.ILogFiner(" - create new list object");

                    /* create list object */
                    p_o_object = (Object)(new List<Object?>());
                }
                else
                { /* we have to retrieve the list object */
                    ForestNET.Lib.Global.ILogFiner(" - retrieve list object");

                    /* get field type */
                    string s_fieldType = p_o_xsdElement.Mapping;

                    if (s_fieldType.Contains(':'))
                    {
                        /* remove enclosure of field type if it exists */
                        s_fieldType = s_fieldType.Substring(0, s_fieldType.IndexOf(":"));
                    }
                    else
                    {
                        /* remove assembly part */
                        if (s_fieldType.Contains(','))
                        {
                            s_fieldType = s_fieldType.Substring(0, s_fieldType.IndexOf(","));
                        }

                        /* remove package prefix */
                        if (s_fieldType.Contains('.'))
                        {
                            s_fieldType = s_fieldType.Substring(s_fieldType.LastIndexOf(".") + 1, s_fieldType.Length - (s_fieldType.LastIndexOf(".") + 1));
                        }

                        /* remove internal class prefix */
                        if (s_fieldType.Contains('+'))
                        {
                            s_fieldType = s_fieldType.Substring(s_fieldType.LastIndexOf("+") + 1, s_fieldType.Length - (s_fieldType.LastIndexOf("+") + 1));
                        }
                    }

                    /* check if we use property methods with invoke to get object data values */
                    if (this.UseProperties)
                    {
                        /* property info for accessing property */
                        System.Reflection.PropertyInfo? o_propertyInfo;

                        /* try to get access to property info */
                        try
                        {
                            o_propertyInfo = p_o_object?.GetType().GetProperty(s_fieldType);

                            if (o_propertyInfo == null)
                            {
                                throw new Exception("property info is null");
                            }
                        }
                        catch (Exception o_exc)
                        {
                            throw new MissingMemberException("Class instance property[" + s_fieldType + "] does not exist for class instance(" + p_o_object?.GetType().FullName + "): " + o_exc.Message);
                        }

                        /* check if we can access property */
                        if (!o_propertyInfo.CanRead)
                        {
                            throw new MemberAccessException("Cannot write property from class; instance property[" + s_fieldType + "] does not exist for class instance(" + p_o_object?.GetType().FullName + ")");
                        }

                        /* get object data of current property */
                        p_o_object = o_propertyInfo.GetValue(p_o_object);
                    }
                    else
                    {
                        /* field info for accessing field */
                        System.Reflection.FieldInfo? o_fieldInfo;

                        /* try to get access to field info */
                        try
                        {
                            o_fieldInfo = p_o_object?.GetType().GetField(s_fieldType);

                            if (o_fieldInfo == null)
                            {
                                throw new Exception("field info is null");
                            }
                        }
                        catch (Exception o_exc)
                        {
                            throw new MissingMemberException("Class instance field[" + s_fieldType + "] does not exist for class instance(" + p_o_object?.GetType().FullName + "): " + o_exc.Message);
                        }

                        /* check if we can access field */
                        if (!o_fieldInfo.IsPublic)
                        {
                            throw new MemberAccessException("Cannot read field from class; instance field[" + s_fieldType + "] does not exist for class instance(" + p_o_object?.GetType().FullName + ")");
                        }

                        /* get object data of current field */
                        p_o_object = o_fieldInfo.GetValue(p_o_object);
                    }

                    /* do not check if object is null or instance of List if we handle a primitive array */
                    if (!((ForestNET.Lib.Helper.IsStringEmpty(p_o_xsdElement.Type)) && (p_o_xsdElement.Mapping.EndsWith("[]"))))
                    {
                        /* check if list object is not null */
                        if (p_o_object == null)
                        {
                            throw new ArgumentNullException("List object from method[" + "get" + s_fieldType + "] not initialised for object: " + p_o_object?.GetType().FullName);
                        }

                        /* check if object is of type List */
                        if (!((p_o_object.GetType().IsGenericType) && (p_o_object.GetType().GetGenericTypeDefinition() == typeof(List<>))))
                        {
                            throw new ArgumentException("Object from method[" + "get" + s_fieldType + "] is not a list object for object: " + p_o_object.GetType().FullName);
                        }
                    }
                }

                int i_tempMin = p_i_min + 1;
                int i_level = 0;

                /* look for end of nested xml element tag */
                while ((!XML.GetXMLType(p_a_xmlTags[i_tempMin]).Equals(XMLType.Close)) || (i_level != 0))
                {
                    if ((XML.GetXMLType(p_a_xmlTags[i_tempMin]).Equals(XMLType.BeginNoAttributes)) || (XML.GetXMLType(p_a_xmlTags[i_tempMin]).Equals(XMLType.BeginWithAttributes)))
                    {
                        i_level++;
                    }
                    else if (XML.GetXMLType(p_a_xmlTags[i_tempMin]).Equals(XMLType.Close))
                    {
                        i_level--;
                    }

                    if (i_tempMin >= p_i_max)
                    {
                        /* forbidden state - interlacing is not valid in xml file */
                        throw new ArgumentException("Invalid nested xml element at(" + (p_i_min + 2) + ".-element) \"" + p_a_xmlTags[p_i_min] + "\".");
                    }

                    i_tempMin++;
                }

                ForestNET.Lib.Global.ILogFiner("\tfound interlacing xml element(" + (p_i_min + 2) + " to " + (i_tempMin + 2) + ") - " + p_a_xmlTags[p_i_min] + " ... " + p_a_xmlTags[i_tempMin]);

                /* create occurrence counter */
                int i_occurCnt = 0;

                int i_tempMin2 = p_i_min + 2;
                int i_until = i_tempMin - 1;

                /* if we handle an object list of primitives or a primitive array */
                if ((p_o_xsdElement.Children[0].IsArray) && (p_a_xmlTags[p_i_min].Substring(1).Equals(p_a_xmlTags[i_tempMin].Substring(2))))
                {
                    ForestNET.Lib.Global.ILogFiner("\t\tchange border counters, because first child of current xsd-element is an array and opening+closing xml-tags are matching");

                    i_tempMin2 = p_i_min + 1;
                    i_until = i_tempMin;
                }

                /* check if we just have an empty xml-element here */
                if (p_a_xmlTags[p_i_min].Equals("<" + p_o_xsdElement.Name + "/>"))
                {
                    i_tempMin2 = p_i_max;
                }

                /* we have to iterate a list of objects, so we must check if there is another nested element */
                /* if the end of nested element not corresponds to end of interlacing, we continue to look for another nested element */
                while (i_tempMin2 < i_until)
                {
                    i_tempMin2 = p_i_min + 2;
                    i_level = 0;

                    /* look for end of another nested xml element tag */
                    while ((!XML.GetXMLType(p_a_xmlTags[i_tempMin2]).Equals(XMLType.Close)) || (i_level != 0))
                    {
                        if ((XML.GetXMLType(p_a_xmlTags[i_tempMin2]).Equals(XMLType.BeginNoAttributes)) || (XML.GetXMLType(p_a_xmlTags[i_tempMin2]).Equals(XMLType.BeginWithAttributes)))
                        {
                            i_level++;
                        }
                        else if (XML.GetXMLType(p_a_xmlTags[i_tempMin2]).Equals(XMLType.Close))
                        {
                            i_level--;
                        }

                        if (i_tempMin2 >= p_i_max)
                        {
                            /* forbidden state - interlacing is not valid in xml file */
                            throw new ArgumentException("Invalid nested xml element at(" + (i_tempMin + 1 + 2) + ".-element) \"" + p_a_xmlTags[i_tempMin + 1] + "\".");
                        }

                        i_tempMin2++;
                    }

                    ForestNET.Lib.Global.ILogFiner("\t\tfound list xml element(" + (p_i_min + 3) + " to " + (i_tempMin2 + 2) + ") - " + p_a_xmlTags[p_i_min + 1] + " ... " + p_a_xmlTags[i_tempMin2]);

                    /* variable with mapping value of element level */
                    string s_oldMapping = p_o_xsdElement.Children[0].Mapping;

                    /* if we handle a primitive array list */
                    if (ForestNET.Lib.Helper.IsStringEmpty(p_o_xsdElement.Type) && (p_o_xsdElement.Mapping.EndsWith("[]")))
                    {
                        /* assume mapping to element level */
                        if (p_o_xsdElement.Mapping.Contains(':'))
                        {
                            p_o_xsdElement.Children[0].Mapping = p_o_xsdElement.Mapping.Split(":")[1];
                        }
                        else
                        {
                            /* remove assembly part */
                            if (p_o_xsdElement.Children[0].Mapping.Contains(','))
                            {
                                p_o_xsdElement.Children[0].Mapping = p_o_xsdElement.Children[0].Mapping.Substring(0, p_o_xsdElement.Children[0].Mapping.IndexOf(","));
                            }

                            /* remove package prefix */
                            if (p_o_xsdElement.Children[0].Mapping.Contains('.'))
                            {
                                p_o_xsdElement.Children[0].Mapping = p_o_xsdElement.Children[0].Mapping.Substring(p_o_xsdElement.Children[0].Mapping.LastIndexOf(".") + 1, p_o_xsdElement.Children[0].Mapping.Length - (p_o_xsdElement.Children[0].Mapping.LastIndexOf(".") + 1));
                            }

                            /* remove internal class prefix */
                            if (p_o_xsdElement.Children[0].Mapping.Contains('+'))
                            {
                                p_o_xsdElement.Children[0].Mapping = p_o_xsdElement.Children[0].Mapping.Substring(p_o_xsdElement.Children[0].Mapping.LastIndexOf("+") + 1, p_o_xsdElement.Children[0].Mapping.Length - (p_o_xsdElement.Children[0].Mapping.LastIndexOf("+") + 1));
                            }
                        }
                    }

                    /* handle nested xml element recursively */
                    Object? o_returnObject = this.XmlDecodeRecursive(p_a_xmlTags, ++p_i_min, i_tempMin, p_o_object, p_o_xsdElement.Children[0]);

                    /* undo change of mapping on element level */
                    if (ForestNET.Lib.Helper.IsStringEmpty(p_o_xsdElement.Type) && (p_o_xsdElement.Mapping.EndsWith("[]")))
                    {
                        p_o_xsdElement.Children[0].Mapping = s_oldMapping;
                    }

                    /* add return object of recursion to list */
                    if (o_returnObject != null)
                    {
                        if ((ForestNET.Lib.Helper.IsStringEmpty(p_o_xsdElement.Type)) && (p_o_xsdElement.Mapping.EndsWith("[]")) && (p_o_xsdElement.Children[0].IsArray))
                        {
                            /* get mapping of current xsd-element */
                            string s_mapping = p_o_xsdElement.Mapping;

                            /* get first part of mapping */
                            if (s_mapping.Contains(':'))
                            {
                                s_mapping = s_mapping.Split(":")[0];
                            }
                            else
                            {
                                /* remove assembly part */
                                if (s_mapping.Contains(','))
                                {
                                    s_mapping = s_mapping.Substring(0, s_mapping.IndexOf(","));
                                }

                                /* remove package prefix */
                                if (s_mapping.Contains('.'))
                                {
                                    s_mapping = s_mapping.Substring(s_mapping.LastIndexOf(".") + 1, s_mapping.Length - (s_mapping.LastIndexOf(".") + 1));
                                }

                                /* remove internal class prefix */
                                if (s_mapping.Contains('+'))
                                {
                                    s_mapping = s_mapping.Substring(s_mapping.LastIndexOf("+") + 1, s_mapping.Length - (s_mapping.LastIndexOf("+") + 1));
                                }
                            }

                            ForestNET.Lib.Global.ILogFiner("\t\tSet primitive array to " + s_xmlElementName + " array " + o_returnObject.GetType().FullName + " returned object from recursion: " + o_parentObject?.GetType().FullName);

                            /* set array elements in generic object list (o_returnObject) as elements of primitive array */
                            /* access by mapping and field type and using parent object to set primitive array in parent class */
                            this.SetPrimitiveArrayFieldOrProperty(s_mapping, o_parentObject, o_returnObject);
                        }
                        else if (p_o_xsdElement.Children[0].IsArray)
                        {
                            /* return object of recursion must be of instance List */
                            if (!((p_o_object != null) && (p_o_object.GetType().IsGenericType) && (p_o_object.GetType().GetGenericTypeDefinition() == typeof(List<>))))
                            {
                                throw new ArgumentException("object type '" + o_returnObject.GetType().FullName + "' of return object is not of instance 'List'");
                            }

                            ForestNET.Lib.Global.ILogFiner("\t\tOverwrite " + s_xmlElementName + " empty list with " + o_returnObject.GetType().FullName + " returned list from recursion");

                            /* overwrite current empty list with filled parsed list */
                            p_o_object = o_returnObject;

                            /* increase occur counter with list size */
                            List<Object?> o_temp = [.. ((System.Collections.IEnumerable)p_o_object).Cast<Object?>()];
                            i_occurCnt += o_temp.Count;
                        }
                        else
                        {
                            /* check if list is not null */
                            if (p_o_object == null)
                            {
                                throw new NullReferenceException("Could not add return object to parameter object list '" + s_xmlElementName + "', beacause parameter object is 'null'");
                            }

                            /* increase occur counter */
                            i_occurCnt++;

                            ForestNET.Lib.Global.ILogFiner("\t\tAdd to " + s_xmlElementName + " list " + o_returnObject.GetType().FullName + " returned object from recursion: " + p_o_object.GetType().FullName);

                            /* add returned object from recursion to our object list */
                            ((System.Collections.IList)p_o_object).Add(o_returnObject);
                        }
                    }

                    /* update xml tag pointer */
                    p_i_min = i_tempMin2;
                }

                /* update xml tag pointer */
                p_i_min = i_tempMin;

                ForestNET.Lib.Global.ILogFiner("\tend interlacing, continue from " + (p_i_min + 2) + " to " + (p_i_max + 2));

                /* check minOccurs attribute of child xs:element of current list */
                if (p_o_xsdElement.Children[0].MinOccurs > i_occurCnt)
                {
                    throw new ArgumentException("Not enough [" + p_o_xsdElement.Children[0].Name + "] xml tags, minimum = " + p_o_xsdElement.Children[0].MinOccurs);
                }

                /* check maxOccurs attribute of child xs:element of current list */
                if ((p_o_xsdElement.Children[0].MaxOccurs >= 0) && (i_occurCnt > p_o_xsdElement.Children[0].MaxOccurs))
                {
                    throw new ArgumentException("Too many [" + p_o_xsdElement.Children[0].Name + "] xml tags, maximum = " + p_o_xsdElement.Children[0].MaxOccurs);
                }
            }
            else if (p_o_xsdElement.Name.Equals(p_o_xsdElement.Type))
            { /* handle array elements as xml element tags */
                ForestNET.Lib.Global.ILogFiner("\t\t\t\titerate array elements from " + p_i_min + "[" + p_a_xmlTags[p_i_min] + "] - " + p_i_max + "[" + p_a_xmlTags[p_i_max - 1] + "]");

                /* object parameter is not of instance List */
                if ((p_o_object != null) && (!((p_o_object.GetType().IsGenericType) && (p_o_object.GetType().GetGenericTypeDefinition() == typeof(List<>)))))
                {
                    /* xs:element must have a primitive array type */
                    if (!((!ForestNET.Lib.Helper.IsStringEmpty(p_o_xsdElement.Mapping)) && (p_o_xsdElement.Mapping.EndsWith("[]"))))
                    {
                        throw new ArgumentException("xsd-element '" + p_o_xsdElement.Name + "' has not a primitive array type which ends with '[]': '" + p_o_xsdElement.Mapping + "'.");
                    }
                }

                /* create temporary list, so we can add new elements */
                List<Object?> o_tempList = [];

                /* iterate elements */
                for (int i = p_i_min; i < p_i_max; i++)
                {
                    /* object variable where value of xml element tag will be parsed in */
                    Object? o_value = null;
                    /* flag if current element returned value */
                    bool b_returnedValue = false;

                    /* get value if xml element is not empty */
                    if (!((e_xmlType == XMLType.Empty) || (e_xmlType == XMLType.EmptyWithAttributes)))
                    {
                        /* get value of xml element */
                        System.Text.RegularExpressions.Regex o_regex = new("(<" + p_o_xsdElement.Name + ">)([^<>]*?)(</" + p_o_xsdElement.Name + ">)");
                        string[] o_matcher = o_regex.Split(p_a_xmlTags[i]);

                        if (o_matcher.Length > 2)
                        {
                            string s_elementValue = o_matcher[2];

                            /* check if xs:element has any restrictions */
                            if (p_o_xsdElement.Restriction)
                            {
                                bool b_enumerationFound = false;
                                bool b_enumerationReturnValue = false;

                                foreach (XSDRestriction o_xsdRestriction in p_o_xsdElement.Restrictions)
                                {
                                    if (o_xsdRestriction.Name.ToLower().Equals("enumeration"))
                                    {
                                        b_enumerationFound = true;
                                    }

                                    b_enumerationReturnValue = this.CheckRestriction(s_elementValue, o_xsdRestriction, p_o_xsdElement.Type);

                                    if ((b_enumerationFound) && (b_enumerationReturnValue))
                                    {
                                        break;
                                    }
                                }

                                if ((b_enumerationFound) && (!b_enumerationReturnValue))
                                {
                                    throw new ArgumentException("Element[" + p_o_xsdElement.Name + "] with value[" + s_elementValue + "] does not match enumaration restrictions defined in xsd-schema for this xs:element");
                                }
                            }

                            /* check if element has value and if element value is not empty string value */
                            if ((!ForestNET.Lib.Helper.IsStringEmpty(s_elementValue)) && (!((this.b_printEmptystring) && (s_elementValue.ToString().Equals("&#x200B;")))))
                            {
                                /* cast string value to object and set returned value flag */
                                o_value = this.CastObjectFromString(s_elementValue, p_o_xsdElement.Type);
                                b_returnedValue = true;
                            }
                        }
                    }

                    ForestNET.Lib.Global.ILogFinest("\t\t\t\t\tarray element[" + p_o_xsdElement.Name + "] returned value=" + b_returnedValue);

                    /* check minOccurs attribute of xml element and if value is empty */
                    if ((p_o_xsdElement.MinOccurs > 0) && (!b_returnedValue))
                    {
                        if (p_o_xsdElement.Choice)
                        {
                            p_i_min--;
                            continue;
                        }
                        else
                        {
                            throw new ArgumentNullException("Missing element value for xs:element[" + p_o_object?.GetType().FullName + "." + p_o_xsdElement.Name + "]");
                        }
                    }

                    if (b_returnedValue)
                    {
                        /* add value to list */
                        o_tempList.Add(o_value);
                    }
                    else
                    {
                        /* add null to list */
                        o_tempList.Add(null);
                    }
                }

                /* check minOccurs attribute of xs:element */
                if (p_o_xsdElement.MinOccurs > o_tempList.Count)
                {
                    throw new ArgumentException("Not enough [" + p_o_xsdElement.Name + "] xml tags, minimum = " + p_o_xsdElement.MinOccurs);
                }

                /* check maxOccurs attribute of xs:element */
                if ((p_o_xsdElement.MaxOccurs >= 0) && (o_tempList.Count > p_o_xsdElement.MaxOccurs))
                {
                    throw new ArgumentException("Too many [" + p_o_xsdElement.Name + "] xml tags, maximum = " + p_o_xsdElement.MaxOccurs);
                }

                /* now we must copy the values of our temp list to our p_o_object */

                if (!((!ForestNET.Lib.Helper.IsStringEmpty(p_o_xsdElement.Mapping)) && (p_o_xsdElement.Mapping.EndsWith("[]"))))
                {
                    /* take over data if we have any */
                    if (o_tempList.Count > 0)
                    {
                        ForestNET.Lib.Global.ILogFiner("\t\t\t\t" + "overwrite p_o_object and take over data from temporary list, type[" + p_o_xsdElement.Type + "]");

                        /* take over data from temporary list to generic list */
                        try
                        {
                            /* get element type name of first element of object list */
                            string s_elementTypeName = o_tempList[0]?.GetType().AssemblyQualifiedName ?? throw new NullReferenceException("Could not get element type name with '" + o_tempList[0]?.GetType() + "'");
                            /* get element type of first element of object list */
                            Type o_elementType = Type.GetType(s_elementTypeName) ?? throw new NullReferenceException("Could not retrieve object type with '" + s_elementTypeName + "'");
                            /* create generic list type with element type */
                            Type o_genericListType = typeof(List<>).MakeGenericType(o_elementType);
                            /* create instance of generic list with generic list type and handle it as IList */
                            System.Collections.IList o_genericList = Activator.CreateInstance(o_genericListType) as System.Collections.IList ?? throw new NullReferenceException("Could not create a generic list with type '" + o_genericListType.FullName + "'");

                            /* iterate all elements of object list */
                            for (int i = 0; i < o_tempList.Count; i++)
                            {
                                /* add element to generic list */
                                o_genericList.Add(o_tempList[i]);
                            }

                            /* set generic list to our return value */
                            p_o_object = o_genericList;
                        }
                        catch (Exception o_exc)
                        {
                            throw new ArgumentException("Invalid type [" + p_o_xsdElement.Type + "] for field or property generic list <" + o_tempList[0]?.GetType().FullName + ">; " + o_exc);
                        }
                    }
                }
                else
                {
                    /* take over data if we have any */
                    if (o_tempList.Count > 0)
                    {
                        ForestNET.Lib.Global.ILogFiner("\t\t\t\t" + "overwrite p_o_object and take over data from primitive array, type[" + p_o_xsdElement.Mapping + "]");

                        /* take over data from temporary list to primitive array */
                        p_o_object = this.TransposeObjectListToPrimitiveArray(p_o_xsdElement.Mapping, o_tempList);
                    }
                }
            }
            else
            {
                /* do not create new instance if we want to skip this level */
                if (!p_o_xsdElement.Mapping.Equals("_skipLevel_"))
                {
                    /* create new object instance which will be returned at the end of this function */
                    p_o_object = Activator.CreateInstance(Type.GetType(p_o_xsdElement.Mapping) ?? throw new NullReferenceException("Could not retrieve object type with '" + p_o_xsdElement.Mapping + "'"));
                }

                /* flag if current element returned attributes */
                bool b_returnedAttributes = false;

                /* check if current xml element should have attributes */
                if (p_o_xsdElement.Attributes.Count > 0)
                {
                    bool b_required = false;

                    /* check if one attribute of the xml element is required */
                    foreach (XSDAttribute o_xsdAttribute in p_o_xsdElement.Attributes)
                    {
                        if (o_xsdAttribute.Required)
                        {
                            b_required = true;
                        }
                    }

                    /* check if current xml element has attributes */
                    if (((e_xmlType != XMLType.BeginWithAttributes) && (e_xmlType != XMLType.ElementWithAttributes) && (e_xmlType != XMLType.EmptyWithAttributes)) && (b_required))
                    {
                        throw new ArgumentException(s_xmlElementName + " has no attributes and is not compatible with xs:element (" + p_o_xsdElement.Name + "[" + p_o_xsdElement.Mapping + "])");
                    }

                    /* retrieve attributes */
                    b_returnedAttributes = this.ParseXMLAttributes(p_a_xmlTags, p_i_min, p_o_object, p_o_xsdElement);
                }

                /* create choice counter */
                int i_choiceCnt = 0;

                /* iterate all children of current xs:element */
                foreach (XSDElement o_xsdElement in p_o_xsdElement.Children)
                {
                    ForestNET.Lib.Global.ILogFinest("\t\t\titerate children of " + p_o_xsdElement.Name + ", choice=" + p_o_xsdElement.Choice + " ... child[" + o_xsdElement.Name + "] with type[" + o_xsdElement.Type + "] and attributes count[" + o_xsdElement.Attributes.Count + "]");

                    /* if xs:element has no primitive type we may have a special object definition */
                    if (ForestNET.Lib.Helper.IsStringEmpty(o_xsdElement.Type))
                    {
                        /* increase xml tag pointer */
                        p_i_min++;

                        ForestNET.Lib.Global.ILogFinest("\t\t\t\tchild[" + o_xsdElement.Name + "] compare to current xml tag[" + p_a_xmlTags[p_i_min] + "]");

                        /* xml tag must match with expected xs:element name or we have an empty xml-element in combination with a primitive array */
                        if ((!p_a_xmlTags[p_i_min].StartsWith("<" + o_xsdElement.Name)) || ((p_a_xmlTags[p_i_min].Equals("<" + o_xsdElement.Name + "/>")) && (o_xsdElement.Mapping.EndsWith("[]"))))
                        {
                            /* if we have a choice scope or child min. occurs is lower than 1, go to next xs:element */
                            if ((p_o_xsdElement.Choice) || (o_xsdElement.MinOccurs < 1))
                            {
                                /* only decrease min. pointer if we have not an empty xml-element */
                                if (!p_a_xmlTags[p_i_min].Equals("<" + o_xsdElement.Name + "/>"))
                                {
                                    p_i_min--;
                                }

                                continue;
                            }

                            /* xml tag does not match with xs:element name */
                            throw new ArgumentException(p_a_xmlTags[p_i_min] + " is not expected xs:element " + o_xsdElement.Name + " for recursion");
                        }

                        /* check choice counter */
                        if ((p_o_xsdElement.Choice) && (++i_choiceCnt > p_o_xsdElement.MaxOccurs))
                        {
                            throw new ArgumentException(p_o_xsdElement.Mapping + " has to many objects(" + i_choiceCnt + ") in xs:choice " + p_o_xsdElement.Name + "(" + p_o_object?.GetType().FullName + "), maximum = " + p_o_xsdElement.MaxOccurs);
                        }

                        int i_tempMin = p_i_min + 1;
                        int i_level = 0;

                        ForestNET.Lib.Global.ILogFinest(p_i_min + "\t\t\tlook for recursion borders for [" + o_xsdElement.Name + "] from " + (i_tempMin + 1));

                        /* look for end of nested xml element tag */
                        while ((!XML.GetXMLType(p_a_xmlTags[i_tempMin]).Equals(XMLType.Close)) || (i_level != 0))
                        {
                            ForestNET.Lib.Global.ILogFinest("\t\t\t" + i_level + "\t" + p_a_xmlTags[i_tempMin]);

                            if ((XML.GetXMLType(p_a_xmlTags[i_tempMin]).Equals(XMLType.BeginNoAttributes)) || (XML.GetXMLType(p_a_xmlTags[i_tempMin]).Equals(XMLType.BeginWithAttributes)))
                            {
                                i_level++;
                            }
                            else if (XML.GetXMLType(p_a_xmlTags[i_tempMin]).Equals(XMLType.Close))
                            {
                                i_level--;
                            }

                            if (i_tempMin >= p_i_max)
                            {
                                /* forbidden state - interlacing is not valid in xml file */
                                throw new ArgumentException("Invalid nested xml element at(" + (p_i_min + 2) + ".-element) \"" + p_a_xmlTags[p_i_min] + "\".");
                            }

                            i_tempMin++;
                        }

                        ForestNET.Lib.Global.ILogFinest("\t\t\t" + o_xsdElement.Name + " has no primitive type -> new recursion (" + (p_i_min + 2) + " to " + (i_tempMin + 2) + ") - " + p_a_xmlTags[p_i_min] + " ... " + p_a_xmlTags[i_tempMin]);

                        /* handle xml element recursively */
                        Object? o_returnObject = this.XmlDecodeRecursive(p_a_xmlTags, p_i_min, i_tempMin, p_o_object, o_xsdElement);

                        p_i_min = i_tempMin;

                        ForestNET.Lib.Global.ILogFinest("\t\t\tend recursion with [" + ((o_returnObject == null) ? "null" : o_returnObject.GetType().FullName) + "], continue " + (p_i_min + 2) + " to " + (p_i_max + 2));

                        /* handle return object of recursion */
                        if (o_returnObject != null)
                        {
                            ForestNET.Lib.Global.ILogFinest("\t\t\tReturned " + o_xsdElement.Mapping + " - " + o_returnObject.GetType().FullName + " for " + p_o_object?.GetType().FullName);

                            /* get mapping type */
                            string s_mapping = o_xsdElement.Mapping;

                            if (s_mapping.Equals("_skipLevel_"))
                            {
                                if (o_returnObject.GetType().FullName != p_o_object?.GetType().FullName)
                                {
                                    throw new ArgumentException("Invalid return object type '" + o_returnObject.GetType().FullName + "' for current element type '" + p_o_object?.GetType().FullName + "'.");
                                }

                                p_o_object = o_returnObject;
                            }
                            else
                            {
                                /* remove enclosure of mapping type if it exists */
                                if (s_mapping.Contains(':'))
                                {
                                    s_mapping = s_mapping.Substring(0, s_mapping.IndexOf(":"));
                                }
                                else
                                {
                                    /* remove assembly part */
                                    if (s_mapping.Contains(','))
                                    {
                                        s_mapping = s_mapping.Substring(0, s_mapping.IndexOf(","));
                                    }

                                    /* remove package prefix */
                                    if (s_mapping.Contains('.'))
                                    {
                                        s_mapping = s_mapping.Substring(s_mapping.LastIndexOf(".") + 1, s_mapping.Length - (s_mapping.LastIndexOf(".") + 1));
                                    }

                                    /* remove internal class prefix */
                                    if (s_mapping.Contains('+'))
                                    {
                                        s_mapping = s_mapping.Substring(s_mapping.LastIndexOf("+") + 1, s_mapping.Length - (s_mapping.LastIndexOf("+") + 1));
                                    }
                                }

                                /* check if we use property methods with invoke to set object data values */
                                if (this.UseProperties)
                                {
                                    /* property info for accessing property */
                                    System.Reflection.PropertyInfo? o_propertyInfo;

                                    /* try to get access to property info */
                                    try
                                    {
                                        o_propertyInfo = p_o_object?.GetType().GetProperty(s_mapping);

                                        if (o_propertyInfo == null)
                                        {
                                            throw new Exception("property info is null");
                                        }
                                    }
                                    catch (Exception o_exc)
                                    {
                                        throw new MissingMemberException("Class instance property[" + s_mapping + "] does not exist for class instance(" + p_o_object?.GetType().FullName + "): " + o_exc.Message);
                                    }

                                    /* check if we can write value to property */
                                    if (!o_propertyInfo.CanWrite)
                                    {
                                        throw new MemberAccessException("Cannot write property from class; instance property[" + s_mapping + "] does not exist for class instance(" + p_o_object?.GetType().FullName + ")");
                                    }

                                    /* set object data of current property */
                                    o_propertyInfo.SetValue(p_o_object, o_returnObject);
                                }
                                else
                                {
                                    /* field info for accessing field */
                                    System.Reflection.FieldInfo? o_fieldInfo;

                                    /* try to get access to field info */
                                    try
                                    {
                                        o_fieldInfo = p_o_object?.GetType().GetField(s_mapping);

                                        if (o_fieldInfo == null)
                                        {
                                            throw new Exception("field info is null");
                                        }
                                    }
                                    catch (Exception o_exc)
                                    {
                                        throw new MissingMemberException("Class instance field[" + s_mapping + "] does not exist for class instance(" + p_o_object?.GetType().FullName + "): " + o_exc.Message);
                                    }

                                    /* check if we can access field */
                                    if (!o_fieldInfo.IsPublic)
                                    {
                                        throw new MemberAccessException("Cannot read field from class; instance field[" + s_mapping + "] does not exist for class instance(" + p_o_object?.GetType().FullName + ")");
                                    }

                                    /* set object data of current field */
                                    o_fieldInfo.SetValue(p_o_object, o_returnObject);
                                }
                            }
                        }
                    }
                    else
                    { /* otherwise we have xs:elements with primitive types */
                        /* flag if current element returned value */
                        bool b_returnedValue = false;

                        /* get value if xml element is not empty */
                        if (!((e_xmlType == XMLType.Empty) || (e_xmlType == XMLType.EmptyWithAttributes)))
                        {
                            /* get value for xml element */
                            if (b_returnedAttributes)
                            {
                                b_returnedValue = this.ParseXMLElement(p_a_xmlTags, ++p_i_min, p_o_object, o_xsdElement, "(<" + o_xsdElement.Name + "[^<>]*?>)([^<>]*?)(</" + o_xsdElement.Name + ">)");
                            }
                            else
                            {
                                b_returnedValue = this.ParseXMLElement(p_a_xmlTags, ++p_i_min, p_o_object, o_xsdElement, "(<" + o_xsdElement.Name + ">)([^<>]*?)(</" + o_xsdElement.Name + ">)");
                            }
                        }

                        ForestNET.Lib.Global.ILogFinest("\t\t\t\tchild[" + o_xsdElement.Name + "] returned value=" + b_returnedValue);

                        /* check minOccurs attribute of xml element and if value is empty */
                        if ((o_xsdElement.MinOccurs > 0) && (!b_returnedValue))
                        {
                            if (p_o_xsdElement.Choice)
                            {
                                p_i_min--;
                                continue;
                            }
                            else
                            {
                                throw new ArgumentNullException("Missing element value for xs:element[" + p_o_object?.GetType().FullName + "." + o_xsdElement.Name + "]");
                            }
                        }

                        /* check choice counter */
                        if (p_o_xsdElement.Choice)
                        {
                            if ((++i_choiceCnt > p_o_xsdElement.MaxOccurs) && (b_returnedValue))
                            {
                                throw new ArgumentException(p_o_xsdElement.Name + " has to many objects(" + i_choiceCnt + ") in xs:choice " + p_o_object?.GetType().FullName + "." + o_xsdElement.Name + ", maximum = " + p_o_xsdElement.MaxOccurs);
                            }
                        }

                        /* check if child xml element has attributes, because of possible simpleContent */
                        if (o_xsdElement.Attributes.Count > 0)
                        {
                            bool b_required = false;

                            /* check if one attribute of the xml element is required */
                            foreach (XSDAttribute o_xsdAttribute in p_o_xsdElement.Attributes)
                            {
                                if (o_xsdAttribute.Required)
                                {
                                    b_required = true;
                                }
                            }

                            /* check if current xml element has attributes */
                            if (((e_xmlType != XMLType.BeginWithAttributes) && (e_xmlType != XMLType.ElementWithAttributes) && (e_xmlType != XMLType.EmptyWithAttributes)) && (b_required))
                            {
                                throw new ArgumentException(o_xsdElement.Name + " has no attributes");
                            }

                            /* retrieve attributes */
                            b_returnedAttributes = this.ParseXMLAttributes(p_a_xmlTags, p_i_min, p_o_object, o_xsdElement);
                        }
                    }
                }

                /* check choice counter for minimum objects */
                if (p_o_xsdElement.Choice)
                {
                    if (i_choiceCnt < p_o_xsdElement.MinOccurs)
                    {
                        throw new ArgumentException(p_o_xsdElement.Name + " has to few objects(" + i_choiceCnt + ") in xs:choice, minimum = " + p_o_xsdElement.MinOccurs);
                    }
                }
            }

            ForestNET.Lib.Global.ILogFiner("\treturn object with type[" + ((p_o_object == null) ? "null" : p_o_object.GetType().FullName) + "] for xsd-element[" + p_o_xsdElement.Name + "]");
            return p_o_object;
        }

        /// <summary>
        /// Parse xml element value into destination c# object field via field access or property method access
        /// </summary>
        /// <param name="p_a_xmlTags">xml lines</param>
        /// <param name="p_i_min">pointer where to start line iteration</param>
        /// <param name="p_o_object">destination c# object to decode xml information</param>
        /// <param name="p_o_xsdElement">current xml schema element</param>
        /// <param name="p_s_regexPattern">regex pattern where xml line must match</param>
        /// <returns>true - xml element has a value, false - could not parse value for xml element</returns>
        /// <exception cref="ArgumentException">element with value does not match structure or restrictions</exception>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        /// <exception cref="FormatException">exception parsing string to target type</exception>
        private bool ParseXMLElement(List<string> p_a_xmlTags, int p_i_min, Object? p_o_object, XSDElement p_o_xsdElement, string p_s_regexPattern)
        {
            /* return value */
            bool b_hasValue = false;

            /* get value of xml element */
            System.Text.RegularExpressions.Regex o_regex = new(p_s_regexPattern);
            string[] o_matcher = o_regex.Split(p_a_xmlTags[p_i_min]);

            if (o_matcher.Length > 2)
            {
                string s_elementValue = o_matcher[2].ToString();

                /* check if element has value */
                if (!ForestNET.Lib.Helper.IsStringEmpty(s_elementValue))
                {
                    b_hasValue = true;
                }

                /* check if xs:element has any restrictions */
                if (p_o_xsdElement.Restriction)
                {
                    bool b_enumerationFound = false;
                    bool b_enumerationReturnValue = false;

                    foreach (XSDRestriction o_xsdRestriction in p_o_xsdElement.Restrictions)
                    {
                        if (o_xsdRestriction.Name.ToLower().Equals("enumeration"))
                        {
                            b_enumerationFound = true;
                        }

                        b_enumerationReturnValue = this.CheckRestriction(s_elementValue, o_xsdRestriction, p_o_xsdElement.Type);

                        if ((b_enumerationFound) && (b_enumerationReturnValue))
                        {
                            break;
                        }
                    }

                    if ((b_enumerationFound) && (!b_enumerationReturnValue))
                    {
                        throw new ArgumentException("Element[" + p_o_xsdElement.Name + "] with value[" + s_elementValue + "] does not match enumaration restrictions defined in xsd-schema for this xs:element");
                    }
                }

                /* get mapping type */
                string s_mapping = p_o_xsdElement.Mapping;

                /* remove enclosure of mapping type if it exists */
                if (s_mapping.Contains(':'))
                {
                    s_mapping = s_mapping.Substring(0, s_mapping.IndexOf(":"));
                }
                else
                {
                    /* remove assembly part */
                    if (s_mapping.Contains(','))
                    {
                        s_mapping = s_mapping.Substring(0, s_mapping.IndexOf(","));
                    }

                    /* remove package prefix */
                    if (s_mapping.Contains('.'))
                    {
                        s_mapping = s_mapping.Substring(s_mapping.LastIndexOf(".") + 1, s_mapping.Length - (s_mapping.LastIndexOf(".") + 1));
                    }

                    /* remove internal class prefix */
                    if (s_mapping.Contains('+'))
                    {
                        s_mapping = s_mapping.Substring(s_mapping.LastIndexOf("+") + 1, s_mapping.Length - (s_mapping.LastIndexOf("+") + 1));
                    }
                }

                /* check if we use property methods with invoke to set object data values */
                if (this.UseProperties)
                {
                    /* property info for accessing property */
                    System.Reflection.PropertyInfo? o_propertyInfo;

                    /* try to get access to property info */
                    try
                    {
                        o_propertyInfo = p_o_object?.GetType().GetProperty(s_mapping);

                        if (o_propertyInfo == null)
                        {
                            throw new Exception("property info is null");
                        }
                    }
                    catch (Exception o_exc)
                    {
                        throw new MissingMemberException("Class instance property[" + s_mapping + "] does not exist for class instance(" + p_o_object?.GetType().FullName + "): " + o_exc.Message);
                    }

                    /* check if we can write value to property */
                    if (!o_propertyInfo.CanWrite)
                    {
                        throw new MemberAccessException("Cannot write property from class; instance property[" + s_mapping + "] does not exist for class instance(" + p_o_object?.GetType().FullName + ")");
                    }

                    /* set object data of current property */
                    o_propertyInfo.SetValue(p_o_object, this.CastObjectFromString(s_elementValue, p_o_xsdElement.Type, o_propertyInfo.PropertyType.FullName));
                }
                else
                {
                    /* field info for accessing field */
                    System.Reflection.FieldInfo? o_fieldInfo;

                    /* try to get access to field info */
                    try
                    {
                        o_fieldInfo = p_o_object?.GetType().GetField(s_mapping);

                        if (o_fieldInfo == null)
                        {
                            throw new Exception("field info is null");
                        }
                    }
                    catch (Exception o_exc)
                    {
                        throw new MissingMemberException("Class instance field[" + s_mapping + "] does not exist for class instance(" + p_o_object?.GetType().FullName + "): " + o_exc.Message);
                    }

                    /* check if we can access field */
                    if (!o_fieldInfo.IsPublic)
                    {
                        throw new MemberAccessException("Cannot read field from class; instance field[" + s_mapping + "] does not exist for class instance(" + p_o_object?.GetType().FullName + ")");
                    }

                    /* set object data of current field */
                    o_fieldInfo.SetValue(p_o_object, this.CastObjectFromString(s_elementValue, p_o_xsdElement.Type, o_fieldInfo.FieldType.FullName));
                }
            }

            return b_hasValue;
        }

        /// <summary>
        /// Parse xml attribute value into destination c# object field via field access or property method access
        /// </summary>
        /// <param name="p_a_xmlTags">xml lines</param>
        /// <param name="p_i_min">pointer where to start line iteration</param>
        /// <param name="p_o_object">destination c# object to decode xml information</param>
        /// <param name="p_o_xsdElement">current xml schema element</param>
        /// <returns>true - xml attribute has a value, false - could not parse value for xml attribute</returns>
        /// <exception cref="ArgumentException">element with value does not match structure or restrictions</exception>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        /// <exception cref="FormatException">exception parsing string to target type</exception>
        private bool ParseXMLAttributes(List<string> p_a_xmlTags, int p_i_min, Object? p_o_object, XSDElement p_o_xsdElement)
        {
            /* return value */
            bool b_hasAttributes = false;

            /* get attributes of xml element */
            System.Text.RegularExpressions.Regex o_regex = new("[^<>\\s/=]*?=\"[^<>/\"]*?\"");
            System.Text.RegularExpressions.MatchCollection o_matcher = o_regex.Matches(p_a_xmlTags[p_i_min]);

            if (o_matcher.Count > 0)
            {
                for (int i = 0; i < o_matcher.Count; i++)
                {
                    string s_attribute = o_matcher[i].ToString();
                    string s_attributeName = s_attribute.Substring(0, s_attribute.IndexOf("="));
                    bool b_found = false;

                    foreach (XSDAttribute o_xsdAttribute in p_o_xsdElement.Attributes)
                    {
                        if (s_attributeName.Equals(o_xsdAttribute.Name))
                        {
                            string s_attributeValue = s_attribute.Substring(s_attribute.IndexOf("=") + 1);
                            s_attributeValue = s_attributeValue.Substring(1, s_attributeValue.Length - 1 - 1);
                            ForestNET.Lib.Global.ILogFiner("\t\t\t\tfound Attribute [" + s_attributeName + "] with value=" + s_attributeValue);

                            /* if attribute is required but value is empty, throw exception */
                            if ((o_xsdAttribute.Required) && (ForestNET.Lib.Helper.IsStringEmpty(s_attributeValue)))
                            {
                                throw new ArgumentException("Missing attribute value for xs:attribute[" + p_o_object?.GetType().FullName + "." + o_xsdAttribute.Name + "]");
                            }

                            /* check if xs:element has any restrictions */
                            if (o_xsdAttribute.Restriction)
                            {
                                bool b_enumerationFound = false;
                                bool b_enumerationReturnValue = false;

                                foreach (XSDRestriction o_xsdRestriction in o_xsdAttribute.Restrictions)
                                {
                                    if (o_xsdRestriction.Name.ToLower().Equals("enumeration"))
                                    {
                                        b_enumerationFound = true;
                                    }

                                    b_enumerationReturnValue = this.CheckRestriction(s_attributeValue, o_xsdRestriction, o_xsdAttribute.Type);

                                    if ((b_enumerationFound) && (b_enumerationReturnValue))
                                    {
                                        break;
                                    }
                                }

                                if ((b_enumerationFound) && (!b_enumerationReturnValue))
                                {
                                    throw new ArgumentException("Attribute[" + o_xsdAttribute.Name + "] with value[" + s_attributeValue + "] does not match enumaration restrictions defined in xsd-schema for this xs:attribute");
                                }
                            }

                            /* get mapping type of attribute */
                            string s_mapping = o_xsdAttribute.Mapping;

                            /* remove enclosure of mapping type if it exists */
                            if (s_mapping.Contains(':'))
                            {
                                s_mapping = s_mapping.Substring(0, s_mapping.IndexOf(":"));
                            }
                            else
                            {
                                /* remove assembly part */
                                if (s_mapping.Contains(','))
                                {
                                    s_mapping = s_mapping.Substring(0, s_mapping.IndexOf(","));
                                }

                                /* remove package prefix */
                                if (s_mapping.Contains('.'))
                                {
                                    s_mapping = s_mapping.Substring(s_mapping.LastIndexOf(".") + 1, s_mapping.Length - (s_mapping.LastIndexOf(".") + 1));
                                }

                                /* remove internal class prefix */
                                if (s_mapping.Contains('+'))
                                {
                                    s_mapping = s_mapping.Substring(s_mapping.LastIndexOf("+") + 1, s_mapping.Length - (s_mapping.LastIndexOf("+") + 1));
                                }
                            }

                            /* check if we use property methods with invoke to set object data values */
                            if (this.UseProperties)
                            {
                                /* property info for accessing property */
                                System.Reflection.PropertyInfo? o_propertyInfo;

                                /* try to get access to property info */
                                try
                                {
                                    o_propertyInfo = p_o_object?.GetType().GetProperty(s_mapping);

                                    if (o_propertyInfo == null)
                                    {
                                        throw new Exception("property info is null");
                                    }
                                }
                                catch (Exception o_exc)
                                {
                                    throw new MissingMemberException("Class instance property[" + s_mapping + "] does not exist for class instance(" + p_o_object?.GetType().FullName + "): " + o_exc.Message);
                                }

                                /* check if we can write value to property */
                                if (!o_propertyInfo.CanWrite)
                                {
                                    throw new MemberAccessException("Cannot write property from class; instance property[" + s_mapping + "] does not exist for class instance(" + p_o_object?.GetType().FullName + ")");
                                }

                                /* set object data of current property */
                                o_propertyInfo.SetValue(p_o_object, this.CastObjectFromString(s_attributeValue, p_o_xsdElement.Type, o_propertyInfo.PropertyType.FullName));
                            }
                            else
                            {
                                /* field info for accessing field */
                                System.Reflection.FieldInfo? o_fieldInfo;

                                /* try to get access to field info */
                                try
                                {
                                    o_fieldInfo = p_o_object?.GetType().GetField(s_mapping);

                                    if (o_fieldInfo == null)
                                    {
                                        throw new Exception("field info is null");
                                    }
                                }
                                catch (Exception o_exc)
                                {
                                    throw new MissingMemberException("Class instance field[" + s_mapping + "] does not exist for class instance(" + p_o_object?.GetType().FullName + "): " + o_exc.Message);
                                }

                                /* check if we can access field */
                                if (!o_fieldInfo.IsPublic)
                                {
                                    throw new MemberAccessException("Cannot read field from class; instance field[" + s_mapping + "] does not exist for class instance(" + p_o_object?.GetType().FullName + ")");
                                }

                                /* set object data of current field */
                                o_fieldInfo.SetValue(p_o_object, this.CastObjectFromString(s_attributeValue, p_o_xsdElement.Type, o_fieldInfo.FieldType.FullName));
                            }

                            b_found = true;
                            b_hasAttributes = true;
                        }
                    }

                    if (!b_found)
                    {
                        throw new ArgumentException("Xml attribute[" + s_attributeName + "] not expected and not availalbe in xsd-schema at(" + (p_i_min + 2) + ".-element) \"" + p_a_xmlTags[p_i_min] + "\".");
                    }
                }
            }
            return b_hasAttributes;
        }

        /// <summary>
        /// Analyze xml element value and get a unique value type from it
        /// </summary>
        /// <param name="p_s_xmlTag">xml element value as string</param>
        /// <returns>unique xml enumeration type</returns>
        private static XMLType GetXMLType(string p_s_xmlTag)
        {
            System.Text.RegularExpressions.Regex o_regex;
            o_regex = new System.Text.RegularExpressions.Regex("<[^<>\\s]*?>[^<>]*?</[^<>]*?>");

            if (o_regex.Match(p_s_xmlTag).Success)
            {
                return XMLType.ElementNoAttributes;
            }

            o_regex = new System.Text.RegularExpressions.Regex("<[^<>]*?>[^<>]*?</[^<>]*?>");

            if (o_regex.Match(p_s_xmlTag).Success)
            {
                return XMLType.ElementWithAttributes;
            }

            o_regex = new System.Text.RegularExpressions.Regex("<[^<>/\\s]*?>");

            if (o_regex.Match(p_s_xmlTag).Success)
            {
                return XMLType.BeginNoAttributes;
            }

            o_regex = new System.Text.RegularExpressions.Regex("<[^<>/]*?>");

            if (o_regex.Match(p_s_xmlTag).Success)
            {
                return XMLType.BeginWithAttributes;
            }

            o_regex = new System.Text.RegularExpressions.Regex("</[^<>\\s]*?>");

            if (o_regex.Match(p_s_xmlTag).Success)
            {
                return XMLType.Close;
            }

            o_regex = new System.Text.RegularExpressions.Regex("<[^<>\\s]*?/>");

            if (o_regex.Match(p_s_xmlTag).Success)
            {
                return XMLType.Empty;
            }

            o_regex = new System.Text.RegularExpressions.Regex("<[^<>]*?/>");

            if (o_regex.Match(p_s_xmlTag).Success)
            {
                return XMLType.EmptyWithAttributes;
            }

            throw new ArgumentException("Could not determine xml type with xml tag: '" + p_s_xmlTag + "'");
        }

        /// <summary>
        /// Convert a string value from a xml element to an object to decode it into an object
        /// </summary>
        /// <param name="p_s_value">string value of xml element from file</param>
        /// <param name="p_s_type">type of destination object field, conform to xsd schema</param>
        /// <returns>casted object value from string</returns>
        /// <exception cref="ArgumentException">invalid type parameter</exception>
        /// <exception cref="FormatException">exception parsing string to target type</exception>
        private Object? CastObjectFromString(string? p_s_value, string? p_s_type)
        {
            return this.CastObjectFromString(p_s_value, p_s_type, p_s_type);
        }

        /// <summary>
        /// Convert a string value from a xml element to an object to decode it into an object
        /// </summary>
        /// <param name="p_s_value">string value of xml element from file</param>
        /// <param name="p_s_schemaType">type of field within schema</param>
        /// <param name="p_s_type">type of destination object field, conform to xsd schema</param>
        /// <returns>casted object value from string</returns>
        /// <exception cref="ArgumentException">invalid type parameter</exception>
        /// <exception cref="FormatException">exception parsing string to target type</exception>
        private Object? CastObjectFromString(string? p_s_value, string? p_s_schemaType, string? p_s_type)
        {
            Object? o_foo = null;

            /* return null if value or type parameter is null */
            if ((p_s_value == null) || (p_s_schemaType == null) || (p_s_type == null))
            {
                return o_foo;
            }

            if (!ForestNET.Lib.Helper.IsStringEmpty(p_s_value))
            {
                p_s_type = p_s_type.ToLower();

                /* transpose primitive type to class type */
                switch (p_s_type)
                {
                    case "system.byte":
                        p_s_type = "unsignedbyte";
                        break;
                    case "system.int16":
                        p_s_type = "short";
                        break;
                    case "system.int32":
                        p_s_type = "integer";
                        break;
                    case "system.int64":
                        p_s_type = "long";
                        break;
                    case "system.single":
                        p_s_type = "float";
                        break;
                    case "system.double":
                        p_s_type = "double";
                        break;
                    case "system.boolean":
                        p_s_type = "boolean";
                        break;
                    case "system.string":
                        p_s_type = "string";
                        break;
                    case "system.datetime":
                        if (p_s_schemaType.ToLower().Equals("date"))
                        {
                            p_s_type = "date";
                        }
                        else if (p_s_schemaType.ToLower().Equals("time"))
                        {
                            p_s_type = "time";
                        }
                        else
                        {
                            p_s_type = "datetime";
                        }
                        break;
                    case "system.timespan":
                        p_s_type = "time";
                        break;
                    case "system.decimal":
                        p_s_type = "decimal";
                        break;
                    case "system.sbyte":
                        p_s_type = "sbyte";
                        break;
                    case "system.uint16":
                        p_s_type = "unsignedshort";
                        break;
                    case "system.uint32":
                        p_s_type = "unsignedinteger";
                        break;
                    case "system.uint64":
                        p_s_type = "unsignedlong";
                        break;
                }

                List<string> a_stringTypes = ["string", "duration", "hexbinary", "base64binary", "anyuri", "normalizedstring", "token", "language", "name", "ncname", "nmtoken", "id", "idref", "entity"];
                List<string> a_integerTypes = ["integer", "int", "positiveinteger", "nonpositiveinteger", "negativeinteger", "nonnegativeinteger"];

                if (p_s_type.Equals("boolean"))
                {
                    o_foo = Convert.ToBoolean(p_s_value);
                }
                else if (a_stringTypes.Contains(p_s_type))
                {
                    if ((p_s_value.Equals("&#x200B;")) && (this.b_printEmptystring))
                    {
                        p_s_value = "";
                    }

                    o_foo = p_s_value;
                }
                else if (p_s_type.Equals("datetime"))
                {
                    if (!ForestNET.Lib.Helper.IsDateTime(p_s_value))
                    {
                        throw new ArgumentException("Illegal value '" + p_s_value + "' for 'datetime'");
                    }

                    if (this.b_useISO8601UTC)
                    {
                        o_foo = ForestNET.Lib.Helper.FromISO8601UTC(p_s_value);
                    }
                    else
                    {
                        o_foo = ForestNET.Lib.Helper.FromDateTimeString(p_s_value);
                    }
                }
                else if (p_s_type.Equals("date"))
                {
                    if (this.b_useISO8601UTC)
                    {
                        if (!ForestNET.Lib.Helper.IsDateTime(p_s_value))
                        {
                            throw new ArgumentException("Illegal value '" + p_s_value + "' for 'date'");
                        }

                        o_foo = ForestNET.Lib.Helper.FromISO8601UTC(p_s_value).Date;
                    }
                    else
                    {
                        if (!ForestNET.Lib.Helper.IsDate(p_s_value))
                        {
                            throw new ArgumentException("Illegal value '" + p_s_value + "' for 'date'");
                        }

                        o_foo = ForestNET.Lib.Helper.FromDateString(p_s_value);
                    }
                }
                else if (p_s_type.Equals("time"))
                {
                    if (this.b_useISO8601UTC)
                    {
                        if (!ForestNET.Lib.Helper.IsDateTime(p_s_value))
                        {
                            throw new ArgumentException("Illegal value '" + p_s_value + "' for 'time'");
                        }

                        /* recognize empty time value in ISO-8601 UTC format */
                        if (p_s_value.Equals("0001-01-01T00:00:00Z"))
                        {
                            o_foo = default(TimeSpan);
                        }
                        else
                        {
                            o_foo = ForestNET.Lib.Helper.FromISO8601UTC(p_s_value).TimeOfDay;
                        }
                    }
                    else
                    {
                        if (!ForestNET.Lib.Helper.IsTime(p_s_value))
                        {
                            throw new ArgumentException("Illegal value '" + p_s_value + "' for 'time'");
                        }

                        o_foo = ForestNET.Lib.Helper.FromTimeString(p_s_value);
                    }
                }
                else if (p_s_type.Equals("decimal"))
                {
                    o_foo = Convert.ToDecimal(p_s_value, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                }
                else if (p_s_type.Equals("double"))
                {
                    o_foo = Convert.ToDouble(p_s_value, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                }
                else if (p_s_type.Equals("float"))
                {
                    o_foo = Convert.ToSingle(p_s_value, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                }
                else if (a_integerTypes.Contains(p_s_type))
                {
                    o_foo = Convert.ToInt32(p_s_value);
                }
                else if (p_s_type.Equals("unsignedinteger"))
                {
                    o_foo = Convert.ToUInt32(p_s_value);
                }
                else if (p_s_type.Equals("sbyte"))
                {
                    o_foo = Convert.ToSByte(p_s_value);
                }
                else if (p_s_type.Equals("unsignedbyte"))
                {
                    o_foo = Convert.ToByte(p_s_value);
                }
                else if (p_s_type.Equals("short"))
                {
                    o_foo = Convert.ToInt16(p_s_value);
                }
                else if (p_s_type.Equals("unsignedshort"))
                {
                    o_foo = Convert.ToUInt16(p_s_value);
                }
                else if (p_s_type.Equals("long"))
                {
                    o_foo = Convert.ToInt64(p_s_value);
                }
                else if (p_s_type.Equals("unsignedlong"))
                {
                    o_foo = Convert.ToUInt64(p_s_value);
                }
                else
                {
                    throw new ArgumentException("Invalid type[" + p_s_type + "] for " + p_s_value);
                }
            }

            return o_foo;
        }

        /// <summary>
        /// Method to transpose all objects from a generic list to a primitive array property/field
        /// </summary>
        /// <param name="p_s_mapping">mapping class and type hint to cast value to object's primitive array field</param>
        /// <param name="p_o_genericList">object parameter where generic lists holds all data</param>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        /// <exception cref="ArgumentException">invalid type parameter</exception>
        /// <exception cref="FormatException">exception parsing string to target type</exception>
        private Object? TransposeObjectListToPrimitiveArray(string p_s_mapping, Object? p_o_genericList)
        {
            /* cast object value parameter to object list */
            List<Object?> o_foo = [.. ((System.Collections.IEnumerable)(p_o_genericList ?? new List<Object?>())).Cast<Object?>()];

            /* check if we have any elements */
            if (o_foo.Count < 1)
            {
                return null;
            }

            /* remove '[]' from mapping value */
            if (p_s_mapping.EndsWith("[]"))
            {
                p_s_mapping = p_s_mapping.Substring(0, p_s_mapping.Length - 2);
            }

            /* lower mapping value */
            p_s_mapping = p_s_mapping.ToLower();

            /* transpose mapping value to primitive class type */
            switch (p_s_mapping)
            {
                case "unsignedbyte":
                    p_s_mapping = "system.byte";
                    break;
                case "short":
                    p_s_mapping = "system.int16";
                    break;
                case "int":
                case "integer":
                    p_s_mapping = "system.int32";
                    break;
                case "long":
                    p_s_mapping = "system.int64";
                    break;
                case "float":
                    p_s_mapping = "system.single";
                    break;
                case "double":
                    p_s_mapping = "system.double";
                    break;
                case "bool":
                case "boolean":
                    p_s_mapping = "system.boolean";
                    break;
                case "string":
                    p_s_mapping = "system.string";
                    break;
                case "date":
                case "datetime":
                    p_s_mapping = "system.datetime";
                    break;
                case "time":
                case "timespan":
                    p_s_mapping = "system.timespan";
                    break;
                case "decimal":
                    p_s_mapping = "system.decimal";
                    break;
                case "byte":
                case "sbyte":
                    p_s_mapping = "system.sbyte";
                    break;
                case "unsignedshort":
                case "ushort":
                    p_s_mapping = "system.uint16";
                    break;
                case "unsignedinteger":
                case "uint":
                    p_s_mapping = "system.uint32";
                    break;
                case "ulong":
                case "unsignedlong":
                    p_s_mapping = "system.uint64";
                    break;
                case "object":
                    p_s_mapping = "system.object";
                    break;
            }

            /* cast object list to primitive array, set object array with property info of current element */
            if (p_s_mapping.Equals("system.boolean"))
            {
                bool[] o_bar = new bool[o_foo.Count];

                for (int i = 0; i < o_foo.Count; i++)
                {
                    o_bar[i] = (bool?)this.CastObjectFromString((o_foo[i]?.ToString()), p_s_mapping) ?? default;
                }

                return o_bar;
            }
            else if (p_s_mapping.Equals("system.byte"))
            {
                byte[] o_bar = new byte[o_foo.Count];

                for (int i = 0; i < o_foo.Count; i++)
                {
                    o_bar[i] = (byte?)this.CastObjectFromString((o_foo[i]?.ToString()), p_s_mapping) ?? default;
                }

                return o_bar;
            }
            else if (p_s_mapping.Equals("system.sbyte"))
            {
                sbyte[] o_bar = new sbyte[o_foo.Count];

                for (int i = 0; i < o_foo.Count; i++)
                {
                    o_bar[i] = (sbyte?)this.CastObjectFromString((o_foo[i]?.ToString()), p_s_mapping) ?? default;
                }

                return o_bar;
            }
            else if (p_s_mapping.Equals("system.char"))
            {
                char[] o_bar = new char[o_foo.Count];

                for (int i = 0; i < o_foo.Count; i++)
                {
                    o_bar[i] = (char?)this.CastObjectFromString((o_foo[i]?.ToString()), p_s_mapping) ?? default;
                }

                return o_bar;
            }
            else if (p_s_mapping.Equals("system.single"))
            {
                float[] o_bar = new float[o_foo.Count];

                for (int i = 0; i < o_foo.Count; i++)
                {
                    string? s_foo = null;

                    if (o_foo[i] != null)
                    {
                        float f_foo = Convert.ToSingle(o_foo[i]);
                        s_foo = f_foo.ToString("0.000000000", new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    }

                    o_bar[i] = (float?)this.CastObjectFromString(s_foo, p_s_mapping) ?? default;
                }

                return o_bar;
            }
            else if (p_s_mapping.Equals("system.double"))
            {
                double[] o_bar = new double[o_foo.Count];

                for (int i = 0; i < o_foo.Count; i++)
                {
                    string? s_foo = null;

                    if (o_foo[i] != null)
                    {
                        double d_foo = Convert.ToDouble(o_foo[i]);
                        s_foo = d_foo.ToString("0.00000000000000000", new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    }

                    o_bar[i] = (double?)this.CastObjectFromString(s_foo, p_s_mapping) ?? default;
                }

                return o_bar;
            }
            else if (p_s_mapping.Equals("system.int16"))
            {
                short[] o_bar = new short[o_foo.Count];

                for (int i = 0; i < o_foo.Count; i++)
                {
                    o_bar[i] = (short?)this.CastObjectFromString((o_foo[i]?.ToString()), p_s_mapping) ?? default;
                }

                return o_bar;
            }
            else if (p_s_mapping.Equals("system.int32"))
            {
                int[] o_bar = new int[o_foo.Count];

                for (int i = 0; i < o_foo.Count; i++)
                {
                    o_bar[i] = (int?)this.CastObjectFromString((o_foo[i]?.ToString()), p_s_mapping) ?? default;
                }

                return o_bar;
            }
            else if (p_s_mapping.Equals("system.int64"))
            {
                long[] o_bar = new long[o_foo.Count];

                for (int i = 0; i < o_foo.Count; i++)
                {
                    o_bar[i] = (long?)this.CastObjectFromString((o_foo[i]?.ToString()), p_s_mapping) ?? default;
                }

                return o_bar;
            }
            else if (p_s_mapping.Equals("system.uint16"))
            {
                ushort[] o_bar = new ushort[o_foo.Count];

                for (int i = 0; i < o_foo.Count; i++)
                {
                    o_bar[i] = (ushort?)this.CastObjectFromString((o_foo[i]?.ToString()), p_s_mapping) ?? default;
                }

                return o_bar;
            }
            else if (p_s_mapping.Equals("system.uint32"))
            {
                uint[] o_bar = new uint[o_foo.Count];

                for (int i = 0; i < o_foo.Count; i++)
                {
                    o_bar[i] = (uint?)this.CastObjectFromString((o_foo[i]?.ToString()), p_s_mapping) ?? default;
                }

                return o_bar;
            }
            else if (p_s_mapping.Equals("system.uint64"))
            {
                ulong[] o_bar = new ulong[o_foo.Count];

                for (int i = 0; i < o_foo.Count; i++)
                {
                    o_bar[i] = (ulong?)this.CastObjectFromString((o_foo[i]?.ToString()), p_s_mapping) ?? default;
                }

                return o_bar;
            }
            else if (p_s_mapping.Equals("system.string"))
            {
                string[] o_bar = new string[o_foo.Count];

                for (int i = 0; i < o_foo.Count; i++)
                {
                    o_bar[i] = (string?)this.CastObjectFromString((o_foo[i]?.ToString()), p_s_mapping) ?? string.Empty;
                }

                return o_bar;
            }
            else if (p_s_mapping.Equals("system.datetime"))
            {
                DateTime[] o_bar = new DateTime[o_foo.Count];

                for (int i = 0; i < o_foo.Count; i++)
                {
                    o_bar[i] = (DateTime?)this.CastObjectFromString((o_foo[i]?.ToString()), p_s_mapping) ?? default;
                }

                return o_bar;
            }
            else if (p_s_mapping.Equals("system.timespan"))
            {
                TimeSpan[] o_bar = new TimeSpan[o_foo.Count];

                for (int i = 0; i < o_foo.Count; i++)
                {
                    o_bar[i] = (TimeSpan?)this.CastObjectFromString((o_foo[i]?.ToString()), p_s_mapping) ?? default;
                }

                return o_bar;
            }
            else if (p_s_mapping.Equals("system.decimal"))
            {
                decimal[] o_bar = new decimal[o_foo.Count];

                for (int i = 0; i < o_foo.Count; i++)
                {
                    string? s_foo = null;

                    if (o_foo[i] != null)
                    {
                        decimal m_foo = Convert.ToDecimal(o_foo[i]);
                        s_foo = m_foo.ToString("0.00000000000000000000000000000", new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    }

                    o_bar[i] = (decimal?)this.CastObjectFromString(s_foo, p_s_mapping) ?? default;
                }

                return o_bar;
            }
            else if (p_s_mapping.Equals("system.object"))
            {
                object[] o_bar = new object[o_foo.Count];

                for (int i = 0; i < o_foo.Count; i++)
                {
                    o_bar[i] = this.CastObjectFromString((o_foo[i]?.ToString()), p_s_mapping) ?? new object();
                }

                return o_bar;
            }
            else
            {
                throw new ArgumentException("Invalid type [" + p_s_mapping + "]");
            }
        }

        /// <summary>
        /// Method to set primitive array property/field of an object with simple object value, so no cast will be done
        /// </summary>
        /// <param name="p_s_mapping">mapping class and type hint to cast value to object's primitive array field</param>
        /// <param name="p_o_object">object parameter where xml data will be decoded and cast into object fields</param>
        /// <param name="p_o_objectValue">object value of xml element from file line</param>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        /// <exception cref="ArgumentException">invalid type parameter</exception>
        /// <exception cref="FormatException">exception parsing string to target type</exception>
        private void SetPrimitiveArrayFieldOrProperty(string p_s_mapping, Object? p_o_object, Object? p_o_objectValue)
        {
            /* cast object value parameter to object list */
            List<Object?> o_foo = [.. ((System.Collections.IEnumerable)(p_o_objectValue ?? new List<Object?>())).Cast<Object?>()];

            /* check if we use property methods with invoke to set object data values */
            if (this.UseProperties)
            {
                /* property info for accessing generic list */
                System.Reflection.PropertyInfo? o_propertyInfo;

                /* try to get access to property info */
                try
                {
                    o_propertyInfo = p_o_object?.GetType().GetProperty(p_s_mapping);

                    if (o_propertyInfo == null)
                    {
                        throw new Exception("property info is null");
                    }
                }
                catch (Exception o_exc)
                {
                    throw new MissingMemberException("Class instance property[" + p_s_mapping + "] does not exist for class instance(" + p_o_object?.GetType().FullName + "): " + o_exc.Message);
                }

                /* check if we can access generic list property */
                if (!o_propertyInfo.CanWrite)
                {
                    throw new MemberAccessException("Cannot write property from class; instance property[" + p_s_mapping + "] does not exist for class instance(" + p_o_object?.GetType().FullName + ")");
                }

                /* get primitive array type */
                string s_primitiveArrayType = o_propertyInfo.PropertyType.FullName ?? "null";

                /* remove '[]' from type value */
                if (s_primitiveArrayType.EndsWith("[]"))
                {
                    s_primitiveArrayType = s_primitiveArrayType.Substring(0, s_primitiveArrayType.Length - 2);
                }

                if (o_foo.Count < 1)
                {
                    /* set primitive array to null if we have no elements */
                    o_propertyInfo.SetValue(p_o_object, null);
                }
                else
                {
                    /* cast object list to primitive array, set object array with property info of current element */
                    if (s_primitiveArrayType.Equals("System.Boolean"))
                    {
                        bool[] o_bar = new bool[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (bool?)this.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.Byte"))
                    {
                        byte[] o_bar = new byte[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (byte?)this.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.SByte"))
                    {
                        sbyte[] o_bar = new sbyte[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (sbyte?)this.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.Char"))
                    {
                        char[] o_bar = new char[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (char?)this.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.Single"))
                    {
                        float[] o_bar = new float[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            string? s_foo = null;

                            if (o_foo[i] != null)
                            {
                                float f_foo = Convert.ToSingle(o_foo[i]);
                                s_foo = f_foo.ToString("0.000000000", new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                            }

                            o_bar[i] = (float?)this.CastObjectFromString(s_foo, s_primitiveArrayType) ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.Double"))
                    {
                        double[] o_bar = new double[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            string? s_foo = null;

                            if (o_foo[i] != null)
                            {
                                double d_foo = Convert.ToDouble(o_foo[i]);
                                s_foo = d_foo.ToString("0.00000000000000000", new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                            }

                            o_bar[i] = (double?)this.CastObjectFromString(s_foo, s_primitiveArrayType) ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.Int16"))
                    {
                        short[] o_bar = new short[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (short?)this.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.Int32"))
                    {
                        int[] o_bar = new int[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (int?)this.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.Int64"))
                    {
                        long[] o_bar = new long[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (long?)this.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.UInt16"))
                    {
                        ushort[] o_bar = new ushort[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (ushort?)this.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.UInt32"))
                    {
                        uint[] o_bar = new uint[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (uint?)this.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.UInt64"))
                    {
                        ulong[] o_bar = new ulong[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (ulong?)this.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.String"))
                    {
                        string[] o_bar = new string[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (string?)this.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? string.Empty;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.DateTime"))
                    {
                        DateTime[] o_bar = new DateTime[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (DateTime?)this.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.TimeSpan"))
                    {
                        TimeSpan[] o_bar = new TimeSpan[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (TimeSpan?)this.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.Decimal"))
                    {
                        decimal[] o_bar = new decimal[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            string? s_foo = null;

                            if (o_foo[i] != null)
                            {
                                decimal m_foo = Convert.ToDecimal(o_foo[i]);
                                s_foo = m_foo.ToString("0.00000000000000000000000000000", new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                            }

                            o_bar[i] = (decimal?)this.CastObjectFromString(s_foo, s_primitiveArrayType) ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.Object"))
                    {
                        object[] o_bar = new object[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = this.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? new object();
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar);
                    }
                    else
                    {
                        throw new ArgumentException("Invalid type [" + s_primitiveArrayType + "] for property[" + p_s_mapping + "] and object [" + p_o_object?.GetType().FullName + "]");
                    }
                }
            }
            else
            {
                /* field info for accessing generic list */
                System.Reflection.FieldInfo? o_fieldInfo;

                /* try to get access to field info */
                try
                {
                    o_fieldInfo = p_o_object?.GetType().GetField(p_s_mapping);

                    if (o_fieldInfo == null)
                    {
                        throw new Exception("field info is null");
                    }
                }
                catch (Exception o_exc)
                {
                    throw new MissingMemberException("Class instance field[" + p_s_mapping + "] does not exist for class instance(" + p_o_object?.GetType().FullName + "): " + o_exc.Message);
                }

                /* check if we can access generic list field */
                if (!o_fieldInfo.IsPublic)
                {
                    throw new MemberAccessException("Cannot read field from class; instance field[" + p_s_mapping + "] does not exist for class instance(" + p_o_object?.GetType().FullName + ")");
                }

                /* get primitive array type */
                string s_primitiveArrayType = o_fieldInfo.FieldType.FullName ?? "null";

                /* remove '[]' from type value */
                if (s_primitiveArrayType.EndsWith("[]"))
                {
                    s_primitiveArrayType = s_primitiveArrayType.Substring(0, s_primitiveArrayType.Length - 2);
                }

                if (o_foo.Count < 1)
                {
                    /* set primitive array to null if we have no elements */
                    o_fieldInfo.SetValue(p_o_object, null);
                }
                else
                {
                    /* cast object list to primitive array, set object array with property info of current element */
                    if (s_primitiveArrayType.Equals("System.Boolean"))
                    {
                        bool[] o_bar = new bool[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (bool?)this.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.Byte"))
                    {
                        byte[] o_bar = new byte[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (byte?)this.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.SByte"))
                    {
                        sbyte[] o_bar = new sbyte[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (sbyte?)this.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.Char"))
                    {
                        char[] o_bar = new char[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (char?)this.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.Single"))
                    {
                        float[] o_bar = new float[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            string? s_foo = null;

                            if (o_foo[i] != null)
                            {
                                float f_foo = Convert.ToSingle(o_foo[i]);
                                s_foo = f_foo.ToString("0.000000000", new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                            }

                            o_bar[i] = (float?)this.CastObjectFromString(s_foo, s_primitiveArrayType) ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.Double"))
                    {
                        double[] o_bar = new double[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            string? s_foo = null;

                            if (o_foo[i] != null)
                            {
                                double d_foo = Convert.ToDouble(o_foo[i]);
                                s_foo = d_foo.ToString("0.00000000000000000", new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                            }

                            o_bar[i] = (double?)this.CastObjectFromString(s_foo, s_primitiveArrayType) ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.Int16"))
                    {
                        short[] o_bar = new short[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (short?)this.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.Int32"))
                    {
                        int[] o_bar = new int[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (int?)this.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.Int64"))
                    {
                        long[] o_bar = new long[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (long?)this.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.UInt16"))
                    {
                        ushort[] o_bar = new ushort[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (ushort?)this.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.UInt32"))
                    {
                        uint[] o_bar = new uint[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (uint?)this.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.UInt64"))
                    {
                        ulong[] o_bar = new ulong[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (ulong?)this.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.String"))
                    {
                        string[] o_bar = new string[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (string?)this.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? string.Empty;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.DateTime"))
                    {
                        DateTime[] o_bar = new DateTime[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (DateTime?)this.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.TimeSpan"))
                    {
                        TimeSpan[] o_bar = new TimeSpan[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (TimeSpan?)this.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.Decimal"))
                    {
                        decimal[] o_bar = new decimal[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            string? s_foo = null;

                            if (o_foo[i] != null)
                            {
                                decimal m_foo = Convert.ToDecimal(o_foo[i]);
                                s_foo = m_foo.ToString("0.00000000000000000000000000000", new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                            }

                            o_bar[i] = (decimal?)this.CastObjectFromString(s_foo, s_primitiveArrayType) ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.Object"))
                    {
                        object[] o_bar = new object[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = this.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? new object();
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar);
                    }
                    else
                    {
                        throw new ArgumentException("Invalid type [" + s_primitiveArrayType + "] for field[" + p_s_mapping + "] and object [" + p_o_object?.GetType().FullName + "]");
                    }
                }
            }
        }

        /* Internal Classes */

        /// <summary>
        /// Enumeration of valid xsd types
        /// </summary>
        public enum XSDType
        {
            Element, ComplexType, Sequence, Attribute, Choice, SimpleType, SimpleContent, Restriction, Extension, RestrictionItem
        }

        /// <summary>
        /// Enumeration of valid xml types
        /// </summary>
        public enum XMLType
        {
            ElementNoAttributes, ElementWithAttributes, BeginNoAttributes, BeginWithAttributes, Close, Empty, EmptyWithAttributes
        }

        /// <summary>
        /// Encapsulation of a schema's xsd element
        /// </summary>
        public class XSDElement
        {

            /* Fields */

            /* Properties */

            public string Name { get; set; }
            public string Type { get; set; }
            public string Mapping { get; set; }
            public string Reference { get; set; }
            public int MinOccurs { get; set; } = 1;
            public int MaxOccurs { get; set; } = 1;
            public bool Choice { get; set; }
            public bool Restriction { get; set; }
            public bool IsArray { get; set; }
            public int SequenceMinOccurs { get; set; } = 1;
            public int SequenceMaxOccurs { get; set; } = 1;
            public bool SimpleType { get; set; }
            public bool SimpleContent { get; set; }
            public List<XSDAttribute> Attributes { get; }
            public List<XSDElement> Children { get; }
            public List<XSDRestriction> Restrictions { get; }

            /* Methods */

            /// <summary>
            /// XSDElement constructor, no choice flag [= false], no max. occur value [= 1], no min. occur value [= 1], no mapping value [= null], no type value [= null], no name value [= null]
            /// </summary>
            public XSDElement() : this("", "", "", 1, 1, false)
            {

            }

            /// <summary>
            /// XSDElement constructor, no choice flag [= false], no max. occur value [= 1], no min. occur value [= 1], no mapping value [= null], no type value [= null]
            /// </summary>
            /// <param name="p_s_name">name of xsd element</param>
            public XSDElement(string p_s_name) : this(p_s_name, "", "", 1, 1, false)
            {

            }

            /// <summary>
            /// XSDElement constructor, no choice flag [= false], no max. occur value [= 1], no min. occur value [= 1], no mapping value [= null]
            /// </summary>
            /// <param name="p_s_name">name of xsd element</param>
            /// <param name="p_s_type">type of xsd element</param>
            public XSDElement(string p_s_name, string p_s_type) : this(p_s_name, p_s_type, "", 1, 1, false)
            {

            }

            /// <summary>
            /// XSDElement constructor, no choice flag [= false], no max. occur value [= 1], no min. occur value [= 1]
            /// </summary>
            /// <param name="p_s_name">name of xsd element</param>
            /// <param name="p_s_type">type of xsd element</param>
            /// <param name="p_s_mapping">mapping of xsd element</param>
            public XSDElement(string p_s_name, string p_s_type, string p_s_mapping) : this(p_s_name, p_s_type, p_s_mapping, 1, 1, false)
            {

            }

            /// <summary>
            /// XSDElement constructor, no choice flag [= false], no max. occur value [= 1]
            /// </summary>
            /// <param name="p_s_name">name of xsd element</param>
            /// <param name="p_s_type">type of xsd element</param>
            /// <param name="p_s_mapping">mapping of xsd element</param>
            /// <param name="p_i_minOccurs">min. occur value of xsd element</param>
            public XSDElement(string p_s_name, string p_s_type, string p_s_mapping, int p_i_minOccurs) : this(p_s_name, p_s_type, p_s_mapping, p_i_minOccurs, 1, false)
            {

            }

            /// <summary>
            /// XSDElement constructor, no choice flag [= false]
            /// </summary>
            /// <param name="p_s_name">name of xsd element</param>
            /// <param name="p_s_type">type of xsd element</param>
            /// <param name="p_s_mapping">mapping of xsd element</param>
            /// <param name="p_i_minOccurs">min. occur value of xsd element</param>
            /// <param name="p_i_maxOccurs">max. occur value of xsd element</param>
            public XSDElement(string p_s_name, string p_s_type, string p_s_mapping, int p_i_minOccurs, int p_i_maxOccurs) : this(p_s_name, p_s_type, p_s_mapping, p_i_minOccurs, p_i_maxOccurs, false)
            {

            }

            /// <summary>
            /// XSDElement constructor
            /// </summary>
            /// <param name="p_s_name">name of xsd element</param>
            /// <param name="p_s_type">type of xsd element</param>
            /// <param name="p_s_mapping">mapping of xsd element</param>
            /// <param name="p_i_minOccurs">min. occur value of xsd element</param>
            /// <param name="p_i_maxOccurs">max. occur value of xsd element</param>
            /// <param name="p_b_choice">choice flag of xsd element</param>
            public XSDElement(string p_s_name, string p_s_type, string p_s_mapping, int p_i_minOccurs, int p_i_maxOccurs, bool p_b_choice) : this(p_s_name, p_s_type, p_s_mapping, p_i_minOccurs, p_i_maxOccurs, p_b_choice, false)
            {

            }

            /// <summary>
            /// XSDElement constructor
            /// </summary>
            /// <param name="p_s_name">name of xsd element</param>
            /// <param name="p_s_type">type of xsd element</param>
            /// <param name="p_s_mapping">mapping of xsd element</param>
            /// <param name="p_i_minOccurs">min. occur value of xsd element</param>
            /// <param name="p_i_maxOccurs">max. occur value of xsd element</param>
            /// <param name="p_b_choice">choice flag of xsd element</param>
            public XSDElement(string p_s_name, string p_s_type, string p_s_mapping, int p_i_minOccurs, int p_i_maxOccurs, bool p_b_choice, bool p_b_isArray)
            {
                this.Attributes = [];
                this.Children = [];
                this.Restrictions = [];
                this.Name = p_s_name;
                this.Type = p_s_type;
                this.Mapping = p_s_mapping;
                this.MinOccurs = p_i_minOccurs;
                this.MaxOccurs = p_i_maxOccurs;
                this.Choice = p_b_choice;
                this.IsArray = p_b_isArray;
                this.Reference = "";
            }

            /// <summary>
            /// Checks if this xs:element object is equal to another xs:element object
            /// </summary>
            /// <param name="p_o_xsdElement">xs:element object for comparison</param>
            /// <returns>true - equal, false - not equal</returns>
            public bool IsEqual(XSDElement p_o_xsdElement)
            {
                if (this.Attributes.Count != p_o_xsdElement.Attributes.Count)
                {
                    return false;
                }

                if (this.Children.Count != p_o_xsdElement.Children.Count)
                {
                    return false;
                }

                if (this.Restrictions.Count != p_o_xsdElement.Restrictions.Count)
                {
                    return false;
                }

                for (int i = 0; i < this.Attributes.Count; i++)
                {
                    if (!this.Attributes[i].IsEqual(p_o_xsdElement.Attributes[i]))
                    {
                        return false;
                    }
                }

                for (int i = 0; i < this.Children.Count; i++)
                {
                    if (!this.Children[i].IsEqual(p_o_xsdElement.Children[i]))
                    {
                        return false;
                    }
                }

                for (int i = 0; i < this.Restrictions.Count; i++)
                {
                    if (!this.Restrictions[i].IsEqual(p_o_xsdElement.Restrictions[i]))
                    {
                        return false;
                    }
                }

                if (
                    (this.Name.Equals(p_o_xsdElement.Name)) &&
                    (this.Type.Equals(p_o_xsdElement.Type)) &&
                    (this.Mapping.Equals(p_o_xsdElement.Mapping)) &&
                    (this.Reference.Equals(p_o_xsdElement.Reference)) &&
                    (this.Choice == p_o_xsdElement.Choice) &&
                    (this.Restriction == p_o_xsdElement.Restriction) &&
                    (this.IsArray == p_o_xsdElement.IsArray) &&
                    (this.SimpleType == p_o_xsdElement.SimpleType) &&
                    (this.SimpleContent == p_o_xsdElement.SimpleContent)
                )
                {
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Clones current xsd element object
            /// </summary>
            public XSDElement Clone()
            {
                XSDElement o_clone = new()
                {
                    Name = this.Name,
                    Type = this.Type,
                    Mapping = this.Mapping,
                    Reference = this.Reference,
                    MinOccurs = this.MinOccurs,
                    MaxOccurs = this.MaxOccurs,
                    Choice = this.Choice,
                    Restriction = this.Restriction,
                    IsArray = this.IsArray,
                    SequenceMinOccurs = this.SequenceMinOccurs,
                    SequenceMaxOccurs = this.SequenceMaxOccurs,
                    SimpleType = this.SimpleType,
                    SimpleContent = this.SimpleContent
                };

                foreach (XSDAttribute o_attribute in this.Attributes)
                {
                    o_clone.Attributes.Add(o_attribute.Clone());
                }

                foreach (XSDElement o_element in this.Children)
                {
                    o_clone.Children.Add(o_element.Clone());
                }

                foreach (XSDRestriction o_restriction in this.Restrictions)
                {
                    o_clone.Restrictions.Add(o_restriction.Clone());
                }

                return o_clone;
            }

            /// <summary>
            /// Returns each field/property of xml/xsd element with name and value, separated by a pipe '|'
            /// </summary>
            override public string ToString()
            {
                string s_foo = "XSDElement: ";

                foreach (System.Reflection.PropertyInfo o_property in this.GetType().GetProperties())
                {
                    string s_value = "";

                    try
                    {
                        if ((o_property.PropertyType.IsGenericType) && (o_property.PropertyType.GetGenericTypeDefinition() == typeof(List<>)))
                        {
                            if (o_property.PropertyType.GenericTypeArguments[0] == typeof(XSDAttribute))
                            {
                                List<XSDAttribute> a_foo = (List<XSDAttribute>)(o_property.GetValue(this) ?? new List<XSDAttribute>());

                                if (a_foo.Count > 0)
                                {
                                    s_foo += ForestNET.Lib.IO.File.NEWLINE;

                                    foreach (XSDAttribute o_foo in a_foo)
                                    {
                                        s_foo += o_foo.ToString() + ForestNET.Lib.IO.File.NEWLINE;
                                    }
                                }
                                else
                                {
                                    s_foo += "null" + ForestNET.Lib.IO.File.NEWLINE;
                                }
                            }
                            else if (o_property.PropertyType.GenericTypeArguments[0] == typeof(XSDElement))
                            {
                                List<XSDElement> a_foo = (List<XSDElement>)(o_property.GetValue(this) ?? new List<XSDElement>());

                                if (a_foo.Count > 0)
                                {
                                    s_foo += ForestNET.Lib.IO.File.NEWLINE;

                                    foreach (XSDElement o_foo in a_foo)
                                    {
                                        s_foo += o_foo.ToString() + ForestNET.Lib.IO.File.NEWLINE;
                                    }
                                }
                                else
                                {
                                    s_foo += "null" + ForestNET.Lib.IO.File.NEWLINE;
                                }
                            }
                            else if (o_property.PropertyType.GenericTypeArguments[0] == typeof(XSDRestriction))
                            {
                                List<XSDRestriction> a_foo = (List<XSDRestriction>)(o_property.GetValue(this) ?? new List<XSDRestriction>());

                                if (a_foo.Count > 0)
                                {
                                    s_foo += ForestNET.Lib.IO.File.NEWLINE;

                                    foreach (XSDRestriction o_foo in a_foo)
                                    {
                                        s_foo += o_foo.ToString() + ForestNET.Lib.IO.File.NEWLINE;
                                    }
                                }
                                else
                                {
                                    s_foo += "null" + ForestNET.Lib.IO.File.NEWLINE;
                                }
                            }
                        }
                        else
                        {
                            s_value = o_property.GetValue(this)?.ToString() ?? "null";
                        }
                    }
                    catch (Exception)
                    {
                        s_value = "null";
                    }

                    s_foo += o_property.Name + " = " + s_value + "|";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 1);

                return s_foo;
            }
        }

        /// <summary>
        /// Encapsulation of a schema's xsd attribute
        /// </summary>
        public class XSDAttribute
        {

            /* Fields */

            /* Properties */

            public string Name { get; set; }
            public string Type { get; set; }
            public string Mapping { get; set; }
            public bool Required { get; set; }
            public string Default { get; set; }
            public string Fixed { get; set; }
            public string Reference { get; set; }
            public bool Restriction { get; set; }
            public bool SimpleType { get; set; }
            public List<XSDRestriction> Restrictions { get; }

            /* Methods */

            /// <summary>
            /// XSDAttribute constructor, no name value [= null], no type value [= null], no mapping value [= null], no required flag [= false], no default value [= null], no fixed value [= null]
            /// </summary>
            public XSDAttribute() : this("", "", "", false, "", "")
            {

            }

            /// <summary>
            /// XSDAttribute constructor, no type value [= null], no mapping value [= null], no required flag [= false], no default value [= null], no fixed value [= null]
            /// </summary>
            /// <param name="p_s_name">name of xsd element attribute</param>
            public XSDAttribute(string p_s_name) : this(p_s_name, "", "", false, "", "")
            {

            }

            /// <summary>
            /// XSDAttribute constructor, no mapping value [= null], no required flag [= false], no default value [= null], no fixed value [= null]
            /// </summary>
            /// <param name="p_s_name">name of xsd element attribute</param>
            /// <param name="p_s_type">type of xsd element attribute</param>
            public XSDAttribute(string p_s_name, string p_s_type) : this(p_s_name, p_s_type, "", false, "", "")
            {

            }

            /// <summary>
            /// XSDAttribute constructor, no required flag [= false], no default value [= null], no fixed value [= null]
            /// </summary>
            /// <param name="p_s_name">name of xsd element attribute</param>
            /// <param name="p_s_type">type of xsd element attribute</param>
            /// <param name="p_s_mapping">mapping of xsd element attribute</param>
            public XSDAttribute(string p_s_name, string p_s_type, string p_s_mapping) : this(p_s_name, p_s_type, p_s_mapping, false, "", "")
            {

            }

            /// <summary>
            /// XSDAttribute constructor, no default value [= null], no fixed value [= null]
            /// </summary>
            /// <param name="p_s_name">name of xsd element attribute</param>
            /// <param name="p_s_type">type of xsd element attribute</param>
            /// <param name="p_s_mapping">mapping of xsd element attribute</param>
            /// <param name="p_b_required">required flag of xsd element attribute</param>
            public XSDAttribute(string p_s_name, string p_s_type, string p_s_mapping, bool p_b_required) : this(p_s_name, p_s_type, p_s_mapping, p_b_required, "", "")
            {

            }

            /// <summary>
            /// XSDAttribute constructor, no fixed value [= null]
            /// </summary>
            /// <param name="p_s_name">name of xsd element attribute</param>
            /// <param name="p_s_type">type of xsd element attribute</param>
            /// <param name="p_s_mapping">mapping of xsd element attribute</param>
            /// <param name="p_b_required">required flag of xsd element attribute</param>
            /// <param name="p_s_default">default value of xsd element attribute</param>
            public XSDAttribute(string p_s_name, string p_s_type, string p_s_mapping, bool p_b_required, string p_s_default) : this(p_s_name, p_s_type, p_s_mapping, p_b_required, p_s_default, "")
            {

            }

            /// <summary>
            /// XSDAttribute constructor
            /// </summary>
            /// <param name="p_s_name">name of xsd element attribute</param>
            /// <param name="p_s_type">type of xsd element attribute</param>
            /// <param name="p_s_mapping">mapping of xsd element attribute</param>
            /// <param name="p_b_required">required flag of xsd element attribute</param>
            /// <param name="p_s_default">default value of xsd element attribute</param>
            /// <param name="p_s_fixed">fixed constant value of xsd element attribute</param>
            public XSDAttribute(string p_s_name, string p_s_type, string p_s_mapping, bool p_b_required, string p_s_default, string p_s_fixed)
            {
                this.Restrictions = [];
                this.Name = p_s_name;
                this.Type = p_s_type;
                this.Mapping = p_s_mapping;
                this.Required = p_b_required;
                this.Default = p_s_default;
                this.Fixed = p_s_fixed;
                this.Reference = "";
            }

            /// <summary>
            /// Checks if this xs:attribute object is equal to another xs:attribute object
            /// </summary>
            /// <param name="p_o_xsdAttribute">xs:attribute object for comparison</param>
            /// <returns>true - equal, false - not equal</returns>
            public bool IsEqual(XSDAttribute p_o_xsdAttribute)
            {
                if (this.Restrictions.Count != p_o_xsdAttribute.Restrictions.Count)
                {
                    return false;
                }

                for (int i = 0; i < this.Restrictions.Count; i++)
                {
                    if (!this.Restrictions[i].IsEqual(p_o_xsdAttribute.Restrictions[i]))
                    {
                        return false;
                    }
                }

                if (
                    (this.Name.Equals(p_o_xsdAttribute.Name)) &&
                    (this.Type.Equals(p_o_xsdAttribute.Type)) &&
                    (this.Mapping.Equals(p_o_xsdAttribute.Mapping)) &&
                    (this.Required == p_o_xsdAttribute.Required) &&
                    (this.Default.Equals(p_o_xsdAttribute.Default)) &&
                    (this.Fixed.Equals(p_o_xsdAttribute.Fixed)) &&
                    (this.Reference.Equals(p_o_xsdAttribute.Reference)) &&
                    (this.Restriction == p_o_xsdAttribute.Restriction) &&
                    (this.SimpleType == p_o_xsdAttribute.SimpleType)
                )
                {
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Clones current xsd attribute object
            /// </summary>
            public XSDAttribute Clone()
            {
                XSDAttribute o_clone = new()
                {
                    Name = this.Name,
                    Type = this.Type,
                    Mapping = this.Mapping,
                    Required = this.Required,
                    Default = this.Default,
                    Fixed = this.Fixed,
                    Reference = this.Reference,
                    Restriction = this.Restriction,
                    SimpleType = this.SimpleType
                };

                foreach (XSDRestriction o_restriction in this.Restrictions)
                {
                    o_clone.Restrictions.Add(o_restriction.Clone());
                }

                return o_clone;
            }

            /// <summary>
            /// Returns each field/property of xml/xsd attribute with name and value, separated by a pipe '|'
            /// </summary>
            override public string ToString()
            {
                string s_foo = ForestNET.Lib.IO.File.NEWLINE + "\t" + "XSDAttribute: ";

                foreach (System.Reflection.PropertyInfo o_property in this.GetType().GetProperties())
                {
                    string s_value = "";

                    try
                    {
                        if ((o_property.PropertyType.IsGenericType) && (o_property.PropertyType.GetGenericTypeDefinition() == typeof(List<>)))
                        {
                            if (o_property.PropertyType.GenericTypeArguments[0] == typeof(XSDRestriction))
                            {
                                List<XSDRestriction> a_foo = (List<XSDRestriction>)(o_property.GetValue(this) ?? new List<XSDRestriction>());

                                if (a_foo.Count > 0)
                                {
                                    s_foo += ForestNET.Lib.IO.File.NEWLINE;

                                    foreach (XSDRestriction o_foo in a_foo)
                                    {
                                        s_foo += o_foo.ToString() + ForestNET.Lib.IO.File.NEWLINE;
                                    }
                                }
                                else
                                {
                                    s_foo += "null" + ForestNET.Lib.IO.File.NEWLINE;
                                }
                            }
                        }
                        else
                        {
                            s_value = o_property.GetValue(this)?.ToString() ?? "null";
                        }
                    }
                    catch (Exception)
                    {
                        s_value = "null";
                    }

                    s_foo += o_property.Name + " = " + s_value + "|";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 1);

                return s_foo;
            }
        }

        /// <summary>
        /// Encapsulation of a schema's xsd element restrictions
        /// </summary>
        public class XSDRestriction
        {

            /* Fields */

            /* Properties */

            public string Name { get; set; }
            public string StrValue { get; set; }
            public int IntValue { get; set; }

            /* Methods */

            /// <summary>
            /// XSDRestriction constructor, no name value [= null], no string value [= null] and no integer value [= 0]
            /// </summary>
            public XSDRestriction() : this("", "", 0)
            {

            }

            /// <summary>
            /// XSDRestriction constructor, no string value [= null] and no integer value [= 0]
            /// </summary>
            /// <param name="p_s_name">name of xml element restriction</param>
            public XSDRestriction(string p_s_name) : this(p_s_name, "", 0)
            {

            }

            /// <summary>
            /// XSDRestriction constructor, no integer value [= 0]
            /// </summary>
            /// <param name="p_s_name">name of xml element restriction</param>
            /// <param name="p_s_strValue">string value of restriction</param>
            public XSDRestriction(string p_s_name, string p_s_strValue) : this(p_s_name, p_s_strValue, 0)
            {

            }

            /// <summary>
            /// XSDRestriction constructor
            /// </summary>
            /// <param name="p_s_name">name of xml element restriction</param>
            /// <param name="p_s_strValue">string value of restriction</param>
            /// <param name="p_i_intValue">integer value of restriction</param>
            public XSDRestriction(string p_s_name, string p_s_strValue, int p_i_intValue)
            {
                this.Name = p_s_name;
                this.StrValue = p_s_strValue;
                this.IntValue = p_i_intValue;
            }

            /// <summary>
            /// Checks if this xs:restriction object is equal to another xs:restriction object
            /// </summary>
            /// <param name="p_o_xsdRestriction">xs:restriction object for comparison</param>
            /// <returns>true - equal, false - not equal</returns>
            public bool IsEqual(XSDRestriction p_o_xsdRestriction)
            {
                if ((this.Name.Equals(p_o_xsdRestriction.Name)) && (this.StrValue.Equals(p_o_xsdRestriction.StrValue)) && (this.IntValue == p_o_xsdRestriction.IntValue))
                {
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Clones current xsd restriction object
            /// </summary>
            public XSDRestriction Clone()
            {
                return new XSDRestriction(this.Name, this.StrValue, this.IntValue);
            }

            /// <summary>
            /// Returns each field/property of xml/xsd element restriction with name and value, separated by a pipe '|'
            /// </summary>
            override public string ToString()
            {
                string s_foo = ForestNET.Lib.IO.File.NEWLINE + "\t\t" + "XSDRestriction: ";

                foreach (System.Reflection.PropertyInfo o_property in this.GetType().GetProperties())
                {
                    string s_value = "";

                    try
                    {
                        s_value = o_property.GetValue(this)?.ToString() ?? "null";
                    }
                    catch (Exception)
                    {
                        s_value = "null";
                    }

                    s_foo += o_property.Name + " = " + s_value + "|";
                }

                s_foo = s_foo.Substring(0, s_foo.Length - 1);

                return s_foo;
            }
        }
    }
}
