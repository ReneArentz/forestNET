//using System.Collections.;
//using System.Globalization.;
//using System.Reflection.;
//using System.Text.;

namespace ForestNETLib.IO
{
    /// <summary>
    /// JSON class to encode and decode c# objects to json files with help of a json schema file/data.
    /// access to object fields can be directly on public fields or with public property methods (getXX setXX) on private fields.
    /// NOTE: mostly only primitive types supported for encoding and decoding, only supporting ISO-8601 UTC timestamps within json files.
    /// </summary>
    public class JSON
    {

        /* Fields */

        private JSONElement? o_currentElement;
        private JSONElement? o_schema;
        private string s_lineBreak = ForestNETLib.IO.File.NEWLINE;
        private JSONElement? a_definitions;
        private JSONElement? a_properties;
        private int i_level;

        private string? s_id;
        private string? s_schemaValue;

        /* Properties */

        public JSONElement? Root
        {
            get; set;
        }

        /// <summary>
        /// Determine line break characters for reading and writing json files
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
                ForestNETLib.Core.Global.ILogConfig("updated line break to [" + ForestNETLib.Core.Helper.BytesToHexString(System.Text.Encoding.UTF8.GetBytes(this.s_lineBreak), true) + "]");
            }
        }

        /// <summary>
        /// Determine if properties shall be used handling Objects.
        /// </summary>
        public bool UseProperties
        {
            get; set;
        }

        /* Methods */

        /// <summary>
        /// Empty JSON constructor
        /// </summary>
        public JSON()
        {
            this.LineBreak = ForestNETLib.IO.File.NEWLINE;
            this.UseProperties = false;
            this.i_level = 0;

            this.Root = new JSONElement("Root");
            this.o_schema = this.Root;
        }

        /// <summary>
        /// JSON constructor, giving file lines of schema as dynamic list for encoding and decoding json data
        /// </summary>
        /// <param name="p_a_jsonSchemaLines">file lines of schema as dynamic list</param>
        /// <exception cref="ArgumentException">value/structure within json schema invalid</exception>
        /// <exception cref="NullReferenceException">json schema, root node is null</exception>
        public JSON(List<string> p_a_jsonSchemaLines)
        {
            this.LineBreak = ForestNETLib.IO.File.NEWLINE;
            this.UseProperties = false;
            this.i_level = 0;

            System.Text.StringBuilder o_stringBuilder = new();

            /* read all json schema file lines to one string builder */
            foreach (string s_line in p_a_jsonSchemaLines)
            {
                o_stringBuilder.Append(s_line);
            }

            /* read all json-schema file lines and delete all line-wraps and tabs */
            string s_json = o_stringBuilder.ToString();
            s_json = new System.Text.RegularExpressions.Regex("[\\r\\n\\t]").Replace(s_json, "");

            /* remove all white spaces, but not between double quotes */
            s_json = JSON.RemoveWhiteSpaces(s_json);

            /* check if json-schema starts with curly brackets */
            if ((!s_json.StartsWith("{")) || (!s_json.EndsWith("}")))
            {
                throw new ArgumentException("JSON-schema must start with curly bracket '{' and end with curly bracket '}'.");
            }

            /* validate json schema */
            JSON.ValidateJSON(s_json);

            ForestNETLib.Core.Global.ILogConfig("json schema file lines validated");

            /* parse json */
            this.ParseJSON(s_json);

            ForestNETLib.Core.Global.ILogConfig("json schema parsed");

            /* set schema element with constructor input, root is unparsed schema */
            this.SetSchema(true);
        }

        /// <summary>
        /// JSON constructor, giving a schema json element object as schema for encoding and decoding json data
        /// </summary>
        /// <param name="p_o_schemaRoot">json element object as root schema node</param>
        /// <exception cref="ArgumentException">invalid parameters for constructor</exception>
        public JSON(JSONElement p_o_schemaRoot)
        {
            this.LineBreak = ForestNETLib.IO.File.NEWLINE;
            this.UseProperties = false;
            this.i_level = 0;
            this.Root = p_o_schemaRoot ?? throw new ArgumentException("json element parameter for schema is null");

            /* set schema element with constructor input, root is already parsed schema */
            this.SetSchema(false);
        }

        /// <summary>
        /// JSON constructor, giving a schema file as orientation for encoding and decoding json data
        /// </summary>
        /// <param name="p_s_file">full-path to json schema file</param>
        /// <exception cref="ArgumentException">value/structure within json schema file invalid</exception>
        /// <exception cref="NullReferenceException">json schema, root node is null</exception>
        /// <exception cref="System.IO.IOException">cannot access or open json file and it's content</exception>
        public JSON(string p_s_file)
        {
            this.LineBreak = ForestNETLib.IO.File.NEWLINE;
            this.UseProperties = false;
            this.i_level = 0;

            /* check if file exists */
            if (!File.Exists(p_s_file))
            {
                throw new ArgumentException("File[" + p_s_file + "] does not exist.");
            }

            /* open json-schema file */
            File o_file = new(p_s_file, false);

            /* read all json-schema file lines and delete all line-wraps and tabs */
            string s_json = o_file.FileContent;
            s_json = new System.Text.RegularExpressions.Regex("[\\r\\n\\t]").Replace(s_json, "");

            /* remove all white spaces, but not between double quotes */
            s_json = JSON.RemoveWhiteSpaces(s_json);

            /* check if json-schema starts with curly brackets */
            if ((!s_json.StartsWith("{")) || (!s_json.EndsWith("}")))
            {
                throw new ArgumentException("JSON-schema must start with curly bracket '{' and end with curly bracket '}'.");
            }

            /* validate json schema */
            JSON.ValidateJSON(s_json);

            ForestNETLib.Core.Global.ILogConfig("json schema file validated");

            /* parse json */
            this.ParseJSON(s_json);

            ForestNETLib.Core.Global.ILogConfig("json schema parsed");

            /* set schema element with constructor input, root is unparsed schema */
            this.SetSchema(true);
        }

        /// <summary>
        /// Method to set schema elements, afterwards each json constructor has read their input
        /// </summary>
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
                throw new NullReferenceException("Root node has no children[size=" + this.Root.Children.Count + "]");
            }

            if (p_b_rootIsUnparsedSchema)
            {
                if (ForestNETLib.Core.Global.IsILevel((byte)ForestNETLib.Log.Level.MASS)) ForestNETLib.Core.Global.ILogMass("Unparsed JSON-Schema: " + this.s_lineBreak + this.Root);

                /* reset level */
                this.i_level = 0;

                /* parse json schema */
                this.ParseJSONSchema(this.Root);

                ForestNETLib.Core.Global.ILogConfig("json schema elements parsed");

                /* reset children of root */
                this.Root.Children.Clear();

                if (this.a_properties != null)
                {
                    /* properties cannot have one child and 'Reference' */
                    if ((this.a_properties.Children.Count > 0) && (this.a_properties.Reference != null))
                    {
                        throw new ArgumentException("Properties after parsing json schema cannot have one child and 'Reference'");
                    }

                    /* check if properties has one child */
                    if (this.a_properties.Children.Count > 0)
                    {
                        /* add all 'properties' children to root */
                        foreach (JSONElement o_jsonElement in this.a_properties.Children)
                        {
                            this.Root.Children.Add(o_jsonElement);
                        }
                    }
                    else if (this.a_properties.Reference != null)
                    { /* we have 'Reference' in properties */
                        /* set properties 'Reference' as root 'Reference' */
                        this.Root.Reference = this.a_properties.Reference;
                    }
                }
            }

            if (ForestNETLib.Core.Global.IsILevel((byte)ForestNETLib.Log.Level.MASS)) ForestNETLib.Core.Global.ILogMass("Parsed JSON-Schema: " + this.s_lineBreak + this.Root);

            this.o_schema = this.Root;

            ForestNETLib.Core.Global.ILogConfig("set json root element as schema element");
        }

        /// <summary>
        /// Remove all white spaces from json content, but not within values escaped by double quotes
        /// </summary>
        /// <param name="p_s_json">json content as string</param>
        /// <returns>modified json content as string value</returns>
        private static string RemoveWhiteSpaces(string p_s_json)
        {
            string s_json = "";

            /* state variable if we are currently in a double quoted value or not */
            bool b_doubleQuoteActive = false;

            /* iterate json for each character */
            for (int i = 0; i < p_s_json.Length; i++)
            {
                /* if we found a not unescaped double quote */
                if ((i != 0) && (p_s_json[i] == '"') && (p_s_json[i - 1] != '\\'))
                {
                    /* if we are at the end of a double quoted value */
                    if (b_doubleQuoteActive)
                    {
                        /* unset state */
                        b_doubleQuoteActive = false;
                    }
                    else
                    {
                        /* we have a new double quoted value incoming, set state */
                        b_doubleQuoteActive = true;
                    }
                }

                if ((p_s_json[i] == ' ') && (!b_doubleQuoteActive))
                {
                    continue;
                }

                s_json += p_s_json[i];
            }

            return s_json;
        }

        /// <summary>
        /// Returns root element of json schema as string output and all of its children
        /// </summary>
        override public string ToString()
        {
            string s_foo = "";

            s_foo += "$id = " + this.s_id;
            s_foo += this.LineBreak;
            s_foo += "$schema = " + this.s_schemaValue;
            s_foo += this.LineBreak;
            s_foo += this.Root;

            return s_foo;
        }

        /// <summary>
        /// Generate indentation string for json specification
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

        /* parsing JSON schema */

        /// <summary>
        /// Validate json file if values and structure are correct, otherwise throw ArgumentException
        /// </summary>
        /// <param name="p_s_json">json content as string</param>
        /// <exception cref="ArgumentException">value or structure within json file lines invalid</exception>
        public static void ValidateJSON(string p_s_json)
        {
            /* remove all white spaces, even within double quoted values for validation */
            p_s_json = (new System.Text.RegularExpressions.Regex("\\s")).Replace(p_s_json, "");
            /* state variable if we are currently in a double quoted value or not */
            bool b_doubleQuoteActive = false;
            /* array of characters which are expected after double quoted value in json */
            List<char> a_allowedCharactersAfterDoubleQuote = [':', ',', '}', ']'];

            /* iterate json for each character */
            for (int i = 0; i < p_s_json.Length; i++)
            {
                /* if we found a not unescaped double quote */
                if ((p_s_json[i] == '"') && (p_s_json[i - 1] != '\\'))
                {
                    /* if we are at the end of a double quoted value */
                    if (b_doubleQuoteActive)
                    {
                        /* unset state */
                        b_doubleQuoteActive = false;

                        /* if we are not at the last character of the json document, we expect an allowed character after double quoted value ends */
                        if ((i != p_s_json.Length - 1) && (!a_allowedCharactersAfterDoubleQuote.Contains(p_s_json[i + 1])))
                        {
                            /* sequence variable for exception message */
                            string s_sequence;

                            if (i <= p_s_json.Length - 4)
                            {
                                if (i == 0)
                                {
                                    /* sequence if we are at the beginning of the json document */
                                    s_sequence = "" + p_s_json[i] + p_s_json[i + 1] + p_s_json[i + 2] + p_s_json[i + 3] + p_s_json[i + 4];
                                }
                                else
                                {
                                    /* standard sequence */
                                    s_sequence = "" + p_s_json[i - 1] + p_s_json[i] + p_s_json[i + 1] + p_s_json[i + 2] + p_s_json[i + 3];
                                }
                            }
                            else
                            {
                                /* add previous and current characters to the sequence */
                                s_sequence = "" + p_s_json[i - 1] + p_s_json[i];

                                /* iterate remaining characters and add them to the sequence */
                                for (int j = i + 1; j < p_s_json.Length; j++)
                                {
                                    s_sequence += p_s_json[j];
                                }
                            }

                            throw new ArgumentException("Expected ':', ',', '}' or ']' character, but found '" + p_s_json[i + 1] + "' in sequence '" + s_sequence + "', please check for unescaped double quotes");
                        }
                        else if (i != p_s_json.Length - 2)
                        { /* this check requires that we are not at the last and second last character of the json document */
                            /* sequence variable for exception message */
                            string s_sequence;

                            if (i <= p_s_json.Length - 5)
                            {
                                /* standard sequence */
                                s_sequence = "" + p_s_json[i] + p_s_json[i + 1] + p_s_json[i + 2] + p_s_json[i + 3] + p_s_json[i + 4];
                            }
                            else
                            {
                                /* add current character to the sequence */
                                s_sequence = "" + p_s_json[i];

                                /* iterate remaining characters and add them to the sequence */
                                for (int j = i + 1; j < p_s_json.Length; j++)
                                {
                                    s_sequence += p_s_json[j];
                                }
                            }

                            /* if the next character after the end of a double quoted value is ':' character */
                            if (p_s_json[i + 1] == ':')
                            {
                                if (
                                    /* allowed characters/digits after the end of a double quoted value and the ':' character */
                                    (p_s_json[i + 2] != '"') &&
                                    (p_s_json[i + 2] != '+') &&
                                    (p_s_json[i + 2] != '-') &&
                                    (p_s_json[i + 2] != 't') &&
                                    (p_s_json[i + 2] != 'f') &&
                                    (p_s_json[i + 2] != 'n') &&
                                    (p_s_json[i + 2] != '{') &&
                                    (p_s_json[i + 2] != '[') &&
                                    (!char.IsDigit(p_s_json[i + 2]))
                                )
                                {
                                    throw new ArgumentException("Expected '\"', '+', '-', 't', 'f', 'n', '{', '[' or a digit character, but found '" + p_s_json[i + 2] + "' in sequence '" + s_sequence + "', after ':'");
                                }
                            }
                            else if (p_s_json[i + 1] == ',')
                            { /* if the next character after the end of a double quoted value is ',' character */
                                /* '"' character only allowed after the end of a double quoted value and the ',' character */
                                if (p_s_json[i + 2] != '"')
                                {
                                    throw new ArgumentException("Expected '\"' character, but found '" + p_s_json[i + 2] + "' in sequence '" + s_sequence + "', after ','");
                                }
                            }
                            else if ((p_s_json[i + 1] == '}') || (p_s_json[i + 1] == ']'))
                            { /* if the next character after the end of a double quoted value is '}' or ']' character */
                                /* '}' or ',' character only allowed after the end of a double quoted value and the '}' or ']' character */
                                if ((p_s_json[i + 2] != '}') && (p_s_json[i + 2] != ',') && (p_s_json[i + 2] != ']'))
                                {
                                    throw new ArgumentException("Expected '}', ']' or ',' character, but found '" + p_s_json[i + 2] + "' in sequence '" + s_sequence + "', after '}' or ']'");
                                }
                            }
                        }
                    }
                    else
                    {
                        /* we have a new double quoted value incoming, set state */
                        b_doubleQuoteActive = true;
                    }
                }
                else
                {
                    /* if we are not at the last character of the json document and not in a double quoted value */
                    if ((i != p_s_json.Length - 1) && (!b_doubleQuoteActive))
                    {
                        /* sequence variable for exception message */
                        string s_sequence = "";

                        /* if we are at the end of the json document, we cannot add all sequence characters to exception message */
                        if (i <= p_s_json.Length - 3)
                        {
                            if (i > 2)
                            {
                                /* standard sequence */
                                s_sequence = "" + p_s_json[i - 2] + p_s_json[i - 1] + p_s_json[i] + p_s_json[i + 1] + p_s_json[i + 2];
                            }
                            else
                            {
                                /* get first characters at the beginning of the json document */
                                for (int j = 0; j < i; j++)
                                {
                                    s_sequence += p_s_json[j];
                                }

                                /* add rest of characters to the sequence */
                                s_sequence += p_s_json[i] + p_s_json[i + 1] + p_s_json[i + 2];
                            }
                        }
                        else
                        {
                            /* add previous and current characters to the sequence */
                            s_sequence = "" + p_s_json[i - 2] + p_s_json[i - 1] + p_s_json[i];

                            /* iterate remaining characters and add them to the sequence */
                            for (int j = i + 1; j < p_s_json.Length; j++)
                            {
                                s_sequence += p_s_json[j];
                            }
                        }

                        if ((p_s_json[i] == ',') && (p_s_json[i + 1] == '}'))
                        {
                            /* character sequence ,} is not allowed */
                            throw new ArgumentException("Sequence of ',}' is not allowed outside of double quoted values in sequence '" + s_sequence + "'");
                        }
                        else if ((p_s_json[i] == '}') && (p_s_json[i + 1] == '"'))
                        {
                            /* character sequence }" is not allowed */
                            throw new ArgumentException("Sequence of '}\"' is not allowed outside of double quoted values in sequence '" + s_sequence + "'");
                        }
                        else if ((i != p_s_json.Length - 2) && (p_s_json[i] == ',') && (p_s_json[i + 1] == '"') && (p_s_json[i + 2] == '"'))
                        {
                            /* character sequence ,"" is not allowed */
                            throw new ArgumentException("Sequence of ',\"\"' is not allowed outside of double quoted values in sequence '" + s_sequence + "'");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Analyze json element value and get a unique value type from it
        /// </summary>
        /// <param name="p_s_jsonValue">json element value as string</param>
        /// <returns>unique json value type</returns>
        /// <exception cref="ArgumentException">invalid value or json value type could not be determined</exception>
        private static JSONValueType GetJSONValueType(string? p_s_jsonValue)
        {
            JSONValueType e_jsonValueType = JSONValueType.String;

            /* get json value type */
            if ((p_s_jsonValue == null) || (ForestNETLib.Core.Helper.IsStringEmpty(p_s_jsonValue)))
            {
                e_jsonValueType = JSONValueType.Null;
            }
            else if (p_s_jsonValue[0] == '"')
            { /* json value starts with '"' character, so it is of type string */
                e_jsonValueType = JSONValueType.String;
            }
            else if ((p_s_jsonValue[0] == '+') || (p_s_jsonValue[0] == '-') || (char.IsDigit(p_s_jsonValue[0])))
            { /* json value starts with digit, '+' or '-' character, so it is of type number or integer */
                bool b_decimalDot = false;
                int i_amountPlusSign = 0;
                int i_amountMinusSign = 0;

                /* iterate value in detail */
                for (int i = 0; i < p_s_jsonValue.Length; i++)
                {
                    /* check if we found a decimal point */
                    if (p_s_jsonValue[i] == '.')
                    {
                        if (b_decimalDot)
                        {
                            /* if we already found a decimal point - number format is invalid */
                            throw new ArgumentException("Invalid number format, found second decimal point in value [" + p_s_jsonValue + "]");
                        }
                        else if (((i == 1) && (!char.IsDigit(p_s_jsonValue[0]))) || (i == p_s_jsonValue.Length - 1))
                        {
                            /* if decimal point has not a single previous digit or is at the end of the value - number format is invalid */
                            throw new ArgumentException("Invalid number format, decimal point at wrong position in value [" + p_s_jsonValue + "]");
                        }
                        else
                        {
                            /* set flag, that decimal point has been found */
                            b_decimalDot = true;
                        }
                    }
                    else if (p_s_jsonValue[i] == '+')
                    {
                        i_amountPlusSign++;

                        if (i_amountPlusSign > 1)
                        { /* exit number check, it seems to be a normal string starting with '++...' */
                            break;
                        }
                    }
                    else if (p_s_jsonValue[i] == '-')
                    {
                        i_amountMinusSign++;

                        if (i_amountMinusSign > 1)
                        { /* exit number check, it seems to be a normal string starting with '--...' */
                            break;
                        }
                    }
                    else if ((i != 0) && (!char.IsDigit(p_s_jsonValue[i])))
                    { /* check if we found a character which is not a digit */
                        /* we do not need an exception, just setting both sign counters to 1 and exit number check */
                        i_amountPlusSign = 1;
                        i_amountMinusSign = 1;
                        break;
                    }
                }

                /* only accept one plus-sign or one minus-sign */
                if (((i_amountPlusSign == 0) && (i_amountMinusSign == 0)) || (i_amountPlusSign == 1) ^ (i_amountMinusSign == 1))
                {
                    if (b_decimalDot)
                    {
                        /* decimal point found, value is of type number */
                        e_jsonValueType = JSONValueType.Number;
                    }
                    else
                    {
                        /* value is of type integer */
                        e_jsonValueType = JSONValueType.Integer;
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid integer or number, amount plus signs[" + i_amountPlusSign + "] and amount minus signs[" + i_amountMinusSign + "] found in value [" + p_s_jsonValue + "]");
                }
            }
            else if (p_s_jsonValue.ToLower().Equals("true") || p_s_jsonValue.ToLower().Equals("false"))
            { /* value equals 'true' or 'false', so it is of type bool */
                e_jsonValueType = JSONValueType.Boolean;
            }
            else if (p_s_jsonValue.ToLower().Equals("null"))
            { /* value equals 'null', so it is of type null */
                e_jsonValueType = JSONValueType.Null;
            }
            else if (p_s_jsonValue[0] == '[')
            { /* json value starts with '[' character, so it is of type array */
                e_jsonValueType = JSONValueType.Array;
            }
            else if (p_s_jsonValue[0] == '{')
            { /* json value starts with '{' character, so it is of type object */
                e_jsonValueType = JSONValueType.Object;
            }

            return e_jsonValueType;
        }

        /// <summary>
        /// Parse json content to json object structure, based on JSONElement and JSONRestriction
        /// </summary>
        /// <param name="p_s_json">json content as string</param>
        /// <exception cref="ArgumentException">value or structure within json file lines invalid</exception>
        /// <exception cref="NullReferenceException">value within json content missing or min. amount not available</exception>
        private void ParseJSON(string p_s_json)
        {
            /* create new root if is null */
            if (this.Root == null)
            {
                this.Root = new JSONElement("Root");
            }

            /* start position for parsing json schema */
            int i = 0;

            /* check if json starts with '{' or '[' character */
            if ((p_s_json[i] != '{') && (p_s_json[i] != '['))
            {
                throw new ArgumentException("Expected '{' or '[' character, but found '" + p_s_json[i] + "' in value [" + p_s_json.Substring(0, 10) + "...]");
            }

            /* increment position */
            i++;

            /* increase position as long as we do not find closing '}' character */
            while (p_s_json[i] != '}')
            {
                /* variable for current json line in json schema document, mostly separated by ',' character */
                string s_jsonLine = "";
                /* variable for interlacing objects/arrays within current part of json schema */
                int i_level = 0;
                /* state variable if we are currently in a double quoted value or not */
                bool b_doubleQuoteActive = false;

                /* increase position as long as we do not find ',' character, in a double quoted value or interlacing/array */
                while ((p_s_json[i] != ',') || (b_doubleQuoteActive) || (i_level != 0))
                {
                    /* if we found a not unescaped double quote */
                    if ((p_s_json[i] == '"') && (p_s_json[i - 1] != '\\'))
                    {
                        /* if we are at the end of a double quoted value */
                        if (b_doubleQuoteActive)
                        {
                            /* unset state */
                            b_doubleQuoteActive = false;
                        }
                        else
                        {
                            /* set state */
                            b_doubleQuoteActive = true;
                        }
                    }

                    /* if we are not in a double quoted value */
                    if (!b_doubleQuoteActive)
                    {
                        /* detect a new interlacing or array */
                        if ((p_s_json[i] == '{') || (p_s_json[i] == '['))
                        {
                            /* increase level value */
                            i_level++;
                        }

                        /* detect close of an interlacing or array */
                        if ((p_s_json[i] == '}') || (p_s_json[i] == ']'))
                        {
                            /* if current level is zero and interlacing is closing, we end 2nd level while loop */
                            if ((i_level == 0) && ((p_s_json[i] == '}') || (p_s_json[i] == ']')))
                            {
                                break;
                            }
                            else
                            {
                                /* decrease level */
                                i_level--;
                            }
                        }
                    }

                    /* add character to current json line */
                    s_jsonLine += p_s_json[i];

                    /* increment position */
                    i++;
                }

                ForestNETLib.Core.Global.ILogFiner(PrintIndentation() + s_jsonLine);

                /* set current element with root if it is empty */
                if (this.o_currentElement == null)
                {
                    this.o_currentElement = this.Root;
                }

                /* we have an array object, so we directly start another recursion */
                if ((s_jsonLine.StartsWith("{")) && (s_jsonLine.EndsWith("}")))
                {
                    ForestNETLib.Core.Global.ILogFiner(PrintIndentation() + "new ArrayObject()");

                    /* save current element in temporary variable */
                    JSONElement o_oldCurrentElement = this.o_currentElement;

                    /* with value type as object, we have a new current element */
                    JSONElement o_jsonElement = new("__ArrayObject__", this.i_level);

                    /* add new element to current element children and set as new current element */
                    this.o_currentElement.Children.Add(o_jsonElement);
                    this.o_currentElement = o_jsonElement;

                    /* increase level for PrintIndentation */
                    this.i_level++;

                    /* parse json value recursively */
                    this.ParseJSON(s_jsonLine);

                    /* decrease level for PrintIndentation */
                    this.i_level--;

                    /* reset current element with temporary variable */
                    this.o_currentElement = o_oldCurrentElement;
                }
                else
                {
                    /* the parsed json line must start with '"' or '{' character */
                    if (!s_jsonLine.StartsWith("\""))
                    {
                        throw new ArgumentException("Invalid format, line does not start with '\"'");
                    }

                    /* get property name of current json line */
                    string s_jsonProperty = s_jsonLine.Substring(0, s_jsonLine.IndexOf(":"));

                    /* property name must start and end with '"' character */
                    if ((!s_jsonProperty.StartsWith("\"")) && (!s_jsonProperty.EndsWith("\"")))
                    {
                        throw new ArgumentException("Invalid format for JSON property '" + s_jsonProperty + "'");
                    }

                    /* delete surrounded '"' characters from property name */
                    s_jsonProperty = s_jsonProperty.Substring(1, s_jsonProperty.Length - 1 - 1);

                    /* get value of current json line */
                    string s_jsonValue = s_jsonLine.Substring(s_jsonLine.IndexOf(":") + 1);

                    /* determine json value type */
                    JSONValueType e_jsonValueType = JSON.GetJSONValueType(s_jsonValue);

                    /* flag for handling array as object */
                    bool b_handleArrayAsObject = false;

                    /* if we have an array, we must determine if it is an array of objects */
                    if (e_jsonValueType == JSONValueType.Array)
                    {
                        if (s_jsonValue[1] == '{')
                        {
                            b_handleArrayAsObject = true;
                        }
                    }

                    /* if json value type is of type object, or array of objects */
                    if ((e_jsonValueType == JSONValueType.Object) || (b_handleArrayAsObject))
                    {
                        ForestNETLib.Core.Global.ILogFiner(PrintIndentation() + s_jsonProperty + " = (" + e_jsonValueType + ")");

                        /* save current element in temporary variable */
                        JSONElement o_oldCurrentElement = this.o_currentElement;

                        /* with value type as object, we have a new current element */
                        JSONElement o_jsonElement = new(s_jsonProperty, this.i_level);

                        /* add new element to current element children and set as new current element */
                        this.o_currentElement.Children.Add(o_jsonElement);
                        this.o_currentElement = o_jsonElement;

                        /* increase level for PrintIndentation */
                        this.i_level++;

                        /* parse json value recursively */
                        this.ParseJSON(s_jsonValue);

                        /* decrease level for PrintIndentation */
                        this.i_level--;

                        /* reset current element with temporary variable */
                        this.o_currentElement = o_oldCurrentElement;
                    }
                    else
                    {
                        ForestNETLib.Core.Global.ILogFiner(PrintIndentation() + s_jsonProperty + " = " + s_jsonValue + " (" + e_jsonValueType + ")");

                        /* add json property as a new child to current element */
                        JSONElement o_jsonElement = new(s_jsonProperty, this.i_level)
                        {
                            Value = s_jsonValue
                        };

                        this.o_currentElement.Children.Add(o_jsonElement);
                    }
                }

                /* if we detect close of json object with '}' or ']' character */
                if ((p_s_json[i] == '}') || (p_s_json[i] == ']'))
                {
                    /* end 1st level while loop */
                    break;
                }
                else
                {
                    /* increment position */
                    i++;
                }
            }
        }

        /// <summary>
        /// Parse json schema elements with all children elements
        /// </summary>
        /// <param name="p_o_jsonSchemaElement">json schema element object which will be parsed</param>
        /// <exception cref="ArgumentException">value or type within json schema invalid</exception>
        /// <exception cref="NullReferenceException">value within json schema missing or min. amount not available</exception>
        private void ParseJSONSchema(JSONElement p_o_jsonSchemaElement)
        {
            if (p_o_jsonSchemaElement.Children.Count > 0)
            {
                bool b_array = false;
                bool b_object = false;
                bool b_properties = false;
                bool b_items = false;

                /* 
				 * check if we have "type": "array" and "items" and no "properties"
				 * or if we have "type": "object" and "properties" and no "items"
				 */
                foreach (JSONElement o_jsonChild in p_o_jsonSchemaElement.Children)
                {
                    if (o_jsonChild.Name.ToLower().Equals("type"))
                    {
                        string s_type = o_jsonChild.Value ?? string.Empty;

                        /* remove surrounded double quotes from value */
                        if ((s_type.StartsWith("\"")) && (s_type.EndsWith("\"")))
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
                    else if (o_jsonChild.Name.ToLower().Equals("properties"))
                    {
                        b_properties = true;
                    }
                    else if (o_jsonChild.Name.ToLower().Equals("items"))
                    {
                        b_items = true;
                    }
                }

                /* control result of check */
                if ((!b_array) && (!b_object))
                {
                    if ((this.i_level == 0) && (!p_o_jsonSchemaElement.Name.ToLower().Equals("definitions")) && (!p_o_jsonSchemaElement.Name.ToLower().Equals("properties")))
                    {
                        throw new ArgumentException("JSON definition of element[definitions] or [properties] necessary on first level for [" + p_o_jsonSchemaElement.Name + "]");
                    }
                }
                else if ((b_array) && (b_properties))
                {
                    throw new ArgumentException("JSON definition with type[array] cannot have [properties] at the same time");
                }
                else if ((b_array) && (!b_items))
                {
                    throw new ArgumentException("JSON definition with type[array] must have [items] definition as well");
                }
                else if ((b_object) && (b_items))
                {
                    throw new ArgumentException("JSON definition with type[object] cannot have [items] at the same time");
                }
                else if ((b_object) && (!b_properties))
                {
                    throw new ArgumentException("JSON definition with type[object] must have [properties] definition as well");
                }

                foreach (JSONElement o_jsonChild in p_o_jsonSchemaElement.Children)
                {
                    JSONValueType e_jsonValueType;

                    /* determine json value type */
                    if (ForestNETLib.Core.Helper.IsStringEmpty(o_jsonChild.Value))
                    {
                        e_jsonValueType = JSONValueType.Object;
                    }
                    else
                    {
                        e_jsonValueType = JSON.GetJSONValueType(o_jsonChild.Value);

                        /* remove surrounded double quotes from value */
                        if ((o_jsonChild.Value != null) && (o_jsonChild.Value.StartsWith("\"")) && (o_jsonChild.Value.EndsWith("\"")))
                        {
                            o_jsonChild.Value = o_jsonChild.Value.Substring(1, o_jsonChild.Value.Length - 1 - 1);
                        }
                    }

                    ForestNETLib.Core.Global.ILogFiner(PrintIndentation() + o_jsonChild.Name + "(" + e_jsonValueType + ") of " + p_o_jsonSchemaElement.Name);

                    /* special properties at level 0 of json schema only */
                    if (o_jsonChild.Level == 0)
                    {
                        if (o_jsonChild.Name.ToLower().Equals("$id"))
                        {
                            if (e_jsonValueType == JSONValueType.String)
                            {
                                this.s_id = o_jsonChild.Value;
                            }
                            else
                            {
                                throw new ArgumentException("Invalid JSON type[" + e_jsonValueType + "] for property[" + o_jsonChild.Name + "] with value[" + o_jsonChild.Value + "]");
                            }
                        }
                        else if (o_jsonChild.Name.ToLower().Equals("$schema"))
                        {
                            if (e_jsonValueType == JSONValueType.String)
                            {
                                this.s_schemaValue = o_jsonChild.Value;
                            }
                            else
                            {
                                throw new ArgumentException("Invalid JSON type[" + e_jsonValueType + "] for property[" + o_jsonChild.Name + "] with value[" + o_jsonChild.Value + "]");
                            }
                        }
                        else if ((o_jsonChild.Name.ToLower().Equals("definitions")) || (o_jsonChild.Name.ToLower().Equals("properties")))
                        {
                            /* if json value type is of type object */
                            if (e_jsonValueType == JSONValueType.Object)
                            {
                                ForestNETLib.Core.Global.ILogFiner(PrintIndentation() + "save current element(" + this.o_currentElement?.Name + ") in temporary variable");

                                /* save current element in temporary variable */
                                JSONElement? o_oldCurrentElement = this.o_currentElement;

                                ForestNETLib.Core.Global.ILogFiner(PrintIndentation() + "start recursion for: " + o_jsonChild.Name + " = (" + e_jsonValueType + ") for " + this.o_currentElement?.Name);

                                if (o_jsonChild.Name.ToLower().Equals("definitions"))
                                {
                                    /* create new json element for 'definitions' */
                                    this.o_currentElement = new JSONElement(o_jsonChild.Name, 0);

                                    /* parse json value recursively */
                                    this.ParseJSONSchema(o_jsonChild);

                                    /* process current element as return value */
                                    this.a_definitions = this.o_currentElement;
                                }
                                else if (o_jsonChild.Name.ToLower().Equals("properties"))
                                {
                                    /* create new json element for 'properties' */
                                    this.o_currentElement = new JSONElement(o_jsonChild.Name, 0);

                                    /* parse json child recursively */
                                    this.ParseJSONSchema(o_jsonChild);

                                    /* process current element as return value */
                                    this.a_properties = this.o_currentElement;
                                }

                                ForestNETLib.Core.Global.ILogFiner(PrintIndentation() + "end recursion for " + this.o_currentElement?.Name + ": " + o_jsonChild.Name + " = (" + e_jsonValueType + ")");

                                ForestNETLib.Core.Global.ILogFiner(PrintIndentation() + "reset current element(" + this.o_currentElement?.Name + ") from temporary variable(" + o_oldCurrentElement?.Name + ")");

                                /* reset current element from temporary variable */
                                this.o_currentElement = o_oldCurrentElement;

                                /* we can skip the rest of loop processing */
                                continue;
                            }
                            else
                            {
                                throw new ArgumentException("Invalid JSON type[" + e_jsonValueType + "] for property[" + o_jsonChild.Name + "] of type object");
                            }
                        }
                    }

                    /* default parsing of properties for json schema */

                    /* if current element is another object we need another recursion */
                    if (e_jsonValueType == JSONValueType.Object)
                    {
                        bool b_skip = false;

                        /* check if new object has property name 'items' */
                        if (o_jsonChild.Name.ToLower().Equals("items"))
                        {
                            if ((this.o_currentElement != null) && (this.o_currentElement.Type != null) && (!this.o_currentElement.Type.ToLower().Equals("array")))
                            {
                                throw new ArgumentException("JSON object[" + this.o_currentElement.Name + "] with property[items] must be of type 'array' != '" + this.o_currentElement.Type + "'");
                            }

                            if (o_jsonChild.Children.Count == 1)
                            {
                                JSONElement o_itemChild = o_jsonChild.Children[0];

                                if (!ForestNETLib.Core.Helper.IsStringEmpty(o_itemChild.Value))
                                {
                                    /* remove surrounded double quotes from value */
                                    string? s_itemValue = o_itemChild.Value?.Substring(1, o_itemChild.Value.Length - 1 - 1);

                                    if ((o_itemChild.Name.ToLower().Equals("$ref")) && (s_itemValue != null) && (s_itemValue.StartsWith("#")))
                                    {
                                        string s_referenceName = s_itemValue.Replace("#/definitions/", "");

                                        if (this.a_definitions != null)
                                        {
                                            foreach (JSONElement o_jsonDefinition in this.a_definitions.Children)
                                            {
                                                if (o_jsonDefinition.Name.Equals(s_referenceName))
                                                {
                                                    ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + "setReference for (" + this.o_currentElement?.Name + ") with reference=" + s_referenceName);

                                                    if (this.o_currentElement == null)
                                                    {
                                                        throw new NullReferenceException("Cannot set reference '" + o_jsonDefinition.Name + "' to current element which is 'null'");
                                                    }

                                                    this.o_currentElement.Reference = o_jsonDefinition;
                                                    b_skip = true;
                                                }
                                            }
                                        }

                                        if (!b_skip)
                                        {
                                            throw new ArgumentException("JSON schema definition[" + s_referenceName + "] not found under #/definitions/");
                                        }
                                    }
                                }
                            }
                        }

                        /* skip because of 'items' object with one reference */
                        if (!b_skip)
                        { /* new object */
                            ForestNETLib.Core.Global.ILogFiner(PrintIndentation() + o_jsonChild.Name + " = (" + e_jsonValueType + ")");

                            if (o_jsonChild.Name.ToLower().Equals("properties"))
                            {
                                ForestNETLib.Core.Global.ILogFiner(PrintIndentation() + "start recursion for: " + o_jsonChild.Name + " = (" + e_jsonValueType + ") for " + this.o_currentElement?.Name);

                                /* parse json child recursively */
                                this.ParseJSONSchema(o_jsonChild);

                                ForestNETLib.Core.Global.ILogFiner(PrintIndentation() + "end recursion for: " + o_jsonChild.Name + " = (" + e_jsonValueType + ")");
                            }
                            else
                            {
                                ForestNETLib.Core.Global.ILogFiner(PrintIndentation() + "save current element(" + this.o_currentElement?.Name + ") in temporary variable");

                                /* save current element in temporary variable */
                                JSONElement? o_oldCurrentElement = this.o_currentElement;

                                if (!ForestNETLib.Core.Helper.MatchesRegex(o_jsonChild.Name, "[a-zA-Z0-9-_]*"))
                                {
                                    throw new ArgumentException("Invalid schema element name '" + o_jsonChild.Name + "', invalid characters. Following characters are allowed: [a-z], [A-Z], [0-9], [-] and [_]");
                                }

                                ForestNETLib.Core.Global.ILogFiner(PrintIndentation() + "start recursion for: " + o_jsonChild.Name + " = (" + e_jsonValueType + ") for " + this.o_currentElement?.Name);

                                /* create new json element */
                                JSONElement o_newJSONElement = new(o_jsonChild.Name, this.i_level);

                                /* if we have Root node as current element on level 0 with type 'array' and new child 'items', we must not add a new child, because of concurrent modification of the for loop */
                                bool b_handleRootItems = false;

                                if ((this.o_currentElement != null) && (this.o_currentElement.Name.Equals("Root")) && (this.o_currentElement.Level == 0) && (this.o_currentElement.Type != null) && (this.o_currentElement.Type.ToLower().Equals("array")) && (o_newJSONElement.Name.ToLower().Equals("items")))
                                {
                                    b_handleRootItems = true;
                                }
                                else
                                {
                                    /* add new json element to current elements children */
                                    this.o_currentElement?.Children.Add(o_newJSONElement);
                                }

                                /* set new json element as current element for recursive processing */
                                this.o_currentElement = o_newJSONElement;

                                /* increase level for PrintIndentation */
                                this.i_level++;

                                /* parse json child recursively */
                                this.ParseJSONSchema(o_jsonChild);

                                ForestNETLib.Core.Global.ILogFiner(PrintIndentation() + "reset current element(" + this.o_currentElement?.Name + ") from temporary variable(" + o_oldCurrentElement?.Name + ")");

                                /* reset current element from temporary variable */
                                this.o_currentElement = o_oldCurrentElement;

                                /* check if we must handle root items */
                                if (b_handleRootItems)
                                {
                                    /* new element 'items', we set the child as reference of Root */
                                    if (o_newJSONElement.Children.Count == 1)
                                    {
                                        ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + "setReference for (" + this.o_currentElement?.Name + ") with object(" + o_newJSONElement.Children[0].Name + ")");

                                        if (this.o_currentElement == null)
                                        {
                                            throw new NullReferenceException("Cannot set reference '" + o_newJSONElement.Children[0].Name + "' to current element which is 'null'");
                                        }

                                        this.o_currentElement.Reference = o_newJSONElement.Children[0];
                                    }
                                    else
                                    {
                                        throw new ArgumentException("Root items must have only one child, but there are (" + o_newJSONElement.Children.Count + ") children");
                                    }
                                }

                                /* decrease level for PrintIndentation */
                                this.i_level--;

                                /* between update of schema definitions, for the case that a definition is depending on another definition before */
                                if ((this.o_currentElement != null) && (this.o_currentElement.Name.ToLower().Equals("definitions")))
                                {
                                    this.a_definitions = this.o_currentElement;
                                }

                                ForestNETLib.Core.Global.ILogFiner(PrintIndentation() + "end recursion for: " + o_jsonChild.Name + " = (" + e_jsonValueType + ")");
                            }
                        }
                    }
                    else
                    {
                        ForestNETLib.Core.Global.ILogFiner(PrintIndentation() + o_jsonChild.Name + " = " + o_jsonChild.Value + " (" + e_jsonValueType + ") for " + this.o_currentElement?.Name);

                        if (this.o_currentElement == null)
                        {
                            throw new NullReferenceException("Cannot continue with current element as 'null'");
                        }

                        if (o_jsonChild.Name.ToLower().Equals("$ref"))
                        {
                            if (e_jsonValueType == JSONValueType.String)
                            {
                                bool b_found = false;
                                string? s_referenceName = o_jsonChild.Value?.Replace("#/definitions/", "");

                                if ((this.a_definitions != null) && (s_referenceName != null))
                                {
                                    foreach (JSONElement o_jsonDefinition in this.a_definitions.Children)
                                    {
                                        if (o_jsonDefinition.Name.Equals(s_referenceName))
                                        {
                                            this.o_currentElement.Reference = o_jsonDefinition;
                                            b_found = true;
                                        }
                                    }
                                }

                                if (!b_found)
                                {
                                    throw new ArgumentException("JSON definition[" + s_referenceName + "] not found under #/definitions/ in json schema");
                                }
                            }
                            else
                            {
                                throw new ArgumentException("Invalid JSON type[" + e_jsonValueType + "] for property[" + o_jsonChild.Name + "] with value[" + o_jsonChild.Value + "]");
                            }
                        }
                        else if (o_jsonChild.Name.ToLower().Equals("type"))
                        {
                            if (e_jsonValueType == JSONValueType.String)
                            {
                                List<string> a_validTypes = ["string", "number", "integer", "boolean", "array", "object", "null"];

                                /* store type value in temp. variable */
                                string s_foo = o_jsonChild.Value ?? string.Empty;

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
                                    throw new ArgumentException("Invalid value[" + o_jsonChild.Value + "] for property[type], allowed values are " + a_validTypes);
                                }

                                /* store type value */
                                this.o_currentElement.Type = s_foo;
                            }
                            else
                            {
                                throw new ArgumentException("Invalid JSON type[" + e_jsonValueType + "] for property[" + o_jsonChild.Name + "] with value[" + o_jsonChild.Value + "]");
                            }
                        }
                        else if (o_jsonChild.Name.ToLower().Equals("description"))
                        {
                            if (e_jsonValueType == JSONValueType.String)
                            {
                                this.o_currentElement.Description = o_jsonChild.Value;
                            }
                            else
                            {
                                throw new ArgumentException("Invalid JSON type[" + e_jsonValueType + "] for property[" + o_jsonChild.Name + "] with value[" + o_jsonChild.Value + "]");
                            }
                        }
                        else if (o_jsonChild.Name.ToLower().Equals("default"))
                        {
                            if (e_jsonValueType == JSONValueType.String)
                            {
                                this.o_currentElement.Default = o_jsonChild.Value;
                            }
                            else
                            {
                                throw new ArgumentException("Invalid JSON type[" + e_jsonValueType + "] for property[" + o_jsonChild.Name + "] with value[" + o_jsonChild.Value + "]");
                            }
                        }
                        else if (o_jsonChild.Name.ToLower().Equals("mapping"))
                        {
                            if (e_jsonValueType == JSONValueType.String)
                            {
                                if ((o_jsonChild.Value != null) && (o_jsonChild.Value.Contains(':')))
                                { /* set mapping and mappingClass */
                                    this.o_currentElement.Mapping = o_jsonChild.Value.Substring(0, o_jsonChild.Value.IndexOf(":"));
                                    this.o_currentElement.MappingClass = o_jsonChild.Value.Substring(o_jsonChild.Value.IndexOf(":") + 1, o_jsonChild.Value.Length - (o_jsonChild.Value.IndexOf(":") + 1));
                                }
                                else
                                { /* set only mappingClass */
                                    this.o_currentElement.MappingClass = o_jsonChild.Value ?? string.Empty;
                                }
                            }
                            else
                            {
                                throw new ArgumentException("Invalid JSON type[" + e_jsonValueType + "] for property[" + o_jsonChild.Name + "] with value[" + o_jsonChild.Value + "]");
                            }
                        }
                        else if (o_jsonChild.Name.ToLower().Equals("maxitems"))
                        {
                            if (e_jsonValueType == JSONValueType.Integer)
                            {
                                if ((this.o_currentElement.Type != null) && (!this.o_currentElement.Type.ToLower().Equals("array")))
                                {
                                    throw new ArgumentException("Invalid JSON restriction[" + o_jsonChild.Name + "] for [" + this.o_currentElement.Name + "] with type[" + o_jsonChild.Type + "], type must be array");
                                }

                                this.o_currentElement.Restrictions.Add(new JSONRestriction(o_jsonChild.Name, this.i_level, "", int.Parse(o_jsonChild.Value ?? string.Empty)));
                            }
                            else
                            {
                                throw new ArgumentException("Invalid JSON type[" + e_jsonValueType + "] for property[" + o_jsonChild.Name + "] with value[" + o_jsonChild.Value + "]");
                            }
                        }
                        else if (o_jsonChild.Name.ToLower().Equals("minitems"))
                        {
                            if (e_jsonValueType == JSONValueType.Integer)
                            {
                                if ((this.o_currentElement.Type != null) && (!this.o_currentElement.Type.ToLower().Equals("array")))
                                {
                                    throw new ArgumentException(this.o_currentElement.Name + " - Invalid JSON restriction[" + o_jsonChild.Name + "] for [" + this.o_currentElement.Name + "] with type[" + o_jsonChild.Type + "], type must be array");
                                }

                                this.o_currentElement.Restrictions.Add(new JSONRestriction(o_jsonChild.Name, this.i_level, "", int.Parse(o_jsonChild.Value ?? string.Empty)));
                            }
                            else
                            {
                                throw new ArgumentException("Invalid JSON type[" + e_jsonValueType + "] for property[" + o_jsonChild.Name + "] with value[" + o_jsonChild.Value + "]");
                            }
                        }
                        else if (o_jsonChild.Name.ToLower().Equals("minimum"))
                        {
                            if (e_jsonValueType == JSONValueType.Integer)
                            {
                                this.o_currentElement.Restrictions.Add(new JSONRestriction(o_jsonChild.Name, this.i_level, "", int.Parse(o_jsonChild.Value ?? string.Empty)));
                            }
                            else
                            {
                                throw new ArgumentException("Invalid JSON type[" + e_jsonValueType + "] for property[" + o_jsonChild.Name + "] with value[" + o_jsonChild.Value + "]");
                            }
                        }
                        else if (o_jsonChild.Name.ToLower().Equals("exclusiveminimum"))
                        {
                            if (e_jsonValueType == JSONValueType.Integer)
                            {
                                this.o_currentElement.Restrictions.Add(new JSONRestriction(o_jsonChild.Name, this.i_level, "", int.Parse(o_jsonChild.Value ?? string.Empty)));
                            }
                            else
                            {
                                throw new ArgumentException("Invalid JSON type[" + e_jsonValueType + "] for property[" + o_jsonChild.Name + "] with value[" + o_jsonChild.Value + "]");
                            }
                        }
                        else if (o_jsonChild.Name.ToLower().Equals("maximum"))
                        {
                            if (e_jsonValueType == JSONValueType.Integer)
                            {
                                this.o_currentElement.Restrictions.Add(new JSONRestriction(o_jsonChild.Name, this.i_level, "", int.Parse(o_jsonChild.Value ?? string.Empty)));
                            }
                            else
                            {
                                throw new ArgumentException("Invalid JSON type[" + e_jsonValueType + "] for property[" + o_jsonChild.Name + "] with value[" + o_jsonChild.Value + "]");
                            }
                        }
                        else if (o_jsonChild.Name.ToLower().Equals("exclusivemaximum"))
                        {
                            if (e_jsonValueType == JSONValueType.Integer)
                            {
                                this.o_currentElement.Restrictions.Add(new JSONRestriction(o_jsonChild.Name, this.i_level, "", int.Parse(o_jsonChild.Value ?? string.Empty)));
                            }
                            else
                            {
                                throw new ArgumentException("Invalid JSON type[" + e_jsonValueType + "] for property[" + o_jsonChild.Name + "] with value[" + o_jsonChild.Value + "]");
                            }
                        }
                        else if (o_jsonChild.Name.ToLower().Equals("maxlength"))
                        {
                            if (e_jsonValueType == JSONValueType.Integer)
                            {
                                this.o_currentElement.Restrictions.Add(new JSONRestriction(o_jsonChild.Name, this.i_level, "", int.Parse(o_jsonChild.Value ?? string.Empty)));
                            }
                            else
                            {
                                throw new ArgumentException("Invalid JSON type[" + e_jsonValueType + "] for property[" + o_jsonChild.Name + "] with value[" + o_jsonChild.Value + "]");
                            }
                        }
                        else if (o_jsonChild.Name.ToLower().Equals("minlength"))
                        {
                            if (e_jsonValueType == JSONValueType.Integer)
                            {
                                this.o_currentElement.Restrictions.Add(new JSONRestriction(o_jsonChild.Name, this.i_level, "", int.Parse(o_jsonChild.Value ?? string.Empty)));
                            }
                            else
                            {
                                throw new ArgumentException("Invalid JSON type[" + e_jsonValueType + "] for property[" + o_jsonChild.Name + "] with value[" + o_jsonChild.Value + "]");
                            }
                        }
                        else if (o_jsonChild.Name.ToLower().Equals("pattern"))
                        {
                            if (e_jsonValueType == JSONValueType.String)
                            {
                                this.o_currentElement.Restrictions.Add(new JSONRestriction(o_jsonChild.Name, this.i_level, o_jsonChild.Value ?? string.Empty));
                            }
                            else
                            {
                                throw new ArgumentException("Invalid JSON type[" + e_jsonValueType + "] for property[" + o_jsonChild.Name + "] with value[" + o_jsonChild.Value + "]");
                            }
                        }
                        else if (o_jsonChild.Name.ToLower().Equals("required"))
                        {
                            if (e_jsonValueType == JSONValueType.Array)
                            {
                                /* check if current element has any children */
                                if (this.o_currentElement.Children.Count < 1)
                                {
                                    throw new ArgumentException("Current element[" + this.o_currentElement.Name + "] must have at least one child for assigning 'required' property");
                                }

                                /* get json array */
                                string s_array = o_jsonChild.Value ?? string.Empty;

                                /* check if array is surrounded with '[' and ']' characters */
                                if ((!s_array.StartsWith("[")) || (!s_array.EndsWith("]")))
                                {
                                    throw new ArgumentException("Invalid format for JSON type[" + e_jsonValueType + "] for property[" + o_jsonChild.Name + "] with value[" + o_jsonChild.Value + "], must start with '[' and end with ']'");
                                }

                                /* remove surrounding '[' and ']' characters */
                                s_array = s_array.Substring(1, s_array.Length - 1 - 1);

                                /* split array in its values */
                                string[] a_arrayValues = s_array.Split(",");

                                /* iterate each array value */
                                foreach (string s_foo in a_arrayValues)
                                {
                                    string s_arrayValue = s_foo;

                                    /* check if array value is surrounded with '"' and '"' characters */
                                    if ((!s_arrayValue.StartsWith("\"")) || (!s_arrayValue.EndsWith("\"")))
                                    {
                                        throw new ArgumentException("Invalid format for array value[" + s_arrayValue + "] for property[" + o_jsonChild.Name + "], must start with '\"' and end with '\"'");
                                    }

                                    /* remove surrounding '"' and '"' characters */
                                    s_arrayValue = s_arrayValue.Substring(1, s_arrayValue.Length - 1 - 1);

                                    bool b_requiredFound = false;
                                    List<JSONElement>? a_children = null;

                                    /* check if we are at 'root' level */
                                    if ((this.o_currentElement.Name.Equals("Root")) && (this.o_currentElement.Level == 0))
                                    {
                                        /* look for 'properties' child */
                                        foreach (JSONElement o_jsonCurrentElementChild in this.o_currentElement.Children)
                                        {
                                            if (o_jsonCurrentElementChild.Name.ToLower().Equals("properties"))
                                            {
                                                /* set 'properties' children as array to search for 'required' element */
                                                a_children = o_jsonCurrentElementChild.Children;
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
                                        throw new NullReferenceException("Cannot handle required property[" + s_arrayValue + "] in array[" + o_jsonChild.Name + "] because current element[" + this.o_currentElement.Name + "] has no children or no properties");
                                    }

                                    /* iterate all children of current element to find required 'property' */
                                    foreach (JSONElement o_jsonCurrentElementChild in a_children)
                                    {
                                        /* compare by property name */
                                        if (o_jsonCurrentElementChild.Name.ToLower().Equals(s_arrayValue.ToLower()))
                                        {
                                            b_requiredFound = true;
                                            o_jsonCurrentElementChild.Required = true;
                                            break;
                                        }
                                    }

                                    if (!b_requiredFound)
                                    {
                                        throw new ArgumentException("Required property[" + s_arrayValue + "] in array[" + o_jsonChild.Name + "] does not exist within 'properties'");
                                    }
                                }
                            }
                            else
                            {
                                throw new ArgumentException("Invalid JSON type[" + e_jsonValueType + "] for property[" + o_jsonChild.Name + "] with value[" + o_jsonChild.Value + "]");
                            }
                        }
                    }
                }
            }
        }

        /* encoding data to JSON with JSON schema */

        /// <summary>
        /// Encode c# object to a json file, keep existing json file
        /// </summary>
        /// <param name="p_o_object">source c# object to encode json information</param>
        /// <param name="p_s_jsonFile">destination json file to save encoded json information</param>
        /// <returns>file object with encoded json content</returns>
        /// <exception cref="System.IO.IOException">cannot create or access destination json file</exception>
        /// <exception cref="NullReferenceException">value in schema or expected element is not available</exception>
        /// <exception cref="ArgumentException">condition failed for decoding json correctly</exception>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        public File JsonEncode(Object? p_o_object, string p_s_jsonFile)
        {
            return this.JsonEncode(p_o_object, p_s_jsonFile, false, true);
        }

        /// <summary>
        /// Encode c# object to a json file
        /// </summary>
        /// <param name="p_o_object">source c# object to encode json information</param>
        /// <param name="p_s_jsonFile">destination json file to save encoded json information</param>
        /// <param name="p_b_overwrite">true - overwrite existing json file, false - keep existing json file</param>
        /// <returns>file object with encoded json content</returns>
        /// <exception cref="System.IO.IOException">cannot create or access destination json file</exception>
        /// <exception cref="NullReferenceException">value in schema or expected element is not available</exception>
        /// <exception cref="ArgumentException">condition failed for decoding json correctly</exception>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        public File JsonEncode(Object? p_o_object, string p_s_jsonFile, bool p_b_overwrite)
        {
            return this.JsonEncode(p_o_object, p_s_jsonFile, p_b_overwrite, true);
        }

        /// <summary>
        /// Encode c# object to a json file
        /// </summary>
        /// <param name="p_o_object">source c# object to encode json information</param>
        /// <param name="p_s_jsonFile">destination json file to save encoded json information</param>
        /// <param name="p_b_overwrite">true - overwrite existing json file, false - keep existing json file</param>
        /// <param name="p_b_prettyPrint">true - keep json file structure over multiple lines, false - delete all line breaks and white spaces not escaped by double quotes</param>
        /// <returns>encoded json information from c# object as string</returns>
        /// <exception cref="System.IO.IOException">cannot create or access destination json file</exception>
        /// <exception cref="NullReferenceException">value in schema or expected element is not available</exception>
        /// <exception cref="ArgumentException">condition failed for decoding json correctly</exception>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        public File JsonEncode(Object? p_o_object, string p_s_jsonFile, bool p_b_overwrite, bool p_b_prettyPrint)
        {
            /* check schema field */
            if (this.o_schema == null)
            {
                throw new NullReferenceException("Cannot encode data. Schema is null.");
            }

            /* set level for PrintIndentation to zero */
            this.i_level = 0;

            /* encode data to json recursive */
            string s_json = this.JsonEncodeRecursive(this.o_schema, p_o_object, false);

            if (ForestNETLib.Core.Global.IsILevel((byte)ForestNETLib.Log.Level.MASS)) ForestNETLib.Core.Global.ILogMass("Encoded JSON:" + this.s_lineBreak + s_json);

            if (!p_b_prettyPrint)
            {
                /* read all json file lines and delete all line-wraps and tabs */
                s_json = new System.Text.RegularExpressions.Regex("[\\r\\n\\t]").Replace(s_json, "");

                /* remove all white spaces, but not between double quotes */
                s_json = JSON.RemoveWhiteSpaces(s_json);
            }

            /* if file does not exist we must create an new file */
            if (!File.Exists(p_s_jsonFile))
            {
                if (p_b_overwrite)
                {
                    p_b_overwrite = false;
                }
            }

            /* open (new) file */
            File o_file = new(p_s_jsonFile, !p_b_overwrite);

            /* save json encoded data into file */
            o_file.ReplaceContent(s_json);

            /* return file object */
            return o_file;
        }

        /// <summary>
        /// Encode c# object to a json content string
        /// </summary>
        /// <param name="p_o_object">source c# object to encode json information</param>
        /// <returns>encoded json information from c# object as string</returns>
        /// <exception cref="System.IO.IOException">cannot create or access destination json file</exception>
        /// <exception cref="NullReferenceException">value in schema or expected element is not available</exception>
        /// <exception cref="ArgumentException">condition failed for decoding json correctly</exception>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        public string JsonEncode(Object? p_o_object)
        {
            return this.JsonEncode(p_o_object, true);
        }

        /// <summary>
        /// Encode c# object to a json content string
        /// </summary>
        /// <param name="p_o_object">source c# object to encode json information</param>
        /// <param name="p_b_prettyPrint">true - keep json file structure over multiple lines, false - delete all line breaks and white spaces not escaped by double quotes</param>
        /// <returns>encoded json information from c# object as string</returns>
        /// <exception cref="System.IO.IOException">cannot create or access destination json file</exception>
        /// <exception cref="NullReferenceException">value in schema or expected element is not available</exception>
        /// <exception cref="ArgumentException">condition failed for decoding json correctly</exception>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        public string JsonEncode(Object? p_o_object, bool p_b_prettyPrint)
        {
            /* check schema field */
            if (this.o_schema == null)
            {
                throw new NullReferenceException("Cannot encode data. Schema is null.");
            }

            /* set level for PrintIndentation to zero */
            this.i_level = 0;

            /* encode data to json recursive */
            string s_json = this.JsonEncodeRecursive(this.o_schema, p_o_object, false);

            if (ForestNETLib.Core.Global.IsILevel((byte)ForestNETLib.Log.Level.MASS)) ForestNETLib.Core.Global.ILogMass("Encoded JSON:" + this.s_lineBreak + s_json);

            if (!p_b_prettyPrint)
            {
                /* read all json file lines and delete all line-wraps and tabs */
                s_json = new System.Text.RegularExpressions.Regex("[\\r\\n\\t]").Replace(s_json, "");

                /* remove all white spaces, but not between double quotes */
                s_json = JSON.RemoveWhiteSpaces(s_json);
            }

            /* return json content string */
            return s_json;
        }

        /// <summary>
        /// Recursive method to encode c# object and it's fields to a json string
        /// </summary>
        /// <param name="p_o_jsonSchemaElement">current json schema element with additional information for encoding</param>
        /// <param name="p_o_object">source c# object to encode json information</param>
        /// <param name="p_b_parentIsArray">hint that the parent json element is an array collection</param>
        /// <returns>encoded json information from c# object as string</returns>
        /// <exception cref="NullReferenceException">value in schema or expected element is not available</exception>
        /// <exception cref="ArgumentException">condition failed for decoding json correctly</exception>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        private string JsonEncodeRecursive(JSONElement p_o_jsonSchemaElement, Object? p_o_object, bool p_b_parentIsArray)
        {
            string s_json = "";
            string s_jsonReferenceParentName = "";

            /* if type and mapping class are not set, we need at least a reference to continue */
            if ((ForestNETLib.Core.Helper.IsStringEmpty(p_o_jsonSchemaElement.Type)) && (ForestNETLib.Core.Helper.IsStringEmpty(p_o_jsonSchemaElement.MappingClass)))
            {
                if (p_o_jsonSchemaElement.Reference == null)
                {
                    throw new NullReferenceException("Schema-element[" + p_o_jsonSchemaElement.Name + "] has no type, no mapping class and no reference");
                }
                else
                {
                    /* save name of current schema-element */
                    s_jsonReferenceParentName = p_o_jsonSchemaElement.Name;

                    /* set reference as current schema-element */
                    p_o_jsonSchemaElement = p_o_jsonSchemaElement.Reference;
                }
            }

            /* check if type is set */
            if ((p_o_jsonSchemaElement.Type == null) || (ForestNETLib.Core.Helper.IsStringEmpty(p_o_jsonSchemaElement.Type)))
            {
                throw new NullReferenceException("Schema-element[" + p_o_jsonSchemaElement.Name + "] has no type");
            }

            /* check if mapping class is set if schema-element is not 'items' */
            if (ForestNETLib.Core.Helper.IsStringEmpty(p_o_jsonSchemaElement.MappingClass))
            {
                if (!p_o_jsonSchemaElement.Name.ToLower().Equals("items"))
                {
                    throw new NullReferenceException("Schema-element[" + p_o_jsonSchemaElement.Name + "] has no mapping class");
                }
            }

            if (p_o_jsonSchemaElement.Type.ToLower().Equals("object"))
            {
                ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + "cast schema-object(" + p_o_jsonSchemaElement.Name + ")[parentName=" + s_jsonReferenceParentName + "] with schema-mapping(" + p_o_jsonSchemaElement.PrintMapping() + ") and p_o_object(" + p_o_object?.GetType().FullName + "), castonly=" + (p_o_jsonSchemaElement.MappingClass.Equals(p_o_object?.GetType().FullName)));

                Object? o_object = null;

                if (p_o_object != null)
                {
                    string s_mappingClass = p_o_jsonSchemaElement.MappingClass;

                    /* remove assembly part of mapping class type value */
                    if (s_mappingClass.Contains(','))
                    {
                        s_mappingClass = s_mappingClass.Substring(0, s_mappingClass.IndexOf(","));
                    }

                    /* cast object of p_o_object */
                    o_object = this.CastObject(p_o_jsonSchemaElement, p_o_object, s_mappingClass.Equals(p_o_object.GetType().FullName));
                }

                /* check if casted object is null */
                if (o_object == null)
                {
                    ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + "casted schema-object(" + p_o_jsonSchemaElement.Name + ")[parentName=" + s_jsonReferenceParentName + "] with schema-mapping(" + p_o_jsonSchemaElement.PrintMapping() + ") is null");

                    /* add object null value to json output */
                    if ((this.i_level == 0) || (p_b_parentIsArray))
                    {
                        s_json += this.PrintIndentation() + "null" + this.s_lineBreak;
                    }
                    else
                    {
                        /* check if a parent name for this reference is set */
                        if (!ForestNETLib.Core.Helper.IsStringEmpty(s_jsonReferenceParentName))
                        {
                            s_json += this.PrintIndentation() + "\"" + s_jsonReferenceParentName + "\": null," + this.s_lineBreak;
                        }
                        else
                        {
                            s_json += this.PrintIndentation() + "\"" + p_o_jsonSchemaElement.Name + "\": null," + this.s_lineBreak;
                        }
                    }
                }
                else
                {
                    /* add object start curved bracket to json output */
                    if ((this.i_level == 0) || (p_b_parentIsArray))
                    {
                        s_json += this.PrintIndentation() + "{" + this.s_lineBreak;
                    }
                    else
                    {
                        /* check if a parent name for this reference is set */
                        if (!ForestNETLib.Core.Helper.IsStringEmpty(s_jsonReferenceParentName))
                        {
                            s_json += this.PrintIndentation() + "\"" + s_jsonReferenceParentName + "\": {" + this.s_lineBreak;
                        }
                        else
                        {
                            s_json += this.PrintIndentation() + "\"" + p_o_jsonSchemaElement.Name + "\": {" + this.s_lineBreak;
                        }
                    }

                    /* increase level for PrintIndentation */
                    this.i_level++;

                    /* help variable to skip children iteration */
                    bool b_childrenIteration = true;

                    /* check conditions for handling object */
                    if (p_o_jsonSchemaElement.Reference != null)
                    {
                        /* check if reference has any children */
                        if (p_o_jsonSchemaElement.Reference.Children.Count < 1)
                        {
                            /* check if reference has another reference */
                            if (p_o_jsonSchemaElement.Reference.Reference == null)
                            {
                                throw new NullReferenceException("Reference[" + p_o_jsonSchemaElement.Reference.Name + "] of schema-element[" + p_o_jsonSchemaElement.Name + "] has no children and no other reference");
                            }
                            else
                            {
                                b_childrenIteration = false;

                                ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + "encode schema-object(" + p_o_jsonSchemaElement.Name + ") with its reference(" + p_o_jsonSchemaElement.Reference.Name + ") and schema-mapping(" + p_o_jsonSchemaElement.PrintMapping() + ") which has another reference with recursion");

                                /* handle reference with recursion */
                                s_json += this.JsonEncodeRecursive(p_o_jsonSchemaElement.Reference, o_object, false);
                            }
                        }
                        else
                        {
                            ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + "encode schema-object(" + p_o_jsonSchemaElement.Name + ") with its reference(" + p_o_jsonSchemaElement.Reference.Name + ") which has children");

                            /* set reference as current json element */
                            p_o_jsonSchemaElement = p_o_jsonSchemaElement.Reference;
                        }
                    }

                    /* execute children iteration */
                    if (b_childrenIteration)
                    {
                        /* check if object has any children */
                        if (p_o_jsonSchemaElement.Children.Count < 1)
                        {
                            throw new ArgumentException("Schema-element[" + p_o_jsonSchemaElement.Name + "] has no children");
                        }

                        /* iterate all children of current json element */
                        foreach (JSONElement o_jsonElement in p_o_jsonSchemaElement.Children)
                        {
                            ForestNETLib.Core.Global.ILogFinest(this.PrintIndentation() + "encode schema-object's(" + p_o_jsonSchemaElement.Name + ") child(" + o_jsonElement.Name + ") with schema-mapping(" + p_o_jsonSchemaElement.PrintMapping() + ") with recursion");

                            /* handle child with recursion */
                            s_json += this.JsonEncodeRecursive(o_jsonElement, o_object, false);
                        }
                    }

                    /* decrease level for PrintIndentation */
                    this.i_level--;

                    /* change last ',\r\n' to '\r\n' */
                    if (s_json.EndsWith("," + this.s_lineBreak))
                    {
                        s_json = s_json.Substring(0, s_json.Length - 1 - this.s_lineBreak.Length) + this.s_lineBreak;
                    }

                    /* add object end curved bracket to json output */
                    s_json += this.PrintIndentation() + "}," + this.s_lineBreak;
                }
            }
            else if (p_o_jsonSchemaElement.Type.ToLower().Equals("array"))
            {
                /* add property to json with starting array with opening bracket */
                if ((this.i_level == 0) || (p_b_parentIsArray))
                {
                    s_json += this.PrintIndentation() + "[" + this.s_lineBreak;
                }
                else
                {
                    /* check if a parent name for this reference is set */
                    if (!ForestNETLib.Core.Helper.IsStringEmpty(s_jsonReferenceParentName))
                    {
                        s_json += this.PrintIndentation() + "\"" + s_jsonReferenceParentName + "\": [" + this.s_lineBreak;
                    }
                    else
                    {
                        s_json += this.PrintIndentation() + "\"" + p_o_jsonSchemaElement.Name + "\": [" + this.s_lineBreak;
                    }
                }

                /* check conditions for handling array */
                if (p_o_jsonSchemaElement.Reference != null)
                {
                    if (p_o_jsonSchemaElement.Reference.Children.Count < 1)
                    {
                        throw new ArgumentException("Reference[" + p_o_jsonSchemaElement.Reference.Name + "] of schema-element[" + p_o_jsonSchemaElement.Name + "] with schema-type[" + p_o_jsonSchemaElement.Type + "] must have at least one child");
                    }

                    ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + "encode schema-array(" + p_o_jsonSchemaElement.Name + ")[parentName=" + s_jsonReferenceParentName + "] with its reference(" + p_o_jsonSchemaElement.Reference.Name + ") which has children");
                }
                else
                {
                    if (p_o_jsonSchemaElement.Children.Count != 1)
                    {
                        throw new ArgumentException("Schema-element[" + p_o_jsonSchemaElement.Name + "] with schema-type[" + p_o_jsonSchemaElement.Type + "] must have just one child");
                    }

                    if (!p_o_jsonSchemaElement.Children[0].Name.ToLower().Equals("items"))
                    {
                        throw new ArgumentException("Schema-element[" + p_o_jsonSchemaElement.Name + "] with schema-type[" + p_o_jsonSchemaElement.Name + "] must have one child with name[items]");
                    }

                    ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + "encode schema-array(" + p_o_jsonSchemaElement.Name + ")[parentName=" + s_jsonReferenceParentName + "] with child(items) which has children");
                }

                ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + "encode schema-array(" + p_o_jsonSchemaElement.Name + ") - and cast object with p_o_object(" + p_o_object?.GetType().FullName + ") and schema-mapping(" + p_o_jsonSchemaElement.PrintMapping() + "), castonly if List=" + ((p_o_object != null) && (p_o_object.GetType().IsGenericType) && (p_o_object.GetType().GetGenericTypeDefinition() == typeof(List<>))));

                /* get array data from object */
                Object? o_object = this.CastObject(p_o_jsonSchemaElement, p_o_object, ((p_o_object != null) && (p_o_object.GetType().IsGenericType) && (p_o_object.GetType().GetGenericTypeDefinition() == typeof(List<>))));

                /* check if we have a primitive array and current value is just empty/null */
                if ((o_object == null) && (p_o_jsonSchemaElement.Children[0].PrimitiveArray))
                {
                    s_json += this.s_lineBreak;
                }
                else if (o_object != null)
                {
                    /* cast current object as list with unknown generic type */
                    List<Object?> a_objects = [.. ((System.Collections.IEnumerable)o_object).Cast<Object?>()];

                    /* flag if we must handle generic list as primitive array */
                    bool b_primitiveArray = p_o_jsonSchemaElement.Children.Count > 0 && p_o_jsonSchemaElement.Children[0].PrimitiveArray;

                    /* check minItems and maxItems restrictions */
                    if (p_o_jsonSchemaElement.Restrictions.Count > 0)
                    {
                        foreach (JSONRestriction o_jsonRestriction in p_o_jsonSchemaElement.Restrictions)
                        {
                            if (o_jsonRestriction.Name.ToLower().Equals("minitems"))
                            {
                                /* check minItems restriction */
                                if (a_objects.Count < o_jsonRestriction.IntValue)
                                {
                                    throw new ArgumentException("Restriction error: not enough [" + p_o_jsonSchemaElement.Name + "] json items(" + a_objects.Count + "), minimum = " + o_jsonRestriction.IntValue);
                                }
                            }

                            if (o_jsonRestriction.Name.ToLower().Equals("maxitems"))
                            {
                                /* check maxItems restriction */
                                if (a_objects.Count > o_jsonRestriction.IntValue)
                                {
                                    throw new ArgumentException("Restriction error: too many [" + p_o_jsonSchemaElement.Name + "] json items(" + a_objects.Count + "), maximum = " + o_jsonRestriction.IntValue);
                                }
                            }
                        }
                    }

                    if (p_o_jsonSchemaElement.Reference != null)
                    {
                        /* set reference as current json element */
                        p_o_jsonSchemaElement = p_o_jsonSchemaElement.Reference;
                    }
                    else
                    {
                        /* set current json element to 'items' child */
                        p_o_jsonSchemaElement = p_o_jsonSchemaElement.Children[0];

                        /* if 'items' child has a child as well, we take this child as current json element */
                        if (p_o_jsonSchemaElement.Children.Count == 1)
                        {
                            p_o_jsonSchemaElement = p_o_jsonSchemaElement.Children[0];
                        }
                    }

                    /* iterate objects in list and encode data to json recursively */
                    for (int i = 0; i < a_objects.Count; i++)
                    {
                        /* increase level for PrintIndentation */
                        this.i_level++;

                        /* check if array object value is null */
                        if (a_objects[i] == null)
                        {
                            ForestNETLib.Core.Global.ILogFinest(this.PrintIndentation() + "encode schema-array(" + p_o_jsonSchemaElement.Name + ") with array element(null) and schema-mapping(" + p_o_jsonSchemaElement.PrintMapping() + ")");

                            s_json += this.PrintIndentation() + "null," + this.s_lineBreak;
                        }
                        else if (b_primitiveArray)
                        {
                            ForestNETLib.Core.Global.ILogFinest(this.PrintIndentation() + "encode schema-array(" + p_o_jsonSchemaElement.Name + ") with primitive array element(#" + (i + 1) + ") and schema-mapping(" + p_o_jsonSchemaElement.PrintMapping() + ")");

                            /* add primitive array element value to JSON */
                            s_json += this.PrintIndentation() + a_objects[i] + "," + this.s_lineBreak;
                        }
                        else
                        {
                            ForestNETLib.Core.Global.ILogFinest(this.PrintIndentation() + "encode schema-array(" + p_o_jsonSchemaElement.Name + ") with array element(" + a_objects[i]?.GetType().FullName + ") and schema-mapping(" + p_o_jsonSchemaElement.PrintMapping() + ") with recursion");

                            /* handle object with recursion */
                            s_json += this.JsonEncodeRecursive(p_o_jsonSchemaElement, a_objects[i], true);
                        }

                        /* decrease level for PrintIndentation */
                        this.i_level--;
                    }
                }

                /* change last "},\r\n" to "}\r\n" */
                if (s_json.EndsWith("}," + this.s_lineBreak))
                {
                    s_json = s_json.Substring(0, s_json.Length - 1 - this.s_lineBreak.Length) + this.s_lineBreak;
                }

                /* change last ',\r\n' to '\r\n' */
                if (s_json.EndsWith("," + this.s_lineBreak))
                {
                    s_json = s_json.Substring(0, s_json.Length - 1 - this.s_lineBreak.Length) + this.s_lineBreak;
                }

                /* end array with closing bracket */
                s_json += this.PrintIndentation() + "]," + this.s_lineBreak;

                /* recognize empty array result and replace it into two brackets */
                s_json = (new System.Text.RegularExpressions.Regex("\\[\" + this.s_lineBreak + \"(\\t)*\\]")).Replace(s_json, "\\[\\]");
            }
            else
            {
                /* set object variable with current object */
                Object? o_object = p_o_object;

                ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + "encode schema-property(" + p_o_jsonSchemaElement.Name + ") with p_o_object(" + p_o_object?.GetType().FullName + ")");

                /* get object property if we have not an array with items */
                if (!p_o_jsonSchemaElement.Name.ToLower().Equals("items"))
                {
                    ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + "encode schema-property(" + p_o_jsonSchemaElement.Name + "), cast object with p_o_object(" + p_o_object?.GetType().FullName + ")");

                    /* get object property of current json element */
                    o_object = this.CastObject(p_o_jsonSchemaElement, p_o_object);
                }

                ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + "encode schema-property(" + p_o_jsonSchemaElement.Name + "), cast string from object with p_o_object(" + p_o_object?.GetType().FullName + ") and schema-type(" + p_o_jsonSchemaElement.Type + ")");

                /* get string value of object for json-element */
                string s_foo = JSON.CastStringFromObject(o_object, p_o_jsonSchemaElement.Type);

                /* check if json-element is required */
                if (p_o_jsonSchemaElement.Required)
                {
                    /* check if value is empty */
                    if ((s_foo.Equals("")) || (s_foo.Equals("null")) || (s_foo.Equals("\"\"")))
                    {
                        throw new ArgumentException("'" + p_o_jsonSchemaElement.Name + "' is required, but value[" + s_foo + "] is empty");
                    }
                }

                /* check if json-element has any restrictions */
                if (p_o_jsonSchemaElement.Restrictions.Count > 0)
                {
                    foreach (JSONRestriction o_jsonRestriction in p_o_jsonSchemaElement.Restrictions)
                    {
                        /* execute restriction check */
                        JSON.CheckRestriction(s_foo, o_jsonRestriction, p_o_jsonSchemaElement);
                    }
                }

                /* add json-element with value */
                if (p_o_jsonSchemaElement.Name.ToLower().Equals("items"))
                {
                    /* array with items does not need captions */
                    s_json += this.PrintIndentation() + s_foo + "," + this.s_lineBreak;
                }
                else
                {
                    s_json += this.PrintIndentation() + "\"" + p_o_jsonSchemaElement.Name + "\": " + s_foo + "," + this.s_lineBreak;
                }
            }

            if (this.i_level == 0)
            {
                if (s_json.EndsWith("," + this.s_lineBreak))
                {
                    s_json = s_json.Substring(0, s_json.Length - 1 - this.s_lineBreak.Length) + this.s_lineBreak;
                }
            }

            return s_json;
        }

        /// <summary>
        /// Cast field of an object
        /// </summary>
        /// <param name="p_o_jsonElement">json element object	with mapping class information</param>
        /// <param name="p_o_object">object to access fields via direct public access or public access to property methods (getXX and setXX)</param>
        /// <returns>casted field value of object as object</returns>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        private Object? CastObject(JSONElement p_o_jsonElement, Object? p_o_object)
        {
            return this.CastObject(p_o_jsonElement, p_o_object, false);
        }

        /// <summary>
        /// Cast field of an object, optional cast only
        /// </summary>
        /// <param name="p_o_jsonElement">json element object	with mapping class information</param>
        /// <param name="p_o_object">object to access fields via direct public access or public access to property methods (getXX and setXX)</param>
        /// <param name="b_castOnly">true - just cast object with json element mapping class information, false - get object field value</param>
        /// <returns>casted field value of object as object</returns>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        private Object? CastObject(JSONElement p_o_jsonElement, Object? p_o_object, bool b_castOnly)
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
                    if (p_o_jsonElement.MappingClass.Equals(p_o_object.GetType().FullName))
                    {
                        o_object = Convert.ChangeType(p_o_object, p_o_object.GetType());
                    }
                    else if ((p_o_jsonElement.MappingClass.Contains(',')) && (p_o_jsonElement.MappingClass.Substring(0, p_o_jsonElement.MappingClass.IndexOf(",")).Equals(p_o_object.GetType().FullName))) /* cast current object by string mapping value, without assembly part */
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
                string s_field = p_o_jsonElement.MappingClass;

                /* if there is additional mapping information, use this for field property access */
                if (!ForestNETLib.Core.Helper.IsStringEmpty(p_o_jsonElement.Mapping))
                {
                    s_field = p_o_jsonElement.Mapping;
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
                if ((o_object != null) && (p_o_jsonElement.Children.Count > 0) && (p_o_jsonElement.Children[0].PrimitiveArray))
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
                                a_primtiveArray.Add(JSON.CastStringFromObject(a_objects[i], p_o_jsonElement.Children[0].Type));
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
                                a_primtiveArray.Add(JSON.CastStringFromObject(a_objects[i], p_o_jsonElement.Children[0].Type));
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
                                a_primtiveArray.Add(JSON.CastStringFromObject(a_objects[i], p_o_jsonElement.Children[0].Type));
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
                                a_primtiveArray.Add(JSON.CastStringFromObject(a_objects[i], p_o_jsonElement.Children[0].Type));
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
                                a_primtiveArray.Add(JSON.CastStringFromObject(a_objects[i], p_o_jsonElement.Children[0].Type));
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
                                a_primtiveArray.Add(JSON.CastStringFromObject(a_objects[i], p_o_jsonElement.Children[0].Type));
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
                                a_primtiveArray.Add(JSON.CastStringFromObject(a_objects[i], p_o_jsonElement.Children[0].Type));
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
                                a_primtiveArray.Add(JSON.CastStringFromObject(a_objects[i], p_o_jsonElement.Children[0].Type));
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
                                a_primtiveArray.Add(JSON.CastStringFromObject(a_objects[i], p_o_jsonElement.Children[0].Type));
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
                                a_primtiveArray.Add(JSON.CastStringFromObject(a_objects[i], p_o_jsonElement.Children[0].Type));
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
                                a_primtiveArray.Add(JSON.CastStringFromObject(a_objects[i], p_o_jsonElement.Children[0].Type));
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
                                a_primtiveArray.Add(JSON.CastStringFromObject(a_objects[i], p_o_jsonElement.Children[0].Type));
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
                                a_primtiveArray.Add(JSON.CastStringFromObject(a_objects[i], p_o_jsonElement.Children[0].Type));
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
                                a_primtiveArray.Add(JSON.CastStringFromObject(a_objects[i], p_o_jsonElement.Children[0].Type));
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
                                a_primtiveArray.Add(JSON.CastStringFromObject(a_objects[i], p_o_jsonElement.Children[0].Type));
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
                                a_primtiveArray.Add(JSON.CastStringFromObject(a_objects[i], p_o_jsonElement.Children[0].Type));
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
                                    a_primtiveArray.Add(JSON.CastStringFromObject(a_objects[i], p_o_jsonElement.Children[0].Type));
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
        /// Method to cast an object value to a string value for encoding json data
        /// </summary>
        /// <param name="p_o_object">object value which will be casted to string</param>
        /// <param name="p_s_type">type as string to distinguish</param>
        /// <returns>casted object value as string</returns>
        /// <exception cref="ArgumentException">invalid or empty type value</exception>
        private static string CastStringFromObject(Object? p_o_object, string? p_s_type)
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
                            s_foo = ForestNETLib.Core.Helper.ToISO8601UTC(o_foo);
                        }
                        else if (p_o_object.GetType() == typeof(TimeSpan))
                        {
                            DateTime o_foo = new DateTime(1900, 1, 1).Date + (TimeSpan)p_o_object;
                            s_foo = ForestNETLib.Core.Helper.ToISO8601UTC(o_foo);
                        }
                        else
                        {
                            s_foo = p_o_object.ToString() ?? string.Empty;
                        }

                        s_foo = "\"" + s_foo.Replace("\"", "\\\"") + "\"";
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
                        s_foo = o_foo.ToString().ToLower();
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
        /// Check if json element restriction is valid with current value
        /// </summary>
        /// <param name="p_s_value">string value for json element restriction, can be casted to integer as well</param>
        /// <param name="p_o_jsonRestriction">json restriction object which holds all restriction information</param>
        /// <param name="p_o_jsonElement">json element object</param>
        /// <exception cref="ArgumentException">unknown restriction name, restriction error or invalid type from json element object</exception>
        private static void CheckRestriction(string? p_s_value, JSONRestriction p_o_jsonRestriction, JSONElement p_o_jsonElement)
        {
            /* check if parameter value is null */
            if (p_s_value == null)
            {
                throw new ArgumentException("Restriction error: value[null] is null for " + p_o_jsonRestriction.Name + " restriction[" + p_o_jsonRestriction.StrValue + "]");
            }

            /* check if type of yaml parameter is null */
            if (p_o_jsonElement.Type == null)
            {
                throw new ArgumentException("Restriction error: value[" + p_s_value + "] for " + p_o_jsonRestriction.Name + " restriction[" + p_o_jsonRestriction.StrValue + "] cannot be used, because json element parameter type property is 'null'");
            }

            string p_s_type = p_o_jsonElement.Type.ToLower();

            /* remove surrounding '"' and '"' characters */
            if ((p_s_value.StartsWith("\"")) && (p_s_value.EndsWith("\"")))
            {
                p_s_value = p_s_value.Substring(1, p_s_value.Length - 1 - 1);
            }

            if (p_o_jsonRestriction.Name.ToLower().Equals("minimum"))
            {
                if (p_s_type.Equals("number"))
                {
                    decimal o_value = Convert.ToDecimal(p_s_value, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    decimal o_restriction = Convert.ToDecimal(p_o_jsonRestriction.StrValue, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    int i_compare = o_value.CompareTo(o_restriction);

                    if (i_compare == -1)
                    {
                        throw new ArgumentException("Restriction error: value[" + o_value + "] does not match " + p_o_jsonRestriction.Name + " restriction[" + p_o_jsonRestriction.StrValue + "]");
                    }
                }
                else if (p_s_type.Equals("integer"))
                {
                    int i_value = int.Parse(p_s_value);
                    int i_restriction = p_o_jsonRestriction.IntValue;
                    int i_compare = i_value.CompareTo(i_restriction);

                    if (i_compare == -1)
                    {
                        throw new ArgumentException("Restriction error: value[" + i_value + "] does not match " + p_o_jsonRestriction.Name + " restriction[" + p_o_jsonRestriction.IntValue + "]");
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid type[" + p_s_type + "] of '" + p_o_jsonElement.Name + "' using restriction[" + p_o_jsonRestriction.Name + "]");
                }
            }
            else if (p_o_jsonRestriction.Name.ToLower().Equals("exclusiveminimum"))
            {
                if (p_s_type.Equals("number"))
                {
                    decimal o_value = Convert.ToDecimal(p_s_value, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    decimal o_restriction = Convert.ToDecimal(p_o_jsonRestriction.StrValue, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    int i_compare = o_value.CompareTo(o_restriction);

                    if ((i_compare == -1) || (i_compare == 0))
                    {
                        throw new ArgumentException("Restriction error: value[" + o_value + "] does not match " + p_o_jsonRestriction.Name + " restriction[" + p_o_jsonRestriction.StrValue + "]");
                    }
                }
                else if (p_s_type.Equals("integer"))
                {
                    int i_value = int.Parse(p_s_value);
                    int i_restriction = p_o_jsonRestriction.IntValue;
                    int i_compare = i_value.CompareTo(i_restriction);

                    if ((i_compare == -1) || (i_compare == 0))
                    {
                        throw new ArgumentException("Restriction error: value[" + i_value + "] does not match " + p_o_jsonRestriction.Name + " restriction[" + p_o_jsonRestriction.IntValue + "]");
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid type[" + p_s_type + "] of '" + p_o_jsonElement.Name + "' using restriction[" + p_o_jsonRestriction.Name + "]");
                }
            }
            else if (p_o_jsonRestriction.Name.ToLower().Equals("maximum"))
            {
                if (p_s_type.Equals("number"))
                {
                    decimal o_value = Convert.ToDecimal(p_s_value, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    decimal o_restriction = Convert.ToDecimal(p_o_jsonRestriction.StrValue, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    int i_compare = o_value.CompareTo(o_restriction);

                    if (i_compare == 1)
                    {
                        throw new ArgumentException("Restriction error: value[" + o_value + "] does not match " + p_o_jsonRestriction.Name + " restriction[" + p_o_jsonRestriction.StrValue + "]");
                    }
                }
                else if (p_s_type.Equals("integer"))
                {
                    int i_value = int.Parse(p_s_value);
                    int i_restriction = p_o_jsonRestriction.IntValue;
                    int i_compare = i_value.CompareTo(i_restriction);

                    if (i_compare == 1)
                    {
                        throw new ArgumentException("Restriction error: value[" + i_value + "] does not match " + p_o_jsonRestriction.Name + " restriction[" + p_o_jsonRestriction.IntValue + "]");
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid type[" + p_s_type + "] of '" + p_o_jsonElement.Name + "' using restriction[" + p_o_jsonRestriction.Name + "]");
                }
            }
            else if (p_o_jsonRestriction.Name.ToLower().Equals("exclusivemaximum"))
            {
                if (p_s_type.Equals("number"))
                {
                    decimal o_value = Convert.ToDecimal(p_s_value, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    decimal o_restriction = Convert.ToDecimal(p_o_jsonRestriction.StrValue, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                    int i_compare = o_value.CompareTo(o_restriction);

                    if ((i_compare == 1) || (i_compare == 0))
                    {
                        throw new ArgumentException("Restriction error: value[" + o_value + "] does not match " + p_o_jsonRestriction.Name + " restriction[" + p_o_jsonRestriction.StrValue + "]");
                    }
                }
                else if (p_s_type.Equals("integer"))
                {
                    int i_value = int.Parse(p_s_value);
                    int i_restriction = p_o_jsonRestriction.IntValue;
                    int i_compare = i_value.CompareTo(i_restriction);

                    if ((i_compare == 1) || (i_compare == 0))
                    {
                        throw new ArgumentException("Restriction error: value[" + i_value + "] does not match " + p_o_jsonRestriction.Name + " restriction[" + p_o_jsonRestriction.IntValue + "]");
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid type[" + p_s_type + "] of '" + p_o_jsonElement.Name + "' using restriction[" + p_o_jsonRestriction.Name + "]");
                }
            }
            else if (p_o_jsonRestriction.Name.ToLower().Equals("minlength"))
            {
                if (p_s_type.Equals("string"))
                {
                    if (p_s_value.Length < p_o_jsonRestriction.IntValue)
                    {
                        throw new ArgumentException("Restriction error: value[" + p_s_value + "] does not match " + p_o_jsonRestriction.Name + " restriction[" + p_o_jsonRestriction.IntValue + "]");
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid type[" + p_s_type + "] of '" + p_o_jsonElement.Name + "' using restriction[" + p_o_jsonRestriction.Name + "]");
                }
            }
            else if (p_o_jsonRestriction.Name.ToLower().Equals("maxlength"))
            {
                if (p_s_type.Equals("string"))
                {
                    if (p_s_value.Length > p_o_jsonRestriction.IntValue)
                    {
                        throw new ArgumentException("Restriction error: value[" + p_s_value + "] does not match " + p_o_jsonRestriction.Name + " restriction[" + p_o_jsonRestriction.IntValue + "]");
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid type[" + p_s_type + "] of '" + p_o_jsonElement.Name + "' using restriction[" + p_o_jsonRestriction.Name + "]");
                }
            }
            else if (p_o_jsonRestriction.Name.ToLower().Equals("pattern"))
            {
                if ((p_s_type.Equals("string")) || (p_s_type.Equals("boolean")) || (p_s_type.Equals("number")) || (p_s_type.Equals("integer")))
                {
                    if (!ForestNETLib.Core.Helper.MatchesRegex(p_s_value, p_o_jsonRestriction.StrValue))
                    {
                        throw new ArgumentException("Restriction error: value[" + p_s_value + "] does not match " + p_o_jsonRestriction.Name + " restriction[" + p_o_jsonRestriction.StrValue + "]");
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid type[" + p_s_type + "] of '" + p_o_jsonElement.Name + "' using restriction[" + p_o_jsonRestriction.Name + "]");
                }
            }
            else
            {
                throw new ArgumentException("Unknown Restriction: " + p_o_jsonRestriction.Name);
            }
        }

        /* validating json data with JSON schema */

        /// <summary>
        /// Validate json file
        /// </summary>
        /// <param name="p_s_jsonFile">full-path to json file</param>
        /// <returns>true - content of json file is valid, false - content of json file is invalid</returns>
        /// <exception cref="ArgumentException">json file does not exist</exception>
        /// <exception cref="System.IO.IOException">cannot read json file content</exception>
        /// <exception cref="NullReferenceException">empty schema, empty json file or root node after parsing json content</exception>
        public bool ValidateAgainstSchema(string p_s_jsonFile)
        {
            /* check schema field */
            if (this.o_schema == null)
            {
                throw new NullReferenceException("Cannot decode data. Schema is null.");
            }

            /* check if file exists */
            if (!ForestNETLib.IO.File.Exists(p_s_jsonFile))
            {
                throw new ArgumentException("JSON file[" + p_s_jsonFile + "] does not exist.");
            }

            /* open json file */
            ForestNETLib.IO.File o_file = new(p_s_jsonFile, false);

            /* load file content into string */
            string s_fileContent = o_file.FileContent;

            List<string> a_fileLines =
            [
                /* load file content lines into array */
                .. s_fileContent.Split(this.s_lineBreak),
            ];

            ForestNETLib.Core.Global.ILogFinest("read all lines from json file '" + p_s_jsonFile + "'");

            /* decode json file lines */
            return this.ValidateAgainstSchema(a_fileLines);
        }

        /// <summary>
        /// Validate json content
        /// </summary>
        /// <param name="p_a_jsonLines">json lines</param>
        /// <returns>true - json content is valid, false - json content is invalid</returns>
        /// <exception cref="NullReferenceException">empty schema, empty json file or root node after parsing json content</exception>
        /// <exception cref="ArgumentException">condition failed for decoding json correctly</exception>
        public bool ValidateAgainstSchema(List<string> p_a_jsonLines)
        {
            /* check schema field */
            if (this.o_schema == null)
            {
                throw new NullReferenceException("Cannot decode data. Schema is null.");
            }

            ForestNETLib.Core.Global.ILogFiner("read all lines: '" + p_a_jsonLines.Count + "'");

            System.Text.StringBuilder o_stringBuilder = new();

            /* read all json schema file lines to one string builder */
            foreach (string s_line in p_a_jsonLines)
            {
                o_stringBuilder.Append(s_line);
            }

            /* read all json-schema file lines and delete all line-wraps and tabs */
            string s_json = o_stringBuilder.ToString();
            s_json = new System.Text.RegularExpressions.Regex("[\\r\\n\\t]").Replace(s_json, "");

            /* remove all white spaces, but not between double quotes */
            s_json = JSON.RemoveWhiteSpaces(s_json);

            /* check if json-schema starts with (curly) brackets */
            if ((((!s_json.StartsWith("{")) || (!s_json.EndsWith("}"))) && ((!s_json.StartsWith("[")) || (!s_json.EndsWith("]")))))
            {
                throw new ArgumentException("JSON-file must start and end with curly bracket '{', '}' or must start and end with bracket '[', ']'");
            }

            /* validate json schema */
            JSON.ValidateJSON(s_json);

            ForestNETLib.Core.Global.ILogFinest("validated json content lines");

            this.Root = null;
            this.o_currentElement = null;

            /* parse json */
            this.ParseJSON(s_json);

            ForestNETLib.Core.Global.ILogFinest("parsed json content lines");

            /* check if root is null */
            if (this.Root == null)
            {
                throw new NullReferenceException("Root node is null");
            }

            if (ForestNETLib.Core.Global.IsILevel((byte)ForestNETLib.Log.Level.MASS)) ForestNETLib.Core.Global.ILogMass("JSON-Schema:" + ForestNETLib.IO.File.NEWLINE + this.o_schema);
            if (ForestNETLib.Core.Global.IsILevel((byte)ForestNETLib.Log.Level.MASS)) ForestNETLib.Core.Global.ILogMass("JSON-Root-Element:" + ForestNETLib.IO.File.NEWLINE + this.Root);

            /* validate json recursively */
            return this.ValidateAgainstSchemaRecursive(this.Root, this.o_schema);
        }

        /// <summary>
        /// Recursive method to validate json content string
        /// </summary>
        /// <param name="p_o_jsonDataElement">current json data element</param>
        /// <param name="p_o_jsonSchemaElement">current json schema element with additional information for decoding</param>
        /// <returns>true - json content is valid, false - json content is invalid</returns>
        /// <exception cref="ArgumentNullException">value in schema or expected element is not available</exception>
        /// <exception cref="ArgumentException">condition failed for decoding json correctly</exception>
        private bool ValidateAgainstSchemaRecursive(JSONElement p_o_jsonDataElement, JSONElement p_o_jsonSchemaElement)
        {
            bool b_return = true;

            /* if type and mapping class are not set, we need at least a reference to continue */
            if ((ForestNETLib.Core.Helper.IsStringEmpty(p_o_jsonSchemaElement.Type)) && (ForestNETLib.Core.Helper.IsStringEmpty(p_o_jsonSchemaElement.MappingClass)))
            {
                if (p_o_jsonSchemaElement.Reference == null)
                {
                    throw new NullReferenceException("Schema-element[" + p_o_jsonSchemaElement.Name + "] has no type, no mapping class and no reference");
                }
                else
                {
                    /* set reference as current schema-element */
                    p_o_jsonSchemaElement = p_o_jsonSchemaElement.Reference;
                }
            }

            /* check if type is set */
            if ((p_o_jsonSchemaElement.Type == null) || (ForestNETLib.Core.Helper.IsStringEmpty(p_o_jsonSchemaElement.Type)))
            {
                throw new ArgumentException("Schema-element[" + p_o_jsonSchemaElement.Name + "] has no type");
            }

            /* check if mapping class is set if schema-element is not 'items' */
            if (ForestNETLib.Core.Helper.IsStringEmpty(p_o_jsonSchemaElement.MappingClass))
            {
                if (!p_o_jsonSchemaElement.Name.ToLower().Equals("items"))
                {
                    throw new ArgumentException("Schema-element[" + p_o_jsonSchemaElement.Name + "] has no mapping class");
                }
            }

            if (p_o_jsonSchemaElement.Type.ToLower().Equals("object"))
            {
                /* check if we have any data for new object */
                if (p_o_jsonDataElement.Children.Count > 0)
                {
                    string s_objectType = p_o_jsonSchemaElement.MappingClass;

                    /* if object has reference, we create new object instance by mapping of reference */
                    if ((p_o_jsonSchemaElement.Reference != null) && (p_o_jsonSchemaElement.Reference.Type != null) && (p_o_jsonSchemaElement.Reference.Type.ToLower().Equals("object")))
                    {
                        s_objectType = p_o_jsonSchemaElement.Reference.MappingClass;
                    }

                    ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + p_o_jsonSchemaElement.Name + ": create new schema-object instance with mapping[" + p_o_jsonSchemaElement.PrintMapping() + "] and type[" + s_objectType + "]");

                    /* increase level for PrintIndentation */
                    this.i_level++;

                    /* help variable to skip children iteration */
                    bool b_childrenIteration = true;

                    /* check conditions for handling object */
                    if (p_o_jsonSchemaElement.Reference != null)
                    {
                        /* check if reference has any children */
                        if (p_o_jsonSchemaElement.Reference.Children.Count < 1)
                        {
                            /* check if reference has another reference */
                            if (p_o_jsonSchemaElement.Reference.Reference == null)
                            {
                                throw new NullReferenceException("Reference[" + p_o_jsonSchemaElement.Reference.Name + "] of schema-element[" + p_o_jsonSchemaElement.Name + "] has no children and no other reference");
                            }
                            else
                            {
                                b_childrenIteration = false;

                                /* check if current element in schema has data element by name, otherwise skip this element */
                                if (p_o_jsonSchemaElement.Name.Equals(p_o_jsonDataElement.Name))
                                {
                                    ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + p_o_jsonSchemaElement.Name + ": get schema-object[" + p_o_jsonSchemaElement.MappingClass + "] with reference[" + p_o_jsonSchemaElement.Reference.Name + "]");

                                    /* only create new object if we have one object data */
                                    if (p_o_jsonDataElement.Children.Count != 1)
                                    {
                                        throw new ArgumentException("We have (" + p_o_jsonDataElement.Children.Count + ") no data children or more than one for schema-element[" + p_o_jsonSchemaElement.Name + "]");
                                    }

                                    /* handle reference with recursion */
                                    b_return = this.ValidateAgainstSchemaRecursive(p_o_jsonDataElement.Children[0], p_o_jsonSchemaElement.Reference);
                                }
                            }
                        }
                        else
                        {
                            ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + "update current schema-element[" + p_o_jsonSchemaElement.Name + "](" + p_o_jsonSchemaElement.PrintMapping() + ") with reference[" + p_o_jsonSchemaElement.Reference.Name + "](" + p_o_jsonSchemaElement.Reference.PrintMapping() + ")");

                            /* set reference as current json element */
                            p_o_jsonSchemaElement = p_o_jsonSchemaElement.Reference;
                        }
                    }

                    /* execute children iteration */
                    if (b_childrenIteration)
                    {
                        ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + p_o_jsonSchemaElement.Name + ": get schema-object with type[" + p_o_jsonSchemaElement.MappingClass + "] and children[" + p_o_jsonSchemaElement.Children.Count + "]");

                        /* only create new object if we have one child definition for object in json schema */
                        if (p_o_jsonSchemaElement.Children.Count < 1)
                        {
                            throw new ArgumentException("Schema-element[" + p_o_jsonSchemaElement.Name + "] has no children");
                        }

                        /* check if new object is required if there is no data */
                        if ((p_o_jsonDataElement.Children.Count < 1) && (p_o_jsonSchemaElement.Required))
                        {
                            throw new ArgumentException("We have no data children for schema-element[" + p_o_jsonSchemaElement.Name + "] which is required");
                        }

                        /* only iterate if we have object data */
                        if (p_o_jsonDataElement.Children.Count > 0)
                        {
                            /* increase level for PrintIndentation */
                            this.i_level++;

                            /* data pointer */
                            int j = 0;

                            for (int i = 0; i < p_o_jsonSchemaElement.Children.Count; i++)
                            {
                                ForestNETLib.Core.Global.ILogFinest(this.PrintIndentation() + "compare schema-child-name(" + p_o_jsonSchemaElement.Children[i].Name + ") with data-child-name(" + p_o_jsonDataElement.Children[j].Name + ")");

                                /* check if current element in schema has data element by name, otherwise skip this element */
                                if (!p_o_jsonSchemaElement.Children[i].Name.Equals(p_o_jsonDataElement.Children[j].Name))
                                {
                                    /* but check if current schema element is required, so we must throw an exception */
                                    if (p_o_jsonSchemaElement.Children[i].Required)
                                    {
                                        throw new ArgumentException("We have no data for schema-element[" + p_o_jsonSchemaElement.Children[i].Name + "] which is required");
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }

                                /* handle new object with recursion */
                                b_return = this.ValidateAgainstSchemaRecursive(p_o_jsonDataElement.Children[j], p_o_jsonSchemaElement.Children[i]);

                                /* increase data pointer */
                                j++;
                            }

                            /* decrease level for PrintIndentation */
                            this.i_level--;
                        }
                    }

                    /* decrease level for PrintIndentation */
                    this.i_level--;
                }
            }
            else if (p_o_jsonSchemaElement.Type.ToLower().Equals("array"))
            {
                /* check conditions for handling array */
                if (p_o_jsonSchemaElement.Reference != null)
                {
                    if (p_o_jsonSchemaElement.Reference.Children.Count < 1)
                    {
                        throw new ArgumentException("Reference[" + p_o_jsonSchemaElement.Reference.Name + "] of schema-array[" + p_o_jsonSchemaElement.Name + "] with mapping[" + p_o_jsonSchemaElement.MappingClass + "] must have at least one child");
                    }
                }
                else
                {
                    if (p_o_jsonSchemaElement.Children.Count != 1)
                    {
                        throw new ArgumentException("Schema-array[" + p_o_jsonSchemaElement.Name + "] with mapping[" + p_o_jsonSchemaElement.MappingClass + "] must have just one child");
                    }

                    if (!p_o_jsonSchemaElement.Children[0].Name.ToLower().Equals("items"))
                    {
                        throw new ArgumentException("Schema-array[" + p_o_jsonSchemaElement.Name + "] with mapping[" + p_o_jsonSchemaElement.MappingClass + "] must have one child with name[items]");
                    }
                }

                /* help variables to handle array */
                bool b_required = false;
                string s_requiredProperty = "";
                List<JSONRestriction> a_restrictions = [];
                string s_amountProperty = "";

                /* check if json-element is required */
                if (p_o_jsonSchemaElement.Required)
                {
                    b_required = true;
                    s_requiredProperty = p_o_jsonSchemaElement.Name;
                }

                /* check minItems and maxItems restrictions and save them for items check afterwards */
                if (p_o_jsonSchemaElement.Restrictions.Count > 0)
                {
                    foreach (JSONRestriction o_jsonRestriction in p_o_jsonSchemaElement.Restrictions)
                    {
                        if ((o_jsonRestriction.Name.ToLower().Equals("minitems")) || (o_jsonRestriction.Name.ToLower().Equals("maxitems")))
                        {
                            a_restrictions.Add(o_jsonRestriction);
                            s_amountProperty = p_o_jsonSchemaElement.Name;
                        }
                    }
                }

                if (p_o_jsonSchemaElement.Reference != null)
                {
                    /* set reference as current json element */
                    p_o_jsonSchemaElement = p_o_jsonSchemaElement.Reference;
                }
                else
                {
                    /* set current json element to 'items' child */
                    p_o_jsonSchemaElement = p_o_jsonSchemaElement.Children[0];

                    /* if 'items' child has a child as well, we take this child as current json element */
                    if (p_o_jsonSchemaElement.Children.Count == 1)
                    {
                        p_o_jsonSchemaElement = p_o_jsonSchemaElement.Children[0];
                    }
                }

                if (ForestNETLib.Core.Helper.IsStringEmpty(p_o_jsonDataElement.Value))
                { /* we have multiple minor objects for current array */
                    ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + p_o_jsonSchemaElement.Name + ": get schema-array with mapping[" + p_o_jsonSchemaElement.PrintMapping() + "]");

                    if (p_o_jsonDataElement.Children.Count > 0)
                    { /* if we have objects to the new array */
                        /* check minItems and maxItems restrictions */
                        if (a_restrictions.Count > 0)
                        {
                            foreach (JSONRestriction o_jsonRestriction in a_restrictions)
                            {
                                if (o_jsonRestriction.Name.ToLower().Equals("minitems"))
                                {
                                    /* check minItems restriction */
                                    if (p_o_jsonDataElement.Children.Count < o_jsonRestriction.IntValue)
                                    {
                                        throw new ArgumentException("Restriction error: not enough [" + p_o_jsonSchemaElement.Name + " of " + s_amountProperty + "] json items(" + p_o_jsonDataElement.Children.Count + "), minimum = " + o_jsonRestriction.IntValue);
                                    }
                                }

                                if (o_jsonRestriction.Name.ToLower().Equals("maxitems"))
                                {
                                    /* check maxItems restriction */
                                    if (p_o_jsonDataElement.Children.Count > o_jsonRestriction.IntValue)
                                    {
                                        throw new ArgumentException("Restriction error: too many [" + p_o_jsonSchemaElement.Name + " of " + s_amountProperty + "] json items(" + p_o_jsonDataElement.Children.Count + "), maximum = " + o_jsonRestriction.IntValue);
                                    }
                                }
                            }
                        }

                        /* iterate objects in list and encode data to json recursively */
                        for (int i = 0; i < p_o_jsonDataElement.Children.Count; i++)
                        {
                            /* increase level for PrintIndentation */
                            this.i_level++;

                            /* handle array object with recursion */
                            b_return = this.ValidateAgainstSchemaRecursive(p_o_jsonDataElement.Children[i], p_o_jsonSchemaElement);

                            /* decrease level for PrintIndentation */
                            this.i_level--;
                        }
                    }
                }
                else
                { /* array objects must be retrieved out of value property */
                    ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + p_o_jsonSchemaElement.Name + ": handle array value = " + p_o_jsonDataElement.Value + " - mapping[" + p_o_jsonSchemaElement.PrintMapping() + "]");

                    /* set array with values if we have any values */
                    if ((p_o_jsonDataElement.Value != null) && (!ForestNETLib.Core.Helper.IsStringEmpty(p_o_jsonDataElement.Value)) && (!p_o_jsonDataElement.Value.Equals("[]")))
                    {
                        /* remove opening and closing brackets */
                        if ((p_o_jsonDataElement.Value.StartsWith("[")) && (p_o_jsonDataElement.Value.EndsWith("]")))
                        {
                            p_o_jsonDataElement.Value = p_o_jsonDataElement.Value.Substring(1, p_o_jsonDataElement.Value.Length - 1 - 1);
                        }

                        /* split array into values, divided by ',' */
                        string[] a_values = p_o_jsonDataElement.Value.Split(",");

                        /* check minItems and maxItems restrictions */
                        if (a_restrictions.Count > 0)
                        {
                            foreach (JSONRestriction o_jsonRestriction in a_restrictions)
                            {
                                if (o_jsonRestriction.Name.ToLower().Equals("minitems"))
                                {
                                    /* check minItems restriction */
                                    if (a_values.Length < o_jsonRestriction.IntValue)
                                    {
                                        throw new ArgumentException("Restriction error: not enough [" + p_o_jsonSchemaElement.Name + " of " + s_amountProperty + "] json items(" + a_values.Length + "), minimum = " + o_jsonRestriction.IntValue);
                                    }
                                }

                                if (o_jsonRestriction.Name.ToLower().Equals("maxitems"))
                                {
                                    /* check maxItems restriction */
                                    if (a_values.Length > o_jsonRestriction.IntValue)
                                    {
                                        throw new ArgumentException("Restriction error: too many [" + p_o_jsonSchemaElement.Name + " of " + s_amountProperty + "] json items(" + a_values.Length + "), maximum = " + o_jsonRestriction.IntValue);
                                    }
                                }
                            }
                        }

                        /* iterate all array values */
                        foreach (string s_foo in a_values)
                        {
                            string s_value = s_foo;

                            JSONValueType e_jsonValueType = JSON.GetJSONValueType(s_value);

                            /* check if JSON value types are matching between schema and data */
                            if ((e_jsonValueType != JSON.StringToJSONValueType(p_o_jsonSchemaElement.Type)) && (e_jsonValueType != JSONValueType.Null))
                            {
                                throw new ArgumentException("JSON schema type[" + StringToJSONValueType(p_o_jsonSchemaElement.Type) + "] does not match with data value type[" + e_jsonValueType + "] with value[" + s_value + "]");
                            }

                            /* check if json-element has any restrictions */
                            if (p_o_jsonSchemaElement.Restrictions.Count > 0)
                            {
                                foreach (JSONRestriction o_jsonRestriction in p_o_jsonSchemaElement.Restrictions)
                                {
                                    /* execute restriction check */
                                    JSON.CheckRestriction(s_value, o_jsonRestriction, p_o_jsonSchemaElement);
                                }
                            }

                            /* cast array string value into object */
                            Object? o_returnObject = JSON.CastObjectFromString(s_value, p_o_jsonSchemaElement.Type);

                            ForestNETLib.Core.Global.ILogFinest(this.PrintIndentation() + "add value[" + (o_returnObject ?? "null") + "] to list of " + p_o_jsonDataElement.Name + ", type[" + ((o_returnObject == null) ? "null" : o_returnObject.GetType().FullName) + "]");
                        }
                    }
                    else if (b_required)
                    { /* if json-element with type array is required, throw exception */
                        throw new ArgumentNullException("'" + p_o_jsonSchemaElement.Name + "' of '" + s_requiredProperty + "' is required, but array has no values");
                    }
                }
            }
            else
            {
                ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + p_o_jsonSchemaElement.Name + ": handle value = " + p_o_jsonDataElement.Value + " - mapping[" + p_o_jsonSchemaElement.PrintMapping() + "]");

                JSONValueType e_jsonValueType = JSON.GetJSONValueType(p_o_jsonDataElement.Value);

                ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + p_o_jsonSchemaElement.Name + ": json value type = " + e_jsonValueType);

                /* check if JSON value types are matching between schema and data, additional condition = JSON value type is not 'null' */
                if ((e_jsonValueType != JSON.StringToJSONValueType(p_o_jsonSchemaElement.Type)) && (e_jsonValueType != JSONValueType.Null))
                {
                    /* it is acceptable if source type is 'integer' and destination type is 'number', valid cast available */
                    if (!((e_jsonValueType == JSONValueType.Integer) && (JSON.StringToJSONValueType(p_o_jsonSchemaElement.Type) == JSONValueType.Number)))
                    {
                        throw new ArgumentException("JSON schema type[" + e_jsonValueType + "] does not match with data value type[" + JSON.StringToJSONValueType(p_o_jsonSchemaElement.Type) + "] with value[" + p_o_jsonDataElement.Value + "]");
                    }
                }

                /* check if json-element is required */
                if (p_o_jsonSchemaElement.Required)
                {
                    /* check if value is empty */
                    if ((p_o_jsonDataElement.Value == null) || (p_o_jsonDataElement.Value.Equals("")) || (p_o_jsonDataElement.Value.Equals("null")) || (p_o_jsonDataElement.Value.Equals("\"\"")))
                    {
                        throw new ArgumentException("'" + p_o_jsonSchemaElement.Name + "' is required, but value[" + p_o_jsonDataElement.Value + "] is empty");
                    }
                }

                /* check if json-element has any restrictions */
                if (p_o_jsonSchemaElement.Restrictions.Count > 0)
                {
                    foreach (JSONRestriction o_jsonRestriction in p_o_jsonSchemaElement.Restrictions)
                    {
                        /* execute restriction check */
                        JSON.CheckRestriction(p_o_jsonDataElement.Value, o_jsonRestriction, p_o_jsonSchemaElement);
                    }
                }

                if (!(p_o_jsonDataElement.Value ?? "").Equals("null"))
                {
                    if ((ForestNETLib.Core.Helper.IsStringEmpty(p_o_jsonSchemaElement.Mapping)) && (ForestNETLib.Core.Helper.IsStringEmpty(p_o_jsonSchemaElement.MappingClass)))
                    {
                        _ = JSON.CastObjectFromString(p_o_jsonDataElement.Value, p_o_jsonSchemaElement.Type);
                    }
                    else
                    {
                        string? s_objectValue = p_o_jsonDataElement.Value;

                        /* remove surrounded double quotes from value */
                        if ((s_objectValue != null) && (s_objectValue.StartsWith("\"")) && (s_objectValue.EndsWith("\"")))
                        {
                            s_objectValue = s_objectValue.Substring(1, s_objectValue.Length - 1 - 1);
                        }

                        try
                        {
                            _ = JSON.CastObjectFromString(s_objectValue, p_o_jsonSchemaElement.Type);
                        }
                        catch (Exception)
                        {
                            try
                            {
                                _ = JSON.CastObjectFromString(s_objectValue, p_o_jsonSchemaElement.Mapping);
                            }
                            catch (Exception)
                            {
                                _ = JSON.CastObjectFromString(s_objectValue, p_o_jsonSchemaElement.MappingClass);
                            }
                        }
                    }
                }
            }

            ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + "return " + b_return);

            return b_return;
        }

        /* decoding JSON to data with JSON schema */

        /// <summary>
        /// Decode json file to an c# object
        /// </summary>
        /// <param name="p_s_jsonFile">full-path to json file</param>
        /// <returns>json decoded c# object</returns>
        /// <exception cref="ArgumentException">json file does not exist</exception>
        /// <exception cref="System.IO.IOException">cannot read json file content</exception>
        /// <exception cref="NullReferenceException">empty schema, empty json file or root node after parsing json content</exception>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        /// <exception cref="FormatException">exception parsing string to target type</exception>
        public Object? JsonDecode(string p_s_jsonFile)
        {
            /* check schema field */
            if (this.o_schema == null)
            {
                throw new NullReferenceException("Cannot decode data. Schema is null.");
            }

            /* check if file exists */
            if (!File.Exists(p_s_jsonFile))
            {
                throw new ArgumentException("JSON file[" + p_s_jsonFile + "] does not exist.");
            }

            /* open json file */
            File o_file = new(p_s_jsonFile, false);

            /* load file content into string */
            string s_fileContent = o_file.FileContent;

            List<string> a_fileLines =
            [
                /* load file content lines into array */
                .. s_fileContent.Split(this.s_lineBreak),
            ];

            ForestNETLib.Core.Global.ILogFiner("read all lines from json file '" + p_s_jsonFile + "'");

            /* decode json file lines */
            return JsonDecode(a_fileLines);
        }

        /// <summary>
        /// Decode json content to an c# object
        /// </summary>
        /// <param name="p_a_jsonLines">json lines</param>
        /// <returns>json decoded c# object</returns>
        /// <exception cref="NullReferenceException">empty schema, empty json file or root node after parsing json content</exception>
        /// <exception cref="ArgumentException">condition failed for decoding json correctly</exception>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        /// <exception cref="FormatException">exception parsing string to target type</exception>
        public Object? JsonDecode(List<string> p_a_jsonLines)
        {
            Object? o_foo = null;

            /* check schema field */
            if (this.o_schema == null)
            {
                throw new NullReferenceException("Cannot decode data. Schema is null.");
            }

            ForestNETLib.Core.Global.ILogFiner("read all lines: '" + p_a_jsonLines.Count + "'");

            System.Text.StringBuilder o_stringBuilder = new();

            /* read all json schema file lines to one string builder */
            foreach (string s_line in p_a_jsonLines)
            {
                o_stringBuilder.Append(s_line);
            }

            /* read all json-schema file lines and delete all line-wraps and tabs */
            string s_json = o_stringBuilder.ToString();
            s_json = new System.Text.RegularExpressions.Regex("[\\r\\n\\t]").Replace(s_json, "");

            /* remove all white spaces, but not between double quotes */
            s_json = JSON.RemoveWhiteSpaces(s_json);

            /* check if json-schema starts with (curly) brackets */
            if ((((!s_json.StartsWith("{")) || (!s_json.EndsWith("}"))) && ((!s_json.StartsWith("[")) || (!s_json.EndsWith("]")))))
            {
                throw new ArgumentException("JSON-file must start and end with curly bracket '{', '}' or must start and end with bracket '[', ']'");
            }

            /* validate json schema */
            JSON.ValidateJSON(s_json);

            ForestNETLib.Core.Global.ILogFinest("validated json content lines");

            this.Root = null;
            this.o_currentElement = null;

            /* parse json */
            this.ParseJSON(s_json);

            ForestNETLib.Core.Global.ILogFinest("parsed json content lines");

            /* check if root is null */
            if (this.Root == null)
            {
                throw new NullReferenceException("Root node is null");
            }

            if (ForestNETLib.Core.Global.IsILevel((byte)ForestNETLib.Log.Level.MASS)) ForestNETLib.Core.Global.ILogMass("JSON-Schema:" + ForestNETLib.IO.File.NEWLINE + this.o_schema);
            if (ForestNETLib.Core.Global.IsILevel((byte)ForestNETLib.Log.Level.MASS)) ForestNETLib.Core.Global.ILogMass("JSON-Root-Element:" + ForestNETLib.IO.File.NEWLINE + this.Root);

            /* decode json recursively */
            o_foo = this.JsonDecodeRecursive(this.Root, this.o_schema, o_foo);

            ForestNETLib.Core.Global.ILogFinest("decoded json content lines");

            return o_foo;
        }

        /// <summary>
        /// Recursive method to decode json string to a c# object and it's fields
        /// </summary>
        /// <param name="p_o_jsonDataElement">current json data element</param>
        /// <param name="p_o_jsonSchemaElement">current json schema element with additional information for decoding</param>
        /// <param name="p_o_object">destination c# object to decode json information</param>
        /// <returns>decoded json information as c# object</returns>
        /// <exception cref="ArgumentNullException">value in schema or expected element is not available</exception>
        /// <exception cref="ArgumentException">condition failed for decoding json correctly</exception>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        /// <exception cref="FormatException">exception parsing string to target type</exception>
        private Object? JsonDecodeRecursive(JSONElement p_o_jsonDataElement, JSONElement p_o_jsonSchemaElement, Object? p_o_object)
        {
            /* if type and mapping class are not set, we need at least a reference to continue */
            if ((ForestNETLib.Core.Helper.IsStringEmpty(p_o_jsonSchemaElement.Type)) && (ForestNETLib.Core.Helper.IsStringEmpty(p_o_jsonSchemaElement.MappingClass)))
            {
                if (p_o_jsonSchemaElement.Reference == null)
                {
                    throw new ArgumentNullException("Schema-element[" + p_o_jsonSchemaElement.Name + "] has no type, no mapping class and no reference");
                }
                else
                {
                    /* set reference as current schema-element */
                    p_o_jsonSchemaElement = p_o_jsonSchemaElement.Reference;
                }
            }

            /* check if type is set */
            if (ForestNETLib.Core.Helper.IsStringEmpty(p_o_jsonSchemaElement.Type))
            {
                throw new ArgumentException("Schema-element[" + p_o_jsonSchemaElement.Name + "] has no type");
            }

            /* check if mapping class is set if schema-element is not 'items' */
            if (ForestNETLib.Core.Helper.IsStringEmpty(p_o_jsonSchemaElement.MappingClass))
            {
                if (!p_o_jsonSchemaElement.Name.ToLower().Equals("items"))
                {
                    throw new ArgumentException("Schema-element[" + p_o_jsonSchemaElement.Name + "] has no mapping class");
                }
            }

            if ((p_o_jsonSchemaElement.Type == null) || (p_o_jsonSchemaElement.Type.ToLower().Equals("object")))
            {
                /* check if we have any data for new object */
                if (p_o_jsonDataElement.Children.Count < 1)
                {
                    /* set current object as null */
                    p_o_object = null;
                }
                else
                {
                    string s_objectType = p_o_jsonSchemaElement.MappingClass;

                    /* if object has reference, we create new object instance by mapping of reference */
                    if ((p_o_jsonSchemaElement.Reference != null) && (p_o_jsonSchemaElement.Reference.Type != null) && (p_o_jsonSchemaElement.Reference.Type.ToLower().Equals("object")))
                    {
                        s_objectType = p_o_jsonSchemaElement.Reference.MappingClass;
                    }

                    ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + p_o_jsonSchemaElement.Name + ": create new schema-object instance with mapping[" + p_o_jsonSchemaElement.PrintMapping() + "] and type[" + s_objectType + "]");

                    /* object variable which will be returned at the end of this function */
                    Object? o_object;

                    /* create new object instance which will be returned at the end of this function */
                    o_object = Activator.CreateInstance(Type.GetType(s_objectType) ?? throw new NullReferenceException("Could not create instance by object type '" + s_objectType + "'"));

                    /* increase level for PrintIndentation */
                    this.i_level++;

                    /* help variable to skip children iteration */
                    bool b_childrenIteration = true;

                    /* help variable for object mapping within objects */
                    string s_objectMapping = p_o_jsonSchemaElement.MappingClass;

                    /* if there is additional mapping information, use this for object mapping access */
                    if (!ForestNETLib.Core.Helper.IsStringEmpty(p_o_jsonSchemaElement.Mapping))
                    {
                        s_objectMapping = p_o_jsonSchemaElement.Mapping;
                    }

                    /* check conditions for handling object */
                    if (p_o_jsonSchemaElement.Reference != null)
                    {
                        /* check if reference has any children */
                        if (p_o_jsonSchemaElement.Reference.Children.Count < 1)
                        {
                            /* check if reference has another reference */
                            if (p_o_jsonSchemaElement.Reference.Reference == null)
                            {
                                throw new ArgumentNullException("Reference[" + p_o_jsonSchemaElement.Reference.Name + "] of schema-element[" + p_o_jsonSchemaElement.Name + "] has no children and no other reference");
                            }
                            else
                            {
                                b_childrenIteration = false;

                                /* check if current element in schema has data element by name, otherwise skip this element */
                                if (p_o_jsonSchemaElement.Name.Equals(p_o_jsonDataElement.Name))
                                {
                                    ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + p_o_jsonSchemaElement.Name + ": get schema-object[" + o_object?.GetType().FullName + "] with reference[" + p_o_jsonSchemaElement.Reference.Name + "]");

                                    /* only create new object if we have one object data */
                                    if (p_o_jsonDataElement.Children.Count != 1)
                                    {
                                        throw new ArgumentException("We have (" + p_o_jsonDataElement.Children.Count + ") no data children or more than one for schema-element[" + p_o_jsonSchemaElement.Name + "]");
                                    }

                                    /* handle reference with recursion */
                                    Object? o_returnObject = this.JsonDecodeRecursive(p_o_jsonDataElement.Children[0], p_o_jsonSchemaElement.Reference, o_object);

                                    /* check if we got a return object of recursion */
                                    if (o_returnObject == null)
                                    {
                                        /* it is valid to return a null object, anyway keep this exception if you want to uncomment it in the future */
                                        /* throw new Exception("Schema-element[" + p_o_jsonSchemaElement.Name + "] returns no object after recursion with data[" + p_o_jsonDataElement.Name + "]"); */
                                    }
                                }
                            }
                        }
                        else
                        {
                            ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + "update current schema-element[" + p_o_jsonSchemaElement.Name + "](" + p_o_jsonSchemaElement.PrintMapping() + ") with reference[" + p_o_jsonSchemaElement.Reference.Name + "](" + p_o_jsonSchemaElement.Reference.PrintMapping() + ")");

                            /* set reference as current json element */
                            p_o_jsonSchemaElement = p_o_jsonSchemaElement.Reference;
                        }
                    }

                    /* execute children iteration */
                    if (b_childrenIteration)
                    {
                        ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + p_o_jsonSchemaElement.Name + ": get schema-object with type[" + o_object?.GetType().FullName + "] and children[" + p_o_jsonSchemaElement.Children.Count + "]");

                        /* only create new object if we have one child definition for object in json schema */
                        if (p_o_jsonSchemaElement.Children.Count < 1)
                        {
                            throw new ArgumentException("Schema-element[" + p_o_jsonSchemaElement.Name + "] has no children");
                        }

                        /* check if new object is required if there is no data */
                        if ((p_o_jsonDataElement.Children.Count < 1) && (p_o_jsonSchemaElement.Required))
                        {
                            throw new ArgumentException("We have no data children for schema-element[" + p_o_jsonSchemaElement.Name + "] which is required");
                        }

                        /* only iterate if we have object data */
                        if (p_o_jsonDataElement.Children.Count > 0)
                        {
                            /* increase level for PrintIndentation */
                            this.i_level++;

                            /* data pointer */
                            int j = 0;

                            for (int i = 0; i < p_o_jsonSchemaElement.Children.Count; i++)
                            {
                                ForestNETLib.Core.Global.ILogFinest(this.PrintIndentation() + "compare schema-child-name(" + p_o_jsonSchemaElement.Children[i].Name + ") with data-child-name(" + p_o_jsonDataElement.Children[j].Name + ")");

                                /* check if current element in schema has data element by name, otherwise skip this element */
                                if (!p_o_jsonSchemaElement.Children[i].Name.Equals(p_o_jsonDataElement.Children[j].Name))
                                {
                                    /* but check if current schema element is required, so we must throw an exception */
                                    if (p_o_jsonSchemaElement.Children[i].Required)
                                    {
                                        throw new ArgumentException("We have no data for schema-element[" + p_o_jsonSchemaElement.Children[i].Name + "] which is required");
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }

                                /* handle new object with recursion */
                                Object? o_returnObject = this.JsonDecodeRecursive(p_o_jsonDataElement.Children[j], p_o_jsonSchemaElement.Children[i], o_object);

                                /* increase data pointer */
                                j++;

                                /* check if we got a return object of recursion */
                                if (o_returnObject == null)
                                {
                                    /* it is valid to return a null object, anyway keep this exception if you want to uncomment it in the future */
                                    /* throw new Exception("Schema-element[" + p_o_jsonSchemaElement.Children[i].Name + "] returns no object after recursion with data[" + p_o_jsonDataElement.Children[i].Name + "]"); */
                                }
                            }

                            /* decrease level for PrintIndentation */
                            this.i_level--;
                        }

                        if ((p_o_object != null) && (!((p_o_object.GetType().IsGenericType) && (p_o_object.GetType().GetGenericTypeDefinition() == typeof(List<>)))))
                        {
                            ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + p_o_jsonSchemaElement.Name + ": set schema-object[" + o_object?.GetType().FullName + "] to current object[" + p_o_object?.GetType().FullName + "] with mapping[" + s_objectMapping + "]");

                            this.SetFieldOrProperty(s_objectMapping, p_o_object, o_object);
                        }
                    }

                    /* decrease level for PrintIndentation */
                    this.i_level--;

                    /* set object instance as current object */
                    p_o_object = o_object;
                }
            }
            else if (p_o_jsonSchemaElement.Type.ToLower().Equals("array"))
            {
                /* check conditions for handling array */
                if (p_o_jsonSchemaElement.Reference != null)
                {
                    if (p_o_jsonSchemaElement.Reference.Children.Count < 1)
                    {
                        throw new ArgumentException("Reference[" + p_o_jsonSchemaElement.Reference.Name + "] of schema-array[" + p_o_jsonSchemaElement.Name + "] with p_o_object[" + p_o_object?.GetType().FullName + "] must have at least one child");
                    }
                }
                else
                {
                    if (p_o_jsonSchemaElement.Children.Count != 1)
                    {
                        throw new ArgumentException("Schema-array[" + p_o_jsonSchemaElement.Name + "] with p_o_object[" + p_o_object?.GetType().FullName + "] must have just one child");
                    }

                    if (!p_o_jsonSchemaElement.Children[0].Name.ToLower().Equals("items"))
                    {
                        throw new ArgumentException("Schema-array[" + p_o_jsonSchemaElement.Name + "] with p_o_object[" + p_o_object?.GetType().FullName + "] must have one child with name[items]");
                    }
                }

                /* help variables to handle array */
                Object o_objectList;
                bool b_required = false;
                string s_requiredProperty = "";
                List<JSONRestriction> a_restrictions = [];
                string s_amountProperty = "";
                bool b_primitiveArray = false;
                string s_primitiveArrayMapping = "";
                string s_genericListMapping = "";

                /* check if current array element is a primitive array */
                if ((p_o_jsonSchemaElement.Children.Count > 0) && (p_o_jsonSchemaElement.Children[0].PrimitiveArray))
                {
                    /* create list object for primitive array */
                    o_objectList = (Object)(new List<Object?>());
                    /* set flag for primitive array */
                    b_primitiveArray = true;

                    /* check if we have a mapping value for primitive array */
                    if ((ForestNETLib.Core.Helper.IsStringEmpty(p_o_jsonSchemaElement.Mapping)) && (ForestNETLib.Core.Helper.IsStringEmpty(p_o_jsonSchemaElement.MappingClass)))
                    {
                        throw new ArgumentException("Schema-primitive-array[" + p_o_jsonSchemaElement.Name + "] with p_o_object[" + p_o_object?.GetType().FullName + "] has no mapping value");
                    }
                    else
                    {
                        /* store mapping value for later use */
                        if (!(ForestNETLib.Core.Helper.IsStringEmpty(p_o_jsonSchemaElement.Mapping)))
                        {
                            s_primitiveArrayMapping = p_o_jsonSchemaElement.Mapping;
                        }
                        else if (!(ForestNETLib.Core.Helper.IsStringEmpty(p_o_jsonSchemaElement.MappingClass)))
                        {
                            s_primitiveArrayMapping = p_o_jsonSchemaElement.MappingClass;
                        }
                    }
                }
                else
                {
                    /* create or retrieve object list data */
                    if (p_o_object == null)
                    { /* create a new object instance of json array element */
                        ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + "p_o_object == null, create new array list");

                        /* create list object */
                        o_objectList = (Object)(new List<Object?>());
                    }
                    else
                    { /* we have to retrieve the list object */
                        ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + "p_o_object(" + p_o_object.GetType().FullName + ") != null, get list property " + p_o_jsonSchemaElement.Name);

                        /* get list property */
                        o_objectList = GetListFieldOrProperty(p_o_jsonSchemaElement, p_o_object);

                        /* we must cast returned generic list to List<object> */
                        o_objectList = ((System.Collections.IEnumerable)o_objectList).Cast<Object>().ToList();
                    }

                    /* check if we have a mapping value for generic list */
                    if ((ForestNETLib.Core.Helper.IsStringEmpty(p_o_jsonSchemaElement.Mapping)) && (ForestNETLib.Core.Helper.IsStringEmpty(p_o_jsonSchemaElement.MappingClass)))
                    {
                        throw new ArgumentException("Schema-primitive-array[" + p_o_jsonSchemaElement.Name + "] with p_o_object[" + p_o_object?.GetType().FullName + "] has no mapping value");
                    }
                    else
                    {
                        /* store mapping value for later use */
                        if (!(ForestNETLib.Core.Helper.IsStringEmpty(p_o_jsonSchemaElement.Mapping)))
                        {
                            s_genericListMapping = p_o_jsonSchemaElement.Mapping;
                        }
                        else if (!(ForestNETLib.Core.Helper.IsStringEmpty(p_o_jsonSchemaElement.MappingClass)))
                        {
                            s_genericListMapping = p_o_jsonSchemaElement.MappingClass;
                        }
                    }
                }

                /* check if json-element is required */
                if (p_o_jsonSchemaElement.Required)
                {
                    b_required = true;
                    s_requiredProperty = p_o_jsonSchemaElement.Name;
                }

                /* check minItems and maxItems restrictions and save them for items check afterwards */
                if (p_o_jsonSchemaElement.Restrictions.Count > 0)
                {
                    foreach (JSONRestriction o_jsonRestriction in p_o_jsonSchemaElement.Restrictions)
                    {
                        if ((o_jsonRestriction.Name.ToLower().Equals("minitems")) || (o_jsonRestriction.Name.ToLower().Equals("maxitems")))
                        {
                            a_restrictions.Add(o_jsonRestriction);
                            s_amountProperty = p_o_jsonSchemaElement.Name;
                        }
                    }
                }

                if (p_o_jsonSchemaElement.Reference != null)
                {
                    /* set reference as current json element */
                    p_o_jsonSchemaElement = p_o_jsonSchemaElement.Reference;
                }
                else
                {
                    /* set current json element to 'items' child */
                    p_o_jsonSchemaElement = p_o_jsonSchemaElement.Children[0];

                    /* if 'items' child has a child as well, we take this child as current json element */
                    if (p_o_jsonSchemaElement.Children.Count == 1)
                    {
                        p_o_jsonSchemaElement = p_o_jsonSchemaElement.Children[0];
                    }
                }

                if (ForestNETLib.Core.Helper.IsStringEmpty(p_o_jsonDataElement.Value))
                { /* we have multiple minor objects for current array */
                    ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + p_o_jsonSchemaElement.Name + ": get schema-array with mapping[" + p_o_jsonSchemaElement.PrintMapping() + "]");

                    if (p_o_jsonDataElement.Children.Count > 0)
                    { /* if we have objects to the new array */
                        /* check minItems and maxItems restrictions */
                        if (a_restrictions.Count > 0)
                        {
                            foreach (JSONRestriction o_jsonRestriction in a_restrictions)
                            {
                                if (o_jsonRestriction.Name.ToLower().Equals("minitems"))
                                {
                                    /* check minItems restriction */
                                    if (p_o_jsonDataElement.Children.Count < o_jsonRestriction.IntValue)
                                    {
                                        throw new ArgumentException("Restriction error: not enough [" + p_o_jsonSchemaElement.Name + " of " + s_amountProperty + "] json items(" + p_o_jsonDataElement.Children.Count + "), minimum = " + o_jsonRestriction.IntValue);
                                    }
                                }

                                if (o_jsonRestriction.Name.ToLower().Equals("maxitems"))
                                {
                                    /* check maxItems restriction */
                                    if (p_o_jsonDataElement.Children.Count > o_jsonRestriction.IntValue)
                                    {
                                        throw new ArgumentException("Restriction error: too many [" + p_o_jsonSchemaElement.Name + " of " + s_amountProperty + "] json items(" + p_o_jsonDataElement.Children.Count + "), maximum = " + o_jsonRestriction.IntValue);
                                    }
                                }
                            }
                        }

                        /* iterate objects in list and encode data to json recursively */
                        for (int i = 0; i < p_o_jsonDataElement.Children.Count; i++)
                        {
                            /* increase level for PrintIndentation */
                            this.i_level++;

                            /* handle array object with recursion */
                            Object? o_returnObject = this.JsonDecodeRecursive(p_o_jsonDataElement.Children[i], p_o_jsonSchemaElement, o_objectList);

                            ForestNETLib.Core.Global.ILogFinest(this.PrintIndentation() + "add return value to list of " + p_o_jsonDataElement.Name + ", type[" + ((o_returnObject == null) ? "object is null" : o_returnObject.GetType().FullName) + "]");

                            /* add return object of recursion to list */
                            ((List<Object?>)o_objectList).Add(o_returnObject);

                            /* decrease level for PrintIndentation */
                            this.i_level--;
                        }
                    }
                }
                else
                { /* array objects must be retrieved out of value property */
                    ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + p_o_jsonSchemaElement.Name + ": handle array value = " + p_o_jsonDataElement.Value + " - mapping[" + p_o_jsonSchemaElement.PrintMapping() + "]");

                    /* set array with values if we have any values */
                    if ((p_o_jsonDataElement.Value != null) && (!ForestNETLib.Core.Helper.IsStringEmpty(p_o_jsonDataElement.Value)) && (!p_o_jsonDataElement.Value.Equals("[]")))
                    {
                        /* remove opening and closing brackets */
                        if ((p_o_jsonDataElement.Value.StartsWith("[")) && (p_o_jsonDataElement.Value.EndsWith("]")))
                        {
                            p_o_jsonDataElement.Value = p_o_jsonDataElement.Value.Substring(1, p_o_jsonDataElement.Value.Length - 1 - 1);
                        }

                        /* split array into values, divided by ',' */
                        string[] a_values = p_o_jsonDataElement.Value.Split(",");

                        /* check minItems and maxItems restrictions */
                        if (a_restrictions.Count > 0)
                        {
                            foreach (JSONRestriction o_jsonRestriction in a_restrictions)
                            {
                                if (o_jsonRestriction.Name.ToLower().Equals("minitems"))
                                {
                                    /* check minItems restriction */
                                    if (a_values.Length < o_jsonRestriction.IntValue)
                                    {
                                        throw new ArgumentException("Restriction error: not enough [" + p_o_jsonSchemaElement.Name + " of " + s_amountProperty + "] json items(" + a_values.Length + "), minimum = " + o_jsonRestriction.IntValue);
                                    }
                                }

                                if (o_jsonRestriction.Name.ToLower().Equals("maxitems"))
                                {
                                    /* check maxItems restriction */
                                    if (a_values.Length > o_jsonRestriction.IntValue)
                                    {
                                        throw new ArgumentException("Restriction error: too many [" + p_o_jsonSchemaElement.Name + " of " + s_amountProperty + "] json items(" + a_values.Length + "), maximum = " + o_jsonRestriction.IntValue);
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

                            /* get json value type of string value */
                            JSONValueType e_jsonValueType = JSON.GetJSONValueType(s_value);

                            /* check if JSON value types are matching between schema and data, if it is not 'null' */
                            if ((e_jsonValueType != JSON.StringToJSONValueType(p_o_jsonSchemaElement.Type)) && (e_jsonValueType != JSONValueType.Null) && ((s_value != null) && (!s_value.Equals("null"))))
                            {
                                throw new ArgumentException("JSON schema type[" + JSON.StringToJSONValueType(p_o_jsonSchemaElement.Type) + "] does not match with data value type[" + e_jsonValueType + "] with value[" + s_value + "]");
                            }

                            /* check if json-element has any restrictions */
                            if (p_o_jsonSchemaElement.Restrictions.Count > 0)
                            {
                                foreach (JSONRestriction o_jsonRestriction in p_o_jsonSchemaElement.Restrictions)
                                {
                                    /* execute restriction check */
                                    JSON.CheckRestriction(s_value, o_jsonRestriction, p_o_jsonSchemaElement);
                                }
                            }

                            /* cast array string value into object */
                            Object? o_returnObject = JSON.CastObjectFromString(s_value, p_o_jsonSchemaElement.Type);

                            ForestNETLib.Core.Global.ILogFinest(this.PrintIndentation() + "add value[" + (o_returnObject ?? "null") + "] to list of " + p_o_jsonDataElement.Name + ", type[" + ((o_returnObject == null) ? "null" : o_returnObject.GetType().FullName) + "]");

                            /* add return object of recursion to list */
                            ((List<Object?>)o_objectList).Add(o_returnObject);
                        }
                    }
                    else if (b_required)
                    { /* if json-element with type array is required, throw exception */
                        throw new ArgumentNullException("'" + p_o_jsonSchemaElement.Name + "' of '" + s_requiredProperty + "' is required, but array has no values");
                    }
                }

                /* if we have a primitive array, we take our object list and convert it to a primitive array */
                if (b_primitiveArray)
                {
                    /* we must have an object to set primitive array property */
                    if (p_o_object == null)
                    {
                        throw new ArgumentNullException("JSON schema element[" + p_o_jsonSchemaElement.Name + "] has not initiated object for primitive array[" + s_primitiveArrayMapping + "]");
                    }

                    ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + "set primitive array for [" + s_primitiveArrayMapping + "] of object [" + p_o_object.GetType().FullName + "]");

                    /* set primitive array property of object */
                    this.SetPrimitiveArrayFieldOrProperty(s_primitiveArrayMapping, p_o_object, o_objectList);
                }

                /* set array instance as current object, if it is still null */
                if (p_o_object == null)
                {
                    ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + "p_o_object == null, set p_o_object = o_objectList");

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
                ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + p_o_jsonSchemaElement.Name + ": handle value = " + p_o_jsonDataElement.Value + " - mapping[" + p_o_jsonSchemaElement.PrintMapping() + "]");

                JSONValueType e_jsonValueType = JSON.GetJSONValueType(p_o_jsonDataElement.Value);

                ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + p_o_jsonSchemaElement.Name + ": json value type = " + e_jsonValueType);

                /* check if JSON value types are matching between schema and data, additional condition = JSON value type is not 'null' */
                if ((e_jsonValueType != JSON.StringToJSONValueType(p_o_jsonSchemaElement.Type)) && (e_jsonValueType != JSONValueType.Null))
                {
                    /* it is acceptable if source type is 'integer' and destination type is 'number', valid cast available */
                    if (!((e_jsonValueType == JSONValueType.Integer) && (JSON.StringToJSONValueType(p_o_jsonSchemaElement.Type) == JSONValueType.Number)))
                    {
                        throw new ArgumentException("JSON schema type[" + e_jsonValueType + "] does not match with data value type[" + JSON.StringToJSONValueType(p_o_jsonSchemaElement.Type) + "] with value[" + p_o_jsonDataElement.Value + "]");
                    }
                }

                /* check if json-element is required */
                if (p_o_jsonSchemaElement.Required)
                {
                    /* check if value is empty */
                    if ((p_o_jsonDataElement.Value == null) || (p_o_jsonDataElement.Value.Equals("")) || (p_o_jsonDataElement.Value.Equals("null")) || (p_o_jsonDataElement.Value.Equals("\"\"")))
                    {
                        throw new ArgumentException("'" + p_o_jsonSchemaElement.Name + "' is required, but value[" + p_o_jsonDataElement.Value + "] is empty");
                    }
                }

                /* check if json-element has any restrictions */
                if (p_o_jsonSchemaElement.Restrictions.Count > 0)
                {
                    foreach (JSONRestriction o_jsonRestriction in p_o_jsonSchemaElement.Restrictions)
                    {
                        /* execute restriction check */
                        JSON.CheckRestriction(p_o_jsonDataElement.Value, o_jsonRestriction, p_o_jsonSchemaElement);
                    }
                }

                this.SetFieldOrProperty(p_o_jsonSchemaElement, p_o_object, p_o_jsonDataElement.Value);
            }

            ForestNETLib.Core.Global.ILogFiner(this.PrintIndentation() + "return " + ((p_o_object == null) ? "null" : p_o_object.GetType().FullName));
            return p_o_object;
        }

        /// <summary>
        /// Convert a string value of json type to json type enumeration value
        /// </summary>
        /// <param name="p_s_jsonValueType">json type string from json element</param>
        /// <returns>json type enumeration value</returns>
        /// <exception cref="ArgumentException">invalid string type parameter</exception>
        private static JSONValueType StringToJSONValueType(string? p_s_jsonValueType)
        {
            p_s_jsonValueType = p_s_jsonValueType?.ToLower();

            if (p_s_jsonValueType == null)
            {
                throw new ArgumentException("Invalid JSON value type with value [null]");
            }
            else if (p_s_jsonValueType.Equals("string"))
            {
                return JSONValueType.String;
            }
            else if (p_s_jsonValueType.Equals("number"))
            {
                return JSONValueType.Number;
            }
            else if (p_s_jsonValueType.Equals("integer"))
            {
                return JSONValueType.Integer;
            }
            else if (p_s_jsonValueType.Equals("boolean"))
            {
                return JSONValueType.Boolean;
            }
            else if (p_s_jsonValueType.Equals("array"))
            {
                return JSONValueType.Array;
            }
            else if (p_s_jsonValueType.Equals("object"))
            {
                return JSONValueType.Object;
            }
            else if (p_s_jsonValueType.Equals("null"))
            {
                return JSONValueType.Null;
            }
            else
            {
                throw new ArgumentException("Invalid JSON value type with value [" + p_s_jsonValueType + "]");
            }
        }

        /// <summary>
        /// Get a List object from object's property, access directly via public field or public methods
        /// </summary>
        /// <param name="p_o_jsonSchemaElement">json schema element with mapping information</param>
        /// <param name="p_o_object">object where to retrieve List object</param>
        /// <returns>List object</returns>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        /// <exception cref="ArgumentNullException">list object is not initialized</exception>
        private Object GetListFieldOrProperty(JSONElement p_o_jsonSchemaElement, Object? p_o_object)
        {
            /* retrieve field information out of schema element */
            string s_field = p_o_jsonSchemaElement.MappingClass;

            /* if there is additional mapping information, use this for field property access */
            if (!ForestNETLib.Core.Helper.IsStringEmpty(p_o_jsonSchemaElement.Mapping))
            {
                s_field = p_o_jsonSchemaElement.Mapping;
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
        /// Method to set property field of an object with value of json element from file line and mapping value of json schema element
        /// </summary>
        /// <param name="p_o_jsonSchemaElement">mapping class and type hint to cast value to object's field from json schema</param>
        /// <param name="p_o_object">object parameter where json data will be decoded and cast into object fields</param>
        /// <param name="p_s_objectValue">string value of json element from file line</param>
        /// <exception cref="MissingMemberException">could not retrieve field type by field name</exception>
        /// <exception cref="MemberAccessException">could not invoke method, access violation</exception>
        /// <exception cref="InvalidCastException">could not retrieve class by string class name</exception>
        /// <exception cref="FormatException">exception parsing string to target type</exception>
        private void SetFieldOrProperty(JSONElement p_o_jsonSchemaElement, Object? p_o_object, string? p_s_objectValue)
        {
            /* detect null value */
            if ((p_s_objectValue == null) || (p_s_objectValue.Equals("null")))
            {
                return;
            }
            else
            {
                /* remove surrounded double quotes from value */
                if ((p_s_objectValue.StartsWith("\"")) && (p_s_objectValue.EndsWith("\"")))
                {
                    p_s_objectValue = p_s_objectValue.Substring(1, p_s_objectValue.Length - 1 - 1);
                }
            }

            /* retrieve field information out of schema element */
            string s_field = p_o_jsonSchemaElement.MappingClass;

            /* if there is additional mapping information, use this for field property access */
            if (!ForestNETLib.Core.Helper.IsStringEmpty(p_o_jsonSchemaElement.Mapping))
            {
                s_field = p_o_jsonSchemaElement.Mapping;
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
                    /* set value to property of current element and cast string to object value with json schema type */
                    o_propertyInfo.SetValue(p_o_object, JSON.CastObjectFromString(p_s_objectValue, p_o_jsonSchemaElement.Type));
                }
                catch (ArgumentException o_exc)
                {
                    ForestNETLib.Core.Global.ILogFinest(this.PrintIndentation() + o_exc.Message + " - with type[" + p_o_jsonSchemaElement.Type + "] - try another cast with type[" + o_propertyInfo.PropertyType.FullName + "]");

                    /* invoke set-property-method to set object data field of current element and cast string to object value */
                    o_propertyInfo.SetValue(p_o_object, JSON.CastObjectFromString(p_s_objectValue, o_propertyInfo.PropertyType.FullName));
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
                    o_fieldInfo.SetValue(p_o_object, JSON.CastObjectFromString(p_s_objectValue, p_o_jsonSchemaElement.Type));
                }
                catch (ArgumentException o_exc)
                {
                    ForestNETLib.Core.Global.ILogFinest(this.PrintIndentation() + o_exc.Message + " - with type[" + p_o_jsonSchemaElement.Type + "]  - try another cast with type[" + o_fieldInfo.FieldType.FullName + "]");

                    o_fieldInfo.SetValue(p_o_object, JSON.CastObjectFromString(p_s_objectValue, o_fieldInfo.FieldType.FullName));
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
        /// <param name="p_o_object">object parameter where json data will be decoded and cast into object fields</param>
        /// <param name="p_o_objectValue">object value of json element from file line</param>
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
        /// <param name="p_o_object">object parameter where json data will be decoded and cast into object fields</param>
        /// <param name="p_o_objectValue">object value of json element from file line</param>
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
                            o_bar[i] = (bool?)JSON.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.Byte"))
                    {
                        byte[] o_bar = new byte[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (byte?)JSON.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.SByte"))
                    {
                        sbyte[] o_bar = new sbyte[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (sbyte?)JSON.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.Char"))
                    {
                        char[] o_bar = new char[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (char?)JSON.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
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

                            o_bar[i] = (float?)JSON.CastObjectFromString(s_foo, s_primitiveArrayType) ?? default;
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

                            o_bar[i] = (double?)JSON.CastObjectFromString(s_foo, s_primitiveArrayType) ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.Int16"))
                    {
                        short[] o_bar = new short[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (short?)JSON.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.Int32"))
                    {
                        int[] o_bar = new int[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (int?)JSON.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.Int64"))
                    {
                        long[] o_bar = new long[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (long?)JSON.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.UInt16"))
                    {
                        ushort[] o_bar = new ushort[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (ushort?)JSON.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.UInt32"))
                    {
                        uint[] o_bar = new uint[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (uint?)JSON.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.UInt64"))
                    {
                        ulong[] o_bar = new ulong[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (ulong?)JSON.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.String"))
                    {
                        string[] o_bar = new string[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (string?)JSON.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? string.Empty;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.DateTime"))
                    {
                        DateTime[] o_bar = new DateTime[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (DateTime?)JSON.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.TimeSpan"))
                    {
                        TimeSpan[] o_bar = new TimeSpan[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (TimeSpan?)JSON.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
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

                            o_bar[i] = (decimal?)JSON.CastObjectFromString(s_foo, s_primitiveArrayType) ?? default;
                        }

                        o_propertyInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.Object"))
                    {
                        object[] o_bar = new object[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = JSON.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? new object();
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
                            o_bar[i] = (bool?)JSON.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.Byte"))
                    {
                        byte[] o_bar = new byte[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (byte?)JSON.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.SByte"))
                    {
                        sbyte[] o_bar = new sbyte[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (sbyte?)JSON.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.Char"))
                    {
                        char[] o_bar = new char[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (char?)JSON.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
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

                            o_bar[i] = (float?)JSON.CastObjectFromString(s_foo, s_primitiveArrayType) ?? default;
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

                            o_bar[i] = (double?)JSON.CastObjectFromString(s_foo, s_primitiveArrayType) ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.Int16"))
                    {
                        short[] o_bar = new short[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (short?)JSON.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.Int32"))
                    {
                        int[] o_bar = new int[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (int?)JSON.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.Int64"))
                    {
                        long[] o_bar = new long[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (long?)JSON.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.UInt16"))
                    {
                        ushort[] o_bar = new ushort[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (ushort?)JSON.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.UInt32"))
                    {
                        uint[] o_bar = new uint[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (uint?)JSON.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.UInt64"))
                    {
                        ulong[] o_bar = new ulong[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (ulong?)JSON.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.String"))
                    {
                        string[] o_bar = new string[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (string?)JSON.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? string.Empty;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.DateTime"))
                    {
                        DateTime[] o_bar = new DateTime[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (DateTime?)JSON.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.TimeSpan"))
                    {
                        TimeSpan[] o_bar = new TimeSpan[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = (TimeSpan?)JSON.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? default;
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

                            o_bar[i] = (decimal?)JSON.CastObjectFromString(s_foo, s_primitiveArrayType) ?? default;
                        }

                        o_fieldInfo.SetValue(p_o_object, o_bar);
                    }
                    else if (s_primitiveArrayType.Equals("System.Object"))
                    {
                        object[] o_bar = new object[o_foo.Count];

                        for (int i = 0; i < o_foo.Count; i++)
                        {
                            o_bar[i] = JSON.CastObjectFromString((o_foo[i]?.ToString()), s_primitiveArrayType) ?? new object();
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
        /// <param name="p_o_object">object parameter where json data will be decoded and cast into object fields</param>
        /// <param name="p_o_objectValue">object value of json element from file line</param>
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
                            o_bar[i] = (string?)o_foo[i] ?? string.Empty;
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
        /// Convert a string value from a json element to an object to decode it into an object
        /// </summary>
        /// <param name="p_s_value">string value of json element from file</param>
        /// <param name="p_s_type">type of destination object field</param>
        /// <returns>casted object value from string</returns>
        /// <exception cref="ArgumentException">invalid type parameter</exception>
        /// <exception cref="FormatException">exception parsing string to target type</exception>
        private static Object? CastObjectFromString(string? p_s_value, string? p_s_type)
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
                    if (ForestNETLib.Core.Helper.IsDateTime(p_s_value))
                    {
                        o_foo = ForestNETLib.Core.Helper.FromISO8601UTC(p_s_value);
                    }
                    else if (ForestNETLib.Core.Helper.IsTime(p_s_value))
                    {
                        o_foo = ForestNETLib.Core.Helper.FromISO8601UTC("1900-01-01T" + p_s_value + "Z").TimeOfDay;
                    }
                    else
                    {
                        o_foo = p_s_value.Replace("\\\"", "\"");
                    }
                }
                else if (p_s_type.Equals("system.datetime"))
                {
                    if (ForestNETLib.Core.Helper.IsDateTime(p_s_value))
                    {
                        o_foo = ForestNETLib.Core.Helper.FromISO8601UTC(p_s_value);
                    }
                    else
                    {
                        throw new ArgumentException("Illegal value '" + p_s_value + "' for 'System.DateTime'");
                    }
                }
                else if (p_s_type.Equals("system.timespan"))
                {
                    if (ForestNETLib.Core.Helper.IsDateTime(p_s_value))
                    {
                        o_foo = ForestNETLib.Core.Helper.FromISO8601UTC(p_s_value).TimeOfDay;
                    }
                    else if (ForestNETLib.Core.Helper.IsTime(p_s_value))
                    {
                        o_foo = ForestNETLib.Core.Helper.FromISO8601UTC("1900-01-01T" + p_s_value + "Z").TimeOfDay;
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
                        o_foo = (object)p_s_value;
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
        /// Enumeration of valid json types
        /// </summary>
        public enum JSONValueType
        {
            String, Number, Integer, Boolean, Array, Object, Null
        }

        /// <summary>
        /// Encapsulation of a schema's json element
        /// </summary>
        public class JSONElement
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
            public JSONElement? Reference { get; set; }
            public List<JSONElement> Children { get; }
            public List<JSONRestriction> Restrictions { get; }

            /* Methods */

            /// <summary>
            /// JSONElement constructor with empty string as name and level 0
            /// </summary>
            public JSONElement() : this("", 0)
            {

            }

            /// <summary>
            /// JSONElement constructor with parameter name and level 0
            /// </summary>
            /// <param name="p_s_name">name of json element in schema</param>
            public JSONElement(string p_s_name) : this(p_s_name, 0)
            {

            }

            /// <summary>
            /// JSONElement constructor
            /// </summary>
            /// <param name="p_s_name">name of json element in schema</param>
            /// <param name="p_i_level">level of json element</param>
            public JSONElement(string p_s_name, int p_i_level)
            {
                this.Children = [];
                this.Restrictions = [];
                this.Name = p_s_name;
                this.Level = p_i_level;

                this.Mapping = string.Empty;
                this.MappingClass = string.Empty;
            }

            /// <summary>
            /// Print i tabs as indentation, i is the level of the json element
            /// </summary>
            /// <returns>indentation string</returns>
            private string PrintIndentation()
            {
                string s_foo = "";

                for (int i = 0; i < this.Level; i++)
                {
                    s_foo += "\t";
                }

                return s_foo;
            }

            /// <summary>
            /// Returns each field of json element with name and value, separated by a pipe '|'
            /// </summary>
            override public string ToString()
            {
                string s_foo = this.PrintIndentation() + "JSONElement: ";

                foreach (System.Reflection.PropertyInfo o_property in this.GetType().GetProperties())
                {
                    string s_value = "";

                    try
                    {
                        if (o_property.PropertyType == typeof(JSONElement))
                        {
                            s_value = "[" + o_property.GetValue(this)?.ToString() + "]";
                        }
                        else if ((o_property.PropertyType.IsGenericType) && (o_property.PropertyType.GetGenericTypeDefinition() == typeof(List<>)))
                        {
                            if (o_property.PropertyType.GenericTypeArguments[0] == typeof(JSONElement))
                            {
                                List<JSONElement> a_foo = (List<JSONElement>)(o_property.GetValue(this) ?? new List<JSONElement>());

                                if (a_foo.Count > 0)
                                {
                                    s_foo += ForestNETLib.IO.File.NEWLINE;

                                    foreach (JSONElement o_foo in a_foo)
                                    {
                                        s_foo += o_foo.ToString() + ForestNETLib.IO.File.NEWLINE;
                                    }
                                }
                                else
                                {
                                    s_foo += "null" + ForestNETLib.IO.File.NEWLINE;
                                }
                            }
                            else if (o_property.PropertyType.GenericTypeArguments[0] == typeof(JSONRestriction))
                            {
                                List<JSONRestriction> a_foo = (List<JSONRestriction>)(o_property.GetValue(this) ?? new List<JSONRestriction>());

                                if (a_foo.Count > 0)
                                {
                                    s_foo += ForestNETLib.IO.File.NEWLINE;

                                    foreach (JSONRestriction o_foo in a_foo)
                                    {
                                        s_foo += o_foo.ToString() + ForestNETLib.IO.File.NEWLINE;
                                    }
                                }
                                else
                                {
                                    s_foo += "null" + ForestNETLib.IO.File.NEWLINE;
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
            /// Returns mapping string of json element, mapping class or a combination of mapping and mapping class separated by ':'
            /// </summary>
            public string PrintMapping()
            {
                string s_foo = "";

                if ((ForestNETLib.Core.Helper.IsStringEmpty(this.Mapping)) && (!ForestNETLib.Core.Helper.IsStringEmpty(this.MappingClass)))
                {
                    s_foo = this.MappingClass;
                }
                else if ((!ForestNETLib.Core.Helper.IsStringEmpty(this.Mapping)) && (!ForestNETLib.Core.Helper.IsStringEmpty(this.MappingClass)))
                {
                    s_foo = this.Mapping + ":" + this.MappingClass;
                }

                return s_foo;
            }
        }

        /// <summary>
        /// Encapsulation of a schema's json element restrictions
        /// </summary>
        public class JSONRestriction
        {

            /* Fields */

            /* Properties */

            public string Name { get; set; }
            public int Level { get; set; }
            public string StrValue { get; set; }
            public int IntValue { get; set; }

            /* Methods */

            /// <summary>
            /// JSONRestriction constructor, no name value [= null], level 0, no string value [= null] and no integer value [= 0]
            /// </summary>
            public JSONRestriction() : this("", 0, "", 0)
            {

            }

            /// <summary>
            /// JSONRestriction constructor, level 0, no string value [= null] and no integer value [= 0]
            /// </summary>
            /// <param name="p_s_name">name of json element restriction</param>
            public JSONRestriction(string p_s_name) : this(p_s_name, 0, "", 0)
            {

            }

            /// <summary>
            /// JSONRestriction constructor, no string value [= null] and no integer value [= 0]
            /// </summary>
            /// <param name="p_s_name">name of json element restriction</param>
            /// <param name="p_i_level">level of json element restriction</param>
            public JSONRestriction(string p_s_name, int p_i_level) : this(p_s_name, p_i_level, "", 0)
            {

            }

            /// <summary>
            /// JSONRestriction constructor, no integer value [= 0]
            /// </summary>
            /// <param name="p_s_name">name of json element restriction</param>
            /// <param name="p_i_level">level of json element restriction</param>
            /// <param name="p_s_strValue">string value of restriction</param>
            public JSONRestriction(string p_s_name, int p_i_level, string p_s_strValue) : this(p_s_name, p_i_level, p_s_strValue, 0)
            {

            }

            /// <summary>
            /// JSONRestriction constructor
            /// </summary>
            /// <param name="p_s_name">name of json element restriction</param>
            /// <param name="p_i_level">level of json element restriction</param>
            /// <param name="p_s_strValue">string value of restriction</param>
            /// <param name="p_i_intValue">integer value of restriction</param>
            public JSONRestriction(string p_s_name, int p_i_level, string p_s_strValue, int p_i_intValue)
            {
                this.Name = p_s_name;
                this.Level = p_i_level;
                this.StrValue = p_s_strValue;
                this.IntValue = p_i_intValue;
            }

            /// <summary>
            /// Print i tabs as indentation, i is the level of the json element
            /// </summary>
            /// <returns>indentation string</returns>
            private string PrintIndentation()
            {
                string s_foo = "";

                for (int i = 0; i < this.Level; i++)
                {
                    s_foo += "\t";
                }

                return s_foo;
            }

            /// <summary>
            /// Returns each field of json element restriction with name and value, separated by a pipe '|'
            /// </summary>
            override public string ToString()
            {
                string s_foo = ForestNETLib.IO.File.NEWLINE + this.PrintIndentation() + "\t" + "JSONRestriction: ";

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