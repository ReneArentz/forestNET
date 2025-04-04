namespace ForestNET.Lib.IO
{
    /// <summary>
    /// Class to handle a fixed length record file - automatically detecting records, group headers or footers as a stacks of data
    /// </summary>
    public class FixedLengthRecordFile
    {

        /* Fields */

        private string s_lineBreak = ForestNET.Lib.IO.File.NEWLINE;

        /* Properties */

        /// <summary>
        /// Determine line break characters for reading and writing fixed length record files
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
        public FLRType? GroupHeader = null;
        public List<FLRType> FLRTypes = [];
        public FLRType? GroupFooter = null;
        public Dictionary<int, FixedLengthRecordStack> Stacks = [];

        /* Methods */

        /// <summary>
        /// Fixed length record file constructor
        /// </summary>
        /// <param name="p_o_flr">fixed length record object for configuration</param>
        /// <param name="p_s_regexFLR">regex recognition</param>
        /// <exception cref="ArgumentException">parameter is null or invalid</exception>
        public FixedLengthRecordFile(IFixedLengthRecord p_o_flr, string p_s_regexFLR) :
            this(p_o_flr, p_s_regexFLR, null, null, null, null)
        {

        }

        /// <summary>
        /// Fixed length record file constructor
        /// </summary>
        /// <param name="p_o_flr">fixed length record object for configuration</param>
        /// <param name="p_s_regexFLR">regex recognition</param>
        /// <param name="p_i_knownLengthFLR">overall line length of flr</param>
        /// <exception cref="ArgumentException">parameter is null or invalid</exception>
        public FixedLengthRecordFile(IFixedLengthRecord p_o_flr, string? p_s_regexFLR, int p_i_knownLengthFLR) :
            this(p_o_flr, p_s_regexFLR, p_i_knownLengthFLR, null, null, -1, null, null, -1)
        {

        }

        /// <summary>
        /// Fixed length record file constructor
        /// </summary>
        /// <param name="p_o_flr">fixed length record object for configuration</param>
        /// <param name="p_s_regexFLR">regex recognition</param>
        /// <param name="p_o_groupHeader">group header object</param>
        /// <param name="p_s_regexGroupHeader">regex recognition</param>
        /// <exception cref="ArgumentException">parameter is null or invalid</exception>
        public FixedLengthRecordFile(IFixedLengthRecord p_o_flr, string p_s_regexFLR, IFixedLengthRecord p_o_groupHeader, string p_s_regexGroupHeader) :
            this(p_o_flr, p_s_regexFLR, p_o_groupHeader, p_s_regexGroupHeader, null, null)
        {

        }

        /// <summary>
        /// Fixed length record file constructor
        /// </summary>
        /// <param name="p_o_flr">fixed length record object for configuration</param>
        /// <param name="p_s_regexFLR">regex recognition</param>
        /// <param name="p_i_knownLengthFLR">overall line length of flr</param>
        /// <param name="p_o_groupHeader">group header object</param>
        /// <param name="p_s_regexGroupHeader">regex recognition</param>
        /// <param name="p_i_knownLengthGroupHeader">overall line length of group header</param>
        /// <exception cref="ArgumentException">parameter is null or invalid</exception>
        public FixedLengthRecordFile(IFixedLengthRecord p_o_flr, string? p_s_regexFLR, int p_i_knownLengthFLR, IFixedLengthRecord p_o_groupHeader, string? p_s_regexGroupHeader, int p_i_knownLengthGroupHeader) :
            this(p_o_flr, p_s_regexFLR, p_i_knownLengthFLR, p_o_groupHeader, p_s_regexGroupHeader, p_i_knownLengthGroupHeader, null, null, -1)
        {

        }

        /// <summary>
        /// Fixed length record file constructor
        /// </summary>
        /// <param name="p_o_flr">fixed length record object for configuration</param>
        /// <param name="p_s_regexFLR">regex recognition</param>
        /// <param name="p_o_groupHeader">group header object</param>
        /// <param name="p_s_regexGroupHeader">regex recognition</param>
        /// <param name="p_o_groupFooter">group footer object</param>
        /// <param name="p_s_regexGroupFooter">regex recognition</param>
        /// <exception cref="ArgumentException">parameter is null or invalid</exception>
        public FixedLengthRecordFile(IFixedLengthRecord p_o_flr, string p_s_regexFLR, IFixedLengthRecord? p_o_groupHeader, string? p_s_regexGroupHeader, IFixedLengthRecord? p_o_groupFooter, string? p_s_regexGroupFooter) :
            this(p_o_flr, p_s_regexFLR, -1, p_o_groupHeader, p_s_regexGroupHeader, -1, p_o_groupFooter, p_s_regexGroupFooter, -1)
        {

        }

        /// <summary>
        /// Fixed length record file constructor
        /// </summary>
        /// <param name="p_o_flr">fixed length record object for configuration</param>
        /// <param name="p_s_regexFLR">regex recognition</param>
        /// <param name="p_i_knownLengthFLR">overall line length of flr</param>
        /// <param name="p_o_groupHeader">group header object</param>
        /// <param name="p_s_regexGroupHeader">regex recognition</param>
        /// <param name="p_i_knownLengthGroupHeader">overall line length of group header</param>
        /// <param name="p_o_groupFooter">group footer object</param>
        /// <param name="p_s_regexGroupFooter">regex recognition</param>
        /// <param name="p_i_knownLengthGroupFooter">overall line length of group footer</param>
        /// <exception cref="ArgumentException">parameter is null or invalid</exception>
        public FixedLengthRecordFile(IFixedLengthRecord p_o_flr, string? p_s_regexFLR, int p_i_knownLengthFLR, IFixedLengthRecord? p_o_groupHeader, string? p_s_regexGroupHeader, int p_i_knownLengthGroupHeader, IFixedLengthRecord? p_o_groupFooter, string? p_s_regexGroupFooter, int p_i_knownLengthGroupFooter)
        {
            if ((p_o_groupHeader != null) && ((ForestNET.Lib.Helper.IsStringEmpty(p_s_regexGroupHeader)) ^ (p_i_knownLengthGroupHeader < 0)))
            {
                this.GroupHeader = new FLRType(p_o_groupHeader, p_s_regexGroupHeader, p_i_knownLengthGroupHeader);
            }

            if ((p_o_groupFooter != null) && ((ForestNET.Lib.Helper.IsStringEmpty(p_s_regexGroupFooter)) ^ (p_i_knownLengthGroupFooter < 0)))
            {
                this.GroupFooter = new FLRType(p_o_groupFooter, p_s_regexGroupFooter, p_i_knownLengthGroupFooter);
            }

            if (p_o_flr == null)
            {
                throw new ArgumentException("Parameter for fixed length record object is null");
            }

            if ((ForestNET.Lib.Helper.IsStringEmpty(p_s_regexFLR)) && (p_i_knownLengthFLR < 0))
            {
                throw new ArgumentException("Parameter for recognizing fixed length record and parameter for known length of flr is null or empty");
            }

            this.AddFLRType(new FLRType(p_o_flr, p_s_regexFLR, p_i_knownLengthFLR));
        }

        /// <summary>
        /// Add flr type to configuration, which are accepted for flr file
        /// </summary>
        /// <param name="p_o_flrType">fixed length record type object for the list of flr types</param>
        /// <exception cref="ArgumentException">regex for recognizing is invalid or flr type already exists in configuration</exception>
        public void AddFLRType(FLRType p_o_flrType)
        {
            /* iterate all existing flr types */
            foreach (FLRType o_flrType in this.FLRTypes)
            {
                /* check if we already have a flr type with matching regex and known length parameter */
                if (((!ForestNET.Lib.Helper.IsStringEmpty(p_o_flrType.RegexFLR)) && ((o_flrType.RegexFLR ?? "").Equals(p_o_flrType.RegexFLR))) && (o_flrType.KnownLengthFLR == p_o_flrType.KnownLengthFLR))
                {
                    throw new ArgumentException("Fixed length record type with regex '" + p_o_flrType.RegexFLR + "' and known length '" + p_o_flrType.KnownLengthFLR + "' already exists in configuration");
                }
            }

            /* clear unique temp map */
            p_o_flrType.FLRObject?.ClearUniqueTemp();

            this.FLRTypes.Add(p_o_flrType);
        }

        /// <summary
        /// </summar>
        /// <returns>get new fixed length record stack object</returns>
        public static FixedLengthRecordStack CreateNewStack()
        {
            return new FixedLengthRecordStack();
        }

        /// <summary>
        /// Add stack to configuration
        /// </summary>
        /// <param name="p_i_key">number of stack</param>
        /// <param name="p_o_stack">flr stack object</param>
        public void AddStack(int p_i_key, FixedLengthRecordStack p_o_stack)
        {
            this.Stacks.Add(p_i_key, p_o_stack);
        }

        /// <summary>
        /// method to read a fixed length record file, with UTF-8 BOM
        /// </summary>
        /// <param name="p_s_file">full-path to flr file</param>
        /// <exception cref="ArgumentException">value/structure within flr file invalid</exception>
        /// <exception cref="System.IO.IOException">cannot access or open flr file and it's content</exception>
        public void ReadFile(string p_s_file)
        {
            this.ReadFile(p_s_file, System.Text.Encoding.GetEncoding(ForestNET.Lib.IO.File.ENCODING), false);
        }

        /// <summary>
        /// method to read a fixed length record file
        /// </summary>
        /// <param name="p_s_file">full-path to flr file</param>
        /// <param name="p_o_encoding">which encoding will be used accessing/modifying the file content</param>
        /// <exception cref="ArgumentException">value/structure within flr file invalid</exception>
        /// <exception cref="System.IO.IOException">cannot access or open flr file and it's content</exception>
        public void ReadFile(string p_s_file, System.Text.Encoding p_o_encoding)
        {
            this.ReadFile(p_s_file, p_o_encoding, false);
        }

        /// <summary>
        /// method to read a fixed length record file
        /// </summary>
        /// <param name="p_s_file">full-path to flr file</param>
        /// <param name="p_o_encoding">which encoding will be used accessing/modifying the file content</param>
        /// <param name="p_b_ignroeUniqueConstraint">true - ignore unique constraint of unique fields and it's values, false - exception will be thrown if unique constraint has been violated</param>
        /// <exception cref="ArgumentException">value/structure within flr file invalid</exception>
        /// <exception cref="System.IO.IOException">cannot access or open flr file and it's content</exception>
        public void ReadFile(string p_s_file, System.Text.Encoding p_o_encoding, bool p_b_ignroeUniqueConstraint)
        {
            /* check if file exists */
            if (!File.Exists(p_s_file))
            {
                throw new ArgumentException("File[" + p_s_file + "] does not exist.");
            }

            /* open flr file */
            File o_file = new(p_s_file, false, p_o_encoding, this.s_lineBreak);

            /* read all flr file lines */
            List<string> a_flrLines = o_file.FileContentAsList ?? [];

            /* help variables */
            int i_stackNumber = 0;
            int i_flrNumber = 0;
            this.Stacks[i_stackNumber] = new FixedLengthRecordStack();
            ReadState e_readState;

            /* iterate each line in flr file */
            foreach (string s_line in a_flrLines)
            {
                FLRType? o_foundFLRType = null;

                /* check if we have a line for group header */
                if ((this.GroupHeader != null) &&
                        (
                            (!ForestNET.Lib.Helper.IsStringEmpty(this.GroupHeader.RegexFLR)) && (ForestNET.Lib.Helper.MatchesRegex(s_line, this.GroupHeader.RegexFLR ?? "")) ||
                            (this.GroupHeader.KnownLengthFLR >= 0) && (s_line.Length == this.GroupHeader.KnownLengthFLR)
                        )
                    )
                {
                    e_readState = ReadState.GROUPHEADER;
                }
                else if ((this.GroupFooter != null) &&
                    (
                            (!ForestNET.Lib.Helper.IsStringEmpty(this.GroupFooter.RegexFLR)) && (ForestNET.Lib.Helper.MatchesRegex(s_line, this.GroupFooter.RegexFLR ?? "")) ||
                            (this.GroupFooter.KnownLengthFLR >= 0) && (s_line.Length == this.GroupFooter.KnownLengthFLR)
                        )
                    )
                { /* check if we have a line for group footer */
                    e_readState = ReadState.GROUPFOOTER;
                }
                else
                { /* check for other flr types */
                    bool b_foundFLR = false;

                    /* iterate all existing flr types */
                    foreach (FLRType o_flrType in this.FLRTypes)
                    {
                        /* regex recognition is not successful -> skip */
                        if ((!ForestNET.Lib.Helper.IsStringEmpty(o_flrType.RegexFLR)) && (!ForestNET.Lib.Helper.MatchesRegex(s_line, o_flrType.RegexFLR ?? "")))
                        {
                            continue;
                        }

                        /* overall line length is not successful -> skip */
                        if ((o_flrType.KnownLengthFLR >= 0) && (s_line.Length != o_flrType.KnownLengthFLR))
                        {
                            continue;
                        }

                        /* memorize found flr type */
                        o_foundFLRType = o_flrType;
                        b_foundFLR = true;
                    }

                    /* have we found a flr type */
                    if (!b_foundFLR)
                    {
                        throw new ArgumentException("could not parse fixed length record #" + (i_flrNumber + 1) + " within stack #" + (i_stackNumber + 1) + ": line does not match any regex or known length values(" + s_line.Length + ")");
                    }

                    e_readState = ReadState.FLR;
                }

                if (e_readState == ReadState.GROUPHEADER)
                { /* read group header */
                    /* if we have no group footer configured and this is not the first group header(flr number != 0), then we can increase our stack number */
                    if ((this.GroupFooter == null) && (i_flrNumber != 0))
                    {
                        /* increase stack number */
                        i_stackNumber++;
                        /* create a new stack */
                        this.Stacks[i_stackNumber] = new FixedLengthRecordStack();
                        /* reset record number to zero */
                        i_flrNumber = 0;

                        /* clear all unique temp lists of flr types */
                        foreach (FLRType o_flrType in this.FLRTypes)
                        {
                            o_flrType.FLRObject?.ClearUniqueTemp();
                        }
                    }

                    /* read all fields from group header */
                    IFixedLengthRecord o_temp = ReadFileLineRecursive(s_line, e_readState, this.GroupHeader?.FLRObject, i_stackNumber, i_flrNumber, p_b_ignroeUniqueConstraint) ?? throw new NullReferenceException("file line returned null value for group header of stack #" + (i_stackNumber + 1));

                    /* add group header to current stack */
                    this.Stacks[i_stackNumber].GroupHeader = o_temp;
                }
                else if (e_readState == ReadState.FLR)
                { /* read fixed length records */
                    /* read all fields from fixed length record */
                    IFixedLengthRecord o_temp = ReadFileLineRecursive(s_line, e_readState, o_foundFLRType?.FLRObject, i_stackNumber, i_flrNumber, p_b_ignroeUniqueConstraint) ?? throw new NullReferenceException("file line returned null value for fixed line record #" + (i_flrNumber + 1) + " of stack #" + (i_stackNumber + 1));

                    /* add fixed length record to current stack */
                    this.Stacks[i_stackNumber].AddFixedLengthRecord(i_flrNumber, o_temp);
                }
                else if (e_readState == ReadState.GROUPFOOTER)
                { /* read group footer */
                    /* read all fields from group footer */
                    IFixedLengthRecord o_temp = ReadFileLineRecursive(s_line, e_readState, this.GroupFooter?.FLRObject, i_stackNumber, i_flrNumber, p_b_ignroeUniqueConstraint) ?? throw new NullReferenceException("file line returned null value for group footer of stack #" + (i_stackNumber + 1));

                    /* add group footer to current stack */
                    this.Stacks[i_stackNumber].GroupFooter = o_temp;

                    /* increase stack number, because we read group footer - thus we closed a stack */
                    i_stackNumber++;
                    /* create a new stack */
                    this.Stacks[i_stackNumber] = new FixedLengthRecordStack();
                    /* reset record number to zero */
                    i_flrNumber = 0;

                    /* clear all unique temp lists of flr types */
                    foreach (FLRType o_flrType in this.FLRTypes)
                    {
                        o_flrType.FLRObject?.ClearUniqueTemp();
                    }
                }

                /* increase fixed length record number */
                i_flrNumber++;
            }

            /* check if last stack is completly empty, because if the last line is a group footer, we might have create a new empty stack */
            if ((this.Stacks[i_stackNumber].GroupHeader == null) && (this.Stacks[i_stackNumber].GroupFooter == null) && (this.Stacks[i_stackNumber].FLRs.Count < 1))
            {
                /* remove last stack */
                this.Stacks.Remove(i_stackNumber);
            }
        }

        /// <summary>
        /// handling a file line and read it as group header, footer or normal flr line
        /// </summary>
        /// <param name="p_s_line">flr line</param>
        /// <param name="p_e_readState">which type of line we want to read</param>
        /// <param name="p_o_flrType">flr object</param>
        /// <param name="p_i_stackNumber">which flr file stack we are processing</param>
        /// <param name="p_i_flrNumber">which record within a stack we are processing</param>
        /// <param name="p_b_ignroeUniqueConstraint">true - ignore unique constraint of unique fields and it's values, false - exception will be thrown if unique constraint has been violated</param>
        /// <returns>fixed length record object which can be added to a stack</returns>
        /// <exception cref="ArgumentException">line length does not match known overall length of fixed length record</exception>
        /// <exception cref="InvalidCastException">we could not parse a value to predicted field object type</exception>
        /// <exception cref="System.Reflection.TargetInvocationException">if the underlying constructor throws an exception</exception>
        /// <exception cref="MissingFieldException">field does not exist</exception>
        /// <exception cref="FieldAccessException">cannot access field, must be public</exception>
        /// <exception cref="MissingMethodException">could not retrieve declared constructor</exception>
        /// <exception cref="TypeLoadException">class cannot be located</exception>
        /// <exception cref="InvalidOperationException">field value for unique field already exists</exception>
        private static IFixedLengthRecord? ReadFileLineRecursive(string p_s_line, ReadState p_e_readState, IFixedLengthRecord? p_o_flrType, int p_i_stackNumber, int p_i_flrNumber, bool p_b_ignroeUniqueConstraint)
        {
            IFixedLengthRecord? o_temp = null;

            /* check if parameter for flr type is not null */
            if (p_o_flrType == null)
            {
                throw new ArgumentException("Parameter for fixed length record type is null");
            }

            /* read group header */
            if (p_e_readState == ReadState.GROUPHEADER)
            {
                try
                {
                    /* read all field values from string line */
                    o_temp = p_o_flrType.ReadFieldsFromString(p_s_line);
                }
                catch (InvalidOperationException o_exc)
                {
                    if (!p_b_ignroeUniqueConstraint)
                    {
                        throw new InvalidOperationException("could not parse group header of stack #" + (p_i_stackNumber + 1) + ": " + o_exc);
                    }
                    else
                    {
                        ForestNET.Lib.Global.ILog("could not parse group header of stack #" + (p_i_stackNumber + 1) + ": " + o_exc);
                    }
                }
            }
            else if (p_e_readState == ReadState.FLR)
            { /* read fixed length records */
                try
                {
                    /* read all field values from string line */
                    o_temp = p_o_flrType.ReadFieldsFromString(p_s_line);
                }
                catch (InvalidOperationException o_exc)
                {
                    if (!p_b_ignroeUniqueConstraint)
                    {
                        throw new InvalidOperationException("could not parse fixed length record #" + (p_i_flrNumber + 1) + " within stack #" + (p_i_stackNumber + 1) + ": " + o_exc);
                    }
                    else
                    {
                        ForestNET.Lib.Global.ILog("could not parse fixed length record #" + (p_i_flrNumber + 1) + " within stack #" + (p_i_stackNumber + 1) + ": " + o_exc);
                    }
                }
            }
            else if (p_e_readState == ReadState.GROUPFOOTER)
            { /* read group footer */
                try
                {
                    /* read all field values from string line */
                    o_temp = p_o_flrType.ReadFieldsFromString(p_s_line);
                }
                catch (InvalidOperationException o_exc)
                {
                    if (!p_b_ignroeUniqueConstraint)
                    {
                        throw new InvalidOperationException("could not parse group footer of stack #" + (p_i_stackNumber + 1) + ": " + o_exc);
                    }
                    else
                    {
                        ForestNET.Lib.Global.ILog("could not parse group footer of stack #" + (p_i_stackNumber + 1) + ": " + o_exc);
                    }
                }
            }

            return o_temp;
        }

        /// <summary>
        /// method to write a fixed length record file, with UTF-8 BOM
        /// </summary>
        /// <param name="p_s_file">full-path to flr file</param>
        /// <exception cref="ArgumentException">flr file already exists</exception>
        /// <exception cref="System.IO.IOException">cannot access or create flr file and it's content</exception>
        /// <exception cref="MissingFieldException">field does not exist</exception>
        /// <exception cref="FieldAccessException">cannot access field, must be public</exception>
        /// <exception cref="InvalidOperationException">unique constraint violation</exception>
        public void WriteFile(string p_s_file)
        {
            this.WriteFile(p_s_file, System.Text.Encoding.GetEncoding(ForestNET.Lib.IO.File.ENCODING));
        }

        /// <summary>
        /// method to write a fixed length record file
        /// </summary>
        /// <param name="p_s_file">full-path to flr file</param>
        /// <param name="p_o_encoding">which encoding will be used accessing/modifying the file content</param>
        /// <exception cref="ArgumentException">flr file already exists</exception>
        /// <exception cref="System.IO.IOException">cannot access or create flr file and it's content</exception>
        /// <exception cref="MissingFieldException">field does not exist</exception>
        /// <exception cref="FieldAccessException">cannot access field, must be public</exception>
        /// <exception cref="InvalidOperationException">unique constraint violation</exception>
        public void WriteFile(string p_s_file, System.Text.Encoding p_o_encoding)
        {
            /* we must check for unique constraint violations */
            int i_stackNumber = 1;

            /* iterate each flr stack */
            foreach (FixedLengthRecordStack o_flrStack in this.Stacks.Values)
            {
                /* check if group header is available */
                if (o_flrStack.GroupHeader != null)
                {
                    /* check for unique constraints with current group header */
                    this.WriteCheckUniqueConstraints(o_flrStack.GroupHeader, i_stackNumber, true);
                }

                /* check if group footer is available */
                if (o_flrStack.GroupFooter != null)
                {
                    /* check for unique constraints with current group footer */
                    this.WriteCheckUniqueConstraints(o_flrStack.GroupFooter, i_stackNumber, false);
                }

                i_stackNumber++;
            }

            /* check if file exists */
            if (File.Exists(p_s_file))
            {
                throw new ArgumentException("File[" + p_s_file + "] does exist.");
            }

            /* create flr file */
            File o_file = new(p_s_file, true, p_o_encoding, this.s_lineBreak);

            /* list of lines */
            List<string> a_lines = [];

            /* iterate each stack in current flr file object */
            foreach (KeyValuePair<int, FixedLengthRecordStack> o_stack in this.Stacks)
            {
                /* check if group header is available */
                if (o_stack.Value.GroupHeader != null)
                {
                    /* add group header as line */
                    a_lines.Add(o_stack.Value.GroupHeader.WriteFieldsToString());
                }

                /* iterate each fixed length record of current stack */
                foreach (IFixedLengthRecord o_foo in o_stack.Value.FLRs.Values)
                {
                    /* check if flr is available */
                    if (o_foo != null)
                    {
                        /* add flr as line */
                        a_lines.Add(o_foo.WriteFieldsToString());
                    }
                }

                /* check if group footer is available */
                if (o_stack.Value.GroupFooter != null)
                {
                    /* add group footer as line */
                    a_lines.Add(o_stack.Value.GroupFooter.WriteFieldsToString());
                }
            }

            /* write lines to flr file */
            o_file.FileContentFromList = a_lines;
        }

        /// <summary>
        /// We must check that we do not violate a unique constraint with group headers or group footers
        /// </summary>
        /// <param name="p_o_flr">fixed length record of group header or group footer</param>
        /// <param name="p_i_stackNumber">origin stack number of header or footer</param>
        /// <param name="p_b_checkHeader">true - compare group headers, false - compare group footers</param>
        /// <exception cref="MissingFieldException">field does not exist</exception>
        /// <exception cref="FieldAccessException">cannot access field, must be public</exception>
        /// <exception cref="InvalidOperationException">unique constraint violation</exception>
        /// <exception cref="ArgumentException">parameter stack number must be at least '1'</exception>
        private void WriteCheckUniqueConstraints(IFixedLengthRecord p_o_flr, int p_i_stackNumber, bool p_b_checkHeader)
        {
            /* check parameter stack number */
            if (p_i_stackNumber < 1)
            {
                throw new ArgumentException("Parameter stack number must be at least '1', not lower");
            }

            /* iterate each unique key */
            foreach (string s_unique in p_o_flr.Unique)
            {
                /* it is possible that a unique constraint exists of multiple columns, separated by semicolon */
                if (s_unique.Contains(';'))
                {
                    string[] a_uniques = s_unique.Split(';');

                    int i_stackNumber = 1;

                    /* iterate each flr stack */
                    foreach (FixedLengthRecordStack o_flrStack in this.Stacks.Values)
                    {
                        /* skip own stack */
                        if (i_stackNumber == p_i_stackNumber)
                        {
                            continue;
                        }

                        /* get group header or group footer */
                        IFixedLengthRecord? o_flr = (p_b_checkHeader) ? o_flrStack.GroupHeader : o_flrStack.GroupFooter;

                        /* check if both records are the same type */
                        if (p_o_flr.GetType() == o_flr?.GetType())
                        {
                            bool b_allAreEqual = true;

                            /* iterate each unique key field */
                            for (int i = 0; i < a_uniques.Length; i++)
                            {
                                /* get unique field values */
                                Object? o_one = p_o_flr.GetFieldValue(a_uniques[i]);
                                Object? o_two = o_flr.GetFieldValue(a_uniques[i]);

                                /* compare both unique field values */
                                if (o_flr.AllowEmptyUniqueFields)
                                {
                                    /* field value is null or an empty string */
                                    if (((o_one == null) || (o_one.ToString()?.Trim().Length < 1)) && ((o_two == null) || (o_two.ToString()?.Trim().Length < 1)))
                                    {
                                        b_allAreEqual = false;
                                    }

                                    try
                                    {
                                        /* field value can be parsed to int and is equal to zero */
                                        if ((Convert.ToInt32((o_one?.ToString() ?? "")) == 0) && (Convert.ToInt32((o_two?.ToString() ?? "")) == 0))
                                        {
                                            b_allAreEqual = false;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        /* nothing to do */
                                    }

                                    if (b_allAreEqual)
                                    {
                                        b_allAreEqual = ((o_one != null) && (o_one.Equals(o_two)));
                                    }
                                }
                                else if (!(((o_one == null) && (o_two == null)) || ((o_one != null) && (o_one.Equals(o_two)))))
                                {
                                    b_allAreEqual = false;
                                }
                            }

                            if (b_allAreEqual)
                            {
                                /* unique fields are equal */
                                throw new InvalidOperationException("Unique constraint issue: values for fields '" + s_unique + "' in stack #" + p_i_stackNumber + " already exists within another stack (#" + i_stackNumber + ")");
                            }
                        }

                        i_stackNumber++;
                    }
                }
                else
                {
                    int i_stackNumber = 1;

                    /* iterate each flr stack */
                    foreach (FixedLengthRecordStack o_flrStack in this.Stacks.Values)
                    {
                        /* skip own stack */
                        if (i_stackNumber == p_i_stackNumber)
                        {
                            continue;
                        }

                        /* get group header or group footer */
                        IFixedLengthRecord? o_flr = (p_b_checkHeader) ? o_flrStack.GroupHeader : o_flrStack.GroupFooter;

                        /* check if both records are the same type */
                        if (p_o_flr.GetType() == o_flr?.GetType())
                        {
                            /* get unique field values */
                            Object? o_one = p_o_flr.GetFieldValue(s_unique);
                            Object? o_two = o_flr.GetFieldValue(s_unique);

                            /* compare both unique field values */
                            if (o_flr.AllowEmptyUniqueFields)
                            {
                                bool b_allAreEqual = true;

                                /* field value is null or an empty string */
                                if (((o_one == null) || (o_one.ToString()?.Trim().Length < 1)) && ((o_two == null) || (o_two.ToString()?.Trim().Length < 1)))
                                {
                                    b_allAreEqual = false;
                                }

                                try
                                {
                                    /* field value can be parsed to int and is equal to zero */
                                    if ((Convert.ToInt32((o_one?.ToString() ?? "")) == 0) && (Convert.ToInt32((o_two?.ToString() ?? "")) == 0))
                                    {
                                        b_allAreEqual = false;
                                    }
                                }
                                catch (Exception)
                                {
                                    /* nothing to do */
                                }

                                if ((b_allAreEqual) && ((o_one != null) && (o_one.Equals(o_two))))
                                {
                                    /* unique field values are equal */
                                    throw new InvalidOperationException("Unique constraint issue: value for field '" + s_unique + "' in stack #" + p_i_stackNumber + " already exists within another record (#" + i_stackNumber + ")");
                                }
                            }
                            else if (((o_one == null) && (o_two == null)) || ((o_one != null) && (o_one.Equals(o_two))))
                            {
                                /* unique field values are equal */
                                throw new InvalidOperationException("Unique constraint issue: value for field '" + s_unique + "' in stack #" + p_i_stackNumber + " already exists within another record (#" + i_stackNumber + ")");
                            }
                        }

                        i_stackNumber++;
                    }
                }
            }
        }

        /* Internal Classes */

        /// <summary>
        /// Enumeration for read state of a fixed length record file
        /// </summary>
        private enum ReadState
        {
            GROUPHEADER, FLR, GROUPFOOTER
        }

        /// <summary>
        /// Internal class to encapsulate a fixed length record object with regex recognition property and/or overall line length property
        /// </summary>
        public class FLRType
        {

            /* Fields */

            private string? s_regexFLR = null;

            /* Properties */

            public IFixedLengthRecord? FLRObject = null;
            public string? RegexFLR
            {
                get { return this.s_regexFLR; }
                set
                {
                    this.s_regexFLR = null;

                    /* check if regex recognition is valid */
                    if (!ForestNET.Lib.Helper.IsStringEmpty(value))
                    {
                        try
                        {
                            System.Text.RegularExpressions.Regex o_regex = new(value ?? "");
                        }
                        catch (Exception o_exc)
                        {
                            throw new ArgumentException("Regex for recognizing group footer is invalid: " + o_exc);
                        }

                        this.s_regexFLR = value;
                    }
                }
            }
            public int KnownLengthFLR = -1;


            /* Methods */

            /// <summary>
            /// Fixed length record type constructor
            /// </summary>
            /// <param name="p_o_groupHeader">flr object</param>
            /// <param name="p_s_regexGroupHeader">regex recognition</param>
            /// <exception cref="ArgumentException">parameter is null, at least a regex recognition or an overall line length parameter, regex for recognizing flr is invalid</exception>
            public FLRType(IFixedLengthRecord? p_o_flrObject, string p_s_regexFLR) :
                this(p_o_flrObject, p_s_regexFLR, -1)
            {

            }

            /// <summary>
            /// Fixed length record type constructor
            /// </summary>
            /// <param name="p_o_groupHeader">flr object</param>
            /// <param name="p_s_regexGroupHeader">regex recognition</param>
            /// <param name="p_i_knownLengthGroupHeader">overall line length of flr</param>
            /// <exception cref="ArgumentException">parameter is null, at least a regex recognition or an overall line length parameter, regex for recognizing flr is invalid</exception>
            public FLRType(IFixedLengthRecord? p_o_flrObject, string? p_s_regexFLR, int p_i_knownLengthFLR)
            {
                /* flr object must not be null */
                if (p_o_flrObject == null)
                {
                    throw new ArgumentException("Parameter for fixed length record object is null");
                }

                /* we must have at least a regex recognition or an overall line length parameter */
                if ((ForestNET.Lib.Helper.IsStringEmpty(p_s_regexFLR)) && (p_i_knownLengthFLR < 0))
                {
                    throw new ArgumentException("Parameter for recognizing fixed length record and parameter for known length of flr is null or empty");
                }

                this.FLRObject = p_o_flrObject;
                this.RegexFLR = p_s_regexFLR;
                this.KnownLengthFLR = p_i_knownLengthFLR;

                /* clear unique temp map */
                this.FLRObject.ClearUniqueTemp();
            }
        }

        /// <summary>
        /// Internal class to encapsulate a stack fixed length record objects, group header and group footer
        /// </summary>
        public class FixedLengthRecordStack
        {

            /* Fields */

            /* Properties */

            public IFixedLengthRecord? GroupHeader = null;
            public Dictionary<int, IFixedLengthRecord> FLRs = [];
            public IFixedLengthRecord? GroupFooter = null;

            /* Methods */

            /// <summary>
            /// Fixed length record stack constructor
            /// </summary>
            public FixedLengthRecordStack()
            {

            }

            /// <summary>
            /// Add fixed length record object to internal list of flr stack object
            /// </summary>
            /// <param name="p_i_key">key number of fixed length record</param>
            /// <param name="p_o_flr">fixed length record object</param>
            /// <exception cref="MissingFieldException">field does not exist</exception>
            /// <exception cref="FieldAccessException">cannot access field, must be public</exception>
            /// <exception cref="InvalidOperationException">unique constraint violation</exception>
            /// <exception cref="ArgumentException">parameter flr key number must be at least '0'</exception>
            public void AddFixedLengthRecord(int p_i_key, IFixedLengthRecord p_o_flr)
            {
                /* check parameter flr key number */
                if (p_i_key < 0)
                {
                    throw new ArgumentException("Parameter flr key number must be at least '0', positive number");
                }

                /* we must check that we do not violate a unique constraint within flr list */

                /* iterate each unique key */
                foreach (string s_unique in p_o_flr.Unique)
                {
                    /* it is possible that a unique constraint exists of multiple columns, separated by semicolon */
                    if (s_unique.Contains(';'))
                    {
                        string[] a_uniques = s_unique.Split(';');

                        int i_recordNumber = 1;

                        /* iterate each flr in current stack */
                        foreach (IFixedLengthRecord o_flr in this.FLRs.Values)
                        {
                            /* check if both records are the same type */
                            if (p_o_flr.GetType() == o_flr.GetType())
                            {
                                bool b_allAreEqual = true;

                                /* iterate each unique key field */
                                for (int i = 0; i < a_uniques.Length; i++)
                                {
                                    /* get unique field values */
                                    Object? o_one = p_o_flr.GetFieldValue(a_uniques[i]);
                                    Object? o_two = o_flr.GetFieldValue(a_uniques[i]);

                                    /* compare both unique field values */
                                    if (o_flr.AllowEmptyUniqueFields)
                                    {
                                        /* field value is null or an empty string */
                                        if (((o_one == null) || (o_one.ToString()?.Trim().Length < 1)) && ((o_two == null) || (o_two.ToString()?.Trim().Length < 1)))
                                        {
                                            b_allAreEqual = false;
                                        }

                                        try
                                        {
                                            /* field value can be parsed to int and is equal to zero */
                                            if ((Convert.ToInt32((o_one?.ToString() ?? "")) == 0) && (Convert.ToInt32((o_two?.ToString() ?? "")) == 0))
                                            {
                                                b_allAreEqual = false;
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            /* nothing to do */
                                        }

                                        if (b_allAreEqual)
                                        {
                                            b_allAreEqual = ((o_one != null) && (o_one.Equals(o_two)));
                                        }
                                    }
                                    else if (!(((o_one == null) && (o_two == null)) || ((o_one != null) && (o_one.Equals(o_two)))))
                                    {
                                        b_allAreEqual = false;
                                    }
                                }

                                if (b_allAreEqual)
                                {
                                    /* unique fields are equal */
                                    throw new InvalidOperationException("Unique constraint issue: values for fields '" + s_unique + "' in record #" + (p_i_key + 1) + " already exists within another record (#" + i_recordNumber + ")");
                                }
                            }

                            i_recordNumber++;
                        }
                    }
                    else
                    {
                        int i_recordNumber = 1;

                        /* iterate each flr in current stack */
                        foreach (IFixedLengthRecord o_flr in this.FLRs.Values)
                        {
                            /* check if both records are the same type */
                            if (p_o_flr.GetType() == o_flr.GetType())
                            {
                                /* get unique field values */
                                Object? o_one = p_o_flr.GetFieldValue(s_unique);
                                Object? o_two = o_flr.GetFieldValue(s_unique);

                                /* compare both unique field values */
                                if (o_flr.AllowEmptyUniqueFields)
                                {
                                    bool b_allAreEqual = true;

                                    /* field value is null or an empty string */
                                    if (((o_one == null) || (o_one.ToString()?.Trim().Length < 1)) && ((o_two == null) || (o_two.ToString()?.Trim().Length < 1)))
                                    {
                                        b_allAreEqual = false;
                                    }

                                    try
                                    {
                                        /* field value can be parsed to int and is equal to zero */
                                        if ((Convert.ToInt32((o_one?.ToString() ?? "")) == 0) && (Convert.ToInt32((o_two?.ToString() ?? "")) == 0))
                                        {
                                            b_allAreEqual = false;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        /* nothing to do */
                                    }

                                    if ((b_allAreEqual) && ((o_one != null) && (o_one.Equals(o_two))))
                                    {
                                        /* unique field values are equal */
                                        throw new InvalidOperationException("Unique constraint issue: value for field '" + s_unique + "' in record #" + (p_i_key + 1) + " already exists within another record (#" + i_recordNumber + ")");
                                    }
                                }
                                else if (((o_one == null) && (o_two == null)) || ((o_one != null) && (o_one.Equals(o_two))))
                                {
                                    /* unique field values are equal */
                                    throw new InvalidOperationException("Unique constraint issue: value for field '" + s_unique + "' in record #" + (p_i_key + 1) + " already exists within another record (#" + i_recordNumber + ")");
                                }
                            }

                            i_recordNumber++;
                        }
                    }
                }

                this.FLRs.Add(p_i_key, p_o_flr);
            }
        }
    }
}