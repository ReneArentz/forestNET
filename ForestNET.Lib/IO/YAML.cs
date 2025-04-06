namespace ForestNET.Lib.IO
{
    /// <summary>
    /// YAML class to encode and decode c# objects to yaml files with help of a yaml schema file/data.
    /// access to object fields can be directly on public fields or with public property methods (getXX setXX) on private fields.
    /// NOTE: mostly only primitive types supported for encoding and decoding, only supporting ISO-8601 UTC timestamps within yaml files.
    /// </summary>
    public class YAML
    {

        /* Fields */

        private YAMLElement? o_currentElement;
        private YAMLElement? o_schema;
        private string s_lineBreak = ForestNET.Lib.IO.File.NEWLINE;
        private YAMLElement? o_definitions;
        private readonly List<string>? a_references;
        private YAMLElement? o_properties;
        private int i_level;

        private int i_amountWhiteSpacesindentation = 4;
        private bool b_firstLevelCollectionElementsOnly;
        private string s_stringQuote = "\"";

        /* Properties */

        public YAMLElement? Root
        {
            get; set;
        }

        /// <summary>
        /// Determine line break characters for reading and writing yaml files
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

        public int AmountWhiteSpacesindentation
        {
            get
            {
                return this.i_amountWhiteSpacesindentation;
            }
            set
            {
                if (value < 2)
                {
                    throw new ArgumentException("Amount of white spaces for indentation must be at least '2', but was set to '" + value + "'");
                }

                this.i_amountWhiteSpacesindentation = value;
                ForestNET.Lib.Global.ILogConfig("updates amount of white spaces indentation to '" + this.i_amountWhiteSpacesindentation + "'");
            }
        }

        public string StringQuote
        {
            get
            {
                return this.s_stringQuote;
            }
            set
            {
                if (value.Length != 1)
                {
                    throw new ArgumentException("string quote must be '1' character, but has a length of '" + value.Length + "'");
                }

                this.s_stringQuote = value;
                ForestNET.Lib.Global.ILogConfig("updates string quote to '" + this.s_stringQuote + "'");

            }
        }

        /* Methods */

        /// <summary>
        /// Empty YAML constructor
        /// </summary>
        public YAML()
        {
            this.AmountWhiteSpacesindentation = 4;
            this.StringQuote = "\"";
            this.LineBreak = ForestNET.Lib.IO.File.NEWLINE;
            this.UseProperties = false;
            this.i_level = 0;

            this.a_references = [];

            this.Root = new YAMLElement("Root");
            this.o_schema = this.Root;
        }

        /// <summary>
        /// YAML constructor, giving file lines of schema as dynamic list and further instructions for encoding and decoding yaml data
        /// </summary>
        /// <param name="p_a_yamlSchemaFileLines">file lines of schema as dynamic list</param>
        /// <exception cref="ArgumentException">value/structure within yaml schema invalid</exception>
        /// <exception cref="ArgumentNullException">yaml schema, root node is null</exception>
        public YAML(List<string> p_a_yamlSchemaFileLines) : this(p_a_yamlSchemaFileLines, 4)
        {

        }

        /// <summary>
        /// YAML constructor, giving file lines of schema as dynamic list and further instructions for encoding and decoding yaml data.
        /// quote string values with '"' character.
        /// </summary>
        /// <param name="p_a_yamlSchemaFileLines">file lines of schema as dynamic list</param>
        /// <param name="p_i_amountWhiteSpacesindentation">define amount of white spaces which are used as indentation for yaml file, read and write</param>
        /// <exception cref="ArgumentException">value/structure within yaml schema invalid</exception>
        /// <exception cref="ArgumentNullException">yaml schema, root node is null</exception>
        public YAML(List<string> p_a_yamlSchemaFileLines, int p_i_amountWhiteSpacesindentation) : this(p_a_yamlSchemaFileLines, p_i_amountWhiteSpacesindentation, "\"")
        {

        }

        /// <summary>
        /// YAML constructor, giving file lines of schema as dynamic list and further instructions for encoding and decoding yaml data
        /// </summary>
        /// <param name="p_a_yamlSchemaFileLines">file lines of schema as dynamic list</param>
        /// <param name="p_i_amountWhiteSpacesindentation">define amount of white spaces which are used as indentation for yaml file, read and write</param>
        /// <param name="p_s_stringQuote">define character for quoting string values within yaml content</param>
        /// <exception cref="ArgumentException">value/structure within yaml schema invalid</exception>
        /// <exception cref="ArgumentNullException">yaml schema, root node is null</exception>
        public YAML(List<string> p_a_yamlSchemaFileLines, int p_i_amountWhiteSpacesindentation, string p_s_stringQuote)
        {
            this.AmountWhiteSpacesindentation = p_i_amountWhiteSpacesindentation;
            this.StringQuote = p_s_stringQuote;
            this.LineBreak = ForestNET.Lib.IO.File.NEWLINE;
            this.UseProperties = false;
            this.i_level = 0;

            this.a_references = [];

            List<string> a_fileLines = this.ValidateYAML(p_a_yamlSchemaFileLines);

            ForestNET.Lib.Global.ILogConfig("yaml schema file lines validated");

            /* check if root is null */
            if ((a_fileLines.Count == 0) || ((a_fileLines.Count == 1) && (a_fileLines[0].Equals("null"))))
            {
                throw new ArgumentException("Schema file is null", nameof(p_a_yamlSchemaFileLines));
            }

            /* reset level */
            this.i_level = 0;

            /* create new root if is null */
            if (this.Root == null)
            {
                this.Root = new YAMLElement("Root");
            }

            /* parse yaml schema */
            this.ParseYAML(1, a_fileLines.Count, a_fileLines, 0, this.Root, true);

            ForestNET.Lib.Global.ILogConfig("yaml schema parsed");

            /* set schema element with constructor input, root is unparsed schema */
            this.SetSchema(true);
        }

        /// <summary>
        /// YAML constructor, giving a schema yaml element object and further instructions for encoding and decoding yaml data.
        /// amount of white spaces which are used as indentation for yaml file is '4'.
        /// quote string values with '"' character.
        /// </summary>
        /// <param name="p_o_schemaRoot">yaml element object as root schema node</param>
        /// <exception cref="ArgumentException">invalid parameters for constructor</exception>
        public YAML(YAMLElement? p_o_schemaRoot) : this(p_o_schemaRoot, 4)
        {

        }

        /// <summary>
        /// YAML constructor, giving a schema yaml element object and further instructions for encoding and decoding yaml data.
        /// amount of white spaces which are used as indentation for yaml file is '4'.
        /// </summary>
        /// <param name="p_o_schemaRoot">yaml element object as root schema node</param>
        /// <param name="p_i_amountWhiteSpacesindentation">define amount of white spaces which are used as indentation for yaml file, read and write</param>
        /// <exception cref="ArgumentException">invalid parameters for constructor</exception>
        public YAML(YAMLElement? p_o_schemaRoot, int p_i_amountWhiteSpacesindentation) : this(p_o_schemaRoot, p_i_amountWhiteSpacesindentation, "\"")
        {

        }

        /// <summary>
        /// YAML constructor, giving a schema yaml element object and further instructions for encoding and decoding yaml data
        /// </summary>
        /// <param name="p_o_schemaRoot">yaml element object as root schema node</param>
        /// <param name="p_i_amountWhiteSpacesindentation">define amount of white spaces which are used as indentation for yaml file, read and write</param>
        /// <param name="p_s_stringQuote">define character for quoting string values within yaml content</param>
        /// <exception cref="ArgumentException">invalid parameters for constructor</exception>
        public YAML(YAMLElement? p_o_schemaRoot, int p_i_amountWhiteSpacesindentation, string p_s_stringQuote)
        {
            this.AmountWhiteSpacesindentation = p_i_amountWhiteSpacesindentation;
            this.StringQuote = p_s_stringQuote;
            this.LineBreak = ForestNET.Lib.IO.File.NEWLINE;
            this.UseProperties = false;
            this.i_level = 0;

            this.a_references = [];

            this.Root = p_o_schemaRoot ?? throw new ArgumentException("yaml element parameter for schema is null");

            /* set schema element with constructor input, root is already parsed schema */
            this.SetSchema(false);
        }

        /// <summary>
        /// YAML constructor, giving a schema file and further instructions for encoding and decoding yaml data.
        /// amount of white spaces which are used as indentation for yaml file is '4'.
        /// quote string values with '"' character.
        /// </summary>
        /// <param name="p_s_file">full-path to yaml schema file</param>
        /// <exception cref="ArgumentException">value/structure within yaml schema file invalid</exception>
        /// <exception cref="ArgumentNullException">yaml schema, root node is null</exception>
        /// <exception cref="System.IO.IOException">cannot access or open yaml file and it's content</exception>
        public YAML(string p_s_file) : this(p_s_file, 4)
        {

        }

        /// <summary>
        /// YAML constructor, giving a schema file and further instructions for encoding and decoding yaml data.
        /// quote string values with '"' character.
        /// </summary>
        /// <param name="p_s_file">full-path to yaml schema file</param>
        /// <param name="p_i_amountWhiteSpacesindentation">define amount of white spaces which are used as indentation for yaml file, read and write</param>
        /// <exception cref="ArgumentException">value/structure within yaml schema file invalid</exception>
        /// <exception cref="ArgumentNullException">yaml schema, root node is null</exception>
        /// <exception cref="System.IO.IOException">cannot access or open yaml file and it's content</exception>
        public YAML(string p_s_file, int p_i_amountWhiteSpacesindentation) : this(p_s_file, p_i_amountWhiteSpacesindentation, "\"")
        {

        }

        /// <summary>
        /// YAML constructor, giving a schema file and further instructions for encoding and decoding yaml data
        /// </summary>
        /// <param name="p_s_file">full-path to yaml schema file</param>
        /// <param name="p_i_amountWhiteSpacesindentation">define amount of white spaces which are used as indentation for yaml file, read and write</param>
        /// <param name="p_s_stringQuote">define character for quoting string values within yaml content</param>
        /// <exception cref="ArgumentException">value/structure within yaml schema file invalid</exception>
        /// <exception cref="ArgumentNullException">yaml schema, root node is null</exception>
        /// <exception cref="System.IO.IOException">cannot access or open yaml file and it's content</exception>
        public YAML(string p_s_file, int p_i_amountWhiteSpacesindentation, string p_s_stringQuote)
        {
            this.AmountWhiteSpacesindentation = p_i_amountWhiteSpacesindentation;
            this.StringQuote = p_s_stringQuote;
            this.LineBreak = ForestNET.Lib.IO.File.NEWLINE;
            this.UseProperties = false;
            this.i_level = 0;

            this.a_references = [];

            List<string> a_fileLines = this.ValidateYAML(p_s_file);

            ForestNET.Lib.Global.ILogConfig("yaml schema file validated");

            /* check if root is null */
            if ((a_fileLines.Count == 0) || ((a_fileLines.Count == 1) && (a_fileLines[0].Equals("null"))))
            {
                throw new ArgumentException("Schema file is null", nameof(p_s_file));
            }

            /* reset level */
            this.i_level = 0;

            /* create new root if is null */
            if (this.Root == null)
            {
                this.Root = new YAMLElement("Root");
            }

            /* parse yaml schema */
            this.ParseYAML(1, a_fileLines.Count, a_fileLines, 0, this.Root, true);

            ForestNET.Lib.Global.ILogConfig("yaml schema parsed");

            /* set schema element with constructor input, root is unparsed schema */
            this.SetSchema(true);
        }

        /// <summary>
        /// Method to set schema elements, afterwards each yaml constructor has read their input
        /// </summary>
        /// <param name="p_b_rootIsUnparsedSchema">true - schema within Root property will be parsed</param>
        private void SetSchema(bool p_b_rootIsUnparsedSchema)
        {
            /* check if root is null */
            if (this.Root == null)
            {
                throw new NullReferenceException("Root node is null");
            }

            /* check if root has any children */
            if (this.Root.Children.Count == 0)
            {
                throw new ArgumentNullException("Root node has no children[size=" + this.Root.Children.Count + "]");
            }

            if (p_b_rootIsUnparsedSchema)
            {
                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("Unparsed YAML-Schema: " + this.s_lineBreak + this.Root);

                /* reset level */
                this.i_level = 0;

                /* set current element with root if it is empty */
                if (this.o_currentElement == null)
                {
                    this.o_currentElement = this.Root;
                }

                this.ParseYAMLSchemaElements(this.Root);

                ForestNET.Lib.Global.ILogConfig("yaml schema elements parsed");

                /* reset children of root */
                this.Root.Children.Clear();

                if (this.o_properties != null)
                {
                    /* properties cannot have one child and 'Reference' */
                    if ((this.o_properties.Children.Count > 0) && (this.o_properties.Reference != null))
                    {
                        throw new ArgumentException("Properties after parsing yaml schema cannot have one child and 'Reference'");
                    }

                    /* check if properties has one child */
                    if (this.o_properties.Children.Count > 0)
                    {
                        /* add all 'properties' children to root */
                        foreach (YAMLElement o_yamlElement in this.o_properties.Children)
                        {
                            this.Root.Children.Add(o_yamlElement);
                        }

                        ForestNET.Lib.Global.ILogConfig("added all 'properties' from yaml schema to root");
                    }
                    else if (this.o_properties.Reference != null)
                    { /* we have 'Reference' in properties */
                        /* set properties 'Reference' as root 'Reference' */
                        this.Root.Reference = this.o_properties.Reference;

                        ForestNET.Lib.Global.ILogConfig("set 'reference' from yaml schema as root 'reference'");
                    }
                }
            }

            if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("Parsed YAML-Schema: " + this.s_lineBreak + this.Root);

            this.o_schema = this.Root;

            ForestNET.Lib.Global.ILogConfig("set yaml root element as schema element");
        }

        /// <summary>
        /// Returns root element of yaml schema as string output and all of its children
        /// </summary>
        override public string ToString()
        {
            string s_foo = "";

            s_foo += this.Root;

            return s_foo;
        }

        /// <summary>
        /// Generate indentation string for yaml specification
        /// </summary>
        /// <returns>indentation string</returns>
        private string PrintIndentation()
        {
            string s_foo = "";

            for (int i = 0; i < this.i_level; i++)
            {
                if ((i == 0) && (this.b_firstLevelCollectionElementsOnly))
                {
                    s_foo += "  ";
                }
                else
                {
                    for (int j = 0; j < this.i_amountWhiteSpacesindentation; j++)
                    {
                        s_foo += " ";
                    }
                }
            }

            return s_foo;
        }

        /* validate YAML */

        /// <summary>
        /// Validate yaml file if values and structure are correct, otherwise throw ArgumentException
        /// </summary>
        /// <param name="p_s_yamlFile">full-path to yaml file</param>
        /// <returns>validated yaml lines</returns>
        /// <exception cref="System.IO.IOException">cannot access or open yaml file and it's content</exception>
        /// <exception cref="ArgumentException">value or structure within yaml file lines invalid</exception>
        public List<string> ValidateYAML(string p_s_yamlFile)
        {
            /* check if file exists */
            if (!ForestNET.Lib.IO.File.Exists(p_s_yamlFile))
            {
                throw new ArgumentException("File[" + p_s_yamlFile + "] does not exist.");
            }

            /* open yaml file */
            ForestNET.Lib.IO.File o_file = new(p_s_yamlFile, false);

            /* load file content into string */
            string s_fileContent = o_file.FileContent;

            ForestNET.Lib.Global.ILogConfig("yaml file accessed and content read");

            List<string> a_fileLines =
            [
                /* load file content lines into array */
                .. s_fileContent.Split(this.s_lineBreak),
            ];

            ForestNET.Lib.Global.ILogConfig("yaml file content transferred to array");

            return this.ValidateYAML(a_fileLines);
        }

        /// <summary>
        /// Validate yaml file lines if values and structure are correct, otherwise throw ArgumentException
        /// </summary>
        /// <param name="p_a_yamlLines">yaml file lines</param>
        /// <returns>validated yaml lines</returns>
        /// <exception cref="ArgumentException">value or structure within yaml file lines invalid</exception>
        public List<string> ValidateYAML(List<string> p_a_yamlLines)
        {
            List<string> a_yamlLines = [];

            if (p_a_yamlLines == null || p_a_yamlLines.Count < 1)
            {
                throw new ArgumentException("YAML lines content is null", nameof(p_a_yamlLines));
            }

            int i_line = 1;

            /* load file content lines into array */
            foreach (string s_foo in p_a_yamlLines)
            {
                string s_line = s_foo;

                /* check if line contains any tabulator-signs */
                if (s_line.Contains('\t'))
                {
                    throw new InvalidOperationException("Line (" + i_line + ") contains 'tab'-signs which are not allowed within yaml-documents");
                }

                /* remove comments */
                if (s_line.Contains('#'))
                {
                    bool b_commentFound = false;
                    bool b_inQuotation = false;
                    bool b_escapeCharacterBefore = false;

                    /* check for unquoted comment character in line */
                    for (int i = 0; i < s_line.Length; i++)
                    {
                        if ((s_line[i] == '"') && (!b_escapeCharacterBefore))
                        {
                            if (b_inQuotation)
                            {
                                b_inQuotation = false;
                            }
                            else
                            {
                                b_inQuotation = true;
                            }
                        }
                        else if ((b_inQuotation) && (s_line[i] == '\\'))
                        {
                            b_escapeCharacterBefore = true;
                        }
                        else if ((s_line[i] == '#') && (!b_inQuotation))
                        {
                            b_commentFound = true;
                            break;
                        }

                        if ((b_inQuotation) && (s_line[i] != '\\'))
                        {
                            b_escapeCharacterBefore = false;
                        }
                    }

                    if (b_commentFound)
                    {
                        s_line = s_line.Substring(0, s_line.IndexOf('#'));
                    }
                }

                bool b_onlyWhiteSpaces = true;

                /* check if line only contains white spaces */
                for (int i = 0; i < s_line.Length; i++)
                {
                    if (s_line[i] != ' ')
                    {
                        b_onlyWhiteSpaces = false;
                        break;
                    }
                }

                /* empty line if it contains only of white spaces */
                if (b_onlyWhiteSpaces)
                {
                    s_line = "";
                }

                /* add file line to array */
                a_yamlLines.Add(s_line);

                i_line++;
            }

            /* check if yaml document starts with '---' */
            if (!a_yamlLines[0].Equals("---"))
            {
                throw new ArgumentException("YAML document must start with '---'");
            }

            /* set level for PrintIndentation to zero */
            this.i_level = 0;

            /* validate yaml file recursively */
            ValidateYAMLRecursive(1, a_yamlLines.Count, a_yamlLines, 0, false);

            return a_yamlLines;
        }

        /// <summary>
        /// Validate yaml file lines if values and structure are correct, otherwise throw ArgumentException
        /// </summary>
        /// <param name="p_i_min">pointer where to start line iteration</param>
        /// <param name="p_i_max">pointer where to end line iteration</param>
        /// <param name="p_a_lines">yaml file lines</param>
        /// <param name="p_i_whiteSpaceindentation">amount of white space indentation in yaml file</param>
        /// <param name="p_b_textBlock">true - current lines are a textblock within yaml data, false - normal yaml data</param>
        /// <exception cref="ArgumentException">value or structure within yaml file lines invalid</exception>
        private void ValidateYAMLRecursive(int p_i_min, int p_i_max, List<string> p_a_lines, int p_i_whiteSpaceindentation, bool p_b_textBlock)
        {
            /* temp variables for key and value */
            string s_key;
            string s_value = "null";

            /* iterate each line */
            for (int i_min = p_i_min; i_min < p_i_max; i_min++)
            {
                /* read line into string */
                string s_line = p_a_lines[i_min];

                ForestNET.Lib.Global.ILogFiner(s_line);

                /* check if line has any content */
                if ((s_line.Length == 0) || (s_line.Equals("...")))
                {
                    ForestNET.Lib.Global.ILogFiner("(" + (i_min + 1) + ")|NULL|");

                    /* skip lines with no content */
                    continue;
                }

                if (p_i_whiteSpaceindentation >= s_line.Length)
                {
                    throw new ArgumentException("Error in line (" + (i_min + 1) + "): Invalid indentation, yaml syntax error");
                }

                /* check if line starts with white space on expected position */
                if (s_line[p_i_whiteSpaceindentation] == ' ')
                {
                    ForestNET.Lib.Global.ILogFiner(s_line);

                    /* check if we already have a value from previous line */
                    if (!s_value.Equals("null"))
                    {
                        /* new indentation with previous value only allowed if value was reference or text block */
                        if ((s_value[0] != '&') && (!s_value.Equals("|")) && (!s_value.Equals(">")))
                        {
                            throw new ArgumentException("Error in line (" + (i_min + 1) + "): New indentation found, but parent element has already a value");
                        }
                    }

                    int i = 0;

                    /* count white spaces */
                    while (s_line[i] == ' ')
                    {
                        i++;
                    }

                    int i_max;

                    /* search for end of indentation block */
                    for (i_max = i_min + 1; i_max < p_i_max; i_max++)
                    {
                        /* read line */
                        string s_lineTemp = p_a_lines[i_max];

                        /* check if line has any content */
                        if ((s_lineTemp.Length == 0) || (s_lineTemp.Equals("...")))
                        {
                            /* skip lines with no content */
                            continue;
                        }

                        int j = 0;

                        /* count white spaces */
                        while (s_lineTemp[j] == ' ')
                        {
                            j++;
                        }

                        /* if counted white spaces matches expected position of indentation or there is a new collection element with '-'-sign as prefix */
                        if ((j == p_i_whiteSpaceindentation) || ((p_i_whiteSpaceindentation >= 2) && (s_lineTemp[p_i_whiteSpaceindentation - 2] == '-') && (s_lineTemp[p_i_whiteSpaceindentation - 1] == ' ')))
                        {
                            /* break - end of indentation block found */
                            break;
                        }
                    }

                    /* increase level for PrintIndentation */
                    this.i_level++;

                    ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "block goes from (" + (i_min + 1) + ") to (" + (i_max) + ") with indentation '" + i + "'");

                    /* start new recursion with new indentation block */
                    this.ValidateYAMLRecursive(i_min, i_max, p_a_lines, i, ((s_value.Equals("|")) || (s_value.Equals(">"))));

                    /* decrease level for PrintIndentation */
                    this.i_level--;

                    /* overwrite line pointer and continue for loop after indentation block */
                    i_min = i_max - 1;
                    continue;
                }

                string s_originalLine = s_line;

                /* cut white space indentation of line */
                if (p_i_whiteSpaceindentation > 0)
                {
                    s_line = s_line.Substring(p_i_whiteSpaceindentation);
                }

                int i_amountColon = 0;
                int i_lineLevel = 0;
                int i_lineLevelSequences = 0;
                bool b_inQuotation = false;
                bool b_escapeCharacterBefore = false;

                /* check for multiple colon characters in line */
                for (int i = 0; i < s_line.Length; i++)
                {
                    if (s_line[i] == '{')
                    {
                        i_lineLevel++;
                    }
                    else if (s_line[i] == '}')
                    {
                        i_lineLevel--;
                    }
                    else if (s_line[i] == '[')
                    {
                        i_lineLevelSequences++;
                    }
                    else if (s_line[i] == ']')
                    {
                        i_lineLevelSequences--;
                    }
                    else if ((s_line[i] == '"') && (!b_escapeCharacterBefore))
                    {
                        if (b_inQuotation)
                        {
                            b_inQuotation = false;
                        }
                        else
                        {
                            b_inQuotation = true;
                        }
                    }
                    else if ((b_inQuotation) && (s_line[i] == '\\'))
                    {
                        b_escapeCharacterBefore = true;
                    }
                    else if ((s_line[i] == ':') && (i_lineLevel == 0) && (!b_inQuotation))
                    {
                        i_amountColon++;
                    }

                    if ((b_inQuotation) && (s_line[i] != '\\'))
                    {
                        b_escapeCharacterBefore = false;
                    }
                }

                if ((i_lineLevel != 0) && (!p_b_textBlock))
                {
                    throw new ArgumentException("Error in line (" + (i_min + 1) + "): Invalid nesting with curly brackets '{', '}'");
                }

                if ((i_lineLevelSequences != 0) && (!p_b_textBlock))
                {
                    throw new ArgumentException("Error in line (" + (i_min + 1) + "): Invalid collection array with squared brackets '[', ']'");
                }

                if (i_amountColon > 1)
                {
                    throw new ArgumentException("Error in line (" + (i_min + 1) + "): A line can only contain one colon ':' character");
                }

                s_key = "null";
                s_value = "null";
                bool b_newElementForCollection = false;

                /* if there is no colon or text block mode we only have a value in line */
                if ((i_amountColon == 0) || (p_b_textBlock))
                {
                    /* assume whole line as value */
                    s_value = s_line;

                    /* if line starts with a colon we have a new collection element */
                    if (s_line.StartsWith("- "))
                    {
                        /* cut colon with white space out of line */
                        s_value = s_value.Substring(2);

                        /* set new collection element flag */
                        b_newElementForCollection = true;
                    }
                    else
                    {
                        if (!(((s_value.StartsWith("{")) && (s_value.EndsWith("}"))) || (p_b_textBlock)))
                        {
                            throw new ArgumentException("Error in line (" + (i_min + 1) + "): A line which is not a key value pair, not a collection element, no inline-object and not part of a text block is not allowed");
                        }
                    }
                }
                else if (i_amountColon == 1)
                {
                    /* split key value before colon and remove all white spaces between key value and colon */
                    s_key = s_line.Substring(0, s_line.IndexOf(':'));
                    s_key = (new System.Text.RegularExpressions.Regex("\\s+$")).Replace(s_key, "");

                    /* if line starts with a colon we have a new collection element */
                    if (s_key.StartsWith("- "))
                    {
                        /* cut colon with white space out of line */
                        s_key = s_key.Substring(2);

                        /* increase indentation deep */
                        p_i_whiteSpaceindentation += 2;

                        /* set new collection element flag */
                        b_newElementForCollection = true;
                    }
                    else if ((p_i_whiteSpaceindentation >= 2) && (s_originalLine[p_i_whiteSpaceindentation - 2] == '-') && (s_originalLine[p_i_whiteSpaceindentation - 1] == ' '))
                    {
                        /* if there is a new collection element with '-'-sign as prefix, set new collection element flag */
                        b_newElementForCollection = true;
                    }

                    /* split value after colon */
                    s_value = s_line.Substring(s_line.IndexOf(':') + 1);
                }

                /* if we are not in text block mode, trim key and value */
                if (!p_b_textBlock)
                {
                    s_key = s_key.Trim();
                    s_value = s_value.Trim();
                }

                if (!ForestNET.Lib.Helper.MatchesRegex(s_key, "[a-zA-Z0-9-_]*"))
                {
                    throw new ArgumentException("Error in line (" + (i_min + 1) + "): Invalid key '" + s_key + "', invalid characters. Following characters are allowed: [a-z], [A-Z], [0-9], [-] and [_]");
                }

                /* if value is not 'NULL' and 'length > 0' */
                if ((!s_value.Equals("null")) && (!s_value.Equals(this.s_stringQuote + this.s_stringQuote)) && (s_value.Length > 0))
                {
                    List<char> a_reservedCharacters = [':', '}', ']', ',', '#', '?', '-', '<', '=', '!', '%', '@', '\\', '\'', this.s_stringQuote[0]];

                    /* remove surrounding string quote characters */
                    if ((s_value.StartsWith(this.s_stringQuote)) && (s_value.EndsWith(this.s_stringQuote)))
                    {
                        s_value = s_value.Substring(1, s_value.Length - 1 - 1);
                    }
                    else if (s_value.Equals("[]"))
                    { /* check for empty collection value */
                        s_value = "null";
                    }
                    else if (a_reservedCharacters.Contains(s_value[0]))
                    { /* check for invalid characters on value's first position */
                        if (!((ForestNET.Lib.Helper.IsInteger(s_value)) || (ForestNET.Lib.Helper.IsDouble(s_value))))
                        { /* leading '-' is valid if value is integer or double */
                            throw new ArgumentException("Error in line (" + (i_min + 1) + "): Value starts with invalid character '" + s_value[0] + "'");
                        }
                    }

                    if (s_value[0] == '[')
                    {
                        /* must end with ']' */
                        if (!s_value.EndsWith("]"))
                        {
                            throw new ArgumentException("Error in line (" + (i_min + 1) + "): Inline collection starts with character '" + s_value[0] + "' must end with character ']', but ends with '" + s_value[s_value.Length - 1] + "'");
                        }
                    }
                    else if (s_value[0] == '{')
                    {
                        /* must end with '}' */
                        if (!s_value.EndsWith("}"))
                        {
                            throw new ArgumentException("Error in line (" + (i_min + 1) + "): Inline object starts with character '" + s_value[0] + "' must end with character '}', but ends with '" + s_value[s_value.Length - 1] + "'");
                        }
                    }
                    else if ((s_value[0] == '&') || (s_value.Equals("|")) || (s_value.Equals(">")))
                    {
                        /* next line must be higher level, not same level, not lower level */
                        string s_lineTemp;
                        int i_tempMin = i_min;

                        do
                        {
                            if (++i_tempMin >= p_i_max)
                            {
                                if (s_value[0] == '&')
                                {
                                    throw new ArgumentException("Error in line (" + (i_min + 1) + "): Reached end of block without any subordinate value of reference start");
                                }
                                else if ((s_value.Equals("|")) || (s_value.Equals(">")))
                                {
                                    throw new ArgumentException("Error in line (" + (i_min + 1) + "): Reached end of block without any subordinate value of textblock start");
                                }
                            }

                            /* read next line into string */
                            s_lineTemp = p_a_lines[i_tempMin];

                            /* check if line has any content */
                        } while ((s_lineTemp.Length == 0) || (s_lineTemp.Equals("...")));

                        int i = 0;

                        /* count white spaces */
                        while (s_lineTemp[i] == ' ')
                        {
                            i++;
                        }

                        if (i <= p_i_whiteSpaceindentation)
                        {
                            if (s_value[0] == '&')
                            {
                                throw new ArgumentException("Error in line (" + (i_min + 1) + "): Next line after reference start is not subordinate");
                            }
                            else if ((s_value.Equals("|")) || (s_value.Equals(">")))
                            {
                                throw new ArgumentException("Error in line (" + (i_min + 1) + "): Next line after textblock start is not subordinate");
                            }
                        }
                    }
                }
                else
                {
                    /* set value 'NULL' if 'length = 0' */
                    s_value = "null";
                }

                ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "(" + (i_min + 1) + ")|key=" + s_key + "|value=" + s_value + "|" + b_newElementForCollection + "|" + p_b_textBlock + "|");
            }
        }

        /* parsing YAML schema */

        /// <summary>
        /// Analyze yaml element value and get a unique value type from it
        /// </summary>
        /// <param name="p_s_yamlValue">yaml element value as string</param>
        /// <returns>unique yaml value type</returns>
        /// <exception cref="ArgumentException">invalid value or yaml value type could not be determined</exception>
        public static YAMLValueType GetYAMLValueType(string? p_s_yamlValue)
        {
            YAMLValueType e_yamlValueType = YAMLValueType.String;

            /* get yaml value type */
            if ((p_s_yamlValue == null) || (ForestNET.Lib.Helper.IsStringEmpty(p_s_yamlValue)))
            {
                e_yamlValueType = YAMLValueType.Null;
            }
            else if (ForestNET.Lib.Helper.IsDateTime(p_s_yamlValue))
            {
                /* recognize date format ISO-8601 as string, not as number or integer */
                e_yamlValueType = YAMLValueType.String;
            }
            else if ((p_s_yamlValue[0] == '+') || (p_s_yamlValue[0] == '-') || (char.IsDigit(p_s_yamlValue[0])))
            { /* yaml value starts with digit, '+' or '-' character, so it is of type number or integer */
                bool b_decimalDot = false;
                int i_amountPlusSign = 0;
                int i_amountMinusSign = 0;

                /* iterate value in detail */
                for (int i = 0; i < p_s_yamlValue.Length; i++)
                {
                    /* check if we found a decimal point */
                    if (p_s_yamlValue[i] == '.')
                    {
                        if (b_decimalDot)
                        {
                            /* if we already found a decimal point - number format is invalid */
                            throw new ArgumentException("Invalid number format, found second decimal point in value [" + p_s_yamlValue + "]");
                        }
                        else if (((i == 1) && (!char.IsDigit(p_s_yamlValue[0]))) || (i == p_s_yamlValue.Length - 1))
                        {
                            /* if decimal point has not a single previous digit or is at the end of the value - number format is invalid */
                            throw new ArgumentException("Invalid number format, decimal point at wrong position in value [" + p_s_yamlValue + "]");
                        }
                        else
                        {
                            /* set flag, that decimal point has been found */
                            b_decimalDot = true;
                        }
                    }
                    else if (p_s_yamlValue[i] == '+')
                    {
                        i_amountPlusSign++;

                        if (i_amountPlusSign > 1)
                        { /* exit number check, it seems to be a normal string starting with '++...' */
                            break;
                        }
                    }
                    else if (p_s_yamlValue[i] == '-')
                    {
                        i_amountMinusSign++;

                        if (i_amountMinusSign > 1)
                        { /* exit number check, it seems to be a normal string starting with '--...' */
                            break;
                        }
                    }
                    else if ((i != 0) && (!char.IsDigit(p_s_yamlValue[i])))
                    { /* check if we found a character which is not a digit */
                        /* we do not need an exception, just setting both sign counters to 1 and exit number check */
                        i_amountPlusSign = 1;
                        i_amountMinusSign = 1;
                        break;
                    }
                }

                /* only accept one plus-sign or one minus-sign, otherwise it is a normal string */
                if (((i_amountPlusSign == 0) && (i_amountMinusSign == 0)) || (i_amountPlusSign == 1) ^ (i_amountMinusSign == 1))
                {
                    if (b_decimalDot)
                    {
                        /* decimal point found, value is of type number */
                        e_yamlValueType = YAMLValueType.Number;
                    }
                    else
                    {
                        /* value is of type integer */
                        e_yamlValueType = YAMLValueType.Integer;
                    }
                }
            }
            else if (p_s_yamlValue.ToLower().Equals("true") || p_s_yamlValue.ToLower().Equals("false"))
            { /* value equals 'true' or 'false', so it is of type bool */
                e_yamlValueType = YAMLValueType.Boolean;
            }
            else if (p_s_yamlValue.ToLower().Equals("null"))
            { /* value equals 'null', so it is of type object */
                e_yamlValueType = YAMLValueType.Object;
            }
            else if ((p_s_yamlValue.StartsWith("[")) && (p_s_yamlValue.EndsWith("]")))
            { /* yaml value starts with '[' character and ends with ']' character, so it is of type array */
                e_yamlValueType = YAMLValueType.Array;
            }

            return e_yamlValueType;
        }

        /// <summary>
        /// Parse yaml file lines to yaml object structure, based on YAMLElement and YAMLRestriction
        /// </summary>
        /// <param name="p_i_min">pointer where to start line iteration</param>
        /// <param name="p_i_max">pointer where to end line iteration</param>
        /// <param name="p_a_lines">yaml file lines</param>
        /// <param name="p_i_whiteSpaceindentation">amount of white space indentation in yaml file</param>
        /// <param name="p_o_yamlElement">yaml element object where yaml information will be parsed</param>
        /// <returns>parsed yaml element object</returns>
        /// <exception cref="ArgumentException">value or structure within yaml file lines invalid</exception>
        /// <exception cref="ArgumentNullException">value within yaml file lines missing or min. amount not available</exception>
        public YAMLElement? ParseYAML(int p_i_min, int p_i_max, List<string> p_a_lines, int p_i_whiteSpaceindentation, YAMLElement p_o_yamlElement)
        {
            return this.ParseYAML(p_i_min, p_i_max, p_a_lines, p_i_whiteSpaceindentation, p_o_yamlElement, false);
        }

        /// <summary>
        /// Parse yaml file lines to yaml object structure, based on YAMLElement and YAMLRestriction
        /// </summary>
        /// <param name="p_i_min">pointer where to start line iteration</param>
        /// <param name="p_i_max">pointer where to end line iteration</param>
        /// <param name="p_a_lines">yaml file lines</param>
        /// <param name="p_i_whiteSpaceindentation">amount of white space indentation in yaml file</param>
        /// <param name="p_o_yamlElement">yaml element object where yaml information will be parsed</param>
        /// <param name="p_b_parseSchema">true - parsing yaml file lines from a yaml schema, false - parsing yaml file lines from a data file</param>
        /// <returns>parsed yaml element object</returns>
        /// <exception cref="ArgumentException">value or structure within yaml file lines invalid</exception>
        /// <exception cref="ArgumentNullException">value within yaml file lines missing or min. amount not available</exception>
        public YAMLElement? ParseYAML(int p_i_min, int p_i_max, List<string> p_a_lines, int p_i_whiteSpaceindentation, YAMLElement p_o_yamlElement, bool p_b_parseSchema)
        {
            /* temp variables for key and value */
            string s_key;
            string s_value = "null";
            string? s_reference = null;
            bool b_collectionLevelFlag = false;
            YAMLElement? o_yamlOldCurrentLevelElement = null;

            /* store current level yaml element in variable */
            YAMLElement? o_yamlCurrentLevelElement = p_o_yamlElement;

            /* iterate each line */
            for (int i_min = p_i_min; i_min < p_i_max; i_min++)
            {
                /* read line into string */
                string s_line = p_a_lines[i_min];

                ForestNET.Lib.Global.ILogFiner(s_line);

                /* check if line has any content */
                if ((s_line.Length == 0) || (s_line.Equals("...")))
                {
                    ForestNET.Lib.Global.ILogFiner("(" + (i_min + 1) + ")|NULL|");

                    /* skip lines with no content */
                    continue;
                }

                if (p_i_whiteSpaceindentation >= s_line.Length)
                {
                    throw new ArgumentException("Error in line (" + (i_min + 1) + "): Invalid indentation, yaml syntax error");
                }

                /* check if line starts with white space on expected position */
                if (s_line[p_i_whiteSpaceindentation] == ' ')
                {
                    ForestNET.Lib.Global.ILogFiner(s_line);

                    /* check if we already have a value from previous line */
                    if (!s_value.Equals("null"))
                    {
                        /* new indentation with previous value only allowed if value was reference or text block */
                        if ((s_value[0] != '&') && (!s_value.Equals("|")) && (!s_value.Equals(">")))
                        {
                            throw new ArgumentException("Error in line (" + (i_min + 1) + "): New indentation found, but parent element has already a value");
                        }
                    }

                    int i = 0;

                    /* count white spaces */
                    while (s_line[i] == ' ')
                    {
                        i++;
                    }

                    int i_max;

                    /* search for end of indentation block */
                    for (i_max = i_min + 1; i_max < p_i_max; i_max++)
                    {
                        /* read line */
                        string s_lineTemp = p_a_lines[i_max];

                        /* check if line has any content */
                        if ((s_lineTemp.Length == 0) || (s_lineTemp.Equals("...")))
                        {
                            /* skip lines with no content */
                            continue;
                        }

                        int j = 0;

                        /* count white spaces */
                        while (s_lineTemp[j] == ' ')
                        {
                            j++;
                        }

                        /* if counted white spaces matches expected position of indentation or there is a new collection element with '-'-sign as prefix */
                        if ((j == p_i_whiteSpaceindentation) || ((p_i_whiteSpaceindentation >= 2) && (s_lineTemp[p_i_whiteSpaceindentation - 2] == '-') && (s_lineTemp[p_i_whiteSpaceindentation - 1] == ' ')))
                        {
                            /* break - end of indentation block found */
                            break;
                        }
                    }

                    /* increase level for PrintIndentation */
                    this.i_level++;

                    ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "block goes from (" + (i_min + 1) + ") to (" + (i_max) + ") with indentation '" + i + "'");

                    /* start new recursion with new indentation block */
                    YAMLElement? o_returnRecursion = this.ParseYAML(i_min, i_max, p_a_lines, i, p_o_yamlElement, p_b_parseSchema);

                    if ((s_reference != null) && (o_returnRecursion != null))
                    {
                        ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "store reference: " + s_reference);

                        o_returnRecursion.Name = s_reference;
                        this.a_references?.Add(o_returnRecursion.Name);
                        s_reference = null;
                    }

                    /* decrease level for PrintIndentation */
                    this.i_level--;

                    /* overwrite line pointer and continue for loop after indentation block */
                    i_min = i_max - 1;
                    continue;
                }

                string s_originalLine = s_line;

                /* cut white space indentation of line */
                if (p_i_whiteSpaceindentation > 0)
                {
                    s_line = s_line.Substring(p_i_whiteSpaceindentation);
                }

                int i_amountColon = 0;
                int i_lineLevel = 0;
                int i_lineLevelSequences = 0;
                bool b_inQuotation = false;
                bool b_escapeCharacterBefore = false;

                /* check for multiple colon characters in line */
                for (int i = 0; i < s_line.Length; i++)
                {
                    if (s_line[i] == '{')
                    {
                        i_lineLevel++;
                    }
                    else if (s_line[i] == '}')
                    {
                        i_lineLevel--;
                    }
                    else if (s_line[i] == '[')
                    {
                        i_lineLevelSequences++;
                    }
                    else if (s_line[i] == ']')
                    {
                        i_lineLevelSequences--;
                    }
                    else if ((s_line[i] == '"') && (!b_escapeCharacterBefore))
                    {
                        if (b_inQuotation)
                        {
                            b_inQuotation = false;
                        }
                        else
                        {
                            b_inQuotation = true;
                        }
                    }
                    else if ((b_inQuotation) && (s_line[i] == '\\'))
                    {
                        b_escapeCharacterBefore = true;
                    }
                    else if ((s_line[i] == ':') && (i_lineLevel == 0) && (!b_inQuotation))
                    {
                        i_amountColon++;
                    }

                    if ((b_inQuotation) && (s_line[i] != '\\'))
                    {
                        b_escapeCharacterBefore = false;
                    }
                }

                if (i_lineLevel != 0)
                {
                    throw new ArgumentException("Error in line (" + (i_min + 1) + "): Invalid nesting with curly brackets '{', '}'");
                }

                if (i_lineLevelSequences != 0)
                {
                    throw new ArgumentException("Error in line (" + (i_min + 1) + "): Invalid collection array with squared brackets '[', ']'");
                }

                if (i_amountColon > 1)
                {
                    throw new ArgumentException("Error in line (" + (i_min + 1) + "): A line can only contain one colon ':' character");
                }

                s_key = "null";
                s_value = "null";
                bool b_newElementForCollection = false;

                /* if there is no colon we only have a value in line */
                if ((i_amountColon == 0))
                {
                    /* assume whole line as value */
                    s_value = s_line;

                    /* if line starts with a colon we have a new collection element */
                    if (s_line.StartsWith("- "))
                    {
                        /* cut colon with white space out of line */
                        s_value = s_value.Substring(2);

                        /* set new collection element flag */
                        b_newElementForCollection = true;
                    }
                    else
                    {
                        if (!(((s_value.StartsWith("{")) && (s_value.EndsWith("}")))))
                        {
                            throw new ArgumentException("Error in line (" + (i_min + 1) + "): A line which is not a key value pair, not a collection element, no inline-object and not part of a text block is not allowed");
                        }
                    }
                }
                else if (i_amountColon == 1)
                {
                    /* split key value before colon and remove all white spaces between key value and colon */
                    s_key = s_line.Substring(0, s_line.IndexOf(':'));
                    s_key = (new System.Text.RegularExpressions.Regex("\\s+$")).Replace(s_key, "");

                    /* if line starts with a colon we have a new collection element */
                    if (s_key.StartsWith("- "))
                    {
                        /* cut colon with white space out of line */
                        s_key = s_key.Substring(2);

                        /* increase indentation deep */
                        p_i_whiteSpaceindentation += 2;

                        /* set new collection element flag */
                        b_newElementForCollection = true;
                    }
                    else if ((p_i_whiteSpaceindentation >= 2) && (s_originalLine[p_i_whiteSpaceindentation - 2] == '-') && (s_originalLine[p_i_whiteSpaceindentation - 1] == ' '))
                    {
                        /* if there is a new collection element with '-'-sign as prefix, set new collection element flag */
                        b_newElementForCollection = true;
                    }

                    /* split value after colon */
                    s_value = s_line.Substring(s_line.IndexOf(':') + 1);
                }

                /* trim key and value */
                s_key = s_key.Trim();
                s_value = s_value.Trim();

                if (!ForestNET.Lib.Helper.MatchesRegex(s_key, "[a-zA-Z0-9-_]*"))
                {
                    throw new ArgumentException("Error in line (" + (i_min + 1) + "): Invalid key '" + s_key + "', invalid characters. Following characters are allowed: [a-z], [A-Z], [0-9], [-] and [_]");
                }

                /* if value is not 'NULL' and 'length > 0' */
                if ((!s_value.Equals("null")) && (!s_value.Equals(this.s_stringQuote + this.s_stringQuote)) && (s_value.Length > 0))
                {
                    List<char> a_reservedCharacters = [':', '}', ']', ',', '#', '?', '-', '<', '>', '|', '=', '!', '%', '@', '\\', '\'', this.s_stringQuote[0]];

                    /* remove surrounding string quote characters */
                    if ((s_value.StartsWith(this.s_stringQuote)) && (s_value.EndsWith(this.s_stringQuote)))
                    {
                        if ((!p_b_parseSchema) && (!s_value.Equals("\"null\"")))
                        {
                            s_value = s_value.Substring(1, s_value.Length - 1 - 1);
                        }
                    }
                    else if (s_value.Equals("[]"))
                    { /* check for empty collection value */
                        s_value = "null";
                    }
                    else if (a_reservedCharacters.Contains(s_value[0]))
                    { /* check for invalid characters on value's first position */
                        if (!((ForestNET.Lib.Helper.IsInteger(s_value)) || (ForestNET.Lib.Helper.IsDouble(s_value))))
                        { /* leading '-' is valid if value is integer or double */
                            throw new ArgumentException("Error in line (" + (i_min + 1) + "): Value starts with invalid character '" + s_value[0] + "'");
                        }
                    }

                    if (s_value[0] == '[')
                    {
                        /* must end with ']' */
                        if (!s_value.EndsWith("]"))
                        {
                            throw new ArgumentException("Error in line (" + (i_min + 1) + "): Inline collection starts with character '" + s_value[0] + "' must end with character ']', but ends with '" + s_value[s_value.Length - 1] + "'");
                        }
                    }
                    else if (s_value[0] == '{')
                    {
                        /* must end with '}' */
                        if (!s_value.EndsWith("}"))
                        {
                            throw new ArgumentException("Error in line (" + (i_min + 1) + "): Inline object starts with character '" + s_value[0] + "' must end with character '}', but ends with '" + s_value[s_value.Length - 1] + "'");
                        }
                    }
                    else if ((s_value[0] == '&') || (s_value.Equals("|")) || (s_value.Equals(">")))
                    {
                        /* next line must be higher level, not same level, not lower level */
                        string s_lineTemp;
                        int i_tempMin = i_min;

                        do
                        {
                            if (++i_tempMin >= p_i_max)
                            {
                                if (s_value[0] == '&')
                                {
                                    throw new ArgumentException("Error in line (" + (i_min + 1) + "): Reached end of block without any subordinate value of reference start");
                                }
                                else if ((s_value.Equals("|")) || (s_value.Equals(">")))
                                {
                                    throw new ArgumentException("Error in line (" + (i_min + 1) + "): Reached end of block without any subordinate value of textblock start");
                                }
                            }

                            /* read next line into string */
                            s_lineTemp = p_a_lines[i_tempMin];

                            /* check if line has any content */
                        } while ((s_lineTemp.Length == 0) || (s_lineTemp.Equals("...")));

                        int i = 0;

                        /* count white spaces */
                        while (s_lineTemp[i] == ' ')
                        {
                            i++;
                        }

                        if (i <= p_i_whiteSpaceindentation)
                        {
                            if (s_value[0] == '&')
                            {
                                throw new ArgumentException("Error in line (" + (i_min + 1) + "): Next line after reference start is not subordinate");
                            }
                            else if ((s_value.Equals("|")) || (s_value.Equals(">")))
                            {
                                throw new ArgumentException("Error in line (" + (i_min + 1) + "): Next line after textblock start is not subordinate");
                            }
                        }

                        if (s_value[0] == '&')
                        {
                            s_reference = s_value.Substring(1);
                        }
                    }
                    else if (s_value[0] == '*')
                    {
                        /* check if reference is stored */
                        string s_referenceValue = s_value.Substring(1);
                        string? s_referenceName = null;

                        foreach (string s_yamlReference in this.a_references ?? [])
                        {
                            if (s_yamlReference.Equals(s_referenceValue))
                            {
                                s_referenceName = s_yamlReference;
                            }
                        }

                        if (s_referenceName == null)
                        {
                            throw new ArgumentNullException("Error in line (" + (i_min + 1) + "): Reference '" + s_value + "' not found in yaml-schema-document");
                        }
                    }
                }
                else
                {
                    if (s_value.Equals(this.s_stringQuote + this.s_stringQuote))
                    {
                        /* set value empty string if 'value = ""' */
                        s_value = "";
                    }
                    else
                    {
                        /* set value 'NULL' if 'length = 0' */
                        s_value = "null";
                    }
                }

                if ((b_newElementForCollection) && (!p_b_parseSchema))
                {
                    if ((!b_collectionLevelFlag) || (!s_key.Equals("null")))
                    {
                        if (!s_key.Equals("null"))
                        {
                            if (o_yamlOldCurrentLevelElement != null)
                            {
                                o_yamlCurrentLevelElement = o_yamlOldCurrentLevelElement;
                                this.i_level--;

                                ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "array object before is finished after new element for collection appeared and array level flag is still 'true'");
                            }
                        }

                        b_collectionLevelFlag = true;

                        ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "new array object");

                        o_yamlOldCurrentLevelElement = o_yamlCurrentLevelElement;

                        YAMLElement o_yamlArrayElement = new("__ArrayObject__", this.i_level);

                        /* add new element to current element children and set as new current element */
                        o_yamlCurrentLevelElement.Children.Add(o_yamlArrayElement);
                        o_yamlCurrentLevelElement = o_yamlArrayElement;

                        this.i_level++;
                    }
                }

                ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "(" + (i_min + 1) + ")|key=" + s_key + "|value=" + s_value + "|" + b_newElementForCollection + "|");

                YAMLElement o_yamlElement = new(s_key, this.i_level);

                if (!s_value.Equals("null"))
                {
                    o_yamlElement.Value = s_value;
                }

                ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + o_yamlCurrentLevelElement.Name + ".Children.add(" + o_yamlElement.Name + ");");

                o_yamlCurrentLevelElement.Children.Add(o_yamlElement);

                ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "p_o_yamlElement = " + o_yamlElement.Name + ";");

                p_o_yamlElement = o_yamlElement;
            }

            if ((b_collectionLevelFlag) && (!p_b_parseSchema))
            {
                o_yamlCurrentLevelElement = o_yamlOldCurrentLevelElement;
                this.i_level--;

                ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "array object before is finished after iteration");
            }

            if (p_o_yamlElement.Name.Equals(o_yamlCurrentLevelElement?.Name))
            {
                ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "return " + p_o_yamlElement.Name);
                return p_o_yamlElement;
            }
            else
            {
                ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "return " + o_yamlCurrentLevelElement?.Name);
                return o_yamlCurrentLevelElement;
            }
        }

        /// <summary>
        /// Parse yaml schema elements with all children elements
        /// </summary>
        /// <param name="p_o_yamlSchemaElement">yaml schema element object which will be parsed</param>
        /// <exception cref="ArgumentException">value or type within yaml schema invalid</exception>
        /// <exception cref="ArgumentNullException">value within yaml schema missing or min. amount not available</exception>
        private void ParseYAMLSchemaElements(YAMLElement p_o_yamlSchemaElement)
        {
            if (p_o_yamlSchemaElement.Children.Count > 0)
            {
                bool b_array = false;
                bool b_object = false;
                bool b_properties = false;
                bool b_items = false;

                /* 
				 * check if we have "type": "array" and "items" and no "properties"
				 * or if we have "type": "object" and "properties" and no "items"
				 */
                foreach (YAMLElement o_yamlChild in p_o_yamlSchemaElement.Children)
                {
                    if (o_yamlChild.Name.ToLower().Equals("type"))
                    {
                        string s_type = o_yamlChild.Value ?? string.Empty;

                        /* remove surrounded double quotes from value */
                        if ((s_type.StartsWith(this.s_stringQuote)) && (s_type.EndsWith(this.s_stringQuote)))
                        {
                            s_type = s_type.Substring(1, s_type.Length - 1 - 1);
                        }

                        if (s_type.Equals("array"))
                        {
                            b_array = true;
                        }
                        else if (s_type.Equals("object"))
                        {
                            b_object = true;
                        }
                    }
                    else if (o_yamlChild.Name.ToLower().Equals("properties"))
                    {
                        b_properties = true;
                    }
                    else if (o_yamlChild.Name.ToLower().Equals("items"))
                    {
                        b_items = true;
                    }
                }

                ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "element: '" + p_o_yamlSchemaElement.Name + "' - array=" + b_array + "|object=" + b_object + "|properties=" + b_properties + "|items=" + b_items + "|");

                /* control result of check */
                if ((!b_array) && (!b_object))
                {
                    if ((this.i_level == 0) && (!p_o_yamlSchemaElement.Name.ToLower().Equals("definitions")) && (!p_o_yamlSchemaElement.Name.ToLower().Equals("properties")))
                    {
                        throw new ArgumentException("YAML definition of element[definitions] or [properties] necessary on first level for [" + p_o_yamlSchemaElement.Name + "]");
                    }
                }
                else if ((b_array) && (b_properties))
                {
                    throw new ArgumentException("YAML definition with type[array] cannot have [properties] at the same time");
                }
                else if ((b_array) && (!b_items))
                {
                    throw new ArgumentException("YAML definition with type[array] must have [items] definition as well");
                }
                else if ((b_object) && (b_items))
                {
                    throw new ArgumentException("YAML definition with type[object] cannot have [items] at the same time");
                }
                else if ((b_object) && (!b_properties))
                {
                    throw new ArgumentException("YAML definition with type[object] must have [properties] definition as well");
                }

                foreach (YAMLElement o_yamlChild in p_o_yamlSchemaElement.Children)
                {
                    YAMLValueType e_yamlValueType;

                    /* determine yaml value type */
                    if (ForestNET.Lib.Helper.IsStringEmpty(o_yamlChild.Value))
                    {
                        e_yamlValueType = YAMLValueType.Object;
                    }
                    else
                    {
                        e_yamlValueType = YAML.GetYAMLValueType(o_yamlChild.Value);

                        /* remove surrounded quote characters from value */
                        if ((o_yamlChild.Value != null) && (o_yamlChild.Value.StartsWith(this.s_stringQuote)) && (o_yamlChild.Value.EndsWith(this.s_stringQuote)))
                        {
                            o_yamlChild.Value = o_yamlChild.Value.Substring(1, o_yamlChild.Value.Length - 1 - 1);
                        }

                        /* if value type is string and value starts with '&' character */
                        if ((e_yamlValueType == YAMLValueType.String) && (o_yamlChild.Value != null) && (o_yamlChild.Value[0] == '&'))
                        {
                            /* look for stored reference */
                            string s_referenceValue = o_yamlChild.Value.Substring(1);
                            string? s_referenceName = null;

                            /* iterate reference store */
                            foreach (string s_yamlReference in this.a_references ?? [])
                            {
                                if (s_yamlReference.Equals(s_referenceValue))
                                {
                                    s_referenceName = s_yamlReference;
                                    break;
                                }
                            }

                            /* if we do not find reference, throw Exception */
                            if (s_referenceName == null)
                            {
                                throw new ArgumentNullException("Invalid YAML reference[" + o_yamlChild.Value + "] for property[" + o_yamlChild.Name + "], reference not found");
                            }
                            else
                            { /* reference found */
                                /* current yaml child is object */
                                e_yamlValueType = YAMLValueType.Object;
                                /* truncate value */
                                o_yamlChild.Value = null;
                            }
                        }
                    }

                    ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + o_yamlChild.Name + "(" + e_yamlValueType + ") of " + p_o_yamlSchemaElement.Name);

                    /* special properties at level 0 of yaml schema only */
                    if (o_yamlChild.Level == 0)
                    {
                        if ((o_yamlChild.Name.ToLower().Equals("definitions")) || (o_yamlChild.Name.ToLower().Equals("properties")))
                        {
                            /* if yaml value type is of type object */
                            if (e_yamlValueType == YAMLValueType.Object)
                            {
                                ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "save current element(" + this.o_currentElement?.Name + ") in temporary variable");

                                /* save current element in temporary variable */
                                YAMLElement? o_oldCurrentElement = this.o_currentElement;

                                ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "start recursion for: " + o_yamlChild.Name + " = (" + e_yamlValueType + ") for " + this.o_currentElement?.Name);

                                if (o_yamlChild.Name.ToLower().Equals("definitions"))
                                {
                                    /* create new yaml element for 'definitions' */
                                    this.o_currentElement = new YAMLElement(o_yamlChild.Name, 0);

                                    /* parse yaml value recursively */
                                    this.ParseYAMLSchemaElements(o_yamlChild);

                                    /* process current element as return value */
                                    this.o_definitions = this.o_currentElement;
                                }
                                else if (o_yamlChild.Name.ToLower().Equals("properties"))
                                {
                                    /* create new yaml element for 'properties' */
                                    this.o_currentElement = new YAMLElement(o_yamlChild.Name, 0);

                                    /* parse yaml child recursively */
                                    this.ParseYAMLSchemaElements(o_yamlChild);

                                    /* process current element as return value */
                                    this.o_properties = this.o_currentElement;
                                }

                                ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "end recursion for " + this.o_currentElement?.Name + ": " + o_yamlChild.Name + " = (" + e_yamlValueType + ")");

                                ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "reset current element(" + this.o_currentElement?.Name + ") from temporary variable(" + o_oldCurrentElement?.Name + ")");

                                /* reset current element from temporary variable */
                                this.o_currentElement = o_oldCurrentElement;

                                /* we can skip the rest of loop processing */
                                continue;
                            }
                            else
                            {
                                throw new ArgumentException("Invalid YAML type[" + e_yamlValueType + "] for property[" + o_yamlChild.Name + "] of type object");
                            }
                        }
                    }

                    /* default parsing of properties for yaml schema */

                    /* if current element is another object we need another recursion */
                    if (e_yamlValueType == YAMLValueType.Object)
                    {
                        bool b_skip = false;

                        /* check if we have a required colletion which is not stored in one line */
                        if (o_yamlChild.Name.ToLower().Equals("required"))
                        {
                            if (o_yamlChild.Type == null)
                            {
                                if (o_yamlChild.Children.Count > 0)
                                {
                                    foreach (YAMLElement o_yamlRequiredValue in o_yamlChild.Children)
                                    {
                                        bool b_nameIsNull = ((o_yamlRequiredValue.Name == null) || (o_yamlRequiredValue.Name == "null"));
                                        bool b_typeIsNull = ((o_yamlRequiredValue.Type == null) || (o_yamlRequiredValue.Type == "null"));

                                        if ((!b_nameIsNull) || (!b_typeIsNull))
                                        {
                                            throw new ArgumentNullException("Invalid YAML schema-element[" + o_yamlChild.Name + "], child required value must not have name[" + o_yamlRequiredValue.Name + "] and type[" + o_yamlRequiredValue.Type + "] definition");
                                        }
                                        else if (o_yamlRequiredValue.Value == null)
                                        {
                                            throw new ArgumentException("Invalid YAML schema-element[" + o_yamlChild.Name + "], child required value has no value");
                                        }
                                        else
                                        {
                                            string s_requiredValue = o_yamlRequiredValue.Value;

                                            /* trim required value */
                                            s_requiredValue = s_requiredValue.Trim();

                                            bool b_requiredFound = false;
                                            List<YAMLElement>? a_children = null;

                                            /* check if we are at 'root' level */
                                            if ((this.o_currentElement != null) && (this.o_currentElement.Name.Equals("Root")) && (this.o_currentElement.Level == 0))
                                            {
                                                /* look for 'properties' child */
                                                foreach (YAMLElement o_yamlCurrentElementChild in this.o_currentElement.Children)
                                                {
                                                    if (o_yamlCurrentElementChild.Name.ToLower().Equals("properties"))
                                                    {
                                                        /* set 'properties' children as array to search for 'required' element */
                                                        a_children = o_yamlCurrentElementChild.Children;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                /* set children of current element as array to search for 'required' element */
                                                a_children = this.o_currentElement?.Children;
                                            }

                                            if (a_children == null)
                                            {
                                                throw new ArgumentNullException("Cannot handle required property[" + s_requiredValue + "] in array[" + o_yamlChild.Name + "] because current element[" + this.o_currentElement?.Name + "] has no children or no properties");
                                            }

                                            /* iterate all children of current element to find required 'property' */
                                            foreach (YAMLElement o_yamlCurrentElementChild in a_children)
                                            {
                                                /* compare by property name */
                                                if (o_yamlCurrentElementChild.Name.ToLower().Equals(s_requiredValue.ToLower()))
                                                {
                                                    b_requiredFound = true;
                                                    o_yamlCurrentElementChild.Required = true;
                                                    break;
                                                }
                                            }

                                            if (!b_requiredFound)
                                            {
                                                throw new ArgumentException("Required property[" + s_requiredValue + "] in array[" + o_yamlChild.Name + "] does not exist within 'properties'");
                                            }
                                        }
                                    }

                                    b_skip = true;
                                }
                                else
                                {
                                    throw new ArgumentException("Invalid YAML schema-element[" + o_yamlChild.Name + "] has no children");
                                }
                            }
                            else
                            {
                                throw new ArgumentException("Invalid YAML schema-element[" + o_yamlChild.Name + "]. Must have type null, but type is '" + o_yamlChild.Type + "'");
                            }
                        }

                        /* skip because of 'items' object with one reference */
                        if (!b_skip)
                        { /* new object */
                            ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + o_yamlChild.Name + " = (" + e_yamlValueType + ")");

                            if (o_yamlChild.Name.ToLower().Equals("properties"))
                            {
                                ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "start recursion for: " + o_yamlChild.Name + " = (" + e_yamlValueType + ") for " + this.o_currentElement?.Name);

                                /* parse yaml child recursively */
                                this.ParseYAMLSchemaElements(o_yamlChild);

                                ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "end recursion for: " + o_yamlChild.Name + " = (" + e_yamlValueType + ")");
                            }
                            else
                            {
                                ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "save current element(" + this.o_currentElement?.Name + ") in temporary variable");

                                /* save current element in temporary variable */
                                YAMLElement? o_oldCurrentElement = this.o_currentElement;

                                ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "start recursion for: " + o_yamlChild.Name + " = (" + e_yamlValueType + ") for " + this.o_currentElement?.Name);

                                /* create new yaml element */
                                YAMLElement o_newYAMLElement = new(o_yamlChild.Name, this.i_level);

                                /* if we have Root node as current element on level 0 with type 'array' and new child 'items', we must not add a new child, because of concurrent modificication of the for loop */
                                bool b_handleRootItems = false;

                                if ((this.o_currentElement != null) && (this.o_currentElement.Name.Equals("Root")) && (this.o_currentElement.Level == 0) && (this.o_currentElement.Type != null) && (this.o_currentElement.Type.ToLower().Equals("array")) && (o_newYAMLElement.Name.ToLower().Equals("items")))
                                {
                                    b_handleRootItems = true;
                                }
                                else
                                {
                                    /* add new yaml element to current elements children */
                                    this.o_currentElement?.Children.Add(o_newYAMLElement);
                                }

                                /* set new yaml element as current element for recursive processing */
                                this.o_currentElement = o_newYAMLElement;

                                /* increase level for PrintIndentation */
                                this.i_level++;

                                /* parse yaml child recursively */
                                this.ParseYAMLSchemaElements(o_yamlChild);

                                ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "reset current element(" + this.o_currentElement?.Name + ") from temporary variable(" + o_oldCurrentElement?.Name + ")");

                                /* reset current element from temporary variable */
                                this.o_currentElement = o_oldCurrentElement;

                                /* check if we must handle root items */
                                if (b_handleRootItems)
                                {
                                    /* new element 'items', we set the child as reference of Root */
                                    if (o_newYAMLElement.Children.Count == 1)
                                    {
                                        ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "setReference for (" + this.o_currentElement?.Name + ") with object(" + o_newYAMLElement.Children[0].Name + ")");

                                        if (this.o_currentElement == null)
                                        {
                                            throw new NullReferenceException("Cannot set reference '" + o_newYAMLElement.Children[0] + "' to current element which is 'null'");
                                        }

                                        this.o_currentElement.Reference = o_newYAMLElement.Children[0];
                                    }
                                    else
                                    {
                                        throw new ArgumentException("Root items must have only one child, but there are (" + o_newYAMLElement.Children.Count + ") children");
                                    }
                                }

                                /* decrease level for PrintIndentation */
                                this.i_level--;

                                /* between update of schema definitions, for the case that a definition is depending on another definition before */
                                if ((this.o_currentElement != null) && (this.o_currentElement.Name.ToLower().Equals("definitions")))
                                {
                                    this.o_definitions = this.o_currentElement;
                                }

                                ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "end recursion for: " + o_yamlChild.Name + " = (" + e_yamlValueType + ")");
                            }
                        }
                    }
                    else
                    {
                        ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + o_yamlChild.Name + " = " + o_yamlChild.Value + " (" + e_yamlValueType + ") for " + this.o_currentElement?.Name);

                        if (this.o_currentElement == null)
                        {
                            throw new NullReferenceException("Cannot continue with current element as 'null'");
                        }

                        if ((o_yamlChild.Value ?? string.Empty)[0] == '*')
                        {
                            if (e_yamlValueType == YAMLValueType.String)
                            {
                                string s_reference = (o_yamlChild.Value ?? "  ").Substring(1);
                                bool b_found = false;

                                foreach (YAMLElement o_yamlDefinition in this.o_definitions?.Children ?? [])
                                {
                                    if (o_yamlDefinition.Name.Equals(s_reference))
                                    {
                                        if (o_yamlChild.Name.ToLower().Equals("items"))
                                        {
                                            /* add reference to current element */
                                            this.o_currentElement.Reference = o_yamlDefinition;
                                        }
                                        else
                                        {
                                            /* save current element in temporary variable */
                                            YAMLElement o_oldCurrentElement = this.o_currentElement;

                                            /* create new yaml element */
                                            YAMLElement o_newYAMLElement = new(o_yamlChild.Name, this.i_level);

                                            /* add new yaml element to current elements children */
                                            this.o_currentElement.Children.Add(o_newYAMLElement);

                                            /* set new yaml element as current element for recursive processing */
                                            this.o_currentElement = o_newYAMLElement;

                                            /* add reference to current element */
                                            this.o_currentElement.Reference = o_yamlDefinition;

                                            /* reset current element from temporary variable */
                                            this.o_currentElement = o_oldCurrentElement;
                                        }

                                        b_found = true;
                                        break;
                                    }
                                }

                                if (!b_found)
                                {
                                    throw new ArgumentException("YAML definition[" + s_reference + "] not found under /definitions in yaml schema");
                                }
                            }
                            else
                            {
                                throw new ArgumentException("Invalid YAML type[" + e_yamlValueType + "] for property[" + o_yamlChild.Name + "] with value[" + o_yamlChild.Value + "]");
                            }
                        }
                        else if (o_yamlChild.Name.ToLower().Equals("type"))
                        {
                            if (e_yamlValueType == YAMLValueType.String)
                            {
                                List<string> a_validTypes = ["string", "number", "integer", "boolean", "array", "object", "null"];

                                /* store type value in temp. variable */
                                string s_foo = o_yamlChild.Value ?? string.Empty;

                                /* check if type value ends with '[]' */
                                if (s_foo.EndsWith("[]"))
                                {
                                    /* delete '[]' from type value */
                                    s_foo = s_foo.Substring(0, s_foo.Length - 2);

                                    /* set primitive array flag */
                                    this.o_currentElement.PrimitiveArray = true;
                                }

                                /* check if we have a valid type value */
                                if (!a_validTypes.Contains(s_foo))
                                {
                                    throw new ArgumentException("Invalid value[" + o_yamlChild.Value + "] for property[type], allowed values are " + a_validTypes);
                                }

                                /* store type value */
                                this.o_currentElement.Type = s_foo;
                            }
                            else
                            {
                                throw new ArgumentException("Invalid YAML type[" + e_yamlValueType + "] for property[" + o_yamlChild.Name + "] with value[" + o_yamlChild.Value + "]");
                            }
                        }
                        else if (o_yamlChild.Name.ToLower().Equals("description"))
                        {
                            if (e_yamlValueType == YAMLValueType.String)
                            {
                                this.o_currentElement.Description = o_yamlChild.Value;
                            }
                            else
                            {
                                throw new ArgumentException("Invalid YAML type[" + e_yamlValueType + "] for property[" + o_yamlChild.Name + "] with value[" + o_yamlChild.Value + "]");
                            }
                        }
                        else if (o_yamlChild.Name.ToLower().Equals("default"))
                        {
                            if (e_yamlValueType == YAMLValueType.String)
                            {
                                this.o_currentElement.Default = o_yamlChild.Value;
                            }
                            else
                            {
                                throw new ArgumentException("Invalid YAML type[" + e_yamlValueType + "] for property[" + o_yamlChild.Name + "] with value[" + o_yamlChild.Value + "]");
                            }
                        }
                        else if (o_yamlChild.Name.ToLower().Equals("mapping"))
                        {
                            if (e_yamlValueType == YAMLValueType.String)
                            {
                                if ((o_yamlChild.Value != null) && (o_yamlChild.Value.Contains(':')))
                                { /* set mapping and mappingClass */
                                    this.o_currentElement.Mapping = o_yamlChild.Value.Substring(0, o_yamlChild.Value.IndexOf(":"));
                                    this.o_currentElement.MappingClass = o_yamlChild.Value.Substring(o_yamlChild.Value.IndexOf(":") + 1, o_yamlChild.Value.Length - (o_yamlChild.Value.IndexOf(":") + 1));
                                }
                                else
                                { /* set only mappingClass */
                                    this.o_currentElement.MappingClass = o_yamlChild.Value ?? string.Empty;
                                }
                            }
                            else
                            {
                                throw new ArgumentException("Invalid YAML type[" + e_yamlValueType + "] for property[" + o_yamlChild.Name + "] with value[" + o_yamlChild.Value + "]");
                            }
                        }
                        else if (o_yamlChild.Name.ToLower().Equals("maxitems"))
                        {
                            if (e_yamlValueType == YAMLValueType.Integer)
                            {
                                if ((this.o_currentElement.Type != null) && (!this.o_currentElement.Type.ToLower().Equals("array")))
                                {
                                    throw new ArgumentException("Invalid YAML restriction[" + o_yamlChild.Name + "] for [" + this.o_currentElement.Name + "] with type[" + o_yamlChild.Type + "], type must be array");
                                }

                                this.o_currentElement.Restrictions.Add(new YAMLRestriction(o_yamlChild.Name, this.i_level, "", int.Parse(o_yamlChild.Value ?? string.Empty)));
                            }
                            else
                            {
                                throw new ArgumentException("Invalid YAML type[" + e_yamlValueType + "] for property[" + o_yamlChild.Name + "] with value[" + o_yamlChild.Value + "]");
                            }
                        }
                        else if (o_yamlChild.Name.ToLower().Equals("minitems"))
                        {
                            if (e_yamlValueType == YAMLValueType.Integer)
                            {
                                if ((this.o_currentElement.Type != null) && (!this.o_currentElement.Type.ToLower().Equals("array")))
                                {
                                    throw new ArgumentException(this.o_currentElement.Name + " - Invalid YAML restriction[" + o_yamlChild.Name + "] for [" + this.o_currentElement.Name + "] with type[" + o_yamlChild.Type + "], type must be array");
                                }

                                this.o_currentElement.Restrictions.Add(new YAMLRestriction(o_yamlChild.Name, this.i_level, "", int.Parse(o_yamlChild.Value ?? string.Empty)));
                            }
                            else
                            {
                                throw new ArgumentException("Invalid YAML type[" + e_yamlValueType + "] for property[" + o_yamlChild.Name + "] with value[" + o_yamlChild.Value + "]");
                            }
                        }
                        else if (o_yamlChild.Name.ToLower().Equals("minimum"))
                        {
                            if (e_yamlValueType == YAMLValueType.Integer)
                            {
                                this.o_currentElement.Restrictions.Add(new YAMLRestriction(o_yamlChild.Name, this.i_level, "", int.Parse(o_yamlChild.Value ?? string.Empty)));
                            }
                            else
                            {
                                throw new ArgumentException("Invalid YAML type[" + e_yamlValueType + "] for property[" + o_yamlChild.Name + "] with value[" + o_yamlChild.Value + "]");
                            }
                        }
                        else if (o_yamlChild.Name.ToLower().Equals("exclusiveminimum"))
                        {
                            if (e_yamlValueType == YAMLValueType.Integer)
                            {
                                this.o_currentElement.Restrictions.Add(new YAMLRestriction(o_yamlChild.Name, this.i_level, "", int.Parse(o_yamlChild.Value ?? string.Empty)));
                            }
                            else
                            {
                                throw new ArgumentException("Invalid YAML type[" + e_yamlValueType + "] for property[" + o_yamlChild.Name + "] with value[" + o_yamlChild.Value + "]");
                            }
                        }
                        else if (o_yamlChild.Name.ToLower().Equals("maximum"))
                        {
                            if (e_yamlValueType == YAMLValueType.Integer)
                            {
                                this.o_currentElement.Restrictions.Add(new YAMLRestriction(o_yamlChild.Name, this.i_level, "", int.Parse(o_yamlChild.Value ?? string.Empty)));
                            }
                            else
                            {
                                throw new ArgumentException("Invalid YAML type[" + e_yamlValueType + "] for property[" + o_yamlChild.Name + "] with value[" + o_yamlChild.Value + "]");
                            }
                        }
                        else if (o_yamlChild.Name.ToLower().Equals("exclusivemaximum"))
                        {
                            if (e_yamlValueType == YAMLValueType.Integer)
                            {
                                this.o_currentElement.Restrictions.Add(new YAMLRestriction(o_yamlChild.Name, this.i_level, "", int.Parse(o_yamlChild.Value ?? string.Empty)));
                            }
                            else
                            {
                                throw new ArgumentException("Invalid YAML type[" + e_yamlValueType + "] for property[" + o_yamlChild.Name + "] with value[" + o_yamlChild.Value + "]");
                            }
                        }
                        else if (o_yamlChild.Name.ToLower().Equals("maxlength"))
                        {
                            if (e_yamlValueType == YAMLValueType.Integer)
                            {
                                this.o_currentElement.Restrictions.Add(new YAMLRestriction(o_yamlChild.Name, this.i_level, "", int.Parse(o_yamlChild.Value ?? string.Empty)));
                            }
                            else
                            {
                                throw new ArgumentException("Invalid YAML type[" + e_yamlValueType + "] for property[" + o_yamlChild.Name + "] with value[" + o_yamlChild.Value + "]");
                            }
                        }
                        else if (o_yamlChild.Name.ToLower().Equals("minlength"))
                        {
                            if (e_yamlValueType == YAMLValueType.Integer)
                            {
                                this.o_currentElement.Restrictions.Add(new YAMLRestriction(o_yamlChild.Name, this.i_level, "", int.Parse(o_yamlChild.Value ?? string.Empty)));
                            }
                            else
                            {
                                throw new ArgumentException("Invalid YAML type[" + e_yamlValueType + "] for property[" + o_yamlChild.Name + "] with value[" + o_yamlChild.Value + "]");
                            }
                        }
                        else if (o_yamlChild.Name.ToLower().Equals("pattern"))
                        {
                            if (e_yamlValueType == YAMLValueType.String)
                            {
                                this.o_currentElement.Restrictions.Add(new YAMLRestriction(o_yamlChild.Name, this.i_level, o_yamlChild.Value ?? string.Empty));
                            }
                            else
                            {
                                throw new ArgumentException("Invalid YAML type[" + e_yamlValueType + "] for property[" + o_yamlChild.Name + "] with value[" + o_yamlChild.Value + "]");
                            }
                        }
                        else if (o_yamlChild.Name.ToLower().Equals("required"))
                        {
                            if (e_yamlValueType == YAMLValueType.Array)
                            {
                                /* check if current element has any children */
                                if (this.o_currentElement.Children.Count < 1)
                                {
                                    throw new ArgumentNullException("Current element[" + this.o_currentElement.Name + "] must have at least one child for assigning 'required' property");
                                }

                                /* get yaml array */
                                string s_array = o_yamlChild.Value ?? string.Empty;

                                /* check if array is surrounded with '[' and ']' characters */
                                if ((!s_array.StartsWith("[")) || (!s_array.EndsWith("]")))
                                {
                                    throw new ArgumentException("Invalid format for YAML type[" + e_yamlValueType + "] for property[" + o_yamlChild.Name + "] with value[" + o_yamlChild.Value + "], must start with '[' and end with ']'");
                                }

                                /* remove surrounding '[' and ']' characters */
                                s_array = s_array.Substring(1, s_array.Length - 1 - 1);

                                /* split array in its values */
                                string[] a_arrayValues = s_array.Split(",");

                                /* iterate each array value */
                                foreach (string s_foo in a_arrayValues)
                                {
                                    string s_arrayValue = s_foo;

                                    /* check if array value is surrounded with quote characters */
                                    if ((s_arrayValue.StartsWith(this.s_stringQuote)) && (s_arrayValue.EndsWith(this.s_stringQuote)))
                                    {
                                        /* remove surrounding quote characters */
                                        s_arrayValue = s_arrayValue.Substring(1, s_arrayValue.Length - 1 - 1);
                                    }

                                    /* trim required value */
                                    s_arrayValue = s_arrayValue.Trim();

                                    bool b_requiredFound = false;
                                    List<YAMLElement>? a_children = null;

                                    /* check if we are at 'root' level */
                                    if ((this.o_currentElement.Name.Equals("Root")) && (this.o_currentElement.Level == 0))
                                    {
                                        /* look for 'properties' child */
                                        foreach (YAMLElement o_yamlCurrentElementChild in this.o_currentElement.Children)
                                        {
                                            if (o_yamlCurrentElementChild.Name.ToLower().Equals("properties"))
                                            {
                                                /* set 'properties' children as array to search for 'required' element */
                                                a_children = o_yamlCurrentElementChild.Children;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        /* set children of current element as array to search for 'required' element */
                                        a_children = this.o_currentElement.Children;
                                    }

                                    if (a_children == null)
                                    {
                                        throw new ArgumentNullException("Cannot handle required property[" + s_arrayValue + "] in array[" + o_yamlChild.Name + "] because current element[" + this.o_currentElement.Name + "] has no children or no properties");
                                    }

                                    /* iterate all children of current element to find required 'property' */
                                    foreach (YAMLElement o_yamlCurrentElementChild in a_children)
                                    {
                                        /* compare by property name */
                                        if (o_yamlCurrentElementChild.Name.ToLower().Equals(s_arrayValue.ToLower()))
                                        {
                                            b_requiredFound = true;
                                            o_yamlCurrentElementChild.Required = true;
                                            break;
                                        }
                                    }

                                    if (!b_requiredFound)
                                    {
                                        throw new ArgumentException("Required property[" + s_arrayValue + "] in array[" + o_yamlChild.Name + "] does not exist within 'properties'");
                                    }
                                }
                            }
                            else
                            {
                                throw new ArgumentException("Invalid YAML type[" + e_yamlValueType + "] for property[" + o_yamlChild.Name + "] with value[" + o_yamlChild.Value + "]");
                            }
                        }
                    }
                }
            }
        }

        /* encoding data to YAML with YAML schema */

        /// <summary>
        /// Encode c# object to a yaml file, keep existing yaml file
        /// </summary>
        /// <param name="p_o_object">source c# object to encode yaml information</param>
        /// <param name="p_s_yamlFile">destination yaml file to save encoded yaml information</param>
        /// <returns>file object with encoded yaml content</returns>
        /// <exception cref="System.IO.IOException">cannot create or access destination yaml file</exception>
        /// <exception cref="ArgumentNullException">value in schema or expected element is not available</exception>
        /// <exception cref="ArgumentException">condition failed for decoding yaml correctly</exception>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        public File YamlEncode(Object? p_o_object, string p_s_yamlFile)
        {
            return this.YamlEncode(p_o_object, p_s_yamlFile, false);
        }

        /// <summary>
        /// Encode c# object to a yaml file
        /// </summary>
        /// <param name="p_o_object">source c# object to encode yaml information</param>
        /// <param name="p_s_yamlFile">destination yaml file to save encoded yaml information</param>
        /// <param name="p_b_overwrite">true - overwrite existing yaml file, false - keep existing yaml file</param>
        /// <returns>file object with encoded yaml content</returns>
        /// <exception cref="System.IO.IOException">cannot create or access destination yaml file</exception>
        /// <exception cref="ArgumentNullException">value in schema or expected element is not available</exception>
        /// <exception cref="ArgumentException">condition failed for decoding yaml correctly</exception>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        public File YamlEncode(Object? p_o_object, string p_s_yamlFile, bool p_b_overwrite)
        {
            /* encode data to yaml content string */
            string s_yaml = this.YamlEncode(p_o_object);

            if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("Encoded YAML:" + this.s_lineBreak + s_yaml);

            /* if file does not exist we must create an new file */
            if (!File.Exists(p_s_yamlFile))
            {
                if (p_b_overwrite)
                {
                    p_b_overwrite = false;
                }
            }

            /* open (new) file */
            ForestNET.Lib.IO.File o_file = new(p_s_yamlFile, !p_b_overwrite);

            /* save yaml encoded data into file */
            o_file.ReplaceContent(s_yaml);

            /* return file object */
            return o_file;
        }

        /// <summary>
        /// Encode c# object to a yaml content string
        /// </summary>
        /// <param name="p_o_object">source c# object to encode yaml information</param>
        /// <returns>encoded yaml information from c# object as string</returns>
        /// <exception cref="NullReferenceException">value in schema or expected element is not available</exception>
        /// <exception cref="ArgumentException">condition failed for decoding yaml correctly</exception>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        public string YamlEncode(Object? p_o_object)
        {
            /* check schema field */
            if (this.o_schema == null)
            {
                throw new NullReferenceException("Cannot encode data. Schema is null.");
            }

            /* set level for PrintIndentation to zero */
            this.i_level = 0;

            /* init flag for indentation mode if first level has only collection elements */
            this.b_firstLevelCollectionElementsOnly = false;

            /* encode data to yaml recursive */
            string s_yaml = "---" + this.s_lineBreak + this.YamlEncodeRecursive(this.o_schema, p_o_object, false) + "...";

            if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("Encoded YAML:" + this.s_lineBreak + s_yaml);

            /* return yaml content string */
            return s_yaml;
        }

        /// <summary>
        /// Recursive method to encode c# object and it's fields to a yaml string
        /// </summary>
        /// <param name="p_o_yamlSchemaElement">current yaml schema element with additional information for encoding</param>
        /// <param name="p_o_object">source c# object to encode yaml information</param>
        /// <param name="p_b_parentIsCollection">hint that the parent yaml element is an array collection</param>
        /// <returns>encoded yaml information from c# object as string</returns>
        /// <exception cref="ArgumentNullException">value in schema or expected element is not available</exception>
        /// <exception cref="ArgumentException">condition failed for decoding yaml correctly</exception>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        private string YamlEncodeRecursive(YAMLElement p_o_yamlSchemaElement, Object? p_o_object, bool p_b_parentIsCollection)
        {
            string s_yaml = "";
            string s_yamlParentName = "";

            /* if type and mapping class are not set, we need at least a reference to continue */
            if ((ForestNET.Lib.Helper.IsStringEmpty(p_o_yamlSchemaElement.Type)) && (ForestNET.Lib.Helper.IsStringEmpty(p_o_yamlSchemaElement.MappingClass)))
            {
                if (p_o_yamlSchemaElement.Reference == null)
                {
                    throw new ArgumentNullException("Schema-element[" + p_o_yamlSchemaElement.Name + "] has no type, no mapping class and no reference");
                }
                else
                {
                    /* save name of current schema-element */
                    s_yamlParentName = p_o_yamlSchemaElement.Name;

                    /* set reference as current schema-element */
                    p_o_yamlSchemaElement = p_o_yamlSchemaElement.Reference;
                }
            }

            /* check if type is set */
            if ((p_o_yamlSchemaElement.Type == null) || (ForestNET.Lib.Helper.IsStringEmpty(p_o_yamlSchemaElement.Type)))
            {
                throw new ArgumentNullException("Schema-element[" + p_o_yamlSchemaElement.Name + "] has no type");
            }

            /* check if mapping class is set if schema-element is not 'items' */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_o_yamlSchemaElement.MappingClass))
            {
                if (!p_o_yamlSchemaElement.Name.ToLower().Equals("items"))
                {
                    throw new ArgumentNullException("Schema-element[" + p_o_yamlSchemaElement.Name + "] has no mapping class");
                }
            }

            if (p_o_yamlSchemaElement.Type.ToLower().Equals("object"))
            {
                ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "cast schema-object(" + p_o_yamlSchemaElement.Name + ") with schema-mapping(" + p_o_yamlSchemaElement.PrintMapping() + ") and p_o_object(" + p_o_object?.GetType().FullName + "), castonly=" + (p_o_yamlSchemaElement.MappingClass.Equals(p_o_object?.GetType().FullName)));

                Object? o_object = null;

                if (p_o_object != null)
                {
                    string s_mappingClass = p_o_yamlSchemaElement.MappingClass;

                    /* remove assembly part of mapping class type value */
                    if (s_mappingClass.Contains(','))
                    {
                        s_mappingClass = s_mappingClass.Substring(0, s_mappingClass.IndexOf(","));
                    }

                    /* cast object of p_o_object */
                    o_object = this.CastObject(p_o_yamlSchemaElement, p_o_object, s_mappingClass.Equals(p_o_object.GetType().FullName));
                }

                /* check if casted object is null */
                if (o_object == null)
                {
                    ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "casted schema-object(" + p_o_yamlSchemaElement.Name + ") with schema-mapping(" + p_o_yamlSchemaElement.PrintMapping() + ") is null");

                    /* add object null value to yaml output */
                    if ((this.i_level == 0) || (p_b_parentIsCollection))
                    {
                        s_yaml += "null" + this.s_lineBreak;
                    }
                    else
                    {
                        /* check if a parent name for this reference is set */
                        if (!ForestNET.Lib.Helper.IsStringEmpty(s_yamlParentName))
                        {
                            s_yaml += this.PrintIndentation() + s_yamlParentName + ": null" + this.s_lineBreak;
                        }
                        else
                        {
                            s_yaml += this.PrintIndentation() + p_o_yamlSchemaElement.Name + ": null" + this.s_lineBreak;
                        }
                    }
                }
                else
                {
                    /* if parent is a collection */
                    if (p_b_parentIsCollection)
                    {
                        /* increase level for PrintIndentation */
                        this.i_level++;

                        /* save name of current schema-element */
                        s_yamlParentName = p_o_yamlSchemaElement.Name;
                    }

                    /* help variable to skip children iteration */
                    bool b_childrenIteration = true;

                    /* check conditions for handling object */
                    if (p_o_yamlSchemaElement.Reference != null)
                    {
                        /* check if reference has any children */
                        if (p_o_yamlSchemaElement.Reference.Children.Count < 1)
                        {
                            /* check if reference has another reference */
                            if (p_o_yamlSchemaElement.Reference.Reference == null)
                            {
                                throw new ArgumentNullException("Reference[" + p_o_yamlSchemaElement.Reference.Name + "] of schema-element[" + p_o_yamlSchemaElement.Name + "] has no children and no other reference");
                            }
                            else
                            {
                                b_childrenIteration = false;

                                ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "encode schema-object(" + p_o_yamlSchemaElement.Name + ") with its reference(" + p_o_yamlSchemaElement.Reference.Name + ") and schema-mapping(" + p_o_yamlSchemaElement.PrintMapping() + ") which has another reference with recursion");

                                /* handle reference with recursion */
                                s_yaml += this.YamlEncodeRecursive(p_o_yamlSchemaElement.Reference, o_object, false);
                            }
                        }
                        else
                        {
                            ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "encode schema-object(" + p_o_yamlSchemaElement.Name + ") with its reference(" + p_o_yamlSchemaElement.Reference.Name + ") which has children");

                            /* set reference as current yaml element */
                            p_o_yamlSchemaElement = p_o_yamlSchemaElement.Reference;
                        }
                    }

                    /* execute children iteration */
                    if (b_childrenIteration)
                    {
                        /* check if object has any children */
                        if (p_o_yamlSchemaElement.Children.Count < 1)
                        {
                            throw new ArgumentNullException("Schema-element[" + p_o_yamlSchemaElement.Name + "] has no children");
                        }

                        /* bool flag for handle indentation and new colon for collection object */
                        bool b_once = false;

                        /* temp variable for indentation string */
                        string s_indentation = this.PrintIndentation();

                        /* if parent is a collection */
                        if (p_b_parentIsCollection)
                        {
                            /* add colon to YAML */
                            s_yaml += s_indentation.Substring(0, (s_indentation.Length - 2)) + "- ";

                            /* set flag */
                            b_once = true;
                        }
                        else if (this.i_level > 0)
                        { /* if level is greater than zero */
                            /* print object name */
                            if (!ForestNET.Lib.Helper.IsStringEmpty(s_yamlParentName))
                            {
                                s_yaml += this.PrintIndentation() + s_yamlParentName + ":" + this.s_lineBreak;
                            }
                            else
                            {
                                s_yaml += this.PrintIndentation() + p_o_yamlSchemaElement.Name + ":" + this.s_lineBreak;
                            }

                            /* increase level for PrintIndentation */
                            this.i_level++;
                        }

                        /* iterate all children of current yaml element */
                        foreach (YAMLElement o_yamlElement in p_o_yamlSchemaElement.Children)
                        {
                            ForestNET.Lib.Global.ILogFinest(this.PrintIndentation() + "encode schema-object's(" + p_o_yamlSchemaElement.Name + ") child(" + o_yamlElement.Name + ") with schema-mapping(" + p_o_yamlSchemaElement.PrintMapping() + ") with recursion");

                            /* handle child with recursion */
                            string s_foo = this.YamlEncodeRecursive(o_yamlElement, o_object, false);

                            /* if parent is a collection */
                            if (b_once)
                            {
                                /* add recursive result, ignoring one indentation because of collection object */
                                s_yaml += s_foo.Substring(s_indentation.Length);

                                /* reset flag */
                                b_once = false;
                            }
                            else
                            {
                                /* add recursive result */
                                s_yaml += s_foo;
                            }
                        }

                        /* if parent is not a collection and level is greater than 0 */
                        if ((!p_b_parentIsCollection) && (this.i_level > 0))
                        {
                            /*decrease level for PrintIndentation */
                            this.i_level--;
                        }
                    }

                    /* decrease level for PrintIndentation if parent is a collection */
                    if (p_b_parentIsCollection)
                    {
                        this.i_level--;
                    }
                }
            }
            else if (p_o_yamlSchemaElement.Type.ToLower().Equals("array"))
            {
                if (!ForestNET.Lib.Helper.IsStringEmpty(s_yamlParentName))
                {
                    s_yaml += this.PrintIndentation() + s_yamlParentName + ": " + this.s_lineBreak;
                }
                else if (
                    (this.i_level > 0) ||
                    ((p_o_yamlSchemaElement.Reference != null) && (p_o_yamlSchemaElement.Reference.Type != null) && (p_o_yamlSchemaElement.Reference.Type.ToLower().Equals("object")) && (p_o_yamlSchemaElement.Level == p_o_yamlSchemaElement.Reference.Level) && (!p_o_yamlSchemaElement.Name.Equals("Root"))) ||
                    ((p_o_yamlSchemaElement.Reference == null) && (p_o_yamlSchemaElement.Children.Count == 1) && (p_o_yamlSchemaElement.Children[0].Name.ToLower().Equals("items")) && (ForestNET.Lib.Helper.IsStringEmpty(p_o_yamlSchemaElement.Children[0].Type)) && (ForestNET.Lib.Helper.IsStringEmpty(p_o_yamlSchemaElement.Children[0].MappingClass)))
                )
                {
                    s_yaml += this.PrintIndentation() + p_o_yamlSchemaElement.Name + ": " + this.s_lineBreak;
                }
                else if (this.i_level <= 1)
                {
                    this.b_firstLevelCollectionElementsOnly = true;
                }

                /* check conditions for handling array */
                if (p_o_yamlSchemaElement.Reference != null)
                {
                    if (p_o_yamlSchemaElement.Reference.Children.Count < 1)
                    {
                        throw new ArgumentException("Reference[" + p_o_yamlSchemaElement.Reference.Name + "] of schema-element[" + p_o_yamlSchemaElement.Name + "] with schema-type[" + p_o_yamlSchemaElement.Type + "] must have at least one child");
                    }

                    ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "encode schema-array(" + p_o_yamlSchemaElement.Name + ") with its reference(" + p_o_yamlSchemaElement.Reference.Name + ") which has children");
                }
                else
                {
                    if (p_o_yamlSchemaElement.Children.Count != 1)
                    {
                        throw new ArgumentException("Schema-element[" + p_o_yamlSchemaElement.Name + "] with schema-type[" + p_o_yamlSchemaElement.Type + "] must have just one child");
                    }

                    if (!p_o_yamlSchemaElement.Children[0].Name.ToLower().Equals("items"))
                    {
                        throw new ArgumentException("Schema-element[" + p_o_yamlSchemaElement.Name + "] with schema-type[" + p_o_yamlSchemaElement.Name + "] must have one child with name[items]");
                    }

                    ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "encode schema-array(" + p_o_yamlSchemaElement.Name + ") with child(items) which has children");
                }

                ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "encode schema-array(" + p_o_yamlSchemaElement.Name + ") - and cast object with p_o_object(" + p_o_object?.GetType().FullName + ") and schema-mapping(" + p_o_yamlSchemaElement.PrintMapping() + "), castonly if List=" + ((p_o_object != null) && (p_o_object.GetType().IsGenericType) && (p_o_object.GetType().GetGenericTypeDefinition() == typeof(List<>))));

                /* get array data from object */
                Object? o_object = this.CastObject(p_o_yamlSchemaElement, p_o_object, ((p_o_object != null) && (p_o_object.GetType().IsGenericType) && (p_o_object.GetType().GetGenericTypeDefinition() == typeof(List<>))));

                /* check if we have a primitive array and current value is just empty/null */
                if ((o_object == null) && (p_o_yamlSchemaElement.Children[0].PrimitiveArray))
                {
                    s_yaml = s_yaml.Substring(0, (s_yaml.Length - this.s_lineBreak.Length)) + "[]" + this.s_lineBreak;
                }
                else if (o_object != null)
                {
                    /* cast current object as list with unknown generic type */
                    List<Object?> a_objects = [.. ((System.Collections.IEnumerable)o_object).Cast<Object?>()];

                    /* flag if we must handle generic list as primitive array */
                    bool b_primitiveArray = p_o_yamlSchemaElement.Children.Count > 0 && p_o_yamlSchemaElement.Children[0].PrimitiveArray;

                    /* check minItems and maxItems restrictions */
                    if (p_o_yamlSchemaElement.Restrictions.Count > 0)
                    {
                        foreach (YAMLRestriction o_yamlRestriction in p_o_yamlSchemaElement.Restrictions)
                        {
                            if (o_yamlRestriction.Name.ToLower().Equals("minitems"))
                            {
                                /* check minItems restriction */
                                if (a_objects.Count < o_yamlRestriction.IntValue)
                                {
                                    throw new ArgumentException("Restriction error: not enough [" + p_o_yamlSchemaElement.Name + "] yaml items(" + a_objects.Count + "), minimum = " + o_yamlRestriction.IntValue);
                                }
                            }

                            if (o_yamlRestriction.Name.ToLower().Equals("maxitems"))
                            {
                                /* check maxItems restriction */
                                if (a_objects.Count > o_yamlRestriction.IntValue)
                                {
                                    throw new ArgumentException("Restriction error: too many [" + p_o_yamlSchemaElement.Name + "] yaml items(" + a_objects.Count + "), maximum = " + o_yamlRestriction.IntValue);
                                }
                            }
                        }
                    }

                    if (p_o_yamlSchemaElement.Reference != null)
                    {
                        /* set reference as current yaml element */
                        p_o_yamlSchemaElement = p_o_yamlSchemaElement.Reference;
                    }
                    else
                    {
                        /* set current yaml element to 'items' child */
                        p_o_yamlSchemaElement = p_o_yamlSchemaElement.Children[0];

                        /* if 'items' child has a child as well, we take this child as current yaml element */
                        if (p_o_yamlSchemaElement.Children.Count == 1)
                        {
                            p_o_yamlSchemaElement = p_o_yamlSchemaElement.Children[0];
                        }
                    }

                    /* iterate objects in list and encode data to yaml recursively */
                    for (int i = 0; i < a_objects.Count; i++)
                    {
                        /* check if array object value is null */
                        if (a_objects[i] == null)
                        {
                            ForestNET.Lib.Global.ILogFinest(this.PrintIndentation() + "encode schema-array(" + p_o_yamlSchemaElement.Name + ") with array element(null) and schema-mapping(" + p_o_yamlSchemaElement.PrintMapping() + ")");

                            /* increase level for PrintIndentation */
                            this.i_level++;

                            /* temp variable for indentation string */
                            string s_indentation = this.PrintIndentation();

                            /* add colon with null value to YAML */
                            s_yaml += s_indentation.Substring(0, (s_indentation.Length - 2)) + "- null" + this.s_lineBreak;

                            /* decrease level for PrintIndentation */
                            this.i_level--;
                        }
                        else if (b_primitiveArray)
                        {
                            ForestNET.Lib.Global.ILogFinest(this.PrintIndentation() + "encode schema-array(" + p_o_yamlSchemaElement.Name + ") with primitive array element(#" + (i + 1) + ") and schema-mapping(" + p_o_yamlSchemaElement.PrintMapping() + ")");

                            /* increase level for PrintIndentation */
                            this.i_level++;

                            /* temp variable for indentation string */
                            string s_indentation = this.PrintIndentation();

                            /* add colon with primitive array element value to YAML */
                            s_yaml += s_indentation.Substring(0, (s_indentation.Length - 2)) + "- " + a_objects[i] + this.s_lineBreak;

                            /* decrease level for PrintIndentation */
                            this.i_level--;
                        }
                        else
                        {
                            ForestNET.Lib.Global.ILogFinest(this.PrintIndentation() + "encode schema-array(" + p_o_yamlSchemaElement.Name + ") with array element(" + a_objects[i]?.GetType().FullName + ") and schema-mapping(" + p_o_yamlSchemaElement.PrintMapping() + ") with recursion");

                            /* handle object with recursion */
                            s_yaml += this.YamlEncodeRecursive(p_o_yamlSchemaElement, a_objects[i], true);
                        }
                    }

                    if (a_objects.Count < 1)
                    {
                        s_yaml = s_yaml.Substring(0, (s_yaml.Length - this.s_lineBreak.Length)) + "[]" + this.s_lineBreak;
                    }
                }
            }
            else
            {
                /* set object variable with current object */
                Object? o_object = p_o_object;

                ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "encode schema-property(" + p_o_yamlSchemaElement.Name + ") with p_o_object(" + p_o_object?.GetType().FullName + ")");

                /* get object property if we have not an array with items */
                if (!p_o_yamlSchemaElement.Name.ToLower().Equals("items"))
                {
                    ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "encode schema-property(" + p_o_yamlSchemaElement.Name + "), cast object with p_o_object(" + p_o_object?.GetType().FullName + ")");

                    /* get object property of current yaml element */
                    o_object = this.CastObject(p_o_yamlSchemaElement, p_o_object);
                }

                ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "encode schema-property(" + p_o_yamlSchemaElement.Name + "), cast string from object with p_o_object(" + p_o_object?.GetType().FullName + ") and schema-type(" + p_o_yamlSchemaElement.Type + ")");

                /* get string value of object for yaml-element */
                string s_foo = this.CastStringFromObject(o_object, p_o_yamlSchemaElement.Type);

                /* check if yaml-element is required */
                if (p_o_yamlSchemaElement.Required)
                {
                    /* check if value is empty */
                    if ((s_foo.Equals("")) || (s_foo.Equals("null")) || (s_foo.Equals(this.s_stringQuote + this.s_stringQuote)))
                    {
                        throw new ArgumentNullException("'" + p_o_yamlSchemaElement.Name + "' is required, but value[" + s_foo + "] is empty");
                    }
                }

                /* check if yaml-element has any restrictions */
                if (p_o_yamlSchemaElement.Restrictions.Count > 0)
                {
                    foreach (YAMLRestriction o_yamlRestriction in p_o_yamlSchemaElement.Restrictions)
                    {
                        /* execute restriction check */
                        this.CheckRestriction(s_foo, o_yamlRestriction, p_o_yamlSchemaElement);
                    }
                }

                /* add yaml-element with value */
                if (p_o_yamlSchemaElement.Name.ToLower().Equals("items"))
                {
                    /* increase level for PrintIndentation */
                    this.i_level++;

                    /* temp variable for indentation string */
                    string s_indentation = this.PrintIndentation();

                    /* array with items does not need captions */
                    s_yaml += ((p_b_parentIsCollection) ? s_indentation.Substring(0, (s_indentation.Length - 2)) + "- " : s_indentation + "") + s_foo + this.s_lineBreak;

                    /* decrease level for PrintIndentation */
                    this.i_level--;
                }
                else
                {
                    s_yaml += this.PrintIndentation() + p_o_yamlSchemaElement.Name + ": " + s_foo + this.s_lineBreak;
                }
            }

            return s_yaml;
        }

        /// <summary>
        /// Cast field of an object
        /// </summary>
        /// <param name="p_o_yamlElement">yaml element object	with mapping class information</param>
        /// <param name="p_o_object">object to access fields via direct public access or public access to property methods (getXX and setXX)</param>
        /// <returns>casted field value of object as object</returns>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        private Object? CastObject(YAMLElement p_o_yamlElement, Object? p_o_object)
        {
            return this.CastObject(p_o_yamlElement, p_o_object, false);
        }

        /// <summary>
        /// Cast field of an object, optional cast only
        /// </summary>
        /// <param name="p_o_yamlElement">yaml element object	with mapping class information</param>
        /// <param name="p_o_object">object to access fields via direct public access or public access to property methods (getXX and setXX)</param>
        /// <param name="b_castOnly">true - just cast object with yaml element mapping class information, false - get object field value</param>
        /// <returns>casted field value of object as object</returns>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        private Object? CastObject(YAMLElement p_o_yamlElement, Object? p_o_object, bool b_castOnly)
        {
            /* variable for object data */
            Object? o_object;

            /* check if we only have to cast the current object */
            if (b_castOnly)
            {
                /* check if parameter object is null */
                if (p_o_object != null)
                {
                    /* cast current object by string mapping value */
                    if (p_o_yamlElement.MappingClass.Equals(p_o_object.GetType().FullName))
                    {
                        o_object = Convert.ChangeType(p_o_object, p_o_object.GetType());
                    }
                    else if ((p_o_yamlElement.MappingClass.Contains(',')) && (p_o_yamlElement.MappingClass.Substring(0, p_o_yamlElement.MappingClass.IndexOf(",")).Equals(p_o_object.GetType().FullName))) /* cast current object by string mapping value, without assembly part */
                    {
                        o_object = Convert.ChangeType(p_o_object, p_o_object.GetType());
                    }
                    else
                    {
                        /* will be cast as List<Object> anyway */
                        o_object = p_o_object;
                    }
                }
                else
                {
                    /* will be cast as List<Object> anyway */
                    o_object = p_o_object;
                }
            }
            else
            {
                /* retrieve field information out of schema element */
                string s_field = p_o_yamlElement.MappingClass;

                /* if there is additional mapping information, use this for field property access */
                if (!ForestNET.Lib.Helper.IsStringEmpty(p_o_yamlElement.Mapping))
                {
                    s_field = p_o_yamlElement.Mapping;
                }

                /* remove assembly part from mapping or mapping class */
                if (s_field.Contains(','))
                {
                    s_field = s_field.Substring(0, s_field.IndexOf(","));
                }

                /* check if we use property methods with invoke to get object data values */
                if (this.UseProperties)
                {
                    /* property info for accessing property */
                    System.Reflection.PropertyInfo? o_propertyInfo;

                    /* try to get access to property info */
                    try
                    {
                        o_propertyInfo = p_o_object?.GetType().GetProperty(s_field);

                        if (o_propertyInfo == null)
                        {
                            throw new Exception("property info is null");
                        }
                    }
                    catch (Exception o_exc)
                    {
                        throw new MissingMemberException("Class instance property[" + s_field + "] does not exist for class instance(" + p_o_object?.GetType().FullName + "): " + o_exc.Message);
                    }

                    /* check if we can access property */
                    if (!o_propertyInfo.CanRead)
                    {
                        throw new MemberAccessException("Cannot write property from class; instance property[" + s_field + "] does not exist for class instance(" + p_o_object?.GetType().FullName + ")");
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
                        o_fieldInfo = p_o_object?.GetType().GetField(s_field);

                        if (o_fieldInfo == null)
                        {
                            throw new Exception("field info is null");
                        }
                    }
                    catch (Exception o_exc)
                    {
                        throw new MissingMemberException("Class instance field[" + s_field + "] does not exist for class instance(" + p_o_object?.GetType().FullName + "): " + o_exc.Message);
                    }

                    /* check if we can access field */
                    if (!o_fieldInfo.IsPublic)
                    {
                        throw new MemberAccessException("Cannot read field from class; instance field[" + s_field + "] does not exist for class instance(" + p_o_object?.GetType().FullName + ")");
                    }

                    /* get object data of current field */
                    o_object = o_fieldInfo.GetValue(p_o_object);
                }

                /* check if we have an array schema element with primitive array type */
                if ((o_object != null) && (p_o_yamlElement.Children.Count > 0) && (p_o_yamlElement.Children[0].PrimitiveArray))
                {
                    /* handle usual arrays */
                    List<string> a_primtiveArray = [];

                    /* get full name of object type */
                    string s_fullName = o_object.GetType().FullName ?? "  ";

                    /* get array type */
                    string s_arrayType = s_fullName.Substring(0, s_fullName.Length - 2).ToLower();

                    if (s_arrayType.Equals("system.boolean"))
                    {
                        /* cast current field of parameter object as array */
                        bool[] a_objects = (bool[])o_object;

                        /* only execute if we have more than one array element */
                        if (a_objects.Length > 0)
                        {
                            /* iterate objects in list and encode data to generic string list */
                            for (int i = 0; i < a_objects.Length; i++)
                            {
                                /* encode array object element and add string value to generic string list */
                                a_primtiveArray.Add(this.CastStringFromObject(a_objects[i], p_o_yamlElement.Children[0].Type));
                            }
                        }
                    }
                    else if (s_arrayType.Equals("system.byte"))
                    {
                        /* cast current field of parameter object as array */
                        byte[] a_objects = (byte[])o_object;

                        /* only execute if we have more than one array element */
                        if (a_objects.Length > 0)
                        {
                            /* iterate objects in list and encode data to generic string list */
                            for (int i = 0; i < a_objects.Length; i++)
                            {
                                /* encode array object element and add string value to generic string list */
                                a_primtiveArray.Add(this.CastStringFromObject(a_objects[i], p_o_yamlElement.Children[0].Type));
                            }
                        }
                    }
                    else if (s_arrayType.Equals("system.sbyte"))
                    {
                        /* cast current field of parameter object as array */
                        sbyte[] a_objects = (sbyte[])o_object;

                        /* only execute if we have more than one array element */
                        if (a_objects.Length > 0)
                        {
                            /* iterate objects in list and encode data to generic string list */
                            for (int i = 0; i < a_objects.Length; i++)
                            {
                                /* encode array object element and add string value to generic string list */
                                a_primtiveArray.Add(this.CastStringFromObject(a_objects[i], p_o_yamlElement.Children[0].Type));
                            }
                        }
                    }
                    else if (s_arrayType.Equals("system.char"))
                    {
                        /* cast current field of parameter object as array */
                        char[] a_objects = (char[])o_object;

                        /* only execute if we have more than one array element */
                        if (a_objects.Length > 0)
                        {
                            /* iterate objects in list and encode data to generic string list */
                            for (int i = 0; i < a_objects.Length; i++)
                            {
                                /* encode array object element and add string value to generic string list */
                                a_primtiveArray.Add(this.CastStringFromObject(a_objects[i], p_o_yamlElement.Children[0].Type));
                            }
                        }
                    }
                    else if (s_arrayType.Equals("system.single"))
                    {
                        /* cast current field of parameter object as array */
                        float[] a_objects = (float[])o_object;

                        /* only execute if we have more than one array element */
                        if (a_objects.Length > 0)
                        {
                            /* iterate objects in list and encode data to generic string list */
                            for (int i = 0; i < a_objects.Length; i++)
                            {
                                /* encode array object element and add string value to generic string list */
                                a_primtiveArray.Add(this.CastStringFromObject(a_objects[i], p_o_yamlElement.Children[0].Type));
                            }
                        }
                    }
                    else if (s_arrayType.Equals("system.double"))
                    {
                        /* cast current field of parameter object as array */
                        double[] a_objects = (double[])o_object;

                        /* only execute if we have more than one array element */
                        if (a_objects.Length > 0)
                        {
                            /* iterate objects in list and encode data to generic string list */
                            for (int i = 0; i < a_objects.Length; i++)
                            {
                                /* encode array object element and add string value to generic string list */
                                a_primtiveArray.Add(this.CastStringFromObject(a_objects[i], p_o_yamlElement.Children[0].Type));
                            }
                        }
                    }
                    else if (s_arrayType.Equals("system.int16"))
                    {
                        /* cast current field of parameter object as array */
                        short[] a_objects = (short[])o_object;

                        /* only execute if we have more than one array element */
                        if (a_objects.Length > 0)
                        {
                            /* iterate objects in list and encode data to generic string list */
                            for (int i = 0; i < a_objects.Length; i++)
                            {
                                /* encode array object element and add string value to generic string list */
                                a_primtiveArray.Add(this.CastStringFromObject(a_objects[i], p_o_yamlElement.Children[0].Type));
                            }
                        }
                    }
                    else if (s_arrayType.Equals("system.int32"))
                    {
                        /* cast current field of parameter object as array */
                        int[] a_objects = (int[])o_object;

                        /* only execute if we have more than one array element */
                        if (a_objects.Length > 0)
                        {
                            /* iterate objects in list and encode data to generic string list */
                            for (int i = 0; i < a_objects.Length; i++)
                            {
                                /* encode array object element and add string value to generic string list */
                                a_primtiveArray.Add(this.CastStringFromObject(a_objects[i], p_o_yamlElement.Children[0].Type));
                            }
                        }
                    }
                    else if (s_arrayType.Equals("system.int64"))
                    {
                        /* cast current field of parameter object as array */
                        long[] a_objects = (long[])o_object;

                        /* only execute if we have more than one array element */
                        if (a_objects.Length > 0)
                        {
                            /* iterate objects in list and encode data to generic string list */
                            for (int i = 0; i < a_objects.Length; i++)
                            {
                                /* encode array object element and add string value to generic string list */
                                a_primtiveArray.Add(this.CastStringFromObject(a_objects[i], p_o_yamlElement.Children[0].Type));
                            }
                        }
                    }
                    else if (s_arrayType.Equals("system.uint16"))
                    {
                        /* cast current field of parameter object as array */
                        ushort[] a_objects = (ushort[])o_object;

                        /* only execute if we have more than one array element */
                        if (a_objects.Length > 0)
                        {
                            /* iterate objects in list and encode data to generic string list */
                            for (int i = 0; i < a_objects.Length; i++)
                            {
                                /* encode array object element and add string value to generic string list */
                                a_primtiveArray.Add(this.CastStringFromObject(a_objects[i], p_o_yamlElement.Children[0].Type));
                            }
                        }
                    }
                    else if (s_arrayType.Equals("system.uint32"))
                    {
                        /* cast current field of parameter object as array */
                        uint[] a_objects = (uint[])o_object;

                        /* only execute if we have more than one array element */
                        if (a_objects.Length > 0)
                        {
                            /* iterate objects in list and encode data to generic string list */
                            for (int i = 0; i < a_objects.Length; i++)
                            {
                                /* encode array object element and add string value to generic string list */
                                a_primtiveArray.Add(this.CastStringFromObject(a_objects[i], p_o_yamlElement.Children[0].Type));
                            }
                        }
                    }
                    else if (s_arrayType.Equals("system.uint64"))
                    {
                        /* cast current field of parameter object as array */
                        ulong[] a_objects = (ulong[])o_object;

                        /* only execute if we have more than one array element */
                        if (a_objects.Length > 0)
                        {
                            /* iterate objects in list and encode data to generic string list */
                            for (int i = 0; i < a_objects.Length; i++)
                            {
                                /* encode array object element and add string value to generic string list */
                                a_primtiveArray.Add(this.CastStringFromObject(a_objects[i], p_o_yamlElement.Children[0].Type));
                            }
                        }
                    }
                    else if (s_arrayType.Equals("system.string"))
                    {
                        /* cast current field of parameter object as array */
                        string[] a_objects = (string[])o_object;

                        /* only execute if we have more than one array element */
                        if (a_objects.Length > 0)
                        {
                            /* iterate objects in list and encode data to generic string list */
                            for (int i = 0; i < a_objects.Length; i++)
                            {
                                /* encode array object element and add string value to generic string list */
                                a_primtiveArray.Add(this.CastStringFromObject(a_objects[i], p_o_yamlElement.Children[0].Type));
                            }
                        }
                    }
                    else if (s_arrayType.Equals("system.datetime"))
                    {
                        /* cast current field of parameter object as array */
                        DateTime[] a_objects = (DateTime[])o_object;

                        /* only execute if we have more than one array element */
                        if (a_objects.Length > 0)
                        {
                            /* iterate objects in list and encode data to generic string list */
                            for (int i = 0; i < a_objects.Length; i++)
                            {
                                /* encode array object element and add string value to generic string list */
                                a_primtiveArray.Add(this.CastStringFromObject(a_objects[i], p_o_yamlElement.Children[0].Type));
                            }
                        }
                    }
                    else if (s_arrayType.Equals("system.timespan"))
                    {
                        /* cast current field of parameter object as array */
                        TimeSpan[] a_objects = (TimeSpan[])o_object;

                        /* only execute if we have more than one array element */
                        if (a_objects.Length > 0)
                        {
                            /* iterate objects in list and encode data to generic string list */
                            for (int i = 0; i < a_objects.Length; i++)
                            {
                                /* encode array object element and add string value to generic string list */
                                a_primtiveArray.Add(this.CastStringFromObject(a_objects[i], p_o_yamlElement.Children[0].Type));
                            }
                        }
                    }
                    else if (s_arrayType.Equals("system.decimal"))
                    {
                        /* cast current field of parameter object as array */
                        decimal[] a_objects = (decimal[])o_object;

                        /* only execute if we have more than one array element */
                        if (a_objects.Length > 0)
                        {
                            /* iterate objects in list and encode data to generic string list */
                            for (int i = 0; i < a_objects.Length; i++)
                            {
                                /* encode array object element and add string value to generic string list */
                                a_primtiveArray.Add(this.CastStringFromObject(a_objects[i], p_o_yamlElement.Children[0].Type));
                            }
                        }
                    }
                    else if (s_arrayType.Equals("system.object"))
                    {
                        /* cast current field of parameter object as array */
                        Object[] a_objects = (Object[])o_object;

                        /* only execute if we have more than one array element */
                        if (a_objects.Length > 0)
                        {
                            /* iterate objects in list and encode data to generic string list */
                            for (int i = 0; i < a_objects.Length; i++)
                            {
                                if (a_objects[i] == null)
                                {
                                    /* it is allowed to have null value for type number, especially Object */
                                    a_primtiveArray.Add("null");
                                }
                                else
                                {
                                    /* encode array object element and add string value to generic string list */
                                    a_primtiveArray.Add(this.CastStringFromObject(a_objects[i], p_o_yamlElement.Children[0].Type));
                                }
                            }
                        }
                    }

                    return a_primtiveArray;
                }
            }

            return o_object;
        }

        /// <summary>
        /// Method to cast an object value to a string value for encoding yaml data
        /// </summary>
        /// <param name="p_o_object">object value which will be casted to string</param>
        /// <param name="p_s_type">type as string to distinguish</param>
        /// <returns>casted object value as string</returns>
        /// <exception cref="ArgumentException">invalid or empty type value</exception>
        private string CastStringFromObject(Object? p_o_object, string? p_s_type)
        {
            string s_foo = "";
            p_s_type = p_s_type?.ToLower();

            if (p_s_type == null)
            {
                throw new ArgumentException("Invalid type[null] for " + p_o_object?.GetType().FullName);
            }

            if (p_o_object != null)
            {
                try
                {
                    if (p_s_type.Equals("string"))
                    {
                        if (p_o_object.GetType() == typeof(DateTime))
                        {
                            DateTime o_foo = Convert.ToDateTime(p_o_object);
                            s_foo = ForestNET.Lib.Helper.ToISO8601UTC(o_foo);
                        }
                        else if (p_o_object.GetType() == typeof(TimeSpan))
                        {
                            DateTime o_foo = new DateTime(1900, 1, 1).Date + (TimeSpan)p_o_object;
                            s_foo = ForestNET.Lib.Helper.ToISO8601UTC(o_foo);
                        }
                        else
                        {
                            s_foo = p_o_object.ToString() ?? string.Empty;
                        }

                        string[] a_reservedCharacters = new string[] { ":", "{", "}", "[", "]", ",", "&", "*", "#", "?", "|", "-", "<", ">", "=", "!", "%", "@", "\\", "'", this.s_stringQuote };

                        bool b_needToQuote = false;

                        foreach (string s_reserverdCharacter in a_reservedCharacters)
                        {
                            if (s_foo.Contains(s_reserverdCharacter))
                            {
                                b_needToQuote = true;
                            }
                        }

                        if (s_foo.Contains(this.s_stringQuote))
                        {
                            s_foo = s_foo.Replace(this.s_stringQuote, "\\" + this.s_stringQuote);
                        }

                        if ((b_needToQuote) || (s_foo.Equals("null")) || (s_foo.Length == 0))
                        {
                            s_foo = this.s_stringQuote + s_foo + this.s_stringQuote;
                        }
                    }
                    else if (p_s_type.Equals("number"))
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
                    else if (p_s_type.Equals("integer"))
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
                    else if (p_s_type.Equals("boolean"))
                    {
                        bool o_foo = Convert.ToBoolean(p_o_object);
                        s_foo = o_foo.ToString();
                    }
                    else if (p_s_type.Equals("null"))
                    {
                        s_foo = "null";
                    }
                    else
                    {
                        throw new ArgumentException("Invalid type[" + p_s_type + "] for " + p_o_object.GetType().FullName);
                    }
                }
                catch (Exception o_exc)
                {
                    throw new ArgumentException("Cannot cast value[" + p_o_object.ToString() + "] to type[" + p_s_type + "]: " + o_exc.Message);
                }
            }
            else
            {
                if ((p_s_type.Equals("number")) || (p_s_type.Equals("integer")) || (p_s_type.Equals("boolean")))
                {
                    throw new ArgumentException("Invalid value[null] for type[" + p_s_type + "]");
                }

                s_foo = "null";
            }

            return s_foo;
        }

        /// <summary>
        /// Check if yaml element restriction is valid with current value
        /// </summary>
        /// <param name="p_s_value">string value for yaml element restriction, can be casted to integer as well</param>
        /// <param name="p_o_yamlRestriction">yaml restriction object which holds all restriction information</param>
        /// <param name="p_o_yamlElement">yaml element object</param>
        /// <exception cref="ArgumentException">unknown restriction name, restriction error or invalid type from yaml element object</exception>
        private void CheckRestriction(string? p_s_value, YAMLRestriction p_o_yamlRestriction, YAMLElement p_o_yamlElement)
        {
            /* check if parameter value is null */
            if (p_s_value == null)
            {
                throw new ArgumentException("Restriction error: value[null] is null for " + p_o_yamlRestriction.Name + " restriction[" + p_o_yamlRestriction.StrValue + "]");
            }

            /* check if type of yaml parameter is null */
            if (p_o_yamlElement.Type == null)
            {
                throw new ArgumentException("Restriction error: value[" + p_s_value + "] for " + p_o_yamlRestriction.Name + " restriction[" + p_o_yamlRestriction.StrValue + "] cannot be used, because yaml element parameter type property is 'null'");
            }

            string p_s_type = p_o_yamlElement.Type.ToLower();

            /* remove surrounding string quote characters */
            if ((p_s_value.StartsWith(this.s_stringQuote)) && (p_s_value.EndsWith(this.s_stringQuote)))
            {
                p_s_value = p_s_value.Substring(1, p_s_value.Length - 1 - 1);
            }

            if (p_o_yamlRestriction.Name.ToLower().Equals("minimum"))
            {
                if (p_s_type.Equals("number"))
                {
                    decimal o_value = Convert.ToDecimal(p_s_value, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    decimal o_restriction = Convert.ToDecimal(p_o_yamlRestriction.StrValue, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    int i_compare = o_value.CompareTo(o_restriction);

                    if (i_compare == -1)
                    {
                        throw new ArgumentException("Restriction error: value[" + o_value + "] does not match " + p_o_yamlRestriction.Name + " restriction[" + p_o_yamlRestriction.StrValue + "]");
                    }
                }
                else if (p_s_type.Equals("integer"))
                {
                    int i_value = int.Parse(p_s_value);
                    int i_restriction = p_o_yamlRestriction.IntValue;
                    int i_compare = i_value.CompareTo(i_restriction);

                    if (i_compare == -1)
                    {
                        throw new ArgumentException("Restriction error: value[" + i_value + "] does not match " + p_o_yamlRestriction.Name + " restriction[" + p_o_yamlRestriction.IntValue + "]");
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid type[" + p_s_type + "] of '" + p_o_yamlElement.Name + "' using restriction[" + p_o_yamlRestriction.Name + "]");
                }
            }
            else if (p_o_yamlRestriction.Name.ToLower().Equals("exclusiveminimum"))
            {
                if (p_s_type.Equals("number"))
                {
                    decimal o_value = Convert.ToDecimal(p_s_value, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    decimal o_restriction = Convert.ToDecimal(p_o_yamlRestriction.StrValue, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    int i_compare = o_value.CompareTo(o_restriction);

                    if ((i_compare == -1) || (i_compare == 0))
                    {
                        throw new ArgumentException("Restriction error: value[" + o_value + "] does not match " + p_o_yamlRestriction.Name + " restriction[" + p_o_yamlRestriction.StrValue + "]");
                    }
                }
                else if (p_s_type.Equals("integer"))
                {
                    int i_value = int.Parse(p_s_value);
                    int i_restriction = p_o_yamlRestriction.IntValue;
                    int i_compare = i_value.CompareTo(i_restriction);

                    if ((i_compare == -1) || (i_compare == 0))
                    {
                        throw new ArgumentException("Restriction error: value[" + i_value + "] does not match " + p_o_yamlRestriction.Name + " restriction[" + p_o_yamlRestriction.IntValue + "]");
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid type[" + p_s_type + "] of '" + p_o_yamlElement.Name + "' using restriction[" + p_o_yamlRestriction.Name + "]");
                }
            }
            else if (p_o_yamlRestriction.Name.ToLower().Equals("maximum"))
            {
                if (p_s_type.Equals("number"))
                {
                    decimal o_value = Convert.ToDecimal(p_s_value, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    decimal o_restriction = Convert.ToDecimal(p_o_yamlRestriction.StrValue, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    int i_compare = o_value.CompareTo(o_restriction);

                    if (i_compare == 1)
                    {
                        throw new ArgumentException("Restriction error: value[" + o_value + "] does not match " + p_o_yamlRestriction.Name + " restriction[" + p_o_yamlRestriction.StrValue + "]");
                    }
                }
                else if (p_s_type.Equals("integer"))
                {
                    int i_value = int.Parse(p_s_value);
                    int i_restriction = p_o_yamlRestriction.IntValue;
                    int i_compare = i_value.CompareTo(i_restriction);

                    if (i_compare == 1)
                    {
                        throw new ArgumentException("Restriction error: value[" + i_value + "] does not match " + p_o_yamlRestriction.Name + " restriction[" + p_o_yamlRestriction.IntValue + "]");
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid type[" + p_s_type + "] of '" + p_o_yamlElement.Name + "' using restriction[" + p_o_yamlRestriction.Name + "]");
                }
            }
            else if (p_o_yamlRestriction.Name.ToLower().Equals("exclusivemaximum"))
            {
                if (p_s_type.Equals("number"))
                {
                    decimal o_value = Convert.ToDecimal(p_s_value, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    decimal o_restriction = Convert.ToDecimal(p_o_yamlRestriction.StrValue, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    int i_compare = o_value.CompareTo(o_restriction);

                    if ((i_compare == 1) || (i_compare == 0))
                    {
                        throw new ArgumentException("Restriction error: value[" + o_value + "] does not match " + p_o_yamlRestriction.Name + " restriction[" + p_o_yamlRestriction.StrValue + "]");
                    }
                }
                else if (p_s_type.Equals("integer"))
                {
                    int i_value = int.Parse(p_s_value);
                    int i_restriction = p_o_yamlRestriction.IntValue;
                    int i_compare = i_value.CompareTo(i_restriction);

                    if ((i_compare == 1) || (i_compare == 0))
                    {
                        throw new ArgumentException("Restriction error: value[" + i_value + "] does not match " + p_o_yamlRestriction.Name + " restriction[" + p_o_yamlRestriction.IntValue + "]");
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid type[" + p_s_type + "] of '" + p_o_yamlElement.Name + "' using restriction[" + p_o_yamlRestriction.Name + "]");
                }
            }
            else if (p_o_yamlRestriction.Name.ToLower().Equals("minlength"))
            {
                if (p_s_type.Equals("string"))
                {
                    if (p_s_value.Length < p_o_yamlRestriction.IntValue)
                    {
                        throw new ArgumentException("Restriction error: value[" + p_s_value + "] does not match " + p_o_yamlRestriction.Name + " restriction[" + p_o_yamlRestriction.IntValue + "]");
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid type[" + p_s_type + "] of '" + p_o_yamlElement.Name + "' using restriction[" + p_o_yamlRestriction.Name + "]");
                }
            }
            else if (p_o_yamlRestriction.Name.ToLower().Equals("maxlength"))
            {
                if (p_s_type.Equals("string"))
                {
                    if (p_s_value.Length > p_o_yamlRestriction.IntValue)
                    {
                        throw new ArgumentException("Restriction error: value[" + p_s_value + "] does not match " + p_o_yamlRestriction.Name + " restriction[" + p_o_yamlRestriction.IntValue + "]");
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid type[" + p_s_type + "] of '" + p_o_yamlElement.Name + "' using restriction[" + p_o_yamlRestriction.Name + "]");
                }
            }
            else if (p_o_yamlRestriction.Name.ToLower().Equals("pattern"))
            {
                if ((p_s_type.Equals("string")) || (p_s_type.Equals("boolean")) || (p_s_type.Equals("number")) || (p_s_type.Equals("integer")))
                {
                    if (!ForestNET.Lib.Helper.MatchesRegex(p_s_value, p_o_yamlRestriction.StrValue))
                    {
                        throw new ArgumentException("Restriction error: value[" + p_s_value + "] does not match " + p_o_yamlRestriction.Name + " restriction[" + p_o_yamlRestriction.StrValue + "]");
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid type[" + p_s_type + "] of '" + p_o_yamlElement.Name + "' using restriction[" + p_o_yamlRestriction.Name + "]");
                }
            }
            else
            {
                throw new ArgumentException("Unknown Restriction: " + p_o_yamlRestriction.Name);
            }
        }

        /* validating yaml data with YAML schema */

        /// <summary>
        /// Validate yaml file
        /// </summary>
        /// <param name="p_s_yamlFile">full-path to yaml file</param>
        /// <returns>true - content of yaml file is valid, false - content of yaml file is invalid</returns>
        /// <exception cref="ArgumentException">yaml file does not exist</exception>
        /// <exception cref="System.IO.IOException">cannot read yaml file content</exception>
        /// <exception cref="ArgumentNullException">empty schema, empty yaml file or root node after parsing yaml content</exception>
        public bool ValidateAgainstSchema(string p_s_yamlFile)
        {
            /* check if file exists */
            if (!ForestNET.Lib.IO.File.Exists(p_s_yamlFile))
            {
                throw new ArgumentException("File[" + p_s_yamlFile + "] does not exist.");
            }

            /* open yaml file */
            ForestNET.Lib.IO.File o_file = new(p_s_yamlFile, false);

            /* load file content into string */
            string s_fileContent = o_file.FileContent;

            List<string> a_fileLines =
            [
                /* load file content lines into array */
                .. s_fileContent.Split(this.s_lineBreak),
            ];

            ForestNET.Lib.Global.ILogFinest("read all lines from yaml file '" + p_s_yamlFile + "'");

            /* decode yaml file lines */
            return this.ValidateAgainstSchema(a_fileLines);
        }

        /// <summary>
        /// Validate yaml content
        /// </summary>
        /// <param name="p_a_yamlLines">yaml lines</param>
        /// <returns>true - yaml content is valid, false - yaml content is invalid</returns>
        /// <exception cref="ArgumentNullException">empty schema, empty yaml file or root node after parsing yaml content</exception>
        /// <exception cref="ArgumentException">condition failed for decoding yaml correctly</exception>
        public bool ValidateAgainstSchema(List<string> p_a_yamlLines)
        {
            /* check schema field */
            if (this.o_schema == null)
            {
                throw new NullReferenceException("Cannot decode data. Schema is null.");
            }

            List<string> a_fileLines = this.ValidateYAML(p_a_yamlLines);

            ForestNET.Lib.Global.ILogFinest("validated yaml content lines");

            /* check if root is null */
            if ((a_fileLines.Count == 0) || ((a_fileLines.Count == 1) && (a_fileLines[0].Equals("null"))))
            {
                throw new ArgumentException("YAML file is null", nameof(p_a_yamlLines));
            }

            /* reset level */
            this.i_level = 0;

            /* create new root */
            this.Root = new YAMLElement("Root");

            /* init flag for indentation mode if first level has only collection elements */
            this.b_firstLevelCollectionElementsOnly = false;

            /* parse yaml content lines */
            this.ParseYAML(1, a_fileLines.Count, a_fileLines, 0, this.Root);

            ForestNET.Lib.Global.ILogFinest("parsed yaml content lines");

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

            this.o_currentElement = null;

            if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("YAML-Schema:" + ForestNET.Lib.IO.File.NEWLINE + this.o_schema);
            if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("YAML-Root-Element:" + ForestNET.Lib.IO.File.NEWLINE + this.Root);

            /* validate yaml content recursively */
            return this.ValidateAgainstSchemaRecursive(this.Root, this.o_schema);
        }

        /// <summary>
        /// Recursive method to validate yaml content string
        /// </summary>
        /// <param name="p_o_yamlDataElement">current yaml data element</param>
        /// <param name="p_o_yamlSchemaElement">current yaml schema element with additional information for decoding</param>
        /// <returns>true - yaml content is valid, false - yaml content is invalid</returns>
        /// <exception cref="ArgumentNullException">value in schema or expected element is not available</exception>
        /// <exception cref="ArgumentException">condition failed for decoding yaml correctly</exception>
        private bool ValidateAgainstSchemaRecursive(YAMLElement p_o_yamlDataElement, YAMLElement p_o_yamlSchemaElement)
        {
            bool b_return = true;

            /* if type and mapping class are not set, we need at least a reference to continue */
            if ((ForestNET.Lib.Helper.IsStringEmpty(p_o_yamlSchemaElement.Type)) && (ForestNET.Lib.Helper.IsStringEmpty(p_o_yamlSchemaElement.MappingClass)))
            {
                if (p_o_yamlSchemaElement.Reference == null)
                {
                    throw new ArgumentNullException("Schema-element[" + p_o_yamlSchemaElement.Name + "] has no type, no mapping class and no reference");
                }
                else
                {
                    /* set reference as current schema-element */
                    p_o_yamlSchemaElement = p_o_yamlSchemaElement.Reference;
                }
            }

            /* check if type is set */
            if ((p_o_yamlSchemaElement.Type == null) || (ForestNET.Lib.Helper.IsStringEmpty(p_o_yamlSchemaElement.Type)))
            {
                throw new ArgumentNullException("Schema-element[" + p_o_yamlSchemaElement.Name + "] has no type");
            }

            /* check if mapping class is set if schema-element is not 'items' */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_o_yamlSchemaElement.MappingClass))
            {
                if (!p_o_yamlSchemaElement.Name.ToLower().Equals("items"))
                {
                    throw new ArgumentException("Schema-element[" + p_o_yamlSchemaElement.Name + "] has no mapping class");
                }
            }

            if (p_o_yamlSchemaElement.Type.ToLower().Equals("object"))
            {
                /* check if we have any data for new object */
                if (p_o_yamlDataElement.Children.Count > 0)
                {
                    string s_objectType = p_o_yamlSchemaElement.MappingClass;

                    /* if object has reference, we create new object instance by mapping of reference */
                    if ((p_o_yamlSchemaElement.Reference != null) && (p_o_yamlSchemaElement.Reference.Type != null) && (p_o_yamlSchemaElement.Reference.Type.ToLower().Equals("object")))
                    {
                        s_objectType = p_o_yamlSchemaElement.Reference.MappingClass;
                    }

                    ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + p_o_yamlSchemaElement.Name + ": create new schema-object instance with mapping[" + p_o_yamlSchemaElement.PrintMapping() + "] and type[" + s_objectType + "]");

                    /* increase level for PrintTabs */
                    this.i_level++;

                    /* help variable to skip children iteration */
                    bool b_childrenIteration = true;

                    /* check conditions for handling object */
                    if (p_o_yamlSchemaElement.Reference != null)
                    {
                        /* check if reference has any children */
                        if (p_o_yamlSchemaElement.Reference.Children.Count < 1)
                        {
                            /* check if reference has another reference */
                            if (p_o_yamlSchemaElement.Reference.Reference == null)
                            {
                                throw new ArgumentNullException("Reference[" + p_o_yamlSchemaElement.Reference.Name + "] of schema-element[" + p_o_yamlSchemaElement.Name + "] has no children and no other reference");
                            }
                            else
                            {
                                b_childrenIteration = false;

                                /* check if current element in schema has data element by name, otherwise skip this element */
                                if (p_o_yamlSchemaElement.Name.Equals(p_o_yamlDataElement.Name))
                                {
                                    ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + p_o_yamlSchemaElement.Name + ": get schema-object[" + p_o_yamlSchemaElement.MappingClass + "] with reference[" + p_o_yamlSchemaElement.Reference.Name + "]");

                                    /* only create new object if we have one object data */
                                    if (p_o_yamlDataElement.Children.Count != 1)
                                    {
                                        throw new ArgumentException("We have (" + p_o_yamlDataElement.Children.Count + ") no data children or more than one for schema-element[" + p_o_yamlSchemaElement.Name + "]");
                                    }

                                    /* handle reference with recursion */
                                    b_return = this.ValidateAgainstSchemaRecursive(p_o_yamlDataElement.Children[0], p_o_yamlSchemaElement.Reference);
                                }
                            }
                        }
                        else
                        {
                            ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "update current schema-element[" + p_o_yamlSchemaElement.Name + "](" + p_o_yamlSchemaElement.PrintMapping() + ") with reference[" + p_o_yamlSchemaElement.Reference.Name + "](" + p_o_yamlSchemaElement.Reference.PrintMapping() + ")");

                            /* set reference as current yaml element */
                            p_o_yamlSchemaElement = p_o_yamlSchemaElement.Reference;
                        }
                    }

                    /* execute children iteration */
                    if (b_childrenIteration)
                    {
                        ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + p_o_yamlSchemaElement.Name + ": get schema-object with type[" + p_o_yamlSchemaElement.MappingClass + "] and children[" + p_o_yamlSchemaElement.Children.Count + "]");

                        /* only create new object if we have one child definition for object in yaml schema */
                        if (p_o_yamlSchemaElement.Children.Count < 1)
                        {
                            throw new ArgumentException("Schema-element[" + p_o_yamlSchemaElement.Name + "] has no children");
                        }

                        /* check if new object is required if there is no data */
                        if ((p_o_yamlDataElement.Children.Count < 1) && (p_o_yamlSchemaElement.Required))
                        {
                            throw new ArgumentException("We have no data children for schema-element[" + p_o_yamlSchemaElement.Name + "] which is required");
                        }

                        /* only iterate if we have object data */
                        if (p_o_yamlDataElement.Children.Count > 0)
                        {
                            /* increase level for PrintTabs */
                            this.i_level++;

                            /* data pointer */
                            int j = 0;

                            for (int i = 0; i < p_o_yamlSchemaElement.Children.Count; i++)
                            {
                                ForestNET.Lib.Global.ILogFinest(this.PrintIndentation() + "compare schema-child-name(" + p_o_yamlSchemaElement.Children[i].Name + ") with data-child-name(" + p_o_yamlDataElement.Children[j].Name + ")");

                                /* check if current element in schema has data element by name, otherwise skip this element */
                                if (!p_o_yamlSchemaElement.Children[i].Name.Equals(p_o_yamlDataElement.Children[j].Name))
                                {
                                    /* but check if current schema element is required, so we must throw an exception */
                                    if (p_o_yamlSchemaElement.Children[i].Required)
                                    {
                                        throw new ArgumentException("We have no data for schema-element[" + p_o_yamlSchemaElement.Children[i].Name + "] which is required");
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }

                                /* handle new object with recursion */
                                b_return = this.ValidateAgainstSchemaRecursive(p_o_yamlDataElement.Children[j], p_o_yamlSchemaElement.Children[i]);

                                /* increase data pointer */
                                j++;
                            }

                            /* decrease level for PrintTabs */
                            this.i_level--;
                        }
                    }

                    /* decrease level for PrintTabs */
                    this.i_level--;
                }
            }
            else if (p_o_yamlSchemaElement.Type.ToLower().Equals("array"))
            {
                /* check conditions for handling array */
                if (p_o_yamlSchemaElement.Reference != null)
                {
                    if (p_o_yamlSchemaElement.Reference.Children.Count < 1)
                    {
                        throw new ArgumentException("Reference[" + p_o_yamlSchemaElement.Reference.Name + "] of schema-array[" + p_o_yamlSchemaElement.Name + "] with mapping[" + p_o_yamlSchemaElement.MappingClass + "] must have at least one child");
                    }
                }
                else
                {
                    if (p_o_yamlSchemaElement.Children.Count != 1)
                    {
                        throw new ArgumentException("Schema-array[" + p_o_yamlSchemaElement.Name + "] with mapping[" + p_o_yamlSchemaElement.MappingClass + "] must have just one child");
                    }

                    if (!p_o_yamlSchemaElement.Children[0].Name.ToLower().Equals("items"))
                    {
                        throw new ArgumentException("Schema-array[" + p_o_yamlSchemaElement.Name + "] with mapping[" + p_o_yamlSchemaElement.MappingClass + "] must have one child with name[items]");
                    }
                }

                /* help variables to handle array */
                bool b_required = false;
                string s_requiredProperty = "";
                List<YAMLRestriction> a_restrictions = [];
                string s_amountProperty = "";

                /* check if yaml-element is required */
                if (p_o_yamlSchemaElement.Required)
                {
                    b_required = true;
                    s_requiredProperty = p_o_yamlSchemaElement.Name;
                }

                /* check minItems and maxItems restrictions and save them for items check afterwards */
                if (p_o_yamlSchemaElement.Restrictions.Count > 0)
                {
                    foreach (YAMLRestriction o_yamlRestriction in p_o_yamlSchemaElement.Restrictions)
                    {
                        if ((o_yamlRestriction.Name.ToLower().Equals("minitems")) || (o_yamlRestriction.Name.ToLower().Equals("maxitems")))
                        {
                            a_restrictions.Add(o_yamlRestriction);
                            s_amountProperty = p_o_yamlSchemaElement.Name;
                        }
                    }
                }

                if (p_o_yamlSchemaElement.Reference != null)
                {
                    /* set reference as current yaml element */
                    p_o_yamlSchemaElement = p_o_yamlSchemaElement.Reference;
                }
                else
                {
                    /* set current yaml element to 'items' child */
                    p_o_yamlSchemaElement = p_o_yamlSchemaElement.Children[0];

                    /* if 'items' child has a child as well, we take this child as current yaml element */
                    if (p_o_yamlSchemaElement.Children.Count == 1)
                    {
                        p_o_yamlSchemaElement = p_o_yamlSchemaElement.Children[0];
                    }

                    /* important part for parsing collection which are not inline collection values in yaml document */
                    if ((!p_o_yamlDataElement.Name.Equals("__ArrayObject__")) && (p_o_yamlSchemaElement.Name.ToLower().Equals("items")))
                    {
                        if ((p_o_yamlDataElement.Children.Count > 0) && (p_o_yamlDataElement.Children[0].Name.Equals("__ArrayObject__")))
                        {
                            p_o_yamlDataElement = p_o_yamlDataElement.Children[0];
                        }
                    }
                }

                if (ForestNET.Lib.Helper.IsStringEmpty(p_o_yamlDataElement.Value))
                { /* we have multiple minor objects for current array */
                    ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + p_o_yamlSchemaElement.Name + ": get schema-array with mapping[" + p_o_yamlSchemaElement.PrintMapping() + "]");

                    if (p_o_yamlDataElement.Children.Count > 0)
                    { /* if we have objects to the new array */
                        /* check minItems and maxItems restrictions */
                        if (a_restrictions.Count > 0)
                        {
                            foreach (YAMLRestriction o_yamlRestriction in a_restrictions)
                            {
                                if (o_yamlRestriction.Name.ToLower().Equals("minitems"))
                                {
                                    /* check minItems restriction */
                                    if (p_o_yamlDataElement.Children.Count < o_yamlRestriction.IntValue)
                                    {
                                        throw new ArgumentException("Restriction error: not enough [" + p_o_yamlSchemaElement.Name + " of " + s_amountProperty + "] yaml items(" + p_o_yamlDataElement.Children.Count + "), minimum = " + o_yamlRestriction.IntValue);
                                    }
                                }

                                if (o_yamlRestriction.Name.ToLower().Equals("maxitems"))
                                {
                                    /* check maxItems restriction */
                                    if (p_o_yamlDataElement.Children.Count > o_yamlRestriction.IntValue)
                                    {
                                        throw new ArgumentException("Restriction error: too many [" + p_o_yamlSchemaElement.Name + " of " + s_amountProperty + "] yaml items(" + p_o_yamlDataElement.Children.Count + "), maximum = " + o_yamlRestriction.IntValue);
                                    }
                                }
                            }
                        }

                        /* iterate objects in list and encode data to yaml recursively */
                        for (int i = 0; i < p_o_yamlDataElement.Children.Count; i++)
                        {
                            /* increase level for PrintTabs */
                            this.i_level++;

                            /* handle array object with recursion */
                            b_return = this.ValidateAgainstSchemaRecursive(p_o_yamlDataElement.Children[i], p_o_yamlSchemaElement);

                            ForestNET.Lib.Global.ILogFinest(this.PrintIndentation() + " return value of " + p_o_yamlDataElement.Name + ": " + b_return);

                            /* decrease level for PrintTabs */
                            this.i_level--;
                        }
                    }
                }
                else
                { /* array objects must be retrieved out of value property */
                    ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + p_o_yamlSchemaElement.Name + ": handle array value = " + p_o_yamlDataElement.Value + " - mapping[" + p_o_yamlSchemaElement.PrintMapping() + "]");

                    /* set array with values if we have any values */
                    if ((p_o_yamlDataElement.Value != null) && (!ForestNET.Lib.Helper.IsStringEmpty(p_o_yamlDataElement.Value)) && (!p_o_yamlDataElement.Value.Equals("[]")))
                    {
                        /* remove opening and closing brackets */
                        if ((p_o_yamlDataElement.Value.StartsWith("[")) && (p_o_yamlDataElement.Value.EndsWith("]")))
                        {
                            p_o_yamlDataElement.Value = p_o_yamlDataElement.Value.Substring(1, p_o_yamlDataElement.Value.Length - 1 - 1);
                        }

                        /* split array into values, divided by ',' */
                        string[] a_values = p_o_yamlDataElement.Value.Split(",");

                        /* check minItems and maxItems restrictions */
                        if (a_restrictions.Count > 0)
                        {
                            foreach (YAMLRestriction o_yamlRestriction in a_restrictions)
                            {
                                if (o_yamlRestriction.Name.ToLower().Equals("minitems"))
                                {
                                    /* check minItems restriction */
                                    if (a_values.Length < o_yamlRestriction.IntValue)
                                    {
                                        throw new ArgumentException("Restriction error: not enough [" + p_o_yamlSchemaElement.Name + " of " + s_amountProperty + "] yaml items(" + a_values.Length + "), minimum = " + o_yamlRestriction.IntValue);
                                    }
                                }

                                if (o_yamlRestriction.Name.ToLower().Equals("maxitems"))
                                {
                                    /* check maxItems restriction */
                                    if (a_values.Length > o_yamlRestriction.IntValue)
                                    {
                                        throw new ArgumentException("Restriction error: too many [" + p_o_yamlSchemaElement.Name + " of " + s_amountProperty + "] yaml items(" + a_values.Length + "), maximum = " + o_yamlRestriction.IntValue);
                                    }
                                }
                            }
                        }

                        /* iterate all array values */
                        foreach (string s_value in a_values)
                        {
                            YAMLValueType e_yamlValueType = YAML.GetYAMLValueType(s_value);

                            /* check if YAML value types are matching between schema and data */
                            if (e_yamlValueType != YAML.StringToYAMLValueType(p_o_yamlSchemaElement.Type))
                            {
                                throw new ArgumentException("YAML schema type[" + YAML.StringToYAMLValueType(p_o_yamlSchemaElement.Type) + "] does not match with data value type[" + e_yamlValueType + "] with value[" + s_value + "]");
                            }

                            /* check if yaml-element has any restrictions */
                            if (p_o_yamlSchemaElement.Restrictions.Count > 0)
                            {
                                foreach (YAMLRestriction o_yamlRestriction in p_o_yamlSchemaElement.Restrictions)
                                {
                                    /* execute restriction check */
                                    this.CheckRestriction(s_value, o_yamlRestriction, p_o_yamlSchemaElement);
                                }
                            }

                            /* cast array string value into object */
                            Object? o_returnObject = this.CastObjectFromString(s_value, p_o_yamlSchemaElement.Type);

                            ForestNET.Lib.Global.ILogFinest(this.PrintIndentation() + "add value[" + (o_returnObject ?? "null") + "] to list of " + p_o_yamlDataElement.Name + ", type[" + ((o_returnObject == null) ? "null" : o_returnObject.GetType().FullName) + "]");
                        }
                    }
                    else if (b_required)
                    { /* if yaml-element with type array is required, throw exception */
                        throw new ArgumentException("'" + p_o_yamlSchemaElement.Name + "' of '" + s_requiredProperty + "' is required, but array has no values");
                    }
                }
            }
            else
            {
                ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + p_o_yamlSchemaElement.Name + ": handle value = " + p_o_yamlDataElement.Value + " - mapping[" + p_o_yamlSchemaElement.PrintMapping() + "]");

                YAMLValueType e_yamlValueType = YAML.GetYAMLValueType(p_o_yamlDataElement.Value);

                /* check if YAML value types are matching between schema and data, additional condition = YAML value type is not 'null' */
                if ((e_yamlValueType != YAML.StringToYAMLValueType(p_o_yamlSchemaElement.Type)) && (e_yamlValueType != YAMLValueType.Null))
                {
                    /* it is acceptable if source type is 'integer' and destination type is 'number', valid cast available */
                    if (!((e_yamlValueType == YAMLValueType.Integer) && (YAML.StringToYAMLValueType(p_o_yamlSchemaElement.Type) == YAMLValueType.Number)))
                    {
                        throw new ArgumentException("YAML schema type[" + e_yamlValueType + "] does not match with data value type[" + YAML.StringToYAMLValueType(p_o_yamlSchemaElement.Type) + "] with value[" + p_o_yamlDataElement.Value + "]");
                    }
                }

                /* check if yaml-element is required */
                if (p_o_yamlSchemaElement.Required)
                {
                    /* check if value is empty */
                    if ((p_o_yamlDataElement.Value == null) || (p_o_yamlDataElement.Value.Equals("")) || (p_o_yamlDataElement.Value.Equals("null")) || (p_o_yamlDataElement.Value.Equals("\"\"")))
                    {
                        throw new ArgumentNullException("'" + p_o_yamlSchemaElement.Name + "' is required, but value[" + p_o_yamlDataElement.Value + "] is empty");
                    }
                }

                /* check if yaml-element has any restrictions */
                if (p_o_yamlSchemaElement.Restrictions.Count > 0)
                {
                    foreach (YAMLRestriction o_yamlRestriction in p_o_yamlSchemaElement.Restrictions)
                    {
                        /* execute restriction check */
                        this.CheckRestriction(p_o_yamlDataElement.Value, o_yamlRestriction, p_o_yamlSchemaElement);
                    }
                }

                if ((ForestNET.Lib.Helper.IsStringEmpty(p_o_yamlSchemaElement.Mapping)) && (ForestNET.Lib.Helper.IsStringEmpty(p_o_yamlSchemaElement.MappingClass)))
                {
                    _ = this.CastObjectFromString(p_o_yamlDataElement.Value, p_o_yamlSchemaElement.Type);
                }
                else
                {
                    string? s_objectValue = p_o_yamlDataElement.Value;

                    /* remove surrounded double quotes from value */
                    if ((s_objectValue != null) && (s_objectValue.StartsWith(this.s_stringQuote)) && (s_objectValue.EndsWith(this.s_stringQuote)))
                    {
                        s_objectValue = s_objectValue.Substring(1, s_objectValue.Length - 1 - 1);
                    }

                    try
                    {
                        _ = this.CastObjectFromString(s_objectValue, p_o_yamlSchemaElement.Type);
                    }
                    catch (Exception)
                    {
                        try
                        {
                            _ = this.CastObjectFromString(s_objectValue, p_o_yamlSchemaElement.Mapping);
                        }
                        catch (Exception)
                        {
                            _ = this.CastObjectFromString(s_objectValue, p_o_yamlSchemaElement.MappingClass);
                        }
                    }
                }
            }

            ForestNET.Lib.Global.ILogFine(this.PrintIndentation() + "return " + b_return);

            return b_return;
        }

        /* decoding YAML to data with YAML schema */

        /// <summary>
        /// Decode yaml file to an c# object
        /// </summary>
        /// <param name="p_s_yamlFile">full-path to yaml file</param>
        /// <returns>yaml decoded c# object</returns>
        /// <exception cref="ArgumentException">yaml file does not exist</exception>
        /// <exception cref="System.IO.IOException">cannot read yaml file content</exception>
        /// <exception cref="ArgumentNullException">empty schema, empty yaml file or root node after parsing yaml content</exception>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        /// <exception cref="FormatException">exception parsing string to target type</exception>
        public Object? YamlDecode(string p_s_yamlFile)
        {
            /* check if file exists */
            if (!ForestNET.Lib.IO.File.Exists(p_s_yamlFile))
            {
                throw new ArgumentException("File[" + p_s_yamlFile + "] does not exist.");
            }

            /* open yaml file */
            ForestNET.Lib.IO.File o_file = new(p_s_yamlFile, false);

            /* load file content into string */
            string s_fileContent = o_file.FileContent;

            List<string> a_fileLines =
            [
                /* load file content lines into array */
                .. s_fileContent.Split(this.s_lineBreak),
            ];

            ForestNET.Lib.Global.ILogFinest("read all lines from yaml file '" + p_s_yamlFile + "'");

            /* decode yaml file lines */
            return this.YamlDecode(a_fileLines);
        }

        /// <summary>
        /// Decode yaml content to an c# object
        /// </summary>
        /// <param name="p_a_yamlLines">yaml lines</param>
        /// <returns>yaml decoded c# object</returns>
        /// <exception cref="ArgumentNullException">empty schema, empty yaml file or root node after parsing yaml content</exception>
        /// <exception cref="ArgumentException">condition failed for decoding yaml correctly</exception>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        /// <exception cref="FormatException">exception parsing string to target type</exception>
        public Object? YamlDecode(List<string> p_a_yamlLines)
        {
            Object? o_foo = null;

            /* check schema field */
            if (this.o_schema == null)
            {
                throw new NullReferenceException("Cannot decode data. Schema is null.");
            }

            List<string> a_fileLines = this.ValidateYAML(p_a_yamlLines);

            ForestNET.Lib.Global.ILogFinest("validated yaml content lines");

            /* check if root is null */
            if ((a_fileLines.Count == 0) || ((a_fileLines.Count == 1) && (a_fileLines[0].Equals("null"))))
            {
                throw new ArgumentException("YAML file is null", nameof(p_a_yamlLines));
            }

            /* reset level */
            this.i_level = 0;

            /* create new root */
            this.Root = new YAMLElement("Root");

            /* init flag for indentation mode if first level has only collection elements */
            this.b_firstLevelCollectionElementsOnly = false;

            /* parse yaml content lines */
            this.ParseYAML(1, a_fileLines.Count, a_fileLines, 0, this.Root);

            ForestNET.Lib.Global.ILogFinest("parsed yaml content lines");

            /* check if root is null */
            if (this.Root == null)
            {
                throw new NullReferenceException("Root node is null");
            }

            /* check if root has any children */
            if (this.Root.Children.Count == 0)
            {
                throw new ArgumentNullException("Root node has no children[size=" + this.Root.Children.Count + "]");
            }

            this.o_currentElement = null;

            if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("YAML-Schema:" + ForestNET.Lib.IO.File.NEWLINE + this.o_schema);
            if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("YAML-Root-Element:" + ForestNET.Lib.IO.File.NEWLINE + this.Root);

            /* decode yaml recursively */
            o_foo = this.YamlDecodeRecursive(this.Root, this.o_schema, o_foo);

            return o_foo;
        }

        /// <summary>
        /// Recursive method to decode yaml string to a c# object and it's fields
        /// </summary>
        /// <param name="p_o_yamlDataElement">current yaml data element</param>
        /// <param name="p_o_yamlSchemaElement">current yaml schema element with additional information for decoding</param>
        /// <param name="p_o_object">destination c# object to decode yaml information</param>
        /// <returns>decoded yaml information as c# object</returns>
        /// <exception cref="ArgumentNullException">value in schema or expected element is not available</exception>
        /// <exception cref="ArgumentException">condition failed for decoding yaml correctly</exception>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        /// <exception cref="FormatException">exception parsing string to target type</exception>
        private Object? YamlDecodeRecursive(YAMLElement p_o_yamlDataElement, YAMLElement p_o_yamlSchemaElement, Object? p_o_object)
        {
            /* if type and mapping class are not set, we need at least a reference to continue */
            if ((ForestNET.Lib.Helper.IsStringEmpty(p_o_yamlSchemaElement.Type)) && (ForestNET.Lib.Helper.IsStringEmpty(p_o_yamlSchemaElement.MappingClass)))
            {
                if (p_o_yamlSchemaElement.Reference == null)
                {
                    throw new ArgumentNullException("Schema-element[" + p_o_yamlSchemaElement.Name + "] has no type, no mapping class and no reference");
                }
                else
                {
                    /* set reference as current schema-element */
                    p_o_yamlSchemaElement = p_o_yamlSchemaElement.Reference;
                }
            }

            /* check if type is set */
            if ((p_o_yamlSchemaElement.Type == null) || (ForestNET.Lib.Helper.IsStringEmpty(p_o_yamlSchemaElement.Type)))
            {
                throw new ArgumentNullException("Schema-element[" + p_o_yamlSchemaElement.Name + "] has no type");
            }

            /* check if mapping class is set if schema-element is not 'items' */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_o_yamlSchemaElement.MappingClass))
            {
                if (!p_o_yamlSchemaElement.Name.ToLower().Equals("items"))
                {
                    throw new ArgumentException("Schema-element[" + p_o_yamlSchemaElement.Name + "] has no mapping class");
                }
            }

            if (p_o_yamlSchemaElement.Type.ToLower().Equals("object"))
            {
                /* check if we have any data for new object */
                if (p_o_yamlDataElement.Children.Count < 1)
                {
                    /* set current object as null */
                    p_o_object = null;
                }
                else
                {
                    string s_objectType = p_o_yamlSchemaElement.MappingClass;

                    /* if object has reference, we create new object instance by mapping of reference */
                    if ((p_o_yamlSchemaElement.Reference != null) && (p_o_yamlSchemaElement.Reference.Type != null) && (p_o_yamlSchemaElement.Reference.Type.ToLower().Equals("object")))
                    {
                        s_objectType = p_o_yamlSchemaElement.Reference.MappingClass;
                    }

                    ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + p_o_yamlSchemaElement.Name + ": create new schema-object instance with mapping[" + p_o_yamlSchemaElement.PrintMapping() + "] and type[" + s_objectType + "]");

                    /* object variable which will be returned at the end of this function */
                    Object? o_object;

                    /* create new object instance which will be returned at the end of this function */
                    o_object = Activator.CreateInstance(Type.GetType(s_objectType) ?? throw new NullReferenceException("Could not create instance by object type '" + s_objectType + "'"));

                    /* increase level for PrintTabs */
                    this.i_level++;

                    /* help variable to skip children iteration */
                    bool b_childrenIteration = true;

                    /* help variable for object mapping within objects */
                    string s_objectMapping = p_o_yamlSchemaElement.MappingClass;

                    /* if there is additional mapping information, use this for object mapping access */
                    if (!ForestNET.Lib.Helper.IsStringEmpty(p_o_yamlSchemaElement.Mapping))
                    {
                        s_objectMapping = p_o_yamlSchemaElement.Mapping;
                    }

                    /* check conditions for handling object */
                    if (p_o_yamlSchemaElement.Reference != null)
                    {
                        /* check if reference has any children */
                        if (p_o_yamlSchemaElement.Reference.Children.Count < 1)
                        {
                            /* check if reference has another reference */
                            if (p_o_yamlSchemaElement.Reference.Reference == null)
                            {
                                throw new ArgumentNullException("Reference[" + p_o_yamlSchemaElement.Reference.Name + "] of schema-element[" + p_o_yamlSchemaElement.Name + "] has no children and no other reference");
                            }
                            else
                            {
                                b_childrenIteration = false;

                                /* check if current element in schema has data element by name, otherwise skip this element */
                                if (p_o_yamlSchemaElement.Name.Equals(p_o_yamlDataElement.Name))
                                {
                                    ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + p_o_yamlSchemaElement.Name + ": get schema-object[" + o_object?.GetType().FullName + "] with reference[" + p_o_yamlSchemaElement.Reference.Name + "]");

                                    /* only create new object if we have one object data */
                                    if (p_o_yamlDataElement.Children.Count != 1)
                                    {
                                        throw new ArgumentException("We have (" + p_o_yamlDataElement.Children.Count + ") no data children or more than one for schema-element[" + p_o_yamlSchemaElement.Name + "]");
                                    }

                                    /* handle reference with recursion */
                                    Object? o_returnObject = this.YamlDecodeRecursive(p_o_yamlDataElement.Children[0], p_o_yamlSchemaElement.Reference, o_object);

                                    /* check if we got a return object of recursion */
                                    if (o_returnObject == null)
                                    {
                                        /* it is valid to return a null object, anyway keep this exception if you want to uncomment it in the future */
                                        /* throw new ArgumentNullException("Schema-element[" + p_o_yamlSchemaElement.Name + "] returns no object after recursion with data[" + p_o_yamlDataElement.Name + "]"); */
                                    }
                                }
                            }
                        }
                        else
                        {
                            ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "update current schema-element[" + p_o_yamlSchemaElement.Name + "](" + p_o_yamlSchemaElement.PrintMapping() + ") with reference[" + p_o_yamlSchemaElement.Reference.Name + "](" + p_o_yamlSchemaElement.Reference.PrintMapping() + ")");

                            /* set reference as current yaml element */
                            p_o_yamlSchemaElement = p_o_yamlSchemaElement.Reference;
                        }
                    }

                    /* execute children iteration */
                    if (b_childrenIteration)
                    {
                        ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + p_o_yamlSchemaElement.Name + ": get schema-object with type[" + o_object?.GetType().FullName + "] and children[" + p_o_yamlSchemaElement.Children.Count + "]");

                        /* only create new object if we have one child definition for object in yaml schema */
                        if (p_o_yamlSchemaElement.Children.Count < 1)
                        {
                            throw new ArgumentException("Schema-element[" + p_o_yamlSchemaElement.Name + "] has no children");
                        }

                        /* check if new object is required if there is no data */
                        if ((p_o_yamlDataElement.Children.Count < 1) && (p_o_yamlSchemaElement.Required))
                        {
                            throw new ArgumentException("We have no data children for schema-element[" + p_o_yamlSchemaElement.Name + "] which is required");
                        }

                        /* only iterate if we have object data */
                        if (p_o_yamlDataElement.Children.Count > 0)
                        {
                            /* increase level for PrintTabs */
                            this.i_level++;

                            /* data pointer */
                            int j = 0;

                            for (int i = 0; i < p_o_yamlSchemaElement.Children.Count; i++)
                            {
                                ForestNET.Lib.Global.ILogFinest(this.PrintIndentation() + "compare schema-child-name(" + p_o_yamlSchemaElement.Children[i].Name + ") with data-child-name(" + p_o_yamlDataElement.Children[j].Name + ")");

                                /* check if current element in schema has data element by name, otherwise skip this element */
                                if (!p_o_yamlSchemaElement.Children[i].Name.Equals(p_o_yamlDataElement.Children[j].Name))
                                {
                                    /* but check if current schema element is required, so we must throw an exception */
                                    if (p_o_yamlSchemaElement.Children[i].Required)
                                    {
                                        throw new ArgumentException("We have no data for schema-element[" + p_o_yamlSchemaElement.Children[i].Name + "] which is required");
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }

                                /* handle new object with recursion */
                                Object? o_returnObject = this.YamlDecodeRecursive(p_o_yamlDataElement.Children[j], p_o_yamlSchemaElement.Children[i], o_object);

                                /* increase data pointer */
                                j++;

                                /* check if we got a return object of recursion */
                                if (o_returnObject == null)
                                {
                                    /* it is valid to return a null object, anyway keep this exception if you want to uncomment it in the future */
                                    /* throw new ArgumentNullException("Schema-element[" + p_o_yamlSchemaElement.Children[i].Name + "] returns no object after recursion with data[" + p_o_yamlDataElement.Children[i].Name + "]"); */
                                }
                            }

                            /* decrease level for PrintTabs */
                            this.i_level--;
                        }

                        if ((p_o_object != null) && (!((p_o_object.GetType().IsGenericType) && (p_o_object.GetType().GetGenericTypeDefinition() == typeof(List<>)))))
                        {
                            ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + p_o_yamlSchemaElement.Name + ": set schema-object[" + o_object?.GetType().FullName + "] to current object[" + p_o_object?.GetType().FullName + "] with mapping[" + s_objectMapping + "]");

                            this.SetFieldOrProperty(s_objectMapping, p_o_object, o_object);
                        }
                    }

                    /* decrease level for PrintTabs */
                    this.i_level--;

                    /* set object instance as current object */
                    p_o_object = o_object;
                }
            }
            else if (p_o_yamlSchemaElement.Type.ToLower().Equals("array"))
            {
                /* check conditions for handling array */
                if (p_o_yamlSchemaElement.Reference != null)
                {
                    if (p_o_yamlSchemaElement.Reference.Children.Count < 1)
                    {
                        throw new ArgumentException("Reference[" + p_o_yamlSchemaElement.Reference.Name + "] of schema-array[" + p_o_yamlSchemaElement.Name + "] with p_o_object[" + p_o_object?.GetType().FullName + "] must have at least one child");
                    }
                }
                else
                {
                    if (p_o_yamlSchemaElement.Children.Count != 1)
                    {
                        throw new ArgumentException("Schema-array[" + p_o_yamlSchemaElement.Name + "] with p_o_object[" + p_o_object?.GetType().FullName + "] must have just one child");
                    }

                    if (!p_o_yamlSchemaElement.Children[0].Name.ToLower().Equals("items"))
                    {
                        throw new ArgumentException("Schema-array[" + p_o_yamlSchemaElement.Name + "] with p_o_object[" + p_o_object?.GetType().FullName + "] must have one child with name[items]");
                    }
                }

                /* help variables to handle array */
                Object o_objectList;
                bool b_required = false;
                string s_requiredProperty = "";
                List<YAMLRestriction> a_restrictions = [];
                string s_amountProperty = "";
                bool b_primitiveArray = false;
                string s_primitiveArrayMapping = "";
                string s_genericListMapping = "";

                /* check if current array element is a primitive array */
                if ((p_o_yamlSchemaElement.Children.Count > 0) && (p_o_yamlSchemaElement.Children[0].PrimitiveArray))
                {
                    /* create list object for primitive array */
                    o_objectList = (Object)(new List<Object?>());
                    /* set flag for primitive array */
                    b_primitiveArray = true;

                    /* check if we have a mapping value for primitive array */
                    if ((ForestNET.Lib.Helper.IsStringEmpty(p_o_yamlSchemaElement.Mapping)) && (ForestNET.Lib.Helper.IsStringEmpty(p_o_yamlSchemaElement.MappingClass)))
                    {
                        throw new ArgumentException("Schema-primitive-array[" + p_o_yamlSchemaElement.Name + "] with p_o_object[" + p_o_object?.GetType().FullName + "] has no mapping value");
                    }
                    else
                    {
                        /* store mapping value for later use */
                        if (!(ForestNET.Lib.Helper.IsStringEmpty(p_o_yamlSchemaElement.Mapping)))
                        {
                            s_primitiveArrayMapping = p_o_yamlSchemaElement.Mapping;
                        }
                        else if (!(ForestNET.Lib.Helper.IsStringEmpty(p_o_yamlSchemaElement.MappingClass)))
                        {
                            s_primitiveArrayMapping = p_o_yamlSchemaElement.MappingClass;
                        }
                    }
                }
                else
                {
                    /* create or retrieve object list data */
                    if (p_o_object == null)
                    { /* create a new object instance of yaml array element */
                        ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "p_o_object == null, create new array list");

                        /* create list object */
                        o_objectList = (Object)(new List<Object?>());
                    }
                    else
                    { /* we have to retrieve the list object */
                        ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "p_o_object(" + p_o_object.GetType().FullName + ") != null, get list property " + p_o_yamlSchemaElement.Name);

                        /* get list property */
                        o_objectList = GetListFieldOrProperty(p_o_yamlSchemaElement, p_o_object);

                        /* we must cast returned generic list to List<object> */
                        o_objectList = ((System.Collections.IEnumerable)o_objectList).Cast<Object?>().ToList();
                    }

                    /* check if we have a mapping value for generic list */
                    if ((ForestNET.Lib.Helper.IsStringEmpty(p_o_yamlSchemaElement.Mapping)) && (ForestNET.Lib.Helper.IsStringEmpty(p_o_yamlSchemaElement.MappingClass)))
                    {
                        throw new ArgumentException("Schema-primitive-array[" + p_o_yamlSchemaElement.Name + "] with p_o_object[" + p_o_object?.GetType().FullName + "] has no mapping value");
                    }
                    else
                    {
                        /* store mapping value for later use */
                        if (!(ForestNET.Lib.Helper.IsStringEmpty(p_o_yamlSchemaElement.Mapping)))
                        {
                            s_genericListMapping = p_o_yamlSchemaElement.Mapping;
                        }
                        else if (!(ForestNET.Lib.Helper.IsStringEmpty(p_o_yamlSchemaElement.MappingClass)))
                        {
                            s_genericListMapping = p_o_yamlSchemaElement.MappingClass;
                        }
                    }
                }

                /* check if yaml-element is required */
                if (p_o_yamlSchemaElement.Required)
                {
                    b_required = true;
                    s_requiredProperty = p_o_yamlSchemaElement.Name;
                }

                /* check minItems and maxItems restrictions and save them for items check afterwards */
                if (p_o_yamlSchemaElement.Restrictions.Count > 0)
                {
                    foreach (YAMLRestriction o_yamlRestriction in p_o_yamlSchemaElement.Restrictions)
                    {
                        if ((o_yamlRestriction.Name.ToLower().Equals("minitems")) || (o_yamlRestriction.Name.ToLower().Equals("maxitems")))
                        {
                            a_restrictions.Add(o_yamlRestriction);
                            s_amountProperty = p_o_yamlSchemaElement.Name;
                        }
                    }
                }

                if (p_o_yamlSchemaElement.Reference != null)
                {
                    /* set reference as current yaml element */
                    p_o_yamlSchemaElement = p_o_yamlSchemaElement.Reference;
                }
                else
                {
                    /* set current yaml element to 'items' child */
                    p_o_yamlSchemaElement = p_o_yamlSchemaElement.Children[0];

                    /* if 'items' child has a child as well, we take this child as current yaml element */
                    if (p_o_yamlSchemaElement.Children.Count == 1)
                    {
                        p_o_yamlSchemaElement = p_o_yamlSchemaElement.Children[0];
                    }

                    /* important part for parsing collection which are not inline collection values in yaml document */
                    if ((!p_o_yamlDataElement.Name.Equals("__ArrayObject__")) && (p_o_yamlSchemaElement.Name.ToLower().Equals("items")))
                    {
                        if ((p_o_yamlDataElement.Children.Count > 0) && (p_o_yamlDataElement.Children[0].Name.Equals("__ArrayObject__")))
                        {
                            p_o_yamlDataElement = p_o_yamlDataElement.Children[0];
                        }
                    }
                }

                if (ForestNET.Lib.Helper.IsStringEmpty(p_o_yamlDataElement.Value))
                { /* we have multiple minor objects for current array */
                    ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + p_o_yamlSchemaElement.Name + ": get schema-array with mapping[" + p_o_yamlSchemaElement.PrintMapping() + "]");

                    if (p_o_yamlDataElement.Children.Count > 0)
                    { /* if we have objects to the new array */
                        /* check minItems and maxItems restrictions */
                        if (a_restrictions.Count > 0)
                        {
                            foreach (YAMLRestriction o_yamlRestriction in a_restrictions)
                            {
                                if (o_yamlRestriction.Name.ToLower().Equals("minitems"))
                                {
                                    /* check minItems restriction */
                                    if (p_o_yamlDataElement.Children.Count < o_yamlRestriction.IntValue)
                                    {
                                        throw new ArgumentException("Restriction error: not enough [" + p_o_yamlSchemaElement.Name + " of " + s_amountProperty + "] yaml items(" + p_o_yamlDataElement.Children.Count + "), minimum = " + o_yamlRestriction.IntValue);
                                    }
                                }

                                if (o_yamlRestriction.Name.ToLower().Equals("maxitems"))
                                {
                                    /* check maxItems restriction */
                                    if (p_o_yamlDataElement.Children.Count > o_yamlRestriction.IntValue)
                                    {
                                        throw new ArgumentException("Restriction error: too many [" + p_o_yamlSchemaElement.Name + " of " + s_amountProperty + "] yaml items(" + p_o_yamlDataElement.Children.Count + "), maximum = " + o_yamlRestriction.IntValue);
                                    }
                                }
                            }
                        }

                        /* iterate objects in list and encode data to yaml recursively */
                        for (int i = 0; i < p_o_yamlDataElement.Children.Count; i++)
                        {
                            /* increase level for PrintTabs */
                            this.i_level++;

                            /* handle array object with recursion */
                            Object? o_returnObject = this.YamlDecodeRecursive(p_o_yamlDataElement.Children[i], p_o_yamlSchemaElement, o_objectList);

                            ForestNET.Lib.Global.ILogFinest(this.PrintIndentation() + "add return value to list of " + p_o_yamlDataElement.Name + ", type[" + ((o_returnObject == null) ? "object is null" : o_returnObject.GetType().FullName) + "]");

                            /* add return object of recursion to list */
                            ((List<Object?>)o_objectList).Add(o_returnObject);

                            /* decrease level for PrintTabs */
                            this.i_level--;
                        }
                    }
                }
                else
                { /* array objects must be retrieved out of value property */
                    ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + p_o_yamlSchemaElement.Name + ": handle array value = " + p_o_yamlDataElement.Value + " - mapping[" + p_o_yamlSchemaElement.PrintMapping() + "]");

                    /* set array with values if we have any values */
                    if ((p_o_yamlDataElement.Value != null) && (!ForestNET.Lib.Helper.IsStringEmpty(p_o_yamlDataElement.Value)) && (!p_o_yamlDataElement.Value.Equals("[]")))
                    {
                        /* remove opening and closing brackets */
                        if ((p_o_yamlDataElement.Value.StartsWith("[")) && (p_o_yamlDataElement.Value.EndsWith("]")))
                        {
                            p_o_yamlDataElement.Value = p_o_yamlDataElement.Value.Substring(1, p_o_yamlDataElement.Value.Length - 1 - 1);
                        }

                        /* split array into values, divided by ',' */
                        string[] a_values = p_o_yamlDataElement.Value.Split(",");

                        /* check minItems and maxItems restrictions */
                        if (a_restrictions.Count > 0)
                        {
                            foreach (YAMLRestriction o_yamlRestriction in a_restrictions)
                            {
                                if (o_yamlRestriction.Name.ToLower().Equals("minitems"))
                                {
                                    /* check minItems restriction */
                                    if (a_values.Length < o_yamlRestriction.IntValue)
                                    {
                                        throw new ArgumentException("Restriction error: not enough [" + p_o_yamlSchemaElement.Name + " of " + s_amountProperty + "] yaml items(" + a_values.Length + "), minimum = " + o_yamlRestriction.IntValue);
                                    }
                                }

                                if (o_yamlRestriction.Name.ToLower().Equals("maxitems"))
                                {
                                    /* check maxItems restriction */
                                    if (a_values.Length > o_yamlRestriction.IntValue)
                                    {
                                        throw new ArgumentException("Restriction error: too many [" + p_o_yamlSchemaElement.Name + " of " + s_amountProperty + "] yaml items(" + a_values.Length + "), maximum = " + o_yamlRestriction.IntValue);
                                    }
                                }
                            }
                        }

                        /* iterate all array values */
                        foreach (string s_foo in a_values)
                        {
                            string s_value = s_foo;

                            /* trim any whitespace characters from string value */
                            s_value = s_value.Trim();

                            /* get yaml value type of string value */
                            YAMLValueType e_yamlValueType = YAML.GetYAMLValueType(s_value);

                            /* check if YAML value types are matching between schema and data, if it is not 'null' */
                            if ((e_yamlValueType != YAML.StringToYAMLValueType(p_o_yamlSchemaElement.Type)) && (e_yamlValueType != YAMLValueType.Null) && ((s_value != null) && (!s_value.Equals("null"))))
                            {
                                throw new ArgumentException("YAML schema type[" + YAML.StringToYAMLValueType(p_o_yamlSchemaElement.Type) + "] does not match with data value type[" + e_yamlValueType + "] with value[" + s_value + "]");
                            }

                            /* check if yaml-element has any restrictions */
                            if (p_o_yamlSchemaElement.Restrictions.Count > 0)
                            {
                                foreach (YAMLRestriction o_yamlRestriction in p_o_yamlSchemaElement.Restrictions)
                                {
                                    /* execute restriction check */
                                    this.CheckRestriction(s_value, o_yamlRestriction, p_o_yamlSchemaElement);
                                }
                            }

                            /* cast array string value into object */
                            Object? o_returnObject = this.CastObjectFromString(s_value, p_o_yamlSchemaElement.Type);

                            ForestNET.Lib.Global.ILogFinest(this.PrintIndentation() + "add value[" + (o_returnObject ?? "null") + "] to list of " + p_o_yamlDataElement.Name + ", type[" + ((o_returnObject == null) ? "null" : o_returnObject.GetType().FullName) + "]");

                            /* add return object of recursion to list */
                            ((List<Object?>)o_objectList).Add(o_returnObject);
                        }
                    }
                    else if (b_required)
                    { /* if yaml-element with type array is required, throw exception */
                        throw new ArgumentException("'" + p_o_yamlSchemaElement.Name + "' of '" + s_requiredProperty + "' is required, but array has no values");
                    }
                }

                /* if we have a primitive array, we take our object list and convert it to a primitive array */
                if (b_primitiveArray)
                {
                    /* we must have an object to set primitive array property */
                    if (p_o_object == null)
                    {
                        throw new ArgumentNullException("YAML schema element[" + p_o_yamlSchemaElement.Name + "] has not initiated object for primitive array[" + s_primitiveArrayMapping + "]");
                    }

                    ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "set primitive array for [" + s_primitiveArrayMapping + "] of object [" + p_o_object.GetType().FullName + "]");

                    /* set primitive array property of object */
                    this.SetPrimitiveArrayFieldOrProperty(s_primitiveArrayMapping, p_o_object, o_objectList);
                }

                /* set array instance as current object, if it is still null */
                if (p_o_object == null)
                {
                    ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + "p_o_object == null, set p_o_object = o_objectList");

                    p_o_object = o_objectList;
                }
                else if (!b_primitiveArray) /* set generic list, not primitive array again */
                {
                    /* only set field/property if gathered generic list has any elements */
                    if (((List<Object?>)o_objectList).Count > 0)
                    {
                        /* set gathered generic list to current parameter object */
                        this.SetGenericListFieldOrProperty(s_genericListMapping, p_o_object, o_objectList);
                    }
                }
            }
            else
            {
                ForestNET.Lib.Global.ILogFiner(this.PrintIndentation() + p_o_yamlSchemaElement.Name + ": handle value = " + p_o_yamlDataElement.Value + " - mapping[" + p_o_yamlSchemaElement.PrintMapping() + "]");

                YAMLValueType e_yamlValueType = YAML.GetYAMLValueType(p_o_yamlDataElement.Value);

                /* check if YAML value types are matching between schema and data, additional condition = YAML value type is not 'null' */
                if ((e_yamlValueType != YAML.StringToYAMLValueType(p_o_yamlSchemaElement.Type)) && (e_yamlValueType != YAMLValueType.Null))
                {
                    /* it is acceptable if source type is 'integer' and destination type is 'number', valid cast available */
                    if (!((e_yamlValueType == YAMLValueType.Integer) && (YAML.StringToYAMLValueType(p_o_yamlSchemaElement.Type) == YAMLValueType.Number)))
                    {
                        throw new ArgumentException("YAML schema type[" + e_yamlValueType + "] does not match with data value type[" + YAML.StringToYAMLValueType(p_o_yamlSchemaElement.Type) + "] with value[" + p_o_yamlDataElement.Value + "]");
                    }
                }

                /* check if yaml-element is required */
                if (p_o_yamlSchemaElement.Required)
                {
                    /* check if value is empty */
                    if ((p_o_yamlDataElement.Value == null) || (p_o_yamlDataElement.Value.Equals("")) || (p_o_yamlDataElement.Value.Equals("null")) || (p_o_yamlDataElement.Value.Equals("\"\"")))
                    {
                        throw new ArgumentNullException("'" + p_o_yamlSchemaElement.Name + "' is required, but value[" + p_o_yamlDataElement.Value + "] is empty");
                    }
                }

                /* check if yaml-element has any restrictions */
                if (p_o_yamlSchemaElement.Restrictions.Count > 0)
                {
                    foreach (YAMLRestriction o_yamlRestriction in p_o_yamlSchemaElement.Restrictions)
                    {
                        /* execute restriction check */
                        this.CheckRestriction(p_o_yamlDataElement.Value, o_yamlRestriction, p_o_yamlSchemaElement);
                    }
                }

                if ((ForestNET.Lib.Helper.IsStringEmpty(p_o_yamlSchemaElement.Mapping)) && (ForestNET.Lib.Helper.IsStringEmpty(p_o_yamlSchemaElement.MappingClass)))
                {
                    p_o_object = this.CastObjectFromString(p_o_yamlDataElement.Value, p_o_yamlSchemaElement.Type);
                }
                else
                {
                    this.SetFieldOrProperty(p_o_yamlSchemaElement, p_o_object, p_o_yamlDataElement.Value);
                }
            }

            ForestNET.Lib.Global.ILogFine(this.PrintIndentation() + "return " + ((p_o_object == null) ? "null" : p_o_object.GetType().FullName));

            return p_o_object;
        }

        /// <summary>
        /// Convert a string value of yaml type to yaml type enumeration value
        /// </summary>
        /// <param name="p_s_yamlValueType">yaml type string from yaml element</param>
        /// <returns>yaml type enumeration value</returns>
        /// <exception cref="ArgumentException">invalid string type parameter</exception>
        private static YAMLValueType StringToYAMLValueType(string? p_s_yamlValueType)
        {
            p_s_yamlValueType = p_s_yamlValueType?.ToLower();

            if (p_s_yamlValueType == null)
            {
                throw new ArgumentException("Invalid YAML value type with value [null]");
            }
            else if (p_s_yamlValueType.Equals("string"))
            {
                return YAMLValueType.String;
            }
            else if (p_s_yamlValueType.Equals("number"))
            {
                return YAMLValueType.Number;
            }
            else if (p_s_yamlValueType.Equals("integer"))
            {
                return YAMLValueType.Integer;
            }
            else if (p_s_yamlValueType.Equals("boolean"))
            {
                return YAMLValueType.Boolean;
            }
            else if (p_s_yamlValueType.Equals("array"))
            {
                return YAMLValueType.Array;
            }
            else if (p_s_yamlValueType.Equals("object"))
            {
                return YAMLValueType.Object;
            }
            else if (p_s_yamlValueType.Equals("null"))
            {
                return YAMLValueType.Null;
            }
            else
            {
                throw new ArgumentException("Invalid YAML value type with value [" + p_s_yamlValueType + "]");
            }
        }

        /// <summary>
        /// Get a List object from object's property, access directly via public field or public methods
        /// </summary>
        /// <param name="p_o_yamlSchemaElement">yaml schema element with mapping information</param>
        /// <param name="p_o_object">object where to retrieve List object</param>
        /// <returns>List object</returns>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        /// <exception cref="ArgumentNullException">list object is not initialized</exception>
        private Object GetListFieldOrProperty(YAMLElement p_o_yamlSchemaElement, Object? p_o_object)
        {
            /* retrieve field information out of schema element */
            string s_field = p_o_yamlSchemaElement.MappingClass;

            /* if there is additional mapping information, use this for field property access */
            if (!ForestNET.Lib.Helper.IsStringEmpty(p_o_yamlSchemaElement.Mapping))
            {
                s_field = p_o_yamlSchemaElement.Mapping;
            }

            /* check if we use property methods with invoke to set object data values */
            if (this.UseProperties)
            {
                /* property info for accessing generic list */
                System.Reflection.PropertyInfo? o_propertyInfo;

                /* try to get access to property info */
                try
                {
                    o_propertyInfo = p_o_object?.GetType().GetProperty(s_field);

                    if (o_propertyInfo == null)
                    {
                        throw new Exception("property info is null");
                    }
                }
                catch (Exception o_exc)
                {
                    throw new MissingMemberException("Class instance property[" + s_field + "] does not exist for class instance(" + p_o_object?.GetType().FullName + "): " + o_exc.Message);
                }

                /* check if we can access generic list property */
                if (!o_propertyInfo.CanRead)
                {
                    throw new MemberAccessException("Cannot read property from class; instance property[" + s_field + "] does not exist for class instance(" + p_o_object?.GetType().FullName + ")");
                }

                /* get generic list for our reference parameter */
                p_o_object = o_propertyInfo.GetValue(p_o_object);
            }
            else
            {
                /* field info for accessing generic list */
                System.Reflection.FieldInfo? o_fieldInfo;

                /* try to get access to field info */
                try
                {
                    o_fieldInfo = p_o_object?.GetType().GetField(s_field);

                    if (o_fieldInfo == null)
                    {
                        throw new Exception("field info is null");
                    }
                }
                catch (Exception o_exc)
                {
                    throw new MissingMemberException("Class instance field[" + s_field + "] does not exist for class instance(" + p_o_object?.GetType().FullName + "): " + o_exc.Message);
                }

                /* check if we can access generic list field */
                if (!o_fieldInfo.IsPublic)
                {
                    throw new MemberAccessException("Cannot read field from class; instance field[" + s_field + "] does not exist for class instance(" + p_o_object?.GetType().FullName + ")");
                }

                /* get generic list for our reference parameter */
                p_o_object = o_fieldInfo.GetValue(p_o_object);
            }

            /* check if list object is not null */
            if (p_o_object == null)
            {
                throw new ArgumentNullException("List object from property/field[" + s_field + "] not initialised for object");
            }

            /* check if object is of type List */
            if (!((p_o_object.GetType().IsGenericType) && (p_o_object.GetType().GetGenericTypeDefinition() == typeof(List<>))))
            {
                throw new TypeLoadException("Object from method[" + s_field + "] is not a list object for object: " + p_o_object.GetType().FullName);
            }

            return p_o_object;
        }

        /// <summary>
        /// Method to set property field of an object with value of yaml element from file line and mapping value of yaml schema element
        /// </summary>
        /// <param name="p_o_yamlSchemaElement">mapping class and type hint to cast value to object's field from yaml schema</param>
        /// <param name="p_o_object">object parameter where yaml data will be decoded and cast into object fields</param>
        /// <param name="p_s_objectValue">string value of yaml element from file line</param>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        /// <exception cref="FormatException">exception parsing string to target type</exception>
        private void SetFieldOrProperty(YAMLElement p_o_yamlSchemaElement, Object? p_o_object, string? p_s_objectValue)
        {
            /* remove surrounded double quotes from value */
            if ((p_s_objectValue != null) && (p_s_objectValue.StartsWith(this.s_stringQuote)) && (p_s_objectValue.EndsWith(this.s_stringQuote)))
            {
                p_s_objectValue = p_s_objectValue.Substring(1, p_s_objectValue.Length - 1 - 1);
            }

            /* retrieve field information out of schema element */
            string s_field = p_o_yamlSchemaElement.MappingClass;

            /* if there is additional mapping information, use this for field property access */
            if (!ForestNET.Lib.Helper.IsStringEmpty(p_o_yamlSchemaElement.Mapping))
            {
                s_field = p_o_yamlSchemaElement.Mapping;
            }

            /* check if we use property methods with invoke to set object data values */
            if (this.UseProperties)
            {
                /* property info for accessing generic list */
                System.Reflection.PropertyInfo? o_propertyInfo;

                /* try to get access to property info */
                try
                {
                    o_propertyInfo = p_o_object?.GetType().GetProperty(s_field);

                    if (o_propertyInfo == null)
                    {
                        throw new Exception("property info is null");
                    }
                }
                catch (Exception o_exc)
                {
                    throw new MissingMemberException("Class instance property[" + s_field + "] does not exist for class instance(" + p_o_object?.GetType().FullName + "): " + o_exc.Message);
                }

                /* check if we can access generic list property */
                if (!o_propertyInfo.CanWrite)
                {
                    throw new MemberAccessException("Cannot write property from class; instance property[" + s_field + "] does not exist for class instance(" + p_o_object?.GetType().FullName + ")");
                }

                try
                {
                    /* set value to property of current element and cast string to object value with yaml schema type */
                    o_propertyInfo.SetValue(p_o_object, this.CastObjectFromString(p_s_objectValue, p_o_yamlSchemaElement.Type));
                }
                catch (ArgumentException o_exc)
                {
                    ForestNET.Lib.Global.ILogFinest(this.PrintIndentation() + o_exc.Message + " - with type[" + p_o_yamlSchemaElement.Type + "] - try another cast with type[" + o_propertyInfo.PropertyType.FullName + "]");

                    /* invoke set-property-method to set object data field of current element and cast string to object value */
                    o_propertyInfo.SetValue(p_o_object, this.CastObjectFromString(p_s_objectValue, o_propertyInfo.PropertyType.FullName ?? "null"));
                }
                catch (Exception o_exc)
                {
                    throw new MemberAccessException("Property[" + s_field + "] is not accessible for object: " + p_o_object?.GetType().FullName + " - " + o_exc.Message);
                }
            }
            else
            {
                /* field info for accessing generic list */
                System.Reflection.FieldInfo? o_fieldInfo;

                /* try to get access to field info */
                try
                {
                    o_fieldInfo = p_o_object?.GetType().GetField(s_field);

                    if (o_fieldInfo == null)
                    {
                        throw new Exception("field info is null");
                    }
                }
                catch (Exception o_exc)
                {
                    throw new MissingMemberException("Class instance field[" + s_field + "] does not exist for class instance(" + p_o_object?.GetType().FullName + "): " + o_exc.Message);
                }

                /* check if we can access generic list field */
                if (!o_fieldInfo.IsPublic)
                {
                    throw new MemberAccessException("Cannot read field from class; instance field[" + s_field + "] does not exist for class instance(" + p_o_object?.GetType().FullName + ")");
                }

                /* call field directly to set field data value */
                try
                {
                    o_fieldInfo.SetValue(p_o_object, this.CastObjectFromString(p_s_objectValue, p_o_yamlSchemaElement.Type));
                }
                catch (ArgumentException o_exc)
                {
                    ForestNET.Lib.Global.ILogFinest(this.PrintIndentation() + o_exc.Message + " - with type[" + p_o_yamlSchemaElement.Type + "]  - try another cast with type[" + o_fieldInfo.FieldType.FullName + "]");

                    o_fieldInfo.SetValue(p_o_object, this.CastObjectFromString(p_s_objectValue, o_fieldInfo.FieldType.FullName ?? "null"));
                }
                catch (Exception o_exc)
                {
                    throw new MemberAccessException("Field[" + s_field + "] is not accessible for object: " + p_o_object?.GetType().FullName + " - " + o_exc.Message);
                }
            }
        }

        /// <summary>
        /// Method to set property field of an object with simple object value, so no cast will be done
        /// </summary>
        /// <param name="p_s_mapping">mapping class and type hint to cast value to object's field</param>
        /// <param name="p_o_object">object parameter where yaml data will be decoded and cast into object fields</param>
        /// <param name="p_o_objectValue">object value of yaml element from file line</param>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        private void SetFieldOrProperty(string p_s_mapping, Object? p_o_object, Object? p_o_objectValue)
        {
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

                try
                {
                    /* set value to property of current element */
                    o_propertyInfo.SetValue(p_o_object, p_o_objectValue);
                }
                catch (Exception o_exc)
                {
                    throw new MemberAccessException("Property[" + p_s_mapping + "] is not accessible for object: " + p_o_object?.GetType().FullName + " - " + o_exc.Message);
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

                /* call field directly to set field data value */
                try
                {
                    o_fieldInfo.SetValue(p_o_object, p_o_objectValue);
                }
                catch (Exception o_exc)
                {
                    throw new MemberAccessException("Field[" + p_s_mapping + "] is not accessible for object: " + p_o_object?.GetType().FullName + " - " + o_exc.Message);
                }
            }
        }

        /// <summary>
        /// Method to set primitive array property/field of an object with simple object value, so no cast will be done
        /// </summary>
        /// <param name="p_s_mapping">mapping class and type hint to cast value to object's primitive array field</param>
        /// <param name="p_o_object">object parameter where yaml data will be decoded and cast into object fields</param>
        /// <param name="p_o_objectValue">object value of yaml element from file line</param>
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

        /// <summary>
        /// Method to set generic list property/field of an object with object value, so any cast will be made
        /// </summary>
        /// <param name="p_s_mapping">mapping class and type hint to cast value to object's generic list</param>
        /// <param name="p_o_object">object parameter where yaml data will be decoded and cast into object fields</param>
        /// <param name="p_o_objectValue">object value of yaml element from file line</param>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        /// <exception cref="ArgumentException">invalid type parameter</exception>
        /// <exception cref="FormatException">exception parsing string to target type</exception>
        private void SetGenericListFieldOrProperty(string p_s_mapping, Object? p_o_object, Object? p_o_objectValue)
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

                /* check if property type is a generic list */
                if (!((o_propertyInfo.PropertyType != null) && (o_propertyInfo.PropertyType.IsGenericType) && (o_propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(List<>))))
                {
                    throw new ArrayTypeMismatchException("Cannot use field from class; instance field[" + p_s_mapping + "] ist not a generic list: '" + p_o_object?.GetType().FullName + "'");
                }

                /* get primitive array type */
                string s_genericListType = o_propertyInfo.PropertyType.GenericTypeArguments[0].FullName ?? "null";

                if (o_foo.Count < 1)
                {
                    /* set primitive array to null if we have no elements */
                    o_propertyInfo.SetValue(p_o_object, null);
                }
                else
                {
                    /* cast object list to primitive array, set object array with property info of current element */
                    if (s_genericListType.Equals("System.Boolean"))
                    {
                        bool[] o_bar = new bool[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (bool?)o_foo[i] ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar.ToList());
                    }
                    else if (s_genericListType.Equals("System.Byte"))
                    {
                        byte[] o_bar = new byte[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (byte?)o_foo[i] ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar.ToList());
                    }
                    else if (s_genericListType.Equals("System.SByte"))
                    {
                        sbyte[] o_bar = new sbyte[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (sbyte?)o_foo[i] ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar.ToList());
                    }
                    else if (s_genericListType.Equals("System.Char"))
                    {
                        char[] o_bar = new char[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (char?)o_foo[i] ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar.ToList());
                    }
                    else if (s_genericListType.Equals("System.Single"))
                    {
                        float[] o_bar = new float[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (float?)o_foo[i] ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar.ToList());
                    }
                    else if (s_genericListType.Equals("System.Double"))
                    {
                        double[] o_bar = new double[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (double?)o_foo[i] ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar.ToList());
                    }
                    else if (s_genericListType.Equals("System.Int16"))
                    {
                        short[] o_bar = new short[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (short?)o_foo[i] ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar.ToList());
                    }
                    else if (s_genericListType.Equals("System.Int32"))
                    {
                        int[] o_bar = new int[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (int?)o_foo[i] ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar.ToList());
                    }
                    else if (s_genericListType.Equals("System.Int64"))
                    {
                        long[] o_bar = new long[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (long?)o_foo[i] ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar.ToList());
                    }
                    else if (s_genericListType.Equals("System.UInt16"))
                    {
                        ushort[] o_bar = new ushort[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (ushort?)o_foo[i] ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar.ToList());
                    }
                    else if (s_genericListType.Equals("System.UInt32"))
                    {
                        uint[] o_bar = new uint[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (uint?)o_foo[i] ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar.ToList());
                    }
                    else if (s_genericListType.Equals("System.UInt64"))
                    {
                        ulong[] o_bar = new ulong[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (ulong?)o_foo[i] ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar.ToList());
                    }
                    else if (s_genericListType.Equals("System.String"))
                    {
                        string[] o_bar = new string[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (string?)o_foo[i] ?? string.Empty;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar.ToList());
                    }
                    else if (s_genericListType.Equals("System.DateTime"))
                    {
                        DateTime[] o_bar = new DateTime[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (DateTime?)o_foo[i] ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar.ToList());
                    }
                    else if (s_genericListType.Equals("System.TimeSpan"))
                    {
                        TimeSpan[] o_bar = new TimeSpan[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (TimeSpan?)o_foo[i] ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar.ToList());
                    }
                    else if (s_genericListType.Equals("System.Decimal"))
                    {
                        decimal[] o_bar = new decimal[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (decimal?)o_foo[i] ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar.ToList());
                    }
                    else if (s_genericListType.Equals("System.Object"))
                    {
                        object[] o_bar = new object[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = o_foo[i] ?? new object();
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar.ToList());
                    }
                    else
                    {
                        try
                        {
                            /* get element type name of first element of object list */
                            string s_elementTypeName = o_foo[0]?.GetType().AssemblyQualifiedName ?? throw new NullReferenceException("Could not get element type name with '" + o_foo[0]?.GetType() + "'");
                            /* get element type of first element of object list */
                            Type o_elementType = Type.GetType(s_elementTypeName) ?? throw new NullReferenceException("Could not retrieve object type with '" + s_elementTypeName + "'");
                            /* create generic list type with element type */
                            Type o_genericListType = typeof(List<>).MakeGenericType(o_elementType);
                            /* create instance of generic list with generic list type and handle it as IList */
                            System.Collections.IList o_genericList = Activator.CreateInstance(o_genericListType) as System.Collections.IList ?? throw new NullReferenceException("Could not create a generic list with type '" + o_genericListType.FullName + "'");

                            /* iterate all elements of object list */
                            for (int i = 0; i < o_foo.Count; i++)
                            {
                                /* add element to generic list */
                                o_genericList.Add(o_foo[i]);
                            }

                            /* set generic list to our field */
                            o_propertyInfo.SetValue(p_o_object, o_genericList);
                        }
                        catch (Exception o_exc)
                        {
                            throw new ArgumentException("Invalid type [" + s_genericListType + "] for property[" + p_s_mapping + "] and object [" + p_o_object?.GetType().FullName + "] " + o_exc);
                        }
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

                /* check if field type is a generic list */
                if (!((o_fieldInfo.FieldType != null) && (o_fieldInfo.FieldType.IsGenericType) && (o_fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>))))
                {
                    throw new ArrayTypeMismatchException("Cannot use field from class; instance field[" + p_s_mapping + "] ist not a generic list: '" + p_o_object?.GetType().FullName + "'");
                }

                /* get primitive array type */
                string s_genericListType = o_fieldInfo.FieldType.GenericTypeArguments[0].FullName ?? "null";

                if (o_foo.Count < 1)
                {
                    /* set primitive array to null if we have no elements */
                    o_fieldInfo.SetValue(p_o_object, null);
                }
                else
                {
                    /* cast object list to primitive array, set object array with property info of current element */
                    if (s_genericListType.Equals("System.Boolean"))
                    {
                        bool[] o_bar = new bool[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (bool?)o_foo[i] ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar.ToList());
                    }
                    else if (s_genericListType.Equals("System.Byte"))
                    {
                        byte[] o_bar = new byte[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (byte?)o_foo[i] ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar.ToList());
                    }
                    else if (s_genericListType.Equals("System.SByte"))
                    {
                        sbyte[] o_bar = new sbyte[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (sbyte?)o_foo[i] ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar.ToList());
                    }
                    else if (s_genericListType.Equals("System.Char"))
                    {
                        char[] o_bar = new char[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (char?)o_foo[i] ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar.ToList());
                    }
                    else if (s_genericListType.Equals("System.Single"))
                    {
                        float[] o_bar = new float[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (float?)o_foo[i] ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar.ToList());
                    }
                    else if (s_genericListType.Equals("System.Double"))
                    {
                        double[] o_bar = new double[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (double?)o_foo[i] ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar.ToList());
                    }
                    else if (s_genericListType.Equals("System.Int16"))
                    {
                        short[] o_bar = new short[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (short?)o_foo[i] ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar.ToList());
                    }
                    else if (s_genericListType.Equals("System.Int32"))
                    {
                        int[] o_bar = new int[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (int?)o_foo[i] ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar.ToList());
                    }
                    else if (s_genericListType.Equals("System.Int64"))
                    {
                        long[] o_bar = new long[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (long?)o_foo[i] ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar.ToList());
                    }
                    else if (s_genericListType.Equals("System.UInt16"))
                    {
                        ushort[] o_bar = new ushort[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (ushort?)o_foo[i] ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar.ToList());
                    }
                    else if (s_genericListType.Equals("System.UInt32"))
                    {
                        uint[] o_bar = new uint[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (uint?)o_foo[i] ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar.ToList());
                    }
                    else if (s_genericListType.Equals("System.UInt64"))
                    {
                        ulong[] o_bar = new ulong[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (ulong?)o_foo[i] ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar.ToList());
                    }
                    else if (s_genericListType.Equals("System.String"))
                    {
                        string[] o_bar = new string[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = o_foo[i] as string ?? string.Empty;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar.ToList());
                    }
                    else if (s_genericListType.Equals("System.DateTime"))
                    {
                        DateTime[] o_bar = new DateTime[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (DateTime?)o_foo[i] ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar.ToList());
                    }
                    else if (s_genericListType.Equals("System.TimeSpan"))
                    {
                        TimeSpan[] o_bar = new TimeSpan[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (TimeSpan?)o_foo[i] ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar.ToList());
                    }
                    else if (s_genericListType.Equals("System.Decimal"))
                    {
                        decimal[] o_bar = new decimal[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (decimal?)o_foo[i] ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar.ToList());
                    }
                    else if (s_genericListType.Equals("System.Object"))
                    {
                        object[] o_bar = new object[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = o_foo[i] ?? new object();
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar.ToList());
                    }
                    else
                    {
                        try
                        {
                            /* get element type name of first element of object list */
                            string s_elementTypeName = o_foo[0]?.GetType().AssemblyQualifiedName ?? throw new NullReferenceException("Could not get element type name with '" + o_foo[0]?.GetType() + "'");
                            /* get element type of first element of object list */
                            Type o_elementType = Type.GetType(s_elementTypeName) ?? throw new NullReferenceException("Could not retrieve object type with '" + s_elementTypeName + "'");
                            /* create generic list type with element type */
                            Type o_genericListType = typeof(List<>).MakeGenericType(o_elementType);
                            /* create instance of generic list with generic list type and handle it as IList */
                            System.Collections.IList o_genericList = Activator.CreateInstance(o_genericListType) as System.Collections.IList ?? throw new NullReferenceException("Could not create a generic list with type '" + o_genericListType.FullName + "'");

                            /* iterate all elements of object list */
                            for (int i = 0; i < o_foo.Count; i++)
                            {
                                /* add element to generic list */
                                o_genericList.Add(o_foo[i]);
                            }

                            /* set generic list to our field */
                            o_fieldInfo.SetValue(p_o_object, o_genericList);
                        }
                        catch (Exception o_exc)
                        {
                            throw new ArgumentException("Invalid type [" + s_genericListType + "] for field[" + p_s_mapping + "] and object [" + p_o_object?.GetType().FullName + "] " + o_exc);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Convert a string value from a yaml element to an object to decode it into an object
        /// </summary>
        /// <param name="p_s_value">string value of yaml element from file</param>
        /// <param name="p_s_type">type of destination object field</param>
        /// <returns>casted object value from string</returns>
        /// <exception cref="ArgumentException">invalid type parameter</exception>
        /// <exception cref="FormatException">exception parsing string to target type</exception>
        private Object? CastObjectFromString(string? p_s_value, string? p_s_type)
        {
            Object? o_foo = null;

            /* return null if value or type parameter is null */
            if ((p_s_value == null) || (p_s_type == null))
            {
                return o_foo;
            }

            /* check if value is not empty */
            if (!p_s_value.Equals(""))
            {
                p_s_type = p_s_type.ToLower();

                /* transpose primitive type to class type */
                switch (p_s_type.ToLower())
                {
                    case "byte":
                        p_s_type = "system.byte";
                        break;
                    case "short":
                        p_s_type = "system.int16";
                        break;
                    case "integer":
                        p_s_type = "system.int32";
                        break;
                    case "long":
                        p_s_type = "system.int64";
                        break;
                    case "float":
                        p_s_type = "system.single";
                        break;
                    case "double":
                        p_s_type = "system.double";
                        break;
                    case "bool":
                    case "boolean":
                        p_s_type = "system.boolean";
                        break;
                    case "char":
                    case "character":
                        p_s_type = "system.char";
                        break;
                    case "string":
                        p_s_type = "system.string";
                        break;
                    case "date":
                    case "time":
                    case "datetime":
                        p_s_type = "system.datetime";
                        break;
                    case "decimal":
                        p_s_type = "system.decimal";
                        break;
                    case "sbyte":
                        p_s_type = "system.sbyte";
                        break;
                    case "ushort":
                        p_s_type = "system.uint16";
                        break;
                    case "uint":
                        p_s_type = "system.uint32";
                        break;
                    case "ulong":
                        p_s_type = "system.uint64";
                        break;
                    case "object":
                        p_s_type = "system.object";
                        break;
                }

                /* cast string value into object */
                if ((p_s_type.Equals("string")) || (p_s_type.Equals("system.string")))
                {
                    /* recognize date format ISO-8601 */
                    if (ForestNET.Lib.Helper.IsDateTime(p_s_value))
                    {
                        o_foo = ForestNET.Lib.Helper.FromISO8601UTC(p_s_value);
                    }
                    else if (ForestNET.Lib.Helper.IsTime(p_s_value))
                    {
                        o_foo = ForestNET.Lib.Helper.FromISO8601UTC("1900-01-01T" + p_s_value + "Z").TimeOfDay;
                    }
                    else
                    {
                        if (p_s_value.Contains("\\" + this.s_stringQuote))
                        {
                            p_s_value = p_s_value.Replace("\\" + this.s_stringQuote, this.s_stringQuote);
                        }

                        o_foo = p_s_value;
                    }
                }
                else if (p_s_type.Equals("system.datetime"))
                {
                    if (ForestNET.Lib.Helper.IsDateTime(p_s_value))
                    {
                        o_foo = ForestNET.Lib.Helper.FromISO8601UTC(p_s_value);
                    }
                    else
                    {
                        throw new ArgumentException("Illegal value '" + p_s_value + "' for 'System.DateTime'");
                    }
                }
                else if (p_s_type.Equals("system.timespan"))
                {
                    if (ForestNET.Lib.Helper.IsDateTime(p_s_value))
                    {
                        o_foo = ForestNET.Lib.Helper.FromISO8601UTC(p_s_value).TimeOfDay;
                    }
                    else if (ForestNET.Lib.Helper.IsTime(p_s_value))
                    {
                        o_foo = ForestNET.Lib.Helper.FromISO8601UTC("1900-01-01T" + p_s_value + "Z").TimeOfDay;
                    }
                    else
                    {
                        throw new ArgumentException("Illegal value '" + p_s_value + "' for 'System.TimeSpan'");
                    }
                }
                else if ((p_s_type.Equals("number")) || (p_s_type.Equals("system.decimal")))
                {
                    if (!p_s_value.Equals("null"))
                    {
                        o_foo = Convert.ToDecimal(p_s_value, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    }
                }
                else if (p_s_type.Equals("system.single"))
                {
                    if (!p_s_value.Equals("null"))
                    {
                        o_foo = Convert.ToSingle(p_s_value, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    }
                }
                else if (p_s_type.Equals("system.double"))
                {
                    if (!p_s_value.Equals("null"))
                    {
                        o_foo = Convert.ToDouble(p_s_value, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    }
                }
                else if (p_s_type.Equals("system.byte"))
                {
                    if (!p_s_value.Equals("null"))
                    {
                        o_foo = Convert.ToByte(p_s_value);
                    }
                }
                else if (p_s_type.Equals("system.sbyte"))
                {
                    if (!p_s_value.Equals("null"))
                    {
                        o_foo = Convert.ToSByte(p_s_value);
                    }
                }
                else if (p_s_type.Equals("system.int16"))
                {
                    if (!p_s_value.Equals("null"))
                    {
                        o_foo = Convert.ToInt16(p_s_value);
                    }
                }
                else if (p_s_type.Equals("system.int32"))
                {
                    if (!p_s_value.Equals("null"))
                    {
                        o_foo = Convert.ToInt32(p_s_value);
                    }
                }
                else if (p_s_type.Equals("system.int64"))
                {
                    if (!p_s_value.Equals("null"))
                    {
                        o_foo = Convert.ToInt64(p_s_value);
                    }
                }
                else if (p_s_type.Equals("system.uint16"))
                {
                    if (!p_s_value.Equals("null"))
                    {
                        o_foo = Convert.ToUInt16(p_s_value);
                    }
                }
                else if (p_s_type.Equals("system.uint32"))
                {
                    if (!p_s_value.Equals("null"))
                    {
                        o_foo = Convert.ToUInt32(p_s_value);
                    }
                }
                else if (p_s_type.Equals("system.uint64"))
                {
                    if (!p_s_value.Equals("null"))
                    {
                        o_foo = Convert.ToUInt64(p_s_value);
                    }
                }
                else if (p_s_type.Equals("system.boolean"))
                {
                    if (!p_s_value.Equals("null"))
                    {
                        o_foo = Convert.ToBoolean(p_s_value);
                    }
                }
                else if (p_s_type.Equals("system.object"))
                {
                    if (!p_s_value.Equals("null"))
                    {
                        o_foo = p_s_value;
                    }
                }
                else if (p_s_type.Equals("null"))
                {
                    o_foo = null;
                }
                else
                {
                    throw new ArgumentException("Invalid type[" + p_s_type + "] for " + p_s_value);
                }
            }
            else
            {
                o_foo = "";
            }

            return o_foo;
        }

        /* Internal Classes */

        /// <summary>
        /// Enumeration of valid yaml types
        /// </summary>
        public enum YAMLValueType
        {
            String, Number, Integer, Boolean, Array, Object, Null
        }

        /// <summary>
        /// Encapsulation of a schema's yaml element
        /// </summary>
        public class YAMLElement
        {

            /* Fields */

            /* Properties */

            public string Name { get; set; }
            public int Level { get; set; }
            public string? Type { get; set; }
            public string? Description { get; set; }
            public string? Default { get; set; }
            public string? Value { get; set; }
            public string Mapping { get; set; }
            public string MappingClass { get; set; }
            public bool Required { get; set; }
            public bool PrimitiveArray { get; set; }
            public YAMLElement? Reference { get; set; }
            public List<YAMLElement> Children { get; }
            public List<YAMLRestriction> Restrictions { get; }

            /* Methods */

            /// <summary>
            /// YAMLElement constructor with empty string as name and level 0
            /// </summary>
            public YAMLElement() : this("", 0)
            {

            }

            /// <summary>
            /// YAMLElement constructor with parameter name and level 0
            /// </summary>
            /// <param name="p_s_name">name of yaml element in schema</param>
            public YAMLElement(string p_s_name) : this(p_s_name, 0)
            {

            }

            /// <summary>
            /// YAMLElement constructor
            /// </summary>
            /// <param name="p_s_name">name of yaml element in schema</param>
            /// <param name="p_i_level">level of yaml element</param>
            public YAMLElement(string p_s_name, int p_i_level)
            {
                this.Children = [];
                this.Restrictions = [];
                this.Name = p_s_name;
                this.Level = p_i_level;

                this.Mapping = string.Empty;
                this.MappingClass = string.Empty;
            }

            /// <summary>
            /// Print i tabs as indentation, i is the level of the yaml element
            /// </summary>
            /// <returns>indentation string</returns>
            private string PrintTabs()
            {
                string s_foo = "";

                for (int i = 0; i < this.Level; i++)
                {
                    s_foo += "\t";
                }

                return s_foo;
            }

            /// <summary>
            /// Returns each field of yaml element with name and value, separated by a pipe '|'
            /// </summary>
            override public string ToString()
            {
                string s_foo = this.PrintTabs() + "YAMLElement: ";

                foreach (System.Reflection.PropertyInfo o_property in this.GetType().GetProperties())
                {
                    string s_value = "";

                    try
                    {
                        if (o_property.PropertyType == typeof(YAMLElement))
                        {
                            s_value = "[" + o_property.GetValue(this)?.ToString() + "]";
                        }
                        else if ((o_property.PropertyType.IsGenericType) && (o_property.PropertyType.GetGenericTypeDefinition() == typeof(List<>)))
                        {
                            if (o_property.PropertyType.GenericTypeArguments[0] == typeof(YAMLElement))
                            {
                                List<YAMLElement> a_foo = (List<YAMLElement>)(o_property.GetValue(this) ?? new List<YAMLElement>());

                                if (a_foo.Count > 0)
                                {
                                    s_foo += ForestNET.Lib.IO.File.NEWLINE;

                                    foreach (YAMLElement o_foo in a_foo)
                                    {
                                        s_foo += o_foo.ToString() + ForestNET.Lib.IO.File.NEWLINE;
                                    }
                                }
                                else
                                {
                                    s_foo += "null" + ForestNET.Lib.IO.File.NEWLINE;
                                }
                            }
                            else if (o_property.PropertyType.GenericTypeArguments[0] == typeof(YAMLRestriction))
                            {
                                List<YAMLRestriction> a_foo = (List<YAMLRestriction>)(o_property.GetValue(this) ?? new List<YAMLRestriction>());

                                if (a_foo.Count > 0)
                                {
                                    s_foo += ForestNET.Lib.IO.File.NEWLINE;

                                    foreach (YAMLRestriction o_foo in a_foo)
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

            /// <summary>
            /// Returns mapping string of yaml element, mapping class or a combination of mapping and mapping class separated by ':'
            /// </summary>
            public string PrintMapping()
            {
                string s_foo = "";

                if ((ForestNET.Lib.Helper.IsStringEmpty(this.Mapping)) && (!ForestNET.Lib.Helper.IsStringEmpty(this.MappingClass)))
                {
                    s_foo = this.MappingClass;
                }
                else if ((!ForestNET.Lib.Helper.IsStringEmpty(this.Mapping)) && (!ForestNET.Lib.Helper.IsStringEmpty(this.MappingClass)))
                {
                    s_foo = this.Mapping + ":" + this.MappingClass;
                }

                return s_foo;
            }
        }

        /// <summary>
        /// Encapsulation of a schema's yaml element restrictions
        /// </summary>
        public class YAMLRestriction
        {

            /* Fields */

            /* Properties */

            public string Name { get; set; }
            public int Level { get; set; }
            public string StrValue { get; set; }
            public int IntValue { get; set; }

            /* Methods */

            /// <summary>
            /// YAMLRestriction constructor, no name value [= null], level 0, no string value [= null] and no integer value [= 0]
            /// </summary>
            public YAMLRestriction() : this("", 0, "", 0)
            {

            }

            /// <summary>
            /// YAMLRestriction constructor, level 0, no string value [= null] and no integer value [= 0]
            /// </summary>
            /// <param name="p_s_name">name of yaml element restriction</param>
            public YAMLRestriction(string p_s_name) : this(p_s_name, 0, "", 0)
            {

            }

            /// <summary>
            /// YAMLRestriction constructor, no string value [= null] and no integer value [= 0]
            /// </summary>
            /// <param name="p_s_name">name of yaml element restriction</param>
            /// <param name="p_i_level">level of yaml element restriction</param>
            public YAMLRestriction(string p_s_name, int p_i_level) : this(p_s_name, p_i_level, "", 0)
            {

            }

            /// <summary>
            /// YAMLRestriction constructor, no integer value [= 0]
            /// </summary>
            /// <param name="p_s_name">name of yaml element restriction</param>
            /// <param name="p_i_level">level of yaml element restriction</param>
            /// <param name="p_s_strValue">string value of restriction</param>
            public YAMLRestriction(string p_s_name, int p_i_level, string p_s_strValue) : this(p_s_name, p_i_level, p_s_strValue, 0)
            {

            }

            /// <summary>
            /// YAMLRestriction constructor
            /// </summary>
            /// <param name="p_s_name">name of yaml element restriction</param>
            /// <param name="p_i_level">level of yaml element restriction</param>
            /// <param name="p_s_strValue">string value of restriction</param>
            /// <param name="p_i_intValue">integer value of restriction</param>
            public YAMLRestriction(string p_s_name, int p_i_level, string p_s_strValue, int p_i_intValue)
            {
                this.Name = p_s_name;
                this.Level = p_i_level;
                this.StrValue = p_s_strValue;
                this.IntValue = p_i_intValue;
            }

            /// <summary>
            /// Print i tabs as indentation, i is the level of the yaml element
            /// </summary>
            /// <returns>indentation string</returns>
            private string PrintTabs()
            {
                string s_foo = "";

                for (int i = 0; i < this.Level; i++)
                {
                    s_foo += "\t";
                }

                return s_foo;
            }

            /// <summary>
            /// Returns each field of yaml element restriction with name and value, separated by a pipe '|'
            /// </summary>
            override public string ToString()
            {
                string s_foo = ForestNET.Lib.IO.File.NEWLINE + this.PrintTabs() + "\t" + "YAMLRestriction: ";

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
